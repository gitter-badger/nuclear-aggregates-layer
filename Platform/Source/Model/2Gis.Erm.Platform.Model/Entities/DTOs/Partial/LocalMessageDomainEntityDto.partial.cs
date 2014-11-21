using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Enums;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
// ReSharper restore CheckNamespace
{
    public partial class LocalMessageDomainEntityDto
    {
        [DataMember]
        public IntegrationTypeImport IntegrationTypeImport { get; set; }
        [DataMember]
        public IntegrationTypeExport IntegrationTypeExport { get; set; }
        [DataMember]
        public IntegrationSystem SenderSystem { get; set; }
        [DataMember]
        public IntegrationSystem ReceiverSystem { get; set; }
    }
}