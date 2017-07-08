define(function (require, exports, module) {

    function checkAjaxScript(data) {
        if (data.indexOf('<script>') >= 0) {

            var start = 8;
            var end = data.indexOf('</script>');
            var s = data.substring(start, end);
            eval(s);
            return true;
        }
        return false;
    }

    exports.checkAjaxScript = checkAjaxScript;
});