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
    using System;
    using System.Drawing;
    using System.Globalization;
    using System.Threading;
    using System.Windows.Forms;
    using Library.Plugin;
    using Library.Process;

    [FixerPluginAttribute("Dead Space")]
    public sealed partial class DeadSpaceGame : BasePlugin, IFixer
    {
        #region Plugin-Specific Data

        private IConfiguration configurationForm;

        private int offsetFieldOfView;

        private int offsetPoke;
        private string pokeEnable;
        private string pokeDisable;
        private int offsetCodeCave;
        private string pokeCodeCave;

        #endregion

        #region Constructor

        public DeadSpaceGame()
            : base()
        {
            this.GameName = "Dead Space";

            this.ValueTitle1 = "Field-of-View";
            this.ValueTitle2 = "Add Amount";
            this.ValueTitle3 = "Base Address";

            this.GameIcon = Properties.Resources.Icon;

            this.configurationForm = new ConfigurationForm();
            this.ConfigurationForm = (ConfigurationForm)this.configurationForm;

            // This data is the same no matter which version the user chooses
            this.ExeName = "Dead Space";
            this.WindowClass = "DeadSpaceWndClass";
            this.WindowTitle = "Dead Space";

            this.pokeEnable = @"call 0x{0:X}
                                nop
                                nop
                                nop";

            this.pokeDisable = @"movss [edi+0x0000014C],xmm0";

            this.pokeCodeCave = @"movss [0x{0:X}],xmm0
                                  fld dword [0x{0:X}]
                                  fld dword [0x{1:X}]
                                  faddp
                                  fstp dword [edi+0x0000014C]
                                  ret";
        }

        #endregion

        #region Overridden Methods

        public override void UpdateOffsets()
        {
            int gameVersion = Properties.Settings.Default.GameVersion;

            switch (gameVersion)
            {
                case 0:
                    this.offsetFieldOfView = 0xB504DC;
                    this.offsetPoke = 0x2FB581;
                    break;

                default:
                    this.offsetFieldOfView = 0xB504DC;
                    this.offsetPoke = 0x2FB581;
                    break;
            }
        }

        public override void UpdateValues()
        {
            base.UpdateValues();

            var fieldOfView = this.ProcessHandle.ReadMemory<float>(this.PointerBaseAddress + this.offsetFieldOfView);

            this.Value1 = fieldOfView > 0.0f ? string.Format(CultureInfo.InvariantCulture, "{0:0.########}", fieldOfView) : "N/A";
            this.Value2 = string.Format(CultureInfo.InvariantCulture, "{0:0.########}", this.configurationForm.AmountToModifyFieldOfView);
            this.Value3 = this.BaseAddress > 0 ? string.Format(CultureInfo.InvariantCulture, "0x{0:X}", this.BaseAddress) : "N/A";
        }

        public override void Enable()
        {
            base.Enable();

            IntPtr offsetCodeCave = ProcessFunctions.AllocateMemory<IntPtr>(this.ProcessHandle, 0x64);

            this.offsetCodeCave = (int)offsetCodeCave;
            int offsetFov = this.offsetCodeCave + 0x30;
            int offsetAdd = this.offsetCodeCave + 0x40;

            if (offsetCodeCave != null && offsetCodeCave != IntPtr.Zero)
            {
                // Fill in the arguments.
                string pokeCodeCave = string.Format(CultureInfo.InvariantCulture, this.pokeCodeCave, offsetFov, offsetAdd);

                // Assemble the code-cave.
                byte[] pokeCodeCaveBytes = ProcessFunctions.Assemble(pokeCodeCave);

                // Write the code-cave to memory.
                this.ProcessHandle.WriteMemory((IntPtr)this.offsetCodeCave, pokeCodeCaveBytes);

                // Calculate the call address.
                int pokeCall = this.offsetCodeCave - (this.BaseAddress + this.offsetPoke);

                // Fill in the arguments.
                string pokeEnable = string.Format(CultureInfo.InvariantCulture, this.pokeEnable, pokeCall);

                // Assemble the enable poke.
                byte[] pokeEnableBytes = ProcessFunctions.Assemble(pokeEnable);

                // Write the call to memory.
                this.ProcessHandle.WriteMemory((IntPtr)this.BaseAddress + this.offsetPoke, pokeEnableBytes);
            }

            this.Thread.Start();
        }

        public override void Continual()
        {
            base.Continual();

            while (!this.StopThread)
            {
                this.ProcessHandle.WriteMemory((UIntPtr)this.offsetCodeCave + 0x40, this.configurationForm.AmountToModifyFieldOfView);

                Thread.Sleep(125);
            }
        }

        public override void Disable()
        {
            base.Disable();

            byte[] disableBytes = ProcessFunctions.Assemble(this.pokeDisable);

            this.ProcessHandle.WriteMemory(this.PointerBaseAddress + this.offsetPoke, disableBytes);
        }

        #endregion
    }
}