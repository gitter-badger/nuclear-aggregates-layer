function tryAgain() {

    var requestUrl = getQueryParam("RequestUri");

    if (!requestUrl) {
        window.location.reload();
    }
    else {
        if (requestUrl === window.location.pathname + window.location.search) {
            window.location.reload();
        }
        else {
            window.location = requestUrl;
        }
    }
}

function goBack() {
    var backUrl = getQueryParam("BackUri");
    if (!backUrl) {
        window.close();
    }
    else {
        window.location = backUrl;
    }
}

function getQueryParam(paramName) {
    var query = window.location.search.substring(1);
    var collection = Ext.urlDecode(query);
    return collection[paramName];
}