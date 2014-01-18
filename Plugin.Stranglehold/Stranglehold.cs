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

namespace Plugin.Stranglehold
{
    using System;
    using System.Drawing;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows.Forms;
    using Library.Plugin;
    using Library.Process;

    [FixerPluginAttribute("Stranglehold")]
    public sealed partial class StrangleholdGame : BasePlugin, IFixer
    {
        #region Plugin-Specific Data

        private int offsetFieldOfView;

        private int offsetPoke1; // 4003F0
        private byte[] pokeEnable1;
        private byte[] pokeDisable1;

        private int offsetPoke2; // 400400
        private byte[] pokeEnable2;
        private byte[] pokeDisable2;

        private int offsetPoke3; // 849E03
        private byte[] pokeEnable3;
        private byte[] pokeDisable3;

        private int offsetPoke4; // 849BE6
        private byte[] pokeEnable4;
        private byte[] pokeDisable4;

        #endregion

        #region Constructor

        public StrangleholdGame() : base()
        {
            this.GameName = "Stranglehold";
            this.ValueTitle1 = "Field-of-View";
            this.ValueTitle2 = "Base Address";

            this.GameIcon = Properties.Resources.Icon;
            this.ConfigurationForm = new ConfigurationForm();

            // This data is the same no matter which version the user chooses
            this.ExeName = "Retail-Stranglehold";
            this.WindowClass = "LaunchUnrealUWindowsClient";
            this.WindowTitle = "StrangleEngine";

            //// Why the fuck don't these have any comments describing their ASM?

            this.pokeEnable1 = new byte[]
            {
                0x00, 0x00, 0x80, 0x3F,
                0x00, 0x00, 0x80, 0x3F,
                0xB8, 0x1E, 0xA5, 0x3F
            };

            this.pokeDisable1 = new byte[]
            {
                0x73, 0x4D, 0x16, 0x06,
                0x4D, 0x02, 0x8C, 0x00,
                0x00, 0x70, 0x64, 0x02
            };

            this.pokeEnable2 = new byte[]
            {
                0xF3, 0x0F, 0x11, 0x8C,
                0x24, 0xB4, 0x00, 0x00,
                0x00, 0xD9, 0x05, 0xF0,
                0x03, 0x40, 0x00, 0xD9,
                0x9C, 0x24, 0x80, 0x00,
                0x00, 0x00, 0xD9, 0x05,
                0xF4, 0x03, 0x40, 0x00,
                0xD9, 0x9C, 0x24, 0x88,
                0x00, 0x00, 0x00, 0xE9,
                0xE4, 0x99, 0x44, 0x00, 
            };

            this.pokeDisable2 = new byte[]
            {
                0xDE, 0xFE, 0xFF, 0xFF,
                0x57, 0x8B, 0x7C, 0x24,
                0x10, 0x83, 0xEF, 0x01,
                0x78, 0x24, 0x53, 0x8B,
                0x5C, 0x24, 0x18, 0x55,
                0x8B, 0x6C, 0x24, 0x14,
                0x56, 0x8B, 0x74, 0x08,
                0x8D, 0xA4, 0x24, 0x00,
                0xC1, 0xFE, 0xBF, 0xFF,
                0x00, 0x8B, 0xCE, 0xFF
            };

            this.pokeEnable3 = new byte[]
            {
                0xE9, 0xF8, 0x65, 0xBB,
                0xFF, 0x90, 0x90, 0x90,
                0x90
            };

            this.pokeDisable3 = new byte[]
            {
                0xF3, 0x0F, 0x11, 0x8C,
                0x24, 0xB4, 0x00, 0x00,
                0x00
            };

            this.pokeEnable4 = new byte[]
            {
                0xF8, 0x03, 0x40, 0x00
            };

            this.pokeDisable4 = new byte[]
            {
                0x94, 0x57, 0xD5, 0x00
            };
        }

        #endregion

        #region Overridden Methods

        public override void UpdateOffsets()
        {
            int gameVersion = Properties.Settings.Default.GameVersion;

            switch (gameVersion)
            {
                case 0:
                    this.offsetFieldOfView = 0x3F8;
                    this.offsetPoke1 = 0x3F0;
                    this.offsetPoke2 = 0x400;
                    this.offsetPoke3 = 0x449E03;
                    this.offsetPoke4 = 0x449BE6;
                    break;

                default:
                    this.offsetFieldOfView = 0x3F8;
                    this.offsetPoke1 = 0x3F0;
                    this.offsetPoke2 = 0x400;
                    this.offsetPoke3 = 0x449E03;
                    this.offsetPoke4 = 0x449BE6;
                    break;
            }
        }

        public override void UpdateValues()
        {
            base.UpdateValues();

            var currentFieldOfView = this.ProcessHandle.ReadMemory<float>(this.PointerBaseAddress + this.offsetFieldOfView);

            this.Value1 = currentFieldOfView > 0.0f ? string.Format(CultureInfo.InvariantCulture, "{0:0.########}", currentFieldOfView) : "N/A";
            this.Value2 = this.BaseAddress > 0 ? string.Format(CultureInfo.InvariantCulture, "0x{0:X}", this.BaseAddress) : "N/A";
        }

        public override void Enable()
        {
            base.Enable();

            Enumerations.MemoryProtections previousMemoryProtection;

            UnsafeNativeMethods.VirtualProtectEx(this.ProcessHandle, this.PointerBaseAddress, new UIntPtr(0x450000), Enumerations.MemoryProtections.ExecuteReadWrite, out previousMemoryProtection);

            this.ProcessHandle.WriteMemory(this.PointerBaseAddress + this.offsetPoke1, this.pokeEnable1);
            this.ProcessHandle.WriteMemory(this.PointerBaseAddress + this.offsetPoke2, this.pokeEnable2);
            this.ProcessHandle.WriteMemory(this.PointerBaseAddress + this.offsetPoke3, this.pokeEnable3);
            this.ProcessHandle.WriteMemory(this.PointerBaseAddress + this.offsetPoke4, this.pokeEnable4);

            //// ProcessFunctions.VirtualProtectEx(this.ProcessHandle, (IntPtr)this.BaseAddress, 0x450000, previousMemoryProtection, out previousMemoryProtection);

            this.Thread.Start();
        }

        public override void Continual()
        {
            base.Continual();

            while (!this.StopThread)
            {
                float fieldOfView = (float)Properties.Settings.Default.TrackBarValue / 1000.0f;

                this.ProcessHandle.WriteMemory(this.PointerBaseAddress + this.offsetFieldOfView, fieldOfView);

                // Sleep for one second.
                Thread.Sleep(1000);
            }
        }

        public override void Disable()
        {
            base.Disable();

            this.ProcessHandle.WriteMemory(this.PointerBaseAddress + this.offsetPoke1, this.pokeDisable1);
            this.ProcessHandle.WriteMemory(this.PointerBaseAddress + this.offsetPoke2, this.pokeDisable2);
            this.ProcessHandle.WriteMemory(this.PointerBaseAddress + this.offsetPoke3, this.pokeDisable3);
            this.ProcessHandle.WriteMemory(this.PointerBaseAddress + this.offsetPoke4, this.pokeDisable4);
        }

        #endregion
    }
}
