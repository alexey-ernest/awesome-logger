(function (window, angular) {
    'use strict';

    var module = angular.module('app.audit', [
        'ui.router',
        'services'
    ]);

    // Routes
    module.config([
        '$stateProvider', function ($stateProvider) {
            $stateProvider
                .state('app.audit', {
                    url: '/audit/{id}',
                    templateUrl: 'audit.html',
                    controller: 'AuditCtrl',
                    data: {
                        pageTitle: 'Subscription History at Awesome Logger'
                    }
                });
        }
    ]);

    // Controllers
    module.controller('AuditCtrl', [
        '$scope', '$stateParams', 'subscriptionService', 'auditService',
        function ($scope, $stateParams, subscriptionService, auditService) {

            // PROPERTIES
            $scope.items = [];
            $scope.filter = {
                id: $stateParams.id,
                skip: 0,
                take: 20
            };

            $scope.isLoading = false;
            $scope.isAllLoaded = false;

            function filterItems() {

                $scope.isLoading = true;
                return auditService.query($scope.filter)
                    .then(function (data) {
                        if (data.length < $scope.filter.take) {
                            $scope.isAllLoaded = true;
                        } else {
                            $scope.isAllLoaded = false;
                        }

                        if (!$scope.filter.skip) {
                            $scope.items = [];
                        }
                        $scope.items = $scope.items.concat(data);

                        $scope.isLoading = false;
                    }, function () {
                        $scope.isLoading = false;
                        throw new Error();
                    });
            };

            $scope.nextPage = function () {
                if ($scope.isAllLoaded || $scope.isLoading) {
                    return;
                }

                $scope.filter.skip += $scope.filter.take;
                filterItems();
            };

            function load() {
                filterItems();
            }

            load();
        }
    ]);

})(window, window.angular);
