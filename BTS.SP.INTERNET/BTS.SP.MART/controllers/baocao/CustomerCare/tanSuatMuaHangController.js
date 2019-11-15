/*  
* Người tạo : Nguyễn Tuấn Hoàng Anh
* View: BTS.SP.MART/views/DCL/CustomerCare
* Vm sevices: BTS.API.SERVICE -> DCL ->CustomerCareVm.cs
* Sevices: BTS.API.SERVICE -> DCL -> CustomerCareService.cs
* Entity: BTS.API.ENTITY -> DCL - > CustomerCare.cs
* Menu: Nghiệp vụ-> khách hàng tần suất mua hàng
*/
define(['ui-bootstrap', '/BTS.SP.MART/controllers/auth/AuthController.js', '/BTS.SP.MART/controllers/htdm/periodController.js', '/BTS.SP.MART/controllers/htdm/merchandiseController.js', '/BTS.SP.MART/controllers/htdm/customerController.js', '/BTS.SP.MART/controllers/htdm/merchandiseTypeController.js', '/BTS.SP.MART/controllers/htdm/nhomVatTuController.js', '/BTS.SP.MART/controllers/htdm/supplierController.js', '/BTS.SP.MART/controllers/htdm/wareHouseController.js', '/BTS.SP.MART/controllers/htdm/packagingController.js', '/BTS.SP.MART/controllers/htdm/taxController.js', '/BTS.SP.MART/controllers/htdm/donViTinhController.js'], function () {
    'use strict';
    var app = angular.module('tanSuatMuaHangModule', ['ui.bootstrap', 'authModule', 'periodModule', 'merchandiseModule', 'customerModule', 'merchandiseTypeModule', 'nhomVatTuModule', 'supplierModule', 'wareHouseModule', 'packagingModule', 'taxModule', 'donViTinhModule']);
    app.factory('tanSuatMuaHangService', ['$http', 'configService', function ($http, configService) {
        var serviceUrl = configService.rootUrlWebApi + '/Ac/BaoCaoTanSuatMuaHang';
        var rootUrl = configService.apiServiceBaseUri;
        this.parameterPrint = {};
        var calc = {
            sum: function (obj, name) {
                var total = 0;
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
            getNewInstance: function (callback) {
                $http.get(serviceUrl + '/GetNewInstance').success(callback);
            },
            postQueryForRetail: function (data, callback) {
                $http.post(serviceUrl + '/PostQueryForRetail', data).success(callback);
            }
        }
        return result;
    }]);
    /* controller list */
    app.controller('tanSuatMuaHangController', [
        '$scope', '$location', '$http', 'configService', 'tanSuatMuaHangService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'securityService', '$rootScope', 'toaster', 'periodService', 'merchandiseService', 'customerService', 'merchandiseTypeService', 'nhomVatTuService', 'supplierService', 'wareHouseService', 'packagingService', 'taxService', 'donViTinhService', '$state',
        function ($scope, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, securityService, $rootScope, toaster, servicePeriod, serviceMerchandise, serviceCustomer, serviceMerchandiseType, serviceNhomVatTu, serviceSupplier, serviceWareHouse, servicePackaging, serviceTax, serviceDonViTinh, $state) {
            //end check
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.filtered = angular.copy(configService.filterDefault);
            $scope.tempData = tempDataService.tempData;
            $scope.isEditable = true;
            $scope.accessList = {};
            $scope.target = {};
            $scope.tagWareHouses = [];
            $scope.tagCustomers = [];
            $scope.options = {
                minDate: null,
                maxDate: null
            };
            //load dữ liệu
            function filterData() {
                service.getNewInstance(function (response) {
                    console.log(response);
                    $scope.filtered.advanceData.tuNgay = response.tuNgay;
                    $scope.filtered.advanceData.denNgay = response.denNgay;
                    $scope.filtered.advanceData.maDonVi = response.maDonVi;
                    $scope.unitUserName = response.tenDonVi;
                });
            };
            //end

            //check quyền truy cập
            function loadAccessList() {
                securityService.getAccessList('tanSuatMuaHang').then(function (successRes) {
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

            $scope.pageChanged = function () {
                var currentPage = $scope.paged.currentPage;
                var itemsPerPage = $scope.paged.itemsPerPage;
                $scope.paged.totalItems = $scope.customerCollection.length;
                $scope.data = [];
                if ($scope.customerCollection) {
                    for (var i = (currentPage - 1) * itemsPerPage; i < currentPage * itemsPerPage && i < $scope.customerCollection.length; i++) {
                        $scope.data.push($scope.customerCollection[i]);
                    }
                }
            };
            $scope.doSearch = function () {
                $scope.isLoading = true;
                $scope.filtered.isAdvance = true;
                var postData = { paged: $scope.paged, filtered: $scope.filtered };
                service.postQueryForRetail(postData, function (response) {
                    if (response.status) {
                        $scope.customerCollection = response.data;
                        $scope.isLoading = false;
                        $scope.pageChanged();
                    }
                });
            };
            //load danh muc
            $scope.displayHepler = function (paraValue, moduleName) {
                var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                if (data && data.length === 1) {
                    return data[0].text;
                } else {
                    return paraValue;
                }
            };
            $scope.report = function () {
                console.log($scope.target);
                $state.go('tanSuatMuaHangReport', { obj: $scope.target });

            }
            $scope.exportExcel = function () {
                service.postExportExcelXNTByMerchandiseByNCC($scope.target, function () {
                });
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

            function loadCustomer() {
                if (!tempDataService.tempData('customers')) {
                    serviceCustomer.getAll_Customer().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                            tempDataService.putTempData('customers', successRes.data.data);
                            $scope.customers = successRes.data.data;
                        } else {
                            console.log('successRes', successRes);
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.customers = tempDataService.tempData('customers');
                }
            }

            loadCustomer();
            loadSupplier();
            loadMerchandiseType();
            loadNhomVatTu();
            loadWareHouse();
            loadPackagings();
            loadTax();
            loadDonViTinh();

            $scope.filtered.advanceData.tagWareHouses = [];
            $scope.filtered.advanceData.tagNhaCungCaps = [];
            $scope.filtered.advanceData.tagMerchandiseTypes = [];
            $scope.filtered.advanceData.tagMerchandises = [];
            $scope.filtered.advanceData.tagMerchandiseGroups = [];

            $scope.setPage = function (pageNo) {
                $scope.paged.currentPage = pageNo;
                filterData();
            };
            $scope.sortType = 'maChungTu';
            $scope.sortReverse = false;
            $scope.doSearch = function () {
                $scope.paged.currentPage = 1;
                filterData();
            };
            $scope.pageChanged = function () {
                filterData();
            };
            $scope.goHome = function () {
                window.location.href = "#!/home";
            };
            $scope.refresh = function () {
                $scope.setPage($scope.paged.currentPage);
            };
            $scope.title = function () { return 'Phiếu xuất bán buôn'; };
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

            //filter khách hàng
            $scope.selectCustomer = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Customer', 'selectData'),
                    controller: 'customerSelectDataController',
                    resolve: {
                        serviceSelectData: function () {
                            return serviceSupplier;
                        },
                        filterObject: function () {
                            return null;
                        }
                    }
                });
                modalInstance.result.then(function () {
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };
            $scope.removeCustomer = function (index) {
                $scope.filtered.advanceData.tagNhaCungCaps.splice(index, 1);
            }
            $scope.$watch('filtered.advanceData.tagNhaCungCaps', function (newValue, oldValue) {
                if ($scope.filtered.advanceData.tagNhaCungCaps) {
                    var values = $scope.filtered.advanceData.tagNhaCungCaps.map(function (element) {
                        return element.value;
                    });
                    $scope.filtered.advanceData.nhaCungCapCodes = values.join();
                }
            }, true);
            //filter loại hàng
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
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            }
            $scope.removeMerchandiseType = function (index) {
                $scope.filtered.advanceData.tagMerchandiseTypes.splice(index, 1);
            }
            $scope.$watch('filtered.advanceData.tagMerchandiseTypes', function (newValue, oldValue) {
                if ($scope.filtered.advanceData.tagMerchandiseTypes) {
                    var values = $scope.filtered.advanceData.tagMerchandiseTypes.map(function (element) {
                        return element.value;
                    });
                    $scope.filtered.advanceData.merchandiseTypeCodes = values.join();
                }
            }, true);
            //filter Hàng hóa
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

                            }
                        }
                    }
                });
                modalInstance.result.then(function (updatedData) {
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            }
            $scope.removeMerchandise = function (index) {
                $scope.filtered.advanceData.tagMerchandises.splice(index, 1);
            }
            $scope.$watch('filtered.advanceData.tagMerchandises', function (newValue, oldValue) {
                if ($scope.filtered.advanceData.tagMerchandises) {
                    var values = $scope.filtered.advanceData.tagMerchandises.map(function (element) {
                        return element.value;
                    });
                    $scope.filtered.advanceData.merchandiseCodes = values.join();
                }
            }, true);
            //filter Nhóm hàng
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

                            }
                        }
                    }
                });
                modalInstance.result.then(function (updatedData) {
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };
            $scope.changewareHouseCodes = function (inputwareHouse) {
                if (typeof inputwareHouse != 'undefined' && inputwareHouse !== '') {
                    serviceWareHouse.filterWareHouse(inputwareHouse, function (response) {
                        if (response) {
                            $scope.data = response;
                            $scope.wareHouseCodes = '';
                            $scope.wareHouseCodes = $scope.data.maKho;
                        } else {
                            // $scope.selectWareHouse();
                        }
                    });
                }
            };
        }]);

    /* controller reportSinhNhatKhachHangController */
    app.controller('reportTanSuatMuaHangController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'tanSuatMuaHangService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', '$rootScope', 'userService', 'merchandiseService', 'customerService', 'merchandiseTypeService', 'nhomVatTuService', 'supplierService', 'wareHouseService', 'packagingService', 'taxService', 'donViTinhService', 'periodService', '$state',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, $rootScope, serviceAuthUser, serviceMerchandise, serviceCustomer, serviceMerchandiseType, serviceNhomVatTu, serviceSupplier, serviceWareHouse, servicePackaging, serviceTax, serviceDonViTinh, servicePeriod, $state) {
            var currentUser = serviceAuthUser.GetCurrentUser();
            $scope.tempData = tempDataService.tempData;
            var para = $state.params.obj;
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.robot = angular.copy(service.robot);
            $scope.target = [];
            $scope.lstSave = [];
            $scope.data = {};
            $scope.goIndex = function () {
                $state.go('tanSuatMuaHang');
            };
            $scope.isLoading = true;
            if (para) {
                service.postReporttanSuatMuaHangTongHop(para, function (response) {
                    if (response.status) {
                        $scope.data = response.data;
                        $scope.isLoading = false;
                        $scope.lstSave = $scope.data.detailData;
                    }
                    $scope.pageChanged();
                });
            };
            $scope.search = function(code) {
                $scope.lst = [];
                if ($scope.data.detailData.length > 0) {
                    if (code !== "") {
                        angular.forEach($scope.data.detailData, function(value, index) {
                            if (value.code.includes(code)) {
                                $scope.lst.push(value);
                            }
                        });
                        var currentPage = $scope.paged.currentPage;
                        var itemsPerPage = $scope.paged.itemsPerPage;
                        $scope.paged.totalItems = $scope.lst.length;
                        $scope.result = [];
                        if ($scope.lst) {
                            for (var i = (currentPage - 1) * itemsPerPage; i < currentPage * itemsPerPage && i < $scope.lst.length; i++) {
                                $scope.result.push($scope.lst[i]);
                            }
                        }
                    } else if (code === "") {
                        $scope.data.detailData = [];
                        $scope.data.detailData = $scope.lstSave;
                        $scope.pageChanged();
                    }

                }
            };
            $scope.print = function () {
                var table = document.getElementById('main-report').innerHTML;
                var myWindow = $window.open('', '', 'width=800, height=600');
                myWindow.document.write(table);
                myWindow.print();
            };
            $scope.printExcel = function() {
                service.postExportExceltanSuatMuaHangTongHop($scope.data, "BaoCaoTonTongHop");
            };
            $scope.pageChanged = function() {
                var currentPage = $scope.paged.currentPage;
                var itemsPerPage = $scope.paged.itemsPerPage;
                $scope.paged.totalItems = $scope.data.detailData.length;
                $scope.result = [];
                if ($scope.data.detailData) {
                    for (var i = (currentPage - 1) * itemsPerPage; i < currentPage * itemsPerPage && i < $scope.data.detailData.length; i++) {
                        $scope.result.push($scope.data.detailData[i]);
                    }
                }
            };
            $scope.displayHepler = function (paraValue, moduleName) {
                var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                if (data && data.length === 1) {
                    return data[0].text;
                } else {
                    return paraValue;
                }
            }
        }]);
    return app;
});

