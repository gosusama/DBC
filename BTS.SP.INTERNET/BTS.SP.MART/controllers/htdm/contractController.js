/*  
* Người tạo : Nguyễn Tuấn Hoàng Anh
* View: BTS.SP.MART/views/htdm/contract
* Vm sevices: BTS.API.SERVICE -> MD ->MdContractVm.cs
* Sevices: BTS.API.SERVICE -> MD -> MdContractService.cs
* Entity: BTS.API.ENTITY -> Md - > MdContract.cs
* Menu: Danh mục-> Danh mục loại vật tư
*/
define(['ui-bootstrap'], function () {
    'use strict';
    var app = angular.module('contractModule', ['ui.bootstrap']);
    app.factory('contractService', ['$http', 'configService', function ($http, configService) {
        var serviceUrl = configService.rootUrlWebApi + '/Md/Contract';
        var calc = {
            changeSoLuongBao: function (item) {
                if (!item.soLuongLe) {
                    item.soLuongLe = 0;
                }
                if (!item.maBaoBi) {
                    item.luongBao = 1;
                }
                item.soLuong = item.soLuongBao * item.luongBao + item.soLuongLe;
                item.thanhTien = item.soLuong * item.donGia;
            },
            changeDonGia: function (item) {
                if (!item.maBaoBi) {
                    item.luongBao = 1;
                }
                if (!item.soLuongBao) {
                    item.soLuongBao = 0;
                }
                if (!item.soLuongLe) {
                    item.soLuongLe = 0;
                }
                item.soLuong = item.soLuongBao * item.luongBao + item.soLuongLe;
                item.thanhTien = item.soLuong * item.donGia;
            },
            changeSoLuongLe: function (item) {
                if (!item.soLuong) {
                    item.soLuong = 0;
                }
                if (!item.donGia) {
                    item.donGia = 0;
                }
                if (!item.maBaoBi) {
                    item.luongBao = 1;
                }
                if (!item.soLuongBao) {
                    item.soLuongBao = 0;
                }
                item.soLuong = item.soLuongBao * item.luongBao + item.soLuongLe;
                item.thanhTien = item.soLuong * item.donGia;
            }
        }
        var result = {
            robot: calc,
            postQuery: function (data) {
                return $http.post(serviceUrl + '/PostQuery', data);
            },
            post: function (data) {
                return $http.post(serviceUrl + '/Post', data);
            },
            update: function (params) {
                return $http.put(serviceUrl + '/' + params.id, params);
            },
            getReport: function (id) {
                return $http.get(serviceUrl + '/GetReport/' + id);
            },
            getDetails: function (id) {
                return $http.get(serviceUrl + '/GetDetails/' + id);
            },
            deleteItem: function (params) {
                return $http.delete(serviceUrl + '/' + params.id, params);
            },
            getMerchandiseForNvByCode: function (code) {
                return $http.get(configService.rootUrlWebApi + '/Md/Merchandise/GetForNvByCode/' + code);
            },
            getProfile: function () {
                return $http.get(configService.rootUrlWebApi + '/Md/Contract/GetProfile');
            }
        }
        return result;
    }]);
    /* controller list */
    app.controller('contractController', ['$scope', '$location', '$http', 'configService', 'contractService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'securityService', 'toaster',
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
                securityService.getAccessList('contract').then(function (successRes) {
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
            $scope.sortType = 'macontract';
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
                    templateUrl: configService.buildUrl('htdm/Contract', 'add'),
                    controller: 'contractCreateController',
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
                    templateUrl: configService.buildUrl('htdm/Contract', 'update'),
                    controller: 'contractEditController',
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
                    templateUrl: configService.buildUrl('htdm/Contract', 'details'),
                    controller: 'contractDetailsController',
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
            $scope.deleteItem = function (target) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Contract', 'delete'),
                    controller: 'contractDeleteController',
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
    app.controller('contractCreateController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'contractService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'serviceContractAndMerchandise',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, serviceContractAndMerchandise) {
            $scope.robot = angular.copy(service.robot);
            $scope.config = angular.copy(configService);
            $scope.target = { dataDetails: [] };
            $scope.data = [];
            $scope.newItem = {};
            $scope.isLoading = false;
            $scope.title = function () { return 'Thêm hợp đồng'; };
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
            service.getProfile().then(function (response) {
                $scope.target.donViThucHien = response.maDonVi;
                $scope.target.nguoiThucHien = response.username;
            });
            $scope.addRow = function () {
                $scope.target.dataDetails.push($scope.newItem);
                $scope.pageChanged();
                $scope.newItem = {};
            };
            $scope.addNewItem = function (strKey) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Contract', 'selectData'),
                    controller: 'merchandiseSelectDataController',
                    windowClass: 'app-modal-window',
                    resolve: {
                        serviceSelectData: function () {
                            return serviceContractAndMerchandise;
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
                        $scope.newItem = updatedData;
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
            $scope.setIndex = function (index) {
                $scope.selectedRow = index;
            }
            $scope.selectedMaBaoBi = function (model, item) {
                if (!model.soLuongBao) {
                    model.soLuongBao = 0;
                }
                if (!model.donGia) {
                    model.donGia = 0;
                }
                if (!model.soLuongLe) {
                    model.soLuongLe = 0;
                }
                model.luongBao = parseFloat(item.extendValue);
                model.soLuong = model.soLuongBao * model.luongBao + model.soLuongLe;
                model.thanhTien = model.soLuong * model.donGia;

            }

            $scope.selectedMaHang = function (code) {
                if (code && code.length > 0) {
                    service.getMerchandiseForNvByCode(code).then(function (response) {
                        $scope.newItem = response.data;
                    },
                       function (error) {
                           $scope.addNewItem(code);
                       }
                    );
                }
            }
            $scope.$watch("target.dataDetails", function (newValue, oldValue) {
                var total = 0;
                angular.forEach($scope.target.dataDetails, function (v, k) {
                    total = total + v.thanhTien;
                });
                $scope.target.giaTriHD = total;
            }, true);
            $scope.target.dataDetails = serviceContractAndMerchandise.getSelectData();

            $scope.createPackage = function (target, name) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Packaging', 'add'),
                    controller: 'packagingCreateController',
                    resolve: {}
                });
                modalInstance.result.then(function (updatedData) {
                    $scope.tempData.update('packagings', function () {
                        if (target && name) {
                            target[name] = updatedData.maBaoBi;
                            target.luongBao = updatedData.soLuong;
                        }
                    });
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };
            $scope.createCustomer = function (target, name) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Customer', 'add'),
                    controller: 'customerCreateController',
                    resolve: {}
                });
                modalInstance.result.then(function (updatedData) {
                    $scope.tempData.update('customers', function () {
                        if (target && name) {
                            target[name] = updatedData.maKH;
                        }

                    });
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };

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
                $scope.target.dataDetails.clear();
                $uibModalInstance.close();
            };
        }]);
    /* controller Edit */
    app.controller('contractEditController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'contractService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify', 'serviceContractAndMerchandise',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify, serviceContractAndMerchandise) {
            $scope.config = angular.copy(configService);
            $scope.robot = angular.copy(service.robot);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.targetData = angular.copy(targetData);
            $scope.tempData = tempDataService.tempData;
            $scope.newItem = {};
            $scope.target = targetData;
            $scope.isLoading = false;
            $scope.title = function () { return 'Cập nhập hợp đồng'; };
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
                $scope.isLoading = true;
                service.getDetails($scope.target.id).then(function (response) {
                    if (response.status) {
                        $scope.target = response.data;
                        serviceContractAndMerchandise.setSelectData($scope.target.dataDetails);
                    }
                    $scope.pageChanged();
                    $scope.isLoading = false;
                    $scope.$watch("target.dataDetails", function (newValue, oldValue) {
                        var total = 0;
                        angular.forEach($scope.target.dataDetails, function (v, k) {
                            total = total + v.thanhTien;
                        });
                        $scope.target.giaTriHD = total;
                    }, true);
                });
            };
            filterData();
            $scope.setIndex = function (index) {
                $scope.selectedRow = index;
            }
            $scope.addRow = function () {
                serviceContractAndMerchandise.getSelectData().push($scope.newItem);
                $scope.target.dataDetails = serviceContractAndMerchandise.getSelectData();
                $scope.pageChanged();
                $scope.newItem = {};
            };
            $scope.removeRow = function (index) {
                var currentPage = $scope.paged.currentPage;
                var itemsPerPage = $scope.paged.itemsPerPage;
                var currentPageIndex = (currentPage - 1) * itemsPerPage + index;
                $scope.target.dataDetails.splice(currentPageIndex, 1);
                serviceContractAndMerchandise.getSelectData().splice(index, 1);
                $scope.pageChanged();
            };
            $scope.selectedMaHang = function(code) {
                if (code && code.length > 0) {
                    service.getMerchandiseForNvByCode(code).then(function(response) {
                            $scope.newItem = response.data;
                        },
                        function(error) {
                            $scope.addNewItem(code);
                        }
                    );
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
                            return serviceContractAndMerchandise;
                        },
                        filterObject: function () {
                            return {
                                summary: strKey
                            };
                        }
                    }
                });
                modalInstance.result.then(function (updatedData) {
                    $scope.target.dataDetails = serviceContractAndMerchandise.getSelectData();
                    if (!updatedData.selected) {
                        $scope.newItem = updatedData;
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
            $scope.selectedMaBaoBi = function (model, item) {
                if (!model.soLuongBao) {
                    model.soLuongBao = 0;
                }
                if (!model.donGia) {
                    model.donGia = 0;
                }
                if (!model.soLuongLe) {
                    model.soLuongLe = 0;
                }
                model.luongBao = parseFloat(item.extendValue);
                model.soLuong = model.soLuongBao * model.luongBao + model.soLuongLe;
                model.thanhTien = model.soLuong * model.donGia;

            }

            $scope.createPackage = function (target, name) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Packaging', 'add'),
                    controller: 'packagingCreateController',
                    resolve: {}
                });
                modalInstance.result.then(function (updatedData) {
                    $scope.tempData.update('packagings', function () {
                        if (target && name) {
                            target[name] = updatedData.maBaoBi;
                            target.luongBao = updatedData.soLuong;
                        }
                    });
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };
            $scope.createCustomer = function (target, name) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Customer', 'add'),
                    controller: 'customerCreateController',
                    resolve: {}
                });
                modalInstance.result.then(function (updatedData) {
                    $scope.tempData.update('customers', function () {
                        if (target && name) {
                            target[name] = updatedData.maKH;
                        }

                    });
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };


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
                $scope.target.dataDetails.clear();
                $uibModalInstance.close();
            };
        }]);

    /* controller Details */
    app.controller('contractDetailsController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'contractService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify) {
            $scope.config = angular.copy(configService);
            $scope.targetData = angular.copy(targetData);
            $scope.tempData = tempDataService.tempData;
            $scope.target = targetData;
            $scope.title = function () { return 'Thông tin hợp đồng'; };
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
            function filterData() {
                $scope.isLoading = true;
                service.getDetails($scope.target.id).then(function (response) {
                    if (response.status) {
                        $scope.target = response.data;
                    }
                    $scope.pageChanged();
                    $scope.isLoading = false;
                });
            };
            filterData();
            $scope.cancel = function () {
                $uibModalInstance.close();
            };

        }]);
    /* controller delete */
    app.controller('contractDeleteController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'contractService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify',
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
    return app;
});

