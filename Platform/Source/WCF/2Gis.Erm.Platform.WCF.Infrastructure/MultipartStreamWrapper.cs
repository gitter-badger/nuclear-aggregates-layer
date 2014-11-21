using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace DoubleGis.Erm.Platform.WCF.Infrastructure
{
    // Original source at http://multipartparser.codeplex.com/
    public class MultipartStreamWrapper
    {
        public MultipartStreamWrapper(Stream stream)
        {
            Parse(stream, Encoding.UTF8);
        }

        public MultipartStreamWrapper(Stream stream, Encoding encoding)
        {
            Parse(stream, encoding);
        }

        public bool IsParsedSuccessfully
        {
            get;
            private set;
        }

        public string ContentType
        {
            get;
            private set;
        }

        public string Filename
        {
            get;
            private set;
        }

        public byte[] BinaryContents
        {
            get;
            private set;
        }

        private static int IndexOf(byte[] searchWithin, byte[] serachFor, int startIndex)
        {
            var index = 0;
            var startPosition = Array.IndexOf(searchWithin, serachFor[0], startIndex);

            if (startPosition != -1)
            {
                while ((startPosition + index) < searchWithin.Length)
                {
                    if (searchWithin[startPosition + index] == serachFor[index])
                    {
                        index++;
                        if (index == serachFor.Length)
                        {
                            return startPosition;
                        }
                    }
                    else
                    {
                        startPosition = Array.IndexOf(searchWithin, serachFor[0], startPosition + index);
                        if (startPosition == -1)
                        {
                            return -1;
                        }

                        index = 0;
                    }
                }
            }

            return -1;
        }

        private void Parse(Stream stream, Encoding encoding)
        {
            IsParsedSuccessfully = false;

            var memory = new MemoryStream();
            stream.CopyTo(memory);
            var data = memory.ToArray();

            var content = encoding.GetString(data);

            var delimiterEndIndex = content.IndexOf(Environment.NewLine, StringComparison.OrdinalIgnoreCase);

            if (delimiterEndIndex <= -1)
            {
                return;
            }

            var delimiter = content.Substring(0, content.IndexOf(Environment.NewLine, StringComparison.OrdinalIgnoreCase));

            var contentTypeRegex = new Regex(string.Format(@"(?<=Content\-Type:)(.*?)(?={0}{0})", Environment.NewLine));
            var filenameRegex = new Regex(@"(?<=filename\=\"")(.*?)(?=\"")");

            var contentTypeMatch = contentTypeRegex.Match(content);
            var filenameMatch = filenameRegex.Match(content);

            if (!contentTypeMatch.Success || !filenameMatch.Success)
            {
                return;
            }

            ContentType = contentTypeMatch.Value.Trim();
            Filename = filenameMatch.Value.Trim();

            var startIndex = contentTypeMatch.Index + contentTypeMatch.Length + string.Format("{0}{0}", Environment.NewLine).Length;

            var delimiterBytes = encoding.GetBytes(Environment.NewLine + delimiter);
            var endIndex = IndexOf(data, delimiterBytes, startIndex);

            var contentLength = endIndex - startIndex;

            var fileData = new byte[contentLength];

            Buffer.BlockCopy(data, startIndex, fileData, 0, contentLength);

            BinaryContents = fileData;
            IsParsedSuccessfully = true;
        }
    }
}