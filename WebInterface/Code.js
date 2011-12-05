var markersArray = [];
var polyArray = [];

///
/// Clears all the overlays on the map.
///
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
    var result ="undefined";
    if (ready == "true") {
        result = next('true');
    }
    if (result == "false") {
        setTimeout(auto, 50);
    }
}

function next(param) {
    $('#nextStepButton').attr("disabled", true);
    ready = false;

    //$('#iterationCount').text(iterationCount++);

    $.getJSON("./GetNext.aspx?next=" + param,
              {},
              function (data) {
                  
                  if (data.success == "true") {

                      ('#innerSideHatch').append("<div>Best path found!</div>");
                      return "true";
                  }
                  clearOverlays();
                  var routeNo = 0;
                  if (data.iteration) {
                      $('#iterationCount').text(data.iteration);
                  }
                  else {
                      $('#iterationCount').text('0');
                  }
                  $.each(data.paths, function (i, route) {
                      //alert(Route);

                      var polyPoints = new Array();
                      $('#dirList .directionEntry').remove();
                      $('#dirList .directionEntryAlt').remove();

                      var alt = false;
                      $.each(route.Route, function (j, node) {




                          //alert(node);

                          var myLatlng = new window.google.maps.LatLng(node.Latitude, node.Longitude);

                          polyPoints.push(myLatlng);
                          var marker = new window.google.maps.Marker({
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





                      });

                      var polyPath;
                      if (routeNo++ == 0) {
                          polyPath = new window.google.maps.Polyline({
                              path: polyPoints,
                              strokeColor: "#FF0000",
                              strokeOpacity: 0.6,
                              strokeWeight: 2
                          });
                      }
                      else {
                          polyPath = new window.google.maps.Polyline({
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
                  return "false";
              });

}


