acModule.factory('baoCaoXuaNhapTonChiTietService', [
    '$resource', '$http', '$window', 'configService', 'clientService',
    function ($resource, $http, $window, configService, clientService) {
        var rootUrl = configService.apiServiceBaseUri;
        var serviceUrl = configService.rootUrlWebApi + '/Ac/BaoCaoXuatNhapTonChiTiet';
        var calc = {
            sum: function (obj, name) {
                var total = 0
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
            }
        }
        var result = {
            robot: calc,
            getUnitUsers: function (callback) {
                $http.get(rootUrl + '/api/Authorize/AuDonVi/GetSelectDataByUnitCode').success(callback);
            },
            getNewParameter: function (callback) {
                $http.get(rootUrl + '/api/Nv/GiaoDichQuay/GetNewParameter').success(callback);
            },
            postReportXNTNewTongHop: function (json, callback) {
                $http.post(rootUrl + '/api/Ac/BaoCaoXuatNhapTonChiTiet/ReportXNTNewTongHop', json).success(callback);
            },
            postExportExcelXNTNewTongHop: function (json, filename) {
                $http({
                    url: rootUrl + '/api/Ac/BaoCaoXuatNhapTonChiTiet/ExportExcelXNTNewTongHop',
                    method: "post",
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
                }).error(function (data, status, headers, config) {
                    //upload failed
                });
            },
            postReportXNTNewChiTiet: function (json, callback) {
                $http.post(rootUrl + '/api/Ac/BaoCaoXuatNhapTonChiTiet/ReportXNTNewChiTiet', json).success(callback);
            },
            postExportExcelXNTNewChiTiet: function (json, filename) {
                $http({
                    url: rootUrl + '/api/Ac/BaoCaoXuatNhapTonChiTiet/ExportExcelXNTNewChiTiet',
                    method: "post",
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
                }).error(function (data, status, headers, config) {
                    //upload failed
                });
            }
        }
        return result;
    }
]);



acModule.controller('baoCaoXuaNhapTonChiTietController', [
    '$scope', '$rootScope', '$window', '$stateParams', '$timeout', '$sce', '$state', '$uibModal', '$log',
    'mdService', 'baoCaoXuaNhapTonChiTietService', 'clientService', 'configService', '$filter', 'serviceInventoryAndSellingMachine', 'serviceInventoryAndWareHouse', 'serviceInventoryAndMerchandiseType', 'serviceInventoryAndMerchandise', 'serviceInventoryAndMerchandiseGroup', 'serviceInventoryAndNhaCungCap', 'inventoryService',
    function ($scope, $rootScope, $window, $stateParams, $timeout, $sce, $state, $uibModal, $log,
        mdService, baoCaoXuaNhapTonChiTietService, clientService, configService, $filter, serviceInventoryAndSellingMachine, serviceInventoryAndWareHouse, serviceInventoryAndMerchandiseType, serviceInventoryAndMerchandise, serviceInventoryAndMerchandiseGroup, serviceInventoryAndNhaCungCap, inventoryService) {
        $scope.tempData = mdService.tempData;
        $scope.config = configService;
        $scope.target = {};
        $scope.tagSellingMachine = [];
        $scope.tagWareHouses = [];
        $scope.tagNhaCungCaps = [];

        $scope.tagMerchandiseTypes = [];
        $scope.tagMerchandises = [];
        $scope.tagMerchandiseGroups = [];

        $scope.options = {
            minDate: null,
            maxDate: null
        };
        $scope.loaiGiaoDichs = [
            { text: "Xuất bán lẻ", value: 1 },
            { text: "Nhập bán lẻ trả lại", value: 2 }
        ];
        //Kho hàng
        $scope.selectWareHouse = function () {
            var modalInstance = $uibModal.open({
                backdrop: 'static',
                templateUrl: mdService.buildUrl('mdWareHouse', 'selectData'),
                controller: 'wareHouseSelectDataController',
                resolve: {
                    serviceSelectData: function () {
                        return serviceInventoryAndWareHouse;
                    },
                    filterObject: function () {
                        return {
                            advanceData: {
                                unitCode: $scope.target.unitCode,
                            },
                            isAdvance: true
                        }
                    }
                }
            });
            modalInstance.result.then(function (updatedData) {
            }, function () {
                $log.info('Modal dismissed at: ' + new Date());
            });
        }
        $scope.removeWareHouse = function (index) {
            $scope.tagWareHouses.splice(index, 1);
        }
        $scope.getWareHouseImportByUnit = function (code) {
            if (code) {
                inventoryService.getPeriodByUnit(code, function (response) {
                    $scope.periods = response;
                });
                inventoryService.getWareHouseByUnit(code, function (response) {
                    $scope.wareHouses = response;
                });
            };
        }

        $scope.$watch('tagWareHouses', function (newValue, oldValue) {
            var values = $scope.tagWareHouses.map(function (element) {
                return element.value;
            });
            $scope.target.wareHouseCodes = values.join();
        }, true);

        //Loại hàng
        $scope.selectMerchandiseType = function () {
            var modalInstance = $uibModal.open({
                backdrop: 'static',
                templateUrl: mdService.buildUrl('MdMerchandiseType', 'selectData'),
                controller: 'merchandiseTypeSelectDataController',
                resolve: {
                    serviceSelectData: function () {
                        return serviceInventoryAndMerchandiseType;
                    },
                    filterObject: function () {
                        return {

                        }
                    }
                }
            });
            modalInstance.result.then(function (updatedData) {
            }, function () {
                $log.info('Modal dismissed at: ' + new Date());
            });
        }
        $scope.removeMerchandiseType = function (index) {
            $scope.tagMerchandiseTypes.splice(index, 1);
        }
        $scope.$watch('tagMerchandiseTypes', function (newValue, oldValue) {
            var values = $scope.tagMerchandiseTypes.map(function (element) {
                return element.value;
            });
            $scope.target.merchandiseTypeCodes = values.join();
        }, true);
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

                        }
                    }
                }
            });
            modalInstance.result.then(function (updatedData) {
            }, function () {
                $log.info('Modal dismissed at: ' + new Date());
            });
        }
        $scope.removeMerchandise = function (index) {
            $scope.tagMerchandises.splice(index, 1);
        }
        $scope.$watch('tagMerchandises', function (newValue, oldValue) {
            var values = $scope.tagMerchandises.map(function (element) {
                return element.value;
            });
            $scope.target.merchandiseCodes = values.join();
        }, true);

        //Nhóm hàng
        $scope.selectMerchandiseGroup = function () {
            var modalInstance = $uibModal.open({
                backdrop: 'static',
                templateUrl: mdService.buildUrl('MdNhomVatTu', 'selectData'),
                controller: 'nhomVatTuSelectDataController',
                resolve: {
                    serviceSelectData: function () {
                        return serviceInventoryAndMerchandiseGroup;
                    },
                    filterObject: function () {
                        return {

                        }
                    }
                }
            });
            modalInstance.result.then(function (updatedData) {
            }, function () {
                $log.info('Modal dismissed at: ' + new Date());
            });
        }
        $scope.removeMerchandiseGroup = function (index) {
            $scope.tagMerchandiseGroups.splice(index, 1);
        }
        $scope.$watch('tagMerchandiseGroups', function (newValue, oldValue) {
            var values = $scope.tagMerchandiseGroups.map(function (element) {
                return element.value;
            });
            $scope.target.merchandiseGroupCodes = values.join();
        }, true);
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

                            }
                        }
                    }
                }
            });
            modalInstance.result.then(function (updatedData) {
            }, function () {
                $log.info('Modal dismissed at: ' + new Date());
            });
        }
        $scope.removeNhaCungCap = function (index) {
            $scope.tagNhaCungCaps.splice(index, 1);
        }
        $scope.$watch('tagNhaCungCaps', function (newValue, oldValue) {
            var values = $scope.tagNhaCungCaps.map(function (element) {
                return element.value;
            });
            $scope.target.nhaCungCapCodes = values.join();
        }, true);
        $rootScope.$on('$locationChangeStart',
            function (event, next, current) {
                $scope.tagWareHouses.clear();
                $scope.tagMerchandiseTypes.clear();
                $scope.tagMerchandises.clear();
                $scope.tagMerchandiseGroups.clear();
                $scope.tagNhaCungCaps.clear();
            });

        function filterData() {

            baoCaoXuaNhapTonChiTietService.getUnitUsers(function (response) {
                $scope.unitUsers = response;
            });
            baoCaoXuaNhapTonChiTietService.getNewParameter(function (response) {
                $scope.target = response;
                $scope.options.maxDate = response.maxDate;
            });
            $scope.tagWareHouses = serviceInventoryAndWareHouse.getSelectData();
            $scope.tagMerchandiseTypes = serviceInventoryAndMerchandiseType.getSelectData();
            $scope.tagMerchandises = serviceInventoryAndMerchandise.getSelectData();
            $scope.tagMerchandiseGroups = serviceInventoryAndMerchandiseGroup.getSelectData();
            $scope.tagNhaCungCaps = serviceInventoryAndNhaCungCap.getSelectData();
            $scope.tagSellingMachine = serviceInventoryAndSellingMachine.getSelectData();


        }
        //báo cáo excel chi tiết theo điều kiện
        filterData();
        $scope.reportChiTiet = function () {
            $state.go('reportXNTNewChiTiet', { obj: $scope.target });
        }
        $scope.reportTongHop = function () {
            $state.go('reportXNTNewTongHop', { obj: $scope.target });
        }

    }
]);
//XNT NEW TONG HOP
acModule.controller('XNTNewTongHopReportController', ['$scope', '$window', '$stateParams', '$timeout', '$state',
    'nvService', 'baoCaoXuaNhapTonChiTietService', 'mdService', 'clientService', 'configService',
function ($scope, $window, $stateParams, $timeout, $state,
nvService, baoCaoXuaNhapTonChiTietService, mdService, clientService, configService) {
    var para = $state.params.obj;
    $scope.paged = angular.copy(configService.pageReport);
    $scope.robot = angular.copy(baoCaoXuaNhapTonChiTietService.robot);
    $scope.tempData = mdService.tempData;
    $scope.target = {};
    $scope.data = {};
    $scope.lstSave = [];
    $scope.goIndex = function () {
        $state.go('baoCaoXuaNhapTonChiTiet');
    }
    function filterData() {
        if (para) {
            baoCaoXuaNhapTonChiTietService.postReportXNTNewTongHop(para, function (response) {
                if (response) {
                    $scope.target = response;
                    $scope.lstSave = $scope.target.data.dataDetails;
                }
                $scope.pageChanged();
            });
            $scope.loaiGdich = para.loaiGiaoDich;
        }
    };
    $scope.pageChanged = function () {
        var currentPage = $scope.paged.currentPage;
        var itemsPerPage = $scope.paged.itemsPerPage;
        $scope.paged.totalItems = $scope.target.data.dataDetails.length;
        $scope.result = [];
        if ($scope.target.data.dataDetails) {
            for (var i = (currentPage - 1) * itemsPerPage; i < currentPage * itemsPerPage && i < $scope.target.data.dataDetails.length; i++) {
                $scope.result.push($scope.target.data.dataDetails[i]);
            }
        }
    }

    $scope.search = function (code) {
        $scope.lst = [];
        if ($scope.target.data.dataDetails.length > 0) {
            if (code != "") {
                angular.forEach($scope.target.data.dataDetails, function (value, index) {
                    if (value.ma.includes(code)) {
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
                $scope.target.data.dataDetails = [];
                $scope.target.data.dataDetails = $scope.lstSave;
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
        var filename = "";
        switch ($scope.target.groupBy) {
            case "1":
                filename = "BaoCaoXuatNhapTonChiTietTheoKho";
                break;
            case "3":
                filename = "BaoCaoXuatNhapTonChiTietTheoNhomHang";
                break;
            case "2":
                filename = "BaoCaoXuatNhapTonChiTietTheoLoaiHang";
                break;
            case "5":
                filename = "BaoCaoXuatNhapTonChiTietTheoNhaCungCap";
                break;
            default:
                filename = "BaoCaoXuatNhapTonChiTietTheoMaVatTu";
                break;
        }
        baoCaoXuaNhapTonChiTietService.postExportExcelXNTNewTongHop(para, filename, function () {
        });
    }
    filterData();
}]);
//XNT NEW CHI TIET
acModule.controller('XNTNewChiTietReportController', ['$scope', '$window', '$stateParams', '$timeout', '$state',
    'nvService', 'baoCaoXuaNhapTonChiTietService', 'mdService', 'clientService', 'configService',
function ($scope, $window, $stateParams, $timeout, $state,
nvService, baoCaoXuaNhapTonChiTietService, mdService, clientService, configService) {
    var para = $state.params.obj;
    $scope.paged = angular.copy(configService.pageReport);
    $scope.robot = angular.copy(baoCaoXuaNhapTonChiTietService.robot);
    $scope.tempData = mdService.tempData;
    $scope.target = {};
    $scope.goIndex = function () {
        $state.go('baoCaoXuaNhapTonChiTiet');
    }
    function filterData() {
        if (para) {
            baoCaoXuaNhapTonChiTietService.postReportXNTNewChiTiet(para, function (response) {
                if (response) {
                    $scope.target = response;
                    //console.log($scope.target);
                }
            });
            $scope.loaiGdich = para.loaiGiaoDich;

        }
    };

    $scope.print = function () {
        var table = document.getElementById('main-report').innerHTML;
        var myWindow = $window.open('', '', 'width=800, height=600');
        myWindow.document.write(table);
        myWindow.print();
    }
    $scope.printExcel = function () {
        var filename = "";
        switch ($scope.target.groupBy) {
            case "1":
                filename = "BaoCaoXuatNhapTonChiTietTheoKho";
                break;
            case "3":
                filename = "BaoCaoXuatNhapTonChiTietTheoNhomHang";
                break;
            case "2":
                filename = "BaoCaoXuatNhapTonChiTietTheoLoaiHang";
                break;
            case "5":
                filename = "BaoCaoXuatNhapTonChiTietTheoNhaCungCap";
                break;
            default:
                filename = "BaoCaoXuatNhapTonChiTietTheoMaVatTu";
                break;
        }
        baoCaoXuaNhapTonChiTietService.postExportExcelXNTNewChiTiet(para, filename, function () {
        });
    }
    filterData();
}]);