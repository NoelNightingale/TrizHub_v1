class TcrGridModel {
    data: any;
    originalData: any;
    totalItems: number;
    currentPage: number;
    maxSize: number;
    recordsPerPage: number;
    numberOfPageButtons: number;
    searchFor: string;
    showInactive: boolean;
    sortKeyOrder: { key: string, order: string };
};

type callback = (data: TcrGridModel) => void;

module TcrGridServiceModule {

    export class TcrGridService {

        private gridModel: TcrGridModel;

        constructor(private defaultSortColumnName: any,
            private searchFunction: any,
            private onDataLoaded: callback,
            private customSearchModel: any,
            private onGetMyModel: any,
            private $state: ng.ui.IStateService) {

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


        sort = (col) => {
            if (this.gridModel.sortKeyOrder.key === col) {
                if (this.gridModel.sortKeyOrder.order == "ASC")
                    this.gridModel.sortKeyOrder.order = "DESC";
                else
                    this.gridModel.sortKeyOrder.order = "ASC";
            } else {
                this.gridModel.sortKeyOrder.key = col;
                this.gridModel.sortKeyOrder.order = "ASC";
            }
            this.loadGrid();
        };


        search = () => {
            this.loadGrid();
        };
        loadGrid = () => {
            const self = this;
            var model = {
                currentPage: this.gridModel.currentPage,
                recordsPerPage: this.gridModel.recordsPerPage,
                sortKey: this.gridModel.sortKeyOrder.key,
                sortOrder: this.gridModel.sortKeyOrder.order,
                searchfor: this.gridModel.searchFor,
                showInactive: this.gridModel.showInactive
            };
            if (this.customSearchModel) { //attach extra properties to the posted model
                this.customSearchModel(model);
            }


            this.searchFunction(model)
                .then(result => {
                        self.gridModel.data = result.results;
                        self.gridModel.originalData = angular.copy(result.results);
                        self.gridModel.totalItems = result.recordCount;

                        self.onDataLoaded(self.gridModel);
                    },
                    error => {
                        console.log("------------Error------------");
                        console.log(error);
                        console.log("------------Error------------");
                        this.$state.go("mainState.home");
                    });
        };

        pageChanged = () => {
            this.loadGrid();
        };


        dataLoadEvent = () => {
            this.onDataLoaded(this.gridModel);
        };
    };


    function getInstance(defaultSortColumnName: any,
        searchFunction: any,
        onDataLoaded: any,
        customSearchModel: any,
        onGetMyModel: any,
        gridModel: any,
        $state: any) {
        return new TcrGridService(defaultSortColumnName, searchFunction, onDataLoaded, customSearchModel, onGetMyModel, $state);
    }

    angular.module("AngularApp")
        .factory("TcrGridService",
        [
            getInstance
        ]);
}