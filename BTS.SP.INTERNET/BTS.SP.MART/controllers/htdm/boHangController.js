/*  
* Người tạo : Nguyễn Tuấn Hoàng Anh
* View: BTS.SP.MART/views/htdm/BoHang
* Vm sevices: BTS.API.SERVICE -> MD ->MdBoHangVm.cs
* Sevices: BTS.API.SERVICE -> MD -> MdBoHangService.cs
* Entity: BTS.API.ENTITY -> Md - > MdBoHang.cs
* Menu: Danh mục-> Danh mục bó hàng
*/
define(['ui-bootstrap', '/BTS.SP.MART/controllers/auth/AuthController.js', '/BTS.SP.MART/controllers/htdm/merchandiseController.js'], function () {
    'use strict';
    var app = angular.module('boHangModule', ['ui.bootstrap', 'authModule', 'merchandiseModule']);
    app.factory('boHangService', ['$http', 'configService', function ($http, configService) {
        var serviceUrl = configService.rootUrlWebApi + '/Md/BoHang';
        var rootUrl = configService.apiServiceBaseUri;
        var selectedData = [];
        var calc = {
            sum: function (obj, name) {
                var total = 0;
                if (obj && obj.length > 0) {
                    angular.forEach(obj, function (v, k) {
                        var increase = v[name];
                        if (!increase) {
                            increase = 0;
                        }
                        total += increase;
                    });
                }
                return total;
            },
            sumVat: function (tyGia, target) {
                var tienVat = 0;
                if (tyGia) {
                    tienVat = (target.thanhTienTruocVatSauCK * tyGia) / 100;
                }
                return tienVat;
            },
            changeChietKhau: function (target) {
                if (!target.thanhTienTruocVat) {
                    target.thanhTienTruocVat = 0;
                }
                if (!target.chietKhau) {
                    target.chietKhau = 0;
                }
                target.tienChietKhau = (target.thanhTienTruocVat * target.chietKhau) / 100;
            },
            changeTienChietKhau: function (target) {
                target.chietKhau = 100 * (target.tienChietKhau / target.thanhTienTruocVat);
            },
            changeSoLuongBao: function (item) {
                if (!item.soLuongLe) {
                    item.soLuongLe = 0;
                }
                if (!item.maBaoBi) {
                    item.luongBao = 1;
                }
                if (!item.giamGia) {
                    item.giamGia = 0;
                }
                item.soLuong = item.soLuongBao * item.luongBao + item.soLuongLe;
                item.tienTruocGiamGia = item.soLuong * item.donGia;
                item.tienGiamGia = item.soLuong * item.giamGia;
                item.thanhTien = item.soLuong * item.donGia - item.tienGiamGia;
            },
            changeSoLuongLe: function (item) {
                if (!item.soLuong) {
                    item.soLuong = 0;
                }
                if (!item.donGia) {
                    item.donGia = 0;
                }
                if (!item.maBaoBi) {
                    item.luongBao = 1;
                }
                if (!item.soLuongBao) {
                    item.soLuongBao = 0;
                }
                if (!item.giamGia) {
                    item.giamGia = 0;
                }
                item.soLuong = item.soLuongBao * item.luongBao + item.soLuongLe;
                item.tienTruocGiamGia = item.soLuong * item.donGia;
                item.tienGiamGia = item.soLuong * item.giamGia;
                item.thanhTien = item.soLuong * item.donGia - item.tienGiamGia;
            },
            changeDonGia: function (item) {
                if (!item.soLuong) {
                    item.soLuong = 0;
                }
                if (!item.giamGia) {
                    item.giamGia = 0;
                }
                item.tienTruocGiamGia = item.soLuong * item.donGia;
                item.tienGiamGia = item.soLuong * item.giamGia;
                item.thanhTien = item.soLuong * item.donGia - item.tienGiamGia;
            },
            changeGiamGia: function (item) {
                if (!item.soLuong) {
                    item.soLuong = 0;
                }
                if (!item.donGia) {
                    item.donGia = 0;
                }
                item.tienGiamGia = item.soLuong * item.giamGia;
                item.thanhTien = item.soLuong * item.donGia - item.tienGiamGia;
            },
            changeTyLeSoLuong: function (item) {
                item.tongBanLe = item.soLuong * (item.donGia - (item.donGia * item.tyLeCKLe / 100));
            },
            changeTyLeCKLe: function (item) {
                item.tongBanLe = item.soLuong * (item.donGia - (item.donGia * item.tyLeCKLe / 100));
            },
            changeTyLeCKBuon: function (item) {
                item.tongBanBuon = item.soLuong * (item.giaBanBuon - (item.giaBanBuon * item.tyLeCKBuon / 100));
            },
            changeThanhTien: function (item) {
                item.tyLeCKLe = (100 * (item.donGia - (item.tongBanLe / item.soLuong))) / item.donGia;
            },
            changeTyLeCKLeAsync: function (item) {
                item.tongtienbanle = item.soluong * (item.giabanlecovat - (item.giabanlecovat * item.tylechietkhaule / 100));
            },
            changeTyLeSoLuongAsync: function (item) {
                item.tongtienbanle = item.soluong * (item.giabanlecovat - (item.giabanlecovat * item.tylechietkhaule / 100));

            },
            changeThanhTienAsync: function (item) {
                item.tylechietkhaule = (100 * (item.giabanlecovat - (item.tongtienbanle / item.soluong))) / item.giabanlecovat;
            }
        }
        var result = {
            robot: calc,
            post: function (data) {
                return $http.post(serviceUrl + '/Insert', data);
            },
            postQuery: function (data) {
                return $http.post(serviceUrl + '/PostQuery', data);
            },
            update: function (params) {
                return $http.put(serviceUrl + '/' + params.id, params);
            },
            getMerchandiseForNvByCode: function (code, wareHouseCode, unitCode) {
                return $http.get(rootUrl + '/api/Md/Merchandise/GetForNvByCode/' + code + '/' + wareHouseCode + '/' + unitCode);
            },
            getDetails: function (id) {
                return $http.get(serviceUrl + '/GetDetails/' + id);
            },
            deleteItem: function (params) {
                return $http.delete(serviceUrl + '/' + params.id, params);
            },
            getNewCode: function () {
                return $http.get(serviceUrl + '/GetNewCode');
            },
            writeDataToExcel: function (data) {
                return $http.post(serviceUrl + '/WriteDataToExcel', data);
            },
            postSelectData: function (jsonObject) {
                return $http.post(serviceUrl + '/PostSelectData', jsonObject);
            },
            getDetailFromSQL: function (data) {
                return $http.get(serviceUrl + '/GetDetailFromSQL/' + data);
            },
            postBoHangToSQL: function (data) {
                return $http.post(serviceUrl + '/PostBoHangToSQL', data);
            },
            getDataByCode: function (code) {
                return $http.get(serviceUrl + '/GetDataByCode/' + code);
            },
            getSelectData: function () {
                return selectedData;
            },
            setSelectData: function (array) {
                selectedData = array;
            }
        };
        return result;
    }]);
    /* controller list */
    app.controller('boHangController', ['$scope', '$location', '$http', 'configService', 'boHangService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'securityService', 'toaster', 'merchandiseService',
        function ($scope, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, securityService, toaster, serviceMerchandise) {
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.filtered = angular.copy(configService.filterDefault);
            $scope.tempData = tempDataService.tempData;
            $scope.isEditable = true;
            //load dữ liệu
            function filterData() {
                $scope.isLoading = true;
                if ($scope.accessList.view) {
                    var postdata = { paged: $scope.paged, filtered: $scope.filtered };
                    service.postQuery(postdata).then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data && successRes.data.status) {
                            $scope.isLoading = false;
                            $scope.data = successRes.data.data.data;
                            angular.extend($scope.paged, successRes.data.data);
                        }
                    }, function (errorRes) {
                        console.log(errorRes);
                    });
                }
            };
            //end

            //check quyền truy cập
            function loadAccessList() {
                securityService.getAccessList('bohang').then(function (successRes) {
                    if (successRes && successRes.status === 200) {
                        $scope.accessList = successRes.data;
                        if (!$scope.accessList.view) {
                            toaster.pop('error', "Lỗi:", "Không có quyền truy cập !");
                        } else {
                            filterData();
                        }
                    } else {
                        toaster.pop('error', "Lỗi:", "Không có quyền truy cập !");
                    }
                }, function (errorRes) {
                    console.log(errorRes);
                    toaster.pop('error', "Lỗi:", "Không có quyền truy cập !");
                    $scope.accessList = null;
                });
            }
            loadAccessList();
            //end

            $scope.setPage = function (pageNo) {
                $scope.paged.currentPage = pageNo;
                filterData();
            };
            $scope.sortType = 'maBoHang';
            $scope.sortReverse = false;
            $scope.doSearch = function () {
                $scope.paged.currentPage = 1;
                filterData();
            };
            $scope.pageChanged = function () {
                filterData();
            };
            $scope.goHome = function () {
                window.location.href = "#!/home";
            };
            $scope.refresh = function () {
                $scope.setPage($scope.paged.currentPage);
            };
            $scope.title = function () { return 'Danh sách bó hàng'; };

            /* Function add New Item */
            $scope.create = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/BoHang', 'add'),
                    controller: 'boHangCreateController',
                    size: 'lg',
                    resolve: {}
                });
                modalInstance.result.then(function (updatedData) {
                    $scope.refresh();
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };

            /* Function Edit Item */
            $scope.update = function (target) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/BoHang', 'update'),
                    controller: 'boHangEditController',
                    windowClass: 'app-modal-window',
                    resolve: {
                        targetData: function () {
                            return target;
                        }
                    }
                });
                modalInstance.result.then(function (updatedData) {
                    $scope.refresh();
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };

            /* Function Details Item */
            $scope.details = function (target) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/BoHang', 'details'),
                    controller: 'boHangDetailsController',
                    windowClass: 'app-modal-window',
                    resolve: {
                        targetData: function () {
                            return target;
                        }
                    }
                });
                modalInstance.result.then(function (updatedData) {
                    $scope.refresh();
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };

            $scope.printITemBoHang = function () { // in nhiều bó hàng
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/boHang', 'printItemBoHang'),
                    controller: 'boHangExportController',
                    windowClass: 'app-modal-window',
                    resolve: {}
                });
                modalInstance.result.then(function (updatedData) {
                    $scope.refresh();
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            }

            /* Function Print Item */
            $scope.printITem = function (target) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/BoHang', 'printItem'),
                    controller: 'boHangExportItemController',
                    windowClass: 'app-modal-window',
                    resolve: {
                        targetData: function () {
                            return target;
                        }
                    }
                });
                modalInstance.result.then(function (updatedData) {
                    $scope.refresh();
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };

            /* Function Delete Item */
            $scope.deleteItem = function (event, target) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/BoHang', 'delete'),
                    controller: 'boHangDeleteController',
                    resolve: {
                        targetData: function () {
                            return target;
                        }
                    }
                });
                modalInstance.result.then(function (updatedData) {
                    $scope.refresh();
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };
        }]);

    /* controller addNew */
    app.controller('boHangCreateController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'boHangService', 'userService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'merchandiseService', 'toaster',
        function ($scope, $uibModalInstance, $location, $http, configService, service, serviceAuthUser, tempDataService, $filter, $uibModal, $log, ngNotify, serviceMerchandise, toaster) {
            var currentUser = serviceAuthUser.GetCurrentUser();
            function getNum(val) {
                if (isNaN(val)) {
                    return 0;
                }
                return val;
            }
            var unitCode = currentUser.unitCode;
            $scope.isListItemNull = true;
            $scope.robot = angular.copy(service.robot);
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.target = { dataDetails: [] };
            $scope.data = [];
            $scope.newItem = {};
            $scope.tempData = tempDataService.tempData;
            $scope.isLoading = false;
            $scope.title = function () { return 'Thêm mới bó hàng'; };
            $scope.target.donViThucHien = currentUser.unitCode;
            $scope.target.nguoiThucHien = currentUser.userName;
            $scope.target.ngayCT = new Date();
            function filterData() {
                $scope.target.dataDetails = serviceMerchandise.getSelectData();
                service.getNewCode().then(function (resNewCode) {
                    if (resNewCode && resNewCode.status == 200 && resNewCode.data) {
                        $scope.target.maBoHang = resNewCode.data;
                        $scope.target.dataDetails = [];
                    }
                });
            };
            filterData();
            $scope.pageChanged = function () {
                var currentPage = $scope.paged.currentPage;
                var itemsPerPage = $scope.paged.itemsPerPage;
                $scope.paged.totalItems = $scope.target.dataDetails.length;
                $scope.data = [];
                if ($scope.target.dataDetails) {
                    for (var i = (currentPage - 1) * itemsPerPage; i < currentPage * itemsPerPage && i < $scope.target.dataDetails.length; i++) {
                        $scope.data.push($scope.target.dataDetails[i]);
                    }
                }
            }
            $scope.selectedMaHang = function (code) {
                if (code) {
                    service.getMerchandiseForNvByCode(code, null, unitCode).then(function (resgetMer) {
                        if (resgetMer && resgetMer.status === 200 && resgetMer.data && resgetMer.data.status) {
                            $scope.newItem = resgetMer.data.data;
                            $scope.newItem.donGia = resgetMer.data.data.giaBanLeVat;
                            $scope.newItem.validateCode = resgetMer.data.data.maHang;
                        }
                        else {
                            $scope.addNewItem(code);
                        }
                    });
                }
            }
            $scope.addRow = function () {
                if (!$scope.newItem.tenHang) {
                    focus('tenHang');
                    document.getElementById('tenHang').focus();
                }
                else if (!$scope.newItem.soLuong) {
                    ngNotify.set("Chưa nhập số lượng", { duration: 1500, type: 'error' });
                    focus('soLuong');
                    document.getElementById('soLuong').focus();
                }
                else if (!$scope.newItem.tenHang) {
                    ngNotify.set("Chưa nhập tên hàng", { duration: 1500, type: 'error' });
                    focus('tenHang');
                    document.getElementById('tenHang').focus();
                }
                else if (!$scope.newItem.donGia) {
                    ngNotify.set("Chưa nhập đơn giá", { duration: 1500, type: 'error' });
                    focus('donGia');
                    document.getElementById('donGia').focus();
                }
                else if (!$scope.newItem.tongBanLe) {
                    ngNotify.set("Chưa nhập thành tiền", { duration: 1500, type: 'error' });
                }
                else {
                    if ($scope.newItem.validateCode == $scope.newItem.maHang) {
                        var exsist = $scope.target.dataDetails.some(function (element, index, array) {
                            return $scope.newItem.maHang == element.maHang;
                        });
                        if (exsist) {
                            toaster.pop('success', "Thông báo:", "Mã hàng này bạn đã nhập rồi. Cộng gộp");
                            angular.forEach($scope.target.dataDetails, function (v, k) {
                                if (v.maHang == $scope.newItem.maHang) {
                                    $scope.target.dataDetails[k].soLuong = getNum($scope.newItem.soLuong) + getNum($scope.target.dataDetails[k].soLuong);
                                    $scope.target.dataDetails[k].soLuongBao = getNum($scope.newItem.soLuongBao) + getNum($scope.target.dataDetails[k].soLuongBao);
                                    $scope.target.dataDetails[k].soLuongLe = getNum($scope.newItem.soLuongLe) + getNum($scope.target.dataDetails[k].soLuongLe);
                                    $scope.target.dataDetails[k].thanhTien = getNum($scope.newItem.soLuong) * getNum($scope.target.dataDetails[k].donGia);
                                    service.robot.changeSoLuongLe($scope.target.dataDetails[k]);
                                }
                            });
                        } else {
                            $scope.target.dataDetails.push($scope.newItem);
                        }
                        $scope.isListItemNull = false;
                    }
                    else {
                        ngNotify.set("Mã hàng chưa đúng", { duration: 1500, type: 'error' });
                    }
                    $scope.pageChanged();
                    $scope.newItem = {};
                    focus('maHang');
                    document.getElementById('maHang').focus();
                }
            };

            $scope.addNewItem = function (strKey) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Merchandise', 'selectData'),
                    controller: 'merchandiseSelectDataController',
                    windowClass: 'app-modal-window',
                    resolve: {
                        serviceSelectData: function () {
                            return serviceMerchandise;
                        },
                        filterObject: function () {
                            return {
                                summary: strKey
                            };
                        }
                    }
                });
                modalInstance.result.then(function (updatedData) {
                    if (!updatedData.selected) {
                        $scope.newItem = updatedData;
                        $scope.newItem.donGia = updatedData.giaBanLeVat;
                        $scope.newItem.validateCode = updatedData.maBoHang;
                    }
                    $scope.pageChanged();
                }, function () {
                });
            }

            $scope.removeItem = function (index) {
                var currentPage = $scope.paged.currentPage;
                var itemsPerPage = $scope.paged.itemsPerPage;
                var currentPageIndex = (currentPage - 1) * itemsPerPage + index;
                $scope.target.dataDetails.splice(currentPageIndex, 1);
                if ($scope.target.dataDetails.length == 0) {
                    $scope.isListItemNull = true;
                }
                $scope.pageChanged();
            }
            $scope.setIndex = function (index) {
                $scope.selectedRow = index;
            }

            $scope.selectedmaBoHang = function (code) {
                if (code) {
                    service.getMerchandiseForNvByCode(code, null, unitCode).then(function (resgetMer) {
                        if (resgetMer && resgetMer.status === 200 && resgetMer.data && resgetMer.data.status) {
                            $scope.newItem = resgetMer.data.data;
                            $scope.newItem.donGia = resgetMer.data.data.giaBanLeVat;
                            $scope.newItem.validateCode = resgetMer.data.data.maBoHang;
                        }
                        else {
                            $scope.addNewItem(code);
                        }
                    });
                }
            }

            $scope.save = function () {
                service.post($scope.target).then(function (successRes) {
                    if (successRes && successRes.status === 201 && successRes.data) {
                        ngNotify.set("Thêm mới thành công", { type: 'success' });
                        $uibModalInstance.close($scope.target);
                    } else {
                        console.log('addNew successRes', successRes);
                        ngNotify.set(successRes.data.message, { duration: 3000, type: 'error' });
                    }
                },
                    function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
            };

            $scope.cancel = function () {
                $scope.isListItemNull = true;
                $uibModalInstance.close();
            };
        }]);
    /* controller Edit */
    app.controller('boHangEditController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'boHangService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify', 'merchandiseService', 'toaster', 'userService',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify, serviceMerchandise, toaster, serviceAuthUser) {
            function getNum(val) {
                if (isNaN(val)) {
                    return 0;
                }
                return val;
            }
            var currentUser = serviceAuthUser.GetCurrentUser();
            var unitCode = currentUser.unitCode;
            $scope.isListItemNull = true;
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.targetData = angular.copy(targetData);
            $scope.tempData = tempDataService.tempData;
            $scope.target = targetData;
            $scope.robot = angular.copy(service.robot);
            $scope.newItem = {};
            $scope.isLoading = false;
            $scope.title = function () { return 'Cập nhập bó hàng'; };
            $scope.pageChanged = function () {
                var currentPage = $scope.paged.currentPage;
                var itemsPerPage = $scope.paged.itemsPerPage;
                $scope.paged.totalItems = $scope.target.dataDetails.length;
                $scope.data = [];
                if ($scope.target.dataDetails) {
                    for (var i = (currentPage - 1) * itemsPerPage; i < currentPage * itemsPerPage && i < $scope.target.dataDetails.length; i++) {
                        $scope.data.push($scope.target.dataDetails[i]);
                    }
                }
            };
            function filterData() {
                $scope.isLoading = true;
                service.getDetails($scope.target.id).then(function (resgetDetails) {
                    if (resgetDetails && resgetDetails.status == 200 && resgetDetails.data) {
                        $scope.target = resgetDetails.data;
                        $scope.target.ngayCT = new Date($scope.target.ngayCT);
                        if ($scope.target.dataDetails.length > 0) {
                            $scope.isListItemNull = false;
                        }
                    }
                    $scope.isLoading = false;
                    $scope.pageChanged();
                });
            };
            filterData();

            $scope.setIndex = function (index) {
                $scope.selectedRow = index;
            }
            $scope.selectedMaHang = function (code) {
                if (code) {
                    service.getMerchandiseForNvByCode(code, null, unitCode).then(function (resgetMer) {
                        if (resgetMer && resgetMer.status === 200 && resgetMer.data && resgetMer.data.status) {
                            $scope.newItem = resgetMer.data.data;
                            $scope.newItem.donGia = resgetMer.data.data.giaBanLeVat;
                            $scope.newItem.validateCode = resgetMer.data.data.maHang;

                        }
                        else {
                            $scope.addNewItem(code);
                        }
                    });
                }
            }
            $scope.addRow = function () {
                if (!$scope.newItem.tenHang) {
                    focus('tenHang');
                    document.getElementById('tenHang').focus();
                }
                else if (!$scope.newItem.soLuong) {
                    ngNotify.set("Chưa nhập số lượng", { duration: 1500, type: 'error' });
                    focus('soLuong');
                    document.getElementById('soLuong').focus();
                }
                else if (!$scope.newItem.tenHang) {
                    ngNotify.set("Chưa nhập tên hàng", { duration: 1500, type: 'error' });
                    focus('tenHang');
                    document.getElementById('tenHang').focus();
                }
                else if (!$scope.newItem.donGia) {
                    ngNotify.set("Chưa nhập đơn giá", { duration: 1500, type: 'error' });
                    focus('donGia');
                    document.getElementById('donGia').focus();
                }
                else if (!$scope.newItem.tongBanLe) {
                    ngNotify.set("Chưa nhập thành tiền", { duration: 1500, type: 'error' });
                }
                else {
                    if ($scope.newItem.validateCode == $scope.newItem.maHang) {
                        var exsist = $scope.target.dataDetails.some(function (element, index, array) {
                            return $scope.newItem.maHang == element.maHang;
                        });
                        if (exsist) {
                            toaster.pop('success', "Thông báo:", "Mã hàng này bạn đã nhập rồi. Cộng gộp");
                            angular.forEach($scope.target.dataDetails, function (v, k) {
                                if (v.maHang == $scope.newItem.maHang) {
                                    $scope.target.dataDetails[k].soLuong = getNum($scope.newItem.soLuong) + getNum($scope.target.dataDetails[k].soLuong);
                                    $scope.target.dataDetails[k].soLuongBao = getNum($scope.newItem.soLuongBao) + getNum($scope.target.dataDetails[k].soLuongBao);
                                    $scope.target.dataDetails[k].soLuongLe = getNum($scope.newItem.soLuongLe) + getNum($scope.target.dataDetails[k].soLuongLe);
                                    $scope.target.dataDetails[k].thanhTien = getNum($scope.newItem.soLuong) * getNum($scope.target.dataDetails[k].donGia);
                                    service.robot.changeSoLuongLe($scope.target.dataDetails[k]);
                                }
                            });
                        } else {
                            $scope.target.dataDetails.push($scope.newItem);
                        }
                        $scope.isListItemNull = false;
                    }
                    else {
                        ngNotify.set("Mã hàng chưa đúng", { duration: 1500, type: 'error' });
                    }
                    $scope.pageChanged();
                    $scope.newItem = {};
                    focus('maHang');
                    document.getElementById('maHang').focus();
                }
            };
            $scope.removeRow = function (index) {
                var currentPage = $scope.paged.currentPage;
                var itemsPerPage = $scope.paged.itemsPerPage;
                var currentPageIndex = (currentPage - 1) * itemsPerPage + index;
                $scope.target.dataDetails.splice(currentPageIndex, 1);
                serviceMerchandise.getSelectData().splice(index, 1);
                if ($scope.target.dataDetails.length == 0) {
                    $scope.isListItemNull = true;
                }
                $scope.pageChanged();
            };
            $scope.selectedmaBoHang = function (code) {
                if (code) {
                    service.getMerchandiseForNvByCode(code, null, unitCode).then(function (resgetMer) {
                        if (resgetMer && resgetMer.status === 200 && resgetMer.data && resgetMer.data.status) {
                            $scope.newItem = resgetMer.data.data;
                            $scope.newItem.donGia = resgetMer.data.data.giaBanLeVat;
                            $scope.newItem.validateCode = resgetMer.data.data.maBoHang;
                        }
                        else {
                            $scope.addNewItem(code);
                        }
                    });
                }
            }
            $scope.addNewItem = function (strKey) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Merchandise', 'selectData'),
                    controller: 'merchandiseSelectDataController',
                    windowClass: 'app-modal-window',
                    resolve: {
                        serviceSelectData: function () {
                            return serviceMerchandise;
                        },
                        filterObject: function () {
                            return {
                                summary: strKey
                            };
                        }
                    }
                });
                modalInstance.result.then(function (updatedData) {
                    if (!updatedData.selected) {
                        $scope.newItem = updatedData;
                        $scope.newItem.donGia = updatedData.giaBanLeVat;
                        $scope.newItem.validateCode = updatedData.maBoHang;
                    }
                    $scope.pageChanged();
                }, function () {
                });
            }

            $scope.removeItem = function (index) {
                var currentPage = $scope.paged.currentPage;
                var itemsPerPage = $scope.paged.itemsPerPage;
                var currentPageIndex = (currentPage - 1) * itemsPerPage + index;
                $scope.target.dataDetails.splice(currentPageIndex, 1);
                $scope.pageChanged();
            }

            $scope.save = function () {
                service.update($scope.target).then(function (successRes) {
                    if (successRes && successRes.status === 200 && successRes.data) {
                        ngNotify.set("Cập nhật thành công", { type: 'success' });
                        $uibModalInstance.close($scope.target);
                    } else {
                        console.log('update successRes', successRes);
                        ngNotify.set(successRes.data.message, { duration: 3000, type: 'error' });
                    }
                },
                    function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
            };
            $scope.cancel = function () {
                $scope.isListItemNull = true;
                $uibModalInstance.close();
            };

        }]);


    /* controller Details */
    app.controller('boHangDetailsController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'boHangService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify) {
            $scope.config = angular.copy(configService);
            $scope.robot = angular.copy(service.robot);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.tempData = tempDataService.tempData;
            $scope.isLoading = false;
            $scope.title = function () { return 'Chi tiết bó hàng'; };
            $scope.pageChanged = function () {
                var currentPage = $scope.paged.currentPage;
                var itemsPerPage = $scope.paged.itemsPerPage;
                $scope.paged.totalItems = $scope.target.dataDetails.length;
                $scope.data = [];
                if ($scope.target.dataDetails) {
                    for (var i = (currentPage - 1) * itemsPerPage; i < currentPage * itemsPerPage && i < $scope.target.dataDetails.length; i++) {
                        $scope.data.push($scope.target.dataDetails[i]);
                    }
                }
            };
            function fillterData() {
                $scope.isLoading = true;
                service.getDetails(targetData.id).then(function (resgetDetails) {
                    if (resgetDetails && resgetDetails.status === 200 && resgetDetails.data) {
                        $scope.target = resgetDetails.data;
                        $scope.target.ngayCT = new Date($scope.target.ngayCT);
                    }
                    $scope.isLoading = false;
                    $scope.pageChanged();
                });
            }
            fillterData();
            $scope.cancel = function () {
                $uibModalInstance.close();
            };

        }]);
    /* controller delete */
    app.controller('boHangDeleteController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'boHangService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify) {
            $scope.config = angular.copy(configService);
            $scope.targetData = angular.copy(targetData);
            $scope.target = targetData;
            $scope.isLoading = false;
            $scope.title = function () { return 'Xoá thành phần'; };
            $scope.save = function () {
                service.deleteItem($scope.target).then(function (successRes) {
                    if (successRes && successRes.status === 200) {
                        ngNotify.set("Xóa thành công", { type: 'success' });
                        $uibModalInstance.close($scope.target);
                    } else {
                        console.log('deleteItem successRes ', successRes);
                        ngNotify.set(successRes.data.message, { duration: 3000, type: 'error' });
                    }
                },
                    function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
            };
            $scope.cancel = function () {
                $uibModalInstance.close();
            };
        }]);

    /* controller export Item bo hang */
    app.controller('boHangExportItemController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'boHangService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify', '$window',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify, $window) {
            $scope.robot = angular.copy(service.robot);
            $scope.config = angular.copy(configService);
            $scope.targetData = angular.copy(targetData);
            $scope.target = targetData;
            $scope.isLoading = false;
            $scope.title = function () { return 'Danh sách hàng nhập'; };
            $scope.lstMerchandise = [];
            $scope.dataBoHang = {};
            $scope.maDonVi = $scope.target.unitCode;
            service.getDetails($scope.target.id).then(function (response) {
                if (response && response.status === 200 && response.data) {
                    $scope.dataHangHoa = response.data;
                    $scope.donGia = $scope.robot.sum($scope.dataHangHoa.dataDetails, 'tongBanLe');
                    $scope.dataHangHoa.thanhTien = $scope.donGia;
                    $scope.dataHangHoa.soLuongIn = $scope.robot.sum($scope.dataHangHoa.dataDetails, 'soLuong');
                }
                $scope.isLoading = false;
            });
            $scope.exportToExcel = function () {
                var lstData = [];
                $scope.dataHangHoa.soLuong = $scope.dataHangHoa.soLuongIn;
                lstData.push($scope.dataHangHoa);
                service.writeDataToExcel(lstData).then(function (response) {
                    if (response && response.status === 200 && response.data) {
                        ngNotify.set("Thành công", { type: 'success' });
                        $scope.hrefTem = configService.apiServiceBaseUri + "/Upload/Barcode/" + response.data.message;
                        $window.location.href = $scope.hrefTem;
                    }
                    else {
                        ngNotify.set(response.data.message, { duration: 3000, type: 'error' });
                    }

                });
            }
            $scope.goIndex = function () {
                $state.go('mdBoHang');
            };
            $scope.cancel = function () {
                $uibModalInstance.close();
            };
        }]);

    /* controller export Item mat hang */
    app.controller('boHangExportController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'boHangService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', '$window', 'toaster',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, $window, toaster) {
            $scope.robot = angular.copy(service.robot);
            $scope.config = angular.copy(configService);
            $scope.target = { dataDetails: [] };
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.isLoading = false;
            $scope.title = function () { return 'In tem bó hàng'; };
            $scope.hrefTem = configService.apiServiceBaseUri + "/Upload/Barcode/TemPlateBoHang.xls";
            $scope.isListItemNull = true;
            $scope.pageChanged = function () {
                var currentPage = $scope.paged.currentPage;
                var itemsPerPage = $scope.paged.itemsPerPage;
                $scope.paged.totalItems = $scope.target.dataDetails.length;
                $scope.data = [];
                if ($scope.target.dataDetails) {
                    for (var i = (currentPage - 1) * itemsPerPage; i < currentPage * itemsPerPage && i < $scope.target.dataDetails.length; i++) {
                        $scope.data.push($scope.target.dataDetails[i]);
                    }
                }
            }
            $scope.addRow = function () {
                if (!$scope.newItem.soLuong || $scope.newItem.soLuong < 1) {
                    document.getElementById('soLuong').focus();
                    var exsist = $scope.target.dataDetails.some(function (element, index, array) {
                        return $scope.newItem.maBoHang.toUpperCase() == element.maBoHang;
                    });
                    if (exsist) {
                        toaster.pop("Thông báo:", "Bó hàng này bạn đã nhập rồi!");
                    }
                    return;
                }
                if ($scope.newItem.validateCode == $scope.newItem.maBoHang) {
                    var exsist = $scope.target.dataDetails.some(function (element, index, array) {
                        return $scope.newItem.maBoHang == element.maBoHang;
                    });
                    if (exsist) {
                        toaster.pop('success', "Thông báo:", "Bó hàng này bạn đã nhập rồi. Cộng gộp");
                        angular.forEach($scope.target.dataDetails, function (v, k) {
                            if (v.maBoHang == $scope.newItem.maBoHang) {
                                $scope.target.dataDetails[k].soLuong = $scope.newItem.soLuong + $scope.target.dataDetails[k].soLuong;
                            }
                        });
                    } else {
                        $scope.target.dataDetails.push($scope.newItem);
                    }
                    $scope.isListItemNull = false;
                } else {
                    toaster.pop('error', "Thông báo:", "Bó hàng chưa đúng");
                }
                $scope.pageChanged();
                $scope.newItem = {};
                document.getElementById('maBoHang').focus();
            };

            $scope.selectedMaBoHang = function (code) {
                if (code) {
                    service.getDataByCode(code).then(function (response) {
                        if (response && response.status == 200 && response.data) {
                            $scope.newItem = response.data;
                            $scope.newItem.validateCode = response.data.maBoHang;
                        } else {
                            $scope.addNewItem(code);
                        }
                    });
                }
            };
            $scope.removeItem = function (index) {
                var currentPage = $scope.paged.currentPage;
                var itemsPerPage = $scope.paged.itemsPerPage;
                var currentPageIndex = (currentPage - 1) * itemsPerPage + index;
                $scope.target.dataDetails.splice(currentPageIndex, 1);
                $scope.pageChanged();
            }
            $scope.setIndex = function (index) {
                $scope.selectedRow = index;
            }

            $scope.exportToExcel = function () {
                service.writeDataToExcel($scope.target.dataDetails).then(function (response) {
                    console.log($scope.target.dataDetails);
                    if (response.status) {
                        ngNotify.set("Thành công", { type: 'success' });
                        $window.location = $scope.hrefTem;
                    }
                    else {
                        ngNotify.set(response.Message, { duration: 3000, type: 'error' });
                    }

                });
            }
            $scope.cancel = function () {
                $scope.target.dataDetails.length = 0;
                $uibModalInstance.dismiss('cancel');
            };
        }]);
    /* bó hàng Select Data Controller */
    app.controller('boHangSelectDataController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'boHangService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'filterObject', 'serviceSelectData',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, filterObject, serviceSelectData) {
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.filtered = angular.copy(configService.filterDefault);
            $scope.filtered = angular.extend($scope.filtered, filterObject);
            angular.extend($scope.filtered, filterObject);
            $scope.modeClickOneByOne = true;
            $scope.listSelectedData = [];
            $scope.title = function () { return 'Danh sách bó hàng'; };
            function filterData() {
                $scope.listSelectedData = serviceSelectData.getSelectData();
                $scope.isLoading = true;
                var postdata = { paged: $scope.paged, filtered: $scope.filtered };
                service.postSelectData(postdata).then(function (response) {
                    $scope.isLoading = false;
                    if (response.status) {
                        $scope.data = response.data.data.data;
                        angular.forEach($scope.data, function (v, k) {
                            var isSelected = $scope.listSelectedData.some(function (element, index, array) {
                                if (!element) return false;
                                return element.value == v.value;
                            });
                            if (isSelected) {
                                $scope.data[k].selected = true;
                            }
                        });
                        angular.extend($scope.paged, response.data.data);
                    }
                });
            };
            filterData();
            $scope.setPage = function (pageNo) {
                $scope.paged.currentPage = pageNo;
                filterData();
            };

            $scope.isLoading = false;
            $scope.sortType = 'maThue'; // set the default sort type
            $scope.sortReverse = false;  // set the default sort order
            $scope.doSearch = function () {
                $scope.paged.currentPage = 1;
                filterData();
            };
            $scope.pageChanged = function () {
                filterData();
            };
            $scope.refresh = function () {
                $scope.setPage($scope.paged.currentPage);
            };
            $scope.doCheck = function (item) {
                if (item) {
                    var isSelected = $scope.listSelectedData.some(function (element, index, array) {
                        return element.id == item.id;
                    });
                    if (item.selected) {
                        if (!isSelected) {
                            $scope.listSelectedData.push(item);
                        }
                    } else {
                        if (isSelected) {
                            $scope.listSelectedData.splice(item, 1);
                        }
                    }
                } else {
                    angular.forEach($scope.data, function (v, k) {
                        $scope.data[k].selected = $scope.all;
                        var isSelected = $scope.listSelectedData.some(function (element, index, array) {
                            if (!element) return false;
                            return element.id == v.id;
                        });

                        if ($scope.all) {
                            if (!isSelected) {
                                $scope.listSelectedData.push($scope.data[k]);
                            }
                        } else {
                            if (isSelected) {
                                $scope.listSelectedData.splice($scope.data[k], 1);
                            }
                        }
                    });
                }
            };
            $scope.selecteItem = function (item) {
                $uibModalInstance.close(item);
            };
            $scope.save = function () {
                $uibModalInstance.close($scope.listSelectedData);
            };
            $scope.cancel = function () {
                $uibModalInstance.close();
            };
        }]);
    return app;
});

