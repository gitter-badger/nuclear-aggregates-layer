/// <reference path="ext-base-debug.js"/>
/// <reference path="ext-all-debug-w-comments.js"/>

Ext.ns("Ext.DoubleGis.UI");

Ext.DoubleGis.UI.AdditionalFirmServicesControl = Ext.extend(Object,
{
    Grid: null,
    ReadOnly: false,
    
    OnSuccess: null,
    OnFailure: null,

    constructor: function (config)
    {
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
                exception: function ()
                {
                    if (this.OnFailure)
                        this.OnFailure();
                }
            }
        });

        // store
        var store = new window.Ext.data.Store(
        {
            proxy: proxy,
            
            // pass firmAddressId to server
            baseParams: config.baseParams,
            autoSave: false,

            reader: new window.Ext.data.JsonReader(
            {
                idProperty: 'Name',
                root: "data",
                successProperty: 'success',

                fields: [
                    { name: 'Name', type: 'string'},
                    { name: 'NameLocalized', type: 'string' },
                    { name: 'Display', type: 'string' }
                ]
            }),
            writer: new Ext.data.JsonWriter({ listful: true }),
            //sortInfo: { field: 'Name', direction: 'ASC' },
            
            listeners:
            {
                scope: this,

                save: function ()
                {
                    if (this.OnSuccess)
                        this.OnSuccess();
                }
            }
        });

        store.load();

        // setup view
        var comboBox = new Ext.form.ComboBox(
        {
            triggerAction: "all",

            // combobox do not have default value
            editable: false,
            autoSelect: false,
            
            // only values from list allowed
            forceSelection: true,
            
            // combobox in grid requirement
            lazyRender: true,
            lastQuery: '',

            mode: 'local',
            
            store: new Ext.data.ArrayStore(
            {
                fields:
                [
                    { name: 'valueField', type: 'string' },
                    { name: 'displayField', type: 'string' },
                    { name: 'showInDropDown', type: 'boolean' }
                ],

                data:
                [
                    // TODO: mapping для enum в js, плохо
                    // с другой стороны тянуть с сервера тоже плохо, потом подумать
                    ['DoNotDisplay', Ext.LocalizedResources.AdditionalServicesDoNotDisplay, true],
                    ['Display', Ext.LocalizedResources.AdditionalServicesDisplay, true],
                    ['Default', Ext.LocalizedResources.AdditionalServicesDefault, false],
                    ['DependsOnAddress', Ext.LocalizedResources.AdditionalServicesDependsOnAddress, false]
                ]
            }),
            
            valueField: 'valueField',
            displayField: 'displayField',
            
            valueNotFoundText: Ext.LocalizedResources.AdditionalServicesValueNotFound,

            listeners:
            {
                // фильтруем допустимые значения combobox
                beforequery: function (queryEvent)
                {
                    var comboStore = queryEvent.combo.store;
                    
                    comboStore.clearFilter(true);
                    comboStore.filter(
                    {
                        fn: function (record)
                        {
                            return record.get('showInDropDown') == true;
                        }
                    });
                },
                
                // copy-paste из интернета чтобы ячейку в combobox нельзя было выделить
                afterRender: function (combo)
                {
                    combo.el.unselectable();
                },
                
                // кидаем событие blur, тем самым сразу коммитим значение
                select: function (combo)
                {
                    combo.fireEvent('blur', combo);
                }
            }
        });

        var columnModel = new Ext.grid.ColumnModel(
        {
            columns:
            [
                {
                    header: Ext.LocalizedResources.AdditionalServicesService,
                    dataIndex: 'NameLocalized',
                    width: 400
                },
                {
                    header: Ext.LocalizedResources.AdditionalServicesDisplayService,
                    dataIndex: 'Display',
                    width: 140,
                
                    editor:
                    {
                        // не отображаем красный уголок если значене не поменялось
                        ignoreNoChange: true,
                        // commit изменений на blur
                        allowBlur: true,
                    
                        field: comboBox
                    },
                    renderer: function (value)
                    {
                        var record = comboBox.findRecord(comboBox.valueField, value);
                        return record ? record.get(comboBox.displayField) : comboBox.valueNotFoundText;
                    }
                }
            ]       
        });

        // view
        this.Grid = new window.Ext.grid.EditorGridPanel(
        {
            store: store,
            colModel: columnModel,
            selModel: new Ext.grid.RowSelectionModel({ singleSelect: true }),

            renderTo: config.div,
            autoHeight: true,
            clicksToEdit: 1,
            
            enableColumnHide: false,
            enableColumnMove: false,
            enableColumnResize: false,
            enableDragDrop: false,
            enableHdMenu: false
        });
    },
    
    Save: function (onSuccess, onFailure, onConfirmation)
    {
        var store = this.Grid.getStore();

        if (this.ReadOnly || store.getModifiedRecords().length == 0)
        {
            onSuccess();
            return;
        }

        this.OnSuccess = onSuccess;
        this.OnFailure = onFailure;

        if (onConfirmation)
        {
            onConfirmation(this);
            return;
        }

        store.save();
    },
    
    OnConfirmation: function ()
    {
        var store = this.Grid.getStore();
        store.save();
    }
});