Ext.namespace('Ext.ux');
Ext.ux.SearchControl = Ext.extend(Ext.Component,
    {
        disabledClass: "ReadOnly",
        btnDis: Ext.DoubleGis.Global.Helpers.GetStaticImagePath("CommonUI/qfinddis.gif"),
        btnOff: Ext.DoubleGis.Global.Helpers.GetStaticImagePath("CommonUI/qfind.gif"),
        btnOn: Ext.DoubleGis.Global.Helpers.GetStaticImagePath("CommonUI/qfindhover.gif"),
        btnDown: Ext.DoubleGis.Global.Helpers.GetStaticImagePath("CommonUI/qfinddown.gif"),

        defaultAutoCreate: { tag: "input", type: "text", autocomplete: "off" },
        readOnly: false,
        initComponent: function ()
        {
            window.Ext.ux.SearchControl.superclass.initComponent.call(this);
            this.addEvents("change", "keydown", "keypress", "keyup", "trigger");
        },
        onRender: function (ct, position)
        {
            this.doc = Ext.isIE ? Ext.getBody() : Ext.getDoc();
            Ext.ux.SearchControl.superclass.onRender.call(this, ct, position);
            this.renderBody();
            this.wrapper = window.Ext.get(this.id + "_Wrapper");
            this.triggerBtn = window.Ext.get(this.id + "_Btn");
            this.setReadOnly(this.readOnly);
            this.setDisabled(this.disabled);
            this.setBtnOff();
            this.initHandlers();
        },
        renderBody: function ()
        {
            var template = new window.Ext.Template('<table id="{id}_Wrapper" class="x-search"><tbody><tr>',
                    '<td>',
                    '<input type="text" id="{id}_stub"/>',
                    '</td>',
                    '<td width="36">',
                    '<img id="{id}_Btn" alt="" title="' + Ext.LocalizedResources.StartSearch + '" src="{btnOff}" />',
                    '</td>',
                    '</tr></tbody></table>',
                    {
                        compiled: true,
                        disableFormats: true
                    });
            template.insertBefore(this.el.dom, this);
            this.el.replace(Ext.get(this.id + "_stub"));
        },
        initHandlers: function ()
        {
            this.mon(this.triggerBtn, "mouseout", this.setBtnOff, this);
            this.mon(this.triggerBtn, "mouseover", this.setBtnOn, this);
            this.mon(this.triggerBtn, "mousedown", this.setBtnDown, this);
            this.mon(this.triggerBtn, "mouseup", this.setBtnOn, this);
            this.mon(this.triggerBtn, "click", this.trigger, this);
            this.mon(this.el, "keyup", this.onKeyUp, this);
            this.mon(this.el, "keydown", this.onKeyDown, this);
            this.mon(this.el, "change", this.onElChange, this);
        },
        trigger: function ()
        {
            if (this.disabled || this.readOnly)
            {
                return;
            }
            this.fireEvent('trigger', this, this.getValue());
        },
        onElChange: function (e, el)
        {
            var v = this.getValue();
            this.fireEvent('change', this, v);
        },
        onKeyUp: function (e, el)
        {
            this.fireEvent('keyup', this, e);
        },
        onKeyDown: function (e, el)
        {
            if (e.keyCode == e.ENTER)
            {
                this.trigger();
                e.stopPropagation();
                e.stopEvent();
            }
            this.fireEvent('keydown', this, e);
        },
        setBtnOff: function (event) { this.triggerBtn.dom.src = this.disabled || this.readOnly ? this.btnDis : this.btnOff; },
        setBtnOn: function (event) { this.triggerBtn.dom.src = this.disabled || this.readOnly ? this.btnDis : this.btnOn; },
        setBtnDown: function (event) { this.triggerBtn.dom.src = this.disabled || this.readOnly ? this.btnDis : this.btnDown; },
        hasValue: function ()
        {
            return this.el.dom.value ? true : false;
        },
        clearValue: function ()
        {
            this.setValue('');
            return this;
        },
        getValue: function ()
        {
            return this.el.dom.value || "";
        },
        setValue: function (value)
        {
            this.el.dom.value = value;
            this.fireEvent("change", this, value);
            return this;
        },
        setReadOnly: function (readOnly)
        {
            if (readOnly === true)
            {
                this.readOnly = true;
                this.setBtnOff();
                this.el.addClass("ReadOnly");
            }
            else
            {
                this.readOnly = false;
                this.setBtnOff();
                this.el.removeClass("ReadOnly");
            }
            this.el.dom.readOnly = this.readOnly;
        },
        disable: function ()
        {
            window.Ext.ux.SearchControl.superclass.disable.call(this);
            this.setBtnOff();
        },
        enable: function ()
        {
            window.Ext.ux.SearchControl.superclass.enable.call(this);
            this.setBtnOff();
        }
    });
