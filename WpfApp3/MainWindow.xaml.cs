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
using Microsoft.Kinect;
using System.Speech.AudioFormat;
using System.Speech.Recognition;
using System.Speech;
using System.IO;
using System.Threading;
using System.Collections;

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

        //For Pythagorean
        private double a = 0;
        private double b = 0;

        //Fields for memory game
        Random rand = new Random();
        ArrayList nums = new ArrayList();
        ArrayList numsGuessed = new ArrayList();
        bool playing = false;
        bool printing = false;
   
        //Represents the index to be compared in above ArrayLists
        int numGuessing = 0;

        KinectAudioSource _kinectSource;
        KinectSensor _kinectSensor;
        SpeechRecognitionEngine _speechEngine;
        Stream _stream;



        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            buttonMul.Tag = Operator.Times;
            buttonDiv.Tag = Operator.Divide;
            buttonSub.Tag = Operator.Minus;
            buttonAdd.Tag = Operator.Plus;
            buttonEquals.Tag = Operator.Equals;

            //listInstalledRecognizers.ItemsSource =
            //        SpeechRecognitionEngine.InstalledRecognizers();
            //if (listInstalledRecognizers.Items.Count > 0)
            //    listInstalledRecognizers.SelectedItem =
            //        listInstalledRecognizers.Items[0];

            _kinectSensor = KinectSensor.KinectSensors[0];
            _kinectSensor.Start();
        }

        public MainWindow()
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

        private void button0_Click(object sender, RoutedEventArgs e)
        {
            HandleDigit(0);
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
            //avoids exception with no number to operate on.
            if (textBoxDisplay.Text == "")
                return;

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
            if (textBoxDisplay.Text != "")
            {
                textBoxDisplay.Text = (Double.Parse(textBoxDisplay.Text) * -1).ToString();
            }
                    
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
            if (textBoxDisplay.Text != "" && textBoxDisplay.Text != ".")
            {
                a = Convert.ToDouble(textBoxDisplay.Text);
                textBoxA.Text = Convert.ToString(a);
                textBoxDisplay.Clear();
            }
        }

        private void buttonSetB_Click(object sender, RoutedEventArgs e)
        {
            textBoxC.Clear();
            if (textBoxDisplay.Text != "" && textBoxDisplay.Text != ".")
            {
                b = Convert.ToDouble(textBoxDisplay.Text);
                textBoxB.Text = Convert.ToString(b);
                textBoxDisplay.Clear();
            }
        }

        private void buttonGetC_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxA.Text != "" && textBoxB.Text != "")
            {
                double a = Convert.ToDouble(textBoxA.Text);
                double b = Convert.ToDouble(textBoxB.Text);

                if (a < 0 || b < 0)
                {
                    textBoxC.Text = "NO NEG";
                }
                
                else
                {
                    textBoxC.Text = Convert.ToString(Math.Sqrt(a * a + b * b));
                }
            }
            else
            {
                textBoxC.Text = "Set A & B";
            }
            //a = 0;
            //b = 0;
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
            if (textBoxDisplay.Text != "")
            {
                textBoxDisplay.Text = Convert.ToString
                    ((Convert.ToDouble(textBoxDisplay.Text) - 32) / 1.8);
            }
        }

        

        private async void buttonEnter_Click(object sender, RoutedEventArgs e)
        {
            if (playing && !printing)//If not playing or if printing, this button does nothing
            {
                printing = true;
                if (textBoxDisplay.Text != "")//Avoid exception
                {
                    //Add users guess to ArrayList field
                    numsGuessed.Add(Convert.ToDouble(textBoxDisplay.Text));
                    textBoxDisplay.Clear();
                    //if user guess matches correct answer...
                    if (Convert.ToInt32(numsGuessed[numGuessing]) == Convert.ToInt32(nums[numGuessing]))
                    {
                        numGuessing++;
                        memTextBox.Text = "Correct";
                        await Task.Run(() => System.Threading.Thread.Sleep(1000));
                        memTextBox.Clear();
                    }
                    else//Wrong game over
                    { 
                        playing = false;//Enter button now does nothing
                        memTextBox.Text = "Wrong";
                        await Task.Run(() => System.Threading.Thread.Sleep(1500));
                        memTextBox.Text = "Game Over";
                        await Task.Run(() => System.Threading.Thread.Sleep(1500));
                        if(nums.Count == 3)//none guessed right
                            memTextBox.Text = "Score: 0";
                        else
                            memTextBox.Text = "Score: " + (nums.Count - 1);//most nums remembered
                        await Task.Run(() => System.Threading.Thread.Sleep(1500));
                        //Reset these for next game
                        numsGuessed = new ArrayList();
                        nums = new ArrayList();
                        numGuessing = 0;
                        return;//End event
                    }
                    //The last thing Enter button does is print the new list of numbers user needs to remember.
                    if (numGuessing == nums.Count)
                    {
                        textBoxDisplay.Clear();
                        nums.Add(rand.Next(0, 10));//Create one new random for this round.
                        for (int i = 0; i < nums.Count; i++)
                        {
                            memTextBox.Text = Convert.ToString(nums[i]);
                            await Task.Run(() => System.Threading.Thread.Sleep(1000));

                            if (i != nums.Count - 1)//No * after last num
                            {
                                memTextBox.Text = Convert.ToString("*");
                                await Task.Run(() => System.Threading.Thread.Sleep(700));
                            }
                        }
                        memTextBox.Text = Convert.ToString("Your Turn");
                        //Reset these for new round of guesses.
                        numsGuessed = new ArrayList();
                        numGuessing = 0;
                    }
              
                }
                printing = false;//Done printing, enter button now has functionality.
            }
        }

        private async void buttonPlay_Click(object sender, RoutedEventArgs e)
        {
            if (!playing)//button does nothing if playing already
            {
                playing = true;
                printing = true;
                //Start with 3 nums to remember
                nums.Add(rand.Next(0, 10));
                nums.Add(rand.Next(0, 10));
                nums.Add(rand.Next(0, 10));

                //Show these nums to user with time delay and * inbetween
                for (int i = 0; i < nums.Count; i++)
                {
                    memTextBox.Text = Convert.ToString(nums[i]);
                    await Task.Run(() => System.Threading.Thread.Sleep(1500));

                    if (i != nums.Count - 1)//No * after last num
                    {
                        memTextBox.Text = Convert.ToString("*");
                        await Task.Run(() => System.Threading.Thread.Sleep(1000));
                    }
                 
                }
                //nums done displaying
                memTextBox.Text = "Your Turn";
                printing = false;
            }

        }

        private void buttonArea_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxDisplay.Text != "")
            {
                if(Convert.ToDouble(textBoxDisplay.Text) > 0)
                {
                    textBoxDisplay.Text = Convert.ToString
                        (Math.Pow(Convert.ToDouble(textBoxDisplay.Text), 2) * Math.PI);
                }
            }
        }

        private void buttonDecimal_Click(object sender, RoutedEventArgs e)
        {
            if(!textBoxDisplay.Text.Contains("."))//Can't have 2
            {
                textBoxDisplay.Text += ".";
            }
        }

        private void buttonCmToInch_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxDisplay.Text != "")
            {
                textBoxDisplay.Text = Convert.ToString
                    ((Convert.ToDouble(textBoxDisplay.Text) * 0.3937007874));
            }
        }

        private void buttonInchToCm_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxDisplay.Text != "")
            {
                textBoxDisplay.Text = Convert.ToString
                    ((Convert.ToDouble(textBoxDisplay.Text) / 0.3937007874));
            }
        }

        private void buttonMilesToKm_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxDisplay.Text != "")
            {
                textBoxDisplay.Text = Convert.ToString
                    ((Convert.ToDouble(textBoxDisplay.Text) * 1.609344));
            }
        }

        private void buttonKmToMiles_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxDisplay.Text != "")
            {
                textBoxDisplay.Text = Convert.ToString
                    ((Convert.ToDouble(textBoxDisplay.Text) / 1.609344));
            }
        }


        /********************************************************************
         *******************************************************************/

        
        //public MainWindow()
        //{
        //    InitializeComponent();
        //}

        //void WindowLoaded(object sender, RoutedEventArgs e)
        //{
            //listInstalledRecognizers.ItemsSource =
            //SpeechRecognitionEngine.InstalledRecognizers();
            //if (listInstalledRecognizers.Items.Count > 0)
            //    listInstalledRecognizers.SelectedItem =
            //        listInstalledRecognizers.Items[0];

            //_kinectSensor = KinectSensor.KinectSensors[0];
            //_kinectSensor.Start();

        //}

        //private void BtnStartClick(object sender, RoutedEventArgs e)
        //{
        //    if (listInstalledRecognizers.SelectedItem == null) return;
        //    var rec = (RecognizerInfo)listInstalledRecognizers.SelectedItem;
        //    DisableUI();
        //    BuildSpeechEngine(rec);
        //}

        //void DisableUI()
        //{
        //    btnStart.IsEnabled = false;
        //    btnStop.IsEnabled = true;
        //    listInstalledRecognizers.IsEnabled = false;
        //}

        //private void BtnStopClick(object sender, RoutedEventArgs e)
        //{
        //    _kinectSource.Stop();
        //    _speechEngine.RecognizeAsyncStop();
        //    ActivateUI();
        //}

        //void ActivateUI()
        //{
        //    btnStart.IsEnabled = true;
        //    btnStop.IsEnabled = false;
        //    listInstalledRecognizers.IsEnabled = true;
        //}

        void BuildSpeechEngine(RecognizerInfo rec)
        {
            _speechEngine = new SpeechRecognitionEngine(rec.Id);

            var choices = new Choices();
            choices.Add("no");
            choices.Add("yes");
            choices.Add("reset");
            choices.Add("word");
            choices.Add("calculator");
            choices.Add("my computer");
            choices.Add("mspaint");
            choices.Add("close");
            choices.Add("Maybe");
            choices.Add("What?");
            choices.Add("huh");

            var gb = new GrammarBuilder { Culture = rec.Culture };
            gb.Append(choices);
            var g = new Grammar(gb);

            _speechEngine.LoadGrammar(g);
            //recognized a word or words that may be a component of multiple
            //complete phrases in a grammar.
            _speechEngine.SpeechHypothesized += new
    EventHandler<SpeechHypothesizedEventArgs>(SpeechEngineSpeechHypothesized);
            //receives input that matches any of its loaded and enabled Grammar
            //objects.
            _speechEngine.SpeechRecognized += new
  EventHandler<SpeechRecognizedEventArgs>(_speechEngineSpeechRecognized);
            //receives input that does not match any of its loaded and enabled
            //Grammar objects.
            _speechEngine.SpeechRecognitionRejected += new
EventHandler<SpeechRecognitionRejectedEventArgs>(_speechEngineSpeechRecognitionRejected);

            //C# threads are MTA by default and calling RecognizeAsync in the same thread will cause an COM exception.
            var t = new Thread(StartAudioStream);
            t.Start();
        }

        void StartAudioStream()
        {
            _kinectSource = _kinectSensor.AudioSource;
            //Important to turn this off for speech recognition
            _kinectSource.AutomaticGainControlEnabled = false;
            _kinectSource.EchoCancellationMode = EchoCancellationMode.None;
            _stream = _kinectSource.Start();

            _speechEngine.SetInputToAudioStream(_stream,
                            new SpeechAudioFormatInfo(
                                EncodingFormat.Pcm, 16000, 16, 1,
                                32000, 2, null));

            _speechEngine.RecognizeAsync(RecognizeMode.Multiple);
        }

        void _speechEngineSpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            Console.Write("\rSpeech Rejected: \t{0} \n", e.Result.Text);
        }

        void _speechEngineSpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            //txtLastWord.Text = e.Result.Text;
            if (e.Result.Text == "huh")
            {
                
            }
        }

        void SpeechEngineSpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        {
            //txtList.Text = string.Format("{0} - Confidence={1}\n{2}", e.Result.Text, e.Result.Confidence, txtList.Text);
        }
    }
}
