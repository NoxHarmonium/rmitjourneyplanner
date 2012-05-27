/* Author:
Sean Dawson
*/
var polyLines = new Array();
var markers = new Array();
var autorun = false;
var drawLimit = 999999;
var biDir = false;
var selectedItems = null;

//$('#btnStep').button();

var colours =
    [
        "#3270FF",
        "#C65EFF",
        "#40A542",
        "#FF0000",
        "#198C8E",
        "#006822"
    ];

$('#divProgress').hide();
//$('#divResults').hide();
$('#lstPopulation').hide();
$('#frmPopControls').hide();
$('#tabResults').hide();
$('#tabRouteExplorer').hide();
$('#txtError').hide();
$('#txtPEtotal').attr("disabled", "disabled");

var id;

function processPopulation(data) {



    $('#selPopulation').children().remove();
    var split = data.split(',');
    for (var n in split) {
        // $('#selPopulation').append("<option value=\"" + n + "\">" + split[n] + "</option>");
        $('#selPopulation').append("<option label=\"" + n + "\">" + split[n] + "</option>");
    }

    if (selectedItems != null) {
        for (var item in selectedItems) {
            $('option[label="' + selectedItems[item] + '"]').attr('selected', 'selected');
        }
    }
    refreshPaths();
    //selectedItems = new Array();
    //$('#selPopulation').val(selectedItems);
    //$('#lstPopulation').show();
    if (!autorun) {
        $('#frmPopControls').show();
        $('#divProgress').hide();
        $('#txtStatus').text('Ready...');
    }
}

function handleStatus(data) {
    //alert(data);

    var split = data.split(',');
    if (split[0] == '1') {
        $.get('aspx/Reset.aspx');
        id = setInterval('updateStatus()', 500);
        $('#divProgress').hide();

    }
    else
        if (split[0] == '2') {
            $('#divProgress').show();
            $('#txtStatus').text('Loading...');
            if (autorun) {
                $('#txtProgress').html('Iteration: ' + split[3] + '<br/>');
            } else {

                var percent = (parseFloat(split[1]) / parseFloat(split[2])) * 100.0;
                $('#txtProgress').html('Iteration: ' + split[3] + '<br/>Generating route ' + split[1] + ' out of ' + split[2]);


                $('#progressBar').css('width', String(parseInt(percent)) + '%');
            }


            id = setInterval('updateStatus()', 300);

        }
        else
            if (split[0] == '3') {
                $.get('aspx/GetPopulation.aspx', processPopulation);
                if (autorun) {
                    step();
                }
            }
}

$.get('aspx/CheckStatus.aspx', handleStatus);

function updateStatus() {
    //$('#divResults').hide();
    $('#lstPopulation').hide();
    $('#frmPopControls').hide();
    $.get('aspx/CheckStatus.aspx', handleStatus);
    clearInterval(id);
}

function processPaths(data) {
    var paths = data.split('#');
    for (var j = 0; j < polyLines.length; j++) {
        polyLines[j].setMap(null);
    }
    polyLines = new Array();
    $('.resultRow').remove();
    for (var i = 0; i < paths.length - 1; i++) {
        var nodes = paths[i].split(';');
        var points = new Array();
        var points2 = new Array();
        $('#txtPEtotal').val(nodes.length);
        var oldRouteId = -1;
        var routeId;
        var pos;
        var className;
        if (biDir) {
            for (var j = 0; j < (nodes.length / 2) + 1 && j < drawLimit / 2; j++) {
                pos = nodes[j].split(',');
                var pos2 = nodes[nodes.length - j - 1].split(',');
                points.push(new google.maps.LatLng(parseFloat(pos[0]), parseFloat(pos[1])));
                points2.push(new google.maps.LatLng(parseFloat(pos2[0]), parseFloat(pos2[1])));
                routeId = parseInt(pos[5]);
                className = 'resultRow';
                if (oldRouteId == routeId&& routeId != -1) {
                    className = 'resultRow subRoute';
                }


                $('#tblResults').append('<tr class=\'' + className + '\'><td>' + j + "</td><td>" + pos[2] + "</td><td>" + pos[3] + "</td><td>" + pos[4] + "</td><td>" + pos[5] + "</td></tr>");
                oldRouteId = routeId;
            }

        } else {
            for (var j = 0; j < nodes.length - 1 && j < drawLimit; j++) {
                pos = nodes[j].split(',');
                points.push(new google.maps.LatLng(parseFloat(pos[0]), parseFloat(pos[1])));
                routeId = parseInt(pos[5]);
                className = 'resultRow';
                if (oldRouteId == routeId && routeId != -1) {
                    className = 'resultRow subRoute';
                }

                oldRouteId = routeId;
                $('#tblResults').append('<tr class=\'' + className + '\'><td>' + j + "</td><td>" + pos[2] + "</td><td>" + pos[3] + "</td><td>" + pos[4] + "</td><td>" + pos[5] + "</td></tr>");


            }
        }

        var colour = colours[i];
        if (i > 5) {
            colour = "#000000";
        }
        var path = new google.maps.Polyline({
            path: points,
            strokeColor: colour,
            strokeOpacity: 0.8,
            strokeWeight: 3
        });
        path.setMap(map);
        if (biDir) {
            var path2 = new google.maps.Polyline({
                path: points2,
                strokeColor: colour,
                strokeOpacity: 0.8,
                strokeWeight: 3
            });
            path2.setMap(map);
            polyLines.push(path2);
        }

        polyLines.push(path);


    }
    $('.resultRow').unbind("mouseenter");
    $('.resultRow').unbind("mouseleave");
    $('.resultRow').mouseenter(function () {
        var index = parseInt($(this).children('td:first').text());
        var path = polyLines[0].getPath();
        markers.push(new google.maps.Marker({
            position: path.getAt(index),
            map: map,
            title: String(index)
        }));

        $('.resultRow').mouseleave(function () {
            while (markers.length > 0)
                var marker = markers.pop();
            if (marker != null) {
                marker.setMap(null);
            }


        });

    });

}

$('#selPopulation').change(function () {

    //alert($('option:selected', '#selPopulation').length);
    if ($('option:selected', '#selPopulation') != undefined)
        selectedItems = new Array();
    for (var i = 0; i < $('option:selected', '#selPopulation').length; i++) {
        //alert($('option:selected', '#selPopulation')[i]);
        selectedItems.push($('option:selected', '#selPopulation')[i].label);
    }
    //var params = '';
    //for (var n in $(this).val()) {
    //    params += $(this).val()[n].split(" ")[1] +',';
    //}
    //$.get('aspx/GetPath.aspx?members=' + params, processPaths);


    refreshPaths();
});

function stepExecuted(data) {


}

function stop() {
    autorun = false;
    $('#btnStep').button('reset');
    $('#btnReset').button('reset');
    $('#btnAuto').button('reset');
    $('#outerProgressBar').show();
}

function refreshPaths() {

    var params = '';
    for (var n in $('#selPopulation').val()) {
        params += $('#selPopulation').val()[n].split(" ")[1] + ',';
        //UNCOMMENT FOR SERVER LIMITING//params += $('#selPopulation').val()[n].split(" ")[1] + '|' +  drawLimit +',';
    }
    $.get('aspx/GetPath.aspx?members=' + params, processPaths);
}

function step() {


    $('#txtError').load('aspx/Step.aspx', function () {
        var text = $.trim($('#txtError').text());

        if (text == '0') {
            $('#txtError').hide();


        } else {
            $('#txtError').show();
            autorun = false;

        }
    });

    id = setInterval('updateStatus()', 300);
}

$('#btnStep').click(step);

$('#btnStop').click(stop);

$('#btnAuto').click(function () {
    autorun = true;
    $('#btnStep').button('loading');
    $('#btnReset').button('loading');
    $('#btnAuto').button('loading');
    $('#outerProgressBar').hide();
    step();
});
$('#btnReset').click(function () {


});


$('#btnTabPop').click(function () {
    $('#tabRouteExplorer').hide();
    $('#tabResults').hide();
    $('#tabPop').show();
    $('#btnTabPop').addClass('active');
    $('#btnTabItinerary').removeClass('active');
    $('#btnRouteExplorer').removeClass('active');
});

$('#btnRouteExplorer').click(function () {
    $('#tabRouteExplorer').show();
    $('#tabResults').hide();
    $('#tabPop').hide();
    $('#btnRouteExplorer').addClass('active');
    $('#btnTabItinerary').removeClass('active');
    $('#btnTabPop').removeClass('active');
});

$('#btnTabItinerary').click(function () {
    $('#tabRouteExplorer').hide();
    $('#tabResults').show();
    $('#tabPop').hide();
    $('#btnTabPop').removeClass('active');
    $('#btnTabItinerary').addClass('active');
    $('#btnRouteExplorer').removeClass('active');
});

$('#btnPEForward').click(function () {
    $('#txtPEcurrent').val(String(parseInt($('#txtPEcurrent').val()) + 1));
    drawLimit = parseInt($('#txtPEcurrent').val());

    refreshPaths();
});

$('#btnPEBack').click(function () {
    $('#txtPEcurrent').val(String(parseInt($('#txtPEcurrent').val()) - 1));
    drawLimit = parseInt($('#txtPEcurrent').val());

    refreshPaths();
});

$('#chkShowSubRoutes').change(function () {
    if (this.checked) {
        $('.subRoute').hide();
    } else {
        $('.subRoute').show();
    }

});

$('#chkBirDir').change(function () {
    biDir = this.checked;
    refreshPaths();
});

