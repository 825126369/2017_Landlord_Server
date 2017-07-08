$(function () {

    $.jqPaginator('#pagination0', {
        totalPages: 100,
        visiblePages: 10,
        currentPage: 3,
        first: '<li class="first"><a href="javascript:;">首页</a></li>',
        prev: '<li class="prev"><a href="javascript:;">Previous</a></li>',
        next: '<li class="next"><a href="javascript:;">Next</a></li>',
        page: '<li class="page"><a href="javascript:;">{{page}}</a></li>',
        last: '<li class="last"><a href="javascript:;">尾页</a></li>',
        
        onPageChange: function (page, type) {
            //alert(page + "..." + type);

        }
    });

});