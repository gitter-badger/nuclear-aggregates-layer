var CultureSpecificOrder = {

    refreshBargainButtons: function () {
        var bargain = window.Ext.getCmp('Bargain').getValue();

        if (!bargain) {
            this.getMenuItem('PrintActions', 'PrintActionsAdditional', 'PrintBargainAction').disable();
            this.getMenuItem('PrintActions', 'PrintActionsAdditional', 'PrintNewSalesModelBargainAction').disable();
        }
    },

    setupMenuAvailability: function () {
        var item = this.getMenuItem('PrintActions', 'PrintActionsAdditional', 'PrintRegionalOrderAction');
        if (!Ext.getDom("ShowRegionalAttributes").checked) {
            item.disable();
        }

        item = this.getMenuItem('Actions', 'SwitchToAccount');
        if (Ext.getDom("CanSwitchToAccount").checked)
            item.enable();
        else {
            item.disable();
        }

        var canEditDocumentsDebt = this.form.HasOrderDocumentsDebtChecking.value.toLowerCase() == 'true';
        Ext.getDom("HasDocumentsDebt").disabled = canEditDocumentsDebt ? null : "disabled";
        Ext.getDom("DocumentsComment").disabled = canEditDocumentsDebt ? null : "disabled";
        Ext.get("DocumentsComment").setReadOnly(!canEditDocumentsDebt);
        Ext.get("RegionalNumber").setReadOnly(!Ext.getDom('EditRegionalNumber').checked);
    },
    
    onLegalPersonChanged: function() {
        this.clearBargain();
        this.tryDetermineBargain();
    }
};
