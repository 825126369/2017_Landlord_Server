$(document).ready(function () {

    $("#MainContent_m_result input[type=button]").click(function () {

        var trElem = $(this).parent().parent();
        var accGm = trElem.children(":first").text();
        var tdChk = trElem.children().eq(1);
        var rightStr = "";

        tdChk.children("input[type=checkbox]").each(function () {
            if ($(this).is(':checked'))
            {
                rightStr += $(this).val() + ",";
            }
        });

        $.ajax({
            type: "POST",
            url: "/ashx/AccountRightAssign.ashx",
            data: { acc: accGm, rstr: rightStr },

            success: function (data) {
                alert(data);
            }
        });

    });



});