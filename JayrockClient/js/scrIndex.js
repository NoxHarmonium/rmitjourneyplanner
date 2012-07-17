/* Author:
	Sean Dawson
*/
//Constants
var url_service = '/jsonws.ashx';


//Autocomplete settings.
var acDefaultBounds = new google.maps.LatLngBounds(
			  new google.maps.LatLng(-36.536123, 143.074951),
			  new google.maps.LatLng(-39.019184, 147.304688));

var acOptions = {
    bounds: acDefaultBounds,
    types: []
};


function RPCCall(method,params,callback) {
    var request = {};
    request.method = method;
    request.params = params;
    //request.params.CID = "45d0677d-a336-463b-ad99-c82137d03a00";
    //request.params.baseDN = "ou=people,dc=example,dc=com";
    //request.params.scope = "ONE";
    //request.params.filter = "(givenName=John)";
    request.id = 1;
    request.jsonrpc = "2.0";

    //
    $.post(url_service , $.toJSON(request), callback, "json");
}
String.prototype.bool = function () {
    return (/^True$/i).test(this);
};


RPCCall('LoadProviders', {}, function () {
    RPCCall('GetProperties', {}, function (data) {

        for (var i = 0; i < data.result.length; i++) {
            var propertyInfo = data.result[i];
            var newdiv = jQuery(document.createElement('div'));
            newdiv.addClass('control-group');
            var label = jQuery(document.createElement('label'));
            label.addClass('control-label');
            label.attr('for', 'txt' + String(i));
            label.html(propertyInfo.name);

            var controls = jQuery(document.createElement('div'));
            controls.addClass('controls');

            if (propertyInfo.type == 'DateTime') {
                controls.addClass('input-append');
                controls.addClass('date');

                controls.attr('id', 'date' + String(i));
                controls.attr('data-date', '20-03-2012');
                controls.attr('data-date-format', 'dd-mm-yyyy');

            }

            var input = jQuery(document.createElement('input'));
            if (propertyInfo.type == 'Boolean') {
                input.attr('type', 'checkbox');
                var b = String(propertyInfo.value).bool;
                if (b == true) {
                    input.attr('checked', '');
                }
            } else {
                input.attr('type', 'text');
            }



            input.addClass('input');
            input.attr('value', propertyInfo.value);
            input.attr('id', 'txt' + String(i));

            if (propertyInfo.type == 'DateTime') {
                input.attr('style', 'width: 90px;');
                input.attr('readonly', '');
                input.addClass('datePickerInput');

                input.val($.format.date(new Date(), "hh:mm a"));

            }

            //propertyInfo.type

            /*
            <div class="controls input-append date" id="dp3" data-date="20-03-2012" data-date-format="dd-mm-yyyy">
            <input id="txtDate" class="span2 input-xlarge" type="text" value="12-02-2012" readonly >
            <span class="add-on"><i class="icon-th"></i></span>
            </div>						
				
            </div>

            */

            newdiv.append(label);
            newdiv.append(controls);
            controls.append(input);

            if (propertyInfo.type == 'DateTime') {
                controls.append('<span class="add-on"><i class="icon-th"></i></span>');
                controls.append('<input type=\'text\' value=\'Text\' class=\'input-append\' data-provide="timepicker" style=\'width: 90px;\'  />');
                controls.append('<span class="add-on"><i class="icon-time"></i></span>');
                controls.datepicker();
            }

            $("#divAutoForm").append(newdiv);



        }


    });
});





