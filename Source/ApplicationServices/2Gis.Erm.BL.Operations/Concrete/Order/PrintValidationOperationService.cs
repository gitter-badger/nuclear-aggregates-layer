using System;
using System.Linq;

using DoubleGis.Erm.BL.API.Operations.Concrete.Order;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;


namespace DoubleGis.Erm.BL.Operations.Concrete.Order
{
    public sealed class PrintValidationOperationService : IPrintValidationOperationService
    {
        private readonly IFinder _finder;

        public PrintValidationOperationService(IFinder finder)
        {
            _finder = finder;
        }

        public void ValidateOrder(long orderId)
        {
            Validate(Specs.Find.ById<Platform.Model.Entities.Erm.Order>(orderId), OrderSpecs.Orders.Select.OrderPrintValidationDto());
        }

        public void ValidateBill(long billId)
        {
            Validate(Specs.Find.ById<Platform.Model.Entities.Erm.Bill>(billId), OrderSpecs.Bills.Select.OrderPrintValidationDto());
        }

        public void Validate<T>(IFindSpecification<T> filter, ISelectSpecification<T, OrderPrintValidationDto> projection) 
            where T : class, IEntity
        {
            var dtos = _finder.Find(projection, filter).Take(2).ToArray();

            if (dtos.Length > 1)
            {
                // ReSharper disable once LocalizableElement
                throw new ArgumentException("Неоднозначный фильтр", "filter");
            }

            if (dtos.Length < 1)
            {
                throw new EntityNotFoundException(typeof(T));
            }

            var dto = dtos.Single();

            if (dto.LegalPersonId == null)
            {
                throw new RequiredFieldIsEmptyException(string.Format(BLResources.OrderFieldNotSpecified, MetadataResources.LegalPerson));
            }

            if (dto.LegalPersonProfileId == null)
            {
                throw new RequiredFieldIsEmptyException(string.Format(BLResources.OrderFieldNotSpecified, MetadataResources.LegalPersonProfile));
            }

            if (dto.BranchOfficeOrganizationUnitId == null)
            {
                throw new RequiredFieldIsEmptyException(string.Format(BLResources.OrderFieldNotSpecified, MetadataResources.BranchOfficeOrganizationUnit));
            }
        }
    }
}
