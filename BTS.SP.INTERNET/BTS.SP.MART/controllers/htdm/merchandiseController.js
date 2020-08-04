/*  
* Người tạo : Nguyễn Tuấn Hoàng Anh
* View: BTS.SP.MART/views/htdm/MatHang
* Vm sevices: BTS.API.SERVICE -> MD ->MdMerchandiseVm.cs
* Sevices: BTS.API.SERVICE -> MD -> MdMerchandiseService.cs
* Entity: BTS.API.ENTITY -> Md - > MdMerchandise.cs
* Menu: Danh mục-> Danh mục mặt hàng
*/
define(['ui-bootstrap', '/BTS.SP.MART/controllers/auth/AuthController.js', '/BTS.SP.MART/controllers/auth/AuDonVi.js', '/BTS.SP.MART/controllers/htdm/supplierController.js', '/BTS.SP.MART/controllers/htdm/merchandiseTypeController.js', '/BTS.SP.MART/controllers/htdm/nhomVatTuController.js', '/BTS.SP.MART/controllers/htdm/packagingController.js', '/BTS.SP.MART/controllers/htdm/taxController.js', '/BTS.SP.MART/controllers/htdm/donViTinhController.js', '/BTS.SP.MART/controllers/htdm/shelvesController.js', '/BTS.SP.MART/controllers/htdm/sizeController.js', '/BTS.SP.MART/controllers/htdm/colorController.js', '/BTS.SP.MART/controllers/htdm/wareHouseController.js'], function () {
    'use strict';
    var app = angular.module('merchandiseModule', ['ui.bootstrap', 'authModule', 'AuDonViModule', 'supplierModule', 'merchandiseTypeModule', 'nhomVatTuModule', 'packagingModule', 'taxModule', 'donViTinhModule', 'shelvesModule', 'sizeModule', 'colorModule', 'wareHouseModule', 'ngNotify']);
    app.factory('merchandiseService', ['$http', 'configService', function ($http, configService, ngNotify) {
        var serviceUrl = configService.rootUrlWebApi + '/Md/Merchandise';
        var selectedData = [];
        var result = {};
        var calc = {
            changeCanDienTu: function (target) {
                if (target.isCanDienTu) {
                    result.getNewCanDienTu().then(function (response) {
                        if (response && response.status === 200 && response.data) {
                            target.itemCode = response.data;
                        }
                    });
                } else {
                    target.itemCode = "";
                }
            },
            changeCanDienTuRoot: function (target) {
                if (target.isCanDienTu) {
                    result.getNewCanDienTuOracleRoot().then(function (response) {
                        console.log('can:', response);
                        if (response && response.status === 200 && response.data) {
                            target.itemCode = response.data;
                        }
                    });
                } else {
                    target.itemCode = "";
                }
            },
            changeCanDienTuFromSQL: function (target) {
                if (target.isCanDienTu) {
                    result.getNewCanDienTuFromSQL().then(function (response) {
                        if (response && response.status === 200 && response.data) {
                            target.itemcode = response.data;
                        }
                    });
                } else {
                    target.itemcode = "";
                }

            },
            changeGiaMuaVat: function (target) {
                target.giaMua = target.giaMuaVat / ((target.tyLeVatVao / 100) + 1);

                target.giaBanLe = target.giaMua * target.tyLeLaiLe / 100 + target.giaMua;
                target.giaBanBuon = target.giaMua * target.tyLeLaiBuon / 100 + target.giaMua;

                target.giaBanLeVat = Math.round(target.giaBanLe * target.tyLeVatRa / 100 + target.giaBanLe);
                target.giaBanBuonVat = Math.round(target.giaBanBuon * target.tyLeVatRa / 100 + target.giaBanBuon);
            },
            changeGiaMua: function (target) {
                target.giaMuaVat = target.giaMua * target.tyLeVatVao / 100 + target.giaMua;
                target.giaBanLe = target.giaMua * target.tyLeLaiLe / 100 + target.giaMua;
                target.giaBanBuon = target.giaMua * target.tyLeLaiBuon / 100 + target.giaMua;
                target.giaBanLeVat = Math.round(target.giaBanLe * target.tyLeVatRa / 100 + target.giaBanLe);
                target.giaBanBuonVat = Math.round(target.giaBanBuon * target.tyLeVatRa / 100 + target.giaBanBuon);
            },
            changeGiaBanLe: function (target) {
                target.tyLeLaiLe = 100 * (target.giaBanLe - target.giaMua) / target.giaMua;
                if (parseInt(target.tyLeLaiLe) <= -100 || parseInt(target.tyLeLaiLe) > 1000) {
                    ngNotify.set("Kiểm tra lại tỷ lệ lãi lẻ (quá lớn hoặc quá nhỏ)", { duration: 1500, type: 'error' });
                }
                target.giaBanLeVat = Math.round(target.giaBanLe * target.tyLeVatRa / 100 + target.giaBanLe);
            },
            changeGiaBanBuon: function (target) {
                target.tyLeLaiBuon = 100 * (target.giaBanBuon - target.giaMua) / target.giaMua;
                target.giaBanBuonVat = Math.round(target.giaBanBuon * target.tyLeVatRa / 100 + target.giaBanBuon);
                if (parseInt(target.tyLeLaiBuon) <= -100 || parseInt(target.tyLeLaiBuon) > 1000) {
                    ngNotify.set("Kiểm tra lại tỷ lệ lãi buôn (quá lớn hoặc quá nhỏ)", { duration: 1500, type: 'error' });
                }
            },
            changeTyLeLaiLe: function (target) {
                target.giaBanLe = target.giaMua * target.tyLeLaiLe / 100 + target.giaMua;
                target.giaBanLeVat = Math.round(target.giaBanLe * target.tyLeVatRa / 100 + target.giaBanLe);

            },
            changeTyLeLaiBuon: function (target) {
                target.giaBanBuon = target.giaMua * target.tyLeLaiBuon / 100 + target.giaMua;
                target.giaBanBuonVat = Math.round(target.giaBanBuon * target.tyLeVatRa / 100 + target.giaBanBuon);
            },
            changGiaBanLeVat: function (target) {
                target.giaBanLe = target.giaBanLeVat / (1 + target.tyLeVatRa / 100);
                target.tyLeLaiLe = 100 * (target.giaBanLe - target.giaMua) / target.giaMua;
                if (parseInt(target.tyLeLaiLe) <= -100 || parseInt(target.tyLeLaiLe) > 1000) {
                    ngNotify.set("Kiểm tra lại tỷ lệ lãi lẻ (quá lớn hoặc quá nhỏ)", { duration: 1500, type: 'error' });
                }
            },
            changeGiaBanBuonVat: function (target) {
                target.giaBanBuon = target.giaBanBuonVat / (1 + target.tyLeVatRa / 100);
                target.tyLeLaiBuon = 100 * (target.giaBanBuon - target.giaMua) / target.giaMua;
                if (parseInt(target.tyLeLaiBuon) <= -100 || parseInt(target.tyLeLaiBuon) > 1000) {
                    ngNotify.set("Kiểm tra lại tỷ lệ lãi buôn (quá lớn hoặc quá nhỏ)", { duration: 1500, type: 'error' });
                }
            }
        }
        result = {
            robot: calc,
            postQuery: function (data) {
                return $http.post(serviceUrl + '/PostQuery', data);
            },
            postSelectDataQuery: function (data) {
                return $http.post(serviceUrl + '/PostSelectDataQuery', data);
            },
            postSelectDataQueryAndPromotion: function (data) {
                return $http.post(serviceUrl + '/PostSelectDataQueryAndPromotion', data);
            },
            postSelectDataSQLQuery: function (data) {
                return $http.post(serviceUrl + '/PostSelectDataSQLQuery', data);
            },
            postSelectDataServerRoot: function (data) {
                return $http.post(serviceUrl + '/PostSelectDataServerRoot', data);
            },
            postSelectData: function (data) {
                return $http.post(serviceUrl + '/PostSelectData', data);
            },
            postQueryDetail: function (data) {
                return $http.post(serviceUrl + '/PostQueryDetail', data);
            },
            post: function (data) {
                return $http.post(serviceUrl + '/Post', data);
            },
            postMatHangToSQL: function (data) {
                return $http.post(serviceUrl + '/PostMatHangToSQL', data);
            },
            postMatHangToOracleRoot: function (data) {
                return $http.post(serviceUrl + '/PostMatHangToOracleRoot', data);
            },
            //Lấy dữ liệu lên đơn vị cha
            postAsyncFromOracleRoot: function (data) {
                return $http.post(serviceUrl + '/PostAsyncFromOracleRoot', data);
            },
            //end
            postAsyncCompareUpdate: function (data) {
                return $http.post(serviceUrl + '/PostAsyncCompareUpdate', data);
            },
            update: function (params) {
                return $http.put(serviceUrl + '/' + params.id, params);
            },
            updateCodeGroup: function (params) {
                return $http.post(serviceUrl + '/UpdateCodeGroup', params);
            },
            updateMatHangToOracleRoot: function (params) {
                return $http.put(serviceUrl + '/UpdateMatHangToOracleRoot/' + params.id, params);
            },
            updatePrice: function (params) {
                return $http.put(serviceUrl + '/PutMerchandisePrice/' + params.id, params);
            },
            deleteItem: function (params) {
                return $http.delete(serviceUrl + '/' + params.id, params);
            },
            getSelectByMaLoai: function (code) {
                return $http.get(configService.rootUrlWebApi + '/Md/NhomVatTu/GetSelectByMaLoai/' + code);
            },
            getSelectByMaLoaiRoot: function (code) {
                return $http.get(configService.rootUrlWebApi + '/Md/NhomVatTu/GetSelectByMaLoaiRoot/' + code);
            },
            getAllTypeMerchandise: function (callback) {
                return $http.get(configService.rootUrlWebApi + '/Md/MerchandiseType/GetSelectData');
            },
            getDetailByCode: function (code) {
                return $http.get(serviceUrl + '/GetDetailByCode/' + code);
            },
            getDetailByCodeRoot: function (code) {
                return $http.get(serviceUrl + '/GetDetailByCodeRoot/' + code);
            },
            getNewCode: function (code) {
                return $http.get(serviceUrl + '/GetNewCode/' + code);
            },
            getNewCodeRoot: function (code) {
                return $http.get(serviceUrl + '/GetNewCodeRoot/' + code);
            },
            getNewCanDienTu: function () {
                return $http.get(serviceUrl + '/GetNewCanDienTu');
            },
            getNewCanDienTuFromSQL: function () {
                return $http.get(serviceUrl + '/GetNewCanDienTuFromSQL');
            },
            getNewCanDienTuOracleRoot: function () {
                return $http.get(serviceUrl + '/GetNewCanDienTuOracleRoot');
            },
            getBaoBiByCode: function (code) {
                return $http.get(configService.rootUrlWebApi + '/Md/Packaging/GetByCode/' + code);
            },
            writeDataToExcel: function (data) {
                return $http.post(serviceUrl + '/WriteDataToExcel', data);
            },
            getMerchandiseForNvByCode: function (code, wareHouseCode, unitCode) {
                return $http.get(serviceUrl + '/GetForNvByCode/' + code + '/' + wareHouseCode + '/' + unitCode);
            },
            getPrice: function (maVatTu) {
                return $http.get(serviceUrl + '/GetPrice/' + maVatTu);
            },
            getPriceFromSQL: function (maVatTu) {
                return $http.get(serviceUrl + '/GetPriceFromSQL/' + maVatTu);
            },
            getNhomVatTuFromSQLByMaLoai: function (code) {
                return $http.get(configService.rootUrlWebApi + '/Md/NhomVatTu/GetSelectDataFromSQLByMaLoai/' + code);
            },
            getLoaiVatTuFromSQL: function () {
                return $http.get(configService.rootUrlWebApi + '/Md/MerchandiseType/GetSelectDataFromSQL');
            },
            getKhachHangFromSQL: function () {
                return $http.get(configService.rootUrlWebApi + '/Md/Customer/GetSelectDataFromSQL');
            },
            getDonViTinhFromSQL: function () {
                return $http.get(configService.rootUrlWebApi + '/Md/Packaging/GetSelectDataFromSQL');
            },
            getKeHangFromSQL: function () {
                return $http.get(configService.rootUrlWebApi + '/Md/Shelves/GetSelectDataFromSQL');
            },
            getThueFromSQL: function () {
                return $http.get(configService.rootUrlWebApi + '/Md/Tax/GetSelectDataFromSQL');
            },
            getDetailThueFromSQL: function (code) {
                return $http.get(configService.rootUrlWebApi + '/Md/Tax/GetTaxFromSQL/' + code);
            },
            getNewCodeFromSQL: function (code) {
                return $http.get(serviceUrl + '/GetNewCodeFromSQL/' + code);
            },
            getByItemCodeNotNull: function (callback) {
                return $http.get(serviceUrl + '/GetByItemCodeNotNull');
            },
            getRootUnitCode: function () {
                return $http.get(serviceUrl + '/GetRootUnitCode');
            },
            postExportExcel: function (json) {
                return $http({
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
                    a.download = "HangHoa.xlsx";
                    a.click();
                }).error(function (data, status, headers, config) {
                    //upload failed
                });
            },
            getExcelTemplate: function (data) {
                return $http({
                    url: serviceUrl + '/ExportTemplateExcel',
                    method: "POST",
                    data: data,
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
                    a.download = "Template.xlsx";
                    a.click();
                }).error(function (data, status, headers, config) {
                    //upload failed
                });
            },
            filterMerchandiseCodes: function (maVatTu) {
                return $http.post(serviceUrl + '/FilterMerchandiseCodes/' + maVatTu);
            },
            getAllDataChild: function (maVatTu) {
                return $http.get(serviceUrl + '/GetAllDataChild/' + maVatTu);
            },
            getNewCodeChild: function (maVatTu) {
                return $http.get(serviceUrl + '/GetNewCodeChild/' + maVatTu);
            },
            postMerchandiseChild: function (data) {
                return $http.post(serviceUrl + '/PostMerchandiseChild', data);
            },
            getSelectData: function () {
                return selectedData;
            },
            setSelectData: function (array) {
                selectedData = array;
            },
            clearSelectData: function () {
                selectedData = [];
            }, 
            getForNvNhapMua: function (supplierCode, unitCode, summary) {
                if (summary) {
                    return $http.get(serviceUrl + '/GetForNvNhapMua/' + supplierCode + '/' + unitCode + '/' + summary);
                } else {
                    return $http.get(serviceUrl + '/GetForNvNhapMua/' + supplierCode + '/' + unitCode);
                }
            }
            //end service
        };
        return result;
    }]);
    /* controller list */
    app.controller('merchandiseController', ['$scope', 'configService', 'merchandiseService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'securityService', 'toaster', 'supplierService', 'merchandiseTypeService', 'nhomVatTuService', 'packagingService', 'taxService', 'donViTinhService', 'shelvesService', 'sizeService', 'colorService', 'AuDonViService', '$window',
        function ($scope, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, securityService, toaster, serviceSupplier, serviceMerchandiseType, serviceNhomVatTu, servicePackaging, serviceTax, serviceDonViTinh, serviceShelves, serviceSize, serviceColor, serviceAuthDonVi, $window) {
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.filtered = angular.copy(configService.filterDefault);
            $scope.tempData = tempDataService.tempData;
            $scope.isEditable = true;
            $scope.accessList = {};
            $scope.target = { options: 'maVatTu' };
            $scope.categories = [{
                value: 'barcode',
                text: 'Barcode'
            },
            {
                value: 'maVatTu',
                text: 'Mã hàng'
            },
            {
                value: 'tenHang',
                text: 'Tên hàng'
            },
            {
                value: 'maKhachHang',
                text: 'Mã NCC'
            },
            {
                value: 'giaBanLeVat',
                text: 'Giá bán lẻ VAT'
            },
            {
                value: 'giaMuaVat',
                text: 'Giá mua VAT'
            },
            {
                value: 'tyLeLaiLe',
                text: 'Tỷ lệ lãi lẻ'
            }];

            //load danh mục

            function loadSupplier() {
                if (!tempDataService.tempData('suppliers')) {
                    serviceSupplier.getAll_Supplier().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                            tempDataService.putTempData('suppliers', successRes.data.data);
                            $scope.suppliers = successRes.data.data;
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
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.nhomVatTus = tempDataService.tempData('nhomVatTus');
                }
            }

            function loadPackagings() {
                if (!tempDataService.tempData('packagings')) {
                    servicePackaging.getAll_Packaging().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                            tempDataService.putTempData('packagings', successRes.data.data);
                            $scope.packagings = successRes.data.data;
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
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.donViTinhs = tempDataService.tempData('donViTinhs');
                }
            }

            function loadeShelves() {
                if (!tempDataService.tempData('shelves')) {
                    serviceShelves.getAll_Shelves().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                            tempDataService.putTempData('shelves', successRes.data.data);
                            $scope.shelves = successRes.data.data;
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.shelves = tempDataService.tempData('shelves');
                }
            }

            function loadeSize() {
                if (!tempDataService.tempData('sizes')) {
                    serviceSize.getAll_Sizes().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                            tempDataService.putTempData('sizes', successRes.data.data);
                            $scope.sizes = successRes.data.data;
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.sizes = tempDataService.tempData('sizes');
                }
            }

            function loadeColor() {
                if (!tempDataService.tempData('colors')) {
                    serviceColor.getAll_Colors().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                            tempDataService.putTempData('colors', successRes.data.data);
                            $scope.colors = successRes.data.data;
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.colors = tempDataService.tempData('colors');
                }
            }

            function loadAuthDonVi() {
                if (!tempDataService.tempData('auDonVis')) {
                    serviceAuthDonVi.getAll_DonVi().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.length > 0) {
                            tempDataService.putTempData('auDonVis', successRes.data);
                            $scope.auDonVis = successRes.data;
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.auDonVis = tempDataService.tempData('auDonVis');
                }
            }

            loadAuthDonVi();
            loadeColor();
            loadeSize();
            loadSupplier();
            loadMerchandiseType();
            loadNhomVatTu();
            loadPackagings();
            loadTax();
            loadDonViTinh();
            loadeShelves();
            //end load danh mục

            //tiện ích sửa giá danh mục hàng hóa
            $scope.utility = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Merchandise', 'utility'),
                    controller: 'utilityMerchandiseController',
                    windowClass: 'app-modal-window',
                    resolve: {}
                });
                modalInstance.result.then(function () {
                    $scope.refresh();
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };
            //end tiện ích
            //load dữ liệu
            function filterAdvanceData() {
                $scope.filtered.advanceData = {};
                if ($scope.target.options) {
                    $scope.filtered.isAdvance = true;
                    $scope.filtered.advanceData[$scope.target.options] = $scope.summary;
                }
                $scope.isLoading = true;
                var postdata = { paged: $scope.paged, filtered: $scope.filtered };
                $scope.paged.data = [];
                postdata.filtered.advanceData.withGiaVon = true;
                service.postSelectDataQuery(postdata).then(function (successRes) {
                    if (successRes && successRes.status === 200 && successRes.data && successRes.data.status) {
                        $scope.isLoading = false;
                        $scope.data = successRes.data.data.data;
                        $scope.data.forEach(function (obj) {
                            if (obj.tyLeLaiBuon >= 100 || obj.tyLeLaiBuon < 0 || obj.tyLeLaiLe >= 100 || obj.tyLeLaiLe < 0) {
                                obj.className = "invalid";
                            }
                            else {
                                obj.className = "";
                            }
                        });
                        angular.extend($scope.paged, successRes.data.data);
                        $scope.filtered.isAdvance = false;
                        if (successRes.message) {
                            ngNotify.set(successRes.message, { type: 'success' });
                        }
                    }
                });
            }

            function filterData() {
                $scope.isLoading = true;
                if ($scope.accessList.view) {
                    var postdata = { paged: $scope.paged, filtered: $scope.filtered };
                    postdata.filtered.advanceData.withGiaVon = true;
                    service.postQuery(postdata).then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data && successRes.data.status) {
                            $scope.isLoading = false;
                            $scope.data = successRes.data.data.data;
                            $scope.data.forEach(function (obj) {
                                if (obj.tyLeLaiBuon >= 100 || obj.tyLeLaiBuon < 0 || obj.tyLeLaiLe >= 100 || obj.tyLeLaiLe < 0) {
                                    obj.className = "invalid";
                                }
                                else {
                                    obj.className = "";
                                }
                            });
                            angular.extend($scope.paged, successRes.data.data);
                        }
                    }, function (errorRes) {
                        console.log(errorRes);
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

            $scope.getNameVAT = function (paraValue, moduleName) {
                var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                if (data && data.length === 1) {
                    return data[0].description;
                } else {
                    return paraValue;
                }
            }
            //check quyền truy cập
            function loadAccessList() {
                securityService.getAccessList('merchandise').then(function (successRes) {
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
                    toaster.pop('error', "Lỗi:", "Không có quyền truy cập !");
                    $scope.accessList = null;
                });
            }
            loadAccessList();
            //end

            $scope.setPage = function (pageNo) {
                $scope.paged.currentPage = pageNo;
                filterAdvanceData();
            };
            $scope.sortType = 'maVatTu';
            $scope.sortReverse = false;
            $scope.doSearch = function () {
                $scope.paged.currentPage = 1;
                filterData();
            };
            $scope.doSearchStr = function () {
                $scope.paged.currentPage = 1;
                filterAdvanceData();
            }
            $scope.pageChanged = function () {
                filterAdvanceData();
            };
            $scope.goHome = function () {
                window.location.href = "#!/home";
            };
            $scope.refresh = function () {
                $scope.setPage($scope.paged.currentPage);
            };
            $scope.title = function () { return 'Danh sách mặt hàng'; };

            $scope.changeAvartar = function (event) {
                if (event.target.checked) {
                    $scope.displayAvatar = true;
                } else {
                    $scope.displayAvatar = false;
                }
            }
            //import Excel
            $scope.importExcel = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Merchandise', 'importExcel'),
                    controller: 'merchandiseImportExcelController',
                    size: 'lg',
                    resolve: {}
                });
                modalInstance.result.then(function () {
                    $scope.refresh();
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };
            //end
            $scope.exportTemplateExcel = function () {
                service.getExcelTemplate().then(function (response) {
                    if (response.status) {
                        var a = document.createElement("a");
                        document.body.appendChild(a);
                        a.style = "display: none";
                        var blob = new Blob([response.data], { type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
                        var objectUrl = URL.createObjectURL(blob);
                        a.href = objectUrl;
                        a.download = "Template.xlsx";
                        a.click();
                    }
                });
            }
            /* Function add New Item */
            $scope.create = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    size: 'md',
                    templateUrl: configService.buildUrl('htdm/Merchandise', 'add'),
                    controller: 'merchandiseCreateController',
                    windowClass: 'app-modal-window',
                    resolve: {}
                });
                modalInstance.result.then(function (updatedData) {
                    $scope.refresh();
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };

            $scope.createChild = function (target) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Merchandise', 'child-index'),
                    controller: 'merchandiseChildController',
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
            /* Function Edit Item */
            $scope.update = function (target) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Merchandise', 'update'),
                    controller: 'merchandiseEditController',
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

            /* Function Edit giá */
            $scope.updatePrice = function (item) {
                service.getPrice(item.maVatTu).then(function (response) {
                    var modalInstance = $uibModal.open({
                        backdrop: 'static',
                        templateUrl: configService.buildUrl('htdm/Merchandise', 'price-edit'),
                        controller: 'merchandisePriceDirectoryEditController',
                        resolve: {
                            targetData: function () {
                                return response;
                            }
                        }
                    });
                    modalInstance.result.then(function (updatedData) {
                    }, function () {
                        $log.info('Modal dismissed at: ' + new Date());
                    });

                });
            }
            $scope.updateGia = function (target) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Merchandise', 'price-edit'),
                    controller: 'merchandisePriceEditController',
                    resolve: {
                        targetData: function () {
                            return target;
                        }
                    }
                });
                modalInstance.result.then(function (updatedData) {
                    var index = $scope.target.dataDetails.indexOf(target);
                    if (index !== -1) {
                        $scope.target.dataDetails[index] = updatedData;
                    }
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };
            /* Function Details Item */
            $scope.details = function (target) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Merchandise', 'details'),
                    controller: 'merchandiseDetailsController',
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
            $scope.printITem = function (target) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Merchandise', 'printItem'),
                    controller: 'merchandiseExportItemController',
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
                    templateUrl: configService.buildUrl('htdm/Merchandise', 'delete'),
                    controller: 'merchandiseDeleteController',
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
            $scope.asyncView = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Merchandise', 'async-index'),
                    controller: 'matHangController',
                    windowClass: 'app-modal-window',
                    resolve: {}
                });
                modalInstance.result.then(function (updatedData) {
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };
            $scope.dayCanDienTu = function () {
                service.getByItemCodeNotNull().then(function (response) {
                    $window.location.href = configService.apiServiceBaseUri + "/Upload/Barcode/MaCanDienTu.xls";
                });
            };
            $scope.ketXuatExcel = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Merchandise', 'exportExcel'),
                    controller: 'exportExcelController',
                    windowClass: 'app-modal-window',
                    resolve: {}
                });
                modalInstance.result.then(function (updatedData) {
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };
        }]);

    //tiện ích
    app.controller('utilityMerchandiseController', [
        '$scope', '$uibModalInstance', 'configService', 'merchandiseService', 'tempDataService', '$filter', 'userService', 'supplierService', 'taxService', 'wareHouseService',
        function ($scope, $uibModalInstance, configService, service, tempDataService, $filter, serviceAuthUser, serviceSupplier, serviceTax, serviceWareHouse) {
            var currentUser = serviceAuthUser.GetCurrentUser();
            var unitCode = currentUser.unitCode;
            $scope.isLoading = false;
            $scope.robot = service.robot;
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.tempData = tempDataService.tempData;
            $scope.title = function () { return 'Tiện ích sửa giá mặt hàng'; };
            $scope.data = [];
            $scope.displayHepler = function (paraValue, moduleName) {
                var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                if (data && data.length === 1) {
                    return data[0].text;
                } else {
                    return;
                }
            };
            $scope.selectKho = function () {
                document.getElementById('_maVatTu').focus();
            };
            $scope.displayTenNhaCungCap = function (paraValue, moduleName) {
                var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                if (data && data.length === 1) {
                    return data[0].description;
                } else {
                    return;
                }
            };
            $scope.invalidLB = false;
            $scope.invalidLL = false;
            $scope.checkTyLeLai = function (item) {
                if ($scope.target.tyLeLaiLe >= 100 || $scope.target.tyLeLaiLe < 0) {
                    $scope.invalidLL = true;
                } else {
                    $scope.invalidLL = false;
                }
                if ($scope.tyLeLaiBuon >= 100 || $scope.tyLeLaiBuon < 0) {
                    $scope.invalidLB = true;
                }
                else {
                    $scope.invalidLB = false;
                }
            };
            function loadSupplier() {
                if (!tempDataService.tempData('suppliers')) {
                    serviceSupplier.getAll_Supplier().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                            tempDataService.putTempData('suppliers', successRes.data.data);
                            $scope.suppliers = successRes.data.data;
                        } else {

                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.suppliers = tempDataService.tempData('suppliers');
                }
            };
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
            loadSupplier();
            $scope.selectedMaHang = function (code) {
                if (code) {
                    service.getMerchandiseForNvByCode(code, $scope.target.maKho, unitCode).then(function (response) {
                        if (response && response.status === 200 && response.data && response.data.status) {
                            $scope.target.maVatTu = response.data.data.maVatTu;
                            $scope.target.tenVatTu = response.data.data.tenVatTu;
                            $scope.target.maNhaCungCap = response.data.data.maKhachHang;
                            $scope.target.barcode = response.data.data.barcode;
                            $scope.target.giaMuaCoVat = response.data.data.giaMuaVat;
                            $scope.target.tyLeVatVao = response.data.data.tyLeVatVao;
                            $scope.target.tyLeLaiBuon = response.data.data.tyLeLaiBuon;
                            $scope.target.giaBanBuonVat = response.data.data.giaBanBuonVat;
                            $scope.target.tyLeLaiLe = response.data.data.tyLeLaiLe
                            $scope.target.giaBanLeVat = response.data.data.giaBanLeVat;
                            $scope.target.giaMua = response.data.data.giaMua;
                            $scope.target.giaBanBuon = response.data.data.giaBanBuon;
                            $scope.target.tyLeVatRa = response.data.data.tyLeVatRa;
                            $scope.target.tyLeVatVao = response.data.data.tyLeVatVao;
                            document.getElementById('_giaBanBuonVat').select();
                            document.getElementById('_giaBanBuonVat').focus();
                        }
                    });
                }
            }
            $scope.addRow = function (target) {
                var item = {};
                item = angular.copy(target);
                if (item) {
                    if ($scope.data.length > 0) {
                        var exist = $filter('filter')($scope.data, { maVatTu: item.maVatTu }, true);
                        if (exist && exist.length === 1) {
                            //update
                            exist.maVatTu = item.maVatTu;
                            exist.tenVatTu = item.tenVatTu;
                            exist.barcode = item.barcode;
                            exist.giaBanBuonVat = item.giaBanBuonVat;
                            exist.giaBanLeVat = item.giaBanLeVat;
                            exist.soLuong = item.soLuong;
                            document.getElementById('_maVatTu').focus();
                            $scope.target.maVatTu = "";
                            $scope.target.tenVatTu = "";
                            $scope.target.maNhaCungCap = "";
                            $scope.target.barcode = "";
                            $scope.target.giaMuaCoVat = 0;
                            $scope.target.tyLeVatVao = 0;
                            $scope.target.tyLeLaiBuon = 0;
                            $scope.target.giaBanBuonVat = 0;
                            $scope.target.tyLeLaiLe = 0;
                            $scope.target.giaBanLeVat = 0;
                        }
                        else {
                            //chưa có thì thêm
                            $scope.data.push(item);
                            document.getElementById('_maVatTu').focus();
                            $scope.target.maVatTu = "";
                            $scope.target.tenVatTu = "";
                            $scope.target.maNhaCungCap = "";
                            $scope.target.barcode = "";
                            $scope.target.giaMuaCoVat = 0;
                            $scope.target.tyLeVatVao = 0;
                            $scope.target.tyLeLaiBuon = 0;
                            $scope.target.giaBanBuonVat = 0;
                            $scope.target.tyLeLaiLe = 0;
                            $scope.target.giaBanLeVat = 0;
                        }
                    }
                    else {
                        $scope.data.push(item);
                        document.getElementById('_maVatTu').focus();
                        $scope.target.maVatTu = "";
                        $scope.target.tenVatTu = "";
                        $scope.target.maNhaCungCap = "";
                        $scope.target.barcode = "";
                        $scope.target.giaMuaCoVat = 0;
                        $scope.target.tyLeVatVao = 0;
                        $scope.target.tyLeLaiBuon = 0;
                        $scope.target.giaBanBuonVat = 0;
                        $scope.target.tyLeLaiLe = 0;
                        $scope.target.giaBanLeVat = 0;
                    }
                }
            };
            $scope.removeItem = function (index) {
                if (index) {
                    $scope.data.splice(index, 1);
                    $scope.target.maVatTu = "";
                    $scope.target.tenVatTu = "";
                    $scope.target.maNhaCungCap = "";
                    $scope.target.barcode = "";
                    $scope.target.giaMuaCoVat = 0;
                    $scope.target.tyLeVatVao = 0;
                    $scope.target.tyLeLaiBuon = 0;
                    $scope.target.giaBanBuonVat = 0;
                    $scope.target.tyLeLaiLe = 0;
                    $scope.target.giaBanLeVat = 0;
                    document.getElementById('_maVatTu').focus();
                }
            }
            $scope.enterGiaBanBuonVat = function () {
                document.getElementById('_giaBanLeVat').focus();
                document.getElementById('_giaBanLeVat').select();
            };
            $scope.enterGiaBanLeVat = function () {
                document.getElementById('_soLuong').focus();
                document.getElementById('_soLuong').select();
            };
            $scope.enterSoLuong = function () {
                document.getElementById('_save').focus();
            };
            $scope.changeVatVao = function (code) {
                if (code) {
                    serviceTax.getTaxByCode(code).then(function (response) {
                        if (response && response.status == 200 && response.data) {
                            $scope.target.tyLeVatVao = response.data.taxRate;
                            angular.forEach($scope.target.dataDetails, function (v, k) {
                                $scope.target.dataDetails[k].maVatVao = $scope.target.maVatVao;
                                $scope.target.dataDetails[k].tyLeVatVao = $scope.target.tyLeVatVao;
                                $scope.target.dataDetails[k].giaMuaVat = $scope.target.dataDetails[k].giaMua * $scope.target.dataDetails[k].tyLeVatVao / 100 + $scope.target.dataDetails[k].giaMua;
                            });
                        }
                    });
                } else {
                    $scope.target.tyLeVatVao = 0;
                    angular.forEach($scope.target.dataDetails, function (v, k) {
                        $scope.target.dataDetails[k].maVatVao = $scope.target.maVatVao;
                        $scope.target.dataDetails[k].tyLeVatVao = $scope.target.tyLeVatVao;
                        $scope.target.dataDetails[k].giaMuaVat = $scope.target.dataDetails[k].giaMua * $scope.target.dataDetails[k].tyLeVatVao / 100 + $scope.target.dataDetails[k].giaMua;
                    });
                }
            };

            $scope.changeVatRa = function (code) {
                if (code) {
                    serviceTax.getTaxByCode(code).then(function (response) {
                        if (response && response.status == 200 && response.data) {
                            $scope.target.tyLeVatRa = response.data.taxRate;
                            angular.forEach($scope.target.dataDetails, function (v, k) {
                                $scope.target.dataDetails[k].maVatRa = $scope.target.maVatRa;
                                $scope.target.dataDetails[k].giaBanBuonVat = $scope.target.giaBanBuon * $scope.target.tyLeVatRa / 100 + $scope.target.giaBanBuon;
                                $scope.target.dataDetails[k].giaBanLeVat = $scope.target.giaBanLe * $scope.target.tyLeVatRa / 100 + $scope.target.giaBanLe;
                            });
                        }
                    });
                } else {
                    $scope.target.tyLeVatRa = 0;
                    angular.forEach($scope.target.dataDetails, function (v, k) {
                        $scope.target.dataDetails[k].maVatRa = $scope.target.maVatRa;
                        $scope.target.dataDetails[k].giaBanBuonVat = $scope.target.giaBanBuon * $scope.target.tyLeVatRa / 100 + $scope.target.giaBanBuon;
                        $scope.target.dataDetails[k].giaBanLeVat = $scope.target.giaBanLe * $scope.target.tyLeVatRa / 100 + $scope.target.giaBanLe;
                    });
                }
            };
            $scope.save = function () {

            };
            $scope.cancel = function () {
                $uibModalInstance.close();
            };
        }]);
    /* controller addNew */
    app.controller('merchandiseCreateController', ['$scope', '$uibModalInstance', 'configService', '$timeout', 'merchandiseService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'Upload', 'supplierService', 'merchandiseTypeService', 'nhomVatTuService', 'packagingService', 'taxService', 'donViTinhService', 'shelvesService', 'sizeService', 'colorService',
        function ($scope, $uibModalInstance, configService, $timeout, service, tempDataService, $filter, $uibModal, $log, ngNotify, upload, serviceSupplier, serviceMerchandiseType, serviceNhomVatTu, servicePackaging, serviceTax, serviceDonViTinh, serviceShelves, serviceSize, serviceColor) {
            $scope.robot = service.robot;
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.tempData = tempDataService.tempData;
            $scope.currentUser = {};
            $scope.nhomVatTus = [];
            $scope.maVatTu = '';
            $scope.maMauSelected = [];
            $scope.maSizeSelected = [];
            $scope.target = { dataDetails: [] };
            $scope.target.maSizes = [];
            $scope.target.maColors = [];
            $scope.lstImagesSrc = [];
            $scope.lstFile = [];
            $scope.fileAvatar = {};
            //load danh mục

            function loadSupplier() {
                if (!tempDataService.tempData('suppliers')) {
                    serviceSupplier.getAll_Supplier().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                            tempDataService.putTempData('suppliers', successRes.data.data);
                            $scope.suppliers = successRes.data.data;
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
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.nhomVatTus = tempDataService.tempData('nhomVatTus');
                }
            }

            function loadPackagings() {
                if (!tempDataService.tempData('packagings')) {
                    servicePackaging.getAll_Packaging().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                            tempDataService.putTempData('packagings', successRes.data.data);
                            $scope.packagings = successRes.data.data;
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
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.donViTinhs = tempDataService.tempData('donViTinhs');
                }
            }

            function loadeShelves() {
                if (!tempDataService.tempData('shelves')) {
                    serviceShelves.getAll_Shelves().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                            tempDataService.putTempData('shelves', successRes.data.data);
                            $scope.shelves = successRes.data.data;
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.shelves = tempDataService.tempData('shelves');
                }
            }
            function loadeSize() {
                if (!tempDataService.tempData('sizes')) {
                    serviceSize.getAll_Sizes().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                            tempDataService.putTempData('sizes', successRes.data.data);
                            $scope.sizes = successRes.data.data;
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.sizes = tempDataService.tempData('sizes');
                }
            }

            function loadeColor() {
                if (!tempDataService.tempData('colors')) {
                    serviceColor.getAll_Colors().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                            tempDataService.putTempData('colors', successRes.data.data);
                            $scope.colors = successRes.data.data;
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.colors = tempDataService.tempData('colors');
                }
            }

            loadeColor();
            loadeSize();
            loadSupplier();
            loadMerchandiseType();
            loadNhomVatTu();
            loadPackagings();
            loadTax();
            loadDonViTinh();
            loadeShelves();
            $scope.changedMerchancedise = function (tenVatTu) {
                $scope.target.tenVietTat = tenVatTu;
            };
            $scope.reLoadSupplier = function () {
                serviceSupplier.getAll_Supplier().then(function (successRes) {
                    if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                        tempDataService.putTempData('suppliers', successRes.data.data);
                        $scope.suppliers = successRes.data.data;
                    }
                }, function (errorRes) {
                    console.log('errorRes', errorRes);
                });
            };
            $scope.reLoadDonViTinh = function () {
                serviceDonViTinh.getAll_DonViTinh().then(function (successRes) {
                    if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                        tempDataService.putTempData('donViTinhs', successRes.data.data);
                        $scope.donViTinhs = successRes.data.data;
                    }
                }, function (errorRes) {
                    console.log('errorRes', errorRes);
                });
            };
            $scope.reLoadShelves = function () {
                serviceShelves.getAll_Shelves().then(function (successRes) {
                    if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                        tempDataService.putTempData('shelves', successRes.data.data);
                        $scope.shelves = successRes.data.data;
                    }
                }, function (errorRes) {
                    console.log('errorRes', errorRes);
                });
            };
            $scope.reLoadTaxs = function () {
                serviceTax.getAll_Tax().then(function (successRes) {
                    if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                        tempDataService.putTempData('taxs', successRes.data.data);
                        $scope.taxs = successRes.data.data;
                    }
                }, function (errorRes) {
                    console.log('errorRes', errorRes);
                });
            };
            //end load danh mục
            //declare
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
            };
            $scope.displayHepler = function (paraValue, moduleName) {
                var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                if (data && data.length === 1) {
                    return data[0].text;
                } else {
                    return;
                }
            };
            $scope.isLoading = false;
            $scope.title = function () { return 'Thêm mới mặt hàng'; };
            $scope.target.useGenCode = true;
            $scope.tempMaVatTu = '';
            $scope.tempEnterMechandise = '';
            //end
            $scope.changeLoaiVatTu = function () {
                service.getSelectByMaLoai($scope.target.maLoaiVatTu).then(function (response) {
                    if (response && response.status == 200 && response.data) {
                        $scope.nhomVatTus = response.data;
                    }
                });
                service.getNewCode($scope.target.maLoaiVatTu).then(function (response) {
                    if (response && response.status == 200 && response.data) {
                        $scope.target.maVatTu = response.data;
                        $scope.maVatTu = response.data;
                        $scope.tempMaVatTu = angular.copy($scope.target.maVatTu);
                    }
                });
            };
            $scope.addBarCode = function (target) {
                if (!target.barcode) {
                    target.barcode = ";";
                }
                target.barcode = target.barcode + target.newBarcode + ";";
                target.newBarcode = "";
            };
            $scope.changeVatVao = function (code) {
                if (code) {
                    serviceTax.getTaxByCode(code).then(function (response) {
                        if (response && response.status == 200 && response.data) {
                            $scope.target.tyLeVatVao = response.data.taxRate;
                            angular.forEach($scope.target.dataDetails, function (v, k) {
                                $scope.target.dataDetails[k].maVatVao = $scope.target.maVatVao;
                                $scope.target.dataDetails[k].tyLeVatVao = $scope.target.tyLeVatVao;
                                $scope.target.dataDetails[k].giaMuaVat = $scope.target.dataDetails[k].giaMua * $scope.target.dataDetails[k].tyLeVatVao / 100 + $scope.target.dataDetails[k].giaMua;
                            });
                            //logcode
                            $scope.changeVatRa(code);
                        }
                    });
                } else {
                    $scope.target.tyLeVatVao = 0;
                    angular.forEach($scope.target.dataDetails, function (v, k) {
                        $scope.target.dataDetails[k].maVatVao = $scope.target.maVatVao;
                        $scope.target.dataDetails[k].tyLeVatVao = $scope.target.tyLeVatVao;
                        $scope.target.dataDetails[k].giaMuaVat = $scope.target.dataDetails[k].giaMua * $scope.target.dataDetails[k].tyLeVatVao / 100 + $scope.target.dataDetails[k].giaMua;
                    });
                }
            };

            $scope.changeVatRa = function (code) {
                if (code) {
                    serviceTax.getTaxByCode(code).then(function (response) {
                        if (response && response.status == 200 && response.data) {
                            $scope.target.maVatRa = code;
                            $scope.target.tyLeVatRa = response.data.taxRate;
                            angular.forEach($scope.target.dataDetails, function (v, k) {
                                $scope.target.dataDetails[k].maVatRa = $scope.target.maVatRa;
                                $scope.target.dataDetails[k].giaBanBuonVat = $scope.target.giaBanBuon * $scope.target.tyLeVatRa / 100 + $scope.target.giaBanBuon;
                                $scope.target.dataDetails[k].giaBanLeVat = $scope.target.giaBanLe * $scope.target.tyLeVatRa / 100 + $scope.target.giaBanLe;
                            });
                        }
                    });
                } else {
                    $scope.target.tyLeVatRa = 0;
                    angular.forEach($scope.target.dataDetails, function (v, k) {
                        $scope.target.dataDetails[k].maVatRa = $scope.target.maVatRa;
                        $scope.target.dataDetails[k].giaBanBuonVat = $scope.target.giaBanBuon * $scope.target.tyLeVatRa / 100 + $scope.target.giaBanBuon;
                        $scope.target.dataDetails[k].giaBanLeVat = $scope.target.giaBanLe * $scope.target.tyLeVatRa / 100 + $scope.target.giaBanLe;
                    });
                }
            };
            //add
            $scope.createMerchandiseType = function (target, name) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/MerchandiseType', 'add'),
                    controller: 'merchandiseTypeCreateController',
                    resolve: {}
                });
                modalInstance.result.then(function (updatedData) {
                    $scope.tempData.update('merchandiseTypes', function () {
                        if (target && name) {
                            target[name] = updatedData.mdNhomVatTu;
                        }

                    });
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };

            $scope.createNhomVatTu = function (target, name) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/NhomVatTu', 'add'),
                    controller: 'nhomVatTuCreateController',
                    resolve: {}
                });
                modalInstance.result.then(function (updatedData) {
                    $scope.tempData.update('nhomVatTus', function () {
                        if (target && name) {
                            target[name] = updatedData.mdNhomVatTu;
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
                    target.maBaoBi = updatedData.maBaoBi;
                    $scope.searchMaBaoBi = updatedData.soLuong;
                    var ob = {
                        description: updatedData.tenBaoBi,
                        extendValue: updatedData.soLuong,
                        value: updatedData.maBaoBi,
                        text: updatedData.maBaoBi + "|" + updatedData.tenBaoBi,
                    };
                    $scope.packagings.push(ob);
                    tempDataService.putTempData('packagings', $scope.packagings);
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };
            $scope.filterQuyCach = function (code) {
                if (code) {
                    var data = $filter('filter')($scope.tempData('packagings'), { value: code }, true);
                    if (data.length > 0) {
                        $scope.searchMaBaoBi = angular.copy(data[0].extendValue);
                    }
                }
            };
            $scope.createShelve = function (target, name) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Shelves', 'add'),
                    controller: 'shelvesCreateController',
                    resolve: {}
                });
                modalInstance.result.then(function (updatedData) {
                    $scope.tempData.update('shelves', function () {
                        if (target && name) {
                            target[name] = updatedData.maKeHang;
                        }
                    });
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };
            $scope.createSize = function (target, name) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Size', 'add'),
                    controller: 'sizeCreateController',
                    resolve: {}
                });
                modalInstance.result.then(function (updatedData) {
                    $scope.tempData.update('sizes', function () {
                        if (target && name) {
                            var data = $filter('filter')($scope.tempData('sizes'), { value: updatedData.maSize }, true);
                            if (data && data.length) {
                                target[name].push(data[0]);
                            }
                        }
                    });
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };
            $scope.createColor = function (target, name) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Color', 'add'),
                    controller: 'colorCreateController',
                    resolve: {}
                });
                modalInstance.result.then(function (updatedData) {
                    $scope.tempData.update('colors', function () {
                        if (target && name) {
                            var data = $filter('filter')($scope.tempData('colors'), { value: updatedData.maColor }, true);
                            if (data && data.length) {
                                target[name].push(data[0]);
                            }
                        }
                    });
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };
            //end

            $scope.update = function (target) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Merchandise', 'price-edit'),
                    controller: 'merchandisePriceEditController',
                    resolve: {
                        targetData: function () {
                            return target;
                        },
                        initData: {
                            maVatRa: $scope.target.maVatRa,
                            maVatVao: $scope.target.maVatVao,
                            tyLeVatRa: $scope.target.tyLeVatRa,
                            tyLeVatVao: $scope.target.tyLeVatVao
                        }
                    }
                });
                modalInstance.result.then(function (updatedData) {
                    var index = $scope.target.dataDetails.indexOf(target);
                    if (index !== -1) {
                        $scope.target.dataDetails[index] = updatedData;
                    }
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };

            $scope.removeItem = function (index) {
                var currentPage = $scope.paged.currentPage;
                var itemsPerPage = $scope.paged.itemsPerPage;
                var currentPageIndex = (currentPage - 1) * itemsPerPage + index;
                $scope.target.dataDetails.splice(currentPageIndex, 1);
                $scope.pageChanged();
            };
            $scope.create = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Merchandise', 'price'),
                    controller: 'merchandisePriceCreateController',
                    resolve: {
                        initData: {
                            maVatRa: $scope.target.maVatRa,
                            maVatVao: $scope.target.maVatVao,
                            tyLeVatRa: $scope.target.tyLeVatRa,
                            tyLeVatVao: $scope.target.tyLeVatVao
                        }
                    }
                });
                modalInstance.result.then(function (updatedData) {
                    var checkIfExsit = $scope.target.dataDetails.some(function (element, index, array) {
                        return element.maDonVi === updatedData.maDonVi;
                    });
                    if (!checkIfExsit) {
                        $scope.target.dataDetails.push(updatedData);
                        $scope.pageChanged();
                    }
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };
            $scope.enterCodeMerchandise = function (codeMerchandise) {
                if ($scope.target.isUpdateCodeMerchandise === 'OK' && codeMerchandise != '') {
                    var temp = codeMerchandise.split('-');
                    $scope.maVatTu = temp[0];
                }
            };
            function change_alias(alias) {
                var str = alias;
                str = str.toLowerCase();
                str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
                str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
                str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
                str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
                str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
                str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
                str = str.replace(/đ/g, "d");
                str = str.replace(/!|@|%|\^|\*|\(|\)|\+|\=|\<|\>|\?|\/|,|\.|\:|\;|\'|\"|\&|\#|\[|\]|~|\$|_|`|-|{|}|\||\\/g, " ");
                str = str.replace(/ + /g, " ");
                str = str.trim();
                return str;
            }
            $scope.genMaVatTu = function () {
                var result = $scope.maVatTu;
                if ($scope.maMauSelected.length > 0) result += '-';
                for (var i = 0; i < $scope.maMauSelected.length; i++) {
                    if (i < $scope.maMauSelected.length - 1)
                        result += $scope.maMauSelected[i] + '-';
                    else
                        result += $scope.maMauSelected[i];
                }
                if ($scope.maSizeSelected.length > 0) result += '-';
                for (var i = 0; i < $scope.maSizeSelected.length; i++) {
                    if (i < $scope.maSizeSelected.length - 1)
                        result += $scope.maSizeSelected[i] + '-';
                    else
                        result += $scope.maSizeSelected[i];
                }
                $scope.target.maVatTu = result;
            };
            $scope.changeColor = function (lstColor) {
                $scope.maMauSelected = [];
                angular.forEach(lstColor, function (value, key) {
                    value.text = change_alias(value.text);
                    value.text = value.text.toUpperCase();
                    value.text = value.text.replace(" ", "");
                    if ($scope.maMauSelected.indexOf(value.text) != -1) {
                    }
                    else
                        $scope.maMauSelected.push(value.text);
                });
                //$scope.genMaVatTu();
            };

            $scope.removeColor = function (lstColor) {
                $scope.maMauSelected = [];
                angular.forEach(lstColor, function (value, key) {
                    value.text = change_alias(value.text);
                    value.text = value.text.toUpperCase();
                    value.text = value.text.replace(" ", "");
                    if ($scope.maMauSelected.indexOf(value.text) != -1) {
                    }
                    else
                        $scope.maMauSelected.push(value.text);
                });
                //$scope.genMaVatTu();
            };

            $scope.changeSize = function (lstSize) {
                if (lstSize) {
                    $scope.maSizeSelected = [];
                    angular.forEach(lstSize, function (value, key) {
                        value.text = change_alias(value.text);
                        value.text = value.text.toUpperCase();
                        value.text = value.text.replace(" ", "");
                        value.text = value.text.replace("SIZE", "");
                        if ($scope.maSizeSelected.indexOf(value.text) != -1) { }
                        else $scope.maSizeSelected.push(value.text);
                    });
                    //$scope.genMaVatTu();
                }
            };
            $scope.removeSize = function (lstSize) {
                if (lstSize) {
                    $scope.maSizeSelected = [];
                    angular.forEach(lstSize, function (value, key) {
                        value.text = change_alias(value.text);
                        value.text = value.text.toUpperCase();
                        value.text = value.text.replace(" ", "");
                        value.text = value.text.replace("SIZE", "");
                        if ($scope.maSizeSelected.indexOf(value.text) != -1) { }
                        else $scope.maSizeSelected.push(value.text);
                    });
                    //$scope.genMaVatTu();
                }
            };
            $scope.uploadFile = function (input) {
                if (input.files && input.files.length > 0) {
                    angular.forEach(input.files, function (file) {
                        $scope.lstFile.push(file);
                        $timeout(function () {
                            var fileReader = new FileReader();
                            fileReader.readAsDataURL(file);
                            fileReader.onload = function (e) {
                                $timeout(function () {
                                    $scope.lstImagesSrc.push(e.target.result);
                                });
                            }
                        });
                    });
                }
            };
            $scope.deleteImage = function (index) {
                $scope.lstImagesSrc.splice(index, 1);
                $scope.lstFile.splice(index, 1);
                if ($scope.lstFile.length < 1) {
                    angular.element("#file-input-upload").val(null);
                }
            };
            $scope.uploadAvatar = function (input) {
                $scope.inputAvatar = input;
                if (input.files && input.files.length > 0) {
                    $timeout(function () {
                        var fileReader = new FileReader();
                        fileReader.readAsDataURL(input.files[0]);
                        fileReader.onload = function (e) {
                            $timeout(function () {
                                $scope.fileAvatar.src = e.target.result;
                            });
                        }
                    });
                    $scope.fileAvatar.file = input.files[0];
                }
            };
            $scope.deleteAvatar = function () {
                $scope.fileAvatar = {};
                angular.element("#file-input-ava").val(null);
            };
            function saveMerchandise() {
                $scope.target.maSize = '';
                $scope.target.maColor = '';
                angular.forEach($scope.target.maSizes, function (size) {
                    $scope.target.maSize = $scope.target.maSize + size.value + ',';
                });
                angular.forEach($scope.target.maColors, function (co) {
                    $scope.target.maColor = $scope.target.maColor + co.value + ',';
                });

                //nếu có sự thay đổi mã so với mã ban đầu tự sinh thì bỏ qua hàm SaveCode
                if ($scope.target.maVatTu !== $scope.tempMaVatTu) {
                    $scope.target.useGenCode = false;
                }
                service.post(JSON.stringify($scope.target)).then(function (response) {
                    if (response && response.status == 200 && response.data && response.data.status && response.data.data) {
                        ngNotify.set('Thành công', { type: 'success' });
                        $uibModalInstance.close($scope.target);
                    } else {
                        ngNotify.set(response.data.message, { duration: 3000, type: 'error' });
                    }
                    //End fix
                });
            };
            function saveAvatar() {
                $scope.fileAvatar.maVatTu = $scope.target.maVatTu;
                upload.upload({
                    url: configService.rootUrlWebApi + '/Md/Merchandise/UploadAvatar',
                    data: $scope.fileAvatar
                }).then(function (response) {
                    if (response.status) {
                        $scope.target.avatarName = response.data.data;
                        saveMerchandise();
                    }
                    else {
                        toaster.pop('error', "Lỗi:", "Không lưu được ảnh! Có thể đã trùng!");
                    }
                });
            };
            function saveImage() {
                $scope.target.file = $scope.lstFile;
                upload.upload({
                    url: configService.rootUrlWebApi + '/Md/Merchandise/UploadMerchandiseImage',
                    data: $scope.target
                }).then(function (response) {
                    if (response.status) {
                        $scope.target.image = response.data.data;
                    }
                    else {
                        toaster.pop('error', "Lỗi:", "Không lưu được ảnh! Có thể đã trùng!");
                    }
                    if ($scope.fileAvatar) {
                        saveAvatar();
                    }
                    else {
                        saveMerchandise();
                    }
                });
            };
            $scope.save = function () {
                if (!$scope.target.maLoaiVatTu) {
                    ngNotify.set("Chưa khai báo mã loại vật tư", { duration: 3000, type: 'error' });
                } else if (!$scope.target.maNhomVatTu) {
                    ngNotify.set("Chưa khai báo mã nhóm vật tư", { duration: 3000, type: 'error' });
                } else if (!$scope.target.maVatTu) {
                    ngNotify.set("Chưa khai báo mã vật tư", { duration: 3000, type: 'error' });
                } else if (!$scope.target.maKhachHang) {
                    ngNotify.set("Chưa khai báo mã nhà cung cấp", { duration: 3000, type: 'error' });
                } else {
                    if ($scope.lstFile && $scope.lstFile.length) {
                        saveImage();
                    } else {
                        if ($scope.fileAvatar) {
                            saveAvatar();
                        } else {
                            saveMerchandise();
                        }
                    }
                }
            };
            $scope.cancel = function () {
                $uibModalInstance.close();
            };
        }]);
    /* controller Edit */
    app.controller('merchandiseEditController', ['$scope', '$uibModalInstance', 'configService', '$timeout', 'merchandiseService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify', 'Upload', 'supplierService', 'packagingService', 'taxService', 'donViTinhService', 'shelvesService', 'securityService',
        function ($scope, $uibModalInstance, configService, $timeout, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify, upload, serviceSupplier, servicePackaging, serviceTax, serviceDonViTinh, serviceShelves, securityService) {
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.robot = service.robot;
            $scope.targetData = angular.copy(targetData);
            $scope.tempData = tempDataService.tempData;
            $scope.target = targetData;
            $scope.nhomVatTus = [];
            $scope.lstImagesSrcFromDb = [];
            $scope.lstImagesSrc = [];
            $scope.lstFile = [];
            $scope.fileAvatar = {};
            $scope.maSizes = [];
            $scope.maColors = [];
            $scope.isLoading = false;
            $scope.title = function () { return 'Cập nhập Hàng hóa, Vật tư '; };
            $scope.flag = false;
            function filterData() {
                service.getDetailByCode($scope.target.maVatTu).then(function (response) {
                    if (response && response.status == 200 && response.data) {
                        $scope.target = response.data;
                        if ($scope.target.image) {
                            var arrImage = $scope.target.image.split(',');
                            arrImage.splice(arrImage.length - 1, 1);
                            $scope.lstImagesSrcFromDb = arrImage;
                        }
                        $scope.target.maSizes = [];
                        if ($scope.target.maSize) {
                            var lst = $scope.target.maSize.split(',');
                            angular.forEach(lst, function (item) {
                                var tmp = $filter('filter')($scope.tempData('sizes'), { value: item }, true);
                                if (tmp && tmp.length) {
                                    $scope.target.maSizes.push(tmp[0]);
                                }
                            });
                        }
                        $scope.target.maColors = [];
                        if ($scope.target.maColor) {
                            var lst = $scope.target.maColor.split(',');
                            angular.forEach(lst, function (item) {
                                var tmp = $filter('filter')($scope.tempData('colors'), { value: item }, true);
                                if (tmp && tmp.length) {
                                    $scope.target.maColors.push(tmp[0]);
                                }
                            });
                        }
                        if ($scope.target.maBaoBi) {
                            service.getBaoBiByCode($scope.target.maBaoBi).then(function (response) {
                                if (response.status && response.data.status) {
                                    $scope.searchMaBaoBi = response.data.data.soLuong;
                                }
                            });
                        }
                        if ($scope.target.maLoaiVatTu) {
                            service.getSelectByMaLoai($scope.target.maLoaiVatTu).then(function (response) {
                                $scope.nhomVatTus = response;
                            });
                        }
                        $scope.pageChanged();
                    }
                });
            }

            $scope.accessList = {};
            //check quyền truy cập
            function loadAccessList() {
                securityService.getAccessList('merchandise').then(function (successRes) {
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
                    toaster.pop('error', "Lỗi:", "Không có quyền truy cập !");
                    $scope.accessList = null;
                });
            }
            loadAccessList();
            //end

            $scope.displayHepler = function (paraValue, moduleName) {
                var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                if (data && data.length === 1) {
                    return data[0].text;
                } else {
                    return paraValue;
                }
            }

            $scope.displayHepler_ByName = function (paraValue, moduleName) {
                var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                if (data && data.length === 1) {
                    return data[0].description;
                } else {
                    return paraValue;
                }
            }

            $scope.addBarCode = function (target) {
                if (!target.barcode) {
                    target.barcode = ";";
                }
                target.barcode = target.barcode + target.newBarcode + ";";
                target.newBarcode = "";
            };
            //nút update giá trong chức năng sửa giá
            $scope.update = function (target) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Merchandise', 'price-edit'),
                    controller: 'merchandisePriceEditController',
                    resolve: {
                        targetData: function () {
                            return target;
                        },
                        initData: {
                            maVatRa: $scope.target.maVatRa,
                            maVatVao: $scope.target.maVatVao,
                            tyLeVatRa: $scope.target.tyLeVatRa,
                            tyLeVatVao: $scope.target.tyLeVatVao
                        }
                    }
                });
                modalInstance.result.then(function (updatedData) {
                    var index = $scope.target.dataDetails.indexOf(target);
                    if (index !== -1) {
                        $scope.target.dataDetails[index] = updatedData;
                    }
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };

            $scope.reLoadSupplier = function () {
                serviceSupplier.getAll_Supplier().then(function (successRes) {
                    if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                        tempDataService.putTempData('suppliers', successRes.data.data);
                        $scope.suppliers = successRes.data.data;
                    }
                }, function (errorRes) {
                    console.log('errorRes', errorRes);
                });
            };
            $scope.reLoadDonViTinh = function () {
                serviceDonViTinh.getAll_DonViTinh().then(function (successRes) {
                    if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                        tempDataService.putTempData('donViTinhs', successRes.data.data);
                        $scope.donViTinhs = successRes.data.data;
                    }
                }, function (errorRes) {
                    console.log('errorRes', errorRes);
                });
            };
            $scope.reLoadShelves = function () {
                serviceShelves.getAll_Shelves().then(function (successRes) {
                    if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                        tempDataService.putTempData('shelves', successRes.data.data);
                        $scope.shelves = successRes.data.data;
                    }
                }, function (errorRes) {
                    console.log('errorRes', errorRes);
                });
            };
            $scope.reLoadTaxs = function () {
                serviceTax.getAll_Tax().then(function (successRes) {
                    if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                        tempDataService.putTempData('taxs', successRes.data.data);
                        $scope.taxs = successRes.data.data;
                    }
                }, function (errorRes) {
                    console.log('errorRes', errorRes);
                });
            };
            $scope.reloadPackagings = function () {
                if (!tempDataService.tempData('packagings')) {
                    servicePackaging.getAll_Packaging().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                            tempDataService.putTempData('packagings', successRes.data.data);
                            $scope.packagings = successRes.data.data;
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.packagings = tempDataService.tempData('packagings');
                }
            }
            //end load danh mục
            //$scope.tempData('status') = angular.copy($scope.tempData('status'));
            $scope.changeLoaiVatTu = function () {
                service.getSelectByMaLoai($scope.target.maLoaiVatTu).then(function (response) {
                    $scope.nhomVatTus = response;
                });
            }
            $scope.changeMaNhom = function () {
                $scope.flag = true;
            }
            $scope.changeVatVao = function (maVatVao) {
                if (maVatVao) {
                    serviceTax.getTaxByCode(maVatVao).then(function (response) {
                        if (response && response.status == 200 && response.data) {
                            $scope.target.tyLeVatVao = response.data.taxRate;
                            $scope.target.maVatVao = maVatVao;
                            angular.forEach($scope.target.dataDetails, function (v, k) {
                                $scope.target.dataDetails[k].maVatVao = $scope.target.maVatVao;
                                $scope.target.dataDetails[k].tyLeVatVao = $scope.target.tyLeVatVao;
                                $scope.target.giaMuaVat = $scope.target.giaMua * $scope.target.tyLeVatVao / 100 + $scope.target.giaMua;
                                $scope.robot.changeTyLeLaiLe($scope.target.dataDetails[k]);
                                $scope.robot.changeTyLeLaiBuon($scope.target.dataDetails[k]);
                                $scope.robot.changeGiaBanLe($scope.target.dataDetails[k]);
                                $scope.robot.changeGiaBanBuon($scope.target.dataDetails[k]);
                                $scope.robot.changGiaBanLeVat($scope.target.dataDetails[k]);
                                $scope.robot.changeGiaBanBuonVat($scope.target.dataDetails[k]);
                            });
                            $scope.pageChanged();
                        }
                    });
                } else {
                    $scope.target.tyLeVatVao = 0;
                    angular.forEach($scope.target.dataDetails, function (v, k) {
                        $scope.target.dataDetails[k].maVatVao = $scope.target.maVatVao;
                        $scope.target.dataDetails[k].tyLeVatVao = $scope.target.tyLeVatVao;
                        $scope.target.giaMuaVat = $scope.target.giaMua * $scope.target.tyLeVatVao / 100 + $scope.target.giaMua;
                        $scope.robot.changeTyLeLaiLe($scope.target.dataDetails[k]);
                        $scope.robot.changeTyLeLaiBuon($scope.target.dataDetails[k]);
                        $scope.robot.changeGiaBanLe($scope.target.dataDetails[k]);
                        $scope.robot.changeGiaBanBuon($scope.target.dataDetails[k]);
                        $scope.robot.changGiaBanLeVat($scope.target.dataDetails[k]);
                        $scope.robot.changeGiaBanBuonVat($scope.target.dataDetails[k]);
                        $scope.pageChanged();
                    });
                }
            };

            $scope.changeVatRa = function (maVatRa) {
                if (maVatRa) {
                    serviceTax.getTaxByCode(maVatRa).then(function (response) {
                        if (response && response.status == 200 && response.data) {
                            $scope.target.tyLeVatRa = response.data.taxRate;
                            $scope.target.maVatRa = maVatRa;
                            angular.forEach($scope.target.dataDetails, function (v, k) {
                                $scope.target.dataDetails[k].maVatRa = $scope.target.maVatRa;
                                $scope.target.dataDetails[k].tyLeVatRa = $scope.target.tyLeVatRa;
                                $scope.target.giaBanBuonVat = $scope.target.giaBanBuon * $scope.target.tyLeVatRa / 100 + $scope.target.giaBanBuon;
                                $scope.target.giaBanLeVat = $scope.target.giaBanLe * $scope.target.tyLeVatRa / 100 + $scope.target.giaBanLe;
                            });
                        }
                    });
                } else {
                    $scope.target.tyLeVatRa = 0;
                    angular.forEach($scope.target.dataDetails, function (v, k) {
                        $scope.target.dataDetails[k].maVatRa = $scope.target.maVatRa;
                        $scope.target.giaBanBuonVat = $scope.target.giaBanBuon * $scope.target.tyLeVatRa / 100 + $scope.target.giaBanBuon;
                        $scope.target.giaBanLeVat = $scope.target.giaBanLe * $scope.target.tyLeVatRa / 100 + $scope.target.giaBanLe;
                    });
                }
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
            };
            $scope.createPackage = function (target, name) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Packaging', 'add'),
                    controller: 'packagingCreateController',
                    resolve: {}
                });
                modalInstance.result.then(function (updatedData) {
                    target.maBaoBi = updatedData.maBaoBi;
                    $scope.searchMaBaoBi = updatedData.soLuong;
                    var ob = {
                        description: updatedData.tenBaoBi,
                        extendValue: updatedData.soLuong,
                        value: updatedData.maBaoBi,
                        text: updatedData.maBaoBi + "|" + updatedData.tenBaoBi,
                    };
                    $scope.packagings.push(ob);
                    tempDataService.putTempData('packagings', $scope.packagings);
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };
            $scope.filterQuyCach = function (code) {
                if (code) {
                    var data = $filter('filter')($scope.tempData('packagings'), { value: code }, true);
                    if (data.length > 0) {
                        $scope.searchMaBaoBi = angular.copy(data[0].extendValue);
                    }
                }
            };
            $scope.changedMerchancedise = function (tenVatTu) {
                $scope.target.tenVietTat = tenVatTu;
            }
            $scope.uploadFile = function (input) {
                if (input.files && input.files.length > 0) {
                    angular.forEach(input.files, function (file) {
                        $scope.lstFile.push(file);
                        $timeout(function () {
                            var fileReader = new FileReader();
                            fileReader.readAsDataURL(file);
                            fileReader.onload = function (e) {
                                $timeout(function () {
                                    $scope.lstImagesSrc.push(e.target.result);
                                });
                            }
                        });
                    });
                }
            };
            $scope.deleteImage = function (index) {
                $scope.lstImagesSrc.splice(index, 1);
                $scope.lstFile.splice(index, 1);
                if ($scope.lstFile.length < 1) {
                    angular.element("#file-input-upload").val(null);
                }
            };
            $scope.deleteImageFromDb = function (index) {
                $scope.lstImagesSrcFromDb.splice(index, 1);
            };
            function saveMerchandise() {
                $scope.target.maSize = '';
                $scope.target.maColor = '';
                angular.forEach($scope.target.maSizes, function (size) {
                    $scope.target.maSize = $scope.target.maSize + size.value + ',';
                });
                angular.forEach($scope.target.maColors, function (co) {
                    $scope.target.maColor = $scope.target.maColor + co.value + ',';
                });
                if ($scope.lstImagesSrcFromDb && $scope.lstImagesSrcFromDb.length > 0) {
                    angular.forEach($scope.lstImagesSrcFromDb, function (item) {
                        $scope.target.image = $scope.target.image + item + ",";
                    });
                };
                if ($scope.flag == true) {
                    service.updateCodeGroup(JSON.stringify($scope.target), function (response) {
                        if (response.status) {
                            ngNotify.set('Thành công', { type: 'success' });
                            $uibModalInstance.close($scope.target);
                        } else {
                            ngNotify.set(response.data.message, { duration: 3000, type: 'error' });
                        }
                    });
                }
                else {
                    if (!$scope.target.maVatRa) {
                        ngNotify.set("Chưa chọn VAT ra", { duration: 2000, type: 'error' });
                    } else if (!$scope.target.maVatVao) {
                        ngNotify.set("Chưa chọn VAT vào", { duration: 2000, type: 'error' });
                    } else if (!$scope.target.maKhachHang) {
                        ngNotify.set("Chưa chọn nhà cung cấp", { duration: 2000, type: 'error' });
                    } else if (!$scope.target.maVatTu) {
                        ngNotify.set("Chưa chọn mã hàng", { duration: 2000, type: 'error' });
                    } else if (!$scope.target.maLoaiVatTu) {
                        ngNotify.set("Chưa chọn mã loại vật tư", { duration: 2000, type: 'error' });
                    } else if (!$scope.target.maNhomVatTu) {
                        ngNotify.set("Chưa chọn mã nhóm vật tư", { duration: 2000, type: 'error' });
                    } else if (!$scope.target.tenHang) {
                        ngNotify.set("Chưa nhập tên vật tư", { duration: 2000, type: 'error' });
                    }
                    else {
                        //trường hợp update giá cho 1 đơn vị
                        if ($scope.target.dataDetails.length == 1) {
                            $scope.target.dataDetails[0].giaBanBuon = $scope.target.giaBanBuon;
                            $scope.target.dataDetails[0].giaBanBuonVat = $scope.target.giaBanBuonVat;
                            $scope.target.dataDetails[0].giaBanLe = $scope.target.giaBanLe;
                            $scope.target.dataDetails[0].giaBanLeVat = $scope.target.giaBanLeVat;
                            $scope.target.dataDetails[0].giaMua = $scope.target.giaMua;
                            $scope.target.dataDetails[0].giaMuaVat = $scope.target.giaMuaVat;
                            $scope.target.dataDetails[0].maVatRa = $scope.target.maVatRa;
                            $scope.target.dataDetails[0].tyLeVatRa = $scope.target.tyLeVatRa;
                            $scope.target.dataDetails[0].maVatVao = $scope.target.maVatVao;
                            $scope.target.dataDetails[0].tyLeVatVao = $scope.target.tyLeVatVao;
                            $scope.target.dataDetails[0].tyLeLaiLe = $scope.target.tyLeLaiLe;
                            $scope.target.dataDetails[0].tyLeLaiBuon = $scope.target.tyLeLaiBuon;

                            service.update($scope.target).then(function (response) {
                                if (response.status && response.status == 200) {
                                    if (response.data.status) {
                                        ngNotify.set('Thành công', { type: 'success' });
                                        $uibModalInstance.close($scope.target);
                                    } else {
                                        ngNotify.set(response.data.message, { duration: 2000, type: 'error' });
                                    }
                                } else {
                                    console.log('ERROR: Update failed! ' + response.errorMessage);
                                    ngNotify.set(response.errorMessage, { duration: 2000, type: 'error' });
                                }
                            },
                                function (response) {
                                    ngNotify.set('ERROR: Update failed!', { duration: 2000, type: 'error' });
                                });
                        }
                        else {
                            ngNotify.set("Mã này tồn tại 2 đơn vị giá", { duration: 2000, type: 'error' });
                        }
                    }
                }
            }
            $scope.uploadAvatar = function (input) {
                $scope.inputAvatar = input;
                if (input.files && input.files.length > 0) {
                    $timeout(function () {
                        var fileReader = new FileReader();
                        fileReader.readAsDataURL(input.files[0]);
                        fileReader.onload = function (e) {
                            $timeout(function () {
                                $scope.fileAvatar.src = e.target.result;
                            });
                        }
                    });
                    $scope.fileAvatar.file = input.files[0];
                }
            };
            $scope.deleteAvatar = function () {
                if ($scope.target.avatar) {
                    $scope.target.avatar = null;
                }
                if ($scope.fileAvatar) {
                    $scope.fileAvatar = {};
                    angular.element("#file-input-ava").val(null);
                }
            }
            function saveAvatar() {
                $scope.fileAvatar.maVatTu = $scope.target.maVatTu;
                upload.upload({
                    url: configService.rootUrlWebApi + '/Md/Merchandise/UploadAvatar',
                    data: $scope.fileAvatar
                }).then(function (response) {
                    if (response.status) {
                        $scope.target.avatarName = response.data.data;
                        saveMerchandise();
                    }
                    else {
                        ngNotify.set('Không lưu được ảnh! Có thể đã trùng!', { duration: 3000, type: 'error' });
                    }
                });
            }
            function saveImage() {
                $scope.target.file = $scope.lstFile;
                upload.upload({
                    url: configService.rootUrlWebApi + '/Md/Merchandise/UploadMerchandiseImage',
                    data: $scope.target
                }).then(function (response) {
                    if (response.status) {
                        $scope.target.image = response.data.data;
                    }
                    else {
                        ngNotify.set('Không lưu được ảnh! Có thể đã trùng!', { duration: 3000, type: 'error' });
                    }
                    if ($scope.fileAvatar) {
                        saveAvatar();
                    }
                    else {
                        saveMerchandise();
                    }
                });
            }
            $scope.save = function () {
                if (!$scope.target.maLoaiVatTu) {
                    ngNotify.set("Chưa khai báo mã loại vật tư", { duration: 3000, type: 'error' });
                } else if (!$scope.target.maNhomVatTu) {
                    ngNotify.set("Chưa khai báo mã nhóm vật tư", { duration: 3000, type: 'error' });
                } else if (!$scope.target.maVatTu) {
                    ngNotify.set("Chưa khai báo mã vật tư", { duration: 3000, type: 'error' });
                } else if (!$scope.target.maKhachHang) {
                    ngNotify.set("Chưa khai báo mã nhà cung cấp", { duration: 3000, type: 'error' });
                } else {
                    $scope.target.image = '';
                    if ($scope.lstFile && $scope.lstFile.length) {
                        saveImage();
                    } else {
                        if ($scope.fileAvatar) {
                            saveAvatar();
                        } else {
                            saveMerchandise();
                        }
                    }
                }
            };

            $scope.cancel = function () {
                $uibModalInstance.close();
            };
        }]);
    /* controller Details */
    app.controller('merchandiseDetailsController', ['$scope', '$uibModalInstance', 'tempDataService', '$filter', 'targetData', 'securityService', 'configService', 'merchandiseService',
        function ($scope, $uibModalInstance, tempDataService, $filter, targetData, securityService, configService, service) {
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.robot = service.robot;
            $scope.targetData = angular.copy(targetData);
            $scope.tempData = tempDataService.tempData;
            $scope.target = targetData;
            $scope.isLoading = false;
            $scope.lstImagesSrcFromDb = [];
            $scope.title = function () { return 'Chi tiết Hàng hóa, Vật tư'; };
            $scope.nhomVatTus = [];
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
            $scope.displayHepler = function (paraValue, moduleName) {
                var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                if (data && data.length === 1) {
                    return data[0].text;
                } else {
                    return paraValue;
                }
            };
            function filterData() {
                service.getDetailByCode($scope.target.maVatTu).then(function (response) {
                    if (response && response.status == 200 && response.data) {
                        $scope.target = response.data;
                        if ($scope.target.image) {
                            var arrImage = $scope.target.image.split(',');
                            arrImage.splice(arrImage.length - 1, 1);
                            $scope.lstImagesSrcFromDb = arrImage;
                        }
                        $scope.target.maSizes = [];
                        if ($scope.target.maSize) {
                            var lst = $scope.target.maSize.split(',');
                            angular.forEach(lst, function (item) {
                                var tmp = $filter('filter')($scope.tempData('sizes'), { value: item }, true);
                                if (tmp && tmp.length) {
                                    $scope.target.maSizes.push(tmp[0]);
                                }
                            });
                        }
                        $scope.target.maColors = [];
                        if ($scope.target.maColor) {
                            var lst = $scope.target.maColor.split(',');
                            angular.forEach(lst, function (item) {
                                var tmp = $filter('filter')($scope.tempData('colors'), { value: item }, true);
                                if (tmp && tmp.length) {
                                    $scope.target.maColors.push(tmp[0]);
                                }
                            });
                        }
                        if ($scope.target.maBaoBi) {
                            service.getBaoBiByCode($scope.target.maBaoBi).then(function (response) {
                                if (response.status && response.data.status) {
                                    $scope.searchMaBaoBi = response.data.data.soLuong;
                                }
                            });
                        }
                        if ($scope.target.maLoaiVatTu) {
                            service.getSelectByMaLoai($scope.target.maLoaiVatTu).then(function (response) {
                                $scope.nhomVatTus = response;
                            });
                        }
                        $scope.pageChanged();
                    }
                });
            }
            $scope.accessList = {};
            //check quyền truy cập
            function loadAccessList() {
                securityService.getAccessList('merchandise').then(function (successRes) {
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
                    toaster.pop('error', "Lỗi:", "Không có quyền truy cập !");
                    $scope.accessList = null;
                });
            }
            loadAccessList();
            //end
            $scope.cancel = function () {
                $uibModalInstance.close();
            };

        }]);
    /* controller delete */
    app.controller('merchandiseDeleteController', ['$scope', '$uibModalInstance', 'configService', 'merchandiseService', 'targetData', 'ngNotify',
        function ($scope, $uibModalInstance, configService, service, targetData, ngNotify) {
            $scope.config = angular.copy(configService);
            $scope.targetData = angular.copy(targetData);
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

    /* controller Import Excel*/
    app.controller('merchandiseImportExcelController', ['$scope', '$uibModalInstance', 'configService', 'ngNotify', 'FileUploader', 'userService',
        function ($scope, $uibModalInstance, configService, ngNotify, fileUploader, serviceAuthUser) {
            $scope.config = angular.copy(configService);
            var currentUser = serviceAuthUser.GetCurrentUser();
            var unitCode = currentUser.unitCode;
            $scope.isLoading = false;
            $scope.title = function () { return 'Import Data'; };
            var serviceUrl = configService.rootUrlWebApi + '/Md/Merchandise';
            var uploader = $scope.uploader = new fileUploader({
                url: serviceUrl + '/UploadFile/' + unitCode
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
                if (status === 200) {
                    ngNotify.set('Thành công', { type: 'success' });
                    $uibModalInstance.dismiss('cancel');
                }
            };
            $scope.cancel = function () {
                $uibModalInstance.close();
            };
        }]);

    /* controller SelectData*/
    app.controller('merchandiseSelectDataController', ['$scope', '$uibModalInstance', 'configService', 'merchandiseService', '$uibModal', '$log', 'ngNotify', 'filterObject',
        function ($scope, $uibModalInstance, configService, service, $uibModal, $log, ngNotify, filterObject) {
            $scope.config = angular.copy(configService);
            $scope.filtered = angular.copy(configService.filterDefault);
            $scope.filtered = angular.extend($scope.filtered, filterObject);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.isLoading = false;
            $scope.title = function () { return 'Hàng hóa, Vật tư'; };
            $scope.cancel = function () {
                $uibModalInstance.close();
            };
            function filterData() {
                $scope.listSelectedData = service.getSelectData();
                $scope.isLoading = true;
                var postdata = { paged: $scope.paged, filtered: $scope.filtered };
                service.postSelectDataQuery(postdata).then(function (successRes) {
                    if (successRes && successRes.status === 200 && successRes.data && successRes.data.status) {
                        $scope.isLoading = false;
                        $scope.data = successRes.data.data.data;
                        angular.forEach($scope.data, function (v, k) {
                            var isSelected = $scope.listSelectedData.some(function (element, index, array) {
                                if (!element) return false;
                                return element.maHang == v.maHang;
                            });
                            if (isSelected) {
                                $scope.data[k].selected = true;
                            }
                        });
                        angular.extend($scope.paged, successRes.data.data);
                        if (successRes.message) {
                            ngNotify.set(successRes.message, { duration: 3000, type: 'error' });
                        }
                    }
                });
                $scope.cancel = function () {
                    $uibModalInstance.dismiss('cancel');
                };
            };
            $scope.setPage = function (pageNo) {
                $scope.paged.currentPage = pageNo;
                filterData();
            };
            $scope.selecteItem = function (item) {
                $uibModalInstance.close(item);
            }
            $scope.sortType = 'maVatTu'; // set the default sort type
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
                                $scope.listSelectedData.push($scope.data[k]);
                            }
                        } else {
                            if (isSelected) {
                                $scope.listSelectedData.splice($scope.data[k], 1);
                            }
                        }
                    });
                }
            };
            filterData();
            $scope.create = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Merchandise', 'add'),
                    controller: 'merchandiseCreateController',
                    windowClass: 'app-modal-window',
                    resolve: {}
                });
                modalInstance.result.then(function (updatedData) {
                    $scope.refresh();
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };
        }]);

    /* controller Simple Select Data*/
    app.controller('merchandiseSimpleSelectDataController', ['$scope', '$uibModalInstance', 'configService', 'merchandiseService', '$uibModal', '$log', 'serviceSelectData', 'filterObject',
        function ($scope, $uibModalInstance, configService, service, $uibModal, $log, serviceSelectData, filterObject) {
            $scope.config = angular.copy(configService);
            $scope.filtered = angular.copy(configService.filterDefault);
            $scope.filtered = angular.extend($scope.filtered, filterObject);
            angular.extend($scope.filtered, filterObject);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.isLoading = false;
            $scope.title = function () { return 'Hàng hóa'; };
            function filterData() {
                $scope.listSelectedData = serviceSelectData.getSelectData();
                $scope.isLoading = true;
                var postdata = { paged: $scope.paged, filtered: $scope.filtered };
                service.postSelectData(postdata).then(function (successRes) {
                    if (successRes && successRes.status === 200 && successRes.data && successRes.data.status) {
                        $scope.isLoading = false;
                        $scope.data = successRes.data.data.data;
                        angular.forEach($scope.data, function (v, k) {
                            var isSelected = $scope.listSelectedData.some(function (element, index, array) {
                                if (!element) return false;
                                return element.value == v.value;
                            });
                            if (isSelected) {
                                $scope.data[k].selected = true;
                            }
                        });
                        angular.extend($scope.paged, successRes.data.data);
                    }
                });
                $scope.cancel = function () {
                    $uibModalInstance.dismiss('cancel');
                };
            };
            filterData();
            $scope.setPage = function (pageNo) {
                $scope.paged.currentPage = pageNo;
                filterData();
            };
            $scope.selecteItem = function (item) {
                $uibModalInstance.close(item);
            }
            $scope.sortType = 'maVatTu'; // set the default sort type
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
                return 'Hàng hóa, Vật tư';
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
            $scope.create = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Merchandise', 'add'),
                    controller: 'merchandiseCreateController',
                    windowClass: 'app-modal-window',
                    resolve: {}
                });
                modalInstance.result.then(function (updatedData) {
                    $scope.refresh();
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };
            $scope.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };
            $scope.save = function () {
                $uibModalInstance.close($scope.listSelectedData);
            };
        }]);

    /* controller export Item mat hang */
    app.controller('merchandiseExportItemController', ['$scope', '$uibModalInstance', 'configService', 'merchandiseService', '$uibModal', 'ngNotify', '$window', 'toaster', 'userService', 'FileUploader',
        function ($scope, $uibModalInstance, configService, service, $uibModal, ngNotify, $window, toaster, serviceAuthUser, FileUploader) {
            var currentUser = serviceAuthUser.GetCurrentUser();
            var unitCode = currentUser.unitCode;
            $scope.robot = angular.copy(service.robot);
            $scope.config = angular.copy(configService);
            $scope.target = { dataDetails: [] };
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.isLoading = false;
            $scope.title = function () { return 'In tem hàng hóa'; };
            $scope.hrefTem = configService.apiServiceBaseUri + "/Upload/Barcode/Barcode.xls";
            $scope.isListItemNull = true;
            var serviceUrl = configService.rootUrlWebApi + '/Md/Merchandise';
            var uploader = $scope.uploader = new FileUploader({
                url: serviceUrl + '/ImportExcelPrintItem/' + unitCode,
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
                if (status === 200 && response.status && response.data && response.data.length > 0) {
                    angular.forEach(response.data, function (value, index) {
                        $scope.newItem = {};
                        $scope.newItem.maHang = value.masieuthi;
                        $scope.newItem.tenHang = value.tenviettat;
                        $scope.newItem.barcode = value.barcode;
                        $scope.newItem.giaBanLeVat = value.giabanlecovat;
                        $scope.newItem.Giabanbuoncovat = value.Giabanbuoncovat;
                        $scope.newItem.giaBanLe = value.giabanle;
                        $scope.newItem.giaBanBuon = value.giabanbuon;
                        $scope.newItem.maKhachHang = value.makhachhang;
                        $scope.newItem.soLuong = value.soluong;
                        $scope.target.dataDetails.push($scope.newItem);
                    });
                    $scope.newItem = {};
                    $scope.pageChanged();
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
            $scope.selectedMaHang = function (code) {
                if (code) {
                    service.getMerchandiseForNvByCode(code, null, unitCode).then(function (response) {
                        if (response && response.status == 200 && response.data && response.data.status) {
                            $scope.newItem = response.data.data;
                            $scope.newItem.validateCode = response.data.data.maVatTu;
                            $scope.newItem.soLuong = 1;
                        } else {
                            $scope.addNewItem(code);
                        }
                    });
                }
            };
            $scope.addRow = function () {
                if (!$scope.newItem.soLuong || $scope.newItem.soLuong < 1) {
                    document.getElementById('soLuong').focus();
                    var exsist = $scope.target.dataDetails.some(function (element, index, array) {
                        return $scope.newItem.maHang.toUpperCase() == element.maHang;
                    });
                    if (exsist) {
                        toaster.pop("Thông báo:", "Mã hàng này bạn đã nhập rồi!");
                    }
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
                            return service;
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
                        $scope.newItem.soTonMax = updatedData.soLuong;
                        $scope.newItem.validateCode = updatedData.maHang;
                    }
                    $scope.pageChanged();
                }, function () {
                });
            }

            function filterData() {
                service.clearSelectData();
                $scope.target.dataDetails = service.getSelectData();
                //service.getNewCode().then(function (response) {
                //    if (response && response.status == 200 && response.data) {
                //        $scope.target.maBoHang = response.data;
                //    }
                //});
            };
            filterData();
            $scope.removeItem = function (index) {
                var currentPage = $scope.paged.currentPage;
                var itemsPerPage = $scope.paged.itemsPerPage;
                var currentPageIndex = (currentPage - 1) * itemsPerPage + index;
                $scope.target.dataDetails.splice(currentPageIndex, 1);
                $scope.pageChanged();
            }
            $scope.setIndex = function (index) {
                $scope.selectedRow = index;
            }

            $scope.exportToExcel = function () {
                service.writeDataToExcel($scope.target.dataDetails).then(function (response) {
                    if (response.status) {
                        ngNotify.set("Thành công", { type: 'success' });
                        $window.location = $scope.hrefTem;
                    }
                    else {
                        ngNotify.set(response.Message, { duration: 3000, type: 'error' });
                    }

                });
            }
            $scope.cancel = function () {
                $scope.target.dataDetails.length = 0;
                $uibModalInstance.dismiss('cancel');
            };
        }]);

    /* controller Select DataSQL Controller*/
    app.controller('merchandiseSelectDataSQLController', ['$scope', '$uibModalInstance', 'configService', 'merchandiseService', '$uibModal', '$log', 'targetData', 'ngNotify', 'serviceSelectData', 'filterObject',
        function ($scope, $uibModalInstance, configService, service, $uibModal, $log, targetData, ngNotify, serviceSelectData, filterObject) {
            $scope.config = angular.copy(configService);
            $scope.targetData = angular.copy(targetData);
            $scope.filtered = angular.copy(configService.filterDefault);
            $scope.filtered = angular.extend($scope.filtered, filterObject);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.target = targetData;
            $scope.isLoading = false;
            $scope.title = function () { return 'Import Data'; };
            function filterData() {
                $scope.listSelectedData = serviceSelectData.getSelectData();
                $scope.isLoading = true;
                var postdata = { paged: $scope.paged, filtered: $scope.filtered };
                service.postSelectDataSQLQuery(postdata).then(function (successRes) {
                    if (successRes && successRes.status === 200 && successRes.data && successRes.data.status) {
                        $scope.isLoading = false;
                        $scope.data = successRes.data.data.data;
                        angular.forEach($scope.data, function (v, k) {
                            var isSelected = $scope.listSelectedData.some(function (element, index, array) {
                                if (!element) return false;
                                return element.maHang == v.maHang;
                            });
                            if (isSelected) {
                                $scope.data[k].selected = true;
                            }
                        });
                        angular.extend($scope.paged, response.data);
                        if (response.message) {
                            ngNotify.set(response.message, { duration: 3000, type: 'error' });
                        }
                    }
                });
                $scope.cancel = function () {
                    $uibModalInstance.dismiss('cancel');
                };
            };
            $scope.setPage = function (pageNo) {
                $scope.paged.currentPage = pageNo;
                filterData();
            };
            $scope.selecteItem = function (item) {
                $uibModalInstance.close(item);
            }
            $scope.sortType = 'maVatTu'; // set the default sort type
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
                return 'Hàng hóa, Vật tư';
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
            $scope.create = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Merchandise', 'add'),
                    controller: 'merchandiseCreateController',
                    windowClass: 'app-modal-window',
                    resolve: {}
                });
                modalInstance.result.then(function (updatedData) {
                    $scope.refresh();
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };
            $scope.cancel = function () {
                serviceSelectData.getSelectData().clear();
                $uibModalInstance.dismiss('cancel');
            };
        }]);
    /* controller export Excel Controller*/
    app.controller('exportExcelController', ['$scope', '$uibModalInstance', 'configService', 'merchandiseService', 'merchandiseTypeService',
        function ($scope, $uibModalInstance, configService, service, merchandiseTypeService) {
            $scope.config = angular.copy(configService);
            $scope.filtered = angular.copy(configService.filterDefault);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.isLoading = false;
            $scope.listSelectedData = [];
            $scope.title = function () { return 'Kết xuất excel'; };
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
                return 'Loại hàng';
            };
            $scope.setPage = function (pageNo) {
                $scope.paged.currentPage = pageNo;
                filterData();
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
                            $scope.listSelectedData.splice(item, 1);
                        }
                    }
                    service.setSelectData($scope.listSelectedData);
                } else {
                    angular.forEach($scope.data, function (v, k) {
                        $scope.data[k].selected = $scope.all;
                        var isSelected = $scope.listSelectedData.some(function (element, index, array) {
                            if (!element) return false;
                            return element.id == v.id;
                        });

                        if ($scope.all) {
                            if (!isSelected) {
                                $scope.listSelectedData.push($scope.data[k]);
                            }
                        } else {
                            if (isSelected) {
                                $scope.listSelectedData.splice($scope.data[k], 1);
                            }
                        }
                    });
                    service.setSelectData($scope.listSelectedData);
                }
            }
            function filterData() {
                var postdata = {};
                $scope.isLoading = true;
                postdata = { paged: $scope.paged, filtered: $scope.filtered };
                merchandiseTypeService.postSelectData(postdata).then(function (response) {
                    $scope.isLoading = false;
                    if (response && response.status == 200 && response.data && response.data.status) {
                        $scope.data = response.data.data.data;
                        angular.extend($scope.paged, response.data.data);
                    }
                });
            }

            filterData();
            $scope.export = function () {
                service.postExportExcel($scope.listSelectedData);
                service.clearSelectData();
                $uibModalInstance.close();
            };
            $scope.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };
        }]);

    /* controller Child Controller */
    app.controller('merchandiseChildController', ['$scope', '$uibModalInstance', 'configService', 'merchandiseService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData',
        function ($scope, $uibModalInstance, configService, service, tempDataService, $filter, $uibModal, $log, targetData) {
            $scope.robot = service.robot;
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.tempData = tempDataService.tempData;
            $scope.parentTarget = targetData;
            function filterData() {
                $scope.isLoading = true;
                service.getAllDataChild($scope.parentTarget.maVatTu).then(function (response) {
                    $scope.isLoading = false;
                    if (response.status) {
                        $scope.target = response.data.data;
                        $scope.pageChanged();
                    }
                });
            };
            $scope.setPage = function (pageNo) {
                $scope.paged.currentPage = pageNo;
                filterData();
            };
            $scope.pageChanged = function () {
                var currentPage = $scope.paged.currentPage;
                var itemsPerPage = $scope.paged.itemsPerPage;
                $scope.paged.totalItems = $scope.target.length;
                $scope.data = [];
                if ($scope.target) {
                    for (var i = (currentPage - 1) * itemsPerPage; i < currentPage * itemsPerPage && i < $scope.target.length; i++) {
                        $scope.data.push($scope.target[i])
                    }
                }
            }
            $scope.refresh = function () {
                $scope.setPage($scope.paged.currentPage);
            };
            $scope.displayHepler = function (paraValue, moduleName) {
                var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                if (data && data.length === 1) {
                    return data[0].text;
                } else {
                    return paraValue;
                }
            };
            function loadTax() {
                if (!tempDataService.tempData('taxs')) {
                    serviceTax.getAll_Tax().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                            tempDataService.putTempData('taxs', successRes.data.data);
                            $scope.taxs = successRes.data.data;
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.taxs = tempDataService.tempData('taxs');
                }
            }

            $scope.getNameVAT = function (paraValue, moduleName) {
                var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                if (data && data.length === 1) {
                    return data[0].description;
                } else {
                    return paraValue;
                }
            }

            $scope.isLoading = false;
            $scope.title = function () { return 'Mã con Hàng hóa, Vật tư'; };
            $scope.create = function (target) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Merchandise', 'child-add'),
                    controller: 'merchandiseChildCreateController',
                    windowClass: 'app-modal-window',
                    resolve: {
                        targetData: function () {
                            return $scope.parentTarget;
                        }
                    }
                });

                modalInstance.result.then(function (updatedData) {
                    $scope.refresh();
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };
            $scope.updatePrice = function (item) {
                service.getPrice(item.maVatTu, function (response) {
                    var modalInstance = $uibModal.open({
                        backdrop: 'static',
                        templateUrl: configService.buildUrl('htdm/Merchandise', 'price-edit'),
                        controller: 'merchandisePriceDirectoryEditController',
                        resolve: {
                            targetData: function () {
                                return response;
                            }
                        }
                    });
                    modalInstance.result.then(function (updatedData) {

                    }, function () {
                        $log.info('Modal dismissed at: ' + new Date());
                    });

                });
            }

            $scope.update = function (target) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Merchandise', 'child-update'),
                    controller: 'merchandiseChildEditController',
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

            $scope.deleteItemChild = function (event, target) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Merchandise', 'delete'),
                    controller: 'merchandiseDeleteController',
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

            $scope.updateGia = function (target) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Merchandise', 'price-edit'),
                    controller: 'merchandisePriceEditController',
                    resolve: {
                        targetData: function () {
                            return target;
                        }
                    }
                });
                modalInstance.result.then(function (updatedData) {
                    var index = $scope.target.dataDetails.indexOf(target);
                    if (index !== -1) {
                        $scope.target.dataDetails[index] = updatedData;
                    }
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };


            $scope.details = function (target) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Merchandise', 'child-detail'),
                    controller: 'merchandiseChildDetailsController',
                    windowClass: 'app-modal-window',
                    resolve: {
                        targetData: function () {
                            return target;
                        }
                    }
                });
            };

            $scope.printITem = function (target) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Merchandise', 'printItem'),
                    controller: 'merchandiseExportItemController',
                    windowClass: 'app-modal-window',
                    resolve: {}

                });
                modalInstance.result.then(function (updatedData) {
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };
            $scope.dayCanDienTu = function () {
                service.getByItemCodeNotNull().then(function (response) {
                    $window.location.href = configService.apiServiceBaseUri + "/Upload/Barcode/MaCanDienTu.xls";
                });
            };
            filterData();
            $scope.goHome = function () {
                $uibModalInstance.dismiss('cancel');
            };
            $scope.cancel = function () {
                $uibModalInstance.close();
            };
        }]);


    /* controller merchandise Child Create Controller */
    app.controller('merchandiseChildCreateController', ['$scope', '$uibModalInstance', 'configService', 'merchandiseService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'taxService', 'Upload', 'targetData', 'userService',
        function ($scope, $uibModalInstance, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, serviceTax, upload, targetData, serviceAuthUser) {
            var currentUser = serviceAuthUser.GetCurrentUser();
            $scope.robot = service.robot;
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.tempData = tempDataService.tempData;
            $scope.target = { dataDetails: [] };
            $scope.maVatTu = '';
            $scope.target.maSizes = [];
            $scope.target.maColors = [];
            $scope.target.useGenCode = true;
            $scope.tempMaVatTu = '';
            $scope.maMauSelected = [];
            $scope.maSizeSelected = [];
            $scope.lstImagesSrc = [];
            $scope.lstFile = [];
            $scope.fileAvatar = {};
            $scope.parentTarget = targetData;
            $scope.disableCreateGiaBan = false;
            //$scope.tempData('status') = angular.copy(mdService.tempData('status'));


            $scope.displayHepler = function (paraValue, moduleName) {
                var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                if (data && data.length === 1) {
                    return data[0].text;
                } else {
                    return paraValue;
                }
            };
            $scope.refresh = function () {
                $scope.setPage($scope.paged.currentPage);
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
                if ($scope.data) {
                    if ($scope.data.length == 1) {
                        $scope.disableCreateGiaBan = true;
                    }
                    else {
                        $scope.disableCreateGiaBan = false;
                    }
                }
                else {
                    $scope.disableCreateGiaBan = false;
                }
            }
            //declare
            function filterData() {
                service.getDetailByCode($scope.parentTarget.maVatTu).then(function (response) {
                    if (response && response.status == 200 && response.data) {
                        $scope.parentTarget = response.data;
                        $scope.target = angular.copy($scope.parentTarget);
                        $scope.target.maCha = $scope.parentTarget.maVatTu;
                        service.getNewCodeChild($scope.parentTarget.maVatTu).then(function (resChildren) {
                            if (resChildren && resChildren.status == 200 && resChildren.data) {
                                $scope.target.maVatTu = resChildren.data;
                                $scope.maVatTu = resChildren.data;
                                $scope.tempMaVatTu = angular.copy($scope.target.maVatTu);
                                $scope.target.useGenCode = true;
                                $scope.target.maSizes = [];
                                $scope.target.maColors = [];
                            }
                        });
                        var maVatTu = $scope.target.maVatTu;
                        angular.forEach($scope.target.dataDetails, function (item) {
                            item.maVatTu = maVatTu;
                        });
                        if ($scope.parentTarget.isCanDienTu) {
                            service.getNewCanDienTu().then(function (response) {
                                if (response && response.status === 200 && response.data) {
                                    $scope.target.itemCode = response.data;
                                }
                            });

                        }
                        service.getSelectByMaLoai($scope.target.maLoaiVatTu).then(function (response) {
                            $scope.nhomVatTus = response;
                        });
                        $scope.pageChanged();
                    }
                });

            }
            filterData();
            $scope.isLoading = false;
            $scope.title = function () { return 'Tạo mã con hàng hóa, vật tư'; };

            $scope.target.useGenCode = true;
            $scope.tempMaVatTu = '';
            $scope.tempEnterMechandise = '';
            //end
            $scope.changeLoaiVatTu = function () {
                service.getSelectByMaLoai($scope.target.maLoaiVatTu).then(function (response) {
                    $scope.nhomVatTus = response;
                });
            }
            $scope.addBarCode = function (target) {
                if (!target.barcode) {
                    target.barcode = ";";
                }
                target.barcode = target.barcode + target.newBarcode + ";";
                target.newBarcode = "";
            }
            $scope.changeVatVao = function (code) {
                if (code) {
                    serviceTax.getTaxByCode(code).then(function (response) {
                        if (response && response.status == 200 && response.data) {
                            $scope.target.tyLeVatVao = response.data.taxRate;
                            angular.forEach($scope.target.dataDetails, function (v, k) {
                                $scope.target.dataDetails[k].maVatVao = $scope.target.maVatVao;
                                $scope.target.dataDetails[k].tyLeVatVao = $scope.target.tyLeVatVao;
                                $scope.target.dataDetails[k].giaMuaVat = $scope.target.dataDetails[k].giaMua * $scope.target.dataDetails[k].tyLeVatVao / 100 + $scope.target.dataDetails[k].giaMua;
                            });
                        }
                    });
                } else {
                    $scope.target.tyLeVatVao = 0;
                    angular.forEach($scope.target.dataDetails, function (v, k) {
                        $scope.target.dataDetails[k].maVatVao = $scope.target.maVatVao;
                        $scope.target.dataDetails[k].tyLeVatVao = $scope.target.tyLeVatVao;
                        $scope.target.dataDetails[k].giaMuaVat = $scope.target.dataDetails[k].giaMua * $scope.target.dataDetails[k].tyLeVatVao / 100 + $scope.target.dataDetails[k].giaMua;
                    });
                }
            }
            $scope.changeVatRa = function (code) {
                if (code) {
                    serviceTax.getTaxByCode(code).then(function (response) {
                        if (response && response.status == 200 && response.data) {
                            $scope.target.tyLeVatRa = response.data.taxRate;
                            angular.forEach($scope.target.dataDetails, function (v, k) {
                                $scope.target.dataDetails[k].maVatRa = $scope.target.maVatRa;
                                $scope.target.dataDetails[k].giaBanBuonVat = $scope.target.giaBanBuon * $scope.target.tyLeVatRa / 100 + $scope.target.giaBanBuon;
                                $scope.target.dataDetails[k].giaBanLeVat = $scope.target.giaBanLe * $scope.target.tyLeVatRa / 100 + $scope.target.giaBanLe;
                            });
                        }
                    });
                } else {
                    $scope.target.tyLeVatRa = 0;
                    angular.forEach($scope.target.dataDetails, function (v, k) {
                        $scope.target.dataDetails[k].maVatRa = $scope.target.maVatRa;
                        $scope.target.dataDetails[k].giaBanBuonVat = $scope.target.giaBanBuon * $scope.target.tyLeVatRa / 100 + $scope.target.giaBanBuon;
                        $scope.target.dataDetails[k].giaBanLeVat = $scope.target.giaBanLe * $scope.target.tyLeVatRa / 100 + $scope.target.giaBanLe;
                    });
                }

            }
            //add
            $scope.createMerchandiseType = function (target, name) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/MerchandiseType', 'add'),
                    controller: 'merchandiseTypeCreateController',
                    resolve: {}
                });
                modalInstance.result.then(function (updatedData) {
                    $scope.tempData.update('merchandiseTypes', function () {
                        if (target && name) {
                            target[name] = updatedData.mdNhomVatTu;
                        }

                    });
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };

            $scope.createNhomVatTu = function (target, name) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/NhomVatTu', 'add'),
                    controller: 'nhomVatTuCreateController',
                    resolve: {}
                });
                modalInstance.result.then(function (updatedData) {
                    $scope.tempData.update('nhomVatTus', function () {
                        if (target && name) {
                            target[name] = updatedData.mdNhomVatTu;
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
                    target.maBaoBi = updatedData.maBaoBi;
                    $scope.searchMaBaoBi = updatedData.soLuong;
                    var ob = {
                        description: updatedData.tenBaoBi,
                        extendValue: updatedData.soLuong,
                        value: updatedData.maBaoBi,
                        text: updatedData.maBaoBi + "|" + updatedData.tenBaoBi,
                    };
                    $scope.packagings.push(ob);
                    tempDataService.putTempData('packagings', $scope.packagings);
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };
            $scope.filterQuyCach = function (code) {
                if (code) {
                    var data = $filter('filter')($scope.tempData('packagings'), { value: code }, true);
                    if (data.length > 0) {
                        $scope.searchMaBaoBi = angular.copy(data[0].extendValue);
                    }
                }
            };
            $scope.createShelve = function (target, name) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Shelves', 'add'),
                    controller: 'shelvesCreateController',
                    resolve: {}
                });
                modalInstance.result.then(function (updatedData) {
                    $scope.tempData.update('shelves', function () {
                        if (target && name) {
                            target[name] = updatedData.maKeHang;
                        }
                    });
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };
            $scope.createSize = function (target, name) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Size', 'add'),
                    controller: 'sizeCreateController',
                    resolve: {}
                });
                modalInstance.result.then(function (updatedData) {
                    $scope.tempData.update('sizes', function () {
                        if (target && name) {
                            var data = $filter('filter')($scope.tempData('sizes'), { value: updatedData.maSize }, true);
                            if (data && data.length) {
                                target[name].push(data[0]);
                            }
                        }
                    });
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };
            $scope.createColor = function (target, name) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Color', 'add'),
                    controller: 'colorCreateController',
                    resolve: {}
                });
                modalInstance.result.then(function (updatedData) {
                    $scope.tempData.update('colors', function () {
                        if (target && name) {
                            var data = $filter('filter')($scope.tempData('colors'), { value: updatedData.maColor }, true);
                            if (data && data.length) {
                                target[name].push(data[0]);
                            }
                        }
                    });
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };
            //end
            $scope.update = function (target) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Merchandise', 'price-edit'),
                    controller: 'merchandisePriceEditController',
                    resolve: {
                        targetData: function () {
                            return target;
                        },
                        initData: {
                            maVatRa: $scope.target.maVatRa,
                            maVatVao: $scope.target.maVatVao,
                            tyLeVatRa: $scope.target.tyLeVatRa,
                            tyLeVatVao: $scope.target.tyLeVatVao
                        }
                    }
                });
                modalInstance.result.then(function (updatedData) {
                    var index = $scope.target.dataDetails.indexOf(target);
                    if (index !== -1) {
                        $scope.target.dataDetails[index] = updatedData;
                    }
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };

            $scope.removeItem = function (index) {
                var currentPage = $scope.paged.currentPage;
                var itemsPerPage = $scope.paged.itemsPerPage;
                var currentPageIndex = (currentPage - 1) * itemsPerPage + index;
                $scope.target.dataDetails.splice(currentPageIndex, 1);
                $scope.pageChanged();
            }
            $scope.create = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Merchandise', 'price'),
                    controller: 'merchandisePriceCreateController',
                    resolve: {
                        initData: {
                            maVatRa: $scope.target.maVatRa,
                            maVatVao: $scope.target.maVatVao,
                            tyLeVatRa: $scope.target.tyLeVatRa,
                            tyLeVatVao: $scope.target.tyLeVatVao
                        }
                    }
                });
                modalInstance.result.then(function (updatedData) {
                    var checkIfExsit = $scope.target.dataDetails.some(function (element, index, array) {
                        return element.maDonVi === updatedData.maDonVi;
                    });
                    if (!checkIfExsit) {
                        $scope.target.dataDetails.push(updatedData);
                        $scope.pageChanged();
                    }
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };
            $scope.enterCodeMerchandise = function (codeMerchandise) {
                if ($scope.target.isUpdateCodeMerchandise === 'OK' && codeMerchandise != '') {
                    var temp = codeMerchandise.split('-');
                    $scope.maVatTu = temp[0];
                }
            };
            function change_alias(alias) {
                var str = alias;
                str = str.toLowerCase();
                str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
                str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
                str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
                str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
                str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
                str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
                str = str.replace(/đ/g, "d");
                str = str.replace(/!|@|%|\^|\*|\(|\)|\+|\=|\<|\>|\?|\/|,|\.|\:|\;|\'|\"|\&|\#|\[|\]|~|\$|_|`|-|{|}|\||\\/g, " ");
                str = str.replace(/ + /g, " ");
                str = str.trim();
                return str;
            }
            $scope.genMaVatTu = function () {
                var result = $scope.maVatTu;
                if ($scope.maMauSelected.length > 0) result += '-';
                for (var i = 0; i < $scope.maMauSelected.length; i++) {
                    if (i < $scope.maMauSelected.length - 1)
                        result += $scope.maMauSelected[i] + '-';
                    else
                        result += $scope.maMauSelected[i];
                }
                if ($scope.maSizeSelected.length > 0) result += '-';
                for (var i = 0; i < $scope.maSizeSelected.length; i++) {
                    if (i < $scope.maSizeSelected.length - 1)
                        result += $scope.maSizeSelected[i] + '-';
                    else
                        result += $scope.maSizeSelected[i];
                }
                $scope.target.maVatTu = result;
            };
            $scope.changeColor = function (lstColor) {
                $scope.maMauSelected = [];
                angular.forEach(lstColor, function (value, key) {
                    value.text = change_alias(value.text);
                    value.text = value.text.toUpperCase();
                    value.text = value.text.replace(" ", "");
                    if ($scope.maMauSelected.indexOf(value.text) != -1) {
                    }
                    else
                        $scope.maMauSelected.push(value.text);
                });
                $scope.genMaVatTu();
            };
            $scope.removeColor = function (lstColor) {
                $scope.maMauSelected = [];
                angular.forEach(lstColor, function (value, key) {
                    value.text = change_alias(value.text);
                    value.text = value.text.toUpperCase();
                    value.text = value.text.replace(" ", "");
                    if ($scope.maMauSelected.indexOf(value.text) != -1) {
                    }
                    else
                        $scope.maMauSelected.push(value.text);
                });
                $scope.genMaVatTu();
            };
            $scope.changeSize = function (lstSize) {
                $scope.maSizeSelected = [];
                angular.forEach(lstSize, function (value, key) {
                    value.text = change_alias(value.text);
                    value.text = value.text.toUpperCase();
                    value.text = value.text.replace(" ", "");
                    value.text = value.text.replace("SIZE", "");
                    if ($scope.maSizeSelected.indexOf(value.text) != -1) { }
                    else $scope.maSizeSelected.push(value.text);
                });
                $scope.genMaVatTu();
            };
            $scope.removeSize = function (lstSize) {
                $scope.maSizeSelected = [];
                angular.forEach(lstSize, function (value, key) {
                    value.text = change_alias(value.text);
                    value.text = value.text.toUpperCase();
                    value.text = value.text.replace(" ", "");
                    value.text = value.text.replace("SIZE", "");
                    if ($scope.maSizeSelected.indexOf(value.text) != -1) { }
                    else $scope.maSizeSelected.push(value.text);
                });
                $scope.genMaVatTu();
            };
            $scope.uploadFile = function (input) {
                if (input.files && input.files.length > 0) {
                    angular.forEach(input.files, function (file) {
                        $scope.lstFile.push(file);
                        $timeout(function () {
                            var fileReader = new FileReader();
                            fileReader.readAsDataURL(file);
                            fileReader.onload = function (e) {
                                $timeout(function () {
                                    $scope.lstImagesSrc.push(e.target.result);
                                });
                            }
                        });
                    });
                }
            };
            $scope.deleteImage = function (index) {
                $scope.lstImagesSrc.splice(index, 1);
                $scope.lstFile.splice(index, 1);
                if ($scope.lstFile.length < 1) {
                    angular.element("#file-input-upload").val(null);
                }
            };
            $scope.uploadAvatar = function (input) {
                $scope.inputAvatar = input;
                if (input.files && input.files.length > 0) {
                    $timeout(function () {
                        var fileReader = new FileReader();
                        fileReader.readAsDataURL(input.files[0]);
                        fileReader.onload = function (e) {
                            $timeout(function () {
                                $scope.fileAvatar.src = e.target.result;
                            });
                        }
                    });
                    $scope.fileAvatar.file = input.files[0];
                }
            }
            $scope.deleteAvatar = function () {
                $scope.fileAvatar = {};
                angular.element("#file-input-ava").val(null);
            }
            function saveMerchandise() {
                $scope.target.maSize = '';
                $scope.target.maColor = '';
                angular.forEach($scope.target.maSizes, function (size) {
                    $scope.target.maSize = $scope.target.maSize + size.value + ',';
                });
                angular.forEach($scope.target.maColors, function (co) {
                    $scope.target.maColor = $scope.target.maColor + co.value + ',';
                });
                //nếu có sự thay đổi mã so với mã ban đầu tự sinh thì bỏ qua hàm SaveCode
                if ($scope.target.maVatTu !== $scope.tempMaVatTu) {
                    $scope.target.useGenCode = false;
                }
                service.post(JSON.stringify($scope.target)).then(function (response) {
                    if (response.status && response.data.status) {
                        console.log('Create  Successfully!');
                        ngNotify.set('Thành công', { type: 'success' });
                        $uibModalInstance.close($scope.target);

                    } else {
                        ngNotify.set(response.data.message, { duration: 3000, type: 'error' });
                    }
                    //End fix
                });
            }
            function saveAvatar() {
                $scope.fileAvatar.maVatTu = $scope.target.maVatTu;
                upload.upload({
                    url: configService.rootUrlWebApi + '/Md/Merchandise/UploadAvatar',
                    data: $scope.fileAvatar
                }).then(function (response) {
                    if (response.status) {
                        $scope.target.avatarName = response.data.data;
                        saveMerchandise();
                    }
                    else {
                        toaster.pop('error', "Lỗi:", "Không lưu được ảnh! Có thể đã trùng!");
                    }
                });
            }
            function saveImage() {
                $scope.target.file = $scope.lstFile;
                upload.upload({
                    url: configService.rootUrlWebApi + '/Md/Merchandise/UploadMerchandiseImage',
                    data: $scope.target
                }).then(function (response) {
                    if (response.status) {
                        $scope.target.image = response.data.data;
                    }
                    else {
                        toaster.pop('error', "Lỗi:", "Không lưu được ảnh! Có thể đã trùng!");
                    }
                    if ($scope.fileAvatar) {
                        saveAvatar();
                    }
                    else {
                        saveMerchandise();
                    }
                });
            }
            $scope.save = function () {
                if ($scope.lstFile && $scope.lstFile.length) {
                    saveImage();
                }
                else {
                    if ($scope.fileAvatar) {
                        saveAvatar();
                    }
                    else {
                        saveMerchandise();
                    }
                }
            };
            $scope.cancel = function () {
                $uibModalInstance.close();
            };
        }]);
    /* controller Edit */
    app.controller('merchandiseChildEditController', ['$scope', '$uibModalInstance', 'configService', 'merchandiseService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify', 'taxService', 'Upload',
        function ($scope, $uibModalInstance, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify, serviceTax, upload) {
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.robot = service.robot;
            $scope.targetData = angular.copy(targetData);
            $scope.tempData = tempDataService.tempData;
            $scope.target = targetData;
            $scope.nhomVatTus = [];
            $scope.lstImagesSrcFromDb = [];
            $scope.lstImagesSrc = [];
            $scope.lstFile = [];
            $scope.fileAvatar = {};
            $scope.maSizes = [];
            $scope.maColors = [];

            $scope.isLoading = false;
            $scope.title = function () { return 'Cập nhập mã con Hàng hóa, Vật tư '; };
            $scope.flag = false;
            //$scope.tempData('status') = angular.copy($scope.tempData('status'));
            $scope.changeLoaiVatTu = function () {
                service.getSelectByMaLoai($scope.target.maLoaiVatTu).then(function (response) {
                    $scope.nhomVatTus = response;
                });
            }
            $scope.displayHepler = function (paraValue, moduleName) {
                var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                if (data && data.length === 1) {
                    return data[0].text;
                } else {
                    return paraValue;
                }
            };
            $scope.changeMaNhom = function () {
                $scope.flag = true;
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
            };
            function filterData() {
                service.getDetailByCode($scope.target.maVatTu).then(function (response) {
                    if (response && response.status == 200 && response.data) {
                        $scope.target = response.data;
                        if ($scope.target.image) {
                            var arrImage = $scope.target.image.split(',');
                            arrImage.splice(arrImage.length - 1, 1);
                            $scope.lstImagesSrcFromDb = arrImage;
                        }
                        $scope.target.maSizes = [];
                        if ($scope.target.maSize) {
                            var lst = $scope.target.maSize.split(',');
                            angular.forEach(lst, function (item) {
                                var tmp = $filter('filter')($scope.tempData('sizes'), { value: item }, true);
                                if (tmp && tmp.length) {
                                    $scope.target.maSizes.push(tmp[0]);
                                }
                            });
                        }
                        $scope.target.maColors = [];
                        if ($scope.target.maColor) {
                            var lst = $scope.target.maColor.split(',');
                            angular.forEach(lst, function (item) {
                                var tmp = $filter('filter')($scope.tempData('colors'), { value: item }, true);
                                if (tmp && tmp.length) {
                                    $scope.target.maColors.push(tmp[0]);
                                }
                            });
                        }
                        if ($scope.target.maBaoBi) {
                            service.getBaoBiByCode($scope.target.maBaoBi).then(function (response) {
                                if (response.status && response.data.status) {
                                    $scope.searchMaBaoBi = response.soLuong;
                                }
                            });
                        }
                        if ($scope.target.maLoaiVatTu) {
                            service.getSelectByMaLoai($scope.target.maLoaiVatTu).then(function (response) {
                                $scope.nhomVatTus = response;
                            });
                        }
                        $scope.pageChanged();
                    }

                });
            }
            filterData();
            $scope.createPackage = function (target, name) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Packaging', 'add'),
                    controller: 'packagingCreateController',
                    resolve: {}
                });
                modalInstance.result.then(function (updatedData) {
                    target.maBaoBi = updatedData.maBaoBi;
                    $scope.searchMaBaoBi = updatedData.soLuong;
                    var ob = {
                        description: updatedData.tenBaoBi,
                        extendValue: updatedData.soLuong,
                        value: updatedData.maBaoBi,
                        text: updatedData.maBaoBi + "|" + updatedData.tenBaoBi,
                    };
                    $scope.packagings.push(ob);
                    tempDataService.putTempData('packagings', $scope.packagings);
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };
            $scope.filterQuyCach = function (code) {
                if (code) {
                    var data = $filter('filter')($scope.tempData('packagings'), { value: code }, true);
                    if (data.length > 0) {
                        $scope.searchMaBaoBi = angular.copy(data[0].extendValue);
                    }
                }
            };
            $scope.createShelve = function (target, name) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Shelves', 'add'),
                    controller: 'shelvesCreateController',
                    resolve: {}
                });
                modalInstance.result.then(function (updatedData) {
                    $scope.tempData.update('shelves', function () {
                        if (target && name) {
                            target[name] = updatedData.maKeHang;
                        }
                    });
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };
            $scope.createSize = function (target, name) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Size', 'add'),
                    controller: 'sizeCreateController',
                    resolve: {}
                });
                modalInstance.result.then(function (updatedData) {
                    $scope.tempData.update('sizes', function () {
                        if (target && name) {
                            var data = $filter('filter')($scope.tempData('sizes'), { value: updatedData.maSize }, true);
                            if (data && data.length) {
                                target[name].push(data[0]);
                            }
                        }
                    });
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };
            $scope.createColor = function (target, name) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Color', 'add'),
                    controller: 'colorCreateController',
                    resolve: {}
                });
                modalInstance.result.then(function (updatedData) {
                    $scope.tempData.update('colors', function () {
                        if (target && name) {
                            var data = $filter('filter')($scope.tempData('colors'), { value: updatedData.maColor }, true);
                            if (data && data.length) {
                                target[name].push(data[0]);
                            }
                        }
                    });
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };
            $scope.changeVatVao = function (code) {
                if (code) {
                    serviceTax.getTaxByCode(code).then(function (response) {
                        if (response && response.status == 200 && response.data) {
                            $scope.target.tyLeVatVao = response.data.taxRate;
                            angular.forEach($scope.target.dataDetails, function (v, k) {
                                $scope.target.dataDetails[k].maVatVao = $scope.target.maVatVao;
                                $scope.target.dataDetails[k].tyLeVatVao = $scope.target.tyLeVatVao;
                                $scope.target.dataDetails[k].giaMuaVat = $scope.target.dataDetails[k].giaMua * $scope.target.dataDetails[k].tyLeVatVao / 100 + $scope.target.dataDetails[k].giaMua;
                            });
                        }
                    });
                } else {
                    $scope.target.tyLeVatVao = 0;
                    angular.forEach($scope.target.dataDetails, function (v, k) {
                        $scope.target.dataDetails[k].maVatVao = $scope.target.maVatVao;
                        $scope.target.dataDetails[k].tyLeVatVao = $scope.target.tyLeVatVao;
                        $scope.target.dataDetails[k].giaMuaVat = $scope.target.dataDetails[k].giaMua * $scope.target.dataDetails[k].tyLeVatVao / 100 + $scope.target.dataDetails[k].giaMua;
                    });
                }
            }
            $scope.changeVatRa = function (code) {
                if (code) {
                    serviceTax.getTaxByCode(code).then(function (response) {
                        if (response && response.status == 200 && response.data) {
                            $scope.target.tyLeVatRa = response.data.taxRate;
                            angular.forEach($scope.target.dataDetails, function (v, k) {
                                $scope.target.dataDetails[k].maVatRa = $scope.target.maVatRa;
                                $scope.target.dataDetails[k].tyLeVatRa = $scope.target.tyLeVatRa;
                                $scope.target.dataDetails[k].giaBanBuonVat = $scope.target.dataDetails[k].giaBanBuon * $scope.target.dataDetails[k].tyLeVatRa / 100 + $scope.target.dataDetails[k].giaBanBuon;
                                $scope.target.dataDetails[k].giaBanLeVat = $scope.target.dataDetails[k].giaBanLe * $scope.target.dataDetails[k].tyLeVatRa / 100 + $scope.target.dataDetails[k].giaBanLe;
                            });
                        }
                    });
                } else {
                    $scope.target.tyLeVatRa = 0;
                    angular.forEach($scope.target.dataDetails, function (v, k) {
                        $scope.target.dataDetails[k].maVatRa = $scope.target.maVatRa;
                        $scope.target.dataDetails[k].tyLeVatRa = $scope.target.tyLeVatRa;
                        $scope.target.dataDetails[k].giaBanBuonVat = $scope.target.dataDetails[k].giaBanBuon * $scope.target.dataDetails[k].tyLeVatRa / 100 + $scope.target.dataDetails[k].giaBanBuon;
                        $scope.target.dataDetails[k].giaBanLeVat = $scope.target.dataDetails[k].giaBanLe * $scope.target.dataDetails[k].tyLeVatRa / 100 + $scope.target.dataDetails[k].giaBanLe;
                    });
                }

            }
            $scope.addBarCode = function (target) {
                if (!target.barcode) {
                    target.barcode = ";";
                }
                target.barcode = target.barcode + target.newBarcode + ";";
                target.newBarcode = "";
            }

            $scope.update = function (target) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Merchandise', 'price-edit'),
                    controller: 'merchandisePriceEditController',
                    resolve: {
                        targetData: function () {
                            return target;
                        }
                    }
                });
                modalInstance.result.then(function (updatedData) {
                    var index = $scope.target.dataDetails.indexOf(target);
                    if (index !== -1) {
                        $scope.target.dataDetails[index] = updatedData;
                    }
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };
            $scope.removeItem = function (index) {
                var currentPage = $scope.paged.currentPage;
                var itemsPerPage = $scope.paged.itemsPerPage;
                var currentPageIndex = (currentPage - 1) * itemsPerPage + index;
                $scope.target.dataDetails.splice(currentPageIndex, 1);
                $scope.pageChanged();
            }


            $scope.create = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Merchandise', 'price'),
                    controller: 'merchandisePriceCreateController',
                    resolve: {
                        initData: {
                            maVatRa: $scope.target.maVatRa,
                            maVatVao: $scope.target.maVatVao,
                            tyLeVatRa: $scope.target.tyLeVatRa,
                            tyLeVatVao: $scope.target.tyLeVatVao
                        }
                    }
                });
                modalInstance.result.then(function (updatedData) {
                    $scope.target.dataDetails.push(updatedData);
                    $scope.pageChanged();
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };

            $scope.uploadFile = function (input) {
                if (input.files && input.files.length > 0) {
                    angular.forEach(input.files, function (file) {
                        $scope.lstFile.push(file);
                        $timeout(function () {
                            var fileReader = new FileReader();
                            fileReader.readAsDataURL(file);
                            fileReader.onload = function (e) {
                                $timeout(function () {
                                    $scope.lstImagesSrc.push(e.target.result);
                                });
                            }
                        });
                    });
                }
            };
            $scope.deleteImage = function (index) {
                $scope.lstImagesSrc.splice(index, 1);
                $scope.lstFile.splice(index, 1);
                if ($scope.lstFile.length < 1) {
                    angular.element("#file-input-upload").val(null);
                }
            };
            $scope.deleteImageFromDb = function (index) {
                $scope.lstImagesSrcFromDb.splice(index, 1);
            };
            function saveMerchandise() {
                $scope.target.maSize = '';
                $scope.target.maColor = '';
                angular.forEach($scope.target.maSizes, function (size) {
                    $scope.target.maSize = $scope.target.maSize + size.value + ',';
                });
                angular.forEach($scope.target.maColors, function (co) {
                    $scope.target.maColor = $scope.target.maColor + co.value + ',';
                });
                if ($scope.lstImagesSrcFromDb && $scope.lstImagesSrcFromDb.length > 0) {
                    angular.forEach($scope.lstImagesSrcFromDb, function (item) {
                        $scope.target.image = $scope.target.image + item + ",";
                    });

                }
                if ($scope.flag == true) {
                    service.updateCodeGroup(JSON.stringify($scope.target), function (response) {
                        if (response.status) {
                            ngNotify.set('Thành công', { type: 'success' });
                            $uibModalInstance.close($scope.target);
                        } else {
                            ngNotify.set(response.data.message, { duration: 3000, type: 'error' });
                        }
                    });
                }
                else {
                    service.update($scope.target).then(
                        function (response) {
                            if (response.status && response.status == 200) {
                                if (response.data.status) {
                                    ngNotify.set('Thành công', { type: 'success' });
                                    $uibModalInstance.close($scope.target);
                                } else {
                                    ngNotify.set(response.data.message, { duration: 3000, type: 'error' });
                                }
                            } else {
                                console.log('ERROR: Update failed! ' + response.errorMessage);
                                ngNotify.set(response.errorMessage, { duration: 3000, type: 'error' });
                            }
                        },
                        function (response) {
                            ngNotify.set('ERROR: Update failed!', { duration: 3000, type: 'error' });
                        });
                }
            }
            $scope.uploadAvatar = function (input) {
                $scope.inputAvatar = input;
                if (input.files && input.files.length > 0) {
                    $timeout(function () {
                        var fileReader = new FileReader();
                        fileReader.readAsDataURL(input.files[0]);
                        fileReader.onload = function (e) {
                            $timeout(function () {
                                $scope.fileAvatar.src = e.target.result;
                            });
                        }
                    });
                    $scope.fileAvatar.file = input.files[0];
                }
            }
            $scope.deleteAvatar = function () {
                if ($scope.target.avatar) {
                    $scope.target.avatar = null;
                }
                if ($scope.fileAvatar) {
                    $scope.fileAvatar = {};
                    angular.element("#file-input-ava").val(null);
                }
            }
            function saveAvatar() {
                $scope.fileAvatar.maVatTu = $scope.target.maVatTu;
                upload.upload({
                    url: configService.rootUrlWebApi + '/Md/Merchandise/UploadAvatar',
                    data: $scope.fileAvatar
                }).then(function (response) {
                    if (response.status) {
                        $scope.target.avatarName = response.data.data;
                        saveMerchandise();
                    }
                    else {
                        ngNotify.set('Không lưu được ảnh! Có thể đã trùng!', { duration: 3000, type: 'error' });
                    }
                });
            }
            function saveImage() {
                $scope.target.file = $scope.lstFile;
                upload.upload({
                    url: configService.rootUrlWebApi + '/Md/Merchandise/UploadMerchandiseImage',
                    data: $scope.target
                }).then(function (response) {
                    if (response.status) {
                        $scope.target.image = response.data.data;
                    }
                    else {
                        ngNotify.set('Không lưu được ảnh! Có thể đã trùng!', { duration: 3000, type: 'error' });
                    }
                    if ($scope.fileAvatar) {
                        saveAvatar();
                    }
                    else {
                        saveMerchandise();
                    }
                });
            }
            $scope.save = function () {
                $scope.target.image = '';
                if ($scope.lstFile && $scope.lstFile.length) {
                    saveImage();
                }
                else {
                    if ($scope.fileAvatar) {
                        saveAvatar();
                    }
                    else {
                        saveMerchandise();
                    }
                }
            };

            $scope.cancel = function () {
                $uibModalInstance.close();
            };
        }]);

    /* controller merchandise Child  Details Controller */
    app.controller('merchandiseChildDetailsController', ['$scope', '$uibModalInstance', 'configService', 'merchandiseService', 'tempDataService', '$filter', 'targetData',
        function ($scope, $uibModalInstance, configService, service, tempDataService, $filter, targetData) {
            $scope.config = angular.copy(configService);
            $scope.robot = service.robot;
            $scope.targetData = angular.copy(targetData);
            $scope.tempData = tempDataService.tempData;
            $scope.target = targetData;
            $scope.isLoading = false;
            $scope.lstImagesSrcFromDb = [];
            $scope.title = function () { return 'Chi tiết Hàng hóa, Vật tư'; };
            $scope.nhomVatTus = [];
            $scope.displayHepler = function (paraValue, moduleName) {
                var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                if (data && data.length === 1) {
                    return data[0].text;
                } else {
                    return paraValue;
                }
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
            function filterData() {
                service.getDetailByCode($scope.target.maVatTu).then(function (response) {
                    if (response && response.status == 200 && response.data) {
                        $scope.target = response.data;
                        if ($scope.target.image) {
                            var arrImage = $scope.target.image.split(',');
                            arrImage.splice(arrImage.length - 1, 1);
                            $scope.lstImagesSrcFromDb = arrImage;
                        }
                        $scope.target.maSizes = [];
                        if ($scope.target.maSize) {
                            var lst = $scope.target.maSize.split(',');
                            angular.forEach(lst, function (item) {
                                var tmp = $filter('filter')(mdService.tempData.sizes, { value: item }, true);
                                if (tmp && tmp.length) {
                                    $scope.target.maSizes.push(tmp[0]);
                                }
                            });
                        }
                        $scope.target.maColors = [];
                        if ($scope.target.maColor) {
                            var lst = $scope.target.maColor.split(',');
                            angular.forEach(lst, function (item) {
                                var tmp = $filter('filter')(mdService.tempData.colors, { value: item }, true);
                                if (tmp && tmp.length) {
                                    $scope.target.maColors.push(tmp[0]);
                                }
                            });
                        }
                        if ($scope.target.maBaoBi) {
                            service.getBaoBiByCode($scope.target.maBaoBi).then(function (response) {
                                if (response.status && response.data.status) {
                                    $scope.searchMaBaoBi = response.soLuong;
                                }
                            });
                        }
                        if ($scope.target.maLoaiVatTu) {
                            service.getSelectByMaLoai($scope.target.maLoaiVatTu).then(function (response) {
                                $scope.nhomVatTus = response;
                            });
                        }
                        $scope.pageChanged();
                    }

                });
            }
            filterData();
            $scope.cancel = function () {
                $uibModalInstance.close();
            };
        }]);

    /* controller merchandise Price Create Controller */
    app.controller('merchandisePriceCreateController', ['$scope', '$uibModalInstance', 'configService', 'merchandiseService', 'tempDataService', '$filter', 'userService', 'AuDonViService', 'initData',
        function ($scope, $uibModalInstance, configService, service, tempDataService, $filter, serviceAuthUser, serviceAuthDonVi, initData) {
            var currentUser = serviceAuthUser.GetCurrentUser();
            $scope.config = angular.copy(configService);
            $scope.robot = service.robot;
            $scope.tempData = tempDataService.tempData;
            $scope.target = {
                giaMua: 0,
                tyLeLaiLe: 0,
                giaBanVat: 0,
                tyLeLaiBuon: 0,
                giaBanBuon: 0,
                giaBanLeVat: 0,
                giaBanBuonVat: 0,
                maDonVi: currentUser.unitCode
            };
            $scope.displayHepler = function (paraValue, moduleName) {
                var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                if (data && data.length === 1) {
                    return data[0].text;
                } else {
                    return paraValue;
                }
            };
            $scope.target = angular.extend($scope.target, initData);
            $scope.tempData = tempDataService.tempData;
            $scope.isGeneralUnit = true;
            //load danh mục
            function loadAuthDonVi() {
                if (!tempDataService.tempData('auDonVis')) {
                    serviceAuthDonVi.getAll_DonVi().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.length > 0) {
                            tempDataService.putTempData('auDonVis', successRes.data);
                            $scope.auDonVis = successRes.data;
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.auDonVis = tempDataService.tempData('auDonVis');
                }
            }
            loadAuthDonVi();
            //end
            $scope.title = function () { return 'Tạo giá cả hàng hóa, vật tư'; };
            $scope.save = function () {
                $uibModalInstance.close($scope.target);
            };
            $scope.cancel = function () {
                $uibModalInstance.close();
            };
        }]);


    /* controller merchandise Price Edit Controller */
    app.controller('merchandisePriceEditController', ['$scope', '$uibModalInstance', 'configService', 'merchandiseService', 'tempDataService', 'targetData', 'userService', 'taxService', 'AuDonViService',
        function ($scope, $uibModalInstance, configService, service, tempDataService, targetData, serviceAuthUser, serviceTax, serviceAuthDonVi) {
            var currentUser = serviceAuthUser.GetCurrentUser();
            $scope.config = angular.copy(configService);
            $scope.robot = service.robot;
            $scope.tempData = tempDataService.tempData;
            $scope.target = angular.copy(targetData);
            function loadTax() {
                if (!tempDataService.tempData('taxs')) {
                    serviceTax.getAll_Tax().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                            tempDataService.putTempData('taxs', successRes.data.data);
                            $scope.taxs = successRes.data.data;
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.taxs = tempDataService.tempData('taxs');
                }
            }

            function loadAuthDonVi() {
                if (!tempDataService.tempData('auDonVis')) {
                    serviceAuthDonVi.getAll_DonVi().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.length > 0) {
                            tempDataService.putTempData('auDonVis', successRes.data);
                            $scope.auDonVis = successRes.data;
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.auDonVis = tempDataService.tempData('auDonVis');
                }
            }

            loadAuthDonVi();
            loadTax();

            $scope.title = function () { return 'Cập nhật giá hàng hóa, vật tư'; };
            $scope.isGeneralUnit = true;
            $scope.save = function () {
                $uibModalInstance.close($scope.target);
            };
            $scope.cancel = function () {
                $uibModalInstance.close();
            };
        }]);


    /* controller merchandise Price Directory Edit Controller */
    app.controller('merchandisePriceDirectoryEditController', ['$scope', '$uibModalInstance', 'configService', 'merchandiseService', 'tempDataService', '$filter', 'targetData', 'ngNotify', 'userService', 'taxService', 'securityService',
        function ($scope, $uibModalInstance, configService, service, tempDataService, $filter, targetData, ngNotify, serviceAuthUser, serviceTax, securityService) {
            var currentUser = serviceAuthUser.GetCurrentUser();
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.robot = service.robot;
            $scope.tempData = tempDataService.tempData;
            $scope.target = targetData.data;
            $scope.invalidLB = false;
            $scope.invalidLL = false;
            $scope.target.maDonVi = currentUser.unitCode;
            $scope.title = function () { return 'Cập nhật giá hàng hóa vật tư'; };
            $scope.isGeneralUnit = true;
            $scope.accessList = {};
            //check quyền truy cập
            function loadAccessList() {
                securityService.getAccessList('merchandise').then(function (successRes) {
                    if (successRes && successRes.status === 200) {
                        $scope.accessList = successRes.data;
                    } else {
                        toaster.pop('error', "Lỗi:", "Không có quyền truy cập !");
                    }
                }, function (errorRes) {
                    toaster.pop('error', "Lỗi:", "Không có quyền truy cập !");
                    $scope.accessList = null;
                });
            }
            loadAccessList();
            //end
            $scope.displayHepler = function (paraValue, moduleName) {
                var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                if (data && data.length === 1) {
                    return data[0].text;
                } else {
                    return paraValue;
                }
            }
            function loadTax() {
                if (!tempDataService.tempData('taxs')) {
                    serviceTax.getAll_Tax().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                            tempDataService.putTempData('taxs', successRes.data.data);
                            $scope.taxs = successRes.data.data;
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.taxs = tempDataService.tempData('taxs');
                }
            }
            loadTax();
            $scope.checkTyLeLai = function (item) {
                if ($scope.target.tyLeLaiLe >= 100 || $scope.target.tyLeLaiLe < 0) {
                    $scope.invalidLL = true;
                } else {
                    $scope.invalidLL = false;
                }
                if ($scope.tyLeLaiBuon >= 100 || $scope.tyLeLaiBuon < 0) {
                    $scope.invalidLB = true;
                }
                else {
                    $scope.invalidLB = false;
                }
            }
            $scope.save = function () {
                service.updatePrice($scope.target).then(function (response) {
                    if (response.status && response.status == 200) {
                        if (response.data.status) {
                            console.log('Create  Successfully!');
                            ngNotify.set('Cập nhật thành công', { type: 'success' });
                            $uibModalInstance.close($scope.target);
                        }
                    } else {
                        ngNotify.set(response.data.message, { duration: 3000, type: 'error' });
                    }
                });
            };
            $scope.cancel = function () {
                $uibModalInstance.close();
            };
        }]);

    //controller đồng bộ
    /* matHang Controller */
    app.controller('matHangController', ['$scope', 'configService', 'merchandiseService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'securityService', 'toaster', '$uibModalInstance', 'taxService', 'supplierService', 'merchandiseTypeService', 'nhomVatTuService', 'packagingService', 'shelvesService', 'donViTinhService',
        function ($scope, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, securityService, toaster, $uibModalInstance, serviceTax, serviceSupplier, serviceMerchandiseType, serviceNhomVatTu, servicePackaging, serviceShelves, serviceDonViTinh) {
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.filtered = angular.copy(configService.filterDefault);
            $scope.tempData = tempDataService.tempData;
            $scope.isEditable = true;
            $scope.accessList = {};
            $scope.target = {
                options: 'barcode',
                listData: []
            };
            $scope.categories = [{
                value: 'barcode',
                text: 'Barcode'
            },
            {
                value: 'maVatTu',
                text: 'Mã hàng'
            },
            {
                value: 'tenHang',
                text: 'Tên hàng'
            },
            {
                value: 'maKhachHang',
                text: 'Mã NCC'
            },
            {
                value: 'giaBanLeVat',
                text: 'Giá bán lẻ VAT'
            },
            {
                value: 'giaMuaVat',
                text: 'Giá mua VAT'
            },
            {
                value: 'tyLeLaiLe',
                text: 'Tỷ lệ lãi lẻ'
            }];
            $scope.filtered.advanceData.tieuChiTimKiem = $scope.target.options;

            function loadTaxRoot() {
                if (!tempDataService.tempData('taxsRoot')) {
                    serviceTax.getAll_TaxRoot().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data && successRes.data.data.length > 0) {
                            tempDataService.putTempData('taxsRoot', successRes.data.data);
                            $scope.taxsRoot = successRes.data.data;
                        } else {

                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.taxsRoot = tempDataService.tempData('taxsRoot');
                }
            };
            function loadSupplierRoot() {
                if (!tempDataService.tempData('suppliersRoot')) {
                    serviceSupplier.getAll_SupplierRoot().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data && successRes.data.data.length > 0) {
                            tempDataService.putTempData('suppliersRoot', successRes.data.data);
                            $scope.suppliersRoot = successRes.data.data;
                        } else {

                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.suppliersRoot = tempDataService.tempData('suppliersRoot');
                }
            };

            function loadMerchandiseTypeRoot() {
                if (!tempDataService.tempData('merchandiseTypesRoot')) {
                    serviceMerchandiseType.getAll_MerchandiseTypeRoot().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data && successRes.data.data.length > 0) {
                            tempDataService.putTempData('merchandiseTypesRoot', successRes.data.data);
                            $scope.merchandiseTypesRoot = successRes.data.data;
                        } else {

                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.merchandiseTypesRoot = tempDataService.tempData('merchandiseTypesRoot');
                }
            };

            function loadNhomVatTusRoot() {
                if (!tempDataService.tempData('nhomVatTusRoot')) {
                    serviceNhomVatTu.getAll_NhomVatTuRoot().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data && successRes.data.data.length > 0) {
                            tempDataService.putTempData('nhomVatTusRoot', successRes.data.data);
                            $scope.nhomVatTusRoot = successRes.data.data;
                        } else {

                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.nhomVatTusRoot = tempDataService.tempData('nhomVatTusRoot');
                }
            };

            function loadPackagingsRoot() {
                if (!tempDataService.tempData('packagingsRoot')) {
                    servicePackaging.getAll_PackagingRoot().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data && successRes.data.data.length > 0) {
                            tempDataService.putTempData('packagingsRoot', successRes.data.data);
                            $scope.packagingsRoot = successRes.data.data;
                        } else {

                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.packagingsRoot = tempDataService.tempData('packagingsRoot');
                }
            };

            function loadShelvesRoot() {
                if (!tempDataService.tempData('shelvesRoot')) {
                    serviceShelves.getAll_ShelvesRoot().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data && successRes.data.data.length > 0) {
                            tempDataService.putTempData('shelvesRoot', successRes.data.data);
                            $scope.shelvesRoot = successRes.data.data;
                        } else {

                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.shelvesRoot = tempDataService.tempData('shelvesRoot');
                }
            };
            function loadDonViTinhRoot() {
                if (!tempDataService.tempData('donViTinhsRoot')) {
                    serviceDonViTinh.getAll_DonViTinhRoot().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data && successRes.data.data.length > 0) {
                            tempDataService.putTempData('donViTinhsRoot', successRes.data.data);
                            $scope.donViTinhsRoot = successRes.data.data;
                        } else {

                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.donViTinhsRoot = tempDataService.tempData('donViTinhsRoot');
                }
            }

            loadTaxRoot();
            loadSupplierRoot();
            loadMerchandiseTypeRoot();
            loadNhomVatTusRoot();
            loadPackagingsRoot();
            loadShelvesRoot();
            loadDonViTinhRoot();


            $scope.displayHepler = function (paraValue, moduleName) {
                var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                if (data && data.length === 1) {
                    return data[0].text;
                } else {
                    return paraValue;
                }
            };
            $scope.getNameVAT = function (paraValue, moduleName) {
                var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                if (data && data.length === 1) {
                    return data[0].description;
                } else {
                    return paraValue;
                }
            }

            //load dữ liệu
            function filterAdvanceData() {
                $scope.filtered.advanceData = {};
                if ($scope.target.options) {
                    $scope.filtered.isAdvance = true;
                    $scope.filtered.advanceData[$scope.target.options] = $scope.summary;
                    $scope.filtered.advanceData.tieuChiTimKiem = $scope.target.options;
                }
                $scope.isLoading = true;
                var postdata = { paged: $scope.paged, filtered: $scope.filtered };
                service.postSelectDataServerRoot(postdata).then(function (successRes) {
                    if (successRes && successRes.status === 200 && successRes.data.status && successRes.data.data) {
                        $scope.isLoading = false;
                        $scope.target.listData = successRes.data.data;
                    }
                    else {
                        $scope.isLoading = false;
                        $scope.target.listData = [];
                    }
                    $scope.pageChanged();
                });
            };

            $scope.pageChanged = function () {
                var currentPage = $scope.paged.currentPage;
                var itemsPerPage = $scope.paged.itemsPerPage;
                $scope.paged.totalItems = $scope.target.listData.length;
                $scope.data = [];
                if ($scope.target.listData) {
                    for (var i = (currentPage - 1) * itemsPerPage;
                        i < currentPage * itemsPerPage && i < $scope.target.listData.length;
                        i++) {
                        $scope.data.push($scope.target.listData[i]);
                    }
                }
            };
            function filterData() {
                $scope.isLoading = true;
                if ($scope.accessList.view) {
                    var postdata = { paged: $scope.paged, filtered: $scope.filtered };
                    service.postSelectDataServerRoot(postdata).then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.status && successRes.data.data) {
                            $scope.isLoading = false;
                            $scope.target.listData = successRes.data.data;
                            $scope.pageChanged();
                        }
                    }, function (errorRes) {
                        console.log(errorRes);
                    });
                }
            };
            //end
            //check quyền truy cập
            function loadAccessList() {
                securityService.getAccessList('merchandise').then(function (successRes) {
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

            $scope.setPage = function (pageNo) {
                $scope.paged.currentPage = pageNo;
                filterAdvanceData();
            };
            $scope.sortType = 'maVatTu';
            $scope.sortReverse = false;
            $scope.doSearch = function () {
                $scope.paged.currentPage = 1;
                filterData();
            };
            $scope.doSearchStr = function () {
                $scope.filtered.isAdvance = true;
                $scope.paged.currentPage = 1;
                filterAdvanceData();
            }

            $scope.goHome = function () {
                window.location.href = "#!/home";
            };
            $scope.refresh = function () {
                filterAdvanceData();
            };
            $scope.title = function () { return 'Danh sách mặt hàng'; };

            $scope.changeAvartar = function (event) {
                if (event.target.checked) {
                    $scope.displayAvatar = true;
                } else {
                    $scope.displayAvatar = false;
                }
            }
            /* Function add New Item */
            $scope.create = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    size: 'md',
                    templateUrl: configService.buildUrl('htdm/Merchandise', 'async-add'),
                    controller: 'matHangCreateController',
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
                    templateUrl: configService.buildUrl('htdm/Merchandise', 'async-edit'),
                    controller: 'matHangEditController',
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
                    templateUrl: configService.buildUrl('htdm/Merchandise', 'async-details'),
                    controller: 'matHangDetailsController',
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
            $scope.startSync = function (item) {
                if ($scope.asyncing) {
                    return;
                }
                $scope.asyncing = true;
                service.postAsyncFromOracleRoot(item).then(function (response) {
                    if (response && response.status === 200 && response.data && response.data.status) {
                        ngNotify.set(response.data.message, { type: 'success' });
                    } else {
                        ngNotify.set(response.data.message, { duration: 3000, type: 'error' });
                        if (!response.data.data && response.data.message === 'Đã tồn tại mặt hàng này tại hệ thống') {
                            var modalInstance = $uibModal.open({
                                backdrop: 'static',
                                windowClass: 'app-modal-window',
                                templateUrl: configService.buildUrl('htdm/Merchandise', 'async-compare'),
                                controller: 'asyncCompareMatHangController',
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
                        }
                    }
                    $scope.asyncing = false;
                    filterData();
                });
            };
            $scope.cancel = function () {
                $uibModalInstance.close();
            };
        }]);

    /** async Compare MatHang Controller -- Nguyễn Tuấn Hoàng Anh, so sánh đồng bộ */
    app.controller('asyncCompareMatHangController', ['$scope', '$uibModalInstance', 'configService', 'merchandiseService', 'tempDataService', '$filter', 'ngNotify', 'targetData', 'taxService', 'supplierService', 'merchandiseTypeService', 'nhomVatTuService', 'packagingService', 'shelvesService', 'donViTinhService', 'userService',
        function ($scope, $uibModalInstance, configService, service, tempDataService, $filter, ngNotify, targetData, serviceTax, serviceSupplier, serviceMerchandiseType, serviceNhomVatTu, servicePackaging, serviceShelves, serviceDonViTinh, serviceAuthUser) {
            var currentUser = serviceAuthUser.GetCurrentUser();
            $scope.unitCode = currentUser.unitCode;
            $scope.config = angular.copy(configService);
            $scope.robot = service.robot;
            $scope.nhomVatTus = [];
            $scope.loaiVatTus = [];
            $scope.khachHangs = [];
            $scope.keHangs = [];
            $scope.taxs = [];
            $scope.target = {};
            $scope.targetClient = {};
            $scope.title = function () { return 'So sánh mã Hàng hóa, Vật tư'; };
            $scope.targetData = angular.copy(targetData);
            $scope.tempData = tempDataService.tempData;
            function loadTaxRoot() {
                if (!tempDataService.tempData('taxsRoot')) {
                    serviceTax.getAll_TaxRoot().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data && successRes.data.data.length > 0) {
                            tempDataService.putTempData('taxsRoot', successRes.data.data);
                            $scope.taxsRoot = successRes.data.data;
                        } else {

                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.taxsRoot = tempDataService.tempData('taxsRoot');
                }
            };
            function loadSupplierRoot() {
                if (!tempDataService.tempData('suppliersRoot')) {
                    serviceSupplier.getAll_SupplierRoot().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data && successRes.data.data.length > 0) {
                            tempDataService.putTempData('suppliersRoot', successRes.data.data);
                            $scope.suppliersRoot = successRes.data.data;
                        } else {

                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.suppliersRoot = tempDataService.tempData('suppliersRoot');
                }
            };

            function loadMerchandiseTypeRoot() {
                if (!tempDataService.tempData('merchandiseTypesRoot')) {
                    serviceMerchandiseType.getAll_MerchandiseTypeRoot().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data && successRes.data.data.length > 0) {
                            tempDataService.putTempData('merchandiseTypesRoot', successRes.data.data);
                            $scope.merchandiseTypesRoot = successRes.data.data;
                        } else {

                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.merchandiseTypesRoot = tempDataService.tempData('merchandiseTypesRoot');
                }
            };

            function loadNhomVatTusRoot() {
                if (!tempDataService.tempData('nhomVatTusRoot')) {
                    serviceNhomVatTu.getAll_NhomVatTuRoot().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data && successRes.data.data.length > 0) {
                            tempDataService.putTempData('nhomVatTusRoot', successRes.data.data);
                            $scope.nhomVatTusRoot = successRes.data.data;
                        } else {

                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.nhomVatTusRoot = tempDataService.tempData('nhomVatTusRoot');
                }
            };

            function loadPackagingsRoot() {
                if (!tempDataService.tempData('packagingsRoot')) {
                    servicePackaging.getAll_PackagingRoot().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data && successRes.data.data.length > 0) {
                            tempDataService.putTempData('packagingsRoot', successRes.data.data);
                            $scope.packagingsRoot = successRes.data.data;
                        } else {

                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.packagingsRoot = tempDataService.tempData('packagingsRoot');
                }
            };

            function loadShelvesRoot() {
                if (!tempDataService.tempData('shelvesRoot')) {
                    serviceShelves.getAll_ShelvesRoot().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data && successRes.data.data.length > 0) {
                            tempDataService.putTempData('shelvesRoot', successRes.data.data);
                            $scope.shelvesRoot = successRes.data.data;
                        } else {

                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.shelvesRoot = tempDataService.tempData('shelvesRoot');
                }
            };
            function loadDonViTinhRoot() {
                if (!tempDataService.tempData('donViTinhsRoot')) {
                    serviceDonViTinh.getAll_DonViTinhRoot().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data && successRes.data.data.length > 0) {
                            tempDataService.putTempData('donViTinhsRoot', successRes.data.data);
                            $scope.donViTinhsRoot = successRes.data.data;
                        } else {

                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.donViTinhsRoot = tempDataService.tempData('donViTinhsRoot');
                }
            };

            loadTaxRoot();
            loadSupplierRoot();
            loadMerchandiseTypeRoot();
            loadNhomVatTusRoot();
            loadPackagingsRoot();
            loadShelvesRoot();
            loadDonViTinhRoot();


            //load danh mục

            function loadSupplier() {
                if (!tempDataService.tempData('suppliers')) {
                    serviceSupplier.getAll_Supplier().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                            tempDataService.putTempData('suppliers', successRes.data.data);
                            $scope.suppliers = successRes.data.data;
                        } else {
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

                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.nhomVatTus = tempDataService.tempData('nhomVatTus');
                }
            }
            function loadDonViTinh() {
                if (!tempDataService.tempData('donViTinhs')) {
                    serviceDonViTinh.getAll_DonViTinh().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                            tempDataService.putTempData('donViTinhs', successRes.data.data);
                            $scope.donViTinhs = successRes.data.data;
                        } else {

                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.donViTinhs = tempDataService.tempData('donViTinhs');
                }
            }

            function loadeShelves() {
                if (!tempDataService.tempData('shelves')) {
                    serviceShelves.getAll_Shelves().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                            tempDataService.putTempData('shelves', successRes.data.data);
                            $scope.shelves = successRes.data.data;
                        } else {

                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.shelves = tempDataService.tempData('shelves');
                }
            };

            function loadRootUnitCode() {
                service.getRootUnitCode().then(function (response) {
                    if (response && response.status == 200 && response.data && response.data.status && response.data.data)
                        $scope.rootUnitCode = response.data.data;
                });
            };

            loadSupplier();
            loadMerchandiseType();
            loadNhomVatTu();
            loadDonViTinh();
            loadeShelves();
            loadRootUnitCode();

            $scope.displayHepler = function (paraValue, moduleName) {
                var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                if (data && data.length === 1) {
                    return data[0].text;
                } else {
                    return paraValue;
                }
            };

            $scope.displaySieuThi = function (paraValue, moduleName) {
                var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                if (data && data.length === 1) {
                    return data[0].text + ' (' + data[0].value + ')';
                } else {
                    return paraValue;
                }
            };
            function filterData() {
                if ($scope.targetData.maVatTu) {
                    //lấy mã đơn vị tạo mã
                    service.getDetailByCodeRoot($scope.targetData.maVatTu).then(function (response) {
                        if (response && response.status == 200 && response.data && response.data.data) {
                            $scope.target = response.data.data;
                        }
                    });
                    //lấy mã đơn vị hiện tại
                    service.getDetailByCode($scope.targetData.maVatTu).then(function (response) {
                        if (response && response.status == 200 && response.data) {
                            $scope.targetClient = response.data;
                        }
                    });
                }
                else {
                    ngNotify.set("Không tìm thấy chi tiết hàng hóa", { duration: 3000, type: 'error' });
                }
            }
            filterData();
            $scope.asyncCompare = function () {
                if ($scope.asyncing) {
                    return;
                }
                $scope.asyncing = true;
                service.postAsyncCompareUpdate(targetData).then(function (response) {
                    if (response && response.status === 200 && response.data && response.data.status) {
                        ngNotify.set(response.data.message, { type: 'success' });
                        $uibModalInstance.close($scope.target);
                    } else {
                        ngNotify.set(response.data.message, { duration: 3000, type: 'error' });
                    }
                    $scope.asyncing = false;
                });
            };
            $scope.goHome = function () {
                $uibModalInstance.close();
            };
            $scope.cancel = function () {
                $uibModalInstance.close();
            };
        }]);

    /** matHang Create Controller */
    app.controller('matHangCreateController', ['$scope', '$uibModalInstance', 'configService', 'merchandiseService', 'tempDataService', '$filter', 'ngNotify', 'taxService', 'supplierService', 'merchandiseTypeService', 'nhomVatTuService', 'packagingService', 'shelvesService', 'donViTinhService',
        function ($scope, $uibModalInstance, configService, service, tempDataService, $filter, ngNotify, serviceTax, serviceSupplier, serviceMerchandiseType, serviceNhomVatTu, servicePackaging, serviceShelves, serviceDonViTinh) {
            $scope.config = angular.copy(configService);
            $scope.robot = service.robot;
            $scope.currentUser = {};
            $scope.lstFile = [];
            $scope.nhomVatTus = [];
            $scope.loaiVatTus = [];
            $scope.khachHangs = [];
            $scope.keHangs = [];
            $scope.taxs = [];
            $scope.target = { dataDetails: [] };
            $scope.title = function () { return 'Tạo đồng bộ Hàng hóa, Vật tư'; };
            $scope.isGeneralUnit = true;
            $scope.tempData = tempDataService.tempData;
            $scope.displayHepler = function (paraValue, moduleName) {
                var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                if (data && data.length === 1) {
                    return data[0].text;
                } else {
                    return paraValue;
                }
            };
            $scope.displayHeplerVat = function (paraValue, moduleName) {
                var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                if (data && data.length === 1) {
                    return data[0].value + '|' + data[0].text;
                } else {
                    return paraValue;
                }
            };
            function loadTaxRoot() {
                if (!tempDataService.tempData('taxsRoot')) {
                    serviceTax.getAll_TaxRoot().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data && successRes.data.data.length > 0) {
                            tempDataService.putTempData('taxsRoot', successRes.data.data);
                            $scope.taxsRoot = successRes.data.data;
                        } else {

                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.taxsRoot = tempDataService.tempData('taxsRoot');
                }
            };
            function loadSupplierRoot() {
                if (!tempDataService.tempData('suppliersRoot')) {
                    serviceSupplier.getAll_SupplierRoot().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data && successRes.data.data.length > 0) {
                            tempDataService.putTempData('suppliersRoot', successRes.data.data);
                            $scope.suppliersRoot = successRes.data.data;
                        } else {

                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.suppliersRoot = tempDataService.tempData('suppliersRoot');
                }
            };
            $scope.changedMerchancedise = function (tenVatTu) {
                $scope.target.tenVietTat = tenVatTu;
            };
            function loadMerchandiseTypeRoot() {
                if (!tempDataService.tempData('merchandiseTypesRoot')) {
                    serviceMerchandiseType.getAll_MerchandiseTypeRoot().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data && successRes.data.data.length > 0) {
                            tempDataService.putTempData('merchandiseTypesRoot', successRes.data.data);
                            $scope.merchandiseTypesRoot = successRes.data.data;
                        } else {

                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.merchandiseTypesRoot = tempDataService.tempData('merchandiseTypesRoot');
                }
            };

            function loadNhomVatTusRoot() {
                if (!tempDataService.tempData('nhomVatTusRoot')) {
                    serviceNhomVatTu.getAll_NhomVatTuRoot().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data && successRes.data.data.length > 0) {
                            tempDataService.putTempData('nhomVatTusRoot', successRes.data.data);
                            $scope.nhomVatTusRoot = successRes.data.data;
                        } else {

                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.nhomVatTusRoot = tempDataService.tempData('nhomVatTusRoot');
                }
            };

            function loadPackagingsRoot() {
                if (!tempDataService.tempData('packagingsRoot')) {
                    servicePackaging.getAll_PackagingRoot().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data && successRes.data.data.length > 0) {
                            tempDataService.putTempData('packagingsRoot', successRes.data.data);
                            $scope.packagingsRoot = successRes.data.data;
                        } else {

                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.packagingsRoot = tempDataService.tempData('packagingsRoot');
                }
            };

            function loadShelvesRoot() {
                if (!tempDataService.tempData('shelvesRoot')) {
                    serviceShelves.getAll_ShelvesRoot().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data && successRes.data.data.length > 0) {
                            tempDataService.putTempData('shelvesRoot', successRes.data.data);
                            $scope.shelvesRoot = successRes.data.data;
                        } else {

                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.shelvesRoot = tempDataService.tempData('shelvesRoot');
                }
            };
            function loadDonViTinhRoot() {
                if (!tempDataService.tempData('donViTinhsRoot')) {
                    serviceDonViTinh.getAll_DonViTinhRoot().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data && successRes.data.data.length > 0) {
                            tempDataService.putTempData('donViTinhsRoot', successRes.data.data);
                            $scope.donViTinhsRoot = successRes.data.data;
                        } else {

                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.donViTinhsRoot = tempDataService.tempData('donViTinhsRoot');
                }
            }

            loadTaxRoot();
            loadSupplierRoot();
            loadMerchandiseTypeRoot();
            loadNhomVatTusRoot();
            loadPackagingsRoot();
            loadShelvesRoot();
            loadDonViTinhRoot();
            //log2
            $scope.changeLoaiVatTu = function () {
                service.getSelectByMaLoaiRoot($scope.target.maLoaiVatTu).then(function (response) {
                    if (response && response.status == 200 && response.data) {
                        $scope.nhomVatTus = response.data;
                    }
                });
                service.getNewCodeRoot($scope.target.maLoaiVatTu).then(function (response) {
                    if (response && response.status == 200 && response.data) {
                        $scope.target.maVatTu = response.data;
                        $scope.maVatTu = response.data;
                        $scope.tempMaVatTu = angular.copy($scope.target.maVatTu);
                    }
                });
            };
            $scope.addBarCode = function (target) {
                if (!target.barcode) {
                    target.barcode = ";";
                }
                target.barcode = target.barcode + target.newBarcode + ";";
                target.newBarcode = "";
            };
            $scope.changeVatVao = function (code) {
                if (code) {
                    serviceTax.getTaxByCode(code).then(function (response) {
                        if (response && response.status == 200 && response.data) {
                            $scope.target.tyLeVatVao = response.data.taxRate;
                            angular.forEach($scope.target.dataDetails, function (v, k) {
                                $scope.target.dataDetails[k].maVatVao = $scope.target.maVatVao;
                                $scope.target.dataDetails[k].tyLeVatVao = $scope.target.tyLeVatVao;
                                $scope.target.dataDetails[k].giaMuaVat = $scope.target.dataDetails[k].giaMua * $scope.target.dataDetails[k].tyLeVatVao / 100 + $scope.target.dataDetails[k].giaMua;
                            });
                        }
                    });
                } else {
                    $scope.target.tyLeVatVao = 0;
                    angular.forEach($scope.target.dataDetails, function (v, k) {
                        $scope.target.dataDetails[k].maVatVao = $scope.target.maVatVao;
                        $scope.target.dataDetails[k].tyLeVatVao = $scope.target.tyLeVatVao;
                        $scope.target.dataDetails[k].giaMuaVat = $scope.target.dataDetails[k].giaMua * $scope.target.dataDetails[k].tyLeVatVao / 100 + $scope.target.dataDetails[k].giaMua;
                    });
                }
            };

            $scope.changeVatRa = function (code) {
                if (code) {
                    serviceTax.getTaxByCode(code).then(function (response) {
                        if (response && response.status == 200 && response.data) {
                            $scope.target.tyLeVatRa = response.data.taxRate;
                            angular.forEach($scope.target.dataDetails, function (v, k) {
                                $scope.target.dataDetails[k].maVatRa = $scope.target.maVatRa;
                                $scope.target.dataDetails[k].giaBanBuonVat = $scope.target.giaBanBuon * $scope.target.tyLeVatRa / 100 + $scope.target.giaBanBuon;
                                $scope.target.dataDetails[k].giaBanLeVat = $scope.target.giaBanLe * $scope.target.tyLeVatRa / 100 + $scope.target.giaBanLe;
                            });
                        }
                    });
                } else {
                    $scope.target.tyLeVatRa = 0;
                    angular.forEach($scope.target.dataDetails, function (v, k) {
                        $scope.target.dataDetails[k].maVatRa = $scope.target.maVatRa;
                        $scope.target.dataDetails[k].giaBanBuonVat = $scope.target.giaBanBuon * $scope.target.tyLeVatRa / 100 + $scope.target.giaBanBuon;
                        $scope.target.dataDetails[k].giaBanLeVat = $scope.target.giaBanLe * $scope.target.tyLeVatRa / 100 + $scope.target.giaBanLe;
                    });
                }
            };
            function saveAvatar() {
                $scope.fileAvatar.maVatTu = $scope.target.maVatTu;
                upload.upload({
                    url: configService.rootUrlWebApi + '/Md/Merchandise/UploadAvatar',
                    data: $scope.fileAvatar
                }).then(function (response) {
                    if (response.status) {
                        $scope.target.avatarName = response.data.data;
                        saveMerchandise();
                    }
                    else {
                        toaster.pop('error', "Lỗi:", "Không lưu được ảnh! Có thể đã trùng!");
                    }
                });
            };

            function saveImage() {
                $scope.target.file = $scope.lstFile;
                upload.upload({
                    url: configService.rootUrlWebApi + '/Md/Merchandise/UploadMerchandiseImage',
                    data: $scope.target
                }).then(function (response) {
                    if (response.status) {
                        $scope.target.image = response.data.data;
                    }
                    else {
                        toaster.pop('error', "Lỗi:", "Không lưu được ảnh! Có thể đã trùng!");
                    }
                    if ($scope.fileAvatar) {
                        saveAvatar();
                    }
                    else {
                        saveMerchandise();
                    }
                });
            };
            $scope.uploadFile = function (input) {
                if (input.files && input.files.length > 0) {
                    angular.forEach(input.files, function (file) {
                        $scope.lstFile.push(file);
                        $timeout(function () {
                            var fileReader = new FileReader();
                            fileReader.readAsDataURL(file);
                            fileReader.onload = function (e) {
                                $timeout(function () {
                                    $scope.lstImagesSrc.push(e.target.result);
                                });
                            }
                        });
                    });
                }
            };
            $scope.deleteImage = function (index) {
                $scope.lstImagesSrc.splice(index, 1);
                $scope.lstFile.splice(index, 1);
                if ($scope.lstFile.length < 1) {
                    angular.element("#file-input-upload").val(null);
                }
            };

            function saveMerchandise() {
                if ($scope.isPending) { return; }
                $scope.isPending = true;
                $scope.target.maSize = '';
                $scope.target.maColor = '';
                angular.forEach($scope.target.maSizes, function (size) {
                    $scope.target.maSize = $scope.target.maSize + size.value + ',';
                });
                angular.forEach($scope.target.maColors, function (co) {
                    $scope.target.maColor = $scope.target.maColor + co.value + ',';
                });
                if ($scope.target.maVatTu !== $scope.tempMaVatTu) {
                    $scope.target.useGenCode = false;
                }
                service.postMatHangToOracleRoot(JSON.stringify($scope.target)).then(function (response) {
                    if (response && response.status === 201 && response.data && response.data.status && response.data.data) {
                        ngNotify.set('Thêm mới đồng bộ thành công', { type: 'success' });
                        $uibModalInstance.close($scope.target);
                    } else {
                        ngNotify.set(response.data.message, { duration: 3000, type: 'error' });
                    }
                    $scope.isPending = false;
                });
            };

            $scope.save = function () {
                if (!$scope.target.maLoaiVatTu) {
                    ngNotify.set("Chưa khai báo mã loại vật tư", { duration: 3000, type: 'error' });
                } else if (!$scope.target.maNhomVatTu) {
                    ngNotify.set("Chưa khai báo mã nhóm vật tư", { duration: 3000, type: 'error' });
                } else if (!$scope.target.maVatTu) {
                    ngNotify.set("Chưa khai báo mã vật tư", { duration: 3000, type: 'error' });
                } else if (!$scope.target.maKhachHang) {
                    ngNotify.set("Chưa khai báo mã nhà cung cấp", { duration: 3000, type: 'error' });
                } else {
                    if ($scope.lstFile && $scope.lstFile.length) {
                        saveImage();
                    }
                    else {
                        if ($scope.fileAvatar) {
                            saveAvatar();
                        }
                        else {
                            saveMerchandise();
                        }
                    }
                }
            };

            $scope.cancel = function () {
                $uibModalInstance.close();
            };
        }]);


    ///** matHang Edit Controller */
    app.controller('matHangEditController', ['$scope', '$uibModalInstance', 'configService', 'merchandiseService', 'tempDataService', '$filter', 'targetData', 'ngNotify', 'taxService', 'supplierService', 'merchandiseTypeService', 'nhomVatTuService', 'packagingService', 'shelvesService', 'donViTinhService',
        function ($scope, $uibModalInstance, configService, service, tempDataService, $filter, targetData, ngNotify, serviceTax, serviceSupplier, serviceMerchandiseType, serviceNhomVatTu, servicePackaging, serviceShelves, serviceDonViTinh) {
            $scope.config = angular.copy(configService);
            $scope.robot = service.robot;
            $scope.target = angular.copy(targetData);
            $scope.currentUser = {};
            $scope.lstFile = [];
            $scope.nhomVatTus = [];
            $scope.loaiVatTus = [];
            $scope.khachHangs = [];
            $scope.keHangs = [];
            $scope.taxs = [];
            $scope.tempData = tempDataService.tempData;
            $scope.title = function () { return 'Cập nhật đồng bộ Hàng hóa, Vật tư'; };
            $scope.displayHepler = function (paraValue, moduleName) {
                var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                if (data && data.length === 1) {
                    return data[0].text;
                } else {
                    return paraValue;
                }
            };
            $scope.displayHeplerVat = function (paraValue, moduleName) {
                var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                if (data && data.length === 1) {
                    return data[0].value + '|' + data[0].text;
                } else {
                    return paraValue;
                }
            };
            function loadTaxRoot() {
                if (!tempDataService.tempData('taxsRoot')) {
                    serviceTax.getAll_TaxRoot().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data && successRes.data.data.length > 0) {
                            tempDataService.putTempData('taxsRoot', successRes.data.data);
                            $scope.taxsRoot = successRes.data.data;
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.taxsRoot = tempDataService.tempData('taxsRoot');
                }
            };
            function loadSupplierRoot() {
                if (!tempDataService.tempData('suppliersRoot')) {
                    serviceSupplier.getAll_SupplierRoot().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data && successRes.data.data.length > 0) {
                            tempDataService.putTempData('suppliersRoot', successRes.data.data);
                            $scope.suppliersRoot = successRes.data.data;
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.suppliersRoot = tempDataService.tempData('suppliersRoot');
                }
            };

            function loadMerchandiseTypeRoot() {
                if (!tempDataService.tempData('merchandiseTypesRoot')) {
                    serviceMerchandiseType.getAll_MerchandiseTypeRoot().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data && successRes.data.data.length > 0) {
                            tempDataService.putTempData('merchandiseTypesRoot', successRes.data.data);
                            $scope.merchandiseTypesRoot = successRes.data.data;
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.merchandiseTypesRoot = tempDataService.tempData('merchandiseTypesRoot');
                }
            };

            function loadNhomVatTusRoot() {
                if (!tempDataService.tempData('nhomVatTusRoot')) {
                    serviceNhomVatTu.getAll_NhomVatTuRoot().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data && successRes.data.data.length > 0) {
                            tempDataService.putTempData('nhomVatTusRoot', successRes.data.data);
                            $scope.nhomVatTusRoot = successRes.data.data;
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.nhomVatTusRoot = tempDataService.tempData('nhomVatTusRoot');
                }
            };

            function loadPackagingsRoot() {
                if (!tempDataService.tempData('packagingsRoot')) {
                    servicePackaging.getAll_PackagingRoot().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data && successRes.data.data.length > 0) {
                            tempDataService.putTempData('packagingsRoot', successRes.data.data);
                            $scope.packagingsRoot = successRes.data.data;
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.packagingsRoot = tempDataService.tempData('packagingsRoot');
                }
            };

            function loadShelvesRoot() {
                if (!tempDataService.tempData('shelvesRoot')) {
                    serviceShelves.getAll_ShelvesRoot().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data && successRes.data.data.length > 0) {
                            tempDataService.putTempData('shelvesRoot', successRes.data.data);
                            $scope.shelvesRoot = successRes.data.data;
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.shelvesRoot = tempDataService.tempData('shelvesRoot');
                }
            };
            function loadDonViTinhRoot() {
                if (!tempDataService.tempData('donViTinhsRoot')) {
                    serviceDonViTinh.getAll_DonViTinhRoot().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data && successRes.data.data.length > 0) {
                            tempDataService.putTempData('donViTinhsRoot', successRes.data.data);
                            $scope.donViTinhsRoot = successRes.data.data;
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.donViTinhsRoot = tempDataService.tempData('donViTinhsRoot');
                }
            }

            loadTaxRoot();
            loadSupplierRoot();
            loadMerchandiseTypeRoot();
            loadNhomVatTusRoot();
            loadPackagingsRoot();
            loadShelvesRoot();
            loadDonViTinhRoot();

            $scope.changedMerchancedise = function (tenVatTu) {
                $scope.target.tenVietTat = tenVatTu;
            };
            //log2
            $scope.changeLoaiVatTu = function () {
                service.getSelectByMaLoaiRoot($scope.target.maLoaiVatTu).then(function (response) {
                    if (response && response.status == 200 && response.data) {
                        $scope.nhomVatTus = response.data;
                    }
                });
                service.getNewCodeRoot($scope.target.maLoaiVatTu).then(function (response) {
                    if (response && response.status == 200 && response.data) {
                        $scope.target.maVatTu = response.data;
                        $scope.maVatTu = response.data;
                        $scope.tempMaVatTu = angular.copy($scope.target.maVatTu);
                    }
                });
            };
            $scope.addBarCode = function (target) {
                if (!target.barcode) {
                    target.barcode = ";";
                }
                target.barcode = target.barcode + target.newBarcode + ";";
                target.newBarcode = "";
            };
            $scope.changeVatVao = function (code) {
                if (code) {
                    serviceTax.getTaxByCode(code).then(function (response) {
                        if (response && response.status == 200 && response.data) {
                            $scope.target.tyLeVatVao = response.data.taxRate;
                            angular.forEach($scope.target.dataDetails, function (v, k) {
                                $scope.target.dataDetails[k].maVatVao = $scope.target.maVatVao;
                                $scope.target.dataDetails[k].tyLeVatVao = $scope.target.tyLeVatVao;
                                $scope.target.dataDetails[k].giaMuaVat = $scope.target.dataDetails[k].giaMua * $scope.target.dataDetails[k].tyLeVatVao / 100 + $scope.target.dataDetails[k].giaMua;
                            });
                        }
                    });
                } else {
                    $scope.target.tyLeVatVao = 0;
                    angular.forEach($scope.target.dataDetails, function (v, k) {
                        $scope.target.dataDetails[k].maVatVao = $scope.target.maVatVao;
                        $scope.target.dataDetails[k].tyLeVatVao = $scope.target.tyLeVatVao;
                        $scope.target.dataDetails[k].giaMuaVat = $scope.target.dataDetails[k].giaMua * $scope.target.dataDetails[k].tyLeVatVao / 100 + $scope.target.dataDetails[k].giaMua;
                    });
                }
            };

            $scope.changeVatRa = function (code) {
                if (code) {
                    serviceTax.getTaxByCode(code).then(function (response) {
                        if (response && response.status == 200 && response.data) {
                            $scope.target.tyLeVatRa = response.data.taxRate;
                            angular.forEach($scope.target.dataDetails, function (v, k) {
                                $scope.target.dataDetails[k].maVatRa = $scope.target.maVatRa;
                                $scope.target.dataDetails[k].giaBanBuonVat = $scope.target.giaBanBuon * $scope.target.tyLeVatRa / 100 + $scope.target.giaBanBuon;
                                $scope.target.dataDetails[k].giaBanLeVat = $scope.target.giaBanLe * $scope.target.tyLeVatRa / 100 + $scope.target.giaBanLe;
                            });
                        }
                    });
                } else {
                    $scope.target.tyLeVatRa = 0;
                    angular.forEach($scope.target.dataDetails, function (v, k) {
                        $scope.target.dataDetails[k].maVatRa = $scope.target.maVatRa;
                        $scope.target.dataDetails[k].giaBanBuonVat = $scope.target.giaBanBuon * $scope.target.tyLeVatRa / 100 + $scope.target.giaBanBuon;
                        $scope.target.dataDetails[k].giaBanLeVat = $scope.target.giaBanLe * $scope.target.tyLeVatRa / 100 + $scope.target.giaBanLe;
                    });
                }
            };
            function saveAvatar() {
                $scope.fileAvatar.maVatTu = $scope.target.maVatTu;
                upload.upload({
                    url: configService.rootUrlWebApi + '/Md/Merchandise/UploadAvatar',
                    data: $scope.fileAvatar
                }).then(function (response) {
                    if (response.status) {
                        $scope.target.avatarName = response.data.data;
                        saveMerchandise();
                    } else {
                        toaster.pop('error', "Lỗi:", "Không lưu được ảnh! Có thể đã trùng!");
                    }
                });
            };

            function saveImage() {
                $scope.target.file = $scope.lstFile;
                upload.upload({
                    url: configService.rootUrlWebApi + '/Md/Merchandise/UploadMerchandiseImage',
                    data: $scope.target
                }).then(function (response) {
                    if (response.status) {
                        $scope.target.image = response.data.data;
                    } else {
                        toaster.pop('error', "Lỗi:", "Không lưu được ảnh! Có thể đã trùng!");
                    }
                    if ($scope.fileAvatar) {
                        saveAvatar();
                    } else {
                        saveMerchandise();
                    }
                });
            };
            $scope.uploadFile = function (input) {
                if (input.files && input.files.length > 0) {
                    angular.forEach(input.files, function (file) {
                        $scope.lstFile.push(file);
                        $timeout(function () {
                            var fileReader = new FileReader();
                            fileReader.readAsDataURL(file);
                            fileReader.onload = function (e) {
                                $timeout(function () {
                                    $scope.lstImagesSrc.push(e.target.result);
                                });
                            }
                        });
                    });
                }
            };
            $scope.deleteImage = function (index) {
                $scope.lstImagesSrc.splice(index, 1);
                $scope.lstFile.splice(index, 1);
                if ($scope.lstFile.length < 1) {
                    angular.element("#file-input-upload").val(null);
                }
            };

            function filterDataUpdate() {
                if ($scope.target.maVatTu) {
                    service.getDetailByCodeRoot($scope.target.maVatTu).then(function (response) {
                        if (response && response.status == 200 && response.data && response.data.data) {
                            $scope.target = response.data.data;
                        }
                    });
                }
                else {
                    ngNotify.set("Không tìm thấy chi tiết hàng hóa", { duration: 3000, type: 'error' });
                }
            }
            filterDataUpdate();

            function saveMerchandise() {
                if ($scope.isPending) { return; }
                $scope.isPending = true;
                $scope.target.maSize = '';
                $scope.target.maColor = '';
                angular.forEach($scope.target.maSizes, function (size) {
                    $scope.target.maSize = $scope.target.maSize + size.value + ',';
                });
                angular.forEach($scope.target.maColors, function (co) {
                    $scope.target.maColor = $scope.target.maColor + co.value + ',';
                });
                if ($scope.target.maVatTu !== $scope.tempMaVatTu) {
                    $scope.target.useGenCode = false;
                }
                service.updateMatHangToOracleRoot(JSON.stringify($scope.target)).then(function (response) {
                    if (response && response.status === 201 && response.data && response.data.status && response.data.data) {
                        ngNotify.set('Thêm mới đồng bộ thành công', { type: 'success' });
                        $uibModalInstance.close($scope.target);
                    } else {
                        ngNotify.set(response.data.message, { duration: 3000, type: 'error' });
                    }
                    $scope.isPending = false;
                });
            };

            $scope.save = function () {
                if (!$scope.target.maLoaiVatTu) {
                    ngNotify.set("Chưa khai báo mã loại vật tư", { duration: 3000, type: 'error' });
                } else if (!$scope.target.maNhomVatTu) {
                    ngNotify.set("Chưa khai báo mã nhóm vật tư", { duration: 3000, type: 'error' });
                } else if (!$scope.target.maVatTu) {
                    ngNotify.set("Chưa khai báo mã vật tư", { duration: 3000, type: 'error' });
                } else if (!$scope.target.maKhachHang) {
                    ngNotify.set("Chưa khai báo mã nhà cung cấp", { duration: 3000, type: 'error' });
                } else {
                    if ($scope.lstFile && $scope.lstFile.length) {
                        saveImage();
                    } else {
                        if ($scope.fileAvatar) {
                            saveAvatar();
                        }
                        else {
                            saveMerchandise();
                        }
                    }
                }
            };

            $scope.cancel = function () {
                $uibModalInstance.close();
            };
        }]);

    /** matHang Details Controller*/
    app.controller('matHangDetailsController', ['$scope', '$uibModalInstance', 'configService', 'merchandiseService', 'tempDataService', 'targetData',
        function ($scope, $uibModalInstance, configService, service, tempDataService, targetData) {
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.robot = service.robot;
            $scope.target = targetData;
            $scope.tempData = tempDataService.tempData;
            $scope.title = function () { return 'Thông tin đồng bộ Hàng hóa, Vật tư'; };
            $scope.pageChanged = function () {
                var currentPage = $scope.paged.currentPage;
                var itemsPerPage = $scope.paged.itemsPerPage;
                $scope.paged.totalItems = $scope.target.listData.length;
                $scope.data = [];
                if ($scope.target.listData) {
                    for (var i = (currentPage - 1) * itemsPerPage;
                        i < currentPage * itemsPerPage && i < $scope.target.listData.length;
                        i++) {
                        $scope.data.push($scope.target.listData[i]);
                    }
                }
            };
            $scope.cancel = function () {
                $uibModalInstance.close();
            };
        }]);


    /**vatTu Select Data Controller*/
    app.controller('vatTuSelectDataController', ['$scope', '$uibModalInstance', 'configService', 'merchandiseService', 'ngNotify', 'filterObject', 'serviceSelectData',
        function ($scope, $uibModalInstance, configService, service, ngNotify, filterObject, serviceSelectData) {
            $scope.config = angular.copy(configService);
            $scope.robot = service.robot;
            $scope.filtered = angular.copy(configService.filterDefault);
            $scope.filtered = angular.extend($scope.filtered, filterObject);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.changeAvartar = function (event) {
                if (event.target.checked) {
                    $scope.isAvatar = true;
                } else {
                    $scope.isAvatar = false;
                }
            }
            $scope.title = function () { return 'Danh sách hàng hóa'; };
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
                $scope.listSelectedData = serviceSelectData.getSelectData();
                $scope.isLoading = true;
                var postdata = { paged: $scope.paged, filtered: $scope.filtered };
                service.postSelectDataQuery(postdata).then(function (response) {
                    $scope.isLoading = false;
                    if (response && response.status === 200 && response.data.status && response.data.data && response.data.data.data.length > 0) {
                        $scope.data = response.data.data.data;
                        angular.forEach($scope.data, function (v, k) {
                            var isSelected = $scope.listSelectedData.some(function (element, index, array) {
                                if (!element) return false;
                                return element.maHang === v.maHang;
                            });
                            if (isSelected) {
                                $scope.data[k].selected = true;
                            }
                        });
                        angular.extend($scope.paged, response.data.data);
                        if (response.message) {
                            ngNotify.set(response.message, { duration: 3000, type: 'error' });
                        }
                    }
                });
            };
            $scope.setPage = function (pageNo) {
                $scope.paged.currentPage = pageNo;
                filterData();
            };
            $scope.sortType = 'maVatTu'; // set the default sort type
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
            $scope.selecteItem = function (item) {
                $uibModalInstance.close(item);
            }
            filterData();
            $scope.cancel = function () {
                $uibModalInstance.close();
            };
        }]);
    //end controller đồng bộ

    app.controller('merchandiseSelectDataForNmController', ['$scope', '$uibModalInstance', 'configService', 'merchandiseService', 'filterObject',
        function ($scope, $uibModalInstance, configService, service, filterObject) {
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.summary = angular.copy(filterObject.summary);
            $scope.isLoading = false;
            $scope.dataPaging = [];
            $scope.title = function () { return 'Hàng hóa, Vật tư'; };

            function filterData() {
                $scope.listSelectedData = service.getSelectData();
                $scope.isLoading = true;
                service.getForNvNhapMua(filterObject.maKhachHang, filterObject.unitCode, $scope.summary).then(function (successRes) {
                    $scope.isLoading = false;
                    if (successRes && successRes.status === 200 && successRes.data && successRes.data.status) {
                        $scope.data = successRes.data.data;
                        $scope.pageChanged();
                    }
                });
                $scope.cancel = function () {
                    $uibModalInstance.dismiss('cancel');
                };
            }
            filterData();

            $scope.selectItem = function (item) {
                $uibModalInstance.close(item);
            };

            $scope.pageChanged = function () {
                var currentPage = $scope.paged.currentPage;
                var itemsPerPage = 10;
                $scope.paged.totalItems = $scope.data.length;
                $scope.dataPaging = [];
                if ($scope.data) {
                    for (var i = (currentPage - 1) * itemsPerPage; i < currentPage * itemsPerPage && i < $scope.data.length; i++) {
                        $scope.dataPaging.push($scope.data[i]);
                    }
                }
            };

            $scope.doSearch = function () {
                $scope.paged.currentPage = 1;
                filterData();
            };

            $scope.cancel = function () {
                $uibModalInstance.close();
            };
        }]);

    return app;
});