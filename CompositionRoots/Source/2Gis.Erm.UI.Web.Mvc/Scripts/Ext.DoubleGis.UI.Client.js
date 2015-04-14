window.InitPage = function () {
    window.Card.on("beforebuild", function() {
        if (Ext.getDom("Id").value != 0) {
            // change owner
            this.ChangeOwner = function() {
                var params = "dialogWidth:450px; dialogHeight:300px; status:yes; scroll:no;resizable:no;";
                var sUrl = "/GroupOperation/Assign/Client";
                var result = window.showModalDialog(sUrl, [Ext.getDom("Id").value], params);
                if (result === true) {
                    this.refresh(true);
                }
            };

            // change territory
            this.ChangeTerritory = function() {
                var params = "dialogWidth:450px; dialogHeight:230px; status:yes; scroll:no; resizable:no; ";
                var sUrl = "/GroupOperation/ChangeTerritory/Client";
                var result = window.showModalDialog(sUrl, [Ext.getDom("Id").value], params);
                if (result === true) {
                    this.refresh(true);
                }
            };

            // qualify
            this.Qualify = function() {
                var params = "dialogWidth:650px; dialogHeight:300px; status:yes; scroll:no; resizable:no; ";
                var sUrl = "/GroupOperation/Qualify/Client/";
                var result = window.showModalDialog(sUrl, [Ext.getDom("Id").value], params);
                if (result === true) {
                    this.refresh(true);
                }
            };

            // disqualify
            this.Disqualify = function() {
                var params = "dialogWidth:650px; dialogHeight:230px; status:yes; scroll:no; resizable:no; ";
                var sUrl = "/GroupOperation/Disqualify/Client";
                var result = window.showModalDialog(sUrl, [Ext.getDom("Id").value], params);
                if (result === true) {
                    this.refresh(true);
                }
            };
            this.Merge = function() {
                var params = "dialogWidth:" + 800 + "px; dialogHeight:" + 600 + "px; status:yes; scroll:yes;resizable:yes;";
                // FIXME {y.baranihin, 16.06.2014}: Давай все же будем использовать ООП. Пусть будет некий объект в этом scope, в не глобальная функция
                var url = GetMergeAddress(Ext.getDom("Id").value);
                window.showModalDialog(url, null, params);
                this.refresh(true);
            };
        }
    });

};