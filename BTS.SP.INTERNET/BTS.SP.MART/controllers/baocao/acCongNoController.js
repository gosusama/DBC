/*  
* Người tạo : Nguyễn Tuấn Hoàng Anh
* View: BTS.SP.MART/views/nv/NvNhapHangMua
* Vm sevices: BTS.API.SERVICE -> NV ->NvNhapHangMuaVm.cs
* Sevices: BTS.API.SERVICE -> NV -> NvNhapHangMuaService.cs
* Entity: BTS.API.ENTITY -> NV - > NvNhapHangMua.cs
* Menu: Nghiệp vụ-> Nhập hàng mua
*/
define(['ui-bootstrap', '/BTS.SP.MART/controllers/auth/AuthController.js', '/BTS.SP.MART/controllers/htdm/periodController.js', '/BTS.SP.MART/controllers/htdm/merchandiseController.js', '/BTS.SP.MART/controllers/htdm/customerController.js', '/BTS.SP.MART/controllers/htdm/merchandiseTypeController.js', '/BTS.SP.MART/controllers/htdm/nhomVatTuController.js', '/BTS.SP.MART/controllers/htdm/supplierController.js', '/BTS.SP.MART/controllers/htdm/wareHouseController.js', '/BTS.SP.MART/controllers/htdm/packagingController.js', '/BTS.SP.MART/controllers/htdm/taxController.js', '/BTS.SP.MART/controllers/htdm/donViTinhController.js', '/BTS.SP.MART/controllers/htdm/typeReasonController.js'], function () {
    'use strict';
    var app = angular.module('acCongNoModule', ['ui.bootstrap', 'authModule', 'periodModule', 'merchandiseModule', 'customerModule', 'merchandiseTypeModule', 'nhomVatTuModule', 'supplierModule', 'wareHouseModule', 'packagingModule', 'taxModule', 'donViTinhModule', 'typeReasonModule']);
    app.factory('acCongNoService', ['$http', 'configService', function ($http, configService) {
        var serviceUrl = configService.rootUrlWebApi + '/Nv/CongNo';
        var rootUrl = configService.rootUrlWebApi;
        var result = {
            getUnitUsers: function () {
                return $http.get(rootUrl + '/Authorize/AuDonVi/GetSelectDataByUnitCode');
            },
            getNewParameter: function () {
                return $http.get(serviceUrl + '/GetNewParameter');
            }
        }
        return result;
    }]);
    /* controller list */
    app.controller('acCongNoController', [
        '$scope', '$location', '$http', 'configService', 'acCongNoService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'securityService', '$rootScope', 'toaster', 'customerService', 'supplierService', '$state', 'userService', 'AuDonViService', 'periodService',
        function ($scope, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, securityService, $rootScope, toaster, serviceCustomer, serviceSupplier, $state, serviceAuthUser, serviceAuDonVi, servicePeriod) {
            var currentUser = serviceAuthUser.GetCurrentUser();
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.filtered = angular.copy(configService.filterDefault);
            $scope.tempData = tempDataService.tempData;
            $scope.target = {};
            $scope.params = {};
            $scope.accessList = {};
            $scope.listBaoCao = [
                {
                    value: '0',
                    text: 'Thu Khách hàng',
                    name: 'congNoKhachHang'
                },
                {
                    value: '1',
                    text: 'Trả Nhà cung cấp',
                    name: 'congNoNCC'
                },
            ];
            $scope.title = "Báo cáo Công nợ ";

            $scope.changeLoaiBaoCao = function (loaiBc) {
                if (loaiBc) {
                    var data = $filter('filter')($scope.listBaoCao, { value: loaiBc }, true);
                    if (data && data.length > 0) {
                        $scope.target.moduleName = data[0].name;
                    }
                    if (loaiBc == '0') {
                        $scope.title = "Báo cáo Công nợ thu Khách hàng";

                    } else if (loaiBc == '1') {
                        $scope.title = "Báo cáo Công nợ trả Nhà cung cấp";
                    }
                }
            };

            //load dữ liệu
            function filterData() {
                service.getUnitUsers().then(function (response) {
                    if (response && response.status == 200 && response.data) {
                        $scope.unitUsers = response.data;
                    }
                });
                service.getNewParameter().then(function (response) {
                    $scope.target = response.data;
                    //$scope.target.fromDate = new Date($scope.target.fromDate);
                    $scope.target.toDate = new Date($scope.target.toDate);
                    $scope.target.loaiBaoCao = '0';
                    $scope.changeLoaiBaoCao($scope.target.loaiBaoCao);
                });
                servicePeriod.getKyKeToan().then(function (response) {
                    if (response) {
                        $scope.target.fromDate = new Date(response.data.fromDate);
                    }
                });
            };
            //end
            //Nhà cung cấp
            $scope.selectNhaCungCap = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Supplier', 'selectData'),
                    controller: 'supplierSelectDataController',
                    resolve: {
                        serviceSelectData: function () {
                            return serviceSupplier;
                        },
                        filterObject: function () {
                            return {
                                advanceData: {
                                }
                            }
                        }
                    }
                });
                modalInstance.result.then(function (updatedData) {
                    if (updatedData != null) {
                        var output = '';
                        angular.forEach(updatedData, function (item, index) {
                            output += item.value + ',';
                        });
                        $scope.nhaCungCapCodes = output.substring(0, output.length - 1);
                    }
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };
            /////////
            //Khách hàng
            $scope.selectCustomer = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Customer', 'selectData'),
                    controller: 'customerSelectDataController',
                    resolve: {
                        serviceSelectData: function () {
                            return serviceCustomer;
                        },
                        filterObject: function () {
                            return {
                                advanceData: {
                                }
                            }
                        }
                    }
                });
                modalInstance.result.then(function (updatedData) {
                    if (updatedData != null) {
                        var output = '';
                        angular.forEach(updatedData, function (item, index) {
                            output += item.value + ',';
                        });
                        $scope.customerCodes = output.substring(0, output.length - 1);
                    }
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };
            // end filter

            $scope.displayHepler = function (paraValue, moduleName) {
                var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                if (data && data.length === 1) {
                    return data[0].text;
                } else {
                    return paraValue;
                }
            }
            //check quyền truy cập
            function loadAccessList() {
                securityService.getAccessList('acCongNo').then(function (successRes) {
                    if (successRes && successRes.status === 200) {
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
            //end
            //load danh muc

            function loadSupplier() {
                if (!tempDataService.tempData('suppliers')) {
                    serviceSupplier.getAll_Supplier().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                            tempDataService.putTempData('suppliers', successRes.data.data);
                            $scope.suppliers = successRes.data.data;
                        } else {
                            console.log('successRes', successRes);
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.suppliers = tempDataService.tempData('suppliers');
                }
            }
            loadSupplier();
            //end


            function loadModeReport(ctrl, moduleName) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    size: 'lg',
                    windowClass: 'class-window',
                    templateUrl: configService.buildUrl('baocao', 'reports.template'),
                    controller: ctrl,
                    resolve: {
                        loadModule: ['$ocLazyLoad', '$q', function ($ocLazyLoad, $q) {
                            var deferred = $q.defer();
                            require(['/BTS.SP.MART/controllers/baocao/' + moduleName], function (module) {
                                deferred.resolve();
                                $ocLazyLoad.inject(module.name);
                            });
                            return deferred.promise;
                        }],
                        obj: function () {
                            return $scope.params;
                        }
                    }
                });
                modalInstance.result.then(function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            }

            function convertToArrayCondition(records) {
                var result = '';
                if (records) {
                    if (records.indexOf(',') !== -1) {
                        var arr = records.split(',');
                        if (arr && arr.length > 0) {
                            angular.forEach(arr, function (value, key) {
                                result = result + '\'' + value + '\'' + ',';
                            });
                            result = result.substr(0, result.length - 1);
                        }
                    } else {
                        result = '\'' + records + '\'';
                    }
                }
                return result;
            };


            //nếu value = 0 :Tổng hợp -- value = 1: Chi tiết
            function initializeParam(targetData, value) {
                console.log('targetData', targetData);
                $scope.params.P_TUNGAY = $filter('date')(targetData.fromDate, 'yyyy-MM-dd');
                $scope.params.P_DENNGAY = $filter('date')(targetData.toDate, 'yyyy-MM-dd');
                $scope.params.P_NHACUNGCAP = convertToArrayCondition($scope.nhaCungCapCodes);
                $scope.params.P_MAKHACHHANG = convertToArrayCondition($scope.customerCodes);
                $scope.params.P_UNITCODE = targetData.unitCode;
                $scope.params.P_USERNAME = currentUser.userName;
                console.log('$scope.params', $scope.params);
                if (value === 0) $scope.params.P_TITLE = ($scope.title + " tổng hợp").toUpperCase();
                else if (value === 1) $scope.params.P_TITLE = ($scope.title + " chi tiết").toUpperCase();
            }
            $scope.report = function () {
                if ($scope.target.loaiBaoCao == "-1") {
                    ngNotify.set("Chưa chọn loại báo cáo", { duration: 3000, type: 'error' });
                } else if ($scope.target.unitCode == "-1") {
                    ngNotify.set("Chưa chọn đơn vị", { duration: 3000, type: 'error' });
                } else {
                    var modulename = "acCongNoController";
                    var controllerRp = $scope.target.moduleName + "ReportController";
                    $scope.target.TENBAOCAO = '';
                    initializeParam($scope.target, 0);
                    loadModeReport(controllerRp, modulename);
                }
            };

            $scope.reportDetails = function () {
                if ($scope.target.loaiBaoCao == "-1") {
                    ngNotify.set("Chưa chọn loại báo cáo", { duration: 3000, type: 'error' });
                } else if ($scope.target.unitCode == "-1") {
                    ngNotify.set("Chưa chọn đơn vị", { duration: 3000, type: 'error' });
                } else {
                    var modulename = "acCongNoController";
                    var controllerRp = $scope.target.moduleName + "DetailsReportController";
                    $scope.target.TENBAOCAO = '';
                    initializeParam($scope.target, 1);
                    loadModeReport(controllerRp, modulename);
                }
            };

        }]);
    app.controller('congNoKhachHangReportController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'obj',
    function ($scope, $uibModalInstance, $location, $http, configService, obj) {
        $scope.para = angular.copy(obj);
        console.log($scope.para);
        $scope.cancel = function () {
            $uibModalInstance.close();
        };
        $scope.report = {
            name: "BTS.SP.API.Reports.CONGNO.CNKH_TONGHOP,BTS.SP.API",
            title: $scope.para.TENBAOCAO,
            params: $scope.para
        }
    }]);
    app.controller('congNoKhachHangDetailsReportController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'obj',
    function ($scope, $uibModalInstance, $location, $http, configService, obj) {
        $scope.para = angular.copy(obj);
        console.log($scope.para);
        $scope.cancel = function () {
            $uibModalInstance.close();
        };
        $scope.report = {
            name: "BTS.SP.API.Reports.CONGNO.CNKH_CHITIET,BTS.SP.API",
            title: $scope.para.TENBAOCAO,
            params: $scope.para
        }
    }]);
    app.controller('congNoNCCReportController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'obj',
    function ($scope, $uibModalInstance, $location, $http, configService, obj) {
        $scope.para = angular.copy(obj);
        console.log($scope.para);
        $scope.cancel = function () {
            $uibModalInstance.close();
        };
        $scope.report = {
            name: "BTS.SP.API.Reports.CONGNO.CNNCC_TONGHOP,BTS.SP.API",
            title: $scope.para.TENBAOCAO,
            params: $scope.para
        }
    }]);
    app.controller('congNoNCCDetailsReportController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'obj',
    function ($scope, $uibModalInstance, $location, $http, configService, obj) {
        $scope.para = angular.copy(obj);
        console.log($scope.para);
        $scope.cancel = function () {
            $uibModalInstance.close();
        };
        $scope.report = {
            name: "BTS.SP.API.Reports.CONGNO.CNNCC_CHITIET,BTS.SP.API",
            title: $scope.para.TENBAOCAO,
            params: $scope.para
        }
    }]);
    return app;
});