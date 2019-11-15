/*  
* Người tạo : Nguyễn Tuấn Hoàng Anh
* View: BTS.SP.MART/views/htdm/Supplier
* Vm sevices: BTS.API.SERVICE -> MD ->MdSupplierVm.cs
* Sevices: BTS.API.SERVICE -> MD -> MdSupplierService.cs
* Entity: BTS.API.ENTITY -> Md - > MdSupplier.cs
* Menu: Danh mục-> Danh mục nhà cung cấp
*/
define(['ui-bootstrap', '/BTS.SP.MART/controllers/auth/AuthController.js'], function () {
    'use strict';
    var app = angular.module('supplierModule', ['ui.bootstrap', 'authModule']);
    app.factory('supplierService', ['$http', 'configService', function ($http, configService) {
        var serviceUrl = configService.rootUrlWebApi + '/Md/Supplier';
        var selectedData = [];
        var result = {
            postQuery: function (data) {
                return $http.post(serviceUrl + '/PostQuery', data);
            },
            postSelectData: function (data) {
                return $http.post(serviceUrl + '/PostSelectData', data);
            },
            post: function (data) {
                return $http.post(serviceUrl + '/Post', data);
            },
            getNewInstance: function () {
                return $http.get(serviceUrl + '/GetNewInstance');
            },
            update: function (params) {
                return $http.put(serviceUrl + '/Update/' + params.id, params);
            },
            deleteItem: function (params) {
                return $http.delete(serviceUrl + '/' + params.id, params);
            },
            getNewCode: function () {
                return $http.get(serviceUrl + '/GetNewCode');
            },
            //service set ; get data
            getSelectData: function () {
                return selectedData;
            },
            setSelectData: function (array) {
                selectedData = array;
            },
            //end service
            getAll_Supplier: function () {
                return $http.get(serviceUrl + '/GetAll_Supplier');
            },
            getAll_SupplierRoot: function () {
                return $http.get(serviceUrl + '/GetAll_SupplierRoot');
            },
            filterNhaCungCap: function (code) {
                return $http.get(serviceUrl + '/CheckExist/' + code);
            },
            postAsyncFromSql: function (data) {
                return $http.post(serviceUrl + '/PostAsyncFromSql', data);
            },
            postDataSQLQuery: function (data) {
                return $http.post(serviceUrl + '/PostDataSQLQuery', data);
            },
            getNewCodeFromSQL: function (maloaikhach) {
                return $http.get(serviceUrl + '/GetNewCodeFromSQL/' + maloaikhach);
            },
            postToSQL: function (data) {
                return $http.post(serviceUrl + '/PostToSQL', data);
            },
            updateToSQLAndSync: function (params) {
                return $http.put(serviceUrl + '/UpDateToSQLAndSync/' + params.makhachhang, params);
            },
            postSelectDataServerRoot: function (data) {
                return $http.post(serviceUrl + '/PostSelectDataServerRoot', data);
            },
            getNewCodeRoot: function () {
                return $http.get(serviceUrl + '/GetNewCodeRoot');
            },
            postNhaCungCapToOracleRoot: function (data) {
                return $http.post(serviceUrl + '/PostNhaCungCapToOracleRoot', data);
            },
            updateNhaCungCapToOracleRoot: function (params) {
                return $http.put(serviceUrl + '/UpdateNhaCungCapToOracleRoot/' + params.id, params);
            },
            postAsyncSupplierFromOracleRoot: function (data) {
                return $http.post(serviceUrl + '/PostAsyncSupplierFromOracleRoot', data);
            },
            getDetailByCodeRoot: function (code) {
                return $http.get(serviceUrl + '/GetDetailByCodeRoot/' + code);
            },
            getDetailByCode: function (code) {
                return $http.get(serviceUrl + '/GetDetailByCode/' + code);
            },
            postAsyncCompareUpdate: function (data) {
                return $http.post(serviceUrl + '/PostAsyncCompareUpdate', data);
            },
            getRootUnitCode: function () {
                return $http.get(serviceUrl + '/GetRootUnitCode');
            }
        }
        return result;
    }]);
    /* controller list */
    app.controller('supplierController', ['$scope', '$location', '$http', 'configService', 'supplierService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'securityService', 'toaster', 'userService',
        function ($scope, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, securityService, toaster, serviceAuthUser) {
            var currentUser = serviceAuthUser.GetCurrentUser();
            var unitCode = currentUser.unitCode;
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.filtered = angular.copy(configService.filterDefault);
            $scope.tempData = tempDataService.tempData;
            if ($scope.tempData('rootUnitCode')[0].value === unitCode) {
                $scope.isRootUnitCode = true;
            } else {
                $scope.isRootUnitCode = false;
            }
            $scope.isEditable = true;
            $scope.disabledSave = false;
            $scope.target = { options: 'maNCC' };
            $scope.categories = [
            {
                value: 'tenNCC',
                text: 'Tên Nhà cung cấp'
            },
            {
                value: 'maNCC',
                text: 'Mã nhà cung cấp'
            },
            {
                value: 'maSoThue',
                text: 'Mã số thuế'
            },
            {
                value: 'dienThoai',
                text: 'Điện thoại'
            }];

            //load dữ liệu
            function filterData() {
                $scope.isLoading = true;
                if ($scope.accessList.view) {
                    $scope.filtered.advanceData = {};
                    if ($scope.target.options) {
                        $scope.filtered.isAdvance = true;
                        $scope.filtered.advanceData[$scope.target.options] = $scope.filtered.summary;
                    }
                    var postdata = { paged: $scope.paged, filtered: $scope.filtered };
                    service.postQuery(postdata).then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data && successRes.data.status) {
                            $scope.isLoading = false;
                            $scope.data = successRes.data.data.data;
                            angular.extend($scope.paged, successRes.data.data);
                        }
                    }, function (errorRes) {
                        console.log(errorRes);
                    });
                }
            };
            //end

            //check quyền truy cập
            function loadAccessList() {
                securityService.getAccessList('supplier').then(function (successRes) {
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
                filterData();
            };
            $scope.sortType = 'supplier';
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
            $scope.title = function () { return 'Danh sách nhà cung cấp' };

            /* Function add New Item */
            $scope.create = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    size: 'lg',
                    templateUrl: configService.buildUrl('htdm/Supplier', 'add'),
                    controller: 'supplierCreateController',
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
                    templateUrl: configService.buildUrl('htdm/Supplier', 'update'),
                    controller: 'supplierEditController',
                    size: 'lg',
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
                    templateUrl: configService.buildUrl('htdm/Supplier', 'details'),
                    controller: 'supplierDetailsController',
                    size: 'lg',
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
            /* Function asyncView Item */
            $scope.asyncView = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Supplier', 'sync-index'),
                    controller: 'nhaCungCapAsyncController',
                    windowClass: 'app-modal-window',
                    resolve: {}
                });

                modalInstance.result.then(function (updatedData) {
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };
            /* Function Delete Item */
            $scope.deleteItem = function (target) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Supplier', 'delete'),
                    controller: 'supplierDeleteController',
                    size: 'lg',
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
    /* controller nhaCungCapAsyncController */
    app.controller('nhaCungCapAsyncController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'supplierService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify) {
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.filtered = angular.copy(configService.filterDefault);
            $scope.tempData = tempDataService.tempData;
            $scope.isEditable = true;
            $scope.disabledSave = false;
            $scope.target = {
                options: 'maNCC',
                listData: []
            };
            $scope.categories = [{
                value: 'tenNCC',
                text: 'Tên Nhà cung cấp'
            },
            {
                value: 'maNCC',
                text: 'Mã nhà cung cấp'
            },
            {
                value: 'maSoThue',
                text: 'Mã số thuế'
            },
            {
                value: 'dienThoai',
                text: 'Điện thoại'
            }];
            $scope.filtered.advanceData.tieuChiTimKiem = $scope.target.options;
            //load dữ liệu
            function filterData() {
                $scope.isLoading = true;
                $scope.filtered.advanceData = {};
                if ($scope.target.options) {
                    $scope.filtered.isAdvance = true;
                    $scope.filtered.advanceData[$scope.target.options] = $scope.filtered.summary;
                    $scope.filtered.advanceData.tieuChiTimKiem = $scope.target.options;
                }
                var postdata = { paged: $scope.paged, filtered: $scope.filtered };
                service.postSelectDataServerRoot(postdata).then(function (successRes) {
                    if (successRes && successRes.status === 200 && successRes.data.status && successRes.data.data) {
                        $scope.isLoading = false;
                        $scope.target.listData = successRes.data.data;
                    } else {
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
                        i < currentPage * itemsPerPage && i < $scope.target.listData.length; i++) {
                        $scope.data.push($scope.target.listData[i]);
                    }
                }
            };
            //end
            filterData();
            //check quyền truy cập
            //end

            $scope.setPage = function (pageNo) {
                $scope.paged.currentPage = pageNo;
                filterData();
            };
            $scope.sortType = 'supplier';
            $scope.sortReverse = false;
            $scope.doSearch = function () {
                $scope.paged.currentPage = 1;
                filterData();
            };
            $scope.goHome = function () {
                window.location.href = "#!/home";
            };
            $scope.refresh = function () {
                $scope.setPage($scope.paged.currentPage);
            };
            $scope.title = function () { return 'Đồng bộ nhà cung cấp' };

            /* Function add New Item */
            $scope.create = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    size: 'lg',
                    templateUrl: configService.buildUrl('htdm/Supplier', 'sync-add'),
                    controller: 'nhaCungCapAsyncCreateController',
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
                    templateUrl: configService.buildUrl('htdm/Supplier', 'sync-update'),
                    controller: 'nhaCungCapAsyncEditController',
                    size: 'lg',
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
                    templateUrl: configService.buildUrl('htdm/Supplier', 'sync-details'),
                    controller: 'nhaCungCapAsyncDetailsController',
                    size: 'lg',
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
            /* Function asyncView Item */
            $scope.startSync = function (item) {
                if ($scope.asyncing) {
                    return;
                }
                $scope.asyncing = true;
                service.postAsyncSupplierFromOracleRoot(item).then(function (response) {
                    if (response && response.status === 200 && response.data && response.data.status) {
                        ngNotify.set(response.data.message, { type: 'success' });
                    } else {
                        ngNotify.set(response.data.message, { duration: 3000, type: 'error' });
                        if (!response.data.data && response.data.message === 'Đã tồn tại nhà cung cấp này tại hệ thống') {
                            var modalInstance = $uibModal.open({
                                backdrop: 'static',
                                windowClass: 'app-modal-window',
                                templateUrl: configService.buildUrl('htdm/Supplier', 'async-compare'),
                                controller: 'asyncCompareNhaCungCapController',
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
    /** async Compare MatHang Controller -- Phạm Tuấn Anh, so sánh đồng bộ */
    app.controller('asyncCompareNhaCungCapController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'supplierService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'targetData', 'userService',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, targetData, serviceAuthUser) {
            var currentUser = serviceAuthUser.GetCurrentUser();
            $scope.unitCode = currentUser.unitCode;
            $scope.config = angular.copy(configService);
            $scope.target = {};
            $scope.targetClient = {};
            $scope.title = function () { return 'So sánh mã nhà cung cấp'; };
            $scope.targetData = angular.copy(targetData);
            $scope.tempData = tempDataService.tempData;
            function loadRootUnitCode() {
                service.getRootUnitCode().then(function (response) {
                    if (response && response.status == 200 && response.data && response.data.status && response.data.data)
                        $scope.rootUnitCode = response.data.data;
                });
            };
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
                if ($scope.targetData.maNCC) {
                    //lấy mã đơn vị tạo mã
                    service.getDetailByCodeRoot($scope.targetData.maNCC).then(function (response) {
                        if (response && response.status == 200 && response.data && response.data.data) {
                            $scope.target = response.data.data;
                        }
                    });
                    //lấy mã đơn vị hiện tại
                    service.getDetailByCode($scope.targetData.maNCC).then(function (response) {
                        if (response && response.status == 200 && response.data) {
                            $scope.targetClient = response.data;
                        }
                    });
                }
                else {
                    ngNotify.set("Không tìm thấy chi tiết nhà cung cấp", { duration: 3000, type: 'error' });
                }
            };
            filterData();
            $scope.asyncCompare = function () {
                if ($scope.asyncing) {
                    return;
                }
                $scope.asyncing = true;
                service.postAsyncCompareUpdate($scope.targetData).then(function (response) {
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
    /* controller nhaCungCapAsyncCreateController */
    app.controller('nhaCungCapAsyncCreateController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'supplierService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify) {
            $scope.robot = angular.copy(service.robot);
            $scope.config = angular.copy(configService);
            $scope.target = {};
            $scope.tempData = tempDataService.tempData;
            $scope.isLoading = false;
            $scope.title = function () { return 'Tạo đồng bộ nhà cung cấp'; };
            service.getNewCodeRoot().then(function (response) {
                if (response && response.status == 200 && response.data) {
                    $scope.target.maNCC = response.data;
                }
            });
            $scope.save = function () {
                if ($scope.isPending) {
                    return;
                }
                $scope.isPending = true;
                if (!$scope.target.maNCC) {
                    ngNotify.set("Chưa khai báo mã nhà cung cấp", { duration: 3000, type: 'error' });
                } else if (!$scope.target.tenNCC) {
                    ngNotify.set("Chưa khai báo tên nhà cung cấp", { duration: 3000, type: 'error' });
                } else {
                    service.postNhaCungCapToOracleRoot(JSON.stringify($scope.target)).then(function (response) {
                        if (response && response.status === 201 && response.data && response.data.status && response.data.data) {
                            ngNotify.set(response.data.message, { type: 'success' });
                            $uibModalInstance.close($scope.target);
                        } else {
                            ngNotify.set(response.data.message, { duration: 3000, type: 'error' });
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

    /* controller nhaCungCapAsyncEditController */
    app.controller('nhaCungCapAsyncEditController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'supplierService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify) {
            $scope.config = angular.copy(configService);
            $scope.tempData = tempDataService.tempData;
            $scope.target = angular.copy(targetData);;
            $scope.isLoading = false;
            $scope.title = function () { return 'Cập nhập đồng bộ nhà cung cấp'; };
            $scope.save = function () {
                if ($scope.isPending) {
                    return;
                }
                $scope.isPending = true;
                service.updateNhaCungCapToOracleRoot($scope.target).then(function (response) {
                    if (response && response.status === 200 && response.data && response.data.status && response.data.data) {
                        ngNotify.set('Cập nhật đồng bộ thành công', { type: 'success' });
                        $uibModalInstance.close($scope.target);
                    } else {
                        ngNotify.set(response.data.message, { duration: 3000, type: 'error' });
                    }
                    $scope.isPending = false;
                });
            };
            $scope.cancel = function () {
                $uibModalInstance.close();
            };
        }]);
    /* controller nhaCungCapAsyncDetailsController */
    app.controller('nhaCungCapAsyncDetailsController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'supplierService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify) {
            $scope.config = angular.copy(configService);
            $scope.tempData = tempDataService.tempData;
            $scope.target = angular.copy(targetData);
            $scope.isLoading = false;
            $scope.title = function () { return 'Thông tin chi tiết đồng bộ nhà cung cấp'; };
            $scope.cancel = function () {
                $uibModalInstance.close();
            };
        }]);
    /* controller addNew */
    app.controller('supplierCreateController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'supplierService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify) {
            $scope.robot = angular.copy(service.robot);
            $scope.config = angular.copy(configService);
            $scope.target = {};
            $scope.tempData = tempDataService.tempData;
            $scope.isLoading = false;
            $scope.title = function () { return 'Tạo nhà cung cấp'; };
            service.getNewInstance().then(function (response) {
                if (response && response.status == 200 && response.data) {
                    $scope.target = response.data;
                }
            });
            $scope.save = function () {
                service.post($scope.target).then(function (successRes) {
                    if (successRes && successRes.status === 201 && successRes.data.status) {
                        ngNotify.set(successRes.data.message, { type: 'success' });
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

            $scope.cancel = function () {
                $uibModalInstance.close();
            };
        }]);
    /* controller Edit */
    app.controller('supplierEditController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'supplierService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify) {
            $scope.config = angular.copy(configService);
            $scope.targetData = angular.copy(targetData);
            $scope.tempData = tempDataService.tempData;
            $scope.target = targetData;
            $scope.isLoading = false;
            $scope.title = function () { return 'Cập nhập nhà cung cấp'; };
            $scope.save = function () {
                service.update($scope.target).then(function (successRes) {
                    if (successRes && successRes.status === 200 && successRes.data.status) {
                        ngNotify.set(successRes.data.message, { type: 'success' });
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
    app.controller('supplierDetailsController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'supplierService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify) {
            $scope.config = angular.copy(configService);
            $scope.targetData = angular.copy(targetData);
            $scope.tempData = tempDataService.tempData;
            $scope.target = targetData;
            $scope.title = function () { return 'Thông tin mức chiết khấu khách hàng'; };
            $scope.cancel = function () {
                $uibModalInstance.close();
            };

        }]);
    /* controller delete */
    app.controller('supplierDeleteController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'supplierService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify) {
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

    /* supplier Select Data Controller */
    app.controller('supplierSelectDataController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'supplierService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'filterObject', 'serviceSelectData',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, filterObject, serviceSelectData) {
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.filtered = angular.copy(configService.filterDefault);
            $scope.filtered = angular.extend($scope.filtered, filterObject);
            angular.extend($scope.filtered, filterObject);
            $scope.tempData = tempDataService.tempData;
            $scope.modeClickOneByOne = true;
            $scope.listSelectedData = [];
            $scope.listSelectedData = service.getSelectData();
            //load dữ liệu
            function filterData() {
                var postdata = {};
                if (serviceSelectData) {
                    $scope.modeClickOneByOne = false;
                }
                if ($scope.modeClickOneByOne) {
                    $scope.isLoading = true;
                    postdata = { paged: $scope.paged, filtered: $scope.filtered };
                    service.postSelectData(postdata).then(function (response) {
                        $scope.isLoading = false;
                        if (response && response.status == 200 && response.data && response.data.status) {
                            $scope.data = response.data.data.data;
                            angular.extend($scope.paged, response.data.data);
                        }
                    });
                } else {
                    $scope.listSelectedData = serviceSelectData.getSelectData();
                    $scope.isLoading = true;
                    postdata = { paged: $scope.paged, filtered: $scope.filtered };
                    service.postSelectData(postdata).then(function (response) {
                        $scope.isLoading = false;
                        if (response && response.status == 200 && response.data && response.data.status) {
                            $scope.data = response.data.data.data;
                            angular.forEach($scope.data, function (v, k) {
                                var isSelected = $scope.listSelectedData.some(function (element, index, array) {
                                    if (!element) return false;
                                    return element.value == v.value;
                                });
                                if (isSelected) {
                                    $scope.data[k].selected = true;
                                }
                            });
                            angular.extend($scope.paged, response.data.data);
                        }
                    });
                }
            };
            //end
            filterData();
            $scope.setPage = function (pageNo) {
                $scope.paged.currentPage = pageNo;
                filterData();
            };
            $scope.sortType = 'supplier';
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
            $scope.title = function () { return 'Danh sách nhà cung cấp' };

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
            $scope.save = function () {
                $uibModalInstance.close($scope.listSelectedData);
            };
            $scope.cancel = function () {
                $uibModalInstance.close();
            };
        }]);

    /* supplier Only Select Data Controller */
    app.controller('supplierOnlySelectDataController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'supplierService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify', 'filterObject',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify, filterObject) {
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.filtered = angular.copy(configService.filterDefault);
            $scope.filtered = angular.extend($scope.filtered, filterObject);
            angular.extend($scope.filtered, filterObject);
            $scope.tempData = tempDataService.tempData;
            $scope.modeClickOneByOne = true;
            //load dữ liệu
            function filterData() {
                var postdata = {};
                if (serviceSelectData) {
                    $scope.modeClickOneByOne = false;
                }
                if ($scope.modeClickOneByOne) {
                    $scope.isLoading = true;
                    postdata = { paged: $scope.paged, filtered: $scope.filtered };
                    service.postSelectData(postdata).then(function (response) {
                        $scope.isLoading = false;
                        if (response.status) {
                            $scope.data = response.data.data;
                            angular.extend($scope.paged, response.data);
                        }
                    });
                } else {
                    $scope.listSelectedData = serviceSelectData.getSelectData();
                    $scope.isLoading = true;
                    postdata = { paged: $scope.paged, filtered: $scope.filtered };
                    service.postSelectData(postdata).then(function (response) {
                        $scope.isLoading = false;
                        if (response.status) {
                            $scope.data = response.data.data;
                            angular.forEach($scope.data, function (v, k) {
                                var isSelected = $scope.listSelectedData.some(function (element, index, array) {
                                    if (!element) return false;
                                    return element.value == v.value;
                                });
                                if (isSelected) {
                                    $scope.data[k].selected = true;
                                }
                            });
                            angular.extend($scope.paged, response.data);
                        }
                    });
                }
            };
            //end
            filterData();
            $scope.setPage = function (pageNo) {
                $scope.paged.currentPage = pageNo;
                filterData();
            };
            $scope.sortType = 'maNCC';
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
            $scope.title = function () { return 'Danh sách nhà cung cấp' };

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
            $scope.save = function () {
                $uibModalInstance.close($scope.listSelectedData);
            };
            $scope.cancel = function () {
                $uibModalInstance.close();
            };
        }]);
    return app;
});