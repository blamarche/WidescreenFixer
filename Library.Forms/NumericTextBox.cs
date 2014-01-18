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
    using System.Globalization;
    using System.Text;
    using System.Windows.Forms;

    public class NumericTextBox : TextBox
    {
        private bool allowSpace = false;

        public bool AllowSpace
        {
            get { return this.allowSpace; }
            set { this.allowSpace = value; }
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (e != null)
            {
                base.OnKeyPress(e);

                string keyInput = e.KeyChar.ToString();

                if (char.IsDigit(e.KeyChar))
                {
                }
                else if (e.KeyChar == '.')
                {
                }
                else if (e.KeyChar == '\b')
                {
                }
                else if (this.allowSpace && e.KeyChar == ' ')
                {
                }
                else
                {
                    e.Handled = true;
                }
            }
        }
    }
}
