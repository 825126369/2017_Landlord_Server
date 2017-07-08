$(document).ready(function () {

    var g_isProgress = false;

    function setStartInfo(id, info) {
        $(id).text(info);
    }

    $("#btnCreateSub").click(function () {
     
        if (g_isProgress)
            return;

        Page_ClientValidate();

        if (!WebForm_OnSubmit())
            return;

        g_isProgress = true;
        setStartInfo("#MainContent_m_res", "正在进行操作，请稍候...");

        $.ajax({
            type: "POST",
            url: "/ashx/AccountCreateSubAcc.ashx",
            data: { pwd: $("#MainContent_m_pwd1").val(), name: $("#MainContent_m_aliasName").val() },

            success: function (data) {
                $("#MainContent_m_res").text(data);

                g_isProgress = false;
            },

            error: function () { g_isProgress = false;}
        });
    });

});


//////////////////////////////////////////////////////////
