(function (window, angular) {
    'use strict';

    var module = angular.module('app.subscriptions', [
        'ui.router',
        'services'
    ]);

    // Routes
    module.config([
        '$stateProvider', function ($stateProvider) {
            $stateProvider
                .state('app.subscriptions', {
                    url: '/',
                    templateUrl: 'subscriptions.html',
                    controller: 'SubscriptionsCtrl',
                    data: {
                        pageTitle: 'Subscriptions at Awesome Logger'
                    }
                })
                .state('app.subscriptiondetails', {
                    url: '/{id:int}',
                    templateUrl: 'subscriptions.details.html',
                    controller: 'SubscriptionDetailsCtrl',
                    data: {
                        pageTitle: 'Subscription details at Awesome Logger'
                    }
                })
                .state('app.subscriptioncreate', {
                    url: '/new',
                    templateUrl: 'subscriptions.create.html',
                    controller: 'SubscriptionCreateCtrl',
                    data: {
                        pageTitle: 'New Subscription at Awesome Logger'
                    }
                });
        }
    ]);

    // Controllers
    module.controller('SubscriptionsCtrl', [
        '$scope', '$state', 'subscriptionService',
        function ($scope, $state, subscriptionService) {

            // PROPERTIES
            $scope.items = [];
            $scope.filter = {
                skip: 0,
                take: 20
            };

            $scope.isLoading = false;
            $scope.isAllLoaded = false;

            function filterItems() {

                $scope.isLoading = true;
                return subscriptionService.query($scope.filter)
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
                $scope.isLoading = true;
                filterItems();
            }

            // INIT
            load();
        }
    ]);

    module.controller('SubscriptionDetailsCtrl', [
        '$scope', '$state', 'subscriptionService', '$stateParams',
        function ($scope, $state, subscriptionService, $stateParams) {

            $scope.item = null;
            $scope.isLoading = false;

            $scope.update = function (form, item) {
                if (form.$invalid) {
                    return;
                }

                item.$isLoading = true;
                item.$update().then(function () {
                    item.$isLoading = false;
                }, function (reason) {
                    item.$isLoading = false;
                    throw new Error(reason);
                });

            };

            $scope.delete = function (item) {
                item.$delete().then(function () {
                    $state.go('^.subscriptions');
                }, function (reason) {
                    throw new Error(reason);
                });
            }

            function load(id) {
                $scope.isLoading = true;
                subscriptionService.get(id).then(function (data) {
                    $scope.item = data;
                    $scope.isLoading = false;
                }, function () {
                    $scope.isLoading = false;
                });
            }

            load($stateParams.id);
        }
    ]);

    module.controller('SubscriptionCreateCtrl', [
        '$scope', '$state', 'subscriptionService',
        function ($scope, $state, subscriptionService) {

            $scope.item = subscriptionService.create({});
            $scope.isLoading = false;

            $scope.create = function (form, item) {
                if (form.$invalid) {
                    return;
                }

                item.$isLoading = true;
                item.$save().then(function () {
                    item.$isLoading = false;
                    $state.go('^.subscriptions');
                }, function (reason) {
                    item.$isLoading = false;
                    throw new Error(reason);
                });

            };
        }
    ]);

})(window, window.angular);
