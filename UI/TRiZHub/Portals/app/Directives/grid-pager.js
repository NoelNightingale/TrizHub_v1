var GridPagerNamespace;
(function (GridPagerNamespace) {
    var GridPagerDirectiveController = (function () {
        function GridPagerDirectiveController($scope) {
            this.$scope = $scope;
            this.range = function (min, max, step) {
                step = step || 1;
                var input = [];
                for (var i = min; i <= max; i += step) {
                    input.push(i);
                }
                return input;
            };
            this.previous = function () {
                if (this.gridModel.currentPage != 1) {
                    this.gridModel.currentPage = this.gridModel.currentPage - 1;
                    this.pageGrid.loadGrid();
                }
            };
            this.next = function () {
                if (this.gridModel.currentPage == this.Math.ceil((this.gridModel.totalItems / this.gridModel.recordsPerPage))) {
                }
                else {
                    this.gridModel.currentPage = this.gridModel.currentPage + 1;
                    this.pageGrid.loadGrid();
                }
            };
            this.pageChanged = function (page) {
                this.gridModel.currentPage = page;
                this.pageGrid.loadGrid();
            };
            this.valueDidChangeCallback = function () {
                // Now I can do the thing I wanted to do ...
            };
            this.Math = Math;
            this.gridModel = this.data.gridModel;
            this.pageGrid = this.data.pageGrid;
            $scope.$watch('data', this.valueDidChangeCallback).bind(this);
        }
        GridPagerDirectiveController.$inject = ['$scope']; //Only way for Scope to be injected that works even with minification.
        return GridPagerDirectiveController;
    })();
    GridPagerNamespace.GridPagerDirectiveController = GridPagerDirectiveController;
    var gridPager = (function () {
        function gridPager() {
            this.restrict = 'E';
            this.templateUrl = 'Portals/app/directives/gridPager.html';
            this.controller = GridPagerNamespace.GridPagerDirectiveController;
            this.controllerAs = 'vm';
            this.bindToController = true;
            this.scope = {
                'data': '='
            };
        }
        return gridPager;
    })();
    GridPagerNamespace.gridPager = gridPager;
    angular.module('AngularApp')
        .directive('gridPager', [function () { return new GridPagerNamespace.gridPager(); }]);
})(GridPagerNamespace || (GridPagerNamespace = {}));
//# sourceMappingURL=grid-pager.js.map