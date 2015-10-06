angular.module("umbraco")
    .controller("TechDevils.DashboardController", function ($scope, urlResource, navigationService, notificationsService) {
        $scope.changeStatus = function(value) {
            var item = $scope.urls[value];

            if (item.disabled) {
                urlResource.enableUrl(value);
                notificationsService.info("Enable '" + item.desc + "' Url");
            } else {
                urlResource.disableUrl(value);
                notificationsService.warning("Disable '" + item.desc + "' Url");
            }

            item.disabled = !item.disabled;
            item["disabledIcon"] = ((item["disabled"]) ? "red" : "blue");
            navigationService.syncTree({ tree: 'urlTasks', path: [-1, -1], forceReload: true });
        }

        $scope.runNowNow = function(value) {
            var item = $scope.urls[value];

            if (item.disabled) {
                var confirmed = confirm(item.desc + " is disabled would you still like to run it");

                if (confirmed) {
                    urlResource.runTaskNow(value);
                } else {
                    notificationsService.info("cancelled run now");
                }
            } else {
                urlResource.runTaskNow(value);
                notificationsService.info("Ran " + item.desc);
            }
        }

        $scope.urls = {};
             
        urlResource.checkStatus(1, 200).then(function(response) {
            for (var i = 0; i < response["data"].length; i++) {
                var item = response["data"][i];

                var feq = "";
                if (item["runningType"] == "dayAndTime") {
                    var days = item["daysToRunOutput"].split(";");
                    var runDays = "";
                    for (var j = 0; j < days.length; j++) {
                        var day = days[j].split(":");
                        if (day[1] == "1") {
                            if (runDays.length == 0) {
                                runDays = day[0];
                            } else {
                                runDays = runDays + "," + day[0];
                            }
                        }
                    }
                    feq = runDays + " @ " + item["timeToRun"];

                } else if (item["runningType"] == "interval") {
                    feq = item["minuteInterval"] + "min";
                }

                item["feq"] = feq;
                item["disabledIcon"] = ((item["disabled"]) ? "red" : "blue");

                $scope.urls[item.id] = item;
            }
        });

});
