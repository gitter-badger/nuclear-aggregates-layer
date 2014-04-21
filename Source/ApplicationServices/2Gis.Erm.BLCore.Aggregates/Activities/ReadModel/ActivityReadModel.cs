using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Activities.DTO;
using DoubleGis.Erm.BLCore.Aggregates.Dynamic.ReadModel;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.EAV;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Activities.ReadModel
{
    public class ActivityReadModel : IActivityReadModel 
    {
        private readonly IFinder _finder;
        private readonly IActivityPropertiesConverter<Task> _taskPropertiesConverter;
        private readonly IActivityPropertiesConverter<Phonecall> _phonecallPropertiesConverter;
        private readonly IActivityPropertiesConverter<Appointment> _appointmentPropertiesConverter;

        public ActivityReadModel(IFinder finder,
                                 IActivityPropertiesConverter<Task> taskPropertiesConverter,
                                 IActivityPropertiesConverter<Phonecall> phonecallPropertiesConverter,
                                 IActivityPropertiesConverter<Appointment> appointmentPropertiesConverter)
        {
            _finder = finder;
            _taskPropertiesConverter = taskPropertiesConverter;
            _phonecallPropertiesConverter = phonecallPropertiesConverter;
            _appointmentPropertiesConverter = appointmentPropertiesConverter;
        }

        public TActivity GetActivity<TActivity>(long activityId) where TActivity : ActivityBase, new()
        {
            var converters = new Dictionary<Type, Func<long, ActivityBase>>
                {
                    { typeof(Task), GetTask },
                    { typeof(Phonecall), GetPhonecall },
                    { typeof(Appointment), GetAppointment },
                };

            Func<long, ActivityBase> getFunc;
            if (!converters.TryGetValue(typeof(TActivity), out getFunc))
            {
                throw new NotSupportedException(string.Format("Type {0} is not supported", typeof(TActivity).Name));
            }

            return (TActivity)getFunc(activityId);
        }

        public Task GetTask(long taskId)
        {
            return _finder.Single(taskId, _taskPropertiesConverter.ConvertFromDynamicEntityInstance);
        }

        public Phonecall GetPhonecall(long phonecallId)
        {
            return _finder.Single(phonecallId, _phonecallPropertiesConverter.ConvertFromDynamicEntityInstance);
        }

        public Appointment GetAppointment(long appointmentId)
        {
            return _finder.Single(appointmentId, _appointmentPropertiesConverter.ConvertFromDynamicEntityInstance);
        }

        public ActivityInstanceDto GetActivityInstanceDto(Task task)
        {
            return _finder.Single(task, _taskPropertiesConverter.ConvertToDynamicEntityInstance);
        }

        public ActivityInstanceDto GetActivityInstanceDto(Phonecall phonecall)
        {
            return _finder.Single(phonecall, _phonecallPropertiesConverter.ConvertToDynamicEntityInstance);
        }

        public ActivityInstanceDto GetActivityInstanceDto(Appointment appointment)
        {
            return _finder.Single(appointment, _appointmentPropertiesConverter.ConvertToDynamicEntityInstance);
        }

        public bool CheckIfRelatedActivitiesExists(long clientId)
        {
            var hasActivitiesInProgress = GetActivityInProgressDtosQuery(clientId).Any();
            return hasActivitiesInProgress;
        }

        public bool TryGetRelatedActivities(long clientId, out IEnumerable<ActivityInstance> activities)
        {
            activities = (from activityInstance in _finder.FindAll<ActivityInstance>()
                          join activityDto in GetActivityInProgressDtosQuery(clientId) on activityInstance.Id equals activityDto.Id
                          select activityInstance).ToArray();
            return activities.Any();
        }

        private IQueryable<ActivityDto> GetActivityInProgressDtosQuery(long clientId)
        {
            var clientRelatedEntitiesDto = _finder.Find(Specs.Find.ById<Client>(clientId))
                                                  .Select(x => new
                                                      {
                                                          FirmsIds = x.Firms.Select(y => y.Id),
                                                          DealIds = x.Deals.Select(y => y.Id),
                                                          ContactIds = x.Contacts.Select(y => y.Id),
                                                      })
                                                  .Single();

            return _finder.Find<ActivityInstance, ActivityDto>(ActivitySpecs.Activity.Select.ActivityDto(),
                                                               Specs.Find.ActiveAndNotDeleted<ActivityInstance>())
                          .Where(ActivitySpecs.Activity.Find.InProgress() &&
                                 ActivitySpecs.Activity.Find.RelatedToClient(clientId,
                                                                             clientRelatedEntitiesDto.FirmsIds,
                                                                             clientRelatedEntitiesDto.ContactIds,
                                                                             clientRelatedEntitiesDto.DealIds));
        }
    }
}