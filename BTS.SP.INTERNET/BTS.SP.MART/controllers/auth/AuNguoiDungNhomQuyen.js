/*  
* Người tạo : Nguyễn Tuấn Hoàng Anh
* View: BTS.SP.MART/views/auth/AuNguoiDungNhomQuyen
* Menu: Danh mục-> Au người dùng nhóm quyền
*/
define(['ui-bootstrap'], function () {
    'use strict';
    var app = angular.module('AuNguoiDungNhomQuyenModule', ['ui.bootstrap']);
    app.factory('AuNguoiDungNhomQuyenService', ['$http', 'configService', function ($http, configService) {
        var serviceUrl = configService.rootUrlWebApi + '/Authorize/AuNguoiDungNhomQuyen';
        var result = {
            post: function (data) {
                return $http.post(serviceUrl + '/Post', data);
            },
            deleteItem: function (data) {
                return $http.delete(serviceUrl + '/' + data.ID);
            },
            getByUsername: function (data) {
                return $http.get(serviceUrl + '/GetByUsername/' + data);
            },
            config: function (data) {
                return $http.post(serviceUrl + '/Config', data);
            }
        }
        return result;
    }]);
    /* controller list */
    app.controller('AuNguoiDungNhomQuyenCreateCtrl', ['$scope', '$location', '$http', '$uibModalInstance', 'configService', 'AuNguoiDungNhomQuyenService', 'AuNhomQuyenService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'securityService', 'toaster', 'targetData',
        function ($scope, $location, $http,$uibModalInstance, configService, service, serviceNhomQuyen, tempDataService, $filter, $uibModal, $log, ngNotify, securityService, toaster, targetData) {
            $scope.config = {
                label: angular.copy(configService.label)
            };
            $scope.title = "Thêm mới phân quyền";
            $scope.data = [];
            $scope.lstNhomQuyen = [];
            $scope.lstAdd = [];
            $scope.lstDelete = [];
            function loadNhomQuyenByUser() {
                service.getByUsername(targetData.username).then(function (successRes) {
                    if (successRes && successRes.status == 200) {
                        $scope.data = successRes.data.data;
                        return $scope.data;
                    } else {
                        toaster.pop('error', "Lỗi:", successRes.data.message);
                        return null;
                    }
                }, function (errorRes) {
                    console.log(errorRes);
                    toaster.pop('error', "Lỗi:", errorRes.statusText);
                }).then(function (data) {
                    if (data) {
                        serviceNhomQuyen.getNhomQuyenConfig(targetData.username).then(function (successRes) {
                            if (successRes && successRes.status == 200) {
                                $scope.lstNhomQuyen = successRes.data.data;
                            } else {
                                toaster.pop('error', "Lỗi:", successRes.data.message);
                            }
                        }, function (errorRes) {
                            console.log(errorRes);
                            toaster.pop('error', "Lỗi:", errorRes.statusText);
                        });
                    }
                });
            }
            loadNhomQuyenByUser();
            $scope.selectNhomQuyen = function (item) {
                var obj = {
                    username: targetData.username,
                    manhomquyen: item.value,
                    tennhomquyen: item.text
                };
                $scope.lstAdd.push(obj);
                var filteredData = $filter('filter')($scope.lstNhomQuyen, { value: item.value }, true);
                if (filteredData && filteredData.length > 0) {
                    var index = $scope.lstNhomQuyen.indexOf(filteredData[0]);
                    if (index != -1) $scope.lstNhomQuyen.splice(index, 1);
                }
            };
            $scope.deSelectNhomQuyen = function (item) {
                $scope.lstNhomQuyen.push({
                    value: item.manhomquyen,
                    text: item.tennhomquyen
                });
                var filteredData = $filter('filter')($scope.data, { manhomquyen: item.manhomquyen }, true);
                if (filteredData && filteredData.length > 0) {
                    var index = $scope.data.indexOf(filteredData[0]);
                    if (index != -1) $scope.data.splice(index, 1);
                    if (filteredData[0].id) {
                        $scope.lstDelete.push({
                            id: filteredData[0].id,
                            manhomquyen: filteredData[0].manhomquyen,
                            username: filteredData[0].username
                        });
                    }
                }
            };
            $scope.deSelectNhomQuyenAdd = function (item) {
                $scope.lstNhomQuyen.push({
                    value: item.manhomquyen,
                    text: item.tennhomquyen
                });
                var filteredData = $filter('filter')($scope.lstAdd, { manhomquyen: item.manhomquyen }, true);
                if (filteredData && filteredData.length > 0) {
                    var index = $scope.lstAdd.indexOf(filteredData[0]);
                    if (index != -1) $scope.lstAdd.splice(index, 1);
                }
            };
            $scope.save = function () {
                var obj = {
                    username: targetData.username,
                    LstAdd: $scope.lstAdd,
                    LstDelete: $scope.lstDelete
                }
                service.config(obj).then(function (successRes) {
                    if (successRes && successRes.status == 200) {
                        toaster.pop('success', "Thông báo", successRes.data.message, 2000);
                        $uibModalInstance.close(successRes.data.Data);
                    } else {
                        toaster.pop('error', "Lỗi:", successRes.data.message);
                    }
                }, function (errorRes) {
                    console.log(errorRes);
                    toaster.pop('error', "Lỗi:", errorRes.statusText + errorRes.data.message);
                });
            };
            $scope.cancel = function () {
                $uibModalInstance.close();
            };
        }]);
    return app;
});

