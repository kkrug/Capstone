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
    /// Interaction logic for ExtensionVideo.xaml
    /// </summary>
    public partial class ExtensionVideo : Window
    {
        public ExtensionVideo()
        {

            InitializeComponent();

            KneeExtensionVideo.Play();
        }

        private void replayExtension_Click(object sender, RoutedEventArgs e)
        {
            KneeExtensionVideo.Position = TimeSpan.FromSeconds(0);
            KneeExtensionVideo.Play();
        }
    }
}
