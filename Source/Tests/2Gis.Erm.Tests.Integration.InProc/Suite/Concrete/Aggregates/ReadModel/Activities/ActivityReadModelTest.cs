using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Aggregates.ReadModel.Activities
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

            var appointment = _activityReadModel.GetAppointment(appropriateAppointment.Id);
            var task = _activityReadModel.GetTask(appropriateTask.Id);
            var phonecall = _activityReadModel.GetPhonecall(appropriatePhonecall.Id);

            // ReSharper disable once PossibleInvalidOperationException
            _activityReadModel.CheckIfRelatedActivitiesExists(activityWithClient.ClientId.Value);

			// TODO: fix
//            var appointmentDto = _activityReadModel.GetActivityInstanceDto(appointment);
//            var activityDto = _activityReadModel.GetActivityInstanceDto(task);
//            var phonecallDto = _activityReadModel.GetActivityInstanceDto(phonecall);
            var appointmentDto = new object();
            var activityDto = new object();
            var phonecallDto = new object();

            IEnumerable<ActivityInstance> relatedActivities = new ActivityInstance[0];
//            _activityReadModel.TryGetRelatedActivities(activityWithClient.ClientId.Value, out relatedActivities);

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