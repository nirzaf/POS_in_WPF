using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace PosControls.Helpers
{
    public static class MathHelper
    {
        public static double Clamp(this double value, double min, double max)
        {
            if (value < min)
                value = min;
            if (value > max)
                value = max;
            return value;
        }

        public static int Clamp(this int value, int min, int max)
        {
            if (value < min)
                value = min;
            if (value > max)
                value = max;
            return value;
        }

        public static short Clamp(this short value, short min, short max)
        {
            if (value < min)
                value = min;
            if (value > max)
                value = max;
            return value;
        }

        public static double ToStandardAngle(double angle)
        {
            while (angle < 0)
                angle += 360;
            while (angle >= 360)
                angle -= 360;
            return angle;
        }

        public static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }

        public static double RadiansToDegrees(double radians)
        {
            return ToStandardAngle(radians * 180 / Math.PI);
        }

        public static Point FindPointOnCircle(Point center, double diameter, double angleDegrees)
        {
            angleDegrees = ToStandardAngle(angleDegrees);
            double angleRadians = DegreesToRadians(angleDegrees);
            double x = center.X + (diameter * Math.Cos(angleRadians));
            double y = center.Y + (diameter * Math.Sin(angleRadians));
            return new Point(x, y);
        }

        public static double FindAngleToPoint(Point from, Point to)
        {
            return Math.Atan2(to.Y - from.Y, to.X - from.X);
        }

        public static double FindDistanceBetweenPoints(Point from, Point to)
        {
            return Math.Sqrt(
                Math.Pow((to.X - from.X), 2) +
                Math.Pow((to.Y - from.Y), 2));
        }

    }
}
