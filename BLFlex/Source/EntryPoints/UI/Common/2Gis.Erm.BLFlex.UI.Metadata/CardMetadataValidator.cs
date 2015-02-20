using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml.Linq;

using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.API.Common.Metadata.Old.Dto;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Validators;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Card;

namespace DoubleGis.Erm.BLFlex.UI.Metadata
{
    public sealed partial class CardMetadataValidator : MetadataValidatorBase<MetadataCardsIdentity>
    {
        private const string EntitySettingsResourceEntry = "EntitySettings.xml";
        private const string EntitySettingsResourceEntryFormat = "EntitySettings.{0}.xml";

        private static readonly Dictionary<BusinessModel, Dictionary<EntityName, CardStructure>> CardSettings = ParseCardSettings();

        private readonly IGlobalizationSettings _globalizationSettings;
        private readonly ICardSettingsProvider _cardSettingsProvider;
        private readonly ICommonLog _commonLog;

        public CardMetadataValidator(
            IGlobalizationSettings globalizationSettings,
            ICardSettingsProvider cardSettingsProvider,
            IMetadataProvider metadataProvider, 
            ICommonLog commonLog)
            : base(metadataProvider)
        {
            _globalizationSettings = globalizationSettings;
            _cardSettingsProvider = cardSettingsProvider;
            _commonLog = commonLog;
        }

        protected override bool IsValidImpl(MetadataSet targetMetadata, out string report)
        {
            var errorsBuilder = new StringBuilder();
            foreach (var cardMetadata in targetMetadata.Metadata.Values.Cast<CardMetadata>())
            {
                var codedCardSettings = _cardSettingsProvider.GetCardSettings(cardMetadata.Entity, Thread.CurrentThread.CurrentUICulture);
                var xmlCardSettings = GetXmlCardSettings(cardMetadata.Entity);
                var errors = GetCardSettingsErrors(xmlCardSettings, codedCardSettings, cardMetadata.Entity);
                if (errors.Any())
                {
                    var errorMessage = string.Format("Для карточки {0} обнаружены следующие расхождения в метаданных: {1}",
                                                     cardMetadata.Entity,
                                                     string.Join(";" + Environment.NewLine, errors));

                    errorsBuilder.AppendLine(errorMessage);
                    _commonLog.Error(errorMessage);
                }

                // TODO {all, 19.01.2015}: Убрать эту проверку, когда найдется случай, что MainAttribute явно не нужен.
                if (!cardMetadata.Uses<MainAttributeFeature>())
                {
                    var errorMessage = string.Format("Для карточки {0} не задан основной атрибут",
                                                     cardMetadata.Entity);

                    errorsBuilder.AppendLine(errorMessage);
                    _commonLog.Error(errorMessage);
                }
            }

            report = errorsBuilder.ToString();
            return string.IsNullOrWhiteSpace(report);
        }

        private CardStructure GetXmlCardSettings(EntityName entityName)
        {
            var culture = Thread.CurrentThread.CurrentUICulture;
            var cardSettings = GetCardSettings(_globalizationSettings.BusinessModel, entityName);

            var localizedCardSettings = new CardStructure
            {
                Icon = cardSettings.Icon,
                Title = GetLocalizedName(cardSettings.TitleResourceId, culture),
                EntityName = cardSettings.EntityName,
                EntityLocalizedName = GetLocalizedName(cardSettings.EntityNameLocaleResourceId, culture),
                EntityMainAttribute = cardSettings.EntityMainAttribute,
                HasComments = cardSettings.HasComments,
                HasAdminTab = cardSettings.HasAdminTab,
                DecimalDigits = cardSettings.DecimalDigits,

                CardToolbar = cardSettings.CardToolbar.Select(x => new ToolbarElementStructure
                {
                    Name = x.Name,
                    ParentName = x.ParentName,
                    HideInCardRelatedGrid = x.HideInCardRelatedGrid,
                    ControlType = x.ControlType,
                    Action = x.Action,
                    Icon = x.Icon,
                    Disabled = x.Disabled,
                    DisableOnEmpty = x.DisableOnEmpty,
                    LocalizedName = GetLocalizedName(x.NameLocaleResourceId ?? x.Name, culture),
                    SecurityPrivelege = x.SecurityPrivelege,
                    NameLocaleResourceId = x.NameLocaleResourceId,
                    LockOnInactive = x.LockOnInactive,
                    LockOnNew = x.LockOnNew,
                }).ToArray(),

                CardRelatedItems = cardSettings.CardRelatedItems.Select(x => new CardRelatedItemsGroupStructure
                {
                    Name = x.Name,
                    LocalizedName = GetLocalizedName(x.NameLocaleResourceId ?? x.Name, culture),
                    NameLocaleResourceId = x.NameLocaleResourceId,
                    Items = x.Items.Select(y => new CardRelatedItemStructure
                    {
                        Name = y.Name,
                        NameLocaleResourceId = y.NameLocaleResourceId,
                        DisabledExpression = y.DisabledExpression,
                        LocalizedName = GetLocalizedName(y.NameLocaleResourceId ?? y.Name, culture),
                        Icon = y.Icon,
                        RequestUrl = y.RequestUrl,
                        ExtendedInfo = y.ExtendedInfo,
                        AppendableEntity = y.AppendableEntity,
                    }).ToArray(),
                }).ToArray(),

                TitleResourceId = cardSettings.TitleResourceId,
                EntityNameLocaleResourceId = cardSettings.EntityNameLocaleResourceId,
            };

            return localizedCardSettings;
        }

        private CardStructure GetCardSettings(BusinessModel adaptation, EntityName entityName)
        {
            Dictionary<EntityName, CardStructure> container;
            if (!CardSettings.TryGetValue(adaptation, out container))
            {
                throw new ArgumentException("Cannot find metadata for adaptation " + adaptation);
            }

            CardStructure xmlCardJson;
            if (!container.TryGetValue(entityName, out xmlCardJson))
            {
                throw new ArgumentException("Cannot find metadata for entity");
            }

            return xmlCardJson;
        }

        private static string GetLocalizedName(string resourceId, CultureInfo culture)
        {
            if (resourceId == null)
            {
                return null;
            }

            return ErmConfigLocalization.ResourceManager.GetString(resourceId, culture)
                ?? MetadataResources.ResourceManager.GetString(resourceId, culture)
                ?? resourceId;
        }

        private static Dictionary<BusinessModel, Dictionary<EntityName, CardStructure>> ParseCardSettings()
        {
            var result = new Dictionary<BusinessModel, Dictionary<EntityName, CardStructure>>();

            foreach (BusinessModel val in Enum.GetValues(typeof(BusinessModel)))
            {
                var stringResourceName = string.Format(EntitySettingsResourceEntryFormat, Enum.GetName(typeof(BusinessModel), val));
                var container = XDocument.Parse(GetResourceEntryContent(stringResourceName, EntitySettingsResourceEntry)).Root;
                var cardSettings = ParseCardSettings(container);
                result.Add(val, cardSettings);
            }

            return result;
        }

        private static Dictionary<EntityName, CardStructure> ParseCardSettings(XContainer container)
        {
            var dictionary = new Dictionary<EntityName, CardStructure>();

            foreach (var entityEl in container.Elements("Entity"))
            {
                var entityNameNonParsed = (string)entityEl.Attribute("Name");

                EntityName entityName;
                if (!Enum.TryParse(entityNameNonParsed, out entityName))
                {
                    throw new ArgumentException("Unrecognized entity type");
                }

                var cardDto = ParseCardDto(entityEl, entityName);
                if (cardDto != null)
                {
                    dictionary.Add(entityName, cardDto);
                }
            }

            return dictionary;
        }

        private static CardStructure ParseCardDto(XContainer entityEl, EntityName entityName)
        {
            var cardEl = entityEl.Element("Card");
            if (cardEl == null)
            {
                return null;
            }

            var cardJson = new CardStructure { EntityName = entityName.ToString() };

            // Сейчас нигде не задается
            var cardNameLocaleResourceId = cardEl.Attribute("CardNameLocaleResourceId");
            if (cardNameLocaleResourceId != null)
            {
                cardJson.TitleResourceId = cardNameLocaleResourceId.Value;
            }

            var entityNameLocaleResourceId = cardEl.Attribute("EntityNameLocaleResourceId");
            if (entityNameLocaleResourceId != null)
            {
                cardJson.EntityNameLocaleResourceId = entityNameLocaleResourceId.Value;
            }

            var entityMainAttribute = cardEl.Attribute("EntityMainAttribute");
            if (entityMainAttribute != null)
            {
                cardJson.EntityMainAttribute = entityMainAttribute.Value;
            }

            var hasComments = cardEl.Attribute("HasComments");
            if (hasComments != null)
            {
                cardJson.HasComments = (bool)hasComments;
            }

            var hasAdminTab = cardEl.Attribute("HasAdminTab");
            if (hasAdminTab != null)
            {
                cardJson.HasAdminTab = (bool)hasAdminTab;
            }

            var decimalDigits = cardEl.Attribute("DecimalDigits");
            if (decimalDigits != null)
            {
                cardJson.DecimalDigits = (int)decimalDigits;
            }

            var icon = cardEl.Attribute("LargeIcon");
            if (icon != null)
            {
                cardJson.Icon = icon.Value;
            }

            cardJson.CardToolbar = ParseToolbarItems(cardEl);
            if (cardJson.CardToolbar == null)
            {
                cardJson.CardToolbar = new ToolbarElementStructure[0];
            }
            cardJson.CardRelatedItems = ParseCardRelatedItemAreas(cardEl);

            return cardJson;
        }

        private static CardRelatedItemsGroupStructure[] ParseCardRelatedItemAreas(XContainer cardEl)
        {
            var cardRelatedItemsAreaJsons = new List<CardRelatedItemsGroupStructure>();

            var relatedItemsEl = cardEl.Element("RelatedItems");
            if (relatedItemsEl == null)
            {
                return cardRelatedItemsAreaJsons.ToArray();
            }

            foreach (var cardRelatedItemsAreaJsonEl in relatedItemsEl.Elements("RelatedItemsArea"))
            {
                var cardRelatedItemsAreaJson = new CardRelatedItemsGroupStructure();

                var name = cardRelatedItemsAreaJsonEl.Attribute("Name");
                if (name != null)
                {
                    cardRelatedItemsAreaJson.Name = name.Value;
                }

                var nameLocaleResourceId = cardRelatedItemsAreaJsonEl.Attribute("NameLocaleResourceId");
                if (nameLocaleResourceId != null)
                {
                    cardRelatedItemsAreaJson.NameLocaleResourceId = nameLocaleResourceId.Value;
                }

                cardRelatedItemsAreaJson.Items = ParseCardRelatedItems(cardRelatedItemsAreaJsonEl);

                cardRelatedItemsAreaJsons.Add(cardRelatedItemsAreaJson);
            }

            return cardRelatedItemsAreaJsons.ToArray();
        }

        private static IEnumerable<CardRelatedItemStructure> ParseCardRelatedItems(XContainer xContainer)
        {
            var cardRelatedItemsJsons = new List<CardRelatedItemStructure>();

            foreach (var relatedItemEl in xContainer.Elements("RelatedItem"))
            {
                var cardRelatedItemsJson = new CardRelatedItemStructure();

                var name = relatedItemEl.Attribute("Name");
                if (name != null)
                {
                    cardRelatedItemsJson.Name = name.Value;
                }

                var nameLocaleResourceId = relatedItemEl.Attribute("NameLocaleResourceId");
                if (nameLocaleResourceId != null)
                {
                    cardRelatedItemsJson.NameLocaleResourceId = nameLocaleResourceId.Value;
                }

                var disabledExpression = relatedItemEl.Attribute("DisabledExpression");
                if (disabledExpression != null)
                {
                    cardRelatedItemsJson.DisabledExpression = disabledExpression.Value;
                }

                var icon = relatedItemEl.Attribute("Icon");
                if (icon != null)
                {
                    cardRelatedItemsJson.Icon = icon.Value;
                }

                var requestUrl = relatedItemEl.Attribute("RequestUrl");
                if (requestUrl != null)
                {
                    cardRelatedItemsJson.RequestUrl = requestUrl.Value;
                }

                var extendedInfo = relatedItemEl.Attribute("ExtendedInfo");
                if (extendedInfo != null)
                {
                    cardRelatedItemsJson.ExtendedInfo = extendedInfo.Value;
                }

                var appendableEntity = relatedItemEl.Attribute("AppendableEntity");
                if (appendableEntity != null)
                {
                    cardRelatedItemsJson.AppendableEntity = appendableEntity.Value;
                }

                cardRelatedItemsJsons.Add(cardRelatedItemsJson);
            }

            return cardRelatedItemsJsons.ToArray();
        }

        private static ToolbarElementStructure[] ParseToolbarItems(XContainer cardEl)
        {
            var toolbarItemsEl = cardEl.Element("ToolbarItems");
            if (toolbarItemsEl == null)
            {
                return null;
            }

            var toolbarItems = new List<ToolbarElementStructure>();

            foreach (var toolbarItemEl in toolbarItemsEl.Elements("ToolbarItem"))
            {
                var toolbarItem = new ToolbarElementStructure();

                var name = toolbarItemEl.Attribute("Name");
                if (name != null)
                {
                    toolbarItem.Name = name.Value;
                }

                var parentName = toolbarItemEl.Attribute("ParentName");
                if (parentName != null)
                {
                    toolbarItem.ParentName = parentName.Value;
                }

                var hideInCardRelatedGrid = toolbarItemEl.Attribute("HideInCardRelatedGrid");
                if (hideInCardRelatedGrid != null)
                {
                    toolbarItem.HideInCardRelatedGrid = hideInCardRelatedGrid.Value;
                }

                var nameLocaleResourceId = toolbarItemEl.Attribute("NameLocaleResourceId");
                if (nameLocaleResourceId != null)
                {
                    toolbarItem.NameLocaleResourceId = nameLocaleResourceId.Value;
                }

                var controlType = toolbarItemEl.Attribute("ControlType");
                if (controlType != null)
                {
                    toolbarItem.ControlType = controlType.Value;
                }

                var lockOnInactive = toolbarItemEl.Attribute("LockOnInactive");
                if (lockOnInactive != null)
                {
                    toolbarItem.LockOnInactive = (bool)lockOnInactive;
                }

                var lockOnNew = toolbarItemEl.Attribute("LockOnNew");
                if (lockOnNew != null)
                {
                    toolbarItem.LockOnNew = (bool)lockOnNew;
                }

                var action = toolbarItemEl.Attribute("Action");
                if (action != null)
                {
                    toolbarItem.Action = action.Value;
                }

                var icon = toolbarItemEl.Attribute("Icon");
                if (icon != null)
                {
                    toolbarItem.Icon = icon.Value;
                }

                var securityPrivelegeFlag = toolbarItemEl.Attribute("SecurityPrivelegeFlag");
                if (securityPrivelegeFlag != null)
                {
                    toolbarItem.SecurityPrivelege = (int)securityPrivelegeFlag;
                }

                var disableOnEmpty = toolbarItemEl.Attribute("DisableOnEmpty");
                if (disableOnEmpty != null)
                {
                    toolbarItem.DisableOnEmpty = (bool)disableOnEmpty;
                }

                toolbarItems.Add(toolbarItem);
            }

            return toolbarItems.ToArray();
        }

        private IEnumerable<string> GetCardSettingsErrors(CardStructure xmlData, CardStructure codeData, EntityName entity)
        {
            var cardMetadataCorrections = _cardMetadataCorrections.ContainsKey(entity) ? _cardMetadataCorrections[entity] : null;
            var errors = new List<string>();
            const string SplitterElementName = "Splitter";
            var elementsToIgnore = new List<string>
                                       {
                                           SplitterElementName,
                                           "Save",
                                           "Create",
                                           "Update",
                                           "SaveAndClose",
                                           "CreateAndClose",
                                           "UpdateAndClose"
                                       };

            errors.AddRange(CheckProperties(xmlData,
                                            codeData,
                                            cardMetadataCorrections != null && cardMetadataCorrections.ContainsKey(entity.ToString()) ? cardMetadataCorrections[entity.ToString()] : null,
                                            x => x.HasAdminTab,
                                            x => x.HasComments,
                                            x => x.DecimalDigits,
                                            x => x.EntityName,
                                            x => x.EntityLocalizedName,
                                            x => x.EntityMainAttribute,
                                            x => x.EntityNameLocaleResourceId,
                                            x => x.Icon,
                                            x => x.Title,
                                            x => x.TitleResourceId));

            errors.AddRange(GetCardRelatedItemsGroupErrors(xmlData.CardRelatedItems.SingleOrDefault(), codeData.CardRelatedItems.SingleOrDefault(), cardMetadataCorrections));

            var xmlToolbarElementsCount = xmlData.CardToolbar.Count(x => !elementsToIgnore.Contains(x.Name));
            var codeToolbarElementsCount = codeData.CardToolbar.Count(x => !elementsToIgnore.Contains(x.Name));
            if (xmlToolbarElementsCount != codeToolbarElementsCount)
            {
                errors.Add(string.Format("Расхождение в количестве элементов меню. Зарегестрировано в xml: {0}; Зарегестрировано в коде: {1}",
                                         xmlToolbarElementsCount,
                                         codeToolbarElementsCount));
            }

            var xmlToolbarSplittersCount = xmlData.CardToolbar.Count(y => y.Name == SplitterElementName);
            var codeToolbarSplittersCount = codeData.CardToolbar.Count(y => y.Name == SplitterElementName);

            if (xmlToolbarSplittersCount != codeToolbarSplittersCount)
            {
                errors.Add(string.Format("Расхождение в количестве резделителей элементов меню. Зарегестрировано в xml: {0}; Зарегестрировано в коде: {1}", xmlToolbarSplittersCount, codeToolbarSplittersCount));
            }

            foreach (var xmlToolbarElement in xmlData.CardToolbar.Where(x => !elementsToIgnore.Contains(x.Name)))
            {
                errors.AddRange(GetToolbarElementErrors(xmlToolbarElement, codeData.CardToolbar.SingleOrDefault(x => x.Name == xmlToolbarElement.Name), cardMetadataCorrections));
            }

            return errors;
        }

        private IEnumerable<string> GetCardRelatedItemsGroupErrors(CardRelatedItemsGroupStructure xmlData, CardRelatedItemsGroupStructure codeData, IDictionary<string, IDictionary<string, Tuple<object, object>>> corrections)
        {
            var errors = new List<string>();
            if (xmlData == null && codeData == null)
            {
                return errors;
            }

            if (xmlData != null && codeData == null)
            {
                errors.Add("В xml зарегистрирована группа связанных объектов, которой нет в коде");
                return errors;
            }

            if (codeData != null && xmlData == null)
            {
                errors.Add("В коде зарегистрирована группа связанных объектов, которой нет в xml");
                return errors;
            }

            errors.AddRange(CheckProperties(xmlData,
                                            codeData,
                                            corrections != null && corrections.ContainsKey(xmlData.Name) ? corrections[xmlData.Name] : null,
                                            x => x.Name,
                                            x => x.LocalizedName,
                                            x => x.NameLocaleResourceId,
                                            x => x.Items.Count()));

            for (var i = 0; i < xmlData.Items.Count(); i++)
            {
                errors.AddRange(GetCardRelatedItemStructureErrors(xmlData.Items.ToArray()[i], codeData.Items.ToArray()[i], corrections));
            }

            return errors;
        }

        private IEnumerable<string> GetCardRelatedItemStructureErrors(CardRelatedItemStructure xmlData,
                                                                      CardRelatedItemStructure codeData,
                                                                      IDictionary<string, IDictionary<string, Tuple<object, object>>> corrections)
        {
            var errors = new List<string>();

            if (xmlData == null && codeData == null)
            {
                return errors;
            }

            if (xmlData != null && codeData == null)
            {
                errors.Add(string.Format("В xml зарегистрирован элемент {0}, который не зарегистрирован в коде", xmlData.Name));
                return errors;
            }

            if (codeData != null && xmlData == null)
            {
                errors.Add(string.Format("В коде зарегистрирован элемент {0}, которого нет в xml", codeData.Name));
                return errors;
            }

            var elementErrors = CheckProperties(xmlData,
                                                codeData,
                                                corrections != null && corrections.ContainsKey(xmlData.Name) ? corrections[xmlData.Name] : null,
                                                x => x.Name,
                                                x => x.LocalizedName,
                                                x => x.NameLocaleResourceId,
                                                x => x.RequestUrl,
                                                x => x.Icon,
                                                x => x.AppendableEntity,
                                                x => x.DisabledExpression,
                                                x => x.ExtendedInfo);

            if (elementErrors.Any())
            {
                errors.Add(string.Format("Для элемента связанных объектов {0} ({1}) зарегистрированы следующие расхождения: {2}",
                                         xmlData.Name,
                                         codeData.Name,
                                         string.Join(";", elementErrors)));
            }

            return errors;
        }

        private IEnumerable<string> GetToolbarElementErrors(ToolbarElementStructure xmlData, ToolbarElementStructure codeData, IDictionary<string, IDictionary<string, Tuple<object, object>>> corrections)
        {
            var errors = new List<string>();

            if (xmlData == null && codeData == null)
            {
                return errors;
            }

            if (xmlData != null && codeData == null)
            {
                errors.Add(string.Format("В xml зарегистрирован элемент {0}, который не зарегистрирован в коде", xmlData.Name));
                return errors;
            }

            if (codeData != null && xmlData == null)
            {
                errors.Add(string.Format("В коде зарегистрирован элемент {0}, которого нет в xml", codeData.Name));
                return errors;
            }

            var toolbarElementErrors =
                CheckProperties(xmlData,
                                codeData,
                                corrections != null && corrections.ContainsKey(xmlData.Name) ? corrections[xmlData.Name] : null,
                                x => x.Name,
                                x => x.ParentName,
                                x => x.LocalizedName,
                                x => x.NameLocaleResourceId,
                                x => x.DisableOnEmpty,
                                x => x.Icon,
                                x => x.Disabled,
                                x => x.LockOnInactive,
                                x => x.LockOnNew,
                                x => x.Action,
                                x => x.ControlType,
                                x => x.HideInCardRelatedGrid);

            if (xmlData.SecurityPrivelege != null || codeData.SecurityPrivelege != 0)
            {
                toolbarElementErrors = toolbarElementErrors.Concat(CheckProperties(xmlData,
                                                                                   codeData,
                                                                                   corrections != null && corrections.ContainsKey(xmlData.Name) ? corrections[xmlData.Name] : null,
                                                                                   x => x.SecurityPrivelege));
            }

            if (toolbarElementErrors.Any())
            {
                errors.Add(string.Format("Для элемента меню {0} ({1}) зарегистрированы следующие расхождения: {2}",
                                         xmlData.Name,
                                         codeData.Name,
                                         string.Join(";", toolbarElementErrors)));
            }

            return errors;
        }

        private static string GetResourceEntryContent(string resourceEntryName, string fallbackResourceEntryName)
        {
            var targetAssembly = Assembly.GetExecutingAssembly();
            var targetResource = targetAssembly.GetManifestResourceNames().FirstOrDefault(e => e.EndsWith(resourceEntryName)) ??
                                 targetAssembly.GetManifestResourceNames().FirstOrDefault(e => e.EndsWith(fallbackResourceEntryName));

            if (targetResource == null)
            {
                throw new InvalidOperationException(
                    string.Format("Can't find in assembly {0} resource entry, which ends with substring {1}. Fallback resource {2} not found", 
                    targetAssembly.FullName, 
                    resourceEntryName, 
                    fallbackResourceEntryName));
            }

            using (var stream = targetAssembly.GetManifestResourceStream(targetResource))
            {
                if (stream == null)
                {
                    throw new InvalidOperationException(string.Format("Can't load from assembly {0} resource entry {1}", targetAssembly.FullName, targetResource));
                }

                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        private IEnumerable<string> CheckProperties<T>(T xmlData, T codeData, IDictionary<string, Tuple<object, object>> corrections, params Expression<Func<T, object>>[] propertyExpressions)
            where T : class 
        {
            var errorsList = new List<string>();
            foreach (var propertyExpression in propertyExpressions)
            {
                var propertyName = StaticReflection.GetMemberName(propertyExpression);
                var func = propertyExpression.Compile();
                var xmlValue = func.Invoke(xmlData);
                var codeValue = func.Invoke(codeData);

                if (corrections != null && corrections.ContainsKey(propertyName))
                {
                    if (xmlValue != corrections[propertyName].Item1 && !xmlValue.Equals(corrections[propertyName].Item1))
                    {
                        errorsList.Add(string.Format("Рассхождение в значениях свойства {0}, заданных в xml и корректировке. Значение в xml: {1}. Значение в корректировке: {2}.",
                                                 propertyName,
                                                 xmlValue,
                                                 corrections[propertyName].Item1));
                    }

                    if (codeValue != corrections[propertyName].Item2 && !codeValue.Equals(corrections[propertyName].Item2))
                    {
                        errorsList.Add(string.Format("Рассхождение в значениях свойства {0}, заданных в коде и корректировке. Значение в коде: {1}. Значение в корректировке: {2}.",
                                                 propertyName,
                                                 codeValue,
                                                 corrections[propertyName].Item2));
                    }
                }

                if (xmlValue != codeValue && (xmlValue == null || !xmlValue.Equals(codeValue)))
                {
                    if (corrections != null && corrections.ContainsKey(propertyName))
                    {
                        if ((xmlValue == corrections[propertyName].Item1 || xmlValue.Equals(corrections[propertyName].Item1)) &&
                            (codeValue == corrections[propertyName].Item2 || codeValue.Equals(corrections[propertyName].Item2)))
                        {
                            continue;
                        }
                    }
                 
                    errorsList.Add(string.Format("Ошибка в свойстве {0}. Значение в xml: {1}. Значение в коде: {2}.",
                                                 propertyName,
                                                 xmlValue,
                                                 codeValue));
                }
            }

            return errorsList;
        }
    }
}
