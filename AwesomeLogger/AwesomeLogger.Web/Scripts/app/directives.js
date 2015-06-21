(function (window, angular) {
    'use strict';

    angular.module('directives', [])
        .directive('loader', [
            function () {

                return {
                    restrict: 'EA',
                    scope: {
                        trigger: '=loaderIf'
                    },
                    template: '<div class="wrapper-loading" ng-show="trigger">Loading...</div></div>'
                };
            }
        ])
        .directive('onConfirmClick', function () {
            return {
                restrict: 'A',
                scope: {
                    message: '@confirmMessage',
                    onConfirm: '&confirmClick'
                },
                link: function (scope, element) {
                    element.bind('click', function () {
                        var message = scope.message || "Are you sure?";
                        if (window.confirm(message)) {
                            var action = scope.onConfirm;
                            if (action != null)
                                scope.$apply(action());
                        }
                    });
                },
            };
        });
})(window, window.angular);