Ext.namespace('Ext.ux');
Ext.ux.Calendar2 = Ext.extend(Ext.Component, {
    storeFormats: {
        absolute: 'c',
        relative: 'Y-m-d\\TH:i:s'
    },

    displayFormats: {
        day: Ext.CultureInfo.DateTimeFormatInfo.PhpShortDatePattern,
        month: Ext.CultureInfo.DateTimeFormatInfo.PhpYearMonthPattern
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

        if (this.mode.display == 'month') {
            this.editor.dom.readOnly = true;
            this.mon(this.editor, 'focus', this.onButtonClick, this);
        }

        var date = this.parseIsoDate(this.store.getValue());
        this.onDateSelect(null, date);

        this.mon(this.editor, 'change', this.onEditorChange, this);
        this.mon(this.button, 'mouseout', this.updateButtonState, this);
        this.mon(this.button, 'mouseover', this.updateButtonState, this);
        this.mon(this.button, 'click', this.onButtonClick, this);
        this.mon(this.menu, 'select', this.onDateSelect, this);

        this.updateButtonState();
        this.editor.setReadOnly(this.readOnly);
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
        if (date) {
            this.setValue(date);
        } else {
            this.store.setValue('');
            this.validate();
        }
    },

    setValue: function (date) {
        this.ignoreChangeEvent = true;
        this.editor.setValue(date.format(this.displayFormat));
        this.store.setValue(date.format(this.storeFormat));
        delete this.ignoreChangeEvent;

        this.validate();
    },

    validate: function() {
        var message = this.getValidationMessage();

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
            return "Некорректное значение. Используйте формат 'ДД.MM.ГГГГ'";
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
    }
});
