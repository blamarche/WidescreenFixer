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

#include "DetectDisplays64.h"

// AMD
int InitializeAMDEyefinity(DisplayInfo *displayInfo)
{
    char OSDisplayName[DISPLAY_NAME_LEN];
    EyefinityInfoStruct eyefinityInfo = {0};
    int iNumDisplaysInfo;
    DisplayInfoStruct *pDisplaysInfo = NULL;

    int iDevNum = 0;
    DISPLAY_DEVICEA displayDevice;

    memset(&OSDisplayName, 0, DISPLAY_NAME_LEN);

    displayDevice.cb = sizeof(displayDevice);

    while (EnumDisplayDevicesA(0, iDevNum, &displayDevice, 0))
    {
        if (0 != (displayDevice.StateFlags & DISPLAY_DEVICE_PRIMARY_DEVICE))
        {
            memcpy(&OSDisplayName, &displayDevice.DeviceName, DISPLAY_NAME_LEN);
            break;
        }

        iDevNum++;
    }

    int returnCode = FALSE;

    if (TRUE == atiEyefinityGetConfigInfo(OSDisplayName, &eyefinityInfo, &iNumDisplaysInfo, &pDisplaysInfo))
    {
        if (TRUE == eyefinityInfo.iSLSActive)
        {
            int iCurrentDisplaysInfo = 0;

            displayInfo->Brand = AMD;

            displayInfo->DisplayCount = iNumDisplaysInfo;
            displayInfo->MultiMonitorEnabled = 1;
            displayInfo->ColumnCount = eyefinityInfo.iSLSGridWidth;
            displayInfo->RowCount = eyefinityInfo.iSLSGridHeight;
            displayInfo->HorizontalResolution = eyefinityInfo.iSLSWidth;
            displayInfo->VerticalResolution = eyefinityInfo.iSLSHeight;
            displayInfo->AspectRatio = (float)eyefinityInfo.iSLSWidth / (float)eyefinityInfo.iSLSHeight;

            if (eyefinityInfo.iBezelCompensatedDisplay == TRUE)
            {
                displayInfo->BezelCorrected = 1;
            }

            for (iCurrentDisplaysInfo = 0; iCurrentDisplaysInfo < iNumDisplaysInfo; iCurrentDisplaysInfo++)
            {
                if (TRUE == pDisplaysInfo[iCurrentDisplaysInfo].iPreferredDisplay)
                {
                    if (pDisplaysInfo[iCurrentDisplaysInfo].displayRectVisible.iWidth > pDisplaysInfo[iCurrentDisplaysInfo].displayRectVisible.iHeight)
                    {
                        displayInfo->Landscape = 1;
                        displayInfo->HUDLeft = pDisplaysInfo[iCurrentDisplaysInfo].displayRectVisible.iXOffset;
                        displayInfo->HUDRight = pDisplaysInfo[iCurrentDisplaysInfo].displayRectVisible.iXOffset + pDisplaysInfo[iCurrentDisplaysInfo].displayRectVisible.iWidth;
                        displayInfo->HUDTop = pDisplaysInfo[iCurrentDisplaysInfo].displayRectVisible.iYOffset;
                        displayInfo->HUDBottom = pDisplaysInfo[iCurrentDisplaysInfo].displayRectVisible.iYOffset + pDisplaysInfo[iCurrentDisplaysInfo].displayRectVisible.iHeight;
                        displayInfo->HUDWidth = pDisplaysInfo[iCurrentDisplaysInfo].displayRectVisible.iWidth;
                        displayInfo->HUDHeight = pDisplaysInfo[iCurrentDisplaysInfo].displayRectVisible.iHeight;
                    }
                    else
                    {
                        displayInfo->Portrait = 1;
                        displayInfo->HUDLeft = 0;
                        displayInfo->HUDRight = eyefinityInfo.iSLSWidth;
                        displayInfo->HUDTop = 0;
                        displayInfo->HUDBottom = eyefinityInfo.iSLSHeight;
                        displayInfo->HUDWidth = eyefinityInfo.iSLSWidth;
                        displayInfo->HUDHeight = eyefinityInfo.iSLSHeight;
                    }
                }
            }

            returnCode = TRUE;
        }

        atiEyefinityReleaseConfigInfo(&pDisplaysInfo);
    }

    return returnCode;
}

// NVIDIA
int InitializeNVIDIASurround(DisplayInfo *displayInfo)
{
    NvU32 displayID = 0;

    NvAPI_Status returnValue;

    returnValue = NvAPI_DISP_GetGDIPrimaryDisplayId(&displayID);
    if (NVAPI_OK == returnValue)
    {
        returnValue = NvAPI_Initialize();
        if (NVAPI_OK == returnValue)
        {
            NV_RECT viewports[NV_MOSAIC_MAX_DISPLAYS];
            NvU8 isBezelCorrected;
            
            returnValue = NvAPI_Mosaic_GetDisplayViewportsByResolution(displayID, 0, 0, viewports, &isBezelCorrected);
            if (NVAPI_OK == returnValue)
            {
                displayInfo->Brand = NVIDIA;

                displayInfo->MultiMonitorEnabled = 1;
                displayInfo->BezelCorrected = isBezelCorrected ? 1 : 0;

                int displayCount = 0;

                while (viewports[displayCount].top != viewports[displayCount].bottom)
                {
                    displayCount++;
                }

                if (displayCount > 0)
                {
                    displayInfo->ColumnCount = 1;
                    displayInfo->RowCount = 1;
                }
                
                displayInfo->DisplayCount = displayCount;

                if ((displayCount > 1) && (displayCount & 1))
                {
                    bool isSingleDisplayRow = true;
                    for (int i = 1; i < displayCount; i++)
                    {
                        if ((viewports[0].top != viewports[i].top) || (viewports[0].bottom != viewports[i].bottom))
                        {
                            isSingleDisplayRow = false;
                            
                            break;
                        }
                    }

                    displayInfo->ColumnCount = displayCount;
                    NvU32 top = viewports[0].top;
                    NvU32 bottom = viewports[0].bottom;
                    for (int i = 0; i < displayCount; i++)
                    {
                        if ((viewports[i].top != top) || (viewports[i].bottom != bottom))
                        {
                            displayInfo->RowCount++;
                            top = viewports[i].top;
                            bottom = viewports[i].bottom;
                        }
                    }
                    displayInfo->ColumnCount /= displayInfo->RowCount;

                    if (isSingleDisplayRow)
                    {
                        NvU32 centerDisplayIdx = displayCount / 2;

                        NvU32 centerWidth = viewports[centerDisplayIdx].right - viewports[centerDisplayIdx].left + 1;
                        NvU32 centerHeight = viewports[centerDisplayIdx].bottom - viewports[centerDisplayIdx].top + 1;

                        displayInfo->HorizontalResolution = viewports[displayCount-1].right+1;
                        displayInfo->VerticalResolution = centerHeight;
                        displayInfo->AspectRatio = (float)(viewports[displayCount-1].right+1) / (float)centerHeight;
                        
                        if (centerWidth > centerHeight)
                        {
                            displayInfo->Landscape = 1;
                            displayInfo->HUDLeft = viewports[centerDisplayIdx].left;
                            displayInfo->HUDRight = viewports[centerDisplayIdx].left + centerWidth;
                            displayInfo->HUDTop = viewports[centerDisplayIdx].top;
                            displayInfo->HUDBottom = viewports[centerDisplayIdx].top + centerHeight;
                            displayInfo->HUDWidth = centerWidth;
                            displayInfo->HUDHeight = centerHeight;
                        }
                        else
                        {
                            displayInfo->Portrait = 1;
                            displayInfo->HUDLeft = viewports[0].left;
                            displayInfo->HUDRight = viewports[0].left + viewports[displayCount-1].right+1;
                            displayInfo->HUDTop = viewports[centerDisplayIdx].top;
                            displayInfo->HUDBottom = viewports[centerDisplayIdx].top + centerHeight;
                            displayInfo->HUDWidth = viewports[displayCount-1].right+1;
                            displayInfo->HUDHeight = centerHeight;
                        }
                    }
                }
                
                NvAPI_Unload();

                return TRUE;
            }
            else
            {
                displayInfo->ErrorCode = returnValue;
                NvAPI_GetErrorMessage(returnValue, displayInfo->ErrorString);
            }
        }
        else
        {
            displayInfo->ErrorCode = returnValue;
            NvAPI_GetErrorMessage(returnValue, displayInfo->ErrorString);
        }
    }
    else
    {
        displayInfo->ErrorCode = returnValue;
        NvAPI_GetErrorMessage(returnValue, displayInfo->ErrorString);
    }

    NvAPI_Unload();

    displayInfo->Error = 1;

    return FALSE;
}

// Initialize and export display information
extern "C" __declspec(dllexport) int Initialize(DisplayInfo *displayInfo)
{
    displayInfo->VideoCard[0] = '\0';
    displayInfo->Brand = UnknownBrand;
    displayInfo->Error = 0;
    displayInfo->ErrorCode = 0;
    displayInfo->ErrorString[0] = '\0';
    displayInfo->MultiMonitorEnabled = 0;
    displayInfo->DisplayCount = 0;
    displayInfo->ColumnCount = 0;
    displayInfo->RowCount = 0;
    displayInfo->BezelCorrected = 0;
    displayInfo->HorizontalResolution = 0;
    displayInfo->VerticalResolution = 0;
    displayInfo->AspectRatio = 0.0f;
    displayInfo->Landscape = 0;
    displayInfo->Portrait = 0;
    displayInfo->HUDLeft = 0;
    displayInfo->HUDRight = 0;
    displayInfo->HUDTop = 0;
    displayInfo->HUDBottom = 0;
    displayInfo->HUDWidth = 0;
    displayInfo->HUDHeight = 0;

    IDirect3D9 *m_pD3D = NULL;
    m_pD3D = Direct3DCreate9(D3D_SDK_VERSION);
    if (m_pD3D)
    {
        D3DADAPTER_IDENTIFIER9 adapterIdentifier;
        m_pD3D->GetAdapterIdentifier(D3DADAPTER_DEFAULT, 0, &adapterIdentifier);

        strncpy_s(displayInfo->VideoCard, adapterIdentifier.Description, 512);

        m_pD3D->Release();
        m_pD3D = NULL;
    }

    if (TRUE == InitializeAMDEyefinity(displayInfo))
    {
        return TRUE;
    }
    else if (TRUE == InitializeNVIDIASurround(displayInfo))
    {
        return TRUE;
    }
    else
    {
        return FALSE;
    }
}
