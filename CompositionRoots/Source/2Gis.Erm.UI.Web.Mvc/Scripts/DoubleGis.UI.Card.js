///<reference path="DoubleGis.GlobalVariables.js"/>
Ext.ns("Ext.DoubleGis.UI");
Ext.DoubleGis.UI.Card = Ext.extend(Ext.util.Observable, {
    renderHeader: true,
    submitMode: 0,
    submitModes:
        {
            NONE: 0,
            SAVE: 1,
            SAVE_AND_CLOSE: 2,
            SAVE_AND_OPEN: 3
        },
    Items: {},
    constructor: function (config) {
        this.addEvents("beforebuild", "afterbuild", 'beforepost', 'afterpost', 'postformsuccess', 'postformfailure', 'formbind', "beforeclose", "afterrelatedlistready"/*, "beforerefresh", "afterrefresh"*/);
        this.init(config);
    },
    init: function (settings) {
        this.form = window.EntityForm;
        this.EntityName = settings.EntityName;
        this.Settings = settings;
        this.ReadOnly = window.Ext.getDom("ViewConfig_ReadOnly").checked;
        this.validator = Ext.DoubleGis.FormValidator = new Ext.DoubleGis.MvcFormValidator(
            {
                listeners:
                {
                    attach: this.onValidatorAttach,
                    detach: this.onValidatorDetach,
                    scope: this
                }
            });
        this.validator.init();

    },
    setVisualFeatures: function () {
        var divRows = window.Ext.query("div.field-wrapper");
        var i;
        for (i = 0; i < divRows.length; i++) {
            window.Ext.fly(divRows[i]).addClassOnOver("field-wrapper-over");
        }

        var inputs = window.Ext.query(".inputfields");
        for (i = 0; i < inputs.length; i++) {
            window.Ext.fly(inputs[i]).addClassOnFocus("inputfields-selected");
        }
    },
    Build: function () {
        if (window.InitPage) {
            window.InitPage.createDelegate(this)();
        }

        if (this.fireEvent("beforebuild", this) === false) {
            return;
        }

        window.Ext.each(window.Ext.CardLookupSettings, function (item) {
            new window.Ext.ux.LookupField(item);
        }, this);

        var depList = window.Ext.getDom("ViewConfig_DependencyList");
        if (depList.value) {
            this.DependencyHandler = new window.Ext.DoubleGis.DependencyHandler();
            this.DependencyHandler.register(window.Ext.decode(depList.value), this.form);
        }
        depList.value = null;


        window.Ext.each(this.form, function (el) {
            if (el.id) {
                if (this.ReadOnly && (el.tagName != "FIELDSET" && el.tagName != "FORM")) {
                    (el.tagName == "SELECT" || el.type == "checkbox" || el.type == "radio")
                        ? window.Ext.fly(el).disable().on("change", this.onFieldChange, this)
                        : window.Ext.fly(el).setReadOnly(true).on("change", this.onFieldChange, this);
                }
                else {
                    window.Ext.fly(el).on("change", this.onFieldChange, this);
                }
            }
        }, this);
        window.Ext.each(window.Ext.query("input.x-calendar"), function (node) {
            var d = window.Ext.getCmp(node.id);
            d.on("select", this.onFieldChange, this);
            if (this.ReadOnly || d.readOnly) {
                d.setReadOnly(true);
            }
        }, this);
        window.Ext.each(window.Ext.query("table.x-calendar-v2"), function (node) {
            var d = window.Ext.getCmp(node.id.replace('_wrapper', ''));
            if (this.ReadOnly || d.readOnly) {
                d.setReadOnly(true);
            }
        }, this);
        window.Ext.each(window.Ext.CardLookupSettings, function (item) {
            var d = Ext.getCmp(item.id);
            if (d) {
                d.on("change", this.onFieldChange, this);
                if (this.ReadOnly) {
                    d.setReadOnly(true);
                }
            }
        }, this);

        window.Ext.getBody().on("keypress", function (e) {
            if (e.keyCode == e.ESC) {
                this.Close();
            }
            else if (e.keyCode == e.ENTER) {
                if (e.target && e.target.form && e.target.form.length == 1) {
                    e.preventDefault();
                    this.Save();
                }
            }
        }, this);
        window.Ext.EventManager.on(window, "beforeunload", this.commitClose, this);

        // Поддержка maxlength для textarea
        var txts = document.getElementsByTagName('TEXTAREA');

        for (var i = 0, l = txts.length; i < l; i++) {
            if (/^[0-9]+$/.test(txts[i].getAttribute("maxlength"))) {
                var func = function () {
                    var len = parseInt(this.getAttribute("maxlength"), 10);
                    if (this.value.length > len) {
                        this.value = this.value.substr(0, len);
                        return false;
                    }

                    return true;
                };

                txts[i].onkeyup = func;
                txts[i].onblur = func;
            }
        }

        this.BuildContentPage();
        this.setVisualFeatures();
        this.fireEvent("afterbuild", this);
    },
    //Построение самой страницы
    BuildContentPage: function () {

        var mainAttributeValue = null;
        {
            var maDom = Ext.getDom(this.Settings.EntityMainAttribute);
            if (maDom) {
                if (maDom.type == "select-one") {
                    // Combobox.
                    mainAttributeValue = "";
                    for (var i = 0; i < maDom.options.length; i++) {
                        if (maDom.options[i].selected) {
                            mainAttributeValue = maDom.options[i].innerHTML;
                        }
                    }
                } else if (maDom.value || maDom.innerHTML) {
                    mainAttributeValue = maDom.value || maDom.innerHTML;
                }
            }
        }

        // после отказа от EntitySettings.xml необходимо убрать приседания с EntityMainAttribute на клиенте.
        // На сервере будет подготовлен необходимый this.Settings.Title
        document.title = this.Settings.Title ? this.Settings.Title
            : this.Settings.EntityLocalizedName + ": " + (mainAttributeValue || window.Ext.LocalizedResources.Create);

        var s = new Ext.Panel(
                {
                    applyTo: this.form,
                    plugins: [new window.Ext.ux.FitToParent(document.body)],
                    footer: true,
                    footerCfg:
                        {
                            tagName: 'div',
                            cls: 'status-bar',
                            html: window.Ext.LocalizedResources.StatusLabel + Ext.getDom("EntityStatus").value
                        },
                    tbar: this.Settings.CardToolbar.length ? window.Ext.DoubleGis.Global.Helpers.ToolbarHelper.BuildToolbar(this.Settings.CardToolbar, true, this) : undefined,
                    layout: 'fit',
                    items: [
                        new Ext.Panel({
                            layout: 'border',
                            headerCfg: this.renderHeader ? {
                                tagName: 'div', cls: 'title-bar',
                                html: '<table><tbody><tr><td class="title-icon">' +
                                    '<img alt="" src="' + Ext.DoubleGis.Global.Helpers.GetEntityIconPath(this.Settings.Icon) + '"/>' +
                                    '</td><td class="title-bar"><span class="title-bar">' + document.title + '</span><br /><span class="title-breadcrumb">' +
                                    '<span class="title-breadcrumb" id="leftNavBreadcrumbText">' + Ext.LocalizedResources.Information + '</span></span></td></tr></tbody></table>'
                            } : undefined,
                            items: [
                                    this.Items.RelatedItems = new window.Ext.tree.TreePanel(
                                        {
                                            bodyCssClass: 'navigation-bar',
                                            region: 'west',
                                            width: 180,
                                            hidden: this.Settings.CardRelatedItems.length ? false : true,
                                            rootVisible: false,
                                            id: "CardRelatedItemsTree",
                                            lines: false,
                                            enableDD: false,
                                            border: false,
                                            root: new window.Ext.tree.AsyncTreeNode({ children: window.Ext.DoubleGis.Global.Helpers.NavBarHelper.BuildTree({ Items: this.Settings.CardRelatedItems }, 23) }),
                                            listeners: {
                                                click: { fn: this.changeFrame, scope: this }
                                            }
                                        }),
                                    new Ext.Panel({
                                        region: 'center',
                                        layout: 'anchor',
                                        id: "CardContentPanel",
                                        items: [
                                            new Ext.Panel({
                                                id: "ContentTab_holder",
                                                layout: 'anchor',
                                                anchor: '100%, 100%',
                                                headerCfg: { tagName: 'div', cls: 'Notifications', id: 'Notifications', style: 'display:none;' },
                                                header: false,
                                                items: [this.Items.TabPanel = new window.Ext.TabPanel({
                                                    id: "TabWrapper",
                                                    anchor: '100%, 100%',
                                                    applyTo: "MainTab_holder",
                                                    border: false,
                                                    margins: "0 0 0 0",
                                                    deferredRender: false,
                                                    activeTab: 0,
                                                    autoTabs: true,
                                                    viewConfig: { forceFit: true },
                                                    autoTabSelector: 'div.Tab'
                                                })]
                                            })
                                        ]
                                    })
                            ]
                        })]
                });
        this.Items.Toolbar = s.getTopToolbar();
        this.Mask = new window.Ext.LoadMask(window.Ext.get("CardContentPanel"));
        if (this.Settings.HasComments === true && window.Ext.getDom("ViewConfig_Id").value && window.Ext.getDom("ViewConfig_Id").value != "0") {
            this.Items.TabPanel.add({ xtype: "notepanel", pCardInfo: { pTypeName: this.Settings.EntityName, pId: window.Ext.getDom("ViewConfig_Id").value } });
        }

        if (this.Settings.HasActionsHistory === true && window.Ext.getDom("ViewConfig_Id").value && window.Ext.getDom("ViewConfig_Id").value != "0") {
            this.Items.TabPanel.add({ xtype: "actionshistorytab", pCardInfo: { pTypeName: this.Settings.EntityName, pId: window.Ext.getDom("ViewConfig_Id").value } });
        }

        if (window.Ext.getDom("Message").innerHTML.trim()) {
            this.isDirty = window.Ext.getDom("MessageType").innerHTML.trim() == "CriticalError";
            this.AddNotification(window.Ext.getDom("Message").innerText.trim(), window.Ext.getDom("MessageType").innerHTML.trim(), "ServerError");
        }
    },

    //Операции с карточкой
    AddNotification: function (message, level, messageId) {

        // FIXME {f.zaharov, 21.08.2014}: Copypaste from \Scripts\Ext.DoubleGis.Order.CheckResultWindow.js processLinks = function(text)
        // Но проблема глубже, там используется иной подход к формированию сообщений, наверно нужен общий
        // COMMENT {all, 10.10.2014}: Больше не копипаст, но это не значит, что тут или там стало хорошо. Просто пофиксил баг.

        var expr = /<(.+?):(.+?):(\d+)>/;
        var match = expr.exec(message);
        while (match) {
            var link = String.format('<a href="{0}">{1}</a>', Ext.DoubleGis.Global.Helpers.EvaluateUpdateEntityUrl(match[1], match[3], ''), match[2]);
            message = message.replace(expr, link);
            match = expr.exec(message);
        }

        if (level == "None")
            return;

        // автоматически назначаем messgeId если не передали его явно
        if (!messageId) {
            switch (level) {
                case "Info":
                    messageId = "ServerInfo";
                    break;

                case "Warning":
                    messageId = "ServerWarning";
                    break;

                case "CriticalError":
                    messageId = "ServerError";
                    break;
            }
        }

        var nopt = { message: message, level: window.Ext.Notification.Icon[level], messageId: messageId };
        var nc = Ext.getCmp('ContentTab_holder');
        if (!this.NotificationTemplate) {
            this.NotificationTemplate = new Ext.XTemplate(
            '<div id="{messageId}" class="Notification">',
            '<table cellspacing="0" cellpadding="0"><tbody><tr><td valign="top">' +
            '<img class="ms-crm-Lookup-Item" alt="" src="{level}"/>',
            '</td><td width="5px"></td><td><span id="NotificationText">{message}</span>',
            '</td></tr></tbody></table></div>');
        }
        nc.header.show(true).dom.innerHTML = "";
        this.NotificationTemplate.append(nc.header, nopt);
    },
    RemoveNotification: function (messageId) {
        var header = Ext.getCmp('ContentTab_holder').header;
        var msg = header.child('#' + messageId);
        if (msg)
            msg.remove();

        if (header.isVisible() && !header.child('.Notification')) {
            header.hide();
        }
    },
    Save: function () {
        this.Items.Toolbar.disable();
        this.submitMode = this.submitModes.SAVE;
        if (this.fireEvent('beforepost', this) === false) {
            this.Items.Toolbar.enable();
            this.recalcToolbarButtonsAvailability();
            return;
        }
        if (this.normalizeForm() !== false) {
            this.postForm();
        }
        else {
            this.Items.Toolbar.enable();
            this.recalcToolbarButtonsAvailability();
        }
    },
    SaveAndClose: function () {
        this.Items.Toolbar.disable();
        this.submitMode = this.submitModes.SAVE_AND_CLOSE;
        if (this.fireEvent('beforepost', this) === false) {
            this.Items.Toolbar.enable();
            this.recalcToolbarButtonsAvailability();
            return;
        }
        if (this.normalizeForm() !== false) {
            this.postForm();
        }
        else {
            this.Items.Toolbar.enable();
            this.recalcToolbarButtonsAvailability();
        }
    },
    SaveAndOpen: function () {
        this.Items.Toolbar.disable();
        this.submitMode = this.submitModes.SAVE_AND_OPEN;
        if (this.fireEvent('beforepost', this) === false) {
            this.Items.Toolbar.enable();
            this.recalcToolbarButtonsAvailability();
            return;
        }
        if (this.normalizeForm() !== false) {
            this.postForm();
        }
        else {
            this.Items.Toolbar.enable();
            this.recalcToolbarButtonsAvailability();
        }
    },
    Close: function () {
        if (this.fireEvent("beforeclose", this) === false) {
            return;
        }

        if (Ext.isChrome) {
            window.open('', '_self', '');
        }

        window.close();
    },
    Print: function (methodName) {
        var entityId = Ext.getDom('Id').value;
        var url = '/' + this.EntityName + '/' + methodName + '/' + entityId + '?__dc=' + Ext.util.Format.cacheBuster();
        this.Items.Toolbar.disable();

        var iframe;
        iframe = document.getElementById("hiddenDownloader");
        if (iframe === null) {
            iframe = document.createElement('iframe');
            iframe.id = "hiddenDownloader";
            iframe.style.visibility = 'hidden';

            var iframeEl = new Ext.Element(iframe);
            iframeEl.on("load", function () {
                var iframeContent = iframe.contentWindow.document.documentElement.innerText;
                if (iframeContent != "") {
                    alert(iframeContent);
                }
            });
            document.body.appendChild(iframe);
        }

        iframe.src = url;
        this.Items.Toolbar.enable();
        this.recalcToolbarButtonsAvailability();
    },

    postForm: function () {
        this.clearMessages();
        this.Mask.show();
        Ext.Ajax.request(
        {
            url: this.form.action,
            method: 'POST',
            form: this.form,
            success: this.postFormSuccess,
            failure: this.postFormFailure,
            scope: this,
            // 5 minutes timeout
            timeout: 300000
        });
    },

    clearMessages: function () {
        this.RemoveNotification("ServerWarning");
        this.RemoveNotification("ServerError");
        this.RemoveNotification("ServerInfo");
        this.validator.clearMessages();
    },

    postFormSuccess: function (response, opts) {
        if (this.fireEvent('afterpost', this) === false) {
            return;
        }

        var frm = Ext.decode(response.responseText);
        if (this.fireEvent('postformsuccess', this, frm) === false) {
            return;
        }

        switch (frm.MessageType) {
            case "None":
            case "Warning":
            case "Info":
                {
                    this.isDirty = false;
                    this.refreshParentGrid();

                    switch (this.submitMode) {
                        case this.submitModes.SAVE:
                            {
                                if (this.form.IsNew.value.toLowerCase() === 'true' && frm.Id != 0) {
                                    window.location.pathname = window.location.pathname + '/' + frm.Id;
                                    return;
                                }
                            }
                            break;

                        case this.submitModes.SAVE_AND_CLOSE:
                            {
                                this.Close();
                            }
                            return;

                        case this.submitModes.SAVE_AND_OPEN:
                            {
                                this.createNewEntity();
                                this.Close();
                            }
                            return;

                        default:
                    }
                }
                break;
        }

        this.AddNotification(frm.Message, frm.MessageType);

        // В ходе привязки формы она становится dirty, поэтому нужен ещё один switch
        this.rebindForm(frm);

        switch (frm.MessageType) {
            case "None":
            case "Warning":
            case "Info":
                {
                    this.isDirty = false;
                }
                break;
        }

        this.Mask.hide();
        this.Items.Toolbar.enable();
        this.recalcToolbarButtonsAvailability();
        this.fireEvent('formbind', this, frm);
    },
    updateValidationMessages: function (form) {
        //Биндим все серверные сообщения
        if (!window.Ext.isEmpty(form.ValidationMessages) && form.ValidationMessages.length) {
            for (var i = 0; i < form.ValidationMessages.length; i++) {
                var msgItem = form.ValidationMessages[i];
                if (Ext.fly(msgItem.For + '_validationMessage')) {
                    this.validator.updateValidationMessage({ FieldName: msgItem.For, ValidationMessageId: msgItem.For + '_validationMessage' }, msgItem.Message);
                }
            }
            this.focusFirstInvalidField();
        }
        delete form.ValidationMessages;
    },
    updateViewConfigInfo: function (form) {
        if (!window.Ext.isEmpty(form.ViewConfig))
            if (!window.Ext.isEmpty(form.ViewConfig)) {
                for (var viewProp in form.ViewConfig) {
                    var value = form.ViewConfig[viewProp];
                    var elem = Ext.getDom("ViewConfig_" + viewProp);
                    if (elem)
                        elem.value = value || '';
                    if (viewProp == "ReadOnly") {
                        elem.checked = value;
                        if (value === true && this.ReadOnly === false) {
                            this.ReadOnly = true;
                            window.Ext.each(this.form, function (el) {
                                if (el.id) {
                                    if (el.tagName != "FIELDSET" && el.type != "button" && el.tagName != "FORM") {
                                        (el.tagName == "SELECT" || el.type == "checkbox" || el.type == "radio") ? window.Ext.fly(el).disable() : window.Ext.fly(el).setReadOnly(true);
                                    }
                                }
                            }, this);
                            window.Ext.each(window.Ext.query("input.x-calendar"), function (node) {
                                window.Ext.getCmp(node.id).setReadOnly(true);
                            }, this);
                            window.Ext.each(window.Ext.query("table.x-calendar-v2"), function (node) {
                                window.Ext.getCmp(node.id.replace('_wrapper', '')).setReadOnly(true);
                            }, this);
                            window.Ext.each(window.Ext.CardLookupSettings, function (item) {
                                Ext.getCmp(item.id).setReadOnly(true);
                            }, this);
                        }
                    }
                }
            }
        this.Settings = form.ViewConfig.CardSettings;
        delete form.ViewConfig;
    },
    recalcDisabling: function () {
        for (var nodeId in this.Items.RelatedItems.nodeHash) {
            var node = this.Items.RelatedItems.nodeHash[nodeId];
            node.attributes.disabled ? node.disable() : node.enable();
        }
        this.recalcToolbarButtonsAvailability();
    },
    recalcToolbarButtonsAvailability: function () {
        Ext.each(this.Settings.CardToolbar, function (item) {
            var cmp = Ext.getCmp(item.Name);
            if (cmp) {
                item.Disabled ? cmp.disable() : cmp.enable();
            }
        }, this);
    },
    rebindForm: function (form) {
        this.updateValidationMessages(form);
        this.updateViewConfigInfo(form);
        //Биндим все поля формы
        for (var fieldName in form) {

            var field = Ext.get(fieldName);
            if (!field)
                continue;
            var val;
            switch (field.dom.type) {
                case 'text':
                    if (field.hasClass('x-calendar')) {
                        Ext.getCmp(field.id).setValue(Ext.isEmpty(form[fieldName]) ? '' : form[fieldName]);
                    }
                    else if (field.hasClass('x-lookup')) {
                        Ext.getCmp(field.id).setValue(form[fieldName] ? { id: form[fieldName].Key, name: form[fieldName].Value } : '', true);
                    }
                    else {
                        if (Ext.isNumber(form[fieldName])) {
                            field.dom.value = Number.formatToLocal(form[fieldName]);
                        }
                        else {
                            val = form[fieldName] === true ? "True" : (form[fieldName] === false ? "False" : form[fieldName]);
                            field.dom.value = Ext.isEmpty(val) ? '' : val;
                        }

                    }
                    break;
                case 'checkbox':
                    field.dom.checked = form[fieldName];
                    break;
                case 'radio':
                    val = form[fieldName] === true ? "True" : (form[fieldName] === false ? "False" : form[fieldName]);
                    Ext.query("input[name=" + field.dom.name + "][value=" + val + "]")[0].checked = true;
                    break;
                case 'select-one':
                    if (field.dom.options[0].value == "") {
                        field.dom.value = "";
                        for (var i = 0; i < field.dom.options.length; i++) {
                            if (field.dom.options[i].value == form[fieldName]) {
                                field.dom.value = Ext.isEmpty(form[fieldName]) ? '' : form[fieldName];
                                break;
                            }
                        }
                    }
                    else {
                        field.dom.value = form[fieldName] || '';
                    }
                    break;
                case 'hidden':

                    if (Ext.isNumber(form[fieldName])) {
                        field.dom.value = Number.formatToLocal(form[fieldName]);
                    }
                    else {
                        val = form[fieldName] === true ? "True" : (form[fieldName] === false ? "False" : form[fieldName]);
                        field.dom.value = Ext.isEmpty(val) ? '' : val;
                    }
                    break;
            }
        }
        //Пересчитываем зависимости
        if (this.DependencyHandler) {
            var deps = this.DependencyHandler.dependencyList;
            this.DependencyHandler.unregister();
            this.DependencyHandler.register(deps, this.form);
        }
        this.recalcDisabling();
    },
    postFormFailure: function (xhr) {
        if (this.fireEvent('afterpost', this) === false) {
            return;
        }
        if (this.fireEvent('postformfailure', this, xhr) === false) {
            return;
        }

        if (xhr.responseText) {
            try {
                var frm = Ext.decode(xhr.responseText);
                this.AddNotification(frm.Message, "CriticalError", "ServerError");
            }
            catch (e) {
                alert(xhr.responseText);
            }
        } else {

            var errorText = xhr.statusText;

            if (errorText === 'transaction aborted') {
                errorText = 'Время ожидания операции истекло, обновите карточку и выполните операцию ещё раз';
            }

            this.AddNotification(errorText, "CriticalError", "ServerError");
        }
        this.Mask.hide();
        this.Items.Toolbar.enable();
        this.recalcToolbarButtonsAvailability();
    },
    normalizeForm: function () {
        this.clearMessages();
        if (this.Settings.CardRelatedItems.length && this.Settings.CardRelatedItems.length > 0) {
            window.Ext.getCmp("CardRelatedItemsTree").root.childNodes[0].firstChild.select();
            this.changeFrame(window.Ext.getCmp("CardRelatedItemsTree").root.childNodes[0].firstChild);
        }
        var isValid = this.validator.validate(this.form);
        if (!isValid) {
            this.focusFirstInvalidField();
            return false;
        }
        window.Ext.each(window.Ext.query("input.x-calendar", this.form), function (node) {
            var cmp = window.Ext.getCmp(node.id);
            node.value = cmp.getValue() ? cmp.getValue().format(Ext.CultureInfo.DateTimeFormatInfo.PhpInvariantDateTimePattern) : "";
            this.on('afterpost', this.setValidDate, cmp, { single: true });
        }, this);

        //	"Включение" select элементов сделано для того, чтобы поля заполненные значениями по умолчанию и недоступные для редактирования "приезжали" на сервер. Решение временное.
        window.Ext.each(window.Ext.query("select", this.form), this.unlockCheckBoxBeforeSubmit, this);
        window.Ext.each(window.Ext.query("input[type=checkbox]", this.form), this.unlockCheckBoxBeforeSubmit, this);
        window.Ext.each(window.Ext.query("input[type=radio]", this.form), this.unlockCheckBoxBeforeSubmit, this);

        if (window.Ext.query("input[type=file]:not(.x-async-file-input) ", this.form).length > 0)
            this.form.encoding = "multipart/form-data";
        return true;
    },
    setValidDate: function () {
        this.setValue(this.getValue()); //.format(Ext.CultureInfo.DateTimeFormatInfo.PhpFullDateTimePattern));
    },
    unlockCheckBoxBeforeSubmit: function (node) {
        if (node.disabled) {
            node.disabled = false;
            this.on('afterpost', this.lockCheckBoxAfterSubmit, node, { single: true });
        }
    },
    lockCheckBoxAfterSubmit: function (node) {
        this.disabled = true;
    },
    refresh: function (deepRefresh) {
        if (deepRefresh === true) {
            this.refreshParentGrid();
        }
        window.location.reload();
    },
    refreshParentGrid: function () {
        try {
            if (window.opener && window.opener.Entity) {
                window.opener.Entity.refresh();

            }
            else if (window.opener && window.opener.crmGrid) {
                window.opener.crmGrid.Refresh();
            }

        }
        catch (err) { }
    },
    createNewEntity: function () {
        try {
            if (window.opener && window.opener.Entity) {
                window.opener.Entity.Create();
            }
            else if (window.opener && window.opener.crmGrid) {
                window.opener.openObj(window.opener.gridBodyTable.getAttribute("oname") * 1);
            }
        }
        catch (err) { }
    },
    focusFirstInvalidField: function () {
        if (this.Items.TabPanel) {
            window.Ext.each(this.Items.TabPanel.items.items,
                function (item) {
                    if (window.Ext.query("span.field-validation-error", item.el.dom).length) {
                        this.Items.TabPanel.activate(item);
                        var elFocus = window.Ext.get(item.el.query("input.input-validation-error")[0]);
                        if (elFocus) {
                            elFocus.focus();
                        }
                        return false;
                    }
                    return true;
                }, this);
        }
    },
    //#region event handlers
    onFieldChange: function () {
        this.isDirty = true;
    },
    commitClose: function () {
        if (this.isDirty) {
            if (window.event) {
                event.returnValue = window.Ext.LocalizedResources.CloseCardConfirmation;
            }
        }
    },
    fireAfterRelatedListReady: function (dataList) {
        this.fireEvent("afterrelatedlistready", this, { dataList: dataList });
    },
    onValidatorAttach: function (formOptions, field, rule) {
        var fieldId = field.FieldName.replace(".", "_");
        var el = window.Ext.getDom(fieldId);
        if (rule.ValidationType == "required") {
            el = window.Ext.select('label[for=' + fieldId + ']', true, formOptions.form.id);
            if (el && el.elements && el.elements[0]) {
                window.Ext.DomHelper.insertHtml('afterEnd', el.elements[0].dom, '<span class="req" id="' + fieldId + "-req" + '">*</span>');
            }
        }
        if (rule.ValidationType == "stringlength") {
            if (el && rule.ValidationParameters.maximumLength) {
                el.maxLength = rule.ValidationParameters.maximumLength;
            }
        }

        if (rule.ValidationType == "email") {
            new Ext.ux.LinkField(
                {
                    applyTo: el,
                    readOnly: this.ReadOnly,
                    contactTypeCfg:
                        {
                            linkCls: Ext.ux.LinkField.prototype.contactTypeRegistry.email.linkCls,
                            protocolPrefix: Ext.ux.LinkField.prototype.contactTypeRegistry.email.protocolPrefix,
                            protocolRegex: Ext.ux.LinkField.prototype.contactTypeRegistry.email.protocolRegex,
                            validator: field.validators.email,
                            validationMessage: rule.ErrorMessage
                        },
                    listeners: {
                        invalid: function (el, msg) {
                            this.validator.updateValidationMessage(field, msg);
                        },
                        valid: function (el) {
                            this.validator.updateValidationMessage(field, '');
                        },
                        change: this.onFieldChange,
                        scope: this
                    }
                });

        }
        if (rule.ValidationType == "url") {
            new Ext.ux.LinkField(
                {
                    applyTo: el,
                    readOnly: this.ReadOnly,
                    contactTypeCfg:
                        {
                            linkCls: Ext.ux.LinkField.prototype.contactTypeRegistry.url.linkCls,
                            protocolPrefix: Ext.ux.LinkField.prototype.contactTypeRegistry.url.protocolPrefix,
                            protocolRegex: Ext.ux.LinkField.prototype.contactTypeRegistry.url.protocolRegex,
                            validator: field.validators.url,
                            validationMessage: rule.ErrorMessage
                        },
                    listeners: {
                        invalid: function (el, msg) {
                            this.validator.updateValidationMessage(field, msg);
                        },
                        valid: function (el) {
                            this.validator.updateValidationMessage(field);
                        },
                        change: this.onFieldChange,
                        scope: this
                    }
                });
        }
    },
    onValidatorDetach: function (formOptions, field, ruleName) {
        var fieldId = field.FieldName.replace(".", "_");
        var el;
        if (ruleName == "required") {
            el = window.Ext.getDom(fieldId + "-req");
            if (el) {
                Ext.removeNode(el);
            }
        }
    },
    changeFrame: function (n) {
        if (!n.leaf) {
            n.expanded ? n.collapse() : n.expand();
            return;
        }
        if (n.id == "ContentTab") {
            this.Mask.hide();
        }
        else {
            this.changeFrame(window.Ext.getCmp("CardRelatedItemsTree").root.childNodes[0].firstChild);
        }
        var cnt = window.Ext.getCmp('CardContentPanel');

        if (!window.Ext.getDom(n.id + "_holder")) {
            this.Mask.show();

            var reg;
            var filters;
            var val;
            var i;                 

            var extendedInfo = n.attributes.extendedInfo;
            if (extendedInfo) {
                filters = extendedInfo.match(/\{\w{1,}\}/g);
                if (filters) {
                    for (i = 0; i < filters.length; i++) {
                        reg = new RegExp(filters[i], "g");
                        val = window.Ext.get(filters[i].substring(1, filters[i].length - 1)).getValue();
                        extendedInfo = extendedInfo.replace(reg, val);
                    }
                }
            }

            var requestUrl = n.attributes.requestUrl;
            if (n.attributes.requestUrl) {
                filters = n.attributes.requestUrl.match(/\{\w{1,}\}/g);
                if (filters) {
                    requestUrl = n.attributes.requestUrl;
                    for (i = 0; i < filters.length; i++) {
                        reg = new RegExp(filters[i], "g");
                        val = encodeURIComponent(window.Ext.get(filters[i].substring(1, filters[i].length - 1)).getValue());
                        requestUrl = requestUrl.replace(reg, val);
                    }
                }
            }

            cnt.add(new window.Ext.Panel({
                id: n.id + '_holder',
                anchor: '100% 100%',
                bodyCssClass: 'Holder',
                html: '<iframe id="' + n.id + '_frame"></iframe>'
            }
            ));
            cnt.doLayout();
            var frame = window.Ext.getDom(n.id + '_frame');
            window.Ext.get(frame).on("load", function (evt, el) {
                el.height = window.Ext.get(el.parentElement).getComputedHeight();
                el.width = window.Ext.get(el.parentElement).getComputedWidth();
                el.style.height = "100%";
                el.style.width = "100%";
                this.Mask.hide();
            }, this);

                var parentEntityTypeName = this.EntityName ? this.EntityName : null;
                var parentEntityId = window.Ext.get('Id') ? window.Ext.get('Id').dom.value : null;
                var parentEntityState = window.Ext.getDom("ViewConfig_ReadOnly").checked ? 'Inactive' : 'Active';

                var frameUrl;

                // Determine that related view is grid or not
                if (!requestUrl || requestUrl.indexOf("Grid") < 0) {
                    // Related view is not grid
                    // Construct generic URL: <server>/{controller}/{action}/{entityTypeName}/{entityId}/{entityState}
                    frameUrl = String.format("{0}/{1}/{2}/{3}",
                        requestUrl,
                        parentEntityTypeName,
                        parentEntityId,
                        parentEntityState);
                }
                else {
                    // Related view is grid
                    // Construct generic URL: <server>/Grid/{action}/{entityTypeName}/{parentEntityType}/{parentEntityId}/{parentEntityState}/{appendedEntityType}
                    var appendedEntityType = n.attributes.appendableEntity ? n.attributes.appendableEntity : null;
                    frameUrl = String.format("{0}/{1}/{2}/{3}/{4}",
                        requestUrl,
                        parentEntityTypeName,
                        parentEntityId,
                        parentEntityState,
                        appendedEntityType);
                }

                if (extendedInfo) {
                    frameUrl = window.Ext.urlAppend(frameUrl, window.Ext.urlEncode({ extendedInfo: extendedInfo }));
                }
                
                var defaultDataView = n.attributes.defaultDataView;
                if (defaultDataView) {
                    frameUrl = window.Ext.urlAppend(frameUrl, window.Ext.urlEncode({ defaultDataView: defaultDataView }));
                }

                frame.setAttribute("src", frameUrl);
            }
        window.Ext.each(cnt.items.items, function (item) {
            if (item.id == n.id + "_holder") {
                item.show();
            }
            else {
                item.hide();
            }
        }, this);
        cnt.doLayout();
        if (this.renderHeader)
            window.Ext.getDom("leftNavBreadcrumbText").innerHTML = n.text;
    },
    //#endregion
    openReferenceWindow: function (entityName, id) {
        var params = String.format("width={0},height={1},status=no,resizable=yes,top={2},left={3}",
            window.Ext.DoubleGis.Global.UISettings.ActualCardWidth,
            window.Ext.DoubleGis.Global.UISettings.ActualCardHeight,
            window.Ext.DoubleGis.Global.UISettings.ScreenCenterTop,
            window.Ext.DoubleGis.Global.UISettings.ScreenCenterLeft);
        var queryString = this.ReadOnly ? '?readOnly=true' : '';
        var sUrl = Ext.DoubleGis.Global.Helpers.EvaluateUpdateEntityUrl(entityName, id, queryString);

        window.open(sUrl, '_blank', params);
    },

    processLinks: function (textToProcess) {

        var div = document.createElement("div");
        div.innerHTML = textToProcess;
        var text = div.textContent || div.innerText;

        var result = document.createElement('span');
        var j;
        for (var i = 0; i < text.length; i++) {
            if (text.charAt(i) != '<') {
                j = i;
                while (j + 1 < text.length && text.charAt(j + 1) != '<') {
                    j++;
                }
                var span = document.createElement('span');
                span.innerText = text.substring(i, j + 1);
                result.appendChild(span);
                i = j;
            } else {
                j = i + 1;
                while (text.charAt(j) != '>') {
                    j++;
                }

                var sp = text.substring(i + 1, j).split(':');

                var link = document.createElement('a');
                link.setAttribute('href', '#');
                link.innerText = sp[1];
                link.onclick = (function (entityName, entId) {
                    return function () { Ext.DoubleGis.Global.Helpers.OpenEntity(entityName, entId); };
                })(sp[0], sp[2]);

                result.appendChild(link);
                i = j;
            }
        }
        return result;
    }
});

Ext.DoubleGis.UI.SharableCard = Ext.extend(Ext.DoubleGis.UI.Card,
    {
        Share: function () {
            //alert("Операция пока не реализована");

            var entityId = Ext.getDom('Id').value;
            var url = '/' + this.EntityName + '/EditAccessSharings/' + entityId;
            var params = "dialogWidth:" + 800 + "px; dialogHeight:" + 500 + "px; status:yes; scroll:no;resizable:no;";

            window.showModalDialog(url, null, params);
        }
    });