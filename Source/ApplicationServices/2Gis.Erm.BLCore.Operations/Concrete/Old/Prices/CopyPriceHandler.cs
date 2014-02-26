using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using DoubleGis.Erm.BLCore.Aggregates.Prices;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Prices;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Prices
{
    public sealed class CopyPriceHandler : RequestHandler<CopyPriceRequest, CopyPriceResponse>
    {
        private readonly IDictionary<long, PricePosition> _pricePositionMap = new Dictionary<long, PricePosition>();
        private readonly IDictionary<long, AssociatedPositionsGroup> _associatedPositionsGroupMap = new Dictionary<long, AssociatedPositionsGroup>();

        private readonly IUserContext _userContext;
        private readonly ICommonLog _logger;
        private readonly IPriceRepository _priceRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CopyPriceHandler(IUserContext userContext, ICommonLog logger, IPriceRepository priceRepository, IUnitOfWork unitOfWork)
        {
            _userContext = userContext;
            _logger = logger;
            _priceRepository = priceRepository;
            _unitOfWork = unitOfWork;
        }

        protected override CopyPriceResponse Handle(CopyPriceRequest request)
        {
            _priceRepository.CheckPriceBusinessRules(request.SourcePriceId, request.OrganizationUnitId, request.BeginDate, request.PublishDate);

            // это те данные, которые требуетс€ скопировать
            var sourcePrice = _priceRepository.GetPriceToCopyDto(request.SourcePriceId);

            if (sourcePrice == null)
            {
                _logger.FatalEx(BLResources.UnableToGetExisitingPrice);
                throw new NotificationException(BLResources.UnableToGetExisitingPrice);
            }

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                //  лиент может попросить заменить существующий прайс свежескопированным.
                var targetPriceInfo = request.TargetPriceId == null ? null : _priceRepository.GetPriceDto(request.TargetPriceId.Value);

                // ј может и не попросить, в обоих случа€х реально в базе создаетс€ нова€ запись.
                var targetPrice = CreatePrice(request, targetPriceInfo);

                var currencyId = _priceRepository.GetCurrencyId(request.OrganizationUnitId);
                targetPrice.CurrencyId = currencyId;

                _priceRepository.Add(targetPrice);

                // «апрещенные позиции
                using (var scope = _unitOfWork.CreateScope())
                {
                    var scopedPriceRepository = scope.CreateRepository<IPriceRepository>();
                    foreach (var deniedPosition in sourcePrice.DeniedPositions.Select(CopyDeniedPosition))
                    {
                        deniedPosition.PriceId = targetPrice.Id;
                        scopedPriceRepository.Add(deniedPosition);
                    }

                    scope.Complete();
                }

                // ѕозиции прайс-листа
                using (var scope = _unitOfWork.CreateScope())
                {
                    var scopedPriceRepository = scope.CreateRepository<IPriceRepository>();
                    foreach (var pricePosition in sourcePrice.PricePositions)
                    {
                        var targetPricePosition = CopyPricePosition(pricePosition.Item);

                        targetPricePosition.PriceId = targetPrice.Id;
                        scopedPriceRepository.Add(targetPricePosition);
                    }

                    scope.Complete();
                }

                // √руппы сопутствущих позиций прайс-листа
                using (var scope = _unitOfWork.CreateScope())
                {
                    var scopedPriceRepository = scope.CreateRepository<IPriceRepository>();
                    foreach (var pricePosition in sourcePrice.PricePositions)
                    {
                        var targetPricePosition = _pricePositionMap[pricePosition.Item.Id];
                        foreach (var associatedPositionsGroup in pricePosition.AssociatedPositionsGroups.Select(x => CopyAssociatedPositionGroup(x.Item)))
                        {
                            associatedPositionsGroup.PricePositionId = targetPricePosition.Id;
                            scopedPriceRepository.Add(associatedPositionsGroup);
                        }
                    }

                    scope.Complete();
                }

                // —опутствущие позиции прайс-листа
                using (var scope = _unitOfWork.CreateScope())
                {
                    var scopedPriceRepository = scope.CreateRepository<IPriceRepository>();
                    foreach (var positionsGroup in sourcePrice.PricePositions.SelectMany(x => x.AssociatedPositionsGroups))
                    {
                        var targetPositionsGroup = _associatedPositionsGroupMap[positionsGroup.Item.Id];
                        foreach (var associatedPosition in positionsGroup.AssociatedPositions.Select(CopyAssociatedPosition))
                        {
                            associatedPosition.AssociatedPositionsGroupId = targetPositionsGroup.Id;
                            scopedPriceRepository.Add(associatedPosition);
                        }
                    }

                    scope.Complete();
                }

                transaction.Complete();
                return new CopyPriceResponse { TargetPriceId = targetPrice.Id };
            }
        }

        private Price CreatePrice(CopyPriceRequest request, PriceDto targetPriceInfo)
        {
            return targetPriceInfo != null
                       ? new Price
                             {
                                 CreateDate = targetPriceInfo.CreateDate,
                                 PublishDate = targetPriceInfo.PublishDate,
                                 BeginDate = targetPriceInfo.BeginDate,
                                 OrganizationUnitId = targetPriceInfo.OrganizationUnitId,
                                 IsActive = true,
                                 CreatedOn = targetPriceInfo.CreateDate,
                                 CreatedBy = _userContext.Identity.Code
                             }
                       : new Price
                             {
                                 CreateDate = DateTime.UtcNow.Date,

                                 // при копировании в новый прайс даты заданы €вно через UI форму
                                 // ReSharper disable PossibleInvalidOperationException
                                 PublishDate = request.PublishDate.Value,
                                 BeginDate = request.BeginDate.Value,
                                 // ReSharper restore PossibleInvalidOperationException
                                 OrganizationUnitId = request.OrganizationUnitId,
                                 IsActive = true,
                                 CreatedOn = DateTime.UtcNow,
                                 CreatedBy = _userContext.Identity.Code
                             };
        }

        private DeniedPosition CopyDeniedPosition(DeniedPosition position)
        {
            return new DeniedPosition
            {
                PositionId = position.PositionId,
                PositionDeniedId = position.PositionDeniedId,
                ObjectBindingType = position.ObjectBindingType,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = _userContext.Identity.Code,
                IsActive = true
            };
        }

        private PricePosition CopyPricePosition(PricePosition pricePosition)
        {
            var targetPricePosition = new PricePosition
            {
                DgppId = pricePosition.DgppId,
                PositionId = pricePosition.PositionId,
                Cost = pricePosition.Cost,
                Amount = pricePosition.Amount,
                AmountSpecificationMode = pricePosition.AmountSpecificationMode,
                RateType = pricePosition.RateType,
                MaxAdvertisementAmount = pricePosition.MaxAdvertisementAmount,
                MinAdvertisementAmount = pricePosition.MinAdvertisementAmount,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = _userContext.Identity.Code,
                IsActive = true
            };
            _pricePositionMap.Add(pricePosition.Id, targetPricePosition);

            return targetPricePosition;
        }

        private AssociatedPositionsGroup CopyAssociatedPositionGroup(AssociatedPositionsGroup associatedPositionsGroup)
        {
            var targetAssociatedPositionsGroup = new AssociatedPositionsGroup
            {
                    Name = associatedPositionsGroup.Name,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = _userContext.Identity.Code,
                IsActive = true
            };
            _associatedPositionsGroupMap.Add(associatedPositionsGroup.Id, targetAssociatedPositionsGroup);

            return targetAssociatedPositionsGroup;
        }

        private AssociatedPosition CopyAssociatedPosition(AssociatedPosition associatedPosition)
        {
            return new AssociatedPosition
            {
                PositionId = associatedPosition.PositionId,
                ObjectBindingType = associatedPosition.ObjectBindingType,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = _userContext.Identity.Code,
                IsActive = true
            };
        }
    }
}