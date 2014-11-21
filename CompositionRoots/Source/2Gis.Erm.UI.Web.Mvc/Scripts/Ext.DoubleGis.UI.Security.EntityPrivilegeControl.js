Ext.ns("Ext.DoubleGis.UI");
Ext.DoubleGis.UI.PrivilegeFields = [
    { name: 'EntityName', type: 'string' },
    // Long'a в js нет и если оставить int, то длинные id-шники считываются с потерей точности. 
    { name: 'PrivilegeId',  type: 'string' },
    { name: 'PrivilegeDepthMask', type: 'string' }
];
Ext.DoubleGis.UI.PrivilegeDepthFields = [
    { name: 'NameLocalized', type: 'string' },
    { name: 'Mask', type: 'string' }
];
Ext.DoubleGis.UI.Privilege      = Ext.data.Record.create(Ext.DoubleGis.UI.PrivilegeFields);
Ext.DoubleGis.UI.PrivilegeDepth = Ext.data.Record.create(Ext.DoubleGis.UI.PrivilegeDepthFields);

// CRInternal: {d.ivanov}:{#2}:{Major}:{16.09.2010}: методы тоже лучше держать в классе в отдельном подобъекте, назвав его, например, RenderUtils
Ext.DoubleGis.UI.ComboRenderer = function (combo) {
    return function(value) {
        var record = combo.findRecord(combo.valueField, value);
        return record ? record.get(combo.displayField) : combo.valueNotFoundText;
    };
};


Ext.DoubleGis.UI.EntityPermissionControl = Ext.extend(Ext.util.Observable, {
    SaveStore: null,
    MainStore: null,
    privilegesDepthsStore: null,

    constructor: function (roleId) {
        this.addEvents('afterpost');
        this.RoleId = roleId;

        this.privilegesDepthsStore = new Ext.data.JsonStore({
            proxy: new Ext.data.HttpProxy({
                method: 'GET',
                url: '/Privilege/GetEntityPrivilegesDepths'
            }),
            root: 'Data',
            idProperty: 'Mask',
            fields: Ext.DoubleGis.UI.PrivilegeDepthFields,
            autoSave: false,
            writer: new Ext.data.JsonWriter({ listful: true })
        });

        this.privilegeDepthsComboBox = new Ext.form.ComboBox({
            triggerAction: 'all',
            store: new Ext.data.ArrayStore({
                id: 1,
                fields: [
                    'NameLocalized',
                    'Mask'
                ],
                data: []
            }),
            mode: 'local',
            lazyRender: true,
            displayField: 'NameLocalized',
            valueField: 'Mask',
            editable: false,
            value: 0,
            height: 20
        });

        this.SaveStore = new Ext.data.JsonStore({
            url: '/Privilege/SaveEntityPrivileges',
            root: 'Data',
            baseParams: { roleId: this.RoleId },
            idProperty: 'EntityName',
            fields: Ext.DoubleGis.UI.PrivilegeFields,
            autoSave: false,
            writer: new Ext.data.JsonWriter({ listful: true }),
            listeners: {
                exception: function (proxy, type, action, o, response, args) {
                    if (type == "remote" || response.status != 200) {
                        if (this.OnFailure) {
                            this.OnFailure();
                        }
                    } else {
                        if (this.OnSuccess) {
                            this.OnSuccess();
                        }
                    }
                }
            }
        });

        this.privilegesStore = new Ext.data.JsonStore({
            url: '#',
            idProperty: 'PrivilegeId',
            fields: [
                { name: 'PrivilegeId', type: 'string' },
                { name: 'NameLocalized', type: 'string' },
                { name: 'PrivilegeDepthMask', type: 'string' }
            ],
            autoSave: false,
            writer: new Ext.data.JsonWriter({ listful: true })
        });

        this.PermissionGrid = new Ext.grid.EditorGridPanel({
            store: this.privilegesStore,
            columns: [
                { header: Ext.getDom('privilegeNameLocalized').value, dataIndex: 'NameLocalized', width: 370 },
                { header: Ext.getDom('privilegeDepthLocalized').value, dataIndex: 'PrivilegeDepthMask',
                    editor: this.privilegeDepthsComboBox,
                    // CRInternal: {d.ivanov}:{#2}:{Major}:{16.09.2010}: достаточно указать scope, а не передавать в качестве параметр поле объекта
                    renderer: Ext.DoubleGis.UI.ComboRenderer(this.privilegeDepthsComboBox),
                    width: 200
                }
            ],
            plugins: this.CheckColumn,
            viewConfig: { forceFit: true },
            height: 525,
            clicksToEdit: '1'
        });
        this.MainStore = new Ext.data.JsonStore({
            url: '/Privilege/GetEntityPrivilegesForRole',
            root: 'Data',
            baseParams: { roleId: this.RoleId },
            idProperty: 'EntityName',
            fields: ['EntityNameLocalized', 'PrivilegeInfoList'],
            autoSave: false,
            writer: new Ext.data.JsonWriter({ listful: true }),
            sortInfo: { field: 'EntityNameLocalized', direction: 'ASC' }
        });
        this.EntityGrid = new Ext.grid.GridPanel({
            store: this.MainStore,
            columns: [
                { header: Ext.getDom('entityNameLocalized').value, dataIndex: 'EntityNameLocalized', menuDisabled: true, width: 280 },
                { dataIndex: 'PrivilegeInfoList', hidden: true }
            ],
            width: 300,
            height: 525,
            sm: new Ext.grid.RowSelectionModel({ singleSelect: true })
        });
        this.Panel = new Ext.Panel({
            renderTo: 'entityPermissionPanel',
            height: 525,
            layout: 'column',
            items: [this.EntityGrid, this.PermissionGrid]
        });

        // CRInternal: {d.ivanov}:{#3}:{Major}:{16.09.2010}: не нужно передавать this, методы в качестве контекста его и имеют
        this.RegisterAfterSaveEvent(this);
        this.RegisterEntityGridRowSelectEvent(this);
        this.RegisterEntityGridRowDeselectEvent(this);
        this.LoadData();
    },

    RegisterEntityGridRowDeselectEvent: function (control) {
        control.EntityGrid.getSelectionModel().on('rowdeselect', function (sm, rowIdx, r) {
            Ext.each(control.privilegesStore.getModifiedRecords(), function (record) {
                var updated = false;
                control.SaveStore.each(function (saveRecord) {
                    if (saveRecord.data.EntityName == r.data.EntityName &&
                        saveRecord.data.PrivilegeId == record.data.PrivilegeId) {
                        saveRecord.PrivilegeDepthMask = record.data.PrivilegeDepthMask;
                        updated = true;
                        return false;
                    }
                    return true;
                });
                if (!updated) {
                    var p = new Ext.DoubleGis.UI.Privilege({
                        EntityName: r.data.EntityName,
                        PrivilegeId: record.data.PrivilegeId,
                        PrivilegeDepthMask: record.data.PrivilegeDepthMask
                    });
                    control.SaveStore.add(p);
                }
                Ext.each(r.data.PrivilegeInfoList, function (permissionInfo) {
                    if (permissionInfo.PrivilegeId == record.data.PrivilegeId) {
                        permissionInfo.PrivilegeDepthMask = record.data.PrivilegeDepthMask;
                        return false;
                    }
                    return true;
                });
            });
        });
    },

    RegisterEntityGridRowSelectEvent: function (control) {
        control.EntityGrid.getSelectionModel().on('rowselect', function (sm, rowIdx, r) {
            control.EntityGrid.stopEditing();

            var comboBoxStore = control.privilegeDepthsComboBox.store;
            comboBoxStore.removeAll();
            control.privilegesDepthsStore.each(function (record) {
                var rec = new Ext.data.Record(
                {
                    NameLocalized: record.data.NameLocalized,
                    Mask: record.data.Mask
                });
                comboBoxStore.add(rec);
            });
            control.privilegesStore.loadData(r.data.PrivilegeInfoList);
        });
    },

    LoadData: function () {
        this.privilegesDepthsStore.load();
        this.MainStore.load();
    },

    Save: function (onSuccess, onFailure) {
        var sm = this.EntityGrid.getSelectionModel();
        if (sm.selectNext(true)) {
            sm.selectPrevious(true);
        }
        else {
            sm.selectPrevious(true);
            sm.selectNext(true);
        }

        this.SaveStore.OnFailure = function () {
            onFailure();
        };

        var control = this;
        this.SaveStore.OnSuccess = function () {
            control.SaveStore.removeAll();
            onSuccess();
        };

        var count = this.SaveStore.save();

        if (count == -1) {
            this.fireEvent('afterpost');
            this.SaveStore.OnSuccess();
        }
    },

    RegisterAfterSaveEvent: function (control) {
        control.SaveStore.on('save', function (store, batch, data) {
            control.fireEvent('afterpost');
        });
    }

});

Ext.onReady(function ()
{
    var roleId = Ext.getDom("RoleId").value;
    Ext.DoubleGis.UI.EntityPermissionControlInstance = new Ext.DoubleGis.UI.EntityPermissionControl(roleId);
});

