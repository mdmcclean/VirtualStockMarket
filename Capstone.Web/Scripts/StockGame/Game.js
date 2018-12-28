$(document).ready(function () {
    $(".jumbotron").remove();


    var UserNumber = $("#PlayerUsername").data().player;

    AddUserToGame();

    getStocksAjax();

    GetUserHoldings();


    ReloadPage();


    function AddUserToGame() {
        $.ajax({
            url: ajaxURL + "api/AddUserToGame",
            type: "POST",
            dataType: "json",
            data: {
                userId: Number(UserNumber),
            }
        }).done(function (data){
        });
    }

    function ReloadPage(){
        setInterval(function () {  UpdateStocks(); }, 5257);
    }

    function GetUserHoldings() {
        $.ajax({
            url: ajaxURL + "/api/UserStocks",
            type: "GET",
            dataType: "json",
            data: {
                userId: UserNumber,
            }

        }).done(function (data) {
            for (let i = 0; i < data._userStocks.length; i++) {
                if (data._userStocks[i].Shares > 0) {
                    let currPriceId = "#priceOf" + data._userStocks[i].UserStock.StockID;
                    let currPrice = $(currPriceId).text().replace(",","");
                    currPrice = currPrice.substring(1);
                    let gainLoss = Number(Number(currPrice) - data._userStocks[i].PurchasePrice) * data._userStocks[i].Shares;
                    $("#sharesOf" + data._userStocks[i].UserStock.StockID).text(numberWithCommas(data._userStocks[i].Shares)).removeClass("loss").addClass("gain");
                    $("#costBasisOf" + data._userStocks[i].UserStock.StockID).text("$" + numberWithCommas(data._userStocks[i].PurchasePrice.toFixed(2)));
                    $("#gainLossOf" + data._userStocks[i].UserStock.StockID).text("$" + numberWithCommas(Number(gainLoss.toFixed(2))));

                    $("#gainLossOf" + data._userStocks[i].UserStock.StockID).removeClass("gain").removeClass("loss");
                    if (gainLoss >= 0) {
                        $("#gainLossOf" + data._userStocks[i].UserStock.StockID).addClass("gain");
                    }
                    else {
                        $("#gainLossOf" + data._userStocks[i].UserStock.StockID).addClass("loss");
                    }
                }
                else {
                    $("#sharesOf" + data._userStocks[i].UserStock.StockID).text("0").removeClass("gain").addClass("loss");
                    $("#costBasisOf" + data._userStocks[i].UserStock.StockID).text("");
                    $("#gainLossOf" + data._userStocks[i].UserStock.StockID).text("")
                }
            }
        });

    }

    function getStocksAjax() {
        $.ajax({
            url: ajaxURL + "api/ListOfAvailableStocks",
            type: "GET",
            dataType: "json"
        }).done(function (data) {
            GetAvailableStocks(data);
        });

    }

    function BuyStock(id, userID) {

        let sharesToBuy = $("#stockID" + id).val();
        if (sharesToBuy > 0) {
            let currPrice = Number($("#priceOf" + id).text())

            $.ajax({
                url: ajaxURL + "/api/BuyStock",
                type: "POST",
                dataType: "json",
                data: {
                    userId: userID,
                    stockId: id,
                    shares: sharesToBuy
                }

            }).done(function (data) {
                UpdateAvailableStockPrice(data);
           });
         }
    }

    function SellStock(id, userID) {

        let sharesToSell = $("#stockID" + id).val();
        if (Number(sharesToSell) > 0) {
            sharesToSell = Number(Number(sharesToSell) * -1);

            $.ajax({
                url: ajaxURL + "/api/BuyStock",
                type: "POST",
                dataType: "json",
                data: {
                    userId: userID,
                    stockId: id,
                    shares: sharesToSell
                }

            }).done(function (data) {
                UpdateAvailableStockPrice(data);
            });
        }
    }

    function UpdateStocks() {

        $.ajax({
            url: ajaxURL + "/api/GetCashBalances",
            type: "GET",
            dataType: "json"
        }).done(function (data) {
            UpdateCashBalances(data);
        });


        $.ajax({
            url: ajaxURL + "/api/GetOwnersOfStock",
            type: "GET",
            dataType: "json"
        }).done(function (data) {
            SetOwners(data);
        });


        $.ajax({
            url: ajaxURL + "/api/Update",
            type: "GET",
            dataType: "json"
        }).done(function (data) {
            UpdateAvailableStockPrice(data);
        });
    }

    function UpdateAvailableStockPrice(data) {
        for (let i = 1; i < data._stocks.length + 1; i++) {
            $("#priceOf" + i).text("$" + numberWithCommas(Number(data._stocks[i - 1].CurrentPrice.toFixed(2))));
            $("#avail" + i).text(data._stocks[i - 1].AvailableShares);
        }
        GetUserHoldings();
    }

    function SetOwners(data) {
        $(".stockOwnership").text("");
        for (let i = 0; i < data.length; i++) {
            $("#ownerOf" + data[i].StockOwned.StockID).text(data[i].Owner.FullName);
        }
    }

    function numberWithCommas(x) {
        var parts = x.toString().split(".");
        parts[0] = parts[0].replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1,');
        return parts.join(".");
    }

    function UpdateCashBalances(data) {
        $(".leaderboardBody").empty()
        let top3 = false
        for (let i = 0; i < data.length; i++) {
            if (i == 0 || i == 1 || i == 2) {
                let leadTableRow = $("<tr>");
                let place = $("<th scope='row'>").text(i + 1 + ".");
                let name = $("<td>").text(data[i].UserInfo.FullName);
                let owned = $("<td>").attr("id", "LBof" + data[i].IdOfUser);
                if (data[i].OwnedStocks > 0) {
                    owned.text(data[i].OwnedStocks);
                }
                else {
                    owned.text("0");
                }
                let stockWorth = $("<td>").text("$" + numberWithCommas(data[i].StockWorth.toFixed(2)));
                let totalCash = $("<td>").text("$" + numberWithCommas(data[i].CurrentCash.toFixed(2)));
                let portfolioVal = $("<td>").text("$" + numberWithCommas(data[i].TotalCash.toFixed(2)));
                leadTableRow.append(place);
                leadTableRow.append(name);
                leadTableRow.append(owned);
                leadTableRow.append(stockWorth);
                leadTableRow.append(totalCash);
                leadTableRow.append(portfolioVal);
                $('.leaderboardBody').append(leadTableRow);

                if (data[i].IdOfUser == UserNumber) {
                    top3 = true;
                }

            }
            else if (!top3 && data[i].IdOfUser == UserNumber) {
                let leadTableRow = $("<tr>");
                let place = $("<th scope='row'>").text(i + 1 + ".");
                let name = $("<td>").text(data[i].UserInfo.FullName);
                let stockWorth = $("<td>").text("$" + numberWithCommas(data[i].StockWorth.toFixed(2)));
                let owned = $("<td>").attr("id", "LBof" + data[i].IdOfUser);
                if (data[i].OwnedStocks > 0) {
                    owned.text(data[i].OwnedStocks);
                }
                else {
                    owned.text("0");
                }
                let totalCash = $("<td>").text("$" + numberWithCommas(data[i].CurrentCash.toFixed(2)));
                let portfolioVal = $("<td>").text("$" + numberWithCommas(data[i].TotalCash.toFixed(2)));
                leadTableRow.append(place);
                leadTableRow.append(name);
                leadTableRow.append(owned);
                leadTableRow.append(stockWorth);
                leadTableRow.append(totalCash);
                leadTableRow.append(portfolioVal);
                $('.leaderboardBody').append(leadTableRow);

            }
            if (data[i].IdOfUser == UserNumber) {
                $("#portfolioValue").text("Current $" + numberWithCommas(data[i].CurrentCash.toFixed(2)));
            }
        }
    }

    function PopulateModal(symbol) {
        
        let stockSymbol = symbol;

        $.ajax({
            url: "https://api.iextrading.com/1.0/stock/" + stockSymbol + "/company",
            type: "GET",
            dataType: "json"
        }).done(function (data) {
            $(".modal-header > h5").empty();
            $(".modal-list").empty();
            $(".modal-header > h5").text(data.companyName);
            $(".modal-list").append('<li><b>Symbol:</b> ' + data.symbol + '</li>');
            $(".modal-list").append('<li><b>Industry:</b> ' + data.industry + '</li>');
            $(".modal-list").append('<li><b>Sector:</b> ' + data.sector + '</li>');
            $(".modal-list").append('<li><b>CEO:</b> ' + data.CEO + '</li>');
            $(".modal-list").append('<li><b>Official Website:</b> <a href="' + data.website + '">' + data.website + '</a></li><br>');
            $(".modal-list").append('<li>' + data.description + '</li>');
        });
    }

    function GetAvailableStocks(data) {

        $("#stockTable").empty();

        for (let i = 0; i < data._stocks.length; i++) {

            let stockTableRow = $("<tr>");
            let stockShares = $('<td>').attr("id","sharesOf" + data._stocks[i].StockID);
            let stockSymbol = $("<td>").html('<button type="button" class="btn btn-link stockSymbol" data-toggle="modal" data-target="#exampleModal">' + data._stocks[i].Symbol + '</button>').on('click', function (e) {
                PopulateModal(data._stocks[i].Symbol);
            });
            let price = $("<td>").text("$" + data._stocks[i].CurrentPrice.toFixed(2)).attr("id", "priceOf" + data._stocks[i].StockID);
            let avail = $('<td>').attr("id", "avail" + data._stocks[i].StockID).text(data._stocks[i].AvailableShares);
            var sharesToBuySell = document.createElement('input');
            sharesToBuySell.type = "text";
            sharesToBuySell.id = "stockID" + data._stocks[i].StockID;
            sharesToBuySell.setAttribute("size", 4);
            sharesToBuySell.setAttribute("maxlength", 4);
            let sharesInput = $("<td>");
            sharesInput.append(sharesToBuySell);
            var buyButton = document.createElement('button');
            buyButton.id = "buyStockId" + data._stocks[i].StockID;
            buyButton.innerText = "Buy";
            buyButton.onclick = function () {
                BuyStock(data._stocks[i].StockID, UserNumber);
            }
            $(buyButton).addClass("btn").addClass("btn-success").addClass("btn-sm");
            let bButtonCol = $("<td>")
            bButtonCol.append(buyButton);
            var sellButton = document.createElement('button');
            sellButton.id = "sellStockId" + data._stocks[i].StockID;
            sellButton.innerText = "Sell";
            sellButton.onclick = function () {
                SellStock(data._stocks[i].StockID, UserNumber);
            }
            $(sellButton).addClass("btn").addClass("btn-danger").addClass("btn-sm");
            let sButtonCol = $("<td>");
            sButtonCol.append(sellButton);

            let ownCol = $("<td>").attr("id", "ownerOf" + data._stocks[i].StockID).addClass("stockOwnership");
            let avgCol = $("<td>").attr("id", "costBasisOf" + data._stocks[i].StockID);
            let gainLoss = $("<td>").attr("id", "gainLossOf" + data._stocks[i].StockID);

            stockTableRow.append(stockSymbol);
            stockTableRow.append(price);
            stockTableRow.append(ownCol);
            stockTableRow.append(avgCol);
            stockTableRow.append(gainLoss);
            stockTableRow.append(stockShares);
            stockTableRow.append(avail);
            stockTableRow.append(bButtonCol);
            stockTableRow.append(sButtonCol);
            stockTableRow.append(sharesInput);

            $("#stockTable").append(stockTableRow);
        }

 
    }

    
})