/*  
* Người tạo : Nguyễn Tuấn Hoàng Anh
* View: BTS.SP.MART/views/nv/NvNhapHangBanTraLai
* Vm sevices: BTS.API.SERVICE -> NV ->NvNhapHangBanTraLaiVm.cs
* Sevices: BTS.API.SERVICE -> NV -> NvNhapHangBanTraLaiService.cs
* Entity: BTS.API.ENTITY -> NV - > NvNhapHangBanTraLai.cs
* Menu: Nghiệp vụ-> Nhập hàng bán trả lại
*/
define(['ui-bootstrap', '/BTS.SP.MART/controllers/auth/AuthController.js', '/BTS.SP.MART/controllers/htdm/periodController.js', '/BTS.SP.MART/controllers/htdm/merchandiseController.js', '/BTS.SP.MART/controllers/htdm/customerController.js', '/BTS.SP.MART/controllers/htdm/merchandiseTypeController.js', '/BTS.SP.MART/controllers/htdm/nhomVatTuController.js', '/BTS.SP.MART/controllers/htdm/supplierController.js', '/BTS.SP.MART/controllers/htdm/wareHouseController.js', '/BTS.SP.MART/controllers/htdm/packagingController.js', '/BTS.SP.MART/controllers/htdm/taxController.js', '/BTS.SP.MART/controllers/htdm/donViTinhController.js', '/BTS.SP.MART/controllers/nv/nvCongNoKhachHangController.js', '/BTS.SP.MART/controllers/nv/phieuXuatBanController.js'], function () {
    'use strict';
    var app = angular.module('phieuNhapHangBanTraLaiModule', ['ui.bootstrap', 'authModule', 'periodModule', 'merchandiseModule', 'customerModule', 'merchandiseTypeModule', 'nhomVatTuModule', 'supplierModule', 'wareHouseModule', 'packagingModule', 'taxModule', 'donViTinhModule', 'nvCongNoKhachHangModule', 'phieuXuatBanModule']);
    app.factory('phieuNhapHangBanTraLaiService', ['$http', 'configService', function ($http, configService) {
        var serviceUrl = configService.rootUrlWebApi + '/NV/NhapHangBanTraLai';
        var rootUrl = configService.apiServiceBaseUri;
        this.parameterPrint = {};
        function getParameterPrint() {
            return this.parameterPrint;
        }
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
            sumVat: function (tyGia, target) {
                var tienVat = 0;
                if (tyGia) {
                    tienVat = (target.thanhTienTruocVat * tyGia) / 100;
                }
                return tienVat;
            },
            changeSoLuongBao: function (item) {
                if (!item.soLuongLe) {
                    item.soLuongLe = 0;
                }
                if (!item.maBaoBi) {
                    item.luongBao = 1;
                }
                if (!item.donGia) {
                    item.donGia = 0;
                }

                item.soLuong = item.soLuongBao * item.luongBao + item.soLuongLe;
                item.thanhTien = item.soLuong * item.donGia;
            },
            changeDonGia: function (item) {
                if (!item.maBaoBi) {
                    item.luongBao = 1;
                }
                if (!item.soLuongBao) {
                    item.soLuongBao = 0;
                }
                if (!item.soLuongLe) {
                    item.soLuongLe = 0;
                }
                item.soLuong = item.soLuongBao * item.luongBao + item.soLuongLe;
                item.thanhTien = item.soLuong * item.donGia;
            },
            changeSoLuongLe: function (item, target) {
                if (!item.soLuong) {
                    item.soLuong = 0;
                }
                if (!item.donGia) {
                    item.donGia = 0;
                }
                if (!item.maBaoBi) {
                    item.luongBao = 1;
                }
                if (!item.soLuongBao) {
                    item.soLuongBao = 0;
                }
                item.soLuong = item.soLuongBao * item.luongBao + item.soLuongLe;
                item.thanhTien = item.soLuong * item.donGia;
            }
        }
        var result = {
            robot: calc,
            setParameterPrint: function (data) {
                parameterPrint = data;
            },
            getParameterPrint: function () {
                return parameterPrint;
            },
            post: function (data) {
                return $http.post(serviceUrl + '/Post', data);
            },
            postQuery: function (data) {
                return $http.post(serviceUrl + '/PostQuery', data);
            },
            postPrint: function () {
                return $http.post(serviceUrl + '/PostPrint', getParameterPrint());
            },
            postPrintDetail: function () {
                return $http.post(serviceUrl + '/PostPrintDetail', getParameterPrint());
            },
            getNewInstance: function () {
                return $http.get(serviceUrl + '/GetNewInstance');
            },
            getReport: function (id) {
                return $http.get(serviceUrl + '/GetReport/' + id);
            },
            getDetails: function (id) {
                return $http.get(serviceUrl + '/GetDetails/' + id);
            },
            getWareHouse: function (id) {
                return $http.get(rootUrl + '/api/Md/WareHouse/' + id);
            },
            getCustomer: function (id) {
                return $http.get(rootUrl + '/api/Md/Customer/' + id);
            },
            getWareHouseByCode: function (code) {
                return $http.get(rootUrl + '/api/Md/WareHouse/GetByCode/' + code);
            },
            postApproval: function (id) {
                return $http.post(serviceUrl + '/PostApproval', id);
            },
            updateCT: function (params) {
                return $http.put(serviceUrl + '/' + params.id, params);
            },
            getMerchandiseForNvByCode: function (code, wareHouseCode, unitCode) {
                return $http.get(rootUrl + '/api/Md/Merchandise/GetForNvByCode/' + code + '/' + wareHouseCode + '/' + unitCode);
            },
            postByCodeByDateTime: function (data) {
                return $http.post(rootUrl + '/api/Md/Merchandise/PostInfoCodeByDateTime', data);
            },
            deleteItem: function (params) {
                return $http.delete(serviceUrl + '/' + params.id, params);
            },
            getOrder: function () {
                return $http.get(rootUrl + '/api/Nv/DatHang/GetSelectDataIsComplete');
            }
        };
        return result;
    }]);
    /* controller list */
    app.controller('phieuNhapHangBanTraLaiController', [
        '$scope', 'configService', 'phieuNhapHangBanTraLaiService', 'tempDataService', '$filter', '$uibModal', '$log', 'securityService', '$rootScope', 'toaster', 'periodService', 'merchandiseService', 'customerService', 'merchandiseTypeService', 'nhomVatTuService', 'supplierService', 'wareHouseService', 'packagingService', 'taxService', 'donViTinhService',
        function ($scope, configService, service, tempDataService, $filter, $uibModal, $log, securityService, $rootScope, toaster, servicePeriod, serviceMerchandise, serviceCustomer, serviceMerchandiseType, serviceNhomVatTu, serviceSupplier, serviceWareHouse, servicePackaging, serviceTax, serviceDonViTinh) {
            $scope.openClosingOut = false;
            //check có mở khóa sổ không
            function checkUnClosingOut() {
                servicePeriod.checkUnClosingOut().then(function (response) {
                    if (response && response.status == 200 && response.data && response.data.data && response.data.data.length > 0) {
                        $scope.listOpen = response.data.data;
                        $scope.openClosingOut = true;
                        for (var i = 0; i < $scope.listOpen.length; i++) {
                            var dateOpen = new Date($scope.listOpen[i].fromDate);
                            angular.forEach($scope.data, function (value, idx) {
                                var ngayChungTu = new Date(value.ngayCT);
                                if (ngayChungTu.getDay() === dateOpen.getDay()) {
                                    console.log('mở khóa sổ');
                                    value.isShow = true;
                                }
                            });
                        }
                    }
                });
            }
            //end check
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.filtered = angular.copy(configService.filterDefault);
            $scope.tempData = tempDataService.tempData;
            $scope.isEditable = true;
            $scope.accessList = {};
            $scope.target = { dataDetails: [], dataClauseDetails: [] };
            $scope.robot = angular.copy(service.robot);
            //load dữ liệu
            function filterData() {
                $scope.isLoading = true;
                if ($scope.accessList.view) {
                    var postdata = {};
                    $scope.isLoading = true;
                    $scope.filtered.advanceData.tagsCustomers = serviceCustomer.getSelectData();
                    $scope.filtered.advanceData.tagMerchandiseTypes = serviceMerchandiseType.getSelectData();
                    $scope.filtered.advanceData.tagMerchandises = serviceMerchandise.getSelectData();
                    $scope.filtered.advanceData.tagMerchandiseGroups = serviceNhomVatTu.getSelectData();
                    $scope.filtered.advanceData.tagNhaCungCaps = serviceSupplier.getSelectData();
                    service.getNewInstance().then(function (response) {
                        if (response && response.status == 200 && response.data) {
                            $scope.filtered.advanceData = response.data;
                            $scope.options = $scope.filtered.advanceData.option;
                        }
                    });
                    postdata = { paged: $scope.paged, filtered: $scope.filtered };
                    service.postQuery(postdata).then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data && successRes.data.status) {
                            $scope.isLoading = false;
                            $scope.data = successRes.data.data.data;
                            checkUnClosingOut();
                            angular.extend($scope.paged, successRes.data.data);
                        }
                    });
                    $scope.$watch('target.vat', function (v, k) {
                        if (!v) {
                            $scope.target.tienVat = 0;
                        } else {
                            $scope.target.tienVat = $scope.robot.sumVat($scope.tyGia, $scope.target);
                        }
                        $scope.target.thanhTienSauVat = $scope.target.thanhTienTruocVat + $scope.target.tienVat - $scope.target.tienChietKhau;
                    });
                    $scope.$watch('target.tienChietKhau', function (v, k) {
                        $scope.target.thanhTienTruocVat = $scope.robot.sum($scope.target.dataDetails, 'thanhTien');
                        $scope.target.tienVat = $scope.robot.sumVat($scope.tyGia, $scope.target);
                        $scope.target.thanhTienSauVat = $scope.target.thanhTienTruocVat + $scope.target.tienVat - $scope.target.tienChietKhau;
                    });
                    $scope.$watch("target.dataDetails", function (newValue, oldValue) {
                        if (!$scope.target.tienChietKhau) {
                            $scope.target.tienChietKhau = 0;
                        }
                        $scope.target.thanhTienTruocVat = $scope.robot.sum($scope.target.dataDetails, 'thanhTien');
                        $scope.target.tienVat = $scope.robot.sumVat($scope.tyGia, $scope.target);
                        $scope.target.thanhTienSauVat = $scope.target.thanhTienTruocVat + $scope.target.tienVat - $scope.target.tienChietKhau;
                    }, true);
                }
            };
            //end
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
                securityService.getAccessList('phieuNhapHangBanTraLai').then(function (successRes) {
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
            //
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
            loadCustomer();
            //end
            $scope.filtered.advanceData.tagWareHouses = [];
            $scope.filtered.advanceData.tagNhaCungCaps = [];
            $scope.filtered.advanceData.tagMerchandiseTypes = [];
            $scope.filtered.advanceData.tagMerchandises = [];
            $scope.filtered.advanceData.tagMerchandiseGroups = [];

            $scope.setPage = function (pageNo) {
                $scope.paged.currentPage = pageNo;
                filterData();
            };
            $scope.sortType = 'maNvNhapHangBanTraLai';
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
            $scope.title = function () { return 'Phiếu nhập hàng bán trả lại'; };

            /* Function add New Item */
            $scope.create = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    size: 'md',
                    templateUrl: configService.buildUrl('nv/NvPhieuNhapHangBanTraLai', 'add'),
                    controller: 'phieuNhapHangBanTraLaiCreateController',
                    windowClass: 'app-modal-window',
                    resolve: {}
                });
                modalInstance.result.then(function (updatedData) {
                    $scope.refresh();
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };

            /* Function Edit Item */
            $scope.update = function (target) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('nv/NvPhieuNhapHangBanTraLai', 'update'),
                    controller: 'phieuNhapHangBanTraLaiEditController',
                    windowClass: 'app-modal-window',
                    resolve: {
                        targetData: function () {
                            return target;
                        }
                    }
                });
                modalInstance.result.then(function (updatedData) {
                    $scope.target.dataDetails = [];
                    $scope.refresh();
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };

            /* Function Details Item */
            $scope.details = function (target) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('nv/NvPhieuNhapHangBanTraLai', 'details'),
                    controller: 'phieuNhapHangBanTraLaiDetailsController',
                    windowClass: 'app-modal-window',
                    resolve: {
                        targetData: function () {
                            return target;
                        }
                    }
                });
                modalInstance.result.then(function (updatedData) {
                    $scope.target.dataDetails = [];
                    $scope.refresh();
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };

            /* Function Print Item */
            $scope.printItem = function (target) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('nv/NvPhieuNhapHangBanTraLai', 'printItem'),
                    controller: 'phieuNhapHangBanTraLaiExportItemController',
                    windowClass: 'app-modal-window',
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

            /* Function Delete Item */
            $scope.deleteItem = function (event, target) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('nv/NvPhieuNhapHangBanTraLai', 'delete'),
                    controller: 'phieuNhapHangBanTraLaiDeleteController',
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
                    })
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
            }
            $scope.removeMerchandiseGroup = function (index) {
                $scope.filtered.advanceData.tagMerchandiseGroups.splice(index, 1);
            }
            $scope.$watch('filtered.advanceData.tagMerchandiseGroups', function (newValue, oldValue) {
                if ($scope.filtered.advanceData.tagMerchandiseGroups) {
                    var values = $scope.filtered.advanceData.tagMerchandiseGroups.map(function (element) {
                        return element.value;
                    })
                    $scope.filtered.advanceData.merchandiseGroupCodes = values.join();
                }
            }, true);
            $rootScope.$on('$locationChangeStart',
            function (event, next, current) {
                if ($scope.tagsCustomers) {
                    $scope.tagsCustomers.clear();
                }
            });
            $scope.tranferFrom = function (item) {
                var modalInstance = $uibModal.open({
                    templateUrl: configService.buildUrl('nv/NvPhieuDieuChuyenNoiBo', 'add'),
                    controller: 'phieuDieuChuyenNoiBoCreateController',
                    windowClass: 'app-modal-window',
                    resolve: {
                        objectFilter: function () {
                            return {
                                maChungTu: item.maChungTu,
                                maKhoXuat: item.maKhoNhap
                            };
                        }
                    }
                });
                modalInstance.result.then(function (updatedData) {
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };
            $scope.sum = function () {
                var total = 0;
                if ($scope.data) {
                    angular.forEach($scope.data, function (v, k) {
                        total = total + v.thanhTienSauVat;
                    });
                }
                return total;
            };
            $scope.print = function () {
                var postdata = { paged: $scope.paged, filtered: $scope.filtered };
                console.log('Print', postdata);
                service.setParameterPrint(postdata);
                $state.go("nvPrintPhieuNhapMua");
            };
            $scope.printDetail = function () {
                var postdata = { paged: $scope.paged, filtered: $scope.filtered };
                service.setParameterPrint(postdata);
                $state.go("nvPrintDetailPhieuNhapHangBanTraLai");
            };
            $scope.exportExcel = function (option) {
                var postdata = {};
                switch (option) {
                    case "phieu":
                        postdata = { paged: $scope.paged, filtered: $scope.filtered };
                        service.postExportExcel(postdata);
                        break;
                    case "hangHoa":
                        postdata = { paged: $scope.paged, filtered: $scope.filtered };
                        service.postExportExcelByMerchandise(postdata);
                        break;
                    case "loaiHang":
                        postdata = { paged: $scope.paged, filtered: $scope.filtered };
                        service.postExportExcelByMerchandiseType(postdata);
                        break;
                    case "nhomHang":
                        postdata = { paged: $scope.paged, filtered: $scope.filtered };
                        service.postExportExcelByMerchandiseGroup(postdata);
                        break;
                    case "nhaCungCap":
                        postdata = { paged: $scope.paged, filtered: $scope.filtered };
                        service.postExportExcelByNhaCungCap(postdata);
                        break;
                    default:
                        postdata = { paged: $scope.paged, filtered: $scope.filtered };
                        service.postExportExcel(postdata);
                        break;
                }
            };
            $scope.exportExcelDetails = function (option) {
                $scope.filtered.advanceData.option = option;
                $scope.filtered.advanceData.fromDate = $scope.filtered.advanceData.tuNgay;
                $scope.filtered.advanceData.toDate = $scope.filtered.advanceData.denNgay;
                var filename = "";
                var postdata = {};
                switch (option) {
                    case "phieu":
                        postdata = { paged: $scope.paged, filtered: $scope.filtered };
                        filename = "XuatNhapTonChiTietTheoPhieu";
                        break;
                    case "hangHoa":
                        postdata = { paged: $scope.paged, filtered: $scope.filtered };
                        filename = "XuatNhapTonChiTietTheoHangHoa";
                        break;
                    case "loaiHang":
                        postdata = { paged: $scope.paged, filtered: $scope.filtered };
                        filename = "XuatNhapTonChiTietTheoLoaiHang";
                        break;
                    case "nhomHang":
                        postdata = { paged: $scope.paged, filtered: $scope.filtered };
                        filename = "XuatNhapTonChiTietTheoNhomHang";
                        break;
                    case "nhaCungCap":
                        postdata = { paged: $scope.paged, filtered: $scope.filtered };
                        filename = "XuatNhapTonChiTietTheoNhaCungCap";
                        break;
                    default:
                        postdata = { paged: $scope.paged, filtered: $scope.filtered };
                        break;
                }
                service.postExportExcelDetail($scope.filtered.advanceData, filename);
            }
        }]);
    /* controller addNew */
    app.controller('phieuNhapHangBanTraLaiCreateController', ['$scope', '$uibModalInstance', 'configService', 'phieuNhapHangBanTraLaiService', 'tempDataService', '$filter', '$uibModal', 'ngNotify', 'userService', 'FileUploader', 'merchandiseService', 'customerService', 'merchandiseTypeService', 'nhomVatTuService', 'supplierService', 'wareHouseService', 'packagingService', 'taxService', 'donViTinhService', 'toaster', 'periodService', 'nvCongNoKhachHangService', 'phieuXuatBanService',
    function ($scope, $uibModalInstance, configService, service, tempDataService, $filter, $uibModal, ngNotify, serviceAuthUser, FileUploader, serviceMerchandise, serviceCustomer, serviceMerchandiseType, serviceNhomVatTu, serviceSupplier, serviceWareHouse, servicePackaging, serviceTax, serviceDonViTinh, toaster, servicePeriod, nvCongNoKhachHangService, phieuXuatBanService) {
        var currentUser = serviceAuthUser.GetCurrentUser();
        var unitCode = currentUser.unitCode;
        var rootUrl = configService.apiServiceBaseUri;
        var serviceUrl = rootUrl + '/api/Nv/NhapHangBanTraLai';
        $scope.robot = angular.copy(service.robot);
        $scope.config = angular.copy(configService);
        $scope.paged = angular.copy(configService.pageDefault);
        $scope.data = [];
        $scope.newItem = {};
        $scope.donHangs = [];
        $scope.target = { dataDetails: [], dataClauseDetails: [] };
        $scope.tkKtKhoNhap = "";
        $scope.tyGia = 0;
        $scope.isListItemNull = true;
        $scope.tempData = tempDataService.tempData;
        $scope.isLoading = false;
        $scope.title = function () { return 'Phiếu nhập hàng bán trả lại'; };
        $scope.displayHepler = function (paraValue, moduleName) {
            var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
            if (data && data.length === 1) {
                return data[0].text;
            } else {
                return paraValue;
            }
        }
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
        //
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
        loadSupplier();
        loadMerchandiseType();
        loadNhomVatTu();
        loadWareHouse();
        loadPackagings();
        loadTax();
        loadDonViTinh();
        loadCustomer();
        //end 


        $scope.pageChanged = function () {
            var currentPage = $scope.paged.currentPage;
            var itemsPerPage = $scope.paged.itemsPerPage;
            if ($scope.target && $scope.target.dataDetails) $scope.paged.totalItems = $scope.target.dataDetails.length;
            $scope.data = [];
            if ($scope.target.dataDetails) {
                for (var i = (currentPage - 1) * itemsPerPage; i < currentPage * itemsPerPage && i < $scope.target.dataDetails.length; i++) {
                    $scope.data.push($scope.target.dataDetails[i]);
                }
            }
        }
        function filterData() {
            $scope.isLoading = true;
            service.getNewInstance().then(function (response) {
                if (response && response.status == 200 && response.data) {
                    $scope.target = response.data;
                    servicePeriod.getKyKeToan().then(function (response) {
                        if (response && response.status == 200 && response.data) {
                            $scope.target.ngayCT = new Date(response.data.toDate);
                        }
                    });
                    $scope.pageChanged();
                    $scope.isLoading = false;
                    $scope.$watch('target.vat', function (v, k) {
                        if (!v) {
                            $scope.target.tienVat = 0;
                        } else {
                            $scope.target.tienVat = $scope.robot.sumVat($scope.tyGia, $scope.target);
                        }
                        $scope.target.thanhTienSauVat = $scope.target.thanhTienTruocVat + $scope.target.tienVat - $scope.target.tienChietKhau;
                    });
                    $scope.$watch('target.tienChietKhau', function (v, k) {
                        $scope.target.thanhTienTruocVat = $scope.robot.sum($scope.target.dataDetails, 'thanhTien');
                        $scope.target.tienVat = $scope.robot.sumVat($scope.tyGia, $scope.target);
                        $scope.target.thanhTienSauVat = $scope.target.thanhTienTruocVat + $scope.target.tienVat - $scope.target.tienChietKhau;
                    });
                    $scope.$watch("target.dataDetails", function (newValue, oldValue) {
                        if (!$scope.target.tienChietKhau) {
                            $scope.target.tienChietKhau = 0;
                        }
                        $scope.target.thanhTienTruocVat = $scope.robot.sum($scope.target.dataDetails, 'thanhTien');
                        $scope.target.tienVat = $scope.robot.sumVat($scope.tyGia, $scope.target);
                        $scope.target.thanhTienSauVat = $scope.target.thanhTienTruocVat + $scope.target.tienVat - $scope.target.tienChietKhau;
                    }, true);
                }
            });
            service.getOrder().then(function (response) {
                if (response && response.status == 200 && response.data) {
                    $scope.donHangs = response.data;
                }
            });
        };
        filterData();
        $scope.changeTongTien = function () {
            $scope.pageChanged();
        }
        $scope.addRow = function () {
            if (!$scope.newItem.soLuong || $scope.newItem.soLuong < 1) {
                focus('soluong');
                document.getElementById('soluong').focus();
                return;
            }
            if (($scope.target.dataDetails.length != 0) && ($scope.newItem.tyLeVatVao != $scope.target.dataDetails[0].tyLeVatVao)) {
                //load
                toaster.pop('error', "Lỗi:", "Các mặt hàng phải cùng loại VAT" + $scope.target.dataDetails[0].tyLeVatVao + "%");
                return;
            }
            if ($scope.newItem.validateCode == $scope.newItem.maHang) {
                var exsist = $scope.target.dataDetails.some(function (element, index, array) {
                    return $scope.newItem.maHang == element.maHang;
                });
                if (exsist) {
                    toaster.pop('success', "Thông báo:", "Mã hàng này bạn đã nhập rồi. Cộng gộp");
                    angular.forEach($scope.target.dataDetails, function (v, k) {
                        if (v.maHang == $scope.newItem.maHang) {
                            $scope.target.dataDetails[k].soLuong = parseInt($scope.newItem.soLuong) + parseInt($scope.target.dataDetails[k].soLuong);
                            $scope.target.dataDetails[k].soLuongBao = parseInt($scope.newItem.soLuongBao) + parseInt($scope.target.dataDetails[k].soLuongBao);
                            $scope.target.dataDetails[k].soLuongLe = parseInt($scope.newItem.soLuongLe) + parseInt($scope.target.dataDetails[k].soLuongLe);
                            $scope.target.dataDetails[k].thanhTien = parseInt($scope.newItem.soLuong) * parseInt($scope.target.dataDetails[k].donGia);
                            service.robot.changeSoLuongLe($scope.target.dataDetails[k], $scope.target);
                        }
                    });
                } else {
                    $scope.target.dataDetails.push($scope.newItem);
                }
                $scope.isListItemNull = false;
            } else {
                toaster.pop('error', "Lỗi:", "Mã hàng chưa đúng!");
            }
            $scope.pageChanged();
            $scope.newItem = {};
            focus('mahang');
            document.getElementById('mahang').focus();
        };

        $scope.addNewItem = function (strKey) {
            var modalInstance = $uibModal.open({
                backdrop: 'static',
                templateUrl: configService.buildUrl('htdm/Merchandise', 'selectData'),
                controller: 'merchandiseSelectDataController',
                windowClass: 'app-modal-window',
                resolve: {
                    serviceSelectData: function () {
                        return serviceMerchandise;
                    },
                    filterObject: function () {
                        return {
                            summary: strKey
                        };
                    }
                }
            });
            modalInstance.result.then(function (updatedData) {
                if (!updatedData.selected) {
                    $scope.newItem = updatedData;
                    service.getMerchandiseForNvByCode(updatedData.maHang, $scope.target.maKhoNhap, unitCode).then(function (response) {
                        if (response && response.status == 200 && response.data && response.data.status) {
                            $scope.newItem = response.data.data;
                            $scope.newItem.donGia = response.data.data.giaVon;
                            $scope.newItem.giaBanLe = updatedData.giaBanLe;
                            $scope.newItem.validateCode = updatedData.maHang;
                            $scope.newItem.giaMuaCoVat = response.data.data.giaVon * (1 + response.data.data.tyLeVatVao / 100);
                            $scope.newItem.soLuongTon = response.data.data.soLuongTon;
                            $scope.newItem.giaVon = response.data.data.giaVon;
                        }
                        else {
                            updatedData.donGia = updatedData.giaMua;
                            $scope.newItem.validateCode = updatedData.maHang;
                            $scope.newItem.giaBanLe = updatedData.giaBanLe;
                            $scope.newItem.giaMuaCoVat = updatedData.giaMua * (1 + updatedData.tyLeVatVao / 100);
                        }
                    });
                }
                $scope.pageChanged();
            }, function () {

            });
        };
        $scope.removeItem = function (index) {
            var currentPage = $scope.paged.currentPage;
            var itemsPerPage = $scope.paged.itemsPerPage;
            var currentPageIndex = (currentPage - 1) * itemsPerPage + index;
            $scope.target.dataDetails.splice(currentPageIndex, 1);
            if ($scope.target.dataDetails.length == 0) {
                $scope.isListItemNull = true;
            }
            $scope.pageChanged();
        };

        //vudq import excel
        $scope.downloadTemplate = function () {
            kmComboService.dowloadTemplateExcel('TemplateExcel-KhuyenMaiCombo');
        };
        var uploader = $scope.uploader = new FileUploader({
            url: serviceUrl + '/ImportExcelKhuyenMaiCombo/' + unitCode
        });
        uploader.filters.push({
            name: 'syncFilter',
            fn: function (item, options) {
                return this.queue.length < 10;
            }
        });
        uploader.filters.push({
            name: 'asyncFilter',
            fn: function (item, options, deferred) {
                setTimeout(deferred.resolve, 1e3);
            }
        });
        uploader.onSuccessItem = function (fileItem, response, status, headers) {
            if (status === 200 && response.data) {
                console.log('response kmCombo:', response);
                if (response.data.length > 0) {
                    for (var i = 0; i < response.data.length; i++) {
                        $scope.target.dataDetails.push(response.data[i]);
                    }
                    $scope.pageChanged();
                }
            }
        };
        //end vudq import excel
        $scope.selectedkhachHang = function (item) {
            $scope.target.maKhachHang = item.value;
            //service.getOrderByCustomer(item.value).then(function (response) {
            //    if (response && response && response.data.length > 0) {
            //        $scope.donHangs = response.data;
            //    }
            //});
            service.getCustomer(item.id).then(function (response) {
                if (response && response && response.data) {
                    $scope.target.maSoThue = response.data.maSoThue;
                }
            });
            nvCongNoKhachHangService.getAmmountCustomerBorrowed(item.value).then(function (response) {
                if (response.status === 200) {
                    $scope.target.tienNoCu = response.data.thanhTienCanTra;
                    $scope.target.noiDung = "Nợ cũ " + Math.round(response.data.thanhTienCanTra / 1000) + "k";
                }
            });
        };
        $scope.selectedDonDatHang = function (item) {
            $scope.isLoading = true;
            service.getOrderById(item.id).then(function (response) {
                var donHang = response.data;
                $scope.target.dataDetails.clear();
                angular.forEach(donHang.dataDetails, function (v, k) {
                    $scope.target.dataDetails.push(v);
                });
                if ($scope.target.dataDetails.length > 0) {
                    $scope.isListItemNull = false;
                }
                $scope.isLoading = false;
                $scope.pageChanged();
            });
        }
        $scope.selectedTkCo = function (item) {
            $scope.target.tkCo = item.value;
        };
        $scope.selectedMaHang = function (code) {
            if (code) {
                service.getMerchandiseForNvByCode(code, $scope.target.maKhoNhap, unitCode).then(function (response) {
                    if (response && response.status == 200 && response.data && response.data.status) {
                        $scope.newItem = response.data.data;
                        $scope.newItem.donGia = $scope.newItem.giaMua;
                        $scope.newItem.validateCode = response.data.data.maHang;
                        $scope.newItem.giaMuaCoVat = response.data.data.giaMua * (1 + response.data.data.tyLeVatVao / 100);
                        $scope.newItem.soLuongTon = response.data.data.soLuongTon;
                        $scope.newItem.giaVon = response.data.data.giaVon;
                    }
                    else {
                        $scope.addNewItem(code);

                    }
                });
            }
        }
        $scope.selectedTax = function (target) {
            for (var i = 0; i < $scope.tempData('taxs').length; i++) {
                var tmp = $scope.tempData('taxs')[i];
                if (target.vat == tmp.value) {
                    $scope.tyGia = tmp.extendValue;
                }
            }
        };
        $scope.selectedKhoNhap = function (item) {
            $scope.target.maKhoNhap = item.value;
            service.getWareHouse(item.id).then(function (response) {
                if (response && response.status == 200 && response.data) {
                    $scope.tkKtKhoNhap = response.taiKhoanKt;
                }
            });
        }
        $scope.selectedMaBaoBi = function (model, item) {
            if (!model.soLuongBao) {
                model.soLuongBao = 0;
            }
            if (!model.donGia) {
                model.donGia = 0;
            }
            if (!model.soLuongLe) {
                model.soLuongLe = 0;
            }
            model.luongBao = parseFloat(item.extendValue);
            model.soLuong = model.soLuongBao * model.luongBao + model.soLuongLe;
            model.thanhTien = model.soLuong * model.donGia;
        }
        //log2

        $scope.save = function () {
            if ($scope.data && $scope.data.length > 0 && $scope.target && $scope.target.dataDetails.length > 0) {
                angular.forEach($scope.data, function (v, k) {
                    var row = $filter('filter')($scope.target.dataDetails, { maHang: v.maHang }, true);
                    if (row && row.length === 1) {
                        if (!v.soLuong) {
                            row[0].soLuong = 0;
                        }
                        if (!v.donGia) {
                            row[0].donGia = 0;
                        }
                        if (!v.maBaoBi) {
                            row[0].luongBao = 1;
                        }
                        if (!v.soLuongBao) {
                            row[0].soLuongBao = 0;
                        }
                        if (!v.soLuongLe) {
                            row[0].soLuongLe = 0;
                        }
                        if (v) {
                            row[0].soLuong = parseFloat(v.soLuongBao) * parseFloat(v.luongBao) + parseFloat(v.soLuongLe);
                            row[0].soLuongLe = parseFloat(v.soLuongLe);
                            row[0].thanhTien = v.soLuong * parseFloat(v.donGia);
                            row[0].thanhTienVAT = parseFloat(v.thanhTien) * (1 + (v.tyLeVatVao) / 100);
                            row[0].tienTruocGiamGia = row[0].thanhTien;
                        }
                    }
                });
            }
            $scope.target.ngayCT = $filter('date')($scope.target.ngayCT);
            $scope.target.maNhanVien = currentUser.userName;
            //index để sắp xếp theo mã hàng lúc thêm
            if ($scope.target.dataDetails.length > 0) {
                angular.forEach($scope.target.dataDetails, function (value, index) {
                    $scope.target.dataDetails.index = index;
                });
            }
            service.post($scope.target).then(function (successRes) {
                if (successRes && successRes.status === 201 && successRes.data) {
                    ngNotify.set("Thêm mới thành công", { type: 'success' });
                    $uibModalInstance.close($scope.target);
                } else {
                    console.log('addNew successRes', successRes);
                    ngNotify.set(successRes.data.message, { duration: 3000, type: 'error' });
                }
            },
                function (errorRes) {
                    console.log('errorRes', errorRes);
                });
        };
        $scope.selectedBill = function (target) {
            phieuXuatBanService.getPhieuByMaPhieu(target).then(function (response) {
                if (response.status === 200 && response.data != '') {
                    phieuXuatBanService.getDetails(response.data).then(function (res) {
                        if (res.status === 200 && res.data != null) {
                            $scope.target.maHoaDon = res.data.maChungTu;
                            $scope.target.maKhachHang = res.data.maKhachHang;
                            $scope.target.dataDetails = angular.copy(res.data.dataDetails);
                            $scope.pageChanged();
                        }
                    });
                } else {
                    var modalInstance = $uibModal.open({
                        backdrop: 'static',
                        templateUrl: configService.buildUrl('nv/NvXuatBan', 'selectData'),
                        controller: 'phieuXuatBanSelectedController',
                        windowClass: 'app-modal-window',
                        resolve: {
                            serviceSelectData: function () {
                                return phieuXuatBanService;
                            },
                            filterObject: function () {
                                return {
                                    summary: target
                                };
                            }
                        }
                    });
                    modalInstance.result.then(function (updatedData) {
                        phieuXuatBanService.getDetails(updatedData).then(function (res) {
                            if (res.status === 200 && res.data != null) {
                                $scope.target.maHoaDon = res.data.maChungTu;
                                $scope.target.maKhachHang = res.data.maKhachHang;
                                $scope.target.dataDetails = angular.copy(res.data.dataDetails);
                                $scope.pageChanged();
                            }
                        });

                    }, function () {
                    });
                }

            });
        }
        $scope.cancel = function () {
            $scope.isListItemNull = true;
            $uibModalInstance.dismiss('cancel');
        };
    }]);
    /* controller Edit */
    app.controller('phieuNhapHangBanTraLaiEditController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'phieuNhapHangBanTraLaiService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify', 'merchandiseService', 'toaster', 'nvCongNoKhachHangService',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify, serviceMerchandise, toaster, nvCongNoKhachHangService) {
            $scope.config = angular.copy(configService);
            $scope.robot = angular.copy(service.robot);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.targetData = angular.copy(targetData);
            $scope.tempData = tempDataService.tempData;
            $scope.target = targetData;
            $scope.newItem = {};
            $scope.tkKtKhoNhap = "";
            $scope.tyGia = 0;

            $scope.isLoading = true;
            $scope.title = function () { return 'Cập nhật phiếu nhập hàng bán trả lại'; };
            $scope.pageChanged = function () {
                var currentPage = $scope.paged.currentPage;
                var itemsPerPage = $scope.paged.itemsPerPage;
                if ($scope.target && $scope.target.dataDetails) $scope.paged.totalItems = $scope.target.dataDetails.length;
                $scope.data = [];
                if ($scope.target.dataDetails) {
                    for (var i = (currentPage - 1) * itemsPerPage; i < currentPage * itemsPerPage && i < $scope.target.dataDetails.length; i++) {
                        $scope.data.push($scope.target.dataDetails[i]);
                    }
                }
            };
            function filterData() {
                service.getDetails($scope.target.id).then(function (response) {
                    if (response && response.status == 200 && response.data) {
                        $scope.target = response.data;
                        $scope.target.ngayCT = new Date($scope.target.ngayCT);
                        $scope.target.ngayHoaDon = new Date($scope.target.ngayHoaDon);
                        if ($scope.target.dataDetails.length > 0) {
                            $scope.isListItemNull = false;
                        }
                        serviceMerchandise.setSelectData($scope.target.dataDetails);
                        $scope.pageChanged();

                        var z = $filter('filter')($scope.tempData('taxs'), { value: $scope.target.vat }, true);
                        if (z.length > 0) {
                            $scope.tyGia = z[0].extendValue;
                        }
                        else {
                            $scope.tyGia = 0;
                        }
                        $scope.$watch('target.vat', function (v, k) {
                            if (!v) {
                                $scope.target.tienVat = 0;
                            } else {
                                $scope.target.tienVat = $scope.robot.sumVat($scope.tyGia, $scope.target);
                            }
                            $scope.target.thanhTienSauVat = $scope.target.thanhTienTruocVat + $scope.target.tienVat - $scope.target.tienChietKhau;
                        });
                        $scope.$watch('target.tienChietKhau', function (v, k) {
                            $scope.target.thanhTienTruocVat = $scope.robot.sum($scope.target.dataDetails, 'thanhTien');
                            $scope.target.tienVat = $scope.robot.sumVat($scope.tyGia, $scope.target);
                            $scope.target.thanhTienSauVat = $scope.target.thanhTienTruocVat + $scope.target.tienVat - $scope.target.tienChietKhau;
                        });
                        $scope.$watch("target.dataDetails", function (newValue, oldValue) {
                            if (!$scope.target.tienChietKhau) {
                                $scope.target.tienChietKhau = 0;
                            }
                            $scope.target.thanhTienTruocVat = $scope.robot.sum($scope.target.dataDetails, 'thanhTien');
                            $scope.target.tienVat = $scope.robot.sumVat($scope.tyGia, $scope.target);
                            $scope.target.thanhTienSauVat = $scope.target.thanhTienTruocVat + $scope.target.tienVat - $scope.target.tienChietKhau;
                        }, true);
                    }
                });
            };
            filterData();
            $scope.displayHepler = function (paraValue, moduleName) {
                var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                if (data && data.length === 1) {
                    return data[0].text;
                } else {
                    return paraValue;
                }
            }
            $scope.addRow = function () {
                if (!$scope.newItem.soLuong || $scope.newItem.soLuong < 1) {
                    focus('soluong');
                    document.getElementById('soluong').focus();
                    return;
                }
                if (($scope.target.dataDetails.length != 0) && ($scope.newItem.tyLeVatVao != $scope.target.dataDetails[0].tyLeVatVao)) {
                    toaster.pop('error', "Thông báo:", "Các mặt hàng phải cùng loại VAT" + $scope.target.dataDetails[0].tyLeVatVao + "%");
                    return;
                }
                if ($scope.newItem.validateCode == $scope.newItem.maHang) {
                    var exsist = $scope.target.dataDetails.some(function (element, index, array) {
                        return $scope.newItem.maHang == element.maHang;
                    });
                    if (exsist) {
                        toaster.pop('success', "Thông báo:", "Mã hàng này bạn đã nhập rồi. Cộng gộp");
                        angular.forEach($scope.target.dataDetails, function (v, k) {
                            if (v.maHang == $scope.newItem.maHang) {
                                $scope.target.dataDetails[k].soLuong = parseInt($scope.newItem.soLuong) + parseInt($scope.target.dataDetails[k].soLuong);
                                $scope.target.dataDetails[k].soLuongBao = parseInt($scope.newItem.soLuongBao) + parseInt($scope.target.dataDetails[k].soLuongBao);
                                $scope.target.dataDetails[k].soLuongLe = parseInt($scope.newItem.soLuongLe) + parseInt($scope.target.dataDetails[k].soLuongLe);
                                $scope.target.dataDetails[k].thanhTien = parseInt($scope.newItem.soLuong) * parseInt($scope.target.dataDetails[k].donGia);
                                service.robot.changeSoLuongLe($scope.target.dataDetails[k], $scope.target);
                            }
                        });
                    } else {
                        $scope.target.dataDetails.push($scope.newItem);
                    }
                    $scope.isListItemNull = false;
                } else {
                    toaster.pop('error', "Thông báo:", "Mã hàng chưa đúng");
                }
                $scope.pageChanged();
                $scope.newItem = {};
                focus('mahang');
                document.getElementById('mahang').focus();
            };

            $scope.addNewItem = function (strKey) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Merchandise', 'selectData'),
                    controller: 'merchandiseSelectDataController',
                    windowClass: 'app-modal-window',
                    resolve: {
                        serviceSelectData: function () {
                            return serviceMerchandise;
                        },
                        filterObject: function () {
                            return {
                                summary: strKey
                            };
                        }
                    }
                });
                modalInstance.result.then(function (updatedData) {
                    if (!updatedData.selected) {
                        $scope.newItem = updatedData;
                        service.getMerchandiseForNvByCode(updatedData.maHang, $scope.target.maKhoNhap, unitCode).then(function (response) {
                            if (response && response.status === 200 && response.data && response.data.status) {
                                $scope.newItem = response.data.data;
                                $scope.newItem.donGia = response.data.data.giaVon;
                                $scope.newItem.validateCode = updatedData.maHang;
                                $scope.newItem.giaBanLe = updatedData.giaBanLe;
                                $scope.newItem.giaMuaCoVat = response.data.data.giaVon * (1 + response.data.data.tyLeVatVao / 100);
                                $scope.newItem.soLuongTon = response.data.data.soLuongTon;
                                $scope.newItem.giaVon = response.data.data.giaVon;
                            }
                            else {
                                updatedData.donGia = updatedData.giaMua;
                                $scope.newItem.validateCode = updatedData.maHang;
                                $scope.newItem.giaBanLe = updatedData.giaBanLe;
                                $scope.newItem.giaMuaCoVat = updatedData.giaMua * (1 + updatedData.tyLeVatVao / 100);
                            }
                        });
                    }
                    $scope.pageChanged();
                }, function () {
                });
            }

            $scope.removeItem = function (index) {
                var currentPage = $scope.paged.currentPage;
                var itemsPerPage = $scope.paged.itemsPerPage;
                var currentPageIndex = (currentPage - 1) * itemsPerPage + index;
                $scope.target.dataDetails.splice(currentPageIndex, 1);
                $scope.pageChanged();
            };
            $scope.selectedkhachHang = function (item) {
                //service.getOrderByCustomer(item.value).then(function (response) {
                //    if (response && response && response.data.length > 0) {
                //        $scope.donHangs = response.data;
                //    }
                //});
                service.getCustomer(item.id).then(function (response) {
                    if (response && response && response.data) {
                        $scope.target.maSoThue = response.data.maSoThue;
                    }
                });
                nvCongNoKhachHangService.getAmmountCustomerBorrowed(item.value).then(function (response) {
                    if (response.status === 200) {
                        $scope.target.tienNoCu = response.data.thanhTienCanTra;
                        $scope.target.noiDung = "Nợ cũ " + Math.round(response.data.thanhTienCanTra / 1000) + "k";
                    }
                });
            };
            $scope.selectedDonDatHang = function (item) {
                $scope.isLoading = true;
                service.getOrderById(item.id).then(function (response) {
                    var donHang = response.data;
                    $scope.target.dataDetails.clear();
                    angular.forEach(donHang.dataDetails, function (v, k) {
                        $scope.target.dataDetails.push(v);
                    });
                    if ($scope.target.dataDetails.length > 0) {
                        $scope.isListItemNull = false;
                    }
                    $scope.isLoading = false;
                    $scope.pageChanged();
                });
            }
            $scope.selectedTkCo = function (item) {
                $scope.target.tkCo = item.value;
            };
            $scope.selectedMaHang = function (code) {
                if (code) {
                    service.getMerchandiseForNvByCode(code, $scope.target.maKhoNhap, unitCode).then(function (response) {
                        if (response && response.status == 200 && response.data && response.data.status) {
                            $scope.newItem = response.data.data;
                            $scope.newItem.donGia = $scope.newItem.giaMua;
                            $scope.newItem.validateCode = response.data.data.maHang;
                            $scope.newItem.giaMuaCoVat = response.data.data.giaMua * (1 + response.data.data.tyLeVatVao / 100);
                            $scope.newItem.soLuongTon = response.data.data.soLuongTon;
                            $scope.newItem.giaVon = response.data.data.giaVon;
                        } else {
                            $scope.addNewItem(code);
                        }
                    });
                }
            };
            $scope.selectedTax = function (target) {
                for (var i = 0; i < $scope.tempData('taxs').length; i++) {
                    var tmp = $scope.tempData('taxs')[i];
                    if (target.vat == tmp.value) {
                        $scope.tyGia = tmp.extendValue;
                    }
                }
            };
            $scope.selectedKhoNhap = function (item) {
                $scope.target.maKhoNhap = item.value;
                service.getWareHouse(item.id).then(function (response) {
                    $scope.tkKtKhoNhap = response.taiKhoanKt;
                });
            }
            $scope.selectedMaBaoBi = function (model, item) {
                if (!model.soLuongBao) {
                    model.soLuongBao = 0;
                }
                if (!model.donGia) {
                    model.donGia = 0;
                }
                if (!model.soLuongLe) {
                    model.soLuongLe = 0;
                }
                model.luongBao = parseFloat(item.extendValue);
                model.soLuong = model.soLuongBao * model.luongBao + model.soLuongLe;
                model.thanhTien = model.soLuong * model.donGia;
            }

            $scope.save = function () {
                //index để sắp xếp theo mã hàng lúc thêm
                if ($scope.target.dataDetails.length > 0) {
                    angular.forEach($scope.target.dataDetails, function (value, index) {
                        $scope.target.dataDetails.index = index;
                    });
                }
                service.updateCT($scope.target).then(function (successRes) {
                    if (successRes && successRes.status === 200 && successRes.data) {
                        ngNotify.set("Thêm mới thành công", { type: 'success' });
                        $uibModalInstance.close($scope.target);
                    } else {
                        console.log('update successRes', successRes);
                        ngNotify.set(successRes.data.message, { duration: 3000, type: 'error' });
                    }
                },
                    function (response) {
                        console.log('ERROR: Update failed! ' + response);
                    }
                );
            };
            $scope.saveAndPrint = function () {
                service.updateCT($scope.target).then(function (successRes) {
                    if (successRes && successRes.status === 200 && successRes.data.status) {
                        ngNotify.set(successRes.data.message, { type: 'success' });
                        var url = $state.href('reportPhieuNhapHangBanTraLai', { id: successRes.data.data.id });
                        window.open(url, 'Report Viewer');
                        $scope.target.dataDetails.clear();
                        $uibModalInstance.close($scope.target);
                    } else {
                        console.log('update successRes', successRes);
                        ngNotify.set(successRes.data.message, { duration: 3000, type: 'error' });
                    }
                },
                    function (response) {
                        console.log('ERROR: Update failed! ' + response);
                    }
                );
            };
            $scope.createWareHouse = function (target, name) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/WareHouse', 'add'),
                    controller: 'wareHouseCreateController',
                    resolve: {}
                });
                modalInstance.result.then(function (updatedData) {
                    $scope.tempData.update('wareHouses', function () {
                        if (target && name) {
                            target[name] = updatedData.maKho;
                        }
                    });
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };
            $scope.createMerchandise = function (target, name) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Merchandise', 'add'),
                    controller: 'merchandiseCreateController',
                    windowClass: 'app-modal-window',
                    resolve: {}
                });
                modalInstance.result.then(function (updatedData) {
                    $scope.tempData.update('merchandises', function () {
                        if (target && name) {
                            target[name] = updatedData.maVatTu;
                        }
                    });
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };
            $scope.createPackage = function (target, name) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Packaging', 'add'),
                    controller: 'packagingCreateController',
                    resolve: {}
                });
                modalInstance.result.then(function (updatedData) {
                    $scope.tempData.update('packagings', function () {
                        if (target && name) {
                            target[name] = updatedData.maBaoBi;
                            target.luongBao = updatedData.soLuong;
                        }
                    });
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };
            $scope.createCustomer = function (target, name) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Customer', 'add'),
                    controller: 'customerCreateController',
                    resolve: {}
                });
                modalInstance.result.then(function (updatedData) {
                    $scope.tempData.update('customers', function () {
                        if (target && name) {
                            target[name] = updatedData.maKH;
                        }
                    });
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };
            $scope.save = function () {
                //index để sắp xếp theo mã hàng lúc thêm
                if ($scope.target.dataDetails.length > 0) {
                    angular.forEach($scope.target.dataDetails, function (value, index) {
                        $scope.target.dataDetails.index = index;
                    });
                }
                service.updateCT($scope.target).then(function (successRes) {
                    if (successRes && successRes.status === 200 && successRes.data) {
                        ngNotify.set("Cập nhật thành công", { type: 'success' });
                        $uibModalInstance.close($scope.target);
                    } else {
                        console.log('update successRes', successRes);
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
    /* controller Details */
    app.controller('phieuNhapHangBanTraLaiDetailsController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'phieuNhapHangBanTraLaiService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify) {
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.tempData = tempDataService.tempData;
            $scope.target = targetData;
            $scope.target.ngayCT = new Date(targetData.ngayCT);
            $scope.isLoading = false;
            $scope.title = function () { return 'Thông tin phiếu nhập hàng bán trả lại'; };
            $scope.pageChanged = function () {
                var currentPage = $scope.paged.currentPage;
                var itemsPerPage = $scope.paged.itemsPerPage;
                if ($scope.target && $scope.target.dataDetails) $scope.paged.totalItems = $scope.target.dataDetails.length;
                $scope.data = [];
                if ($scope.target.dataDetails) {
                    for (var i = (currentPage - 1) * itemsPerPage; i < currentPage * itemsPerPage && i < $scope.target.dataDetails.length; i++) {
                        $scope.target.dataDetails[i].thanhTienVAT = $scope.target.dataDetails[i].thanhTien * (1 + $scope.target.dataDetails[i].tyLeVatVao / 100);
                        $scope.target.dataDetails[i].giaMuaCoVat = $scope.target.dataDetails[i].giaMua * (1 + $scope.target.dataDetails[i].tyLeVatVao / 100);
                        $scope.data.push($scope.target.dataDetails[i]);
                    }
                }
            }
            //note
            function fillterData() {
                $scope.isLoading = true;
                service.getDetails($scope.target.id).then(function (resgetDetails) {
                    if (resgetDetails && resgetDetails.status == 200 && resgetDetails.data) {
                        $scope.target = resgetDetails.data;
                        $scope.target.ngayCT = new Date($scope.target.ngayCT);
                        $scope.target.ngayHoaDon = new Date($scope.target.ngayHoaDon);
                    }
                    $scope.isLoading = false;
                    $scope.pageChanged();
                });
            }
            fillterData();
            $scope.displayHepler = function (paraValue, moduleName) {
                var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                if (data && data.length === 1) {
                    return data[0].text;
                } else {
                    return paraValue;
                }
            }
            $scope.approval = function () {
                service.postApproval($scope.target).then(function (response) {
                    if (response) {
                        alert("Duyệt thành công!");
                        $uibModalInstance.close($scope.target);
                        $scope.goIndex = function () {
                            $state.go('nvNhapHangBanTraLai');
                        };
                    }
                    else { alert("Thất bại! - Xảy ra lỗi hoặc phiếu này đã duyệt"); }
                });
            };
            $scope.cancel = function () {
                $uibModalInstance.close();
            };

        }]);
    /* controller delete */
    app.controller('phieuNhapHangBanTraLaiDeleteController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'phieuNhapHangBanTraLaiService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify',
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
                        console.log('deleteItem successRes ', successRes);
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
    /* report Phieu Nhap Hang Mua Controller */
    app.controller('reportPhieuNhapHangBanTraLaiController', ['$scope', '$location', '$http', 'configService', 'phieuNhapHangBanTraLaiService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'userService', '$stateParams', '$window',
        function ($scope, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, serviceAuthUser, $stateParams, $window) {
            var currentUser = serviceAuthUser.GetCurrentUser();
            $scope.robot = angular.copy(service.robot);
            var id = $stateParams.id;
            $scope.target = {};
            $scope.goIndex = function () {
                $state.go('nvNhapHangBanTraLai');
            }
            function filterData() {
                if (id) {
                    service.getReport(id).then(function (response) {
                        if (response && response.status && response.data) {
                            $scope.target = response.data;
                        }
                    });
                    $scope.currentUser = currentUser.userName;
                }
            };
            filterData();
            $scope.displayHepler = function (paraValue, moduleName) {
                var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                if (data && data.length === 1) {
                    return data[0].text;
                } else {
                    return paraValue;
                }
            }
            $scope.checkDuyet = function () {
                if ($scope.target.trangThai == 10) {
                    return false;
                } else {
                    return true;
                }
            };
            $scope.print = function () {
                var table = document.getElementById('main-report').innerHTML;
                var myWindow = $window.open('', '', 'width=800, height=600');
                myWindow.document.write(table);
                myWindow.print();
            }
            $scope.printExcel = function () {
                var data = [document.getElementById('main-report').innerHTML];
                var fileName = "NhapHangBanTraLai_ExportData.xls";
                var filetype = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=utf-8";
                var ieEDGE = navigator.userAgent.match(/Edge/g);
                var ie = navigator.userAgent.match(/.NET/g); // IE 11+
                var oldIE = navigator.userAgent.match(/MSIE/g);
                if (ie || oldIE || ieEDGE) {
                    var blob = new window.Blob(data, { type: filetype });
                    window.navigator.msSaveBlob(blob, fileName);
                }
                else {
                    var a = $("<a style='display: none;'/>");
                    var url = window.webkitURL.createObjectURL(new Blob(data, { type: filetype }));
                    a.attr("href", url);
                    a.attr("download", fileName);
                    $("body").append(a);
                    a[0].click();
                    window.url.revokeObjectURL(url);
                    a.remove();
                }
            }
        }]);
    /* print Phieu Nhap Hang Mua Controller */
    app.controller('printPhieuNhapHangBanTraLaiController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'phieuNhapHangBanTraLaiService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify) {
            $scope.robot = angular.copy(service.robot);
            $scope.config = angular.copy(configService);
            $scope.targetData = angular.copy(targetData);
            $scope.target = targetData;
            var id = $stateParams.id;
            $scope.target = {};
            $scope.info = service.getParameterPrint().filtered.advanceData;
            $scope.goIndex = function () {
                $state.go('nvNhapHangBanTraLai');
            }
            function filterData() {
                service.postPrint().then(function (response) {
                    $scope.printData = response;
                });
            };
            filterData();
            $scope.printExcel = function () {
                var data = [document.getElementById('dataTable').innerHTML];
                configService.saveExcel(data, "Danh_sach");
            }
            $scope.sum = function () {
                var total = 0;
                if ($scope.printData) {
                    angular.forEach($scope.printData, function (v, k) {
                        total = total + v.thanhTienSauVat;
                    });
                }
                return total;
            }
            $scope.print = function () {
                var table = document.getElementById('dataTable').innerHTML;
                var myWindow = $window.open('', '', 'width=800, height=600');
                myWindow.document.write(table);
                myWindow.print();
            }
            $scope.cancel = function () {
                $uibModalInstance.close();
            };
        }]);
    /* print Detail Phieu Nhap Hang Mua Controller */
    app.controller('printDetailPhieuNhapHangBanTraLaiController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'phieuNhapHangBanTraLaiService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify) {
            $scope.robot = angular.copy(service.robot);
            $scope.config = angular.copy(configService);
            $scope.targetData = angular.copy(targetData);
            $scope.target = targetData;
            var id = $stateParams.id;
            $scope.target = {};
            $scope.info = service.getParameterPrint().filtered.advanceData;
            $scope.goIndex = function () {
                $state.go('nvNhapHangBanTraLai');
            }
            function filterData() {
                service.postPrintDetail().then(function (response) {
                    $scope.printData = response;
                });
            };
            filterData();
            $scope.printExcel = function () {
                var data = [document.getElementById('dataTable').innerHTML];
                configService.saveExcel(data, "Danh_sach_chi_tiet");
            }
            $scope.sum = function () {
                var total = 0;
                if ($scope.printData) {
                    angular.forEach($scope.printData, function (v, k) {
                        total = total + v.thanhTienSauVat;
                    });
                }
                return total;
            }
            $scope.print = function () {
                var table = document.getElementById('dataTable').innerHTML;
                var myWindow = $window.open('', '', 'width=800, height=600');
                myWindow.document.write(table);
                myWindow.print();
            }
            $scope.cancel = function () {
                $uibModalInstance.close();
            };
        }]);
    /* controller export Item bo hang */
    app.controller('phieuNhapHangBanTraLaiExportItemController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'phieuNhapHangBanTraLaiService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify) {
            $scope.robot = angular.copy(service.robot);
            $scope.config = angular.copy(configService);
            $scope.targetData = angular.copy(targetData);
            $scope.target = targetData;
            $scope.isLoading = false;
            $scope.lstMerchandise = [];
            $scope.dataHangHoa = {};
            $scope.maChungTuPk = targetData.maChungTuPk;
            $scope.title = function () { return 'Danh sách hàng nhập'; };
            $scope.hrefTem = configService.apiServiceBaseUri + "/Upload/Barcode/";
            var index = $scope.maChungTuPk.indexOf('.');
            $scope.maDonVi = $scope.maChungTuPk.substr(0, index);
            service.getUnitName($scope.maDonVi).then(function (response) {
                $scope.tenDonVi = response.data;
            });
            service.getInfoItemDetails($scope.target.id).then(function (response) {
                if (response.status) {
                    $scope.dataHangHoa = response.data;
                    $scope.lstMerchandise = response.data.dataDetails;
                }
                $scope.isLoading = false;
            });
            $scope.exportToExcel = function () {
                service.writeDataToExcel($scope.dataHangHoa).then(function (response) {
                    if (response.status) {
                        ngNotify.set("Thành công", { type: 'success' });
                        $window.location.href = $scope.hrefTem + "" + response.message;
                        $uibModalInstance.close($scope.target);
                    }
                    else {
                        ngNotify.set("Không Thành công", { duration: 3000, type: 'error' });
                    }
                });
            }
            $scope.goIndex = function () {
                $state.go('nvNhapHangBanTraLai');
            };
            $scope.cancel = function () {
                $uibModalInstance.close();
            };
        }]);
    return app;
});