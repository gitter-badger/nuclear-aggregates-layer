//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//-----------------------------------------------------------------------------

// ReSharper disable RedundantUsingDirective
// ReSharper disable InconsistentNaming
// ReSharper disable PartialTypeWithSinglePart
// ReSharper disable RedundantNameQualifier
// ReSharper disable ConvertNullableToShortForm

using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Model.Entities.Enums;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public partial class ContactDomainEntityDto : IDomainEntityDto<DoubleGis.Erm.Platform.Model.Entities.Erm.Contact>
    {
    	[DataMember]
        public long Id { get; set; }
    	[DataMember]
        public Nullable<long> DgppId { get; set; }
    	[DataMember]
        public System.Guid ReplicationCode { get; set; }
    	[DataMember]
        public string FullName { get; set; }
    	[DataMember]
        public string FirstName { get; set; }
    	[DataMember]
        public string MiddleName { get; set; }
    	[DataMember]
        public string LastName { get; set; }
    	[DataMember]
        public string Salutation { get; set; }
    	[DataMember]
        public Gender GenderCode { get; set; }
    	[DataMember]
        public FamilyStatus FamilyStatusCode { get; set; }
    	[DataMember]
        public string MainPhoneNumber { get; set; }
    	[DataMember]
        public string AdditionalPhoneNumber { get; set; }
    	[DataMember]
        public string MobilePhoneNumber { get; set; }
    	[DataMember]
        public string HomePhoneNumber { get; set; }
    	[DataMember]
        public string Fax { get; set; }
    	[DataMember]
        public string WorkEmail { get; set; }
    	[DataMember]
        public string HomeEmail { get; set; }
    	[DataMember]
        public string Website { get; set; }
    	[DataMember]
        public string ImIdentifier { get; set; }
    	[DataMember]
        public EntityReference ClientRef { get; set; }
    	[DataMember]
        public string JobTitle { get; set; }
    	[DataMember]
        public AccountRole AccountRole { get; set; }
    	[DataMember]
        public string Department { get; set; }
    	[DataMember]
        public bool IsFired { get; set; }
    	[DataMember]
        public Nullable<System.DateTime> BirthDate { get; set; }
    	[DataMember]
        public string WorkAddress { get; set; }
    	[DataMember]
        public string HomeAddress { get; set; }
    	[DataMember]
        public string Comment { get; set; }
    	[DataMember]
        public bool IsDeleted { get; set; }
    	[DataMember]
        public bool IsActive { get; set; }
    	[DataMember]
        public EntityReference OwnerRef { get; set; }
    	[DataMember]
        public EntityReference CreatedByRef { get; set; }
    	[DataMember]
        public EntityReference ModifiedByRef { get; set; }
    	[DataMember]
        public System.DateTime CreatedOn { get; set; }
    	[DataMember]
        public Nullable<System.DateTime> ModifiedOn { get; set; }
    	[DataMember]
        public byte[] Timestamp { get; set; }
    }
}

// ReSharper enable RedundantUsingDirective
// ReSharper enable InconsistentNaming
// ReSharper enable PartialTypeWithSinglePart
// ReSharper enable RedundantNameQualifier
// ReSharper enable ConvertNullableToShortForm
