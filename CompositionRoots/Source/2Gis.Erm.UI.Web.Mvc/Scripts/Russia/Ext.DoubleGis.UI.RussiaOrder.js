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
    }
};
