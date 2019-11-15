/*  
* Người tạo : Nguyễn Tuấn Hoàng Anh
* View: BTS.SP.MART/views/nv/NvDieuChuyenNoiBoNhanNhan
* Vm sevices: BTS.API.SERVICE -> NV ->NvPhieuDieuChuyenNoiBoNhanVm.cs
* Sevices: BTS.API.SERVICE -> NV -> NvPhieuDieuChuyenNoiBoNhanService.cs
* Entity: BTS.API.ENTITY -> NV - > NvDieuChuyenNoiBoNhan.cs
* Menu: Nghiệp vụ-> Điều chuyển nội bộ
*/
define(['ui-bootstrap', '/BTS.SP.MART/controllers/auth/AuthController.js', '/BTS.SP.MART/controllers/htdm/periodController.js', '/BTS.SP.MART/controllers/htdm/merchandiseController.js', '/BTS.SP.MART/controllers/htdm/customerController.js', '/BTS.SP.MART/controllers/htdm/merchandiseTypeController.js', '/BTS.SP.MART/controllers/htdm/nhomVatTuController.js', '/BTS.SP.MART/controllers/htdm/supplierController.js', '/BTS.SP.MART/controllers/htdm/wareHouseController.js', '/BTS.SP.MART/controllers/htdm/packagingController.js', '/BTS.SP.MART/controllers/htdm/taxController.js', '/BTS.SP.MART/controllers/htdm/donViTinhController.js', '/BTS.SP.MART/controllers/auth/AuDonVi.js', '/BTS.SP.MART/controllers/nv/phieuDieuChuyenNoiBoController.js'], function () {
    'use strict';
    var app = angular.module('phieuDieuChuyenNoiBoNhanModule', ['ui.bootstrap', 'authModule', 'periodModule', 'merchandiseModule', 'customerModule', 'merchandiseTypeModule', 'nhomVatTuModule', 'supplierModule', 'wareHouseModule', 'packagingModule', 'taxModule', 'donViTinhModule', 'phieuDieuChuyenNoiBoModule']);
    app.factory('phieuDieuChuyenNoiBoNhanService', ['$http', 'configService', function ($http, configService) {
        var serviceUrl = configService.rootUrlWebApi + '/NV/DieuChuyenNoiBo';
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
                target.thanhTienVAT = target.thanhTien * (1 + target.tyLeVatVao / 100);

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
                target.thanhTienVAT = target.thanhTien * (1 + target.tyLeVatVao / 100);

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
                item.thanhTienVAT = item.thanhTien * (1 + item.tyLeVatVao / 100);

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
                item.soLuong = parseInt(item.soLuongBao) * parseInt(item.luongBao) + parseInt(item.soLuongLe);
                item.thanhTien = parseInt(item.soLuong) * parseInt(item.donGia);
                item.thanhTienVAT = parseInt(item.thanhTien) * (1 + parseInt(item.tyLeVatVao) / 100);

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
                item.giaMuaCoVat = item.donGia * (1 + item.tyLeVatVao / 100);
                item.thanhTienVAT = item.thanhTien * (1 + item.tyLeVatVao / 100);
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
                item.donGia = item.giaMuaCoVat / (1 + item.tyLeVatVao / 100);
                item.thanhTien = item.soLuong * item.donGia;
                item.thanhTienVAT = item.thanhTien * (1 + item.tyLeVatVao / 100);
                item.giaMuaCoVat = item.donGia * (1 + item.tyLeVatVao / 100);
            }
        }
        this.parameterPrint = {};

        function getParameterPrint() {
            return this.parameterPrint;
        }

        var result = {
            robot: calc,
            setParameterPrint: function (data) {
                parameterPrint = data;
            },
            getInfoItemDetails: function (id) {
                return $http.get(serviceUrl + '/GetInfoItemDetails/' + id);
            },
            writeDataToExcel: function (data) {
                return $http.post(serviceUrl + '/WriteDataToExcel', data);
            },
            getParameterPrint: function () {
                return parameterPrint;
            },
            post: function (data) {
                return $http.post(serviceUrl + '/Post', data);
            },
            writeDataToExcelByShelves: function (data) {
                return $http.post(serviceUrl + '/WriteDataToExcelByShelves', data);
            },
            postQuery: function (data) {
                return $http.post(serviceUrl + '/PostQuery', data);
            },
            postRecieveQuery: function (data) {
                return $http.post(serviceUrl + '/PostRecieveQuery', data);
            },
            postQueryApproval: function (data, callback) {
                $http.post(serviceUrl + '/PostQueryApproval', data).success(callback);
            },
            postPrint: function (callback) {
                $http.post(serviceUrl + '/PostPrint', getParameterPrint()).success(callback);
            },
            postPrintDetail: function (callback) {
                $http.post(serviceUrl + '/PostPrintDetail', getParameterPrint()).success(callback);
            },
            postPostRecieve: function (data) {
                return $http.post(serviceUrl + '/PostRecieve', data);
            },
            postPostNewRecieve: function (data, callback) {
                return $http.post(serviceUrl + '/PostNewRecieve', data).success(callback);
            },
            postApproval: function (data) {
                return $http.post(serviceUrl + '/PostApproval', data);
            },
            postItemInventoryByCode: function (data, callback) {
                return $http.post(rootUrl + '/api/Md/Merchandise/PostItemInventoryByCode', data).success(callback);
            },
            getReport: function (id, callback) {
                $http.get(serviceUrl + '/GetReport/' + id).success(callback);
            },
            updateCT: function (params) {
                return $http.put(serviceUrl + '/' + params.id, params);
            },
            getReportReceive: function (id) {
                return $http.get(serviceUrl + '/GetReportReceive/' + id);
            },
            getNewInstance: function (callback) {
                $http.get(serviceUrl + '/GetNewInstance').success(callback);
            },
            getNewReciveInstance: function (callback) {
                $http.get(serviceUrl + '/GetNewReciveInstance').success(callback);
            },
            getNewInstanceFrom: function (maChungTu, callback) {
                $http.get(serviceUrl + '/GetNewInstanceFrom/' + maChungTu).success(callback);
            },
            getDetails: function (id) {
                return $http.get(serviceUrl + '/GetDetails/' + id);
            },
            getCurrentUser: function (callback) {
                $http.get(rootUrl + '/api/Authorize/AuNguoiDung/GetCurrentUser').success(callback);
            },
            getWareHouseByUnit: function (maDonVi) {
                return $http.get(rootUrl + '/api/Md/WareHouse/GetByUnit/' + maDonVi);
            },
            getWareHouseByCode: function (code, callback) {
                $http.get(rootUrl + '/api/Md/WareHouse/GetByCode/' + code).success(callback);
            },
            getModuleById: function (id, module, callback) {
                $http.get(rootUrl + '/api/Md/' + module + '/' + id).success(callback);
            },
            update: function (params) {
                return $http.put(serviceUrl + '/' + params.id, params);
            },
            deleteItem: function (params) {
                return $http.delete(serviceUrl + '/' + params.id, params);
            },
            getDataExcel: function (callback) {
                $http.get(serviceUrl + '/GetDataExcel').success(callback);
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
                    a.download = "PhieuDieuChuyenXuat.xlsx";
                    a.click();
                    //window.URL.revokeObjectURL(url);
                }).error(function (data, status, headers, config) {
                    //upload failed
                });
                //$http.post(serviceUrl + '/WriteDataToExcel', data).success(callback);
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
                    a.download = "PhieuDieuChuyenXuatTheoHang.xlsx";
                    a.click();
                    // window.URL.revokeObjectURL(objectUrl);
                }).error(function (data, status, headers, config) {
                    //upload failed
                });

                //$http.post(serviceUrl + '/WriteDataToExcel', data).success(callback);
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
                    a.download = "PhieuDieuChuyenXuatTheoLoaiHang.xlsx";
                    a.click();
                    // window.URL.revokeObjectURL(objectUrl);
                }).error(function (data, status, headers, config) {
                    //upload failed
                });
                //$http.post(serviceUrl + '/WriteDataToExcel', data).success(callback);
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
                    a.download = "PhieuDieuChuyenXuatTheoNhomHang.xlsx";
                    a.click();
                    // window.URL.revokeObjectURL(objectUrl);
                }).error(function (data, status, headers, config) {
                    //upload failed
                });
                //$http.post(serviceUrl + '/WriteDataToExcel', data).success(callback);
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
                    a.download = "PhieuDieuChuyenXuatTheoNCC.xlsx";
                    a.click();
                    // window.URL.revokeObjectURL(objectUrl);
                }).error(function (data, status, headers, config) {
                    //upload failed
                });
                //$http.post(serviceUrl + '/WriteDataToExcel', data).success(callback);
            },
            postExportExcelReceive: function (json) {
                $http({
                    url: serviceUrl + '/postExportExcelReceive',
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
                    a.download = "PhieuDieuChuyenNhan.xlsx";
                    a.click();
                    //window.URL.revokeObjectURL(url);
                }).error(function (data, status, headers, config) {
                    //upload failed
                });
                //$http.post(serviceUrl + '/WriteDataToExcel', data).success(callback);
            },
            postExportExcelByMerchandiseReceive: function (json) {
                $http({
                    url: serviceUrl + '/postExportExcelByMerchandiseReceive',
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
                    a.download = "PhieuDieuChuyenNhanTheoHang.xlsx";
                    a.click();
                    // window.URL.revokeObjectURL(objectUrl);
                }).error(function (data, status, headers, config) {
                    //upload failed
                });

                //$http.post(serviceUrl + '/WriteDataToExcel', data).success(callback);
            },
            postExportExcelByMerchandiseTypeReceive: function (json) {
                $http({
                    url: serviceUrl + '/postExportExcelByMerchandiseTypeReceive',
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
                    a.download = "PhieuDieuChuyenNhanTheoLoaiHang.xlsx";
                    a.click();
                    // window.URL.revokeObjectURL(objectUrl);
                }).error(function (data, status, headers, config) {
                    //upload failed
                });

                //$http.post(serviceUrl + '/WriteDataToExcel', data).success(callback);
            },
            postExportExcelByMerchandiseGroupReceive: function (json) {
                $http({
                    url: serviceUrl + '/PostExportExcelByMerchandiseGroupReceive',
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
                    a.download = "PhieuDieuChuyenNhanTheoNhomHang.xlsx";
                    a.click();
                }).error(function (data, status, headers, config) {
                });
            },
            postExportExcelByNhaCungCapReceive: function (json) {
                $http({
                    url: serviceUrl + '/PostExportExcelByNhaCungCapReceive',
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
                    a.download = "PhieuDieuChuyenNhanTheoNCC.xlsx";
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
    app.controller('phieuDieuChuyenNoiBoNhanController', [
        '$scope', '$location', '$http', 'configService', 'phieuDieuChuyenNoiBoNhanService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'securityService', '$rootScope', 'toaster', 'periodService', 'merchandiseService', 'customerService', 'merchandiseTypeService', 'nhomVatTuService', 'supplierService', 'wareHouseService', 'packagingService', 'taxService', 'donViTinhService', 'AuDonViService',
        function ($scope, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, securityService, $rootScope, toaster, servicePeriod, serviceMerchandise, serviceCustomer, serviceMerchandiseType, serviceNhomVatTu, serviceSupplier, serviceWareHouse, servicePackaging, serviceTax, serviceDonViTinh, serviceAuthDonVi) {
            $scope.openClosingOut = false;
            //check có mở khóa sổ không
            function checkUnClosingOut() {
                servicePeriod.checkUnClosingOut().then(function (response) {
                    if (response && response.status === 200 && response.data && response.data.data) {
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
            //load dữ liệu
            function filterData() {
                $scope.isLoading = true;
                if ($scope.accessList.view) {
                    var postdata = {};
                    $scope.isLoading = true;
                    postdata = { paged: $scope.paged, filtered: $scope.filtered };
                    service.postRecieveQuery(postdata).then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data) {
                            $scope.isLoading = false;
                            $scope.data = successRes.data.data.data;
                            $scope.data.forEach(function (obj) {
                                if (obj.maDonViXuat == obj.unitCode) {
                                    obj.checkDCX = false;
                                }
                                else {
                                    obj.checkDCX = true;
                                }
                            });
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
            };
            $scope.displayUniCode = function (paraValue, moduleName) {
                var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                if (data && data.length === 1) {
                    return data[0].description;
                } else {
                    return paraValue;
                }
            };
            //check quyền truy cập
            function loadAccessList() {
                securityService.getAccessList('nvPhieuDieuChuyenNoiBoNhan').then(function (successRes) {
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

            function loadAuthDonVi() {
                if (!tempDataService.tempData('auDonVis')) {
                    serviceAuthDonVi.getAll_DonVi().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.length > 0) {
                            tempDataService.putTempData('auDonVis', successRes.data);
                            $scope.auDonVis = successRes.data;
                        } else {
                            console.log('successRes', successRes);
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.auDonVis = tempDataService.tempData('auDonVis');
                }
            }

            loadAuthDonVi();
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
            $scope.viewRecieve = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('nv/NvPhieuDieuChuyenNoiBoNhan', 'recieve'),
                    controller: 'phieuDieuChuyenNoiBoNhanRecieveController',
                    windowClass: 'app-modal-window',
                    resolve: {

                    }
                });
            }
            $scope.setPage = function (pageNo) {
                $scope.paged.currentPage = pageNo;
                filterData();
            };
            $scope.sortType = 'maNvDieuChuyenNoiBoNhan';
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
            $scope.title = function () {
                if ($scope.isSameUnitUser) {
                    return 'Phiếu nhận chuyển kho';
                }
                else {
                    return 'Phiếu nhận siêu thị thành viên';
                }
            };
            $scope.create = function () {
                var modalInstance = $uibModal.open({
                    templateUrl: configService.buildUrl('nv/NvPhieuDieuChuyenNoiBoNhan', 'add'),
                    controller: 'phieuDieuChuyenNoiBoNhanCreateController',
                    windowClass: 'app-modal-window',
                    backdrop: 'static',
                    resolve: {
                        objectFilter: function () {
                            return {};
                        }
                    }
                });
                modalInstance.result.then(function (updatedData) {
                    $scope.refresh();
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };
            /* Function add New Item */

            /* Function Edit Item */
            $scope.update = function (target) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('nv/NvPhieuDieuChuyenNoiBoNhan', 'update'),
                    controller: 'phieuDieuChuyenNoiBoNhanEditController',
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

            $scope.tranferFrom = function (item) {
                item.iState = null;
                var modalInstance = $uibModal.open({
                    templateUrl: configService.buildUrl('nv/NvPhieuDieuChuyenNoiBo', 'add'),
                    controller: 'phieuDieuChuyenNoiBoCreateController',
                    windowClass: 'app-modal-window',
                    resolve: {
                        objectFilter: function () {
                            return {
                                loaiPhieu: item.loaiPhieu,
                                maKhoXuat: item.unitCode + '-K1',
                                maChungTu: item.maChungTu,
                                maKhoNhap: item.unitCode + '-K2',
                            };
                        }
                    }
                });
                modalInstance.result.then(function (updatedData) {
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };

            /* Function Details Item */
            $scope.details = function (target) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('nv/NvPhieuDieuChuyenNoiBoNhan', 'details'),
                    controller: 'phieuDieuChuyenNoiBoNhanDetailsController',
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
                    templateUrl: configService.buildUrl('nv/NvPhieuDieuChuyenNoiBoNhan', 'printItem'),
                    controller: 'phieuDieuChuyenNoiBoNhanExportItemController',
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

            $scope.printItemShelves = function (target) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('nv/NvPhieuDieuChuyenNoiBoNhan', 'printItemShelves'),
                    controller: 'printItemItemShelvesController',
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
                    templateUrl: configService.buildUrl('nv/NvPhieuDieuChuyenNoiBoNhan', 'delete'),
                    controller: 'phieuDieuChuyenNoiBoNhanDeleteController',
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

            $scope.changeFilterLoaiDCN = function (item) {
                $scope.isLoading = true;
                if ($scope.accessList.view) {
                    var postdata = {};
                    $scope.isLoading = true;
                    postdata = { paged: $scope.paged, filtered: $scope.filtered, loaiDCN: item };
                    service.postRecieveQuery(postdata).then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data) {
                            $scope.isLoading = false;
                            $scope.data = successRes.data.data.data;
                            $scope.data.forEach(function (obj) {
                                if (obj.maDonViXuat == obj.unitCode) {
                                    obj.checkDCX = false;
                                }
                                else {
                                    obj.checkDCX = true;
                                }
                            });
                            checkUnClosingOut();
                            angular.extend($scope.paged, successRes.data.data);
                        }
                    });
                }
            }
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
                $state.go("nvPrintDetailPhieuNhapHangMua");
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
    app.controller('phieuDieuChuyenNoiBoNhanCreateController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'phieuDieuChuyenNoiBoNhanService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', '$rootScope', 'userService', 'FileUploader', 'merchandiseService', 'customerService', 'merchandiseTypeService', 'nhomVatTuService', 'supplierService', 'wareHouseService', 'packagingService', 'taxService', 'donViTinhService', 'toaster', 'periodService', 'objectFilter', 'AuDonViService',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, $rootScope, serviceAuthUser, FileUploader, serviceMerchandise, serviceCustomer, serviceMerchandiseType, serviceNhomVatTu, serviceSupplier, serviceWareHouse, servicePackaging, serviceTax, serviceDonViTinh, toaster, servicePeriod, objectFilter, serviceAuthDonVi) {
            var currentUser = serviceAuthUser.GetCurrentUser();
            var unitCode = currentUser.unitCode;
            $scope.robot = angular.copy(service.robot);
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.data = [];
            $scope.acceptEnter = true;
            $scope.target = { dataDetails: [] };
            $scope.khoNhaps = [];
            $scope.newItem = {};
            var targetObj = {};
            $scope.donHangs = [];
            $scope.tkKtKhoNhap = "";
            $scope.tyGia = 0;
            $scope.isListItemNull = true;
            $scope.tempData = tempDataService.tempData;
            $scope.isLoading = false;
            $scope.isSameUnitUser = true;
            $scope.displayHepler = function (paraValue, moduleName) {
                var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                if (data && data.length === 1) {
                    return data[0].text;
                } else {
                    return paraValue;
                }
            };
            $scope.$watch("target.maDonViXuat", function (newValue, oldValue) {
                var maDonViXuat = $scope.target.maDonViXuat;
                service.getCurrentUser(function (response) {
                    var maDonViNhan = response.unitUser;
                    if (maDonViXuat === maDonViNhan) {
                        $scope.isSameUnitUser = true;
                    }
                    else {
                        $scope.isSameUnitUser = false;
                    }
                });
            }, true);
            //load danh muc
            $scope.changeDonViXuat = function (maDonViXuat) {
                $scope.target.maKhoXuat = '';
                serviceWareHouse.getByUnit(maDonViXuat).then(function (successRes) {
                    if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                        tempDataService.putTempData('wareHousesByUnit', successRes.data.data);
                        $scope.wareHousesByUnit = successRes.data.data;
                    } else {
                        console.log('successRes', successRes);
                    }
                }, function (errorRes) {
                    console.log('errorRes', errorRes);
                });

            };

            function loadCurrentWareHouse() {
                if (!tempDataService.tempData('currentWareHouses')) {
                    serviceWareHouse.getByUnit(unitCode).then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                            tempDataService.putTempData('currentWareHouses', successRes.data.data);
                            $scope.currentWareHouses = successRes.data.data;
                        } else {
                            console.log('successRes', successRes);
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.currentWareHouses = tempDataService.tempData('currentWareHouses');
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
            function loadAuthDonVi() {
                if (!tempDataService.tempData('auDonVis')) {
                    serviceAuthDonVi.getAll_DonVi().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.length > 0) {
                            tempDataService.putTempData('auDonVis', successRes.data);
                            $scope.auDonVis = successRes.data;
                        } else {
                            console.log('successRes', successRes);
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.auDonVis = tempDataService.tempData('auDonVis');
                }
            }

            loadAuthDonVi();
            loadSupplier();
            loadMerchandiseType();
            loadNhomVatTu();
            loadWareHouse();
            loadPackagings();
            loadTax();
            loadDonViTinh();
            loadCurrentWareHouse();
            //end 
            $scope.title = function () {
                if ($scope.isSameUnitUser) {
                    return 'Phiếu nhận chuyển kho';
                }
                else {
                    return 'Phiếu nhận siêu thị thành viên';
                }
            };

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
            function getNew(maChungTu) {
                service.getNewInstanceFrom(maChungTu, function (response) {
                    $scope.target = response;
                    $scope.pageChanged();
                    var data = $filter('filter')($scope.khoXuats, { value: objectFilter.maKhoNhap }, true);
                    if (data && data.length == 1) {
                        $scope.target.maKhoNhap = data[0];
                    }
                    $scope.target.lenhDieuDong = objectFilter.maChungTu;
                });
            };

            function filterData() {
                var maChungTu = "";
                $scope.isLoading = true;
                if (objectFilter && objectFilter.maChungTu) {
                    maChungTu = objectFilter.maChungTu;
                    getNew(maChungTu);
                } else {
                    service.getNewReciveInstance(function (response) {
                        $scope.target = response;
                        servicePeriod.getKyKeToan().then(function (response) {
                            if (response && response.status == 200 && response.data) {
                                $scope.target.ngayCT = new Date(response.data.toDate);
                                $scope.target.ngayDieuDong = new Date(response.data.toDate);
                                targetObj.fromDate = $filter('date')($scope.target.ngayCT, 'yyyy-MM-dd');
                                targetObj.toDate = $filter('date')($scope.target.ngayCT, 'yyyy-MM-dd');
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
                        $scope.pageChanged();
                    });
                }
            };
            filterData();

            $scope.addRow = function () {
                if (!$scope.newItem.soLuong || $scope.newItem.soLuong < 1) {
                    focus('soluong');
                    document.getElementById('soluong').focus();
                    return;
                }
                if ($scope.newItem.validateCode === $scope.newItem.maHang) {
                    service.postItemInventoryByCode({ code: $scope.newItem.maHang, wareHouseCode: $scope.target.maKhoNhap }, function (response) {
                        $scope.newItem.soLuongTon = response.closingQuantity;
                        console.log('thành công');
                        if (!angular.isUndefined($scope.newItem.maHang)) {
                            $scope.target.dataDetails.push($scope.newItem);
                        }
                        $scope.pageChanged();
                        $scope.newItem = {};
                        focus('mahang');
                        document.getElementById('mahang').focus();
                    }).error(function (response) {
                        console.log('lỗi');
                        $scope.target.dataDetails.push($scope.newItem);
                        $scope.pageChanged();
                        $scope.newItem = {};
                        focus('mahang');
                        document.getElementById('mahang').focus();
                    });
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
                                summary: strKey,
                                advanceData: { withGiaVon: true, maKho: $scope.target.maKhoNhap },
                            };
                        }
                    }
                });
                modalInstance.result.then(function (updatedData) {
                    if (!updatedData.selected) {
                        $scope.newItem = updatedData;
                        targetObj.merchandiseCodes = updatedData.maHang;
                        targetObj.wareHouseCodes = $scope.target.maKhoXuat;
                        targetObj.wareHouseRecieveCode = $scope.target.maKhoNhap;
                        servicePeriod.getSoLuongTonByDate(targetObj).then(function (response) {
                            if (response && response.status === 200 && response.data.data) {
                                $scope.newItem.soLuongTonXuat = response.data.data.soLuongTonKhoXuat;
                                $scope.newItem.soLuongTonNhap = response.data.data.soLuongTonKhoNhap;
                            }
                        });
                        //-------------------------
                        service.getByCodeWithGiaVon(updatedData.maHang, $scope.target.maKhoXuat, unitCode).then(function (response) {
                            if (response && response.status === 200 && response.data.status) {
                                if ($scope.isSameUnitUser) {
                                    $scope.newItem.donGia = response.data.data.giaVon;
                                    $scope.robot.changeDonGia($scope.newItem);
                                } else {
                                    $scope.newItem.donGia = updatedData.giaMua;;
                                    $scope.robot.changeDonGia($scope.newItem);
                                }
                                $scope.newItem.validateCode = updatedData.maHang;
                            }
                        });
                        //-------------------------
                    }
                    $scope.pageChanged();
                }, function () {

                });
            };
            $scope.getWareHouseImportByUnit = function () {
                if ($scope.target.maDonViXuat) {
                    service.getWareHouseByUnit($scope.target.maDonViXuat).then(function (response) {
                        if (response && response.status === 200 && response.length > 0)
                            $scope.khoNhaps = response;
                    });
                };
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
                    if (response && response.status == 200 && response.data) {
                        $scope.target.maSoThue = response.data.maSoThue;
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
                    service.getByCodeWithGiaVon(code, $scope.target.maKhoXuat, unitCode).then(function (response) {
                        console.log(response);
                        if (response && response.status == 200 && response.data.status) {
                            $scope.newItem = response.data.data;
                            $scope.newItem.vat = response.data.data.maVatVao;
                            targetObj.merchandiseCodes = code;
                            targetObj.wareHouseCodes = $scope.target.maKhoXuat;
                            targetObj.wareHouseRecieveCode = $scope.target.maKhoNhap;
                            servicePeriod.getSoLuongTonByDate(targetObj).then(function (response) {
                                if (response && response.status === 200 && response.data.data) {
                                    $scope.newItem.soLuongTonXuat = response.data.data.soLuongTonKhoXuat;
                                    $scope.newItem.soLuongTonNhap = response.data.data.soLuongTonKhoNhap;
                                }
                            });
                            if ($scope.isSameUnitUser) {
                                $scope.newItem.donGia = response.data.data.giaVon;
                                $scope.robot.changeDonGia($scope.newItem);
                            } else {
                                $scope.newItem.donGia = response.data.data.giaVon;
                                $scope.robot.changeDonGia($scope.newItem);
                            }
                            if ($scope.newItem.donGia === 0) {
                                $scope.newItem.donGia = response.data.data.giaMua;
                                $scope.robot.changeDonGia($scope.newItem);
                            }
                            $scope.newItem.validateCode = response.data.data.maHang;
                            document.getElementById('soluong').focus();
                        } else {
                            $scope.addNewItem(code);
                        }
                    }, function () {

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
                $scope.target.maKhoXuat = item.value;
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
                //index để sắp xếp theo mã hàng lúc thêm
                if ($scope.target.dataDetails.length > 0) {
                    angular.forEach($scope.target.dataDetails, function (value, index) {
                        $scope.target.dataDetails.index = index;
                    });
                }
                service.postPostRecieve($scope.target).then(function (successRes) {
                    if (successRes && successRes.status === 200 && successRes.data) {
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
                        $scope.target.dataDetails.clear();
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
                        var url = $state.href('reportPhieuNhapHangMua', { id: response.data.id });
                        window.open(url, 'Report Viewer');
                        $scope.target.dataDetails.clear();
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
    app.controller('phieuDieuChuyenNoiBoNhanEditController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'phieuDieuChuyenNoiBoNhanService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify', 'merchandiseService', 'toaster', 'AuDonViService', 'taxService', 'periodService', 'userService', 'wareHouseService',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify, serviceMerchandise, toaster, serviceAuthDonVi, serviceTax, servicePeriod, serviceAuthUser, serviceWareHouse) {
            var currentUser = serviceAuthUser.GetCurrentUser();
            var unitCode = currentUser.unitCode;
            $scope.config = angular.copy(configService);
            $scope.robot = angular.copy(service.robot);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.tempData = tempDataService.tempData;
            $scope.target = targetData;
            $scope.newItem = {};
            $scope.khoNhaps = [];
            $scope.tkKtKhoNhap = "";
            $scope.isSameUnitUser = true;
            $scope.tyGia = 0;
            $scope.isLoading = true;
            $scope.title = function () {
                if ($scope.isSameUnitUser) {
                    return 'Cập nhật phiếu nhận chuyển kho';
                }
                else {
                    return 'Cập nhật phiếu nhận siêu thị thành viên';
                }
            };
            $scope.$watch("target.maDonViXuat", function (newValue, oldValue) {
                var maDonViXuat = $scope.target.maDonViXuat;
                if (maDonViXuat === unitCode) {
                    $scope.isSameUnitUser = true;
                }
                else {
                    $scope.isSameUnitUser = false;
                }
            }, true);
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
                        $scope.target.ngayDieuDong = new Date($scope.target.ngayDieuDong);
                        if ($scope.target.dataDetails.length > 0) {
                            $scope.isListItemNull = false;
                            $scope.target.dataDetails.forEach(function (obj) {
                                var targetObj = {};
                                obj.thanhTienVAT = obj.soLuong * obj.giaMuaCoVat;
                                var giaTriVat = $filter('filter')(tempDataService.tempData('taxs'), { value: obj.vat }, true);
                                if (giaTriVat && giaTriVat.length > 0) {
                                    obj.tyLeVatVao = giaTriVat[0].extendValue;
                                } else {
                                    obj.tyLeVatVao = 0;
                                }
                                targetObj.fromDate = $filter('date')($scope.target.ngayCT, 'yyyy-MM-dd');
                                targetObj.toDate = $filter('date')($scope.target.ngayCT, 'yyyy-MM-dd');
                                targetObj.merchandiseCodes = obj.maHang;
                                targetObj.wareHouseCodes = $scope.target.maKhoXuat;
                                targetObj.wareHouseRecieveCode = $scope.target.maKhoNhap;
                                servicePeriod.getSoLuongTonByDate(targetObj).then(function (response) {
                                    if (response && response.status === 200 && response.data.data) {
                                        obj.soLuongTonXuat = response.data.data.soLuongTonKhoXuat;
                                        obj.soLuongTonNhap = response.data.data.soLuongTonKhoNhap;
                                    }
                                });
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
            };
            $scope.changeDonViXuat = function (maDonViXuat) {
                $scope.target.maKhoXuat = '';
                serviceWareHouse.getByUnit(maDonViXuat).then(function (successRes) {
                    if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                        tempDataService.putTempData('wareHousesByUnit', successRes.data.data);
                        $scope.wareHousesByUnit = successRes.data.data;
                    } else {
                        console.log('successRes', successRes);
                    }
                }, function (errorRes) {
                    console.log('errorRes', errorRes);
                });
            };

            function loadCurrentWareHouse() {
                if (!tempDataService.tempData('currentWareHouses')) {
                    serviceWareHouse.getByUnit(unitCode).then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                            tempDataService.putTempData('currentWareHouses', successRes.data.data);
                            $scope.currentWareHouses = successRes.data.data;
                        } else {
                            console.log('successRes', successRes);
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.currentWareHouses = tempDataService.tempData('currentWareHouses');
                }
            }
            function loadWareHouseByUnit() {
                if (!tempDataService.tempData('wareHousesByUnit')) {
                    serviceWareHouse.getByUnit($scope.target.maDonViXuat).then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                            tempDataService.putTempData('wareHousesByUnit', successRes.data.data);
                            $scope.wareHousesByUnit = successRes.data.data;
                        } else {
                            console.log('successRes', successRes);
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.wareHousesByUnit = tempDataService.tempData('wareHousesByUnit');
                }
            }

            loadCurrentWareHouse();
            loadWareHouseByUnit();
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
            };
            $scope.sum = function () {
                var total = 0;
                angular.forEach($scope.data, function (value, key) {
                    total = total + value.thanhTien;
                });
                $scope.total = total;
            }

            $scope.addRow = function () {
                if (!$scope.newItem.soLuong || $scope.newItem.soLuong < 1) {
                    focus('soluong');
                    document.getElementById('mahang').focus();
                    return;
                }
                if ($scope.newItem.validateCode == $scope.newItem.maHang) {
                    service.postItemInventoryByCode({ code: $scope.newItem.maHang, wareHouseCode: $scope.target.maKhoNhap }, function (response) {
                        $scope.newItem.soLuongTon = response.closingQuantity;
                        console.log('thành công');
                        $scope.target.dataDetails.push($scope.newItem);
                        $scope.pageChanged();
                        $scope.newItem = {};
                        focus('mahang');
                        document.getElementById('mahang').focus();
                    }).error(function (response) {
                        console.log('lỗi');
                        $scope.target.dataDetails.push($scope.newItem);
                        $scope.pageChanged();
                        $scope.newItem = {};
                        focus('mahang');
                        document.getElementById('mahang').focus();
                    });
                }
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
                                summary: strKey,
                                advanceData: { withGiaVon: true, maKho: $scope.target.maKhoNhap },
                            };
                        }
                    }
                });
                modalInstance.result.then(function (updatedData) {
                    if (!updatedData.selected) {
                        var targetObj = {};
                        targetObj.fromDate = $filter('date')($scope.target.ngayCT, 'yyyy-MM-dd');
                        targetObj.toDate = $filter('date')($scope.target.ngayCT, 'yyyy-MM-dd');
                        targetObj.merchandiseCodes = updatedData.maHang;
                        targetObj.wareHouseCodes = $scope.target.maKhoXuat;
                        targetObj.wareHouseRecieveCode = $scope.target.maKhoNhap;
                        servicePeriod.getSoLuongTonByDate(targetObj).then(function (response) {
                            if (response && response.status === 200 && response.data.data) {
                                $scope.newItem.soLuongTonXuat = response.data.data.soLuongTonKhoXuat;
                                $scope.newItem.soLuongTonNhap = response.data.data.soLuongTonKhoNhap;
                            }
                        });
                        $scope.newItem = updatedData;
                        //-------------------------
                        service.getByCodeWithGiaVon(updatedData.maHang, $scope.target.maKhoXuat, unitCode).then(function (response) {
                            if (response && response.status === 200 && response.data.status) {
                                if ($scope.isSameUnitUser) {
                                    $scope.newItem.donGia = response.data.data.giaVon;
                                    $scope.robot.changeDonGia($scope.newItem);
                                } else {
                                    $scope.newItem.donGia = response.data.data.giaMua;;
                                    $scope.robot.changeDonGia($scope.newItem);
                                }
                                $scope.newItem.validateCode = updatedData.maHang;
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
                $scope.pageChanged();
            };
            $scope.selectedkhachHang = function (item) {
                $scope.target.maKhachHang = item.value;
                service.getOrderByCustomer(item.value).then(function (response) {
                    if (response && response.data && response.data.length > 0) {
                        $scope.donHangs = response.data;
                    }
                });
                service.getCustomer(item.id).then(function (response) {
                    if (response && response.data) {
                        $scope.target.maSoThue = response.data.maSoThue;
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
                    service.getByCodeWithGiaVon(code, $scope.target.maKhoXuat, unitCode).then(function (response) {
                        if (response && response.data && response.data.status) {
                            var targetObj = {};
                            $scope.newItem = response.data.data;
                            targetObj.fromDate = $filter('date')($scope.target.ngayCT, 'yyyy-MM-dd');
                            targetObj.toDate = $filter('date')($scope.target.ngayCT, 'yyyy-MM-dd');
                            targetObj.merchandiseCodes = code;
                            targetObj.wareHouseCodes = $scope.target.maKhoXuat;
                            targetObj.wareHouseRecieveCode = $scope.target.maKhoNhap;
                            servicePeriod.getSoLuongTonByDate(targetObj).then(function (response) {
                                if (response && response.status === 200 && response.data.data) {
                                    $scope.newItem.soLuongTonXuat = response.data.data.soLuongTonKhoXuat;
                                    $scope.newItem.soLuongTonNhap = response.data.data.soLuongTonKhoNhap;
                                }
                            });
                            $scope.newItem.VAT = response.data.data.maVatVao;
                            if ($scope.isSameUnitUser) {
                                $scope.newItem.donGia = response.data.data.giaVon;
                                $scope.robot.changeDonGia($scope.newItem);
                            } else {
                                $scope.newItem.donGia = response.data.data.giaMua;
                                $scope.robot.changeDonGia($scope.newItem);
                            }
                            $scope.newItem.validateCode = response.data.data.maHang;
                            document.getElementById('soluong').focus();
                        } else {
                            $scope.addNewItem(code);
                        }
                    }, function () {
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
                $scope.target.maKhoXuat = item.value;
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
            };
            $scope.saveAndPrint = function () {
                service.updateCT($scope.target).then(function (successRes) {
                    if (successRes && successRes.status === 200 && successRes.data.status) {
                        ngNotify.set(successRes.data.message, { type: 'success' });
                        var url = $state.href('reportPhieuNhapHangMua', { id: successRes.data.data.id });
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
    app.controller('phieuDieuChuyenNoiBoNhanDetailsController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'phieuDieuChuyenNoiBoNhanService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify', 'merchandiseService', 'taxService', 'periodService',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify, serviceMerchandise, serviceTax, servicePeriod) {
            $scope.config = angular.copy(configService);
            $scope.robot = angular.copy(service.robot);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.tempData = tempDataService.tempData;
            $scope.target = targetData;
            $scope.isSameUnitUser = true;
            $scope.khoNhaps = [];
            $scope.khoXuats = [];
            var trangThai = $scope.target.trangThai;
            var thanhTienSauVat = $scope.target.thanhTienSauVat;
            var taxRate = 0;
            $scope.target.ngayCT = new Date(targetData.ngayCT);
            $scope.isLoading = false;
            $scope.title = function () {
                if ($scope.isSameUnitUser) {
                    return 'Thông tin phiếu nhận chuyển kho';
                }
                else {
                    return 'Thông tin phiếu nhận siêu thị thành viên';
                }
            };
            $scope.sum = function () {
                var total = 0;
                if ($scope.target.dataDetails) {
                    angular.forEach($scope.target.dataDetails, function (v, k) {
                        total = total + v.thanhTien;
                    });
                }
                return total;
            };
            $scope.getWareHouseImportByUnit = function () {
                if ($scope.target.maDonViXuat) {
                    service.getWareHouseByUnit($scope.target.maDonViXuat).then(function (response) {
                        if (response && response.status === 200 && response.data) {
                            $scope.khoNhaps = response.data;
                            var data = $filter('filter')($scope.khoNhaps, { value: $scope.target.maKhoXuat }, true);
                            if (data && data.length === 1) {
                                $scope.target.maKhoXuatTemp = data[0].value + ' | ' + data[0].text;
                            }
                        }

                    });
                };
                if ($scope.target.maDonViNhan) {
                    service.getWareHouseByUnit($scope.target.maDonViNhan).then(function (response) {
                        if (response && response.status === 200 && response.data) {
                            $scope.khoXuats = response.data;
                            var data = $filter('filter')($scope.khoXuats, { value: $scope.target.maKhoNhap }, true);
                            if (data && data.length === 1) {
                                $scope.target.maKhoNhapTemp = data[0].value + ' | ' + data[0].text;
                            }
                        }
                    });
                };
            }
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
            $scope.approval = function () {
                service.postApproval($scope.target).then(function (response) {
                    if (response) {
                        alert("Duyệt thành công!");
                        $uibModalInstance.close($scope.target);
                        $scope.goIndex = function () {
                            $state.go('nvDieuChuyenNoiBoNhan');
                        };
                    }
                    else { alert("Thất bại! - Xảy ra lỗi hoặc phiếu này đã duyệt"); }
                });
            };
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
                                            var targetObj = {};
                                            obj.vat = $scope.target.vat;
                                            obj.giaMuaCoVat = obj.donGia * (1 + taxRate / 100);
                                            obj.thanhTienVAT = obj.soLuong * obj.giaMuaCoVat;
                                            targetObj.fromDate = $filter('date')($scope.target.ngayCT, 'yyyy-MM-dd');
                                            targetObj.toDate = $filter('date')($scope.target.ngayCT, 'yyyy-MM-dd');
                                            targetObj.merchandiseCodes = obj.maHang;
                                            targetObj.wareHouseCodes = $scope.target.maKhoXuat;
                                            targetObj.wareHouseRecieveCode = $scope.target.maKhoNhap;
                                            servicePeriod.getSoLuongTonByDate(targetObj).then(function (response) {
                                                if (response && response.status === 200 && response.data.data) {
                                                    obj.soLuongTonXuat = response.data.data.soLuongTonKhoXuat;
                                                    obj.soLuongTonNhap = response.data.data.soLuongTonKhoNhap;
                                                }
                                            });
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
                                    var targetObj = {};
                                    obj.thanhTienVAT = obj.soLuong * obj.giaMuaCoVat;
                                    targetObj.fromDate = $filter('date')($scope.target.ngayCT, 'yyyy-MM-dd');
                                    targetObj.toDate = $filter('date')($scope.target.ngayCT, 'yyyy-MM-dd');
                                    targetObj.merchandiseCodes = obj.maHang;
                                    targetObj.wareHouseCodes = $scope.target.maKhoXuat;
                                    targetObj.wareHouseRecieveCode = $scope.target.maKhoNhap;
                                    servicePeriod.getSoLuongTonByDate(targetObj).then(function (response) {
                                        if (response && response.status === 200 && response.data.data) {
                                            obj.soLuongTonXuat = response.data.data.soLuongTonKhoXuat;
                                            obj.soLuongTonNhap = response.data.data.soLuongTonKhoNhap;
                                        }
                                    });
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
            };
            filterData();
            $scope.cancel = function () {
                $uibModalInstance.close();
            };

        }]);
    /* controller delete */
    app.controller('phieuDieuChuyenNoiBoNhanDeleteController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'phieuDieuChuyenNoiBoNhanService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify',
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
    /* report Phieu Dieu Chuyen Noi Bo Nhan Controller */
    app.controller('reportPhieuDieuChuyenNoiBoNhanController', ['$scope', '$location', '$http', 'configService', 'phieuDieuChuyenNoiBoNhanService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'userService', '$stateParams', '$window', 'taxService',
        function ($scope, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, serviceAuthUser, $stateParams, $window, serviceTax) {
            var currentUser = serviceAuthUser.GetCurrentUser();
            $scope.robot = angular.copy(service.robot);
            var id = $stateParams.id;
            $scope.tempData = tempDataService.tempData;
            $scope.target = {};
            var taxRate = 0;
            $scope.goIndex = function () {
                $state.go('nvDieuChuyenNoiBoNhan');
            }
            function filterData() {
                if (id) {
                    service.getReportReceive(id).then(function (response) {
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
                var fileName = "DieuChuyenNoiBoNhan_ExportData.xls";
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
    app.controller('phieuDieuChuyenNoiBoNhanRecieveController', [
        '$scope', '$uibModal', '$uibModalInstance',
        'phieuDieuChuyenNoiBoNhanService', 'configService', '$mdDialog', '$log',
        function ($scope, $uibModal, $uibModalInstance,
            phieuDieuChuyenNoiBoNhanService, configService, $mdDialog, $log) {
            $scope.config = configService.config;
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.filtered = angular.copy(configService.filterDefault);
            $scope.isEditable = true;
            function filterData() {
                var postdata = { paged: $scope.paged, filtered: $scope.filtered };
                phieuDieuChuyenNoiBoNhanService.postQueryApproval(
                    JSON.stringify(postdata),
                    function (response) {
                        if (response.status) {
                            $scope.data = response.data.data;
                            angular.extend($scope.paged, response.data);
                        }
                    });
            };
            $scope.setPage = function (pageNo) {
                $scope.paged.currentPage = pageNo;
                filterData();
            };
            /* Function Delete Item */
            $scope.deleteItem = function (event, item) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('nv/NvPhieuDieuChuyenNoiBoNhan', 'delete'),
                    controller: 'phieuDieuChuyenNoiBoReciveDeleteController',
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
            $scope.details = function (target) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('nv/NvPhieuDieuChuyenNoiBoNhan', 'recieveDetails'),
                    controller: 'phieuDieuChuyenNoiBoRecieveDetailsController',
                    windowClass: 'app-modal-window',
                    resolve: {
                        targetData: function () {
                            return target;
                        }
                    }
                });
                modalInstance.result.then(function (updatedData) {
                    if (updatedData == true) {
                        $uibModalInstance.close(true);
                    }
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };

            $scope.sortType = 'ngayCT'; // set the default sort type
            $scope.sortReverse = false;  // set the default sort order
            $scope.doSearch = function () {
                $scope.paged.currentPage = 1;
                filterData();
            };
            $scope.pageChanged = function () {
                filterData();
            };

            $scope.goHome = function () {
                $uibModalInstance.dismiss('cancel');
            };
            $scope.refresh = function () {
                $scope.setPage($scope.paged.currentPage);
            };
            $scope.title = function () {
                return 'Phiếu nhận điều chuyển nội bộ';
            };
            $scope.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };

            filterData();

        }]);
    app.controller('phieuDieuChuyenNoiBoNhanExportItemController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'phieuDieuChuyenNoiBoNhanService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify', '$window',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify, $window) {
            $scope.robot = angular.copy(service.robot);
            $scope.config = angular.copy(configService);
            $scope.targetData = angular.copy(targetData);
            $scope.target = targetData;
            $scope.isLoading = false;
            $scope.lstMerchandise = [];
            $scope.dataHangHoa = {};
            $scope.maChungTuPk = targetData.maChungTuPk;
            $scope.title = function () { return 'Danh sách hàng nhận'; };
            var index = $scope.maChungTuPk.indexOf('.');
            $scope.maDonVi = $scope.maChungTuPk.substr(0, index);
            service.getInfoItemDetails($scope.target.id).then(function (response) {
                if (response.status) {
                    $scope.dataHangHoa = response.data;
                    $scope.lstMerchandise = response.data.dataDetails;
                    angular.forEach($scope.lstMerchandise, function (v, k) {
                        if ($scope.lstMerchandise[k].barcode) {
                            if ($scope.lstMerchandise[k].barcode.length > 1) {
                                var b = $scope.lstMerchandise[k].barcode.split(';');
                                for (var i = b.length - 1; i >= 0; i--) {
                                    if (b[i]) {
                                        $scope.lstMerchandise[k].barcode = b[i];
                                        break;
                                    }
                                }
                            }
                        }
                    });
                }
                $scope.isLoading = false;
            });
            $scope.sum = function () {
                var total = 0;
                if ($scope.lstMerchandise.length > 0) {
                    angular.forEach($scope.lstMerchandise, function (v, k) {
                        total = total + v.soLuong;
                    });
                }
                return total;
            }
            $scope.hrefTem = configService.apiServiceBaseUri + "/Upload/Barcode/";
            $scope.exportToExcel = function () {
                service.writeDataToExcel($scope.dataHangHoa).then(function (response) {
                    if (response.data.status) {
                        ngNotify.set("Thành công", { type: 'success' });
                        $window.location.href = $scope.hrefTem + "" + response.data.message;
                        $uibModalInstance.close($scope.target);
                    }
                    else {
                        ngNotify.set("Không Thành công", { duration: 3000, type: 'error' });
                    }
                });
            }
            $scope.goIndex = function () {
                $state.go('nvPhieuDieuChuyenNoiBoNhan');
            };
            $scope.cancel = function () {
                $uibModalInstance.close();
            };
        }]);
    app.controller('printItemItemShelvesController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'phieuDieuChuyenNoiBoNhanService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify', '$window',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify, $window) {
            $scope.config = angular.copy(configService);
            $scope.target = targetData;
            $scope.isLoading = false;
            $scope.lstMerchandise = [];
            $scope.dataHangHoa = {};
            $scope.maChungTuPk = targetData.maChungTuPk;
            $scope.title = function () { return 'In tem mã kệ'; };
            var index = $scope.maChungTuPk.indexOf('.');
            $scope.maDonVi = $scope.maChungTuPk.substr(0, index);
            service.getInfoItemDetails($scope.target.id).then(function (response) {
                if (response && response.status == 200 && response.data) {
                    $scope.dataHangHoa = response.data;
                    $scope.lstMerchandise = response.data.dataDetails;
                    if ($scope.lstMerchandise.length > 0) {
                        angular.forEach($scope.lstMerchandise, function (v, k) {
                            v.soLuong = 1;
                        });
                    }
                    angular.forEach($scope.lstMerchandise, function (v, k) {
                        if ($scope.lstMerchandise[k].barcode) {
                            if ($scope.lstMerchandise[k].barcode.length > 1) {
                                var b = $scope.lstMerchandise[k].barcode.split(';');
                                for (var i = b.length - 1; i >= 0; i--) {
                                    if (b[i]) {
                                        $scope.lstMerchandise[k].barcode = b[i];
                                        break;
                                    }
                                }
                            }
                        }
                    });
                }
                $scope.isLoading = false;
            });
            $scope.sum = function () {
                var total = 0;
                if ($scope.lstMerchandise.length > 0) {
                    angular.forEach($scope.lstMerchandise, function (v, k) {
                        total = total + v.soLuong;
                    });
                }
                return total;
            }
            $scope.exportToExcel = function () {
                service.writeDataToExcelByShelves($scope.dataHangHoa).then(function (response) {
                    if (response && response.status === 200 && response.data) {
                        ngNotify.set("Thành công", { type: 'success' });
                        $scope.hrefTem = configService.apiServiceBaseUri + "/Upload/Barcode/" + response.data.message;
                        $window.location.href = $scope.hrefTem;
                    }
                    else {
                        ngNotify.set(response.data.message, { duration: 3000, type: 'error' });
                    }

                });
            }
            $scope.goIndex = function () {
                $state.go('nvNhapHangMua');
            };
            $scope.cancel = function () {
                $uibModalInstance.close();
            };
        }]);
    /* controller delete */
    app.controller('phieuDieuChuyenNoiBoReciveDeleteController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'phieuDieuChuyenNoiBoNhanService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify',
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
    app.controller('phieuDieuChuyenNoiBoRecieveDetailsController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'phieuDieuChuyenNoiBoNhanService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify', 'merchandiseService', 'wareHouseService',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify, serviceMerchandise, serviceWareHouse) {
            $scope.robot = angular.copy(service.robot);
            $scope.target = targetData;
            $scope.tempData = tempDataService.tempData;
            $scope.khoXuats = [];
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
            loadWareHouse();
            function fillterData() {
                service.getDetails($scope.target.id).then(function (response) {
                    if (response && response.status === 200 && response.data) {
                        $scope.target = response.data;
                        if ($scope.target.maDonViXuat) {
                            service.getWareHouseByUnit($scope.target.maDonViXuat).then(function (response) {
                                if (response && response.status === 200 && response.data) {
                                    $scope.khoXuats = response.data;
                                    var data = $filter('filter')($scope.khoXuats, { value: $scope.target.maKhoXuat }, true);
                                    if (data && data.length === 1) {
                                        $scope.target.maKhoXuatTemp = data[0].value;
                                    }
                                }
                            });
                        };
                        serviceMerchandise.setSelectData($scope.target.dataDetails);
                        $scope.pageChanged();
                    }
                });
            }
            $scope.title = function () {
                return 'Chi tiết phiếu nhận điều chuyển nội bộ';
            };
            $scope.sum = function () {
                var total = 0;
                angular.forEach($scope.target.dataDetails, function (value, key) {
                    total = total + value.thanhTien;
                });
                $scope.total = total;
            }
            $scope.save = function () {
                $scope.isImportDisable = true;
                //index để sắp xếp theo mã hàng lúc thêm
                if ($scope.target.dataDetails.length > 0) {
                    angular.forEach($scope.target.dataDetails, function (value, index) {
                        $scope.target.dataDetails.index = index;
                    });
                }
                service.postPostRecieve($scope.target).then(function (successRes) {
                    if (successRes && successRes.status === 200 && successRes.data) {
                        ngNotify.set("Thêm mới thành công", { type: 'success' });
                        $uibModalInstance.close(true);
                    } else {
                        console.log('addNew successRes', successRes);
                        ngNotify.set(successRes.data.message, { duration: 3000, type: 'error' });
                    }
                },
                    function (errorRes) {
                        ngNotify.set(errorRes.data.message, { duration: 3000, type: 'error' });
                        console.log('errorRes', errorRes);
                    });
            };

            $scope.displayHepler = function (paraValue, moduleName) {
                var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                if (data && data.length === 1) {
                    return data[0].text;
                } else {
                    return paraValue;
                }
            };

            $scope.formatLabelForWareHouseOtherUnit = function (model) {
                if (!model) return "";
                service.getWareHouseByUnit($scope.target.maDonViXuat).then(function (response) {
                    if (response && response.status === 200 && response.data) {
                        $scope.khoXuats = response.data;
                    }
                });
                return "Empty!";
            }
            fillterData();

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
            $scope.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };
        }]);

    return app;
});