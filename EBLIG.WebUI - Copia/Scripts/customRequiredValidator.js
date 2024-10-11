
$.validator.addMethod('customrequiredvalidator', function (value, element, params) {
    if (value == "" || String(value).replace(/\s/g, '').length ==0) {
        setTimeout(function () {
            $("span[data-valmsg-for='" + element.id + "']").removeClass("field-validation-valid");
            $("span[data-valmsg-for='" + element.id + "']").addClass("field-validation-error");
            $("span[data-valmsg-for='" + element.id + "']").html("Il campo " + $("[for='" + element.id +"']").html() +" è obbligatorio.");
            $("span[data-valmsg-for='" + element.id + "']").show();
        }, 10);
        return false;
    }
  
    return true;
});

$.validator.unobtrusive.adapters.add("customrequiredvalidator", function (options) {
    options.rules["customrequiredvalidator"] = options.params;
    options.messages["customrequiredvalidator"] = options.message;
});

