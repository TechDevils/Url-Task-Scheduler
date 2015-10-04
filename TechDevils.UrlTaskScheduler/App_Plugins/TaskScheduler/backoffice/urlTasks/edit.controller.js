angular.module("umbraco").controller("TechDevils.EditUrlSchedulerController",
    function ($scope, $routeParams, urlResource, notificationsService, navigationService) {
        var daysOfTheWeek = ["mon", "tue", "wed", "thu", "fri", "sat", "sun"];

        $scope.loaded = false;
        $scope.creationId = $routeParams.id;
        if ($routeParams.id == -1) {
            $scope.url = {};
            $scope.url.daysToRun = {};
        } else {
            $scope.url = {};
            $scope.url.daysToRun = {};
            urlResource.getUrlTaskById($routeParams.id).then(function(response) {

                SetupUrlData($scope, response.data);
                $scope.loaded = true;

            });
        }

        $scope.loaded = true;

        $scope.checkSetAllValue = function () {
            var value = true;
            for (var i = 0; i < daysOfTheWeek.length; i++) {
                var day = daysOfTheWeek[i];
                if (value && $scope.url.daysToRun[day] != value) {
                    value = !value;
                }
            }

            $scope.url.daysToRunAll = value;
        }

        $scope.setUrlsDays = function() {
            var value = $scope.url.daysToRunAll;           
            for (var i = 0; i < daysOfTheWeek.length; i++) {
                var day = daysOfTheWeek[i];
                $scope.url.daysToRun[day] = value;
            }
        }

        $scope.enableUrl = function () {
            urlResource.enableUrl($routeParams.id).then(function (response) {
                notificationsService.info("Enabled url");
                //navigationService.reloadNode();
                navigationService.syncTree({ tree: 'urlTasks', path: [-1, -1], forceReload: true });
                $scope.url.disabled = false;
            });
            return false;
        }

        $scope.disableUrl = function () {
            urlResource.disableUrl($routeParams.id).then(function(response) {
                notificationsService.error("Disabled url");
                //navigationService.reloadNode();
                navigationService.syncTree({ tree: 'urlTasks', path: [-1, -1], forceReload: true });
                $scope.url.disabled = true;
            });
            return false;
        }

        $scope.save = function (url) {
            url.daysToRun.toString = function urlToString() {
                var daysOfTheWeekValue = [0, 0, 0, 0, 0, 0, 0];

                for (var i = 0; i < daysOfTheWeek.length; i++) {
                    var day = daysOfTheWeek[i];
                    if (this[day] != undefined && this[day]) {
                        daysOfTheWeekValue[i] = 1;
                    }
                }

                var returnValue = "mon:" + daysOfTheWeekValue[0] + ";tue:" + daysOfTheWeekValue[1] + ";wed:" + daysOfTheWeekValue[2] + ";thu:" + daysOfTheWeekValue[3] + ";fri:" + daysOfTheWeekValue[4] + ";sat:" + daysOfTheWeekValue[5] + ";sun:" + daysOfTheWeekValue[6];

                return returnValue;
            }


            url.daysToRunOutput = url.daysToRun.toString();
            if (url.timeToRun == undefined) {
                url.timeToRun = "";
            }
            urlResource.save(url).then(function (response) {
                SetupUrlData($scope, response.data);
                $scope.urlTask.$dirty = false;
                navigationService.syncTree({ tree: 'urlTasks', path: [-1, -1], forceReload: true });
                notificationsService.success("Success test has been saved");
            });
        }

       
    });

function SetupUrlData($scope, urlData) {
    $scope.url = urlData;
    $scope.url.daysToRun = {};
    $scope.url.daysToRunAll = true;

    if ($scope.url.nextRun != undefined) {
        $scope.url.nextRunDate = new Date($scope.url.nextRun);
    }

    if ($scope.url.lastRun != undefined) {
        $scope.url.lastRunDate = new Date($scope.url.lastRun);
    }

    var daysOfTheWeek = $scope.url.daysToRunOutput.split(';');

    for (var i = 0; i < daysOfTheWeek.length; i++) {
        var day = [];

        if (daysOfTheWeek[i] != undefined) {
            day = daysOfTheWeek[i].split(":");
        } else {
            continue;
        }
        var value = day[1] == "1";
        $scope.url.daysToRun[day[0]] = value;

        if ($scope.url.daysToRunAll && !value) {
            $scope.url.daysToRunAll = false;
        }

    }
}