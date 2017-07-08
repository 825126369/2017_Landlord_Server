define(function (require, exports, module) {
$(function () {

    var oConfirmDlg = $('#_normalPopDlg');
    var oTitle = $('#_normalPopDlgTitle');
    var oContent = $('#_normalPopDlgContent');

    function doModal(title, content)
    {
        if (title == '' || title == null)
        {
            title = '结果提示';
        }

        oTitle.html(title);
        oContent.html(content);
        oConfirmDlg.modal();
    }

    exports.doModal = doModal;

});
});
