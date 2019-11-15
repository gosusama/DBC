define(['ui-bootstrap', '/BTS.SP.MART/controllers/auth/AuthController.js', '/BTS.SP.MART/controllers/htdm/periodController.js', '/BTS.SP.MART/controllers/htdm/merchandiseController.js', '/BTS.SP.MART/controllers/htdm/customerController.js', '/BTS.SP.MART/controllers/htdm/merchandiseTypeController.js', '/BTS.SP.MART/controllers/htdm/nhomVatTuController.js', '/BTS.SP.MART/controllers/htdm/supplierController.js', '/BTS.SP.MART/controllers/htdm/wareHouseController.js', '/BTS.SP.MART/controllers/htdm/packagingController.js', '/BTS.SP.MART/controllers/htdm/taxController.js', '/BTS.SP.MART/controllers/htdm/donViTinhController.js', '/BTS.SP.MART/controllers/htdm/typeReasonController.js', '/BTS.SP.MART/controllers/auth/AuNguoiDung.js', '/BTS.SP.MART/controllers/nv/khuyenMaiController.js'], function () {
    'use strict';
    var app = angular.module('kmVoucherModule', ['ui.bootstrap', 'authModule', 'periodModule', 'merchandiseModule', 'customerModule', 'merchandiseTypeModule', 'nhomVatTuModule', 'supplierModule', 'wareHouseModule', 'packagingModule', 'taxModule', 'donViTinhModule', 'typeReasonModule', 'AuNguoiDungModule', 'khuyenMaiModule']);
    app.factory('kmVoucherService', ['$resource', '$http', '$window', 'configService',
	function ($resource, $http, $window, configService) {
	    var rootUrl = configService.apiServiceBaseUri;
	    var serviceUrl = rootUrl + '/api/Nv/KhuyenMaiVoucher';
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
	                item.donGiaKM = 0;
	            }
	            if (item.tyLeKhuyenMai < 100) {
	                item.donGiaKM = item.donGia - (item.donGia * item.tyLeKhuyenMai / 100);
	            } else {
	                item.donGiaKM = item.donGia - item.tyLeKhuyenMai;
	            }
	        }
	    }
	    var parameterPrint = {};
	    var selectedData = [];

	    function getParameterPrint() {
	        return this.parameterPrint;
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
	        postSelectData: function (data) {
	            return $http.post(serviceUrl + '/PostSelectData', data);
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
	        getDetails: function (id) {
	            return $http.get(serviceUrl + '/GetDetails/' + id);
	        },
	        update: function (params) {
	            return $http.put(serviceUrl + '/' + params.id, params);
	        },
	        approval: function (params) {
	            return $http.post(serviceUrl + '/PostApproval/' + params.id);
	        },
	        unapprove: function (params) {
	            return $http.post(serviceUrl + '/PostUnApprove/' + params.id);
	        },
	        getMerchandiseForNvByCode: function (code) {
	            return $http.get(rootUrl + '/api/Md/Merchandise/GetForNvByCode/' + code);
	        },
	        getMerchandiseTypeForNvByCode: function (code) {
	            return $http.get(rootUrl + '/api/Md/MerchandiseType/GetForNvByCode/' + code);
	        },
	        getMerchandiseGroupForNvByCode: function (code) {
	            return $http.get(rootUrl + '/api/Md/NhomVatTu/GetForNvByCode/' + code);
	        },
	        getNhaCungCapForNvByCode: function (code) {
	            return $http.get(rootUrl + '/api/Md/Customer/GetNhaCungCapForNvByCode/' + code);
	        },
	        getUnitUsers: function () {
	            return $http.get(rootUrl + '/api/Md/UnitUser/GetSelectAll');
	        },
	        getSelectData: function () {
	            return selectedData;
	        },
	        setSelectData: function (array) {
	            selectedData = array;
	        },
	        //lấy dữ liệu mã voucher
	        getDisCountVoucher: function (voucher) {
	            return $http.get(serviceUrl + '/GetDisCountVoucher/' + voucher);
	        },

	        //lấy dữ liệu giao dịch mã voucher đó
	        getGiaoDichByVoucher: function (maGiamGia, callback) {
	            return $http.get(serviceUrl + '/GetGiaoDichByVoucher/' + maGiamGia);
	        }
	    };
	    return result;
	}]);

    app.controller('kmVoucherController', ['$scope', '$rootScope', '$location', '$window', '$uibModal', '$log', '$state', '$filter', '$http',
    'kmVoucherService', 'configService', 'localStorageService', 'wareHouseService', 'tempDataService', 'ngNotify', 'khuyenMaiService',
	function ($scope, $rootScope, $location, $window, $uibModal, $log, $state, $filter, $http,
        kmVoucherService, configService, localStorageService, serviceWareHouse, tempDataService, ngNotify, khuyenMaiService) {
	    $scope.config = angular.copy(configService);
	    $scope.paged = angular.copy(configService.pageDefault);
	    $scope.robot = angular.copy(kmVoucherService.robot);
	    $scope.filtered = angular.copy(configService.filterDefault);
	    $scope.tempData = tempDataService.tempData;
	    $scope.tempData.trangThaiVoucher = tempDataService.tempData('trangThaiVoucher');
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
	    $scope.isEditable = true;
	    $scope.setPage = function (pageNo) {
	        $scope.paged.currentPage = pageNo;
	        filterData();
	    };
	    $scope.idMenu = 'KhuyenMai';
	    $scope.sortType = 'maChuongTrinh'; // set the default sort type
	    $scope.sortReverse = false; // set the default sort order
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
	    $scope.printDetail = function () {
	        var postdata = { paged: $scope.paged, filtered: $scope.filtered };
	        khuyenMaiService.setParameterPrint(postdata);
	        $state.go("nvPrintDetailKhuyenMai");
	    }
	    $scope.title = function () {
	        return 'Chương trình khuyến mại: Voucher, thẻ giảm giá';
	    };
	    $scope.displayHepler = function (paraValue, moduleName) {
	        var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
	        if (data && data.length === 1) {
	            return data[0].text;
	        } else {
	            return paraValue;
	        }
	    }
	    $scope.create = function () {
	        var modalInstance = $uibModal.open({
	            backdrop: 'static',
	            templateUrl: configService.buildUrl('nv/nvKhuyenMai/Voucher', 'add'),
	            controller: 'kmVoucherCreateController',
	            windowClass: 'app-modal-window',
	            resolve: {}
	        });

	        modalInstance.result.then(function (updatedData) {
	            filterData();
	        });
	    };
	    $scope.details = function (target) {
	        var modalInstance = $uibModal.open({
	            backdrop: 'static',
	            templateUrl: configService.buildUrl('nv/nvKhuyenMai/Voucher', 'details'),
	            controller: 'kmVoucherDetailsController',
	            windowClass: 'app-modal-window',
	            resolve: {
	                targetData: function () {
	                    return target;
	                }
	            }
	        });
	        modalInstance.result.then(function (updatedData) {
	            filterData();
	        });
	    };
	    $scope.update = function (target) {
	        var modalInstance = $uibModal.open({
	            backdrop: 'static',
	            templateUrl: configService.buildUrl('nv/nvKhuyenMai/Voucher', 'update'),
	            controller: 'kmVoucherEditController',
	            windowClass: 'app-modal-window',
	            resolve: {
	                targetData: function () {
	                    return target;
	                }
	            }
	        });
	        modalInstance.result.then(function (updatedData) {
	            filterData();
	        });
	    };
	    $scope.approval = function (target) {
	        kmVoucherService.approval(target).then(function (response) {
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
	        kmVoucherService.unapprove(target).then(function (response) {
	            if (response) {
	                ngNotify.set("Hủy CT Khuyến mại thành công", { type: 'success' });
	                filterData();
	            } else {
	                ngNotify.set("Thất bại! - Xảy ra lỗi hoặc phiếu này đã hủy", { type: 'danger' });
	                filterData();
	            }
	        });
	    };
	    $scope.infoUsed = function (target) {
	        var modalInstance = $uibModal.open({
	            backdrop: 'static',
	            templateUrl: configService.buildUrl('nv/nvKhuyenMai/Voucher', 'infoUsed'),
	            controller: 'kmVoucherInfoUsedController',
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
	    }
	    $scope.sum = function () {
	        var total = 0;
	        if ($scope.data) {
	            angular.forEach($scope.data, function (v, k) {
	                total = total + v.thanhTien;
	            });
	        }
	        return total;
	    }

	    filterData();

	    function filterData() {
	        $scope.isLoading = true;
	        var postdata = { paged: $scope.paged, filtered: $scope.filtered };
	        kmVoucherService.postQuery(postdata).then(function (response) {
	            $scope.isLoading = false;
	            console.log('response', response);
	            if (response.status === 200) {
	                $scope.data = response.data.data.data;
	                console.log('$scope.data', $scope.data);
	                angular.extend($scope.paged, response.data.data);
	            }
	        });
	    };
	}
    ]);

    app.controller('kmVoucherCreateController', ['$scope', '$rootScope', '$location', '$window', '$uibModal', '$log', '$state', '$filter', '$http', 'kmVoucherService', 'configService', 'wareHouseService', 'tempDataService', '$uibModalInstance', 'ngNotify',
	function ($scope, $rootScope, $location, $window, $uibModal, $log, $state, $filter, $http,
        kmVoucherService, configService, wareHouseService, tempDataService, $uibModalInstance, ngNotify) {
	    $scope.robot = angular.copy(kmVoucherService.robot);
	    $scope.paged = angular.copy(configService.pageDefault);
	    $scope.config = angular.copy(configService);
	    $scope.tempData = tempDataService.tempData;
	    $scope.tagWareHouses = [];
	    $scope.target = {};
	    $scope.displayHepler = function (paraValue, moduleName) {
	        var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
	        if (data && data.length === 1) {
	            return data[0].text;
	        } else {
	            return paraValue;
	        }
	    };
	    $scope.cancel = function () {
	        $uibModalInstance.close();
	    };
	    $scope.title = function () {
	        return 'Chương trình khuyến mại: Voucher, thẻ giảm giá';
	    };
	    $scope.selectWareHouse = function () {
	        wareHouseService.clearSelectData();
	        var modalInstance = $uibModal.open({
	            backdrop: 'static',
	            templateUrl: configService.buildUrl('htdm/WareHouse', 'selectData'),
	            controller: 'wareHouseSelectDataController',
	            resolve: {
	                serviceSelectData: function () {
	                    return wareHouseService;
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
	            if (updatedData) {
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
	            wareHouseService.filterWareHouse(inputwareHouse, function (response) {
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

	    function filterData() {
	        $scope.isLoading = true;
	        kmVoucherService.getNewInstance().then(function (response) {
	            $scope.target = response.data;
	            $scope.pageChanged();
	            $scope.isLoading = false;
	        });
	    };

	    filterData();

	    $scope.save = function () {
	        $scope.target.maKhoXuatKhuyenMai = $scope.wareHouseCodes;
	        console.log('$scope.target', $scope.target);
	        kmVoucherService.post($scope.target).then(function (response) {
	            if (response.status) {
	                ngNotify.set("Thành công", { type: 'success' });
	                $scope.target.dataDetails = [];
	                $uibModalInstance.close($scope.target);
	            } else {
	                ngNotify.set(response.message, { type: 'danger' });
	            }
	        });
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

    app.controller('kmVoucherEditController', ['$scope', '$rootScope', '$location', '$window', '$uibModal', '$log', '$state', '$filter', '$http', 'kmVoucherService', 'configService', 'wareHouseService', 'tempDataService', '$uibModalInstance', 'ngNotify', 'targetData',
	function ($scope, $rootScope, $location, $window, $uibModal, $log, $state, $filter, $http,
        kmVoucherService, configService, wareHouseService, tempDataService, $uibModalInstance, ngNotify, targetData) {
	    $scope.robot = angular.copy(kmVoucherService.robot);
	    $scope.paged = angular.copy(configService.pageDefault);
	    $scope.config = angular.copy(configService);
	    $scope.tempData = tempDataService.tempData;
	    $scope.wareHouseCodes = [];
	    $scope.target = targetData;
	    $scope.displayHepler = function (paraValue, moduleName) {
	        var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
	        if (data && data.length === 1) {
	            return data[0].text;
	        } else {
	            return paraValue;
	        }
	    };
	    $scope.cancel = function () {
	        $uibModalInstance.close();
	    };
	    $scope.title = function () {
	        return 'Chương trình khuyến mại: Voucher, thẻ giảm giá';
	    };
	    $scope.selectWareHouse = function () {
	        wareHouseService.clearSelectData();
	        var modalInstance = $uibModal.open({
	            backdrop: 'static',
	            templateUrl: configService.buildUrl('htdm/WareHouse', 'selectData'),
	            controller: 'wareHouseSelectDataController',
	            resolve: {
	                serviceSelectData: function () {
	                    return wareHouseService;
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
	            if (updatedData) {
	                var output = '';
	                angular.forEach(updatedData, function (item, index) {
	                    output += item.value + ',';
	                });

	                $scope.target.maKhoXuatKhuyenMai = output.substring(0, output.length - 1);
	                $scope.pageChanged();
	            }
	        }, function () {
	            $log.info('Modal dismissed at: ' + new Date());
	        });
	    }
	    //filter by kho

	    filterData();
	    function filterData() {
	        $scope.isLoading = true;
	        kmVoucherService.getDetails($scope.target.id).then(function (response) {
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
	                wareHouseService.clearSelectData();
	                wareHouseService.setSelectData($scope.wareHouseCodes);
	            }
	            $scope.isLoading = false;
	            $scope.pageChanged();
	        });
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
	    }
	    $scope.save = function () {
	        console.log('$scope.target', $scope.target);
	        kmVoucherService.update($scope.target).then(function (response) {
	            if (response.status) {
	                ngNotify.set("Thành công", { type: 'success' });
	                $scope.target.dataDetails = [];
	                $uibModalInstance.close($scope.target);
	            } else {
	                ngNotify.set(response.message, { type: 'danger' });
	            }
	        });
	    }
	}]);

    app.controller('kmVoucherDetailsController', ['$scope', '$rootScope', '$location', '$window', '$uibModal', '$log', '$state', '$filter', '$http', 'kmVoucherService', 'configService', 'wareHouseService', 'tempDataService', '$uibModalInstance', 'ngNotify', 'targetData',
	function ($scope, $rootScope, $location, $window, $uibModal, $log, $state, $filter, $http,
        kmVoucherService, configService, wareHouseService, tempDataService, $uibModalInstance, ngNotify, targetData) {
	    $scope.robot = angular.copy(kmVoucherService.robot);
	    $scope.paged = angular.copy(configService.pageDefault);
	    $scope.config = angular.copy(configService);
	    $scope.tempData = tempDataService.tempData;
	    $scope.tagWareHouses = [];
	    $scope.target = targetData;
	    $scope.displayHepler = function (paraValue, moduleName) {
	        var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
	        if (data && data.length === 1) {
	            return data[0].text;
	        } else {
	            return paraValue;
	        }
	    };
	    $scope.cancel = function () {
	        $uibModalInstance.close();
	    };
	    $scope.save = function () {
	        $uibModalInstance.close();
	    };
	    $scope.title = function () {
	        return 'Chương trình khuyến mại: Voucher, thẻ giảm giá';
	    };
	    filterData();
	    function filterData() {
	        $scope.isLoading = true;
	        kmVoucherService.getDetails($scope.target.id).then(function (response) {
	            if (response.status) {
	                $scope.target = response.data.data;
	                $scope.target.tuNgay = new Date($scope.target.tuNgay);
	                $scope.target.denNgay = new Date($scope.target.denNgay);
	                $scope.wareHouseCodes = $scope.target.maKhoXuatKhuyenMai;
	            }
	            $scope.isLoading = false;
	        });
	    }
	}]);

    return app;
});