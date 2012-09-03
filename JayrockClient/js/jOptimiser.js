///
/// Fields
///
var oRefreshInterval = 200; //ms

///
/// Events
///
$('#btnJORefresh').click(function () {
    OptimiserRefresh();
});

$('#btnDelJourney').click(function ()
{
    RPCCall('DequeueJourney', { uuid: selectedJourneyUuid }, function (data) {
        if (CheckForError(data)) {
            return;
        }
        setTimeout(OptimiserRefresh, oRefreshInterval);
    });
});

$('#btnJOEnqueue').click(function () {
    RPCCall('EnqueueJourney', { uuid: selectedJourneyUuid, runs: 1 }, function (data) {
        if (CheckForError(data)) {
            return;
        }
        setTimeout(OptimiserRefresh, oRefreshInterval);
    });
});


$('#btnJOPause').click(function () {
    RPCCall('PauseJourneyOptimiser', {}, function (data) {
        if (CheckForError(data)) {
            return;
        }
        setTimeout(OptimiserRefresh, oRefreshInterval);
    });
});


$('#btnJOResume').click(function () {
    RPCCall('ResumeJourneyOptimiser', {}, function (data) {
        if (CheckForError(data)) {
            return;
        }
        setTimeout(OptimiserRefresh, oRefreshInterval);
    });
});






///
/// Functions
///
/*
    public enum OptimiserState
    {
        Waiting,
        Optimising,
        Saving,
        Cancelling,
        Idle

    }

*/


function OptimiserRefresh() {
    RPCCall('GetOptimisationState', {}, function (data) {
        if (CheckForError(data)) {
            return;
        }

        //Show form / hide loading message.
        $('#frmJourneyOptimiser').show();

        var state = data.result.state;
        $('#txtOptStatus').val(data.result.state);
        $('#btnJOPause').attr('disabled', 'disabled');
        $('#btnJOResume').attr('disabled', 'disabled');


        if (selectedJourneyUuid == undefined
            || selectedJourneyUuid == "" 
                || selectedJourneyUuid == null) {

            $('#btnJOEnqueue').attr('disabled', 'disabled');
        } else {
            $('#btnJOEnqueue').removeAttr('disabled');
        }


        switch (state) {
            case "Waiting":
                //Nothing
                break;
            case "Optimising":
                $('#btnJOPause').removeAttr('disabled');
                setTimeout(OptimiserRefresh, oRefreshInterval);
                break;
            case "Saving":
                //Nothing
                break;
            case "Paused":
                $('#btnJOResume').removeAttr('disabled');
                break;
            case "Cancelling":
                //Nothing
                break;
            case "Idle":
                //Nothing
                break;

            default:
        }


        $('#selOptimiserQueue').empty();
        for (var i in data.result.queue) {
            var queuedJourney = data.result.queue[i];
            var opt = jQuery(document.createElement('option'));
            opt.text(queuedJourney);
            $('#selOptimiserQueue').append(opt);

        }
        $('#txtJPCurrentIter').val(data.result.currentIteration);
        $('#txtJOTotalIters').val(data.result.totalIterations);
        $('#prgJO>.bar').css('width', String(Math.round((data.result.currentIteration / data.result.totalIterations) * 100.0)) + "%");
        $('#txtJOCurrentJourney').val(data.result.currentJourney);



    });




}