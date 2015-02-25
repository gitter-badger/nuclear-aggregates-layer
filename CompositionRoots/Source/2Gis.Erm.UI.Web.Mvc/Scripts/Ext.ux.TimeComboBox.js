Ext.ns('Ext.ux');
Ext.ux.TimeComboBox = Ext.extend(Ext.form.ComboBox, {
    minValue: undefined,
    maxValue: undefined,
    step: undefined,
    // private override
    mode: 'local',
    // private override
    triggerAction: 'all',
    // private override
    typeAhead: false,
    displayFormat:Ext.CultureInfo.DateTimeFormatInfo.MomentJsShortTimePattern,
    initComponent: function () {
        if (!this.store) {
            this.generateStore(true);
        }
        Ext.ux.TimeComboBox.superclass.initComponent.call(this);
    },
    selectByValue: function (v, scrollIntoView) {
        if (!Ext.isEmpty(v, true)) {
            var roundedTime = this.roundToInterval(v);
            var r = this.findRecord(this.valueField || this.displayField, roundedTime);
            if (r) {
                this.select(this.store.indexOf(r), scrollIntoView);
                return true;
            }
        }
        return false;
    },
    generateStore: function (initial) {
        if (this.minValue && this.maxValue && this.step) {
            var times = this.initTime(this.minValue, this.maxValue, this.step);
            this.bindStore(times, initial);
        }
    },
    roundToInterval: function (date) {
        if (!this.step)
            return date;
        var time = moment(date, "HH:mm", true);        
        var remainder = time.minute() % this.step;
        if (remainder != 0) {
            var addingMinutes = this.step - remainder;
            time.add('minutes', addingMinutes);
        }

        var rval = time.format(this.displayFormat);
        return rval;
    },
    initTime: function (start, end, step) {

        // Функция возвращает массив строк, содержащий временные отмет от start до end c шагом step
        // start, end - строки, время в формате чч:мм:сс
        // step - число, интервал в миллисекундах
        start = moment(start, "HH:mm:ss", true);
        end = moment(end, "HH:mm:ss", true);

        var values = [];
        while (start <= end) {
            values.push(start.format(this.displayFormat));
            start.add('minutes',step);
        }

        return values;
    }


   
});