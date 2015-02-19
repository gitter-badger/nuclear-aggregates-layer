using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Deals.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get.Activity
{
    public class ActivityReferenceReader : IActivityReferenceReader
    {
        private readonly IAppointmentReadModel _appointmentReadModel;
        private readonly ITaskReadModel _taskReadModel;
        private readonly ILetterReadModel _letterReadModel;
        private readonly IPhonecallReadModel _phonecallReadModel;
        private readonly IClientReadModel _clientReadModel;
        private readonly IDealReadModel _dealReadModel;
        private readonly IFirmReadModel _firmReadModel;

        public ActivityReferenceReader(
            IAppointmentReadModel appointmentReadModel,
            ITaskReadModel taskReadModel,
            ILetterReadModel letterReadModel,
            IPhonecallReadModel phonecallReadModel,          
            IClientReadModel clientReadModel,
            IDealReadModel dealReadModel,
            IFirmReadModel firmReadModel)
        {
            _appointmentReadModel = appointmentReadModel;
            _taskReadModel = taskReadModel;
            _letterReadModel = letterReadModel;
            _phonecallReadModel = phonecallReadModel;
            _clientReadModel = clientReadModel;
            _dealReadModel = dealReadModel;
            _firmReadModel = firmReadModel;
        }

        public IEnumerable<EntityReference> GetRegardingObjects(EntityName entityName, long entityId)
        {
            switch (entityName)
            {
                case EntityName.Appointment:
                    return AdaptReferences(_appointmentReadModel.GetRegardingObjects(entityId));
                case EntityName.Task:
                    return AdaptReferences(_taskReadModel.GetRegardingObjects(entityId));
                case EntityName.Letter:
                    return AdaptReferences(_letterReadModel.GetRegardingObjects(entityId));
                case EntityName.Phonecall:
                    return AdaptReferences(_phonecallReadModel.GetRegardingObjects(entityId));
                default: throw new NotSupportedException("entityName");
            }
        }

        public IEnumerable<EntityReference> GetAttendees(EntityName entityName, long entityId)
        {
            switch (entityName)
            {
                case EntityName.Appointment:
                    return AdaptReferences(_appointmentReadModel.GetAttendees(entityId));
                case EntityName.Task:
                    return Enumerable.Empty<EntityReference>();
                case EntityName.Letter:
                    return AdaptReferences(new[] { _letterReadModel.GetRecipient(entityId) });
                case EntityName.Phonecall:
                    return AdaptReferences(new[] { _phonecallReadModel.GetRecipient(entityId) });
                default:
                    throw new NotSupportedException("entityName");
            }
        }

        private EntityReference ToEntityReference<TEntity>(EntityReference<TEntity> entity) where TEntity : IEntity
        {
            if (entity == null)
            {
                return null;
            }

            string name;
            switch (entity.TargetEntityName)
            {
                case EntityName.Client:
                    name = _clientReadModel.GetClientName(entity.TargetEntityId);
                    break;
                case EntityName.Deal:
                    name = _dealReadModel.GetDeal(entity.TargetEntityId).Name;
                    break;
                case EntityName.Firm:
                    name = _firmReadModel.GetFirmName(entity.TargetEntityId);
                    break;
                case EntityName.Contact:
                    name = _clientReadModel.GetContactName(entity.TargetEntityId);
                    break;
                default:
                    return null;
            }

            return new EntityReference { Id = entity.TargetEntityId, Name = name, EntityName = entity.TargetEntityName };
        }

        private IEnumerable<EntityReference> AdaptReferences<TEntity>(IEnumerable<EntityReference<TEntity>> references) where TEntity : IEntity
        {
            return references.Select(ToEntityReference).Where(x => x != null).ToList();
        }
    }
}
