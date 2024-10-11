///////////////////////////////////////////////////////////////////////////////////////
var _exitPage = true;

function resetPageExit() {
    _exitPage = false;
}

function resetPageExitHideModal() {
    resetPageExit();
    hideModal();
}

$(document).ajaxError(function (e, jqxhr, settings, exception) {
    e.stopPropagation();
    //ajax error event handler that looks for either a 401 (regular authorized) or 403 (AjaxAuthorized custom actionfilter). 
    if (jqxhr.status == 403) {
        hideModal();
        alertDanger(jqxhr.statusText);
        $("#contentrenderbody").html(getAlert_Danger(jqxhr.statusText));
    }

    if (jqxhr.status == 401) {
        this.location.href = "/";
    }
    //
    //if (!$('.modal:visible').length && $('body').hasClass('modal-open')) {
    //    hideModal();
    //    if (jqxhr != null) {
    //        showModal("Errore", getAlert_Danger(jqxhr.responseText));
    //    }
    //}
});

function handleError(error) {
    try {
        if (error.responseText == undefined) {
            alertDanger("Si e verificato un errore.");
        }
        else {

            var dom_nodes = $($.parseHTML(error.responseText));
            var _error = dom_nodes.filter('title').text();

            alertDanger(_error);
        }

    } catch (e) {
        alertDanger("Si e verificato un errore.");
    }
    enableAllBtn();
}

/////////////////////////////////////////////////////////////////////////////////////////
//disable invio da input
function stopRKey(evt) {
    var evt = (evt) ? evt : ((event) ? event : null);
    var node = (evt.target) ? evt.target : ((evt.srcElement) ? evt.srcElement : null);
    if ((evt.keyCode == 13) && ((node.type == "text") || (node.type == "radio") || (node.type == "checkbox"))) { return false; }
}

object.addEventListener("keypress", stopRKey);

$(document).ready(function () {
    $("form").submit(function (e) {
        e.preventDefault(e);
    });
});



/////////////////////////////////////////////////////////////////////////////////////////

$(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip();
    $('[data-toggle="popover"]').popover();
});

function disableAllBtn() {

    $("input[type=submit],input[type=button],input[type=reset], button, a, [data-disable='true']").each(function () {

        var _hashClass = $(this).hasClass("disabled");
        var _disabled = $(this).attr("disabled") != undefined;
        var _navlink = $(this).hasClass("nav-link");

        if (!_hashClass && !_disabled && !_navlink) {
            var _att = "data-disableBtn";
            $(this).attr(_att, "1")
            $(this).attr("disabled", "disabled");
            $(this).prop("disabled");
            $(this).addClass("disabled");
        }
    });
}

function enableAllBtn() {
    $("[data-disableBtn='1']").each(function () {
        $(this).removeData("disableBtn")
        $(this).removeAttr("disabled");
        $(this).removeClass("disabled");
        $(this).removeProp("disabled");
        $(this).removeAttr("data-disableBtn");
    });
}

function togglePanel(id) {

    var _e = $("#" + id);

    var _visible = _e.is(":visible");

    _e.toggle("slow");

    var _iPanel = $("#i_" + id);

    _iPanel.removeClass("glyphicon glyphicon-chevron-down fa-1x");

    _iPanel.removeClass("glyphicon glyphicon-chevron-up fa-1x");
    //alert(_visible);
    _iPanel.addClass(_visible ? "glyphicon glyphicon-chevron-down fa-1x" : "glyphicon glyphicon-chevron-up fa-1x");
}

function GetAntiForgeryToken() {
    var tokenField = $("input[type='hidden'][name$='RequestVerificationToken']");
    if (tokenField.length == 0) {
        return null;
    } else {
        return {
            name: tokenField[0].name,
            value: tokenField[0].value
        };
    }
}

function generateGuid() {
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

//#region alert

function getAlert(text, css) {

    switch (css) {
        case "primary":
            text = "<span class=\"spinner-border mr-3\"  style=\"width: 22px; height: 22px;float:left\" ></span>" + text;
            break;
        case "success":
            text = "<span class=\"fas fa-check-circle mr-3\" style=\"font-size:22px;float:left\"></span>" + text;
            break;
        case "warning":
            text = "<span class=\"fas fa-exclamation-circle mr-3\" style=\"font-size:22px;float:left\"></span>" + text;
            break;
        case "danger":
            text = "<span class=\"fas fa-exclamation-triangle mr-3\" style=\"font-size:22px;float:left\"></span>" + text;
            break;
        case "info":
            text = "<span class=\"fas fa-info-circle mr-3\" style=\"font-size:22px;float:left\"></span>" + text;
            break;
        default:
    }

    //var _t = "<div class=\"alert alert-" + css + "\" style=\"margin-top:25px\">";
    var _t = "<div class=\"alert alert-" + css + "\">";
    _t += text;
    _t += "</div>";
    return _t;
}

function getAlert_Waid(text) {
    return getAlert(text == undefined || text == null || text == "" ? "Attendere, operazione in corso..." : text, "primary")
}

function getAlert_Success(text) {
    return getAlert(text == undefined || text == null || text == "" ? "Operazione eseguita con successo." : text, "success")
}

function getAlert_Warning(text) {
    return getAlert(text == undefined || text == null || text == "" ? "Si e verificato un errore." : text, "warning")
}

function getAlert_Danger(text) {
    return getAlert(text == undefined || text == null || text == "" ? "Attenzione..." : text, "danger")
}

function getAlert_Info(text) {
    return getAlert(text == undefined || text == null || text == "" ? "Attenzione..." : text, "info")
}

function alert(text) {
    Swal.fire(text);
}

function alertWarning(text) {
    Swal.fire({
        icon: 'warning',
        //title: 'Attenzione',
        html: text,
    });

    var notify = new Audio("/Content/sound/alert.mp3");
    notify.muted = true;
    notify.play();

}

function alertDanger(text) {
    Swal.fire({
        icon: 'error',
        //title: 'Errore',
        html: text,
    });

    var notify = new Audio("/Content/sound/error.mp3");
    notify.muted = true;
    notify.play();

}

function alertInfo(text) {
    Swal.fire({
        icon: 'info',
        html: text,
    });

    var notify = new Audio("/Content/sound/info.mp3");
    notify.muted = true;
    notify.play();

}

function alertInfoNoCloseButton(text) {
    Swal.fire({
        icon: 'info',
        html: text,
        showConfirmButton: false,
        allowOutsideClick: false,
        allowEscapeKey: false
    });
}

function alertSuccess(text) {
    Swal.fire({
        icon: 'success',
        //title: 'Informazione',
        html: text,
    });
    var notify = new Audio("/Content/sound/success.mp3");
    notify.muted = true;
    notify.play();
}

function alertSuccessNoCloseButton(text) {
    Swal.fire({
        icon: 'success',
        //title: 'Informazione',
        html: text,
        showConfirmButton: false,
        allowOutsideClick: false,
        allowEscapeKey: false
    });
}

function alertWaid(text) {
    Swal.fire({
        html: text == undefined ? "Attendere, operazione in corso..." : text,
        iconHtml: '<div class=\"spinner-border text-primary\"></div>',
        //title: 'Operazione in corso...',
        showConfirmButton: false,
        allowOutsideClick: false,
        allowEscapeKey: false
    });
}

function alertClose() {
    Swal.close();
    $("#waidModalshowWaidModal").remove();

    //$(".swal2-popup").remove();
    //$(".swal2-container").remove();
    //$("html").removeClass("swal2-shown swal2-height-auto");
    //$("body").removeClass("swal2-shown swal2-height-auto");

}
//#endregion

function scrollToElement(e) {
    try {
        $('html, body').animate({
            scrollTop: $(e).offset().top - 50
        }, 800);
    } catch (e) {

    }
}

function scrollModalTop() {
    $('.modal').animate({ scrollTop: 0 }, 'slow');
}

function scrollTop() {
    try {
        $('html, body').animate({
            scrollTop: 0
        }, 800);
    } catch (e) {
    }
}

function getFileSize(size) {
    var totalSizeMb = size / Math.pow(1024, 2);
    return parseFloat(totalSizeMb.toFixed(2));
}


function getFileExtension(filename) {
    var r = /.+\.(.+)$/.exec(filename);
    return r ? r[1] : null;
}


function toLocalCurrency(number) {

    var _p = number.toLocaleString(undefined, { minimumFractionDigits: 2 });

    if (number < 0) {
        _p = "<span class='text-danger'>" + _p + "</span>";
    }

    return _p;

}

function toLocalDate(value) {
    try {
        var dateParts = value.split('/');
        var dateStr = dateParts[2] + '-' + dateParts[1] + '-' + dateParts[0];

        var isSafari = /Safari/.test(navigator.userAgent) && /Apple Computer/.test(navigator.vendor);

        if (isSafari) {
            var d = new Date();
            return new Date(dateParts[2], dateParts[1], dateParts[0]);
        } else {
            return new Date(dateStr);
        }
    } catch (e) {
        return undefined;
    }
}

function jsonDate(value) {
    var dateParts = value.split('/');
    return dateParts[2] + '-' + dateParts[1] + '-' + dateParts[0];
}


Date.daysBetween = function (date1, date2) {
    //Get 1 day in milliseconds
    var one_day = 1000 * 60 * 60 * 24;

    // Convert both dates to milliseconds
    var date1_ms = date1.getTime();
    var date2_ms = date2.getTime();

    // Calculate the difference in milliseconds
    var difference_ms = date2_ms - date1_ms;

    // Convert back to days and return
    return Math.round(difference_ms / one_day);
}

function createInput(name, value) {
    return $('<input>').attr({
        name: name,
        value: value
    });
}

function createInputButton(value, onclick, css) {
    return $('<button>').attr({
        onclick: onclick,
        type: "button"
    }).addClass(css).html(value);
}

function createHref(value, onclick, css) {
    return $('<a>').attr({
        onclick: onclick + ";return false",
        href: "javascript:#"
    }).addClass(css).html(value);
}

function clearForm(form) {
    // iterate over all of the inputs for the form
    // element that was passed in
    $(':input', form).each(function () {
        var type = this.type;
        var tag = this.tagName.toLowerCase(); // normalize case
        // it's ok to reset the value attr of text inputs,
        // password inputs, and textareas
        if (type == 'text' || type == 'password' || tag == 'textarea')
            this.value = "";
        // checkboxes and radios need to have their checked state cleared
        // but should *not* have their 'value' changed
        else if (type == 'checkbox')// || type == 'radio')
            this.checked = false;
        // select elements need to have their 'selectedIndex' property set to -1
        // (this works for both single and multiple select elements)
        else if (tag == 'select')
            this.selectedIndex = -1;
    });
};


//#region pdf

function getXml(base64, filename) {
    const a = document.createElement("a");
    a.href = URL.createObjectURL(
        base64toBlob(base64, "application/xml")
    );
    a.setAttribute("download", filename + ".xml");
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
}

function getTXT(base64, filename) {
    const a = document.createElement("a");
    a.href = URL.createObjectURL(
        base64toBlob(base64, "text/plain")
    );
    a.setAttribute("download", filename + ".txt");
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
}

function getPDF(base64, filename) {
    const a = document.createElement("a");
    a.href = URL.createObjectURL(
        base64toBlob(base64, "application/pdf")
    );
    a.setAttribute("download", filename + ".pdf");
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
}

function getExcel(base64, filename) {
    const a = document.createElement("a");
    a.href = URL.createObjectURL(
        base64toBlob(base64, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
    );
    a.setAttribute("download", filename + ".xlsx");
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
}
function base64toBlob(base64Data, contentType) {
    contentType = contentType || '';
    var sliceSize = 1024;
    var byteCharacters = atob(base64Data);
    var bytesLength = byteCharacters.length;
    var slicesCount = Math.ceil(bytesLength / sliceSize);
    var byteArrays = new Array(slicesCount);

    for (var sliceIndex = 0; sliceIndex < slicesCount; ++sliceIndex) {
        var begin = sliceIndex * sliceSize;
        var end = Math.min(begin + sliceSize, bytesLength);

        var bytes = new Array(end - begin);
        for (var offset = begin, i = 0; offset < end; ++i, ++offset) {
            bytes[i] = byteCharacters[offset].charCodeAt(0);
        }
        byteArrays[sliceIndex] = new Uint8Array(bytes);
    }
    return new Blob(byteArrays, { type: contentType });
}

//#endregion


function fillSelectOption(element, val, text) {
    $("<option/>", {
        value: val,
        text: text,
    }).appendTo("#" + element);
}

$('.table-responsive').on('hide.bs.dropdown', function () {
    $('.table-responsive').css("overflow", "auto");
})

function removejscssfile(filename, filetype) {
    var targetelement = (filetype == "js") ? "script" : (filetype == "css") ? "link" : "none" //determine element type to create nodelist from
    var targetattr = (filetype == "js") ? "src" : (filetype == "css") ? "href" : "none" //determine corresponding attribute to test for
    var allsuspects = document.getElementsByTagName(targetelement)
    for (var i = allsuspects.length; i >= 0; i--) { //search backwards within nodelist for matching elements to remove
        if (allsuspects[i] && allsuspects[i].getAttribute(targetattr) != null && allsuspects[i].getAttribute(targetattr).indexOf(filename) != -1)
            allsuspects[i].parentNode.removeChild(allsuspects[i]) //remove element by calling parentNode.removeChild()
    }
}

function toastInfo(message) {
    toast("info", message);
}

function toastSuccess(message) {
    toast("success", message);
}

function toastWarning(message) {
    toast("warning", message);
}

function toast(tipo, message) {
    toastr.options = {
        "closeButton": true,
        "debug": false,
        "newestOnTop": true,
        "progressBar": false,
        "positionClass": "toast-bottom-right",
        "preventDuplicates": true,
        "onclick": null,
        "closeDuration": false,
        "showDuration": "90000",
        "hideDuration": "1000",
        "timeOut": "90000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    };

    toastr[tipo](message);
}
