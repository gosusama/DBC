/*  
* Người tạo : Nguyễn Tuấn Hoàng Anh
* View: BTS.SP.MART/views/nv/NvKiemKe
* Vm sevices: BTS.API.SERVICE -> NV ->NvKiemKeVm.cs
* Sevices: BTS.API.SERVICE -> NV -> NvKiemKeService.cs
* Entity: BTS.API.ENTITY -> NV - > NvKiemKe.cs
* Menu: Nghiệp vụ-> Kiểm kê hàng hóa
*/
define(['ui-bootstrap', '/BTS.SP.MART/controllers/auth/AuthController.js', '/BTS.SP.MART/controllers/htdm/periodController.js', '/BTS.SP.MART/controllers/htdm/merchandiseController.js', '/BTS.SP.MART/controllers/htdm/customerController.js', '/BTS.SP.MART/controllers/htdm/merchandiseTypeController.js', '/BTS.SP.MART/controllers/htdm/nhomVatTuController.js', '/BTS.SP.MART/controllers/htdm/supplierController.js', '/BTS.SP.MART/controllers/htdm/wareHouseController.js', '/BTS.SP.MART/controllers/htdm/packagingController.js', '/BTS.SP.MART/controllers/htdm/taxController.js', '/BTS.SP.MART/controllers/htdm/donViTinhController.js', '/BTS.SP.MART/controllers/htdm/shelvesController.js'], function () {
    'use strict';
    var app = angular.module('kiemKeModule', ['ui.bootstrap', 'authModule', 'periodModule', 'merchandiseModule', 'customerModule', 'merchandiseTypeModule', 'nhomVatTuModule', 'supplierModule', 'wareHouseModule', 'packagingModule', 'taxModule', 'donViTinhModule', 'shelvesModule']);
    app.factory('kiemKeService', ['$http', 'configService', function ($http, configService) {
        var serviceUrl = configService.rootUrlWebApi + '/NV/KiemKe';
        var rootUrl = configService.apiServiceBaseUri;
        var result = {
            getMerchandiseForActionInventory: function (maKho) {
                return $http.get(rootUrl + '/api/Md/Merchandise/GetMerchandiseForActionInventory/' + maKho);
            },
            getExternalCode: function (data) {
                return $http.post(serviceUrl + '/GetExternalCode', data);
            },
            post: function (data) {
                return $http.post(serviceUrl + '/Post', data);
            },
            updatePhieuKiemKe: function (params) {
                return $http.put(serviceUrl + '/' + params.id, params);
            },
            postQuery: function (data) {
                return $http.post(serviceUrl + '/PostQuery', data);
            },
            getDetails: function (id) {
                return $http.get(serviceUrl + '/GetDetails/' + id);
            },
            getDataDetails: function (id) {
                return $http.get(serviceUrl + '/GetDataKiemKe/' + id);
            },
            getMerchandiseByCode: function (maHang) {
                return $http.get(serviceUrl + '/GetMerchandiseByCodeKK/' + maHang);
            },
            postComplete: function (data) {
                return $http.post(serviceUrl + '/PostComplete', data);
            },
            searchMerchandise: function (code) {
                return $http.get(serviceUrl + '/GetInfoMerchandiseByCode/' + code);
            },
            deleteItem: function (params) {
                return $http.delete(serviceUrl + '/' + params.id, params);
            },
            postExportExcel: function (json) {
                return $http({
                    url: serviceUrl + '/PostExportExcel',
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
                    a.download = "HangChuaKiemKe.xlsx";
                    a.click();
                }).error(function (data, status, headers, config) {
                    //upload failed
                });
            }

        };
        return result;
    }]);
    /* controller list */
    app.controller('kiemKeController', [
        '$scope', '$location', '$http', 'configService', 'kiemKeService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'securityService', '$rootScope', 'toaster', 'periodService', 'merchandiseService', 'customerService', 'merchandiseTypeService', 'nhomVatTuService', 'supplierService', 'wareHouseService', 'packagingService', 'taxService', 'donViTinhService', 'shelvesService', 'FileUploader', 'userService',
        function ($scope, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, securityService, $rootScope, toaster, servicePeriod, serviceMerchandise, serviceCustomer, serviceMerchandiseType, serviceNhomVatTu, serviceSupplier, serviceWareHouse, servicePackaging, serviceTax, serviceDonViTinh, serviceShelves, FileUploader, serviceAuthUser) {
            var currentUser = serviceAuthUser.GetCurrentUser();
            var unitCode = currentUser.unitCode;
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.filtered = angular.copy(configService.filterDefault);
            $scope.tempData = tempDataService.tempData;
            $scope.isEditable = true;
            $scope.accessList = {};
            $scope.target = {};
            $scope.target.maKho = unitCode + '-K2';
            var serviceUrl = configService.rootUrlWebApi + '/NV/KiemKe';
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
                    });
                }
            };
            //end

            //check quyền truy cập
            function loadAccessList() {
                securityService.getAccessList('nvKiemKe').then(function (successRes) {
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
            //load danh muc
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
            function loadShelves() {
                if (!tempDataService.tempData('shelves')) {
                    serviceShelves.getAll_Shelves().then(function (successRes) {
                        if (successRes && successRes.status === 200 && successRes.data.data.length > 0) {
                            tempDataService.putTempData('shelves', successRes.data.data);
                            $scope.shelves = successRes.data.data;
                        } else {
                            console.log('successRes', successRes);
                        }
                    }, function (errorRes) {
                        console.log('errorRes', errorRes);
                    });
                } else {
                    $scope.shelves = tempDataService.tempData('shelves');
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

            loadCustomer();
            loadSupplier();
            loadMerchandiseType();
            loadNhomVatTu();
            loadWareHouse();
            loadPackagings();
            loadTax();
            loadDonViTinh();
            loadShelves();

            $scope.filtered.advanceData.tagWareHouses = [];
            $scope.filtered.advanceData.tagNhaCungCaps = [];
            $scope.filtered.advanceData.tagMerchandiseTypes = [];
            $scope.filtered.advanceData.tagMerchandises = [];
            $scope.filtered.advanceData.tagMerchandiseGroups = [];

            $scope.setPage = function (pageNo) {
                $scope.paged.currentPage = pageNo;
                filterData();
            };
            $scope.sortType = 'maChungTu';
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
            $scope.title = function () { return 'Kiểm kê hàng hóa'; };
            $scope.downLoadXML = function () {
                service.getMerchandiseForActionInventory($scope.target.maKho).then(function (response) {
                    if (response && response.status == 200 && response.data) {
                        $scope.toJSON = '';
                        $scope.toJSON = angular.toJson($scope.data);
                        var blob = new Blob([response.data], { type: "application/xml" });
                        var downloadLink = angular.element('<a></a>');
                        downloadLink.attr('href', window.URL.createObjectURL(blob));
                        downloadLink.attr('download', 'Data.xml');
                        downloadLink[0].click();
                    }
                });
            };
            $scope.externalCode = function () {
                if ($scope.target.maKho) {
                    var modalInstance = $uibModal.open({
                        backdrop: 'static',
                        templateUrl: configService.buildUrl('nv/NvKiemKe', 'externalCode'),
                        controller: 'kiemKeExternalCodeController',
                        windowClass: 'app-modal-window',
                        resolve: {
                            targetData: function () {
                                return $scope.target.maKho;
                            }
                        }
                    });
                    modalInstance.result.then(function (updatedData) {
                        $scope.refresh();
                    }, function () {
                        $log.info('Modal dismissed at: ' + new Date());
                    });
                } else {
                    clientService.noticeAlert('Chưa chọn kho', "danger");
                }
            };
            /* Function add New Item */
            $scope.create = function () {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    size: 'md',
                    templateUrl: configService.buildUrl('nv/NvKiemKe', 'receiveData'),
                    controller: 'receiveDataKiemKeController',
                    windowClass: 'app-modal-window',
                    resolve: {
                        targetData: function () {
                            return $scope.target.maKho;
                        }
                    }
                });
                modalInstance.result.then(function (updatedData) {
                    $uibModalInstance.close($scope.target);
                    $scope.refresh();
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };

            /* Function Edit Item */
            $scope.update = function (target) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('nv/NvKiemKe', 'update'),
                    controller: 'kiemKeEditController',
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
                    templateUrl: configService.buildUrl('nv/NvKiemKe', 'details'),
                    controller: 'kiemKeDetailsController',
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
            var uploader = $scope.uploader = new FileUploader({
                url: serviceUrl + '/UploadFile'
            });
            uploader.filters.push({
                name: 'syncFilter',
                fn: function (item, options) {
                    return this.queue.length < 10;
                }
            });
            uploader.filters.push({
                name: 'asyncFilter',
                fn: function (item, options, deferred) {
                    setTimeout(deferred.resolve, 1e3);
                }
            });
            /* Function Delete Item */
            $scope.delete = function (event, target) {
                var modalInstance = $uibModal.open({
                    backdrop: 'static',
                    templateUrl: configService.buildUrl('nv/NvKiemKe', 'delete'),
                    controller: 'kiemKeDeleteController',
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
    app.controller('receiveDataKiemKeController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'kiemKeService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', '$rootScope', 'userService', 'FileUploader', 'merchandiseService', 'customerService', 'merchandiseTypeService', 'nhomVatTuService', 'supplierService', 'wareHouseService', 'packagingService', 'taxService', 'donViTinhService', 'periodService', 'targetData',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, $rootScope, serviceAuthUser, FileUploader, serviceMerchandise, serviceCustomer, serviceMerchandiseType, serviceNhomVatTu, serviceSupplier, serviceWareHouse, servicePackaging, serviceTax, serviceDonViTinh, servicePeriod, targetData) {
            var currentUser = serviceAuthUser.GetCurrentUser();
            var unitCode = currentUser.unitCode;
            var maNhanVien = currentUser.userName;
            var rootUrl = configService.apiServiceBaseUri;
            var serviceUrl = rootUrl + '/api/Nv/KiemKe';
            var uploader = $scope.uploader = new FileUploader({
                url: serviceUrl + '/ReceiveDataKiemKe/' + targetData
            });
            uploader.filters.push({
                name: 'syncFilter',
                fn: function (item, options) {
                    return this.queue.length < 10;
                }
            });
            uploader.filters.push({
                name: 'asyncFilter',
                fn: function (item, options, deferred) {
                    setTimeout(deferred.resolve, 1e3);
                }
            });
            uploader.onSuccessItem = function (fileItem, response, status, headers) {
                if (status == 200) {
                    $scope.lstVatTu = response;
                    var modalInstance = $uibModal.open({
                        backdrop: 'static',
                        templateUrl: configService.buildUrl('nv/NvKiemKe', 'repKiemKeLuuTam'),
                        controller: 'repKiemKeLuuTamController',
                        windowClass: 'app-modal-window',
                        resolve: {
                            targetData: function () {
                                return $scope.lstVatTu;
                            }
                        }
                    });
                }
            };
            $scope.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };
        }]);
    /* controller Edit */
    app.controller('kiemKeEditController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'kiemKeService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify', 'merchandiseService',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify, serviceMerchandise) {
            $scope.config = angular.copy(configService);
            $scope.robot = angular.copy(service.robot);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.targetData = angular.copy(targetData);
            $scope.tempData = tempDataService.tempData;
            $scope.target = targetData;
            $scope.newItem = {};
            $scope.searchItem = {};
            $scope.isLoading = true;
            $scope.title = function () { return 'Cập nhật dữ liệu kiểm kê'; };
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
                service.getDetails(targetData.id).then(function (response) {
                    if (response && response.status === 200 && response.data) {
                        $scope.target = response.data.data;
                    }
                });

            };
            filterData();
            $scope.selectedMaHang = function (code) {
                if (code) {
                    service.getMerchandiseByCode(code).then(function (response) {
                        if (response && response.status == 200 && response.data && response.data.status) {
                            $scope.newItem = response.data.dataDetails[0];
                        }
                    });
                }
            };
            $scope.displayHepler = function (paraValue, moduleName) {
                var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                if (data && data.length === 1) {
                    return data[0].text;
                } else {
                    return paraValue;
                }
            }
            $scope.addRow = function () {
                var exsist = $scope.target.dataDetails.some(function (element, index, array) {
                    return $scope.newItem.maVatTu === element.maVatTu;
                });
                if (exsist) {
                    toaster.pop('success', "Thông báo:", "Mã hàng này bạn đã nhập rồi");
                    angular.forEach($scope.target.dataDetails, function (v, k) {
                        if (v.maVatTu === $scope.newItem.maVatTu) {

                        }
                    });
                } else {
                    $scope.target.dataDetails.push($scope.newItem);
                }

                $scope.pageChanged();
                $scope.newItem = {};
                focus('maHang');
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
                        updatedData.donGia = updatedData.giaMua;
                        $scope.newItem = updatedData;
                        $scope.newItem.validateCode = updatedData.maHang;
                        $scope.newItem.giaMuaCoVat = updatedData.giaMua * (1 + updatedData.tyLeVatVao / 100);
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
            };
            $scope.removeMaHang = function (index) {
                var currentPage = $scope.paged.currentPage;
                var itemsPerPage = $scope.paged.itemsPerPage;
                var currentPageIndex = (currentPage - 1) * itemsPerPage + index;
                $scope.target.dataDetails.splice(index, 1);
                if ($scope.target.dataDetails.length === 0) {
                    $scope.isListItemNull = true;
                }
                $scope.pageChanged();
            };
            $scope.modifiedKeHang = function (index) {
                alert('Cap nhat ma ke hang : ' + $scope.target.dataDetails[index].maVatTu);
            }
            $scope.save = function () {
                service.updatePhieuKiemKe($scope.target).then(function (successRes) {
                    if (successRes && successRes.status === 200 && successRes.data) {
                        ngNotify.set("Cập nhật thành công", { type: 'success' });
                        $uibModalInstance.close($scope.target);
                    } else {
                        console.log('update successRes', successRes);
                        ngNotify.set(successRes.data.message, { duration: 3000, type: 'error' });
                    }
                },
                    function (response) {
                        console.log('ERROR: Update failed! ' + response);
                    }
                );
            };
            $scope.cancel = function () {
                $scope.target.dataDetails = [];
                $uibModalInstance.close();
            };

        }]);


    /* controller Details */
    app.controller('kiemKeDetailsController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'kiemKeService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify) {
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.tempData = tempDataService.tempData;
            $scope.target = {};
            $scope.isLoading = false;
            $scope.title = function () { return 'Chi tiết phiếu kiểm kê'; };
            $scope.pageChanged = function () {
                var currentPage = $scope.paged.currentPage;
                var itemsPerPage = $scope.paged.itemsPerPage;
                $scope.paged.totalItems = $scope.target.dataDetails.length;
                $scope.data = [];
                if ($scope.target.dataDetails) {
                    for (var i = (currentPage - 1) * itemsPerPage; i < currentPage * itemsPerPage && i < $scope.target.dataDetails.length; i++) {
                        $scope.target.dataDetails[i].thanhTienVAT = $scope.target.dataDetails[i].thanhTien * (1 + $scope.target.dataDetails[i].tyLeVatVao / 100);
                        $scope.target.dataDetails[i].giaMuaCoVat = $scope.target.dataDetails[i].giaMua * (1 + $scope.target.dataDetails[i].tyLeVatVao / 100);
                        $scope.data.push($scope.target.dataDetails[i]);
                    }
                }
            }
            //note
            function fillterData() {
                $scope.isLoading = true;
                service.getDetails(targetData.id).then(function (resgetDetails) {
                    if (resgetDetails && resgetDetails.status == 200 && resgetDetails.data) {
                        $scope.target = resgetDetails.data.data;
                        $scope.target.ngayCT = new Date($scope.target.ngayCT);
                        $scope.target.ngayHoaDon = new Date($scope.target.ngayHoaDon);
                        $scope.target.ngayKiemKe = new Date($scope.target.ngayKiemKe);
                    }
                    $scope.isLoading = false;
                    $scope.pageChanged();
                });
            }
            $scope.formatLabel = function (paraValue, moduleName) {
                var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                if (data && data.length === 1) {
                    return data[0].text;
                } else {
                    return paraValue;
                }
            }
            fillterData();
            $scope.approval = function () {
                $scope.isDisabled = true;
                $scope.stateIsRunning = true;
                service.postComplete($scope.target).then(function (response) {
                    if (response && response.status == 200 && response.data) {
                        alert("Duyệt thành công!");
                        $scope.isDisabled = false;
                        $uibModalInstance.close(response.data);
                        $scope.refresh();
                    } else {
                        $scope.isDisabled = false;
                        alert("Thất bại! - Xảy ra lỗi hoặc phiếu này đã duyệt");
                    }
                    $scope.stateIsRunning = false;
                }).error(function (error) {
                    $scope.isDisabled = false;
                    $scope.stateIsRunning = false;
                });;
            };
            $scope.cancel = function () {
                $uibModalInstance.close();
            };

        }]);
    /* controller delete */
    app.controller('kiemKeDeleteController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'kiemKeService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify',
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

    /* report Phieu Nhap Hang Mua Controller */
    app.controller('reportkiemKeController', ['$scope', '$location', '$http', 'configService', 'kiemKeService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', '$stateParams', 'userService','$window',
        function ($scope, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, $stateParams, serviceAuthUser, $window) {
            $scope.robot = angular.copy(service.robot);
            $scope.config = angular.copy(configService);
            $scope.typeReportKiemKe = [
             {
                 value: '1', text: 'Hàng đủ'
             },
             {
                 value: '2', text: "Hàng thừa"
             },
             {
                 value: '3', text: "Hàng thiếu"
             }

            ];
            function filterData(option) {
                $scope.para = id + '-' + option;
                if (id) {
                    service.getDataDetails($scope.para).then(function (response) {
                        if (response && response.status === 200 && response.data) {
                            $scope.lstKiemKe = response.data.data;
                            $scope.lstKiemKe.ngayIn = new Date();
                            $scope.sumSoLuongMay = sumSoLuongTonMay($scope.lstKiemKe.dataDetails);
                            $scope.sumKiemKe = sumSoLuongKiemKe($scope.lstKiemKe.dataDetails);
                            $scope.sumGiaVon = sumGiaVon($scope.lstKiemKe.dataDetails);
                            $scope.thanhTienThua = sumThanhTienThua($scope.lstKiemKe.dataDetails);
                            $scope.thanhTienThieu = sumThanhTienThieu($scope.lstKiemKe.dataDetails);
                        }
                    });

                }
            };


            var currentUser = serviceAuthUser.GetCurrentUser();
            var unitCode = currentUser.unitCode;
            $scope.currentUser = currentUser.userName;
            $scope.target = {};
            var id = $stateParams.id;
            $scope.target = {};
            $scope.goIndex = function () {
                $state.go('nvKiemKe');
            }
            $scope.selectAction = function (option) {
                console.log(option);
                filterData(option);
            };
            function sumSoLuongTonMay(lst) {
                var result = 0;
                for (var i = 0; i < lst.length; i++) {
                    result += lst[i].soLuongTonMay;
                }
                return result;
            };
            function sumSoLuongKiemKe(lst) {
                var result = 0;
                for (var i = 0; i < lst.length; i++) {
                    result += lst[i].soLuongKiemKe;
                }
                return result;
            };

            function sumGiaVon(lst) {
                var result = 0;
                for (var i = 0; i < lst.length; i++) {
                    result += lst[i].giaVon;
                }
                return result;
            };

            function sumThanhTienThua(lst) {
                var result = 0;
                for (var i = 0; i < lst.length; i++) {
                    var ttThua = (lst[i].soLuongKiemKe - lst[i].soLuongTonMay) * lst[i].giaVon;
                    result += ttThua;
                }
                return result;
            };

            function sumThanhTienThieu(lst) {
                var result = 0;
                for (var i = 0; i < lst.length; i++) {
                    var ttThieu = (lst[i].soLuongTonMay - lst[i].soLuongKiemKe) * lst[i].giaVon;
                    result += ttThieu;
                }
                return result;
            };

            $scope.displayHepler = function (paraValue, moduleName) {
                var data = $filter('filter')($scope.tempData(moduleName), { value: paraValue }, true);
                if (data && data.length === 1) {
                    return data[0].text;
                } else {
                    return paraValue;
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
                var fileName = "PhieuKiemKe.xls";
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
                    var url = window.webkitURL.createObjectURL(new Blob(data, { type: filetype }));
                    a.attr("href", url);
                    a.attr("download", fileName);
                    $("body").append(a);
                    a[0].click();
                    window.url.revokeObjectURL(url);
                    a.remove();
                }
            };
            $scope.myOption = '3';
            filterData($scope.myOption);
        }]);
    app.controller('repKiemKeLuuTamController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'kiemKeService', 'tempDataService', '$filter', '$uibModal', '$log', 'targetData', 'ngNotify',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, targetData, ngNotify) {
            $scope.robot = angular.copy(service.robot);
            $scope.lstHangHoa = [];
            $scope.target = targetData;
            $scope.lstHangHoa = targetData;
            $scope.config = angular.copy(configService);
            $scope.title = function () { return ''; };
            $scope.save = function () {
                service.post($scope.lstHangHoa).then(function (response) {
                    if (response && response.status === 200 && response.data) {
                        ngNotify.set("Thành công", { type: 'success' });
                        $scope.lstHangHoa.dataDetails.clear();
                        $uibModalInstance.close($scope.target);
                    } else {

                    }
                }
                    );
            };
            $scope.print = function () {
                var table = document.getElementById('main-report').innerHTML;
                var myWindow = $window.open('', '', 'width=800, height=600');
                myWindow.document.write(table);
                myWindow.print();
            }
            $scope.cancel = function () {
                $uibModalInstance.close();
            };
        }]);
    app.controller('kiemKeExternalCodeController', ['$scope', '$uibModalInstance', '$location', '$http', 'configService', 'kiemKeService', 'tempDataService', '$filter', '$uibModal', '$log', 'ngNotify', 'targetData',
        function ($scope, $uibModalInstance, $location, $http, configService, service, tempDataService, $filter, $uibModal, $log, ngNotify, targetData) {
            $scope.robot = angular.copy(service.robot);
            $scope.config = angular.copy(configService);
            $scope.paged = angular.copy(configService.pageDefault);
            $scope.target = {};
            $scope.lstTmp = [];
            $scope.target.count = 0;
            $scope.target.maKho = targetData;
            $scope.target.wareHouseCodes = targetData;
            $scope.title = function () { return 'Mã hàng chưa kiểm kê'; };
            $scope.title = function () { return ''; };
            $scope.doSearch = function () {
                $scope.paged.currentPage = 1;
                $scope.target.merchandiseCodes = $scope.target.merchandiseCodes;
                $scope.target.nhaCungCapCodes = $scope.target.nhaCungCapCodes;
                $scope.target.merchandiseTypeCodes = $scope.target.merchandiseTypeCodes;
                $scope.target.keHangCodes = $scope.target.keHangCodes;
                $scope.target.merchandiseGroupCodes = $scope.target.merchandiseGroupCodes;
                service.getExternalCode($scope.target).then(function (response) {
                    if (response && response.status == 200 && response.data) {
                        $scope.lstExternalCode = response.data;
                        $scope.target.count = $scope.lstExternalCode.length;
                        $scope.lstTmp = response.data;
                    }
                    $scope.pageChanged();
                })
            }
            $scope.refresh = function () {
                $scope.setPage($scope.paged.currentPage);
            };
            $scope.pageChanged = function () {
                var currentPage = $scope.paged.currentPage;
                var itemsPerPage = $scope.paged.itemsPerPage;
                $scope.paged.totalItems = $scope.lstExternalCode.length;
                $scope.data = [];
                if ($scope.lstExternalCode) {
                    for (var i = (currentPage - 1) * itemsPerPage; i < currentPage * itemsPerPage && i < $scope.lstExternalCode.length; i++) {
                        $scope.data.push($scope.lstExternalCode[i]);
                    }
                }
            };
            service.getExternalCode($scope.target).then(function (response) {
                if (response && response.status == 200 && response.data) {
                    $scope.lstExternalCode = response.data;
                    $scope.target.count = $scope.lstExternalCode.length;
                    $scope.lstTmp = response.data;
                }
                $scope.pageChanged();
            });
            function convertToArrayCondition(records) {
                var result = '';
                if (records) {
                    if (records.indexOf(',') !== -1) {
                        var arr = records.split(',');
                        if (arr && arr.length > 0) {
                            angular.forEach(arr, function (value, key) {
                                result = result + '\'' + value + '\'' + ',';
                            });
                            result = result.substr(0, result.length - 1);
                        }
                    } else {
                        result = '\'' + records + '\'';
                    }
                }
                return result;
            };
            $scope.exportData = function () {
                service.postExportExcel($scope.lstTmp);
            }
            $scope.cancel = function () {
                $uibModalInstance.close();
            };
        }]);
    return app;
});

