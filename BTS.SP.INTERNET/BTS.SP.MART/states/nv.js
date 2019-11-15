define([
], function () {
    var layoutUrl = "/BTS.SP.MART/views/nv/";
    var controlUrl = "/BTS.SP.MART/controllers/nv/";
    var states = [
		{
		    name: 'phieuNhapHangMua',
		    url: '/phieuNhapHangMua',
		    parent: 'layout',
		    abstract: false,
		    views: {
		        'viewMain@root': {
		            templateUrl: layoutUrl + "NvNhapHangMua/index.html?d=" + new Date(),
		            controller: "phieuNhapHangMuaController as ctrl"
		        }
		    },
		    moduleUrl: controlUrl + "phieuNhapHangMuaController.js"
		},
		{
		    name: 'reportPhieuNhapHangMua',
		    url: '/nv/nvNhapHangMua/reportNhapHangMua/:id',
		    parent: 'layout',
		    abstract: false,
		    views: {
		        'viewMain@root': {
		            templateUrl: layoutUrl + "NvNhapHangMua/report.html",
		            controller: "reportPhieuNhapHangMuaController as ctrl"
		        }
		    },
		    moduleUrl: controlUrl + "phieuNhapHangMuaController.js"
		},
        {
            name: 'reportPhieuNhapKhac',
            url: '/nv/nvNhapKhac/reportNhapKhac/:id',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "NvNhapKhac/report.html",
                    controller: "reportPhieuNhapKhacController as ctrl"
                }
            },
            moduleUrl: controlUrl + "phieuNhapKhacController.js"
        },
        {
            name: 'phieuXuatBan',
            url: '/phieuXuatBan',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "NvXuatBan/index.html?d=" + new Date(),
                    controller: "phieuXuatBanController as ctrl"
                }
            },
            moduleUrl: controlUrl + "phieuXuatBanController.js"
        },
        {
            name: 'reportPhieuXuatBan',
            url: '/nv/nvXuatBan/reportPhieuXuatBan/:id',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "NvXuatBan/report.html",
                    controller: "reportPhieuXuatBanController as ctrl"
                }
            },
            moduleUrl: controlUrl + "phieuXuatBanController.js"
        },
        {
            name: 'reportPhieuXuatBan_onlySL',
            url: '/nv/nvXuatBan/reportPhieuXuatBan/:id',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "NvXuatBan/report_onlySL.html",
                    controller: "reportPhieuXuatBanController_onlySL as ctrl"
                }
            },
            moduleUrl: controlUrl + "phieuXuatBanController.js"
        },
        {
            name: 'phieuXuatKhac',
            url: '/phieuXuatKhac',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "NvXuatKhac/index.html?d=" + new Date(),
                    controller: "phieuXuatKhacController as ctrl"
                }
            },
            moduleUrl: controlUrl + "phieuXuatKhacController.js"
        },
        {
            name: 'phieuNhapKhac',
            url: '/phieuNhapKhac',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "NvNhapKhac/index.html?d=" + new Date(),
                    controller: "phieuNhapKhacController as ctrl"
                }
            },
            moduleUrl: controlUrl + "phieuNhapKhacController.js"
        },
        {
            name: 'phieuTonDauKy',
            url: '/phieuTonDauKy',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "NvTonDauKy/index.html",
                    controller: "phieuTonDauKyController as ctrl"
                }
            },
            moduleUrl: controlUrl + "phieuTonDauKyController.js"
        },
        {
            name: 'reportPhieuTonDauKy',
            url: '/nv/nvTonDauKy/reportPhieuTonDauKy/:id',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "NvTonDauKy/report.html",
                    controller: "reportPhieuTonDauKyController as ctrl"
                }
            },
            moduleUrl: controlUrl + "phieuTonDauKyController.js"
        },
        {
            name: 'phieuNhapHangBanTraLai',
            url: '/phieuNhapHangBanTraLai',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "NvPhieuNhapHangBanTraLai/index.html",
                    controller: "phieuNhapHangBanTraLaiController as ctrl"
                }
            },
            moduleUrl: controlUrl + "phieuNhapHangBanTraLaiController.js"
        },
        {
            name: 'reportPhieuNhapHangBanTraLai',
            url: '/nv/nvNhapHangBanTraLai/reportPhieuNhapHangBanTraLai/:id',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "NvPhieuNhapHangBanTraLai/report.html",
                    controller: "reportPhieuNhapHangBanTraLaiController as ctrl"
                }
            },
            moduleUrl: controlUrl + "phieuNhapHangBanTraLaiController.js"
        },
        {
            name: 'phieuDatHang',
            url: '/phieuDatHang',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "NvDatHang/index.html",
                    controller: "phieuDatHangController as ctrl"
                }
            },
            moduleUrl: controlUrl + "phieuDatHangController.js"
        },
        {
            name: 'nvGiaoDichQuay',
            url: '/nvGiaoDichQuay',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "NvGiaoDichQuay/index.html",
                    controller: "giaoDichQuayController as ctrl"
                }
            },
            moduleUrl: controlUrl + "giaoDichQuayController.js"
        },
        {
            name: 'nvKiemKe',
            url: '/nvKiemKe',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "NvKiemKe/index.html?d="+ new Date(),
                    controller: "kiemKeController as ctrl"
                }
            },
            moduleUrl: controlUrl + "kiemKeController.js"
        },
        {
            name: 'reportPhieuKiemKe',
            url: '/nv/nvKiemKe/reportPhieuKiemKe/:id',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "NvKiemKe/report.html",
                    controller: "reportkiemKeController as ctrl"
                }
            },
            moduleUrl: controlUrl + "kiemKeController.js"
        },
        {
            name: 'phieuDieuChuyenNoiBo',
            url: '/phieuDieuChuyenNoiBo',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "NvPhieuDieuChuyenNoiBo/index.html",
                    controller: "phieuDieuChuyenNoiBoController as ctrl"
                }
            },
            moduleUrl: controlUrl + "phieuDieuChuyenNoiBoController.js"
        },
		{
		    name: 'reportPhieuDieuChuyenNoiBo',
		    url: '/nv/nvDieuChuyenNoiBo/reportDieuChuyenNoiBo/:id',
		    parent: 'layout',
		    abstract: false,
		    views: {
		        'viewMain@root': {
		            templateUrl: layoutUrl + "NvPhieuDieuChuyenNoiBo/report.html",
		            controller: "reportPhieuDieuChuyenNoiBoController as ctrl"
		        }
		    },
		    moduleUrl: controlUrl + "phieuDieuChuyenNoiBoController.js"
		},
        {
            name: 'reportChuyenKho',
            url: '/nv/nvDieuChuyenNoiBo/reportChuyenKho/:id',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "NvPhieuDieuChuyenNoiBo/reportChuyenKho.html",
                    controller: "reportPhieuDieuChuyenNoiBoController as ctrl"
                }
            },
            moduleUrl: controlUrl + "phieuDieuChuyenNoiBoController.js"
        },
		{
		    name: 'nvPhieuDieuChuyenNoiBoNhan',
		    url: '/nvPhieuDieuChuyenNoiBoNhan',
		    parent: 'layout',
		    abstract: false,
		    views: {
		        'viewMain@root': {
		            templateUrl: layoutUrl + "NvPhieuDieuChuyenNoiBoNhan/index.html",
		            controller: "phieuDieuChuyenNoiBoNhanController as ctrl"
		        }
		    },
		    moduleUrl: controlUrl + "phieuDieuChuyenNoiBoNhanController.js"
		},
		{
		    name: 'reportPhieuDieuChuyenNoiBoNhan',
		    url: '/nv/nvDieuChuyenNoiBoNhan/reportDieuChuyenNoiBoNhan/:id',
		    parent: 'layout',
		    abstract: false,
		    views: {
		        'viewMain@root': {
		            templateUrl: layoutUrl + "NvPhieuDieuChuyenNoiBoNhan/report-receive.html",
		            controller: "reportPhieuDieuChuyenNoiBoNhanController as ctrl"
		        }
		    },
		    moduleUrl: controlUrl + "phieuDieuChuyenNoiBoNhanController.js"
		},
        {
            name: 'nvDatHangNCC',
            url: '/nvDatHangNCC',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "NvDatHangNCC/index.html",
                    controller: "phieuDatHangNCCController as ctrl"
                }
            },
            moduleUrl: controlUrl + "phieuDatHangNCCController.js"
        },
        {
            name: 'printDonDatHangNCC',
            url: '/nv/NvDatHangNCC/printDonDatHangNCC/:id',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "NvDatHangNCC/donDatHang.html",
                    controller: "printDonDatHangNCCController as ctrl"
                }
            },
            moduleUrl: controlUrl + "phieuDatHangNCCController.js"
        },
        {
            name: 'printBienBanNCC',
            url: '/nv/NvDatHangNCC/printBienBanNCC/:id',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "NvDatHangNCC/bienBan.html",
                    controller: "printBienBanNCCController as ctrl"
                }
            },
            moduleUrl: controlUrl + "phieuDatHangNCCController.js"
        },
        {
            name: 'nvSummaryDatHangNCC',
            url: '/nvSummaryDatHangNCC',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "NvDatHangNCC/index-summary.html",
                    controller: "phieuDatHangNCCSummaryController as ctrl"
                }
            },
            moduleUrl: controlUrl + "phieuDatHangNCCController.js"
        },
		{
		    name: 'reportPhieuXuatKhac',
		    url: '/nv/nvXuatKhac/reportPhieuXuatKhac/:id',
		    parent: 'layout',
		    abstract: false,
		    views: {
		        'viewMain@root': {
		            templateUrl: layoutUrl + "NvXuatKhac/report.html",
		            controller: "reportPhieuXuatKhacController as ctrl"
		        }
		    },
		    moduleUrl: controlUrl + "phieuXuatKhacController.js"
		},
		{
		    name: 'nvKhuyenMai',
		    url: '/nvKhuyenMai',
		    parent: 'layout',
		    abstract: false,
		    views: {
		        'viewMain@root': {
		            templateUrl: layoutUrl + "NvKhuyenMai/index.html",
		            controller: "khuyenMaiController as ctrl"
		        }
		    },
		    moduleUrl: controlUrl + "khuyenMaiController.js"
		},
		{
		    name: 'nvKMTinhTien',
		    url: '/nvKMTinhTien',
		    parent: 'layout',
		    abstract: false,
		    views: {
		        'viewMain@root': {
		            templateUrl: layoutUrl + "NvKhuyenMai/TinhTien/index.html",
		            controller: "kmTinhTienController as ctrl"
		        }
		    },
		    moduleUrl: controlUrl + "kmTinhTienController.js"
		},
		{
		    name: 'nvKMDongGia',
		    url: '/nvKMDongGia',
		    parent: 'layout',
		    abstract: false,
		    views: {
		        'viewMain@root': {
		            templateUrl: layoutUrl + "NvKhuyenMai/DongGia/index.html",
		            controller: "kmDongGiaController as ctrl"
		        }
		    },
		    moduleUrl: controlUrl + "kmDongGiaController.js"
		},
		{
		    name: 'nvKMVoucher',
		    url: '/nvKMVoucher',
		    parent: 'layout',
		    abstract: false,
		    views: {
		        'viewMain@root': {
		            templateUrl: layoutUrl + "NvKhuyenMai/Voucher/index.html",
		            controller: "kmVoucherController as ctrl"
		        }
		    },
		    moduleUrl: controlUrl + "kmVoucherController.js"
		},
		{
		    name: 'nvKMBuy1Get1',
		    url: '/nvKMBuy1Get1',
		    parent: 'layout',
		    abstract: false,
		    views: {
		        'viewMain@root': {
		            templateUrl: layoutUrl + "NvKhuyenMai/Buy1Get1/index.html",
		            controller: "kmBuy1Get1Controller as ctrl"
		        }
		    },
		    moduleUrl: controlUrl + "kmBuy1Get1Controller.js"
		},
		{
		    name: 'nvKMCombo',
		    url: '/nvKMCombo',
		    parent: 'layout',
		    abstract: false,
		    views: {
		        'viewMain@root': {
		            templateUrl: layoutUrl + "NvKhuyenMai/Combo/index.html",
		            controller: "kmComboController as ctrl"
		        }
		    },
		    moduleUrl: controlUrl + "kmComboController.js"
		},
		{
		    name: 'nvPrintDetailKhuyenMai',
		    url: '/nvPrintDetailKhuyenMai',
		    parent: 'layout',
		    abstract: false,
		    views: {
		        'viewMain@root': {
		            templateUrl: layoutUrl + "NvKhuyenMai/printDetail.html",
		            controller: "printDetailKhuyenMaiController as ctrl"
		        }
		    },
		    moduleUrl: controlUrl + "khuyenMaiController.js"
		},
		{
		    name: 'nvKMTichDiem',
		    url: '/nvKMTichDiem',
		    parent: 'layout',
		    abstract: false,
		    views: {
		        'viewMain@root': {
		            templateUrl: layoutUrl + "NvKhuyenMai/TichDiem/index.html",
		            controller: "kmTichDiemController as ctrl"
		        }
		    },
		    moduleUrl: controlUrl + "kmTichDiemController.js"
		},
        {
            name: 'nvBanLe',
            url: '/banLe',
            abstract: false,
            views: {
                'viewRoot': {
                    templateUrl: layoutUrl + "BanLe/index.html",
                    controller: "banLeController as ctrl"
                }
            },
            moduleUrl: controlUrl + "banLeController.js"
        },
        {
            name: 'nvCongNoKhachHang',
            url: '/nvCongNoKhachHang',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "NvCongNoKhachHang/index.html",
                    controller: "nvCongNoKhachHangController as ctrl"
                }
            },
            moduleUrl: controlUrl + "nvCongNoKhachHangController.js"
        },
        {
            name: 'nvCongNoNCC',
            url: '/nvCongNoNCC',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "NvCongNoNCC/index.html",
                    controller: "nvCongNoNCCController as ctrl"
                }
            },
            moduleUrl: controlUrl + "nvCongNoNCCController.js"
        },
        {
            name: 'reportGiaoDichQuay',
            url: '/nv/nvGiaDichQuay/reportGiaoDichQuay/:id',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "NvGiaoDichQuay/report.html",
                    controller: "reportGiaoDichQuayController as ctrl"
                }
            },
            moduleUrl: controlUrl + "giaoDichQuayController.js"
        },
        {
            name: 'phieuNgayHetHanHangHoa',
            url: '/phieuNgayHetHanHangHoa',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "NvNgayHetHanHangHoa/index.html",
                    controller: "phieuNgayHetHanHangHoaController as ctrl"
                }
            },
            moduleUrl: controlUrl + "phieuNgayHetHanHangHoaController.js"
        },
        {
            name: 'reportPhieuNgayHetHanHangHoa',
            url: '/nv/nvNgayHetHanHangHoa/reportPhieuNgayHetHanHangHoa/:id',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "NvNgayHetHanHangHoa/report.html",
                    controller: "reportPhieuNgayHetHanHangHoaController as ctrl"
                }
            },
            moduleUrl: controlUrl + "phieuNgayHetHanHangHoaController.js"
        },
    ];
    return states;
});