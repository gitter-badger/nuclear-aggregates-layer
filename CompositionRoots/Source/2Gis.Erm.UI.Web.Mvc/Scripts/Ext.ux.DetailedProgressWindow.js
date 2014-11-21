Ext.ns('Ext.ux');

Ext.ux.DetailedProgressWindow = Ext.extend(Ext.util.Observable,
function ()
{
    var //Private fields
        self,
        extWindow,
        currentItemNumber,
        domObjects = {
            renderTarget: null,
            resultTable: null,
            detailsCell: null,
            images: null
        };

    var //Private methods
        initializeTable = function ()
        {
            var table = document.createElement('table');
            table.className = 'detailedProgress';
            domObjects.renderTarget.dom.appendChild(table);

            var tableBody = document.createElement('tbody');
            table.appendChild(tableBody);

            var titleRow = document.createElement('tr');
            tableBody.appendChild(titleRow);

            var detailsRow = document.createElement('tr');
            tableBody.appendChild(detailsRow);

            var details = document.createElement('td');
            detailsRow.appendChild(details);
            details.className = 'details';

            domObjects.resultTable = table;
            domObjects.detailsCell = details;
        },
        initializeWindow = function (windowTitle)
        {
            extWindow = new Ext.Window({
                modal: false,
                layout: 'fit',
                items: [{ contentEl: domObjects.resultTable, autoScroll: true}],
                width: 500,
                height: 260,
                closable: false,
                title: windowTitle
            });
        },
        renderDetails = function (detailsDescriptions)
        {
            domObjects.images = [];
            for (var index = 0; index < detailsDescriptions.length; index++)
            {
                var row = document.createElement('div');
                domObjects.detailsCell.appendChild(row);

                var imageCell = document.createElement('div');
                row.appendChild(imageCell);
                imageCell.className = 'image';

                var image = document.createElement('img');
                image.setAttribute("width", 16);
                image.setAttribute("height", 16);
                imageCell.appendChild(image);
                domObjects.images.push(image);

                var messageCell = document.createElement('div');
                row.appendChild(messageCell);
                messageCell.className = 'message';

                messageCell.innerHTML = detailsDescriptions[index];

                var clearCell = document.createElement('div');
                row.appendChild(clearCell);
                clearCell.className = 'clear';
            }
        },
        setCurrentProgress = function (newCurrentItemNumber)
        {
            currentItemNumber = newCurrentItemNumber;
            var index;
            for (index = 0; index < currentItemNumber; index++)
                domObjects.images[index].setAttribute("src", '<%=WebResource("images/done.gif")%>');

            if (currentItemNumber >= 0 && currentItemNumber < domObjects.images.length)
            {
                domObjects.images[currentItemNumber].setAttribute("src", '<%=WebResource("images/progress.gif")%>');
                domObjects.images[Math.min(currentItemNumber + 2, domObjects.images.length - 1)].scrollIntoView(false);
            }

            for (index = currentItemNumber + 1; index < domObjects.images.length; index++)
                domObjects.images[index].setAttribute("src", Ext.BLANK_IMAGE_URL);
        };

    return {
        constructor: function (config)
        {
            self = this;
            domObjects.renderTarget = config.renderTarget;
            initializeTable();
        },

        show: function (windowTitle, detailsDescriptions)
        {
            renderDetails(detailsDescriptions);
            initializeWindow(windowTitle);
            extWindow.show();
            setCurrentProgress(0);
        },

        setProgress: function (operationNumber)
        {
            setCurrentProgress(operationNumber);
        },

        close: function ()
        {
            extWindow.close();
        }
    };
} ());