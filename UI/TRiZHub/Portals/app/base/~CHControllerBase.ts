class CHControllerBase {

    constructor(private baseScope: ng.IScope, private basePopups: any, private state: ng.ui.IStateService) {
    }

    handleError = (error: string) => {
        this.basePopups.showError(this.baseScope, error);
        console.log("------------Error------------");
        console.log(error);
        console.log("------------Error------------");
    };
};