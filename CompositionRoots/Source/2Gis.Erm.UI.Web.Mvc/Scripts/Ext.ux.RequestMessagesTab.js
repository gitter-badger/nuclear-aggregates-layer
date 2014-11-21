Ext.namespace('Ext.ux');
Ext.ux.RequestMessagesTab = Ext.extend(Ext.Panel, {
    tabGroupRenderer: Ext.LocalizedResources.OrderRequestMessagesTabGroupRenderer,
    tabGroupTemplate: Ext.LocalizedResources.OrderRequestMessagesTabGroupTemplate,

    constructor: function (config) {
        config = config || {};
        config.title = Ext.LocalizedResources.OrderRequestMessagesTabTitle;
        config.id = "messagesTab";
        config.listeners = {
            beforeshow: { fn: this.beforeShow, scope: this }
        };
        config.items = [];
        Ext.ux.RequestMessagesTab.superclass.constructor.call(this, config);
    },
    // #region routines
    beforeShow: function () {
        window.Card.Mask.show();

        if (this.contentRendered) {
            this.store.reload();
        }
        else {
            this.renderContent();
        }
    },
    renderContent: function () {
        this.add(new window.Ext.Panel(
            {
                layout: 'fit',
                plugins: [new window.Ext.ux.FitToParent("messagesTab")],
                items: [this.renderMessagesList(this.pCardInfo)]
            }));
        this.doLayout();
        this.contentRendered = true;
    },
    renderMessagesList: function (cardCfg) {
        this.reader = new Ext.data.JsonReader({
            totalProperty: 'RowCount',
            fields: [
                { name: 'id', mapping: 'Id' },
                { name: 'messageType', mapping: 'MessageType' },
                { name: 'messageText', mapping: 'MessageText' },
                { name: 'requestId', mapping: 'RequestId' },
                { name: 'groupId', mapping: 'GroupId' },
                { name: 'createdBy', mapping: 'CreatedBy' },
                { name: 'createdOn', mapping: 'CreatedOn' }
            ]
        });
        this.store = new Ext.data.GroupingStore({
            reader: this.reader,
            autoLoad: true,
            proxy: new Ext.data.HttpProxy({
                method: 'GET',
                url: '/OrderProcessingRequest/GetMessages?orderProcessingRequestId=' + cardCfg.pId
            }),
            remoteSort: true,
            sortInfo: { field: 'id', direction: "ASC" },
            groupDir: "DESC",
            groupField: 'groupId',
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
        this.grid = new Ext.grid.GridPanel({
            store: this.store,
            columns:
                [
                    {
                        id: 'messageType', header: Ext.LocalizedResources.MessageType, width: 20, sortable: true, dataIndex: 'messageType', menuDisabled: true,
                        renderer: function (value, metadata, record, rowIndex, colIndex, store) {
                            metadata.attr = 'ext:qtip="' + record.data.messageText.replace("\"", "&quot;") + '"';
                            return value;
                        }
                    },
                    {
                        id: 'messageText', header: Ext.LocalizedResources.MessageText, width: 90, sortable: true, dataIndex: 'messageText', menuDisabled: true,
                        renderer: function (value, metadata, record, rowIndex, colIndex, store) {
                            metadata.attr = 'ext:qtip="' + value.replace("\"", "&quot;") + '"';
                            return value;
                        }
                    },
                    { id: 'groupId', header: "groupId", dataIndex: 'groupId', hidden: true,
                    groupRenderer: function (v, unused, r, rowIndex, colIndex, ds) {
                            var parsedCreatedOn = Ext.isDate(r.data.createdOn) ? r.data.createdOn : Date.parseDate(r.data.createdOn, 'M$');
                            return String.format(Ext.ux.RequestMessagesTab.prototype.tabGroupRenderer,
                                r.data.createdBy,
                                Ext.util.Format.dateWOffset(parsedCreatedOn));
                        }
                    },
                    { id: 'createdOn', header: "createdOn", dataIndex: 'createdOn', hidden: true },
                    { id: 'createdBy', header: "createdBy", dataIndex: 'createdBy', hidden: true },
                    { id: 'requestId', header: "requestId", dataIndex: 'requestId', hidden: true },
                    { id: 'id', header: "id", dataIndex: 'id', hidden: true }
                ],
            view: new Ext.grid.GroupingView({
                forceFit: true,
                groupTextTpl: this.tabGroupTemplate,
                startCollapsed: true
            })
        });
        this.grid.on({
            rowdblclick: function (grid, rowindex, e) {
                var record = grid.getStore().getAt(rowindex);

                var messageText = record.get('messageText');
                
                Ext.MessageBox.show({
                    title: Ext.LocalizedResources.MessageText,
                    msg: messageText,
                    buttons: Ext.MessageBox.OK,
                    icon: Ext.MessageBox.INFO
                });
            }
        });

        return this.grid;
    }
});

Ext.reg('requestmessagestab', Ext.ux.RequestMessagesTab);
