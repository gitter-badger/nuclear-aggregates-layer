
window.InitPage = function () {
    if (this.isSearchForm)
        return;

    this.on("beforebuild", function () {
        Ext.apply(this, {
            EditActivity: function () {
                if (this.Items.Grid.getSelectionModel().selections.items.length == 0) return;
                var selectedActivityType = this.Items.Grid.getSelectionModel().selections.items[0].data.ActivityTypeEnum;
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
            CreateLetter: function () {
                this.Create({ overridenEntityName: "Letter" });
            },
            renderImage: function (id, col, record) {
                var iconFileName;
                switch (record.data.ActivityTypeEnum) {
                    case "Appointment":
                        iconFileName = "en_ico_16_Appointment.gif";
                        break;
                    case "Letter":
                        iconFileName = "en_ico_16_Letter.gif";
                        break;
                    case "Phonecall":
                        iconFileName = "en_ico_16_Phonecall.gif";
                        break;
                    case "Task":
                        iconFileName = "en_ico_16_Task.gif";
                        break;
                    default:
                        iconFileName = "en_ico_16_Default.gif";
                        break;
                }
                return Ext.DoubleGis.Global.Helpers.GridColumnHelper.RenderEntityIcon(iconFileName);
            },
            CancelActivity: function () {
                if (this.EnsureOneOrMoreSelected) {

                    var vals = [];
                    Ext.each(this.Items.Grid.getSelectionModel().selections.items,
						function (val) {
						    vals.push({ entityId: val.data.Id, entityName: val.data.ActivityTypeEnum });
						});

                    var parameters = {
                        Values: vals
                    };

                    var result = window.showModalDialog("/GroupOperation/Cancel/" + this.EntityName, parameters, "dialogHeight:300px; dialogWidth:650px; status:yes; scroll:no; resizable:no;");
                    if (result == true) {
                        this.refresh();
                    }
                }
            },
            Assign: function () {
            	if (this.EnsureOneOrMoreSelected) {

            		var vals = [];
            		Ext.each(this.Items.Grid.getSelectionModel().selections.items,
						function (val) {
							vals.push({ entityId: val.data.Id, entityName: val.data.ActivityTypeEnum });
						});

            		var parameters = {
            			Values: vals
            		};

            		var result = window.showModalDialog("/GroupOperation/Assign/" + this.EntityName, parameters, "dialogHeight:300px; dialogWidth:650px; status:yes; scroll:no; resizable:no;");
            		if (result == true) {
            			this.refresh();
            		}
            	}
            },
            Delete: function (cmp, evt, doSpecialConfirmation) {
            	if (this.EnsureOneOrMoreSelected) {

            		var vals = [];
            		Ext.each(this.Items.Grid.getSelectionModel().selections.items,
						function (val) {
							vals.push({ entityId: val.data.Id, entityName: val.data.ActivityTypeEnum });
						});

            		var parameters = {
            			Values: vals,
            			DoSpecialConfirmation: doSpecialConfirmation
            		};

            		var result = window.showModalDialog("/GroupOperation/Delete/" + this.EntityName, parameters, "dialogWidth:500px; dialogHeight:203px; scroll:no;resizable:no;");
            		if (result == true) {
            			this.refresh();
            		}
				}
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
