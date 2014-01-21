Ext.ns("Ext.DoubleGis.UI");
Ext.DoubleGis.UI.BillSliderTip = Ext.extend(Ext.slider.Tip, {
    offsets: [150, 0],
    cls: 'bill-slider-tip',
    init: function (slider) {
        slider.on({
            scope: this,
            dragstart: this.onSlide,
            drag: this.onSlide,
            change: this.onSlide,
            destroy: this.destroy
        });
    },
    onSlide: function (slider, e, thumb) {
        if (slider.getValue() > 4) {
            this.show();
            this.body.update(Ext.LocalizedResources.BillCreationApprovalNeeded);
            this.body.dom.style.color = '#7D0000';
            this.doAutoWidth();
            this.el.alignTo('billsAmountWarning', 'b-c?', this.offsets);
        }
        else {
            this.hide();
        }
    },
    getText: function (thumb) {
        return String(thumb.value);
    }
});
Ext.DoubleGis.UI.BillPaymentsFields = [
    { name: 'PaymentNumber', type: 'string' },
    { name: 'PaymentDatePlan', type: 'date' },
    { name: 'BeginDistributionDate', type: 'date' },
    { name: 'EndDistributionDate', type: 'date' },
    { name: 'PayablePlan', type: 'float' }
];
Ext.DoubleGis.UI.BillPaymentsRecord = Ext.data.Record.create(Ext.DoubleGis.UI.BillPaymentsFields);
Ext.DoubleGis.UI.BillPaymentsControl = Ext.extend(Ext.util.Observable, {
    OrderId: null,
    PaymentsAmount: 1,
    InitPayments: null,
    PaymentsStore: null,
    GridColumnModel: null,
    PaymentsGrid: null,
    PaymentsAmountSlider: null,
    OrderSumField: null,
    OrderSum: null,
    OrderReleaseCount: null,
    BillsCount: null,
    PaymentTypes: {
        // эти свойства устанавливаются вызывающим кодом
        // важно соблюдать интерфейс
        Single: null,
        ByPlan: null,
        Custom: null
    },
    MonthYearDateFormatPattern: 'M Y',

    constructor: function (settings) {
        this.OrderId = settings.orderId;
        this.OrderSumField = new window.Ext.form.NumberField({
            renderTo: settings.orderSumElement,
            readOnly: true,
            width: 100,
            allowBlank: false,
            allowNegative: false
        });
        this.PaymentsStore = new window.Ext.data.ArrayStore({
            autoDestroy: true,
            storeId: 'billPaymentsStore',
            fields: window.Ext.DoubleGis.UI.BillPaymentsFields
        });
        var monthYearEndDateField = new window.Ext.form.DateField(
            {
                format: Ext.CultureInfo.DateTimeFormatInfo.PhpShortDatePattern,
                editable: false
            });
        monthYearEndDateField.on('select', function (field, selectedValue) {
            field.setValue(selectedValue.getLastDateOfMonth());
        });
        this.GridColumnModel = new window.Ext.grid.ColumnModel({
            columns: [
                {
                    header: Ext.LocalizedResources.Payments,
                    dataIndex: 'PaymentNumber',
                    width: 100 // заданная явно ширина, при fixed = false и forceFit = true (у всего грида) - приводит к растягиванию колонок, но при этом сохраняются их относительные друг друга размеры
                },
                {
                    header: Ext.LocalizedResources.PaymentDatePlan,
                    dataIndex: 'PaymentDatePlan',
                    xtype: 'datecolumn',
                    width: 70,
                    format: Ext.CultureInfo.DateTimeFormatInfo.PhpShortDatePattern,
                    editor: new window.Ext.form.DateField(
                            {
                                format: Ext.CultureInfo.DateTimeFormatInfo.PhpShortDatePattern,
                                editable: false
                            })
                },
                {
                    header: Ext.LocalizedResources.BeginDistributionDate,
                    dataIndex: 'BeginDistributionDate',
                    xtype: 'datecolumn',
                    width: 80,
                    format: this.MonthYearDateFormatPattern
                },
                {
                    header: Ext.LocalizedResources.EndDistributionDate,
                    dataIndex: 'EndDistributionDate',
                    xtype: 'datecolumn',
                    width: 80,
                    format: this.MonthYearDateFormatPattern,
                    editor: monthYearEndDateField
                },
                {
                    header: Ext.LocalizedResources.PayablePlan,
                    dataIndex: 'PayablePlan',
                    width: 70,
                    renderer: function (value) {
                        return value != undefined ? value : '';
                    }
                }
            ]
        });
        this.PaymentsGrid = new window.Ext.grid.EditorGridPanel({
            store: this.PaymentsStore,
            renderTo: settings.billPaymentControlElement,
            width: 760,
            height: 150,
            clicksToEdit: 1,
            colModel: this.GridColumnModel,
            viewConfig: {
                forceFit: true
            }
        });
        this.PaymentsGrid.on('afteredit', function (e) {
            if (e.field == "PaymentDatePlan") {
                e.record.commit();
                return;
            };

            var record = e.record;

            record.data.EndDistributionDate.setHours(23, 59, 59);
            record.commit();

            if (e.value.getTime() != e.originalValue.getTime()) {
                this.setDistributedPayments();
            }
        }, this);
        this.PaymentsAmountSlider = new window.Ext.Slider({
            renderTo: settings.paymentsAmountSliderElement,
            value: this.PaymentsAmount,
            minValue: 1,
            maxValue: 12,
            width: 400,
            plugins: [new window.Ext.slider.Tip(), new Ext.DoubleGis.UI.BillSliderTip()]
        });
        this.PaymentsAmountSlider.on('beforechange', function (slider, value, thumb) {
            return value <= this.OrderReleaseCount;
        }, this);
        this.PaymentsAmountSlider.on('changecomplete', function (slider, value, thumb) {
            this.getInitPayments(this.PaymentTypes.custom, value);
            if (value > 1 && !Ext.getDom("PaymentTypeByPlan").checked) {
                Ext.getDom("PaymentTypeByPlan").checked = true;
            }
            else if (value == 1 && !Ext.getDom("PaymentTypeSingle").checked) {
                Ext.getDom("PaymentTypeSingle").checked = true;
            }
        }, this);
        this.PaymentTypes = settings.paymentTypes;
    },
    getInitPayments: function (paymentType, paymentAmount) {
        var self = this;
        Ext.Ajax.request({
            timeout: 1200000,
            method: 'GET',
            url: '/Bill/GetInitPaymentsInfo',
            params: { orderId: this.OrderId, paymentType: paymentType, paymentAmount: paymentAmount },
            success: function (xhr, options) {
                var paymentInfo = Ext.decode(xhr.responseText);
                self.setInitPayments(paymentInfo);
            },
            failure: function (xhr, options) {
                Ext.MessageBox.show({
                    title: '',
                    msg: xhr.responseText,
                    buttons: Ext.MessageBox.OK,
                    fn: function () { window.close(); },
                    width: 300,
                    icon: Ext.MessageBox.ERROR
                });
            }
        });
    },
    setDistributedPayments: function () {
        var payments = [];
        this.PaymentsStore.each(function (record) {
            var payment = record.data;
            if (payment.PaymentDatePlan == undefined ||
                payment.BeginDistributionDate == undefined ||
                payment.EndDistributionDate == undefined ||
                payment.PayablePlan == undefined) {

                Ext.MessageBox.show({
                    title: '',
                    msg: Ext.LocalizedResources.AllFieldsMustBeSetForPayment + ' "' + payment.PaymentNumber + '"',
                    buttons: Ext.MessageBox.OK,
                    width: 300,
                    icon: Ext.MessageBox.ERROR
                });
                return false;
            }

            var dateTimeFormat = window.Ext.CultureInfo.DateTimeFormatInfo.PhpInvariantDateTimePattern;
            payment.BeginDistributionDate = Ext.isDate(payment.BeginDistributionDate) ? payment.BeginDistributionDate.format(dateTimeFormat) : payment.BeginDistributionDate;
            payment.EndDistributionDate = Ext.isDate(payment.EndDistributionDate) ? payment.EndDistributionDate.format(dateTimeFormat) : payment.EndDistributionDate;
            payment.PaymentDatePlan = Ext.isDate(payment.PaymentDatePlan) ? payment.PaymentDatePlan.format(dateTimeFormat) : payment.PaymentDatePlan;

            payments.push({
                PaymentDatePlan: payment.PaymentDatePlan,
                PaymentPeriodStart: payment.BeginDistributionDate,
                PaymentPeriodEnd: payment.EndDistributionDate,
                PaymentValue: payment.PayablePlan
            });
        }, this);
        var self = this;
        Ext.Ajax.request({
            timeout: 1200000,
            method: 'POST',
            url: '/Bill/GetDistributedPaymentsInfo',
            params: { orderId: this.OrderId, payments: Ext.encode(payments) },
            success: function (xhr, options) {
                var paymentInfo = Ext.decode(xhr.responseText);
                self.fillPaymentsStore(paymentInfo);
            },
            failure: function (xhr, options) {
                Ext.MessageBox.show({
                    title: '',
                    msg: xhr.responseText,
                    buttons: Ext.MessageBox.OK,
                    width: 300,
                    icon: Ext.MessageBox.ERROR
                });
                self.fillPaymentsStore(self.InitPayments);
            }
        });
    },
    fillPaymentsStore: function (payments) {
        this.PaymentsStore.removeAll();
        window.Ext.each(payments, function (item, index) {
            var record = new window.Ext.DoubleGis.UI.BillPaymentsRecord({
                PaymentNumber: Ext.LocalizedResources.Payment + ' ' + (index + 1),
                PaymentDatePlan: item.PaymentDatePlan,
                BeginDistributionDate: item.PaymentPeriodStart,
                EndDistributionDate: item.PaymentPeriodEnd,
                PayablePlan: item.PaymentValue
            });
            this.PaymentsStore.add(record);
        }, this);
    },
    setInitPayments: function (paymentsInfo) {
        this.OrderReleaseCount = paymentsInfo.OrderReleaseCount;
        this.BillsCount = paymentsInfo.BillsCount;
        this.PaymentsAmount = paymentsInfo.Payments.length;
        this.OrderSum = paymentsInfo.OrderSum;

        this.PaymentsAmountSlider.setValue(this.PaymentsAmount);
        this.OrderSumField.setValue(this.OrderSum);

        this.InitPayments = paymentsInfo.Payments;
        this.fillPaymentsStore(this.InitPayments);
    },
    getPayments: function () {
        var result = {
            HasError: false,
            ErrorMessage: null,
            ConfirmationRequired: false,
            ConfirmationMessage: null,
            Payments: []
        };
        this.PaymentsStore.each(function (record) {
            var payment = record.data;
            if (payment.PaymentDatePlan == undefined ||
                payment.BeginDistributionDate == undefined ||
                payment.EndDistributionDate == undefined ||
                payment.PayablePlan == undefined) {

                result.HasError = true;
                result.ErrorMessage = Ext.LocalizedResources.AllFieldsMustBeSetForPayment + ' "' + payment.PaymentNumber + '"';
                return result;
            }
            var dateTimeFormat = window.Ext.CultureInfo.DateTimeFormatInfo.PhpInvariantDateTimePattern;
            payment.BeginDistributionDate = Ext.isDate(payment.BeginDistributionDate) ? payment.BeginDistributionDate.format(dateTimeFormat) : payment.BeginDistributionDate;
            payment.EndDistributionDate = Ext.isDate(payment.EndDistributionDate) ? payment.EndDistributionDate.format(dateTimeFormat) : payment.EndDistributionDate;
            payment.PaymentDatePlan = Ext.isDate(payment.PaymentDatePlan) ? payment.PaymentDatePlan.format(dateTimeFormat) : payment.PaymentDatePlan;

            result.Payments.push(payment);
        }, this);

        if (result.HasError == true) return result;

        if (this.BillsCount > 0) {
            result.ConfirmationRequired = true;
            result.ConfirmationMessage = Ext.LocalizedResources.AllExistingBillsWillBeDeleted;
        }

        return result;
    },
    getOrderSum: function () {
        return this.OrderSum;
    }
});
