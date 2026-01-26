angular.module("AngularApp")
    .directive("ngFileModel",
    [
        "$parse", function($parse) {
            return {
                restrict: "A",
                link: function(scope, element, attrs) {
                    const model = $parse(attrs.ngFileModel);
                    var modelSetter = model.assign;
                    element.bind("change",
                        function(changeEvent) {
                            const reader = new FileReader();
                            reader.onload = function(loadEvent) {
                                scope.$apply(function() {
                                    const fileObject = {
                                        fileName: element[0].files[0].name,
                                        fileDataBase64: loadEvent.target.result
                                    };
                                    modelSetter(scope, fileObject);
                                });
                            };
                            reader.readAsDataURL(changeEvent.target.files[0]);
                        });
                }
            };
        }
    ])