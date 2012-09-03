if (googleEnabled)
{

    ///
    /// Fields
    ///
    var defaultBounds = new google.maps.LatLngBounds(
			  new google.maps.LatLng(-36.536123, 143.074951),
			  new google.maps.LatLng(-39.019184, 147.304688));

    var mapOptions = {
        bounds: defaultBounds,
        types: [],
        center: new google.maps.LatLng(-37.810191, 144.962511),
        zoom: 11,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    };
    
    
    var map;


    ///
    /// Custom functions
    ///    
    refreshMap = function()
    {
	    map = new google.maps.Map(document.getElementById("map_canvas"),
            mapOptions);
    };
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
