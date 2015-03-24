using System.Diagnostics.CodeAnalysis;
using System.Linq;

using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Bargain;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Withdrawal;

namespace DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export
{
    public static partial class ExportMetadata
    {
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "Reviewed. Suppression is OK here.")]
        public static readonly QueryRuleContainer<Order> Order = QueryRuleContainer<Order>.Create(
            () => EntityOperationMapping<Order>.ForEntity(EntityName.Order)
                  .Operation<CreateIdentity>()
                  .Operation<UpdateIdentity>()
                  .Operation<AssignIdentity>()
                  .NonCoupledOperation<CopyOrderIdentity>()
                  .NonCoupledOperation<RemoveBargainIdentity>()
                  .NonCoupledOperation<SetInspectorIdentity>()
                  .NonCoupledOperation<ChangeDealIdentity>()
                  .NonCoupledOperation<CloseWithDenialIdentity>()
                  .NonCoupledOperation<RepairOutdatedIdentity>()
                  .Use((finder, ids) => finder.Find(Specs.Find.ByIds<Order>(ids))),

            () => EntityOperationMapping<Order>.ForEntity(EntityName.OrderPosition)
                  .Operation<CreateIdentity>()
                  .Operation<UpdateIdentity>()
                  .Operation<DeleteIdentity>()
                  .Use((finder, ids) => finder.Find(Specs.Find.ByIds<OrderPosition>(ids)).Select(position => position.Order)),

            () => EntityOperationMapping<Order>.ForEntity(EntityName.Deal)
                  .Operation<AssignIdentity>()
                  .Use((finder, ids) => finder.Find(Specs.Find.ByIds<Deal>(ids)).SelectMany(deal => deal.Orders)),

            () => EntityOperationMapping<Order>.ForEntity(EntityName.Client)
                  .Operation<AssignIdentity>()
                  .Operation<DisqualifyIdentity>()
                  .Operation<QualifyIdentity>()
                  .Use((finder, ids) => finder.Find(Specs.Find.ByIds<Client>(ids)).SelectMany(client => client.Firm.Orders)),

            () => EntityOperationMapping<Order>.ForEntity(EntityName.LegalPerson)
                  .Operation<ChangeClientIdentity>()
                  .Operation<AssignIdentity>()
                  .Operation<MergeIdentity>()
                  .Use((finder, ids) => finder.Find(Specs.Find.ByIds<LegalPerson>(ids)).SelectMany(person => person.Orders)),

            () => EntityOperationMapping<Order>.ForEntity(EntityName.Bargain)
                  .NonCoupledOperation<BindToOrderIdentity>()
                  .Use((finder, ids) => finder.Find(Specs.Find.ByIds<Bargain>(ids)).SelectMany(bargain => bargain.Orders)),

            () => EntityOperationMapping<Order>.ForEntity(EntityName.Firm)
                  .NonCoupledOperation<ImportFirmIdentity>()
                  .Use((finder, ids) => finder.Find(Specs.Find.ByIds<Firm>(ids))
                                              .SelectMany(firm => firm.Orders)),

            // При получении карточки адреса фирмы выгружаются все связанные с данной фирмой заказа активные, не удалённые, которые НЕ в статусе "Архив";
            // http://confluence.2gis.local/pages/viewpage.action?pageId=95748369
           () => EntityOperationMapping<Order>.ForEntity(EntityName.Firm)
                  .NonCoupledOperation<ImportCardIdentity>()
                  .Use((finder, ids) => finder.Find(Specs.Find.ByIds<Firm>(ids))
                                              .SelectMany(firm => firm.Orders)
                                              .Where(order => order.WorkflowStepId != OrderState.Archive && order.IsActive && !order.IsDeleted)),

            () => EntityOperationMapping<Order>.ForEntity(EntityName.Firm)
                  .NonCoupledOperation<ImportCardForErmIdentity>()
                  .Use((finder, ids) => finder.Find(Specs.Find.ByIds<Firm>(ids))
                                              .SelectMany(firm => firm.Orders)
                                              .Where(order => order.WorkflowStepId != OrderState.Archive && order.IsActive && !order.IsDeleted)),
     
            () => EntityOperationMapping<Order>.ForEntity(EntityName.WithdrawalInfo)
                  .NonCoupledOperation<WithdrawIdentity>()
                  .Use((finder, ids) => finder.Find(Specs.Find.ByIds<WithdrawalInfo>(ids))
                                              .SelectMany(info => info.OrganizationUnit.OrdersBySource
                                                                    .Where(order => order.WorkflowStepId == OrderState.Archive &&
                                                                                    order.Locks.Count(l => !l.IsDeleted && !l.IsActive) == order.ReleaseCountFact &&
                                                                                    order.EndDistributionDateFact == info.PeriodEndDate))),

            () => EntityOperationMapping<Order>.ForEntity(EntityName.WithdrawalInfo)
                  .NonCoupledOperation<RevertWithdrawalIdentity>()
                  .Use((finder, ids) => finder.Find(Specs.Find.ByIds<WithdrawalInfo>(ids))
                                              .SelectMany(info => info.OrganizationUnit.OrdersBySource
                                                                        .Where(order => (order.WorkflowStepId == OrderState.Approved || 
                                                                                         order.WorkflowStepId == OrderState.OnTermination) &&
                                                                                         order.Locks.Count(l => !l.IsDeleted && !l.IsActive) + 1 == order.ReleaseCountFact && 
                                                                                         order.EndDistributionDateFact == info.PeriodEndDate))));
    }
}
