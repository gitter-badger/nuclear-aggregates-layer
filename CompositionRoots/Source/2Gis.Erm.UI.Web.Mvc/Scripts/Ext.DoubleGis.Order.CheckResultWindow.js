Ext.ns("Ext.DoubleGis.UI.Order");

Ext.DoubleGis.UI.Order.CheckResultWindow = Ext.extend(Ext.util.Observable,
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
        },
        messageType = {
            None: 0,
            Info: 1,
            Warning: 2,
            Error: 3
        };

    var //Private methods
        initializeTable = function()
        {
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

        initializeWindow = function()
        {
            extWindow = new Ext.Window({
                modal: false,
                layout: 'fit',
                items: [{ contentEl: domObjects.resultTable, autoScroll: true}],
                width: 600,
                height: 300,
                closable: false,
                title: Ext.LocalizedResources.OrderCheckResults,
                tools: [
                    { id: 'toggle', handler: function(event, toolEl, panel) { panel.collapsed ? panel.expand() : panel.collapse(); } },
                    {
                        id: 'refresh',
                        scope: this,
                        handler: function()
                        {
                            extWindow.hide();
                            self.fireEvent("action", "refresh");
                        }
                    }
                ],
                buttons: [{
                    id: 'repairButton',
                    text: Ext.LocalizedResources.RepairOutdatedOrderPositions,
                    handler: function () {
                        extWindow.hide();
                        self.fireEvent("action", "repairOutdatedOrderPositions");
                    }
                }, {
                    id: 'continueButton',
                    text: Ext.LocalizedResources.Continue,
                    handler: function()
                    {
                        extWindow.hide();
                        self.fireEvent("action", "proceed");
                    }
                }, {
                    id: 'closeButton',
                    text: Ext.LocalizedResources.Close,
                    handler: function()
                    {
                        extWindow.hide();
                        self.fireEvent("action", "close");
                    }
                }]
            });
        },

        renderCheckResult = function(messages)
        {
            messages = messages.slice(); //work with a copy because we'll sort it

            //Sort order: errors go before warnings, messages of the same type are grouped by OrderPosition they belong to
            messages.sort(function(a, b)
            {
                var aTypeOrder;
                if (a.Type == messageType.Error)
                    aTypeOrder = 3;
                else if (a.Type == messageType.Warning)
                    aTypeOrder = 2;
                else if (a.Type == messageType.Info)
                    aTypeOrder = 1;
                else
                    aTypeOrder = 0;

                var bTypeOrder;
                if (b.Type == messageType.Error)
                    bTypeOrder = 3;
                else if (b.Type == messageType.Warning)
                    bTypeOrder = 2;
                else if (b.Type == messageType.Info)
                    bTypeOrder = 1;
                else
                    bTypeOrder = 0;
                
                if (a.Type != b.Type)
                    return bTypeOrder - aTypeOrder;

                return a.OrderPositionId - b.OrderPositionId;
            });

            var prevGroupNumber = -1;

            while(domObjects.detailsCell.lastChild)
            {
                domObjects.detailsCell.removeChild(domObjects.detailsCell.lastChild);
            }

            for(var index = 0; index < messages.length; index++)
            {
                var row = document.createElement('div');
                domObjects.detailsCell.appendChild(row);

                if(prevGroupNumber != -1 && prevGroupNumber != messages[index].GroupNumber)
                {
                    row.className = 'firstInGroup';
                }

                prevGroupNumber = messages[index].GroupNumber;

                var imageCell = document.createElement('div');
                row.appendChild(imageCell);
                imageCell.className = 'image';

                var image = document.createElement('img');
                imageCell.appendChild(image);

                var imageSrc;
                if (messages[index].Type == messageType.Error)
                    imageSrc = Ext.Notification.Icon.CriticalError;
                else if (messages[index].Type == messageType.Warning)
                    imageSrc = Ext.Notification.Icon.Warning;
                else if (messages[index].Type == messageType.Info)
                    imageSrc = Ext.Notification.Icon.Info;
                else
                    imageSrc = Ext.Notification.Icon.None;

                image.setAttribute('src', imageSrc);

                var messageCell = document.createElement('div');
                row.appendChild(messageCell);
                messageCell.className = 'message';

                messageCell.appendChild(processLinks(messages[index].MessageText));

                var clearCell = document.createElement('div');
                row.appendChild(clearCell);
                clearCell.className = 'clear';
            }
        },

        setTitle = function(hasErrors, hasWarnings)
        {
            if (hasErrors) {
                domObjects.titleCell.innerText = Ext.LocalizedResources.CheckProducedTheFollowingErrors;
            }
            else if (hasWarnings) {
                domObjects.titleCell.innerText = Ext.LocalizedResources.OrderCheckProducedTheFollowingWarnings;
            } else {
                domObjects.titleCell.innerText = Ext.LocalizedResources.OrderIsCorrect;
            }
        },

        setupButtons = function (hasErrors, canProceed, hasOutdatedPricePositions) {
            var repairButton = extWindow.buttons.findOne(function (button) {
                return button.id === 'repairButton';
            });

            if (hasOutdatedPricePositions) {
                repairButton.show();
            } else {
                repairButton.hide();
            }

            var saveButton = extWindow.buttons[1];
            if(canProceed) {
                saveButton.show();
                if(hasErrors) {
                    saveButton.disable();
                } else {
                    saveButton.enable();
                }
            } else {
                saveButton.hide();
            }
        },

        showResultWindow = function(messages, canProceed) {
            var hasErrors = false;
            var hasWarnings = false;
            var hasOutdatedPricePositions = false;

            Ext.each(messages, function (message) {
                if (message.Type == messageType.Error) {
                    hasErrors = true;
                }
                
                if (message.Type == messageType.Warning || message.Type == messageType.Info) {
                    hasWarnings = true;
                }

                if (message.Type == messageType.Error && message.AdditionalInfo) {
                    for (var i in message.AdditionalInfo) {
                        if (message.AdditionalInfo[i].Key == 'HasOutdatedPricePositions' && message.AdditionalInfo[i].Value) {
                            hasOutdatedPricePositions = true;
                        }
                    }
                }
            });

            setTitle(hasErrors, hasWarnings);
            setupButtons(hasErrors, canProceed, hasOutdatedPricePositions);

            domObjects.resultTable.style.visibility = 'visible';
            extWindow.show();
        },

        processLinks = function(text)
        {
            var result = document.createElement('span');
            var j;
            for(var i = 0; i < text.length; i++)
            {
                if(text.charAt(i) != '<')
                {
                    j = i;
                    while(j + 1 < text.length && text.charAt(j + 1) != '<')
                    {
                        j++;
                    }
                    var span = document.createElement('span');
                    span.innerText = text.substring(i, j + 1);
                    result.appendChild(span);
                    i = j;
                } else
                {
                    j = i + 1;
                    while(text.charAt(j) != '>')
                    {
                        j++;
                    }

                    var sp = text.substring(i + 1, j).split(':');

                    var link = document.createElement('a');
                    link.setAttribute('href', '#');
                    link.innerText = sp[1];
                    link.onclick = (function(entityName, entId)
                    {
                        return function() { Ext.DoubleGis.Global.Helpers.OpenEntity(entityName, entId); };
                    })(sp[0], sp[2]);

                    result.appendChild(link);
                    i = j;
                }
            }
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

        showResult: function(messages, canProceed)
        {
            renderCheckResult(messages);
            showResultWindow(messages, canProceed);
        }
    };
} ());
