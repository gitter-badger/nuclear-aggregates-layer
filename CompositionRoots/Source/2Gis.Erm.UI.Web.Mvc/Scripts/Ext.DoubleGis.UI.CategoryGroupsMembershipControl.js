Ext.ns("Ext.DoubleGis.UI");

Ext.grid.CategoryColumn = Ext.extend(Object, {
    readonly: false,

    constructor: function(config) {
        Ext.apply(this, config);
        if (!this.id) {
            this.id = Ext.id();
        }
        this.renderer = this.renderer.createDelegate(this);
    },

    renderer: function(v, meta, record) {
        // Костыль, чтобы красный треугольник отрисовывался только у первой колонки.
        if (Ext.encode(record.getChanges()) != '{}')
        {
            meta.css += ' x-grid3-dirty-cell';
        }

        return v;
    }
});
    
Ext.grid.MembershipCheckColumn = Ext.extend(Object, {

    readOnly: false,
    triState: false,

    constructor: function (config) {
        Ext.apply(this, config);
        if (!this.id) {
            this.id = Ext.id();
        }
        this.renderer = this.renderer.createDelegate(this);
    },

    init: function (grid) {
        this.grid = grid;

        this.grid.on('render', function () {
            var view = grid.getView();
            view.mainBody.on('mousedown', this.onMouseDown, this);
        }, this);
       
    },

    onMouseDown: function (e, t) {

        if (!t.className)
            return;

        if (t.className.indexOf('x-grid3-cc-' + this.id + '-') == -1)
            return;
        
        if (!this.ColumnCategoryGroupId) {
            return;
        }

        e.stopEvent();

        if (this.readOnly)
            return;

        var index = this.grid.getView().findRowIndex(t);
        var record = this.grid.store.getAt(index);
        var newState = record.data.CategoryGroupId != this.ColumnCategoryGroupId;
        
        if (newState) {
            record.set('CategoryGroupId', this.ColumnCategoryGroupId);
        } else {
            record.set('CategoryGroupId', null);
        }
        
        window.Card.isDirty = true;
    },

    renderer: function (v, p, record) {
        // Костыль, чтобы красный треугольник отрисовывался только у первой колонки.
        record.isDirty = false;
        p.css += ' x-grid3-check-col-td';

        var idClass = 'x-grid3-cc-' + this.id + '-';

        var cellValue = record.data && record.data.CategoryGroupId == this.ColumnCategoryGroupId;
        
        var checkedClass = 'x-grid3-check-col';
        if (cellValue) {
            checkedClass += '-on';
        } else {
            checkedClass += '';
        }

        if (this.readOnly) {
            var readonlyClass = checkedClass + '-disabled';
            checkedClass += ' ' + readonlyClass;
        }

        return '<div class="' + idClass + ' ' + checkedClass + '">&#160;</div>';
    }
});

Ext.DoubleGis.UI.CategoryGroupsMembershipControl = Ext.extend(Object,
{
    Grid: null,
    ReadOnly: false,

    OnSuccess: null,
    OnFailure: null,

    constructor: function (config) {
        if (config.readOnly)
            this.ReadOnly = config.readOnly;

        // proxy
        var proxy = new Ext.data.HttpProxy(
        {
            api: config.api,

            listeners:
            {
                scope: this,

                // слушаем событие exception
                exception: function () {
                    if (this.OnFailure)
                        this.OnFailure();
                }
            }
        });

        this.OnDataLoaded = function() {
            var categoryGroupsData = store.reader.jsonData.AllCategoryGroups;

            columns = [];
            columns.push(
                {
                    header: 'Id',
                    dataIndex: 'CategoryId',
                    hidden: true
                });
            columns.push( new Ext.grid.CategoryColumn(
                {
                    header: Ext.LocalizedResources.CategoryName,
                    dataIndex: 'CategoryName',
                    sortable: true,
                    width: 400
                }));
            columns.push(new Ext.grid.CategoryColumn(
                {
                    header: 'Level',
                    dataIndex: 'CategoryLevel',
                    sortable: true,
                    width: 50
                }));
            
            var checkColumns = [];
            for (var i = 0; i < categoryGroupsData.length; i++) {
                var checkColumn = new Ext.grid.MembershipCheckColumn(
                    {
                        header: categoryGroupsData[i].Name,
                        dataIndex: categoryGroupsData[i].Id,
                        ColumnCategoryGroupId: categoryGroupsData[i].Id,
                        id: categoryGroupsData[i].Id,
                        sortable: false,
                        readOnly: false
                    });

                checkColumns.push(checkColumn);
                columns.push(checkColumn);
            }
            var columnModel = new Ext.grid.ColumnModel(
                {
                    columns: columns
                });
            
            this.Grid = new window.Ext.grid.EditorGridPanel(
                {
                    layout: 'fit',
                    plugins: checkColumns.concat([new window.Ext.ux.FitToParent('MainTab')]),
                    store: store,
                    colModel: columnModel,
                    selModel: new Ext.grid.RowSelectionModel({ singleSelect: true }),

                    renderTo: config.div,
                    clicksToEdit: 1,

                    enableColumnHide: false,
                    enableColumnMove: false,
                    enableColumnResize: false,
                    enableDragDrop: false,
                    enableHdMenu: false
                });

            this.Grid.on('headerclick', function (container, column, e) {

                var clickedColumn = container.colModel.columns[column];
                
                if (clickedColumn.ColumnCategoryGroupId) {
                    // Проверяем - если у всех рубрик в этой колонке поставлена галочка, то надо будет её снять.
                    var areAllItemsChecked = true;
                    for (var itemIndex = 0; itemIndex < container.store.data.items.length; itemIndex++) {
                        if (container.store.data.items[itemIndex].get('CategoryGroupId') != clickedColumn.ColumnCategoryGroupId) {
                            areAllItemsChecked = false;
                            break;
                        }
                    }

                    var valueToSet = areAllItemsChecked ? null : clickedColumn.ColumnCategoryGroupId;
                    
                    for (itemIndex = 0; itemIndex < container.store.data.items.length; itemIndex++) {
                        container.store.data.items[itemIndex].set('CategoryGroupId', valueToSet);
                    }
                    
                    window.Card.isDirty = true;
                    container.getView().refresh();
                }
            });
        };
        
        // store
        var store = new window.Ext.data.Store(
        {
            proxy: proxy,

            baseParams: config.baseParams,
            autoSave: false,
            
            reader: new window.Ext.data.JsonReader({
                idProperty: 'Id',
                root: "categoryGroupsMembership",
                successProperty: 'success',

                fields: [
                    { name: 'Id', type: 'int' },
                    { name: 'CategoryId', type: 'string' },
                    { name: 'CategoryName', type: 'string' },
                    { name: 'CategoryLevel', type: 'string' },
                    { name: 'CategoryGroupId', type: 'string' }
                ]
            }),

            writer: new Ext.data.JsonWriter({
                writeAllFields: true,
                listful: true,
                fields: [{ name: 'Id', type: 'string' },
                    { name: 'CategoryId', type: 'string' },
                    { name: 'CategoryGroupId', type: 'string' }
                ]
            }),
            sortInfo: { field: 'CategoryName', direction: 'ASC' },

            listeners:
            {
                scope: this,

                load: function() {
                    if (this.OnDataLoaded)
                        this.OnDataLoaded();
                },
                save: function () {
                    if (this.OnSuccess)
                        this.OnSuccess();
                }
            }
        });
        
        store.load();
    },

    Save: function (onSuccess, onFailure, onConfirmation) {
        var store = this.Grid.getStore();

        if (this.ReadOnly || store.getModifiedRecords().length == 0) {
            onSuccess();
            return;
        }

        this.OnSuccess = onSuccess;
        this.OnFailure = onFailure;

        if (onConfirmation) {
            onConfirmation(this);
            return;
        }

        store.save();
    },

    OnConfirmation: function () {
        var store = this.Grid.getStore();
        store.save();
    }
});
