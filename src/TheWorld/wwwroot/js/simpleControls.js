// simpleControls.js

(function () {

    "use strict";

    angular.module("simpleControls", [])
        .directive("waitCursor", waitCursor);

    //custom directive 
    function waitCursor() {
        return {
            scope: {
                show: "=displayWhen"
            },
            restrict: "E",
            templateUrl: "/views/waitCursor.html"//client side file found inside wwwwroot
        };
    };

})();