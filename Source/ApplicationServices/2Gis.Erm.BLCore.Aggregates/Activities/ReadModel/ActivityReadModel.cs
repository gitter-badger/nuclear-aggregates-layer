using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Activities.ReadModel
{
    public class ActivityReadModel : IActivityReadModel
    {
        private readonly IFinder _finder;

        public ActivityReadModel(IFinder finder)
        {
            _finder = finder;
        }

        public Task GetTask(long taskId)
        {
			return _finder.FindOne(Specs.Find.ById<Task>(taskId));
        }

	    public Phonecall GetPhonecall(long phonecallId)
        {
			return _finder.FindOne(Specs.Find.ById<Phonecall>(phonecallId));
        }

        public Appointment GetAppointment(long appointmentId)
        {
			return _finder.FindOne(Specs.Find.ById<Appointment>(appointmentId));
        }

        public bool CheckIfRelatedActivitiesExists(long clientId)
        {
			return
				_finder.Find<AppointmentBase>(x => true).Select(x => new 
				{
					References = x.AppointmentReferences.Where(r => r.Reference == (int)ReferenceType.RegardingObject && r.ReferencedObjectId == clientId).Select(r => r.ReferencedObjectId),
				})
				.Concat(_finder.Find<PhonecallBase>(x => true).Select(x => new 
				{
					References = x.PhonecallReferences.Where(r => r.Reference == (int)ReferenceType.RegardingObject && r.ReferencedObjectId == clientId).Select(r => r.ReferencedObjectId),
				}))
				.Concat(_finder.Find<TaskBase>(x => true).Select(x => new 
				{
					References = x.TaskReferences.Where(r => r.Reference == (int)ReferenceType.RegardingObject && r.ReferencedObjectId == clientId).Select(r => r.ReferencedObjectId),
				})).Any();
        }
   }
}