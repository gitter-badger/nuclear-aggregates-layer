Ext.ns("Ext.DoubleGis.UI");
Ext.DoubleGis.UI.Appointment = Ext.extend(Ext.DoubleGis.UI.ActivityBase, {
    constructor: function (config) {
        Ext.DoubleGis.UI.Appointment.superclass.constructor.call(this, config);
    },
    getPurpose: function () {
        return this.getComboboxText("Purpose");
    },
    Build: function () {
        Ext.DoubleGis.UI.Appointment.superclass.Build.call(this);

        Ext.get("Purpose").on("change", this.autocompleteHeader, this);

        this.createTimeCombo("ScheduledStartTime");
        this.createTimeCombo("ScheduledEndTime");
    }
});
