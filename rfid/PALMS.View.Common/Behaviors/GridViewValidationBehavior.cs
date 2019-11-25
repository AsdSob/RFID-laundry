using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Interactivity;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Grid.TreeList;

namespace PALMS.View.Common.Behaviors
{
    public class GridViewValidationBehavior : Behavior<GridControl>
    {
        private readonly Dictionary<object, Dictionary<string, string>> _errors = new Dictionary<object, Dictionary<string, string>>();

        public static readonly DependencyProperty IsValidProperty = DependencyProperty.Register(
            "IsValid", typeof(bool), typeof(GridViewValidationBehavior), new PropertyMetadata(default(bool)));

        public bool IsValid
        {
            get => (bool) GetValue(IsValidProperty);
            set => SetValue(IsValidProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.Loaded += OnLoaded;
            AssociatedObject.ItemsSourceChanged += OnItemsSourceChanged;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Loaded -= OnLoaded;
            AssociatedObject.ItemsSourceChanged -= OnItemsSourceChanged;

            if (AssociatedObject.View is TableView tableView)
                tableView.CellValueChanging -= TableOnCellValueChanging;

            base.OnDetaching();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (!(sender is GridControl gridControl)) return;
            if (gridControl.View is TableView tableView)
            {
                tableView.CellValueChanging += TableOnCellValueChanging;
                return;
            }
            if (gridControl.View is TreeListView treeView)
            {
                treeView.CellValueChanging += TreeOnCellValueChanging;
                return;
            }

            throw new Exception($"Grid Validation Behavior can't support {gridControl.View?.GetType().Name}");
        }

        private void OnItemsSourceChanged(object sender, ItemsSourceChangedEventArgs e)
        {
            if (e.OldItemsSource is INotifyCollectionChanged oldSource)
            {
                _errors.Clear();
                oldSource.CollectionChanged -= SourceOnCollectionChanged;
            }

            if (e.NewItemsSource is INotifyCollectionChanged newSource)
            {
                newSource.CollectionChanged += SourceOnCollectionChanged;
                AddItems(newSource as IList);
                Raise();
            }
        }

        private void SourceOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
            {
                AddItems(e.NewItems);
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
            {
                RemoveItems(e.OldItems);
            }

            Raise();
        }

        private void AddItems(IList items)
        {
            foreach (var item in items)
            {
                if (_errors.ContainsKey(item)) continue;

                var columns = new Dictionary<string, string>();
                _errors.Add(item, columns);

                foreach (var column in AssociatedObject.Columns)
                {
                    columns.Add(column.FieldName, null);

                    SetError(item, column);
                }
            }
        }

        private void RemoveItems(IList items)
        {
            foreach (var item in items)
                _errors.Remove(item);
        }

        private void TableOnCellValueChanging(object sender, CellValueChangedEventArgs e)
        {
            SetError(e.Row, e.Column);

            Raise();
        }

        private void TreeOnCellValueChanging(object sender, TreeListCellValueChangedEventArgs e)
        {
            SetError(e.Row, e.Column);

            Raise();
        }

        private void SetError(object row, ColumnBase column)
        {
            if (!(row is IDataErrorInfo dataErrorInfo)) return;

            _errors[row][column.FieldName] = dataErrorInfo[column.FieldName];
        }

        private void Raise()
        {
            IsValid = _errors.All(row => row.Value.All(column => string.IsNullOrEmpty(column.Value)));
        }
    }
}
