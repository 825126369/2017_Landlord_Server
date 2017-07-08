$(document).ready(function () {

    function check()
    {
        var res = confirm("确定要删除吗，删除后将不可恢复?");
        return res;
    }

    function setStartInfo(id, info)
    {
        $(id).text(info);
    }

    $("#delLog").click(function ()
    {
        var res = check();
        if (res)
        {
            setStartInfo("#m_res", "正在进行操作，请稍候...");
            $.ajax({
                type: "GET",
                url: "/ashx/AccountDel.ashx",
                data: { op: 2 },

                success: function (data) {
                    $("#m_res").text(data);
                }
            });
        }
    });

});


