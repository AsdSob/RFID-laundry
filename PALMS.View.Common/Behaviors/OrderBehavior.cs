using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Interactivity;
using DevExpress.Xpf.Grid;
using PALMS.ViewModels.Common.Interfaces;

namespace PALMS.View.Common.Behaviors
{
    public class OrderBehavior : Behavior<GridControl>
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
            {
                oldSource.CollectionChanged -= SourceOnCollectionChanged;
            }

            if (e.NewItemsSource is INotifyCollectionChanged newSource)
            {
                newSource.CollectionChanged += SourceOnCollectionChanged;
                Reorder();
            }
        }

        private void SourceOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Reorder();
        }

        private void Reorder()
        {
            if (!(AssociatedObject.ItemsSource is IEnumerable items)) return;

            var orderNumber = 1;

            foreach (var item in  items.OfType<IOrderItem>())
            {
                item.OrderNumber = orderNumber++;
            }
        }
    }
}
