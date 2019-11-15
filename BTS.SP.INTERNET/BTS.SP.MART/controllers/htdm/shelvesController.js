/*  
* Người tạo : Nguyễn Tuấn Hoàng Anh
* View: BTS.SP.MART/views/htdm/shelves
* Vm sevices: BTS.API.SERVICE -> MD ->MdshelvesVm.cs
* Sevices: BTS.API.SERVICE -> MD -> MdshelvesService.cs
* Entity: BTS.API.ENTITY -> Md - > Mdshelves.cs
* Menu: Danh mục-> Danh mục chiết khấu KH
*/
define(['ui-bootstrap'], function () {
    'use strict';
    var app = angular.module('shelvesModule', ['ui.bootstrap']);
    app.factory('shelvesService', ['$http', 'configService', function ($http, configService) {
        var serviceUrl = configService.rootUrlWebApi + '/Md/shelves';
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
            updateListHangHoa: function (params) {
                return $http.post(serviceUrl + '/PostListHangHoa', params);
            },
            deleteItem: function (params) {
                return $http.delete(serviceUrl + '/' + params.id, params);
            },
            getNewInstance: function () {
                return $http.get(serviceUrl + '/GetNewInstance');
            },
            postSelectData: function (data) {
                return $http.post(serviceUrl + '/PostSelectData', data);
            },
            getAll_Shelves: function () {
                return $http.get(serviceUrl + '/GetAll_Shelves');
            },
            getAll_ShelvesRoot: function () {
                return $http.get(serviceUrl + '/GetAll_ShelvesRoot');
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
        }
        return result;
    }]);
    /* controller list */
    app.controller('shelvesController', ['$scope', '$location', '$http', 'configService', 'shelvesService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'securityService', 'toaster',
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
                securityService.getAccessList('shelves').then(function (successRes) {
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
            $scope.sortType = 'mashelves';
            $scope.sortReverse = false;
            $scope.doSearch = function () {
                $scope.paged.currentPage = 1;
                filterData();
            };
            function loadShelves() {
                if (!tempDataService.tempData('shelves')) {
                    service.getAll_Shelves().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                            tempDataService.putTempData('shelves', successRes.data.data);
                            $scope.shelves = successRes.data.data;
                        } else {
                            console.log('successRes', successRes);
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.shelves = tempDataService.tempData('shelves');
                }
            }
            loadShelves();
            $scope.pageChanged = function () {
                filterData();
            };
            $scope.goHome = function () {
                window.location.href = "#!/home";
            };
            $scope.refresh = function () {
                $scope.setPage($scope.paged.currentPage);
            };
            $scope.title = function () { return 'Kệ hàng' };

            /* Function add New Item */
            $scope.create = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    size: 'md',
                    templateUrl: configService.buildUrl('htdm/Shelves', 'add'),
                    controller: 'shelvesCreateController',
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
                    templateUrl: configService.buildUrl('htdm/Shelves', 'update'),
                    controller: 'shelvesEditController',
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
            $scope.updateShelves = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Shelves', 'updateshelves'),
                    controller: 'shelvesUpdateShelvesController',
                    size: 'lg',
                    resolve: {}
                });
                modalInstance.result.then(function (updatedData) {
                    $scope.tempData.update('shelves');
                    $scope.refresh();
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };

            /* Function Details Item */
            $scope.details = function (target) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Shelves', 'details'),
                    controller: 'shelvesDetailsController',
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
                    templateUrl: configService.buildUrl('htdm/Shelves', 'delete'),
                    controller: 'shelvesDeleteController',
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
    app.controller('shelvesCreateController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'shelvesService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify) {
            $scope.robot = angular.copy(service.robot);
            $scope.config = angular.copy(configService);
            $scope.tempData = tempDataService.tempData;
            $scope.target = {};
            $scope.isLoading = false;
            $scope.title = function () { return 'Thêm kệ hàng'; };
            function filterData() {
                service.getNewInstance().then(function (response) {
                    if (response && response.status == 200 && response.data) {
                        $scope.target = response.data;
                    }
                });
            };
            filterData();
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
    app.controller('shelvesUpdateShelvesController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'shelvesService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'FileUploader',
       function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, FileUploader) {
           $scope.tempData = tempDataService.tempData;
           $scope.target = {};
           $scope.config = angular.copy(configService);
           $scope.title = function () { return 'Cập nhật kệ hàng'; };
           var uploader = $scope.uploader = new FileUploader({
               url: configService.rootUrlWebApi + '/Md/Shelves/UploadFile'
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
               if (status == 200) {
                   $scope.lstVatTu = response;
                   for (var i = 0; i < $scope.lstVatTu.length; i++) {
                       if ($scope.lstVatTu[i]) {
                           $scope.lstVatTu[i].maKeHang = $scope.maKe;
                       }
                   }
                   var modalInstance = $uibModal.open({
                       backdrop: 'static',
                       templateUrl: configService.buildUrl('htdm/Shelves', 'updateKeHang'),
                       controller: 'kiemKeUpdateKeController',
                       size: 'lg',
                       resolve: {
                           targetData: function () {
                               return $scope.lstVatTu;
                           }
                       }
                   });
               }
           };

           $scope.cancel = function () {
               $uibModalInstance.dismiss('cancel');
           };
       }
    ]);
    app.controller('kiemKeUpdateKeController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'shelvesService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'FileUploader', 'targetData',
       function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, FileUploader, targetData) {
           $scope.config = angular.copy(configService);
           $scope.tempData = tempDataService.tempData;
           $scope.lstHangHoa = targetData;
           $scope.title = function () { return 'Cập nhật kệ kiểm kê'; };
           $scope.save = function () {
               var convertData = $scope.lstHangHoa;
               service.updateListHangHoa($scope.lstHangHoa).then(
               function (response) {
                   if (response && response.status == 200 && response.data) {
                       ngNotify.set("Thành công", { type: 'success' });
                       $uibModalInstance.close($scope.target);

                   } else {
                       console.log('ERROR: Update failed! ' + response.errorMessage);
                       ngNotify.set(response.data.errorMessage, { duration: 3000, type: 'error' });
                   }
               });
           };
           $scope.cancel = function () {
               $uibModalInstance.dismiss('cancel');
           };
       }
    ]);
    /* controller Edit */
    app.controller('shelvesEditController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'shelvesService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify) {
            $scope.config = angular.copy(configService);
            $scope.tempData = tempDataService.tempData;
            $scope.target = angular.copy(targetData);;
            $scope.isLoading = false;
            $scope.title = function () { return 'Cập nhập kệ hàng'; };
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
    app.controller('shelvesDetailsController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'shelvesService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify) {
            $scope.config = angular.copy(configService);
            $scope.tempData = tempDataService.tempData;
            $scope.target = angular.copy(targetData);;
            $scope.title = function () { return 'Thông tin kệ hàng'; };
            $scope.cancel = function () {
                $uibModalInstance.close();
            };

        }]);
    /* controller delete */
    app.controller('shelvesDeleteController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'shelvesService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify) {
            $scope.config = angular.copy(configService);
            $scope.target = angular.copy(targetData);;
            $scope.isLoading = false;
            $scope.title = function () { return 'Xoá kệ hàng'; };
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
    /* wareHouse Select Data Controller */
    app.controller('shelvesSelectDataController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'shelvesService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'filterObject', 'serviceSelectData',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, filterObject, serviceSelectData) {
            $scope.config = angular.copy(configService);;
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.filtered = angular.copy(configService.filterDefault);
            $scope.filtered = angular.extend($scope.filtered, filterObject);
            angular.extend($scope.filtered, filterObject);
            var lstTemp = [];
            $scope.modeClickOneByOne = true;
            $scope.title = function () { return 'Danh sách kệ hàng'; };
            $scope.selecteItem = function (item) {
                $uibModalInstance.close(item);
            }
            $scope.isLoading = false;
            $scope.sortType = 'maKeHang'; // set the default sort type
            $scope.sortReverse = false;  // set the default sort order
            function filterData() {
                $scope.listSelectedData = serviceSelectData.getSelectData();
                $scope.isLoading = true;
                var postdata = { paged: $scope.paged, filtered: $scope.filtered };
                service.postSelectData(postdata).then(function (response) {
                    console.log(response);
                    $scope.isLoading = false;
                    if (response.status) {
                        $scope.data = response.data.data.data;
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
    return app;
});

