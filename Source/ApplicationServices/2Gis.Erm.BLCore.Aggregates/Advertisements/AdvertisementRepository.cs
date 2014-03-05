using System;
using System.Globalization;
using System.IO;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Advertisements.DTO;
using DoubleGis.Erm.BLCore.Aggregates.Advertisements.ReadModel;
using DoubleGis.Erm.BLCore.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Advertisements;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Operations.Generic.File;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Advertisements
{
    public sealed class AdvertisementRepository : IAdvertisementRepository
    {
        private readonly IRepository<Advertisement> _advertisementGenericRepository;
        private readonly IRepository<AdvertisementElement> _advertisementElementGenericRepository;
        private readonly IRepository<AdvertisementTemplate> _advertisementTemplateGenericRepository;
        private readonly IRepository<AdvertisementElementTemplate> _advertisementElementTemplateGenericRepository;
        private readonly IRepository<AdsTemplatesAdsElementTemplate> _adsTemplatesAdsElementTemplateGenericRepository;
        private readonly IRepository<FileWithContent> _fileRepository;
        private readonly IUserContext _userContext;
        private readonly IFinder _finder;
        private readonly IFileContentFinder _fileContentFinder;
        private readonly ISecureFinder _secureFinder;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IIdentityProvider _identityProvider;

        public AdvertisementRepository(IRepository<Advertisement> advertisementGenericRepository,
                                       IRepository<AdvertisementElement> advertisementElementGenericRepository,
                                       IRepository<AdvertisementTemplate> advertisementTemplateGenericRepository,
                                       IRepository<AdvertisementElementTemplate> advertisementElementTemplateGenericRepository,
                                       IRepository<AdsTemplatesAdsElementTemplate> adsTemplatesAdsElementTemplateGenericRepository,
                                       IRepository<FileWithContent> fileRepository,
                                       IUserContext userContext,
                                       IFinder finder,
                                       ISecureFinder secureFinder,
                                       IIdentityProvider identityProvider,
                                       IOperationScopeFactory scopeFactory,
                                       IFileContentFinder fileContentFinder)
        {
            _advertisementGenericRepository = advertisementGenericRepository;
            _advertisementElementGenericRepository = advertisementElementGenericRepository;
            _advertisementTemplateGenericRepository = advertisementTemplateGenericRepository;
            _advertisementElementTemplateGenericRepository = advertisementElementTemplateGenericRepository;
            _adsTemplatesAdsElementTemplateGenericRepository = adsTemplatesAdsElementTemplateGenericRepository;
            _fileRepository = fileRepository;
            _userContext = userContext;
            _finder = finder;
            _secureFinder = secureFinder;
            _scopeFactory = scopeFactory;
            _identityProvider = identityProvider;
            _scopeFactory = scopeFactory;
            _fileContentFinder = fileContentFinder;
        }

        public int Delete(Advertisement entity)
        {
            using (var scope = _scopeFactory.CreateSpecificFor<DeleteIdentity, Advertisement>())
            {
                var hasOrderPositions = _finder.Find<Advertisement>(x => x.Id == entity.Id)
                                               .SelectMany(x => x.OrderPositionAdvertisements)
                                               .Select(x => x.OrderPosition)
                                               .Any(x => !x.IsDeleted);
                if (hasOrderPositions)
                {
                    throw new ArgumentException(BLResources.UnableToDeleteAssignedAdvertisement);
                }

                var advertisementElements = _finder.Find(Specs.Find.ById<Advertisement>(entity.Id)).SelectMany(x => x.AdvertisementElements);
                foreach (var advertisementElement in advertisementElements)
                {
                    _advertisementElementGenericRepository.Delete(advertisementElement);
                    scope.Deleted<AdvertisementElement>(advertisementElement.Id);
                }

                _advertisementGenericRepository.Delete(entity);

                var count = _advertisementGenericRepository.Save() + _advertisementElementGenericRepository.Save();

                scope.Deleted<Advertisement>(entity.Id)
                     .Complete();

                return count;
            }
        }

        public int Delete(AdvertisementTemplate entity)
        {
            using (var scope = _scopeFactory.CreateSpecificFor<DeleteIdentity, AdvertisementTemplate>())
            {
                var validations = _finder.Find<AdvertisementTemplate>(x => x.Id == entity.Id)
                                         .Select(x => new
                                             {
                                                 HasNotDeletedAdvertisements = x.Advertisements.Any(y => !y.IsDeleted && y.FirmId != null),
                                                 HasNotDeletedPositions = x.Positions.Any(y => !y.IsDeleted),
                                                 DummyAdvertisement = x.Advertisements.FirstOrDefault(y => y.Id == x.DummyAdvertisementId),
                                                 AdsTemplatesAdsElementTemplates = x.AdsTemplatesAdsElementTemplates.Where(y => !y.IsDeleted)
                                             })
                                         .Single();

                if (validations.HasNotDeletedAdvertisements)
                {
                    throw new ArgumentException(BLResources.UnableToDeleteAssignedAdvertisementTemplate);
                }

                if (validations.HasNotDeletedPositions)
                {
                    throw new ArgumentException(BLResources.UnableToDeleteAdvertisementTemplateWhileUsedInPositions);
                }

                Delete(validations.DummyAdvertisement);
                foreach (var adsTemplatesAdsElementTemplate in validations.AdsTemplatesAdsElementTemplates)
                {
                    Delete(adsTemplatesAdsElementTemplate);
                }

                _advertisementTemplateGenericRepository.Delete(entity);
                var count = _advertisementTemplateGenericRepository.Save();

                scope.Deleted<AdvertisementTemplate>(entity.Id)
                     .Complete();

                return count;
            }
        }

        public int Delete(AdvertisementElement entity)
        {
            int deletedCount;
            using (var scope = _scopeFactory.CreateSpecificFor<DeleteIdentity, AdvertisementElement>())
            {
                _advertisementElementGenericRepository.Delete(entity);
                deletedCount = _advertisementElementGenericRepository.Save();

                scope.Deleted<AdvertisementElement>(entity.Id)
                     .Complete();
            }

            return deletedCount;
        }

        public int Delete(AdvertisementElementTemplate entity)
        {
            int deletedCount;
            using (var scope = _scopeFactory.CreateSpecificFor<DeleteIdentity, AdvertisementElementTemplate>())
            {
                var validations = _finder.Find<AdvertisementElementTemplate>(x => x.Id == entity.Id)
                                         .Select(x => new
                                             {
                                                 HasNotDeletedAdvertisementTemplates = x.AdsTemplatesAdsElementTemplates.Any(y => !y.IsDeleted),
                                             })
                                         .Single();

                if (validations.HasNotDeletedAdvertisementTemplates)
                {
                    throw new ArgumentException(BLResources.UnableToDeleteAssignedAdvertisementElementTemplate);
                }

                _advertisementElementTemplateGenericRepository.Delete(entity);
                deletedCount = _advertisementElementTemplateGenericRepository.Save();

                scope.Deleted<AdvertisementElementTemplate>(entity.Id)
                     .Complete();
            }

            return deletedCount;
        }

        public int Delete(AdsTemplatesAdsElementTemplate entity)
        {
            int deletedCount;
            using (var scope = _scopeFactory.CreateSpecificFor<DeleteIdentity, AdsTemplatesAdsElementTemplate>())
            {
                // Нельзя удалить связь, если у нас есть ЭРМ(не заглушка) с контентом
                if (_finder.Find<AdvertisementElement>(x =>
                                                           !x.IsDeleted && !x.Advertisement.IsDeleted && x.Advertisement.FirmId != null &&
                                                           x.AdsTemplatesAdsElementTemplatesId == entity.Id &&
                                                           (x.FileId != null || x.BeginDate != null || x.EndDate != null || x.Text != null)).Any())
                {
                    throw new BusinessLogicException(BLResources.CanNotDeleteAdsTemplatesAdsElementTemplateSinceThereIsAdvertisementMaterialsWithContent);
                }

                _adsTemplatesAdsElementTemplateGenericRepository.Delete(entity);
                scope.Deleted<AdsTemplatesAdsElementTemplate>(entity.Id);

                // удаляем связанные с этим шаблоном элементы РМ из всех РМ
                var advertisementElementsWithAdvertisements = _secureFinder
                    .Find(Specs.Find.ById<AdsTemplatesAdsElementTemplate>(entity.Id))
                    .SelectMany(x => x.AdvertisementElements)
                    .ToArray();

                foreach (var advertisementElement in advertisementElementsWithAdvertisements)
                {
                    _advertisementElementGenericRepository.Delete(advertisementElement);
                    scope.Deleted<AdvertisementElement>(advertisementElement.Id);
                }

                deletedCount = _adsTemplatesAdsElementTemplateGenericRepository.Save() + _advertisementElementGenericRepository.Save();
                scope.Complete();
            }

            return deletedCount;
        }

        public bool IsAdvertisementTemplatePublished(long advertisementTemplateId)
        {
            return _finder.Find(Specs.Find.ById<AdvertisementTemplate>(advertisementTemplateId)).Select(x => x.IsPublished).Single();
        }

        public bool IsAdvertisementTemplateTheSameInAdvertisementAndElements(long advertisementTemplateId, long advertisementId)
        {
            return !_finder.Find<AdvertisementElement>(
                                                       x => !x.IsDeleted
                                                            && x.AdvertisementId == advertisementId
                                                            && x.AdsTemplatesAdsElementTemplate.AdsTemplateId != advertisementTemplateId)
                           .Any();
        }

        public AdsTemplatesAdsElementTemplate GetAdsTemplatesAdsElementTemplate(long entityId)
        {
            return _finder.Find(Specs.Find.ById<AdsTemplatesAdsElementTemplate>(entityId)).Single();
        }

        public AdvertisementElementTemplate GetAdvertisementElementTemplate(long entityId)
        {
            return _finder.Find(Specs.Find.ById<AdvertisementElementTemplate>(entityId)).Single();
        }

        public Advertisement GetSelectedToWhiteListAdvertisement(long firmId)
        {
            return _finder.Find<Advertisement>(x => x.FirmId == firmId && !x.IsDeleted && x.IsSelectedToWhiteList).SingleOrDefault();
        }

        public void CreateOrUpdate(AdvertisementTemplate advertisementTemplate)
        {
            var notUniqueName = _finder.Find<AdvertisementTemplate>(x => x.Id != advertisementTemplate.Id &&
                                                                         !x.IsDeleted &&
                                                                         x.Name == advertisementTemplate.Name)
                                       .Any();
            if (notUniqueName)
            {
                throw new NotificationException(string.Format(CultureInfo.CurrentCulture,
                                                              BLResources.AdvertisementTemplateNameMustBeUnique,
                                                              advertisementTemplate.Name));
            }

            using (var scope = _scopeFactory.CreateOrUpdateOperationFor(advertisementTemplate))
            {
                if (advertisementTemplate.IsNew())
                {
                    _advertisementTemplateGenericRepository.Add(advertisementTemplate);
                    scope.Added<AdvertisementTemplate>(advertisementTemplate.Id);
                }
                else
                {
                    _advertisementTemplateGenericRepository.Update(advertisementTemplate);
                    scope.Updated<AdvertisementTemplate>(advertisementTemplate.Id);
                }

                _advertisementTemplateGenericRepository.Save();

                scope.Complete();
            }            
        }

        public void CreateOrUpdate(AdvertisementElementTemplate advertisementElementTemplate)
        {
            using (var scope = _scopeFactory.CreateOrUpdateOperationFor(advertisementElementTemplate))
            {
                var notUniqueName = _finder.Find<AdvertisementElementTemplate>(x => x.Id != advertisementElementTemplate.Id &&
                                                                                    !x.IsDeleted &&
                                                                                    x.Name == advertisementElementTemplate.Name)
                                           .Any();
                if (notUniqueName)
                {
                    throw new NotificationException(string.Format(CultureInfo.CurrentCulture,
                                                                  BLResources.AdvertisementTemplateNameMustBeUnique,
                                                                  advertisementElementTemplate.Name));
                }

                if (advertisementElementTemplate.IsAdvertisementLink && advertisementElementTemplate.FormattedText)
                {
                    throw new NotificationException(BLResources.AdvertisementElementTemplateIsLinkIsFormattedError);
                }

                if (advertisementElementTemplate.IsNew())
                {
                    _advertisementElementTemplateGenericRepository.Add(advertisementElementTemplate);
                    scope.Added<AdvertisementElementTemplate>(advertisementElementTemplate.Id);
                }
                else
                {
                    _advertisementElementTemplateGenericRepository.Update(advertisementElementTemplate);
                    scope.Updated<AdvertisementElementTemplate>(advertisementElementTemplate.Id);
                }

                _advertisementElementTemplateGenericRepository.Save();
                
                scope.Complete();
            }
        }

        public void Create(AdsTemplatesAdsElementTemplate adsTemplatesAdsElementTemplate)
        {
            using (var scope = _scopeFactory.CreateSpecificFor<CreateIdentity, AdsTemplatesAdsElementTemplate>())
            {
                _identityProvider.SetFor(adsTemplatesAdsElementTemplate);
                _adsTemplatesAdsElementTemplateGenericRepository.Add(adsTemplatesAdsElementTemplate);
                _adsTemplatesAdsElementTemplateGenericRepository.Save();

                scope.Added<AdsTemplatesAdsElementTemplate>(adsTemplatesAdsElementTemplate.Id)
                     .Complete();
            }
        }

        public void Update(AdsTemplatesAdsElementTemplate adsTemplatesAdsElementTemplate)
        {
            using (var scope = _scopeFactory.CreateSpecificFor<UpdateIdentity, AdsTemplatesAdsElementTemplate>())
            {
                _adsTemplatesAdsElementTemplateGenericRepository.Update(adsTemplatesAdsElementTemplate);
                _adsTemplatesAdsElementTemplateGenericRepository.Save();
            
                scope.Updated<AdsTemplatesAdsElementTemplate>(adsTemplatesAdsElementTemplate.Id)
                     .Complete();
            }
        }

        public void AddAdvertisementsElementsFromTemplate(AdsTemplatesAdsElementTemplate adsTemplatesAdsElementTemplate)
        {
            var advertisements = _finder.Find<Advertisement>(x => x.AdvertisementTemplateId == adsTemplatesAdsElementTemplate.AdsTemplateId && !x.IsDeleted).ToArray();
            var dummyAdvertisementElement = _finder.Find<AdvertisementElement>(
                x => !x.IsDeleted && !x.Advertisement.IsDeleted && x.AdvertisementElementTemplateId == adsTemplatesAdsElementTemplate.AdsElementTemplateId && x.Advertisement.FirmId == null)
                                                   .FirstOrDefault();

            // TODO {all, 04.09.2013}: В процессе рефакторинга перевести на операцию c bulkcreateidentity + использовать отложенное сохранение
            // done {i.maslennikov. 25.09.2013}: done
            using (var operationScope = _scopeFactory.CreateSpecificFor<CreateIdentity, AdvertisementElement>())
            {
                foreach (var advertisement in advertisements)
                {
                    var advertisementElement = new AdvertisementElement
                        {
                            AdvertisementId = advertisement.Id,
                            AdvertisementElementTemplateId = adsTemplatesAdsElementTemplate.AdsElementTemplateId,
                            AdsTemplatesAdsElementTemplatesId = adsTemplatesAdsElementTemplate.Id,
                        };

                    if (advertisement.FirmId == null && dummyAdvertisementElement != null)
                    {
                        advertisementElement.FileId = dummyAdvertisementElement.FileId;
                        advertisementElement.Text = dummyAdvertisementElement.Text;
                        advertisementElement.FasCommentType = dummyAdvertisementElement.FasCommentType;
                        advertisementElement.BeginDate = dummyAdvertisementElement.BeginDate;
                        advertisementElement.EndDate = dummyAdvertisementElement.EndDate;
                    }

                    _identityProvider.SetFor(advertisementElement);

                    _advertisementElementGenericRepository.Add(advertisementElement);
                    operationScope.Added<AdvertisementElement>(advertisementElement.Id);
                }

                _advertisementElementGenericRepository.Save();
                operationScope.Complete();
            }
        }

        public void CreateOrUpdate(Advertisement advertisement)
        {
            var notUniqueName = _finder.Find<Advertisement>(x => x.Id != advertisement.Id &&
                                                                 !x.IsDeleted &&
                                                                 x.FirmId == advertisement.FirmId &&
                                                                 x.Name == advertisement.Name)
                .Any();
            if (notUniqueName)
            {
                throw new NotificationException(string.Format(BLResources.AdsCheckNameMustBeUnique, advertisement.Name));
            }

            // проверка, что нет другого рекламного материала в белый список для данной фирмы
            if (advertisement.IsSelectedToWhiteList)
            {
                var otherWhiteListedAdName = _finder.Find<Firm>(x => x.Id == advertisement.FirmId)
                    .SelectMany(x => x.Advertisements)
                    .Where(x => !x.IsDeleted && x.Id != advertisement.Id && x.IsSelectedToWhiteList)
                        .Select(x => x.Name)
                        .FirstOrDefault();
                if (otherWhiteListedAdName != null)
                {
                    throw new NotificationException(string.Format(BLResources.AdsCheckOnlyOneSelectedToWhiteListAllowed, otherWhiteListedAdName));
                }
            }

            using (var scope = _scopeFactory.CreateOrUpdateOperationFor(advertisement))
            {
                if (advertisement.IsNew())
                {
                    _identityProvider.SetFor(advertisement);
                    _advertisementGenericRepository.Add(advertisement);
                    scope.Added<Advertisement>(advertisement.Id);
                }
                else
                {
                    _advertisementGenericRepository.Update(advertisement);
                    scope.Updated<Advertisement>(advertisement.Id);
                }

                _advertisementGenericRepository.Save();

                scope.Complete();
            }
        }

        public void AddAdvertisementsElementsFromAdvertisement(Advertisement advertisement)
        {
            // создаём дочерние элементы РМ по шаблону
            var adsTemplatesAdsElementTemplateDtos =
                _finder.Find(Specs.Find.ById<AdvertisementTemplate>(advertisement.AdvertisementTemplateId))
                       .SelectMany(x => x.AdsTemplatesAdsElementTemplates)
                       .Where(x => !x.IsDeleted)
                       .Select(x => new
                           {
                               AdsTemplatesAdsElementTemplate = x,
                               IsFasComment = x.AdvertisementElementTemplate.RestrictionType == (int)AdvertisementElementRestrictionType.FasComment,
                               NeedsValidation = x.AdvertisementElementTemplate.NeedsValidation,
                               IsRequired = x.AdvertisementElementTemplate.IsRequired,
                           })
                       .ToArray();

            foreach (var adsTemplatesAdsElementTemplateDto in adsTemplatesAdsElementTemplateDtos)
            {
                var adsTemplatesAdsElementTemplate = adsTemplatesAdsElementTemplateDto.AdsTemplatesAdsElementTemplate;

                var advertisementElement = new AdvertisementElement
                    {
                        AdvertisementId = advertisement.Id,
                        AdvertisementElementTemplateId = adsTemplatesAdsElementTemplate.AdsElementTemplateId,
                        OwnerCode = advertisement.OwnerCode,
                        AdsTemplatesAdsElementTemplatesId = adsTemplatesAdsElementTemplate.Id,

                        // необязательный для заполнения и при этом требующий валидацию элемент по умолчанию (пока его никто не редактировал) валиден
                        Status = (int)(!adsTemplatesAdsElementTemplateDto.IsRequired && adsTemplatesAdsElementTemplateDto.NeedsValidation ? AdvertisementElementStatus.Valid : AdvertisementElementStatus.NotValidated),
                    };

                // TODO: косяк что значение по умолчанию это 6, из-за этого его теперь надо excplicitly проставлять, должен быть 0
                if (adsTemplatesAdsElementTemplateDto.IsFasComment)
                {
                    advertisementElement.FasCommentType = (int)FasComment.NewFasComment;
                }

                _identityProvider.SetFor(advertisementElement);

                // TODO {all, 04.09.2013}: В процессе рефакторинга перевести на операцию c bulkcreateidentity + использовать отложенное сохранение
                using (var operationScope = _scopeFactory.CreateSpecificFor<CreateIdentity, AdvertisementElement>())
                {
                    _advertisementElementGenericRepository.Add(advertisementElement);
                    _advertisementElementGenericRepository.Save();
                    operationScope.Added<AdvertisementElement>(advertisementElement.Id)
                                  .Complete();
                }
            }
        }

        public void Publish(long advertisementTemplateId)
        {
            var isDummyValuesUnfilled = _finder.Find(AdvertisementSpecs.AdvertisementElements.Find.UnfilledDummyValuesForTemplate(advertisementTemplateId)).Any();
            if (isDummyValuesUnfilled)
            {
                throw new BusinessLogicException(BLResources.YouMustFillInAllDummyValuesToPublishAdvertisementTemplate);
            }

            var advertisementTemplate = _finder.Find(Specs.Find.ById<AdvertisementTemplate>(advertisementTemplateId))
                                               .Select(at => new
                                                   {
                                                       AdvertisementTemplate = at,
                                                       HasElements = at.AdsTemplatesAdsElementTemplates.Any(ataet => !ataet.IsDeleted)
                                                   })
                                               .SingleOrDefault();

            if (advertisementTemplate == null)
            {
                throw new ArgumentException(BLResources.EntityNotFound, "advertisementTemplateId");
            }

            if (!advertisementTemplate.HasElements)
            {
                throw new NotificationException(BLResources.CannotPublishAdvertisementTemplateWithNoElements);
            }

            // TODO {all, 04.09.2013}: пока используем updateidentity, хотя, фактически, это отдельная publish 
            using (var scope = _scopeFactory.CreateSpecificFor<UpdateIdentity, AdvertisementTemplate>())
            {
                advertisementTemplate.AdvertisementTemplate.IsPublished = true;
                _advertisementTemplateGenericRepository.Update(advertisementTemplate.AdvertisementTemplate);
                _advertisementTemplateGenericRepository.Save();

                scope.Updated<AdvertisementTemplate>(advertisementTemplate.AdvertisementTemplate.Id)
                     .Complete();
            }
        }

        public void Unpublish(long advertisementTemplateId)
        {
            var advertisementTemplate = _finder.Find(Specs.Find.ById<AdvertisementTemplate>(advertisementTemplateId)).SingleOrDefault();
            if (advertisementTemplate == null)
            {
                return;
            }

            // TODO {all, 04.09.2013}: пока используем updateidentity, хотя, фактически, это отдельная unpublish 
            using (var scope = _scopeFactory.CreateSpecificFor<UpdateIdentity, AdvertisementTemplate>())
            {
                advertisementTemplate.IsPublished = false;
                _advertisementTemplateGenericRepository.Update(advertisementTemplate);
                _advertisementTemplateGenericRepository.Save();

                scope.Updated<AdvertisementTemplate>(advertisementTemplate.Id)
                     .Complete();
            }
        }

        public void SelectToWhiteList(long firmId, long advertisementId)
        {
            var dto = _finder.Find<Firm>(x => x.Id == firmId).Select(x => new
            {
                Advertisements = x.Advertisements.Where(y => !y.IsDeleted),
            }).Select(x => new
            {
                AdvertisementToSelect = x.Advertisements.FirstOrDefault(y => y.Id == advertisementId),
                AdvertisementsToDeselect = x.Advertisements.Where(y => y.Id != advertisementId && y.IsSelectedToWhiteList),
            })
            .Single();

            // TODO {all, 04.09.2013}: при рефакторинге нужно объединить функционал из handler и данный метод, так чтобы логировать операцию Selecttowhitelist, а не update, но с указанием всех изменяемых рекламных материалов
            // comment {i.maslennikov, 25.09.2013}: разве текущая иделогия не подразумевает бизнес-операцию как корень и элементарные дочерние?
            using (var scope = _scopeFactory.CreateSpecificFor<UpdateIdentity, Advertisement>())
            {
                if (dto.AdvertisementToSelect == null)
                {
                    throw new NotificationException(BLResources.SelectedAdvertisementDoesNotBelongToFirm);
                }

                dto.AdvertisementToSelect.IsSelectedToWhiteList = true;
                _advertisementGenericRepository.Update(dto.AdvertisementToSelect);
                scope.Updated<Advertisement>(dto.AdvertisementToSelect.Id);

                // set all selected as not selected
                foreach (var advertisementToDeselect in dto.AdvertisementsToDeselect)
                {
                    advertisementToDeselect.IsSelectedToWhiteList = false;
                    _advertisementGenericRepository.Update(advertisementToDeselect);
                    scope.Updated<Advertisement>(advertisementToDeselect.Id);
                }

                _advertisementGenericRepository.Save();
                scope.Complete();
            }
        }

        public AdvertisementBagItem[] GetAdvertisementBag(long advertisementId)
        {
            var advertisementBag = _finder.Find<Advertisement>(x => x.Id == advertisementId)
            .SelectMany(x => x.AdvertisementElements)
            .Where(x => x.IsDeleted == x.Advertisement.IsDeleted)
            .Select(x => new
            {
                Element = x,
                Temaplte = x.AdvertisementElementTemplate,
            })
            .Select(x => new
            {
                x.Element,
                x.Temaplte,

                        ValidText = (x.Temaplte.RestrictionType == (int)AdvertisementElementRestrictionType.Text ||
                                     x.Temaplte.RestrictionType == (int)AdvertisementElementRestrictionType.FasComment) &&
                            !string.IsNullOrEmpty(x.Element.Text),

                        ValidDate = x.Temaplte.RestrictionType == (int)AdvertisementElementRestrictionType.Date &&
                                    x.Element.BeginDate != null &&
                                    x.Element.EndDate != null,

                        ValidFile = (x.Temaplte.RestrictionType == (int)AdvertisementElementRestrictionType.Image ||
                                     x.Temaplte.RestrictionType == (int)AdvertisementElementRestrictionType.Article) &&
                            x.Element.FileId != null,
            })
            .Select(x => new AdvertisementBagItem
            {
                Id = x.Element.Id,
                Name = x.Temaplte.Name,

                Text = x.Element.Text,
                FileId = x.Element.FileId,
                FileName = x.Element.File.FileName,
                BeginDate = x.Element.BeginDate,
                EndDate = x.Element.EndDate,

                FormattedText = x.Temaplte.FormattedText,
                RestrictionType = (AdvertisementElementRestrictionType)x.Temaplte.RestrictionType,

                IsValid = !x.Temaplte.IsRequired || x.ValidText || x.ValidDate || x.ValidFile,

                NeedsValidation = x.Temaplte.NeedsValidation,
                Status = (AdvertisementElementStatus)x.Element.Status
                    })
                .ToArray();

            return advertisementBag;
        }

        public AdvertisementTemplateIdNameDto GetAdvertisementTemplate(long advertisementId)
        {
            return _finder.Find<AdvertisementTemplate>(item => item.Id == advertisementId)
                                                                .Select(item => new AdvertisementTemplateIdNameDto
                                                                    {
                                                                        Id = item.Id,
                                                                        Name = item.Name,
                                                                        IsPublished = item.IsPublished
                                                                    })
                                                                .Single();
        }

        int IDeleteAggregateRepository<AdvertisementTemplate>.Delete(long entityId)
        {
            var entity = _secureFinder.Find(Specs.Find.ById<AdvertisementTemplate>(entityId)).Single();
            return Delete(entity);
        }

        int IDeleteAggregateRepository<Advertisement>.Delete(long entityId)
        {
            var entity = _finder.Find(Specs.Find.ById<Advertisement>(entityId)).Single();
            return Delete(entity);
        }

        int IDeleteAggregateRepository<AdvertisementElement>.Delete(long entityId)
        {
            var entity = _finder.Find(Specs.Find.ById<AdvertisementElement>(entityId)).Single();
            return Delete(entity);
        }

        int IDeleteAggregateRepository<AdsTemplatesAdsElementTemplate>.Delete(long entityId)
        {
            var entity = _secureFinder.Find(Specs.Find.ById<AdsTemplatesAdsElementTemplate>(entityId)).Single();
            return Delete(entity);
        }

        int IDeleteAggregateRepository<AdvertisementElementTemplate>.Delete(long entityId)
        {
            var entity = _secureFinder.Find(Specs.Find.ById<AdvertisementElementTemplate>(entityId)).Single();
            return Delete(entity);
        }

        public StreamResponse DownloadFile(DownloadFileParams<AdvertisementElement> downloadFileParams)
        {
            var file = _fileContentFinder.Find(Specs.Find.ById<FileWithContent>(downloadFileParams.FileId)).Single();
            return new StreamResponse { FileName = file.FileName, ContentType = file.ContentType, Stream = file.Content };
        }
    }
}
