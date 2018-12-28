$(document).ready(function () {

    GetTimeEnd();

    function GetTimeEnd() {
        $.ajax({
            url: ajaxURL + "api/GetTimeEnd",
            type: "GET",
            dataType: "json"
        }).done(function (data) {
            console.log(data);
            var date = new Date(parseInt(data.substr(6)));
            console.log(date);
            // Set the date we're counting down to
             var countDownDate = new Date(date).getTime();

            // Update the count down every 1 second
            var x = setInterval(function () {

                // Get todays date and time
                var now = new Date().getTime();

                // Find the distance between now and the count down date
                var distance = countDownDate - now;

                // Time calculations for days, hours, minutes and seconds
                var days = Math.floor(distance / (1000 * 60 * 60 * 24));
                var hours = Math.floor((distance % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
                var minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
                var seconds = Math.floor((distance % (1000 * 60)) / 1000);

                // Display the result in the element with id="demo"
                document.getElementById("demo").innerHTML = days + "d " + hours + "h "
                    + minutes + "m " + seconds + "s ";

                // If the count down is finished, write some text
                if (distance <= 0) {
                    clearInterval(x);
                    document.getElementById("demo").innerHTML = "EXPIRED";

                    SwitchSetting();
                    location.replace(ajaxURL + "StockGame/Results");
                   

                }
            }, 1000);
        });

    }

    function SwitchSetting() {

        $.ajax({
            url: ajaxURL + "api/SwitchSettings",
            type: "POST",
            data: {
                setting: 0,
            },
            dataType: "json"
        }).done(function (data) { });
    }
    

})