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

        public void UpdateMessage(StateManager stateManager, PlayerManager playerManager, RiskMap map)
        {
            string msg;
            switch (stateManager.State)
            {
                case GameState.ChooseCountry:
                    msg = "Pick one country. ";
                    break;

                case GameState.AddArmy:
                    msg = "Add your army! (" + stateManager.AvailabeNewArmy.ToString() + " left)";
                    break;

                case GameState.AttackChooseSource:
                    msg = "Choose your army to attack! ";
                    break;

                case GameState.AttackChooseDest:
                    msg = "Where will you attack? ";
                    break;

                case GameState.AttackWaitDice:
                    msg = "Throw dice! ";
                    break;

                case GameState.AttackPickArmy:
                    msg = "Pick more army to defense your new territory!";
                    break;

                default:
                    msg = null;
                    break;
            }

            if (stateManager.State == GameState.AttackWaitDice)
            {
                Player attacker = playerManager.GetPlayer(stateManager.ActivePlayerId);
                Player defender = playerManager.GetPlayer(map.GetArmyViz(stateManager.AttackDest).PlayerId);
                string attackerDice = stateManager.AttackerMaxDiceNum > 1 ? "dices" : "dice";
                string defenderDice = stateManager.DefenderMaxDiceNum > 1 ? "dices" : "dice";
                msgTextBlock.Text = string.Format("Player {0}: Throw {1} {2}! Player {3}: Throw {4} {5}! ", 
                    attacker.PlayerId, 
                    stateManager.AttackerMaxDiceNum, 
                    attackerDice,
                    defender.PlayerId, 
                    stateManager.DefenderMaxDiceNum,
                    defenderDice);
            }
            else if (msg != null)
            {
                Player player = playerManager.GetPlayer(stateManager.ActivePlayerId);
                msgTextBlock.Text = "Player " + player.PlayerId + ": " + msg;
                msgTextBlock.Foreground = new SolidColorBrush(player.MainColor);
            }
            else
            {
                msgTextBlock.Text = "";
            }
        }
    }
}
