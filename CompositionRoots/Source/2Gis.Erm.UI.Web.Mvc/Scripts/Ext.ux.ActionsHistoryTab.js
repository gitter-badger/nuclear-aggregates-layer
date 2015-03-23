Ext.namespace('Ext.ux');
Ext.ux.ActionsHistoryTab = Ext.extend(Ext.Panel, {
    tabGroupRenderer: Ext.LocalizedResources.ActionsHistoryTabGroupRenderer,
    tabGroupTemplate: Ext.LocalizedResources.ActionsHistoryTabGroupTemplate,    

    constructor: function(config) {
        config = config || {};
        config.title = Ext.LocalizedResources.ActionsHistory;
        config.id = "actionsTab";
        config.listeners = {
            beforeshow: { fn: this.beforeShow, scope: this }
        };
        config.items = [];
        Ext.ux.ActionsHistoryTab.superclass.constructor.call(this, config);
    },
    // #region routines
    beforeShow: function() {
        window.Card.Mask.show();

        if (this.contentRendered) {
            this.loadStoreData(this.store, this.pCardInfo);
        } else {
            this.renderContent();
        }
    },
    renderContent: function() {
        this.add(new window.Ext.Panel(
        {
            layout: 'fit',
            plugins: [new window.Ext.ux.FitToParent("actionsTab")],
            items: [this.renderActionsList(this.pCardInfo)]
        }));
        this.doLayout();
        this.contentRendered = true;
    },
    renderActionsList: function(cardCfg) {
        this.reader = new Ext.data.JsonReader({
            totalProperty: 'RowCount',
            root: 'ActionHistoryDetailsData',
            fields: [
                { name: 'id', mapping: 'Id' },
                { name: 'propertyName', mapping: 'PropertyName' },
                { name: 'originalValue', convert: function(value, record) {
                    var date = Date.parseDate(record.OriginalValue, Ext.util.Format.dateTimePattern);
                    if (date) {
                        return Ext.util.Format.dateWOffset(date);
                    }
                    return record.OriginalValue;
                }  },
                {
                    name: 'modifiedValue', convert: function (value, record) {
                        var date = Date.parseDate(record.ModifiedValue, Ext.util.Format.dateTimePattern);
                        if (date) {
                            return Ext.util.Format.dateWOffset(date);
                        }
                        return record.ModifiedValue;
                    }
                },
                { name: 'actionsHistoryId', mapping: 'ActionsHistoryId' },
                { name: 'actionType', mapping: 'ActionType' },
                { name: 'createdBy', mapping: 'CreatedBy' },
                { name: 'createdOn', mapping: 'CreatedOn' }
            ]
        });
        this.store = new Ext.data.GroupingStore({
            reader: this.reader,
            remoteSort: true,
            sortInfo: { field: 'propertyName', direction: "ASC" },
            groupDir: "DESC",
            groupField: 'actionsHistoryId',
            listeners: {
                load:
                {
                    fn: function() {
                        window.Card.Mask.hide();
                    }
                },
                exception:
                {
                    fn: function(p, m, t, req, resp) {
                        Ext.Msg.alert(Ext.LocalizedResources.Error, resp.responseText || resp.statusText);
                    }
                }
            }
        });

        this.loadStoreData(this.store, cardCfg);

        this.grid = new Ext.grid.GridPanel({
            store: this.store,
            columns:
            [
                { id: 'propertyName', header: Ext.LocalizedResources.PropertyName, width: 60, sortable: true, dataIndex: 'propertyName', menuDisabled: true },
                { id: 'originalValue', header: Ext.LocalizedResources.OriginalValue, width: 60, sortable: true, dataIndex: 'originalValue', menuDisabled: true },
                { id: 'modifiedValue', header: Ext.LocalizedResources.ModifiedValue, width: 60, sortable: true, dataIndex: 'modifiedValue', menuDisabled: true },
                {
                    id: 'actionsHistoryId',
                    header: "actionsHistoryId",
                    dataIndex: 'actionsHistoryId',
                    hidden: true,
                    groupRenderer: function(v, unused, r, rowIndex, colIndex, ds) {
                        var ahRow = {};
                        for (var i = 0; i < ds.reader.jsonData.ActionHistoryData.length; i++)
                            if (ds.reader.jsonData.ActionHistoryData[i].Id == r.data.actionsHistoryId) {
                                ahRow = ds.reader.jsonData.ActionHistoryData[i];
                                break;
                            }

                        var parsedCreatedOn = Ext.isDate(ahRow.CreatedOn) ? ahRow.CreatedOn : Date.parseDate(ahRow.CreatedOn, 'M$');
                        return String.format(Ext.ux.ActionsHistoryTab.prototype.tabGroupRenderer,
                            ahRow.ActionType,
                            ahRow.CreatedBy,
                            Ext.util.Format.dateWOffset(parsedCreatedOn));
                    }
                },
                { id: 'actionType', header: "actionType", dataIndex: 'actionType', hidden: true },
                { id: 'createdBy', header: "createdBy", dataIndex: 'createdBy', hidden: true },
                { id: 'createdOn', header: "createdOn", dataIndex: 'createdOn', hidden: true }
            ],

            view: new Ext.grid.GroupingView({
                forceFit: true,
                groupTextTpl: this.tabGroupTemplate,
                startCollapsed: true
            })
        });
        return this.grid;
    },
    loadStoreData: function (store, cardCfg) {
        var url = Ext.BasicOperationsServiceRestUrl + 'ActionsHistory.svc/Rest/' + cardCfg.pTypeName + "/" + cardCfg.pId;
        url = Ext.urlAppend(url, '_dc=' + (new Date().getTime()));

        var storeScope = store;
        var actionsHistoryTabScope = this;
        var xmlhttp = new XMLHttpRequest();
        xmlhttp.withCredentials = true;
        xmlhttp.open('GET', url, true);

        xmlhttp.onreadystatechange = function () {
            if (xmlhttp.readyState == 4) {
                if (xmlhttp.status == 200) {
                    storeScope.loadData(Ext.decode(xmlhttp.responseText));
                } else if (xmlhttp.responseText || xmlhttp.statusText) {
                    actionsHistoryTabScope.showError(xmlhttp.responseText || xmlhttp.statusText);
                }
            }
        };
        xmlhttp.send();
    },
    showError: function(error) {
        var errorInfo = Ext.decode(error);
        if (errorInfo && errorInfo.Message) {
            error = errorInfo.Message;
        }

        window.Ext.MessageBox.show({
            title: '',
            msg: error,
            width: 300,
            buttons: window.Ext.MessageBox.OK,
            icon: window.Ext.MessageBox.ERROR
        });
    }
});

Ext.reg('actionshistorytab', Ext.ux.ActionsHistoryTab);
