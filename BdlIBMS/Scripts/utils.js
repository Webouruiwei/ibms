// 获取指定名称的页面传递参数
function getQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]);
    return null;
}

// 错误提示框
function ShowErrModal(error, status) {
    ShowModal(2, error);
    if (status == 401) { // 如果未经授权则跳转到登录页面，其他情况则跳转到首页。
        $("#modelClose").click(function () {
            window.location.href = "../user/login";
        });
    }
}

// 消息提示框
function ShowTipsModel(tips) {
    ShowModal(0, tips);
}

// 信息提示框
// type 0:消息，1:警告，2:错误
function ShowModal(type, msg) {
    var title;
    switch (type) {
        case 0:
            title = '<i class="fa fa-comment"></i><span style="margin-left: 2px;">提示</span>';
            break;
        case 1:
            title = '<i class="fa fa-exclamation-triangle"></i><span style="margin-left: 2px;">警告</span>';
            break;
        case 2:
            title = '<i class="fa fa-times-circle"></i><span style="margin-left: 2px;">错误</span>';
            break;
        default:
            title = '<i class="fa fa-comment"></i><span style="margin-left: 2px;">提示</span>';
            break;
    }
    $("#model-msg-title").html(title);
    $("#showerrTitle").html(msg);
    $('#commontiperrModal').modal({
        backdrop: 'static',
        show: true
    });
}

// 确认提示框
function ShowConfirmModal(title, callback) {
    $("#showConfirmTitle").html(title);
    $('#commontipConfirmModal').modal({
        backdrop: 'static',
        show: true
    });
    $("#confirmPop").click(function () {
        $('#commontipConfirmModal').modal("hide");
        callback();
    });
}
