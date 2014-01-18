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
    public partial class WidescreenFixerApp
    {
        private Library.Forms.TabControlNoFocus tabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.GroupBox groupBoxStatus;
        private System.Windows.Forms.Label labelValue1;
        private System.Windows.Forms.Label labelFixEnabledValue;
        private System.Windows.Forms.Label labelGameRunningValue;
        private System.Windows.Forms.Label labelTitleValue1;
        private System.Windows.Forms.Label labelFixEnabled;
        private System.Windows.Forms.Label labelGameRunning;
        private System.Windows.Forms.GroupBox groupBoxOptions;
        private Library.Forms.ComboBoxNoFocus comboBoxGameSelection;
        private Library.Forms.ButtonBorderless buttonDonate;
        private System.Windows.Forms.LinkLabel linkLabelHomePage;
        private System.Windows.Forms.Label labelCopyright;
        private System.Windows.Forms.Label labelWidescreenFixer;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.Timer timerValues;
        private System.Windows.Forms.Timer timerHotkey;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemRestore;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemExit;
        private System.Windows.Forms.Label labelLicenseText;
        private Library.Forms.CheckBoxNoFocus checkBoxWindowState;
        private Library.Forms.CheckBoxNoFocus checkBoxUpdate;
        private System.Windows.Forms.GroupBox groupBoxHotkey;
        private System.Windows.Forms.TextBox textBoxHotkey;
        private System.Windows.Forms.Label labelDonateThanks;
        private Library.Forms.CheckBoxNoFocus checkBoxKeyAlt;
        private Library.Forms.CheckBoxNoFocus checkBoxKeyShift;
        private Library.Forms.CheckBoxNoFocus checkBoxKeyControl;
        private System.Windows.Forms.LinkLabel linkLabelWidescreenForum;
        private System.Windows.Forms.Label labelValue3;
        private System.Windows.Forms.Label labelTitleValue3;
        private System.Windows.Forms.Label labelValue2;
        private System.Windows.Forms.Label labelTitleValue2;
        private Library.Forms.ButtonBorderless buttonConfigure;
        private Library.Forms.ButtonBorderless buttonDisplayInfo;
        private System.Windows.Forms.Label labelTitleValue4;
        private System.Windows.Forms.Label labelTitleValue5;
        private System.Windows.Forms.Label labelValue5;
        private System.Windows.Forms.Label labelValue4;
        private System.Windows.Forms.Timer timerSetup;
        private Library.Forms.CheckBoxNoFocus checkBoxDisplayObsoletePlugins;

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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WidescreenFixerApp));
            this.tabControl = new Library.Forms.TabControlNoFocus();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBoxStatus = new System.Windows.Forms.GroupBox();
            this.labelValue5 = new System.Windows.Forms.Label();
            this.labelValue4 = new System.Windows.Forms.Label();
            this.labelTitleValue5 = new System.Windows.Forms.Label();
            this.labelTitleValue4 = new System.Windows.Forms.Label();
            this.buttonDisplayInfo = new Library.Forms.ButtonBorderless();
            this.labelValue3 = new System.Windows.Forms.Label();
            this.labelTitleValue3 = new System.Windows.Forms.Label();
            this.labelValue2 = new System.Windows.Forms.Label();
            this.labelTitleValue2 = new System.Windows.Forms.Label();
            this.labelValue1 = new System.Windows.Forms.Label();
            this.labelFixEnabledValue = new System.Windows.Forms.Label();
            this.labelGameRunningValue = new System.Windows.Forms.Label();
            this.labelTitleValue1 = new System.Windows.Forms.Label();
            this.labelFixEnabled = new System.Windows.Forms.Label();
            this.labelGameRunning = new System.Windows.Forms.Label();
            this.groupBoxOptions = new System.Windows.Forms.GroupBox();
            this.buttonConfigure = new Library.Forms.ButtonBorderless();
            this.comboBoxGameSelection = new Library.Forms.ComboBoxNoFocus();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.checkBoxDisplayObsoletePlugins = new Library.Forms.CheckBoxNoFocus();
            this.groupBoxHotkey = new System.Windows.Forms.GroupBox();
            this.checkBoxKeyAlt = new Library.Forms.CheckBoxNoFocus();
            this.checkBoxKeyShift = new Library.Forms.CheckBoxNoFocus();
            this.checkBoxKeyControl = new Library.Forms.CheckBoxNoFocus();
            this.textBoxHotkey = new System.Windows.Forms.TextBox();
            this.checkBoxWindowState = new Library.Forms.CheckBoxNoFocus();
            this.checkBoxUpdate = new Library.Forms.CheckBoxNoFocus();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.linkLabelWidescreenForum = new System.Windows.Forms.LinkLabel();
            this.labelDonateThanks = new System.Windows.Forms.Label();
            this.labelLicenseText = new System.Windows.Forms.Label();
            this.linkLabelHomePage = new System.Windows.Forms.LinkLabel();
            this.labelCopyright = new System.Windows.Forms.Label();
            this.labelWidescreenFixer = new System.Windows.Forms.Label();
            this.buttonDonate = new Library.Forms.ButtonBorderless();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemRestore = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemExit = new System.Windows.Forms.ToolStripMenuItem();
            this.timerValues = new System.Windows.Forms.Timer(this.components);
            this.timerHotkey = new System.Windows.Forms.Timer(this.components);
            this.timerSetup = new System.Windows.Forms.Timer(this.components);
            this.tabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBoxStatus.SuspendLayout();
            this.groupBoxOptions.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBoxHotkey.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.tabPage1);
            this.tabControl.Controls.Add(this.tabPage2);
            this.tabControl.Controls.Add(this.tabPage3);
            this.tabControl.Location = new System.Drawing.Point(12, 12);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(304, 211);
            this.tabControl.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBoxStatus);
            this.tabPage1.Controls.Add(this.groupBoxOptions);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(296, 185);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Main";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBoxStatus
            // 
            this.groupBoxStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxStatus.Controls.Add(this.labelValue5);
            this.groupBoxStatus.Controls.Add(this.labelValue4);
            this.groupBoxStatus.Controls.Add(this.labelTitleValue5);
            this.groupBoxStatus.Controls.Add(this.labelTitleValue4);
            this.groupBoxStatus.Controls.Add(this.buttonDisplayInfo);
            this.groupBoxStatus.Controls.Add(this.labelValue3);
            this.groupBoxStatus.Controls.Add(this.labelTitleValue3);
            this.groupBoxStatus.Controls.Add(this.labelValue2);
            this.groupBoxStatus.Controls.Add(this.labelTitleValue2);
            this.groupBoxStatus.Controls.Add(this.labelValue1);
            this.groupBoxStatus.Controls.Add(this.labelFixEnabledValue);
            this.groupBoxStatus.Controls.Add(this.labelGameRunningValue);
            this.groupBoxStatus.Controls.Add(this.labelTitleValue1);
            this.groupBoxStatus.Controls.Add(this.labelFixEnabled);
            this.groupBoxStatus.Controls.Add(this.labelGameRunning);
            this.groupBoxStatus.Location = new System.Drawing.Point(7, 66);
            this.groupBoxStatus.Name = "groupBoxStatus";
            this.groupBoxStatus.Size = new System.Drawing.Size(283, 113);
            this.groupBoxStatus.TabIndex = 1;
            this.groupBoxStatus.TabStop = false;
            this.groupBoxStatus.Text = "Status";
            // 
            // labelValue5
            // 
            this.labelValue5.AutoSize = true;
            this.labelValue5.Location = new System.Drawing.Point(155, 94);
            this.labelValue5.Name = "labelValue5";
            this.labelValue5.Size = new System.Drawing.Size(0, 13);
            this.labelValue5.TabIndex = 13;
            // 
            // labelValue4
            // 
            this.labelValue4.AutoSize = true;
            this.labelValue4.Location = new System.Drawing.Point(155, 81);
            this.labelValue4.Name = "labelValue4";
            this.labelValue4.Size = new System.Drawing.Size(0, 13);
            this.labelValue4.TabIndex = 11;
            // 
            // labelTitleValue5
            // 
            this.labelTitleValue5.AutoSize = true;
            this.labelTitleValue5.Location = new System.Drawing.Point(6, 94);
            this.labelTitleValue5.Name = "labelTitleValue5";
            this.labelTitleValue5.Size = new System.Drawing.Size(0, 13);
            this.labelTitleValue5.TabIndex = 12;
            // 
            // labelTitleValue4
            // 
            this.labelTitleValue4.AutoSize = true;
            this.labelTitleValue4.Location = new System.Drawing.Point(6, 81);
            this.labelTitleValue4.Name = "labelTitleValue4";
            this.labelTitleValue4.Size = new System.Drawing.Size(0, 13);
            this.labelTitleValue4.TabIndex = 10;
            // 
            // buttonDisplayInfo
            // 
            this.buttonDisplayInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDisplayInfo.BackColor = System.Drawing.Color.WhiteSmoke;
            this.buttonDisplayInfo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonDisplayInfo.FlatAppearance.BorderSize = 0;
            this.buttonDisplayInfo.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.buttonDisplayInfo.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.buttonDisplayInfo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonDisplayInfo.Location = new System.Drawing.Point(262, 92);
            this.buttonDisplayInfo.Name = "buttonDisplayInfo";
            this.buttonDisplayInfo.Size = new System.Drawing.Size(15, 15);
            this.buttonDisplayInfo.TabIndex = 14;
            this.buttonDisplayInfo.UseVisualStyleBackColor = false;
            this.buttonDisplayInfo.Click += new System.EventHandler(this.ButtonDisplayInfo_Click);
            // 
            // labelValue3
            // 
            this.labelValue3.AutoSize = true;
            this.labelValue3.Location = new System.Drawing.Point(155, 67);
            this.labelValue3.Name = "labelValue3";
            this.labelValue3.Size = new System.Drawing.Size(0, 13);
            this.labelValue3.TabIndex = 9;
            // 
            // labelTitleValue3
            // 
            this.labelTitleValue3.AutoSize = true;
            this.labelTitleValue3.Location = new System.Drawing.Point(6, 68);
            this.labelTitleValue3.Name = "labelTitleValue3";
            this.labelTitleValue3.Size = new System.Drawing.Size(0, 13);
            this.labelTitleValue3.TabIndex = 8;
            // 
            // labelValue2
            // 
            this.labelValue2.AutoSize = true;
            this.labelValue2.Location = new System.Drawing.Point(155, 54);
            this.labelValue2.Name = "labelValue2";
            this.labelValue2.Size = new System.Drawing.Size(0, 13);
            this.labelValue2.TabIndex = 7;
            // 
            // labelTitleValue2
            // 
            this.labelTitleValue2.AutoSize = true;
            this.labelTitleValue2.Location = new System.Drawing.Point(6, 55);
            this.labelTitleValue2.Name = "labelTitleValue2";
            this.labelTitleValue2.Size = new System.Drawing.Size(0, 13);
            this.labelTitleValue2.TabIndex = 6;
            // 
            // labelValue1
            // 
            this.labelValue1.AutoSize = true;
            this.labelValue1.Location = new System.Drawing.Point(155, 41);
            this.labelValue1.Name = "labelValue1";
            this.labelValue1.Size = new System.Drawing.Size(0, 13);
            this.labelValue1.TabIndex = 5;
            // 
            // labelFixEnabledValue
            // 
            this.labelFixEnabledValue.AutoSize = true;
            this.labelFixEnabledValue.Location = new System.Drawing.Point(155, 28);
            this.labelFixEnabledValue.Name = "labelFixEnabledValue";
            this.labelFixEnabledValue.Size = new System.Drawing.Size(21, 13);
            this.labelFixEnabledValue.TabIndex = 3;
            this.labelFixEnabledValue.Text = "No";
            // 
            // labelGameRunningValue
            // 
            this.labelGameRunningValue.AutoSize = true;
            this.labelGameRunningValue.Location = new System.Drawing.Point(155, 15);
            this.labelGameRunningValue.Name = "labelGameRunningValue";
            this.labelGameRunningValue.Size = new System.Drawing.Size(21, 13);
            this.labelGameRunningValue.TabIndex = 1;
            this.labelGameRunningValue.Text = "No";
            // 
            // labelTitleValue1
            // 
            this.labelTitleValue1.AutoSize = true;
            this.labelTitleValue1.Location = new System.Drawing.Point(6, 42);
            this.labelTitleValue1.Name = "labelTitleValue1";
            this.labelTitleValue1.Size = new System.Drawing.Size(0, 13);
            this.labelTitleValue1.TabIndex = 4;
            // 
            // labelFixEnabled
            // 
            this.labelFixEnabled.AutoSize = true;
            this.labelFixEnabled.Location = new System.Drawing.Point(6, 29);
            this.labelFixEnabled.Name = "labelFixEnabled";
            this.labelFixEnabled.Size = new System.Drawing.Size(65, 13);
            this.labelFixEnabled.TabIndex = 2;
            this.labelFixEnabled.Text = "Fix Enabled:";
            // 
            // labelGameRunning
            // 
            this.labelGameRunning.AutoSize = true;
            this.labelGameRunning.Location = new System.Drawing.Point(6, 16);
            this.labelGameRunning.Name = "labelGameRunning";
            this.labelGameRunning.Size = new System.Drawing.Size(81, 13);
            this.labelGameRunning.TabIndex = 0;
            this.labelGameRunning.Text = "Game Running:";
            // 
            // groupBoxOptions
            // 
            this.groupBoxOptions.Controls.Add(this.buttonConfigure);
            this.groupBoxOptions.Controls.Add(this.comboBoxGameSelection);
            this.groupBoxOptions.Location = new System.Drawing.Point(7, 7);
            this.groupBoxOptions.Name = "groupBoxOptions";
            this.groupBoxOptions.Size = new System.Drawing.Size(283, 53);
            this.groupBoxOptions.TabIndex = 0;
            this.groupBoxOptions.TabStop = false;
            this.groupBoxOptions.Text = "Select Game";
            // 
            // buttonConfigure
            // 
            this.buttonConfigure.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.buttonConfigure.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonConfigure.FlatAppearance.BorderSize = 0;
            this.buttonConfigure.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.buttonConfigure.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.buttonConfigure.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonConfigure.Image = global::WidescreenFixer.Properties.Resources.Configure;
            this.buttonConfigure.Location = new System.Drawing.Point(239, 10);
            this.buttonConfigure.Name = "buttonConfigure";
            this.buttonConfigure.Size = new System.Drawing.Size(38, 37);
            this.buttonConfigure.TabIndex = 1;
            this.buttonConfigure.UseVisualStyleBackColor = true;
            this.buttonConfigure.Click += new System.EventHandler(this.ButtonConfigure_Click);
            // 
            // comboBoxGameSelection
            // 
            this.comboBoxGameSelection.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.comboBoxGameSelection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxGameSelection.FormattingEnabled = true;
            this.comboBoxGameSelection.Location = new System.Drawing.Point(6, 19);
            this.comboBoxGameSelection.Name = "comboBoxGameSelection";
            this.comboBoxGameSelection.Size = new System.Drawing.Size(227, 21);
            this.comboBoxGameSelection.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.checkBoxDisplayObsoletePlugins);
            this.tabPage2.Controls.Add(this.groupBoxHotkey);
            this.tabPage2.Controls.Add(this.checkBoxWindowState);
            this.tabPage2.Controls.Add(this.checkBoxUpdate);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(296, 185);
            this.tabPage2.TabIndex = 2;
            this.tabPage2.Text = "Settings";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // checkBoxDisplayObsoletePlugins
            // 
            this.checkBoxDisplayObsoletePlugins.AutoSize = true;
            this.checkBoxDisplayObsoletePlugins.Enabled = false;
            this.checkBoxDisplayObsoletePlugins.Location = new System.Drawing.Point(6, 80);
            this.checkBoxDisplayObsoletePlugins.Name = "checkBoxDisplayObsoletePlugins";
            this.checkBoxDisplayObsoletePlugins.Size = new System.Drawing.Size(139, 17);
            this.checkBoxDisplayObsoletePlugins.TabIndex = 4;
            this.checkBoxDisplayObsoletePlugins.Text = "Display obsolete plugins";
            this.checkBoxDisplayObsoletePlugins.UseVisualStyleBackColor = true;
            // 
            // groupBoxHotkey
            // 
            this.groupBoxHotkey.Controls.Add(this.checkBoxKeyAlt);
            this.groupBoxHotkey.Controls.Add(this.checkBoxKeyShift);
            this.groupBoxHotkey.Controls.Add(this.checkBoxKeyControl);
            this.groupBoxHotkey.Controls.Add(this.textBoxHotkey);
            this.groupBoxHotkey.Location = new System.Drawing.Point(6, 132);
            this.groupBoxHotkey.Name = "groupBoxHotkey";
            this.groupBoxHotkey.Size = new System.Drawing.Size(284, 47);
            this.groupBoxHotkey.TabIndex = 3;
            this.groupBoxHotkey.TabStop = false;
            this.groupBoxHotkey.Text = "Hotkey";
            // 
            // checkBoxKeyAlt
            // 
            this.checkBoxKeyAlt.AutoSize = true;
            this.checkBoxKeyAlt.Location = new System.Drawing.Point(124, 19);
            this.checkBoxKeyAlt.Name = "checkBoxKeyAlt";
            this.checkBoxKeyAlt.Size = new System.Drawing.Size(38, 17);
            this.checkBoxKeyAlt.TabIndex = 4;
            this.checkBoxKeyAlt.Text = "Alt";
            this.checkBoxKeyAlt.UseVisualStyleBackColor = true;
            // 
            // checkBoxKeyShift
            // 
            this.checkBoxKeyShift.AutoSize = true;
            this.checkBoxKeyShift.Location = new System.Drawing.Point(71, 19);
            this.checkBoxKeyShift.Name = "checkBoxKeyShift";
            this.checkBoxKeyShift.Size = new System.Drawing.Size(47, 17);
            this.checkBoxKeyShift.TabIndex = 3;
            this.checkBoxKeyShift.Text = "Shift";
            this.checkBoxKeyShift.UseVisualStyleBackColor = true;
            // 
            // checkBoxKeyControl
            // 
            this.checkBoxKeyControl.AutoSize = true;
            this.checkBoxKeyControl.Location = new System.Drawing.Point(6, 19);
            this.checkBoxKeyControl.Name = "checkBoxKeyControl";
            this.checkBoxKeyControl.Size = new System.Drawing.Size(59, 17);
            this.checkBoxKeyControl.TabIndex = 2;
            this.checkBoxKeyControl.Text = "Control";
            this.checkBoxKeyControl.UseVisualStyleBackColor = true;
            // 
            // textBoxHotkey
            // 
            this.textBoxHotkey.Location = new System.Drawing.Point(168, 17);
            this.textBoxHotkey.Name = "textBoxHotkey";
            this.textBoxHotkey.Size = new System.Drawing.Size(110, 20);
            this.textBoxHotkey.TabIndex = 0;
            // 
            // checkBoxWindowState
            // 
            this.checkBoxWindowState.AutoSize = true;
            this.checkBoxWindowState.Checked = true;
            this.checkBoxWindowState.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxWindowState.Location = new System.Drawing.Point(6, 29);
            this.checkBoxWindowState.Name = "checkBoxWindowState";
            this.checkBoxWindowState.Size = new System.Drawing.Size(236, 17);
            this.checkBoxWindowState.TabIndex = 1;
            this.checkBoxWindowState.Text = "Remember window position and state on exit";
            this.checkBoxWindowState.UseVisualStyleBackColor = true;
            // 
            // checkBoxUpdate
            // 
            this.checkBoxUpdate.AutoSize = true;
            this.checkBoxUpdate.Checked = true;
            this.checkBoxUpdate.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxUpdate.Location = new System.Drawing.Point(6, 6);
            this.checkBoxUpdate.Name = "checkBoxUpdate";
            this.checkBoxUpdate.Size = new System.Drawing.Size(158, 17);
            this.checkBoxUpdate.TabIndex = 0;
            this.checkBoxUpdate.Text = "Check for update on launch";
            this.checkBoxUpdate.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.linkLabelWidescreenForum);
            this.tabPage3.Controls.Add(this.labelDonateThanks);
            this.tabPage3.Controls.Add(this.labelLicenseText);
            this.tabPage3.Controls.Add(this.linkLabelHomePage);
            this.tabPage3.Controls.Add(this.labelCopyright);
            this.tabPage3.Controls.Add(this.labelWidescreenFixer);
            this.tabPage3.Controls.Add(this.buttonDonate);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(296, 185);
            this.tabPage3.TabIndex = 1;
            this.tabPage3.Text = "About";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // linkLabelWidescreenForum
            // 
            this.linkLabelWidescreenForum.Location = new System.Drawing.Point(6, 87);
            this.linkLabelWidescreenForum.Name = "linkLabelWidescreenForum";
            this.linkLabelWidescreenForum.Size = new System.Drawing.Size(284, 13);
            this.linkLabelWidescreenForum.TabIndex = 6;
            this.linkLabelWidescreenForum.TabStop = true;
            this.linkLabelWidescreenForum.Text = "Widescreen Gaming Forum";
            this.linkLabelWidescreenForum.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.linkLabelWidescreenForum.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabelWidescreenForum_LinkClicked);
            // 
            // labelDonateThanks
            // 
            this.labelDonateThanks.Location = new System.Drawing.Point(6, 62);
            this.labelDonateThanks.Name = "labelDonateThanks";
            this.labelDonateThanks.Size = new System.Drawing.Size(284, 13);
            this.labelDonateThanks.TabIndex = 5;
            this.labelDonateThanks.Text = "Thanks to everyone who has donated!";
            this.labelDonateThanks.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelLicenseText
            // 
            this.labelLicenseText.Location = new System.Drawing.Point(6, 112);
            this.labelLicenseText.Name = "labelLicenseText";
            this.labelLicenseText.Size = new System.Drawing.Size(284, 26);
            this.labelLicenseText.TabIndex = 4;
            this.labelLicenseText.Text = "This software is distributed under the terms of the GNU General Public License.";
            this.labelLicenseText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // linkLabelHomePage
            // 
            this.linkLabelHomePage.Location = new System.Drawing.Point(6, 147);
            this.linkLabelHomePage.Name = "linkLabelHomePage";
            this.linkLabelHomePage.Size = new System.Drawing.Size(284, 13);
            this.linkLabelHomePage.TabIndex = 2;
            this.linkLabelHomePage.TabStop = true;
            this.linkLabelHomePage.Text = "Widescreen Fixer Home Page";
            this.linkLabelHomePage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.linkLabelHomePage.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabelHomePage_LinkClicked);
            // 
            // labelCopyright
            // 
            this.labelCopyright.Location = new System.Drawing.Point(6, 169);
            this.labelCopyright.Name = "labelCopyright";
            this.labelCopyright.Size = new System.Drawing.Size(284, 13);
            this.labelCopyright.TabIndex = 1;
            this.labelCopyright.Text = "Copyright © David Rudie 2007-2014. All rights reserved.";
            this.labelCopyright.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelWidescreenFixer
            // 
            this.labelWidescreenFixer.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.labelWidescreenFixer.Location = new System.Drawing.Point(6, 3);
            this.labelWidescreenFixer.Name = "labelWidescreenFixer";
            this.labelWidescreenFixer.Size = new System.Drawing.Size(284, 21);
            this.labelWidescreenFixer.TabIndex = 0;
            this.labelWidescreenFixer.Text = "Widescreen Fixer";
            this.labelWidescreenFixer.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonDonate
            // 
            this.buttonDonate.AutoSize = true;
            this.buttonDonate.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.buttonDonate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonDonate.FlatAppearance.BorderSize = 0;
            this.buttonDonate.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.buttonDonate.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.buttonDonate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonDonate.Image = global::WidescreenFixer.Properties.Resources.Donate;
            this.buttonDonate.Location = new System.Drawing.Point(98, 26);
            this.buttonDonate.Name = "buttonDonate";
            this.buttonDonate.Size = new System.Drawing.Size(98, 32);
            this.buttonDonate.TabIndex = 3;
            this.buttonDonate.UseVisualStyleBackColor = true;
            this.buttonDonate.Click += new System.EventHandler(this.ButtonDonate_Click);
            // 
            // notifyIcon
            // 
            this.notifyIcon.ContextMenuStrip = this.contextMenuStrip;
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "Widescreen Fixer";
            this.notifyIcon.Visible = true;
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemRestore,
            this.toolStripMenuItemExit});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(114, 48);
            // 
            // toolStripMenuItemRestore
            // 
            this.toolStripMenuItemRestore.Name = "toolStripMenuItemRestore";
            this.toolStripMenuItemRestore.Size = new System.Drawing.Size(113, 22);
            this.toolStripMenuItemRestore.Text = "Restore";
            this.toolStripMenuItemRestore.Click += new System.EventHandler(this.ToolStripMenuItemRestore_Click);
            // 
            // toolStripMenuItemExit
            // 
            this.toolStripMenuItemExit.Name = "toolStripMenuItemExit";
            this.toolStripMenuItemExit.Size = new System.Drawing.Size(113, 22);
            this.toolStripMenuItemExit.Text = "Exit";
            this.toolStripMenuItemExit.Click += new System.EventHandler(this.ToolStripMenuItemExit_Click);
            // 
            // timerValues
            // 
            this.timerValues.Enabled = true;
            // 
            // timerHotkey
            // 
            this.timerHotkey.Enabled = true;
            this.timerHotkey.Interval = 1;
            this.timerHotkey.Tick += new System.EventHandler(this.TimerHotkey_Tick);
            // 
            // timerSetup
            // 
            this.timerSetup.Enabled = true;
            this.timerSetup.Interval = 25;
            this.timerSetup.Tick += new System.EventHandler(this.TimerSetup_Tick);
            // 
            // WidescreenFixerApp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(328, 235);
            this.Controls.Add(this.tabControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "WidescreenFixerApp";
            this.Text = "Widescreen Fixer";
            this.tabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBoxStatus.ResumeLayout(false);
            this.groupBoxStatus.PerformLayout();
            this.groupBoxOptions.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.groupBoxHotkey.ResumeLayout(false);
            this.groupBoxHotkey.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion
    }
}