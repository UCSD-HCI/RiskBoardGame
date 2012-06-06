using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO.MemoryMappedFiles;
using System.Threading;
using System.Diagnostics;

namespace RiskSpace
{
    /// <summary>
    /// Interaction logic for WizrdOzWindow.xaml
    /// </summary>
    public partial class WizardOzWindow : Window
    {
        private MemoryMappedFile rectifiedTabletopMmf;
        private const int imgSize = 640 * 480 * 3;
        private Timer timer;
        private WriteableBitmap videoSource;
        private byte[] buffer;

        private WizardOzState state;
        private List<Dice> dices;

        public event EventHandler<DiceEventArgs> DiceLocated;
        public event EventHandler<DiceEventArgs> DiceLabeled;
        public event EventHandler<DiceFinishedEventArgs> DiceFinished;

        public WizardOzWindow()
        {
            InitializeComponent();

            videoSource = new WriteableBitmap(640, 480, 72, 72, PixelFormats.Rgb24, null);
            rectifiedTabletopMmf = MemoryMappedFile.OpenExisting("InteractiveSpaceRectifiedTabletop", MemoryMappedFileRights.Read);
            buffer = new byte[imgSize];
            dices = new List<Dice>();

            state = WizardOzState.Idle;
        }

        public void RequestDice()
        {
            state = WizardOzState.Dice;
            finishButton.IsEnabled = true;
            dices.Clear();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            timer = new Timer(timerCallback, null, 0, 1000 / 12);
            videoImage.Source = videoSource;
        }

        private void timerCallback(object state)
        {
            Dispatcher.BeginInvoke((Action)delegate()
            {
                using (MemoryMappedViewStream stream = rectifiedTabletopMmf.CreateViewStream(0, imgSize, MemoryMappedFileAccess.Read))
                {
                    stream.Read(buffer, 0, imgSize);
                    videoSource.Lock();
                    videoSource.WritePixels(new Int32Rect(0, 0, 640, 480), buffer, 640 * 3, 0);
                    videoSource.Unlock();
                }
            }, null);
        }

        private void videoImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (state == WizardOzState.Dice)
            {
                Point vizPos = e.GetPosition(imageCanvas);
                Point actualPos = new Point(vizPos.X * 2.5, vizPos.Y * 2.5);    //TODO: hardcoding!

                DiceColor diceColor = e.RightButton == MouseButtonState.Pressed ? DiceColor.Red : DiceColor.White;
                Dice d = new Dice(dices.Count, actualPos, diceColor);
                dices.Add(d);

                Canvas.SetLeft(diceTextBox, e.GetPosition(imageCanvas).X + 10);
                Canvas.SetTop(diceTextBox, e.GetPosition(imageCanvas).Y + 10);
                //diceTextBox.SelectAll();
                diceTextBox.Text = "";
                diceTextBox.Visibility = Visibility.Visible;
                diceTextBox.IsReadOnly = false;
                diceTextBox.Foreground = diceColor == DiceColor.Red ? Brushes.White : Brushes.Black;
                diceTextBox.Background = diceColor == DiceColor.Red ? Brushes.DarkRed : Brushes.White;
                diceTextBox.Focus();

                if (DiceLocated != null)
                {
                    DiceLocated(this, new DiceEventArgs(d));
                }
            }
        }

        private void diceTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Debug.Assert(state == WizardOzState.Dice);

            if (diceTextBox.Text == "")
            {
                return;
            }

            int value;
            if (!int.TryParse(diceTextBox.Text, out value))
            {
                diceTextBox.Text = "";
                return;
            }

            diceTextBox.IsReadOnly = true;
            dices.Last().Value = value;
            if (DiceLabeled != null)
            {
                DiceLabeled(this, new DiceEventArgs(dices.Last()));
            }
        }

        private void finishButton_Click(object sender, RoutedEventArgs e)
        {
            switch (state)
            {
                case WizardOzState.Dice:
                    if (DiceFinished != null)
                    {
                        DiceFinished(this, new DiceFinishedEventArgs(dices));
                    }
                    finishButton.IsEnabled = false;
                    break;

                default:
                    Debug.Assert(false);
                    break;
            }
        }
    }

    public class DiceEventArgs : EventArgs
    {
        public Dice Dice { get; private set; }
        public DiceEventArgs(Dice dice)
        {
            this.Dice = dice;
        }
    }

    public class DiceFinishedEventArgs : EventArgs
    {
        public List<Dice> AllDices { get; private set; }
        public DiceFinishedEventArgs(List<Dice> allDices)
        {
            this.AllDices = allDices;
        }
    }

    public enum WizardOzState
    {
        Idle = 0,
        Dice = 1,
        Card = 2
    }
}
