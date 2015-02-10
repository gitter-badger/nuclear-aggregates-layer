window.Ext.DoubleGis.CustomValidatorRegistry["validateAmount"] = function (value, context)
{
    var amount = parseInt(value);
    return (amount > 0);
};

window.Ext.DoubleGis.CustomValidatorRegistry["validateDiscountSum"] = function (value, context)
{
    if (parseInt(window.Ext.getDom('Amount').value) > 0)
    {
        var discountSum = Number.parseFromLocal(value);
        return (discountSum >= 0);
    }
    return true;
};

window.Ext.DoubleGis.CustomValidatorRegistry["validateDiscountPercent"] = function (value, context)
{
    if (parseInt(window.Ext.getDom('Amount').value) > 0)
    {
        var discountPercent = Number.parseFromLocal(value);
        return (discountPercent >= 0 && discountPercent <= 100);
    }
    return true;
};

window.Ext.DoubleGis.CustomValidatorRegistry["validatePayablePlan"] = function (value, context)
{
    return (Number.parseFromLocal(value) >= 0);
};

window.Ext.DoubleGis.CustomValidatorRegistry["validatePricePerUnitWithVat"] = function (value, context) {
    var number = Number.parseFromLocal(value);
    return (number >= 0);
};

// FIXME {y.baranihin, 16.06.2014}: Давай все же будем использовать ООП. Пусть будет некий объект в этом scope, в не глобальная функция
function RegisterBusinessLogicDomElements(businessLogic, pricePositionLookup) {
    businessLogic.registerDomElements({
        AdvertisementsJson: window.Ext.get('AdvertisementsJson'),
        DiscountPercent: window.Ext.get('DiscountPercent'),
        DiscountSum: window.Ext.get('DiscountSum')
    }, {
        Amount: window.Ext.get('Amount'),
        Platform: window.Ext.get('Platform'),
        ShipmentPlan: window.Ext.get('ShipmentPlan'),
        SnapObjectType: window.Ext.get('SnapObjectType'),
        PricePerUnit: window.Ext.get('PricePerUnit'),
        PricePerUnitWithVat: window.Ext.get('PricePerUnitWithVat'),
        PayablePrice: window.Ext.get('PayablePrice'),
        PayablePlan: window.Ext.get('PayablePlan'),
        DiscountPercent: window.Ext.get('DiscountPercentText'),
        DiscountSum: window.Ext.get('DiscountSumText')
    }, {
        CalculateDiscountViaPercentFalse: window.Ext.get('CalculateDiscountViaPercentFalse'),
        CalculateDiscountViaPercentTrue: window.Ext.get('CalculateDiscountViaPercentTrue')
    }, {
        DiscountSumOuter: window.Ext.get('discountSumOuter'),
        DiscountPercentOuter: window.Ext.get('discountPercentOuter')
    }, {
        PricePosition: pricePositionLookup
    },
    {
        DecimalDigits: window.Ext.get('MoneySignificantDigitsNumber').getValue()
    });
}

window.InitPage = function ()
{
    this.ChangeBindingObjects = function() {
        // Смена позиций заказа с точки зрения клиентского js:
        // 1. Показать диалог с моделью позиции и объектом смены позиций заказа.
        // 2. Получить из этого диалога объект смены позиций заказа.
        // 3. Заслать этот объект на сервер.
        // 4. Обновить окно редактирования позиции заказа.

        var parametrs = 'dialogWidth:800px; dialogHeight:600px; status:yes; scroll:yes; resizable:yes; ';
        var url = '/MultiCulture/ChangeBindingObjects/ChangeBindingObjects';
        url = window.Ext.urlAppend(url, window.Ext.urlEncode({ positionId: this.form.Id.value }));
        var dialogResult = window.showModalDialog(url, null, parametrs);
        dialogResult = window.Ext.decode(dialogResult); // тип результата не должен быть ссылочным, иначе после закрытия диалога с ним нельзя будет работать.

        if (!dialogResult)
            return;
        
        this.Mask.show();
        window.Ext.Ajax.request({
            url: url,
            jsonData: dialogResult,
            success: function () { this.refresh(true); },
            failure: this.postFormFailure,
            scope: this
        });
    };

    this.on("afterbuild", function (cardObject)
    {
        var pricePositionLookup = Ext.getCmp('PricePosition');

        // Если указана позиция прайса, значит позиция заказа уже создана - показываем маску,
        // если позиция заказа создается - маску показывать не нужно
        if (pricePositionLookup.getValue()) {
            this.Mask.show();
        }
        
        this.BusinessLogic = new window.Ext.DoubleGis.UI.OrderPosition.BusinessLogic();

        RegisterBusinessLogicDomElements(this.BusinessLogic, pricePositionLookup);

        this.BusinessLogic.registerToolbar(this.Items.Toolbar);

        var orderId = window.Ext.getDom("OrderId").value;
        if (!orderId)
        {
            orderId = window.ExturlDecode(window.ExturlDecode(location.search.substring(1)).parentInfo).OrderId;
        }

        this.BusinessLogic.setLocalData({
            Id: window.Ext.getDom('Id').value,
            OrderId: orderId
        });

        this.Advertisements = new window.Ext.DoubleGis.UI.OrderPosition.Advertisements();
        this.Advertisements.registerControls({
            jsonHidden: window.Ext.get('AdvertisementsJson'),
            targetDiv: window.Ext.get('linkingObjectContainer')
        });
        this.Advertisements.setLocalData({
            firmId: window.Ext.getDom('OrderFirmId').value,
            readOnly: this.ReadOnly,
            areLinkingObjectParametersLocked: this.form.IsLocked.value.toLowerCase() == true.toString().toLowerCase(),
            organizationUnitId: window.Ext.getDom('OrganizationUnitId').value
        });

        this.BusinessLogic.on("pricePositionChanged", function (linkingObjectsShema, isPositionOfPlannedProvisionSalesModel, salesModel) {
            this.Advertisements.localData.useSingleCategoryForPackage = isPositionOfPlannedProvisionSalesModel;
            this.Advertisements.localData.salesModel = salesModel;
            this.Advertisements.setSchema(linkingObjectsShema);
        }, this);

        // Закончены все пересчеты после открытия карточки позиции заказа - убираем маску
        this.BusinessLogic.on("recalculateDiscountCompleted", function () {
            this.Mask.hide();
        }, this);
        
        this.BusinessLogic.initialize(this.form.IsLocked.value.toLowerCase() == true.toString().toLowerCase());
        this.Advertisements.on("selectedCountChanged", function (args)
        {
            if (this.form.IsLocked.value.toLowerCase() != true.toString().toLowerCase())
            {
                this.BusinessLogic.onSelectedAdvertisementCountChanged(args);
            }
        }, this);
        if (this.ReadOnly)
        {
            Ext.getCmp('PricePosition').disable();
        }
    }, this);

    this.on('beforepost', function ()
    {
        var report = this.Advertisements.validate();
        var errorId = 'div-advertisement-error';

        this.RemoveNotification(errorId);
        if (report.length > 0)
        {
            this.AddNotification(report[0].Message, report[0].Level, errorId);
            return false;
        }

        this.BusinessLogic.prepareToSave();
        this.Advertisements.prepareToSave();
    }, this);

    this.on('afterpost', function () {
        var self = this;
        setTimeout(function () { self.BusinessLogic.setupAmountFieldAvailability(); });
    });
}



