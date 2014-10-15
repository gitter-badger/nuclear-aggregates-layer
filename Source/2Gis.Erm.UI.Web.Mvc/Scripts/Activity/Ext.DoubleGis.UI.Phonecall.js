Ext.ns("Ext.DoubleGis.UI");
Ext.DoubleGis.UI.Phonecall = Ext.extend(Ext.DoubleGis.UI.ActivityBase, {
    constructor: function (config) {
        Ext.DoubleGis.UI.Phonecall.superclass.constructor.call(this, config);
    },
    getTitleSuffix: function () {
        return this.getComboboxText("Purpose");
    },
    Build: function () {
        Ext.DoubleGis.UI.Phonecall.superclass.Build.call(this);

        Ext.get("Purpose").on("change", this.autocompleteHeader, this);

        this.createTimeCombo("ScheduledStartTime");
    }
});
