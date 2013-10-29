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
using System.Collections.Generic;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces.Integration;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed partial class FirmAddress : 
        IEntity, 
        IEntityKey, 
        IAuditableEntity, 
        IDeletableEntity, 
        IDeactivatableEntity, 
        IReplicableEntity, 
        IStateTrackingEntity
    {
        public FirmAddress()
        {
            this.CategoryFirmAddresses = new HashSet<CategoryFirmAddress>();
            this.FirmContacts = new HashSet<FirmContact>();
            this.OrderPositionAdvertisements = new HashSet<OrderPositionAdvertisement>();
            this.FirmAddressServices = new HashSet<FirmAddressService>();
        }
    
        public long Id { get; set; }
        public long FirmId { get; set; }
        public string Address { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public long CreatedBy { get; set; }
        public Nullable<long> ModifiedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public byte[] Timestamp { get; set; }
        public System.Guid ReplicationCode { get; set; }
        public bool ClosedForAscertainment { get; set; }
        public int SortingPosition { get; set; }
        public string PaymentMethods { get; set; }
        public string WorkingTime { get; set; }
        public Nullable<long> BuildingCode { get; set; }
        public bool IsLocatedOnTheMap { get; set; }
        public Nullable<long> AddressCode { get; set; }
        public string ReferencePoint { get; set; }
    
        public ICollection<CategoryFirmAddress> CategoryFirmAddresses { get; set; }
        public ICollection<FirmContact> FirmContacts { get; set; }
        public ICollection<OrderPositionAdvertisement> OrderPositionAdvertisements { get; set; }
        public Firm Firm { get; set; }
        public Building Building { get; set; }
        public ICollection<FirmAddressService> FirmAddressServices { get; set; }
    }
}

// ReSharper enable RedundantUsingDirective
// ReSharper enable InconsistentNaming
// ReSharper enable PartialTypeWithSinglePart
// ReSharper enable RedundantNameQualifier
// ReSharper enable ConvertNullableToShortForm


