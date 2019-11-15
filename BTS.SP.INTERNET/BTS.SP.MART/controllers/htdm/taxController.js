/*  
* Người tạo : Nguyễn Tuấn Hoàng Anh
* View: BTS.SP.MART/views/htdm/Tax
* Vm sevices: BTS.API.SERVICE -> MD ->MdTaxVm.cs
* Sevices: BTS.API.SERVICE -> MD -> MdTaxService.cs
* Entity: BTS.API.ENTITY -> Md - > MdTax.cs
* Menu: Danh mục-> Danh mục thuế
*/
define(['ui-bootstrap'], function () {
    'use strict';
    var app = angular.module('taxModule', ['ui.bootstrap']);
    app.factory('taxService', ['$http', 'configService', function ($http, configService) {
        var serviceUrl = configService.rootUrlWebApi + '/Md/Tax';
        var selectedData = [];
        var result = {
            postQuery: function (data) {
                return $http.post(serviceUrl + '/PostQuery', data);
            },
            post: function (data) {
                return $http.post(serviceUrl + '/Post', data);
            },
            update: function (params) {
                return $http.put(serviceUrl + '/' + params.id, params);
            },
            deleteItem: function (params) {
                return $http.delete(serviceUrl + '/' + params.id, params);
            },
            postSelectData: function (jsonObject) {
                return $http.post(serviceUrl + '/PostSelectData', jsonObject);
            },
            getTaxByCode: function (code) {
                return $http.get(serviceUrl + '/GetTaxByCode/' + code);
            },
            //service set ; get data
            getSelectData: function () {
                return selectedData;
            },
            setSelectData: function (array) {
                selectedData = array;
            },
            //end service
            getAll_Tax: function () {
                return $http.get(serviceUrl + '/GetAll_Tax');
            },
            getAll_TyGia: function () {
                return $http.get(serviceUrl + '/GetAll_TyGia');
            },
            getAll_TaxRoot: function () {
                return $http.get(serviceUrl + '/GetAll_TaxRoot');
            }
        }
        return result;
    }]);
    /* controller list */
    app.controller('taxController', ['$scope', '$location', '$http', 'configService', 'taxService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'securityService', 'toaster',
        function ($scope, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, securityService, toaster) {
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.filtered = angular.copy(configService.filterDefault);
            $scope.tempData = tempDataService.tempData;
            $scope.isEditable = true;
            $scope.disabledSave = false;
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
                securityService.getAccessList('tax').then(function (successRes) {
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
            $scope.title = function () { return 'Danh mục thuế' };

            /* Function add New Item */
            $scope.create = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    size: 'md',
                    templateUrl: configService.buildUrl('htdm/Tax', 'add'),
                    controller: 'taxCreateController',
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
                    templateUrl: configService.buildUrl('htdm/Tax', 'update'),
                    controller: 'taxEditController',
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
                    templateUrl: configService.buildUrl('htdm/Tax', 'details'),
                    controller: 'taxDetailsController',
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
                    templateUrl: configService.buildUrl('htdm/Tax', 'delete'),
                    controller: 'taxDeleteController',
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
    app.controller('taxCreateController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'taxService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify) {
            $scope.robot = angular.copy(service.robot);
            $scope.config = angular.copy(configService);
            $scope.tempData = tempDataService.tempData;
            $scope.target = {};
            $scope.isLoading = false;
            $scope.title = function () { return 'Tạo danh mục thuế'; };
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
    app.controller('taxEditController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'taxService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify) {
            $scope.config = angular.copy(configService);
            $scope.targetData = angular.copy(targetData);
            $scope.tempData = tempDataService.tempData;
            $scope.isLoading = false;
            $scope.title = function () { return 'Cập nhập đơn vị tính'; };
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
    app.controller('taxDetailsController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'taxService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify',
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
    app.controller('taxDeleteController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'taxService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify',
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

    /* tax Select Data Controller */
    app.controller('taxSelectDataController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'taxService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'filterObject', 'serviceSelectData',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, filterObject, serviceSelectData) {
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.filtered = angular.copy(configService.filterDefault);
            $scope.filtered = angular.extend($scope.filtered, filterObject);
            angular.extend($scope.filtered, filterObject);
            $scope.modeClickOneByOne = true;
            $scope.title = function () { return 'Danh sách thuế'; };
            function filterData() {
                $scope.listSelectedData = serviceSelectData.getSelectData();
                $scope.isLoading = true;
                var postdata = { paged: $scope.paged, filtered: $scope.filtered };
                service.postSelectData(postdata).then(function (response) {
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
                        })
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
            $scope.sortType = 'maThue'; // set the default sort type
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
            $scope.save = function () {
                $uibModalInstance.close($scope.listSelectedData);
            };
            $scope.cancel = function () {
                $uibModalInstance.close();
            };
        }]);
    return app;
});

