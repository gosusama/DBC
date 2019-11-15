acModule.factory('importExportService', [
    '$resource', '$http', '$window', 'configService', 'clientService',
    function ($resource, $http, $window, configService, clientService) {
        var rootUrl = configService.apiServiceBaseUri;
        var serviceUrl = configService.rootUrlWebApi + '/Ac/ImportExport';
        //var serviceUrl = rootUrl + '/_vti_bin/Apps/AC/CloseoutService.svc';
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
            },
        }
        var result = {
            robot: calc,
            postInventoryReportByDay: function (filter, callback) {
                $http.post(serviceUrl + '/ReportInventoryByDay', filter).success(callback);
            },
            getWareHouseByUnit: function (maDonVi, callback) {
                $http.get(rootUrl + '/api/Md/WareHouse/GetByUnit/' + maDonVi).success(callback);
            },
            getPeriodByUnit: function (maDonVi, callback) {
                $http.get(rootUrl + '/api/Md/Period/GetByUnit/' + maDonVi).success(callback);
            },
            getNewParameter: function (callback) {
                $http.get(serviceUrl + '/GetNewParameter').success(callback);
            },
            getUnitUsers: function (callback) {
                $http.get(rootUrl + '/api/Authorize/AuDonVi/GetSelectDataByUnitCode').success(callback);
            },
            postExportExcelXNT: function (json, filename) {
                $http({
                    url: serviceUrl + '/PostExportExcelXNT',
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
            getLastPeriod: function (callback) {
                $http.get(serviceUrl + '/GetLastPeriod').success(callback);
            },
            postApproval: function (data, callback) {
                return $http.post(configService.rootUrlWebApi + '/Md/Period/PostUpdateGiaoDich', data).success(callback);
            },
            postExcelInventoryReportByDay: function (json, filename) {
                $http({
                    url: serviceUrl + '/ExportInventoryByDay',
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
            postReportInventoryDetail: function (data, callback) {
                $http.post(serviceUrl + '/PostReportInventoryDetail', data).success(callback);
            }
        }
        return result;
    }
]);

acModule.controller('importExportController', [
    '$scope', '$rootScope', '$window', '$stateParams', '$timeout', '$sce', '$state', '$uibModal', '$log',
    'mdService', 'importExportService', 'clientService', '$filter',
	'serviceInventoryAndWareHouse', 'serviceInventoryAndMerchandiseType', 'serviceInventoryAndMerchandise',
	'serviceInventoryAndMerchandiseGroup', 'serviceInventoryAndNhaCungCap', 'configService', 'wareHouseService',
	'merchandiseTypeService', 'merchandiseService', 'nhomVatTuService', 'customerService',
    function ($scope, $rootScope, $window, $stateParams, $timeout, $sce, $state, $uibModal, $log,
        mdService, importExportService, clientService,
		$filter, serviceInventoryAndWareHouse, serviceInventoryAndMerchandiseType, serviceInventoryAndMerchandise,
		serviceInventoryAndMerchandiseGroup, serviceInventoryAndNhaCungCap, configService, wareHouseService,
		merchandiseTypeService, merchandiseService, nhomVatTuService, customerService) {
        $scope.tempData = mdService.tempData;
        $scope.target = {};
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
                var output = '';
                angular.forEach(updatedData, function (item, index) {
                    output += item.value + ',';
                });
                $scope.wareHouseCodes = output.substring(0, output.length - 1);
            }, function () {
                $log.info('Modal dismissed at: ' + new Date());
            });
        }

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
                var output = '';
                angular.forEach(updatedData, function (item, index) {
                    output += item.value + ',';
                });
                $scope.merchandiseTypeCodes = output.substring(0, output.length - 1);
            }, function () {
                $log.info('Modal dismissed at: ' + new Date());
            });


        }
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
                            isAdvance: true,
                            advanceData: { maLoaiVatTu: $scope.target.merchandiseTypeCodes }
                        }
                    }
                }
            });
            modalInstance.result.then(function (updatedData) {
                var output = '';
                angular.forEach(updatedData, function (item, index) {
                    output += item.value + ',';
                });
                $scope.merchandiseGroupCodes = output.substring(0, output.length - 1);
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
        $scope.groupBy = '';
        //filter by kho
        $scope.changewareHouseCodes = function (inputwareHouse) {
            if (typeof inputwareHouse != 'undefined' && inputwareHouse !== '') {
                wareHouseService.filterWareHouse(inputwareHouse, function (response) {
                    if (response) {
                        $scope.data = response;
                        $scope.wareHouseCodes = '';
                        $scope.wareHouseCodes = $scope.data.maKho;
                    }
                    else {
                        //$scope.selectWareHouse();
                    }
                });
            }
        }
        //filter by loai
        $scope.changeTypeMerchandiseCodes = function (inputTypeMer) {
            if (typeof inputTypeMer != 'undefined' && inputTypeMer !== '') {
                merchandiseTypeService.filterTypeMerchandiseCodes(inputTypeMer, function (response) {
                    if (response) {
                        $scope.data = response;
                        $scope.merchandiseTypeCodes = '';
                        $scope.merchandiseTypeCodes = $scope.data.maLoaiVatTu;
                    }
                    else {
                        //$scope.selectMerchandiseType();
                    }
                });
            }
        }
        //filter by nhom hang hoa
        $scope.changeMerchandiseGroup = function (inputMerchandiseGroup) {
            if (typeof inputMerchandiseGroup != 'undefined' && inputMerchandiseGroup !== '') {
                nhomVatTuService.filterGroupMerchandiseCodes(inputMerchandiseGroup, function (response) {
                    if (response) {
                        $scope.data = response;
                        $scope.merchandiseGroupCodes = '';
                        $scope.merchandiseGroupCodes = $scope.data.maNhom;
                    }
                    else {
                       // $scope.selectMerchandiseGroup();
                    }
                });
            }
        }



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
                       // $scope.selectMerchandise();
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
                      //  $scope.selectNhaCungCap();
                    }
                });
            }
        }
        // end filter

        $scope.getWareHouseImportByUnit = function (code) {
            if (code) {
                importExportService.getPeriodByUnit(code, function (response) {
                    $scope.periods = response;
                });
                importExportService.getWareHouseByUnit(code, function (response) {
                    $scope.wareHouses = response;
                });
            };
        }
        $scope.options = {
            minDate: null,
            maxDate: null
        };

        function filterData() {
            $scope.tagWareHouses = serviceInventoryAndWareHouse.getSelectData();
            $scope.tagMerchandiseTypes = serviceInventoryAndMerchandiseType.getSelectData();
            $scope.tagMerchandises = serviceInventoryAndMerchandise.getSelectData();
            $scope.tagMerchandiseGroups = serviceInventoryAndMerchandiseGroup.getSelectData();
            $scope.tagNhaCungCaps = serviceInventoryAndNhaCungCap.getSelectData();
            importExportService.getUnitUsers(function (response) {
                $scope.unitUsers = response;
            });
            importExportService.getNewParameter(function (response) {
                $scope.target = response;
                $scope.options.minDate = response.minDate;
                $scope.options.maxDate = response.maxDate;
            });
            importExportService.getLastPeriod(function (response) {
                $scope.lastPeriod = response;
            });

        }
        filterData();
        if ($state.params.obj != null) {
            var param = $state.params.obj;
            $scope.target = angular.copy(param);
            $scope.target.fromDate = param.fromDate;
            $scope.target.toDate = param.toDate;
            $scope.target.unitCode = param.unitCode;
            $scope.wareHouseCodes = param.wareHouseCodes;
            $scope.merchandiseTypeCodes = param.merchandiseTypeCodes;
            $scope.merchandiseGroupCodes = param.merchandiseGroupCodes;
            $scope.merchandiseCodes = param.merchandiseCodes;
            $scope.nhaCungCapCodes = param.nhaCungCapCodes;
            $scope.target.groupBy = $rootScope.groupBy;
            $scope.groupBy = $rootScope.groupBy;
        }
        if ($scope.groupBy != '') $scope.target.groupBy = $scope.groupBy;
        $scope.report = function () {
            $scope.target.wareHouseCodes = $scope.wareHouseCodes;
            $scope.target.merchandiseTypeCodes = $scope.merchandiseTypeCodes;
            $scope.target.merchandiseGroupCodes = $scope.merchandiseGroupCodes;
            $scope.target.merchandiseCodes = $scope.merchandiseCodes;
            $scope.target.nhaCungCapCodes = $scope.nhaCungCapCodes;
            $rootScope.groupBy = $scope.groupBy;
            $scope.target.groupBy = $rootScope.groupBy;
            if ($scope.target.groupBy === '') $scope.target.groupBy = 1;
            $state.go('reportImportExport', { obj: $scope.target });
        }

        $scope.exportExcel = function () {
            var filename = "";
            $scope.target.wareHouseCodes = $scope.wareHouseCodes;
            $scope.target.merchandiseTypeCodes = $scope.merchandiseTypeCodes;
            $scope.target.merchandiseGroupCodes = $scope.merchandiseGroupCodes;
            $scope.target.merchandiseCodes = $scope.merchandiseCodes;
            $scope.target.nhaCungCapCodes = $scope.nhaCungCapCodes;
            $rootScope.groupBy = $scope.groupBy;
            $scope.target.groupBy = $rootScope.groupBy;
            switch ($scope.target.groupBy) {
                case "2":
                    filename = "XuatNhapTonTheoLoaiHang";
                    break;
                case "3":
                    filename = "XuatNhapTonTheoNhomHang";
                    break;
                case "5":
                    filename = "XuatNhapTonTheoNhaCungCap";
                    break;
                case "1":
                    filename = "XuatNhapTonTheoKho";
                    break;

                default:
                    filename = "XuatNhapTonChiTiet";
                    break;
            }
            var postdata = { invoryexp: $scope.target };
            importExportService.postExportExcelXNT($scope.target, filename);
        }
        $scope.approval = function () {
            importExportService.postApproval(
                JSON.stringify($scope.lastPeriod),
                function (response) {
                    //Fix
                    if (response.status) {
                        console.log('Create  Successfully!');
                        clientService.noticeAlert(response.message, "success");
                        $scope.isDisabled = false;
                    } else {
                        $scope.isDisabled = false;

                        clientService.noticeAlert(response.message, "danger");
                    }
                    $scope.stateIsRunning = false;
                    //End fix
                }).error(function (error) {
                    $scope.isDisabled = false;
                    $scope.stateIsRunning = false;
                });
        }
    }
]);
nvModule.controller('reportImportExportController', ['$scope', '$window', '$stateParams', '$timeout', '$state',
    'nvService', 'importExportService', 'mdService', 'clientService', 'configService','authorizeService','$filter',
function ($scope, $window, $stateParams, $timeout, $state,
nvService, importExportService, mdService, clientService, configService, authorizeService, $filter) {
    var para = $state.params.obj;
    $scope.paged = angular.copy(configService.pageReport);
    $scope.robot = angular.copy(importExportService.robot);
    $scope.tempData = mdService.tempData;
    $scope.data = {};
    $scope.lstSave = [];
    $scope.target = [];
    $scope.goIndex = function () {
        $state.go('importExport', { obj: para });
    }
    $scope.formatLabel = function (model, module) {
        if (!model) return "";
        var data = $filter('filter')(authorizeService.tempData[module], { value: model }, true);
        if (data && data.length == 1) {
            return data[0].text;
        }
        return "Empty!";
    };
    function filterData() {
        $scope.isLoading = true;
        if (para) {
            importExportService.postInventoryReportByDay(para, function (response) {
                if (response.status) {
                    //console.log('ByDay', response.data);
                    $scope.condition = response.data;
                    $scope.data = response.data;
                    $scope.isLoading = false;
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

    $scope.print = function () {
        var table = document.getElementById('main-report').innerHTML;
        var myWindow = $window.open('', '', 'width=800, height=600');
        myWindow.document.write(table);
        myWindow.print();
    }
    $scope.printExcel = function () {
        importExportService.postExcelInventoryReportByDay($scope.data, "BaoCaoXuatNhapTonTongHop");
    }
    filterData();
}]);
nvModule.controller('reportInventoryDetailController', ['$scope', '$window', '$stateParams', '$timeout', '$state', '$filter',
    'nvService', 'importExportService', 'mdService', 'clientService',
function ($scope, $window, $stateParams, $timeout, $state, $filter,
nvService, importExportService, mdService, clientService) {
    var para = $state.params.obj;
    $scope.robot = angular.copy(importExportService.robot);
    $scope.tempData = mdService.tempData;
    $scope.target = [];
    $scope.goIndex = function () {
        $state.go('importExport');
    }
    function filterData() {
        $scope.isLoading = true;
        if (para) {
            importExportService.postReportInventoryDetail(para, function (response) {
                if (response.status) {
                    console.log('inventoryDetail', response.data);
                    $scope.data = response.data;
                    $scope.isLoading = false;
                }
            });
        }
    };
    $scope.displayHepler = function (code, module) {
        if (!code) {
            return "";
        }
        var data = $filter('filter')(mdService.tempData[module], { value: code }, true);
        if (data && data.length == 1) {
            return data[0].text;
        };

    }
    $scope.print = function () {
        var table = document.getElementById('main-report').innerHTML;
        var myWindow = $window.open('', '', 'width=800, height=600');
        myWindow.document.write(table);
        myWindow.print();
    }
    filterData();
}]);
