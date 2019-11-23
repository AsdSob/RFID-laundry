using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Threading;
using PALMS.ViewModels.Common;

namespace PALMS.View.Common.Behaviors
{
    public class InitializationAsyncBehavior : Behavior<FrameworkElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.Loaded += AssociatedObjectOnLoaded;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Loaded -= AssociatedObjectOnLoaded;

            base.OnDetaching();
        }

        private void AssociatedObjectOnLoaded(object sender, RoutedEventArgs e)
        {
            if (!(AssociatedObject.DataContext is IInitializationAsync viewModel))
                return;

            Dispatcher.BeginInvoke(DispatcherPriority.Loaded, (Action) (() =>
            {
                Task.Factory.StartNew(async () => await viewModel.InitializeAsync());
            }));
        }
    }
}
