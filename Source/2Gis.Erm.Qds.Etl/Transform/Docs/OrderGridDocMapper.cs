using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Qds.Docs;

namespace DoubleGis.Erm.Qds.Etl.Transform.Docs
{
    public class OrderGridDocMapper : IDocMapper<OrderGridDoc>
    {
        readonly IEnumLocalizer _enumLocalizer;

        public OrderGridDocMapper(IEnumLocalizer enumLocalizer)
        {
            if (enumLocalizer == null)
            {
                throw new ArgumentNullException("enumLocalizer");
            }

            _enumLocalizer = enumLocalizer;
        }

        public void UpdateDocByEntity(IEnumerable<OrderGridDoc> docs, IEntityKey entity)
        {
            if (docs == null)
            {
                throw new ArgumentNullException("docs");
            }

            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            if (!TryUpdate(docs, entity as Order))
                throw new NotSupportedException(entity.GetType().FullName);
        }

        bool TryUpdate(IEnumerable<OrderGridDoc> docs, Order e)
        {
            if (docs == null)
            {
                throw new ArgumentNullException("docs");
            }

            if (e == null) return false;

            foreach (var doc in docs)
            {
                doc.Id = e.Id.ToString();
                doc.Number = e.Number;
                doc.BeginDistributionDate = e.BeginDistributionDate;
                doc.EndDistributionDatePlan = e.EndDistributionDatePlan;
                doc.EndDistributionDateFact = e.EndDistributionDateFact;
                doc.IsActive = e.IsActive;
                doc.IsDeleted = e.IsDeleted;
                doc.HasDocumentsDebt = e.HasDocumentsDebt;
                doc.CreatedOn = e.CreatedOn;
                doc.ModifiedOn = e.ModifiedOn;
                doc.PayablePlan = (double)e.PayablePlan;
                doc.WorkflowStepId = e.WorkflowStepId;
                doc.WorkflowStep = _enumLocalizer.LocalizeFromId<OrderState>(e.WorkflowStepId);
                doc.AmountWithdrawn = (double)e.AmountWithdrawn;
                doc.AmountToWithdraw = (double)e.AmountToWithdraw;
            }

            return true;
        }
    }
}