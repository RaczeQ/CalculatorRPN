using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Calculator
{
    public sealed partial class MainPage : Page
    {
        private static string _inputString = "";
        private static readonly string ErrorText = "Given input is an invalid RPN equation!";
        private static readonly SolidColorBrush Black = new SolidColorBrush(Colors.Black);
        private static readonly SolidColorBrush Red = new SolidColorBrush(Colors.Red);
        public MainPage()
        {
            this.InitializeComponent();
        }

        public void ReadInput()
        {
            _inputString = InputTextBox.Text;
        }

        public void WriteInput()
        {
            ClearError();
            InputTextBox.Text = _inputString;
        }

        public void ClearError()
        {
            InputTextBox.Foreground = Black;
            ErrorBlock.Text = "";
        }

        public void SaveLastOperation(string input, string value)
        {
            HistoryBlock.Text = $"Last operation: { input } = { value }";
        }

        public void SetError(string errMsg)
        {
            InputTextBox.Foreground = Red;
            ErrorBlock.Text = $"{ ErrorText } ({ errMsg })";
        }

        private void OnKeyDownHandler(object sender, KeyRoutedEventArgs e)
        {
            ClearError();
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                CountValue();
            }
        }

        private void AddNumber(object sender, RoutedEventArgs e)
        {
            ReadInput();
            _inputString += ((Button)sender).Tag;
            WriteInput();
        }

        private void AddSign(object sender, RoutedEventArgs e)
        {
            ReadInput();
            _inputString += ((Button)sender).Tag;
            WriteInput();
        }

        private void Backspace(object sender, RoutedEventArgs e)
        {
            ReadInput();
            if (_inputString.Length <= 0) return;
            _inputString = _inputString.Substring(0, _inputString.Length - 1);
            WriteInput();
        }

        private void Count(object sender, RoutedEventArgs e)
        {
            CountValue();
        }

        private void CountValue()
        {
            ReadInput();
            var inputList = _inputString.Split(' ');
            Stack<double> st = new Stack<double>();
            foreach (var input in inputList)
            {
                if (double.TryParse(input, out double number))
                {
                    st.Push(number);
                }
                else
                {
                    if (st.Count < 2)
                    {
                        SetError("Stack has invalid amount of numbers");
                        return;
                    }
                    double d1 = st.Pop(), d2 = st.Pop();
                    switch (input)
                    {
                        case "+":
                            st.Push(d2 + d1);
                            break;
                        case "-":
                            st.Push(d2 - d1);
                            break;
                        case "*":
                            st.Push(d2 * d1);
                            break;
                        case "/":
                            st.Push(d2 / d1);
                            break;
                        default:
                            SetError($"Couldn't parse '{ input }'");
                            return;
                    }
                }
            }
            if (st.Count > 1)
            {
                SetError("Stack has invalid amount of symbols");
                return;
            }
            var value = st.Pop().ToString();
            SaveLastOperation(_inputString, value);
            _inputString = value;
            WriteInput();
        }
    }
}
