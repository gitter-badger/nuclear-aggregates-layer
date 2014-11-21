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
    public sealed partial class NotificationEmails : 
        IEntity, 
        IEntityKey, 
        IAuditableEntity, 
        IDeletableEntity, 
        IDeactivatableEntity, 
        IStateTrackingEntity
    {
        public NotificationEmails()
        {
            this.NotificationEmailsAttachments = new HashSet<NotificationEmailsAttachments>();
            this.NotificationEmailsCc = new HashSet<NotificationEmailsCc>();
            this.NotificationEmailsTo = new HashSet<NotificationEmailsTo>();
            this.NotificationProcessings = new HashSet<NotificationProcessings>();
        }
        public long Id { get; set; }
        public Nullable<long> SenderId { get; set; }
        public string Subject { get; set; }
        public string SubjectEncoding { get; set; }
        public string Body { get; set; }
        public string BodyEncoding { get; set; }
        public Nullable<System.DateTime> ExpirationTime { get; set; }
        public string Priority { get; set; }
        public Nullable<int> MaxAttemptsCount { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public long CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<long> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public byte[] Timestamp { get; set; }
        public bool IsBodyHtml { get; set; }
    
        public NotificationAddresses Sender { get; set; }
        public ICollection<NotificationEmailsAttachments> NotificationEmailsAttachments { get; set; }
        public ICollection<NotificationEmailsCc> NotificationEmailsCc { get; set; }
        public ICollection<NotificationEmailsTo> NotificationEmailsTo { get; set; }
        public ICollection<NotificationProcessings> NotificationProcessings { get; set; }
    
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
