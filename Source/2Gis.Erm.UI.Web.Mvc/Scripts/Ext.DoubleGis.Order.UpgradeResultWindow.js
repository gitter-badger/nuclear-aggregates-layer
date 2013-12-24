Ext.ns("Ext.DoubleGis.UI.Order");

Ext.DoubleGis.UI.Order.UpgradeResultWindow = Ext.extend(Ext.util.Observable,
function()
{
    var //Private fields
        self,
        extWindow,
        domObjects = {
            renderTarget: null,
            resultTable: null,
            titleCell: null,
            detailsCell: null
        };

    var//Private methods
        initializeTable = function() {
            var table = document.createElement('table');
            table.className = 'orderCheckReport';
            domObjects.renderTarget.appendChild(table);

            var tableBody = document.createElement('tbody');
            table.appendChild(tableBody);

            var titleRow = document.createElement('tr');
            tableBody.appendChild(titleRow);

            var titleCell = document.createElement('td');
            titleRow.appendChild(titleCell);
            titleCell.className = 'title';

            var detailsRow = document.createElement('tr');
            tableBody.appendChild(detailsRow);

            var details = document.createElement('td');
            detailsRow.appendChild(details);
            details.className = 'details';

            domObjects.resultTable = table;
            domObjects.titleCell = titleCell;
            domObjects.detailsCell = details;
        },
        initializeWindow = function() {
            extWindow = new Ext.Window({
                modal: true,
                layout: 'fit',
                items: [{ contentEl: domObjects.resultTable, autoScroll: true }],
                width: 500,
                height: 300,
                closable: false,
                title: Ext.LocalizedResources.OrderUpgradeResults,
                buttons: [{
                    id: 'closeButton',
                    text: Ext.LocalizedResources.Close,
                    handler: function() {
                        extWindow.hide();
                        self.fireEvent("action", "close");
                    }
                }]
            });
        },
        renderCheckResult = function(messages) {
            messages = messages.slice(); //work with a copy because we'll sort it

            //Sort order: errors go before warnings, messages of the same type are grouped by OrderPosition they belong to
            messages.sort(function(a, b) {
                var aTypeOrder;
                if (a.Type == 'Error')
                    aTypeOrder = 3;
                else if (a.Type == 'Warning')
                    aTypeOrder = 2;
                else if (a.Type == 'Info')
                    aTypeOrder = 1;
                else
                    aTypeOrder = 0;

                var bTypeOrder;
                if (b.Type == 'Error')
                    bTypeOrder = 3;
                else if (b.Type == 'Warning')
                    bTypeOrder = 2;
                else if (b.Type == 'Info')
                    bTypeOrder = 1;
                else
                    bTypeOrder = 0;

                if (a.Type != b.Type)
                    return bTypeOrder - aTypeOrder;

                return a.OrderPositionId - b.OrderPositionId;
            });

            while (domObjects.detailsCell.lastChild) {
                domObjects.detailsCell.removeChild(domObjects.detailsCell.lastChild);
            }

            for (var index = 0; index < messages.length; index++) {
                var row = document.createElement('div');
                domObjects.detailsCell.appendChild(row);

                var imageCell = document.createElement('div');
                row.appendChild(imageCell);
                imageCell.className = 'image';

                var image = document.createElement('img');
                imageCell.appendChild(image);

                var imageSrc;
                if (messages[index].Type == 'Error')
                    imageSrc = Ext.Notification.Icon.CriticalError;
                else if (messages[index].Type == 'Warning')
                    imageSrc = Ext.Notification.Icon.Warning;
                else if (messages[index].Type == 'Info')
                    imageSrc = Ext.Notification.Icon.Info;
                else
                    imageSrc = Ext.Notification.Icon.None;

                image.setAttribute('src', imageSrc);

                var messageCell = document.createElement('div');
                row.appendChild(messageCell);
                messageCell.className = 'message';

                messageCell.appendChild(wrapText(messages[index].MessageText));

                var clearCell = document.createElement('div');
                row.appendChild(clearCell);
                clearCell.className = 'clear';
            }
        };

        wrapText = function(text)
        {
            var result = document.createElement('span');
            result.innerText = text;
            return result;
        };

    return { //Public methods
        constructor: function(config)
        {
            self = this;

            this.addEvents("action");

            domObjects.renderTarget = config.renderTarget;
            initializeTable();
            initializeWindow();
        },

        showResult: function(messages)
        {
            renderCheckResult(messages);
        
            domObjects.titleCell.innerText = Ext.LocalizedResources.OrderUpgradeProducedTheFollowingWarnings;
            domObjects.resultTable.style.visibility = 'visible';
            extWindow.show();
        }
    };
} ());