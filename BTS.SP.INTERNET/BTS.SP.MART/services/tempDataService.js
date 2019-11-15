define(['angular'], function () {
    var app = angular.module('tempDataModule', []);
    app.factory('tempDataService', ['CacheFactory', function (CacheFactory) {
        var profileCache;
        if (!CacheFactory.get('profileCache')) {
            profileCache = CacheFactory('profileCache');
        }
        profileCache.put('rootUnitCode', [
           {
               text: 'Siêu thị Dabaco Lý Thái Tổ',
               value: 'DV1-CH2'
           }
        ]);
        profileCache.put('typeXKs', [
           {
               text: 'Xuất',
               value: 0
           },
           {
               text: 'Nhập',
               value: 1
           }
        ]);
        profileCache.put('status', [
            {
                text: 'Sử dụng',
                value: 10
            },
            {
                text: 'Không sử dụng',
                value: 0
            }
        ]);
        profileCache.put('trangThaiVoucher', [
            {
                text: 'Chưa sử dụng',
                value: 10
            },
            {
                text: 'Đã sử dụng',
                value: 30
            },
            {
                text: 'Chưa kích hoạt',
                value: 0
            }
        ]);
        profileCache.put('trangThaiThanhToan', [
            {
                text: 'Đã thanh toán',
                value: 10
            },
            {
                text: 'Chưa thanh toán',
                value: 0
            }
        ]);
        profileCache.put('trangThai', [
           {
               text: 'Đã duyệt',
               value: 10
           },
           {
               text: 'Chưa duyệt',
               value: 0
           }
        ]);
        profileCache.put('sieuthi', [
           {
               text: 'Siêu thị Dabaco Lý Thái Tổ',
               value: 'DV1-CH2'
           },
           {
               text: 'Siêu thị Dabaco Từ Sơn',
               value: 'DV1-CH1'
           },
           {
               text: 'Siêu thị Dabaco Quế Võ',
               value: 'DV1-CH4'
           },
           {
               text: 'Siêu thị Dabaco Gia Bình',
               value: 'DV1-CH6'
           }
        ]);
        profileCache.put('trangThaiDonHang', [
                { value: 1, text: 'Mới' },
                { value: 2, text: 'Đang xác nhận' },
                { value: 3, text: 'Đã xác nhận' },
                { value: 4, text: 'Đơn đang chuyển' },
                { value: 5, text: 'Đơn thành công' },
                { value: 6, text: 'Đơn thất bại' },
                { value: 7, text: 'Đơn chuyển hoàn' },
                { value: 8, text: 'Đơn trả lại' },
                { value: 9, text: 'Đơn đổi' },
                { value: 10, text: 'Đơn hủy' },
                { value: 11, text: 'Đơn hết' }
        ]);
        profileCache.put('hinhThucThanhToan', [
                { value: 'tienMat', text: 'Tiền mặt' },
                { value: 'the', text: 'Thẻ' },
                { value: 'cod', text: 'COD' }
        ]);
        var result = {
            dateFormat: 'dd/MM/yyyy',
            delegateEvent: function ($event) {
                $event.preventDefault();
                $event.stopPropagation();
            }
        };
        result.tempData = function (name) {
            return profileCache.get(name);
        }
        result.putTempData = function (module, data) {
            profileCache.put(module, data);
        }
        return result;
    }
    ]);
    return app;
});
