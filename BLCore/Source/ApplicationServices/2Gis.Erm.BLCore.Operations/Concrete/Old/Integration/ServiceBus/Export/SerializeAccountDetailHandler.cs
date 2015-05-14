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
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Withdrawal;

using NuClear.Model.Common.Operations.Identity.Generic;
using NuClear.Storage;
using NuClear.Storage.Specifications;
using NuClear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Integration.ServiceBus.Export
{
    public sealed class SerializeAccountDetailHandler : SerializeObjectsHandler<AccountDetail, ExportFlowFinancialDataDebitsInfoInitial>
    {
        private readonly IFinder _finder;
        private readonly IOperationContextParser _operationContextParser;
        private readonly ITracer _tracer;

        public SerializeAccountDetailHandler(IExportRepository<AccountDetail> exportRepository, ITracer tracer, IFinder finder, IOperationContextParser operationContextParser)
            : base(exportRepository, tracer)
        {
            _finder = finder;
            _tracer = tracer;
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
                                new XAttribute("OrganizationUnitCode", dto.SourceOrganizationUnitSyncCode1C),
                                new XAttribute("StartDate", dto.StartDate),
                                new XAttribute("EndDate", dto.EndDate),
                                new XAttribute("ClientDebitTotalAmount", dto.ClientDebitTotalAmount),
                                new XAttribute("AccountingMethod", dto.AccountingMethod),
                                new XElement("ClientDebits", debits));
        }

        protected override IEnumerable<IExportableEntityDto> ProcessOperations(IEnumerable<PerformedBusinessOperation> operations)
        {
            return operations.OrderBy(operation => operation.Date).Select(FormatExportRecord).Where(x => x != null).ToArray();
        }

        protected override IEnumerable<IExportableEntityDto> ProcessFailedEntities(IEnumerable<ExportFailedEntity> entities)
        {
            throw new NotSupportedException("Обработка невыгруженных с первой попытки пакетов не поддерживается");
        }

        protected override SelectSpecification<AccountDetail, IExportableEntityDto> CreateDtoExpression()
        {
            throw new NotSupportedException("Данные выбираются только пакетом по операции");
        }

        private IExportableEntityDto FormatExportRecord(PerformedBusinessOperation operation)
        {
            var filter = CreateAccountDetailsFilter(operation);
            var selector = AccountDetailDtoSelectSpecification();
            var exportData = _finder.Find(selector, filter).ToArray();

            if (exportData.Length == 0)
            {
                // Ещё один костыль, который можно будет убрать перейдя на новую выгрузку.
                // Дело в том, что если после списания сразу сделали откат, 
                // то при выгрузке списания AccountDetail не будут привязаны к Lock.
                // В этом случае не определить ни город, ни период, ни модель продаж.
                // Но это не страшно - это значит, дальше в очереди есть операция отката, которая выплюнет пустое сообщение.
                _tracer.WarnFormat("Нет данных для экспорта по операции {0} ", operation.Id);
                return null;
            }

            var key = exportData.Select(dto => new DebitContainerKey
                                                   {
                                                       SourceOrganizationUnitSyncCode1C = dto.SourceOrganizationUnitSyncCode1C,
                                                       AccountingMethod = ModelToMethod(dto.SalesModel),
                                                       PeriodStartDate = dto.PeriodStartDate,
                                                       PeriodEndDate = dto.PeriodEndDate
                                                   })
                                .Distinct()
                                .Single();

            var debits = operation.Operation == RevertWithdrawFromAccountsIdentity.Instance.Id
                             ? new DebitDto[0]
                             : exportData.Where(dto => dto.Amount.HasValue && dto.Amount.Value > 0)
                                         .Select(dto => new DebitDto
                                                            {
                                                                AccountCode = dto.AccountId,
                                                                Amount = dto.Amount.Value,
                                                                LegalEntityBranchCode1C = dto.LegalEntityBranchCode1C,
                                                                MediaInfo = dto.ElectronicMedia,
                                                                OrderCode = dto.OrderId,
                                                                OrderNumber = dto.OrderNumber,
                                                                OrderType = (int)dto.OrderType,
                                                                ProfileCode = dto.OrderLegalPersonProfileId ?? dto.MainLegalPersonProfileId,
                                                            })
                                         .ToArray();

            return new DebitContainerDto
                       {
                           Id = operation.Id,
                           SourceOrganizationUnitSyncCode1C = key.SourceOrganizationUnitSyncCode1C,
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

        private SelectSpecification<Lock, AccountDetailDto> AccountDetailDtoSelectSpecification()
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
                             SourceOrganizationUnitSyncCode1C = x.Order.SourceOrganizationUnit.SyncCode1C,
                             SalesModel = x.Order.OrderPositions
                                           .Where(position => position.IsActive && !position.IsDeleted)
                                           .Select(position => position.PricePosition.Position.SalesModel)
                                           .FirstOrDefault(),
                             LegalEntityBranchCode1C = x.Order.BranchOfficeOrganizationUnit.SyncCode1C,
                             MainLegalPersonProfileId = x.Account.LegalPerson.LegalPersonProfiles
                                                         .Where(p => !p.IsDeleted && p.IsMainProfile)
                                                         .Select(p => p.Id)
                                                         .FirstOrDefault(),
                         });
        }

        private FindSpecification<Lock> CreateAccountDetailsFilter(PerformedBusinessOperation operation)
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
                // Обращение в таблицу PBO в прикладной логике экспорта - ОГРОМНЫЙ тех долг, т.к. в прикладной логике мы оперируем понятием очереди,
                // но никак не транспорта, на котором эта очередь реализована. То есть к таблице ни в коем случае нельзя обращаться.
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
            public decimal? Amount { get; set; }
            public DateTime PeriodStartDate { get; set; }
            public DateTime PeriodEndDate { get; set; }
            public long OrderId { get; set; }
            public long? OrderLegalPersonProfileId { get; set; }
            public OrderType OrderType { get; set; }
            public string OrderNumber { get; set; }
            public string ElectronicMedia { get; set; }
            public string SourceOrganizationUnitSyncCode1C { get; set; }
            public SalesModel SalesModel { get; set; }
            public string LegalEntityBranchCode1C { get; set; }
            public long MainLegalPersonProfileId { get; set; }
        }

        private sealed class DebitContainerDto : IExportableEntityDto
        {
            public long Id { get; set; }
            
            public string SourceOrganizationUnitSyncCode1C { get; set; }
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

        private sealed class DebitContainerKey
        {
            public string SourceOrganizationUnitSyncCode1C { get; set; }
            public string AccountingMethod { get; set; }
            public DateTime PeriodStartDate { get; set; }
            public DateTime PeriodEndDate { get; set; }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                {
                    return false;
                }

                if (ReferenceEquals(this, obj))
                {
                    return true;
                }

                return obj is DebitContainerKey && Equals((DebitContainerKey)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = SourceOrganizationUnitSyncCode1C != null ? SourceOrganizationUnitSyncCode1C.GetHashCode() : 0;
                    hashCode = (hashCode * 397) ^ (AccountingMethod != null ? AccountingMethod.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ PeriodStartDate.GetHashCode();
                    hashCode = (hashCode * 397) ^ PeriodEndDate.GetHashCode();
                    return hashCode;
                }
            }

            private bool Equals(DebitContainerKey other)
            {
                return string.Equals(SourceOrganizationUnitSyncCode1C, other.SourceOrganizationUnitSyncCode1C)
                       && string.Equals(AccountingMethod, other.AccountingMethod)
                       && PeriodStartDate.Equals(other.PeriodStartDate)
                       && PeriodEndDate.Equals(other.PeriodEndDate);
            }
        }
    }
}