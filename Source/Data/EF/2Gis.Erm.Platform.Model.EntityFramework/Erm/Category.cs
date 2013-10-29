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
    public sealed partial class Category : 
        IEntity, 
        IEntityKey, 
        IAuditableEntity, 
        IDeletableEntity, 
        IDeactivatableEntity, 
        IReplicableEntity, 
        IStateTrackingEntity
    {
        public Category()
        {
            this.ChildCategories = new HashSet<Category>();
            this.CategoryFirmAddresses = new HashSet<CategoryFirmAddress>();
            this.CategoryOrganizationUnits = new HashSet<CategoryOrganizationUnit>();
            this.OrderPositionAdvertisements = new HashSet<OrderPositionAdvertisement>();
            this.ThemeCategories = new HashSet<ThemeCategory>();
        }
    
        public long Id { get; set; }
        public string Name { get; set; }
        public Nullable<long> ParentId { get; set; }
        public int Level { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public long CreatedBy { get; set; }
        public Nullable<long> ModifiedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public byte[] Timestamp { get; set; }
        public System.Guid ReplicationCode { get; set; }
        public string Comment { get; set; }
    
        public ICollection<Category> ChildCategories { get; set; }
        public Category ParentCategory { get; set; }
        public ICollection<CategoryFirmAddress> CategoryFirmAddresses { get; set; }
        public ICollection<CategoryOrganizationUnit> CategoryOrganizationUnits { get; set; }
        public ICollection<OrderPositionAdvertisement> OrderPositionAdvertisements { get; set; }
        public ICollection<ThemeCategory> ThemeCategories { get; set; }
    }
}

// ReSharper enable RedundantUsingDirective
// ReSharper enable InconsistentNaming
// ReSharper enable PartialTypeWithSinglePart
// ReSharper enable RedundantNameQualifier
// ReSharper enable ConvertNullableToShortForm


