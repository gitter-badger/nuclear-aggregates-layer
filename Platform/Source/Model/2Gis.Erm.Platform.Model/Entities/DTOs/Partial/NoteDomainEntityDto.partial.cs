using System.Runtime.Serialization;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
// ReSharper restore CheckNamespace
{
    public partial class NoteDomainEntityDto
    {
        [DataMember]
        public EntityName ParentTypeName { get; set; }
        [DataMember]
        public long FileContentLength { get; set; }
        [DataMember]
        public string FileContentType { get; set; }
    }
}