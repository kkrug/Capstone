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
    /// Interaction logic for FeedbackWindow.xaml
    /// </summary>
    public partial class FeedbackWindow : Window
    {
        public List<double> repScores = new List<double>();
        public double avgScore;
        public string exercise;

        public FeedbackWindow()
        {
            InitializeComponent();

            if (exercise == "extension")
                exerciseName.Content = "Knee Extension";
            else if (exercise == "bend")
                exerciseName.Content = "Knee Bend";
            else if (exercise == "squat")
                exerciseName.Content = "Squat";

            average.Content = Math.Round(avgScore, 2);

            int repNums = repScores.Count;

            string score1 = repScores[0].ToString();

            switch (repNums)
            {
                case 5:
                    scores.Content = "1: " + repScores[0].ToString() + "\n2: " + repScores[1].ToString() + 
                        "\n3: " + repScores[2].ToString() + "\n4: " + repScores[3].ToString() + "\n5: " +
                        repScores[4].ToString();
                    break;
                case 10:
                    scores.Content = "";
                    break;
                case 15:
                    scores.Content = "";
                    break;
                case 20:
                    scores.Content = "";
                    break;
                case 25:
                    scores.Content = "";
                    break;
                case 30:
                    scores.Content = "";
                    break;
            }
        }

        private void menuBtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            mw.Show();
            this.Close();
        }
    }
}
