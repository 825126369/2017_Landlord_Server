$(document).ready(function () {

    var g_countDown = parseInt($("#idInputCountDown").val());
    var g_timer;
    g_timer = setInterval(countDown, 1000);
    $("#idCountDown").text(g_countDown);

    function countDown() {
        g_countDown--;
        if(g_countDown<=0)
        {
            clearInterval(g_timer);
            location.href = "/appaspx/LoginAccount.aspx";
        }

        $("#idCountDown").text(g_countDown);
        $("#idInputCountDown").val(g_countDown);
    }
});

