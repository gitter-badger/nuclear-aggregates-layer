var CultureSpecificOrder = {
    PrintOrder: function () {
        this.Print('PrintOrder');
    },
    PrintRegionalOrder: function () {
        this.Print('PrintRegionalOrder');
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
    PrintRegionalTerminationNotice: function () {
        this.Print('PrintRegionalTerminationNotice');
    },
    PrintAdditionalAgreement: function () {
        this.Print('PrintAdditionalAgreement');
    },
    PrintOrderBargainAdditionalAgreement: function () {
        this.Print('PrintOrderBargainAdditionalAgreement');
    },
    PrintReferenceInformation: function () {
        this.Print('PrintReferenceInformation');
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

        Ext.get("RegionalNumber").setReadOnly(!Ext.getDom('EditRegionalNumber').checked);
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
