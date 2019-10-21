using System.Windows;
using System.Windows.Interactivity;
using DevExpress.Xpf.Grid;
using PALMS.Settings.ViewModel.Dictionaries.Base;

namespace PALMS.Settings.View.Behaviors
{
    public class DictionaryEditBehavior : Behavior<GridControl>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.DataContextChanged += OnDataContextChanged;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.DataContextChanged -= OnDataContextChanged;

            base.OnDetaching();
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(e.NewValue is IDictionaryViewModel viewModel))
                return;

            viewModel.CancelEditAction = AssociatedObject.View.CancelRowEdit;
        }
    }
}
