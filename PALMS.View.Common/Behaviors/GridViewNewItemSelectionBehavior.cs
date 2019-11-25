using System;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Interactivity;
using System.Windows.Threading;
using DevExpress.Utils;
using DevExpress.Xpf.Grid;

namespace PALMS.View.Common.Behaviors
{
    /// <summary>
    /// Provides selection of new item.
    /// </summary>
    public class GridViewNewItemSelectionBehavior : Behavior<GridControl>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.ItemsSourceChanged += OnItemsSourceChanged;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.ItemsSourceChanged -= OnItemsSourceChanged;

            base.OnDetaching();
        }

        private void OnItemsSourceChanged(object sender, ItemsSourceChangedEventArgs e)
        {
            if (e.OldItemsSource is INotifyCollectionChanged oldSource)
                oldSource.CollectionChanged -= SourceOnCollectionChanged;

            if (e.NewItemsSource is INotifyCollectionChanged newSource)
                newSource.CollectionChanged += SourceOnCollectionChanged;
        }

        private void SourceOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
            {
                // select new item
                if (e.NewItems.Count > 0)
                {
                    AssociatedObject.SelectedItem = e.NewItems[0];
                    AssociatedObject.CurrentColumn = AssociatedObject.Columns.FirstOrDefault(x => x.AllowEditing != DefaultBoolean.True);

                    AssociatedObject.Focus();

                    Dispatcher.BeginInvoke(new Action(() => AssociatedObject.View.ShowEditor(true)), DispatcherPriority.Loaded);
                }
            }
        }
    }
}