/**
 * 
 */
Ext.ux.MonthPicker = Ext.extend(Ext.BoxComponent, {

    okText: Ext.LocalizedResources.OK,

    cancelText: Ext.LocalizedResources.Cancel,

    hideOnClick: false,

    // private
    initComponent: function () {
        Ext.ux.MonthPicker.superclass.initComponent.call(this);

        this.value = this.value ?
                 this.value.clearTime(true) : new Date().clearTime();

        this.addEvents('select', 'cancel');
    },

    setValue: function(value) {
        this.value = value;
    },

    // private
    onRender: function (container, position) {
        var el = document.createElement('div');
        el.className = 'x-date-picker';
        el.innerHTML = '<div class="x-date-mp"></div>';

        container.dom.insertBefore(el, position);

        this.el = Ext.get(el);

        this.monthPicker = this.el.down('div.x-date-mp');
        this.monthPicker.enableDisplayMode('block');

        this.el.unselectable();

        this.showMonthPicker();
    },

    // private
    createMonthPicker: function () {
        if (!this.monthPicker.dom.firstChild) {
            var buf = ['<table border="0" cellspacing="0">'];
            for (var i = 0; i < 6; i++) {
                buf.push(
                    '<tr><td class="x-date-mp-month"><a href="#">', Date.getShortMonthName(i), '</a></td>',
                    '<td class="x-date-mp-month x-date-mp-sep"><a href="#">', Date.getShortMonthName(i + 6), '</a></td>',
                    i === 0 ?
                    '<td class="x-date-mp-ybtn" align="center"><a class="x-date-mp-prev"></a></td><td class="x-date-mp-ybtn" align="center"><a class="x-date-mp-next"></a></td></tr>' :
                    '<td class="x-date-mp-year"><a href="#"></a></td><td class="x-date-mp-year"><a href="#"></a></td></tr>'
                );
            }
            buf.push(
                '<tr class="x-date-mp-btns"><td colspan="4"><button type="button" class="x-date-mp-ok">',
                    this.okText,
                    '</button><button type="button" class="x-date-mp-cancel">',
                    this.cancelText,
                    '</button></td></tr>',
                '</table>'
            );
            this.monthPicker.update(buf.join(''));

            this.mon(this.monthPicker, 'click', this.onMonthClick, this);
            this.mon(this.monthPicker, 'dblclick', this.onMonthDblClick, this);

            this.mpMonths = this.monthPicker.select('td.x-date-mp-month');
            this.mpYears = this.monthPicker.select('td.x-date-mp-year');

            this.mpMonths.each(function (m, a, i) {
                i += 1;
                if ((i % 2) === 0) {
                    m.dom.xmonth = 5 + Math.round(i * 0.5);
                } else {
                    m.dom.xmonth = Math.round((i - 1) * 0.5);
                }
            });
        }
    },

    // private
    showMonthPicker: function () {
        this.createMonthPicker();
        var size = { height: 190, width: 180 };
        this.el.setSize(size);
        this.monthPicker.setSize(size);
        this.monthPicker.child('table').setSize(size);

        this.mpSelMonth = (this.value).getMonth();
        this.updateMPMonth(this.mpSelMonth);
        this.mpSelYear = (this.value).getFullYear();
        this.updateMPYear(this.mpSelYear);

        this.monthPicker.show();
    },

    // private
    updateMPYear: function (y) {
        this.mpyear = y;
        var ys = this.mpYears.elements;
        for (var i = 1; i <= 10; i++) {
            var td = ys[i - 1], y2;
            if ((i % 2) === 0) {
                y2 = y + Math.round(i * 0.5);
                td.firstChild.innerHTML = y2;
                td.xyear = y2;
            } else {
                y2 = y - (5 - Math.round(i * 0.5));
                td.firstChild.innerHTML = y2;
                td.xyear = y2;
            }
            this.mpYears.item(i - 1)[y2 == this.mpSelYear ? 'addClass' : 'removeClass']('x-date-mp-sel');
        }
    },

    // private
    updateMPMonth: function (sm) {
        this.mpMonths.each(function (m, a, i) {
            m[m.dom.xmonth == sm ? 'addClass' : 'removeClass']('x-date-mp-sel');
        });
    },

    // private
    onMonthClick: function (e, t) {
        e.stopEvent();
        var el = new Ext.Element(t), pn;
        if (el.is('button.x-date-mp-cancel')) {
            this.fireEvent('cancel', this, null);
        }
        else if (el.is('button.x-date-mp-ok')) {
            this.selectedValue = new Date(this.mpSelYear, this.mpSelMonth, (this.value).getDate());
            if (this.selectedValue.getMonth() != this.mpSelMonth) {
                // 'fix' the JS rolling date conversion if needed
                this.selectedValue = new Date(this.mpSelYear, this.mpSelMonth, 1).getLastDateOfMonth();
            }
            this.hideMonthPicker();
        }
        else if ((pn = el.up('td.x-date-mp-month', 2))) {
            this.mpMonths.removeClass('x-date-mp-sel');
            pn.addClass('x-date-mp-sel');
            this.mpSelMonth = pn.dom.xmonth;

            if (this.hideOnClick) {
                this.onMonthDblClick(e, t);
            }
        }
        else if ((pn = el.up('td.x-date-mp-year', 2))) {
            this.mpYears.removeClass('x-date-mp-sel');
            pn.addClass('x-date-mp-sel');
            this.mpSelYear = pn.dom.xyear;
        }
        else if (el.is('a.x-date-mp-prev')) {
            this.updateMPYear(this.mpyear - 10);
        }
        else if (el.is('a.x-date-mp-next')) {
            this.updateMPYear(this.mpyear + 10);
        }
    },

    // private
    onMonthDblClick: function (e, t) {
        e.stopEvent();
        var el = new Ext.Element(t), pn;
        if ((pn = el.up('td.x-date-mp-month', 2))) {
            this.selectedValue = new Date(this.mpSelYear, pn.dom.xmonth, 1);
            this.hideMonthPicker();
        }
        else if ((pn = el.up('td.x-date-mp-year', 2))) {
            this.selectedValue = new Date(pn.dom.xyear, this.mpSelMonth, 1);
            this.hideMonthPicker();
        }
    },

    // private
    hideMonthPicker: function (disableAnim) {
        this.fireEvent('select', this, this.selectedValue);
    },

    // private
    beforeDestroy: function () {
        if (this.rendered) {
            Ext.destroy(
                this.monthPicker
            );
        }
    }
});

Ext.reg('monthpicker', Ext.ux.MonthPicker);