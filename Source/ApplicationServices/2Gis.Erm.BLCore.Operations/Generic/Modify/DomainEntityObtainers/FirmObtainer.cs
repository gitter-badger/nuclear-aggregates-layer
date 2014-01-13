using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

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
            var firm = dto.Id == 0
                           ? new Firm { IsActive = true }
                           : _finder.Find(Specs.Find.ById<Firm>(dto.Id)).Single();

            firm.Id = dto.Id;
            firm.ReplicationCode = dto.ReplicationCode;
            firm.Name = dto.Name;
            firm.PromisingScore = dto.PromisingScore;
            firm.ProductType = (int)dto.ProductType;
            firm.UsingOtherMedia = (int)dto.UsingOtherMedia;
            firm.MarketType = (int)dto.MarketType;
            firm.BudgetType = (int)dto.BudgetType;
            firm.Geolocation = (int)dto.Geolocation;
            firm.InCityBranchesAmount = (int)dto.InCityBranchesAmount;
            firm.OutCityBranchesAmount = (int)dto.OutCityBranchesAmount;
            firm.StaffAmount = (int)dto.StaffAmount;
            firm.Comment = dto.Comment;
            firm.ClientId = dto.ClientRef.Id;

            firm.Timestamp = dto.Timestamp;

            return firm;
        }
    }
}