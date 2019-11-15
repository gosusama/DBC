/*  
* Người tạo : Nguyễn Tuấn Hoàng Anh
* View: BTS.SP.MART/views/nv/nvCongNoNCC
* Vm sevices: BTS.API.SERVICE -> NV ->NvCongNoVm.cs
* Sevices: BTS.API.SERVICE -> NV -> NvCongNoService.cs
* Entity: BTS.API.ENTITY -> NV - > NvCongNo.cs
* Menu: Nghiệp vụ-> Phiếu xuất bán buôn
*/
define(['ui-bootstrap', '/BTS.SP.MART/controllers/auth/AuthController.js', '/BTS.SP.MART/controllers/htdm/periodController.js', '/BTS.SP.MART/controllers/htdm/merchandiseController.js', '/BTS.SP.MART/controllers/htdm/customerController.js', '/BTS.SP.MART/controllers/htdm/merchandiseTypeController.js', '/BTS.SP.MART/controllers/htdm/nhomVatTuController.js', '/BTS.SP.MART/controllers/htdm/supplierController.js', '/BTS.SP.MART/controllers/htdm/wareHouseController.js', '/BTS.SP.MART/controllers/htdm/packagingController.js', '/BTS.SP.MART/controllers/htdm/taxController.js', '/BTS.SP.MART/controllers/htdm/donViTinhController.js'], function () {
    'use strict';
    var app = angular.module('nvCongNoNCCModule', ['ui.bootstrap', 'authModule', 'periodModule', 'merchandiseModule', 'customerModule', 'merchandiseTypeModule', 'nhomVatTuModule', 'supplierModule', 'wareHouseModule', 'packagingModule', 'taxModule', 'donViTinhModule']);
    app.factory('nvCongNoNCCService', ['$http', 'configService', function ($http, configService) {
        var serviceUrl = configService.rootUrlWebApi + '/NV/CongNo';
        var rootUrl = configService.apiServiceBaseUri;
        var serviceDatHangUrl = rootUrl + '/api/Nv/CongNo';
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
            }
        }
        var result = {
            robot: calc,
            post: function (data) {
                return $http.post(serviceUrl + '/Post', data);
            },
            postQuery: function (data) {
                return $http.post(serviceUrl + '/PostQuery', data);
            },
            getNewInstance: function (loaiChungTu) {
                return $http.get(serviceUrl + '/GetNewInstance/' + loaiChungTu);
            },
            getDetails: function (id) {
                return $http.get(serviceUrl + '/GetDetails/' + id);
            },
            update: function (params) {
                return $http.put(serviceUrl + '/' + params.id, params);
            },
            getAmmountSupplierLend: function (data) {
                return $http.get(serviceUrl + '/GetAmmountSupplierLend/' + data);
            },
            deleteItem: function (params) {
                return $http.delete(serviceUrl + '/' + params.id, params);
            },
        };
        return result;
    }]);
    /* controller list */
    app.controller('nvCongNoNCCController', [
        '$scope', '$location', '$http', 'configService', 'nvCongNoNCCService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'securityService', '$rootScope', 'toaster', 'customerService', 'supplierService',
        function ($scope, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, securityService, $rootScope, toaster, serviceCustomer, serviceSupplier) {
            $scope.LOAICHUNGTU = "CNNCC";
            $scope.robot = angular.copy(service.robot);
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
                    $scope.filtered.advanceData.loaiChungTu = $scope.LOAICHUNGTU;
                    postdata = { paged: $scope.paged, filtered: $scope.filtered };
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
                securityService.getAccessList('nvCongNoNCC').then(function (successRes) {
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
            $scope.title = function () { return 'Phiếu công nợ trả Nhà cung cấp'; };

            /* Function add New Item */
            $scope.create = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    size: 'md',
                    templateUrl: configService.buildUrl('nv/NvCongNoNCC', 'add'),
                    controller: 'nvCongNoNCCCreateController',
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
                    templateUrl: configService.buildUrl('nv/NvCongNoNCC', 'update'),
                    controller: 'nvCongNoNCCEditController',
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
                    templateUrl: configService.buildUrl('nv/NvCongNoNCC', 'details'),
                    controller: 'nvCongNoNCCDetailsController',
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
                    templateUrl: configService.buildUrl('nv/NvCongNoNCC', 'delete'),
                    controller: 'nvCongNoNCCDeleteController',
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

        }]);
    /* controller addNew */
    app.controller('nvCongNoNCCCreateController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'nvCongNoNCCService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'customerService', 'supplierService',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, serviceCustomer, serviceSupplier) {
            $scope.LOAICHUNGTU = "CNNCC";
            $scope.robot = angular.copy(service.robot);
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.filtered = angular.copy(configService.filterDefault);
            $scope.tempData = tempDataService.tempData;
            $scope.target = {};
            $scope.isLoading = false;
            $scope.title = function () { return 'Thêm mới phiếu công nợ trả NCC'; };
            $scope.formatLabel = function (paraValue, moduleName) {
                var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                if (data && data.length === 1) {
                    return data[0].text;
                } else {
                    return paraValue;
                }
            }

            function filterData() {
                $scope.isLoading = true;
                service.getNewInstance($scope.LOAICHUNGTU).then(function (response) {
                    if (response && response.status == 200 && response.data) {
                        $scope.target = response.data;
                        $scope.target.ngayCT = new Date($scope.target.ngayCT);
                        $scope.isLoading = false;
                    }
                });
            };
            filterData();
            $scope.selectedkhachHang = function (maKH) {
                service.getAmmountSupplierLend(maKH).then(function (response) {
                    console.log('response', response);
                    if (response.status === 200) {
                        $scope.target.thanhTienCanTra = response.data.thanhTienCanTra;
                    } else {
                        // console.log('errr', response.message);
                    }
                });
            }
            $scope.save = function () {
                $scope.target.ngayCT = $filter('date')($scope.target.ngayCT, 'yyyy-MM-dd');
                service.post($scope.target).then(function (successRes) {
                    if (successRes && successRes.data) {
                        ngNotify.set("Thêm thành công", { type: 'success' });
                        $uibModalInstance.close($scope.target);
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
            $scope.cancel = function () {
                $uibModalInstance.close();
            };

        }]);
    /* controller Edit */
    app.controller('nvCongNoNCCEditController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'nvCongNoNCCService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify) {
            $scope.LOAICHUNGTU = "CNNCC";
            $scope.robot = angular.copy(service.robot);
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.filtered = angular.copy(configService.filterDefault);
            $scope.tempData = tempDataService.tempData;
            $scope.target = targetData;
            $scope.data = [];
            $scope.isLoading = false;
            $scope.title = function () { return 'Sửa phiếu công nợ trả NCC'; };
            $scope.formatLabel = function (paraValue, moduleName) {
                var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                if (data && data.length === 1) {
                    return data[0].text;
                } else {
                    return paraValue;
                }
            }

            $scope.selectedkhachHang = function (maKH) {
                service.getAmmountSupplierLend(maKH).then(function (response) {
                    console.log('response', response);
                    if (response.status === 200) {
                        $scope.thanhTienCanTra = response.data.thanhTienCanTra;
                    } else {
                        // console.log('errr', response.message);
                    }
                });
            }
            function filterData() {
                service.getDetails($scope.target.id).then(function (response) {
                    if (response && response.status == 200 && response.data) {
                        $scope.target = response.data;
                        $scope.target.ngayCT = new Date($scope.target.ngayCT);
                    }
                });

            };
            filterData();
            $scope.save = function () {
                service.update($scope.target).then(function (successRes) {
                    if (successRes && successRes.data) {
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
            $scope.cancel = function () {
                $uibModalInstance.close();
            };

        }]);
    /* controller Details */
    app.controller('nvCongNoNCCDetailsController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'nvCongNoNCCService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify) {
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.tempData = tempDataService.tempData;
            $scope.target = targetData;
            $scope.isLoading = false;
            $scope.title = function () { return 'Thông tin phiếu công nợ'; };
            //note
            $scope.formatLabel = function (paraValue, moduleName) {
                var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                if (data && data.length === 1) {
                    return data[0].text;
                } else {
                    return paraValue;
                }
            }
            function fillterData() {
                $scope.isLoading = true;
                service.getDetails($scope.target.id).then(function (resgetDetails) {
                    if (resgetDetails && resgetDetails.status == 200 && resgetDetails.data) {
                        $scope.target = resgetDetails.data;
                        $scope.target.ngayCT = new Date($scope.target.ngayCT);
                    }
                    $scope.isLoading = false;
                });
            }

            fillterData();

            $scope.cancel = function () {
                $uibModalInstance.close();
            };

        }]);
    /* controller delete */
    app.controller('nvCongNoNCCDeleteController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'nvCongNoNCCService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify',
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

    return app;
});

