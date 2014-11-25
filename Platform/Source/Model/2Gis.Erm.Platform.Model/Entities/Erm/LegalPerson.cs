using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces.Integration;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class LegalPerson : IEntity,
        IEntityKey, 
        ICuratedEntity, 
        IAuditableEntity, 
        IDeletableEntity, 
        IDeactivatableEntity, 
        IReplicableEntity, 
                                      IStateTrackingEntity,
                                      IPartable
    {
        private long _ownerCode;
        private long? _oldOwnerCode;
    
        public LegalPerson()
        {
            Accounts = new HashSet<Account>();
            Bargains = new HashSet<Bargain>();
            LegalPersonProfiles = new HashSet<LegalPersonProfile>();
            Orders = new HashSet<Order>();
            OrderProcessingRequests = new HashSet<OrderProcessingRequest>();
            LegalPersonDeals = new HashSet<LegalPersonDeal>();
        }

        public long Id { get; set; }
        public Guid ReplicationCode { get; set; }
        public long? DgppId { get; set; }
        public bool IsInSyncWith1C { get; set; }
        public long? ClientId { get; set; }
        public string LegalName { get; set; }
        public string ShortName { get; set; }
        // TODO {a.tukaev, 10.11.2014}: опять же стоит избавиться от суффикса Enum
        public LegalPersonType LegalPersonTypeEnum { get; set; }
        public string LegalAddress { get; set; }
        public string Inn { get; set; }
        public string Kpp { get; set; }
        public string PassportSeries { get; set; }
        public string PassportNumber { get; set; }
        public string PassportIssuedBy { get; set; }
        public string RegistrationAddress { get; set; }
        public string Comment { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }

        public long OwnerCode 
    	{
            get { return _ownerCode; }
    				 
    		set
    		{
    			_oldOwnerCode = _ownerCode;
    			_ownerCode = value;
    		} 
        }

        long? ICuratedEntity.OldOwnerCode
        {
            get { return _oldOwnerCode; }
    	}

        public long CreatedBy { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public byte[] Timestamp { get; set; }
        public string VAT { get; set; }
        public string CardNumber { get; set; }
        public string Ic { get; set; }
    
        public ICollection<Account> Accounts { get; set; }
        public ICollection<Bargain> Bargains { get; set; }
        public Client Client { get; set; }
        public ICollection<LegalPersonProfile> LegalPersonProfiles { get; set; }
        public ICollection<Order> Orders { get; set; }
        public ICollection<OrderProcessingRequest> OrderProcessingRequests { get; set; }
        public ICollection<LegalPersonDeal> LegalPersonDeals { get; set; }
    
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
    
        public override int GetHashCode()
    	{
    		return Id.GetHashCode();
    	}

        #region Parts

        private IEnumerable<IEntityPart> _parts;

        public IEnumerable<IEntityPart> Parts
        {
            get { return _parts ?? (_parts = Enumerable.Empty<IEntityPart>()); }

            set
            {
                if (value == null)
                {
                    throw new InvalidOperationException("Parts cannot be null");
                }

                _parts = value;
            }
        }

        #endregion
    }
}
