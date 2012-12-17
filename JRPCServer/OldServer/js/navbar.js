

///
/// Fields
///
var pages = new Array(
    "#divJSelector",
	"#divJEditor",
    "#divJOptimiser",
    "#divJViewer"
	);

var currentTab = "";
	var mode = "fade";
	var updateScroll = true;

///
/// Events
///	
	var $sections = $('.section');
// all content sections
	var $navs = $('.nav > li');
// all nav sections

	var topsArray = $sections.map(function() {
	    return $(this).position().top - 300; // make array of the tops of content
	}).get();
//   sections, with some padding to
                                          //   change the class a little sooner
	var len = topsArray.length;
// quantity of total sections
	var currentIndex = 0;
// current section selected

	var getCurrent = function(top) { // take the current top position, and see which
	    for (var i = 0; i < len; i++) { // index should be displayed
	        if (top > topsArray[i] && topsArray[i + 1] && top < topsArray[i + 1]) {
	            return i;
	        }
	    }
	};

	function updateNavBar() {
	    topsArray = $sections.map(function() {
	        return $(this).position().top - 300; // make array of the tops of content
	    }).get(); //   sections, with some padding to
	    //   change the class a little sooner
	    len = topsArray.length; // quantity of total sections
	    currentIndex = 0;

}

   // on scroll,  call the getCurrent() function above, and see if we are in the
   //    current displayed section. If not, add the "selected" class to the
   //    current nav, and remove it from the previous "selected" nav
	if (mode == "scroll") {
	    $(document).scroll(function(e) {
	        if (updateScroll) {
	            var scrollTop = $(this).scrollTop();
	            var checkIndex = getCurrent(scrollTop);
	            if (checkIndex !== currentIndex) {
	                currentIndex = checkIndex;
	                $navs.eq(currentIndex).addClass("active").siblings(".active").removeClass("active");
	            }
	        }
	    });
	}


	$('.nav a').click(function() {

	    var targetPage = $($(this).attr('data-page'));

	    var targetDiv = $(targetPage);
	    //$(currentTab).parent().removeClass('active');


	    if (mode == "fade") {
	        var prevPage = $($(currentTab).attr('data-page'));
	        var prevDiv = $(prevPage);


	        if (currentTab != "" && prevDiv.exists()) {
	            prevDiv.fadeOut('fast', function() {
	                targetDiv.fadeIn('fast', function() {
	                    //targetDiv.refresh();

	                });
	            });
	        } else {
	            targetDiv.fadeIn('fast', function() {
	                //targetDiv.refresh();

	            });
	        }
	    } else {
	        updateScroll = false;
	        $('html, body').animate({
	                scrollTop: targetDiv.offset().top + error_scroll_offset
	            }, error_scroll_time, function() {
	                updateScroll = true;
	            });

	    }
	    currentTab = $(this);
	    $(this).parent().addClass('active').siblings(".active").removeClass("active");
	    ;


	});

///
///	Main Code
///
	if (mode == "fade") {
	    for (var p in pages) {
	        //$(pages[p]).parent().removeClass('active');
	        $(pages[p]).hide();
	    }
	}