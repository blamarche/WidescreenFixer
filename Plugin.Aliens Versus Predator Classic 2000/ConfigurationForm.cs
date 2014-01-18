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

namespace Plugin.AliensVersusPredatorClassic2000
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.Reflection;
    using System.Text;
    using System.Windows.Forms;
    using Library.DetectDisplays;

    public partial class ConfigurationForm : Form
    {
        private string websiteUrl = string.Empty;
        private string donateUrl = string.Empty;
        private string detailedReportUrl = string.Empty;

        public ConfigurationForm()
        {
            this.InitializeComponent();

            this.Load += new EventHandler(this.Configuration_Load);
            this.FormClosed += new FormClosedEventHandler(this.Configuration_FormClosed);

            this.comboBoxGameVersion.DrawItem += new DrawItemEventHandler(this.ComboBoxGameVersion_DrawItem);

            this.comboBoxGameVersion.Items.Add("Steam");  // 0

            // Plugin information goes below here.
            this.labelAuthor.Text = "Dopefish";
            this.linkLabelWebsite.Text = "Widescreen Fixer";
            this.websiteUrl = "https://www.widescreenfixer.org/";
            this.linkLabelDonate.Text = "PayPal";
            this.donateUrl = "https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&item_name=Donation%20for%20Widescreen%20Fixer%20%26%20Aliens%20Versus%20Predator%20Classic%202000&hosted_button_id=10165367";
            this.labelDescription.Text = "This plugin will center the HUD.";
            this.detailedReportUrl = "http://widescreengamingforum.com/dr/aliens-versus-predator";

            Assembly pluginAssembly = Assembly.GetExecutingAssembly();
            AssemblyName pluginName = pluginAssembly.GetName();
            Version pluginVersion = pluginName.Version;
            this.labelPluginVersion.Text = string.Format(CultureInfo.InvariantCulture, "{0}.{1} r{2}", pluginVersion.Major, pluginVersion.Minor, pluginVersion.Build);
        }

        private void Configuration_Load(object sender, EventArgs e)
        {
            this.toolTipAutoDetect.SetToolTip(this.checkBoxAutoDetectDisplay, "Enabling this setting enables automatic display detection.");

            this.comboBoxGameVersion.SelectedIndex = Properties.Settings.Default.GameVersion;

            this.numericTextBoxDisplayCount.Text = Properties.Settings.Default.DisplayCount.ToString(CultureInfo.InvariantCulture);
            this.numericTextBoxNormalX.Text = Properties.Settings.Default.NormalX.ToString(CultureInfo.InvariantCulture);
            this.numericTextBoxNormalY.Text = Properties.Settings.Default.NormalY.ToString(CultureInfo.InvariantCulture);
            this.numericTextBoxCorrectedX.Text = Properties.Settings.Default.CorrectedX.ToString(CultureInfo.InvariantCulture);
            this.numericTextBoxCorrectedY.Text = Properties.Settings.Default.CorrectedY.ToString(CultureInfo.InvariantCulture);

            if (Properties.Settings.Default.AutoDetection == CheckState.Checked)
            {
                this.checkBoxAutoDetectDisplay.CheckState = CheckState.Checked;

                this.numericTextBoxDisplayCount.Visible = false;
                this.numericTextBoxNormalX.Visible = false;
                this.numericTextBoxNormalY.Visible = false;
                this.numericTextBoxCorrectedX.Visible = false;
                this.numericTextBoxCorrectedY.Visible = false;
                this.labelCorrectedResolution.Visible = false;

                this.numericTextBoxAutoDisplayCount.Visible = true;
                this.numericTextBoxAutoDetectedX.Visible = true;
                this.numericTextBoxAutoDetectedY.Visible = true;
                this.labelUncorrectedResolution.Text = "Detected Resolution:";

                this.numericTextBoxAutoDisplayCount.Text = DetectedDisplays.DisplayCount.ToString(CultureInfo.InvariantCulture);
                this.numericTextBoxAutoDetectedX.Text = DetectedDisplays.HorizontalResolution.ToString(CultureInfo.InvariantCulture);
                this.numericTextBoxAutoDetectedY.Text = DetectedDisplays.VerticalResolution.ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                this.checkBoxAutoDetectDisplay.CheckState = CheckState.Unchecked;

                this.numericTextBoxDisplayCount.Visible = true;
                this.numericTextBoxNormalX.Visible = true;
                this.numericTextBoxNormalY.Visible = true;
                this.numericTextBoxCorrectedX.Visible = true;
                this.numericTextBoxCorrectedY.Visible = true;
                this.labelCorrectedResolution.Visible = true;

                this.numericTextBoxAutoDisplayCount.Visible = false;
                this.numericTextBoxAutoDetectedX.Visible = false;
                this.numericTextBoxAutoDetectedY.Visible = false;
                this.labelUncorrectedResolution.Text = "Uncorrected Resolution:";
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

            // Auto Detection
            try
            {
                Properties.Settings.Default.AutoDetection = this.checkBoxAutoDetectDisplay.CheckState;
            }
            catch
            {
                Properties.Settings.Default.AutoDetection = CheckState.Checked;

                throw;
            }

            // Display Count
            try
            {
                Properties.Settings.Default.DisplayCount = Convert.ToInt32(this.numericTextBoxDisplayCount.Text, CultureInfo.InvariantCulture);
            }
            catch
            {
                Properties.Settings.Default.DisplayCount = 3;

                throw;
            }

            // Standard X
            try
            {
                Properties.Settings.Default.NormalX = Convert.ToInt32(this.numericTextBoxNormalX.Text, CultureInfo.InvariantCulture);
            }
            catch
            {
                Properties.Settings.Default.NormalX = 5760;

                throw;
            }

            // Standard Y
            try
            {
                Properties.Settings.Default.NormalY = Convert.ToInt32(this.numericTextBoxNormalY.Text, CultureInfo.InvariantCulture);
            }
            catch
            {
                Properties.Settings.Default.NormalY = 1080;

                throw;
            }

            // Custom X
            try
            {
                Properties.Settings.Default.CorrectedX = Convert.ToInt32(this.numericTextBoxCorrectedX.Text, CultureInfo.InvariantCulture);
            }
            catch
            {
                Properties.Settings.Default.CorrectedX = 6056;

                throw;
            }

            // Custom Y
            try
            {
                Properties.Settings.Default.CorrectedY = Convert.ToInt32(this.numericTextBoxCorrectedY.Text, CultureInfo.InvariantCulture);
            }
            catch
            {
                Properties.Settings.Default.CorrectedY = 1080;

                throw;
            }

            // Save settings
            Properties.Settings.Default.Save();
        }

        private void CheckBoxAutoDetectDisplay_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBoxAutoDetectDisplay.CheckState == CheckState.Checked)
            {
                this.numericTextBoxDisplayCount.Visible = false;
                this.numericTextBoxNormalX.Visible = false;
                this.numericTextBoxNormalY.Visible = false;
                this.numericTextBoxCorrectedX.Visible = false;
                this.numericTextBoxCorrectedY.Visible = false;
                this.labelCorrectedResolution.Visible = false;

                this.numericTextBoxAutoDisplayCount.Visible = true;
                this.numericTextBoxAutoDetectedX.Visible = true;
                this.numericTextBoxAutoDetectedY.Visible = true;
                this.labelUncorrectedResolution.Text = "Detected Resolution:";

                this.numericTextBoxAutoDisplayCount.Text = DetectedDisplays.DisplayCount.ToString(CultureInfo.InvariantCulture);
                this.numericTextBoxAutoDetectedX.Text = DetectedDisplays.HorizontalResolution.ToString(CultureInfo.InvariantCulture);
                this.numericTextBoxAutoDetectedY.Text = DetectedDisplays.VerticalResolution.ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                this.numericTextBoxDisplayCount.Visible = true;
                this.numericTextBoxNormalX.Visible = true;
                this.numericTextBoxNormalY.Visible = true;
                this.numericTextBoxCorrectedX.Visible = true;
                this.numericTextBoxCorrectedY.Visible = true;
                this.labelCorrectedResolution.Visible = true;

                this.numericTextBoxAutoDisplayCount.Visible = false;
                this.numericTextBoxAutoDetectedX.Visible = false;
                this.numericTextBoxAutoDetectedY.Visible = false;
                this.labelUncorrectedResolution.Text = "Uncorrected Resolution:";
            }
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
