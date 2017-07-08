$(document).ready(function () {

    $("#btnAdd").click(function () {

        $.ajax({
            type: "POST",
            url: "/ashx/ChampionSetting.ashx",
            data: { op: 1, param: $("#playerId").val() },

            success: function (data) {
                process(data);
            }
        });
    });

    $("#btnRemove").click(function () {

        $.ajax({
            type: "POST",
            url: "/ashx/ChampionSetting.ashx",
            data: { op: 2, param: $("#playerId").val() },

            success: function (data) {
                process(data);
            }
        });
    });

    $("#btnModify").click(function () {

        $.ajax({
            type: "POST",
            url: "/ashx/ChampionSetting.ashx",
            data: { op: 3, param: $("#scorePlayerId").val(), score: $("#score").val() },

            success: function (data) {
                process(data);
            }
        });
    });

    function process(data)
    {
        var obj = JSON.parse(data);
       // console.log(obj);
        switch(obj.op)
        {
            case 1: // 添加
                {
                    if (obj.result != 0) {
                        alert(obj.resultStr);
                        return;
                    }

                    var tr = document.createElement("tr");
                    var td1 = document.createElement("td");
                    td1.innerText = obj.param;
                    var td2 = document.createElement("td");
                    td2.innerText = obj.nickName;
                    tr.appendChild(td1);
                    tr.appendChild(td2);
                    tr.id = obj.param;
                    document.getElementById("MainContent_operation_common_m_result").appendChild(tr);
                }
                break;
            case 2:
                {
                    if (obj.result == 0) {                        
                        var table = document.getElementById("MainContent_operation_common_m_result");
                        var tr = document.getElementById(obj.param);
                        if(tr != null)
                        {
                            table.removeChild(tr);
                        }
                    }
                    else
                    {
                        alert(obj.resultStr);
                    }
                }
                break;
            case 3:
                {
                    alert(obj.resultStr);
                }
                break;
        }
    }

});


