jQuery.extend(jQuery.validator.messages, {
    required: "Questo campo è obbligatorio.",
    remote: "Please fix this field.",
    email: "Si prega di inserire un indirizzo email valido.",
    url: "Si prega di inserire un URL valido.",
    date: "Si prega di inserire una data valida.",
    dateISO: "Si prega di inserire una data valida (ISO).",
    number: "Si prega di inserire un numero valido.",
    digits: "Si prega di inserire solo cifre.",
    creditcard: "Immettere un numero di carta di credito valido.",
    equalTo: "Si prega di inserire lo stesso valore di nuovo.",
    accept: "Immettere un valore con un'estensione valida.",
    maxlength: jQuery.validator.format("Si prega di inserire non più di {0} caratteri."),
    minlength: jQuery.validator.format("Si prega di inserire almeno {0} caratteri."),
    rangelength: jQuery.validator.format("Si prega di inserire un valore tra {0} e {1} caratteri lunghi."),
    range: jQuery.validator.format("Si prega di inserire un valore tra {0} e {1}."),
    max: jQuery.validator.format("Immettere un valore minore o uguale a {0}."),
    min: jQuery.validator.format("Immettere un valore maggiore o uguale a {0}.")
});


$(document).ready(function () {

    Globalize.culture('it-IT');

    $.validator.methods.number = function (value, element) {
        //$(element).attr("data-val-number", "aaa");

        if (value == "")
            return true;

        if (!isNaN(Globalize.parseFloat(value))) {

            value = Globalize.parseFloat(value).toFixed(2);

            return true;// this.optional(element);
        }


        return false;
    }

    $.validator.methods.max = function (value, element) {
        var _value = toDecimalFormatUS(value);
        var _valuemax = toDecimalFormatUS($(element).attr("max"));
        //alert(parseInt(value));
        //alert(parseInt(_valuemax));
        return (parseInt(value) <= parseInt(_valuemax));
    };

    $.validator.methods.date = function (value, element) {
        var dateParts = value.split('/');
        var dateStr = dateParts[2] + '-' + dateParts[1] + '-' + dateParts[0];
        var isSafari = /Safari/.test(navigator.userAgent) && /Apple Computer/.test(navigator.vendor);

        if (isSafari) {
            var d = new Date();
            return this.optional(element) || !/Invalid|NaN/.test(new Date(dateParts[2], dateParts[1], dateParts[0]));
        } else {
            return this.optional(element) || !/Invalid|NaN/.test(new Date(dateStr));
        }
    };

    $.validator.methods.range = function (value, element, param) {
        try {

            if (element.type === 'checkbox') {
                // if it's a checkbox return true if it is checked
                return element.checked;
            }

            if (!isNaN(Globalize.parseFloat(value))) {

                var _min = param[0];

                var _max = param[1];

                var _value = Globalize.parseFloat(value).toFixed(2);

                if (_value >= _min && _value <= _max)
                    return true;// this.optional(element);
            }

            return false;

        } catch (e) {
            return false;
        }
    };
});

function toDecimalFormatUS(value) {
    value = String(value).replace(",", "");
    value = String(value).replace(".", ",");

    var formatter = new Intl.NumberFormat('en-US', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
    });

    return formatter.format(value);
}

function toDecimalFormatEuropa(value) {
    value = String(value).replace(".", "");
    value = String(value).replace(",", ".");

    var formatter = new Intl.NumberFormat('it', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
    });

    return formatter.format(value);
}

function parseToFloat(value) {
    try {
        return parseFloat(String(value).replace('.', '').replace(',', '.'));
    } catch (e) {
        return 0;
    }
}

function isNanOrZero(value) {
    try {
        var _p = parseToFloat(value);
        return isNaN(_p) || _p <= 0;
    } catch (e) {
        return 0;
    }
}


function warningClass(data) {
    removeValidationClass(data);
    $(data).parent().addClass("has-warning");
    $(data).addClass("form-control-warning");
}

function dangerClass(data) {
    removeValidationClass(data);
    $(data).parent().addClass("has-danger");
    $(data).addClass("form-control-danger");
}

function successClass(data) {
    removeValidationClass(data);
    $(data).parent().addClass("has-success");
    $(data).addClass("form-control-success");
}

function removeValidationClass(data) {
    $(data).removeClass("has-success");
    $(data).removeClass("has-danger");
    $(data).removeClass("has-warning");

    $(data).removeClass("form-control-success");
    $(data).removeClass("form-control-danger");
    $(data).removeClass("form-control-warning");

    $(data).parent().removeClass("has-success");
    $(data).parent().removeClass("has-danger");
    $(data).parent().removeClass("has-warning");
}


    function resetValidator(el) {
        $("[data-valmsg-for='" + el + "']").removeClass("field-validation-error").addClass("field-validation-valid").html("");
    }

    function setValidatorError(el, error) {
        $("[data-valmsg-for='" + el + "']").removeClass("field-validation-valid").addClass("field-validation-error").html(error);
    }

