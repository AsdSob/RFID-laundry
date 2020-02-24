using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;

namespace Client.Desktop.Views.Behaviors
{
    public class DefaultFocusBehavior : Behavior<FrameworkElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.Loaded += OnLoaded;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Loaded -= OnLoaded;

            base.OnDetaching();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            AssociatedObject.Focus();
        }
    }

    public class PasswordBoxBehavior : Behavior<PasswordBox>
    {
        public static readonly DependencyProperty PasswordProperty = DependencyProperty.Register(
            "Password", typeof(string), typeof(PasswordBoxBehavior), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty ResetObjectProperty = DependencyProperty.Register("ResetObject", typeof(object), typeof(PasswordBoxBehavior), new PropertyMetadata(default(object), ResetObjectPropertyChangedCallback));

        public string Password
        {
            get => (string)GetValue(PasswordProperty);
            set => SetValue(PasswordProperty, value);
        }

        public object ResetObject
        {
            get { return GetValue(ResetObjectProperty); }
            set { SetValue(ResetObjectProperty, value); }
        }

        protected override void OnAttached()
        {
            AssociatedObject.PasswordChanged += OnPasswordChanged;
            AssociatedObject.Loaded += OnLoaded; 

            base.OnAttached();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Password = null;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.PasswordChanged -= OnPasswordChanged;
            AssociatedObject.Loaded -= OnLoaded;

            base.OnDetaching();
        }

        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            Password = AssociatedObject.Password;
        }

        private static void ResetObjectPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is PasswordBoxBehavior behavior)) return;

            behavior.AssociatedObject.Password = string.Empty;
        }
    }
}
