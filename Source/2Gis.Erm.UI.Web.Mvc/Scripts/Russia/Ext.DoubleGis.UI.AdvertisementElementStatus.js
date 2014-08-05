﻿/// <reference path="/Scripts/ext-all-debug-w-comments.js" />
/// <reference path="/Scripts/ext-base-debug.js" />

(function () {
    var cardSurrogate = new Ext.util.Observable();

    Ext.onReady(function () {
        var ruleRecord = Ext.data.Record.create([
            { name: 'id', mapping: 'Id' },
            { name: 'reasonId', mapping: 'DenialReasonId' },
            { name: 'name', mapping: 'DenialReasonName' },
            { name: 'group', mapping: 'DenialReasonType' },
            { name: 'active', mapping: 'IsActive', type: 'boolean' },
            { name: 'comment', mapping: 'Comment' },
            { name: 'checked', mapping: 'Checked', type: 'boolean' }
        ]);

        var reader = new Ext.data.JsonReader({
            root: 'Data',
            totalProperty: 'RowCount'
        }, ruleRecord);

        var sm = new Ext.grid.CheckboxSelectionModel({
            checkOnly: true,
            header: '',
            onKeyPress: function (e, name) { }, // Убивает навигацию стрелками
            listeners: {
                beforerowselect: onBeforeRowSelect,
                rowdeselect: onRowDeselect,
                rowselect: onRowSelect,
                selectionchange: onSelectionChange
            }
        });

        var store = new Ext.data.GroupingStore({
            reader: reader,
            groupField: 'group',
            modifiedRecords: new Array(),
            listeners: {
                load: onStoreLoad(sm)
            }
        });

        loadData('', function (data) { store.loadData(data); });

        var cm = new Ext.grid.ColumnModel({
            columns: [
                sm,
                { dataIndex: 'id', hidden: true },
                { dataIndex: 'group', header: "Группа", hidden: true },
                { dataIndex: 'name', width: 30, header: "Правило", renderer: function (value, metaData, record) { return record.data.active ? value : value + ' (неактивно)'; } },
                { dataIndex: 'comment', header: "Комментарий", editor: new Ext.form.TextField() }
            ],

            isCellEditable: function (col, row) {
                var record = store.getAt(row);
                if (!record.data.checked) {
                    return false;
                }

                return Ext.grid.ColumnModel.prototype.isCellEditable.call(this, col, row);
            }
        });

        var view = new Ext.grid.GroupingView({
            forceFit: true,
            cancelEditOnToggle: false,
            markDirty: false,
            showGroupName: false,
            groupTextTpl: '{text}',
            getRowClass: function (record, index) {
                if (!record.data.active && !record.data.checked) {
                    return 'x-item-disabled';
                }
                return '';
            }
        });

        var footer = new Ext.Toolbar({
            items: [
                {
                    id: 'summary-tbtext', // при обновлении тексте находим его по идентификатору
                    xtype: 'tbtext',
                    text: ''
                }
            ]
        });

        var header = new Ext.Toolbar({
            items: [
                {
                    id: 'search-textfield',
                    xtype: 'textfield',
                    width: '300',
                    enableKeyEvents: true,
                    listeners: {
                        keypress: function(textfield, eo) {
                            if (eo.getCharCode() == Ext.EventObject.ENTER) {
                                var filter = Ext.getCmp('search-textfield').getValue();
                                loadData(filter, function(data) { store.loadData(data); });
                            }
                        }
                    }
                }, {
                    id: 'search-button',
                    xtype: 'button',
                    icon: Ext.DoubleGis.Global.Helpers.GetStaticImagePath("CommonUI/qfind.gif"),
                    handler: function() {
                        var filter = Ext.getCmp('search-textfield').getValue();
                        loadData(filter, function(data) { store.loadData(data); });
                    }
                },
                {
                    id: 'clear-search-button',
                    xtype: 'button',
                    icon: Ext.DoubleGis.Global.Helpers.GetStaticImagePath("CommonUI/qfindclear.gif"),
                    handler: function () {
                        Ext.getCmp('search-textfield').setValue('');
                        loadData('', function(data) { store.loadData(data); });
                    }
                }
            ]
        });

        var tabPanel = new window.Ext.TabPanel({
            id: "TabWrapper",
            plugins: [new window.Ext.ux.FitToParent('TabPanelContainer')],
            applyTo: "MainTab_holder",
            border: false,
            deferredRender: false,
            activeTab: 0,
            autoTabs: true,
            autoTabSelector: 'div.Tab'
        });

        var grid = new Ext.grid.EditorGridPanel({
            sm: sm,
            cm: cm,
            fit: true,
            store: store,
            view: view,
            clicksToEdit: 'auto',
            renderTo: 'ReasonTable',
            bbar: footer,
            tbar: header,
            enableHdMenu: false,
            plugins: [new Ext.ux.FitToParent('ReasonTable')],
            enableColumnMove: false,
            listeners: {
                rowclick: onRowClick,
                rowmousedown: onRowMouseDown,
                afteredit: onCellEdited
            }
        });

        Ext.get('Approve').on('click', function () { grid.stopEditing(); saveStatus('Valid', sm); });
        Ext.get('Reject').on('click', function () { grid.stopEditing(); saveStatus('Invalid', sm); });

        cardSurrogate.addEvents('afterpost', 'afterbuild', 'beforepost', 'formbind');
        
        window.InitAdvertisementElement.call(cardSurrogate);
        cardSurrogate.refresh = refresh;
        window.Card = cardSurrogate;

        cardSurrogate.Mask = new window.Ext.LoadMask('PageContentCell');

        var depList = window.Ext.getDom("ViewConfig_DependencyList");
        if (depList.value) {
            this.DependencyHandler = new window.Ext.DoubleGis.DependencyHandler();
            this.DependencyHandler.register(window.Ext.decode(depList.value), window.EntityForm);
        }

        tabPanel.add({ xtype: "notepanel", pCardInfo: { pTypeName: 'AdvertisementElement', pId: window.Ext.getDom("Id").value } });
        tabPanel.add({ xtype: "actionshistorytab", pCardInfo: { pTypeName: 'AdvertisementElementStatus', pId: window.Ext.getDom("Id").value } });

        cardSurrogate.fireEvent('afterbuild');
    });

    function loadData(filterInput, callback) {
        Ext.get('Approve').disable();
        Ext.get('Reject').disable();
        var mask = new Ext.LoadMask('PageContentCell');
        mask.show();
        var params = {
            start: '0',
            limit: '65535',
            filterInput: filterInput,
            nameLocaleResourceId: 'DListAdvertisementElementDenialReasonForEdit',
            pId: Ext.get('Id').getValue(),
            pType: 'AdvertisementElementStatus',
            _dc: (new Date().getTime())
        };
        var url = Ext.urlAppend(Ext.BasicOperationsServiceRestUrl + 'List.svc/Rest/AdvertisementElementDenialReason', Ext.urlEncode(params));
        var request = new XMLHttpRequest();
        request.open('GET', url, true);
        request.withCredentials = true;
        request.onreadystatechange = function () {
            if (request.readyState == 4 && request.status == 200) {
                callback(Ext.util.JSON.decode(request.responseText));
                mask.hide();
                Ext.get('Approve').enable();
            } else if (request.readyState == 4) {
                alert("Не удалось получить данные:\n" + request.statusText);
            }
        };
        request.send();
    }

    function onRowMouseDown(grid, index, e) {
        var store = grid.getStore();
        var record = store.getAt(index);
        if (grid.editing) {
            return;
        }

        if (record.data.checked) {
            var sm = grid.getSelectionModel();
            sm.deselectRow(index);
        }
    }

    function onCellEdited(e) {
        var record = e.record;
        updateModifiedValues(e.grid.store.modifiedRecords, record);
    }

    function onRowClick(grid, index, e) {
        var store = grid.getStore();
        var record = store.getAt(index);
        if (grid.editing) {
            return;
        }

        if (!record.data.checked) {
            var sm = grid.getSelectionModel();
            sm.selectRow(index, true);
        }
    }

    function onStoreLoad(selectionModel) {
        return function(store, records, options) {
            var checkedRecords = [];
            store.each(function (record) {
                var recordId = record.data.reasonId.toString();
                var modifiedRecord = store.modifiedRecords[recordId];
                if (modifiedRecord != undefined) {
                    record.data.comment = modifiedRecord.comment;
                    record.data.checked = modifiedRecord.checked;
                    record.commit();
                }

                if (record.data.checked) {
                    checkedRecords.push(record);
                }
            });

            selectionModel.suspendEvents(true);
            selectionModel.selectRecords(checkedRecords);
            selectionModel.resumeEvents();
        }
    }

    function onBeforeRowSelect(a, b, c, record) {
        if (record.data.active) {
            return true;
        } else {
            return false;
        }
    };

    function onRowDeselect(sm, b, record) {
        record.data.checked = false;
        record.data.comment = null;
        updateModifiedValues(sm.grid.store.modifiedRecords, record);
        sm.grid.view.refreshRow(record);
    };

    function onRowSelect(sm, b, record) {
        record.data.checked = true;
        updateModifiedValues(sm.grid.store.modifiedRecords, record);
    };

    function updateModifiedValues(modifiedRecords, record) {
        var recordId = record.data.reasonId.toString();
        var value = {
            comment: record.data.comment,
            checked: record.data.checked
        }

        modifiedRecords[recordId] = value;
    }

    function onSelectionChange(sm) {
        var label = Ext.getCmp('summary-tbtext');
        var reasonsCount = sm.getCount();
        label.setText(String.format('Выбрано правил: {0}', reasonsCount));
        Ext.get('Reject').disable();
        for (var reasonId in sm.grid.store.modifiedRecords) {
            var record = sm.grid.store.modifiedRecords[reasonId];
            if (record.checked) {
                Ext.get('Reject').enable();
                return;
            }
        };
    }


    function saveStatus(status, sm) {
        var checkedRecords = [];
        for (var reasonId in sm.grid.store.modifiedRecords) {
            var record = sm.grid.store.modifiedRecords[reasonId];
            if (record.checked) {
                checkedRecords.push({
                    Id: reasonId,
                    Comment: record.comment
                });
            }
        };

        var form = document.forms[0];

        var isValid = Ext.DoubleGis.FormValidator.validate(form);
        if (!isValid) {
            addErrorNotification(Ext.LocalizedResources.ActionIsNotAvailableSinceThereIsAnError, 'CriticalError');
            return false;
        }

        Ext.get('Reasons').setValue(Ext.util.JSON.encode(checkedRecords));
        Ext.get('Status').setValue(status);

        var isRejectionAvailable = !Ext.get('Reject').dom.disabled;
        Ext.get('Approve').disable();
        Ext.get('Reject').disable();

        var mask = new Ext.LoadMask('PageContentCell');
        mask.show();

        cardSurrogate.fireEvent('beforepost');
        Ext.Ajax.request({
            url: form.action,
            method: 'POST',
            form: form,
            success: function () {
                mask.hide();
                if (window.opener && window.opener.AdvertisementBagPanel) {
                    window.opener.AdvertisementBagPanel.refresh();
                }
                window.close();
            },
            failure: function (response) {
                clearMessages();
                var model = Ext.decode(response.responseText);
                addErrorNotification(model.Message, model.MessageType);
                mask.hide();
                Ext.get('Approve').enable();
                if (isRejectionAvailable) {
                    Ext.get('Reject').enable();
                }
            },
            scope: this,
            // 5 minutes timeout
            timeout: 300000
        });
    };

    function addErrorNotification(message, level)
    {
        var templateParameters = { message: message, level: window.Ext.Notification.Icon[level], messageId: 'ServerNotification' };
        var template = new Ext.XTemplate(
            '<div id="{messageId}" class="Notification">',
            '<table cellspacing="0" cellpadding="0"><tbody><tr><td valign="top">' +
                '<img class="ms-crm-Lookup-Item" alt="" src="{level}"/>',
            '</td><td width="5px"></td><td><span id="NotificationText">{message}</span>',
            '</td></tr></tbody></table></div>');

        var notifications = Ext.getDom("Notifications");
        notifications.style.display = "block";
        template.append(notifications, templateParameters);
    };

    function refresh(deepRefresh) {
        window.location.reload();
    };

    function clearMessages() {
        var message = Ext.getDom("ServerNotification");
        if (message) {
            message.parentNode.removeChild(message);
        }
        var notifications = Ext.getDom("Notifications");
        notifications.style.display = "none";
    };
})();
