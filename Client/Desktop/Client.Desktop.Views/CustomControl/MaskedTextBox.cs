using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Client.Desktop.ViewModels.Common.Extensions;

namespace Client.Desktop.Views.CustomControl
{
    public class MaskedTextBox : TextBox
    {
        public static Regex NumericRegex = new Regex("[^0-9]+");
        static MaskedTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MaskedTextBox), new FrameworkPropertyMetadata(typeof(MaskedTextBox)));
        }

        public TextBoxMask Mask { get; set; }

        public MaskedTextBox()
        {
            this.TextChanged += new TextChangedEventHandler(MaskedTextBox_TextChanged);
        }

        void MaskedTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.CaretIndex = this.Text.Length;

            var tbEntry = sender as MaskedTextBox;
            var text = "0";

            if (tbEntry != null && tbEntry.Text.Length > 0)
            {
                text = FormatNumber(tbEntry.Text, tbEntry.Mask);

            }

            if (String.IsNullOrWhiteSpace(text))
            {
                text = "0";
            }

            tbEntry.Text = text;
        }

        public static string FormatNumber(string maskedNum, TextBoxMask phoneFormat)
        {
            if (maskedNum != null)
            {
                switch (phoneFormat)
                {
                    case TextBoxMask.Decimal:
                        return FormatDecimal(maskedNum);

                    default:
                        break;
                }

            }
            return "0";
        }

        public static string FormatDecimal(string text)
        {
            if (!NumericRegex.IsMatch(text))
            {
                return text;
            }
            return Regex.Replace(text, "[^0-9]", "");
        }

    }
}
