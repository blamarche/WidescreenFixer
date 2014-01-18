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
    using System.Runtime.InteropServices;

    // Sets up the interface and requirements for plugins.
    public interface IFixer
    {
        // Gets the name of the game being fixed.
        string GameName { get; }

        // Gets the icon used by the plugin.
        Bitmap GameIcon { get; }

        // Gets a value indicating whether the plugin is obsolete or not.
        bool PluginIsObsolete { get; }

        // Gets a value indicating whether the fix is enable or not.
        bool Enabled { get; }

        // Gets a value indicating whether the fix should be enabled when a game is detected as running or not.
        bool EnableOnLaunch { get; }

        // Gets the first value displayed on the main application.
        string Value1 { get; }

        // Gets the second value displayed on the main application.
        string Value2 { get; }

        // Gets the third value displayed on the main application.
        string Value3 { get; }

        // Gets the fourth value displayed on the main application.
        string Value4 { get; }

        // Gets the fifth value displayed on the main application.
        string Value5 { get; }

        // Gets the title of the first value displayed on the main application.
        string ValueTitle1 { get; }

        // Gets the title of the second value displayed on the main application.
        string ValueTitle2 { get; }

        // Gets the title of the third value displayed on the main application.
        string ValueTitle3 { get; }

        // Gets the title of the fourth value displayed on the main application.
        string ValueTitle4 { get; }

        // Gets the title of the fifth value displayed on the main application.
        string ValueTitle5 { get; }

        // Gets a value indicating whether the game is running or not.
        bool GameRunning { get; }

        // Shows the configuration form for the plugin.
        void ShowConfiguration();

        // Sets up the fix by opening handles, getting the process ID, and getting the base address.
        bool Setup();

        // Updates the values being displayed on the main application.
        void UpdateValues();

        // Enables the fix and starts a thread that will run until Disable() is called.
        void Enable();

        // This is the thread method that will be called by Enable().
        void Continual();

        // Disables the fix and the thread running from the Enable() method.
        void Disable();

        // Cleans up the fix, closes handles, and resets plugin settings back to the default state.
        void Finish();
    }
}
