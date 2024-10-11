//pino tuzzolino
var _modalArray = new Array();

function dynamicModal_PushNewModal(id, modalHtml) {

    try {
        if (id == "dynamicModal_waid")
            return;

        $.each(_modalArray, function (index, item) {
           $("#" + item.id).modal("hide");
           if (item.id == id) {
                _modalArray.remove()
            }
        })

        for (var i = 0; i < _modalArray.length; i++) {
            $("#" + _modalArray[i][0]).modal("hide");
        }
        _modalArray.push({ id: id, html: modalHtml });

    } catch (e) {
        //alert("dynamicModal_PushNewModal: " + e.description);
    }
    // alert(_modalArray.length);
}

function dynamicModal_RemovewModal(id) {

    try {

        // 
        // setTimeout(function () {

        $("body").removeClass("modal-open");
        //$('body').removeClass('blur');

        $('#wrapper').removeClass('blur');


        $(".modal").removeClass("fade").removeClass("show");
        $(".modal").addClass("fade").addClass("hide");

        $(".modal").remove();

        $(".modal-backdrop").removeClass("fade").removeClass("show");
        $(".modal-backdrop").addClass("fade").addClass("hide");

        $(".modal-backdrop").remove();

        $("body").attr("style", "padding-right: 0px !important;");

        //$(".modal-backdrop").each(function (e) {
        //    $(this).remove();
        //});
        // }, 250);

        return;

        //TODO
        //setTimeout(function () {
        //    $("body").removeAttr("class");
        //    $("body").attr("style", "");
        //}, 10);


        var _modalArray_new = Array();

        for (var i = 0; i < _modalArray.length; i++) {
            if (_modalArray[i][0] != id) {
                _modalArray_new.push([_modalArray[i][0], _modalArray[i][1]]);
            }
        }

        if (_modalArray_new.length > 0) {

            setTimeout(function () {
                $("#dynamicModal_waid").modal("hide");
                $("#dynamicModal_waid").remove();

                // $("#wrapper").removeClass("blur");

                $("#" + id).hide();
                $("#" + id).remove();


                $(".modal-backdrop").each(function (e) {
                    $(this).remove();
                });


                var _lastModal = _modalArray_new[_modalArray_new.length - 1];
                if (_lastModal != undefined) {

                    $("#" + _lastModal[0]).modal("show");
                    $("body").addClass("modal-open");
                    $("body").attr("style", "overflow:hidden; padding-right:0");
                    $("#wrapper").addClass("blur");
                }

            }, 100);

        }
        else {

            setTimeout(function () {
                $("#dynamicModal_waid").modal("hide");
                $("#dynamicModal_waid").remove();

                // $("#wrapper").removeClass("blur");

                $("#" + id).hide();
                $("#" + id).remove();

                $(".modal-backdrop").each(function (e) {
                    $(this).remove();
                });

                $("body").removeClass("modal-open");
                $("body").attr("style", "");
                $("#wrapper").removeClass("blur");
            }, 100);
        }

        _modalArray = new Array();

        _modalArray = _modalArray_new;

    } catch (e) {
        //alert(e.message);
    }
}

function dynamicModal_CreateModal(id, classname, title, html, showHeader, showFooter, showDismissButton) {
    try {
        var _lastid = getModalGenerateUUID();

        if (_lastid != undefined) {
            var _lastHtml = $("#" + _lastid + "-body").html();
            dynamicModal_PushNewModal(_lastid, _lastHtml);
        }

        $("[data-toggle='tooltip']").tooltip("hide");

        hideWaidModal();

        //  alert(_modalArray.length)
        //alert("id: " + id);

        //$("#dynamicModal_waid").modal("hide");
        //$("#dynamicModal_waid").remove();

        //$(".modal-backdrop").each(function (e) {
        //    //$(this).removeClass("fade in");
        //    //$(this).addClass("fade out");
        //    $(this).remove();
        //});

        var _id = id;
        var _modaleHtml = "<div role=\"dialog\"  class=\"modal fade\" id=\"" + _id + "\" tabindex=\"-1\" role=\"dialog\" aria-labelledby=\"" + id + "-label\" aria-hidden=\"true\" data-bs-keyboard=\"false\" data-bs-backdrop=\"static\">";
        _modaleHtml += "<div class=\"modal-dialog\">";
        _modaleHtml += "<div class=\"modal-content\">";

        if (showHeader == undefined || showHeader) {
            _modaleHtml += "<div class=\"modal-header\">";
            _modaleHtml += "<h4 class=\"modal-title\" id=\"" + _id + "-label\">" + title + "</h4>";

            if (_id != "dynamicModal_waid" && showDismissButton == undefined && showDismissButton != false)
                _modaleHtml += "<button type=\"button\" class=\"close\" data-dismiss=\"modal\" aria-hidden=\"true\" onclick=\"dynamicModal_RemovewModal('" + _id + "')\">&times;</button>";

            _modaleHtml += "</div>";
        }
        else {
            if (showDismissButton) {
                _modaleHtml += "<div class=\"/*modal-header*/\">";
                _modaleHtml += "<a href=\"javascript:void(0)\" class=\"close fas fa-window-close fa-3x text-danger\" data-dismiss=\"modal\" aria-hidden=\"true\" onclick=\"dynamicModal_RemovewModal('" + _id + "')\" style=\"opacity:1 !important\"></a>";
                _modaleHtml += "</div>";
            }
        }

        _modaleHtml += "<div id=\"" + _id + "-body\" class=\"modal-body\" style=\"overflow:hidden\">";

        if (classname == "" || classname == undefined)
            _modaleHtml += "<p>" + html + "</p>";
        else
            _modaleHtml += html;

        //_modaleHtml += html;
        _modaleHtml += "</div>";

        if (showFooter == undefined || showFooter) {
            _modaleHtml += "<div class=\"modal-footer\">";
            _modaleHtml += "<button type=\"button\" class=\"btn btn-danger\" data-dismiss=\"modal\" style=\"margin-bottom:20px\" onclick=\"dynamicModal_RemovewModal('" + _id + "')\" id=\"closeButton_" + id + "\">Chiudi</button>";
            _modaleHtml += "<span id=\"" + _id + "_ModalButtonDelete\" style=\"margin-left:10px\"></span>";
            //_modaleHtml += "<a class=\"btn\" role=\"button\" onclick=\"showDynamicModal('myModal_7', 'm','00', true, true)\" href=\"#myModal_7\" data-toggle=\"modal\">Launch other modal</a>";
            _modaleHtml += "</div>";
        }
        else {
            _modaleHtml += "<div style=\"height:15px\"></div>";
        }

        _modaleHtml += "</div>";
        _modaleHtml += "</div>";
        _modaleHtml += "</div>";

        ////nascondi quelli visibili prima di aggiungere uno nuovo
        //for (var i = 0; i < _modalArray.length; i++) {
        //    $("#" + _modalArray[i][0]).modal("hide");
        //}

        ////nascondi waid modale
        //$("#dynamicModal_waid").modal("hide");

        $("#waidModalshowWaidModal").remove();

        $("body").append(_modaleHtml);

        $("#" + _id).addClass(classname);

        //dynamicModal_PushNewModal(_id, html);

        $("#" + _id).modal("show");

        $('#wrapper').addClass('blur');
        //$('body').addClass('blur');
        //$('footer').addClass('blur');
        $("body").attr("style", "padding-right: 0px !important;");

        //setTimeout(function () {
        //    //$('#wrapper').addClass('blur');
        //    //$('footer').addClass('blur');
        //    $("#" + _id).modal("show");
        //    $("body").attr("style", "overflow:hidden");
        //    $("body").addClass("modal-open");
        //}, 250);

    } catch (e) {

        //alert(e.message);
    }
}

//basic functions
function dynamicModal_GetCloseButtonID() {
    return "#closeButton_" + getModalGenerateUUID();
}

function showDynamicModal_FullScreenNoScroll(id, title, html, showHeader, showFooter) {
    dynamicModal_CreateModal(id, "modal-fullscreen-noscroll", title, html, showHeader, showFooter, true);
}

function showModalFullScreen_NoHeaderFooterNoScroll(html) {
    showDynamicModal_FullScreenNoScroll(modalGenerateUUID(), "Informazioni", html, false, false);
    enableAllBtn();
}

function showModalFullScreen_NoHeaderNoScroll(html) {
    showDynamicModal_FullScreenNoScroll(modalGenerateUUID(), "Informazioni", html, false, true);
    enableAllBtn();
}

function showModalFullScreen_NoFooterNoScroll(html) {
    showDynamicModal_FullScreenNoScroll(modalGenerateUUID(), "Informazioni", html, true, false);
    enableAllBtn();
}

function showDynamicModal_FullScreen(id, title, html, showHeader, showFooter) {
    dynamicModal_CreateModal(id, "modal-fullscreen", title, html, showHeader, showFooter, true);
}

function showDynamicModal_FullScreen_NoHeaderFooter(id, title, html) {
    dynamicModal_CreateModal(id, "modal-fullscreen", title, html, false, false, true);
}

function showDynamicModal(id, title, html, showHeader, showFooter, showDismissButton) {
    dynamicModal_CreateModal(id, "", title, html, showHeader, showFooter, showDismissButton);
}

function showDynamicModal_NoHeaderFooter(id, title, html) {
    dynamicModal_CreateModal(id, "", title, html, false, false);
}

function getModalGenerateUUID() {
    return $("[role='dialog']").attr("id");
}



function modalGenerateUUID() {
    var d = new Date().getTime();
    if (window.performance && typeof window.performance.now === "function") {
        d += performance.now(); //use high-precision timer if available
    }
    var uuid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = (d + Math.random() * 16) % 16 | 0;
        d = Math.floor(d / 16);
        return (c == 'x' ? r : (r & 0x3 | 0x8)).toString(16);
    });
    return uuid;
}

//function getModalGenerateUUID() {
//    try {
//        if (_modalArray.length > 0) {
//            return _modalArray[_modalArray.length - 1].id;
//        }
//        return "";

//    } catch (e) {
//        return "";
//    }
//}

//end basic functions


//$().ready(function () {
//    $("[data-dismiss='modal']").click(function () {
//        alert()
//    });
//})

//$(document).on('hide.bs.modal', function () {


//    alert()
//});

function hideModal() {
    enableAllBtn();
    var _id = getModalGenerateUUID();
    dynamicModal_RemovewModal(_id);
    $("#" + _id).remove();
    $("body").attr("style", "");
    $('#wrapper').removeClass('blur');
    $("#waidModalshowWaidModal").remove();
    alertClose();

}

function hideWaidModal() {
    enableAllBtn();
    dynamicModal_RemovewModal("dynamicModal_waid");
    $("body").attr("style", "");
    $('#wrapper').removeClass('blur');
}

function showModalFullScreen(html) {
    showDynamicModal_FullScreen(modalGenerateUUID(), "Informazioni", html, true, true);
    enableAllBtn();
}

function showModalFullScreen(title, html) {
    showDynamicModal_FullScreen(modalGenerateUUID(), title, html, true, true);
    enableAllBtn();
}

function showModalFullScreen_NoHeaderFooter(html) {
    showDynamicModal_FullScreen_NoHeaderFooter(modalGenerateUUID(), "Informazioni", html);
    enableAllBtn();
}

function showModalFullScreen_NoHeader(html) {
    showDynamicModal_FullScreen(modalGenerateUUID(), "Informazioni", html, false, true);
    enableAllBtn();
}

function showModal(html) {
    showDynamicModal(modalGenerateUUID(), "Informazioni", html, true, true);
    enableAllBtn();
}

function showModal(title, html) {
    showDynamicModal(modalGenerateUUID(), title, html, true, true);
    enableAllBtn();
}

function showModal_NoHeaderFooter(html) {
    showDynamicModal(modalGenerateUUID(), "", html, false, false);
    enableAllBtn();
}

function showModal_NoHeader(html) {
    showDynamicModal(modalGenerateUUID(), "", html, false, true);
    enableAllBtn();
}

function showModal_NoFooter(title, html) {
    showDynamicModal(modalGenerateUUID(), title, html, true, false);
    enableAllBtn();
}

function showModal_NoFooterNoDismissButton(title, html) {
    showDynamicModal(modalGenerateUUID(), title, html, true, false, false);
    enableAllBtn();
}

function showWaidModal(e) {
    hideModal();

    var _t = "";
    _t = "<div id=\"waidModalshowWaidModal\" class=\"swal2-container swal2-center swal2-backdrop-show\" style=\"overflow-y: auto;\"><div aria-labelledby=\"swal2-title\" aria-describedby=\"swal2-html-container\" class=\"swal2-popup swal2-modal swal2-show\" tabindex=\"-1\" role=\"dialog\" aria-live=\"assertive\" aria-modal=\"true\" style=\"display: grid;\"><button type=\"button\" class=\"swal2-close disabled\" aria-label=\"Close this dialog\" data-disablebtn=\"1\" disabled=\"disabled\" style=\"display: none;\">×</button><ul class=\"swal2-progress-steps\" style=\"display: none;\"></ul><div class=\"swal2-icon swal2-icon-show\" style=\"display: flex;\"><div class=\"swal2-icon-content\"><div class=\"spinner-border text-primary\"></div></div></div><img class=\"swal2-image\" style=\"display: none;\"><div class=\"swal2-html-container\" id=\"swal2-html-container\" style=\"display: block;\">Attendere, operazione in corso...</div><input class=\"swal2-input\" style=\"display: none;\"><input type=\"file\" class=\"swal2-file\" style=\"display: none;\"><div class=\"swal2-range\" style=\"display: none;\"><input type=\"range\"><output></output></div><select class=\"swal2-select\" style=\"display: none;\"></select><div class=\"swal2-radio\" style=\"display: none;\"></div><label for=\"swal2-checkbox\" class=\"swal2-checkbox\" style=\"display: none;\"><input type=\"checkbox\"><span class=\"swal2-label\"></span></label><textarea class=\"swal2-textarea\" style=\"display: none;\"></textarea><div class=\"swal2-validation-message\" id=\"swal2-validation-message\" style=\"display: none;\"></div><div class=\"swal2-actions\" style=\"display: none;\"><div class=\"swal2-loader\"></div><button type=\"button\" class=\"swal2-confirm swal2-styled disabled\" aria-label=\"\" data-disablebtn=\"1\" disabled=\"disabled\" style=\"display: none;\">OK</button><button type=\"button\" class=\"swal2-deny swal2-styled disabled\" aria-label=\"\" data-disablebtn=\"1\" disabled=\"disabled\" style=\"display: none;\">No</button><button type=\"button\" class=\"swal2-cancel swal2-styled disabled\" aria-label=\"\" data-disablebtn=\"1\" disabled=\"disabled\" style=\"display: none;\">Cancel</button></div><div class=\"swal2-footer\" style=\"display: none;\"></div><div class=\"swal2-timer-progress-bar-container\"><div class=\"swal2-timer-progress-bar\" style=\"display: none;\"></div></div></div></div>";
    $("body").append(_t);


    //    var _m = (e == undefined ? getAlert_Waid("Attendere, operazione in corso...") : "<br/>" + e);// "<span class='text-default'>" + (e == undefined ? "Attendere, operazione in corso..." : e) + "</span>";
    //    showDynamicModal("dynamicModal_waid", "Informazioni", _m, false, false);
}

function setModalTitle(e) {
    var _id = getModalGenerateUUID() + "-label";
    $("#" + _id).html(e);
}

function setModalBody(e) {
    var _id = getModalGenerateUUID() + "-body";
    $("#" + _id).html(e);
}

function destroyModal() {

    $("#dynamicModal_waid").modal("hide");
    $("#dynamicModal_waid").remove();
    $('#wrapper').removeClass('blur');

    for (var i = 0; i < _modalArray.length; i++) {
        $("#" + _modalArray[i][0]).modal("hide");
        $("#" + _modalArray[i][0]).remove();
    }

    $(".modal-backdrop").each(function (e) {
        $(this).remove();
    });

    _modalArray = null;
    _modalArray = new Array();

    setTimeout(function () {
        $("body").removeClass("modal-open");
        $("body").removeAttr("class");
        $("body").attr("style", "");
    }, 10);

}

function createDeleteModal(html, action, onSuccess, onComplete) {

    //uso
    //function OnAjaxRequestBegin()
    //function OnAjaxRequestSuccess(data)
    //function OnAjaxRequestFailure(request, error) 
    //function OnAjaxRequestComplete(request, status)

    var _btn = "";

    _btn += "<a class=\"btn btn-danger\" href=\"" + action + "\" data-ajax-method=\"Post\" data-ajax=\"true\" ";

    _btn += "data-ajax-failure=\"OnAjaxRequestFailure_createDeleteModal\" ";

    // _btn += " data-ajax-begin=\"" + onBegin + "\" data-ajax-complete=\"" + onComplete + "\" data-ajax-fail=\"" + onFail + "\"";

    if (onComplete != undefined && onComplete != null && onComplete != '')
        _btn += "data-ajax-complete=\"" + onComplete + "\" ";

    if (onSuccess != undefined && onSuccess != null && onSuccess != '')
        _btn += "data-ajax-success=\"" + onSuccess + "\" ";

    _btn += "data-ajax-begin=\"disableAllBtn\">Conferma Elimina</a>";

    var _body = "<div>" + html + "<br/>Confermi l'operazione?<br/></div>";

    showModal("Elimina", _body);

    var _id = getModalGenerateUUID() + "_ModalButtonDelete";
    $("#" + _id).html(_btn);
}

function OnAjaxRequestFailure_createDeleteModal(request, error) {
    setModalBody("Si è verificato un errore");
    handleError(error);
}

function showModal_IsOpen() {
    if ($("[role='dialog']").hasClass("modal")) {
        return true;
    }
    return false;
}

//$('.modal').on('hide', function (e) {
//    alert();
//});

//$('#' + getModalGenerateUUID()).on('hidden.bs.modal', function () {
//    alert();
//    $('body').removeClass('blur');
//})

//$('#myModal').on('show.bs.modal', function () {
//    $('body').addClass('blur');
//})

//old function call
//$('.modal').on('show.bs.modal', function () {
//    alert('hi')
//})

//$('.modal').on('hidden.bs.modal', function (e) {
//    alert();
//    // do something...
//})

//$('.modal-dialog').on('hidden.bs.modal', function (e) {
//    // call your method
//    alert();
//})

//$('.modal').on('hidden', function (e) {
//    alert();
//    // do something...
//})

//$(window).on('hidden.bs.modal', function (e) {
//    alert("hidden");

//    //if (_modalArray.length >= 1) {
//    //    $("body").addClass("modal-open");
//    //}
//    //else
//    //{
//    //    alert();
//    //    //$(".modal-backdrop fade in").remove();

//    //}

//});

