using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BL.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Enums;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Aggregates.ReadModel.Activity
{
    public class ActivityReadModelTest : IIntegrationTest 
    {
        private readonly IActivityReadModel _activityReadModel;
        private readonly IAppropriateEntityProvider<ActivityInstance> _activityInstanceProvider; 

        public ActivityReadModelTest(IActivityReadModel activityReadModel, IAppropriateEntityProvider<ActivityInstance> activityInstanceProvider)
        {
            _activityReadModel = activityReadModel;
            _activityInstanceProvider = activityInstanceProvider;
        }

        public ITestResult Execute()
        {
            var appropriateAppointment = _activityInstanceProvider.Get(new FindSpecification<ActivityInstance>(x => x.Type == (int)ActivityType.Appointment));
            var appropriateTask = _activityInstanceProvider.Get(new FindSpecification<ActivityInstance>(x => x.Type == (int)ActivityType.Task));
            var appropriatePhonecall = _activityInstanceProvider.Get(new FindSpecification<ActivityInstance>(x => x.Type == (int)ActivityType.Phonecall));
            var activityWithClient = _activityInstanceProvider.Get(new FindSpecification<ActivityInstance>(x => x.ClientId != null));


            if (appropriateAppointment == null || appropriatePhonecall == null || appropriateTask == null || activityWithClient == null)
            {
                return OrdinaryTestResult.As.NotExecuted;
            }

            var appointment = _activityReadModel.GetActivity<Appointment>(appropriateAppointment.Id);
            var activity = _activityReadModel.GetActivity<Task>(appropriateTask.Id);
            var phonecall = _activityReadModel.GetActivity<Phonecall>(appropriatePhonecall.Id);

            // ReSharper disable once PossibleInvalidOperationException
            _activityReadModel.CheckIfRelatedActivitiesExists(activityWithClient.ClientId.Value);

            var appointmentDto = _activityReadModel.GetActivityInstanceDto(appointment);
            var activityDto = _activityReadModel.GetActivityInstanceDto(activity);
            var phonecallDto = _activityReadModel.GetActivityInstanceDto(phonecall);

            IEnumerable<ActivityInstance> relatedActivities;
            _activityReadModel.TryGetRelatedActivities(activityWithClient.ClientId.Value, out relatedActivities);

            return new object[]
                {
                    appointmentDto,
                    activityDto,
                    phonecallDto,
                    relatedActivities
                }.Any(x => x == null)
                       ? OrdinaryTestResult.As.Failed
                       : OrdinaryTestResult.As.Succeeded;
        }
    }
}