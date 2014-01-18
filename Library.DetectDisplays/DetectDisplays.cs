#region File Information
/*
 * Copyright (C) 2012-2014 David Rudie
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

namespace Library.DetectDisplays
{
    using System;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    // This class will provide a way of gathering display information.
    public static class DetectedDisplays
    {
        #region Fields

        // This field will hold the display information.
        private static DisplayInfo displayInfo;

        #endregion

        #region Public Enums

        // This enumeration is used to determine the brand of the user's video card.
        public enum BrandType
        {
            Amd,
            Nvidia,
            Unknown
        }

        #endregion

        #region Public Properties

        // Gets the name of the video card.
        public static string VideoCard { get { return displayInfo.VideoCard; } }

        // Gets the brand of the video card.
        public static BrandType Brand { get { return displayInfo.Brand; } }

        // Gets a value indicating whether an error was thrown or not.
        public static bool Error { get { return displayInfo.Error == 0 ? false : true; } }

        // Gets a value indicating the error code that was thrown.
        public static int ErrorCode { get { return displayInfo.ErrorCode; } }

        // Gets a value indicating a textual representation of the error.
        public static string ErrorString { get { return displayInfo.ErrorString; } }

        // Gets a value indicating whether a multiple-monitor mode is in use or not.
        public static bool MultipleMonitorEnabled { get { return displayInfo.MultipleMonitorEnabled == 0 ? false : true; } }

        // Gets a value indicating the number of displays in use.
        public static int DisplayCount { get { return displayInfo.DisplayCount; } }

        // Gets a value indicating the number of columns in the display setup.
        public static int ColumnCount { get { return displayInfo.ColumnCount; } }

        // Gets a value indicating the number of rows in the display setup.
        public static int RowCount { get { return displayInfo.RowCount; } }

        // Gets a value indicating whether bezel correction is in use or not.
        public static bool BezelCorrected { get { return displayInfo.BezelCorrected == 0 ? false : true; } }

        // Gets a value indicating the combined horizontal resolution.
        public static int HorizontalResolution { get { return displayInfo.HorizontalResolution; } }

        // Gets a value indicating the combined vertical resolution.
        public static int VerticalResolution { get { return displayInfo.VerticalResolution; } }

        // Gets a value indicating the aspect-ratio of the combined displays.
        public static float AspectRatio { get { return displayInfo.AspectRatio; } }

        // Gets a value indicating whether the display configuration is in landscape mode or not.
        public static bool Landscape { get { return displayInfo.Landscape == 0 ? false : true; } }

        // Gets a value indicating whether the display configuration is in portrait mode or not.
        public static bool Portrait { get { return displayInfo.Portrait == 0 ? false : true; } }

        // Gets a value indicating where the left side of a HUD should be placed.
        public static int HudLeft { get { return displayInfo.HudLeft; } }

        // Gets a value indicating where the right side of a HUD should be placed.
        public static int HudRight { get { return displayInfo.HudRight; } }

        // Gets a value indicating where the top of a HUD should be placed.
        public static int HudTop { get { return displayInfo.HudTop; } }

        // Gets a value indicating where the bottom of a HUD should be placed.
        public static int HudBottom { get { return displayInfo.HudBottom; } }

        // Gets a value indicating how wide a HUD should be.
        public static int HudWidth { get { return displayInfo.HudWidth; } }

        // Gets a value indicating how tall a HUD should be.
        public static int HudHeight { get { return displayInfo.HudHeight; } }

        #endregion

        #region Public Methods

        // Initializes the DetectDisplays library.
        public static void Initialize()
        {
            displayInfo = new DisplayInfo();
            IntPtr pointer = IntPtr.Zero;

            try
            {
                pointer = Marshal.AllocHGlobal(Marshal.SizeOf(displayInfo));
                int returnValue = DD.Initialize(pointer);

                displayInfo = (DisplayInfo)Marshal.PtrToStructure(pointer, typeof(DisplayInfo));

                // Detection failed
                if (returnValue == 0)
                {
                    InitializeNoDetectDisplays();
                }
            }
            catch
            {
                //// (Exception e)
                //// MessageBox.Show(string.Format("Exception: {0}\n\nStack Trace:\n{1}", e.Message, e.StackTrace));
                InitializeNoDetectDisplays();

                throw;
            }
            finally
            {
                Marshal.FreeHGlobal(pointer);
            }
        }

        #endregion

        #region Private Methods

        // This method will set default values for when automatic detection fails.
        private static void InitializeNoDetectDisplays()
        {
            // Clear the displayInfo struct to populate with different data
            displayInfo = default(DisplayInfo);

            int minX = 0;
            int minY = 0;
            int maxX = 0;
            int maxY = 0;

            foreach (Screen screen in Screen.AllScreens)
            {
                var bounds = screen.Bounds;

                minX = Math.Min(minX, bounds.X);
                minY = Math.Min(minY, bounds.Y);
                maxX = Math.Max(maxX, bounds.Right);
                maxY = Math.Max(maxY, bounds.Bottom);
            }

            //// this.displayInfo.DisplayCount = Screen.AllScreens.Length;
            displayInfo.DisplayCount = SystemInformation.MonitorCount;

            // Force these settings until I can use the bounds above to determine just how many columns and rows there are.
            // Almost always people will be using a straight horizontal setup.
            displayInfo.Brand = BrandType.Unknown;
            displayInfo.ColumnCount = SystemInformation.MonitorCount;
            displayInfo.RowCount = 1;
            displayInfo.HorizontalResolution = maxX - minX;
            displayInfo.VerticalResolution = maxY - minY;
            displayInfo.AspectRatio = (float)(maxX - minX) / (maxY - minY);
            displayInfo.HudRight = maxX;
            displayInfo.HudBottom = maxY;
            displayInfo.HudWidth = maxX - minX;
            displayInfo.HudHeight = maxY - minY;

            if (displayInfo.ColumnCount > 1)
            {
                if (Screen.PrimaryScreen.Bounds.Width > Screen.PrimaryScreen.Bounds.Height)
                {
                    displayInfo.Landscape = 1;
                }
                else
                {
                    displayInfo.Portrait = 1;
                }
            }
            else
            {
                if (displayInfo.HorizontalResolution > displayInfo.VerticalResolution)
                {
                    displayInfo.Landscape = 1;
                }
                else
                {
                    displayInfo.Portrait = 1;
                }
            }
        }

        #endregion

        #region Private Structs

        // This is the struct for display information.
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct DisplayInfo
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = DescriptionStringMax)]
            private string videoCard;
            private BrandType brand;
            private int error;
            private int errorCode;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ErrorStringMax)]
            private string errorString;
            private int multipleMonitorsEnabled;
            private int displayCount;
            private int columnCount;
            private int rowCount;
            private int bezelCorrected;
            private int horizontalResolution;
            private int verticalResolution;
            private float aspectRatio;
            private int landscape;
            private int portrait;
            private int hudLeft;
            private int hudRight;
            private int hudTop;
            private int hudBottom;
            private int hudWidth;
            private int hudHeight;

            // This is used to set the size of VideoCard.
            public const int DescriptionStringMax = 512;

            // This is used to set the size of ErrorString.
            public const int ErrorStringMax = 64;

            // The name of the video card.
            public string VideoCard
            {
                get { return this.videoCard; }
            }

            // This will contain the brand of the video card in use.
            public BrandType Brand
            {
                get { return this.brand; }
                set { this.brand = value; }
            }

            // This will be 0 if successful and 1 if not.
            public int Error
            {
                get { return this.error; }
            }

            // This will be 0 if successful and less than 0 if not.
            public int ErrorCode
            {
                get { return this.errorCode; }
            }

            // If an error was thrown this will contain a textual representation of the error.
            public string ErrorString
            {
                get { return this.errorString; }
            }

            // If multiple monitors are in use this will be true.
            public int MultipleMonitorEnabled
            {
                get { return this.multipleMonitorsEnabled; }
            }

            // How many displays are in use.
            public int DisplayCount
            {
                get { return this.displayCount; }
                set { this.displayCount = value; }
            }

            // How many columns in the display setup there are.
            public int ColumnCount
            {
                get { return this.columnCount; }
                set { this.columnCount = value; }
            }

            // How many rows in the display setup there are.
            public int RowCount
            {
                get { return this.rowCount; }
                set { this.rowCount = value; }
            }

            // If bezel correction is in use this will be true.
            public int BezelCorrected
            {
                get { return this.bezelCorrected; }
            }

            // Combined horizontal resolution.
            public int HorizontalResolution
            {
                get { return this.horizontalResolution; }
                set { this.horizontalResolution = value; }
            }

            // Combined vertical resolution.
            public int VerticalResolution
            {
                get { return this.verticalResolution; }
                set { this.verticalResolution = value; }
            }

            // The aspect-ratio of the entire resolution.
            public float AspectRatio
            {
                get { return this.aspectRatio; }
                set { this.aspectRatio = value; }
            }

            // This will be equal to 1 if the display configuration landscape.
            public int Landscape
            {
                get { return this.landscape; }
                set { this.landscape = value; }
            }

            // This will be equal to 1 if the display configuration portrait.
            public int Portrait
            {
                get { return this.portrait; }
                set { this.portrait = value; }
            }

            // The position in pixels where the left side of a HUD should be placed.
            public int HudLeft
            {
                get { return this.hudLeft; }
            }

            // The position in pixels where the right side of a HUD should be placed.
            public int HudRight
            {
                get { return this.hudRight; }
                set { this.hudRight = value; }
            }

            // The position in pixels where the top of a HUD should be placed.
            public int HudTop
            {
                get { return this.hudTop; }
            }

            // The position in pixels where the bottom of a HUD should be placed.
            public int HudBottom
            {
                get { return this.hudBottom; }
                set { this.hudBottom = value; }
            }

            // How wide a HUD should be in pixels.
            public int HudWidth
            {
                get { return this.hudWidth; }
                set { this.hudWidth = value; }
            }

            // How tall a HUD should be in pixels.
            public int HudHeight
            {
                get { return this.hudWidth; }
                set { this.hudHeight = value; }
            }
        }

        #endregion

        #region Private Classes

        // This class wraps the Initialize() function.
        private static class DD
        {
            // This will determine whether this library is being run as 32-bit or 64-bit and call the appropriate unmanaged DLL function.
            public static int Initialize(IntPtr displayInfo)
            {
                if (Environment.Is64BitProcess)
                {
                    return UnsafeNativeMethods.Initialize64(displayInfo);
                }
                else
                {
                    return UnsafeNativeMethods.Initialize32(displayInfo);
                }
            }
        }

        // Class for importing the 32-bit Initialize() function for NVIDIA users.
        private static class UnsafeNativeMethods
        {
            // The name of the 32-bit unmanaged library to be imported.
            internal const string UnmanagedLibrary32 = "Unmanaged.DetectDisplays32.dll";

            [DllImport(UnmanagedLibrary32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Initialize")]
            internal static extern int Initialize32(IntPtr displayInfo);

            // The name of the 64-bit unmanaged library to be imported.
            internal const string UnmanagedLibrary64 = "Unmanaged.DetectDisplays64.dll";

            [DllImport(UnmanagedLibrary64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Initialize")]
            internal static extern int Initialize64(IntPtr displayInfo);
        }

        #endregion
    }
}
