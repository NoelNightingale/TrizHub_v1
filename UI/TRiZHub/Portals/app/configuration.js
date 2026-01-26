"use strict";

angular.module("config", [])
    .constant("ENV",
    {
        serverLocation: baseUrl !== "/" ? baseUrl : "",
    });