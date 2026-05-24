window.mapInstance = null;
window.markersLayer = null;

const defaultIcon = new L.Icon({
    iconUrl: 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon.png',
    shadowUrl: 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-shadow.png',
    iconSize: [25, 41],
    iconAnchor: [12, 41]
});

const hoverIcon = new L.Icon({
    iconUrl: 'https://raw.githubusercontent.com/pointhi/leaflet-color-markers/master/img/marker-icon-red.png',
    shadowUrl: 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-shadow.png',
    iconSize: [25, 41],
    iconAnchor: [12, 41]
});

window.initMap = (lat, lng, zoom = 12) => {

    if (window.mapInstance) {
        window.mapInstance.remove();
        window.mapInstance = null;
        window.markersLayer = null;
    }

    const map = L.map('map').setView([lat, lng], zoom);

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; OpenStreetMap contributors'
    }).addTo(map);

    window.mapInstance = map;
    window.markersLayer = L.layerGroup().addTo(map);
};


window.setCompaniesOnMap = (companies) => {

    if (!window.mapInstance) return;

    window.markersLayer.clearLayers();

    let hoverTimeout;

    companies.forEach(c => {

        if (!c.latitude || !c.longitude) return;

        const marker = L.marker(
            [c.latitude, c.longitude],
            { icon: defaultIcon } 
        );

        const popup = L.popup({
            closeButton: true,
            autoClose: false,
            closeOnClick: false,
            className: "custom-map-popup",
            offset: [0, -10]
        }).setContent(createPopupHtml(c));

        marker.on("mouseover", function () {
            clearTimeout(hoverTimeout);

            marker.setIcon(hoverIcon);
            marker.bindPopup(popup).openPopup();
        });

        marker.on("mouseout", function () {
            hoverTimeout = setTimeout(() => {
                window.mapInstance.closePopup(popup);
                marker.setIcon(defaultIcon); 
            });
        });

        marker.on("click", function () {
            window.location.href = `/companies/${c.id}`;
        });

        marker.addTo(window.markersLayer);
    });
};


function renderStars(rating) {

    const percentage = (rating / 5) * 100;

    return `
        <div class="map-stars">
            <div class="map-stars-base">★★★★★</div>
            <div class="map-stars-fill" style="width:${percentage}%">★★★★★</div>
        </div>
    `;
}


function createPopupHtml(c) {

    return `
        <div class="map-popup"
             onclick="window.location.href='/companies/${c.id}'">

            <div class="map-popup-image">
                <img src="${c.imageUrl || '/images/default.jpg'}" />
            </div>

            <div class="map-popup-body">

                <div class="map-popup-title" title="${c.name}">
                    ${c.name}
                </div>

                <div class="map-popup-rating">
                    ${renderStars(c.rating || 0)}
                    <span class="rating-value">
                        ${(c.rating ?? 0).toFixed(1)}
                    </span>
                </div>

                <div class="map-popup-address" title="${c.address}">
                    📍 ${c.address}
                </div>

            </div>
        </div>
    `;
}