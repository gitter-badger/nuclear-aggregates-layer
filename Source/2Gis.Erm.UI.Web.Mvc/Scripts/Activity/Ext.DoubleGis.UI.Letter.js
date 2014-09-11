Ext.ns("Ext.DoubleGis.UI");
Ext.DoubleGis.UI.Letter = Ext.extend(Ext.DoubleGis.UI.ActivityBase, {
    constructor: function (config) {
        Ext.DoubleGis.UI.Letter.superclass.constructor.call(this, config);
    },
    getPurpose: function () {
        return Ext.LocalizedResources.Letter;
    },
    Build: function () {
        Ext.DoubleGis.UI.Letter.superclass.Build.call(this);
    }
});
