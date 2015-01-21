Ext.ns("Ext.DoubleGis.UI");
Ext.DoubleGis.UI.Letter = Ext.extend(Ext.DoubleGis.UI.ActivityBase, {
    constructor: function (config) {
        Ext.DoubleGis.UI.Letter.superclass.constructor.call(this, config);
    },
    Build: function () {
        window.Ext.each(window.Ext.CardLookupSettings, function (item) {
            if (item.id === 'Firm') {
                item.tplFields = [{ name: "id", mapping: "Id" }, { name: 'name', mapping: "Name" }, { name: 'city', mapping: "OrganizationUnitName" }];
                item.tplHeaderTextTemplate = '<span class="x-lookup-thumb">{name}</span>&nbsp;<span class="x-lookup-thumb" style="color:gray">{city}</span>&nbsp;';
            }
        }, this);
        Ext.DoubleGis.UI.Letter.superclass.Build.call(this);
    }
});
