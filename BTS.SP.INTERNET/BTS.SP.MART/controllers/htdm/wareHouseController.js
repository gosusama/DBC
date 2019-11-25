/*  
* Người tạo : Nguyễn Tuấn Hoàng Anh
* View: BTS.SP.MART/views/htdm/WareHouse
* Vm sevices: BTS.API.SERVICE -> MD ->MdWareHouseVm.cs
* Sevices: BTS.API.SERVICE -> MD -> MdWareHouseService.cs
* Entity: BTS.API.ENTITY -> Md - > MdWareHouse.cs
* Menu: Danh mục-> Danh mục kho hàng
*/
define(['ui-bootstrap', '/BTS.SP.MART/controllers/auth/AuthController.js'], function () {
    'use strict';
    var app = angular.module('wareHouseModule', ['ui.bootstrap', 'authModule']);
    app.factory('wareHouseService', ['$http', 'configService', function ($http, configService) {
        var serviceUrl = configService.rootUrlWebApi + '/Md/WareHouse';
        var selectedData = [];
        var result = {
            postQuery: function (data) {
                return $http.post(serviceUrl + '/PostQuery', data);
            },
            post: function (data) {
                return $http.post(serviceUrl + '/Post', data);
            },
            wareHouseCtl_GetSelectDataByUnitCode_page: function (data) {
                return $http.post(serviceUrl + '/wareHouseCtl_GetSelectDataByUnitCode_page', data);
            },
            getNewInstance: function (callback) {
                return $http.get(serviceUrl + '/GetNewInstance');
            },
            update: function (params) {
                return $http.put(serviceUrl + '/' + params.id, params);
            },
            deleteItem: function (params) {
                return $http.delete(serviceUrl + '/' + params.id, params);
            },
            filterWareHouse: function (maKho) {
                return $http.get(serviceUrl + '/FilterWareHouse/' + maKho);
            },
            //service set ; get data
            getSelectData: function () {
                return selectedData;
            },
            setSelectData: function (array) {
                selectedData = array;
            },
            clearSelectData: function () {
                selectedData = [];
            },
            //end service
            getAll_WareHouse: function () {
                return $http.get(serviceUrl + '/GetAll_WareHouse');
            },
            getByUnit: function (data) {
                return $http.get(serviceUrl + '/GetByUnit/' + data);
            }
        }
        return result;
    }]);
    /* controller list */
    app.controller('wareHouseController', ['$scope', '$location', '$http', 'configService', 'wareHouseService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'securityService', 'toaster',
        function ($scope, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, securityService, toaster) {
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.filtered = angular.copy(configService.filterDefault);
            $scope.tempData = tempDataService.tempData;
            $scope.accessList = {};
            //load dữ liệu
            function filterData() {
                $scope.isLoading = true;
                if ($scope.accessList.view) {
                    var postdata = { paged: $scope.paged, filtered: $scope.filtered };
                    service.postQuery(postdata).then(function (successRes) {
                        console.log('data ck kh:', successRes);
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
                securityService.getAccessList('wareHouse').then(function (successRes) {
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
            $scope.sortType = 'maDVT';
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
            $scope.title = function () { return 'Danh mục kho hàng' };

            /* Function add New Item */
            $scope.create = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    size: 'md',
                    templateUrl: configService.buildUrl('htdm/WareHouse', 'add'),
                    controller: 'wareHouseCreateController',
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
                    templateUrl: configService.buildUrl('htdm/WareHouse', 'update'),
                    controller: 'wareHouseEditController',
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
                    templateUrl: configService.buildUrl('htdm/WareHouse', 'details'),
                    controller: 'wareHouseDetailsController',
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
                    templateUrl: configService.buildUrl('htdm/WareHouse', 'delete'),
                    controller: 'wareHouseDeleteController',
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
    app.controller('wareHouseCreateController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'wareHouseService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'userService',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, serviceAuthUser) {
            var currentUser = serviceAuthUser.GetCurrentUser();
            $scope.robot = angular.copy(service.robot);
            $scope.config = angular.copy(configService);
            $scope.tempData = tempDataService.tempData;
            $scope.target = {};
            $scope.isLoading = false;
            $scope.title = function () { return 'Tạo mới kho hàng'; };
            service.getNewInstance().then(function (response) {
                $scope.target = response;
                if ($scope.target.isHaveKhoBanLe) {
                    $scope.target.isKhoBanLe = false;
                }
                if ($scope.target.isHaveKhoKM) {
                    $scope.target.isKhoKM = false;
                }
            });
            $scope.changeKhoBanLe = function () {
                if ($scope.target.isKhoBanLe && $scope.target.isKhoBanLe == true) {
                    $scope.target.maKho = currentUser.unitCode + "-KBL";
                    if ($scope.target.isKhoKM && $scope.target.isKhoKM == true) {
                        $scope.target.isKhoKM = false;
                    }
                } else {
                    service.getNewInstance().then(function (response) {
                        $scope.target = response;
                        if ($scope.target.isHaveKhoBanLe) {
                            $scope.target.isKhoBanLe = false;
                        }
                        if ($scope.target.isHaveKhoKM) {
                            $scope.target.isKhoKM = false;
                        }
                    });
                }
            };
            $scope.changeKhoKM = function () {
                if ($scope.target.isKhoKM && $scope.target.isKhoKM == true) {
                    $scope.target.maKho = currentUser.unitCode + "-KKM";
                    if ($scope.target.isKhoBanLe && $scope.target.isKhoBanLe == true) {
                        $scope.target.isKhoBanLe = false;
                    }
                } else {
                    service.getNewInstance().then(function (response) {
                        $scope.target = response;
                        if ($scope.target.isHaveKhoBanLe) {
                            $scope.target.isKhoBanLe = false;
                        }
                        if ($scope.target.isHaveKhoKM) {
                            $scope.target.isKhoKM = false;
                        }
                    });
                }
            };
            service.getNewInstance().then(function (response) {
                $scope.target = response;
            });
            $scope.save = function () {
                $scope.target.unitCode = currentUser.unitCode;
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
    app.controller('wareHouseEditController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'wareHouseService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify) {
            $scope.config = angular.copy(configService);
            $scope.targetData = angular.copy(targetData);
            $scope.tempData = tempDataService.tempData;
            $scope.isLoading = false;
            $scope.title = function () { return 'Cập nhập kho hàng'; };
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
    app.controller('wareHouseDetailsController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'wareHouseService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify) {
            $scope.config = angular.copy(configService);
            $scope.targetData = angular.copy(targetData);
            $scope.tempData = tempDataService.tempData;
            $scope.target = targetData;
            $scope.title = function () { return 'Thông tin kho hàng'; };
            $scope.cancel = function () {
                $uibModalInstance.close();
            };

        }]);
    /* controller delete */
    app.controller('wareHouseDeleteController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'wareHouseService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify) {
            $scope.config = angular.copy(configService);
            $scope.targetData = angular.copy(targetData);
            $scope.target = targetData;
            $scope.isLoading = false;
            $scope.title = function () { return 'Xoá kho hàng'; };
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

    /* wareHouse Select Data Controller */
    app.controller('wareHouseSelectDataController', ['$scope', '$uibModalInstance', 'configService', 'wareHouseService', 'filterObject', 'serviceSelectData',
        function ($scope, $uibModalInstance, configService, service, filterObject, serviceSelectData) {
            $scope.config = angular.copy(configService);;
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.filtered = angular.copy(configService.filterDefault);
            $scope.filtered = angular.extend($scope.filtered, filterObject);
            angular.extend($scope.filtered, filterObject);
            var lstTemp = [];
            $scope.modeClickOneByOne = true;
            $scope.title = function () { return 'Danh sách kho hàng'; };
            $scope.isLoading = false;
            $scope.sortType = 'maKho'; // set the default sort type
            $scope.sortReverse = false;  // set the default sort order
            function filterData() {
                $scope.listSelectedData = serviceSelectData.getSelectData();
                $scope.isLoading = true;
                var postdata = { paged: $scope.paged, filtered: $scope.filtered };
                service.wareHouseCtl_GetSelectDataByUnitCode_page(postdata).then(function (response) {
                    $scope.isLoading = false;
                    if (response.status) {
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
            };
            filterData();
            $scope.setPage = function (pageNo) {
                $scope.paged.currentPage = pageNo;
                filterData();
            };
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
            $scope.save = function () {
                $uibModalInstance.close($scope.listSelectedData);
            };
            $scope.cancel = function () {
                service.setSelectData(lstTemp);
                $uibModalInstance.close();
            };
        }]);

    /* wareHouse Only Select Data Controller*/
    app.controller('wareHouseOnlySelectDataController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'wareHouseService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify', 'filterObject', 'serviceSelectData',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify, filterObject, serviceSelectData) {
            $scope.config = angular.copy(configService);
            $scope.targetData = angular.copy(targetData);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.filtered = angular.copy(configService.filterDefault);
            $scope.filtered = angular.extend($scope.filtered, filterObject);
            angular.extend($scope.filtered, filterObject);
            $scope.title = function () { return 'Danh sách kho hàng'; };
            function filterData() {
                $scope.listSelectedData = serviceSelectData.getSelectData();
                $scope.isLoading = true;
                var postdata = { paged: $scope.paged, filtered: $scope.filtered };
                service.wareHouseCtl_GetSelectDataByUnitCode_page(postdata).then(function (response) {
                    $scope.isLoading = false;
                    if (response.status) {
                        $scope.data = response.data.data;
                        //console.log($scope.data);
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
            };
            filterData();
            $scope.setPage = function (pageNo) {
                $scope.paged.currentPage = pageNo;
                filterData();
            };
            $scope.selecteItem = function (item) {
                $uibModalInstance.close(item);
            }
            $scope.isLoading = false;
            $scope.sortType = 'maDVT'; // set the default sort type
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

            $scope.create = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    size: 'md',
                    templateUrl: configService.buildUrl('htdm/WareHouse', 'add'),
                    controller: 'wareHouseCreateController',
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