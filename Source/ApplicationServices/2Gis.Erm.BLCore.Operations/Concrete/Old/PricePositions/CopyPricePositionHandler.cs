using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Prices;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.PricePositions;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.PricePositions
{
    public sealed class CopyPricePositionHandler : RequestHandler<CopyPricePositionRequest, EmptyResponse>
    {
        private readonly IUserContext _userContext;
        private readonly ICommonLog _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPriceRepository _priceRepository;

        public CopyPricePositionHandler(IUserContext userContext,
            ICommonLog logger,
            IUnitOfWork unitOfWork,
            IPriceRepository priceRepository)
        {
            _userContext = userContext;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _priceRepository = priceRepository;
        }

        protected override EmptyResponse Handle(CopyPricePositionRequest request)
        {
            if (!_priceRepository.PriceExists(request.PriceId))
            {
                _logger.FatalEx(BLResources.UnableToGetExisitingPrice);
                throw new NotificationException(BLResources.UnableToGetExisitingPrice);
            }

            if (_priceRepository.PriceContainsPosition(request.PriceId, request.PositionId))
            {
                _logger.FatalEx(BLResources.PricePositionForPositionAlreadyCreated);
                throw new NotificationException(BLResources.PricePositionForPositionAlreadyCreated);
            }

            var sourcePricePosition = _priceRepository.GetPricePosition(request.SourcePricePositionId);
            if (sourcePricePosition == null)
            {
                _logger.FatalEx(BLResources.UnableToGetExisitingPricePosition);
                throw new NotificationException(BLResources.UnableToGetExisitingPricePosition);
            }

            var targetPricePosition = new PricePosition
            {
                PriceId = request.PriceId,
                DgppId = sourcePricePosition.DgppId,
                PositionId = request.PositionId,
                RatePricePositions = sourcePricePosition.RatePricePositions,
                Cost = sourcePricePosition.Cost,
                Amount = sourcePricePosition.Amount,
                AmountSpecificationMode = sourcePricePosition.AmountSpecificationMode,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = _userContext.Identity.Code,
                IsActive = true
            };

            using (var scope = _unitOfWork.CreateScope())
            {
                var scopedPriceRepository = scope.CreateRepository<IPriceRepository>();

                // Позиция прайса.
                scopedPriceRepository.CreateOrUpdate(targetPricePosition);

                // Запрещенные позиции. Они не связаны с позицией прайса.
                var sourceDeniedPositions = scopedPriceRepository.GetDeniedPositions(request.PriceId, sourcePricePosition.PositionId);
                var positions = CopyDeniedPositions(sourceDeniedPositions, request.PositionId);
                foreach (var position in positions)
                {
                    position.PriceId = request.PriceId;
                    scopedPriceRepository.CreateOrUpdate(position);
                }

                scope.Complete();
            }

            // Сразу связать группы и их позиции неполучается из-за отложенного сохранения, поэтому связь хранится в этом списке.
            var groupPositions = new List<Tuple<AssociatedPositionsGroup, IEnumerable<AssociatedPosition>>>();
            using (var scope = _unitOfWork.CreateScope())
            {
                var scopedPriceRepository = scope.CreateRepository<IPriceRepository>();
                
                // Группы основных позиций привязаны к позиции прайса по id
                var associatedPositionsGroups = scopedPriceRepository.GetAssociatedPositionsGroups(sourcePricePosition.Id);
                foreach (var sourceGroup in associatedPositionsGroups)
                {
                    var targetGroup = CopyAssociatedPositionsGroup(sourceGroup);
                    targetGroup.PricePositionId = targetPricePosition.Id;
                    scopedPriceRepository.CreateOrUpdate(targetGroup);

                    var targetGroupPositions = scopedPriceRepository.GetAssociatedPositions(sourceGroup.Id).Select(CopyAssociatedPosition).ToArray();
                    groupPositions.Add(new Tuple<AssociatedPositionsGroup, IEnumerable<AssociatedPosition>>(targetGroup, targetGroupPositions));
                }

                scope.Complete();
            }

            using (var scope = _unitOfWork.CreateScope())
            {
                var scopedPriceRepository = scope.CreateRepository<IPriceRepository>();

                // Основные позиции привязаны к группам по id
                foreach (var tuple in groupPositions)
                {
                    var group = tuple.Item1;
                    foreach (var associatedPosition in tuple.Item2)
                    {
                        associatedPosition.AssociatedPositionsGroupId = group.Id;
                        scopedPriceRepository.CreateOrUpdate(associatedPosition);
                    }
                }

                scope.Complete();
            }

            return Response.Empty;
        }

        private IEnumerable<DeniedPosition> CopyDeniedPositions(IEnumerable<DeniedPosition> deniedPositions, long targetPositionId)
        {
            var result = new List<DeniedPosition>();

            // Если исходную позицию нельзя было добавить дважды в заказ, то и скопиранную нельзя будет.
            var priceSelfDeniedPosition = deniedPositions.SingleOrDefault(x => x.PositionId == x.PositionDeniedId);

            if (priceSelfDeniedPosition != null)
            {
                var targetSelfDeniedPosition = CreateDeniedPosition(targetPositionId,
                                                                               targetPositionId,
                                                                               priceSelfDeniedPosition.ObjectBindingType);
                result.Add(targetSelfDeniedPosition);
            }

            // Остальные запрещенные. Симметричные позиции репозиторий при сохранении создаёт сам.
            var priceDeniedPositions = deniedPositions.Where(x => x.PositionDeniedId != x.PositionId);
            foreach (var deniedPosition in priceDeniedPositions)
            {
                var targetDeniedPosition = CreateDeniedPosition(targetPositionId,
                                                                deniedPosition.PositionDeniedId,
                                                                deniedPosition.ObjectBindingType);
                result.Add(targetDeniedPosition);
            }

            return result;
        }

        private AssociatedPositionsGroup CopyAssociatedPositionsGroup(AssociatedPositionsGroup positionGroup)
        {
            return new AssociatedPositionsGroup
            {
                Name = positionGroup.Name,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = _userContext.Identity.Code,
                IsActive = true
            };
        }

        private AssociatedPosition CopyAssociatedPosition(AssociatedPosition position)
        {
            return new AssociatedPosition
            {
                PositionId = position.PositionId,
                ObjectBindingType = position.ObjectBindingType,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = _userContext.Identity.Code,
                IsActive = true
            };
        }

        private DeniedPosition CreateDeniedPosition(long positionId, long positionDeniedId, int objectBindingType)
        {
            return new DeniedPosition
            {
                PositionId = positionId,
                PositionDeniedId = positionDeniedId,
                ObjectBindingType = objectBindingType,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = _userContext.Identity.Code,
                IsActive = true
            };
        }
    }
}