define([
], function () {
    var layoutUrl = "/BTS.SP.MART/views/auth/";
    var controlUrl = "/BTS.SP.MART/controllers/auth/";
    var states = [
        {
            name: 'auNguoiDung',
            url: '/auNguoiDung',
        	parent: 'layout',
        	abstract: false,
        	views: {
        		'viewMain@root': {
        		    templateUrl: layoutUrl + "/AuNguoiDung/index.html",
        		    controller: "AuNguoiDungViewCtrl as ctrl"
        		}
        	},
        	moduleUrl: controlUrl + "AuNguoiDung.js"
        },
        {
            name: 'auGroup',
            url: '/auGroup',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "/AuNhomQuyen/index.html",
                    controller: "AuNhomQuyenViewCtrl as ctrl"
                }
            },
            moduleUrl: controlUrl + "AuNhomQuyen.js"
        },
        {
            name: 'auThamSoHeThong',
            url: '/auThamSoHeThong',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "/AuThamSoHeThong/index.html",
                    controller: "AuThamSoHeThongViewCtrl as ctrl"
                }
            },
            moduleUrl: controlUrl + "AuThamSoHeThong.js"
        },
        {
            name: 'auMenu',
            url: '/auMenu',
            parent: 'layout',
            abstract: false,
            views: {
                'viewMain@root': {
                    templateUrl: layoutUrl + "/AuMenu/index.html",
                    controller: "AuMenu_ctrl as ctrl"
                }
            },
            moduleUrl: controlUrl + "AuMenu.js"
        },
         {
             name: 'auDonVi',
             url: '/auDonVi',
             parent: 'layout',
             abstract: false,
             views: {
                 'viewMain@root': {
                     templateUrl: layoutUrl + "/AuDonVi/index.html",
                     controller: "AuDonVi_ctrl as ctrl"
                 }
             },
             moduleUrl: controlUrl + "AuDonVi.js"
         }
    ];
    return states;
});