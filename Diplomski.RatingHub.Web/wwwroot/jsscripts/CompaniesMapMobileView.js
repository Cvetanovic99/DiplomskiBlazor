window.dialogMap = {
    map: null,
    markersLayer: null,

    defaultIcon: new L.Icon({
        iconUrl: 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon.png',
        shadowUrl: 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-shadow.png',
        iconSize: [25, 41],
        iconAnchor: [12, 41]
    }),

    hoverIcon: new L.Icon({
        iconUrl: 'https://raw.githubusercontent.com/pointhi/leaflet-color-markers/master/img/marker-icon-red.png',
        shadowUrl: 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-shadow.png',
        iconSize: [25, 41],
        iconAnchor: [12, 41]
    }),

    init: function (lat, lng, zoom = 12) {
        
        if (this.map) {
            this.map.remove();
            this.map = null;
            this.markersLayer = null;
        }

        const map = L.map('map-mobile').setView([lat, lng], zoom);

        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            attribution: '&copy; OpenStreetMap contributors'
        }).addTo(map);

        this.map = map;
        this.markersLayer = L.layerGroup().addTo(map);
        
        setTimeout(() => {
            map.invalidateSize();
        }, 200);
    },

    setCompanies: function (companies) {

        if (!this.map) return;

        this.markersLayer.clearLayers();

        let hoverTimeout;

        companies.forEach(c => {

            if (!c.latitude || !c.longitude) return;

            const marker = L.marker(
                [c.latitude, c.longitude],
                { icon: this.defaultIcon }
            );

            const popup = L.popup({
                closeButton: true,
                autoClose: false,
                closeOnClick: false,
                className: "custom-map-popup",
                offset: [0, -10]
            }).setContent(this.createPopupHtml(c));

            marker.on("mouseover", () => {
                clearTimeout(hoverTimeout);

                marker.setIcon(this.hoverIcon);
                marker.bindPopup(popup).openPopup();
            });

            marker.on("mouseout", () => {
                hoverTimeout = setTimeout(() => {
                    this.map.closePopup(popup);
                    marker.setIcon(this.defaultIcon);
                }, 300);
            });

            marker.on("click", () => {
                window.location.href = `/companies/${c.id}`;
            });

            marker.addTo(this.markersLayer);
        });
    },

    destroy: function () {
        if (this.map) {
            this.map.remove();
            this.map = null;
            this.markersLayer = null;
        }
    },

    renderStars: function (rating) {
        const percentage = (rating / 5) * 100;

        return `
            <div class="map-stars">
                <div class="map-stars-base">★★★★★</div>
                <div class="map-stars-fill" style="width:${percentage}%">★★★★★</div>
            </div>
        `;
    },

    createPopupHtml: function (c) {
        return `
            <div class="map-popup"
                 onclick="window.location.href='/companies/${c.id}'">

                <div class="map-popup-image">
                    <img src="${c.imageUrl || '/images/company-placeholder.svg'}" />
                </div>

                <div class="map-popup-body">

                    <div class="map-popup-title" title="${c.name}">
                        ${c.name}
                    </div>

                    <div class="map-popup-rating">
                        ${this.renderStars(c.rating || 0)}
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
};