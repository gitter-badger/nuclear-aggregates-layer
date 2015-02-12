var CultureSpecificOrder = {
    PrintOrder: function () {
        this.Print('PrintOrder');
    },
    PrintOrderBargain: function () {
        this.Print('PrintOrderBargain');
    },
    PrintBill: function () {
        this.Print('PrintBill');
    },
    PrintTerminationNotice: function () {
        this.Print('PrintTerminationNotice');
    },
    PrintTerminationNoticeWithoutReason: function () {
        this.Print('PrintTerminationNoticeWithoutReason');
    },
    PrintTerminationBargainNotice: function () {
        this.Print('PrintTerminationBargainNotice');
    },
    PrintTerminationBargainNoticeWithoutReason: function () {
        this.Print('PrintTerminationBargainNoticeWithoutReason');
    },
    PrintAdditionalAgreement: function () {
        this.Print('PrintAdditionalAgreement');
    },
    PrintLetterOfGuarantee: function () {
        this.Print('PrintLetterOfGuarantee');
    },
    refreshBargainButtons: function () {
        var bargain = window.Ext.getCmp('Bargain').getValue();

        if (!bargain) {
            this.getMenuItem('PrintActions', 'PrintActionsAdditional', 'PrintBargainAction').disable();
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
                url: '/Chile/LegalPerson/GetPaymentMethod',
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
