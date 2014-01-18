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
    using System.Collections;
    using System.ComponentModel;
    using System.Drawing.Design;
    using System.Runtime.CompilerServices;
    using System.Windows.Forms;
    using System.Windows.Forms.Design;

    public class TrackBarDrawModeEditor : UITypeEditor
    {
        #region Methods

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            TrackBarOwnerDrawParts parts = TrackBarOwnerDrawParts.None;

            if (!(value is TrackBarOwnerDrawParts) || (provider == null))
            {
                return value;
            }

            IWindowsFormsEditorService service = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

            if (service == null)
            {
                return value;
            }

            if (context != null)
            {
                using (CheckedListBox control = new CheckedListBox())
                {
                    control.BorderStyle = BorderStyle.None;
                    control.CheckOnClick = true;
                    control.Items.Add("Ticks", (((TrackBarTransparent)context.Instance).OwnerDrawParts & TrackBarOwnerDrawParts.Ticks) == TrackBarOwnerDrawParts.Ticks);
                    control.Items.Add("Thumb", (((TrackBarTransparent)context.Instance).OwnerDrawParts & TrackBarOwnerDrawParts.Thumb) == TrackBarOwnerDrawParts.Thumb);
                    control.Items.Add("Channel", (((TrackBarTransparent)context.Instance).OwnerDrawParts & TrackBarOwnerDrawParts.Channel) == TrackBarOwnerDrawParts.Channel);
                    service.DropDownControl(control);

                    IEnumerator enumerator = control.CheckedItems.GetEnumerator();

                    while (enumerator.MoveNext())
                    {
                        object objectValue = RuntimeHelpers.GetObjectValue(enumerator.Current);
                        parts |= (TrackBarOwnerDrawParts)Enum.Parse(typeof(TrackBarOwnerDrawParts), objectValue.ToString());
                    }

                    service.CloseDropDown();
                }

                return parts;
            }

            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        #endregion
    }
}
