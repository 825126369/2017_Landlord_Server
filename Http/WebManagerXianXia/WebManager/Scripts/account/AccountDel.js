$(document).ready(function () {

    function check(id)
    {
        var str = $(id).val();
        if (str == "")
        {
            alert("不能为空");
            return false;
        }
            
        var res = confirm("确定要删除吗，删除后将不可恢复?");
        return res;
    }

    function setStartInfo(id, info)
    {
        $(id).text(info);
    }

    // 删除代理
    $("#btnDelAgency").click(function ()
    {
        var res = check("#txtAgency");
        if (res)
        {
            setStartInfo("#txtAgencyMsg", "正在进行操作，请稍候...");
            $.ajax({
                type: "GET",
                url: "/ashx/AccountDel.ashx",
                data: { acc: $("#txtAgency").val(), op: 0 },

                success: function (data) {
                    $("#txtAgencyMsg").text(data);
                }
            });
        }
    });

    // 删除会员
    $("#btnDelPlayer").click(function ()
    {
        var res = check("#txtPlayer");
        if (res) {
            setStartInfo("#txtPlayerMsg", "正在进行操作，请稍候...");
            $.ajax({
                type: "GET",
                url: "/ashx/AccountDel.ashx",
                data: { acc: $("#txtPlayer").val(), op: 1 },

                success: function (data) {
                    $("#txtPlayerMsg").text(data);
                }
            });
        }
    });

});


