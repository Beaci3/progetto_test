
//load content - views content
var _innerTextLoadContainer = "<div class=\"loading_outer text-center p-5\">" +
    "<div class=\"spinner-border mt-3 text-primary\"></div>" +
    "<div class=\"text-primary mt-3\"><strong>Caricamento in corso...</strong></div>" +
    "</div> ";

var _innerTextLoadContainerWhite = "<div class=\"loading_outer text-center\">" +
    "<div class=\"spinner-border mt-3 text-white\"></div>" +
    "<div class=\"text-white mt-3\"><strong>Caricamento in corso...</strong></div>" +
    "</div> ";


var _innerTextLoadContainer_small = "<div class=\"loading_outer_small text-center\">" +
    "<div class=\"spinner-border mt-3 text-secondary\"></div>" +
    "</div> ";

var _innerTextOperazioneContainer = "<div class=\"loading_outer text-center\">" +
    "<div class=\"spinner-border mt-3 text-secondary\"></div>" +
    "<div class=\"text-secondary mt-3\"><strong>Elaborazione in corso...</strong></div>" +
    "</div> ";


function innerTextLoadContainer(text, height) {
    text = text == "" || text == undefined ? "Caricamento in corso..." : text;
    height = height == "" || height == undefined ? "140" : height
    return "<div class=\"text-center mt-2 mb-2 w-100\" style=\"border:1px solid #efefef;height:" + height + "px\">" +
        "<div class=\"spinner-border mt-3 text-info\"></div>" +
        "<div class=\"text-info mt-3\"><strong>" + text + "</strong ></div > " +
        "</div>";
}

function innerTextLoadContainerNoBorder(text, height) {
    text = text == "" || text == undefined ? "Caricamento in corso..." : text;
    height = height == "" || height == undefined ? "140" : height
    return "<div class=\"text-center mt-2 mb-2 w-100\" style=\"height:" + height + "px\">" +
        "<div class=\"spinner-border mt-3 text-info\"></div>" +
        "<div class=\"text-info mt-3\"><strong>" + text + "</strong ></div > " +
        "</div>";
}

function onBeginRicerca() {
    $("#panelRicerca-collapseOne").collapse('hide');
    $("#resultRicerca").html(containerLoadingText());
    alertWaid();
}

function ajaxLoader() {
    return "<img src=\"data:image/gif;base64,R0lGODlhEAALAPQAAD3A9QAAADSk0jGdyDiw4QEEBQAAAAsiLB9ifRdIXCyMswgZIBE3RyFohRhLYC2PtwkcJAADAxI6Szet3TOj0Dq46w0qNjSm1Dq26SuJryZ4mjCYwjmz5QAAAAAAAAAAACH/C05FVFNDQVBFMi4wAwEAAAAh/hpDcmVhdGVkIHdpdGggYWpheGxvYWQuaW5mbwAh+QQJCwAAACwAAAAAEAALAAAFLSAgjmRpnqSgCuLKAq5AEIM4zDVw03ve27ifDgfkEYe04kDIDC5zrtYKRa2WQgAh+QQJCwAAACwAAAAAEAALAAAFJGBhGAVgnqhpHIeRvsDawqns0qeN5+y967tYLyicBYE7EYkYAgAh+QQJCwAAACwAAAAAEAALAAAFNiAgjothLOOIJAkiGgxjpGKiKMkbz7SN6zIawJcDwIK9W/HISxGBzdHTuBNOmcJVCyoUlk7CEAAh+QQJCwAAACwAAAAAEAALAAAFNSAgjqQIRRFUAo3jNGIkSdHqPI8Tz3V55zuaDacDyIQ+YrBH+hWPzJFzOQQaeavWi7oqnVIhACH5BAkLAAAALAAAAAAQAAsAAAUyICCOZGme1rJY5kRRk7hI0mJSVUXJtF3iOl7tltsBZsNfUegjAY3I5sgFY55KqdX1GgIAIfkECQsAAAAsAAAAABAACwAABTcgII5kaZ4kcV2EqLJipmnZhWGXaOOitm2aXQ4g7P2Ct2ER4AMul00kj5g0Al8tADY2y6C+4FIIACH5BAkLAAAALAAAAAAQAAsAAAUvICCOZGme5ERRk6iy7qpyHCVStA3gNa/7txxwlwv2isSacYUc+l4tADQGQ1mvpBAAIfkECQsAAAAsAAAAABAACwAABS8gII5kaZ7kRFGTqLLuqnIcJVK0DeA1r/u3HHCXC/aKxJpxhRz6Xi0ANAZDWa+kEAA7AAAAAAAAAAAA\"/>";
}

function containerLoadingText() {
    return "<div style=\"background-color: white; border:1px solid rgba(0, 0, 0, 0.125)\">" + _innerTextLoadContainer + "</div>";
}

window.addEventListener('popstate', function () {
    loadContent(event.state, false);
});

function loadContent(action, savestate = true) {

    $.xhrPool.abortAll();

    if (savestate)
        window.history.pushState(action, "Eblig", action);

    $.ajax({
        type: "GET",
        cache: false,
        url: action,
        error: function (e) {
            onErrorLoadContainer(e);
        },
        beforeSend: function () {
            onBeginnLoadContainer();
        },
        success: function (e) {
            onSuccessLoadContainer(e);
        },
        complete: function (e) {
        }
    });
}

function onErrorLoadContainer(error) {
    alertClose();
    $('#contentrenderbody').html(error.responseText);
    scrollTop();
}

function onSuccessLoadContainer(data) {
    alertClose();
    $('#contentrenderbody').html(data);
    scrollTop();
}

function onBeginnLoadContainer() {
    scrollTop();
    alertWaid();
    return;
}

$.xhrPool = [];
$.xhrPool.abortAll = function () {
    try {
        $(this).each(function (i, jqXHR) {
            jqXHR.abort();
            $.xhrPool.splice(i, 1);
        });
    } catch (e) {
    }
};

function xhrPoolAbortAll() {
    $.xhrPool.abortAll();
}

//$(document).ajaxSend(function (event, jqXHR, options) {
//    if (!options.url.endsWith('/Backend/Messaggistica/MessaggioNonLetti')
//        && !options.url.endsWith('/Backend/Notifiche/Scadenze')
//        && !options.url.endsWith('/Backend/Notifiche/Informazioni')) {
//        $.xhrPool.push(jqXHR);
//    }
//});

