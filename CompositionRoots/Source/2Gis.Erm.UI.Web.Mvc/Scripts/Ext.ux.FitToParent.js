Ext.namespace('Ext.ux');
Ext.ux.FitToParent = Ext.extend(Object, {
    fitWidth: true,
    fitHeight: true,
    offsets: [0, 0],
    constructor: function (config)
    {
        config = config || {};
        if (config.tagName || config.dom || Ext.isString(config))
        {
            config = { parent: config };
        }
        Ext.apply(this, config);
    },
    init: function (c)
    {
        this.component = c;
        c.on('render', function (cmp)
        {
            this.parent = Ext.get(this.parent || cmp.getPositionEl().dom.parentNode);
            if (cmp.doLayout)
            {
                cmp.monitorResize = true;
                cmp.doLayout = cmp.doLayout.createInterceptor(this.fitSize, this);
            } else
            {
                this.fitSize();
                Ext.EventManager.onWindowResize(this.fitSize, this);
            }
        }, this, { single: true });
    },
    fitSize: function ()
    {
        var pos = this.component.getPosition(true),
            size = this.parent.getViewSize();
        this.component.setSize(
            this.fitWidth ? size.width - pos[0] - this.offsets[0] : undefined,
            this.fitHeight ? size.height - pos[1] - this.offsets[1] : undefined);
    }
});
Ext.preg('fittoparent', Ext.ux.FitToParent);