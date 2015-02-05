using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.API.Common.Metadata.Old;
using DoubleGis.Erm.BLCore.API.Common.Metadata.Old.Dto;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.Model;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLFlex.UI.Metadata.Config.Old
{
    public sealed class UIConfigurationService : IUIConfigurationService
    {
        private const string EntitySettingsResourceEntry = "EntitySettings.xml";
        private const string NavigationSettingsResourceEntry = "NavigationSettings.xml";
        private const string EntitySettingsResourceEntryFormat = "EntitySettings.{0}.xml";
        private const string NavigationSettingsResourceEntryFormat = "NavigationSettings.{0}.xml";

        private static readonly Dictionary<BusinessModel, List<NavigationElementStructure>> NavigationSettings = ParseNavigationSettings();
        private static readonly Dictionary<BusinessModel, Dictionary<IEntityType, CardStructure>> CardSettings = ParseCardSettings();
        private static readonly Dictionary<BusinessModel, Dictionary<IEntityType, EntityDataListsContainer>> GridSettings = ParseGridSettings();

        private readonly IGlobalizationSettings _globalizationSettings;

        public UIConfigurationService(IGlobalizationSettings globalizationSettings)
        {
            _globalizationSettings = globalizationSettings;
        }

        #region NavigationArea

        private static Dictionary<BusinessModel, List<NavigationElementStructure>> ParseNavigationSettings()
        {
            var result = new Dictionary<BusinessModel, List<NavigationElementStructure>>();

            foreach (BusinessModel val in Enum.GetValues(typeof(BusinessModel)))
            {
                var stringResourceName = string.Format(NavigationSettingsResourceEntryFormat, Enum.GetName(typeof(BusinessModel), val));
                var container = XDocument.Parse(GetResourceEntryContent(stringResourceName, NavigationSettingsResourceEntry)).Root;
                var navigationSettings = ParseNavigationSettings(container);
                result.Add(val, navigationSettings);
            }

            return result;
        }

        private static List<NavigationElementStructure> ParseNavigationSettings(XContainer container)
        {
            var navigationSettings = new List<NavigationElementStructure>();

            foreach (var item in container.Elements("Items"))
            {
                var navigationDto = ParseNavigationDto(item);
                navigationDto.Items = ParseNavigationSettings(item);

                navigationSettings.Add(navigationDto);
            }

            return navigationSettings;
        }

        private static NavigationElementStructure ParseNavigationDto(XElement element)
        {
            var navigationDto = new NavigationElementStructure();

            var name = element.Attribute("Name");
            if (name != null)
            {
                navigationDto.NameLocaleResourceId = name.Value;
            }

            var icon = element.Attribute("Icon");
            if (icon != null)
            {
                navigationDto.Icon = icon.Value;
            }

            var requestUrl = element.Attribute("RequestUrl");
            if (requestUrl != null)
            {
                navigationDto.RequestUrl = requestUrl.Value;
            }

            return navigationDto;
        }

        #endregion

        #region DataList

        public EntityDataListsContainer GetGridSettings(BusinessModel adaptation, IEntityType entityName)
        {
            Dictionary<IEntityType, EntityDataListsContainer> container;
            if (!GridSettings.TryGetValue(adaptation, out container))
            {
                throw new ArgumentException("Cannot find metadata for adaptation " + adaptation);
            }

            EntityDataListsContainer entityViewSet;
            if (!container.TryGetValue(entityName, out entityViewSet))
            {
                throw new ArgumentException("Cannot find metadata for entity " + entityName);
            }

            return entityViewSet;
        }

        private static Dictionary<BusinessModel, Dictionary<IEntityType, EntityDataListsContainer>> ParseGridSettings()
        {
            var result = new Dictionary<BusinessModel, Dictionary<IEntityType, EntityDataListsContainer>>();

            foreach (BusinessModel val in Enum.GetValues(typeof(BusinessModel)))
            {
                var stringResourceName = string.Format(EntitySettingsResourceEntryFormat, Enum.GetName(typeof(BusinessModel), val));
                var container = XDocument.Parse(GetResourceEntryContent(stringResourceName, EntitySettingsResourceEntry)).Root;
                var gridSettings = ParseGridSettings(container);
                result.Add(val, gridSettings);
            }

            return result;
        }

        private static Dictionary<IEntityType, EntityDataListsContainer> ParseGridSettings(XContainer container)
        {
            var dictionary = new Dictionary<IEntityType, EntityDataListsContainer>();

            foreach (var entityEl in container.Elements("Entity"))
            {
                var entityNameNonParsed = (string)entityEl.Attribute("Name");

                IEntityType entityName;
                if (!EntityType.Instance.TryParse(entityNameNonParsed, out entityName))
                {
                    throw new ArgumentException("Unrecognized entity type");
                }

                var entityViewSet = ParseEntityViewSet(entityEl, entityName);
                dictionary.Add(entityName, entityViewSet);
            }

            return dictionary;
        }

        private static EntityDataListsContainer ParseEntityViewSet(XElement entityEl, IEntityType entityName)
        {
            var entityViewSet = new EntityDataListsContainer { EntityName = entityName.Description };

            var dataListsEl = entityEl.Element("DataLists");
            if (dataListsEl == null)
            {
                return entityViewSet;
            }

            var cardEl = entityEl.Element("Card");
            if (cardEl != null)
            {
                entityViewSet.HasCard = true;
            }

            entityViewSet.DataViews = ParseDataViews(dataListsEl, entityName);

            return entityViewSet;
        }

        private static IEnumerable<DataListStructure> ParseDataViews(XContainer dataListsEl, IEntityType entityName)
        {
            var dataViews = new List<DataListStructure>();
            var dataLists = dataListsEl.Elements("DataList").ToList();
            dataLists.Sort(DataListCompare);

            foreach (var dataListEl in dataListsEl.Elements("DataList"))
            {
                DataListStructure dataView;

                var basedOnAtt = dataListEl.Attribute("BasedOn");

                if (basedOnAtt != null)
                {
                    var basedOnDataView = dataViews.FirstOrDefault(x => x.NameLocaleResourceId == basedOnAtt.Value);
                    if (basedOnDataView == null)
                    {
                        throw new InvalidOperationException(
                            string.Format("Base data list '{0}' not found for data list '{1}'",
                                          basedOnAtt.Value,
                                          dataListEl.Attribute("NameLocaleResourceId").Value));
                    }

                    dataView = new DataListStructure
                    {
                        NameLocaleResourceId = basedOnDataView.NameLocaleResourceId,
                        TitleLocaleResourceId = basedOnDataView.TitleLocaleResourceId,
                        Name = basedOnDataView.Name,
                        Icon = basedOnDataView.Icon,
                        MainAttribute = basedOnDataView.MainAttribute,
                        RowsPerPage = basedOnDataView.RowsPerPage,
                        AllowMultiple = basedOnDataView.AllowMultiple,
                        ControllerAction = basedOnDataView.ControllerAction,
                        ReadOnly = basedOnDataView.ReadOnly,
                        DefaultSortField = basedOnDataView.DefaultSortField,
                        DefaultSortDirection = basedOnDataView.DefaultSortDirection,
                        DisableEdit = basedOnDataView.DisableEdit,
                        HideInCardRelatedGrid = basedOnDataView.HideInCardRelatedGrid,
                        IsHidden = basedOnDataView.IsHidden,

                        Fields = basedOnDataView.Fields.Select(y => new DataListColumnStructure
                        {
                            ReferenceTo = y.ReferenceTo,
                            ReferenceKeyField = y.ReferenceKeyField,
                            NameLocaleResourceId = y.NameLocaleResourceId,
                            Hidden = y.Hidden,
                            Name = y.Name,
                            Width = y.Width,
                            FieldType = y.FieldType,
                            Type = y.Type,
                            Sortable = y.Sortable,
                        }).ToArray(),

                        ToolbarItems = basedOnDataView.ToolbarItems.Select(y => new ToolbarElementStructure
                        {
                            Name = y.Name,
                            ParentName = y.ParentName,
                            HideInCardRelatedGrid = y.HideInCardRelatedGrid,
                            ControlType = y.ControlType,
                            Action = y.Action,
                            Icon = y.Icon,
                            Disabled = y.Disabled,
                            SecurityPrivelege = y.SecurityPrivelege,
                            NameLocaleResourceId = y.NameLocaleResourceId,
                            LockOnInactive = y.LockOnInactive,
                            LockOnNew = y.LockOnNew,
                        }).ToArray(),

                        Scripts = basedOnDataView.Scripts.Select(y => new DataListScriptReference
                        {
                            FileName = y.FileName,
                        }).ToArray(),
                    };
                }
                else
                {
                    dataView = new DataListStructure { Name = entityName.ToString() };
                }

                // Далее получается, что в случае унаследованного дата листа перекрываем 
                // родительские свойства, в случае нового - тупо инициализируем.
                ReadDataViewAttributesFromElement(dataView, dataListEl);

                var scripts = ParseDataListScripts(dataListEl);
                if (scripts != null)
                {
                    dataView.Scripts = scripts;
                }

                var toolbarItems = ParseToolbarItems(dataListEl);
                if (toolbarItems != null)
                {
                    dataView.ToolbarItems = toolbarItems;
                }

                var fields = ParseDataListFields(dataListEl);
                if (fields != null)
                {
                    dataView.Fields = ParseDataListFields(dataListEl);
                }

                dataView.Scripts = dataView.Scripts ?? new DataListScriptReference[0];
                dataView.ToolbarItems = dataView.ToolbarItems ?? new ToolbarElementStructure[0];
                dataView.Fields = dataView.Fields ?? new DataListColumnStructure[0];

                dataViews.Add(dataView);
            }

            return dataViews.ToArray();
        }

        /// <summary>
        /// Сравнивание листов для сортировки по иерархии наследования - от базовых к наследникам
        /// </summary>
        private static int DataListCompare(XElement dataListElement1, XElement dataListElement2)
        {
            var basedOnAtt1 = dataListElement1.Attribute("BasedOn");
            var basedOnAtt2 = dataListElement2.Attribute("BasedOn");
            if (basedOnAtt1 == null && basedOnAtt2 == null)
            {
                return 0;
            }

            if (basedOnAtt1 == null)
            {
                return -1;
            }

            if (basedOnAtt2 == null)
            {
                return 1;
            }

            if (basedOnAtt1.Value == dataListElement2.Attribute("NameLocaleResourceId").Value)
            {
                return 1;
            }
                
            if (basedOnAtt2.Value == dataListElement1.Attribute("NameLocaleResourceId").Value)
            {
                return -1;
            }

            return 0;
        }

        private static void ReadDataViewAttributesFromElement(DataListStructure dataView, XElement dataListEl)
        {
            var nameLocaleResourceId = dataListEl.Attribute("NameLocaleResourceId");
            if (nameLocaleResourceId != null)
            {
                dataView.NameLocaleResourceId = nameLocaleResourceId.Value;
            }

            var gridTitleResourceId = dataListEl.Attribute("TitleLocaleResourceId");

            if (gridTitleResourceId != null)
            {
                dataView.TitleLocaleResourceId = gridTitleResourceId.Value;
            }

            var defaultSortField = dataListEl.Attribute("DefaultSortField");
            if (defaultSortField != null)
            {
                dataView.DefaultSortField = defaultSortField.Value;
            }

            var sortDescending = dataListEl.Attribute("SortDescending");
            if (sortDescending != null)
            {
                dataView.DefaultSortDirection = (byte)(int)sortDescending;
            }

            var rowsPerPage = dataListEl.Attribute("RowsPerPage");
            if (rowsPerPage != null)
            {
                dataView.RowsPerPage = (short)rowsPerPage;
            }

            var allowMultiple = dataListEl.Attribute("AllowMultiple");
            if (allowMultiple != null)
            {
                dataView.AllowMultiple = (bool)allowMultiple;
            }

            var readOnly = dataListEl.Attribute("ReadOnly");
            if (readOnly != null)
            {
                dataView.ReadOnly = (bool)readOnly;
            }

            var mainAttribute = dataListEl.Attribute("MainAttribute");
            if (mainAttribute != null)
            {
                dataView.MainAttribute = mainAttribute.Value;
            }

            var icon = dataListEl.Attribute("Icon");
            if (icon != null)
            {
                dataView.Icon = icon.Value;
            }

            var controllerAction = dataListEl.Attribute("ControllerAction");
            if (controllerAction != null)
            {
                dataView.ControllerAction = controllerAction.Value;
            }

            var disableEdit = dataListEl.Attribute("DisableEdit");
            if (disableEdit != null)
            {
                dataView.DisableEdit = (bool)disableEdit;
            }

            var hideInCardRelatedGrid = dataListEl.Attribute("HideInCardRelatedGrid");
            if (hideInCardRelatedGrid != null)
            {
                dataView.HideInCardRelatedGrid = hideInCardRelatedGrid.Value;
            }

            var isHidden = dataListEl.Attribute("IsHidden");
            if (isHidden != null)
            {
                dataView.IsHidden = (bool)isHidden;
            }
        }

        private static IEnumerable<DataListColumnStructure> ParseDataListFields(XContainer dataListEl)
        {
            var fieldsEl = dataListEl.Element("Fields");
            if (fieldsEl == null)
            {
                return null;
            }

            var fields = new List<DataListColumnStructure>();

            foreach (var fieldEl in fieldsEl.Elements("Field"))
            {
                var field = new DataListColumnStructure();

                var name = fieldEl.Attribute("Name");
                if (name != null)
                {
                    field.Name = name.Value;
                }

                var nameLocaleResourceId = fieldEl.Attribute("NameLocaleResourceId");
                if (nameLocaleResourceId != null)
                {
                    field.NameLocaleResourceId = nameLocaleResourceId.Value;
                }

                var width = fieldEl.Attribute("Width");
                if (width != null)
                {
                    field.Width = (short)width;
                }

                var hidden = fieldEl.Attribute("Hidden");
                if (hidden != null)
                {
                    field.Hidden = (bool)hidden;
                }

                var fieldType = fieldEl.Attribute("FieldType");
                if (fieldType != null)
                {
                    field.FieldType = fieldType.Value;
                }

                var extTypeName = fieldEl.Attribute("ExtTypeName");
                if (extTypeName != null)
                {
                    field.Type = extTypeName.Value;
                }

                var referenceEntityName = fieldEl.Attribute("ReferenceEntityName");
                if (referenceEntityName != null)
                {
                    field.ReferenceEntityName = referenceEntityName.Value;
                }

                var referenceFieldName = fieldEl.Attribute("ReferenceFieldName");
                if (referenceFieldName != null)
                {
                    field.ReferenceFieldName = referenceFieldName.Value;
                }

                var sortableField = fieldEl.Attribute("Sortable");
                field.Sortable = sortableField == null || (bool)sortableField; // true by default

                fields.Add(field);
            }

            return fields;
        }

        private static IEnumerable<DataListScriptReference> ParseDataListScripts(XContainer dataListEl)
        {
            var scriptsEl = dataListEl.Element("Scripts");
            if (scriptsEl == null)
            {
                return null;
            }

            var scripts = new List<DataListScriptReference>();

            foreach (var scriptEl in scriptsEl.Elements("Script"))
            {
                var script = new DataListScriptReference();

                var fileName = scriptEl.Attribute("FileName");
                if (fileName != null)
                {
                    script.FileName = fileName.Value;
                }

                scripts.Add(script);
            }

            return scripts;
        }

        #endregion

        #region Card

        public CardStructure GetCardSettings(BusinessModel adaptation, IEntityType entityName)
        {
            Dictionary<IEntityType, CardStructure> container;
            if (!CardSettings.TryGetValue(adaptation, out container))
            {
                throw new ArgumentException("Cannot find metadata for adaptation " + adaptation);
            }

            CardStructure cardJson;
            if (!container.TryGetValue(entityName, out cardJson))
            {
                throw new ArgumentException("Cannot find metadata for entity");
            }

            return cardJson;
        }

        public IEnumerable<NavigationElementStructure> GetNavigationSettings(CultureInfo culture)
        {
            List<NavigationElementStructure> container;

            if (!NavigationSettings.TryGetValue(_globalizationSettings.BusinessModel, out container))
            {
                throw new ArgumentException("Cannot find metadata for adaptation " + _globalizationSettings.BusinessModel);
            }

            return LocalizeNavigationSettings(container, culture);
        }

        private static IEnumerable<NavigationElementStructure> LocalizeNavigationSettings(IEnumerable<NavigationElementStructure> navigationSettings, CultureInfo culture)
        {
            var localizedNavigationSettings = navigationSettings.Select(x => new NavigationElementStructure
            {
                NameLocaleResourceId = x.NameLocaleResourceId,
                LocalizedName = GetLocalizedName(x.NameLocaleResourceId, culture),
                Icon = x.Icon,
                RequestUrl = x.RequestUrl,

                Items = LocalizeNavigationSettings(x.Items, culture),
            }).ToArray();

            return localizedNavigationSettings;
        }

        public EntityDataListsContainer GetGridSettings(IEntityType entityName, CultureInfo culture)
        {
            var gridSettings = GetGridSettings(_globalizationSettings.BusinessModel, entityName);

            var localizedGridSettings = new EntityDataListsContainer
            {
                HasCard = gridSettings.HasCard,
                EntityName = gridSettings.EntityName,

                DataViews = gridSettings.DataViews.Select(x => new DataListStructure
                {
                    NameLocaleResourceId = x.NameLocaleResourceId,
                    Title = GetLocalizedName(x.TitleLocaleResourceId, culture),
                    TitleLocaleResourceId = x.TitleLocaleResourceId,
                    Name = x.Name,
                    Icon = x.Icon,
                    MainAttribute = x.MainAttribute,
                    RowsPerPage = x.RowsPerPage,
                    AllowMultiple = x.AllowMultiple,
                    ControllerAction = x.ControllerAction,
                    ReadOnly = x.ReadOnly,
                    LocalizedName = GetLocalizedName(x.NameLocaleResourceId ?? x.Name, culture),
                    DefaultSortField = x.DefaultSortField,
                    DefaultSortDirection = x.DefaultSortDirection,
                    DisableEdit = x.DisableEdit,
                    HideInCardRelatedGrid = x.HideInCardRelatedGrid,
                    IsHidden = x.IsHidden,

                    Fields = x.Fields.Select(y => new DataListColumnStructure
                    {
                        ReferenceTo = y.ReferenceTo,
                        ReferenceKeyField = y.ReferenceKeyField,
                        NameLocaleResourceId = y.NameLocaleResourceId,
                        LocalizedName = GetLocalizedName(y.NameLocaleResourceId ?? y.Name, culture),
                        Hidden = y.Hidden,
                        Name = y.Name,
                        Width = y.Width,
                        FieldType = y.FieldType,
                        Type = y.Type,
                        Sortable = y.Sortable,
                    }).ToArray(),

                    ToolbarItems = x.ToolbarItems.Select(y => new ToolbarElementStructure
                    {
                        Name = y.Name,
                        ParentName = y.ParentName,
                        HideInCardRelatedGrid = y.HideInCardRelatedGrid,
                        ControlType = y.ControlType,
                        Action = y.Action,
                        Icon = y.Icon,
                        Disabled = y.Disabled,
                        LocalizedName = GetLocalizedName(y.NameLocaleResourceId ?? y.Name, culture),
                        SecurityPrivelege = y.SecurityPrivelege,
                        NameLocaleResourceId = y.NameLocaleResourceId,
                        LockOnInactive = y.LockOnInactive,
                        LockOnNew = y.LockOnNew,
                        DisableOnEmpty = y.DisableOnEmpty
                    }).ToArray(),

                    Scripts = x.Scripts.Select(y => new DataListScriptReference
                    {
                        FileName = y.FileName,
                    }).ToArray(),

                }).ToArray(),
            };

            return localizedGridSettings;
        }

        public CardStructure GetCardSettings(IEntityType entityName, CultureInfo culture)
        {
            var cardSettings = GetCardSettings(_globalizationSettings.BusinessModel, entityName);

            var localizedCardSettings = new CardStructure
            {
                Icon = cardSettings.Icon,
                LargeIcon = cardSettings.LargeIcon,
                CardLocalizedName = GetLocalizedName(cardSettings.CardNameLocaleResourceId, culture),
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

                CardNameLocaleResourceId = cardSettings.CardNameLocaleResourceId,
                EntityNameLocaleResourceId = cardSettings.EntityNameLocaleResourceId,
            };

            return localizedCardSettings;
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

        private static Dictionary<BusinessModel, Dictionary<IEntityType, CardStructure>> ParseCardSettings()
        {
            var result = new Dictionary<BusinessModel, Dictionary<IEntityType, CardStructure>>();

            foreach (BusinessModel val in Enum.GetValues(typeof(BusinessModel)))
            {
                var stringResourceName = string.Format(EntitySettingsResourceEntryFormat, Enum.GetName(typeof(BusinessModel), val));
                var container = XDocument.Parse(GetResourceEntryContent(stringResourceName, EntitySettingsResourceEntry)).Root;
                var cardSettings = ParseCardSettings(container);
                result.Add(val, cardSettings);
            }

            return result;
        }

        private static Dictionary<IEntityType, CardStructure> ParseCardSettings(XContainer container)
        {
            var dictionary = new Dictionary<IEntityType, CardStructure>();

            foreach (var entityEl in container.Elements("Entity"))
            {
                var entityNameNonParsed = (string)entityEl.Attribute("Name");

                IEntityType entityName;
                if (!EntityType.Instance.TryParse(entityNameNonParsed, out entityName))
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

        private static CardStructure ParseCardDto(XContainer entityEl, IEntityType entityName)
        {
            var cardEl = entityEl.Element("Card");
            if (cardEl == null)
            {
                return null;
            }

            var cardJson = new CardStructure { EntityName = entityName.Description };

            var cardNameLocaleResourceId = cardEl.Attribute("CardNameLocaleResourceId");
            if (cardNameLocaleResourceId != null)
            {
                cardJson.CardNameLocaleResourceId = cardNameLocaleResourceId.Value;
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

            var icon = cardEl.Attribute("Icon");
            if (icon != null)
            {
                cardJson.Icon = icon.Value;
            }

            var largeIcon = cardEl.Attribute("LargeIcon");
            if (largeIcon != null)
            {
                cardJson.LargeIcon = largeIcon.Value;
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

        #endregion

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
    }
}
