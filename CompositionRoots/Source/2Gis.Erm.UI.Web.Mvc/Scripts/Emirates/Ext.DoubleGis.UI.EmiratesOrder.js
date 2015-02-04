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
        var bargain = window.Ext.getCmp('Bargain').getValue();

        if (!bargain) {
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

        this.clearBargain();
        this.tryDetermineBargain();
    }
}
