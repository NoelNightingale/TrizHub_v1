angular.module("AngularApp")
    .factory("Popups",
        [
            "$uibModal", "$q",
            function ($uibModal, $q) {

                var _root = "Portals/app/popups/";
                const _confirmationDialog = function ($scope, header, question, yesButton, noButton) {
                    if (!header)
                        header = "Confirmation";
                    if (!question)
                        question = "Some question?";
                    if (!yesButton)
                        yesButton = "Yes";
                    if (!noButton)
                        noButton = "No";

                    var deferred = $q.defer();
                    $scope.popupModel = {
                        question: question,
                        header: header,
                        yesButton: yesButton,
                        noButton: noButton,
                        modalInstance:
                            $uibModal.open({
                                animation: false,
                                templateUrl: _root + "Confirmation.html",
                                scope: $scope,
                                size: "sm"
                            }),
                        noClick: function () {
                            this.modalInstance.close(false);
                            deferred.resolve(false);
                        },
                        yesClick: function () {
                            this.modalInstance.close(true);
                            deferred.resolve(true);
                        }
                    };

                    $scope.popupModel.modalInstance.result.then(
                        function (e) {
                        },
                        function (e) {
                            deferred.reject(e);
                        });

                    return deferred.promise;
                };
                const _confirmationReasonDialog = function ($scope, header, question, reasonPrompt, yesButton, noButton) {
                    if (!header)
                        header = "Confirmation";
                    if (!reasonPrompt)
                        reasonPrompt = "Reason";
                    if (!question)
                        question = "Some question?";
                    if (!yesButton)
                        yesButton = "Yes";
                    if (!noButton)
                        noButton = "No";

                    var deferred = $q.defer();
                    $scope.popupModel = {
                        question: question,
                        header: header,
                        yesButton: yesButton,
                        noButton: noButton,
                        reason: reasonPrompt,
                        reasonText: "",
                        modalInstance:
                            $uibModal.open({
                                animation: false,
                                templateUrl: _root + "ConfirmationReason.html",
                                scope: $scope,
                                size: "sm"
                            }),
                        noClick: function () {
                            const me = this;
                            me.modalInstance.close(true);
                            deferred.resolve(
                                {
                                    result: false,
                                    text: me.reasonText
                                });
                        },

                        yesClick: function () {
                            const me = this;
                            if (!me.reasonText) {
                                $scope.$broadcast("show-errors-check-validity");
                                return;
                            }
                            me.modalInstance.close(true);
                            const result = {
                                result: true,
                                text: me.reasonText
                            };
                            deferred.resolve(result);
                        }
                    };

                    $scope.popupModel.modalInstance.result.then(
                        function (e) {
                        },
                        function (e) {
                            deferred.reject(e);
                        });
                    return deferred.promise;
                };

                //#Region Timesheet Report Dailog
                const _showReportDateBetweenTimesheetViewDialog = function ($scope, header, question, viewLocation, yesButton, noButton, projectId, userId) {
                    if (!header)
                        header = "Confirmation";
                    if (!question)
                        question = "Some question?";
                    if (!yesButton)
                        yesButton = "Yes";
                    if (!noButton)
                        noButton = "No";
                    if (!projectId)
                        projectId = "";
                    if (!userId)
                        userId = "";

                    var deferred = $q.defer();
                    $scope.popupModel = {
                        question: question,
                        header: header,
                        yesButton: yesButton,
                        noButton: noButton,
                        startDate: null,
                        endDate: null,
                        projectId: null,
                        userId: null,
                        modalInstance:
                            $uibModal.open({
                                animation: false,
                                templateUrl: viewLocation,
                                scope: $scope,
                                size: "m"
                            }),
                        noClick: function () {
                            const me = this;
                            me.modalInstance.close(true);
                            deferred.resolve(
                                {
                                    result: false
                                });
                        },

                        yesClick: function () {
                            const me = this;
                            me.modalInstance.close(true);
                            const result = {
                                result: true,
                                startDate: me.startDate,
                                endDate: me.endDate,
                                projectId: me.projectId,
                                userId: me.userId,
                                showBillingPeriod: me.disableFilter,
                                showRates: me.showRates,
                            };
                            deferred.resolve(result);
                        }
                    };

                    $scope.popupModel.modalInstance.result.then(
                        function (e) {
                        },
                        function (e) {
                            deferred.reject(e);
                        });
                    return deferred.promise;
                };

                //#endregion

                //#region User Report Dailog
                const _showUserReportDailog = function ($scope, header, question, viewLocation, yesButton, noButton, userId, showInactive) {
                    if (!header)
                        header = "Confirmation";
                    if (!question)
                        question = "Some question?";
                    if (!yesButton)
                        yesButton = "Yes";
                    if (!noButton)
                        noButton = "No";
                    if (!userId)
                        userId = "";

                    var deferred = $q.defer();
                    $scope.popupModel = {
                        question: question,
                        header: header,
                        yesButton: yesButton,
                        noButton: noButton,
                        userId: null,
                        showInactive: null,
                        modalInstance:
                            $uibModal.open({
                                animation: false,
                                templateUrl: viewLocation,
                                scope: $scope,
                                size: "m"
                            }),
                        noClick: function () {
                            const me = this;
                            me.modalInstance.close(true);
                            deferred.resolve(
                                {
                                    result: false
                                });
                        },

                        yesClick: function () {
                            const me = this;
                            me.modalInstance.close(true);
                            const result = {
                                result: true,
                                userId: me.userId,
                                showInactive: me.showInactive,
                            };
                            deferred.resolve(result);
                        }
                    };

                    $scope.popupModel.modalInstance.result.then(
                        function (e) {
                        },
                        function (e) {
                            deferred.reject(e);
                        });
                    return deferred.promise;
                };
                //#endregion

                //#Region Scorecard Report Dailog
                const _showScorecardReportDailog = function ($scope, header, question, viewLocation, yesButton, noButton, scorecardTemplate, period, employee) {
                    if (!header)
                        header = "Confirmation";
                    if (!question)
                        question = "Some question?";
                    if (!yesButton)
                        yesButton = "Yes";
                    if (!noButton)
                        noButton = "No";
                    if (!scorecardTemplate)
                        scorecardTemplate = "";
                    if (!period)
                        period = "";
                    if (!employee)
                        employee = "";


                    var deferred = $q.defer();
                    $scope.popupModel = {
                        question: question,
                        header: header,
                        yesButton: yesButton,
                        noButton: noButton,
                        scorecardTemplate: null,
                        period: null,
                        employee: null,
                        modalInstance:
                            $uibModal.open({
                                animation: false,
                                templateUrl: viewLocation,
                                scope: $scope,
                                size: "m"
                            }),
                        noClick: function () {
                            const me = this;
                            me.modalInstance.close(true);
                            deferred.resolve(
                                {
                                    result: false
                                });
                        },

                        yesClick: function () {
                            const me = this;
                            me.modalInstance.close(true);
                            const result = {
                                result: true,
                                scorecardTemplate: me.scorecardTemplate,
                                period: me.period,
                                employee: me.employee,
                            };
                            deferred.resolve(result);
                        }
                    };

                    $scope.popupModel.modalInstance.result.then(
                        function (e) {
                        },
                        function (e) {
                            deferred.reject(e);
                        });
                    return deferred.promise;
                };
                //#endregion

                //#region Time Sheet Records Template Dialog
                const _timeSheetRecordDailog = function ($scope, header, yesButton, noButton, project, startDate, endDate, team, activity) {
                    if (!header)
                        header = "Add Records";
                    if (!yesButton)
                        yesButton = "Add";
                    if (!noButton)
                        noButton = "Cancel";
                    if (!project)
                        project = "";
                    if (!startDate)
                        startDate = Date.now();
                    if (!endDate)
                        endDate = Date.now();
                    if (!team)
                        team = "";
                    if (!activity)
                        activity = "";

                    var deferred = $q.defer();
                    $scope.popupModel = {
                        header: header,
                        yesButton: yesButton,
                        noButton: noButton,
                        project: {},
                        team: null,
                        activity: null,
                        startDate: new Date(),
                        endDate: null,
                        modalInstance:
                            $uibModal.open({
                                animation: false,
                                templateUrl: _root + "TimeSheetAddRecords.html",
                                scope: $scope,
                                size: "m"
                            }),
                        noClick: function () {
                            const me = this;
                            me.modalInstance.close(true);
                            deferred.resolve(
                                {
                                    result: false
                                });
                        },

                        yesClick: function () {
                            const me = this;
                            me.modalInstance.close(true);
                            const result = {
                                result: true,
                                project: me.project,
                                startDate: me.startDate,
                                endDate: me.endDate,
                                team: me.team,
                                activity: me.activity,
                                disableFilter: me.disableFilter
                            };
                            deferred.resolve(result);
                        }
                    };

                    $scope.popupModel.modalInstance.result.then(
                        function (e) {
                        },
                        function (e) {
                            deferred.reject(e);
                        });
                    return deferred.promise;
                };

                //#endregion

                //#Region User Asset Register CSV Import Daialog
                const _showUserAssetRegisterImportDialog = function ($scope, header, question, viewLocation, yesButton, noButton) {
                    if (!header)
                        header = "Confirmation";
                    if (!question)
                        question = "Some question?";
                    if (!yesButton)
                        yesButton = "Upload";
                    if (!noButton)
                        noButton = "Cancel";



                    var deferred = $q.defer();
                    $scope.popupimportModel = {
                        question: question,
                        header: header,
                        yesButton: yesButton,
                        noButton: noButton,
                        modalInstance:
                            $uibModal.open({
                                animation: false,
                                templateUrl: viewLocation,
                                scope: $scope,
                                size: "m"
                            }),
                        noClick: function () {
                            const me = this;
                            me.modalInstance.close(true);
                            deferred.resolve(
                                {
                                    //  result: false
                                });
                        },

                        yesClick: function () {
                            const me = this;
                            me.modalInstance.close(true);
                            const result = {
                                result: true,
                            };
                            deferred.resolve(result);
                        }
                    };

                    $scope.popupimportModel.modalInstance.result.then(
                        function (e) {
                        },
                        function (e) {
                            deferred.reject(e);
                        });
                    return deferred.promise;
                };
                //#endregion


                const _notificationDialog = function ($scope, text, header, okButton) {

                    if (!header)
                        header = "Information";
                    if (!text)
                        text = "Notification Text";
                    if (!okButton)
                        okButton = "Ok";

                    var deferred = $q.defer();
                    $scope.popupModel = {
                        text: text,
                        header: header,
                        okButton: okButton,
                        modalInstance:
                            $uibModal.open({
                                animation: false,
                                templateUrl: _root + "Notification.html",
                                scope: $scope,
                                size: "sm"
                            }),
                        okClick: function () {
                            this.modalInstance.close(true);
                            deferred.resolve(true);
                        }
                    };
                    $scope.popupModel.modalInstance.result.then(
                        function (e) {
                        },
                        function (e) {
                            deferred.reject(e);
                        });
                    return deferred.promise;
                };
                const _showError = function ($scope, error, header, okButton) {

                    if (!header)
                        header = "Warning! Something went wrong.";
                    if (!okButton)
                        okButton = "Ok";


                    var deferred = $q.defer();
                    $scope.popupModel = {
                        notification: error.toString(),
                        header: header,
                        okButton: okButton,
                        modalInstance:
                            $uibModal.open({
                                animation: false,
                                templateUrl: _root + "Error.html",
                                scope: $scope,
                                size: "lg"
                            }),
                        okClick: function () {
                            this.modalInstance.close(true);
                            deferred.resolve(true);
                        }
                    };
                    $scope.popupModel.modalInstance.result.then(
                        function (e) {
                        },
                        function (e) {
                            deferred.reject(e);
                        });
                    return deferred.promise;
                };

                const _filteredDropDownModal = function ($scope, dropDownList) {
                    var deferred = $q.defer();

                    $scope.dropDownList = dropDownList;

                    $scope.popupModel1 = {
                        modalInstance:
                            $uibModal.open({
                                animation: false,
                                templateUrl: _root + "FilteredDropDownModal.html",
                                scope: $scope,
                                size: "lg"
                            }),
                        itemChoosen: function (project) {
                            this.modalInstance.close(true);                            
                            deferred.resolve(project);
                        }
                    };

                    $scope.popupModel1.modalInstance.result.then(
                        function (e) {
                        },
                        function (e) {
                        });

                    return deferred.promise;
                };

                const _scorecardReassignDailog = function ($scope, header, yesButton, noButton, scorecard, users) {
                    if (!header)
                        header = "Reassign Scorecard";
                    if (!yesButton)
                        yesButton = "Ok";
                    if (!noButton)
                        noButton = "Cancel";

                    $scope.scorecard = scorecard;
                    $scope.users = users;

                    var deferred = $q.defer();
                    $scope.popupModel = {
                        header: header,
                        yesButton: yesButton,
                        noButton: noButton,
                        evaluator: {},
                        scorecard: scorecard,
                        modalInstance:
                            $uibModal.open({
                                animation: false,
                                templateUrl: _root + "ScorecardReassign.html",
                                scope: $scope,
                                size: "m"
                            }),
                        noClick: function () {
                            const me = this;
                            me.modalInstance.close(true);
                            deferred.resolve(
                                {
                                    result: false
                                });
                        },

                        yesClick: function () {
                            const me = this;
                            me.modalInstance.close(true);
                            const result = {
                                evaluator: me.evaluator,
                                scorecard: me.scorecard
                            };
                            deferred.resolve(result);
                        }
                    };

                    $scope.popupModel.modalInstance.result.then(
                        function (e) {
                        },
                        function (e) {
                            deferred.reject(e);
                        });
                    return deferred.promise;
                };

                const _scorecardDefinitionDailog = function ($scope, $timeout, header, yesButton, definition) {
                    if (!header)
                        header = "Definition";
                    if (!yesButton)
                        yesButton = "Ok";

                    var deferred = $q.defer();
                    $scope.popupModel = {
                        summernoteOptions: {
                            height: 110,
                            focus: false,
                            airMode: false,
                            shortcuts: false,
                            toolbar: [],
                            disableDragAndDrop: true
                        },
                        header: header,
                        yesButton: yesButton,
                        definition: definition,
                        modalInstance:
                            $uibModal.open({
                                animation: false,
                                templateUrl: _root + "ScorecardDefinition.html",
                                scope: $scope,
                                size: "m"
                            }),
                        yesClick: function () {
                            const me = this;
                            me.modalInstance.close(true);
                            deferred.resolve({result: false});
                        }
                    };

                    $scope.popupModel.modalInstance.result.then(
                        function (e) {
                        },
                        function (e) {
                            deferred.reject(e);
                        });
                    return deferred.promise;
                };

                return {
                    confirmationDialog: _confirmationDialog,
                    confirmationReasonDialog: _confirmationReasonDialog,
                    notificationDialog: _notificationDialog,
                    showError: _showError,
                    showReportDateBetweenTimesheetViewDialog: _showReportDateBetweenTimesheetViewDialog,
                    showUserReportDailog: _showUserReportDailog,
                    timeSheetRecordDailog: _timeSheetRecordDailog,
                    showScorecardReportDailog: _showScorecardReportDailog,
                    filteredDropDownModal: _filteredDropDownModal,
                    showUserAssetRegisterImportDialog: _showUserAssetRegisterImportDialog,
                    scorecardReassignDailog: _scorecardReassignDailog,
                    scorecardDefinitionDailog: _scorecardDefinitionDailog
                };
            }
        ]);