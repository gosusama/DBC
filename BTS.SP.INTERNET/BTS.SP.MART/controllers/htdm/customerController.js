/*  
* Người tạo : Nguyễn Tuấn Hoàng Anh
* View: BTS.SP.MART/views/htdm/customer
* Vm sevices: BTS.API.SERVICE -> MD ->MdcustomerVm.cs
* Sevices: BTS.API.SERVICE -> MD -> MdcustomerService.cs
* Entity: BTS.API.ENTITY -> Md - > Mdcustomer.cs
* Menu: Danh mục-> Danh mục khách hàng
*/
define(['ui-bootstrap', '/BTS.SP.MART/controllers/htdm/hangKhController.js', '/BTS.SP.MART/controllers/auth/AuthController.js'], function () {
    'use strict';
    var app = angular.module('customerModule', ['ui.bootstrap', 'hangKhModule', 'authModule']);
    app.factory('customerService', ['$http', 'configService', function ($http, configService) {
        var serviceUrl = configService.rootUrlWebApi + '/Md/Customer';
        var selectedData = [];
        var result = {
            postQuery: function (data) {
                return $http.post(serviceUrl + '/PostQuery', data);
            },
            post: function (data) {
                return $http.post(serviceUrl + '/Post', data);
            },
            update: function (params) {
                return $http.put(serviceUrl + '/Update/' + params.id, params);
            },
            updateOracleRoot: function (params) {
                return $http.put(serviceUrl + '/UpdateCustomerToOracleRoot/' + params.id, params);
            },
            deleteItem: function (params) {
                return $http.delete(serviceUrl + '/' + params.id, params);
            },
            postSelectData: function (jsonObject) {
                return $http.post(serviceUrl + '/PostSelectData', jsonObject);
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
            getAll_DataCity: function () {
                return $http.get(serviceUrl + '/GetAll_DataCity');
            },
            getDistrictByCity: function (code) {
                return $http.get(serviceUrl + '/GetDistrictByCity/' + code);
            },
            getAll_Customer: function () {
                return $http.get(serviceUrl + '/GetAll_Customer');
            },
            getDetails: function (data) {
                return $http.post(serviceUrl + '/GetDetails', data);
            },
            getNewCodeRoot: function () {
                return $http.get(serviceUrl + '/GetNewCodeRoot');
            },
            postCustomerToOracleRoot: function (data) {
                return $http.post(serviceUrl + '/PostCustomerToOracleRoot', data);
            },
            postSelectDataServerRoot: function (data) {
                return $http.post(serviceUrl + '/PostSelectDataServerRoot', data);
            },
            getDetailsOracleRoot: function (data) {
                return $http.post(serviceUrl + '/GetDetailsOracleRoot', data);
            },
            postAsyncFromOracleRoot: function (data) {
                return $http.post(serviceUrl + '/PostAsyncFromOracleRoot', data);
            }
        }
        return result;
    }]);
    /* controller list */
    app.controller('customerController', ['$scope', 'configService', 'customerService', 'tempDataService', '$filter', '$uibModal', '$log', 'securityService', 'toaster', 'hangKhService', 'userService',
        function ($scope, configService, service, tempDataService, $filter, $uibModal, $log, securityService, toaster, serviceHangKh, serviceAuthUser) {
            var currentUser = serviceAuthUser.GetCurrentUser();
            var unitCode = currentUser.unitCode;
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.filtered = angular.copy(configService.filterDefault);
            $scope.tempData = tempDataService.tempData;
            $scope.isEditable = true;
            $scope.disabledSave = false;
            if ($scope.tempData('rootUnitCode')[0].value === unitCode) {
                $scope.isRootUnitCode = true;
            }
            else {
                $scope.isRootUnitCode = false;
            }
            //load danh muc
            function loadSupplier() {
                if (!tempDataService.tempData('hangKhs')) {
                    serviceHangKh.getAll_HangKh().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                            tempDataService.putTempData('hangKhs', successRes.data.data);
                            $scope.hangKhs = successRes.data.data;
                        } else {
                            console.log('successRes', successRes);
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.hangKhs = tempDataService.tempData('hangKhs');
                }
            };
            function loadCitys() {
                if (!tempDataService.tempData('citys')) {
                    service.getAll_DataCity().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data && successRes.data.data.length > 0) {
                            tempDataService.putTempData('citys', successRes.data.data);
                            $scope.citys = successRes.data.data;
                        } else {
                            console.log('successRes', successRes);
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.citys = tempDataService.tempData('citys');
                }
            };
            loadSupplier();
            loadCitys();

            $scope.displayHepler = function (paraValue, moduleName) {
                var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                if (data && data.length === 1) {
                    return data[0].text;
                } else {
                    return paraValue;
                }
            }
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
                securityService.getAccessList('customer').then(function (successRes) {
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
            $scope.sortType = 'makhachhang';
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
            $scope.title = function () { return 'Khách hàng' };

            /* Function add New Item */
            $scope.create = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    size: 'lg',
                    templateUrl: configService.buildUrl('htdm/Customer', 'add'),
                    controller: 'customerCreateController',
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
                    size: 'lg',
                    templateUrl: configService.buildUrl('htdm/Customer', 'update'),
                    controller: 'customerEditController',
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
                    size: 'lg',
                    templateUrl: configService.buildUrl('htdm/Customer', 'details'),
                    controller: 'customerDetailsController',
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
                    templateUrl: configService.buildUrl('htdm/Customer', 'delete'),
                    controller: 'customerDeleteController',
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

            $scope.asyncView = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Customer', 'async-index'),
                    controller: 'customerAsyncController',
                    windowClass: 'app-modal-window',
                    resolve: {}
                });
                modalInstance.result.then(function (updatedData) {
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };
        }]);

    /* controller addNew */
    app.controller('customerCreateController', ['$scope', '$uibModalInstance', 'configService', 'customerService', 'tempDataService', '$filter', 'ngNotify', 'hangKhService',
        function ($scope, $uibModalInstance, configService, service, tempDataService, $filter, ngNotify, serviceHangKh) {
            $scope.robot = angular.copy(service.robot);
            $scope.config = angular.copy(configService);
            $scope.tempData = tempDataService.tempData;
            $scope.target = {};
            $scope.isLoading = false;
            $scope.title = function () { return 'Thêm khách hàng'; };
            $scope.isGenCode = true;
            $scope.disableGenCode = true;
            //load danh muc
            function loadSupplier() {
                if (!tempDataService.tempData('hangKhs')) {
                    serviceHangKh.getAll_HangKh().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                            tempDataService.putTempData('hangKhs', successRes.data.data);
                            $scope.hangKhs = successRes.data.data;
                        } else {
                            console.log('successRes', successRes);
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.hangKhs = tempDataService.tempData('hangKhs');
                }
            };
            loadSupplier();
            $scope.displayHepler = function (paraValue, moduleName) {
                var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                if (data && data.length === 1) {
                    return data[0].text;
                } else {
                    return paraValue;
                }
            }
            //end
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
            service.getNewCode().then(function (response) {
                if (response && response.status == 200 && response.data) {
                    $scope.target.makh = response.data;
                }
            });
            $scope.changeCity = function (codeCity) {
                if (codeCity) {
                    if (!tempDataService.tempData('districts')) {
                        service.getDistrictByCity(codeCity).then(function (successRes) {
                            if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                                tempDataService.putTempData('districts', successRes.data.data);
                                $scope.districts = successRes.data.data;
                            } else {
                                console.log('successRes', successRes);
                            }
                        }, function (errorRes) {
                            console.log('errorRes', errorRes);
                        });
                    } else {
                        $scope.districts = tempDataService.tempData('districts');
                    }
                }
            };
            $scope.save = function () {
                $scope.target.isGenCode = $scope.isGenCode;
                service.post($scope.target).then(function (successRes) {
                    if (successRes && successRes.status === 201 && successRes.data) {
                        ngNotify.set("Thêm mới thành công", { type: 'success' });
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
    app.controller('customerEditController', ['$scope', '$uibModalInstance', 'configService', 'customerService', 'tempDataService', '$filter', 'targetData', 'ngNotify', 'hangKhService',
        function ($scope, $uibModalInstance, configService, service, tempDataService, $filter, targetData, ngNotify, serviceHangKh) {
            $scope.config = angular.copy(configService);
            $scope.targetData = angular.copy(targetData);
            $scope.tempData = tempDataService.tempData;
            $scope.target = targetData;
            $scope.isLoading = false;
            $scope.title = function () { return 'Cập nhập khách hàng'; };
            //load danh muc
            function loadSupplier() {
                if (!tempDataService.tempData('hangKhs')) {
                    serviceHangKh.getAll_HangKh().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                            tempDataService.putTempData('hangKhs', successRes.data.data);
                            $scope.hangKhs = successRes.data.data;
                        } else {
                            console.log('successRes', successRes);
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.hangKhs = tempDataService.tempData('hangKhs');
                }
            }
            loadSupplier();
            $scope.displayHepler = function (paraValue, moduleName) {
                var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                if (data && data.length === 1) {
                    return data[0].text;
                } else {
                    return paraValue;
                }
            }
            //end
            function filterData() {
                service.getDetails(targetData).then(function (response) {
                    if (response && response.status === 200 && response.data) {
                        $scope.target = response.data;
                        $scope.target.ngaySinh = new Date($scope.target.ngaySinh);
                        $scope.target.ngayDacBiet = new Date($scope.target.ngayDacBiet);
                        $scope.target.ngayCapThe = new Date($scope.target.ngayCapThe);
                        $scope.target.ngayHetHan = new Date($scope.target.ngayHetHan);
                    }
                });
                service.getAll_DataCity().then(function (response) {
                    if (response.status && response.data.length > 0) {
                        $scope.listCity = response.data;
                    }
                });
            }
            filterData();
            $scope.changeCity = function (codeCity) {
                if (codeCity) {
                    if (!tempDataService.tempData('districts')) {
                        service.getDistrictByCity(codeCity).then(function (successRes) {
                            if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                                tempDataService.putTempData('districts', successRes.data.data);
                                $scope.districts = successRes.data.data;
                            } else {
                                console.log('successRes', successRes);
                            }
                        }, function (errorRes) {
                            console.log('errorRes', errorRes);
                        });
                    } else {
                        $scope.districts = tempDataService.tempData('districts');
                    }
                }
            };
            $scope.save = function () {
                service.update($scope.target).then(function (successRes) {
                    if (successRes && successRes.status === 200 && successRes.data) {
                        ngNotify.set("Cập nhật thành công", { type: 'success' });
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
    app.controller('customerDetailsController', ['$scope', '$uibModalInstance', 'configService', 'customerService', 'tempDataService', '$filter', 'targetData', 'hangKhService',
        function ($scope, $uibModalInstance, configService, service, tempDataService, $filter, targetData, serviceHangKh) {
            $scope.config = angular.copy(configService);
            $scope.tempData = tempDataService.tempData;

            //load danh muc
            function loadSupplier() {
                if (!tempDataService.tempData('hangKhs')) {
                    serviceHangKh.getAll_HangKh().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                            tempDataService.putTempData('hangKhs', successRes.data.data);
                            $scope.hangKhs = successRes.data.data;
                        } else {
                            console.log('successRes', successRes);
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.hangKhs = tempDataService.tempData('hangKhs');
                }
            }
            loadSupplier();
            $scope.displayHepler = function (paraValue, moduleName) {
                var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                if (data && data.length === 1) {
                    return data[0].text;
                } else {
                    return paraValue;
                }
            }
            //end
            service.getDetails(targetData).then(function (response) {
                if (response && response.status === 200 && response.data) {
                    $scope.target = response.data;
                }
            });
            $scope.title = function () { return 'Thông tin khách hàng'; };
            $scope.cancel = function () {
                $uibModalInstance.close();
            };

        }]);
    /* controller delete */
    app.controller('customerDeleteController', ['$scope', '$uibModalInstance', 'configService', 'customerService', 'targetData', 'ngNotify',
        function ($scope, $uibModalInstance, configService, service, targetData, ngNotify) {
            $scope.config = angular.copy(configService);
            $scope.isLoading = false;
            $scope.title = function () { return 'Xoá thành phần'; };
            $scope.save = function () {
                service.deleteItem(targetData).then(function (successRes) {
                    if (successRes && successRes.status === 200) {
                        ngNotify.set("Xóa thành công", { type: 'success' });
                        $uibModalInstance.close(targetData);
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

    app.controller('customerSelectDataController', ['$scope', '$uibModalInstance', 'configService', 'customerService', 'filterObject', 'serviceSelectData',
        function ($scope, $uibModalInstance, configService, service, filterObject, serviceSelectData) {
            $scope.config = angular.copy(configService);;
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.filtered = angular.copy(configService.filterDefault);
            $scope.filtered = angular.extend($scope.filtered, filterObject);
            angular.extend($scope.filtered, filterObject);
            var lstTemp = [];
            $scope.modeClickOneByOne = true;
            $scope.title = function () { return 'Danh sách khách hàng'; };
            $scope.selecteItem = function (item) {
                $uibModalInstance.close(item);
            }
            $scope.isLoading = false;
            $scope.sortType = 'makhachhang'; // set the default sort type
            $scope.sortReverse = false;  // set the default sort order
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
                            angular.extend($scope.paged, response.data.data);
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
                            console.log('$scope.data', $scope.data);
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

    app.controller('customerAsyncController', ['$scope', 'configService', 'customerService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'securityService', 'toaster', '$uibModalInstance',
        function ($scope, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, securityService, toaster, $uibModalInstance) {
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.filtered = angular.copy(configService.filterDefault);
            $scope.tempData = tempDataService.tempData;
            $scope.isEditable = true;
            $scope.accessList = {};
            $scope.target = {
                listData: []
            };
            $scope.title = function () { return 'Danh sách khách hàng đồng bộ'; };
            $scope.displayHepler = function (paraValue, moduleName) {
                var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                if (data && data.length === 1) {
                    return data[0].text;
                } else {
                    return paraValue;
                }
            };
            //load dữ liệu
            $scope.pageChanged = function () {
                var currentPage = $scope.paged.currentPage;
                var itemsPerPage = $scope.paged.itemsPerPage;
                $scope.paged.totalItems = $scope.target.listData.length;
                $scope.data = [];
                if ($scope.target.listData) {
                    for (var i = (currentPage - 1) * itemsPerPage;
                        i < currentPage * itemsPerPage && i < $scope.target.listData.length;
                        i++) {
                        $scope.data.push($scope.target.listData[i]);
                    }
                }
            };
            function filterData() {
                $scope.isLoading = true;
                if ($scope.accessList.view) {
                    var postdata = { paged: $scope.paged, filtered: $scope.filtered };
                    service.postSelectDataServerRoot(postdata).then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.status && successRes.data.data) {
                            $scope.isLoading = false;
                            $scope.target.listData = successRes.data.data;
                            $scope.pageChanged();
                        }
                    }, function (errorRes) {
                        console.log(errorRes);
                    });
                }
            };
            //end
            //check quyền truy cập
            function loadAccessList() {
                securityService.getAccessList('customer').then(function (successRes) {
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
            $scope.sortType = 'maVatTu';
            $scope.sortReverse = false;
            $scope.doSearch = function () {
                $scope.paged.currentPage = 1;
                filterData();
            };
            $scope.doSearchStr = function () {
                $scope.filtered.isAdvance = true;
                $scope.paged.currentPage = 1;
                filterData();
            }

            $scope.goHome = function () {
                window.location.href = "#!/home";
            };
            $scope.refresh = function () {
                filterData();
            };

            /* Function add New Item */
            $scope.create = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    size: 'md',
                    templateUrl: configService.buildUrl('htdm/Customer', 'async-add'),
                    controller: 'customerAsyncCreateController',
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
                    templateUrl: configService.buildUrl('htdm/Customer', 'async-edit'),
                    controller: 'customerAsyncEditController',
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
                    templateUrl: configService.buildUrl('htdm/Customer', 'async-details'),
                    controller: 'customerAsyncDetailsController',
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
            $scope.startSync = function (item) {
                if ($scope.asyncing) {
                    return;
                }
                $scope.asyncing = true;
                service.postAsyncFromOracleRoot(item).then(function (response) {
                    if (response && response.status === 200 && response.data && response.data.status) {
                        ngNotify.set(response.data.message, { type: 'success' });
                        $uibModalInstance.close();
                    } else {
                        ngNotify.set(response.data.message, { duration: 3000, type: 'error' });
                    }
                    $scope.asyncing = false;
                    filterData();
                });
            };
            $scope.cancel = function () {
                $uibModalInstance.close();
            };
        }]);

    /* controller Async create Customer */
    app.controller('customerAsyncCreateController', ['$scope', '$uibModalInstance', 'configService', 'customerService', 'tempDataService', '$filter', 'ngNotify', 'hangKhService',
        function ($scope, $uibModalInstance, configService, service, tempDataService, $filter, ngNotify, serviceHangKh) {
            $scope.robot = angular.copy(service.robot);
            $scope.config = angular.copy(configService);
            $scope.tempData = tempDataService.tempData;
            $scope.target = {};
            $scope.isLoading = false;
            $scope.title = function () { return 'Thêm mới đồng bộ khách hàng'; };
            //load danh muc
            function loadHangKhachHangRoot() {
                if (!tempDataService.tempData('hangKhachHangRoot')) {
                    serviceHangKh.getAll_HangKhachHangRoot().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                            tempDataService.putTempData('hangKhachHangRoot', successRes.data.data);
                            $scope.hangKhachHangRoot = successRes.data.data;
                        } else {
                            console.log('successRes', successRes);
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.hangKhachHangRoot = tempDataService.tempData('hangKhachHangRoot');
                }
            };
            loadHangKhachHangRoot();
            $scope.displayHepler = function (paraValue, moduleName) {
                var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                if (data && data.length === 1) {
                    return data[0].text;
                } else {
                    return paraValue;
                }
            }
            //end
            service.getNewCodeRoot().then(function (response) {
                if (response && response.status == 200 && response.data) {
                    $scope.target.makh = response.data;
                }
            });
            $scope.changeCity = function (codeCity) {
                if (codeCity) {
                    if (!tempDataService.tempData('districts')) {
                        service.getDistrictByCity(codeCity).then(function (successRes) {
                            if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                                tempDataService.putTempData('districts', successRes.data.data);
                                $scope.districts = successRes.data.data;
                            } else {
                                console.log('successRes', successRes);
                            }
                        }, function (errorRes) {
                            console.log('errorRes', errorRes);
                        });
                    } else {
                        $scope.districts = tempDataService.tempData('districts');
                    }
                }
            };
            $scope.save = function () {
                $scope.target.isGenCode = $scope.isGenCode;
                service.postCustomerToOracleRoot($scope.target).then(function (successRes) {
                    if (successRes && successRes.status === 201 && successRes.data) {
                        ngNotify.set("Thêm mới thành công", { type: 'success' });
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

    /* controller Details Async*/
    app.controller('customerAsyncDetailsController', ['$scope', '$uibModalInstance', 'configService', 'customerService', 'tempDataService', '$filter', 'targetData', 'hangKhService',
        function ($scope, $uibModalInstance, configService, service, tempDataService, $filter, targetData, serviceHangKh) {
            $scope.config = angular.copy(configService);
            $scope.tempData = tempDataService.tempData;
            //load danh muc
            function loadHangKhachHangRoot() {
                if (!tempDataService.tempData('hangKhachHangRoot')) {
                    serviceHangKh.getAll_HangKhachHangRoot().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                            tempDataService.putTempData('hangKhachHangRoot', successRes.data.data);
                            $scope.hangKhachHangRoot = successRes.data.data;
                        } else {
                            console.log('successRes', successRes);
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.hangKhachHangRoot = tempDataService.tempData('hangKhachHangRoot');
                }
            };
            loadHangKhachHangRoot();
            $scope.displayHepler = function (paraValue, moduleName) {
                var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                if (data && data.length === 1) {
                    return data[0].text;
                } else {
                    return paraValue;
                }
            }
            //end
            service.getDetailsOracleRoot(targetData).then(function (response) {
                if (response && response.status === 200 && response.data) {
                    $scope.target = response.data;
                }
            });
            $scope.title = function () { return 'Thông tin khách hàng đồng bộ'; };
            $scope.cancel = function () {
                $uibModalInstance.close();
            };
        }]);

    /* controller Edit Async*/
    app.controller('customerAsyncEditController', ['$scope', '$uibModalInstance', 'configService', 'customerService', 'tempDataService', '$filter', 'targetData', 'ngNotify', 'hangKhService',
        function ($scope, $uibModalInstance, configService, service, tempDataService, $filter, targetData, ngNotify, serviceHangKh) {
            $scope.config = angular.copy(configService);
            $scope.targetData = angular.copy(targetData);
            $scope.tempData = tempDataService.tempData;
            $scope.target = targetData;
            $scope.isLoading = false;
            $scope.title = function () { return 'Cập nhập khách hàng đồng bộ'; };
            //load danh muc
            function loadHangKhachHangRoot() {
                if (!tempDataService.tempData('hangKhachHangRoot')) {
                    serviceHangKh.getAll_HangKhachHangRoot().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                            tempDataService.putTempData('hangKhachHangRoot', successRes.data.data);
                            $scope.hangKhachHangRoot = successRes.data.data;
                        } else {
                            console.log('successRes', successRes);
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.hangKhachHangRoot = tempDataService.tempData('hangKhachHangRoot');
                }
            };
            loadHangKhachHangRoot();
            $scope.displayHepler = function (paraValue, moduleName) {
                var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                if (data && data.length === 1) {
                    return data[0].text;
                } else {
                    return paraValue;
                }
            }
            //end
            function filterData() {
                service.getDetailsOracleRoot(targetData).then(function (response) {
                    if (response && response.status === 200 && response.data) {
                        $scope.target = response.data;
                    }
                });
            };
            filterData();
            $scope.changeCity = function (codeCity) {
                if (codeCity) {
                    if (!tempDataService.tempData('districts')) {
                        service.getDistrictByCity(codeCity).then(function (successRes) {
                            if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                                tempDataService.putTempData('districts', successRes.data.data);
                                $scope.districts = successRes.data.data;
                            } else {
                                console.log('successRes', successRes);
                            }
                        }, function (errorRes) {
                            console.log('errorRes', errorRes);
                        });
                    } else {
                        $scope.districts = tempDataService.tempData('districts');
                    }
                }
            };
            $scope.save = function () {
                service.updateOracleRoot($scope.target).then(function (successRes) {
                    console.log(successRes);
                    if (successRes && successRes.status === 200 && successRes.data && successRes.data.maKH != '') {
                        ngNotify.set("Cập nhật thành công", { type: 'success' });
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
    return app;
});