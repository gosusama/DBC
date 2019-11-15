acModule.factory('theoNhaCungCapService', ['$resource', '$http', '$window', 'configService', 'clientService',
function ($resource, $http, $window, configService, clientService) {
    var rootUrl = configService.apiServiceBaseUri;
    var serviceUrl = configService.rootUrlWebApi + '/Ac/TheoNhaCungCap';
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
        postCreateReporttheoNhaCungCapByStaff: function (filter, callback) {
            $http.post(serviceUrl + '/CreateReporttheoNhaCungCapByStaff', filter).success(callback);
        },
        getUnitUsers: function (callback) {
            $http.get(rootUrl + '/api/Authorize/AuDonVi/GetSelectAll').success(callback);
        },
        postPrintTranferCashieer: function (filter, callback) {
            $http.post(rootUrl + '/api/Nv/GiaoDichQuay/PostPrintTranferCashieer', filter).success(callback);
        },
        getNewParameter: function (callback) {
            $http.get(rootUrl + '/api/Nv/GiaoDichQuay/GetNewParameter').success(callback);
        },
        exportExcel: function (json, callback) {
            $http({
                url: rootUrl + '/api/Nv/GiaoDichQuay/exportExcel',
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
                a.download = "BaoCaoGiaoDichQuay.xlsx";
                a.click();
                window.URL.revokeObjectURL(url);
            }).error(function (data, status, headers, config) {
                //upload failed
            });

            //$http.get(rootUrl + '/api/Nv/GiaoDichQuay/exportExcel').success(callback);
        },
        postExportExcelXNTByMerchandiseByNCC: function (json) {
            $http({
                url: serviceUrl + '/PostExportExcelXNTByMerchandiseByNCC',
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
                a.download = "XuatNhapTonTheoHang.xlsx";
                a.click();
                // window.URL.revokeObjectURL(objectUrl);
            }).error(function (data, status, headers, config) {
                //upload failed
            });

            //$http.post(serviceUrl + '/WriteDataToExcel', data).success(callback);
        },

    }
    return result;
}])

acModule.controller('theoNhaCungCapController', ['$scope', '$rootScope', '$window', '$stateParams', '$timeout', '$sce', '$state', '$uibModal', '$log',
'mdService', 'theoNhaCungCapService', 'clientService', 'configService', '$filter', 'serviceInventoryAndNhaCungCap',
function ($scope, $rootScope, $window, $stateParams, $timeout, $sce, $state, $uibModal, $log,
mdService, theoNhaCungCapService, clientService, configService, $filter, serviceInventoryAndNhaCungCap) {
    $scope.tempData = mdService.tempData;
    $scope.config = configService;
    $scope.target = {};
    $scope.tagSellingMachine = [];
    $scope.tagWareHouses = [];

    $scope.options = {
        minDate: null,
        maxDate: null
    };

    function filterData() {
        $scope.tagWareHouses = serviceInventoryAndNhaCungCap.getSelectData();

        theoNhaCungCapService.getUnitUsers(function (response) {
            console.log(response);
            $scope.unitUsers = response;
        });
        theoNhaCungCapService.getNewParameter(function (response) {
            console.log(response);
            $scope.target = response;
            $scope.options.maxDate = response.maxDate;
        });



    }
    filterData();
    $scope.report = function () {
        $state.go('theoNhaCungCapReport', { obj: $scope.target });
    }
    $scope.exportExcel = function () {
        theoNhaCungCapService.postExportExcelXNTByMerchandiseByNCC($scope.target, function () {

        });
    }
    //Kho hàng
    $scope.selectWareHouse = function () {
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
        })
        $scope.target.wareHouseCodes = values.join();
    }, true);

    $rootScope.$on('$locationChangeStart',
function (event, next, current) {
    $scope.tagSellingMachine.clear();
})
}])
nvModule.controller('reportTheoNhaCungCapController', ['$scope', '$window', '$stateParams', '$timeout', '$state',
    'nvService', 'theoNhaCungCapService', 'mdService', 'clientService',
function ($scope, $window, $stateParams, $timeout, $state,
nvService, theoNhaCungCapService, mdService, clientService) {
    var para = $state.params.obj;
    $scope.robot = angular.copy(theoNhaCungCapService.robot);
    $scope.tempData = mdService.tempData;
    $scope.target = [];
    $scope.goIndex = function () {
        $state.go('theoNhaCungCapController');
    }
    function filterData() {
        if (para) {
            theoNhaCungCapService.postPrintTranferCashieer(para, function (response) {
                console.log(response);
                if (response != null) {
                    $scope.target = response;
                }
            });
        }
        //theoNhaCungCapService.getPrintTranferCashieer( function (response) {
        //        if (response!=null) {
        //            $scope.target.data = response;
        //        }
        //    });
    };

    $scope.print = function () {
        var table = document.getElementById('main-report').innerHTML;
        var myWindow = $window.open('', '', 'width=800, height=600');
        myWindow.document.write(table);
        myWindow.print();
    }

    filterData();
}]);