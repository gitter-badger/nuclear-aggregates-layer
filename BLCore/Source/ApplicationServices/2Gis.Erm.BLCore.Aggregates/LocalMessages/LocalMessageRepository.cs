using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DoubleGis.Erm.BLCore.API.Aggregates.LocalMessages;
using DoubleGis.Erm.BLCore.API.Aggregates.LocalMessages.DTO;
using DoubleGis.Erm.Platform.API.Core.Identities;
using NuClear.Storage;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.BLCore.Aggregates.LocalMessages
// ReSharper restore CheckNamespace
{
    public sealed class LocalMessageRepository : ILocalMessageRepository
    {
        private readonly IFinder _finder;
        private readonly IRepository<LocalMessage> _localMessageGenericRepository;
        private readonly IIdentityProvider _identityProvider;

        public LocalMessageRepository(IFinder finder, IRepository<LocalMessage> localMessageGenericRepository, IIdentityProvider identityProvider)
        {
            _localMessageGenericRepository = localMessageGenericRepository;
            _identityProvider = identityProvider;
            _finder = finder;
        }

        public void Delete(LocalMessage localMessage)
        {
            _localMessageGenericRepository.Delete(localMessage);
            _localMessageGenericRepository.Save();
        }

        public void Create(LocalMessage localMessage, int integrationType)
        {
            localMessage.MessageTypeId = _finder.Find<MessageType>(x => x.IntegrationType == integrationType).Select(x => x.Id).Single();
            _identityProvider.SetFor(localMessage);
            _localMessageGenericRepository.Add(localMessage);
            _localMessageGenericRepository.Save();
        }

        public IEnumerable<LocalMessage> GetByIds(long[] ids)
        {
            return _finder.Find<LocalMessage>(x => ids.Contains(x.Id)).ToArray();
        }

        public void SetProcessingState(LocalMessage localMessage)
        {
            localMessage.Status = LocalMessageStatus.Processing;
            _localMessageGenericRepository.Update(localMessage);
            _localMessageGenericRepository.Save();
        }

        public LocalMessageDto GetMessageToProcess()
        {
            var hasProcessingMessages = _finder.Find<LocalMessage>(x => !x.IsDeleted && x.Status == LocalMessageStatus.Processing).Any();
            if (hasProcessingMessages)
            {
                return null;
            }

            var localMessage = _finder.Find<LocalMessage>(x => !x.IsDeleted && x.Status == LocalMessageStatus.WaitForProcess)
                                .OrderBy(x => x.CreatedOn)
                                .Select(x => new LocalMessageDto
                                {
                                    LocalMessage = x,
                                    IntegrationType = x.MessageType.IntegrationType,
                                    FileName = x.File.FileName,
                    })
                .FirstOrDefault();
            return localMessage;
        }

        public void SetWaitForProcessState(long localMessageId)
        {
            var localMessage = _finder.Find(Specs.Find.ById<LocalMessage>(localMessageId)).Single();
            localMessage.Status = LocalMessageStatus.WaitForProcess;
            _localMessageGenericRepository.Update(localMessage);
            _localMessageGenericRepository.Save();
        }

        public IEnumerable<LocalMessage> GetLongProcessingMessages(int periodInMinutes)
        {
            var period = DateTime.UtcNow.AddMinutes(-periodInMinutes);

            var localMessages = _finder.Find<LocalMessage>(x => !x.IsDeleted && x.Status == LocalMessageStatus.Processing &&
                                                                (x.ModifiedOn.HasValue ? x.ModifiedOn <= period : x.CreatedOn <= period))
                .OrderBy(x => x.CreatedOn)
                .ToArray();
            return localMessages;
        }

        public void SetResult(LocalMessage localMessage, LocalMessageStatus localMessageStatus, IEnumerable<string> messages, long processingTime)
        {
            localMessage.Status = localMessageStatus;
            localMessage.ProcessingTime = processingTime;

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("-- {0:g} --", DateTime.UtcNow);
            stringBuilder.AppendLine();
            foreach (var message in messages)
            {
                stringBuilder.AppendLine(message);
        }
            stringBuilder.AppendLine();

            localMessage.ProcessResult = stringBuilder + localMessage.ProcessResult;

            _localMessageGenericRepository.Update(localMessage);
            _localMessageGenericRepository.Save();
        }
    }
}