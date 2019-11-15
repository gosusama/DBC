define(['ui-bootstrap', '/BTS.SP.MART/controllers/auth/AuthController.js', '/BTS.SP.MART/controllers/htdm/periodController.js', '/BTS.SP.MART/controllers/htdm/merchandiseController.js', '/BTS.SP.MART/controllers/htdm/customerController.js', '/BTS.SP.MART/controllers/htdm/merchandiseTypeController.js', '/BTS.SP.MART/controllers/htdm/nhomVatTuController.js', '/BTS.SP.MART/controllers/htdm/supplierController.js', '/BTS.SP.MART/controllers/htdm/wareHouseController.js', '/BTS.SP.MART/controllers/htdm/packagingController.js', '/BTS.SP.MART/controllers/htdm/taxController.js', '/BTS.SP.MART/controllers/htdm/donViTinhController.js', '/BTS.SP.MART/controllers/htdm/typeReasonController.js', '/BTS.SP.MART/controllers/auth/AuNguoiDung.js'], function () {
    'use strict';

    var app = angular.module('khuyenMaiModule', ['ui.bootstrap', 'authModule', 'periodModule', 'merchandiseModule', 'customerModule', 'merchandiseTypeModule', 'nhomVatTuModule', 'supplierModule', 'wareHouseModule', 'packagingModule', 'taxModule', 'donViTinhModule', 'typeReasonModule', 'AuNguoiDungModule']);
    app.factory('khuyenMaiService', ['$http', 'configService',
    function ( $http, configService) {
        var rootUrl = configService.apiServiceBaseUri;
        var serviceUrl = rootUrl + '/api/Nv/ChuongTrinhKhuyenMai';
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
            deleteItem: function (id) {
                return $http.delete(serviceUrl + '/' + id);
            },
            approval: function (params, callback) {
                return $http.post(serviceUrl + '/PostApproval/' + params.id).success(callback);
            },
            unapprove: function (params, callback) {
                return $http.post(serviceUrl + '/PostUnApprove/' + params.id).success(callback);
            },
            getMerchandiseForNvByCode: function (code, wareHouseCode, unitCode) {
                return $http.get(rootUrl + '/api/Md/Merchandise/GetForNvByCode/' + code + '/' + wareHouseCode + '/' + unitCode);
            },
            getMerchandiseTypeForNvByCode: function (code) {
                return $http.get(rootUrl + '/api/Md/MerchandiseType/GetForNvByCode/' + code);
            },
            getMerchandiseGroupForNvByCode: function (code) {
                return $http.get(rootUrl + '/api/Md/NhomVatTu/GetForNvByCode/' + code);
            },
            getNhaCungCapForNvByCode: function (code) {
                return $http.get(rootUrl + '/api/Md/Supplier/GetNhaCungCapForNvByCode/' + code);
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
            resetMerchandise: function () {
                selectedMerchandise = [];
            },
            dowloadTemplateExcel: function (filename) {
                $http({
                    url: serviceUrl + '/TemplateExcel_CK_HangHoa',
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
    }
    ]);
    app.controller('khuyenMaiController', [
        '$scope', '$uibModal', '$log', '$state', '$filter', 'khuyenMaiService', 'configService', 'localStorageService', 'ngNotify', 'tempDataService', 'wareHouseService',
        function ($scope, $uibModal, $log, $state, $filter, khuyenMaiService, configService, localStorageService, ngNotify, tempDataService, wareHouseService) {
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.robot = angular.copy(khuyenMaiService.robot);
            $scope.filtered = angular.copy(configService.filterDefault);
            $scope.tempData = tempDataService.tempData;
            $scope.isEditable = true;
            function filterData() {
                $scope.isLoading = true;
                var postdata = {};
                $scope.isLoading = true;
                postdata = { paged: $scope.paged, filtered: $scope.filtered };
                khuyenMaiService.postQuery(postdata).then(function (successRes) {
                    if (successRes && successRes.status === 200 && successRes.data) {
                        $scope.isLoading = false;
                        $scope.data = successRes.data.data.data;
                        angular.extend($scope.paged, successRes.data.data);
                    }
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
            $scope.setPage = function (pageNo) {
                $scope.paged.currentPage = pageNo;
                filterData();
            };
            function loadWareHouse() {
                if (!tempDataService.tempData('wareHouses')) {
                    wareHouseService.getAll_WareHouse().then(function (successRes) {
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
                khuyenMaiService.setParameterPrint(
                    postdata);
                $state.go("nvPrintDetailKhuyenMai");
            }
            $scope.title = function () {
                return 'Chương trình khuyến mại: Chiết khấu';
            };

            $scope.isAdd = localStorageService.get('localStorageIsAdd');

            $scope.details = function (target) {
                khuyenMaiService.resetMerchandise();
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('/nv/NvKhuyenMai', 'details'),
                    controller: 'khuyenMaiDetailsController',
                    windowClass: 'app-modal-window',
                    resolve: {
                        targetData: function () {
                            return target;
                        }
                    }
                });
            };
            $scope.create = function () {
                khuyenMaiService.resetMerchandise();
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('/nv/NvKhuyenMai', 'add'),
                    controller: 'khuyenMaiCreateController',
                    windowClass: 'app-modal-window',
                    resolve: {}
                });
                modalInstance.result.then(function (updatedData) {
                    $scope.refresh();
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };
            $scope.update = function (target) {
                khuyenMaiService.resetMerchandise();
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('/nv/nvKhuyenMai', 'update'),
                    controller: 'khuyenMaiEditController',
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

            $scope.delete = function (item) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('/nv/nvKhuyenMai', 'delete'),
                    controller: 'khuyenMaiDeleteController',
                    windowClass: 'app-modal-window',
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

            $scope.approval = function (target) {
                khuyenMaiService.approval(target, function (response) {
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
                khuyenMaiService.unapprove(target, function (response) {
                    if (response) {
                        ngNotify.set("Hủy CT Khuyến mại thành công", { type: 'success' });
                        $scope.refresh();
                    } else {
                        ngNotify.set("Thất bại! - Xảy ra lỗi hoặc phiếu này đã hủy", { type: 'danger' });
                        $scope.refresh();
                    }
                });
            };
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

        }]);
    app.controller('khuyenMaiDetailsController', [
    '$scope', '$uibModalInstance', '$uibModal', '$filter', 'khuyenMaiService', 'targetData', 'configService', 'tempDataService',
    function ($scope, $uibModalInstance, $uibModal, $filter, khuyenMaiService, targetData, configService, tempDataService) {
        $scope.paged = angular.copy(configService.pageDefault);
        $scope.config = angular.copy(configService);
        $scope.target = targetData;
        $scope.tempData = tempDataService.tempData;
        $scope.lstMerchandises = [];
        $scope.lstMerchandiseTypes = [];
        $scope.lstMerchandiseGroups = [];
        $scope.lstSponsors = [];
        $scope.title = function () {
            return 'Chương trình khuyến mại: Chiết khấu';
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
                templateUrl: configService.buildUrl('nv/nvKhuyenMai', 'kmMatHang'),
                controller: 'kmMatHangDetailsController',
                size: 'lg',
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
                templateUrl: configService.buildUrl('nv/nvKhuyenMai', 'kmLoaiHang'),
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
                templateUrl: configService.buildUrl('nv/nvKhuyenMai', 'kmNhomHang'),
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
                templateUrl: configService.buildUrl('nv/nvKhuyenMai', 'kmNCC'),
                controller: 'kmNCCEditController',
                resolve: {
                    initData: function () {
                        return $scope.lstSponsors;
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
        }
        function fillterData() {
            $scope.isLoading = true;
            khuyenMaiService.getDetails($scope.target.id).then(function (response) {
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
            khuyenMaiService.setSelectMerchandise($scope.data);
        }
        $scope.countItem = function (valueOfTypePromotion) {
            var dataFilter = [];
            if ($scope.lstMerchandises) {
                switch (valueOfTypePromotion) {
                    case '1':
                        dataFilter = $scope.lstMerchandises.filter(function (element) {
                            return element.loaiChuongTrinh == '1';
                        });
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
                        break;
                }
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
    app.controller('printDetailKhuyenMaiController', [
        '$scope', '$state', '$window', '$stateParams', '$timeout', '$filter',
        'tempDataService', 'khuyenMaiService', 'configService',
        function ($scope, $state, $window, $stateParams, $timeout, $filter,
            tempDataService, khuyenMaiService, configService) {
            $scope.tempData = tempDataService.tempData;
            $scope.robot = angular.copy(khuyenMaiService.robot);
            $scope.displayHepler = function (code, module) {
                var data = $filter('filter')($scope.tempData(module), { value: code }, true);
                if (data && data.length == 1) {
                    return data[0].text;
                };
                return "Empty!";
            }
            $scope.info = khuyenMaiService.getParameterPrint().filtered.advanceData;
            $scope.goIndex = function () {
                $state.go("nvKhuyenMai");
            }

            function filterData() {
                khuyenMaiService.postPrintDetail(
                    function (response) {
                        $scope.printData = response;
                    });
            }
            $scope.printExcel = function () {
                var data = [document.getElementById('dataTable').innerHTML];
                var fileName = "KhuyenMai_ExportData.xls";
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
            $scope.print = function () {
                var table = document.getElementById('dataTable').innerHTML;
                var myWindow = $window.open('', '', 'width=800, height=600');
                myWindow.document.write(table);
                myWindow.print();
            }
            filterData();
        }
    ]);
    app.controller('khuyenMaiEditController', [
        '$scope', '$uibModal', '$uibModalInstance', '$filter', '$state', '$log',
         'khuyenMaiService', 'tempDataService', 'configService', 'targetData', 'wareHouseService', 'ngNotify',
        function ($scope, $uibModal, $uibModalInstance, $filter, $state, $log,
             khuyenMaiService, tempDataService, configService, targetData, wareHouseService, ngNotify) {
            $scope.robot = angular.copy(khuyenMaiService.robot);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.config = angular.copy(configService);
            $scope.tempData = tempDataService.tempData;
            $scope.contracts = [];
            $scope.lstMerchandises = [];
            $scope.tagWareHouses = [];
            $scope.lstMerchandiseTypes = [];
            $scope.lstMerchandiseGroups = [];
            $scope.wareHouseCodes = [];
            $scope.lstSponsors = [];
            $scope.target = targetData;
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
                return 'Chương trình khuyến mại: Chiết khấu';
            };
            $scope.save = function () {
                $scope.target.dataDetails = [];
                angular.forEach($scope.lstMerchandises, function (v, k) {
                    $scope.target.dataDetails.push(v);
                });
                angular.forEach($scope.lstMerchandiseTypes, function (v, k) {
                    $scope.target.dataDetails.push(v);
                });
                angular.forEach($scope.lstMerchandiseGroups, function (v, k) {
                    $scope.target.dataDetails.push(v);
                });
                angular.forEach($scope.lstSponsors, function (v, k) {
                    $scope.target.dataDetails.push(v);
                });
                khuyenMaiService.update($scope.target).then(function (successRes) {
                    if (successRes && successRes.status === 200 && successRes.data) {
                        ngNotify.set("Sửa thành công", { type: 'success' });
                        $uibModalInstance.close($scope.target);
                    } else {
                        console.log('update successRes', successRes);
                        ngNotify.set('Cập nhật thất bại !', { duration: 3000, type: 'error' });
                        $uibModalInstance.close();
                    }
                },
                    function (response) {
                        console.log('ERROR: Update failed! ' + response);
                        ngNotify.set('Cập nhật thất bại !', { duration: 3000, type: 'error' });
                        $uibModalInstance.close();
                    }
                );
            };
            $scope.createWareHouse = function (target, name) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/WareHouse', 'add'),
                    controller: 'wareHouseCreateController',
                    resolve: {
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
                $scope.tagWareHouses = $scope.target.maKhoXuatKhuyenMai.split(',');
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
                    }
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            }
            
            $scope.saveAndKeep = function () {
                angular.forEach($scope.lstMerchandises, function (v, k) {
                    $scope.target.dataDetails.push(v);
                });
                angular.forEach($scope.lstMerchandiseTypes, function (v, k) {
                    $scope.target.dataDetails.push(v);
                });
                angular.forEach($scope.lstMerchandiseGroups, function (v, k) {
                    $scope.target.dataDetails.push(v);
                });
                angular.forEach($scope.lstSponsors, function (v, k) {
                    $scope.target.dataDetails.push(v);
                });
                var tempData = angular.copy($scope.target);
                khuyenMaiService.post(
                    JSON.stringify($scope.target), function (response) {
                        if (response.status) {

                            ngNotify.set("Thành công", { type: 'danger' });
                            khuyenMaiService.getNewInstance(function (response1) {
                                tempData.soPhieu = expectData.soPhieu;
                                tempData.ngay = expectData.ngay;
                                $scope.target = tempData;
                            })
                        } else {
                            ngNotify.set(response.message, { type: 'danger' });
                        }
                    }
                    );
            };
            $scope.kmMatHang = function () {
                if (!$scope.statePromotion.buyItemGetItemHasValue()) {
                    var modalInstance = $uibModal.open({
                        backdrop: 'static',
                        templateUrl: configService.buildUrl('nv/nvKhuyenMai', 'kmMatHang'),
                        controller: 'kmMatHangCreateController',
                        size: 'lg',
                        resolve: {
                            initService: function () {
                                return $scope.lstMerchandises;
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
                        $scope.lstMerchandises = [];
                        if (updatedData && updatedData.length > 0) {
                            angular.forEach(updatedData, function (v, k) {
                                $scope.lstMerchandises.push(v);
                            })
                        } else {
                        }
                    }, function () {
                        $log.info('Modal dismissed at: ' + new Date());

                    });
                } else {
                    var modalInstance = $uibModal.open({
                        backdrop: 'static',
                        templateUrl: configService.buildUrl('nv/nvKhuyenMai', 'kmMatHang'),
                        controller: 'kmMatHangEditController',
                        size: 'lg',
                        resolve: {
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
                        $scope.lstMerchandises = [];
                        if (updatedData) {
                            angular.forEach(updatedData, function (v, k) {
                                $scope.lstMerchandises.push(v);
                            })
                        } else {

                        }
                        $scope.pageChanged();
                    }, function () {
                        $log.info('Modal dismissed at: ' + new Date());
                    });
                }

            }
            $scope.kmLoaiHang = function () {
                if (!$scope.statePromotion.kmMerchandiseTypeHasValue()) {

                    var modalInstance = $uibModal.open({
                        backdrop: 'static',
                        templateUrl: configService.buildUrl('nv/nvKhuyenMai', 'kmLoaiHang'),
                        controller: 'kmLoaiHangCreateController',
                        resolve: {
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
                        if (updatedData && updatedData.length > 0) {
                            angular.forEach(updatedData, function (v, k) {
                                $scope.lstMerchandiseTypes.push(v);
                            })
                        } else {
                        }
                    }, function () {
                        $log.info('Modal dismissed at: ' + new Date());
                    });
                }
                else {
                    var modalInstance = $uibModal.open({
                        backdrop: 'static',
                        templateUrl: configService.buildUrl('nv/nvKhuyenMai', 'kmLoaiHang'),
                        controller: 'kmLoaiHangEditController',
                        resolve: {
                            initData: function () {
                                return $scope.lstMerchandiseTypes;
                            }
                        }
                    });
                    modalInstance.result.then(function (updatedData) {
                        $scope.lstMerchandiseTypes = [];
                        if (updatedData && updatedData.length > 0) {
                            angular.forEach(updatedData, function (v, k) {
                                $scope.lstMerchandiseTypes.push(v);
                            })
                        } else {
                        }
                    }, function () {
                        $log.info('Modal dismissed at: ' + new Date());
                    });
                }
            }
            $scope.kmNhomHang = function () {
                if (!$scope.statePromotion.kmMerchandiseGroupHasValue()) {
                    var modalInstance = $uibModal.open({
                        backdrop: 'static',
                        templateUrl: configService.buildUrl('nv/nvKhuyenMai', 'kmNhomHang'),
                        controller: 'kmNhomHangCreateController',
                        resolve: {
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
                        if (updatedData && updatedData.length > 0) {
                            angular.forEach(updatedData, function (v, k) {
                                $scope.lstMerchandiseGroups.push(v);
                            })
                        } else {
                        }
                    }, function () {
                        $log.info('Modal dismissed at: ' + new Date());
                    });
                }
                else {
                    var modalInstance = $uibModal.open({
                        backdrop: 'static',
                        templateUrl: configService.buildUrl('nv/nvKhuyenMai', 'kmNhomHang'),
                        controller: 'kmNhomHangEditController',
                        resolve: {
                            initData: function () {
                                return $scope.lstMerchandiseGroups;
                            }

                        }
                    });

                    modalInstance.result.then(function (updatedData) {
                        $scope.lstMerchandiseGroups.clear();
                        $scope.lstMerchandiseGroups = [];
                        if (updatedData && updatedData.length > 0) {
                            angular.forEach(updatedData, function (v, k) {
                                $scope.lstMerchandiseGroups.push(v);
                            })
                        } else {
                        }
                    }, function () {
                        $log.info('Modal dismissed at: ' + new Date());
                    });
                }
            }
            $scope.kmNCC = function () {
                if (!$scope.statePromotion.kmSponsorHasValue()) {
                    var modalInstance = $uibModal.open({
                        backdrop: 'static',
                        templateUrl: configService.buildUrl('nv/nvKhuyenMai', 'kmNCC'),
                        controller: 'kmNCCCreateController',
                        resolve: {
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
                            if (updatedData && updatedData.length > 0) {
                                angular.forEach(updatedData, function (v, k) {
                                    $scope.lstSponsors.push(v);
                                });
                            } else {
                            }
                        }
                    }, function () {
                        $log.info('Modal dismissed at: ' + new Date());
                    });
                }
                else {
                    var modalInstance = $uibModal.open({
                        backdrop: 'static',
                        templateUrl: configService.buildUrl('nv/nvKhuyenMai', 'kmNCC'),
                        controller: 'kmNCCEditController',
                        resolve: {
                            initData: function () {
                                return $scope.lstSponsors;
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
                            $scope.lstSponsors = [];
                            if (updatedData && updatedData.length > 0) {
                                angular.forEach(updatedData, function (v, k) {
                                    $scope.lstSponsors.push(v);
                                });
                            } else {
                                $scope.lstSponsors = [];
                            }
                        }
                    }, function () {
                        $log.info('Modal dismissed at: ' + new Date());
                    });
                }
            }
            $scope.countItem = function (valueOfTypePromotion) {
                var dataFilter = [];
                if ($scope.lstMerchandises) {
                    switch (valueOfTypePromotion) {
                        case '1':
                            dataFilter = $scope.lstMerchandises.filter(function (element) {
                                return element.loaiChuongTrinh == '1';
                            });
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
                            break;
                    }
                }
                return dataFilter.length;
            }
            filterData();
            function filterData() {
                $scope.isLoading = true;
                khuyenMaiService.getDetails($scope.target.id).then(function (response) {
                    if (response.status) {
                        $scope.target = response.data.data;
                        if ($scope.target.maKhoXuatKhuyenMai) {
                            var lstSelectedWareHouse = $scope.target.maKhoXuatKhuyenMai.split(',');
                            angular.forEach(lstSelectedWareHouse, function (item) {
                                var data = $filter('filter')($scope.tempData('wareHouses'), { value: item }, true);
                                if (data && data.length === 1) {
                                    $scope.wareHouseCodes.push(data[0]);
                                }
                            });
                            wareHouseService.clearSelectData();
                            wareHouseService.setSelectData($scope.wareHouseCodes);
                            console.log($scope.wareHouseCodes);
                        }

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
            };
            $scope.pageChanged = function () {
                var currentPage = $scope.paged.currentPage;
                var itemsPerPage = $scope.paged.itemsPerPage;
                if ($scope.target.dataDetails) {
                    $scope.paged.totalItems = $scope.target.dataDetails.length;
                    $scope.data = [];
                    $scope.data = angular.copy($scope.lstMerchandises);
                    khuyenMaiService.setSelectMerchandise($scope.data);
                }
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
    app.controller('khuyenMaiCreateController', ['$scope', '$uibModal', '$uibModalInstance', 'ngNotify', '$filter', '$log','khuyenMaiService', 'tempDataService', 'configService', 'wareHouseService', 'merchandiseTypeService',
        function ($scope, $uibModal, $uibModalInstance, ngNotify, $filter, $log, khuyenMaiService, tempDataService, configService, wareHouseService, serviceMerchandiseType) {
            $scope.robot = angular.copy(khuyenMaiService.robot);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.config = angular.copy(configService);
            $scope.tempData = tempDataService.tempData;
            $scope.putTempData = tempDataService.putTempData;
            $scope.contracts = [];
            $scope.lstMerchandises = [];
            $scope.lstMerchandiseTypes = [];
            $scope.lstMerchandiseGroups = [];
            $scope.lstSponsors = [];
            $scope.target = { dataDetails: [], dataClauseDetails: [] };
            $scope.kmMatHang = function () {
                if ($scope.target.tuNgay && $scope.target.denNgay) {
                    if (!$scope.statePromotion.buyItemGetItemHasValue()) {
                        var modalInstanceCreate = $uibModal.open({
                            backdrop: 'static',
                            templateUrl: configService.buildUrl('nv/nvKhuyenMai', 'kmMatHang'),
                            controller: 'kmMatHangCreateController',
                            size: 'lg',
                            resolve: {
                                filterObject: function() {
                                    return {
                                        advanceData: {
                                            unitCode: $scope.target.unitCode
                                        },
                                        isAdvance: true
                                    }
                                },
                                targetData: function() {
                                    return {
                                        passingData: {
                                            unitCode: $scope.target
                                        }
                                    }
                                }
                            }
                        });
                        modalInstanceCreate.result.then(function(updatedData) {
                            angular.forEach(updatedData, function(v, k) {
                                $scope.lstMerchandises.push(v);
                            });
                            khuyenMaiService.setSelectMerchandise($scope.lstMerchandises);
                        }, function() {
                            $log.info('Modal dismissed at: ' + new Date());
                        });
                    } else {
                        var modalInstance = $uibModal.open({
                            backdrop: 'static',
                            templateUrl: configService.buildUrl('nv/nvKhuyenMai', 'kmMatHang'),
                            controller: 'kmMatHangEditController',
                            size: 'lg',
                            resolve: {
                                filterObject: function() {
                                    return {
                                        advanceData: {
                                            unitCode: $scope.target.unitCode,
                                        },
                                        isAdvance: true
                                    }
                                }
                            }
                        });
                        modalInstance.result.then(function(updatedData) {

                            $scope.lstMerchandises = [];
                            if (updatedData && updatedData.length > 0) {
                                angular.forEach(updatedData, function(v, k) {
                                    $scope.lstMerchandises.push(v);
                                })
                            } else {
                            }

                        }, function() {
                            $log.info('Modal dismissed at: ' + new Date());
                        });
                    }
                }
                else {
                    ngNotify.set("Chưa chọn từ ngày - đến ngày", { duration: 2000, type: 'error' });
                }
                $scope.lstMerchandises = khuyenMaiService.getSelectMerchandise();
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
                return 'Chương trình khuyến mại: Chiết khấu';
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
            
            $scope.save = function () {
                angular.forEach($scope.lstMerchandises, function (v, k) {
                    $scope.target.dataDetails.push(v);
                });
                angular.forEach($scope.lstMerchandiseTypes, function (v, k) {
                    $scope.target.dataDetails.push(v);
                });
                angular.forEach($scope.lstMerchandiseGroups, function (v, k) {
                    $scope.target.dataDetails.push(v);
                });
                angular.forEach($scope.lstSponsors, function (v, k) {
                    $scope.target.dataDetails.push(v);
                });
                $scope.target.tuNgay = $filter('date')($scope.target.tuNgay, 'yyyy-MM-dd');
                $scope.target.denNgay = $filter('date')($scope.target.denNgay, 'yyyy-MM-dd');
                khuyenMaiService.post($scope.target).then(function (successRes) {
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
            $scope.kmLoaiHang = function () {
                if (!$scope.statePromotion.kmMerchandiseTypeHasValue()) {

                    var modalInstance = $uibModal.open({
                        backdrop: 'static',
                        templateUrl: configService.buildUrl('nv/nvKhuyenMai', 'kmLoaiHang'),
                        controller: 'kmLoaiHangCreateController',
                        resolve: {
                            serviceSelectData: function () {
                                return serviceMerchandiseType;
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
                        $scope.lstMerchandiseTypes = [];
                        if (updatedData && updatedData.length > 0) {
                            angular.forEach(updatedData, function (v, k) {
                                $scope.lstMerchandiseTypes.push(v);
                            })
                        } else {
                        }
                    }, function () {
                        $log.info('Modal dismissed at: ' + new Date());
                    });
                }
                else {
                    var modalInstance = $uibModal.open({
                        backdrop: 'static',
                        templateUrl: configService.buildUrl('nv/nvKhuyenMai', 'kmLoaiHang'),
                        controller: 'kmLoaiHangEditController',
                        resolve: {
                            initData: function () {
                                return $scope.lstMerchandiseTypes;
                            }
                        }
                    });

                    modalInstance.result.then(function (updatedData) {
                        $scope.lstMerchandiseTypes = [];
                        if (updatedData && updatedData.length > 0) {
                            angular.forEach(updatedData, function (v, k) {
                                $scope.lstMerchandiseTypes.push(v);
                            })
                        } else {
                        }
                    }, function () {
                        $log.info('Modal dismissed at: ' + new Date());
                    });
                }
            }
            $scope.kmNhomHang = function () {
                if (!$scope.statePromotion.kmMerchandiseGroupHasValue()) {
                    var modalInstance = $uibModal.open({
                        backdrop: 'static',
                        templateUrl: configService.buildUrl('nv/nvKhuyenMai', 'kmNhomHang'),
                        controller: 'kmNhomHangCreateController',
                        resolve: {
                        }
                    });

                    modalInstance.result.then(function (updatedData) {
                        if (updatedData && updatedData.length > 0) {
                            angular.forEach(updatedData, function (v, k) {
                                $scope.lstMerchandiseGroups.push(v);
                            })
                        } else {
                        }
                    }, function () {
                        $log.info('Modal dismissed at: ' + new Date());
                    });
                }
                else {
                    var modalInstance = $uibModal.open({
                        backdrop: 'static',
                        templateUrl: configService.buildUrl('nv/nvKhuyenMai', 'kmNhomHang'),
                        controller: 'kmNhomHangEditController',
                        resolve: {
                            initData: function () {
                                return $scope.lstMerchandiseGroups;
                            }

                        }
                    });

                    modalInstance.result.then(function (updatedData) {
                        $scope.lstMerchandiseGroups.clear();
                        if (updatedData && updatedData.length > 0) {
                            angular.forEach(updatedData, function (v, k) {
                                $scope.lstMerchandiseGroups.push(v);
                            })
                        } else {
                        }
                    }, function () {
                        $log.info('Modal dismissed at: ' + new Date());
                    });
                }
            }
            $scope.kmNCC = function () {
                if (!$scope.statePromotion.kmSponsorHasValue()) {
                    var modalInstance = $uibModal.open({
                        backdrop: 'static',
                        templateUrl: configService.buildUrl('nv/nvKhuyenMai', 'kmNCC'),
                        controller: 'kmNCCCreateController',
                        resolve: {
                        }
                    });

                    modalInstance.result.then(function (updatedData) {
                        if (updatedData) {
                            angular.forEach(updatedData, function (v, k) {
                                $scope.lstSponsors.push(v);
                            })
                        } else {
                        }
                    }, function () {
                        $log.info('Modal dismissed at: ' + new Date());
                    });
                }
                else {
                    var modalInstance = $uibModal.open({
                        backdrop: 'static',
                        templateUrl: configService.buildUrl('nv/nvKhuyenMai', 'kmNCC'),
                        controller: 'kmNCCEditController',
                        resolve: {
                            initData: function () {
                                return $scope.lstSponsors;
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
                        $scope.lstSponsors.clear();
                        if (updatedData && updatedData.length > 0) {
                            angular.forEach(updatedData, function (v, k) {
                                $scope.lstSponsors.push(v);
                            })
                        } else {
                        }
                    }, function () {
                        $log.info('Modal dismissed at: ' + new Date());
                    });
                }
            }
            $scope.countItem = function (valueOfTypePromotion) {
                var dataFilter = [];
                if ($scope.lstMerchandises) {
                    switch (valueOfTypePromotion) {
                        case '1':
                            dataFilter = $scope.lstMerchandises.filter(function (element) {
                                return element.loaiChuongTrinh == '1';
                            });
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
                            break;
                    }
                }
                return dataFilter.length;
            }
            function filterData() {
                $scope.isLoading = true;
                khuyenMaiService.getNewInstance(function (response) {
                    $scope.target = response;
                    $scope.pageChanged();
                    $scope.isLoading = false;
                })
            };

            filterData();
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
        }
    ]);
    app.controller('kmMatHangCreateController', ['$scope', '$uibModalInstance', '$uibModal','khuyenMaiService', 'ngNotify', 'configService', 'tempDataService', 'FileUploader', '$rootScope', 'userService', 'filterObject',
        function ($scope, $uibModalInstance, $uibModal, khuyenMaiService, ngNotify, configService, tempDataService, FileUploader, $rootScope, serviceAuthUser, filterObject) {
            var rootUrl = configService.apiServiceBaseUri;
            var currentUser = serviceAuthUser.GetCurrentUser();
            var unitCode = currentUser.unitCode;
            var serviceUrl = rootUrl + '/api/Nv/ChuongTrinhKhuyenMai';
            $scope.filtered = angular.copy(configService.filterDefault);
            $scope.filtered = angular.extend($scope.filtered, filterObject);
            angular.extend($scope.filtered, filterObject);
            $scope.lstMatHang = [];
            $scope.robot = angular.copy(khuyenMaiService.robot);
            $scope.config = angular.copy(configService);
            $scope.tempData = tempDataService.tempData;
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.target = {
                maHang: ""
            };
            //log
            $scope.downloadTemplate = function () {
                khuyenMaiService.dowloadTemplateExcel('TemplateExcel-KhuyenMaiChietKhau-HangHoa');
            };
            var uploader = $scope.uploader = new FileUploader({
                url: serviceUrl + '/ImportExcelChietKhauHangHoa/' + unitCode,
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

            function changePage() {
                var currentPage = $scope.paged.currentPage;
                var itemsPerPage = $scope.paged.itemsPerPage;
                $scope.paged.totalItems = $scope.hangHoaCollection.length;
                $scope.data = [];
                if ($scope.hangHoaCollection) {
                    for (var i = (currentPage - 1) * itemsPerPage; i < currentPage * itemsPerPage && i < $scope.hangHoaCollection.length; i++) {
                        $scope.data.push($scope.hangHoaCollection[i])
                    }
                }
            }
            changePage();
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
                }, function () {

                });
            }
            $scope.selectedMaHang = function (code) {
                if (code) {
                        khuyenMaiService.getMerchandiseForNvByCode(code, $scope.target.maKhoNhap, unitCode).then(function (response) {
                        if (response && response.status === 200 && response.data && response.data.status) {
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
            $scope.addRow = function () {
                if (!$scope.newItem.soLuong) {
                    $scope.newItem.soLuong = 1;
                    document.getElementById('soLuong').focus();
                }
                else if (!$scope.newItem.tyLeKhuyenMai) {
                    ngNotify.set("Chưa nhập tỷ lệ khuyến mãi", { duration: 1500, type: 'error' });
                    document.getElementById('tyLeKhuyenMai').focus();
                }
                else if (!$scope.newItem.giaTriKhuyenMai) {
                    ngNotify.set("Chưa nhập giá trị khuyến mãi", { duration: 1500, type: 'error' });
                    document.getElementById('giaTriKhuyenMai').focus();
                }
                else {
                    if ($scope.newItem.maHang && $scope.newItem.validateCode == $scope.newItem.maHang) {
                        var exsist = $scope.hangHoaCollection.some(function (element, index, array) {
                            return $scope.newItem.maHang == element.maHang;
                        });
                        if (exsist) {
                            ngNotify.set("Mã hàng này bạn đã nhập rồi. Cộng gộp", { type: 'success' });

                        } else {
                            $scope.newItem.loaiChuongTrinh = 1;
                            $scope.hangHoaCollection.push($scope.newItem);
                        }
                    }
                    $scope.pageChanged();
                    $scope.newItem = {};
                    focus('maHang');
                    document.getElementById('maHang').focus();
                }
            };
            $scope.removeItem = function (item) {
                var index = $scope.hangHoaCollection.indexOf(item);
                $scope.hangHoaCollection.splice(index, 1);
                $scope.pageChanged();
            }
            $scope.save = function () {
                $uibModalInstance.close($scope.hangHoaCollection);
            };
            $scope.pageChanged = function () {
                changePage();
            }
            $scope.cancel = function () {
                $uibModalInstance.close();
            };
            $scope.title = function () {
                return 'Khuyến mại hàng hóa';
            };
        }
    ]);
    app.controller('kmMatHangEditController', [
        '$scope', '$uibModalInstance', '$filter', '$uibModal',
         'khuyenMaiService', 'ngNotify', 'configService', 'tempDataService', 'FileUploader', '$rootScope', 'userService', 'filterObject',
        function ($scope, $uibModalInstance, $filter, $uibModal,
             khuyenMaiService, ngNotify, configService, tempDataService, FileUploader, $rootScope, serviceAuthUser, filterObject) {
            var rootUrl = configService.apiServiceBaseUri;
            var currentUser = serviceAuthUser.GetCurrentUser();
            var unitCode = currentUser.unitCode;
            var serviceUrl = rootUrl + '/api/Nv/ChuongTrinhKhuyenMai';
            $scope.filtered = angular.copy(configService.filterDefault);
            $scope.filtered = angular.extend($scope.filtered, filterObject);
            angular.extend($scope.filtered, filterObject);
            var lstMatHang = [];
            $scope.robot = angular.copy(khuyenMaiService.robot);
            $scope.config = angular.copy(configService);
            $scope.tempData = tempDataService.tempData;
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.target = {
                maHang: ""
            };
            //log
            $scope.downloadTemplate = function () {
                khuyenMaiService.dowloadTemplateExcel('TemplateExcel-KhuyenMaiChietKhau-HangHoa');
            };
            var uploader = $scope.uploader = new FileUploader({
                url: serviceUrl + '/ImportExcelChietKhauHangHoa/' + unitCode,
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
            $scope.hangHoaCollection = khuyenMaiService.getSelectMerchandise();
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

            function changePage() {
                var currentPage = $scope.paged.currentPage;
                var itemsPerPage = $scope.paged.itemsPerPage;
                $scope.paged.totalItems = $scope.hangHoaCollection.length;
                $scope.data = [];
                if ($scope.hangHoaCollection) {
                    for (var i = (currentPage - 1) * itemsPerPage; i < currentPage * itemsPerPage && i < $scope.hangHoaCollection.length; i++) {
                        $scope.data.push($scope.hangHoaCollection[i])
                    }
                }
            }
            changePage();
            $scope.addNewItem = function (strKey) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Merchandise', 'selectData'),
                    controller: 'merchandiseSelectDataController',
                    windowClass: 'app-modal-window',
                    resolve: {
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
                }, function () {

                });
            }
            $scope.selectedMaHang = function (code) {
                if (code) {
                    khuyenMaiService.getMerchandiseForNvByCode(code, $scope.target.maKhoNhap, unitCode).then(function (response) {
                        if (response && response.status === 200 && response.data && response.data.status) {
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
            $scope.addRow = function () {
                if ($scope.newItem.maHang && $scope.newItem.validateCode == $scope.newItem.maHang) {
                    var exsist = $scope.hangHoaCollection.some(function (element, index, array) {
                        return $scope.newItem.maHang == element.maHang;
                    });
                    if (exsist) {
                        ngNotify.set("Mã hàng này bạn đã nhập rồi. Cộng gộp", { type: 'success' });

                    } else {
                        $scope.newItem.loaiChuongTrinh = 1;
                        $scope.hangHoaCollection.push($scope.newItem);
                    }
                }
                $scope.pageChanged();
                $scope.newItem = {};
            };
            $scope.removeItem = function (item) {
                var index = $scope.hangHoaCollection.indexOf(item);
                $scope.hangHoaCollection.splice(index, 1);
                $scope.pageChanged();
            }
            $scope.save = function () {
                $uibModalInstance.close($scope.hangHoaCollection);
            };
            $scope.pageChanged = function () {
                changePage();
            }
            $scope.cancel = function () {
                $uibModalInstance.close();
            };
            $scope.title = function () {
                return 'Khuyến mại hàng hóa';
            };
        }
    ]);
    app.controller('kmMatHangDetailsController', [
        '$scope', '$uibModalInstance', '$filter', '$uibModal',
         'khuyenMaiService', 'ngNotify', 'configService', 'initData', 'tempDataService', 'FileUploader', 'userService',
        function ($scope, $uibModalInstance, $filter, $uibModal,
             khuyenMaiService, ngNotify, configService, initData, tempDataService, FileUploader, serviceAuthUser) {
            var rootUrl = configService.apiServiceBaseUri;
            var currentUser = serviceAuthUser.GetCurrentUser();
            var unitCode = currentUser.unitCode;
            var serviceUrl = rootUrl + '/api/Nv/ChuongTrinhKhuyenMai';
            $scope.config = angular.copy(configService);
            $scope.tempData = tempDataService.tempData;
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.target = {
                maHang: ""
            };
            var uploader = $scope.uploader = new FileUploader({
                url: serviceUrl + '/ImportExcelChietKhauHangHoa/' + unitCode,
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
            $scope.save = function () {
                $uibModalInstance.dismiss('cancel');
            };
            $scope.pageChanged = function () {

                var currentPage = $scope.paged.currentPage;
                var itemsPerPage = $scope.paged.itemsPerPage;
                $scope.paged.totalItems = $scope.hangHoaCollection.length;
                $scope.data = [];
                if ($scope.hangHoaCollection) {
                    for (var i = (currentPage - 1) * itemsPerPage; i < currentPage * itemsPerPage && i < $scope.hangHoaCollection.length; i++) {
                        $scope.data.push($scope.hangHoaCollection[i])
                    }
                }
            }
            $scope.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };
            filterData();
            function filterData() {
                $scope.hangHoaCollection = initData;
                $scope.pageChanged();
            }
            $scope.title = function () {
                return 'Khuyến mại hàng hóa';
            };
        }
    ]);
    app.controller('kmLoaiHangCreateController', [
            '$scope', '$uibModalInstance', 'ngNotify', '$filter', '$state', '$uibModal', '$log',
         'khuyenMaiService', 'tempDataService', 'configService', 'merchandiseTypeService',
        function ($scope, $uibModalInstance, ngNotify, $filter, $state, $uibModal, $log,
             khuyenMaiService, tempDataService, configService, merchandiseTypeService) {
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.save = function () {
                var result = angular.copy($scope.target.dataDetails);
                $uibModalInstance.close(result);
            };
            $scope.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };
            $scope.tempData = tempDataService.tempData;
            $scope.target = { dataDetails: [] };
            $scope.newItem = {};
            $scope.addRow = function () {
                if ($scope.newItem.maHang && $scope.newItem.validateCode == $scope.newItem.maHang) {
                    var exsist = $scope.target.dataDetails.some(function (element, index, array) {
                        return $scope.newItem.maHang == element.maHang;
                    });
                    if (exsist) {
                        ngNotify.set("Mã loại hàng này bạn đã nhập rồi. Cộng gộp", { type: 'success' });

                    } else {
                        $scope.newItem.loaiChuongTrinh = 2;
                        $scope.target.dataDetails.push($scope.newItem);
                    }
                } else {
                    ngNotify.set("Mã Loại hàng chưa đúng!", { type: 'danger' });
                }
                $scope.newItem = {};
                $scope.pageChanged();
            };
            $scope.selectedLoaiVatTu = function (code) {
                if (code) {
                    khuyenMaiService.getMerchandiseTypeForNvByCode(code).then(function (response) {
                        $scope.newItem.maHang = response.data.maLoaiVatTu;
                        $scope.newItem.tenHang = "[" + response.data.maLoaiVatTu + "]" + "-" + response.data.tenLoaiVatTu;
                        $scope.newItem.validateCode = response.data.maLoaiVatTu;
                    }, function (error) {
                        $scope.addNewItem(code);
                    }
                    )
                }
            };
            $scope.addNewItem = function (strKey) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/MerchandiseType', 'selectData'),
                    controller: 'merchandiseTypeSelectDataController',
                    windowClass: 'app-modal-window',
                    resolve: {
                        serviceSelectData: function () {
                            return merchandiseTypeService;
                        },
                        filterObject: function () {
                            return {
                                summary: strKey
                            };
                        }
                    }
                });
                modalInstance.result.then(function (updatedData) {
                    if (updatedData) {
                        if (!updatedData.selected) {
                            $scope.newItem.maHang = updatedData[0].maLoaiVatTu;
                            $scope.newItem.validateCode = updatedData[0].value;
                            $scope.newItem.tenHang = updatedData[0].tenLoaiVatTu;
                            $scope.selectedLoaiVatTu($scope.newItem.maHang);
                        }
                        $scope.pageChanged();
                    }
                }, function () {

                });
            }
            $scope.removeItem = function (index) {
                var currentPage = $scope.paged.currentPage;
                var itemsPerPage = $scope.paged.itemsPerPage;
                var currentPageIndex = (currentPage - 1) * itemsPerPage + index;
                $scope.target.dataDetails.splice(currentPageIndex, 1);
                $scope.pageChanged();
            }
            filterData();
            function filterData() {
                //
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
            }
            $scope.title = function () {
                return 'Khuyến mại Loại hàng hóa';
            };
        }
    ]);
    app.controller('kmLoaiHangEditController', [
            '$scope', '$uibModalInstance', 'ngNotify', '$filter', '$state', '$uibModal', '$log',
         'khuyenMaiService', 'tempDataService', 'configService', 'merchandiseTypeService', 'initData',
        function ($scope, $uibModalInstance, ngNotify, $filter, $state, $uibModal, $log,
             khuyenMaiService, tempDataService, configService, merchandiseTypeService, initData) {
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.save = function () {
                var result = angular.copy($scope.target.dataDetails);
                $uibModalInstance.close(result);
            };
            $scope.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };
            $scope.tempData = tempDataService.tempData;
            $scope.target = { dataDetails: [] };
            $scope.newItem = {};
            $scope.addRow = function () {
                if ($scope.target.dataDetails.length > 0) {
                    if ($scope.newItem.maHang && $scope.newItem.validateCode == $scope.newItem.maHang) {
                        var exsist = $scope.target.dataDetails.some(function (element, index, array) {
                            return $scope.newItem.maHang == element.maHang;
                        });
                        if (exsist) {
                            ngNotify.set("Mã loại hàng này bạn đã nhập rồi. Cộng gộp", { type: 'success' });
                        } else {
                            $scope.newItem.loaiChuongTrinh = 2;
                            $scope.target.dataDetails.push($scope.newItem);
                        }
                    } else {
                        ngNotify.set("Mã Loại hàng chưa đúng!", { type: 'danger' });
                    }
                }
                $scope.newItem = {};
                $scope.pageChanged();
            };
            $scope.selectedLoaiVatTu = function (code) {
                if (code) {
                    khuyenMaiService.getMerchandiseTypeForNvByCode(code).then(function (response) {
                        $scope.newItem.maHang = response.data.maLoaiVatTu;
                        $scope.newItem.tenHang = "[" + response.data.maLoaiVatTu + "]" + "-" + response.data.tenLoaiVatTu;
                        $scope.newItem.validateCode = response.data.maLoaiVatTu;
                    }, function (error) {
                        $scope.addNewItem(code);
                    }
                    )
                }
            };
            $scope.addNewItem = function (strKey) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('mdMerchandiseType', 'selectData'),
                    controller: 'merchandiseTypeSelectDataController',
                    windowClass: 'app-modal-window',
                    resolve: {
                        serviceSelectData: function () {
                            return merchandiseTypeService;
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
                        $scope.newItem.maHang = updatedData.maLoaiVatTu;
                        $scope.newItem.validateCode = updatedData.value;
                        $scope.newItem.tenHang = updatedData.tenLoaiVatTu;
                        $scope.selectedLoaiVatTu();
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
            $scope.title = function () {
                return 'Khuyến mại Loại hàng hóa';
            };
            filterData();
            function filterData() {
                $scope.target.dataDetails = initData;
                $scope.pageChanged();
            }
        }
    ]);
    app.controller('kmNhomHangCreateController', [
            '$scope', '$uibModalInstance', 'ngNotify', '$filter', '$state', '$uibModal', '$log',
         'khuyenMaiService', 'tempDataService', 'configService', 'supplierService',
        function ($scope, $uibModalInstance, ngNotify, $filter, $state, $uibModal, $log,
             khuyenMaiService, tempDataService, configService, supplierService) {

            var validateCode = "";
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.save = function () {
                var result = angular.copy($scope.target.dataDetails);
                $uibModalInstance.close(result);
            };
            $scope.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };
            $scope.tempData = tempDataService.tempData;
            $scope.target = { dataDetails: [] };
            $scope.newItem = {};
            $scope.addRow = function () {
                if ($scope.newItem.maHang && $scope.newItem.validateCode == $scope.newItem.maHang) {
                    var exsist = $scope.target.dataDetails.some(function (element, index, array) {
                        return $scope.newItem.maHang == element.maHang;
                    });
                    if (exsist) {
                        ngNotify.set("Mã nhóm này bạn đã nhập rồi. Cộng gộp", { type: 'success' });

                    } else {
                        $scope.newItem.loaiChuongTrinh = 3;
                        $scope.target.dataDetails.push($scope.newItem);
                    }
                } else {
                    ngNotify.set("Mã nhóm chưa đúng!", { type: 'danger' });
                }
                $scope.newItem = {};
                $scope.pageChanged();
            };
            $scope.selectedNhomHang = function (code) {
                if (code) {
                    khuyenMaiService.getMerchandiseGroupForNvByCode(code).then(function (response) {
                        $scope.newItem.maHang = response.data.maLoaiVatTu;
                        $scope.newItem.validateCode = response.data.maLoaiVatTu;
                        $scope.newItem.tenHang = "[" + response.data.maLoaiVatTu + "]" + "-" + response.data.tenLoaiVatTu;

                    }, function (error) {
                        $scope.addNewItem(code);
                    });
                }
            };
            $scope.addNewItem = function (strKey) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/NhomVatTu', 'selectData'),
                    controller: 'nhomVatTuSelectDataController',
                    windowClass: 'app-modal-window',
                    resolve: {
                        serviceSelectData: function () {
                            return supplierService;
                        },
                        filterObject: function () {
                            return {
                                summary: strKey
                            };
                        }
                    }
                });
                modalInstance.result.then(function (updatedData) {
                    if (updatedData) {
                        if (!updatedData.selected) {
                            $scope.newItem.maHang = updatedData[0].value;
                            $scope.newItem.validateCode = updatedData[0].value;
                            $scope.newItem.tenHang = updatedData[0].text;
                        }
                        $scope.pageChanged();
                    }
                }, function () {

                });
            }
            $scope.removeItem = function (index) {
                var currentPage = $scope.paged.currentPage;
                var itemsPerPage = $scope.paged.itemsPerPage;
                var currentPageIndex = (currentPage - 1) * itemsPerPage + index;
                $scope.target.dataDetails.splice(currentPageIndex, 1);
                $scope.pageChanged();
            }
            filterData();
            function filterData() {
            }
            $scope.pageChanged = function () {
                var currentPage = $scope.paged.currentPage;
                var itemsPerPage = $scope.paged.itemsPerPage;
                $scope.paged.totalItems = $scope.target.dataDetails.length;
                $scope.data = [];
                if ($scope.target.dataDetails && $scope.target.dataDetails.length > 0) {
                    for (var i = (currentPage - 1) * itemsPerPage; i < currentPage * itemsPerPage && i < $scope.target.dataDetails.length; i++) {
                        $scope.data.push($scope.target.dataDetails[i])
                    }
                }
            }
            $scope.title = function () {
                return 'Khuyến mại Nhóm hàng hóa';
            };
        }
    ]);
    app.controller('kmNhomHangEditController', [
            '$scope', '$uibModalInstance', 'ngNotify', '$filter', '$state', '$uibModal', '$log',
         'khuyenMaiService', 'tempDataService', 'configService', 'initData',
        function ($scope, $uibModalInstance, ngNotify, $filter, $state, $uibModal, $log,
             khuyenMaiService, tempDataService, configService, initData) {
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.save = function () {
                var result = angular.copy($scope.target.dataDetails);
                $uibModalInstance.close(result);
            };
            $scope.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };
            $scope.tempData = tempDataService.tempData;
            $scope.target = { dataDetails: [] };
            $scope.newItem = {};
            $scope.addRow = function () {
                if ($scope.newItem.maHang && $scope.newItem.validateCode == $scope.newItem.maHang) {
                    var exsist = $scope.target.dataDetails.some(function (element, index, array) {
                        return $scope.newItem.maHang == element.maHang;
                    });
                    if (exsist) {
                        ngNotify.set("Mã nhóm này bạn đã nhập rồi. Cộng gộp", { type: 'success' });
                    } else {
                        $scope.newItem.loaiChuongTrinh = 3;
                        $scope.target.dataDetails.push($scope.newItem);
                    }
                } else {
                    ngNotify.set("Mã nhóm chưa đúng", { type: 'danger' });
                }
                $scope.newItem = {};
                $scope.pageChanged();
            };
            $scope.selectedNhomHang = function (code) {
                if (code) {
                    khuyenMaiService.getMerchandiseGroupForNvByCode(code).then(function (response) {
                        $scope.newItem.maHang = response.data.maNhom;
                        $scope.newItem.validateCode = response.data.maNhom;
                        $scope.newItem.tenHang = "[" + response.data.maNhom + "]" + "-" + response.data.tenNhom;

                    }, function (error) {
                        $scope.addNewItem(code);
                    }
                    )
                }
            };
            $scope.addNewItem = function (strKey) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/NhomVatTu', 'selectData'),
                    controller: 'nhomVatTuSelectDataController',
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
                    if (!updatedData.selected) {
                        $scope.newItem.maHang = updatedData.value;
                        $scope.newItem.validateCode = updatedData.value;
                        $scope.newItem.tenHang = updatedData.text;
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
            filterData();
            function filterData() {
                $scope.target.dataDetails = initData;
                $scope.pageChanged();
            }
            $scope.title = function () {
                return 'Khuyến mại Nhóm hàng hóa';
            };
        }
    ]);
    app.controller('kmNCCCreateController', [
            '$scope', '$uibModalInstance', 'ngNotify', '$filter', '$state', '$uibModal', '$log',
         'khuyenMaiService', 'tempDataService', 'configService', 'supplierService',
        function ($scope, $uibModalInstance, ngNotify, $filter, $state, $uibModal, $log,
             khuyenMaiService, tempDataService, configService, supplierService) {
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.save = function () {
                var result = angular.copy($scope.target.dataDetails);
                $uibModalInstance.close(result);
            };
            $scope.cancel = function () {
                $uibModalInstance.close();
            };
            $scope.tempData = tempDataService.tempData;
            $scope.target = { dataDetails: [] };
            $scope.newItem = {};
            $scope.addRow = function () {
                if ($scope.newItem.maHang && $scope.newItem.maHang && $scope.newItem.validateCode == $scope.newItem.maHang) {
                    var exsist = $scope.target.dataDetails.some(function (element, index, array) {
                        return $scope.newItem.maHang == element.maHang;
                    });
                    if (exsist) {
                        ngNotify.set("Mã nhóm này bạn đã nhập rồi. Cộng gộp", { type: 'success' });
                    } else {
                        $scope.newItem.loaiChuongTrinh = 4;
                        $scope.target.dataDetails.push($scope.newItem);
                    }
                } else {
                    ngNotify.set("Mã nhóm chưa đúng!", { type: 'danger' });
                }
                $scope.newItem = {};
                $scope.pageChanged();
            };
            $scope.selectedNCC = function (code) {
                if (code) {
                    khuyenMaiService.getNhaCungCapForNvByCode(code).then(function (response) {
                        $scope.newItem.maHang = response.data.mancc;
                        $scope.newItem.validateCode = response.data.mancc;
                        $scope.newItem.tenHang = "[" + response.data.mancc + "]" + "-" + response.data.tenNCC;

                    }, function (error) {
                        $scope.addNewItem(code);
                    }
                    );
                }
            };
            $scope.addNewItem = function (strKey) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/supplier', 'selectData'),
                    controller: 'supplierSelectDataController',
                    windowClass: 'app-modal-window',
                    resolve: {
                        serviceSelectData: function () {
                            return supplierService;
                        },
                        filterObject: function () {
                            return {
                                summary: strKey
                            };
                        }
                    }
                });
                modalInstance.result.then(function (updatedData) {
                    if (updatedData) {
                        if (!updatedData.selected) {
                            $scope.newItem.maHang = updatedData[0].value;
                            $scope.newItem.validateCode = updatedData[0].value;
                            $scope.newItem.tenHang = updatedData[0].text;
                        }
                        $scope.pageChanged();
                    }
                }, function () {

                });
            }
            $scope.removeItem = function (index) {
                var currentPage = $scope.paged.currentPage;
                var itemsPerPage = $scope.paged.itemsPerPage;
                var currentPageIndex = (currentPage - 1) * itemsPerPage + index;
                $scope.target.dataDetails.splice(currentPageIndex, 1);
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
            $scope.title = function () {
                return 'Khuyến mại Nhà cung cấp';
            };
        }
    ]);
    app.controller('kmNCCEditController', [
            '$scope', '$uibModalInstance', 'ngNotify', '$filter', '$state', '$uibModal', '$log',
         'khuyenMaiService', 'tempDataService', 'configService', 'filterObject', 'initData','supplierService',
        function ($scope, $uibModalInstance, ngNotify, $filter, $state, $uibModal, $log,
             khuyenMaiService, tempDataService, configService, filterObject, initData, supplierService) {
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.save = function () {
                var result = angular.copy($scope.target.dataDetails);
                $uibModalInstance.close(result);
            };
            $scope.cancel = function () {
                $uibModalInstance.close();
            };
            $scope.tempData = tempDataService.tempData;
            $scope.target = { dataDetails: [] };
            $scope.newItem = {};
            $scope.addRow = function () {
                if ($scope.newItem.maHang && $scope.newItem.validateCode == $scope.newItem.maHang) {
                    var exsist = $scope.target.dataDetails.some(function (element, index, array) {
                        return $scope.newItem.maHang == element.maHang;
                    });
                    if (exsist) {
                        ngNotify.set("Mã nhà cung cấp này bạn đã nhập rồi. Cộng gộp", { type: 'success' });
                    } else {
                        $scope.newItem.loaiChuongTrinh = 4;
                        $scope.target.dataDetails.push($scope.newItem);
                    }
                } else {
                    ngNotify.set("Mã nhà cung cấp chưa đúng!", { type: 'danger' });

                }
                $scope.newItem = {};
                $scope.pageChanged();
            };
            $scope.addNewItem = function (strKey) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/supplier', 'selectData'),
                    controller: 'supplierSelectDataController',
                    windowClass: 'app-modal-window',
                    resolve: {
                        serviceSelectData: function () {
                            return supplierService;
                        },
                        filterObject: function () {
                            return {
                                summary: strKey
                            };
                        }
                    }
                });
                modalInstance.result.then(function (updatedData) {
                    if (updatedData) {
                        if (!updatedData.selected) {
                            $scope.newItem.maHang = updatedData[0].value;
                            $scope.newItem.validateCode = updatedData[0].value;
                            $scope.newItem.tenHang = updatedData[0].text;
                        }
                        $scope.pageChanged();
                    }
                }, function () {

                });
            }
            $scope.removeItem = function (index) {
                var currentPage = $scope.paged.currentPage;
                var itemsPerPage = $scope.paged.itemsPerPage;
                var currentPageIndex = (currentPage - 1) * itemsPerPage + index;
                $scope.target.dataDetails.splice(currentPageIndex, 1);
                $scope.pageChanged();
            }
            $scope.selectedNCC = function (code) {
                if (code) {
                    khuyenMaiService.getNhaCungCapForNvByCode(code).then(function (response) {
                        $scope.newItem.maHang = response.data.mancc;
                        $scope.newItem.validateCode = response.data.mancc;
                        $scope.newItem.tenHang = "[" + response.data.mancc + "]" + "-" + response.data.tenNCC;

                    }, function (error) {
                        $scope.addNewItem(code);
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
            }
            function filterData() {
                $scope.target.dataDetails = angular.copy(initData);
                $scope.pageChanged();
            }
            filterData();
            $scope.title = function () {
                return 'Khuyến mại Nhà cung cấp';
            };
        }
    ]);
    app.controller('khuyenMaiDeleteController', ['$scope', '$uibModalInstance', '$filter', '$uibModal',
         'khuyenMaiService', 'ngNotify', 'configService', 'tempDataService', 'FileUploader', '$rootScope', 'userService', 'targetData',
        function ($scope, $uibModalInstance, $filter, $uibModal,
        khuyenMaiService, ngNotify, configService, tempDataService, FileUploader, $rootScope, serviceAuthUser, targetData) {
            $scope.target = angular.copy(targetData);
            $scope.save = function () {
                khuyenMaiService.deleteItem($scope.target.id).then(function (response) {
                    if (response.status == 200) {
                        $uibModalInstance.close(response.data);
                    }
                });
            }
            $scope.cancel = function () {
                $uibModalInstance.close();
            }
        }]);

    return app;
});