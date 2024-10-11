
$.validator.addMethod('customrangevalidator', function (value, element, params) {

    if (params["validate"] == "False") {
        return true;
    }

    var _value = parseToFloat(value);

    var _minvalue = parseToFloat(params["minvalue"]);
    var _maxvalue = parseToFloat(params["maxvalue"]);

    if (_value <= 0) {
        setTimeout(function () {
            $("span[data-valmsg-for='" + element.id + "']").removeClass("field-validation-valid");
            $("span[data-valmsg-for='" + element.id + "']").addClass("field-validation-error");
            $("span[data-valmsg-for='" + element.id + "']").html("Inserire un valore positivo");
            $("span[data-valmsg-for='" + element.id + "']").show();
        }, 10);
        return false;
    }

    if (_minvalue != undefined && _value < _minvalue) {
        setTimeout(function () {
            $("span[data-valmsg-for='" + element.id + "']").removeClass("field-validation-valid");
            $("span[data-valmsg-for='" + element.id + "']").addClass("field-validation-error");
            $("span[data-valmsg-for='" + element.id + "']").html(params["minvalueerrormessage"]);
            $("span[data-valmsg-for='" + element.id + "']").show();
        }, 10);
        return false;
    }

    if (_maxvalue != undefined && _value > _maxvalue) {
        setTimeout(function () {
            $("span[data-valmsg-for='" + element.id + "']").removeClass("field-validation-valid");
            $("span[data-valmsg-for='" + element.id + "']").addClass("field-validation-error");
            $("span[data-valmsg-for='" + element.id + "']").html(params["maxvalueerrormessage"]);
            $("span[data-valmsg-for='" + element.id + "']").show();
        }, 10);
        return false;
    }

    return true;
});

$.validator.unobtrusive.adapters.add("customrangevalidator", ["validate","minvalue", "maxvalue", "minvalueerrormessage", "maxvalueerrormessage"], function (options) {
    options.rules["customrangevalidator"] = options.params;
    options.messages["customrangevalidator"] = options.message;
});

