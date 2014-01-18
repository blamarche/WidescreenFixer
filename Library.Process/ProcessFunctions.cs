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

namespace Library.Process
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Text;

    // This class provides all the necessary native functions to work with processes
    public static class ProcessFunctions
    {
        #region Public Methods

        // This will get the process ID of the specificed executable
        public static int GetProcessId(this string exeName)
        {
            Process[] gameProcess = Process.GetProcessesByName(exeName);

            int processId = 0;

            if (gameProcess.Length > 0)
            {
                processId = gameProcess[0].Id;
            }

            foreach (var process in gameProcess)
            {
                process.Dispose();
            }

            gameProcess = null;

            return processId;
        }

        // This will get the process ID of the next specified executable
        public static int GetProcessIdNext(this string exeName)
        {
            System.Diagnostics.Process[] gameProcess = System.Diagnostics.Process.GetProcessesByName(exeName);

            if (gameProcess.Length > 1)
            {
                return gameProcess[1].Id;
            }
            else
            {
                return 0;
            }
        }

        // Gets the process ID from a window title.
        public static int GetProcessIdFromWindow(string className, string windowTitle)
        {
            IntPtr windowHandle = UnsafeNativeMethods.FindWindow(className, windowTitle);

            int processId = 0;

            int threadId = UnsafeNativeMethods.GetWindowThreadProcessId(windowHandle, out processId);

            if (threadId > 0)
            {
                return processId;
            }
            else
            {
                return 0;
            }
        }

        // This will get the base address of the specified process ID
        public static int GetBaseAddress(this int processId)
        {
            System.Diagnostics.Process gameProcess = System.Diagnostics.Process.GetProcessById(processId);

            try
            {
                bool notNull = false;

                while (!notNull)
                {
                    if (gameProcess != null)
                    {
                        if (gameProcess.MainModule != null)
                        {
                            if (gameProcess.MainModule.BaseAddress != IntPtr.Zero)
                            {
                                notNull = true;
                            }
                        }
                    }
                }

                return gameProcess.MainModule.BaseAddress.ToInt32();
            }
            catch
            {
                return 0;

                throw;
            }
        }

        // This will get the base address of the specified process ID
        public static long GetBaseAddress64(this int processId, string processExe)
        {
            ModuleInfo moduleInfo = new ModuleInfo();

            moduleInfo = GetModuleInfo(processId, processExe + @".exe");

            return moduleInfo.BaseOfDll.ToInt64();
        }

        // This will get the base size of the specified process ID
        public static int GetBaseSize(this int processId)
        {
            System.Diagnostics.Process gameProcess = System.Diagnostics.Process.GetProcessById(processId);

            try
            {
                bool notNull = false;

                while (!notNull)
                {
                    if (gameProcess != null)
                    {
                        if (gameProcess.MainModule != null)
                        {
                            if (gameProcess.MainModule.ModuleMemorySize > 0)
                            {
                                notNull = true;
                            }
                        }
                    }
                }

                return gameProcess.MainModule.ModuleMemorySize;
            }
            catch
            {
                return 0;

                throw;
            }
        }

        // This will get the module information of the specified module
        public static ModuleInfo GetModuleInfo(this int processId, string moduleName)
        {
            if (!string.IsNullOrEmpty(moduleName))
            {
                System.Diagnostics.Process process = System.Diagnostics.Process.GetProcessById(processId);

                // Less than or equal to Windows XP/2000/2003
                if (Environment.OSVersion.Version.Major <= 5)
                {
                    try
                    {
                        bool notNull = false;

                        while (!notNull)
                        {
                            if (process != null)
                            {
                                if (process.Modules != null)
                                {
                                    notNull = true;
                                }
                            }
                        }

                        System.Diagnostics.ProcessModuleCollection moduleCollection = process.Modules;

                        for (int i = 0; i < moduleCollection.Count; i++)
                        {
                            if (moduleCollection[i].ModuleName.ToUpperInvariant().StartsWith(moduleName.ToUpperInvariant(), StringComparison.OrdinalIgnoreCase))
                            {
                                ModuleInfo moduleInfo;

                                moduleInfo.BaseOfDll = moduleCollection[i].BaseAddress;
                                moduleInfo.SizeOfImage = moduleCollection[i].ModuleMemorySize;
                                moduleInfo.EntryPoint = moduleCollection[i].EntryPointAddress;

                                return moduleInfo;
                            }
                        }
                    }
                    catch
                    {
                        return new ModuleInfo();

                        throw;
                    }
                }
                else
                {
                    try
                    {
                        bool notNull = false;

                        while (!notNull)
                        {
                            if (process != null)
                            {
                                notNull = true;
                            }
                        }

                        IntPtr[] moduleHandles = new IntPtr[1024];

                        GCHandle handleGc = GCHandle.Alloc(moduleHandles, GCHandleType.Pinned);
                        IntPtr modules_p = handleGc.AddrOfPinnedObject();

                        int size = Marshal.SizeOf(typeof(IntPtr)) * moduleHandles.Length;
                        int requiredSize = 0;

                        bool is32Bit = false;
                        UnsafeNativeMethods.IsWow64Process(process.Handle, out is32Bit);

                        Enumerations.ModuleFilter moduleFilter;

                        if (is32Bit)
                        {
                            moduleFilter = Enumerations.ModuleFilter.X32Bit;
                        }
                        else
                        {
                            moduleFilter = Enumerations.ModuleFilter.X64Bit;
                        }

                        if (UnsafeNativeMethods.EnumProcessModulesEx(process.Handle, moduleHandles, 1024, out requiredSize, moduleFilter))
                        {
                            foreach (IntPtr module in moduleHandles)
                            {
                                StringBuilder sb = new StringBuilder(1024);

                                int moduleLength = UnsafeNativeMethods.GetModuleBaseName(process.Handle, module, sb, sb.Capacity);

                                if (moduleLength > 0)
                                {
                                    if (sb.ToString().ToUpperInvariant() == moduleName.ToUpperInvariant())
                                    {
                                        IntPtr pointer = IntPtr.Zero;
                                        pointer = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(ModuleInfo)));

                                        ModuleInfo moduleInfo;

                                        UnsafeNativeMethods.GetModuleInformation(process.Handle, module, pointer, ModuleInfo.SizeOf);

                                        moduleInfo = (ModuleInfo)Marshal.PtrToStructure(pointer, typeof(ModuleInfo));

                                        Marshal.FreeHGlobal(pointer);

                                        return moduleInfo;
                                    }
                                }
                            }
                        }

                        handleGc.Free();
                    }
                    catch
                    {
                        return new ModuleInfo();

                        throw;
                    }
                }

                return new ModuleInfo();
            }

            return new ModuleInfo();
        }

        // This will check if the process is a 32-bit process on a 64-bit processor
        public static bool Is32BitProcessOn64BitProcessor(this int processId)
        {
            System.Diagnostics.Process process = System.Diagnostics.Process.GetProcessById(processId);

            bool is32Bit = false;

            UnsafeNativeMethods.IsWow64Process(process.Handle, out is32Bit);

            return is32Bit;
        }

        // This will get the path to the specified process
        public static string GetProcessPath(this int processId)
        {
            System.Diagnostics.Process gameProcess = System.Diagnostics.Process.GetProcessById(processId);
            string path = gameProcess.Modules[0].FileName;
            return path;
        }

        // Reads from a process's memory.  This method assumes that you are reading 4-bytes exactly.  Do not use this method if you are reading more or less than 4-bytes.
        public static dynamic ReadMemory<T>(this IntPtr processHandle, IntPtr address)
        {
            int bufferSize = 0x4;

            byte[] buffer = new byte[bufferSize];

            UnsafeNativeMethods.ReadProcessMemory(processHandle, address, buffer, new IntPtr(bufferSize), IntPtr.Zero);

            if (typeof(T) == typeof(int))
            {
                return BitConverter.ToInt32(buffer, 0);
            }
            else if (typeof(T) == typeof(long))
            {
                return BitConverter.ToInt64(buffer, 0);
            }
            else if (typeof(T) == typeof(float))
            {
                return BitConverter.ToSingle(buffer, 0);
            }
            else if (typeof(T) == typeof(double))
            {
                return BitConverter.ToDouble(buffer, 0);
            }
            else if (typeof(T) == typeof(uint))
            {
                return BitConverter.ToUInt32(buffer, 0);
            }
            else if (typeof(T) == typeof(ulong))
            {
                return BitConverter.ToUInt64(buffer, 0);
            }
            else
            {
                return buffer;
            }
        }

        // Reads from a process's memory.  This method assumes that you are reading 4-bytes exactly.  Do not use this method if you are reading more or less than 4-bytes.
        public static dynamic ReadMemory<T>(this IntPtr processHandle, UIntPtr address)
        {
            int bufferSize = 0x4;

            byte[] buffer = new byte[bufferSize];

            UnsafeNativeMethods.ReadProcessMemory(processHandle, address, buffer, new IntPtr(bufferSize), IntPtr.Zero);

            if (typeof(T) == typeof(int))
            {
                return BitConverter.ToInt32(buffer, 0);
            }
            else if (typeof(T) == typeof(long))
            {
                return BitConverter.ToInt64(buffer, 0);
            }
            else if (typeof(T) == typeof(float))
            {
                return BitConverter.ToSingle(buffer, 0);
            }
            else if (typeof(T) == typeof(double))
            {
                return BitConverter.ToDouble(buffer, 0);
            }
            else if (typeof(T) == typeof(uint))
            {
                return BitConverter.ToUInt32(buffer, 0);
            }
            else if (typeof(T) == typeof(ulong))
            {
                return BitConverter.ToUInt64(buffer, 0);
            }
            else
            {
                return buffer;
            }
        }

        // Reads from a process's memory.
        public static byte[] ReadMemory(this IntPtr processHandle, IntPtr address, int bufferSize)
        {
            IntPtr addressPtr = (IntPtr)address;

            byte[] buffer = new byte[bufferSize];

            UnsafeNativeMethods.ReadProcessMemory(processHandle, address, buffer, new IntPtr(bufferSize), IntPtr.Zero);

            return buffer;
        }

        // Reads from a process's memory.
        public static byte[] ReadMemory(this IntPtr processHandle, UIntPtr address, int bufferSize)
        {
            UIntPtr addressPtr = (UIntPtr)address;

            byte[] buffer = new byte[bufferSize];

            UnsafeNativeMethods.ReadProcessMemory(processHandle, address, buffer, new IntPtr(bufferSize), IntPtr.Zero);

            return buffer;
        }

        // This will convert a string containing hexadecimal values into a byte array version of the hex.
        // http://stackoverflow.com/questions/321370/convert-hex-string-to-byte-array
        public static byte[] HexStringToByteArray(string hexValue)
        {
            if (!string.IsNullOrEmpty(hexValue))
            {
                if (hexValue.Length % 2 == 1)
                {
                    throw new Exception("String contains an odd number of characters");
                }

                byte[] byteArray = new byte[hexValue.Length >> 1];

                for (int i = 0; i < (hexValue.Length >> 1); ++i)
                {
                    byteArray[i] = (byte)((GetHexValue(hexValue[i << 1]) << 4) + GetHexValue(hexValue[(i << 1) + 1]));
                }

                return byteArray;
            }
            else
            {
                return null;
            }
        }

        // This will convert a byte array to a hexadecimal string.
        public static string ByteArrayToHexString(byte[] byteArray)
        {
            if (byteArray != null && byteArray.Length > 0)
            {
                StringBuilder stringBuilder = new StringBuilder(byteArray.Length * 2);

                foreach (byte b in byteArray)
                {
                    stringBuilder.AppendFormat("{0:X2}", b);
                }

                return stringBuilder.ToString();
            }
            else
            {
                return null;
            }
        }

        // Finds a string within the memory of a process.
        public static int FindInMemory(this int processId, IntPtr address, int size, string searchValue)
        {
            if (!string.IsNullOrEmpty(searchValue))
            {
                Process process = Process.GetProcessById(processId);

                if (process != null)
                {
                    int foundAddress = 0;

                    int readSize = 1024 * 64;

                    byte[] search = HexStringToByteArray(searchValue);

                    for (int j = (int)address; j < ((int)address + size); j += readSize)
                    {
                        using (ProcessMemoryChunk memoryChunk = new ProcessMemoryChunk(process, (IntPtr)j, readSize + search.Length))
                        {
                            byte[] chunk = memoryChunk.Read();

                            for (int k = 0; k < chunk.Length - search.Length; k++)
                            {
                                bool foundOffset = true;

                                for (int l = 0; l < search.Length; l++)
                                {
                                    if (searchValue[l * 2] != '?' && searchValue[(l * 2) + 1] != '?')
                                    {
                                        if (chunk[k + l] != search[l])
                                        {
                                            foundOffset = false;

                                            break;
                                        }
                                    }
                                }

                                if (foundOffset)
                                {
                                    foundAddress = k + j;

                                    break;
                                }
                            }
                        }

                        if (foundAddress != 0)
                        {
                            break;
                        }
                    }

                    return foundAddress;
                }
                else
                {
                    return 0;
                }
            }

            return 0;
        }

        // Writes to a process's memory.
        public static bool WriteMemory(this IntPtr processHandle, IntPtr address, string data)
        {
            Encoding ascii = Encoding.ASCII;
            Encoding unicode = Encoding.Unicode;

            byte[] unicodeBytes = unicode.GetBytes(data);

            byte[] buffer = Encoding.Convert(unicode, ascii, unicodeBytes);

            if (!UnsafeNativeMethods.WriteProcessMemory(processHandle, address, buffer, new IntPtr(buffer.Length), IntPtr.Zero))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Writes to a process's memory.
        public static bool WriteMemory(this IntPtr processHandle, UIntPtr address, string data)
        {
            Encoding ascii = Encoding.ASCII;
            Encoding unicode = Encoding.Unicode;

            byte[] unicodeBytes = unicode.GetBytes(data);

            byte[] buffer = Encoding.Convert(unicode, ascii, unicodeBytes);

            if (!UnsafeNativeMethods.WriteProcessMemory(processHandle, address, buffer, new IntPtr(buffer.Length), IntPtr.Zero))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Writes to a process's memory.
        public static bool WriteMemory(this IntPtr processHandle, IntPtr address, int data)
        {
            byte[] buffer = BitConverter.GetBytes(data);

            if (!UnsafeNativeMethods.WriteProcessMemory(processHandle, address, buffer, new IntPtr(buffer.Length), IntPtr.Zero))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Writes to a process's memory.
        public static bool WriteMemory(this IntPtr processHandle, UIntPtr address, int data)
        {
            byte[] buffer = BitConverter.GetBytes(data);

            if (!UnsafeNativeMethods.WriteProcessMemory(processHandle, address, buffer, new IntPtr(buffer.Length), IntPtr.Zero))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Writes to a process's memory.
        public static bool WriteMemory(this IntPtr processHandle, IntPtr address, float data)
        {
            byte[] buffer = BitConverter.GetBytes(data);

            if (!UnsafeNativeMethods.WriteProcessMemory(processHandle, address, buffer, new IntPtr(buffer.Length), IntPtr.Zero))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Writes to a process's memory.
        public static bool WriteMemory(this IntPtr processHandle, UIntPtr address, float data)
        {
            byte[] buffer = BitConverter.GetBytes(data);

            if (!UnsafeNativeMethods.WriteProcessMemory(processHandle, address, buffer, new IntPtr(buffer.Length), IntPtr.Zero))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Writes to a process's memory.
        public static bool WriteMemory(this IntPtr processHandle, IntPtr address, double data)
        {
            byte[] buffer = BitConverter.GetBytes(data);

            if (!UnsafeNativeMethods.WriteProcessMemory(processHandle, address, buffer, new IntPtr(buffer.Length), IntPtr.Zero))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Writes to a process's memory.
        public static bool WriteMemory(this IntPtr processHandle, UIntPtr address, double data)
        {
            byte[] buffer = BitConverter.GetBytes(data);

            if (!UnsafeNativeMethods.WriteProcessMemory(processHandle, address, buffer, new IntPtr(buffer.Length), IntPtr.Zero))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Writes to a process's memory.
        public static bool WriteMemory(this IntPtr processHandle, IntPtr address, byte[] data)
        {
            if (data != null && data.Length > 0)
            {
                if (!UnsafeNativeMethods.WriteProcessMemory(processHandle, address, data, new IntPtr(data.Length), IntPtr.Zero))
                {
                    return true;
                }
            }

            return false;
        }

        // Writes to a process's memory.
        public static bool WriteMemory(this IntPtr processHandle, UIntPtr address, byte[] data)
        {
            if (data != null && data.Length > 0)
            {
                if (!UnsafeNativeMethods.WriteProcessMemory(processHandle, address, data, new IntPtr(data.Length), IntPtr.Zero))
                {
                    return true;
                }
            }

            return false;
        }

        // Writes to a process's memory
        public static bool WriteMemory(this IntPtr processHandle, IntPtr address, byte[] buffer, int size)
        {
            if (!UnsafeNativeMethods.WriteProcessMemory(processHandle, address, buffer, new IntPtr(size), IntPtr.Zero))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Writes to a process's memory
        public static bool WriteMemory(this IntPtr processHandle, UIntPtr address, byte[] buffer, int size)
        {
            if (!UnsafeNativeMethods.WriteProcessMemory(processHandle, address, buffer, new IntPtr(size), IntPtr.Zero))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Opens an existing local process object.  This method assumes you do not wish to inherit the specified process's handle.
        public static IntPtr OpenProcess(this int processId, Enumerations.ProcessAccess desiredAccess)
        {
            return UnsafeNativeMethods.OpenProcess(new IntPtr((long)desiredAccess), false, processId);
        }

        // This will close a handle to an open process
        public static bool CloseMemory(this IntPtr processHandle)
        {
            return UnsafeNativeMethods.CloseHandle(processHandle);
        }

        // This will allocate memory within a given process
        public static dynamic AllocateMemory<T>(this IntPtr processHandle, int size)
        {
            if (typeof(T) == typeof(IntPtr))
            {
                return (IntPtr)UnsafeNativeMethods.VirtualAllocEx(processHandle, IntPtr.Zero, new IntPtr(size), Enumerations.MemoryStates.Commit | Enumerations.MemoryStates.Reserve, Enumerations.MemoryProtections.ExecuteReadWrite);
            }
            else if (typeof(T) == typeof(UIntPtr))
            {
                return (UIntPtr)UnsafeNativeMethods.VirtualAllocEx(processHandle, UIntPtr.Zero, new IntPtr(size), Enumerations.MemoryStates.Commit | Enumerations.MemoryStates.Reserve, Enumerations.MemoryProtections.ExecuteReadWrite);
            }
            else
            {
                return IntPtr.Zero;
            }
        }

        // This will copy a value to a destination byte array at the specified offset.
        public static void BlockCopy(byte[] value, byte[] array, int destination)
        {
            if (value != null && value.Length > 0)
            {
                Buffer.BlockCopy(value, 0x0, array, destination, value.Length);
            }
        }

        // This will copy a value to a destination byte array at the specified offset.  WARNING: This assumes you are copying only 4 bytes!
        public static void BlockCopy(float value, byte[] array, int destination)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            Buffer.BlockCopy(buffer, 0x0, array, destination, 0x4);
        }

        // This will copy a value to a destination byte array at the specified offset.  WARNING: This assumes you are copying only 4 bytes!
        public static void BlockCopy(int value, byte[] array, int destination)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            Buffer.BlockCopy(buffer, 0x0, array, destination, 0x4);
        }

        // This function will calculate jumping to a specified area of memory.
        public static int JumpToAddress(int startAddress, int destinationAddress)
        {
            // We add 0x5 because a jump is five bytes long.  One for the instruction and four for the address.
            if (destinationAddress > startAddress)
            {
                return destinationAddress - (startAddress + 0x5);
            }
            else
            {
                return (startAddress + 0x5) - destinationAddress;
            }
        }

        // This will get the client size of a window handle.
        public static Rectangle GetClientSize(this int processId)
        {
            Rect rect = new Rect();

            IntPtr windowHandle = processId.GetProcessWindowHandle();

            if (windowHandle != IntPtr.Zero)
            {
                UnsafeNativeMethods.GetClientRect(windowHandle, out rect);
                return rect.AsRectangle;
            }
            else
            {
                return new Rectangle();
            }
        }

        // This will get the client size of a window.
        public static Rectangle GetClientSizeByWindowTitle(this string windowTitle)
        {
            Rect rect = new Rect();

            IntPtr windowHandle = UnsafeNativeMethods.FindWindow(string.Empty, windowTitle);

            if (windowHandle != IntPtr.Zero)
            {
                UnsafeNativeMethods.GetClientRect(windowHandle, out rect);
                return rect.AsRectangle;
            }
            else
            {
                return new Rectangle();
            }
        }

        public static byte[] Assemble(string source)
        {
            string windowsTempPath = System.IO.Path.GetTempPath();
            string yasmPath = AppDomain.CurrentDomain.BaseDirectory + @"\yasm.exe";

            string use32prefix = string.Format(
                CultureInfo.InvariantCulture,
                @"bits 32
                  {0}",
                  source);

            System.IO.File.WriteAllText(windowsTempPath + @"widescreenfixer.yasm", use32prefix.Trim());

            using (Process process = new Process())
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo();
                processStartInfo.FileName = yasmPath;
                processStartInfo.Arguments = string.Format(CultureInfo.InvariantCulture, @"-a x86 -f bin -m x86 -o ""{0}widescreenfixer.bin"" ""{0}widescreenfixer.yasm""", windowsTempPath);
                process.StartInfo = processStartInfo;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.Start();
                process.WaitForExit();

                byte[] assembledBytes = System.IO.File.ReadAllBytes(windowsTempPath + @"widescreenfixer.bin");

                System.IO.File.Delete(windowsTempPath + @"widescreenfixer.yasm");
                System.IO.File.Delete(windowsTempPath + @"widescreenfixer.bin");

                return assembledBytes;
            }
        }

        public static byte[] Assemble64(string source)
        {
            string windowsTempPath = System.IO.Path.GetTempPath();
            string yasmPath = AppDomain.CurrentDomain.BaseDirectory + @"\yasm.exe";

            string use32prefix = string.Format(
                CultureInfo.InvariantCulture,
                @"bits 64
                  {0}",
                  source);

            System.IO.File.WriteAllText(windowsTempPath + @"widescreenfixer.yasm", use32prefix.Trim());

            using (Process process = new Process())
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo();
                processStartInfo.FileName = yasmPath;
                processStartInfo.Arguments = string.Format(CultureInfo.InvariantCulture, @"-a x86 -f bin -m amd64 -o ""{0}widescreenfixer.bin"" ""{0}widescreenfixer.yasm""", windowsTempPath);
                process.StartInfo = processStartInfo;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.Start();
                process.WaitForExit();

                byte[] assembledBytes = System.IO.File.ReadAllBytes(windowsTempPath + @"widescreenfixer.bin");

                System.IO.File.Delete(windowsTempPath + @"widescreenfixer64.yasm");
                System.IO.File.Delete(windowsTempPath + @"widescreenfixer64.bin");

                return assembledBytes;
            }
        }

        public static short GetAsynchronousKeyState(int key)
        {
            if (key > 0)
            {
                return UnsafeNativeMethods.GetAsyncKeyState(key);
            }

            return 0;
        }

        #endregion

        #region DLL Injection

        /*
        public static Boolean Inject(Int32 gameProcessID, String dllPath)
        {
            IntPtr windowHandle = IntPtr.Zero;
            if (!CreateRemoteThread(gameProcessID, dllPath, out windowHandle))
            {
                if (windowHandle != (IntPtr)0)
                    CloseMemory(windowHandle);
                return false;
            }
            return true;
        }

        private static Boolean CreateRemoteThread(Int32 gameProcessID, String dllPath, out IntPtr windowHandle)
        {
            IntPtr gameProcess = OpenProcess(0x2 | 0x8 | 0x10 | 0x20 | 0x400, true, gameProcessID);

            windowHandle = gameProcess;

            if (gameProcess == (IntPtr)0)
                return false;

            IntPtr loadLibraryAddressPointer = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");

            if (loadLibraryAddressPointer == (IntPtr)0)
                return false;

            IntPtr addressPointer = VirtualAllocEx(
                gameProcess,
                (IntPtr)null,
                (IntPtr)dllPath.Length,
                (UInt32)0x1000 | (UInt32)0x2000,
                (UInt32)0x40);

            if (addressPointer == (IntPtr)0)
                return false;

            Byte[] bytes = CalculateBytes(dllPath);
            IntPtr tempPointer = IntPtr.Zero;

            WriteMemory(gameProcess, addressPointer, bytes, (UInt32)bytes.Length, ref tempPointer);

            IntPtr threadPointer = CreateRemoteThread(
                gameProcess,
                (IntPtr)null,
                (IntPtr)0,
                loadLibraryAddressPointer,
                addressPointer,
                0,
                (IntPtr)null);

            if (threadPointer == (IntPtr)0)
                return false;

            return true;
        }

        private static Byte[] CalculateBytes(String stringToConvert)
        {
            Byte[] byteReturn = Encoding.ASCII.GetBytes(stringToConvert);
            return byteReturn;
        }
         */

        #endregion

        #region Private Methods

        // This will get the integer value of the hex value provided.
        private static int GetHexValue(char hex)
        {
            int value = (int)hex;

            // For uppercase A-F letters:
            return value - (value < 58 ? 48 : 55);

            //// For lowercase a-f letters:
            //// return val - (val < 58 ? 48 : 87);
            //// Or the two combined, but a bit slower:
            //// return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }

        // Compares two byte arrays in an unsafe, but very fast way.
        // http://techmikael.blogspot.com/2009/01/fast-byte-array-comparison-in-c.html
        private static unsafe bool UnsafeEquals(byte[] byteArrayA, byte[] byteArrayB)
        {
            int length = byteArrayA.Length;

            if (length != byteArrayB.Length)
            {
                return false;
            }

            fixed (byte* byteArray1 = byteArrayA)
            {
                byte* charPointer1 = byteArray1;

                fixed (byte* byteArray2 = byteArrayB)
                {
                    byte* charPointer2 = byteArray2;
                    byte* charPointer3 = charPointer1;
                    byte* charPointer4 = charPointer2;

                    while (length >= 10)
                    {
                        if ((((*(((int*)charPointer3)) != *(((int*)charPointer4)))
                            || (*(((int*)(charPointer3 + 2))) != *(((int*)(charPointer4 + 2)))))
                            || ((*(((int*)(charPointer3 + 4))) != *(((int*)(charPointer4 + 4))))
                            || (*(((int*)(charPointer3 + 6))) != *(((int*)(charPointer4 + 6))))))
                            || (*(((int*)(charPointer3 + 8))) != *(((int*)(charPointer4 + 8)))))
                        {
                            break;
                        }

                        charPointer3 += 10;
                        charPointer4 += 10;
                        length -= 10;
                    }

                    while (length > 0)
                    {
                        if (*(((int*)charPointer3)) != *(((int*)charPointer4)))
                        {
                            break;
                        }

                        charPointer3 += 2;
                        charPointer4 += 2;
                        length -= 2;
                    }

                    return (length <= 0);
                }
            }
        }

        // This will get the main window handle for the specified process
        private static IntPtr GetProcessWindowHandle(this int processId)
        {
            if (processId > 0)
            {
                System.Diagnostics.Process gameProcess = System.Diagnostics.Process.GetProcessById(processId);
                return gameProcess.MainWindowHandle;
            }
            else
            {
                return IntPtr.Zero;
            }
        }

        #endregion

        #region Public Structures

        [StructLayout(LayoutKind.Sequential)]
        public struct ModuleInfo
        {
            public static readonly int SizeOf;

            public IntPtr BaseOfDll;
            public int SizeOfImage;
            public IntPtr EntryPoint;

            static ModuleInfo()
            {
                SizeOf = Marshal.SizeOf(typeof(ModuleInfo));
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ParentProcessUtilities
        {
            internal IntPtr ExitStatus;
            internal IntPtr PebBaseAddress;
            internal IntPtr AffinityMask;
            internal IntPtr BasePriority;
            internal IntPtr UniqueProcessId;
            internal IntPtr InheritedFromUniqueProcessId;

            public static System.Diagnostics.Process GetParentProcess()
            {
                return GetParentProcess(System.Diagnostics.Process.GetCurrentProcess().Handle);
            }

            public static System.Diagnostics.Process GetParentProcess(int id)
            {
                System.Diagnostics.Process process = System.Diagnostics.Process.GetProcessById(id);
                return GetParentProcess(process.Handle);
            }

            public static System.Diagnostics.Process GetParentProcess(IntPtr handle)
            {
                ParentProcessUtilities pbi = new ParentProcessUtilities();

                int returnLength;

                int status = NtQueryInformationProcess(handle, 0, ref pbi, Marshal.SizeOf(pbi), out returnLength);

                if (status != 0)
                {
                    throw new System.ComponentModel.Win32Exception(status);
                }

                try
                {
                    return System.Diagnostics.Process.GetProcessById(pbi.InheritedFromUniqueProcessId.ToInt32());
                }
                catch (ArgumentException)
                {
                    return null;
                }
            }

            [DllImport("ntdll.dll")]
            private static extern int NtQueryInformationProcess(
                [In] IntPtr processHandle,
                [In] int processInformationClass,
                [In] [Out] ref ParentProcessUtilities processInformation,
                [In] int processInformationLength,
                [Out] [Optional] out int returnLength);
        }

        #endregion

        #region Private Structures

        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            private int Left;
            private int Top;
            private int Right;
            private int Bottom;

            public Rect(int left, int top, int right, int bottom)
            {
                this.Left = left;
                this.Top = top;
                this.Right = right;
                this.Bottom = bottom;
            }

            public Rectangle AsRectangle
            {
                get
                {
                    return new Rectangle(this.Left, this.Top, this.Right - this.Left, this.Bottom - this.Top);
                }
            }
        }

        #endregion
    }

    public static class Enumerations
    {
        // This enum contains the flags for standard access rights
        public enum StandardRights : long
        {
            // No rights
            None = 0x00000000,

            // The right to delete the object.
            Delete = 0x00010000,

            // The right to read data from the security descriptor of the object, not including the data in the SACL.
            ReadControl = 0x00020000,

            // The right to modify the discretionary access-control list (DACL) in the object security descriptor.
            WriteDac = 0x00040000,

            // The right to assume ownership of the object. The user must be an object trustee. The user cannot transfer the ownership to other users.
            WriteOwner = 0x00080000,

            // The right to use the object for synchronization. This enables a thread to wait until the object is in the signaled state.
            Synchronize = 0x00100000,

            // I could not find documentation on this.
            Required = 0x000f0000,

            // I could not find documentation on this.
            Read = ReadControl,

            // I could not find documentation on this.
            Write = ReadControl,

            // I could not find documentation on this.
            Execute = ReadControl,

            // I could not find documentation on this.
            All = 0x001f0000,

            // I could not find documentation on this.
            SpecificRightsAll = 0x0000ffff,

            // The right to get or set the SACL in the object security descriptor.
            AccessSystemSecurity = 0x01000000,

            // I could not find documentation on this.
            MaximumAllowed = 0x02000000,

            // The right to read permissions on this object, read all the properties on this object, list this object name when the parent container is listed, and list the contents of this object if it is a container.
            GenericRead = 0x80000000,

            // The right to read permissions on this object, write all the properties on this object, and perform all validated writes to this object.
            GenericWrite = 0x40000000,

            // The right to read permissions on, and list the contents of, a container object.
            GenericExecute = 0x20000000,

            // The right to create or delete child objects, delete a subtree, read and write properties, examine child objects and the object itself, add and remove the object from the directory, and read or write with an extended right.
            GenericAll = 0x10000000
        }

        // This enum contains the flags for process access
        public enum ProcessAccess : long
        {
            None = 0x0000,
            Terminate = 0x0001,
            CreateThread = 0x0002,
            SetSessionId = 0x0004,
            VmOperation = 0x0008,
            VmRead = 0x0010,
            VmWrite = 0x0020,
            VmAll = 0x0038, // VmOperation | VmRead | VmWrite
            DupHandle = 0x0040,
            CreateProcess = 0x0080,
            SetQuota = 0x0100,
            SetInformation = 0x0200,
            QueryInformation = 0x0400,
            SetPort = 0x0800,
            SuspendResume = 0x0800,
            QueryLimitedInformation = 0x1000,

            // Should be 0x1fff on Vista but is 0xfff for backwards compatibility
            All = StandardRights.Required | StandardRights.Synchronize | 0xfff
        }

        // This enum contains the flags for module filters
        public enum ModuleFilter : int
        {
            None = 0x00,
            X32Bit = 0x01,
            X64Bit = 0x02,
            All = 0x03
        }

        // This enum contains the flags for memory state
        [Flags]
        public enum MemoryStates : int
        {
            ReadWrite = 0x4,
            Commit = 0x1000,
            Reserve = 0x2000,
            Decommit = 0x4000,
            Release = 0x8000,
            Free = 0x10000,
            Reset = 0x80000,
            Physical = 0x400000,
            LargePages = 0x20000000
        }

        [Flags]
        public enum FreeTypes : int
        {
            Decommit = 0x4000,
            Release = 0x8000
        }

        // This enum contains the flags for memory protection
        [Flags]
        public enum MemoryProtections : int
        {
            None = 0x00,
            Execute = 0x10,
            ExecuteRead = 0x20,
            ExecuteReadWrite = 0x40,
            ExecuteWriteCopy = 0x80,
            Guard = 0x100,
            NoCache = 0x200,
            WriteCombine = 0x400,
            NoAccess = 0x01,
            ReadOnly = 0x02,
            ReadWrite = 0x04,
            WriteCopy = 0x08
        }

        // This enum contains the flags for process creation
        [Flags]
        public enum ProcessCreation : long
        {
            DebugProcess = 0x1,
            DebugOnlyThisProcess = 0x2,
            CreateSuspended = 0x4,
            DetachedProcess = 0x8,
            CreateNewConsole = 0x10,
            NormalPriorityClass = 0x20,
            IdlePriorityClass = 0x40,
            HighPriorityClass = 0x80,
            RealtimePriorityClass = 0x100,
            CreateNewProcessGroup = 0x200,
            CreateUnicodeEnvironment = 0x400,
            CreateSeparateWowVdm = 0x800,
            CreateSharedWowVdm = 0x1000,
            CreateForceDos = 0x2000,
            BelowNormalPriorityClass = 0x4000,
            AboveNormalPriorityClass = 0x8000,
            StackSizeParamIsAReservation = 0x10000,
            InheritCallerPriority = 0x20000,
            CreateProtectedProcess = 0x40000,
            ExtendedStartupInfoPresent = 0x80000,
            ProcessModeBackgroundBegin = 0x100000,
            ProcessModeBackgroundEnd = 0x200000,
            CreateBreakawayFromJob = 0x1000000,
            CreatePreserveCodeAuthzLevel = 0x2000000,
            CreateDefaultErrorMode = 0x4000000,
            CreateNoWindow = 0x8000000,
            ProfileUser = 0x10000000,
            ProfileKernel = 0x20000000,
            ProfileServer = 0x40000000,
            CreateIgnoreSystemDefault = 0x80000000
        }
    }

    public static class UnsafeNativeMethods
    {
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern short GetAsyncKeyState(
            [In] int keyInt);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr GetModuleHandle(
            [MarshalAs(UnmanagedType.LPWStr)]
            [In] [Optional] string moduleName);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool VirtualProtectEx(
            [In] IntPtr process,
            [In] IntPtr baseAddress,
            [In] UIntPtr size,
            [In] Enumerations.MemoryProtections newProtect,
            [Out] out Enumerations.MemoryProtections oldProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool VirtualFreeEx(
            [In] IntPtr process,
            [In] IntPtr baseAddress,
            [In] UIntPtr size,
            [In] Enumerations.FreeTypes freeType);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DestroyIcon(
            [In] IntPtr handle);

        /*
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern IntPtr FindWindowEx(
            [In] IntPtr parentHandle,
            [In] IntPtr childHandle,
            [In] string className,
            [In] string windowName);
         */

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool IsWow64Process(
            [In] IntPtr process,
            [Out] [MarshalAs(UnmanagedType.Bool)] out bool systemInfo);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CloseHandle(
            [In] IntPtr handle);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetClientRect(
            [In] IntPtr handle,
            [Out] out ProcessFunctions.Rect rect);

        /*
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetWindowRect(
            [In] IntPtr handle,
            [Out] out Rect rect);
         */

        /*
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        internal static extern IntPtr GetProcAddress(
            [In] IntPtr module,
            [In] string processName);
         */

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr OpenProcess(
            [In] IntPtr desiredAccess,
            [In] [MarshalAs(UnmanagedType.Bool)] bool inheritHandle,
            [In] int processId);

        [DllImport("psapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern int GetModuleBaseName(
            [In] IntPtr process,
            [In] [Optional] IntPtr moduleHandle,
            [Out] StringBuilder baseName,
            [In] int size);

        [DllImport("psapi.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetModuleInformation(
            [In] IntPtr process,
            [In] [Optional] IntPtr moduleHandle,
            [Out] IntPtr moduleInfo, // out ModuleInfo moduleInfo
            [In] int size);

        [DllImport("psapi.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool EnumProcessModulesEx(
            [In] IntPtr process,
            [Out] IntPtr[] moduleHandles,
            [In] int size,
            [Out] out int requiredSize,
            [In] Enumerations.ModuleFilter filterFlags);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ReadProcessMemory(
            [In] IntPtr process,
            [In] IntPtr baseAddress,
            [Out] byte[] buffer,
            [In] IntPtr size,
            [Out] IntPtr bytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ReadProcessMemory(
            [In] IntPtr process,
            [In] UIntPtr baseAddress,
            [Out] byte[] buffer,
            [In] IntPtr size,
            [Out] IntPtr bytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ReadProcessMemory(
            [In] IntPtr process,
            [In] IntPtr baseAddress,
            [Out] IntPtr buffer,
            [In] IntPtr size,
            [Out] IntPtr bytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ReadProcessMemory(
            [In] IntPtr process,
            [In] UIntPtr baseAddress,
            [Out] IntPtr buffer,
            [In] IntPtr size,
            [Out] IntPtr bytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ReadProcessMemory(
            [In] IntPtr process,
            [In] IntPtr baseAddress,
            [Out] byte[] buffer,
            [In] IntPtr size,
            [Out] out int bytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ReadProcessMemory(
            [In] IntPtr process,
            [In] UIntPtr baseAddress,
            [Out] byte[] buffer,
            [In] IntPtr size,
            [Out] out int bytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool WriteProcessMemory(
            [In] IntPtr process,
            [In] IntPtr baseAddress,
            [In] byte[] buffer,
            [In] IntPtr size,
            [Out] IntPtr bytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool WriteProcessMemory(
            [In] IntPtr process,
            [In] UIntPtr baseAddress,
            [In] byte[] buffer,
            [In] IntPtr size,
            [Out] IntPtr bytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool WriteProcessMemory(
            [In] IntPtr process,
            [In] IntPtr baseAddress,
            [In] IntPtr buffer,
            [In] IntPtr size,
            [Out] IntPtr bytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool WriteProcessMemory(
            [In] IntPtr process,
            [In] UIntPtr baseAddress,
            [In] IntPtr buffer,
            [In] IntPtr size,
            [Out] IntPtr bytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool WriteProcessMemory(
            [In] IntPtr process,
            [In] IntPtr baseAddress,
            [In] byte[] buffer,
            [In] IntPtr size,
            [Out] out int bytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool WriteProcessMemory(
            [In] IntPtr process,
            [In] UIntPtr baseAddress,
            [In] byte[] buffer,
            [In] IntPtr size,
            [Out] out int bytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr VirtualAllocEx(
            [In] IntPtr process,
            [In] [Optional] IntPtr baseAddress,
            [In] IntPtr size,
            [In] Enumerations.MemoryStates type,
            [In] Enumerations.MemoryProtections protect);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern UIntPtr VirtualAllocEx(
            [In] IntPtr process,
            [In] [Optional] UIntPtr baseAddress,
            [In] IntPtr size,
            [In] Enumerations.MemoryStates type,
            [In] Enumerations.MemoryProtections protect);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern IntPtr FindWindow(
            [In] string className,
            [In] string windowName);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern int GetWindowThreadProcessId(
            [In] IntPtr windowHandle,
            [Out] [Optional] out int processId);

        /*
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr CreateRemoteThread(
            [In] IntPtr process,
            [In] IntPtr threadAttributes,
            [In] IntPtr stackSize,
            [In] IntPtr startAddress,
            [In] IntPtr parameter,
            [In] ProcessCreationFlags creationFlags,
            [Out] out int threadId);
         */
    }
}
