#region File Information
/*
 * Copyright (C) 2007-2014 David Rudie
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02111, USA.
 */
#endregion

namespace Library.Math
{
    using System;
    using System.Drawing;

    // This class provides some helpful math functions for use in plugins.
    public static class MathFunctions
    {
        // This will calculate a new aspect-ratio by dividing the source aspect-ratio by the destination aspect-ratio.
        public static float CalculateAspectRatioInRelationTo(float sourceAspectRatio, float destinationAspectRatio)
        {
            return sourceAspectRatio / destinationAspectRatio;
        }

        // This will calculate a new field of view based on a provided source aspect-ratio and destination aspect-ratio.
        public static float CalculateFieldOfViewDegrees(float sourceAspectRatio, float destinationAspectRatio, float fieldOfView)
        {
            double newAspectRatio = destinationAspectRatio / sourceAspectRatio;
            double fieldOfView_d = (double)fieldOfView.DegreesToRadians() / 2;

            float newFieldOfView = 2 * (float)System.Math.Atan(newAspectRatio * System.Math.Tan(fieldOfView_d));

            return newFieldOfView.RadiansToDegrees();
        }

        // This will calculate a new field of view based on a provided source aspect-ratio and destination aspect-ratio.
        public static float CalculateFieldOfViewDegrees(float sourceAspectRatio, float destinationAspectRatio, float fieldOfView, float maximumFieldOfView)
        {
            double newAspectRatio = destinationAspectRatio / sourceAspectRatio;
            double fieldOfView_d = (double)fieldOfView.DegreesToRadians() / 2;

            float newFieldOfView = 2 * (float)System.Math.Atan(newAspectRatio * System.Math.Tan(fieldOfView_d));

            newFieldOfView = newFieldOfView.RadiansToDegrees();

            if (newFieldOfView > maximumFieldOfView)
            {
                return maximumFieldOfView;
            }
            else
            {
                return newFieldOfView;
            }
        }

        // This will calculate a new field of view based on a provided source aspect-ratio and destination aspect-ratio.
        public static float CalculateFieldOfViewRadians(float sourceAspectRatio, float destinationAspectRatio, float fieldOfView)
        {
            double newAspectRatio = destinationAspectRatio / sourceAspectRatio;
            double fieldOfView_d = (double)fieldOfView / 2;

            float newFieldOfView = 2 * (float)System.Math.Atan(newAspectRatio * System.Math.Tan(fieldOfView_d));

            return newFieldOfView;
        }

        // This converts degrees into radians.
        public static float DegreesToRadians(this float degrees)
        {
            return (float)(degrees * (System.Math.PI / 180));
        }

        // This converts radians into degrees.
        public static float RadiansToDegrees(this float radians)
        {
            return (float)(radians * (180 / System.Math.PI));
        }

        // This will figure out the greatest common divisor of the two provided values.
        public static int GreatestCommonDivisor(int one, int two)
        {
            while (one != 0 && two != 0)
            {
                if (one > two)
                {
                    one %= two;
                }
                else
                {
                    two %= one;
                }
            }

            if (one == 0)
            {
                return two;
            }
            else
            {
                return one;
            }
        }

        // This will figure out which aspect-ratio to use based on the selection the user chose.
        public static float GetAspectRatio(this int aspectRatioSelection, int aspectX, int aspectY)
        {
            switch (aspectRatioSelection)
            {
                case 0:
                    return 5.0f / 4.0f;
                case 1:
                    return 4.0f / 3.0f;
                case 2:
                    return 8.0f / 5.0f;
                case 3:
                    return 5.0f / 3.0f;
                case 4:
                    return 128.0f / 75.0f;
                case 5:
                    return 16.0f / 9.0f;
                case 6:
                    return 5.0f / 2.0f;
                case 7:
                    return 8.0f / 3.0f;
                case 8:
                    return 16.0f / 5.0f;
                case 9:
                    return 10.0f / 3.0f;
                case 10:
                    return 256.0f / 75.0f;
                case 11:
                    return 32.0f / 9.0f;
                case 12:
                    return 15.0f / 4.0f;
                case 13:
                    return 4.0f / 1.0f;
                case 14:
                    return 24.0f / 5.0f;
                case 15:
                    return 5.0f / 1.0f;
                case 16:
                    return 128.0f / 25.0f;
                case 17:
                    return 16.0f / 3.0f;
                case 18:
                    return 55.0f / 23.0f;
                case 19:
                    return (float)aspectX / (float)aspectY;
                default:
                    return 16.0f / 9.0f;
            }
        }

        // This will figure out which aspect-ratio to use based on the selection the user chose.
        public static PointF GetAspectRatioPoints(this int aspectRatioSelection, int aspectX, int aspectY)
        {
            switch (aspectRatioSelection)
            {
                case 0:
                    return new PointF(5.0f, 4.0f);
                case 1:
                    return new PointF(4.0f, 3.0f);
                case 2:
                    return new PointF(8.0f, 5.0f);
                case 3:
                    return new PointF(5.0f, 3.0f);
                case 4:
                    return new PointF(128.0f, 75.0f);
                case 5:
                    return new PointF(16.0f, 9.0f);
                case 6:
                    return new PointF(5.0f, 2.0f);
                case 7:
                    return new PointF(8.0f, 3.0f);
                case 8:
                    return new PointF(16.0f, 5.0f);
                case 9:
                    return new PointF(10.0f, 3.0f);
                case 10:
                    return new PointF(256.0f, 75.0f);
                case 11:
                    return new PointF(32.0f, 9.0f);
                case 12:
                    return new PointF(15.0f, 4.0f);
                case 13:
                    return new PointF(4.0f, 1.0f);
                case 14:
                    return new PointF(24.0f, 5.0f);
                case 15:
                    return new PointF(5.0f, 1.0f);
                case 16:
                    return new PointF(128.0f, 25.0f);
                case 17:
                    return new PointF(16.0f, 3.0f);
                case 18:
                    return new PointF(55.0f, 23.0f);
                case 19:
                    return new PointF((float)aspectX, (float)aspectY);
                default:
                    return new PointF(16.0f, 9.0f);
            }
        }
    }
}
