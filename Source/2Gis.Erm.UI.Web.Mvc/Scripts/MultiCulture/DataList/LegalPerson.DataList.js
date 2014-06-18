
window.InitPage = function () {
    if (this.isSearchForm)
        return;

    this.on("beforebuild", function () {

        // Удаляем кнопку "Создать" для общего грида
        if (this.ParentType == null) {
            this.currentSettings.ToolbarItems.removeFirstWhere(function(item) { return item.Name === "Create"; });
        }
        
        Ext.apply(this, {
            Assign: function () {
                this.ShowDialogWindowForOneOrMoreEntities("/GroupOperation/Assign/" + this.EntityName, "dialogHeight:300px; dialogWidth:650px; status:yes; scroll:no; resizable:no; ");
            }
        });
    });
};
