using System.Collections.Generic;
using System.IO;
using System.Net.Mime;

using DoubleGis.Erm.BLCore.API.Aggregates.LocalMessages;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.LocalMessages;
using DoubleGis.Erm.BLCore.API.Operations.Generic.File;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.Common.Compression;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.LocalMessages
{
    public sealed class SaveLocalMessageHandler : RequestHandler<SaveLocalMessageRequest, StreamResponse>
    {
        private readonly ILocalMessageRepository _localMessageRepository;
        private readonly IFileService _fileService;

        public SaveLocalMessageHandler(ILocalMessageRepository localMessageRepository, IFileService fileService)
        {
            _localMessageRepository = localMessageRepository;
            _fileService = fileService;
        }

        protected override StreamResponse Handle(SaveLocalMessageRequest request)
        {
            var files = new Dictionary<string, Stream>();

            var localMessages = _localMessageRepository.GetByIds(request.Ids);

            foreach (var localMessage in localMessages)
            {
                var file = _fileService.GetFileById(localMessage.FileId);

                var fileName = file.FileName;
                if (string.IsNullOrEmpty(fileName) || files.ContainsKey(fileName))
                {
                    fileName = string.Format("LocalMessage-id{0}", localMessage.Id);
                }

                var memoryStream = new MemoryStream((int)file.ContentLength);
                file.Content.CopyTo(memoryStream);

                files.Add(fileName, memoryStream);
            }

            var zipStream = files.ZipStreamDictionary();

            foreach (var stream in files.Values)
            {
                stream.Dispose();
            }

            files.Clear();

            return new StreamResponse
            {
                Stream = zipStream,
                ContentType = MediaTypeNames.Application.Zip,
                FileName = "LocalMessages.zip"
            };
        }
    }
}
