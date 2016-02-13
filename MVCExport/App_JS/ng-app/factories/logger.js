'use strict';
(function (window, ng) {
    var tsr = window.toastr
    ng.module('app')
     .factory('logger', [ function () {
         return {
             error: function (exption) {
                 if (tsr)
                     tsr.error(exption.message);
             },
             warn: function (exption) {
                 if (tsr)
                     tsr.warn(exption.message);
             },
             info: function (exption) {
                 if (tsr)
                     tsr.info(exption.message);
             }
         }
     }])




})(this, angular)
