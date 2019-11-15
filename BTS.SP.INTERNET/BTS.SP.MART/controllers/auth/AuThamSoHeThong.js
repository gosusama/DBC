/*  
* Người tạo : Nguyễn Tuấn Hoàng Anh
* View: BTS.SP.MART/views/htdm/AuThamSoHeThong
* Menu: Danh mục-> Danh mục tham số hệ thống
*/
define(['ui-bootstrap', '/BTS.SP.MART/controllers/auth/AuthController.js'], function () {
    'use strict';
    var app = angular.module('AuThamSoHeThongModule', ['ui.bootstrap', 'authModule']);
    app.factory('AuThamSoHeThongService', ['$http', 'configService', function ($http, configService) {
        var serviceUrl = configService.rootUrlWebApi + '/Authorize/AuThamSoHeThong';
        var result = {
            postQuery: function (data) {
                return $http.post(serviceUrl + '/PostQuery', data);
            },
            post: function (data) {
                return $http.post(serviceUrl + '/Post', data);
            },
            update: function (params) {
                return $http.put(serviceUrl + '/' + params.id, params);
            },
            deleteItem: function (params) {
                return $http.delete(serviceUrl + '/' + params.id, params);
            }
        }
        return result;
    }]);
    /* controller list */
    app.controller('AuThamSoHeThongViewCtrl', ['$scope', '$location', '$http', 'configService', 'AuThamSoHeThongService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'securityService', 'toaster','userService','$state',
        function ($scope, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, securityService, toaster, serviceAuthUser, $state) {
            var currentUser = serviceAuthUser.GetCurrentUser();
            var unitCode = currentUser.unitCode;
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.filtered = angular.copy(configService.filterDefault);
            $scope.tempData = tempDataService.tempData;
            $scope.accessList = {};
            function filterData() {
                if ($scope.accessList.view) {
                    var postdata = { paged: $scope.paged, filtered: $scope.filtered };
                    service.postQuery(postdata).then(function (successRes) {
                        if (successRes && successRes.status == 200 && successRes.data && !successRes.data.Error) {
                            $scope.data = successRes.data.data.data;
                            $scope.paged.totalItems = successRes.data.data.totalItems;
                            console.log($scope.data);
                        } else {
                            $scope.data = [];
                            $scope.paged.totalItems = 0;
                            toaster.pop('error', "Lỗi:", successRes.data.message);
                        }
                    }, function (errorRes) {
                        toaster.pop('error', "Lỗi:", errorRes.statusText);
                    });
                }
            };
            function loadAccessList() {
                securityService.getAccessList('auThamSoHeThong').then(function (successRes) {
                    console.log('successRes', successRes);
                    if (successRes && successRes.status == 200) {
                        $scope.accessList = successRes.data;

                        if (!$scope.accessList.view) {
                            toaster.pop('error', "Lỗi:", "Không có quyền truy cập !");
                        } else {
                            filterData();
                        }
                    } else {
                        toaster.pop('error', "Lỗi:", "Không có quyền truy cập !");
                    }
                }, function (errorRes) {
                    console.log(errorRes);
                    toaster.pop('error', "Lỗi:", "Không có quyền truy cập !");
                    $scope.accessList = null;
                });
            }
            loadAccessList();
            $scope.setPage = function (pageNo) {
                $scope.paged.currentPage = pageNo;
                filterData();
            };
            $scope.sortType = 'makhachhang';
            $scope.sortReverse = false;
            $scope.doSearch = function () {
                $scope.paged.currentPage = 1;
                filterData();
            };
            $scope.pageChanged = function () {
                filterData();
            };
            $scope.refresh = function () {
                $scope.setPage($scope.paged.currentPage);
            };
            $scope.title = function () { return 'Tham số hệ thống' };


            $scope.displayHepler = function (paraValue, moduleName) {
                var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                if (data && data.length === 1) {
                    return data[0].text;
                } else {
                    return paraValue;
                }
            }
            $scope.setPage = function (pageNo) {
                $scope.paged.currentPage = pageNo;
                filterData();
            };

            $scope.doSearch = function () {
                $scope.paged.currentPage = 1;
                filterData();
            };

            $scope.pageChanged = function () {
                filterData();
            };

            $scope.goHome = function () {
                $state.go('home');
            };

            $scope.refresh = function () {
                $scope.setPage($scope.paged.currentPage);
            };
            $scope.create = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    size: 'md',
                    templateUrl: configService.buildUrl('auth/AuThamSoHeThong', 'add'),
                    controller: 'AuThamSoHeThongCreate',
                    resolve: {}
                });
                modalInstance.result.then(function (updatedData) {
                    $scope.refresh();
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };
            $scope.update = function (item) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    size: 'md',
                    templateUrl: configService.buildUrl('auth/AuThamSoHeThong', 'update'),
                    controller: 'AuThamSoHeThongEditCtrl',
                    resolve: {
                        targetData: function () {
                            return item;
                        }
                    }
                });
                modalInstance.result.then(function (updatedData) {
                    $scope.refresh();
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };
            $scope.details = function (item) {
                $uibModal.open({
                    backdrop: 'static',
                    size: 'md',
                    templateUrl: configService.buildUrl('auth/AuThamSoHeThong', 'details'),
                    controller: 'AuThamSoHeThongDetails',
                    resolve: {
                        targetData: function () {
                            return item;
                        }
                    }
                });
            };
            /* Function Delete Item */
            $scope.deleteItem = function (event, target) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('auth/AuThamSoHeThong', 'delete'),
                    controller: 'AuThamSoHeThongDelete',
                    resolve: {
                        targetData: function () {
                            return target;
                        }
                    }
                });
                modalInstance.result.then(function (updatedData) {
                    $scope.refresh();
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };
            //change
            $scope.changeGiaTriThamSo = function (item) {
                if (item) {
                    $scope.object = {};
                    $scope.object.id = item.id;
                    $scope.object.maThamSo = item.maThamSo;
                    $scope.object.tenThamSo = item.tenThamSo;
                    if (item.giaTriThamSo) $scope.object.giaTriThamSo = 1;
                    else $scope.object.giaTriThamSo = 0;
                    $scope.object.kieuDuLieu = item.kieuDuLieu;
                    $scope.object.isEdit = item.isEdit;
                    $scope.object.unitCode = unitCode;
                    $scope.object.iCreateBy = item.iCreateBy;
                    $scope.object.iCreateDate = item.iCreateDate;
                    service.update($scope.object).then(function (successRes) {
                        console.log(successRes);
                        if (successRes && successRes.status == 200 && successRes.data.data) {
                            toaster.pop('success', "Thông báo", "Thành công", 1000);
                        } else {
                            toaster.pop('error', "Lỗi:", "Xảy ra lỗi khi cập nhật");
                        }
                    }, function (errorRes) {
                        console.log(errorRes);
                        toaster.pop('error', "Lỗi:", "Xảy ra lỗi khi cập nhật");
                    });
                }
            };
        }]);

    /* controller Details */
    app.controller('AuThamSoHeThongDetails', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'AuThamSoHeThongService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify) {
            $scope.config = angular.copy(configService);
            $scope.tempData = tempDataService.tempData;
            $scope.target = targetData;
            $scope.isLoading = false;
            $scope.title = function () { return 'Thông tin Tham số hệ thống' };
            $scope.displayHepler = function (paraValue, moduleName) {
                var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                if (data && data.length === 1) {
                    return data[0].text;
                } else {
                    return paraValue;
                }
            }
            $scope.cancel = function () {
                $uibModalInstance.close();
            };

        }]);
    /* controller Create */
    app.controller('AuThamSoHeThongCreate', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'AuThamSoHeThongService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'toaster',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, toaster) {
            $scope.config = angular.copy(configService);
            $scope.tempData = tempDataService.tempData;
            $scope.target = {};
            $scope.title = function () { return 'Thêm Tham số hệ thống' };
            $scope.save = function () {
                service.post($scope.target).then(function (successRes) {
                    if (successRes && successRes.status == 200) {
                        toaster.pop('success', "Thông báo", successRes.data.message, 2000);
                        $uibModalInstance.close(successRes.data.data);
                    } else {
                        toaster.pop('error', "Lỗi:", successRes.data.message);
                    }
                }, function (errorRes) {
                    console.log(errorRes);
                    toaster.pop('error', "Lỗi:", errorRes.statusText);
                });
            };
            $scope.cancel = function () {
                $uibModalInstance.close();
            };
        }]);

    /* controller Create */
    app.controller('AuThamSoHeThongEditCtrl', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'AuThamSoHeThongService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify) {
            $scope.config = angular.copy(configService);
            $scope.title = function () { return 'Cập nhập Tham số hệ thống' };
            $scope.tempData = tempDataService.tempData;
            $scope.target = angular.copy(targetData);
            $scope.save = function () {
                service.update($scope.target).then(function (successRes) {
                    if (successRes && successRes.status == 200) {
                        toaster.pop('success', "Thông báo", successRes.data.message, 2000);
                        $uibModalInstance.close(successRes.data.data);
                    } else {
                        toaster.pop('error', "Lỗi:", successRes.statusText);
                    }
                }, function (errorRes) {
                    console.log(errorRes);
                    toaster.pop('error', "Lỗi:", errorRes.statusText);
                });
            };
            $scope.cancel = function () {
                $uibModalInstance.close();
            };
        }]);
    /* controller delete */
    app.controller('AuThamSoHeThongDelete', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'AuThamSoHeThongService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify) {
            $scope.config = angular.copy(configService);
            $scope.isLoading = false;
            $scope.title = function () { return 'Xoá thành phần'; };
            $scope.save = function () {
                service.deleteItem(targetData).then(function (successRes) {
                    if (successRes && successRes.status === 200) {
                        ngNotify.set("Xóa thành công", { type: 'success' });
                        $uibModalInstance.close(targetData);
                    } else {
                        ngNotify.set(successRes.data.message, { duration: 3000, type: 'error' });
                    }
                },
                function (errorRes) {
                    console.log('errorRes', errorRes);
                });
            };
            $scope.cancel = function () {
                $uibModalInstance.close();
            };
        }]);
    return app;
});

