define(['angular', '/BTS.SP.MART/controllers/auth/AuthController.js'], function () {
    //interceptorService.$inject = ['$q', '$injector', '$location', '$log'];
    var app = angular.module('InterceptorModule', ['authModule']);
    app.factory('interceptorService', ['$q', '$injector', '$location', '$log', 'userService', '$state', function ($q, $injector, $location, $log, userService, $state) {


        var arrDate = ['ngayCT', 'ngayHoaDon', 'fromDate', 'toDate', 'ngayDacBiet', 'ngaySinh', 'ngayHetHan', 'ngayCapThe'];
        function convertDate(inputFormat) {
            function pad(s) { return (s < 10) ? '0' + s : s; }
            if (inputFormat == null) {
                return null;
            }
            var d = new Date(inputFormat);
            return [d.getFullYear(), pad(d.getMonth() + 1), pad(d.getDate())].join('-');

        }

        var interceptorServiceFactory = {};
        var _request = function (request) {
            request.headers = request.headers || {};
            var currentUser = userService.GetCurrentUser();
            if (currentUser != null) {
                request.headers.Authorization = 'Bearer ' + currentUser.access_token;
            }
            return request;
        }
        var _response = function (res) {
            if (res.data && res.data.data && res.data.status) {
                var object;
                try {
                    object = JSON.parse(JSON.stringify(res.data.data));
                } catch (e) {
                    object = res.data.data;
                }
                angular.forEach(object, function (value, key) {
                    if (arrDate.indexOf(key) != -1) {
                        object[key] = convertDate(object[key]);
                        res.data = object;
                    }
                });
            }
            return res;
        }
        var _requestError = function (request) {
            return request
        }
        var _responseError = function (rejection) {
            if (rejection.status === 401) {
                console.log('AccessDenied :', rejection);
                $state.go('login');
            } else {
            }
            return $q.reject(rejection);
        }
        interceptorServiceFactory.request = _request;
        interceptorServiceFactory.response = _response;
        interceptorServiceFactory.requestError = _requestError;
        interceptorServiceFactory.responseError = _responseError;
        return interceptorServiceFactory;
    }]);
    return app;
});