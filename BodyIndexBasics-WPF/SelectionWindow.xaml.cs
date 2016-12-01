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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Microsoft.Samples.Kinect.BodyIndexBasics
{
    /// <summary>
    /// Interaction logic for SelectionWindow.xaml
    /// </summary>
    public partial class SelectionWindow : Window
    {
        public int ExtensionReps = 0;
        public int BendReps = 0;
        public int SquatReps = 0;

        public string ExtensionLR;
        public string BendLR;

        public SelectionWindow()
        {
            InitializeComponent();
        }

        private void Extension_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxItem selectedItem = (ComboBoxItem)Reps1.SelectedValue;
            ExtensionReps = Convert.ToInt32(selectedItem.Content);
            ExtensionLR = ((ComboBoxItem)LR1.SelectedItem).Content.ToString();
            MainWindow mw = new MainWindow();
            mw.ExtensionReps = ExtensionReps;
            mw.ExtensionLR = ExtensionLR;
            mw.exercise = "extension";
            mw.Show();
            this.Close();
        }

        private void Bend_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxItem selectedItem = (ComboBoxItem)Reps2.SelectedValue;
            BendReps = Convert.ToInt32(selectedItem.Content);
            BendLR = ((ComboBoxItem)LR2.SelectedItem).Content.ToString();
            MainWindow mw = new MainWindow();
            mw.BendReps = BendReps;
            mw.BendLR = BendLR;
            mw.exercise = "bend";
            mw.Show();
            this.Close();
        }

        private void Squat_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxItem selectedItem = (ComboBoxItem)Reps3.SelectedValue;
            SquatReps = Convert.ToInt32(selectedItem.Content);
            MainWindow mw = new MainWindow();
            mw.SquatReps = SquatReps;
            mw.exercise = "squat";
            mw.Show();
            this.Close();
        }
        private void extensionPlay_Click(object sender, RoutedEventArgs e)
        {
            Window extensionPlay = new ExtensionVideo();
            extensionPlay.Show();
        }

        private void bendPlay_Click(object sender, RoutedEventArgs e)
        {
            Window bendPlay = new BendVideo();
            bendPlay.Show();
        }

        private void squatPlay_Click(object sender, RoutedEventArgs e)
        {
            Window squatPlay = new SquatVideo();
            squatPlay.Show();
        }
    }
}
