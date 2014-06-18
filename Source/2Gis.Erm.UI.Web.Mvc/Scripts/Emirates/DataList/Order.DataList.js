window.InitPage = function () {
    if (this.isSearchForm)
        return;

    this.on("beforebuild", function () {

        Ext.apply(this, SharedOrderDataListOperations);

        Ext.apply(this, {
            GenerateAcceptanceReports: function () {
                this.ShowDialogWindow('/Emirates/AcceptanceReport/Generate', 'dialogHeight:280px; dialogWidth:530px; status:yes; scroll:no; resizable:no; ', false);
            }
        });

        this.on("afterbuild", function () {
            this.CustomizeToolbar();
        });

        this.on("afterrebuild", function () {
            this.CustomizeToolbar();
        });
    });
};