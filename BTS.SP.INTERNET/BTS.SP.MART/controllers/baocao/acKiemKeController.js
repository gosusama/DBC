define(['ui-bootstrap', '/BTS.SP.MART/controllers/auth/AuthController.js', '/BTS.SP.MART/controllers/htdm/periodController.js', '/BTS.SP.MART/controllers/htdm/merchandiseController.js', '/BTS.SP.MART/controllers/htdm/customerController.js', '/BTS.SP.MART/controllers/htdm/merchandiseTypeController.js', '/BTS.SP.MART/controllers/htdm/nhomVatTuController.js', '/BTS.SP.MART/controllers/htdm/supplierController.js', '/BTS.SP.MART/controllers/htdm/wareHouseController.js', '/BTS.SP.MART/controllers/htdm/packagingController.js', '/BTS.SP.MART/controllers/htdm/taxController.js', '/BTS.SP.MART/controllers/htdm/donViTinhController.js', '/BTS.SP.MART/controllers/htdm/shelvesController.js'], function () {
    'use strict';
    var app = angular.module('acKiemKeModule', ['ui.bootstrap', 'authModule', 'periodModule', 'merchandiseModule', 'customerModule', 'merchandiseTypeModule', 'nhomVatTuModule', 'supplierModule', 'wareHouseModule', 'packagingModule', 'taxModule', 'donViTinhModule', 'shelvesModule']);
    app.factory('acKiemKeService', ['$http', 'configService', function ($http, configService) {
        var serviceUrl = configService.rootUrlWebApi + '/Nv/KiemKe';
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
            },
        }
        var result = {
            robot: calc,
            getUnitUsers: function () {
                return $http.get(rootUrl + '/api/Authorize/AuDonVi/GetSelectDataByUnitCode');
            },
            getNewParameter: function () {
                return $http.get(rootUrl + '/api/Nv/KiemKe/GetNewParameter');
            },
            getFromDate: function () {
                return $http.get(rootUrl + '/api/Ac/ImportExport/GetDateToAC_NhapMua');
            },
            postReportTongHop: function (json, callback) {
                $http.post(rootUrl + '/api/Nv/KiemKe/PostReportTongHop', json).success(callback);
            },
            postExportExcelChiTiet: function (json, filename, callback) {
                $http({
                    url: rootUrl + '/api/Nv/KiemKe/PostExportExcelChiTiet',
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
            postExportExcelTongHop: function (json, filename, callback) {
                $http({
                    url: rootUrl + '/api/Nv/KiemKe/PostExportExcelTongHop',
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
        }
        return result;
    }]);
    /* controller list */
    app.controller('acKiemKeController', [
        '$scope', '$location', '$http', 'configService', 'acKiemKeService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'securityService', '$rootScope', 'toaster', 'periodService', 'merchandiseService', 'customerService', 'merchandiseTypeService', 'nhomVatTuService', 'supplierService', 'wareHouseService', 'packagingService', 'taxService', 'donViTinhService', '$state', 'userService', 'AuDonViService', 'shelvesService',
        function ($scope, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, securityService, $rootScope, toaster, servicePeriod, serviceMerchandise, serviceCustomer, serviceMerchandiseType, serviceNhomVatTu, serviceSupplier, serviceWareHouse, servicePackaging, serviceTax, serviceDonViTinh, $state, serviceAuthUser, AuDonViService, shelvesService) {
            var currentUser = serviceAuthUser.GetCurrentUser();
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.filtered = angular.copy(configService.filterDefault);
            $scope.tempData = tempDataService.tempData;
            $scope.isEditable = true;
            $scope.target = {};
            $scope.accessList = {};
            $scope.params = {};
            $scope.target.unitCode = currentUser.unitCode;
            $scope.options = {
                minDate: null,
                maxDate: null
            };
            $scope.groupBy = 0;
            $scope.target.moduleName = 'baoCaoDayDu';
            $scope.nameController = 'reportKiemKe';
            $scope.lstLoaiBaoCao = [
				{
				    value: 1,
				    text: 'Báo cáo đầy đủ',
				    name: 'baoCaoDayDu'
				},
				{
				    value: 2,
				    text: 'Báo cáo thừa',
				    name: 'baoCaoThua'
				},
				{
				    value: 3,
				    text: 'Báo cáo thiếu',
				    name: 'baoCaoThieu'
				},
            ];
            var InventoryGroupBy = [
				'MANHACUNGCAP',
				'MAKHO',
				'MALOAIVATTU',
				'MANHOMVATTU',
				'MAVATTU',
				'PHIEU',
				'MAGIAODICH',
				'MAKHACHHANG',
				'MALOAITHUE',
				'MADONVI',
				'MADONVIXUAT',
				'MADONVINHAN',
				'MAXUATXU',
                'MAKEHANG'
            ];
            var NameGroupBy = [
				'Mã nhà cung cấp',
				'Mã kho',
				'Mã loại vật tư',
				'Mã nhóm vật tư',
				'Mã vật tư',
				'Phiếu',
				'Mã giao dịch',
				'Mã khách hàng',
				'Mã loại thuế',
				'Mã đơn vị',
				'Mã đơn vị xuất',
				'Mã đơn vị nhận',
				'Mã xuất xứ',
                'Mã kệ hàng'
            ];
            $scope.target.TENBAOCAO = $scope.lstLoaiBaoCao[0].text;
            function filterData() {
                $scope.target.toDate = new Date();
                service.getUnitUsers().then(function (response) {
                    if (response && response.status == 200 && response.data) {
                        $scope.unitUsers = response.data;
                    }
                });
                //service.getFromDate().then(function (response) {
                //    if (response.status == 200) {
                //        $scope.target.fromDate = new Date(response.data);
                //        //getKyKeToan($scope.target);
                //    }
                //});
                servicePeriod.getKyKeToan().then(function (response) {
                    if (response) {
                        $scope.target.fromDate = new Date(response.data.fromDate);
                    }
                });
            };

            filterData();
            function getKyKeToan(targetData) {
                servicePeriod.getTableNameByDate(targetData).then(function (response) {
                    if (response && response.status == 200 && response.data) {
                        $scope.params.P_KY = response.data.data;
                    }
                });
            }

            $scope.changeLBC = function (index) {
                $scope.params.P_DKIENLOC = angular.uppercase($scope.lstLoaiBaoCao[index - 1].name);
                $scope.target.moduleName = $scope.lstLoaiBaoCao[index - 1].name;
                $scope.target.TENBAOCAO = $scope.lstLoaiBaoCao[index - 1].text;
            }

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
                        console.log('updatedData', updatedData);
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
                        $scope.donViXuat = output.substring(0, output.length - 1);
                    }
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };
            $scope.selectShelves = function (inputShelves) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Shelves', 'selectData'),
                    controller: 'shelvesSelectDataController',
                    resolve: {
                        serviceSelectData: function () {
                            return shelvesService;
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
                        $scope.shelves = output.substring(0, output.length - 1);
                    }
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            }

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
                        if (response.data) {
                            console.log('response.data', response.data)
                        }
                        else {
                            $scope.selectNhaCungCap();
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
                securityService.getAccessList('baoCaoKiemKe').then(function (successRes) {
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
                            require(['controllers/baocao/' + moduleName], function (module) {
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
            //end
            function initializeParam(targetData, value) {
                $scope.params.P_NAMEGROUPBY = NameGroupBy[$scope.groupBy];
                $scope.params.P_UNITCODE = targetData.unitCode;
                $scope.params.P_USERNAME = currentUser.userName;
                $scope.params.P_TUNGAY = $filter('date')($scope.target.fromDate, 'yyyy-MM-dd');
                $scope.params.P_DENNGAY = $filter('date')($scope.target.toDate, 'yyyy-MM-dd');
                $scope.params.P_MATHUE = convertToArrayCondition($scope.taxsCodes);
                $scope.params.P_GROUPBY = InventoryGroupBy[$scope.groupBy];
                $scope.params.P_WAREHOUSE = convertToArrayCondition($scope.wareHouseCodes);
                $scope.params.P_MALOAI = convertToArrayCondition($scope.merchandiseTypeCodes);
                $scope.params.P_MANHOM = convertToArrayCondition($scope.merchandiseGroupCodes);
                $scope.params.P_MAVATTU = convertToArrayCondition($scope.merchandiseCodes);
                $scope.params.P_NHACUNGCAP = convertToArrayCondition($scope.nhaCungCapCodes);
                $scope.params.P_MAKEHANG = convertToArrayCondition($scope.shelves);
                $scope.params.TENBAOCAO = $scope.target.TENBAOCAO;
                //getKyKeToan($scope.target);
                if (value === 0) $scope.params.P_TITLE = ($scope.target.TENBAOCAO + " tổng hợp").toUpperCase();
                else if (value === 1) $scope.params.P_TITLE = ($scope.target.TENBAOCAO + " chi tiết").toUpperCase();
            }
            $scope.report = function () {
                if ($scope.target.loaiBaoCao == "-1") {
                    ngNotify.set("Chưa chọn loại báo cáo", { duration: 3000, type: 'error' });
                } else if ($scope.target.unitCode == "-1") {
                    ngNotify.set("Chưa chọn đơn vị", { duration: 3000, type: 'error' });
                } else {
                    var modulename = $scope.nameController + "Controller";
                    var controllerRp = $scope.target.moduleName + "ReportController";
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
                    var modulename = $scope.nameController + "Controller";
                    var controllerRp = $scope.target.moduleName + "DetailsReportController";
                    initializeParam($scope.target, 1);
                    loadModeReport(controllerRp, modulename);
                }
            };

            $scope.exportExcelDetails = function () {
                var filename = "";
                switch ($scope.target.groupBy) {
                    case 1:
                        filename = "BaoCao_KiemKe_ChiTiet_TheoKhoHang";
                        break;
                    case 2:
                        filename = "BaoCao_KiemKe_ChiTiet_TheoLoaiHang";
                        break;
                    case 3:
                        filename = "BaoCao_KiemKe_ChiTiet_TheoNhomHang";
                        break;
                    case 5:
                        filename = "BaoCao_KiemKe_ChiTiet_TheoNhaCungCap";
                        break;
                    case 4:
                        filename = "BaoCao_KiemKe_ChiTiet_TheoHang";
                        break;
                    case 8:
                        filename = "BaoCao_KiemKe_ChiTiet_TheoKeHang";
                        break;
                    default:
                        filename = "BaoCao_KiemKe_ChiTiet";
                        break;
                }
                service.postExportExcelChiTiet($scope.target, filename, function () {

                });
            };
        }]);
    return app;
});