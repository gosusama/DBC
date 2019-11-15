define(['ui-bootstrap', '/BTS.SP.MART/controllers/auth/AuNguoiDungNhomQuyen.js', '/BTS.SP.MART/controllers/auth/AuNhomQuyen.js', '/BTS.SP.MART/controllers/auth/AuNguoiDungQuyen.js', '/BTS.SP.MART/controllers/auth/AuthController.js'], function () {
    'use strict';
    var app = angular.module('AuNguoiDungModule', ['ui.bootstrap', 'AuNguoiDungNhomQuyenModule', 'AuNhomQuyenModule', 'AuNguoiDungQuyenModule', 'authModule', 'angular-md5']);
    app.factory('AuNguoiDungService', [
        '$http', 'configService', function ($http, configService) {
            var serviceUrl = configService.rootUrlWebApi + '/Authorize/AuNguoiDung';
            var selectedData = [];
            var result = {
                postQuery: function (data, callback) {
                    $http.post(serviceUrl + '/PostQuery', data).success(callback);
                },
                postSelectData: function (data) {
                    return $http.post(serviceUrl + '/PostSelectData', data);
                },
                post: function (data, callback) {
                    $http.post(serviceUrl + '/Post', data).success(callback);
                },
                getAll_NguoiDung: function () {
                    return $http.get(serviceUrl + '/GetSelectData');
                },
                getCurrentUser: function (callback) {
                    $http.get(serviceUrl + '/GetCurrentUser').success(callback);
                },
                update: function (params) {
                    return $http.put(serviceUrl + '/' + params.id, params);
                },
                deleteItem: function (params) {
                    return $http.delete(serviceUrl + '/' + params.id, params);
                },
                checkUserNameExist: function (params, callback) {
                    $http.get(serviceUrl + '/CheckUserNameExist/' + params).success(callback);
                },
                getUserByProfile: function (params) {
                    return $http.get(serviceUrl + '/GetUserByProfile/' + params);
                },
                getUserByUsername: function (params) {
                    return $http.get(serviceUrl + '/GetUserByUsername/' + params);
                },
                getNewInstance: function (unitCode) {
                    return $http.get(serviceUrl + '/GetNewInstance/' + unitCode);
                },
                getSelectData: function () {
                    return selectedData;
                },
                setSelectData: function (array) {
                    selectedData = array;
                },
            }
            return result;
        }
    ]);
    app.controller('AuNguoiDungViewCtrl', ['$scope', 'configService', 'AuNguoiDungService', 'tempDataService', '$uibModal', '$log', 'securityService', 'toaster',
        function ($scope, configService, service, tempDataService, $uibModal, $log, securityService, toaster) {
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.filtered = angular.copy(configService.filterDefault);
            $scope.tempData = tempDataService.tempData;
            $scope.accessList = {};
            function filterData() {
                if ($scope.accessList.view) {
                    $scope.isLoading = true;
                    var postdata = { paged: $scope.paged, filtered: $scope.filtered };
                    service.postQuery(
                        JSON.stringify(postdata),
                        function (response) {
                            $scope.isLoading = false;
                            if (response.status) {
                                $scope.data = response.data.data;
                                angular.extend($scope.paged, response.data);

                            }
                        });
                }
            };

            function loadAccessList() {
                securityService.getAccessList('AuNguoiDung').then(function (successRes) {
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
                    toaster.pop('error', "Lỗi:", "Không có quyền truy cập !");
                    $scope.accessList = null;
                });
            }
            loadAccessList();
            $scope.setPage = function (pageNo) {
                $scope.paged.currentPage = pageNo;
                filterData();
            };
            $scope.sortType = 'userName'; // set the default sort type
            $scope.sortReverse = false; // set the default sort order
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
                return 'Tài khoản người dùng';
            };

            $scope.create = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    size: 'lg',
                    templateUrl: configService.buildUrl('auth/AuNguoiDung', 'add'),
                    controller: 'AuNguoiDungCreateController',
                    resolve: {
                    }
                });
                modalInstance.result.then(function (updatedData) {
                    $scope.refresh();
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };


            $scope.details = function (target) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    size: 'lg',
                    templateUrl: configService.buildUrl('auth/AuNguoiDung', 'details'),
                    controller: 'AuNguoiDungDetailsController',
                    resolve: {
                        targetData: function () {
                            return target;
                        }
                    }
                });
            };
            $scope.update = function (target) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('auth/AuNguoiDung', 'update'),
                    controller: 'AuNguoiDungEditController',
                    resolve: {
                        targetData: function () {
                            return target;
                        }
                    }
                });
                modalInstance.result.then(function (updatedData) {
                    var index = $scope.data.indexOf(target);
                    if (index !== -1) {
                        $scope.data[index] = updatedData;
                    }
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };

            $scope.addVaiTro = function (item) {
                $uibModal.open({
                    backdrop: 'static',
                    size: 'md',
                    templateUrl: configService.buildUrl('auth/AuNguoiDungNhomQuyen', 'add'),
                    controller: 'AuNguoiDungNhomQuyenCreateCtrl',
                    resolve: {
                        targetData: function () {
                            return item;
                        }
                    }
                });
            };

            $scope.addQuyen = function (item) {
                $uibModal.open({
                    backdrop: 'static',
                    size: 'lg',
                    windowClass: 'app-modal-window',
                    templateUrl: configService.buildUrl('auth/AuNguoiDungQuyen', 'add'),
                    controller: 'AuNguoiDungQuyenCreateCtrl',
                    resolve: {
                        targetData: function () {
                            return item;
                        }
                    }
                });
            };

            /* Function Delete Item */
            $scope.deleteItem = function (event, target) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('auth/AuNguoiDung', 'delete'),
                    controller: 'AuNguoiDungDeleteController',
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
            $scope.changePass = function (target) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    size: 'sm',
                    templateUrl: configService.buildUrl('auth/AuNguoiDung', 'changePassword'),
                    controller: 'AuNguoiDungChangePasswordController',
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
            }
        }
    ]);

    app.controller('AuNguoiDungDetailsController', ['$scope', '$uibModalInstance', 'configService', 'targetData', 'tempDataService',
        function ($scope, $uibModalInstance, configService, targetData, tempDataService) {
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.filtered = angular.copy(configService.filterDefault);
            $scope.tempData = tempDataService.tempData;
            $scope.target = targetData;
            $scope.displayHepler = function (paraValue, moduleName) {
                var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                if (data && data.length === 1) {
                    return data[0].text;
                } else {
                    return paraValue;
                }
            }
            $scope.title = function () { return 'Thông tin người dùng'; };
            $scope.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };
        }
    ]);
    app.controller('AuNguoiDungCreateController', [
        '$scope', '$uibModalInstance', 'AuNguoiDungService', 'configService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'userService',
        function ($scope, $uibModalInstance, service, configService, tempDataService, $filter, $uibModal, $log, ngNotify, serviceAuthUser) {
            var currentUser = serviceAuthUser.GetCurrentUser();
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.filtered = angular.copy(configService.filterDefault);
            $scope.tempData = tempDataService.tempData;
            $scope.isExist = false;
            service.getNewInstance(currentUser.parentUnitCode).then(function (response) {
                if (response && response.status == 200 && response.data) {
                    $scope.target.maNhanVien = response.data;
                }
            });
            $scope.enterUserNameProfiles = function (input) {
                if (input) {
                    service.checkUserNameExist(input, function (response) {
                        if (response.status) {
                            ngNotify.set("Đã tồn tại tên người dùng này !", { duration: 3000, type: 'error' });
                            $scope.isExist = true;
                        } else {
                            $scope.isExist = false;
                        }
                    });

                }
            };
            $scope.save = function () {
                service.post(
                    JSON.stringify($scope.target),
                    function (response) {
                        //Fix
                        if (response.status) {
                            console.log('Create  Successfully!');
                            ngNotify.set("Thêm mới thành công", { type: 'success' });
                            $uibModalInstance.close($scope.target);

                        } else {
                            ngNotify.set(response.message, { duration: 3000, type: 'error' });
                        }
                        //End fix
                    });
            };
            $scope.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };
        }
    ]);
    app.controller('AuNguoiDungEditController', [
        '$scope', '$uibModalInstance', 'AuNguoiDungService', 'configService', 'tempDataService', 'ngNotify', 'targetData',
        function ($scope, $uibModalInstance, service, configService, tempDataService, ngNotify, targetData) {
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.filtered = angular.copy(configService.filterDefault);
            $scope.tempData = tempDataService.tempData;
            targetData.userName = targetData.username;
            $scope.target = targetData;
            $scope.title = function () { return 'Cập nhập người dùng'; };
            $scope.enterUserNameProfiles = function (input) {
                if (input) {
                    service.checkUserNameExist(input, function (response) {
                        if (response.status) {
                            ngNotify.set("Đã tồn tại tên người dùng này !", { duration: 3000, type: 'error' });
                        } else {
                        }
                    });
                }
            };
            $scope.save = function () {
                service.update($scope.target).then(
                    function (response) {
                        if (response.status && response.status === 200) {
                            if (response.data.status) {
                                console.log('Create  Successfully!');
                                ngNotify.set("Cập nhập thành công", { type: 'success' });
                                $uibModalInstance.close($scope.target);
                            } else {

                            }
                        } else {
                            console.log('ERROR: Update failed! ' + response.errorMessage);
                            ngNotify.set(response.errorMessage, { duration: 3000, type: 'error' });
                        }
                    },
                    function (response) {
                        console.log('ERROR: Update failed! ' + response);
                    });
            };
            $scope.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };
        }
    ]);
    app.controller('AuNguoiDungSelectDataController', ['$scope', '$uibModalInstance', 'configService', 'AuNguoiDungService', 'filterObject', 'serviceSelectData',
        function ($scope, $uibModalInstance, configService, service, filterObject, serviceSelectData) {
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.filtered = angular.copy(configService.filterDefault);
            angular.extend($scope.filtered, filterObject);
            $scope.sortType = 'username'; // set the default sort type
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
                    postdata.filtered.isAdvance = false;
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
                    postdata.filtered.isAdvance = false;
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
                return 'Danh sách người dùng';
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
    /* controller delete */
    app.controller('AuNguoiDungDeleteController', ['$scope', '$uibModalInstance', 'configService', 'AuNguoiDungService', 'targetData', 'ngNotify',
        function ($scope, $uibModalInstance, configService, service, targetData, ngNotify) {
            $scope.config = angular.copy(configService);
            $scope.targetData = angular.copy(targetData);
            $scope.target = targetData;
            $scope.isLoading = false;
            $scope.title = function () { return 'Xoá thành phần'; };
            $scope.save = function () {
                service.deleteItem($scope.target).then(function (successRes) {
                    if (successRes && successRes.status === 200) {
                        ngNotify.set("Xóa thành công", { type: 'success' });
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
    /* controller saleRetails */
    app.controller('saleRetails', [
        '$scope', function ($scope) {
            var hostname = window.location.hostname;
            var port = window.location.port;
            var rootUrl = 'http://' + hostname + ':' + port;
            var unitCode = '';
            $scope.nvRetails = function () {
                var url = rootUrl + '/BTS.SP.MART/NV/BanLeHangHoa/index.html';
                var newWindow = window.open(url, 'Bán hàng');

                var urlTraTon = rootUrl + '/BTS.SP.MART/NV/BanLeHangHoa/traton.html';
                var newWindowTraTon = window.open(urlTraTon, 'Tra tồn hàng hóa');
            }
        }
    ]);

    app.controller('AuNguoiDungChangePasswordController', ['$scope', '$uibModalInstance', 'configService', 'AuNguoiDungService', 'targetData', 'ngNotify', 'md5',
        function ($scope, $uibModalInstance, configService, service, targetData, ngNotify, md5) {
            $scope.config = angular.copy(configService);
            $scope.targetData = angular.copy(targetData);
            $scope.target = targetData;
            $scope.isLoading = false;
            $scope.target.checkPassOld = false;
            $scope.isConfirmPass = true;
            $scope.target.changePassSuccess = false;
            $scope.title = function () { return 'Đổi mật khẩu'; };
            function filterData() {
            }
            $scope.checkPassOld = function (passOld) {
                var pass = md5.createHash(passOld);
                if (pass == $scope.target.password) {
                    $scope.target.checkPassOld = true;
                }
                else {
                    $scope.target.checkPassOld = false;
                }
            }
            $scope.checkPassConfirm = function (passNewConfirm) {
                if (passNewConfirm == $scope.target.passwordNew) {
                    $scope.isConfirmPass = true;
                }
                else {
                    $scope.isConfirmPass = false;
                }
            }
            $scope.checkSuccess = function (passNewConfirm) {
                if (passNewConfirm == $scope.target.passwordNew) {
                    $scope.target.changePassSuccess = true;
                }
                else {
                    $scope.target.changePassSuccess = false;
                }
            }

            $scope.save = function () {
                $scope.target.password = md5.createHash($scope.target.passwordNew);
                service.update($scope.target).then(
                    function (response) {
                        if (response.status && response.status === 200) {
                            if (response.data.status) {
                                console.log('Create  Successfully!');
                                ngNotify.set("Cập nhập thành công", { type: 'success' });
                                $uibModalInstance.close($scope.target);
                            } else {

                            }
                        } else {
                            console.log('ERROR: Update failed! ' + response.errorMessage);
                            ngNotify.set(response.errorMessage, { duration: 3000, type: 'error' });
                        }
                    },
                    function (response) {
                        console.log('ERROR: Update failed! ' + response);
                    });
            };
            $scope.cancel = function () {
                $uibModalInstance.close();
            };
        }]);
    return app;
});