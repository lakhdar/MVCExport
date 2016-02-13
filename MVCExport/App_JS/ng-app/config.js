(function (win, ng) {
  'use strict';
  ng.module('app')
   .constant('routes', [
          {
            url: '/',
            config: {
              templateUrl: 'home/home',
              settings: {
              }
            }
          }, {
            url: '/contact',
            config: {
              templateUrl: 'home/contact',
              settings: {
              }
            }
          }, {
            url: '/about',
            config: {
              templateUrl: 'home/about',
              settings: {
              }
            }
          }, {
            url: '/Employee',
            config: {
              templateUrl: 'Employee/index',
              settings: {
              }
            }
          }  , {
            url: '/Employee/details/:id',
            config: {
              templateUrl: function (params) {
                return 'Employee/details/' + params.id;
              },
              settings: {
              }
            }
          } 
   ]);

})(this, angular)