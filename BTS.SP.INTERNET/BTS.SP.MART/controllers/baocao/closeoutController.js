acModule.factory('closeoutService', ['$resource', '$http', '$window', 'configService',
function ($resource, $http, $window, configService) {
    var rootUrl = configService.rootUrlWeb;
    var serviceUrl = rootUrl + '/_vti_bin/Apps/AC/CloseoutService.svc';
    var result = {
    }
    return result;
}])


acModule.controller('closeoutController', ['$scope', '$window', '$stateParams', '$timeout',
'mdService', 'closeoutService', 'clientService', '$filter',
function ($scope, $window, $stateParams, $timeout,
mdService, closeoutService, clientService, $filter) {
    function filterData() {
    };
    filterData();

}])