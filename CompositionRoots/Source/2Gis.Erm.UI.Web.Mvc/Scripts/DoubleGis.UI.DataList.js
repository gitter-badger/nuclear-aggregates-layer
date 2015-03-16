Ext.ns("Ext.DoubleGis.UI");
Ext.DoubleGis.UI.DataList = Ext.extend(Ext.util.Observable, {
    Items: {},
    Utils: {},
    //конструктор объекта. В качестве параметра принимает все настройки листа и карточки.
    constructor: function (model) {
        this.addEvents("beforebuild", "afterbuild", "beforerebuild", "afterrebuild", "beforecreate", "beforeedit", "beforedelete", "beforeappend", "beforerefresh", "afterrefresh");
        if (model) {
            var urlComponents = location.pathname.split('/');

            // Removing last slash component
            if (urlComponents[urlComponents.length - 1] == '') {
                urlComponents.splice(urlComponents.length - 1, 1);
            }
            
            var parentEntityState = null;
            var parentEntityId = null;
            var parentEntityType = null;
            var appendedEntityType = null;
            
            // General route looks like this:
            // <URI>/Grid/{action}/{entityTypeName}/{parentEntityType}/{parentEntityId}/{parentEntityState}/{appendedEntityType}
            
            if (urlComponents.length == 7) {
                // {appendedEntityType} is absent
                
                parentEntityState = urlComponents[6] != "null" ? urlComponents[6] : "Active";
                parentEntityId = urlComponents[5];
                parentEntityType = urlComponents[4];
            }
            else if (urlComponents.length == 8) {
                // {appendedEntityType} is present

                appendedEntityType = urlComponents[7] != "null" ? urlComponents[7] : null;
                parentEntityState = urlComponents[6] != "null" ? urlComponents[6] : "Active";
                parentEntityId = urlComponents[5];
                parentEntityType = urlComponents[4];
            }

            model.DataViews = this.ExcludeHiddenViews(model.DataViews);
            if (parentEntityType && parentEntityId && model.DataViews.length > 0) {
                var newDataViews = [];
                for (var i = 0; i < model.DataViews.length; i++) {
                    var dataView = model.DataViews[i];
                    if (!dataView.HideInCardRelatedGrid) {
                        newDataViews.push(dataView);
                    }
                    else if (dataView.HideInCardRelatedGrid == "*") {

                    } else {
                        var prohibitedParents = dataView.HideInCardRelatedGrid.split(/[ ,]+/);
                        
                        if (prohibitedParents.indexOf(parentEntityType) == -1) {
                            newDataViews.push(dataView);
                        }
                    }
                }

                model.DataViews = newDataViews;
            }
            
            var decodedQueryString = window.Ext.urlDecode(window.location.search.substring(1));
            this.init(new Object(
                        {
                            defaultDataView: decodedQueryString.defaultDataView,
                            singleDataView: decodedQueryString.singleDataView,
                            extendedInfo: decodedQueryString.extendedInfo,
                            appendedEntity: appendedEntityType,
                            parentId: parentEntityId,
                            parentType: parentEntityType,
                            parentState: parentEntityState,
                            modelSettings: model
                        }));
            if (window.InitPage) {
                window.InitPage.createDelegate(this)();
            }
            this.Build();
        }
        else {
            Logger.HandleError("Не удалось получить настройки списка данных.", window.location, 0);
        }
    },
    init: function (model) {
        var currentDataView = null;
        if (model.defaultDataView) {
            currentDataView = model.modelSettings.DataViews.findOne(function (view) {
                return view.NameLocaleResourceId === model.defaultDataView;
            });
        }
        else if (model.singleDataView) {
            currentDataView = model.modelSettings.DataViews.findOne(function (view) {
                return view.NameLocaleResourceId === model.singleDataView;
            });
            if (currentDataView)
                model.modelSettings.DataViews = [currentDataView];
        }

        this.extendedInfo = model.extendedInfo;
        this.AppendedEntity = model.appendedEntity;
        this.ParentId = model.parentId;
        this.ParentState = model.parentState;
        this.ParentType = model.parentType;
        this.EntityModel = model.modelSettings;
        this.EntityName = model.modelSettings.EntityName;
        // До того, как была добавлена возможность указать представление по умочанию (или единственное), использовалось всегда первое.
        this.currentSettings = currentDataView || model.modelSettings.DataViews[0];
        this.ContentContainer = null;

        if (model.modelSettings.listeners) {
            var p, l = model.modelSettings.listeners;
            for (p in l) {
                if (window.Ext.isFunction(l[p])) {
                    this.on(p, l[p], this);
                }
            }
        }

        this.BuildContentPage();
    },
    BuildContentPage: function () {
        var northPanel = {
            id: "Toolbar",
            region: "north",
            height: 35,
            minSize: 35,
            maxSize: 35,
            collapsible: false,
            border: false,
            layout: "anchor",
            items: [{ id: "FilterBar", height: 35, border: false, header: false, contentEl: "FilterPanel" }]
        };

        var centerPanel = {
            id: "DataList",
            collapsible: false,
            layout: "anchor",
            region: "center",
            border: false,
            margins: "0 0 0 0"
        };

        this.ContentContainer = new window.Ext.Viewport({
            layout: "border",
            hideBorders: true,
            hideLabel: true,
            defaults: {
                split: false,
                bodyStyle: "padding:0px"
            },
            items: [northPanel, centerPanel]
        });
    },
    Build: function () {
        if (this.fireEvent("beforebuild", this) === false) {
            return;
        }
        
        var columns = [new Object({ id: "Image", width: 26, menuDisabled: true, renderer: { fn: window.Ext.DoubleGis.Global.Helpers.GridColumnHelper.RenderDefaultIcon, scope: this.currentSettings } })];
        var rdrFields = [];

        window.Ext.each(this.currentSettings.Fields, function (field) {
            columns.push(new Object({
                header: field.LocalizedName,
                width: field.Width,
                sortable: field.Sortable,
                dataIndex: field.Name,
                id: field.Name,
                hidden: field.Hidden,
                menuDisabled: true,
                css: (field.Name == this.currentSettings.MainAttribute) ? "background-color: #E3EFFF;" : field.Style,
                renderer: { fn: window.Ext.DoubleGis.Global.Helpers.GridColumnHelper.GetColumnRenderer(field), scope: field },
                align: field.align,
                xtype: field.xtype
            }));

            rdrFields.push(
                {
                    name: field.Name,
                    type: field.Type
                });
        }, this);
      
        this.BuildStore(rdrFields);
        //this.ApplyToolbarSettings();
        this.BuildGrid(columns);
        //this.Items.Store.load();
        this.ContentContainer.doLayout();
        this.ApplyToolbarHidingSettings();
        
        this.fireEvent("afterbuild", this);
        if (window.parent && window.parent.Card && window.parent.Card.fireAfterRelatedListReady) {
            window.parent.Card.fireAfterRelatedListReady(this);
        }
    },
    Rebuild: function (currentView) {
        if (this.fireEvent("beforerebuild", this) === false) {
            return;
        }
        
        this.currentSettings = currentView;
        var columns = [new Object(
            {
                id: "Image",
                width: 25,
                menuDisabled: true,
                renderer: {
                    fn: window.Ext.DoubleGis.Global.Helpers.GridColumnHelper.RenderDefaultIcon,
                    scope: this.currentSettings
                }
            })];

        var rdrFields = [];

        window.Ext.each(this.currentSettings.Fields, function (field) {
            columns.push(new Object({
                header: field.LocalizedName,
                width: field.Width,
                sortable: field.Sortable,
                dataIndex: field.Name,
                id: field.Name,
                hidden: field.Hidden,
                menuDisabled: true,
                css: (field.Name == this.currentSettings.MainAttribute) ? "background-color: #E3EFFF;" : field.Style,
                renderer: { fn: window.Ext.DoubleGis.Global.Helpers.GridColumnHelper.GetColumnRenderer(field), scope: field },
                align: field.align,
                xtype: field.xtype
            }));
            rdrFields.push(new Object({ name: field.Name, type: field.Type }));
        }, this);

        
        this.BuildStore(rdrFields);
        this.BuildGrid(columns);
        //this.Items.Store.load();
        this.ContentContainer.doLayout();
        this.ApplyToolbarHidingSettings();
        this.fireEvent("afterrebuild", this);
    },
    //Создание хранилища для данных (инициализиция колонок, конфигуряние адреса для запроса данных и т.д.)
    BuildStore: function (rdrFields) {
        this.Items.Store = new Ext.DoubleGis.Store({
            remoteSort: true,
            autoLoad: false,
            reader: new window.Ext.data.JsonReader({
                root: "Data",
                totalProperty: "RowCount",
                fields: rdrFields
            }),

            proxy: new window.Ext.data.HttpProxy({
                api: {
                    read: {
                        method: 'GET',
                        url: Ext.BasicOperationsServiceRestUrl + "List.svc/Rest/" + this.EntityName
                    }
                },
                timeout : 1200000
            }),
            baseParams: {
                start: 0,
                filterInput: "",
                extendedInfo: this.extendedInfo,
                nameLocaleResourceId: this.currentSettings.NameLocaleResourceId,
                limit: this.currentSettings.RowsPerPage,
                dir: this.currentSettings.DefaultSortDirection == 0 ? "ASC" : "DESC",
                sort: this.currentSettings.DefaultSortField,
                pId: this.ParentId,
                pType: this.ParentType
            },
            listeners: {
                exception: function (proxy, type, action, o, response, args) {
                    Ext.MessageBox.show({
                        title: Ext.LocalizedResources.Error,
                        msg: response.responseText || response.statusText,
                        buttons: Ext.MessageBox.OK,
                        icon: Ext.MessageBox.ERROR
                    });
                    this.removeAll(true);
                }
            }
        });

        var dataList = this;
        this.Items.Store.on("datachanged", function () {
            dataList.ApplyToolbarDisablingSettings();
            dataList.fireEvent("afterrefresh", this);
        });
    },
   
    //отрисовка самого грида
    BuildGrid: function (columns) {
        if (this.Items.Grid) {
            window.Ext.getCmp("DataList").remove(this.Items.Grid, true);
        }

        this.Items.Grid = new window.Ext.grid.GridPanel({
            anchor: "100%, 100%",
            stripeRows: true,
            autoScroll: true,
            border: true,
            loadMask: true,
            store: this.Items.Store,
            sortInfo: this.DefaultSortField,
            draggable: false,
            enableColumnMove: false,
            bbar: new window.Ext.PagingToolbar({
                pageSize: this.currentSettings.RowsPerPage,
                displayInfo: false,
                emptyMsg: "",
                store: this.Items.Store
            }),
            columns: columns,
            autoExpandColumn: this.currentSettings.Fields[this.currentSettings.Fields.length - 1].Name,
            autoExpandMin: this.currentSettings.Fields[this.currentSettings.Fields.length - 1].Width,
            selModel: new window.Ext.grid.RowSelectionModel({ singleSelect: !(this.currentSettings.AllowMultiple) }),
            tbar: this.BuildToolbar(),
            listeners:
                {
                    cellclick: { fn: this.onCellClick, scope: this }
                }
        });

        window.Ext.getCmp("DataList").add(this.Items.Grid);

        if (this.EntityModel.HasCard) {
            this.Items.Grid.addListener("RowDblClick", this.Edit, this);
        }

    },
    onCellClick: function (cmp, rowIndex, columnIndex, evt) {
        if (Ext.get(evt.target).hasClass('x-entity-link')) {
            var fieldNum = columnIndex - 1/*Смещение на 1 из-за отрисованной колонки с картинкой сущности*/;
            var fieldSet = this.currentSettings.Fields[fieldNum];
            var record = this.Items.Grid.getStore().getAt(rowIndex);
            if (record && record.data[fieldSet.ReferenceKeyField]) {
                this.openReferenceWindow(fieldSet.ReferenceTo, record.data[fieldSet.ReferenceKeyField]);
            }
        }
    },

    //Построение верхней панели с кнопками
    BuildToolbar: function () {
        for (var i = 0; i < this.currentSettings.ToolbarItems.length; i++) {
            this.currentSettings.ToolbarItems[i].Icon = this.currentSettings.ToolbarItems[i].Name == "Create" ? this.currentSettings.Icon : this.currentSettings.ToolbarItems[i].Icon;
        }
        return window.Ext.DoubleGis.Global.Helpers.ToolbarHelper.BuildToolbar(this.currentSettings.ToolbarItems, true, this);
    },
    
    // Скрытие элементов в тулбаре по названию.
    HideToolbarItems: function (scope, itemsCollection, buttonsToHide) {
        if (itemsCollection.length && buttonsToHide && buttonsToHide.length) {
            Ext.each(itemsCollection, function (item) {
                var itemLocal = item;
                if (buttonsToHide.indexOf(itemLocal.id) != -1) {
                    itemLocal.hide();
                }

                if (itemLocal.menu && itemLocal.menu.items && itemLocal.menu.items.length) {
                    scope.HideToolbarItems(scope, itemLocal.menu.items.items, buttonsToHide);
                }
            });
        }
    },
    
    RebuildPage: function () {
        window.Ext.each(this.EntityModel.DataViews,
            function (item) {
                if (item.NameLocaleResourceId == event.srcElement.value) {
                    this.Rebuild(item);
                }
            }, this);
    },

    //Функции, выполняющие операции по открытию всяческий карточек
    Append: function (options)
    {
        if (window.Ext.isNullOrDefault(this.AppendedEntity))
        {
            window.Ext.MessageBox.show({
                title: '',
                msg: Ext.LocalizedResources.ListDoesNotSupportAppend,
                width: 300,
                buttons: window.Ext.MessageBox.OK,
                icon: window.Ext.MessageBox.ERROR
            });
            this.ContentContainer.doLayout();
            return;
        }

        if (this.fireEvent("beforeappend", this) === false) {
            return;
        }

        var url = "/Grid/SearchMultiple/" + this.AppendedEntity + "/" + this.ParentType + "/" + this.ParentId;

        if (options && options.UrlParameters) {
            url = Ext.urlAppend(url, Ext.urlEncode(options.UrlParameters));
        }

        var result = window.showModalDialog(url, null, 'status:no; resizable:yes; dialogWidth:900px; dialogHeight:500px; resizable: yes; scroll: no; location:yes;');
        if (result) {
            var errors = ''; 
            for (var i = 0; i < result.items.length; i++) {
                var response = window.Ext.Ajax.syncRequest(
                    {
                        timeout: 1200000,
                        url: Ext.BasicOperationsServiceRestUrl + 'Append.svc/Rest/' + this.ParentType + '/' + this.ParentId + '/' + this.AppendedEntity + '/' + result.items[i].id,
                        method: "POST"
                    });
                var success = (response.conn.status >= 200 && response.conn.status < 300) || (Ext.isIE && response.conn.status == 1223);
                if (!success) {
                    var error = window.Ext.decode(response.conn.responseText);
                    Logger.HandleError(error.Message, window.location, 0);
                    
                    errors += error.Message + '<br/>';
                }
            }

            if (errors) {
                    window.Ext.MessageBox.show({
                        title: Ext.LocalizedResources.OperationFailed,
                        msg: errors, // fixme {all, 2014-09-30}: Вероятно, массив в качестве строкового параметра не сработает и отобрахится что-то типа [object Object]
                        buttons: window.Ext.MessageBox.OK,
                        icon: window.Ext.MessageBox.ERROR
                    });
                    this.ContentContainer.doLayout();
                }
            this.refresh();
        }
    },

    Create: function (settings) {
        var sUrl;
        var queryString = "";
        var params;

        if (!window.Ext.isNullOrDefault(this.AppendedEntity)) {
            params = String.format("width={0},height={1},status=no,resizable=yes,top={2},left={3}", window.Ext.DoubleGis.Global.UISettings.ActualCardWidth, window.Ext.DoubleGis.Global.UISettings.ActualCardHeight, window.Ext.DoubleGis.Global.UISettings.ScreenCenterTop, window.Ext.DoubleGis.Global.UISettings.ScreenCenterLeft);

            if (this.ParentType && this.ParentId) {
                queryString = '?pId=' + this.ParentId + '&pType=' + this.ParentType;
            }
            sUrl = Ext.DoubleGis.Global.Helpers.EvaluateCreateEntityUrl(this.AppendedEntity, queryString);
            window.open(sUrl, "_blank", params);
            return;
        }

        var overridenEntityName = settings ? settings.overridenEntityName : null;

        if (!this.EntityModel.HasCard && !overridenEntityName) {
            window.Ext.MessageBox.show({
                title: '',
                msg: window.Ext.LocalizedResources.CardIsUndefined,
                width: 300,
                buttons: window.Ext.MessageBox.OK,
                icon: window.Ext.MessageBox.ERROR
            });
            this.ContentContainer.doLayout();
            return;
        }

        if (this.fireEvent("beforecreate", this) === false) {
            return;
        }

        if (this.currentSettings.ReadOnly) {
            window.Ext.MessageBox.show({
                title: '',
                msg: window.Ext.LocalizedResources.ItsReadOnlyMode,
                width: 300,
                buttons: window.Ext.MessageBox.OK,
                icon: window.Ext.MessageBox.ERROR
            });
            this.ContentContainer.doLayout();
            return;
        }
        
        params = String.format("width={0},height={1},status=no,resizable=yes,top={2},left={3}", window.Ext.DoubleGis.Global.UISettings.ActualCardWidth, window.Ext.DoubleGis.Global.UISettings.ActualCardHeight, window.Ext.DoubleGis.Global.UISettings.ScreenCenterTop, window.Ext.DoubleGis.Global.UISettings.ScreenCenterLeft);
        if (this.ParentType && this.ParentId) {
            queryString = '?pId=' + this.ParentId + '&pType=' + this.ParentType;
        }

        sUrl = Ext.DoubleGis.Global.Helpers.EvaluateCreateEntityUrl(overridenEntityName ? overridenEntityName : this.EntityName, queryString);
        window.open(sUrl, "_blank", params);
    },
    Edit: function (arg) {
        if (this.currentSettings.DisableEdit || !window.Ext.isNullOrDefault(this.AppendedEntity)) {
            return;
        }

        var overridenEntityName = arg ? arg.overridenEntityName : null;

        if (!this.EntityModel.HasCard && !overridenEntityName) {
            window.Ext.MessageBox.show({
                title: '',
                msg: window.Ext.LocalizedResources.CardIsUndefined,
                width: 300,
                buttons: window.Ext.MessageBox.OK,
                icon: window.Ext.MessageBox.ERROR
            });

            return;
        }

        if (this.fireEvent("beforeedit", this) === false) {
            return;
        }

        if (this.Items.Grid.getSelectionModel().selections.items.length == 0) {
            return;
        }
        
        var val = this.Items.Grid.getSelectionModel().selections.items[0].data.Id;

        var queryString = "";
        if (this.ParentType && this.ParentId) {
            queryString += (queryString ? "&" : "?") + "pType=" + this.ParentType;
            queryString += (queryString ? "&" : "?") + "pId=" + this.ParentId;
        }
        
        if (this.currentSettings.ReadOnly) {
            queryString += (queryString ? "&" : "?") + "ReadOnly=" + this.currentSettings.ReadOnly;
        }

        var sUrl = Ext.DoubleGis.Global.Helpers.EvaluateUpdateEntityUrl(overridenEntityName ? overridenEntityName : this.EntityName, val, queryString);


        if (this.EntityName === 'Order') {
            RecalculateCardSize(0.9);
        }
        var params = String.format("width={0},height={1},status=no,resizable=yes,top={2},left={3}",
            window.Ext.DoubleGis.Global.UISettings.ActualCardWidth,
            window.Ext.DoubleGis.Global.UISettings.ActualCardHeight,
            window.Ext.DoubleGis.Global.UISettings.ScreenCenterTop,
            window.Ext.DoubleGis.Global.UISettings.ScreenCenterLeft);
        
        window.open(sUrl, "_blank", params);
    },

    GetSelectedItems: function () {
        var vals = [];

        window.Ext.each(this.Items.Grid.getSelectionModel().selections.items,
                    function (val) {
                        vals.push(val.data.Id);
                    });
        return vals;
    },

    EnsureOneSelected: function () {
        var selectedItems = this.Items.Grid.getSelectionModel().selections.items;
        if (selectedItems.length == 0 || selectedItems.length > 1) {
            window.Ext.MessageBox.show({
                title: '',
                msg: Ext.LocalizedResources.NeedToSelectOneItem,
                width: 300,
                buttons: window.Ext.MessageBox.OK,
                icon: window.Ext.MessageBox.ERROR
            });

            return false;
        }
        return true;
    },
    
    EnsureOneOrMoreSelected: function () {
        if (this.Items.Grid.getSelectionModel().selections.items.length == 0) {
            window.Ext.MessageBox.show({
                title: '',
                msg: Ext.LocalizedResources.MustSelectOneOrMoreObject,
                width: 300,
                buttons: window.Ext.MessageBox.OK,
                icon: window.Ext.MessageBox.ERROR
            });

            return false;
        }

        return true;
    },

    Delete: function (cmp, evt, doSpecialConfirmation) {

        if (this.Items.Grid.getSelectionModel().selections.items.length == 0) {
            window.Ext.MessageBox.show({
                title: '',
                msg: Ext.LocalizedResources.MustSelectOneOrMoreObject,
                width: 300,
                buttons: window.Ext.MessageBox.OK,
                icon: window.Ext.MessageBox.ERROR
            });
            this.ContentContainer.doLayout();
            return;
        }

        if (this.fireEvent("beforedelete", this) === false) {
            return;
        }
        var vals = [];

        window.Ext.each(this.Items.Grid.getSelectionModel().selections.items,
                    function (val) {
                        vals.push(val.data.Id);
                    });

        var parameters = {
            Values: vals,
            DoSpecialConfirmation: doSpecialConfirmation
        };


        var result = window.showModalDialog("/GroupOperation/Delete/" + this.EntityName, parameters, "dialogWidth:500px; dialogHeight:203px; scroll:no;resizable:no;");
        if (result == true) {
            this.refresh();
        }
    },

    DeleteConfirmed: function (cmp, evt) {
        this.Delete(cmp, evt, true);
    },

    refresh: function () {
        if (this.fireEvent("beforerefresh", this) === false) {
            return;
        }

        this.Items.Store.reload();
    },

    openReferenceWindow: function (entityName, id) {
        var params = String.format("width={0},height={1},status=no,resizable=yes,top={2},left={3}", window.Ext.DoubleGis.Global.UISettings.ActualCardWidth, window.Ext.DoubleGis.Global.UISettings.ActualCardHeight, window.Ext.DoubleGis.Global.UISettings.ScreenCenterTop, window.Ext.DoubleGis.Global.UISettings.ScreenCenterLeft);
        var queryString = this.currentSettings.ReadOnly ? '?readOnly=true' : '';
        var sUrl = Ext.DoubleGis.Global.Helpers.EvaluateUpdateEntityUrl(entityName, id, queryString);
        
        window.open(sUrl, '_blank', params);
    },
    
    ShowDialogWindowForOneOrMoreEntities: function (url, dialogParams) {
        if (!this.EnsureOneOrMoreSelected()) {
            return undefined;
        }

        return this.ShowDialogWindow(url, dialogParams, true);
    },

    ShowDialogWindow: function (url, dialogParams, appendSelectedItemsIds, donotRefresh) {
        var result = window.showModalDialog(url, appendSelectedItemsIds == true ? this.GetSelectedItems() : null, dialogParams);
        if (!donotRefresh) {
            this.refresh();
        }
        return result;
    },
    
    Activate: function () {
        if (this.Items.Grid.getSelectionModel().selections.items.length == 0) {
            window.Ext.MessageBox.show({
                title: '',
                msg: Ext.LocalizedResources.MustSelectOneOrMoreObject,
                width: 300,
                buttons: window.Ext.MessageBox.OK,
                icon: window.Ext.MessageBox.ERROR
            });
            this.ContentContainer.doLayout();
            return;
        }
        var vals = this.GetSelectedItems();
        var parameters = {
            Values: vals,
            DoSpecialConfirmation: null
        };
        var url = "/GroupOperation/Activate/" + this.EntityName;
        var result = window.showModalDialog(url, parameters, "dialogWidth:500px; dialogHeight:203px; scroll:no;resizable:no;");
        if (result == true) {
            this.refresh();
        }
    },
    Deactivate: function () {
        if (this.Items.Grid.getSelectionModel().selections.items.length == 0) {
            window.Ext.MessageBox.show({
                title: '',
                msg: Ext.LocalizedResources.MustSelectOneOrMoreObject,
                width: 300,
                buttons: window.Ext.MessageBox.OK,
                icon: window.Ext.MessageBox.ERROR
            });
            this.ContentContainer.doLayout();
            return;
        }
        var vals = this.GetSelectedItems();
        var parameters = {
            Values: vals,
            DoSpecialConfirmation: null
        };
        var url = "/GroupOperation/Deactivate/" + this.EntityName;
        var result = window.showModalDialog(url, parameters, "dialogWidth:500px; dialogHeight:350px; scroll:no;resizable:no;");
        if (result == true) {
            this.refresh();
        }
    },
    
    ApplyToolbarDisablingSettings: function () {
        // Подумать еще над производительностью решения с блокированием кнопок, если не выбрана ни одна строка
        var isEmpty = this.Items.Store.data.length == 0;
        var self = this;

        this.Items.Grid.getTopToolbar().items.each(function (item) {
            if (item.menu) {
                var disableMenu = true;
                item.menu.items.each(function (menuItem) {
                    if (menuItem.hidden) return;
                    self.SetDisabled(menuItem, isEmpty);
                    disableMenu = disableMenu && menuItem.disabled;
                });
                
                if (disableMenu) {
                    item.disable();
                } else {
                    item.enable();
                }
            } else {
                self.SetDisabled(item, isEmpty);
            }
        });
    },
    
    ApplyToolbarHidingSettings: function () {
        var toolbarItems = this.currentSettings.ToolbarItems;
        var buttonsToHide = [];
        for (var i = 0; i < toolbarItems.length; i++) {
            var hideInCardRelatedGrid = toolbarItems[i].HideInCardRelatedGrid;
            if (hideInCardRelatedGrid) {
                if (hideInCardRelatedGrid == '*' && this.ParentType) {
                    buttonsToHide.push(toolbarItems[i].Name);
                } else if (this.ParentType) {
                    var prohibitedParents = hideInCardRelatedGrid.split(/[ ,]+/);

                    if (prohibitedParents.indexOf(this.ParentType) != -1) {
                        buttonsToHide.push(toolbarItems[i].Name);
                    }
                }
            }
        }

        var tbar = this.Items.Grid.getTopToolbar();
        this.HideToolbarItems(this, tbar.items.items, buttonsToHide);
        tbar.doLayout();
    },
    
    SetDisabled: function (item, disabled) {
        if (item.initialConfig.disableOnEmpty) {
            if (!disabled && item.initialConfig.disabledInitially === false) {
                item.enable();
            }
            
            if (disabled) {
                item.disable();
            }
        }
    },

    ExcludeHiddenViews: function (views) {
        var result = [];
        Ext.each(views, function (view) {
            if (!view.IsHidden) {
                result.push(view);
            }
        });

        return result;
    }
});
