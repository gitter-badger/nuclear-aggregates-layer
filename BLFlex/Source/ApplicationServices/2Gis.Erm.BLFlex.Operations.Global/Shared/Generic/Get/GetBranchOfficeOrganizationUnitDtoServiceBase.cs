using System;

using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.OrganizationUnits.ReadModel;
using DoubleGis.Erm.BLCore.Operations.Generic.Get;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared.Generic.Get
{
    // FIXME {d.ivanov, 17.06.2014}: Убрать этот базовый класс, см. комментарии к GetLegalPersonDtoServiceBase
    [Obsolete]
    public abstract class GetBranchOfficeOrganizationUnitDtoServiceBase<TDto> : GetDomainEntityDtoServiceBase<BranchOfficeOrganizationUnit>
        where TDto : IDomainEntityDto<BranchOfficeOrganizationUnit>, new()
    {
        private readonly IBranchOfficeReadModel _branchOfficeReadModel;
        private readonly IOrganizationUnitReadModel _organizationUnitReadModel;

        protected GetBranchOfficeOrganizationUnitDtoServiceBase(
            IUserContext userContext,
            IBranchOfficeReadModel branchOfficeReadModel,
            IOrganizationUnitReadModel organizationUnitReadModel)
            : base(userContext)
        {
            _branchOfficeReadModel = branchOfficeReadModel;
            _organizationUnitReadModel = organizationUnitReadModel;
        }

        protected override IDomainEntityDto<BranchOfficeOrganizationUnit> GetDto(long entityId)
        {
            var boou = _branchOfficeReadModel.GetBranchOfficeOrganizationUnit(entityId);

            var dto = GetProjectSpecification().Project(boou);

            var organizationUnitRef = dto.GetPropertyValue<TDto, EntityReference>("OrganizationUnitRef");
            if (organizationUnitRef.Id.HasValue)
            {
                organizationUnitRef.Name = _organizationUnitReadModel.GetName(organizationUnitRef.Id.Value);
            }
            
            var branchOfficeRef = dto.GetPropertyValue<TDto, EntityReference>("BranchOfficeRef");
            if (branchOfficeRef.Id.HasValue)
            {
                branchOfficeRef.Name = _branchOfficeReadModel.GetBranchOfficeName(branchOfficeRef.Id.Value);
                SetSpecificPropertyValues(dto);
            }

            return dto;
        }

        protected virtual void SetSpecificPropertyValues(TDto dto)
        {
            // do nothing
        }

        protected abstract IProjectSpecification<BranchOfficeOrganizationUnit, TDto> GetProjectSpecification();

        protected override IDomainEntityDto<BranchOfficeOrganizationUnit> CreateDto(long? parentEntityId, EntityName parentEntityName, string extendedInfo)
        {
            var dto = new TDto();

            switch (parentEntityName)
            {
                case EntityName.BranchOffice:
                {
                    var branchOfficeName = _branchOfficeReadModel.GetBranchOfficeName(parentEntityId.Value);

                    dto.SetPropertyValue("BranchOfficeRef", new EntityReference { Id = parentEntityId.Value, Name = branchOfficeName });
                    dto.SetPropertyValue("ShortLegalName", branchOfficeName); 
                    break;
                }

                case EntityName.OrganizationUnit:
                {
                    dto.SetPropertyValue("OrganizationUnitRef",
                                         new EntityReference { Id = parentEntityId.Value, Name = _organizationUnitReadModel.GetName(parentEntityId.Value) });
                    break;
                }
            }

            return dto;
        }
    }
}