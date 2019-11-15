/*  
* Người tạo : Nguyễn Tuấn Hoàng Anh
* View: BTS.SP.MART/views/nv/NvNhapHangMua
* Vm sevices: BTS.API.SERVICE -> NV ->NvNhapHangMuaVm.cs
* Sevices: BTS.API.SERVICE -> NV -> NvNhapHangMuaService.cs
* Entity: BTS.API.ENTITY -> NV - > NvNhapHangMua.cs
* Menu: Nghiệp vụ-> Nhập hàng mua
*/
define(['ui-bootstrap', '/BTS.SP.MART/controllers/auth/AuthController.js', '/BTS.SP.MART/controllers/auth/AuNguoiDung.js', '/BTS.SP.MART/controllers/htdm/periodController.js', '/BTS.SP.MART/controllers/htdm/merchandiseController.js', '/BTS.SP.MART/controllers/htdm/merchandiseTypeController.js', '/BTS.SP.MART/controllers/htdm/nhomVatTuController.js', '/BTS.SP.MART/controllers/htdm/supplierController.js'], function () {
    'use strict';
    var app = angular.module('acNgayHetHanHangHoaModule', ['ui.bootstrap', 'authModule', 'AuNguoiDungModule', 'periodModule', 'merchandiseModule', 'merchandiseTypeModule', 'nhomVatTuModule', 'supplierModule']);
    app.factory('acNgayHetHanHangHoaService', ['$http', 'configService', function ($http, configService) {
        var serviceUrl = configService.rootUrlWebApi + '/Nv/NgayHetHanHangHoa';
        var rootUrl = configService.rootUrlWebApi;
        var rootUrlAcCloseout = configService.rootUrlWebApi + '/AC/Closeout';
        var result = {
            getUnitUsers: function () {
                return $http.get(rootUrl + '/Authorize/AuDonVi/GetSelectDataByUnitCode');
            },
            getNewParameter: function () {
                return $http.get(rootUrl + '/Nv/NgayHetHanHangHoa/GetNewParameter');
            },
            getLastPeriod: function () {
                return $http.get(rootUrlAcCloseout + '/GetLastPeriod');
            },
        }
        return result;
    }]);
    /* controller list */
    app.controller('acNgayHetHanHangHoaController', [
        '$scope', '$location', '$http', 'configService', 'acNgayHetHanHangHoaService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'securityService', '$rootScope', 'toaster', 'periodService', 'merchandiseService', 'merchandiseTypeService', 'nhomVatTuService', 'supplierService', '$state', 'userService', 'AuDonViService', 'AuNguoiDungService',
        function ($scope, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, securityService, $rootScope, toaster, servicePeriod, serviceMerchandise, serviceMerchandiseType, serviceNhomVatTu, serviceSupplier, $state, serviceAuthUser, serviceAuDonVi, AuNguoiDungService) {
            var currentUser = serviceAuthUser.GetCurrentUser();
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.filtered = angular.copy(configService.filterDefault);
            $scope.tempData = tempDataService.tempData;
            $scope.target = {};
            var date = [];
            $scope.accessList = {};
            $scope.params = {};
            $scope.options = {
                minDate: null,
                maxDate: null
            };
            $scope.groupBy = 'MADONVI';
            $scope.title = "Báo cáo phiếu ngày hết hạn hàng hóa";
            $scope.titleReport = "Báo cáo phiếu ngày hết hạn hàng hóa";
            //load dữ liệu
            function filterData() {
                service.getUnitUsers().then(function (response) {
                    if (response && response.status == 200 && response.data) {
                        $scope.unitUsers = response.data;
                    }
                });
                service.getNewParameter().then(function (response) {
                    if (response && response.status == 200 && response.data) {
                        $scope.target = response.data;
                        $scope.options.minDate = new Date(response.data.minDate);
                        $scope.options.maxDate = new Date(response.data.maxDate);
                        $scope.target.moduleName = 'ngayHetHanHangHoa';
                        date = new Date(response.data.maxDate);
                        $scope.target.fromDate = date.setDate(date.getDate() - 1);
                        $scope.target.toDate = new Date(response.data.maxDate);
                    }
                });
                service.getLastPeriod().then(function (response) {
                    if (response && response.status == 200 && response.data) {
                        $scope.lastPeriod = response.data;
                    }
                });

            };
            //end
            //Loại hàng
            $scope.selectMerchandiseType = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/MerchandiseType', 'selectData'),
                    controller: 'merchandiseTypeSelectDataController',
                    resolve: {
                        serviceSelectData: function () {
                            return serviceMerchandiseType;
                        },
                        filterObject: function () {
                            return {

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
                        $scope.merchandiseTypeCodes = output.substring(0, output.length - 1);
                    }
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });


            }
            //Hàng hóa
            $scope.selectMerchandise = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Merchandise', 'selectDataSimple'),
                    controller: 'merchandiseSimpleSelectDataController',
                    resolve: {
                        serviceSelectData: function () {
                            return serviceMerchandise;
                        },
                        filterObject: function () {
                            return {
                                isAdvance: true,
                                advanceData: { maLoaiVatTu: $scope.target.merchandiseTypeCodes, maNhomVatTu: $scope.target.merchandiseGroupCodes }
                            }
                        }
                    }
                });
                modalInstance.result.then(function (updatedData) {
                    var output = '';
                    angular.forEach(updatedData, function (item, index) {
                        output += item.value + ',';
                    });
                    $scope.merchandiseCodes = output.substring(0, output.length - 1);
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            }
            $scope.selectNguoiDung = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('auth/AuNguoiDung', 'selectData'),
                    controller: 'AuNguoiDungSelectDataController',
                    resolve: {
                        serviceSelectData: function () {
                            return AuNguoiDungService;
                        },
                        filterObject: function () {
                            return {
                                isAdvance: true,
                                advanceData: { unitCode: $scope.target.unitCode }
                            }
                        }
                    }
                });
                modalInstance.result.then(function (updatedData) {
                    var output = '';
                    angular.forEach(updatedData, function (item, index) {
                        output += item.value + ',';
                    });
                    $scope.userCodes = output.substring(0, output.length - 1);
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            }
            //Nhóm hàng
            $scope.selectMerchandiseGroup = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/NhomVatTu', 'selectData'),
                    controller: 'nhomVatTuSelectDataController',
                    resolve: {
                        serviceSelectData: function () {
                            return serviceNhomVatTu;
                        },
                        filterObject: function () {
                            return {
                                isAdvance: true,
                                advanceData: { maLoaiVatTu: $scope.target.merchandiseTypeCodes }
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
                        $scope.merchandiseGroupCodes = output.substring(0, output.length - 1);
                    }
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            }

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
            //filter by loai
            $scope.changeTypeMerchandiseCodes = function (inputTypeMer) {
                if (typeof inputTypeMer != 'undefined' && inputTypeMer !== '') {
                    serviceMerchandiseType.filterTypeMerchandiseCodes(inputTypeMer).then(function (response) {
                        if (response && response.status == 200 && response.data) {
                            $scope.data = response.data;
                            $scope.merchandiseTypeCodes = '';
                            $scope.merchandiseTypeCodes = $scope.data.maLoaiVatTu;
                        }
                        else {
                            //$scope.selectMerchandiseType();
                        }
                    });
                }
            }
            //filter by nhom hang hoa
            $scope.changeMerchandiseGroup = function (inputMerchandiseGroup) {
                if (typeof inputMerchandiseGroup != 'undefined' && inputMerchandiseGroup !== '') {
                    serviceNhomVatTu.filterGroupMerchandiseCodes(inputMerchandiseGroup).then(function (response) {
                        if (response && response.status == 200 && response.data) {
                            $scope.data = response.data;
                            $scope.merchandiseGroupCodes = '';
                            $scope.merchandiseGroupCodes = $scope.data.maNhom;
                        }
                        else {
                            //$scope.selectMerchandiseGroup();
                        }
                    });
                }
            }
            //filter by hang hoa
            $scope.changeMerchandise = function (inputMerchandise) {
                if (typeof inputMerchandise != 'undefined' && inputMerchandise !== '') {
                    serviceMerchandise.filterMerchandiseCodes(inputMerchandise).then(function (response) {
                        if (response && response.status == 200 && response.data) {
                            $scope.data = response.data;
                            $scope.merchandiseCodes = '';
                            $scope.merchandiseCodes = $scope.data.maVatTu;
                        }
                        else {
                            //$scope.selectMerchandise();
                        }
                    });
                }
            }

            //filter by nha cung cap
            $scope.changeNhaCungCap = function (inputNhaCungCap) {
                if (typeof inputNhaCungCap != 'undefined' && inputNhaCungCap !== '') {
                    serviceSupplier.filterNhaCungCap(inputNhaCungCap).then(function (response) {
                        console.log('response', response);
                        if (response && response.status == 200 && response.data) {
                            $scope.data = response.data;
                            $scope.nhaCungCapCodes = '';
                            $scope.nhaCungCapCodes = $scope.data.maNCC;
                        }
                        else {
                            //$scope.selectNhaCungCap();
                        }
                    });
                }
            }
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
                securityService.getAccessList('ngayHetHanHangHoa').then(function (successRes) {
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

            function loadNguoiDung() {
                if (!tempDataService.tempData('users')) {
                    AuNguoiDungService.getAll_NguoiDung().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.length > 0) {
                            tempDataService.putTempData('users', successRes.data);
                            $scope.users = successRes.data;
                        } else {
                            console.log('successRes', successRes);
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.users = tempDataService.tempData('users');
                }
            }

            function loadMerchandiseType() {
                if (!tempDataService.tempData('merchandiseTypes')) {
                    serviceMerchandiseType.getAll_MerchandiseType().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                            tempDataService.putTempData('merchandiseTypes', successRes.data.data);
                            $scope.merchandiseTypes = successRes.data.data;
                        } else {
                            console.log('successRes', successRes);
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.merchandiseTypes = tempDataService.tempData('merchandiseTypes');
                }
            }

            function loadNhomVatTu() {
                if (!tempDataService.tempData('nhomVatTus')) {
                    serviceNhomVatTu.getAll_NhomVatTu().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                            tempDataService.putTempData('nhomVatTus', successRes.data.data);
                            $scope.nhomVatTus = successRes.data.data;
                        } else {
                            console.log('successRes', successRes);
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.nhomVatTus = tempDataService.tempData('nhomVatTus');
                }
            }

            loadSupplier();
            loadNguoiDung();
            loadMerchandiseType();
            loadNhomVatTu();
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

            function initializeParam(targetData) {
                $scope.params.P_TUNGAY = $filter('date')($scope.target.fromDate, 'yyyy-MM-dd');
                $scope.params.P_DENNGAY = $filter('date')($scope.target.toDate, 'yyyy-MM-dd');
                $scope.params.P_GROUPBY = $scope.groupBy;
                $scope.params.P_MALOAI = convertToArrayCondition($scope.merchandiseTypeCodes);
                $scope.params.P_MANHOM = convertToArrayCondition($scope.merchandiseGroupCodes);
                $scope.params.P_MAVATTU = convertToArrayCondition($scope.merchandiseCodes);
                $scope.params.P_NHACUNGCAP = convertToArrayCondition($scope.nhaCungCapCodes);
                $scope.params.P_NGUOIDUNG = convertToArrayCondition($scope.userCodes);
                $scope.params.P_UNITCODE = targetData.unitCode;
                $scope.params.P_USERNAME = currentUser.userName;
                $scope.params.P_TITLE = $scope.titleReport.toUpperCase();
            }
            $scope.report = function () {
                if ($scope.target.unitCode === "-1") {
                    ngNotify.set("Chưa chọn đơn vị", { duration: 3000, type: 'error' });
                } else {
                    var modulename = $scope.target.moduleName + "Controller";
                    var controllerRp = $scope.target.moduleName + "ReportController";
                    $scope.target.TENBAOCAO = '';
                    initializeParam($scope.target);
                    loadModeReport(controllerRp, modulename);
                }
            };
        }]);
    return app;
});