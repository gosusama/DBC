/*  
* Người tạo : Nguyễn Tuấn Hoàng Anh
* Menu: Báo cáo-> Báo cáo tồn kho theo từng ngày
*/
define(['ui-bootstrap', '/BTS.SP.MART/controllers/auth/AuthController.js', '/BTS.SP.MART/controllers/htdm/periodController.js', '/BTS.SP.MART/controllers/htdm/merchandiseController.js', '/BTS.SP.MART/controllers/htdm/customerController.js', '/BTS.SP.MART/controllers/htdm/merchandiseTypeController.js', '/BTS.SP.MART/controllers/htdm/nhomVatTuController.js', '/BTS.SP.MART/controllers/htdm/supplierController.js', '/BTS.SP.MART/controllers/htdm/wareHouseController.js', '/BTS.SP.MART/controllers/htdm/packagingController.js', '/BTS.SP.MART/controllers/htdm/taxController.js', '/BTS.SP.MART/controllers/htdm/donViTinhController.js'], function () {
    'use strict';
    var app = angular.module('tonKhoModule', ['ui.bootstrap', 'authModule', 'periodModule', 'merchandiseModule', 'customerModule', 'merchandiseTypeModule', 'nhomVatTuModule', 'supplierModule', 'wareHouseModule', 'packagingModule', 'taxModule', 'donViTinhModule']);
    app.factory('tonKhoService', ['$http', 'configService', function ($http, configService) {
    }]);
    /* controller Nhập mua */
    app.controller('nhapMuaReportController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'obj',
	function ($scope, $uibModalInstance, $location, $http, configService, obj) {
	    $scope.para = angular.copy(obj);
	    $scope.cancel = function () {
	        $uibModalInstance.close();
	    };
	    $scope.report = {
	        name: "BTS.SP.API.Reports.NHAPMUA.NHAPMUA_TONGHOP,BTS.SP.API",
	        title: $scope.para.TENBAOCAO,
	        params: $scope.para
	    }
	}]);

    app.controller('nhapMuaDetailsReportController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'obj',
	function ($scope, $uibModalInstance, $location, $http, configService, obj) {
	    $scope.para = angular.copy(obj);
	    $scope.cancel = function () {
	        $uibModalInstance.close();
	    };
	    $scope.report = {
	        name: "BTS.SP.API.Reports.NHAPMUA.NHAPMUA_CHITIET,BTS.SP.API",
	        title: $scope.para.TENBAOCAO,
	        params: $scope.para
	    }
	}]);

    /*Nhập bán buôn trả lại*/
    app.controller('nhapBanBuonTraLaiReportController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'obj',
	function ($scope, $uibModalInstance, $location, $http, configService, obj) {
	    $scope.para = angular.copy(obj);
	    $scope.cancel = function () {
	        $uibModalInstance.close();
	    };
	    $scope.report = {
	        name: "BTS.SP.API.Reports.NHAPBANBUONTRALAI.NHAPBANBUONTRALAI_TONGHOP,BTS.SP.API",
	        title: $scope.para.TENBAOCAO,
	        params: $scope.para
	    }
	}]);
    app.controller('nhapBanBuonTraLaiDetailsReportController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'obj',
	function ($scope, $uibModalInstance, $location, $http, configService, obj) {
	    $scope.para = angular.copy(obj);
	    $scope.cancel = function () {
	        $uibModalInstance.close();
	    };
	    $scope.report = {
	        name: "BTS.SP.API.Reports.NHAPBANBUONTRALAI.NHAPBANBUONTRALAI_CHITIET,BTS.SP.API",
	        title: $scope.para.TENBAOCAO,
	        params: $scope.para
	    }
	}]);

    /*Nhập điều chuyển nội bộ*/
    app.controller('nhapDieuChuyenNoiBoReportController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'obj',
	function ($scope, $uibModalInstance, $location, $http, configService, obj) {
	    $scope.para = angular.copy(obj);
	    $scope.cancel = function () {
	        $uibModalInstance.close();
	    };
	    $scope.report = {
	        name: "BTS.SP.API.Reports.NHAPDIEUCHUYENNOIBO.NHAPDIEUCHUYENNOIBO_TONGHOP,BTS.SP.API",
	        title: $scope.para.TENBAOCAO,
	        params: $scope.para
	    }
	}]);

    app.controller('nhapDieuChuyenNoiBoDetailsReportController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'obj',
	function ($scope, $uibModalInstance, $location, $http, configService, obj) {
	    $scope.para = angular.copy(obj);
	    $scope.cancel = function () {
	        $uibModalInstance.close();
	    };
	    $scope.report = {
	        name: "BTS.SP.API.Reports.NHAPDIEUCHUYENNOIBO.NHAPDIEUCHUYENNOIBO_CHITIET,BTS.SP.API",
	        title: $scope.para.TENBAOCAO,
	        params: $scope.para
	    }
	}]);
    /*Nhập khác*/
    app.controller('nhapKhacReportController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'obj',
	function ($scope, $uibModalInstance, $location, $http, configService, obj) {
	    $scope.para = angular.copy(obj);
	    $scope.cancel = function () {
	        $uibModalInstance.close();
	    };
	    $scope.report = {
	        name: "BTS.SP.API.Reports.NHAPKHAC.NHAPKHAC_TONGHOP,BTS.SP.API",
	        title: $scope.para.TENBAOCAO,
	        params: $scope.para
	    }
	}]);
    app.controller('nhapKhacDetailsReportController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'obj',
	function ($scope, $uibModalInstance, $location, $http, configService, obj) {
	    $scope.para = angular.copy(obj);
	    $scope.cancel = function () {
	        $uibModalInstance.close();
	    };
	    $scope.report = {
	        name: "BTS.SP.API.Reports.NHAPKHAC.NHAPKHAC_CHITIET,BTS.SP.API",
	        title: $scope.para.TENBAOCAO,
	        params: $scope.para
	    }
	}]);
    return app;
});