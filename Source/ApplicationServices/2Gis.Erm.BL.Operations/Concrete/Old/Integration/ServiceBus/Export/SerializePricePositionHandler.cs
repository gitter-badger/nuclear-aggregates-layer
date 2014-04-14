﻿using System.Xml.Linq;

using DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export;
using DoubleGis.Erm.BLCore.Operations.Concrete.Old.Integration.ServiceBus.Export;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BL.Operations.Concrete.Old.Integration.ServiceBus.Export
{
    public sealed class SerializePricePositionHandler : SerializeObjectsHandler<PricePosition>
    {
        public SerializePricePositionHandler(IExportRepository<PricePosition> exportOperationsRepository,
                                      ICommonLog logger)
            : base(exportOperationsRepository, logger)
        {
        }

        protected override string GetXsdSchemaContent(string schemaName)
        {
            return Properties.Resources.ResourceManager.GetString(schemaName);
        }

        protected override XElement SerializeDtoToXElement(IExportableEntityDto entiryDto)
        {
            var pricePositionDto = (PricePositionDto)entiryDto;

            var pricePositionElement = new XElement("PriceListPosition",
                                            new XAttribute("Code", pricePositionDto.Id),
                                            new XAttribute("PriceListCode", pricePositionDto.PriceCode),
                                            new XAttribute("NomenclatureCode", pricePositionDto.PositionId),
                                            new XAttribute("Cost", pricePositionDto.Cost),
                                            new XAttribute("IsHidden", pricePositionDto.IsHidden));

            return pricePositionElement;
        }

        protected override ISelectSpecification<PricePosition, IExportableEntityDto> CreateDtoExpression()
        {
            return new SelectSpecification<PricePosition, IExportableEntityDto>(x =>
                                                                                new PricePositionDto
                                                                                    {
                                                                                        Id = x.Id,
                                                                                        PriceCode = x.PriceId,
                                                                                        PositionId = x.PositionId,
                                                                                        Cost = x.Cost,
                                                                                        IsHidden = !x.IsActive
                                                                                    });
        }

        #region nested types

        private sealed class PricePositionDto : IExportableEntityDto
        {
            public long Id { get; set; }
            public long PriceCode { get; set; }
            public long PositionId { get; set; }
            public decimal Cost { get; set; }
            public bool IsHidden { get; set; }
        }

        #endregion
    }
}
