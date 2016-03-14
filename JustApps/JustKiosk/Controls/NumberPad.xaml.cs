using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace JustKiosk.Controls
{
    public sealed partial class NumberPad : UserControl
    {
        public NumberPad()
        {
            InitializeComponent();
        }

        public string Pin
        {
            get { return (string)GetValue(PinProperty); }
            set { SetValue(PinProperty, value); }
        }
        public static readonly DependencyProperty PinProperty =
            DependencyProperty.Register("Pin", typeof(string), typeof(NumberPad), new PropertyMetadata(string.Empty));

        public void Add1() { if (Pin.Length < 4) Pin += "1"; }
        public void Add2() { if (Pin.Length < 4) Pin += "2"; }
        public void Add3() { if (Pin.Length < 4) Pin += "3"; }
        public void Add4() { if (Pin.Length < 4) Pin += "4"; }
        public void Add5() { if (Pin.Length < 4) Pin += "5"; }
        public void Add6() { if (Pin.Length < 4) Pin += "6"; }
        public void Add7() { if (Pin.Length < 4) Pin += "7"; }
        public void Add8() { if (Pin.Length < 4) Pin += "8"; }
        public void Add9() { if (Pin.Length < 4) Pin += "9"; }
        public void Add0() { if (Pin.Length < 4) Pin += "0"; }

        public void Clear() { Pin = string.Empty; }
        public void Submit() { OnSubmit?.Invoke(this, Pin); }

        public event EventHandler<string> OnSubmit;
    }
}
