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
using System.Windows.Media.Effects;
using System.Windows.Media.Animation;

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
        private bool attacking;

        private DropShadowEffect shadowEffect;
        private TranslateTransform translate;

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
            set
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

        public bool IsAttacking
        {
            get { return attacking; }
            set
            {
                attacking = value;
                if (attacking)
                {
                    shadowEffect = new DropShadowEffect()
                    {
                        BlurRadius = 3,
                        ShadowDepth = 4,
                    };

                    counterTextBlock.Effect = shadowEffect;
                    DoubleAnimation depthAnim = new DoubleAnimation(shadowEffect.ShadowDepth + 1, shadowEffect.ShadowDepth - 1,new Duration(TimeSpan.FromSeconds(0.5)));
                    depthAnim.RepeatBehavior = RepeatBehavior.Forever;
                    depthAnim.AutoReverse = true;
                    depthAnim.EasingFunction = new SineEase()
                    {
                        EasingMode = EasingMode.EaseInOut
                    };
                    shadowEffect.BeginAnimation(DropShadowEffect.ShadowDepthProperty, depthAnim);

                    translate = new TranslateTransform(0, 0);
                    counterTextBlock.RenderTransform = translate;

                    DoubleAnimation translateAnim = new DoubleAnimation(-1, 1, new Duration(TimeSpan.FromSeconds(0.5)));
                    translateAnim.RepeatBehavior = RepeatBehavior.Forever;
                    translateAnim.AutoReverse = true;
                    translateAnim.EasingFunction = new SineEase()
                    {
                        EasingMode = EasingMode.EaseInOut
                    };
                    translate.BeginAnimation(TranslateTransform.XProperty, translateAnim);
                    translate.BeginAnimation(TranslateTransform.YProperty, translateAnim);
                }
                else
                {
                    counterTextBlock.Effect = null;
                    counterTextBlock.RenderTransform = null;
                }
            }
        }
    }
}
