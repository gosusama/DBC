/*  
* Người tạo : Nguyễn Tuấn Hoàng Anh
* View: BTS.SP.MART/views/nv/NvNhapHangMua
* Vm sevices: BTS.API.SERVICE -> NV ->NvNhapHangMuaVm.cs
* Sevices: BTS.API.SERVICE -> NV -> NvNhapHangMuaService.cs
* Entity: BTS.API.ENTITY -> NV - > NvNhapHangMua.cs
* Menu: Nghiệp vụ-> Nhập hàng mua
*/
define(['ui-bootstrap', '/BTS.SP.MART/controllers/auth/AuthController.js', '/BTS.SP.MART/controllers/htdm/periodController.js', '/BTS.SP.MART/controllers/htdm/merchandiseController.js', '/BTS.SP.MART/controllers/htdm/customerController.js', '/BTS.SP.MART/controllers/htdm/merchandiseTypeController.js', '/BTS.SP.MART/controllers/htdm/nhomVatTuController.js', '/BTS.SP.MART/controllers/htdm/supplierController.js', '/BTS.SP.MART/controllers/htdm/wareHouseController.js', '/BTS.SP.MART/controllers/htdm/packagingController.js', '/BTS.SP.MART/controllers/htdm/taxController.js', '/BTS.SP.MART/controllers/htdm/donViTinhController.js', '/BTS.SP.MART/controllers/auth/AuNguoiDung.js', '/BTS.SP.MART/controllers/auth/AuDonVi.js'], function () {
    'use strict';
    var app = angular.module('phieuDatHangNCCModule', ['ui.bootstrap', 'authModule', 'periodModule', 'merchandiseModule', 'customerModule', 'merchandiseTypeModule', 'nhomVatTuModule', 'supplierModule', 'wareHouseModule', 'packagingModule', 'taxModule', 'donViTinhModule', 'AuNguoiDungModule', 'AuDonViModule']);
    app.factory('phieuDatHangNCCService', [
        '$resource', '$http', '$window', 'configService',
        function ($resource, $http, $window, configService) {
            var rootUrl = configService.apiServiceBaseUri;
            var serviceUrl = rootUrl + '/api/Nv/DatHangNCC';
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

                },
            }
            var selectedData = [];

            var result = {
                robot: calc,
                post: function (data, callback) {
                    $http.post(serviceUrl + '/Post', data).success(callback);
                },
                postQuery: function (data, callback) {

                    $http.post(serviceUrl + '/PostQuery', data).success(callback);
                },
                postSelectData: function (data, callback) {
                    $http.post(serviceUrl + '/PostSelectData', data).success(callback);
                },
                postPrint: function (callback) {
                    $http.post(serviceUrl + '/PostPrint', getParameterPrint()).success(callback);
                },
                postPrintDetail: function (callback) {
                    $http.post(serviceUrl + '/PostPrintDetail', getParameterPrint()).success(callback);
                },
                postFilterMerchandise: function (data, callback) {
                    return $http.post(configService.rootUrlWebApi + '/Md/Merchandise/PostFilterMerchandise', data).success(callback);
                },
                getNewInstance: function (callback) {
                    $http.get(serviceUrl + '/GetNewInstance').success(callback);
                },
                getReport: function (id, callback) {
                    $http.get(serviceUrl + '/GetReport/' + id).success(callback);
                },
                getDetails: function (id, callback) {
                    $http.get(serviceUrl + '/GetDetails/' + id).success(callback);
                },
                getChild: function (id, callback) {
                    $http.get(serviceUrl + '/GetChild/' + id).success(callback);
                },
                getDetailsContract: function (id, callback) {
                    $http.get(rootUrl + '/api/Md/Contract/GetDetails/' + id).success(callback);
                },
                postApproval: function (data) {
                    return $http.post(serviceUrl + '/PostApproval', data);
                },
                postMerge: function (data, callback) {
                    $http.post(serviceUrl + '/PostMerge', data).success(callback);
                },
                updateCT: function (params) {
                    return $http.put(serviceUrl + '/' + params.id, params);
                },
                getMerchandiseForNvByCode: function (code, wareHouseCode, unitCode) {
                    return $http.get(rootUrl + '/api/Md/Merchandise/GetForNvByCode/' + code + '/' + wareHouseCode + '/' + unitCode);
                },
                getUnitUsers: function (callback) {
                    $http.get(rootUrl + '/api/Md/UnitUser/GetSelectAll').success(callback);
                },
                deleteItem: function (params) {
                    return $http.delete(serviceUrl + '/' + params.id, params);
                },
                deleteSummary: function (params) {
                    return $http.delete(serviceUrl + '/DeleteSummary/' + params.id, params);
                },
                getSelectDataApprovalBySupplierCode: function (code) {
                    return $http.get(serviceUrl + '/GetSelectDataApprovalBySupplierCode/' + code);
                },
                getSelectData: function () {
                    return selectedData;
                },
                setSelectData: function (array) {
                    selectedData = array;
                },
                postQuerySummary: function (data, callback) {

                    $http.post(serviceUrl + '/PostQuerySummary', data).success(callback);
                },
                postAddNewSummary: function (data, callback) {
                    $http.post(serviceUrl + '/PostAddNewSummary', data).success(callback);
                },
                postReceiveSummary: function (data, callback) {
                    $http.post(serviceUrl + '/PostReceiveSummary', data).success(callback);
                },
            };
            return result;
        }
    ]);

    app.controller('phieuDatHangNCCController', ['$mdDialog', '$scope', '$location', '$http', '$state', 'configService', 'phieuDatHangNCCService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'securityService', '$rootScope', 'toaster', 'periodService', 'merchandiseService', 'customerService', 'merchandiseTypeService', 'nhomVatTuService', 'supplierService', 'wareHouseService', 'packagingService', 'taxService', 'donViTinhService', 'AuNguoiDungService', 'AuDonViService',
    function ($mdDialog, $scope, $location, $http, $state, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, securityService, $rootScope, toaster, servicePeriod, serviceMerchandise, serviceCustomer, serviceMerchandiseType, serviceNhomVatTu, serviceSupplier, serviceWareHouse, servicePackaging, serviceTax, serviceDonViTinh, serviceAuNguoiDung, serviceAuDonVi) {
        $scope.config = angular.copy(configService);
        $scope.paged = angular.copy(configService.pageDefault);
        $scope.robot = angular.copy(service.robot);
        $scope.filtered = angular.copy(configService.filterDefault);
        $scope.tempData = tempDataService.tempData;
        $scope.sortType = 'ngay'; // set the default sort type
        $scope.sortReverse = false; // set the default sort order
        $scope.accessList = {};

        function loadAccessList() {
            securityService.getAccessList('nvDatHangNCC').then(function (successRes) {
                if (successRes && successRes.status == 200) {
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
                toaster.pop('error', "Lỗi:", "Không có quyền truy cập !");
                $scope.accessList = null;
            });
        }
        loadAccessList();

        function loadSupplier() {
            if (!tempDataService.tempData('suppliers')) {
                serviceSupplier.getAll_Supplier().then(function (successRes) {
                    if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                        tempDataService.putTempData('suppliers', successRes.data.data);
                        $scope.lstSupplier = successRes.data.data;
                    } else {
                        console.log('successRes', successRes);
                    }
                }, function (errorRes) {
                    console.log('errorRes', errorRes);
                });
            } else {
                $scope.lstSupplier = tempDataService.tempData('suppliers');
            }
        }

        function loadAuDonVi() {
            if (!tempDataService.tempData('auDonVis')) {
                serviceAuDonVi.getAll_DonVi().then(function (successRes) {
                    if (successRes && successRes.status === 200 && successRes.data.length > 0) {

                        tempDataService.putTempData('auDonVis', successRes.data);
                        $scope.lstDonVi = successRes.data;
                    } else {
                        console.log('successRes', successRes);
                    }
                }, function (errorRes) {
                    console.log('errorRes', errorRes);
                });
            } else {
                $scope.lstDonVi = tempDataService.tempData('auDonVis');
            }
        }

        function loadNguoiDung() {
            if (!tempDataService.tempData('auUsers')) {
                serviceAuNguoiDung.getAll_NguoiDung().then(function (successRes) {
                    if (successRes && successRes.status === 200 && successRes.data.length > 0) {
                        tempDataService.putTempData('auUsers', successRes.data);
                        $scope.lstNguoiDung = successRes.data;
                    } else {
                        console.log('successRes', successRes);
                    }
                }, function (errorRes) {
                    console.log('errorRes', errorRes);
                });
            } else {
                $scope.lstNguoiDung = tempDataService.tempData('auUsers');
            }
        }

        function loadWareHouse() {
            if (!tempDataService.tempData('warehouses')) {
                serviceWareHouse.getAll_WareHouse().then(function (successRes) {
                    if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                        tempDataService.putTempData('warehouses', successRes.data.data);
                        $scope.warehouses = successRes.data.data;
                    } else {
                        console.log('successRes', successRes);
                    }
                }, function (errorRes) {
                    console.log('errorRes', errorRes);
                });
            } else {
                $scope.warehouses = tempDataService.tempData('warehouses');
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
        loadAuDonVi();
        loadNguoiDung();
        loadWareHouse();
        loadPackagings();
        loadTax();
        loadDonViTinh();

        $scope.setPage = function (pageNo) {
            $scope.paged.currentPage = pageNo;
            filterData();
        };
        $scope.doSearch = function () {
            $scope.paged.currentPage = 1;
            filterData();
        };
        $scope.pageChanged = function () {
            filterData();
        };
        $scope.goHome = function () {
            $state.go('home');
        };
        $scope.refresh = function () {
            $scope.setPage($scope.paged.currentPage);
        };
        $scope.title = function () {
            return 'Phiếu đặt hàng Nhà cung cấp';
        };
        $scope.displayHepler = function (code, module) {
            var data = $filter('filter')($scope.tempData(module), { value: code }, true);
            if (data && data.length == 1) {
                return data[0].text;
            };
            return "Empty!";
        }

        $scope.formatLabel = function (model, module, displayModel) {
            if (!model) return "";
            var data = $filter('filter')(tempDataService.tempData[module], { value: model }, true);
            if (data && data.length == 1) {
                displayModel = data[0].text;
                return data[0].text;
            }
            return "Empty!";
        };
        $scope.create = function () {
            var modalInstance = $uibModal.open({
                backdrop: 'static',
                templateUrl: configService.buildUrl('nv/NvDatHangNCC', 'add-plus'),
                controller: 'phieuDatHangNCCCreateController',
                windowClass: 'app-modal-window',
                resolve: {}
            });

            modalInstance.result.then(function (updatedData) {
                $scope.refresh();
            }, function () {
                $log.info('Modal dismissed at: ' + new Date());
            });
        };
        $scope.summarize = function () {
            $state.go('nvSummaryDatHangNCC');
        };

        $scope.details = function (target) {
            var modalInstance = $uibModal.open({
                backdrop: 'static',
                templateUrl: configService.buildUrl('nv/NvDatHangNCC', 'details'),
                controller: 'phieuDatHangNCCDetailsController',
                windowClass: 'app-modal-window',
                resolve: {
                    targetData: function () {
                        return target;
                    }
                }
            });
        };
        $scope.deleteItem = function (event, target) {
            var modalInstance = $uibModal.open({
                backdrop: 'static',
                templateUrl: configService.buildUrl('nv/NvDatHangNCC', 'delete'),
                controller: 'phieuDatHangNCCDeleteController',
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

        $scope.update = function (target) {
            var modalInstance = $uibModal.open({
                backdrop: 'static',
                templateUrl: configService.buildUrl('nv/nvDatHangNCC', 'update-plus'),
                controller: 'phieuDatHangNCCEditController',
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

        function filterData() {
            $scope.isLoading = true;
            var postdata = { paged: $scope.paged, filtered: $scope.filtered };
            service.postQuery(
                JSON.stringify(postdata),
                function (response) {
                    $scope.isLoading = false;
                    if (response.status) {
                        $scope.data = response.data.data;
                        angular.extend($scope.paged, response.data);
                    }
                });
        };
    }
    ]);
    app.controller('phieuDatHangNCCCreateController', [
        '$scope', '$uibModal', '$uibModalInstance', 'configService', 'phieuDatHangNCCService', 'tempDataService', '$filter', '$log', 'ngNotify', 'securityService', '$rootScope', 'toaster', 'merchandiseService', 'merchandiseTypeService', 'nhomVatTuService', 'supplierService', 'wareHouseService', 'userService',
        function ($scope, $uibModal, $uibModalInstance, configService, service, tempDataService, $filter, $log, ngNotify, securityService, $rootScope, toaster, serviceMerchandise, serviceMerchandiseType, serviceNhomVatTu, serviceSupplier, serviceWareHouse, serviceAuthUser) {
            var currentUser = serviceAuthUser.GetCurrentUser();
            var unitCode = currentUser.unitCode;
            $scope.title = function () {
                return 'Phiếu đặt hàng Nhà cung cấp';
            };
            $scope.cancel = function () {
                $scope.target.dataDetails = [];
                $scope.tagsCustomers = [];
                $scope.tagsGroups = [];
                $scope.tagsTypes = [];
                $uibModalInstance.dismiss('cancel');
            };
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.robot = angular.copy(service.robot);
            $scope.filtered = angular.copy(configService.filterDefault);
            $scope.tempData = tempDataService.tempData;
            $scope.target = {
                dataDetails: [],
                listAdd: []
            };
            $scope.tagsCustomers = [];
            $scope.tagsGroups = [];
            $scope.tagsTypes = [];
            $scope.tagsWarehouses = [];
            $scope.filtered = { advanceData: {} };
            $scope.newItem = {};
            $scope.tyGia = 0;
            $scope.stateIsRunning = false;
            $scope.addRow = function () {
                $scope.addRow = function () {
                    if (!$scope.newItem.soLuong || $scope.newItem.soLuong < 1) {
                        focus('soluong');
                        document.getElementById('soluong').focus();
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
                                    $scope.target.dataDetails[k].soLuong = $scope.newItem.soLuong + $scope.target.dataDetails[k].soLuong;
                                    $scope.target.dataDetails[k].soLuongBao = $scope.newItem.soLuongBao + $scope.target.dataDetails[k].soLuongBao;
                                    $scope.target.dataDetails[k].soLuongLe = $scope.newItem.soLuongLe + $scope.target.dataDetails[k].soLuongLe;
                                    $scope.target.dataDetails[k].thanhTien = $scope.newItem.soLuong * $scope.target.dataDetails[k].donGia;
                                    service.robot.changeSoLuongLe($scope.target.dataDetails[k]);
                                }
                            });
                        } else {
                            $scope.target.dataDetails.push($scope.newItem);
                        }
                    } else {
                        toaster.pop('error', "Lỗi:", "Mã hàng chưa đúng!");
                    }
                    $scope.pageChanged();
                    $scope.newItem = {};
                    focus('mahang');
                    document.getElementById('mahang').focus();
                };
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
                        $scope.newItem.donGia = updatedData.giaMua;
                        $scope.newItem.validateCode = updatedData.maHang;
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
            }
            $scope.selectedMaHang = function (code) {
                if (code) {
                    service.getMerchandiseForNvByCode(code, null, unitCode).then(function (response) {
                        if (response.data.data && response.data.status) {
                            $scope.newItem = response.data.data;
                            $scope.newItem.donGia = response.data.data.giaMua;
                            $scope.newItem.validateCode = response.data.data.maHang;
                        } else {
                            $scope.addNewItem(code);
                        }
                    }, function (error) {
                        $scope.addNewItem(code);
                    });
                }
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
            $scope.createPackage = function (target, name) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('mdPackaging', 'add'),
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
            $scope.pageChanged = function () {
                var currentPage = $scope.paged.currentPage;
                var itemsPerPage = 10;
                $scope.paged.totalItems = $scope.target.dataDetails.length;
                $scope.data = [];
                if ($scope.target.dataDetails) {
                    for (var i = (currentPage - 1) * itemsPerPage; i < currentPage * itemsPerPage && i < $scope.target.dataDetails.length; i++) {
                        $scope.data.push($scope.target.dataDetails[i]);
                    }
                }
            }
            $scope.filterByQuantity = function (operator, value) {
                $scope.isLoading = true;
                $scope.filtered.isAdvance = true;
                $scope.filtered.advanceData.maNhaCungCap = $scope.target.maNhaCungCap;
                $scope.filtered.advanceData.ngayChungTu = $scope.target.ngay;
                service.postFilterMerchandise(JSON.stringify({ filtered: $scope.filtered, filterQuantity: $scope.filterQuantity }), function (response) {
                    if (response && response.status && response.data && response.data.length > 0) {
                        $scope.target.dataDetails = response.data;
                        $scope.pageChanged();
                    }
                    else {
                        ngNotify.set("Xảy ra lỗi", { duration: 2000, type: 'error' });
                    }
                }, function (errorRes) {
                });
            };
            $scope.operators = [
                { value: "=", text: "Bằng" },
        				{ value: "<=", text: "Nhỏ hơn hoặc bằng" },
        				{ value: "<", text: "Nhỏ hơn" },
        				{ value: ">", text: "Lớn hơn" },
        				{ value: ">=", text: "Lớn hơn hoặc bằng" }
            ];
            $scope.formatLabel = function (model, module) {
                if (!model) return "";
                var data = $filter('filter')($scope.tempData(module), { value: model }, true);
                if (data && data.length == 1) {
                    return data[0].text;
                }
                return "Empty!";
            };

            $scope.selectCustomer = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Supplier', 'selectData'),
                    controller: 'supplierSelectDataController',
                    resolve: {
                        serviceSelectData: function () { //log2
                            return serviceSupplier;
                        },
                        filterObject: function () {
                            return null;
                        }
                    }
                });
                modalInstance.result.then(function (updatedData) {
                }, function () {
                });
            }
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
                modalInstance.result.then(function (updatedData) {
                    $scope.tagsTypes = updatedData;
                    var output = '';
                    angular.forEach($scope.tagsTypes, function (item, index) {
                        output += item.value + ',';
                    });
                    $scope.filtered.advanceData.maLoaiVatTu = output.substring(0, output.length - 1);
                }, function () {
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
                modalInstance.result.then(function (updatedData) {
                    $scope.tagsGroups = updatedData;
                    var output = '';
                    angular.forEach($scope.tagsGroups, function (item, index) {
                        output += item.value + ',';
                    });
                    $scope.filtered.advanceData.maNhomVatTu = output.substring(0, output.length - 1);
                }, function () {
                });
            };
            $scope.selectWarehouse = function () {
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
                                isAdvance: true,
                                advanceData: { maKhoHang: $scope.filtered.advanceData.maKhoHang }
                            };
                        }
                    }
                });
                modalInstance.result.then(function (updatedData) {
                    var output = '';
                    $scope.tagsWarehouses = updatedData;
                    angular.forEach($scope.tagsWarehouses, function (item, index) {
                        output += item.value + ',';
                    });
                    $scope.filtered.advanceData.maKhoHang = output.substring(0, output.length - 1);
                }, function () {
                });
            };
            $scope.removeType = function (index) {
                $scope.tagsTypes.splice(index, 1);
                if ($scope.tagsTypes && $scope.tagsTypes.length > 0) {
                    var output = '';
                    angular.forEach($scope.tagsTypes, function (item, index) {
                        output += item.value + ',';
                    });
                    $scope.filtered.advanceData.maLoaiVatTu = output.substring(0, output.length - 1);
                } else {
                    $scope.filtered.advanceData.maLoaiVatTu = '';
                }
            }
            $scope.removeGroup = function (index) {
                $scope.tagsGroups.splice(index, 1);
                if ($scope.tagsGroups && $scope.tagsGroups.length > 0) {
                    var output = '';
                    angular.forEach($scope.tagsGroups, function (item, index) {
                        output += item.value + ',';
                    });
                    $scope.filtered.advanceData.maNhomVatTu = output.substring(0, output.length - 1);
                } else {
                    $scope.filtered.advanceData.maNhomVatTu = '';
                }
            }
            $scope.removeCustomer = function (index) {
                $scope.tagsCustomers.splice(index, 1);
                if ($scope.tagsCustomers && $scope.tagsCustomers.length > 0) {
                    var output = '';
                    angular.forEach($scope.tagsCustomers, function (item, index) {
                        output += item.value + ',';
                    });
                    $scope.filtered.advanceData.maNhaCungCap = output.substring(0, output.length - 1);
                } else {
                    $scope.filtered.advanceData.maNhaCungCap = '';
                }
            }
            $scope.removeWarehouse = function (index) {
                $scope.tagsWarehouses.splice(index, 1);
                if ($scope.tagsWarehouses && $scope.tagsWarehouses.length > 0) {
                    var output = '';
                    angular.forEach($scope.tagsWarehouses, function (item, index) {
                        output += item.value + ',';
                    });
                    $scope.filtered.advanceData.maKhoHang = output.substring(0, output.length - 1);
                } else {
                    $scope.filtered.advanceData.maKhoHang = '';
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
            $rootScope.$on('$locationChangeStart',
                function (event, next, current) {
                    $scope.tagsCustomers = [];
                    $scope.tagsGroups = [];
                    $scope.tagsTypes = [];
                });
            $scope.filterMerchandise = function () {
                $scope.isLoading = true;
                $scope.filtered.isAdvance = true;
                $scope.filtered.advanceData.maNhaCungCap = $scope.target.maNhaCungCap;
                $scope.filtered.advanceData.ngayChungTu = $scope.target.ngay;
                service.postFilterMerchandise(JSON.stringify({ filtered: $scope.filtered, filterQuantity: $scope.filterQuantity }), function (response) {
                    if (response && response.status && response.data && response.data.length > 0) {
                        $scope.target.dataDetails = response.data;
                        $scope.pageChanged();
                    }
                    else {
                        ngNotify.set("Xảy ra lỗi", { duration: 2000, type: 'error' });
                    }
                }, function (errorRes) {
                });
            };
            var arrayCondition = ["=", "<", "<=", ">", ">="];
            function hasNumbers(t) {
                return /\d/.test(t);
            }
            String.prototype.removeWord = function (searchWord) {
                var str = this;
                var n = str.search(searchWord);
                while (str.search(searchWord) > -1) {
                    n = str.search(searchWord);
                    str = str.substring(0, n) + str.substring(n + searchTerm.length, str.length);
                }
                return str;
            }
            $scope.filterFuncSoLuong = function () {
                $scope.data = $filter('filter')($scope.target.dataDetails, function (value, index) {
                    return value.soLuongLe != null;
                }, false);
            };
            $scope.filterFunc = function () {
                if ($scope.maHangFilter) {
                    $scope.data = $filter("filter")($scope.target.dataDetails, {
                        $: $scope.maHangFilter
                    });
                }
                else if ($scope.tenHangFilter) {
                    $scope.data = $filter("filter")($scope.target.dataDetails, {
                        $: $scope.tenHangFilter
                    });
                }
                else if ($scope.barcodeFilter) {
                    $scope.data = $filter("filter")($scope.target.dataDetails, {
                        $: $scope.barcodeFilter
                    });
                }
                else if ($scope.soLuongLeFilter) {
                    if (hasNumbers($scope.soLuongLeFilter)) {
                        var r = /\d+/;
                        var number = $scope.soLuongLeFilter.match(r)[0];
                        var opers = $scope.soLuongLeFilter.replace(number, "").trim();
                        if (opers == ">") {
                            $scope.data = $filter('filter')($scope.target.dataDetails, function (value, index) {
                                return value.soLuongLe > parseFloat(number);
                            }, false);
                        }
                        else if (opers == ">=") {
                            $scope.data = $filter('filter')($scope.target.dataDetails, function (value, index) {
                                return value.soLuongLe >= parseFloat(number);
                            }, false);
                        }
                        else if (opers == "<=") {
                            $scope.data = $filter('filter')($scope.target.dataDetails, function (value, index) {
                                return value.soLuongLe <= parseFloat(number);
                            }, false);
                        }
                        else if (opers == ">") {
                            $scope.data = $filter('filter')($scope.target.dataDetails, function (value, index) {
                                return value.soLuongLe > parseFloat(number);
                            }, false);
                        }
                        else if (opers == "<") {
                            $scope.data = $filter('filter')($scope.target.dataDetails, function (value, index) {
                                return value.soLuongLe < parseFloat(number);
                            }, false);
                        }
                        else {
                            $scope.data = $filter('filter')($scope.target.dataDetails, function (value, index) {
                                return value.soLuongLe == parseFloat(number);
                            }, false);
                        }
                    }
                }
                else if ($scope.soLuongTonFilter) {
                    if (hasNumbers($scope.soLuongTonFilter)) {
                        var r = /\d+/;
                        var number = $scope.soLuongTonFilter.match(r)[0];
                        var opers = $scope.soLuongTonFilter.replace(number, "").trim();
                        if (opers == ">") {
                            $scope.data = $filter('filter')($scope.target.dataDetails, function (value, index) {
                                return value.soLuongTon > parseFloat(number);
                            }, false);
                        }
                        else if (opers == ">=") {
                            $scope.data = $filter('filter')($scope.target.dataDetails, function (value, index) {
                                return value.soLuongTon >= parseFloat(number);
                            }, false);
                        }
                        else if (opers == "<=") {
                            $scope.data = $filter('filter')($scope.target.dataDetails, function (value, index) {
                                return value.soLuongTon <= parseFloat(number);
                            }, false);
                        }
                        else if (opers == ">") {
                            $scope.data = $filter('filter')($scope.target.dataDetails, function (value, index) {
                                return value.soLuongTon > parseFloat(number);
                            }, false);
                        }
                        else if (opers == "<") {
                            $scope.data = $filter('filter')($scope.target.dataDetails, function (value, index) {
                                return value.soLuongTon < parseFloat(number);
                            }, false);
                        }
                        else {
                            $scope.data = $filter('filter')($scope.target.dataDetails, function (value, index) {
                                return value.soLuongTon == parseFloat(number);
                            }, false);
                        }
                    }
                }
                else if ($scope.soLuongXuatTrongKyFilter) {
                    if (hasNumbers($scope.soLuongXuatTrongKyFilter)) {
                        var r = /\d+/;
                        var number = $scope.soLuongXuatTrongKyFilter.match(r)[0];
                        var opers = $scope.soLuongXuatTrongKyFilter.replace(number, "").trim();
                        if (opers == ">") {
                            $scope.data = $filter('filter')($scope.target.dataDetails, function (value, index) {
                                return value.soLuongXuatTrongKy > parseFloat(number);
                            }, false);
                        }
                        else if (opers == ">=") {
                            $scope.data = $filter('filter')($scope.target.dataDetails, function (value, index) {
                                return value.soLuongXuatTrongKy >= parseFloat(number);
                            }, false);
                        }
                        else if (opers == "<=") {
                            $scope.data = $filter('filter')($scope.target.dataDetails, function (value, index) {
                                return value.soLuongXuatTrongKy <= parseFloat(number);
                            }, false);
                        }
                        else if (opers == ">") {
                            $scope.data = $filter('filter')($scope.target.dataDetails, function (value, index) {
                                return value.soLuongXuatTrongKy > parseFloat(number);
                            }, false);
                        }
                        else if (opers == "<") {
                            $scope.data = $filter('filter')($scope.target.dataDetails, function (value, index) {
                                return value.soLuongXuatTrongKy < parseFloat(number);
                            }, false);
                        }
                        else {
                            $scope.data = $filter('filter')($scope.target.dataDetails, function (value, index) {
                                return value.soLuongXuatTrongKy == parseFloat(number);
                            }, false);
                        }
                    }
                }
                else if ($scope.soLuongNhapTrongKyFilter) {
                    if (hasNumbers($scope.soLuongNhapTrongKyFilter)) {
                        var r = /\d+/;
                        var number = $scope.soLuongNhapTrongKyFilter.match(r)[0];
                        var opers = $scope.soLuongNhapTrongKyFilter.replace(number, "").trim();
                        if (opers == ">") {
                            $scope.data = $filter('filter')($scope.target.dataDetails, function (value, index) {
                                return value.soLuongNhapTrongKy > parseFloat(number);
                            }, false);
                        }
                        else if (opers == ">=") {
                            $scope.data = $filter('filter')($scope.target.dataDetails, function (value, index) {
                                return value.soLuongNhapTrongKy >= parseFloat(number);
                            }, false);
                        }
                        else if (opers == "<=") {
                            $scope.data = $filter('filter')($scope.target.dataDetails, function (value, index) {
                                return value.soLuongNhapTrongKy <= parseFloat(number);
                            }, false);
                        }
                        else if (opers == ">") {
                            $scope.data = $filter('filter')($scope.target.dataDetails, function (value, index) {
                                return value.soLuongNhapTrongKy > parseFloat(number);
                            }, false);
                        }
                        else if (opers == "<") {
                            $scope.data = $filter('filter')($scope.target.dataDetails, function (value, index) {
                                return value.soLuongNhapTrongKy < parseFloat(number);
                            }, false);
                        }
                        else {
                            $scope.data = $filter('filter')($scope.target.dataDetails, function (value, index) {
                                return value.soLuongNhapTrongKy == parseFloat(number);
                            }, false);
                        }
                    }
                }
                else {
                    $scope.pageChanged();
                }
            };
            $scope.displayHepler = function (code, module) {
                if (code) {
                    var data = $filter('filter')($scope.tempData(module), { value: code }, true);
                    if (data && data.length == 1) {
                        return data[0].text;
                    };
                }
                return "";
            };
            function filterData() {
                $scope.isLoading = true;
                service.getNewInstance(function (response) {
                    $scope.target = response;
                    $scope.target.ngay = new Date($scope.target.ngay);
                    $scope.target.dataDetails = [];
                    $scope.tagsGroups = [];
                    $scope.tagsTypes = [];
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
                })
                $scope.filtered.advanceData.denNgay = new Date;
                $scope.filtered.advanceData.tuNgay = new Date;
            };

            $scope.save = function () {
                $scope.target.listAdd = $filter('filter')($scope.target.dataDetails, function (value, index) {
                    return value.soLuongLe != null;
                }, false);
                service.post(JSON.stringify($scope.target), function (response) {
                    if (response.status) {
                        ngNotify.set("Thêm mới thành công", { type: 'success' });
                        $scope.tagsCustomers = [];
                        $scope.tagsGroups = [];
                        $scope.tagsTypes = [];
                        $uibModalInstance.close($scope.target);
                    } else {
                        ngNotify.set("response.message", { duration: 3000, type: 'error' });
                    }
                });
            };
            filterData();

            $scope.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };
        }
    ]);
    app.controller('phieuDatHangNCCEditController', ['$scope', '$uibModal', '$uibModalInstance', 'configService', 'targetData', 'phieuDatHangNCCService', 'tempDataService', '$filter', '$log', 'ngNotify', 'securityService', '$rootScope', 'toaster', 'merchandiseService', '$state', 'userService',
        function ($scope, $uibModal, $uibModalInstance, configService, targetData, service, tempDataService, $filter, $log, ngNotify, securityService, $rootScope, toaster, serviceMerchandise, $state, serviceAuthUser) {
            $scope.config = angular.copy(configService);
            var currentUser = serviceAuthUser.GetCurrentUser();
            var unitCode = currentUser.unitCode;
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.robot = angular.copy(service.robot);
            $scope.filtered = angular.copy(configService.filterDefault);
            $scope.tempData = tempDataService.tempData;
            $scope.target = targetData;
            $scope.newItem = {};
            $scope.tagsCustomers = [];
            $scope.tagsGroups = [];
            $scope.tagsTypes = [];
            $scope.tagsWarehouses = [];
            $scope.filtered = { advanceData: {} };
            $scope.tyGia = 0;
            $scope.addRow = function () {
                $scope.target.dataDetails.push($scope.newItem);
                $scope.pageChanged();
                $scope.newItem = {};
            };
            $scope.target.listEdit = [];
            $scope.createPackage = function (target, name) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('mdPackaging', 'add'),
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
            $scope.filterByQuantity = function (operator, value) {
                $scope.isLoading = true;
                $scope.filtered.isAdvance = true;
                $scope.filtered.advanceData.maNhaCungCap = $scope.target.maNhaCungCap;
                $scope.filtered.advanceData.ngayChungTu = $scope.target.ngay;
                service.postFilterMerchandise(JSON.stringify({ filtered: $scope.filtered, filterQuantity: $scope.filterQuantity }), function (response) {
                    if (response && response.status && response.data && response.data.length > 0) {
                        if ($scope.target.dataDetails.length > 0) {
                            angular.forEach($scope.target.dataDetails,
                                function (value, index) {
                                    response.data.splice(value, 1);
                                });
                            angular.forEach(response.data,
                                function (value, index) {
                                    $scope.target.dataDetails.push(value);
                                });
                            $scope.pageChanged();
                        }
                    }
                    else {
                        ngNotify.set("Xảy ra lỗi", { duration: 2000, type: 'error' });
                    }
                }, function (errorRes) {
                });
            };
            $scope.operators = [
                { value: "=", text: "Bằng" },
                { value: "<=", text: "Nhỏ hơn hoặc bằng" },
                { value: "<", text: "Nhỏ hơn" },
                { value: ">", text: "Lớn hơn" },
                { value: ">=", text: "Lớn hơn hoặc bằng" }
            ];
            $scope.formatLabel = function (model, module) {
                if (!model) return "";
                var data = $filter('filter')($scope.tempData(module), { value: model }, true);
                if (data && data.length == 1) {
                    return data[0].text;
                }
                return "Empty!";
            };
            $scope.selectedMaHang = function (code) {
                if (code) {
                    service.getMerchandiseForNvByCode(code, null, unitCode).then(function (response) {
                        if (response.data.data && response.data.status) {
                            $scope.newItem = response.data.data;
                            $scope.newItem.donGia = response.data.data.giaMua;
                        } else {
                            $scope.addNewItem(code);
                        }
                    }, function (error) {
                        $scope.addNewItem(code);
                    });
                }
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
                        $scope.newItem.donGia = updatedData.giaMua;
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
            $scope.selectCustomer = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Supplier', 'selectData'),
                    controller: 'supplierSelectDataController',
                    resolve: {
                        serviceSelectData: function () { //log2
                            return serviceSupplier;
                        },
                        filterObject: function () {
                            return null;
                        }
                    }
                });
                modalInstance.result.then(function (updatedData) {
                }, function () {
                });
            }
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
                modalInstance.result.then(function (updatedData) {
                    $scope.tagsTypes = updatedData;
                    var output = '';
                    angular.forEach($scope.tagsTypes, function (item, index) {
                        output += item.value + ',';
                    });
                    $scope.filtered.advanceData.maLoaiVatTu = output.substring(0, output.length - 1);
                }, function () {
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
                modalInstance.result.then(function (updatedData) {
                    $scope.tagsGroups = updatedData;
                    var output = '';
                    angular.forEach($scope.tagsGroups, function (item, index) {
                        output += item.value + ',';
                    });
                    $scope.filtered.advanceData.maNhomVatTu = output.substring(0, output.length - 1);
                }, function () {
                });
            };
            $scope.selectWarehouse = function () {
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
                                isAdvance: true,
                                advanceData: { maKhoHang: $scope.filtered.advanceData.maKhoHang }
                            };
                        }
                    }
                });
                modalInstance.result.then(function (updatedData) {
                    var output = '';
                    $scope.tagsWarehouses = updatedData;
                    angular.forEach($scope.tagsWarehouses, function (item, index) {
                        output += item.value + ',';
                    });
                    $scope.filtered.advanceData.maKhoHang = output.substring(0, output.length - 1);
                }, function () {
                });
            };
            $scope.removeType = function (index) {
                $scope.tagsTypes.splice(index, 1);
                if ($scope.tagsTypes && $scope.tagsTypes.length > 0) {
                    var output = '';
                    angular.forEach($scope.tagsTypes, function (item, index) {
                        output += item.value + ',';
                    });
                    $scope.filtered.advanceData.maLoaiVatTu = output.substring(0, output.length - 1);
                } else {
                    $scope.filtered.advanceData.maLoaiVatTu = '';
                }
            }
            $scope.removeGroup = function (index) {
                $scope.tagsGroups.splice(index, 1);
                if ($scope.tagsGroups && $scope.tagsGroups.length > 0) {
                    var output = '';
                    angular.forEach($scope.tagsGroups, function (item, index) {
                        output += item.value + ',';
                    });
                    $scope.filtered.advanceData.maNhomVatTu = output.substring(0, output.length - 1);
                } else {
                    $scope.filtered.advanceData.maNhomVatTu = '';
                }
            }
            $scope.removeCustomer = function (index) {
                $scope.tagsCustomers.splice(index, 1);
                if ($scope.tagsCustomers && $scope.tagsCustomers.length > 0) {
                    var output = '';
                    angular.forEach($scope.tagsCustomers, function (item, index) {
                        output += item.value + ',';
                    });
                    $scope.filtered.advanceData.maNhaCungCap = output.substring(0, output.length - 1);
                } else {
                    $scope.filtered.advanceData.maNhaCungCap = '';
                }
            }
            $scope.removeWarehouse = function (index) {
                $scope.tagsWarehouses.splice(index, 1);
                if ($scope.tagsWarehouses && $scope.tagsWarehouses.length > 0) {
                    var output = '';
                    angular.forEach($scope.tagsWarehouses, function (item, index) {
                        output += item.value + ',';
                    });
                    $scope.filtered.advanceData.maKhoHang = output.substring(0, output.length - 1);
                } else {
                    $scope.filtered.advanceData.maKhoHang = '';
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
            $rootScope.$on('$locationChangeStart',
            function (event, next, current) {
                $scope.tagsCustomers = [];
                $scope.tagsGroups = [];
                $scope.tagsTypes = [];
            })
            $scope.filterMerchandise = function () {
                $scope.isLoading = true;
                $scope.filtered.isAdvance = true;
                $scope.filtered.advanceData.maNhaCungCap = $scope.target.maNhaCungCap;
                $scope.filtered.advanceData.ngayChungTu = $scope.target.ngay;
                service.postFilterMerchandise(JSON.stringify({ filtered: $scope.filtered, filterQuantity: $scope.filterQuantity }), function (response) {
                    if (response && response.status && response.data && response.data.length > 0) {
                        if ($scope.target.dataDetails.length > 0) {
                            angular.forEach($scope.target.dataDetails,
                                function(value,index) {
                                    response.data.splice(value,1);
                                });
                            angular.forEach(response.data,
                                function (value, index) {
                                    $scope.target.dataDetails.push(value);
                                });
                            $scope.pageChanged();
                        }
                    }
                    else {
                        ngNotify.set("Xảy ra lỗi", { duration: 2000, type: 'error' });
                    }
                }, function (errorRes) {
                });
            };
            var arrayCondition = ["=", "<", "<=", ">", ">="];
            function hasNumbers(t) {
                return /\d/.test(t);
            }
            String.prototype.removeWord = function (searchWord) {
                var str = this;
                var n = str.search(searchWord);
                while (str.search(searchWord) > -1) {
                    n = str.search(searchWord);
                    str = str.substring(0, n) + str.substring(n + searchTerm.length, str.length);
                }
                return str;
            }
            $scope.filterFuncSoLuong = function () {
                $scope.data = $filter('filter')($scope.target.dataDetails, function (value, index) {
                    return value.soLuongLe != null;
                }, false);
            };
            $scope.filterFunc = function () {
                if ($scope.maHangFilter) {
                    $scope.data = $filter("filter")($scope.target.dataDetails, {
                        $: $scope.maHangFilter
                    });
                }
                else if ($scope.tenHangFilter) {
                    $scope.data = $filter("filter")($scope.target.dataDetails, {
                        $: $scope.tenHangFilter
                    });
                }
                else if ($scope.barcodeFilter) {
                    $scope.data = $filter("filter")($scope.target.dataDetails, {
                        $: $scope.barcodeFilter
                    });
                }
                else if ($scope.soLuongLeFilter) {
                    if (hasNumbers($scope.soLuongLeFilter)) {
                        var r = /\d+/;
                        var number = $scope.soLuongLeFilter.match(r)[0];
                        var opers = $scope.soLuongLeFilter.replace(number, "").trim();
                        if (opers == ">") {
                            $scope.data = $filter('filter')($scope.target.dataDetails, function (value, index) {
                                return value.soLuongLe > parseFloat(number);
                            }, false);
                        }
                        else if (opers == ">=") {
                            $scope.data = $filter('filter')($scope.target.dataDetails, function (value, index) {
                                return value.soLuongLe >= parseFloat(number);
                            }, false);
                        }
                        else if (opers == "<=") {
                            $scope.data = $filter('filter')($scope.target.dataDetails, function (value, index) {
                                return value.soLuongLe <= parseFloat(number);
                            }, false);
                        }
                        else if (opers == ">") {
                            $scope.data = $filter('filter')($scope.target.dataDetails, function (value, index) {
                                return value.soLuongLe > parseFloat(number);
                            }, false);
                        }
                        else if (opers == "<") {
                            $scope.data = $filter('filter')($scope.target.dataDetails, function (value, index) {
                                return value.soLuongLe < parseFloat(number);
                            }, false);
                        }
                        else {
                            $scope.data = $filter('filter')($scope.target.dataDetails, function (value, index) {
                                return value.soLuongLe == parseFloat(number);
                            }, false);
                        }
                    }
                }
                else if ($scope.soLuongTonFilter) {
                    if (hasNumbers($scope.soLuongTonFilter)) {
                        var r = /\d+/;
                        var number = $scope.soLuongTonFilter.match(r)[0];
                        var opers = $scope.soLuongTonFilter.replace(number, "").trim();
                        if (opers == ">") {
                            $scope.data = $filter('filter')($scope.target.dataDetails, function (value, index) {
                                return value.soLuongTon > parseFloat(number);
                            }, false);
                        }
                        else if (opers == ">=") {
                            $scope.data = $filter('filter')($scope.target.dataDetails, function (value, index) {
                                return value.soLuongTon >= parseFloat(number);
                            }, false);
                        }
                        else if (opers == "<=") {
                            $scope.data = $filter('filter')($scope.target.dataDetails, function (value, index) {
                                return value.soLuongTon <= parseFloat(number);
                            }, false);
                        }
                        else if (opers == ">") {
                            $scope.data = $filter('filter')($scope.target.dataDetails, function (value, index) {
                                return value.soLuongTon > parseFloat(number);
                            }, false);
                        }
                        else if (opers == "<") {
                            $scope.data = $filter('filter')($scope.target.dataDetails, function (value, index) {
                                return value.soLuongTon < parseFloat(number);
                            }, false);
                        }
                        else {
                            $scope.data = $filter('filter')($scope.target.dataDetails, function (value, index) {
                                return value.soLuongTon == parseFloat(number);
                            }, false);
                        }
                    }
                }
                else if ($scope.soLuongXuatTrongKyFilter) {
                    if (hasNumbers($scope.soLuongXuatTrongKyFilter)) {
                        var r = /\d+/;
                        var number = $scope.soLuongXuatTrongKyFilter.match(r)[0];
                        var opers = $scope.soLuongXuatTrongKyFilter.replace(number, "").trim();
                        if (opers == ">") {
                            $scope.data = $filter('filter')($scope.target.dataDetails, function (value, index) {
                                return value.soLuongXuatTrongKy > parseFloat(number);
                            }, false);
                        }
                        else if (opers == ">=") {
                            $scope.data = $filter('filter')($scope.target.dataDetails, function (value, index) {
                                return value.soLuongXuatTrongKy >= parseFloat(number);
                            }, false);
                        }
                        else if (opers == "<=") {
                            $scope.data = $filter('filter')($scope.target.dataDetails, function (value, index) {
                                return value.soLuongXuatTrongKy <= parseFloat(number);
                            }, false);
                        }
                        else if (opers == ">") {
                            $scope.data = $filter('filter')($scope.target.dataDetails, function (value, index) {
                                return value.soLuongXuatTrongKy > parseFloat(number);
                            }, false);
                        }
                        else if (opers == "<") {
                            $scope.data = $filter('filter')($scope.target.dataDetails, function (value, index) {
                                return value.soLuongXuatTrongKy < parseFloat(number);
                            }, false);
                        }
                        else {
                            $scope.data = $filter('filter')($scope.target.dataDetails, function (value, index) {
                                return value.soLuongXuatTrongKy == parseFloat(number);
                            }, false);
                        }
                    }
                }
                else if ($scope.soLuongNhapTrongKyFilter) {
                    if (hasNumbers($scope.soLuongNhapTrongKyFilter)) {
                        var r = /\d+/;
                        var number = $scope.soLuongNhapTrongKyFilter.match(r)[0];
                        var opers = $scope.soLuongNhapTrongKyFilter.replace(number, "").trim();
                        if (opers == ">") {
                            $scope.data = $filter('filter')($scope.target.dataDetails, function (value, index) {
                                return value.soLuongNhapTrongKy > parseFloat(number);
                            }, false);
                        }
                        else if (opers == ">=") {
                            $scope.data = $filter('filter')($scope.target.dataDetails, function (value, index) {
                                return value.soLuongNhapTrongKy >= parseFloat(number);
                            }, false);
                        }
                        else if (opers == "<=") {
                            $scope.data = $filter('filter')($scope.target.dataDetails, function (value, index) {
                                return value.soLuongNhapTrongKy <= parseFloat(number);
                            }, false);
                        }
                        else if (opers == ">") {
                            $scope.data = $filter('filter')($scope.target.dataDetails, function (value, index) {
                                return value.soLuongNhapTrongKy > parseFloat(number);
                            }, false);
                        }
                        else if (opers == "<") {
                            $scope.data = $filter('filter')($scope.target.dataDetails, function (value, index) {
                                return value.soLuongNhapTrongKy < parseFloat(number);
                            }, false);
                        }
                        else {
                            $scope.data = $filter('filter')($scope.target.dataDetails, function (value, index) {
                                return value.soLuongNhapTrongKy == parseFloat(number);
                            }, false);
                        }
                    }
                }
                else {
                    $scope.pageChanged();
                }
            };
            $scope.save = function () {
                $scope.target.listEdit = $filter('filter')($scope.target.dataDetails, function (value, index) {
                    return value.soLuongLe != null;
                }, false);
                service.updateCT($scope.target).then(
                       function (response) {
                           if (response.status && response.status == 200) {
                               if (response.data.status) {
                                   ngNotify.set("Cập nhật thành công", { type: 'success' });
                                   $uibModalInstance.close($scope.target);
                               } else {
                                   ngNotify.set(response.message, { duration: 3000, type: 'error' });
                               }
                           } else {
                               console.log('ERROR: Update failed! ' + response.errorMessage);
                               ngNotify.set(response.errorMessage, { duration: 3000, type: 'error' });
                           }
                       },
                        function (response) {
                            console.log('ERROR: Update failed! ' + response);
                        }
                    );
            };
            function filterData() {
                $scope.isLoading = true;
                service.getDetails($scope.target.id, function (response) {
                    if (response.data.id) {
                        $scope.target = response.data;
                        $scope.target.ngay = new Date($scope.target.ngay);
                    }
                    $scope.pageChanged();
                    $scope.isLoading = false;
                    $scope.selectedTax($scope.target);
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
                });
                $scope.filtered.advanceData.denNgay = new Date;
                $scope.filtered.advanceData.tuNgay = new Date;
            };
            filterData();
            $scope.pageChanged = function () {
                var currentPage = $scope.paged.currentPage;
                var itemsPerPage = $scope.paged.itemsPerPage;
                $scope.paged.totalItems = $scope.target.dataDetails.length;
                $scope.data = [];
                if ($scope.target.dataDetails) {
                    for (var i = (currentPage - 1) * itemsPerPage; i < currentPage * itemsPerPage && i < $scope.target.dataDetails.length; i++) {
                        $scope.data.push($scope.target.dataDetails[i])
                    }
                }
            }
            $scope.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };
        }
    ]);
    app.controller('phieuDatHangNCCDeleteController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'phieuDatHangNCCService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify) {
            $scope.config = angular.copy(configService);
            $scope.isLoading = false;
            $scope.target = angular.copy(targetData);
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
    app.controller('phieuDatHangNCCDetailsController', ['$scope', '$uibModal', '$uibModalInstance', 'configService', 'targetData', 'phieuDatHangNCCService', 'tempDataService', '$filter', '$log', 'ngNotify', 'securityService', '$rootScope', 'toaster', 'merchandiseService', '$state',
        function ($scope, $uibModal, $uibModalInstance, configService, targetData, service, tempDataService, $filter, $log, ngNotify, securityService, $rootScope, toaster, serviceMerchandise, $state) {
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.robot = angular.copy(service.robot);
            $scope.filtered = angular.copy(configService.filterDefault);
            $scope.tempData = tempDataService.tempData;
            $scope.target = targetData;
            $scope.title = function () {
                return 'Phiếu đặt hàng Nhà cung cấp';
            };
            $scope.formatLabel = function (model, module) {
                if (!model) return "";
                var data = $filter('filter')($scope.tempData(module), { value: model }, true);
                if (data && data.length == 1) {
                    return data[0].text;
                }
                return "Empty!";
            };
            $scope.sum = function () {
                var total = 0;
                if ($scope.target.dataDetails) {
                    angular.forEach($scope.target.dataDetails, function (v, k) {
                        total = total + v.thanhTien;
                    })
                }
                return total;
            };
            $scope.approval = function () {
                service.postApproval($scope.target).then(function (response) {
                    if (response) {
                        ngNotify.set("Duyệt Thành công", { type: 'success' });
                        $uibModalInstance.close($scope.target);
                        $scope.goIndex = function () {
                            $state.go('nvDatHangNCC');
                        };
                    }
                    else {
                        ngNotify.set("Thất bại! - Xảy ra lỗi hoặc phiếu này đã duyệt", { duration: 3000, type: 'error' });
                    }
                });
            };
            function fillterData() {
                $scope.isLoading = true;
                service.getDetails($scope.target.id, function (response) {
                    if (response.status) {
                        $scope.target = response.data;
                        $scope.target.ngay = new Date($scope.target.ngay);
                    }
                    $scope.isLoading = false;
                    $scope.pageChanged();
                });
            }
            fillterData();
            $scope.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };
            $scope.pageChanged = function () {
                var currentPage = $scope.paged.currentPage;
                var itemsPerPage = $scope.paged.itemsPerPage;
                $scope.paged.totalItems = $scope.target.dataDetails.length;
                $scope.data = [];
                if ($scope.target.dataDetails) {
                    for (var i = (currentPage - 1) * itemsPerPage; i < currentPage * itemsPerPage && i < $scope.target.dataDetails.length; i++) {
                        $scope.data.push($scope.target.dataDetails[i])
                    }
                }
            }
        }
    ]);

    /* Summary Region*/
    app.controller('phieuDatHangNCCSummaryController', [
        '$scope', '$location', '$http', '$state', 'configService', 'phieuDatHangNCCService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'securityService', '$rootScope', 'toaster', 'periodService', 'merchandiseService', 'customerService', 'merchandiseTypeService', 'nhomVatTuService', 'supplierService', 'wareHouseService', 'packagingService', 'taxService', 'donViTinhService', 'AuNguoiDungService', 'AuDonViService',
    function ($scope, $location, $http, $state, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, securityService, $rootScope, toaster, servicePeriod, serviceMerchandise, serviceCustomer, serviceMerchandiseType, serviceNhomVatTu, serviceSupplier, serviceWareHouse, servicePackaging, serviceTax, serviceDonViTinh, serviceAuNguoiDung, serviceAuDonVi) {
        $scope.config = angular.copy(configService);
        $scope.paged = angular.copy(configService.pageDefault);
        $scope.robot = angular.copy(service.robot);
        $scope.filtered = angular.copy(configService.filterDefault);
        $scope.tempData = tempDataService.tempData;
        $scope.sortType = 'ngay'; // set the default sort type
        $scope.sortReverse = false; // set the default sort order
        $scope.accessList = {};
        function loadAccessList() {
            securityService.getAccessList('nvDatHangNCC').then(function (successRes) {
                if (successRes && successRes.status == 200) {
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

        function loadSupplier() {
            if (!tempDataService.tempData('suppliers')) {
                serviceSupplier.getAll_Supplier().then(function (successRes) {

                    if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                        tempDataService.putTempData('suppliers', successRes.data.data);
                        $scope.lstSupplier = successRes.data.data;
                    } else {
                        console.log('successRes', successRes);
                    }
                }, function (errorRes) {
                    console.log('errorRes', errorRes);
                });
            } else {
                $scope.lstSupplier = tempDataService.tempData('suppliers');
            }
        }

        function loadAuDonVi() {
            if (!tempDataService.tempData('auDonVis')) {
                serviceAuDonVi.getAll_DonVi().then(function (successRes) {
                    if (successRes && successRes.status === 200 && successRes.data.length > 0) {

                        tempDataService.putTempData('auDonVis', successRes.data);
                        $scope.lstDonVi = successRes.data;
                    } else {
                        console.log('successRes', successRes);
                    }
                }, function (errorRes) {
                    console.log('errorRes', errorRes);
                });
            } else {
                $scope.lstDonVi = tempDataService.tempData('auDonVis');
            }
        }

        function loadNguoiDung() {
            if (!tempDataService.tempData('auUsers')) {
                serviceAuNguoiDung.getAll_NguoiDung().then(function (successRes) {
                    if (successRes && successRes.status === 200 && successRes.data.length > 0) {
                        tempDataService.putTempData('auUsers', successRes.data);
                        $scope.lstNguoiDung = successRes.data;
                    } else {
                        console.log('successRes', successRes);
                    }
                }, function (errorRes) {
                    console.log('errorRes', errorRes);
                });
            } else {
                $scope.lstNguoiDung = tempDataService.tempData('auUsers');
            }
        }

        function loadWareHouse() {
            if (!tempDataService.tempData('warehouses')) {
                serviceWareHouse.getAll_WareHouse().then(function (successRes) {
                    if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                        tempDataService.putTempData('warehouses', successRes.data.data);
                        $scope.warehouses = successRes.data.data;
                    } else {
                        console.log('successRes', successRes);
                    }
                }, function (errorRes) {
                    console.log('errorRes', errorRes);
                });
            } else {
                $scope.warehouses = tempDataService.tempData('warehouses');
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
        loadAuDonVi();
        loadNguoiDung();
        loadWareHouse();
        loadPackagings();
        loadTax();
        loadDonViTinh();
        $scope.setPage = function (pageNo) {
            $scope.paged.currentPage = pageNo;
            filterData();
        };
        $scope.doSearch = function () {
            $scope.paged.currentPage = 1;
            filterData();
        };
        $scope.pageChanged = function () {
            filterData();
        };
        $scope.goHome = function () {
            $state.go('nvDatHangNCC');
        };
        $scope.refresh = function () {
            $scope.setPage($scope.paged.currentPage);
        };
        $scope.title = function () {
            return 'Phiếu đặt hàng Nhà cung cấp';
        };
        $scope.displayHepler = function (code, module) {
            var data = $filter('filter')($scope.tempData(module), { value: code }, true);
            if (data && data.length == 1) {
                return data[0].text;
            };
            return "Empty!";
        }

        $scope.formatLabel = function (model, module, displayModel) {
            if (!model) return "";
            var data = $filter('filter')($scope.tempData(module), { value: model }, true);
            if (data && data.length == 1) {
                displayModel = data[0].text;
                return data[0].text;
            }
            return "Empty!";
        };
        $scope.create = function () {

            var modalInstance = $uibModal.open({
                backdrop: 'static',
                templateUrl: configService.buildUrl('/nv/NvDatHangNCC', 'add-summary'),
                controller: 'phieuDatHangNCCSummaryCreateController',
                windowClass: 'app-modal-window',
                resolve: {}
            });

            modalInstance.result.then(function (updatedData) {
                $scope.refresh();
            }, function () {
                $log.info('Modal dismissed at: ' + new Date());
            });
        };

        $scope.details = function (target) {
            var modalInstance = $uibModal.open({
                backdrop: 'static',
                templateUrl: configService.buildUrl('/nv/NvDatHangNCC', 'details-summary'),
                controller: 'phieuDatHangNCCDetailsController',
                windowClass: 'app-modal-window',
                resolve: {
                    targetData: function () {
                        return target;
                    }
                }
            });
        };
        $scope.receive = function (target) {
            var modalInstance = $uibModal.open({
                backdrop: 'static',
                templateUrl: configService.buildUrl('/nv/NvDatHangNCC', 'receive-summary'),
                controller: 'phieuGiaoNhanController',
                windowClass: 'app-modal-window',
                resolve: {
                    targetData: function () {
                        return target;
                    }
                }
            });
        };
        $scope.deleteItem = function (event, target) {
            var modalInstance = $uibModal.open({
                backdrop: 'static',
                templateUrl: configService.buildUrl('nv/NvDatHangNCC', 'delete'),
                controller: 'phieuDatHangNCCSummaryDeleteController',
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
        $scope.printChild = function (target) {
            var modalInstance = $uibModal.open({
                backdrop: 'static',
                templateUrl: configService.buildUrl('/nv/NvDatHangNCC', 'printChild-summary'),
                controller: 'printChildDatHangNCCController',
                resolve: {
                    targetData: function () {
                        return target;
                    }
                }
            });
        };
        function filterData() {
            $scope.isLoading = true;
            var postdata = { paged: $scope.paged, filtered: $scope.filtered };
            service.postQuerySummary(
                JSON.stringify(postdata),
                function (response) {
                    $scope.isLoading = false;
                    if (response.status) {
                        $scope.data = response.data.data;
                        angular.extend($scope.paged, response.data);
                    }
                });
        };
    }
    ]);
    app.controller('phieuDatHangNCCSummaryCreateController', ['$scope', '$uibModal', '$uibModalInstance', 'configService', 'phieuDatHangNCCService', 'tempDataService', '$filter', '$log', 'ngNotify', 'securityService', '$rootScope', 'toaster', 'merchandiseService', 'userService',
        function ($scope, $uibModal, $uibModalInstance, configService, service, tempDataService, $filter, $log, ngNotify, securityService, $rootScope, toaster, serviceMerchandise, serviceAuthUser) {
            $scope.config = angular.copy(configService);
            var currentUser = serviceAuthUser.GetCurrentUser();
            var unitCode = currentUser.unitCode;
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.robot = angular.copy(service.robot);
            $scope.filtered = angular.copy(configService.filterDefault);
            $scope.tempData = tempDataService.tempData;
            //--------
            $scope.title = function () {
                return 'Tổng hợp phiếu đặt hàng Nhà cung cấp';
            };
            $scope.cancel = function () {
                $scope.target.dataDetails = [];
                $uibModalInstance.dismiss('cancel');
            };
            $scope.newItem = {};
            $scope.tyGia = 0;
            $scope.addRow = function () {
                $scope.addRow = function () {
                    if (!$scope.newItem.soLuong || $scope.newItem.soLuong < 1) {
                        focus('soluong');
                        document.getElementById('soluong').focus();
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
                                    $scope.target.dataDetails[k].soLuong = $scope.newItem.soLuong + $scope.target.dataDetails[k].soLuong;
                                    $scope.target.dataDetails[k].soLuongBao = $scope.newItem.soLuongBao + $scope.target.dataDetails[k].soLuongBao;
                                    $scope.target.dataDetails[k].soLuongLe = $scope.newItem.soLuongLe + $scope.target.dataDetails[k].soLuongLe;
                                    $scope.target.dataDetails[k].thanhTien = $scope.newItem.soLuong * $scope.target.dataDetails[k].donGia;
                                    service.robot.changeSoLuongLe($scope.target.dataDetails[k]);
                                }
                            });
                        } else {
                            $scope.target.dataDetails.push($scope.newItem);
                        }
                    } else {
                        toaster.pop('error', "Lỗi:", "Mã hàng chưa đúng!");
                    }
                    $scope.pageChanged();
                    $scope.newItem = {};
                    focus('mahang');
                    document.getElementById('mahang').focus();
                };
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
                        $scope.newItem.donGia = updatedData.giaMua;
                        $scope.newItem.validateCode = updatedData.maHang;
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
            }
            $scope.selectedMaHang = function (code) {
                if (code) {
                    service.getMerchandiseForNvByCode(code, null, unitCode).then(function (response) {
                        if (response.data.data && response.data.status) {
                            $scope.newItem = response.data.data;
                            $scope.newItem.donGia = response.data.data.giaMua;
                            $scope.newItem.validateCode = response.data.data.maHang;
                        } else {
                            $scope.addNewItem(code);
                        }
                    }, function (error) {
                        $scope.addNewItem(code);
                    });
                }
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
            $scope.createPackage = function (target, name) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('mdPackaging', 'add'),
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

            $scope.selectedTax = function (target) {
                for (var i = 0; i < $scope.tempData('taxs').length; i++) {
                    var tmp = $scope.tempData('taxs')[i];
                    if (target.vat == tmp.value) {
                        $scope.tyGia = tmp.extendValue;
                    }
                }
            };
            $scope.isSummarized = false;
            $scope.target = { dataDetails: [] };
            $scope.formatLabel = function (model, module) {
                if (!model) return "";
                var data = $filter('filter')($scope.tempData(module), { value: model }, true);
                if (data && data.length == 1) {
                    return data[0].text;
                }
                return "Empty!";
            };
            $scope.save = function () {
                service.postAddNewSummary(
                    JSON.stringify($scope.target), function (response) {
                        if (response.status) {
                            ngNotify.set("Thành công", { type: 'success' });
                            $scope.target.dataDetails = [];
                            $uibModalInstance.close($scope.target);
                        } else {
                            ngNotify.set(response.message, { duration: 3000, type: 'error' });
                        }
                    }
                    );
            };
            function filterData() {
                $scope.isLoading = true;
                service.getNewInstance(function (response) {
                    $scope.target = response;
                    $scope.target.ngay = new Date($scope.target.ngay);
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
                });
            };
            filterData();
            $scope.pageChanged = function () {
                var currentPage = $scope.paged.currentPage;
                var itemsPerPage = $scope.paged.itemsPerPage;
                $scope.paged.totalItems = $scope.target.dataDetails.length;
                $scope.data = [];
                if ($scope.target.dataDetails) {
                    for (var i = (currentPage - 1) * itemsPerPage; i < currentPage * itemsPerPage && i < $scope.target.dataDetails.length; i++) {
                        $scope.data.push($scope.target.dataDetails[i])
                    }
                }
            }
            $scope.selectPhieu = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('nv/NvDatHangNCC', 'selectData'),
                    controller: 'datHangNCCSelectDataController',
                    resolve: {
                        serviceSelectData: function () {
                            return service;
                        },
                        filterObject: function () {
                            return {
                                advanceData: {
                                    maNhaCungCap: $scope.target.maNhaCungCap,
                                    trangThai: 20 //Được duyệt, chưa hoàn thành
                                }
                            }
                        }
                    }
                });
                modalInstance.result.then(function (updatedData) {
                    $scope.target.maNhaCungCap = updatedData[0].maNhaCungCap;
                    $scope.lstPhieuDatHang = [];
                    $scope.target.soPhieuCon = '';
                    angular.forEach(updatedData, function (v, k) {
                        $scope.lstPhieuDatHang.push(v.soPhieuPk);
                        $scope.target.soPhieuCon = $scope.target.soPhieuCon + v.soPhieuPk + ',';
                    })
                    $scope.isLoading = true;
                    service.postMerge($scope.lstPhieuDatHang, function (res) {
                        if (res && res.status) {
                            $scope.isSummarized = true;
                            $scope.isLoading = false;
                            angular.extend($scope.target.dataDetails, res.data);
                            $scope.pageChanged();
                        }
                        else {
                            ngNotify.set(res.message, { duration: 3000, type: 'error' });
                        }
                    });
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            }
        }
    ]);
    app.controller('phieuDatHangNCCSummaryDeleteController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'phieuDatHangNCCService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify',
    function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify) {
        $scope.config = angular.copy(configService);
        $scope.isLoading = false;
        $scope.target = angular.copy(targetData);
        $scope.title = function () { return 'Xoá thành phần'; };
        $scope.save = function () {
            service.deleteSummary(targetData).then(function (successRes) {
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
    app.controller('phieuGiaoNhanController', ['$scope', '$uibModal', '$uibModalInstance', 'configService', 'targetData', 'phieuDatHangNCCService', 'tempDataService', '$filter', '$log', 'ngNotify', 'securityService', '$rootScope', 'toaster', 'merchandiseService', '$state',
    function ($scope, $uibModal, $uibModalInstance, configService, targetData, service, tempDataService, $filter, $log, ngNotify, securityService, $rootScope, toaster, serviceMerchandise, $state) {
        $scope.config = angular.copy(configService);
        $scope.paged = angular.copy(configService.pageDefault);
        $scope.robot = angular.copy(service.robot);
        $scope.filtered = angular.copy(configService.filterDefault);
        $scope.tempData = tempDataService.tempData;
        $scope.target = targetData;
        $scope.title = function () {
            return 'Phiếu giao nhận đặt hàng Nhà cung cấp';
        };
        $scope.formatLabel = function (model, module) {
            if (!model) return "";
            var data = $filter('filter')($scope.tempData(module), { value: model }, true);
            if (data && data.length == 1) {
                return data[0].text;
            }
            return "Empty!";
        };
        function filterData() {
            $scope.isLoading = true;
            service.getDetails($scope.target.id, function (response) {
                if (response.status) {
                    $scope.target = response.data;
                    $scope.target.ngay = new Date($scope.target.ngay);
                }
                $scope.isLoading = false;
                $scope.pageChanged();
            });
        }
        filterData();
        $scope.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        };
        $scope.save = function () {
            service.postReceiveSummary(
                JSON.stringify($scope.target), function (response) {
                    if (response.status) {
                        ngNotify.set("Thành công", { type: 'success' });
                        $scope.target.dataDetails = [];
                        $uibModalInstance.close(response.data);
                    } else {
                        ngNotify.set(response.message, { duration: 3000, type: 'error' });
                    }
                }
                );
        }
        $scope.pageChanged = function () {
            var currentPage = $scope.paged.currentPage;
            var itemsPerPage = $scope.paged.itemsPerPage;
            $scope.paged.totalItems = $scope.target.dataDetails.length;
            $scope.data = [];
            if ($scope.target.dataDetails) {
                for (var i = (currentPage - 1) * itemsPerPage; i < currentPage * itemsPerPage && i < $scope.target.dataDetails.length; i++) {
                    $scope.data.push($scope.target.dataDetails[i])
                }
            }
        }
    }
    ]);
    app.controller('printChildDatHangNCCController', ['$scope', '$uibModal', '$uibModalInstance', 'configService', 'targetData', 'phieuDatHangNCCService', 'tempDataService', '$filter', '$log', 'ngNotify', 'securityService', '$rootScope', 'toaster', 'merchandiseService', '$state',
function ($scope, $uibModal, $uibModalInstance, configService, targetData, service, tempDataService, $filter, $log, ngNotify, securityService, $rootScope, toaster, serviceMerchandise, $state) {
    $scope.config = angular.copy(configService);
    $scope.paged = angular.copy(configService.pageDefault);
    $scope.robot = angular.copy(service.robot);
    $scope.filtered = angular.copy(configService.filterDefault);
    $scope.tempData = tempDataService.tempData;
    $scope.target = targetData;
    $scope.title = function () {
        return 'In phiếu con Đặt hàng Nhà cung cấp';
    };
    $scope.formatLabel = function (model, module) {
        if (!model) return "";
        var data = $filter('filter')($scope.tempData(module), { value: model }, true);
        if (data && data.length == 1) {
            return data[0].text;
        }
        return "Empty!";
    };
    function filterData() {
        $scope.isLoading = true;
        service.getChild($scope.target.id, function (response) {
            if (response.status) {
                $scope.target = response.data;
            }
            $scope.isLoading = false;
        });
    }
    filterData();
    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };

    $scope.details = function (target) {
        var modalInstance = $uibModal.open({
            backdrop: 'static',
            templateUrl: configService.buildUrl('/nv/NvDatHangNCC', 'details-summary'),
            controller: 'phieuDatHangNCCDetailsController',
            windowClass: 'app-modal-window',
            resolve: {
                targetData: function () {
                    return target;
                }
            }
        });
    };
    $scope.receive = function (target) {
        var modalInstance = $uibModal.open({
            backdrop: 'static',
            templateUrl: configService.buildUrl('/nv/NvDatHangNCC', 'receive-summary'),
            controller: 'phieuGiaoNhanController',
            windowClass: 'app-modal-window',
            resolve: {
                targetData: function () {
                    return target;
                }
            }
        });
        modalInstance.result.then(function (updatedData) {
            filterData();
        }, function () {
            $log.info('Modal dismissed at: ' + new Date());
        });
    };
}
    ]);
    app.controller('datHangNCCSelectDataController', [
    '$scope', '$resource', '$rootScope', '$location', '$window', '$uibModal', '$uibModalInstance', '$log', '$state',
    'phieuDatHangNCCService', 'configService', 'filterObject', '$filter', 'tempDataService',
    function ($scope, $resource, $rootScope, $location, $window, $uibModal, $uibModalInstance, $log, $state,
        service, configService, filterObject, $filter, tempDataService) {
        $scope.config = angular.copy(configService);
        $scope.paged = angular.copy(configService.pageDefault);
        $scope.robot = angular.copy(service.robot);
        $scope.filtered = angular.copy(configService.filterDefault);
        $scope.tempData = tempDataService.tempData;
        angular.extend($scope.filtered, filterObject);
        $scope.isEditable = true;
        $scope.setPage = function (pageNo) {
            $scope.paged.currentPage = pageNo;
            filterData();
        };
        $scope.selecteItem = function (item) {
            $uibModalInstance.close(item);
        }
        $scope.selectedDonVi = function () {
            filterData();
        }
        $scope.formatLabel = function (model, module) {
            if (!model) return "";
            var data = $filter('filter')($scope.tempData(module), { value: model }, true);
            if (data && data.length == 1) {
                return data[0].text;
            }
            return "Empty!";
        };
        $scope.sortType = 'soPhieu'; // set the default sort type
        $scope.sortReverse = false;  // set the default sort order
        $scope.doSearch = function () {
            $scope.paged.currentPage = 1;
            filterData();
        };
        $scope.pageChanged = function () {
            filterData();
        };
        $scope.refresh = function () {
            $scope.setPage($scope.paged.currentPage);
        };
        $scope.title = function () {
            return 'Đặt hàng NCC';
        };

        $scope.doCheck = function (item) {
            if (item) {
                var isSelected = $scope.listSelectedData.some(function (element, index, array) {
                    return element.id == item.id;
                });
                if (item.selected) {
                    if (!isSelected) {
                        $scope.listSelectedData.push(item);
                    }
                } else {
                    if (isSelected) {
                        if (item.maNhaCungCap != $scope.filtered.advanceData.maNhaCungCap) {
                            console.log('err');
                            ngNotify.set('Vui lòng chọn phiếu của 1 NCC', { duration: 3000, type: 'error' });
                            return;
                        }
                        $scope.listSelectedData.splice(item, 1);
                    }
                }
            } else {
                angular.forEach($scope.data, function (v, k) {
                    $scope.data[k].selected = $scope.all;
                    var isSelected = $scope.listSelectedData.some(function (element, index, array) {
                        if (!element) return false;
                        return element.id == v.id;
                    });
                    if ($scope.all) {
                        if (!isSelected) {
                            if (v.maNhaCungCap != $scope.filtered.advanceData.maNhaCungCap) {
                                ngNotify.set('Vui lòng chọn phiếu của 1 NCC', { duration: 3000, type: 'error' });
                                return;
                            }
                            $scope.listSelectedData.push($scope.data[k]);
                        }
                    } else {
                        if (isSelected) {
                            $scope.listSelectedData.splice($scope.data[k], 1);
                        }
                    }
                });
            }
        }
        $scope.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        };
        $scope.save = function () {
            $uibModalInstance.close($scope.listSelectedData);
        };
        filterData();
        function filterData() {
            $scope.listSelectedData = service.getSelectData();
            $scope.isLoading = true;
            var postdata = { paged: $scope.paged, filtered: $scope.filtered };
            service.postSelectData(
                JSON.stringify(postdata),
                function (response) {
                    $scope.isLoading = false;
                    if (response.status) {
                        $scope.data = response.data.data;
                        angular.forEach($scope.data, function (v, k) {
                            var isSelected = $scope.listSelectedData.some(function (element, index, array) {
                                if (!element) return false;
                                return element.soPhieu == v.soPhieu;
                            });
                            if (isSelected) {
                                $scope.data[k].selected = true;
                            }
                        })
                        angular.extend($scope.paged, response.data);
                    }
                });
        };
    }]);
    /* report Phieu DatHangNCC Controller */
    app.controller('printDonDatHangNCCController', ['$scope', '$location', '$http', 'configService', 'phieuDatHangNCCService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'userService', '$stateParams', '$window',
        function ($scope, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, serviceAuthUser, $stateParams, $window) {
            var currentUser = serviceAuthUser.GetCurrentUser();
            $scope.robot = angular.copy(service.robot);
            var id = $stateParams.id;
            $scope.target = {};
            $scope.goIndex = function () {
                $state.go('nvDatHangNCC');
            }
            $scope.tempData = tempDataService.tempData;
            function filterData() {
                if (id) {
                    service.getReport(id, function (response) {
                        if (response && response.status && response.data) {
                            $scope.target = response.data;
                        }
                    });
                    $scope.currentUser = currentUser.userName;
                }
            };
            filterData();
            $scope.formatLabel = function (paraValue, moduleName) {
                var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                if (data && data.length === 1) {
                    return data[0].description;
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
                myWindow.document.close();
                myWindow.focus();
                myWindow.print();
                myWindow.close();;
            }
            $scope.printExcel = function () {
                var data = [document.getElementById('main-report').innerHTML];
                var fileName = "DonDatHang_ExportData.xls";
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
    app.controller('printBienBanNCCController', ['$scope', '$location', '$http', 'configService', 'phieuDatHangNCCService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'userService', '$stateParams', '$window',
    function ($scope, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, serviceAuthUser, $stateParams, $window) {
        var currentUser = serviceAuthUser.GetCurrentUser();
        $scope.robot = angular.copy(service.robot);
        var id = $stateParams.id;
        $scope.target = {};
        $scope.goIndex = function () {
            $state.go('nvDatHangNCC');
        }
        $scope.tempData = tempDataService.tempData;
        function filterData() {
            if (id) {
                service.getReport(id, function (response) {
                    if (response && response.status && response.data) {
                        $scope.target = response.data;
                    }
                });
                $scope.currentUser = currentUser.userName;
            }
        };
        filterData();
        $scope.formatLabel = function (paraValue, moduleName) {
            var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
            if (data && data.length === 1) {
                return data[0].description;
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
            myWindow.document.close();
            myWindow.focus();
            myWindow.print();
            myWindow.close();


        }
        $scope.printExcel = function () {
            var data = [document.getElementById('main-report').innerHTML];
            var fileName = "DonDatHang_ExportData.xls";
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
    return app;
});