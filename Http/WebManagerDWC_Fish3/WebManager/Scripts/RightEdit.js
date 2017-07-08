define(function (require, exports, module) {
$(function () {

    var g_right = {};
    var g_table = {};
    var common = require('./common.js');
    var g_btnModifyIdPrefix = 'btnModify-';

    var g_game = {
        'op': '运营相关',
        'svr': '客服相关',
        'td': '运营数据',
        'data': '数据统计',
        'fish': '经典捕鱼',
        'crod': '鳄鱼大亨',
        'dice': '骰宝',
        'bacc': '百家乐',
        'cow': '牛牛',
        'd5': '五龙',
        'shcd': '黑红梅方',
        'calf': '套牛',
        'other':'其他'
    };

    $("#gmTypeList").change(function () {
        initRightCheck($(this).val());
    });

    function init()
    {
        $.ajax({
            type: "POST",
            url: "/ashx/RightEdit.ashx",
            data: { "op": 0},

            success: function (data) {
                console.log(data);

                g_right = JSON.parse(data);
                console.log(g_right);

                createEditList();
                createGmTypeList();

                initAfter();
            }
        });
    }

    function initAfter()
    {
        $('#btnEditSelAll').click(function () {

            $('#editRight input[type=checkbox]').prop('checked', true);
        });

        $('#btnSubmitOnceKey').click(function () {

            $('span[id^=spanOpRes]').html('');

            $('input[id^=' + g_btnModifyIdPrefix).trigger('click');
        });

        $('#btnEditCancelSelAll').click(function () {

            $('#editRight input[type=checkbox]').prop('checked', false);
            //$('#editRight input[type=checkbox]').attr('disabled', false);
        });
    }

    function createEditList() {
        var rightList = g_right.rightList;

        for (var i = 0; i < rightList.length; i++)
        {
            createOneEditItem(rightList[i]);
        }
    }

    function createTableHead(table)
    {
        var str = '<tr>' +
            '<td>{0}</td>' +
            '<td>{1}</td>' +
            '<td>{2}</td>' +
            '<td>{3}</td>' +
            '<td>{4}</td>' +
            '</tr>';
        var html = str.format('权限名称', '可查看', '可编辑', '操作', '操作结果');
        $(html).appendTo(table);
    }

    function createOneEditItem(right)
    {
        var table = createTable(right);
        var str = '<tr id="{0}">' +
            '<td>{1}</td>' +
            '<td><input type="checkbox"  id="{2}"/></td>' +
            '<td><input type="checkbox"  id="{3}"/></td>' +
            '<td><input type="button" id="{4}" value="提交修改" rid="{5}"/></td>' +
            '<td><span id="{6}" class="text-success"></span></td>' +
            '</tr>';
        var html = str.format(right.rightId, right.rightName,
            'chkview' + right.rightId, 'chkedit' + right.rightId, g_btnModifyIdPrefix + right.rightId, right.rightId,
            'spanOpRes' + right.rightId);

        $(html).appendTo(table);

        var btnId = '#' + g_btnModifyIdPrefix + right.rightId;

        $(btnId).addClass('btn btn-primary');

        $(btnId).click(function () {

            var id = $(this).attr('rid');
            
            var rstr1 = $('#chkview' + right.rightId).is(':checked') ? '1' : '0';
            var rstr2 = $('#chkedit' + right.rightId).is(':checked') ? '1' : '0';
            if (rstr2 == '1')
            {
                rstr1 = '1';
            }
            var rstr = rstr1 + ',' + rstr2;
            
            reqModifyRight(right.rightId, rstr);
        });

        $('#chkedit' + right.rightId).click(function () {

            if($(this).is(':checked'))
            {
               // alert(right.rightId);
                $('#chkview' + right.rightId).prop('checked', true);
                //$('#chkview' + right.rightId).attr('disabled', true);
            }
            else
            {
                //$('#chkview' + right.rightId).attr('disabled', false);
            }
        });
    }

    function createTable(right)
    {
        var id = getTableKey(right);
        if (g_table[id])
            return g_table[id];
        
        // $('#editRight').append($t);
        var container = $('#div_' + id);
        var title = $('<h2>');
        title.html(g_game[id] ? g_game[id] : "")
        title.appendTo(container);
        title.css('text-align', 'center');

        var $t = $('<table>');
        g_table[id] = $t;
        container.append($t);

        //$t.addClass('cTable').addClass('table-bordered')
        $t.addClass('cTable');
        createTableHead($t);

        return $t;
    }

    function getTableKey(right)
    {
        if (g_game[right.category])
            return right.category;

        return '$1';
    }

    function createGmTypeList()
    {
        $sel = $('#gmTypeList');

        var List = g_right.gmTypeList;

        for(var i = 0; i < List.length; i++)
        {
            $op = $('<option>');
            $op.attr('value', List[i].gmTypeId);
            $op.html(List[i].gmTypeName);
            $op.appendTo($sel);
        }
        if (List.length > 0)
        {
            $sel.val(List[0].gmTypeId);
            initRightCheck(List[0].gmTypeId);
        }
        else
        {
            $('#editRight table').hide();
        }
    }

    function initRightCheck(gmType)
    {
        $('#editRight input[type=checkbox]').attr('checked', false); //.attr('disabled', false);

        var rightList = g_right.rightList;

        for (var i = 0; i < rightList.length; i++) {
         
            var id = rightList[i].rightId;
            var r1 = canEdit(gmType, id);
            var r2 = canView(gmType, id);

            $('#chkedit' + id).prop('checked', r1);
            $('#chkview' + id).prop('checked', r2);

            /*if(r1)
            {
                $('#chkview' + id).attr('disabled', true);
            }*/
        }
    }

    function canEdit(gmType, rightId)
    {
        var rightForGm = g_right[gmType];
        if (rightForGm == null)
            return false;

        var r = rightForGm[rightId];
        if (r == null)
            return false;

        return r.canEdit;
    }

    function canView(gmType, rightId) {
        var rightForGm = g_right[gmType];
        if (rightForGm == null)
            return false;

        var r = rightForGm[rightId];
        if (r == null)
            return false;

        return r.canView;
    }

    function reqModifyRight(rightId, rstr)
    {
        $('#spanOpRes' + rightId).html('提交中，请稍候...');

        $.ajax({
            type: "POST",
            url: "/ashx/RightEdit.ashx",
            data: { "op": 1, "rightId": rightId, 'gmType': $("#gmTypeList").val(), "rightStr": rstr },

            success: function (data) {
                common.checkAjaxScript(data);

                var d = JSON.parse(data);
                if(d.result == 0)
                {
                    modifyRight(d.gmType, d.rid, d.rstr);
                }
                //alert(d.resultStr);
                $('#spanOpRes' + d.rid).html(d.resultStr);
            }
        });
    }

    function modifyRight(gmType, rid, rstr)
    {
        var arr = rstr.split(',');
        var canView = arr[0] == '1' ? true : false;
        var canEdit = arr[1] == '1' ? true : false;

        var json = {};
        if(g_right[gmType])
        {
            json = g_right[gmType];
        }
        else
        {
            g_right[gmType] = json;
        }

        json[rid] = { 'canEdit': canEdit, 'canView': canView };
    }

    init();
});

});
