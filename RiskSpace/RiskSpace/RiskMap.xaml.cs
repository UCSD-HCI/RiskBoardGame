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
    /// Interaction logic for RiskMap.xaml
    /// </summary>
    public partial class RiskMap : UserControl
    {
        public event EventHandler<CountryContactEventArgs> CountryClick;

        public RiskMap()
        {
            InitializeComponent();
        }

        /*public void AddArmy(string countryId)
        {
            GetArmyViz(countryId).AddArmy();
        }

        public int GetCountryOwner(string countryId)
        {
            return GetArmyViz(countryId).PlayerID;
        }*/

        public ArmyViz GetArmyViz(string countryId)
        {
            ArmyViz viz = null;
            foreach (var child in counterCanvas.Children)
            {
                ArmyViz t = child as ArmyViz;
                if (t != null && t.CountryID == countryId)
                {
                    viz = t;
                    break;
                }
            }

            return viz;
        }

        public IEnumerable<ArmyViz> GetAllArmyViz()
        {
            List<ArmyViz> res = new List<ArmyViz>();
            foreach (var child in counterCanvas.Children)
            {
                ArmyViz t = child as ArmyViz;
                if (t != null)
                {
                    //yield return t;
                    res.Add(t);
                }
            }
            return res;
        }

        public int GetCountryNum()
        {
            return counterCanvas.Children.Count;
        }

        private void country_ContactEnter(object sender, ContactEventArgs e)
        {
            (sender as Image).Opacity = 1.0;
        }

        private void country_ContactLeave(object sender, ContactEventArgs e)
        {
            (sender as Image).Opacity = 0.5;
        }

        private void country_ContactRemoved(object sender, ContactEventArgs e)
        {
            string countryId = (sender as Image).Tag as string;

            /*ArmyViz viz = GetArmyViz(countryId);
            if (viz == null)
            {
                return;
            }
            viz.AddArmy();*/

            if (CountryClick != null)
            {
                CountryClick(this, new CountryContactEventArgs(countryId, e));
            }
        }
    }

    public class CountryContactEventArgs : EventArgs
    {
        public ContactEventArgs ContactEventArgs { get; private set; }
        public string CountryId { get; private set; }
        public CountryContactEventArgs(string countryId, ContactEventArgs e)
        {
            this.CountryId = countryId;
            this.ContactEventArgs = e;
        }
    }
}
