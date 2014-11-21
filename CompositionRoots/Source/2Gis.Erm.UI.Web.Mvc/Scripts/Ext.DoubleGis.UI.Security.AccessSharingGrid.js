Ext.ns("Ext.DoubleGis.UI.Security");
(function () {
    var AccessSharing = Ext.extend(Ext.util.Observable, {
        accessSharingStore: null,
        accessSharingGrid: null,

        userAlreadyInListMessageFormat: null,

        constructor: function (data) {
            accessSharingStore = new Ext.data.JsonStore({
                root: 'Data',
                idProperty: 'UserAccountId',
                fields: ['Selected', 'UserAccountId', 'UserAccountName', 'CanCreate', 'CanRead', 'CanUpdate', 'CanDelete', 'CanShare', 'CanAssign'],
                writer: new Ext.data.JsonWriter(),
                autoSave: false,
                batch: true
            });

            var createColumn = function (settings) {
                if (!settings.Width) {
                    settings.Width = 70;
                }
                return new Ext.grid.CheckColumn({
                    header: settings.LocalizedName,
                    dataIndex: settings.Name,
                    width: settings.Width,
                    readOnly: settings.ReadOnly
                });
            };

            var selectColumn = [
                createColumn({ LocalizedName: '', Name: 'Selected', Width: 20, ReadOnly: false})
            ];
            
            var readOnlyColumns = [
                { header: data.LayoutData.IdColumnLocalizedName, dataIndex: 'UserAccountId', sortable: true, hidden: true },
                { header: data.LayoutData.NameColumnLocalizedName, dataIndex: 'UserAccountName', sortable: true, width: 241 }
            ];

            var checkBoxColumns = new Array();
            for (var i = 0; i < data.LayoutData.AccessRightColumns.length; i++) {
                checkBoxColumns.push(createColumn(data.LayoutData.AccessRightColumns[i]));
            }

            var accessSharingGrid = new Ext.grid.EditorGridPanel({
                store: accessSharingStore,
                columns: selectColumn.concat(readOnlyColumns, checkBoxColumns),
                plugins: selectColumn.concat(checkBoxColumns),
                clicksToEdit: 1,
                applyTo: 'accessSharingGrid',
                width: 615,
                height: 370,
                loadMask: true,
                border: false,
                batchSave: false
            });
            this.accessSharingGrid = accessSharingGrid;
            this.userAlreadyInListMessageFormat = data.LayoutData.UserAlreadyInListMessageFormat;

            accessSharingStore.loadData(data.GridData);
        },

        InsertNew: function(userAccountId, userAccountName) {
           var items = accessSharingStore.data.items;
           for (var i = 0; i < items.length; i++) {
                if (items[i].data.UserAccountId == userAccountId) {
                    alert(this.userAlreadyInListMessageFormat.replace('{0}', userAccountName));
                    return;
                }
           }

           var Record = Ext.data.Record.create([
                { name: 'UserAccountId' },
                { name: 'UserAccountName' },
                { name: 'CanCreate' },
                { name: 'CanRead' },
                { name: 'CanUpdate' },
                { name: 'CanDelete' },
                { name: 'CanShare' },
                { name: 'CanAssign' }
           ]); 
           var newRecord = new Record(
                {
                    UserAccountId: userAccountId,
                    UserAccountName: userAccountName,
                    CanCreate: false,
                    CanRead: false,
                    CanUpdate: false,
                    CanDelete: false,
                    CanShare: false,
                    CanAssign: false
                },
                userAccountId
           );
           accessSharingStore.add(newRecord);
        },

        RemoveAt: function(index) {
            if (!index) {
                index = 0;
            }  
            accessSharingStore.removeAt(index);
        },

        RemoveSelected: function() {
            var selectedItems = [];
            var i;
            for (i = 0; i < accessSharingStore.data.items.length; i++) {
                if (accessSharingStore.data.items[i].data.Selected) {
                    selectedItems.push(accessSharingStore.data.items[i]);
                }
            }
            for (i = 0; i < selectedItems.length; i++) {
                accessSharingStore.remove(selectedItems[i]);
            }
        },

        GetData: function() {
            var result = new Array();
            var items = accessSharingStore.data.items;
            for (var i = 0; i < items.length; i++) {
                var obj = Ext.apply(new Object(), items[i].data, items[i].modified);
                result.push(obj);
            }
            return result;
        },

        Clear: function() {
            for (var i = accessSharingStore.data.items.length - 1; i >= 0; i--) {
                accessSharingStore.remove(accessSharingStore.data.items[i]);
            }
        },

        InvertSelectedRows: function() {
            
            var selectedRows = new Array();
            var i;
            for (i = 0; i < accessSharingStore.data.items.length; i++) {
                if (accessSharingStore.data.items[i].data.Selected) {
                    selectedRows.push(accessSharingStore.data.items[i]);
                }
            }

            if(selectedRows.length == 0) {
                return;
            }
            
            var valueToSet;

            var columns = this.accessSharingGrid.colModel.config;
            var column;
            for (i = 3; i < columns.length; i++) {
                column = columns[i];
                if (column.readOnly === false) {
                    valueToSet = !selectedRows[0].data[column.dataIndex];
                    break;
                }
            }

            for (i = 0; i < selectedRows.length; i++) {
                for (var j = 3; j < columns.length; j++) {
                    column = columns[j];
                    if (column.readOnly === false) {
                        selectedRows[i].data[column.dataIndex] = valueToSet;
                    }
                }
            }

            this.accessSharingGrid.view.refresh();
        }

    })

    Ext.DoubleGis.UI.Security.AccessSharing = AccessSharing;
})()


