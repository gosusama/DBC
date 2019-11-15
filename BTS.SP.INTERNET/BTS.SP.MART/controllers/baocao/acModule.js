var acModule = angular.module('acModule', ['ui.bootstrap', 'ngFilters', 'ngServices', 'ngResource', 'blockUI', 'xeditable']);
acModule.config(['$windowProvider', '$stateProvider', '$httpProvider', '$urlRouterProvider', '$routeProvider', '$locationProvider',
    function ($windowProvider, $stateProvider, $httpProvider, $urlRouterProvider, $routeProvider, $locationProvider) {
        var window = $windowProvider.$get('$window');
        var hostname = window.location.hostname;
        var port = window.location.port;
        var rootUrl = 'http://' + hostname + ':' + port;
        $stateProvider
        .state('generalJournal',
        {
            url: '/ac/generalJournal',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: rootUrl + '/BTS.SP.MART/AC/GeneralJournal/index.html',
                    controller: "generalJournalController as ctrl"
                }
            }
        })
        .state('subsidiaryLedger',
        {
            url: '/ac/subsidiaryLedger',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: rootUrl + '/BTS.SP.MART/AC/SubsidiaryLedger/index.html',
                    controller: "subsidiaryLedgerController as ctrl"
                }
            }
        })
        .state('cashBook',
        {
            url: '/ac/cashBook',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: rootUrl + '/BTS.SP.MART/AC/CashBook/index.html',
                    controller: "cashBookController as ctrl"
                }
            }
        })
        .state('closeout',
        {
            url: '/ac/closeout',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: rootUrl + '/BTS.SP.MART/AC/Closeout/index.html',
                    controller: "closeoutController as ctrl"
                }
            }
        }) 
        //Báo cáo xuất, nhập, tồn theo ngày
        .state('inventory',
        {
            url: '/ac/inventory',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: rootUrl + '/BTS.SP.MART/AC/Inventory/index.html',
                    controller: "inventoryController as ctrl",
                    params: {
                        obj: null
                    }
                }
            }
        })
        .state('inventoryReport',
        {
            url: '/ac/inventoryReport',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: rootUrl + '/BTS.SP.MART/AC/Inventory/report.html',
                    controller: "reportInventoryController as ctrl",
                    params: {
                        obj: null
                    }
                }
            }
        })
        .state('reportInventoryDetail',
        {
            url: '/ac/inventoryDetailReport',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: rootUrl + '/BTS.SP.MART/AC/ImportExport/reportInventoryDetail.html',
                    controller: "reportInventoryDetailController as ctrl",
                    params: {
                        obj: null
                    }
                }
            }
        })
        //end Báo cáo xuất, nhập, tồn theo ngày
        //Báo cáo xuất, nhập, tồn
        .state('importExport',
        {
            url: '/ac/importExport',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: rootUrl + '/BTS.SP.MART/AC/ImportExport/index.html',
                    controller: "importExportController as ctrl",
                    params: {
                        obj: null
                    }
                }
            }
        })
        .state('reportImportExport',
        {
            url: '/ac/reportImportExport',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: rootUrl + '/BTS.SP.MART/AC/ImportExport/report.html',
                    controller: "reportImportExportController as ctrl",
                    params: {
                        obj: null
                    }
                }
            }
        })
        //end Báo cáo xuất, nhập, tồn

        //Phạm tuấn anh báo cáo Xuất nhập tồn chi tiết ngày 16/05/2017
        .state('baoCaoXuaNhapTonChiTiet',
        {
            url: '/ac/baoCaoXuaNhapTonChiTiet',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: rootUrl + '/BTS.SP.MART/AC/BaoCaoXuatNhapTonChiTiet/index.html',
                    controller: "baoCaoXuaNhapTonChiTietController as ctrl"
                }
            }
        })
        .state('reportXNTNewTongHop',
        {
            url: '/ac/reportXNTNewTongHop',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: rootUrl + '/BTS.SP.MART/AC/BaoCaoXuatNhapTonChiTiet/reportXNTNewTongHop.html',
                    controller: "XNTNewTongHopReportController as ctrl",
                    params: {
                        obj: null
                    }
                }
            }
        })
        .state('reportXNTNewChiTiet',
        {
            url: '/ac/reportXNTNewChiTiet',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: rootUrl + '/BTS.SP.MART/AC/BaoCaoXuatNhapTonChiTiet/reportXNTNewChiTiet.html',
                    controller: "XNTNewChiTietReportController as ctrl",
                    params: {
                        obj: null
                    }
                }
            }
        })
        //end Xuất nhập tồn chi tiết

        //Báo cáo bán lẻ
        .state('cashier',
        {
            url: '/ac/cashier',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: rootUrl + '/BTS.SP.MART/AC/Cashier/index.html',
                    controller: "cashierController as ctrl",
                    params: {
                        obj: null
                    }
                }
            }
        })
        .state('cashierReport',
        {
            url: '/ac/cashierReport',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: rootUrl + '/BTS.SP.MART/AC/Cashier/report.html',
                    controller: "reportCashierController as ctrl",
                    params: {
                        obj: null
                    }
                }
            }
        })
        //end Báo cáo bán lẻ
        .state('giaoDichQuayReport',
        {
            url: '/ac/giaoDichQuayReport',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: rootUrl + '/BTS.SP.MART/AC/Cashier/giaoDichQuayReport.html',
                    controller: "giaoDichQuayReportController as ctrl",
                    params: {
                        obj: null
                    }
                }
            }
        })
        .state('theoNhaCungCap',
        {
            url: '/ac/theoNhaCungCap',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: rootUrl + '/BTS.SP.MART/AC/TheoNhaCungCap/index.html',
                    controller: "theoNhaCungCapController as ctrl"
                }
            }
        })
        .state('theoNhaCungCapReport',
        {
            url: '/ac/theoNhaCungCap',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: rootUrl + '/BTS.SP.MART/AC/TheoNhaCungCap/report.html',
                    controller: "reportTheoNhaCungCapController as ctrl"
                }
            }
        })
        .state('baoCaoNhapMua',
        {
            url: '/ac/acNhapMua',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: rootUrl + '/BTS.SP.MART/AC/AcNhapMua/index.html',
                    controller: "acNhapMuaController as ctrl"
                }
            }
        })
        .state('reportNhapMua',
        {
            url: '/ac/reportNhapMua',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: rootUrl + '/BTS.SP.MART/AC/AcNhapMua/report.html',
                    controller: "reportNhapMuaController as ctrl",
                    params: {
                        obj: null
                    }
                }
            }
        })
        .state('reportDieuChuyenNhan',
        {
            url: '/ac/reportDieuChuyenNhan',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: rootUrl + '/BTS.SP.MART/AC/AcNhapMua/reportDieuChuyenNhan.html',
                    controller: "reportDieuChuyenNhanController as ctrl",
                    params: {
                        obj: null
                    }
                }
            }
        })
        .state('baoCaoXuatBan',
        {
            url: '/ac/acXuatBan',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: rootUrl + '/BTS.SP.MART/AC/AcXuatBan/index.html',
                    controller: "acXuatBanController as ctrl"
                }
            }
        })
        .state('xuatBanReport',
        {
            url: '/ac/xuatBanReport',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: rootUrl + '/BTS.SP.MART/AC/AcXuatBan/xuatBanReport.html',
                    controller: "reportXuatBanController as ctrl",
                    params: {
                        obj: null
                    }
                }
            }
        })
        .state('xuatDieuChuyenReport',
        {
            url: '/ac/xuatDieuChuyenReport',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: rootUrl + '/BTS.SP.MART/AC/AcXuatBan/xuatDieuChuyenReport.html',
                    controller: "reportXuatDieuChuyenController as ctrl",
                    params: {
                        obj: null
                    }
                }
            }
        })
        .state('xuatKhacReport',
        {
            url: '/ac/xuatKhacReport',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: rootUrl + '/BTS.SP.MART/AC/AcXuatBan/xuatKhacReport.html',
                    controller: "reportXuatKhacController as ctrl",
                    params: {
                        obj: null
                    }
                }
            }
        })
        .state('baoCaoKiemKe',
        {
            url: '/ac/acKiemKe',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: rootUrl + '/BTS.SP.MART/AC/AcKiemKe/index.html',
                    controller: "acKiemKeController as ctrl"
                }
            }
        })
        .state('kiemKeReport',
        {
            url: '/ac/kiemKeReport',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: rootUrl + '/BTS.SP.MART/AC/AcKiemKe/kiemKeReport.html',
                    controller: "reportKiemKeController as ctrl",
                    params: {
                        obj: null
                    }
                }
            }
        })
        // Báo cáo dặt hàng
        .state('baoCaoDatHang',
        {
            url: '/ac/baoCaoDatHang',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: rootUrl + '/BTS.SP.MART/AC/AcDatHang/index.html',
                    controller: "acDatHangController as ctrl",
                    params: {
                        obj: null
                    }
                }
            }
        })
        .state('baoCaoDatHangReport',
        {
            url: '/ac/baoCaoDatHangReport',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: rootUrl + '/BTS.SP.MART/AC/AcDatHang/report.html',
                    controller: "reportAcDatHangController as ctrl",
                    params: {
                        obj: null
                    }
                }
            }
        })
        //end module báo cáo

        //Module Chăm sóc khách hàng

        //tần suất mua hàng
        .state('baoCaoTanSuatMuaHang',
        {
            url: '/ac/baoCaoTanSuatMuaHang',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: rootUrl + '/BTS.SP.MART/AC/TanSuatMuaHang/index.html',
                    controller: "tanSuatMuaHangController as ctrl",
                    params: {
                        obj: null
                    }
                }
            }
        })
        .state('customerLevelUp',
        {
            url: '/ac/customerLevelUp',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: rootUrl + '/BTS.SP.MART/AC/CustomerCare/CustomerLevelUp/index.html',
                    controller: "customerLevelUpController as ctrl",
                    params: {
                        obj: null
                    }
                }
            }
        })
        .state('customerLevelUpReport',
        {
            url: '/ac/customerLevelUpReport',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: rootUrl + '/BTS.SP.MART/AC/CustomerCare/CustomerLevelUp/report.html',
                    controller: "reportCustomerLevelUpController as ctrl",
                    params: {
                        obj: null
                    }
                }
            }
        })
        .state('notBuyCustomer',
        {
            url: '/ac/notBuyCustomer',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: rootUrl + '/BTS.SP.MART/AC/CustomerCare/NotBuyCustomer/index.html',
                    controller: "notBuyCustomerController as ctrl",
                    params: {
                        obj: null
                    }
                }
            }
        })
        .state('notBuyCustomerReport',
        {
            url: '/ac/notBuyCustomerReport',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: rootUrl + '/BTS.SP.MART/AC/CustomerCare/NotBuyCustomer/report.html',
                    controller: "reportNotBuyCustomerController as ctrl",
                    params: {
                        obj: null
                    }
                }
            }
        })
        .state('forgetCardCustomer',
        {
            url: '/ac/forgetCardCustomer',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: rootUrl + '/BTS.SP.MART/AC/CustomerCare/ForgetCardCustomer/index.html',
                    controller: "forgetCardCustomerController as ctrl",
                    params: {
                        obj: null
                    }
                }
            }
        })
        .state('forgetCardCustomerReport',
        {
            url: '/ac/forgetCardCustomerReport',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: rootUrl + '/BTS.SP.MART/AC/CustomerCare/ForgetCardCustomer/report.html',
                    controller: "reportForgetCardCustomerController as ctrl",
                    params: {
                        obj: null
                    }
                }
            }
        })
        .state('historyGiveCardCustomer',
        {
            url: '/ac/historyGiveCardCustomer',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: rootUrl + '/BTS.SP.MART/AC/CustomerCare/HistoryGiveCardCustomer/index.html',
                    controller: "historyGiveCardCustomerController as ctrl",
                    params: {
                        obj: null
                    }
                }
            }
        })
        .state('historyGiveCardCustomerReport',
        {
            url: '/ac/historyGiveCardCustomerReport',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: rootUrl + '/BTS.SP.MART/AC/CustomerCare/HistoryGiveCardCustomer/report.html',
                    controller: "reportHistoryGiveCardCustomerController as ctrl",
                    params: {
                        obj: null
                    }
                }
            }
        })
        .state('beChangedCardCustomer',
        {
            url: '/ac/beChangedCardCustomer',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: rootUrl + '/BTS.SP.MART/AC/CustomerCare/BeChangedCardCustomer/index.html',
                    controller: "beChangedCardCustomerController as ctrl",
                    params: {
                        obj: null
                    }
                }
            }
        })
        .state('beChangedCardCustomerReport',
        {
            url: '/ac/beChangedCardCustomerReport',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: rootUrl + '/BTS.SP.MART/AC/CustomerCare/BeChangedCardCustomer/report.html',
                    controller: "reportBeChangedCardCustomerController as ctrl",
                    params: {
                        obj: null
                    }
                }
            }
        })
       .state('loyalCustomer',
        {
            url: '/ac/loyalCustomer',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: rootUrl + '/BTS.SP.MART/AC/CustomerCare/LoyalCustomer/index.html',
                    controller: "loyalCustomerController as ctrl",
                    params: {
                        obj: null
                    }
                }
            }
        })
        .state('loyalCustomerReport',
        {
            url: '/ac/loyalCustomerReport',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: rootUrl + '/BTS.SP.MART/AC/CustomerCare/LoyalCustomer/report.html',
                    controller: "reportLoyalCustomerController as ctrl",
                    params: {
                        obj: null
                    }
                }
            }
        })
        .state('acSinhNhatKh',
        {
            url: '/ac/acSinhNhatKh',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: rootUrl + '/BTS.SP.MART/AC/AcSinhNhatKh/index.html',
                    controller: "acSinhNhatKhController as ctrl",
                    params: {
                        obj: null
                    }
                }
            }
        })
        .state('acSinhNhatKhReport',
        {
            url: '/ac/acSinhNhatKhReport',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: rootUrl + '/BTS.SP.MART/AC/AcSinhNhatKh/report.html',
                    controller: "reportAcSinhNhatKhController as ctrl",
                    params: {
                        obj: null
                    }
                }
            }
        })
        .state('acDacBietKh',
        {
            url: '/ac/acDacBietKh',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: rootUrl + '/BTS.SP.MART/AC/AcDacBietKh/index.html',
                    controller: "acDacBietKhController as ctrl",
                    params: {
                        obj: null
                    }
                }
            }
        })
        .state('acDacBietKhReport',
        {
            url: '/ac/acDacBietKhReport',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: rootUrl + '/BTS.SP.MART/AC/AcDacBietKh/report.html',
                    controller: "reportAcDacBietKhController as ctrl",
                    params: {
                        obj: null
                    }
                }
            }
        })
        .state('acLanDauKh',
        {
            url: '/ac/acLanDauKh',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: rootUrl + '/BTS.SP.MART/AC/AcLanDauKh/index.html',
                    controller: "acLanDauKhController as ctrl",
                    params: {
                        obj: null
                    }
                }
            }
        })
        .state('acLanDauKhReport',
        {
            url: '/ac/acLanDauKhReport',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: rootUrl + '/BTS.SP.MART/AC/AcLanDauKh/report.html',
                    controller: "reportAcLanDauKhController as ctrl",
                    params: {
                        obj: null
                    }
                }
            }
        })
        .state('acDoanhSoSn',
        {
            url: '/ac/acDoanhSoSn',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: rootUrl + '/BTS.SP.MART/AC/AcDoanhSoSn/index.html',
                    controller: "acDoanhSoSnController as ctrl",
                    params: {
                        obj: null
                    }
                }
            }
        })
       .state('acDoanhSoSnReport',
        {
            url: '/ac/acDoanhSoSnReport',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: rootUrl + '/BTS.SP.MART/AC/AcDoanhSoSn/report.html',
                    controller: "reportAcDoanhSoSnController as ctrl",
                    params: {
                        obj: null
                    }
                }
            }
        })
        .state('acDoanhSoMoi',
        {
            url: '/ac/acDoanhSoMoi',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: rootUrl + '/BTS.SP.MART/AC/AcDoanhSoMoi/index.html',
                    controller: "acDoanhSoMoiController as ctrl",
                    params: {
                        obj: null
                    }
                }
            }
        })
        .state('acDoanhSoMoiReport',
        {
            url: '/ac/acDoanhSoMoiReport',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: rootUrl + '/BTS.SP.MART/AC/AcDoanhSoMoi/report.html',
                    controller: "reportAcDoanhSoMoiController as ctrl",
                    params: {
                        obj: null
                    }
                }
            }
        })
        .state('acDoanhSoMoiReportDetails',
        {
            url: '/ac/acDoanhSoMoiReportDetails',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: rootUrl + '/BTS.SP.MART/AC/AcDoanhSoMoi/report_details.html',
                    controller: "reportAcDoanhSoMoiDetailsController as ctrl",
                    params: {
                        obj: null
                    }
                }
            }
        })
        //End Region Customer Care
        ;
    }
]);

acModule.factory('acService', [
    '$resource', '$http', '$window', 'clientService', 'configService',
    function ($resource, $http, $window, clientService, configService) {
        var hostname = window.location.hostname;
        var port = window.location.port;
        var rootUrl = 'http://' + hostname + ':' + port;
        var result = {
            config: configService,
            client: clientService,
        };
        result.buildUrl = function (module, action) {
            return rootUrl + '_layouts/15/BTS.SP.MART/AC/' + module + '/' + action + '.html';
        };
        return result;
    }
]);
acModule.factory('serviceInventoryAndUnitUser', ['$resource', '$http', '$window', 'configService', 'clientService',
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
acModule.factory('serviceInventoryAndTax', ['$resource', '$http', '$window', 'configService', 'clientService',
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
acModule.factory('serviceInventoryAndWareHouse', ['$resource', '$http', '$window', 'configService', 'clientService',
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

acModule.factory('serviceInventoryAndMerchandiseType', ['$resource', '$http', '$window', 'configService', 'clientService',
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
acModule.factory('serviceInventoryAndMerchandise', ['$resource', '$http', '$window', 'configService', 'clientService',
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
acModule.factory('serviceInventoryAndMerchandiseGroup', ['$resource', '$http', '$window', 'configService', 'clientService',
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
acModule.factory('serviceInventoryAndNhaCungCap', ['$resource', '$http', '$window', 'configService', 'clientService',
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
acModule.factory('serviceInventoryAndSellingMachine', ['$resource', '$http', '$window', 'configService', 'clientService',
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
acModule.factory('serviceInventoryAndXuatXu', ['$resource', '$http', '$window', 'configService', 'clientService',
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
acModule.factory('serviceReportXuatBanAndUnitUser', ['$resource', '$http', '$window', 'configService', 'clientService',
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
acModule.factory('serviceReportXuatBanAndWareHouse', ['$resource', '$http', '$window', 'configService', 'clientService',
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
acModule.factory('serviceReportXuatBanAndMerchandiseType', ['$resource', '$http', '$window', 'configService', 'clientService',
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
acModule.factory('serviceReportXuatBanAndMerchandise', ['$resource', '$http', '$window', 'configService', 'clientService',
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
acModule.factory('serviceReportXuatBanAndMerchandiseGroup', ['$resource', '$http', '$window', 'configService', 'clientService',
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
acModule.factory('serviceReportXuatBanAndNhaCungCap', ['$resource', '$http', '$window', 'configService', 'clientService',
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
acModule.factory('serviceReportXuatBanAndCustomer', ['$resource', '$http', '$window', 'configService', 'clientService',
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
acModule.factory('serviceReportXuatBanAndTax', ['$resource', '$http', '$window', 'configService', 'clientService',
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
acModule.factory('serviceReportXuatBanAndXuatXu', ['$resource', '$http', '$window', 'configService', 'clientService',
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
acModule.factory('serviceReportKiemKeAndWareHouse', ['$resource', '$http', '$window', 'configService', 'clientService',
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
acModule.factory('serviceReportKiemKeAndMerchandiseType', ['$resource', '$http', '$window', 'configService', 'clientService',
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
acModule.factory('serviceReportKiemKeAndMerchandise', ['$resource', '$http', '$window', 'configService', 'clientService',
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
acModule.factory('serviceReportKiemKeAndMerchandiseGroup', ['$resource', '$http', '$window', 'configService', 'clientService',
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
acModule.factory('serviceReportKiemKeAndNhaCungCap', ['$resource', '$http', '$window', 'configService', 'clientService',
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
acModule.factory('serviceReportKiemKeAndKeHang', ['$resource', '$http', '$window', 'configService', 'clientService',
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
//DatHang
acModule.factory('serviceDatHangAndNhanVien', ['$resource', '$http', '$window', 'configService', 'clientService',
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