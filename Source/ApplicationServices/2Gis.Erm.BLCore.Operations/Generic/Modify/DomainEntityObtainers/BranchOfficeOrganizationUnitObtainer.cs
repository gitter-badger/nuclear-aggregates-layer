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
    public sealed class BranchOfficeOrganizationUnitObtainer : IBusinessModelEntityObtainer<BranchOfficeOrganizationUnit>, IAggregateReadModel<BranchOffice>
    {
        private readonly IFinder _finder;

        public BranchOfficeOrganizationUnitObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public BranchOfficeOrganizationUnit ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (BranchOfficeOrganizationUnitDomainEntityDto)domainEntityDto;

            var entity = _finder.FindOne(Specs.Find.ById<BranchOfficeOrganizationUnit>(dto.Id)) ??
                         new BranchOfficeOrganizationUnit { IsActive = true, Id = dto.Id };

            if (dto.Timestamp == null && entity.Timestamp != null)
            {
                throw new BusinessLogicException(string.Format(BLResources.CannotCreateObjectWithSpecifiedId, dto.Id));
            }

            entity.OrganizationUnitId = dto.OrganizationUnitRef.Id.Value;
            entity.BranchOfficeId = dto.BranchOfficeRef.Id.Value;
            entity.ChiefNameInGenitive = dto.ChiefNameInGenitive;
            entity.ChiefNameInNominative = dto.ChiefNameInNominative;
            entity.Registered = dto.Registered;
            entity.OperatesOnTheBasisInGenitive = dto.OperatesOnTheBasisInGenitive;
            entity.Kpp = dto.Kpp;
            entity.ActualAddress = dto.ActualAddress;
            entity.PostalAddress = dto.PostalAddress;
            entity.PaymentEssentialElements = dto.PaymentEssentialElements;
            entity.PhoneNumber = dto.PhoneNumber;
            entity.PositionInGenitive = dto.PositionInGenitive;
            entity.PositionInNominative = dto.PositionInNominative;
            entity.ShortLegalName = dto.ShortLegalName;
            entity.SyncCode1C = dto.SyncCode1C;
            entity.RegistrationCertificate = dto.RegistrationCertificate;
            entity.Email = dto.Email;
            entity.Timestamp = dto.Timestamp;

            return entity;
        }
    }
}