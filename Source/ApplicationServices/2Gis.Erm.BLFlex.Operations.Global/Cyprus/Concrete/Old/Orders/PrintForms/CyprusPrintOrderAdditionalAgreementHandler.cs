﻿using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Cyprus.Concrete.Old.Orders.PrintForms
{
    public sealed class CyprusPrintOrderAdditionalAgreementHandler : RequestHandler<PrintOrderAdditionalAgreementRequest, Response>, ICyprusAdapted
    {
        private readonly IFinder _finder;
        private readonly IFormatter _longDateFormatter;
        private readonly IFormatter _shortDateFormatter;
        private readonly ISubRequestProcessor _requestProcessor;

        public CyprusPrintOrderAdditionalAgreementHandler(ISubRequestProcessor requestProcessor, IFormatterFactory formatterFactory, IFinder finder)
        {
            _finder = finder;
            _requestProcessor = requestProcessor;
            _longDateFormatter = formatterFactory.Create(typeof(DateTime), FormatType.LongDate, 0);
            _shortDateFormatter = formatterFactory.Create(typeof(DateTime), FormatType.ShortDate, 0);
        }

        protected override Response Handle(PrintOrderAdditionalAgreementRequest request)
        {
            var orderInfo =
                _finder.Find(Specs.Find.ById<Order>(request.OrderId))
                    .Select(order => new { WorkflowStep = (OrderState)order.WorkflowStepId, order.IsTerminated, order.RejectionDate })
                    .Single();

            if (!orderInfo.IsTerminated)
            {
                throw new NotificationException(BLResources.OrderShouldBeTerminated);
            }

            if (orderInfo.WorkflowStep != OrderState.OnTermination && orderInfo.WorkflowStep != OrderState.Archive)
            {
                throw new NotificationException(BLResources.OrderShouldBeTerminatedOrArchive);
            }

            if (orderInfo.RejectionDate == null)
            {
                throw new NotificationException(BLResources.OrderRejectDateFieldIsNotFilled);
            }

            var printData = _finder.Find(Specs.Find.ById<Order>(request.OrderId))
                .Select(order => new
                    {
                        Order = order,
                        order.Bargain,
                        order.LegalPerson,
                        Profile =
                                     order.LegalPerson.LegalPersonProfiles.FirstOrDefault(
                                         y => request.LegalPersonProfileId.HasValue && y.Id == request.LegalPersonProfileId.Value),
                        OrganizationUnitName = order.LegalPerson.Client.Territory.OrganizationUnit.Name,
                        order.BranchOfficeOrganizationUnit,
                        order.BranchOfficeOrganizationUnit.BranchOffice,
                        CurrencyISOCode = order.Currency.ISOCode,
                        LegalPersonType = (LegalPersonType)order.LegalPerson.LegalPersonTypeEnum,
                        order.BranchOfficeOrganizationUnitId,
                    })
                .AsEnumerable()

                // in-memory transformations
                .Select(x => new
                    {
                        x.Order,
                        RelatedBargainInfo = (x.Bargain != null) ?
                                                                     string.Format(BLResources.RelatedToBargainInfoTemplate, x.Bargain.Number, _longDateFormatter.Format(x.Bargain.CreatedOn)) : null,
                        NextReleaseDate = x.Order.RejectionDate.Value.AddMonths(1).GetFirstDateOfMonth(),
                        x.LegalPerson,
                        x.Profile,
                        x.OrganizationUnitName,
                        x.BranchOfficeOrganizationUnit,
                        x.BranchOffice,
                        x.CurrencyISOCode,
                        x.LegalPersonType,
                        x.BranchOfficeOrganizationUnitId,
                        OperatesOnTheBasisInGenitive = GetOperatesOnTheBasisInGenitive(x.Profile, x.LegalPersonType)
                    })
                .Single();

            return
                _requestProcessor.HandleSubRequest(
                    new PrintDocumentRequest
                        {
                            CurrencyIsoCode = printData.CurrencyISOCode,
                            BranchOfficeOrganizationUnitId = printData.BranchOfficeOrganizationUnitId,
                            TemplateCode = GetTemplateCode(printData.LegalPersonType),
                            FileName = string.Format(BLResources.PrintAdditionalAgreementFileNameFormat, printData.Order.Number),
                            PrintData = printData
                        },
                    Context);
        }

        private static TemplateCode GetTemplateCode(LegalPersonType legalPersonType)
        {
            switch (legalPersonType)
            {
                case LegalPersonType.LegalPerson:
                    return TemplateCode.AdditionalAgreementLegalPerson;

                case LegalPersonType.Businessman:
                    return TemplateCode.AdditionalAgreementBusinessman;

                case LegalPersonType.NaturalPerson:
                    return TemplateCode.AdditionalAgreementNaturalPerson;

                default:
                    throw new ArgumentOutOfRangeException("legalPersonType");
            }
        }

        private string GetOperatesOnTheBasisInGenitive(LegalPersonProfile profile, LegalPersonType legalPersonType)
        {
            var operatesOnTheBasisInGenitive = string.Empty;
            if (profile.OperatesOnTheBasisInGenitive != null)
            {
                switch ((OperatesOnTheBasisType)profile.OperatesOnTheBasisInGenitive)
                {
                    case OperatesOnTheBasisType.Undefined:
                        operatesOnTheBasisInGenitive = string.Empty;
                        break;
                    case OperatesOnTheBasisType.Charter:
                        operatesOnTheBasisInGenitive = string.Format(
                            BLResources.OperatesOnBasisOfCharterTemplate,
                            ((OperatesOnTheBasisType)profile.OperatesOnTheBasisInGenitive).ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture));
                        break;
                    case OperatesOnTheBasisType.Certificate:
                        operatesOnTheBasisInGenitive = string.Format(
                            BLResources.OperatesOnBasisOfCertificateTemplate,
                            ((OperatesOnTheBasisType)profile.OperatesOnTheBasisInGenitive).ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture),
                            profile.CertificateNumber,
                            _shortDateFormatter.Format(profile.CertificateDate));
                        break;
                    case OperatesOnTheBasisType.Warranty:
                        operatesOnTheBasisInGenitive =
                            string.Format(
                                legalPersonType == LegalPersonType.NaturalPerson
                                    ? BLResources.OperatesOnBasisOfWarantyTemplateForNaturalPerson
                                    : BLResources.OperatesOnBasisOfWarantyTemplate,
                                ((OperatesOnTheBasisType)profile.OperatesOnTheBasisInGenitive).ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture),
                                profile.WarrantyNumber,
                                _shortDateFormatter.Format(profile.WarrantyBeginDate));
                        break;
                    case OperatesOnTheBasisType.Bargain:
                        operatesOnTheBasisInGenitive = string.Format(
                            BLResources.OperatesOnBasisOfBargainTemplate,
                            ((OperatesOnTheBasisType)profile.OperatesOnTheBasisInGenitive).ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture),
                            profile.BargainNumber,
                            _shortDateFormatter.Format(profile.BargainBeginDate));
                        break;
                    case OperatesOnTheBasisType.FoundingBargain:
                        operatesOnTheBasisInGenitive = string.Format(
                            BLResources.OperatesOnBasisOfFoundingBargainTemplate,
                            ((OperatesOnTheBasisType)profile.OperatesOnTheBasisInGenitive).ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return operatesOnTheBasisInGenitive;
        }
    }
}