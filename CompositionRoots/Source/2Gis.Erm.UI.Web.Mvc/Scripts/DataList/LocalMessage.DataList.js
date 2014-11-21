window.InitPage = function () {
    if (this.isSearchForm)
        return;

    // hidden form, used to "save as" messages
    window.Ext.getBody().createChild({ tag: "form", id: "SaveMessageForm", method: "post", action: "/LocalMessage/SaveAs" });

    // create custom option groups for better visualization
    this.on("afterbuild", function () {

        var availableViews = Ext.getDom("availableViews");

        var inboxOptionGroup = document.createElement("optgroup");
        inboxOptionGroup.label = Ext.LocalizedResources.IncomingMessages;
        availableViews.appendChild(inboxOptionGroup);
        var inboxOptions = [];

        var outboxOptionGroup = document.createElement("optgroup");
        outboxOptionGroup.label = Ext.LocalizedResources.OutgoingMessages;
        availableViews.appendChild(outboxOptionGroup);
        var outboxOptions = [];

        var i;
        for (i = 0; i < availableViews.options.length; i++) {

            var option = availableViews.options[i];

            // 1-3 - "входящие сообщения"
            if (i >= 1 && i <= 3)
                inboxOptions.push(option);
                // 4-6 - "исходящие сообщения"
            else if (i >= 4 && i <= 6)
                outboxOptions.push(option);
        }

        for (i = 0; i < inboxOptions.length; i++)
            inboxOptionGroup.appendChild(inboxOptions[i]);

        for (i = 0; i < outboxOptions.length; i++)
            outboxOptionGroup.appendChild(outboxOptions[i]);
    });

    this.on("beforebuild", function () {
        Ext.apply(this, {
            ImportFromFile: function () {
                var url = "/LocalMessage/ImportFromFile/";
                var params = "dialogWidth: 500px; dialogHeight: 220px; status:yes; scroll:no;resizable:no;";

                window.showModalDialog(url, null, params);
                this.refresh();
            },
            Export: function () {
                var url = "/LocalMessage/Export/";
                var params = "dialogWidth: 500px; dialogHeight: 300px; status:yes; scroll:no;resizable:no;";

                window.showModalDialog(url, null, params);
                this.refresh();
            },
            SaveAs: function () {
                var selectedItems = this.Items.Grid.getSelectionModel().selections.items;
                var saveMessageForm = document.forms['SaveMessageForm'];

                var selectedItemsLength = selectedItems.length;

                if (selectedItemsLength == 0) {
                    alert(Ext.LocalizedResources.SelectMessageToSave);
                    return;
                }

                for (var i = 0; i < selectedItemsLength; i++) {

                    var id = selectedItems[i].data.Id;

                    var element = document.createElement("input");
                    element.setAttribute("name", "Ids");
                    element.setAttribute("type", "hidden");
                    element.setAttribute("value", id);

                    saveMessageForm.appendChild(element);
                }

                saveMessageForm.submit();

                while (saveMessageForm.hasChildNodes()) {
                    saveMessageForm.removeChild(saveMessageForm.firstChild);
                }
                this.ContentContainer.doLayout();
            },
            ProcessMessage: function () {
                if (this.Items.Grid.getSelectionModel().selections.items.length == 0) {
                    alert(Ext.LocalizedResources.SelectObjectsToProcess);
                    return;
                }

                // sort messages before processing
                var ids = [];

                var selectedRows = this.Items.Grid.getSelectionModel().selections.items;
                for (var i = 0; i < selectedRows.length; i++) {
                    var selectedRow = selectedRows[i];
                    ids.push(selectedRow.data.Id);
                }

                ids.sort();
                ids.reverse();

                var url = "/LocalMessage/ProcessMessages";
                var params = "dialogWidth: 500px; dialogHeight: 200px; status:yes; scroll:no; resizable:no;";

                window.showModalDialog(url, ids, params);
                this.refresh();
            }

        });
    });
};
