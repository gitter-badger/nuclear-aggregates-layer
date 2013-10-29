using System;
using System.IO;

namespace DoubleGis.Erm.Platform.API.Core.Notifications
{
    public sealed class NotificationAttachment
    {
        private readonly Stream _stream;
        private readonly string _name;
        private readonly string _contentType;

        public NotificationAttachment(Stream stream, String name, String contentType)
        {
            _stream = stream;
            _name = name;
            _contentType = contentType;
        }

        public string ContentType
        {
            get { return _contentType; }
        }

        public string Name
        {
            get { return _name; }
        }

        public Stream Stream1
        {
            get { return _stream; }
        }
    }
}
