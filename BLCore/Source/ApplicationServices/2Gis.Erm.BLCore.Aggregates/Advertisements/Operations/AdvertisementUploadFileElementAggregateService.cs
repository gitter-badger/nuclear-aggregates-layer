using System.IO;

using DoubleGis.Erm.BLCore.API.Aggregates.Advertisements.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Generic.File;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage.Writings;

namespace DoubleGis.Erm.BLCore.Aggregates.Advertisements.Operations
{
    public sealed class AdvertisementUploadFileElementAggregateService : IAdvertisementUploadElementFileAggregateService
    {
        private readonly ISecureRepository<AdvertisementElement> _secureAdvertisementElementRepository;
        private readonly IRepository<FileWithContent> _fileRepository;
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _scopeFactory;

        public AdvertisementUploadFileElementAggregateService(
            ISecureRepository<AdvertisementElement> secureAdvertisementElementRepository,
            IRepository<FileWithContent> fileRepository,
            IIdentityProvider identityProvider,
            IOperationScopeFactory scopeFactory)
        {
            _secureAdvertisementElementRepository = secureAdvertisementElementRepository;
            _fileRepository = fileRepository;
            _identityProvider = identityProvider;
            _scopeFactory = scopeFactory;
        }

        public UploadFileResult UploadFile(AdvertisementElement advertisementElement,
                                           UploadFileParams<AdvertisementElement> uploadFileParams)
        {
            if (advertisementElement == null)
            {
                throw new BusinessLogicException(BLResources.MustBeExistingEntity);
            }

            var file = new FileWithContent
                {
                    Id = uploadFileParams.FileId,
                    ContentType = uploadFileParams.ContentType,
                    ContentLength = uploadFileParams.ContentLength,
                    Content = uploadFileParams.Content,
                    FileName = Path.GetFileName(uploadFileParams.FileName)
                };

            using (var scope = _scopeFactory.CreateOrUpdateOperationFor(file))
            {
                if (file.IsNew())
                {
                    _identityProvider.SetFor(file);
                    _fileRepository.Add(file);
                    scope.Added<FileWithContent>(file.Id);
                }
                else
                {
                    _fileRepository.Update(file);
                    scope.Updated<FileWithContent>(file.Id);
                }

                _fileRepository.Save();

                // до запиливания специфического OperationService загрузки файлов к элементу рекламного материала usecase загрузки был крайне мутный - фактически двухфазный, 
                // нужно было сначала загрузить файл, а потом через usecase обновления ЭРМ сохранить связь ЭРМ с этим файлом
                // При этом складывалось ложное впечатление, что если не нажимать кнопку сохранить на карточке, то реальной смены файла у рекламного материала не будет, однако, это не так,
                // т.к. изменился контент в таблице filebinaries, то независимо от сохранения/не сохранения карточки, контент файла обновлялся (единственное исключение - это когда файла раньше не было)
                // После рефакторинга FileId обновляется в рамках поддержания инварианта добавления файла к ЭРМ, т.е. всегда, нажатие на Save в карточке для этого совсем не требуется.
                advertisementElement.FileId = file.Id;

                _secureAdvertisementElementRepository.Update(advertisementElement);
                scope.Updated<AdvertisementElement>(advertisementElement.Id);
                _secureAdvertisementElementRepository.Save();

                scope.Complete();
            }

            return new UploadFileResult
                {
                    ContentType = file.ContentType,
                    ContentLength = file.ContentLength,
                    FileName = file.FileName,
                    FileId = file.Id
                };
        }
    }
}
