$(document).ready(function ()
{
    var id = "#MainContent_m_type";
    //var sel = $(id).get(0).selectedIndex;
    var sel = $(id).val();
    show(sel);

    $(id).change(function ()
    {
        // var v = $(this).get(0).selectedIndex;
        var v = $(this).val();
        show(v);
    });
});

function show(sel)
{
    if (sel == 2) // 2下级代理   2是选中的值
    {
        $(".cAgency").show();
        $(".cAPI").hide();
        addPostfix(false);

        apiPrefixData(false);
    }
    else if(sel == 3) // 3 api
    {
        $(".cAgency").hide();
        $(".cAPI").show();
        addPostfix(false);

        apiPrefixData(true);
    }
    else // 4子账号
    {
        $(".cAgency").hide();
        $(".cAPI").hide();
        $(".cSub").show();
        addPostfix(true);

        apiPrefixData(false);
    }
}

function addPostfix(add)
{
    var prefixId = "#MainContent_m_prefix";
    var txt = $(prefixId).text(); // 需要修改
    var ii = txt.indexOf('#');
    if(add) // 添加
    {
        if (ii < 0)
        {
            $(prefixId).text(txt + "#");
        }
    }
    else
    {
        if (ii >= 0)
        {
            var t = txt.replace("#", '');
            $(prefixId).text(t);
        }
    }
}

// 操作API前缀
function apiPrefixData(empty)
{
    if(empty)
    {
        $("#m_apiPrefix").val("");
    }
    else
    {
        $("#m_apiPrefix").val("123");
    }
}


