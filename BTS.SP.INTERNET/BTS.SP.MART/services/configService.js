define(['angular'], function () {
    var app = angular.module('configModule', []);
    app.factory('configService', function () {
        var hostname = window.location.hostname;
        var port = window.location.port;
        var rootUrl = 'http://' + hostname + ':' + port;
        var rootUrlApi = 'http://localhost:44915';
        
        if (!port) {
            rootUrl = 'http://' + hostname;
        }
        var result = {
            rootUrlWeb: rootUrl,
            rootUrlWebApi: rootUrlApi + '/api',
            apiServiceBaseUri: rootUrlApi,
            dateFormat: 'dd/MM/yyyy',
            delegateEvent: function ($event) {
                $event.preventDefault();
                $event.stopPropagation();
            }
        };
        result.buildUrl = function (folder, file) {
            return this.rootUrlWeb + "/BTS.SP.MART/views/" + folder + "/" + file + ".html";
        };
        result.pageDefault = {
            totalItems: 0,
            itemsPerPage: 10,
            currentPage: 1,
            pageSize: 5,
            totalPage: 5,
            maxSize: 5
        };
        result.filterDefault = {
            summary: '',
            isAdvance: false,
            advanceData: {},
            orderBy: '',
            orderType: 'ASC',
        };
        result.saveExcel = function (data, fileName) {
            var fileName = fileName + ".xls"
            var filetype = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=utf-8";
            var ieEDGE = navigator.userAgent.match(/Edge/g);
            var ie = navigator.userAgent.match(/.NET/g); // IE 11+
            var oldIE = navigator.userAgent.match(/MSIE/g);
            if (ie || oldIE || ieEDGE) {
                var blob = new window.Blob(data, { type: filetype });
                window.navigator.msSaveBlob(blob, fileName);
            }
            else {
                var a = $("<a style='display: none;'/>");
                var url = window.url.createObjectURL(new Blob(data, { type: filetype }));
                a.attr("href", url);
                a.attr("download", fileName);
                $("body").append(a);
                a[0].click();
                window.url.revokeObjectURL(url);
                a.remove();
            }
        }
        var label = {
            lblMessage: 'Thông báo',
            lblNotifications: 'Thông báo',
            lblindex: '',
            lblDetails: 'Thông tin',
            lblEdit: 'Cập nhập',
            lblCreate: 'Thêm',
            lbl: '',
            btnCreate: 'Thêm mới',
            btnImport: 'Cập nhật từ tệp excel',
            btnEdit: 'Sửa',
            btnDelete: 'Xóa',
            btnRemove: 'Xóa',
            btnActive: 'Active',
            btnInactive: 'Inactive',
            btnToggle: 'Toggle',
            btnSaveAndKeep: 'Lưu và giữ lại',
            btnSaveAndPrint: 'Lưu và in phiếu',

            btnSearch: 'Tìm kiếm',
            btnRefresh: 'Làm mới',
            btnBack: 'Quay lại',
            btnClear: 'Xóa tất cả',
            btnCancel: 'Hủy',

            btnSave: 'Lưu lại',
            btnSubmit: 'Lưu',

            btnLogOn: 'Đăng nhập',
            btnLogOff: 'Đăng xuất',
            btnChangePassword: 'Đổi mật khẩu',

            btnSendMessage: 'Gửi tin nhắn',
            btnSendNotification: 'Gửi thông báo',
            btnNotifications: 'Thông báo',

            btnDisconnect: 'Hủy kết nối',
            btnDisconnectSession: 'Hủy kết nối',
            btnDisconnectAccount: 'Hủy mọi kết nối',

            btnUpload: 'Upload',
            btnUploadAll: 'Upload tất cả',
            btnFileCancel: 'Hủy',
            btnFileCancelAll: 'Hủy tất cả',
            btnFileRemove: 'Xóa',
            btnFileRemoveAll: 'Xóa tất cả',
            btn: '',

            btnImportExcel: 'Import từ file excel',
            btnExportExcel: 'Xuất ra file excel',

            btnCall: 'Call',
            btnChart: 'Biểu đồ',
            btnData: 'Số liệu',
            btnPrint: 'In phiếu',
            btnExit: 'Thoát',
            btnExportPDF: 'Kết xuất file PDF',
            btnExport: 'Kết xuất',

            btnPrintList: 'In DS',
            btnPrintDetailList: 'In DS chi tiết',
            btnSend: 'DS duyệt',
            btnApproval: 'Duyệt',
            btnComplete: 'Hoàn thành',
            btnAddInfo: 'Bổ sung'
        };
        result.label = label;
        return result;
    }
    );
    return app;
});
