using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.Activity
{
	public class EntityToEntityReference : IEntity
	{
		public ReferenceType ReferenceType { get; set; }
		public EntityName SourceEntityName { get; set; }
		public long SourceEntityId { get; set; }
		public EntityName TargetEntityName { get; set; }
		public long TargetEntityId { get; set; }

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}
			if (ReferenceEquals(this, obj))
			{
				return true;
			}
			if (obj.GetType() != GetType())
			{
				return false;
			}
			return Equals((EntityToEntityReference)obj);
		}

		protected bool Equals(EntityToEntityReference other)
		{
			return ReferenceType == other.ReferenceType 
				&& SourceEntityName == other.SourceEntityName 
				&& SourceEntityId == other.SourceEntityId 
				&& TargetEntityName == other.TargetEntityName 
				&& TargetEntityId == other.TargetEntityId;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (int) ReferenceType;
				hashCode = (hashCode * 397) ^ (int)SourceEntityName;
				hashCode = (hashCode * 397) ^ SourceEntityId.GetHashCode();
				hashCode = (hashCode * 397) ^ (int)TargetEntityName;
				hashCode = (hashCode * 397) ^ TargetEntityId.GetHashCode();
				return hashCode;
			}
		}
	}

	public enum ReferenceType
	{
		Unspecified = 0,
		RegardingObject = 1,
		Organizer = 2,
		RequiredAttendees = 3,
		OptionalAttendees = 4,
		From = 5,
		To = 6
	}
}