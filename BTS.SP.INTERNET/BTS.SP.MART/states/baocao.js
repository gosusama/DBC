define([
], function () {
    var layoutUrl = "/BTS.SP.MART/views/baocao/";
    var controlUrl = "/BTS.SP.MART/controllers/baocao/";
    var controlUrlCustomerCare = "/BTS.SP.MART/controllers/baocao/CustomerCare/";
    var states = [
        {
            name: 'inventory',
            url: '/inventory',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "/Inventory/index.html",
                    controller: "inventoryController as ctrl"
                }
            },
            moduleUrl: controlUrl + "inventoryController.js"
        },
        {
            name: 'ngayHetHanHangHoa',
            url: '/ngayHetHanHangHoa',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "/AcNgayHetHanHangHoa/index.html",
                    controller: "acNgayHetHanHangHoaController as ctrl"
                }
            },
            moduleUrl: controlUrl + "acNgayHetHanHangHoa.js"
        },
        {
            name: 'baoCaoCongNo',
            url: '/baoCaoCongNo',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "/AcCongNo/index.html",
                    controller: "acCongNoController as ctrl"
                }
            },
            moduleUrl: controlUrl + "acCongNoController.js"
        },
		{
		    name: 'baoCaoXuatBan',
		    url: '/baoCaoXuatBan',
		    parent: 'layout',
		    abstract: false,
		    views: {
		        'viewMain@root': {
		            templateUrl: layoutUrl + "/AcXuatBan/index.html",
		            controller: "acXuatBanController as ctrl"
		        }
		    },
		    moduleUrl: controlUrl + "acXuatBanController.js"
		},
		{
		    name: 'baoCaoNhapMua',
		    url: '/baoCaoNhapMua',
		    parent: 'layout',
		    abstract: false,
		    views: {
		        'viewMain@root': {
		            templateUrl: layoutUrl + "/AcNhapMua/index.html",
		            controller: "acNhapMuaController as ctrl"
		        }
		    },
		    moduleUrl: controlUrl + "acNhapMuaController.js"
		},
		{
		    name: 'baoCaoKiemKe',
		    url: '/baoCaoKiemKe',
		    parent: 'layout',
		    abstract: false,
		    views: {
		        'viewMain@root': {
		            templateUrl: layoutUrl + "/AcKiemKe/index.html",
		            controller: "acKiemKeController as ctrl"
		        }
		    },
		    moduleUrl: controlUrl + "acKiemKeController.js"
		},
        // chăm sóc khách hàng
        {
            name: 'beChangedCardCustomer',
            url: '/khachHangDoiThe',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "/CustomerCare/BeChangedCardCustomer/index.html",
                    controller: "beChangedCardCustomerController as ctrl"
                }
            },
            moduleUrl: controlUrlCustomerCare + "beChangedCardCustomerController.js"
        },
        {
            name: 'beChangedCardCustomerReport',
            url: '/khachHangDoiTheReport',
            parent: 'layout',
            abstract: false,
            params: {
                obj: null
            },
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "/CustomerCare/BeChangedCardCustomer/report.html",
                    controller: "reportBeChangedCardCustomerController as ctrl"
                }
            },
            moduleUrl: controlUrlCustomerCare + "beChangedCardCustomerController.js"
        },
        {
            name: 'customerLevelUp',
            url: '/khachHangLenHang',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "/CustomerCare/CustomerLevelUp/index.html",
                    controller: "customerLevelUpController as ctrl"
                }
            },
            moduleUrl: controlUrlCustomerCare + "customerLevelUpController.js"
        },
        {
            name: 'customerLevelUpReport',
            url: '/khachHangDoiTheReport',
            parent: 'layout',
            abstract: false,
            params: {
                obj: null
            },
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "/CustomerCare/CustomerLevelUp/report.html",
                    controller: "reportCustomerLevelUpController as ctrl"
                }
            },
            moduleUrl: controlUrlCustomerCare + "customerLevelUpController.js"
        },
        {
            name: 'acDoanhSoMoi',
            url: '/doanhSoKhachMoi',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "/AcDoanhSoMoi/index.html",
                    controller: "doanhSoKhachHangMoiController as ctrl"
                }
            },
            moduleUrl: controlUrlCustomerCare + "doanhSoKhachHangMoiController.js"
        },
        {
            name: 'doanhSoKhachHangMoiReport',
            url: '/doanhSoKhachMoiReport',
            parent: 'layout',
            abstract: false,
            params: {
                obj: null
            },
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "/AcDoanhSoMoi/report.html",
                    controller: "reportDoanhSoKhachHangMoiController as ctrl"
                }
            },
            moduleUrl: controlUrlCustomerCare + "doanhSoKhachHangMoiController.js"
        },
        {
            name: 'acDoanhSoSn',
            url: '/doanhSoSinhNhat',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "/AcDoanhSoSn/index.html",
                    controller: "doanhSoSinhNhatController as ctrl"
                }
            },
            moduleUrl: controlUrlCustomerCare + "doanhSoSinhNhatController.js"
        },
        {
            name: 'doanhSoSinhNhatReport',
            url: '/doanhSoSinhNhatReport',
            parent: 'layout',
            abstract: false,
            params: {
                obj: null
            },
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "/AcDoanhSoSn/report.html",
                    controller: "reportDoanhSoSinhNhatController as ctrl"
                }
            },
            moduleUrl: controlUrlCustomerCare + "doanhSoSinhNhatController.js"
        },
        {
            name: 'forgetCardCustomer',
            url: '/khachQuenThe',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "/CustomerCare/ForgetCardCustomer/index.html",
                    controller: "forgetCardCustomerController as ctrl"
                }
            },
            moduleUrl: controlUrlCustomerCare + "forgetCardCustomerController.js"
        },
        {
            name: 'forgetCardCustomerReport',
            url: '/khachQuenTheReport',
            parent: 'layout',
            abstract: false,
            params: {
                obj: null
            },
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "/CustomerCare/ForgetCardCustomer/report.html",
                    controller: "reportForgetCardCustomerController as ctrl"
                }
            },
            moduleUrl: controlUrlCustomerCare + "forgetCardCustomerController.js"
        },
        {
            name: 'historyGiveCardCustomer',
            url: '/lichSuDoiThe',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "/CustomerCare/HistoryGiveCardCustomer/index.html",
                    controller: "historyGiveCardCustomerController as ctrl"
                }
            },
            moduleUrl: controlUrlCustomerCare + "historyGiveCardCustomerController.js"
        },
        {
            name: 'historyGiveCardCustomerReport',
            url: '/lichSuDoiTheReport',
            parent: 'layout',
            abstract: false,
            params: {
                obj: null
            },
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "/CustomerCare/HistoryGiveCardCustomer/report.html",
                    controller: "reportHistoryGiveCardCustomerController as ctrl"
                }
            },
            moduleUrl: controlUrlCustomerCare + "historyGiveCardCustomerController.js"
        },
        {
            name: 'acDacBietKh',
            url: '/khachHangDacBiet',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "/AcDacBietKh/index.html",
                    controller: "khachHangDacBietController as ctrl"
                }
            },
            moduleUrl: controlUrlCustomerCare + "khachHangDacBietController.js"
        },
        {
            name: 'khachHangDacBietReport',
            url: '/khachHangDacBietReport',
            parent: 'layout',
            abstract: false,
            params: {
                obj: null
            },
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "/AcDacBietKh/report.html",
                    controller: "reportKhachHangDacBietController as ctrl"
                }
            },
            moduleUrl: controlUrlCustomerCare + "khachHangDacBietController.js"
        },
        {
            name: 'acLanDauKh',
            url: '/khachHangLanDauMua',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "/AcLanDauKh/index.html",
                    controller: "khachHangLanDauMuaController as ctrl"
                }
            },
            moduleUrl: controlUrlCustomerCare + "khachHangLanDauMuaController.js"
        },
        {
            name: 'khachHangLanDauMuaReport',
            url: '/khachHangLanDauMuaReport',
            parent: 'layout',
            abstract: false,
            params: {
                obj: null
            },
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "/AcLanDauKh/report.html",
                    controller: "reportKhachHangLanDauMuaController as ctrl"
                }
            },
            moduleUrl: controlUrlCustomerCare + "khachHangLanDauMuaController.js"
        },
        {
            name: 'loyalCustomer',
            url: '/loyalCustomer',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "/CustomerCare/LoyalCustomer/index.html",
                    controller: "loyalCustomerController as ctrl"
                }
            },
            moduleUrl: controlUrlCustomerCare + "loyalCustomerController.js"
        },
        {
            name: 'loyalCustomerReport',
            url: '/loyalCustomerReport',
            parent: 'layout',
            abstract: false,
            params: {
                obj: null
            },
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "/CustomerCare/LoyalCustomer/report.html",
                    controller: "reportLoyalCustomerController as ctrl"
                }
            },
            moduleUrl: controlUrlCustomerCare + "loyalCustomerController.js"
        },
        {
            name: 'notBuyCustomer',
            url: '/lauKhongMua',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "/CustomerCare/NotBuyCustomer/index.html",
                    controller: "notBuyCustomerController as ctrl"
                }
            },
            moduleUrl: controlUrlCustomerCare + "notBuyCustomerController.js"
        },
        {
            name: 'notBuyCustomerReport',
            url: '/lauKhongMuaReport',
            parent: 'layout',
            abstract: false,
            params: {
                obj: null
            },
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "/CustomerCare/NotBuyCustomer/report.html",
                    controller: "reportNotBuyCustomerController as ctrl"
                }
            },
            moduleUrl: controlUrlCustomerCare + "notBuyCustomerController.js"
        },
        {
            name: 'acSinhNhatKh',
            url: '/sinhNhatKhachHang',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "/AcSinhNhatKh/index.html",
                    controller: "sinhNhatKhachHangController as ctrl"
                }
            },
            moduleUrl: controlUrlCustomerCare + "sinhNhatKhachHangController.js"
        },
        {
            name: 'sinhNhatKhachHangReport',
            url: '/sinhNhatKhachHangReport',
            parent: 'layout',
            abstract: false,
            params: {
                obj: null
            },
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "/AcSinhNhatKh/report.html",
                    controller: "reportSinhNhatKhachHangController as ctrl"
                }
            },
            moduleUrl: controlUrlCustomerCare + "sinhNhatKhachHangController.js"
        },
        {
            name: 'baoCaoTanSuatMuaHang',
            url: '/baoCaoTanSuatMuaHang',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "/TanSuatMuaHang/index.html",
                    controller: "tanSuatMuaHangController as ctrl"
                }
            },
            moduleUrl: controlUrlCustomerCare + "tanSuatMuaHangController.js"
        },
        {
            name: 'tanSuatMuaHangReport',
            url: '/tanSuatMuaHangReport',
            parent: 'layout',
            abstract: false,
            params: {
                obj: null
            },
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "/TanSuatMuaHang/report.html",
                    controller: "reportTanSuatMuaHangController as ctrl"
                }
            },
            moduleUrl: controlUrlCustomerCare + "tanSuatMuaHangController.js"
        },
        {
            name: 'cashier',
            url: '/cashier',
            parent: 'layout',
            abstract: false,
            params: {
                obj: null
            },
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "/Cashier/index.html",
                    controller: "cashierController as ctrl"
                }
            },
            moduleUrl: controlUrl + "cashierController.js"
        },
        {
            name: 'tinNhanKhachHang',
            url: '/tinNhan',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "CustomerCare/TinNhanKhachHang/index.html",
                    controller: "tinNhanKhachHangController as ctrl"
                }
            },
            moduleUrl: controlUrlCustomerCare + "tinNhanKhachHangController.js"
        }
    ];
    return states;
}); define([
], function () {
    var layoutUrl = "/BTS.SP.MART/views/baocao/";
    var controlUrl = "/BTS.SP.MART/controllers/baocao/";
    var controlUrlCustomerCare = "/BTS.SP.MART/controllers/baocao/CustomerCare/";
    var states = [
        {
            name: 'inventory',
            url: '/inventory',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "/Inventory/index.html",
                    controller: "inventoryController as ctrl"
                }
            },
            moduleUrl: controlUrl + "inventoryController.js"
        },
		{
		    name: 'baoCaoXuatBan',
		    url: '/baoCaoXuatBan',
		    parent: 'layout',
		    abstract: false,
		    views: {
		        'viewMain@root': {
		            templateUrl: layoutUrl + "/AcXuatBan/index.html",
		            controller: "acXuatBanController as ctrl"
		        }
		    },
		    moduleUrl: controlUrl + "acXuatBanController.js"
		},
		{
		    name: 'baoCaoNhapMua',
		    url: '/baoCaoNhapMua',
		    parent: 'layout',
		    abstract: false,
		    views: {
		        'viewMain@root': {
		            templateUrl: layoutUrl + "/AcNhapMua/index.html",
		            controller: "acNhapMuaController as ctrl"
		        }
		    },
		    moduleUrl: controlUrl + "acNhapMuaController.js"
		},
        // chăm sóc khách hàng
        {
            name: 'beChangedCardCustomer',
            url: '/khachHangDoiThe',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "/CustomerCare/BeChangedCardCustomer/index.html",
                    controller: "beChangedCardCustomerController as ctrl"
                }
            },
            moduleUrl: controlUrlCustomerCare + "beChangedCardCustomerController.js"
        },
        {
            name: 'beChangedCardCustomerReport',
            url: '/khachHangDoiTheReport',
            parent: 'layout',
            abstract: false,
            params: {
                obj: null
            },
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "/CustomerCare/BeChangedCardCustomer/report.html",
                    controller: "reportBeChangedCardCustomerController as ctrl"
                }
            },
            moduleUrl: controlUrlCustomerCare + "beChangedCardCustomerController.js"
        },
        {
            name: 'customerLevelUp',
            url: '/khachHangLenHang',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "/CustomerCare/CustomerLevelUp/index.html",
                    controller: "customerLevelUpController as ctrl"
                }
            },
            moduleUrl: controlUrlCustomerCare + "customerLevelUpController.js"
        },
        {
            name: 'customerLevelUpReport',
            url: '/khachHangDoiTheReport',
            parent: 'layout',
            abstract: false,
            params: {
                obj: null
            },
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "/CustomerCare/CustomerLevelUp/report.html",
                    controller: "reportCustomerLevelUpController as ctrl"
                }
            },
            moduleUrl: controlUrlCustomerCare + "customerLevelUpController.js"
        },
        {
            name: 'acDoanhSoMoi',
            url: '/doanhSoKhachMoi',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "/AcDoanhSoMoi/index.html",
                    controller: "doanhSoKhachHangMoiController as ctrl"
                }
            },
            moduleUrl: controlUrlCustomerCare + "doanhSoKhachHangMoiController.js"
        },
        {
            name: 'doanhSoKhachHangMoiReport',
            url: '/doanhSoKhachMoiReport',
            parent: 'layout',
            abstract: false,
            params: {
                obj: null
            },
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "/AcDoanhSoMoi/report.html",
                    controller: "reportDoanhSoKhachHangMoiController as ctrl"
                }
            },
            moduleUrl: controlUrlCustomerCare + "doanhSoKhachHangMoiController.js"
        },
        {
            name: 'acDoanhSoSn',
            url: '/doanhSoSinhNhat',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "/AcDoanhSoSn/index.html",
                    controller: "doanhSoSinhNhatController as ctrl"
                }
            },
            moduleUrl: controlUrlCustomerCare + "doanhSoSinhNhatController.js"
        },
        {
            name: 'doanhSoSinhNhatReport',
            url: '/doanhSoSinhNhatReport',
            parent: 'layout',
            abstract: false,
            params: {
                obj: null
            },
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "/AcDoanhSoSn/report.html",
                    controller: "reportDoanhSoSinhNhatController as ctrl"
                }
            },
            moduleUrl: controlUrlCustomerCare + "doanhSoSinhNhatController.js"
        },
        {
            name: 'forgetCardCustomer',
            url: '/khachQuenThe',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "/CustomerCare/ForgetCardCustomer/index.html",
                    controller: "forgetCardCustomerController as ctrl"
                }
            },
            moduleUrl: controlUrlCustomerCare + "forgetCardCustomerController.js"
        },
        {
            name: 'forgetCardCustomerReport',
            url: '/khachQuenTheReport',
            parent: 'layout',
            abstract: false,
            params: {
                obj: null
            },
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "/CustomerCare/ForgetCardCustomer/report.html",
                    controller: "reportForgetCardCustomerController as ctrl"
                }
            },
            moduleUrl: controlUrlCustomerCare + "forgetCardCustomerController.js"
        },
        {
            name: 'historyGiveCardCustomer',
            url: '/lichSuDoiThe',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "/CustomerCare/HistoryGiveCardCustomer/index.html",
                    controller: "historyGiveCardCustomerController as ctrl"
                }
            },
            moduleUrl: controlUrlCustomerCare + "historyGiveCardCustomerController.js"
        },
        {
            name: 'historyGiveCardCustomerReport',
            url: '/lichSuDoiTheReport',
            parent: 'layout',
            abstract: false,
            params: {
                obj: null
            },
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "/CustomerCare/HistoryGiveCardCustomer/report.html",
                    controller: "reportHistoryGiveCardCustomerController as ctrl"
                }
            },
            moduleUrl: controlUrlCustomerCare + "historyGiveCardCustomerController.js"
        },
        {
            name: 'acDacBietKh',
            url: '/khachHangDacBiet',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "/AcDacBietKh/index.html",
                    controller: "khachHangDacBietController as ctrl"
                }
            },
            moduleUrl: controlUrlCustomerCare + "khachHangDacBietController.js"
        },
        {
            name: 'khachHangDacBietReport',
            url: '/khachHangDacBietReport',
            parent: 'layout',
            abstract: false,
            params: {
                obj: null
            },
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "/AcDacBietKh/report.html",
                    controller: "reportKhachHangDacBietController as ctrl"
                }
            },
            moduleUrl: controlUrlCustomerCare + "khachHangDacBietController.js"
        },
        {
            name: 'acLanDauKh',
            url: '/khachHangLanDauMua',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "/AcLanDauKh/index.html",
                    controller: "khachHangLanDauMuaController as ctrl"
                }
            },
            moduleUrl: controlUrlCustomerCare + "khachHangLanDauMuaController.js"
        },
        {
            name: 'khachHangLanDauMuaReport',
            url: '/khachHangLanDauMuaReport',
            parent: 'layout',
            abstract: false,
            params: {
                obj: null
            },
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "/AcLanDauKh/report.html",
                    controller: "reportKhachHangLanDauMuaController as ctrl"
                }
            },
            moduleUrl: controlUrlCustomerCare + "khachHangLanDauMuaController.js"
        },
        {
            name: 'loyalCustomer',
            url: '/loyalCustomer',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "/CustomerCare/LoyalCustomer/index.html",
                    controller: "loyalCustomerController as ctrl"
                }
            },
            moduleUrl: controlUrlCustomerCare + "loyalCustomerController.js"
        },
        {
            name: 'loyalCustomerReport',
            url: '/loyalCustomerReport',
            parent: 'layout',
            abstract: false,
            params: {
                obj: null
            },
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "/CustomerCare/LoyalCustomer/report.html",
                    controller: "reportLoyalCustomerController as ctrl"
                }
            },
            moduleUrl: controlUrlCustomerCare + "loyalCustomerController.js"
        },
        {
            name: 'notBuyCustomer',
            url: '/lauKhongMua',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "/CustomerCare/NotBuyCustomer/index.html",
                    controller: "notBuyCustomerController as ctrl"
                }
            },
            moduleUrl: controlUrlCustomerCare + "notBuyCustomerController.js"
        },
        {
            name: 'notBuyCustomerReport',
            url: '/lauKhongMuaReport',
            parent: 'layout',
            abstract: false,
            params: {
                obj: null
            },
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "/CustomerCare/NotBuyCustomer/report.html",
                    controller: "reportNotBuyCustomerController as ctrl"
                }
            },
            moduleUrl: controlUrlCustomerCare + "notBuyCustomerController.js"
        },
        {
            name: 'acSinhNhatKh',
            url: '/sinhNhatKhachHang',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "/AcSinhNhatKh/index.html",
                    controller: "sinhNhatKhachHangController as ctrl"
                }
            },
            moduleUrl: controlUrlCustomerCare + "sinhNhatKhachHangController.js"
        },
        {
            name: 'sinhNhatKhachHangReport',
            url: '/sinhNhatKhachHangReport',
            parent: 'layout',
            abstract: false,
            params: {
                obj: null
            },
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "/AcSinhNhatKh/report.html",
                    controller: "reportSinhNhatKhachHangController as ctrl"
                }
            },
            moduleUrl: controlUrlCustomerCare + "sinhNhatKhachHangController.js"
        },
        {
            name: 'baoCaoTanSuatMuaHang',
            url: '/baoCaoTanSuatMuaHang',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "/TanSuatMuaHang/index.html",
                    controller: "tanSuatMuaHangController as ctrl"
                }
            },
            moduleUrl: controlUrlCustomerCare + "tanSuatMuaHangController.js"
        },
        {
            name: 'tanSuatMuaHangReport',
            url: '/tanSuatMuaHangReport',
            parent: 'layout',
            abstract: false,
            params: {
                obj: null
            },
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "/TanSuatMuaHang/report.html",
                    controller: "reportTanSuatMuaHangController as ctrl"
                }
            },
            moduleUrl: controlUrlCustomerCare + "tanSuatMuaHangController.js"
        },
        {
            name: 'cashier',
            url: '/cashier',
            parent: 'layout',
            abstract: false,
            params: {
                obj: null
            },
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "/Cashier/index.html",
                    controller: "cashierController as ctrl"
                }
            },
            moduleUrl: controlUrl + "cashierController.js"
        }
    ];
    return states;
});