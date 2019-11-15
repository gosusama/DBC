/*  
* Người tạo : Nguyễn Tuấn Hoàng Anh
* Menu: Báo cáo-> Báo cáo tồn kho theo từng ngày
*/
define(['ui-bootstrap'], function () {
    'use strict';
    var app = angular.module('xuatNhapTonChiTietModule', ['ui.bootstrap']);
    app.factory('xuatNhapTonChiTietService', ['$http', 'configService', function ($http, configService) {
    }]);
    /* controller ShowReportController */
    app.controller('xuatNhapTonChiTietReportController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'obj',
	function ($scope, $uibModalInstance, $location, $http, configService, obj) {
	    $scope.para = angular.copy(obj);
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
	        name: "BTS.SP.API.Reports.XUATNHAPTON_CHITIET.XUATNHAPTON_CHITIET_TONGHOP,BTS.SP.API",
	        title: $scope.para.TENBAOCAO,
	        params: $scope.para
	    }
	}]);

    app.controller('xuatNhapTonChiTietDetailsReportController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'obj',
	function ($scope, $uibModalInstance, $location, $http, configService, obj) {
	    $scope.para = angular.copy(obj);
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
	        name: "BTS.SP.API.Reports.XUATNHAPTON_CHITIET.XUATNHAPTON_CHITIET_CHITIET,BTS.SP.API",
	        title: $scope.para.TENBAOCAO,
	        params: $scope.para
	    }
	}]);
    return app;
});