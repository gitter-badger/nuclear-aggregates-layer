﻿
window.InitPage = function () {
    if (this.isSearchForm)
        return;

    this.on("beforebuild", function () {
        Ext.apply(this, {
            EditActivity: function () {
                if (this.Items.Grid.getSelectionModel().selections.items.length == 0) return;
                var selectedActivityType = this.Items.Grid.getSelectionModel().selections.items[0].data.Type;
                this.Edit({ overridenEntityName : selectedActivityType });
            },
            CreateTask: function () {
                this.Create({overridenEntityName : "Task"});
            },
            CreatePhonecall: function () {
                this.Create({ overridenEntityName: "Phonecall" });
            },
            CreateAppointment: function () {
                this.Create({ overridenEntityName: "Appointment" });
            },
            renderImage: function (id, col, record) {
                var iconFileName;
                switch (record.data.Type) {
                    case "Appointment":
                        iconFileName = "en_ico_16_Appointment.gif";
                        break;
                    case "Phonecall":
                        iconFileName = "en_ico_16_Phonecall.gif";
                        break;
                    case "Task":
                        iconFileName = "en_ico_16_Task.gif";
                        break;
                }
                return Ext.DoubleGis.Global.Helpers.GridColumnHelper.RenderEntityIcon(iconFileName);
            },
            Assign: function () {
                this.ShowDialogWindowForOneOrMoreEntities("/GroupOperation/Assign/" + this.EntityName, "dialogHeight:300px; dialogWidth:650px; status:yes; scroll:no; resizable:no; ");
            }
        });
    });
    
    this.on("afterbuild", function () {
        this.Items.Grid.addListener("RowDblClick", this.EditActivity, this);
        this.Items.Grid.colModel.getColumnById("Image").renderer = this.renderImage;
    });
    this.on("afterrebuild", function () {
        this.Items.Grid.addListener("RowDblClick", this.EditActivity, this);
        this.Items.Grid.colModel.getColumnById("Image").renderer = this.renderImage;
    });
};
