
//$("[data-val-requiredspan], [data-val-required]").each(function (i) {

//    //var a = $("label[for='" + $(this).attr("name") + "']").html();

//    //if (a.indexOf("*") == -1)
//    //    alert(a.indexOf("*"));

//    $("label[for='" + $(this).attr("name") + "']").append("<span class='text-danger'> *</span>");
//});

$().ready(function () {
    for (var i = 0; i < $("[data-val-requiredspan]").length; i++) {
        var id = $("[data-val-requiredspan]")[i].id;

        var label = $("label[for='" + id + "']").html();

        if (label != undefined) {
            if (label.indexOf("*") == -1)
                $("label[for='" + id + "']").append("<span class='text-danger'> *</span>");
        }
    }

    for (var i = 0; i < $("[data-val-requiredoptional]").length; i++) {
        var id = $("[data-val-requiredoptional]")[i].id;

        var label = $("label[for='" + id + "']").html();

        var _idr = $('#' + id).data('val-requiredoptional-isrequiredfield');

        if (label != undefined) {
            if (label.indexOf("*") == -1)
                if (String($("#" + _idr).val()).toLowerCase() == "true")
                    $("label[for='" + id + "']").append("<span class='text-danger'> *</span>");
        }
    }

    for (var i = 0; i < $("[data-val-required]").length; i++) {
        var id = $("[data-val-required]")[i].id;

        var label = $("label[for='" + id + "']").html();

        if (label != undefined) {
            if (label.indexOf("*") == -1)
                $("label[for='" + id + "']").append("<span class='text-danger'> *</span>");
        }
    }
});
