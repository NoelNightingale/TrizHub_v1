namespace GridPagerNamespace {

    export class GridPagerDirectiveController {
        data: any;
        Math: any;
        gridModel: any;
        pageGrid: any;
        static $inject = ["$scope"]; //Only way for Scope to be injected that works even with minification.

        constructor(private $scope: ng.IScope) {
            this.Math = Math;
            this.gridModel = this.data.gridModel;
            this.pageGrid = this.data.pageGrid;

            $scope.$watch("data", this.valueDidChangeCallback).bind(this);
        }

        range = function(min, max, step) {
            step = step || 1;
            const input = [];
            for (let i = min; i <= max; i += step) {
                input.push(i);
            }
            return input;
        };

        previous = function() {
            if (this.gridModel.currentPage != 1) {
                this.gridModel.currentPage = this.gridModel.currentPage - 1;
                this.pageGrid.loadGrid();
            }
        };

        next = function() {
            if (this.gridModel.currentPage ==
                this.Math.ceil((this.gridModel.totalItems / this.gridModel.recordsPerPage))) {
            } else {
                this.gridModel.currentPage = this.gridModel.currentPage + 1;
                this.pageGrid.loadGrid();
            }
        };

        pageChanged = function(page) {
            this.gridModel.currentPage = page;
            this.pageGrid.loadGrid();
        };

        valueDidChangeCallback: any = () => {
            // Now I can do the thing I wanted to do ...

        };
    }

    export class gridPager {
        restrict = "E";
        templateUrl = "Portals/app/directives/gridPager.html";
        controller = GridPagerDirectiveController;
        controllerAs = "vm";
        bindToController = true;
        scope: any = {
            'data': "="
        };
    }

    angular.module("AngularApp")
        .directive("gridPager", [() => { return new gridPager() }]);
}