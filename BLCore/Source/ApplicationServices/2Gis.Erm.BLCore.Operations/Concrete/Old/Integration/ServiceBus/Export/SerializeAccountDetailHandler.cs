using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts;
using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export;
using DoubleGis.Erm.Platform.API.Aggregates.SimplifiedModel.PerformedOperations.ReadModel;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Withdrawal;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Integration.ServiceBus.Export
{
    public sealed class SerializeAccountDetailHandler : SerializeObjectsHandler<AccountDetail, ExportFlowFinancialDataDebitsInfoInitial>
    {
        private readonly IFinder _finder;
        private readonly IOperationContextParser _operationContextParser;

        public SerializeAccountDetailHandler(IExportRepository<AccountDetail> exportRepository, ICommonLog logger, IFinder finder, IOperationContextParser operationContextParser)
            : base(exportRepository, logger)
        {
            _finder = finder;
            _operationContextParser = operationContextParser;
        }

        protected override string GetXsdSchemaContent(string schemaName)
        {
            return Properties.Resources.ResourceManager.GetString(schemaName);
        }

        protected override XElement SerializeDtoToXElement(IExportableEntityDto entityDto)
        {
            var dto = (DebitContainerDto)entityDto;
            var debits = dto.Debits.Select(debitDto =>
                                           new XElement("Debit",
                                                        new XAttribute("OrderCode", debitDto.OrderCode),
                                                        new XAttribute("AccountCode", debitDto.AccountCode),
                                                        new XAttribute("LegalEntityBranchCode1C", debitDto.LegalEntityBranchCode1C),
                                                        new XAttribute("ProfileCode", debitDto.ProfileCode),
                                                        new XAttribute("OrderType", debitDto.OrderType),
                                                        new XAttribute("OrderNumber", debitDto.OrderNumber),
                                                        new XAttribute("Amount", debitDto.Amount),
                                                        new XAttribute("MediaInfo", debitDto.MediaInfo)));

            return new XElement("DebitsInfoInitial",
                                new XAttribute("OrganizationUnitCode", dto.OrganizationUnitCode),
                                new XAttribute("StartDate", dto.StartDate),
                                new XAttribute("EndDate", dto.EndDate),
                                new XAttribute("ClientDebitTotalAmount", dto.ClientDebitTotalAmount),
                                new XAttribute("AccountingMethod", dto.AccountingMethod),
                                new XElement("ClientDebits", debits));
        }

        protected override IEnumerable<IExportableEntityDto> ProcessOperations(IEnumerable<PerformedBusinessOperation> operations)
        {
            return operations.OrderBy(operation => operation.Date).Select(FormatExportRecord).ToArray();
        }

        protected override IEnumerable<IExportableEntityDto> ProcessFailedEntities(IEnumerable<ExportFailedEntity> entities)
        {
            throw new NotSupportedException("Обработка невыгруженных с первой попытки пакетов не поддерживается");
        }

        protected override ISelectSpecification<AccountDetail, IExportableEntityDto> CreateDtoExpression()
        {
            throw new NotSupportedException("Данные выбираются только пакетом по операции");
        }

        private IExportableEntityDto FormatExportRecord(PerformedBusinessOperation operation)
        {
            var filter = CreateAccountDetailsFilter(operation);
            var selector = AccountDetailDtoSelectSpecification();
            var exportData = _finder.Find(selector, filter).ToArray();

            var key = exportData.Select(dto => new DebitContainerKey
                                                   {
                                                       DestOrganizationUnitId = dto.DestOrganizationUnitId,
                                                       AccountingMethod = ModelToMethod(dto.SalesModel),
                                                       PeriodStartDate = dto.PeriodStartDate,
                                                       PeriodEndDate = dto.PeriodEndDate
                                                   })
                                .Distinct()
                                .Single();

            var debits = operation.Operation == RevertWithdrawFromAccountsIdentity.Instance.Id
                             ? new DebitDto[0]
                             : exportData.Select(dto => new DebitDto
                                                            {
                                                                AccountCode = dto.AccountId,
                                                                Amount = dto.Amount,
                                                                LegalEntityBranchCode1C = dto.LegalEntityBranchCode1C,
                                                                MediaInfo = dto.ElectronicMedia,
                                                                OrderCode = dto.OrderId,
                                                                OrderNumber = dto.OrderNumber,
                                                                OrderType = (int)dto.OrderType,
                                                                ProfileCode = dto.OrderLegalPersonProfileId ?? dto.MainLegalPersonProfileId,
                                                            })
                                         .Where(dto => dto.Amount > 0)
                                         .ToArray();

            return new DebitContainerDto
                       {
                           Id = operation.Id,
                           OrganizationUnitCode = key.DestOrganizationUnitId.ToString(),
                           AccountingMethod = key.AccountingMethod,
                           StartDate = key.PeriodStartDate,
                           EndDate = key.PeriodEndDate,
                           Debits = debits
                       };
        }

        private string ModelToMethod(SalesModel model)
        {
            var method = model.ToAccountingMethod();
            switch (method)
            {
                case AccountingMethod.GuaranteedProvision:
                    return "Fact";
                case AccountingMethod.PlannedProvision:
                    return "Planned";
                default:
                    throw new ArgumentException("model");
            }
        }

        private ISelectSpecification<Lock, AccountDetailDto> AccountDetailDtoSelectSpecification()
        {
            return new SelectSpecification<Lock, AccountDetailDto>(
                x => new AccountDetailDto
                         {
                             AccountId = x.AccountId,
                             Amount = x.AccountDetail.Amount,
                             PeriodStartDate = x.PeriodStartDate,
                             PeriodEndDate = x.PeriodEndDate,

                             OrderId = x.OrderId,
                             OrderLegalPersonProfileId = x.Order.LegalPersonProfileId,
                             OrderType = x.Order.OrderType,
                             OrderNumber = x.Order.Number,
                             ElectronicMedia = x.Order.DestOrganizationUnit.ElectronicMedia,
                             DestOrganizationUnitId = x.Order.DestOrganizationUnit.Id,
                             SalesModel = x.Order.OrderPositions.Select(position => position.PricePosition.Position.SalesModel).Distinct().FirstOrDefault(),
                             LegalEntityBranchCode1C = x.Order.BranchOfficeOrganizationUnit.SyncCode1C,
                             MainLegalPersonProfileId = x.Account.LegalPerson.LegalPersonProfiles
                                                         .Where(p => !p.IsDeleted && p.IsMainProfile)
                                                         .Select(p => p.Id)
                                                         .FirstOrDefault(),
                         });
        }

        private IFindSpecification<Lock> CreateAccountDetailsFilter(PerformedBusinessOperation operation)
        {
            if (operation.Operation == WithdrawFromAccountsIdentity.Instance.Id)
            {
                string report;
                EntityChangesContext changesContext;
                if (!_operationContextParser.TryParse(operation.Context, out changesContext, out report))
                {
                    throw new InvalidOperationException("Can't parse operation context. Detail: " + report);
                }

                var accountDetailIds = GetAccountDetails<AccountDetail>(changesContext.AddedChanges);
                return Specs.Find.NotDeleted<Lock>() && AccountSpecs.Locks.Find.ByAccountDetails(accountDetailIds);
            }

            if (operation.Operation == RevertWithdrawFromAccountsIdentity.Instance.Id)
            {
                // Операция отката списания разрывает связь между AccountDetail и Lock (AccountDetail становится IsDeleted = true)
                // Cтановится невозможно определить период, город истоник и назначения, тип заказа.
                // Поэтому в том-же UseCase идем операцию активации блокировок и из неё извлекаем список блокировок
                var activateLocksOperation = _finder.Find(OperationSpecs.Performed.Find.InUseCase(operation.UseCaseId)
                                                          && OperationSpecs.Performed.Find.Specific<BulkActivateIdentity, Lock>())
                                                    .Single();

                string report;
                EntityChangesContext changesContext;
                if (!_operationContextParser.TryParse(activateLocksOperation.Context, out changesContext, out report))
                {
                    throw new InvalidOperationException("Can't parse operation context. Detail: " + report);
                }

                var lockIds = GetAccountDetails<Lock>(changesContext.UpdatedChanges);

                return Specs.Find.NotDeleted<Lock>() && Specs.Find.ByIds<Lock>(lockIds);
            }

            throw new UnsupportedExportOperationException(operation);
        }

        private IEnumerable<long> GetAccountDetails<TEntity>(IReadOnlyDictionary<Type, ConcurrentDictionary<long, int>> changes)
        {
            ConcurrentDictionary<long, int> ids;
            return changes.TryGetValue(typeof(TEntity), out ids) 
                ? ids.Keys 
                : new long[0];
        }

        private sealed class AccountDetailDto
        {
            public long AccountId { get; set; }
            public decimal Amount { get; set; }
            public DateTime PeriodStartDate { get; set; }
            public DateTime PeriodEndDate { get; set; }
            public long OrderId { get; set; }
            public long? OrderLegalPersonProfileId { get; set; }
            public OrderType OrderType { get; set; }
            public string OrderNumber { get; set; }
            public string ElectronicMedia { get; set; }
            public long DestOrganizationUnitId { get; set; }
            public SalesModel SalesModel { get; set; }
            public string LegalEntityBranchCode1C { get; set; }
            public long MainLegalPersonProfileId { get; set; }
        }

        private sealed class DebitContainerDto : IExportableEntityDto
        {
            public long Id { get; set; }
            
            public string OrganizationUnitCode { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public IEnumerable<DebitDto> Debits { get; set; }
            public string AccountingMethod { get; set; }

            public decimal ClientDebitTotalAmount
            {
                get { return (Debits ?? new DebitDto[0]).Sum(debit => debit.Amount); }
            }
        }

        private sealed class DebitDto
        {
            public long OrderCode { get; set; }
            public long AccountCode { get; set; }
            public long ProfileCode { get; set; }
            public int OrderType { get; set; }
            public string OrderNumber { get; set; }
            public decimal Amount { get; set; }
            public string MediaInfo { get; set; }
            public string LegalEntityBranchCode1C { get; set; }
        }

        private sealed class DebitContainerKey : IEquatable<DebitContainerKey>
        {
            public long DestOrganizationUnitId { get; set; }
            public string AccountingMethod { get; set; }
            public DateTime PeriodStartDate { get; set; }
            public DateTime PeriodEndDate { get; set; }

            public bool Equals(DebitContainerKey other)
            {
                if (other == null)
                {
                    return false;
                }

                return DestOrganizationUnitId == other.DestOrganizationUnitId
                       && string.Equals(AccountingMethod, other.AccountingMethod)
                       && PeriodStartDate == other.PeriodStartDate
                       && PeriodEndDate == other.PeriodEndDate;
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as DebitContainerKey);
            }

            public override int GetHashCode()
            {
                return DestOrganizationUnitId.GetHashCode()
                    ^ AccountingMethod.GetHashCode()
                    ^ PeriodStartDate.GetHashCode()
                    ^ PeriodEndDate.GetHashCode();
            }
        }
    }
}