using System;

using DoubleGis.Erm.BLCore.Aggregates.Orders;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Old
{
    public sealed class EditBargainHandler : RequestHandler<EditRequest<Bargain>, EmptyResponse>
    {
        private readonly IBargainRepository _bargainRepository;

        public EditBargainHandler(IBargainRepository bargainRepository)
        {
            _bargainRepository = bargainRepository;
        }

        protected override EmptyResponse Handle(EditRequest<Bargain> request)
        {
            if (request.Entity.IsNew())
            {
                throw new NotSupportedException("Only update operation supported");
            }

            _bargainRepository.Update(request.Entity);
            return Response.Empty;
        }
    }
}