﻿using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using TrelloKinectControl.Gestures;
using TrelloKinectControl;
using System.Windows.Forms;

namespace TrelloKinectControl.Kinect
{
    class KinectControl
    {

        System.Timers.Timer timer;
        KinectSensor kinectSensor;
        private double TIMER_DELAY = 500;

        private GestureFinder gestureFinder;
        private TrelloGestureManager trelloGestureManager;
        private bool suspended = false;
        private BrowserForm browserForm;
        
        public KinectControl(BrowserForm browserForm)
        {
            gestureFinder = new GestureFinder();
            trelloGestureManager = new TrelloGestureManager();
            this.browserForm = browserForm;
        }

        public void Start()
        {
            InitializeKinect();
            InitializeTimer();
            InitializeTrello();
        }
        
        public void Stop()
        {
            StopTimer();
            // Give he last timer a chance before stopping the kinect
            System.Threading.Thread.Sleep(700);
            StopKinect();
        }
        
        private void kinect_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            if (!suspended)
            {
                SkeletonFrame frame = e.OpenSkeletonFrame();
                if (frame == null)
                {
                    return;
                }
                if (frame.SkeletonArrayLength == 0)
                {
                    return;
                }
                Skeleton[] skeletons = new Skeleton[frame.SkeletonArrayLength];
                frame.CopySkeletonDataTo(skeletons);
                Skeleton skeleton = skeletons[0];
                if (skeleton != null)
                {
                    ProcessSkeleton(skeleton);
                };
            }
        }

        private Skeleton FindSkeleton()
        {
            var skeletonData = kinectSensor.SkeletonStream.OpenNextFrame(200);
            if (skeletonData != null)
            {
                Skeleton[] skeletons = new Skeleton[skeletonData.SkeletonArrayLength];
                skeletonData.CopySkeletonDataTo(skeletons);
                return skeletons.First();
            }
            return null;
        }

        private void ProcessSkeleton(Skeleton skeleton)
        {
            Gesture gesture = gestureFinder.GetGesture(skeleton);
            if (gesture == Gesture.ToggleAssign || gesture == Gesture.View)
            {
                DelayProcessingNextGesture();
            }
            trelloGestureManager.ProcessGesture(gesture);
            UpdateForm(skeleton);

        }

        private void UpdateForm(Skeleton skeleton)
        {
            float handDistance = skeleton.Joints[JointType.HandRight].Position.Z;
            browserForm.UpdateProgressBar(handDistance == 0.0f ? 2.0F : handDistance < 0.9f ? 0.9f : handDistance);
        }

        private void InitializeKinect()
        {
            foreach (KinectSensor kinect in KinectSensor.KinectSensors)
            {
                if (kinect.Status == KinectStatus.Connected)
                {
                    kinectSensor = kinect;
                    kinectSensor.SkeletonStream.EnableTrackingInNearRange = true;
                    kinectSensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Default;
                    kinectSensor.SkeletonStream.Enable(SmoothingParams());
                    kinect.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(kinect_SkeletonFrameReady);
                    kinectSensor.Start();
                    break;
                }
            }
        }

        private static TransformSmoothParameters SmoothingParams()
        {
            TransformSmoothParameters verySmoothParam = new TransformSmoothParameters();
            {
                verySmoothParam.Smoothing = 0.7f;
                verySmoothParam.Correction = 0.3f;
                verySmoothParam.Prediction = 1.0f;
                verySmoothParam.JitterRadius = 1.0f;
                verySmoothParam.MaxDeviationRadius = 1.0f;
            };

            TransformSmoothParameters smoothParam = new TransformSmoothParameters();
            {
                smoothParam.Smoothing = 0.5f;
                smoothParam.Correction = 0.1f;
                smoothParam.Prediction = 0.5f;
                smoothParam.JitterRadius = 0.1f;
                smoothParam.MaxDeviationRadius = 0.1f;
            };

            TransformSmoothParameters fastSmoothingParam = new TransformSmoothParameters();
            {
                fastSmoothingParam.Smoothing = 0.5f;
                fastSmoothingParam.Correction = 0.5f;
                fastSmoothingParam.Prediction = 0.5f;
                fastSmoothingParam.JitterRadius = 0.05f;
                fastSmoothingParam.MaxDeviationRadius = 0.04f;
            };

            return verySmoothParam;
        }

        private void InitializeTrello()
        {
            trelloGestureManager.ResetCursorPosition();
        }

        private void InitializeTimer()
        {
            timer = new System.Timers.Timer(TIMER_DELAY);
            timer.AutoReset = false;
            timer.Elapsed += new ElapsedEventHandler(_enableProcessing);
        }

        private void DelayProcessingNextGesture()
        {
            System.Diagnostics.Debug.WriteLine("$$ suspending");
            this.suspended = true;
            timer.Start();
        }

        private void _enableProcessing(object sender, ElapsedEventArgs e)
        {
            this.suspended = false;
            System.Diagnostics.Debug.WriteLine("$$ unsuspending");
        }

        private void StopTimer()
        {
            if (timer != null)
            {
                timer.Stop();
                timer = null;
            }
        }

        private void StopKinect()
        {
            if (kinectSensor != null)
            {
                kinectSensor.Stop();
                kinectSensor = null;
            }
        }
    }
}
