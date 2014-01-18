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
    using System.Threading;
    using System.Windows.Forms;
    using Library.DetectDisplays;
    using Library.Process;

    // This is the base class that a plugin will use and override.
    public class BasePlugin
    {
        #region These need set via the inherited constructor

        // The icon used by the plugin.
        private Bitmap gameIcon = null;

        #endregion

        #region Constructor

        // Initializes a new instance of the BasePlugin class.
        public BasePlugin()
        {
            //// Nothing to do yet.  Reserved space for later.
        }
        
        #endregion

        #region Properties

        // Gets or sets a value indicating the name of the game being fixed by the plugin.
        public virtual string GameName { get; set; }

        // Gets or sets the icon used in the combo-box for game selection, as well as the icon used in the configuration form.
        public virtual Bitmap GameIcon
        {
            get { return new Bitmap(this.gameIcon); }
            set { this.gameIcon = value; }
        }

        // Gets or sets a value indicating whether the plugin is obsolete.
        public virtual bool PluginIsObsolete { get; set; }

        // Gets or sets a value indicating whether the fix for the game is currently enabled or not.
        public virtual bool Enabled { get; set; }

        // Gets or sets a value indicating whether the fix should be enabled as soon as the game is detected as running or not.
        public virtual bool EnableOnLaunch { get; set; }

        // Gets or sets the title for the first value that will be displayed on the main application.
        public virtual string ValueTitle1 { get; set; }

        // Gets or sets the first value that will be displayed on the main application.
        public virtual string Value1 { get; set; }

        // Gets or sets the title for the second value that will be displayed on the main application.
        public virtual string ValueTitle2 { get; set; }

        // Gets or sets the second value that will be displayed on the main application.
        public virtual string Value2 { get; set; }

        // Gets or sets the title for the third value that will be displayed on the main application.
        public virtual string ValueTitle3 { get; set; }

        // Gets or sets the third value that will be displayed on the main application.
        public virtual string Value3 { get; set; }

        // Gets or sets the title for the fourth value that will be displayed on the main application.
        public virtual string ValueTitle4 { get; set; }

        // Gets or sets the fourth value that will be displayed on the main application.
        public virtual string Value4 { get; set; }

        // Gets or sets the title for the fifth value that will be displayed on the main application.
        public virtual string ValueTitle5 { get; set; }

        // Gets or sets the fifth value that will be displayed on the main application.
        public virtual string Value5 { get; set; }

        // Gets a value indicating whether the game is currently running or not.
        public virtual bool GameRunning
        {
            get
            {
                this.UpdateOffsets();

                if (this.ExeName.GetProcessId() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        // Gets a value indicating whether the process ID and base address are still valid.
        public virtual bool SetupIsValid
        {
            get
            {
                if (this.ProcessId > 0 && (this.BaseAddress > 0 || this.BaseAddress64 > 0))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        // Gets or sets the name of the executable file.  This will be used to determine if the game is running or not and to get the process ID.
        public virtual string ExeName { get; set; }

        // Gets or sets the name of the window class.
        public virtual string WindowClass { get; set; }

        // Gets or sets the title of the window.
        public virtual string WindowTitle { get; set; }

        // Gets or sets the process ID of the game.
        public virtual int ProcessId { get; set; }

        // Gets or sets a value indicating whether the parent process ID should be used instead of the child process ID.
        public virtual bool UseParentProcessId { get; set; }

        // Gets or sets the handle of the game process.
        public virtual IntPtr ProcessHandle { get; set; }

        // Gets or sets the base address of the game process.
        public virtual int BaseAddress { get; set; }

        // Gets or sets the base address of the game process.
        public virtual long BaseAddress64 { get; set; }

        // Gets or sets the pointer of the base address of the game process.
        public virtual IntPtr PointerBaseAddress { get; set; }

        // Gets or sets the base size of the game process.
        public virtual int BaseSize { get; set; }

        // Gets or sets the configuration form that will be loaded when the user clicks on the configure button.
        public virtual Form ConfigurationForm { get; set; }

        // Gets or sets the thread to be used.
        public virtual Thread Thread { get; set; }

        // Gets or sets a value indicating whether to stop the thread or not.
        public virtual bool StopThread { get; set; }

        #endregion

        #region Methods

        // Updates the offsets that will be used by the Enable() method.
        public virtual void UpdateOffsets()
        {
        }

        // Displays the configuration form for the plugin.
        public virtual void ShowConfiguration()
        {
            // Set the form icon.
            Bitmap icon = this.GameIcon;
            IntPtr iconHandle = icon.GetHicon();
            System.Drawing.Icon formIcon = System.Drawing.Icon.FromHandle(iconHandle);
            this.ConfigurationForm.Icon = formIcon;

            // Set the form title.
            this.ConfigurationForm.Text = this.GameName;

            // Show the window.
            this.ConfigurationForm.ShowDialog();

            // Destroy the form icon.
            UnsafeNativeMethods.DestroyIcon(formIcon.Handle);
        }

        // Sets up the plugin by opening the process handle, getting the process ID, and getting the base address.
        public virtual bool Setup()
        {
            this.UpdateOffsets();

            if (!string.IsNullOrEmpty(this.WindowClass) && !string.IsNullOrEmpty(this.WindowTitle))
            {
                // Both the window class and window title are set.
                this.ProcessId = ProcessFunctions.GetProcessIdFromWindow(this.WindowClass, this.WindowTitle);
            }
            else if (!string.IsNullOrEmpty(this.WindowClass) && string.IsNullOrEmpty(this.WindowTitle))
            {
                // Just the window class is set and not the title.
                this.ProcessId = ProcessFunctions.GetProcessIdFromWindow(this.WindowClass, string.Empty);
            }
            else
            {
                // Neither the window class or the title are set.
                this.ProcessId = this.ExeName.GetProcessId();
            }

            if (this.ProcessId > 0)
            {
                if (this.UseParentProcessId)
                {
                    System.Diagnostics.Process parentProcess = ProcessFunctions.ParentProcessUtilities.GetParentProcess(this.ProcessId);

                    this.ProcessId = parentProcess.Id;
                }

                this.ProcessHandle = this.ProcessId.OpenProcess(Enumerations.ProcessAccess.VmAll);

                this.BaseAddress = this.ProcessId.GetBaseAddress();
                this.BaseAddress64 = this.ProcessId.GetBaseAddress64(this.ExeName);
                this.PointerBaseAddress = (IntPtr)this.BaseAddress;

                if (IntPtr.Size == 8 || (IntPtr.Size == 4 && ProcessFunctions.Is32BitProcessOn64BitProcessor(this.ProcessId)))
                {
                    this.PointerBaseAddress = (IntPtr)this.BaseAddress64;
                }
                else
                {
                    this.PointerBaseAddress = (IntPtr)this.BaseAddress;
                }

                this.BaseSize = this.ProcessId.GetBaseSize();

                // Check if the base address is valid.
                if (this.BaseAddress > 0 || this.BaseAddress64 > 0)
                {
                    return true;
                }
            }

            return false;
        }

        // Updates the values that will be displayed on the main application.
        public virtual void UpdateValues()
        {
            if (!this.SetupIsValid)
            {
                return;
            }
        }

        // Enables the fix and starts a thread.
        public virtual void Enable()
        {
            if (!this.SetupIsValid)
            {
                return;
            }

            // Mark the plugin as enabled.
            this.Enabled = true;

            this.Thread = new Thread(new ThreadStart(this.Continual));
        }

        // This is thread that will continuously run.
        public virtual void Continual()
        {
            if (!this.SetupIsValid)
            {
                return;
            }
        }

        // Stops the fix and ends a thread.
        public virtual void Disable()
        {
            // Mark the fix as disabled.
            this.Enabled = false;

            // Mark the thread as ready to be stopped.
            this.StopThread = true;

            // Check if the thread is valid first.
            if (this.Thread != null)
            {
                // Check if the thread was ever started.
                if (this.Thread.IsAlive)
                {
                    // Check if the thread has stopped and wait for 2 seconds otherwise.
                    this.Thread.Join(2000);
                }

                // Reset the StopThread state.
                this.StopThread = false;
            }

            if (!this.SetupIsValid)
            {
                return;
            }
        }

        // Resets the plugin to the default state.
        public virtual void Finish()
        {
            if (this.ProcessHandle != IntPtr.Zero)
            {
                this.ProcessHandle.CloseMemory();
            }

            this.Enabled = false;
            this.Thread = null;
            this.StopThread = false;
            this.ProcessId = 0;
            this.ProcessHandle = IntPtr.Zero;
            this.BaseAddress = 0;
            this.BaseAddress64 = 0L;
            this.BaseSize = 0;
            this.ValueTitle1 = string.Empty;
            this.Value1 = string.Empty;
            this.ValueTitle2 = string.Empty;
            this.Value2 = string.Empty;
            this.ValueTitle3 = string.Empty;
            this.Value3 = string.Empty;
            this.ValueTitle4 = string.Empty;
            this.Value4 = string.Empty;
            this.ValueTitle5 = string.Empty;
            this.Value5 = string.Empty;
        }

        #endregion
    }
}
