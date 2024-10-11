
$.validator.addMethod('requiredfromebligadmin', function (value, element, params) {
    return value != "";
});

$.validator.unobtrusive.adapters.add("requiredfromebligadmin", function (options) {
    options.rules["requiredfromebligadmin"] = options.params;
    options.messages["requiredfromebligadmin"] = options.message;
});

