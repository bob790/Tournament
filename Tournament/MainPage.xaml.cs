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

namespace Tournament
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public Tourney k;
        public MainPage(string filename)
        {
            InitializeComponent();
            k = new Tourney(filename);
            this.DisplayDetails();
        }

        private void btnEpisode_Click(object sender, RoutedEventArgs e)
        {
            lblEpisodeNumber.Content = k.currentMatch.episode;
        }

        private void btnContestant1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (k.remainder < 2) { return; }
                k.currentMatch.Winner(1);
                k.nextMatch();
                k.saveGame();
                this.DisplayDetails();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnContestant2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (k.remainder < 2) { return; }
                k.currentMatch.Winner(2);
                k.nextMatch();
                k.saveGame();
                this.DisplayDetails();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void DisplayDetails()
        {
            lblProgress.Content = "Round " + k.round + " (" + k.remainder + " Series)";
            lblContestant1.Content = k.currentMatch.player1.name;
            btnContestant1.Content = k.currentMatch.player1.name;
            try
            {
                lblContestant2.Content = k.currentMatch.player2.name;
                btnContestant2.Content = k.currentMatch.player2.name;
            }
            catch(Exception e)
            {
                lblContestant2.Content = " ";
                btnContestant2.Content = " ";
            }
            lblEpisodeNumber.Content = null;
        }
    }
}
