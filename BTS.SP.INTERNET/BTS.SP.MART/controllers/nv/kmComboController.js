define(['ui-bootstrap', '/BTS.SP.MART/controllers/auth/AuthController.js', '/BTS.SP.MART/controllers/htdm/periodController.js', '/BTS.SP.MART/controllers/htdm/merchandiseController.js', '/BTS.SP.MART/controllers/htdm/customerController.js', '/BTS.SP.MART/controllers/htdm/merchandiseTypeController.js', '/BTS.SP.MART/controllers/htdm/nhomVatTuController.js', '/BTS.SP.MART/controllers/htdm/supplierController.js', '/BTS.SP.MART/controllers/htdm/wareHouseController.js', '/BTS.SP.MART/controllers/htdm/packagingController.js', '/BTS.SP.MART/controllers/htdm/taxController.js', '/BTS.SP.MART/controllers/htdm/donViTinhController.js', '/BTS.SP.MART/controllers/htdm/typeReasonController.js', '/BTS.SP.MART/controllers/auth/AuNguoiDung.js', '/BTS.SP.MART/controllers/nv/khuyenMaiController.js'], function () {
    'use strict';
    var app = angular.module('kmComboModule', ['ui.bootstrap', 'authModule', 'periodModule', 'merchandiseModule', 'customerModule', 'merchandiseTypeModule', 'nhomVatTuModule', 'supplierModule', 'wareHouseModule', 'packagingModule', 'taxModule', 'donViTinhModule', 'typeReasonModule', 'AuNguoiDungModule', 'khuyenMaiModule']);
    app.factory('kmComboService', ['$resource', '$http', '$window', 'configService',
		function ($resource, $http, $window, configService) {
		    var rootUrl = configService.apiServiceBaseUri;
		    var serviceUrl = rootUrl + '/api/Nv/KhuyenMaiCombo';
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
		        //lấy dữ liệu mã Combo
		        getDisCountCombo: function (Combo) {
		            return $http.get(serviceUrl + '/GetDisCountCombo/' + Combo);
		        },

		        //lấy dữ liệu giao dịch mã Combo đó
		        getGiaoDichByCombo: function (maGiamGia, callback) {
		            return $http.get(serviceUrl + '/GetGiaoDichByCombo/' + maGiamGia);
		        },
		        dowloadTemplateExcel: function (filename) {
		            $http({
		                url: serviceUrl + '/ImportExcelKhuyenMaiCombo',
		                method: "POST",
		                data: null, //this is your json data string
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
		                // window.URL.revokeObjectURL(objectUrl);
		            }).error(function (data, status, headers, config) {
		                //upload failed
		            });

		            //$http.post(serviceUrl + '/WriteDataToExcel', data).success(callback);
		        }
		    };
		    return result;
		}]);

    app.controller('kmComboController', ['$scope', '$rootScope', '$location', '$window', '$uibModal', '$log', '$state', '$filter', '$http',
		'kmComboService', 'configService', 'localStorageService', 'wareHouseService', 'tempDataService', 'ngNotify', 'khuyenMaiService',
		function ($scope, $rootScope, $location, $window, $uibModal, $log, $state, $filter, $http,
			kmComboService, configService, localStorageService, serviceWareHouse, tempDataService, ngNotify, khuyenMaiService) {
		    $scope.config = angular.copy(configService);
		    $scope.paged = angular.copy(configService.pageDefault);
		    $scope.robot = angular.copy(kmComboService.robot);
		    $scope.filtered = angular.copy(configService.filterDefault);
		    $scope.tempData = tempDataService.tempData;
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
		    $scope.printDetail = function () {
		        var postdata = { paged: $scope.paged, filtered: $scope.filtered };
		        khuyenMaiService.setParameterPrint(postdata);
		        $state.go("nvPrintDetailKhuyenMai");
		    }
		    $scope.title = function () {
		        return 'Chương trình khuyến mại: Combo';
		    };
		    $scope.displayHelper = function (paraValue, moduleName) {
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
		            templateUrl: configService.buildUrl('nv/nvKhuyenMai/Combo', 'add'),
		            controller: 'kmComboCreateController',
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
		            templateUrl: configService.buildUrl('nv/nvKhuyenMai/Combo', 'details'),
		            controller: 'kmComboDetailsController',
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
		            templateUrl: configService.buildUrl('nv/nvKhuyenMai/Combo', 'update'),
		            controller: 'kmComboEditController',
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
		        kmComboService.approval(target).then(function (response) {
		            if (response) {
		                ngNotify.set("Kích hoạt CT Khuyến mại thành công", { type: 'success' });
		                filterData();
		            } else {
		                ngNotify.set("Thất bại! - Xảy ra lỗi hoặc phiếu này đã duyệt", { type: 'danger' });
		                filterData();
		            }
		        });
		    };
		    $scope.unapprove = function (target) {
		        kmComboService.unapprove(target).then(function (response) {
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
		            templateUrl: configService.buildUrl('nv/nvKhuyenMai/Combo', 'infoUsed'),
		            controller: 'kmComboInfoUsedController',
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
		        kmComboService.postQuery(postdata).then(function (response) {
		            $scope.isLoading = false;
		            if (response.status === 200) {
		                $scope.data = response.data.data.data;
		                console.log('$scope.data', $scope.data);
		                angular.extend($scope.paged, response.data.data);
		            }
		        });
		    };
		}
    ]);

    app.controller('kmComboCreateController', ['$scope', '$rootScope', '$location', '$window', '$uibModal', '$log', '$state', '$filter', '$http', 'kmComboService', 'configService', 'wareHouseService', 'tempDataService', '$uibModalInstance', 'ngNotify', 'FileUploader', 'userService',
		function ($scope, $rootScope, $location, $window, $uibModal, $log, $state, $filter, $http,
			kmComboService, configService, wareHouseService, tempDataService, $uibModalInstance, ngNotify, FileUploader, serviceAuthUser) {
		    var rootUrl = configService.apiServiceBaseUri;
		    var currentUser = serviceAuthUser.GetCurrentUser();
		    var unitCode = currentUser.unitCode;
		    var serviceUrl = rootUrl + '/api/Nv/ChuongTrinhKhuyenMai';
		    $scope.robot = angular.copy(kmComboService.robot);
		    $scope.paged = angular.copy(configService.pageDefault);
		    $scope.pagedGift = angular.copy(configService.pageDefault);
		    $scope.config = angular.copy(configService);
		    $scope.tempData = tempDataService.tempData;
		    $scope.tagWareHouses = [];
		    $scope.target = { dataDetails: [], dataGifts: [] };
		    $scope.displayHelper = function (paraValue, moduleName) {
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
		        return 'Chương trình khuyến mại: Combo';
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
		        kmComboService.getNewInstance().then(function (response) {
		            $scope.target = response.data;
		            $scope.pageChanged();
		            $scope.isLoading = false;
		        });
		    };

		    filterData();

		    $scope.save = function () {
		        $scope.target.maKhoXuatKhuyenMai = $scope.wareHouseCodes;
		        $scope.target.dataDetails = $scope.data;
		        kmComboService.post($scope.target).then(function (response) {
		            if (response.status) {
		                ngNotify.set("Thành công", { type: 'success' });
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
		    $scope.downloadTemplate = function () {
		        kmComboService.dowloadTemplateExcel('TemplateExcel-KhuyenMaiCombo');
		    };
		    var uploader = $scope.uploader = new FileUploader({
		        url: serviceUrl + '/ImportExcelKhuyenMaiCombo/' + unitCode
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
		        if (status === 200 && response.data) {
		            console.log('response kmCombo:', response);
		            if (response.data.length > 0) {
		                for (var i = 0; i < response.data.length; i++) {
		                    $scope.newItem = {};
		                    $scope.newItemGift = {};
		                    //hang mua
		                    $scope.newItem.maHang = response.data[i].maHang;
		                    $scope.newItem.tenHang = response.data[i].tenHang;
		                    //hang tang combo
		                    $scope.newItemGift.maHang = response.data[i].maHangKhuyenMai;
		                    $scope.newItemGift.tenHang = response.data[i].tenHangKhuyenMai;
		                    //push
		                    $scope.target.dataDetails.push($scope.newItem);
		                    $scope.target.dataGifts.push($scope.newItemGift);
		                }
		                $scope.pageChanged();
		                $scope.pageChangedGift();
		            }
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
		                    return;
		                },
		                filterObject: function () {
		                    return {
		                        summary: strKey
		                    };
		                }
		            }
		        });
		        modalInstance.result.then(function (updatedData) {
		            $scope.newItem.maHang = updatedData.maHang
		            $scope.newItem.tenHang = updatedData.tenHang;
		            $scope.newItem.donGia = updatedData.giaBanLeVat;
		            $scope.newItem.validateCode = updatedData.maHang;
		            console.log($scope.newItem);
		        }, function () {

		        });
		    }
		    $scope.removeItem = function (item) {
		        var index = $scope.data.indexOf(item);
		        $scope.data.splice(index, 1);
		        $scope.target.dataDetails = $scope.data;
		        $scope.pageChanged();
		    }
		    $scope.selectedMaHang = function (code) {
		        if (code) {
		            kmComboService.getMerchandiseForNvByCode(code).then(function (response) {
		                if (response.status === 200 && response.data.data != null) {
		                    $scope.newItem.maHang = response.data.data.maHang;
		                    $scope.newItem.tenHang = response.data.data.tenHang;
		                    $scope.newItem.donGia = response.data.data.giaBanLeVat;
		                    $scope.newItem.validateCode = response.data.data.maHang;
		                }
		                else {
		                    $scope.addNewItem(code);

		                }
		            });
		        }
		    }
		    $scope.addNewItemGift = function (strKey) {
		        var modalInstance = $uibModal.open({
		            backdrop: 'static',
		            templateUrl: configService.buildUrl('htdm/Merchandise', 'selectData'),
		            controller: 'merchandiseSelectDataController',
		            windowClass: 'app-modal-window',
		            resolve: {
		                serviceSelectData: function () {
		                    return;
		                },
		                filterObject: function () {
		                    return {
		                        summary: strKey
		                    };
		                }
		            }
		        });
		        modalInstance.result.then(function (updatedData) {
		            $scope.newItemGift.maHang = updatedData.maHang
		            $scope.newItemGift.tenHang = updatedData.tenHang;
		            $scope.newItemGift.donGia = updatedData.giaBanLeVat;
		            $scope.newItemGift.validateCode = updatedData.maHang;
		            console.log($scope.newItemGift);
		        }, function () {

		        });
		    }
		    $scope.selectedMaHangGift = function (code) {
		        if (code) {
		            kmComboService.getMerchandiseForNvByCode(code).then(function (response) {
		                console.log('response', response);
		                if (response.status && response.data.data != null) {
		                    $scope.newItemGift.maHang = response.data.data.maHang;
		                    $scope.newItemGift.tenHang = response.data.data.tenHang;
		                    $scope.newItemGift.donGia = response.data.data.giaBanLeVat;
		                    $scope.newItemGift.validateCode = response.data.data.maHang;
		                    console.log($scope.newItemGift);
		                }
		                else {
		                    $scope.addNewItemGift(code);
		                }
		            });
		        }
		    }

		    $scope.addRowGift = function () {
		        if (!$scope.newItemGift.maHang) {
		            document.getElementById('mahangGift').focus();
		            return;
		        }
		        if (!$scope.newItemGift.tenHang) {
		            document.getElementById('addG').focus();
		            return;
		        }
		        if ($scope.newItemGift.validateCode == $scope.newItemGift.maHang) {
		            var exsist = $scope.target.dataGifts.some(function (element, index, array) {
		                return $scope.newItemGift.maHang == element.maHang;
		            });
		            if (exsist) {
		                ngNotify.set("Mã hàng này bạn đã nhập rồi. Cộng gộp", { type: 'success' });
		            } else {
		                $scope.target.dataGifts.push($scope.newItemGift);
		            }
		        } else {
		            ngNotify.set("Mã hàng chưa đúng!", { type: 'danger' });
		        }
		        $scope.pageChangedGift();
		        $scope.newItemGift = {};
		        document.getElementById('mahangGift').focus();
		    };
		    $scope.removeItemGift = function (item) {
		        var index = $scope.target.dataGifts.indexOf(item);
		        $scope.target.dataGifts.splice(index, 1);
		        $scope.pageChangedGift();
		    }
		    $scope.pageChangedGift = function () {
		        var currentPage = $scope.pagedGift.currentPage;
		        var itemsPerPage = $scope.pagedGift.itemsPerPage;
		        if ($scope.target.dataGifts) {
		            $scope.pagedGift.totalItems = $scope.target.dataGifts.length;
		            $scope.dataGift = [];
		            for (var i = (currentPage - 1) * itemsPerPage; i < currentPage * itemsPerPage && i < $scope.target.dataGifts.length; i++) {
		                $scope.dataGift.push($scope.target.dataGifts[i])
		            }
		        }
		    }
		    $scope.data = [];
		    $scope.addRow = function () {
		        if (!$scope.newItem.tenHang) {
		            document.getElementById('add').focus();
		            return;
		        }
		        if ($scope.newItem.maHang && $scope.newItem.validateCode == $scope.newItem.maHang) {
		            var exsist = $scope.target.dataDetails.some(function (element, index, array) {
		                return $scope.newItem.maHang == element.maHang;
		            });
		            if (exsist) {
		                ngNotify.set("Mã hàng này bạn đã nhập rồi. Cộng gộp", { type: 'success' });

		            } else {
		                $scope.newItem.loaiChuongTrinh = 1;
		                $scope.target.dataDetails.push($scope.newItem);
		            }
		        }
		        $scope.pageChanged();
		        $scope.newItem = {};
		        document.getElementById('mahang').focus();
		    };
		}
    ]);

    app.controller('kmComboEditController', ['$scope', '$rootScope', '$location', '$window', '$uibModal', '$log', '$state', '$filter', '$http', 'kmComboService', 'configService', 'wareHouseService', 'tempDataService', '$uibModalInstance', 'ngNotify', 'targetData',
		function ($scope, $rootScope, $location, $window, $uibModal, $log, $state, $filter, $http,
			kmComboService, configService, wareHouseService, tempDataService, $uibModalInstance, ngNotify, targetData) {
		    $scope.robot = angular.copy(kmComboService.robot);
		    $scope.paged = angular.copy(configService.pageDefault);
		    $scope.pagedGift = angular.copy(configService.pageDefault);
		    $scope.config = angular.copy(configService);
		    $scope.tempData = tempDataService.tempData;
		    $scope.wareHouseCodes = [];
		    $scope.target = { dataDetails: [], dataGifts: [] };
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
		        return 'Chương trình khuyến mại: Combo';
		    };
		    $scope.selectWareHouse = function () {
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
		    $scope.wareHouseCodes = [];
		    filterData();
		    function filterData() {
		        $scope.isLoading = true;
		        $scope.target.dataDetails = [];
		        kmComboService.getDetails($scope.target.id).then(function (response) {
		            if (response.status) {
		                $scope.target = response.data.data;
		                $scope.target.tuNgay = new Date($scope.target.tuNgay);
		                $scope.target.denNgay = new Date($scope.target.denNgay);
		                if ($scope.target.maKhoXuatKhuyenMai) {
		                    var lstSelectedWareHouse = $scope.target.maKhoXuatKhuyenMai.split(',');
		                    angular.forEach(lstSelectedWareHouse, function (item) {
		                        var data = $filter('filter')($scope.tempData('wareHouses'), { value: item }, true);
		                        if (data && data.length === 1) {
		                            $scope.wareHouseCodes.push(data[0]);
		                        }
		                    });
		                }
		                wareHouseService.clearSelectData();
		                wareHouseService.setSelectData($scope.wareHouseCodes);
		                $scope.data = $scope.target.dataDetails;
		            }
		            $scope.isLoading = false;
		            $scope.pageChanged();
		            $scope.pageChangedGift();
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
		        $scope.target.dataDetails = $scope.data;
		        kmComboService.update($scope.target).then(function (response) {
		            if (response.status) {
		                ngNotify.set("Thành công", { type: 'success' });
		                $uibModalInstance.close($scope.target);
		            } else {
		                ngNotify.set(response.message, { type: 'danger' });
		            }
		        });
		    }
		    $scope.removeItem = function (item) {
		        var index = $scope.data.indexOf(item);
		        $scope.data.splice(index, 1);
		        $scope.target.dataDetails = $scope.data;
		        $scope.pageChanged();

		    }
		    $scope.addNewItem = function (strKey) {
		        var modalInstance = $uibModal.open({
		            backdrop: 'static',
		            templateUrl: configService.buildUrl('htdm/Merchandise', 'selectData'),
		            controller: 'merchandiseSelectDataController',
		            windowClass: 'app-modal-window',
		            resolve: {
		                serviceSelectData: function () {
		                    return;
		                },
		                filterObject: function () {
		                    return {
		                        summary: strKey
		                    };
		                }
		            }
		        });
		        modalInstance.result.then(function (updatedData) {
		            $scope.newItem.maHang = updatedData.maHang
		            $scope.newItem.tenHang = updatedData.tenHang;
		            $scope.newItem.donGia = updatedData.giaBanLeVat;
		            $scope.newItem.validateCode = updatedData.maHang;
		            console.log($scope.newItem);
		        }, function () {

		        });
		    }
		    $scope.selectedMaHang = function (code) {
		        if (code) {
		            kmComboService.getMerchandiseForNvByCode(code).then(function (response) {
		                if (response.status === 200 && response.data.data != null) {
		                    $scope.newItem.maHang = response.data.data.maHang;
		                    $scope.newItem.tenHang = response.data.data.tenHang;
		                    $scope.newItem.donGia = response.data.data.giaBanLeVat;
		                    $scope.newItem.validateCode = response.data.data.maHang;
		                }
		                else {
		                    $scope.addNewItem(code);

		                }
		            });
		        }
		    }
		    $scope.addNewItemGift = function (strKey) {
		        var modalInstance = $uibModal.open({
		            backdrop: 'static',
		            templateUrl: configService.buildUrl('htdm/Merchandise', 'selectData'),
		            controller: 'merchandiseSelectDataController',
		            windowClass: 'app-modal-window',
		            resolve: {
		                serviceSelectData: function () {
		                    return;
		                },
		                filterObject: function () {
		                    return {
		                        summary: strKey
		                    };
		                }
		            }
		        });
		        modalInstance.result.then(function (updatedData) {
		            $scope.newItemGift.maHang = updatedData.maHang
		            $scope.newItemGift.tenHang = updatedData.tenHang;
		            $scope.newItemGift.donGia = updatedData.giaBanLeVat;
		            $scope.newItemGift.validateCode = updatedData.maHang;
		            console.log($scope.newItemGift);
		        }, function () {

		        });
		    }
		    $scope.selectedMaHangGift = function (code) {
		        if (code) {
		            kmComboService.getMerchandiseForNvByCode(code).then(function (response) {
		                console.log('response', response);
		                if (response.status && response.data.data != null) {
		                    $scope.newItemGift.maHang = response.data.data.maHang;
		                    $scope.newItemGift.tenHang = response.data.data.tenHang;
		                    $scope.newItemGift.donGia = response.data.data.giaBanLeVat;
		                    $scope.newItemGift.validateCode = response.data.data.maHang;
		                    console.log($scope.newItemGift);
		                }
		                else {
		                    $scope.addNewItemGift(code);
		                }
		            });
		        }
		    }

		    $scope.addRowGift = function () {
		        if (!$scope.newItemGift.maHang) {
		            document.getElementById('mahangGift').focus();
		            return;
		        }
		        if (!$scope.newItemGift.tenHang) {
		            document.getElementById('addG').focus();
		            return;
		        }
		        if ($scope.newItemGift.validateCode == $scope.newItemGift.maHang) {
		            var exsist = $scope.target.dataGifts.some(function (element, index, array) {
		                return $scope.newItemGift.maHang == element.maHang;
		            });
		            if (exsist) {
		                ngNotify.set("Mã hàng này bạn đã nhập rồi. Cộng gộp", { type: 'success' });
		            } else {
		                $scope.target.dataGifts.push($scope.newItemGift);
		            }
		        } else {
		            ngNotify.set("Mã hàng chưa đúng!", { type: 'danger' });
		        }
		        $scope.pageChangedGift();
		        $scope.newItemGift = {};
		        document.getElementById('mahangGift').focus();
		    };
		    $scope.removeItemGift = function (item) {
		        var index = $scope.target.dataGifts.indexOf(item);
		        $scope.target.dataGifts.splice(index, 1);
		        $scope.pageChangedGift();
		    }
		    $scope.pageChangedGift = function () {
		        var currentPage = $scope.pagedGift.currentPage;
		        var itemsPerPage = $scope.pagedGift.itemsPerPage;
		        if ($scope.target.dataGifts) {
		            $scope.pagedGift.totalItems = $scope.target.dataGifts.length;
		            $scope.dataGift = [];
		            for (var i = (currentPage - 1) * itemsPerPage; i < currentPage * itemsPerPage && i < $scope.target.dataGifts.length; i++) {
		                $scope.dataGift.push($scope.target.dataGifts[i])
		            }
		        }
		    }
		    $scope.data = [];
		    $scope.addRow = function () {
		        if (!$scope.newItem.tenHang) {
		            document.getElementById('add').focus();
		            return;
		        }
		        if ($scope.newItem.maHang && $scope.newItem.validateCode == $scope.newItem.maHang) {
		            var exsist = $scope.target.dataDetails.some(function (element, index, array) {
		                return $scope.newItem.maHang == element.maHang;
		            });
		            if (exsist) {
		                ngNotify.set("Mã hàng này bạn đã nhập rồi. Cộng gộp", { type: 'success' });

		            } else {
		                $scope.newItem.loaiChuongTrinh = 1;
		                $scope.target.dataDetails.push($scope.newItem);
		            }
		        }
		        $scope.pageChanged();
		        $scope.newItem = {};
		        document.getElementById('mahang').focus();
		    };
		}]);

    app.controller('kmComboDetailsController', ['$scope', '$rootScope', '$location', '$window', '$uibModal', '$log', '$state', '$filter', '$http', 'kmComboService', 'configService', 'wareHouseService', 'tempDataService', '$uibModalInstance', 'ngNotify', 'targetData',
		function ($scope, $rootScope, $location, $window, $uibModal, $log, $state, $filter, $http,
			kmComboService, configService, wareHouseService, tempDataService, $uibModalInstance, ngNotify, targetData) {
		    $scope.robot = angular.copy(kmComboService.robot);
		    $scope.paged = angular.copy(configService.pageDefault);
		    $scope.pagedGift = angular.copy(configService.pageDefault);
		    $scope.config = angular.copy(configService);
		    $scope.tempData = tempDataService.tempData;
		    $scope.wareHouseCodes = [];
		    $scope.target = { dataDetails: [], dataGifts: [] };
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
		        return 'Chương trình khuyến mại: Combo';
		    };

		    filterData();
		    function filterData() {
		        $scope.isLoading = true;
		        $scope.target.dataDetails = [];
		        kmComboService.getDetails($scope.target.id).then(function (response) {
		            if (response.status) {
		                $scope.target = response.data.data;
		                $scope.target.tuNgay = new Date($scope.target.tuNgay);
		                $scope.target.denNgay = new Date($scope.target.denNgay);

		            }
		            $scope.isLoading = false;
		            $scope.pageChanged();
		            $scope.pageChangedGift();
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
		        kmComboService.update($scope.target).then(function (response) {
		            if (response.status) {
		                ngNotify.set("Thành công", { type: 'success' });
		                $uibModalInstance.close($scope.target);
		            } else {
		                ngNotify.set(response.message, { type: 'danger' });
		            }
		        });
		    }
		    $scope.pageChangedGift = function () {
		        var currentPage = $scope.pagedGift.currentPage;
		        var itemsPerPage = $scope.pagedGift.itemsPerPage;
		        if ($scope.target.dataGifts) {
		            $scope.pagedGift.totalItems = $scope.target.dataGifts.length;
		            $scope.dataGift = [];
		            for (var i = (currentPage - 1) * itemsPerPage; i < currentPage * itemsPerPage && i < $scope.target.dataGifts.length; i++) {
		                $scope.dataGift.push($scope.target.dataGifts[i])
		            }
		        }
		    }
		}]);

    return app;
});