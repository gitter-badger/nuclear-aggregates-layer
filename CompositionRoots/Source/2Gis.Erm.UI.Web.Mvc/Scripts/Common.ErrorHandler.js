var Log = function () {
    this.ErrorLog = "";
};

Log.prototype.SendError = function(message)
{
    this.ErrorLog = message;
    var errorLog = this.ErrorLog;

    Ext.Ajax.request({
            timeout: 1200000,
            url: "/Error/LogError",
            params: message,
            method: "POST",
            scope: this,
            failure: function(xhr)
            {
                Ext.MessageBox.show({
                        title: '',
                        msg: "Во время выполнения программы произошла ошибка. Залогировать сообщение также не удалось.\n" + "Сделайте снимок этого экрана и обратитесь, пожалуйста, к разработчикам\n" + xhr.responseText + "\n" + errorLog,
                        buttons: Ext.MessageBox.OK,
                        icon: Ext.MessageBox.ERROR
                    });
            },
            success: function() { errorLog = ""; }
        });
};

Log.prototype.HandleError = function (message, url, line)
{
    message = Ext.IS_DEBUG ? "Message: " + message + "\r\nErrorFile: " + url + "\r\nLine: " + line + "\r\nWindowUrl: " + window.location : message;
    
    if (Ext.isReady)
    {
        this.SendError(message);
        
        Ext.MessageBox.show({
            title: '',
            msg: message,
            buttons: Ext.MessageBox.OK,
            icon: Ext.MessageBox.ERROR
        });
    }
    else
    {
        this.ErrorLog += message;
        window.alert(message);
    }
    return true;
};

window.Logger = new Log();
window.onerror = function (msg, url, line) {
    Logger.HandleError(msg, url, line);
};
Ext.onReady(function () { if (Logger.ErrorLog) { Logger.SendError(Logger.ErrorLog); } });