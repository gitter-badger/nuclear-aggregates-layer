﻿var CultureSpecificOrder = {
    PrintTerminationNoticeWithoutReason: function () {
        this.Print('PrintTerminationNoticeWithoutReason');
    },
    PrintTerminationBargainNotice: function () {
        this.Print('PrintTerminationBargainNotice');
    },
    PrintTerminationBargainNoticeWithoutReason: function () {
        this.Print('PrintTerminationBargainNoticeWithoutReason');
    },
    PrintOrderBargainAdditionalAgreement: function () {
        this.Print('PrintOrderBargainAdditionalAgreement');
    },

    refreshBargainButtons: function () {
        var bargain = window.Ext.getCmp('Bargain').getValue();

        if (!bargain) {
            this.getMenuItem('PrintActions', 'PrintActionsAdditional', 'PrintBargainAction').disable();
            this.getMenuItem('PrintActions', 'PrintActionsAdditional', 'PrintTerminationBargainNoticeAction').disable();
            this.getMenuItem('PrintActions', 'PrintActionsAdditional', 'PrintTerminationBargainNoticeWithoutReasonAction').disable();
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
    
    onLegalPersonChanged: function (cmp) {
        var legalPersonLookup = Ext.getCmp('LegalPerson');
        var legalPersonId = legalPersonLookup.item ? legalPersonLookup.item.id : null;

        if (legalPersonId != null) {
            this.Request({
                method: 'POST',
                url: '/Czech/LegalPerson/GetPaymentMethod',
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
