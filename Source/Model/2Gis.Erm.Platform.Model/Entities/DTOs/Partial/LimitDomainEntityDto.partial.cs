using System;
using System.Runtime.Serialization;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
// ReSharper restore CheckNamespace
{
    public partial class LimitDomainEntityDto
    {
        [DataMember]
        public EntityReference LegalPersonRef { get; set; }
        [DataMember]
        public EntityReference BranchOfficeRef { get; set; }
        [DataMember]
        public EntityReference InspectorRef { get; set; }
        [DataMember]
        public Uri BasicOperationsServiceUrl { get; set; }
        [DataMember]
        public string ErrorMessage { get; set; }
    }
}