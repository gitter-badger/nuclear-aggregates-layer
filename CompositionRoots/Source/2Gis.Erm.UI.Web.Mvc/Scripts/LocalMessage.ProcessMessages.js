function Fail(xmlHttpRequest)
{
    alert(xmlHttpRequest.responseText);
    window.returnValue = "OK";
    window.close();
}
function Sucess()
{
    var pb = Ext.getCmp("progressBarDel");
    pb.updateProgress(pb.value + (1 / window.dialogArguments.length));

    if (--window.itemsLeft == 0)
    {
        setTimeout(function ()
        {
            window.returnValue = "OK";
            window.close();
        }, 1000);
    }
    else
    {
        Ext.Ajax.request({
            timeout: 1200000,
            url: window.targetUrl + window.dialogArguments[window.itemsLeft-1],
            method: 'POST',
            success: Sucess,
            failure: Fail
        });
    }
}
function SubmitForm()
{
    if (window.dialogArguments)
    {
        var pb = new Ext.ProgressBar({ width: 580, cls: "custom", id: "progressBarDel", applyTo: "pbInner", value: 0 });

        window.targetUrl = "/LocalMessage/ProcessMessages/";
        window.itemsLeft = window.dialogArguments.length;
        Ext.get("pbDiv").dom.style.visibility = "";
        Ext.get("bodyMessage").dom.style.visibility = "hidden";
        Ext.getDom("OK").disabled = true;

        Ext.Ajax.request({
            timeout: 1200000,
            url: window.targetUrl + window.dialogArguments[window.dialogArguments.length-1],
            method: 'POST',
            success: Sucess,
            failure: Fail
        });
    }
    else
    {
        alert(Ext.LocalizedResources.MustSelectOneOrMoreObject);
    }
}
Ext.onReady(function ()
{
    Ext.getDom("OK").onclick = SubmitForm;
    window.dialogWidth = "600px";
    if (window.dialogArguments && window.dialogArguments.length > 0)
    {
        //прикрепляем обработчики
        Ext.getDom("Cancel").onclick = function () { window.returnValue = "Cancel"; window.close(); };
        Ext.getDom("OK").onclick = SubmitForm;

        Ext.get("pbDiv").dom.style.visibility = "hidden";

        //выводим метку о количестве удаляемых записей
        if (window.dialogArguments)
        {
            Ext.get("bodyMessage").dom.innerHTML = Ext.LocalizedResources.ProcessMessagesConfirm;
            Ext.get("TopBarMessage").dom.innerHTML += window.dialogArguments.length;
        }
        else
        {
            Ext.get("TopBarMessage").dom.innerHTML = Ext.LocalizedResources.MustSelectOneOrMoreObject;
            Ext.getDom("OK").disabled = true;
        }
    }
    else
    {
        alert(Ext.LocalizedResources.SelectOneOrMoreRecords);
        window.returnValue = "Cancel";
        window.close();
    }
});
