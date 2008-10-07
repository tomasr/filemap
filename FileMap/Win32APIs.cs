
//
// Win32APIs.cs
//    
//    Implementation of a library to use Win32 Memory Mapped
//    Files from within .NET applications
//
// COPYRIGHT (C) 2001, Tomas Restrepo (tomasr@mvps.org)
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//

using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Winterdom.IO.FileMap
{

   /// <summary>Win32 APIs used by the library</summary>
   /// <remarks>
   ///   Defines the PInvoke functions we use
   ///   to access the FileMapping Win32 APIs
   /// </remarks>
   internal class Win32MapApis
   {
      [ DllImport("kernel32", SetLastError=true, CharSet=CharSet.Auto) ]
      public static extern IntPtr CreateFile ( 
         String lpFileName, int dwDesiredAccess, int dwShareMode,
         IntPtr lpSecurityAttributes, int dwCreationDisposition,
         int dwFlagsAndAttributes, IntPtr hTemplateFile );

      [ DllImport("kernel32", SetLastError=true, CharSet=CharSet.Auto) ]
      public static extern IntPtr CreateFileMapping ( 
         IntPtr hFile, IntPtr lpAttributes, int flProtect, 
         int dwMaximumSizeLow, int dwMaximumSizeHigh,
         String lpName );

      [ DllImport("kernel32", SetLastError=true) ]
      public static extern bool FlushViewOfFile ( 
         IntPtr lpBaseAddress, int dwNumBytesToFlush );

      [ DllImport("kernel32", SetLastError=true) ]
      public static extern IntPtr MapViewOfFile (
         IntPtr hFileMappingObject, int dwDesiredAccess, int dwFileOffsetHigh,
         int dwFileOffsetLow, IntPtr dwNumBytesToMap );

      [ DllImport("kernel32", SetLastError=true, CharSet=CharSet.Auto) ]
      public static extern IntPtr OpenFileMapping (
         int dwDesiredAccess, bool bInheritHandle, String lpName );

      [ DllImport("kernel32", SetLastError=true) ]
      public static extern bool UnmapViewOfFile ( IntPtr lpBaseAddress );

      [ DllImport("kernel32", SetLastError=true) ]
      public static extern bool CloseHandle ( IntPtr handle );

   } // class Win32MapApis

} // namespace Winterdom.IO.FileMap
