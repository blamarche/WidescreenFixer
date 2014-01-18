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
    using System.Drawing;
    using System.Globalization;
    using System.Threading;
    using System.Windows.Forms;
    using Library.Plugin;
    using Library.Process;

    [FixerPluginAttribute("Battlefield 3")]
    public sealed partial class Battlefield3Game : BasePlugin, IFixer
    {
        #region Plugin-Specific Data

        private IConfiguration configurationForm;

        private int offsetFieldOfView;
        private int offsetFieldOfViewAdd;
        private int offsetFieldOfViewRead;

        private int offsetPoke;
        private string pokeOn;
        private string pokeOff;
        private int offsetCodeCave;
        private string pokeCodeCave;

        private float defaultFieldOfView = 55.0f;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the Battlefield_3 class.
        /// </summary>
        public Battlefield3Game() : base()
        {
            // This is the text that will appear within the main application.
            this.GameName = "Battlefield 3";

            // These are the titles that will appear on the main application under the Status section.
            this.ValueTitle1 = "Field-of-View";
            this.ValueTitle2 = "Add Amount";
            this.ValueTitle3 = "Base Address";

            // This is the game icon that will appear in the combobox on the main application as well as the configuration form titlebar.
            this.GameIcon = Properties.Resources.Icon;

            // Create a new instance of the Configuration class.
            this.configurationForm = new ConfigurationForm();
            this.ConfigurationForm = (ConfigurationForm)this.configurationForm;

            //// The data below this line will be the same no matter which version of the game the user chooses.
            //// Everything that does change between versions needs to be placed in the UpdateOffsets() method.

            // The name of the game's executable file.
            this.ExeName = "bf3";
            this.WindowClass = "Battlefield 3™";
            this.WindowTitle = "Battlefield 3™";

            this.pokeOn = @"call {0:X}h
                            nop
                            nop
                            nop";

                        //// fld dword [esp+04h]
                        //// mov ecx,[esp+0Ch]
            this.pokeOff = @"movq xmm0,[ecx]
                             movq [eax],xmm0";
                        //// movq xmm0,[ecx+08h]
                        //// fstp dword [eax+34h]

            this.pokeCodeCave = @"movq xmm0,[ecx]
                                  movq [{0:X}h],xmm0
                                  fld dword [{0:X}h]
                                  fld dword [{1:X}h]
                                  faddp
                                  fstp dword [eax]
                                  ret";
        }

        #endregion

        #region Overridden Methods

        /// <summary>
        /// Override the UpdateOffsets() method with our own code.
        /// </summary>
        public override void UpdateOffsets()
        {
            // Switch between the game version value and fill the fields with the appropriate data.
            switch (this.configurationForm.GameVersion)
            {
                // v925790
                case 5:
                    this.offsetFieldOfView = 0x1F79A60;
                    this.offsetFieldOfViewAdd = 0x0;
                    this.offsetFieldOfViewRead = 0x30;
                    this.offsetPoke = 0x7649FD;
                    break;

                // v936211
                case 4:
                    this.offsetFieldOfView = 0x1F8D4F0;
                    this.offsetFieldOfViewAdd = 0x0;
                    this.offsetFieldOfViewRead = 0x30;
                    this.offsetPoke = 0xA3469D;
                    break;

                // v944019
                case 3:
                    this.offsetFieldOfView = 0x1FC8360;
                    this.offsetFieldOfViewAdd = 0x0;
                    this.offsetFieldOfViewRead = 0x30;
                    this.offsetPoke = 0xE1F99D;
                    break;

                // v981420
                case 2:
                    this.offsetFieldOfView = 0x1F4AC30;
                    this.offsetFieldOfViewAdd = 0x0;
                    this.offsetFieldOfViewRead = 0x30;
                    this.offsetPoke = 0x88D2DD;
                    break;

                // v1089904
                case 1:
                    this.offsetFieldOfView = 0x1F67810;
                    this.offsetFieldOfViewAdd = 0x0;
                    this.offsetFieldOfViewRead = 0x30;
                    this.offsetPoke = 0xC456CD;
                    break;

                // v1147186
                case 0:
                    this.offsetFieldOfView = 0x1F8B4A0;
                    this.offsetFieldOfViewAdd = 0x0;
                    this.offsetFieldOfViewRead = 0x30;
                    this.offsetPoke = 0xC9122D;
                    break;

                // This will be used if the game version is an invalid setting.
                default:
                    this.offsetFieldOfView = 0x1F67810;
                    this.offsetFieldOfViewAdd = 0x0;
                    this.offsetFieldOfViewRead = 0x30;
                    this.offsetPoke = 0xC456CD;
                    break;
            }
        }

        /// <summary>
        /// Override the UpdateValues() method with our own code.
        /// </summary>
        public override void UpdateValues()
        {
            // Execute the base UpdateValues() method.
            // THIS IS REQUIRED!
            base.UpdateValues();

            var offsetRead = this.ProcessHandle.ReadMemory<uint>((IntPtr)this.BaseAddress + this.offsetFieldOfView);
            var fieldOfView = this.ProcessHandle.ReadMemory<float>((UIntPtr)offsetRead + this.offsetFieldOfViewRead);

            this.Value1 = fieldOfView > 0.0f ? string.Format(CultureInfo.InvariantCulture, "{0:0.########}", fieldOfView) : "N/A";
            this.Value2 = string.Format(CultureInfo.InvariantCulture, "{0:0.########}", this.configurationForm.AmountToModifyFieldOfView);
            this.Value3 = this.BaseAddress > 0 ? string.Format(CultureInfo.InvariantCulture, "0x{0:X}", this.BaseAddress) : "N/A";
        }

        /// <summary>
        /// Override the Enable() method with our own code.
        /// </summary>
        public override void Enable()
        {
            // Execute the base Enable() method.
            // THIS IS REQUIRED!
            base.Enable();

            // Allocate 64 bytes for the code-cave.
            IntPtr offsetCodeCave = this.ProcessHandle.AllocateMemory<IntPtr>(0x64);

            // Store the address of the code-cave.
            this.offsetCodeCave = (int)offsetCodeCave;
            int offsetFov = this.offsetCodeCave + 0x30;
            int offsetAdd = this.offsetCodeCave + 0x40;

            // Check if the code-cave is valid.
            // Should we compare against IntPtr.Zero?
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
                string pokeEnable = string.Format(CultureInfo.InvariantCulture, this.pokeOn, pokeCall);

                // Assemble the enable poke.
                byte[] pokeEnableBytes = ProcessFunctions.Assemble(pokeEnable);

                // Write the call to memory.
                this.ProcessHandle.WriteMemory((IntPtr)this.BaseAddress + this.offsetPoke, pokeEnableBytes);

                // Now write the initial field-of-view so the change takes effect right away.
                float amountToAdd = this.defaultFieldOfView + this.configurationForm.AmountToModifyFieldOfView;
                var readMemory = this.ProcessHandle.ReadMemory<uint>(this.PointerBaseAddress + this.offsetFieldOfView);
                this.ProcessHandle.WriteMemory((UIntPtr)readMemory + this.offsetFieldOfViewAdd, amountToAdd);
            }

            // Start the Continual() thread.
            // THIS IS REQUIRED!
            this.Thread.Start();
        }

        /// <summary>
        /// Override the Continual() method with our own code.
        /// </summary>
        public override void Continual()
        {
            // Execute the base Continual() method.
            // THIS IS REQUIRED!
            base.Continual();

            // Continually execute this code until StopThread is set to true.
            // THIS IS REQUIRED!
            while (!this.StopThread)
            {
                // Write the amount to add to the field-of-view.
                this.ProcessHandle.WriteMemory((UIntPtr)this.offsetCodeCave + 0x40, this.configurationForm.AmountToModifyFieldOfView);

                //// float amountToAdd = this.defaultFieldOfView + this.configurationForm.AmountToModifyFieldOfView;
                //// var readMemory = this.ProcessHandle.ReadMemory<uint>((IntPtr)this.BaseAddress + this.offsetFieldOfView);
                //// this.ProcessHandle.WriteMemory((UIntPtr)readMemory + this.offsetFieldOfViewAdd, amountToAdd);
            }
        }

        /// <summary>
        /// Override the Disable() method with our own code.
        /// </summary>
        public override void Disable()
        {
            // Execute the base Disable() method.
            // THIS IS REQUIRED!
            base.Disable();

            // Assemble the disable code.
            byte[] disableBytes = ProcessFunctions.Assemble(this.pokeOff);

            // Write the necessary code to disable the fix.
            this.ProcessHandle.WriteMemory(this.PointerBaseAddress + this.offsetPoke, disableBytes);

            var offsetFieldOfView = this.ProcessHandle.ReadMemory<uint>(this.PointerBaseAddress + this.offsetFieldOfView);
            this.ProcessHandle.WriteMemory((UIntPtr)offsetFieldOfView + this.offsetFieldOfViewAdd, this.defaultFieldOfView);
        }

        #endregion
    }
}
