var myApi = function () { }

myApi.prototype.postwithform = function (options) {

    $.ajax({

        url: options.url,
        data: options.form.serialize(),
        type: 'POST',
        success: options.success,
        error: function (result) {
            alert("error yields, read console")
            console.error(result)
            //$("#errorDisplay").show();
            //$("#errorDisplay").html(result.responseText);
        }
    });
}

myApi.prototype.postwithdata = function (options) {

    $.ajax({
        url: options.url,
        data: options.data,
        type: 'POST',
        contentType: "application/json; charset=utf-8",
        success: options.success,
        error: function (result) {

            console.error(result);
            //$("#errorDisplay").show();
            //$("#errorDisplay").html(result.responseText);
        }
    });
}

myApi.prototype.renderpartialview = function (options) {
    $.ajax({
        url: options.url,
        type: 'GET',
        success: function (data) {
            var container = options.container;
            var func = options.func;
            container.empty();
            container.html(data);
            if (func != null)
                func();
        }
    });
}

myApi.prototype.handleError400 = function (options) {
    alert("http response 400 with error")
}

myApi.prototype.handleError = function (result) {
    if (result.error)
        alert(result.error)
}


////////following methods are used for generic paing, sorting and filtering
function SetShouldReOrder() {
    $('#ShouldReOrder').val(true);
}

function ShouldResetPageIndex() {
    $('#ShouldResetPageIndex').val(true);
}