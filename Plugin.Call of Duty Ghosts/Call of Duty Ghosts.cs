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
    using System.Drawing;
    using System.Globalization;
    using System.Threading;
    using System.Windows.Forms;
    using Library.DetectDisplays;
    using Library.Plugin;
    using Library.Process;

    [FixerPluginAttribute("Call of Duty Ghosts")]
    public sealed partial class CallOfDutyGhostsGame : BasePlugin, IFixer
    {
        #region Plugin-Specific Data

        // Used to restore the view aspect-ratio to default
        private readonly float defaultAspectRatio = 1.77777777f;

        // Divide the hud resolution by this for element offsets (left, top, right, bottom)
        private readonly float hudPlacementDivisor = 20.0f;

        // Vertical resolution used for calculating scaling
        private readonly float defaultVerticalResolution = 480.0f;

        private IConfiguration configurationForm;

        private int offsetPokeMovieHorizontalResolution;
        private byte[] pokeMovieHorizontalResolutionEnable;
        private byte[] pokeMovieHorizontalResolutionDisable;
        private byte[] movieHorizontalResolutionRestoreAddress;

        // This is the start of the set of rects for the menu.
        private int offsetMenuStart;

        // This controls the in-game view aspect-ratio
        private int offsetViewAspectRatio;

        // This holds the horizontal mouse scale as an int
        // Set it to single-screen resolution (how will this work with bezel management?)
        private int offsetMouseHorizontal;

        // This is the start of the rects for the interface/hud.
        private int offsetInterfaceStart;

        // This controls the amount of blur used during menus and while using a scope
        // Force to 1.0f
        private int offsetBlur;

        // This affects the black-bars/vertical aspect-ratio of movies.
        // It also appears to affect text scaling within the game.
        private int offsetMovieVerticalScale;

        private Menu gameMenu;
        private Interface gameInterface;

        #endregion

        #region Constructor

        public CallOfDutyGhostsGame() : base()
        {
            this.EnableOnLaunch = true;
            this.GameName = "Call of Duty: Ghosts";
            this.ValueTitle1 = "Base Address";

            this.GameIcon = Properties.Resources.Icon;

            this.configurationForm = new ConfigurationForm();
            this.ConfigurationForm = (ConfigurationForm)this.configurationForm;

            // This data is the same no matter which version the user chooses
            //// this.WindowClass = "IW6";

            this.gameMenu = new Menu();
            this.gameMenu.Smoke.Left = 0x14;
            this.gameMenu.Smoke.Right = 0x1C;
            this.gameMenu.Text.Left = 0x210;
            this.gameMenu.Text.Right = 0x218;
            this.gameMenu.Position.Left = 0x254;
            this.gameMenu.Position.Right = 0x25C;
            this.gameMenu.Background.Left = 0x570;
            this.gameMenu.Background.Right = 0x57C;

            this.gameInterface = new Interface();
            this.gameInterface.TextHorizontalScale = 0x70;
            this.gameInterface.TextHorizontalPlacement = 0x80;
            this.gameInterface.LoadScreen.Left = 0xA8;
            this.gameInterface.LoadScreen.Right = 0xB0;
            this.gameInterface.Chat.Left = 0xC8;
            this.gameInterface.Chat.Right = 0xD0;
            this.gameInterface.ObjectiveText.Left = 0x118;
            this.gameInterface.ObjectiveText.Right = 0x120;
            this.gameInterface.ObjectiveMarkers.Left = 0x138;
            this.gameInterface.ObjectiveMarkers.Right = 0x140;

            this.pokeMovieHorizontalResolutionEnable = new byte[]
            {
                0xB8, 0x00, 0x00, 0x00, 0x00, 0x90 // mov eax,single horizontal resolution as int
            };

            this.pokeMovieHorizontalResolutionDisable = new byte[]
            {
                0x8B, 0x05, 0x00, 0x00, 0x00, 0x00  // mov eax,[address] (this is the horizontal resolution as an int)
            };
        }

        #endregion

        #region Overridden Methods

        public override void UpdateOffsets()
        {
            string gameVersion = this.configurationForm.GameVersion;

            switch (gameVersion)
            {
                case "v3.2 (Single-Player)":
                    this.ExeName = "iw6sp64_ship";
                    this.offsetPokeMovieHorizontalResolution = 0x4E3D89;
                    this.movieHorizontalResolutionRestoreAddress = new byte[] { 0x29, 0x2B, 0x84, 0x05 };
                    this.offsetMenuStart = 0x122BF00;
                    this.offsetViewAspectRatio = 0x136F380;
                    this.offsetMouseHorizontal = -1;
                    this.offsetInterfaceStart = 0x165E190;
                    this.offsetBlur = 0x5D268E8;
                    this.offsetMovieVerticalScale = 0x5D268EC;
                    break;

                case "v3.3 (Single-Player)":
                    this.ExeName = "iw6sp64_ship";
                    this.offsetPokeMovieHorizontalResolution = 0x4E4B69;
                    this.movieHorizontalResolutionRestoreAddress = new byte[] { 0xC9, 0xFD, 0x9B, 0x05 };
                    this.offsetMenuStart = 0x13A9F00;
                    this.offsetViewAspectRatio = 0x14ED390;
                    this.offsetMouseHorizontal = 0x16421E4;
                    this.offsetInterfaceStart = 0x17DC190;
                    this.offsetBlur = 0x5EA4968;
                    this.offsetMovieVerticalScale = 0x5EA496C;
                    break;

                case "v3.4 (Single-Player)":
                    this.ExeName = "iw6sp64_ship";
                    this.offsetPokeMovieHorizontalResolution = 0x4E52B9;
                    this.movieHorizontalResolutionRestoreAddress = new byte[] { 0xF9, 0x23, 0x9C, 0x05 };
                    this.offsetMenuStart = 0x13AB080;
                    this.offsetViewAspectRatio = 0x14EE4E0;
                    this.offsetMouseHorizontal = 0x1643364;
                    this.offsetInterfaceStart = 0x17DD310;
                    this.offsetBlur = 0x5EA76E8;
                    this.offsetMovieVerticalScale = 0x5EA76EC;
                    break;

                case "v3.5 (Single-Player)":
                    this.ExeName = "iw6sp64_ship";
                    this.offsetPokeMovieHorizontalResolution = 0x4E5E69;
                    this.movieHorizontalResolutionRestoreAddress = new byte[] { 0x49, 0x29, 0x9C, 0x05 };
                    this.offsetMenuStart = 0x13AC100;
                    this.offsetViewAspectRatio = 0x14EF510;
                    this.offsetMouseHorizontal = 0x1644434;
                    this.offsetInterfaceStart = 0x17DE390;
                    this.offsetBlur = 0x5EA87E8;
                    this.offsetMovieVerticalScale = 0x5EA87EC;
                    break;

                case "v3.6 (Single-Player)":
                    this.ExeName = "iw6sp64_ship";
                    this.offsetPokeMovieHorizontalResolution = 0x4E6FC9;
                    this.movieHorizontalResolutionRestoreAddress = new byte[] { 0xE9, 0x80, 0x9C, 0x05 };
                    this.offsetMenuStart = 0x13AF600;
                    this.offsetViewAspectRatio = 0x14F2A10;
                    this.offsetMouseHorizontal = 0x1624A64;
                    this.offsetInterfaceStart = 0x17E1890;
                    this.offsetBlur = 0x5EAF0E8;
                    this.offsetMovieVerticalScale = 0x5EAF0EC;
                    break;

                case "v3.2 (Multi-Player)":
                    this.ExeName = "iw6mp64_ship";
                    this.offsetPokeMovieHorizontalResolution = -1;
                    this.offsetMenuStart = 0x1479B40;
                    this.offsetViewAspectRatio = 0x1691290;
                    this.offsetMouseHorizontal = -1;
                    this.offsetInterfaceStart = 0x1C34490;
                    this.offsetBlur = 0x7D40CE8;
                    this.offsetMovieVerticalScale = 0x7D40CEC;
                    break;

                case "v3.3 (Multi-Player)":
                    this.ExeName = "iw6mp64_ship";
                    this.offsetPokeMovieHorizontalResolution = -1;
                    this.offsetMenuStart = 0x15F7B50;
                    this.offsetViewAspectRatio = 0x180F310;
                    this.offsetMouseHorizontal = 0x1CAA930;
                    this.offsetInterfaceStart = 0x1DB2830;
                    this.offsetBlur = 0x7EBF2E8;
                    this.offsetMovieVerticalScale = 0x7EBF2EC;
                    break;

                case "v3.4 (Multi-Player)":
                    this.ExeName = "iw6mp64_ship";
                    this.offsetPokeMovieHorizontalResolution = -1;
                    this.offsetMenuStart = 0x15FAC50;
                    this.offsetViewAspectRatio = 0x1821290;
                    this.offsetMouseHorizontal = 0x1CBC8B0;
                    this.offsetInterfaceStart = 0x1DC47B0;
                    this.offsetBlur = 0x7ED2EE8;
                    this.offsetMovieVerticalScale = 0x7ED2EEC;
                    break;

                case "v3.5 (Multi-Player)":
                    this.ExeName = "iw6mp64_ship";
                    this.offsetPokeMovieHorizontalResolution = -1;
                    this.offsetMenuStart = 0x15FDDD0;
                    this.offsetViewAspectRatio = 0x1824410;
                    this.offsetMouseHorizontal = 0x1CBFA30;
                    this.offsetInterfaceStart = 0x1DC7A50;
                    this.offsetBlur = 0x7EEC468;
                    this.offsetMovieVerticalScale = 0x7EEC46C;
                    break;

                case "v3.6 (Multi-Player)":
                    this.ExeName = "iw6mp64_ship";
                    this.offsetPokeMovieHorizontalResolution = -1;
                    this.offsetMenuStart = 0x1611C50;
                    this.offsetViewAspectRatio = 0x1835C90;
                    this.offsetMouseHorizontal = 0x1CD5770;
                    this.offsetInterfaceStart = 0x1DD96F0;
                    this.offsetBlur = 0x7F32BE8;
                    this.offsetMovieVerticalScale = 0x7F32BEC;
                    break;

                default:
                    this.ExeName = "iw6mp64_ship";
                    this.offsetPokeMovieHorizontalResolution = -1;
                    this.offsetMenuStart = 0x1611C50;
                    this.offsetViewAspectRatio = 0x1835C90;
                    this.offsetMouseHorizontal = 0x1CD5770;
                    this.offsetInterfaceStart = 0x1DD96F0;
                    this.offsetBlur = 0x7F32BE8;
                    this.offsetMovieVerticalScale = 0x7F32BEC;
                    break;
            }
        }

        public override void UpdateValues()
        {
            base.UpdateValues();

            this.Value1 = this.BaseAddress64 > 0 ? string.Format(CultureInfo.InvariantCulture, "0x{0:X}", this.BaseAddress64) : "N/A";
        }

        public override void Enable()
        {
            base.Enable();

            if (this.offsetPokeMovieHorizontalResolution > 0)
            {
                Thread movieResolutionThread = new Thread(new ThreadStart(this.ModifyMovieResolution));

                movieResolutionThread.Start();
            }

            this.Thread.Start();
        }

        public override void Continual()
        {
            base.Continual();

            while (!this.StopThread)
            {
                float aspectRatio = this.configurationForm.AspectRatio;
                float verticalResolution = this.configurationForm.VerticalResolution;
                float hudLeft = this.configurationForm.HudLeft;
                float hudRight = this.configurationForm.HudRight;
                float one = 1.0f;
                float hudLeftObj = (hudLeft / this.hudPlacementDivisor) + hudLeft;
                float hudRightObj = hudRight - (hudRight / this.hudPlacementDivisor);
                float objTextScale = verticalResolution / this.defaultVerticalResolution;
                float objTextPlacement = this.defaultVerticalResolution / verticalResolution;

                this.ProcessHandle.WriteMemory(this.PointerBaseAddress + this.offsetMenuStart + this.gameMenu.Smoke.Left, hudLeft);
                this.ProcessHandle.WriteMemory(this.PointerBaseAddress + this.offsetMenuStart + this.gameMenu.Smoke.Right, hudRight);
                this.ProcessHandle.WriteMemory(this.PointerBaseAddress + this.offsetMenuStart + this.gameMenu.Text.Left, hudLeft);
                this.ProcessHandle.WriteMemory(this.PointerBaseAddress + this.offsetMenuStart + this.gameMenu.Text.Right, hudRight);
                this.ProcessHandle.WriteMemory(this.PointerBaseAddress + this.offsetMenuStart + this.gameMenu.Position.Left, hudLeft);
                this.ProcessHandle.WriteMemory(this.PointerBaseAddress + this.offsetMenuStart + this.gameMenu.Position.Right, hudRight);
                this.ProcessHandle.WriteMemory(this.PointerBaseAddress + this.offsetMenuStart + this.gameMenu.Background.Left, hudLeft);
                this.ProcessHandle.WriteMemory(this.PointerBaseAddress + this.offsetMenuStart + this.gameMenu.Background.Right, hudRight);

                this.ProcessHandle.WriteMemory(this.PointerBaseAddress + this.offsetViewAspectRatio, aspectRatio);

                if (this.offsetMouseHorizontal > 0)
                {
                    int hudSize = (int)this.configurationForm.HudWidth;
                    this.ProcessHandle.WriteMemory(this.PointerBaseAddress + this.offsetMouseHorizontal, hudSize);
                }

                this.ProcessHandle.WriteMemory(this.PointerBaseAddress + this.offsetInterfaceStart + this.gameInterface.TextHorizontalScale, objTextScale);
                this.ProcessHandle.WriteMemory(this.PointerBaseAddress + this.offsetInterfaceStart + this.gameInterface.TextHorizontalPlacement, objTextPlacement);
                this.ProcessHandle.WriteMemory(this.PointerBaseAddress + this.offsetInterfaceStart + this.gameInterface.LoadScreen.Left, hudLeftObj);
                this.ProcessHandle.WriteMemory(this.PointerBaseAddress + this.offsetInterfaceStart + this.gameInterface.LoadScreen.Right, hudRightObj);
                this.ProcessHandle.WriteMemory(this.PointerBaseAddress + this.offsetInterfaceStart + this.gameInterface.Chat.Left, hudLeft);
                this.ProcessHandle.WriteMemory(this.PointerBaseAddress + this.offsetInterfaceStart + this.gameInterface.Chat.Right, hudRight);
                this.ProcessHandle.WriteMemory(this.PointerBaseAddress + this.offsetInterfaceStart + this.gameInterface.ObjectiveText.Left, hudLeftObj);
                this.ProcessHandle.WriteMemory(this.PointerBaseAddress + this.offsetInterfaceStart + this.gameInterface.ObjectiveText.Right, hudRightObj);
                this.ProcessHandle.WriteMemory(this.PointerBaseAddress + this.offsetInterfaceStart + this.gameInterface.ObjectiveMarkers.Left, hudLeft);
                this.ProcessHandle.WriteMemory(this.PointerBaseAddress + this.offsetInterfaceStart + this.gameInterface.ObjectiveMarkers.Right, hudRight);

                this.ProcessHandle.WriteMemory(this.PointerBaseAddress + this.offsetBlur, one);
                this.ProcessHandle.WriteMemory(this.PointerBaseAddress + this.offsetMovieVerticalScale, one);

                Thread.Sleep(100);
            }
        }

        public override void Disable()
        {
            if (this.offsetPokeMovieHorizontalResolution > 0)
            {
                ProcessFunctions.BlockCopy(this.movieHorizontalResolutionRestoreAddress, this.pokeMovieHorizontalResolutionDisable, 0x2);

                this.ProcessHandle.WriteMemory((IntPtr)this.BaseAddress64 + this.offsetPokeMovieHorizontalResolution, this.pokeMovieHorizontalResolutionDisable);
            }

            float hudLeft = 0.0f;
            float hudRight = this.configurationForm.HorizontalResolution;
            float hudLeftObj = (hudRight / this.hudPlacementDivisor) + hudLeft;
            float hudRightObj = hudRight - (hudRight / this.hudPlacementDivisor);

            this.ProcessHandle.WriteMemory(this.PointerBaseAddress + this.offsetMenuStart + this.gameMenu.Smoke.Left, hudLeft);
            this.ProcessHandle.WriteMemory(this.PointerBaseAddress + this.offsetMenuStart + this.gameMenu.Smoke.Right, hudRight);
            this.ProcessHandle.WriteMemory(this.PointerBaseAddress + this.offsetMenuStart + this.gameMenu.Text.Left, hudLeft);
            this.ProcessHandle.WriteMemory(this.PointerBaseAddress + this.offsetMenuStart + this.gameMenu.Text.Right, hudRight);
            this.ProcessHandle.WriteMemory(this.PointerBaseAddress + this.offsetMenuStart + this.gameMenu.Position.Left, hudLeft);
            this.ProcessHandle.WriteMemory(this.PointerBaseAddress + this.offsetMenuStart + this.gameMenu.Position.Right, hudRight);
            this.ProcessHandle.WriteMemory(this.PointerBaseAddress + this.offsetMenuStart + this.gameMenu.Background.Left, hudLeft);
            this.ProcessHandle.WriteMemory(this.PointerBaseAddress + this.offsetMenuStart + this.gameMenu.Background.Right, hudRight);

            this.ProcessHandle.WriteMemory(this.PointerBaseAddress + this.offsetViewAspectRatio, this.defaultAspectRatio);

            if (this.offsetMouseHorizontal > 0)
            {
                int hudSize = (int)this.configurationForm.HorizontalResolution;
                this.ProcessHandle.WriteMemory(this.PointerBaseAddress + this.offsetMouseHorizontal, hudSize);
            }

            //// this.ProcessHandle.WriteMemory(this.pBaseAddress + this.offsetInterfaceStart + this.gameInterface.TextHorizontalScale, NAHHH);
            //// this.ProcessHandle.WriteMemory(this.pBaseAddress + this.offsetInterfaceStart + this.gameInterface.TextHorizontalPlacement, NAHHH);

            this.ProcessHandle.WriteMemory(this.PointerBaseAddress + this.offsetInterfaceStart + this.gameInterface.LoadScreen.Left, hudLeftObj);
            this.ProcessHandle.WriteMemory(this.PointerBaseAddress + this.offsetInterfaceStart + this.gameInterface.LoadScreen.Right, hudRightObj);
            this.ProcessHandle.WriteMemory(this.PointerBaseAddress + this.offsetInterfaceStart + this.gameInterface.Chat.Left, hudLeft);
            this.ProcessHandle.WriteMemory(this.PointerBaseAddress + this.offsetInterfaceStart + this.gameInterface.Chat.Right, hudRight);
            this.ProcessHandle.WriteMemory(this.PointerBaseAddress + this.offsetInterfaceStart + this.gameInterface.ObjectiveText.Left, hudLeftObj);
            this.ProcessHandle.WriteMemory(this.PointerBaseAddress + this.offsetInterfaceStart + this.gameInterface.ObjectiveText.Right, hudRightObj);
            this.ProcessHandle.WriteMemory(this.PointerBaseAddress + this.offsetInterfaceStart + this.gameInterface.ObjectiveMarkers.Left, hudLeft);
            this.ProcessHandle.WriteMemory(this.PointerBaseAddress + this.offsetInterfaceStart + this.gameInterface.ObjectiveMarkers.Right, hudRight);

            //// this.ProcessHandle.WriteMemory(this.pBaseAddress + this.offsetBlur, NAHHH);
            //// this.ProcessHandle.WriteMemory(this.pBaseAddress + this.offsetMovieVerticalScale, NAHHH);

            base.Disable();
        }

        #endregion

        #region Custom Methods

        private void ModifyMovieResolution()
        {
            // Sleep two seconds to give the code a chance to fall into place.
            Thread.Sleep(2000);

            int hudHorizontalSize = (int)this.configurationForm.HudWidth;

            byte[] hudHorizontalSizeBytes = BitConverter.GetBytes(hudHorizontalSize);

            ProcessFunctions.BlockCopy(hudHorizontalSizeBytes, this.pokeMovieHorizontalResolutionEnable, 0x1);

            this.ProcessHandle.WriteMemory(this.PointerBaseAddress + this.offsetPokeMovieHorizontalResolution, this.pokeMovieHorizontalResolutionEnable);
        }

        #endregion

        #region Custom Structs

        private struct Rect
        {
            public int Left { get; set; }

            // public int Top { get; set; }

            public int Right { get; set; }

            // public int Bottom { get; set; }
        }

        private struct Menu
        {
            public Rect Smoke;
            public Rect Text;
            public Rect Position;
            public Rect Background;
        }

        private struct Interface
        {
            public Rect LoadScreen;
            public Rect Chat;
            public Rect ObjectiveText;
            public Rect ObjectiveMarkers;

            public int TextHorizontalScale { get; set; }

            public int TextHorizontalPlacement { get; set; }
        }

        #endregion
    }
}
