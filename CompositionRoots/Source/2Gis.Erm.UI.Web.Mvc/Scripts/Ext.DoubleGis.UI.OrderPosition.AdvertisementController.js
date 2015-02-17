Ext.ns("Ext.DoubleGis.ui.OrderPosition");

Ext.DoubleGis.UI.OrderPosition.Advertisements = Ext.extend(Ext.util.Observable, {
    nodeTypes: {
        General: 0,
        LinkingObject: 1,
        AddCategoryLink: 2,
        Warning: 3
    },

    linkingObjectTypes: Ext.DoubleGis.ui.OrderPosition.LinkingObjectTypes,

    /* Private properties */

    advertisements: {
        byKey: null,
        byPosition: null
    },

    controls: {
        jsonHidden: null,
        targetDiv: null
    },

    localData: {
        firmId: null,
        organizationUnitId: null,
        areLinkingObjectParametersLocked: null,
        useSingleCategoryForPackage: null,
        salesModel: null,
        linkingObjectsByKey: [],
        linkingObjects: [],
        positions: [],
        lastSelectedCount: null,
        nodeIdentities: []
    },

    serverData: {
        linkingObjectsSchema: {
            categories: [], //Categories item is { Id:<int>, Name:<string>, Level:<int> }
            firmAddresses: [], //FirmAddresses item is { Id:<int>, Address:<string>, Categories:<array of int> }
            positions: [] // Positions item is { Id:<int>, AdvertisementTemplateId:<int>, DummyAdvertisementId:<int>, LinkingObjectType:<string>, Name:<string>, LinkingObjects: <array of LinkingObject> }
        }
    },

    ui: {
        treeGrid: null,
        dumbCheckbox: null,
        settings: {
            titleColumnWidth: 470,
            advertisementColumnWidth: 450,
            widthDelta: 50,
            checkBoxColumnWidth: 30,
            dummyCheckBoxColumnWidth: 70
        }
    },


    // Содержит настройки, применяемые в зависимости от выбранного режима элемента управления.
    // Внешний вид и поведение создаваемого элемента управления различаются для разных режимов работы.
    // По-умолчанию выставляется режим для работы в карточке позиции заказа - тот, который раньше был единственным.
    modeDescriptions: {
        editOrderPosition: {
            setupWidth: function (columns, width, settings) {
                var sum = settings.advertisementColumnWidth + settings.titleColumnWidth + settings.checkBoxColumnWidth + settings.dummyCheckBoxColumnWidth;
                var rate = width / sum;
                columns[0].width = settings.titleColumnWidth * rate;
                columns[2].width = settings.advertisementColumnWidth * rate;
                columns[3].width = settings.dummyCheckBoxColumnWidth * rate;
            },
            getColumns: function () {
                var self = this;

                return [{
                    header: Ext.LocalizedResources.LinkingObject,
                    dataIndex: 'name',
                    width: self.ui.settings.titleColumnWidth
                }, {
                    header: '',
                    dataIndex: 'dumb',
                    sortType: 'asFloat',
                    width: self.ui.settings.checkBoxColumnWidth,
                    align: 'center',

                    tpl: new window.Ext.XTemplate('{id:this.renderCheckbox}', {
                        renderCheckbox: function (id) {
                            var identity = self.localData.nodeIdentities[id];
                            if (identity.nodeType == self.nodeTypes.LinkingObject) {
                                var element = identity.linkingObject.beginCheckboxCreation();
                                return element.outerHTML;
                            }
                            return '<span></span>';
                        }
                    })
                }, {
                    header: Ext.LocalizedResources.TheAdvertisement,
                    dataIndex: 'id',
                    sortType: 'asFloat',
                    width: self.ui.settings.advertisementColumnWidth,
                    align: 'center',

                    tpl: new window.Ext.XTemplate('{id:this.renderAdvertisement}', {
                        renderAdvertisement: function (id) {
                            var identity = self.localData.nodeIdentities[parseInt(id)];
                            if (identity.nodeType == self.nodeTypes.LinkingObject) {
                                if (identity.linkingObject.supportsAdvertisement()) {
                                    var element = identity.linkingObject.beginAdvertisementLookupCreation();
                                    return element.outerHTML;
                                }
                            }
                            return '<span></span>';
                        }
                    })
                }, {
                    header: Ext.LocalizedResources.DummyValue,
                    dataIndex: 'isDummy',
                    sortType: 'asFloat',
                    width: self.ui.settings.dummyCheckBoxColumnWidth,
                    align: 'center',

                    tpl: new window.Ext.XTemplate('{id:this.renderCheckbox}', {
                        renderCheckbox: function (id) {
                            var identity = self.localData.nodeIdentities[parseInt(id)];
                            if (identity.nodeType == self.nodeTypes.LinkingObject) {
                                if (identity.linkingObject.supportsAdvertisement()) {
                                    var element = identity.linkingObject.beginDummyCheckboxCreation();
                                    return element.outerHTML;
                                }
                            }
                            return '<span></span>';
                        }
                    })
                }];
            }
        },
        changeLinkedObjects: {
            setupWidth: function (columns, width, settings) {
                columns[0].width = width - 2 * settings.checkBoxColumnWidth;
            },
            getColumns: function () {
                var self = this;

                return [{
                    header: Ext.LocalizedResources.LinkingObject,
                    dataIndex: 'name',
                    width: self.ui.settings.titleColumnWidth
                }, {
                    header: '',
                    dataIndex: 'dumb',
                    sortType: 'asFloat',
                    width: self.ui.settings.checkBoxColumnWidth,
                    align: 'center',

                    tpl: new window.Ext.XTemplate('{id:this.renderCheckbox}', {
                        renderCheckbox: function (id) {
                            var identity = self.localData.nodeIdentities[id];
                            if (identity.nodeType == self.nodeTypes.LinkingObject) {
                                var element = identity.linkingObject.beginDisabledCheckboxCreation();
                                return element.outerHTML;
                            }
                            return '<span></span>';
                        }
                    })
                }, {
                    header: '',
                    dataIndex: 'dumb',
                    sortType: 'asFloat',
                    width: self.ui.settings.checkBoxColumnWidth,
                    align: 'center',

                    tpl: new window.Ext.XTemplate('{id:this.renderCheckbox}', {
                        renderCheckbox: function (id) {
                            var identity = self.localData.nodeIdentities[parseInt(id)];
                            if (identity.nodeType == self.nodeTypes.LinkingObject) {
                                var element = identity.linkingObject.beginCheckboxCreation();
                                return element.outerHTML;
                            }
                            return '<span></span>';
                        }
                    })
                }];
            }
        }
    },

    /* Public methods */

    constructor: function (mode) {
        this.addEvents("selectedCountChanged");
        this.mode = mode ? this.modeDescriptions[mode] : this.modeDescriptions['editOrderPosition'];
    },

    setLocalData: function (data) {
        window.Ext.apply(this.localData, data);
    },

    setSchema: function (linkingObjectsSchema) {
        this.reset();
        this.serverData.linkingObjectsSchema = linkingObjectsSchema;
        this.rebuildLinkingObjectsLayout();

       
        var selectedLinkingObject = this.localData.useSingleCategoryForPackage
             // get the first selected linking object 
            ? this.localData.linkingObjects.findOne(function(object) {
                return object.isSelected();
            })
            : null;
        
        this.notifySelectedCountChanged(selectedLinkingObject);
    },

    registerControls: function (controls) {
        window.Ext.apply(this.controls, controls);
        this.registerAdvertisements(window.Ext.decode(this.controls.jsonHidden.dom.value));
    },

    hasInvalidCheckedObjects: function () {
        var invalidObject = this.localData.linkingObjects.findOne(function (object) {
            return object.isDeleted() && object.isSelected() && object.isChanged();
        });

        return invalidObject;
    },

    prepareToSave: function () {
        this.controls.jsonHidden.dom.value = window.Ext.encode(this.gatherAdvertisements());
    },

    /* Events */

    notifySelectedCountChanged: function (source) {
        var args = { selectedCount: 0, isLimitReached: false, categoryIds: [] };
        this.localData.positions.forEach(function (object) {
            object.isAdvertisementLimitReached = false;
        });

        var isSingleCategoryType = function(object) {
            return object.type == Ext.DoubleGis.UI.OrderPosition.LinkingObjectTypes.CategorySingle ||
                object.type == Ext.DoubleGis.UI.OrderPosition.LinkingObjectTypes.AddressCategorySingle ||
                object.type == Ext.DoubleGis.UI.OrderPosition.LinkingObjectTypes.AddressFirstLevelCategorySingle;
        };

        if (this.localData.useSingleCategoryForPackage && source) {
            this.localData.linkingObjects.forEach(function(object) {
                if (object.categoryId == source.categoryId) {
                    object.checkbox.checked = source.checkbox.checked;
                } else if (isSingleCategoryType(object)) {
                    object.checkbox.checked = false;
                }
            });
        }

        // Собираем все выбранные категории, чтобы на основании их определить применяемый коэффициент.
        // Имеет смысл только при рассчёте коэффициента по выбранным рубрикам.
        this.localData.linkingObjects.forEach(function (object) {
            if (object.checkbox.checked && args.categoryIds.indexOf(object.categoryId) < 0) {
                args.categoryIds.push(object.categoryId);
            }
        });

        this.localData.linkingObjects.forEach(function (object) {
            if (object.isSelected()) {
                if (object.position.IsLinkingObjectOfSingleType) object.position.isAdvertisementLimitReached = true;
                args.selectedCount++;
            } else {
                object.clearAdvertisement();
            }
        });

        this.fireEvent("selectedCountChanged", args);
        this.setLimitReached(args.isLimitReached);
        this.localData.lastSelectedCount = args.selectedCount;
    },

    /* Private methods - tree creation/maintenance */

    rebuildLinkingObjectsLayout: function () {

        window.Ext.QuickTips.init();

        var rootConfig = {
            expanded: true,
            draggable: false,
            id: this.createNodeIdentity(this.nodeTypes.General, null),
            name: 'Root',
            advertisement: null,
            children: [],
            leaf: false
        };

        this.createTree(rootConfig);

        var schema = this.serverData.linkingObjectsSchema;
        var categoriesMap = new Object();
        var i;
        for (i = 0; i < schema.FirmCategories.length; i++) {
            categoriesMap[schema.FirmCategories[i].Id] = schema.FirmCategories[i];
        }
        for (i = 0; i < schema.AdditionalCategories.length; i++) {
            categoriesMap[schema.AdditionalCategories[i].Id] = schema.AdditionalCategories[i];
        }

        this.localData.linkingObjectsByKey = {};

        var linkingObjectConstructor = window.Ext.DoubleGis.UI.OrderPosition.LinkingObject;

        var linkingObjectTypes = Ext.DoubleGis.UI.OrderPosition.LinkingObjectTypes;

        for (i = 0; i < schema.Positions.length; i++) {
            var position = schema.Positions[i];
            position.LinkingObjects = [];
            position.isAdvertisementLimitReached = false;

            var positionNode = {
                expanded: true,
                expandable: true,
                draggable: false,
                id: this.createNodeIdentity(this.nodeTypes.General, position),
                name: position.Name,
                advertisement: null,
                children: [],
                dumb: '0',
                leaf: false
            };
            rootConfig.children.push(positionNode);

            if (schema.Warnings && schema.Warnings.length > 0) {

                var warningNode = this.createWarningNode(schema.Warnings[0].Text, this.createNodeIdentity(this.nodeTypes.Warning, position));
                warningNode.leaf = true;
                positionNode.children.push(warningNode);
            }

            var linkingObject;
            var address;
            var addressNode;
            var categoryNode;
            var j;

            switch (position.LinkingObjectType) {
                // firm    
                case linkingObjectTypes.Firm:
                    {
                        linkingObject = new linkingObjectConstructor(this, position, null, null, position.LinkingObjectType);
                        linkingObject.node = positionNode;
                        positionNode.id = this.createNodeIdentity(this.nodeTypes.LinkingObject, position, linkingObject),
                        positionNode.leaf = true;
                    }
                    break;

                    // address     
                case linkingObjectTypes.AddressSingle:
                case linkingObjectTypes.AddressMultiple:
                    {
                        for (j = 0; j < schema.FirmAddresses.length; j++) {
                            address = schema.FirmAddresses[j];
                            linkingObject = new linkingObjectConstructor(this, position, null, address.Id, position.LinkingObjectType);

                            addressNode = this.createAddressNode(address, this.createNodeIdentity(this.nodeTypes.LinkingObject, position, linkingObject));
                            addressNode.leaf = true;
                            positionNode.children.push(addressNode);
                            linkingObject.node = addressNode;
                        }
                    }
                    break;

                    // category      
                case linkingObjectTypes.CategorySingle:
                case linkingObjectTypes.CategoryMultiple:
                case linkingObjectTypes.CategoryMultipleAsterix:
                    {
                        var requiredCategories = [];
                        var requiredCategoriesList = [];

                        for (j = 0; j < schema.FirmCategories.length; j++) {
                            if (schema.FirmCategories[j].Level != 3)
                                continue;

                            requiredCategories[schema.FirmCategories[j].Id] = schema.FirmCategories[j];
                            requiredCategoriesList.push(schema.FirmCategories[j]);
                        }

                        var categoriesToAdd = [];
                        var positionAdvertisements = this.advertisements.byPosition[position.Id];
                        if (positionAdvertisements) {
                            for (j = 0; j < positionAdvertisements.length; j++) {
                                if (positionAdvertisements[j].CategoryId && !requiredCategories[positionAdvertisements[j].CategoryId]) {
                                    categoriesToAdd[positionAdvertisements[j].CategoryId] = true;
                                }
                            }
                            for (j = 0; j < schema.AdditionalCategories.length; j++) {
                                if (categoriesToAdd[schema.AdditionalCategories[j].Id]) {
                                    requiredCategories[schema.AdditionalCategories[j].Id] = schema.AdditionalCategories[j];
                                    requiredCategoriesList.push(schema.AdditionalCategories[j]);
                                }
                            }
                        }

                        for (j = 0; j < requiredCategoriesList.length; j++) {
                            var category = requiredCategoriesList[j];
                            linkingObject = new linkingObjectConstructor(this, position, category.Id, null, position.LinkingObjectType);
                            categoryNode = this.createCategoryNode(category, this.createNodeIdentity(this.nodeTypes.LinkingObject, position, linkingObject));
                            positionNode.children.push(categoryNode);
                            linkingObject.node = positionNode;
                        }

                        var addCategoryLinkNode = this.createAddCategoryLinkNode(this.createNodeIdentity(this.nodeTypes.AddCategoryLink, position));
                        positionNode.children.push(addCategoryLinkNode);
                    }
                    break;

                    // address and category     
                case linkingObjectTypes.AddressCategorySingle:
                case linkingObjectTypes.AddressCategoryMultiple:
                case linkingObjectTypes.AddressFirstLevelCategorySingle:
                case linkingObjectTypes.AddressFirstLevelCategoryMultiple:
                    for (j = 0; j < schema.FirmAddresses.length; j++) {
                        address = schema.FirmAddresses[j];
                        addressNode = this.createAddressNode(address, this.createNodeIdentity(this.nodeTypes.General, position));
                        addressNode.expanded = true;
                        positionNode.children.push(addressNode);

                        requiredCategories = [];
                        requiredCategoriesList = [];

                        for (var k = 0; k < address.Categories.length; k++) {
                            var categoryId = address.Categories[k];
                            var categoryLevel = categoriesMap[categoryId].Level;

                            if (position.LinkingObjectType == linkingObjectTypes.AddressCategoryMultiple && categoryLevel != 3)
                                continue;

                            if (position.LinkingObjectType == linkingObjectTypes.AddressCategorySingle && categoryLevel != 3)
                                continue;

                            if ((position.LinkingObjectType == linkingObjectTypes.AddressFirstLevelCategoryMultiple || position.LinkingObjectType == linkingObjectTypes.AddressFirstLevelCategorySingle) && categoryLevel != 1)
                                continue;

                            requiredCategories[categoriesMap[categoryId].Id] = categoriesMap[categoryId];
                            requiredCategoriesList.push(categoriesMap[categoryId]);
                        }

                        categoriesToAdd = [];
                        positionAdvertisements = this.advertisements.byPosition[position.Id];
                        if (positionAdvertisements) {
                            for (k = 0; k < positionAdvertisements.length; k++) {
                                if (positionAdvertisements[k].CategoryId && !requiredCategories[positionAdvertisements[k].CategoryId]) {
                                    categoriesToAdd[positionAdvertisements[k].CategoryId] = true;
                                }
                            }
                            for (k = 0; k < schema.AdditionalCategories.length; k++) {
                                if (categoriesToAdd[schema.AdditionalCategories[k].Id]) {
                                    requiredCategories[schema.AdditionalCategories[k].Id] = schema.AdditionalCategories[k];
                                    requiredCategoriesList.push(schema.AdditionalCategories[k]);
                                }
                            }
                        }

                        for (k = 0; k < requiredCategoriesList.length; k++) {
                            category = requiredCategoriesList[k];

                            linkingObject = new linkingObjectConstructor(this, position, category.Id, address.Id, position.LinkingObjectType);
                            categoryNode = this.createCategoryNode(category, this.createNodeIdentity(this.nodeTypes.LinkingObject, position, linkingObject));
                            categoryNode.disabled = address.IsDeleted;
                            linkingObject.node = categoryNode;
                            addressNode.children.push(categoryNode);
                        }

                        if (!address.IsDeleted) {
                            addCategoryLinkNode = this.createAddCategoryLinkNode(this.createNodeIdentity(this.nodeTypes.AddCategoryLink, position, null, address.Id));
                            addressNode.children.push(addCategoryLinkNode);
                        }
                    }
                    break;
                case linkingObjectTypes.ThemeMultiple:
                    Ext.each(schema.Themes, function (theme) {
                        linkingObject = new linkingObjectConstructor(this, position, null, null, theme.Id, position.LinkingObjectType);
                        var nodeId = this.createNodeIdentity(this.nodeTypes.LinkingObject, position, linkingObject);
                        var themeNode = this.createThemeNode(theme, nodeId);
                        positionNode.children.push(themeNode);
                        linkingObject.node = themeNode;

                    }, this);
                    break;
            }
            this.localData.positions.push(position);
        }

        this.removeEmptyNodes(rootConfig);

        var root = new window.Ext.tree.AsyncTreeNode(rootConfig);
        this.ui.treeGrid.setRootNode(root);
        this.renderCustomTreeNodes(root);

        this.setupColumnsWidths(document.body.clientWidth, document.body.clientHeight);

        this.localData.linkingObjects.forEach(function (obj) { obj.finalizeInitalization(); });

        this.ui.dumbCheckbox = document.createElement("input");
        this.ui.dumbCheckbox.type = 'checkbox';
        this.bindCheckboxes(this.ui.treeGrid.root, this.ui.dumbCheckbox);


    },

    createAddCategoryLinkNode: function (id) {
        return {
            type: 'addCategory',
            expanded: false,
            draggable: false,
            id: id,
            name: Ext.LocalizedResources.AddCategory,
            advertisement: null,
            leaf: true,
            children: [],
            dumb: '0'
        };
    },

    createThemeNode: function (theme, id) {
        return {
            id: id,
            type: 'theme',
            expanded: false,
            draggable: false,
            name: theme.Name,
            advertisement: null,
            leaf: true,
            children: [],
            dumb: '0'
        };
    },

    createCategoryNode: function (category, id) {
        return {
            id: id,
            type: 'category',
            expanded: false,
            draggable: false,
            name: category.Name,
            advertisement: null,
            leaf: true,
            children: [],
            dumb: '0'
        };
    },

    createWarningNode: function (warning, id) {
        return {
            id: id,
            type: 'warning',
            expanded: false,
            draggable: false,
            name: warning,
            advertisement: null,
            leaf: true,
            children: [],
            dumb: '0'
        };
    },

    createAddressNode: function (address, id) {
        return {
            id: id,
            type: 'address',
            draggable: false,
            name: this.getAddressName(address),
            disabled: address.IsDeleted,
            hiddenTemporarily: address.IsHidden,
            children: [],
            advertisement: null,
            dumb: '0'
        };
    },

    getAddressName: function (address) {
        if (address.IsHidden) {
            return String.format('<i>{0} ({1})</i>', address.Address, Ext.LocalizedResources.HiddenItem);
        }

        if (address.IsDeleted) {
            return String.format('<i>{0} ({1})</i>', address.Address, Ext.LocalizedResources.DeletedItem);
        }

        if (!address.IsLocatedOnTheMap) {
            return String.format('<i>{0} ({1})</i>', address.Address, Ext.LocalizedResources.AddressIsNotLocatedOnTheMap);
        }

        return address.Address;
    },

    createTree: function (rootConfig) {
        this.ui.AdvertisementLookups = [];

        var tree = new window.Ext.ux.tree.TreeGrid({
            title: Ext.LocalizedResources.Advertisements,
            width: 500,
            height: 300,
            plugins: [new window.Ext.ux.FitToParent(this.controls.targetDiv.dom.id)],
            enableDD: true,
            xtype: 'treepanel',
            stripeRows: true,
            renderTo: this.controls.targetDiv,
            columns: this.mode.getColumns.call(this)
        });

        tree.treeGridSorter.defaultSortFn = this.createCustomSortFn(tree.treeGridSorter.defaultSortFn);
        tree.treeGridSorter.sortFn = this.createCustomSortFn(tree.treeGridSorter.sortFn);

        tree.setRootNode(new window.Ext.tree.AsyncTreeNode(rootConfig));

        this.ui.treeGrid = tree;
        window.Ext.EventManager.onWindowResize(this.setupColumnsWidths, this);
    },

    createCustomSortFn: function (sortFn) {
        var nodeTypes = this.nodeTypes;
        var nodeIdentities = this.localData.nodeIdentities;
        var getSortIndex = function (node) {

            var nodeType = nodeIdentities[node.id].nodeType;

            if (nodeType == nodeTypes.AddCategoryLink) {
                return 3;
            }
            if (node.disabled) {
                return 2;
            }
            if (node.hiddenTemporarily) {
                return 1;
            }
            if (nodeType == nodeTypes.Warning) {
                return -1;
            }
            return 0;
        };
        return function (n1, n2) {
            var sortIndexOne = getSortIndex(n1);
            var sortIndexTwo = getSortIndex(n2);
            if (sortIndexOne == sortIndexTwo)
                return sortFn(n1, n2);
            return sortIndexOne - sortIndexTwo;
        };
    },

    removeEmptyNodes: function (node) {
        var nodeIdentity = this.localData.nodeIdentities[node.id];
        var counter = (nodeIdentity.nodeType == this.nodeTypes.LinkingObject || nodeIdentity.nodeType == this.nodeTypes.AddCategoryLink || nodeIdentity.nodeType == this.nodeTypes.Warning) ? 1 : 0;

        for (var i = 0; i < node.children.length; i++) {
            var activeLeaves = this.removeEmptyNodes(node.children[i]);
            if (activeLeaves) {
                counter += activeLeaves;
            } else {
                node.children.remove(node.children[i]);
                i--;
            }
        }
        return counter;
    },

    renderCustomTreeNodes: function (node) {
        var nodeIdentity = this.localData.nodeIdentities[node.id];
        if (nodeIdentity.nodeType == this.nodeTypes.AddCategoryLink) {
            if (this.localData.areLinkingObjectParametersLocked)
                node.remove();
            else
                this.renderAddCategoryLink(node);
        }
        else if (nodeIdentity.nodeType == this.nodeTypes.Warning) {
            this.renderWarning(node);
        }
        if (node.attributes.type == 'address') {
            this.renderAddressNode(node);
        }

        for (var i = 0; i < node.childNodes.length; i++) {
            this.renderCustomTreeNodes(node.childNodes[i]);
        }
    },

    renderAddressNode: function (node) {
        var ui = node.getUI();
        var textEl = ui.getTextEl();
        textEl.style.color = 'Black';
        textEl.getAttribute('style').cssText = textEl.getAttribute('style').cssText + ' !important';
    },

    renderAddCategoryLink: function (node) {
        var ui = node.getUI();
        ui.getIconEl().style.display = 'none';
        var textEl = ui.getTextEl();
        textEl.style.textDecorationUnderline = true;
        textEl.style.fontStyle = "italic";
        textEl.style.color = 'Blue';
        var self = this;
        ui.getAnchor().onclick = function () {
            self.addCategoryClick(node);
        };
    },

    renderWarning: function (node) {
        var ui = node.getUI();
        ui.getIconEl().style.display = 'none';
        var textEl = ui.getTextEl();
        textEl.style.color = 'Red';
        textEl.style.fontWeight = 'bold';
        textEl.getAttribute('style').cssText = textEl.getAttribute('style').cssText + ' !important';
    },

    setupColumnsWidths: function (width) {
        this.mode.setupWidth(this.ui.treeGrid.columns, width - this.ui.settings.widthDelta, this.ui.settings);
        this.ui.treeGrid.updateColumnWidths();
    },

    //Traverses the tree and binds a dumb checkbox to each node. This action is required by tree's internal logic.
    bindCheckboxes: function (node, dumbCheckbox) {
        if (node.isLeaf())
            node.getUI().checkbox = dumbCheckbox;
        else
            node.childNodes.forEach(function (child) { this.bindCheckboxes(child, dumbCheckbox); }, this);
    },

    reset: function () {
        if (!this.ui.treeGrid) {
            return;
        }

        this.ui.treeGrid.destroy();
        this.localData.linkingObjects.forEach(function (object) { object.destroy(); });
        this.localData.linkingObjects = [];
        this.localData.linkingObjectsByKey = [];

        this.localData.nextNodeId = 0;
    },

    /* Private methods - other */

    registerLinkingObject: function (linkingObject) {
        this.localData.linkingObjectsByKey[linkingObject.key] = linkingObject;
        this.localData.linkingObjects.push(linkingObject);
    },

    registerAdvertisements: function (advertisements) {
        this.advertisements.byKey = {};
        this.advertisements.byPosition = {};
        for (var i = 0; i < advertisements.length; i++) {
            var advertisement = advertisements[i];

            var key = this.makeLinkingObjectKey(advertisement.PositionId, advertisement.CategoryId, advertisement.FirmAddressId, advertisement.ThemeId);
            this.advertisements.byKey[key] = advertisement;
            if (!this.advertisements.byPosition[advertisement.PositionId]) {
                this.advertisements.byPosition[advertisement.PositionId] = [];
            }
            this.advertisements.byPosition[advertisement.PositionId].push(advertisement);
        }
    },

    gatherAdvertisements: function () {
        var advertisements = [];
        if (!this.schemaIsLoaded()) {
            return advertisements;
        }

        for (var i = 0; i < this.serverData.linkingObjectsSchema.Positions.length; i++) {
            var position = this.serverData.linkingObjectsSchema.Positions[i];
            for (var j = 0; j < position.LinkingObjects.length; j++) {
                var linkingObject = position.LinkingObjects[j];

                if (linkingObject.checkbox.checked) {
                    var advertisementId = null;

                    if (linkingObject.supportsAdvertisement() && linkingObject.isAdvertisementSelected()) {
                        advertisementId = linkingObject.getSelectedAdvertisement().id;
                    }

                    var linkingObjectTypes = Ext.DoubleGis.UI.OrderPosition.LinkingObjectTypes;

                    switch (position.LinkingObjectType) {
                        case linkingObjectTypes.AddressCategorySingle:
                        case linkingObjectTypes.AddressCategoryMultiple:
                        case linkingObjectTypes.AddressFirstLevelCategorySingle:
                        case linkingObjectTypes.AddressFirstLevelCategoryMultiple:
                            advertisements.push({
                                PositionId: position.Id,
                                CategoryId: linkingObject.categoryId,
                                FirmAddressId: linkingObject.firmAddressId,
                                AdvertisementId: advertisementId
                            });
                            break;

                        case linkingObjectTypes.Firm:
                            advertisements.push({
                                PositionId: position.Id,
                                AdvertisementId: advertisementId
                            });
                            break;

                        case linkingObjectTypes.AddressSingle:
                        case linkingObjectTypes.AddressMultiple:
                            advertisements.push({
                                PositionId: position.Id,
                                FirmAddressId: linkingObject.firmAddressId,
                                AdvertisementId: advertisementId
                            });
                            break;

                        case linkingObjectTypes.CategorySingle:
                        case linkingObjectTypes.CategoryMultiple:
                        case linkingObjectTypes.CategoryMultipleAsterix:
                            advertisements.push({
                                PositionId: position.Id,
                                CategoryId: linkingObject.categoryId,
                                AdvertisementId: advertisementId
                            });
                            break;
                        case linkingObjectTypes.ThemeMultiple:
                            advertisements.push({
                                PositionId: position.Id,
                                ThemeId: linkingObject.themeId,
                                AdvertisementId: advertisementId
                            });
                            break;
                    }
                }
            }
        }

        return advertisements;
    },

    validate: function () {
        var results = [];

        if (!this.schemaIsLoaded()) {
            return results;
        }

        var badPositions = [];
        var i;

        for (i = 0; i < this.serverData.linkingObjectsSchema.Positions.length; i++) {
            var position = this.serverData.linkingObjectsSchema.Positions[i];
            var found = false;
            for (var j = 0; j < position.LinkingObjects.length && !found; j++) {
                found |= position.LinkingObjects[j].isSelected();
            }
            if (!found) {
                badPositions.push(position);
            }
        }

        if (badPositions.length > 0) {
            var message;
            if (this.serverData.linkingObjectsSchema.Positions.length == 1) {
                message = Ext.LocalizedResources.AtLeastOneLinkingObjectMustBeSelected;
                results.push({ Level: 'CriticalError', Message: message });
            }
            // Теперь в пакетных позициях можно отключить всё что угодно (исключение - новая модель продаж)
            else if (this.localData.useSingleCategoryForPackage) {
                badPositions.sort(function (a, b) {
                    if (a.Name < b.Name) return -1;
                    if (a.Name > b.Name) return 1;
                    return 0;
                });

                message = Ext.LocalizedResources.AtLeastOneLinkingObjectMustBeSelectedForTheFollowingSubpositions;
                for (i = 0; i < badPositions.length; i++) {
                    if (i > 0) {
                        message += Ext.LocalizedResources.ListSeparator;
                    }
                    message += badPositions[i].Name;
                }
                results.push({ Level: 'CriticalError', Message: message });
            }
        }

        return results;
    },

    makeLinkingObjectKey: function (positionId, categoryId, firmAddressId, themeId) {
        return 'adv-' + positionId + '-' + categoryId + '-' + firmAddressId + '-' + themeId;
    },

    setLimitReached: function (newValue) {
        for (var index = 0; index < this.localData.linkingObjects.length; index++) {
            this.localData.linkingObjects[index].setupControlsAvailability();
        }
    },

    createNodeIdentity: function (nodeType, position, linkingObject, firmAddressId) {
        if (firmAddressId == undefined) {
            firmAddressId = null;
        }
        var identity = {
            nodeType: nodeType,
            position: position,
            firmAddressId: firmAddressId
        };
        if (nodeType == this.nodeTypes.LinkingObject) {
            identity.linkingObject = linkingObject;
        }

        this.localData.nodeIdentities.push(identity);
        return (this.localData.nodeIdentities.length - 1).toString();
    },

    addCategoryClick: function (node) {
        var nodeIndentity = this.localData.nodeIdentities[node.id];
        var position = nodeIndentity.position;
        var firmAddressId = nodeIndentity.firmAddressId;
        var positionNode = node.parentNode;

        var category = this.selectOrganizationUnitCategory(position);
        if (!category) {
            return;
        }

        var linkingObject;
        try {
            linkingObject = new window.Ext.DoubleGis.UI.OrderPosition.LinkingObject(this, position, category.Id, firmAddressId, position.LinkingObjectType);
        } catch (exc) {
            if (exc == "AlreadyExists") {
                alert(Ext.LocalizedResources.CategoryAlreasyExistsInTheList.replace("{0}", category.Name));
                return;
            } else {
                throw exc;
            }
        }

        var categoryNode = this.createCategoryNode(category, this.createNodeIdentity(this.nodeTypes.LinkingObject, position, linkingObject));
        linkingObject.node = categoryNode;
        positionNode.appendChild(categoryNode);
        linkingObject.finalizeInitalization();
        this.bindCheckboxes(this.ui.treeGrid.root, this.ui.dumbCheckbox);
    },

    selectOrganizationUnitCategory: function (position) {
        var linkingObjectTypes = Ext.DoubleGis.UI.OrderPosition.LinkingObjectTypes;

        var categoryLevel;
        if (position.LinkingObjectType == linkingObjectTypes.AddressFirstLevelCategorySingle || position.LinkingObjectType == linkingObjectTypes.AddressFirstLevelCategoryMultiple)
            categoryLevel = 1;
        else
            categoryLevel = 3;

        var extendedInfo = {
            OrganizationUnitId: this.localData.organizationUnitId.toString(),
            Level: categoryLevel,
            SalesModel: this.localData.salesModel
        }

        var url = "/Grid/Search/Category?" + "extendedInfo=" + encodeURIComponent(Ext.urlEncode(extendedInfo));

        var result = window.showModalDialog(url, null, 'status:no; resizable:yes; dialogWidth:900px; dialogHeight:500px; resizable: yes; scroll: no; location:yes;');
        return result ? result.items[0].data : null;
    },

    schemaIsLoaded: function () {
        return this.serverData.linkingObjectsSchema && this.serverData.linkingObjectsSchema.Positions;
    }
});
