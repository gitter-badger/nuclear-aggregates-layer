using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using SevenZip;

namespace DoubleGis.Erm.Platform.Common.Compression
{
    public static class StreamExtensions
    {
        static StreamExtensions()
        {
            // load native 7z.dll from platform-specific folder
            var baseDirectory = AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory;
            var platformSpecificDirectory = Environment.Is64BitProcess ? "x64" : "x86";
            var libraryPath = Path.Combine(baseDirectory, platformSpecificDirectory, "7z.dll");

            SevenZipBase.SetLibraryPath(libraryPath);
        }

        public static Stream ZipStreamDictionary(this Dictionary<string, Stream> streamDictionary)
        {
            var compressor = new SevenZipCompressor
            {
                ArchiveFormat = OutArchiveFormat.Zip,
                FastCompression = true
            };

            var zipStream = new MemoryStream();
            compressor.CompressStreamDictionary(streamDictionary, zipStream);
            zipStream.Position = 0;

            return zipStream;
        }

        public static IEnumerable<ArchiveFileInfo> ZipStreamFindFiles(this Stream stream, Func<ArchiveFileInfo, bool> predicate)
        {
            using (var sevenZipExtractor = new SevenZipExtractor(stream))
            {
                return sevenZipExtractor.ArchiveFileData.Where(predicate);
            }
        }

        public static IDictionary<string, Stream> UnzipStream(this Stream zipStream)
        {
            using (var extractor = new SevenZipExtractor(zipStream))
            {
                var fileNames = extractor.ArchiveFileNames;
                return fileNames.ToDictionary(fileName => fileName,
                                              fileName =>
                                                  {
                                                      Stream ms = new MemoryStream();
                                                      extractor.ExtractFile(fileName, ms);
                                                      ms.Seek(0, SeekOrigin.Begin);
                                                      return ms;
                                                  });
            }
        }

        public static IDictionary<string, Stream> UnzipStream(this Stream zipStream, Func<string, bool> predicate)
        {
            using (var extractor = new SevenZipExtractor(zipStream))
            {
                var fileNames = extractor.ArchiveFileNames.Where(predicate);
                return fileNames.ToDictionary(fileName => fileName,
                                              fileName =>
                                              {
                                                  Stream ms = new MemoryStream();
                                                  extractor.ExtractFile(fileName, ms);
                                                  ms.Seek(0, SeekOrigin.Begin);
                                                  return ms;
                                              });
            }
        }
    }
}