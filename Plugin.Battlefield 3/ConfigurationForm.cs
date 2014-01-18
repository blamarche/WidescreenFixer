#region File Information
/*
 * Copyright (C) 2012-2014 David Rudie
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

namespace Plugin.Battlefield3
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.Reflection;
    using System.Text;
    using System.Windows.Forms;

    public partial class ConfigurationForm : Form, IConfiguration
    {
        private string websiteUrl = string.Empty;
        private string donateUrl = string.Empty;
        private string detailedReportUrl = string.Empty;

        private volatile float trackBarValue = 0.0f;

        public ConfigurationForm()
        {
            this.InitializeComponent();

            this.FormClosed += new FormClosedEventHandler(this.Configuration_FormClosed);

            this.comboBoxGameVersion.DrawItem += new DrawItemEventHandler(this.ComboBoxGameVersion_DrawItem);

            this.comboBoxGameVersion.Items.Add("v1147186"); // 0
            this.comboBoxGameVersion.Items.Add("v1089904"); // 1
            this.comboBoxGameVersion.Items.Add("v981420");  // 2
            this.comboBoxGameVersion.Items.Add("v944019");  // 3
            this.comboBoxGameVersion.Items.Add("v936211");  // 4
            this.comboBoxGameVersion.Items.Add("v925790");  // 5

            try
            {
                this.trackBarFieldOfView.Value = Properties.Settings.Default.TrackBarValue;
                this.labelTrackBarValue.Text = Properties.Settings.Default.LabelTrackBarValue;

                this.comboBoxGameVersion.SelectedIndex = Properties.Settings.Default.GameVersion;
            }
            catch
            {
                this.trackBarFieldOfView.Value = 500;
                this.labelTrackBarValue.Text = "0";

                this.comboBoxGameVersion.SelectedIndex = 0;

                throw;
            }

            // Plugin information goes below here.
            this.labelAuthor.Text = "Dopefish";
            this.linkLabelWebsite.Text = "Widescreen Fixer";
            this.websiteUrl = "https://www.widescreenfixer.org/";
            this.linkLabelDonate.Text = "PayPal";
            this.donateUrl = "https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&item_name=Donation%20for%20Widescreen%20Fixer%20%26%20Battlefield%203&hosted_button_id=10165367";
            this.labelDescription.Text = "This plugin will add the specified amount of degrees to the game's field-of-view.\n\nDO NOT USE ONLINE!";
            this.detailedReportUrl = "http://www.wsgf.org/dr/battlefield-3";

            Assembly pluginAssembly = Assembly.GetExecutingAssembly();
            AssemblyName pluginName = pluginAssembly.GetName();
            Version pluginVersion = pluginName.Version;
            this.labelPluginVersion.Text = string.Format(CultureInfo.InvariantCulture, "{0}.{1} r{2}", pluginVersion.Major, pluginVersion.Minor, pluginVersion.Build);
        }

        public int GameVersion
        {
            get
            {
                return this.comboBoxGameVersion.SelectedIndex;
            }
        }

        public float AmountToModifyFieldOfView
        {
            get
            {
                return this.trackBarValue;
            }
        }

        private void Configuration_FormClosed(object sender, EventArgs e)
        {
            this.SaveSettings();
        }

        private void ComboBoxGameVersion_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            e.DrawFocusRectangle();

            if (e.Index >= 0)
            {
                string item = (string)this.comboBoxGameVersion.Items[e.Index];
                using (SolidBrush solidBrush = new SolidBrush(e.ForeColor))
                {
                    e.Graphics.DrawString(item, e.Font, solidBrush, new Point(e.Bounds.X, e.Bounds.Y + 1));
                }
            }
        }

        private void TrackBarFieldOfView_ValueChanged(object sender, EventArgs e)
        {
            this.labelTrackBarValue.Text = string.Format(CultureInfo.InvariantCulture, "{0}", (float)(this.trackBarFieldOfView.Value - 500) / 10.0f);
            this.trackBarValue = (this.trackBarFieldOfView.Value - 500) / 10.0f;
        }

        private void SaveSettings()
        {
            // Game version
            try
            {
                Properties.Settings.Default.GameVersion = this.comboBoxGameVersion.SelectedIndex;
            }
            catch
            {
                Properties.Settings.Default.GameVersion = 0;

                throw;
            }

            // Trackbar value
            try
            {
                Properties.Settings.Default.TrackBarValue = this.trackBarFieldOfView.Value;
            }
            catch
            {
                Properties.Settings.Default.TrackBarValue = 500;

                throw;
            }

            // Label value
            try
            {
                Properties.Settings.Default.LabelTrackBarValue = string.Format(CultureInfo.InvariantCulture, "{0}", (float)(this.trackBarFieldOfView.Value - 500) / 10.0f);
            }
            catch
            {
                Properties.Settings.Default.LabelTrackBarValue = "0";

                throw;
            }

            // Save settings
            Properties.Settings.Default.Save();
        }

        private void ButtonSaveSettings_Click(object sender, EventArgs e)
        {
            this.SaveSettings();
        }

        private void ButtonCloseWindow_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void LinkLabelWebsite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(this.websiteUrl);
        }

        private void LinkLabelDonate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(this.donateUrl);
        }

        private void LinkLabelWSGFDetailedReport_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(this.detailedReportUrl);
        }
    }
}
