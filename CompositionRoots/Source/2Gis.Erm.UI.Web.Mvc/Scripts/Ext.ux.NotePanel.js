Ext.namespace('Ext.ux');
Ext.ux.NotePanel = Ext.extend(Ext.Panel, {
    constructor: function (config)
    {
        config = config || {};
        config.title = Ext.LocalizedResources.Notes;
        config.id = "notesTab";
        config.listeners = {
            beforedestroy: { fn: this.cleanupNoteControl, scope: this },
            beforeshow: { fn: this.beforeShow, scope: this }
        };
        config.items = [];
        window.Ext.ux.NotePanel.superclass.constructor.call(this, config);
    },
    // #region routines
    beforeShow: function ()
    {
        if (this.contentRendered)
            this.refreshList();
        else this.renderContent();
    },
    renderContent: function ()
    {
        window.Entity = this;
        this.add(new window.Ext.Panel(
            {
                id: 'notesBody',
                layout: 'fit',
                headerCfg: { cls: "x-notes-title", html: "<div class='x-notes-hr'><span>" + Ext.LocalizedResources.Notes + "</span></div>" },
                bodyCfg: { cls: "x-notes-body" },
                plugins: [new window.Ext.ux.FitToParent("notesTab")],
                items: [this.renderNotesList(this.pCardInfo)]
            }));
        this.doLayout();
        this.contentRendered = true;
    },
    cleanupNoteControl: function ()
    {
        if (this.ctxMenu)
        {
            this.ctxMenu.destroy();
            this.ctxMenu = null;
        }
    },
    renderNotesList: function (cardCfg)
    {
        var headRowText = '<div style="height:20px;"><span id="addNoteLink" class="x-note-link">' + Ext.LocalizedResources.ClickHereToAddNote + '</span></div>' +
            '<div id="newMessageWrapper" style="display:none"><textarea rows="3" class="inputfields"></textarea></div><hr/>';
        var store = new window.Ext.data.Store({
            xtype: 'jsonstore',
            url: "/Note/GetEntityNotes/" + cardCfg.pTypeName + "/" + cardCfg.pId,

            reader: new window.Ext.data.JsonReader({
                idProperty: 'Id',
                root: 'items',
                totalProperty: 'RowCount',
                fields: [
                    { name: "id", mapping: "Id" },
                    { name: "title", mapping: "Title" },
                    { name: "text", mapping: "Text" },
                    { name: "fileid", mapping: "FileId" },
                    { name: "filename", mapping: "FileName" },
                    { name: "createdon", mapping: "CreatedOn" },
                    { name: "createdby", mapping: "CreatedBy" },
                    { name: "modifiedon", mapping: "ModifiedOn" },
                    { name: "modifiedby", mapping: "ModifiedBy" },
                    { name: "readonly", mapping: "Readonly" }
                ]
            })
        });
        store.load();

        var tpl = new window.Ext.XTemplate(
        headRowText,
        '<tpl for=".">',
        '<div class="x-notes-thumb" id="{id}">',
        '<tpl if="filename"><img alt="" src="' + Ext.DoubleGis.Global.Helpers.GetEntityIconPath("en_ico_16_Note.gif") + '" class="x-note"/></tpl><tpl if="!filename"><img alt="" src="' + Ext.DoubleGis.Global.Helpers.GetEntityIconPath("en_ico_16_Note.gif") + '" class="x-note"/></tpl>',
        '<span class="x-notes-thumb">' + Ext.LocalizedResources.Title + ': {title}</span>&nbsp;<br>',
        '<span class="x-note-created">' + Ext.LocalizedResources.NoteCreated + ': {createdon:dateWOffset()} {createdby}</span>&nbsp;',
        '<tpl if="modifiedon"><span class="x-note-modified">' + Ext.LocalizedResources.Modified + ': {modifiedon:dateWOffset()} {modifiedby}</span>&nbsp;</tpl>',
        '<tpl if="filename"><br/><img id="fileLink" alt="" src="' + Ext.DoubleGis.Global.Helpers.GetStaticImagePath("CommonUI/attach.png") + '" class="x-note"/><span id="fileLink" class="x-note-link">{filename}</span></tpl>',
        '<br/><br/><textarea readonly="true" class="x-note-text">{text}</textarea>',
        '</div><hr/>',
        '</tpl>',
        '<div class="x-clear"></div>');

        this.notesView = new window.Ext.DataView({
            tpl: tpl,
            store: store,
            anchor: "100%,100%",
            singleSelect: true,
            loadingText: Ext.LocalizedResources.IndicatorText,
            overClass: 'x-notes-thumb-over',
            selectedClass: 'x-notes-thumb-selected',
            itemSelector: 'div.x-notes-thumb',
            style: 'overflow:auto; background-color: #FFFFFF;border: #6699cc 1px solid;',
            emptyText: headRowText,
            listeners: {
                contextmenu: { fn: this.showContextMenu, scope: this },
                containercontextmenu: { fn: this.showCtnContextMenu, scope: this },
                dblclick: { fn: this.openNote, scope: this },
                containerclick: { fn: this.containerClickHandler, scope: this },
                click: { fn: this.maybeLink, scope: this }
            }
        });
        return this.notesView;
    },
    maybeLink: function (view, index, node, evt)
    {
        if (evt.target.id == "fileLink")
        {
            var url = Ext.BasicOperationsServiceRestUrl + 'DownloadBinary.svc/Rest/Note/' + view.getRecord(node).data.fileid;
            var fileFrame;
            fileFrame = Ext.get("hiddenDownloader");
            if (fileFrame === null)
            {
                fileFrame = window.Ext.getBody()
                    .createChild({
                        tag: "iframe",
                        id: "hiddenDownloader",
                        style: "visibility: hidden;"
                    });
                fileFrame.on("load", function ()
                {
                    var iframeContent = this.dom.contentWindow.document.documentElement.innerText;
                    if (iframeContent != "")
                    {
                        Ext.MessageBox.show({
                            title: Ext.LocalizedResources.Error,
                            msg: iframeContent,
                            width: 300,
                            buttons: window.Ext.MessageBox.OK,
                            icon: window.Ext.MessageBox.ERROR
                        });
                    }
                });
            }
            fileFrame.dom.contentWindow.location.href = url;
        }
        var nodeEl = Ext.get(node);
        nodeEl.removeAllListeners();
        nodeEl.on("keyup", function (e, el)
        {
            e.stopPropagation();
            if (e.keyCode == e.DELETE)
            {
                this.deleteNote();
            }
            if (e.keyCode == e.ENTER)
            {
                this.openNote();
            }

        }, this);
        nodeEl.on("focusout", function (e, el)
        {
            this.removeAllListeners();
        }, undefined, { single: true });
    },
    containerClickHandler: function (view, evt)
    {
        if (evt.target.id == "addNoteLink")
        {
            this.addNote();
        }
    },
    //#endregion
    //#region crud
    openNote: function ()
    {
        var record = this.notesView.getSelectedRecords()[0];
        var params = String.format("width={0},height={1},status=no,resizable=yes,top={2},left={3}", window.Ext.DoubleGis.Global.UISettings.ActualCardWidth * 0.7, window.Ext.DoubleGis.Global.UISettings.ActualCardHeight * 0.7, window.Ext.DoubleGis.Global.UISettings.ScreenCenterTop, window.Ext.DoubleGis.Global.UISettings.ScreenCenterLeft);
        var queryString = '?ReadOnly=' + (record.data.readonly);
        var sUrl = Ext.DoubleGis.Global.Helpers.EvaluateUpdateEntityUrl('Note', record.id, queryString);
        window.open(sUrl, "_blank", params);

    },
    deleteNote: function ()
    {
        var vals = [this.notesView.getSelectedRecords()[0].id];
        var parameters = {
            Values: vals,
            DoSpecialConfirmation: false
        };

        var result = window.showModalDialog("/GroupOperation/Delete/Note", parameters, "dialogWidth:500px; dialogHeight:203px; scroll:no;resizable:no;");
        if (result == "OK")
        {
            this.notesView.store.reload();
        }

    },
    addNote: function ()
    {
        if (!Ext.get("newMessageWrapper").isVisible())
            Ext.get("newMessageWrapper").fadeIn({ useDisplay: true, duration: 0.5 }).child("textarea").addListener("focusout", this.createNewNote, this, { single: true }).focus();
    },
    createNewNote: function (evt, field)
    {
        if (field.value) {
            var sUrl = Ext.DoubleGis.Global.Helpers.EvaluateCreateEntityUrl('Note', '');
            window.Ext.Ajax.request({
                timeout: 1200000,
                url: sUrl,
                method: "POST",
                scope: this,
                params: { Id: 0, ParentId: this.pCardInfo.pId, ParentTypeName: this.pCardInfo.pTypeName, Title: Ext.LocalizedResources.NoteCreated + ': ' + new Date().format(Ext.CultureInfo.DateTimeFormatInfo.PhpFullDateTimePattern), Text: field.value },
                success: function (jsonResponse) { this.refreshList(); },
                failure: function (xhr)
                {
                    Ext.MessageBox.show({
                        title: Ext.LocalizedResources.Error,
                        msg: xhr.responseText,
                        width: 300,
                        buttons: window.Ext.MessageBox.OK,
                        icon: window.Ext.MessageBox.ERROR
                    });
                    this.refreshList();
                }
            });
        }
        Ext.get("newMessageWrapper").fadeOut({ useDisplay: true, duration: 0.5 });

    },
    printNote: Ext.emtyFn,
    refreshList: function ()
    {
        this.notesView.store.reload();
    },
    refresh: function () { this.refreshList(); },
    //#endregion
    //#region ctx
    showContextMenu: function (view, index, node, evt)
    {
        evt.stopEvent();
        view.select(node, false, false);
        if (!this.ctxMenu)
        {
            this.ctxMenu = this.buildCtxMenu();
        }
        var deleteItem = this.ctxMenu.getComponent('delete');
        var readonly = view.store.data.items[index].data.readonly;
        readonly ? deleteItem.disable() : deleteItem.enable();
        readonly ? deleteItem.setIconClass('x-note-ctx-delete-disabled') : deleteItem.setIconClass('x-note-ctx-delete');

        this.ctxMenu.showAt(evt.getXY());
    },
    showCtnContextMenu: function (view, evt)
    {
        evt.stopEvent();
        if (!this.cntCtxMenu)
        {
            this.cntCtxMenu = this.buildCntCtxMenu();
        }
        this.cntCtxMenu.showAt(evt.getXY());
    },
    buildCtxMenu: function ()
    {
        return new Ext.menu.Menu({
            items: [
                {
                    itemId: 'edit',
                    handler: this.openNote,
                    text: Ext.LocalizedResources.OpenText,
                    iconCls: 'x-note-ctx-open',
                    scope: this
                },
                {
                    itemId: 'delete',
                    handler: this.deleteNote,
                    text: Ext.LocalizedResources.DeleteText,
                    iconCls: 'x-note-ctx-delete',
                    scope: this
                },
                {
                    itemId: 'print',
                    handler: this.printNote,
                    text: Ext.LocalizedResources.PrintText,
                    iconCls: 'x-note-ctx-print',
                    scope: this
                },
                {
                    itemId: 'refresh',
                    handler: this.refreshList,
                    text: Ext.LocalizedResources.RefreshText,
                    iconCls: 'x-note-ctx-refresh',
                    scope: this
                }]
        });
    },
    buildCntCtxMenu: function ()
    {
        return new Ext.menu.Menu({
            items: [
                {
                    itemId: 'edit',
                    handler: this.addNote,
                    text: Ext.LocalizedResources.CreateText,
                    iconCls: 'x-note-ctx-open',
                    scope: this
                },
                {
                    itemId: 'refresh',
                    handler: this.refreshList,
                    text: Ext.LocalizedResources.RefreshText,
                    iconCls: 'x-note-ctx-refresh',
                    scope: this
                }]
        });
    }
    //#endregion
});

Ext.reg('notepanel', Ext.ux.NotePanel);