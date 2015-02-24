Ext.ns('Ext.ux');
Ext.ux.TimeComboBox = Ext.extend(Ext.form.ComboBox, {

    initComponent: function () {
        
        Ext.ux.TimeComboBox.superclass.initComponent.call(this);
    },
    selectByValue: function (v, scrollIntoView) {
        if (!Ext.isEmpty(v, true)) {
            var roundedTime = this.roundToHalfAHour(v);
            var r = this.findRecord(this.valueField || this.displayField, roundedTime);
            if (r) {
                this.select(this.store.indexOf(r), scrollIntoView);
                return true;
            }
        }
        return false;
    },
    roundToHalfAHour: function (date) { 
        var time = moment(date, "HH:mm", true);        
        var remainder = time.minute() % 30;
        if (remainder != 0) {
            var addingMinutes = 30 - remainder;
            time.add('minutes', addingMinutes);
        }

        var rval = time.format(Ext.CultureInfo.DateTimeFormatInfo.MomentJsShortTimePattern);
        return rval;
    }


   
});