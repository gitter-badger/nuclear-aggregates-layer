Ext.namespace('Ext.ux');
Ext.ux.LinkField = Ext.extend(Ext.Component, {
    contactTypeRegistry:
        {
            email:
                {
                    linkCls: 'x-contact-link-email',
                    protocolPrefix: 'mailto:',
                    protocolRegex: /^mailto:([а-яёa-z0-9-])/,
                    validator: function (value)
                    {
                        return value == '' || /^[а-яёa-z0-9_+.-]+\@([а-яёa-z0-9-]+\.)+[а-яёa-z0-9]{2,4}$/i.test(value);
                    },
                    validationMessage: Ext.LocalizedResources.LinkFieldInvalidEmailMessage
                },
            url:
                    {
                        linkCls: 'x-contact-link-href',
                        protocolPrefix: 'http://',
                        protocolRegex: /^https?:\/\/([а-яёa-z0-9-])/,
                        validator: function (value)
                        {
                            return value == '' || /^https?:\/\/([а-яёa-z0-9-]+\.)+[а-яёa-z0-9]{2,4}.*$/.test(value);
                        },
                        validationMessage: Ext.LocalizedResources.LinkFieldInvalidWebsiteMessage
                    }
        },
    tabIndex: -1,
    readOnly: false,
    initComponent: function ()
    {
        window.Ext.ux.LinkField.superclass.initComponent.call(this);
        this.addEvents("change", "invalid", "valid");
    },
    onRender: function ()
    {
        this.contactTypeCfg = this.contactTypeCfg || this.contactTypeRegistry.url;
        this.tabIndex = this.el.dom.tabIndex;
        this.wrapper = this.el.wrap({ cls: 'x-contact-wrapper' });
        this.content = this.wrapper.createChild({ cls: 'x-contact' });
        this.content.dom.style.display = "none";
        this.link = this.content.createChild({ tag: 'a', href: '#', target: '_blank', cls: this.contactTypeCfg.linkCls });

        this.content.dom.tabIndex = this.tabIndex;
        this.link.dom.tabIndex = -1;

        this.readOnly = this.readOnly || this.el.dom.readOnly;
        this.disabled = this.disabled || this.el.dom.disabled == "disabled";
        this.setReadOnly(this.readOnly);
        this.setDisabled(this.disabled);
        this.initHandlers();
        this.setValue(this.el.dom.value, true);
    },
    initHandlers: function ()
    {
        this.content.on("mousedown", this.beginEdit, this);
        this.content.on("focusin", this.beginEdit, this);
        this.link.on('mousedown', function (e) { e.stopPropagation(); }, this);
        this.link.on('focusin', function (e) { e.stopPropagation(); }, this);
        this.el.on("focusout", this.elFocusOut, this);
    },
    beforeDestroy: function ()
    {
        if (this.content) this.content.removeAllListeners();
        if (this.el) this.el.removeAllListeners();
        if (this.link) this.link.removeAllListeners();
        this.wrapper.remove();
    },
    renderValue: function ()
    {
        this.el.dom.style.display = this.el.dom.value ? "none" : "";
        this.content.dom.style.display = this.el.dom.value ? "" : "none";
        this.link.dom.href = this.contactTypeCfg.protocolRegex.test(this.el.dom.value) ? this.el.dom.value : this.contactTypeCfg.protocolPrefix + this.el.dom.value;
        this.link.dom.innerText = this.el.dom.value;
    },
    beginEdit: function (e)
    {
        if (this.disabled || this.readOnly || e.target == this.link)
            return;
        this.el.dom.style.display = "";
        this.content.dom.style.display = "none";
        this.el.focusTo(this.el.dom.value.length, e.type == "mousedown" ? undefined : 0);
    },
    elFocusOut: function ()
    {
        this.setValue(this.el.dom.value.trim());
    },

    // #region public methods
    clearValue: function ()
    {
        this.setValue(undefined);
    },
    getValue: function ()
    {
        return this.el.dom.value || "";
    },
    getRawValue: function ()
    {
        return this.el.dom.value || "";
    },
    setValue: function (value, silent)
    {
        silent = silent || false;
        if (this.el.dom.value !== value)
        {
            this.el.dom.value = value ? value.trim() : "";
            if (!silent)
                this.fireEvent("change", this, value);
        }
        if (this.validate() === true)
        {
            this.renderValue();
        }
    },
    validate: function ()
    {
        var result = this.contactTypeCfg.validator(this.el.dom.value);
        if (result !== true)
        {
            result = this.contactTypeCfg.validator(this.contactTypeCfg.protocolPrefix + this.el.dom.value);
            if (result !== true)
                this.fireEvent("invalid", this, this.contactTypeCfg.validationMessage);
            else
            {
                this.setValue(this.contactTypeCfg.protocolPrefix + this.el.dom.value, true);
            }
        }
        else
        {
            this.fireEvent("valid", this);
        }
        return result;
    },
    setReadOnly: function (readOnly)
    {
        if (readOnly === true)
        {
            this.readOnly = true;
            this.el.dom.readOnly = true;
            this.el.addClass("ReadOnly");
            this.content.addClass(["ReadOnly", "x-lookup-readonly"]);
        }
        else
        {
            this.readOnly = false;
            this.el.dom.readOnly = false;
            if (!this.disabled)
            {
                this.el.removeClass("ReadOnly");
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
        this.el.dom.disabled = "disabled";
        this.el.addClass("ReadOnly");
        this.content.addClass(["ReadOnly", "x-lookup-readonly"]);
        this.content.dom.tabIndex = -1;
    },
    enable: function ()
    {
        this.disabled = false;
        this.el.dom.disabled = false;
        this.content.dom.tabIndex = this.tabIndex;
        if (!this.readOnly)
        {
            this.el.removeClass("ReadOnly");
            this.content.removeClass(["ReadOnly", "x-lookup-readonly"]);
        }
    }
});
Ext.reg('linkfield', Ext.ux.LinkField);