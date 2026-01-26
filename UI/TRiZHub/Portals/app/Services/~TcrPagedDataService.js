var TcrPagedDataModel = /** @class */ (function () {
    function TcrPagedDataModel() {
    }
    return TcrPagedDataModel;
}());
;
var TcrPagedDataServiceModule;
(function (TcrPagedDataServiceModule) {
    var TcrPagedDataService = /** @class */ (function () {
        function TcrPagedDataService(recordsPerPage, searchFunction, onDataLoaded, customSearchModel, onGetMyModel) {
            var _this = this;
            this.searchFunction = searchFunction;
            this.onDataLoaded = onDataLoaded;
            this.customSearchModel = customSearchModel;
            this.onGetMyModel = onGetMyModel;
            this.loadList = function () {
                var self = _this;
                var model = {
                    currentPage: _this.pagedDataModel.currentPage,
                    recordsPerPage: _this.pagedDataModel.recordsPerPage,
                };
                if (_this.customSearchModel) { //attach extra properties to the posted model
                    _this.customSearchModel(model);
                }
                _this.searchFunction(model)
                    .then(function (result) {
                    self.pagedDataModel.data = result.results;
                    self.pagedDataModel.totalItems = result.recordCount;
                    self.onDataLoaded(self.pagedDataModel);
                }, function (error) {
                    //    alert("an error occured: unable to get data");
                });
            };
            this.pageChanged = function () {
                _this.loadList();
            };
            this.dataLoadEvent = function () {
                _this.onDataLoaded(_this.pagedDataModel);
            };
            this.pagedDataModel = new TcrPagedDataModel();
            this.pagedDataModel.data = [];
            this.pagedDataModel.totalItems = 0;
            this.pagedDataModel.currentPage = 1;
            this.pagedDataModel.recordsPerPage = recordsPerPage;
            this.pagedDataModel.numberOfPageButtons = 5;
        }
        return TcrPagedDataService;
    }());
    TcrPagedDataServiceModule.TcrPagedDataService = TcrPagedDataService;
    ;
    function getInstance(recordsPerPage, searchFunction, onDataLoaded, customSearchModel, onGetMyModel) {
        return new TcrPagedDataService(recordsPerPage, searchFunction, onDataLoaded, customSearchModel, onGetMyModel);
    }
    angular.module("AngularApp").factory("TcrPagedDataService", [getInstance]);
})(TcrPagedDataServiceModule || (TcrPagedDataServiceModule = {}));
//# sourceMappingURL=~TcrPagedDataService.js.map