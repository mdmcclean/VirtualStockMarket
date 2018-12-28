$(document).ready(function () {

    $(".new-game-button").click(NewGame);

    function NewGame() {
        $.ajax({
            url: ajaxURL + "api/CheckSetting",
            type: "GET",
            dataType: "json"
        }).done(function (data) {
            if (data.SettingValue == 0) {
                SwitchSetting();
                location.replace(ajaxURL + "StockGame/Settings");
            }
            else if (data.SettingValue == 2) {
                $(".new-game-button").text("Settings are being made...")
                setInterval(function () { CheckIfTheresAGame(); }, 5257);
            }
            else {
                location.replace(ajaxURL + "StockGame/Game");
            }
        });    
    }

    function CheckIfTheresAGame() {

        $.ajax({
            url: ajaxURL + "api/CheckSetting",
            type: "GET",
            dataType: "json"
        }).done(function (data) {
            if (data.SettingValue == 1) {
                $(".new-game-button").text("The Game Has Started! Click to Join!")

                $(".new-game-button").click(function () {
                    location.replace(ajaxURL + "StockGame/Game");
                });
                
            }
        });

    }

    function SwitchSetting() {

        $.ajax({
            url: ajaxURL + "api/SwitchSettings",
            type: "POST",
            data: {
                setting: 2,
            },
            dataType: "json"
        }).done(function (data) { });
    }
})