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

namespace RiskSpace
{
    /// <summary>
    /// Interaction logic for ArmyViz.xaml
    /// </summary>
    public partial class ArmyViz : UserControl
    {
        private int playerId;
        private int armyCount;
        private string countryId;

        public ArmyViz()
        {
            InitializeComponent();
            PlayerId = 0;
        }

        public int PlayerId
        {
            get { return playerId; }
            set
            {
                playerId = value;
                counterTextBlock.Foreground = new SolidColorBrush(Player.GetMainColor(playerId));
            }
        }

        public string CountryID
        {
            get { return countryId; }
            set
            {
                countryId = value;
            }
        }

        public int ArmyCount
        {
            get { return armyCount; }
            private set
            {
                armyCount = value;
                counterTextBlock.Text = armyCount.ToString();
                this.Visibility = armyCount > 0 ? Visibility.Visible : Visibility.Hidden;
            }
        }

        public void AddArmy()
        {
            ArmyCount++;
        }
    }
}
