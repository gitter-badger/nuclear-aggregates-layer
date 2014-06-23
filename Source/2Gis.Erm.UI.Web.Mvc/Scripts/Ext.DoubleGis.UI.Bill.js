window.InitPage = function () {
    Ext.apply(this, PrintLogic);

    Ext.apply(this,
    {
        PrintBill: function () {
            var entityId = {
                billId: Ext.getDom('Id').value
            };
            var callback = function(profileId) {
                this.PrintWithoutProfileChoosing('PrintSingleBill', entityId.billId, profileId);
            };

            this.ChooseProfile(entityId, callback);
        }
    });
}