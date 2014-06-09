using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Concrete.Old.Orders.PrintForms
{
    public sealed class PrintLetterOfGuaranteeHandler : RequestHandler<PrintLetterOfGuaranteeRequest, Response>, IRussiaAdapted
    {
        private readonly IFinder _finder;
        private readonly IFormatter _longDateFormatter;
        private readonly ISubRequestProcessor _requestProcessor;

        public PrintLetterOfGuaranteeHandler(ISubRequestProcessor requestProcessor, IFormatterFactory formatterFactory, IFinder finder)
        {
            _requestProcessor = requestProcessor;
            _finder = finder;
            _longDateFormatter = formatterFactory.Create(typeof(DateTime), FormatType.LongDate, 0);
        }

        protected override Response Handle(PrintLetterOfGuaranteeRequest request)
        {
            var printData =
                _finder.Find(Specs.Find.ById<Order>(request.OrderId))
                       .Select(
                           order =>
                           new
                               {
                                   Order = order,
                                   ProfileId = order.LegalPerson.LegalPersonProfiles
                                                    .FirstOrDefault(y => request.LegalPersonProfileId.HasValue && y.Id == request.LegalPersonProfileId)
                                                    .Id,
                                   order.LegalPersonId, 
                                   order.BranchOfficeOrganizationUnitId, 
                               })
                       .ToArray()
                       .Select(
                           x =>
                           new
                               {
                                   x.Order,
                                   SignupDate = _longDateFormatter.Format(x.Order.SignupDate),
                                   ChangeDate = _longDateFormatter.Format(DateTime.Now.GetNextMonthFirstDate()),
                                   Profile = _finder.FindOne(Specs.Find.ById<LegalPersonProfile>(x.ProfileId)), 
                                   LegalPerson = _finder.FindOne(Specs.Find.ById<LegalPerson>(x.LegalPersonId.Value)), 
                                   BranchOfficeOrganizationUnit = x.BranchOfficeOrganizationUnitId.HasValue
                                        ? _finder.FindOne(Specs.Find.ById<BranchOfficeOrganizationUnit>(x.BranchOfficeOrganizationUnitId.Value))
                                        : null,
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
