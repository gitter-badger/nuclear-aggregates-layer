using System;

using Nest;

namespace DoubleGis.Erm.Elastic.Nest.Qds.Indexing
{
    public interface IDocumentWrapper
    {
        Func<BulkDescriptor, BulkDescriptor> IndexFunc { get; }
    }

    public interface IDocumentWrapper<out TDocument> : IDocumentWrapper
    {
        string Id { get; }
        TDocument Document { get; }
    }

    public sealed class DocumentWrapper<TDocument> : IDocumentWrapper<TDocument>
        where TDocument : class
    {
        public string Id { get; set; }
        public TDocument Document { get; set; }
        private string _versionType;

        private string _version;
        public string Version
        {
            get { return _version; }
            set
            {
                _version = value;
                _versionType = "internal";
            }
        }

        public byte[] TimeStamp
        {
            set
            {
                _version = ConvertTimestampToVersion(value);
                _versionType = "external_gte";
            }
        }

        private static string ConvertTimestampToVersion(byte[] bytes)
        {
            if (bytes.Length != 8)
            {
                throw new ArgumentException("bytes");
            }

            ulong result = 0;
            result |= bytes[7];
            result |= (ulong)bytes[6] << 8;
            result |= (ulong)bytes[5] << 16;
            result |= (ulong)bytes[4] << 24;
            result |= (ulong)bytes[3] << 32;
            result |= (ulong)bytes[2] << 40;
            result |= (ulong)bytes[1] << 48;
            result |= (ulong)bytes[0] << 56;

            var version = result + "0000";
            return version;
        }

        public Func<BulkDescriptor, BulkDescriptor> IndexFunc
        {
            get
            {
                if (Version == null)
                {
                    return bulkDescriptor => bulkDescriptor
                        .Update<TDocument, TDocument>(bulkUpdateDescriptor => bulkUpdateDescriptor
                            .Id(Id)
                            .Document(Document));
                }

                return bulkDescriptor => bulkDescriptor
                    .Index<TDocument>(bulkIndexDescriptor => bulkIndexDescriptor
                        .Id(Id)
                        .Object(Document)
                        .Version(Version)
                        .VersionType(_versionType));
            }
        }
    }
}