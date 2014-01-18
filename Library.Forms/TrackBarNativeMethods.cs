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

namespace Library.Forms
{
    using System;
    using System.Drawing;
    using System.Globalization;
    using System.Runtime.InteropServices;

    internal class TrackBarNativeMethods
    {
        #region Constants

        public const int NM_CUSTOMDRAW = -12;
        public const int NM_FIRST = 0;
        public const int S_OK = 0;
        public const int TMT_COLOR = 0xcc;

        #endregion

        #region Constructors

        private TrackBarNativeMethods()
        {
        }

        #endregion

        #region Enumerations

        public enum CustomDrawDrawStage
        {
            CDDS_ITEM = 0x10000,
            CDDS_ITEMPOSTERASE = 0x10004,
            CDDS_ITEMPOSTPAINT = 0x10002,
            CDDS_ITEMPREERASE = 0x10003,
            CDDS_ITEMPREPAINT = 0x10001,
            CDDS_POSTERASE = 4,
            CDDS_POSTPAINT = 2,
            CDDS_PREERASE = 3,
            CDDS_PREPAINT = 1,
            CDDS_SUBITEM = 0x20000
        }

        public enum CustomDrawItemState
        {
            CDIS_CHECKED = 8,
            CDIS_DEFAULT = 0x20,
            CDIS_DISABLED = 4,
            CDIS_FOCUS = 0x10,
            CDIS_GRAYED = 2,
            CDIS_HOT = 0x40,
            CDIS_INDETERMINATE = 0x100,
            CDIS_MARKED = 0x80,
            CDIS_SELECTED = 1,
            CDIS_SHOWKEYBOARDCUES = 0x200
        }

        public enum CustomDrawReturnFlags
        {
            CDRF_DODEFAULT = 0,
            CDRF_NEWFONT = 2,
            CDRF_NOTIFYITEMDRAW = 0x20,
            CDRF_NOTIFYPOSTERASE = 0x40,
            CDRF_NOTIFYPOSTPAINT = 0x10,
            CDRF_NOTIFYSUBITEMDRAW = 0x20,
            CDRF_SKIPDEFAULT = 4
        }

        public enum TrackBarCustomDrawPart
        {
            TBCD_CHANNEL = 3,
            TBCD_THUMB = 2,
            TBCD_TICS = 1
        }

        public enum TrackBarParts
        {
            TKP_THUMB = 3,
            TKP_THUMBBOTTOM = 4,
            TKP_THUMBLEFT = 7,
            TKP_THUMBRIGHT = 8,
            TKP_THUMBTOP = 5,
            TKP_THUMBVERT = 6,
            TKP_TICS = 9,
            TKP_TICSVERT = 10,
            TKP_TRACK = 1,
            TKP_TRACKVERT = 2
        }

        #endregion

        #region Structures

        [StructLayout(LayoutKind.Sequential)]
        public struct DLLVERSIONINFO
        {
            public int cbSize;
            public int dwMajorVersion;
            public int dwMinorVersion;
            public int dwBuildNumber;
            public int dwPlatformID;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NMCUSTOMDRAW
        {
            public TrackBarNativeMethods.NMHDR hdr;
            public TrackBarNativeMethods.CustomDrawDrawStage dwDrawStage;
            public IntPtr hdc;
            public TrackBarNativeMethods.RECT rc;
            public IntPtr dwItemSpec;
            public TrackBarNativeMethods.CustomDrawItemState uItemState;
            public IntPtr lItemlParam;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NMHDR
        {
            public IntPtr HWND;
            public int idFrom;
            public int code;

            public override string ToString()
            {
                object[] id = new object[] { this.HWND, this.idFrom, this.code };

                return string.Format(CultureInfo.InvariantCulture, "Hwnd: {0}, ControlID: {1}, Code: {2}", id);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            public override string ToString()
            {
                object[] coordinates = new object[] { this.Left, this.Top, this.Right, this.Bottom };

                return string.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}, {3}", coordinates);
            }

            public Rectangle ToRectangle()
            {
                return Rectangle.FromLTRB(this.Left, this.Top, this.Right, this.Bottom);
            }
        }

        #endregion
    }
}
