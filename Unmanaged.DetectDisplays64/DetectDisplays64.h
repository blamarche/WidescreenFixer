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

#include <windows.h>
#include <windef.h>
#include <stdio.h>
#include <d3d9.h>
#include <d3d9types.h>
#include "nvapi.h"

extern "C"
{
    #include "..\Unmanaged.AMD64\ati_eyefinity.h"
}

#define DISPLAY_NAME_LEN 32

enum BrandType
{
    AMD,
    NVIDIA,
    UnknownBrand
};

struct DisplayInfo
{
    char VideoCard[512];
    BrandType Brand;
    int Error;
    int ErrorCode;
    char ErrorString[64];
    int MultiMonitorEnabled;
    int DisplayCount;
    int ColumnCount;
    int RowCount;
    int BezelCorrected;
    int HorizontalResolution;
    int VerticalResolution;
    float AspectRatio;
    int Landscape;
    int Portrait;
    int HUDLeft;
    int HUDRight;
    int HUDTop;
    int HUDBottom;
    int HUDWidth;
    int HUDHeight;
};

int InitializeAMDEyefinity(DisplayInfo *displayInfo);
int InitializeNVIDIASurround(DisplayInfo *displayInfo);

extern "C" __declspec(dllexport) int Initialize(DisplayInfo *displayInfo);
