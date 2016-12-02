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
                    scores.Content = "1:\t" + repScores[0].ToString() + "\t\t11:\t" + repScores[10].ToString() +
                        "\n2:\t" + repScores[1].ToString() + "\t\t12:\t" + repScores[11].ToString() + "\n3:\t" +
                        repScores[2].ToString() + "\t\t13:\t" + repScores[12].ToString() +
                        "\n4:\t" + repScores[3].ToString() + "\t\t14:\t" + repScores[13].ToString() + "\n5:\t" +
                        repScores[4].ToString() + "\t\t15:\t" + repScores[14].ToString() + "\n6:\t" + repScores[5].ToString() +
                        "\n7:\t" + repScores[6].ToString() + "\n8:\t" + repScores[7].ToString() + "\n9:\t" +
                        repScores[8].ToString() + "\n10:\t" + repScores[9].ToString();
                    scores.FontSize = 12;
                    break;
                case 20:
                    scores.Content = "1:\t" + repScores[0].ToString() + "\t\t11:\t" + repScores[10].ToString() +
                        "\n2:\t" + repScores[1].ToString() + "\t\t12:\t" + repScores[11].ToString() + "\n3:\t" +
                        repScores[2].ToString() + "\t\t13:\t" + repScores[12].ToString() +
                        "\n4:\t" + repScores[3].ToString() + "\t\t14:\t" + repScores[13].ToString() + "\n5:\t" +
                        repScores[4].ToString() + "\t\t15:\t" + repScores[14].ToString() + "\n6:\t" + repScores[5].ToString() +
                        "\t\t16:\t" + repScores[15].ToString() + "\n7:\t" + repScores[6].ToString() + "\t\t17:\t" +
                        repScores[16].ToString() + "\n8:\t" + repScores[7].ToString() + "\t\t18:\t" + repScores[17].ToString() +
                        "\n9:\t" + repScores[8].ToString() + "\t\t19:\t" + repScores[18].ToString() + "\n10:\t" + repScores[9].ToString()
                        + "\t\t20:\t" + repScores[19].ToString();
                    scores.FontSize = 12;
                    break;
                case 25:
                    scores.Content = "1:\t" + repScores[0].ToString() + "\t11:\t" + repScores[10].ToString() + "\t21:\t" + repScores[20].ToString() +
                        "\n2:\t" + repScores[1].ToString() + "\t12:\t" + repScores[11].ToString() + "\t22:\t" + repScores[21].ToString() + 
                        "\n3:\t" + repScores[2].ToString() + "\t13:\t" + repScores[12].ToString() + "\t23:\t" + repScores[22].ToString() +
                        "\n4:\t" + repScores[3].ToString() + "\t14:\t" + repScores[13].ToString() + "\t24:\t" + repScores[23].ToString() + 
                        "\n5:\t" + repScores[4].ToString() + "\t15:\t" + repScores[14].ToString() + "\t25:\t" + repScores[24].ToString() + 
                        "\n6:\t" + repScores[5].ToString() + "\t16:\t" + repScores[15].ToString() + "\n7:\t" + repScores[6].ToString() + "\t17:\t" +
                        repScores[16].ToString() + "\n8:\t" + repScores[7].ToString() + "\t18:\t" + repScores[17].ToString() +
                        "\n9:\t" + repScores[8].ToString() + "\t19:\t" + repScores[18].ToString() + "\n10:\t" + repScores[9].ToString()
                        + "\t20:\t" + repScores[19].ToString();
                    scores.FontSize = 12;
                    break;
                case 30:
                    scores.Content = "1:\t" + repScores[0].ToString() + "\t11:\t" + repScores[10].ToString() + "\t21:\t" + repScores[20].ToString() +
                        "\n2:\t" + repScores[1].ToString() + "\t12:\t" + repScores[11].ToString() + "\t22:\t" + repScores[21].ToString() +
                        "\n3:\t" + repScores[2].ToString() + "\t13:\t" + repScores[12].ToString() + "\t23:\t" + repScores[22].ToString() +
                        "\n4:\t" + repScores[3].ToString() + "\t14:\t" + repScores[13].ToString() + "\t24:\t" + repScores[23].ToString() +
                        "\n5:\t" + repScores[4].ToString() + "\t15:\t" + repScores[14].ToString() + "\t25:\t" + repScores[24].ToString() +
                        "\n6:\t" + repScores[5].ToString() + "\t16:\t" + repScores[15].ToString() + "\t26:\t" + repScores[25].ToString() + 
                        "\n7:\t" + repScores[6].ToString() + "\t17:\t" + repScores[16].ToString() + "\t27:\t" + repScores[26].ToString() + 
                        "\n8:\t" + repScores[7].ToString() + "\t18:\t" + repScores[17].ToString() + "\t28:\t" + repScores[27].ToString() +
                        "\n9:\t" + repScores[8].ToString() + "\t19:\t" + repScores[18].ToString() + "\t29:\t" + repScores[28].ToString() + 
                        "\n10:\t" + repScores[9].ToString()+ "\t20:\t" + repScores[19].ToString() + "\t30:\t" + repScores[29].ToString();
                    scores.FontSize = 12;
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
