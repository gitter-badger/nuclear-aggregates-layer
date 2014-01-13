using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DoubleGis.Erm.BLCore.Aggregates.Firms;
using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Orders.PrintForms
{
    public sealed class PrintReferenceInformationHandler : RequestHandler<PrintReferenceInformationRequest, StreamResponse>
    {
        private const int ShowFlampLinkServiceId = 1;

        private static readonly Dictionary<int, string> FirmAddressContactTypePlural = new Dictionary<int, string>
        {
            {(int) FirmAddressContactType.Phone, BLResources.Phones},
            {(int) FirmAddressContactType.Fax, BLResources.Faxes},
            {(int) FirmAddressContactType.Email, BLResources.Emails},
            {(int) FirmAddressContactType.Website, BLResources.WebSites},
            {(int) FirmAddressContactType.Icq, BLResources.Icqs},
            {(int) FirmAddressContactType.Skype, BLResources.Skypes},
            {(int) FirmAddressContactType.Other, BLResources.Others}, // other means "jabber"
        };

        private readonly IFinder _finder;
        private readonly ISubRequestProcessor _requestProcessor;
        private readonly IUserContext _userContext;
        private readonly IFirmRepository _firmAggregateRepository;
        private readonly ISecurityServiceUserIdentifier _securityServiceUserIdentifier;

        public PrintReferenceInformationHandler(ISubRequestProcessor requestProcessor,
            IFinder finder,
            IUserContext userContext,
            IFirmRepository firmAggregateRepository,
            ISecurityServiceUserIdentifier securityServiceUserIdentifier)
        {
            _requestProcessor = requestProcessor;
            _userContext = userContext;
            _finder = finder;
            _firmAggregateRepository = firmAggregateRepository;
            _securityServiceUserIdentifier = securityServiceUserIdentifier;
        }

        protected override StreamResponse Handle(PrintReferenceInformationRequest request)
        {
            var orderInfo = _finder.Find<Order>(x => x.Id == request.OrderId).Select(x => new
            {
                CurrencyIsoCode = x.Currency.ISOCode,
                x.BranchOfficeOrganizationUnitId,

                Platform = x.Platform != null ? (PlatformEnum?)x.Platform.DgppId : null,

                Addresses = x.Firm.FirmAddresses.Where(y => y.IsActive && !y.IsDeleted && !y.ClosedForAscertainment)
                    .OrderBy(y => y.SortingPosition)
                    .Select(y => new FirmAddressDto
                    {
                        Id = y.Id,
                        Address = y.Address,
                        ReferencePoint = y.ReferencePoint,
                        WorkingTime = y.WorkingTime,
                        PaymentMethods = y.PaymentMethods,
                        ShowFlampLink = y.FirmAddressServices.Where(z => z.ServiceId == ShowFlampLinkServiceId).Select(z => (bool?)z.DisplayService).FirstOrDefault(),
                    }),

                Categories = x.Firm.FirmAddresses.Where(y => y.IsActive && !y.IsDeleted && !y.ClosedForAscertainment)
                    .OrderBy(y => y.SortingPosition)
                    .SelectMany(y => y.CategoryFirmAddresses)
                    .Where(y => y.IsActive && !y.IsDeleted)
                    .OrderBy(y => y.SortingPosition)
                    .Select(y => y.Category.Name)
                    .Distinct(),

                OrderSignupDate = x.SignupDate,
                x.DestOrganizationUnit.ElectronicMedia,
                FirmName = x.Firm.Name,
                LegalPersonChiefNameInNominative = x.LegalPerson.LegalPersonProfiles.FirstOrDefault(y => y.Id == request.LegalPersonProfileId).ChiefNameInNominative,
                OrderNumber = x.Number,
                x.OwnerCode,
                SourceElectronicMedia = x.SourceOrganizationUnit.ElectronicMedia,
            })
            .SingleOrDefault();

            if (orderInfo == null || orderInfo.BranchOfficeOrganizationUnitId == null)
            {
                throw new NotificationException("Печать документа невозможна.");
            }

            if (orderInfo.Platform == null)
            {
                throw new NotificationException(BLResources.OrderPlatformIsNotDefined);
            }

            var printData = new PrintData
            {
                FirmAddresses = Enumerable.Range(1, int.MaxValue).Zip(orderInfo.Addresses, CreateFirmAddressPrintData).ToArray(),

                FirmCategories = FormatCategories(orderInfo.Categories),

                OrderOwnerName = _securityServiceUserIdentifier.GetUserInfo(orderInfo.OwnerCode).DisplayName,

                FinalParagraph = orderInfo.Platform == PlatformEnum.Api ?
                            BLResources.PrintReferenceInformationHandler_FinalParagraphApi :
                            BLResources.PrintReferenceInformationHandler_FinalParagraphGeneral,

                OrderSignupDate = orderInfo.OrderSignupDate,
                ElectronicMedia = orderInfo.ElectronicMedia,
                FirmName = orderInfo.FirmName,
                LegalPersonChiefNameInNominative = orderInfo.LegalPersonChiefNameInNominative,
                OrderNumber = orderInfo.OrderNumber,
                SourceElectronicMedia = orderInfo.SourceElectronicMedia,
            };

            var currentUserTime = TimeZoneInfo.ConvertTime(DateTime.Now, _userContext.Profile.UserLocaleInfo.UserTimeZoneInfo);
            var response = (StreamResponse)_requestProcessor.HandleSubRequest(new PrintDocumentRequest
            {
                PrintData = printData,
                TemplateCode = TemplateCode.ReferenceInformation,
                BranchOfficeOrganizationUnitId = orderInfo.BranchOfficeOrganizationUnitId,
                FileName = string.Format(_userContext.Profile.UserLocaleInfo.UserCultureInfo, "Справочная информация_{0}_{1:d}", orderInfo.OrderNumber, currentUserTime),
                CurrencyIsoCode = orderInfo.CurrencyIsoCode,
            }, Context);

            return response;
        }

        private static string FormatCategories(IEnumerable<string> categories)
        {
            return string.Join("; ", Enumerable.Range(1, Int32.MaxValue).Zip(categories, (index, category) => string.Format("{0}. {1}", index, category)));
        }

        private FirmAddressPrintData CreateFirmAddressPrintData(int index, FirmAddressDto firmAddressDto)
        {
            // Address
            var addressBuilder = new StringBuilder();

            // address number
            addressBuilder.Append(BLResources.AddressNumber).Append(index).AppendLine(":");
            var address = firmAddressDto.Address + ((firmAddressDto.ReferencePoint == null) ? string.Empty : " — " + firmAddressDto.ReferencePoint);
            addressBuilder.AppendLine(address);

            // Contacts
            foreach (var contact in _firmAggregateRepository.GetContacts(firmAddressDto.Id).ToLookup(x => x.ContactType, x => x.Contact))
            {
                var template = FirmAddressContactTypePlural[contact.Key];
                addressBuilder.Append(template).Append(": ").AppendLine(string.Join("; ", contact));
            }

            // WorkingTime
            if (!string.IsNullOrEmpty(firmAddressDto.WorkingTime))
            {
                var localizedWorkingTime = FirmWorkingTimeLocalizer.LocalizeWorkingTime(firmAddressDto.WorkingTime, _userContext.Profile.UserLocaleInfo.UserCultureInfo);
                addressBuilder.Append(MetadataResources.WorkingTime).Append(": ").AppendLine(localizedWorkingTime);                
            }

            // PaymentMethods
            if (!string.IsNullOrEmpty(firmAddressDto.PaymentMethods))
            {
                addressBuilder.Append(MetadataResources.PaymentMethods).Append(": ").AppendLine(firmAddressDto.PaymentMethods);                
            }

            string flampLink;
            switch (firmAddressDto.ShowFlampLink)
            {
                case null:
                    flampLink = null;
                    break;

                case true:
                    flampLink = BLResources.PrintOrderHandler_ShowFlampLinkModeFlampPublished;
                    break;

                case false:
                    flampLink = BLResources.PrintOrderHandler_ShowFlampLinkModeFlampNotPublished;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
        }

            if (!string.IsNullOrEmpty(flampLink))
        {
                addressBuilder.Append(MetadataResources.Reviews).Append(": ").AppendLine(flampLink);
            }

            return new FirmAddressPrintData
            {
                FirmAddressInfo = addressBuilder.ToString(),
            };
        }

        // ReSharper disable UnusedAutoPropertyAccessor.Local
        private sealed class PrintData
        {
            public FirmAddressPrintData[] FirmAddresses { get; set; }
            public string FirmCategories { get; set; }
            public string OrderOwnerName { get; set; }
            public string FinalParagraph { get; set; }
            public DateTime OrderSignupDate { get; set; }
            public string ElectronicMedia { get; set; }
            public string FirmName { get; set; }
            public string LegalPersonChiefNameInNominative { get; set; }
            public string OrderNumber { get; set; }
            public string SourceElectronicMedia { get; set; }
        }

        private sealed class FirmAddressPrintData
            {
            public string FirmAddressInfo { get; set; }
        }
        // ReSharper restore UnusedAutoPropertyAccessor.Local

        private sealed class FirmAddressDto
        {
            public long Id { get; set; }
            public string Address { get; set; }
            public string ReferencePoint { get; set; }
            public string WorkingTime { get; set; }
            public string PaymentMethods { get; set; }
            public bool? ShowFlampLink { get; set; }
        }
    }
}
