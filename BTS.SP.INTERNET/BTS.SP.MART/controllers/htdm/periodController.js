/*  
* Người tạo : Nguyễn Tuấn Hoàng Anh
* View: BTS.SP.MART/views/htdm/period
* Vm sevices: BTS.API.SERVICE -> MD ->MdPeriodVm.cs
* Sevices: BTS.API.SERVICE -> MD -> MdPeriodService.cs
* Entity: BTS.API.ENTITY -> Md - > MdPeriod.cs
* Menu: Danh mục-> Danh mục kỳ kế toán
*/
define(['ui-bootstrap'], function () {
    'use strict';
    var app = angular.module('periodModule', ['ui.bootstrap']);
    app.factory('periodService', ['$http', 'configService', function ($http, configService) {
        var serviceUrl = configService.rootUrlWebApi + '/Md/Period';
        var result = {
            postQuery: function (data) {
                return $http.post(serviceUrl + '/PostQuery', data);
            },
            postCreateNewPeriod: function (data) {
                return $http.post(serviceUrl + '/PostCreateNewPeriod', data);
            },
            post: function (data) {
                return $http.post(serviceUrl + '/Post', data);
            },
            postApproval: function (data) {
                return $http.post(serviceUrl + '/PostAppoval', data);
            },
            postMultiple: function (data) {
                return $http.post(serviceUrl + '/PostMultiplePeriod', data);
            },
            getStatus: function () {
                return $http.get(serviceUrl + '/GetStatus');
            },
            getUpdateGiaVonStatus: function () {
                return $http.get(serviceUrl + '/GetUpdateGiaVonStatus');
            },
            update: function (params) {
                return $http.put(serviceUrl + '/' + params.id, params);
            },
            deleteItem: function (params) {
                return $http.delete(serviceUrl + '/' + params.id, params);
            },
            postCurrentPeriod: function (data) {
                return $http.post(serviceUrl + '/PostCurrentPeriod', data);
            },
            postUpdateGiaVon: function (data) {
                return $http.post(serviceUrl + '/PostUpdateGiaVon', data);
            },
            updatePrice: function (data) {
                return $http.post(serviceUrl + '/UpdatePrice', data);
            },
            getCurrentPeriod: function () {
                return $http.get(serviceUrl + '/GetCurrentPeriod');
            },
            getNewParameter: function () {
                return $http.get(serviceUrl + '/GetNewParameter');
            },
            getKyKeToan: function () {
                return $http.get(serviceUrl + '/GetKyKeToan');
            },
            getNextPeriod: function (period) {
                return $http.post(serviceUrl + '/GetNextPeriod', period);
            },
            openApproval: function (itemPeriod) {
                return $http.post(serviceUrl + '/OpenApproval', itemPeriod);
            },
            checkUnClosingOut: function () {
                return $http.get(serviceUrl + '/CheckUnClosingOut');
            },
            getTableNameByDate: function (paramsDay) {
                return $http.post(serviceUrl + '/GetTableNameByDate', paramsDay);
            },
            getSoLuongTonByDate: function (paramsDay) {
                return $http.post(serviceUrl + '/GetSoLuongTonByDate', paramsDay);
            }
        }
        return result;
    }]);
    /* controller list */
    app.controller('periodController', ['$scope', '$location', '$http', 'configService', 'periodService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'securityService', 'toaster', '$mdDialog',
        function ($scope, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, securityService, toaster, $mdDialog) {
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.filtered = angular.copy(configService.filterDefault);
            $scope.tempData = tempDataService.tempData;
            $scope.target = {};

            $scope.isDisabled = false;
            $scope.isCreateDisabled = false;
            $scope.isUpdateGiaVon = false;
            $scope.stateIsRunning = false;
            $scope.updateGiaVonIsRunning = false;

            function initCollectionYears(year) {
                if (!year) {
                    var currentDate = new Date();
                    var currentYear = currentDate.getFullYear();
                } else {
                    var currentYear = year;
                }
                $scope.collectionYears = [
                    {
                        text: 'Năm ' + (currentYear - 1),
                        value: currentYear - 1
                    },
                    {
                        text: 'Năm ' + currentYear,
                        value: currentYear
                    },
                    {
                        text: 'Năm ' + (currentYear + 1),
                        value: (currentYear + 1)
                    }
                ];
                $scope.target.year = currentYear;
            };

            //load dữ liệu
            function filterData(year) {
                $scope.isLoading = true;
                if ($scope.accessList.view) {
                    $scope.filtered.isAdvance = true;
                    $scope.filtered.advanceData["year"] = year;
                    var postdata = { paged: $scope.paged, filtered: $scope.filtered };
                    service.postQuery(postdata).then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data && successRes.data.status) {
                            $scope.isLoading = false;
                            $scope.data = successRes.data.data.data;
                            var date = new Date();
                            if ($scope.data && $scope.data.length > 0) {
                                angular.forEach($scope.data, function (value, key) {
                                    var datePeriod = new Date(value.toDate);
                                    var nowDate = new Date(date);
                                    if (datePeriod <= nowDate) {
                                        value.isLog = false;
                                    }
                                    else {
                                        value.isLog = true;
                                    }
                                });
                            }
                            angular.extend($scope.paged, successRes.data.data);
                        }
                    }, function (errorRes) {
                        console.log(errorRes);
                    });

                    //service.getStatus().then(function (response) {
                    //    console.log('response', response);
                    //    if (response.status && response.data.state === 20) {
                    //        $scope.stateIsRunning = true;
                    //    }
                    //});
                    service.getUpdateGiaVonStatus().then(function (response) {
                        if (response) {
                            if (response.state === 20) {
                                $scope.updateGiaVonIsRunning = true;
                            }
                        }
                    });
                }
                initCollectionYears($scope.target.year);
            };

            $scope.changeYear = function (year) {
                $scope.target.year = year;
                filterData(year);
                initCollectionYears(year);
            }
            //end
            //check quyền truy cập
            function loadAccessList() {
                securityService.getAccessList('period').then(function (successRes) {
                    if (successRes && successRes.status === 200) {
                        $scope.accessList = successRes.data;
                        if (!$scope.accessList.view) {
                            toaster.pop('error', "Lỗi:", "Không có quyền truy cập !");
                        } else {
                            var currentYear = new Date().getFullYear();
                            filterData(currentYear);
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
                filterData($scope.target.year);
            };
            $scope.sortType = 'maperiod';
            $scope.sortReverse = false;
            $scope.doSearch = function () {
                $scope.paged.currentPage = 1;
                filterData($scope.target.year);
            };
            $scope.pageChanged = function () {
                filterData($scope.target.year);
            };
            $scope.goHome = function () {
                window.location.href = "#!/home";
            };
            $scope.refresh = function () {
                $scope.setPage($scope.paged.currentPage);
            };
            $scope.title = function () { return 'Kỳ khóa sổ' };

            //khóa sổ nhiều kỳ
            $scope.approvalMutiple = function (item) {
                //từ kỳ đang bị mở (chưa khóa) --> item (đến kỳ)
                if (item) {
                    item.toDate = $filter('date')(item.toDate, 'yyyy-MM-dd');
                    item.fromDate = $filter('date')(item.fromDate, 'yyyy-MM-dd');
                    $scope.isDisabled = true;
                    $scope.stateIsRunning = true;
                    service.postMultiple(item).then(function (response) {
                        if (response && response.status == 200 && response.data) {
                            toaster.pop('success', "Thông báo", "Khóa sổ thành công", 2000);
                            $scope.isDisabled = false;
                            $scope.refresh();
                        } else {
                            $scope.isDisabled = false;
                            toaster.pop('error', "Lỗi:", response.message);
                        }
                        $scope.stateIsRunning = false;
                        //End fix
                    });
                }
            };
            $scope.approval = function (item) {
                $scope.isDisabled = true;
                $scope.stateIsRunning = true;
                item.toDate = $filter('date')(item.toDate, 'yyyy-MM-dd');
                item.fromDate = $filter('date')(item.fromDate, 'yyyy-MM-dd');
                service.postApproval(item).then(function (response) {
                    if (response && response.status === 200 && response.data.status) {
                        console.log('Create  Successfully!');
                        ngNotify.set(response.data.message, { type: 'success' });
                        $scope.isDisabled = false;
                        $scope.refresh();
                    } else {
                        $scope.isDisabled = false;
                        ngNotify.set(response.message, { duration: 3000, type: 'error' });
                    }
                    $scope.stateIsRunning = false;
                    //End fix
                });
            };
            $scope.settingPeriod = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Period', 'settingPeriod'),
                    controller: 'settingPeriodController',
                    size: 'lg',
                    resolve: {}
                });
                modalInstance.result.then(function (updatedData) {
                    $scope.tempData.update('periods');
                    $scope.refresh();
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };
            /* Function add New Item */
            $scope.create = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    size: 'md',
                    templateUrl: configService.buildUrl('htdm/Period', 'add'),
                    controller: 'periodCreateController',
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
                    templateUrl: configService.buildUrl('htdm/Period', 'update'),
                    controller: 'periodEditController',
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
                    templateUrl: configService.buildUrl('htdm/Period', 'details'),
                    controller: 'periodDetailsController',
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
                    templateUrl: configService.buildUrl('htdm/Period', 'delete'),
                    controller: 'periodDeleteController',
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

            $scope.save = function () {
                $scope.isCreateDisabled = true;
                service.postCreateNewPeriod(JSON.stringify({ year: $scope.target.year })).then(function (successRes) {
                    if (successRes && successRes.status === 200 && successRes.data) {
                        ngNotify.set(successRes.data.message, { type: 'success' });
                    } else {
                        ngNotify.set(successRes.data.message, { duration: 3000, type: 'error' });
                    }
                    $scope.isCreateDisabled = false;
                },
                function (errorRes) {
                    console.log('errorRes', errorRes);
                });
            };
            $scope.currentPeriod = function () {
                $scope.data = [];
                $scope.sortType = 'period';
                $scope.sortReverse = false;
                service.getCurrentPeriod().then(function (response) {
                    if (response && response.status === 200 && response.data && response.data.status) {
                        $scope.isLoading = false;
                        $scope.data = response.data.data;
                        angular.extend($scope.paged, response.data);
                    }
                });
            };
            $scope.updateGiaVon = function (item) {
                $scope.updateGiaVonIsRunning = true;
                item.toDate = $filter('date')(item.toDate, 'yyyy-MM-dd');
                item.fromDate = $filter('date')(item.fromDate, 'yyyy-MM-dd');
                service.postUpdateGiaVon(item).then(function (response) {
                    if (response && response.status === 200 && response.statusText === "OK") {
                        ngNotify.set("Cập nhật giá vốn thành công", { type: 'success' });
                        $scope.updateGiaVonIsRunning = false;
                    } else {
                        ngNotify.set("Xảy ra lỗi !", { duration: 3000, type: 'error' });
                        $scope.updateGiaVonIsRunning = false;
                    }
                });
            };
            //chức năng mở khóa sổ
            $scope.openApproval = function (item, ev) {
                item.toDate = $filter('date')(item.toDate, 'yyyy-MM-dd');
                item.fromDate = $filter('date')(item.fromDate, 'yyyy-MM-dd');
                var confirm = $mdDialog.confirm()
                .title('Cảnh báo')
                .textContent('Bạn hãy khóa sổ lại ngay khi đã thao tác xong !, bạn có chắc muốn thực hiện?')
                .ariaLabel('Lucky day')
                .targetEvent(ev)
                .ok('Ok')
                .cancel('Cancel');
                $mdDialog.show(confirm).then(function () {
                    service.openApproval(item).then(function (response) {
                        if (response && response.status === 200 && response.data && response.data.data.length > 0) {
                            toaster.pop('success', "Thông báo", response.data.message, 2000);
                        }
                    });
                }, function () {
                    console.log('Dừng thao tác');
                });
            };
        }]);

    /* controller addNew */
    app.controller('periodCreateController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'periodService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify) {
            $scope.robot = angular.copy(service.robot);
            $scope.config = angular.copy(configService);
            $scope.target = {};
            $scope.title = function () { return 'Tạo mới kỳ khóa sổ'; };
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
    app.controller('periodEditController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'periodService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify) {
            $scope.config = angular.copy(configService);
            $scope.tempData = tempDataService.tempData;
            $scope.target = targetData;
            $scope.tempObject = angular.copy(targetData);
            $scope.tempObject.toDate = $filter('date')($scope.tempObject.toDate, 'yyyy-MM-dd');
            $scope.tempObject.fromDate = $filter('date')($scope.tempObject.fromDate, 'yyyy-MM-dd');
            $scope.target.toDate = new Date($scope.target.toDate);
            $scope.target.fromDate = new Date($scope.target.fromDate);
            $scope.title = function () { return 'Cập nhập kỳ khóa sổ'; };
            $scope.save = function () {
                $scope.tempObject.trangThai = $scope.target.trangThai;
                service.update($scope.tempObject).then(function (successRes) {
                    if (successRes && successRes.status == 200 && successRes.statusText === "OK") {
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
    /* setting Period Controller */
    app.controller('settingPeriodController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'periodService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify) {
            $scope.config = angular.copy(configService);
            $scope.tempData = tempDataService.tempData;
            $scope.target = {};
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.filtered = angular.copy(configService.filterDefault);
            $scope.title = function () { return 'Cài đặt kỳ làm việc'; };
            $scope.choiceItem = {};
            $scope.options = {
                minDate: null,
                maxDate: null
            };
            function filterData() {
                $scope.isLoading = true;
                var postdata = { paged: $scope.paged, filtered: $scope.filtered };
                service.postQuery(postdata).then(function (response) {
                    $scope.isLoading = false;
                    if (response.status) {
                        $scope.data = response.data.data;
                        angular.extend($scope.paged, response.data);
                    }
                });
            };
            $scope.setPage = function (pageNo) {
                $scope.paged.currentPage = pageNo;
                filterData();
            };
            $scope.sortType = 'period';
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
            $scope.updateSelection = function (position, data, item, event) {
                if (event.target.checked) {
                    angular.forEach(data, function (subscription, index) {
                        if (position != index)
                            subscription.checked = false;
                        $scope.selected = item;
                        $scope.choiceItem = item;
                        $scope.isSave = true;
                    }
                    );
                }
                else {
                    $scope.choiceItem = {};
                    $scope.isSave = false;
                }
            }
            filterData();
            service.getNewParameter().then(function (response) {
                $scope.filtered.advanceData.fromDate = response.fromDate;
                $scope.filtered.advanceData.toDate = response.toDate;
                $scope.filtered.advanceData.year = response.year;
                $scope.options.minDate = response.minDate;
                $scope.options.maxDate = response.maxDate;
            });
            $scope.save = function () {
                service.update($scope.choiceItem).then(function (successRes) {
                    if (successRes && successRes.status === 200 && successRes.data.status) {
                        ngNotify.set("Khởi tạo kỳ kế toán thành công", { type: 'success' });
                        $uibModalInstance.close($scope.choiceItem);
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
    app.controller('periodDetailsController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'periodService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify) {
            $scope.config = angular.copy(configService);
            $scope.tempData = tempDataService.tempData;
            $scope.target = targetData;
            $scope.target.toDate = new Date($scope.target.toDate);
            $scope.target.fromDate = new Date($scope.target.fromDate);
            $scope.title = function () { return 'Thông tin kỳ khóa sổ'; };
            $scope.cancel = function () {
                $uibModalInstance.close();
            };

        }]);
    /* controller delete */
    app.controller('periodDeleteController', ['$scope', '$uibMoodalInstance', '$location', '$http', 'configService', 'periodService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify',
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
    /* controller updatePriceController */
    app.controller('updatePriceController', ['$scope', '$location', '$uibModalInstance', '$http', 'configService', 'periodService', 'tempDataService', '$filter', '$log', 'ngNotify', 'targetData',
        function ($scope, $location, $uibModalInstance, $http, configService, service, tempDataService, $filter, $log, ngNotify, targetData) {
            $scope.config = angular.copy(configService);
            $scope.target = angular.copy(targetData);
            $scope.options = {
                minDate: null,
                maxDate: null
            };
            $scope.isLoading = false;
            $scope.title = function () { return 'Cập nhật giá vốn từ ngày đến ngày'; };
            $scope.update = function () {
                if (!$scope.target.fromDate) {
                    ngNotify.set("Chưa chọn từ ngày !", { duration: 3000, type: 'error' });
                }
                else if (!$scope.target.toDate) {
                    ngNotify.set("Chưa chọn đến ngày !", { duration: 3000, type: 'error' });
                }
                else if ($scope.target.fromDate > $scope.target.toDate) {
                    ngNotify.set("Từ ngày phải nhỏ hơn Đến ngày !", { duration: 3000, type: 'error' });
                }
                else {
                    $scope.isLoading = true;
                    $scope.target.fromDate = $filter('date')($scope.target.fromDate, 'yyyy-MM-dd');
                    $scope.target.toDate = $filter('date')($scope.target.toDate, 'yyyy-MM-dd');
                    service.updatePrice($scope.target).then(function (response) {
                        if (response && response.status === 200 && response.data && response.data.data && response.data.status) {
                            ngNotify.set("Cập nhật giá vốn thành công", { type: 'success' });
                            $scope.isLoading = false;
                        } else {
                            $scope.isLoading = false;
                            ngNotify.set("Xảy ra lỗi !", { duration: 3000, type: 'error' });
                        }
                    });
                }
            };
            $scope.cancel = function () {
                $uibModalInstance.close();
            };
        }]);

    return app;
});

