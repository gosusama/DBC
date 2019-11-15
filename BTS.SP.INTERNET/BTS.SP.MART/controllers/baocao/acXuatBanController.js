/*  
* Người tạo : Nguyễn Tuấn Hoàng Anh
* View: BTS.SP.MART/views/nv/NvNhapHangMua
* Vm sevices: BTS.API.SERVICE -> NV ->NvNhapHangMuaVm.cs
* Sevices: BTS.API.SERVICE -> NV -> NvNhapHangMuaService.cs
* Entity: BTS.API.ENTITY -> NV - > NvNhapHangMua.cs
* Menu: Nghiệp vụ-> Nhập hàng mua
*/
define(['ui-bootstrap', '/BTS.SP.MART/controllers/auth/AuthController.js', '/BTS.SP.MART/controllers/auth/AuNguoiDung.js', '/BTS.SP.MART/controllers/htdm/periodController.js', '/BTS.SP.MART/controllers/htdm/merchandiseController.js', '/BTS.SP.MART/controllers/htdm/customerController.js', '/BTS.SP.MART/controllers/htdm/merchandiseTypeController.js', '/BTS.SP.MART/controllers/htdm/nhomVatTuController.js', '/BTS.SP.MART/controllers/htdm/supplierController.js', '/BTS.SP.MART/controllers/htdm/wareHouseController.js', '/BTS.SP.MART/controllers/htdm/packagingController.js', '/BTS.SP.MART/controllers/htdm/taxController.js', '/BTS.SP.MART/controllers/htdm/donViTinhController.js', '/BTS.SP.MART/controllers/htdm/typeReasonController.js'], function () {
    'use strict';
    var app = angular.module('acXuatBanModule', ['ui.bootstrap', 'authModule', 'AuNguoiDungModule', 'periodModule', 'merchandiseModule', 'customerModule', 'merchandiseTypeModule', 'nhomVatTuModule', 'supplierModule', 'wareHouseModule', 'packagingModule', 'taxModule', 'donViTinhModule', 'typeReasonModule']);
    app.factory('acXuatBanService', ['$http', 'configService', function ($http, configService) {
        var serviceUrl = configService.rootUrlWebApi + '/Nv/XuatBan';
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
                return $http.get(rootUrl + '/Nv/XuatBan/GetNewParameter');
            },
            postReportXBTongHop: function (json) {
                return $http.post(rootUrl + '/api/Nv/XuatBan/PostReportXBTongHop', json);
            },
            getLastPeriod: function () {
                return $http.get(rootUrlAcCloseout + '/GetLastPeriod');
            },
            postExportExcelXBChiTiet: function (json, filename, callback) {
                $http({
                    url: serviceUrl + '/PostExportExcelChiTiet',
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
    app.controller('acXuatBanController', [
        '$scope', '$location', '$http', 'configService', 'acXuatBanService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'securityService', '$rootScope', 'toaster', 'periodService', 'merchandiseService', 'customerService', 'merchandiseTypeService', 'nhomVatTuService', 'supplierService', 'wareHouseService', 'packagingService', 'taxService', 'donViTinhService', '$state', 'userService', 'AuDonViService', 'typeReasonService', 'AuNguoiDungService',
        function ($scope, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, securityService, $rootScope, toaster, servicePeriod, serviceMerchandise, serviceCustomer, serviceMerchandiseType, serviceNhomVatTu, serviceSupplier, serviceWareHouse, servicePackaging, serviceTax, serviceDonViTinh, $state, serviceAuthUser, serviceAuDonVi, serviceTypeReason, AuNguoiDungService) {
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
            //loại điều chuyển
            $scope.listRouteTypes = [
                {
                    value: '0',
                    text: 'Xuất chuyển kho',
                    name: 'xuatChuyenKho'
                },
                {
                    value: '1',
                    text: 'Xuất siêu thị thành viên',
                    name: 'xuatSieuThiThanhVien'
                }
            ];
            //loại lý do
            $scope.listTypesReason = [
                {
                    value: '0',
                    text: 'Xuất hủy',
                    name: 'xuatHuy'
                },
                {
                    value: '1',
                    text: 'Xuất trả nhà cung cấp',
                    name: 'xuatTraNhaCungCap'
                },
                {
                    value: '2',
                    text: 'Xuất hủy hàng hỏng',
                    name: 'xuatHuyHangHong'
                },
                {
                    value: '3',
                    text: 'Xuất điều chỉnh',
                    name: 'xuatDieuChinh'
                }
            ];
            $scope.listBaoCao = [
                {
                    value: '0',
                    text: 'Xuất bán buôn',
                    name: 'xuatBanBuon'
                },
                {
                    value: '1',
                    text: 'Xuất điều chuyển nội bộ',
                    name: 'xuatDieuChuyenNoiBo'
                },
                {
                    value: '2',
                    text: 'Xuất khác',
                    name: 'xuatKhac'
                }
            ];
            $scope.groupBy = 'MADONVIXUAT';
            $scope.title = "Báo cáo xuất bán buôn";
            $scope.titleReport = "Báo cáo xuất bán buôn";
            $scope.changeTypesReason = function (loaiLyDo) {
                $scope.titleReport = "Báo cáo ";
                $scope.title = "Báo cáo ";
                angular.forEach($scope.typeReasons, function (v, k) {
                    if (loaiLyDo == $scope.typeReasons[k].value) {
                        $scope.title += $scope.typeReasons[k].description;
                        $scope.titleReport += $scope.typeReasons[k].description;
                    }
                });
            };
            $scope.chageIsNotPay = function (value) {
                if ($scope.target.isPay) {
                    $scope.target.isPay = false;
                    $scope.target.isPay = 0;
                }
            };
            $scope.chageIsPay = function (value) {
                if ($scope.target.isNotPay) {
                    $scope.target.isNotPay = false;
                    $scope.target.isPay = 10;
                }
            };

            $scope.changeLoaiBaoCao = function (loaiBc) {
                if (loaiBc) {
                    var data = $filter('filter')($scope.listBaoCao, { value: loaiBc }, true);
                    if (data && data.length > 0) {
                        $scope.target.moduleName = data[0].name;
                    }
                    if (loaiBc === '0') {
                        $scope.title = "Báo cáo xuất bán buôn";
                        $scope.target.loaiBaoCao = '0';
                        $scope.target.reportType = '0';
                        $scope.titleReport = "Báo cáo xuất bán buôn";

                    } else if (loaiBc === '1') {
                        $scope.title = "Báo cáo xuất điều chuyển nội bộ";
                        $scope.target.loaiBaoCao = '1';
                        $scope.target.reportType = '1';
                        $scope.titleReport = "Báo cáo xuất điều chuyển nội bộ";
                    } else if (loaiBc === '2') {
                        $scope.title = "Báo cáo ";
                        $scope.target.loaiBaoCao = '2';
                        $scope.target.reportType = '2';
                        $scope.titleReport = "Báo cáo ";

                        serviceTypeReason.getAll_TypeReasonByParent('XUAT').then(function (successRes) {
                            if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                                tempDataService.putTempData('typeReasons', successRes.data.data);
                                $scope.typeReasons = successRes.data.data;
                            } else {
                                console.log('successRes', successRes);
                            }
                        }, function (errorRes) {
                            console.log('errorRes', errorRes);
                        });
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
                        $scope.target.loaiBaoCao = '0';
                        $scope.target.moduleName = 'xuatBanBuon';
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
                securityService.getAccessList('baoCaoXuatBan').then(function (successRes) {
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
            loadNguoiDung();
            loadMerchandiseType();
            loadNhomVatTu();
            loadWareHouse();
            loadPackagings();
            loadTax();
            loadDonViTinh();
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
                $scope.params.P_UNITUSER = convertToArrayCondition($scope.unitUserCodes);
                $scope.params.P_MAKHO = convertToArrayCondition($scope.wareHouseCodes);
                $scope.params.P_MALOAI = convertToArrayCondition($scope.merchandiseTypeCodes);
                $scope.params.P_MANHOM = convertToArrayCondition($scope.merchandiseGroupCodes);
                $scope.params.P_MAVATTU = convertToArrayCondition($scope.merchandiseCodes);
                $scope.params.P_NHACUNGCAP = convertToArrayCondition($scope.nhaCungCapCodes);
                $scope.params.P_MAKHACHHANG = convertToArrayCondition($scope.customerCodes);
                $scope.params.P_TAX = convertToArrayCondition($scope.taxCodes);
                $scope.params.P_NGUOIDUNG = convertToArrayCondition($scope.userCodes);
                if ($scope.target.reasonType) {
                    $scope.params.P_PTHUCXUAT = $scope.target.reasonType;
                }
                else {
                    $scope.params.P_PTHUCXUAT = $scope.target.routeType;
                }
                $scope.params.P_UNITCODE = targetData.unitCode;
                $scope.params.P_USERNAME = currentUser.userName;
                if (!$scope.target.isPay) $scope.params.P_ISPAY = '3';
                else $scope.params.P_ISPAY = $scope.target.isPay;
                if (value === 0) $scope.params.P_TITLE = ($scope.titleReport + " tổng hợp").toUpperCase();
                else if (value === 1) $scope.params.P_TITLE = ($scope.titleReport + " chi tiết").toUpperCase();
            }
            $scope.report = function () {
                if ($scope.target.loaiBaoCao === "-1") {
                    ngNotify.set("Chưa chọn loại báo cáo", { duration: 3000, type: 'error' });
                } else if ($scope.target.unitCode === "-1") {
                    ngNotify.set("Chưa chọn đơn vị", { duration: 3000, type: 'error' });
                } else {
                    var modulename = $scope.target.moduleName + "Controller";
                    var controllerRp = $scope.target.moduleName + "ReportController";
                    $scope.target.TENBAOCAO = '';
                    initializeParam($scope.target, 0);
                    loadModeReport(controllerRp, modulename);
                }
            };

            $scope.reportDetails = function () {
                if ($scope.target.loaiBaoCao === "-1") {
                    ngNotify.set("Chưa chọn loại báo cáo", { duration: 3000, type: 'error' });
                } else if ($scope.target.unitCode === "-1") {
                    ngNotify.set("Chưa chọn đơn vị", { duration: 3000, type: 'error' });
                } else {
                    var modulename = $scope.target.moduleName + "Controller";
                    var controllerRp = $scope.target.moduleName + "DetailsReportController";
                    $scope.target.TENBAOCAO = '';
                    initializeParam($scope.target, 1);
                    loadModeReport(controllerRp, modulename);
                }
            };

            $scope.reportExcelDetails = function () {
                var filename = "";
                if (angular.isUndefined($scope.target.isNotPay) && $scope.target.isPay === 0) {
                    $scope.target.isPay = 3;
                };
                $scope.target.wareHouseCodes = $scope.wareHouseCodes;
                $scope.target.merchandiseTypeCodes = $scope.merchandiseTypeCodes;
                $scope.target.merchandiseGroupCodes = $scope.merchandiseGroupCodes;
                $scope.target.merchandiseCodes = $scope.merchandiseCodes;
                $scope.target.nhaCungCapCodes = $scope.nhaCungCapCodes;
                $rootScope.groupBy = $scope.groupBy;
                $scope.target.groupBy = $rootScope.groupBy;
                switch ($scope.target.loaiBaoCao) {
                    case "1":
                        switch ($scope.target.groupBy) {
                            case "MALOAIVATTU":
                                filename = "BCDieuChuyenXuat_ChiTiet_TheoLoaiHang";
                                break;
                            case "MANHOMVATTU":
                                filename = "BCDieuChuyenXuat_ChiTiet_TheoNhomHang";
                                break;
                            case "MANHACUNGCAP":
                                filename = "BCDieuChuyenXuat_ChiTiet_TheoNhaCungCap";
                                break;
                            case "MAVATTU":
                                filename = "BCDieuChuyenXuat_ChiTiet_TheoHang";
                                break;
                            case "MAKHO":
                                filename = "BCDieuChuyenXuat_ChiTiet_TheoKhoHang";
                                break;
                            case "MAKHACHHANG":
                                filename = "BCDieuChuyenXuat_ChiTiet_TheoKhachHang";
                                break;
                            default:
                                filename = "BCDieuChuyenXuat_ChiTiet";
                                break;
                        }
                        break;
                    case "2":
                        switch ($scope.target.groupBy) {
                            case "MALOAIVATTU":
                                filename = "BCXuatKhac_ChiTiet_TheoLoaiHang";
                                break;
                            case "MANHOMVATTU":
                                filename = "BCXuatKhac_ChiTiet_TheoNhomHang";
                                break;
                            case "MANHACUNGCAP":
                                filename = "BCXuatKhac_ChiTiet_TheoNhaCungCap";
                                break;
                            case "MAVATTU":
                                filename = "BCXuatKhac_ChiTiet_TheoHang";
                                break;
                            case "MAKHO":
                                filename = "BCXuatKhac_ChiTiet_TheoKhoHang";
                                break;
                            case "MAKHACHHANG":
                                filename = "BCDieuChuyenXuat_ChiTiet_TheoKhachHang";
                                break;
                            default:
                                filename = "BCXuatKhac_ChiTiet";
                                break;
                        }
                        break;
                    default:
                        switch ($scope.target.groupBy) {
                            case "MALOAIVATTU":
                                filename = "BaoCao_XB_ChiTiet_TheoLoaiHang";
                                break;
                            case "MANHOMVATTU":
                                filename = "BaoCao_XB_ChiTiet_TheoNhomHang";
                                break;
                            case "MANHACUNGCAP":
                                filename = "BaoCao_XB_ChiTiet_TheoNhaCungCap";
                                break;
                            case "MAVATTU":
                                filename = "BaoCao_XB_ChiTiet_TheoHang";
                                break;
                            case "MAKHACHHANG":
                                filename = "BaoCao_XB_ChiTiet_TheoKhachHang";
                                break;
                            case "MAKHO":
                                filename = "BaoCao_XB_ChiTiet_TheoKhoHang";
                                break;
                            default:
                                filename = "BaoCao_XB_ChiTiet_ChiTiet";
                                break;
                        }
                        break;
                }
                service.postExportExcelXBChiTiet($scope.target, filename, function () {
                });
            }
        }]);
    return app;
});