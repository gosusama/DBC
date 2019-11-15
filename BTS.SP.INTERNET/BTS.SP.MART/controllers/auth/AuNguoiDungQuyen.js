/*  
* Người tạo : Nguyễn Tuấn Hoàng Anh
* View: BTS.SP.MART/views/auth/AuNguoiDungQuyen
* Menu: Danh mục-> Au người dùng quyền
*/
define(['ui-bootstrap'], function () {
    'use strict';
    var app = angular.module('AuNguoiDungQuyenModule', ['ui.bootstrap']);
    app.factory('AuNguoiDungQuyenService', ['$http', 'configService', function ($http, configService) {
        var serviceUrl = configService.rootUrlWebApi + '/Authorize/AuNguoiDungQuyen';
        var result = {
            config: function (data) {
                return $http.post(serviceUrl + '/Config', data);
            },
            getByUsername: function (data) {
                return $http.get(serviceUrl + '/GetByUsername/' + data);
            }
        }
        return result;
    }]);
    /* controller list */
    app.controller('AuNguoiDungQuyenCreateCtrl', ['$scope', '$uibModalInstance', 'configService', 'AuNguoiDungQuyenService', '$filter', 'toaster', 'AuMenuService', 'targetData',
        function ($scope, $uibModalInstance, configService, service, $filter, toaster, serviceAuMenu, targetData) {
            $scope.config = {
                label: angular.copy(configService.label)
            };
            $scope.title = "Thêm mới phân quyền người dùng";
            $scope.data = [];
            $scope.lstChucNang = [];
            $scope.lstAdd = [];
            $scope.lstEdit = [];
            $scope.lstDelete = [];
            $scope.changeShowAdd = function (node) {
                if (node.isOpenAdd) {
                    node.isOpenAdd = false;
                } else {
                    node.isOpenAdd = true;
                }
            }
            $scope.changeShowUpdate = function (node) {
                if (node.isOpenUpdate) {
                    node.isOpenUpdate = false;
                } else {
                    node.isOpenUpdate = true;
                }
            }
            function treeify(list, idAttr, parentAttr, childrenAttr) {
                if (!idAttr) idAttr = 'menuId';
                if (!parentAttr) parentAttr = 'menuIdCha';
                if (!childrenAttr) childrenAttr = 'children';
                var lookup = {};
                var result = {};
                result[childrenAttr] = [];
                list.forEach(function (obj) {
                    lookup[obj[idAttr]] = obj;
                    obj[childrenAttr] = [];
                });
                list.forEach(function (obj) {
                    if (obj[parentAttr] != null) {
                        try { lookup[obj[parentAttr]][childrenAttr].push(obj); }
                        catch (err) {
                            result[childrenAttr].push(obj);
                        }

                    } else {
                        result[childrenAttr].push(obj);
                    }
                });
                return result;
            };
            function loadQuyen() {
                service.getByUsername(targetData.username).then(function (successRes) {
                    if (successRes && successRes.status == 200) {
                        $scope.data = successRes.data.data;
                        $scope.data_update = angular.copy($scope.data);
                        $scope.lstEdit = angular.copy($scope.data);
                        return $scope.data;
                    } else {
                        toaster.pop('error', "Lỗi:", successRes.data.message);
                        return null;
                    }
                }, function (errorRes) {
                    console.log(errorRes);
                    toaster.pop('error', "Lỗi:", errorRes.statusText);
                }).then(function (data) {
                    serviceAuMenu.getAllForConfigQuyen(targetData.username).then(function (successRes) {
                        if (successRes && successRes.status == 200) {
                            $scope.lstChucNang = successRes.data.data;
                            if ($scope.lstChucNang) {
                                $scope.lstChucNang.forEach(function (obj) {
                                    if (obj.menuId == 'merchandise' || obj.menuId == 'NghiepVu' || obj.menuIdCha == 'NghiepVu' || obj.menuIdCha == 'BaoCao' || obj.menuId == 'BaoCao' || obj.menuIdCha == 'ThanhToanCongNo' || obj.menuId == 'ThanhToanCongNo') {
                                        obj.checkState = true;
                                    } else {
                                        obj.checkState = false;
                                    }
                                });
                            }
                            if ($scope.lstChucNang && $scope.lstChucNang.length > 0) {
                                $scope.data = treeify($scope.lstChucNang).children;
                            } else {
                                $scope.data = [];
                            }
                        } else {
                            toaster.pop('error', "Lỗi:", successRes.data.message);
                        }
                    }, function (errorRes) {
                        console.log(errorRes);
                        toaster.pop('error', "Lỗi:", errorRes.statusText);
                    });
                });
            }
            loadQuyen();
            $scope.collapseAll = function () {
                $scope.$broadcast('angular-ui-tree:collapse-all');
            };
            $scope.expandAll = function () {
                $scope.$broadcast('angular-ui-tree:expand-all');
            };
            $scope.selectChucNang = function (item) {
                if (item.menuIdCha != '') {
                    var parent = {};
                    if (item.menuIdCha && $scope.data.length > 0) {
                        var nodeParent = $filter('filter')($scope.data, { menuId: item.menuIdCha }, true);
                        if (nodeParent && nodeParent.length === 1) {
                            parent = nodeParent[0];
                        }
                    }
                    var existParent = $filter('filter')($scope.lstAdd, { menuId: item.menuIdCha }, true);
                    if (!existParent || existParent.length !== 1) {
                        var objParent = {
                            machucnang: parent.menuId,
                            tenchucnang: parent.title,
                            sothutu: parent.sort,
                            state: parent.menuId,
                            xem: true,
                            them: false,
                            sua: false,
                            xoa: false,
                            duyet: false,
                            giamua: false,
                            giaban: false,
                            giavon: false,
                            tylelai: false,
                            banchietkhau: false,
                            banbuon: false,
                            bantralai: false,
                            username: targetData.username,
                            menuId: parent.menuId,
                            menuIdCha: parent.menuIdCha,
                            sort: parent.sort,
                            title: parent.title,
                            children: [],
                            checkState: item.checkState,
                        }
                        $scope.lstAdd.push(objParent);
                    }

                    var obj = {
                        machucnang: item.menuId,
                        tenchucnang: item.title,
                        sothutu: item.sort,
                        state: item.menuId,
                        xem: true,
                        them: false,
                        sua: false,
                        xoa: false,
                        duyet: false,
                        giamua: false,
                        giaban: false,
                        giavon: false,
                        tylelai: false,
                        banchietkhau: false,
                        banbuon: false,
                        bantralai: false,
                        username: targetData.username,
                        menuId: item.menuId,
                        menuIdCha: item.menuIdCha,
                        sort: item.sort,
                        title: item.title,
                        children: [],
                        checkState: item.checkState,
                    }
                    if (item.menuId === 'merchandise' || item.menuIdCha === 'NghiepVu' || item.menuIdCha === 'KhuyenMai' || item.menuIdCha === 'BaoCao') {
                        obj.checkState = true;
                    } else {
                        obj.checkState = false;
                    }
                    var existNodeParent = $filter('filter')($scope.lstAdd, { menuId: item.menuIdCha }, true);
                    if (existNodeParent && existNodeParent.length === 1) {
                        var existNodeAdd = $filter('filter')(existNodeParent[0].children, { menuId: item.menuId }, true);
                        if (existNodeAdd && existNodeAdd.length === 1) {
                            console.log('exist');
                        } else {
                            $scope.lstAdd.push(obj);
                        }
                    }
                    var filteredData = $filter('filter')($scope.lstChucNang, { menuId: item.menuId }, true);
                    if (filteredData && filteredData.length > 0) {
                        var index = $scope.lstChucNang.indexOf(filteredData[0]);
                        if (index != -1) $scope.lstChucNang.splice(index, 1);
                    }
                    $scope.data = treeify($scope.lstChucNang).children;
                    $scope.lstAdd = $filter('orderBy')($scope.lstAdd, 'sort', false);
                    $scope.data_add = treeify($scope.lstAdd).children;
                } else {
                    console.log('parent');
                    var existParent = $filter('filter')($scope.lstAdd, { menuId: item.menuId }, true);
                    if (existParent && existParent.length === 1) {
                        var allNodeParent = $filter('filter')($scope.data, { menuId: item.menuId }, true);
                        if (allNodeParent && allNodeParent.length === 1) {
                            angular.forEach(allNodeParent[0].children, function (item, key) {
                                var checkIsExist = $filter('filter')($scope.lstAdd, { menuId: item.menuId }, true);
                                if (checkIsExist && checkIsExist.length === 1) {
                                    console.log('exist');
                                } else {
                                    var objNode = {
                                        machucnang: item.menuId,
                                        tenchucnang: item.title,
                                        sothutu: item.sort,
                                        state: item.menuId,
                                        xem: true,
                                        them: false,
                                        sua: false,
                                        xoa: false,
                                        duyet: false,
                                        giamua: false,
                                        giaban: false,
                                        giavon: false,
                                        tylelai: false,
                                        banchietkhau: false,
                                        banbuon: false,
                                        bantralai: false,
                                        username: targetData.username,
                                        menuId: item.menuId,
                                        menuIdCha: item.menuIdCha,
                                        sort: item.sort,
                                        title: item.title,
                                        children: [],
                                        checkState: item.checkState,
                                    }
                                    $scope.lstAdd.push(objNode);
                                }
                                $scope.lstChucNang.splice(key + 1, 1);
                            });
                            $scope.lstAdd = $filter('orderBy')($scope.lstAdd, 'sort', false);
                            $scope.data_add = treeify($scope.lstAdd).children;
                            $scope.data = treeify($scope.lstChucNang).children;
                        }
                    } else {
                        var objParent = {
                            machucnang: item.menuId,
                            tenchucnang: item.title,
                            sothutu: item.sort,
                            state: item.menuId,
                            xem: true,
                            them: false,
                            sua: false,
                            xoa: false,
                            duyet: false,
                            giamua: false,
                            giaban: false,
                            giavon: false,
                            tylelai: false,
                            banchietkhau: false,
                            banbuon: false,
                            bantralai: false,
                            username: targetData.username,
                            menuId: item.menuId,
                            menuIdCha: item.menuIdCha,
                            sort: item.sort,
                            title: item.title,
                            children: [],
                            checkState: item.checkState,
                        }
                        $scope.lstAdd.push(objParent);
                        $scope.lstAdd = $filter('orderBy')($scope.lstAdd, 'sort', false);
                        $scope.data_add = treeify($scope.lstAdd).children;
                        var allNodeInParent = $filter('filter')($scope.data, { menuId: item.menuId }, true);
                        if (allNodeInParent && allNodeInParent.length === 1) {
                            angular.forEach(allNodeInParent[0].children, function (item, key) {
                                var obj = {
                                    machucnang: item.menuId,
                                    tenchucnang: item.title,
                                    sothutu: item.sort,
                                    state: item.menuId,
                                    xem: true,
                                    them: false,
                                    sua: false,
                                    xoa: false,
                                    duyet: false,
                                    giamua: false,
                                    giaban: false,
                                    giavon: false,
                                    tylelai: false,
                                    banchietkhau: false,
                                    banbuon: false,
                                    bantralai: false,
                                    username: targetData.username,
                                    menuId: item.menuId,
                                    menuIdCha: item.menuIdCha,
                                    sort: item.sort,
                                    title: item.title,
                                    children: [],
                                    checkState: item.checkState,
                                }
                                $scope.lstAdd.push(obj);
                                var index = $scope.lstChucNang.indexOf(item);
                                $scope.lstChucNang.splice(index, 1);
                            });
                            $scope.lstAdd = $filter('orderBy')($scope.lstAdd, 'sort', false);
                            $scope.data_add = treeify($scope.lstAdd).children;
                            $scope.data = treeify($scope.lstChucNang).children;
                        }
                    }
                }
            };

            $scope.deSelectChucNang = function (item) {
                if (item.menuIdCha != '') {
                    var existParent = $filter('filter')($scope.lstAdd, { menuId: item.menuIdCha }, true);
                    if (existParent && existParent.length === 1) {
                        var childrenNode = $filter('filter')(existParent[0].children, { menuId: item.menuId }, true);
                        if (childrenNode && childrenNode.length === 1) {
                            var index = $scope.lstAdd.indexOf(childrenNode[0]);
                            $scope.lstAdd.splice(index, 1);
                            $scope.data_add = treeify($scope.lstAdd).children;
                            $scope.lstChucNang.push(item);
                            $scope.lstChucNang = $filter('orderBy')($scope.lstChucNang, 'sort', false);
                            $scope.data = treeify($scope.lstChucNang).children;
                        }
                    }
                } else {
                    var existParent = $filter('filter')($scope.lstAdd, { menuId: item.menuId }, true);
                    if (existParent && existParent.length === 1) {
                        var index = $scope.lstAdd.indexOf(existParent[0]);
                        $scope.lstAdd.splice(index, 1);
                        var indexParent = $scope.data_add.indexOf(existParent[0]);
                        $scope.data_add.splice(indexParent, 1);
                        angular.forEach(existParent[0].children, function (item, key) {
                            console.log('remove');
                            var index = $scope.lstAdd.indexOf(item);
                            $scope.lstAdd.splice(index, 1);
                            $scope.lstChucNang.push(item);
                            $scope.data_add = treeify($scope.lstAdd).children;
                        });
                        $scope.lstChucNang = $filter('orderBy')($scope.lstChucNang, 'sort', false);
                        $scope.data = treeify($scope.lstChucNang).children;
                    }
                }

                var filteredData = $filter('filter')($scope.data, { machucnang: item.machucnang }, true);
                if (filteredData && filteredData.length > 0) {
                    var index = $scope.data.indexOf(filteredData[0]);
                    if (index != -1) $scope.data.splice(index, 1);
                    if (filteredData[0].id) {
                        $scope.lstDelete.push({
                            ID: filteredData[0].id,
                            username: filteredData[0].username,
                            machucnang: filteredData[0].machucnang
                        });
                    }
                }
            };

            $scope.deSelectChucNangEdit = function (item) {
                var index = $scope.lstEdit.findIndex(x => x.machucnang == item.machucnang);

                if (index >= 0) {
                    var itemDeleted = $scope.lstEdit[index];
                    $scope.lstDelete.push({
                        ID: itemDeleted.id,
                        username: itemDeleted.username,
                        machucnang: itemDeleted.machucnang
                    });

                    serviceAuMenu.getByMenuId(item.machucnang).then(function (successRes) {
                        if (successRes.data) {
                            $scope.lstChucNang.push(successRes.data);
                            $scope.lstChucNang = $filter('orderBy')($scope.lstChucNang, 'sort', false);
                            $scope.data = treeify($scope.lstChucNang).children;
                        }
                    }, function (errorRes) {
                        console.log(errorRes);
                    });

                    $scope.lstEdit.splice(index, 1);
                    $scope.data_update = treeify($scope.lstEdit).children;
                }
            };

            $scope.modified = function (item) {
                var filteredData = $filter('filter')($scope.lstEdit, { machucnang: item.machucnang }, true);
                if (!filteredData || filteredData.length < 1) {
                    $scope.lstEdit.push(item);
                }
            };

            $scope.asyncParentToChild = function (parent) {
                parent.children.forEach(function (obj) {
                    obj.xem = parent.xem;
                    obj.xoa = parent.xoa;
                    obj.them = parent.them;
                    obj.sua = parent.sua;
                    obj.duyet = parent.duyet;
                    obj.giamua = parent.giamua;
                    obj.giaban = parent.giaban;
                    obj.giavon = parent.giavon;
                    obj.tylelai = parent.tylelai;
                    obj.banchietkhau = parent.banchietkhau;
                    obj.banbuon = parent.banbuon;
                    obj.bantralai = parent.bantralai;
                });
            };
            $scope.check = function (node) {
                if (node) {
                    var choiceChucNang = $filter('filter')($scope.lstEdit, { machucnang: node.machucnang }, true);
                    if (choiceChucNang && choiceChucNang.length === 1) {
                        choiceChucNang[0].xem = node.xem;
                        choiceChucNang[0].them = node.them;
                        choiceChucNang[0].sua = node.sua;
                        choiceChucNang[0].xoa = node.xoa;
                        choiceChucNang[0].banbuon = node.banbuon;
                        choiceChucNang[0].banchietkhau = node.banchietkhau;
                        choiceChucNang[0].bantralai = node.bantralai;
                        choiceChucNang[0].duyet = node.duyet;
                        choiceChucNang[0].giaban = node.giaban;
                        choiceChucNang[0].giamua = node.giamua;
                        choiceChucNang[0].giavon = node.giavon;
                    }
                }

            };
            $scope.save = function () {
                $scope.lstAdd.forEach(function (obj) {
                    var dataExist = $filter('filter')($scope.lstEdit, { machucnang: obj.menuId }, true);
                    if (dataExist && dataExist.length === 1) {
                        var index = $scope.lstEdit.indexOf(dataExist[0]);
                        $scope.lstAdd.splice(index, 1);
                    }
                    if (obj.menuIdCha == "") {
                        var indexCha = $scope.lstAdd.indexOf(obj);
                        $scope.lstAdd.splice(indexCha, 1);
                    }
                });

                var obj = {
                    USERNAME: targetData.username,
                    LstAdd: $scope.lstAdd,
                    LstEdit: $scope.lstEdit,
                    LstDelete: $scope.lstDelete
                }
                service.config(obj).then(function (successRes) {
                    if (successRes && successRes.status == 200) {
                        toaster.pop('success', "Thông báo", successRes.data.message, 2000);
                        $uibModalInstance.close(successRes.data.data);
                    } else {
                        toaster.pop('error', "Lỗi:", successRes.data.message);
                    }
                }, function (errorRes) {
                    console.log(errorRes);
                    toaster.pop('error', "Lỗi:", errorRes.statusText + errorRes.data.message);
                });
            };
            $scope.cancel = function () {
                $uibModalInstance.close();
            };
        }]);
    return app;
});