var CHControllerBase = /** @class */ (function () {
    function CHControllerBase(baseScope, basePopups, state) {
        var _this = this;
        this.baseScope = baseScope;
        this.basePopups = basePopups;
        this.state = state;
        this.handleError = function (error) {
            _this.basePopups.showError(_this.baseScope, error);
            console.log("------------Error------------");
            console.log(error);
            console.log("------------Error------------");
        };
    }
    return CHControllerBase;
}());
;
//# sourceMappingURL=~CHControllerBase.js.map