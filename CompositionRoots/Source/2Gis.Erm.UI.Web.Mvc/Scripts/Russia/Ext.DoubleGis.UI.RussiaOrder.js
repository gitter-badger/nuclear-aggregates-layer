var CultureSpecificOrder = {

    refreshBargainButtons: function () {
        var bargain = window.Ext.getCmp('Bargain').getValue();

        if (!bargain) {
            this.getMenuItem('PrintActions', 'PrintActionsAdditional', 'PrintBargainAction').disable();
            this.getMenuItem('PrintActions', 'PrintActionsAdditional', 'PrintNewSalesModelBargainAction').disable();
        }
    },

    setupMenuAvailability: function () {
        var item = this.getMenuItem('Actions', 'SwitchToAccount');
        if (Ext.getDom("CanSwitchToAccount").checked)
            item.enable();
        else {
            item.disable();
        }

        var canEditDocumentsDebt = this.form.HasOrderDocumentsDebtChecking.value.toLowerCase() == 'true';
        Ext.getDom("HasDocumentsDebt").disabled = canEditDocumentsDebt ? null : "disabled";
        Ext.getDom("DocumentsComment").disabled = canEditDocumentsDebt ? null : "disabled";
        Ext.get("DocumentsComment").setReadOnly(!canEditDocumentsDebt);
    },
    
    onLegalPersonChanged: function() {
        this.clearBargain();
        this.tryDetermineBargain();
    },

    PrintRussiaCancellationAgreement: function () {
        this.Print('CancellationAgreement', 'Russia');
    },

    PrintRussiaFirmNameChangeAgreement: function () {
        this.Print('FirmNameChangeAgreement', 'Russia');
    },

    PrintRussiaBindingChangeAgreement: function () {
        this.Print('BindingChangeAgreement', 'Russia');
    }
};
