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

        // space for the recording
        private Dictionary<int, Dictionary<JointType, CameraSpacePoint>> bodyIndexRecording = null;

        private int currentFrameCount;

        Canvas body;
        IList<Body> bodies;
        CoordinateMapper cm;
        MultiSourceFrameReader reader;
        private const double JointThickness = 60;
        private readonly Brush trackedJointBrush = new SolidColorBrush(Color.FromArgb(255, 68, 192, 68));
        private readonly Brush inferredJointBrush = Brushes.Yellow;
        private readonly Brush pathBrush = new SolidColorBrush(Color.FromArgb(150, 150, 150, 90));
        //The path the user needs to follow has been drawn
        private bool pathDrawn = false;
        private bool exerciseStarted = false;
        private CameraSpacePoint initialKnee;
        private CameraSpacePoint initialFoot;
        private List<CameraSpacePoint> footPoints = new List<CameraSpacePoint>();
        private List<double> repScores = new List<double>();

        public int ExtensionReps;
        public string ExtensionLR;
        public int BendReps;
        public string BendLR;
        public int SquatReps;
        public string exercise;
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

            // allocate space for the recording (not sure how long, so max for now)
            this.bodyIndexRecording = new Dictionary<int, Dictionary<JointType, CameraSpacePoint>>();

            this.currentFrameCount = 0;

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

            // At this point, we are done recording. this.bodyIndexRecording contains the data we just recorded
            // TODO get the .xef file to conform to a Dictionary<int, Dictionary<JointType, CameraSpacePoint>> type

            // similarity_score = CompareExercise(this.bodyIndexRecording, previous_recording_as_correct_type) 

        }
        /// <summary>
        /// Takes in two representations of an exercise and returns an integer indicating how similar they are.
        /// </summary>
        internal double CompareExercise(Dictionary<int, Dictionary<JointType, CameraSpacePoint>> input, Dictionary<int, Dictionary<JointType, CameraSpacePoint>> recorded)
        {
            Dictionary<JointType, CameraSpacePoint> inputJoints, recordedJoints;
            int currTime = 0;
            double score = 0.0; // similarity score, lower is better (more similar)
            // while both inputs still have data
            while (input[currTime] != null && recorded[currTime] != null)
            {
                inputJoints = input[currTime];
                recordedJoints = recorded[currTime];
                foreach(JointType jointType in inputJoints.Keys)
                {
                    if (!recordedJoints.ContainsKey(jointType))
                    {
                        // encode some error behavior here I guess, this is when the recording
                        // is missing some data that the input contain
                    }
                    else
                    {
                        score += ComputeHistogram(inputJoints[jointType], recordedJoints[jointType]);
                    }
                }
                currTime++;
            }
            return score;
        }

        internal double ComputeHistogram(CameraSpacePoint input, CameraSpacePoint recorded)
        {
            // This is a very naive program, and as such just computes the Euclidean distance between the points
            // This algorithm can be tweaked by multiplying all values in an area (say we isolate the area around a knee) by a scalar
            // This will result in the Euclidean distance for those values increasing by an amount directly proportional to the scalar,
            // Effectively weighting those values higher than other values.
            return Math.Sqrt(Math.Pow((input.X - recorded.X), 2) + Math.Pow((input.Y - recorded.Y), 2) + Math.Pow((input.Z - recorded.Z), 2));
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
            this.currentFrameCount++;
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
          
                        //-------------------------
                        //THE FUNCTION THAT TRACKS THE JOINTS FOR A SPECIFIC EXERCISE
                        //---------------------------
                        legExtension(joints[jointType], jointPoints, joints);
                        
                    }
                    //What to do before starting a repition
                    if(!exerciseStarted)
                    {
                        legExtensionBeforeStart(joints);
                    }
                    //What to do during an exercise
                    if(exerciseStarted) {
                        legExtensionScoring();
                    }
                    
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        private void legExtension(Joint joint, Dictionary<JointType, Point> jointPoints, IReadOnlyDictionary<JointType, Joint> joints)
        {
            // this gets the position of the joints in meters
            // if aligning ankles 
            if (joint.JointType == JointType.KneeLeft || joint.JointType == JointType.FootLeft)
            {
                CameraSpacePoint position = joint.Position;
                // We have X, Y, Z coordinates of the joint now (even better than R, G, B, Z because we don't have to do edge detection)
                //this.bodyIndexRecording[this.currentFrameCount].Add(jointType, position);
                // this converts the 3D camera space point in meters to a 2D pixel on the RGB camera/video 
                ColorSpacePoint colorSpacePoint = this.cm.MapCameraPointToColorSpace(position);
                jointPoints[joint.JointType] = new Point(colorSpacePoint.X, colorSpacePoint.Y);
                DrawCircleAt(joints, jointPoints, joint.JointType);
                if (exerciseStarted && joint.JointType == JointType.FootLeft)
                {
                    footPoints.Add(position);
                }
            }
        }
     
        private void legExtensionBeforeStart(IReadOnlyDictionary<JointType, Joint> joints)
        {
            JointType joint1 = JointType.KneeLeft;
            JointType joint2 = JointType.FootLeft;
            //Get initial points
            initialKnee = joints[joint1].Position;
            initialFoot = joints[joint2].Position;
            footPoints = new List<CameraSpacePoint>();
            repScores.Add(0);

        }

        private void legExtensionScoring()
        {
            int exerciseNum = ExtensionReps;
            double[] scoreArray = new double[exerciseNum];
            int i = 0;
            //while (i < exerciseNum)
            //{
                CameraSpacePoint maxFoot = footPoints.OrderBy(t => t.Y).Last();//Max Y value
                //Calculate STD of X
                double M = 0.0;
                double S = 0.0;
                int k = 1;
                foreach (CameraSpacePoint point in footPoints)
                {
                    double value = point.X;
                    double tmpM = M;
                    M += (value - tmpM) / k;
                    S += (value - tmpM) * (value - M);
                    k++;
                }
                double stdX = Math.Sqrt(S / (k - 2));

                Feedback.Content = (initialKnee.Y - maxFoot.Y);
                string feedbackContent = "";
                if (initialKnee.Y - maxFoot.Y < 0)
                {
                    feedbackContent += "Done with Rep.\n";
                }
                if (Math.Abs(stdX) > 0.045)
                {
                    feedbackContent += "Stop moving your foot so much.";
                }
                double stdX_Score = -200 * stdX + 15;
                //If the foot goes above the knee, automatically give a 10
                double diffHeight = Math.Abs(maxFoot.Y - initialKnee.Y);
                double heightScore = 10 - (Math.Min((diffHeight / Math.Abs(initialFoot.Y - initialKnee.Y)) * 10, 10));
                if (maxFoot.Y > initialKnee.Y)
                {
                    heightScore = 10;
                }

                if (stdX_Score > 10)
                {
                    stdX_Score = 10;
                }
                else if (stdX_Score < 0)
                {
                    stdX_Score = 0;
                }

                double avgScore = (heightScore + stdX_Score) / 2;
                //if () {//done with rep
                 //   scoreArray[i] = avgScore;
                   // i++;
                //}
                repScores[repScores.Count - 1] = avgScore;
                Feedback.Content = avgScore.ToString() + feedbackContent;
                ColorSpacePoint colorSpacePoint = this.cm.MapCameraPointToColorSpace(maxFoot);
                DrawEllipseOnCanvas(inferredJointBrush, new Point(colorSpacePoint.X, colorSpacePoint.Y), 50, pathCanvas);

            //} //may want to move end of while to after done with rep
        }

        private void start_clicked(object sender, RoutedEventArgs e)
        {
            exerciseStarted = !exerciseStarted;

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
            else if (trackingState == TrackingState.Inferred)
            {
                drawBrush = this.inferredJointBrush;
            }

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
            DrawEllipseOnCanvas(drawBrush, point, JointThickness, bodyCanvas);
        }

        /// <summary>
        /// Draws an elipse
        /// </summary>
        private void DrawEllipseOnCanvas(Brush drawBrush, Point point, double JointThickness, Canvas c)
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

            c.Children.Add(jointEllipse);
        }

        /// <summary>
        /// Draws a path between two joints
        /// </summary>
        private void DrawPath(IReadOnlyDictionary<JointType, Point> jointPoints, JointType joint1, JointType joint2)
        {
            // draws a circle for the tip of the hand

            Brush drawBrush = null;

            if (jointPoints.ContainsKey(joint1) && jointPoints.ContainsKey(joint2))
            {
                drawBrush = this.pathBrush;
                Point center = jointPoints[joint1];
                Point p2 = jointPoints[joint2];
                double radius = Math.Sqrt((Math.Pow(center.X - p2.X, 2) + Math.Pow(center.Y - p2.Y, 2)));
                System.Windows.Shapes.Path arc_path = new System.Windows.Shapes.Path();
                arc_path.Stroke = drawBrush;
                arc_path.StrokeThickness = 50;
                Canvas.SetLeft(arc_path, 0);
                Canvas.SetTop(arc_path, 0);
                PathGeometry pathGeometry = new PathGeometry();
                PathFigure pathFigure = new PathFigure();
                ArcSegment arcSegment = new ArcSegment();
                arcSegment.IsLargeArc = false;
                //Set start of arc
                Point start = new Point(center.X + radius, center.Y);
                pathFigure.StartPoint = start;//new Point(center.X + radius * Math.Cos(start_angle), center.Y + radius * Math.Sin(start_angle));
                //set end point of arc.
                arcSegment.Point = p2;
                arcSegment.Size = new Size(radius, radius);
                arcSegment.SweepDirection = SweepDirection.Clockwise;

                pathFigure.Segments.Add(arcSegment);
                pathGeometry.Figures.Add(pathFigure);
                arc_path.Data = pathGeometry;
                pathCanvas.Children.Add(arc_path);
            }
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
