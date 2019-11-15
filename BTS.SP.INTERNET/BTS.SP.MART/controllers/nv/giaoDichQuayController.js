/*  
* Người tạo : Nguyễn Tuấn Hoàng Anh
* View: BTS.SP.MART/views/nv/NvGiaoDichQuay
* Vm sevices: BTS.API.SERVICE -> NV ->NvGiaoDichQuayVm.cs
* Sevices: BTS.API.SERVICE -> NV -> NvGiaoDichQuayService.cs
* Entity: BTS.API.ENTITY -> NV - > NvGiaoDichQuay.cs
* Menu: Nghiệp vụ-> Danh sách giao dịch quầy
*/
define(['ui-bootstrap', '/BTS.SP.MART/controllers/auth/AuthController.js', '/BTS.SP.MART/controllers/htdm/periodController.js', '/BTS.SP.MART/controllers/htdm/merchandiseController.js', '/BTS.SP.MART/controllers/htdm/customerController.js', '/BTS.SP.MART/controllers/htdm/merchandiseTypeController.js', '/BTS.SP.MART/controllers/htdm/nhomVatTuController.js', '/BTS.SP.MART/controllers/htdm/supplierController.js', '/BTS.SP.MART/controllers/htdm/wareHouseController.js', '/BTS.SP.MART/controllers/htdm/packagingController.js', '/BTS.SP.MART/controllers/htdm/taxController.js', '/BTS.SP.MART/controllers/htdm/donViTinhController.js'], function () {
    'use strict';
    var app = angular.module('giaoDichQuayModule', ['ui.bootstrap', 'authModule', 'periodModule', 'merchandiseModule', 'customerModule', 'merchandiseTypeModule', 'nhomVatTuModule', 'supplierModule', 'wareHouseModule', 'packagingModule', 'taxModule', 'donViTinhModule']);
    app.factory('giaoDichQuayService', ['$http', 'configService', function ($http, configService) {
        var serviceUrl = configService.rootUrlWebApi + '/Nv/GiaoDichQuay';
        var rootUrl = configService.apiServiceBaseUri;
        this.parameterPrint = {};
        var selectedData = [];
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
                item.thanhTienVAT = item.thanhTien * (1 + item.tyLeVatVao / 100);

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
                item.thanhTienVAT = item.thanhTien * (1 + item.tyLeVatVao / 100);

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
                item.soLuong = item.soLuongBao * item.luongBao + item.soLuongLe;
                item.thanhTien = item.soLuong * item.donGia;
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
            getDataReport: function (id) {
                return $http.get(serviceUrl + '/GetDataReport/' + id);
            },
            postDetails: function (data) {
                return $http.post(serviceUrl + '/PostDetails/', data);
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
            getMerchandiseForNvByCode: function (code) {
                return $http.get(rootUrl + '/api/Md/Merchandise/GetForNvByCode/' + code);
            },
            getMerchandise: function (maChungTuPk) {
                return $http.get(rootUrl + '/api/Nv/NhapHangMua/GetMerchandise/' + maChungTuPk);
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
            getDetailByCode: function (code) {
                return $http.get(rootUrl + '/api/Md/Merchandise/GetDetailByCode/' + code);
            },
            getByCode: function (code) {
                return $http.get(rootUrl + '/api/Md/Merchandise/GetByCode/' + code);
            },
            getDetailInTem: function (code) {
                return $http.get(rootUrl + '/api/Md/Merchandise/GetDetailInTem/' + code);
            },
            deleteItem: function (params) {
                return $http.delete(serviceUrl + '/' + params.id, params);
            },
            postExportExcelGDQByMerchandiseType: function (json) {
                return $http({
                    url: serviceUrl + '/PostExportExcelGDQByMerchandiseType',
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
                    a.download = "GiaoDichQuayTheoLoaiHang.xlsx";
                    a.click();
                    // window.URL.revokeObjectURL(objectUrl);
                }).error(function (data, status, headers, config) {
                    //upload failed
                });

                //$http.post(serviceUrl + '/WriteDataToExcel', data);
            },
            exportExcelDetailsByCondition: function (instance) {
                return $http({
                    url: serviceUrl + '/ExportExcelDetailsByCondition',
                    method: "POST",
                    data: instance, //this is your json data string
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
                    var fileName = '';
                    if (instance.tuNgay && instance.denNgay) {
                        fileName = 'GiaoDichQuay-Tu' + instance.tuNgay + 'Den' + instance.denNgay
                    }
                    else {
                        fileName = 'GiaoDichQuay';
                    }
                    a.download = fileName + '.xlsx';
                    a.click();
                    // window.URL.revokeObjectURL(objectUrl);
                }).error(function (data, status, headers, config) {
                    //upload failed
                });

                //$http.post(serviceUrl + '/WriteDataToExcel', data);
            },
            exportSyntheticExcelByCondition: function (instance) {
                return $http({
                    url: serviceUrl + '/ExportExcelSyntheticByCondition',
                    method: "POST",
                    data: instance, //this is your json data string
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
                    var fileName = '';
                    if (instance.tuNgay && instance.denNgay) {
                        fileName = 'GiaoDichQuayTongHop-Tu' + instance.tuNgay + 'Den' + instance.denNgay
                    }
                    else {
                        fileName = 'GiaoDichQuayTongHop';
                    }
                    a.download = fileName + '.xlsx';
                    a.click();
                    // window.URL.revokeObjectURL(objectUrl);
                }).error(function (data, status, headers, config) {
                    //upload failed
                });

                //$http.post(serviceUrl + '/WriteDataToExcel', data);
            },
            postExportExcelGDQByMerchandiseGroup: function (json) {
                return $http({
                    url: serviceUrl + '/PostExportExcelGDQByMerchandiseGroup',
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
                    a.download = "GiaoDichQuayTheoNhomHang.xlsx";
                    a.click();
                    // window.URL.revokeObjectURL(objectUrl);
                }).error(function (data, status, headers, config) {
                    //upload failed
                });

                //$http.post(serviceUrl + '/WriteDataToExcel', data);
            },
            postExportExcelGDQByMerchandiseNCC: function (json) {
                return $http({
                    url: serviceUrl + '/PostExportExcelGDQByMerchandiseNCC',
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
                    a.download = "GiaoDichQuayTheoNhacCungCap.xlsx";
                    a.click();
                    // window.URL.revokeObjectURL(objectUrl);
                }).error(function (data, status, headers, config) {
                    //upload failed
                });

                //$http.post(serviceUrl + '/WriteDataToExcel', data);
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
                    a.download = "GiaoDichQuayTheoHangHoa.xlsx";
                    a.click();
                    // window.URL.revokeObjectURL(objectUrl);
                }).error(function (data, status, headers, config) {
                    //upload failed
                });

                //$http.post(serviceUrl + '/WriteDataToExcel', data);
            },
            postSelectDataQuery: function (data) {
                return $http.post(serviceUrl + '/PostSelectDataQuery', data);
            },
            getAllDataTrade: function (codeTrade) {
                return $http.get(serviceUrl + '/GetAllDataTrade/' + codeTrade);
            },
            //service set ; get data
            getSelectData: function () {
                return selectedData;
            },
            setSelectData: function (array) {
                selectedData = array;
            }
        };
        return result;
    }]);
    /* controller list */
    app.controller('giaoDichQuayController', [
        '$scope', '$location', '$http', 'configService', 'giaoDichQuayService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'securityService', '$rootScope', 'toaster', 'periodService', 'merchandiseService', 'customerService', 'merchandiseTypeService', 'nhomVatTuService', 'supplierService', 'wareHouseService', 'packagingService', 'taxService', 'donViTinhService',
        function ($scope, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, securityService, $rootScope, toaster, servicePeriod, serviceMerchandise, serviceCustomer, serviceMerchandiseType, serviceNhomVatTu, serviceSupplier, serviceWareHouse, servicePackaging, serviceTax, serviceDonViTinh) {
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
                    var postdata = { paged: $scope.paged, filtered: $scope.filtered };
                    $scope.isLoading = true;
                    service.postQuery(postdata).then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data && successRes.data.status) {
                            $scope.isLoading = false;
                            $scope.data = successRes.data.data.data;
                            angular.extend($scope.paged, successRes.data.data);
                        }
                    });
                }
            };
            //end
            //check quyền truy cập
            function loadAccessList() {
                securityService.getAccessList('nvGiaoDichQuay').then(function (successRes) {
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
                serviceTax.getAll_Tax().then(function (successRes) {
                    if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                        tempDataService.putTempData('taxs', successRes.data.data);
                        $scope.taxs = successRes.data.data;
                    }
                }, function (errorRes) {
                    console.log('errorRes', errorRes);
                });
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
            $scope.setPage = function (pageNo) {
                $scope.paged.currentPage = pageNo;
                filterData();
            };
            $scope.sortType = 'ngayPhatSinh';
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
            $scope.title = function () { return 'Danh sách hàng giao dịch quầy'; };

            /* Function add New Item */
            $scope.create = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    size: 'md',
                    templateUrl: configService.buildUrl('nv/NvGiaoDichQuay', 'add'),
                    controller: 'giaoDichQuayCreateController',
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
                    templateUrl: configService.buildUrl('nv/NvGiaoDichQuay', 'update'),
                    controller: 'giaoDichQuayEditController',
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
                    templateUrl: configService.buildUrl('nv/NvGiaoDichQuay', 'details'),
                    controller: 'giaoDichQuayDetailsController',
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
            $scope.sum = function () {
                var total = 0;
                if ($scope.data) {
                    angular.forEach($scope.data, function (v, k) {
                        total = total + v.tTienCoVat;
                    });
                }
                return total;
            };
            $scope.exportSyntheticByCondition = function (param) {
                if (param) {
                    service.exportSyntheticExcelByCondition(param);
                }
            };
            $scope.exportDetailsByCondition = function (param) {
                if (param) {
                    service.exportExcelDetailsByCondition(param);
                }
            };
            $scope.print = function () {
                var postdata = { paged: $scope.paged, filtered: $scope.filtered };
                service.setParameterPrint(postdata);
                $state.go("nvPrintPhieuNhapMua");
            };
            $scope.printDetail = function () {
                var postdata = { paged: $scope.paged, filtered: $scope.filtered };
                service.setParameterPrint(postdata);
                $state.go("nvPrintDetailgiaoDichQuay");
            };
            $scope.exportExcel = function (option) {
                var postdata = {};
                switch (option) {
                    case "loaiHang":
                        postdata = { paged: $scope.paged, filtered: $scope.filtered };
                        service.postExportExcelGDQByMerchandiseType(postdata);
                        break;
                    case "nhomHang":
                        postdata = { paged: $scope.paged, filtered: $scope.filtered };
                        service.postExportExcelGDQByMerchandiseGroup(postdata);
                        break;
                    case "nhaCungCap":
                        postdata = { paged: $scope.paged, filtered: $scope.filtered };
                        service.postExportExcelGDQByMerchandiseNCC(postdata);
                        break;
                    case "hangHoa":
                        postdata = { paged: $scope.paged, filtered: $scope.filtered };
                        service.postExportExcel(postdata);
                        break;
                    default:
                        postdata = { paged: $scope.paged, filtered: $scope.filtered };
                        service.postExportExcel(postdata);
                        break;
                }
            };
        }]);

    /* controller addNew */
    app.controller('giaoDichQuayCreateController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'giaoDichQuayService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', '$rootScope', 'userService', 'FileUploader', 'merchandiseService', 'customerService', 'merchandiseTypeService', 'nhomVatTuService', 'supplierService', 'wareHouseService', 'packagingService', 'taxService', 'donViTinhService', 'periodService',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, $rootScope, serviceAuthUser, FileUploader, serviceMerchandise, serviceCustomer, serviceMerchandiseType, serviceNhomVatTu, serviceSupplier, serviceWareHouse, servicePackaging, serviceTax, serviceDonViTinh, servicePeriod) {
            var currentUser = serviceAuthUser.GetCurrentUser();
            var unitCode = currentUser.unitCode;
            var rootUrl = configService.apiServiceBaseUri;
            var serviceUrl = rootUrl + '/api/Nv/NhapHangMua';
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
                        updatedData.donGia = updatedData.giaMua;
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
                var itemsPerPage = $scope.paged.itemsPerPage;
                var currentPageIndex = (currentPage - 1) * itemsPerPage + index;
                $scope.target.dataDetails.splice(currentPageIndex, 1);
                if ($scope.target.dataDetails.length == 0) {
                    $scope.isListItemNull = true;
                }
                $scope.pageChanged();
            };
            //end vudq import excel
            $scope.selectedkhachHang = function (item) {
                $scope.target.maKhachHang = item.value;
                service.getOrderByCustomer(item.value).then(function (response) {
                    if (response && response && response.data.length > 0) {
                        $scope.donHangs = response;
                    }
                });
                service.getCustomer(item.id, function (response) {
                    if (response && response && response.data) {
                        $scope.target.maSoThue = response.maSoThue;
                    }
                });
            };
            $scope.selectedDonDatHang = function (item) {
                $scope.isLoading = true;
                service.getOrderById(item.id).then(function (response) {
                    var donHang = response.data;
                    $scope.target.dataDetails = []
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
                    service.getMerchandiseForNvByCode(code).then(function (response) {
                        if (response && response.status == 200 && response.data && response.data.status) {
                            $scope.newItem = response.data.data;
                            $scope.newItem.donGia = $scope.newItem.giaMua;
                            $scope.newItem.validateCode = response.data.data.maHang;
                            $scope.newItem.giaMuaCoVat = response.data.data.giaMua * (1 + response.data.data.tyLeVatVao / 100);
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
            $scope.save = function () {
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
                        $scope.target.dataDetails = []
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
                        var url = $state.href('reportgiaoDichQuay', { id: response.data.id });
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
    app.controller('giaoDichQuayEditController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'giaoDichQuayService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify', 'merchandiseService',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify, serviceMerchandise) {
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
            $scope.title = function () { return 'Cập nhật phiếu xuất bán'; };
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
                                $scope.target.dataDetails[k].soLuong = $scope.newItem.soLuong + $scope.target.dataDetails[k].soLuong;
                                $scope.target.dataDetails[k].soLuongBao = $scope.newItem.soLuongBao + $scope.target.dataDetails[k].soLuongBao;
                                $scope.target.dataDetails[k].soLuongLe = $scope.newItem.soLuongLe + $scope.target.dataDetails[k].soLuongLe;
                                $scope.target.dataDetails[k].thanhTien = $scope.newItem.soLuong * $scope.target.dataDetails[k].donGia;
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
                    $scope.target.dataDetails = []
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
                    service.getMerchandiseForNvByCode(code).then(function (response) {
                        if (response && response.status == 200 && response.data && response.data.status) {
                            $scope.newItem = response.data.data;
                            $scope.newItem.donGia = $scope.newItem.giaMua;
                            $scope.newItem.validateCode = response.data.data.maHang;
                            $scope.newItem.giaMuaCoVat = response.data.data.giaMua * (1 + response.data.data.tyLeVatVao / 100);
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
                        var url = $state.href('reportgiaoDichQuay', { id: successRes.data.data.id });
                        window.open(url, 'Report Viewer');
                        $scope.target.dataDetails = []
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
    app.controller('giaoDichQuayDetailsController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'giaoDichQuayService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify) {
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.targetData = angular.copy(targetData);
            $scope.tempData = tempDataService.tempData;
            $scope.target = targetData;
            $scope.isLoading = false;
            $scope.title = function () { return 'Chi tiết giao dịch quầy'; };
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
                service.postDetails($scope.target).then(function (resgetDetails) {
                    if (resgetDetails && resgetDetails.status == 200 && resgetDetails.data) {
                        $scope.target = resgetDetails.data;
                        $scope.target.ngayCT = new Date($scope.target.ngayCT);
                        $scope.target.ngayHoaDon = new Date($scope.target.ngayHoaDon);
                        if ($scope.target.loaiGiaoDich === 1) $scope.target.loaiGiaoDich = 'BANLE';
                        else if ($scope.target.loaiGiaoDich === 2) $scope.target.loaiGiaoDich = 'TRALAI';
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
            };
            fillterData();
            $scope.cancel = function () {
                $uibModalInstance.close();
            };

        }]);

    app.controller('reportGiaoDichQuayController', ['$scope', '$location', '$http', 'configService', 'giaoDichQuayService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', '$stateParams', '$timeout', '$state', 'userService', '$window', 'taxService',
         function ($scope, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, $stateParams, $timeout, $state, userService, $window, serviceTax) {
             $scope.robot = angular.copy(service.robot);
             $scope.tempData = tempDataService.tempData;
             var id = $stateParams.id;
             $scope.target = {};
             $scope.goIndex = function () {
                 $state.go('nvGiaoDichQuay');
             }
             function filterData() {
                 if (id) {
                     service.getDataReport(id).then(function (response) {
                         if (response && response.status == 200 && response.data) {
                             $scope.target = response.data;
                             $scope.target.NgayIn = new Date();
                             $scope.target.username = userService.GetCurrentUser().fullName;
                         }
                     });
                 }
             };

             $scope.displayHepler = function (paraValue, moduleName) {
                 var data = $filter('filter')(tempDataService.tempData(moduleName), { extendValue: paraValue + '' }, true);
                 if (data && data.length === 1) {
                     return data[0].text;
                 } else {
                     return paraValue;
                 }
             };
             $scope.print = function () {
                 var table = document.getElementById('main-report').innerHTML;
                 var myWindow = $window.open('', '', 'width=800, height=600');
                 myWindow.document.write(table);
                 myWindow.print();
             }
             filterData();

             $scope.printExcel = function () {
                 var data = [document.getElementById('main-report').innerHTML];
                 var fileName = "PhieuGiaoDichQuay.xls";
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
         }
    ]);
    /* controller SelectData*/
    app.controller('nvGiaoDichQuaySelectDataController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'giaoDichQuayService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'filterObject',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, filterObject) {
            $scope.config = angular.copy(configService);
            $scope.filtered = angular.copy(configService.filterDefault);
            $scope.filtered = angular.extend($scope.filtered, filterObject);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.isLoading = false;
            $scope.title = function () { return 'Danh sách giao dịch quầy'; };
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
            };
            $scope.setPage = function (pageNo) {
                $scope.paged.currentPage = pageNo;
                filterData();
            };
            $scope.selecteItem = function (item) {
                service.getAllDataTrade(item.maGiaoDich).then(function (response) {
                    $uibModalInstance.close(response.data);
                });
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
            $scope.printReturn = function (item) {
                if (item) {
                    $scope.target = {};
                    $scope.target = {
                        dataDto: {},
                        dataDetails: []
                    };
                    if (item.maGiaoDich) {
                        var allTienKhuyenMai = 0;
                        service.getDataReport(item.maGiaoDich).then(function (response) {
                            if (response && response.status == 200 && response.data) {
                                $scope.target.dataDto.id = response.data.id;
                                $scope.target.dataDto.unitCode = response.data.maDonVi;
                                $scope.target.dataDto.maGiaoDich = response.data.maGiaoDich;
                                $scope.target.dataDto.ngayChungTu = response.data.ngayPhatSinh;
                                $scope.target.dataDto.maNhanVien = response.data.maNguoiTao;
                                $scope.target.dataDto.maVoucher = response.data.maVoucher;
                                $scope.target.dataDto.makh = response.data.maKhachHang;
                                $scope.target.dataDto.maThe = response.data.maThe;
                                $scope.target.dataDto.ghiChu = response.data.ghiChu;
                                $scope.target.dataDto.allSumTienHang = response.data.tTienCoVat;
                                $scope.target.dataDto.allSumTienKhuyenMai = response.data.tienKhuyenMai;
                                $scope.target.dataDto.tienVoucher = response.data.tienVoucher;
                                $scope.target.dataDto.tienThe = response.data.tienThe === null ? 0 : response.data.tienThe;
                                $scope.target.dataDto.tienCOD = response.data.tienCOD === null ? 0 : response.data.tienCOD;
                                $scope.target.dataDto.tienMat = response.data.tienMat === null ? 0 : response.data.tienMat;
                                $scope.target.dataDto.tienKhachDua = response.data.tienKhachDua;
                                $scope.target.dataDto.tienThua = response.data.tienTraLai === '' ? 0 : response.data.tienTraLai;
                                $scope.target.dataDto.thoiGian = response.data.thoiGian;
                                angular.forEach(response.data.dataDetails, function (value, index) {
                                    value.tenVatTu = value.tenDayDu;
                                    value.soLuong = value.soLuong;
                                    value.giaBanLeVat = value.giaBanLeCoVat;
                                    value.tyLeChietKhau = value.tyLeChietKhau;
                                    value.tienDuocKhuyenMai = value.tienKhuyenMai;
                                    value.thanhTien = value.giaBanLeCoVat;
                                    allTienKhuyenMai = allTienKhuyenMai + value.tienDuocKhuyenMai;
                                    $scope.target.dataDetails.push(value);
                                });
                                $scope.target.dataDto.tongTienKhuyenMai = parseInt(allTienKhuyenMai, 10);
                                $scope.target.dataDto.tongTien = parseInt(allTienKhuyenMai, 10) + $scope.target.dataDto.allSumTienHang;
                            }
                        });
                    }
                }
            };
            $scope.printDiv = function (divName) {
                setTimeout(function () {
                    var printContents = document.getElementById(divName).innerHTML;
                    var originalContents = document.body.innerHTML;
                    if (navigator.userAgent.toLowerCase().indexOf('chrome') > -1) {
                        var popupWin = window.open('', '_blank', 'width=600,height=600,scrollbars=no,menubar=no,toolbar=no,location=no,status=no,titlebar=no');
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
                        var popupWin = window.open('', '_blank', 'width=800,height=600');
                        popupWin.document.open();
                        popupWin.document.write('<html><head><link rel="stylesheet" type="text/css" href="style.css" /></head><body onload="window.print()">' + printContents + '</html>');
                        popupWin.document.close();
                    }
                    popupWin.document.close();
                    return true;
                }, 1000);
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
            $scope.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };
            filterData();
        }]);
    return app;
});

