Ext.namespace('Ext.ux');
Ext.ux.SearchFormMultiple = Ext.extend(Ext.Panel, {
    isSearchForm: true,
    constructor: function (config)
    {
        config = config || {};
        this.addEvents("beforebuild", "afterbuild", "beforerefresh");
        this.extendedInfo = config.extendedInfo;
        this.entityModel = config.searchFormSettings;
        this.currentSettings = this.findDataView(config.nameLocaleResourceId);
        this.currentSettings.ReadOnly |= config.readOnly;
        this.existingItem = config.existingItem;

        Ext.apply(config, {
            layout: 'fit',
            headerCfg: {
                tagName: 'div',
                cls: 'modal-top-bar',
                html: '<div style="margin-bottom:5px;"><span class="title">' + Ext.LocalizedResources.SearchRecords + '</span></div><span class="regular">' + Ext.LocalizedResources.MultiSelectionListSearchInstructions + '</span>'
            },
            listeners: {
                afterrender: this.initHotKeys,
                afterbuild: this.selectSearchField,
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
                        this.selectionList = new Ext.ux.MultiSelectionList({
                            anchor: '100%,100%',
                            showCreateButton: this.currentSettings.ReadOnly,
                            viewSettings: this.currentSettings,
                            entityModel: this.entityModel,
                            extendedInfo: this.extendedInfo,
                            cls: 'gridPane'
                        })]
        });

        if (window.InitPage)
        {
            window.InitPage.createDelegate(this)();
        }
        if (this.fireEvent("beforebuild", this) === false)
        {
            return;
        }
        window.Ext.ux.SearchFormMultiple.superclass.constructor.call(this, config);

        this.fireEvent("afterbuild", this);
    },
    findDataView: function (nameLocaleResourceId) {
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
    refresh: function ()
    {
        this.selectionList.refresh();
    },
    saveChanges: function ()
    {
        window.returnValue = { items: this.selectionList.getValue() };
        window.close();
    },
    clearValue: function ()
    {
        window.returnValue = [];
        window.close();
    },
    cancel: function ()
    {
        window.close();
    }
});
