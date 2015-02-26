Ext.ns('Ext.ux');
Ext.ux.TimeComboBox = Ext.extend(Ext.form.ComboBox, {
    minValue: undefined,
    maxValue: undefined,
    step: undefined,
    // private override
    mode: 'local',
    // private override
    triggerAction: 'all',
    needRoundInSelect: true,
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
            var r = this.findRecord(this.valueField , roundedTime);
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
            var store = new window.Ext.data.Store( {
                xtype: 'jsonstore',
                autoLoad: false,
                data: times,
                reader: new window.Ext.data.JsonReader({                  
                    fields: [{ name: "id", mapping: "id" }, { name: "name", mapping: 'name' }]
                })
            });
            this.displayField = 'name';
            this.valueField = 'id';
            this.store = store;
        }
    },
    roundToInterval: function (date) {
        var time = moment(date, this.displayFormat, true);
        if (!this.step || !this.needRoundInSelect)
            return moment.duration(time).asMilliseconds();
           
        var remainder = time.minute() % this.step;
        if (remainder != 0) {
            var addingMinutes = this.step - remainder;
            time.add('minutes', addingMinutes);
        }
        
        return moment.duration(time).asMilliseconds();
    },
    initTime: function (start, end, step) {

        // Функция возвращает массив строк, содержащий временные отмет от start до end c шагом step
        // start, end - строки, время в формате чч:мм:сс
        // step - число, интервал в миллисекундах
        start = moment(start, "HH:mm:ss", true);
        end = moment(end, "HH:mm:ss", true);

        var values = [];
        while (start <= end) {
            //var id = start.clone();
            values.push({ id: moment.duration(start).asMilliseconds(), name: start.format(this.displayFormat) });
            start.add('minutes',step);
        }

        return values;
    }


   
});