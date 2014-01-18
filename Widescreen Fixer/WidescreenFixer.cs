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

namespace WidescreenFixer
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms;
    using Library.DetectDisplays;
    using Library.Forms;
    using Library.Plugin;
    using Library.Process;
    using Library.Update;

    public partial class WidescreenFixerApp : Form
    {
        private FixerHost fixerHost = null;
        private bool gameChecked = true;
        private bool fixSetup = false;
        private int keyValue = 106; // Multiply key
        private int lastKeyState = 0;
        private bool valuesDisabled = false;

        public WidescreenFixerApp()
        {
            this.InitializeComponent();

            // Initialize the detect displays library.
            DetectedDisplays.Initialize();

            // Default Window State
            this.WindowState = FormWindowState.Normal;

            // Check if the application even cares
            if (Properties.Settings.Default.RememberWindowPosition)
            {
                // Are the bounds okay?
                if (Properties.Settings.Default.WindowGeometry != Rectangle.Empty && IsVisibleOnAnyScreen(Properties.Settings.Default.WindowGeometry))
                {
                    // Set bounds
                    this.StartPosition = FormStartPosition.Manual;
                    this.DesktopBounds = Properties.Settings.Default.WindowGeometry;

                    // Use saved state value
                    this.WindowState = Properties.Settings.Default.WindowState;
                }
                else
                {
                    // Reset upper-left corner
                    this.StartPosition = FormStartPosition.WindowsDefaultLocation;

                    // Restore saved size
                    if (Properties.Settings.Default.WindowGeometry != Rectangle.Empty)
                    {
                        try
                        {
                            this.Size = Properties.Settings.Default.WindowGeometry.Size;
                        }
                        catch
                        {
                            // MAKE SURE THIS IS UPDATED IF THE MAIN WINDOW FORM IS EVER RESIZED!
                            this.Size = new Size(334, 263);

                            throw;
                        }
                    }
                }
            }

            // Update the Game Selection
            this.UpdateGameSelection();
            this.fixerHost = this.comboBoxGameSelection.SelectedItem as FixerHost;

            this.comboBoxGameSelection.DrawItem += new DrawItemEventHandler(this.ComboBoxGameSelection_DrawItem);

            this.Load += new EventHandler(this.MainForm_Load);
            this.FormClosed += new FormClosedEventHandler(this.MainForm_FormClosed);
            this.Resize += new EventHandler(this.MainForm_Resize);
            this.notifyIcon.DoubleClick += new EventHandler(this.NotifyIcon_DoubleClick);
            this.timerValues.Tick += new EventHandler(this.TimerValues_Tick);
            this.comboBoxGameSelection.SelectedIndexChanged += new EventHandler(this.ComboBoxGameSelection_SelectedIndexChanged);
            this.textBoxHotkey.KeyDown += new KeyEventHandler(this.TextBoxHotkey_KeyDown);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            /*
            this.Text = string.Format(
                CultureInfo.CurrentCulture,
                "Widescreen Fixer v{0}.{1} r{2}",
                Assembly.GetExecutingAssembly().GetName().Version.Major,
                Assembly.GetExecutingAssembly().GetName().Version.Minor,
                Assembly.GetExecutingAssembly().GetName().Version.Build);
             */

            this.Text = string.Format(
                CultureInfo.CurrentCulture,
                "Widescreen Fixer v{0}.{1}",
                Assembly.GetExecutingAssembly().GetName().Version.Major,
                Assembly.GetExecutingAssembly().GetName().Version.Minor);

            // Load in tray if that's how it was saved
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
            }

            this.checkBoxUpdate.Checked = Properties.Settings.Default.CheckForUpdateOnLaunch;
            this.checkBoxWindowState.Checked = Properties.Settings.Default.RememberWindowPosition;

            // Hotkey name
            this.checkBoxKeyControl.Checked = Properties.Settings.Default.KeyControl;
            this.checkBoxKeyShift.Checked = Properties.Settings.Default.KeyShift;
            this.checkBoxKeyAlt.Checked = Properties.Settings.Default.KeyAlt;
            this.keyValue = Properties.Settings.Default.KeyValue;
            this.textBoxHotkey.Text = Enum.GetName(typeof(Keys), Properties.Settings.Default.KeyValue);

            // Set the Game Selection
            if (this.fixerHost != null)
            {
                // Game selection.
                try
                {
                    this.comboBoxGameSelection.SelectedIndex = Properties.Settings.Default.GameSelection;
                }
                catch
                {
                    this.comboBoxGameSelection.SelectedIndex = 0;

                    throw;
                }

                // Value title 1.
                try
                {
                    string title = this.fixerHost.ValueTitle1;

                    if (string.IsNullOrEmpty(title))
                    {
                        this.labelTitleValue1.Text = string.Empty;
                    }
                    else
                    {
                        this.labelTitleValue1.Text = title + ":";
                    }
                }
                catch
                {
                    this.labelTitleValue1.Text = string.Empty;

                    throw;
                }

                // Value title 2.
                try
                {
                    string title = this.fixerHost.ValueTitle2;

                    if (string.IsNullOrEmpty(title))
                    {
                        this.labelTitleValue2.Text = string.Empty;
                    }
                    else
                    {
                        this.labelTitleValue2.Text = title + ":";
                    }
                }
                catch
                {
                    this.labelTitleValue2.Text = string.Empty;

                    throw;
                }

                // Value title 3.
                try
                {
                    string title = this.fixerHost.ValueTitle3;

                    if (string.IsNullOrEmpty(title))
                    {
                        this.labelTitleValue3.Text = string.Empty;
                    }
                    else
                    {
                        this.labelTitleValue3.Text = title + ":";
                    }
                }
                catch
                {
                    this.labelTitleValue3.Text = string.Empty;

                    throw;
                }

                // Value title 4.
                try
                {
                    string title = this.fixerHost.ValueTitle4;

                    if (string.IsNullOrEmpty(title))
                    {
                        this.labelTitleValue4.Text = string.Empty;
                    }
                    else
                    {
                        this.labelTitleValue4.Text = title + ":";
                    }
                }
                catch
                {
                    this.labelTitleValue4.Text = string.Empty;

                    throw;
                }

                // Value title 5.
                try
                {
                    string title = this.fixerHost.ValueTitle5;

                    if (string.IsNullOrEmpty(title))
                    {
                        this.labelTitleValue5.Text = string.Empty;
                    }
                    else
                    {
                        this.labelTitleValue5.Text = title + ":";
                    }
                }
                catch
                {
                    this.labelTitleValue5.Text = string.Empty;

                    throw;
                }
            }
            else
            {
                // Game selection.
                try
                {
                    this.comboBoxGameSelection.SelectedIndex = 0;
                }
                catch
                {
                    this.comboBoxGameSelection.SelectedIndex = -1;

                    throw;
                }

                // Set value1 title.
                if (string.IsNullOrEmpty(Properties.Settings.Default.LastValue1Title))
                {
                    this.labelTitleValue1.Text = string.Empty;
                }
                else
                {
                    this.labelTitleValue1.Text = Properties.Settings.Default.LastValue1Title;
                }

                // Set value2 title.
                if (string.IsNullOrEmpty(Properties.Settings.Default.LastValue2Title))
                {
                    this.labelTitleValue2.Text = string.Empty;
                }
                else
                {
                    this.labelTitleValue2.Text = Properties.Settings.Default.LastValue2Title;
                }

                // Set value3 title.
                if (string.IsNullOrEmpty(Properties.Settings.Default.LastValue3Title))
                {
                    this.labelTitleValue3.Text = string.Empty;
                }
                else
                {
                    this.labelTitleValue3.Text = Properties.Settings.Default.LastValue3Title;
                }

                // Set value4 title.
                if (string.IsNullOrEmpty(Properties.Settings.Default.LastValue4Title))
                {
                    this.labelTitleValue4.Text = string.Empty;
                }
                else
                {
                    this.labelTitleValue4.Text = Properties.Settings.Default.LastValue4Title;
                }

                // Set value5 title.
                if (string.IsNullOrEmpty(Properties.Settings.Default.LastValue5Title))
                {
                    this.labelTitleValue5.Text = string.Empty;
                }
                else
                {
                    this.labelTitleValue5.Text = Properties.Settings.Default.LastValue5Title;
                }
            }

            if (Properties.Settings.Default.CheckForUpdateOnLaunch)
            {
                new Thread(new ThreadStart(this.UpdateThread)).Start();
            }
        }

        private void UpdateThread()
        {
            UpdateCheck.CheckForUpdate();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Check if a fixer host exists and finish it up so we can cleanly exit.
            if (this.fixerHost != null)
            {
                if (this.fixerHost.Enabled)
                {
                    this.fixerHost.Disable();
                }

                this.fixerHost.Finish();
            }

            // Main Tab
            Properties.Settings.Default.GameSelection = this.comboBoxGameSelection.SelectedIndex;

            // Settings Tab
            Properties.Settings.Default.CheckForUpdateOnLaunch = this.checkBoxUpdate.Checked;
            Properties.Settings.Default.RememberWindowPosition = this.checkBoxWindowState.Checked;

            // Hotkey settings
            Properties.Settings.Default.KeyControl = this.checkBoxKeyControl.Checked;
            Properties.Settings.Default.KeyShift = this.checkBoxKeyShift.Checked;
            Properties.Settings.Default.KeyAlt = this.checkBoxKeyAlt.Checked;
            Properties.Settings.Default.KeyValue = this.keyValue;

            // Last Value Titles
            Properties.Settings.Default.LastValue1Title = this.labelTitleValue1.Text;
            Properties.Settings.Default.LastValue2Title = this.labelTitleValue2.Text;
            Properties.Settings.Default.LastValue3Title = this.labelTitleValue3.Text;
            Properties.Settings.Default.LastValue4Title = this.labelTitleValue4.Text;
            Properties.Settings.Default.LastValue5Title = this.labelTitleValue5.Text;

            // Window Position and State
            if (this.checkBoxWindowState.Checked)
            {
                Properties.Settings.Default.WindowState = this.WindowState;
                if (this.WindowState == FormWindowState.Normal)
                {
                    Properties.Settings.Default.WindowGeometry = this.DesktopBounds;
                }
            }

            // Save Settings
            Properties.Settings.Default.Save();

            // Is this necessary?
            //// this.notifyIcon.Dispose();
            //// this.notifyIcon = null;

            Application.Exit();
        }

        private void TimerValues_Tick(object sender, EventArgs e)
        {
            if (this.fixerHost != null)
            {
                if (this.fixerHost.GameRunning)
                {
                    if (this.fixSetup)
                    {
                        this.valuesDisabled = false;

                        this.fixerHost.UpdateValues();
                        this.labelGameRunningValue.Text = "Yes";

                        if (this.fixerHost.Enabled)
                        {
                            this.labelFixEnabledValue.Text = "Yes";
                        }
                        else
                        {
                            this.labelFixEnabledValue.Text = "No";
                        }

                        this.labelValue1.Text = this.fixerHost.Value1;
                        this.labelValue2.Text = this.fixerHost.Value2;
                        this.labelValue3.Text = this.fixerHost.Value3;
                        this.labelValue4.Text = this.fixerHost.Value4;
                        this.labelValue5.Text = this.fixerHost.Value5;
                    }
                }
                else
                {
                    if (!this.valuesDisabled)
                    {
                        this.labelGameRunningValue.Text = "No";
                        this.labelFixEnabledValue.Text = "No";
                        this.labelValue1.Text = string.Empty;
                        this.labelValue2.Text = string.Empty;
                        this.labelValue3.Text = string.Empty;
                        this.labelValue4.Text = string.Empty;
                        this.labelValue5.Text = string.Empty;

                        this.valuesDisabled = true;
                    }
                }
            }
        }

        private void TimerHotkey_Tick(object sender, EventArgs e)
        {
            bool control = this.checkBoxKeyControl.Checked;
            bool shift = this.checkBoxKeyShift.Checked;
            bool alt = this.checkBoxKeyAlt.Checked;

            int controlKey = ProcessFunctions.GetAsynchronousKeyState((int)Keys.ControlKey);
            int shiftKey = ProcessFunctions.GetAsynchronousKeyState((int)Keys.ShiftKey);
            int altKey = ProcessFunctions.GetAsynchronousKeyState((int)Keys.Menu);

            int keyState = ProcessFunctions.GetAsynchronousKeyState(this.keyValue) & 0x8000;

            if (control && shift && alt)
            {
                if (controlKey != 0 && shiftKey != 0 && altKey != 0 && keyState > 0 && keyState != this.lastKeyState)
                {
                    this.EnableDisableFix();
                }
            }

            if (control && shift && !alt)
            {
                if (controlKey != 0 && shiftKey != 0 && keyState > 0 && keyState != this.lastKeyState)
                {
                    this.EnableDisableFix();
                }
            }

            if (control && !shift && alt)
            {
                if (controlKey != 0 && altKey != 0 && keyState > 0 && keyState != this.lastKeyState)
                {
                    this.EnableDisableFix();
                }
            }

            if (!control && shift && alt)
            {
                if (shiftKey != 0 && altKey != 0 && keyState > 0 && keyState != this.lastKeyState)
                {
                    this.EnableDisableFix();
                }
            }

            if (control && !shift && !alt)
            {
                if (controlKey != 0 && keyState > 0 && keyState != this.lastKeyState)
                {
                    this.EnableDisableFix();
                }
            }

            if (!control && shift && !alt)
            {
                if (shiftKey != 0 && keyState > 0 && keyState != this.lastKeyState)
                {
                    this.EnableDisableFix();
                }
            }

            if (!control && !shift && alt)
            {
                if (altKey != 0 && keyState > 0 && keyState != this.lastKeyState)
                {
                    this.EnableDisableFix();
                }
            }

            if (!control && !shift && !alt)
            {
                if (keyState > 0 && keyState != this.lastKeyState)
                {
                    this.EnableDisableFix();
                }
            }

            this.lastKeyState = ProcessFunctions.GetAsynchronousKeyState(this.keyValue) & 0x8000;
        }

        private void EnableDisableFix()
        {
            if (this.fixerHost != null)
            {
                if (this.fixerHost.Enabled)
                {
                    if (this.fixerHost.GameRunning)
                    {
                        this.labelFixEnabledValue.Text = "No";
                        this.fixerHost.Disable();
                    }
                }
                else
                {
                    if (this.fixerHost.GameRunning)
                    {
                        this.labelFixEnabledValue.Text = "Yes";
                        this.fixerHost.Enable();
                    }
                }
            }
        }

        private void ComboBoxGameSelection_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.fixerHost != null)
            {
                this.fixerHost.Disable();
                this.fixerHost.Finish();
            }

            this.labelGameRunningValue.Text = "No";
            this.labelFixEnabledValue.Text = "No";
            this.labelValue1.Text = string.Empty;
            this.labelValue2.Text = string.Empty;
            this.labelValue3.Text = string.Empty;
            this.labelValue4.Text = string.Empty;
            this.labelValue5.Text = string.Empty;

            if (this.comboBoxGameSelection.SelectedItem != null && this.comboBoxGameSelection.SelectedIndex >= 0)
            {
                this.fixerHost = this.comboBoxGameSelection.SelectedItem as FixerHost;
                this.fixSetup = false;
            }

            // Sets the title for the first custom value slot.
            if (!string.IsNullOrEmpty(this.fixerHost.ValueTitle1))
            {
                this.labelTitleValue1.Text = this.fixerHost.ValueTitle1 + ":";
            }
            else
            {
                this.labelTitleValue1.Text = string.Empty;
            }

            // Sets the title for the second custom value slot.
            if (!string.IsNullOrEmpty(this.fixerHost.ValueTitle2))
            {
                this.labelTitleValue2.Text = this.fixerHost.ValueTitle2 + ":";
            }
            else
            {
                this.labelTitleValue2.Text = string.Empty;
            }

            // Sets the title for the third custom value slot.
            if (!string.IsNullOrEmpty(this.fixerHost.ValueTitle3))
            {
                this.labelTitleValue3.Text = this.fixerHost.ValueTitle3 + ":";
            }
            else
            {
                this.labelTitleValue3.Text = string.Empty;
            }

            // Sets the title for the fourth custom value slot.
            if (!string.IsNullOrEmpty(this.fixerHost.ValueTitle4))
            {
                this.labelTitleValue4.Text = this.fixerHost.ValueTitle4 + ":";
            }
            else
            {
                this.labelTitleValue4.Text = string.Empty;
            }

            // Sets the title for the fifth custom value slot.
            if (!string.IsNullOrEmpty(this.fixerHost.ValueTitle5))
            {
                this.labelTitleValue5.Text = this.fixerHost.ValueTitle5 + ":";
            }
            else
            {
                this.labelTitleValue5.Text = string.Empty;
            }
        }

        private void ComboBoxGameSelection_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            e.DrawFocusRectangle();

            if (e.Index >= 0)
            {
                FixerHost item = (FixerHost)this.comboBoxGameSelection.Items[e.Index];
                if (item.GameIcon != null)
                {
                    Bitmap icon = item.GameIcon;
                    e.Graphics.DrawImage(icon, e.Bounds.X, e.Bounds.Y, 15, 15);
                    using (SolidBrush solidBrush = new SolidBrush(e.ForeColor))
                    {
                        e.Graphics.DrawString(item.GameName, e.Font, solidBrush, new Point(e.Bounds.X + 16, e.Bounds.Y + 1));
                    }
                }
                else
                {
                    using (SolidBrush solidBrush = new SolidBrush(e.ForeColor))
                    {
                        e.Graphics.DrawString(item.GameName, e.Font, solidBrush, new Point(e.Bounds.X + 16, e.Bounds.Y + 1));
                    }
                }
            }
        }

        private void UpdateGameSelection()
        {
            List<FixerHost> fixes = new List<FixerHost>();
            FixerHostProvider fixerHostProvider = new FixerHostProvider();

            foreach (FixerHost fix in fixerHostProvider.Fixers)
            {
                fixes.Add(fix);
            }

            this.comboBoxGameSelection.DisplayMember = "FixName";
            this.comboBoxGameSelection.DataSource = fixes;

            if (fixes.Count <= 0)
            {
                this.buttonConfigure.Enabled = false;
            }
        }

        private void TextBoxHotkey_KeyDown(object sender, KeyEventArgs e)
        {
            // Stops the ping sound when the alt or control key are used in the textbox.
            e.SuppressKeyPress = e.KeyCode == e.KeyCode;

            this.keyValue = e.KeyValue;
            this.textBoxHotkey.Text = e.KeyCode.ToString();
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
            }
        }

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void ToolStripMenuItemRestore_Click(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void ToolStripMenuItemExit_Click(object sender, EventArgs e)
        {
            // Is this necessary?
            //// this.notifyIcon.Dispose();

            Application.Exit();
        }

        private void ButtonDonate_Click(object sender, EventArgs e)
        {
            const string Url = "https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&item_name=Donation%20for%20Widescreen%20Fixer&hosted_button_id=10165367";

            System.Diagnostics.Process.Start(Url);
        }

        private void LinkLabelHomePage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            const string Url = "https://www.widescreenfixer.org/";

            System.Diagnostics.Process.Start(Url);
        }

        private static bool IsVisibleOnAnyScreen(Rectangle rectangle)
        {
            foreach (Screen screen in Screen.AllScreens)
            {
                if (screen.WorkingArea.IntersectsWith(rectangle))
                {
                    return true;
                }
            }

            return false;
        }

        private void LinkLabelWidescreenForum_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            const string Url = "http://www.widescreengamingforum.com/";

            System.Diagnostics.Process.Start(Url);
        }

        private void ButtonConfigure_Click(object sender, EventArgs e)
        {
            if (this.fixerHost != null)
            {
                this.fixerHost.ShowConfiguration();
            }
        }

        private void ButtonDisplayInfo_Click(object sender, EventArgs e)
        {
            string displayInfo = string.Format(
                CultureInfo.CurrentCulture,
                "Video Card: {0}\n" +
                "Video Card Brand: {1}\n" +
                "Error? {2}\n" +
                "Error Code: {3}\n" +
                "Error String: {4}\n" +
                "Multi-Monitor Enabled? {5}\n" +
                "Display Count: {6}\n" +
                "Column Count: {7}\n" +
                "Row Count: {8}\n" +
                "Bezel Corrected? {9}\n" +
                "Horizontal Resolution: {10}\n" +
                "Vertical Resolution: {11}\n" +
                "Aspect-Ratio: {12}\n" +
                "Screen Orientation: {13}\n" +
                "HUD Left: {14}\n" +
                "HUD Right: {15}\n" +
                "HUD Top: {16}\n" +
                "HUD Bottom: {17}\n" +
                "HUD Width: {18}\n" +
                "HUD Height: {19}",
                DetectedDisplays.VideoCard,
                DetectedDisplays.Brand.ToString(),
                DetectedDisplays.Error == true ? "Yes" : "No",
                DetectedDisplays.ErrorCode,
                string.IsNullOrEmpty(DetectedDisplays.ErrorString) ? "N/A" : DetectedDisplays.ErrorString,
                DetectedDisplays.MultipleMonitorEnabled == true ? "Yes" : "No",
                DetectedDisplays.DisplayCount,
                DetectedDisplays.ColumnCount,
                DetectedDisplays.RowCount,
                DetectedDisplays.BezelCorrected == true ? "Yes" : "No",
                DetectedDisplays.HorizontalResolution,
                DetectedDisplays.VerticalResolution,
                DetectedDisplays.AspectRatio,
                DetectedDisplays.Landscape == true ? "Landscape" : "Portrait",
                DetectedDisplays.HudLeft,
                DetectedDisplays.HudRight,
                DetectedDisplays.HudTop,
                DetectedDisplays.HudBottom,
                DetectedDisplays.HudWidth,
                DetectedDisplays.HudHeight);

            MessageBox.Show(displayInfo, "Detected Display Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void TimerSetup_Tick(object sender, EventArgs e)
        {
            if (this.fixerHost != null)
            {
                if (this.fixerHost.GameRunning)
                {
                    if (!this.fixSetup)
                    {
                        // Run Setup() so that UpdateValues() will work correctly.
                        while (!this.fixerHost.Setup())
                        {
                            // Do nothing until it returns successfully.
                        }

                        // Check if the fix should be enabled as soon as the game is launched.
                        if (this.fixerHost.EnableOnLaunch)
                        {
                            this.fixerHost.Enable();
                        }

                        this.fixSetup = true;       // Mark the fix as setup so that Setup() doesn't repeatedly get executed.
                        this.gameChecked = true;    // Mark the game as checked so that when the game is not running, the text labels won't be updated each tick.
                    }
                }
                else
                {
                    if (this.gameChecked)
                    {
                        if (this.fixSetup)
                        {
                            this.fixerHost.Disable();
                        }
                    }

                    this.fixSetup = false;      // Mark the fix as not setup so it can be setup next time.
                    this.gameChecked = false;   // Mark the game as not checked so text is not repeatedly updated.
                }
            }
        }
    }
}
