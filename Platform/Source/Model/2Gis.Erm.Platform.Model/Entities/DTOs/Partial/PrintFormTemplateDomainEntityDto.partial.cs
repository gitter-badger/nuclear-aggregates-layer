using System.Runtime.Serialization;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
// ReSharper restore CheckNamespace
{
    public partial class PrintFormTemplateDomainEntityDto
    {
        [DataMember]
        public string FileContentType { get; set; }
        [DataMember]
        public long FileContentLength { get; set; }
    }
}