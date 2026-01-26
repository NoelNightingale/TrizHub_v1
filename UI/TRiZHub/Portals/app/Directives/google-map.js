angular.module("AngularApp")
    .directive("map",
    [
        "$parse", function($parse) {
            return {
                restrict: "E",
                replace: true,
                template: "<div></div>",
                link: function(scope, element, attrs) {
                    console.log(element);
                    const myOptions = {
                        zoom: 6,
                        center: new google.maps.LatLng(46.87916, -3.32910),
                        mapTypeId: google.maps.MapTypeId.ROADMAP,
                        disableDefaultUI: true,
                    };
                    var map = new google.maps.Map(document.getElementById(attrs.id), myOptions);
                    var currentMaker = null;


                    google.maps.event.addListener(map,
                        "click",
                        function(e) {
                            scope.$apply(function() {


                                addMarker({
                                    lat: e.latLng.lat(),
                                    lng: e.latLng.lng()
                                });

                                console.log(e);
                            });

                        }); // end click listener

                    addMarker = function(pos) {
                        if (currentMaker != null) {
                            currentMaker.setMap(null);

                        }
                        const myLatlng = new google.maps.LatLng(pos.lat, pos.lng);
                        const marker = new google.maps.Marker({
                            id: 0,
                            position: myLatlng,
                            map: map,
                            title: "Hello World!",
                            draggable: true

                        });
                        currentMaker = marker;

                    }; //end addMarker

                }
            };
        }
    ]);