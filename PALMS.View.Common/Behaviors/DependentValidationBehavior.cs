using System.Windows;
using System.Windows.Interactivity;
using DevExpress.Xpf.Editors;

namespace PALMS.View.Common.Behaviors
{
    public class DependentValidationBehavior : Behavior<BaseEdit>
    {
        public static readonly DependencyProperty ControlProperty = DependencyProperty.Register(
            "Control", typeof(BaseEdit), typeof(DependentValidationBehavior), new PropertyMetadata(default(BaseEdit)));

        public BaseEdit Control
        {
            get => (BaseEdit) GetValue(ControlProperty);
            set => SetValue(ControlProperty, value);
        }

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
            if (Control == null) return;

            Control.Tag = AssociatedObject;
            Control.EditValueChanged += ControlOnEditValueChanged;
        }

        private static void ControlOnEditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            if (!((sender as BaseEdit)?.Tag is BaseEdit associatedObject)) return;

            associatedObject.GetBindingExpression(BaseEdit.EditValueProperty)?.UpdateSource();
        }
    }
}
