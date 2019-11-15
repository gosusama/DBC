/*  
* Người tạo : Nguyễn Tuấn Hoàng Anh
* View: BTS.SP.MART/views/nv/NvNhapHangMua
* Vm sevices: BTS.API.SERVICE -> NV ->NvNhapHangMuaVm.cs
* Sevices: BTS.API.SERVICE -> NV -> NvNhapHangMuaService.cs
* Entity: BTS.API.ENTITY -> NV - > NvNhapHangMua.cs
* Menu: Nghiệp vụ-> Nhập hàng mua
*/
define(['ui-bootstrap', '/BTS.SP.MART/controllers/auth/AuthController.js', '/BTS.SP.MART/controllers/htdm/periodController.js', '/BTS.SP.MART/controllers/htdm/merchandiseController.js', '/BTS.SP.MART/controllers/htdm/customerController.js', '/BTS.SP.MART/controllers/htdm/merchandiseTypeController.js', '/BTS.SP.MART/controllers/htdm/nhomVatTuController.js', '/BTS.SP.MART/controllers/htdm/supplierController.js', '/BTS.SP.MART/controllers/htdm/wareHouseController.js', '/BTS.SP.MART/controllers/htdm/packagingController.js', '/BTS.SP.MART/controllers/htdm/taxController.js', '/BTS.SP.MART/controllers/htdm/donViTinhController.js'], function () {
    'use strict';
    var app = angular.module('inventoryModule', ['ui.bootstrap', 'authModule', 'periodModule', 'merchandiseModule', 'customerModule', 'merchandiseTypeModule', 'nhomVatTuModule', 'supplierModule', 'wareHouseModule', 'packagingModule', 'taxModule', 'donViTinhModule']);
    app.factory('inventoryService', ['$http', 'configService', function ($http, configService) {
        var serviceUrl = configService.rootUrlWebApi + '/Ac/Closeout';
        var rootUrl = configService.apiServiceBaseUri;
        var calc = {
            sum: function (obj, name) {
                var total = 0
                if (obj && obj.length > 0) {
                    angular.forEach(obj, function (v, k) {
                        var increase = v[name];
                        if (!increase) {
                            increase = 0;
                        }
                        total += increase;
                    });
                }
                return total;
            }
        }
        var result = {
            robot: calc,
            postReportInventoryTongHop: function (filter) {
                return $http.post(serviceUrl + '/PostReportInventoryTongHop', filter);
            },
            getWareHouseByUnit: function (maDonVi) {
                return $http.get(rootUrl + '/api/Md/WareHouse/GetByUnit/' + maDonVi);
            },
            getPeriodByUnit: function (maDonVi) {
                return $http.get(rootUrl + '/api/Md/Period/GetByUnit/' + maDonVi);
            },
            getNewParameter: function () {
                return $http.get(serviceUrl + '/GetNewParameter');
            },
            getUnitUsers: function () {
                return $http.get(rootUrl + '/api/Authorize/AuDonVi/GetSelectDataByUnitCode');
            },
            postExportExcelInventoryChiTiet: function (json, filename) {
                $http({
                    url: serviceUrl + '/PostExportExcelInventoryChiTiet',
                    method: "POST",
                    data: json, //this is your json data string
                    headers: {
                        'Content-type': 'application/json'
                    },
                    responseType: 'arraybuffer'
                }).success(function (data, status, headers, config) {
                    var a = document.createElement("a");
                    document.body.appendChild(a);
                    a.style = "display: none";
                    var blob = new Blob([data], { type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
                    var objectUrl = URL.createObjectURL(blob);
                    a.href = objectUrl;
                    a.download = filename + ".xlsx";
                    a.click();
                    // window.URL.revokeObjectURL(objectUrl);
                }).error(function (data, status, headers, config) {
                    //upload failed
                });

                //$http.post(serviceUrl + '/WriteDataToExcel', data);
            },
            getLastPeriod: function () {
                return $http.get(serviceUrl + '/GetLastPeriod');
            },
            postApproval: function (data) {
                return $http.post(configService.rootUrlWebApi + '/Md/Period/PostUpdateGiaoDich', data);
            },
            postExportExcelInventoryTongHop: function (json, filename) {
                $http({
                    url: serviceUrl + '/PostExportExcelInventoryTongHop',
                    method: "POST",
                    data: json, //this is your json data string
                    headers: {
                        'Content-type': 'application/json'
                    },
                    responseType: 'arraybuffer'
                }).success(function (data, status, headers, config) {
                    var a = document.createElement("a");
                    document.body.appendChild(a);
                    a.style = "display: none";
                    var blob = new Blob([data], { type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
                    var objectUrl = URL.createObjectURL(blob);
                    a.href = objectUrl;
                    a.download = filename + ".xlsx";
                    a.click();
                    // window.URL.revokeObjectURL(objectUrl);
                }).error(function (data, status, headers, config) {
                    //upload failed
                });

                //$http.post(serviceUrl + '/WriteDataToExcel', data);
            },
            postHangTonAm: function () {
                return $http.post(rootUrl + '/api/Md/Merchandise/PostHangTonAm');
            }
        }
        return result;
    }]);
    /* controller list */
    app.controller('inventoryController', [
        '$scope', '$location', '$http', 'configService', 'inventoryService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'securityService', '$rootScope', 'toaster', 'periodService', 'merchandiseService', 'customerService', 'merchandiseTypeService', 'nhomVatTuService', 'supplierService', 'wareHouseService', 'packagingService', 'taxService', 'donViTinhService', '$state', 'userService',
        function ($scope, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, securityService, $rootScope, toaster, servicePeriod, serviceMerchandise, serviceCustomer, serviceMerchandiseType, serviceNhomVatTu, serviceSupplier, serviceWareHouse, servicePackaging, serviceTax, serviceDonViTinh, $state, serviceAuthUser) {
            var currentUser = serviceAuthUser.GetCurrentUser();
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.filtered = angular.copy(configService.filterDefault);
            $scope.tempData = tempDataService.tempData;
            $scope.isEditable = true;
            $scope.target = {};
            $scope.accessList = {};
            $scope.params = {};
            $scope.options = {
                minDate: null,
                maxDate: null
            };
            $scope.listBaoCao = [
                {
                    value: '0',
                    text: 'Báo cáo tồn kho',
                    name: 'tonKho'
                },
                {
                    value: '1',
                    text: 'Báo cáo xuất, nhập, tồn',
                    name: 'xuatNhapTon'
                },
                 {
                     value: '2',
                     text: 'Báo cáo xuất, nhập, tồn chi tiết',
                     name: 'xuatNhapTonChiTiet'
                 }
            ];
            $scope.typeValues = [
                {
                    value: 'ALL',
                    text: 'Tất cả'
                },
                {
                    value: 'POSITIVE',
                    text: 'Hàng tồn dương'
                },
                {
                    value: 'NEGATIVE',
                    text: 'Hàng tồn âm'
                },
                {
                    value: 'ZERO',
                    text: 'Hàng tồn 0'
                }
            ];
            $scope.groupBy = 1;
            $scope.title = "Báo cáo tồn kho";
            $scope.titleReport = "Báo cáo tồn kho";
            $scope.changeLoaiBaoCao = function (loaiBc) {
                if (loaiBc) {
                    var data = $filter('filter')($scope.listBaoCao, { value: loaiBc }, true);
                    if (data && data.length > 0) {
                        $scope.target.moduleName = data[0].name;
                    }
                    if (loaiBc == '0') {
                        $scope.title = "Báo cáo tồn kho";
                        $scope.target.loaiBaoCao = '0';
                        $scope.titleReport = "Báo cáo tồn kho";

                    } else if (loaiBc == '1') {
                        $scope.title = "Báo cáo xuất, nhập, tồn";
                        $scope.target.loaiBaoCao = '1';
                        $scope.titleReport = "Báo cáo xuất, nhập, tồn";
                    } else if (loaiBc == '2') {
                        $scope.title = "Báo cáo xuất, nhập, tồn chi tiết";
                        $scope.target.loaiBaoCao = '2';
                        $scope.titleReport = "Báo cáo xuất, nhập, tồn chi tiết";
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
                    if (response && response.status == 200 && response.data) {
                        $scope.target = response.data;
                        //$scope.options.minDate = new Date(response.data.minDate);
                        $scope.target.typeValue = "ALL";
                        $scope.target.loaiBaoCao = '0';
                        $scope.target.moduleName = 'tonKho';

                        $scope.target.toDate = new Date(response.data.maxDate);
                    }
                });
                service.getLastPeriod().then(function (response) {
                    if (response && response.status == 200 && response.data) {
                        $scope.lastPeriod = response.data;
                    }
                });
                servicePeriod.getKyKeToan().then(function (response) {
                    if (response) {
                        $scope.target.fromDate = new Date(response.data.fromDate);
                    }
                });
            };
            //end
            //Kho hàng
            $scope.selectWareHouse = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/WareHouse', 'selectData'),
                    controller: 'wareHouseSelectDataController',
                    resolve: {
                        serviceSelectData: function () {
                            return serviceWareHouse;
                        },
                        filterObject: function () {
                            return {
                                advanceData: {
                                    unitCode: $scope.target.unitCode,
                                },
                                isAdvance: true
                            }
                        }
                    }
                });
                modalInstance.result.then(function (updatedData) {
                    var output = '';
                    angular.forEach(updatedData, function (item, index) {
                        output += item.value + ',';
                    });
                    $scope.wareHouseCodes = output.substring(0, output.length - 1);
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            }

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
                    var output = '';
                    angular.forEach(updatedData, function (item, index) {
                        output += item.value + ',';
                    });
                    $scope.merchandiseTypeCodes = output.substring(0, output.length - 1);
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
                    var output = '';
                    angular.forEach(updatedData, function (item, index) {
                        output += item.value + ',';
                    });
                    $scope.merchandiseGroupCodes = output.substring(0, output.length - 1);
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
                    var output = '';
                    angular.forEach(updatedData, function (item, index) {
                        output += item.value + ',';
                    });
                    $scope.nhaCungCapCodes = output.substring(0, output.length - 1);
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };

            $scope.updatePrice = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Period', 'updatePrice'),
                    controller: 'updatePriceController',
                    resolve: {
                        targetData: function () {
                            return $scope.target;
                        }
                    }
                });
            };

            $scope.getWareHouseImportByUnit = function (code) {
                if (code) {
                    service.getPeriodByUnit(code).then(function (response) {
                        if (response && response.status == 200 && response.data) {
                            $scope.periods = response;
                        }
                    });
                    service.getWareHouseByUnit(code).then(function (response) {
                        if (response && response.status == 200 && response.data) {
                            $scope.wareHouses = response;
                        }
                    });
                };
            }
            //filter by kho
            //filter by kho
            $scope.changewareHouseCodes = function (inputwareHouse) {
                if (typeof inputwareHouse != 'undefined' && inputwareHouse !== '') {
                    serviceWareHouse.filterWareHouse(inputwareHouse).then(function (response) {
                        if (response && response.status == 200 && response.data) {
                            $scope.data = response.data;
                            $scope.wareHouseCodes = '';
                            $scope.wareHouseCodes = $scope.data.maKho;
                        }
                        else {
                            // $scope.selectWareHouse();
                        }
                    });
                }
            }
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
                        if (response && response.status == 200 && response.data && response.data.status) {
                            $scope.data = response.data.data;
                            $scope.nhaCungCapCodes = '';
                            $scope.nhaCungCapCodes = $scope.data.mancc;
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
                securityService.getAccessList('inventory').then(function (successRes) {
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

            function loadWareHouse() {
                if (!tempDataService.tempData('wareHouses')) {
                    serviceWareHouse.getAll_WareHouse().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                            tempDataService.putTempData('wareHouses', successRes.data.data);
                            $scope.wareHouses = successRes.data.data;
                        } else {
                            console.log('successRes', successRes);
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.wareHouses = tempDataService.tempData('wareHouses');
                }
            }

            function loadPackagings() {
                if (!tempDataService.tempData('packagings')) {
                    servicePackaging.getAll_Packaging().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                            tempDataService.putTempData('packagings', successRes.data.data);
                            $scope.packagings = successRes.data.data;
                        } else {
                            console.log('successRes', successRes);
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.packagings = tempDataService.tempData('packagings');
                }
            }

            function loadTax() {
                if (!tempDataService.tempData('taxs')) {
                    serviceTax.getAll_Tax().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                            tempDataService.putTempData('taxs', successRes.data.data);
                            $scope.taxs = successRes.data.data;
                        } else {
                            console.log('successRes', successRes);
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.taxs = tempDataService.tempData('taxs');
                }
            }

            function loadDonViTinh() {
                if (!tempDataService.tempData('donViTinhs')) {
                    serviceDonViTinh.getAll_DonViTinh().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                            tempDataService.putTempData('donViTinhs', successRes.data.data);
                            $scope.donViTinhs = successRes.data.data;
                        } else {
                            console.log('successRes', successRes);
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.donViTinhs = tempDataService.tempData('donViTinhs');
                }
            }
            loadSupplier();
            loadMerchandiseType();
            loadNhomVatTu();
            loadWareHouse();
            loadPackagings();
            loadTax();
            loadDonViTinh();
            //end

            if ($state.params.obj != null) {
                var param = $state.params.obj;
                $scope.target = angular.copy(param);
                $scope.wareHouseCodes = param.wareHouseCodes;
                $scope.merchandiseTypeCodes = param.merchandiseTypeCodes;
                $scope.merchandiseGroupCodes = param.merchandiseGroupCodes;
                $scope.merchandiseCodes = param.merchandiseCodes;
                $scope.nhaCungCapCodes = param.nhaCungCapCodes;
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
                            require(['/BTS.SP.MART/controllers/baocao/' + moduleName + '.js'], function (module) {
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
            //nếu value = 0 :Tổng hợp -- value = 1: Chi tiết
            function initializeParam(targetData, value, controllerRp, modulename) {
                var targetObj = angular.copy(targetData);
                targetObj.fromDate = $filter('date')(targetObj.fromDate, 'yyyy-MM-dd');
                targetObj.toDate = $filter('date')(targetObj.toDate, 'yyyy-MM-dd');
                $scope.params.tempTuNgay = $scope.target.fromDate;
                $scope.params.tempDenNgay = $scope.target.toDate;
                $scope.params.P_TUNGAY = new Date(Date.UTC(targetData.fromDate.getFullYear(), targetData.fromDate.getMonth(), targetData.fromDate.getDate()));
                $scope.params.P_DENNGAY = new Date(Date.UTC(targetData.toDate.getFullYear(), targetData.toDate.getMonth(), targetData.toDate.getDate()));

                $scope.params.P_DKIENLOC = targetData.typeValue;
                $scope.params.P_GROUPBY = $scope.groupBy;
                $scope.params.P_MAKHO = convertToArrayCondition($scope.wareHouseCodes);
                $scope.params.P_MALOAI = convertToArrayCondition($scope.merchandiseTypeCodes);
                $scope.params.P_MANHOM = convertToArrayCondition($scope.merchandiseGroupCodes);
                $scope.params.P_MAVATTU = convertToArrayCondition($scope.merchandiseCodes);
                $scope.params.P_NHACUNGCAP = convertToArrayCondition($scope.nhaCungCapCodes);
                $scope.params.P_UNITCODE = targetData.unitCode;
                $scope.params.P_USERNAME = currentUser.userName;
                if ($scope.selectedVAT) {
                    $scope.params.P_SELECTEDVAT = '1';
                } else {
                    $scope.params.P_SELECTEDVAT = '0';
                }
                if (value === 0) $scope.params.P_TITLE = ($scope.titleReport + " tổng hợp").toUpperCase();
                else if (value === 1) $scope.params.P_TITLE = ($scope.titleReport + " chi tiết").toUpperCase();
                if (targetData.xuatNhapTon === "xuatNhapTon" || targetData.xuatNhapTon === "xuatNhapTonChiTiet") {

                }
                servicePeriod.getTableNameByDate(targetObj).then(function (response) {
                    if (response && response.status == 200 && response.data) {
                        $scope.params.P_TABLE_NAME = response.data.data;
                    }
                    return 1;
                }).then(function (ok) {
                    loadModeReport(controllerRp, modulename);
                });
                //return true;

            }
            $scope.report = function () {
                if ($scope.target.loaiBaoCao == "-1") {
                    ngNotify.set("Chưa chọn loại báo cáo", { duration: 3000, type: 'error' });
                } else if ($scope.target.unitCode == "-1") {
                    ngNotify.set("Chưa chọn đơn vị", { duration: 3000, type: 'error' });
                } else if ($scope.target.typeValue == "-1") {
                    ngNotify.set("Chưa chọn điều kiện lọc", { duration: 3000, type: 'error' });
                } else {
                    $scope.target.wareHouseCodes = $scope.wareHouseCodes;
                    $scope.target.TENBAOCAO = '';
                    var modulename = $scope.target.moduleName + "Controller";
                    var controllerRp = $scope.target.moduleName + "ReportController";
                    initializeParam($scope.target, 0, controllerRp, modulename);
                }
            };

            $scope.reportDetails = function () {
                if ($scope.target.loaiBaoCao == "-1") {
                    ngNotify.set("Chưa chọn loại báo cáo", { duration: 3000, type: 'error' });
                } else if ($scope.target.unitCode == "-1") {
                    ngNotify.set("Chưa chọn đơn vị", { duration: 3000, type: 'error' });
                } else if ($scope.target.typeValue == "-1") {
                    ngNotify.set("Chưa chọn điều kiện lọc", { duration: 3000, type: 'error' });
                } else {
                    var modulename = $scope.target.moduleName + "Controller";
                    var controllerRp = $scope.target.moduleName + "DetailsReportController";
                    $scope.target.TENBAOCAO = '';
                    initializeParam($scope.target, 1, controllerRp, modulename);
                }
            };
        }]);
    return app;
});