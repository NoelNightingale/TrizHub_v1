class TcrPagedDataModel {
    data: any;
    totalItems: number;
    currentPage: number;
    recordsPerPage: number;
    numberOfPageButtons: number;
};

type TcrPagedDataCallback = (data: TcrPagedDataModel) => void;

module TcrPagedDataServiceModule {

    export class TcrPagedDataService {

        private pagedDataModel: TcrPagedDataModel;

        constructor(recordsPerPage: number,
            private searchFunction: any,
            private onDataLoaded: TcrPagedDataCallback,
            private customSearchModel: any,
            private onGetMyModel: any) {

            this.pagedDataModel = new TcrPagedDataModel();

            this.pagedDataModel.data = [];
            this.pagedDataModel.totalItems = 0;
            this.pagedDataModel.currentPage = 1;
            this.pagedDataModel.recordsPerPage = recordsPerPage;
            this.pagedDataModel.numberOfPageButtons = 5;
        }

        loadList = () => {
            const self = this;
            const model = {
                currentPage: this.pagedDataModel.currentPage,
                recordsPerPage: this.pagedDataModel.recordsPerPage,
            };
            if (this.customSearchModel) { //attach extra properties to the posted model
                this.customSearchModel(model);
            }

            this.searchFunction(model)
                .then(
                    result => {
                        self.pagedDataModel.data = result.results;
                        self.pagedDataModel.totalItems = result.recordCount;

                        self.onDataLoaded(self.pagedDataModel);
                    },
                    error => {
                        //    alert("an error occured: unable to get data");
                    });
        };

        pageChanged = () => {
            this.loadList();
        };

        dataLoadEvent = () => {
            this.onDataLoaded(this.pagedDataModel);
        };
    };

    function getInstance(recordsPerPage: number,
        searchFunction: any,
        onDataLoaded: TcrPagedDataCallback,
        customSearchModel: any,
        onGetMyModel: any) {

        return new TcrPagedDataService(recordsPerPage, searchFunction, onDataLoaded, customSearchModel, onGetMyModel);
    }

    angular.module("AngularApp").factory("TcrPagedDataService", [getInstance]);
}