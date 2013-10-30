window.InitPage = function () {
    if (this.isSearchForm)
        return;

    this.on("beforebuild", function () {
        Ext.apply(this, {
            Detach: function () {
                if (!this.EnsureOneSelected()) {
                    return;
                }
               
                window.showModalDialog("/GroupOperation/Disqualify/" + this.EntityName, this.GetSelectedItems(), "dialogWidth:650px; dialogHeight:230px; status:yes; scroll:no; resizable:no; ");
                this.refresh();
            },
            Qualify: function () {
                this.ShowDialogWindowForOneOrMoreEntities("/GroupOperation/Qualify/" + this.EntityName, "dialogHeight:430px; dialogWidth:650px; status:yes; scroll:no; resizable:no; ");
            },
            Assign: function () {
                this.ShowDialogWindowForOneOrMoreEntities("/GroupOperation/Assign/" + this.EntityName, "dialogHeight:300px; dialogWidth:650px; status:yes; scroll:no; resizable:no; ");
            },
            
            ChangeTerritory: function () {
                this.ShowDialogWindowForOneOrMoreEntities("/GroupOperation/ChangeTerritory/" + this.EntityName, "dialogWidth:450px; dialogHeight:230px; status:yes; scroll:no; resizable:no; ");
            },
            
            ChangeClient : function() {
                this.ShowDialogWindowForOneOrMoreEntities("/GroupOperation/ChangeClient/" + this.EntityName, "dialogWidth:450px; dialogHeight:230px; status:yes; scroll:no; resizable:no; ");
            }
        });
    });
};
