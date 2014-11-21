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
    public sealed partial class Territory : 
        IEntity, 
        IEntityKey, 
        IAuditableEntity, 
        IDeactivatableEntity, 
        IReplicableEntity, 
        IStateTrackingEntity
    {
        public Territory()
        {
            this.IsActive = true;
            this.UserTerritoriesOrganizationUnits = new HashSet<UserTerritoriesOrganizationUnits>();
            this.Clients = new HashSet<Client>();
            this.Firms = new HashSet<Firm>();
            this.FirmAddresses = new HashSet<FirmAddress>();
            this.Buildings = new HashSet<Building>();
        }
        public long Id { get; set; }
        public string Name { get; set; }
        public long OrganizationUnitId { get; set; }
        public System.Guid ReplicationCode { get; set; }
        public bool IsActive { get; set; }
        public long CreatedBy { get; set; }
        public Nullable<long> ModifiedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public byte[] Timestamp { get; set; }
    
        public OrganizationUnit OrganizationUnit { get; set; }
        public ICollection<UserTerritoriesOrganizationUnits> UserTerritoriesOrganizationUnits { get; set; }
        public ICollection<Client> Clients { get; set; }
        public ICollection<Firm> Firms { get; set; }
        public ICollection<FirmAddress> FirmAddresses { get; set; }
        public ICollection<Building> Buildings { get; set; }
    
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
