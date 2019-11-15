/*  
* Người tạo : Nguyễn Tuấn Hoàng Anh
* Menu: Báo cáo-> Báo cáo xuất bán lẻ
*/
define(['ui-bootstrap',], function () {
    'use strict';
    var app = angular.module('xuatBanLeModule', ['ui.bootstrap']);

    /* controller ShowReportController */
    app.controller('xuatBanLeReportController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'obj',
	function ($scope, $uibModalInstance, $location, $http, configService, obj) {
	    $scope.para = angular.copy(obj);
	    console.log($scope.para);
	    $scope.cancel = function () {
	        $uibModalInstance.close();
	    };
	    $scope.report = {
	        name: "BTS.SP.API.Reports.XUATBANLE.XBANLE_TONGHOP,BTS.SP.API",
	        title: $scope.para.TENBAOCAO,
	        params: $scope.para
	    }
	}]);

    app.controller('xuatBanLeDetailsReportController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'obj',
   function ($scope, $uibModalInstance, $location, $http, configService, obj) {
       $scope.para = angular.copy(obj);
       console.log($scope.para);
       $scope.cancel = function () {
           $uibModalInstance.close();
       };
       $scope.report = {
           name: "BTS.SP.API.Reports.XUATBANLE.XBANLE_CHITIET,BTS.SP.API",
           title: $scope.para.TENBAOCAO,
           params: $scope.para
       }
   }]);
    return app;
});