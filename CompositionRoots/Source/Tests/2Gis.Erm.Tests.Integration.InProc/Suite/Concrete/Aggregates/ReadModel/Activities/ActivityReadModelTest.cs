using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using NuClear.Model.Common.Entities;
using NuClear.Storage;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Aggregates.ReadModel.Activities
{
    public class ActivityReadModelTest : IIntegrationTest
    {
        private readonly IFinder _finder;
        private readonly IAppointmentReadModel _appointmentReadModel;
        private readonly ILetterReadModel _letterReadModel;
        private readonly IPhonecallReadModel _phonecallReadModel;
        private readonly ITaskReadModel _taskReadModel;

        public ActivityReadModelTest(
            IFinder finder,
            IAppointmentReadModel appointmentReadModel,
            ILetterReadModel letterReadModel,
            IPhonecallReadModel phonecallReadModel,
            ITaskReadModel taskReadModel)
        {
            _finder = finder;
            _appointmentReadModel = appointmentReadModel;
            _letterReadModel = letterReadModel;
            _phonecallReadModel = phonecallReadModel;
            _taskReadModel = taskReadModel;
        }

        public ITestResult Execute()
        {
            var appropriateAppointment = _finder.FindMany(Specs.Find.Active<Appointment>()).FirstOrDefault();
            var appropriateLetter = _finder.FindMany(Specs.Find.Active<Letter>()).FirstOrDefault();
            var appropriatePhonecall = _finder.FindMany(Specs.Find.Active<Phonecall>()).FirstOrDefault();
            var appropriateTask = _finder.FindMany(Specs.Find.Active<Task>()).FirstOrDefault();
            var clientTypeId = EntityType.Instance.Client().Id;
            var reference = _finder.FindMany(Specs.Find.Custom<AppointmentRegardingObject>(x => x.TargetEntityTypeId == clientTypeId)).FirstOrDefault();
            var activityWithClient = reference != null ? _finder.FindMany(Specs.Find.ById<Appointment>(reference.SourceEntityId)) : null;

            if (appropriateAppointment == null || appropriateLetter == null || appropriatePhonecall == null || appropriateTask == null || activityWithClient == null)
            {
                return OrdinaryTestResult.As.NotExecuted;
            }

            var appointment = _appointmentReadModel.GetAppointment(appropriateAppointment.Id);
            var letter = _letterReadModel.GetLetter(appropriateLetter.Id);
            var phonecall = _phonecallReadModel.GetPhonecall(appropriatePhonecall.Id);
            var task = _taskReadModel.GetTask(appropriateTask.Id);

            // ReSharper disable once PossibleInvalidOperationException
            _appointmentReadModel.CheckIfAppointmentExistsRegarding(EntityType.Instance.Client(), reference.TargetEntityId);

            return new object[] { appointment, letter, task, phonecall }
                       .Any(x => x == null)
                       ? OrdinaryTestResult.As.Failed
                       : OrdinaryTestResult.As.Succeeded;
        }
    }
}