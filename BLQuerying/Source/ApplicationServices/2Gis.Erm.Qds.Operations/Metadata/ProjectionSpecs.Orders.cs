using System.Globalization;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Qds.API.Operations.Docs;
using DoubleGis.Erm.Qds.Operations.Indexing;

using FastMember;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Qds.Operations.Metadata
{
    public static partial class ProjectionSpecs
    {
        public static class Orders
        {
            public static ISelectSpecification<Order, object> Select()
            {
                return new SelectSpecification<Order, object>(
                    x => new
                             {
                                 x.Id,
                                 x.Number,

                                 x.BeginDistributionDate,
                                 x.EndDistributionDatePlan,
                                 x.EndDistributionDateFact,

                                 x.IsActive,
                                 x.IsDeleted,
                                 x.HasDocumentsDebt,

                                 x.CreatedOn,
                                 x.ModifiedOn,

                                 x.PayablePlan,
                                 x.WorkflowStepId,

                                 x.AmountToWithdraw,
                                 x.AmountWithdrawn,

                                 x.FirmId,
                                 x.OwnerCode,
                                 x.SourceOrganizationUnitId,
                                 x.DestOrganizationUnitId,
                                 x.LegalPersonId,
                                 x.BargainId,
                                 x.AccountId,
                                 x.Firm.ClientId,
                                 x.DealId,
                             });
            }

            public static IProjectSpecification<ObjectAccessor, IndexedDocumentWrapper<OrderGridDoc>> Project(CultureInfo cultureInfo)
            {
                return new ProjectSpecification<ObjectAccessor, IndexedDocumentWrapper<OrderGridDoc>>(
                    x =>
                        {
                            var accessor = x.BasedOn<Order>();
                            var orderState = accessor.Get(c => c.WorkflowStepId);
                            return new IndexedDocumentWrapper<OrderGridDoc>
                                       {
                                           Id = accessor.Get(c => c.Id).ToString(),
                                           Document = new OrderGridDoc
                                                          {
                                                              Id = accessor.Get(c => c.Id),

                                                              Number = accessor.Get(c => c.Number),

                                                              BeginDistributionDate = accessor.Get(c => c.BeginDistributionDate),
                                                              EndDistributionDatePlan = accessor.Get(c => c.EndDistributionDatePlan),
                                                              EndDistributionDateFact = accessor.Get(c => c.EndDistributionDateFact),

                                                              IsActive = accessor.Get(c => c.IsActive),
                                                              IsDeleted = accessor.Get(c => c.IsDeleted),
                                                              HasDocumentsDebt = accessor.Get(c => c.HasDocumentsDebt),

                                                              CreatedOn = accessor.Get(c => c.CreatedOn),
                                                              ModifiedOn = accessor.Get(c => c.ModifiedOn),

                                                              PayablePlan = (double)accessor.Get(c => c.PayablePlan),
                                                              WorkflowStep = orderState.ToStringLocalized(EnumResources.ResourceManager, cultureInfo),

                                                              AmountToWithdraw = (double)accessor.Get(c => c.AmountToWithdraw),
                                                              AmountWithdrawn = (double)accessor.Get(c => c.AmountWithdrawn),

                                                              // relations
                                                              FirmId = GetRelatedId(accessor.Get(c => c.FirmId)),
                                                              OwnerCode = GetRelatedId(accessor.Get(c => c.OwnerCode)),
                                                              SourceOrganizationUnitId = GetRelatedId(accessor.Get(c => c.SourceOrganizationUnitId)),
                                                              DestOrganizationUnitId = GetRelatedId(accessor.Get(c => c.DestOrganizationUnitId)),
                                                              LegalPersonId = GetRelatedId(accessor.Get(c => c.LegalPersonId)),
                                                              BargainId = GetRelatedId(accessor.Get(c => c.BargainId)),
                                                              AccountId = GetRelatedId(accessor.Get(c => c.AccountId)),
                                                              ClientId = GetRelatedId(accessor.Get(c => c.Firm.ClientId)),
                                                              DealId = GetRelatedId(accessor.Get(c => c.DealId)),
                                                          }
                                       };
                        });
            }
        }
    }
}