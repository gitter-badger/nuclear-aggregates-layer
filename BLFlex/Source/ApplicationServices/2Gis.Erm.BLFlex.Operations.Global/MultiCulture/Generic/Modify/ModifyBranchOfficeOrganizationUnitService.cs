using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.OrganizationUnits.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLFlex.API.Operations.Global.MultiCulture.Operations.Modify;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic.Modify
{
    public class ModifyBranchOfficeOrganizationUnitService : IModifyBusinessModelEntityService<BranchOfficeOrganizationUnit>, IRussiaAdapted, ICyprusAdapted,
                                                             ICzechAdapted, IChileAdapted, IUkraineAdapted, IEmiratesAdapted, IKazakhstanAdapted
    {
        private readonly IBusinessModelEntityObtainer<BranchOfficeOrganizationUnit> _obtainer;
        private readonly ICreateAggregateRepository<BranchOfficeOrganizationUnit> _createRepository;
        private readonly IUpdateAggregateRepository<BranchOfficeOrganizationUnit> _updateRepository;
        private readonly IPartableEntityValidator<BranchOfficeOrganizationUnit> _validator;
        private readonly IBranchOfficeReadModel _branchOfficeReadModel;
        private readonly IOrganizationUnitReadModel _organizationUnitReadModel;

        public ModifyBranchOfficeOrganizationUnitService(
            IBusinessModelEntityObtainer<BranchOfficeOrganizationUnit> obtainer,
            ICreateAggregateRepository<BranchOfficeOrganizationUnit> createService,
            IUpdateAggregateRepository<BranchOfficeOrganizationUnit> updateService,
            IPartableEntityValidator<BranchOfficeOrganizationUnit> validator,
            IBranchOfficeReadModel branchOfficeReadModel,
            IOrganizationUnitReadModel organizationUnitReadModel)
        {
            _obtainer = obtainer;
            _createRepository = createService;
            _updateRepository = updateService;
            _validator = validator;
            _branchOfficeReadModel = branchOfficeReadModel;
            _organizationUnitReadModel = organizationUnitReadModel;
        }

        public long Modify(IDomainEntityDto domainEntityDto)
        {
            var entity = _obtainer.ObtainBusinessModelEntity(domainEntityDto);

            var branchOffice = _branchOfficeReadModel.GetBranchOffice(entity.BranchOfficeId);

            if (branchOffice.IsDeleted || !branchOffice.IsActive)
            {
                throw new NotificationException(BLResources.ActiveAndNotDeletedBranchOfficeRequired);
            }

            var organizationUnit = _organizationUnitReadModel.GetOrganizationUnit(entity.OrganizationUnitId);

            if (organizationUnit.IsDeleted || !organizationUnit.IsActive)
            {
                throw new NotificationException(BLResources.ActiveAndNotDeletedOrganizationUnitRequired);
            }

            var duplicate = _branchOfficeReadModel.GetBranchOfficeOrganizationUnitDuplicate(entity.OrganizationUnitId, entity.BranchOfficeId, entity.Id);
            if (duplicate != null)
            {
                var messageTemplate = duplicate.IsActive
                                          ? BLResources.OrgUnitForBranchOfficeAlreadyExist
                                          : BLResources.InactiveOrgUnitForBranchOfficeAlreadyExist;

                throw new NotificationException(string.Format(messageTemplate,
                                                              duplicate.OrganizationUnitName,
                                                              duplicate.BranchOfficeName));
            }

            var primaryBranchOfficeOrganizationUnits = _branchOfficeReadModel.GetPrimaryBranchOfficeOrganizationUnits(entity.OrganizationUnitId);

            if (primaryBranchOfficeOrganizationUnits.Primary == null)
            {
                // если нет основного то назначим
                entity.IsPrimary = true;
            }
            else if (primaryBranchOfficeOrganizationUnits.Primary.Id != entity.Id)
            {
                // если основной уже есть, то признак снимется
                entity.IsPrimary = false;
            }

            if (primaryBranchOfficeOrganizationUnits.PrimaryForRegionalSales == null)
            {
                // первый добавленный автоматически будет назначен основным
                entity.IsPrimaryForRegionalSales = true;
            }
            else if (primaryBranchOfficeOrganizationUnits.PrimaryForRegionalSales.Id != entity.Id)
            {
                // если основной уже есть, то признак снимется
                entity.IsPrimaryForRegionalSales = false;
            }

            _validator.Check(entity);

            if (entity.IsNew())
            {
                _createRepository.Create(entity);
            }
            else
            {
                _updateRepository.Update(entity);
            }

            return entity.Id;
        }
    }
}
