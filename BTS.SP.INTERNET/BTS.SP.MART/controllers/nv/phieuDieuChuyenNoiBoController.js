/*  
* Người tạo : Nguyễn Tuấn Hoàng Anh
* View: BTS.SP.MART/views/nv/NvDieuChuyenNoiBo
* Vm sevices: BTS.API.SERVICE -> NV ->NvDieuChuyenNoiBoVm.cs
* Sevices: BTS.API.SERVICE -> NV -> NvDieuChuyenNoiBoService.cs
* Entity: BTS.API.ENTITY -> NV - > NvDieuChuyenNoiBo.cs
* Menu: Nghiệp vụ-> Điều chuyển nội bộ
*/
define(['ui-bootstrap', '/BTS.SP.MART/controllers/auth/AuthController.js', '/BTS.SP.MART/controllers/htdm/periodController.js', '/BTS.SP.MART/controllers/htdm/merchandiseController.js', '/BTS.SP.MART/controllers/htdm/customerController.js', '/BTS.SP.MART/controllers/htdm/merchandiseTypeController.js', '/BTS.SP.MART/controllers/htdm/nhomVatTuController.js', '/BTS.SP.MART/controllers/htdm/supplierController.js', '/BTS.SP.MART/controllers/htdm/wareHouseController.js', '/BTS.SP.MART/controllers/htdm/packagingController.js', '/BTS.SP.MART/controllers/htdm/taxController.js', '/BTS.SP.MART/controllers/htdm/donViTinhController.js', '/BTS.SP.MART/controllers/auth/AuDonVi.js', '/BTS.SP.MART/controllers/nv/phieuNhapHangMuaController.js'], function () {
    'use strict';
    var app = angular.module('phieuDieuChuyenNoiBoModule', ['ui.bootstrap', 'authModule', 'periodModule', 'merchandiseModule', 'customerModule', 'merchandiseTypeModule', 'nhomVatTuModule', 'supplierModule', 'wareHouseModule', 'packagingModule', 'taxModule', 'donViTinhModule', 'phieuNhapHangMuaModule']);
    app.factory('phieuDieuChuyenNoiBoService', ['$http', 'configService', 'taxService', function ($http, configService, serviceTax) {
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
                if (!item.tyLeVatVao) {
                    serviceTax.getTaxByCode(item.vat).then(function (response) {
                        item.tyLeVatVao = response.data.taxRate;
                    });
                }
                item.soLuong = (item.soLuongBao) * (item.luongBao) + (item.soLuongLe);
                if (item.soLuong > item.soLuongTonXuat) {
                    item.soLuong = item.soLuongTonXuat;
                }
                item.thanhTien = (item.soLuong) * (item.donGia);
                item.thanhTienVAT = (item.thanhTien) * (1 + (item.tyLeVatVao) / 100);

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


            }
        }
        this.parameterPrint = {};

        function getParameterPrint() {
            return this.parameterPrint;
        }

        var result = {
            robot: calc,
            updateCT: function (params) {
                return $http.put(serviceUrl + '/' + params.id, params);
            },
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
            postRecieveQuery: function (data, callback) {
                $http.post(serviceUrl + '/PostRecieveQuery', data).success(callback);
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
            postPostRecieve: function (data, callback) {
                return $http.post(serviceUrl + '/PostRecieve', data).success(callback);
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
            getReport: function (id) {
                return $http.get(serviceUrl + '/GetReport/' + id);
            },
            getReportReceive: function (id, callback) {
                $http.get(serviceUrl + '/GetReportReceive/' + id).success(callback);
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
            getWareHouseByUnit: function (maDonVi, callback) {
                $http.get(rootUrl + '/api/Md/WareHouse/GetByUnit/' + maDonVi).success(callback);
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
            getByCodeWithGiaVon: function (code, wareHouseCode, maDonVi) {
                return $http.get(rootUrl + '/api/Md/Merchandise/GetByCodeWithGiaVon/' + code + '/' + wareHouseCode + '/' + maDonVi);
            },
            getPhieuByLenhDieuDong: function (code) {
                return $http.get(serviceUrl + '/GetPhieuByLenhDieuDong/' + code);
            },
            deleteItem: function (params) {
                return $http.delete(serviceUrl + '/' + params.id, params);
            },
            getDataExcel: function (callback) {
                $http.get(serviceUrl + '/GetDataExcel').success(callback);
            },
            checkConnectServer: function (unitCode) {
                return $http.get(serviceUrl + '/CheckConnectServer/' + unitCode);
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
                    // window.URL.revokeObjectURL(objectUrl);
                }).error(function (data, status, headers, config) {
                    //upload failed
                });

                //$http.post(serviceUrl + '/WriteDataToExcel', data).success(callback);
            },
            postExportExcelByNhaCungCapReceive: function (json) {
                $http({
                    url: serviceUrl + '/PostExportExcelByNhaCungCapReceive',
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
                    a.download = "PhieuDieuChuyenNhanTheoNCC.xlsx";
                    a.click();
                    // window.URL.revokeObjectURL(objectUrl);
                }).error(function (data, status, headers, config) {
                    //upload failed
                });

                //$http.post(serviceUrl + '/WriteDataToExcel', data).success(callback);
            }
        };
        return result;
    }]);
    /* controller list */
    app.controller('phieuDieuChuyenNoiBoController', [
        '$scope', '$location', '$http', 'configService', 'phieuDieuChuyenNoiBoService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'securityService', '$rootScope', 'toaster', 'periodService', 'merchandiseService', 'customerService', 'merchandiseTypeService', 'nhomVatTuService', 'supplierService', 'wareHouseService', 'packagingService', 'taxService', 'donViTinhService', 'AuDonViService', 'userService',
        function ($scope, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, securityService, $rootScope, toaster, servicePeriod, serviceMerchandise, serviceCustomer, serviceMerchandiseType, serviceNhomVatTu, serviceSupplier, serviceWareHouse, servicePackaging, serviceTax, serviceDonViTinh, serviceAuthDonVi, serviceAuthUser) {
            var currentUser = serviceAuthUser.GetCurrentUser();
            var unitCode = currentUser.unitCode;
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
            $scope.target = { dataDetails: [], dataClauseDetails: [] };
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
                            angular.forEach(successRes.data.data.data, function (v, k) {
                                if (v.maKhoXuat.includes(unitCode) && !v.maDonViNhan.includes(unitCode) && !v.maKhoNhap.includes(unitCode)) {
                                    v.differenceUnitCode = true;
                                }
                                else {
                                    v.differenceUnitCode = false;
                                }
                            });
                            $scope.data = successRes.data.data.data;
                            checkUnClosingOut();
                            angular.extend($scope.paged, successRes.data.data);
                        }
                    });
                }
            };

            $scope.checkDieuChuyen = function (maDonViNhan, maDonViXuat) {
                return maDonViNhan === maDonViXuat;
            }
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
                securityService.getAccessList('phieuDieuChuyenNoiBo').then(function (successRes) {
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

            $scope.setPage = function (pageNo) {
                $scope.paged.currentPage = pageNo;
                filterData();
            };
            $scope.sortType = 'maNvDieuChuyenNoiBo';
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
                    return 'Phiếu xuất chuyển kho';
                }
                else {
                    return 'Phiếu xuất siêu thị thành viên';
                }
            };
            $scope.create = function () {
                var modalInstance = $uibModal.open({
                    templateUrl: configService.buildUrl('nv/NvPhieuDieuChuyenNoiBo', 'add'),
                    controller: 'phieuDieuChuyenNoiBoCreateController',
                    windowClass: 'app-modal-window',
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
                    templateUrl: configService.buildUrl('nv/NvPhieuDieuChuyenNoiBo', 'update'),
                    controller: 'phieuDieuChuyenNoiBoEditController',
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
                    templateUrl: configService.buildUrl('nv/NvPhieuDieuChuyenNoiBo', 'details'),
                    controller: 'phieuDieuChuyenNoiBoDetailsController',
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
                    templateUrl: configService.buildUrl('nv/NvPhieuDieuChuyenNoiBo', 'printItem'),
                    controller: 'phieuDieuChuyenNoiBoExportItemController',
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
                    templateUrl: configService.buildUrl('nv/NvPhieuDieuChuyenNoiBo', 'delete'),
                    controller: 'phieuDieuChuyenNoiBoDeleteController',
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
                    controller: 'phieuDieuChuyenNoiBoBoCreateController',
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
    app.controller('phieuDieuChuyenNoiBoCreateController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'phieuDieuChuyenNoiBoService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', '$rootScope', 'userService', 'FileUploader', 'merchandiseService', 'customerService', 'merchandiseTypeService', 'nhomVatTuService', 'supplierService', 'wareHouseService', 'packagingService', 'taxService', 'donViTinhService', 'toaster', 'periodService', 'objectFilter', 'AuDonViService', 'phieuNhapHangMuaService',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, $rootScope, serviceAuthUser, FileUploader, serviceMerchandise, serviceCustomer, serviceMerchandiseType, serviceNhomVatTu, serviceSupplier, serviceWareHouse, servicePackaging, serviceTax, serviceDonViTinh, toaster, servicePeriod, objectFilter, serviceAuthDonVi, phieuNhapHangMuaService) {
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
            $scope.donHangs = [];
            $scope.tkKtKhoNhap = "";
            $scope.invalid = "";
            $scope.tyGia = 0;
            var taxRate = 0;
            $scope.checkExistPhieu = false;
            $scope.isListItemNull = true;
            $scope.tempData = tempDataService.tempData;
            $scope.isLoading = false;
            $scope.isSameUnitUser = true;
            var tableName = null;
            var targetObj = null;
            $scope.displayHepler = function (paraValue, moduleName) {
                var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                if (data && data.length === 1) {
                    return data[0].text;
                } else {
                    return paraValue;
                }
            };
            $scope.$watch("target.maDonViNhan", function (newValue, oldValue) {
                var maDonViNhan = $scope.target.maDonViNhan;
                service.getCurrentUser(function (response) {
                    var maDonViXuat = response.unitUser;
                    if (maDonViNhan === maDonViXuat) {
                        $scope.isSameUnitUser = true;
                    }
                    else {
                        $scope.isSameUnitUser = false;
                    }
                });
            }, true);


            $scope.changeDonViNhan = function (maDonViNhan) {
                $scope.target.maKhoNhap = '';
                serviceWareHouse.getByUnit(maDonViNhan).then(function (successRes) {
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
            function loadNhapMua() {
                if (!tempDataService.tempData('nhapMuas')) {
                    phieuNhapHangMuaService.getAll_NhapMua().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                            tempDataService.putTempData('nhapMuas', successRes.data.data);
                            $scope.nhapMuas = successRes.data.data;
                        } else {
                            console.log('successRes', successRes);
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.nhapMuas = tempDataService.tempData('nhapMuas');
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
            loadNhapMua();
            loadDonViTinh();
            loadCurrentWareHouse();
            //end 
            $scope.title = function () {
                if ($scope.isSameUnitUser) {
                    return 'Phiếu xuất chuyển kho';
                }
                else {
                    return 'Phiếu xuất siêu thị thành viên';
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
            $scope.loadDataByNhapMua = function (lenhDieuDong) {
                var dataId = $filter('filter')($scope.nhapMuas, { value: lenhDieuDong }, true);
                service.getNewInstanceFrom(lenhDieuDong, function (response) {
                    $scope.target = response;
                    $scope.pageChanged();
                    $scope.target.ngayCT = new Date(response.ngayCT);
                    $scope.target.ngayDieuDong = new Date(response.ngayDieuDong);

                });
                if (dataId && dataId.length === 1) {
                    phieuNhapHangMuaService.getDetails(dataId[0].id).then(function (response) {
                        if (response && response.status == 200 && response.data) {
                            $scope.target.maKhoXuat = response.data.maKhoNhap;
                            $scope.target.lenhDieuDong = response.data.maChungTu;
                            if ($scope.target.dataDetails.length > 0) {
                                $scope.isListItemNull = false;
                                $scope.target.dataDetails.forEach(function (obj) {
                                    obj.tyLeVatVao = taxRate;
                                    obj.giaMuaCoVat = obj.donGia * (1 + taxRate / 100);
                                    obj.thanhTienVAT = obj.soLuong * obj.giaMuaCoVat;
                                });
                            }
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
            function getNew(maChungTu) {
                service.getPhieuByLenhDieuDong(maChungTu).then(function (response) {
                    if (response && response.status == 200 && response.data) {
                        $scope.checkExistPhieu = true;
                        $scope.codePromissoryNote = response.data;
                    }
                });
                service.getNewInstanceFrom(maChungTu, function (response) {
                    console.log(response);
                    $scope.target = response;
                    if ($scope.target.vat != null) {
                        serviceTax.getTaxByCode($scope.target.vat).then(function (response) {
                            if (response.status == 200) {
                                taxRate = response.data.taxRate;
                                if ($scope.target.dataDetails.length > 0) {
                                    $scope.isListItemNull = false;
                                    $scope.target.dataDetails.forEach(function (obj) {
                                        var targetObj = {};
                                        obj.vat = $scope.target.vat;
                                        obj.tyLeVatVao = taxRate;
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
                                                if (obj.soLuongTonXuat < obj.soLuong) {
                                                    $scope.invalid = "selectFail";
                                                }
                                            }
                                        });
                                    });
                                }
                            }
                        });
                    }
                    if (objectFilter.loaiPhieu == 'DCN') { // nếu là điều chuyển nhận lấy ra giá trị tồn
                        $scope.target.dataDetails.forEach(function (obj) {
                            var targetObj = {};
                            obj.vat = $scope.target.vat;
                            obj.tyLeVatVao = taxRate;
                            obj.giaMuaCoVat = obj.donGia * (1 + taxRate / 100);
                            obj.thanhTienVAT = obj.soLuong * obj.giaMuaCoVat;
                            targetObj.fromDate = $filter('date')($scope.target.ngayCT, 'yyyy-MM-dd');
                            targetObj.toDate = $filter('date')($scope.target.ngayCT, 'yyyy-MM-dd');
                            targetObj.merchandiseCodes = obj.maHang;
                            targetObj.wareHouseCodes = objectFilter.maKhoXuat;
                            targetObj.wareHouseRecieveCode = objectFilter.maKhoNhap;
                            servicePeriod.getSoLuongTonByDate(targetObj).then(function (response) {
                                if (response && response.status === 200 && response.data.data) {
                                    obj.soLuongTonXuat = response.data.data.soLuongTonKhoXuat;
                                    obj.soLuongTonNhap = response.data.data.soLuongTonKhoNhap;
                                    if (obj.soLuongTonXuat < obj.soLuong) {
                                        $scope.invalid = "selectFail";
                                    }
                                }
                            });
                        });
                        $scope.target.maKhoXuat = objectFilter.maKhoXuat
                        $scope.target.maKhoNhap = objectFilter.maKhoNhap;
                    }
                    $scope.pageChanged();
                    $scope.target.ngayCT = new Date(response.ngayCT);
                    $scope.target.ngayDieuDong = new Date(response.ngayDieuDong);
                    var data = $filter('filter')($scope.wareHouses, { value: objectFilter.maKhoXuat }, true);
                    if (data && data.length == 1) {
                        $scope.target.maKhoXuat = data[0].value;
                    }
                    $scope.target.lenhDieuDong = objectFilter.maChungTu;
                });
            };

            $scope.displayDVT = function (paraValue, moduleName) {
                if (paraValue) {
                    var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                    if (data && data.length === 1) {
                        return data[0].description;
                    } else {
                        return paraValue;
                    }
                }
            }

            function filterData() {
                var maChungTu = "";
                $scope.isLoading = true;
                if (objectFilter && objectFilter.maChungTu) {
                    maChungTu = objectFilter.maChungTu;
                    getNew(maChungTu);
                } else {
                    $scope.checkExistPhieu = false;
                    service.getNewInstance(function (response) {
                        $scope.target = response;
                        servicePeriod.getKyKeToan().then(function (response) {
                            if (response && response.status == 200 && response.data) {
                                targetObj = angular.copy(response.data);
                                $scope.target.ngayCT = new Date(response.data.toDate);
                                $scope.target.ngayDieuDong = new Date(response.data.toDate);
                                targetObj.fromDate = $filter('date')(response.data.toDate, 'yyyy-MM-dd');
                                targetObj.toDate = $filter('date')(response.data.toDate, 'yyyy-MM-dd');
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

            function checkExistSelectMaHang(item) {
                $scope.target.dataDetails.forEach(function (obj) {
                    if (obj.maHang == item) {
                        ngNotify.set(obj.maHang + " số lượng : " + obj.soLuong, { type: 'success' });
                    }
                });
            }


            function checkExistMaHang(item) {
                $scope.target.dataDetails.forEach(function (obj) {
                    if (obj.maHang == item.maHang) {
                        if (obj.soLuongTonXuat < (obj.soLuong + item.soLuong)) {
                            focus('soluong');
                            document.getElementById('soluong').focus();
                            // toaster.pop('error', "Lỗi:", "Không cộng gộp được !");
                            toaster.pop({
                                type: 'error',
                                title: 'Lỗi:',
                                body: 'Không cộng gộp được !',
                                timeout: 1000
                            });
                        }
                        else {
                            obj.soLuong += item.soLuong;
                            obj.soLuongLe += item.soLuongLe;
                            obj.soLuongBao += item.soLuongBao;
                            obj.thanhTien += item.thanhTien;
                            obj.thanhTienVAT += item.thanhTienVAT;
                            toaster.pop({
                                type: 'success',
                                title: 'Thành công:',
                                body: 'Cộng gộp thành công !',
                                timeout: 1000
                            });
                        }
                        $scope.statusChange = true;
                    }
                    else {
                        $scope.statusChange = false;
                    }
                });
            }

            $scope.addRow = function () {
                if ($scope.target.dataDetails) {
                    checkExistMaHang($scope.newItem);
                    if ($scope.statusChange) {
                        return;
                    }
                }
                if (!$scope.newItem.soLuong || $scope.newItem.soLuong < 1 || $scope.newItem.soLuong > $scope.newItem.soLuongTonXuat) {
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
                    service.postItemInventoryByCode({ code: $scope.newItem.maHang, wareHouseCode: $scope.target.maKhoXuat }, function (response) {
                        $scope.newItem.soLuongTon = response.closingQuantity;
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
                                advanceData: { withGiaVon: true, maKho: $scope.target.maKhoXuat },
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
                                    $scope.newItem.validateCode = updatedData.maHang;
                                } else {
                                    $scope.newItem.donGia = response.data.data.giaVon;
                                    $scope.robot.changeDonGia($scope.newItem);
                                    $scope.newItem.validateCode = updatedData.maHang;
                                }
                            }
                        });
                        //-------------------------
                    }
                    $scope.pageChanged();
                }, function () {
                });
            };
            $scope.getWareHouseImportByUnit = function () {
                if ($scope.target.maDonViNhan) {
                    service.getWareHouseByUnit($scope.target.maDonViNhan, function (response) {
                        if (response && response.length > 0)
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
                        if (response && response.status == 200 && response.data.status) {
                            targetObj.merchandiseCodes = code;
                            targetObj.wareHouseCodes = $scope.target.maKhoXuat;
                            targetObj.wareHouseRecieveCode = $scope.target.maKhoNhap;
                            servicePeriod.getSoLuongTonByDate(targetObj).then(function (response) {
                                if (response && response.status === 200 && response.data.data) {
                                    $scope.newItem.soLuongTonXuat = response.data.data.soLuongTonKhoXuat;
                                    $scope.newItem.soLuongTonNhap = response.data.data.soLuongTonKhoNhap;
                                }
                            });
                            $scope.newItem = response.data.data;
                            $scope.newItem.vat = response.data.data.maVatVao;
                            checkExistSelectMaHang($scope.newItem.maHang);
                            if ($scope.isSameUnitUser) {
                                $scope.newItem.donGia = response.data.data.giaVon;
                                $scope.robot.changeDonGia($scope.newItem);
                            } else {
                                $scope.newItem.donGia = response.data.data.giaVon;
                                $scope.robot.changeDonGia($scope.newItem);
                            }
                            $scope.newItem.validateCode = response.data.data.maHang;
                        } else {
                            $scope.addNewItem(code);
                        }
                        focus('soluong');
                        document.getElementById('soluong').focus();
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
                var check = true;
                $scope.target.dataDetails.forEach(function (obj) {
                    if (obj.soLuong > obj.soLuongTonXuat) {
                        ngNotify.set("Kiểm tra lại số lượng nhập !", { type: 'error' });
                        check = false;
                        return;
                    }
                });
                if (check) {
                    $scope.target.ngayCT = $filter('date')($scope.target.ngayCT);
                    $scope.target.ngayDieuDong = $filter('date')($scope.target.ngayDieuDong);
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
                }
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
            $scope.createNhapMua = function (target, name) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('nv/NvNhapHangMua', 'add'),
                    controller: 'phieuNhapHangMuaCreateController',
                    resolve: {}
                });
                modalInstance.result.then(function (updatedData) {
                    phieuNhapHangMuaService.getAll_NhapMua().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                            tempDataService.putTempData('nhapMuas', successRes.data.data);
                            $scope.nhapMuas = successRes.data.data;
                            target[name] = updatedData.maChungTu;
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
    app.controller('phieuDieuChuyenNoiBoEditController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'phieuDieuChuyenNoiBoService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify', 'merchandiseService', 'toaster', 'AuDonViService', 'taxService', 'periodService', 'userService', 'wareHouseService',
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
            $scope.checkVAT = false;
            $scope.isSameUnitUser = true;
            $scope.tyGia = 0;
            var taxRate = 0;
            $scope.isLoading = true;
            $scope.title = function () {
                if ($scope.isSameUnitUser) {
                    return 'Cập nhật phiếu xuất chuyển kho';
                }
                else {
                    return 'Cập nhật phiếu xuất siêu thị thành viên';
                }
            };
            $scope.$watch("target.maDonViNhan", function (newValue, oldValue) {
                var maDonViNhan = $scope.target.maDonViNhan;
                service.getCurrentUser(function (response) {
                    var maDonViXuat = response.unitUser;
                    if (maDonViNhan === maDonViXuat) {
                        $scope.isSameUnitUser = true;
                    }
                    else {
                        $scope.isSameUnitUser = false;
                    }
                });
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
                if ($scope.target.vat != null) { // Nếu phiếu có VAT thì filter lại giá vat ...
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
            $scope.displayDVT = function (paraValue, moduleName) {
                if (paraValue) {
                    var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                    if (data && data.length === 1) {
                        return data[0].description;
                    } else {
                        return paraValue;
                    }
                }
            }
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

            function checkExistSelectMaHang(item) {
                $scope.target.dataDetails.forEach(function (obj) {
                    if (obj.maHang == item) {
                        ngNotify.set(obj.maHang + " số lượng : " + obj.soLuong, { type: 'success' });
                    }
                });
            }

            $scope.changeDonViNhan = function (maDonViNhan) {
                $scope.target.maKhoNhap = '';
                serviceWareHouse.getByUnit(maDonViNhan).then(function (successRes) {
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
                    serviceWareHouse.getByUnit($scope.target.maDonViNhan).then(function (successRes) {
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

            function checkExistMaHang(item) {
                $scope.target.dataDetails.forEach(function (obj) {
                    if (obj.maHang == item.maHang) {
                        if (obj.soLuongTonXuat < (obj.soLuong + item.soLuong)) {
                            focus('mahang');
                            document.getElementById('mahang').focus();
                            //toaster.pop('error', "Lỗi:", "Không cộng gộp được !");
                            toaster.pop({
                                type: 'error',
                                title: 'Lỗi:',
                                body: 'Không cộng gộp được !',
                                timeout: 1000
                            });
                        }
                        else {
                            obj.soLuong += item.soLuong;
                            obj.soLuongLe += item.soLuongLe;
                            obj.soLuongBao += item.soLuongBao;
                            obj.thanhTien += item.thanhTien;
                            obj.thanhTienVAT += item.thanhTienVAT;
                            toaster.pop('success', "Thành công:", "Cộng gộp thành công !");
                        }
                        $scope.statusChange = true;
                    }
                    else {
                        $scope.statusChange = false;
                    }
                });
            }
            $scope.addRow = function () {
                if ($scope.target.dataDetails) {
                    checkExistMaHang($scope.newItem)
                    if ($scope.statusChange) {
                        return;
                    }
                }
                if (!$scope.newItem.soLuong || $scope.newItem.soLuong < 1) {
                    focus('soluong');
                    document.getElementById('soluong').focus();
                    toaster.pop('error', "Lỗi:", "Nhập sai số lượng !");
                    return;
                }
                if ($scope.newItem.validateCode == $scope.newItem.maHang) {
                    service.postItemInventoryByCode({ code: $scope.newItem.maHang, wareHouseCode: $scope.target.maKhoXuat }, function (response) {
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
                                advanceData: { withGiaVon: true, maKho: $scope.target.maKhoXuat },
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
                                    $scope.newItem.donGia = response.data.data.giaVon;
                                    $scope.robot.changeDonGia($scope.newItem);
                                }
                                console.log($scope.newItem.donGia);
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
                        if (response.data.status) {
                            var targetObj = {};
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
                            $scope.newItem = response.data.data;
                            $scope.newItem.VAT = response.data.data.maVatVao;
                            checkExistSelectMaHang($scope.newItem.maHang);
                            if ($scope.isSameUnitUser) {
                                $scope.newItem.donGia = response.data.data.giaVon;
                                $scope.robot.changeDonGia($scope.newItem);
                            } else {
                                $scope.newItem.donGia = response.data.data.giaVon;
                                $scope.robot.changeDonGia($scope.newItem);
                            }
                            $scope.newItem.validateCode = response.data.data.maHang;
                        } else {
                            $scope.addNewItem(code);
                        }
                        focus('soluong');
                        document.getElementById('soluong').focus();
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
                var check = true;
                $scope.target.dataDetails.forEach(function (obj) {
                    if (obj.soLuong > obj.soLuongTonXuat) {
                        ngNotify.set("Kiểm tra lại số lượng nhập !", { type: 'error' });
                        check = false;
                        return;
                    }
                });
                if (check) {
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
                }
            };
            $scope.cancel = function () {
                $uibModalInstance.close();
            };
        }]);
    /* controller Details */
    app.controller('phieuDieuChuyenNoiBoDetailsController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'phieuDieuChuyenNoiBoService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify', 'merchandiseService', 'taxService', 'periodService', 'userService', 'toaster',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify, serviceMerchandise, serviceTax, servicePeriod, serviceAuthUser, toaster) {
            var currentUser = serviceAuthUser.GetCurrentUser();
            var unitCode = currentUser.unitCode;
            $scope.config = angular.copy(configService);
            $scope.robot = angular.copy(service.robot);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.tempData = tempDataService.tempData;
            $scope.target = targetData;
            var trangThai = $scope.target.trangThai;
            var thanhTienSauVat = $scope.target.thanhTienSauVat;
            $scope.isSameUnitUser = true;
            $scope.khoNhaps = [];
            $scope.khoXuats = [];
            var taxRate = 0;
            $scope.target.ngayCT = new Date(targetData.ngayCT);
            $scope.isLoading = false;
            $scope.title = function () {
                if ($scope.isSameUnitUser) {
                    return 'Thông tin phiếu xuất chuyển kho';
                }
                else {
                    return 'Thông tin phiếu xuất siêu thị thành viên';
                }
            };
            $scope.$watch("target.maDonViNhan", function (newValue, oldValue) {
                var maDonViNhan = $scope.target.maDonViNhan;
                service.getCurrentUser(function (response) {
                    var maDonViXuat = response.unitUser;
                    if (maDonViNhan === maDonViXuat) {
                        $scope.isSameUnitUser = true;
                    }
                    else {
                        $scope.isSameUnitUser = false;
                    }
                });
            }, true);
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
                if ($scope.target.maDonViNhan) {
                    service.getWareHouseByUnit($scope.target.maDonViNhan, function (response) {
                        $scope.khoNhaps = response;
                        var data = $filter('filter')($scope.khoNhaps, { value: $scope.target.maKhoNhap }, true);
                        if (data && data.length == 1) {
                            $scope.target.maKhoNhap = data[0];
                        }
                    });
                };
                if ($scope.target.maDonViXuat) {
                    service.getWareHouseByUnit($scope.target.maDonViXuat, function (response) {
                        $scope.khoXuats = response;
                        var data = $filter('filter')($scope.khoXuats, { value: $scope.target.maKhoXuat }, true);
                        if (data && data.length == 1) {
                            $scope.target.maKhoXuat = data[0];
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
            $scope.displayDVT = function (paraValue, moduleName) {
                if (paraValue) {
                    var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                    if (data && data.length === 1) {
                        return data[0].description;
                    } else {
                        return paraValue;
                    }
                }
            }
            //note
            $scope.displayHepler = function (paraValue, moduleName) {
                var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                if (data && data.length === 1) {
                    return data[0].text;
                } else {
                    return paraValue;
                }
            }
            $scope.approval = function () {
                if (!$scope.target || $scope.target.dataDetails.length <= 0) {
                    toaster.pop('error', "Lỗi:", "Không có chi tiết phiếu !");
                }
                else {
                    if ($scope.target.maKhoXuat.includes(unitCode) && !$scope.target.maDonViNhan.includes(unitCode) && !$scope.target.maKhoNhap.includes(unitCode)) {
                        var cf = confirm("Bạn đang điều chuyển hàng tới siêu thị " + $scope.displayHepler($scope.target.maDonViNhan, 'auDonVis') + " bạn có muốn tiếp tục ?");
                        if (cf) {
                            //Check connect server
                            service.checkConnectServer($scope.target.maDonViNhan).then(function (successRes) {
                                if (successRes && successRes.status === 200 && successRes.data) {
                                    service.postApproval($scope.target).then(function (response) {
                                        if (response && response.status === 200 && response.data && response.data.data === "Success") {
                                            toaster.pop({
                                                type: 'success',
                                                title: 'Thành công',
                                                body: response.data.message,
                                                timeout: 3000
                                            });
                                            $uibModalInstance.close($scope.target);
                                            $scope.goIndex = function () {
                                                $state.go('nvDieuChuyenNoiBo');
                                            };
                                        }
                                        else if (response && response.status === 200 && response.data && response.data.data === "NoPeriod") {
                                            toaster.pop({
                                                type: 'error',
                                                title: 'Không thành công',
                                                body: response.data.message,
                                                timeout: 3000
                                            });
                                            $uibModalInstance.close($scope.target);
                                        }
                                        else if (response && response.status === 200 && response.data && response.data.data === "Complete") {
                                            toaster.pop({
                                                type: 'success',
                                                title: 'Thành công',
                                                body: response.data.message,
                                                timeout: 3000
                                            });
                                            $uibModalInstance.close($scope.target);
                                        }
                                        else if (response && response.status === 200 && response.data && response.data.data === "Failed") {
                                            toaster.pop({
                                                type: 'error',
                                                title: 'Lỗi',
                                                body: response.data.message,
                                                timeout: 3000
                                            });
                                            $uibModalInstance.close($scope.target);
                                        }
                                        else {
                                            toaster.pop({
                                                type: 'error',
                                                title: 'Lỗi',
                                                body: "Thất bại! - Xảy ra lỗi hoặc phiếu này đã duyệt",
                                                timeout: 3000
                                            });
                                        }
                                    });
                                } else {
                                    toaster.pop('error', "Lỗi:", "Không thể kết nối tới máy chủ siêu thị " + $scope.displayHepler($scope.target.maDonViNhan, 'auDonVis') + "! Hiện tại chưa thể duyệt phiếu");
                                }
                            }, function (errorRes) {
                                toaster.pop('error', "Lỗi:", "Xảy ra lỗi");
                            });
                        }
                    }
                    else {
                        service.postApproval($scope.target).then(function (response) {
                            if (response && response.status === 200 && response.data && response.data.data === "Complete") {
                                toaster.pop({
                                    type: 'success',
                                    title: 'Thành công',
                                    body: "Duyệt thành công!",
                                    timeout: 3000
                                });
                                $uibModalInstance.close($scope.target);
                                $scope.goIndex = function () {
                                    $state.go('nvDieuChuyenNoiBo');
                                };
                            }
                            else if (response && response.status === 200 && response.data && response.data.data === "Failed") {
                                toaster.pop({
                                    type: 'error',
                                    title: 'Lỗi',
                                    body: response.data.message,
                                    timeout: 3000
                                });
                                $uibModalInstance.close($scope.target);
                            }
                            else if (response && response.status === 200 && response.data && response.data.data === "NoPeriod") {
                                toaster.pop({
                                    type: 'error',
                                    title: 'Không thành công',
                                    body: response.data.message,
                                    timeout: 3000
                                });
                                $uibModalInstance.close($scope.target);
                            }
                            else {
                                toaster.pop({
                                    type: 'error',
                                    title: 'Lỗi',
                                    body: "Thất bại! - Xảy ra lỗi hoặc phiếu này đã duyệt",
                                    timeout: 3000
                                });
                            }
                        });
                    }
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
            };
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
            fillterData();
            $scope.cancel = function () {
                $uibModalInstance.close();
            };

        }]);
    /* controller delete */
    app.controller('phieuDieuChuyenNoiBoDeleteController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'phieuDieuChuyenNoiBoService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify',
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

    app.controller('reportPhieuDieuChuyenNoiBoController', ['$scope', '$location', '$http', 'configService', 'phieuDieuChuyenNoiBoService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'userService', '$stateParams', '$window', 'taxService',
        function ($scope, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, serviceAuthUser, $stateParams, $window, serviceTax) {
            var currentUser = serviceAuthUser.GetCurrentUser();
            $scope.robot = angular.copy(service.robot);
            $scope.tempData = tempDataService.tempData;
            var id = $stateParams.id;
            $scope.target = {};
            var taxRate = 0;
            $scope.goIndex = function () {
                $state.go('nvDieuChuyenNoiBo');
            }
            function filterData() {
                if (id) {
                    service.getReport(id).then(function (response) {
                        console.log('response', response);
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
                                        angular.forEach($scope.target.dataReportDetails, function (v, k) {
                                            if ($scope.target.dataReportDetails[k].barcode) {
                                                if ($scope.target.dataReportDetails[k].barcode.length > 1) {
                                                    var b = $scope.target.dataReportDetails[k].barcode.split(';');
                                                    for (var i = b.length - 1; i >= 0; i--) {
                                                        if (b[i]) {
                                                            $scope.target.dataReportDetails[k].barcode = b[i];
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
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
                                angular.forEach($scope.target.dataReportDetails, function (v, k) {
                                    if ($scope.target.dataReportDetails[k].barcode) {
                                        if ($scope.target.dataReportDetails[k].barcode.length > 1) {
                                            var b = $scope.target.dataReportDetails[k].barcode.split(';');
                                            $scope.target.dataReportDetails[k].barcode = b[b.length - 2];
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
    return app;
});