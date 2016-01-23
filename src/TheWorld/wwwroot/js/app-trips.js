// app-trips.js

(function () {

    "use strict";
    
    // - create the module
    angular.module("app-trips", ["simpleControls", "ngRoute"])
        .config(function ($routeProvider) {
            
            $routeProvider.when("/", {
                controller: "tripsController",
                controllerAs: "vm",
                templateUrl: "/views/tripsView.html"
            });

            $routeProvider.when("/editor/:tripName", {//:tripName parm
                controller: "tripEditorController",
                controllerAs: "vm",
                templateUrl: "/views/tripEditorView.html"
            });

            $routeProvider.otherwise({ redirectTo: "/" });
        });

})();

