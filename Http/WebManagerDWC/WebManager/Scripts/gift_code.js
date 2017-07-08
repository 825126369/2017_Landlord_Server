/*
        礼包码相关
*/

// 提交激活码参数
function submitGiftCodeParam()
{
    // 传给服务器的信息
    var code = document.getElementById("MainContent_operation_common_m_codeInfo");
    if (code == null)
    {
        addErrorInfo("unknown error");
        return;
    }
    
    var clientInfo = document.getElementById("MainContent_operation_common_m_clientInfo").value;
    // 长度，ID列表
    var arr = clientInfo.split(",");
    if(arr.length == 0)
        return;

    var intArr = new Array();
    for (i = 0; i < arr.length; i++)
    {
        intArr[i] = parseInt(arr[i], 10);
    }

    code.value = "";
    for (i = 0; i < intArr[0]; i++)
    {
        var txt = document.getElementById("CodeNum" + i);
        var retCode = isValidCount(txt.value);
        if (retCode == 2)
        {
            addErrorInfo("param invalid");
            return;
        }
        
        if (retCode == 0)
        {
            var plat = document.getElementById("MainContent_operation_common_Plat" + i);
            // 礼包ID，平台,个数
            code.value = code.value + intArr[i + 1] + "," + plat.selectedIndex + "," + txt.value + ";";
        }
    }

    if (code.value == "") // 不提交
        return;

    var f = document.getElementById("Form1");
    f.submit();
}

// 返回0成功, 1不添加,2非法
function isValidCount(val)
{
    if (isNaN(val))
        return 2;

    var res = parseInt(val);
    if (res <= 0)
        return 1;

    return 0;
}

function addErrorInfo(errInfo)
{
    var sp = document.getElementById("MainContent_operation_common_m_res");
    sp.innerText = errInfo;
}
