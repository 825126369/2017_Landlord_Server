$(document).ready(function ()
{
    var sel = $("#MainContent_m_accList").get(0).selectedIndex;
    show(sel);

    $("#MainContent_m_accList").change(function ()
    {
        var v = $(this).get(0).selectedIndex;
        show(v);
    });

    /////////////////////////////////////////////////    
    var val = $('input:radio[name="ctl00$MainContent$content"]:checked').val();
    toggleOpContent(val);

    $(":radio").click(function ()
    {
        var v = $(this).val();
        toggleOpContent(v);
    });
});

function show(sel)
{
    if (sel == 0)
    {
        $(".cOriPwd").show();
    }
    else
    {
        $(".cOriPwd").hide();
    }
}

function toggleOpContent(val)
{
    if (val == 0)  // 登录密码
    {
        $(".cOpLoginPwd").show();
        $(".cOpVerCode").hide();

        $("#m_verCode1").val("");
        $("#m_verCode2").val("");
    }
    else // 四位固定验证码
    {
        $(".cOpLoginPwd").hide();
        $(".cOpVerCode").show();

        $("#m_pwd1").val("");
        $("#m_pwd2").val("");
    }

    $("#m_oriPwd").val("");
}



