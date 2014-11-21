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
    public partial class OrderReleaseTotalDomainEntityDto : IDomainEntityDto<DoubleGis.Erm.Platform.Model.Entities.Erm.OrderReleaseTotal>
    {
    	[DataMember]
        public long Id { get; set; }
    	[DataMember]
        public long OrderId { get; set; }
    	[DataMember]
        public int ReleaseNumber { get; set; }
    	[DataMember]
        public System.DateTime ReleaseBeginDate { get; set; }
    	[DataMember]
        public System.DateTime ReleaseEndDate { get; set; }
    	[DataMember]
        public decimal AmountToWithdraw { get; set; }
    	[DataMember]
        public EntityReference OwnerRef { get; set; }
    	[DataMember]
        public EntityReference CreatedByRef { get; set; }
    	[DataMember]
        public System.DateTime CreatedOn { get; set; }
    	[DataMember]
        public EntityReference ModifiedByRef { get; set; }
    	[DataMember]
        public Nullable<System.DateTime> ModifiedOn { get; set; }
    	[DataMember]
        public decimal Vat { get; set; }
    }
}

// ReSharper enable RedundantUsingDirective
// ReSharper enable InconsistentNaming
// ReSharper enable PartialTypeWithSinglePart
// ReSharper enable RedundantNameQualifier
// ReSharper enable ConvertNullableToShortForm
