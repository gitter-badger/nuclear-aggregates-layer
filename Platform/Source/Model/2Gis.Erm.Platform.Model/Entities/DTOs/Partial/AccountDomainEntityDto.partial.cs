using System;
using System.Runtime.Serialization;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
// ReSharper restore CheckNamespace
{
    public partial class AccountDomainEntityDto
    {
        [DataMember]
        public decimal AccountDetailBalance { get; set; }
        [DataMember]
        public EntityReference CurrencyRef { get; set; }
        [DataMember]
        public decimal LockDetailBalance { get; set; }
        [DataMember]
        public bool OwnerCanBeChanged { get; set; }
        [DataMember]
        public Uri BasicOperationsServiceUrl { get; set; }
    }
}