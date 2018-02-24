using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        //Enumeration Type - defined set of named integral constants
        public enum Operator { None, Plus, Minus, Times, Divide, Equals }

        //private class variables
        private Operator lastOperator = Operator.None;
        private decimal valueSoFar = 0;
        private bool numberHitSinceLastOperator = false;

        private double a = 0;
        private double b = 0;


        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            buttonMul.Tag = Operator.Times;
            buttonDiv.Tag = Operator.Divide;
            buttonSub.Tag = Operator.Minus;
            buttonAdd.Tag = Operator.Plus;
            buttonEquals.Tag = Operator.Equals;
        }

        public MainWindow()//gjhfjhf
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            HandleDigit(1);
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            HandleDigit(2);
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            HandleDigit(3);
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            HandleDigit(4);
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            HandleDigit(5);
        }

        private void button6_Click(object sender, RoutedEventArgs e)
        {
            HandleDigit(6);
        }

        private void button7_Click(object sender, RoutedEventArgs e)
        {
            HandleDigit(7);
        }

        private void button8_Click(object sender, RoutedEventArgs e)
        {
            HandleDigit(8);
        }

        private void button9_Click(object sender, RoutedEventArgs e)
        {
            HandleDigit(9);
        }


        private void HandleDigit(int digit)
        {
            //Ternary Operator
            //assign value = condition evaluate ? True (value) : False (value)
            //valueSoFar and newValue are local variables
            string valueSoFar = numberHitSinceLastOperator ? textBoxDisplay.Text : "";
            string newValue = valueSoFar + digit.ToString();
            textBoxDisplay.Text = newValue;
            numberHitSinceLastOperator = true;

        }

        private void OnClickOperator(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn.Tag != null)
            {
                Operator op = (Operator)btn.Tag;
                ExecuteLastOperator(op);
            }
        }

        
        private void ExecuteLastOperator(Operator newOperator)
        {
            decimal currentValue = Convert.ToDecimal(textBoxDisplay.Text);
            decimal newValue = currentValue;
            if (numberHitSinceLastOperator)
            {
                switch (lastOperator)
                {
                    case Operator.Plus:
                        newValue = valueSoFar + currentValue;
                        break;
                    case Operator.Minus:
                        newValue = valueSoFar - currentValue;
                        break;
                    case Operator.Times:
                        newValue = valueSoFar * currentValue;
                        break;
                    case Operator.Divide:
                        if (currentValue == 0)
                            newValue = 0;
                        else
                            newValue = valueSoFar / currentValue;
                        break;
                    case Operator.Equals:
                        newValue = currentValue;
                        break;
                }
            }
            valueSoFar = newValue;
            lastOperator = newOperator;
            numberHitSinceLastOperator = false;
            textBoxDisplay.Text = valueSoFar.ToString();
        }

        private void buttonC_Click(object sender, RoutedEventArgs e)
        {
            textBoxDisplay.Clear();
        }

        private void buttonBack_Click(object sender, RoutedEventArgs e)
        {
            String box = textBoxDisplay.Text;
            if(box.Length >= 1)
            {
                box = box.Substring(0, box.Length - 1);
                textBoxDisplay.Text = box;
            }
        
        
        }

        private void buttonMSign_Click(object sender, RoutedEventArgs e)
        {
            textBoxDisplay.Text = (Double.Parse(textBoxDisplay.Text) * -1).ToString();
        }

        private void buttonCE_Click(object sender, RoutedEventArgs e)
        {
            textBoxA.Clear();
            textBoxB.Clear();
            textBoxC.Clear();
            textBoxDisplay.Clear();
        }

       

        private void buttonSetA_Click(object sender, RoutedEventArgs e)
        {
            textBoxC.Clear();//Clear error message if a or b not set
            if (textBoxDisplay.Text != "")
            {
                a = Convert.ToDouble(textBoxDisplay.Text);
                textBoxA.Text = Convert.ToString(a);
                textBoxDisplay.Clear();
            }
        }

        private void buttonSetB_Click(object sender, RoutedEventArgs e)
        {
            textBoxC.Clear();
            if (textBoxDisplay.Text != "")
            {
                b = Convert.ToDouble(textBoxDisplay.Text);
                textBoxB.Text = Convert.ToString(b);
                textBoxDisplay.Clear();
            }
        }

        private void buttonGetC_Click(object sender, RoutedEventArgs e)
        {
            if(a < 0 || b < 0)
            {
                textBoxC.Text = "No neg";
            }
            else if (a == 0 || b == 0)
            {
                textBoxC.Text = "Set A & B";
            }
            else
            {
                textBoxC.Text = Convert.ToString(Math.Sqrt(a * a + b * b));
            }
            a = 0;
            b = 0;
        }

        private void buttonCToF_Click(object sender, RoutedEventArgs e)
        {
            if(textBoxDisplay.Text != "")
            {
                textBoxDisplay.Text = Convert.ToString
                    (Convert.ToDouble(textBoxDisplay.Text) * 1.8 + 32);
            }
        }

        private void buttonFToC_Click(object sender, RoutedEventArgs e)
        {
            textBoxDisplay.Text = Convert.ToString
                    ((Convert.ToDouble(textBoxDisplay.Text) - 32) / 1.8);
        }
    }
}
