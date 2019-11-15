/*  
* Người tạo : Nguyễn Tuấn Hoàng Anh
* View: BTS.SP.MART/views/nv/NvNhapKhac
* Vm sevices: BTS.API.SERVICE -> NV ->NvNhapKhacVm.cs
* Sevices: BTS.API.SERVICE -> NV -> NvNhapKhacService.cs
* Entity: BTS.API.ENTITY -> NV - > NvNhapKhac.cs
* Menu: Nghiệp vụ-> Nhập hàng mua
*/
define(['ui-bootstrap', '/BTS.SP.MART/controllers/auth/AuthController.js', '/BTS.SP.MART/controllers/htdm/periodController.js', '/BTS.SP.MART/controllers/htdm/merchandiseController.js', '/BTS.SP.MART/controllers/htdm/customerController.js', '/BTS.SP.MART/controllers/htdm/merchandiseTypeController.js', '/BTS.SP.MART/controllers/htdm/nhomVatTuController.js', '/BTS.SP.MART/controllers/htdm/supplierController.js', '/BTS.SP.MART/controllers/htdm/wareHouseController.js', '/BTS.SP.MART/controllers/htdm/packagingController.js', '/BTS.SP.MART/controllers/htdm/taxController.js', '/BTS.SP.MART/controllers/htdm/donViTinhController.js', '/BTS.SP.MART/controllers/htdm/typeReasonController.js', '/BTS.SP.MART/controllers/auth/AuNguoiDung.js'], function () {
    'use strict';
    var app = angular.module('phieuNhapKhacModule', ['ui.bootstrap', 'authModule', 'periodModule', 'merchandiseModule', 'customerModule', 'merchandiseTypeModule', 'nhomVatTuModule', 'supplierModule', 'wareHouseModule', 'packagingModule', 'taxModule', 'donViTinhModule', 'typeReasonModule', 'AuNguoiDungModule']);
    app.factory('phieuNhapKhacService', ['$http', 'configService', function ($http, configService) {
        var serviceUrl = configService.rootUrlWebApi + '/NV/NhapKhac';
        var rootUrl = configService.apiServiceBaseUri;
        this.parameterPrint = {};
        var maLyDo = '';
        function getParameterPrint() {
            return this.parameterPrint;
        }
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
                target.thanhTienVAT = target.thanhTienTruocVat * (1 + target.chietKhau / 100);
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
                target.thanhTienVAT = target.thanhTienTruocVat * (1 + target.chietKhau / 100);
            },
            changeSoLuongBao: function (item) {
                if (!item.soLuongLe) {
                    item.soLuongLe = 0;
                }
                if (!item.maBaoBi) {
                    item.luongBao = 1;
                }
                if (item) {
                    item.soLuong = item.soLuongBao * item.luongBao + item.soLuongLe;
                    item.thanhTien = item.soLuong * item.donGia;
                    item.thanhTienVAT = item.thanhTien * (1 + item.tyLeVatVao / 100);
                }
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
                if (item) {
                    item.soLuong = item.soLuongBao * item.luongBao + item.soLuongLe;
                    item.thanhTien = item.soLuong * item.donGia;
                    item.giaMuaCoVat = item.donGia * (1 + item.tyLeVatVao / 100);
                    item.thanhTienVAT = item.thanhTien * (1 + item.tyLeVatVao / 100);
                }
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
                if (item) {
                    item.soLuong = item.soLuongBao * item.luongBao + item.soLuongLe;
                    item.thanhTien = item.soLuong * item.donGia;
                    item.thanhTienVAT = item.thanhTien * (1 + item.tyLeVatVao / 100);
                }

            },
            changeGiaMuaDonGia: function (item) {
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
                if (item) {
                    item.donGia = item.giaMuaCoVat / (1 + item.tyLeVatVao / 100);
                    item.thanhTien = item.soLuong * item.donGia;
                    item.thanhTienVAT = item.thanhTien * (1 + item.tyLeVatVao / 100);
                }
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
                return $http.get(rootUrl + '/api/Md/Supplier/' + id);
            },
            getWareHouseByCode: function (code) {
                return $http.get(rootUrl + '/api/Md/WareHouse/GetByCode/' + code);
            },
            getOrderById: function (id) {
                return $http.get(rootUrl + '/api/Nv/DatHang/GetDetailComplete/' + id);
            },
            getOrderByCustomer: function (code) {
                return $http.get(rootUrl + '/api/Nv/DatHang/GetSelectDataIsCompleteByCustomerCode/' + code);
            },
            getOrder: function () {
                return $http.get(rootUrl + '/api/Nv/DatHang/GetSelectDataIsComplete');
            },
            postApproval: function (data) {
                return $http.post(serviceUrl + '/PostApproval', data);
            },
            updateCT: function (params) {
                return $http.put(serviceUrl + '/' + params.id, params);
            },
            getMerchandiseForNvByCode: function (code, wareHouseCode, unitCode) {
                return $http.get(rootUrl + '/api/Md/Merchandise/GetForNvByCode/' + code + '/' + wareHouseCode + '/' + unitCode);
            },
            getMerchandise: function (maChungTuPk) {
                return $http.get(rootUrl + '/api/Nv/NhapKhac/GetMerchandise/' + maChungTuPk);
            },
            getUnitName: function (maDonVi) {
                return $http.get(rootUrl + '/api/Md/UnitUser/GetUnitName/' + maDonVi);
            },
            getCustomerName: function (maHang) {
                return $http.get(rootUrl + '/api/Md/Merchandise/GetByCode/' + maHang);
            },
            getCurrentUser: function () {
                return $http.get(rootUrl + '/api/Authorize/AuNguoiDung/GetCurrentUser');
            },
            writeDataToExcel: function (data) {
                return $http.post(serviceUrl + '/WriteDataToExcel', data);
            },

            exportReportToExcel: function (data) {
                return $http.post(serviceUrl + '/ExportReportToExcel', data);
            },
            getDetailByCode: function (code) {
                return $http.get(rootUrl + '/api/Md/Merchandise/GetDetailByCode/' + code);
            },
            getByCode: function (code) {
                return $http.get(rootUrl + '/api/Md/Merchandise/GetByCode/' + code);
            },
            getDetailInTem: function (code) {
                return $http.get(rootUrl + '/api/Md/Merchandise/GetDetailInTem/' + code);
            },
            getInfoItemDetails: function (id) {
                return $http.get(serviceUrl + '/GetInfoItemDetails/' + id);
            },
            deleteItem: function (params) {
                return $http.delete(serviceUrl + '/' + params.id, params);
            },
            getByCodeWithGiaVon: function (code, wareHouseCode, maDonVi) {
                return $http.get(rootUrl + '/api/Md/Merchandise/GetByCodeWithGiaVon/' + code + '/' + wareHouseCode + '/' + maDonVi);
            }
        };
        return result;
    }]);
    /* controller list */
    app.controller('phieuNhapKhacController', [
        '$scope', '$location', '$http', 'configService', 'phieuNhapKhacService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'securityService', '$rootScope', 'toaster', 'periodService', 'merchandiseService', 'customerService', 'merchandiseTypeService', 'nhomVatTuService', 'supplierService', 'wareHouseService', 'packagingService', 'taxService', 'donViTinhService', 'typeReasonService',
        function ($scope, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, securityService, $rootScope, toaster, servicePeriod, serviceMerchandise, serviceCustomer, serviceMerchandiseType, serviceNhomVatTu, serviceSupplier, serviceWareHouse, servicePackaging, serviceTax, serviceDonViTinh, serviceTypeReason) {
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
            $scope.displayHepler = function (paraValue, moduleName) {
                var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);


                if (data && data.length === 1) {
                    return data[0].text;
                } else {
                    return paraValue;
                }
            }
            //end check
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.filtered = angular.copy(configService.filterDefault);
            $scope.tempData = tempDataService.tempData;
            $scope.isEditable = true;
            $scope.accessList = {};
            //load dữ liệu
            function filterData() {
                $scope.isLoading = true;
                if ($scope.accessList.view) {
                    var postdata = {};
                    $scope.isLoading = true;
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

            //check quyền truy cập
            function loadAccessList() {
                securityService.getAccessList('phieuNhapKhac').then(function (successRes) {
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

            function loadTypeReasonNhap() {
                if (!tempDataService.tempData('typeReasonNhaps')) {
                    serviceTypeReason.getAll_TypeReasonByParent('NHAP').then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                            tempDataService.putTempData('typeReasonNhaps', successRes.data.data);
                            $scope.typeReasonNhaps = successRes.data.data;
                        } else {
                            console.log('successRes', successRes);
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.typeReasonNhaps = tempDataService.tempData('typeReasonNhaps');
                }
            }

            loadTypeReasonNhap();
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
            $scope.sortType = 'maNvNhapKhac';
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
                service.maLyDo = $scope.filtered.advanceData.maLyDo;
            };
            $scope.title = function () { return 'Phiếu nhập khác'; };

            /* Function add New Item */
            $scope.create = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    size: 'md',
                    templateUrl: configService.buildUrl('nv/NvNhapKhac', 'add'),
                    controller: 'phieuNhapKhacCreateController',
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
                    templateUrl: configService.buildUrl('nv/NvNhapKhac', 'update'),
                    controller: 'phieuNhapKhacEditController',
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

            /* Function Details Item */
            $scope.details = function (target) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('nv/NvNhapKhac', 'details'),
                    controller: 'phieuNhapKhacDetailsController',
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

            /* Function Print Item */
            $scope.printItem = function (target) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('nv/NvNhapKhac', 'printItem'),
                    controller: 'phieuNhapKhacExportItemController',
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
            $scope.deleteItem = function (target) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('nv/NvNhapKhac', 'delete'),
                    controller: 'phieuNhapKhacDeleteController',
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
                $state.go("nvPrintDetailPhieuNhapKhac");
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
    app.controller('phieuNhapKhacCreateController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'phieuNhapKhacService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', '$rootScope', 'userService', 'FileUploader', 'merchandiseService', 'customerService', 'merchandiseTypeService', 'nhomVatTuService', 'supplierService', 'wareHouseService', 'packagingService', 'taxService', 'donViTinhService', 'periodService', 'typeReasonService', 'toaster',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, $rootScope, serviceAuthUser, FileUploader, serviceMerchandise, serviceCustomer, serviceMerchandiseType, serviceNhomVatTu, serviceSupplier, serviceWareHouse, servicePackaging, serviceTax, serviceDonViTinh, servicePeriod, serviceTypeReason, toaster) {
            var currentUser = serviceAuthUser.GetCurrentUser();
            var unitCode = currentUser.unitCode;
            var rootUrl = configService.apiServiceBaseUri;
            var serviceUrl = rootUrl + '/api/Nv/NhapKhac';
            $scope.robot = angular.copy(service.robot);
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.target = { dataDetails: [] };
            $scope.data = [];
            $scope.newItem = {};
            $scope.donHangs = [];
            $scope.target = { dataDetails: [], dataClauseDetails: [] };
            $scope.tkKtKhoNhap = "";
            $scope.tyGia = 0;
            $scope.isListItemNull = true;
            $scope.tempData = tempDataService.tempData;
            $scope.putTempData = tempDataService.putTempData;

            $scope.isLoading = false;
            $scope.title = function () { return 'Phiếu nhập khác'; };
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
            function loadTypeReasonNhap() {
                if (!tempDataService.tempData('typeReasonNhaps')) {
                    serviceTypeReason.getAll_TypeReasonByParent('NHAP').then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                            tempDataService.putTempData('typeReasonNhaps', successRes.data.data);
                            $scope.typeReasonNhaps = successRes.data.data;
                        } else {
                            console.log('successRes', successRes);
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.typeReasonNhaps = tempDataService.tempData('typeReasonNhaps');
                }
            }

            loadTypeReasonNhap();

            loadSupplier();
            loadMerchandiseType();
            loadNhomVatTu();
            loadWareHouse();
            loadPackagings();
            loadTax();
            loadDonViTinh();
            //end 


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
                        $scope.target.maLyDo = service.maLyDo;
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
                            $scope.target.thanhTienTruocVat = $scope.robot.sum($scope.target.dataDetails, 'thanhTienVAT');
                            $scope.target.tienVat = $scope.robot.sumVat($scope.tyGia, $scope.target);
                            $scope.target.thanhTienSauVat = $scope.target.thanhTienTruocVat + $scope.target.tienVat - $scope.target.tienChietKhau;
                        });
                        $scope.$watch("target.dataDetails", function (newValue, oldValue) {
                            if (!$scope.target.tienChietKhau) {
                                $scope.target.tienChietKhau = 0;
                            }
                            $scope.target.thanhTienTruocVat = $scope.robot.sum($scope.target.dataDetails, 'thanhTienVAT');
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
                                $scope.target.dataDetails[k].soLuong = parseInt($scope.newItem.soLuong) + parseInt($scope.target.dataDetails[k].soLuong);
                                $scope.target.dataDetails[k].soLuongBao = parseInt($scope.newItem.soLuongBao) + parseInt($scope.target.dataDetails[k].soLuongBao);
                                $scope.target.dataDetails[k].soLuongLe = parseInt($scope.newItem.soLuongLe) + parseInt($scope.target.dataDetails[k].soLuongLe);
                                $scope.target.dataDetails[k].thanhTien = parseInt($scope.newItem.soLuong) * parseInt($scope.target.dataDetails[k].donGia);
                                service.robot.changeSoLuongLe($scope.target.dataDetails[k]);
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
                        //-------------------------
                        service.getByCodeWithGiaVon(updatedData.maHang, $scope.target.maKhoNhap, unitCode).then(function (response) {
                            if (response && response.status === 200 && response.data.status) {
                                console.log(response.data.data);
                                $scope.newItem.donGia = response.data.data.giaVon;
                                updatedData.donGia = response.data.data.giaVon;
                                $scope.newItem.giaBanLe = updatedData.giaBanLe;
                                $scope.newItem.giaMuaCoVat = response.data.data.giaVon * (1 + updatedData.tyLeVatVao / 100);
                                $scope.newItem.validateCode = updatedData.maHang;
                            } else {
                                updatedData.donGia = updatedData.giaMua;
                                $scope.newItem.validateCode = updatedData.maHang;
                                $scope.newItem.giaBanLe = updatedData.giaBanLe;
                                $scope.newItem.giaMuaCoVat = updatedData.giaMua * (1 + updatedData.tyLeVatVao / 100);
                            }
                        });
                        //-------------------------
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

            $scope.selectedkhachHang = function (item) {
                $scope.target.maKhachHang = item.value;
                service.getOrderByCustomer(item.value).then(function (response) {
                    $scope.donHangs = response;
                });
                service.getCustomer(item.id, function (response) {
                    $scope.target.maSoThue = response.maSoThue;
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
            $scope.selectedMaHang = function (code) {
                if (code) {
                    service.getMerchandiseForNvByCode(code, $scope.target.maKhoNhap, unitCode).then(function (response) {
                        if (response && response.status == 200 && response.data && response.data.status) {
                            $scope.newItem = response.data.data;
                            $scope.newItem.donGia = $scope.newItem.giaMua;
                            $scope.newItem.vat = response.data.data.maVatVao;
                            $scope.newItem.validateCode = response.data.data.maHang;
                            $scope.newItem.giaMuaCoVat = response.data.data.giaMua * (1 + response.data.data.tyLeVatVao / 100);
                        }
                        else {
                            $scope.addNewItem(code);
                        }
                        $scope.robot.changeDonGia($scope.newItem);
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
                service.post($scope.target).then(function (successRes) {
                    if (successRes && successRes.status === 201 && successRes.data) {
                        ngNotify.set("Thêm thành công", { type: 'success' });
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
                    serviceWareHouse.getAll_WareHouse().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                            tempDataService.putTempData('wareHouses', successRes.data.data);
                            $scope.wareHouses = successRes.data.data;
                            target[name] = updatedData.maKho;
                        } else {
                            console.log('successRes', successRes);
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
                    serviceMerchandiseType.getAll_MerchandiseType().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                            tempDataService.putTempData('merchandiseTypes', successRes.data.data);
                            $scope.merchandiseTypes = successRes.data.data;
                        } else {
                            console.log('successRes', successRes);
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
                    servicePackaging.getAll_Packaging().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                            tempDataService.putTempData('packagings', successRes.data.data);
                            $scope.packagings = successRes.data.data;
                        } else {
                            console.log('successRes', successRes);
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
                    tempDataService.putTempData('customers', updatedData);
                    target[name] = updatedData.maKhachHang;

                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };
            $scope.cancel = function () {
                $scope.isListItemNull = true;
                $uibModalInstance.dismiss('cancel');
            };
        }]);
    /* controller Edit */
    app.controller('phieuNhapKhacEditController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'phieuNhapKhacService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify', 'merchandiseService', 'typeReasonService', 'toaster','taxService',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify, serviceMerchandise, serviceTypeReason, toaster, serviceTax) {
            $scope.config = angular.copy(configService);
            $scope.robot = angular.copy(service.robot);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.targetData = angular.copy(targetData);
            $scope.tempData = tempDataService.tempData;
            $scope.displayHepler = function (paraValue, moduleName) {
                var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                if (data && data.length === 1) {
                    return data[0].text;
                } else {
                    return paraValue;
                }
            }
            $scope.target = targetData;
            $scope.newItem = {};
            $scope.tkKtKhoNhap = "";
            $scope.tyGia = 0;

            $scope.isLoading = true;
            $scope.title = function () { return 'Cập nhật phiếu nhập khác'; };
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
                    if (response.data.id) {
                        $scope.target = response.data;
                        $scope.target.ngayCT = new Date(response.data.ngayCT);
                        if ($scope.target.dataDetails.length > 0) {
                            $scope.isListItemNull = false;
                            $scope.target.dataDetails.forEach(function (obj) {
                                obj.giaMuaCoVat = obj.donGia * (1 +obj.tyLeVatVao / 100);
                                obj.thanhTienVAT = obj.soLuong * obj.giaMuaCoVat;
                            });
                        }
                        serviceMerchandise.setSelectData($scope.target.dataDetails);
                        $scope.pageChanged();
                        if ($scope.target.vat) {
                            var z = $filter('filter')($scope.tempData('taxs'), { value: $scope.target.vat }, true);
                            $scope.tyGia = z[0].extendValue;
                        }
                        else {
                            $scope.tyGia = 0;
                        }
                        $scope.target.maKhachHang = response.data.maKhachHang;
                        $scope.$watch('target.vat', function (newValue, oldValue) {
                            if (!newValue) {
                                $scope.target.tienVat = 0;
                            } else {
                                $scope.target.tienVat = $scope.robot.sumVat($scope.tyGia, $scope.target);
                            }
                            $scope.target.thanhTienSauVat = $scope.target.thanhTienTruocVat + $scope.target.tienVat - $scope.target.tienChietKhau;
                        });
                        $scope.$watch('target.tienChietKhau', function (newValue, oldValue) {
                            $scope.target.thanhTienTruocVat = $scope.robot.sum($scope.target.dataDetails, 'thanhTienVAT');
                            $scope.target.tienVat = $scope.robot.sumVat($scope.tyGia, $scope.target);
                            $scope.target.thanhTienSauVat = $scope.target.thanhTienTruocVat + $scope.target.tienVat - $scope.target.tienChietKhau;
                        });
                        $scope.$watch("target.dataDetails", function (newValue, oldValue) {
                            if (!$scope.target.tienChietKhau) {
                                $scope.target.tienChietKhau = 0;
                            }
                            $scope.target.thanhTienTruocVat = $scope.robot.sum($scope.target.dataDetails, 'thanhTienVAT');
                            $scope.target.tienVat = $scope.robot.sumVat($scope.tyGia, $scope.target);
                            $scope.target.thanhTienSauVat = $scope.target.thanhTienTruocVat + $scope.target.tienVat - $scope.target.tienChietKhau;
                        }, true);
                    }
                });

            };
            filterData();
            function loadTypeReasonNhap() {
                if (!tempDataService.tempData('typeReasonNhaps')) {
                    serviceTypeReason.getAll_TypeReasonByParent('NHAP').then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                            tempDataService.putTempData('typeReasonNhaps', successRes.data.data);
                            $scope.typeReasonNhaps = successRes.data.data;
                            console.log('$scope.typeReasonNhaps', $scope.typeReasonNhaps);
                        } else {
                            console.log('successRes', successRes);
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.typeReasonNhaps = tempDataService.tempData('typeReasonNhaps');
                }
            }
            function loadTax() {
                if (!tempDataService.tempData('taxRate')) {
                    serviceTax.getAll_TyGia().then(function (successRes) {
                        console.log(successRes);
                        if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                            tempDataService.putTempData('taxRate', successRes.data.data);
                            $scope.taxRate = successRes.data.data;
                        } else {
                            console.log('successRes', successRes);
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.taxRate = tempDataService.tempData('taxRate');
                }
            }
            loadTax();
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
            loadTypeReasonNhap();
            loadSupplier();
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
                                $scope.target.dataDetails[k].soLuong = parseInt($scope.newItem.soLuong) + parseInt($scope.target.dataDetails[k].soLuong);
                                $scope.target.dataDetails[k].soLuongBao = parseInt($scope.newItem.soLuongBao) + parseInt($scope.target.dataDetails[k].soLuongBao);
                                $scope.target.dataDetails[k].soLuongLe = parseInt($scope.newItem.soLuongLe) + parseInt($scope.target.dataDetails[k].soLuongLe);
                                $scope.target.dataDetails[k].thanhTien = parseInt($scope.newItem.soLuong) * parseInt($scope.target.dataDetails[k].donGia);
                                service.robot.changeSoLuongLe($scope.target.dataDetails[k]);
                            }
                        })
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
                        //-------------------------
                        service.getByCodeWithGiaVon(updatedData.maHang, $scope.target.maKhoNhap, unitCode).then(function (response) {
                            if (response && response.status === 200 && response.data.status) {
                                $scope.newItem.donGia = response.data.data.giaVon;
                                updatedData.donGia = response.data.data.giaVon;
                                $scope.newItem.giaBanLe = updatedData.giaBanLe;
                                $scope.newItem.giaMuaCoVat = response.data.data.giaVon * (1 + updatedData.tyLeVatVao / 100);
                                $scope.newItem.validateCode = updatedData.maHang;
                            } else {
                                updatedData.donGia = updatedData.giaMua;
                                $scope.newItem.validateCode = updatedData.maHang;
                                $scope.newItem.giaBanLe = updatedData.giaBanLe;
                                $scope.newItem.giaMuaCoVat = updatedData.giaMua * (1 + updatedData.tyLeVatVao / 100);
                            }
                        });
                        //-------------------------
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
                    $scope.donHangs = response;
                });
                service.getCustomer(item.id, function (response) {
                    $scope.target.maSoThue = response.maSoThue;
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
            };
            $scope.selectedTkCo = function (item) {
                $scope.target.tkCo = item.value;
            };
            $scope.selectedMaHang = function (code) {
                if (code) {
                    service.getMerchandiseForNvByCode(code, $scope.target.maKhoNhap, unitCode).then(function (response) {
                        if (response.data.status) {
                            $scope.newItem = response.data.data;
                            $scope.newItem.donGia = $scope.newItem.giaMua;
                            $scope.newItem.vat = response.data.data.maVatVao;
                            $scope.newItem.validateCode = response.data.data.maHang;
                            $scope.newItem.giaMuaCoVat = response.data.data.giaMua * (1 + response.data.data.tyLeVatVao / 100);
                        } else {
                            $scope.addNewItem(code);

                        }
                        $scope.robot.changeDonGia($scope.newItem);
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
                        ngNotify.set("Sửa thành công", { type: 'success' });
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
                    if (successRes && successRes.status === 200 && successRes.data) {
                        ngNotify.set(successRes.data.message, { type: 'success' });
                        var url = $state.href('reportPhieuNhapKhac', { id: successRes.data.data.id });
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
            $scope.cancel = function () {

                $uibModalInstance.close();
            };

        }]);

    /* controller Details */
    app.controller('phieuNhapKhacDetailsController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'phieuNhapKhacService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify', 'typeReasonService','taxService',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify, serviceTypeReason, serviceTax) {
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.tempData = tempDataService.tempData;
            $scope.target = targetData;
            $scope.isLoading = false;
            $scope.title = function () { return 'Thông tin phiếu nhập khác'; };
            $scope.pageChanged = function () {
                var currentPage = $scope.paged.currentPage;
                var itemsPerPage = $scope.paged.itemsPerPage;
                $scope.paged.totalItems = $scope.target.dataDetails.length;
                $scope.data = [];
                if ($scope.target.dataDetails) {
                    for (var i = (currentPage - 1) * itemsPerPage; i < currentPage * itemsPerPage && i < $scope.target.dataDetails.length; i++) {
                        $scope.target.dataDetails[i].thanhTienVAT = $scope.target.dataDetails[i].thanhTien * (1 + $scope.target.dataDetails[i].tyLeVatVao / 100);
                        $scope.target.dataDetails[i].giaMuaCoVat = $scope.target.dataDetails[i].donGia * (1 + $scope.target.dataDetails[i].tyLeVatVao / 100);
                        $scope.data.push($scope.target.dataDetails[i]);
                    }
                }
            }
            $scope.displayHepler = function (paraValue, moduleName) {
                var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                console.log('data', data);
                if (data && data.length === 1) {
                    return data[0].text;
                } else {
                    return paraValue;
                }
            }
            //note
            function loadTypeReasonNhap() {
                if (!tempDataService.tempData('typeReasonNhaps')) {
                    serviceTypeReason.getAll_TypeReasonByParent('NHAP').then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                            tempDataService.putTempData('typeReasonNhaps', successRes.data.data);
                            $scope.typeReasonNhaps = successRes.data.data;
                        } else {
                            console.log('successRes', successRes);
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.typeReasonNhaps = tempDataService.tempData('typeReasonNhaps');
                }
            }
            function loadTax() {
                if (!tempDataService.tempData('taxRate')) {
                    serviceTax.getAll_TyGia().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                            tempDataService.putTempData('taxRate', successRes.data.data);
                            $scope.taxRate = successRes.data.data;
                        } else {
                            console.log('successRes', successRes);
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.taxRate = tempDataService.tempData('taxRate');
                }
            }
            loadTax();
            loadTypeReasonNhap();
            function fillterData() {
                $scope.isLoading = true;
                service.getDetails($scope.target.id).then(function (resgetDetails) {
                    if (resgetDetails.status) {
                        $scope.target = resgetDetails.data;
                        $scope.target.ngayCT = new Date(resgetDetails.data.ngayCT);
                        for (var i = 0; i < resgetDetails.data.dataDetails.length; i++) {
                            if ($scope.target.vat === '0004') resgetDetails.data.dataDetails[i].tyLeVatVao = 'KCT-0';
                        }
                    }
                    $scope.isLoading = false;
                    $scope.pageChanged();
                });
            }
            fillterData();
            $scope.approval = function () {
                service.postApproval($scope.target).then(function (response) {
                    if (response) {
                        alert("Duyệt thành công!");
                        $uibModalInstance.close($scope.target);
                        $scope.goIndex = function () {
                            $state.go('nvNhapKhac');
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
    app.controller('phieuNhapKhacDeleteController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'phieuNhapKhacService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify) {
            $scope.config = angular.copy(configService);
            $scope.tempData = tempDataService.tempData;
            $scope.target = targetData;
            $scope.isLoading = false;
            $scope.title = function () { return 'Xoá thành phần'; };
            $scope.save = function () {
                service.deleteItem($scope.target).then(function (successRes) {
                    if (successRes && successRes.status === 200) {
                        ngNotify.set(successRes.data.message, { type: 'success' });
                        $uibModalInstance.close($scope.target);
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
    app.controller('reportPhieuNhapKhacController', ['$scope', '$location', '$http', 'configService', 'phieuNhapKhacService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'userService', '$stateParams', '$window','taxService',
        function ($scope, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, serviceAuthUser, $stateParams, $window, serviceTax) {
            var currentUser = serviceAuthUser.GetCurrentUser();
            $scope.robot = angular.copy(service.robot);
            var id = $stateParams.id;
            $scope.tempData = tempDataService.tempData;
            $scope.target = {};
            var taxRate = 0;
            $scope.goIndex = function () {
                $state.go('nvNhapKhac');
            }
            function filterData() {
                if (id) {
                    service.getReport(id).then(function (response) {
                        if (response && response.status && response.data) {
                            $scope.target = response.data;
                            $scope.target.dataReportDetails.forEach(function (obj) {
                                obj.thanhTien = obj.soLuong * obj.giaMuaCoVat;
                            });
                            if ($scope.target.vat == null) {
                                $scope.target.dataReportDetails.forEach(function (obj) {
                                    if (obj.vat == null) {
                                        obj.vat = 0;
                                        obj.giaMuaCoVat = obj.donGia;
                                        obj.thanhTien = obj.soLuong * obj.giaMuaCoVat;
                                    }
                                });
                            }
                            else {
                                serviceTax.getTaxByCode(response.data.vat).then(function (res) {
                                    if (res.status == 200) {
                                        taxRate = res.data.taxRate;
                                        if (response && response.status && response.data) {
                                            $scope.target = response.data;
                                            $scope.target.dataReportDetails.forEach(function (obj) {
                                                obj.vat = taxRate;
                                                obj.giaMuaCoVat = obj.donGia * (1 + taxRate / 100);
                                                obj.thanhTien = obj.soLuong * obj.giaMuaCoVat;
                                            });
                                        }
                                    }
                                });
                            }
                        }
                    });
                    $scope.currentUser = currentUser.userName;
                }
            };
            filterData();
            function loadTax() {
                if (!tempDataService.tempData('taxRate')) {
                    serviceTax.getAll_TyGia().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                            tempDataService.putTempData('taxRate', successRes.data.data);
                            $scope.taxRate = successRes.data.data;
                        } else {
                            console.log('successRes', successRes);
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.taxRate = tempDataService.tempData('taxRate');
                }
            }
            loadTax();
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
                var fileName = "NhapKhac_ExportData.xls";
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
                    a.remove();
                }
            };
        }]);
    /* print Phieu Nhap Hang Mua Controller */
    app.controller('printPhieuNhapKhacController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'phieuNhapKhacService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify) {
            $scope.robot = angular.copy(service.robot);
            $scope.config = angular.copy(configService);
            $scope.targetData = angular.copy(targetData);
            $scope.target = targetData;
            var id = $stateParams.id;
            $scope.target = {};
            $scope.info = service.getParameterPrint().filtered.advanceData;
            $scope.goIndex = function () {
                $state.go('nvNhapKhac');
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
    app.controller('printDetailPhieuNhapKhacController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'phieuNhapKhacService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify) {
            $scope.robot = angular.copy(service.robot);
            $scope.config = angular.copy(configService);
            $scope.targetData = angular.copy(targetData);
            $scope.target = targetData;
            var id = $stateParams.id;
            $scope.target = {};
            $scope.info = service.getParameterPrint().filtered.advanceData;
            $scope.goIndex = function () {
                $state.go('nvNhapKhac');
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
    app.controller('phieuNhapKhacExportItemController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'phieuNhapKhacService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify',
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
                $state.go('nvNhapKhac');
            };
            $scope.cancel = function () {
                $uibModalInstance.close();
            };
        }]);
    return app;
});