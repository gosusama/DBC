/**
 * loads sub modules and wraps them up into the main module
 * this should be used for top-level module definitions only
 */
define([
	'jquery',
    'jquery-ui',
    'angular',
    'states/htdm',
    'states/nv',
    'states/baocao',
    'states/auth',
    'config/config',
	'ocLazyLoad',
	'uiRouter',
	'angularStorage',
    'angular-animate',
    'angular-resource',
    'angular-filter',
    'angular-sanitize',
    'angular-cache',
	'ui-bootstrap',
    'loading-bar',
    'smartTable',
    'ngTable',
    'ngNotify',
    'ui.tree',
    'dynamic-number',
    'angular-confirm',
	'services/interceptorService',
    'services/configService',
    'services/tempDataService',
    'filters/common',
    'kendo',
    'telerikReportViewer',
    'telerikReportViewer_kendo',
    'toaster',
    'ui-grid',
    'fileUpload',
    'ng-file-upload',
    'ng-tags-input',
    'ng-ckeditor',
    'ngMaterial',
    'ngAria',
    'angular-md5',
], function (jquery, jqueryui, angular, stateHtdm, stateNv, stateBaoCao, stateAuth) {
    'use strict';
    var app = angular.module('myApp', ['oc.lazyLoad', 'ui.router', 'InterceptorModule', 'LocalStorageModule', 'ui.bootstrap', 'configModule', 'tempDataModule', 'angular-loading-bar', 'ngAnimate', 'ngSanitize', 'common-filter', 'ngResource', 'smart-table', 'angular.filter', 'ngTable', 'angular-cache', 'ngNotify', 'ui.tree', 'dynamicNumber', 'toaster', 'ui.grid', 'cp.ngConfirm', 'angularFileUpload', 'ngFileUpload', 'ngTagsInput', 'ngCkeditor', 'ngMaterial', 'ngAria', 'angular-md5']);
    app.run(['ngTableDefaults', 'ngNotify', function (ngTableDefaults, ngNotify) {
        ngTableDefaults.params.count = 5;
        ngTableDefaults.settings.counts = [];
        ngNotify.config({
            theme: 'pure',
            position: 'bottom',
            duration: 800,
            type: 'info',
            sticky: false,
            button: true,
            html: true
        });
    }]);
    app.constant("keyCodes", {
        A: 65,
        B: 66,
        C: 67,
        D: 68,
        E: 69,
        F: 70,
        G: 71,
        H: 72,
        I: 73,
        J: 74,
        K: 75,
        L: 76,
        M: 77,
        N: 78,
        O: 79,
        P: 80,
        Q: 81,
        R: 82,
        S: 83,
        T: 84,
        U: 85,
        V: 86,
        W: 87,
        X: 88,
        Y: 89,
        Z: 90,
        ZERO: 48,
        ONE: 49,
        TWO: 50,
        THREE: 51,
        FOUR: 52,
        FIVE: 53,
        SIX: 54,
        SEVEN: 55,
        EIGHT: 56,
        NINE: 57,
        NUMPAD_0: 96,
        NUMPAD_1: 97,
        NUMPAD_2: 98,
        NUMPAD_3: 99,
        NUMPAD_4: 100,
        NUMPAD_5: 101,
        NUMPAD_6: 102,
        NUMPAD_7: 103,
        NUMPAD_8: 104,
        NUMPAD_9: 105,
        NUMPAD_MULTIPLY: 106,
        NUMPAD_ADD: 107,
        NUMPAD_ENTER: 108,
        NUMPAD_SUBTRACT: 109,
        NUMPAD_DECIMAL: 110,
        NUMPAD_DIVIDE: 111,
        F1: 112,
        F2: 113,
        F3: 114,
        F4: 115,
        F5: 116,
        F6: 117,
        F7: 118,
        F8: 119,
        F9: 120,
        F10: 121,
        F11: 122,
        F12: 123,
        F13: 124,
        F14: 125,
        F15: 126,
        COLON: 186,
        EQUALS: 187,
        UNDERSCORE: 189,
        QUESTION_MARK: 191,
        TILDE: 192,
        OPEN_BRACKET: 219,
        BACKWARD_SLASH: 220,
        CLOSED_BRACKET: 221,
        QUOTES: 222,
        BACKSPACE: 8,
        TAB: 9,
        CLEAR: 12,
        ENTER: 13,
        SHIFT: 16,
        CONTROL: 17,
        ALT: 18,
        CAPS_LOCK: 20,
        ESC: 27,
        SPACEBAR: 32,
        PAGE_UP: 33,
        PAGE_DOWN: 34,
        END: 35,
        HOME: 36,
        LEFT: 37,
        UP: 38,
        RIGHT: 39,
        DOWN: 40,
        INSERT: 45,
        DELETE: 46,
        HELP: 47,
        NUM_LOCK: 144
    });
    app.directive('dateCheck', [function () {
        return {
            require: 'ngModel',
            link: function (scope, elem, attrs, ctrl) {
                var firstDateElement = '#' + attrs.dateCheck;
                elem.add(firstDateElement).on('change', function () {
                    scope.$apply(function () {
                        var firstDate = $(firstDateElement).val().replace('-', '/');
                        var curDate = elem.val().replace('-', '/');
                        if (curDate >= firstDate) {
                            ctrl.$setValidity('dateok', true);
                        } else {
                            ctrl.$setValidity('dateok', false);
                        }
                    });
                });

            }
        }
    }]);

    app.directive('myLoading', function () {
        var hostname = window.location.hostname;
        var port = window.location.port;
        var rootUrl = 'http://' + hostname + ':' + port;
        var result = {
            restrict: 'E',
            templateUrl: rootUrl + '/BTS.SP.MART/views/layouts/loading.html'
        }
        return result;
    });
    app.directive('keyboard', function ($document, keyCodes) {
        return {
            link: function (scope, element, attrs) {

                var keysToHandle = scope.$eval(attrs.keyboard);
                var keyHandlers = {};
                // Registers key handlers
                angular.forEach(keysToHandle, function (callback, keyName) {
                    var keyCode = keyCodes[keyName];
                    keyHandlers[keyCode] = { callback: callback, name: keyName };
                });
                // Bind to document keydown event
                $document.on("keydown", function (event) {
                    var keyDown = keyHandlers[event.keyCode];
                    // Handler is registered
                    if (keyDown) {
                        event.preventDefault();
                        // Invoke the handler and digest
                        scope.$apply(function () {
                            keyDown.callback(keyDown.name, event.keyCode);
                        });
                    }
                });
            }
        };
    });
    app.directive('report', ['configService', function (configService) {
        return {
            restrict: 'EA',
            transclude: 'true',
            scope: {
                name: '@',
                params: '@'
            },
            template: "",
            link: function (scope, element, attrs) {
                //create the viewer object first, can be done in index.html as well
                var reportViewer = $("#reportViewer1").data("telerik_ReportViewer");
                if (!reportViewer) {
                    $("#reportViewer1").toggle();
                    var objpara = JSON.parse(scope.params);
                    $(document).ready(function () {
                        $("#reportViewer1").telerik_ReportViewer({
                            error: function (e, args) {
                                alert('Error from report directive:' + args);
                            },
                            reportSource: {
                                report: scope.name,
                                parameters: objpara,

                            },
                            serviceUrl: configService.rootUrlWebApi + "/reports",
                            scaleMode: 'SPECIFIC',
                            scale: 1.0,
                            viewMode: 'PRINT_PREVIEW',
                            ready: function () {

                            },
                            renderingBegin: function () {
                                console.log("renderingBegin");
                            },
                            renderingEnd: function () {
                                console.log("renderingEnd");
                            }
                        });
                    });
                }
                //on state change update the report source
                scope.$watch('name', function () {
                    var reportViewer = $("#reportViewer1").data("telerik_ReportViewer");
                    if (reportViewer) {
                        var rs = reportViewer.reportSource();
                        if (rs && rs.report) {
                            if (rs.report != scope.name &&
                                rs.parameters != scope.parameters) {
                                reportViewer.reportSource({
                                    report: scope.name,
                                    parameters: angular.toJson(scope.parameters),
                                });
                            }
                        }
                    }
                });
            }
        }
    }]);
    app.directive('preventDefault', function () {
        return function (scope, element, attrs) {
            angular.element(element).bind('click', function (event) {
                event.preventDefault();
                event.stopPropagation();
            });
        }
    });

    app.service('blockModalService', function () {
        var result = this;
        result.isBlocked = false;
        result.setValue = function (value) {
            if (result.isBlocked !== value) {
                result.isBlocked = value;
            }
        }
        return result;
    });
    app.directive('ngEnter', function () {
        return function (scope, element, attrs) {
            element.bind("keydown keypress", function (event) {
                if (event.which === 13) {
                    scope.$apply(function () {
                        scope.$eval(attrs.ngEnter);
                    });

                    event.preventDefault();
                }
            });
        };
    });

    app.config(['$httpProvider', 'CacheFactoryProvider', 'cfpLoadingBarProvider', 'localStorageServiceProvider', function ($httpProvider, CacheFactoryProvider, cfpLoadingBarProvider, localStorageServiceProvider) {
        $httpProvider.interceptors.push('interceptorService');

        cfpLoadingBarProvider.includeSpinner = true;
        cfpLoadingBarProvider.includeBar = true;

        localStorageServiceProvider.setStorageType('cookie').setPrefix('ss');

        angular.extend(CacheFactoryProvider.defaults, {
            maxAge: 3600000, //1 hour
            deleteOnExpire: 'aggressive',
            storageMode: 'memory',
            onExpire: function (key, value) {
                var _this = this; // "this" is the cache in which the item expired
                if (key.indexOf('/') !== -1) {
                    angular.injector(['ng']).get('$http').get(key).then(function (successRes) {
                        //console.log('successRes', successRes);
                        _this.put(key, successRes.data);
                    }, function (errorRes) {
                        //console.log('errorRes', errorRes);
                    });
                } else {
                    _this.put(key, value);
                    //console.log(key, angular.toJson(value));
                }
            }
        });
    }]);

    //validate number
    app.config(function (dynamicNumberStrategyProvider) {
        dynamicNumberStrategyProvider.addStrategy('number', {
            numInt: 18,
            numFract: 3,
            numSep: ',',
            numPos: true,
            numNeg: true,
            numRound: 'round',
            numThousand: true,
            numThousandSep: ' '
        });
        dynamicNumberStrategyProvider.addStrategy('number-utc', {
            numInt: 18,
            numFract: 2,
            numSep: '.',
            numPos: true,
            numNeg: true,
            numRound: 'round',
            numThousand: true
        });
        dynamicNumberStrategyProvider.addStrategy('number-int', {
            numInt: 18,
            numFract: 0,
            numSep: ',',
            numPos: true,
            numNeg: true,
            numRound: 'ceil',
            numThousand: true
        });
        dynamicNumberStrategyProvider.addStrategy('number-38', {
            numInt: 21,
            numFract: 2,
            numSep: ',',
            numPos: true,
            numNeg: true,
            numRound: 'round',
            numThousand: true
        });
        dynamicNumberStrategyProvider.addStrategy('number-int-38', {
            numInt: 21,
            numFract: 0,
            numSep: ',',
            numPos: true,
            numNeg: true,
            numRound: 'ceil',
            numThousand: true
        });
    });

    //Auth - check role
    app.service('securityService', ['$http', 'configService', function ($http, configService) {
        var result = {
            getAccessList: function (machucnang) {
                return $http.get(configService.rootUrlWebApi + '/Authorize/Shared/GetAccesslist/' + machucnang);
            }
        };
        return result;
    }]);

    app.config(['$stateProvider', '$urlRouterProvider', '$ocLazyLoadProvider', '$locationProvider',
		function ($stateProvider, $urlRouterProvider, $ocLazyLoadProvider, $locationProvider) {
		    $ocLazyLoadProvider.config({
		        jsLoader: requirejs,
		        loadedModules: ['app'],
		        asyncLoader: require
		    });
		    var layoutUrl = "/BTS.SP.MART/";
		    $urlRouterProvider.otherwise("/home");

		    $stateProvider.state('root', {
		        abstract: true,
		        views: {
		            'viewRoot': {
		                templateUrl: layoutUrl + "views/layouts/layout.html",
		                controller: "layoutCrtl as ctrl"

		            }
		        },
		        resolve: {
		            loadModule: ['$ocLazyLoad', '$q', function ($ocLazyLoad, $q) {
		                var deferred = $q.defer();
		                require(['/BTS.SP.MART/controllers/layouts/layout-controller.js'],
                            function (layoutModule) {//url c?a module
                                deferred.resolve();
                                $ocLazyLoad.inject(layoutModule.name);
                            });
		                return deferred.promise;
		            }]
		        }
		    });

		    $stateProvider.state('layout', {
		        parent: 'root',
		        abstract: true,
		        views: {
		            'viewHeader': {
		                templateUrl: layoutUrl + "views/layouts/header.html",
		                controller: "HeaderCtrl as ctrl"
		            }
		        },
		        resolve: {
		            loadModule: [
                        '$ocLazyLoad', '$q', function ($ocLazyLoad, $q) {
                            var deferred = $q.defer();
                            require([
                                    '/BTS.SP.MART/controllers/layouts/header-controller.js'
                            ],
                                function (headerModule) { //url c?a module
                                    deferred.resolve();
                                    $ocLazyLoad.inject(headerModule.name);
                                });
                            return deferred.promise;
                        }
		            ]

		        }
		    });

		    $stateProvider.state('login', {
		        url: "/login",
		        abstract: false,
		        views: {
		            'viewRoot': {
		                templateUrl: layoutUrl + "views/layouts/login.html",
		                controller: "loginCrtl as ctrl"
		            }
		        },
		        resolve: {
		            loadModule: ['$ocLazyLoad', '$q', function ($ocLazyLoad, $q) {
		                var deferred = $q.defer();
		                require(['/BTS.SP.MART/controllers/auth/AuthController.js'],
                            function (layoutModule) {//url c?a module
                                deferred.resolve();
                                $ocLazyLoad.inject(layoutModule.name);
                            });
		                return deferred.promise;
		            }]
		        }
		    });

		    $stateProvider.state('banLe', {
		        url: "/banLe",
		        abstract: false,
		        views: {
		            'viewRoot': {
		                templateUrl: layoutUrl + "views/nv/BanLe/index.html",
		                controller: "banLeController as ctrl"
		            }
		        },
		        resolve: {
		            loadModule: ['$ocLazyLoad', '$q', function ($ocLazyLoad, $q) {
		                var deferred = $q.defer();
		                require(['/BTS.SP.MART/controllers/nv/banLeController.js'],
                            function (layoutModule) {//url c?a module
                                deferred.resolve();
                                $ocLazyLoad.inject(layoutModule.name);
                            });
		                return deferred.promise;
		            }]
		        }
		    });

		    $stateProvider.state('home', {
		        url: "/home",
		        parent: 'layout',
		        abstract: false,
		        views: {
		            'viewMain@root': {
		                templateUrl: layoutUrl + "views/layouts/home.html",
		                controller: "homeCtrl as ctrl"
		            }
		        },
		        resolve: {
		            loadModule: ['$ocLazyLoad', '$q', function ($ocLazyLoad, $q) {
		                var deferred = $q.defer();
		                require(['/BTS.SP.MART/controllers/layouts/home-controller.js'],
                            function (homeModule) {
                                deferred.resolve();
                                $ocLazyLoad.inject(homeModule.name);
                            });
		                return deferred.promise;
		            }]
		        }
		    });

		    var lststate = [];
		    lststate = lststate.concat(stateHtdm).concat(stateNv).concat(stateBaoCao).concat(stateAuth);
		    angular.forEach(lststate, function (state) {
		        $stateProvider.state(state.name, {
		            url: state.url,
		            parent: state.parent,
		            abstract: state.abstract,
		            views: state.views,
		            resolve: {
		                loadModule: ['$ocLazyLoad', '$q', function ($ocLazyLoad, $q) {
		                    var deferred = $q.defer();
		                    require([state.moduleUrl], function (module) {
		                        deferred.resolve();
		                        $ocLazyLoad.inject(module.name);
		                    });
		                    return deferred.promise;
		                }]
		            }
		        });
		    });


		}]);

    return app;
});
