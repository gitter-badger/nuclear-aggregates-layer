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
    public partial class PositionDomainEntityDto : IDomainEntityDto<DoubleGis.Erm.Platform.Model.Entities.Erm.Position>
    {
    	[DataMember]
        public long Id { get; set; }
    	[DataMember]
        public System.Guid ReplicationCode { get; set; }
    	[DataMember]
        public string Name { get; set; }
    	[DataMember]
        public bool IsComposite { get; set; }
    	[DataMember]
        public PositionCalculationMethod CalculationMethodEnum { get; set; }
    	[DataMember]
        public PositionBindingObjectType BindingObjectTypeEnum { get; set; }
    	[DataMember]
        public PositionAccountingMethod AccountingMethodEnum { get; set; }
    	[DataMember]
        public EntityReference PlatformRef { get; set; }
    	[DataMember]
        public EntityReference CategoryRef { get; set; }
    	[DataMember]
        public EntityReference AdvertisementTemplateRef { get; set; }
    	[DataMember]
        public bool IsDeleted { get; set; }
    	[DataMember]
        public bool IsActive { get; set; }
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
        public Nullable<long> DgppId { get; set; }
    	[DataMember]
        public int ExportCode { get; set; }
    	[DataMember]
        public bool IsControlledByAmount { get; set; }
    	[DataMember]
        public bool RestrictChildPositionPlatforms { get; set; }
    }
}

// ReSharper enable RedundantUsingDirective
// ReSharper enable InconsistentNaming
// ReSharper enable PartialTypeWithSinglePart
// ReSharper enable RedundantNameQualifier
// ReSharper enable ConvertNullableToShortForm
