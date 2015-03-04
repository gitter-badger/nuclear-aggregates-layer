using System;
using System.Linq;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export;
using DoubleGis.Erm.BLCore.Operations.Concrete.Old.Integration.ServiceBus.Export;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BL.Operations.Concrete.Old.Integration.ServiceBus.Export
{
    public sealed class SerializePriceHandler : SerializeObjectsHandler<Price, ExportFlowPriceListsPriceList>
    {
        public SerializePriceHandler(IExportRepository<Price> exportOperationsRepository,
                                     ITracer logger)
            : base(exportOperationsRepository, logger)
        {
        }

        protected override string GetError(IExportableEntityDto entityDto)
        {
            var priceDto = (PriceDto)entityDto;

            if (priceDto.IsPublished
                && string.IsNullOrWhiteSpace(priceDto.CurrencyName))
            {
                return "У опубликованного прайса не задан код валюты";
            }

            if (priceDto.IsPublished
                && priceDto.ProjectCode == default(long))
            {
                return "У опубликованного прайса не задан код проекта";
            }

            return null;
        }

        // TODO {all, 04.03.2014}: После выполнения задачи ERM-3530 необходимо выгружать неопубликованные прайсы по сокращенной схеме
        protected override string GetXsdSchemaContent(string schemaName)
        {
            return Properties.Resources.ResourceManager.GetString(schemaName);
        }

        protected override XElement SerializeDtoToXElement(IExportableEntityDto entiryDto)
        {
            var priceDto = (PriceDto)entiryDto;

            var priceElement = new XElement("PriceList",
                                            new XAttribute("Code", priceDto.Id),
                                            new XAttribute("PublishedDate", priceDto.PublishDate),
                                            new XAttribute("BeginingDate", priceDto.BeginDate),
                                            new XAttribute("IsPublished", priceDto.IsPublished),
                                            new XAttribute("BranchCode", priceDto.ProjectCode),
                                            new XAttribute("Currency", priceDto.CurrencyName),
                                            new XAttribute("IsHidden", priceDto.IsHidden),
                                            new XAttribute("IsDeleted", priceDto.IsDeleted));

            return priceElement;
        }

        // TODO {all, 04.03.2014}: После выполнения задачи ERM-3530 необходимо выгружать неопубликованные прайсы по сокращенной схеме
        protected override ISelectSpecification<Price, IExportableEntityDto> CreateDtoExpression()
        {
            return new SelectSpecification<Price, IExportableEntityDto>(x => new PriceDto
                {
                    Id = x.Id,
                    IsPublished = x.IsPublished,
                    PublishDate = x.PublishDate,
                    BeginDate = x.BeginDate,
                                                                                    
                    // Олег подтвердил, что берем первый код, даже если у отделения несколько проектов. 
                    ProjectCode =
                        x.OrganizationUnit.Projects.Select(y => y.Id).FirstOrDefault(),
                    CurrencyName = x.Currency.Symbol,
                    IsHidden = !x.IsActive,
                    IsDeleted = x.IsDeleted
                });
        }


        #region nested types

        private sealed class PriceDto : IExportableEntityDto
        {
            public long Id { get; set; }
            public DateTime PublishDate { get; set; }
            public DateTime BeginDate { get; set; }
            public bool IsPublished { get; set; }
            public long ProjectCode { get; set; }
            public string CurrencyName { get; set; }
            public bool IsHidden { get; set; }
            public bool IsDeleted { get; set; }
        }

        #endregion
    }
}
