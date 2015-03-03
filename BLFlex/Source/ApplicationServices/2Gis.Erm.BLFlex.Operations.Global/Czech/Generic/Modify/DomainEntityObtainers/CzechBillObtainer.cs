using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Czech.Generic.Modify.DomainEntityObtainers
{
    public sealed class BillObtainer : IBusinessModelEntityObtainer<Bill>, IAggregateReadModel<Order>, ICzechAdapted
    {
        private readonly IFinder _finder;

        public BillObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public Bill ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (BillDomainEntityDto)domainEntityDto;

            bool isNew = dto.IsNew();
            var bill = isNew
                           ? new Bill { IsActive = true }
                           : _finder.Find(Specs.Find.ById<Bill>(dto.Id)).Single();
            if (isNew)
            {
                bill.OrderId = dto.OrderId;
                bill.BillNumber = dto.BillNumber;
                bill.BeginDistributionDate = dto.BeginDistributionDate;
                bill.EndDistributionDate = dto.EndDistributionDate;
                bill.PayablePlan = dto.PayablePlan;
                bill.OwnerCode = dto.OwnerRef.Id.Value;
            }
            else
            {
                bill.Timestamp = dto.Timestamp;
            }

            bill.BillDate = dto.BillDate;
            bill.PaymentDatePlan = dto.PaymentDatePlan;
            bill.Comment = dto.Comment;

            return bill;
        }
    }
}