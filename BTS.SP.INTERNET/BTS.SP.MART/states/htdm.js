define([
], function () {
    var layoutUrl = "/BTS.SP.MART/views/htdm/";
    var controlUrl = "/BTS.SP.MART/controllers/htdm/";
    var states = [
        // Danh mục bó hàng
        {
            name: 'bohang',
            url: '/bohang',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "BoHang/index.html",
                    controller: "boHangController as ctrl"
                }
            },
            moduleUrl: controlUrl + "boHangController.js"
        },
        // Danh mục chiết khấu khách hàng
        {
            name: 'chietKhauKh',
            url: '/chietKhauKh',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "ChietKhauKhacHang/index.html",
                    controller: "chietKhauKhController as ctrl"
                }
            },
            moduleUrl: controlUrl + "chietKhauKhController.js"
        },
        // Danh mục màu sắc
        {
            name: 'color',
            url: '/color',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "Color/index.html",
                    controller: "colorController as ctrl"
                }
            },
            moduleUrl: controlUrl + "colorController.js"
        },
        // Danh mục mặt hàng
        {
            name: 'merchandise',
            url: '/merchandise',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "Merchandise/index.html",
                    controller: "merchandiseController as ctrl"
                }
            },
            moduleUrl: controlUrl + "merchandiseController.js"
        },
        // Danh mục loại hàng hóa
        {
            name: 'merchandiseType',
            url: '/merchandiseType',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "MerchandiseType/index.html",
                    controller: "merchandiseTypeController as ctrl"
                }
            },
            moduleUrl: controlUrl + "merchandiseTypeController.js"
        },
        // Danh mục nhóm hàng hóa
        {
            name: 'nhomVatTu',
            url: '/nhomVatTu',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "NhomVatTu/index.html",
                    controller: "nhomVatTuController as ctrl"
                }
            },
            moduleUrl: controlUrl + "nhomVatTuController.js"
        },
        // Danh mục hợp đồng
        {
            name: 'contract',
            url: '/contract',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "Contract/index.html",
                    controller: "contractController as ctrl"
                }
            },
            moduleUrl: controlUrl + "contractController.js"
        },
        // Danh mục tiền tệ
        {
            name: 'currency',
            url: '/currency',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "Currency/index.html",
                    controller: "currencyController as ctrl"
                }
            },
            moduleUrl: controlUrl + "currencyController.js"
        },
        // Danh mục khách hàng
        {
            name: 'customer',
            url: '/customer',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "Customer/index.html",
                    controller: "customerController as ctrl"
                }
            },
            moduleUrl: controlUrl + "customerController.js"
        },
        // Danh mục phòng ban
        {
            name: 'department',
            url: '/department',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "Department/index.html",
                    controller: "departmentController as ctrl"
                }
            },
            moduleUrl: controlUrl + "departmentController.js"
        },
        // Danh mục đơn vị tính
        {
            name: 'donViTinh',
            url: '/donViTinh',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "DonViTinh/index.html",
                    controller: "donViTinhController as ctrl"
                }
            },
            moduleUrl: controlUrl + "donViTinhController.js"
        },
        // Danh mục hạng khách hàng
        {
            name: 'hangKh',
            url: '/hangKh',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "HangKhachHang/index.html",
                    controller: "hangKhController as ctrl"
                }
            },
            moduleUrl: controlUrl + "hangKhController.js"
        },
        // Danh mục bao bì
        {
            name: 'packaging',
            url: '/packaging',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "Packaging/index.html",
                    controller: "packagingController as ctrl"
                }
            },
            moduleUrl: controlUrl + "packagingController.js"
        },
        // Danh mục kỳ kế toán
        {
            name: 'period',
            url: '/period',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "Period/index.html",
                    controller: "periodController as ctrl"
                }
            },
            moduleUrl: controlUrl + "periodController.js"
        },
        // Danh mục máy bán hàng
        {
            name: 'sellingMachine',
            url: '/sellingMachine',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "SellingMachine/index.html",
                    controller: "sellingMachineController as ctrl"
                }
            },
            moduleUrl: controlUrl + "sellingMachineController.js"
        },
        // Danh mục kệ hàng
        {
            name: 'shelves',
            url: '/shelves',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "Shelves/index.html",
                    controller: "shelvesController as ctrl"
                }
            },
            moduleUrl: controlUrl + "shelvesController.js"
        },
        // Danh mục size
        {
            name: 'size',
            url: '/size',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "Size/index.html",
                    controller: "sizeController as ctrl"
                }
            },
            moduleUrl: controlUrl + "sizeController.js"
        },
        // Danh mục nhà cung cấp
        {
            name: 'supplier',
            url: '/supplier',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "Supplier/index.html",
                    controller: "supplierController as ctrl"
                }
            },
            moduleUrl: controlUrl + "supplierController.js"
        },
        // Danh mục thuế
        {
            name: 'tax',
            url: '/tax',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "Tax/index.html",
                    controller: "taxController as ctrl"
                }
            },
            moduleUrl: controlUrl + "taxController.js"
        },
        // Danh mục loại lý do
        {
            name: 'typeReason',
            url: '/typeReason',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "TypeReason/index.html",
                    controller: "typeReasonController as ctrl"
                }
            },
            moduleUrl: controlUrl + "typeReasonController.js"
        },
        // Danh mục kho hàng
        {
            name: 'wareHouse',
            url: '/wareHouse',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "WareHouse/index.html",
                    controller: "wareHouseController as ctrl"
                }
            },
            moduleUrl: controlUrl + "wareHouseController.js"
        },
        // Danh mục xuất xứ
        {
            name: 'xuatXu',
            url: '/xuatXu',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "XuatXu/index.html",
                    controller: "xuatXuController as ctrl"
                }
            },
            moduleUrl: controlUrl + "xuatXuController.js"
        }
    ];
    return states;
});