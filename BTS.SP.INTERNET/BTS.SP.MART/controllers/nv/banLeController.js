/*  
* Người tạo : Nguyễn Tuấn Hoàng Anh
* View: BTS.SP.MART/views/nv/NvBanLe
* Vm sevices: BTS.API.SERVICE -> NV ->NvBanLeVm.cs
* Sevices: BTS.API.SERVICE -> NV -> NvBanLeService.cs
* Entity: BTS.API.ENTITY -> NV - > NvBanLe.cs
* Menu: Nghiệp vụ-> Bán lẻ quầy thu ngân
*/
define(['ui-bootstrap', '/BTS.SP.MART/controllers/auth/AuthController.js', '/BTS.SP.MART/controllers/htdm/periodController.js', '/BTS.SP.MART/controllers/htdm/merchandiseController.js', '/BTS.SP.MART/controllers/htdm/customerController.js', '/BTS.SP.MART/controllers/htdm/merchandiseTypeController.js', '/BTS.SP.MART/controllers/htdm/nhomVatTuController.js', '/BTS.SP.MART/controllers/htdm/supplierController.js', '/BTS.SP.MART/controllers/htdm/wareHouseController.js', '/BTS.SP.MART/controllers/htdm/packagingController.js', '/BTS.SP.MART/controllers/htdm/taxController.js', '/BTS.SP.MART/controllers/htdm/donViTinhController.js', '/BTS.SP.MART/controllers/nv/giaoDichQuayController.js'], function () {
    'use strict';
    var app = angular.module('banLeModule', ['ui.bootstrap', 'authModule', 'periodModule', 'merchandiseModule', 'customerModule', 'merchandiseTypeModule', 'nhomVatTuModule', 'supplierModule', 'wareHouseModule', 'packagingModule', 'taxModule', 'donViTinhModule', 'giaoDichQuayModule']);
    app.factory('banLeService', ['$http', 'configService', function ($http, configService) {
        var serviceUrl = configService.rootUrlWebApi + '/Nv/BanLe';
        var rootUrl = configService.apiServiceBaseUri;
        var result = {
            //lấy dữ liệu kỳ khóa sổ
            getPeriod: function () {
                return $http.get(serviceUrl + '/GetPeriodDate');
            },
            writeLog: function (data) {
                return $http.post(serviceUrl + '/WriteLog', data);
            },
            //sinh mã giao dịch
            buildCodeTrade: function () {
                return $http.post(serviceUrl + '/BuildCodeTrade');
            },
            getPackageMerchandise: function (maBoHang) {
                return $http.get(serviceUrl + '/GetDataPackageMerchandise/' + maBoHang);
            },
            //lưu dữ liệu bán
            post: function (data) {
                return $http.post(serviceUrl + '/Post', data);
            },
            //lấy toàn bộ giao dịch 1 ngày trước
            getAllDataTrade: function (codeTrade) {
                return $http.get(serviceUrl + '/GetAllDataTrade/' + codeTrade);
            },
            //lấy chi tiết giao dịch con khi click 1 row
            getDataDetailsGDQuay: function (data) {
                return $http.post(serviceUrl + '/GetDataDetailsGDQuay', data);
            },
            filterCustomerData: function (strKey) {
                return $http.get(serviceUrl + '/FilterCustomerData/' + strKey);
            },
            //lấy dữ liệu hàng hóa khi tìm kiếm
            postDataMerchandise: function (data) {
                return $http.post(serviceUrl + '/PostDataMerchandise', data);
            },
            getUserByUnitCode: function () {
                return $http.post(serviceUrl + '/GetUserByUnitCode');
            },
            getKhuyenMaiCombo: function () {
                return $http.get(serviceUrl + '/GetKhuyenMaiCombo');
            },
            getKhuyenMaiHangTangHang: function () {
                return $http.get(serviceUrl + '/GetKhuyenMaiHangTangHang');
            },
            checkInventory: function (code) {
                return $http.get(serviceUrl + '/CheckInventory/' + code);
            },
            historyBuy: function (para) {
                return $http.get(serviceUrl + '/HistoryBuyOfCustomer/' + para);
            },
            getHangKhachHang: function (hangKhachHang) {
                return $http.get(serviceUrl + '/GetHangKhachHang/' + hangKhachHang);
            },
            kiemTraKhoaBanAm: function () {
                return $http.get(serviceUrl + '/KiemTraKhoaBanAm');
            }
        }
        return result;
    }]);
    /* controller list */
    app.controller('banLeController', [
        '$scope', '$location', '$http', 'configService', 'banLeService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'securityService', '$rootScope', 'toaster', 'periodService', 'merchandiseService', 'customerService', 'merchandiseTypeService', 'nhomVatTuService', 'supplierService', 'wareHouseService', 'packagingService', 'taxService', 'donViTinhService', 'userService', 'giaoDichQuayService', 'keyCodes', 'accountService', 'AuDonViService',
        function ($scope, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, securityService, $rootScope, toaster, servicePeriod, serviceMerchandise, serviceCustomer, serviceMerchandiseType, serviceNhomVatTu, serviceSupplier, serviceWareHouse, servicePackaging, serviceTax, serviceDonViTinh, serviceAuthUser, serviceGiaoDichQuay, keyCodes, accountService, serviceAuthDonVi) {
            $scope.currentUser = serviceAuthUser.GetCurrentUser();
            $scope.keys = keyCodes;
            $scope.khongBanAm = true;
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.filtered = angular.copy(configService.filterDefault);
            $scope.tempData = tempDataService.tempData;
            $scope.isEditable = true;
            $scope.accessList = {};
            //kiểm tra có khóa bán âm hay không
            service.kiemTraKhoaBanAm(function (response) {
                $scope.khongBanAm = response;
            });
            $scope.logOut = function () {
                accountService.logout();
            };
            $scope.data = {};
            $scope.target = {};
            $scope.disSoGiaoDich = true;
            $scope.disCheck = true;
            $scope.disMaVatTu = true;
            $scope.disSumSoLuong = true;
            $scope.disSoLuongBan = true;
            $scope.disSumTienHang = true;
            $scope.disTienKhuyenMai = true;
            $scope.disTienKhachCanTra = true;
            $scope.disTienKhachTra = true;
            $scope.disTienTraLai = true;
            $scope.disThanhToan = true;
            $scope.showMessage = false;
            $scope.disableF1 = true;
            $scope.disableF2 = false;
            $scope.disableF3 = true;
            $scope.disableF4 = true;
            $scope.disableF5 = true;
            $scope.disableF6 = true;
            $scope.disableF7 = true;
            $scope.disableMaKhachHang = false;
            $scope.disableGenKhachHang = false;
            $scope.disableNguoiMua = false;
            $scope.disableDienThoai = false;
            $scope.disableNgaySinh = false;
            $scope.disableEmail = false;
            $scope.disableDiaChi = false;
            $scope.disableNgayDacBiet = false;
            $scope.thanhToanTienMat = true;
            $scope.status = true; //tăng hàng
            $scope.showPopup = 0; //tăng hàng
            $scope.lstVatTu_Top = [];
            $scope.lstVatTu = [];
            $scope.showClassTrungTam = true;
            $scope.showClassTraLai = false;
            $scope.trangThaiGiaoDich = 0; //bắt đầu giao dịch
            //khởi tạo biến toàn cục lưu giá trị toàn trang
            $scope.viewTraCuu = false;
            $scope.currentCount = 0;
            $scope.soLuongBan = 0;
            $scope.currentCode = '';
            $scope.target.khachThanhToan = 0;
            $scope.target.khachCanTra = 0;
            $scope.popup = "";
            $scope.result = {};
            $scope.listNhanVien = {};
            $scope.listTempNhanVien = [];
            $scope.currentMoney = 0;
            $scope.currentMoneyTemp = 0;
            $scope.currentSumTienHangTemp = 0;
            $scope.logKhuyenMaiChietKhau = false;
            $scope.logKhuyenMaiDongGia = false;
            $scope.logKhuyenMaiBuy1Get1 = false;
            $scope.logKhuyenMaiCombo = false;
            $scope.logKhuyenMaiSaleTinhTien = false;
            $scope.logKhuyenMaiNhanDoiTichDiem = false;
            $scope.logKhuyenMaiVoucher = false;
            $scope.logChietKhauTay = false;
            $scope.logChietKhauSinhNhat = false;
            var listCopy = [];
            //check lấy giá bán buôn hoặc bán lẻ
            var banBuon = false;
            var banLe = true;
            //end check
            function pad(d) {
                return (d < 10) ? '0' + d.toString() : d.toString();
            };
            //show thông báo
            function showNotification(message) {
                if (message != '') {
                    $scope.message = message;
                    var popup = document.getElementById('popupMessage').style.display = 'block';
                    $("#popupMessage").delay(1500).fadeOut(300);
                }
            };
            //end show thông báo

            //service.getUserByUnitCode().then(function (response) {
            //    if (response && response.status === 200 && response.data && response.data.status) {
            //        $scope.listNhanVien = response.data.data;
            //        $scope.listTempNhanVien = angular.copy($scope.listNhanVien);
            //    }
            //});

            //update dòng được chọn chuyển lên trên cùng
            function updateList(list) {
                if (list.length > 1) {
                    var temp = {};
                    temp = list[0];
                    list[0] = list[list.length - 1];
                    list[list.length - 1] = temp;
                }
            };
            $scope.tabs = [
            {
                title: 'Bán hàng 01',
                dataDto: {
                    maNhanVien: $scope.currentUser.userName,
                    tenNhanVien: $scope.currentUser.fullName,
                    userName: $scope.currentUser.userName,
                    chucVu: $scope.currentUser.chucVu,
                    gioiTinh: $scope.currentUser.gioiTinh,
                    soDienThoai: $scope.currentUser.soDienThoai,
                    unitCode: $scope.currentUser.unitCode,
                    ngayChungTu: '',
                    maGiaoDich: '',
                    loaiGiaoDich: 1,
                    ngayPhatSinh: '',
                    ghiChu: '',
                    sumSoLuong: 0,
                    sumTienHang: 0,
                    tienVoucher: 0,
                    khachCanTra: 0,
                    tienKhachDua: 0,
                    tienThua: 0,
                    tienKhuyenMai: 0,
                    makh: '',
                    tenKH: '',
                    theNhanTien: '',
                    voucher: '',
                    dienThoai: '',
                    phieuDatCoc: '',
                    ngaySinh: '',
                    tienDatCoc: 0,
                    email: '',
                    maThe: '',
                    diaChi: '',
                    ngayHetHan: '',
                    ngayDacBiet: '',
                    quenThe: 0,
                    tienThe: 0,
                    tienCOD: 0,
                    tienMat: 0
                },
                dataDetails: []
            }];
            if ($scope.currentUser.maMayBan) {
                console.log('Đã chọn máy bán');
            };
            service.getPeriod().then(function (response) {
                if (response && response.status === 200 && response.data && response.data.trangThai === 10) {
                    $scope.tabs[$scope.tabHienTai].dataDto.ngayChungTu = $filter('date')(response.data.toDate, "MM-dd-yyyy");
                    $scope.tabs[$scope.tabHienTai].dataDto.ngayPhatSinh = $filter('date')(response.data.toDate, "MM-dd-yyyy");;
                }
                else {
                    $scope.tabs[$scope.tabHienTai].dataDto.ngayChungTu = $filter('date')(new Date, "MM-dd-yyyy");
                    $scope.tabs[$scope.tabHienTai].dataDto.ngayPhatSinh = $filter('date')(response.data.toDate, "MM-dd-yyyy");;
                }
            });
            //chế độ gõ nhanh
            $scope.allowInputNumber = false;
            $scope.allowEnterNumber = function () {
                if (!$scope.allowInputNumber) {
                    ngNotify.set("Kích hoạt CT Khuyến mại thành công", { type: 'success' });
                    $scope.allowInputNumber = true;
                }
                else {
                    $scope.allowInputNumber = false;
                }
                $('#MAVATTUSEARCHFOCUS').focus();
            };
            //end
            //tính toán tổng tiền giao dịch
            function sumGiaoDich(lst) {
                var currentTab = $scope.tabs[$scope.tabHienTai];
                if (lst.length !== 0) {
                    var tongSoLuongBan = lst.reduce((tongSoLuong, x) => tongSoLuong + x.soLuong, 0);
                    var tongThanhTien = lst.reduce((tongThanhTien, y) => tongThanhTien + y.thanhTien, 0);
                    var tongTienKhuyenMai = 0;
                    var tienPhaiTra = lst.reduce((tienPhaiTra, y) => tienPhaiTra + y.thanhTien, 0);
                    currentTab.dataDto.khachCanTra = tienPhaiTra;
                    currentTab.dataDto.sumSoLuong = tongSoLuongBan;
                    currentTab.dataDto.sumTienHang = tongThanhTien;
                    currentTab.dataDto.sumTienKhuyenMai = tongTienKhuyenMai;
                    currentTab.dataDto.tienKhachDua = parseInt(tienPhaiTra, 10);
                }
                return currentTab;
            };
            //end 
            function total(list) {
                if (list.length > 0) {
                    var total = 0;
                    for (var i = 0; i < list.length; i++) {
                        var number = list[i].thanhTien;
                        total += number;
                    }
                    return total;
                }
            };
            function addInitKhuyenMai(rowData) {
                var currentTab = $scope.tabs[$scope.tabHienTai];
                if (rowData != null) {
                    var index = currentTab.dataDetails.findIndex(x => x.maVatTu === rowData.maVatTu);
                    if (rowData.isTichDiem === true) {
                        console.log('km tích điểm');
                    }
                    switch (rowData.loaiKhuyenMai) {
                        case 'DongGia':
                            console.log('km DongGia');
                            if (rowData.giaTriKhuyenMai_DongGia !== 0) {
                                //write log
                                $scope.logKhuyenMaiDongGia = true;
                                rowData.tienKhuyenMaiDongGia = rowData.giaTriKhuyenMai_DongGia;
                                rowData.tienDuocKhuyenMai = rowData.soLuong * rowData.giaBanLeVat - rowData.soLuong * rowData.tienKhuyenMaiDongGia;
                                rowData.tienChietKhau = rowData.tienDuocKhuyenMai;
                                rowData.tyLeChietKhau = 100 * (rowData.soLuong * rowData.giaBanLeVat - rowData.tienDuocKhuyenMai) / (rowData.soLuong * rowData.giaBanLeVat);
                                rowData.thanhTienTruocKm = rowData.soLuong * rowData.giaBanLeVat;
                                rowData.thanhTien = rowData.soLuong * rowData.giaTriKhuyenMai_DongGia;
                            }
                            break;

                        case 'TinhTien':
                            console.log('km TinhTien');
                            if (rowData.tyLeKhuyenMai_TinhTien !== 0) {
                                //write log
                                $scope.logKhuyenMaiSaleTinhTien = true;
                                rowData.isTinhTien = true;
                                var index = currentTab.dataDetails.findIndex(x => x.isTinhTien === true);
                                if (index === -1) {
                                    rowData.isTinhTien = true;
                                    rowData.countTinhTien = 1;
                                    rowData.tyLeChietKhau = rowData.countTinhTien * rowData.tyLeBatDau_TinhTien;
                                    rowData.tienKhuyenMai = (rowData.countTinhTien * rowData.tyLeBatDau_TinhTien * rowData.giaBanLeVat) / 100;
                                    rowData.tienDuocKhuyenMai = (rowData.countTinhTien * rowData.tyLeBatDau_TinhTien * rowData.giaBanLeVat) / 100;
                                    rowData.thanhTien = rowData.soLuong * rowData.giaBanLeVat - rowData.tienDuocKhuyenMai;
                                    rowData.hangKhuyenMai = true;
                                    rowData.thanhTienTruocKm = rowData.soLuong * rowData.giaBanLeVat;
                                    console.log('1:', rowData.countTinhTien);
                                }
                                else {
                                    //đã có 1 mã nằm trong chương trình này rồi
                                    //lấy ra mã chương trình của hàng trước
                                    //write log
                                    var maChuongTrinhKhuyenMai = currentTab.dataDetails[index].maChuongTrinhKhuyenMai;
                                    //check mã vật tư này đã có trong danh sách hàng hóa chưa -- kèm chương trình tịnh tiến thì sinh thêm dòng mới
                                    var indexMaVatTu = currentTab.dataDetails.findIndex(x => x.maVatTu === rowData.maVatTu);
                                    if (rowData.maChuongTrinhKhuyenMai === maChuongTrinhKhuyenMai) {
                                        //cùng nằm trong 1 chương trình khuyến mại tịnh tiến
                                        //update rowCountTinhTien của dòng hàng 1
                                        if (currentTab.dataDetails[indexMaVatTu].tienDuocKhuyenMai > 0) { //thêm mã mới
                                            var rowNew = {};
                                            var maxCountTinhTien = 0;
                                            //lấy ra countTinhTien lớn nhất trong toàn list
                                            currentTab.dataDetails[indexMaVatTu].soLuong = currentTab.dataDetails[indexMaVatTu].soLuong - 1;
                                            rowNew = angular.copy(currentTab.dataDetails[indexMaVatTu]);
                                            angular.forEach(currentTab.dataDetails, function (value, index) {
                                                if (value.countTinhTien) {

                                                }
                                                else {
                                                    value.countTinhTien = 0;
                                                }
                                            });
                                            maxCountTinhTien = Math.max.apply(Math, currentTab.dataDetails.map(function (item) { return item.countTinhTien; }));
                                            maxCountTinhTien++;
                                            rowNew.countTinhTien = maxCountTinhTien;
                                            rowNew.hangKhuyenMai = true;
                                            rowNew.tyLeChietKhau = rowNew.countTinhTien * rowNew.tyLeBatDau_TinhTien;
                                            rowNew.tienKhuyenMai = (rowNew.tyLeKhuyenMai_TinhTien * rowNew.countTinhTien * rowNew.giaBanLeVat) / 100;
                                            rowNew.tienDuocKhuyenMai = (rowNew.countTinhTien * rowNew.tyLeBatDau_TinhTien * rowNew.giaBanLeVat) / 100;
                                            rowNew.thanhTien = rowNew.soLuong * rowNew.giaBanLeVat - rowNew.tienDuocKhuyenMai;
                                            currentTab.dataDetails.push(rowNew);
                                            updateList(currentTab.dataDetails);
                                            rowData.countTinhTien++;
                                        }
                                        else {
                                            currentTab.dataDetails[index].countTinhTien++;
                                            rowData.countTinhTien = currentTab.dataDetails[index].countTinhTien;
                                            rowData.tienKhuyenMai = (rowData.tyLeKhuyenMai_TinhTien * currentTab.dataDetails[index].countTinhTien * rowData.giaBanLeVat) / 100;
                                            rowData.tienDuocKhuyenMai = (currentTab.dataDetails[index].countTinhTien * rowData.tyLeKhuyenMai_TinhTien * rowData.giaBanLeVat) / 100;
                                            rowData.thanhTien = rowData.soLuong * rowData.giaBanLeVat - rowData.tienDuocKhuyenMai;
                                            rowData.hangKhuyenMai = true;
                                            rowData.tyLeChietKhau = rowData.countTinhTien * rowData.tyLeBatDau_TinhTien;
                                            rowData.thanhTienTruocKm = rowData.soLuong * rowData.giaBanLeVat;
                                            console.log('2:', rowData.countTinhTien);
                                        }
                                    }
                                    else if (rowData.maChuongTrinhKhuyenMai !== maChuongTrinhKhuyenMai) {
                                        //nếu có km tịnh tiến nhưng nằm trong chương trình khác
                                        rowData.isTinhTien = true;
                                        rowData.countTinhTien = 1;
                                        rowData.tienKhuyenMai = (rowData.countTinhTien * rowData.tyLeBatDau_TinhTien * rowData.giaBanLeVat) / 100;
                                        rowData.tienDuocKhuyenMai = (rowData.countTinhTien * rowData.tyLeBatDau_TinhTien * rowData.giaBanLeVat) / 100;
                                        rowData.thanhTien = rowData.soLuong * rowData.giaBanLeVat - rowData.tienDuocKhuyenMai;
                                        rowData.hangKhuyenMai = true;
                                        rowData.countTinhTien++;
                                        rowData.thanhTienTruocKm = rowData.soLuong * rowData.giaBanLeVat;
                                        console.log('3:', rowData.countTinhTien);
                                    }
                                }
                            }
                            break;
                        case 'ChietKhau':
                            if (rowData.tyLeKhuyenMai_ChietKhau !== 0 || rowData.giaTriKhuyenMai_ChietKhau !== 0) { //khuyến mại 
                                console.log('km ChietKhau');
                                //write log
                                $scope.logKhuyenMaiChietKhau = true;
                                if (rowData.tyLeKhuyenMai_ChietKhau <= 100) {
                                    //KHUYẾN MẠI TỶ LỆ
                                    rowData.tyLeChietKhau = rowData.tyLeKhuyenMai_ChietKhau;
                                    rowData.tienChietKhau = rowData.tyLeKhuyenMai_ChietKhau * rowData.soLuong * rowData.giaBanLeVat / 100;
                                    rowData.tienKhuyenMaiChietKhau = rowData.tyLeKhuyenMai_ChietKhau * rowData.soLuong * rowData.giaBanLeVat / 100;
                                    rowData.tienDuocKhuyenMai = rowData.tyLeKhuyenMai_ChietKhau * rowData.soLuong * rowData.giaBanLeVat / 100;
                                    rowData.thanhTien = rowData.soLuong * rowData.giaBanLeVat - rowData.tienDuocKhuyenMai;
                                    rowData.thanhTienTruocKm = rowData.soLuong * rowData.giaBanLeVat;
                                }
                                else if (rowData.giaTriKhuyenMai_ChietKhau > 100) {
                                    //KHUYẾN MẠI TIỀN
                                    rowData.tyLeChietKhau = 100 * (rowData.giaBanLeVat - rowData.giaTriKhuyenMai_ChietKhau) / rowData.giaBanLeVat;
                                    rowData.tienChietKhau = rowData.giaTriKhuyenMai_ChietKhau;
                                    rowData.tienKhuyenMaiChietKhau = rowData.giaTriKhuyenMai_ChietKhau * rowData.soLuong;
                                    rowData.tienDuocKhuyenMai = rowData.giaTriKhuyenMai_ChietKhau * rowData.soLuong;
                                    rowData.thanhTien = rowData.soLuong * rowData.giaBanLeVat - rowData.tienDuocKhuyenMai;
                                    rowData.thanhTienTruocKm = rowData.soLuong * rowData.giaBanLeVat;
                                }
                            }
                            break;
                        case 'Buy1Get1':
                            var lstHangTangHang = [];
                            var listMin = [];
                            var listMax = [];
                            $scope.hangAddKhuyenMai = {};
                            console.log('km Buy1Get1');
                            rowData.thanhTienTruocKm = rowData.soLuong * rowData.giaBanLeVat;
                            rowData.thanhTien = rowData.soLuong * rowData.giaBanLeVat;
                            if ($rootScope.listHangTangHang && $rootScope.listHangTangHang.length > 0) {
                                //write log
                                $scope.logKhuyenMaiBuy1Get1 = true;
                                angular.forEach(currentTab.dataDetails, function (v, k) {
                                    var idx = $rootScope.listHangTangHang.findIndex(x => x.maVatTu === v.maVatTu);
                                    if (idx !== -1) lstHangTangHang.push(v);
                                });
                                if (lstHangTangHang.length > 0 && lstHangTangHang.length % 2 === 0) {
                                    var countHangTang = lstHangTangHang.length / 2;
                                    lstHangTangHang.sort(function (a, b) {
                                        return parseFloat(a.giaBanLeVat) - parseFloat(b.giaBanLeVat);
                                    });
                                    console.log('lstHangTangHang:', lstHangTangHang);
                                    for (var i = 0; i < lstHangTangHang.length; i++) {
                                        if (i < countHangTang) listMin.push(lstHangTangHang[i]);
                                        else listMax.push(lstHangTangHang[i]);
                                    }
                                    if (listMax.length > 0) {
                                        for (var max = 0; max < listMax.length; max++) {
                                            var idxMax = currentTab.dataDetails.findIndex(x => x.maVatTu === listMax[max].maVatTu);
                                            if (idxMax !== -1) {
                                                currentTab.dataDetails[idxMax].thanhTienTruocKm = currentTab.dataDetails[idxMax].soLuong * currentTab.dataDetails[idxMax].giaBanLeVat;
                                                currentTab.dataDetails[idxMax].thanhTien = currentTab.dataDetails[idxMax].soLuong * currentTab.dataDetails[idxMax].giaBanLeVat;
                                            }
                                        }
                                    }
                                    if (listMin.length > 0) {
                                        for (var min = 0; min < listMin.length; min++) {
                                            var idxMin = currentTab.dataDetails.findIndex(x => x.maVatTu === listMin[min].maVatTu);
                                            if (idxMin !== -1) {
                                                currentTab.dataDetails[idxMin].thanhTienTruocKm = currentTab.dataDetails[idxMin].soLuong * currentTab.dataDetails[idxMin].giaBanLeVat;
                                                currentTab.dataDetails[idxMin].thanhTien = 0;
                                                currentTab.dataDetails[idxMin].tienChietKhau = currentTab.dataDetails[idxMin].soLuong * currentTab.dataDetails[idxMin].giaBanLeVat;
                                                currentTab.dataDetails[idxMin].tyLeChietKhau = 100;
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                        case 'Combo':
                            $scope.hangAddKhuyenMai = {};
                            if (rowData.giaTriKhuyenMai_Combo > 0) {
                                console.log('km Combo');
                                //write log
                                $scope.logKhuyenMaiCombo = true;
                                rowData.tienDuocKhuyenMai = 0;
                                rowData.thanhTien = rowData.soLuong * rowData.giaBanLeVat - rowData.tienDuocKhuyenMai;
                                rowData.thanhTienTruocKm = rowData.soLuong * rowData.giaBanLeVat;
                                //check mavattu left
                                if ($rootScope.listComboLeft && $rootScope.listComboLeft.length > 0) {
                                    if (!$rootScope.left) {
                                        $rootScope.left = {
                                            countL: {},
                                            maVatTu: []
                                        };
                                        $rootScope.left.countL = 0;
                                    }
                                    if (!$rootScope.right) {
                                        $rootScope.right = {
                                            countR: {},
                                            maVatTu: []
                                        };
                                        $rootScope.right.countR = 0;
                                    }
                                    $rootScope.left.countL = 0;
                                    $rootScope.left.maVatTu = [];
                                    for (var i = 0; i < currentTab.dataDetails.length; i++) {
                                        var idx = $rootScope.listComboLeft.findIndex(x => x.maVatTuLeft === currentTab.dataDetails[i].maVatTu);
                                        if (idx !== -1) {
                                            $rootScope.left.countL = $rootScope.left.countL + currentTab.dataDetails[i].soLuong;
                                            var idxVt = $rootScope.left.maVatTu.findIndex(x => x === currentTab.dataDetails[i].maVatTu);
                                            if (idxVt === -1) $rootScope.left.maVatTu.push(currentTab.dataDetails[i].maVatTu);
                                        }
                                    }
                                    $scope.khuyenMaiCombo($rootScope.left, $rootScope.right);
                                    //console.log('$rootScope.left:',$rootScope.left);
                                }
                            }
                            break;
                        default:
                            console.log('không nằm trong chương trình khuyến mại nào');
                            //check mavattu right
                            if ($rootScope.listComboRight && $rootScope.listComboRight.length > 0) {// && rowData.logRightCombo) {
                                if (!$rootScope.right) {
                                    $rootScope.right = {
                                        countR: {},
                                        maVatTu: []
                                    };
                                    $rootScope.right.countR = 0;
                                }
                                if (!$rootScope.left) {
                                    $rootScope.left = {
                                        countL: {},
                                        maVatTu: []
                                    };
                                    $rootScope.left.countL = 0;
                                }
                                $rootScope.right.countR = 0;
                                $rootScope.right.maVatTu = []
                                for (var i = 0; i < currentTab.dataDetails.length; i++) {
                                    var countRight = $rootScope.listComboRight.findIndex(x => x.maVatTuRight === currentTab.dataDetails[i].maVatTu);
                                    if (countRight !== -1) {
                                        $rootScope.right.countR = $rootScope.right.countR + currentTab.dataDetails[i].soLuong;
                                        var idxVt = $rootScope.right.maVatTu.findIndex(x => x === currentTab.dataDetails[i].maVatTu);
                                        if (idxVt === -1) $rootScope.right.maVatTu.push(currentTab.dataDetails[i].maVatTu);
                                    }
                                }
                                rowData.thanhTienTruocKm = rowData.soLuong * rowData.giaBanLeVat;
                                rowData.thanhTien = rowData.soLuong * rowData.giaBanLeVat;
                                $scope.khuyenMaiCombo($rootScope.left, $rootScope.right);
                                //console.log('$rootScope.right:',$rootScope.right);
                            }
                            else {
                                rowData.thanhTien = rowData.soLuong * rowData.giaBanLeVat;
                                rowData.thanhTienTruocKm = rowData.soLuong * rowData.giaBanLeVat;
                                rowData.tyLeChietKhau = 0;
                                rowData.tienDuocKhuyenMai = 0;
                                rowData.tienChietKhau = 0;
                                rowData.hangKhuyenMai = false;
                            }

                    } //end case            
                    sumGiaoDich(currentTab.dataDetails);
                    //currentTab.dataDto.tienMat = currentTab.dataDto.sumTienHang;
                    $scope.currentSumTienHangTemp = angular.copy(currentTab.dataDto.sumTienHang);
                    listCopy = angular.copy(currentTab.dataDetails);
                }
            }
            $scope.khuyenMaiCombo = function (left, right) {
                var currentTab = $scope.tabs[$scope.tabHienTai];
                var combo = [
                    {
                        soLuongCombo: 0,
                        giaTriCombo: 0,
                    }
                ];
                var combo2 = [
                    {
                        soLuongCombo: 0,
                        giaTriCombo: 0,
                    }
                ];
                if ($rootScope.listComboLeft && $rootScope.listComboLeft.length > 0) {
                    var groups = {};
                    var groups2 = {};
                    for (var i = 0; i < $rootScope.listComboLeft.length; i++) {
                        var groupName = $rootScope.listComboLeft[i].soLuongKhuyenMai;
                        if (!groups[groupName]) {
                            groups[groupName] = [];
                        }
                        groups[groupName].push($rootScope.listComboLeft[i].soLuongKhuyenMai);
                    }
                    for (var a = 0; a < $rootScope.listComboLeft.length; a++) {
                        var groupGiaTri = $rootScope.listComboLeft[a].giaTriKhuyenMai;
                        if (!groups2[groupGiaTri]) {
                            groups2[groupGiaTri] = [];
                        }
                        groups2[groupGiaTri].push($rootScope.listComboLeft[a].giaTriKhuyenMai);
                    }
                    for (var groupName in groups) {
                        combo.push({ soLuongCombo: groupName, giaTriCombo: 0 });
                    }
                    for (var groupGiaTri in groups2) {
                        combo2.push({ groupGiaTri});
                        }
                      var index = combo.findIndex(x => x.soLuongCombo === 0 && x.giaTriCombo === 0);
                      if(index != -1) combo.splice(index, 1);
                      var index2 = combo2.findIndex(x => x.soLuongCombo === 0 && x.giaTriCombo === 0);
                    if (index2 != -1) combo2.splice(index2, 1);
                    for (var b = 0; b < combo.length; b++) {
                        combo[b].giaTriCombo = combo2[b].groupGiaTri;
                    }
                }
                if (left.countL > 0 && right.countR > 0) {
                    var number = left.countL + right.countR;
                    for (var c = 0; c < combo.length ; c++) {
                        switch (parseInt(combo[c].soLuongCombo, 10)) {
                            case number:
                                //console.log('nhay vao ' + combo[c].giaTriCombo);
                                //console.log('left:',left);
                                //console.log('right:',right);
                                var countCombo = 0;
                                var tienChenhCombo = 0;
                                var thanhTienCombo = 0;
                                angular.forEach(left.maVatTu, function (v, k) {
                                    var index = currentTab.dataDetails.findIndex(x => x.maVatTu === v);
                                    if (index !== -1) {
                                        thanhTienCombo = thanhTienCombo + currentTab.dataDetails[index].soLuong * currentTab.dataDetails[index].giaBanLeVat;
                                        countCombo++;
                                    }
                                });
                                angular.forEach(right.maVatTu, function (v, k) {
                                    var index = currentTab.dataDetails.findIndex(x => x.maVatTu === v);
                                    if (index !== -1) {
                                        thanhTienCombo = thanhTienCombo + currentTab.dataDetails[index].soLuong * currentTab.dataDetails[index].giaBanLeVat;
                                        countCombo++;
                                    }
                                });
                                tienChenhCombo = thanhTienCombo - combo[c].giaTriCombo;
                                angular.forEach(left.maVatTu, function (v, k) {
                                    var index = currentTab.dataDetails.findIndex(x => x.maVatTu === v);
                                    if (index !== -1) {
                                        currentTab.dataDetails[index].tienChietKhau = tienChenhCombo / countCombo;
                                        currentTab.dataDetails[index].tyLeChietKhau = 100 * currentTab.dataDetails[index].tienChietKhau / currentTab.dataDetails[index].giaBanLeVat;
                                        currentTab.dataDetails[index].thanhTienTruocKm = currentTab.dataDetails[index].soLuong * currentTab.dataDetails[index].giaBanLeVat;
                                        currentTab.dataDetails[index].thanhTien = currentTab.dataDetails[index].soLuong * currentTab.dataDetails[index].giaBanLeVat - currentTab.dataDetails[index].tienChietKhau;
                                    }
                                });
                                angular.forEach(right.maVatTu, function (v, k) {
                                    var index = currentTab.dataDetails.findIndex(x => x.maVatTu === v);
                                    if (index !== -1) {
                                        currentTab.dataDetails[index].tienChietKhau = tienChenhCombo / countCombo;
                                        currentTab.dataDetails[index].tyLeChietKhau = 100 * currentTab.dataDetails[index].tienChietKhau / currentTab.dataDetails[index].giaBanLeVat;
                                        currentTab.dataDetails[index].thanhTienTruocKm = currentTab.dataDetails[index].soLuong * currentTab.dataDetails[index].giaBanLeVat;
                                        currentTab.dataDetails[index].thanhTien = currentTab.dataDetails[index].soLuong * currentTab.dataDetails[index].giaBanLeVat - currentTab.dataDetails[index].tienChietKhau;
                                    }
                                });
                                break;
                        }
                    }
                }
            };
            $scope.removeMaHang = function (index) {
                var currentTab = $scope.tabs[$scope.tabHienTai];
                currentTab.dataDetails.splice(index, 1);
                sumGiaoDich(currentTab.dataDetails);
            };
            //

            //lấy giá bán buôn
            $scope.layGiaBanBuon = function (event) {
                if (event.target.checked) {
                    banBuon = true;
                    banLe = false;
                } else {
                    banBuon = false;
                    banLe = true;
                }
            };
            //end lấy giá bán buôn
            $scope.searchMerchandise = function () {
                if ($scope.disableF2) {
                    var modalInstance = $uibModal.open({
                        backdrop: 'static',
                        templateUrl: configService.buildUrl('htdm/Merchandise', 'selectDataRetails'),
                        controller: 'vatTuSelectDataController',
                        size: 'lg',
                        resolve: {
                            serviceSelectData: function () {
                                return serviceMerchandise;
                            },
                            filterObject: function () {
                                return {
                                    summary: ''
                                };
                            }
                        }
                    });
                    modalInstance.result.then(function (updatedData) {
                        $scope.soLuong = 1;
                        var obj = {};
                        $scope.dataMerchandise = {};
                        var currentTab = $scope.tabs[$scope.tabHienTai];
                        $scope.dataSearch = { maVatTu: updatedData.maVatTu };
                        service.postDataMerchandise($scope.dataSearch).then(function (response) {
                            if (response && response.status === 200 && response.data.status) {
                                $scope.dataMerchandise = response.data.data;
                                obj =
                                {
                                    maVatTu: $scope.dataMerchandise.maVatTu,
                                    tenVatTu: $scope.dataMerchandise.tenVatTu,
                                    soLuong: $scope.soLuong === null ? 1 : $scope.soLuong,
                                    giaBanLeVat: (banLe == true ? $scope.dataMerchandise.giaBanLeVat : $scope.dataMerchandise.giaBanBuonVat),
                                    donGia: (banLe == true ? $scope.dataMerchandise.giaBanLeVat : $scope.dataMerchandise.giaBanBuonVat),
                                    thanhTien: $scope.soLuong * (banLe == true ? $scope.dataMerchandise.giaBanLeVat : $scope.dataMerchandise.giaBanBuonVat),
                                    maLoaiVatTu: $scope.dataMerchandise.maLoaiVatTu,
                                    maNhomVatTu: $scope.dataMerchandise.maNhomVatTu,
                                    maDonVi: $scope.dataMerchandise.maDonVi,
                                    giaVon: $scope.dataMerchandise.giaVon,
                                    maColor: $scope.dataMerchandise.maColor,
                                    tyLeVatRa: $scope.dataMerchandise.tyLeVatRa,
                                    tyLeVatVao: $scope.dataMerchandise.tyLeVatVao,
                                    maVatRa: $scope.dataMerchandise.maVatRa,
                                    maVatVao: $scope.dataMerchandise.maVatVao,
                                    donViTinh: $scope.dataMerchandise.donViTinh,
                                    avatar: $scope.dataMerchandise.avatar,
                                    image: $scope.dataMerchandise.image,
                                    path_image: $scope.dataMerchandise.path_image,
                                    trangThaiCon: $scope.dataMerchandise.trangThaiCon,
                                    giaTriKhuyenMai: $scope.dataMerchandise.giaTriKhuyenMai,
                                    tyLeKhuyenMai: $scope.dataMerchandise.tyLeKhuyenMai,
                                    noiDungKhuyenMai: $scope.dataMerchandise.noiDungKhuyenMai,
                                    maChuongTrinhKhuyenMai: $scope.dataMerchandise.maChuongTrinhKhuyenMai,
                                    soLuongKhuyenMai: $scope.dataMerchandise.soLuong_KhuyenMai,
                                    maKhoKhuyenMai: $scope.dataMerchandise.maKhoKhuyenMai,
                                    loaiKhuyenMai: $scope.dataMerchandise.loaiKhuyenMai,
                                    tyLeChietKhau: 0,
                                    tienChietKhau: 0,
                                    chietKhau: 0,
                                    tienThe: 0,
                                    tienCOD: 0,
                                    tienDuocKhuyenMai: 0,
                                    giaTriKhuyenMai_ChietKhau: $scope.dataMerchandise.giaTriKhuyenMai_ChietKhau,
                                    giaTriKhuyenMai_DongGia: $scope.dataMerchandise.giaTriKhuyenMai_DongGia,
                                    giaTriKhuyenMai_TichDiem: $scope.dataMerchandise.giaTriKhuyenMai_TichDiem,
                                    giaTriKhuyenMai_TinhTien: $scope.dataMerchandise.giaTriKhuyenMai_TinhTien,
                                    giaTriKhuyenMai_Voucher: $scope.dataMerchandise.giaTriKhuyenMai_Voucher,
                                    maHang_Km_Buy1Get1: $scope.dataMerchandise.maHang_Km_Buy1Get1,
                                    tenHang_Km_Buy1Get1: $scope.dataMerchandise.tenHang_Km_Buy1Get1,
                                    soLuong_Km_Buy1Get1: $scope.dataMerchandise.soLuong_Km_Buy1Get1,
                                    tyLeKhuyenMai_ChietKhau: $scope.dataMerchandise.tyLeKhuyenMai_ChietKhau,
                                    tyLeKhuyenMai_DongGia: $scope.dataMerchandise.tyLeKhuyenMai_DongGia,
                                    tyLeKhuyenMai_TichDiem: $scope.dataMerchandise.tyLeKhuyenMai_TichDiem,
                                    tyLeKhuyenMai_TinhTien: $scope.dataMerchandise.tyLeKhuyenMai_TinhTien,
                                    tyLeKhuyenMai_Voucher: $scope.dataMerchandise.tyLeKhuyenMai_Voucher,
                                    tyLeBatDau_TinhTien: $scope.dataMerchandise.tyLeBatDau_TinhTien,
                                    tuGio: $scope.dataMerchandise.tuGio,
                                    denGio: $scope.dataMerchandise.denGio,
                                    tonCuoiKySl: $scope.dataMerchandise.tonCuoiKySl === null ? 0 : $scope.dataMerchandise.tonCuoiKySl,
                                    isBanAm: $scope.dataMerchandise.isBanAm,
                                    isTichDiem: $scope.dataMerchandise.isTichDiem
                                };
                                if (currentTab.dataDetails.filter(x=>x.maVatTu === updatedData.maVatTu).length === 0) {
                                    currentTab.dataDetails.push(obj);
                                    var index = currentTab.dataDetails.findIndex(x => x.maVatTu === obj.maVatTu);
                                    $scope.selectedRow = 0;
                                    $scope.target.maVatTu = null;
                                    $('#SOLUONGFOCUS').focus();
                                    $scope.target.soLuong = obj.soLuong;
                                    addInitKhuyenMai(currentTab.dataDetails[index]);
                                }
                                else {
                                    //cộng dồn mã đã có trong danh sách
                                    var index = currentTab.dataDetails.findIndex(x => x.maVatTu === updatedData.maVatTu);
                                    if (currentTab.dataDetails[index].hangKhuyenMai) {
                                        currentTab.dataDetails[index].soLuong = currentTab.dataDetails[index].soLuong + 1;
                                    }
                                    else {
                                        currentTab.dataDetails[index].soLuong = currentTab.dataDetails[index].soLuong + 1;
                                    }
                                    if (index != null) {
                                        var temp = currentTab.dataDetails[currentTab.dataDetails.length - 1];
                                        currentTab.dataDetails[currentTab.dataDetails.length - 1] = currentTab.dataDetails[index];
                                        currentTab.dataDetails[index] = temp;
                                        $scope.selectedRow = 0;
                                        $scope.target.maVatTu = null;
                                    }
                                    $scope.currentCount = currentTab.dataDetails[currentTab.dataDetails.length - 1].soLuong
                                    $scope.soLuongBan = $scope.currentCount;
                                    addInitKhuyenMai(currentTab.dataDetails[index]);
                                }
                                $scope.soLuongBan = currentTab.dataDetails[currentTab.dataDetails.length - 1].soLuong;
                                //hàm tính tổng tiền
                                //reload lại tổng tiền, số lượng 
                                sumGiaoDich(currentTab.dataDetails);
                                listCopy = angular.copy(currentTab.dataDetails);
                                $scope.disableF4 = false;
                                $scope.disableF3 = false;
                            }
                        });
                    }, function () {
                    });
                }
                else {
                }
            };
            //end
            $scope.filterNotFoundMerchandise = function (strKey) {
                if ($scope.disableF2) {
                    var modalInstance = $uibModal.open({
                        backdrop: 'static',
                        templateUrl: configService.buildUrl('htdm/Merchandise', 'selectDataRetails'),
                        controller: 'vatTuSelectDataController',
                        size: 'lg',
                        resolve: {
                            serviceSelectData: function () {
                                return serviceMerchandise;
                            },
                            filterObject: function () {
                                return {
                                    summary: ''
                                };
                            }
                        }
                    });
                    modalInstance.result.then(function (updatedData) {
                        var obj = {};
                        $scope.dataMerchandise = {};
                        var currentTab = $scope.tabs[$scope.tabHienTai];
                        //gọi hàm để lấy ra giá trị khuyến mại của dòng đang chọn
                        $scope.dataSearch = { maVatTu: updatedData.maVatTu };
                        service.postDataMerchandise($scope.dataSearch).then(function (response) {
                            if (response && response.status === 200 && response.data.status) {
                                $scope.dataMerchandise = response.data.data;
                                obj =
                                {
                                    maVatTu: $scope.dataMerchandise.maVatTu,
                                    tenVatTu: $scope.dataMerchandise.tenVatTu,
                                    soLuong: $scope.soLuong,
                                    giaBanLeVat: (banLe == true ? $scope.dataMerchandise.giaBanLeVat : $scope.dataMerchandise.giaBanBuonVat),
                                    thanhTien: $scope.soLuong * (banLe == true ? $scope.dataMerchandise.giaBanLeVat : $scope.dataMerchandise.giaBanBuonVat),
                                    maLoaiVatTu: $scope.dataMerchandise.maLoaiVatTu,
                                    maNhomVatTu: $scope.dataMerchandise.maNhomVatTu,
                                    maDonVi: $scope.dataMerchandise.maDonVi,
                                    giaVon: $scope.dataMerchandise.giaVon,
                                    maColor: $scope.dataMerchandise.maColor,
                                    tyLeVatRa: $scope.dataMerchandise.tyLeVatRa,
                                    tyLeVatVao: $scope.dataMerchandise.tyLeVatVao,
                                    maVatRa: $scope.dataMerchandise.maVatRa,
                                    maVatVao: $scope.dataMerchandise.maVatVao,
                                    donViTinh: $scope.dataMerchandise.donViTinh,
                                    avatar: $scope.dataMerchandise.avatar,
                                    image: $scope.dataMerchandise.image,
                                    path_image: $scope.dataMerchandise.path_image,
                                    trangThaiCon: $scope.dataMerchandise.trangThaiCon,
                                    giaTriKhuyenMai: $scope.dataMerchandise.giaTriKhuyenMai,
                                    tyLeKhuyenMai: $scope.dataMerchandise.tyLeKhuyenMai,
                                    noiDungKhuyenMai: $scope.dataMerchandise.noiDungKhuyenMai,
                                    maChuongTrinhKhuyenMai: $scope.dataMerchandise.maChuongTrinhKhuyenMai,
                                    soLuongKhuyenMai: $scope.dataMerchandise.soLuong_KhuyenMai,
                                    maKhoKhuyenMai: $scope.dataMerchandise.maKhoKhuyenMai,
                                    loaiKhuyenMai: $scope.dataMerchandise.loaiKhuyenMai,
                                    tyLeChietKhau: 0,
                                    tienChietKhau: 0,
                                    chietKhau: 0,
                                    tienThe: 0,
                                    tienCOD: 0,
                                    tienDuocKhuyenMai: 0,
                                    giaTriKhuyenMai_ChietKhau: $scope.dataMerchandise.giaTriKhuyenMai_ChietKhau,
                                    giaTriKhuyenMai_DongGia: $scope.dataMerchandise.giaTriKhuyenMai_DongGia,
                                    giaTriKhuyenMai_TichDiem: $scope.dataMerchandise.giaTriKhuyenMai_TichDiem,
                                    giaTriKhuyenMai_TinhTien: $scope.dataMerchandise.giaTriKhuyenMai_TinhTien,
                                    giaTriKhuyenMai_Voucher: $scope.dataMerchandise.giaTriKhuyenMai_Voucher,
                                    maHang_Km_Buy1Get1: $scope.dataMerchandise.maHang_Km_Buy1Get1,
                                    tenHang_Km_Buy1Get1: $scope.dataMerchandise.tenHang_Km_Buy1Get1,
                                    soLuong_Km_Buy1Get1: $scope.dataMerchandise.soLuong_Km_Buy1Get1,
                                    tyLeKhuyenMai_ChietKhau: $scope.dataMerchandise.tyLeKhuyenMai_ChietKhau,
                                    tyLeKhuyenMai_DongGia: $scope.dataMerchandise.tyLeKhuyenMai_DongGia,
                                    tyLeKhuyenMai_TichDiem: $scope.dataMerchandise.tyLeKhuyenMai_TichDiem,
                                    tyLeKhuyenMai_TinhTien: $scope.dataMerchandise.tyLeKhuyenMai_TinhTien,
                                    tyLeKhuyenMai_Voucher: $scope.dataMerchandise.tyLeKhuyenMai_Voucher,
                                    tyLeBatDau_TinhTien: $scope.dataMerchandise.tyLeBatDau_TinhTien,
                                    tuGio: $scope.dataMerchandise.tuGio,
                                    denGio: $scope.dataMerchandise.denGio,
                                    tonCuoiKySl: $scope.dataMerchandise.tonCuoiKySl === null ? 0 : $scope.dataMerchandise.tonCuoiKySl,
                                    isBanAm: $scope.dataMerchandise.isBanAm,
                                    isTichDiem: $scope.dataMerchandise.isTichDiem
                                };
                                if (currentTab.dataDetails.filter(x=>x.maVatTu === updatedData.maVatTu).length === 0) {
                                    currentTab.dataDetails.push(obj);
                                    var index = currentTab.dataDetails.findIndex(x => x.maVatTu === obj.maVatTu);
                                    $scope.selectedRow = 0;
                                    $scope.target.maVatTu = null;
                                    $('#SOLUONGFOCUS').focus();
                                    $scope.target.soLuong = obj.soLuong;
                                    addInitKhuyenMai(currentTab.dataDetails[index]);
                                }
                                else {
                                    //cộng dồn mã đã có trong danh sách
                                    var index = currentTab.dataDetails.findIndex(x => x.maVatTu === updatedData.maVatTu);
                                    currentTab.dataDetails[index].soLuong = currentTab.dataDetails[index].soLuong + 1;
                                    if (index != null) {
                                        var temp = currentTab.dataDetails[currentTab.dataDetails.length - 1];
                                        currentTab.dataDetails[currentTab.dataDetails.length - 1] = currentTab.dataDetails[index];
                                        currentTab.dataDetails[index] = temp;
                                        $scope.selectedRow = 0;
                                        $scope.target.maVatTu = null;
                                    }
                                    addInitKhuyenMai(currentTab.dataDetails[index]);
                                    $scope.currentCount = currentTab.dataDetails[currentTab.dataDetails.length - 1].soLuong
                                    $scope.soLuongBan = $scope.currentCount;
                                }
                                $scope.soLuongBan = currentTab.dataDetails[currentTab.dataDetails.length - 1].soLuong;
                                //hàm tính tổng tiền
                                //reload lại tổng tiền, số lượng 
                                sumGiaoDich(currentTab.dataDetails);
                                listCopy = angular.copy(currentTab.dataDetails);
                                $scope.disableF4 = false;
                                $scope.disableF3 = false;
                            }
                        });
                    }, function () {
                    });
                }
                else {

                }
            };

            //chọn loại thanh toán
            $scope.enterTienMat = function (tienMat) {
                var currentTab = $scope.tabs[$scope.tabHienTai];
                if (tienMat && tienMat > 0) {
                    tienMat = parseInt(tienMat, 10);
                    currentTab.dataDto.tienMat = parseInt(tienMat, 10);
                    currentTab.dataDto.tienThua = (currentTab.dataDto.tienThe + currentTab.dataDto.tienCOD + currentTab.dataDto.tienMat) - currentTab.dataDto.sumTienHang;
                }
                else if (currentTab.dataDto.sumTienHang === tienMat) {
                    currentTab.dataDto.tienThe = 0;
                    currentTab.dataDto.tienCOD = 0;
                    currentTab.dataDto.tienThua = 0;
                }
                else if (currentTab.dataDto.tienThe >= $scope.currentSumTienHangTemp || currentTab.dataDto.tienCOD >= currentTab.dataDto.sumTienHang) {
                    currentTab.dataDto.tienMat = 0;
                    currentTab.dataDto.tienThua = 0;
                }
            };
            $scope.enterTienThe = function (tienThe) {
                var currentTab = $scope.tabs[$scope.tabHienTai];
                if (tienThe && tienThe > 0) {
                    tienThe = parseInt(tienThe, 10);
                    //nếu tiền thẻ và tiền cod
                    currentTab.dataDto.tienThe = tienThe;
                    if (currentTab.dataDto.tienCOD && currentTab.dataDto.tienCOD > 0) {
                        currentTab.dataDto.tienMat = currentTab.dataDto.sumTienHang - tienThe - currentTab.dataDto.tienCOD;
                        currentTab.dataDto.tienThua = currentTab.dataDto.sumTienHang - currentTab.dataDto.tienThe - currentTab.dataDto.tienCOD - currentTab.dataDto.tienMat;
                    }
                    else if (currentTab.dataDto.sumTienHang === tienThe) {
                        currentTab.dataDto.tienMat = 0;
                        currentTab.dataDto.tienCOD = 0;
                        currentTab.dataDto.tienThua = 0;
                    }
                    else if (tienThe > currentTab.dataDto.sumTienHang) {
                        currentTab.dataDto.tienMat = 0;
                        currentTab.dataDto.tienCOD = 0;
                        currentTab.dataDto.tienThua = tienThe - currentTab.dataDto.sumTienHang - currentTab.dataDto.tienVoucher;
                    }
                    else {
                        currentTab.dataDto.tienMat = currentTab.dataDto.sumTienHang - tienThe;
                    }
                }
                else {
                    if (currentTab.dataDto.tienCOD && currentTab.dataDto.tienCOD > 0) {
                        currentTab.dataDto.tienThe = 0;
                        currentTab.dataDto.tienMat = currentTab.dataDto.sumTienHang - currentTab.dataDto.tienCOD;
                        currentTab.dataDto.tienThua = currentTab.dataDto.sumTienHang - currentTab.dataDto.tienCOD;
                    }
                    else {
                        currentTab.dataDto.tienThe = 0;
                        currentTab.dataDto.tienMat = currentTab.dataDto.sumTienHang;
                    }
                }
            };
            $scope.enterTienCOD = function (tienCOD) {
                var currentTab = $scope.tabs[$scope.tabHienTai];
                if (tienCOD && tienCOD > 0) {
                    tienCOD = parseInt(tienCOD, 10);
                    currentTab.dataDto.tienCOD = tienCOD;
                    if (currentTab.dataDto.tienThe && currentTab.dataDto.tienThe > 0) {
                        currentTab.dataDto.tienMat = currentTab.dataDto.sumTienHang - currentTab.dataDto.tienThe - tienCOD;
                        currentTab.dataDto.tienThua = currentTab.dataDto.sumTienHang - currentTab.dataDto.tienThe - tienCOD - currentTab.dataDto.tienMat;
                    }
                    else if (currentTab.dataDto.sumTienHang === tienCOD) {
                        currentTab.dataDto.tienMat = 0;
                        currentTab.dataDto.tienThe = 0;
                        currentTab.dataDto.tienThua = 0;
                    }
                    else {
                        currentTab.dataDto.tienMat = currentTab.dataDto.sumTienHang - tienCOD;
                        currentTab.dataDto.tienThua = currentTab.dataDto.sumTienHang - currentTab.dataDto.tienThe - tienCOD - currentTab.dataDto.tienMat;
                    }
                }
                else {
                    if (currentTab.dataDto.tienThe && currentTab.dataDto.tienThe > 0) {
                        currentTab.dataDto.tienCOD = 0;
                        currentTab.dataDto.tienMat = currentTab.dataDto.sumTienHang - currentTab.dataDto.tienThe;
                    }
                    else {
                        currentTab.dataDto.tienCOD = 0;
                        currentTab.dataDto.tienMat = currentTab.dataDto.sumTienHang;
                    }
                }
            };

            //end chọn loại thanh toán

            //Nhập tỷ lệ chiết khấu
            $scope.enterTyLeChietKhau = function (ck, index) {
                var currentTab = $scope.tabs[$scope.tabHienTai];
                var number = 0;
                if (ck) {
                    //đánh tỷ lệ
                    if (ck === 0) {
                        ck = 1;
                    }
                    if (ck > 0 && ck <= 100) {
                        ck = parseInt(ck, 10);
                        if (currentTab.dataDetails[index].hangKhuyenMai) {
                            var tyLeGiam = listCopy[index].thanhTien - banLe ? (listCopy[index].giaBanLeVat * ck) / 100 : (listCopy[index].giaBanBuonVat * ck) / 100;
                            number = currentTab.dataDetails[index].soLuong * currentTab.dataDetails[index].thanhtienDuocKhuyenMai - tyLeGiam;
                            currentTab.dataDetails[index].thanhTien = parseInt(number, 10);
                        }
                        else {
                            var tyLeGiam = currentTab.dataDetails[index].soLuong * listCopy[index].giaBanLeVat - banLe ? (currentTab.dataDetails[index].soLuong * listCopy[index].giaBanLeVat * ck) / 100 : (currentTab.dataDetails[index].soLuong * listCopy[index].giaBanBuon * ck) / 100;
                            currentTab.dataDetails[index].tienChietKhau = parseInt(tyLeGiam, 10);
                            currentTab.dataDetails[index].thanhTien = parseInt(currentTab.dataDetails[index].soLuong * listCopy[index].giaBanLeVat - tyLeGiam, 10);
                            currentTab.dataDetails[index].tyLeChietKhau = parseInt(ck, 10);
                        }
                        //write log
                        $scope.logChietKhauTay = true;
                        console.log($scope.logChietKhauTay);
                    }
                }
                else {
                    currentTab.dataDetails[index].tyLeChietKhau = parseInt(0, 10);
                    currentTab.dataDetails[index].tienChietKhau = parseInt(0, 10);
                    number = currentTab.dataDetails[index].giaBanLeVat * currentTab.dataDetails[index].soLuong;
                    currentTab.dataDetails[index].thanhTien = parseInt(number, 10);
                    $scope.logChietKhauTay = false;
                }
                sumGiaoDich(currentTab.dataDetails);
                $scope.currentCount = currentTab.dataDetails[index].soLuong;
                $scope.target.soLuong = $scope.currentCount;
                $scope.soLuongBan = $scope.currentCount;
            };
            //end nhập tỷ lệ chiết khấu

            //Nhập tiền chiết khấu
            $scope.enterTienChietKhau = function (tienck, index) {
                var currentTab = $scope.tabs[$scope.tabHienTai];
                var number = 0;
                if (tienck) {
                    //đánh tỷ lệ
                    if (tienck === 0) {
                        tienck = 1;
                    }
                    if (tienck > 0) {
                        tienck = parseInt(tienck, 10);
                        if (currentTab.dataDetails[index].hangKhuyenMai) {
                            var tienGiam = listCopy[index].thanhTien - tienck;
                            number = listCopy[index].soLuong * listCopy[index].thanhtienDuocKhuyenMai - tienGiam;
                            currentTab.dataDetails[index].thanhTien = parseInt(number, 10);
                        }
                        else {
                            var tienGiam = listCopy[index].soLuong * listCopy[index].giaBanLeVat - tienck;
                            currentTab.dataDetails[index].tienChietKhau = parseInt(tienck, 10);
                            currentTab.dataDetails[index].thanhTien = parseInt(tienGiam, 10);
                            currentTab.dataDetails[index].tyLeChietKhau = 100 * (tienck / currentTab.dataDetails[index].giaBanLeVat);
                        }
                        //write log
                        $scope.logChietKhauTay = true;
                    }
                }
                else {
                    currentTab.dataDetails[index].tyLeChietKhau = parseInt(0, 10);
                    currentTab.dataDetails[index].tienChietKhau = parseInt(0, 10);
                    number = currentTab.dataDetails[index].giaBanLeVat * currentTab.dataDetails[index].soLuong;
                    currentTab.dataDetails[index].thanhTien = parseInt(number, 10);
                    $scope.logChietKhauTay = false;
                }
                sumGiaoDich(currentTab.dataDetails);
                $scope.currentCount = currentTab.dataDetails[index].soLuong;
                $scope.target.soLuong = $scope.currentCount;
                $scope.soLuongBan = $scope.currentCount;
            };
            //end nhập tiền chiết khấu
            //-------------------------------------------------------------------------------------------------------------------------------------------------
            //nhấn tìm hóa đơn 
            $scope.filterNotFound = function (strKey) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('nv/NvGiaoDichQuay', 'selectData'),
                    controller: 'nvGiaoDichQuaySelectDataController',
                    windowClass: 'app-modal-window',
                    resolve: {
                        serviceSelectData: function () {
                            return serviceGiaoDichQuay;
                        },
                        filterObject: function () {
                            return {
                                summary: strKey
                            };
                        }
                    }
                });
                modalInstance.result.then(function (updatedData) {
                    $scope.tabs[$scope.idTab].dataDto.soHoaDon = updatedData.maGiaoDich;
                    $scope.tabs[$scope.idTab].dataDto.maNhanVien = $scope.currentUser.maNhanVien;
                    $scope.tabs[$scope.idTab].dataDto.tenNhanVien = $scope.currentUser.tenNhanVien;
                    $scope.tabs[$scope.idTab].dataDto.userName = $scope.currentUser.userName;
                    $scope.tabs[$scope.idTab].dataDto.chucVu = $scope.currentUser.chucVu;
                    $scope.tabs[$scope.idTab].dataDto.gioiTinh = $scope.currentUser.gioiTinh;
                    $scope.tabs[$scope.idTab].dataDto.soDienThoai = $scope.currentUser.soDienThoai;
                    $scope.tabs[$scope.idTab].dataDto.unitCode = $scope.currentUser.unitCode;
                    $scope.tabs[$scope.idTab].dataDto.hinhThucThanhToan = updatedData.hinhThucThanhToan;
                    $scope.tabs[$scope.idTab].dataDto.maGiaoDich = updatedData.maGiaoDich;
                    $scope.tabs[$scope.idTab].dataDto.loaiGiaoDich = 2;
                    $scope.tabs[$scope.idTab].dataDto.ghiChu = updatedData.ghiChu;
                    $scope.tabs[$scope.idTab].dataDto.sumSoLuong = 0;
                    $scope.tabs[$scope.idTab].dataDto.sumTienHang = parseInt(updatedData.tTienCoVat, 10);
                    $scope.tabs[$scope.idTab].dataDto.tienVoucher = parseInt(updatedData.tienVoucher, 10);
                    $scope.tabs[$scope.idTab].dataDto.khachCanTra = parseInt(updatedData.khachCanTra, 10);
                    $scope.tabs[$scope.idTab].dataDto.tienKhachDua = parseInt(updatedData.tienKhachDua, 10);
                    $scope.tabs[$scope.idTab].dataDto.tienThua = parseInt(updatedData.tienThua === null ? 0 : updatedData.tienThua, 10);
                    $scope.tabs[$scope.idTab].dataDto.tienKhuyenMai = parseInt(updatedData.tienKhuyenMai === null ? 0 : updatedData.tienKhuyenMai, 10);
                    $scope.tabs[$scope.idTab].dataDto.tienDuocKhuyenMaiKhuyenMai = parseInt(updatedData.tienKhuyenMai === null ? 0 : updatedData.tienKhuyenMai, 10);
                    $scope.tabs[$scope.idTab].dataDto.makh = updatedData.maKhachHang;
                    $scope.tabs[$scope.idTab].dataDto.tenKH = updatedData.tenKhachHang;
                    $scope.tabs[$scope.idTab].dataDto.voucher = updatedData.voucher;
                    $scope.tabs[$scope.idTab].dataDto.thoiGian = updatedData.thoiGian;
                    $scope.tabs[$scope.idTab].dataDto.dienThoai = updatedData.dienThoai;
                    $scope.tabs[$scope.idTab].dataDto.ngayPhatSinh = updatedData.ngayPhatSinh;
                    $scope.tabs[$scope.idTab].dataDto.ngaySinh = updatedData.ngaySinh;
                    $scope.tabs[$scope.idTab].dataDto.email = updatedData.email;
                    $scope.tabs[$scope.idTab].dataDto.maThe = updatedData.maThe;
                    $scope.tabs[$scope.idTab].dataDto.diaChi = updatedData.diaChi;
                    $scope.tabs[$scope.idTab].dataDto.ngayHetHan = updatedData.ngayHetHan;
                    $scope.tabs[$scope.idTab].dataDto.ngayDacBiet = updatedData.ngayDacBiet;
                    $scope.tabs[$scope.idTab].dataDto.quenThe = updatedData.quenThe === null ? 0 : updatedData.quenThe;
                    $scope.tabs[$scope.idTab].dataDto.tienSale = updatedData.tienSale;
                    $scope.tabs[$scope.idTab].dataDto.tienNguyenGia = updatedData.tienNguyenGia;
                    $scope.tabs[$scope.idTab].dataDto.soDiem = updatedData.soDiem;
                    $scope.tabs[$scope.idTab].dataDto.tienThe = updatedData.tienThe === null ? 0 : updatedData.tienThe;
                    $scope.tabs[$scope.idTab].dataDto.tienCOD = updatedData.tienCOD === null ? 0 : updatedData.tienCOD;
                    $scope.tabs[$scope.idTab].dataDto.tienMat = updatedData.tienMat === null ? 0 : updatedData.tienMat;
                    $scope.tabs[$scope.idTab].dataDto.soDiem = updatedData.soDiem === null ? 0 : updatedData.soDiem;
                    if (updatedData.dataDetails.length > 0) {
                        angular.forEach(updatedData.dataDetails, function (value, index) {
                            value.tenVatTu = value.tenDayDu;
                            value.tienDuocKhuyenMai = value.tienKhuyenMai;
                            value.giaBanLeVat = value.giaBanLeCoVat;
                            value.tyLeChietKhau = 0;
                            value.tienChietKhau = value.tienKhuyenMai;
                            value.thanhTienTruocKm = value.giaBanLeVat * value.soLuong;
                            value.thanhTien = value.tTienCoVat;
                        });
                        $scope.tabs[$scope.idTab].dataDetails = updatedData.dataDetails;
                    }
                    $scope.trangThaiGiaoDich = 4;
                    $scope.disableF2 = true;
                }, function () {
                });
            };

            //
            $scope.enterCodeTrade = function (codeTrade) {
                if (codeTrade) {
                    var currentTab = $scope.tabs[$scope.idTab];
                    service.getAllDataTrade(codeTrade, function (response) {
                        if (response && response.status) {

                            $scope.tabs[$scope.idTab].dataDto.maNhanVien = $scope.currentUser.maNhanVien;
                            $scope.tabs[$scope.idTab].dataDto.tenNhanVien = $scope.currentUser.tenNhanVien;
                            $scope.tabs[$scope.idTab].dataDto.userName = $scope.currentUser.userName;
                            $scope.tabs[$scope.idTab].dataDto.chucVu = $scope.currentUser.chucVu;
                            $scope.tabs[$scope.idTab].dataDto.gioiTinh = $scope.currentUser.gioiTinh;
                            $scope.tabs[$scope.idTab].dataDto.soDienThoai = $scope.currentUser.soDienThoai;
                            $scope.tabs[$scope.idTab].dataDto.unitCode = $scope.currentUser.unitCode;
                            $scope.tabs[$scope.idTab].dataDto.hinhThucThanhToan = response.data.hinhThucThanhToan;
                            $scope.tabs[$scope.idTab].dataDto.maGiaoDich = response.data.maGiaoDich;
                            $scope.tabs[$scope.idTab].dataDto.loaiGiaoDich = 2;
                            $scope.tabs[$scope.idTab].dataDto.ghiChu = response.data.ghiChu;
                            $scope.tabs[$scope.idTab].dataDto.sumSoLuong = 0;
                            $scope.tabs[$scope.idTab].dataDto.sumTienHang = parseInt(response.data.tTienCoVat, 10);
                            $scope.tabs[$scope.idTab].dataDto.tienVoucher = parseInt(response.data.tienVoucher, 10);
                            $scope.tabs[$scope.idTab].dataDto.khachCanTra = parseInt(response.data.khachCanTra, 10);
                            $scope.tabs[$scope.idTab].dataDto.tienKhachDua = parseInt(response.data.tienKhachDua, 10);
                            $scope.tabs[$scope.idTab].dataDto.tienThua = parseInt(response.data.tienThua, 10);
                            $scope.tabs[$scope.idTab].dataDto.tienKhuyenMai = parseInt(response.data.tienKhuyenMai, 10);
                            $scope.tabs[$scope.idTab].dataDto.makh = response.data.makh;
                            $scope.tabs[$scope.idTab].dataDto.tenKH = response.data.tenKhachHang;
                            $scope.tabs[$scope.idTab].dataDto.voucher = response.data.voucher;
                            $scope.tabs[$scope.idTab].dataDto.thoiGian = response.data.thoiGian;
                            $scope.tabs[$scope.idTab].dataDto.dienThoai = response.data.dienThoai;
                            $scope.tabs[$scope.idTab].dataDto.ngayPhatSinh = response.data.ngayPhatSinh;
                            $scope.tabs[$scope.idTab].dataDto.ngaySinh = response.data.ngaySinh;
                            $scope.tabs[$scope.idTab].dataDto.email = response.data.email;
                            $scope.tabs[$scope.idTab].dataDto.maThe = response.data.maThe;
                            $scope.tabs[$scope.idTab].dataDto.diaChi = response.data.diaChi;
                            $scope.tabs[$scope.idTab].dataDto.ngayHetHan = response.data.ngayHetHan;
                            $scope.tabs[$scope.idTab].dataDto.ngayDacBiet = response.data.ngayDacBiet;
                            $scope.tabs[$scope.idTab].dataDto.quenThe = response.data.quenThe === null ? 0 : response.data.quenThe;
                            if (response.data.dataDetails.length > 0) {
                                $scope.tabs[$scope.idTab].dataDetails = response.data.dataDetails;
                            }

                        }
                        else if (!response.status) {
                            $scope.filterNotFound(codeTrade);
                        }
                    });
                }
                else {
                    console.log('clear all data !');
                }
            };
            //click bán hàng trả lại
            $scope.banTraLaiClick = function () {
                $scope.soLuong = 1;
                $scope.tabHienTai = 0;
                var count = ' 01';
                $scope.lstVatTuTraLai = [];
                $scope.loaiGiaoDich = 2;
                $scope.showClassTrungTam = false;
                $scope.showClassTraLai = true;
                var currentTab = $scope.tabs[$scope.tabHienTai];
                $scope.tabs.push({
                    title: 'Trả lại' + count,
                    dataDto: {
                        maGiaoDich: '',
                        loaiGiaoDich: $scope.loaiGiaoDich,
                        maNhanVien: $scope.currentUser.maNhanVien,
                        tenNhanVien: $scope.currentUser.tenNhanVien,
                        userName: $scope.currentUser.userName,
                        chucVu: $scope.currentUser.chucVu,
                        gioiTinh: $scope.currentUser.gioiTinh,
                        soDienThoai: $scope.currentUser.soDienThoai,
                        unitCode: $scope.currentUser.unitCode,
                    },
                    dataDetails: $scope.lstVatTuTraLai,
                    active: true
                });
                //tạo mã giao dịch trả lại mới
                service.buildCodeTrade().then(function (response) {
                    if (response && response.status === 200 && response.data) {
                        $scope.tabs[$scope.tabHienTai].dataDto.maGiaoDich = 'TL-' + response.data;
                    }
                });
                service.getPeriod(function (response) {
                    if (response) {
                        $scope.tabs[$scope.tabHienTai].dataDto.ngayChungTu = $filter('date')(response.toDate, "MM-dd-yyyy");
                        $scope.tabs[$scope.tabHienTai].dataDto.ngayPhatSinh = $filter('date')(response.toDate, "MM-dd-yyyy");;
                    }
                    else {
                        $scope.tabs[$scope.tabHienTai].dataDto.ngayChungTu = $filter('date')(new Date, "MM-dd-yyyy");
                        $scope.tabs[$scope.tabHienTai].dataDto.ngayPhatSinh = $filter('date')(response.toDate, "MM-dd-yyyy");;
                    }
                });
            }
            //end bán hàng trả lại

            //highlight dòng chọn
            $scope.getTab = function (index) {
                var currentTab = $scope.tabs[index];
                $scope.tabHienTai = index;
                $scope.target.maGiaoDich = $scope.tabs[index].maGiaoDich;
                $scope.loaiGiaoDich = $scope.tabs[index].loaiGiaoDich;
                if ($scope.tabs[index].dataDetails && $scope.tabs[index].dataDetails.length > 0) {
                    //tính lại tổng tiền
                    sumGiaoDich($scope.tabs[index].dataDetails);
                }
                else {
                    currentTab.dataDto.sumSoLuong = 0;
                    currentTab.dataDto.sumTienHang = 0;
                    currentTab.dataDto.sumTienKhuyenMai = 0;
                    currentTab.dataDto.tienKhachDua = 0;
                }
                $scope.idTab = index;
                if (currentTab.dataDto.loaiGiaoDich === 1) {
                    $scope.showClassTrungTam = true;
                    $scope.showClassTraLai = false;
                }
                else {
                    $scope.showClassTrungTam = false;
                    $scope.showClassTraLai = true;
                }
            }

            //thêm tab bán hàng
            $scope.addTab = function () {
                var number = $scope.tabs.length + 1;
                $scope.loaiGiaoDich = 1;
                var currentTab = $scope.tabs[$scope.idTab];
                var dataTab = {
                    title: 'Bán hàng ' + pad(number),
                    dataDto: {
                        maNhanVien: $scope.currentUser.maNhanVien,
                        tenNhanVien: $scope.currentUser.tenNhanVien,
                        userName: $scope.currentUser.userName,
                        chucVu: $scope.currentUser.chucVu,
                        gioiTinh: $scope.currentUser.gioiTinh,
                        soDienThoai: $scope.currentUser.soDienThoai,
                        unitCode: $scope.currentUser.unitCode,
                        ngayChungTu: '',
                        maGiaoDich: '',
                        loaiGiaoDich: 1,
                        ngayPhatSinh: '',
                        ghiChu: '',
                        sumSoLuong: 0,
                        sumTienHang: 0,
                        tienVoucher: 0,
                        khachCanTra: 0,
                        tienKhachDua: 0,
                        tienThua: 0,
                        tienKhuyenMai: 0,
                        makh: '',
                        tenKH: '',
                        theNhanTien: '',
                        voucher: '',
                        dienThoai: '',
                        phieuDatCoc: '',
                        ngaySinh: '',
                        tienDatCoc: 0,
                        email: '',
                        maThe: '',
                        diaChi: '',
                        ngayHetHan: '',
                        ngayDacBiet: '',
                        quenThe: 0
                    },
                    dataDetails: [],
                    active: true
                };
                service.getPeriod(function (response) {
                    if (response) {
                        $scope.tabs[$scope.tabHienTai].dataDto.ngayChungTu = $filter('date')(response.toDate, "MM-dd-yyyy");
                        $scope.tabs[$scope.tabHienTai].dataDto.ngayPhatSinh = $filter('date')(response.toDate, "MM-dd-yyyy");
                    } else {
                        $scope.tabs[$scope.tabHienTai].dataDto.ngayChungTu = $filter('date')(new Date, "MM-dd-yyyy");
                        $scope.tabs[$scope.tabHienTai].dataDto.ngayPhatSinh = $filter('date')(new Date, "MM-dd-yyyy");
                    }
                });
                $scope.tabs.push(dataTab);
            };
            //end thêm tab
            $scope.removeTab = function (index) {
                if ($scope.tabs.length > 1) {
                    $scope.tabs.splice(index, 1);
                } else {
                }
            };

            $scope.changeSoLuong = function (index, soluong, ev) {
                var currentTab = $scope.tabs[$scope.tabHienTai];
                currentTab.dataDetails[index].soLuong = soluong;
                currentTab.dataDetails[currentTab.dataDetails.length - 1].thanhTien = currentTab.dataDetails[index].donGia * currentTab.dataDetails[index].soLuong - currentTab.dataDetails[index].khuyenMai;
                currentTab.dataDetails[index].thanhTien = currentTab.dataDetails[index].donGia * currentTab.dataDetails[index].soLuong - currentTab.dataDetails[index].khuyenMai;
                //tính lại tổng tiền
                sumGiaoDich(currentTab.dataDetails);
                if (soluong === '0') {
                    currentTab.dataDetails.splice(index, 1);
                    //tính lại tổng tiền
                    sumGiaoDich(currentTab.dataDetails);
                    if (currentTab.dataDetails.length !== 0) {
                        $scope.currentCode = currentTab.dataDetails[currentTab.dataDetails.length - 1].maVatTu;
                    } else {
                        currentTab.dataDetails = [];
                        currentTab.dataDto.sumSoLuong = 0;
                        currentTab.dataDto.sumTienHang = 0;
                        currentTab.dataDto.tienKhuyenMai = 0;
                        currentTab.dataDto.tienKhachDua = 0;
                    }
                }
            };
            //end change số lượng
            service.getPeriod(function (response) {
                var currentTab = $scope.tabs[$scope.tabHienTai];
                if (response) {
                    currentTab.dataDto.ngayChungTu = $filter('date')(response.toDate, "MM-dd-yyyy");
                }
                else {
                    currentTab.dataDto.ngayChungTu = $filter('date')(new Date, "MM-dd-yyyy");
                }
            });
            $scope.clickThemMoi = function () {
                var currentTab = $scope.tabs[$scope.tabHienTai];
                $scope.disMaVatTu = false;
                service.buildCodeTrade().then(function (response) {
                    if (response && response.status === 200 && response.data) {
                        currentTab.dataDto.maGiaoDich = response.data;
                        currentTab.dataDto.loaiGiaoDich = 1;
                    }
                });
            };
            function F1(ev) {
                var currentTab = $scope.tabs[$scope.tabHienTai];
                $scope.disTienKhachTra = false;
                if (currentTab.dataDetails.length > 0) {
                    var allSumTienHang = 0;
                    var allSumTienKhuyenMai = 0;
                    angular.forEach(currentTab.dataDetails, function (value, index) {
                        allSumTienHang = allSumTienHang + (value.thanhTien + value.tienDuocKhuyenMai);
                        allSumTienKhuyenMai = allSumTienKhuyenMai + value.tienDuocKhuyenMai;
                        //kiểm tra 
                        if (value.isTichDiem === true) currentTab.dataDto.isTichDiem = true;
                    });
                }
                $scope.result = {
                    dataDetails: currentTab.dataDetails,
                    dataDto: currentTab.dataDto
                }
                if ($scope.thanhToanTienMat) $scope.result.dataDto.hinhThucThanhToan = 'TIENMAT';
                $scope.result.dataDto.allSumTienHang = allSumTienHang;
                $scope.result.dataDto.allSumTienKhuyenMai = allSumTienKhuyenMai;
                if (currentTab.dataDetails.length > 0 && $scope.disableF2) {
                    $('#TienKhachTraFocus').focus();
                    //bật popup thanh toán hóa đơn nếu mã giao dịch tồn tại
                    if ($scope.result.dataDto.tienThe >= 0 && $scope.result.dataDto.tienCOD >= 0 && $scope.result.dataDto.tienMat >= 0) {
                        var modalPay = $uibModal.open({
                            backdrop: 'static',
                            templateUrl: configService.buildUrl('nv/BanLe', 'pay'),
                            controller: 'payBillController',
                            size: 'sm',
                            resolve: {
                                targetData: function () {
                                    return $scope.result;
                                }
                            }
                        });
                        modalPay.result.then(function (updatedData) {
                            $('#MAVATTUSEARCHFOCUS').focus();
                            $scope.disableF1 = true;
                            $scope.disableF2 = false;
                            $scope.disableF3 = true;
                            $scope.disableF4 = true;
                            $scope.disableF5 = true;
                            $scope.disableF6 = true;
                            $scope.disableF7 = true;
                            $scope.trangThaiGiaoDich = 10;
                        }, function () {
                            $log.info('Modal dismissed at: ' + new Date());
                        });
                    }
                    else {
                        showNotification('Tiền thanh toán không hợp lệ !');
                    }
                    //nếu loaiGiaoDich=2 thì bật popup xác thực trả lại các hàng trong danh sách
                }
            }
            function F2(ev) {
                var currentTab = $scope.tabs[$scope.tabHienTai];
                if (currentTab.dataDto.maGiaoDich && currentTab.dataDto.maGiaoDich !== '') {
                    if ($scope.trangThaiGiaoDich === 0) { //bắt đầu thanh toán
                        $scope.disableF1 = false;
                        $scope.disableF5 = false;
                        $scope.disableF6 = false;
                        $scope.disableF2 = true;
                        $scope.showPlus = true;
                        $scope.lstVatTu = [];
                        $scope.lstVatTu_Top = [];
                        //nếu có mã giao dịch rồi thì không làm gì
                        service.buildCodeTrade().then(function (response) {
                            if (response && response.status === 200 && response.data) {
                                $scope.tabs[$scope.tabHienTai].dataDto.maGiaoDich = response.data;
                            }
                        });
                    }
                    else if ($scope.trangThaiGiaoDich === 10) //đã giao dịch xong -  chuyển giao dịch mới
                    {
                        $scope.tabs = [
                        {
                            title: 'Bán hàng 01',
                            dataDto: {
                                maNhanVien: $scope.currentUser.userName,
                                tenNhanVien: $scope.currentUser.fullName,
                                userName: $scope.currentUser.userName,
                                chucVu: $scope.currentUser.chucVu,
                                gioiTinh: $scope.currentUser.gioiTinh,
                                soDienThoai: $scope.currentUser.soDienThoai,
                                unitCode: $scope.currentUser.unitCode,
                                ngayChungTu: '',
                                maGiaoDich: '',
                                loaiGiaoDich: 1,
                                ngayPhatSinh: '',
                                ghiChu: '',
                                sumSoLuong: 0,
                                sumTienHang: 0,
                                tienVoucher: 0,
                                khachCanTra: 0,
                                tienKhachDua: 0,
                                tienThua: 0,
                                tienKhuyenMai: 0,
                                makh: '',
                                tenKH: '',
                                theNhanTien: '',
                                voucher: '',
                                dienThoai: '',
                                phieuDatCoc: '',
                                ngaySinh: '',
                                tienDatCoc: 0,
                                email: '',
                                maThe: '',
                                diaChi: '',
                                ngayHetHan: '',
                                ngayDacBiet: '',
                                quenThe: 0,
                                tienThe: 0,
                                tienCOD: 0,
                                tienMat: 0
                            },
                            dataDetails: []
                        }];
                        service.buildCodeTrade().then(function (response) {
                            if (response && response.status === 200 && response.data) {
                                $scope.tabs[$scope.tabHienTai].dataDto.maGiaoDich = response.data;
                            }
                        });
                        service.getPeriod().then(function (response) {
                            if (response && response.status === 200 && response.data && response.data.trangThai === 10) {
                                $scope.tabs[$scope.tabHienTai].dataDto.ngayChungTu = $filter('date')(response.data.toDate, "MM-dd-yyyy");
                                $scope.tabs[$scope.tabHienTai].dataDto.ngayPhatSinh = $filter('date')(response.data.toDate, "MM-dd-yyyy");;
                            }
                            else {
                                $scope.tabs[$scope.tabHienTai].dataDto.ngayChungTu = $filter('date')(new Date, "MM-dd-yyyy");
                                $scope.tabs[$scope.tabHienTai].dataDto.ngayPhatSinh = $filter('date')(new Date, "MM-dd-yyyy");;
                            }
                        });
                        $scope.disableF2 = true; //thêm giao dịch tiếp theo thì kích hoạt nút này để bỏ qua check nhấn F2
                        $('#MAVATTUSEARCHFOCUS').focus();
                    }
                }
                else {
                    $rootScope.listComboLeft = [];
                    $rootScope.listComboRight = [];
                    $rootScope.listHangTangHang = [];
                    $scope.disableF1 = false;
                    $scope.disableF5 = false;
                    $scope.disableF6 = false;
                    $scope.disableF2 = true;
                    $scope.showPlus = true;
                    $scope.lstVatTu = [];
                    $scope.lstVatTu_Top = [];
                    $scope.trangThaiGiaoDich = 1; //trạng thái đang giao dịch
                    //nếu có mã giao dịch rồi thì không làm gì
                    service.buildCodeTrade().then(function (response) {
                        if (response && response.status === 200 && response.data) {
                            $scope.tabs[$scope.tabHienTai].dataDto.maGiaoDich = response.data;
                        }
                    });
                    service.getKhuyenMaiCombo(function (response) {
                        if (response.status && response.data) {
                            if (response.data.listComboLeft.length > 0) {
                                angular.forEach(response.data.listComboLeft, function (v, k) {
                                    $rootScope.listComboLeft.push(v);
                                });
                            }
                            if (response.data.listComboRight.length > 0) {
                                angular.forEach(response.data.listComboRight, function (v, k) {
                                    $rootScope.listComboRight.push(v);
                                });
                            }
                        }
                        //console.log('lstLeft:',$rootScope.listComboLeft);
                        //console.log('lstRight:',$rootScope.listComboRight);
                    });
                    service.getKhuyenMaiHangTangHang(function (response) {
                        if (response.status && response.data) {
                            angular.forEach(response.data, function (v, k) {
                                $rootScope.listHangTangHang.push(v);
                            });
                        }
                        //console.log('$rootScope.listHangTangHang:',$rootScope.listHangTangHang);
                        //console.log('lstRight:',$rootScope.listComboRight);
                    });
                }
                $('#MAVATTUSEARCHFOCUS').focus();
            }
            function F3(key, ev) {
                if (key === 114 && $scope.disableF3 === false) {
                    if ($scope.status) {
                        $scope.status = false;
                    }
                    else if (!$scope.status) {
                        var currentTab = $scope.tabs[$scope.tabHienTai];
                        var index = 0;
                        if ($scope.currentCode !== "") {
                            index = currentTab.dataDetails.findIndex(x => x.maVatTu === $scope.currentCode);
                        }
                        if (currentTab.dataDetails[0].soLuong < 1) {
                            currentTab.dataDetails[0].splice(0, 1);
                            currentTab.dataDetails.splice(index, 1);
                        }
                        else {
                            if (currentTab.dataDetails[0].soLuong === 1) {
                                currentTab.dataDetails.splice(currentTab.dataDetails[0], 1);
                                if (currentTab.dataDetails.length != 0) {
                                    $scope.currentCode = currentTab.dataDetails[0].maVatTu;
                                }
                                else {
                                    currentTab.dataDetails[0] = [];
                                    currentTab.dataDto.sumSoLuong = 0;
                                    currentTab.dataDto.sumTienHang = 0;
                                    currentTab.dataDto.tienKhuyenMai = 0;
                                    currentTab.dataDto.tienKhachDua = 0;
                                }
                                $('#MAVATTUSEARCHFOCUS').focus();
                            }
                            else {
                                if (currentTab.dataDto.loaiGiaoDich === 2) {
                                    $scope.giamGia = 0;
                                }
                                currentTab.dataDto.voucher = '';
                                currentTab.dataDto.logVoucher = false;
                                currentTab.dataDto.tienVoucher = 0;
                                $scope.currentCount = $scope.currentCount - 1;
                                $scope.target.soLuong = $scope.currentCount;
                                $scope.soLuongBan = $scope.currentCount;
                                currentTab.dataDetails[0].soLuong = $scope.currentCount;
                                addInitKhuyenMai(currentTab.dataDetails[0]);
                            }
                        }
                    }
                }
            }
            function F4(key, ev) {
                if (key === 115 && $scope.disableF4 === false) {
                    if (!$scope.status) {
                        $scope.status = true;
                    }
                    else if ($scope.status) {
                        var currentTab = $scope.tabs[$scope.tabHienTai];
                        currentTab.dataDetails[0].soLuong = currentTab.dataDetails[0].soLuong + 1;
                        $scope.currentCount = currentTab.dataDetails[0].soLuong;
                        $scope.target.soLuong = $scope.currentCount;
                        $scope.soLuongBan = $scope.currentCount;
                        currentTab.dataDto.voucher = '';
                        currentTab.dataDto.logVoucher = false;
                        if (currentTab.dataDto.logVoucher) currentTab.dataDto.tienVoucher = 0;
                        addInitKhuyenMai(currentTab.dataDetails[0]);
                    }
                }
            }
            function F5(key, ev) {
                var currentTab = $scope.tabs[$scope.tabHienTai];
                if (key === 116 && currentTab.dataDetails.length === 0 && currentTab.dataDto.maGiaoDich !== '') {

                }
                else if (key === 116 && currentTab.dataDetails.length > 0) {
                    currentTab.dataDetails = [];
                    //tính lại tiền
                    currentTab.dataDto.sumSoLuong = 0;
                    currentTab.dataDto.sumTienHang = 0;
                    currentTab.dataDto.tienKhuyenMai = 0;
                    currentTab.dataDto.tienKhachDua = 0;
                    $scope.disableF1 = true;
                    $scope.disableF2 = true;
                    $scope.disableF3 = true;
                    $scope.disableF4 = true;
                    $scope.disableF5 = true;
                    $scope.disableF6 = false;
                    $scope.disablePlus = true;
                    $scope.disableMinus = true;
                    $('#MAVATTUSEARCHFOCUS').focus();
                }
                else {
                    $window.location.reload();
                }
            }
            function F6(key, ev) {
                var currentTab = $scope.tabs[$scope.tabHienTai];
                if (key === 117 && $scope.disableF2 === 1) {
                    var len = $scope.tabs.length + 1;
                    var numLbl = '' + ((len > 9) ? '' : '0') + String(len);
                    var lstvattu = [];
                    $scope.loaiGiaoDich = 1;
                    $scope.tabs[$scope.tabHienTai].active = false;
                    //tạo mã giao dịch mới
                    service.buildCodeTrade().then(function (response) {
                        if (response && response.status === 200 && response.data) {
                            currentTab.dataDto.maGiaoDich = response.data;
                        }
                    });
                    //
                    $scope.tabs.push({
                        title: 'Bán hàng ' + numLbl,
                        dataDto: {
                            maGiaoDich: currentTab.dataDto.maGiaoDich,
                            loaiGiaoDich: $scope.loaiGiaoDich
                        },
                        dataDetails: lstvattu,
                        active: true
                    });
                }
            }
            $scope.clickedF1 = function (ev) {
                F1(ev);
            }
            $scope.clickedF2 = function (ev) {
                F2(ev);
            }
            $scope.clickedF3 = function (ev) {
                F3(114, ev);
            }
            $scope.clickedF4 = function (ev) {
                F4(115, ev);
            }
            $scope.clickedF5 = function (ev) {
                F5(116, ev);
            }
            $scope.clickedF6 = function (ev) {
                F6(117, ev);
            }
            $scope.keys = {
                //thanh toán hóa đơn
                F1: function (name, code, ev) {
                    F1(ev);
                },
                //thêm mới giao dịch
                F2: function (name, code, ev) {
                    F2(ev);
                },
                //giảm
                F3: function (name, code, ev) {
                    F3(114, ev);
                },
                //tăng
                F4: function (name, code, ev) {
                    F4(115, ev);
                },
                //làm mới giao dịch
                F5: function (name, code, ev) {
                    F5(116, ev);
                },
                //thêm mới tab giao dịch
                F6: function (name, code, ev) {
                    F6(117, ev);
                }
            };

            $scope.enterDiscountPrice = function (disCount) {
                var currentTab = $scope.tabs[$scope.tabHienTai];
                if (currentTab.dataDto.sumTienHang >= 0 && $scope.target.sumTienHang >= disCount) {
                    currentTab.dataDto.discountPrice = disCount;
                    currentTab.dataDto.sumTienHang = total(currentTab.dataDetails) - disCount;
                }
                else {
                    currentTab.dataDto.discountPrice = 0;
                    currentTab.dataDto.sumTienHang = total(currentTab.dataDetails);
                }
            };

            //nếu nhập số lượng 
            $scope.enterRowSoLuong = function (enterSoLuong, index) {
                enterSoLuong = parseInt(enterSoLuong, 10);
                var currentTab = $scope.tabs[$scope.tabHienTai];
                if (enterSoLuong === 0) {
                    enterSoLuong = 1;
                }
                currentTab.dataDetails[index].soLuong = parseInt(enterSoLuong, 10);
                $scope.currentCount = currentTab.dataDetails[index].soLuong;
                currentTab.dataDto.voucher = '';
                currentTab.dataDto.logVoucher = false;
                $scope.target.soLuong = $scope.currentCount;
                $scope.soLuongBan = $scope.currentCount;
                addInitKhuyenMai(currentTab.dataDetails[index]);
            };

            //nếu nhập số lượng
            $scope.enterSoLuong = function (inputNumber) {
                if ($scope.allowInputNumber) {
                    inputNumber = parseInt(inputNumber, 10);
                    if (inputNumber === 0) {
                        inputNumber = 1;
                    }
                    var currentTab = $scope.tabs[$scope.tabHienTai];
                    currentTab.dataDetails[0].soLuong = parseInt(inputNumber, 10);
                    if (currentTab.dataDetails[0].tyLeChietKhau !== 0) {
                        currentTab.dataDetails[0].thanhTien = currentTab.dataDetails[0].thanhTien - (currentTab.dataDetails[0].soLuong * currentTab.dataDetails[0].giaBanLeVat * currentTab.dataDetails[0].tyLeChietKhau) / 100;
                    }
                    else if (currentTab.dataDetails[0].tienChietKhau !== 0) {
                        currentTab.dataDetails[0].thanhTien = currentTab.dataDetails[0].thanhTien - (currentTab.dataDetails[0].soLuong * currentTab.dataDetails[0].tienChietKhau)
                    }
                    $scope.currentCount = currentTab.dataDetails[0].soLuong;
                    currentTab.dataDto.voucher = '';
                    currentTab.dataDto.logVoucher = false;
                    $scope.target.soLuong = $scope.currentCount;
                    $scope.soLuongBan = $scope.currentCount;
                    addInitKhuyenMai(currentTab.dataDetails[0]);
                    $('#MAVATTUSEARCHFOCUS').focus();
                }
            };
            //end nhập số lượng
            $scope.filterMerchandise = function (code, ev) {
                var currentTab = $scope.tabs[$scope.tabHienTai];
                if (!$scope.disableF2) {
                    toaster.pop('error', "Thông báo:", "Nhấn F2 để thêm mới giao dịch!");
                }
                else {
                    $scope.trangThaiGiaoDich = 2; //trạng thái đang scan hàng
                    if (code) { //}&& (code.length === 7 || code.length === 9)) {
                        $scope.soLuong = 1;
                        var obj = {};
                        code = code.toUpperCase();
                        if (currentTab.dataDto.loaiGiaoDich === 1) {
                            if ($scope.status && $scope.disableF2) {
                                //trường hợp bán bó hàng
                                if (code.substring(0, 2) === 'BH') {
                                    $scope.soLuongMaBo = 1;
                                    service.getPackageMerchandise(code).then(function (response) {
                                        obj =
                                        {
                                            maVatTu: response.data.maBoHang,
                                            tenVatTu: response.data.tenBoHang,
                                            donGia: response.data.thanhTienBoHang,
                                            soLuong: $scope.soLuongMaBo,
                                            khuyenMai: response.data.listMaHang[0].tienHangKhuyenMai,
                                            thanhTien: $scope.soLuongMaBo * response.data.thanhTienBoHang,
                                            noiDungKhuyenMai: response.data.listMaHang[0].noiDungKhuyenMai
                                        };
                                        $scope.giamGia = response.data.listMaHang[0].tienHangKhuyenMai;
                                        if (currentTab.dataDetails.filter(x=>x.maVatTu === response.data.maBoHang).length == 0) {
                                            currentTab.dataDetails.push(obj);
                                        }
                                        else {
                                            //cộng dồn mã đã có trong danh sách
                                            var index = currentTab.dataDetails.findIndex(x => x.maVatTu === response.data.maBoHang);
                                            currentTab.dataDetails[index].soLuong = currentTab.dataDetails[index].soLuong + 1;
                                            currentTab.dataDetails[index].khuyenMai = currentTab.dataDetails[index].soLuong * $scope.giamGia;
                                            currentTab.dataDetails[index].thanhTien = currentTab.dataDetails[index].soLuong * currentTab.dataDetails[index].donGia;
                                            if (index != null) {
                                                var temp = currentTab.dataDetails[0];
                                                currentTab.dataDetails[0] = currentTab.dataDetails[index];
                                                currentTab.dataDetails[index] = temp;
                                            }
                                            $scope.currentCount = currentTab.dataDetails[0].soLuong;
                                            $scope.soLuongBan = $scope.currentCount;
                                        }
                                        $scope.soLuongBan = currentTab.dataDetails[0].soLuong;
                                        //hàm tính tổng tiền
                                        sumGiaoDich(currentTab.dataDetails);
                                    }
                                    , function (error) {
                                    });
                                }
                                    //hết trường hợp bán bó hàng
                                    //trường hợp bán mã hàng và mã cân
                                else {

                                    /*$scope.filtered.isAdvance = true;
                                    $scope.filtered.advanceData['maHang'] = code;
                                    var postdata = { paged: $scope.paged, filtered: $scope.filtered };*/
                                    $scope.dataSearch = { maVatTu: code };
                                    service.postDataMerchandise($scope.dataSearch).then(function (response) {
                                        if (response && response.status === 200 && response.data.status && response.data && response.data.data.trangThaiCon === 0) {
                                            $scope.data = response.data.data;
                                            obj =
                                            {
                                                maVatTu: $scope.data.maVatTu,
                                                tenVatTu: $scope.data.tenVatTu,
                                                soLuong: $scope.data.soLuong > 0 ? $scope.data.soLuong : $scope.soLuong,
                                                giaBanLeVat: (banLe == true ? $scope.data.giaBanLeVat : $scope.data.giaBanBuonVat),
                                                thanhTien: $scope.soLuong * (banLe == true ? $scope.data.giaBanLeVat : $scope.data.giaBanBuonVat),
                                                maLoaiVatTu: $scope.data.maLoaiVatTu,
                                                maNhomVatTu: $scope.data.maNhomVatTu,
                                                maDonVi: $scope.data.maDonVi,
                                                giaVon: $scope.data.giaVon,
                                                maColor: $scope.data.maColor,
                                                tyLeVatRa: $scope.data.tyLeVatRa,
                                                tyLeVatVao: $scope.data.tyLeVatVao,
                                                maVatRa: $scope.data.maVatRa,
                                                maVatVao: $scope.data.maVatVao,
                                                donViTinh: $scope.data.donViTinh,
                                                avatar: $scope.data.avatar,
                                                image: $scope.data.image,
                                                path_image: $scope.data.path_image,
                                                trangThaiCon: $scope.data.trangThaiCon,
                                                giaTriKhuyenMai: $scope.data.giaTriKhuyenMai,
                                                tyLeKhuyenMai: $scope.data.tyLeKhuyenMai,
                                                noiDungKhuyenMai: $scope.data.noiDungKhuyenMai,
                                                maChuongTrinhKhuyenMai: $scope.data.maChuongTrinhKhuyenMai,
                                                soLuongKhuyenMai: $scope.data.soLuong_KhuyenMai,
                                                maKhoKhuyenMai: $scope.data.maKhoKhuyenMai,
                                                loaiKhuyenMai: $scope.data.loaiKhuyenMai,
                                                tyLeChietKhau: 0,
                                                tienChietKhau: 0,
                                                chietKhau: 0,
                                                tienThe: 0,
                                                tienCOD: 0,
                                                tienDuocKhuyenMai: 0,
                                                giaTriKhuyenMai_ChietKhau: $scope.data.giaTriKhuyenMai_ChietKhau,
                                                giaTriKhuyenMai_DongGia: $scope.data.giaTriKhuyenMai_DongGia,
                                                giaTriKhuyenMai_TichDiem: $scope.data.giaTriKhuyenMai_TichDiem,
                                                giaTriKhuyenMai_TinhTien: $scope.data.giaTriKhuyenMai_TinhTien,
                                                giaTriKhuyenMai_Voucher: $scope.data.giaTriKhuyenMai_Voucher,
                                                giaTriKhuyenMai_Combo: $scope.data.giaTriKhuyenMai_Combo,
                                                maHang_Km_Buy1Get1: $scope.data.maHang_Km_Buy1Get1,
                                                tenHang_Km_Buy1Get1: $scope.data.tenHang_Km_Buy1Get1,
                                                soLuong_Km_Buy1Get1: $scope.data.soLuong_Km_Buy1Get1,
                                                soLuongKhuyenMai_Combo: $scope.data.soLuongKhuyenMai_Combo,
                                                tyLeKhuyenMai_ChietKhau: $scope.data.tyLeKhuyenMai_ChietKhau,
                                                tyLeKhuyenMai_DongGia: $scope.data.tyLeKhuyenMai_DongGia,
                                                tyLeKhuyenMai_TichDiem: $scope.data.tyLeKhuyenMai_TichDiem,
                                                tyLeKhuyenMai_TinhTien: $scope.data.tyLeKhuyenMai_TinhTien,
                                                tyLeKhuyenMai_Voucher: $scope.data.tyLeKhuyenMai_Voucher,
                                                tyLeBatDau_TinhTien: $scope.data.tyLeBatDau_TinhTien,
                                                tuGio: $scope.data.tuGio,
                                                denGio: $scope.data.denGio,
                                                tonCuoiKySl: $scope.data.tonCuoiKySl === null ? 0 : $scope.data.tonCuoiKySl,
                                                isBanAm: $scope.data.isBanAm,
                                                isTichDiem: $scope.data.isTichDiem
                                            };
                                            if (currentTab.dataDetails.filter(x=>x.maVatTu === $scope.data.maVatTu).length == 0) {
                                                //chức năng kiểm tra hàng scan vào có nằm bên list combo bên phải
                                                if ($rootScope.listComboRight && $rootScope.listComboRight.length > 0) {
                                                    var checkRightCombo = $rootScope.listComboRight.findIndex(x => x.maVatTuRight === obj.maVatTu);
                                                    if (checkRightCombo !== -1) obj.logRightCombo = true;
                                                    else obj.logRightCombo = false;
                                                }
                                                //$scope.khongBanAm == true -- không cho phép bán âm
                                                if (response.data.isBanAm) {
                                                    if ($scope.khongBanAm) {
                                                        alert('Hàng tồn âm ! Không thể bán');
                                                    } else {
                                                        currentTab.dataDetails.push(obj);
                                                        var index = currentTab.dataDetails.findIndex(x => x.maVatTu === $scope.data.maVatTu);
                                                        currentTab.dataDetails[index].isTrue = true;
                                                    }
                                                }
                                                else {
                                                    currentTab.dataDetails.push(obj);
                                                }
                                                updateList(currentTab.dataDetails);
                                                $scope.selectedRow = 0;
                                                $scope.target.maVatTu = null;
                                                $('#SOLUONGFOCUS').focus();
                                                $scope.target.soLuong = obj.soLuong;
                                                var index = currentTab.dataDetails.findIndex(x => x.maVatTu === obj.maVatTu);
                                                addInitKhuyenMai(currentTab.dataDetails[index]);
                                            }
                                            else {
                                                //cộng dồn mã đã có trong danh sách
                                                var index = currentTab.dataDetails.findIndex(x => x.maVatTu === $scope.data.maVatTu);
                                                if (currentTab.dataDetails.length > 0) {
                                                    var temp = {};
                                                    temp = currentTab.dataDetails[0];
                                                    currentTab.dataDetails[0] = currentTab.dataDetails[index];
                                                    currentTab.dataDetails[index] = temp;
                                                }
                                                currentTab.dataDetails[0].soLuong = currentTab.dataDetails[0].soLuong + 1;
                                                //currentTab.dataDetails[0].thanhTien = currentTab.dataDetails[0].soLuong * currentTab.dataDetails[0].giaBanLeVat;
                                                if (index != null) {
                                                    $scope.selectedRow = 0;
                                                    $scope.target.maVatTu = null;
                                                }
                                                $scope.currentCount = currentTab.dataDetails[0].soLuong
                                                $scope.soLuongBan = $scope.currentCount;
                                                addInitKhuyenMai(currentTab.dataDetails[0]);
                                            }
                                            currentTab.dataDto.checkForget = false;
                                            $scope.soLuongBan = currentTab.dataDetails[0].soLuong;
                                            //hàm tính tổng tiền
                                            //reload lại tổng tiền, số lượng 
                                            sumGiaoDich(currentTab.dataDetails);
                                            listCopy = angular.copy(currentTab.dataDetails);
                                        }
                                        else {
                                            $scope.filterNotFoundMerchandise(code);
                                        }
                                    }, function (error) {
                                        ngNotify.set("Không tìm thấy sản phẩm này !", { duration: 2000, type: 'error' });
                                        $scope.target.maVatTu = null;
                                        $('#MAVATTUSEARCHFOCUS').focus();
                                    });
                                }
                                //end tìm kiếm mã
                                $scope.disableF4 = false;
                                $scope.disableF3 = false;
                                $scope.disableF1 = false;
                                $scope.disableF2 = true;
                                $scope.disTienKhachTra = true;
                                $scope.disThanhToan = false;
                                $scope.target.maVatTu = null;
                                $('#MAVATTUSEARCHFOCUS').focus();
                            }
                            else if (!$scope.disableF2) {
                                $scope.message = 'Nhấn F2 để thêm mới giao dịch';
                                $scope.target.maVatTu = null;
                                $('#MAVATTUSEARCHFOCUS').focus();
                            }
                            else {
                                $scope.target.maVatTu = null;
                                $('#MAVATTUSEARCHFOCUS').focus();
                            }

                        }//end loaiGiaoDich = 1
                        else {
                            //NẾU LIST HÀNG HÓA TỒN TẠI
                            var currentTab = $scope.tabs[$scope.tabHienTai];
                            if (currentTab.dataDetails.length > 0) {
                                if (currentTab.dataDetails.filter(x=>x.maVatTu === code).length !== 0) //nếu có hàng thì bật lên list
                                {
                                    var lst = [];
                                    var index = currentTab.dataDetails.findIndex(x => x.maVatTu === code);
                                    currentTab.dataDetails[index].Selected = true;
                                    $scope.selectedRowListHangHoa = index;
                                    $scope.target.maVatTu = null;
                                    $('#MAVATTUSEARCHFOCUS').focus();
                                }
                                else { //không có thì bật message
                                    showNotification('Hàng không tồn tại trong giao dịch!');
                                }
                            }
                        }
                    }
                }//end if F2 disable
            };
            //tìm theo mã khách hàng
            $scope.filterCustomer = function (inputSearch) {
                var currentTab = $scope.tabs[$scope.tabHienTai];
                $scope.showResult = false;
                if (inputSearch !== '') {
                    service.filterCustomerData(inputSearch).then(function (response) {
                        if (response && response.status == 200 && response.data && response.data.status) {
                            $scope.resultCustomer = response.data.data;
                            $scope.showResult = true;
                            angular.extend($scope.paged, response.data);
                        }
                        else {
                            //continue
                        }
                    });
                }
                else {
                    $scope.showResult = false;
                    currentTab.dataDto.tenKH = '';
                    currentTab.dataDto.makh = '';
                    currentTab.dataDto.diaChi = '';
                    currentTab.dataDto.ngaySinh = null;
                    currentTab.dataDto.hangKhachHang = '';
                    currentTab.dataDto.tongTien = '';
                    currentTab.dataDto.tienSale = 0;
                    currentTab.dataDto.tienNguyenGia = 0;
                    currentTab.dataDto.quenThe = 0;
                }
            };
            $scope.addCustomer = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    size: 'lg',
                    templateUrl: configService.buildUrl('htdm/Customer', 'add'),
                    controller: 'customerCreateController',
                    resolve: {}
                });
                modalInstance.result.then(function (updatedData) {
                    var currentTab = $scope.tabs[$scope.tabHienTai];
                    currentTab.dataDto.makh = updatedData.makh;
                    currentTab.dataDto.tenKH = updatedData.tenKH;
                    currentTab.dataDto.dienThoai = updatedData.dienThoai;
                    currentTab.dataDto.email = updatedData.email;
                    currentTab.dataDto.diaChi = updatedData.diaChi;
                    currentTab.dataDto.ngaySinh = updatedData.ngaySinh;
                    currentTab.dataDto.ngayDacBiet = updatedData.ngayDacBiet;
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };
            $scope.changeCustomer = function (customer) {
                $scope.viewTraCuu = true;
                $scope.showResult = false;
                $scope.showPhoneResult = false;
                var currentTab = $scope.tabs[$scope.tabHienTai];
                currentTab.dataDto.checkForget = false;
                $scope.disableNguoiMua = true;
                $scope.disableDienThoai = true;
                $scope.disableNgaySinh = true;
                $scope.disableEmail = true;
                $scope.disableDiaChi = true;
                $scope.disableNgayDacBiet = true;
                $scope.disableGenKhachHang = true;
                currentTab.dataDto.makh = customer.makh;
                currentTab.dataDto.tenKH = customer.tenKH;
                currentTab.dataDto.dienThoai = customer.dienThoai;
                currentTab.dataDto.email = customer.email;
                currentTab.dataDto.diaChi = customer.diaChi;
                currentTab.dataDto.quenThe = customer.quenThe === null ? 0 : customer.quenThe;
                currentTab.dataDto.ngaySinh = customer.ngaySinh;
                currentTab.dataDto.hangKhachHang = customer.hangKhachHang;
                currentTab.dataDto.ngayDacBiet = customer.ngayDacBiet;
                currentTab.dataDto.ngayHetHan = customer.ngayHetHan;
                currentTab.dataDto.ngayCapThe = customer.ngayCapThe;
                currentTab.dataDto.tongTien = customer.tongTien;
                currentTab.dataDto.tienSale = customer.tienSale;
                currentTab.dataDto.tienNguyenGia = customer.tienNguyenGia;
                currentTab.dataDto.soDiem = customer.soDiem;
                currentTab.dataDto.maThe = customer.maThe;
            };
            //chi tiết
            $scope.showInfomationCustomer = function (maKhachHang) {
                if (maKhachHang) {
                    var modalInstance = $uibModal.open({
                        backdrop: 'static',
                        size: 'lg',
                        templateUrl: configService.buildUrl('htdm/Customer', 'details'),
                        controller: 'customerDetailsController',
                        resolve: {
                            targetData: function () {
                                return maKhachHang;
                            }
                        }
                    });
                    modalInstance.result.then(function (updatedData) {
                    }, function () {
                        $log.info('Modal dismissed at: ' + new Date());
                    });
                }
            };
            //danh sách giao dịch -- in lại bill hàng
            $scope.danhSachGiaoDich = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    windowClass: 'app-modal-window',
                    templateUrl: configService.buildUrl('nv/NvGiaoDichQuay', 'index'),
                    controller: 'nvGiaoDichQuaySelectDataController',
                    resolve: {
                        serviceSelectData: function () {
                            return serviceGiaoDichQuay;
                        },
                        filterObject: function () {
                            return {
                                summary: ''
                            };
                        }
                    }
                });
                modalInstance.result.then(function (updatedData) {
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };
            //$scope.enterVoucher = function (voucher) {
            //    //đã tồn tại các chương trình km khác thì không được km voucher
            //    $scope.logKhuyenMai = false;
            //    if ($scope.logKhuyenMaiChietKhau || $scope.logKhuyenMaiDongGia
            //        || $scope.logKhuyenMaiBuy1Get1 || $scope.logKhuyenMaiCombo
            //        || $scope.logKhuyenMaiSaleTinhTien || $scope.logKhuyenMaiNhanDoiTichDiem || $scope.logChietKhauTay) $scope.logKhuyenMai = true;
            //    var currentTab = $scope.tabs[$scope.tabHienTai];
            //    currentTab.dataDto.checkForget = false;
            //    if (!currentTab.dataDto.logVoucher) currentTab.dataDto.logVoucher = false;
            //    if (voucher !== '') {
            //        kmVoucherService.getDisCountVoucher(voucher).then(function (response) {
            //            console.log('response kmVoucher:', response);
            //            if (response.data && response.status) {
            //                if (!currentTab.dataDto.logVoucher && response.data.data.trangThai == 10 && response.data.data.trangThaiSuDung == 10
            //                && parseInt(currentTab.dataDto.sumTienHang, 10) >= response.data.data.tienBatDau
            //                && parseInt(currentTab.dataDto.sumTienHang, 10) <= response.data.data.tienKetThuc) {
            //                    if (response.data.data.giaTriKhuyenMai > 0 && !$scope.logKhuyenMai) //khuyến mã voucher theo tiền
            //                    {
            //                        currentTab.dataDto.logVoucher = true;
            //                        currentTab.dataDto.sumTienHang = parseInt(currentTab.dataDto.sumTienHang, 10) - parseInt(response.data.data.giaTriKhuyenMai, 10);
            //                        currentTab.dataDto.tienVoucher = parseInt(response.data.data.giaTriKhuyenMai, 10);
            //                        currentTab.dataDto.khachCanTra = currentTab.dataDto.sumTienHang;
            //                        currentTab.dataDto.tienThua = currentTab.dataDto.sumTienHang - currentTab.dataDto.khachCanTra;
            //                        currentTab.dataDto.tienKhachDua = currentTab.dataDto.khachCanTra;
            //                        showNotification('Khuyến mãi voucher: ' + response.data.data.giaTriKhuyenMai + ' ngàn đồng');
            //                        currentTab.dataDto.tienCOD = 0;
            //                        currentTab.dataDto.tienThe = 0;
            //                        currentTab.dataDto.tienMat = 0;
            //                    }
            //                    else if (response.data.data.tyLeKhuyenMai > 0 && !$scope.logKhuyenMai) { //khuyến mã voucher theo %
            //                        currentTab.dataDto.logVoucher = true;
            //                        currentTab.dataDto.tyLeVoucher = parseInt(response.data.data.tyLeKhuyenMai, 10);
            //                        currentTab.dataDto.tienVoucher = (currentTab.dataDto.tyLeVoucher * parseInt(currentTab.dataDto.sumTienHang, 10)) / 100;
            //                        currentTab.dataDto.sumTienHang = parseInt(currentTab.dataDto.sumTienHang, 10) - parseInt(currentTab.dataDto.tienVoucher, 10);
            //                        currentTab.dataDto.khachCanTra = currentTab.dataDto.sumTienHang;
            //                        currentTab.dataDto.tienThua = currentTab.dataDto.sumTienHang - currentTab.dataDto.khachCanTra;
            //                        currentTab.dataDto.tienKhachDua = currentTab.dataDto.khachCanTra;
            //                        showNotification('Khuyến mãi voucher: ' + response.data.data.tyLeKhuyenMai + '% <=>' + currentTab.dataDto.tienVoucher + ' ngàn đồng');
            //                        currentTab.dataDto.tienCOD = 0;
            //                        currentTab.dataDto.tienThe = 0;
            //                        currentTab.dataDto.tienMat = 0;

            //                    }
            //                    else {
            //                        showNotification('Đã tồn tại CTKM khác !');
            //                    }
            //                }
            //                else if (response.data.data.trangThaiSuDung === 30) {
            //                    showNotification(response.data.message);
            //                }
            //                else {
            //                    currentTab.dataDto.tienVoucher = 0;
            //                    showNotification('Chưa đủ điều kiện khuyến mãi !');
            //                }
            //            }
            //        });
            //    } else {
            //        sumGiaoDich(currentTab.dataDetails);
            //    }
            //};
            //$scope.voucherIsEmpty = function (item) {
            //    var currentTab = $scope.tabs[$scope.tabHienTai];
            //    if (item === '') {
            //        currentTab.dataDto.logVoucher = false;
            //        currentTab.dataDto.tienVoucher = 0;
            //        sumGiaoDich(currentTab.dataDetails);
            //    }
            //};
            ////nhập mã thẻ vip
            //$scope.enterTheVip = function (maTheVip) {
            //    var currentTab = $scope.tabs[$scope.tabHienTai];
            //    if (maTheVip) {
            //        var birthday = new Date(currentTab.dataDto.ngaySinh).getDate();
            //        var day = new Date();
            //        var firstDay = day.getDate() - 1;
            //        var nextDay = day.getDate() + 1;
            //        if (maTheVip === currentTab.dataDto.maThe && firstDay < birthday < nextDay) {
            //            console.log('km trong ngày sinh nhật the vip');
            //            //lấy phần trăm chiết khấu từ bảng hạng khách hàng
            //            banLeService.getHangKhachHang(currentTab.dataDto.hangKhachHang, function (response) {
            //                if (response.status && response.data && response.data.maHangKh) {
            //                    $scope.logChietKhauSinhNhat = true;
            //                    var tyLeGiamGiaSinhNhat = parseInt(response.data.tyLeGiamGia, 10);
            //                    currentTab.dataDto.tyLeGiamGiaSinhNhat = tyLeGiamGiaSinhNhat;
            //                    currentTab.dataDto.tyLeKhuyenMai = tyLeGiamGiaSinhNhat
            //                    currentTab.dataDto.tienKhuyenMai = (currentTab.dataDto.tyLeGiamGiaSinhNhat * parseInt(currentTab.dataDto.sumTienHang, 10)) / 100;
            //                    currentTab.dataDto.sumTienHang = parseInt(currentTab.dataDto.sumTienHang, 10) - parseInt(currentTab.dataDto.tienKhuyenMai, 10);
            //                    currentTab.dataDto.khachCanTra = currentTab.dataDto.sumTienHang;
            //                    currentTab.dataDto.tienThua = currentTab.dataDto.sumTienHang - currentTab.dataDto.khachCanTra;
            //                    currentTab.dataDto.tienKhachDua = currentTab.dataDto.khachCanTra;
            //                }
            //            });
            //        }
            //    }
            //};
            ////sử dụng chiết khấu tay
            //$scope.changeChietKhauTay = function (event) {
            //    var currentTab = $scope.tabs[$scope.tabHienTai];
            //    if (event.target.checked) {
            //        currentTab.dataDto.chietKhauTay = true;
            //    }
            //    else {
            //        currentTab.dataDto.chietKhauTay = false;
            //    }
            //};
            ////
            //$scope.tienThanhToan = function (tienTra) {
            //    var currentTab = $scope.tabs[$scope.tabHienTai];
            //    tienTra = parseInt(tienTra, 10);
            //    currentTab.dataDto.tienThua = tienTra - parseInt(currentTab.dataDto.khachCanTra, 10);
            //    currentTab.dataDto.tienThua = parseInt(currentTab.dataDto.tienThua, 10);
            //    $scope.tienKhachDua = tienTra;
            //    if (tienTra > currentTab.dataDto.khachCanTra) {
            //        $scope.fullInfo = 1;
            //    }
            //    else {
            //        $scope.fullInfo = 0;
            //    }
            //    if (tienTra / 10 >= currentTab.dataDto.khachCanTra) $scope.fullInfo = 0;
            //    $scope.modifield = 1;
            //};

            $scope.changeBanGiaVon = function(item) {
                if (item.choseGiaVon) {
                    item.thanhTien = item.giaVon * item.soLuong;
                    item.tienChietKhau = item.giaBanLeVat * item.soLuong - item.thanhTien;
                    item.tyLeChietKhau = item.tienChietKhau / (item.giaBanLeVat * item.soLuong);
                    sumGiaoDich($scope.tabs[$scope.tabHienTai].dataDetails);
                } else {
                    item.thanhTien = item.giaBanLeVat * item.soLuong;
                    item.tienChietKhau = 0;
                    item.tyLeChietKhau = 0;
                    sumGiaoDich($scope.tabs[$scope.tabHienTai].dataDetails);
                }
            }

            $scope.displayHepler = function (paraValue, moduleName) {
                var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                if (data && data.length === 1) {
                    return data[0].text;
                } else {
                    return paraValue;
                }
            }
            function loadSupplier() {
                if (!tempDataService.tempData('suppliers')) {
                    serviceSupplier.getAll_Supplier().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                            tempDataService.putTempData('suppliers', successRes.data.data);
                            $scope.suppliers = successRes.data.data;
                        } else {
                            console.log('successRes', successRes);
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.suppliers = tempDataService.tempData('suppliers');
                }
            }

            function loadMerchandiseType() {
                if (!tempDataService.tempData('merchandiseTypes')) {
                    serviceMerchandiseType.getAll_MerchandiseType().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                            tempDataService.putTempData('merchandiseTypes', successRes.data.data);
                            $scope.merchandiseTypes = successRes.data.data;
                        } else {
                            console.log('successRes', successRes);
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.merchandiseTypes = tempDataService.tempData('merchandiseTypes');
                }
            }

            function loadNhomVatTu() {
                if (!tempDataService.tempData('nhomVatTus')) {
                    serviceNhomVatTu.getAll_NhomVatTu().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                            tempDataService.putTempData('nhomVatTus', successRes.data.data);
                            $scope.nhomVatTus = successRes.data.data;
                        } else {
                            console.log('successRes', successRes);
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.nhomVatTus = tempDataService.tempData('nhomVatTus');
                }
            }

            function loadWareHouse() {
                if (!tempDataService.tempData('wareHouses')) {
                    serviceWareHouse.getAll_WareHouse().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                            tempDataService.putTempData('wareHouses', successRes.data.data);
                            $scope.wareHouses = successRes.data.data;
                        } else {
                            console.log('successRes', successRes);
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.wareHouses = tempDataService.tempData('wareHouses');
                }
            }

            function loadPackagings() {
                if (!tempDataService.tempData('packagings')) {
                    servicePackaging.getAll_Packaging().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                            tempDataService.putTempData('packagings', successRes.data.data);
                            $scope.packagings = successRes.data.data;
                        } else {
                            console.log('successRes', successRes);
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.packagings = tempDataService.tempData('packagings');
                }
            }

            function loadTax() {
                if (!tempDataService.tempData('taxs')) {
                    serviceTax.getAll_Tax().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                            tempDataService.putTempData('taxs', successRes.data.data);
                            $scope.taxs = successRes.data.data;
                        } else {
                            console.log('successRes', successRes);
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.taxs = tempDataService.tempData('taxs');
                }
            }

            function loadDonViTinh() {
                if (!tempDataService.tempData('donViTinhs')) {
                    serviceDonViTinh.getAll_DonViTinh().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                            tempDataService.putTempData('donViTinhs', successRes.data.data);
                            $scope.donViTinhs = successRes.data.data;
                        } else {
                            console.log('successRes', successRes);
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.donViTinhs = tempDataService.tempData('donViTinhs');
                }
            }

            function loadCustomer() {
                if (!tempDataService.tempData('customers')) {
                    serviceCustomer.getAll_Customer().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                            tempDataService.putTempData('customers', successRes.data.data);
                            $scope.customers = successRes.data.data;
                        } else {
                            console.log('successRes', successRes);
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.customers = tempDataService.tempData('customers');
                }
            }

            function loadAuthDonVi() {
                if (!tempDataService.tempData('sysDonVis')) {
                    serviceAuthDonVi.getAll_DonVi().then(function(successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.length > 0) {
                            tempDataService.putTempData('sysDonVis', successRes.data);
                            $scope.auDonVis = successRes.data;
                        } else {
                            console.log('successRes', successRes);
                        }
                    }, function(errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.auDonVis = tempDataService.tempData('sysDonVis');
                }
            }

            loadCustomer();
            loadSupplier();
            loadMerchandiseType();
            loadNhomVatTu();
            loadWareHouse();
            loadPackagings();
            loadTax();
            loadDonViTinh();
            loadAuthDonVi();
        }]);
    app.controller('payBillController', [
      '$scope', '$location', '$http', 'configService', 'banLeService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'securityService', '$rootScope', 'toaster', 'periodService', 'merchandiseService', 'customerService', 'merchandiseTypeService', 'nhomVatTuService', 'supplierService', 'wareHouseService', 'packagingService', 'taxService', 'donViTinhService', 'userService', 'giaoDichQuayService', 'keyCodes', 'accountService', 'targetData', '$uibModalInstance', 'AuDonViService',
      function ($scope, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, securityService, $rootScope, toaster, servicePeriod, serviceMerchandise, serviceCustomer, serviceMerchandiseType, serviceNhomVatTu, serviceSupplier, serviceWareHouse, servicePackaging, serviceTax, serviceDonViTinh, serviceAuthUser, serviceGiaoDichQuay, keyCodes, accountService, targetData, $uibModalInstance , auDonViService) {
          $scope.currentUser = serviceAuthUser.GetCurrentUser();
          $scope.keys = keyCodes;
          $scope.target = {};
          $scope.modifield = 0;
          $scope.dataDto = {
              dataDetails: []
          }
          $scope.linkStyle = configService.rootUrlWeb + "/BTS.SP.MART/styles/css/pay.css";
          $scope.tempData = tempDataService.tempData;
          //ExtendValue = địa chỉ cửa hàng
          //ReferenceDataId = số điện thoại cửa hàng
          $scope.formatLabel = function (paraValue, moduleName) {
              var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
              if (data && data.length === 1) {
                  return data[0].text;
              } else {
                  return paraValue;
              }
          }
          $scope.target = targetData;

          $scope.target.DateNow = new Date();

          if ($scope.target && $scope.target.dataDto && $scope.target.dataDetails.length > 0) {
              $scope.dataDto = $scope.target.dataDto;
              $scope.dataDto.tienKhachDua = parseInt($scope.dataDto.tienKhachDua, 10);
              $scope.dataDto.dataDetails = $scope.target.dataDetails;

              if ($scope.dataDto.makh === null || $scope.dataDto.makh === '') $scope.dataDto.makh = 'KHACHLE';
              angular.forEach($scope.dataDto.dataDetails, function (value, index) {
                  value.maVatTu = value.maVatTu;
                  value.tenDayDu = value.tenVatTu;
                  value.soLuong = value.soLuong;
                  value.tTienCoVat = value.thanhTien;
                  value.giaBanLeCoVat = value.giaBanLeVat;
                  value.vatBan = value.tyLeVatRa;
                  value.maChuongTrinhKm = value.maChuongTrinhKhuyenMai;
                  value.tienKhuyenMai = value.tienDuocKhuyenMai;
              });
          }
          auDonViService.getUnitByUnitCode($scope.target.dataDto.unitCode).then(function (response) {
              if (response) {
                  $scope.target.dataDto.unitPhone = response.data.soDienThoai;
                  $scope.target.dataDto.diaChi = response.data.diaChi;
              }
          });

          $scope.printDiv = function (divName) {
              var popupWin = '';
              var printContents = document.getElementById(divName).innerHTML;
              var originalContents = document.body.innerHTML;
              if (navigator.userAgent.toLowerCase().indexOf('chrome') > -1) {
                  popupWin = window.open('', '_blank', 'width=600,height=600,scrollbars=no,menubar=no,toolbar=no,location=no,status=no,titlebar=no');
                  popupWin.window.focus();
                  popupWin.document.write('<!DOCTYPE html><html><head>' +
                      '<link rel="stylesheet" type="text/css" />' +
                      '</head><body onload="window.print()"><div class="reward-body">' + printContents + '</div></html>');
                  popupWin.document.close();
                  popupWin.close();
                  popupWin.onbeforeunload = function (event) {
                      popupWin.close();
                      return '.\n';
                  };
                  popupWin.onabort = function (event) {
                      popupWin.document.close();
                      popupWin.close();
                  }
              } else {
                  popupWin = window.open('', '_blank', 'width=800px,height=600px');
                  popupWin.document.open();
                  popupWin.document.write('<html><head><link rel="stylesheet" type="text/css" href="style.css" /></head><body onload="window.print()">' + printContents + '</html>');
                  popupWin.document.close();
              }
              popupWin.document.close();
              return true;
          }
          //hàm thanh toán hóa đơn
          $scope.payBill = function () {
              service.post($scope.dataDto).then(function (response) {
                  if (response && response.status === 201 && response.data) {
                      ngNotify.set("Giao dịch thành công", { type: 'success' });
                      $uibModalInstance.close($scope.dataDto);
                  } else {
                      ngNotify.set("Xảy ra lỗi trong quá trình thanh toán", { duration: 3000, type: 'error' });
                  }
              });
              $uibModalInstance.close();
          }
          //end hàm thanh toán hóa đơn
          $scope.cancel = function () {
              $uibModalInstance.dismiss('cancel');
          }
      }]);
    return app;
});

