var CultureSpecificOrder = {

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

    setupCultureSpecificEventListeners: function () {
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

                // Если не из сделки, то очищаю поля
                if (!this.form.DealId.value) {
                    Ext.getCmp('Bargain').clearValue();
                    Ext.getCmp('LegalPerson').clearValue();
                }
            }
        }
    },

    // Определение Юр.лица клиента/Города назначения в зависимости от фирмы
    onFirmChanged: function (cmp) {
        var legalPersonLookup = Ext.getCmp('LegalPerson');
        var clientElement = Ext.get('ClientId');
        var oldClientId = clientElement.dom.value;

        if (!Ext.getDom('DealId').value) {
            clientElement.setValue('');
            legalPersonLookup.showReadOnlyCard = true;
        }

        if (cmp.item) {
            if (cmp.item.data) {
                var firmClientId = cmp.item.data.ClientId;

                // Если указана сделка, то смена фирмы не аффектит на юр. лицо. Смена юр лица только при смене клиента фирмы
                if (!Ext.getDom('DealId').value && (oldClientId != firmClientId)) {
                    clientElement.setValue(firmClientId);
                    legalPersonLookup.clearValue();
                    legalPersonLookup.showReadOnlyCard = false;
                    this.Request({
                        method: 'POST',
                        url: '/Order/GetLegalPerson',
                        params: { firmClientId: firmClientId },
                        success: function (xhr) {
                            var legalPersonInfo = Ext.decode(xhr.responseText);
                            if (legalPersonInfo && legalPersonInfo.Id && legalPersonInfo.Name) {
                                Ext.getCmp('LegalPerson').setValue({ id: legalPersonInfo.Id, name: legalPersonInfo.Name });
                            }
                        },
                        failure: function (xhr) {
                            alert(xhr.responseText);
                        }
                    });
                }
            }

            // Если отделение организации указано, фирмы и так будут находиться в рамках него и смена значения не нужна
            if (!Ext.getCmp('DestinationOrganizationUnit').getValue()) {
                this.Request({
                    method: 'POST',
                    url: '/Order/GetDestinationOrganizationUnit',
                    params: { firmId: cmp.getValue().id },
                    success: function (xhr) {
                        var orgUnitInfo = Ext.decode(xhr.responseText);
                        if (orgUnitInfo) {
                            // Ставим флаг, чтобы в обработчике смены города назначения не обнулилась фирма.
                            this.destinationOrgUnitChangedByFirmChangedEvent = true;
                            Ext.getCmp('DestinationOrganizationUnit').setValue({ id: orgUnitInfo.Id, name: orgUnitInfo.Name });
                            delete (this.destinationOrgUnitChangedByFirmChangedEvent);
                        }
                    },
                    failure: function (xhr) {
                        alert(xhr.responseText);
                    }
                });
            }
        }
    },
    
    onLegalPersonChanged: function() {
        this.updateBargain(true);
    }
};
