Ext.namespace('Ext.ux');
Ext.ux.PhonecallField = Ext.extend(Ext.Component, {
	tabIndex: -1,
	readOnly: false,
    form: undefined,
	initComponent: function () {
		window.Ext.ux.LinkField.superclass.initComponent.call(this);
		this.addEvents("change", "invalid", "valid");
	},
	onRender: function() {		
		this.tabIndex = this.el.dom.tabIndex;
		this.wrapper = this.el.wrap({ cls: 'x-phonecall-wrapper' });
		
		this.content = this.wrapper.createChild({ cls: 'x-phonecall' });
		this.content.dom.style.display = "none";
		this.link = this.content.createChild({ tag: 'img', cls: 'x-phonecall-image' });
	    this.text = this.content.createChild({tag:'span' });
		this.content.dom.tabIndex = this.tabIndex;
		this.link.dom.tabIndex = -1;

		this.readOnly = this.readOnly || this.el.dom.readOnly;
		this.disabled = this.disabled || this.el.dom.disabled == "disabled";
		this.setReadOnly(this.readOnly);
		this.setDisabled(this.disabled);
		this.initHandlers();
		this.setValue(this.el.dom.value, true);
	},
	initHandlers: function () {
		this.content.on("mousedown", this.beginEdit, this);
		this.content.on("focusin", this.beginEdit, this);
		this.link.on('mousedown', function (e) { e.stopPropagation(); }, this);
		this.link.on('focusin', function (e) { e.stopPropagation(); }, this);
		this.el.on("focusout", this.elFocusOut, this);
	    this.link.on("click", this.imageClickHandler, this);
	},
	beforeDestroy: function () {
		if (this.content) this.content.removeAllListeners();
		if (this.el) this.el.removeAllListeners();
		if (this.link) this.link.removeAllListeners();
		this.wrapper.remove();
	},
	imageClickHandler: function() {
	    this.Call(this.el.dom.value);
	},
	postRequestSuccess:function(response, opts) {
	    
	},
	postRequestFailure : function(response, opts) {
	    if (response.responseText) {
	        try {
	            var frm = Ext.decode(response.responseText);	            	            
	            this.form.AddNotification(frm.Message, "CriticalError", "ServerError");
	        }
	        catch (e) {	            
	            this.form.AddNotification(response.responseText, "CriticalError", "ServerError");
	        }
	    }
	},
	Call: function (number) {
	    Ext.Msg.show({
	        msg: Ext.LocalizedResources.StartCalling,
            buttons: Ext.Msg.OK,
            icon: Ext.MessageBox.INFO
	    });
	    var url = Ext.urlAppend(Ext.SpecialOperationsServiceRestUrl + "Dial.svc/Rest/dial/" );
	    Ext.Ajax.request(
	    {
	    	url: url,
	    	method: 'POST',
            jsonData: {phone: number},
	    	success: this.postRequestSuccess,
	    	failure: this.postRequestFailure,
	    	scope: this,
			timeout: 300000 //5*60*1000
			
	});
	},
	renderValue: function () {
		this.el.dom.style.display = this.el.dom.value ? "none" : "";
		this.content.dom.style.display = this.el.dom.value ? "" : "none";		
		this.text.dom.value = this.el.dom.value;
	    this.text.dom.innerHTML = this.el.dom.value;
	},
	beginEdit: function (e) {
		if (this.disabled || this.readOnly || e.target == this.link)
			return;
		this.el.dom.style.display = "";
		this.content.dom.style.display = "none";
		this.el.focusTo(this.el.dom.value.length, e.type == "mousedown" ? undefined : 0);
	},
	elFocusOut: function () {
		this.setValue(this.el.dom.value.trim());
	},

	// #region public methods
	clearValue: function () {
		this.setValue(undefined);
	},
	getValue: function () {
		return this.el.dom.value || "";
	},
	getRawValue: function () {
		return this.el.dom.value || "";
	},
	setValue: function (value, silent) {
		silent = silent || false;
		if (this.el.dom.value !== value) {
			this.el.dom.value = value ? value.trim() : "";
			if (!silent)
				this.fireEvent("change", this, value);
		}
		if (this.validate() === true) {
			this.renderValue();
		}
	},
	validate: function () {
		
		return true;
	},
	setReadOnly: function (readOnly) {
		if (readOnly === true) {
			this.readOnly = true;
			this.el.dom.readOnly = true;
			this.el.addClass("ReadOnly");
			this.content.addClass(["ReadOnly", "x-lookup-readonly"]);
		}
		else {
			this.readOnly = false;
			this.el.dom.readOnly = false;
			if (!this.disabled) {
				this.el.removeClass("ReadOnly");
				this.content.removeClass(["ReadOnly", "x-lookup-readonly"]);
			}
		}
	},
	setDisabled: function (disabled) {
		if (disabled === true) {
			this.disable();
		}
		else {
			this.enable();
		}
	},
	disable: function () {
		this.disabled = true;
		this.el.dom.disabled = "disabled";
		this.el.addClass("ReadOnly");
		this.content.addClass(["ReadOnly", "x-lookup-readonly"]);
		this.content.dom.tabIndex = -1;
	},
	enable: function () {
		this.disabled = false;
		this.el.dom.disabled = false;
		this.content.dom.tabIndex = this.tabIndex;
		if (!this.readOnly) {
			this.el.removeClass("ReadOnly");
			this.content.removeClass(["ReadOnly", "x-lookup-readonly"]);
		}
	}
});
Ext.reg('phonecallfield', Ext.ux.PhonecallField);