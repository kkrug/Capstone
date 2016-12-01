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
    /// Interaction logic for BendVideo.xaml
    /// </summary>
    public partial class BendVideo : Window
    {
        public BendVideo()
        {

            InitializeComponent();

            KneeBendVideo.Play();
        }

        private void replayBend_Click(object sender, RoutedEventArgs e)
        {
            KneeBendVideo.Position = TimeSpan.FromSeconds(0);
            KneeBendVideo.Play();
        }
    }
}
