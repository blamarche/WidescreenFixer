#region File Information
/*
 * Copyright (C) 2013-2014 David Rudie
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

namespace Plugin.CallOfDutyGhosts
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.Reflection;
    using System.Text;
    using System.Windows.Forms;
    using Library.DetectDisplays;

    public partial class ConfigurationForm : Form, IConfiguration
    {
        private string websiteUrl = string.Empty;
        private string donateUrl = string.Empty;
        private string detailedReportUrl = string.Empty;

        private volatile string displayCount = "3";
        private volatile string uncorrectedX = "5760";
        private volatile string correctedX = "6056";
        private volatile string uncorrectedY = "1080";
        private volatile string correctedY = "1080";

        public ConfigurationForm()
        {
            this.InitializeComponent();

            this.FormClosed += new FormClosedEventHandler(this.Configuration_FormClosed);

            this.comboBoxGameVersion.DrawItem += new DrawItemEventHandler(this.ComboBoxGameVersion_DrawItem);
            this.comboBoxGameVersion.Items.Add("v3.6 (Multi-Player)");
            this.comboBoxGameVersion.Items.Add("v3.6 (Single-Player)");
            this.comboBoxGameVersion.Items.Add("v3.5 (Multi-Player)");
            this.comboBoxGameVersion.Items.Add("v3.5 (Single-Player)");
            this.comboBoxGameVersion.Items.Add("v3.4 (Multi-Player)");
            this.comboBoxGameVersion.Items.Add("v3.4 (Single-Player)");
            this.comboBoxGameVersion.Items.Add("v3.3 (Multi-Player)");
            this.comboBoxGameVersion.Items.Add("v3.3 (Single-Player)");
            this.comboBoxGameVersion.Items.Add("v3.2 (Multi-Player)");
            this.comboBoxGameVersion.Items.Add("v3.2 (Single-Player)");

            this.toolTipAutoDetect.SetToolTip(this.checkBoxAutoDetectDisplay, "Enabling this setting enables automatic display detection.");

            try
            {
                this.comboBoxGameVersion.Text = Properties.Settings.Default.GameVersion;
            }
            catch
            {
                this.comboBoxGameVersion.SelectedIndex = 0;

                throw;
            }

            this.numericTextBoxDisplayCount.Text = Properties.Settings.Default.DisplayCount.ToString(CultureInfo.InvariantCulture);
            this.numericTextBoxNormalX.Text = Properties.Settings.Default.UncorrectedX.ToString(CultureInfo.InvariantCulture);
            this.numericTextBoxNormalY.Text = Properties.Settings.Default.UncorrectedY.ToString(CultureInfo.InvariantCulture);
            this.numericTextBoxCorrectedX.Text = Properties.Settings.Default.CorrectedX.ToString(CultureInfo.InvariantCulture);
            this.numericTextBoxCorrectedY.Text = Properties.Settings.Default.CorrectedY.ToString(CultureInfo.InvariantCulture);

            if (Properties.Settings.Default.AutoDetection)
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

            // Plugin information goes below here.
            this.labelAuthor.Text = "Dopefish";
            this.linkLabelWebsite.Text = "Widescreen Fixer";
            this.websiteUrl = "https://www.widescreenfixer.org/";
            this.linkLabelDonate.Text = "PayPal";
            this.donateUrl = "https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&item_name=Donation%20for%20Widescreen%20Fixer%20%26%20Call%20of%20Duty:%20Ghosts&hosted_button_id=10165367";
            this.labelDescription.Text = "This plugin will correct the game's aspect-ratio as well as reposition various interface elements.";
            this.detailedReportUrl = "http://widescreengamingforum.com/dr/call-duty-ghosts/en";

            Assembly pluginAssembly = Assembly.GetExecutingAssembly();
            AssemblyName pluginName = pluginAssembly.GetName();
            Version pluginVersion = pluginName.Version;
            this.labelPluginVersion.Text = string.Format(CultureInfo.InvariantCulture, "{0}.{1} r{2}", pluginVersion.Major, pluginVersion.Minor, pluginVersion.Build);
        }

        public string GameVersion
        {
            get
            {
                return this.comboBoxGameVersion.Text;
            }
        }

        public float AspectRatio
        {
            get
            {
                if (this.checkBoxAutoDetectDisplay.Checked)
                {
                    return DetectedDisplays.AspectRatio;
                }
                else
                {
                    int correctedX;
                    int correctedY;

                    bool successfulX = int.TryParse(this.correctedX, out correctedX);
                    bool successfulY = int.TryParse(this.correctedY, out correctedY);

                    if (successfulX && successfulY)
                    {
                        return (float)correctedX / correctedY;
                    }
                    else
                    {
                        return 1.77777777f;
                    }
                }
            }
        }

        public float HudLeft
        {
            get
            {
                if (this.checkBoxAutoDetectDisplay.Checked)
                {
                    return (float)DetectedDisplays.HudLeft;
                }
                else
                {
                    if (int.Parse(this.displayCount, CultureInfo.InvariantCulture) < 3)
                    {
                        return 0.0f;
                    }
                    else
                    {
                        float displayCount = float.Parse(this.displayCount, CultureInfo.InvariantCulture);

                        float standardX = float.Parse(this.uncorrectedX, CultureInfo.InvariantCulture);
                        float correctedX = float.Parse(this.correctedX, CultureInfo.InvariantCulture);

                        float bezelOffset = (correctedX - standardX) / 2.0f;

                        float dividedXResolution = standardX / displayCount;

                        return dividedXResolution + bezelOffset;
                    }
                }
            }
        }

        public float HudRight
        {
            get
            {
                if (this.checkBoxAutoDetectDisplay.Checked)
                {
                    return (float)DetectedDisplays.HudRight;
                }
                else
                {
                    if (int.Parse(this.displayCount, CultureInfo.InvariantCulture) < 3)
                    {
                        return 0.0f;
                    }
                    else
                    {
                        float displayCount = float.Parse(this.displayCount, CultureInfo.InvariantCulture);

                        float standardX = float.Parse(this.uncorrectedX, CultureInfo.InvariantCulture);
                        float correctedX = float.Parse(this.correctedX, CultureInfo.InvariantCulture);

                        float bezelOffset = (correctedX - standardX) / 2.0f;

                        float dividedXResolution = standardX / displayCount;

                        return dividedXResolution + bezelOffset + (standardX / displayCount);
                    }
                }
            }
        }

        public float HudWidth
        {
            get
            {
                if (this.checkBoxAutoDetectDisplay.Checked)
                {
                    return (float)DetectedDisplays.HudWidth;
                }
                else
                {
                    if (int.Parse(this.displayCount, CultureInfo.InvariantCulture) < 3)
                    {
                        return float.Parse(this.uncorrectedX, CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        float displayCount = float.Parse(this.displayCount, CultureInfo.InvariantCulture);

                        float hudWidth = float.Parse(this.uncorrectedX, CultureInfo.InvariantCulture) / displayCount;

                        return hudWidth;
                    }
                }
            }
        }

        public float HorizontalResolution
        {
            get
            {
                if (this.checkBoxAutoDetectDisplay.Checked)
                {
                    return (float)DetectedDisplays.HorizontalResolution;
                }
                else
                {
                    return float.Parse(this.correctedX, CultureInfo.InvariantCulture);
                }
            }
        }

        public float VerticalResolution
        {
            get
            {
                if (this.checkBoxAutoDetectDisplay.Checked)
                {
                    return (float)DetectedDisplays.VerticalResolution;
                }
                else
                {
                    return float.Parse(this.correctedY, CultureInfo.InvariantCulture);
                }
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
                Properties.Settings.Default.GameVersion = this.comboBoxGameVersion.Text;
            }
            catch
            {
                Properties.Settings.Default.GameVersion = string.Empty;

                throw;
            }

            // Auto Detection
            try
            {
                Properties.Settings.Default.AutoDetection = this.checkBoxAutoDetectDisplay.Checked;
            }
            catch
            {
                Properties.Settings.Default.AutoDetection = true;

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
                Properties.Settings.Default.UncorrectedX = Convert.ToInt32(this.numericTextBoxNormalX.Text, CultureInfo.InvariantCulture);
            }
            catch
            {
                Properties.Settings.Default.UncorrectedX = 5760;

                throw;
            }

            // Standard Y
            try
            {
                Properties.Settings.Default.UncorrectedY = Convert.ToInt32(this.numericTextBoxNormalY.Text, CultureInfo.InvariantCulture);
            }
            catch
            {
                Properties.Settings.Default.UncorrectedY = 1080;

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

        private void NumericTextBoxDisplayCount_TextChanged(object sender, EventArgs e)
        {
            this.displayCount = this.numericTextBoxDisplayCount.Text;
        }

        private void NumericTextBoxNormalX_TextChanged(object sender, EventArgs e)
        {
            this.uncorrectedX = this.numericTextBoxNormalX.Text;
        }

        private void NumericTextBoxNormalY_TextChanged(object sender, EventArgs e)
        {
            this.uncorrectedY = this.numericTextBoxNormalY.Text;
        }

        private void NumericTextBoxCorrectedX_TextChanged(object sender, EventArgs e)
        {
            this.correctedX = this.numericTextBoxCorrectedX.Text;
        }

        private void NumericTextBoxCorrectedY_TextChanged(object sender, EventArgs e)
        {
            this.correctedY = this.numericTextBoxCorrectedY.Text;
        }
    }
}
