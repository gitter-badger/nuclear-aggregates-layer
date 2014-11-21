window.InitPage = function () {
    if (this.isSearchForm)
        return;

    this.on("beforebuild", function () {

        Ext.apply(this, SharedOrderDataListOperations);

        this.on("afterbuild", function () {
            this.CustomizeToolbar();
        });

        this.on("afterrebuild", function () {
            this.CustomizeToolbar();
        });
    });
};