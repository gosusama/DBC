/*  
* Người tạo : Nguyễn Tuấn Hoàng Anh
* View: BTS.SP.MART/views/nv/NvNgayHetHanHangHoa
* Vm sevices: BTS.API.SERVICE -> NV ->NvNgayHetHanHangHoaVm.cs
* Sevices: BTS.API.SERVICE -> NV -> NvNgayHetHanHangHoaService.cs
* Entity: BTS.API.ENTITY -> NV - > NvNgayHetHanHangHoa.cs
* Entity: BTS.API.ENTITY -> NV - > NvNgayHetHanHangHoaChiTiet.cs
* Menu: Nghiệp vụ-> Ngày hết hạn hàng hóa
*/
define(['ui-bootstrap', '/BTS.SP.MART/controllers/auth/AuthController.js', '/BTS.SP.MART/controllers/auth/AuNguoiDung.js', '/BTS.SP.MART/controllers/htdm/periodController.js', '/BTS.SP.MART/controllers/htdm/supplierController.js', '/BTS.SP.MART/controllers/htdm/merchandiseController.js'], function () {
    'use strict';
    var app = angular.module('phieuNgayHetHanHangHoaModule', ['ui.bootstrap', 'authModule', 'AuNguoiDungModule', 'periodModule', 'supplierModule', 'merchandiseModule']);
    app.factory('phieuNgayHetHanHangHoaService', ['$http', 'configService', function ($http, configService) {
        var serviceUrl = configService.rootUrlWebApi + '/NV/NgayHetHanHangHoa';
        var rootUrl = configService.apiServiceBaseUri;
        var result = {
            getReport: function (id) {
                return $http.get(serviceUrl + '/GetReport/' + id);
            },
            post: function (data) {
                return $http.post(serviceUrl + '/Post', data);
            },
            postQuery: function (data) {
                return $http.post(serviceUrl + '/PostQuery', data);
            },
            getNewInstance: function () {
                return $http.get(serviceUrl + '/GetNewInstance');
            },
            getNewParameter: function () {
                return $http.get(serviceUrl + '/GetNewParameter');
            },
            getDetails: function (id) {
                return $http.get(serviceUrl + '/GetDetails/' + id);
            },
            updateCT: function (params) {
                return $http.put(serviceUrl + '/' + params.id, params);
            },
            getSupplierForNvByCode: function (code) {
                return $http.get(rootUrl + '/api/Md/Supplier/GetForNvByCode/' + code);
            },
            getMerchandiseForNvByCode: function (code, wareHouseCode, unitCode) {
                return $http.get(rootUrl + '/api/Md/Merchandise/GetForNvByCode/' + code + '/' + wareHouseCode + '/' + unitCode);
            },
            deleteItem: function (params) {
                return $http.delete(serviceUrl + '/' + params.id, params);
            },
        };
        return result;
    }]);
    /* controller list */
    app.controller('phieuNgayHetHanHangHoaController', [
        '$scope', 'configService', 'phieuNgayHetHanHangHoaService', 'tempDataService', '$uibModal', '$log', 'securityService', 'toaster', 'periodService',
        function ($scope, configService, service, tempDataService, $uibModal, $log, securityService, toaster, servicePeriod) {
            //check có mở khóa sổ không
            function checkUnClosingOut() {
                servicePeriod.checkUnClosingOut(function (response) {
                    if (response.status && response.data.length > 0) {
                        $scope.listOpen = response.data;
                        for (var i = 0; i < $scope.listOpen.length; i++) {
                            $scope.date = new Date($scope.listOpen[i].fromDate);
                            angular.forEach($scope.data, function (value, idx) {
                                if (value.ngayCT.getTime() == $scope.date.getTime()) {
                                    console.log('mở khóa sổ');
                                    value.isOpen = true;
                                }
                            });
                        }
                    }
                });
            }
            //end check
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.filtered = angular.copy(configService.filterDefault);
            $scope.tempData = tempDataService.tempData;

            $scope.isEditable = true;
            $scope.accessList = {};
            //load dữ liệu
            function filterData() {
                $scope.isLoading = true;
                if ($scope.accessList.view) {
                    var postdata = {};
                    $scope.isLoading = true;
                    postdata = { paged: $scope.paged, filtered: $scope.filtered };
                    service.postQuery(postdata).then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data && successRes.data.status) {
                            $scope.isLoading = false;
                            $scope.data = successRes.data.data.data;
                            checkUnClosingOut();
                            angular.extend($scope.paged, successRes.data.data);
                        }
                    });
                }
            };
            //end

            //check quyền truy cập
            function loadAccessList() {
                securityService.getAccessList('phieuNgayHetHanHangHoa').then(function (successRes) {
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

            $scope.setPage = function (pageNo) {
                $scope.paged.currentPage = pageNo;
                filterData();
            };
            $scope.sortType = 'maNvNgayHetHanHangHoa';
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
                service.maLyDo = $scope.filtered.advanceData.maLyDo;
            };
            $scope.title = function () { return 'Phiếu ngày hết hạn hàng hóa'; };

            /* Function add New Item */
            $scope.create = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    size: 'md',
                    templateUrl: configService.buildUrl('nv/NvNgayHetHanHangHoa', 'add'),
                    controller: 'phieuNgayHetHanHangHoaCreateController',
                    windowClass: 'app-modal-window',
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
                    templateUrl: configService.buildUrl('nv/NvNgayHetHanHangHoa', 'update'),
                    controller: 'phieuNgayHetHanHangHoaEditController',
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
                    templateUrl: configService.buildUrl('nv/NvNgayHetHanHangHoa', 'details'),
                    controller: 'phieuNgayHetHanHangHoaDetailsController',
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
            $scope.deleteItem = function (target) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('nv/NvNgayHetHanHangHoa', 'delete'),
                    controller: 'phieuNgayHetHanHangHoaDeleteController',
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
    app.controller('phieuNgayHetHanHangHoaCreateController', ['$scope', '$uibModalInstance', 'configService', 'phieuNgayHetHanHangHoaService', 'tempDataService', '$uibModal', 'ngNotify', 'userService', 'toaster', 'merchandiseService', 'supplierService', '$filter',
        function ($scope, $uibModalInstance, configService, service, tempDataService, $uibModal, ngNotify, serviceAuthUser, toaster, serviceMerchandise, serviceSupplier, $filter) {
            var currentUser = serviceAuthUser.GetCurrentUser();
            $scope.fullName = currentUser.fullName;
            var unitCode = currentUser.unitCode;
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.target = { dataDetails: [] };
            $scope.data = [];
            $scope.newItem = {};
            $scope.isListItemNull = true;
            $scope.tempData = tempDataService.tempData;
            $scope.isLoading = false;
            $scope.title = function () { return 'Phiếu ngày hết hạn hàng hóa'; };
            //load danh muc
            $scope.changeNgay = function () {
                if ($scope.newItem.ngayHetHan) {
                    var timeDiff = (($scope.newItem.ngayHetHan).getTime() - ($scope.target.ngayBaoDate).getTime());
                    $scope.newItem.conLai_NgayBao = Math.ceil(timeDiff / (1000 * 3600 * 24));
                    var timeDiff2 = (($scope.newItem.ngayHetHan).getTime() - (new Date()).getTime());
                    $scope.newItem.conLai_NgayHetHan = Math.ceil(timeDiff2 / (1000 * 3600 * 24));
                    document.getElementById('_addNew').focus();
                }
            }
            //end 
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
            function filterData() {
                $scope.isLoading = true;
                service.getNewInstance().then(function (response) {
                    if (response && response.status == 200 && response.data) {
                        $scope.target = response.data;
                        $scope.target.ngayBaoDate = new Date();
                        $scope.pageChanged();
                        $scope.isLoading = false;
                    }
                });
            };
            filterData();
            $scope.addRow = function () {
                if (!$scope.newItem.maVatTu && !$scope.newItem.barcode) {
                    document.getElementById('mavattu').focus();
                    return;
                }
                if (!$scope.newItem.soLuong || $scope.newItem.soLuong < 1) {
                    document.getElementById('soluong').focus();
                    return;
                }
                if ($scope.newItem.validateCode == $scope.newItem.maVatTu) {
                    var exsist = $scope.target.dataDetails.some(function (element, index, array) {
                        return $scope.newItem.maVatTu == element.maVatTu && $scope.newItem.ngayHetHan == element.ngayHetHan;
                    });
                    if (exsist) {
                        angular.forEach($scope.target.dataDetails, function (v, k) {
                            if (v.maVatTu == $scope.newItem.maVatTu && v.ngaySanXuat == $scope.newItem.ngaySanXuat && v.ngayHetHan == $scope.newItem.ngayHetHan) {
                                toaster.pop('success', "Thông báo:", "Mã hàng này bạn đã nhập rồi. Cộng gộp");
                                $scope.target.dataDetails[k].soLuong = parseInt($scope.newItem.soLuong) + parseInt($scope.target.dataDetails[k].soLuong);
                            }
                        });
                    } else {
                        console.log('$scope.newItem', $scope.newItem);
                        $scope.target.dataDetails.push($scope.newItem);
                    }
                    $scope.isListItemNull = false;
                } else {
                    toaster.pop('error', "Lỗi:", "Mã hàng chưa đúng!");
                }
                $scope.pageChanged();
                $scope.newItem = {};
                document.getElementById('mavattu').focus();
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
                        if (!$scope.newItem.maNhaCungCap) {
                            $scope.newItem.barcode = updatedData.barcode;
                            $scope.newItem.maVatTu = updatedData.maVatTu;
                            $scope.newItem.tenVatTu = updatedData.tenVatTu;
                            $scope.newItem.maNhaCungCap = updatedData.maKhachHang;
                            $scope.newItem.validateCode = updatedData.maVatTu;
                            service.getSupplierForNvByCode($scope.newItem.maNhaCungCap).then(function (response) {
                                if (response && response.data) {
                                    $scope.newItem.maNhaCungCap = response.data.maNCC;
                                    $scope.newItem.tenNhaCungCap = response.data.tenNCC;
                                }
                                else {
                                    $scope.addNewItemNcc($scope.newItem.maNhaCungCap);
                                }
                            });
                            focus('soluong');
                            document.getElementById('soluong').focus();
                        }
                        else {
                            toaster.pop('error', "Thông báo:", "Mã hàng bạn nhập không thuộc về nhà cung cấp");
                        }
                    }
                    $scope.pageChanged();
                }, function () {

                });
            };
            $scope.addNewItemNcc = function (strKey) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Supplier', 'selectData'),
                    controller: 'supplierSelectDataController',
                    windowClass: 'app-modal-window',
                    resolve: {
                        serviceSelectData: function () {
                            return serviceSupplier;
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
                        $scope.newItem.maNhaCungCap = updatedData[0].value;
                        $scope.newItem.tenNhaCungCap = updatedData[0].description;
                        focus('mavattu');
                        document.getElementById('mavattu').focus();
                    }
                    $scope.pageChanged();
                }, function () {
                });
            };
            $scope.removeItem = function (index) {
                var currentPage = $scope.paged.currentPage;
                var itemsPerPage = $scope.paged.itemsPerPage;
                var currentPageIndex = (currentPage - 1) * itemsPerPage + index;
                $scope.target.dataDetails.splice(currentPageIndex, 1);
                if ($scope.target.dataDetails.length == 0) {
                    $scope.isListItemNull = true;
                }
                $scope.pageChanged();
            };

            $scope.selectedMaHang = function (code) {
                if (code) {
                    service.getMerchandiseForNvByCode(code, null, unitCode).then(function (response) {
                        if (response && response.status == 200 && response.data && response.data.status) {
                            if (!$scope.newItem.maNhaCungCap) {
                                $scope.newItem.maBaoBi = response.data.data.maBaoBi;
                                $scope.newItem.barcode = response.data.data.barcode;
                                $scope.newItem.maVatTu = response.data.data.maVatTu;
                                $scope.newItem.validateCode = response.data.data.maVatTu;
                                $scope.newItem.tenVatTu = response.data.data.tenVatTu;
                                $scope.newItem.maNhaCungCap = response.data.data.maKhachHang;
                                service.getSupplierForNvByCode($scope.newItem.maNhaCungCap).then(function (response) {
                                    if (response && response.data) {
                                        $scope.newItem.maNhaCungCap = response.data.maNCC;
                                        $scope.newItem.tenNhaCungCap = response.data.tenNCC;
                                    }
                                    else {
                                        $scope.addNewItemNcc($scope.newItem.maNhaCungCap);
                                    }
                                });
                            }
                            else if ($scope.newItem.maNhaCungCap != response.data.data.maKhachHang) {
                                toaster.pop('error', "Thông báo:", "Mã hàng bạn nhập không thuộc về nhà cung cấp");
                            }
                        }
                        else {
                            $scope.addNewItem(code);
                        }
                    });
                }
            }
            $scope.selectedMaNcc = function (code) {
                if (code) {
                    service.getSupplierForNvByCode(code).then(function (response) {
                        if (response && response.data) {
                            $scope.newItem.maNhaCungCap = response.data.maNCC;
                            $scope.newItem.tenNhaCungCap = response.data.tenNCC;
                        }
                        else {
                            $scope.addNewItemNcc(code);
                        }
                    });
                }
            }
            $scope.save = function () {
                angular.forEach($scope.target.dataDetails, function (v, k) {
                    $scope.target.dataDetails[k].ngaySanXuat = $filter('date')($scope.target.dataDetails[k].ngaySanXuat, 'yyyy-MM-dd');
                    $scope.target.dataDetails[k].ngayHetHan = $filter('date')($scope.target.dataDetails[k].ngayHetHan, 'yyyy-MM-dd');
                });
                service.post($scope.target).then(function (successRes) {
                    if (successRes && successRes.status === 201 && successRes.data) {
                        ngNotify.set("Thêm thành công", { type: 'success' });
                        $uibModalInstance.close($scope.target);
                    } else {
                        ngNotify.set(successRes.data.message, { duration: 3000, type: 'error' });
                    }
                },
                    function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
            };
            $scope.cancel = function () {
                $scope.isListItemNull = true;
                $uibModalInstance.dismiss('cancel');
            };
        }]);

    /* controller Edit */
    app.controller('phieuNgayHetHanHangHoaEditController', ['$scope', '$uibModalInstance', 'configService', 'phieuNgayHetHanHangHoaService', 'tempDataService', '$uibModal', 'targetData', 'ngNotify', 'merchandiseService', 'toaster', 'AuNguoiDungService', '$filter',
        function ($scope, $uibModalInstance, configService, service, tempDataService, $uibModal, targetData, ngNotify, serviceMerchandise, toaster, AuNguoiDungService, $filter) {
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.targetData = angular.copy(targetData);
            $scope.tempData = tempDataService.tempData;
            $scope.target = targetData;
            $scope.newItem = {};
            $scope.isLoading = true;
            $scope.title = function () { return 'Cập nhật phiếu ngày hết hạn hàng hóa'; };
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
            $scope.changeNgay = function () {
                if ($scope.newItem.ngayHetHan) {
                    var timeDiff = (($scope.newItem.ngayHetHan).getTime() - ($scope.target.ngayBaoDate).getTime());
                    $scope.newItem.conLai_NgayBao = Math.ceil(timeDiff / (1000 * 3600 * 24));
                    var timeDiff2 = (($scope.newItem.ngayHetHan).getTime() - (new Date()).getTime());
                    $scope.newItem.conLai_NgayHetHan = Math.ceil(timeDiff2 / (1000 * 3600 * 24));
                    document.getElementById('_addNew').focus();
                }
            }
            function filterData() {
                $scope.isLoading = true;
                service.getDetails($scope.target.id).then(function (resgetDetails) {
                    if (resgetDetails.status) {
                        $scope.target = resgetDetails.data.data;
                        $scope.target.ngayBaoDate = new Date(resgetDetails.data.data.ngayBaoDate);
                        angular.forEach($scope.target.dataDetails, function (v, k) {
                            if ($scope.target.dataDetails[k].ngayHetHan) {
                                var timeDiff2 = ((new Date($scope.target.dataDetails[k].ngayHetHan)).getTime() - (new Date()).getTime());
                                $scope.target.dataDetails[k].conLai_NgayHetHan = Math.ceil(timeDiff2 / (1000 * 3600 * 24));
                            }
                        });
                    }
                    $scope.isLoading = false;
                    $scope.pageChanged();
                });
                AuNguoiDungService.getUserByUsername($scope.target.iCreateBy).then(function (response) {
                    if (response && response.data.data) {
                        $scope.fullName = response.data.data.tenNhanVien;
                    }
                });
            }
            filterData();
            $scope.selectedMaHang = function (code) {
                if (code) {
                    service.getMerchandiseForNvByCode(code, null, unitCode).then(function (response) {
                        if (response && response.status == 200 && response.data && response.data.status) {
                            if (!$scope.newItem.maNhaCungCap) {
                                $scope.newItem.maBaoBi = response.data.data.maBaoBi;
                                $scope.newItem.barCode = response.data.data.barcode;
                                $scope.newItem.maVatTu = response.data.data.maVatTu;
                                $scope.newItem.validateCode = response.data.data.maVatTu;
                                $scope.newItem.tenVatTu = response.data.data.tenVatTu;
                                $scope.newItem.maNhaCungCap = response.data.data.maKhachHang;
                                service.getSupplierForNvByCode($scope.newItem.maNhaCungCap).then(function (response) {
                                    if (response && response.data) {
                                        $scope.newItem.maNhaCungCap = response.data.maNCC;
                                        $scope.newItem.tenNhaCungCap = response.data.tenNCC;
                                    }
                                    else {
                                        $scope.addNewItemNcc($scope.newItem.maNhaCungCap);
                                    }
                                });
                            }
                            else if ($scope.newItem.maNhaCungCap != response.data.data.maKhachHang) {
                                toaster.pop('error', "Thông báo:", "Mã hàng bạn nhập không thuộc về nhà cung cấp");
                            }
                        }
                        else {
                            $scope.addNewItem(code);
                        }
                    });
                }
            }
            $scope.selectedMaNcc = function (code) {
                if (code) {
                    service.getSupplierForNvByCode(code).then(function (response) {
                        if (response && response.data) {
                            $scope.newItem.maNhaCungCap = response.data.maNCC;
                            $scope.newItem.tenNhaCungCap = response.data.tenNCC;
                        }
                        else {
                            $scope.addNewItemNcc(code);
                        }
                    });
                }
            }

            $scope.addRow = function () {
                if (!$scope.newItem.maVatTu && !$scope.newItem.barcode) {
                    document.getElementById('mavattu').focus();
                    return;
                }
                if (!$scope.newItem.soLuong || $scope.newItem.soLuong < 1) {
                    document.getElementById('soluong').focus();
                    return;
                }
                if ($scope.newItem.validateCode == $scope.newItem.maVatTu) {
                    var exsist = $scope.target.dataDetails.some(function (element, index, array) {
                        return $scope.newItem.maVatTu == element.maVatTu && $scope.newItem.ngayHetHan == element.ngayHetHan;
                    });
                    if (exsist) {
                        angular.forEach($scope.target.dataDetails, function (v, k) {
                            if (v.maVatTu == $scope.newItem.maVatTu && v.ngaySanXuat == $scope.newItem.ngaySanXuat && v.ngayHetHan == $scope.newItem.ngayHetHan) {
                                toaster.pop('success', "Thông báo:", "Mã hàng này bạn đã nhập rồi. Cộng gộp");
                                $scope.target.dataDetails[k].soLuong = parseInt($scope.newItem.soLuong) + parseInt($scope.target.dataDetails[k].soLuong);
                            }
                        });
                    } else {
                        console.log('$scope.newItem', $scope.newItem);
                        $scope.target.dataDetails.push($scope.newItem);
                    }
                    $scope.isListItemNull = false;
                } else {
                    toaster.pop('error', "Lỗi:", "Mã hàng chưa đúng!");
                }
                $scope.pageChanged();
                $scope.newItem = {};
                document.getElementById('mavattu').focus();
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
                        if (!$scope.newItem.maNhaCungCap) {
                            $scope.newItem.barcode = updatedData.barcode;
                            $scope.newItem.maVatTu = updatedData.maVatTu;
                            $scope.newItem.tenVatTu = updatedData.tenVatTu;
                            $scope.newItem.maNhaCungCap = updatedData.maKhachHang;
                            $scope.newItem.validateCode = updatedData.maVatTu;
                            service.getSupplierForNvByCode($scope.newItem.maNhaCungCap).then(function (response) {
                                if (response && response.data) {
                                    $scope.newItem.maNhaCungCap = response.data.maNCC;
                                    $scope.newItem.tenNhaCungCap = response.data.tenNCC;
                                }
                                else {
                                    $scope.addNewItemNcc($scope.newItem.maNhaCungCap);
                                }
                            });
                            document.getElementById('soluong').focus();
                        }
                        else {
                            toaster.pop('error', "Thông báo:", "Mã hàng bạn nhập không thuộc về nhà cung cấp");
                        }
                    }
                    $scope.pageChanged();
                });
            };
            $scope.addNewItemNcc = function (strKey) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('htdm/Supplier', 'selectData'),
                    controller: 'supplierSelectDataController',
                    windowClass: 'app-modal-window',
                    resolve: {
                        serviceSelectData: function () {
                            return serviceSupplier;
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
                        $scope.newItem.maNhaCungCap = updatedData[0].value;
                        $scope.newItem.tenNhaCungCap = updatedData[0].description;
                        focus('mavattu');
                        document.getElementById('mavattu').focus();
                    }
                    $scope.pageChanged();
                }, function () {
                });
            };

            $scope.removeItem = function (index) {
                var currentPage = $scope.paged.currentPage;
                var itemsPerPage = $scope.paged.itemsPerPage;
                var currentPageIndex = (currentPage - 1) * itemsPerPage + index;
                $scope.target.dataDetails.splice(currentPageIndex, 1);
                $scope.pageChanged();
            };


            $scope.save = function () {
                angular.forEach($scope.target.dataDetails, function (v, k) {
                    $scope.target.dataDetails[k].ngaySanXuat = $filter('date')($scope.target.dataDetails[k].ngaySanXuat, 'yyyy-MM-dd');
                    $scope.target.dataDetails[k].ngayHetHan = $filter('date')($scope.target.dataDetails[k].ngayHetHan, 'yyyy-MM-dd');
                });
                service.updateCT($scope.target).then(function (successRes) {
                    if (successRes && successRes.status === 200 && successRes.data) {
                        ngNotify.set("Sửa thành công", { type: 'success' });
                        $uibModalInstance.close($scope.target);
                    } else {
                        ngNotify.set(successRes.data.message, { duration: 3000, type: 'error' });
                    }
                },
                    function (response) {
                        console.log('ERROR: Update failed! ' + response);
                    }
                );
            };

            $scope.cancel = function () {

                $uibModalInstance.close();
            };

        }]);

    /* controller Details */
    app.controller('phieuNgayHetHanHangHoaDetailsController', ['$scope', '$uibModalInstance', 'configService', 'phieuNgayHetHanHangHoaService', 'tempDataService', 'targetData', 'AuNguoiDungService',
        function ($scope, $uibModalInstance, configService, service, tempDataService, targetData, AuNguoiDungService) {
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.targetData = angular.copy(targetData);
            $scope.tempData = tempDataService.tempData;
            $scope.target = targetData;
            $scope.isLoading = false;
            $scope.title = function () { return 'Thông tin phiếu ngày hết hạn hàng hóa'; };
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
            //note
            function filterData() {
                $scope.isLoading = true;
                service.getDetails($scope.target.id).then(function (resgetDetails) {
                    if (resgetDetails.status) {
                        $scope.target = resgetDetails.data.data;
                        $scope.target.ngayBaoDate = new Date(resgetDetails.data.data.ngayBaoDate);
                        angular.forEach($scope.target.dataDetails, function (v, k) {
                            if ($scope.target.dataDetails[k].ngayHetHan) {
                                var timeDiff2 = ((new Date($scope.target.dataDetails[k].ngayHetHan)).getTime() - (new Date()).getTime());
                                $scope.target.dataDetails[k].conLai_NgayHetHan = Math.ceil(timeDiff2 / (1000 * 3600 * 24));
                            }
                        });
                    }
                    $scope.isLoading = false;
                    $scope.pageChanged();
                });
                AuNguoiDungService.getUserByUsername($scope.target.iCreateBy).then(function (response) {
                    if (response && response.data.data) {
                        $scope.fullName = response.data.data.tenNhanVien;
                    }
                });
            }
            filterData();
            $scope.cancel = function () {
                $uibModalInstance.close();
            };

        }]);

    /* controller delete */
    app.controller('phieuNgayHetHanHangHoaDeleteController', ['$scope', '$uibModalInstance', 'configService', 'phieuNgayHetHanHangHoaService', 'targetData', 'ngNotify',
        function ($scope, $uibModalInstance, configService, service, targetData, ngNotify) {
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

    app.controller('reportPhieuNgayHetHanHangHoaController', ['$scope', 'phieuNgayHetHanHangHoaService',, '$filter', 'userService', '$stateParams', '$window',
        function ($scope, service, $filter, serviceAuthUser, $stateParams, $window) {
            var currentUser = serviceAuthUser.GetCurrentUser();
            var id = $stateParams.id;
            $scope.target = {};
            $scope.goIndex = function () {
                $state.go('phieuNgayHetHanHangHoa');
            }
            function filterData() {
                if (id) {
                    service.getReport(id).then(function (response) {
                        if (response && response.status && response.data.data) {
                            $scope.target = response.data.data;
                            angular.forEach($scope.target.dataReportDetails, function (v, k) {
                                var timeDiff2 = ((new Date($scope.target.dataReportDetails[k].ngayHetHan)).getTime() - (new Date()).getTime());
                                $scope.target.dataReportDetails[k].conLai_NgayHetHan = Math.ceil(timeDiff2 / (1000 * 3600 * 24));
                            });
                        }
                    });
                    $scope.currentUser = currentUser.userName;
                }
            };
            filterData();
            $scope.displayHepler = function (paraValue, moduleName) {
                var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                if (data && data.length === 1) {
                    return data[0].text;
                } else {
                    return paraValue;
                }
            }
            $scope.checkDuyet = function () {
                if ($scope.target.trangThai == 10) {
                    return false;
                } else {
                    return true;
                }
            };
            $scope.print = function () {
                var table = document.getElementById('main-report').innerHTML;
                var myWindow = $window.open('', '', 'width=800, height=600');
                myWindow.document.write(table);
                myWindow.print();
            }
            $scope.printExcel = function () {
                var data = [document.getElementById('main-report').innerHTML];
                var fileName = "NgayHetHanHangHoa_ExportData.xls";
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
                    var url = window.URL.createObjectURL(new Blob(data, { type: filetype }));
                    a.attr("href", url);
                    a.attr("download", fileName);
                    $("body").append(a);
                    a[0].click();
                    a.remove();
                }
            }
        }]);

    return app;
});