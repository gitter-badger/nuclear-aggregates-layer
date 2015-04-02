var CultureSpecificOrder = {

    refreshBargainButtons: function () {
        var bargain = window.Ext.getCmp('Bargain').getValue();

        if (!bargain) {
            this.getMenuItem('PrintActions', 'PrintActionsAdditional', 'PrintBargainAction').disable();
            this.getMenuItem('PrintActions', 'PrintActionsAdditional', 'PrintNewSalesModelBargainAction').disable();
        }
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
