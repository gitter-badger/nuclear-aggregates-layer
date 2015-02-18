using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Read;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Custom
{
    public sealed class ActivityReadService : IActivityReadService
    {
        private readonly IAppointmentReadModel _appointmentReadModel;
        private readonly ILetterReadModel _letterReadModel;

        private readonly ISecurityServiceUserIdentifier _userIdentifierService;

        private readonly IFinder _finder;

        private readonly IPhonecallReadModel _phonecallReadModel;
        private readonly ITaskReadModel _taskReadModel;

        public ActivityReadService(
            IAppointmentReadModel appointmentReadModel, 
            ILetterReadModel letterReadModel,
            ISecurityServiceUserIdentifier userIdentifierService,
            IFinder finder,
            IPhonecallReadModel phonecallReadModel, 
            ITaskReadModel taskReadModel)
        {
            _appointmentReadModel = appointmentReadModel;
            _letterReadModel = letterReadModel;
            _userIdentifierService = userIdentifierService;
            _finder = finder;
            _phonecallReadModel = phonecallReadModel;
            _taskReadModel = taskReadModel;
        }

        public bool CheckIfActivityExistsRegarding(EntityName entityName, long entityId)
        {
            return
                _appointmentReadModel.CheckIfAppointmentExistsRegarding(entityName, entityId)
                || _letterReadModel.CheckIfLetterExistsRegarding(entityName, entityId)
                || _phonecallReadModel.CheckIfPhonecallExistsRegarding(entityName, entityId)
                || _taskReadModel.CheckIfTaskExistsRegarding(entityName, entityId);
        }

        public bool CheckIfOpenActivityExistsRegarding(EntityName entityName, long entityId)
        {
            return
                _appointmentReadModel.CheckIfOpenAppointmentExistsRegarding(entityName, entityId)
                || _letterReadModel.CheckIfOpenLetterExistsRegarding(entityName, entityId)
                || _phonecallReadModel.CheckIfOpenPhonecallExistsRegarding(entityName, entityId)
                || _taskReadModel.CheckIfOpenTaskExistsRegarding(entityName, entityId);
        }

        public IEnumerable<IEntity> LookupActivitiesRegarding(EntityName entityName, long entityId)
        {
            return
                _appointmentReadModel.LookupAppointmentsRegarding(entityName, entityId).Cast<IEntity>()
                                     .Concat(_letterReadModel.LookupLettersRegarding(entityName, entityId))
                                     .Concat(_phonecallReadModel.LookupPhonecallsRegarding(entityName, entityId))
                                     .Concat(_taskReadModel.LookupTasksRegarding(entityName, entityId));
        }
       
        public void CheckIfAnyEntityReferencesContainsReserve(IEnumerable<EntityReference> references)
        {
            var entityReferences = references as EntityReference[] ?? references.ToArray();
            CheckIfAnySpecificEntityReferencesContainsReserve<Client>(entityReferences, EntityName.Client);
            CheckIfAnySpecificEntityReferencesContainsReserve<Deal>(entityReferences, EntityName.Deal); 
            CheckIfAnySpecificEntityReferencesContainsReserve<Contact>(entityReferences, EntityName.Contact);
            CheckIfAnySpecificEntityReferencesContainsReserve<Firm>(entityReferences, EntityName.Firm);
        }

        public void CheckIfEntityReferencesContainsReserve(EntityReference reference)
        {
            CheckIfAnyEntityReferencesContainsReserve(new[] { reference });
        }

        private void CheckIfAnySpecificEntityReferencesContainsReserve<TCuratedEntity>(IEnumerable<EntityReference> references, EntityName entityName)         
            where TCuratedEntity : class, ICuratedEntity, IEntity, IEntityKey
        {            
            var reserveCode = _userIdentifierService.GetReserveUserIdentity().Code;
            var entityIds = references.Where(s => s.EntityName == entityName).Select(s => s.Id != null ? (long)s.Id : 0).ToArray();

            if (entityIds.Any())
            {
                var clients = _finder.FindMany(Specs.Find.ByIds<TCuratedEntity>(entityIds));
                if (clients.Any(s => s.OwnerCode == reserveCode))
                {
                    throw new BusinessLogicException(string.Format(BLResources.CannotSaveActivityForObjectInReserve, entityName.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture)));
                }
            }
        }
    }
}