var phieuNhapKhoThanhPhamService = nvModule.factory('phieuNhapKhoThanhPhamService',
    ['$resource', '$http', '$window', 'configService', 'clientService',
  function ($resource, $http, $window, configService, clientService) {
      var rootUrl = configService.apiServiceBaseUri;
      var serviceUrl = rootUrl + '/api/Nv/NhapKhoThanhPham';
      var result = {
          post: function (data, callback) {
              $http.post(serviceUrl + '/Post', data).success(callback)
			    .error(function (msg) {
			        clientService.noticeAlert("Lỗi không xác định", "danger");
			    });
          },
          postQuery: function (data, callback) {
              $http.post(serviceUrl + '/PostQuery', data).success(callback);
          },
          getNewInstance: function (callback) {
              $http.get(serviceUrl + '/GetNewInstance').success(callback);
          },
          getReport: function (id, callback) {
              $http.get(serviceUrl + '/GetReport/' + id).success(callback);
          },
          getDetails: function (id, callback) {
              $http.get(serviceUrl + '/GetDetails/' + id).success(callback);
          },
          getWareHouse: function (id, callback) {
              $http.get(rootUrl + '/api/Md/WareHouse/' + id).success(callback);
          },
          postApproval: function (id, callback) {
              $http.post(serviceUrl + '/PostApproval', id).success(callback);
          },
          updateCT: function (params) {
              return $http.put(serviceUrl + '/' + params.id, params);
          },
      };
      return result;

  }
])

nvModule.controller('phieuNhapKhoThanhPhamController', [
'$scope', '$rootScope', '$location', '$window', '$uibModal', '$log', '$state', '$filter', '$http',
'phieuNhapKhoThanhPhamService', 'configService', 'clientService', 'nvService', 'mdService', 'blockUI', function (
    $scope, $rootScope, $location, $window, $uibModal, $log, $state, $filter, $http,
phieuNhapKhoThanhPhamService, configService, clientService, nvService, mdService, blockUI
    ) {
    $scope.config = nvService.config;
    $scope.paged = angular.copy(configService.pageDefault);
    $scope.filtered = angular.copy(configService.filterDefault);
    $scope.isEditable = true;
    $scope.setPage = function (pageNo) {
        $scope.paged.currentPage = pageNo;
        filterData();
    };
    $scope.sortType = 'maChungTu'; // set the default sort type
    $scope.sortReverse = false;  // set the default sort order
    $scope.doSearch = function () {
        $scope.paged.currentPage = 1;
        filterData();
    };
    $scope.pageChanged = function () {
        filterData();
    };

    $scope.goHome = function () {
        $state.go('home');
    };
    $scope.refresh = function () {
        $scope.setPage($scope.paged.currentPage);
    };
    $scope.title = function () {
        return 'Phiếu nhập kho thành phẩm từ sản xuất';
    };
    $scope.displayHepler = function(code, module)
    {
        var data = $filter('filter')(mdService.tempData[module], { value: code }, true);
        if (data && data.length == 1) {
            return data[0].text;
        }
        return "Empty!";
    }
    $scope.formatLabel = function (model, module) {
        if (!model) return "";
        var data = $filter('filter')(mdService.tempData[module], { value: model }, true);
        if (data && data.length == 1) {
            return data[0].text;
        }
        return "Empty!";
    };
    $scope.create = function () {
        var modalInstance = $uibModal.open({
            backdrop: 'static',
            templateUrl: nvService.buildUrl('NvphieuNhapKhoThanhPham', 'add'),
            controller: 'phieuNhapKhoThanhPhamCreateController',
            windowClass: 'app-modal-window',
            resolve: {}
            
        });

        modalInstance.result.then(function (updatedData) {
            $scope.refresh();
        }, function () {
            $log.info('Modal dismissed at: ' + new Date());
        });
    };
    $scope.update = function (target) {
        var modalInstance = $uibModal.open({
            backdrop: 'static',
            templateUrl: nvService.buildUrl('NvphieuNhapKhoThanhPham', 'update'),
            controller: 'phieuNhapKhoThanhPhamEditController',
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

    $scope.details = function (target) {
        var modalInstance = $uibModal.open({
            backdrop: 'static',
            templateUrl: nvService.buildUrl('NvphieuNhapKhoThanhPham', 'details'),
            controller: 'phieuNhapKhoThanhPhamDetailsController',
            windowClass: 'app-modal-window',
            resolve: {
                targetData: function () {
                    return target;
                }
            }
        });
    };

    filterData();
    function filterData() {
        $scope.isLoading = true;
        var postdata = { paged: $scope.paged, filtered: $scope.filtered };
        phieuNhapKhoThanhPhamService.postQuery(
            JSON.stringify(postdata),
            function (response) {
                $scope.isLoading = false;
                if (response.status) {
                    $scope.data = response.data.data;
                    angular.extend($scope.paged, response.data);
                    console.log($scope.paged);
                }
            });
    };
}])
nvModule.controller('phieuNhapKhoThanhPhamDetailsController', [
'$scope', '$uibModalInstance',
'mdService', 'phieuNhapKhoThanhPhamService', 'targetData', 'clientService', '$filter',
function ($scope, $uibModalInstance,
    mdService, phieuNhapKhoThanhPhamService, targetData, clientService, $filter) {

    $scope.config = mdService.config;
    $scope.target = targetData;
    $scope.tempData = mdService.tempData;
    $scope.title = function () {
        return 'Phiếu nhập kho thành phẩm từ sản xuất';
    };
    fillterData();
    $scope.formatLabel = function (model, module) {
        var data = $filter('filter')(mdService.tempData[module], { value: model }, true);
        if (data && data.length == 1) {
            return data[0].text;
        }
        return "Empty!";
    };
    $scope.approval = function () {
        phieuNhapKhoThanhPhamService.postApproval($scope.target, function (response) {
            if (response) {
                alert("Duyệt thành công!");
                fillterData();
            }
            else { alert("Thất bại! - Xảy ra lỗi hoặc phiếu này đã duyệt"); }
        });
    };
    function fillterData() {
        phieuNhapKhoThanhPhamService.getDetails($scope.target.id, function (response) {
            if (response.status) {
                $scope.target = response.data;
            }
        });
    }

    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
}
]);
nvModule.controller('phieuNhapKhoThanhPhamCreateController', [
    '$scope', '$uibModalInstance', 'clientService', '$filter', '$state', '$uibModal', '$log',
    'nvService', 'phieuNhapKhoThanhPhamService', 'mdService',
    function ($scope, $uibModalInstance, clientService, $filter, $state, $uibModal, $log,
        nvService, phieuNhapKhoThanhPhamService, mdService) {
        $scope.config = nvService.config;
        $scope.tempData = mdService.tempData;
        $scope.target = { dataDetails: [], dataClauseDetails: []};
        $scope.tkKtKhoNhap = "";
        $scope.countIndex = 0;
        $scope.formatLabel = function (model, module) {
            if (!model) return "";
            var data = $filter('filter')(mdService.tempData[module], { value: model }, true);
            if (data && data.length == 1) {
                return data[0].text;
            }
            return "Empty!";
        };
        $scope.setIndex = function (index) {
            $scope.selectedRow = index;
        }
        $scope.removeRow = function (index) {
            $scope.target.dataDetails.splice(index, 1);
        };
        function removeRowEx(index) {
            $scope.target.dataClauseDetails.splice(index, 1);
        };
        function addRowEx(item) {
            if (item) {

                if ($scope.target.dataClauseDetails.length > 0) {
                    item.doiTuongNo = $scope.target.dataClauseDetails[$scope.target.dataClauseDetails.length - 1].doiTuongNo;
                    item.doiTuongCo = $scope.target.dataClauseDetails[$scope.target.dataClauseDetails.length - 1].doiTuongCo;
                }
                $scope.target.dataClauseDetails.push(item);
            } else {
                var copyer = angular.copy($scope.target.dataClauseDetails[0]);
                copyer.tkCo = null;
                copyer.tkNo = null;
                copyer.soTien = 0;
                $scope.target.dataClauseDetails.push(copyer);
            }
        };
        $scope.addRow = function (defaultValue) {
            $scope.countIndex++;

            var item = {
                index: $scope.countIndex,
                maHang: '',
                maBaoBi: '',
                soLuongBao: 0,
                soLuong: 0,
                donGia: 0, //default
                thanhTien: 0
            };
            if ($scope.target.dataDetails.length > 0) {
                var preItem = $scope.target.dataDetails[$scope.target.dataDetails.length - 1];
                item.index = $scope.countIndex;
            }
            if (defaultValue) { item = defaultValue };

            var itemClause = {
                index: $scope.countIndex,
                tkNo: '155',
                tkCo: '154',
                doiTuongCo: null,
                doiTuongNo: null,
                soTien: 0
            }
            if ($scope.target.dataClauseDetails.length > 0) {
                var preItem = $scope.target.dataClauseDetails[$scope.target.dataClauseDetails.length - 1];
                itemClause = angular.copy(preItem);
                itemClause.index = $scope.countIndex;
                itemClause.tkNo = '155';
                itemClause.tkCo = '154';
                itemClause.soTien = 0;
            };

            $scope.target.dataDetails.push(item);
            $scope.target.dataClauseDetails.push(itemClause);
        };
        $scope.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        };
        $scope.title = function () {
            return 'Phiếu nhập kho thành phẩm từ sản xuất';
        };
        $scope.updateDetails = function () {
            angular.forEach($scope.target.dataDetails, function (value, key) {
                angular.extend(value, { tkKtKhoNhap: $scope.tkKtKhoNhap });
            });
        };
        $scope.selectedMaHang = function (model, value) {
        };
        $scope.selectedKhoNhap = function (item) {
            $scope.target.maKhoNhap = item.value;

            phieuNhapKhoThanhPhamService.getWareHouse(item.id, function (response) {
                if (response.taiKhoanKt == '155') {
                    angular.forEach($scope.target.dataClauseDetails, function (value, key) {
                        value.doiTuongNo = $scope.target.maKhoNhap;
                        value.tkNo = response.tkKtKhoNhap;
                    });
                    $scope.tkKtKhoNhap = response.taiKhoanKt;
                };
            });
        };
        $scope.selectedMaBaoBi = function(model, item, index)
        {
            var itemCurrent = $scope.target.dataDetails[index];
            itemCurrent.luongBao = parseFloat(item.extendValue);
            itemCurrent.soLuong = itemCurrent.soLuongBao * itemCurrent.luongBao;
            itemCurrent.thanhTien = itemCurrent.soLuong * itemCurrent.donGia;
            updateClause(itemCurrent.index, itemCurrent);
        }
        $scope.changeSoLuongBao = function (item) {
            item.soLuong = item.soLuongBao * item.luongBao;       
            item.thanhTien = item.soLuong * item.donGia;

            updateClause(item.index, item);
        }
        function updateClause(index, itemFocus) {
            angular.forEach($scope.target.dataClauseDetails, function (value, key) {
                if (value.index == index) {
                    value.soTien = itemFocus.thanhTien;
                };
            });
        };

        function sumDataDetail() {
            if ($scope.target.dataDetails.length > 0) {
                var total = 0;
                angular.forEach($scope.target.dataDetails, function (value, key) {
                    total = total + value.thanhTien;
            });
                return total;
        }
            return 0;
        };
        $scope.save = function () {
            $scope.updateDetails();
            phieuNhapKhoThanhPhamService.post(
                JSON.stringify($scope.target), function (response) {
                    if (response.status) {
                        clientService.noticeAlert("Thành công", "success");
                        $uibModalInstance.close($scope.target);
                    } else {
                        clientService.noticeAlert(response.message, "danger");
                    }
                }
                );
        };
        $scope.saveAndKeep = function () {
            $scope.updateDetails();
            var tempData = angular.copy($scope.target);
            phieuNhapKhoThanhPhamService.post(
                JSON.stringify($scope.target), function (response) {
                    if (response.status) {
                        clientService.noticeAlert("Thành công", "success");
                        phieuNhapKhoThanhPhamService.getNewInstance(function (response) {
                            var expectData = response;
                            tempData.maChungTu = expectData.maChungTu;
                            tempData.ngay = expectData.ngay;
                            $scope.target = tempData;
                        })
                    } else {
                        clientService.noticeAlert(response.message, "danger");
                    }    
                }
                );
        };
        $scope.saveAndPrint = function () {
            $scope.updateDetails();
            phieuNhapKhoThanhPhamService.post(
                JSON.stringify($scope.target), function (response) {
                    if (response.status) {
                        clientService.noticeAlert("Thành công", "success");
                        console.log(response);
                        var url = $state.href('reportphieuNhapKhoThanhPham', { id: response.id });
                        window.open(url, 'Report Viewer');
                        $uibModalInstance.close($scope.target);
                    } else {
                        clientService.noticeAlert(response.message, "danger");
                    }
                }
                );
        };
        $scope.createWareHouse = function () {

            var modalInstance = $uibModal.open({
                backdrop: 'static',
                templateUrl: mdService.buildUrl('mdWareHouse', 'add'),
                controller: 'wareHouseCreateController',
                resolve: {}
            });

            modalInstance.result.then(function (updatedData) {
                $scope.tempData.update('wareHouses');
            }, function () {
                $log.info('Modal dismissed at: ' + new Date());
            });
        };
        $scope.createMerchandise = function () {
            var modalInstance = $uibModal.open({
                backdrop: 'static',
                templateUrl: mdService.buildUrl('mdMerchandise', 'add'),
                controller: 'merchandiseCreateController',
                windowClass: 'app-modal-window',
                resolve: {}
            });

            modalInstance.result.then(function (updatedData) {
                $scope.tempData.update('merchandises');
            }, function () {
                $log.info('Modal dismissed at: ' + new Date());
            });
        };
        $scope.createPackage = function () {
            var modalInstance = $uibModal.open({
                backdrop: 'static',
                templateUrl: mdService.buildUrl('mdPackaging', 'add'),
                controller: 'packagingCreateController',
                resolve: {}
            });

            modalInstance.result.then(function (updatedData) {
                $scope.tempData.update('packagings');
            }, function () {
                $log.info('Modal dismissed at: ' + new Date());
            });
        };
        function filterData() {
            phieuNhapKhoThanhPhamService.getNewInstance(function (response) {

                $scope.target = response;
                if ($scope.target.dataDetails.length == 0) {
                    $scope.addRow();
                }
            })
        };
        filterData();

        }
]);
nvModule.controller('phieuNhapKhoThanhPhamEditController', [
    '$scope', '$uibModalInstance', 'clientService', '$filter', '$state', '$uibModal', '$log',
    'nvService', 'phieuNhapKhoThanhPhamService', 'mdService', 'targetData',
    function ($scope, $uibModalInstance, clientService, $filter, $state, $uibModal, $log,
        nvService, phieuNhapKhoThanhPhamService, mdService, targetData) {
        $scope.config = nvService.config;
        $scope.tempData = mdService.tempData;
        $scope.target = targetData;
        $scope.tkKtKhoNhap = "";
        $scope.formatLabel = function (model, module) {
            if (!model) return "";
            var data = $filter('filter')(mdService.tempData[module], { value: model }, true);
            if (data && data.length == 1) {
                return data[0].text;
            }
            return "Empty!";
        };

        $scope.setIndex = function (index) {
            $scope.selectedRow = index;
        }
        $scope.removeRow = function (index) {
            $scope.target.dataDetails.splice(index, 1);
        };
        function removeRowEx(index) {
            $scope.target.dataClauseDetails.splice(index, 1);
        };
        function addRowEx(item) {
            if (item) {

                if ($scope.target.dataClauseDetails.length > 0) {
                    item.doiTuongNo = $scope.target.dataClauseDetails[$scope.target.dataClauseDetails.length - 1].doiTuongNo;
                    item.doiTuongCo = $scope.target.dataClauseDetails[$scope.target.dataClauseDetails.length - 1].doiTuongCo;
                }
                $scope.target.dataClauseDetails.push(item);
            } else {
                var copyer = angular.copy($scope.target.dataClauseDetails[0]);
                copyer.tkCo = null;
                copyer.tkNo = null;
                copyer.soTien = 0;
                $scope.target.dataClauseDetails.push(copyer);
            }
        };
        $scope.addRow = function (defaultValue) {
            $scope.countIndex++;

            var item = {
                index: $scope.countIndex,
                maHang: '',
                maBaoBi: '',
                soLuongBao: 0,
                soLuong: 0,
                donGia: 0, //default
                thanhTien: 0
            };
            if ($scope.target.dataDetails.length > 0) {
                var preItem = $scope.target.dataDetails[$scope.target.dataDetails.length - 1];
                item.index = $scope.countIndex;
            }
            if (defaultValue) { item = defaultValue };

            var itemClause = {
                index: $scope.countIndex,
                tkNo: '155',
                tkCo: '154',
                doiTuongCo: null,
                doiTuongNo: null,
                soTien: 0
            }
            if ($scope.target.dataClauseDetails.length > 0) {
                var preItem = $scope.target.dataClauseDetails[$scope.target.dataClauseDetails.length - 1];
                itemClause = angular.copy(preItem);
                itemClause.index = $scope.countIndex;
                itemClause.tkNo = '155';
                itemClause.tkCo = '154';
                itemClause.soTien = 0;
            };

            $scope.target.dataDetails.push(item);
            $scope.target.dataClauseDetails.push(itemClause);
        };

        $scope.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        };
        $scope.title = function () {
            return 'Phiếu nhập kho thành phẩm từ sản xuất';
        };
        $scope.updateDetails = function () {
            angular.forEach($scope.target.dataDetails, function (value, key) {
                angular.extend(value, { tkKtKhoNhap: $scope.tkKtKhoNhap });
            });
        };
        $scope.selectedMaHang = function (model, value) {
        };
        $scope.selectedKhoNhap = function (item) {
            $scope.target.maKhoNhap = item.value;

            phieuNhapKhoThanhPhamService.getWareHouse(item.id, function (response) {
                if (response.taiKhoanKt == '155') {
                    angular.forEach($scope.target.dataClauseDetails, function (value, key) {
                        value.doiTuongNo = $scope.target.maKhoNhap;
                        value.tkNo = response.tkKtKhoNhap;
                    });
                    $scope.tkKtKhoNhap = response.taiKhoanKt;
                };
            });
        };
        $scope.selectedMaBaoBi = function (model, item, index) {
            var itemCurrent = $scope.target.dataDetails[index];
            itemCurrent.luongBao = parseFloat(item.extendValue);
            itemCurrent.soLuong = itemCurrent.soLuongBao * itemCurrent.luongBao;
            itemCurrent.thanhTien = itemCurrent.soLuong * itemCurrent.donGia;
            updateClause(itemCurrent.index, itemCurrent);
        }
        $scope.changeSoLuongBao = function (item) {
            item.soLuong = item.soLuongBao * item.luongBao;
            item.thanhTien = item.soLuong * item.donGia;

            updateClause(item.index, item);
        }
        $scope.save = function () {
            $scope.updateDetails();
            phieuNhapKhoThanhPhamService.updateCT($scope.target).then(
                    function (response) {
                        if (response.status && response.status == 200) {
                            if (response.data.status) {
                                console.log('Create  Successfully!');
                                clientService.noticeAlert("Thành công", "success");
                                $uibModalInstance.close($scope.target);
                            } else {
                                clientService.noticeAlert(response.message, "danger");
                            }
                        } else {
                            console.log('ERROR: Update failed! ' + response.errorMessage);
                            clientService.noticeAlert(response.errorMessage, "danger");
                        }
                    },
                    function (response) {
                        console.log('ERROR: Update failed! ' + response);
                    }
                );
        };
        $scope.saveAndPrint = function () {
            $scope.updateDetails();
            phieuNhapKhoThanhPhamService.updateCT($scope.targe).then(
                    function (response) {
                        if (response.data.status) {
                            clientService.noticeAlert("Thành công", "success");
                            console.log('Update Successfully!');
                            var url = $state.href('reportphieuNhapHangMua', { id: response.data.data.id });
                            window.open(url, 'Report Viewer');
                            $uibModalInstance.close($scope.target);
                        } else {
                            clientService.noticeAlert(response.data.message, "danger");
                        }
                    },
                    function (response) {
                        console.log('ERROR: Update failed! ' + response);
                    }
                );
        }
        function updateClause(index, itemFocus) {
            angular.forEach($scope.target.dataClauseDetails, function (value, key) {
                if (value.index == index) {
                    value.soTien = itemFocus.thanhTien;
                };
            });
        };
        function sumDataDetail() {
            if ($scope.target.dataDetails.length > 0) {
                var total = 0;
                angular.forEach($scope.target.dataDetails, function (value, key) {
                    total = total + value.thanhTien;
                });
                return total;
            }
            return 0;
        };
        function filterData() {
            phieuNhapKhoThanhPhamService.getDetails($scope.target.id, function (response) {
                if (response.status) {

                    $scope.target = response.data;
                    var max = 0;
                    angular.forEach($scope.target.dataDetails, function (value, key) {
                        if (value.index > 0) {
                            $scope.countIndex = value.index;
                            max = value.index;;
                        }

                    });
                    init();
        }
            });

        };
        function init() {
        }
        $scope.createWareHouse = function () {

            var modalInstance = $uibModal.open({
                backdrop: 'static',
                templateUrl: mdService.buildUrl('mdWareHouse', 'add'),
                controller: 'wareHouseCreateController',
                resolve: {}
            });

            modalInstance.result.then(function (updatedData) {
                $scope.tempData.update('wareHouses');
            }, function () {
                $log.info('Modal dismissed at: ' + new Date());
            });
        };
        $scope.createMerchandise = function () {
            var modalInstance = $uibModal.open({
                backdrop: 'static',
                templateUrl: mdService.buildUrl('mdMerchandise', 'add'),
                controller: 'merchandiseCreateController',
                windowClass: 'app-modal-window',
                resolve: {}
            });

            modalInstance.result.then(function (updatedData) {
                $scope.tempData.update('merchandises');
            }, function () {
                $log.info('Modal dismissed at: ' + new Date());
            });
        };
        $scope.createPackage = function () {
            var modalInstance = $uibModal.open({
                backdrop: 'static',
                templateUrl: mdService.buildUrl('mdPackaging', 'add'),
                controller: 'packagingCreateController',
                resolve: {}
            });

            modalInstance.result.then(function (updatedData) {
                $scope.tempData.update('packagings');
            }, function () {
                $log.info('Modal dismissed at: ' + new Date());
            });
        };
        filterData();
    }
]);
nvModule.controller('reportphieuNhapKhoThanhPhamController', ['$scope', '$window', '$stateParams', '$timeout',
'mdService', 'phieuNhapKhoThanhPhamService', 'clientService',
function ($scope, $window, $stateParams, $timeout,
mdService, phieuNhapKhoThanhPhamService, clientService) {
    var id = $stateParams.id;
    $scope.target = {};

    function filterData() {
        if (id) {
            phieuNhapKhoThanhPhamService.getReport(id, function (response) {

                if (response.status) {
                    $scope.target = response.data;

                }
            });
        }
    };

    $scope.print = function () {
        var table = document.getElementById('test').innerHTML;
        var myWindow = $window.open('', '', 'width=800, height=600');
        myWindow.document.write(table);
        myWindow.print();
    }

    $scope.$on('$viewContentLoaded', function () {
        $scope.$watch('target', function (newVal, oldVal) {

            //Force angular not to fire script on load
            if (newVal != oldVal) {

                //Force script to run AFTER ng-repeat has rendered its things to the DOM
                $timeout(function () {

                    //And finally setTimout to wait for browser to render the custom fonts for print preview
                    setTimeout(function () {

                        //Print document
                        $scope.print();
                        //window.close();
                    }, 100);
                }, 0);
            }
        }, true);
    });
    filterData();
}]);




   