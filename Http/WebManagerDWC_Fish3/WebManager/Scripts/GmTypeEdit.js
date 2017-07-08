define(function (require, exports, module) {
$(function () {

    var cdata = require('./cdata.js');
    var common = require('./common.js');

    var BTN_ID = 0;

    function init()
    {
        doOp(cdata.opTypeDef.OP_VIEW, '', processGmList);

        $('#btnAdd').click(function () {

            doOp(cdata.opTypeDef.OP_ADD, $('#gmType').val(), addOneType);

        });
    }

    function processGmList(data)
    {
        console.log(data);
        var html = '<tr><td>类型名称</td> <td>新名称</td> <td></td> <td></td> </tr>';
        var $tr = $(html);
        $tr.appendTo($('#gmList'));

        var d = JSON.parse(data);

        var gmList = d.gmList;
        for(var i = 0; i < gmList.length; i++)
        {
            addOneRow(gmList[i].typeName, gmList[i].id);
        }
    }

    function addOneType(data)
    {
        console.log(data);
        var d = JSON.parse(data);
        if(d.result==0)
        {
            addOneRow(d.acc, d.id);
        }
    }

    function addOneRow(acc, id)
    {
        var str = '<tr id="{0}"><td>{1}</td> <td><input type="text" id="{2}"/></td> <td><input type="button" id="{3}" value="修改" /></td> <td><input type="button" id="{4}" value="删除" {5} /></td> </tr>';
        var btnModify = "m" + BTN_ID;
        var btnDel = "d" + BTN_ID;
        var newName = 'n' + BTN_ID;

        BTN_ID++;

        var delDesc = '';
        if (id.length < 24)
        {
            delDesc = 'disabled=disabled';
        }
        var html = str.format(id, acc, newName, btnModify, btnDel, delDesc);
        var $tr = $(html);
        $tr.appendTo($('#gmList'));
        
        if (delDesc)
        {
            $('#' + btnDel).css('opacity', '0.5');
        }

        $('#' + btnModify).click(function () {

            doOp(cdata.opTypeDef.OP_MODIFY, getGmType(this), function (data) {
                console.log(data);
                modifyGmType(data);
            },

            $('#' + newName).val());

        });

        $('#' + btnDel).click(function () {

            var res = confirm('可能会影响到此类型的GM账号, 确定删除该类型?');
            if (!res)
                return;

            doOp(cdata.opTypeDef.OP_REMOVE, getGmType(this), function (data) {

                removeGmType(data);
            });
        });
    }

    function getGmType(btn)
    {
        return $(btn).parent().parent().attr('id');
    }

    function doOp(op, param, callBack, param1)
    {
        console.log(param + '....' +param1);

        $.ajax({
            type: "POST",
            url: "/ashx/GmTypeEdit.ashx",
            data: { "op": op, "param": param, 'newValue': param1 },

            success: function (data) {
                callBack(data);
            }
        });
    }

    function removeGmType(data) {

        var d = JSON.parse(data);

        if (d.result == 0) {
            $('#gmList').find('tr[id=' + d.id + ']').remove();
        }
    }

    function modifyGmType(data) {

        var d = JSON.parse(data);

        if (d.result == 0) {
            $('#gmList').find('tr[id=' + d.id + ']').find('td').eq(0).html(d.newValue);
        }
    }

    init();
});

});
