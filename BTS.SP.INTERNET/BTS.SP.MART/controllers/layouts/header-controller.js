define(['angular', '/BTS.SP.MART/controllers/auth/AuthController.js', '/BTS.SP.MART/controllers/auth/AuMenu.js', '/BTS.SP.MART/controllers/htdm/periodController.js', '/BTS.SP.MART/controllers/auth/AuNguoiDung.js', '/BTS.SP.MART/controllers/nv/phieuDatHangController.js'], function (angular) {
    var app = angular.module('headerModule', ['authModule', 'configModule', 'AuMenuModule', 'periodModule', 'AuNguoiDungModule', 'phieuDatHangModule']);
    var layoutUrl = "/BTS.SP.MART/";

    app.directive('tree', function () {
        return {
            restrict: "E",
            replace: true,
            scope: {
                tree: '='
            },
            templateUrl: layoutUrl + 'utils/tree/template-ul.html'
        };
    });
    app.directive('leaf', function ($compile) {
        return {
            restrict: "E",
            replace: true,
            scope: {
                leaf: "="
            },
            templateUrl: layoutUrl + 'utils/tree/template-li.html',
            link: function (scope, element, attrs) {
                if (angular.isArray(scope.leaf.Children) && scope.leaf.Children.length > 0) {
                    element.append("<tree tree='leaf.Children'></tree>");
                    element.addClass('dropdown-submenu');
                    $compile(element.contents())(scope);
                }
            }
        };
    });
    app.controller('HeaderCtrl', ['$scope', '$uibModal', 'configService', '$state', 'accountService', '$log', 'userService', 'AuMenuService', 'periodService', 'toaster', 'AuNguoiDungService', 'securityService', '$window', 'phieuDatHangService',
    function ($scope, $uibModal, configService, $state, accountService, $log, userService, auMenuService, periodService, toaster, AuNguoiDungService, securityService, $window, phieuDatHangService) {
        //var socket = $window.socket;
        var appID = $window.appId;
        $scope.rootUrlHome = configService.rootUrl;
        $scope.countMessage = 0;
        $scope.countOrderNew = 0;
        function treeify(list, idAttr, parentAttr, childrenAttr) {
            if (!idAttr) idAttr = 'value';
            if (!parentAttr) parentAttr = 'parent';
            if (!childrenAttr) childrenAttr = 'children';
            var lookup = {};
            var result = {};
            result[childrenAttr] = [];
            list.forEach(function (obj) {
                lookup[obj[idAttr]] = obj;
                obj[childrenAttr] = [];
            });
            list.forEach(function (obj) {
                if (obj[parentAttr] != null) {
                    try { lookup[obj[parentAttr]][childrenAttr].push(obj); }
                    catch (err) {
                        result[childrenAttr].push(obj);
                    }

                } else {
                    result[childrenAttr].push(obj);
                }
            });
            return result;
        };
        $scope.linkHref = function () {
            $state.go('home');
        };
        function filterData() {
            $scope.target = {};
            $scope.isCompare = true;
            periodService.getKyKeToan().then(function (response) {
                if (response && response.status == 200 && response.data) {
                    $scope.target = response.data;
                    var now = new Date();
                    var datePeriod = new Date($scope.target.toDate); //Year, Month, Date
                    if (now.getDay() === datePeriod.getDay()) {
                        $scope.isCompare = true;
                    } else {
                        $scope.isCompare = false;
                    }
                }
            });
            phieuDatHangService.countOrderNew().then(function (response) {
                if (response.status === 200) {
                    $scope.countOrderNew = response.data;
                }
            });
        }
        $scope.dateNow = new Date();
        //lấy thông tin ngày khóa sổ
        $scope.nextPeriod = function (item) {
            if (item != null) {
                $scope.itemNext = {};
                //lấy thông tin kỳ tiếp theo -- khóa sổ
                periodService.getNextPeriod(item).then(function (responseNext) {
                    if (responseNext && responseNext.status == 200 && responseNext.data) {
                        $scope.isDisabled = true;
                        $scope.stateIsRunning = true;
                        $scope.itemNext = responseNext.data;
                        periodService.postApproval($scope.itemNext).then(function (response) {
                            if (response && response.status == 200 && response.data) {
                                console.log('Create  Successfully!');
                                toaster.pop('success', "Thông báo", response.message, 2000);
                                $scope.isDisabled = false;
                                filterData();
                            } else {
                                $scope.isDisabled = false;
                                toaster.pop('error', "Lỗi:", response.message);
                            }
                            $scope.stateIsRunning = false;
                            //End fix
                        }).error(function (error) {
                            $scope.isDisabled = false;
                            $scope.stateIsRunning = false;
                        });
                    }
                });
                //end
            }

        };

        $scope.viewRecieve = function () {
            $state.go('phieuDatHang');
        }

        function loadUser() {
            $scope.currentUser = userService.GetCurrentUser();
            if (!$scope.currentUser) {
                $state.go('login');
            } else {
                auMenuService.getMenu($scope.currentUser.userName).then(function (rspMenu) {
                    if (rspMenu && rspMenu.status === 200 && rspMenu.data && rspMenu.data.data && rspMenu.data.data.length > 0) {
                        rspMenu.data.data.extendValue = 0;
                        $scope.treeMenu = treeify(rspMenu.data.data);
                        filterData();
                    }
                });
            }
        }
        loadUser();
        $scope.logOut = function () {
            accountService.logout();
        };
        $scope.downloadApp = function () {
            $scope.linkDownload = configService.apiServiceBaseUri + '/Upload/TBNet.Mart.Sub.Client.application';
            window.open($scope.linkDownload);
        };
        $scope.changePass = function () {
            var currentUser = userService.GetCurrentUser();
            if (currentUser) {
                var target = {};
                AuNguoiDungService.getUserByUsername(currentUser.userName).then(function (res) {
                    target = res.data.data;
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
                    }, function () {
                        $log.info('Modal dismissed at: ' + new Date());
                    });
                });

            }
        }
    }]);
    return app;
});