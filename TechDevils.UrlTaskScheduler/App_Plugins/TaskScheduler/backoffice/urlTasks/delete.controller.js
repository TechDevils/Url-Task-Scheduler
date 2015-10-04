angular.module("umbraco")
.controller("TechDevils.DeleteUrlSchedulerController", function ($scope, urlResource, navigationService, treeService) {
	$scope.delete = function (id) {
		urlResource.deleteById(id).then(function () {
			treeService.removeNode($scope.currentNode);
			navigationService.hideNavigation();

		});

	};
	$scope.cancelDelete = function () {
		navigationService.hideNavigation();
	};
})