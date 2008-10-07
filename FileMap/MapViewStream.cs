
//
// MapViewStream.cs
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

   /// <summary>
   ///   Specifies page protection for the mapped file
   ///   These correspond to the PAGE_XXX set of flags
   ///   passed to CreateFileMapping()
   /// </summary>
   [ Flags ]
   public enum MapProtection 
   {
      PageNone       = 0x00000000,
      // protection
      PageReadOnly   = 0x00000002,
      PageReadWrite  = 0x00000004,
      PageWriteCopy  = 0x00000008,
      // attributes
      SecImage       = 0x01000000,
      SecReserve     = 0x04000000,
      SecCommit      = 0x08000000,
      SecNoCache     = 0x10000000,
   }


   /// <summary>
   ///   Allows you to read/write from/to
   ///   a view of a memory mapped file.
   /// </summary>
   public class MapViewStream : Stream, IDisposable
   {

      #region variables

      //! What's our access?
      MapProtection _protection = MapProtection.PageNone;
      //! base address of our buffer
      IntPtr _base = IntPtr.Zero;
      //! our current buffer length
      long _length = 0;
      //! our current position in the stream buffer
      long _position = 0;
      //! are we open?
      bool _isOpen = false;

      #endregion // variables



      /// <summary>
      /// Constructor used internally by MemoryMappedFile.
      /// </summary>
      /// <param name="baseAddress">base address where the view starts</param>
      /// <param name="length">Length of view, in bytes</param>
      /// <param name="protection"></param>
      internal MapViewStream ( IntPtr baseAddress, long length, 
                               MapProtection protection )
      {
         _base = baseAddress;
         _length = length;
         _protection = protection;
         _position = 0;
         _isOpen = (baseAddress != IntPtr.Zero);
      }
      ~MapViewStream()
      {
         Dispose(false);
      }


      #region Properties

      public override bool CanRead  
      {
         get { return true; }
      }
      public override bool CanSeek  {
         get { return true; }
      }
      public override bool CanWrite  {
         get { return (((int)_protection) & 0x000000C) != 0; }
      }
      public override long Length  {
         get { return _length; }
      }
      public override long Position  {
         get { return _position; }
         set { Seek(value, SeekOrigin.Begin); }
      }
      private bool IsOpen  {
         get { return _isOpen; }
         set { _isOpen = value; }
      }

      #endregion // Properties


      #region Stream Methods

      public override void Flush() 
      {
         if ( !IsOpen ) 
            throw new ObjectDisposedException ( "Stream is closed" );
         // flush the view but leave the buffer intact
         // FIX: get rid of cast
         Win32MapApis.FlushViewOfFile ( _base, (int)_length );
      }

      public override int Read ( byte[] buffer, int offset, int count )
      {
         if ( !IsOpen ) 
            throw new ObjectDisposedException ( "Stream is closed" );

         if ( buffer.Length - offset < count  )
            throw new ArgumentException ( "Invalid Offset" );

         int bytesToRead = (int)Math.Min(Length-_position, count);
         Marshal.Copy((IntPtr)(_base.ToInt64()+_position), buffer, offset, bytesToRead);

         _position += bytesToRead;
         return bytesToRead;
      }
    
      public override void Write ( byte[] buffer, int offset, int count ) 
      {
         if ( !this.IsOpen ) 
            throw new ObjectDisposedException("Stream is closed");
         if ( !this.CanWrite ) 
            throw new FileMapIOException ( "Stream cannot be written to" );

         if ( buffer.Length - offset < count  )
            throw new ArgumentException ( "Invalid Offset" );
      
         int bytesToWrite = (int)Math.Min(Length-_position, count);
         if ( bytesToWrite==0 ) 
            return;

         Marshal.Copy(buffer, offset, (IntPtr)(_base.ToInt64()+_position), bytesToWrite);

         _position += bytesToWrite;
      }

      public override long Seek ( long offset, SeekOrigin origin )
      {
         if ( !IsOpen ) 
            throw new ObjectDisposedException ( "Stream is closed" );

         long newpos = 0;
         switch ( origin )
         {
         case SeekOrigin.Begin:    newpos = offset;             break;
         case SeekOrigin.Current:  newpos = Position + offset;  break;
         case SeekOrigin.End:      newpos = Length + offset;    break;
         }
         // sanity check
         if ( newpos < 0 || newpos > Length )
            throw new FileMapIOException ( "Invalid Seek Offset" );
         _position = newpos;

         return newpos;
      }

      public override void SetLength ( long value )
      {
         // not supported!
         throw new NotSupportedException ( "Can't change View Size" );
      }

      public override void Close()
      {
         Dispose(true);
      }

      #endregion // Stream methods


      #region IDisposable Implementation

      public void Dispose()
      {
         Dispose(true);
      }

      protected virtual void Dispose(bool disposing)
      {
         if ( IsOpen ) 
         {
            Flush();
            // FIX: eliminate cast
            Win32MapApis.UnmapViewOfFile ( _base );
            IsOpen = false;
         }

         if ( disposing )
            GC.SuppressFinalize(this);
      }

      #endregion // IDisposable Implementation

   } // class MapViewStream

} // namespace Winterdom.IO.FileMap
