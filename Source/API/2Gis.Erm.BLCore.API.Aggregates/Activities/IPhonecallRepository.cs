using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Activities
{
	public interface IPhonecallRepository : IAggregateRootRepository<Phonecall>, IDeleteAggregateRepository<Phonecall>
	{
		long Add(Phonecall phonecall);
		void UpdateContent(Phonecall phonecall);
		void UpdateRegardingObjects(Phonecall phonecall);
	}
}