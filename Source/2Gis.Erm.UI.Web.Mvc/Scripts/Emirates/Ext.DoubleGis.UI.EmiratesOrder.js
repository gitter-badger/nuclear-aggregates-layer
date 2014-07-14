var CultureSpecificOrder = {
    // FIXME {a.rechkalov, 16.06.2014}: Стоит придерживаться единого стиля - функции с меленькой буквы
    // COMMENT {d.ivanov, 18.06.2014}: Я так понимаю, сейчас принятый нами стиль предполагает, что функции-обработчики события click пишутся с большой буквы.
    //                                 См. EntitySettings.xml и функции типа Assign, Save, SaveAndClose... Надо наносить массовые улучшения, чтобы везде стало одинаково.
    PrintBargainAdditionalAgreement: function () {
        this.Print('PrintBargainAdditionalAgreement');
    },
    PrintOrderBills: function () {
        this.Print('PrintOrderBills');
    },
    refreshBargainButtons: function () {
        // Обновление договора после смены юр.лица клиента в зависимости от (юр.лица клиента & юр. лица отд. организации)
        var legalPerson = window.Ext.getCmp('LegalPerson').getValue();
        var branchOfficeOrganizationUnit = window.Ext.getCmp('BranchOfficeOrganizationUnit').getValue();
        var bargain = window.Ext.getCmp('Bargain').getValue();

        if (bargain || !legalPerson || !branchOfficeOrganizationUnit) {
            this.getMenuItem('Actions', 'CreateBargain').disable();
        }

        if (!bargain) {
            this.getMenuItem('Actions', 'RemoveBargain').disable();
            this.getMenuItem('PrintActions', 'PrintActionsAdditional', 'PrintBargainAction').disable();
            this.getMenuItem('PrintActions', 'PrintActionsAdditional', 'PrintBargainAdditionalAgreementAction').disable();
        }
    },

    setupMenuAvailability: function () {
        var item = this.getMenuItem('Actions', 'SwitchToAccount');
        if (Ext.getDom("CanSwitchToAccount").checked)
            item.enable();
        else {
            item.disable();
        }

        Ext.get("RegionalNumber").setReadOnly(!Ext.getDom('EditRegionalNumber').checked); 
    },

    setupCultureSpecificEventListeners: function () {
        Ext.getCmp("Client").on("change", this.onClientChanged, this);
    },

    // При обновлении клиента (нередактируемое поле, обновление может быть вызвано выбором фирмы) автоматически выбираем юрлицо, если оно единственное.
    onClientChanged: function () {
        var clientLookup = Ext.getCmp('Client');
        var clientId = clientLookup.item ? clientLookup.item.id : null;

        var legalPersonLookup = Ext.getCmp('LegalPerson');
        if (clientId) {
            legalPersonLookup.supressMatchesErrors = true;
            legalPersonLookup.forceGetData({
                limit: 1
            });
        } else {
            legalPersonLookup.clearValue();
        }
    },

    // При выборе фирмы автоматически проставляем клиента (нередактируемое поле) и отделение организации (если не было выбрано ранее)
    onFirmChanged: function (cmp) {
        var firmLookup = Ext.getCmp('Firm');
        var firmId = firmLookup.item ? firmLookup.item.id : null;
        var oldValue;

        var clientLookup = Ext.getCmp('Client');
        if (firmId) {
            clientLookup.supressMatchesErrors = true;
            clientLookup.forceGetData({
                extendedInfo: "FirmId={FirmId}"
            });
        } else {
            clientLookup.clearValue();
        }

        var destinationOrganizationUnitLookup = Ext.getCmp('DestinationOrganizationUnit');
        if (firmId && !destinationOrganizationUnitLookup.item) {
            destinationOrganizationUnitLookup.forceGetData({
                extendedInfo: "FirmId={FirmId}"
            });
        }
    },

    onDestinationOrganizationUnit: function (cmp) {
        this.refreshReleaseDistributionInfo();
        if (cmp.getValue()) {
            this.Request({
                method: 'POST',
                url: '/Order/GetHasDestOrganizationUnitPublishedPrice',
                params: { orderId: this.form.Id.value, orgUnitId: cmp.getValue().id },
                success: function (xhr) {
                    var response = Ext.decode(xhr.responseText);
                    Ext.fly("HasDestOrganizationUnitPublishedPrice").setValue((response && response === true) ? "true" : "false");
                },
                failure: function () {
                    Ext.fly("HasDestOrganizationUnitPublishedPrice").setValue("false");
                }
            });

            // Если смена города назначения вызвана пользователем
            if (this.destinationOrgUnitChangedByFirmChangedEvent != true) {
                // При смене города назначения обнулить фирму, юр. лицо клиента, договор
                if (this.oldDestOrgUnitId && (this.oldDestOrgUnitId != cmp.getValue().id)) {
                    Ext.getCmp('Firm').clearValue();
                }
            }
        }
    },
    
    onLegalPersonChanged: function (cmp) {
        var legalPersonLookup = Ext.getCmp('LegalPerson');
        var legalPersonId = legalPersonLookup.item ? legalPersonLookup.item.id : null;

        if (legalPersonId != null) {
            this.Request({
                method: 'POST',
                url: '/Emirates/LegalPerson/GetPaymentMethod',
                params: {
                    legalPersonId: legalPersonId
                },
                success: function (xhr) {
                    var paymentMethodResponse = Ext.decode(xhr.responseText);
                    if (paymentMethodResponse) {
                        var paymentMethodComboBox = Ext.get('PaymentMethod');
                        paymentMethodComboBox.setValue(paymentMethodResponse.PaymentMethod);
                    }
                },
                scope: this
            });
        }

        this.updateBargain(true);
    }
}
