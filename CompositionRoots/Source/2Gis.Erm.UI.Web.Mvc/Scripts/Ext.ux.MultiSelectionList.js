Ext.ns('Ext.ux');
Ext.ux.MultiSelectionList = Ext.extend(Ext.Panel, {
    showCreateButton: true,
    constructor: function(config) {
        config = config || {};
        this.addEvents("beforebuild", "afterbuild", "beforecreate", "beforeedit", "beforerefresh");

        this.viewSettings = config.viewSettings;
        this.entityModel = config.entityModel;
        this.extendedInfo = config.extendedInfo;
        this.initReaderFieldSet();
        if (config.honourDataListFields) {
            this.initColumnSetFromDataList();
        } else {
            this.initColumnSetDefault();
        }
        
        this.initStore();


        Ext.apply(config, {
            layout: {
                type: 'hbox',
                pack: 'start',
                align: 'stretch',
                padding: '5'
            },
            bodyStyle: 'background-color:#e3efff;',
            bbarCfg: { tagName: 'div', cls: 'x-modal-toolbar' },
            bbar: new Ext.Toolbar({
                items: [
                    this.btnProp = new Ext.ux.TabularButton({
                        xtype: 'tabularbutton',
                        text: Ext.LocalizedResources.Properties,
                        handler: this.editItem,
                        scope: this
                    }),
                    this.btnCreate = new Ext.ux.TabularButton({
                        xtype: 'tabularbutton',
                        text: Ext.LocalizedResources.Create,
                        handler: this.createItem,
                        disabled: config.showCreateButton,
                        scope: this
                    })
                ]
            }),
            headerCfg: {
                tagName: 'div',
                cls: 'search-header',
                html: '<div style="width: 50%; float: left;"><div style="font-weight: bold; width: 50px; float: left; padding-top: 3px;">' + Ext.LocalizedResources.Search + '</div>' +
                    '<div style="float: left;"><select class="inputfields" disabled="disabled"></select></div>' +
                    '</div><div style="width: 49%; float: right; margin-top: -1px;">' +
                    '<div style="width: 10px; float: left;"></div>' +
                    '<div style="float: left;"><input type="text" class="inputfields x-search" id="searchInput"/></div></div>' +
                    '<div class="Notifications" style="clear: both;visibility:hidden;"></div><div style="clear:both;border-bottom:#cccccc 1px solid;"></div>'
            },
            headerAsText: false,
            items: [
                this.fromList = new Ext.list.ListView(
                    {
                        id: 'grid-left',
                        deferEmptyText: false,
                        emptyText: '<table style="width:100%;height:100%;"><tr><td style="height:100%;text-align: center;">' + Ext.LocalizedResources.MultiSelectionListFromListEmptyText + '</td></tr></table>',
                        cls: 'x-multiselect-list-body',
                        flex: 1,
                        multiSelect: true,
                        store: this.store,
                        columns: this.fromColumnSet,
                        listeners: {
                            afterRender: this.afterListRendered,
                            click: this.checkList,
                            dblclick: this.onRowDblClick,
                            scope: this
                        }
                    }),
                {
                    width: 60,
                    bodyStyle: 'background-color:#e3efff;border:none;',
                    html: '<table style="height:100%;width:60px;"><tr><td><p style="padding:10px;"><input style="width:40px;" type="button" class="CrmButton" value=">>" id="btnAppend"/></p><p style="padding:10px;"><input style="width:40px;" type="button" class="CrmButton" value="<<" id="btnRemove"/></p></td><tr><table>',
                    listeners: {
                        afterRender: this.afterMiddleBarRendered,
                        scope: this
                    }
                },
                this.toList = new Ext.list.ListView(
                    {
                        deferEmptyText: false,
                        emptyText: '<table style="width:100%;height:100%;"><tr><td style="height:100%;text-align: center;color:gray;">' + Ext.LocalizedResources.MultiSelectionListToListEmptyText + '</td></tr></table>',
                        flex: 1,
                        cls: 'x-multiselect-list-body',
                        multiSelect: true,
                        store: new Ext.data.ArrayStore({ fields: this.rdrFieldSet }),
                        columns: this.toColumnSet,
                        listeners: {
                            afterRender: this.afterListRendered,
                            click: this.checkList,
                            dblclick: this.onRowDblClick,
                            scope: this
                        }
                    })]
        });
        window.Ext.ux.SearchFormMultiple.superclass.constructor.call(this, config);
    },
    initReaderFieldSet: function () {
        this.rdrFieldSet = [];
        window.Ext.each(this.viewSettings.Fields, function(field, i) {
            this.rdrFieldSet.push(new Object({ name: field.Name }));
        }, this);
    },
    initColumnSetDefault: function () {
        this.fromColumnSet = [];
        this.toColumnSet = [];
        var imageTag = this.formatEntityIconTag();

        this.addColumn(this.fromColumnSet, Ext.LocalizedResources.AvailableRecords, this.viewSettings.MainAttribute, imageTag);
        this.addColumn(this.toColumnSet, Ext.LocalizedResources.SelectedRecords, this.viewSettings.MainAttribute, imageTag);
    },
    initColumnSetFromDataList: function () {
        this.fromColumnSet = [];
        this.toColumnSet = [];
        var imageTag = this.formatEntityIconTag();

        Ext.each(this.viewSettings.Fields, function (field, i) {
            if (!field.Hidden) {
                this.addColumn(this.fromColumnSet, field.LocalizedName, field.Name, imageTag);
                this.addColumn(this.toColumnSet, field.LocalizedName, field.Name, imageTag);
                imageTag = ''; // image only on first visible column
            }
        }, this);
    },
    formatEntityIconTag: function () {
        return "<img class='x-multiselect-icon' src='" + window.Ext.DoubleGis.Global.Helpers.GetEntityIconPath(this.viewSettings.Icon) + "'/>";
    },
    addColumn: function (targetArray, localizedName, name, imageTag) {
        targetArray.push({
            header: localizedName,
            tpl: "<nobr unselectable='on'>" + imageTag + "{" + name + "}" + "</nobr>"
        });
    },
    initStore: function() {
        var qstringparams = this.parseWindowLocation();

        this.store = new Ext.DoubleGis.Store({
            remoteSort: true,
            //Ставим false потому, что при загрузке страницы будет присобачен фильтр
            autoLoad: false,
            reader: new window.Ext.data.JsonReader({
                root: 'Data',
                totalProperty: 'RowCount',
                fields: this.rdrFieldSet
            }),
            proxy: new window.Ext.data.HttpProxy({
                method: "GET",
                url: Ext.BasicOperationsServiceRestUrl + "List.svc/Rest/" + this.entityModel.EntityName
            }),
            baseParams: new Object({
                nameLocaleResourceId: this.viewSettings.NameLocaleResourceId,
                extendedInfo: this.extendedInfo,
                filterInput: "",
                start: 0,
                pId: qstringparams.parentEntityId,
                pType: qstringparams.parentEntityType,
                limit: 100,
                dir: this.viewSettings.DefaultSortDirection == 0 ? "ASC" : "DESC",
                sort: this.viewSettings.DefaultSortField
            }),
            listeners:
            {
                exception: function(e) {
                    Ext.MessageBox.show({
                        title: Ext.LocalizedResources.GetDataError,
                        msg: Ext.LocalizedResources.ErrorDuringOperation,
                        buttons: Ext.MessageBox.OK,
                        icon: Ext.MessageBox.ERROR
                    });
                    this.fromList.store.removeAll(true);
                },
                beforeload: this.beforeStoreLoad,
                load: this.onStoreLoad,
                scope: this
            }
        });
    },
    createItem: function() {
        if (this.fireEvent("beforecreate", this) === false) {
            return;
        }

        var parentExp = "";
        var qstringparams = window.Ext.urlDecode(location.search.substring(1));
        if (qstringparams.pId && qstringparams.pType) {
            parentExp = String.format("&pId={0}&pType={1}", qstringparams.pId, qstringparams.pType);
        }
        if (this.EntityModel && !this.EntityModel.HasCard) {
            window.Ext.MessageBox.show({
                title: '',
                msg: window.Ext.LocalizedResources.CardIsUndefined,
                width: 300,
                buttons: window.Ext.MessageBox.OK,
                icon: window.Ext.MessageBox.ERROR
            });
            return;
        }
        if (this.viewSettings.ReadOnly) {
            window.Ext.MessageBox.show({
                title: '',
                msg: window.Ext.LocalizedResources.ItsReadOnlyMode,
                width: 300,
                buttons: window.Ext.MessageBox.OK,
                icon: window.Ext.MessageBox.ERROR
            });
            return;
        }
        var params = String.format("width={0},height={1},status=no,resizable=yes,top={2},left={3}", window.Ext.DoubleGis.Global.UISettings.ActualCardWidth, window.Ext.DoubleGis.Global.UISettings.ActualCardHeight, window.Ext.DoubleGis.Global.UISettings.ScreenCenterTop, window.Ext.DoubleGis.Global.UISettings.ScreenCenterLeft);
        var queryString = '?ReadOnly=' + this.viewSettings.ReadOnly + parentExp + "&extendedInfo=" + encodeURIComponent(this.extendedInfo);
        var sUrl = Ext.DoubleGis.Global.Helpers.EvaluateCreateEntityUrl(this.entityModel.EntityName, queryString);
        window.open(sUrl, '_blank', params);
    },
    editItem: function() {
        if (this.fireEvent("beforeedit", this) === false) {
            return;
        }
        if (this.fromList.getSelectedRecords().length < 1) {
            return;
        }

        if (this.EntityModel && !this.EntityModel.HasCard) {
            window.Ext.MessageBox.show({
                title: '',
                msg: window.Ext.LocalizedResources.CardIsUndefined,
                width: 300,
                buttons: window.Ext.MessageBox.OK,
                icon: window.Ext.MessageBox.ERROR
            });
            return;
        }

        var parentExp = "";
        var qstringparams = window.Ext.urlDecode(location.search.substring(1));
        if (qstringparams.pId && qstringparams.pType) {
            parentExp = String.format("&pId={0}&pType={1}", qstringparams.pId, qstringparams.pType);
        }
        var params = String.format("width={0},height={1},status=no,resizable=yes,top={2},left={3}", window.Ext.DoubleGis.Global.UISettings.ActualCardWidth, window.Ext.DoubleGis.Global.UISettings.ActualCardHeight, window.Ext.DoubleGis.Global.UISettings.ScreenCenterTop, window.Ext.DoubleGis.Global.UISettings.ScreenCenterLeft);
        var queryString = '?ReadOnly=' + this.viewSettings.ReadOnly + parentExp;
        var sUrl = Ext.DoubleGis.Global.Helpers.EvaluateUpdateEntityUrl(this.entityModel.EntityName, this.fromList.getSelectedRecords()[0].data.Id, queryString);
        window.open(sUrl, '_blank', params);
    },
    checkList: function() {
        this.btnAppend.dom.disabled = this.fromList.getSelectedRecords().length == 0 ? "disabled" : null;
        this.btnRemove.dom.disabled = this.toList.getSelectedRecords().length == 0 ? "disabled" : null;
        if (this.fromList.getSelectedRecords().length == 0 || this.fromList.getSelectedRecords().length > 1) {
            this.btnProp.disable();
        } else if (this.entityModel && this.entityModel.HasCard) {
            this.btnProp.enable();
        }
    },
    afterListRendered: function(cmp) {
        cmp.dragGroup = cmp.dropGroup = 'MultiselectDD';
        cmp.dragZone = new Ext.ux.MultiSelectionList.DragZone(cmp.el, {
            ddGroup: cmp.dragGroup,
            view: cmp,
            removeFromSource: cmp == this.toList
        });
        cmp.dropZone = new Ext.ux.MultiSelectionList.DropZone(cmp.el,
            {
                ddGroup: cmp.dropGroup,
                view: cmp
            });
    },
    afterMiddleBarRendered: function(cmp) {
        this.btnAppend = cmp.el.child("#btnAppend").on("click", this.appendSelection, this);
        this.btnRemove = cmp.el.child("#btnRemove").on("click", this.removeSelection, this);
        this.checkList();
    },
    afterRender: function() {
        Ext.ux.MultiSelectionList.superclass.afterRender.call(this);
        var s = new Ext.ux.SearchControl({ applyTo: 'searchInput' });
        s.on('trigger', this.searchRecords, this);
        this.header.setStyle('-moz-user-select', 'text');
        this.header.removeClass('x-unselectable');
        this.header.dom.unselectable = "off";
        this.header.removeAllListeners();
    },
    appendSelection: function() {
        var records = this.fromList.getSelectedRecords();
        if (records.length > 0) {
            Ext.each(records, function(record) {
                var found = this.toList.store.findBy(function(r) {
                    return r.data.Id === record.data.Id;

                });
                if (found == -1) {
                    this.toList.store.add(record);
                }
            }, this);

        }
        this.toList.select(records);
        this.fromList.clearSelections();
        this.checkList();
    },
    removeSelection: function() {
        var records = this.toList.getSelectedRecords();
        if (records.length > 0) {
            Ext.each(records, function(record) {
                this.toList.store.remove(record);
            }, this);

        }
        this.fromList.select(records);
        this.toList.clearSelections();
        this.checkList();
    },
    getValue: function() {
        var items = [];
        var records = this.toList.store.data.items;
        if (records.length) {
            Ext.each(records,
                function(record) {
                    items.push({
                        id: record.data.Id,
                        name: record.data[this.viewSettings.MainAttribute],
                        data: record.data
                    });
                }, this);
            window.returnValue = { items: [item] };
            window.close();
        }
        return items;
    },
    refresh: function() {
        this.fromList.store.reload();
    },
    searchRecords: function(el, value) {
        this.fromList.store.setBaseParam("filterInput", value);
        this.fromList.store.setBaseParam("extendedInfo", this.extendedInfo);
        this.fromList.store.reload();
    },
    onRowDblClick: function(vw, index, node, e) {
        if (vw == this.toList) {
            this.removeSelection();
        } else if (vw == this.fromList) {
            this.appendSelection();
        }
        return this.fireEvent('rowdblclick', vw, index, node, e);
    },
    beforeStoreLoad: function(store, operation, eOpts) {
        if (!this.mask) {
            this.mask = new window.Ext.LoadMask(window.Ext.get("grid-left"));
        }

        this.mask.show();
    },
    onStoreLoad: function(store, records, options) {
        this.mask.hide();
        if (records.length >= 100) {
            this.addNotification(Ext.LocalizedResources.MultiSelectionListTooManyRecordsAlert, 'Info', 'TooManyRecords');
        } else {
            this.removeNotification('TooManyRecords');
        }
    },
    addNotification: function(message, level, messageId) {
        var nopt = { message: message, level: window.Ext.Notification.Icon[level], messageId: messageId };
        var header = this.header.child('.Notifications');
        if (!this.NotificationTemplate) {
            this.NotificationTemplate = new Ext.XTemplate(
                '<div id="{messageId}" class="Notification">',
                '<table cellspacing="0" cellpadding="0"><tbody><tr><td valign="top">' +
                    '<img class="ms-crm-Lookup-Item" alt="" src="{level}"/>',
                '</td><td width="5px"></td><td><span id="NotificationText">{message}</span>',
                '</td></tr></tbody></table></div>');
        }
        header.show(true).dom.innerHTML = "";
        this.NotificationTemplate.append(header, nopt);
        this.doLayout();
    },
    removeNotification: function(messageId) {
        var header = this.header.child('.Notifications');
        var msg = header.child('#' + messageId);
        if (msg)
            msg.remove();
        if (!header.child('.Notification')) {
            header.hide(true);
        }
        this.doLayout();
    },
    parseWindowLocation: function() {

        // General route looks like this:
        // <URI>/Grid/SearchMultiple/{entityTypeName}/{parentEntityType}/{parentEntityId}
        
        var urlComponents = location.pathname.split('/');

        // Removing last slash component
        if (urlComponents[urlComponents.length - 1] == '') {
            urlComponents.splice(urlComponents.length - 1, 1);
        }

        // Removing first slash component
        if (urlComponents[0] == '') {
            urlComponents.splice(0, 1);
        }

        return {
            entityTypeName: urlComponents[2],
            parentEntityType: urlComponents[3],
            parentEntityId: urlComponents[4]
        };
    }
});
Ext.reg('multiselectionlist', Ext.ux.MultiSelectionList);
//backwards compat
Ext.ux.Multiselect = Ext.ux.MultiSelectionList;
Ext.ux.MultiSelectionList.DragZone = Ext.extend(Ext.dd.DragZone, {
    containerScroll: false,
    scroll: false,
    getDragData: function (evt)
    {
        var dataView = this.view;
        var srcEl = evt.getTarget(dataView.itemSelector, 10);
        if (srcEl)
        {
            var selectedNodes = dataView.getSelectedNodes();
            var ddEl = document.createElement('div');
            ddEl.style.width = "250px";
            if (selectedNodes.length < 1)
            {
                selectedNodes.push(srcEl);
            }
            Ext.each(selectedNodes, function (node)
            {
                ddEl.appendChild(node.cloneNode(true));
            });
            return {
                ddel: ddEl,
                repairXY: Ext.fly(srcEl).getXY(),
                dragRecords: dataView.getSelectedRecords(),
                sourceDataView: dataView,
                removeFromSource: this.removeFromSource
            };
        }
    },
    getRepairXY: function ()
    {
        return this.dragData.repairXY;
    }
});
Ext.ux.MultiSelectionList.DropZone = Ext.extend(Ext.dd.DropZone, {
    onContainerOver: function ()
    {
        return this.dropAllowed;
    },
    onContainerDrop: function (dropZone, evt, dragData)
    {
        if (dragData.sourceDataView != this.view)
        {
            var dragRecords = dragData.dragRecords;
            var store = this.view.store;
            Ext.each(dragRecords, function (record)
            {
                var found = store.findBy(function (r)
                {
                    return record ? r.data.Id === record.data.Id : 0;
                });
                if (found == -1)
                {
                    this.view.store.add(record);
                }
            }, this);
            if (dragData.removeFromSource)
            {
                dragData.sourceDataView.store.remove(dragRecords);
            }

        }
        dragData.sourceDataView.clearSelections();
        return true;
    }
});