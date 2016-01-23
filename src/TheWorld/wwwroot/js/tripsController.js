// tripsController.js

(function  () {

    "use strict";

    /* 
     * - get the app-trips module and add a controller 
     * - controller called tripsController and function that actually is the controller
     *  - note - use same name
     */
    angular.module("app-trips")
       .controller("tripsController", tripsController);

    function tripsController($http) {

        // - expose some data
        var vm = this;

        vm.trips = [];

        vm.errorMessage = "";
        vm.isBusy = true;

        $http.get("/api/trips")
            .then(function (response) {
                //sucess
                angular.copy(response.data, vm.trips);
            }, function (error) {
                //fail
                vm.errorMessage = "Failed to load data:" + error;
            })
            .finally(function () {

                vm.isBusy = false;
            });

        vm.newTrip = {};

        vm.addTrip = function () {
            
            vm.isBusy = true;
            vm.errorMessage = "";

            $http.post("/api/trips/", vm.newTrip)
            .then(function (response) {
                //success
                vm.trips.push(response.data);
                vm.newTrip = {};
            }, function (error) {
                //fail
                vm.errorMessage = error;

            })
            .finally(function () {
                vm.isBusy = false;
            });

        };

    };

})();

