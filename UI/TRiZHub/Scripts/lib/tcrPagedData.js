function tcrPagedData(recordsInPage,
    searchFunction,
    onDataLoaded,
    customSearchModel,
    onGetMyModel) {

    var pageModel = {
        data: [],
        totalItems: 0,
        currentPage: 1,
        recordsPerPage: recordsInPage,
        numberOfPageButtons: 5
    };

    if (onGetMyModel) {
        pageModel = onGetMyModel();
    }

    var _loadGrid = function() {
        var model = {
            currentPage: pageModel.currentPage,
            recordsPerPage: pageModel.recordsPerPage,
        };
        if (customSearchModel) { //attach extra properties to the posted model
            customSearchModel(model);
        }

        searchFunction(model)
            .then(
                function(result) {
                    pageModel.data = result.results;
                    pageModel.totalItems = result.recordCount;
                    onDataLoaded(pageModel);
                },
                function(error) {
                    //    alert("an error occured: unable to get data");
                });
    };

    var _pageChanged = function() {
        _loadGrid();
    };


    onDataLoaded(pageModel);

    return {
        load: _loadGrid,
        pageChanged: _pageChanged
    };
}