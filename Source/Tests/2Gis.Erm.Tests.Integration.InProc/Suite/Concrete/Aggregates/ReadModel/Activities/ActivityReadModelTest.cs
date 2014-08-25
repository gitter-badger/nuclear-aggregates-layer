using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
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
	        var appropriateAppointment = _finder.FindMany(Specs.Find.Any<Appointment>()).FirstOrDefault();
	        var appropriatePhonecall = _finder.FindMany(Specs.Find.Any<Phonecall>()).FirstOrDefault();
	        var appropriateTask = _finder.FindMany(Specs.Find.Any<Task>()).FirstOrDefault();
	        var reference = _finder.FindMany(Specs.Find.Custom<RegardingObject<Appointment>>(x => x.TargetEntityName == EntityName.Client)).FirstOrDefault();
	        var activityWithClient = reference != null ? _finder.FindMany(Specs.Find.ById<Appointment>(reference.SourceEntityId)) : null;

	        if (appropriateAppointment == null || appropriatePhonecall == null || appropriateTask == null || activityWithClient == null)
	        {
		        return OrdinaryTestResult.As.NotExecuted;
	        }

	        var appointment = _activityReadModel.GetAppointment(appropriateAppointment.Id);
	        var task = _activityReadModel.GetTask(appropriateTask.Id);
	        var phonecall = _activityReadModel.GetPhonecall(appropriatePhonecall.Id);

	        // ReSharper disable once PossibleInvalidOperationException
	        _activityReadModel.CheckIfRelatedActivitiesExists(reference.TargetEntityId);

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