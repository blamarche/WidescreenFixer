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
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Design;
    using System.Drawing.Drawing2D;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
    using System.Windows.Forms.VisualStyles;

    public class TrackBarTransparent : TrackBar
    {
        #region Fields

        private Rectangle channelBounds;
        private TrackBarOwnerDrawParts trackBarOwnerDrawParts;
        private Rectangle thumbBounds;
        private int thumbState;

        #endregion

        #region Constructors

        public TrackBarTransparent()
        {
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        }

        #endregion

        #region Public Events

        public event EventHandler<TrackBarDrawItemEventArgs> DrawChannel;

        public event EventHandler<TrackBarDrawItemEventArgs> DrawThumb;

        public event EventHandler<TrackBarDrawItemEventArgs> DrawTicks;

        #endregion

        #region Properties

        [DefaultValue(typeof(TrackBarOwnerDrawParts), "None")]
        [Description("Gets/sets the trackbar parts that will be OwnerDrawn.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Editor(typeof(TrackBarDrawModeEditor), typeof(UITypeEditor))]
        public TrackBarOwnerDrawParts OwnerDrawParts
        {
            get
            {
                return this.trackBarOwnerDrawParts;
            }

            set
            {
                this.trackBarOwnerDrawParts = value;
            }
        }

        #endregion

        #region Overrides

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 20)
            {
                m.Result = IntPtr.Zero;
            }
            else
            {
                base.WndProc(ref m);

                if (m.Msg == 0x204e)
                {
                    TrackBarNativeMethods.NMHDR structure = (TrackBarNativeMethods.NMHDR)Marshal.PtrToStructure(m.LParam, typeof(TrackBarNativeMethods.NMHDR));

                    if (structure.code == -12)
                    {
                        IntPtr ptr;
                        Marshal.StructureToPtr(structure, m.LParam, false);
                        TrackBarNativeMethods.NMCUSTOMDRAW nmcustomdraw = (TrackBarNativeMethods.NMCUSTOMDRAW)Marshal.PtrToStructure(m.LParam, typeof(TrackBarNativeMethods.NMCUSTOMDRAW));

                        if (nmcustomdraw.dwDrawStage == TrackBarNativeMethods.CustomDrawDrawStage.CDDS_PREPAINT)
                        {
                            Graphics graphics = Graphics.FromHdc(nmcustomdraw.hdc);
                            using (PaintEventArgs e = new PaintEventArgs(graphics, this.Bounds))
                            {
                                e.Graphics.TranslateTransform((float)(0 - this.Left), (float)(0 - this.Top));
                                this.InvokePaintBackground(this.Parent, e);
                                this.InvokePaint(this.Parent, e);
                                using (SolidBrush brush = new SolidBrush(this.BackColor))
                                {
                                    e.Graphics.FillRectangle(brush, this.Bounds);
                                }
                                e.Graphics.ResetTransform();
                                graphics.Dispose();
                                ptr = new IntPtr(0x30);
                                m.Result = ptr;
                            }
                        }
                        else if (nmcustomdraw.dwDrawStage == TrackBarNativeMethods.CustomDrawDrawStage.CDDS_POSTPAINT)
                        {
                            this.OnDrawTicks(nmcustomdraw.hdc);
                            this.OnDrawChannel(nmcustomdraw.hdc);
                            this.OnDrawThumb(nmcustomdraw.hdc);
                        }
                        else if (nmcustomdraw.dwDrawStage == TrackBarNativeMethods.CustomDrawDrawStage.CDDS_ITEMPREPAINT)
                        {
                            if (nmcustomdraw.dwItemSpec.ToInt32() == 2)
                            {
                                this.thumbBounds = nmcustomdraw.rc.ToRectangle();
                                if (this.Enabled)
                                {
                                    if (nmcustomdraw.uItemState == TrackBarNativeMethods.CustomDrawItemState.CDIS_SELECTED)
                                    {
                                        this.thumbState = 3;
                                    }
                                    else
                                    {
                                        this.thumbState = 1;
                                    }
                                }
                                else
                                {
                                    this.thumbState = 5;
                                }

                                this.OnDrawThumb(nmcustomdraw.hdc);
                            }
                            else if (nmcustomdraw.dwItemSpec.ToInt32() == 3)
                            {
                                this.channelBounds = nmcustomdraw.rc.ToRectangle();
                                this.OnDrawChannel(nmcustomdraw.hdc);
                            }
                            else if (nmcustomdraw.dwItemSpec.ToInt32() == 1)
                            {
                                this.OnDrawTicks(nmcustomdraw.hdc);
                            }

                            ptr = new IntPtr(4);
                            m.Result = ptr;
                        }
                    }
                }
            }
        }

        #endregion

        #region Event Invoker Methods

        protected virtual void OnDrawTicks(IntPtr hdc)
        {
            Graphics graphics = Graphics.FromHdc(hdc);

            if (((this.OwnerDrawParts & TrackBarOwnerDrawParts.Ticks) == TrackBarOwnerDrawParts.Ticks) && !this.DesignMode)
            {
                TrackBarDrawItemEventArgs e = new TrackBarDrawItemEventArgs(graphics, this.ClientRectangle, (TrackBarItemState)this.thumbState);

                if (this.DrawTicks != null)
                {
                    this.DrawTicks(this, e);
                }
            }
            else
            {
                if (this.TickStyle == TickStyle.None)
                {
                    return;
                }

                if (this.thumbBounds == Rectangle.Empty)
                {
                    return;
                }

                Color black = Color.Black;

                if (VisualStyleRenderer.IsSupported)
                {
                    VisualStyleRenderer vsr = new VisualStyleRenderer("TRACKBAR", (int)TrackBarNativeMethods.TrackBarParts.TKP_TICS, this.thumbState);
                    black = vsr.GetColor(ColorProperty.TextColor);
                }

                if (this.Orientation == Orientation.Horizontal)
                {
                    this.DrawHorizontalTicks(graphics, black);
                }
                else
                {
                    this.DrawVerticalTicks(graphics, black);
                }
            }

            graphics.Dispose();
        }

        protected virtual void OnDrawThumb(IntPtr hdc)
        {
            Graphics graphics = Graphics.FromHdc(hdc);
            graphics.Clip = new Region(this.thumbBounds);

            if (((this.OwnerDrawParts & TrackBarOwnerDrawParts.Thumb) == TrackBarOwnerDrawParts.Thumb) && !this.DesignMode)
            {
                TrackBarDrawItemEventArgs e = new TrackBarDrawItemEventArgs(graphics, this.thumbBounds, (TrackBarItemState)this.thumbState);
                if (this.DrawThumb != null)
                {
                    this.DrawThumb(this, e);
                }
            }
            else
            {
                // Determine the style of the thumb, based on the tickstyle
                TrackBarNativeMethods.TrackBarParts part = TrackBarNativeMethods.TrackBarParts.TKP_THUMB;
                if (this.thumbBounds == Rectangle.Empty)
                {
                    return;
                }

                switch (this.TickStyle)
                {
                    case TickStyle.None:
                    case TickStyle.BottomRight:
                        part = (this.Orientation != Orientation.Horizontal) ? TrackBarNativeMethods.TrackBarParts.TKP_THUMBRIGHT : TrackBarNativeMethods.TrackBarParts.TKP_THUMBBOTTOM;
                        break;
                    case TickStyle.TopLeft:
                        part = (this.Orientation != Orientation.Horizontal) ? TrackBarNativeMethods.TrackBarParts.TKP_THUMBLEFT : TrackBarNativeMethods.TrackBarParts.TKP_THUMBTOP;
                        break;

                    case TickStyle.Both:
                        part = (this.Orientation != Orientation.Horizontal) ? TrackBarNativeMethods.TrackBarParts.TKP_THUMBVERT : TrackBarNativeMethods.TrackBarParts.TKP_THUMB;
                        break;
                }

                // Perform drawing
                if (VisualStyleRenderer.IsSupported)
                {
                    VisualStyleRenderer vsr = new VisualStyleRenderer("TRACKBAR", (int)part, this.thumbState);
                    vsr.DrawBackground(graphics, this.thumbBounds);
                    graphics.ResetClip();
                    graphics.Dispose();
                    return;
                }
                else
                {
                    switch (part)
                    {
                        case TrackBarNativeMethods.TrackBarParts.TKP_THUMBBOTTOM:
                            this.DrawPointerDown(graphics);
                            break;

                        case TrackBarNativeMethods.TrackBarParts.TKP_THUMBTOP:
                            this.DrawPointerUp(graphics);

                            break;

                        case TrackBarNativeMethods.TrackBarParts.TKP_THUMBLEFT:
                            this.DrawPointerLeft(graphics);

                            break;

                        case TrackBarNativeMethods.TrackBarParts.TKP_THUMBRIGHT:
                            this.DrawPointerRight(graphics);

                            break;

                        default:
                            if ((this.thumbState == 3) || !this.Enabled)
                            {
                                ControlPaint.DrawButton(graphics, this.thumbBounds, ButtonState.All);
                            }
                            else
                            {
                                // Tick-style is both - draw the thumb as a solid rectangle
                                graphics.FillRectangle(SystemBrushes.Control, this.thumbBounds);
                            }

                            ControlPaint.DrawBorder3D(graphics, this.thumbBounds, Border3DStyle.Raised);

                            break;
                    }
                }
            }

            graphics.ResetClip();
            graphics.Dispose();
        }

        protected virtual void OnDrawChannel(IntPtr hdc)
        {
            Graphics graphics = Graphics.FromHdc(hdc);

            if (((this.OwnerDrawParts & TrackBarOwnerDrawParts.Channel) == TrackBarOwnerDrawParts.Channel) && !this.DesignMode)
            {
                TrackBarDrawItemEventArgs e = new TrackBarDrawItemEventArgs(graphics, this.channelBounds, (TrackBarItemState)this.thumbState);

                if (this.DrawChannel != null)
                {
                    this.DrawChannel(this, e);
                }
            }
            else
            {
                if (this.channelBounds == Rectangle.Empty)
                {
                    return;
                }

                if (VisualStyleRenderer.IsSupported)
                {
                    VisualStyleRenderer vsr = new VisualStyleRenderer("TRACKBAR", (int)TrackBarNativeMethods.TrackBarParts.TKP_TRACK, (int)TrackBarItemState.Normal);
                    vsr.DrawBackground(graphics, this.channelBounds);
                    graphics.ResetClip();
                    graphics.Dispose();

                    return;
                }

                ControlPaint.DrawBorder3D(graphics, this.channelBounds, Border3DStyle.Sunken);
            }

            graphics.Dispose();
        }

        #endregion

        #region Private Drawing Methods

        private void DrawHorizontalTicks(Graphics g, Color color)
        {
            RectangleF innerTickRect;
            int numofTicks = (this.Maximum / this.TickFrequency) - 1;
            using (Pen tickPen = new Pen(color))
            {
                RectangleF endTickRRect = new RectangleF((float)(this.channelBounds.Left + (this.thumbBounds.Width / 2)), (float)(this.thumbBounds.Top - 5), 0f, 3f);
                RectangleF endTickLRect = new RectangleF((float)((this.channelBounds.Right - (this.thumbBounds.Width / 2)) - 1), (float)(this.thumbBounds.Top - 5), 0f, 3f);
                float tickPitch = (endTickLRect.Right - endTickRRect.Left) / ((float)(numofTicks + 1));

                // Draw upper (top) ticks
                if (this.TickStyle != TickStyle.BottomRight)
                {
                    // Draw right outer tick
                    g.DrawLine(tickPen, endTickRRect.Left, endTickRRect.Top, endTickRRect.Right, endTickRRect.Bottom);

                    // Draw left outer tick
                    g.DrawLine(tickPen, endTickLRect.Left, endTickLRect.Top, endTickLRect.Right, endTickLRect.Bottom);

                    // Draw inner ticks
                    innerTickRect = endTickRRect;
                    innerTickRect.Height--;
                    innerTickRect.Offset(tickPitch, 1f);

                    int numOfInnerTicks = numofTicks - 1;

                    for (int i = 0; i <= numOfInnerTicks; i++)
                    {
                        g.DrawLine(tickPen, innerTickRect.Left, innerTickRect.Top, innerTickRect.Left, innerTickRect.Bottom);
                        innerTickRect.Offset(tickPitch, 0f);
                    }
                }

                endTickRRect.Offset(0f, (float)(this.thumbBounds.Height + 6));
                endTickLRect.Offset(0f, (float)(this.thumbBounds.Height + 6));

                // Draw lower (bottom) ticks
                if (this.TickStyle != TickStyle.TopLeft)
                {
                    // Draw right outer tick
                    g.DrawLine(tickPen, endTickRRect.Left, endTickRRect.Top, endTickRRect.Left, endTickRRect.Bottom);

                    // Draw left outer tick
                    g.DrawLine(tickPen, endTickLRect.Left, endTickLRect.Top, endTickLRect.Left, endTickLRect.Bottom);

                    // Draw innerticks
                    innerTickRect = endTickRRect;
                    innerTickRect.Height--;
                    innerTickRect.Offset(tickPitch, 0f);

                    int numOfInnerTicks = numofTicks - 1;

                    for (int j = 0; j <= numOfInnerTicks; j++)
                    {
                        g.DrawLine(tickPen, innerTickRect.Left, innerTickRect.Top, innerTickRect.Left, innerTickRect.Bottom);
                        innerTickRect.Offset(tickPitch, 0f);
                    }
                }
            }
        }

        private void DrawVerticalTicks(Graphics g, Color color)
        {
            RectangleF innerTickRect;
            int numOfTicks = (this.Maximum / this.TickFrequency) - 1;
            using (Pen tickPen = new Pen(color))
            {
                RectangleF endTickBottomRect = new RectangleF((float)(this.thumbBounds.Left - 5), (float)((this.channelBounds.Bottom - (this.thumbBounds.Height / 2)) - 1), 3f, 0f);
                RectangleF endTickTopRect = new RectangleF((float)(this.thumbBounds.Left - 5), (float)(this.channelBounds.Top + (this.thumbBounds.Height / 2)), 3f, 0f);
                float y = (endTickTopRect.Bottom - endTickBottomRect.Top) / ((float)(numOfTicks + 1));

                // Draw left-hand ticks
                if (this.TickStyle != TickStyle.BottomRight)
                {
                    // Draw lower (bottom) outer tick
                    g.DrawLine(tickPen, endTickBottomRect.Left, endTickBottomRect.Top, endTickBottomRect.Right, endTickBottomRect.Bottom);

                    // Draw upper (top) outer tick
                    g.DrawLine(tickPen, endTickTopRect.Left, endTickTopRect.Top, endTickTopRect.Right, endTickTopRect.Bottom);

                    // Draw inner ticks
                    innerTickRect = endTickBottomRect;
                    innerTickRect.Width--;
                    innerTickRect.Offset(1f, y);
                    int numOfInnerTicks = numOfTicks - 1;

                    for (int i = 0; i <= numOfInnerTicks; i++)
                    {
                        g.DrawLine(tickPen, innerTickRect.Left, innerTickRect.Top, innerTickRect.Right, innerTickRect.Bottom);
                        innerTickRect.Offset(0f, y);
                    }
                }

                endTickBottomRect.Offset((float)(this.thumbBounds.Width + 6), 0f);
                endTickTopRect.Offset((float)(this.thumbBounds.Width + 6), 0f);

                // Draw right-hand ticks
                if (this.TickStyle != TickStyle.TopLeft)
                {
                    // Draw lower (bottom) tick
                    g.DrawLine(tickPen, endTickBottomRect.Left, endTickBottomRect.Top, endTickBottomRect.Right, endTickBottomRect.Bottom);

                    // Draw upper (top) tick
                    g.DrawLine(tickPen, endTickTopRect.Left, endTickTopRect.Top, endTickTopRect.Right, endTickTopRect.Bottom);

                    // Draw inner ticks
                    innerTickRect = endTickBottomRect;
                    innerTickRect.Width--;
                    innerTickRect.Offset(0f, y);
                    int numOfInnerTicks = numOfTicks - 1;

                    for (int j = 0; j <= numOfInnerTicks; j++)
                    {
                        g.DrawLine(tickPen, innerTickRect.Left, innerTickRect.Top, innerTickRect.Right, innerTickRect.Bottom);
                        innerTickRect.Offset(0f, y);
                    }
                }
            }
        }

        private void DrawPointerDown(Graphics g)
        {
            Point[] points = new Point[6]
            {
                new Point(this.thumbBounds.Left + (this.thumbBounds.Width / 2), this.thumbBounds.Bottom - 1),
                new Point(this.thumbBounds.Left, (this.thumbBounds.Bottom - (this.thumbBounds.Width / 2)) - 1),
                this.thumbBounds.Location,
                new Point(this.thumbBounds.Right - 1, this.thumbBounds.Top),
                new Point(this.thumbBounds.Right - 1, (this.thumbBounds.Bottom - (this.thumbBounds.Width / 2)) - 1),
                new Point(this.thumbBounds.Left + (this.thumbBounds.Width / 2), this.thumbBounds.Bottom - 1)
            };

            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddLines(points);
                Region region = new Region(path);
                g.Clip = region;

                if ((this.thumbState == 3) || !this.Enabled)
                {
                    ControlPaint.DrawButton(g, this.thumbBounds, ButtonState.All);
                }
                else
                {
                    g.Clear(SystemColors.Control);
                }

                g.ResetClip();
                region.Dispose();
            }

            // Draw light shadow
            Point[] shadowPoints = new Point[] { points[0], points[1], points[2], points[3] };
            g.DrawLines(SystemPens.ControlLightLight, shadowPoints);

            // Draw dark shadow
            shadowPoints = new Point[] { points[3], points[4], points[5] };
            g.DrawLines(SystemPens.ControlDarkDark, shadowPoints);
            points[0].Offset(0, -1);
            points[1].Offset(1, 0);
            points[2].Offset(1, 1);
            points[3].Offset(-1, 1);
            points[4].Offset(-1, 0);
            points[5] = points[0];
            shadowPoints = new Point[] { points[0], points[1], points[2], points[3] };
            g.DrawLines(SystemPens.ControlLight, shadowPoints);
            shadowPoints = new Point[] { points[3], points[4], points[5] };
            g.DrawLines(SystemPens.ControlDark, shadowPoints);
        }

        private void DrawPointerLeft(Graphics g)
        {
            Point[] points = new Point[6]
            {            
                new Point(this.thumbBounds.Left, this.thumbBounds.Top + (this.thumbBounds.Height / 2)),
                new Point(this.thumbBounds.Left + (this.thumbBounds.Height / 2), this.thumbBounds.Top),
                new Point(this.thumbBounds.Right - 1, this.thumbBounds.Top),
                new Point(this.thumbBounds.Right - 1, this.thumbBounds.Bottom - 1),
                new Point(this.thumbBounds.Left + (this.thumbBounds.Height / 2), this.thumbBounds.Bottom - 1),
                new Point(this.thumbBounds.Left, this.thumbBounds.Top + (this.thumbBounds.Height / 2)),
            };

            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddLines(points);
                Region region = new Region(path);
                g.Clip = region;

                if ((this.thumbState == 3) || !this.Enabled)
                {
                    ControlPaint.DrawButton(g, this.thumbBounds, ButtonState.All);
                }
                else
                {
                    g.Clear(SystemColors.Control);
                }

                g.ResetClip();
                region.Dispose();
            }

            // Draw light shadow
            Point[] shadowPoints = new Point[] { points[0], points[1], points[2] };
            g.DrawLines(SystemPens.ControlLightLight, shadowPoints);

            // Draw dark shadow
            shadowPoints = new Point[] { points[2], points[3], points[4], points[5] };
            g.DrawLines(SystemPens.ControlDarkDark, shadowPoints);
            points[0].Offset(1, 0);
            points[1].Offset(0, 1);
            points[2].Offset(-1, 1);
            points[3].Offset(-1, -1);
            points[4].Offset(0, -1);
            points[5] = points[0];
            shadowPoints = new Point[] { points[0], points[1], points[2] };
            g.DrawLines(SystemPens.ControlLight, shadowPoints);
            shadowPoints = new Point[] { points[2], points[3], points[4], points[5] };
            g.DrawLines(SystemPens.ControlDark, shadowPoints);
        }

        private void DrawPointerRight(Graphics g)
        {
            Point[] points = new Point[6]
            {
                new Point(this.thumbBounds.Left, this.thumbBounds.Bottom - 1),
                new Point(this.thumbBounds.Left, this.thumbBounds.Top),
                new Point((this.thumbBounds.Right - (this.thumbBounds.Height / 2)) - 1, this.thumbBounds.Top),
                new Point(this.thumbBounds.Right - 1, this.thumbBounds.Top + (this.thumbBounds.Height / 2)),
                new Point((this.thumbBounds.Right - (this.thumbBounds.Height / 2)) - 1, this.thumbBounds.Bottom - 1),
                new Point(this.thumbBounds.Left, this.thumbBounds.Bottom - 1)
            };

            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddLines(points);
                Region region = new Region(path);
                g.Clip = region;

                if ((this.thumbState == 3) || !this.Enabled)
                {
                    ControlPaint.DrawButton(g, this.thumbBounds, ButtonState.All);
                }
                else
                {
                    g.Clear(SystemColors.Control);
                }

                g.ResetClip();
                region.Dispose();
            }

            // Draw light shadow
            Point[] shadowPoints = new Point[] { points[0], points[1], points[2], points[3] };
            g.DrawLines(SystemPens.ControlLightLight, shadowPoints);

            // Draw dark shadow
            shadowPoints = new Point[] { points[3], points[4], points[5] };
            g.DrawLines(SystemPens.ControlDarkDark, shadowPoints);
            points[0].Offset(1, -1);
            points[1].Offset(1, 1);
            points[2].Offset(0, 1);
            points[3].Offset(-1, 0);
            points[4].Offset(0, -1);
            points[5] = points[0];
            shadowPoints = new Point[] { points[0], points[1], points[2], points[3] };
            g.DrawLines(SystemPens.ControlLight, shadowPoints);
            shadowPoints = new Point[] { points[3], points[4], points[5] };
            g.DrawLines(SystemPens.ControlDark, shadowPoints);
        }

        private void DrawPointerUp(Graphics g)
        {
            Point[] points = new Point[6]
            {
                new Point(this.thumbBounds.Left, this.thumbBounds.Bottom - 1),
                new Point(this.thumbBounds.Left, this.thumbBounds.Top + (this.thumbBounds.Width / 2)),
                new Point(this.thumbBounds.Left + (this.thumbBounds.Width / 2), this.thumbBounds.Top),
                new Point(this.thumbBounds.Right - 1, this.thumbBounds.Top + (this.thumbBounds.Width / 2)),
                new Point(this.thumbBounds.Right - 1, this.thumbBounds.Bottom - 1),
                new Point(this.thumbBounds.Left, this.thumbBounds.Bottom - 1)
            };

            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddLines(points);
                Region region = new Region(path);
                g.Clip = region;

                if ((this.thumbState == 3) || !this.Enabled)
                {
                    ControlPaint.DrawButton(g, this.thumbBounds, ButtonState.All);
                }
                else
                {
                    g.Clear(SystemColors.Control);
                }

                g.ResetClip();
                region.Dispose();
            }

            // Draw light shadow
            Point[] shadowPoints = new Point[] { points[0], points[1], points[2] };
            g.DrawLines(SystemPens.ControlLightLight, shadowPoints);

            // Draw dark shadow
            shadowPoints = new Point[] { points[2], points[3], points[4], points[5] };
            g.DrawLines(SystemPens.ControlDarkDark, shadowPoints);
            points[0].Offset(1, -1);
            points[1].Offset(1, 0);
            points[2].Offset(0, 1);
            points[3].Offset(-1, 0);
            points[4].Offset(-1, -1);
            points[5] = points[0];
            shadowPoints = new Point[] { points[0], points[1], points[2] };
            g.DrawLines(SystemPens.ControlLight, shadowPoints);
            shadowPoints = new Point[] { points[2], points[3], points[4], points[5] };
            g.DrawLines(SystemPens.ControlDark, shadowPoints);
        }

        #endregion
    }
}
