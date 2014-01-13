using System;
using System.Transactions;

using DoubleGis.Erm.BLCore.Aggregates.LocalMessages;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.LocalMessages;
using DoubleGis.Erm.BLCore.API.Operations.Generic.File;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.LocalMessages
{
    public sealed class CreateLocalMessageHandler : RequestHandler<CreateLocalMessageRequest, EmptyResponse>
    {
        private readonly IFileService _fileService;
        private readonly ILocalMessageRepository _localMessageRepository;

        public CreateLocalMessageHandler(IFileService fileService, ILocalMessageRepository localMessageRepository)
        {
            _fileService = fileService;
            _localMessageRepository = localMessageRepository;
        }

        protected override EmptyResponse Handle(CreateLocalMessageRequest request)
        {
            var localMessage = request.Entity;

            if (localMessage.Id > 0)
            {
                throw new InvalidOperationException("This handler cannot update existing messages");
            }

            if (localMessage.FileId > 0)
            {
                throw new InvalidOperationException("This handler cannot update existing file");
            }

            var file = new FileWithContent { Id = -1, ContentType = request.ContentType, Content = request.Content, FileName = request.FileName };

            using (var transactionScope = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                _fileService.Add(file);

                localMessage.FileId = file.Id;
                _localMessageRepository.Create(localMessage, request.IntegrationType);
                transactionScope.Complete();
            }

            return Response.Empty;
        }
    }
}