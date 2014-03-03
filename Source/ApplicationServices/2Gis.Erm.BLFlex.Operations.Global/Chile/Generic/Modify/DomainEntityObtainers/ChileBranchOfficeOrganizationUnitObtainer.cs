using System;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.Aggregates.Dynamic.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.EAV;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Chile.Generic.Modify.DomainEntityObtainers
{
    public class ChileBranchOfficeOrganizationUnitObtainer : IBusinessModelEntityObtainer<BranchOfficeOrganizationUnit>, IAggregateReadModel<BranchOffice>, IChileAdapted
    {
        private readonly IDynamicEntityPropertiesConverter<BranchOfficeOrganizationUnitPart, BusinessEntityInstance, BusinessEntityPropertyInstance> _partPropertiesConverter;
        private readonly IFinder _finder;

        public ChileBranchOfficeOrganizationUnitObtainer(
            IDynamicEntityPropertiesConverter<BranchOfficeOrganizationUnitPart, BusinessEntityInstance, BusinessEntityPropertyInstance> partPropertiesConverter,
            IFinder finder)
        {
            _partPropertiesConverter = partPropertiesConverter;
            _finder = finder;
        }

        public BranchOfficeOrganizationUnit ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (ChileBranchOfficeOrganizationUnitDomainEntityDto)domainEntityDto;

            var entity = _finder.Find(Specs.Find.ById<BranchOfficeOrganizationUnit>(dto.Id)).SingleOrDefault()
                                     ?? new BranchOfficeOrganizationUnit { IsActive = true };
            
            if (dto.Timestamp == null && entity.Timestamp != null)
            {
                throw new BusinessLogicException(string.Format(BLResources.CannotCreateObjectWithSpecifiedId, dto.Id));
            }

            CopyFields(dto, entity);

            var entityPart = entity.IsNew()
                                ? new BranchOfficeOrganizationUnitPart()
                                : _finder.SingleOrDefault(dto.Id, _partPropertiesConverter.ConvertFromDynamicEntityInstance);

            entityPart.RepresentativeRut = dto.RepresentativeRut; // RUT представителя

            entity.Parts = new[] { entityPart };
            return entity;
        }

        private static void CopyFields(ChileBranchOfficeOrganizationUnitDomainEntityDto dto, BranchOfficeOrganizationUnit entity)
        {
            entity.ShortLegalName = dto.ShortLegalName;

            entity.BranchOfficeId = dto.BranchOfficeRef.Id.Value;

            entity.OrganizationUnitId = dto.OrganizationUnitRef.Id.Value;
            entity.PhoneNumber = dto.PhoneNumber;

            entity.ChiefNameInNominative = dto.ChiefNameInNominative; // представитель

            entity.PositionInNominative = dto.PositionInNominative; // должность представителя
            entity.RegistrationCertificate = dto.RegistrationCertificate;

            entity.ActualAddress = dto.ActualAddress;
            entity.Email = dto.Email;

            entity.PostalAddress = dto.PostalAddress;

            entity.PaymentEssentialElements = dto.PaymentEssentialElements;

            entity.Timestamp = dto.Timestamp;
        }
    }
}
