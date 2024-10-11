
$.validator.addMethod('checkboxvalidation', function (value, element, params) {

    if (value != undefined && String(value).toLowerCase() == "true") {
        return true;
    }

    return $("#" + element.id).is(':checked');
});

$.validator.unobtrusive.adapters.add("checkboxvalidation", function (options) {
    options.rules["checkboxvalidation"] = options.params;
    options.messages["checkboxvalidation"] = options.message;
});






