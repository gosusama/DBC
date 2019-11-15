define(['angular'], function (angular) {
    var app = angular.module('common-filter', []);
    app.filter('displayBool', function () {
        return function (input) {
            if (input === 1) {
                return "<i class='glyphicon glyphicon-ok text-success'></i>";
            }
        }
    });
    app.filter("approval", ['$filter', function ($filter) {
        return function (input) {
            if (input) {
                if (input == 10) {
                    return "Hoàn thành";
                }
                if (input == 20) {
                    return "Đã duyệt";
                }
                if (input == 0) {
                    return "Chưa duyệt";
                }
            }
            return "Chưa duyệt";
        };
    }]);
    app.filter("stateTranferFilter", [
        '$filter', function($filter) {
            return function(input) {
                if (input) {
                    if (input == "isRecievedButNotApproval") {
                        return "Đang xử lý"
                    }
                    if (input == "IsComplete") {
                        return "Đã xử lý"
                    }

                }
                return "Chờ xử lý";
            }
        }
    ]);
    app.filter("closedFilter", ['$filter', function ($filter) {
        return function (input) {
            if (input) {
                if (input == 10) {
                    return "Đã khóa";
                }
                if (input == 0) {
                    return "Chưa Khóa";
                }
            }
            return "";
        };
    }]);
    app.filter("approvalKiemKe", ['$filter', function ($filter) {
        return function (input) {
            if (input) {
                if (input === 10) {
                    return "Hoàn thành";
                }
                if (input === 20) {
                    return "Lưu tạm";
                }
                if (input === 30) {
                    return "Đã chỉnh sửa";
                }
            }
            return "Phiếu chờ";
        };
    }]);
    app.filter("orderstate", ['$filter', function ($filter) {
        return function (input) {
            if (input) {
                if (input == 10) {
                    return "Hoàn thành";
                }
                if (input == 20) {
                    return "Đã duyệt";
                }
                if (input == 0) {
                    return "Chưa duyệt";
                }
                if (input == 30) {
                    return "Đã nhập hàng";
                }
            }
            return "Chưa duyệt";
        };
    }]);
    app.filter("statusDatHangNCC", ['$filter', function ($filter) {
        return function (input) {
            if (input && input == 30) {
                return "Đã giao nhận";
            }
            return "Chưa giao nhận";
        };
    }]);
    app.filter('status', function () {
        return function (input) {
            if (input === 10) {
                return "Sử dụng";
            }
            return "Không sử dụng";
        }
    });
    app.filter("statusVoucher", ['$filter', function ($filter) {
        return function (input) {
            if (input) {
                if (input == 10) {
                    return "Chưa sử dụng";
                }
                if (input == 30) {
                    return "Đã sử dụng";
                }
                if (input == 0) {
                    return "Chưa kích hoạt";
                }
            }
            return "Chưa duyệt";
        };
    }]);
    app.filter('words', function () {
        function isInteger(x) {
            return x % 1 === 0;
        }
        var th = ['đồng', ' ngàn đồng', 'triệu', 'tỷ'];
        var dg = ['không', 'một', 'hai', 'ba', 'bốn', 'năm', 'sáu', 'bảy', 'tám', 'chín'];
        var tn = ['mười', 'mười một', 'mười hai', 'mười ba', 'mười bốn', 'mười năm', 'mười sáu', 'mười bảy', 'mười tám', 'mười chín'];
        var tw = ['hai mươi', 'ba mươi', 'bốn mươi', 'năm mươi', 'sáu mươi', 'bảy mươi', 'tám mươi', 'chín mươi'];


        function toWords(s) {
            s = s.toString();
            s = s.replace(/[\, ]/g, '');
            if (s != parseFloat(s)) return 'not a number';
            var x = s.indexOf('.');
            if (x == -1) x = s.length;
            if (x > 15) return 'too big';
            var n = s.split('');
            var str = '';
            var sk = 0;
            for (var i = 0; i < x; i++) {
                if ((x - i) % 3 == 2) {
                    if (n[i] == '1') {
                        str += tn[Number(n[i + 1])] + ' ';
                        i++;
                        sk = 1;
                    }
                    else if (n[i] != 0) {
                        str += tw[n[i] - 2] + ' ';
                        sk = 1;
                    }
                }
                else if (n[i] != 0) {
                    str += dg[n[i]] + ' ';
                    if ((x - i) % 3 == 0) str += 'trăm ';
                    sk = 1;
                }


                if ((x - i) % 3 == 1) {
                    if (sk) str += th[(x - i - 1) / 3] + ' ';
                    sk = 0;
                }
            }
            if (x != s.length) {
                var y = s.length;
                str += 'point ';
                for (var i = x + 1; i < y; i++) str += dg[n[i]] + ' ';
            }
            return str.replace(/\s+/g, ' ');
        }
        return function (value) {
            if (value && isInteger(value))
                return toWords(value);

            return value;
        };

    });
    app.filter("statusDathang", ['$filter', function ($filter) {
        return function (input) {
            if (input) {
                if (input == 1) {
                    return "Mới";
                }
                if (input == 2) {
                    return "Đang xác nhận";
                }
                if (input == 3) {
                    return "Đã xác nhận";
                }
                if (input == 4) {
                    return "Đơn đang chuyển";
                }
                if (input == 5) {
                    return "Đơn thành công";
                }
                if (input == 6) {
                    return "Đơn thất bại";
                }
                if (input == 7) {
                    return "Đơn chuyển hoàn";
                }
                if (input == 8) {
                    return "Đơn trả lại";
                }
                if (input == 9) {
                    return "Đơn đổi";
                }
                if (input == 10) {
                    return "Đơn hủy";
                }
                if (input == 11) {
                    return "Đơn hết";
                }
            }
            return "";
        };
    }]);

    app.filter("statusThanhToan", ['$filter', function ($filter) {
        return function (input) {
            if (input) {
                if (input == 1) {
                    return "Đã thanh toán";
                }
                if (input == 0) {
                    return "Chưa thanh toán";
                }
            }
            return "";
        };
    }]);
    return app;
});