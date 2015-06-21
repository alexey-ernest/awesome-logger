(function (window, angular) {
    'use strict';

    var module = angular.module('services', ['ngResource']);

    module.factory('$exceptionHandler', [
        function () {
            return function (exception) {

                var message = exception.message;
                if (message === '[object Object]') {
                    message = 'Unknown Error';
                }

                alert(message);
            };
        }
    ]);

    function buildODataQueryParams(filter, customParams) {

        customParams = customParams || {};

        var params = {};

        // standard params
        if (filter.orderBy) {
            var order = filter.orderByDesc ? 'desc' : 'asc';
            params['$orderby'] = filter.orderBy + ' ' + order;
            delete filter.orderBy;
            delete filter.orderByDesc;
        }

        if (filter.skip) {
            params['$skip'] = filter.skip;
            delete filter.skip;
        }

        if (filter.take) {
            params['$top'] = filter.take;
            delete filter.take;
        }

        // custom params
        var filterParts = [];
        for (var param in customParams) {
            if (!filter.hasOwnProperty(param)) continue;
            filterParts.push(customParams[param].replace('%', filter[param]));
        }

        if (filterParts.length > 0) {
            var filterExpr = filterParts.join(' and ');
            params['$filter'] = filterExpr;
        }

        return params;
    }

    module.factory('subscriptionService', [
        '$resource', '$q', function ($resource, $q) {

            var url = '/api/subscriptions';
            var resource = $resource(url + '/:id',
            { id: '@id' },
            {
                update: { method: 'PUT' }
            });


            return {
                create: function (options) {
                    return new resource(options);
                },
                get: function (id) {
                    var deferred = $q.defer();

                    resource.get({ id: id }, function (data) {
                        deferred.resolve(data);
                    }, function () {
                        deferred.reject();
                    });

                    return deferred.promise;
                },
                query: function (filter) {

                    filter = filter || {};
                    var params = buildODataQueryParams(filter);

                    var deferred = $q.defer();
                    resource.query(params, function (data) {
                        deferred.resolve(data);
                    }, function () {
                        deferred.reject();
                    });

                    return deferred.promise;
                }
            };
        }
    ]);

    module.factory('auditService', [
            '$resource', '$q', function ($resource, $q) {

                var url = '/api/audit/';
                var resource = $resource(url + '/:id',
                { id: '@id' },
                {
                    update: { method: 'PUT' }
                });

                return {
                    query: function (params) {

                        params = params || {};

                        var deferred = $q.defer();
                        resource.query(params, function (data) {
                            deferred.resolve(data);
                        }, function () {
                            deferred.reject();
                        });

                        return deferred.promise;
                    }
                };
            }
    ]);

})(window, window.angular);
