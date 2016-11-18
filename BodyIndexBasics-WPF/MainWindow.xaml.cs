//------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.BodyIndexBasics
{
    using Microsoft.Kinect;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;
    using System.Windows.Threading;

    /// <summary>
    /// Interaction logic for the MainWindow
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Active Kinect sensor
        /// </summary>
        private KinectSensor kinectSensor = null;
        /// <summary>
        /// the bitmap video data for the kinect 
        /// </summary>
        private WriteableBitmap colorBitmap = null;
        /// <summary>
        /// Size of the RGB pixel in the bitmap
        /// </summary>
        private const int BytesPerPixel = 4;

        /// <summary>
        /// Description of the data contained in the body index frame
        /// </summary>
        private FrameDescription bodyIndexFrameDescription = null;

        /// <summary>
        /// Intermediate storage for frame data converted to color
        /// </summary>
        private uint[] bodyIndexPixels = null;

        Canvas body;
        IList<Body> bodies;
        CoordinateMapper cm;
        MultiSourceFrameReader reader;
        private const double JointThickness = 60;
        private readonly Brush trackedJointBrush = new SolidColorBrush(Color.FromArgb(255, 68, 192, 68));
        private readonly Brush inferredJointBrush = Brushes.Yellow;
        private readonly Brush pathBrush = new SolidColorBrush(Color.FromArgb(100, 100, 100, 68));
        //The path the user needs to follow has been drawn
        private bool pathDrawn = false;

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            // get the kinectSensor object
            this.kinectSensor = KinectSensor.GetDefault();

            this.bodyIndexFrameDescription = this.kinectSensor.BodyIndexFrameSource.FrameDescription;

            // allocate space to put the pixels being converted
            this.bodyIndexPixels = new uint[this.bodyIndexFrameDescription.Width * this.bodyIndexFrameDescription.Height];


            // create the colorFrameDescription from the ColorFrameSource using Bgra format
            FrameDescription colorFrameDescription = this.kinectSensor.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Bgra);

            //Initalize the bitmap to show kinect data
            this.colorBitmap = new WriteableBitmap(colorFrameDescription.Width, colorFrameDescription.Height, 96.0, 96.0, PixelFormats.Bgr32, null);

            // use the window object as the view model in this simple example
            this.DataContext = this;

            // open the sensor
            this.kinectSensor.Open();

            //Initialize the kinect
            InitializeKinect(bodyCanvas, kinectVideoImage);
            // initialize the components (controls) of the window
            this.InitializeComponent();
        }

        /// <summary>
        /// Sets up the kinect settings
        /// </summary>
        internal void InitializeKinect(Canvas bodyCanvas, Image kinectVideoImage)
        {
            this.body = bodyCanvas;
            this.kinectVideoImage = kinectVideoImage;

            // get the coordinate mapper
            this.cm = this.kinectSensor.CoordinateMapper;

            // create the colorFrameDescription from the ColorFrameSource using Bgra format
            FrameDescription colorFrameDescription = this.kinectSensor.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Bgra);

            // create the bitmap to display
            this.colorBitmap = new WriteableBitmap(colorFrameDescription.Width, colorFrameDescription.Height, 96.0, 96.0, PixelFormats.Bgr32, null);

            reader = kinectSensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Body);
            reader.MultiSourceFrameArrived += Reader_MultiSourceFrameArrived;
        }

        /// <summary>
        /// What to do with each kinect frame
        /// </summary>
        void Reader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            var reference = e.FrameReference.AcquireFrame();

            // Body
            using (var frame = reference.BodyFrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    bodyCanvas.Children.Clear();
                    bodies = new Body[frame.BodyFrameSource.BodyCount];
                    frame.GetAndRefreshBodyData(bodies);
                    this.TrackSkeleton(bodies);
                }
            }

            // Color - get this last for performance issues
            // Use the color camera for augmented reality mode
            using (var colorFrame = reference.ColorFrameReference.AcquireFrame())
                if (colorFrame != null)
                {
                    FrameDescription colorFrameDescription = colorFrame.FrameDescription;

                    using (KinectBuffer colorBuffer = colorFrame.LockRawImageBuffer())
                    {
                        this.colorBitmap.Lock();

                        // verify data and write the new color frame data to the display bitmap
                        if ((colorFrameDescription.Width == this.colorBitmap.PixelWidth) && (colorFrameDescription.Height == this.colorBitmap.PixelHeight))
                        {
                            colorFrame.CopyConvertedFrameDataToIntPtr(
                                this.colorBitmap.BackBuffer,
                                (uint)(colorFrameDescription.Width * colorFrameDescription.Height * 4),
                                ColorImageFormat.Bgra);

                            this.colorBitmap.AddDirtyRect(new Int32Rect(0, 0, this.colorBitmap.PixelWidth, this.colorBitmap.PixelHeight));
                        }

                        this.colorBitmap.Unlock();

                        kinectVideoImage.Source = this.colorBitmap;
                    }
                }

        }

        /// <summary>
        /// Starts tracking the skeleton and does whatever you want to it
        /// </summary>
        private void TrackSkeleton(IList<Body> bodies)
        {
            foreach (Body body in bodies)
            {
                // a body that is tracked has a ID
                if (body.IsTracked)
                {
                    IReadOnlyDictionary<JointType, Joint> joints = body.Joints;

                    // convert the joint points to depth (display) space
                    Dictionary<JointType, Point> jointPoints = new Dictionary<JointType, Point>();

                    foreach (JointType jointType in joints.Keys)
                    {
                        // this gets the position of the joints in meters
                        // if aligning ankles 
                        if (jointType == JointType.WristLeft || jointType == JointType.ElbowLeft)
                        {
                            CameraSpacePoint position = joints[jointType].Position;
                            // this converts the 3D camera space point in meters to a 2D pixel on the RGB camera/video
                            ColorSpacePoint colorSpacePoint = this.cm.MapCameraPointToColorSpace(position);
                            jointPoints[jointType] = new Point(colorSpacePoint.X, colorSpacePoint.Y);
                            DrawCircleAt(joints, jointPoints, jointType);
                        }
                    }
                }
            }

        }


        /// <summary>
        /// Draws a circle on the specified joint
        /// </summary>
        private void DrawCircleAt(IReadOnlyDictionary<JointType, Joint> joints, Dictionary<JointType, Point> jointPoints, JointType jointType)
        {
            // draws a circle for the tip of the hand

            Brush drawBrush = null;

            TrackingState trackingState = joints[jointType].TrackingState;

            if (trackingState == TrackingState.Tracked)
            {
                drawBrush = this.trackedJointBrush;
            }
  //          else if (trackingState == TrackingState.Inferred)
  //          {
  //              drawBrush = this.inferredJointBrush;
  //          }

            if (drawBrush != null)
            {
                this.DrawEllipse(drawBrush, jointPoints[jointType], JointThickness);
            }
        }

        /// <summary>
        /// Draws an elipse
        /// </summary>
        private void DrawEllipse(Brush drawBrush, Point point, double JointThickness)
        {
            Ellipse jointEllipse = new Ellipse();
            jointEllipse.Stroke = drawBrush;
            jointEllipse.StrokeThickness = 5;
            jointEllipse.Fill = drawBrush;
            jointEllipse.Height = JointThickness;
            jointEllipse.Width = JointThickness;

            Point pt = ConvertToCanvas(point);

            Canvas.SetLeft(jointEllipse, pt.X - (JointThickness / 2));
            Canvas.SetTop(jointEllipse, pt.Y - (JointThickness / 2));

            bodyCanvas.Children.Add(jointEllipse);
        }



        /// <summary>
        /// Converts to canvas coordinates
        /// </summary>
        private Point ConvertToCanvas(Point point)
        {

            // color stream is coming in at 1920(w)×1080(h) 
            Point pt = new Point();
            pt.X = (double)(point.X / 1920.0 * bodyCanvas.ActualWidth);
            pt.Y = (double)(point.Y / 1080.0 * bodyCanvas.ActualHeight);

            // if off screen, return 0
            if (pt.X < 0 || pt.Y < 0)
            {
                pt.X = 0;
                pt.Y = 0;
            }

            return pt;
        }




        /// <summary>
        /// INotifyPropertyChangedPropertyChanged event to allow window controls to bind to changeable data
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the bitmap to display
        /// </summary>
        public ImageSource ImageSource
        {
            get
            {
                return this.colorBitmap;
            }
        }
        /// <summary>
        /// Renders color pixels into the writeableBitmap.
        /// </summary>
        private void RenderBodyIndexPixels()
        {
            this.colorBitmap.WritePixels(
                new Int32Rect(0, 0, this.colorBitmap.PixelWidth, this.colorBitmap.PixelHeight),
                this.bodyIndexPixels,
                this.colorBitmap.PixelWidth * (int)BytesPerPixel,
                0);
        }

        /// <summary>
        /// Execute shutdown tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }

    }
}
