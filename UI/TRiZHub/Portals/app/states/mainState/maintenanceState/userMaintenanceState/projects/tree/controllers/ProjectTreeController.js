var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
var UserProjectsController = (function (_super) {
    __extends(UserProjectsController, _super);
    //#endregion
    //#region Ctor
    function UserProjectsController($scope, $stateParams, $timeout, $window, $state, Popups, ProjectService) {
        _super.call(this, $scope, Popups, $state);
        this.$scope = $scope;
        this.$stateParams = $stateParams;
        this.$timeout = $timeout;
        this.$window = $window;
        this.$state = $state;
        this.Popups = Popups;
        this.ProjectService = ProjectService;
        var self = this;
        this.viewModel = {};
        this.viewModel.userid = this.$stateParams["id"];
        console.log(this.viewModel.userid);
        this.ProjectService.userIdentityProjects(this.viewModel.userid)
            .then(function (result) {
            console.log(result);
        }, function (error) {
            self.handleError(error);
        });
    }
    return UserProjectsController;
}(CHControllerBase));
angular.module("AngularApp")
    .controller("UserProjectsController", [
    "$scope",
    "$stateParams",
    "$timeout",
    "$window",
    "$state",
    "ProjectService",
    "Popups",
    UserProjectsController
]);
//# sourceMappingURL=ProjectTreeController.js.map