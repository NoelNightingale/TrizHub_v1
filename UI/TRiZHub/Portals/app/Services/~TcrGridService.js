var TcrGridModel = /** @class */ (function () {
    function TcrGridModel() {
    }
    return TcrGridModel;
}());
;
var TcrGridServiceModule;
(function (TcrGridServiceModule) {
    var TcrGridService = /** @class */ (function () {
        function TcrGridService(defaultSortColumnName, searchFunction, onDataLoaded, customSearchModel, onGetMyModel, $state) {
            var _this = this;
            this.defaultSortColumnName = defaultSortColumnName;
            this.searchFunction = searchFunction;
            this.onDataLoaded = onDataLoaded;
            this.customSearchModel = customSearchModel;
            this.onGetMyModel = onGetMyModel;
            this.$state = $state;
            this.sort = function (col) {
                if (_this.gridModel.sortKeyOrder.key === col) {
                    if (_this.gridModel.sortKeyOrder.order == "ASC")
                        _this.gridModel.sortKeyOrder.order = "DESC";
                    else
                        _this.gridModel.sortKeyOrder.order = "ASC";
                }
                else {
                    _this.gridModel.sortKeyOrder.key = col;
                    _this.gridModel.sortKeyOrder.order = "ASC";
                }
                _this.loadGrid();
            };
            this.search = function () {
                _this.loadGrid();
            };
            this.loadGrid = function () {
                var self = _this;
                var model = {
                    currentPage: _this.gridModel.currentPage,
                    recordsPerPage: _this.gridModel.recordsPerPage,
                    sortKey: _this.gridModel.sortKeyOrder.key,
                    sortOrder: _this.gridModel.sortKeyOrder.order,
                    searchfor: _this.gridModel.searchFor,
                    showInactive: _this.gridModel.showInactive
                };
                if (_this.customSearchModel) { //attach extra properties to the posted model
                    _this.customSearchModel(model);
                }
                _this.searchFunction(model)
                    .then(function (result) {
                    self.gridModel.data = result.results;
                    self.gridModel.originalData = angular.copy(result.results);
                    self.gridModel.totalItems = result.recordCount;
                    self.onDataLoaded(self.gridModel);
                }, function (error) {
                    console.log("------------Error------------");
                    console.log(error);
                    console.log("------------Error------------");
                    _this.$state.go("mainState.home");
                });
            };
            this.pageChanged = function () {
                _this.loadGrid();
            };
            this.dataLoadEvent = function () {
                _this.onDataLoaded(_this.gridModel);
            };
            this.gridModel = new TcrGridModel();
            this.gridModel.data = [];
            this.gridModel.originalData = [];
            this.gridModel.totalItems = 0;
            this.gridModel.currentPage = 1;
            this.gridModel.maxSize = 5;
            this.gridModel.recordsPerPage = 100;
            this.gridModel.searchFor = "";
            this.gridModel.showInactive = false;
            this.gridModel.sortKeyOrder = { key: this.defaultSortColumnName, order: "ASC" };
        }
        return TcrGridService;
    }());
    TcrGridServiceModule.TcrGridService = TcrGridService;
    ;
    function getInstance(defaultSortColumnName, searchFunction, onDataLoaded, customSearchModel, onGetMyModel, gridModel, $state) {
        return new TcrGridService(defaultSortColumnName, searchFunction, onDataLoaded, customSearchModel, onGetMyModel, $state);
    }
    angular.module("AngularApp")
        .factory("TcrGridService", [
        getInstance
    ]);
})(TcrGridServiceModule || (TcrGridServiceModule = {}));
//# sourceMappingURL=~TcrGridService.js.map