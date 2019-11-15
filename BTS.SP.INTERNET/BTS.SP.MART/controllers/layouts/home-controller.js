
define(['angular', 'chart-js', 'angular-chart', '/BTS.SP.MART/controllers/auth/AuDonVi.js'], function (angular, chartjs, angularChart) {
    'use strict';
    var app = angular.module('homeModule', ['ui.bootstrap', 'chart.js', 'AuDonViModule']);

    app.config(function (ChartJsProvider) {
        ChartJsProvider.setOptions({ colors: ['#803690', '#00ADF9', '#DCDCDC', '#46BFBD', '#FDB45C', '#949FB1', '#4D5360'] });
    });
    app.factory('homeService', ['$http', 'configService', function ($http, configService) {
        var serviceUrl = configService.rootUrlWebApi + '/Dashboard';
        var rootUrl = configService.apiServiceBaseUri;

        var result = {
            getRetailRevenue: function () {
                return $http.get(serviceUrl + '/GetRetailRevenue');
            },
            getBestOfFiveMerchandiseSelled: function () {
                return $http.get(serviceUrl + '/GetBestOfFiveMerchandiseSelled');
            },
            getAmmountImportToDay: function () {
                return $http.get(serviceUrl + '/GetAmmountImportToDay');
            },
            getAmmountExportToDay: function () {
                return $http.get(serviceUrl + '/GetAmmountExportToDay');
            },
            getCountImportTransactionNotApproved: function () {
                return $http.get(serviceUrl + '/GetCountImportTransactionNotApproved');
            },
            getCountExportTransactionNotApproved: function () {
                return $http.get(serviceUrl + '/GetCountExportTransactionNotApproved');
            },
            getDoanhThuLoaiHang: function () {
                return $http.get(serviceUrl + '/GetDoanhThuLoaiHang');
            },
            getDoanhThuNhomHang: function () {
                return $http.get(serviceUrl + '/GetDoanhThuNhomHang');
            }
        };
        return result;
    }]);

    app.controller('homeCtrl', ['$scope', 'homeService', 'tempDataService', '$filter', 'AuDonViService',
    function ($scope, service, tempDataService, $filter, serviceAuthDonVi) {
        $scope.tempData = tempDataService.tempData;
        $scope.seriesLineChart = [];
        $scope.labelsLineChart = [];
        $scope.dataLineChart = [];
        $scope.labelsPolarChart = [];
        $scope.dataPolarChart = [];
        $scope.merchandiseTypeLabel = [];
        $scope.merchandiseTypeData = [];
        $scope.merchandiseGroupLabel = [];
        $scope.merchandiseGroupData = [];
        function loadAuthDonVi() {
            if (!tempDataService.tempData('auDonVis')) {
                serviceAuthDonVi.getAll_DonVi().then(function (successRes) {
                    if (successRes && successRes.status === 200 && successRes.data.length > 0) {
                        tempDataService.putTempData('auDonVis', successRes.data);
                        $scope.auDonVis = successRes.data;
                    } else {
                        console.log('successRes', successRes);
                    }
                }, function (errorRes) {
                    console.log('errorRes', errorRes);
                });
            } else {
                $scope.auDonVis = tempDataService.tempData('auDonVis');
            }
        }
        loadAuthDonVi();

        function filterData() {
            service.getAmmountImportToDay().then(function (successRes) {
                if (successRes.data.status) {
                    $scope.tongMua = successRes.data.data;
                }
            });
            service.getAmmountExportToDay().then(function (successRes) {
                if (successRes.data.status) {
                    $scope.tongBan = successRes.data.data;
                }
            });
            service.getCountImportTransactionNotApproved().then(function (successRes) {
                if (successRes.data.status) {
                    $scope.soPhieuNhap = successRes.data.data;
                }
            });
            service.getCountExportTransactionNotApproved().then(function (successRes) {
                if (successRes.data.status) {
                    $scope.soPhieuXuat = successRes.data.data;
                }
            });
            service.getRetailRevenue().then(function (successRes) {
                if (successRes.data.status) {
                    angular.forEach(successRes.data.data, function (item) {
                        $scope.seriesLineChart.push(item.unitCode);
                        angular.forEach(item.dataDetails, function (subItem) {
                            $scope.labelsLineChart.push($filter('date')(subItem.ngayCT, 'dd/MM'));
                            $scope.dataLineChart.push(parseFloat(subItem.doanhThu));
                        });
                    });
                }
            });
            service.getBestOfFiveMerchandiseSelled().then(function (successRes) {
                if (successRes.data.status) {
                    angular.forEach(successRes.data.data, function (item) {
                        $scope.labelsPolarChart.push(item.tenVatTu);
                        $scope.dataPolarChart.push(parseFloat(item.giaTri));
                    });
                }
            });
            service.getDoanhThuLoaiHang().then(function (succesRes) {
                if (succesRes.data.status) {
                    succesRes.data.data.forEach(function (obj) {
                        $scope.merchandiseTypeLabel.push(obj.tenVatTu);
                        $scope.merchandiseTypeData.push(obj.giaTri);
                    });
                }
            });
            service.getDoanhThuNhomHang().then(function (succesRes) {
                if (succesRes.data.status) {
                    succesRes.data.data.forEach(function (obj) {
                        $scope.merchandiseGroupLabel.push(obj.tenVatTu);
                        $scope.merchandiseGroupData.push(obj.giaTri);
                    });
                }
            });
        }
        filterData();
        $scope.displayHepler = function (paraValue, moduleName) {
            var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
            if (data && data.length === 1) {
                return data[0].description;
            } else {
                return paraValue;
            }
        }
        //$scope.labels = ["January", "February", "March", "April", "May", "June", "July"];
        //$scope.seriesLineChart = ['Series A', 'Series B'];
        //$scope.data = [
        //  [65, 59, 80, 81, 56, 55, 40],
        //  [28, 48, 40, 19, 86, 27, 90]
        //];
        $scope.onClickLineChart = function (points, evt) {
            console.log(points, evt);
        };
        $scope.datasetOverrideLineChart = { backgroundColor: "rgba( 0,203,254,0.3)" };
        $scope.optionsLineChart = {
            scales: {
                yAxes: [
                  {
                      id: 'y-axis-1',
                      type: 'linear',
                      display: true,
                      position: 'left'
                  },
                  {
                      id: 'y-axis-2',
                      type: 'linear',
                      display: false,
                      position: 'right'
                  }
                ]
            },
            multiTooltipTemplate: function (label) {
                return label.label + ': ' + label.value;
            }
        };

    }]);
    return app;
});

