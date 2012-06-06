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
using System.Diagnostics;

namespace RiskSpace
{
    /// <summary>
    /// Interaction logic for GameMessageControl.xaml
    /// </summary>
    public partial class GameMessageControl : UserControl
    {
        public GameMessageControl()
        {
            InitializeComponent();
        }

        public void UpdateMessage(GameState state, Player activePlayer, int availabeNewArmy)
        {
            string msg;
            switch (state)
            {
                case GameState.ChooseCountry:
                    msg = "Pick one country. ";
                    break;

                case GameState.AddArmy:
                    msg = "Add your army! (" + availabeNewArmy.ToString() + " left)";
                    break;

                default:
                    msg = null;
                    break;
            }

            if (msg != null)
            {
                msgTextBlock.Text = "Player " + activePlayer.PlayerId + ": " + msg;
                msgTextBlock.Foreground = new SolidColorBrush(activePlayer.MainColor);
            }
            else
            {
                msgTextBlock.Text = "";
            }
        }

        public void UpdateMessage(GameState state, Player activePlayer)
        {
            Debug.Assert(state != GameState.AddArmy);
            UpdateMessage(state, activePlayer, 0);
        }
    }
}
