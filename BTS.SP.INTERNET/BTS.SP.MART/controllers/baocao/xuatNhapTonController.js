/*  
* Người tạo : Nguyễn Tuấn Hoàng Anh
* Menu: Báo cáo-> Báo cáo tồn kho theo từng ngày
*/
define(['ui-bootstrap'], function () {
    'use strict';
    var app = angular.module('xuatNhapTonModule', ['ui.bootstrap']);
    app.factory('xuatNhapTonService', ['$http', 'configService', function ($http, configService) {
    }]);
    /* controller ShowReportController */
    app.controller('xuatNhapTonReportController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'obj',
	function ($scope, $uibModalInstance, $location, $http, configService, obj) {
	    $scope.para = angular.copy(obj);
	    console.log($scope.para);
	    $scope.cancel = function () {
	        $uibModalInstance.close();
	    };
	    if ($scope.para) {
	        switch ($scope.para.P_GROUPBY) {
	            case '0':
	                $scope.para.P_GROUPBY = 'MADONVI';
	                break;
	            case '1':
	                $scope.para.P_GROUPBY = 'MAKHO';
	                break;
	            case '2':
	                $scope.para.P_GROUPBY = 'MALOAIVATTU';
	                break;
	            case '3':
	                $scope.para.P_GROUPBY = 'MANHOMVATTU';
	                break;
	            case '4':
	                $scope.para.P_GROUPBY = 'MAVATTU';
	                break;
	            case '5':
	                $scope.para.P_GROUPBY = 'MAKHACHHANG';
	                break;
	            default:
	                $scope.para.P_GROUPBY = 'MAKHO';
	        }
	    }
	    console.log($scope.para);
	    $scope.para.P_TUNGAY = new Date(Date.UTC($scope.para.tempTuNgay.getFullYear(), $scope.para.tempTuNgay.getMonth(), $scope.para.tempTuNgay.getDate()));
	    $scope.para.P_DENNGAY = new Date(Date.UTC($scope.para.tempDenNgay.getFullYear(), $scope.para.tempDenNgay.getMonth(), $scope.para.tempDenNgay.getDate()));
	    $scope.report = {
	        name: "BTS.SP.API.Reports.XUATNHAPTON.XUATNHAPTON_TONGHOP,BTS.SP.API",
	        title: $scope.para.TENBAOCAO,
	        params: $scope.para
	    }
	}]);

    app.controller('xuatNhapTonDetailsReportController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'obj',
	function ($scope, $uibModalInstance, $location, $http, configService, obj) {
	    $scope.para = angular.copy(obj);
	    console.log($scope.para);
	    $scope.cancel = function () {
	        $uibModalInstance.close();
	    };
	    if ($scope.para) {
	        switch ($scope.para.P_GROUPBY) {
	            case '0':
	                $scope.para.P_GROUPBY = 'MADONVI';
	                break;
	            case '1':
	                $scope.para.P_GROUPBY = 'MAKHO';
	                break;
	            case '2':
	                $scope.para.P_GROUPBY = 'MALOAIVATTU';
	                break;
	            case '3':
	                $scope.para.P_GROUPBY = 'MANHOMVATTU';
	                break;
	            case '4':
	                $scope.para.P_GROUPBY = 'MAVATTU';
	                break;
	            case '5':
	                $scope.para.P_GROUPBY = 'MAKHACHHANG';
	                break;
	            default:
	                $scope.para.P_GROUPBY = 'MAKHO';
	        }
	    }
	    $scope.para.P_TUNGAY = new Date(Date.UTC($scope.para.tempTuNgay.getFullYear(), $scope.para.tempTuNgay.getMonth(), $scope.para.tempTuNgay.getDate()));
	    $scope.para.P_DENNGAY = new Date(Date.UTC($scope.para.tempDenNgay.getFullYear(), $scope.para.tempDenNgay.getMonth(), $scope.para.tempDenNgay.getDate()));
	    $scope.report = {
	        name: "BTS.SP.API.Reports.XUATNHAPTON.XUATNHAPTON_CHITIET,BTS.SP.API",
	        title: $scope.para.TENBAOCAO,
	        params: $scope.para
	    }
	}]);
    return app;
});