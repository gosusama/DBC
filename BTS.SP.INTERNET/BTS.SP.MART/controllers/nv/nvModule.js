var nvModule = angular.module('nvModule', ['ui.bootstrap', 'ngFilters', 'ngServices', 'ngResource', 'blockUI', 'xeditable', 'ngAnimate', 'ngFileUpload']);
nvModule.config(['$windowProvider', '$stateProvider', '$httpProvider', '$urlRouterProvider', '$routeProvider', '$locationProvider',
    function ($windowProvider, $stateProvider, $httpProvider, $urlRouterProvider, $routeProvider, $locationProvider) {
        var window = $windowProvider.$get('$window');
        var hostname = window.location.hostname;
        var port = window.location.port;
        var rootUrl = 'http://' + hostname + ':' + port;
        var rootUrl_Layout = '/BTS.SP.MART';
        $stateProvider
            .state('nvTonDauKy',
            {
                url: '/nv/nvTonDauKy',
                parent: 'layout',
                abstract: false,
                views: {
                    'viewMain@root': {
                        templateUrl: rootUrl + rootUrl_Layout + '/NV/NvTonDauKy/index.html',
                        controller: "phieuTonDauKyController as ctrl"
                    }
                }
            })
            .state('nvNhapHangMua',
            {
                url: '/nv/nvNhapHangMua',
                parent: 'layout',
                abstract: false,
                views: {
                    'viewMain@root': {
                        templateUrl: rootUrl + rootUrl_Layout + '/NV/NvNhapHangMua/index.html',
                        controller: "phieuNhapHangMuaController as ctrl"
                    }
                }
            })
            .state('reportPhieuNhapHangMua',
            {
                url: '/nv/nvNhapHangMua/reportNhapHangMua/:id',
                parent: 'layout',
                abstract: false,
                views: {
                    'viewMain@root': {
                        templateUrl: rootUrl + rootUrl_Layout + '/NV/NvNhapHangMua/report.html',
                        controller: "reportPhieuNhapHangMuaController as ctrl"
                    }
                }
            })
            .state('nvNhapKhac',
            {
                url: '/nv/nvNhapKhac',
                parent: 'layout',
                abstract: false,
                views: {
                    'viewMain@root': {
                        templateUrl: rootUrl + rootUrl_Layout + '/NV/NvNhapKhac/index.html',
                        controller: "phieuNhapKhacController as ctrl"
                    }
                }
            })
            .state('nvPhieuDatHangParameter',
            {
                url: '/nv/nvphieuDatHangParameter',
                parent: 'layout',
                abstract: false,
                views: {
                    'viewMain@root': {
                        templateUrl: rootUrl + rootUrl_Layout + '/NV/NvDatHang/param-merger.html',
                        controller: "phieuDatHangParameterController as ctrl"
                    }
                }
            })
            .state('nvKhuyenMai',
            {
                url: '/nv/nvKhuyenMai',
                parent: 'layout',
                abstract: false,
                views: {
                    'viewMain@root': {
                        templateUrl: rootUrl + rootUrl_Layout + '/NV/NvKhuyenMai/index.html',
                        controller: "khuyenMaiController as ctrl"
                    }
                }
            })
            .state('nvPrintDetailKhuyenMai',
            {
                url: '/nv/nvPrintDetailKhuyenMai',
                parent: 'layout',
                abstract: false,
                views: {
                    'viewMain@root': {
                        templateUrl: rootUrl + rootUrl_Layout + '/NV/NvKhuyenMai/printDetail.html',
                        controller: "printDetailKhuyenMaiController as ctrl"
                    }
                }
            })
            .state('nvDatHang',
            {
                url: '/nv/nvDatHang',
                parent: 'layout',
                abstract: false,
                views: {
                    'viewMain@root': {
                        templateUrl: rootUrl + rootUrl_Layout + '/NV/NvDatHang/index.html',
                        controller: "phieuDatHangController as ctrl"
                    }
                }
            })
            .state('nvDatHangNCC',
            {
                url: '/nv/nvDatHangNCC',
                parent: 'layout',
                abstract: false,
                views: {
                    'viewMain@root': {
                        templateUrl: rootUrl + rootUrl_Layout + '/NV/NvDatHangNCC/index.html',
                        controller: "phieuDatHangNCCController as ctrl"
                    }
                }
            })
            .state('reportDatHang',
            {
                url: '/nv/reportDatHang/:id',
                parent: 'layout',
                abstract: false,
                views: {
                    'viewMain@root': {
                        templateUrl: rootUrl + rootUrl_Layout + '/NV/NvDatHang/report.html',
                        controller: "reportDatHangController as ctrl"
                    }
                }
            })
            .state('nvPrintDatHang',
            {
                url: '/nv/nvPrintDatHang',
                parent: 'layout',
                abstract: false,
                views: {
                    'viewMain@root': {
                        templateUrl: rootUrl + rootUrl_Layout + '/NV/NvDatHang/print.html',
                        controller: "printDatHangController as ctrl"
                    }
                }
            })
            .state('nvPrintDetailDatHang',
            {
                url: '/nv/nvPrintDetailDatHang',
                parent: 'layout',
                abstract: false,
                views: {
                    'viewMain@root': {
                        templateUrl: rootUrl + rootUrl_Layout + '/NV/NvDatHang/printDetail.html',
                        controller: "printDetailDatHangController as ctrl"
                    }
                }
            })
            .state('approvalList',
            {
                url: '/nv/nvDatHang/approvalList',
                parent: 'layout',
                abstract: false,
                views: {
                    'viewMain@root': {
                        templateUrl: rootUrl + rootUrl_Layout + '/NV/NvDatHang/index-approval.html',
                        controller: "phieuDatHangApprovalController as ctrl"
                    }
                }
            })
            .state('reportPhieuKiemKe',
            {
                url: '/nv/nvKiemKe/reportPhieuKiemKe/:id',
                parent: 'layout',
                abstract: false,
                views: {
                    'viewMain@root': {
                        templateUrl: rootUrl + rootUrl_Layout + '/NV/NvKiemKe/report.html',
                        controller: "reportPhieuKiemKeController as ctrl"
                    }
                }
            })
            .state('nvPrintDetailPhieuNhapHangMua',
            {
                url: '/nv/nvPrintDetailPhieuNhapMua',
                parent: 'layout',
                abstract: false,
                views: {
                    'viewMain@root': {
                        templateUrl: rootUrl + rootUrl_Layout + '/NV/NvNhapHangMua/printDetail.html',
                        controller: "printDetailPhieuNhapHangMuaController as ctrl"
                    }
                }
            })
            .state('nvPrintPhieuNhapMua',
            {
                url: '/nv/nvPrintPhieuNhapMua',
                parent: 'layout',
                abstract: false,
                views: {
                    'viewMain@root': {
                        templateUrl: rootUrl + rootUrl_Layout + '/NV/NvNhapHangMua/print.html',
                        controller: "printPhieuNhapHangMuaController as ctrl"
                    }
                }
            })
            .state('reportPhieuNhapKhac',
            {
                url: '/nv/nvNhapHangMua/reportPhieuNhapKhac/:id',
                parent: 'layout',
                abstract: false,
                views: {
                    'viewMain@root': {
                        templateUrl: rootUrl + rootUrl_Layout + '/NV/NvNhapKhac/report.html',
                        controller: "reportPhieuNhapKhacController as ctrl"
                    }
                }
            })
            .state('nvPrintDetailPhieuNhapKhac',
            {
                url: '/nv/nvPrintDetailPhieuNhapKhac',
                parent: 'layout',
                abstract: false,
                views: {
                    'viewMain@root': {
                        templateUrl: rootUrl + rootUrl_Layout + '/NV/NvNhapKhac/printDetail.html',
                        controller: "printDetailPhieuNhapKhacController as ctrl"
                    }
                }
            })
            .state('nvPrintPhieuNhapKhac',
            {
                url: '/nv/nvPrintPhieuNhapKhac',
                parent: 'layout',
                abstract: false,
                views: {
                    'viewMain@root': {
                        templateUrl: rootUrl + rootUrl_Layout + '/NV/NvNhapKhac/print.html',
                        controller: "printPhieuNhapKhacController as ctrl"
                    }
                }
            })
            .state('nvPhieuDieuChuyenNoiBo',
            {
                url: '/nv/nvPhieuDieuChuyenNoiBo',
                parent: 'layout',
                abstract: false,
                views: {
                    'viewMain@root': {
                        templateUrl: rootUrl + rootUrl_Layout + '/NV/NvPhieuDieuChuyenNoiBo/index.html',
                        controller: "phieuDieuChuyenNoiBoController as ctrl"
                    }
                }
            })
            .state('nvPhieuDieuChuyenNoiBoNhan',
            {
                url: '/nv/nvPhieuDieuChuyenNoiBoNhan',
                parent: 'layout',
                abstract: false,
                views: {
                    'viewMain@root': {
                        templateUrl: rootUrl + rootUrl_Layout + '/NV/NvPhieuDieuChuyenNoiBo/index-recieve.html',
                        controller: "phieuDieuChuyenNoiBoNhanController as ctrl"
                    }
                }
            })
            .state('nvPrintDetailPhieuDieuChuyenNoiBo',
            {
                url: '/nv/nvPrintDetailPhieuDieuChuyenNoiBo',
                parent: 'layout',
                abstract: false,
                views: {
                    'viewMain@root': {
                        templateUrl: rootUrl + rootUrl_Layout + '/NV/NvPhieuDieuChuyenNoiBo/printDetail.html',
                        controller: "printDetailPhieuDieuChuyenNoiBoController as ctrl"
                    }
                }
            })
            .state('nvPrintPhieuDieuChuyenNoiBo',
            {
                url: '/nv/nvPrintPhieuDieuChuyenNoiBo',
                parent: 'layout',
                abstract: false,
                views: {
                    'viewMain@root': {
                        templateUrl: rootUrl + rootUrl_Layout + '/NV/NvPhieuDieuChuyenNoiBo/print.html',
                        controller: "printPhieuDieuChuyenNoiBoController as ctrl"
                    }
                }
            })
            .state('nvPhieuDieuChuyenNoiBoRecieve',
            {
                url: '/nv/nvPhieuDieuChuyenNoiBoRecieve',
                parent: 'layout',
                abstract: false,
                views: {
                    'viewMain@root': {
                        templateUrl: rootUrl + rootUrl_Layout + '/NV/NvPhieuDieuChuyenNoiBo/recieve.html',
                        controller: "phieuDieuChuyenNoiBoRecieveController as ctrl"
                    }
                }
            })
            .state('reportPhieuDieuChuyenNoiBo',
            {
                url: '/nv/nvPhieuDieuChuyenNoiBo/reportPhieuDieuChuyenNoiBo/:id',
                parent: 'layout',
                abstract: false,
                views: {
                    'viewMain@root': {
                        templateUrl: rootUrl + rootUrl_Layout + '/NV/NvPhieuDieuChuyenNoiBo/report.html',
                        controller: "reportPhieuDieuChuyenNoiBoController as ctrl"
                    }
                }
            })
            .state('reportPhieuDieuChuyenNoiBoNhan',
            {
                url: '/nv/nvPhieuDieuChuyenNoiBo/reportPhieuDieuChuyenNoiBoNhan/:id',
                parent: 'layout',
                abstract: false,
                views: {
                    'viewMain@root': {
                        templateUrl: rootUrl + rootUrl_Layout + '/NV/NvPhieuDieuChuyenNoiBo/report-receive.html',
                        controller: "reportPhieuDieuChuyenNoiBoNhanController as ctrl"
                    }
                }
            })
            .state('nvPhieuXuatNVLSanXuat',
            {
                url: '/nv/nvPhieuXuatNVLSanXuat',
                parent: 'layout',
                abstract: false,
                views: {
                    'viewMain@root': {
                        templateUrl: rootUrl + rootUrl_Layout + '/NV/NvPhieuXuatNVLSanXuat/index.html',
                        controller: "phieuXuatNVLSanXuatController as ctrl"
                    }
                }
            })
            .state('nvAddphieuXuatBanLe',
            {
                url: '/nv/nvAddXuatBanLe',
                parent: 'layout',
                abstract: false,
                views: {
                    'viewMain@root': {
                        templateUrl: rootUrl + rootUrl_Layout + '/NV/NvXuatBanLe/add.html',
                        controller: "phieuXuatBanLeCreateController as ctrl"
                    }
                }
            })
            .state('nvXuatBanLe',
            {
                url: '/nv/nvXuatBanLe',
                parent: 'layout',
                abstract: false,
                views: {
                    'viewMain@root': {
                        templateUrl: rootUrl + rootUrl_Layout + '/NV/NvXuatBanLe/index.html',
                        controller: "phieuXuatBanLeController as ctrl"
                    }
                }
            })
            .state('reportPhieuXuatBanLe',
            {
                url: '/nv/nvXuatBanLe/reportPhieuXuatBanLe/:id',
                parent: 'layout',
                abstract: false,
                views: {
                    'viewMain@root': {
                        templateUrl: rootUrl + rootUrl_Layout + '/NV/NvXuatBanLe/report.html',
                        controller: "reportPhieuXuatBanLeController as ctrl"
                    }
                }
            })
            .state('reportPhieuXuatBan_onlySL',
            {
                url: '/nv/nvXuatBan/reportPhieuXuatBan/:id',
                parent: 'layout',
                abstract: false,
                views: {
                    'viewMain@root': {
                        templateUrl: rootUrl + rootUrl_Layout + '/NV/NvXuatBan/report_onlySL.html',
                        controller: "reportPhieuXuatBanController_onlySL as ctrl"
                    }
                }
            })
            .state('nvPrintDetailPhieuXuatBanLe',
            {
                url: '/nv/nvPrintDetailPhieuXuatBanLe',
                parent: 'layout',
                abstract: false,
                views: {
                    'viewMain@root': {
                        templateUrl: rootUrl + rootUrl_Layout + '/NV/NvXuatBanLe/printDetail.html',
                        controller: "printDetailPhieuXuatBanLeController as ctrl"
                    }
                }
            })
            .state('nvPrintPhieuXuatBanLe',
            {
                url: '/nv/nvPrintPhieuXuatBanLe',
                parent: 'layout',
                abstract: false,
                views: {
                    'viewMain@root': {
                        templateUrl: rootUrl + rootUrl_Layout + '/NV/NvXuatBanLe/print.html',
                        controller: "printPhieuXuatBanLeController as ctrl"
                    }
                }
            })
            .state('nvXuatBan',
            {
                url: '/nv/nvXuatBan',
                parent: 'layout',
                abstract: false,
                views: {
                    'viewMain@root': {
                        templateUrl: rootUrl + rootUrl_Layout + '/NV/NvXuatBan/index.html',
                        controller: "phieuXuatBanController as ctrl"
                    }
                }
            })
            .state('reportPhieuXuatBan', {
                url: '/nv/nvXuatBan/reportPhieuXuatBan/:id',
                parent: 'layout',
                abstract: false,
                views: {
                    'viewMain@root': {
                        templateUrl: rootUrl + rootUrl_Layout + '/NV/NvXuatBan/index.html',
                        controller: "reportPhieuXuatBanController as ctrl"
                    }
                }
            })
            .state('nvPrintDetailPhieuXuatBan', {
                url: '/nv/nvPrintDetailPhieuXuatBan',
                parent: 'layout',
                abstract: false,
                views: {
                    'viewMain@root': {
                        templateUrl: rootUrl + rootUrl_Layout + '/NV/NvXuatBan/printDetail.html',
                        controller: "printDetailPhieuXuatBanController as ctrl"
                    }
                }
            })
            .state('nvPrintPhieuXuatBan', {
                url: '/nv/nvPrintPhieuXuatBan',
                parent: 'layout',
                abstract: false,
                views: {
                    'viewMain@root': {
                        templateUrl: rootUrl + rootUrl_Layout + '/NV/NvXuatBan/print.html',
                        controller: "printPhieuXuatBanController as ctrl"
                    }
                }
            })
            .state('nvXuatKhac',
            {
                url: '/nv/nvXuatKhac',
                parent: 'layout',
                abstract: false,
                views: {
                    'viewMain@root': {
                        templateUrl: rootUrl + rootUrl_Layout + '/NV/NvXuatKhac/index.html',
                        controller: "phieuXuatKhacController as ctrl"
                    }
                }
            })
            .state('reportPhieuXuatKhac',
            {
                url: '/nv/nvXuatBan/reportPhieuXuatKhac/:id',
                parent: 'layout',
                abstract: false,
                views: {
                    'viewMain@root': {
                        templateUrl: rootUrl + rootUrl_Layout + '/NV/NvXuatKhac/report.html',
                        controller: "reportPhieuXuatKhacController as ctrl"
                    }
                }
            })
            .state('nvPrintDetailPhieuXuatKhac',
            {
                url: '/nv/nvPrintDetailPhieuXuatKhac',
                parent: 'layout',
                abstract: false,
                views: {
                    'viewMain@root': {
                        templateUrl: rootUrl + rootUrl_Layout + '/NV/NvXuatKhac/printDetail.html',
                        controller: "printDetailPhieuXuatKhacController as ctrl"
                    }
                }
            })
            .state('nvPrintPhieuXuatKhac',
            {
                url: '/nv/nvPrintPhieuXuatKhac',
                parent: 'layout',
                abstract: false,
                views: {
                    'viewMain@root': {
                        templateUrl: rootUrl + rootUrl_Layout + '/NV/NvXuatKhac/print.html',
                        controller: "printPhieuXuatKhacController as ctrl"
                    }
                }
            })
            .state('nvPhieuNhapHangBanTraLai',
            {
                url: '/nv/nvPhieuNhapHangBanTraLai',
                parent: 'layout',
                abstract: false,
                views: {
                    'viewMain@root': {
                        templateUrl: rootUrl + rootUrl_Layout + '/NV/NvPhieuNhapHangBanTraLai/index.html',
                        controller: "phieuNhapHangBanTraLaiController as ctrl"
                    }
                }
            })
            .state('printPhieuNhapHangBanTraLai',
            {
                url: '/nv/nvPrintPhieuNhapHangBanTraLai',
                parent: 'layout',
                abstract: false,
                views: {
                    'viewMain@root': {
                        templateUrl: rootUrl + rootUrl_Layout + '/NV/NvPhieuNhapHangBanTraLai/print.html',
                        controller: "printNhapHangBanTraLaiController as ctrl"
                    }
                }
            })
            .state('printDetailNhapHangBanTraLaiController',
            {
                url: '/nv/nvPrintDetailPhieuNhapHangBanTraLai',
                parent: 'layout',
                abstract: false,
                views: {
                    'viewMain@root': {
                        templateUrl: rootUrl + rootUrl_Layout + '/NV/NvPhieuNhapHangBanTraLai/printDetail.html',
                        controller: "printDetailNhapHangBanTraLaiController as ctrl"
                    }
                }
            })
            .state('reportPhieuNhapHangBanTraLai',
            {
                url: '/nv/nvXuatBan/reportPhieuNhapHangBanTraLai/:id',
                parent: 'layout',
                abstract: false,
                views: {
                    'viewMain@root': {
                        templateUrl: rootUrl + rootUrl_Layout + '/NV/NvPhieuNhapHangBanTraLai/report.html',
                        controller: "reportPhieuNhapHangBanTraLaiController as ctrl"
                    }
                }
            })
            .state('nvGiaoDichQuay',
            {
                url: '/nv/nvGiaoDichQuay',
                parent: 'layout',
                abstract: false,
                views: {
                    'viewMain@root': {
                        templateUrl: rootUrl + rootUrl_Layout + '/NV/NvGiaoDichQuay/index.html',
                        controller: "giaoDichQuayController as ctrl"
                    }
                }
            })
            .state('nvPrintDetailGiaoDichQuay',
            {
                url: '/nv/nvGiaoDichQuay',
                parent: 'layout',
                abstract: false,
                views: {
                    'viewMain@root': {
                        templateUrl: rootUrl + rootUrl_Layout + '/NV/NvGiaoDichQuay/report.html',
                        controller: "reportGiaoDichQuayController as ctrl",
                        params: {
                            obj: null
                        }
                    }
                }
            })
            .state('reportGiaoDichQuay',
            {
                url: '/nv/nvGiaoDichQuay/reportGiaoDichQuay/:id',
                parent: 'layout',
                abstract: false,
                views: {
                    'viewMain@root': {
                        templateUrl: rootUrl + rootUrl_Layout + '/NV/NvGiaoDichQuay/report.html',
                        controller: "reportGiaoDichQuayController as ctrl"
                    }
                }
            })
            .state('nvKiemKe',
            {
                url: '/nv/nvKiemKe',
                parent: 'layout',
                abstract: false,
                views: {
                    'viewMain@root': {
                        templateUrl: rootUrl + rootUrl_Layout + '/NV/NvKiemKe/index.html',
                        controller: "kiemKeController as ctrl"
                    }
                }
            })
            .state('nvKhuyenMai',
            {
                url: '/nv/nvKhuyenMai',
                parent: 'layout',
                abstract: false,
                views: {
                    'viewMain@root': {
                        templateUrl: rootUrl + rootUrl_Layout + '/NV/NvKhuyenMai/index.html',
                        controller: "khuyenMaiController as ctrl"
                    }
                }
            })
            .state('nvKMDongGia',
            {
                url: '/nv/nvKhuyenMai/DongGia',
                parent: 'layout',
                abstract: false,
                views: {
                    'viewMain@root': {
                        templateUrl: rootUrl + rootUrl_Layout + '/NV/NvKhuyenMai/DongGia/index.html',
                        controller: "kmDongGiaController as ctrl"
                    }
                }
            })
            .state('nvKMVoucher',
            {
                url: '/nv/nvKhuyenMai/Voucher',
                parent: 'layout',
                abstract: false,
                views: {
                    'viewMain@root': {
                        templateUrl: rootUrl + rootUrl_Layout + '/NV/NvKhuyenMai/Voucher/index.html',
                        controller: "kmVoucherController as ctrl"
                    }
                }
            })
            .state('nvKMTinhTien',
            {
                url: '/nv/nvKhuyenMai/TinhTien',
                parent: 'layout',
                abstract: false,
                views: {
                    'viewMain@root': {
                        templateUrl: rootUrl + rootUrl_Layout + '/NV/NvKhuyenMai/TinhTien/index.html',
                        controller: "kmTinhTienController as ctrl"
                    }
                }
            })
            .state('nvKMBuy1Get1',
            {
                url: '/nv/nvKhuyenMai/Buy1Get1',
                parent: 'layout',
                abstract: false,
                views: {
                    'viewMain@root': {
                        templateUrl: rootUrl + rootUrl_Layout + '/NV/NvKhuyenMai/Buy1Get1/index.html',
                        controller: "kmBuy1Get1Controller as ctrl"
                    }
                }
            })
            .state('nvKMCombo',
            {
                url: '/nv/nvKhuyenMai/Combo',
                parent: 'layout',
                abstract: false,
                views: {
                    'viewMain@root': {
                        templateUrl: rootUrl + rootUrl_Layout + '/NV/NvKhuyenMai/Combo/index.html',
                        controller: "kmComboController as ctrl"
                    }
                }
            })
            .state('nvKMTichDiem',
            {
                url: '/nv/nvKhuyenMai/TichDiem',
                parent: 'layout',
                abstract: false,
                views: {
                    'viewMain@root': {
                        templateUrl: rootUrl + rootUrl_Layout + '/NV/NvKhuyenMai/TichDiem/index.html',
                        controller: "kmTichDiemController as ctrl"
                    }
                }
            });
    }]);
nvModule.factory('nvService', [
    '$resource', '$http', '$window', 'clientService', 'configService',
    function ($resource, $http, $window, clientService, configService) {
        var hostname = window.location.hostname;
        var port = window.location.port;
        var rootUrl = 'http://' + hostname + ':' + port;
        var rootUrl_Layout = '/BTS.SP.MART';
        var result = {
            config: configService,
            client: clientService,
        };
        result.buildUrl = function (module, action) {
            return rootUrl + rootUrl_Layout + '/Nv/' + module + '/' + action + '.html';
        };
        return result;
    }]);
nvModule.controller('initController', ['$scope', '$resource', '$rootScope', '$location', '$window', '$uibModal', '$log', '$filter', '$http',
    'configService', 'nvService', 'blockUI', function (
        $scope, $resource, $rootScope, $location, $window, $uibModal, $log, $filter, $http,
        configService, nvService, blockUI) {
        $scope.config = nvService.config;
        $scope.thuTien = function () {
            var modalInstance = $uibModal.open({
                templateUrl: nvService.buildUrl('NvPhieuThuTienMat', 'index'),
                controller: 'phieuThuTienMatController',
                resolve: {},
                size: 'lg'
            });

            modalInstance.result.then(function (updatedData) {
                console.log(updatedData);
            }, function () {
                $log.info('Modal dismissed at: ' + new Date());
            });
        };
        $scope.chiTien = function () {
            var modalInstance = $uibModal.open({
                templateUrl: nvService.buildUrl('NvPhieuChiTienMat', 'index'),
                controller: 'phieuChiTienMatController',
                resolve: {},
                size: 'lg'
            });

            modalInstance.result.then(function (updatedData) {
                console.log(updatedData);
            }, function () {
                $log.info('Modal dismissed at: ' + new Date());
            });

        };
        $scope.uyNhiemChi = function () {
            var modalInstance = $uibModal.open({
                templateUrl: nvService.buildUrl('NvUyNhiemChi', 'index'),
                controller: 'uyNhiemChiController',
                resolve: {},
                size: 'lg'
            });

            modalInstance.result.then(function (updatedData) {
                console.log(updatedData);
            }, function () {
                $log.info('Modal dismissed at: ' + new Date());
            });
        }

    }]);

nvModule.factory('serviceDatHangAndMerchandise', ['$resource', '$http', '$window', 'configService', 'clientService',
    function ($resource, $http, $window, configService, clientService) {
        var rootUrl = configService.rootUrlWeb;
        var urlService = configService.rootUrlWebApi;
        var serviceMerchandiseUrl = urlService + '/api/Md/Merchandise';
        var selectedData = [];
        var result = {
            getSelectData: function () {
                return selectedData;
            },
            setSelectData: function (array) {
                selectedData = array;
            }
        }
        return result;
    }]);
nvModule.factory('serviceDatHangNCCAndMerchandise', ['$resource', '$http', '$window', 'configService', 'clientService',
    function ($resource, $http, $window, configService, clientService) {
        var rootUrl = configService.rootUrlWeb;
        var urlService = configService.rootUrlWebApi;
        var serviceMerchandiseUrl = urlService + '/api/Md/Merchandise';
        var selectedData = [];
        var result = {
            getSelectData: function () {
                return selectedData;
            },
            setSelectData: function (array) {
                selectedData = array;
            }
        }
        return result;
    }]);
//AnhPt service giao dịch quầy -- get data trade
nvModule.factory('serviceGiaoDichQuay', ['$resource', '$http', '$window', 'configService', 'clientService',
    function ($resource, $http, $window, configService, clientService) {
        var rootUrl = configService.rootUrlWeb;
        var urlService = configService.rootUrlWebApi;
        var serviceMerchandiseUrl = urlService + '/api/Nv/GiaoDichQuay';
        var selectedData = [];
        var result = {
            getSelectData: function () {
                return selectedData;
            },
            setSelectData: function (array) {
                selectedData = array;
            }
        }
        return result;
    }]);
//end
nvModule.factory('serviceNhapHangAndMerchandise', ['$resource', '$http', '$window', 'configService', 'clientService',
    function ($resource, $http, $window, configService, clientService) {
        var rootUrl = configService.rootUrlWeb;
        var urlService = configService.rootUrlWebApi;
        var serviceMerchandiseUrl = urlService + '/api/Md/Merchandise';
        var selectedData = [];
        var result = {
            getSelectData: function () {
                return selectedData;
            },
            setSelectData: function (array) {
                selectedData = array;
            }
        }
        return result;
    }]);
nvModule.factory('serviceXuatBanAndMerchandise', ['$resource', '$http', '$window', 'configService', 'clientService',
    function ($resource, $http, $window, configService, clientService) {
        var rootUrl = configService.rootUrlWeb;
        var urlService = configService.rootUrlWebApi;
        var serviceMerchandiseUrl = urlService + '/api/Md/Merchandise';
        var selectedData = [];
        var result = {
            getSelectData: function () {
                return selectedData;
            },
            setSelectData: function (array) {
                selectedData = array;
            }
        }
        return result;
    }]);
nvModule.factory('serviceDieuChuyenAndMerchandise', ['$resource', '$http', '$window', 'configService', 'clientService',
    function ($resource, $http, $window, configService, clientService) {
        var rootUrl = configService.rootUrlWeb;
        var urlService = configService.rootUrlWebApi;
        var serviceMerchandiseUrl = urlService + '/api/Md/Merchandise';
        var selectedData = [];
        var result = {
            getSelectData: function () {
                return selectedData;
            },
            setSelectData: function (array) {
                selectedData = array;
            }
        }
        return result;
    }]);
nvModule.factory('serviceBanHangTraLaiAndMerchandise', ['$resource', '$http', '$window', 'configService', 'clientService',
    function ($resource, $http, $window, configService, clientService) {
        var rootUrl = configService.rootUrlWeb;
        var urlService = configService.rootUrlWebApi;
        var serviceMerchandiseUrl = urlService + '/api/Md/Merchandise';
        var selectedData = [];
        var result = {
            getSelectData: function () {
                return selectedData;
            },
            setSelectData: function (array) {
                selectedData = array;
            }
        }
        return result;
    }]);
nvModule.factory('serviceTonDauKyAndMerchandise', ['$resource', '$http', '$window', 'configService', 'clientService',
    function ($resource, $http, $window, configService, clientService) {
        var rootUrl = configService.rootUrlWeb;
        var urlService = configService.rootUrlWebApi;
        var serviceMerchandiseUrl = urlService + '/api/Md/Merchandise';
        var selectedData = [];
        var result = {
            getSelectData: function () {
                return selectedData;
            },
            setSelectData: function (array) {
                selectedData = array;
            }
        }
        return result;
    }]);
nvModule.factory('serviceNhapKhacAndMerchandise', ['$resource', '$http', '$window', 'configService', 'clientService',
    function ($resource, $http, $window, configService, clientService) {
        var rootUrl = configService.rootUrlWeb;
        var urlService = configService.rootUrlWebApi;
        var serviceMerchandiseUrl = urlService + '/api/Md/Merchandise';

        var selectedData = [];
        var result = {
            getSelectData: function () {
                return selectedData;
            },
            setSelectData: function (array) {
                selectedData = array;
            }
        }
        return result;
    }]);
nvModule.factory('serviceNhapHangAndCustomer', ['$resource', '$http', '$window', 'configService', 'clientService',
    function ($resource, $http, $window, configService, clientService) {
        var rootUrl = configService.rootUrlWeb;
        var urlService = configService.rootUrlWebApi;
        var selectedData = [];
        var result = {
            getSelectData: function () {
                return selectedData;
            },
            setSelectData: function (array) {
                selectedData = array;
            }
        }
        return result;
    }]);
nvModule.factory('serviceXuatKhacAndMerchandise', ['$resource', '$http', '$window', 'configService', 'clientService',
    function ($resource, $http, $window, configService, clientService) {
        var rootUrl = configService.rootUrlWeb;
        var urlService = configService.rootUrlWebApi;
        var serviceMerchandiseUrl = urlService + '/api/Md/Merchandise';
        var selectedData = [];
        var result = {
            getSelectData: function () {
                return selectedData;
            },
            setSelectData: function (array) {
                selectedData = array;
            }
        }
        return result;
    }]);
nvModule.factory('serviceXuatBanLeAndMerchandise', ['$resource', '$http', '$window', 'configService', 'clientService',
    function ($resource, $http, $window, configService, clientService) {
        var rootUrl = configService.rootUrlWeb;
        var urlService = configService.rootUrlWebApi;
        var serviceMerchandiseUrl = urlService + '/api/Md/Merchandise';
        var selectedData = [];
        var result = {
            getSelectData: function () {
                return selectedData;
            },
            setSelectData: function (array) {
                selectedData = array;
            }
        }
        return result;
    }]);
nvModule.factory('serviceKhuyenMaiAndMerchandise', ['$resource', '$http', '$window', 'configService', 'clientService',
    function ($resource, $http, $window, configService, clientService) {
        var rootUrl = configService.rootUrlWeb;
        var urlService = configService.rootUrlWebApi;
        var serviceMerchandiseUrl = urlService + '/api/Md/Merchandise';
        var selectedData = [];
        var result = {
            getSelectData: function () {
                return selectedData;
            },
            setSelectData: function (array) {
                selectedData = array;
            }
        }
        return result;
    }]);
nvModule.factory('serviceKhuyenMaiAndMerchandiseGift', ['$resource', '$http', '$window', 'configService', 'clientService',
    function ($resource, $http, $window, configService, clientService) {
        var rootUrl = configService.rootUrlWeb;
        var urlService = configService.rootUrlWebApi;
        var serviceMerchandiseUrl = urlService + '/api/Md/Merchandise';
        var selectedData = [];
        var result = {
            getSelectData: function () {
                return selectedData;
            },
            setSelectData: function (array) {
                selectedData = array;
            }
        }
        return result;
    }]);
nvModule.factory('serviceDatHangAndCustomer', ['$resource', '$http', '$window', 'configService', 'clientService',
    function ($resource, $http, $window, configService, clientService) {
        var rootUrl = configService.rootUrlWeb;
        var urlService = configService.rootUrlWebApi;
        var serviceMerchandiseUrl = urlService + '/api/Md/Merchandise';
        var selectedData = [];
        var result = {
            getSelectData: function () {
                return selectedData;
            },
            setSelectData: function (array) {
                selectedData = array;
            }
        }
        return result;
    }]);
nvModule.factory('serviceDatHangAndType', ['$resource', '$http', '$window', 'configService', 'clientService',
    function ($resource, $http, $window, configService, clientService) {
        var rootUrl = configService.rootUrlWeb;
        var urlService = configService.rootUrlWebApi;
        var selectedData = [];
        var result = {
            getSelectData: function () {
                return selectedData;
            },
            setSelectData: function (array) {
                selectedData = array;
            }
        }
        return result;
    }]);
nvModule.factory('serviceDatHangAndGroup', ['$resource', '$http', '$window', 'configService', 'clientService',
    function ($resource, $http, $window, configService, clientService) {
        var rootUrl = configService.rootUrlWeb;
        var urlService = configService.rootUrlWebApi;
        var selectedData = [];
        var result = {
            getSelectData: function () {
                return selectedData;
            },
            setSelectData: function (array) {
                selectedData = array;
            }
        }
        return result;
    }]);
nvModule.factory('serviceKhuyenMaiAndWareHouse', ['$resource', '$http', '$window', 'configService', 'clientService',
    function ($resource, $http, $window, configService, clientService) {
        var selectedData = [];
        var result = {
            getSelectData: function () {
                return selectedData;
            },
            setSelectData: function (array) {
                selectedData = array;
            }
        }
        return result;
    }]);
nvModule.factory('serviceXuatBanAndDatHang', ['$resource', '$http', '$window', 'configService', 'clientService',
    function ($resource, $http, $window, configService, clientService) {
        var selectedData = [];
        var result = {
            getSelectData: function () {
                return selectedData;
            },
            setSelectData: function (array) {
                selectedData = array;
            }
        }
        return result;
    }]);