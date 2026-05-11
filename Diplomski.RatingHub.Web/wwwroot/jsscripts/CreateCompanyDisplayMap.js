window.mapHelper = {
    maps: {}, 

    initMapWithMarker: function (elementId, cityLat, cityLng, initialLat, initialLng, dotnetRef) {

        
        if (this.maps[elementId]) {
            this.maps[elementId].remove();
            delete this.maps[elementId];
        }

        var map = L.map(elementId).setView([cityLat, cityLng], 13);

        this.maps[elementId] = map; 

        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            attribution: '© OpenStreetMap'
        }).addTo(map);

        var marker = null;

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
    },
    
    
    destroyMap: function (elementId) {
        if (this.maps[elementId]) {
            this.maps[elementId].remove();
            delete this.maps[elementId];
        }
    }
};