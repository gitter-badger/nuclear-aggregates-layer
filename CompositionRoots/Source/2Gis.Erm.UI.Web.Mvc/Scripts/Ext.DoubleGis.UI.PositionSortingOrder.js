Ext.namespace('Ext.DoubleGis.UI');
Ext.DoubleGis.UI.PositionSortingOrder = Ext.extend(Ext.Panel, {
    constructor: function (config) {
        config = config || {};
        config.title = 'Позиции';
        config.id = "actionsTab";
        config.items = [this.renderGrid()];
        config.plugins = [new window.Ext.ux.FitToParent("actionsTab")];
        config.layout = 'fit';
        Ext.DoubleGis.UI.PositionSortingOrder.superclass.constructor.call(this, config);
    },
    renderGrid: function () {
        this.proxy = new Ext.data.HttpProxy({
            api: {
                read: { url: '/Price/PositionSortingOrderData', method: 'GET' },
                update: { url: '/Price/PositionSortingOrderData', method: 'POST' }
            }
        });

        this.writer = new Ext.data.JsonWriter({
            writeAllFields: true,
            listful: true
        });

        this.reader = new Ext.data.JsonReader({
            idProperty: 'Id',
            root: 'Records',
            successProperty: 'Success'
        }, [
            { name: 'name', mapping: 'Name' },
            { name: 'id', mapping: 'Id' },
            { name: 'index', mapping: 'Index' }
        ]);

        this.store = new Ext.data.Store({
            proxy: this.proxy,
            writer: this.writer,
            reader: this.reader,
            autoLoad: true,
            autoSave: false,
            batch: true,
            listeners: {
                write: {
                    fn: function() { this.markDirty(false); },
                    scope: this
                },
                exception: {
                    fn: function(proxy, type, action, options, response, arg) {
                        Ext.MessageBox.show({
                            title: '',
                            msg: response.responseText,
                            buttons: Ext.MessageBox.OK,
                            width: 300,
                            icon: Ext.MessageBox.ERROR
                        });
                    },
                    scope: this
                }
            },
            sortInfo: {
                field: 'index',
                direction: 'ASC'
            }
        });

        this.grid = new Ext.grid.GridPanel({
            enableDragDrop: true,
            ddGroup: 'firstGridDDGroup',
            store: this.store,
            stripeRows: true,
            columns:
            [
                { id: 'id', hidden: true, dataIndex: 'id', menuDisabled: true, type: 'string' },
                { id: 'index', header: '№', dataIndex: 'index', menuDisabled: true, width: 50, fixed: true },
                { id: 'name', header: 'Позиция', dataIndex: 'name', menuDisabled: true }
            ],
            viewConfig: {
                forceFit: true
            }
        });

        return this.grid;
    },
    AddPositions: function() {
        var url = "/Grid/SearchMultiple/Position";
        var self = Ext.getCmp('actionsTab');
        var result = window.showModalDialog(url, null, 'status:no; resizable:yes; dialogWidth:900px; dialogHeight:500px; resizable: yes; scroll: no; location:yes;');
        if (result) {
            var store = self.store;
            Ext.each(result.items, function (item) {
                if (!store.getById(item.id)) {
                    var data = {  name: item.name, index: store.data.items.length };
                    var record = new store.recordType(data, data.id);
                    this.markDirty(true);
                    record.markDirty();
                    store.add(record);
                }
            });
        }
    },
    Save: function() {
        this.store.save();
    },
    RegisterDragAndDrop: function () {
        var self = this;
        this.dropTarget = new Ext.dd.DropTarget(this.grid.getView().el.dom, {
            ddGroup: 'firstGridDDGroup',
            notifyDrop: function (ddSource, event, data) {
                var rows = self.grid.getView().getRows();
                var insertIndex = rows.length;
                for (var index = 0; index < rows.length; index++) {
                    if (Ext.get(rows[index]).getTop() > event.xy[1]) {
                        insertIndex = index;
                        break;
                    }
                }

                Ext.each(ddSource.dragData.selections, function (record) {
                    self.move(record, insertIndex - 1);
                });

                self.store.sort(self.store.sortInfo.field, self.store.sortInfo.direction);
            }
        });
    },
    update: function () {
        var x = 0;
    },
    move: function (record, newIndex) {
        if (record.data.index < newIndex) {
            // Смещаем записи между начальной и конечной точками вставки вверх
            this.moverange(record.data.index, newIndex, -1);
            record.set('index', newIndex);
            this.markDirty(true);
        } else if (record.data.index > newIndex + 1) {
            // Смещаем записи между начальной и конечной точками вставки вниз
            this.moverange(newIndex + 1, record.data.index, 1);
            record.set('index', newIndex + 1);
            this.markDirty(true);
        }
    },
    moverange: function (startIndex, endIndex, shift) {
        var allRecords = this.store.data.items;
        Ext.each(allRecords, function(r) {
            if (r.data.index >= startIndex && r.data.index <= endIndex)
                r.set('index', r.data.index + shift);
        });
    },
    markDirty: function (value) {
        window.Card.isDirty = value;
    }
});

window.InitPage = function () {
    var sortingComponent = new Ext.DoubleGis.UI.PositionSortingOrder();
    window.Card.AddPositions = sortingComponent.AddPositions;
    window.Card.Save = function () { sortingComponent.Save(); };
    window.Card.SaveAndClose = function () { sortingComponent.Save(); window.Card.Close(); };

    window.Card.on("afterbuild", function (card) {
        card.Items.TabPanel.add(sortingComponent);
        card.Items.TabPanel.activate(0);
        sortingComponent.RegisterDragAndDrop();
    });
};
