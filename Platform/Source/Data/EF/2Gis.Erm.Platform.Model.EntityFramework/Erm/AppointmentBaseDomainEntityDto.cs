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
    public partial class AppointmentBaseDomainEntityDto : IDomainEntityDto<DoubleGis.Erm.Platform.Model.Entities.Erm.AppointmentBase>
    {
    	[DataMember]
        public long Id { get; set; }
    	[DataMember]
        public System.Guid ReplicationCode { get; set; }
    	[DataMember]
        public EntityReference CreatedByRef { get; set; }
    	[DataMember]
        public System.DateTime CreatedOn { get; set; }
    	[DataMember]
        public EntityReference ModifiedByRef { get; set; }
    	[DataMember]
        public Nullable<System.DateTime> ModifiedOn { get; set; }
    	[DataMember]
        public bool IsActive { get; set; }
    	[DataMember]
        public bool IsDeleted { get; set; }
    	[DataMember]
        public byte[] Timestamp { get; set; }
    	[DataMember]
        public EntityReference OwnerRef { get; set; }
    	[DataMember]
        public string Subject { get; set; }
    	[DataMember]
        public string Description { get; set; }
    	[DataMember]
        public System.DateTime ScheduledStart { get; set; }
    	[DataMember]
        public System.DateTime ScheduledEnd { get; set; }
    	[DataMember]
        public Nullable<System.DateTime> ActualEnd { get; set; }
    	[DataMember]
        public int Priority { get; set; }
    	[DataMember]
        public int Status { get; set; }
    	[DataMember]
        public bool IsAllDayEvent { get; set; }
    	[DataMember]
        public string Location { get; set; }
    	[DataMember]
        public int Purpose { get; set; }
    }
}

// ReSharper enable RedundantUsingDirective
// ReSharper enable InconsistentNaming
// ReSharper enable PartialTypeWithSinglePart
// ReSharper enable RedundantNameQualifier
// ReSharper enable ConvertNullableToShortForm
