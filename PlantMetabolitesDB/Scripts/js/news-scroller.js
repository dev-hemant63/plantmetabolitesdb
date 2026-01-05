function initImagePopup(elem){
    // check for mouse click, add event listener on document
    document.addEventListener('click', function (e) {
        // check if click target is img of the elem - elem is image container
        if (!e.target.matches(elem +' img')) return;
        else{

            var image = e.target; // get current clicked image

            // create new popup image with all attributes for clicked images and offsets of the clicked image
            var popupImage = document.createElement("img"); 
            popupImage.setAttribute('src', image.src);
            popupImage.style.width = image.width+"px";
            popupImage.style.height = image.height+"px";
            popupImage.style.right = image.offsetRight+"px";
            popupImage.style.bottom = image.offsetBottom+"px";
            popupImage.classList.add('popImage');

            // creating popup image container
            var popupContainer = document.createElement("div"); 
            popupContainer.classList.add('popupContainer');
            
            // creating popup image background
            var popUpBackground = document.createElement("div"); 
            popUpBackground.classList.add('popUpBackground');

            // append all created elements to the popupContainer then on the document.body
            popupContainer.appendChild(popUpBackground);
            popupContainer.appendChild(popupImage);
            document.body.appendChild(popupContainer);

            // call function popup image to create new dimensions for popup image and make the effect
            popupImageFunction();


            // resize function, so that popup image have responsive ability
            var wait;
            window.onresize = function(){
                clearTimeout(wait);
                wait = setTimeout(popupImageFunction, 100);
            };

            // close popup image clicking on it
            popupImage.addEventListener('click', function (e) {
                closePopUpImage();
            });
            // close popup image on clicking on the background
            popUpBackground.addEventListener('click', function (e) {
                closePopUpImage();
            });


            function popupImageFunction(){
                // wait few miliseconds (10) and change style of the popup image and make it popup
                // waiting is for animation to work, yulu can disable it and check what is happening when it's not there
                setTimeout(function(){      
                    // I created this part very simple, but you can do it much better by calculating height and width of the screen,
                    // image dimensions.. so that popup image can be placed much better
                    popUpBackground.classList.add('active');
                    popupImage.style.right = "15%";
                    popupImage.style.bottom = "50px";       
                    popupImage.style.width = window.innerWidth * 0.7+"px";
                    popupImage.style.height = ((image.height / image.width) * (window.innerWidth * 0.7))+"px";       
                }, 10);
            }

            // function for closing popup image, first it will be return to the place where 
            // it started then it will be removed totaly (deleted) after animation is over, in our case 300ms
            function closePopUpImage(){
                popupImage.style.width = image.width+"px";
                popupImage.style.height = image.height+"px";
                popupImage.style.right = image.offsetRight+"px";
                popupImage.style.bottom = image.offsetBottom+"px";
                popUpBackground.classList.remove('active');
                setTimeout(function(){      
                    popupContainer.remove();
                }, 300);
            }
            
        }
    });
}

// Start popup image function
initImagePopup(".img-container") // elem = image container



















// JavaScript Document
$(function () {
	var ticker = $("#ticker1");
	var ticker2 = $("#ticker2");
	ticker.children().filter("ul").each(function () {

		var dt = $(this),

			container = $("<div>");
		dt.next().appendTo(container);
		dt.prependTo(container);
		container.appendTo(ticker);
	});
	ticker.css("overflow", "hidden");

	function animator(currentItem) {

		var distance = currentItem.height();
		duration = (distance + parseInt(currentItem.css("marginTop"))) / 0.025;
		currentItem.animate({
			marginTop: -distance
		}, duration, "linear", function () {
			currentItem.appendTo(currentItem.parent()).css("marginTop", 0);
			animator(currentItem.parent().children(":first"));
		});
	};
	animator(ticker.children(":first"));
	var j = 0
	$('#stop').click(function () {

		ticker.children().stop();
		j = 1
		$('#stop').hide();
		$('#play').show();

		$("#ticker1").css("overflow", "scroll");
		$("#ticker1 li").addClass("stopped");

	});


	ticker.mouseenter(function () {
		ticker.children().stop();
	});

	$('#play').click(function () {

		animator(ticker.children(":first"));
		j = 0;
		$('#stop').show();
		$('#play').hide();

		$("#ticker1").css("overflow", "hidden");
		$("#ticker1 li").removeClass('stopped');
	});

	ticker.mouseleave(function () {
		if (j == 0)
			animator(ticker.children(":first"));
	});





	ticker2.children().filter("ul").each(function () {
		var dt = $(this),
			container = $("<div>");
		dt.next().appendTo(container);
		dt.prependTo(container);
		container.appendTo(ticker2);
	});
	ticker2.css("overflow", "hidden");



	animator(ticker2.children(":first"));
	var k = 0
	$('#stop2').click(function () {
		ticker2.children().stop();
		k = 1
		$('#stop2').hide();
		$('#play2').show();

		$("#ticker2").css("overflow", "scroll");
		$("#ticker2 li").addClass("stopped");

	});


	ticker2.mouseenter(function () {
		ticker2.children().stop();
	});

	$('#play2').click(function () {
		animator(ticker2.children(":first"));
		k = 0
		$('#stop2').show();
		$('#play2').hide();

		$("#ticker2").css("overflow", "hidden");
		$("#ticker2 li").removeClass("stopped");

	});

	ticker2.mouseleave(function () {
		if (k == 0)
			animator(ticker2.children(":first"));
	});

	$(".runningCourses").prepend("<span class='lftTop'></span><span class='rgtTop'></span><span class='btLft'></span><span class='btRgt'></span>");
	$(".commingCourses").prepend("<span class='lftTop1'></span><span class='rgtTop1'></span><span class='btLft1'></span><span class='btRgt1'></span>");
	$(".edge").prepend("<span class='lftEdge'></span><span class='rgtEdge'></span>");
	$(".message").prepend("<span></span>");
	$(document).ready(function () {
		$(".sf-vertical li:odd").addClass("even");
		$(".dataTable tr:even").addClass("even");
		$("#tabs table tr:odd").addClass("eventr");
	});

});
