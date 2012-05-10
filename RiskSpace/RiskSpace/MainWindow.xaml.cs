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
using Multitouch.Framework.WPF.Input;

namespace RiskSpace
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Multitouch.Framework.WPF.Controls.Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //africaImg.AddHandler(MultitouchScreen.NewContactEvent, (NewContactEventHandler)africaImg_NewContact);
            //backImg.AddHandler(MultitouchScreen.NewContactEvent, (NewContactEventHandler)Image_NewContact);
            //testButton.AddHandler(MultitouchScreen.NewContactEvent, (NewContactEventHandler)Image_NewContact);
        }

        private void Image_NewContact(object sender, Multitouch.Framework.WPF.Input.NewContactEventArgs e)
        {
            
        }

        private void Image_ContactRemoved(object sender, ContactEventArgs e)
        {
            
        }

        private void Image_ContactEnter(object sender, ContactEventArgs e)
        {
            (sender as Image).Opacity = 1.0;
        }

        private void Image_ContactLeave(object sender, ContactEventArgs e)
        {
            (sender as Image).Opacity = 0.5;
        }
    }
}
