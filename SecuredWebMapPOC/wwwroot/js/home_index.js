(async function () {
    // Make request to Server to obtain token
    var response = await fetch("/Home/GetToken", {
        method: "POST",
        cache: "no-cache",
        headers: {
            "Content-Type": "application/json"
        }
    });

    var token = await response.text();


    // After token is obtained, begin using ESRI JavaScript API
    require([
        "esri/views/MapView",
        "esri/WebMap",
        "esri/identity/IdentityManager"
    ], function (MapView, WebMap, esriId) {
            // This registers the token
            esriId.registerToken({
                server: "https://www.arcgis.com/sharing/rest",
                token: token
            });

            var webmap = new WebMap({
                portalItem: {
                    id: _eatsPortalId // This is set in Index.cshtml, as I have it loaded in from appsettings.json
                }
            });

            var view = new MapView({
                map: webmap,
                container: "viewDiv" // Finds an HTML element with id="viewDiv" and the map view is rendered inside of it.
            });
    });
})();