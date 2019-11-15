/*  
* Người tạo : Nguyễn Tuấn Hoàng Anh
* View: BTS.SP.MART/views/htdm/merchandiseType
* Vm sevices: BTS.API.SERVICE -> MD ->MdMerchandiseTypeVm.cs
* Sevices: BTS.API.SERVICE -> MD -> MdMerchandiseTypeService.cs
* Entity: BTS.API.ENTITY -> Md - > MdMerchandiseType.cs
* Menu: Danh mục-> Danh mục loại vật tư
*/
define(['ui-bootstrap'], function () {
    'use strict';
    var app = angular.module('merchandiseTypeModule', ['ui.bootstrap']);
    app.factory('merchandiseTypeService', ['$http', 'configService', function ($http, configService) {
        var serviceUrl = configService.rootUrlWebApi + '/Md/MerchandiseType';
        var selectedData = [];
        var result = {
            postQuery: function (data) {
                return $http.post(serviceUrl + '/PostQuery', data);
            },
            post: function (data) {
                return $http.post(serviceUrl + '/Post', data);
            },
            postSelectData: function (data) {
                return $http.post(serviceUrl + '/PostSelectData', data);
            },
            getNewInstance: function () {
                return $http.get(serviceUrl + '/GetNewInstance');
            },
            getSelectAll: function () {
                return $http.get(serviceUrl + '/GetSelectAll');
            },
            update: function (params) {
                return $http.put(serviceUrl + '/' + params.id, params);
            },
            deleteItem: function (params) {
                return $http.delete(serviceUrl + '/' + params.id, params);
            },
            filterTypeMerchandiseCodes: function (maLoai) {
                return $http.post(serviceUrl + '/FilterTypeMerchandiseCodes/' + maLoai);
            },
            //service set ; get data
            getSelectData: function () {
                return selectedData;
            },
            setSelectData: function (array) {
                selectedData = array;
            },
            //end service
            getAll_MerchandiseType: function () {
                return $http.get(serviceUrl + '/GetAll_MerchandiseType');
            },
            getAll_MerchandiseTypeRoot: function () {
                return $http.get(serviceUrl + '/GetAll_MerchandiseTypeRoot');
            },
            clearSelectedData: function () {
                selectedData = [];
            }
        }
        return result;
    }]);
    /* controller list */
    app.controller('merchandiseTypeController', ['$scope', 'configService', 'merchandiseTypeService', 'tempDataService', '$uibModal', '$log', 'securityService', 'toaster',
        function ($scope, configService, service, tempDataService, $uibModal, $log, securityService, toaster) {
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
                securityService.getAccessList('merchandiseType').then(function (successRes) {
                    if (successRes && successRes.status == 200) {
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
            $scope.sortType = 'mamerchandiseType';
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
            $scope.title = function () { return 'Danh sách loại Vật tư, hàng hóa' };

            /* Function add New Item */
            $scope.create = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    size: 'md',
                    templateUrl: configService.buildUrl('htdm/MerchandiseType', 'add'),
                    controller: 'merchandiseTypeCreateController',
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
                    templateUrl: configService.buildUrl('htdm/MerchandiseType', 'update'),
                    controller: 'merchandiseTypeEditController',
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
                    templateUrl: configService.buildUrl('htdm/MerchandiseType', 'details'),
                    controller: 'merchandiseTypeDetailsController',
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
                    templateUrl: configService.buildUrl('htdm/MerchandiseType', 'delete'),
                    controller: 'merchandiseTypeDeleteController',
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
    app.controller('merchandiseTypeCreateController', ['$scope', '$uibModalInstance', 'configService', 'merchandiseTypeService', 'tempDataService', 'ngNotify',
        function ($scope, $uibModalInstance, configService, service, tempDataService, ngNotify) {
            $scope.robot = angular.copy(service.robot);
            $scope.config = angular.copy(configService);
            $scope.target = {};
            $scope.isGenCode = true;
            $scope.disableGenCode = true;
            $scope.tempData = tempDataService.tempData;
            service.getNewInstance().then(function (resNewIn) {
                if (resNewIn && resNewIn.status == 200 && resNewIn.data) {
                    $scope.target = resNewIn.data;
                    $scope.target.maLoaiVatTu = resNewIn.data.maLoaiVatTu;
                    $scope.target.trangThai = 10;
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
            $scope.isLoading = false;
            $scope.title = function () { return 'Thêm loại Vật tư, Hàng hóa'; };
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
    app.controller('merchandiseTypeEditController', ['$scope', '$uibModalInstance', 'configService', 'merchandiseTypeService', 'tempDataService', 'targetData', 'ngNotify',
        function ($scope, $uibModalInstance, configService, service, tempDataService, targetData, ngNotify) {
            $scope.config = angular.copy(configService);
            $scope.targetData = angular.copy(targetData);
            $scope.tempData = tempDataService.tempData;
            $scope.target = targetData;
            $scope.isLoading = false;
            $scope.title = function () { return 'Cập nhập loại Vật tư, Hàng hóa'; };
            function filterData() {
                service.getSelectAll().then(function (response) {
                    $scope.merchandiseTypeSort = response;
                });
            }
            filterData();
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
    app.controller('merchandiseTypeDetailsController', ['$scope', '$uibModalInstance', 'configService', 'merchandiseTypeService', 'tempDataService', 'targetData',
        function ($scope, $uibModalInstance, configService, service, tempDataService, targetData) {
            $scope.config = angular.copy(configService);
            $scope.targetData = angular.copy(targetData);
            $scope.tempData = tempDataService.tempData;
            $scope.target = targetData;
            $scope.title = function () { return 'Thông tin loại Vật tư, Hàng hóa'; };
            function filterData() {
                service.getSelectAll().then(function (response) {
                    $scope.merchandiseTypeSort = response;
                });
            }
            filterData();
            $scope.cancel = function () {
                $uibModalInstance.close();
            };

        }]);
    /* controller delete */
    app.controller('merchandiseTypeDeleteController', ['$scope', '$uibModalInstance', 'configService', 'merchandiseTypeService', 'targetData', 'ngNotify',
        function ($scope, $uibModalInstance, configService, service, targetData, ngNotify) {
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
    /* controller delete */
    app.controller('merchandiseTypeSelectDataController', ['$scope', '$uibModalInstance', 'configService', 'merchandiseTypeService', 'filterObject', 'serviceSelectData',
        function ($scope, $uibModalInstance, configService, service, filterObject, serviceSelectData) {
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.filtered = angular.copy(configService.filterDefault);
            angular.extend($scope.filtered, filterObject);
            $scope.sortType = 'maLoaiVatTu'; // set the default sort type
            $scope.sortReverse = false;  // set the default sort order
            $scope.modeClickOneByOne = true;
            $scope.listSelectedData = [];
            var lstTemp = [];
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
                        if (response && response.status == 200 && response.data && response.data.status) {
                            $scope.data = response.data.data.data;
                            angular.extend($scope.paged, response.data);
                        }
                    });
                } else {
                    $scope.listSelectedData = serviceSelectData.getSelectData();
                    lstTemp = angular.copy($scope.listSelectedData);
                    $scope.isLoading = true;
                    postdata = { paged: $scope.paged, filtered: $scope.filtered };
                    service.postSelectData(postdata).then(function (response) {
                        $scope.isLoading = false;
                        if (response && response.status == 200 && response.data && response.data.status) {
                            $scope.data = response.data.data.data;
                            angular.forEach($scope.data, function (v, k) {
                                var isSelected = $scope.listSelectedData.some(function (element, index, array) {
                                    if (!element) return false;
                                    if (typeof element === 'string')
                                        return element == v.value;
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
            filterData();
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
            $scope.title = function () {
                return 'Loại hàng';
            };
            $scope.setPage = function (pageNo) {
                $scope.paged.currentPage = pageNo;
                filterData();
            };
            $scope.selecteItem = function (item) {
                $uibModalInstance.close(item);
            }
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
                    service.setSelectData($scope.listSelectedData);
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
                    service.setSelectData($scope.listSelectedData);
                }
            }
            $scope.save = function () {
                $uibModalInstance.close($scope.listSelectedData);
            };
            $scope.cancel = function () {
                service.setSelectData(lstTemp);
                $uibModalInstance.close();
            };
        }]);
    return app;
});