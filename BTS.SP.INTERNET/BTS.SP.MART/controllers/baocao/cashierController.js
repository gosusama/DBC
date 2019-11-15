/*  
* Người tạo : Nguyễn Tuấn Hoàng Anh
* View: BTS.SP.MART/views/nv/NvNhapHangMua
* Vm sevices: BTS.API.SERVICE -> NV ->NvNhapHangMuaVm.cs
* Sevices: BTS.API.SERVICE -> NV -> NvNhapHangMuaService.cs
* Entity: BTS.API.ENTITY -> NV - > NvNhapHangMua.cs
* Menu: Nghiệp vụ-> Nhập hàng mua
*/
define(['ui-bootstrap', '/BTS.SP.MART/controllers/auth/AuthController.js', '/BTS.SP.MART/controllers/htdm/periodController.js', '/BTS.SP.MART/controllers/htdm/merchandiseController.js', '/BTS.SP.MART/controllers/htdm/customerController.js', '/BTS.SP.MART/controllers/htdm/merchandiseTypeController.js', '/BTS.SP.MART/controllers/htdm/nhomVatTuController.js', '/BTS.SP.MART/controllers/htdm/supplierController.js', '/BTS.SP.MART/controllers/htdm/wareHouseController.js', '/BTS.SP.MART/controllers/htdm/packagingController.js', '/BTS.SP.MART/controllers/htdm/taxController.js', '/BTS.SP.MART/controllers/htdm/donViTinhController.js', '/BTS.SP.MART/controllers/htdm/typeReasonController.js', '/BTS.SP.MART/controllers/baocao/xuatBanLeController.js', '/BTS.SP.MART/controllers/htdm/boHangController.js'], function () {
    'use strict';
    var app = angular.module('cashierModule', ['ui.bootstrap', 'authModule', 'periodModule', 'merchandiseModule', 'customerModule', 'merchandiseTypeModule', 'nhomVatTuModule', 'supplierModule', 'wareHouseModule', 'packagingModule', 'taxModule', 'donViTinhModule', 'typeReasonModule', 'xuatBanLeModule']);
    app.factory('cashierService', ['$http', 'configService', function ($http, configService) {
        var serviceUrl = configService.rootUrlWebApi + '/Nv/GiaoDichQuay';
        var rootUrl = configService.rootUrlWebApi;
        var rootUrlAcCloseout = configService.rootUrlWebApi + '/AC/Closeout';
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
        };
        var result = {
            robot: calc,
            getUnitUsers: function () {
                return $http.get(rootUrl + '/Authorize/AuDonVi/GetSelectDataByUnitCode');
            },
            getNewParameter: function () {
                return $http.get(rootUrl + '/Nv/GiaoDichQuay/GetNewParameter');
            },
            postReportXBTongHop: function (json) {
                return $http.post(rootUrl + '/api/Nv/GiaoDichQuay/PostReportXBTongHop', json);
            },
            getLastPeriod: function () {
                return $http.get(rootUrlAcCloseout + '/GetLastPeriod');
            },
            postExportExcel: function (json, filename, callback) {
                $http({
                    url: rootUrl + '/Nv/GiaoDichQuay/PostExportExcel',
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
                }).error(function (data, status, headers, config) {
                    //upload failed
                });

                //$http.get(rootUrl + '/api/Nv/GiaoDichQuay/exportExcel').success(callback);
            }
        }
        return result;
    }]);
    /* controller list */
    app.controller('cashierController', [
        '$scope', '$location', '$http', 'configService', 'cashierService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'securityService', '$rootScope', 'toaster', 'periodService', 'merchandiseService', 'customerService', 'merchandiseTypeService', 'nhomVatTuService', 'supplierService', 'wareHouseService', 'packagingService', 'taxService', 'donViTinhService', '$state', 'userService', 'AuDonViService', 'typeReasonService',
        function ($scope, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, securityService, $rootScope, toaster, servicePeriod, serviceMerchandise, serviceCustomer, serviceMerchandiseType, serviceNhomVatTu, serviceSupplier, serviceWareHouse, servicePackaging, serviceTax, serviceDonViTinh, $state, serviceAuthUser, serviceAuDonVi, serviceTypeReason) {
            var currentUser = serviceAuthUser.GetCurrentUser();
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.filtered = angular.copy(configService.filterDefault);
            $scope.tempData = tempDataService.tempData;
            $scope.isEditable = true;
            $scope.target = {};
            var date = [];
            $scope.accessList = {};
            $scope.params = {};
            $scope.options = {
                minDate: null,
                maxDate: null
            };

            $scope.listGiaoDich = [
                {
                    value: '0',
                    text: 'Xuất bán lẻ',
                    name: 'xuatBanLe'
                },
                {
                    value: '1',
                    text: 'Nhập bán lẻ trả lại',
                    name: 'nhapBanLeTraLai'
                },
                {
                    value: '2',
                    text: 'Bán buôn quầy thu ngân',
                    name: 'banBuonQuayThuNgan'
                },
                {
                    value: '3',
                    text: 'Bán lẻ + bán buôn quầy thu ngân',
                    name: 'banLe_And_banBuon_QuayThuNgan'
                }
            ];
            $scope.groupBy = 'MADONVI';
            $scope.changeLoaiGiaoDich = function (loaiBc) {
                if (loaiBc) {
                    var data = $filter('filter')($scope.listGiaoDich, { value: loaiBc }, true);
                    if (data && data.length > 0) {
                        $scope.target.moduleName = data[0].name;
                    }
                    if (loaiBc == '0') {
                        $scope.title = "Báo cáo xuất bán lẻ";
                        $scope.target.loaiGiaoDich = '0';
                        $scope.titleReport = "Báo cáo xuất bán lẻ";

                    } else if (loaiBc == '1') {
                        $scope.title = "Báo cáo nhập bán lẻ trả lại";
                        $scope.target.loaiGiaoDich = '1';
                        $scope.titleReport = "Báo cáo nhập bán lẻ trả lại";
                    }
                    else if (loaiBc == '2') {
                        $scope.title = "Báo cáo xuất bán buôn quầy thu ngân";
                        $scope.target.loaiGiaoDich = '2';
                        $scope.titleReport = "Báo cáo xuất bán buôn quầy thu ngân";
                    }
                    else if (loaiBc == '3') {
                        $scope.title = "Báo cáo xuất bán lẻ + buôn quầy thu ngân";
                        $scope.target.loaiGiaoDich = '3';
                        $scope.titleReport = "Báo cáo xuất bán lẻ + buôn quầy thu ngân";
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
                        $scope.options.minDate = new Date(response.data.minDate);
                        $scope.options.maxDate = new Date(response.data.maxDate);
                        $scope.target.loaiGiaoDich = '0';
                        $scope.changeLoaiGiaoDich($scope.target.loaiGiaoDich);
                        date = new Date(response.data.maxDate);
                        $scope.target.fromDate = new Date(response.data.maxDate);
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
                    if (updatedData != null) {
                        var output = '';
                        angular.forEach(updatedData, function (item, index) {
                            output += item.value + ',';
                        });
                        $scope.wareHouseCodes = output.substring(0, output.length - 1);
                        $scope.target.maKhoXuatKhuyenMai = $scope.wareHouseCodes;
                    }
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
                                advanceData: {
                                    unitCode: $scope.target.unitCode,
                                    chucVu: 'THUNGAN'
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
                    $scope.userCodes = output.substring(0, output.length - 1);
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            }
            $scope.selectUnitUser = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('auth/AuDonVi', 'selectData'),
                    controller: 'donViSelectDataController',
                    resolve: {
                        serviceSelectData: function () {
                            return serviceAuDonVi;
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
                        $scope.unitUserCodes = output.substring(0, output.length - 1);
                    }
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
            ////////  
            //Thuế
            $scope.selectTax = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Tax', 'selectData'),
                    controller: 'taxSelectDataController',
                    resolve: {
                        serviceSelectData: function () {
                            return serviceTax;
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
                        $scope.taxCodes = output.substring(0, output.length - 1);
                    }
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };
            ////////  
            //Bó hàng
            $scope.selectBoHang = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/BoHang', 'selectData'),
                    controller: 'boHangSelectDataController',
                    resolve: {
                        serviceSelectData: function () {
                            return serviceBoHang;
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
                        $scope.boHangs = output.substring(0, output.length - 1);
                    }
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
            ////////
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
                securityService.getAccessList('cashier').then(function (successRes) {
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
            loadNguoiDung();
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
                $scope.params.P_TUNGAY = $filter('date')($scope.target.fromDate, 'yyyy-MM-dd');
                $scope.params.P_DENNGAY = $filter('date')($scope.target.toDate, 'yyyy-MM-dd');
                $scope.params.P_GROUPBY = $scope.groupBy;
                $scope.params.P_UNITUSER = $scope.target.unitUserCodes;
                $scope.params.P_MAKHO = convertToArrayCondition($scope.wareHouseCodes);
                $scope.params.P_MALOAI = convertToArrayCondition($scope.merchandiseTypeCodes);
                $scope.params.P_MANHOM = convertToArrayCondition($scope.merchandiseGroupCodes);
                $scope.params.P_MAVATTU = convertToArrayCondition($scope.merchandiseCodes);
                $scope.params.P_NHACUNGCAP = convertToArrayCondition($scope.nhaCungCapCodes);
                $scope.params.P_MAKHACHHANG = convertToArrayCondition($scope.customerCodes);
                $scope.params.P_TAX = convertToArrayCondition($scope.taxCodes);
                $scope.params.P_UNITCODE = targetData.unitCode;
                $scope.params.P_USERNAME = currentUser.userName;
                $scope.params.P_NGUOIDUNG = convertToArrayCondition($scope.userCodes);
                $scope.params.P_BOHANG = $scope.boHangs;
                if (value === 0) $scope.params.P_TITLE = ($scope.titleReport + " tổng hợp").toUpperCase();
                else if (value === 1) $scope.params.P_TITLE = ($scope.titleReport + " chi tiết").toUpperCase();
                else $scope.params.P_TITLE = ("BÁO CÁO TÓM TẮT THU NGÂN");
            }
            $scope.reportTn = function () {
                var modulename = "giaoDichQuayController.js";
                var controllerRp = "giaoDichQuayReportController";
                $scope.target.TENBAOCAO = '';
                initializeParam($scope.target, 2);
                loadModeReport(controllerRp, modulename);
            }
            $scope.reportTongHop = function () {
                if ($scope.target.loaiGiaoDich === "-1") {
                    ngNotify.set("Chưa chọn loại báo cáo", { duration: 3000, type: 'error' });
                } else if ($scope.target.unitCode === "-1") {
                    ngNotify.set("Chưa chọn đơn vị", { duration: 3000, type: 'error' });
                } else {
                    var modulename = $scope.target.moduleName + "Controller.js";
                    var controllerRp = $scope.target.moduleName + "ReportController";
                    $scope.target.TENBAOCAO = '';
                    initializeParam($scope.target, 0);
                    loadModeReport(controllerRp, modulename);
                }
            };

            $scope.reportDetails = function () {
                if ($scope.target.loaiGiaoDich === "-1") {
                    ngNotify.set("Chưa chọn loại báo cáo", { duration: 3000, type: 'error' });
                } else if ($scope.target.unitCode === "-1") {
                    ngNotify.set("Chưa chọn đơn vị", { duration: 3000, type: 'error' });
                } else {
                    var modulename = $scope.target.moduleName + "Controller.js";
                    var controllerRp = $scope.target.moduleName + "DetailsReportController";
                    $scope.target.TENBAOCAO = '';
                    initializeParam($scope.target, 1);
                    loadModeReport(controllerRp, modulename);
                }
            };

            $scope.reportExcelDetails = function () {
                var filename = "";
                $scope.target.wareHouseCodes = $scope.wareHouseCodes;
                $scope.target.merchandiseTypeCodes = $scope.merchandiseTypeCodes;
                $scope.target.merchandiseGroupCodes = $scope.merchandiseGroupCodes;
                $scope.target.merchandiseCodes = $scope.merchandiseCodes;
                $scope.target.nhaCungCapCodes = $scope.nhaCungCapCodes;
                $rootScope.groupBy = $scope.groupBy;
                $scope.target.groupBy = $rootScope.groupBy;
                console.log('$scope.target', $scope.target);
                switch ($scope.target.loaiGiaoDich) {
                    case "1":
                        switch ($scope.target.groupBy) {
                            case "MALOAIVATTU":
                                filename = "BC_BanLeTraLai_ChiTietTheoLoaiHang";
                                break;
                            case "MANHOMVATTU":
                                filename = "BC_BanLeTraLai_ChiTietTheoNhomHang";
                                break;
                            case "MAVATTU":
                                filename = "BC_BanLeTraLai_ChiTietTheoHang";
                                break;
                            case "MAKHACHHANG":
                                filename = "BC_BanLeTraLai_ChiTietTheoNhaCungCap";
                                break;
                            case "MAGIAODICH":
                                filename = "BC_BanLeTraLai_ChiTietTheoGiaoDich";
                                break;
                            case "MAKHO":
                                filename = "BC_BanLeTraLai_ChiTietTheoKhoHang";
                                break;
                            default:
                                filename = "BC_BanLeTraLai_ChiTiet";
                                break;
                        }
                        break;
                    default:
                        switch ($scope.target.groupBy) {
                            case "MALOAIVATTU":
                                filename = "BaoCao_GDQ_ChiTiet_TheoLoaiHang";
                                break;
                            case "MANHOMVATTU":
                                filename = "BaoCao_GDQ_ChiTiet_TheoNhomHang";
                                break;
                            case "MAVATTU":
                                filename = "BaoCao_GDQ_ChiTiet_TheoHang";
                                break;
                            case "MAKHACHHANG":
                                filename = "BaoCao_GDQ_ChiTiet_TheoNhaCungCap";
                                break;
                            case "MAGIAODICH":
                                filename = "BaoCao_GDQ_ChiTiet_TheoGiaoDich";
                                break;
                            case "MAKHO":
                                filename = "BC_BanLeTraLai_ChiTietTheoKhoHang";
                                break;
                            default:
                                filename = "BaoCao_GDQ_ChiTiet";
                                break;
                        }
                }
                service.postExportExcel($scope.target, filename, function () {
                });
            }
        }]);

    return app;
});