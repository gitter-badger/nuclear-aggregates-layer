Ext.onReady(function () {
    // Вторая фаза загрузки формы - после получения объектов привязки от сервера.
    var continueLoad = function (response) {
        var linkingObjectsSchema = Ext.decode(response.responseText).LinkingObjectsSchema;

        adv.registerControls({
            jsonHidden: window.Ext.get('AdvertisementsJson'),
            targetDiv: window.Ext.get('linkingObjectContainer')
        });
        adv.setLocalData({
            firmId: window.Ext.getDom('OrderFirmId').value,
            areLinkingObjectParametersLocked: this.EntityForm.IsLocked.value.toLowerCase() == true.toString().toLowerCase(),
            organizationUnitId: window.Ext.getDom('OrganizationUnitId').value
        });
        adv.on('selectedCountChanged', function (args) {
            args.isLimitReached = args.selectedCount >= parseInt(window.EntityForm.Amount.value);
        }, this);
        adv.setSchema(linkingObjectsSchema);

    };

    var reportFailure = function () {
        window.alert('Произошла ошибка, обратитесь к разработчикам'); // todo: сделать как везде
    };
    
    var adv = new window.Ext.DoubleGis.UI.OrderPosition.Advertisements('changeLinkedObjects');
    
    Ext.get("Cancel").on("click", function () {
        window.returnValue = null;
        window.close();
    });
    
    Ext.get("OK").on("click", function () {
        if (adv.localData.lastSelectedCount != parseInt(window.EntityForm.Amount.value)) {
            alert(String.format(Ext.LocalizedResources.InvalidSelectedPositionsCount, window.EntityForm.Amount.value));
            return;
        }

        if (adv.hasInvalidCheckedObjects()) {
            alert(Ext.LocalizedResources.CanNotSelectHiddenPosition);
            return;
        }

        adv.prepareToSave();
        window.returnValue = window.Ext.get('AdvertisementsJson').dom.value;
        window.close();
    });
    
    // Ниже только запрос объектов привязки с сервера.
    var params = {
        pricePositionId: Ext.decode(this.EntityForm.PricePosition.value).Key,
        orderPositionId: window.EntityForm.Id.value,
        orderId: window.EntityForm.OrderId.value,
        includeHidden: true
    };

    var url = '/OrderPosition/GetEditValues';
    url = window.Ext.urlAppend(url, window.Ext.urlEncode(params));

    window.Ext.Ajax.request({
        url: url,
        success: continueLoad,
        failure: reportFailure
    });
});
