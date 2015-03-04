Ext.ns("Ext.DoubleGis.UI");
Ext.DoubleGis.UI.Appointment = Ext.extend(Ext.DoubleGis.UI.ActivityBase, {
    duration:undefined,
    constructor: function (config) {
        Ext.DoubleGis.UI.Appointment.superclass.constructor.call(this, config);
        this.duration = config.duration;
    },
    getTitleSuffix: function () {
        return this.getComboboxText("Purpose");
    },
    Build: function () {
        Ext.DoubleGis.UI.Appointment.superclass.Build.call(this);
       
        Ext.get("Purpose").on("change", this.autocompleteHeader, this);
        Ext.get("ScheduledStart").on("change", this.test, this);
    },
    test:function() {
        var time = Ext.getCmp("ScheduledStart").getElementDate();
        if (time) {
            var newTime = time.add(this.duration, 'minutes');
            Ext.getCmp("ScheduledEnd").setValue(newTime);
        }
    }
});
