//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
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
using DoubleGis.Erm.Model.Entities;
using DoubleGis.Erm.Model.Entities.Enums;
using DoubleGis.Erm.Model.Entities.Erm;
using DoubleGis.Erm.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Model.DTOs.DomainEntity
{
    [DataContract]
    public partial class PrivilegeDomainEntityDto : IDomainEntityDto<DoubleGis.Erm.Model.Entities.Security.Privilege>
    {
    	[DataMember]
        public long Id { get; set; }
    	[DataMember]
        public Nullable<int> EntityType { get; set; }
    	[DataMember]
        public int Operation { get; set; }
    }
}

// ReSharper enable RedundantUsingDirective
// ReSharper enable InconsistentNaming
// ReSharper enable PartialTypeWithSinglePart
// ReSharper enable RedundantNameQualifier
// ReSharper enable ConvertNullableToShortForm
