using System;
using System.Collections.Generic;

using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class File :
        IEntity,
        IEntityKey,
        IAuditableEntity,
        IStateTrackingEntity
    {
        public File()
        {
            AdvertisementElements = new HashSet<AdvertisementElement>();
            BargainFiles = new HashSet<BargainFile>();
            OrderFiles = new HashSet<OrderFile>();
            Notes = new HashSet<Note>();
            ReleaseInfos = new HashSet<ReleaseInfo>();
            LocalMessages = new HashSet<LocalMessage>();
            NotificationEmailsAttachments = new HashSet<NotificationEmailsAttachments>();
            PrintFormTemplates = new HashSet<PrintFormTemplate>();
            Operations = new HashSet<Operation>();
            Themes = new HashSet<Theme>();
            ThemeTemplates = new HashSet<ThemeTemplate>();
        }

        public long Id { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public long CreatedBy { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public byte[] Timestamp { get; set; }
        public long ContentLength { get; set; }
        public long? DgppId { get; set; }

        public ICollection<AdvertisementElement> AdvertisementElements { get; set; }
        public ICollection<BargainFile> BargainFiles { get; set; }
        public ICollection<OrderFile> OrderFiles { get; set; }
        public ICollection<Note> Notes { get; set; }
        public ICollection<ReleaseInfo> ReleaseInfos { get; set; }
        public ICollection<LocalMessage> LocalMessages { get; set; }
        public ICollection<NotificationEmailsAttachments> NotificationEmailsAttachments { get; set; }
        public ICollection<PrintFormTemplate> PrintFormTemplates { get; set; }
        public ICollection<Operation> Operations { get; set; }
        public ICollection<Theme> Themes { get; set; }
        public ICollection<ThemeTemplate> ThemeTemplates { get; set; }

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