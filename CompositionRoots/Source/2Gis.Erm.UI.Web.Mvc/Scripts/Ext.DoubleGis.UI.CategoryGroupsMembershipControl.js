Ext.ns("Ext.DoubleGis.UI");

Ext.grid.MembershipCheckColumn = Ext.extend(Ext.grid.Column, {
    constructor: function (config) {
        Ext.apply(this, config);
        Ext.grid.MembershipCheckColumn.superclass.constructor.call(this, config);
    },

    processEvent: function (name, e, grid, rowIndex, colIndex) {
        var record = grid.store.data.items[rowIndex];
        var previousValue = record.get('CategoryGroupId');

        if (previousValue != this.dataIndex) {
            this.setRecordGroup(record, previousValue, this.dataIndex);
        } else {
            this.setRecordGroup(record, this.dataIndex, this.defaultGroupId);
    }
    
        return true;
    },

    renderer: function (value, meta, record) {
        var checkedClass = value ? 'x-grid3-check-col-on' : 'x-grid3-check-col';
        return '<div class="' + checkedClass + '">&#160;</div>';
    },

    setRecordGroup: function (record, oldValue, newValue) {
        newValue = newValue || this.defaultGroupId;

        record.beginEdit();

        record.set('CategoryGroupId', newValue);
        record.set(oldValue, false);
        record.set(newValue, true);
        
        record.endEdit();
        }
});

Ext.DoubleGis.UI.CategoryGroupsMembershipControl = Ext.extend(Ext.Panel,
{
    constructor: function (config) {
        Ext.Ajax.request({
            method: 'GET',
            url: '/CategoryGroupsMembership/CategoryGroups',
            success: function (response) {
                var groups = Ext.decode(response.responseText);
                this.finishCreation(config, groups);
            },
            failure: function (response) {
                Ext.MessageBox.alert('', response.responseText);
            },
            scope: this
        });
    },

    finishCreation: function (config, groups) {
        var records = this.createRecord(groups);
        var columns = this.createGridColumnms(groups);

        this.proxy = new Ext.data.HttpProxy({
            api: {
                read: { url: '/CategoryGroupsMembership/CategoryGroupsMembership', method: 'GET' },
                update: { url: '/CategoryGroupsMembership/CategoryGroupsMembership', method: 'POST' }
        }
        });
        
        this.reader = new window.Ext.data.JsonReader({
            idProperty: 'Id',
            root: "categoryGroupsMembership",
            successProperty: 'success'
        }, records);

        this.writer = new Ext.data.JsonWriter({
            writeAllFields: true,
            listful: true
        }, records);

        this.store = new window.Ext.data.Store({
            baseParams: { organizationUnitId: config.organizationUnitId },
            autoSave: false,
        
            proxy: this.proxy,
            reader: this.reader,
            writer: this.writer,

            sortInfo: { field: 'CategoryName', direction: 'ASC' },

            listeners: {
                scope: this,
                load: this.loadSuccess,
                save: this.saveSuccess,
                exception: this.saveFailure
    }
});

        this.grid = new window.Ext.grid.EditorGridPanel({
            store: this.store, 
            selModel: new Ext.grid.RowSelectionModel({ singleSelect: true }),
            colModel: columns,

            clicksToEdit: 1,

            enableColumnHide: false,
            enableColumnMove: false,
            enableColumnResize: false,
            enableDragDrop: false,
            enableHdMenu: false,

            listeners: {
                scope: this,
                headerclick: this.applyCategoryToAllRecords
            },

            viewConfig: {
                markDirty: false
            }
        });


        Ext.DoubleGis.UI.CategoryGroupsMembershipControl.superclass.constructor.call(this, {
            items: [this.grid],
            plugins: [new window.Ext.ux.FitToParent('MainTab')],
            layout: 'fit',
            renderTo: config.renderTo
                    });

        this.mask = new Ext.LoadMask('MainTab');
        this.mask.show();
            
        this.store.load();
    },

    save: function () {
        window.Card.Items.Toolbar.disable();
        this.mask.show();
        var pending = this.store.save();

        if (pending == -1) {
            this.saveSuccess();
        }
    },

    applyCategoryToAllRecords: function (container, column, e) {
        var clickedColumn = container.getColumnModel().getColumnById(column);

        if (clickedColumn.columnCategoryGroupId) {
                    // Проверяем - если у всех рубрик в этой колонке поставлена галочка, то надо будет её снять.
                    var areAllItemsChecked = true;
            Ext.each(container.store.data.items, function (item) {
                if (item.get('CategoryGroupId') != clickedColumn.columnCategoryGroupId) {
                            areAllItemsChecked = false;
                        }
            });

            var valueToSet = areAllItemsChecked ? null : clickedColumn.columnCategoryGroupId;
            Ext.each(container.store.data.items, function(item) {
                clickedColumn.setRecordGroup(item, item.get('CategoryGroupId'), valueToSet);
            });
                    
                    window.Card.isDirty = true;
                    container.getView().refresh();
                }
    },
        
    createRecord: function (groups) {
        var fields = [
            { name: 'Id', mapping: 'Id', type: 'string' },
            { name: 'CategoryId', mapping: 'CategoryId', type: 'string' },
            { name: 'CategoryName', mapping: 'CategoryName', type: 'string' },
            { name: 'CategoryLevel', mapping: 'CategoryLevel', type: 'string' },
            { name: 'CategoryGroupId', mapping: 'CategoryGroupId', type: 'string' },
            { name: 'OriginalCategoryGroupId', convert: function(v, record) { return record['CategoryGroupId']; } }
        ];

        Ext.each(groups, function(group) {
            fields.push({
                name: group.Id,
                mapping: group.Id,
                convert: function(v, record) { return record['CategoryGroupId'] === group.Id; }
            });
        });
            
        return Ext.data.Record.create(fields);
    },

    createGridColumnms: function (groups) {
        var columns = [
            { header: 'Id', dataIndex: 'CategoryId', hidden: true },
            { header: Ext.LocalizedResources.CategoryName, dataIndex: 'CategoryName', sortable: true, width: 400, renderer: function(value, meta, record) {
                meta.css += record.get('CategoryGroupId') !== record.get('OriginalCategoryGroupId')
                    ? ' x-grid3-dirty-cell'
                    : '';
                return value;
            } },
            { header: 'Level', dataIndex: 'CategoryLevel', sortable: true, width: 50 }
        ];

        var defaultGroupId = null;
        Ext.each(groups, function(group) {
            if (group.IsDefault) {
                defaultGroupId = group.Id;
            }
        });
        
        Ext.each(groups, function (group) {
            columns.push(new Ext.grid.MembershipCheckColumn({
                header: group.Name,
                dataIndex: group.Id,
                columnCategoryGroupId: group.Id,
                hidden: group.IsDefault,
                defaultGroupId: defaultGroupId,
                sortable: false
            }));
        });

        return new Ext.grid.ColumnModel(columns);
    },

    saveSuccess: function () {
        this.store.load();
        this.markDirty(false);
        window.Card.Items.Toolbar.enable();
        window.Card.recalcToolbarButtonsAvailability();
        this.mask.hide();
    },

    loadSuccess: function() {
        this.mask.hide();
    },

    saveFailure: function (proxy, type, action, options, response, arg) {
        Ext.MessageBox.show({
            title: Ext.LocalizedResources.ApplicationError,
            msg: response.responseText,
            buttons: Ext.MessageBox.OK,
            width: 300,
            icon: Ext.MessageBox.ERROR
        });
        window.Card.Items.Toolbar.enable();
        window.Card.recalcToolbarButtonsAvailability();
        this.mask.hide();
    },

    markDirty: function (value) {
        window.Card.isDirty = value;
    }
});
