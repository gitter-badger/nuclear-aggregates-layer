using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.CommonService;
using DoubleGis.Erm.BLCore.Aggregates.SimplifiedModel.OrderProcessingRequest;
using DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Model.Entities.Enums;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;

namespace DoubleGis.Erm.BLCore.Operations.Special.OrderProcessingRequests.Concrete
{
    // FIXME {all, 13.01.2014}: нужна конвертация в readmodel с перемещением в нужную сборку
    public sealed class ReadOrderRequestMessagesService : IReadOrderRequestMessagesService
    {
        private readonly IFinder _finder;
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;

        public ReadOrderRequestMessagesService(
            IFinder finder,
            ISecurityServiceUserIdentifier userIdentifierService)
        {
            _finder = finder;
            _userIdentifierService = userIdentifierService;
        }

        public IEnumerable<RequestMessageDetailDto> GetRequestMessages(long requestId)
        {
            var result =
                _finder
                    .Find(
                        OrderProcessingRequestMessageSpecifications.Find.ActiveMessages() &
                        OrderProcessingRequestMessageSpecifications.Find.ByRequestId(requestId))
                    .Select(x => new
                        {
                            x.Id,
                            x.GroupId,
                            x.OrderRequestId,
                            x.MessageParameters,
                            x.MessageTemplateCode,
                            x.MessageType,
                            x.CreatedBy,
                            x.CreatedOn
                        })
                    .ToArray()
                    .Select(x => new RequestMessageDetailDto
                        {
                            CreatedBy = _userIdentifierService.GetUserInfo(x.CreatedBy).DisplayName,
                            RequestId = x.OrderRequestId,
                            CreatedOn = x.CreatedOn,
                            GroupId = x.GroupId,
                            Id = x.Id,
                            MessageText = MessageHelper.MakeMessage(x.MessageTemplateCode, x.MessageParameters),
                            MessageType =
                                ((RequestMessageType)x.MessageType).ToStringLocalized(EnumResources.ResourceManager,
                                                                                      CultureInfo.CurrentCulture)
                        }).ToArray();

            return result;
        }
    }
}
