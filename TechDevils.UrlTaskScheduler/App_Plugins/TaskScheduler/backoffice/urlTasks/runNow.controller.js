angular.module("umbraco").controller("TechDevils.RunNowUrlSchedulerController", function ($scope, urlResource, navigationService, treeService) {

    $scope.completeModel = {};

    $scope.closeRunMenu = function () {
    	navigationService.hideNavigation();
    };


    $scope.runNow = function (id) {
        urlResource.runTaskNow(id).then(function(response) {
        	var returnValue = response.data;

        	$scope.completeModel.ran = returnValue;
        });
    };


});
