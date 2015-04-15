Ext.namespace('Ext.ux');
Ext.ux.Calendar2 = Ext.extend(Ext.Component, {
    userTimeZone: '',

    storeFormats: {
        absolute: 'YYYY-MM-DDTHH:mm:ssZ', //  Z означает не букву Z, а смещение.
        relative: 'YYYY-MM-DDTHH:mm:ss'
    },

    displayFormats: {
        day: Ext.CultureInfo.DateTimeFormatInfo.MomentJsShortDatePattern,
        month: Ext.CultureInfo.DateTimeFormatInfo.MomentJsYearMonthPattern,
        time: Ext.CultureInfo.DateTimeFormatInfo.MomentJsShortTimePattern
    },

    initComponent: function () {
        this.userTimeZone = Ext.DoubleGis.TimeZoneMap[Ext.CultureInfo.DateTimeFormatInfo.TimeZoneId];

        // formats
        this.storeFormat = this.storeFormats[this.mode.store];
        this.userInputFormat = this.displayFormats.day;
        this.displayFormat = this.displayFormats[this.mode.display];

        this.minDate = this.parseStoreDate(this.minDate);
        this.maxDate = this.parseStoreDate(this.maxDate);

        // components
        this.store = Ext.get(this.storeId);
        this.editor = Ext.get(this.editorId);
        this.editor.dom.title = this.userTimeZone;
        this.button = Ext.get(this.buttonId);
        this.menu = this.mode.display == 'month'
            ? new Ext.ux.MonthMenu({
                hideOnClick: true
            })
            : new Ext.menu.DateMenu({
                hideOnClick: true,
                minDate: this.minDate ? this.minDate.toDate() : null,
                maxDate: this.maxDate ? this.maxDate.toDate() : null
            });

        if (this.mode.time)
        {
            this.time = new Ext.ux.TimeComboBox({              
                renderTo: this.timeId,
                minValue: this.mode.time.min,
                maxValue: this.mode.time.max,
                step: this.mode.time.step,
                width: 80, // меняешь? посмотри в DateTimeViewModel.cshtml ширину ячейки.
                fieldClass: 'inputfields',
                triggerClass: 'calendar-time-button'
            });
            this.time.removeClass('x-form-text'); // Наличие этого класса заставляет контрол "прыгать"
        }

        this.editor.setReadOnly(this.readOnly);

        if (this.mode.display == 'month') {
            this.editor.dom.readOnly = true;
            this.mon(this.editor, 'focus', this.onButtonClick, this);
        }

        this.setValue(this.parseStoreDate(this.store.getValue()));

        this.mon(this.editor, 'change', this.onEditorChange, this);
        this.mon(this.button, 'mouseout', this.updateButtonState, this);
        this.mon(this.button, 'mouseover', this.updateButtonState, this);
        this.mon(this.button, 'click', this.onButtonClick, this);
        this.mon(this.menu, 'select', this.onDateSelect, this);
        if (this.time) this.mon(this.time, 'change', this.onEditorChange, this);
    },

    onButtonClick: function () {
        if (this.readOnly) {
            return;
        }

        var selectedDate = this.parseStoreDate(this.store.getValue()) || moment();
        this.menu.picker.setValue(selectedDate.toDate());
        this.menu.show(this.button, "bl");
    },

    onDateSelect: function (unused, date) {
        this.setValue(date);
    },

    setReadOnly: function(value) {
        this.readOnly = value;
        this.updateButtonState();
        if (this.time) {
            this.time.setReadOnly(value);
        }
    },

    updateButtonState: function (event) {
        // псевдо-класс :hover в режиме совместимости IE не работает, 
        // :disable - предназначен только для input, поэтому эмулируем
        this.button.removeClass('calendar-button-normal');
        this.button.removeClass('calendar-button-hover');
        this.button.removeClass('calendar-button-disabled');

        if (this.readOnly) {
            this.button.addClass('calendar-button-disabled');
            return;
        }

        if (!event || event.type == 'mouseout') {
            this.button.addClass('calendar-button-normal');
            return;
        }

        if (event.type == 'mouseover') {
            this.button.addClass('calendar-button-hover');
            return;
        }
    },

    onEditorChange: function () {
        if (this.ignoreChangeEvent) {
            return;
        }

        var date = this.getValue();
        if (date) {
            this.setValue(date);
        } else {
            this.store.setValue('');
            this.validate();
        }
    },
    getValue: function() {
        var date = this.parseUserDate(this.editor.getValue());
        var time = this.time ? this.parseUserTime(this.time.getValue()) : 0;
        if (date) {
            date.add(time, "ms");        
        }

        return date;
    },
    setValue: function (date) {
        this.ignoreChangeEvent = true;
        if (date) {
            date = moment.isMoment(date) ? date : moment(date);
        }

        this.editor.setValue(date ? date.format(this.displayFormat) : '');
        if (this.time) this.time.setValue(date.format(this.displayFormats.time));

        // Ловушка: дата momentjs включает в себя локаль. 
        // Некоторые локали используют не арабские цифры (например, арабская локаль)
        // Поэтому даже используя формат хранения this.storeFormat есть риск получить нечитаемое значение.
        // Поэтому копируем дату и указываем для неё локаль с гарантированно арабскими цифрами.
        var storeDate = moment(date).locale('en');
        this.store.setValue(date ? storeDate.format(this.storeFormat) : '');

        delete this.ignoreChangeEvent;

        this.validate();
    },

    validate: function() {
        var message = this.getValidationMessage();
        this.store.dom.validationMessage = message;

        if (Ext.DoubleGis.FormValidator) {
            var field = {
                ValidationMessageId: this.storeId + "_validationMessage",
                FieldName: this.storeId
            };

            Ext.DoubleGis.FormValidator.updateValidationMessage(field, message);
        } else if(message) {
            Ext.Msg.alert('', message);
        }
    },

    getValidationMessage: function() {
        var storeValue = this.store.getValue();
        var editorValue = this.editor.getValue();
        var value = this.parseStoreDate(storeValue);

        if (editorValue && !storeValue) {
            return Ext.LocalizedResources.InvalidDateTimeText;
        }

        if (this.maxDate && value > this.maxDate) {
            return String.format(Ext.LocalizedResources.MaxDateText, this.maxDate.format(this.displayFormat));
        }

        if (this.minDate && value < this.minDate) {
            return String.format(Ext.LocalizedResources.MinDateText, this.minDate.format(this.displayFormat));
        }

        return '';
    },

    parseStoreDate: function (value) {
        return value ? moment.tz(value, this.storeFormat, this.userTimeZone) : null;
    },

    parseUserDate: function (value) {
        var formats = this.userInputFormat.split('|');
        var date = null;
        for (var i = 0; i < formats.length && !date; i++) {
            // moment.tz может принимать массив форматов, но (!) в ie5 это даёт 
            // неправильное поведение: дата парсится в текущей таймзоне и приводится в указанную (например, "2012-01-01" в "America/Los_Angeles" означает "2011-12-31T18:00:00-08:00")
            // ожидаемое (и документированное) поведение: дата парсится в указанной таймзоне (например, "2012-01-01" в "America/Los_Angeles" означает "2012-01-01T00:00:00-08:00")
            // ожидаемое поведение в ie5 достигается только при использовании метода со строкой, не массивом.
            date = moment.tz(value, formats[i], true, this.userTimeZone);
            if (!date.isValid()) {
                date = null;
            }
        }

        return date;
    },

    parseUserTime: function(value) {
        return moment(value, this.displayFormats.time) - moment().startOf("day");
    }
});
