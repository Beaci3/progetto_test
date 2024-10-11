
$.validator.addMethod('datadaldataal', function (value, element, params) {

    var _dataal = String($("#" + params.dataalfield).val());

    if (value != "" && _dataal != "") {

        var _d1s = String(value).split('/');
        var _d2s = String(_dataal).split('/');

        var _d1 = new Date(_d1s[2], _d1s[1], _d1s[0]);
        var _d2 = new Date(_d2s[2], _d2s[1], _d2s[0]);

        if (_d1 > _d2) {
            return false;
        }
    }

    return true;
});

$.validator.unobtrusive.adapters.add("datadaldataal", ["dataalfield", "dataalrequired"], function (options) {
    
    $("#" + options.params.dataalfield).on("blur", function () {
        setTimeout(function () {
            $("#" + options.element.name).valid();
        }, 100);
    });
    options.rules["datadaldataal"] = options.params;
    options.messages["datadaldataal"] = options.message;
});

