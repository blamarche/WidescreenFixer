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
    using System.Windows.Forms;

    public class ButtonBorderless : Button
    {
        #region Properties

        protected override bool ShowFocusCues
        {
            get
            {
                return false;
            }
        }

        #endregion

        #region Methods

        public override void NotifyDefault(bool value)
        {
            base.NotifyDefault(false);
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            if (pevent != null)
            {
                base.OnPaint(pevent);

                using (Pen backColor = new Pen(this.BackColor, 5))
                {
                    pevent.Graphics.DrawRectangle(backColor, this.ClientRectangle);
                }
            }
        }

        #endregion
    }
}
