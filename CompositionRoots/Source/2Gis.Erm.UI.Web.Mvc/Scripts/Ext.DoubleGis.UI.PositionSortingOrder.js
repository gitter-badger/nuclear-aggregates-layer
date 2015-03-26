﻿Ext.namespace('Ext.DoubleGis.UI');
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
                create: { url: 'NOT IMPLEMENTED', method: 'POST' },
                update: { url: '/Price/PositionSortingOrderData', method: 'POST' },
                destroy: { url: 'NOT IMPLEMENTED', method: 'POST' }
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
            { name: 'index', mapping: 'Index', sortType: function (value) { return value == null ? -1 : value;  } }
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
                    fn: this.saveSuccess,
                    scope: this
                },
                exception: {
                    fn: this.saveFailure,
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
                { id: 'index', header: '№', dataIndex: 'index', menuDisabled: true, width: 50, fixed: true, renderer: function (value) { return value === null ? "" : parseInt(value) + 1; } },
                {
                    id: 'name',
                    header: 'Позиция',
                    dataIndex: 'name',
                    menuDisabled: true,
                    renderer: function(value, metaData, record) {
                         if (record.get('index') === null) {
                              metaData.style += 'text-decoration: line-through;';
                         }

                         return value;
                    }
                }
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
                    var data = { id: item.id, name: item.name, index: store.data.items.length };
                    var record = new store.recordType(data, data.id);
                    self.markDirty(true);
                    record.markDirty();
                    store.add(record);
                }
            });
        }
    },
    RemovePositions: function () {
        var self = Ext.getCmp('actionsTab');

        var selection = self.grid.getSelectionModel().getSelections();
        Ext.each(selection, function (record) {
            this.moverange(record.get('index'), Number.MAX_VALUE, -1);
            record.set('index', null);
            this.markDirty(true);
        }, self);

        self.store.sort(self.store.sortInfo.field, self.store.sortInfo.direction);
    },
    Save: function () {
        window.Card.Items.Toolbar.disable();
        var pending = this.store.save();

        if (pending == -1) {
            this.saveSuccess();
        }
    },
    saveSuccess: function () {
        this.store.load();
        this.markDirty(false);
        window.Card.Items.Toolbar.enable();
        window.Card.recalcToolbarButtonsAvailability();
    },
    saveFailure: function(proxy, type, action, options, response, arg) {
        Ext.MessageBox.show({
            title: Ext.LocalizedResources.ApplicationError,
            msg: response.responseText,
            buttons: Ext.MessageBox.OK,
            width: 300,
            icon: Ext.MessageBox.ERROR
            });
        window.Card.Items.Toolbar.enable();
        window.Card.recalcToolbarButtonsAvailability();
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

                var deletedCount = 0;
                Ext.each(self.store.data.items, function(record) {
                    if (record.get('index') === null) {
                        deletedCount++;
                    }
                });

                insertIndex -= deletedCount;
                if (insertIndex < 0) {
                    insertIndex = 0;
                }

                Ext.each(ddSource.dragData.selections, function (record) {
                    self.move(record, insertIndex - 1);
                });

                self.store.sort(self.store.sortInfo.field, self.store.sortInfo.direction);
            }
        });
    },
    move: function (record, newIndex) {
        if (record.data.index === null) {
            return;
        }

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
            if (r.data.index === null) {
                return;
            }

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
    window.Card.RemovePositions = sortingComponent.RemovePositions;
    window.Card.Save = function () { sortingComponent.Save(); };
    window.Card.SaveAndClose = function () { sortingComponent.Save(); window.Card.Close(); };

    window.Card.on("afterbuild", function (card) {
        card.Items.TabPanel.add(sortingComponent);
        card.Items.TabPanel.activate(0);
        sortingComponent.RegisterDragAndDrop();
    });
};
