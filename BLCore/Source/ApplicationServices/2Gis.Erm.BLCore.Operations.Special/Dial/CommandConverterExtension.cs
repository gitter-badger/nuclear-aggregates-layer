using System;
using System.Globalization;
using System.Text;

namespace DoubleGis.Erm.BLCore.Operations.Special.Dial
{
    internal static class CommandConverterExtension
    {        
        public static string MakeXmlCommand(this string phone, string line, PhoneMode mode)
        {
            var xmlCommand = string.Format(
                CultureInfo.InvariantCulture,
                @"<Message ServerType=""{0}"" Command =""{1}"" Line=""{2}"" Address=""{3}""/>",
                (int)mode,
                (int)CommandType.Dial,
                line,
                phone);
            return xmlCommand;
        }

        public static byte[] MakeBytesFromCommand(this string command)
        {
            const int SizePrefixLength = 2;

            var commandBytes = Encoding.Unicode.GetBytes(command);

            var commandSize = (short)(commandBytes.Length + SizePrefixLength);

            var bytes = new byte[commandSize];
            var sizeArray = BitConverter.GetBytes((ushort)commandSize);

            sizeArray.CopyTo(bytes, 0);
            commandBytes.CopyTo(bytes, SizePrefixLength);

            return bytes;
        }
    }
}
