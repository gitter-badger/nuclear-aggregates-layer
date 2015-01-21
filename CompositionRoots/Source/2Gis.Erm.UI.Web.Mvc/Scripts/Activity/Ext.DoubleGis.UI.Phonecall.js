Ext.ns("Ext.DoubleGis.UI");
Ext.DoubleGis.UI.Phonecall = Ext.extend(Ext.DoubleGis.UI.ActivityBase, {
    constructor: function (config) {
        Ext.DoubleGis.UI.Phonecall.superclass.constructor.call(this, config);
    },
    getTitleSuffix: function () {
        return this.getComboboxText("Purpose");
    },
    Build: function () {
        window.Ext.each(window.Ext.CardLookupSettings, function (item) {
            if (item.id === 'Firm') {
                item.tplFields = [{ name: "id", mapping: "Id" }, { name: 'name', mapping: "Name" }, { name: 'city', mapping: "OrganizationUnitName" }];
                item.tplHeaderTextTemplate = '<span class="x-lookup-thumb">{name}</span>&nbsp;<span class="x-lookup-thumb" style="color:gray">{city}</span>&nbsp;';
            }
        }, this);
        Ext.DoubleGis.UI.Phonecall.superclass.Build.call(this);

        Ext.get("Purpose").on("change", this.autocompleteHeader, this);
    }
});
