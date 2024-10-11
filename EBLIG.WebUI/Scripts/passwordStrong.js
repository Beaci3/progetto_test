
$.validator.addMethod('passwordstrong', function (value, element, params) {

    var _value = String(value).trim() == "" ? "" : value;
    var _hashValue = String(_value).trim() != "";

    var _RequiredLength = true;
    var _RequireNonLetterOrDigit = true;
    var _RequiredPattern = true;
    var _RequireLowercase = true;
    var _RequireUppercase = true;
    var _RequireDigit = true;

    var _text = "<li>Password e un campo obbligatorio</li>";
    var _ar = params.requiredpattern.split("");

    if (String(_value).trim() != "") {
        _text = "";

        if (params.requiredlength > 0) {
            //lunghezza minima password
            _RequiredLength = String(_value).trim().length >= params.requiredlength;
            if (!_RequiredLength) {
                _text += "<li>Password deve avere almeno " + params.requiredlength + " caratteri</li>";
            }
        }

        if (params.requirenonletterordigit) {
            //verifica carattere speciale

            _RequireNonLetterOrDigit = _value.replace(/[a-zA-Z0-9]/g, '').length > 0;
            if (!_RequireNonLetterOrDigit) {
                _text += "<li>Password deve avere un carattere speciale</li>";
            }
        }

        if (String(params.requiredpattern).trim() != "") {
            //verifica pattern

            $.each(_ar, function (index, item) {
                if (value.indexOf(item) != -1) {
                    _RequiredPattern = true;
                    return;
                }
            });

            if (!_RequiredPattern) {
                _text += "<li>Password deve contenere un carattere tra questi " + params.requiredpattern + "</li>";
            }
        }

        if (params.requirelowercase) {
            //verifica pattern
            _RequireLowercase = new RegExp("[a-z]").test(_value);
            if (!_RequireLowercase) {
                _text += "<li>Password deve avere almeno una lettera minuscola</li>";
            }
        }

        if (params.requireuppercase) {
            //verifica pattern
            _RequireUppercase = new RegExp("[A-Z]").test(_value);
            if (!_RequireUppercase) {
                _text += "<li>Password deve avere almeno una lettera maiuscola</li>";
            }
        }

        if (params.requiredigit) {
            //verifica numero
            _RequireDigit = new RegExp("[0-9]").test(_value);
            if (!_RequireDigit) {
                _text += "<li>Password deve avere almeno un numero</li>";
            }
        }
    }

    var _isValid = _hashValue && _RequiredLength && _RequireNonLetterOrDigit && _RequiredPattern && _RequireLowercase && _RequireUppercase && _RequireDigit;

    $("[data-valmsg-for='" + element.id + "']").html("");

    setTimeout(function () {
        if (!_isValid) {
            var _ul = "<ul class=\"field-validation-error\">" + _text + "</ul>";
            $("[data-valmsg-for='" + element.id + "']").html(_ul);
            $("[data-valmsg-for='" + element.id + "']").removeClass("field-validation-valid");
        }
    }, 0);

    return _isValid;
});

$.validator.unobtrusive.adapters.add("passwordstrong", ["requiredpattern", "requiredlength", "requiredigit", "requirelowercase", "requireuppercase", "requirenonletterordigit", "requiredlength"], function (options) {
    options.rules["passwordstrong"] = options.params;
    options.messages["passwordstrong"] = options.message;
});

