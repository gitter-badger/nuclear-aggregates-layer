using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.Activity
{
	public abstract class EntityReference<TEntity>
		where TEntity : IEntity
	{
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
			return Equals((EntityReference<TEntity>)obj);
		}

		protected bool Equals(EntityReference<TEntity> other)
		{
			return SourceEntityId == other.SourceEntityId 
				&& TargetEntityName == other.TargetEntityName 
				&& TargetEntityId == other.TargetEntityId;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = SourceEntityId.GetHashCode();
				hashCode = (hashCode * 397) ^ (int)TargetEntityName;
				hashCode = (hashCode * 397) ^ TargetEntityId.GetHashCode();
				return hashCode;
			}
		}
	}

	public sealed class RegardingObject<TEntity> : EntityReference<TEntity>, IEntity 
		where TEntity : class, IEntity
	{
	}
}