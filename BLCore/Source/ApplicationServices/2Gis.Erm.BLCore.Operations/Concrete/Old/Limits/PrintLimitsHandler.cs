using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Limits;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified;
using DoubleGis.Erm.BLCore.API.Operations.Generic.File;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.Common.Compression;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Limits
{
    public sealed class PrintLimitsHandler : RequestHandler<PrintLimitsRequest, StreamResponse>
    {
        private readonly IPrintFormService _printFormService;
        private readonly ISecurityServiceUserIdentifier _securityServiceUserIdentifier;
        private readonly IFinder _finder;
        private readonly IPrintFormTemplateService _printFormTemplateService;
        private readonly IFileService _fileService;

        public PrintLimitsHandler(IFileService fileService,
            IFinder finder,
            IPrintFormService printFormService,
            ISecurityServiceUserIdentifier securityServiceUserIdentifier, 
            IPrintFormTemplateService printFormTemplateService)
        {
            _fileService = fileService;
            _finder = finder;
            _printFormService = printFormService;
            _securityServiceUserIdentifier = securityServiceUserIdentifier;
            _printFormTemplateService = printFormTemplateService;
        }

        protected override StreamResponse Handle(PrintLimitsRequest request)
        {
            var branchOfficeOrgUnitInfos = request.LimitIds.Select(x => _finder.Find(Specs.Find.ById<Limit>(x))
                                                                               .Select(limit => new
                                                                                   {
                                                                                       limit.Account.BranchOfficeOrganizationUnit.ShortLegalName,
                                                                                       limit.Account.BranchOfficeOrganizationUnit.Id
                                                                                   })
                                                                               .Single())
                                                  .Distinct()
                                                  .ToArray();

            var fileTemplateInfos = branchOfficeOrgUnitInfos.Select(x => new
                {
                    BranchOfficeOrganizationUnitId = x.Id,
                    PrintFormTemplateFileId = _printFormTemplateService.GetPrintFormTemplateFileId(x.Id, TemplateCode.LimitRequest),
                })
                                                            .Where(x => x.PrintFormTemplateFileId != null)
                                                            .ToArray();

            if (branchOfficeOrgUnitInfos.Count() != fileTemplateInfos.Count())
            {
                var invalidOrgUnitNames = branchOfficeOrgUnitInfos.Select(x => x.Id)
                                                                  .Except(fileTemplateInfos.Select(x => x.BranchOfficeOrganizationUnitId))
                                                                  .Select(
                                                                      x => branchOfficeOrgUnitInfos.Where(y => y.Id == x).Select(y => y.ShortLegalName).Single());

                throw new NotificationException(
                    string.Format("Ќе найдены шаблоны печатных форм лимитов дл€ юр. лиц отделений организации: {0}", String.Join(", ", invalidOrgUnitNames)));
            }

            var printDocs = new Dictionary<string, Stream>();

            foreach (var fileTemplateInfo in fileTemplateInfos)
            {
                var printData = GetPrintData(request.LimitIds, fileTemplateInfo.BranchOfficeOrganizationUnitId);

                var templateFile = _fileService.GetFileById(fileTemplateInfo.PrintFormTemplateFileId.Value);

                var ms = new MemoryStream((int)templateFile.ContentLength);
                templateFile.Content.CopyTo(ms);
                _printFormService.PrintToDocx(ms, printData.Data, printData.CurrencyIsoCode);

                printDocs[printData.FileName] = ms;
            }

            var zipStream = printDocs.ZipStreamDictionary();

            foreach (var stream in printDocs.Values)
                stream.Dispose();
            printDocs.Clear();

            return new StreamResponse
            {
                Stream = zipStream,
                ContentType = MediaTypeNames.Application.Zip,
                FileName = "limits.zip"
            };
        }

        private PrintData GetPrintData(IEnumerable<long> limitIds, long branchOfficeOrgUnitId)
        {
            var limits = limitIds.Select((id, index) =>
                                        _finder.Find(new FindSpecification<Limit>(limit => limit.Id == id && limit.Account.BranchOfficeOrganizationUnitId == branchOfficeOrgUnitId))
                                             .Select(limit => new
                                                          {
                                                              ClientName = limit.Account.LegalPerson.Client.Name,
                                                              LegalPersonLegalName = limit.Account.LegalPerson.LegalName,
                                                              BranchOfficeOrganizationUnitShortLegalName = limit.Account.BranchOfficeOrganizationUnit.ShortLegalName,
                                                              limit.Amount,
                                                              OrdersSumForDistribution = limit.Account.BranchOfficeOrganizationUnit.Orders
                                                                                             .Where(y => y.LegalPersonId == limit.Account.LegalPersonId && y.IsActive && !y.IsDeleted && y.WorkflowStepId == OrderState.Approved)
                                                                                             .SelectMany(y => y.OrderReleaseTotals)
                                                                                             .Where(t => t.ReleaseBeginDate == limit.StartPeriodDate && t.ReleaseEndDate == limit.EndPeriodDate)
                                                                                             .Sum(a => a.AmountToWithdraw),
                                                              limit.StartPeriodDate,
                                                              limit.EndPeriodDate,
                                                              // CR: {any}:{399}:{Minor}:{25.08.2011}: есть подозрение, что сумму по активным лимитам нужно сюда же добавить
                                                              AccountBalance = limit.Account.Balance - (limit.Account.Locks.Where(y => y.IsActive && !y.IsDeleted).Sum(y => y.PlannedAmount)),
                                                              limit.OwnerCode,
                                                              limit.InspectorCode,
                                                              limit.Comment,
                                                              CurrencyISOCode = limit.Account.BranchOfficeOrganizationUnit.OrganizationUnit.Country.Currency.ISOCode
                                                          })
                                             .AsEnumerable()
                                             .Select(item => new
                                                          {
                                                              Number = index + 1,
                                                              item.ClientName,
                                                              item.LegalPersonLegalName,
                                                              item.BranchOfficeOrganizationUnitShortLegalName,
                                                              item.Amount,
                                                              item.AccountBalance,
                                                              item.OrdersSumForDistribution,
                                                              OwnerName = _securityServiceUserIdentifier.GetUserInfo(item.OwnerCode).DisplayName,
                                                              InspectorName = _securityServiceUserIdentifier.GetUserInfo(item.InspectorCode).DisplayName,
                                                              item.Comment,
                                                              item.CurrencyISOCode
                                                          })
                                             .SingleOrDefault())
                .Where(x => x != null)
                .ToArray();
            
            var firstLimitInfo = limits.FirstOrDefault();
            
            return new PrintData
            {
                Data = new { Limits = limits },
                CurrencyIsoCode = firstLimitInfo != null ? firstLimitInfo.CurrencyISOCode : 0,
                FileName = string.Format("{0}.docx", firstLimitInfo != null  
                    ? firstLimitInfo.BranchOfficeOrganizationUnitShortLegalName.Replace("\"", string.Empty)
                    : "Not defined") 
            };
        }

        private struct PrintData
        {
            public int CurrencyIsoCode { get; set; }
            public object Data { get; set; }
            public string FileName { get; set; }
        }
    }
}