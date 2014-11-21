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
    public partial class FirmDomainEntityDto : IDomainEntityDto<DoubleGis.Erm.Platform.Model.Entities.Erm.Firm>
    {
    	[DataMember]
        public long Id { get; set; }
    	[DataMember]
        public System.Guid ReplicationCode { get; set; }
    	[DataMember]
        public string Name { get; set; }
    	[DataMember]
        public int PromisingScore { get; set; }
    	[DataMember]
        public UsingOtherMediaOption UsingOtherMedia { get; set; }
    	[DataMember]
        public ProductType ProductType { get; set; }
    	[DataMember]
        public MarketType MarketType { get; set; }
    	[DataMember]
        public EntityReference OrganizationUnitRef { get; set; }
    	[DataMember]
        public EntityReference TerritoryRef { get; set; }
    	[DataMember]
        public EntityReference ClientRef { get; set; }
    	[DataMember]
        public bool ClosedForAscertainment { get; set; }
    	[DataMember]
        public Nullable<System.DateTime> LastQualifyTime { get; set; }
    	[DataMember]
        public Nullable<System.DateTime> LastDisqualifyTime { get; set; }
    	[DataMember]
        public string Information { get; set; }
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
    	[DataMember]
        public BudgetType BudgetType { get; set; }
    	[DataMember]
        public Geolocation Geolocation { get; set; }
    	[DataMember]
        public InCityBranchesAmount InCityBranchesAmount { get; set; }
    	[DataMember]
        public OutCityBranchesAmount OutCityBranchesAmount { get; set; }
    	[DataMember]
        public StaffAmount StaffAmount { get; set; }
    }
}

// ReSharper enable RedundantUsingDirective
// ReSharper enable InconsistentNaming
// ReSharper enable PartialTypeWithSinglePart
// ReSharper enable RedundantNameQualifier
// ReSharper enable ConvertNullableToShortForm
