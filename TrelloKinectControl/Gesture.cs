﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrelloKinectControl.Mouse;

namespace TrelloKinectControl.Gestures
{
    public enum Gesture
    {
        HandUp,
        HandDown,
        HandLeft,
        HandRight,
        None,
        NotInteracting,
        PickUp,
        PutDown
    }

    public static class GestureExtensions
    {
        private static int CARD_HEIGHT = 30;
        private static int COLUMN_WIDTH = 125;
        private static int MIN_Y = 190;
        private static int MIN_X = 200;
        private static int MAX_X = 640;
        public static void MouseAction(this Gesture gesture)
        {
            switch (gesture)
            {
                case Gesture.PickUp:
                    MouseInterop.LeftDown();
                    System.Threading.Thread.Sleep(20);
                    MouseInterop.Move(0, 3);
                    System.Threading.Thread.Sleep(20);
                    MouseInterop.Move(0, 3);
                    System.Threading.Thread.Sleep(20);
                    MouseInterop.Move(0, 3);
                    System.Threading.Thread.Sleep(20);
                    MouseInterop.Move(0, 3);
                    System.Threading.Thread.Sleep(20);
                    MouseInterop.Move(0, -3);
                    System.Threading.Thread.Sleep(20);
                    MouseInterop.Move(0, -3);
                    System.Threading.Thread.Sleep(20);
                    MouseInterop.Move(0, -3);
                    System.Threading.Thread.Sleep(20);
                    MouseInterop.Move(0, -3);
                    break;
                case Gesture.PutDown:
                    MouseInterop.LeftUp();
                    break;
                case Gesture.HandUp:
                    if (System.Windows.Forms.Cursor.Position.Y > MIN_Y)
                    {
                        MouseInterop.Move(0, 0 - CARD_HEIGHT);
                    }
                    break;
                case Gesture.HandDown:
                    MouseInterop.Move(0, CARD_HEIGHT);
                    break;
                case Gesture.HandLeft:
                    if (System.Windows.Forms.Cursor.Position.X > MIN_X)
                    {
                        MouseInterop.Move(0 - COLUMN_WIDTH, 0);
                    }
                    break;
                case Gesture.HandRight:
                    if (System.Windows.Forms.Cursor.Position.X < MAX_X)
                    {
                        MouseInterop.Move(COLUMN_WIDTH, 0);
                    }
                    break;
                default:
                    break;
            }
        }
    }
}