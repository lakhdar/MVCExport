
(function (window, ng) {
  'use strict';

  var app = ng.module('app', [
         'ngAnimate',        // animations
         'ngRoute',          // routing
         'ngSanitize',       // sanitizes html bindings (ex: sidebar.js)
  ])
  .config(['$routeProvider', 'routes', function ($routeProvider, routes) {
    routes.forEach(function (r) {
      $routeProvider.when(r.url, r.config);
    });
    $routeProvider.otherwise({ redirectTo: '/' });
  }])
  .config(['$controllerProvider', function ($controllerProvider) {
    $controllerProvider.allowGlobals();
  }])
  .config(['$provide', function ($provide) {
      $provide.decorator('$exceptionHandler',
          ['$delegate', 'logger', function ($delegate, logger) {
            return function (exception, cause) {
              $delegate(exception, cause);
              var errorData = { message: exception, cause: cause };
              logger.error(errorData)
            };
          }]);

  }]).config(['$httpProvider', function ($httpProvider) {
    $httpProvider.defaults.headers.common["X-Requested-With"] = "XMLHttpRequest";
  }])
})(this, angular)
 