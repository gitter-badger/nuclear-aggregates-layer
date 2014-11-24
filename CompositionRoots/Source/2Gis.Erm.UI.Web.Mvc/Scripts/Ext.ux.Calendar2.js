Ext.namespace('Ext.ux');
Ext.ux.Calendar2 = Ext.extend(Ext.Component, {
    storeFormats: {
        absolute: 'c',
        relative: 'Y-m-d\\TH:i:s',
        relativeWithoutTime: 'Y-m-d',
        time: 'H:i:s'
    },

    displayFormats: {
        day: Ext.CultureInfo.DateTimeFormatInfo.PhpShortDatePattern,
        month: Ext.CultureInfo.DateTimeFormatInfo.PhpYearMonthPattern,
        time: Ext.CultureInfo.DateTimeFormatInfo.PhpShortTimePattern
    },

    initComponent: function () {
        // formats
        this.storeFormat = this.storeFormats[this.mode.store];
        this.userInputFormat = this.displayFormats.day;
        this.displayFormat = this.displayFormats[this.mode.display];

        this.minDate = this.parseIsoDate(this.minDate);
        this.maxDate = this.parseIsoDate(this.maxDate);

        // components
        this.store = Ext.get(this.storeId);
        this.editor = Ext.get(this.editorId);
        this.button = Ext.get(this.buttonId);
        this.menu = this.mode.display == 'month'
            ? new Ext.ux.MonthMenu({
                hideOnClick: true
            })
            : new Ext.menu.DateMenu({
                hideOnClick: true,
                minDate: this.minDate,
                maxDate: this.maxDate
            });

        if (this.mode.time)
        {
            var times = this.initTime(this.mode.time.min, this.mode.time.max, this.mode.time.step);
            this.time = new Ext.form.ComboBox({
                triggerAction: 'all',
                mode: 'local',
                renderTo: this.timeId,
                store: times,
                width: 80, // меняешь? посмотри в DateTimeViewModel.cshtml ширину ячейки.
                fieldClass: 'inputfields',
                triggerClass: 'calendar-time-button'
            });
            this.time.removeClass('x-form-text'); // Наличие этого класса заставляет контрол "прыгать"
        }

        if (this.mode.display == 'month') {
            this.editor.dom.readOnly = true;
            this.mon(this.editor, 'focus', this.onButtonClick, this);
        }

        this.setValue(this.parseIsoDate(this.store.getValue()));

        this.mon(this.editor, 'change', this.onEditorChange, this);
        this.mon(this.button, 'mouseout', this.updateButtonState, this);
        this.mon(this.button, 'mouseover', this.updateButtonState, this);
        this.mon(this.button, 'click', this.onButtonClick, this);
        this.mon(this.menu, 'select', this.onDateSelect, this);
        if (this.time) this.mon(this.time, 'change', this.onEditorChange, this);

        this.updateButtonState();
        this.editor.setReadOnly(this.readOnly);
    },

    initTime: function (start, end, step) {
        // Функция возвращает массив строк, содержащий временные отмет от start до end c шагом step
        // start, end - строки, время в формате чч:мм:сс
        // step - число, интервал в миллисекундах
        start = this.parseInvariantTime(start);
        end = this.parseInvariantTime(end);
        var offset = this.hoursToMilliseconds(new Date(0).getTimezoneOffset());

        var values = [];
        while (start <= end) {
            values.push(new Date(offset + start).format(this.displayFormats.time));
            start += step;
        }

        return values;
    },

    onButtonClick: function () {
        if (this.readOnly) {
            return;
        }

        this.menu.picker.setValue(this.parseIsoDate(this.store.getValue()) || new Date());
        this.menu.show(this.button, "bl");
    },

    onDateSelect: function (unused, date) {
        this.setValue(date);
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

        var date = this.parseUserDate(this.editor.getValue());
        var time = this.time ? this.parseUserTime(this.time.getValue()) : 0;
        if (date) {
            this.setValue(new Date(date.getTime() + time));
        } else {
            this.store.setValue('');
            this.validate();
        }
    },

    setValue: function (date) {
        this.ignoreChangeEvent = true;
        this.editor.setValue(date ? date.format(this.displayFormat) : '');
        if (this.time) this.time.setValue(date.format(this.displayFormats.time));
        this.store.setValue(date ? date.format(this.storeFormat) : '');
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
        var value = this.parseIsoDate(storeValue);

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

    parseIsoDate: function (value) {
        return Date.parseDate(value, this.storeFormat, false); // strict would be better, but...
    },

    parseUserDate: function (value) {
        var formats = this.userInputFormat.split('|');
        var date = null;
        for (var i = 0, len = formats.length; i < len && !date; i++) {
            date = Date.parseDate(value, formats[i], true);
        }

        return date;
    },

    parseUserTime: function(value) {
        return this.getMillisecondsFromTimeOfDay(value, this.displayFormats.time);
    },

    parseInvariantTime: function (value) {
        return this.getMillisecondsFromTimeOfDay(value, this.storeFormats.time);
    },

    getMillisecondsFromTimeOfDay: function (time, format) {
        var zeroTime = new Date(0);
        var dateString = zeroTime.format(this.storeFormats.relativeWithoutTime) + ' ' + time;
        var date = Date.parseDate(dateString, this.storeFormats.relativeWithoutTime + ' ' + format);
        return date.getTime() - this.hoursToMilliseconds(date.getTimezoneOffset());
    },

    hoursToMilliseconds: function(hours) {
        return hours * 60 * 1000;
    }
});
