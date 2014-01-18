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
    using System.Text;

    public class TrackBarDrawItemEventArgs : EventArgs
    {
        #region Fields

        private Rectangle bounds;
        private Graphics graphics;
        private TrackBarItemState state;

        #endregion

        #region Methods

        public TrackBarDrawItemEventArgs(Graphics graphics, Rectangle bounds, TrackBarItemState state)
        {
            this.graphics = graphics;
            this.bounds = bounds;
            this.state = state;
        }

        #endregion

        #region Properties

        public Rectangle Bounds
        {
            get
            {
                return this.bounds;
            }
        }

        public Graphics Graphics
        {
            get
            {
                return this.graphics;
            }
        }

        public TrackBarItemState State
        {
            get
            {
                return this.state;
            }
        }

        #endregion
    }
}
