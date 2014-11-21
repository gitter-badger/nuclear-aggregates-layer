window.InitPage = function () {
    this.on("beforebuild", function (cardObject) {
        Ext.apply(this,
        {
            SetAsPrimary: function () {
                var params = "dialogHeight:210px; dialogWidth:500px; status:yes; scroll:no; resizable:no; ";
                var sUrl = "/BranchOfficeOrganizationUnit/SetAsPrimary";
                window.showModalDialog(sUrl, [Ext.getDom("Id").value], params);
                this.refresh(true);
            },
            SetAsPrimaryForRegSales: function () {
                var params = "dialogHeight:210px; dialogWidth:500px; status:yes; scroll:no; resizable:no; ";
                var sUrl = "/BranchOfficeOrganizationUnit/SetAsPrimaryForRegSales";
                window.showModalDialog(sUrl, [Ext.getDom("Id").value], params);
                this.refresh(true);
            }
        });
    });
};
