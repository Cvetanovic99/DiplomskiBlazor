window.mapHelper = {
    initMapWithMarker: function (elementId, cityLat, cityLng, initialLat, initialLng, dotnetRef) {

        var map = L.map(elementId).setView([cityLat, cityLng], 13);

        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            attribution: '© OpenStreetMap'
        }).addTo(map);

        var marker = null;

        // ako postoji već lokacija (edit)
        if (initialLat && initialLng) {
            marker = L.marker([initialLat, initialLng]).addTo(map);
            map.setView([initialLat, initialLng], 15);
        }

        map.on('click', function (e) {
            if (marker) {
                map.removeLayer(marker);
            }

            marker = L.marker(e.latlng).addTo(map);

            dotnetRef.invokeMethodAsync('OnMapClick', e.latlng.lat, e.latlng.lng);
        });

        setTimeout(() => {
            map.invalidateSize();
        }, 200);
    }
};