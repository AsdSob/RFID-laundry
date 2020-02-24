using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;

namespace Client.Desktop.Views.Behaviors
{
    public class PasswordBoxBehavior : Behavior<PasswordBox>
    {
        public static readonly DependencyProperty PasswordProperty = DependencyProperty.Register(
            "Password", typeof(string), typeof(PasswordBoxBehavior), new PropertyMetadata(string.Empty));

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
        }

        public string Password
        {
            get => (string)GetValue(PasswordProperty);
            set => SetValue(PasswordProperty, value);
        }

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
            Password = AssociatedObject.Password;
        }
    }
}
