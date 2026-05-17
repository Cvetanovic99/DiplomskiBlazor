window.localStorageHelper = {
    // Upisivanje vrednosti u localStorage
    setItem: (key, value) => {
        try {
            localStorage.setItem(key, value);
            return true;
        } catch (e) {
            console.error('Greška pri upisu u localStorage:', e);
            return false;
        }
    },

    // Čitanje vrednosti iz localStorage
    getItem: (key) => {
        try {
            const value = localStorage.getItem(key);
            return value !== null ? value : null;
        } catch (e) {
            console.error('Greška pri čitanju iz localStorage:', e);
            return null;
        }
    },

    // Brisanje vrednosti iz localStorage
    removeItem: (key) => {
        try {
            localStorage.removeItem(key);
            return true;
        } catch (e) {
            console.error('Greška pri brisanju iz localStorage:', e);
            return false;
        }
    },

    // Provera da li ključ postoji
    hasItem: (key) => {
        try {
            return localStorage.getItem(key) !== null;
        } catch (e) {
            console.error('Greška pri proveri localStorage:', e);
            return false;
        }
    }
};