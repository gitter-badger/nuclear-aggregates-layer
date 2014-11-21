using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.ServiceModel.Channels;
using System.Xml;

namespace DoubleGis.Erm.Platform.WCF.Infrastructure.ServiceModel.EndpointBehaviors.Jsonp
{
    public class JsonpEncoderFactory : MessageEncoderFactory
    {
        private readonly JsonpEncoder _encoder;

        public JsonpEncoderFactory()
        {
            _encoder = new JsonpEncoder();
        }

        public override MessageEncoder Encoder
        {
            get
            {
                return _encoder;
            }
        }

        public override MessageVersion MessageVersion
        {
            get
            {
                return _encoder.MessageVersion;
            }
        }

        private class JsonpEncoder : MessageEncoder
        {
            private readonly MessageEncoder _encoder;

            public JsonpEncoder()
            {
                var element = new WebMessageEncodingBindingElement();
                _encoder = element.CreateMessageEncoderFactory().Encoder;
            }

            public override string ContentType
            {
                get
                {
                    return _encoder.ContentType;
                }
            }

            public override string MediaType
            {
                get
                {
                    return _encoder.MediaType;
                }
            }

            public override MessageVersion MessageVersion
            {
                get
                {
                    return _encoder.MessageVersion;
                }
            }

            public override Message ReadMessage(ArraySegment<byte> buffer, BufferManager bufferManager, string contentType)
            {
                return _encoder.ReadMessage(buffer, bufferManager, contentType);
            }

            public override Message ReadMessage(Stream stream, int maxSizeOfHeaders, string contentType)
            {

                return _encoder.ReadMessage(stream, maxSizeOfHeaders, contentType);
            }

            public override ArraySegment<byte> WriteMessage(Message message, int maxMessageSize, BufferManager bufferManager, int messageOffset)
            {
                var stream = new MemoryStream();
                var streamWriter = new StreamWriter(stream);

                string methodName = null;
                if (message.Properties.ContainsKey(JsonpMessageProperty.Name))
                {
                    methodName = ((JsonpMessageProperty)message.Properties[JsonpMessageProperty.Name]).MethodName;
                }

                if (methodName != null)
                {
                    streamWriter.Write(methodName + "(");
                    streamWriter.Flush();
                }

                XmlWriter jsonWriter = JsonReaderWriterFactory.CreateJsonWriter(stream);
                message.WriteMessage(jsonWriter);
                jsonWriter.Flush();

                if (methodName != null)
                {
                    streamWriter.Write(");");
                    streamWriter.Flush();
                }

                var messageBytes = stream.GetBuffer();
                var messageLength = (int)stream.Position;
                var totalLength = messageLength + messageOffset;
                var totalBytes = bufferManager.TakeBuffer(totalLength);
                Array.Copy(messageBytes, 0, totalBytes, messageOffset, messageLength);

                var byteArray = new ArraySegment<byte>(totalBytes, messageOffset, messageLength);
                jsonWriter.Close();
                return byteArray;
            }

            public override void WriteMessage(Message message, Stream stream)
            {
                string methodName = null;
                if (message.Properties.ContainsKey(JsonpMessageProperty.Name))
                {
                    methodName = ((JsonpMessageProperty)message.Properties[JsonpMessageProperty.Name]).MethodName;
                }

                if (methodName == null)
                {
                    _encoder.WriteMessage(message, stream);
                    return;
                }

                WriteToStream(stream, methodName + "(");
                _encoder.WriteMessage(message, stream);
                WriteToStream(stream, ");");
            }

            public override bool IsContentTypeSupported(string contentType)
            {
                return _encoder.IsContentTypeSupported(contentType);
            }

            private static void WriteToStream(Stream stream, string content)
            {
                using (var sw = new StreamWriter(stream))
                {
                    sw.Write(content);
                }
            }
        }
    }
}