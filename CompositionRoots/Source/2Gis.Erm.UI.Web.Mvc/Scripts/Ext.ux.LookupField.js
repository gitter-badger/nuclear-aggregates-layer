Ext.namespace('Ext.ux');
Ext.ux.LookupField = Ext.extend(Ext.Component, {
    btnDis: Ext.DoubleGis.Global.Helpers.GetStaticImagePath("CommonUI/btn_dis_lookup.gif"),
    btnOff: Ext.DoubleGis.Global.Helpers.GetStaticImagePath("CommonUI/btn_off_lookup.gif"),
    btnOn: Ext.DoubleGis.Global.Helpers.GetStaticImagePath("CommonUI/btn_on_lookup.gif"),
    btnResolving: Ext.DoubleGis.Global.Helpers.GetStaticImagePath("CommonUI/btn_lookup_resolving.gif"),
    emptyText: Ext.LocalizedResources.LookupEmptyValue,

    extendedInfo: "",
    showReadOnlyCard: false,
    entityName: "",
    entityIcon: "",
    thumbData: null,
    thumbPanel: undefined,
    item: undefined,
    isValid: true,
    errorMessage: true,
    tabIndex: -1,
    readOnly: false,
    supressMatchesErrors: false,
    parentEntityName: "None",
    parentIdPattern: "",
    
    initComponent: function ()
    {
        window.Ext.ux.LookupField.superclass.initComponent.call(this);
        this.addEvents("afterselect", "beforequery", "afterquery", "beforechange", "change", "contentkeyup", "contentkeypress");
    },

    onRender: function ()
    {
        this.entityIcon = window.Ext.DoubleGis.Global.Helpers.GetEntityIconPath(String.format("en_ico_16_{0}.gif", this.entityName));

        this.tabIndex = this.el.dom.tabIndex;

        this.deserialize();
        this.renderBody();

        this.imgItem = window.Ext.get(this.name + "_LookupImage");
        this.linkItem = window.Ext.get(this.name + "_LookupLink");
        this.searchBtn = window.Ext.get(this.name + "_Btn");
        this.filter = window.Ext.get(this.name + "_Filter");
        this.content = window.Ext.get(this.name + "_Content");
        this.wrapper = window.Ext.get(this.name + "_Wrapper");
        this.thumbEl = window.Ext.get(this.name + "_Thumb");

        this.content.dom.tabIndex = this.tabIndex;

        this.renderItems();

        this.searchUrl = "/Grid/Search/" + this.entityName;
        this.setReadOnly(this.readOnly);
        this.setDisabled(this.disabled);
        this.searchBtn.dom.src = this.disabled || this.readOnly ? this.btnDis : this.btnOff;
        this.initHandlers();
    },
    renderItems: function ()
    {
        this.filter.dom.value = "";
        this.serialize();
        this.setValid();
    },
    renderBody: function ()
    {
        var template = new window.Ext.Template(
                '<table id="{name}_Wrapper" class="x-lookup"><tbody><tr>',
                    '<td>',
                    '<div id="{name}_Content" class="x-lookup" style="display: none;">',
                    '<img alt="" id="{name}_LookupImage" src="{entityIcon}" class="x-lookup-item" onerror="Ext.getCmp(\'{id}\').imageItemError()"/>',
                    '<span id="{name}_LookupLink" class="x-lookup-item">',
                    '</span>&nbsp;</div>',
                    '<input type="text" id="{name}_Filter" class="x-lookup-normal inputfields"/>',
        // #note строка ниже есть предмет для рефакторинга 
                    '<input type="hidden" id="{name}Id"/>',
                    '<input type="hidden" id="{name}Name"/>',
                    '</td>',
                    '<td width="22">',
                    '<img id="{name}_Btn" alt="" title="" class="x-lookup" src="{btnOff}" />',
                    '</td>',
                    '</tr></tbody></table>',
                    '<div id="{name}_Thumb" style="display:none;border:1px solid #6699cc;z-index:100;position:absolute;"></div>',
                    {
                        compiled: true,
                        disableFormats: true
                    }
                );
        template.insertBefore(this.el.dom, this);
        this.el.addClass('x-lookup');
        this.el.dom.style.display = "none";
    },
    initHandlers: function ()
    {
        this.imgItem.on("click", function () { this.dom.nextSibling.click(); });

        this.content.on("keypress", this.contentKeyPress, this);
        this.content.on("dblclick", this.contentKeyPress, this);
        this.content.on("keyup", this.contentKeyUp, this);
        this.content.on("focusin", this.contentFocusIn, this);
        this.content.on("focusout", this.contentFocusOut, this, { delay: 100 });

        this.linkItem.on("click", this.openEntityCard, this);
        this.searchBtn.on("mouseout", this.setBtnOff, this);
        this.searchBtn.on("mouseover", this.setBtnOn, this);
        this.searchBtn.on("click", this.openSearchWin, this);

        this.filter.on("keydown", function (e) { if (e.keyCode == e.ENTER) { this.searchBtn.dom.click(); e.stopPropagation(); e.stopEvent(); } }, this);
        this.filter.on("focusout", this.filterFocusOut, this);
    },
    beforeDestroy: function ()
    {
        if (this.thumbPanel)
        {
            this.thumbEl.hide();
            this.thumbPanel.destroy();
            this.thumbPanel = null;
        }

        if (this.thumbEl) this.thumbEl.removeAllListeners();
        if (this.imgItem) this.imgItem.removeAllListeners();
        if (this.content) this.content.removeAllListeners();
        if (this.linkItem) this.linkItem.removeAllListeners();
        if (this.searchBtn) this.searchBtn.removeAllListeners();
        if (this.filter) this.filter.removeAllListeners();

        this.wrapper.remove();
        this.thumbEl.remove();
    },
    deserialize: function ()
    {
        var value;
        try
        {
            value = window.Ext.decode(this.getRawValue());
            if (value.Key)
            {
                this.item = { id: value.Key, name: value.Value };
            }
            else
            {
                this.item = undefined;
            }
        }
        catch (e)
        {
            this.item = undefined;
        }
    },
    serialize: function ()
    {
        try
        {
            this.el.dom.value = this.item && this.item.id ? window.Ext.encode({ Key: this.item.id, Value: (this.item.name || this.emptyText) == this.emptyText ? "" : this.item.name }) : "";
            Ext.getDom(this.name + "Id").value = this.item && this.item.id || "";
            Ext.getDom(this.name + "Name").value = this.item && this.item.name || "";
        }
        catch (e)
        {
            this.el.dom.value = "";
        }
    },
    getDataFromServer: function (config, silent)
    {
        if (this.fireEvent("beforequery", this) === false)
        {
            this.clearValue();
            return;
        }
        this.searchBtn.dom.src = this.disabled || this.readOnly ? this.btnDis : this.btnResolving;
        
        var queryStringParams = {
            start: 0,
            filterInput: null,
            extendedInfo: null,
            nameLocaleResourceId: null,
            limit: 5,
            sort: 'Id DESC',
            pType: this.parentEntityName,
            pId: this.parentIdPattern
        };

        Ext.apply(queryStringParams, config);

        if (queryStringParams.extendedInfo) {
            queryStringParams.extendedInfo = this.prepareFilterExpression(queryStringParams.extendedInfo);
        }

        if (this.extendedInfo)
        {
            queryStringParams.extendedInfo += '&' + this.prepareFilterExpression(this.extendedInfo);
        }

        if (queryStringParams.pId) {
            queryStringParams.pId = window.Ext.getDom(queryStringParams.pId).value;
        }

        var url = Ext.urlAppend(Ext.BasicOperationsServiceRestUrl + "List.svc/Rest/" + this.entityName, Ext.urlEncode(queryStringParams));
        window.Ext.Ajax.request({
            timeout: 1200000,
            url: url,
            scope: this,
            success: function (jsonResponse) { this.getDataFromServerSuccess(jsonResponse, queryStringParams.filterInput, silent); },
            failure: function (xhr) { this.getDataFromServerFailure(xhr, queryStringParams.filterInput); }
        });
    },
    getDataFromServerSuccess: function (jsonResponse, filter, silent)
    {
        this.searchBtn.dom.src = this.disabled || this.readOnly ? this.btnDis : this.btnOn;
        var result = undefined;
        try
        {
            result = window.Ext.decode(jsonResponse.responseText);
        }
        catch (err)
        {

        }

        if (!result.MainAttribute)
        {
            this.clearValue();
            throw new Error("Не указан основной аттрибут запрошенной сущности.");
        }


        if (!result || !result.Data || !result.Data.length)
        {
            this.linkItem.dom.innerHTML = filter || Ext.LocalizedResources.MatchesNotFound;
            if (!this.supressMatchesErrors) {
                this.setInvalid(Ext.LocalizedResources.MatchesNotFound, "CriticalError");
            }
        }
        else if (result.Data.length > 1)
        {
            this.linkItem.dom.innerHTML = filter || Ext.LocalizedResources.MultipleMatchesFound;
            if (!this.supressMatchesErrors) {
                this.setInvalid(Ext.LocalizedResources.MultipleMatchesFound, "Warning");
                this.prepareThumbPanel(result);
            }
        }
        else if (result.Data.length === 1)
        {
            var item = { id: result.Data[0].Id, name: result.Data[0][result.MainAttribute], data: result.Data[0] };
            this.setValue(item, silent);
        }
        this.fireEvent("afterquery", this);

    },
    getDataFromServerFailure: function (xhr, filter)
    {
        this.searchBtn.dom.src = this.disabled || this.readOnly ? this.btnDis : this.btnOn;
        this.linkItem.dom.innerHTML = filter;
        this.clearValue();
        this.setInvalid(xhr.responseText, "CriticalError");
        this.fireEvent("afterquery", this);
    },
    prepareFilterExpression: function (expression)
    {
        var result = "";
        if (expression)
        {
            var filters = expression.match(/\{\w{1,}\}/g);

            result = expression;
            if (filters)
            {
                for (var i = 0; i < filters.length; i++)
                {
                    var reg = new RegExp(filters[i], "g");
                    var val = this.getControlValue(filters[i].substring(1, filters[i].length - 1));
                    if (val)
                    {
                        result = result.replace(reg, val);
                    } else
                    {
                        result = result.replace(reg, "null");
                    }
                }
            }
        }
        return result;
    },
    getControlValue: function (id)
    {
        var cmp = window.Ext.getCmp(id);
        if (cmp)
        {
            var xtype = window.Ext.getCmp(id).getXType();
            if (xtype == "lookupfield")
            {
                return cmp.item ? cmp.item.id : null;
            }
        }
        if (Ext.getDom(id).value === undefined && !cmp)
        {
            throw new Error("Данный элемент не поддерживает свойство value.");
        }
        if (Ext.getDom(id).value !== undefined)
            return Ext.getDom(id).value;
        return cmp.getValue();
    },
    prepareThumbPanel: function (data)
    {
        if (this.thumbPanel)
        {
            this.thumbEl.fadeOut({ useDisplay: true, duration: 0.3 });
            this.thumbPanel.destroy();
            this.thumbPanel = null;
        }       

        var store = new window.Ext.data.Store({
            xtype: 'jsonstore',
            autoLoad: false,
            data: data,
            reader: new window.Ext.data.JsonReader({
                idProperty: 'Id',
                root: 'Data',
                totalProperty: 'RowCount',
                fields: this.tplFields || [{ name: "id", mapping: "Id" }, { name: "name", mapping: data.MainAttribute }]
            })
        });

        var headerText =this.tplHeaderTextTemplate || '<span class="x-lookup-thumb">{name}</span>&nbsp;';        
        var tpl = new window.Ext.XTemplate(
                    '<tpl for=".">', '<div class="x-lookup-thumb" id="{id}">', '<img alt="" src="' + this.entityIcon + '" class="x-lookup-item"/>', headerText, '</div>', '</tpl>', '<div class="x-clear"></div>');

        var dataView = new window.Ext.DataView({
            tpl: tpl,
            renderTo: this.thumbEl,
            store: store,
            singleSelect: true,
            overClass: 'x-lookup-thumb-over',
            itemSelector: 'div.x-lookup-thumb',
            style: 'overflow:auto; background-color: #FFFFFF;',
            listeners: { selectionchange: { fn: function (view)
            {
                var record = view.getRecord(view.getSelectedNodes()[0]);
                this.setValue({ id: record.data.id, name: record.data.name, data: record.json });
            }, scope: this
            }
            }
        });


        this.thumbPanel = new window.Ext.Panel({
            renderTo: this.thumbEl.dom,
            autoHeight: true,
            items: [new window.Ext.Panel({ height: 20, html: '<div class="x-lookup-thumb-head"><span class="x-lookup-thumb">' + Ext.LocalizedResources.MultipleMatchesFound + '</span><div>' }),
                            dataView,
                            new window.Ext.Panel({ height: 20, html: '<div class="x-lookup-thumb-bottom"><span style="text-align: center;" id="' + this.name + '_AdditionalLink" class="x-lookup-item">' + Ext.LocalizedResources.FindMoreRecords + '</span><div>' })]
        });
        window.Ext.get(this.name + '_AdditionalLink').on('click', this.openSearchWin, this);

        this.thumbPanel.on("beforedestroy", function () { window.Ext.get(this.name + '_AdditionalLink').removeAllListeners(); }, this);
    },
    openEntityCard: function ()
    {
        if (this.isValid)
        {
            var params = String.format("width={0},height={1},status=no,resizable=yes,top={2},left={3}", window.Ext.DoubleGis.Global.UISettings.ActualCardWidth, window.Ext.DoubleGis.Global.UISettings.ActualCardHeight, window.Ext.DoubleGis.Global.UISettings.ScreenCenterTop, window.Ext.DoubleGis.Global.UISettings.ScreenCenterLeft);
            var queryString = this.showReadOnlyCard ? '?ReadOnly=' + this.showReadOnlyCard : '';
            var sUrl = Ext.DoubleGis.Global.Helpers.EvaluateUpdateEntityUrl(this.entityName, this.item.id, queryString);
            window.open(sUrl, '_blank', params);
        }
        else
        {
            this.openSearchHint();
        }
    },
    openSearchHint: function() {
        if (this.disabled || this.readOnly) {
            return;
        }

        if (this.errorMessage === Ext.LocalizedResources.MultipleMatchesFound) {
            if (this.thumbEl.dom.style.display == "none") {
                this.thumbEl.fadeIn({ useDisplay: true, duration: 0.5 });
            } else {
                this.thumbEl.fadeOut({ useDisplay: true, duration: 0.5 });
            }
        }
    },
    openSearchWin: function (evt, el)
    {
        if (this.disabled || this.readOnly) {
             return;
        }

        if (this.fireEvent("beforequery", this) === false)
        {
            return;
        }

        var extraParameters = new Object();

        var filter = "";
        if (this.isValid)
        {
            filter = this.filter.dom.value;
        }
        else if (this.errorMessage === Ext.LocalizedResources.MultipleMatchesFound)
        {
            filter = this.linkItem.dom.innerHTML != Ext.LocalizedResources.MultipleMatchesFound ? this.linkItem.dom.innerHTML : '';
        }
        this.filter.dom.value = "";

        extraParameters.search = filter;

        if (this.parentEntityName !== "None" || this.parentIdPattern !== "") {

            // Вместо очистки флага 'filterToParent', которая тут была раньше, 
            // явно указываем null, если в лукапе ничего не выбрано (ERM-3576, ERM-3832)
            var parentId = window.Ext.getDom(this.parentIdPattern).value || "null";
            extraParameters.pType = this.parentEntityName;
            extraParameters.pId = parentId;
        } else if (window.Ext.getDom("ViewConfig_Id") && window.Ext.getDom("ViewConfig_EntityName")) {
            var pid = window.Ext.getDom("ViewConfig_Id").value;
            var ptype = window.Ext.getDom("ViewConfig_EntityName").value;
            if (pid && ptype) {
                extraParameters.pType = ptype;
                extraParameters.pId = pid;
            }
        }

        extraParameters.ReadOnly = this.showReadOnlyCard;
        
        if (this.extendedInfo)
        {
            var filterExpr = this.prepareFilterExpression(this.extendedInfo);
            extraParameters.extendedInfo = filterExpr;
        }
        if (this.defaultSortFields && this.defaultSortFieldsDirs) {
            extraParameters.defaultSortFields = this.defaultSortFields;
            extraParameters.defaultSortFieldsDirs = this.defaultSortFieldsDirs;           
        }                

        var url = this.searchUrl +  "?" + Ext.urlEncode(extraParameters);

        var result = window.showModalDialog(url, this.item ? { items: [this.item]} : null, 'status:no; resizable:yes; dialogWidth:900px; dialogHeight:500px; resizable: yes; scroll: no; location:yes;');
        this.filter.dom.style.display == "none" ? this.content.focus() : this.filter.focus();
        if (result)
        {
            if (result.items && result.items.length > 0)
            {
                this.setValue(result.items[0]);
            }
            else
            {
                this.clearValue();
            }
        }

        this.fireEvent("afterquery", this);
        this.fireEvent("afterselect", this);
    },
    imageItemError: function ()
    {
        this.entityIcon = window.Ext.DoubleGis.Global.Helpers.GetEntityIconPath("en_ico_16_Default.gif");
        this.imgItem.dom.src = this.entityIcon;
    },
    contentKeyUp: function (e)
    {
        if (this.disabled || this.readOnly)
            return;
        if (this.fireEvent("contentkeyup", this, e) === false)
        {
            return;
        }
        if (e.keyCode == e.DELETE)
        {
            this.clearValue();
            this.filter.focus();
        }
    },
    contentKeyPress: function (e)
    {
        if (this.disabled || this.readOnly)
            return;
        if (this.fireEvent("contentkeypress", this, e) === false)
        {
            return;
        }
        else if (e.keyCode == e.ENTER)
        {
            if (this.thumPanel)
            {
                if (this.thumbEl.dom.style.display == "none")
                    this.thumbEl.fadeIn({ useDisplay: true, duration: 0.5 });
            }
            else
            {
                this.openEntityCard();
            }
            e.stopPropagation();
            e.stopEvent();
            return;
        }
        this.filter.dom.style.display = "";
        this.content.dom.style.display = "none";
        this.filter.focus();
    },
    contentFocusIn: function ()
    {
        this.content.addClass("x-lookup-focus");
    },
    contentFocusOut: function ()
    {
        this.content.removeClass("x-lookup-focus"); if (this.thumbEl.dom.style.display != "none") this.thumbEl.fadeOut({ useDisplay: true, duration: 0.3 });
    },
    filterFocusOut: function ()
    {
        var res = this.filter.getValue();
        if (res)
        {
            this.clearValue();
            this.getDataFromServer({
                filterInput: res
            });
            this.filter.dom.value = "";
        }
        else
        {
            if (this.isValid === false || (this.item && this.item.id))
            {
                this.filter.dom.style.display = "none";
                this.content.dom.style.display = "";
            }
        }
    },
    forceGetData: function (config, silent) {
        this.clearValue(silent);
        this.getDataFromServer(config, silent);
        this.filter.dom.value = "";
    },

    setBtnOff: function (event) { if (event.target) { event.target.src = this.disabled || this.readOnly ? this.btnDis : this.btnOff; } },
    setBtnOn: function (event) { if (event.target) { event.target.src = this.disabled || this.readOnly ? this.btnDis : this.btnOn; } },
    clearValue: function (silent)
    {
        this.setValue(undefined, silent);
        return this.item;

    },
    getValue: function ()
    {
        this.deserialize();
        return this.item;
    },
    getRawValue: function ()
    {
        return this.el.dom.value || "{}";
    },
    setValue: function (item, silent)
    {
        silent = silent || false;
        if (!silent)
        {
            if (this.fireEvent("beforechange", this, item) === false)
            {
                return;
            }
        }
        if (this.thumbPanel)
        {
            this.thumbEl.fadeOut({ useDisplay: true, duration: 0.3 });
            this.thumbPanel.destroy();
            this.thumbPanel = null;
        }
        if (item && item.id)
        {
            Ext.getDom(this.name + "Id").value = item.id;
            Ext.getDom(this.name + "Name").value = item.name;
            this.item = item;
            this.renderItems();
        }
        else
        {
            Ext.getDom(this.name + "Id").value = "";
            Ext.getDom(this.name + "Name").value = "";
            this.item = undefined;
            this.renderItems();
        }
        if (!silent)
        {
            this.fireEvent("change", this, item);
        }
    },
    setValid: function ()
    {
        this.linkItem.dom.style.fontStyle = "";
        this.isValid = true;
        this.errorMessage = "";
        this.linkItem.removeClass("x-lookup-item-invalid");
        this.imgItem.dom.src = this.entityIcon;
        if (this.item)
        {
            this.item.name = this.item.name || this.emptyText;
            this.linkItem.dom.style.fontStyle = this.item.name == this.emptyText ? "italic" : "";
            this.linkItem.dom.innerHTML = this.item.name;
            this.linkItem.dom.title = this.item.name;
            this.imgItem.dom.title = this.item.name;
            this.filter.dom.style.display = "none";
            this.content.dom.style.display = "";
        }
        else
        {
            this.linkItem.dom.title = "";
            this.imgItem.dom.title = "";
            this.filter.dom.style.display = "";
            this.content.dom.style.display = "none";
        }

    },
    setInvalid: function (msg, severity)
    {
        this.linkItem.dom.style.fontStyle = "";
        this.isValid = false;
        this.errorMessage = msg;
        this.linkItem.addClass("x-lookup-item-invalid");
        this.imgItem.dom.src = severity ? window.Ext.Notification.Icon[severity] : window.Ext.Notification.Icon.CriticalError;
        this.linkItem.dom.title = msg;
        this.imgItem.dom.title = msg;
        this.filter.dom.style.display = "none";
        this.content.dom.style.display = "";

    },
    setReadOnly: function (readOnly)
    {
        //сделать нормальный readonly когда грохнем старые лукапы
        if (readOnly === true)
        {
            this.readOnly = true;
            this.searchBtn.dom.src = this.btnDis;
            this.filter.dom.readOnly = true;
            this.filter.addClass("ReadOnly");
            this.content.addClass(["ReadOnly", "x-lookup-readonly"]);
        }
        else
        {
            this.readOnly = false;
            this.searchBtn.dom.src = this.btnOff;
            this.filter.dom.readOnly = false;
            if (!this.disabled)
            {
                this.filter.removeClass("ReadOnly");
                this.content.removeClass(["ReadOnly", "x-lookup-readonly"]);
            }
        }
    },
    setDisabled: function (disabled)
    {
        if (disabled === true)
        {
            this.disable();
        }
        else
        {
            this.enable();
        }
    },
    disable: function ()
    {
        this.disabled = true;
        this.searchBtn.dom.src = this.btnDis;
        this.filter.dom.disabled = "disabled";
        this.filter.addClass("ReadOnly");
        this.content.addClass(["ReadOnly", "x-lookup-readonly"]);
        this.content.dom.tabIndex = -1;
        this.el.dom.disabled = "disabled";
    },
    enable: function ()
    {
        this.disabled = false;
        this.searchBtn.dom.src = this.btnOff;
        this.filter.dom.disabled = false;
        this.content.dom.tabIndex = this.tabIndex;
        this.el.dom.disabled = false;
        if (!this.readOnly)
        {
            this.filter.removeClass("ReadOnly");
            this.content.removeClass(["ReadOnly", "x-lookup-readonly"]);
        }

    }
});
Ext.reg('lookupfield', Ext.ux.LookupField);
