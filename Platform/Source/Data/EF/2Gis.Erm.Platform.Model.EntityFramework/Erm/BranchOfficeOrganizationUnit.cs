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
    public sealed partial class BranchOfficeOrganizationUnit : 
        IEntity, 
        IEntityKey, 
        ICuratedEntity, 
        IAuditableEntity, 
        IDeletableEntity, 
        IDeactivatableEntity, 
        IReplicableEntity, 
        IStateTrackingEntity
    {
        private long _ownerCode;
        private long? _oldOwnerCode;
    
        public BranchOfficeOrganizationUnit()
        {
            this.Accounts = new HashSet<Account>();
            this.Bargains = new HashSet<Bargain>();
            this.Orders = new HashSet<Order>();
            this.PrintFormTemplates = new HashSet<PrintFormTemplate>();
        }
        public long Id { get; set; }
        public System.Guid ReplicationCode { get; set; }
        public string SyncCode1C { get; set; }
        public string RegistrationCertificate { get; set; }
        public long BranchOfficeId { get; set; }
        public long OrganizationUnitId { get; set; }
        public string ShortLegalName { get; set; }
        public string PositionInNominative { get; set; }
        public string PositionInGenitive { get; set; }
        public string ChiefNameInNominative { get; set; }
        public string ChiefNameInGenitive { get; set; }
        public string Registered { get; set; }
        public string OperatesOnTheBasisInGenitive { get; set; }
        public string Kpp { get; set; }
        public string PaymentEssentialElements { get; set; }
        public string PhoneNumber { get; set; }
        public string ActualAddress { get; set; }
        public string PostalAddress { get; set; }
        public bool IsPrimary { get; set; }
        public bool IsPrimaryForRegionalSales { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public long OwnerCode 
    	{
    		get
    		{
    			return _ownerCode;
    		}
    				 
    		set
    		{
    			_oldOwnerCode = _ownerCode;
    			_ownerCode = value;
    		} 
    	}
    	long? ICuratedEntity.OldOwnerCode { get { return _oldOwnerCode; } }
        public long CreatedBy { get; set; }
        public Nullable<long> ModifiedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public byte[] Timestamp { get; set; }
        public string Email { get; set; }
    
        public ICollection<Account> Accounts { get; set; }
        public ICollection<Bargain> Bargains { get; set; }
        public BranchOffice BranchOffice { get; set; }
        public OrganizationUnit OrganizationUnit { get; set; }
        public ICollection<Order> Orders { get; set; }
        public ICollection<PrintFormTemplate> PrintFormTemplates { get; set; }
    
    	public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
    
            if (GetType() != obj.GetType())
            {
                return false;
            }
    
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
    
    		var entityKey = obj as IEntityKey;
    		if (entityKey != null)
    		{
    			return Id == entityKey.Id;
    		}
    		
    		return false;
        }
    
    	override public int GetHashCode()
    	{
    		return Id.GetHashCode();
    	}
    }
}
