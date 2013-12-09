using System;
using System.Globalization;
using System.Linq;

using DoubleGis.Erm.BL.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BL.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.Operations.Global.DuplicatesFromOperations
{
    // FIXME {all, 06.11.2013}: вынесено из BL.Operations - уже копия в данном проекте, похоже на дублирование файлов в TFS из-за многочисленных merge - пока оставлены обе копии, при RI из 1.0 нужно обращать внимание какой целевой файл выбирается из 2ух
    // указан модификатор доступа internal, чтобы не подхватывался massprocessor
    internal sealed class CzechPrintOrderAdditionalAgreementHandler : RequestHandler<PrintOrderAdditionalAgreementRequest, Response>, ICzechAdapted
    {
        private readonly IFinder _finder;
        private readonly ISubRequestProcessor _requestProcessor;

        public CzechPrintOrderAdditionalAgreementHandler(ISubRequestProcessor requestProcessor, IFinder finder)
        {
            _finder = finder;
            _requestProcessor = requestProcessor;
        }

        protected override Response Handle(PrintOrderAdditionalAgreementRequest request)
        {
            var orderInfoValidation =
                _finder.Find(Specs.Find.ById<Order>(request.OrderId))
                    .Select(order => new { WorkflowStep = (OrderState)order.WorkflowStepId, order.IsTerminated, order.RejectionDate })
                    .Single();

            if (!orderInfoValidation.IsTerminated)
            {
                throw new NotificationException(BLResources.OrderShouldBeTerminated);
            }

            if (orderInfoValidation.WorkflowStep != OrderState.OnTermination && orderInfoValidation.WorkflowStep != OrderState.Archive)
            {
                throw new NotificationException(BLResources.OrderShouldBeTerminatedOrArchive);
            }

            if (orderInfoValidation.RejectionDate == null)
            {
                throw new NotificationException(BLResources.OrderRejectDateFieldIsNotFilled);
            }

            var orderInfo = _finder.Find(Specs.Find.ById<Order>(request.OrderId))
                .Select(order => new
                    {
                        Order = order,
                        order.Bargain,
                        order.LegalPerson,
                        Profile = order.LegalPerson.LegalPersonProfiles.FirstOrDefault(
                            y => request.LegalPersonProfileId.HasValue && y.Id == request.LegalPersonProfileId.Value),
                        OrganizationUnitName = order.LegalPerson.Client.Territory.OrganizationUnit.Name,
                        order.BranchOfficeOrganizationUnit,
                        order.BranchOfficeOrganizationUnit.BranchOffice,
                        CurrencyISOCode = order.Currency.ISOCode,
                        LegalPersonType = (LegalPersonType)order.LegalPerson.LegalPersonTypeEnum,
                        order.BranchOfficeOrganizationUnitId,
                    })
                .Single();

            var printData = new
                {
                    orderInfo.Order,
                    RelatedBargainInfo = (orderInfo.Bargain != null)
                                             ? string.Format(
                                                 BLResources.RelatedToBargainInfoTemplate,
                                                 orderInfo.Bargain.Number,
                                                 PrintFormFieldsFormatHelper.FormatLongDate(orderInfo.Bargain.CreatedOn))
                                             : null,
                    NextReleaseDate = orderInfo.Order.RejectionDate.Value.AddMonths(1).GetFirstDateOfMonth(),
                    orderInfo.LegalPerson,
                    orderInfo.Profile,
                    orderInfo.OrganizationUnitName,
                    orderInfo.BranchOfficeOrganizationUnit,
                    orderInfo.BranchOffice,
                    orderInfo.CurrencyISOCode,
                    orderInfo.LegalPersonType,
                    orderInfo.BranchOfficeOrganizationUnitId,
                    OperatesOnTheBasis = GetOperatesOnTheBasisString(orderInfo.Profile)
                };

            return
                _requestProcessor.HandleSubRequest(
                    new PrintDocumentRequest
                        {
                            CurrencyIsoCode = printData.CurrencyISOCode,
                            BranchOfficeOrganizationUnitId = printData.BranchOfficeOrganizationUnitId,
                            TemplateCode = GetTemplateCode(printData.LegalPersonType, request.PrintType),
                            FileName = string.Format(BLResources.PrintAdditionalAgreementFileNameFormat, printData.Order.Number),
                            PrintData = printData
                        },
                    Context);
        }

        private static string GetOperatesOnTheBasisString(LegalPersonProfile profile)
        {
            if (profile.OperatesOnTheBasisInGenitive != (int)OperatesOnTheBasisType.Warranty)
            {
                return string.Empty;
            }

            if (profile.WarrantyBeginDate == null)
            {
                return string.Empty;
            }

            return string.Format(
                CultureInfo.CurrentCulture,
                BLResources.CzechPrintOrderHandler_OperatesOnTheBasisStringTemplate,
                ((OperatesOnTheBasisType)profile.OperatesOnTheBasisInGenitive).ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture),
                profile.WarrantyBeginDate.Value.ToShortDateString());
        }

        private static TemplateCode GetTemplateCode(LegalPersonType legalPersonType, PrintAdditionalAgreementTarget printType)
        {
            if (printType == PrintAdditionalAgreementTarget.Bargain)
            {
                switch (legalPersonType)
                {
                    case LegalPersonType.LegalPerson:
                        return TemplateCode.BargainAdditionalAgreementLegalPerson;

                    case LegalPersonType.Businessman:
                        return TemplateCode.BargainAdditionalAgreementBusinessman;
                }   
            }

            switch (legalPersonType)
            {
                case LegalPersonType.LegalPerson:
                    return TemplateCode.AdditionalAgreementLegalPerson;

                case LegalPersonType.Businessman:
                    return TemplateCode.AdditionalAgreementBusinessman;
            }

            throw new ArgumentOutOfRangeException("legalPersonType");
        }
    }
}