using System.Xml.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Integration.ServiceBus.Export
{
    public sealed class SerializeAccountDetailHandler : SerializeObjectsHandler<AccountDetail, ExportFlowFinancialDataDebitsInfoInitial>
    {
        public SerializeAccountDetailHandler(IExportRepository<AccountDetail> exportRepository, ICommonLog logger) 
            : base(exportRepository, logger)
        {
        }

        protected override string GetXsdSchemaContent(string schemaName)
        {
            throw new System.NotImplementedException();
        }

        protected override XElement SerializeDtoToXElement(IExportableEntityDto entityDto)
        {
            throw new System.NotImplementedException();
        }

        protected override ISelectSpecification<AccountDetail, IExportableEntityDto> CreateDtoExpression()
        {
            throw new System.NotImplementedException();
        }
    }
}