Ext.ns("Ext.DoubleGis.UI.OrderPosition");

Ext.DoubleGis.UI.OrderPosition.BusinessLogic = Ext.extend(Ext.util.Observable, {
    TenPower: null,
    Constants: {
        AmountSpecificationMode: {
            Counting: 1,
            FixedValue: 2,
            ArbitraryValue: 3
        },

        LinkingObjectType: {
            Firm: 'Firm'
        },

        FixedLinkedFirmCount: '1'
    },

    Settings: {
        DecimalDigits: null,
        DiscountPercentDecimalDigits: 4,
        DiscountRecalcDelay: 1000
    },

    LocalData: {
        Id: null,
        OrderId: null,
        SelectedAdvertisementCount: null,
        IsLocked: null
    },

    ServerData: {
        VatRatio: null,
        PricePerUnit: null,
        PlatformName: null,
        SnapObjectType: null,
        OrderReleaseCountPlan: null,
        PricePositionAmount: null,
        AmountSpecificationMode: null,
        LinkingObjectsSchema: null,
        IsPositionComposite: null,
        IsPositionOfPlannedProvisionSalesModel: null,
        IsPositionCategoryBound: null
    },

    ComputationalData: {
        PayablePricePriorDiscount: null,
        ShipmentPlan: null,
        PayablePrice: null,
        PricePerUnit: null
    },

    UI: {
        Hiddens: {
            DiscountSum: null,
            DiscountPercent: null
        },

        Texts: {
            PricePerUnit: null,
            PricePerUnitWithVat: null,
            Amount: null,
            Platform: null,
            ShipmentPlan: null,
            PayablePrice: null,
            PayablePlan: null,
            DiscountPercent: null,
            DiscountSum: null
        },

        Radios: {
            CalculateDiscountViaPercentFalse: null,
            CalculateDiscountViaPercentTrue: null
        },

        Divs: {
            DiscountSumOuter: null,
            DiscountPercentOuter: null
        },

        Lookups: {
            PricePosition: null
        },
        
        Toolbar: null
    },

    constructor: function ()
    {
        this.addEvents("pricePositionChanged");
        this.addEvents("recalculateDiscountCompleted");
    },

    initialize: function (isLocked)
    {
        var self = this;
        self.LocalData.IsLocked = isLocked;

        this.UI.Texts.DiscountPercent.dom.value = this.UI.Hiddens.DiscountPercent.dom.value;
        this.UI.Texts.DiscountSum.dom.value = this.UI.Hiddens.DiscountSum.dom.value;

        this.registerTextFieldChangeHandler(this.UI.Texts.DiscountSum, function (value) { return !isNaN(self.parseFloatLocalized(value)); }, self.recalculateDiscount);

        this.registerTextFieldChangeHandler(this.UI.Texts.DiscountPercent, function (value) { return !isNaN(self.parseFloatLocalized(value)); }, self.recalculateDiscount);

        this.registerTextFieldChangeHandler(
            this.UI.Texts.Amount,
            function (value) { return !isNaN(parseInt(value)); },
            function (afterrecalc)
            {
                self.LocalData.AmountWasEntered = true;
                self.recalculateAll(null);
                self.recalculateDiscount();
                if (afterrecalc) afterrecalc();
            }
        );

        this.registerTextFieldChangeHandler(this.UI.Texts.PricePerUnitWithVat,
            function (value) { return !isNaN(parseInt(value)); },
            function (afterrecalc)
            {
                self.recalculateAll(null);
                self.recalculateDiscount();
                if (afterrecalc) afterrecalc();
            }
        );

        this.UI.Radios.CalculateDiscountViaPercentFalse.on('change', function ()
        {
            self.setDiscountMode();
        });

        this.UI.Radios.CalculateDiscountViaPercentTrue.on('change', function ()
        {
            self.setDiscountMode();
        });

        this.UI.Radios.CalculateDiscountViaPercentFalse.on('click', function ()
        {
            self.setDiscountMode();
        });

        this.UI.Radios.CalculateDiscountViaPercentTrue.on('click', function ()
        {
            self.setDiscountMode();
        });

        if (this.UI.Lookups.PricePosition.getValue())
        {
            self.acquireData(self.UI.Lookups.PricePosition.getValue().id);
        }

        this.UI.Lookups.PricePosition.on("change", function (lookup, newValue)
        {
            if (newValue)
            {
                self.acquireData(newValue.id);
            }
        });

        this.setDiscountMode();
    },
    
    registerTextFieldChangeHandler: function (textField, validator, changeHandler) {
        var timerId = null;
        var self = this;
        textField.on('keyup', function (event) {
            // dont handle the left arrow and right arrow keys
            if (event.keyCode == 37 || event.keyCode == 39) {
                return;
            }
            // dont handle anything, if field can not be edited
            if (textField.dom.readOnly) {
                return;
            }
            if (validator(textField.dom.value)) {
                // handling the event if text field value not ends with '.' or ',' char
                var regexp = new RegExp('[\.|\,]$', 'g');
                if (!regexp.test(textField.dom.value)) {
                    
                    if (timerId != null)
                        clearTimeout(timerId);
                    
                    self.UI.Toolbar.disable();
                    timerId = setTimeout(function () {
                        changeHandler.call(self, function () { self.UI.Toolbar.enable(); });
                        timerId = null;
                    }, self.Settings.DiscountRecalcDelay);
                }
            }
        });
        textField.on('blur', function ()
        {
            if (textField.dom.value == '')
            {
                textField.dom.value = '0';
                changeHandler.call(self);
            }
        });
    },

    registerDomElements: function (hiddens, textFields, radios, divs, lookups, settings)
    {
        var self = this;

        this.UI.Texts.PricePerUnit = textFields.PricePerUnit;
        this.UI.Texts.PricePerUnitWithVat = textFields.PricePerUnitWithVat;
        this.UI.Texts.Amount = textFields.Amount;
        this.UI.Texts.Platform = textFields.Platform;
        this.UI.Texts.ShipmentPlan = textFields.ShipmentPlan;
        this.UI.Texts.PayablePrice = textFields.PayablePrice;
        this.UI.Texts.PayablePlan = textFields.PayablePlan;
        this.UI.Hiddens.DiscountPercent = hiddens.DiscountPercent;
        this.UI.Hiddens.DiscountSum = hiddens.DiscountSum;
        this.UI.Texts.PayablePlan = textFields.PayablePlan;
        this.UI.Texts.DiscountPercent = textFields.DiscountPercent;
        this.UI.Texts.DiscountSum = textFields.DiscountSum;
        this.UI.Radios.CalculateDiscountViaPercentFalse = radios.CalculateDiscountViaPercentFalse;
        this.UI.Radios.CalculateDiscountViaPercentTrue = radios.CalculateDiscountViaPercentTrue;
        this.UI.Divs.DiscountPercentOuter = divs.DiscountPercentOuter;
        this.UI.Divs.DiscountSumOuter = divs.DiscountSumOuter;
        this.UI.Lookups.PricePosition = lookups.PricePosition;
        this.Settings.DecimalDigits = settings.DecimalDigits;

        for (var item in this.UI.Texts)
        {
            this.UI.Texts[item].setValueAdv = function (value)
            {
                if (!self.LocalData.IsLocked)
                    this.dom.value = value;
            };
        }
    },

    registerToolbar: function(toolbar) {
        this.UI.Toolbar = toolbar;
    },

    setLocalData: function (data)
    {
        window.Ext.apply(this.LocalData, data);
    },

    acquireData: function (pricePositionId)
    {
        if (this.LocalData.AmountWasEntered)
        {
            this.LocalData.IgnoreNextAmountChange = true;
        }

        this.UI.Divs.DiscountSumOuter.dom.disabled = 'disabled';
        this.UI.Divs.DiscountPercentOuter.dom.disabled = 'disabled';

        this.setReadonly(this.UI.Texts.PricePerUnitWithVat, true);
        this.setReadonly(this.UI.Texts.Amount, true);

        var requestParams = {
            orderPositionId: this.LocalData.Id,
            pricePositionId: pricePositionId,
            orderId: this.LocalData.OrderId,
            includeHidden: false
        };

        var url = '/OrderPosition/GetEditValues';
        url = window.Ext.urlAppend(url, window.Ext.urlEncode(requestParams));

        var self = this;

        window.Ext.Ajax.request({
            url: url,
            timeout: 1200000,
            success: function (response, opts) {
                var responseData = window.Ext.decode(response.responseText);
                self.ServerData.IsPositionOfPlannedProvisionSalesModel = responseData.IsPositionOfPlannedProvisionSalesModel;
                self.ServerData.IsPositionCategoryBound = responseData.IsPositionCategoryBound;
                self.recalculateAll(responseData);
                self.recalculateDiscount();
                self.fireEvent("pricePositionChanged", self.ServerData.LinkingObjectsSchema, responseData.IsPositionOfPlannedProvisionSalesModel, responseData.SalesModel);
            },
            failure: function (response)
            {
                if (response.status == 500 && response.responseText && response.responseText.length > 0)
                {
                    Ext.Msg.show({
                        title: Ext.LocalizedResources.Error,
                        msg: Ext.LocalizedResources.ErrorRetrievingDataFromServer + ': \n' + response.responseText,
                        buttons: Ext.Msg.OK,
                        icon: Ext.MessageBox.ERROR
                    });
                } else
                {
                    alert(Ext.LocalizedResources.ErrorRetrievingDataFromServer);
                }
            }
        });
    },

    setDiscountMode: function ()
    {
            var value = this.UI.Radios.CalculateDiscountViaPercentFalse.dom.checked;
            this.UI.Texts.DiscountPercent.dom.disabled = value;
            this.UI.Texts.DiscountSum.dom.disabled = !value;
    },

    recalculateDiscount: function (afterrecalc)
    {
        // todo: post on server non-localized decimals, this is real shit

        var self = this;

        var inPercent = this.UI.Radios.CalculateDiscountViaPercentTrue.dom.checked;
        var discountPercent = this.formatDiscountPercentLocalized(this.parseFloatLocalized(this.UI.Texts.DiscountPercent.dom.value));
        var discountSum = this.formatLocalized(this.parseFloatLocalized(this.UI.Texts.DiscountSum.dom.value));
        
        window.Ext.Ajax.request({
            timeout: 1200000,
            method: 'POST',
            url: '/OrderPosition/DiscountRecalc',
            params:
            {
                pricePerUnitWithVat: this.formatLocalized(this.parseFloatLocalized(this.UI.Texts.PricePerUnitWithVat.dom.value)),
                amount: parseInt(this.UI.Texts.Amount.dom.value),
                orderReleaseCountPlan: parseInt(this.ServerData.OrderReleaseCountPlan),
                inPercent: inPercent,
                discountPercent: discountPercent,
                discountSum: discountSum
            },
            success: function (response, opts)
            {
                var jsonResponse = window.Ext.decode(response.responseText);

                // В jsonResponse суммы приходят с десятичным разделителем "."
                var correctedDiscountSum = self.formatLocalized(jsonResponse.CorrectedDiscountSum);
                var correctedDiscountPercent = self.formatDiscountPercentLocalized(jsonResponse.CorrectedDiscountPercent);

                self.UI.Texts.DiscountSum.setValueAdv(correctedDiscountSum);
                self.UI.Texts.DiscountPercent.setValueAdv(correctedDiscountPercent);

                var payablePlan = self.ComputationalData.PayablePricePriorDiscount - jsonResponse.CorrectedDiscountSum;
                self.UI.Texts.PayablePlan.setValueAdv(self.formatLocalized(payablePlan));

                if (afterrecalc) afterrecalc();
                self.fireEvent('recalculateDiscountCompleted');
            },
            failure: function (response) {
                if (afterrecalc) afterrecalc();
                self.fireEvent('recalculateDiscountCompleted');
                
                if (response.status == 500 && response.responseText && response.responseText.length > 0)
                {
                    alert(Ext.LocalizedResources.ErrorRetrievingDataFromServer + ': \n' + response.responseText);
                } else
                {
                    alert(Ext.LocalizedResources.ErrorRetrievingDataFromServer);
                }
            }
        });
    },

    setValue: function (element, value)
    {
        if (!this.LocalData.IsLocked)
        {
            element.dom.value = value;
        }
    },

    recalculateAll: function (data)
    {
        if (data) {
            window.Ext.apply(this.ServerData, data);
            document.getElementById('IsComposite').value = this.ServerData.IsPositionComposite;
            this.UI.Texts.Platform.dom.value = this.ServerData.PlatformName;

            if (this.ServerData.AmountSpecificationMode == this.Constants.AmountSpecificationMode.FixedValue) {
                this.UI.Texts.Amount.dom.value = this.ServerData.PricePositionAmount;
            }
            else if (this.ServerData.AmountSpecificationMode == this.Constants.AmountSpecificationMode.Counting &&
                this.ServerData.LinkingObjectType == this.Constants.LinkingObjectType.Firm) {
                this.UI.Texts.Amount.dom.value = this.Constants.FixedLinkedFirmCount;
            }

            this.UI.Divs.DiscountSumOuter.dom.disabled = false;
            this.UI.Divs.DiscountPercentOuter.dom.disabled = false;

            this.UI.Texts.PricePerUnitWithVat.setValueAdv(this.formatLocalized(this.ServerData.PricePerUnit * (1 + this.ServerData.VatRatio)));
            this.ComputationalData.PricePerUnit = this.ServerData.PricePerUnit;

            this.setupAmountFieldAvailability();
        }

        var amount = parseInt(this.UI.Texts.Amount.dom.value);
        pricePerUnitWithVat = this.parseFloatLocalized(this.UI.Texts.PricePerUnitWithVat.dom.value);

        this.ComputationalData.PayablePricePriorDiscount = pricePerUnitWithVat * amount * this.ServerData.OrderReleaseCountPlan;
        this.ComputationalData.ShipmentPlan = amount * this.ServerData.OrderReleaseCountPlan;

        this.ComputationalData.PayablePrice = amount * this.ServerData.OrderReleaseCountPlan * this.ComputationalData.PricePerUnit;

        this.UI.Texts.ShipmentPlan.setValueAdv(this.ComputationalData.ShipmentPlan);
        this.UI.Texts.PayablePrice.setValueAdv(this.formatLocalized(this.ComputationalData.PayablePrice));
        this.UI.Texts.PricePerUnit.setValueAdv(this.formatLocalized(this.ComputationalData.PricePerUnit));

        this.setDiscountMode();
    },

    round: function (number)
    {
        return Number(number.toFixed(this.Settings.DecimalDigits));
    },
    
    toFixedWithoutRounding: function (figure, decimals) {
        if (!decimals) decimals = 2;
        var d = Math.pow(10, decimals);
        return (parseInt(figure * d) / d).toFixed(decimals);
    },

    roundDiscountPercent: function (number) {
        return Number(this.toFixedWithoutRounding(number, this.Settings.DiscountPercentDecimalDigits));
    },

    formatLocalized: function (number)
    {
        return Number.formatToLocal(this.round(number));
    },
    
    formatDiscountPercentLocalized: function (number) {
        return Number.formatToLocal(this.roundDiscountPercent(number));
    },

    parseFloatLocalized: Number.parseFromLocal,
    setReadonly: function (field, readonly)
    {
        if (!readonly)
        {
            if (this.LocalData.IsLocked)
            {
                readonly = true;
            }
        }
        field.setReadOnly(readonly);
    },

    prepareToSave: function ()
    {
        this.UI.Hiddens.DiscountSum.dom.value = this.UI.Texts.DiscountSum.dom.value;
        this.UI.Hiddens.DiscountPercent.dom.value = this.UI.Texts.DiscountPercent.dom.value;
    },

    onSelectedAdvertisementCountChanged: function (args)
    {
        if (this.ServerData.IsPositionCategoryBound) {
            this.acquirePricesForCategory(args.categoryIds);
        }

        if (this.ServerData.IsPositionComposite) {
            return;
        }

        this.LocalData.SelectedAdvertisementCount = args.selectedCount;

        switch (this.ServerData.AmountSpecificationMode) {
            case this.Constants.AmountSpecificationMode.Counting:
                this.UI.Texts.Amount.dom.value = args.selectedCount.toString();
                if (this.UI.Texts.Amount.dom.value == '0') {
                    // ERM-913 до 0 кол-во не должно опускаться
                    this.UI.Texts.Amount.dom.value = '1';
                }
                this.recalculateAll();
                this.recalculateDiscount();
                break;
            case this.Constants.AmountSpecificationMode.FixedValue:
                args.isLimitReached = this.ServerData.PricePositionAmount == args.selectedCount;
                break;
        }
    },

    acquirePricesForCategory: function (categoryIds) {
        var requestParams = {
            categoryIds: Ext.encode(categoryIds),
            orderId: this.LocalData.OrderId,
            pricePositionId: this.UI.Lookups.PricePosition.getValue().id
        };

        window.Ext.Ajax.request({
            url: '/OrderPosition/GetRatedPrices',
            timeout: 1200000,
            scope: this,
            params: requestParams,
            method: "GET",
            success: function (response, opts) {
                var responseData = window.Ext.decode(response.responseText);
                this.recalculateAll(responseData);
                this.recalculateDiscount();
            },
            failure: function (response) {
                if (response.status == 500 && response.responseText && response.responseText.length > 0) {
                    Ext.Msg.show({
                        title: Ext.LocalizedResources.Error,
                        msg: Ext.LocalizedResources.ErrorRetrievingDataFromServer + ': \n' + response.responseText,
                        buttons: Ext.Msg.OK,
                        icon: Ext.MessageBox.ERROR
                    });
                } else {
                    alert(Ext.LocalizedResources.ErrorRetrievingDataFromServer);
                }
            }
        });
    },

    setupAmountFieldAvailability: function ()
    {
        var readonly = this.ServerData.AmountSpecificationMode != this.Constants.AmountSpecificationMode.ArbitraryValue;
        this.setReadonly(this.UI.Texts.Amount, readonly);
    }
});
