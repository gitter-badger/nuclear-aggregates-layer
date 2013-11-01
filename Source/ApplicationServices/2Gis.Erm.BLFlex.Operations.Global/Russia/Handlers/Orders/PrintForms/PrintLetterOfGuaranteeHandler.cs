using System;
using System.Linq;

using DoubleGis.Erm.BL.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BL.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Handlers.Orders.PrintForms
{
    public sealed class PrintLetterOfGuaranteeHandler : RequestHandler<PrintLetterOfGuaranteeRequest, Response>, IRussiaAdapted
    {
        private readonly IFinder _finder;
        private readonly ISubRequestProcessor _requestProcessor;

        public PrintLetterOfGuaranteeHandler(ISubRequestProcessor requestProcessor, IFinder finder)
        {
            _requestProcessor = requestProcessor;
            _finder = finder;
        }

        protected override Response Handle(PrintLetterOfGuaranteeRequest request)
        {
            var printData =
                _finder.Find(GenericSpecifications.ById<Order>(request.OrderId))
                       .Select(
                           order =>
                           new
                               {
                                   Order = order, 
                                   Profile =
                                       order.LegalPerson.LegalPersonProfiles.FirstOrDefault(
                                           y => request.LegalPersonProfileId.HasValue && y.Id == request.LegalPersonProfileId), 
                                   order.LegalPerson, 
                                   order.BranchOfficeOrganizationUnit, 
                                   order.BranchOfficeOrganizationUnitId, 
                               })
                       .AsEnumerable()
                       .Select(
                           x =>
                           new
                               {
                                   x.Order, 
                                   SignupDate = PrintFormFieldsFormatHelper.FormatLongDate(x.Order.SignupDate),
                                   ChangeDate = PrintFormFieldsFormatHelper.FormatLongDate(DateTime.Now.GetNextMonthFirstDate()),
                                   x.Profile, 
                                   x.LegalPerson, 
                                   x.BranchOfficeOrganizationUnit, 
                                   x.BranchOfficeOrganizationUnitId
                               })
                       .Single();

            return
                _requestProcessor.HandleSubRequest(
                    new PrintDocumentRequest
                        {
                            TemplateCode = request.IsChangingAdvMaterial ? TemplateCode.LetterOfGuaranteeAdvMaterial : TemplateCode.LetterOfGuarantee,
                            FileName = string.Format("{0}-Гарантийное письмо", printData.Order.Number),
                            BranchOfficeOrganizationUnitId = printData.BranchOfficeOrganizationUnitId, 
                            PrintData = printData
                        }, 
                    Context);
        }
    }
}
