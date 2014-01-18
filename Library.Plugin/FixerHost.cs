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

namespace Library.Plugin
{
    using System;
    using System.Drawing;

    // This is the plugin host class that Widescreen Fixer will access the plugins through.
    public class FixerHost
    {
        // An instance of the IFixer class.
        private IFixer fix;

        // Initializes a new instance of the FixerHost class.
        public FixerHost(IFixer fix)
        {
            this.fix = fix;
        }

        // Gets the name of the game being fixed.
        public string GameName
        {
            get { return this.fix.GameName; }
        }

        // Gets the game's icon to be used in the combo-box and the configuration form title.
        public Bitmap GameIcon
        {
            get { return this.fix.GameIcon; }
        }

        // Gets a value indicating whether the plugin is obsolete or not.
        public bool PluginIsObsolete
        {
            get { return this.fix.PluginIsObsolete; }
        }

        // Gets a value indicating whether the fix is enabled or not.
        public bool Enabled
        {
            get { return this.fix.Enabled; }
        }

        // Gets a value indicating whether the fix should be enabled when a game is detected as running or not.
        public bool EnableOnLaunch
        {
            get { return this.fix.EnableOnLaunch; }
        }

        // Gets the first value to be displayed on the main application.
        public string Value1
        {
            get { return this.fix.Value1; }
        }

        // Gets the second value to be displayed on the main application.
        public string Value2
        {
            get { return this.fix.Value2; }
        }

        // Gets the third value to be displayed on the main application.
        public string Value3
        {
            get { return this.fix.Value3; }
        }

        // Gets the fourth value to be displayed on the main application.
        public string Value4
        {
            get { return this.fix.Value4; }
        }

        // Gets the fifth value to be displayed on the main application.
        public string Value5
        {
            get { return this.fix.Value5; }
        }

        // Gets the title for the first value to be displayed on the main application.
        public string ValueTitle1
        {
            get { return this.fix.ValueTitle1; }
        }

        // Gets the title for the second value to be displayed on the main application.
        public string ValueTitle2
        {
            get { return this.fix.ValueTitle2; }
        }

        // Gets the title for the third value to be displayed on the main application.
        public string ValueTitle3
        {
            get { return this.fix.ValueTitle3; }
        }

        // Gets the title for the fourth value to be displayed on the main application.
        public string ValueTitle4
        {
            get { return this.fix.ValueTitle4; }
        }

        // Gets the title for the fifth value to be displayed on the main application.
        public string ValueTitle5
        {
            get { return this.fix.ValueTitle5; }
        }

        // Gets a value indicating whether the game is currently running or not.
        public bool GameRunning
        {
            get { return this.fix.GameRunning; }
        }

        // Shows the configuration form.
        public void ShowConfiguration()
        {
            this.fix.ShowConfiguration();
        }

        // Sets up the fix by getting the game's process ID, opening handles, and getting the base address.
        public bool Setup()
        {
            return this.fix.Setup();
        }

        // Updates the values that are shown on the main application.
        public void UpdateValues()
        {
            this.fix.UpdateValues();
        }

        // Enables the fix.
        public void Enable()
        {
            this.fix.Enable();
        }

        // Disables the fix.
        public void Disable()
        {
            this.fix.Disable();
        }

        // Cleans up the plugin.
        public void Finish()
        {
            this.fix.Finish();
        }
    }
}
