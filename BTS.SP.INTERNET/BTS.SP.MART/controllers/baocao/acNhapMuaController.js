/*  
* Người tạo : Nguyễn Tuấn Hoàng Anh
* View: BTS.SP.MART/views/nv/NvNhapHangMua
* Vm sevices: BTS.API.SERVICE -> NV ->NvNhapHangMuaVm.cs
* Sevices: BTS.API.SERVICE -> NV -> NvNhapHangMuaService.cs
* Entity: BTS.API.ENTITY -> NV - > NvNhapHangMua.cs
* Menu: Nghiệp vụ-> Nhập hàng mua
*/
define(['ui-bootstrap', '/BTS.SP.MART/controllers/auth/AuthController.js', '/BTS.SP.MART/controllers/auth/AuNguoiDung.js', '/BTS.SP.MART/controllers/htdm/periodController.js', '/BTS.SP.MART/controllers/htdm/merchandiseController.js', '/BTS.SP.MART/controllers/htdm/customerController.js', '/BTS.SP.MART/controllers/htdm/merchandiseTypeController.js', '/BTS.SP.MART/controllers/htdm/nhomVatTuController.js', '/BTS.SP.MART/controllers/htdm/supplierController.js', '/BTS.SP.MART/controllers/htdm/wareHouseController.js', '/BTS.SP.MART/controllers/htdm/packagingController.js', '/BTS.SP.MART/controllers/htdm/taxController.js', '/BTS.SP.MART/controllers/htdm/donViTinhController.js'], function () {
    'use strict';
    var app = angular.module('acNhapMuaModule', ['ui.bootstrap', 'authModule', 'AuNguoiDungModule', 'periodModule', 'merchandiseModule', 'customerModule', 'merchandiseTypeModule', 'nhomVatTuModule', 'supplierModule', 'wareHouseModule', 'packagingModule', 'taxModule', 'donViTinhModule']);
    app.factory('acNhapMuaService', ['$http', 'configService', function ($http, configService) {
        var serviceUrl = configService.rootUrlWebApi + '/Nv/NhapHangMua';
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
            getUnitUsers: function () {
                return $http.get(rootUrl + '/api/Authorize/AuDonVi/GetSelectDataByUnitCode');
            },
            getSelectDataType: function (type) {
                return $http.get(rootUrl + '/api/Md/TypeReason/GetSelectDataType/' + type);
            },
            postReportXBTongHop: function (json, callback) {
                $http.post(rootUrl + '/api/Nv/NhapMua/PostReportXBTongHop', json).success(callback);
            },
            getNewParameter: function () {
                return $http.get(rootUrl + '/api/Ac/ImportExport/GetDateToAC_NhapMua');
            },
            postExportExcelXBChiTiet: function (json, filename, callback) {
                $http({
                    url: rootUrl + '/api/Nv/NhapMua/PostExportExcelChiTiet',
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
            },
            postExportExcelXBTongHop: function (json, filename, callback) {
                $http({
                    url: rootUrl + '/api/Nv/NhapMua/PostExportExcelTongHop',
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
            },
            postExportExcelDetailCap2: function (json, filename) {
                $http({
                    url: serviceUrl + '/PostExportExcelDetailCap2',
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

                //$http.post(serviceUrl + '/WriteDataToExcel', data).success(callback);
            },
            //ReportTongHop
            postReportNhapMua: function (filter, callback) {
                $http.post(serviceUrl + '/PostReportNhapMua', filter).success(callback);
            },
            postReportDieuChuyenNhan: function (filter, callback) {
                $http.post(serviceUrl + '/PostReportDieuChuyenNhan', filter).success(callback);
            },
            postExportExcelDCNTongHop: function (json, filename) {
                $http({
                    url: serviceUrl + '/PostExportExcelDCNTongHop',
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
                    window.URL.revokeObjectURL(objectUrl);
                }).error(function (data, status, headers, config) {
                    //upload failed
                });
            },
            postExportExcelDCNChiTiet: function (json, filename) {
                $http({
                    url: serviceUrl + '/PostExportExcelDCNChiTiet',
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
                    window.URL.revokeObjectURL(objectUrl);
                }).error(function (data, status, headers, config) {
                    //upload failed
                });
            }
        }
        return result;
    }]);
    /* controller list */
    app.controller('acNhapMuaController', [
        '$scope', 'configService', 'acNhapMuaService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'securityService', 'toaster', 'periodService', 'merchandiseService', 'customerService', 'merchandiseTypeService', 'nhomVatTuService', 'supplierService', 'wareHouseService', 'packagingService', 'taxService', 'donViTinhService', '$state', 'userService', 'AuDonViService', 'AuNguoiDungService',
        function ($scope, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, securityService, toaster, servicePeriod, serviceMerchandise, serviceCustomer, serviceMerchandiseType, serviceNhomVatTu, serviceSupplier, serviceWareHouse, servicePackaging, serviceTax, serviceDonViTinh, $state, serviceAuthUser, AuDonViService, AuNguoiDungService) {
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
            $scope.groupBy = 'donVi';
            $scope.target.PHUONGTHUC = "NMUA";
            var inventoryGroupBy = [
                { value: 'maKho', text: 'Mã kho', extendValue: 'MAKHO' },
                { value: 'donVi', text: 'Mã đơn vị', extendValue: 'MADONVI' },
                { value: 'donViXuat', text: 'Mã đơn vị xuất', extendValue: 'MADONVIXUAT' },
                { value: 'nhaCungCap', text: 'Nhà cung cấp', extendValue: 'MANHACUNGCAP' },
                { value: 'loaiHang', text: 'Mã loại vật tư', extendValue: 'MALOAIVATTU' },
                { value: 'nhomHang', text: 'Mã nhóm vật tư', extendValue: 'MANHOMVATTU' },
                { value: 'hangHoa', text: 'Mã vật tư', extendValue: 'MAVATTU' },
                { value: 'loaiThue', text: 'Mã loại thuế', extendValue: 'MALOAITHUE' },
                { value: 'phieu', text: 'Mã phiếu', extendValue: 'PHIEU' },
                { value: 'nguoidung', text: 'Mã người tạo phiếu', extendValue: 'NGUOIDUNG' },
            ];
            //loại điều chuyển
            $scope.listLoaiDieuChuyens = [
				{
				    value: 1,
				    text: 'Nhận chuyển kho',
				    extendValue: 'NHANCHUYENKHO'
				},
				{
				    value: 2,
				    text: 'Nhận siêu thị thành viên',
				    extendValue: 'NHANSIEUTHITHANHVIEN'
				}
            ];
            $scope.listPTNX = [
				{
				    value: 0,
				    text: 'Nhập mua',
				    name: 'nhapMua',
				    extendValue: 'NMUA'
				},
				{
				    value: 1,
				    text: 'Nhập bán buôn trả lại',
				    name: 'nhapBanBuonTraLai',
				    extendValue: 'NHBANTL'
				},
				{
				    value: 2,
				    text: 'Nhập điều chuyển nội bộ',
				    name: 'nhapDieuChuyenNoiBo',
				    extendValue: 'DCN'
				},
				{
				    value: 3,
				    text: 'Nhập khác',
				    name: 'nhapKhac',
				    extendValue: 'NKHAC'
				}
            ];
            $scope.chageIsNotPay = function (value) {
                if ($scope.target.isPay) {
                    $scope.target.isPay = false;
                    $scope.target.isPay = 0;
                }
            };
            $scope.chageIsPay = function (value) {
                if ($scope.target.isNotPay) {
                    $scope.target.isNotPay = false;
                    $scope.target.isPay = 1;
                }
            };
            $scope.changePTN = function (ptn) {
                if (ptn === 3) {
                    $scope.target.routeType = 1;
                }
                else if (ptn === 4) {
                    $scope.target.loaiNhapKhac = 'N3';
                }
                var data = $filter('filter')($scope.listPTNX, { value: ptn }, true);
                if (data && data.length > 0) {
                    $scope.target.moduleName = data[0].name;
                }
                $scope.target.TENBAOCAO = $scope.listPTNX[ptn].text;
                $scope.target.PHUONGTHUC = $scope.listPTNX[ptn].extendValue;
            };
            //load dữ liệu
            function filterData() {
                $scope.target.toDate = new Date();
                service.getUnitUsers().then(function (response) {
                    if (response && response.status === 200 && response.data) {
                        $scope.unitUsers = response.data;
                        $scope.target.unitCode = currentUser.unitCode;
                    }
                });
                service.getSelectDataType('NHAP').then(function (response) {
                    if (response.data != null) {
                        $scope.listLoaiNhapKhacs = response.data;
                    }
                });
                service.getNewParameter().then(function (response) {
                    if (response && response.status === 200 && response.data) {
                        $scope.target.fromDate = new Date(response.data);
                        servicePeriod.getTableNameByDate($scope.target).then(function (response) {
                            if (response && response.status == 200 && response.data) {
                                $scope.params.P_TABLE_NAME = response.data.data;
                            }
                            return 1;
                        });
                    }
                });
                servicePeriod.getKyKeToan().then(function (response) {
                    if (response) {
                        $scope.target.fromDate = new Date(response.data.fromDate);
                    }
                });
                $scope.target.moduleName = 'nhapMua';
                $scope.nameController = 'reportNhap';
                $scope.target.TENBAOCAO = $scope.listPTNX[$scope.target.phuongThucNhap].text;
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

            $scope.selectUnitUser = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('auth/AuDonVi', 'selectData'),
                    controller: 'donViSelectDataController',
                    resolve: {
                        serviceSelectData: function () {
                            return AuDonViService;
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
            ////////  
            //Thuế
            $scope.selectTax = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Tax', 'selectData'),
                    controller: 'taxSelectDataController',
                    resolve: {
                        targetData: function () {

                        },
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
                        $scope.taxsCodes = output.substring(0, output.length - 1);
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
                securityService.getAccessList('baoCaoNhapMua').then(function (successRes) {
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
            function initializeParam(targetData, value, controllerRp, modulename) {
                var targetObj = angular.copy(targetData);
                targetObj.fromDate = $filter('date')(targetObj.fromDate, 'yyyy-MM-dd');
                targetObj.toDate = $filter('date')(targetObj.toDate, 'yyyy-MM-dd');
                $scope.params.P_TUNGAY = $filter('date')($scope.target.fromDate, 'yyyy-MM-dd');
                $scope.params.P_DENNGAY = $filter('date')($scope.target.toDate, 'yyyy-MM-dd');
                $scope.params.P_NAMEGROUPBY = $filter('filter')(inventoryGroupBy, { value: $scope.groupBy }, true)[0].text;
                $scope.params.P_UNITCODE = targetData.unitCode;
                $scope.params.P_USERNAME = currentUser.userName;
                $scope.params.P_UNITUSER = $scope.unitUserCodes;
                $scope.params.P_MATHUE = convertToArrayCondition($scope.taxsCodes);
                $scope.params.P_GROUPBY = $filter('filter')(inventoryGroupBy, { value: $scope.groupBy }, true)[0].extendValue;
                $scope.params.P_WAREHOUSE = convertToArrayCondition($scope.wareHouseCodes);
                $scope.params.P_MALOAI = convertToArrayCondition($scope.merchandiseTypeCodes);
                $scope.params.P_MANHOM = convertToArrayCondition($scope.merchandiseGroupCodes);
                $scope.params.P_MAVATTU = convertToArrayCondition($scope.merchandiseCodes);
                $scope.params.P_NHACUNGCAP = convertToArrayCondition($scope.nhaCungCapCodes);
                $scope.params.P_NGUOIDUNG = convertToArrayCondition($scope.userCodes);
                $scope.params.P_PHUONGTHUCNHAP = $scope.target.phuongThucNhap;
                $scope.params.P_LOAILYDO = $scope.target.loaiNhapKhacs;
                $scope.params.P_LOAICHUNGTU = $scope.target.PHUONGTHUC;
                $scope.params.TENBAOCAO = $scope.target.TENBAOCAO;
                $scope.params.P_PTHUCNHAN = $filter('filter')($scope.listLoaiDieuChuyens, { value: $scope.target.routeType }, true)[0].extendValue;
                if (value === 0) $scope.params.P_TITLE = ($scope.target.TENBAOCAO + " tổng hợp").toUpperCase();
                else if (value === 1) $scope.params.P_TITLE = ($scope.target.TENBAOCAO + " chi tiết").toUpperCase();
                servicePeriod.getTableNameByDate(targetObj).then(function (response) {
                    if (response && response.status === 200 && response.data) {
                        $scope.params.P_TABLE_NAME = response.data.data;
                    }
                    return 1;
                }).then(function (ok) {
                    loadModeReport(controllerRp, modulename);
                });
            }
            $scope.report = function () {
                if ($scope.target.loaiBaoCao == "-1") {
                    ngNotify.set("Chưa chọn loại báo cáo", { duration: 3000, type: 'error' });
                } else if ($scope.target.unitCode == "-1") {
                    ngNotify.set("Chưa chọn đơn vị", { duration: 3000, type: 'error' });
                } else {
                    var modulename = $scope.nameController + "Controller";
                    var controllerRp = $scope.target.moduleName + "ReportController";
                    initializeParam($scope.target, 0, controllerRp, modulename);
                }
            };

            $scope.reportDetails = function () {
                if ($scope.target.loaiBaoCao == "-1") {
                    ngNotify.set("Chưa chọn loại báo cáo", { duration: 3000, type: 'error' });
                } else if ($scope.target.unitCode == "-1") {
                    ngNotify.set("Chưa chọn đơn vị", { duration: 3000, type: 'error' });
                } else {
                    var modulename = $scope.nameController + "Controller";
                    var controllerRp = $scope.target.moduleName + "DetailsReportController";
                    initializeParam($scope.target, 1, controllerRp, modulename);
                }
            };
        }]);
    return app;
});