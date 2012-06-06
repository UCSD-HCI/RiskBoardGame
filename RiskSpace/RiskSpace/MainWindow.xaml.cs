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
        private StateManager stateManager;
        private PlayerManager playerManager;

        public MainWindow()
        {
            InitializeComponent();
            playerManager = new PlayerManager(riskMap);
            stateManager = new StateManager(playerManager);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            msgControl.UpdateMessage(stateManager.State, playerManager.GetPlayer(stateManager.ActivePlayerId));
        }

        private void riskMap_CountryClick(object sender, CountryContactEventArgs e)
        {
            switch (stateManager.State)
            {
                case GameState.ChooseCountry:
                    chooseCountry(e.CountryId);
                    break;

                case GameState.AddArmy:
                    addArmy(e.CountryId);
                    break;

                default:
                    break;
            }
        }

        private void refreshViews()
        {
            playerInfo1.Update(playerManager.GetPlayer(1), stateManager.ActivePlayerId == 1);
            playerInfo2.Update(playerManager.GetPlayer(2), stateManager.ActivePlayerId == 2);

            //udpate messages
            msgControl.UpdateMessage(stateManager.State, playerManager.GetPlayer(stateManager.ActivePlayerId), stateManager.AvailabeNewArmy);
        }

        private void chooseCountry(string countryId)
        {
            ArmyViz armyViz = riskMap.GetArmyViz(countryId);

            if (armyViz.PlayerId != 0)
            {
                return;
            }

            armyViz.PlayerId = stateManager.ActivePlayerId;
            armyViz.AddArmy();

            //check if all countries are picked
            int blankCount = 0;
            foreach (ArmyViz eachArmyViz in riskMap.GetAllArmyViz())
            {
                if (eachArmyViz.PlayerId == 0)
                {
                    blankCount++;
                    armyViz = eachArmyViz;

                    if (blankCount >= 2)
                    {
                        break;
                    }
                }
            }

            if (blankCount == 1)
            {
                armyViz.PlayerId = stateManager.ActivePlayerId == PlayerManager.PlayerNum ? 1 : stateManager.ActivePlayerId + 1;
                armyViz.AddArmy();
                stateManager.Finish();
            }
            else
            {
                stateManager.Next();
            }               
                
            refreshViews();
            
        }

        private void addArmy(string countryId)
        {
            ArmyViz armyViz = riskMap.GetArmyViz(countryId);
            if (riskMap.GetArmyViz(countryId).PlayerId != stateManager.ActivePlayerId)
            {
                return;
            }

            armyViz.AddArmy();
            stateManager.Next();
            refreshViews();
        }
    }
}
