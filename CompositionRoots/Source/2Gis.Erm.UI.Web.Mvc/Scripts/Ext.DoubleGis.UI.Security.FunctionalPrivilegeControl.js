Ext.ns("Ext.DoubleGis.UI");

Ext.DoubleGis.UI.ComboRenderer = function (store)
{
    return function(value)
    {
        var result;
        store.findBy(function (record) {
            if (record.get('Mask') == value) {
                result = record.get('NameLocalized');
                return true;
            }
            return false;
        });
        return result;
    };
};

Ext.DoubleGis.UI.FunctionalPermissionControl = Ext.extend(Object, {
    privilegesStore: null,
    privilegesDepthsStore: null,

    constructor: function (roleId) {
        this.RoleId = roleId;

        this.privilegesStore = new Ext.data.JsonStore({
            proxy: new Ext.data.HttpProxy({
                method: 'POST',
                prettyUrls: false,
                api: {
                    read: '/Privilege/GetFunctionalPrivilegesForRole',
                    create: '/Privilege/SaveFunctionalPrivileges',
                    update: '/Privilege/SaveFunctionalPrivileges'
                }
            }),
            root: 'Data',
            baseParams: { roleId: this.RoleId },
            idProperty: 'PrivilegeId',
            fields: ['PrivilegeId', 'NameLocalized', 'Mask', 'Priority'],
            autoSave: false,
            writer: new Ext.data.JsonWriter({ listful: true }),
            sortInfo: { field: 'NameLocalized', direction: 'ASC' },
            listeners: {
                exception: function (proxy, type, action, o, response, args) {
                    if (type == "remote" || response.status != 200) {
                        if (this.OnFailure) {
                            this.OnFailure();
                        }
                    } else {
                        if (this.OnSuccess) {
                            this.OnSuccess();
                            // Снимаем со всех записей отметку об изменении.
                            if (this.data) {
                                this.data.each(function(record) { record.commit(); });
                            }
                        }
                    }
                }
            }
        });

        this.privilegesDepthsStore = new Ext.data.JsonStore({
            proxy: new Ext.data.HttpProxy({
                method: 'POST',
                prettyUrls: false,
                api: {
                    read: '/Privilege/GetFunctionalPrivilegesDepths'
                }
            }),
            root: 'Data',
            baseParams: {},
            fields: ['PrivilegeId', 'NameLocalized', 'Mask', 'Priority']
        });

        this.privilegeDepthsComboBox = new Ext.form.ComboBox({
            ignoreNoChange: true,
            triggerAction: 'all',
            lazyRender: true,
            mode: 'local',
            editable: false,
            store: new Ext.data.ArrayStore({
                id: 1,
                fields: [
                    'PrivilegeId',
                    'NameLocalized',
                    'Mask',
                    'Priority'
                ],
                data: []
            }),
            listeners: {
                scope: this,
                'select': function (combo, record, index) {
                    this.privilegesStore.findBy(function (gridRecord) {
                        if (gridRecord.id != record.data.RowId) {
                            return false;
                        }

                        if (gridRecord.data.OriginalMask == record.data.Mask) {
                            gridRecord.reject();
                        }
                        else {
                            gridRecord.data.Priority = record.data.Priority;
                        }
                        return true;
                    });
                }
            },
            valueField: 'Mask',
            displayField: 'NameLocalized'
        });

        this.PermissionGrid = new Ext.grid.EditorGridPanel({
            store: this.privilegesStore,
            columns: [{
                header: Ext.getDom('privilegeNameLocalized').value,
                dataIndex: 'NameLocalized'
            }, {
                header: Ext.getDom('valueLocalized').value,
                dataIndex: 'Mask',
                editor: this.privilegeDepthsComboBox,
                renderer: Ext.DoubleGis.UI.ComboRenderer(this.privilegesDepthsStore)
            }],
            renderTo: 'funcPermissionPanel',
            viewConfig: { forceFit: true },
            clicksToEdit: '1',
            sm: new Ext.grid.RowSelectionModel({ singleSelect: true }),
            height: 525
        });

        this.RegisterGridRowSelectEvent(this);
        this.LoadData();
    },

    LoadData: function () {
        var control = this;
        this.privilegesDepthsStore.on('load', function () {
            control.privilegesStore.load();
        });
        
        this.privilegesStore.on('load', function () {
            this.each(function(record) {
                record.data.OriginalMask = record.data.Mask;
            });
        });

        control.privilegesDepthsStore.load();
    },

    Save: function (onSuccess, onFailure) {
        this.privilegesStore.OnSuccess = onSuccess;
        this.privilegesStore.OnFailure = onFailure;

        var count = this.privilegesStore.save();
        if (count == -1) {
            onSuccess();
        }
    },

    RegisterGridRowSelectEvent: function (control) {
        control.PermissionGrid.getSelectionModel().on('rowselect', function (sm, rowIdx, r) {
            control.PermissionGrid.stopEditing();

            var comboBoxStore = control.privilegeDepthsComboBox.store;
            comboBoxStore.removeAll();
            control.privilegesDepthsStore.each(function (record) {
                var data = record.data;
                if (data.PrivilegeId == r.data.PrivilegeId || data.PrivilegeId == -1) {
                    var dataRecord = {
                        PrivilegeId: data.PrivilegeId,
                        Mask: data.Mask,
                        NameLocalized: data.NameLocalized,
                        Priority: data.Priority,

                        // выпадающий список содержит позиции с разными PrivilegeId, поэтому нам требуется дополнительное 
                        // поле, чтобы идентифицировать строку, в которую сейчас выбираем значение
                        RowId: r.id
                    };

                    var rec = new Ext.data.Record(new Object(dataRecord));
                    comboBoxStore.add(rec);
                }
            });
        });
    }
});

Ext.onReady(function ()
{
    var roleId = Ext.getDom("RoleId").value;
    Ext.DoubleGis.UI.FunctionalPermissionControlInstance = new Ext.DoubleGis.UI.FunctionalPermissionControl(roleId);

    if (Ext.DoubleGis.UI.EntityPermissionControlInstance)
    {
        Ext.DoubleGis.UI.EntityPermissionControlInstance.on('afterpost', function ()
        {
            Ext.DoubleGis.UI.FunctionalPermissionControlInstance.Save();
        });
    }
});

