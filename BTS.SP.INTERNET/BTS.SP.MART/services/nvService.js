define(['angular'], function (angular) {
    var app = angular.module('nvModule', []);
    app.service('nvService', ['configService', '$resource', function (configService, $resource) {
        var result = {
            config: configService
        };
        result.buildUrl = function (module, action) {
            return configService.rootUrlWeb + '/BTS.SP.MART/views/nv/' + module + '/' + action + '.html';
        };
        return result;
    }]);

    return app;
});