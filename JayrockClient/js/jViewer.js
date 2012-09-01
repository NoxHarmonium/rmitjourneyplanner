if (googleEnabled)
{

///
/// Fields
///
var mapOptions = {
      center: new google.maps.LatLng(-34.397, 150.644),
      zoom: 8,
      mapTypeId: google.maps.MapTypeId.ROADMAP
    };
    
var map;


///
/// Custom functions
///    
jQuery.fn.refresh = function()
{
	map = new google.maps.Map(document.getElementById("map_canvas"),
        mapOptions);
}    

    
///
/// Event triggers
///
$(document).ready(function()
{
	
});

}
else
{
	$('#map_canvas').text('Google maps requires a connection to the internet and is disabled.');
}
