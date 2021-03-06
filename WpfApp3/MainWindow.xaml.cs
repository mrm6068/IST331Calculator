﻿using System;
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
using System.Globalization;

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

        public bool IsRunning { get; private set; }
        private KinectSensor kinectSensor;
        private DateTime recordingStartTime;
        private Thread workingThread;
        private MemoryStream contentMemoryStream;
        private string audioFilePath;
        MediaPlayer mp = new MediaPlayer();
        bool somethingRecorded = false;


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IsRunning = false;
            kinectSensor = KinectSensor.KinectSensors[0];
            kinectSensor.Start();

            buttonMul.Tag = Operator.Times;
            buttonDiv.Tag = Operator.Divide;
            buttonSub.Tag = Operator.Minus;
            buttonAdd.Tag = Operator.Plus;
            buttonEquals.Tag = Operator.Equals;

            listInstalledRecognizers.ItemsSource =
                    SpeechRecognitionEngine.InstalledRecognizers();
            if (listInstalledRecognizers.Items.Count > 0)
                listInstalledRecognizers.SelectedItem =
                    listInstalledRecognizers.Items[0];

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
            memTextBox.Clear();
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
                            await Task.Run(() => System.Threading.Thread.Sleep(600));

                            if (i != nums.Count - 1)//No * after last num
                            {
                                memTextBox.Text = Convert.ToString("*");
                                await Task.Run(() => System.Threading.Thread.Sleep(400));
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
                    await Task.Run(() => System.Threading.Thread.Sleep(600));

                    if (i != nums.Count - 1)//No * after last num
                    {
                        memTextBox.Text = Convert.ToString("*");
                        await Task.Run(() => System.Threading.Thread.Sleep(400));
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

        private void buttonVoiceOn_Click(object sender, RoutedEventArgs e)
        {
            if (listInstalledRecognizers.SelectedItem == null) return;
            var rec = (RecognizerInfo)listInstalledRecognizers.SelectedItem;
            DisableUI();
            BuildSpeechEngine(rec);
        }

        void DisableUI()
        {
            buttonVoiceOn.IsEnabled = false;
            buttonVoiceOff.IsEnabled = true;
            listInstalledRecognizers.IsEnabled = false;
        }

        private void buttonVoiceOff_Click(object sender, RoutedEventArgs e)
        {
            _kinectSource.Stop();
            _speechEngine.RecognizeAsyncStop();
            ActivateUI();
        }

        void ActivateUI()
        {
            buttonVoiceOn.IsEnabled = true;
            buttonVoiceOff.IsEnabled = false;
            listInstalledRecognizers.IsEnabled = true;
        }

        void BuildSpeechEngine(RecognizerInfo rec)
        {
            _speechEngine = new SpeechRecognitionEngine(rec.Id);

            var choices = new Choices();
            choices.Add("minus");
            choices.Add("plus");
            choices.Add("times");
            choices.Add("divided by");
            choices.Add("over");
            choices.Add("back");
            choices.Add("clear");
            choices.Add("clear everything");
            choices.Add("equals");
            choices.Add("point");
            choices.Add("enter");
            choices.Add("play");
            choices.Add("zero");
            choices.Add("one");
            choices.Add("two");
            choices.Add("three");
            choices.Add("four");
            choices.Add("five");
            choices.Add("six");
            choices.Add("seven");
            choices.Add("eight");
            choices.Add("nine");
            choices.Add("ten");
            choices.Add("eleven");
            choices.Add("twelve");
            choices.Add("thirteen");
            choices.Add("forteen");
            choices.Add("fifteen");
            choices.Add("sixteen");
            choices.Add("seventeen");
            choices.Add("eighteen");
            choices.Add("ninteen");
            choices.Add("twenty");
            choices.Add("set A");
            choices.Add("set B");
            choices.Add("get C");
            choices.Add("miles to kilometers");
            choices.Add("kilometers to miles");
            choices.Add("inches to centimeters");
            choices.Add("centimeters to inches");
            choices.Add("fahrenheit to celsius");
            choices.Add("celsius to fahrenheit");
            choices.Add("area from radius");

            choices.Add("exit calculator");



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
            switch(e.Result.Text)
            {
                case "play":
                    buttonPlay_Click(new object(), new RoutedEventArgs());
                    break;

                case "enter":
                    buttonEnter_Click(new object(), new RoutedEventArgs());
                    break;

                case "minus":
                    ExecuteLastOperator(Operator.Minus);
                    break;

                case "plus":
                    ExecuteLastOperator(Operator.Plus);
                    break;

                case "times":
                    ExecuteLastOperator(Operator.Times);
                    break;

                case "divided by":
                    ExecuteLastOperator(Operator.Divide);
                    break;

                case "over":
                    ExecuteLastOperator(Operator.Divide);
                    break;

                case "back":
                    buttonBack_Click(new object(), new RoutedEventArgs());
                    break;

                case "clear":
                    buttonC_Click(new object(), new RoutedEventArgs());
                    break;

                case "equals":
                    ExecuteLastOperator(Operator.Equals);
                    break;

                case "point":
                    buttonDecimal_Click(new object(), new RoutedEventArgs());
                    break;

                case "zero":
                    button0_Click(new object(), new RoutedEventArgs());
                    break;

                case "one":
                    button1_Click(new object(), new RoutedEventArgs());
                    break;

                case "two":
                    button2_Click(new object(), new RoutedEventArgs());
                    break;

                case "three":
                    button3_Click(new object(), new RoutedEventArgs());
                    break;

                case "four":
                    button4_Click(new object(), new RoutedEventArgs());
                    break;

                case "five":
                    button5_Click(new object(), new RoutedEventArgs());
                    break;

                case "six":
                    button6_Click(new object(), new RoutedEventArgs());
                    break;

                case "seven":
                    button7_Click(new object(), new RoutedEventArgs());
                    break;

                case "eight":
                    button8_Click(new object(), new RoutedEventArgs());
                    break;

                case "nine":
                    button9_Click(new object(), new RoutedEventArgs());
                    break;
                case "ten":
                    button1_Click(new object(), new RoutedEventArgs());
                    button0_Click(new object(), new RoutedEventArgs());
                    break;
                case "eleven":
                    button1_Click(new object(), new RoutedEventArgs());
                    button1_Click(new object(), new RoutedEventArgs());
                    break;
                case "twelve":
                    button1_Click(new object(), new RoutedEventArgs());
                    button2_Click(new object(), new RoutedEventArgs());
                    break;
                case "thirteen":
                    button1_Click(new object(), new RoutedEventArgs());
                    button3_Click(new object(), new RoutedEventArgs());
                    break;
                case "forteen":
                    button1_Click(new object(), new RoutedEventArgs());
                    button4_Click(new object(), new RoutedEventArgs());
                    break;
                case "fifteen":
                    button1_Click(new object(), new RoutedEventArgs());
                    button5_Click(new object(), new RoutedEventArgs());
                    break;
                case "sixteen":
                    button1_Click(new object(), new RoutedEventArgs());
                    button6_Click(new object(), new RoutedEventArgs());
                    break;
                case "seventeen":
                    button1_Click(new object(), new RoutedEventArgs());
                    button7_Click(new object(), new RoutedEventArgs());
                    break;
                case "eighteen":
                    button1_Click(new object(), new RoutedEventArgs());
                    button8_Click(new object(), new RoutedEventArgs());
                    break;
                case "ninteen":
                    button1_Click(new object(), new RoutedEventArgs());
                    button9_Click(new object(), new RoutedEventArgs());
                    break;
                case "twenty":
                    button2_Click(new object(), new RoutedEventArgs());
                    button0_Click(new object(), new RoutedEventArgs());
                    break;
                case "set A":
                    buttonSetA_Click(new object(), new RoutedEventArgs());
                    break;

                case "set B":
                    buttonSetB_Click(new object(), new RoutedEventArgs());
                    break;

                case "get C":
                    buttonGetC_Click(new object(), new RoutedEventArgs());
                    break;

                case "miles to kilometers":
                    buttonMilesToKm_Click(new object(), new RoutedEventArgs());
                    break;

                case "kilometers to miles":
                    buttonKmToMiles_Click(new object(), new RoutedEventArgs());
                    break;

                case "inches to centimeters":
                    buttonInchToCm_Click(new object(), new RoutedEventArgs());
                    break;

                case "centimeters to inches":
                    buttonCmToInch_Click(new object(), new RoutedEventArgs());
                    break;

                case "fahrenheit to celsius":
                    buttonFToC_Click(new object(), new RoutedEventArgs());
                    break;

                case "celsius to fahrenheit":
                    buttonCToF_Click(new object(), new RoutedEventArgs());
                    break;

                case "area from radius":
                    buttonArea_Click(new object(), new RoutedEventArgs());
                    break;

                case "clear everything":
                    buttonCE_Click(new object(), new RoutedEventArgs());
                    break;

                    //case "exit calculator":


            }

        }

        void SpeechEngineSpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        {
            //string.Format("{0} - Confidence={1}\n{2}", e.Result.Text, e.Result.Confidence, txtList.Text);
        }

        private void buttonMul_Click(object sender, RoutedEventArgs e)
        {
            OnClickOperator(sender, e);
        }

        private void buttonDiv_Click(object sender, RoutedEventArgs e)
        {
            OnClickOperator(sender, e);
        }

        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            OnClickOperator(sender, e);
        }

        private void buttonSub_Click(object sender, RoutedEventArgs e)
        {
            OnClickOperator(sender, e);
        }

        private void buttonEquals_Click(object sender, RoutedEventArgs e)
        {
            OnClickOperator(sender, e);
        }

        private void buttonRecord_Click(object sender, RoutedEventArgs e)
        {
            if (IsRunning)
            {
                IsRunning = false;
                stop();
                return;
            }
            if(somethingRecorded)
            {
                File.Delete(audioFilePath);
            }
            textBoxDisplay.Text = "Recording";
            IsRunning = true;
            recordingStartTime = DateTime.Now;
            workingThread = new Thread(RecordAudio) { IsBackground = true };
            workingThread.Start();
            somethingRecorded = true;
        }

        private void buttonPlayBack_Click(object sender, RoutedEventArgs e)
        {
            if(somethingRecorded)
            {
                mp.Open(new System.Uri(audioFilePath));
                mp.Play();
            }
        }

        void RecordAudio(object o)
        {
            var source = kinectSensor.AudioSource;
            contentMemoryStream = null;
            var buffer = new byte[1024];

            using (var sourceStream = source.Start())
            {
                while (IsRunning)
                {
                    if (contentMemoryStream == null)
                        contentMemoryStream = new MemoryStream();
                    int count;
                    while ((count = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
                        contentMemoryStream.Write(buffer, 0, count);
                }
            }
        }

        private void stop()
        {
            string time = System.DateTime.Now.ToString("hh'-'mm'-'ss", CultureInfo.CurrentUICulture.DateTimeFormat);
            string myMusic = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            audioFilePath = System.IO.Path.Combine(Environment.CurrentDirectory, "KinectAudio-" + time + ".wav");

            textBoxDisplay.Clear();
            IsRunning = false;
            kinectSensor.AudioSource.Stop();
            SaveAudioToFile();
        }

        private void SaveAudioToFile()
        {
            if (contentMemoryStream == null) return;

            var recordingDuration = DateTime.Now.Subtract(recordingStartTime).TotalSeconds;
            var recordingLength = (int)recordingDuration * 2 * 16000;
            using (var headerMemoryStream = new MemoryStream())
            {
                WriteWavHeader(headerMemoryStream, recordingLength);
                using (var fileStream = new FileStream(audioFilePath, FileMode.Create))
                {
                    headerMemoryStream.WriteTo(fileStream);
                    contentMemoryStream.WriteTo(fileStream);
                    contentMemoryStream.Dispose();
                    fileStream.Close();
                }
            }
        }

        static void WriteWavHeader(Stream stream, int dataLength)
        {
            //We need to use a memory stream because the BinaryWriter will close the underlying stream when it is closed
            using (var memStream = new MemoryStream(64))
            {
                const int cbFormat = 18; //sizeof(WAVEFORMATEX)
                var format = new WaveFormatEx
                {
                    wFormatTag = 1,
                    nChannels = 1,
                    nSamplesPerSec = 16000,
                    nAvgBytesPerSec = 32000,
                    nBlockAlign = 2,
                    wBitsPerSample = 16,
                    cbSize = 0
                };

                using (var bw = new BinaryWriter(memStream))
                {
                    //RIFF header
                    WriteString(memStream, "RIFF");
                    bw.Write(dataLength + cbFormat + 4); //File size - 8
                    WriteString(memStream, "WAVE");
                    WriteString(memStream, "fmt ");
                    bw.Write(cbFormat);

                    //WAVEFORMATEX
                    bw.Write(format.wFormatTag);
                    bw.Write(format.nChannels);
                    bw.Write(format.nSamplesPerSec);
                    bw.Write(format.nAvgBytesPerSec);
                    bw.Write(format.nBlockAlign);
                    bw.Write(format.wBitsPerSample);
                    bw.Write(format.cbSize);

                    //data header
                    WriteString(memStream, "data");
                    bw.Write(dataLength);
                    memStream.WriteTo(stream);
                }
            }
        }

        static void WriteString(Stream stream, string s)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(s);
            stream.Write(bytes, 0, bytes.Length);
        }

        struct WaveFormatEx
        {
            public ushort wFormatTag;
            public ushort nChannels;
            public uint nSamplesPerSec;
            public uint nAvgBytesPerSec;
            public ushort nBlockAlign;
            public ushort wBitsPerSample;
            public ushort cbSize;
        }




    }
}
