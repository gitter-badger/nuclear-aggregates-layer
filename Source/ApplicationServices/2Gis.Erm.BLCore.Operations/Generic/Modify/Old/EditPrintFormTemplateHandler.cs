using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Old
{
    public sealed class EditPrintFormTemplateHandler : RequestHandler<EditRequest<PrintFormTemplate>, EmptyResponse>
    {
        private readonly IPrintFormTemplateService _printFormTemplateService;

        public EditPrintFormTemplateHandler(IPrintFormTemplateService printFormTemplateService)
        {
            _printFormTemplateService = printFormTemplateService;
        }

        protected override EmptyResponse Handle(EditRequest<PrintFormTemplate> request)
        {
            var printFormTemplate = request.Entity;

            if (_printFormTemplateService.TemplateExists(printFormTemplate))
            {
                throw new NotificationException(BLResources.PrintFormTemplateWithSameTypeExists);
            }

            _printFormTemplateService.CreateOrUpdatePrintFormTemplate(printFormTemplate);

            return Response.Empty;
        }
    }
}