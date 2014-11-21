Ext.namespace('Ext.ux');
Ext.ux.TabularButton = Ext.extend(Ext.Button, {
    handleMouseEvents: false,
    template: new Ext.Template('<button type="{0}"></button>'),
    cls: 'CrmButton',
    constructor: function (config)
    {
        window.Ext.ux.TabularButton.superclass.constructor.call(this, config);
    },
    onRender: function (ct, position)
    {
        var targs = this.getTemplateArgs();
        if (position)
        {
            this.btnEl = this.template.insertBefore(position, targs, true);
        }
        else
        {
            this.btnEl = this.template.append(ct, targs, true);
        }
        this.initButtonEl(this.btnEl, this.btnEl);
        Ext.ButtonToggleMgr.register(this);
    },
    doAutoWidth: Ext.emptyFn,
    setIconClass: function (cls)
    {
        this.iconCls = cls;
        if (this.el)
        {
            this.btnEl.dom.className = '';
            this.btnEl.addClass([cls || '']);
            this.setButtonClass();
        }
        return this;
    }
});
Ext.reg('tabularbutton', Ext.ux.TabularButton);