//pino tuzzolino
$(document).ready(function () {

    $("form").attr("autocomplete", "off");
    $("form").attr("list", "autocompleteOff");

    $("input").each(function (e) {

        if ($(this).data("val") == undefined) {
            return;
        }

        $(this).change(function () {
            $(this).valid();
        });

        if ($(this).data("val-length-max") != undefined)
            $(this).attr("maxlength", $(this).data("val-length-max"));

        if ($(this).data("val-maxlength-max") != undefined)
            $(this).attr("maxlength", $(this).data("val-maxlength-max"));

        $(this).focus(function () {
            $(this).on("click", function (e) {
                $(this).select();
            });
        });

        $(this).attr("autocomplete", "off");
        $(this).attr("list", "autocompleteOff");

    });
});

(function ($) {
    $.fn.toCurrency = function () {
        return this.focusout(function () {
            var language = navigator.language;
            var _val = $(this).val();
            //alert(language);
            if (language == "it-IT" || language == "it")
                _val = $(this).val().replace('.', ',')

            _val = Globalize.parseFloat(_val).toFixed(2);

            if (String(Globalize.parseFloat(_val)) == "NaN") {
                $(this).val("0,00");
                return;
            }

            var _floor = String(_val).split('.');//  Math.floor(_val);

            var _euro = _floor[0];

            var _decimal = _floor[1];

            var _euroArray = Array.from(String(_euro)).reverse();

            var _points = Math.round(_euro.length / 3);

            _points = _points * 3 == _euro.length ? _points - 1 : _points;

            var _result = "";

            for (var i = 0; i < _euroArray.length; i++) {

                if (i % 3 == 0 && i > 0)
                    _result = String(_euroArray[i]) + "." + String(_result);
                else
                    _result = String(_euroArray[i]) + String(_result);

            }

            _result = _result + "," + _decimal;

            $(this).val(_result);

        });
    };

}(jQuery));

(function ($) {

    $.fn.noAutocomplete = function () {
        $(this).focus(function () {
            //$(this).once("click keyup", function (e) {
            $(this).attr("autocomplete", "off");
            //});
        });
    };

}(jQuery));

(function ($) {
    $.fn.once = function (events, callback) {
        return this.each(function () {
            var myCallback = function (e) {
                callback.call(this, e);
                $(this).off(events, myCallback);
            };
            $(this).on(events, myCallback);
        });
    };
}(jQuery));

(function ($) {
    $.fn.noSubmit = function (options) {

        // Default params
        //var params = $.extend({
        //    text: 'Default Title',
        //    fontsize: 10,
        //}, options);
        //return $(this).text(params.text);

        $(this).bind("keydown", function (e) {
            if (e.keyCode === 13)
                return false;
        });

    }
}(jQuery));

function toBoolean(val) {
    return Boolean(String(val).toLowerCase() == "true");
}

function checkRadio(val, data, el) {
    //setTimeout(function () {
    processCheckRadio(val, data, el);
    //}, 0);
}

function processCheckRadio(val, data, el) {
    try {

        // 

        if (data != undefined && data != "") {

            //$(data).removeClass("has-success");
            //$(data).removeClass("has-danger");

            //$(data).removeClass("form-control-success");
            //$(data).removeClass("form-control-danger");

            //$(data).parent().removeClass("has-success");
            //$(data).parent().removeClass("has-danger");

            //$(data).parent().addClass("has-warning");
            //$(data).addClass("form-control-warning");

            removeValidationClass(data);
            warningClass(data);
        }

        if (!val) {

            if (el != undefined && el != "")
                $(el).fadeOut();

            if (data != undefined && data != "") {

                $(data).each(function () {
                    if ($(this).prop("type") == "radio" || $(this).prop("type") == "checkbox") {
                        $(this).removeAttr("checked");
                        $(this).prop("checked", "");
                    }

                    if ($(this).prop("type") == "text" || $(this).prop("type") == "password") {
                        $(this).val("");
                    }
                });

                $(data).addClass("disabled");
                $(data).prop("disabled", "disabled");

            }

        }
        else {
            if (data != undefined && data != "") {
                $(data).removeClass("disabled");
                $(data).removeAttr("disabled");
            }

            if (el != undefined && el != "")
                $(el).fadeIn();
        }

    } catch (e) {
        alert(e.description);
    }
}

function disableInput(input) {
    $(input).addClass("disabled");
    $(input).prop('readonly', true);
}

function disableInputRemove(input) {
    $(input).removeClass("disabled");
    $(input).removeAttr('readonly');
}

//$("[data-val-number]").each(function (item) {

//    $(item).attr("data-val-number", "aaaa");
//})




