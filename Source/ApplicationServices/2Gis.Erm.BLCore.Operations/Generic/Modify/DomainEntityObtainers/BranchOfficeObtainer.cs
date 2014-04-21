using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.DomainEntityObtainers
{
    // TODO {all, 07.04.2014}: в  целом по obtainers см. коммент к IBusinessModelEntityObtainerFlex, до выработки болееменее четкой идеологии дальнейшего развития предлагаю пока дальше obtainers такого типа не масштабировать/клонировать
    public sealed class BranchOfficeObtainer : IBusinessModelEntityObtainer<BranchOffice>, IAggregateReadModel<BranchOffice>
    {
        private readonly IFinder _finder;
        private readonly IBusinessModelEntityObtainerFlex<BranchOffice> _flexBehaviour;

        public BranchOfficeObtainer(IFinder finder, IBusinessModelEntityObtainerFlex<BranchOffice> flexBehaviour)
        {
            _finder = finder;
            _flexBehaviour = flexBehaviour;
        }

        public BranchOffice ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (BranchOfficeDomainEntityDto)domainEntityDto;

            var entity = _finder.Find(Specs.Find.ById<BranchOffice>(dto.Id)).SingleOrDefault() ??
                new BranchOffice { IsActive = true, Id = dto.Id };

            if (dto.Timestamp == null && entity.Timestamp != null)
            {
                throw new BusinessLogicException(string.Format(BLResources.CannotCreateObjectWithSpecifiedId, dto.Id));
            }

            entity.DgppId = dto.DgppId;
            entity.Name = dto.Name;
            entity.Inn = dto.Inn;
            entity.Ic = dto.Ic;
            entity.Annotation = dto.Annotation;
            entity.BargainTypeId = dto.BargainTypeRef.Id;
            entity.ContributionTypeId = dto.ContributionTypeRef.Id;
            entity.LegalAddress = dto.LegalAddress;
            entity.UsnNotificationText = dto.UsnNotificationText;
            entity.Timestamp = dto.Timestamp;

            entity.Parts = _flexBehaviour.GetEntityParts(entity);

            _flexBehaviour.CopyPartFields(entity, dto);

            return entity;
        }
    }
}