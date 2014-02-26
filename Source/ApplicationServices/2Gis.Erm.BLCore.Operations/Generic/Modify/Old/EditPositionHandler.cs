﻿using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Prices;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Old
{
    public sealed class EditPositionHandler : RequestHandler<EditRequest<Position>, EmptyResponse>
    {
        private static readonly PositionBindingObjectType[] AllowedBindingObjectTypes =
            {
                PositionBindingObjectType.Firm,
                PositionBindingObjectType.CategorySingle
            };

        private readonly IPositionRepository _positionRepository;

        public EditPositionHandler(IPositionRepository positionRepository)
        {
            _positionRepository = positionRepository;
        }

        protected override EmptyResponse Handle(EditRequest<Position> request)
        {
            var position = request.Entity;

            if (position.IsComposite && !AllowedBindingObjectTypes.Contains((PositionBindingObjectType)position.BindingObjectTypeEnum))
            {
                throw new NotificationException(string.Format(BLResources.CompositePositionLinkingObjectTypeMustOneOf,
                                                              string.Join(", ",
                                                                          AllowedBindingObjectTypes.Select(
                                                                              x => x.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture)))));
            }

            if (position.IsComposite && position.AdvertisementTemplateId.HasValue)
            {
                throw new NotificationException(BLResources.CompositePositionCannotBeWithAdvertisementTemplate);
            }

            _positionRepository.CreateOrUpdate(position);

            return Response.Empty;
        }
    }
}