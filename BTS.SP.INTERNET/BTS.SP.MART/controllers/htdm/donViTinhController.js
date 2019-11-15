/*  
* Người tạo : Nguyễn Tuấn Hoàng Anh
* View: BTS.SP.MART/views/htdm/donViTinh
* Vm sevices: BTS.API.SERVICE -> MD ->MdDonViTinhVm.cs
* Sevices: BTS.API.SERVICE -> MD -> MdDonViTinhService.cs
* Entity: BTS.API.ENTITY -> Md - > MdDonViTinh.cs
* Menu: Danh mục-> Danh mục đơn vị tính
*/
define(['ui-bootstrap'], function () {
    'use strict';
    var app = angular.module('donViTinhModule', ['ui.bootstrap']);
    app.factory('donViTinhService', ['$http', 'configService', function ($http, configService) {
        var serviceUrl = configService.rootUrlWebApi + '/Md/DonViTinh';
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
                return $http.put(serviceUrl + '/' + params.id, params);
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
            getAll_DonViTinh: function () {
                return $http.get(serviceUrl + '/GetAll_DonViTinh');
            },
            getAll_DonViTinhRoot: function () {
                return $http.get(serviceUrl + '/GetAll_DonViTinhRoot');
            }
            //end service
        }
        return result;
    }]);
    /* controller list */
    app.controller('donViTinhController', ['$scope', '$location', '$http', 'configService', 'donViTinhService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'securityService', 'toaster',
        function ($scope, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, securityService, toaster) {
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.filtered = angular.copy(configService.filterDefault);
            $scope.tempData = tempDataService.tempData;
            $scope.isEditable = true;
            $scope.disabledSave = false;
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
                securityService.getAccessList('donViTinh').then(function (successRes) {
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
            $scope.title = function () { return 'Danh mục đơn vị tính' };

            /* Function add New Item */
            $scope.create = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    size: 'md',
                    templateUrl: configService.buildUrl('htdm/DonViTinh', 'add'),
                    controller: 'donViTinhCreateController',
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
                    templateUrl: configService.buildUrl('htdm/DonViTinh', 'update'),
                    controller: 'donViTinhEditController',
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
                    templateUrl: configService.buildUrl('htdm/DonViTinh', 'details'),
                    controller: 'donViTinhDetailsController',
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
                    templateUrl: configService.buildUrl('htdm/DonViTinh', 'delete'),
                    controller: 'donViTinhDeleteController',
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
    app.controller('donViTinhCreateController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'donViTinhService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify) {
            $scope.robot = angular.copy(service.robot);
            $scope.config = angular.copy(configService);
            $scope.tempData = tempDataService.tempData;
            $scope.target = {};
            $scope.isLoading = false;
            $scope.title = function () { return 'Thêm đơn vị tính'; };
            $scope.target = {};
            $scope.isGenCode = true;
            $scope.disableGenCode = true;
            service.getNewInstance().then(function (response) {
                if (response && response.status == 200 && response.data) {
                    $scope.target = response.data;
                }
            });
            $scope.unAuToGenCode = function (event) {
                if (event.target.checked) {
                    $scope.disableGenCode = false;
                    $scope.isGenCode = false;
                }
                else {
                    $scope.disableGenCode = true;
                    $scope.isGenCode = true;
                }
            };
            $scope.save = function () {
                $scope.target.isGenCode = $scope.isGenCode;
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
    app.controller('donViTinhEditController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'donViTinhService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify) {
            $scope.config = angular.copy(configService);
            $scope.target = angular.copy(targetData);
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
    app.controller('donViTinhDetailsController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'donViTinhService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify) {
            $scope.config = angular.copy(configService);
            $scope.tempData = tempDataService.tempData;
            $scope.target = angular.copy(targetData);;
            $scope.title = function () { return 'Thông tin đơn vị tính'; };
            $scope.cancel = function () {
                $uibModalInstance.close();
            };

        }]);
    /* controller delete */
    app.controller('donViTinhDeleteController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'donViTinhService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify) {
            $scope.config = angular.copy(configService);
            $scope.targetData = angular.copy(targetData);
            $scope.target = targetData;
            $scope.isLoading = false;
            $scope.title = function () { return 'Xoá đơn vị tính'; };
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

    /* donViTinh Select Data Controller */
    app.controller('donViTinhSelectDataController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'donViTinhService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify', 'filterObject', 'serviceSelectData',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify, filterObject, serviceSelectData) {
            $scope.config = angular.copy(configService);
            $scope.targetData = angular.copy(targetData);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.filtered = angular.copy(configService.filterDefault);
            $scope.filtered = angular.extend($scope.filtered, filterObject);
            angular.extend($scope.filtered, filterObject);
            $scope.modeClickOneByOne = true;
            $scope.title = function () { return 'Danh sách đơn vị tính'; };
            function filterData() {
                if (serviceSelectData) {
                    $scope.modeClickOneByOne = false;
                }
                var postdata = {};
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
            $scope.save = function () {
                $uibModalInstance.close($scope.listSelectedData);
            };
            $scope.cancel = function () {
                $uibModalInstance.close();
            };
        }]);

    /* donViTinh Only Select Data Controller*/
    app.controller('donViTinhOnlySelectDataController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'donViTinhService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify', 'filterObject', 'serviceSelectData',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify, filterObject, serviceSelectData) {
            $scope.config = angular.copy(configService);
            $scope.targetData = angular.copy(targetData);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.filtered = angular.copy(configService.filterDefault);
            $scope.filtered = angular.extend($scope.filtered, filterObject);
            angular.extend($scope.filtered, filterObject);
            $scope.modeClickOneByOne = true;
            $scope.title = function () { return 'Danh sách đơn vị tính'; };
            function filterData() {
                if (serviceSelectData) {
                    $scope.modeClickOneByOne = false;
                }
                var postdata = {};
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

