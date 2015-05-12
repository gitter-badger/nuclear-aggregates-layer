using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Aggregates;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.DomainEntityObtainers
{
    public sealed class FirmObtainer : IBusinessModelEntityObtainer<Firm>, IAggregateReadModel<Firm>
    {
        private readonly IFinder _finder;

        public FirmObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public Firm ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (FirmDomainEntityDto)domainEntityDto;

            // do not set Territory to entity
            // do not set ClosedForAscertainment to entity
            var firm = _finder.FindOne(Specs.Find.ById<Firm>(dto.Id)) 
                ?? new Firm { IsActive = true };

            firm.Id = dto.Id;
            firm.ReplicationCode = dto.ReplicationCode;
            firm.Name = dto.Name;
            firm.PromisingScore = dto.PromisingScore;
            firm.ProductType = dto.ProductType;
            firm.UsingOtherMedia = dto.UsingOtherMedia;
            firm.MarketType = dto.MarketType;
            firm.BudgetType = dto.BudgetType;
            firm.Geolocation = dto.Geolocation;
            firm.InCityBranchesAmount = dto.InCityBranchesAmount;
            firm.OutCityBranchesAmount = dto.OutCityBranchesAmount;
            firm.StaffAmount = dto.StaffAmount;
            firm.Comment = dto.Comment;
            firm.ClientId = dto.ClientRef.Id;

            firm.Timestamp = dto.Timestamp;

            return firm;
        }
    }
}