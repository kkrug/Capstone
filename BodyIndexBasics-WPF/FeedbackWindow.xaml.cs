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

        public FeedbackWindow(string exercise, List<double> repScores)
        {
            
            InitializeComponent();
            setValues(exercise, repScores);
        }

        private void setValues(string exercise, List<double> repScores)
        {
            if (exercise == "extension")
                exerciseName.Content = "Knee Extension";
            else if (exercise == "bend")
                exerciseName.Content = "Knee Bend";
            else if (exercise == "squat")
                exerciseName.Content = "Squat";

            int repNums = repScores.Count - 1;
            double sum = 0;

            for(int i=0; i<repScores.Count; i++)
            {
                repScores[i] = Math.Round(repScores[i], 2);
                sum = sum + repScores[i];
            }

            avgScore = Math.Round(sum / repNums, 2);
            average.Content = Math.Round(avgScore, 2);

            switch (repNums)
            {
                case 5:
                    scores.Content = "1:\t" + repScores[0].ToString() + "\n2:\t" + repScores[1].ToString() +
                        "\n3:\t" + repScores[2].ToString() + "\n4:\t" + repScores[3].ToString() + "\n5:\t" +
                        repScores[4].ToString();
                    scores.FontSize = 18;
                    break;
                case 10:
                    scores.Content = "1:\t" + repScores[0].ToString() + "\t\t6:\t" + repScores[5].ToString() +
                        "\n2:\t" + repScores[1].ToString() + "\t\t7:\t" + repScores[6].ToString() + "\n3:\t" +
                        repScores[2].ToString() + "\t\t8:\t" + repScores[7].ToString() +
                        "\n4:\t" + repScores[3].ToString() + "\t\t9:\t" + repScores[8].ToString() + "\n5:\t" +
                        repScores[4].ToString() + "\t\t10:\t" + repScores[9].ToString();
                    scores.FontSize = 18;
                    break;
                case 15:
                    scores.Content = "";
                    scores.FontSize = 16;
                    break;
                case 20:
                    scores.Content = "";
                    scores.FontSize = 16;
                    break;
                case 25:
                    scores.Content = "";
                    scores.FontSize = 16;
                    break;
                case 30:
                    scores.Content = "";
                    scores.FontSize = 16;
                    break;
            }
        }

        private void menuBtn_Click(object sender, RoutedEventArgs e)
        {
            SelectionWindow sw = new SelectionWindow();
            sw.Show();
            this.Close();
        }
    }
}
