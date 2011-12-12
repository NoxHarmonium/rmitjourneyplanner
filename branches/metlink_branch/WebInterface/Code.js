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

next('false');

function auto() {
    if (ready == true) {
        next('true');
    }
    setTimeout(auto, 50);

}

function next(param) {
    $('#nextStepButton').attr("disabled", true);
    ready = false;

    //$('#iterationCount').text(iterationCount++);

    $.getJSON("./GetNext.aspx?next=" + param,
              {},
              function (data) {
                  clearOverlays();
                  var routeNo = 0;
                  if (data.success == "true") {

                      ('#innerSideHatch').append("<div>Best path found!</div>");
                      return;
                  }

                  if (data.iteration) {
                      $('#iterationCount').text(data.iteration);
                  }
                  else {
                      $('#iterationCount').text('0');
                  }
                  $.each(data.paths, function (i, Route) {
                      //alert(Route);
                      var pColor = undefined;
                      var polyPoints = new Array();

                      if (routeNo == 0) {
                          $('#dirList .directionEntry').remove();
                          $('#dirList .directionEntryAlt').remove();
                      }

                      var alt = false;
                      $.each(Route.Route, function (j, node) {

                          var myLatlng = new google.maps.LatLng(node.Latitude, node.Longitude);

                          polyPoints.push(myLatlng);
                         

                          

                          pColor = node.Colour;
                          
                          if (routeNo == 0) {

                              var marker = new google.maps.Marker({
                                  position: myLatlng,
                                  map: window.map,
                                  title: node.Name,
                                  icon: node.Image
                              });
                              markersArray.push(marker);
                              var item = $(document.createElement('div'));
                              var type = $(document.createElement('div'));
                              var name = $(document.createElement('div'));
                              var time = $(document.createElement('div'));
                              //var image = $(document.createElement('img'));
                              //image.attr("href") = node.Image;
                              item.append("<img class='transportImage' src='" + node.Image + "'/>");

                             

                              type.text(node.Type);
                              name.text(node.Name);
                              time.text(" (" + node.TotalTime + ")");



                              type.attr('class', 'typeSpan');
                              name.attr('class', 'nameSpan');
                              time.attr('class', 'timeSpan');

                              item.append(type);
                              item.append(name);
                              item.append(time);

                              if (alt) {
                                  item.attr('class', 'directionEntry');
                              }
                              else {
                                  item.attr('class', 'directionEntryAlt');

                              }
                              alt = !alt;
                              $('#dirList').append(item);

                              //$('#dirList').attr("disabled", true);
                          }




                      });
                      var polyPath;

                      if (pColor) {
                          polyPath = new google.maps.Polyline({
                              path: polyPoints,
                              strokeColor: String(pColor),
                              strokeOpacity: 0.5,
                              strokeWeight: 2
                          });
                          routeNo++;
                      }
                      else {
                          if (routeNo++ == 0) {
                              polyPath = new google.maps.Polyline({
                                  path: polyPoints,
                                  strokeColor: "#FF0000",
                                  strokeOpacity: 0.5,
                                  strokeWeight: 2
                              });
                          }
                          else {
                              polyPath = new google.maps.Polyline({
                                  path: polyPoints,
                                  strokeColor: "#0000FF",
                                  strokeOpacity: 0.5,
                                  strokeWeight: 2
                              });
                          }

                      }


                      polyPath.setMap(window.map);
                      polyArray.push(polyPath);

                  });
                  $('#nextStepButton').attr("disabled", false);
                  ready = true;

              });

}


