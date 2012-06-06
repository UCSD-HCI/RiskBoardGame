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
using System.Diagnostics;

namespace RiskSpace
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Multitouch.Framework.WPF.Controls.Window
    {
        private StateManager stateManager;
        private PlayerManager playerManager;
        private WizardOzWindow wizardOzWindow;

        public MainWindow()
        {
            InitializeComponent();

            MultitouchScreen.AllowNonContactEvents = true;

            playerManager = new PlayerManager(riskMap);
            stateManager = new StateManager(playerManager);

            wizardOzWindow = new WizardOzWindow();
            wizardOzWindow.Show();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            msgControl.UpdateMessage(stateManager, playerManager, riskMap);
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

                case GameState.AttackChooseSource:
                    attackChooseSource(e.CountryId);
                    break;

                case GameState.AttackChooseDest:
                    attackChooseDest(e.CountryId);
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
            msgControl.UpdateMessage(stateManager, playerManager, riskMap);

            //update buttons
            ControlButton activeButtons = stateManager.ActivePlayerId == 1 ? controlButton1 : controlButton2;
            ControlButton inactiveButtons = stateManager.ActivePlayerId == 1 ? controlButton2 : controlButton1;

            activeButtons.HideAllButtons();
            switch (stateManager.State)
            {
                case GameState.AttackChooseSource:
                    activeButtons.NextButtonVisible = true;
                    break;

                case GameState.AttackChooseDest:
                case GameState.AttackWaitDice:
                    activeButtons.CancelButtonVisible = true;
                    break;
                    
                default:
                    activeButtons.HideAllButtons();
                    break;
            }

            inactiveButtons.HideAllButtons();

            //stop attacking
            if (stateManager.State == GameState.AttackChooseSource && stateManager.AttackSource != null)
            {
                riskMap.GetArmyViz(stateManager.AttackSource).IsAttacking = false;
            }

            //attack line
            if (stateManager.State == GameState.AttackWaitDice && !stateManager.IsErrored)
            {
                riskMap.DrawAttackLine(stateManager.AttackSource, stateManager.AttackDest);
            }
            else if (stateManager.State != GameState.AttackWaitDice)
            {
                riskMap.HideAttackLine();
            }

            //wizardOz
            if (stateManager.State == GameState.AttackWaitDice)
            {
                wizardOzWindow.RequestDice();
            }
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

        private void attackChooseSource(string countryId)
        {
            ArmyViz armyViz = riskMap.GetArmyViz(countryId);
            if (armyViz.PlayerId != stateManager.ActivePlayerId || armyViz.ArmyCount <= 2 )
            {
                return;
            }

            /*attackArmyViz.CountryID = countryId;
            attackArmyViz.PlayerId = stateManager.ActivePlayerId;
            attackArmyViz.AddArmy();

            armyViz.ArmyCount--;

            Point p = armyViz.TranslatePoint(new Point(0, 0), mainGrid);
            
            //Size vizSize = armyViz.RenderSize;
            //p -= new Vector(vizSize.Width, vizSize.Height);

            p -= new Vector(50, 50);
            attackArmyViz.Margin = new Thickness(p.X, p.Y, 0, 0);*/

            armyViz.IsAttacking = true;
            int maxDiceNum = Math.Min(3, armyViz.ArmyCount - 1);
            stateManager.AttackSourceSelected(countryId, maxDiceNum);
            //attackArmyViz.Visibility = Visibility.Visible;

            refreshViews();
        }

        private void attackChooseDest(string countryId)
        {
            ArmyViz armyViz = riskMap.GetArmyViz(countryId);
            if (armyViz.PlayerId == stateManager.ActivePlayerId)
            {
                return;
            }

            int maxDiceNum = Math.Min(2, armyViz.ArmyCount);
            stateManager.AttackDestSelected(countryId, maxDiceNum);
            refreshViews();
        }

        private void controlButton_OkButtonClick(object sender, EventArgs e)
        {
            
        }

        private void controlButton_CancelButtonClick(object sender, EventArgs e)
        {
            switch (stateManager.State)
            {
                case GameState.AttackChooseDest:
                case GameState.AttackWaitDice:
                    stateManager.Cancel();
                    break;
            }

            refreshViews();
        }

        private void controlButton_NextButtonClick(object sender, EventArgs e)
        {
            switch (stateManager.State)
            {
                case GameState.AttackChooseSource:
                    stateManager.RoundPass();
                    break;

                default:
                    Debug.Assert(false);
                    break;
            }

            refreshViews();
        }
    }
}
