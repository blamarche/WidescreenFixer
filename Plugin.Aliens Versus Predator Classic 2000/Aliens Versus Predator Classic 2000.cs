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
    using System.Drawing;
    using System.Globalization;
    using System.Threading;
    using System.Windows.Forms;
    using Library.DetectDisplays;
    using Library.Plugin;
    using Library.Process;

    [FixerPluginAttribute("Aliens Versus Predator Classic 2000")]
    public sealed partial class AliensVersusPredatorClassic2000Game : BasePlugin, IFixer
    {
        #region Plugin-Specific Data

        private int hudLeft;
        private int hudRight;

        // alien
        // 0046BAD6 - 8B 2D 00007F03             - mov ebp,[037F0000] : [00000780]      shrinks health bar and moves to left
        // 0046BB91 - 8B 0D 00007F03             - mov ecx,[037F0000] : [00000780]      shrinks health bar and moves to left
        // 0046BBA0 - 8B 1D 00007F03             - mov ebx,[037F0000] : [00000780]      shrinks health bar and moves to left
        // 0046BBAD - 8B 1D 00007F03             - mov ebx,[037F0000] : [00000780]      shrinks health bar and moves to left
        // marine
        // 0046BC40 - 03 35 204A8700             - add esi,[00874A20] : [000017A8]      hud right
        // 0049D177 - A1 00007F03                - mov eax,[00874A20] : [000017A8]      round lights
        // predator
        private int offsetHudLeft;

        private int offsetHudRight;
        private byte[] pokeHudRightEnable;
        private byte[] pokeHudRightDisable;

        #endregion

        #region Constructor

        public AliensVersusPredatorClassic2000Game() : base()
        {
            this.GameName = "Aliens Versus Predator Classic 2000";
            this.ValueTitle1 = "Base Address";

            this.GameIcon = Properties.Resources.Icon;
            this.ConfigurationForm = new ConfigurationForm();

            // This data is the same no matter which version the user chooses
            this.ExeName = "AvP_Classic";
            this.WindowClass = "AvP";
            this.WindowTitle = "AvP";

            this.pokeHudRightEnable = new byte[] { 0xB8, 0x00, 0x00, 0x00, 0x00 };      // mov eax,value -- value gets written later for right-side hud placement
            this.pokeHudRightDisable = new byte[] { 0xA1, 0x20, 0x4A, 0x87, 0x00 };     // mov eax,[00874A20]
        }

        #endregion

        #region Overridden Methods

        public override void UpdateOffsets()
        {
            int gameVersion = Properties.Settings.Default.GameVersion;

            switch (gameVersion)
            {
                case 0:
                    this.offsetHudLeft = 0x92847;   // 0x92843 C7 44 24 1C 00 00 00 00  - mov [esp+1C],00000000
                    this.offsetHudRight = 0x928C3;  // 0x928C3 A1 20 4A 87 00           - mov eax,[00874A20]
                    break;

                default:
                    this.offsetHudLeft = 0x92847;
                    this.offsetHudRight = 0x928C3;
                    break;
            }
        }

        public override void UpdateValues()
        {
            base.UpdateValues();

            this.Value1 = this.BaseAddress > 0 ? string.Format(CultureInfo.InvariantCulture, "0x{0:X}", this.BaseAddress) : "N/A";
        }

        public override void Enable()
        {
            base.Enable();

            if (Properties.Settings.Default.AutoDetection == CheckState.Checked)
            {
                this.hudLeft = DetectedDisplays.HudLeft;
                this.hudRight = DetectedDisplays.HudRight;
            }
            else
            {
                int bezelOffset;
                int dividedXResolution;
                int newLeftHud;
                int newRightHud;

                int displayCount = Properties.Settings.Default.DisplayCount;
                int normalX = Properties.Settings.Default.NormalX;
                int correctedX = Properties.Settings.Default.CorrectedX;

                if (displayCount == 1 || displayCount == 2)
                {
                    this.hudLeft = 0;
                    this.hudRight = correctedX;
                }
                else
                {
                    bezelOffset = (correctedX - normalX) / 2;
                    dividedXResolution = normalX / displayCount;
                    newLeftHud = dividedXResolution + bezelOffset;
                    newRightHud = newLeftHud + (normalX / displayCount);

                    this.hudLeft = newLeftHud;
                    this.hudRight = newRightHud;
                }
            }

            this.ProcessHandle.WriteMemory((IntPtr)this.BaseAddress + this.offsetHudLeft, this.hudLeft);

            this.ProcessHandle.WriteMemory((IntPtr)this.BaseAddress + this.offsetHudRight, this.pokeHudRightEnable);
            this.ProcessHandle.WriteMemory((IntPtr)this.BaseAddress + this.offsetHudRight + 0x1, this.hudRight);
        }

        public override void Disable()
        {
            base.Disable();

            this.ProcessHandle.WriteMemory((IntPtr)this.BaseAddress + this.offsetHudLeft, 0);
            this.ProcessHandle.WriteMemory((IntPtr)this.BaseAddress + this.offsetHudRight, this.pokeHudRightDisable);
        }

        #endregion
    }
}
