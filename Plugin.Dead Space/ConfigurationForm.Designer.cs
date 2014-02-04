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

namespace Plugin.DeadSpace
{
    public partial class ConfigurationForm
    {
        private System.Windows.Forms.GroupBox groupBoxVersion;
        private Library.Forms.ComboBoxNoFocus comboBoxGameVersion;
        private System.Windows.Forms.GroupBox groupBoxFov;
        private System.Windows.Forms.Label labelHigh;
        private System.Windows.Forms.Label labelTrackBarValue;
        private System.Windows.Forms.Label labelLow;
        private Library.Forms.TrackBarNoFocus trackBarFieldOfView;
        private Library.Forms.ButtonNoFocus buttonSaveSettings;
        private Library.Forms.ButtonNoFocus buttonCloseWindow;
        private System.Windows.Forms.GroupBox groupBoxPlugin;
        private System.Windows.Forms.Label labelDescription;
        private System.Windows.Forms.LinkLabel linkLabelWSGFDetailedReport;
        private System.Windows.Forms.Label labelPluginVersion;
        private System.Windows.Forms.Label labelTitlePluginVersion;
        private System.Windows.Forms.Label labelTitleDescription;
        private System.Windows.Forms.LinkLabel linkLabelDonate;
        private System.Windows.Forms.Label labelTitleDonate;
        private System.Windows.Forms.LinkLabel linkLabelWebsite;
        private System.Windows.Forms.Label labelTitleWebsite;
        private System.Windows.Forms.Label labelAuthor;
        private System.Windows.Forms.Label labelTitleAuthor;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBoxVersion = new System.Windows.Forms.GroupBox();
            this.comboBoxGameVersion = new Library.Forms.ComboBoxNoFocus();
            this.groupBoxFov = new System.Windows.Forms.GroupBox();
            this.labelHigh = new System.Windows.Forms.Label();
            this.labelTrackBarValue = new System.Windows.Forms.Label();
            this.labelLow = new System.Windows.Forms.Label();
            this.trackBarFieldOfView = new Library.Forms.TrackBarNoFocus();
            this.buttonSaveSettings = new Library.Forms.ButtonNoFocus();
            this.buttonCloseWindow = new Library.Forms.ButtonNoFocus();
            this.groupBoxPlugin = new System.Windows.Forms.GroupBox();
            this.labelDescription = new System.Windows.Forms.Label();
            this.linkLabelWSGFDetailedReport = new System.Windows.Forms.LinkLabel();
            this.labelPluginVersion = new System.Windows.Forms.Label();
            this.labelTitlePluginVersion = new System.Windows.Forms.Label();
            this.labelTitleDescription = new System.Windows.Forms.Label();
            this.linkLabelDonate = new System.Windows.Forms.LinkLabel();
            this.labelTitleDonate = new System.Windows.Forms.Label();
            this.linkLabelWebsite = new System.Windows.Forms.LinkLabel();
            this.labelTitleWebsite = new System.Windows.Forms.Label();
            this.labelAuthor = new System.Windows.Forms.Label();
            this.labelTitleAuthor = new System.Windows.Forms.Label();
            this.groupBoxVersion.SuspendLayout();
            this.groupBoxFov.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarFieldOfView)).BeginInit();
            this.groupBoxPlugin.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxVersion
            // 
            this.groupBoxVersion.Controls.Add(this.comboBoxGameVersion);
            this.groupBoxVersion.Location = new System.Drawing.Point(12, 12);
            this.groupBoxVersion.Name = "groupBoxVersion";
            this.groupBoxVersion.Size = new System.Drawing.Size(270, 52);
            this.groupBoxVersion.TabIndex = 0;
            this.groupBoxVersion.TabStop = false;
            this.groupBoxVersion.Text = "Select Game Version";
            // 
            // comboBoxGameVersion
            // 
            this.comboBoxGameVersion.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.comboBoxGameVersion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxGameVersion.FormattingEnabled = true;
            this.comboBoxGameVersion.Location = new System.Drawing.Point(6, 19);
            this.comboBoxGameVersion.Name = "comboBoxGameVersion";
            this.comboBoxGameVersion.Size = new System.Drawing.Size(258, 21);
            this.comboBoxGameVersion.TabIndex = 0;
            // 
            // groupBoxFov
            // 
            this.groupBoxFov.Controls.Add(this.labelHigh);
            this.groupBoxFov.Controls.Add(this.labelTrackBarValue);
            this.groupBoxFov.Controls.Add(this.labelLow);
            this.groupBoxFov.Controls.Add(this.trackBarFieldOfView);
            this.groupBoxFov.Location = new System.Drawing.Point(12, 70);
            this.groupBoxFov.Name = "groupBoxFov";
            this.groupBoxFov.Size = new System.Drawing.Size(270, 77);
            this.groupBoxFov.TabIndex = 1;
            this.groupBoxFov.TabStop = false;
            this.groupBoxFov.Text = "Add to Field-of-View";
            // 
            // labelHigh
            // 
            this.labelHigh.AutoSize = true;
            this.labelHigh.Location = new System.Drawing.Point(245, 51);
            this.labelHigh.Name = "labelHigh";
            this.labelHigh.Size = new System.Drawing.Size(13, 13);
            this.labelHigh.TabIndex = 3;
            this.labelHigh.Text = "1";
            // 
            // labelTrackBarValue
            // 
            this.labelTrackBarValue.Location = new System.Drawing.Point(45, 51);
            this.labelTrackBarValue.Name = "labelTrackBarValue";
            this.labelTrackBarValue.Size = new System.Drawing.Size(180, 13);
            this.labelTrackBarValue.TabIndex = 2;
            this.labelTrackBarValue.Text = "0";
            this.labelTrackBarValue.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // labelLow
            // 
            this.labelLow.AutoSize = true;
            this.labelLow.Location = new System.Drawing.Point(13, 51);
            this.labelLow.Name = "labelLow";
            this.labelLow.Size = new System.Drawing.Size(13, 13);
            this.labelLow.TabIndex = 1;
            this.labelLow.Text = "0";
            // 
            // trackBarFieldOfView
            // 
            this.trackBarFieldOfView.Location = new System.Drawing.Point(6, 19);
            this.trackBarFieldOfView.Maximum = 100;
            this.trackBarFieldOfView.Name = "trackBarFieldOfView";
            this.trackBarFieldOfView.Size = new System.Drawing.Size(258, 45);
            this.trackBarFieldOfView.TabIndex = 0;
            this.trackBarFieldOfView.TickFrequency = 5;
            this.trackBarFieldOfView.ValueChanged += new System.EventHandler(this.TrackBarFieldOfView_ValueChanged);
            // 
            // buttonSaveSettings
            // 
            this.buttonSaveSettings.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonSaveSettings.Location = new System.Drawing.Point(155, 237);
            this.buttonSaveSettings.Name = "buttonSaveSettings";
            this.buttonSaveSettings.Size = new System.Drawing.Size(90, 23);
            this.buttonSaveSettings.TabIndex = 2;
            this.buttonSaveSettings.Text = "Save Settings";
            this.buttonSaveSettings.UseVisualStyleBackColor = true;
            this.buttonSaveSettings.Click += new System.EventHandler(this.ButtonSaveSettings_Click);
            // 
            // buttonCloseWindow
            // 
            this.buttonCloseWindow.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonCloseWindow.Location = new System.Drawing.Point(255, 237);
            this.buttonCloseWindow.Name = "buttonCloseWindow";
            this.buttonCloseWindow.Size = new System.Drawing.Size(90, 23);
            this.buttonCloseWindow.TabIndex = 3;
            this.buttonCloseWindow.Text = "Close Window";
            this.buttonCloseWindow.UseVisualStyleBackColor = true;
            this.buttonCloseWindow.Click += new System.EventHandler(this.ButtonCloseWindow_Click);
            // 
            // groupBoxPlugin
            // 
            this.groupBoxPlugin.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxPlugin.Controls.Add(this.labelDescription);
            this.groupBoxPlugin.Controls.Add(this.linkLabelWSGFDetailedReport);
            this.groupBoxPlugin.Controls.Add(this.labelPluginVersion);
            this.groupBoxPlugin.Controls.Add(this.labelTitlePluginVersion);
            this.groupBoxPlugin.Controls.Add(this.labelTitleDescription);
            this.groupBoxPlugin.Controls.Add(this.linkLabelDonate);
            this.groupBoxPlugin.Controls.Add(this.labelTitleDonate);
            this.groupBoxPlugin.Controls.Add(this.linkLabelWebsite);
            this.groupBoxPlugin.Controls.Add(this.labelTitleWebsite);
            this.groupBoxPlugin.Controls.Add(this.labelAuthor);
            this.groupBoxPlugin.Controls.Add(this.labelTitleAuthor);
            this.groupBoxPlugin.Location = new System.Drawing.Point(288, 12);
            this.groupBoxPlugin.MinimumSize = new System.Drawing.Size(200, 190);
            this.groupBoxPlugin.Name = "groupBoxPlugin";
            this.groupBoxPlugin.Size = new System.Drawing.Size(200, 219);
            this.groupBoxPlugin.TabIndex = 13;
            this.groupBoxPlugin.TabStop = false;
            this.groupBoxPlugin.Text = "Plugin Information";
            // 
            // labelDescription
            // 
            this.labelDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelDescription.Location = new System.Drawing.Point(6, 73);
            this.labelDescription.Name = "labelDescription";
            this.labelDescription.Size = new System.Drawing.Size(188, 104);
            this.labelDescription.TabIndex = 12;
            this.labelDescription.Text = "N/A";
            // 
            // linkLabelWSGFDetailedReport
            // 
            this.linkLabelWSGFDetailedReport.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.linkLabelWSGFDetailedReport.AutoSize = true;
            this.linkLabelWSGFDetailedReport.Location = new System.Drawing.Point(6, 177);
            this.linkLabelWSGFDetailedReport.Name = "linkLabelWSGFDetailedReport";
            this.linkLabelWSGFDetailedReport.Size = new System.Drawing.Size(166, 13);
            this.linkLabelWSGFDetailedReport.TabIndex = 11;
            this.linkLabelWSGFDetailedReport.TabStop = true;
            this.linkLabelWSGFDetailedReport.Text = "Click to Read the Detailed Report";
            this.linkLabelWSGFDetailedReport.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabelWSGFDetailedReport_LinkClicked);
            // 
            // labelPluginVersion
            // 
            this.labelPluginVersion.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.labelPluginVersion.AutoSize = true;
            this.labelPluginVersion.Location = new System.Drawing.Point(79, 201);
            this.labelPluginVersion.Name = "labelPluginVersion";
            this.labelPluginVersion.Size = new System.Drawing.Size(27, 13);
            this.labelPluginVersion.TabIndex = 10;
            this.labelPluginVersion.Text = "N/A";
            // 
            // labelTitlePluginVersion
            // 
            this.labelTitlePluginVersion.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.labelTitlePluginVersion.AutoSize = true;
            this.labelTitlePluginVersion.Location = new System.Drawing.Point(6, 201);
            this.labelTitlePluginVersion.Name = "labelTitlePluginVersion";
            this.labelTitlePluginVersion.Size = new System.Drawing.Size(77, 13);
            this.labelTitlePluginVersion.TabIndex = 9;
            this.labelTitlePluginVersion.Text = "Plugin Version:";
            // 
            // labelTitleDescription
            // 
            this.labelTitleDescription.AutoSize = true;
            this.labelTitleDescription.Location = new System.Drawing.Point(6, 60);
            this.labelTitleDescription.Name = "labelTitleDescription";
            this.labelTitleDescription.Size = new System.Drawing.Size(63, 13);
            this.labelTitleDescription.TabIndex = 8;
            this.labelTitleDescription.Text = "Description:";
            // 
            // linkLabelDonate
            // 
            this.linkLabelDonate.AutoSize = true;
            this.linkLabelDonate.Location = new System.Drawing.Point(79, 42);
            this.linkLabelDonate.Name = "linkLabelDonate";
            this.linkLabelDonate.Size = new System.Drawing.Size(27, 13);
            this.linkLabelDonate.TabIndex = 7;
            this.linkLabelDonate.TabStop = true;
            this.linkLabelDonate.Text = "N/A";
            this.linkLabelDonate.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabelDonate_LinkClicked);
            // 
            // labelTitleDonate
            // 
            this.labelTitleDonate.AutoSize = true;
            this.labelTitleDonate.Location = new System.Drawing.Point(6, 42);
            this.labelTitleDonate.Name = "labelTitleDonate";
            this.labelTitleDonate.Size = new System.Drawing.Size(45, 13);
            this.labelTitleDonate.TabIndex = 6;
            this.labelTitleDonate.Text = "Donate:";
            // 
            // linkLabelWebsite
            // 
            this.linkLabelWebsite.AutoSize = true;
            this.linkLabelWebsite.Location = new System.Drawing.Point(79, 29);
            this.linkLabelWebsite.Name = "linkLabelWebsite";
            this.linkLabelWebsite.Size = new System.Drawing.Size(27, 13);
            this.linkLabelWebsite.TabIndex = 5;
            this.linkLabelWebsite.TabStop = true;
            this.linkLabelWebsite.Text = "N/A";
            this.linkLabelWebsite.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabelWebsite_LinkClicked);
            // 
            // labelTitleWebsite
            // 
            this.labelTitleWebsite.AutoSize = true;
            this.labelTitleWebsite.Location = new System.Drawing.Point(6, 29);
            this.labelTitleWebsite.Name = "labelTitleWebsite";
            this.labelTitleWebsite.Size = new System.Drawing.Size(49, 13);
            this.labelTitleWebsite.TabIndex = 4;
            this.labelTitleWebsite.Text = "Website:";
            // 
            // labelAuthor
            // 
            this.labelAuthor.AutoSize = true;
            this.labelAuthor.Location = new System.Drawing.Point(79, 16);
            this.labelAuthor.Name = "labelAuthor";
            this.labelAuthor.Size = new System.Drawing.Size(27, 13);
            this.labelAuthor.TabIndex = 1;
            this.labelAuthor.Text = "N/A";
            // 
            // labelTitleAuthor
            // 
            this.labelTitleAuthor.AutoSize = true;
            this.labelTitleAuthor.Location = new System.Drawing.Point(6, 16);
            this.labelTitleAuthor.Name = "labelTitleAuthor";
            this.labelTitleAuthor.Size = new System.Drawing.Size(41, 13);
            this.labelTitleAuthor.TabIndex = 0;
            this.labelTitleAuthor.Text = "Author:";
            // 
            // Configuration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(494, 266);
            this.Controls.Add(this.groupBoxPlugin);
            this.Controls.Add(this.buttonCloseWindow);
            this.Controls.Add(this.buttonSaveSettings);
            this.Controls.Add(this.groupBoxFov);
            this.Controls.Add(this.groupBoxVersion);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Configuration";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configuration";
            this.groupBoxVersion.ResumeLayout(false);
            this.groupBoxFov.ResumeLayout(false);
            this.groupBoxFov.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarFieldOfView)).EndInit();
            this.groupBoxPlugin.ResumeLayout(false);
            this.groupBoxPlugin.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion
    }
}