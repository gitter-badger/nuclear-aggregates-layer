window.InitPage = function () {
    Ext.apply(this, PrintLogic);

    Ext.apply(this,
    {
        PrintBargain: function () {
            var entityId = {
                bargainId: Ext.getDom('Id').value
            };
            var profileId = this.ChooseProfile(entityId);
            if (profileId) {
                this.PrintWithoutProfileChoosing('PrintBargain', entityId.bargainId, profileId);
            }
        },

        PrintNewSalesModelBargain: function () {
            var entityId = {
                bargainId: Ext.getDom('Id').value
            };
            var profileId = this.ChooseProfile(entityId);
            if (profileId) {
                this.PrintWithoutProfileChoosing('PrintNewSalesModelBargain', entityId.bargainId, profileId);
            }
        },

        PrintBargainProlongationAgreement: function () {
            var entityId = {
                bargainId: Ext.getDom('Id').value
            };
            var profileId = this.ChooseProfile(entityId);
            if (profileId) {
                this.PrintWithoutProfileChoosing('PrintBargainProlongationAgreement', entityId.bargainId, profileId);
            }
        },

        ChooseProfile: function (urlParameters) {
            var url = Ext.urlAppend('/Bargain/SelectLegalPersonProfile', Ext.urlEncode(urlParameters));
            var params = "dialogWidth:600px; dialogHeight:300px; status:yes; scroll:no;resizable:no;";
            var result = window.showModalDialog(url, null, params);
            if (!result) {
                return null;
            }

            return result.legalPersonProfile;
        }
    });

    window.Card.on('beforepost', function () { window.returnValue = true; });

}