using System;

using DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.Print;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders.Print;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLFlex.Operations.Global.Russia.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Concrete.Orders.Print
{
    public class PrintFirmChangeAgreementOperationService : IPrintFirmChangeAgreementOperationService, IRussiaAdapted
    {
        private readonly IPrintReadModel _readModel;
        private readonly IPublicService _publicService;
        private readonly IFormatter _shortDateFormatter;

        public PrintFirmChangeAgreementOperationService(IPublicService publicService, IFormatterFactory formatterFactory, IPrintReadModel readModel)
        {
            _publicService = publicService;
            _readModel = readModel;
            _shortDateFormatter = formatterFactory.Create(typeof(DateTime), FormatType.ShortDate, 0);
        }

        public PrintFormDocument Print(long orderId)
        {
            var order = _readModel.GetOrder(orderId);

            if (order == null)
            {
                throw new EntityNotFoundException(typeof(Order), orderId);
            }

            if (order.BranchOfficeOrganizationUnitId == null)
            {
                throw new FieldNotSpecifiedException(BLResources.OrderHasNoBranchOfficeOrganizationUnit);
            }

            if (order.LegalPersonProfileId == null)
            {
                throw new FieldNotSpecifiedException(BLResources.LegalPersonProfileMustBeSpecified);
            }

            var currency = _readModel.GetCurrency(order.CurrencyId);
            var bargain = _readModel.GetBargain(order.BargainId);
            var legalPerson = _readModel.GetLegalPerson(order.LegalPersonId);
            var legalPersonProfile = _readModel.GetLegalPersonProfile(order.LegalPersonProfileId);
            var branchOfficeOrganizationUnit = _readModel.GetBranchOfficeOrganizationUnit(order.BranchOfficeOrganizationUnitId);
            var branchOffice = _readModel.GetBranchOffice(branchOfficeOrganizationUnit.BranchOfficeId);
            var firm = _readModel.GetFirm(order.FirmId);

            var documentData = PrintHelper.AgreementSharedBody(order, legalPerson, legalPersonProfile, branchOfficeOrganizationUnit, _shortDateFormatter);
            var documenSpecificData = PrintHelper.ChangeAgreementSpecificBody(firm);
            var bargainData = PrintHelper.RelatedBrgain(bargain);
            var requisites = PrintHelper.Requisites(legalPerson, legalPersonProfile, branchOffice, branchOfficeOrganizationUnit);

            var printDocumentRequest = new PrintDocumentRequest
            {
                CurrencyIsoCode = currency.ISOCode,
                FileName = string.Format(BLResources.PrintAdditionalAgreementFileNameFormat, order.Number),
                BranchOfficeOrganizationUnitId = order.BranchOfficeOrganizationUnitId.Value,
                TemplateCode = TemplateCode.FirmChangeAgreement,
                PrintData = PrintData.Concat(documentData, requisites, bargainData, documenSpecificData)
            };

            var printDocumentResponse = (StreamResponse)_publicService.Handle(printDocumentRequest);

            return new PrintFormDocument
                       {
                           Stream = printDocumentResponse.Stream,
                           ContentType = printDocumentResponse.ContentType,
                           FileName = printDocumentResponse.FileName,
                       };
        }
    }
}