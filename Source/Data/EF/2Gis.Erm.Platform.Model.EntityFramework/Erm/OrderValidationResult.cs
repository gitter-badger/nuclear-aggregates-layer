//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces.Integration;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed partial class OrderValidationResult : 
        IEntity
    {
        public long OrderId { get; set; }
        public int ValidatorId { get; set; }
        public byte[] ValidVersion { get; set; }
        public System.Guid OperationId { get; set; }
    
    
    	override public int GetHashCode()
    	{
    		return base.GetHashCode();
    	}
    }
}
