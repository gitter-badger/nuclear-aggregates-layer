using System;
using System.Globalization;
using System.Text;

namespace DoubleGis.Erm.BLCore.Operations.Special.Dial.Telephony
{
    public enum PhoneMode
    {
        None = 0,
        Tapi = 2,
        Ami = 3,
        IntegratedVoip = 4
    }

    internal enum CommandType
    {
        None = 0,
        Dial = 1,
        Drop = 2
    }

    public static class CommandConverterExtension
    {        
        public static string MakeXmlCommand(this string phone, string line)
        {
            var xmlCommand = string.Format(
                CultureInfo.InvariantCulture,
                @"<Message ServerType=""{0}"" Command =""{1}"" Line=""{2}"" Address=""{3}""/>",
                (int)PhoneMode.Tapi,
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
