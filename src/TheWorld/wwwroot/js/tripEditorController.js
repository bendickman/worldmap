// tripEditorController.js

(function () {
    
    "use strict";

    angular.module("app-trips")
        .controller("tripEditorController", tripEditorController);

    function tripEditorController($routeParams, $http) {

        var vm = this;

        vm.tripName = $routeParams.tripName;
        vm.stops = [];
        vm.errorMessage = "";
        vm.isBusy = true;

        var url = "/api/trips/" + vm.tripName + "/stops";

        $http.get(url)
        .then(function (response) {
            //success
            angular.copy(response.data, vm.stops);
            _shopMap(vm.stops);
        }, function (error) {
            //fail
            vm.errorMessage = "Failed to load stops: " + error;
        }).finally(function () {
            
            vm.isBusy = false;
        });

        vm.addStop = function () {
            vm.isBusy = true;

            $http.post(url, vm.newStop)
            .then(function (response) {
                //sucesss
                vm.stops.push(response.data);
                _shopMap(vm.stops);
                vm.newStop = {};
            }, function (error) {
                //fail
                vm.errorMessage = "Failed to add new stop";
            }).finally(function () {
                vm.isBusy = false;
            });

        };
    };

    function _shopMap(stops) {
        if (stops && stops.length > 0) {

            var mapStops = _.map(stops, function (item) {
                return {
                    lat: item.latitude,
                    long: item.longitude,
                    info: item.name
                };
            });

            //show map
            travelMap.createMap({
                stops: mapStops,
                selector: "#map",
                currentStop: 1,
                initialZoom: 3
            })
        }
    };


})();