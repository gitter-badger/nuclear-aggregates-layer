using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class NotificationAddresses :
        IEntity,
        IEntityKey,
        IAuditableEntity,
        IDeletableEntity,
        IDeactivatableEntity,
        IStateTrackingEntity
    {
        public NotificationAddresses()
        {
            NotificationEmails = new HashSet<NotificationEmails>();
            NotificationEmailsCcs = new HashSet<NotificationEmailsCc>();
            NotificationEmailsToes = new HashSet<NotificationEmailsTo>();
        }

        public long Id { get; set; }
        public string Address { get; set; }
        public string DisplayName { get; set; }
        public string DisplayNameEncoding { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public byte[] Timestamp { get; set; }

        public ICollection<NotificationEmails> NotificationEmails { get; set; }
        public ICollection<NotificationEmailsCc> NotificationEmailsCcs { get; set; }
        public ICollection<NotificationEmailsTo> NotificationEmailsToes { get; set; }

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
    }
}