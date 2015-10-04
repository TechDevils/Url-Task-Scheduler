angular.module("umbraco.resources")
	.factory("urlResource", function ($http) {
	    return {
	        save: function (url) {
	            return $http.post("backoffice/TaskScheduler/UrlScheduleTaskApi/PostSave", angular.toJson(url));
	        },
	        getUrlTaskById : function(id) {
	            return $http.get("backoffice/TaskScheduler/UrlScheduleTaskApi/GetTaskById?id=" + id);
	        },
            deleteById : function(id) {
                return $http.delete("backoffice/TaskScheduler/UrlScheduleTaskApi/DeleteById?id=" + id);
            },
            runTaskNow : function(id) {
                return $http.post("backoffice/TaskScheduler/UrlScheduleTaskApi/RunTaskNow?id=" + id);
            },
            disableUrl: function (id) {
                return $http.post("backoffice/TaskScheduler/UrlScheduleTaskApi/DisableUrl?id=" + id);
            },
            enableUrl: function (id) {
                return $http.post("backoffice/TaskScheduler/UrlScheduleTaskApi/EnableUrl?id=" + id);
            },
            checkStatus:function(page, size) {
                return $http.post("backoffice/TaskScheduler/UrlScheduleTaskApi/GetStatus?page=" + page + "&pageSize=" + size);
            }
	    };
	});