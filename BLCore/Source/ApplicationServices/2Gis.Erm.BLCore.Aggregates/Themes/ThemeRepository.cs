using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Themes;
using DoubleGis.Erm.BLCore.API.Aggregates.Themes.DTO;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Operations.Generic.File;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Common.Compression;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Model.Common.Operations.Identity.Generic;

using File = DoubleGis.Erm.Platform.Model.Entities.Erm.File;

namespace DoubleGis.Erm.BLCore.Aggregates.Themes
{
    public sealed class ThemeRepository : IThemeRepository
    {
        private const int MaxThemesPerOrganizationUnit = 10;
        private const int MaxSkyscraperThemesPerOrganizationUnit = 1;
        private const int MaxDefaultThemesPerOrganizationUnit = 1;

        private readonly IFinder _finder;
        private readonly IFileContentFinder _fileContentFinder;
        private readonly IRepository<Theme> _themeRepository;
        private readonly IRepository<ThemeTemplate> _themeTemplateRepository;
        private readonly IRepository<ThemeCategory> _themeCategoryRepository;
        private readonly IRepository<ThemeOrganizationUnit> _themeOrganizationUnitRepository;
        private readonly IRepository<FileWithContent> _fileRepository;
        private readonly IConcurrentPeriodCounter _periodCounter;
        private readonly IUserContext _userContext;
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _scopeFactory;

        public ThemeRepository(
            IFinder finder,
            IFileContentFinder fileContentFinder,
            IRepository<Theme> themeRepository,
            IRepository<ThemeTemplate> themeTemplateRepository,
            IRepository<ThemeCategory> themeCategoryRepository,
            IRepository<ThemeOrganizationUnit> themeOrganizationUnitRepository,
            IRepository<FileWithContent> fileRepository,
            IConcurrentPeriodCounter periodCounter,
            IUserContext userContext,
            IIdentityProvider identityProvider,
            IOperationScopeFactory scopeFactory)
        {
            _finder = finder;
            _fileContentFinder = fileContentFinder;
            _themeRepository = themeRepository;
            _themeTemplateRepository = themeTemplateRepository;
            _themeCategoryRepository = themeCategoryRepository;
            _themeOrganizationUnitRepository = themeOrganizationUnitRepository;
            _fileRepository = fileRepository;
            _periodCounter = periodCounter;
            _userContext = userContext;
            _identityProvider = identityProvider;
            _scopeFactory = scopeFactory;
        }

        int IDeleteAggregateRepository<Theme>.Delete(long entityId)
        {
            using (var scope = _scopeFactory.CreateSpecificFor<DeleteIdentity, Theme>())
            {
                if (IsThemeUsedInOrders(entityId))
                {
                    throw new ArgumentException(BLResources.CannotDeleteUsedTheme);
                }

                var themeOrganizationUnits = _finder.Find<ThemeOrganizationUnit>(unit => unit.ThemeId == entityId).ToArray();
                foreach (var themeOrganizationUnit in themeOrganizationUnits)
                {
                    themeOrganizationUnit.IsActive = false;
                    _themeOrganizationUnitRepository.Update(themeOrganizationUnit);
                    scope.Updated<ThemeOrganizationUnit>(themeOrganizationUnit.Id);
                }

                var theme = _finder.Find(Specs.Find.ById<Theme>(entityId)).Single();
                _themeRepository.Delete(theme);
                scope.Deleted<Theme>(theme.Id);

                var cnt = _themeRepository.Save() +
                          _themeOrganizationUnitRepository.Save() +
                          _themeCategoryRepository.Save();

                scope.Complete();
                return cnt;
            }
        }

        int IDeleteAggregateRepository<ThemeTemplate>.Delete(long entityId)
        {
            if (IsTemplateUsedInThemes(entityId))
            {
                throw new ArgumentException(BLResources.CannotDeleteUsedThemeTemplate);
            }

            var themeTemplate = _finder.Find(Specs.Find.ById<ThemeTemplate>(entityId)).Single();
            using (var scope = _scopeFactory.CreateSpecificFor<DeleteIdentity, ThemeTemplate>())
            {
                _themeTemplateRepository.Delete(themeTemplate);
                var cnt = _themeTemplateRepository.Save();
                scope.Deleted<ThemeTemplate>(themeTemplate.Id).Complete();
                return cnt;
            }
        }

        int IDeleteAggregateRepository<ThemeCategory>.Delete(long entityId)
        {
            var themeCategory = _finder.Find(Specs.Find.ById<ThemeCategory>(entityId)).Single();
            if (themeCategory.IsDeleted)
            {
                return 0;
            }

            using (var scope = _scopeFactory.CreateSpecificFor<DeleteIdentity, ThemeCategory>())
            {
                _themeCategoryRepository.Delete(themeCategory);
                var cnt = _themeCategoryRepository.Save();
                scope.Deleted<ThemeCategory>(themeCategory.Id).Complete();
                return cnt;
            }
        }

        int IDeleteAggregateRepository<ThemeOrganizationUnit>.Delete(long entityId)
        {
            var themeOrganizationUnit = _finder.Find(Specs.Find.ById<ThemeOrganizationUnit>(entityId)).Single();
            if (themeOrganizationUnit.IsDeleted)
            {
                return 0;
            }

            var theme = _finder.Find(Specs.Find.ById<Theme>(themeOrganizationUnit.ThemeId)).Single();

            // Если тематика установлена по умолчанию - редактирование отделений организации НЕ допустимо
            if (theme.IsDefault)
            {
                throw new ArgumentException(BLResources.CannotEditThemeOrganizationUnitsInDefaultTheme);
            }

            // [ERM-4573] Не должны давать удалять отделения организаций, которые являются городом назначения в активном заказе по данной тематике.
            var orderWithSameDestOrgUnit = FindFirstOrderWithSameDestOrgUnitAndTheme(themeOrganizationUnit.ThemeId, themeOrganizationUnit.OrganizationUnitId);
            if (orderWithSameDestOrgUnit != null)
            {
                throw new ArgumentException(string.Format(BLResources.CannotDeleteOrganizationUnitsUsedInOrderByThatTheme, orderWithSameDestOrgUnit.Number));
            }

            using (var scope = _scopeFactory.CreateSpecificFor<DeleteIdentity, ThemeOrganizationUnit>())
            {
                _themeOrganizationUnitRepository.Delete(themeOrganizationUnit);
                var cnt = _themeOrganizationUnitRepository.Save();
                scope.Deleted<ThemeOrganizationUnit>(themeOrganizationUnit.Id).Complete();
                return cnt;
            }
        }

        public Theme FindTheme(long themeId)
        {
            return _finder.Find(Specs.Find.ById<Theme>(themeId)).SingleOrDefault();
        }

        public ThemeTemplate FindThemeTemplate(long templateId)
        {
            return _finder.Find(Specs.Find.ById<ThemeTemplate>(templateId)).SingleOrDefault();
        }

        public ThemeTemplate FindThemeTemplateByThemeId(long themeId)
        {
            return _finder.Find(Specs.Find.ById<Theme>(themeId)).Select(x => x.ThemeTemplate).SingleOrDefault();
        }

        public ThemeTemplate FindThemeTemplateByThemplateCode(long templateCode)
        {
            return _finder.Find(Specs.Find.ActiveAndNotDeleted<ThemeTemplate>())
                .SingleOrDefault(template => template.TemplateCode == templateCode);
        }

        public File GetThemeTemplateFile(long templateId)
        {
            return _finder.Find(Specs.Find.ById<ThemeTemplate>(templateId))
                .Select(template => template.File)
                .SingleOrDefault();
        }

        public File GetThemeFile(long themeId)
        {
            return _finder.Find(Specs.Find.ById<Theme>(themeId))
                .Select(theme => theme.File)
                .SingleOrDefault();
        }

        public void CreateOrUpdate(ThemeTemplate template)
        {
            if (template.IsSkyScraper &&
                _finder.Find<ThemeTemplate>(
                    themeTemplate =>
                    themeTemplate.IsActive
                    && !themeTemplate.IsDeleted
                    && themeTemplate.IsSkyScraper
                    && themeTemplate.Id != template.Id).Any())
            {
                throw new ArgumentException(BLResources.ThemeTemplateCannotBeSkyscapper);
            }

            using (var scope = _scopeFactory.CreateOrUpdateOperationFor(template))
            {
                if (template.IsNew())
                {
                    _identityProvider.SetFor(template);
                    _themeTemplateRepository.Add(template);
                    scope.Added<ThemeTemplate>(template.Id);
                }
                else
                {
                    _themeTemplateRepository.Update(template);
                    scope.Updated<ThemeTemplate>(template.Id);
                }

                _themeTemplateRepository.Save();
                scope.Complete();
            }
        }

        public void CreateOrUpdate(Theme theme)
        {
            if (theme.IsDefault && !CanThemeBeDefault(theme.Id))
            {
                throw new ArgumentException(BLResources.CannotSetThemeAsDefault);
            }

            if (theme.BeginDistribution > theme.EndDistribution)
            {
                throw new ArgumentException(BLResources.EndDistributionDateMustBeGreaterOrEqualThanBeginDistributionDate);
            }

            // Контроллируем, что после правки не нарушится условие по числу тематик по отделению оргранизации
            var units = _finder.Find(Specs.Find.ActiveAndNotDeleted<ThemeOrganizationUnit>())
                               .Where(link => link.ThemeId == theme.Id)
                               .Select(link => new { link.OrganizationUnit.Id, link.OrganizationUnit.Name })
                               .ToArray();

            foreach (var unit in units.Where(unit => IsThemeLimitReachedInOrganizationUnit(theme, unit.Id)))
            {
                var message = string.Format(BLResources.CannotSaveThemeOrganizationUnitThemeLimitReached, unit.Name);
                throw new ArgumentException(message);
            }

            if (!theme.IsNew() && IsThemeUsedInOrders(theme.Id))
            {
                var period = GetMinimalThemeDistributionPeriod(theme.Id);
                if (period.Start < theme.BeginDistribution)
                {
                    throw new ArgumentException(string.Format(BLResources.CannotChangeThemePeriodStart, period.Start.ToShortDateString()));
                }

                if (period.End > theme.EndDistribution)
                {
                    throw new ArgumentException(string.Format(BLResources.CannotChangeThemePeriodEnd, period.End.ToShortDateString()));
                }
            }


            using (var scope = _scopeFactory.CreateOrUpdateOperationFor(theme))
            {
                if (theme.IsNew())
                {
                    // Сейчас тематика входит в число сущностей, для которых идентификаторы вводятся вручную, а шаблон - нет.
                    // Но теперь я не уверен, что это было правильное ррешение: было бы логичным, чтоб у них был одинаковый подход.
                    // ... По прошествии ещё некоторого времни я понял, что есть решение лучше: у шаблонов поле TemplateCode должно быть идентификатором.
                    _themeRepository.Add(theme);
                    scope.Added<Theme>(theme.Id);
                }
                else
                {
                    _themeRepository.Update(theme);
                    scope.Updated<Theme>(theme.Id);
                }

                _themeRepository.Save();
                scope.Complete();
            }
        }

        public bool IsThemeAppendedToOrganizationUnit(long themeId, long organizationUnitId)
        {
            var linkExists = _finder.Find(Specs.Find.ActiveAndNotDeleted<ThemeOrganizationUnit>())
                                    .Any(link => link.OrganizationUnitId == organizationUnitId && link.ThemeId == themeId);
            return linkExists;
        }

        public ThemeOrganizationUnit AppendThemeToOrganizationUnit(long themeId, long organizationUnitId)
        {
            var themeOrganizationUnit = _finder.Find<ThemeOrganizationUnit>(themeCategory => themeCategory.OrganizationUnitId == organizationUnitId && themeCategory.ThemeId == themeId)
                              .SingleOrDefault();

            if (themeOrganizationUnit == null)
            {
                using (var scope = _scopeFactory.CreateSpecificFor<CreateIdentity, ThemeOrganizationUnit>())
                {
                    themeOrganizationUnit = new ThemeOrganizationUnit
                        {
                            ThemeId = themeId,
                            OrganizationUnitId = organizationUnitId,
                            IsActive = true,
                        };
                    _identityProvider.SetFor(themeOrganizationUnit);
                    _themeOrganizationUnitRepository.Add(themeOrganizationUnit);
                    _themeOrganizationUnitRepository.Save();
                    scope.Added<ThemeOrganizationUnit>(themeOrganizationUnit.Id);
                    scope.Complete();
                }
            }
            else if (themeOrganizationUnit.IsDeleted)
            {
                using (var scope = _scopeFactory.CreateSpecificFor<UpdateIdentity, ThemeOrganizationUnit>())
                {
                    themeOrganizationUnit.IsDeleted = false;
                    themeOrganizationUnit.IsActive = true;
                    _themeOrganizationUnitRepository.Update(themeOrganizationUnit);
                    _themeOrganizationUnitRepository.Save();
                    scope.Updated<ThemeOrganizationUnit>(themeOrganizationUnit.Id);
                    scope.Complete();
                }
            }

            return themeOrganizationUnit;
        }

        public ThemeCategory AppendThemeToCategory(long themeId, long categoryId)
        {
            var themeCategory = _finder.Find<ThemeCategory>(x => x.CategoryId == categoryId && x.ThemeId == themeId)
                              .SingleOrDefault();

            if (themeCategory == null)
            {
                using (var scope = _scopeFactory.CreateSpecificFor<CreateIdentity, ThemeCategory>())
                {
                    themeCategory = new ThemeCategory { ThemeId = themeId, CategoryId = categoryId };
                    _identityProvider.SetFor(themeCategory);
                    _themeCategoryRepository.Add(themeCategory);
                    _themeCategoryRepository.Save();
                    scope.Added<ThemeCategory>(themeCategory.Id);
                    scope.Complete();
                }
            }
            else if (themeCategory.IsDeleted)
            {
                using (var scope = _scopeFactory.CreateSpecificFor<UpdateIdentity, ThemeCategory>())
                {
                    themeCategory.IsDeleted = false;
                    _themeCategoryRepository.Update(themeCategory);
                    _themeCategoryRepository.Save();
                    scope.Updated<ThemeCategory>(themeCategory.Id);
                    scope.Complete();
                }
            }

            return themeCategory;
        }

        public int CountThemeCategories(long themeId)
        {
            return _finder.Find<ThemeCategory>(link => !link.IsDeleted && link.ThemeId == themeId)
                .Select(link => link.Theme)
                .Where(Specs.Find.ActiveAndNotDeleted<Theme>())
                .Count();
        }

        public bool IsThemeLimitReachedInOrganizationUnit(Theme theme, long organizationUnitId)
        {
            var isSkyScraper = _finder.Find(Specs.Find.ById<ThemeTemplate>(theme.ThemeTemplateId)).Select(themeTemplate => themeTemplate.IsSkyScraper).Single();
            var isDefault = theme.IsDefault;
            FindSpecification<Theme> themeKindSpecification;
            int entityLimit;

            if (isSkyScraper)
            {
                themeKindSpecification = ThemeSpecifications.Find.SkyScrapper();
                entityLimit = MaxSkyscraperThemesPerOrganizationUnit;
            }
            else if (isDefault)
            {
                themeKindSpecification = ThemeSpecifications.Find.Default();
                entityLimit = MaxDefaultThemesPerOrganizationUnit;
            }
            else
            {
                themeKindSpecification = ThemeSpecifications.Find.NotSkyScrapperAndNotDefault();
                entityLimit = MaxThemesPerOrganizationUnit;
            }

            return CountOrganizationUnitThemes(organizationUnitId, theme.BeginDistribution, theme.EndDistribution, themeKindSpecification, theme.Id) >= entityLimit;
        }

        public bool IsTemplateUsedInThemes(long templateId)
        {
            return _finder.Find(Specs.Find.ActiveAndNotDeleted<Theme>())
                .Any(theme => theme.ThemeTemplateId == templateId);
        }

        public bool CanThemeBeDefault(long themeId)
        {
            var theme = FindTheme(themeId);
            if (theme == null)
            {
                return false;
            }

            var themeOrganizationUnits = _finder.Find(Specs.Find.ById<Theme>(themeId))
                                                .SelectMany(th => th.ThemeOrganizationUnits)
                                                .Where(Specs.Find.ActiveAndNotDeleted<ThemeOrganizationUnit>())
                                                .Select(unit => unit.OrganizationUnitId)
                                                .ToArray();

            // Тематика без указанных подразделений не может быть тематикой по умолчанию
            if (themeOrganizationUnits.Length == 0)
            {
                return false;
            }

            var anyConflictExist = _finder.Find(Specs.Find.ActiveAndNotDeleted<ThemeOrganizationUnit>())
                                            .Where(unit => unit.Theme.Id != themeId &&
                                                           unit.Theme.IsDefault &&
                                                           unit.OrganizationUnit.IsActive && !unit.OrganizationUnit.IsDeleted &&
                                                           unit.Theme.IsActive && !unit.Theme.IsDeleted)
                                            .Where(unit => themeOrganizationUnits.Contains(unit.OrganizationUnitId))
                                            .Any(unit => unit.Theme.BeginDistribution < theme.EndDistribution &&
                                                         unit.Theme.EndDistribution > theme.BeginDistribution);

            return !anyConflictExist;
        }

        public bool IsThemeUsedInOrders(long themeId)
        {
            return _finder.Find<OrderPositionAdvertisement>(advertisement => advertisement.ThemeId == themeId &&
                                                                             advertisement.OrderPosition.IsActive &&
                                                                             !advertisement.OrderPosition.IsDeleted &&
                                                                             advertisement.OrderPosition.Order.IsActive &&
                                                                             !advertisement.OrderPosition.Order.IsDeleted)
                          .Any();
        }

        public Order FindFirstOrderWithSameDestOrgUnitAndTheme(long themeId, long organizationUnitId)
        {
            return _finder.Find<OrderPositionAdvertisement>(advertisement => advertisement.ThemeId == themeId &&
                                                                                advertisement.OrderPosition.IsActive &&
                                                                                !advertisement.OrderPosition.IsDeleted &&
                                                                                advertisement.OrderPosition.Order.IsActive &&
                                                                                !advertisement.OrderPosition.Order.IsDeleted &&
                                                                                advertisement.OrderPosition.Order.DestOrganizationUnitId == organizationUnitId)
                             .Select(a => a.OrderPosition.Order).FirstOrDefault();
        }

        public string GetOrganizationUnitName(long organizationUnitId)
        {
            return _finder.Find(Specs.Find.ById<OrganizationUnit>(organizationUnitId))
                          .Select(unit => unit.Name)
                          .Single();
        }

        public StreamResponse DownloadFile(DownloadFileParams<Theme> downloadFileParams)
        {
            var file = _fileContentFinder.Find(Specs.Find.ById<FileWithContent>(downloadFileParams.FileId)).Single();
            return new StreamResponse { FileName = file.FileName, ContentType = file.ContentType, Stream = file.Content };
        }

        public StreamResponse DownloadFile(DownloadFileParams<ThemeTemplate> downloadFileParams)
        {
            var file = _fileContentFinder.Find(Specs.Find.ById<FileWithContent>(downloadFileParams.FileId)).Single();
            return new StreamResponse { FileName = file.FileName, ContentType = file.ContentType, Stream = file.Content };
        }

        public string GetBase64EncodedFile(long fileId)
        {
            var file = _fileContentFinder.Find(Specs.Find.ById<FileWithContent>(fileId)).Single();
            using (var stream = new MemoryStream((int)file.Content.Length))
            {
                file.Content.CopyTo(stream);
                return Convert.ToBase64String(stream.ToArray());
            }
        }

        public IEnumerable<ThemeTemplateUsageDto> GetThemeUsage(long organizationUnit, TimePeriod period)
        {
            return _finder.Find(OrderSpecs.Orders.Find.ForRelease(organizationUnit, period) && Specs.Find.ActiveAndNotDeleted<Order>())
                   .SelectMany(order => order.OrderPositions.SelectMany(position => position.OrderPositionAdvertisements))
                   .Where(advertisement => advertisement.ThemeId != null)
                   .Select(advertisement => new { advertisement.Theme, advertisement.Theme.ThemeTemplate })
                   .GroupBy(arg => arg.ThemeTemplate)
                   .Select(grouping => new ThemeTemplateUsageDto
                       {
                           Id = grouping.Key.Id,
                           FileId = grouping.Key.FileId,

                           Themes = grouping.Select(theme => new ThemeUsageDto { Id = theme.Theme.Id, FileId = theme.Theme.FileId })
                       })
                   .ToArray();
        }

        public int CountThemeOrganizationUnits(long themeId)
        {
            return _finder.Find(Specs.Find.ById<Theme>(themeId))
                          .SelectMany(theme => theme.ThemeOrganizationUnits)
                          .Count(link => link.IsActive && !link.IsDeleted);
        }

        UploadFileResult IUploadFileAggregateRepository<Theme>.UploadFile(UploadFileParams<Theme> uploadFileParams)
        {
            return UploadFileInternal(_themeRepository, uploadFileParams);
        }

        UploadFileResult IUploadFileAggregateRepository<ThemeTemplate>.UploadFile(UploadFileParams<ThemeTemplate> uploadFileParams)
        {
            try
            {
                var containDirectories = uploadFileParams.Content.ZipStreamFindFiles(x => x.IsDirectory).Any();
                if (containDirectories)
                {
                    throw new BusinessLogicException(BLResources.InvalidArchiveContent);
                }
            }
            catch (ArgumentException ex)
            {
                // Если архиватор не смог прочитать структуру архива - это достаточный повод отказать в приёме этого файла.
                throw new BusinessLogicException(BLResources.InvalidArchiveFormat, ex);
            }

            return UploadFileInternal(_themeTemplateRepository, uploadFileParams);
        }

        private TimePeriod GetMinimalThemeDistributionPeriod(long themeId)
        {
            var orders = _finder.Find<OrderPositionAdvertisement>(advertisement => advertisement.ThemeId == themeId)
                .Select(advertisement => advertisement.OrderPosition)
                .Where(position => position.IsActive && !position.IsDeleted)
                .Select(position => position.Order)
                .Where(order => order.IsActive && !order.IsDeleted)
                .Select(order => new { order.BeginDistributionDate, order.EndDistributionDateFact })
                .ToArray();

            if (!orders.Any())
            {
                return new TimePeriod(DateTime.MaxValue, DateTime.MinValue);
            }

            return new TimePeriod(orders.Select(arg => arg.BeginDistributionDate).Min(), orders.Select(arg => arg.EndDistributionDateFact).Max());
        }

        private int CountOrganizationUnitThemes(long organizationUnitId, DateTime periodStart, DateTime periodEnd, FindSpecification<Theme> themeKindSpecification, long excludeTheme)
        {
            // Суть задачи получить не общее число тематик, относящихся к подразделению за период,
            // а получить их максимальное количество, действующее одновременно.
            // Т.е. если одня тематика была в январе, другая в феврале, а в марте не было
            // то за период с января по март в ответе должна быть единица.

            // Все тематики по умолчанию, действовавшие в требуемый период
            var themes = _finder.Find(Specs.Find.ActiveAndNotDeleted<ThemeOrganizationUnit>())
                                .Where(link => link.OrganizationUnitId == organizationUnitId && excludeTheme != link.ThemeId)
                                .Select(link => link.Theme)
                                .Where(ThemeSpecifications.Find.InPeriod(periodStart, periodEnd))
                                .Where(themeKindSpecification)
                                .Where(Specs.Find.ActiveAndNotDeleted<Theme>())
                                .Select(theme => new { theme.BeginDistribution, theme.EndDistribution })
                                .ToArray();

            return _periodCounter.Count(themes.Select(arg => new TimePeriod(arg.BeginDistribution, arg.EndDistribution)), periodStart, periodEnd);
        }

        private UploadFileResult UploadFileInternal<TEntity>(IRepository<TEntity> secureRepository, UploadFileParams uploadFileParams)
            where TEntity : class, IEntity, IEntityKey, IEntityFile, IAuditableEntity
        {
            if (uploadFileParams.Content != null && uploadFileParams.Content.Length > 10485760)
            {
                throw new BusinessLogicException(BLResources.FileSizeMustBeLessThan10MB);
            }

            var file = new FileWithContent
            {
                Id = uploadFileParams.FileId,
                ContentType = uploadFileParams.ContentType,
                ContentLength = uploadFileParams.ContentLength,
                Content = uploadFileParams.Content,
                FileName = Path.GetFileName(uploadFileParams.FileName)
            };

            using (var operationScope = _scopeFactory.CreateOrUpdateOperationFor(file))
            {
                if (file.IsNew())
                {
                    _identityProvider.SetFor(file);
                    _fileRepository.Add(file);
                    operationScope.Added<FileWithContent>(file.Id);
                }
                else
                {
                    _fileRepository.Update(file);
                    operationScope.Updated<FileWithContent>(file.Id);
                }

                var entity = _finder.Find(Specs.Find.ByFileId<TEntity>(uploadFileParams.FileId)).FirstOrDefault();
                if (entity != null)
                {
                    entity.ModifiedOn = DateTime.UtcNow;
                    entity.ModifiedBy = _userContext.Identity.Code;

                    secureRepository.Update(entity);
                    secureRepository.Save();
                    operationScope.Updated<TEntity>(entity.Id);
                }

                operationScope.Complete();
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
