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

namespace DoubleGis.Erm.Platform.Model.Entities.Security
{
    public sealed partial class User : 
        IEntity, 
        IEntityKey, 
        IAuditableEntity, 
        IDeletableEntity, 
        IDeactivatableEntity, 
        IStateTrackingEntity
    {
        public User()
        {
            this.UserOrganizationUnits = new HashSet<UserOrganizationUnit>();
            this.UserRoles = new HashSet<UserRole>();
            this.Children = new HashSet<User>();
            this.UserTerritories = new HashSet<UserTerritory>();
            this.UserEntities = new HashSet<UserEntity>();
            this.UserProfiles = new HashSet<UserProfile>();
        }
    
        public long Id { get; set; }
        public string Account { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public long DepartmentId { get; set; }
        public Nullable<long> ParentId { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public long CreatedBy { get; set; }
        public Nullable<long> ModifiedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public byte[] Timestamp { get; set; }
        public string DisplayName { get; set; }
        public bool IsServiceUser { get; set; }
    
        public Department Department { get; set; }
        public ICollection<UserOrganizationUnit> UserOrganizationUnits { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }
        public ICollection<User> Children { get; set; }
        public User Parent { get; set; }
        public ICollection<UserTerritory> UserTerritories { get; set; }
        public ICollection<UserEntity> UserEntities { get; set; }
        public ICollection<UserProfile> UserProfiles { get; set; }
    }
}

// ReSharper enable RedundantUsingDirective
// ReSharper enable InconsistentNaming
// ReSharper enable PartialTypeWithSinglePart
// ReSharper enable RedundantNameQualifier
// ReSharper enable ConvertNullableToShortForm


