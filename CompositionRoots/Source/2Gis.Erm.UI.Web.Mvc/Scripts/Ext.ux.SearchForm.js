Ext.namespace('Ext.ux');
Ext.ux.SearchForm = Ext.extend(Ext.Panel, {
    isSearchForm: true,
    constructor: function (config)
    {
        config = config || {};

        this.addEvents("beforebuild", "afterbuild", "beforecreate", "beforeedit", "beforerefresh");
        this.extendedInfo = config.extendedInfo;
        this.entityModel = config.searchFormSettings;
        this.currentSettings = this.findDataView(config.nameLocaleResourceId);
        this.currentSettings.ReadOnly |= config.readOnly;
        this.existingItem = config.existingItem;


        this.initColumnSet();
        this.initStore();


        Ext.apply(config, {
            layout: 'fit',
            headerCfg: {
                tagName: 'div',
                cls: 'modal-top-bar',
                html: '<div style="margin-bottom:5px;"><span class="title">' + Ext.LocalizedResources.SearchRecords + '</span></div><span class="regular">' + Ext.LocalizedResources.SearchInstructions + '</span>'
            },
            listeners: {
                afterrender: this.initHotKeys,
                scope: this
            },
            headerAsText: false,
            bbarCfg: { tagName: 'div', cls: 'x-modal-toolbar' },
            bbar: [
                        {
                            xtype: 'tbfill'
                        },
                        {
                            id: 'btnSave',
                            xtype: 'tabularbutton',
                            text: Ext.LocalizedResources.OK,
                            handler: this.saveChanges,
                            scope: this
                        },
                        {
                            id: 'btnCancel',
                            xtype: 'tabularbutton',
                            text: Ext.LocalizedResources.Cancel,
                            handler: this.cancel,
                            scope: this
                        },
                        {
                            id: 'btnClear',
                            xtype: 'tabularbutton',
                            text: Ext.LocalizedResources.DeleteValue,
                            handler: this.clearValue,
                            width: 112,
                            disabled: this.existingItem && this.existingItem.items ? false : true,
                            scope: this
                        }
                    ],
            items: [
                        new Ext.Panel({
                            layout: 'anchor',
                            id: 'gridPane',
                            bbarCfg: { tagName: 'div', cls: 'x-modal-toolbar' },
                            bbar: [
                                    {
                                        id: 'btnProp',
                                        xtype: 'tabularbutton',
                                        text: Ext.LocalizedResources.Properties,
                                        handler: this.editItem,
                                        scope: this
                                    },
                                    {
                                        id: 'btnCreate',
                                        xtype: 'tabularbutton',
                                        text: Ext.LocalizedResources.Create,
                                        handler: this.createItem,
                                        disabled: this.currentSettings.ReadOnly,
                                        scope: this
                                    }
                                ],
                            headerCfg: {
                                tagName: 'div',
                                cls: 'search-header',
                                html: '<div style="width: 50%; float: left;"><div style="font-weight: bold; width: 50px; float: left; padding-top: 3px;">' + Ext.LocalizedResources.Search + '</div><div style="float: left;"><select class="inputfields" disabled="disabled"></select></div></div><div style="width: 49%; float: right; margin-top: -1px;"><div style="width: 10px; float: left;"></div><div style="float: left;"><input type="text" class="inputfields x-search" id="searchInput" unselectable="off"/></div></div><div style="clear:both;border-bottom:#cccccc 1px solid;"></div>'
                            },
                            headerAsText: false,
                            listeners: {
                                afterrender: this.initPaneToolbar,
                                scope: this
                            },
                            items: [
                                    this.grid = new window.Ext.grid.GridPanel(
                                        {
                                            id: 'searchGrid',
                                            anchor: '100%, 100%',
                                            stripeRows: true,
                                            autoScroll: true,
                                            border: true,
                                            loadMask: true,
                                            store: this.store,
                                            sortInfo: this.currentSettings.DefaultSortField,
                                            //margins: {top:0, right:5, bottom:0, left:5},
                                            bbar: new window.Ext.PagingToolbar(
                                                {
                                                    pageSize: this.currentSettings.RowsPerPage,
                                                    displayInfo: false,
                                                    emptyMsg: "",
                                                    store: this.store
                                                }),
                                            columns: this.columnSet,
                                            autoExpandColumn: this.currentSettings.Fields[this.currentSettings.Fields.length - 1].Name,
                                            selModel: new window.Ext.grid.RowSelectionModel({ singleSelect: true }),
                                            listeners: {
                                                rowclick: this.checkGrid,
                                                rowdblclick: this.saveChanges,
                                                cellclick: this.onCellClick,
                                                scope: this
                                            }
                                        })]
                        })
                        ]
        });

        if (window.InitPage)
        {
            window.InitPage.createDelegate(this)();
        }
        if (this.fireEvent("beforebuild", this) === false)
        {
            return;
        }
        window.Ext.ux.SearchForm.superclass.constructor.call(this, config);

        this.fireEvent("afterbuild", this);
    },
    findDataView: function(nameLocaleResourceId) {
        if (nameLocaleResourceId) {
            for (var index in this.entityModel.DataViews) {
                var dataView = this.entityModel.DataViews[index];
                if (dataView.NameLocaleResourceId === nameLocaleResourceId) {
                    return dataView;
                }
            }
            Logger.HandleError("Can't find data view " + nameLocaleResourceId, window.location, 0);
        }

        return this.entityModel.DataViews[0];
    },
    initHotKeys: function (cmp)
    {
        cmp.el.on("keypress", function (e)
        {
            if (e.keyCode == e.ESC)
            {
                this.cancel();
            }
            else if (e.keyCode == e.ENTER)
            {
                this.saveChanges();
            }
        }, this);
    },
    initPaneToolbar: function (cmp)
    {
        var s = new Ext.ux.SearchControl({ applyTo: 'searchInput' });
        s.on('trigger', this.searchRecords, this);
        s.setValue(Ext.urlDecode(location.search.substring(1)).search || '').trigger();
        cmp.header.setStyle('-moz-user-select', 'text');
        cmp.header.removeClass('x-unselectable');
        cmp.header.dom.unselectable = "off";
        cmp.header.removeAllListeners();
    },
    initColumnSet: function ()
    {
        this.columnSet = [new Object({ id: "Image", width: 25, menuDisabled: true, renderer: { fn: window.Ext.DoubleGis.Global.Helpers.GridColumnHelper.RenderDefaultIcon, scope: this.currentSettings } })];
        this.rdrFieldSet = [];
        window.Ext.each(this.currentSettings.Fields, function (field, i)
        {
            this.columnSet.push(new Object({ id: field.Name,
                header: field.LocalizedName,
                width: field.Width,
                sortable: true,
                dataIndex: field.Name,
                hidden: field.Hidden,
                menuDisabled: true,
                css: (field.Name == this.currentSettings.MainAttribute) ? "background-color: #E3EFFF;" : field.Style,
                renderer: { fn: window.Ext.DoubleGis.Global.Helpers.GridColumnHelper.GetColumnRenderer(field), scope: field }
            }));
            this.rdrFieldSet.push(new Object({ name: field.Name, type: field.Type }));
        }, this);
    },
    initStore: function ()
    {
        var qstringparams = window.Ext.urlDecode(location.search.substring(1));

        this.store = new Ext.DoubleGis.Store({
            remoteSort: true,
            //Ставим false потому, что при загрузке страницы будет присобачен фильтр
            autoLoad: false,
            reader: new window.Ext.data.JsonReader({
                root: 'Data',
                totalProperty: 'RowCount',
                fields: this.rdrFieldSet
            }),
            proxy: new window.Ext.data.HttpProxy({
                method: "GET",
                url: Ext.BasicOperationsServiceRestUrl + "List.svc/Rest/" + this.entityModel.EntityName,
                timeout: 1200000
            }),
            baseParams: new Object({
                nameLocaleResourceId: this.currentSettings.NameLocaleResourceId,
                extendedInfo: this.extendedInfo,
                filterInput: "",
                start: 0,
                pId: qstringparams.pId,
                pType: qstringparams.pType,
                limit: this.currentSettings.RowsPerPage,
                sort: qstringparams.defaultSortFields || this.currentSettings.DefaultSortField,
                dir: qstringparams.defaultSortFieldsDirs || (this.currentSettings.DefaultSortDirection == 0 ? "ASC" : "DESC" )
            }),
            listeners:
                            {
                                exception: function (e)
                                {
                                    Ext.MessageBox.show({

                                        title: Ext.LocalizedResources.GetDataError,
                                        msg: Ext.LocalizedResources.ErrorDuringOperation,
                                        buttons: Ext.MessageBox.OK,
                                        icon: Ext.MessageBox.ERROR
                                    });
                                    this.grid.store.removeAll(true);
                                },
                                //load: this.selectDefaultRow,
                                load: this.selectSearchField,
                                scope: this
                            }
        });
    },
  
    onCellClick: function (cmp, rowIndex, columnIndex, evt)
    {
        if (Ext.get(evt.target).hasClass('x-entity-link'))
        {
            var fieldNum = columnIndex - 1/*Смещение на 1 из-за отрисованной колонки с картинкой сущности*/;
            var fieldSet = this.currentSettings.Fields[fieldNum];
            var record = this.grid.getStore().getAt(rowIndex);
            if (record && record.data[fieldSet.ReferenceKeyField])
            {
                this.openReferenceWindow(fieldSet.ReferenceTo, record.data[fieldSet.ReferenceKeyField]);
            }
        }
    },
    openReferenceWindow: function (entityName, id)
    {
        var params = String.format("width={0},height={1},status=yes,resizable=yes,top={2},left={3}", window.Ext.DoubleGis.Global.UISettings.ActualCardWidth, window.Ext.DoubleGis.Global.UISettings.ActualCardHeight, window.Ext.DoubleGis.Global.UISettings.ScreenCenterTop, window.Ext.DoubleGis.Global.UISettings.ScreenCenterLeft);
        var queryString = '?ReadOnly=false';
        var sUrl = Ext.DoubleGis.Global.Helpers.EvaluateUpdateEntityUrl(entityName, id, queryString);
        window.open(sUrl, '_blank', params);
    },
    checkGrid: function ()
    {
        if (this.grid.getSelectionModel().selections.length == 0 || this.grid.getSelectionModel().selections.length > 1)
        {
            window.Ext.getCmp("btnProp").disable();
            window.Ext.getCmp("btnSave").disable();
        }
        else
        {
            if (this.EntityModel && this.EntityModel.HasCard)
            {
                window.Ext.getCmp("btnProp").enable();
            }
            window.Ext.getCmp("btnSave").enable();
        }
    },  
    selectSearchField: function () {
        Ext.get('searchInput').focus();
    },
    selectDefaultRow: function (store, records)
    {
        if (this.existingItem && this.existingItem.items && this.existingItem.items.length)
        {
            var item = this.existingItem.items[0];
            for (var i = 0; i < records.length; i++)
            {
                if (records[i].data["Id"] == item.id && records[i].data[this.currentSettings.MainAttribute] == item.name)
                {
                    this.grid.getSelectionModel().selectRecords([records[i]]);
                    this.grid.getView().focusRow(i);
                }
            }
        }
        this.checkGrid();
    },
    saveChanges: function ()
    {
        if (this.grid.getSelectionModel().selections.items.length) {

            // очистка name от разметки
            var name = this.grid.getSelectionModel().selections.items[0].data[this.currentSettings.MainAttribute];
            var spanTag = document.createElement("span");
            spanTag.innerHTML = name;
            name = spanTag.innerText;

            var item =
                    {
                        id: this.grid.getSelectionModel().selections.items[0].data.Id,
                        name: name,
                        data: this.grid.getSelectionModel().selections.items[0].data
                    };
            window.returnValue = { items: [item] };
            window.close();
        }
        else
        {
            window.Ext.MessageBox.show({
                title: '',
                msg: Ext.LocalizedResources.MustSelectOneOrMoreObject,
                width: 300,
                buttons: window.Ext.MessageBox.OK,
                icon: window.Ext.MessageBox.ERROR
            });
            return;
        }
    },
    clearValue: function ()
    {
        window.returnValue = [];
        window.close();
    },
    cancel: function ()
    {
        window.close();
    },
    editItem: function ()
    {
        if (this.fireEvent("beforeedit", this) === false)
        {
            return;
        }

        if (this.EntityModel && !this.EntityModel.HasCard)
        {
            window.Ext.MessageBox.show({
                title: '',
                msg: window.Ext.LocalizedResources.CardIsUndefined,
                width: 300,
                buttons: window.Ext.MessageBox.OK,
                icon: window.Ext.MessageBox.ERROR
            });
            return;
        }
        
        var selectedItems = this.grid.getSelectionModel().selections.items;
        if (selectedItems.length != 1) {
            return;
        }
        
        var parentExp = "";
        var qstringparams = window.Ext.urlDecode(location.search.substring(1));
        if (qstringparams.pId && qstringparams.pType)
        {
            parentExp = String.format("&pId={0}&pType={1}", qstringparams.pId, qstringparams.pType);
        }
        
        var params = String.format("width={0},height={1},status=no,resizable=yes,top={2},left={3}", window.Ext.DoubleGis.Global.UISettings.ActualCardWidth, window.Ext.DoubleGis.Global.UISettings.ActualCardHeight, window.Ext.DoubleGis.Global.UISettings.ScreenCenterTop, window.Ext.DoubleGis.Global.UISettings.ScreenCenterLeft);
        var queryString = '?ReadOnly=' + this.currentSettings.ReadOnly + parentExp;
        var sUrl = Ext.DoubleGis.Global.Helpers.EvaluateUpdateEntityUrl(this.entityModel.EntityName, selectedItems[0].data.Id, queryString);
        window.open(sUrl, '_blank', params);
    },
    createItem: function ()
    {
        if (this.fireEvent("beforecreate", this) === false)
        {
            return;
        }

        var parentExp = "";
        var qstringparams = window.Ext.urlDecode(location.search.substring(1));
        if (qstringparams.pId && qstringparams.pType)
        {
            parentExp = String.format("&pId={0}&pType={1}", qstringparams.pId, qstringparams.pType);
        }
        if (this.EntityModel && !this.EntityModel.HasCard)
        {
            window.Ext.MessageBox.show({
                title: '',
                msg: window.Ext.LocalizedResources.CardIsUndefined,
                width: 300,
                buttons: window.Ext.MessageBox.OK,
                icon: window.Ext.MessageBox.ERROR
            });
            return;
        }
        if (this.currentSettings.ReadOnly)
        {
            window.Ext.MessageBox.show({
                title: '',
                msg: window.Ext.LocalizedResources.ItsReadOnlyMode,
                width: 300,
                buttons: window.Ext.MessageBox.OK,
                icon: window.Ext.MessageBox.ERROR
            });
            return;
        }
        
        var params = String.format("width={0},height={1},status=no,resizable=yes,top={2},left={3}", window.Ext.DoubleGis.Global.UISettings.ActualCardWidth, window.Ext.DoubleGis.Global.UISettings.ActualCardHeight, window.Ext.DoubleGis.Global.UISettings.ScreenCenterTop, window.Ext.DoubleGis.Global.UISettings.ScreenCenterLeft);
        var queryString = '?ReadOnly=' + this.currentSettings.ReadOnly + parentExp + "&extendedInfo=" + encodeURIComponent(this.extendedInfo);
        var sUrl = Ext.DoubleGis.Global.Helpers.EvaluateCreateEntityUrl(this.entityModel.EntityName, queryString);
        window.open(sUrl, '_blank', params);
    },
    refresh: function ()
    {
        if (this.fireEvent("beforerefresh", this) === false)
        {
            return;
        }
        this.grid.store.reload();
    },
    searchRecords: function (el, value)
    {
        this.grid.store.setBaseParam("filterInput", value);
        this.grid.store.setBaseParam("extendedInfo", this.extendedInfo);
        this.grid.getBottomToolbar().changePage(1);
    }
});
