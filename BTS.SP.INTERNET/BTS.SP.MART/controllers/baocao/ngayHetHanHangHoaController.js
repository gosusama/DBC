/*  
* Người tạo : Nguyễn Tuấn Hoàng Anh
* Menu: Báo cáo-> Báo cáo tồn kho theo từng ngày
*/
define(['ui-bootstrap', '/BTS.SP.MART/controllers/auth/AuthController.js', '/BTS.SP.MART/controllers/htdm/periodController.js', '/BTS.SP.MART/controllers/htdm/merchandiseController.js', '/BTS.SP.MART/controllers/htdm/merchandiseTypeController.js', '/BTS.SP.MART/controllers/htdm/nhomVatTuController.js', '/BTS.SP.MART/controllers/htdm/supplierController.js'], function () {
    'use strict';
    var app = angular.module('xuatBanBuonModule', ['ui.bootstrap', 'authModule', 'periodModule', 'merchandiseModule', 'merchandiseTypeModule', 'nhomVatTuModule', 'supplierModule']);
    app.factory('xuatBanBuonService', ['$http', 'configService', function ($http, configService) {
    }]);
    /* controller ShowReportController */
    app.controller('ngayHetHanHangHoaReportController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'obj',
	function ($scope, $uibModalInstance, $location, $http, configService, obj) {
		$scope.para = angular.copy(obj);
	    $scope.cancel = function () {
	        $uibModalInstance.close();
	    };

	    $scope.report = {
	        name: "BTS.SP.API.Reports.NGAYHETHANHANGHOA.NGAYHETHANHANGHOA,BTS.SP.API",
	        title: $scope.para.TENBAOCAO,
	        params: $scope.para
	    }
	}]);
    return app;
});