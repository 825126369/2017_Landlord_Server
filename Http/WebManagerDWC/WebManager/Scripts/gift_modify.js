/*
        礼包修改相关
*/

var DATE_TIME = new RegExp("^\\s*(\\d{4})/(\\d{1,2})/(\\d{1,2})\\s*$");

// 以空格相隔的两个数字
var TWO_NUM_BY_SPACE = new RegExp("^\\s*(\\d+)\\s+(\\d+)\\s*$");

// 以空格相隔的两个数字
var TWO_NUM_BY_SPACE_SEQ = new RegExp("^(\\s*(\\d+)\\s+(\\d+)\\s*)(;\\s*(\\d+)\\s+(\\d+)\\s*)+$");

// 提交礼包修改参数
function submitGiftModifyParam()
{
    // 传给服务器的信息
    var code = document.getElementById("MainContent_operation_common_m_modifyInfo");
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
        // 礼包内容
        var content = document.getElementById("MainContent_operation_common_Content" + i);
        if (!isContentValid(content.value))
        {
            addErrorInfo("param invalid");
            return;
        }

        // 截止日期
        var deadTime = document.getElementById("DeadTime" + i);
        if (!isDateTimeValid(deadTime.value))
        {
            addErrorInfo("param invalid");
            return;
        }

        // 礼包id, 内容，载止日期
        code.value = code.value + intArr[i + 1] + "@" + content.value + "@" + deadTime.value + "#";
    }

    if (code.value == "") // 不提交
        return;

    var f = document.getElementById("Form1");
    f.submit();
}

// 返回true合法，false非法
function isContentValid(val)
{
    if (TWO_NUM_BY_SPACE.test(val))
        return true;

    if (TWO_NUM_BY_SPACE_SEQ.test(val))
        return true;

    return false;
}

// 返回true合法，false非法
function isDateTimeValid(val)
{
    return DATE_TIME.test(val);
}

function addErrorInfo(errInfo)
{
    var sp = document.getElementById("MainContent_operation_common_m_res");
    sp.innerText = errInfo;
}
