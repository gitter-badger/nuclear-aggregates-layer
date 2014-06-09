using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared.Generic.Modify
{
    public abstract class BranchOfficeObtainerBase<TDto, TEntity> : IBusinessModelEntityObtainer<TEntity>, IAggregateReadModel<BranchOffice>
        where TDto : IDomainEntityDto<TEntity>
        where TEntity : class, IEntity, IEntityKey, IPartable, IDeactivatableEntity, IStateTrackingEntity, new()
    {
        private readonly IFinder _finder;

        protected BranchOfficeObtainerBase(IFinder finder)
        {
            _finder = finder;
        }

        public TEntity ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (TDto)domainEntityDto;

            var entity = _finder.FindOne(Specs.Find.ById<TEntity>(dto.Id)) ?? CreateEntity();

            if (dto.GetPropertyValue("Timestamp") == null && entity.Timestamp != null)
            {
                throw new BusinessLogicException(string.Format(BLResources.CannotCreateObjectWithSpecifiedId, dto.Id));
            }

            GetAssignSpecification().Assign(dto, entity);

            return entity;
        }

        protected virtual TEntity CreateEntity()
        {
            return new TEntity { IsActive = true };
        }

        protected abstract IAssignSpecification<TDto, TEntity> GetAssignSpecification();
    }

    public abstract class BranchOfficeObtainerBase<TDto> : BranchOfficeObtainerBase<TDto, BranchOffice> 
        where TDto : IDomainEntityDto<BranchOffice>
    {
        protected BranchOfficeObtainerBase(IFinder finder) : base(finder)
        {
        }
    }

    public abstract class BranchOfficeOrganizationUnitObtainerBase<TDto> : BranchOfficeObtainerBase<TDto, BranchOfficeOrganizationUnit>
        where TDto : IDomainEntityDto<BranchOfficeOrganizationUnit>
    {
        protected BranchOfficeOrganizationUnitObtainerBase(IFinder finder)
            : base(finder)
        {
        }
    }
}
