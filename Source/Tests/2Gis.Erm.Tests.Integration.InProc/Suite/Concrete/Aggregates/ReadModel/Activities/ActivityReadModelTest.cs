using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.Platform.Aggregates.EAV;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Aggregates.ReadModel.Activities
{
    public class ActivityReadModelTest : IIntegrationTest
    {
        private readonly IActivityReadModel _activityReadModel;
		private readonly IAppropriateEntityProvider<DictionaryEntityInstance> _activityInstanceProvider;

        public ActivityReadModelTest(IActivityReadModel activityReadModel, IAppropriateEntityProvider<DictionaryEntityInstance> activityInstanceProvider)
        {
            _activityReadModel = activityReadModel;
            _activityInstanceProvider = activityInstanceProvider;
        }

        public ITestResult Execute()
        {
            var appropriateAppointment = _activityInstanceProvider.Get(DictionaryEntitySpecs.DictionaryEntity.Find.ByEntityName(EntityName.Appointment));
			var appropriateTask = _activityInstanceProvider.Get(DictionaryEntitySpecs.DictionaryEntity.Find.ByEntityName(EntityName.Task));
			var appropriatePhonecall = _activityInstanceProvider.Get(DictionaryEntitySpecs.DictionaryEntity.Find.ByEntityName(EntityName.Phonecall));
			var activityWithClient = _activityInstanceProvider.Get(ActivitySpecs.Activity.Find.OnlyActivities() &&
					new FindSpecification<DictionaryEntityInstance>(
						entity => entity.DictionaryEntityPropertyInstances.Any(
						property => property.PropertyId == ClientIdIdentity.Instance.Id && property.NumericValue.HasValue)));

            if (appropriateAppointment == null || appropriatePhonecall == null || appropriateTask == null || activityWithClient == null)
            {
                return OrdinaryTestResult.As.NotExecuted;
            }

            var appointment = _activityReadModel.GetAppointment(appropriateAppointment.Id);
            var task = _activityReadModel.GetTask(appropriateTask.Id);
            var phonecall = _activityReadModel.GetPhonecall(appropriatePhonecall.Id);

            // ReSharper disable once PossibleInvalidOperationException
			_activityReadModel.CheckIfRelatedActivitiesExists(
				(long)activityWithClient.DictionaryEntityPropertyInstances
				.First(property => property.PropertyId == ClientIdIdentity.Instance.Id).NumericValue.Value);

            return new object[]
                {
                    appointment,
                    task,
                    phonecall
                }.Any(x => x == null)
                       ? OrdinaryTestResult.As.Failed
                       : OrdinaryTestResult.As.Succeeded;
        }
    }
}