using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using Risiko;

namespace WPFLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal LauncherControl Control;

        //Constructor
        public MainWindow()
        {
            InitializeComponent();
            canvasGames.Visibility = Visibility.Hidden;
            canvasProfile.Visibility = Visibility.Hidden;
            gridBigGame.Visibility = Visibility.Hidden;
            Control = new LauncherControl(this);
        }


        //Für Buttons, MouseDown usw
        private void rectHome_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(canvasHome.Visibility == Visibility.Hidden)
            {
                if(canvasGames.Visibility == Visibility.Visible)
                    VanishAndAppear(canvasGames, canvasHome);
                else if(canvasProfile.Visibility == Visibility.Visible)
                    VanishAndAppear(canvasProfile, canvasHome);
            }
        }

        

        /// <summary>
        /// Lässt FEToVanish verschwinden und anschließen FEToAppear erscheinen
        /// </summary>
        /// <param name="FEToVanish"></param>
        /// <param name="FEToAppear"></param>
        internal void VanishAndAppear(FrameworkElement FEToVanish, FrameworkElement FEToAppear)
        {
            FEToVanish.Visibility = Visibility.Visible;
            FEToVanish.Opacity = 1.0;

            //appear
            Storyboard vanishStoryboard = new Storyboard();
            DoubleAnimation vanishDoubleAnimation = new DoubleAnimation(1.0, 0.0, new Duration(TimeSpan.FromMilliseconds(500)));

            vanishStoryboard.Children.Add(vanishDoubleAnimation);
            Storyboard.SetTargetProperty(vanishDoubleAnimation, new PropertyPath(OpacityProperty));
            vanishDoubleAnimation.Completed += (sender, args) => MyVanishStoryCompleted(FEToVanish, FEToAppear, args);
            Storyboard.SetTargetName(vanishDoubleAnimation, FEToVanish.Name);

            vanishStoryboard.Begin(this);
        }
        /// <summary>
        /// Event von VanishBoard, leitet Erscheinen nach verschwinden ein
        /// </summary>
        /// <param name="ControlToVanish"></param>
        /// <param name="ControlToAppear"></param>
        /// <param name="e"></param>
        internal void MyVanishStoryCompleted(FrameworkElement ControlToVanish, FrameworkElement ControlToAppear, EventArgs e)
        {
            ControlToVanish.Visibility = Visibility.Hidden;
            ControlToAppear.Visibility = Visibility.Visible;

            //appear
            Storyboard appearStoryboard = new Storyboard(); ;
            DoubleAnimation appearDoubleAnimation = new DoubleAnimation(0.0, 1.0, new Duration(TimeSpan.FromMilliseconds(500)));

            appearStoryboard.Children.Add(appearDoubleAnimation);
            Storyboard.SetTargetProperty(appearDoubleAnimation, new PropertyPath(OpacityProperty));
            Storyboard.SetTargetName(appearDoubleAnimation, ControlToAppear.Name);
            appearStoryboard.Begin(this);
        }

        private void btnLPGame1_Click(object sender, RoutedEventArgs e)
        {
            VanishAndAppear(gridBigHome, gridBigGame);
            Control.ActualGame = 0;
            lblGameName.Content = "Risiko";
        }

        private void btnFriendOnline1_Click(object sender, RoutedEventArgs e)
        {

        }

        private void rectLoad_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Control.LoadGame();
        }

    }
}
