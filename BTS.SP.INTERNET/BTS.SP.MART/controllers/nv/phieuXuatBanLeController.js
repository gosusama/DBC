nvModule.factory('phieuXuatBanLeService',
    ['$resource', '$http', '$window', 'configService', 'clientService',
  function ($resource, $http, $window, configService, clientService) {
      var rootUrl = configService.apiServiceBaseUri;
      this.parameterPrint = {};
      function getParameterPrint() {
          return this.parameterPrint;
      }
      var serviceUrl = rootUrl + '/api/Nv/XuatBanLe';

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
          }

      }

      var result = {
          robot: calc,
          setParameterPrint: function (data) {
              parameterPrint = data;
          },
          getParameterPrint: function () {
              return parameterPrint;
          },
          post: function (data, callback) {
              $http.post(serviceUrl + '/Post', data).success(callback);
          },
          postQuery: function (data, callback) {
              $http.post(serviceUrl + '/PostQuery', data).success(callback);
          },
          postPrint: function (callback) {
              $http.post(serviceUrl + '/PostPrint', getParameterPrint()).success(callback);
          },
          postPrintDetail: function (callback) {
              $http.post(serviceUrl + '/PostPrintDetail', getParameterPrint()).success(callback);
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
          getCustomer: function (id, callback) {
              $http.get(rootUrl + '/api/Md/Customer/' + id).success(callback);
          },
          postApproval: function (id, callback) {
              $http.post(serviceUrl + '/PostApproval', id).success(callback);
          },
          update: function (params) {
              return $http.put(serviceUrl + '/' + params.id, params);
          },
          getMerchandiseForNvByCode: function (code) {
              return $http.get(rootUrl + '/api/Md/Merchandise/GetForNvByCode/' + code);
          },
          getCustomerForNvByCode: function (code) {
              return $http.get(rootUrl + '/api/Md/Customer/GetForNvByCode/' + code);
          },

      };
      return result;
  }]);
nvModule.controller('phieuXuatBanLeController', [
'$scope', '$rootScope', '$location', '$window', '$uibModal', '$log', '$state', '$filter', '$http',
'phieuXuatBanLeService', 'configService', 'clientService', 'nvService', 'mdService', 'blockUI', 'serviceXuatKhacAndMerchandise',
function (
    $scope, $rootScope, $location, $window, $uibModal, $log, $state, $filter, $http,
phieuXuatBanLeService, configService, clientService, nvService, mdService, blockUI, serviceXuatKhacAndMerchandise
    ) {
    $scope.config = nvService.config;
    $scope.paged = angular.copy(configService.pageDefault);
    $scope.filtered = angular.copy(configService.filterDefault);
    $scope.isEditable = true;
    $scope.setPage = function (pageNo) {
        $scope.paged.currentPage = pageNo;
        filterData();
    };
    $scope.sortType = 'ngayCT'; // set the default sort type
    $scope.sortReverse = false;  // set the default sort order
    $scope.doSearch = function () {
        $scope.paged.currentPage = 1;
        filterData();
    };
    $scope.pageChanged = function () {
        filterData();
    };
    $scope.sum = function () {
        var total = 0;
        if ($scope.data) {
            angular.forEach($scope.data, function (v, k) {
                total = total + v.thanhTienSauVat;
            })
        }
        return total;
    };
    $scope.print = function () {
        var postdata = { paged: $scope.paged, filtered: $scope.filtered };
        phieuXuatBanLeService.setParameterPrint(
            postdata);
        $state.go("nvPrintphieuXuatBanLe");
    }
    $scope.printDetail = function () {
        var postdata = { paged: $scope.paged, filtered: $scope.filtered };
        phieuXuatBanLeService.setParameterPrint(
            postdata);
        $state.go("nvPrintDetailphieuXuatBanLe");
    }
    $scope.goHome = function () {
        $state.go('home');
    };
    $scope.refresh = function () {
        $scope.setPage($scope.paged.currentPage);
    };
    $scope.title = function () {
        return 'Xuất bán lẻ';
    };
    $scope.displayHepler = function (code, module) {
        var data = $filter('filter')(mdService.tempData[module], { value: code }, true);
        if (data && data.length == 1) {
            return data[0].text;
        }
        return "Empty!";
    }
    $scope.formatLabel = function (model, module, displayModel) {
        if (!model) return "";
        var data = $filter('filter')(mdService.tempData[module], { value: model }, true);
        if (data && data.length == 1) {
            displayModel = data[0].text;
            return data[0].text;
        }
        return "Empty!";
    };
    $scope.create = function () {

        //var modalInstance = $uibModal.open({
        //    backdrop: 'static',
        //    templateUrl: nvService.buildUrl('nvXuatBanLe', 'add'),
        //    controller: 'phieuXuatBanLeCreateController',
        //    windowClass: 'app-modal-window',
        //    resolve: {}
        $state.go("nvAddphieuXuatBanLe");
        //});

        //modalInstance.result.then(function (updatedData) {
        //    serviceXuatKhacAndMerchandise.getSelectData().clear();
        //    $scope.refresh();
        //}, function () {
        //    $log.info('Modal dismissed at: ' + new Date());
        //});
    };
    //$scope.update = function (target) {
    //    var modalInstance = $uibModal.open({
    //        backdrop: 'static',
    //        templateUrl: nvService.buildUrl('nvXuatBan', 'update'),
    //        controller: 'phieuXuatBanLeEditController',
    //        windowClass: 'app-modal-window',
    //        resolve: {
    //            targetData: function () {
    //                return target;
    //            }
    //        }
    //    });

    //    modalInstance.result.then(function (updatedData) {
    //        serviceXuatKhacAndMerchandise.getSelectData().clear();
    //        $scope.refresh();
    //    }, function () {
    //        $log.info('Modal dismissed at: ' + new Date());
    //    });
    //};
    $scope.details = function (target) {
        var modalInstance = $uibModal.open({
            backdrop: 'static',
            templateUrl: nvService.buildUrl('nvXuatBanLe', 'details'),
            controller: 'phieuXuatBanLeDetailsController',
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
        phieuXuatBanLeService.postQuery(
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
nvModule.controller('phieuXuatBanLeDetailsController', [
'$scope', '$uibModalInstance',
'mdService', 'phieuXuatBanLeService', 'targetData', 'clientService', '$filter', 'configService',
function ($scope, $uibModalInstance,
    mdService, phieuXuatBanLeService, targetData, clientService, $filter, configService) {
    $scope.paged = angular.copy(configService.pageDefault);
    $scope.config = mdService.config;
    $scope.target = targetData;
    $scope.tempData = mdService.tempData;
    $scope.title = function () {
        return 'Xuất bán lẻ';
    };
    fillterData();
    $scope.formatLabel = function (model, module) {
        if (!model) return "";
        var data = $filter('filter')(mdService.tempData[module], { value: model }, true);
        if (data && data.length == 1) {
            return data[0].text;
        }
        return "Empty!";
    };

    $scope.approval = function () {
        phieuXuatBanLeService.postApproval($scope.target, function (response) {
            if (response) {
                alert("Duyệt thành công!");
                fillterData();
            }
            else { alert("Thất bại! - Xảy ra lỗi hoặc phiếu này đã duyệt"); }
        });
    };

    function fillterData() {
        $scope.isLoading = true;
        phieuXuatBanLeService.getDetails($scope.target.id, function (response) {
            if (response.status) {
                $scope.target = response.data;
            }
            $scope.isLoading = false;
            $scope.pageChanged();
        });
    }
    $scope.pageChanged = function () {
        var currentPage = $scope.paged.currentPage;
        var itemsPerPage = $scope.paged.itemsPerPage;
        $scope.paged.totalItems = $scope.target.dataDetails.length;
        $scope.data = [];
        if ($scope.target.dataDetails) {
            for (var i = (currentPage - 1) * itemsPerPage; i < currentPage * itemsPerPage && i < $scope.target.dataDetails.length; i++) {
                $scope.data.push($scope.target.dataDetails[i])
            }
        }
    }
    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
}
]);
nvModule.controller('phieuXuatBanLeCreateController', [
'$scope', '$rootScope', '$location', '$window', '$uibModal', '$log', '$state', '$filter', '$http',
'phieuXuatBanLeService', 'configService', 'clientService', 'nvService', 'mdService', 'blockUI', 'serviceXuatBanLeAndMerchandise',
function (
    $scope, $rootScope, $location, $window, $uibModal, $log, $state, $filter, $http,
phieuXuatBanLeService, configService, clientService, nvService, mdService, blockUI, serviceXuatBanLeAndMerchandise
    ) {
    $scope.robot = phieuXuatBanLeService.robot;
    $scope.tempData = mdService.tempData;
    $scope.config = nvService.config;

    $scope.tyGia = 0;
    $scope.goHome = function () {
        $state.go('home');
    };
    $scope.title = function () {
        return 'Xuất bán lẻ';
    };
    $scope.target = {};
    $scope.newItem = {};
    $scope.displayHepler = function (code, module) {
        var data = $filter('filter')(mdService.tempData[module], { value: code }, true);
        if (data && data.length == 1) {
            return data[0].text;
        }
        return "Empty!";
    }
    $scope.formatLabel = function (model, module, displayModel) {
        if (!model) return "";
        var data = $filter('filter')(mdService.tempData[module], { value: model }, true);
        if (data && data.length == 1) {
            displayModel = data[0].text;
            return data[0].text;
        }
        return "Empty!";
    };
    $scope.getNewInstance = function () {
        phieuXuatBanLeService.getNewInstance(function (response) {
            $scope.target = response;
            $scope.focusKhachHang = true;
        })
    }
    $scope.addNewItem = function (strKey) {
        var modalInstance = $uibModal.open({
            backdrop: 'static',
            templateUrl: mdService.buildUrl('mdMerchandise', 'selectData'),
            controller: 'merchandiseSelectDataController',
            windowClass: 'app-modal-window',
            resolve: {
                serviceSelectData: function () {
                    return serviceXuatBanLeAndMerchandise;
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
                $scope.target.maHang = updatedData.maHang;
                $scope.newItem = updatedData;

            }
        }, function () {
        });
    }

    $scope.addNewCustomer = function (code) {
        var modalInstance = $uibModal.open({
            backdrop: 'static',
            templateUrl: mdService.buildUrl('mdCustomer', 'selectData'),
            controller: 'customerSelectDataController',
            windowClass: 'app-modal-window',
            resolve: {
                serviceSelectData: function () {
                    return null;
                },
                filterObject: function () {
                    return {
                        advanceData: {
                            maKH: code
                        },
                        isAdvance: true,
                    };
                }
            }
        });
        modalInstance.result.then(function (updatedData) {
            $scope.target.maKhachHang = updatedData.value;
            $scope.target.tenKhachHang = updatedData.text;
            $scope.searchMaKH = "";
            $scope.focusKhachHang = true;
        }, function () {
        });
    }
    $scope.selectedMaHang = function (code) {
        if (code) {
            phieuXuatBanLeService.getMerchandiseForNvByCode(code).then(function (response) {
                $scope.newItem = response.data;
            }, function () {
                $scope.addNewItem(code);
            }
            )
        }
    }
    $scope.selectedMaKhachHang = function (code) {
        if (code) {
            phieuXuatBanLeService.getCustomerForNvByCode(code).then(function (response) {
                $scope.target.maKhachHang = response.data.maKH;
                $scope.target.tenKhachHang = response.data.tenKhachHang;
            }, function () {
                $scope.addNewCustomer(code);
            }
            )
        }
    }
    $scope.createCustomer = function (target, name) {

        var modalInstance = $uibModal.open({
            backdrop: 'static',
            templateUrl: mdService.buildUrl('mdCustomer', 'add'),
            controller: 'customerCreateController',
            resolve: {}
        });

        modalInstance.result.then(function (updatedData) {
            $scope.tempData.update('customers', function () {
                if (target && name) {
                    target[name] = updatedData.maKH;
                }
            });
        }, function () {
            $log.info('Modal dismissed at: ' + new Date());
        });
    };
    $scope.changeSoLuong = function (item) {
        if (!item.giamGia) {
            item.giamGia = 0;
        }
        item.tienGiamGia = item.donGia * (item.giamGia / 100);
        item.thanhTien = item.soLuong * (item.donGia - item.tienGiamGia);
    }
    $scope.changeGiamGia = function (item) {
        item.tienGiamGia = item.donGia * (item.giamGia / 100);
        item.thanhTien = item.soLuong * (item.donGia - item.tienGiamGia);
    }
    $scope.changeTienKhachDua = function (target) {
        target.tienTraLai = target.tienKhachDua - target.thanhTienSauVat;
    }
    $scope.remove = function (index) {
        $scope.target.dataDetails.splice(index, 1);
    }

    $scope.changeChietKhau = function (target) {
        target.tienChietKhau = (target.thanhTienTruocVat + target.tongTienGiamGia) * (target.chietKhau / 100);
        target.thanhTienSauVat = target.thanhTienTruocVat - target.tienChietKhau;
    }
    $scope.addToOrder = function (item) {
        if (!item.maHang) {
            return;
        }
        item.donGia = item.giaBanLe;
        $scope.changeSoLuong(item);
        $scope.target.dataDetails.push(item);

        $scope.newItem = {};
    }

    $scope.thanhToan = function () {
        phieuXuatBanLeService.post(
            JSON.stringify($scope.target), function (response) {
                if (response.status) {
                    printBill($scope.target);
                } else {
                    clientService.noticeAlert(response.message, "danger");
                }
            }
    );
    }
    $scope.cancel = function () {
        $scope.target.dataDetails.clear();
        $uibModalInstance.dismiss('cancel');
    };
    filterData();
    function filterData() {
        $scope.getNewInstance();

        $scope.$watch("target.dataDetails", function (newValue, oldValue) {
            $scope.target.tongTienGiamGia = $scope.robot.sum($scope.target.dataDetails, 'tienGiamGia');
            $scope.target.thanhTienTruocVat = $scope.robot.sum($scope.target.dataDetails, 'thanhTien');
            //$scope.target.tienVat = 0.1 * $scope.target.thanhTienTruocVat;
            $scope.target.thanhTienSauVat = $scope.target.thanhTienTruocVat - $scope.target.tienChietKhau;
        }, true);
    }
    function printBill(target) {
        var table = document.getElementById('bill').innerHTML;
        var myWindow = $window.open('', '', 'width=800, height=600');
        myWindow.document.write(table);
        myWindow.print();
        filterData();
    }
}])
nvModule.controller('reportphieuXuatBanLeController', ['$scope', '$window', '$stateParams', '$timeout', '$state',
'mdService', 'phieuXuatBanLeService', 'clientService',
function ($scope, $window, $stateParams, $timeout, $state,
mdService, phieuXuatBanLeService, clientService) {
    $scope.robot = angular.copy(phieuXuatBanLeService.robot);
    var id = $stateParams.id;
    $scope.target = {};
    $scope.goIndex = function () {
        $state.go('nvXuatBan');
    }
    function filterData() {
        if (id) {
            phieuXuatBanLeService.getReport(id, function (response) {

                if (response.status) {
                    $scope.target = response.data;
                }
            });
        }
    };


    $scope.print = function () {
        var table = document.getElementById('main-report').innerHTML;
        var myWindow = $window.open('', '', 'width=800, height=600');
        myWindow.document.write(table);
        myWindow.print();
    }

    //$scope.$on('$viewContentLoaded', function () {
    //    $scope.$watch('target', function (newVal, oldVal) {

    //        //Force angular not to fire script on load
    //        if (newVal != oldVal) {

    //            //Force script to run AFTER ng-repeat has rendered its things to the DOM
    //            $timeout(function () {

    //                //And finally setTimout to wait for browser to render the custom fonts for print preview
    //                setTimeout(function () {

    //                    //Print document
    //                    $scope.print();
    //                    //window.close();
    //                }, 100);
    //            }, 0);
    //        }
    //    }, true);
    //});
    filterData();
}])
nvModule.controller('printphieuXuatBanLeController', ['$scope', '$state', '$window', '$stateParams', '$timeout', '$filter',
'mdService', 'phieuXuatBanLeService', 'clientService',
function ($scope, $state, $window, $stateParams, $timeout, $filter,
mdService, phieuXuatBanLeService, clientService) {
    $scope.robot = angular.copy(phieuXuatBanLeService.robot);
    $scope.displayHepler = function (code, module) {
        var data = $filter('filter')(mdService.tempData[module], { value: code }, true);
        if (data && data.length == 1) {
            return data[0].text;
        };
        return "Empty!";
    }
    function filterData() {
        phieuXuatBanLeService.postPrint(
            function (response) {
                $scope.printData = response;
            });
    };
    $scope.info = phieuXuatBanLeService.getParameterPrint().filtered.advanceData;
    $scope.goIndex = function () {
        $state.go("nvXuatBanLe");
    }
    $scope.printExcel = function () {
        var data = [document.getElementById('dataTable').innerHTML];
        clientService.saveExcel(data, "Danh_sach");
    }
    $scope.sum = function () {
        var total = 0;
        if ($scope.printData) {
            angular.forEach($scope.printData, function (v, k) {
                total = total + v.thanhTienSauVat;
            })
        }
        return total;
    }
    $scope.print = function () {
        var table = document.getElementById('dataTable').innerHTML;
        var myWindow = $window.open('', '', 'width=800, height=600');
        myWindow.document.write(table);
        myWindow.print();
    }
    filterData();
}])
nvModule.controller('printDetailphieuXuatBanLeController', ['$scope', '$state', '$window', '$stateParams', '$timeout', '$filter',
'mdService', 'phieuXuatBanLeService', 'clientService',
function ($scope, $state, $window, $stateParams, $timeout, $filter,
mdService, phieuXuatBanLeService, clientService) {
    $scope.robot = angular.copy(phieuXuatBanLeService.robot);
    $scope.displayHepler = function (code, module) {
        var data = $filter('filter')(mdService.tempData[module], { value: code }, true);
        if (data && data.length == 1) {
            return data[0].text;
        };
        return "Empty!";
    }
    $scope.info = phieuXuatBanLeService.getParameterPrint().filtered.advanceData;
    $scope.goIndex = function () {
        $state.go("nvXuatBanLe");
    }
    function filterData() {
        phieuXuatBanLeService.postPrintDetail(
            function (response) {
                $scope.printData = response;
            });
    }
    $scope.sum = function () {
        var total = 0;
        if ($scope.printData) {
            angular.forEach($scope.printData, function (v, k) {
                total = total + v.thanhTienSauVat;
            })
        }
        return total;
    }
    $scope.printExcel = function () {
        var data = [document.getElementById('dataTable').innerHTML];
        clientService.saveExcel(data, "Danh_sach_chi_tiet");
    }
    $scope.print = function () {
        var table = document.getElementById('dataTable').innerHTML;
        var myWindow = $window.open('', '', 'width=800, height=600');
        myWindow.document.write(table);
        myWindow.print();
    }
    filterData();

}])