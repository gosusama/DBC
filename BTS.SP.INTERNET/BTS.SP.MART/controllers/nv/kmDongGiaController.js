define(['ui-bootstrap', '/BTS.SP.MART/controllers/auth/AuthController.js', '/BTS.SP.MART/controllers/htdm/periodController.js', '/BTS.SP.MART/controllers/htdm/merchandiseController.js', '/BTS.SP.MART/controllers/htdm/customerController.js', '/BTS.SP.MART/controllers/htdm/merchandiseTypeController.js', '/BTS.SP.MART/controllers/htdm/nhomVatTuController.js', '/BTS.SP.MART/controllers/htdm/supplierController.js', '/BTS.SP.MART/controllers/htdm/wareHouseController.js', '/BTS.SP.MART/controllers/htdm/packagingController.js', '/BTS.SP.MART/controllers/htdm/taxController.js', '/BTS.SP.MART/controllers/htdm/donViTinhController.js', '/BTS.SP.MART/controllers/htdm/typeReasonController.js', '/BTS.SP.MART/controllers/auth/AuNguoiDung.js', '/BTS.SP.MART/controllers/nv/khuyenMaiController.js'], function () {
    'use strict';

    var app = angular.module('kmDongGiaModule', ['ui.bootstrap', 'authModule', 'periodModule', 'merchandiseModule', 'customerModule', 'merchandiseTypeModule', 'nhomVatTuModule', 'supplierModule', 'wareHouseModule', 'packagingModule', 'taxModule', 'donViTinhModule', 'typeReasonModule', 'AuNguoiDungModule', 'khuyenMaiModule']);
    app.factory('kmDongGiaService', [
        '$resource', '$http', '$window', 'configService',
        function ($resource, $http, $window, configService) {
            var rootUrl = configService.apiServiceBaseUri;
            var serviceUrl = rootUrl + '/api/Nv/KhuyenMaiDongGia';
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
                changeTyLeKhuyenMai: function (item) {
                    if (!item.donGia) {
                        item.giaTriKhuyenMai = 0;
                    }
                    if (item.tyLeKhuyenMai < 100) {
                        item.giaTriKhuyenMai = item.donGia - (item.donGia * item.tyLeKhuyenMai / 100);
                    } else {
                        item.giaTriKhuyenMai = item.donGia - item.tyLeKhuyenMai;
                    }
                }
            }
            var parameterPrint = {};
            var selectedMerchandise = [];

            function getParameterPrint() {
                return parameterPrint;
            }

            var result = {
                robot: calc,
                setParameterPrint: function (data) {
                    parameterPrint = data;
                },
                getParameterPrint: function () {
                    return parameterPrint;
                },
                getAllData: function () {
                    return $http.post(serviceUrl + '/GetAllData');
                },
                post: function (data) {
                    return $http.post(serviceUrl + '/Post', data);
                },
                postQuery: function (data) {
                    return $http.post(serviceUrl + '/PostQuery', data);
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
                getNewInstance: function (callback) {
                    $http.get(serviceUrl + '/GetNewInstance').success(callback);
                },
                getDetails: function (id) {
                    return $http.get(serviceUrl + '/GetDetails/' + id);
                },
                update: function (params) {
                    return $http.put(serviceUrl + '/' + params.id, params);
                },
                approval: function (params, callback) {
                    return $http.post(serviceUrl + '/PostApproval/' + params.id).success(callback);
                },
                unapprove: function (params, callback) {
                    return $http.post(serviceUrl + '/PostUnApprove/' + params.id).success(callback);
                },
                getMerchandiseForNvByCode: function (code, callback) {
                    return $http.get(rootUrl + '/api/Md/Merchandise/GetForNvByCode/' + code).success(callback);
                },
                getMerchandiseTypeForNvByCode: function (code) {
                    return $http.get(rootUrl + '/api/Md/MerchandiseType/GetForNvByCode/' + code);
                },
                getMerchandiseGroupForNvByCode: function (code) {
                    return $http.get(rootUrl + '/api/Md/NhomVatTu/GetForNvByCode/' + code);
                },
                getNhaCungCapForNvByCode: function (code) {
                    return $http.get(rootUrl + '/api/Md/Supplier/GetForNvByCode/' + code);
                },
                getUnitUsers: function (callback) {
                    $http.get(rootUrl + '/api/Md/UnitUser/GetSelectAll').success(callback);
                },
                getSelectMerchandise: function () {
                    return selectedMerchandise;
                },
                setSelectMerchandise: function (array) {
                    selectedMerchandise = array;
                },
                dowloadTemplateExcel: function (filename) {
                    $http({
                        url: serviceUrl + '/TemplateExcel_KhuyenMaiDongGia',
                        method: "POST",
                        data: null,
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
                }
            };
            return result;
        }
    ]);
    app.controller('kmDongGiaController', [
        '$scope', '$rootScope', '$location', '$window', '$uibModal', '$log', '$state', '$filter', '$http',
        'kmDongGiaService', 'configService', 'ngNotify', 'tempDataService', 'wareHouseService', 'khuyenMaiService',
        function ($scope, $rootScope, $location, $window, $uibModal, $log, $state, $filter, $http,
            kmDongGiaService, configService, ngNotify, tempDataService, serviceWareHouse, khuyenMaiService) {
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.robot = angular.copy(kmDongGiaService.robot);
            $scope.filtered = angular.copy(configService.filterDefault);
            $scope.tempData = tempDataService.tempData;
            $scope.isEditable = true;
            $scope.setPage = function (pageNo) {
                $scope.paged.currentPage = pageNo;
                filterData();
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
            };
            $scope.refresh = function () {
                $scope.setPage($scope.paged.currentPage);
            };
            loadWareHouse();
            $scope.approval = function (target) {
                kmDongGiaService.approval(target, function (response) {
                    if (response) {
                        ngNotify.set("Kích hoạt CT Khuyến mại thành công", { type: 'success' });
                        $scope.refresh();
                    } else {
                        ngNotify.set("Thất bại! - Xảy ra lỗi hoặc phiếu này đã duyệt", { type: 'danger' });
                        $scope.refresh();
                    }
                });
            };


            $scope.unapprove = function (target) {
                kmDongGiaService.unapprove(target, function (response) {
                    if (response) {
                        ngNotify.set("Hủy CT Khuyến mại thành công", { type: 'success' });
                        $scope.refresh();
                    } else {
                        ngNotify.set("Thất bại! - Xảy ra lỗi hoặc phiếu này đã hủy", { type: 'danger' });
                        $scope.refresh();
                    }
                });
            };
            $scope.sortType = 'maChuongTrinh'; // set the default sort type
            $scope.sortReverse = false; // set the default sort order
            $scope.doSearch = function () {
                $scope.paged.currentPage = 1;
                filterData();
            };
            $scope.pageChanged = function () {
                filterData();
            };

            $scope.printDetail = function () {
                var postdata = { paged: $scope.paged, filtered: $scope.filtered };
                khuyenMaiService.setParameterPrint(postdata);
                $state.go("nvPrintDetailKhuyenMai");
            }
            $scope.title = function () {
                return 'Chương trình khuyến mại: Đồng giá';
            };

            $scope.details = function (target) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('/nv/NvKhuyenMai/DongGia', 'details'),
                    controller: 'kmDongGiaDetailsController',
                    windowClass: 'app-modal-window',
                    resolve: {
                        targetData: function () {
                            return target;
                        }
                    }
                });
            };

            $scope.create = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('/nv/NvKhuyenMai/DongGia', 'add'),
                    controller: 'kmDongGiaCreateController',
                    windowClass: 'app-modal-window',
                    resolve: {}
                });

                modalInstance.result.then(function (updatedData) {
                    filterData();
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };
            $scope.update = function (target) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('/nv/NvKhuyenMai/DongGia', 'update'),
                    controller: 'kmDongGiaEditController',
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
            $scope.displayHelper = function (paraValue, moduleName) {
                var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                if (data && data.length === 1) {
                    return data[0].text;
                } else {
                    return paraValue;
                }
            }
            filterData();
            function filterData() {
                $scope.isLoading = true;
                var postdata = {};
                $scope.isLoading = true;
                postdata = { paged: $scope.paged, filtered: $scope.filtered };
                kmDongGiaService.postQuery(postdata).then(function (successRes) {
                    if (successRes && successRes.status === 200 && successRes.data && successRes.data.status) {
                        $scope.isLoading = false;
                        $scope.data = successRes.data.data.data;
                        angular.extend($scope.paged, successRes.data.data);
                    }
                });
            };

        }]);
    app.controller('kmDongGiaDetailsController', [
        '$scope', '$uibModalInstance', '$rootScope', '$location', '$window', '$uibModal', '$log', '$state', '$filter', '$http',
        'kmDongGiaService', 'targetData', 'configService', 'tempDataService',
        function ($scope, $uibModalInstance, $rootScope, $location, $window, $uibModal, $log, $state, $filter, $http,
            kmDongGiaService, targetData, configService, tempDataService) {
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.targetData = angular.copy(targetData);
            $scope.config = angular.copy(configService);
            $scope.target = targetData;
            $scope.tempData = tempDataService.tempData;
            $scope.lstMerchandises = [];
            $scope.lstMerchandiseTypes = [];
            $scope.lstMerchandiseGroups = [];
            $scope.lstSponsors = [];
            $scope.title = function () {
                return 'Chương trình khuyến mại: Đồng giá';
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
            $scope.kmMatHang = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('nv/NvKhuyenMai/DongGia', 'kmMatHang'),
                    controller: 'kmMatHangDetailsController',
                    windowClass: 'app-modal-window',
                    resolve: {
                        initData: function () {
                            return $scope.lstMerchandises;
                        }
                    }
                });
            }
            $scope.kmLoaiHang = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('nv/NvKhuyenMai/DongGia', 'kmLoaiHang'),
                    controller: 'kmLoaiHangEditController',
                    resolve: {
                        initData: function () {
                            return $scope.lstMerchandiseTypes;
                        }
                    }
                });
            }
            $scope.kmNhomHang = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('nv/NvKhuyenMai/DongGia', 'kmNhomHang'),
                    controller: 'kmNhomHangEditController',
                    resolve: {
                        initData: function () {
                            return $scope.lstMerchandiseGroups;
                        }
                    }
                });
            }
            $scope.kmNCC = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('nv/NvKhuyenMai/DongGia', 'kmNCC'),
                    controller: 'kmNCCEditController',
                    resolve: {
                        initData: function () {
                            return $scope.lstSponsors;
                        }
                    }
                });
            }
            function fillterData() {
                $scope.isLoading = true;
                kmDongGiaService.getDetails($scope.target.id).then(function (response) {
                    if (response.status) {
                        $scope.target = response.data.data;
                        $scope.wareHouseCodes = $scope.target.maKhoXuatKhuyenMai;
                        $scope.target.tuNgay = new Date($scope.target.tuNgay);
                        $scope.target.denNgay = new Date($scope.target.denNgay);
                        $scope.lstMerchandises = $scope.target.dataDetails.filter(function (element) {
                            return element.loaiChuongTrinh == 1;
                        });
                        $scope.lstMerchandiseTypes = $scope.target.dataDetails.filter(function (element) {
                            return element.loaiChuongTrinh == 2;
                        });
                        $scope.lstMerchandiseGroups = $scope.target.dataDetails.filter(function (element) {
                            return element.loaiChuongTrinh == 3;
                        });
                        $scope.lstSponsors = $scope.target.dataDetails.filter(function (element) {
                            return element.loaiChuongTrinh == 4;
                        });
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
                if ($scope.target.dataDetails) {
                    $scope.paged.totalItems = $scope.target.dataDetails.length;
                    $scope.data = [];
                    for (var i = (currentPage - 1) * itemsPerPage; i < currentPage * itemsPerPage && i < $scope.target.dataDetails.length; i++) {
                        $scope.data.push($scope.target.dataDetails[i])
                    }
                }
                kmDongGiaService.setSelectMerchandise($scope.data);
            }
            $scope.countItem = function (valueOfTypePromotion) {
                var dataFilter = [];
                switch (valueOfTypePromotion) {
                    case '1':
                        dataFilter = $scope.lstMerchandises;
                        break;
                    case '2':
                        dataFilter = $scope.lstMerchandiseTypes.filter(function (element) {
                            return element.loaiChuongTrinh == '2';
                        });
                        break;
                    case '3':
                        dataFilter = $scope.lstMerchandiseGroups.filter(function (element) {
                            return element.loaiChuongTrinh == '3';
                        });
                        break;
                    case '4':
                        dataFilter = $scope.lstSponsors.filter(function (element) {
                            return element.loaiChuongTrinh == '4';
                        });
                        break;
                    default:

                }
                return dataFilter.length;
            }
            $scope.displayHepler = function (paraValue, moduleName) {
                var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                if (data && data.length === 1) {
                    return data[0].text;
                } else {
                    return paraValue;
                }
            }
        }
    ]);
    app.controller('kmDongGiaEditController', [
        '$scope', '$uibModal', '$uibModalInstance', '$filter', '$state', '$log',
        'kmDongGiaService', 'tempDataService', 'configService', 'targetData', 'wareHouseService', 'ngNotify',
        function ($scope, $uibModal, $uibModalInstance, $filter, $state, $log,
            kmDongGiaService, tempDataService, configService, targetData, serviceWareHouse, ngNotify) {
            $scope.robot = angular.copy(kmDongGiaService.robot);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.config = angular.copy(configService);
            $scope.tempData = tempDataService.tempData;
            $scope.contracts = [];
            $scope.wareHouseCodes = [];
            $scope.target = targetData;

            $scope.cancel = function () {
                $uibModalInstance.close();
            };
            $scope.title = function () {
                return 'Chương trình khuyến mại: Đồng giá';
            };

            $scope.save = function () {

                kmDongGiaService.update($scope.target).then(function (successRes) {
                    if (successRes && successRes.status === 200 && successRes.data) {
                        ngNotify.set("Sửa thành công", { type: 'success' });
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

            //Kho hàng
            $scope.selectWareHouse = function () {
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
                                advanceData: {
                                    unitCode: $scope.target.unitCode,
                                },
                                isAdvance: true
                            }
                        }
                    }
                });
                modalInstance.result.then(function (updatedData) {
                    if (updatedData != null) {
                        var output = '';
                        angular.forEach(updatedData, function (item, index) {
                            output += item.value + ',';
                        });
                        $scope.wareHouseCodes = output.substring(0, output.length - 1);
                        $scope.target.maKhoXuatKhuyenMai = $scope.wareHouseCodes;
                    }
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            }
            //filter by kho
            $scope.changewareHouseCodes = function (inputwareHouse) {
                if (typeof inputwareHouse != 'undefined' && inputwareHouse !== '') {
                    serviceWareHouse.filterWareHouse(inputwareHouse, function (response) {
                        if (response) {
                            $scope.data = response;
                            $scope.wareHouseCodes = '';
                            $scope.wareHouseCodes = $scope.data.maKho;
                        }
                        else {
                            //$scope.selectWareHouse();
                        }
                    });
                }
            }



            function fillterData() {
                $scope.isLoading = true;
                kmDongGiaService.getDetails($scope.target.id).then(function (response) {
                    if (response.status) {
                        $scope.target = response.data.data;
                        $scope.target.tuNgay = new Date($scope.target.tuNgay);
                        $scope.target.denNgay = new Date($scope.target.denNgay);
                        var lstSelectedWareHouse = $scope.target.maKhoXuatKhuyenMai.split(',');
                        angular.forEach(lstSelectedWareHouse, function (item) {
                            var data = $filter('filter')($scope.tempData('wareHouses'), { value: item }, true);
                            if (data && data.length === 1) {
                                $scope.wareHouseCodes.push(data[0]);
                            }
                        });
                        serviceWareHouse.clearSelectData();
                        serviceWareHouse.setSelectData($scope.wareHouseCodes);
                    }
                    $scope.isLoading = false;
                    $scope.pageChanged();
                });
            }
            $scope.removeItem = function (item) {
                var index = $scope.target.dataDetails.indexOf(item);
                $scope.target.dataDetails.splice(index, 1);
                $scope.pageChanged();
            }
            fillterData();
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
                        if ($scope.isSameUnitUser) {
                            $scope.newItem.donGia = updatedData.giaBanLeVat;
                        } else {
                            $scope.newItem.donGia = updatedData.giaBanBuonVat;
                        }
                        $scope.newItem.validateCode = updatedData.maHang;
                    }
                    $scope.pageChanged();
                }, function () {

                });
            };
            $scope.addRow = function () {
                if (!newItem.maHang) {
                    document.getElementById('mahang').focus(); S
                }
                if (!newItem.giaTriKhuyenMai) {
                    document.getElementById('donGiaKM').focus();
                }
                if ($scope.newItem.maHang && $scope.newItem.validateCode == $scope.newItem.maHang) {
                    var exsist = $scope.target.dataDetails.some(function (element, index, array) {
                        return $scope.newItem.maHang == element.maHang;
                    });
                    if (exsist) {
                        ngNotify.set("Mã hàng này bạn đã nhập rồi. Cộng gộp", { type: 'success' });
                    } else {
                        $scope.newItem.loaiChuongTrinh = 2;
                        $scope.target.dataDetails.push($scope.newItem);
                    }
                }
                $scope.pageChanged();
                $scope.newItem = {};
                document.getElementById('mahang').focus();
            };
            $scope.selectedMaHang = function (code) {
                if (code) {
                    kmDongGiaService.getMerchandiseForNvByCode(code, function (response) {
                        if (response.status) {
                            $scope.newItem.maHang = response.data.maHang;
                            $scope.newItem.tenHang = response.data.tenHang;
                            $scope.newItem.donGia = response.data.giaBanLeVat;
                            $scope.newItem.validateCode = response.data.maHang;
                        }
                        else {
                            $scope.addNewItem(code);
                        }
                    });
                }
            }
            $scope.pageChanged = function () {
                var currentPage = $scope.paged.currentPage;
                var itemsPerPage = $scope.paged.itemsPerPage;
                if ($scope.target.dataDetails) {
                    $scope.paged.totalItems = $scope.target.dataDetails.length;
                    $scope.data = [];
                    for (var i = (currentPage - 1) * itemsPerPage; i < currentPage * itemsPerPage && i < $scope.target.dataDetails.length; i++) {
                        $scope.data.push($scope.target.dataDetails[i])
                    }
                }
                kmDongGiaService.setSelectMerchandise($scope.data);
            }
        }
    ]);
    app.controller('kmDongGiaCreateController', [
        '$scope', '$uibModal', '$uibModalInstance', 'ngNotify', '$filter', '$state', '$log',
        'kmDongGiaService', 'tempDataService', 'configService', 'wareHouseService', 'FileUploader', 'userService', 'merchandiseService',
        function ($scope, $uibModal, $uibModalInstance, ngNotify, $filter, $state, $log,
            kmDongGiaService, tempDataService, configService, serviceWareHouse, FileUploader, serviceAuthUser, serviceMerchandise) {
            $scope.robot = angular.copy(kmDongGiaService.robot);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.config = angular.copy(configService);
            var rootUrl = configService.apiServiceBaseUri;
            var currentUser = serviceAuthUser.GetCurrentUser();
            var unitCode = currentUser.unitCode;
            var serviceUrl = rootUrl + '/api/Nv/KhuyenMaiDongGia';
            $scope.tempData = tempDataService.tempData;
            $scope.putTempData = tempDataService.putTempData;
            $scope.contracts = [];
            $scope.lstMerchandises = [];
            $scope.lstMerchandises = kmDongGiaService.getSelectMerchandise();
            $scope.lstMerchandiseTypes = [];
            $scope.lstMerchandiseGroups = [];
            $scope.lstSponsors = [];
            $scope.downloadTemplate = function () {
                kmDongGiaService.dowloadTemplateExcel('TemplateExcel-KhuyenMaiDongGia');
            };
            $scope.target = { dataDetails: [], dataClauseDetails: [] };
            var uploader = $scope.uploader = new FileUploader({
                url: serviceUrl + '/ImportExcelKhuyenMaiDongGia/' + unitCode,
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
            $scope.hangHoaCollection = [];
            uploader.onSuccessItem = function (fileItem, response, status, headers) {
                if (status === 200 && response.data) {
                    if (response.data.length > 0) {
                        for (var i = 0; i < response.data.length; i++) {
                            $scope.newItem = {};
                            $scope.newItem.maHang = response.data[i].maHang;
                            $scope.newItem.tenHang = response.data[i].tenHang;
                            $scope.newItem.giaTriKhuyenMai = response.data[i].giaTriKhuyenMai;
                            $scope.newItem.tyLeKhuyenMai = response.data[i].tyLeKhuyenMai;
                            $scope.hangHoaCollection.push($scope.newItem);
                        }
                        $scope.pageChanged();
                    }
                }
            };
            $scope.statePromotion = {
                buyItemGetItemHasValue: function () {
                    if ($scope.lstMerchandises.length < 1) {
                        return false;
                    }
                    return true;
                },
                kmMerchandiseTypeHasValue: function () {
                    if ($scope.lstMerchandiseTypes.length < 1) {
                        return false;
                    }
                    else {
                        return true;
                    }
                },
                kmMerchandiseGroupHasValue: function () {
                    if ($scope.lstMerchandiseGroups.length < 1) {
                        return false;
                    }
                    else {
                        return true;
                    }
                },
                kmSponsorHasValue: function () {
                    if ($scope.lstSponsors.length < 1) {
                        return false;
                    }
                    else {
                        return true;
                    }
                }
            };
            $scope.cancel = function () {
                $uibModalInstance.close();
            };
            $scope.title = function () {
                return 'Chương trình khuyến mại: Đồng giá';
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
            //Kho hàng
            $scope.selectWareHouse = function () {
                serviceWareHouse.clearSelectData();
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
                                advanceData: {
                                    unitCode: $scope.target.unitCode,
                                },
                                isAdvance: true
                            }
                        }
                    }
                });
                modalInstance.result.then(function (updatedData) {
                    var output = '';
                    angular.forEach(updatedData, function (item, index) {
                        output += item.value + ',';
                    });
                    $scope.wareHouseCodes = output.substring(0, output.length - 1);
                    $scope.target.maKhoXuatKhuyenMai = $scope.wareHouseCodes;
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            }
            //filter by kho
            $scope.changewareHouseCodes = function (inputwareHouse) {
                if (typeof inputwareHouse != 'undefined' && inputwareHouse !== '') {
                    serviceWareHouse.filterWareHouse(inputwareHouse, function (response) {
                        if (response) {
                            $scope.data = response;
                            $scope.wareHouseCodes = '';
                            $scope.wareHouseCodes = $scope.data.maKho;
                        }
                        else {
                            //$scope.selectWareHouse();
                        }
                    });
                }
            }
            $scope.save = function () {
                angular.forEach($scope.lstMerchandises, function (v, k) {
                    $scope.target.dataDetails.push(v);
                });
                kmDongGiaService.post($scope.target).then(function (successRes) {
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
            $scope.selectedMaHang = function (code) {
                if (code) {
                    kmDongGiaService.getMerchandiseForNvByCode(code, function (response) {
                        if (response.status) {
                            $scope.newItem.maHang = response.data.maHang;
                            $scope.newItem.tenHang = response.data.tenHang;
                            $scope.newItem.donGia = response.data.giaBanLeVat;
                            $scope.newItem.validateCode = response.data.maHang;
                        }
                        else {
                            $scope.addNewItem(code);
                        }
                    });
                }
            }
            function filterData() {
                $scope.isLoading = true;
                kmDongGiaService.getNewInstance(function (response) {
                    $scope.target = response;
                    $scope.pageChanged();
                    $scope.isLoading = false;
                })
            };
            $scope.addRow = function () {
                if (!newItem.maHang) {
                    document.getElementById('mahang').focus();
                }
                if (!newItem.giaTriKhuyenMai) {
                    document.getElementById('donGiaKM').focus();
                }
                if ($scope.newItem.maHang && $scope.newItem.validateCode == $scope.newItem.maHang) {
                    var exsist = $scope.target.dataDetails.some(function (element, index, array) {
                        return $scope.newItem.maHang == element.maHang;
                    });
                    if (exsist) {
                        ngNotify.set("Mã hàng này bạn đã nhập rồi. Cộng gộp", { type: 'success' });
                    } else {
                        $scope.newItem.loaiChuongTrinh = 2;
                        $scope.target.dataDetails.push($scope.newItem);
                    }
                }
                $scope.pageChanged();
                $scope.newItem = {};
                document.getElementById('mahang').focus();
            };
            filterData();
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
                        if ($scope.isSameUnitUser) {
                            $scope.newItem.donGia = updatedData.giaBanLeVat;
                        } else {
                            $scope.newItem.donGia = updatedData.giaBanBuonVat;
                        }
                        $scope.newItem.validateCode = updatedData.maHang;
                    }
                    $scope.pageChanged();
                }, function () {

                });
            };
            $scope.removeItem = function (item) {
                var index = $scope.target.dataDetails.indexOf(item);
                $scope.target.dataDetails.splice(index, 1);
                $scope.pageChanged();
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

    return app;
});