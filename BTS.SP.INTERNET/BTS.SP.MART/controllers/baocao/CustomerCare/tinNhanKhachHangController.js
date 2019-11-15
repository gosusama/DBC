/*  
* Người tạo : Phạm Tuấn Anh
* View: BTS.SP.MART/views/DCL/CustomerCare
* Vm sevices: BTS.API.SERVICE -> DCL ->CustomerCareVm.cs
* Sevices: BTS.API.SERVICE -> DCL -> CustomerCareService.cs
* Entity: BTS.API.ENTITY -> DCL - > CustomerCare.cs
* Menu: Nghiệp vụ-> khách hàng tần suất mua hàng
*/
define(['ui-bootstrap', 'controllers/auth/AuthController', 'controllers/htdm/periodController', 'controllers/htdm/merchandiseController', 'controllers/htdm/customerController', 'controllers/htdm/merchandiseTypeController', 'controllers/htdm/nhomVatTuController', 'controllers/htdm/supplierController', 'controllers/htdm/wareHouseController', 'controllers/htdm/packagingController', 'controllers/htdm/taxController', 'controllers/htdm/donViTinhController'], function () {
    'use strict';
    var app = angular.module('tinNhanKhachHangModule', ['ui.bootstrap', 'authModule', 'periodModule', 'merchandiseModule', 'customerModule', 'merchandiseTypeModule', 'nhomVatTuModule', 'supplierModule', 'wareHouseModule', 'packagingModule', 'taxModule', 'donViTinhModule']);
    app.factory('tinNhanKhachHangService', ['$rootScope', '$window', function ($rootScope, $window) {
        //Creating connection with server

        //This part is only for login users for authenticated socket connection between client and server.
        //If you are not using login page in you website then you should remove rest piece of code..

        return '';
    }]);
    /* controller list */
    app.controller('tinNhanKhachHangController', [
        '$scope', '$location', '$http', 'configService', 'tinNhanKhachHangService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'securityService', '$rootScope', 'toaster', '$window', '$anchorScroll',
        function ($scope, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, securityService, $rootScope, toaster, $window, $anchorScroll) {
            //end check
            var socket = $window.socket;
            var appID = $window.appId;
            $scope.dataDTO = {
                AppId: appID,
                Receive: 'cskh'
            }
            var access = false;
            var messageSelected = '';
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.filtered = angular.copy(configService.filterDefault);
            $scope.tempData = tempDataService.tempData;
            $scope.isEditable = true;
            $scope.accessList = {};
            $scope.target = {};
            $scope.tagWareHouses = [];
            $scope.tagCustomers = [];
            $scope.lstMessage = [];
            $scope.lstMessageDetail = [];
            $scope.options = {
                minDate: null,
                maxDate: null
            };
            filterData();
            $scope.selectMessage = function (item) {
                messageSelected = item._id;
                $scope.titleMessage = item._id;
                $scope.lstMessage.forEach(function (obj) {
                    if (obj._id === item._id) {
                        obj.isSelected = true;
                    } else {
                        obj.isSelected = false;
                    }
                });
                var obj = {
                    AppId: appID,
                    Name: item._id,
                    CSKH: 'cskh',
                }
                socket.emit('select-message-by-name', obj);
            }
            socket.emit('get-message', $scope.dataDTO);

            socket.on('response-get-list-custom-online', function (res) {
                console.log(res);
            });

            socket.on('response-select-message-by-name', function (res) {
                if (res) {
                    $scope.lstMessageDetail = angular.copy(res);
                    $location.hash(($scope.lstMessageDetail.length-1).toString());

                    // call $anchorScroll()
                    $anchorScroll();
                    $scope.$apply();
                }
            });
            socket.on('response-get-message', function (data) {
                if (data) {
                    $scope.lstMessage = data;
                    $scope.lstMessage.forEach(function (obj) {
                        if (obj.data) {
                            obj.count = 0;
                            obj.isSelected = false;
                            obj.data.forEach(function (item) {
                                if (!item.Status) {
                                    obj.count++;
                                }
                            });
                        }
                    });
                }
            })

            socket.on('request-message-person', function (data) {
                var obj = {
                    ContentMessage: data.ContentMessage,
                    Time: data.Time,
                    SendBy: data.SendBy,
                    DateTime: data.DateTime,
                    idSend: data.id,
                    Receive: data.Receive
                }
                var index = 0;
                $scope.lstMessage.forEach(function (item) {
                    if (item._id === data.SendBy) {
                        item.count += 1;
                    }
                    if (item.isSelected === true && item._id === data.SendBy) {
                        $scope.lstMessageDetail.push(obj);
                    }
                });
                $scope.$apply();
            });

            socket.on('response-update-status', function (data) {
                if (data) {
                    $scope.lstMessage.forEach(function (item) {
                        if (item.isSelected === true) {
                            item.count = 0;
                        }
                    });
                }
            });

            $scope.displayDate = function (item) {
                var date = new Date(item.DateTime + ' ' + item.Time);
                return date;
            }

            //load dữ liệu
            function filterData() {

            };
            //end

            $scope.sendMessage = function (item) {
                var obj = {
                    SendBy: 'cskh',
                    Receive: messageSelected,
                    ContentMessage: item,
                    AppId: appID,
                    DateTime: convertDateNow(),
                    Time: convertDateNowToHour(),
                }
                $scope.lstMessageDetail.push(obj);
                $location.hash(($scope.lstMessageDetail.length - 1).toString());

                // call $anchorScroll()
                $anchorScroll();
                socket.emit('reply-message-custommer', obj);
            }

            function convertDateNow() {
                var date = new Date();
                return date.getFullYear() + '-' + (date.getMonth() + 1) + '-' + date.getDate();
            }

            function convertDateNowToHour() {
                var date = new Date();
                return date.getHours() + ':' + date.getMinutes();
            }


            //check quyền truy cập
            function loadAccessList() {
                securityService.getAccessList('tinNhanKhachHang').then(function (successRes) {
                    if (successRes && successRes.status === 200) {
                        $scope.accessList = successRes.data;
                        if (!$scope.accessList.view) {
                            toaster.pop('error', "Lỗi:", "Không có quyền truy cập !");
                        } else {
                            filterData();
                            access = true;
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

        }]);
    return app;
});

