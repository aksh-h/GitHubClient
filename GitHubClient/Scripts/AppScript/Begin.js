$(document).ready(function () {
    $('#start').click(function () {
        var token = $('#token').val();
        $.ajax({
            url: "../Account/StartProcess",
            type: "GET",
            data: { token: token },
            success: function (da) {

            },
            error: function (xhr) {
                console.log(xhr);
            }
        });
    });
});
