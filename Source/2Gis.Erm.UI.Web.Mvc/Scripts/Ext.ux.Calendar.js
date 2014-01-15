Ext.namespace('Ext.ux');
Ext.ux.Calendar = Ext.extend(Ext.Component, {
    periodTypes: { NONE: 0, MONTHLY_UPPER_BOUND: 1, MONTHLY_LOWER_BOUND: 2 },
    periodType: 0,
    displayStyles: { FULL: 0, WITHOUT_DAY_NUMBER: 1 },
    displayStyle: 0,
    disabledClass: "ReadOnly",
    btnDis: Ext.DoubleGis.Global.Helpers.GetStaticImagePath("CommonUI/btn_dis_cal.gif"),
    btnOff: Ext.DoubleGis.Global.Helpers.GetStaticImagePath("CommonUI/btn_off_cal.gif"),
    btnOn: Ext.DoubleGis.Global.Helpers.GetStaticImagePath("CommonUI/btn_on_cal.gif"),

    format: Ext.form.DateField.prototype.format,
    altFormats: Ext.form.DateField.prototype.altFormats + "|d.m.Y G:i:s",
    disabledDaysText: Ext.form.DateField.prototype.disabledDaysText,
    disabledDatesText: Ext.form.DateField.prototype.disabledDatesText,
    minText: Ext.form.DateField.prototype.minText,
    maxText: Ext.form.DateField.prototype.maxText,
    invalidText: Ext.form.DateField.prototype.invalidText,
    validationEvent: 'change',
    validationMessage: '',
    showToday: true,
    startDay: Ext.form.DateField.prototype.startDay,
    defaultAutoCreate: { tag: "input", type: "text", size: "10", autocomplete: "off" },
    initTime: '12', // 24 hour format
    initTimeFormat: 'H',
    monthlyUpperBoundText: Ext.LocalizedResources.MonthlyUpperBoundText,
    monthlyLowerBoundText: Ext.LocalizedResources.MonthlyLowerBoundText,
    readOnly: false,
    initComponent: function ()
    {
        window.Ext.ux.Calendar.superclass.initComponent.call(this);
        this.addEvents("change", "select", "keydown", "keypress", "keyup", "valid", "invalid");
        if (Ext.isString(this.minValue))
        {
            this.minValue = this.parseDate(this.minValue);
            this.minValue = this.shiftOffset && Ext.isDate(this.minValue) ? this.minValue.shiftOffset() : this.minValue;
        }
        if (Ext.isString(this.maxValue))
        {
            this.maxValue = this.parseDate(this.maxValue);
            this.maxValue = this.shiftOffset && Ext.isDate(this.maxValue) ? this.maxValue.shiftOffset() : this.maxValue;
        }
        this.disabledDatesRE = null;
        this.shiftOffset = this.shiftOffset === undefined ? true : this.shiftOffset;
        this.initDisabledDays();
    },
    onRender: function (ct, position)
    {
        this.doc = Ext.isIE ? Ext.getBody() : Ext.getDoc();
        Ext.ux.Calendar.superclass.onRender.call(this, ct, position);

        this.renderBody();
        if (this.value)
        {
            this.setValue(this.parseDate(this.value), true);
        }
        else
        {
            this.setValue(this.parseDate(this.el.dom.value), true);
        }
        this.wrapper = window.Ext.get(this.id + "_Wrapper");
        this.triggerBtn = window.Ext.get(this.id + "_Btn");
        this.setReadOnly(this.readOnly);
        this.setDisabled(this.disabled);
        this.triggerBtn.dom.src = this.disabled || this.readOnly ? this.btnDis : this.btnOff;
        this.initHandlers();
    },
    renderBody: function ()
    {
        var withoutDays = this.displayStyle == this.displayStyles.WITHOUT_DAY_NUMBER;
        var template = new window.Ext.Template(
                '<table id="{id}_Wrapper" class="x-calendar"><tbody><tr>',
                    '<td>',
                    withoutDays ? '<label id="{id}_withoutdays" readonly="readonly"/>' : '',
                    withoutDays ? '</td>' : '',
                    withoutDays ? '<td width="1">' : '',
                    '<input type="text" id="{id}_stub"/>',
                    '</td>',
                    '<td width="36">',
                    '<img id="{id}_Btn" alt="" title="" src="{btnOff}" />',
                    '</td>',
                    '</tr></tbody></table>',
                    {
                        compiled: true,
                        disableFormats: true
                    }
                );
        template.insertBefore(this.el.dom, this);
        this.el.replace(Ext.get(this.id + "_stub"));
        if (withoutDays) {
            this.elWithoutDay = Ext.get(this.id + "_withoutdays");
            this.elWithoutDay.addClass(this.el.dom.className);
            this.el.hide();
        }
    },
    initHandlers: function ()
    {
        this.mon(this.triggerBtn, "mouseout", this.setBtnOff, this);
        this.mon(this.triggerBtn, "mouseover", this.setBtnOn, this);
        this.mon(this.triggerBtn, "click", this.onTriggerClick, this);
        this.mon(this.el, "keyup", this.onKeyUp, this);
        this.mon(this.el, "keydown", this.onKeyDown, this);
        this.mon(this.el, "change", this.onElChange, this);
        this.mon(this.el, 'focus', this.onFocus, this);
        if (this.elWithoutDay) this.mon(this.elWithoutDay, "click", this.onTriggerClick, this);
    },
    parseDate: function (value)
    {
        if (!value || Ext.isDate(value))
        {
            return value;
        }

        var v = this.safeParse(value, this.format), af = this.altFormats, afa = this.altFormatsArray;
        if (!v && af)
        {
            afa = afa || af.split("|");
            for (var i = 0, len = afa.length; i < len && !v; i++)
            {
                v = this.safeParse(value, afa[i]);
            }
        }
        return v || "";
    },
    safeParse: function (value, format)
    {
        if (Date.formatContainsHourInfo(format))
        {
            // if parse format contains hour information, no DST adjustment is necessary
            return Date.parseDate(value, format);
        } else
        {
            // set time to 12 noon, then clear the time
            var parsedDate = Date.parseDate(value + ' ' + this.initTime, format + ' ' + this.initTimeFormat);

            if (parsedDate)
            {
                return parsedDate.clearTime();
            }
        }
        return undefined;
    },
    // private
    initDisabledDays: function ()
    {
        if (this.disabledDates)
        {
            var dd = this.disabledDates,
                len = dd.length - 1,
                re = "(?:";

            Ext.each(dd, function (d, i)
            {
                re += Ext.isDate(d) ? '^' + Ext.escapeRe(d.dateFormat(this.format)) + '$' : dd[i];
                if (i != len)
                {
                    re += '|';
                }
            }, this);
            this.disabledDatesRE = new RegExp(re + ')');
        }
    },
    menuEvents: function (method)
    {
        this.menu[method]('select', this.onSelect, this);
        this.menu[method]('hide', this.onMenuHide, this);
        if (this.menu.picker.monthPicker && (this.periodType == this.periodTypes.MONTHLY_LOWER_BOUND || this.periodType == this.periodTypes.MONTHLY_UPPER_BOUND))
        {
            this.menu.picker.monthPicker[method]('dblclick', this.onMonthDblClick, this);
            this.menu.picker.monthPicker[method]('click', this.onMonthClick, this);
        }

    },
    showValidationMessage: function ()
    {
        alert(this.validationMessage);

        //        if (Ext.fly(this.id + "_validationMessage") && Ext.DoubleGis.FormValidator)
        //        {
        //            var field = {
        //                ValidationMessageId: this.id + "_validationMessage",
        //                FieldName: this.id
        //            };
        //            Ext.DoubleGis.FormValidator.updateValidationMessage(field, this.validationMessage);
        //        }
        //        else
        //        {
        //            alert(this.validationMessage);
        //        }
    },
    isValid: function ()
    {
        this.validationMessage = '';
        if (!Ext.isEmpty(this.getValue()))
        {
            if (this.el.dom.value && !this.getValue())
            {
                this.validationMessage = String.format(this.invalidText, this.el.dom.value, this.format);
                this.fireEvent('invalid', this);
                return false;
            }
            if (this.getValue() < this.minValue)
            {
                this.validationMessage = String.format(this.minText, this.minValue.dateFormat(this.format));
                this.fireEvent('invalid', this);
                return false;
            }
            if (this.getValue() > this.maxValue)
            {
                this.validationMessage = String.format(this.maxText, this.maxValue.dateFormat(this.format));
                this.fireEvent('invalid', this);
                return false;
            }
            if (this.disabledDatesRE && this.format && this.disabledDatesRE.test(this.getValue().dateFormat(this.format)))
            {
                this.validationMessage = this.disabledDatesText;
                this.fireEvent('invalid', this);
                return false;
            }
            if (this.periodType == this.periodTypes.MONTHLY_LOWER_BOUND && this.getValue().getFirstDateOfMonth().valueOf() != this.getValue().valueOf())
            {
                this.validationMessage = this.monthlyLowerBoundText;
                this.fireEvent('invalid', this);
                return false;
            }

            if (this.periodType == this.periodTypes.MONTHLY_UPPER_BOUND && this.getValue().getLastDateOfMonth().valueOf() != this.getValue().valueOf())
            {
                this.validationMessage = this.monthlyUpperBoundText;
                this.fireEvent('invalid', this);
                return false;
            }
        }
        this.fireEvent('valid', this);
        return true;
    },
    formatDate: function (date)
    {
        return Ext.isDate(date) ? date.dateFormat(this.format) : date;
    },
    onDestroy: function ()
    {
        Ext.destroy(this.menu, this.keyNav);
        Ext.ux.Calendar.superclass.onDestroy.call(this);
    },
    onTriggerClick: function ()
    {
        if (this.disabled || this.readOnly)
        {
            return;
        }
        if (this.menu == null)
        {
            this.menu = new Ext.menu.DateMenu({
                hideOnClick: false,
                focusOnSelect: false
            });
        }
        Ext.apply(this.menu.picker, {
            minDate: this.minValue,
            maxDate: this.maxValue,
            disabledDatesRE: this.disabledDatesRE,
            disabledDatesText: this.disabledDatesText,
            disabledDays: this.disabledDays,
            disabledDaysText: this.disabledDaysText,
            format: this.format,
            showToday: this.showToday,
            startDay: this.startDay,
            minText: String.format(this.minText, this.formatDate(this.minValue)),
            maxText: String.format(this.maxText, this.formatDate(this.maxValue))
        });



        this.menu.picker.setValue(this.getValue() || new Date());
        this.startValue = this.getValue();



        this.menu.show(this.el, "tl-bl?");
        if (this.periodType == this.periodTypes.MONTHLY_LOWER_BOUND || this.periodType == this.periodTypes.MONTHLY_UPPER_BOUND)
        {
            this.menu.picker.showMonthPicker();
        }
        this.menuEvents('on');
    },

    /*MonthPicker Handlers*/
    onMonthDblClick: function (e, t)
    {
        e.stopEvent();
        var d;
        if (this.periodType == this.periodTypes.MONTHLY_LOWER_BOUND)
            d = new Date(this.menu.picker.mpSelYear, this.menu.picker.mpSelMonth, 1).getFirstDateOfMonth();
        else if (this.periodType == this.periodTypes.MONTHLY_UPPER_BOUND)
            d = new Date(this.menu.picker.mpSelYear, this.menu.picker.mpSelMonth, 1).getLastDateOfMonth();
        else
            d = new Date(this.mpSelYear, this.mpSelMonth, (this.menu.picker.activeDate || this.menu.picker.value).getDate());
        this.onSelect(this.menu, d);
    },
    onMonthClick: function (e, t)
    {
        e.stopEvent();
        var el = new Ext.Element(t);
        if (el.is('button.x-date-mp-cancel'))
        {
            this.menu.hide();
        }
        else if (el.is('button.x-date-mp-ok'))
        {
            var d;
            if (this.periodType == this.periodTypes.MONTHLY_LOWER_BOUND)
                d = new Date(this.menu.picker.mpSelYear, this.menu.picker.mpSelMonth, 1).getFirstDateOfMonth();
            else if (this.periodType == this.periodTypes.MONTHLY_UPPER_BOUND)
                d = new Date(this.menu.picker.mpSelYear, this.menu.picker.mpSelMonth, 1).getLastDateOfMonth();
            else
                d = new Date(this.mpSelYear, this.mpSelMonth, (this.menu.picker.activeDate || this.menu.picker.value).getDate());
            this.onSelect(this.menu, d);
        }
    },

    onSelect: function (m, d)
    {
        this.setValue(d);
        this.fireEvent('select', this, d);
        this.menu.hide();
    },
    onMenuHide: function ()
    {
        this.focus(false, 60);
        this.menuEvents('un');
    },
    onFocus: function ()
    {
        this.startValue = this.getValue();
        this.fireEvent('focus', this);
    },
    onElChange: function (evt, el)
    {
        var v = this.getValue();
        if (String(v) !== String(this.startValue))
        {
            this.fireEvent('change', this, v, this.startValue);
        }
        if (this.validationEvent !== false && this.validationEvent == 'change')
        {
            if (!this.isValid())
            {
                this.showValidationMessage();
                this.setRawValue(this.startValue.dateFormat(this.format));
            }
        }
    },
    onKeyPress: function (evt, el)
    {
        this.fireEvent('keypress', this, evt);
    },
    onKeyUp: function (evt, el)
    {
        this.fireEvent('keyup', this, evt);
    },
    onKeyDown: function (evt, el)
    {
        this.fireEvent('keydown', this, evt);
    },
    setBtnOff: function (event) { if (event.target) { event.target.src = this.disabled || this.readOnly ? this.btnDis : this.btnOff; } },
    setBtnOn: function (event) { if (event.target) { event.target.src = this.disabled || this.readOnly ? this.btnDis : this.btnOn; } },
    setDisabledDates: function (dd)
    {
        this.disabledDates = dd;
        this.initDisabledDays();
        if (this.menu)
        {
            this.menu.picker.setDisabledDates(this.disabledDatesRE);
        }
    },
    setDisabledDays: function (dd)
    {
        this.disabledDays = dd;
        if (this.menu)
        {
            this.menu.picker.setDisabledDays(dd);
        }
    },
    setMinValue: function (dt)
    {
        this.minValue = (Ext.isString(dt) ? this.parseDate(dt) : dt);
        if (this.menu)
        {
            this.menu.picker.setMinDate(this.minValue);
        }
    },
    setMaxValue: function (dt)
    {
        this.maxValue = (Ext.isString(dt) ? this.parseDate(dt) : dt);
        if (this.menu)
        {
            this.menu.picker.setMaxDate(this.maxValue);
        }
    },
    clearValue: function ()
    {
        this.setValue('');
        return this;
    },
    getValue: function ()
    {
        var date = this.parseDate(this.el.dom.value);
        date = this.shiftOffset && Ext.isDate(date) ? date.clearOffset() : date;
        return date || "";
    },
    setValue: function (date, silent)
    {
        date = this.parseDate(date);
        date = this.shiftOffset && Ext.isDate(date) ? date.shiftOffset() : date;

        silent = silent || false;
        // FIXME {all, 09.01.2014}: нужна доработка контрола - зачем-то затирается точное дата и время/вместо этого остается просто дата,
        // получаем возможность ошибки - получили с сервера дату со временем, добавили смещение, отрезали часть содержащую время,
        // перед постом карточки на сервер, убираем смещение из this.el.dom.value, однако в итоге получаем не ту же дату и время которую первоначально получили от сервера
        // а измененное значение => если данное значение datetime на сервере не readonly, то произойдет запись в базу и данные будут изменены, хотя реально никаких изменений небыло
        // Пока эта ошибка маскируется тем, что значение даты времени присваивается где-то в серверной логике, и данные передаваемые с клиента игнорируются, т.е. для клиента эта дата и время readonly
        // Пока усечение оставлено, т.к. если его убрать начинает отображаться не только дата но и время в контроле календаря, однако, очевидно нужно изменить сам контрол (см. ERM-2842),
        // чтобы он четко разделял отображение даты времени и хранение - отображать можно что угодно, а вот хранить нужно полноценное дату и время причем в UTC.
        this.el.dom.value = this.formatDate(date); 
        if (this.elWithoutDay)
        {
            this.elWithoutDay.update(date ? date.format(Ext.CultureInfo.DateTimeFormatInfo.PhpYearMonthPattern) : date);
        }
        if (!silent)
        {
            this.fireEvent("change", this, date);
        }
        if (!silent && this.validationEvent !== false && this.validationEvent == 'change')
        {
            if (!this.isValid())
            {
                this.showValidationMessage();
                if (this.startValue)
                    this.setRawValue(this.startValue.dateFormat(this.format));
            }
        }
        return this;
    },
    getRawValue: function ()
    {
        return this.el.dom.value;
    },
    setRawValue: function (date)
    {
        this.el.dom.value = this.formatDate(this.parseDate(date));
    },
    setReadOnly: function (readOnly)
    {
        if (readOnly === true)
        {
            this.readOnly = true;
            this.triggerBtn.dom.src = this.btnDis;
            this.el.addClass("ReadOnly");
            if (this.elWithoutDay) this.elWithoutDay.addClass("ReadOnly");
        }
        else
        {
            this.readOnly = false;
            this.triggerBtn.dom.src = this.btnOff;
            this.el.removeClass("ReadOnly");
            if (this.elWithoutDay) this.elWithoutDay.removeClass("ReadOnly");
        }
        this.el.dom.readOnly = this.readOnly;
    },
    disable: function ()
    {
        window.Ext.ux.Calendar.superclass.disable.call(this);
        this.triggerBtn.dom.src = this.btnDis;
    },
    enable: function ()
    {
        window.Ext.ux.Calendar.superclass.enable.call(this);
        this.triggerBtn.dom.src = this.btnOff;
    }
});
Ext.reg('calendar', Ext.ux.Calendar);