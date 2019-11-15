/*  
* Người tạo : Nguyễn Tuấn Hoàng Anh
* Menu: Báo cáo-> Báo cáo tồn kho theo từng ngày
*/
define(['ui-bootstrap'], function () {
    'use strict';
    var app = angular.module('xuatKhacModule', ['ui.bootstrap']);
    app.factory('xuatKhacService', ['$http', 'configService', function ($http, configService) {
    }]);
    /* controller ShowReportController */
    app.controller('xuatKhacReportController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'obj',
	function ($scope, $uibModalInstance, $location, $http, configService, obj) {
	    $scope.para = angular.copy(obj);
	    $scope.para.P_PHUONGTHUCXUAT = "1";
	    $scope.cancel = function () {
	        $uibModalInstance.close();
	    };
	    $scope.report = {
	        name: "BTS.SP.API.Reports.XUATKHAC.XKHAC_TONGHOP,BTS.SP.API",
	        title: $scope.para.TENBAOCAO,
	        params: $scope.para
	    }
	}]);

    app.controller('xuatKhacDetailsReportController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'obj',
	function ($scope, $uibModalInstance, $location, $http, configService, obj) {
	    $scope.para = angular.copy(obj);
	    $scope.para.P_PHUONGTHUCXUAT = "1";
	    $scope.cancel = function () {
	        $uibModalInstance.close();
	    };
	    $scope.report = {
	        name: "BTS.SP.API.Reports.XUATKHAC.XKHAC_CHITIET,BTS.SP.API",
	        title: $scope.para.TENBAOCAO,
	        params: $scope.para
	    }
	}]);
    return app;
});