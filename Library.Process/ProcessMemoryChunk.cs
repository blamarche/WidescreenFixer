#region File Information
/*
 * Copyright (C) 2006 Michael Schierl
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
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Text;

    // A chunk in another processes memory. Mostly used to allocate buffers
    // in another process for sending messages to its windows.
    public class ProcessMemoryChunk : IDisposable
    {
        readonly Process process;
        readonly IntPtr location, hProcess;
        readonly int size;
        readonly bool free;

        // Create a new memory chunk that points to existing memory.
        // Mostly used to read that memory.
        public ProcessMemoryChunk(Process process, IntPtr location, int size)
        {
            if (process != null)
            {
                this.process = process;
                this.hProcess = UnsafeNativeMethods.OpenProcess(new IntPtr((long)Enumerations.ProcessAccess.VmOperation | (long)Enumerations.ProcessAccess.VmRead | (long)Enumerations.ProcessAccess.VmWrite), false, process.Id);
                ApiHelper.FailIfZero(this.hProcess);
                this.location = location;
                this.size = size;
                this.free = false;
            }
        }

        private ProcessMemoryChunk(Process process, IntPtr hProcess, IntPtr location, int size, bool free)
        {
            if (process != null)
            {
                this.process = process;
                this.hProcess = hProcess;
                this.location = location;
                this.size = size;
                this.free = free;
            }
        }

        // The process this chunk refers to.
        public Process Process { get { return this.process; } }

        // The location in memory (of the other process) this chunk refers to.
        public IntPtr Location { get { return this.location; } }

        // The size of the chunk.
        public int Size { get { return this.size; } }

        // Allocate a chunk in another process.
        public static ProcessMemoryChunk Allocate(Process process, int size)
        {
            if (process != null)
            {
                IntPtr hProcess = UnsafeNativeMethods.OpenProcess(new IntPtr((long)Enumerations.ProcessAccess.VmOperation | (long)Enumerations.ProcessAccess.VmRead | (long)Enumerations.ProcessAccess.VmWrite), false, process.Id);
                IntPtr remotePointer = UnsafeNativeMethods.VirtualAllocEx(
                    hProcess,
                    IntPtr.Zero,
                    new IntPtr(size),
                    Enumerations.MemoryStates.Commit | Enumerations.MemoryStates.Reserve,
                    Enumerations.MemoryProtections.ReadWrite);
                ApiHelper.FailIfZero(remotePointer);
                return new ProcessMemoryChunk(process, hProcess, remotePointer, size, true);
            }

            return null;
        }

        // Allocate a chunk in another process and unmarshal a struct there.
        public static ProcessMemoryChunk AllocateStruct(Process process, object structure)
        {
            int size = Marshal.SizeOf(structure);
            ProcessMemoryChunk result = Allocate(process, size);
            result.WriteStructure(0, structure);
            return result;
        }

        // Free the memory in the other process, if it has been allocated before.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ ProcessMemoryChunk()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.TryToVirtualFreeEx();

                UnsafeNativeMethods.CloseHandle(this.hProcess);
            }
        }

        private void TryToVirtualFreeEx()
        {
            if (this.free)
            {
                if (!UnsafeNativeMethods.VirtualFreeEx(
                    this.hProcess,
                    this.location,
                    UIntPtr.Zero,
                    Enumerations.FreeTypes.Release))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
            }
        }

        // Write a structure into this chunk.
        public void WriteStructure(int offset, object structure)
        {
            int structureSize = Marshal.SizeOf(structure);
            IntPtr localPtr = Marshal.AllocHGlobal(structureSize);
            try
            {
                Marshal.StructureToPtr(structure, localPtr, false);
                this.Write(offset, localPtr, structureSize);
            }
            finally
            {
                Marshal.FreeHGlobal(localPtr);
            }
        }

        // Write into this chunk.
        public void Write(int offset, IntPtr addressValue, int length)
        {
            if (offset < 0) throw new ArgumentException("Offset may not be negative", "offset");
            if (offset + length > this.size) throw new ArgumentException("Exceeding chunk size");
            UnsafeNativeMethods.WriteProcessMemory(this.hProcess, new IntPtr(this.location.ToInt64() + offset), addressValue, new IntPtr(length), IntPtr.Zero);
        }

        // Write a byte array into this chunk.
        public void Write(int offset, byte[] addressValue)
        {
            if (addressValue != null && addressValue.Length > 0)
            {
                if (offset < 0) throw new ArgumentException("Offset may not be negative", "offset");
                if (offset + addressValue.Length > this.size) throw new ArgumentException("Exceeding chunk size");
                UnsafeNativeMethods.WriteProcessMemory(this.hProcess, new IntPtr(this.location.ToInt64() + offset), addressValue, new IntPtr(addressValue.Length), IntPtr.Zero);
            }
        }

        // Read this chunk.
        public byte[] Read() { return this.Read(0, this.size); }

        // Read a part of this chunk.
        public byte[] Read(int offset, int length)
        {
            if (offset + length > this.size) throw new ArgumentException("Exceeding chunk size");
            byte[] result = new byte[length];
            UnsafeNativeMethods.ReadProcessMemory(this.hProcess, new IntPtr(this.location.ToInt64() + offset), result, new IntPtr(length), IntPtr.Zero);
            return result;
        }

        // Read this chunk to a pointer in this process.
        public void ReadToPointer(IntPtr addressValue)
        {
            this.ReadToPointer(0, this.size, addressValue);
        }

        // Read a part of this chunk to a pointer in this process.
        public void ReadToPointer(int offset, int length, IntPtr addressValue)
        {
            if (offset + length > this.size) throw new ArgumentException("Exceeding chunk size");
            UnsafeNativeMethods.ReadProcessMemory(this.hProcess, new IntPtr(this.location.ToInt64() + offset), addressValue, new IntPtr(length), IntPtr.Zero);
        }

        // Read a part of this chunk to a structure.
        public object ReadToStructure(int offset, Type structureType)
        {
            int structureSize = Marshal.SizeOf(structureType);
            IntPtr localPtr = Marshal.AllocHGlobal(structureSize);
            try
            {
                this.ReadToPointer(offset, structureSize, localPtr);
                return Marshal.PtrToStructure(localPtr, structureType);
            }
            finally
            {
                Marshal.FreeHGlobal(localPtr);
            }
        }
    }
}
