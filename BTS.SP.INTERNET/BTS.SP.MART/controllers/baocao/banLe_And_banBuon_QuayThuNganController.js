/*  
* Người tạo : HuyNQ
* Menu: Báo cáo-> Báo cáo xuất bán lẻ
*/
define(['ui-bootstrap',], function () {
    'use strict';
    var app = angular.module('banLe_And_banBuon_QuayThuNganModule', ['ui.bootstrap']);

    /* controller ShowReportController */
    app.controller('banLe_And_banBuon_QuayThuNganReportController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'obj',
	function ($scope, $uibModalInstance, $location, $http, configService, obj) {
	    $scope.para = angular.copy(obj);
	    $scope.cancel = function () {
	        $uibModalInstance.close();
	    };
	    $scope.report = {
	        name: "BTS.SP.API.Reports.BANLE_AND_BANBUON_QUAYTHUNGAN.BANLE_AND_BANBUON_QUAYTHUNGAN_TONGHOP,BTS.SP.API",
	        title: $scope.para.TENBAOCAO,
	        params: $scope.para
	    }
	}]);

    app.controller('banLe_And_banBuon_QuayThuNganDetailsReportController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'obj',
	function ($scope, $uibModalInstance, $location, $http, configService, obj) {
	    $scope.para = angular.copy(obj);
	    $scope.cancel = function () {
	        $uibModalInstance.close();
	    };
	    $scope.report = {
	        name: "BTS.SP.API.Reports.BANLE_AND_BANBUON_QUAYTHUNGAN.BANLE_AND_BANBUON_QUAYTHUNGAN_CHITIET,BTS.SP.API",
	        title: $scope.para.TENBAOCAO,
	        params: $scope.para
	    }
	}]);
    return app;
});