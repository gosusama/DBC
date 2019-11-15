/*  
* Người tạo : Nguyễn Tuấn Hoàng Anh
* View: BTS.SP.MART/views/nv/NvDatHang
* Vm sevices: BTS.API.SERVICE -> NV ->NvDatHangVm.cs
* Sevices: BTS.API.SERVICE -> NV -> NvDatHangService.cs
* Entity: BTS.API.ENTITY -> NV - > NvDatHang.cs
* Menu: Nghiệp vụ-> Phiếu đặt hàng
*/
define(['ui-bootstrap', '/BTS.SP.MART/controllers/auth/AuthController.js', '/BTS.SP.MART/controllers/htdm/periodController.js', '/BTS.SP.MART/controllers/htdm/merchandiseController.js', '/BTS.SP.MART/controllers/htdm/customerController.js', '/BTS.SP.MART/controllers/htdm/merchandiseTypeController.js', '/BTS.SP.MART/controllers/htdm/nhomVatTuController.js', '/BTS.SP.MART/controllers/htdm/supplierController.js', '/BTS.SP.MART/controllers/htdm/wareHouseController.js', '/BTS.SP.MART/controllers/htdm/packagingController.js', '/BTS.SP.MART/controllers/htdm/taxController.js', '/BTS.SP.MART/controllers/htdm/donViTinhController.js'], function () {
    'use strict';
    var app = angular.module('phieuDatHangModule', ['ui.bootstrap', 'authModule', 'periodModule', 'merchandiseModule', 'customerModule', 'merchandiseTypeModule', 'nhomVatTuModule', 'supplierModule', 'wareHouseModule', 'packagingModule', 'taxModule', 'donViTinhModule']);
    app.factory('phieuDatHangService', ['$http', 'configService', function ($http, configService) {
        var serviceUrl = configService.rootUrlWebApi + '/NV/DatHang';
        var rootUrl = configService.apiServiceBaseUri;
        this.parameterPrint = {};
        function getParameterPrint() {
            return this.parameterPrint;
        }
        this.parameterPrint = {};
        var selectedData = [];
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
            changeSoLuongBao: function (item) {
                if (!item.soLuongLe) {
                    item.soLuongLe = 0;
                }
                if (!item.maBaoBi) {
                    item.luongBao = 1;
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
            changeSoLuongLe: function (item) {
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
            },
            changeSoLuong: function (item) {
                if (!item.soLuong) {
                    item.soLuong = 0;
                }
                if (!item.donGia) {
                    item.donGia = 0;
                }
                item.thanhTien = item.soLuong * item.donGia;
            },

            changeSoLuongLeDuyet: function (item) {
                if (!item.soLuongDuyet) {
                    item.soLuongDuyet = 0;
                }
                if (!item.donGiaDuyet) {
                    item.donGiaDuyet = 0;
                }
                item.thanhTien = item.donGiaDuyet * (item.soLuongDuyet + item.soLuongLeDuyet);
            },
            changeSoLuongBaoDuyet: function (item) {
                if (!item.soLuongLeDuyet) {
                    item.soLuongLeDuyet = 0;
                }
                if (!item.donGiaDuyet) {
                    item.donGiaDuyet = 0;
                }
                item.soLuongDuyet = item.luongBao * item.soLuongBaoDuyet + item.soLuongLeDuyet;
                item.thanhTien = item.donGiaDuyet * item.soLuongDuyet;
            },
            changeDonGiaDuyet: function (item) {
                if (!item.soLuongDuyet) {
                    item.soLuongDuyet = 0;
                }
                if (!item.soLuongLeDuyet) {
                    item.soLuongLeDuyet = 0;
                }
                item.thanhTien = item.donGiaDuyet * (item.soLuongDuyet + item.soLuongLeDuyet);
            },
            sumVat: function (tyGia, target) {
                var tienVat = 0;
                if (tyGia) {
                    tienVat = (target.thanhTienTruocVat - target.tienChietKhau) * (tyGia / 100);
                }
                return tienVat;
            },
            changeChietKhau: function (target) {
                if (!target.tienChietKhau) {
                    target.tienChietKhau = 0;
                }
                if (!target.thanhTienTruocVat) {
                    target.thanhTienTruocVat = 0;
                }
                if (!target.tienVat) {
                    target.tienVat = 0;
                }
                target.tienChietKhau = (target.thanhTienTruocVat * target.chietKhau) / 100;
                target.thanhTienSauVat = target.thanhTienTruocVat + target.tienVat - target.tienChietKhau;


            },
            changeTienChietKhau: function (target) {
                if (!target.thanhTienTruocVat) {
                    target.thanhTienTruocVat = 0;
                }
                if (!target.tienVat) {
                    target.tienVat = 0;
                }
                target.chietKhau = (target.tienChietKhau * 100) / target.thanhTienTruocVat;
                target.thanhTienSauVat = target.thanhTienTruocVat + target.tienVat - target.tienChietKhau;
            }
        }
        var filter = {
            equaTo: function equaTo(soLuongTon, data) {
                if (data && data.length > 0) {
                    data.sort(function (a, b) {
                        if (b.soLuongTon == soLuongTon && a.soLuongTon == soLuongTon) {
                            return 0;
                        } else if (b.soLuongTon == soLuongTon) {
                            return 1;
                        } else {
                            return -1;
                        };

                    });
                }
            },
            lessThan: function lessThan(soLuongTon, data) {
                if (data && data.length > 0) {
                    data.sort(function (a, b) {
                        if (b.soLuongTon < soLuongTon && a.soLuongTon < soLuongTon) {
                            return 0;
                        } else if (b.soLuongTon < soLuongTon) {
                            return 1;
                        } else {
                            return -1;
                        };

                    });
                }
            },
            lessThanOrEquaTo: function lessThanOrEquaTo(soLuongTon, data) {
                if (data && data.length > 0) {
                    data.sort(function (a, b) {
                        if (b.soLuongTon < soLuongTon && a.soLuongTon < soLuongTon) {
                            return 0;
                        } else if (b.soLuongTon < soLuongTon) {
                            return 1;
                        } else {
                            return -1;
                        };

                    });
                }
            },
            greaterThan: function greaterThan(soLuongTon, data) {
                if (data && data.length > 0) {
                    data.sort(function (a, b) {
                        if (b.soLuongTon > soLuongTon && a.soLuongTon > soLuongTon) {
                            return 0;
                        } else if (b.soLuongTon > soLuongTon) {
                            return 1;
                        } else {
                            return -1;
                        };

                    });
                }
            },
            greaterThanOrEquaTo: function greaterThanOrEquaTo(soLuongTon, data) {
                if (data && data.length > 0) {
                    data.sort(function (a, b) {
                        if (b.soLuongTon > soLuongTon && a.soLuongTon > soLuongTon) {
                            return 0;
                        } else if (b.soLuongTon > soLuongTon) {
                            return 1;
                        } else {
                            return -1;
                        };

                    });
                }
            }
        }
        var result = {
            robot: calc,
            filter: filter,
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
            postQueryApproval: function (data) {

                return $http.post(serviceUrl + '/PostQueryApproval', data);
            },
            postSelectData: function (data) {
                return $http.post(serviceUrl + '/PostSelectData', data);
            },
            postMerger: function (data) {
                return $http.post(serviceUrl + '/PostMerger', data);
            },
            postPrint: function () {
                return $http.post(serviceUrl + '/PostPrint', getParameterPrint());
            },
            postPrintDetail: function () {
                return $http.post(serviceUrl + '/PostPrintDetail', getParameterPrint());
            },
            postFilterMerchandise: function (data) {
                return $http.post(configService.rootUrlWebApi + '/Md/Merchandise/PostFilterMerchandise', data);
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
            getDetailsContract: function (id) {
                return $http.get(rootUrl + '/api/Md/Contract/GetDetails/' + id);
            },
            getContractByCustomerId: function (id) {
                return $http.get(rootUrl + '/api/Md/Contract/GetContractByCustomerId/' + id);
            },
            getCustomerById: function (id) {
                $http.get(rootUrl + '/api/Md/Customer/GetForNvByCode/' + id);
            },
            postApproval: function (data) {
                return $http.post(serviceUrl + '/PostApproval', data);
            },
            postComplete: function (data) {
                return $http.post(serviceUrl + '/PostComplete', data);
            },
            postCompletes: function (data) {
                return $http.post(serviceUrl + '/PostCompletes', data);
            },
            updateCT: function (params) {
                return $http.put(serviceUrl + '/' + params.id, params);
            },
            getMerchandiseForNvByCode: function (code) {
                return $http.get(rootUrl + '/api/Md/Merchandise/GetForNvByCode/' + code);
            },
            getUnitUsers: function () {
                $http.get(rootUrl + '/api/Md/UnitUser/GetSelectAll');
            },
            getSelectData: function () {
                return selectedData;
            },
            setSelectData: function (array) {
                selectedData = array;
            },
            deleteItem: function (params) {
                return $http.delete(serviceUrl + '/' + params.id, params);
            },
            countOrderNew: function () {
                return $http.get(serviceUrl + '/CountOrderNew');
            }
        };
        return result;
    }]);
    /* controller list */
    app.controller('phieuDatHangController', [
        '$scope', '$location', '$http', 'configService', 'phieuDatHangService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'securityService', '$rootScope', 'toaster', 'periodService', 'merchandiseService', 'customerService', 'merchandiseTypeService', 'nhomVatTuService', 'supplierService', 'wareHouseService', 'packagingService', 'taxService', 'donViTinhService',
        function ($scope, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, securityService, $rootScope, toaster, servicePeriod, serviceMerchandise, serviceCustomer, serviceMerchandiseType, serviceNhomVatTu, serviceSupplier, serviceWareHouse, servicePackaging, serviceTax, serviceDonViTinh) {
            $scope.openClosingOut = false;
            //check có mở khóa sổ không
            function checkUnClosingOut() {
                servicePeriod.checkUnClosingOut().then(function (response) {
                    if (response && response.status === 200 && response.data && response.data.data && response.data.data.length > 0) {
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
            $scope.robot = angular.copy(service.robot);
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.filtered = angular.copy(configService.filterDefault);
            $scope.tempData = tempDataService.tempData;
            $scope.isEditable = true;
            $scope.accessList = {};
            $scope.target = { dataDetails: [], dataClauseDetails: [] };
            //load dữ liệu
            function filterData() {
                $scope.isLoading = true;
                if ($scope.accessList.view) {
                    var postdata = {};
                    $scope.isLoading = true;
                    //$scope.filtered.advanceData.tagsCustomers = serviceCustomer.getSelectData();
                    //$scope.filtered.advanceData.tagMerchandiseTypes = serviceMerchandiseType.getSelectData();
                    //$scope.filtered.advanceData.tagMerchandises = serviceMerchandise.getSelectData();
                    //$scope.filtered.advanceData.tagMerchandiseGroups = serviceNhomVatTu.getSelectData();
                    //$scope.filtered.advanceData.tagNhaCungCaps = serviceSupplier.getSelectData();
                    //service.getNewInstance().then(function (response) {
                    //    if (response && response.status == 200 && response.data) {
                    //        $scope.filtered.advanceData = response.data;
                    //        $scope.options = $scope.filtered.advanceData.option;
                    //    }
                    //});
                    postdata = { paged: $scope.paged, filtered: $scope.filtered };
                    service.postQuery(postdata).then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data && successRes.data.status) {
                            $scope.isLoading = false;
                            $scope.data = successRes.data.data.data;
                            checkUnClosingOut();
                            angular.extend($scope.paged, successRes.data.data);
                        }
                    });
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
                securityService.getAccessList('phieuDatHang').then(function (successRes) {
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

            $scope.nextToApproval = function () {
                $state.go('approvalList');
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
            loadSupplier();
            loadMerchandiseType();
            loadNhomVatTu();
            loadWareHouse();
            loadPackagings();
            loadTax();
            loadDonViTinh();
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
            $scope.sortType = 'maNvDatHang';
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
            $scope.title = function () { return 'Phiếu đặt hàng'; };

            /* Function add New Item */
            $scope.create = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    size: 'md',
                    templateUrl: configService.buildUrl('nv/NvDatHang', 'add-plus'),
                    controller: 'phieuDatHangPlusCreateController',
                    windowClass: 'app-modal-window',
                    resolve: {}
                });
                modalInstance.result.then(function (updatedData) {
                    filterData();
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };

            /* Function Edit Item */
            $scope.update = function (target) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('nv/NvDatHang', 'update-plus'),
                    controller: 'phieuDatHangPlusEditController',
                    windowClass: 'app-modal-window',
                    resolve: {
                        targetData: function () {
                            return target;
                        }
                    }
                });
                modalInstance.result.then(function (updatedData) {
                    //$scope.target.dataDetails = [];
                    filterData();
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };

            /* Function Details Item */
            $scope.details = function (target) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('nv/NvDatHang', 'details'),
                    controller: 'phieuDatHangDetailsController',
                    windowClass: 'app-modal-window',
                    resolve: {
                        targetData: function () {
                            return target;
                        }
                    }
                });
                modalInstance.result.then(function (updatedData) {
                    //$scope.target.dataDetails = [];
                    filterData();
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };

            /* Function Print Item */
            $scope.printItem = function (target) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('nv/NvDatHang', 'printItem'),
                    controller: 'phieuDatHangExportItemController',
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
                    templateUrl: configService.buildUrl('nv/NvDatHang', 'delete'),
                    controller: 'phieuDatHangDeleteController',
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
            $scope.selectType = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/MerchandiseType', 'selectData'),
                    controller: 'merchandiseTypeSelectDataController',
                    resolve: {
                        serviceSelectData: function () {
                            return serviceMerchandiseType;
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
            $scope.selectGroup = function () {
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
                                advanceData: { maLoaiVatTu: $scope.filtered.advanceData.maLoaiVatTu }
                            };
                        }
                    }
                });
                modalInstance.result.then(function () {
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };
            $scope.sum = function () {
                var total = 0;
                if ($scope.data) {
                    angular.forEach($scope.data, function(v, k) {
                        total = total + v.thanhTien;
                    });
                }
                return total;
            }
            $scope.removeType = function (index) {
                $scope.tagsTypes.splice(index, 1);
            }
            $scope.removeGroup = function (index) {
                $scope.tagsGroups.splice(index, 1);
            }
            $scope.removeCustomer = function (index) {
                $scope.tagsCustomers.splice(index, 1);
            }

            $scope.selectedTax = function (target) {
                for (var i = 0; i < $scope.tempData('taxs').length; i++) {
                    var tmp = $scope.tempData('taxs')[i];
                    if (target.vat == tmp.value) {
                        $scope.tyGia = tmp.extendValue;
                    }
                }
            };
            $scope.goToParam = function () {
                $state.go('nvPhieuDatHangParameter');
            }
            $scope.print = function () {
                var postdata = { paged: $scope.paged, filtered: $scope.filtered };
                console.log('Print', postdata);
                service.setParameterPrint(postdata);
                $state.go("nvPrintDatHang");
            };
            $scope.printDetail = function () {
                var postdata = { paged: $scope.paged, filtered: $scope.filtered };
                service.setParameterPrint(postdata);
                $state.go("nvPrintDetailDatHang");
            };

            $scope.approval = function (target) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    size: 'md',
                    templateUrl: configService.buildUrl('nv/NvDatHang', 'update-need-approval'),
                    controller: 'phieuDatHangApprovalEditController',
                    windowClass: 'app-modal-window',
                    resolve: {
                        targetData: function () {
                            return target;
                        }
                    }
                });
                modalInstance.result.then(function (updatedData) {
                    $scope.refresh();
                    filterData();
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };
        }]);

    /* print Phieu Nhap Hang Mua Controller */ //log
    app.controller('phieuDatHangParameterController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'phieuDatHangService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify','$rootScope',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, $rootScope) {
            $scope.robot = angular.copy(service.robot);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.config = angular.copy(configService);
            $scope.tempData = tempDataService.tempData;
            $scope.tagDatHangs = [];
            $scope.target = {};
            $scope.title = function () {
                return 'Phiếu đặt hàng';
            };
            $scope.tagDatHangs = service.getSelectData();
            $scope.selectDatHang = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('nv/NvDatHang', 'selectData'),
                    controller: 'datHangSelectDataController',
                    resolve: {
                        serviceSelectData: function () {
                            return service;
                        },
                        filterObject: function () {
                            return {
                                advanceData: {
                                    trangThai: 20 //Được duyệt, chưa hoàn thành
                                },
                                isAdvance: true
                            }
                        }
                    }
                });
                modalInstance.result.then(function (updatedData) {
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            }
            $scope.removeDatHang = function (index) {
                $scope.tagDatHangs.splice(index, 1);
            }
            $scope.$watch('tagDatHangs', function (newValue, oldValue) {
                var values = $scope.tagDatHangs.map(function(element) {
                    return element.value;
                });
                $scope.target.tagDatHangs = values;
            }, true);
            $scope.displayHepler = function (paraValue, moduleName) {
                var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                if (data && data.length === 1) {
                    return data[0].text;
                } else {
                    return paraValue;
                }
            }
            $scope.merger = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('nv/NvDatHang', 'merger'),
                    controller: 'phieuDatHangMergerController',
                    windowClass: 'app-modal-window',
                    resolve: {
                        filterObject: function () {
                            return $scope.target.tagDatHangs;
                        }
                    }
                });

                modalInstance.result.then(function (updatedData) {
                    service.getSelectData().clear();
                }, function () {
                    service.getSelectData().clear();
                    $log.info('Modal dismissed at: ' + new Date());
                });
            }
         
            $scope.cancel = function () {
                $uibModalInstance.close();
            };
        }]);


    /* controller addNew */
    app.controller('phieuDatHangPlusCreateController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'phieuDatHangService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', '$rootScope', 'userService', 'merchandiseService', 'customerService', 'merchandiseTypeService', 'nhomVatTuService', 'supplierService', 'wareHouseService', 'packagingService', 'taxService', 'donViTinhService', 'toaster', 'periodService',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, $rootScope, serviceAuthUser, serviceMerchandise, serviceCustomer, serviceMerchandiseType, serviceNhomVatTu, serviceSupplier, serviceWareHouse, servicePackaging, serviceTax, serviceDonViTinh, toaster, servicePeriod) {
            var currentUser = serviceAuthUser.GetCurrentUser();
            var unitCode = currentUser.unitCode;
            var rootUrl = configService.apiServiceBaseUri;
            var serviceUrl = rootUrl + '/api/Nv/NhapHangMua';
            $scope.robot = angular.copy(service.robot);
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.data = [];
            $scope.contracts = [];
            $scope.target = { dataDetails: [] };
            $scope.tagsCustomers = [];
            $scope.tagsGroups = [];
            $scope.tagsTypes = [];
            $scope.filtered = { advanceData: {} };
            $scope.newItem = {};
            $scope.isHopDongInpDisabled = true;
            $scope.tyGia = 0;
            $scope.isListItemNull = true;
            $scope.tempData = tempDataService.tempData;
            $scope.isLoading = false;
            $scope.title = function () { return 'Phiếu đặt hàng'; };
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

            loadSupplier();
            loadMerchandiseType();
            loadNhomVatTu();
            loadWareHouse();
            loadPackagings();
            loadTax();
            loadDonViTinh();
            //end 
            $scope.filterByQuantity = function (operator, value) {
                switch (operator) {
                    case ">":
                        service.filter.greaterThan(value, $scope.target.dataDetails);
                        break;
                    case ">=":
                        service.filter.greaterThanOrEquaTo(value, $scope.target.dataDetails);
                        break;
                    case "<":
                        service.filter.lessThan(value, $scope.target.dataDetails);
                        break;
                    case "<=":
                        service.filter.lessThanOrEquaTo(value, $scope.target.dataDetails);
                        break;
                    case "=":
                        service.filter.equaTo(value, $scope.target.dataDetails);
                        break;
                }
                $scope.pageChanged();
            }
            $scope.operators = [
                { value: "=", text: "Bằng" },
                { value: "<=", text: "Nhỏ hơn hoặc bằng" },
                { value: "<", text: "Nhỏ hơn" },
                { value: ">", text: "Lớn hơn" },
                { value: ">=", text: "Lớn hơn hoặc bằng" }
            ];

            $scope.pageChanged = function () {
                var currentPage = $scope.paged.currentPage;
                var itemsPerPage = $scope.paged.itemsPerPage;
                $scope.paged.totalItems = $scope.target.dataDetails.length;
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
            };
            filterData();
            $scope.addRow = function () {
                if ($scope.newItem.maHang != null) {
                    $scope.target.dataDetails.push($scope.newItem);
                    $scope.pageChanged();
                    $scope.newItem = {};
                }
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
                        $scope.newItem.giaBanLe = updatedData.giaBanLe;
                        $scope.newItem.giaBanBuon = updatedData.giaBanBuon;
                        $scope.newItem.donGia = $scope.target.isBuon ? $scope.newItem.giaBanBuon : $scope.newItem.giaBanLe;
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
                $scope.pageChanged();
            }
            
            $scope.selectedkhachHang = function (item) {
                $scope.target.maKhachHang = item.value;
                service.getOrderByCustomer(item.value).then(function (response) {
                    if (response && response.status && response.data.length > 0) {
                        $scope.donHangs = response.data;
                    }
                });
                service.getCustomer(item.id).then(function (response) {
                    if (response && response.status == 200 && response.data) {
                        $scope.target.maSoThue = response.data.maSoThue;
                    }
                });
            };
            $scope.selectedTkCo = function (item) {
                $scope.target.tkCo = item.value;
            };
            $scope.selectedMaHang = function (code) {
                if (code) {
                    service.getMerchandiseForNvByCode(code).then(function (response) {
                        if (response && response.status == 200 && response.data && response.data.status) {
                            $scope.newItem = response.data.data;
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
            $scope.selectedHD = function (item) {
                $scope.isLoading = true;
                service.getDetailsContract(item.id, function (response) {
                    if (response && response.status == 200 && response.data) {
                        console.log('response.data:', response.data);
                        
                        angular.forEach(response.data.dataDetails, function(v, k) {
                            $scope.target.dataDetails.push(v);
                        });
                    }
                    $scope.isLoading = false;
                    $scope.pageChanged();

                });
            };
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
            $scope.saveAndKeep = function () {
                var tempData = angular.copy($scope.target);
                service.post($scope.target).then(function (successRes) {
                    if (successRes && successRes.status === 201 && successRes.data.status) {
                        ngNotify.set(successRes.data.message, { type: 'success' });
                        
                        $scope.isListItemNull = true;
                        service.getNewInstance().then(function (response) {
                            if (response && response.status == 200 && response.data) {
                                console.log('response.data:', response.data);
                                var expectData = response.data;
                                tempData.soPhieu = expectData.soPhieu;
                                tempData.ngay = expectData.ngay;
                                $scope.target = tempData;
                            }
                        });
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
            $scope.saveAndPrint = function () {
                service.post($scope.target).then(function (successRes) {
                    if (successRes && successRes.status === 201 && successRes.data) {
                        ngNotify.set(successRes.data.message, { type: 'success' });
                        var url = $state.href('reportPhieuDatHang', { id: response.data.id });
                        window.open(url, 'Report Viewer');
                        
                        $scope.isListItemNull = true;
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

            $scope.cancel = function () {
                
                
                
                
                $uibModalInstance.dismiss('cancel');
            };
        }]);
    /* controller Edit */
    app.controller('phieuDatHangPlusEditController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'phieuDatHangService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify', 'merchandiseService', 'toaster', 'customerService', 'merchandiseTypeService', 'nhomVatTuService', 'supplierService', 'wareHouseService', 'packagingService', 'taxService', 'donViTinhService', 'periodService',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify, serviceMerchandise, toaster, serviceCustomer, serviceMerchandiseType, serviceNhomVatTu, serviceSupplier, serviceWareHouse, servicePackaging, serviceTax, serviceDonViTinh, servicePeriod) {
            $scope.config = angular.copy(configService);
            $scope.robot = angular.copy(service.robot);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.targetData = angular.copy(targetData);
            $scope.tempData = tempDataService.tempData;
            $scope.target = targetData;
            $scope.newItem = {};
            $scope.tagsCustomers = [];
            $scope.tagsGroups = [];
            $scope.tagsTypes = [];
            $scope.filtered = { advanceData: {} };
            $scope.tyGia = 0;

            $scope.isLoading = true;
            $scope.title = function () { return 'Cập nhật phiếu đặt hàng'; };
            $scope.pageChanged = function () {
                var currentPage = $scope.paged.currentPage;
                var itemsPerPage = $scope.paged.itemsPerPage;
                $scope.paged.totalItems = $scope.target.dataDetails.length;
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
                        $scope.tyGia = z[0].extendValue;
                        $scope.$watch('target.vat', function (newValue, oldValue) {
                            if (!newValue) {
                                $scope.target.tienVat = 0;
                            } else {
                                $scope.target.tienVat = $scope.robot.sumVat($scope.tyGia, $scope.target);
                            }
                            $scope.target.thanhTienSauVat = $scope.target.thanhTienTruocVat + $scope.target.tienVat - $scope.target.tienChietKhau;
                        });
                        $scope.$watch('target.tienChietKhau', function (newValue, oldValue) {
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
                if ($scope.newItem.maHang != null) {
                    $scope.target.dataDetails.push($scope.newItem);
                    $scope.pageChanged();
                    $scope.newItem = {};
                }
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
                        updatedData.donGia = updatedData.giaMua;
                        $scope.newItem = updatedData;
                        $scope.newItem.validateCode = updatedData.maHang;
                        $scope.newItem.giaMuaCoVat = updatedData.giaMua * (1 + updatedData.tyLeVatVao / 100);
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
                $scope.target.maKhachHang = item.value;
                service.getOrderByCustomer(item.value).then(function (response) {
                    if (response && response && response.data.length > 0) {
                        $scope.donHangs = response.data;
                    }
                });
                service.getCustomer(item.id).then(function (response) {
                    if (response && response && response.data) {
                        $scope.target.maSoThue = response.data.maSoThue;
                    }
                });
            };
            $scope.selectedDonDatHang = function (item) {
                $scope.isLoading = true;
                service.getOrderById(item.id).then(function (response) {
                    var donHang = response.data;
                    
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
            $scope.selectedMaHang = function(code) {
                if (code) {
                    service.getMerchandiseForNvByCode(code).then(function(response) {
                        if (response && response.status == 200 && response.data && response.data.status) {
                            $scope.newItem = response.data.data;
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
            $scope.filterByQuantity = function (operator, value) {
                switch (operator) {
                    case ">":
                        service.filter.greaterThan(value, $scope.target.dataDetails);
                        break;
                    case ">=":
                        service.filter.greaterThanOrEquaTo(value, $scope.target.dataDetails);
                        break;
                    case "<":
                        service.filter.lessThan(value, $scope.target.dataDetails);
                        break;
                    case "<=":
                        service.filter.lessThanOrEquaTo(value, $scope.target.dataDetails);
                        break;
                    case "=":
                        service.filter.equaTo(value, $scope.target.dataDetails);
                        break;
                }
                $scope.pageChanged();
            }
            $scope.operators = [
                { value: "=", text: "Bằng" },
                { value: "<=", text: "Nhỏ hơn hoặc bằng" },
                { value: "<", text: "Nhỏ hơn" },
                { value: ">", text: "Lớn hơn" },
                { value: ">=", text: "Lớn hơn hoặc bằng" }
            ];

            $scope.selectedHD = function (item) {
                $scope.isLoading = true;
                service.getDetailsContract(item.id, function (response) {
                    if (response && response.status == 200 && response.data) {
                        console.log('response.data:', response.data);
                        
                        angular.forEach(response.data.dataDetails, function (v, k) {
                            $scope.target.dataDetails.push(v);
                        });
                    }
                    $scope.isLoading = false;
                    $scope.pageChanged();

                });
            };

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
                        var url = $state.href('reportPhieuDatHang', { id: successRes.data.data.id });
                        window.open(url, 'Report Viewer');
                        
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
            $scope.ClearContent = function () {
                var inpKhachHang = $scope.target.maKhachHang;
                if (inpKhachHang == null) {
                    
                    $scope.target.maHd = "";
                    $scope.pageChanged();
                }
            }
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
            $scope.selectType = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/MerchandiseType', 'selectData'),
                    controller: 'merchandiseTypeSelectDataController',
                    resolve: {
                        serviceSelectData: function () {
                            return serviceMerchandiseType;
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
            $scope.selectGroup = function () {
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
                                advanceData: { maLoaiVatTu: $scope.filtered.advanceData.maLoaiVatTu }
                            };
                        }
                    }
                });
                modalInstance.result.then(function () {
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };
            $scope.$watch('tagsGroups', function (newValue, oldValue) {
                var values = $scope.tagsGroups.map(function(element) {
                    return element.value;
                });
                $scope.filtered.advanceData.maNhomVatTu = values.join();
            }, true);
            $scope.$watch('tagsTypes', function (newValue, oldValue) {
                var values = $scope.tagsTypes.map(function(element) {
                    return element.value;
                });
                $scope.filtered.advanceData.maLoaiVatTu = values.join();
            }, true);
            $scope.removeType = function (index) {
                $scope.tagsTypes.splice(index, 1);
            }
            $scope.removeGroup = function (index) {
                $scope.tagsGroups.splice(index, 1);
            }
            $scope.removeCustomer = function (index) {
                $scope.tagsCustomers.splice(index, 1);
            }
            $scope.approval = function (target) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    size: 'md',
                    templateUrl: configService.buildUrl('nv/NvDatHang', 'update-need-approval'),
                    controller: 'phieuDatHangApprovalEditController',
                    windowClass: 'app-modal-window',
                    resolve: {
                        targetData: function () {
                            return target;
                        }
                    }
                });
                modalInstance.result.then(function (updatedData) {
                    $scope.refresh();
                    filterData();
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };
            $scope.filterMerchandise = function () {
                $scope.filtered.isAdvance = true;
                service.postFilterMerchandise(JSON.stringify({ filtered: $scope.filtered }), function(response) {
                    if (response.status) {
                        
                        angular.forEach(response.data, function(v, k) {
                            v.soLuong = 0;
                            v.thanhTien = 0;
                            $scope.target.dataDetails.push(v);
                        });
                        $scope.pageChanged();
                    }
                });
            }
            $scope.cancel = function () {
                
                
                
                
                $uibModalInstance.close();
            };
        }]);


    /* controller Details */
    app.controller('phieuDatHangDetailsController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'phieuDatHangService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify) {
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.tempData = tempDataService.tempData;
            $scope.target = targetData;
            $scope.isLoading = false;
            $scope.title = function () { return 'Thông tin phiếu đặt hàng'; };
            $scope.sum = function () {
                var total = 0;
                if ($scope.target.dataDetails) {
                    angular.forEach($scope.target.dataDetails, function(v, k) {
                        total = total + v.thanhTien;
                    });
                }
                return total;
            };
            $scope.pageChanged = function () {
                var currentPage = $scope.paged.currentPage;
                var itemsPerPage = $scope.paged.itemsPerPage;
                $scope.paged.totalItems = $scope.target.dataDetails.length;
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
                    if (resgetDetails && resgetDetails.status === 200 && resgetDetails.data) {
                        $scope.target = resgetDetails.data.data;
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
                            $state.go('nvNhapHangMua');
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
    app.controller('phieuDatHangDeleteController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'phieuDatHangService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify',
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
    app.controller('reportPhieuDatHangController', ['$scope', '$location', '$http', 'configService', 'phieuDatHangService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'userService', '$stateParams', '$window',
        function ($scope, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, serviceAuthUser, $stateParams, $window) {
            var currentUser = serviceAuthUser.GetCurrentUser();
            $scope.robot = angular.copy(service.robot);
            var id = $stateParams.id;
            $scope.target = {};
            $scope.goIndex = function () {
                $state.go('nvNhapHangMua');
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
                var fileName = "NhapHangMua_ExportData.xls";
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
    app.controller('phieuDatHangMergerController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'phieuDatHangService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify', 'filterObject', 'merchandiseService',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify, filterObject, serviceMerchandise) {
            $scope.robot = angular.copy(service.robot);
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.isHopDongInpDisabled = true;
            $scope.contracts = [];
            $scope.target = { dataDetails: [] };
            $scope.newItem = {};
            $scope.isLoading = true;
            $scope.isLoading = true;
            $scope.pageChanged = function () {
                var currentPage = $scope.paged.currentPage;
                var itemsPerPage = $scope.paged.itemsPerPage;
                $scope.paged.totalItems = $scope.target.dataDetails.length;
                $scope.data = [];
                if ($scope.target.dataDetails) {
                    for (var i = (currentPage - 1) * itemsPerPage; i < currentPage * itemsPerPage && i < $scope.target.dataDetails.length; i++) {
                        $scope.data.push($scope.target.dataDetails[i]);
                    }
                }
            }
            function getDetail(callback) {
                service.postMerger(filterObject, function(response) {
                    if (response && response.length > 0) {
                        angular.forEach(response, function(v, k) {
                            $scope.target.dataDetails.push(v);
                        });
                    }
                    callback();
                });
            }
            service.getNewInstance().then(function (response) {
                if (response && response.status == 200 && response.data) {
                    console.log('response.data:', response.data);
                    $scope.target = response.data;
                    $scope.target.dataDetails = service.getSelectData();
                    getDetail($scope.pageChanged);
                    $scope.isLoading = false;
                }
            });
            $scope.addRow = function () {
                $scope.target.dataDetails.push($scope.newItem);
                $scope.pageChanged();
                $scope.newItem = {};
            };
            $scope.title = function () {
                return 'Phiếu đặt hàng';
            };
            $scope.selectedMaHang = function (code) {
                if (code) {
                    service.getMerchandiseForNvByCode(code).then(function (response) {
                        if (response && response.status == 200 && response.data && response.data.status) {
                            $scope.newItem = response.data.data;
                        } else {
                            $scope.addNewItem(code);

                        }
                    });
                }
            };
            $scope.selectedMaKhachHang = function (item) {
                service.getContractByCustomerId(item.id).then(function (response) {
                    if (response && response.status == 200 && response.data) {
                        $scope.contracts = response.data;
                    }
                });
            };
            $scope.selectedHD = function (item) {
                $scope.isLoading = true;
                service.getDetailsContract(item.id, function (response) {
                    if (response && response.status == 200 && response.data) {
                        
                        angular.forEach(response.data.dataDetails, function (v, k) {
                            $scope.target.dataDetails.push(v);
                        });
                    }
                    $scope.isLoading = false;
                    $scope.pageChanged();
                });
            };
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
                    }
                    $scope.pageChanged();
                }, function () {

                });
            }
            $scope.ClearContent = function () {
                var inpKhachHang = $scope.target.maKhachHang;
                if (inpKhachHang == null) {
                    $scope.target.maHd = "";
                    $scope.pageChanged();
                }
            }
            $scope.removeItem = function (index) {
                var currentPage = $scope.paged.currentPage;
                var itemsPerPage = $scope.paged.itemsPerPage;
                var currentPageIndex = (currentPage - 1) * itemsPerPage + index;
                $scope.target.dataDetails.splice(currentPageIndex, 1);
                $scope.pageChanged();
            }
           
            $scope.cancel = function () {
                $uibModalInstance.close();
            };
        }]);
    return app;
});