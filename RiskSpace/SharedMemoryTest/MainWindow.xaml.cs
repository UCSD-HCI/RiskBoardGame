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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.MemoryMappedFiles;
using System.Threading;

namespace SharedMemoryTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MemoryMappedFile rectifiedTabletopMmf;
        private const int imgSize = 640 * 480 * 3;
        private Timer timer;
        private WriteableBitmap videoSource;
        private byte[] buffer;

        public MainWindow()
        {
            InitializeComponent();
            videoSource = new WriteableBitmap(640, 480, 72, 72, PixelFormats.Rgb24, null);
            rectifiedTabletopMmf = MemoryMappedFile.OpenExisting("InteractiveSpaceRectifiedTabletop", MemoryMappedFileRights.Read);
            buffer = new byte[imgSize];
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            timer = new Timer(timerCallback, null, 0, 1000 / 12);
            img.Source = videoSource;
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
    }
}
