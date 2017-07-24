var app = angular.module("BMCBuild", []);


app.service('RestCall', ['$http', function ($http) {

    //Update this url While hosting
    var urlBase = 'http://localhost:61319/';

    this.getCall = function (name ){
        return $http.get(urlBase + name);
    };
    this.getCallWithParam = function (name,param) {
        return $http.get(urlBase + name+'?'+param);
    };

}]);

app.controller('Build', function ($scope, $interval, RestCall, $timeout) {
    $scope.data = [];
    $scope.logData = [];
    $scope.status = null;
    $scope.details = null;

    OnLoad();

    function OnLoad() {
        GetStatus();
        GetLogData();
        GetProcessDetails();
    }

    var poll = $interval(function () {
        GetStatus();
    },10000)

    $scope.executeTask = function () {
        var email = "";
        if ($scope.IsCustomMail)
            email = $scope.email;
        RestCall.getCallWithParam("Execute","email="+email)
            .then(function (response) {
                GetStatus();
                GetProcessDetails();
                $timeout(GetLogData(),5000);
        });
    }

    $scope.UpdatelogTable = function () {
        GetLogData();
    }

    function GetStatus() {
        RestCall.getCall("GetProcessStatus").then(function (response) {
            $scope.status = response.data;
        });
    };

    function GetLogData() {
        RestCall.getCall("GetLogData").then(function (response) {
            $scope.logData = response.data;
        }, function (response) {
            $scope.data = response.data;
            console.log(response);
        });
    }
    function GetProcessDetails() { 
        RestCall.getCall("GetProcessDetails").then(function (response) {
            $scope.details = response.data;
            console.log(response);
        });
    }
    $scope.$on('$destroy', function() {
        if (angular.isDefined(poll)) {
            $interval.cancel(poll);
            poll = undefined;
        }

    });
});