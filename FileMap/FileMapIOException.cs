
//
// FileMapIOException.cs
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

   /// <summary>Exception class thrown by the library</summary>
   /// <remarks>
   ///   Represents an exception occured as a result of an
   ///   invalid IO operation on any of the File mapping classes
   ///   It wraps the error message and the underlying Win32 error
   ///   code that caused the error.
   /// </remarks>
   // TODO: Make Serializable!
   public class FileMapIOException : IOException
   {
      //
      // properties
      //
      private int m_win32Error = 0;
      public int Win32ErrorCode
      {
         get { return m_win32Error; }
      }
      public override string Message
      {
         get 
         {
            if ( Win32ErrorCode != 0 )
               return base.Message + " (" + Win32ErrorCode + ")";
            else
               return base.Message;
         }
      }

      // construction
      public FileMapIOException ( int error ) : base()
      {
         m_win32Error = error;
      }
      public FileMapIOException ( string message ) : base(message)
      {
      }
      public FileMapIOException ( string message, Exception innerException ) 
               : base(message, innerException)
      {
      }

   } // class FileMapIOException


} // namespace Winterdom.IO.FileMap
