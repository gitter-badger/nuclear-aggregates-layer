using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Activities
{
	public interface ITaskRepository : IAggregateRootRepository<Task>, IDeleteAggregateRepository<Task>
	{
		long Add(Task task);
		void UpdateContent(Task task);
		void UpdateRegardingObjects(Task task);
	}
}