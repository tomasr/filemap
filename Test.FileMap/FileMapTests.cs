
//
// FileMapTests.cs
//
// Author:
//    Tomas Restrepo (tomasr@mvps.org)
//

using System;
using System.IO;
using System.Reflection;
using System.Text;
using NUnit.Framework;

using Winterdom.IO.FileMap;

namespace Winterdom.IO.FileMap.Tests 
{

   [ TestFixture ]
   public class FileMapTests
   {
      const int MAX_BYTES = 8*1024;

      /// <summary>
      /// Test simple writing and reading on non-backed MMF
      /// </summary>
      [ Test ]
      public void TestNonBackedMMF()
      {
         try {
            MemoryMappedFile map = 
               MemoryMappedFile.Create(MapProtection.PageReadWrite, MAX_BYTES);

            using ( Stream view = map.MapView(MapAccess.FileMapWrite, 0, MAX_BYTES) )
            {
               WriteBytesToStream(view, MAX_BYTES);
            }
            using ( Stream view = map.MapView(MapAccess.FileMapRead, 0, MAX_BYTES/2) )
            {
               VerifyBytesInStream(view, MAX_BYTES/2);
            }
            map.Close();

         } catch ( FileMapIOException e ) {
            Assert.Fail("Write error: " + e);
         }
      }

      /// <summary>
      /// Test named MMF
      /// </summary>
      [ Test ]
      public void TestNamedMMF()
      {
         try {
            const string NAME = "MyMappedFile";
            MemoryMappedFile map = 
               MemoryMappedFile.Create(MapProtection.PageReadWrite, MAX_BYTES, NAME);
               
               MemoryMappedFile map2 = 
                  MemoryMappedFile.Open(MapAccess.FileMapRead, NAME);
               map2.Close();
            map.Close();

         } catch ( FileMapIOException e ) {
            Assert.Fail("Failed Named MMF: " + e);
         }
      }


      /// <summary>
      /// Test simple writing and reading
      /// </summary>
      [ Test ]
      public void TestMMFFileWrite()
      {
         try {
            string filename = Path.GetTempFileName();
            MemoryMappedFile map = 
               MemoryMappedFile.Create(filename, MapProtection.PageReadWrite, MAX_BYTES);

            using ( Stream view = map.MapView(MapAccess.FileMapWrite, 0, MAX_BYTES) )
            {
               WriteBytesToStream(view, MAX_BYTES);
            }
            using ( Stream view = map.MapView(MapAccess.FileMapRead, 0, MAX_BYTES/2) )
            {
               VerifyBytesInStream(view, MAX_BYTES/2);
            }
            map.Close();

         } catch ( FileMapIOException e ) {
            Assert.Fail("Write error: " + e);
         }
      }

      /// <summary>
      /// Test the position property
      /// </summary>
      [ Test ]
      public void TestMMFViewSeeking()
      {
         string filename = Path.GetTempFileName();
         MemoryMappedFile map = 
            MemoryMappedFile.Create(filename, MapProtection.PageReadWrite, MAX_BYTES);

         using ( Stream view = map.MapView(MapAccess.FileMapWrite, 0, MAX_BYTES) )
         {
            view.WriteByte(0x12);
            // seek from start of view
            view.Seek(12, SeekOrigin.Begin);
            Assert.AreEqual(12, (int)view.Position);
            // Seek from current pos
            view.Seek(1, SeekOrigin.Current);
            Assert.AreEqual(13, (int)view.Position);
            // seek from end
            view.Seek(-1, SeekOrigin.End);
            Assert.AreEqual(MAX_BYTES-1, (int)view.Position);

            try {
               // no seeking past end of stream
               view.Seek(1, SeekOrigin.End);
               Assert.Fail("Seeked past end of stream");
            } catch ( FileMapIOException ) {
            }

         }
         map.Close();
      }


      /// <summary>
      /// Verifies that you can't read or write beyond
      /// a view size
      /// </summary>
      [ Test ]
      public void TestMMFViewSize()
      {
         string filename = Path.GetTempFileName();
         MemoryMappedFile map = 
            MemoryMappedFile.Create(filename, MapProtection.PageReadWrite, MAX_BYTES);

         using ( Stream view = map.MapView(MapAccess.FileMapWrite, 0, MAX_BYTES) )
         {
            view.Seek(MAX_BYTES, SeekOrigin.Begin);
            // no writing past end of view
            view.WriteByte(0x01);
            Assert.AreEqual(MAX_BYTES, (int)view.Position);
            // no reading past end of stream
            Assert.AreEqual(-1, view.ReadByte());
         }
         map.Close();
      }


      #region Private Methods

      private void WriteBytesToStream(Stream stream, int count)
      {
         for ( int i=0; i < count; i++ ) {
            stream.WriteByte((byte)(i/255));
         }
      }

      private void VerifyBytesInStream(Stream stream, int count)
      {
         for ( int i=0; i < count; i++ ) {
            int b = stream.ReadByte();
            if ( b == -1 )
               Assert.Fail("Reached end of stream prematurely");
            Assert.AreEqual((byte)(i/255), (byte)b);
         }
      }

      #endregion // Private Methods


   } // class FileMapTests

} // namespace Winterdom.IO.FileMap.Tests 

