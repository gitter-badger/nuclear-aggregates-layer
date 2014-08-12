using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Aggregates.ReadModel.Activities
{
    public class ActivityReadModelTest : IIntegrationTest
    {
	    private readonly IFinder _finder;
	    private readonly IActivityReadModel _activityReadModel;

        public ActivityReadModelTest(IFinder finder, IActivityReadModel activityReadModel)
        {
	        _finder = finder;
	        _activityReadModel = activityReadModel;
        }

        public ITestResult Execute()
        {
			var appropriateAppointment = _finder.Find<AppointmentBase>(x => true).FirstOrDefault();
			var appropriatePhonecall = _finder.Find<PhonecallBase>(x => true).FirstOrDefault();
			var appropriateTask = _finder.Find<TaskBase>(x => true).FirstOrDefault();
			var activityWithClient = _finder.Find<AppointmentBase>(x => x.AppointmentReferences.Any(
				r => r.Reference == (int)ReferenceType.RegardingObject && r.ReferencedType == (int)EntityName.Client)).FirstOrDefault();

            if (appropriateAppointment == null || appropriatePhonecall == null || appropriateTask == null || activityWithClient == null)
            {
                return OrdinaryTestResult.As.NotExecuted;
            }

            var appointment = _activityReadModel.GetAppointment(appropriateAppointment.Id);
            var task = _activityReadModel.GetTask(appropriateTask.Id);
            var phonecall = _activityReadModel.GetPhonecall(appropriatePhonecall.Id);

            // ReSharper disable once PossibleInvalidOperationException
			_activityReadModel.CheckIfRelatedActivitiesExists(activityWithClient.AppointmentReferences.First(r => r.ReferencedType == (int)EntityName.Client).ReferencedObjectId);

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