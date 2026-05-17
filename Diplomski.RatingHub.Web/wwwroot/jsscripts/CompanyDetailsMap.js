window.companyDetailsMap = {

    map: null,
    marker: null,

    init: function (lat, lng) {
        
        if (this.map) return;

        const map = L.map('companyDetailsMap', {
            zoomControl: true
        }).setView([lat, lng], 14);

        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            attribution: '&copy; OpenStreetMap contributors'
        }).addTo(map);

        const marker = L.marker([lat, lng]).addTo(map);

        this.map = map;
        this.marker = marker;
        
        setTimeout(() => {
            map.invalidateSize();
        }, 200);
    }
};