/*  
* Người tạo : Nguyễn Tuấn Hoàng Anh
* View: BTS.SP.MART/views/nv/NvXuatBan
* Vm sevices: BTS.API.SERVICE -> NV ->NvXuatBanVm.cs
* Sevices: BTS.API.SERVICE -> NV -> NvXuatBanService.cs
* Entity: BTS.API.ENTITY -> NV - > NvXuatBan.cs
* Menu: Nghiệp vụ-> Phiếu xuất bán buôn
*/
define(['ui-bootstrap', '/BTS.SP.MART/controllers/auth/AuthController.js', '/BTS.SP.MART/controllers/htdm/periodController.js', '/BTS.SP.MART/controllers/htdm/merchandiseController.js', '/BTS.SP.MART/controllers/htdm/customerController.js', '/BTS.SP.MART/controllers/htdm/merchandiseTypeController.js', '/BTS.SP.MART/controllers/htdm/nhomVatTuController.js', '/BTS.SP.MART/controllers/htdm/supplierController.js', '/BTS.SP.MART/controllers/htdm/wareHouseController.js', '/BTS.SP.MART/controllers/htdm/packagingController.js', '/BTS.SP.MART/controllers/htdm/taxController.js', '/BTS.SP.MART/controllers/htdm/donViTinhController.js', '/BTS.SP.MART/controllers/nv/nvCongNoKhachHangController.js'], function () {
    'use strict';
    var app = angular.module('phieuXuatBanModule', ['ui.bootstrap', 'authModule', 'periodModule', 'merchandiseModule', 'customerModule', 'merchandiseTypeModule', 'nhomVatTuModule', 'supplierModule', 'wareHouseModule', 'packagingModule', 'taxModule', 'donViTinhModule', 'nvCongNoKhachHangModule']);
    app.factory('phieuXuatBanService', ['$http', 'configService', function ($http, configService) {
        var serviceUrl = configService.rootUrlWebApi + '/NV/XuatBan';
        var rootUrl = configService.apiServiceBaseUri;
        this.parameterPrint = {};
        function getParameterPrint() {
            return this.parameterPrint;
        }
        var serviceDatHangUrl = rootUrl + '/api/Nv/DatHang';
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
            sumVat: function (target) {
                var tienVat = 0;

                return tienVat;
            },
            changeChietKhau: function (target) {
                if (!target.thanhTienTruocVat) {
                    target.thanhTienTruocVat = 0;
                }
                if (!target.chietKhau) {
                    target.chietKhau = 0;
                }
                target.tienChietKhau = (target.thanhTienTruocVat * target.chietKhau) / 100;
                target.thanhTienTruocVatSauCK = target.thanhTienTruocVat - (target.thanhTienTruocVat * target.chietKhau) / 100;
            },
            changeTienChietKhau: function (target) {
                target.thanhTienTruocVatSauCK = target.thanhTienTruocVat - target.tienChietKhau;
            },
            changeSoLuongBao: function (item) {
                if (!item.soLuongLe) {
                    item.soLuongLe = 0;
                }
                if (!item.maBaoBi) {
                    item.luongBao = 1;
                }
                if (!item.giamGia) {
                    item.giamGia = 0;
                }
                if (isNaN(item.luongBao)) {
                    item.giamGia = 0;
                    if (item.soLuong > 0) item.thanhTien = item.soLuong * item.donGia - item.tienGiamGia;
                }
                item.soLuong = item.soLuongBao * item.luongBao + item.soLuongLe;
                item.tienTruocGiamGia = item.soLuong * item.donGia;
                if (item.giamGia < 100) {
                    item.donGia = item.giaBanBuonVat - (item.giaBanBuonVat * item.giamGia / 100);
                    item.tienGiamGia = (item.giaBanBuonVat - item.donGia) * item.soLuong;
                }
                else {
                    item.donGia = item.giaBanBuonVat - item.giamGia;
                    item.tienGiamGia = item.soLuong * item.giamGia;
                }
                item.thanhTien = item.soLuong * item.donGia - item.tienGiamGia;
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
                if (!item.giamGia) {
                    item.giamGia = 0;
                }
                if (isNaN(item.soLuongLe)) {
                    item.soLuong = 0;
                    item.giamGia = 0;
                    item.thanhTien = 0;
                }
                item.soLuong = item.soLuongBao * item.luongBao + item.soLuongLe;
                //item.donGia = item.giaBanLeVat;
                item.tienGiamGia = item.giamGia * item.soLuong;
                item.tienTruocGiamGia = item.soLuong * item.donGia;
                item.thanhTien = item.soLuong * item.donGia - item.tienGiamGia;
            },
            changeDonGia: function (item) {
                if (!item.soLuong) {
                    item.soLuong = 0;
                }
                if (!item.giamGia) {
                    item.giamGia = 0;
                }
                if (item.tienGiamGia) {
                    item.giamGia = item.giaBanLeVat - item.donGia;
                    item.tienGiamGia = item.giamGia * item.soLuong;
                }
                item.tienTruocGiamGia = item.soLuong * item.donGia;
                item.giaBanBuonVat = item.donGia / (1 + item.tyLeVatRa / 100);
                item.thanhTien = item.soLuong * item.donGia;
                //item. = $scope.newItem.soLuong*$scope.newItem.tyLeVatRa/100*($scope.newItem.donGia/(1 + $scope.newItem.tyLeVatRa/100));
            },
            changeGiamGia: function (item) {
                if (item) {
                    if (!item.soLuong) {
                        item.soLuong = 0;
                    }
                    if (!item.donGia) {
                        item.donGia = 0;
                    }
                    if (isNaN(item.giamGia) || item.giamGia < 0) {
                        item.giamGia = 0;
                    }
                    item.giaBanBuonVat = (item.giaBanLeVat - item.giamGia) / (1 + item.tyLeVatRa / 100);
                    item.tienGiamGia = item.giamGia * item.soLuong;
                    item.thanhTien = item.soLuong * item.donGia - item.tienGiamGia;
                }
                else {
                    item.thanhTien = item.soLuong * item.donGia;
                    console.log(item.thanhTien);
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
                return $http.get(rootUrl + '/api/Md/Customer/' + id);
            },
            postApproval: function (id) {
                return $http.post(serviceUrl + '/PostApproval', id);
            },
            update: function (params) {
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
            updateStatusPhieu: function (data) {
                return $http.post(serviceUrl + '/UpdateStatusPhieu', data);
            },
            postExportExcel: function (json) {
                $http({
                    url: serviceUrl + '/PostExportExcel',
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
                    a.download = "PhieuXuatBan.xlsx";
                    a.click();
                }).error(function (data, status, headers, config) {
                    //upload failed
                });
            },
            postExportExcelByMerchandise: function (json) {
                $http({
                    url: serviceUrl + '/PostExportExcelByMerchandise',
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
                    a.download = "PhieuXuatBanTheoHang.xlsx";
                    a.click();
                }).error(function (data, status, headers, config) {
                    //upload failed
                });
            },
            postExportExcelByMerchandiseType: function (json) {
                $http({
                    url: serviceUrl + '/PostExportExcelByMerchandiseType',
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
                    a.download = "PhieuXuatBanTheoLoaiHang.xlsx";
                    a.click();
                }).error(function (data, status, headers, config) {
                    //upload failed
                });
            },
            postExportExcelByMerchandiseGroup: function (json) {
                $http({
                    url: serviceUrl + '/PostExportExcelByMerchandiseGroup',
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
                    a.download = "PhieuXuatBanTheoNhomHang.xlsx";
                    a.click();
                }).error(function (data, status, headers, config) {
                    //upload failed
                });
            },
            postExportExcelByNhaCungCap: function (json) {
                $http({
                    url: serviceUrl + '/PostExportExcelByNhaCungCap',
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
                    a.download = "PhieuXuatBanTheoNCC.xlsx";
                    a.click();
                }).error(function (data, status, headers, config) {
                    //upload failed
                });
            },
            getDathangDetails: function (id) {
                return $http.get(serviceDatHangUrl + '/GetDetails/' + id);
            },
            getNewParameter: function () {
                return $http.get(serviceUrl + '/GetNewParameter');
            },
            getPhieuByMaPhieu: function (MaPhieu) {
                return $http.get(serviceUrl + '/CheckExist/' + MaPhieu);
            }
        };
        return result;
    }]);
    /* controller list */
    app.controller('phieuXuatBanController', [
        '$scope', 'configService', 'phieuXuatBanService', 'tempDataService', '$filter', '$uibModal', '$log', 'securityService', '$rootScope', 'toaster', 'periodService', 'merchandiseService', 'customerService', 'merchandiseTypeService', 'nhomVatTuService', 'supplierService', 'wareHouseService', 'packagingService', 'taxService', 'donViTinhService',
        function ($scope, configService, service, tempDataService, $filter, $uibModal, $log, securityService, $rootScope, toaster, servicePeriod, serviceMerchandise, serviceCustomer, serviceMerchandiseType, serviceNhomVatTu, serviceSupplier, serviceWareHouse, servicePackaging, serviceTax, serviceDonViTinh) {
            $scope.openClosingOut = false;
            //check có mở khóa sổ không
            function checkUnClosingOut() {
                servicePeriod.checkUnClosingOut().then(function (response) {
                    if (response && response.status == 200 && response.data && response.data.data) {
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
                    service.getNewParameter().then(function (response) {
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
                }
            };
            //end

            //check quyền truy cập
            function loadAccessList() {
                securityService.getAccessList('phieuXuatBan').then(function (successRes) {
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
            $scope.displayHepler = function (paraValue, moduleName) {
                var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                if (data && data.length === 1) {
                    return data[0].text;
                } else {
                    return paraValue;
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

            /* Function add New Item */
            $scope.create = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    size: 'md',
                    templateUrl: configService.buildUrl('nv/NvXuatBan', 'add'),
                    controller: 'phieuXuatBanCreateController',
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
                    templateUrl: configService.buildUrl('nv/NvXuatBan', 'update'),
                    controller: 'phieuXuatBanEditController',
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
                    templateUrl: configService.buildUrl('nv/NvXuatBan', 'details'),
                    controller: 'phieuXuatBanDetailsController',
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
                    templateUrl: configService.buildUrl('nv/NvXuatBan', 'printItem'),
                    controller: 'phieuXuatBanExportItemController',
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
                    templateUrl: configService.buildUrl('nv/NvXuatBan', 'delete'),
                    controller: 'phieuXuatBanDeleteController',
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

            $scope.loadItem = function (item) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('nv/NvXuatBan', 'infoThanhToan'),
                    controller: 'infoThanhToanController',
                    resolve: {
                        targetData: function () {
                            return item;
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
                $state.go("nvPrintDetailphieuXuatBan");
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
    app.controller('phieuXuatBanCreateController', ['$scope', '$uibModalInstance', 'configService', 'phieuXuatBanService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'userService', 'merchandiseService', 'customerService', 'merchandiseTypeService', 'nhomVatTuService', 'supplierService', 'wareHouseService', 'packagingService', 'taxService', 'donViTinhService', 'periodService', 'nvCongNoKhachHangService', 'toaster',
        function ($scope, $uibModalInstance, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, serviceAuthUser, serviceMerchandise, serviceCustomer, serviceMerchandiseType, serviceNhomVatTu, serviceSupplier, serviceWareHouse, servicePackaging, serviceTax, serviceDonViTinh, servicePeriod, nvCongNoKhachHangService, toaster) {
            var currentUser = serviceAuthUser.GetCurrentUser();
            var unitCode = currentUser.unitCode;
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
            $scope.changeDonGia = function (item) {
                if (item) {
                    if (!item.soLuong) {
                        item.soLuong = 0;
                    }
                    if (!item.giamGia) {
                        item.giamGia = 0;
                    }
                    if (item.tienGiamGia) {
                        item.giamGia = item.giaBanLeVat - item.donGia;
                        item.tienGiamGia = item.giamGia * item.soLuong;
                    }
                    item.tienTruocGiamGia = item.soLuong * item.donGia;
                    //item.giaBanBuonVat = item.donGia / (1 + item.tyLeVatRa / 100);
                    item.thanhTien = item.soLuong * item.donGia;
                }
            };
            $scope.isLoading = false;
            $scope.title = function () { return 'Phiếu xuất bán'; };
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
            //end 


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
                $scope.robot.changeDonGia($scope.newItem);
                if (!$scope.newItem.soLuong || $scope.newItem.soLuong < 1) {
                    focus('soluong');
                    document.getElementById('soluong').focus();
                    return;
                }
                //if (!$scope.newItem.soLuong || $scope.newItem.soLuong < 1 || $scope.newItem.soLuong > $scope.newItem.soLuongTon) {
                //    focus('soluong');
                //    document.getElementById('soluong').focus();
                //    toaster.pop({
                //        type: 'error',
                //        title: 'Lỗi:',
                //        body: 'Nhập sai số lượng !',
                //        timeout: 1000
                //    });
                //    return;
                //}
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
                                $scope.target.dataDetails[k].thanhTien = $scope.newItem.soLuong * $scope.target.dataDetails[k].giaBanBuonVat;
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
            $scope.selectedMaHang = function (code) {
                if (code) {
                    service.getMerchandiseForNvByCode(code, $scope.target.maKhoXuat, unitCode).then(function (response) {
                        console.log(response);
                        if (response && response.status === 200 && response.data && response.data.status) {
                            $scope.newItem = response.data.data;
                            //giá bán có vat
                            $scope.newItem.donGia = $scope.newItem.giaBanBuonVat;
                            $scope.newItem.giaBanLe = response.data.data.giaBanLe;
                            $scope.newItem.validateCode = response.data.data.maHang;
                            $scope.newItem.giaMuaCoVat = response.data.data.giaMua * (1 + response.data.data.tyLeVatVao / 100);
                            $scope.newItem.soLuongTon = response.data.data.soLuongTon;
                        }
                        else {
                            $scope.addNewItem(code);
                        }
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
                        service.getMerchandiseForNvByCode(updatedData.maHang, $scope.target.maKhoXuat, unitCode).then(function (response) {
                            if (response && response.status === 200 && response.data && response.data.status) {
                                $scope.newItem.giaVonVat = response.data.data.giaVonVat;
                                $scope.newItem.soLuongTon = response.data.data.soLuongTon;
                            }
                        });
                        updatedData.donGia = updatedData.giaBanBuonVat;
                        $scope.newItem.validateCode = updatedData.maHang;
                        $scope.newItem.giaBanLe = updatedData.giaBanLe;
                        $scope.newItem.giaMuaCoVat = updatedData.giaMua * (1 + updatedData.tyLeVatVao / 100);
                    }
                    $scope.pageChanged();
                }, function () {

                });
            };
            $scope.removeItem = function (index) {
                var currentPage = $scope.paged.currentPage;
                var itemsPerPage = 10;
                var currentPageIndex = (currentPage - 1) * itemsPerPage + index;
                $scope.target.dataDetails.splice(currentPageIndex, 1);
                if ($scope.target.dataDetails.length == 0) {
                    $scope.isListItemNull = true;
                }
                $scope.pageChanged();
            };
            //end vudq import excel
            $scope.selectedkhachHang = function (item) {
                service.getCustomer(item.id).then(function (response) {
                    if (response && response.data) {
                        $scope.target.maSoThue = response.data.maSoThue;
                        $scope.target.diaChiKhachHang = response.data.diaChi;
                    }
                });
                nvCongNoKhachHangService.getAmmountCustomerBorrowed(item.value).then(function (response) {
                    if (response.status === 200) {
                        $scope.target.tienNoCu = response.data.thanhTienCanTra;
                        $scope.target.noiDung = "Nợ cũ " + Math.round(response.data.thanhTienCanTra / 1000) + "k";
                    }
                });
            };
            $scope.selectedTkCo = function (item) {
                $scope.target.tkCo = item.value;
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
            $scope.save = function () {

                if ($scope.target.trangThaiThanhToan && $scope.target.trangThaiThanhToan === 10) {
                    $scope.target.tienThanhToan = $scope.target.thanhTienSauVat;
                }
                else {
                    $scope.target.tienThanhToan = 0;
                }
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
            $scope.saveAndKeep = function () {
                var tempData = angular.copy($scope.target);
                service.post($scope.target).then(function (successRes) {
                    if (successRes && successRes.status === 201 && successRes.data) {
                        ngNotify.set(successRes.data.message, { type: 'success' });
                        $scope.target.dataDetails = [];
                        $scope.isListItemNull = true;
                        service.getNewInstance().then(function (response) {
                            var expectData = response;
                            tempData.maChungTu = expectData.maChungTu;
                            tempData.ngay = expectData.ngay;
                            $scope.target = tempData;
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
                        var url = $state.href('reportphieuXuatBan', { id: response.data.id });
                        window.open(url, 'Report Viewer');
                        $scope.target.dataDetails = []
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
                $scope.isListItemNull = true;
                $uibModalInstance.dismiss('cancel');
            };
        }]);
    /* controller Edit */
    app.controller('phieuXuatBanEditController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'phieuXuatBanService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify', 'merchandiseService', 'toaster', 'nvCongNoKhachHangService', 'userService',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify, serviceMerchandise, toaster, nvCongNoKhachHangService, serviceAuthUser) {
            var currentUser = serviceAuthUser.GetCurrentUser();
            var unitCode = currentUser.unitCode;
            $scope.config = angular.copy(configService);
            $scope.robot = angular.copy(service.robot);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.targetData = angular.copy(targetData);
            $scope.tempData = tempDataService.tempData;

            $scope.target = targetData;
            $scope.newItem = {};
            $scope.tkKtKhoNhap = "";
            $scope.tyGia = 0;
            $scope.changeDonGia = function (item) {
                if (item) {
                    if (!item.soLuong) {
                        item.soLuong = 0;
                    }
                    if (!item.giamGia) {
                        item.giamGia = 0;
                    }
                    if (item.tienGiamGia) {
                        item.giamGia = item.giaBanLeVat - item.donGia;
                        item.tienGiamGia = item.giamGia * item.soLuong;
                    }
                    item.tienTruocGiamGia = item.soLuong * item.donGia;
                    //item.giaBanBuonVat = item.donGia / (1 + item.tyLeVatRa / 100);
                    item.thanhTien = item.soLuong * item.donGia;
                }
            };
            $scope.isLoading = true;
            $scope.title = function () { return 'Cập nhật phiếu xuất bán'; };
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
            };
            function filterData() {
                service.getDetails($scope.target.id).then(function (response) {
                    if (response && response.status == 200 && response.data) {
                        $scope.target = response.data;
                        $scope.target.ngayCT = new Date($scope.target.ngayCT);
                        $scope.target.dataDetails.forEach(function (obj) {
                            obj.giaVonVat = obj.giaVon * (1 + obj.tyLeVatVao / 100);
                        });
                        $scope.target.ngayHoaDon = new Date($scope.target.ngayHoaDon);
                        if ($scope.target.dataDetails.length > 0) {
                            $scope.isListItemNull = false;
                        }
                        serviceMerchandise.setSelectData($scope.target.dataDetails);
                        $scope.pageChanged();
                        if ($scope.target.vat) {
                            var z = $filter('filter')($scope.tempData('taxs'), { value: $scope.target.vat }, true);
                            $scope.tyGia = z[0].extendValue;
                        } else {
                            $scope.tyGia = 0;
                        }

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
                $scope.robot.changeDonGia($scope.newItem);
                if (!$scope.newItem.soLuong || $scope.newItem.soLuong < 1) {
                    focus('soluong');
                    document.getElementById('soluong').focus();
                    return;
                }
                //if (!$scope.newItem.soLuong || $scope.newItem.soLuong < 1 || $scope.newItem.soLuong > $scope.newItem.soLuongTon) {
                //    focus('soluong');
                //    document.getElementById('soluong').focus();
                //    toaster.pop({
                //        type: 'error',
                //        title: 'Lỗi:',
                //        body: 'Nhập sai số lượng !',
                //        timeout: 1000
                //    });
                //    return;
                //}
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
                                $scope.target.dataDetails[k].thanhTien = $scope.newItem.soLuong * $scope.target.dataDetails[k].giaBanBuonVat;
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
                        service.getMerchandiseForNvByCode(updatedData.maHang, $scope.target.maKhoXuat, unitCode).then(function (response) {
                            if (response && response.status === 200 && response.data && response.data.status) {
                                $scope.newItem.giaVonVat = response.data.data.giaVonVat;
                                $scope.newItem.soLuongTon = response.data.data.soLuongTon;
                            }
                        });
                        updatedData.donGia = updatedData.giaBanBuonVat;
                        $scope.newItem.validateCode = updatedData.maHang;
                        $scope.newItem.giaBanLe = updatedData.giaBanLe;
                        $scope.newItem.giaMuaCoVat = updatedData.giaMua * (1 + updatedData.tyLeVatVao / 100);
                    }
                    $scope.pageChanged();
                }, function () {
                });
            }

            $scope.removeItem = function (index) {
                var currentPage = $scope.paged.currentPage;
                var itemsPerPage = 10;
                var currentPageIndex = (currentPage - 1) * itemsPerPage + index;
                $scope.target.dataDetails.splice(currentPageIndex, 1);
                $scope.pageChanged();
            };
            $scope.selectedkhachHang = function (item) {

                service.getCustomer(item.id).then(function (response) {
                    if (response && response.data) {
                        $scope.target.maSoThue = response.data.maSoThue;
                        $scope.target.diaChiKhachHang = response.data.diaChi;
                    }
                });
                nvCongNoKhachHangService.getAmmountCustomerBorrowed(item.value).then(function (response) {
                    if (response.status === 200) {
                        $scope.target.tienNoCu = response.data.thanhTienCanTra;
                        $scope.target.noiDung = "Nợ cũ " + Math.round(response.data.thanhTienCanTra / 1000) + "k";
                    }
                });
            };
            $scope.selectedTkCo = function (item) {
                $scope.target.tkCo = item.value;
            };
            $scope.selectedMaHang = function (code) {
                if (code) {
                    service.getMerchandiseForNvByCode(code, $scope.target.maKhoXuat, unitCode).then(function (response) {
                        if (response && response.status === 200 && response.data && response.data.status) {
                            $scope.newItem = response.data.data;
                            $scope.newItem.donGia = $scope.newItem.giaMua;
                            $scope.newItem.validateCode = response.data.data.maHang;
                            $scope.newItem.giaMuaCoVat = response.data.data.giaMua * (1 + response.data.data.tyLeVatVao / 100);
                            $scope.newItem.soLuongTon = response.data.data.soLuongTon;
                        }
                        else {
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

            $scope.save = function () {
                if ($scope.target.trangThaiThanhToan && $scope.target.trangThaiThanhToan === 10) {
                    $scope.target.tienThanhToan = $scope.target.thanhTienSauVat;
                }
                else {
                    $scope.target.tienThanhToan = 0;
                }
                //index để sắp xếp theo mã hàng lúc sửa
                if ($scope.target.dataDetails.length > 0) {
                    angular.forEach($scope.target.dataDetails, function (value, index) {
                        value.index = index;
                    });
                }
                service.update($scope.target).then(function (successRes) {
                    if (successRes && successRes.status === 200 && successRes.data) {
                        ngNotify.set("Cập nhật thành công", { type: 'success' });
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
                        var url = $state.href('reportphieuXuatBan', { id: successRes.data.data.id });
                        window.open(url, 'Report Viewer');
                        $scope.target.dataDetails = [];
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
                $scope.target.dataDetails = []
                $uibModalInstance.close();
            };

        }]);


    /* controller Details */
    app.controller('phieuXuatBanDetailsController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'phieuXuatBanService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify) {
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.targetData = angular.copy(targetData);
            $scope.tempData = tempDataService.tempData;
            $scope.target = targetData;
            $scope.isLoading = false;
            $scope.title = function () { return 'Thông tin phiếu xuất bán'; };
            $scope.pageChanged = function () {
                var currentPage = $scope.paged.currentPage;
                var itemsPerPage = 10;
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
                    console.log(resgetDetails);
                    if (resgetDetails && resgetDetails.status == 200 && resgetDetails.data) {
                        $scope.target = resgetDetails.data;
                        $scope.target.dataDetails.forEach(function (obj) {
                            obj.giaVonVat = obj.giaVon * (1 + obj.tyLeVatVao / 100);
                        });
                        $scope.target.ngayCT = new Date($scope.target.ngayCT);
                        $scope.target.ngayHoaDon = new Date($scope.target.ngayHoaDon);
                    }
                    $scope.isLoading = false;
                    $scope.pageChanged();
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
            fillterData();
            $scope.approval = function () {
                service.postApproval($scope.target).then(function (response) {
                    if (response && response.status == 200 && response.data) {
                        alert("Duyệt thành công!");
                        $uibModalInstance.close($scope.target);
                        $scope.goIndex = function () {
                            $state.go('NvXuatBan');
                        };
                    }
                    else { alert("Thất bại! - Xảy ra lỗi hoặc phiếu này đã duyệt"); }
                });
            };
            $scope.cancel = function () {
                $uibModalInstance.close();
            };

        }]);

    /* controller info */
    app.controller('infoThanhToanController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'phieuXuatBanService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify', 'userService',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify, serviceAuthUser) {
            $scope.config = angular.copy(configService);
            var currentUser = serviceAuthUser.GetCurrentUser();
            var unitCode = currentUser.unitCode;
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.tempData = tempDataService.tempData;
            $scope.item = targetData;
            $scope.target = {};
            $scope.target = {
                dataDto: {},
                dataDetails: []
            };
            $scope.target.tempTien = 0;
            $scope.undisabled = true;

            $scope.isLoading = false;
            $scope.title = function () { return 'Thông tin thanh toán'; };
            if ($scope.item.id) {
                var allTienKhuyenMai = 0;
                service.getDetails($scope.item.id).then(function (response) {
                    if (response && response.status === 200 && response.data) {
                        $scope.target.dataDto.id = response.data.id;
                        $scope.target.dataDto.noiDung = response.data.noiDung;
                        $scope.target.dataDto.unitCode = currentUser.unitCode;
                        $scope.target.dataDto.maGiaoDich = response.data.maChungTu;
                        $scope.target.dataDto.ngayChungTu = response.data.ngayCT;
                        $scope.target.dataDto.maNhanVien = response.data.maNhanVien;
                        $scope.target.dataDto.maVoucher = response.data.maVoucher;
                        $scope.target.dataDto.makh = response.data.maKhachHang;
                        $scope.target.dataDto.maThe = response.data.maThe;
                        $scope.target.dataDto.ghiChu = response.data.ghiChu;
                        $scope.target.dataDto.allSumTienHang = response.data.thanhTienSauVat;
                        $scope.target.dataDto.allSumTienKhuyenMai = response.data.tienChietKhau;
                        $scope.target.dataDto.tienVoucher = response.data.tienVoucher === null ? 0 : response.data.tienVoucher;
                        $scope.target.dataDto.tienThua = 0;
                        $scope.target.dataDto.trangThaiThanhToan = response.data.trangThaiThanhToan;
                        $scope.target.dataDto.tienKhachDua = response.data.tienKhachDua;
                        angular.forEach(response.data.dataDetails, function (value, index) {
                            value.maVatTu = value.maHang;
                            value.tenVatTu = value.tenHang;
                            value.soLuong = value.soLuong;
                            value.giaBanLeVat = value.donGia;
                            value.tyLeChietKhau = value.tyLeChietKhau;
                            value.tienDuocKhuyenMai = value.giamGia;
                            value.tTienCoVat = value.thanhTien;
                            allTienKhuyenMai = allTienKhuyenMai + value.giamGia;
                            $scope.target.dataDetails.push(value);
                        });
                        $scope.target.dataDto.tongTienKhuyenMai = parseInt(allTienKhuyenMai, 10);
                        $scope.target.dataDto.allSumTienHang = parseInt($scope.target.dataDto.allSumTienHang, 10);
                        $scope.target.tempTien = angular.copy($scope.target.dataDto.allSumTienHang);
                        $scope.target.dataDto.tongTien = parseInt(allTienKhuyenMai, 10) + parseInt($scope.target.dataDto.allSumTienHang, 0);
                        $scope.target.dataDto.tienThe = response.data.tienThe === null ? 0 : response.data.tienThe;
                        $scope.target.dataDto.tienCOD = response.data.tienCOD === null ? 0 : response.data.tienCOD;
                        $scope.target.dataDto.tienMat = response.data.tienMat === null ? 0 : response.data.tienMat;
                    }
                });
            }

            $scope.enterTienThe = function (item) {
                if (item) {
                    $scope.target.dataDto.tienThe = parseInt(item, 10);
                    $scope.target.dataDto.tienThua = ($scope.target.dataDto.tienThe + $scope.target.dataDto.tienCOD + $scope.target.dataDto.tienMat) - $scope.target.tempTien;
                    $scope.undisabled = false;
                }
            };
            $scope.enterTienCOD = function (item) {
                $scope.target.dataDto.tienCOD = parseInt(item, 10);
                $scope.target.dataDto.tienThua = ($scope.target.dataDto.tienThe + $scope.target.dataDto.tienCOD + $scope.target.dataDto.tienMat) - $scope.target.tempTien;
                $scope.undisabled = false;
            };
            $scope.enterTienMat = function (item) {
                $scope.target.dataDto.tienMat = parseInt(item, 10);
                $scope.target.dataDto.tienThua = ($scope.target.dataDto.tienThe + $scope.target.dataDto.tienCOD + $scope.target.dataDto.tienMat) - $scope.target.tempTien;
                $scope.undisabled = false;
            };
            $scope.displayHepler = function (paraValue, moduleName) {
                var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                if (data && data.length === 1) {
                    return data[0].text;
                } else {
                    return paraValue;
                }
            }
            $scope.printDiv = function (divName) {
                //update trạng thái phiếu
                if ($scope.target.dataDto) {
                    service.updateStatusPhieu($scope.target.dataDto).then(function (response) {
                        if (response && response.status === 200 && response.data) {
                            ngNotify.set(response.data.message, { duration: 1000, type: 'success' });
                            $uibModalInstance.close();

                        } else {
                            ngNotify.set(response.data.message, { duration: 3000, type: 'error' });
                        }
                    });
                }
                //print bill
                var printContents = document.getElementById(divName).innerHTML;
                var popupWin;
                if (navigator.userAgent.toLowerCase().indexOf('chrome') > -1) {

                    popupWin = window.open('', '_blank', 'width=600,height=600,scrollbars=no,menubar=no,toolbar=no,location=no,status=no,titlebar=no');
                    popupWin.window.focus();
                    popupWin.document.write('<!DOCTYPE html><html><head>' +
                        '<link rel="stylesheet" type="text/css" />' +
                        '</head><body onload="window.print()"><div class="reward-body">' + printContents + '</div></html>');
                    popupWin.document.close();
                    popupWin.close();
                    popupWin.onbeforeunload = function (event) {
                        popupWin.close();
                        return '.\n';
                    };
                    popupWin.onabort = function (event) {
                        popupWin.document.close();
                        popupWin.close();
                    }
                } else {
                    popupWin = window.open('', '_blank', 'width=800,height=600');
                    popupWin.document.open();
                    popupWin.document.write('<html><head><link rel="stylesheet" type="text/css" href="style.css" /></head><body onload="window.print()">' + printContents + '</html>');
                    popupWin.document.close();
                }
                popupWin.document.close();
                return true;
            };
            $uibModalInstance.close();
            $scope.cancel = function () {
                $uibModalInstance.close();
            };

        }]);
    /* controller delete */
    app.controller('phieuXuatBanDeleteController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'phieuXuatBanService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify) {
            $scope.config = angular.copy(configService);
            $scope.targetData = angular.copy(targetData);
            $scope.target = targetData;
            $scope.isLoading = false;
            $scope.title = function () { return 'Xoá thành phần'; };
            $scope.save = function () {
                service.deleteItem($scope.target).then(function (successRes) {
                    if (successRes && successRes.status === 200) {
                        ngNotify.set("Xóa thành công", { type: 'success' });
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

    /* print Phieu Nhap Hang Mua Controller */
    app.controller('printphieuXuatBanController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'phieuXuatBanService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify) {
            $scope.robot = angular.copy(service.robot);
            $scope.config = angular.copy(configService);
            $scope.targetData = angular.copy(targetData);
            $scope.target = targetData;
            var id = $stateParams.id;
            $scope.target = {};
            $scope.info = service.getParameterPrint().filtered.advanceData;
            $scope.goIndex = function () {
                $state.go('NvXuatBan');
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
    app.controller('printDetailphieuXuatBanController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'phieuXuatBanService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify) {
            $scope.robot = angular.copy(service.robot);
            $scope.config = angular.copy(configService);
            $scope.targetData = angular.copy(targetData);
            $scope.target = targetData;
            var id = $stateParams.id;
            $scope.target = {};
            $scope.info = service.getParameterPrint().filtered.advanceData;
            $scope.goIndex = function () {
                $state.go('NvXuatBan');
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

    /* report Phieu Xuất bán Controller */
    app.controller('reportPhieuXuatBanController', ['$scope', '$location', '$http', 'configService', 'phieuXuatBanService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'userService', '$stateParams', '$window',
        function ($scope, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, serviceAuthUser, $stateParams, $window) {
            var currentUser = serviceAuthUser.GetCurrentUser();
            $scope.robot = angular.copy(service.robot);
            $scope.tempData = tempDataService.tempData;
            var id = $stateParams.id;
            $scope.target = {};
            $scope.goIndex = function () {
                $state.go('nvXuatBan');
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
                    return data[0].description;
                } else {
                    return paraValue;
                }
            }

            $scope.print = function () {
                var table = document.getElementById('main-report').innerHTML;
                var myWindow = $window.open('', '', 'width=800, height=600');
                myWindow.document.write(table);
                myWindow.print();
            }
            $scope.printExcel = function () {
                var data = [document.getElementById('main-report').innerHTML];
                var fileName = "XuatBan_ExportData.xls";
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
    /* report Phieu Xuất bán Controller */
    app.controller('reportPhieuXuatBanController_onlySL', ['$scope', '$location', '$http', 'configService', 'phieuXuatBanService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'userService', '$stateParams', '$window',
        function ($scope, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, serviceAuthUser, $stateParams, $window) {
            var currentUser = serviceAuthUser.GetCurrentUser();
            $scope.robot = angular.copy(service.robot);
            var id = $stateParams.id;
            $scope.target = {};
            $scope.goIndex = function () {
                $state.go('nvXuatBan');
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

            $scope.print = function () {
                var table = document.getElementById('main-report').innerHTML;
                var myWindow = $window.open('', '', 'width=800, height=600');
                myWindow.document.write(table);
                myWindow.print();
            }
            $scope.printExcel = function () {
                var data = [document.getElementById('main-report').innerHTML];
                var fileName = "XuatBan_ExportData.xls";
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
    /* controller export Item bo hang */
    app.controller('phieuXuatBanExportItemController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'phieuXuatBanService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify',
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
                $state.go('NvXuatBan');
            };
            $scope.cancel = function () {
                $uibModalInstance.close();
            };
        }]);

    app.controller('phieuXuatBanSelectedController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'phieuXuatBanService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'serviceSelectData', 'periodService', 'merchandiseService', 'customerService', 'merchandiseTypeService', 'nhomVatTuService', 'supplierService', 'wareHouseService', 'packagingService', 'taxService', 'donViTinhService', 'filterObject',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, serviceSelectData, servicePeriod, serviceMerchandise, serviceCustomer, serviceMerchandiseType, serviceNhomVatTu, serviceSupplier, serviceWareHouse, servicePackaging, serviceTax, serviceDonViTinh, filterObject) {
            $scope.robot = angular.copy(service.robot);
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.filtered = angular.copy(configService.filterDefault);
            $scope.filtered.summary = filterObject.summary;
            $scope.isLoading = false;
            $scope.lstMerchandise = [];
            $scope.dataHangHoa = {};
            $scope.title = function () { return 'Danh sách phiếu xuất bán'; };
            function filterData() {
                $scope.isLoading = true;
                var postdata = {};
                $scope.isLoading = true;
                postdata = { paged: $scope.paged, filtered: $scope.filtered };
                service.postQuery(postdata).then(function (successRes) {
                    console.log(successRes);
                    if (successRes && successRes.status === 200 && successRes.data && successRes.data.status) {
                        $scope.isLoading = false;
                        $scope.data = successRes.data.data.data;
                        angular.extend($scope.paged, successRes.data.data);
                    }
                });
            };

            $scope.selectedRow = function (item) {
                $uibModalInstance.close(item.id);
            }
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
            filterData();
            $scope.goIndex = function () {
                $state.go('NvXuatBan');
            };
            $scope.cancel = function () {
                $uibModalInstance.close();
            };
        }]);
    return app;
});