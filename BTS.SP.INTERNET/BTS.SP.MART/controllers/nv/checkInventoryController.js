var inventoryApps = angular.module('inventoryApp', ['ngResource', 'ngServices', 'ui.bootstrap', 'ui.router', 'ngRoute', 'angular-loading-bar', 'ngDirectives', 'ngMaterial', 'ngTagsInput', 'ngFilters', 'ngHandlers', 'ngSanitize', 'mdModule', 'nvModule', 'authorizeModule', 'ngCookies', 'FBAngular']);
inventoryApps.config([
    '$windowProvider', '$stateProvider', '$httpProvider', '$urlRouterProvider', '$routeProvider', '$locationProvider', '$provide', 'cfpLoadingBarProvider',
    function ($windowProvider, $stateProvider, $httpProvider, $urlRouterProvider, $routeProvider, $locationProvider, $provide, cfpLoadingBarProvider) {
        var window = $windowProvider.$get('$window');
        cfpLoadingBarProvider.includeSpinner = false;
        $httpProvider.defaults.transformResponse.push(function (responseData) {
            return responseData;
        });
        $httpProvider.interceptors.push('httpInterceptor');
    }
]);

inventoryApps.factory('checkInventoryService', [
    '$resource', '$http', '$window', 'configService', 'clientService',
    function ($resource, $http, $window, configService, clientService) {
        var rootUrl = configService.rootUrlWeb;
        var serviceUrl = configService.rootUrlWebApi + '/Nv/TraTon';
        var result = {
            //lấy dữ liệu kỳ khóa sổ
            getPeriod: function (callback) {
                return $http.get(serviceUrl + '/GetPeriodDate').success(callback);
            },
            checkInventory: function (code, callback) {
                return $http.get(serviceUrl + '/CheckInventory/' + code).success(callback);
            },
            bindingDataHangHoa: function (strKey, callback) {
                return $http.get(serviceUrl + '/BindingDataHangHoa/' + strKey).success(callback);
            }
        }
        return result;
    }
]);

inventoryApps.controller('inventoryCheckController', [
    '$scope', '$rootScope', '$location', '$window', '$uibModal', '$log', '$filter', '$http', 'mdService', 'nvService', 'blockUI', 'checkInventoryService',
	'configService', 'clientService', '$mdDialog', 'serviceMerchandise', '$rootScope',
    function ($scope, $rootScope, $location, $window, $uibModal, $log, $filter, $http, mdService, nvService, blockUI, checkInventoryService,
        configService, clientService, $mdDialog, serviceMerchandise, $rootScope) {
        console.log($rootScope.currentUser);
        $scope.paged = angular.copy(configService.pageDefault);
        $scope.filtered = angular.copy(configService.filterDefault);
        $scope.data = {};
        $scope.target = {};
        function showNotification(message) {
            if (message != '') {
                $scope.message = message;
                var popup = document.getElementById('popupMessage').style.display = 'block';
                $("#popupMessage").delay(1500).fadeOut(300);
            }
        }
        checkInventoryService.getPeriod(function (response) {
            console.log(response);
        });
        $scope.filterData = function (item) {
            $scope.showResult = false;
            if (item) {
                checkInventoryService.bindingDataHangHoa(item, function (response) {
                    if (response.status && response.data.length > 0) {
                        $scope.result = response.data;
                        $scope.showResult = true;
                        angular.extend($scope.paged, response.data);
                    }
                    else {
                        //continue
                    }
                });
            }
        }
        $scope.changeHangHoa = function (item) {
            $scope.showResult = false;
            $scope.target.maVatTu = item.maVatTu;
            filterData($scope.target.maVatTu);
        };
        $scope.changeMerchandise = function (item) {
            $scope.showResult = false;
            if (item) {
                filterData(item);
            }
        };

        $scope.pageChanged = function () {
            var currentPage = $scope.paged.currentPage;
            var itemsPerPage = $scope.paged.itemsPerPage;
            $scope.paged.totalItems = $scope.listData.length;
            $scope.data = [];
            if ($scope.listData) {
                for (var i = (currentPage - 1) * itemsPerPage; i < currentPage * itemsPerPage && i < $scope.listData.length; i++) {
                    $scope.data.push($scope.listData[i]);
                }
                if ($scope.data.length > 0) {
                    $scope.listTemp = angular.copy($scope.data);
                    $scope.listTemp.splice(0, 1);
                }
            }
        }
        function filterData(code) {
            checkInventoryService.checkInventory(code, function (response) {
                if (response.data && response.data.length > 0 && response.status) {
                    console.log(response);
                    $scope.listData = response.data;
                }
                $scope.pageChanged();
            });
        }
    }
]);