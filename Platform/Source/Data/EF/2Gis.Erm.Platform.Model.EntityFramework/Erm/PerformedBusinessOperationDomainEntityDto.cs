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
    public partial class PerformedBusinessOperationDomainEntityDto : IDomainEntityDto<DoubleGis.Erm.Platform.Model.Entities.Erm.PerformedBusinessOperation>
    {
    	[DataMember]
        public long Id { get; set; }
    	[DataMember]
        public int Operation { get; set; }
    	[DataMember]
        public int Descriptor { get; set; }
    	[DataMember]
        public string Context { get; set; }
    	[DataMember]
        public System.DateTime Date { get; set; }
    	[DataMember]
        public Nullable<long> Parent { get; set; }
    	[DataMember]
        public EntityReference UseCaseRef { get; set; }
    	[DataMember]
        public string OperationEntities { get; set; }
    }
}

// ReSharper enable RedundantUsingDirective
// ReSharper enable InconsistentNaming
// ReSharper enable PartialTypeWithSinglePart
// ReSharper enable RedundantNameQualifier
// ReSharper enable ConvertNullableToShortForm
