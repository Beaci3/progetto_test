$.validator.addMethod('checksumcfpiva', function (value, element, params) {

    if (params.required == "False") {
        if (value == "") {
            return true;
        }
    }

    var validi, i, s, set1, set2, setpari, setdisp;

    if (value == '' || value == undefined || value == null) {
        return false;
    }

    var cf = String(value).toUpperCase();

    if (params.requiredpivaorcf == "True") {
        if (cf.length == 11) {
            if (new RegExp("[0-9]{11}").test(value)) {
                return true;
            }
        }
    }

    if (cf.length != 16)
        return false;

    validi = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    for (i = 0; i < 16; i++) {
        if (validi.indexOf(cf.charAt(i)) == -1)
            return false;
    }

    set1 = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    set2 = "ABCDEFGHIJABCDEFGHIJKLMNOPQRSTUVWXYZ";
    setpari = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    setdisp = "BAKPLCQDREVOSFTGUHMINJWZYX";
    s = 0;
    for (i = 1; i <= 13; i += 2)
        s += setpari.indexOf(set2.charAt(set1.indexOf(cf.charAt(i))));
    for (i = 0; i <= 14; i += 2)
        s += setdisp.indexOf(set2.charAt(set1.indexOf(cf.charAt(i))));
    if (s % 26 != cf.charCodeAt(15) - 'A'.charCodeAt(0))
        return false;

    return true;
});

$.validator.unobtrusive.adapters.add("checksumcfpiva", ["requiredpivaorcf", "required"], function (options) {
    options.rules["checksumcfpiva"] = options.params;
    options.messages["checksumcfpiva"] = options.message;
});
