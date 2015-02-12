using System;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class LegalPersonDeal :
        IEntity,
        IEntityKey,
        IAuditableEntity,
        IDeletableEntity
    {
        public long Id { get; set; }
        public long LegalPersonId { get; set; }
        public long DealId { get; set; }
        public bool IsMain { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public bool IsDeleted { get; set; } // comment {y.baranihin, 03.09.2014}: ј почему не использовать полное удаление?

        public Deal Deal { get; set; }
        public LegalPerson LegalPerson { get; set; }

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