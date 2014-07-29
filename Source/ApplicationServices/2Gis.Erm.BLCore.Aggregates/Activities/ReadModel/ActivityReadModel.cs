using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

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
			return ResolveReferences(_finder.FindOne(Specs.Find.ById<Task>(taskId)));
        }

        public Phonecall GetPhonecall(long phonecallId)
        {
			return ResolveReferences(_finder.FindOne(Specs.Find.ById<Phonecall>(phonecallId)));
        }

        public Appointment GetAppointment(long appointmentId)
        {
			return ResolveReferences(_finder.FindOne(Specs.Find.ById<Appointment>(appointmentId)));
        }

        public bool CheckIfRelatedActivitiesExists(long clientId)
        {
            var hasActivitiesInProgress = GetActivityInProgressDtosQuery(clientId).Any();
            return hasActivitiesInProgress;
        }

		private IQueryable<DictionaryEntityInstance> GetActivityInProgressDtosQuery(long clientId)
        {
            var clientRelatedEntitiesDto = _finder.Find(Specs.Find.ById<Client>(clientId))
                                                  .Select(x => new
                                                      {
                                                          FirmsIds = x.Firms.Select(y => y.Id),
                                                          DealIds = x.Deals.Select(y => y.Id),
                                                          ContactIds = x.Contacts.Select(y => y.Id),
                                                      })
                                                  .Single();

			return _finder.Find(Specs.Find.ActiveAndNotDeleted<DictionaryEntityInstance>()
								&& ActivitySpecs.Activity.Find.OnlyActivities()
			                    && ActivitySpecs.Activity.Find.InProgress()
			                    && ActivitySpecs.Activity.Find.RelatedToClient(clientId,
			                                                                   clientRelatedEntitiesDto.FirmsIds,
			                                                                   clientRelatedEntitiesDto.ContactIds,
			                                                                   clientRelatedEntitiesDto.DealIds));
        }

		private TActivity ResolveReferences<TActivity>(TActivity activity) where TActivity : ActivityBase
		{
			if (activity != null)
			{
				ResolveReference<Client>(activity.ClientId, client => activity.ClientName = client.Name);
				ResolveReference<Contact>(activity.ContactId, contact => activity.ContactName = contact.FullName);
				ResolveReference<Deal>(activity.DealId, deal => activity.DealName = deal.Name);
				ResolveReference<Firm>(activity.FirmId, firm => activity.FirmName = firm.Name);
			}

			return activity;
		}

		private void ResolveReference<TEntity>(long? id, Action<TEntity> action) where TEntity : class, IEntity, IEntityKey
	    {
			if (!id.HasValue) return;
			
			var entity = _finder.FindOne(Specs.Find.ById<TEntity>(id.Value));
			if (entity == null) return;
			
			action(entity);
	    }
    }
}