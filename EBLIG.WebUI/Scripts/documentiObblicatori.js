
$.validator.addMethod('documentiobblicatori', function (value, element, params) {
    if (value != null && value != undefined && value != "") {
        var _a = String(value).split(',');
        var _b = String($("#" + params.allegatiidselinput).val()).split(',');

        for (var i = 0; i < _b.length; i++) {

            for (var ii = 0; ii < _a.length; ii++) {
                if (_a[ii] == _b[i]) {
                    _a.splice(_a.findIndex(f => f == _a[ii]), 1);
                }
            }
        }

        if (_a.length > 0) {
            $("span[data-valmsg-for='AllegatiId']").show();
            return false;
        }
    }

    return true;

});

$.validator.unobtrusive.adapters.add("documentiobblicatori", ["allegatiidselinput"], function (options) {
    options.rules["documentiobblicatori"] = options.params;
    options.messages["documentiobblicatori"] = options.message;
});

