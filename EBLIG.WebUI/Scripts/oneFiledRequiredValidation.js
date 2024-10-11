
$.validator.addMethod('onefiledrequiredvalidation', function (value, element, params) {
    if (String(value).trim() != "") {
        return true;
    }

    if (String($("#" + params.other).val()).trim() != "") {
        return true;
    }

    return false;
});

$.validator.unobtrusive.adapters.add("onefiledrequiredvalidation", ["other"], function (options) {
    options.rules["onefiledrequiredvalidation"] = options.params;
    options.messages["onefiledrequiredvalidation"] = options.message;
});






