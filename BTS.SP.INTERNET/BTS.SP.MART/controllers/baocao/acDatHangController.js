acModule.factory('acDatHangService', [
    '$resource', '$http', '$window', 'configService', 'clientService',
    function($resource, $http, $window, configService, clientService) {
        var rootUrl = configService.apiServiceBaseUri;
        var serviceUrl = configService.rootUrlWebApi + '/Ac/Closeout';
        var serviceDathangUrl = configService.rootUrlWebApi + '/Ac/DatHang';

        //var serviceUrl = rootUrl + '/_vti_bin/Apps/AC/CloseoutService.svc';
        var calc = {
            sum: function(obj, name) {
                var total = 0
                if (obj && obj.length > 0) {
                    angular.forEach(obj, function(v, k) {
                        var increase = v[name];
                        if (!increase) {
                            increase = 0;
                        }
                        total += increase;
                    });
                }
                return total;
            },
        }
        var result = {
            robot: calc,
            postReportDatHang: function(filter, callback) {
                $http.post(serviceDathangUrl + '/PostReportDatHang', filter).success(callback);
            },
            getWareHouseByUnit: function(maDonVi, callback) {
                $http.get(rootUrl + '/api/Md/WareHouse/GetByUnit/' + maDonVi).success(callback);
            },
            getPeriodByUnit: function(maDonVi, callback) {
                $http.get(rootUrl + '/api/Md/Period/GetByUnit/' + maDonVi).success(callback);
            },
            getNewParameter: function(callback) {
                $http.get(serviceDathangUrl + '/GetNewParameter').success(callback);
            },
            getUnitUsers: function(callback) {
                $http.get(rootUrl + '/api/Authorize/AuDonVi/GetSelectDataByUnitCode').success(callback);
            },
            postExportExcelInventoryChiTiet: function(json, filename) {
                $http({
                    url: serviceUrl + '/PostExportExcelInventoryChiTiet',
                    method: "POST",
                    data: json, //this is your json data string
                    headers: {
                        'Content-type': 'application/json'
                    },
                    responseType: 'arraybuffer'
                }).success(function(data, status, headers, config) {
                    var a = document.createElement("a");
                    document.body.appendChild(a);
                    a.style = "display: none";
                    var blob = new Blob([data], { type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
                    var objectUrl = URL.createObjectURL(blob);
                    a.href = objectUrl;
                    a.download = filename + ".xlsx";
                    a.click();
                    // window.URL.revokeObjectURL(objectUrl);
                }).error(function(data, status, headers, config) {
                    //upload failed
                });

                //$http.post(serviceUrl + '/WriteDataToExcel', data).success(callback);
            },
            getLastPeriod: function(callback) {
                $http.get(serviceUrl + '/GetLastPeriod').success(callback);
            },
            postApproval: function(data, callback) {
                return $http.post(configService.rootUrlWebApi + '/Md/Period/PostUpdateGiaoDich', data).success(callback);
            },
            postReportDatHangChiTiet: function (json, filename) {
                $http({
                    url: serviceDathangUrl + '/PostReportDatHangChiTiet',
                    method: "POST",
                    data: json, //this is your json data string
                    headers: {
                        'Content-type': 'application/json'
                    },
                    responseType: 'arraybuffer'
                }).success(function (data, status, headers, config) {
                    var a = document.createElement("a");
                    document.body.appendChild(a);
                    a.style = "display: none";
                    var blob = new Blob([data], { type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
                    var objectUrl = URL.createObjectURL(blob);
                    a.href = objectUrl;
                    a.download = filename + ".xlsx";
                    a.click();
                    // window.URL.revokeObjectURL(objectUrl);
                }).error(function (data, status, headers, config) {
                    //upload failed
                });

                //$http.post(serviceUrl + '/WriteDataToExcel', data).success(callback);
            },

        }
        return result;
    }
]);

acModule.controller('acDatHangController', ['$scope', '$rootScope', '$window', '$stateParams', '$timeout', '$sce', '$state', '$uibModal', '$log',
'mdService','authorizeService', 'acDatHangService', 'clientService', '$filter', 'serviceInventoryAndWareHouse', 'serviceInventoryAndMerchandiseType',
'serviceInventoryAndMerchandise', 'serviceInventoryAndMerchandiseGroup', 'serviceInventoryAndNhaCungCap','serviceDatHangAndNhanVien', 'configService', 'wareHouseService',
'merchandiseTypeService', 'merchandiseService', 'nhomVatTuService', 'customerService',
function ($scope, $rootScope, $window, $stateParams, $timeout, $sce, $state, $uibModal, $log,
mdService,authorizeService, acDatHangService, clientService, $filter, serviceInventoryAndWareHouse, serviceInventoryAndMerchandiseType,
 serviceInventoryAndMerchandise, serviceInventoryAndMerchandiseGroup, serviceInventoryAndNhaCungCap,serviceDatHangAndNhanVien, configService, wareHouseService,
 merchandiseTypeService, merchandiseService, nhomVatTuService, customerService) {
    $scope.filtered = angular.copy(configService.filterDefault);
    $scope.paged = angular.copy(configService.pageDefault);
    $scope.tempData = mdService.tempData;
    $scope.target = {};
    $scope.goIndex = function () {
        $state.go('baoCaoDatHang');
    }
    $scope.options = {
        minDate: null,
        maxDate: null
    };

    //Hàng hóa
    $scope.selectMerchandise = function () {
        var modalInstance = $uibModal.open({
            backdrop: 'static',
            templateUrl: mdService.buildUrl('MdMerchandise', 'selectDataSimple'),
            controller: 'merchandiseSimpleSelectDataController',
            resolve: {
                serviceSelectData: function () {
                    return serviceInventoryAndMerchandise;
                },
                filterObject: function () {
                    return {
                        isAdvance: true,
                        advanceData: { maLoaiVatTu: $scope.target.merchandiseTypeCodes, maNhomVatTu: $scope.target.merchandiseGroupCodes }
                    }
                }
            }
        });
        modalInstance.result.then(function (updatedData) {
            var output = '';
            angular.forEach(updatedData, function (item, index) {
                output += item.value + ',';
            });
            $scope.merchandiseCodes = output.substring(0, output.length - 1);
        }, function () {
            $log.info('Modal dismissed at: ' + new Date());
        });
    }

    //Khách hàng
    $scope.selectNhaCungCap = function () {
        var modalInstance = $uibModal.open({
            backdrop: 'static',
            templateUrl: mdService.buildUrl('mdCustomer', 'selectData'),
            controller: 'customerSelectDataController',
            resolve: {
                serviceSelectData: function () {
                    return serviceInventoryAndNhaCungCap;
                },
                filterObject: function () {
                    return {
                        advanceData: {

                        },
                    }
                }
            }
        });
        modalInstance.result.then(function (updatedData) {
            var output = '';
            angular.forEach(updatedData, function (item, index) {
                output += item.value + ',';
            });
            $scope.nhaCungCapCodes = output.substring(0, output.length - 1);
        }, function () {
            $log.info('Modal dismissed at: ' + new Date());
        });
    }
    //Nhân viên
    $scope.selectNhanVien= function () {
        var modalInstance = $uibModal.open({
            backdrop: 'static',
            templateUrl: authorizeService.buildUrl('Sys_User', 'selectData'),
            controller: 'sys_UserSelectDataController',
            resolve: {
                serviceSelectData: function () {
                    return serviceDatHangAndNhanVien;
                },

                filterObject: function () {
                    return {
                        advanceData: {
                        },
                    }
                }
            }
        });
        modalInstance.result.then(function (updatedData) {
            var output = '';
            angular.forEach(updatedData, function (item, index) {
                output += item.value + ',';
            });
            $scope.nhanVienCodes = output.substring(0, output.length - 1);
        }, function () {
            $log.info('Modal dismissed at: ' + new Date()); 1
        });
    }

    $scope.getWareHouseImportByUnit = function (code) {
        if (code) {
            acDatHangService.getPeriodByUnit(code, function (response) {
                $scope.periods = response;
            });
            acDatHangService.getWareHouseByUnit(code, function (response) {
                $scope.wareHouses = response;
            });
        };
    }
    $scope.groupBy = '';

    //filter by hang hoa
    $scope.changeMerchandise = function (inputMerchandise) {
        if (typeof inputMerchandise != 'undefined' && inputMerchandise !== '') {
            merchandiseService.filterMerchandiseCodes(inputMerchandise, function (response) {
                if (response) {
                    $scope.data = response;
                    $scope.merchandiseCodes = '';
                    $scope.merchandiseCodes = $scope.data.maVatTu;
                }
                else {
                    //$scope.selectMerchandise();
                }
            });
        }
    }

    //filter by nha cung cap
    $scope.changeNhaCungCap = function (inputNhaCungCap) {
        if (typeof inputNhaCungCap != 'undefined' && inputNhaCungCap !== '') {
            customerService.filterNhaCungCap(inputNhaCungCap, function (response) {
                if (response) {
                    $scope.data = response;
                    $scope.nhaCungCapCodes = '';
                    $scope.nhaCungCapCodes = $scope.data.makh;
                }
                else {
                    //$scope.selectNhaCungCap();
                }
            });
        }
    }
    //filter by nhân viên
    $scope.changeNhaCungCap = function (inputNhanVien) {
        if (typeof inputNhanVien != 'undefined' && inputNhanVien !== '') {
            customerService.filterNhanVien(inputNhanVien, function (response) {
                if (response) {
                    $scope.data = response;
                    $scope.nhanVienCodes = '';
                    $scope.nhanVienCodes = $scope.data.makh;
                }
                else {
                    //$scope.selectNhaCungCap();
                }
            });
        }
    }
    // end filter


    function filterData() {
        acDatHangService.getUnitUsers(function (response) {
            $scope.unitUsers = response;
        });
        acDatHangService.getNewParameter(function (response) {
            $scope.target = response;
            $scope.groupBy = $scope.target.groupBy;
            $scope.options.minDate = response.minDate;
            $scope.options.maxDate = response.maxDate;
            $scope.target.typeValue = "ALL";
        });
        acDatHangService.getLastPeriod(function (response) {
            $scope.lastPeriod = response;
        });
    }
    if ($state.params.obj != null) {
        var param = $state.params.obj;
        $scope.target = angular.copy(param);
        $scope.wareHouseCodes = param.wareHouseCodes;
        $scope.merchandiseTypeCodes = param.merchandiseTypeCodes;
        $scope.merchandiseGroupCodes = param.merchandiseGroupCodes;
        $scope.merchandiseCodes = param.merchandiseCodes;
        $scope.nhaCungCapCodes = param.nhaCungCapCodes;
        $scope.nhanVienCodes = param.nhanVienCodes;

        $scope.target.groupBy = $rootScope.groupBy;
        $scope.groupBy = $rootScope.groupBy;
    }
    filterData();
    if ($scope.groupBy != '') $scope.target.groupBy = $scope.groupBy;
    $scope.report = function () {
        $scope.target.merchandiseCodes = $scope.merchandiseCodes;
        $scope.target.nhaCungCapCodes = $scope.nhaCungCapCodes; 
        $scope.target.nhanVienCodes = $scope.nhanVienCodes;
        $rootScope.groupBy = $scope.groupBy;
        $scope.target.groupBy = $rootScope.groupBy;
        $state.go('baoCaoDatHangReport', { obj: $scope.target });

        //$scope.target.wareHouseCodes = $scope.wareHouseCodes;
        //$scope.target.merchandiseTypeCodes = $scope.merchandiseTypeCodes;
        //$scope.target.merchandiseGroupCodes = $scope.merchandiseGroupCodes;
        //$scope.target.merchandiseCodes = $scope.merchandiseCodes;
        //$scope.target.nhaCungCapCodes = $scope.nhaCungCapCodes;
        //$rootScope.groupBy = $scope.groupBy;
        //$scope.target.groupBy = $rootScope.groupBy;
        //if ($scope.target.groupBy === '') $scope.target.groupBy = 1;
    }


    $scope.exportExcel = function () {
        var filename = "";
        $scope.target.merchandiseCodes = $scope.merchandiseCodes;
        $scope.target.nhaCungCapCodes = $scope.nhaCungCapCodes;
        $scope.target.nhanVienCodes = $scope.nhanVienCodes;
        $rootScope.groupBy = $scope.groupBy;
        $scope.target.groupBy = $rootScope.groupBy;
        switch ($scope.target.groupBy) {
            case 2:
                filename = "DatHangTheoMaNhanVien";
                break;
            case 3:
                filename = "DatHangTheoHangHoa";
                break;
            case 4:
                filename = "DatHangTheoMaKhachHang";
                break;
            case 1:
                filename = "DatHangTheoTrangThai";
                break;
            default:
                filename = "XuatNhapTonChiTiet";
                break;
        }
        var postdata = { invoryexp: $scope.target };
        acDatHangService.postReportDatHangChiTiet($scope.target, filename);
    }


}]);

nvModule.controller('reportAcDatHangController', ['$scope', '$window', '$stateParams', '$timeout', '$state',
    'nvService', 'acDatHangService', 'mdService', 'clientService', 'configService',
function ($scope, $window, $stateParams, $timeout, $state,
nvService, acDatHangService, mdService, clientService, configService) {
    var para = $state.params.obj;
    $scope.paged = angular.copy(configService.pageReport);
    $scope.robot = angular.copy(acDatHangService.robot);
    $scope.tempData = mdService.tempData;
    $scope.target = [];
    $scope.lstSave = [];
    $scope.data = {};
    $scope.goIndex = function () {
        $state.go('inventory', { obj: para });
    }
    function filterData() {
        $scope.isLoading = true;
        if (para) {
            acDatHangService.postReportDatHang(para, function (response) {
                if (response.status) {
                    $scope.data = response.data;
                    $scope.isLoading = false;
                    //console.log('Inventory', $scope.data);
                    $scope.lstSave = $scope.data.detailData;
                }
                $scope.pageChanged();
            });
        }

    };

    $scope.search = function (code) {
        $scope.lst = [];
        if ($scope.data.detailData.length > 0) {
            if (code != "") {
                angular.forEach($scope.data.detailData, function (value, index) {
                    if (value.code.includes(code)) {
                        $scope.lst.push(value);
                    }
                });
                var currentPage = $scope.paged.currentPage;
                var itemsPerPage = $scope.paged.itemsPerPage;
                $scope.paged.totalItems = $scope.lst.length;
                $scope.result = [];
                if ($scope.lst) {
                    for (var i = (currentPage - 1) * itemsPerPage; i < currentPage * itemsPerPage && i < $scope.lst.length; i++) {
                        $scope.result.push($scope.lst[i]);
                    }
                }
            }
            else if (code === "") {
                $scope.data.detailData = [];
                $scope.data.detailData = $scope.lstSave;
                $scope.pageChanged();
            }

        }

    }


    $scope.print = function () {
        var table = document.getElementById('main-report').innerHTML;
        var myWindow = $window.open('', '', 'width=800, height=600');
        myWindow.document.write(table);
        myWindow.print();
    }
    $scope.printExcel = function () {
        acDatHangService.postExportExcelInventoryTongHop($scope.data, "BaoCaoTonTongHop");
    }

    filterData();

    $scope.pageChanged = function () {
        var currentPage = $scope.paged.currentPage;
        var itemsPerPage = $scope.paged.itemsPerPage;
        $scope.paged.totalItems = $scope.data.detailData.length;
        $scope.result = [];
        if ($scope.data.detailData) {
            for (var i = (currentPage - 1) * itemsPerPage; i < currentPage * itemsPerPage && i < $scope.data.detailData.length; i++) {
                $scope.result.push($scope.data.detailData[i]);
            }
        }
    }
}]);
