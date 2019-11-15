/*  
* Người tạo : Nguyễn Tuấn Hoàng Anh
* View: BTS.SP.MART/views/nv/NvXuatKhac
* Vm sevices: BTS.API.SERVICE -> NV ->NvXuatKhacVm.cs
* Sevices: BTS.API.SERVICE -> NV -> NvXuatKhacService.cs
* Entity: BTS.API.ENTITY -> NV - > NvXuatKhac.cs
* Menu: Nghiệp vụ-> Xuất khác
*/
define(['ui-bootstrap', '/BTS.SP.MART/controllers/auth/AuthController.js', '/BTS.SP.MART/controllers/htdm/periodController.js', '/BTS.SP.MART/controllers/htdm/merchandiseController.js', '/BTS.SP.MART/controllers/htdm/customerController.js', '/BTS.SP.MART/controllers/htdm/merchandiseTypeController.js', '/BTS.SP.MART/controllers/htdm/nhomVatTuController.js', '/BTS.SP.MART/controllers/htdm/supplierController.js', '/BTS.SP.MART/controllers/htdm/wareHouseController.js', '/BTS.SP.MART/controllers/htdm/packagingController.js', '/BTS.SP.MART/controllers/htdm/taxController.js', '/BTS.SP.MART/controllers/htdm/donViTinhController.js', '/BTS.SP.MART/controllers/htdm/typeReasonController.js'], function () {
    'use strict';
    var app = angular.module('phieuXuatKhacModule', ['ui.bootstrap', 'authModule', 'periodModule', 'merchandiseModule', 'customerModule', 'merchandiseTypeModule', 'nhomVatTuModule', 'supplierModule', 'wareHouseModule', 'packagingModule', 'taxModule', 'donViTinhModule', 'typeReasonModule']);
    app.factory('phieuXuatKhacService', ['$http', 'configService', function ($http, configService) {
        var serviceUrl = configService.rootUrlWebApi + '/NV/XuatKhac';
        var rootUrl = configService.apiServiceBaseUri;
        var maLyDo = '';
        this.parameterPrint = {};
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
            changeGiamGia: function (item) {
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
                return $http.get(rootUrl + '/api/Nv/XuatKhac/GetMerchandise/' + maChungTuPk);
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
                    a.download = "PhieuNhapMua.xlsx";
                    a.click();
                }).error(function (data, status, headers, config) {
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
                    a.download = "PhieuNhapTheoHang.xlsx";
                    a.click();
                }).error(function (data, status, headers, config) {
                });
            },
            postExportExcelByMerchandiseType: function (json) {
                $http({
                    url: serviceUrl + '/PostExportExcelByMerchandiseType',
                    method: "POST",
                    data: json,
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
                    a.download = "PhieuNhapTheoLoaiHang.xlsx";
                    a.click();
                }).error(function (data, status, headers, config) {
                });
            },
            postExportExcelByMerchandiseGroup: function (json) {
                $http({
                    url: serviceUrl + '/PostExportExcelByMerchandiseGroup',
                    method: "POST",
                    data: json, 
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
                    a.download = "PhieuNhapTheoNhomHang.xlsx";
                    a.click();
                }).error(function (data, status, headers, config) {
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
                    a.download = "PhieuNhapTheoNCC.xlsx";
                    a.click();
                }).error(function (data, status, headers, config) {
                });
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
            postExportExcelDetail: function (json, filename) {
                $http({
                    url: serviceUrl + '/PostExportExcelDetail',
                    method: "POST",
                    data: json, 
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
                });
            },
            getByCodeWithGiaVon: function (code, wareHouseCode, maDonVi) {
                return $http.get(rootUrl + '/api/Md/Merchandise/GetByCodeWithGiaVon/' + code + '/' + wareHouseCode + '/' + maDonVi);
            }
        };
        return result;
    }]);
    /* controller list */
    app.controller('phieuXuatKhacController', [
        '$scope', '$location', '$http', 'configService', 'phieuXuatKhacService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'securityService', '$rootScope', 'toaster', 'periodService', 'merchandiseService', 'customerService', 'merchandiseTypeService', 'nhomVatTuService', 'supplierService', 'wareHouseService', 'packagingService', 'taxService', 'donViTinhService', 'typeReasonService',
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
                securityService.getAccessList('phieuXuatKhac').then(function (successRes) {
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
            function loadTypeReason() {
                if (!tempDataService.tempData('typeReasons')) {
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
                } else {
                    $scope.typeReasons = tempDataService.tempData('typeReasons');
                }
            };
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
            };
            $scope.reLoadTypeReason = function () {
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
            };
            loadTypeReason();
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
            $scope.sortType = 'maNvXuatKhac';
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
            $scope.title = function () { return 'Phiếu xuất khác'; };

            /* Function add New Item */
            $scope.create = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    size: 'md',
                    templateUrl: configService.buildUrl('nv/NvXuatKhac', 'add'),
                    controller: 'phieuXuatKhacCreateController',
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
                    templateUrl: configService.buildUrl('nv/NvXuatKhac', 'update'),
                    controller: 'phieuXuatKhacEditController',
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
                    templateUrl: configService.buildUrl('nv/NvXuatKhac', 'details'),
                    controller: 'phieuXuatKhacDetailsController',
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
                    templateUrl: configService.buildUrl('nv/NvXuatKhac', 'printItem'),
                    controller: 'phieuXuatKhacExportItemController',
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
                    templateUrl: configService.buildUrl('nv/NvXuatKhac', 'delete'),
                    controller: 'phieuXuatKhacDeleteController',
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
                                maKhoXuat: item.maKhoXuat
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
                service.setParameterPrint(postdata);
                $state.go("nvPrintPhieuNhapMua");
            };
            $scope.printDetail = function () {
                var postdata = { paged: $scope.paged, filtered: $scope.filtered };
                service.setParameterPrint(postdata);
                $state.go("nvPrintDetailPhieuXuatKhac");
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
    app.controller('phieuXuatKhacCreateController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'phieuXuatKhacService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', '$rootScope', 'userService', 'FileUploader', 'merchandiseService', 'customerService', 'merchandiseTypeService', 'nhomVatTuService', 'supplierService', 'wareHouseService', 'packagingService', 'taxService', 'donViTinhService', 'periodService', 'toaster', 'typeReasonService',
    function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, $rootScope, serviceAuthUser, FileUploader, serviceMerchandise, serviceCustomer, serviceMerchandiseType, serviceNhomVatTu, serviceSupplier, serviceWareHouse, servicePackaging, serviceTax, serviceDonViTinh, servicePeriod, toaster, serviceTypeReason) {
        var currentUser = serviceAuthUser.GetCurrentUser();
        var unitCode = currentUser.unitCode;
        var rootUrl = configService.apiServiceBaseUri;
        var serviceUrl = rootUrl + '/api/Nv/XuatKhac';
        $scope.robot = angular.copy(service.robot);
        $scope.config = angular.copy(configService);
        $scope.paged = angular.copy(configService.pageDefault);
        $scope.target = { dataDetails: [] };
        $scope.data = [];
        $scope.newItem = {};
        $scope.donHangs = [];
        $scope.target = { dataDetails: [], dataClauseDetails: [] };
        $scope.tkKtKhoXuat = "";
        $scope.target.tienChietKhau = 0;
        $scope.target.tongTienGiamGia = 0;
        $scope.target.thanhTienTruocVat = 0;
        $scope.target.thanhTienTruocVatSauCK = 0;
        $scope.target.tienVat = 0;
        $scope.target.thanhTienSauVat = 0;
        $scope.tyGia = 0;
        $scope.isListItemNull = true;
        $scope.tempData = tempDataService.tempData;
        $scope.putTempData = tempDataService.putTempData;

        $scope.isLoading = false;
        $scope.title = function () { return 'Phiếu xuất khác'; };
        $scope.displayHepler = function (paraValue, moduleName) {
            var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
            if (data && data.length === 1) {
                return data[0].text;
            } else {
                return paraValue;
            }
        }
        //load danh muc
        function loadTypeReason() {
            if (!tempDataService.tempData('typeReasons')) {
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
            } else {
                $scope.typeReasons = tempDataService.tempData('typeReasons');
            }
        }
        $scope.reLoadTypeReason = function () {
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
        };
        loadTypeReason();

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
        };
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
        };

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
            if (!$scope.newItem.soLuong || $scope.newItem.soLuong < 1 || $scope.newItem.soLuong > $scope.newItem.soLuongTon) {
                focus('soluong');
                document.getElementById('soluong').focus();
                toaster.pop({
                    type: 'error',
                    title: 'Lỗi:',
                    body: 'Nhập sai số lượng !',
                    timeout: 1000
                });
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
                    service.getByCodeWithGiaVon(updatedData.maHang, $scope.target.maKhoXuat, unitCode).then(function (response) {
                        if (response && response.status === 200 && response.data.status) {
                            $scope.newItem.donGia = response.data.data.giaVon;
                            $scope.newItem.soLuongTon = response.data.data.soLuongTon;
                            updatedData.donGia = response.data.data.giaVon;
                            $scope.newItem.giaBanLe = updatedData.giaBanLe;
                            $scope.newItem.giaMuaCoVat = response.data.data.giaVon * (1 + updatedData.tyLeVatVao / 100);
                            $scope.newItem.validateCode = updatedData.maHang;
                        } else {
                            updatedData.donGia = updatedData.giaMua;
                            $scope.newItem.validateCode = updatedData.maHang;
                            $scope.newItem.giaBanLe = updatedData.giaBanLe;
                            $scope.newItem.giaMuaCoVat = updatedData.giaMua * (1 + updatedData.tyLeVatVao / 100);
                            $scope.newItem.soLuongTon = response.data.data.soLuongTon;
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
                service.getMerchandiseForNvByCode(code, $scope.target.maKhoXuat,unitCode).then(function (response) {
                    if (response && response.status == 200 && response.data && response.data.status) {
                        $scope.newItem = response.data.data;
                        $scope.newItem.soLuongTon = response.data.data.soLuongTon;
                        $scope.newItem.donGia = response.data.data.giaVon;
                        $scope.newItem.vat = response.data.data.maVatVao;
                        $scope.newItem.validateCode = response.data.data.maHang;
                        $scope.newItem.giaMuaCoVat = response.data.data.giaVon * (1 + response.data.data.tyLeVatVao / 100);
                        console.log($scope.newItem);
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
        $scope.selectedKhoXuat = function (item) {
            $scope.target.maKhoXuat = item.value;
            service.getWareHouse(item.id).then(function (response) {
                $scope.tkKtKhoXuat = response.taiKhoanKt;
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
                $scope.tempData.putTempData('merchandises', updatedData);
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
                $scope.tempData.putTempData('packagings', updatedData);
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
                $scope.tempData.putTempData('customers', updatedData);
            }, function () {
                $log.info('Modal dismissed at: ' + new Date());
            });
        };
        $scope.createSupplier = function (target, name) {
            var modalInstance = $uibModal.open({
                backdrop: 'static',
                size: 'lg',
                templateUrl: configService.buildUrl('htdm/Supplier', 'add'),
                controller: 'supplierCreateController',
                resolve: {}
            });
            modalInstance.result.then(function (updatedData) {
                serviceSupplier.getAll_Supplier().then(function (successRes) {
                    if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                        tempDataService.putTempData('suppliers', successRes.data.data);
                        $scope.suppliers = successRes.data.data;
                        target[name] = updatedData.maKhachHang;
                    } else {
                        console.log('successRes', successRes);
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
    app.controller('phieuXuatKhacEditController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'phieuXuatKhacService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify', 'merchandiseService', 'toaster', 'taxService',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify, serviceMerchandise, toaster, serviceTax) {
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
            $scope.tkKtKhoXuat = "";
            $scope.tyGia = 0;

            $scope.isLoading = true;
            $scope.title = function () { return 'Cập nhật phiếu xuất khác'; };
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
                            $scope.target.dataDetails.forEach(function (obj) {
                                obj.thanhTienVAT = obj.soLuong * obj.giaMuaCoVat;
                            });
                        }
                        serviceMerchandise.setSelectData($scope.target.dataDetails);
                        $scope.pageChanged();
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
            function loadTypeReason() {
                if (!tempDataService.tempData('typeReasons')) {
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
                } else {
                    $scope.typeReasons = tempDataService.tempData('typeReasons');
                }
            }
            $scope.reLoadTypeReason = function () {
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
            };
            loadTypeReason();

            $scope.addRow = function () {
                if (!$scope.newItem.soLuong || $scope.newItem.soLuong < 1) {
                    focus('soluong');
                    document.getElementById('soluong').focus();
                    return;
                }
                if (!$scope.newItem.soLuong || $scope.newItem.soLuong < 1 || $scope.newItem.soLuong > $scope.newItem.soLuongTon) {
                    focus('soluong');
                    document.getElementById('soluong').focus();
                    toaster.pop({
                        type: 'error',
                        title: 'Lỗi:',
                        body: 'Nhập sai số lượng !',
                        timeout: 1000
                    });
                    return;
                }
                if ($scope.newItem.validateCode === $scope.newItem.maHang) {
                    var exsist = $scope.target.dataDetails.some(function (element, index, array) {
                        return $scope.newItem.maHang === element.maHang;
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
                    $scope.target.dataDetails = serviceMerchandise.getSelectData();
                    //-------------------------
                    service.getByCodeWithGiaVon(updatedData.maHang, $scope.target.maKhoXuat, unitCode).then(function (response) {
                        if (response && response.status === 200 && response.data.status) {
                            $scope.newItem.donGia = response.data.data.giaVon;
                            updatedData.donGia = response.data.data.giaVon;
                            $scope.newItem.giaBanLe = updatedData.giaBanLe;
                            $scope.newItem.giaMuaCoVat = response.data.data.giaVon * (1 + updatedData.tyLeVatVao / 100);
                            $scope.newItem.validateCode = updatedData.maHang;
                            $scope.newItem.soLuongTon = response.data.data.soLuongTon;
                        } else {
                            updatedData.donGia = updatedData.giaMua;
                            $scope.newItem.validateCode = updatedData.maHang;
                            $scope.newItem.giaBanLe = updatedData.giaBanLe;
                            $scope.newItem.giaMuaCoVat = updatedData.giaMua * (1 + updatedData.tyLeVatVao / 100);
                            $scope.newItem.soLuongTon = response.data.data.soLuongTon;
                        }
                    });
                    //-------------------------
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
                    service.getMerchandiseForNvByCode(code, $scope.target.maKhoXuat, unitCode).then(function (response) {
                        if (response && response.status === 200 && response.data && response.data.status) {
                            $scope.newItem = response.data.data;
                            $scope.newItem.soLuongTon = response.data.data.soLuongTon;
                            $scope.newItem.vat = response.data.data.maVatVao;
                            $scope.newItem.donGia = response.data.data.giaVon;
                            $scope.newItem.validateCode = response.data.data.maHang;
                            $scope.newItem.giaMuaCoVat = response.data.data.giaVon * (1 + response.data.data.tyLeVatVao / 100);
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
            $scope.selectedKhoXuat = function (item) {
                $scope.target.maKhoXuat = item.value;
                service.getWareHouse(item.id).then(function (response) {
                    $scope.tkKtKhoXuat = response.taiKhoanKt;
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
            $scope.createWareHouse = function (target, name) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/WareHouse', 'add'),
                    controller: 'wareHouseCreateController',
                    resolve: {}
                });
                modalInstance.result.then(function (updatedData) {
                    $scope.putTempData('wareHouses', updatedData);
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
                    $scope.putTempData('merchandises', updatedData);
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
                    $scope.putTempData('packagings', updatedData);
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
                    $scope.putTempData('customers', updatedData);
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };
            $scope.createSupplier = function (target, name) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    size: 'lg',
                    templateUrl: configService.buildUrl('htdm/Supplier', 'add'),
                    controller: 'supplierCreateController',
                    resolve: {}
                });
                modalInstance.result.then(function (updatedData) {
                    serviceSupplier.getAll_Supplier().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                            tempDataService.putTempData('suppliers', successRes.data.data);
                            $scope.suppliers = successRes.data.data;
                            target[name] = updatedData.maKhachHang;
                        } else {
                            console.log('successRes', successRes);
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
    app.controller('phieuXuatKhacDetailsController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'phieuXuatKhacService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify', 'taxService', 'merchandiseService',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify, serviceTax, serviceMerchandise) {
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.targetData = angular.copy(targetData);
            $scope.tempData = tempDataService.tempData;
            $scope.target = targetData;
            $scope.robot = angular.copy(service.robot);
            $scope.isLoading = false;
            var taxRate = 0;
            $scope.title = function () { return 'Thông tin phiếu xuất khác'; };
            $scope.displayHepler = function (paraValue, moduleName) {
                var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                if (data && data.length === 1) {
                    return data[0].text;
                } else {
                    return paraValue;
                }
            }
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
            //note
            function loadTypeReason() {
                if (!tempDataService.tempData('typeReasons')) {
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
                } else {
                    $scope.typeReasons = tempDataService.tempData('typeReasons');
                }
            }
            $scope.reLoadTypeReason = function () {
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
            };
            loadTypeReason();
            function fillterData() {
                if ($scope.target.vat != null) { // Nếu phiếu có VAT thì filter lại giá vat ... (dữ liệu cũ)
                    serviceTax.getTaxByCode($scope.target.vat).then(function (response) {
                        if (response.status == 200) {
                            taxRate = response.data.taxRate;
                            service.getDetails($scope.target.id).then(function (response) {
                                if (response && response.status == 200 && response.data) {
                                    $scope.target = response.data;
                                    $scope.target.ngayCT = new Date($scope.target.ngayCT);
                                    $scope.target.ngayDieuDong = new Date($scope.target.ngayDieuDong);
                                    if ($scope.target.dataDetails.length > 0) {
                                        $scope.isListItemNull = false;
                                        $scope.target.dataDetails.forEach(function (obj) {
                                            obj.vat = $scope.target.vat;
                                            obj.giaMuaCoVat = obj.donGia * (1 + taxRate / 100);
                                            obj.thanhTienVAT = obj.soLuong * obj.giaMuaCoVat;
                                        });
                                    }
                                    serviceMerchandise.setSelectData($scope.target.dataDetails);
                                    $scope.pageChanged();
                                    var z = $filter('filter')($scope.tempData('taxs'), { value: $scope.target.vat }, true);
                                    //$scope.tyGia = z[0].extendValue;
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
                        }
                    });
                }
                else {
                    service.getDetails($scope.target.id).then(function (response) {
                        if (response && response.status == 200 && response.data) {
                            $scope.target = response.data;
                            $scope.target.ngayCT = new Date($scope.target.ngayCT);
                            $scope.target.ngayDieuDong = new Date($scope.target.ngayDieuDong);

                            if ($scope.target.dataDetails.length > 0) {
                                $scope.isListItemNull = false;
                                $scope.target.dataDetails.forEach(function (obj) {
                                    obj.thanhTienVAT = obj.soLuong * obj.giaMuaCoVat;
                                });
                            }

                            serviceMerchandise.setSelectData($scope.target.dataDetails);
                            $scope.pageChanged();
                            var z = $filter('filter')($scope.tempData('taxs'), { value: $scope.target.vat }, true);
                            //$scope.tyGia = z[0].extendValue;
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
                }

            }
            fillterData();
            $scope.approval = function () {
                service.postApproval($scope.target).then(function (response) {
                    if (response) {
                        alert("Duyệt thành công!");
                        $uibModalInstance.close($scope.target);
                        $scope.goIndex = function () {
                            $state.go('nvXuatKhac');
                        };
                    }
                    else { alert("Thất bại! - Xảy ra lỗi hoặc phiếu này đã duyệt"); }
                });
            };
            $scope.cancel = function () {
                $uibModalInstance.close();
            };
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
        }]);
    /* controller delete */
    app.controller('phieuXuatKhacDeleteController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'phieuXuatKhacService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify',
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

    /* report Phieu Xuat Khac Controller */
    app.controller('reportPhieuXuatKhacController', ['$scope', '$location', '$http', 'configService', 'phieuXuatKhacService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'userService', '$stateParams', '$window', 'taxService',
        function ($scope, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, serviceAuthUser, $stateParams, $window, serviceTax) {
            var currentUser = serviceAuthUser.GetCurrentUser();
            $scope.robot = angular.copy(service.robot);
            var id = $stateParams.id;
            var taxRate = 0;
            $scope.tempData = tempDataService.tempData;
            $scope.target = {};
            $scope.goIndex = function () {
                $state.go('nvXuatKhac');
            }
            function filterData() {
                if (id) {
                    service.getReport(id).then(function (response) {
                        if (response.data.vat != null) {
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
                        else {
                            if (response && response.status && response.data) {
                                $scope.target = response.data;
                                $scope.target.dataReportDetails.forEach(function (obj) {
                                    obj.thanhTien = obj.soLuong * obj.giaMuaCoVat;
                                    if (obj.vat == null) {
                                        obj.vat = 0;
                                        obj.giaMuaCoVat = obj.donGia;
                                        obj.thanhTien = obj.soLuong * obj.giaMuaCoVat;
                                    }
                                });
                            }
                        }
                    });
                    $scope.currentUser = currentUser.userName;
                }
            };
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
                var fileName = "XuatKhac_ExportData.xls";
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
                    var url = window.URL.createObjectURL(new Blob(data, { type: filetype }));
                    a.attr("href", url);
                    a.attr("download", fileName);
                    $("body").append(a);
                    a[0].click();
                    a.remove();
                }
            }
        }]);
    /* print Phieu Xuat Khac Controller */
    app.controller('printPhieuXuatKhacController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'phieuXuatKhacService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify) {
            $scope.robot = angular.copy(service.robot);
            $scope.config = angular.copy(configService);
            $scope.targetData = angular.copy(targetData);
            $scope.target = targetData;
            var id = $stateParams.id;
            $scope.target = {};
            $scope.info = service.getParameterPrint().filtered.advanceData;
            $scope.goIndex = function () {
                $state.go('nvXuatKhac');
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

    /* print Detail Phieu Xuat Khac Controller */
    app.controller('printDetailPhieuXuatKhacController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'phieuXuatKhacService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify) {
            $scope.robot = angular.copy(service.robot);
            $scope.config = angular.copy(configService);
            $scope.targetData = angular.copy(targetData);
            $scope.target = targetData;
            var id = $stateParams.id;
            $scope.target = {};
            $scope.info = service.getParameterPrint().filtered.advanceData;
            $scope.goIndex = function () {
                $state.go('nvXuatKhac');
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
    app.controller('phieuXuatKhacExportItemController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'phieuXuatKhacService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify) {
            $scope.robot = angular.copy(service.robot);
            $scope.config = angular.copy(configService);
            $scope.targetData = angular.copy(targetData);
            $scope.target = targetData;
            $scope.isLoading = false;
            $scope.lstMerchandise = [];
            $scope.dataHangHoa = {};
            $scope.maChungTuPk = targetData.maChungTuPk;
            $scope.title = function () { return 'Danh sách hàng xuất'; };
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
                $state.go('nvXuatKhac');
            };
            $scope.cancel = function () {
                $uibModalInstance.close();
            };
        }]);
    return app;
});