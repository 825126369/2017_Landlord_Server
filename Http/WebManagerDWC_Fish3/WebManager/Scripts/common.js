define(function (require, exports, module) {

    function checkAjaxScript(data) {
        if (data.indexOf('<script>') >= 0) {

            var start = 8;
            var end = data.indexOf('</script>');
            var s = data.substring(start, end);
            eval(s);
        }
    }

    String.prototype.format = function () {
        var args = arguments;
        return this.replace(/\{(\d+)\}/g, function (s, i) {
            return args[i];
        });
    }

    exports.checkAjaxScript = checkAjaxScript;
});
