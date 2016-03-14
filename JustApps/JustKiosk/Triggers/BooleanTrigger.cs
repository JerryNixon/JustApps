using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace JustKiosk.Triggers
{
    public class BooleanTrigger : StateTriggerBase
    {
        public bool Value
        {
            get { return (bool)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(bool), typeof(BooleanTrigger), new PropertyMetadata(false, ValueChanged));
        private static void ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var trigger = d as BooleanTrigger;
            trigger.SetActive(e.NewValue.Equals(trigger.When));
        }

        public bool When
        {
            get { return (bool)GetValue(WhenProperty); }
            set { SetValue(WhenProperty, value); }
        }
        public static readonly DependencyProperty WhenProperty =
            DependencyProperty.Register(nameof(When), typeof(bool), typeof(BooleanTrigger), new PropertyMetadata(false));
    }
}
