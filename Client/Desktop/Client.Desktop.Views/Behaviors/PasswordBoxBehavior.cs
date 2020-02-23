using System.Windows;
using System.Windows.Controls;
using Client.Desktop.ViewModels.Common;
using Microsoft.Xaml.Behaviors;

namespace Client.Desktop.Views.Behaviors
{
    public class PasswordBoxBehavior : Behavior<PasswordBox>
    {
        protected override void OnAttached()
        {
            AssociatedObject.PasswordChanged += OnPasswordChanged; 

            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            AssociatedObject.PasswordChanged -= OnPasswordChanged;

            base.OnDetaching();
        }

        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            var dataContext = AssociatedObject.DataContext as ISelectedItem;
            if (dataContext == null) return;

            var passwordDataContext = dataContext.GetSelected() as IPassword;
            if (passwordDataContext == null) return;

            passwordDataContext.Password = AssociatedObject.Password;
        }
    }
}
