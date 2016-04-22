var app = angular.module('app', [], function ($httpProvider) {
    for (var index in activeIndexs) {
        $("#" + activeIndexs[index]).addClass("active");
    }

    // Use x-www-form-urlencoded Content-Type 修改请求头
    $httpProvider.defaults.headers.post['Content-Type'] = 'application/x-www-form-urlencoded;charset=utf-8';

    /**
     * jquery.post会把Data序列化成字符串而angular不会，这里进行手动转换
     * The workhorse; converts an object to x-www-form-urlencoded serialization.
     * @param {Object} obj
     * @return {String}
     */
    var param = function (obj) {
        var query = '', name, value, fullSubName, subName, subValue, innerObj, i;

        for (name in obj) {
            value = obj[name];

            if (value instanceof Array) {
                for (i = 0; i < value.length; ++i) {
                    subValue = value[i];
                    fullSubName = name + '[' + i + ']';
                    innerObj = {};
                    innerObj[fullSubName] = subValue;
                    query += param(innerObj) + '&';
                }
            }
            else if (value instanceof Object) {
                for (subName in value) {
                    subValue = value[subName];
                    fullSubName = name + '[' + subName + ']';
                    innerObj = {};
                    innerObj[fullSubName] = subValue;
                    query += param(innerObj) + '&';
                }
            }
            else if (value !== undefined && value !== null)
                query += encodeURIComponent(name) + '=' + encodeURIComponent(value) + '&';
        }

        return query.length ? query.substr(0, query.length - 1) : query;
    };

    // Override $http service's default transformRequest
    $httpProvider.defaults.transformRequest = [function (data) {
        return angular.isObject(data) && String(data) !== '[object File]' ? param(data) : data;
    }];
});

// 页面渲染完毕执行的自定义回调
app.directive('onFinishRenderFilters', function ($timeout) {
    return {
        restrict: 'A',
        link: function (scope, element, attr) {
            if (scope.$last === true) {
                $timeout(function () {
                    scope.$emit('ngRepeatFinished');
                });
            }
        }
    };
});

// 该控制器针对布局页面
app.controller('layoutCtrl', function ($scope, $http) {
    getModules();
    getUserSession();

    // 获取系统所有模块
    function getModules() {
        $http({
            method: "get",
            withCredentials: true,
            url: "../api/modules"
        }).success(function (data, status, headers, config) {
            $scope.LayoutModules = data.Items;
        }).error(function (data, status, headers, config) {
            ShowErrModal(data, status);
        });
    }

    // 获取用户信息
    function getUserSession() {
        $http({
            method: "get",
            withCredentials: true,
            url: "../api/user"
        }).success(function (data, status, headers, config) {
            $scope.UserSession = data;
        }).error(function (data, status, headers, config) {
            ShowErrModal(data, status);
        });
    }

    // 退出系统
    $scope.logout = function () {
        ShowConfirmModal("确定要退出系统吗？", function () {
            $http({
                method: "post",
                withCredentials: true,
                url: "../api/users/logout"
            }).success(function (data, status, headers, config) {
                window.location.href = "../user/login";
            }).error(function (data, status, headers, config) {
                ShowErrModal(data, status);
            });
        });
    };

    // 检测该登录用户是否有访问当前点击模块系统的权限
    $scope.chkAccess = function (moduleUUID) {
        $http({
            method: "get",
            withCredentials: true,
            url: "../api/users/access/" + moduleUUID
        }).success(function (data, status, headers, config) {
            $scope.Access = data;
            if (!$scope.Access.CanRead) {
                ShowModal(1,"你没有该系统的访问权限！");
                return;
            } else {
                var url = "../system/index?UUID=" + moduleUUID;
                window.location.href = url;
            }
        }).error(function (data, status, headers, config) {
            ShowErrModal(data, status);
        });
    }
});