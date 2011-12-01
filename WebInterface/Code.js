var markersArray = [];
var polyArray = [];
function clearOverlays() {
    if (markersArray) {
        var marker = markersArray.pop();
        while (marker != undefined) {
            marker.setMap(null);
            marker = markersArray.pop();

        }

    }
    if (polyArray) {
        var poly = polyArray.pop();
        while (poly != undefined) {
            poly.setMap(null);
            poly = polyArray.pop();

        }

    }
}

var iterationCount = 0;
var ready = true;

function auto() {
    if (ready == true) {
        next();
    }
    setTimeout(auto, 50);

}

function next() {
    $('#nextStepButton').attr("disabled", true);
    ready = false;

    $('#iterationCount').text(iterationCount++);

    $.getJSON("./GetNext.aspx",
              {},
              function (data) {
                  clearOverlays();
                  var routeNo = 0;
                  $.each(data.paths, function (i, Route) {
                      //alert(Route);

                      var polyPoints = new Array();
                      $('#dirList li').remove()
                      $.each(Route.Route, function (j, node) {




                          //alert(node);

                          var myLatlng = new google.maps.LatLng(node.Latitude, node.Longitude);
                          polyPoints.push(myLatlng);
                          var marker = new google.maps.Marker({
                              position: myLatlng,
                              map: window.map,
                              title: node.Name
                          });

                          markersArray.push(marker);

                          var item = $(document.createElement('li'));
                          item.text(node.Type + node.Name + " (" + node.TotalTime + ")");
                          $('#dirList').append(item);

                          //$('#dirList').attr("disabled", true);





                      });

                      var polyPath;
                      if (routeNo++ == 0) {
                          polyPath = new google.maps.Polyline({
                              path: polyPoints,
                              strokeColor: "#FF0000",
                              strokeOpacity: 0.6,
                              strokeWeight: 2
                          });
                      }
                      else {
                          polyPath = new google.maps.Polyline({
                              path: polyPoints,
                              strokeColor: "#0000FF",
                              strokeOpacity: 0.6,
                              strokeWeight: 2
                          });
                      }
                      polyPath.setMap(window.map);
                      polyArray.push(polyPath);

                  });
                  $('#nextStepButton').attr("disabled", false);
                  ready = true;

              });

}


