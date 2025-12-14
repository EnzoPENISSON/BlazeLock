window.blazeCrypto = {
    // 1. Générer un sel aléatoire (16 octets)
    generateSalt: function () {
        const salt = window.crypto.getRandomValues(new Uint8Array(16));
        // Retourne en Base64 pour le C#
        return btoa(String.fromCharCode(...salt));
    },

    // 2. Dériver la clé (PBKDF2) -> Transforme le mot de passe en clé AES
    deriveKey: async function (password, saltBase64) {
        const enc = new TextEncoder();
        // Convertir le sel Base64 en Uint8Array
        const salt = Uint8Array.from(atob(saltBase64), c => c.charCodeAt(0));

        // Importer le mot de passe brut
        const keyMaterial = await window.crypto.subtle.importKey(
            "raw",
            enc.encode(password),
            { name: "PBKDF2" },
            false,
            ["deriveKey"]
        );

        // Dériver la clé AES-GCM (256 bits, 100 000 itérations)
        const key = await window.crypto.subtle.deriveKey(
            {
                name: "PBKDF2",
                salt: salt,
                iterations: 100000,
                hash: "SHA-256"
            },
            keyMaterial,
            { name: "AES-GCM", length: 256 },
            true, // Extractable (pour l'exporter vers C#)
            ["encrypt", "decrypt"]
        );

        // Exporter la clé en format brut (raw bytes) puis en Base64
        const exported = await window.crypto.subtle.exportKey("raw", key);
        return btoa(String.fromCharCode(...new Uint8Array(exported)));
    },

    // 3. Chiffrer (AES-GCM)
    encryptData: async function (plainText, keyBase64) {
        const enc = new TextEncoder();
        const keyBytes = Uint8Array.from(atob(keyBase64), c => c.charCodeAt(0));

        // Importer la clé AES
        const key = await window.crypto.subtle.importKey(
            "raw",
            keyBytes,
            { name: "AES-GCM" },
            false,
            ["encrypt"]
        );

        // Générer un IV (Vecteur d'initialisation) de 12 octets
        const iv = window.crypto.getRandomValues(new Uint8Array(12));

        // Chiffrer
        const encryptedContent = await window.crypto.subtle.encrypt(
            {
                name: "AES-GCM",
                iv: iv
            },
            key,
            enc.encode(plainText)
        );

        // Combiner [IV] + [ContenuChiffré] dans un seul tableau
        const combined = new Uint8Array(iv.length + encryptedContent.byteLength);
        combined.set(iv);
        combined.set(new Uint8Array(encryptedContent), iv.length);

        // Retourner en Base64
        return btoa(String.fromCharCode(...combined));
    },

    // 4. Déchiffrer (AES-GCM)
    decryptData: async function (encryptedBase64, keyBase64) {
        try {
            const keyBytes = Uint8Array.from(atob(keyBase64), c => c.charCodeAt(0));
            const encryptedBytes = Uint8Array.from(atob(encryptedBase64), c => c.charCodeAt(0));

            // Importer la clé
            const key = await window.crypto.subtle.importKey(
                "raw",
                keyBytes,
                { name: "AES-GCM" },
                false,
                ["decrypt"]
            );

            // Séparer l'IV (12 premiers octets) du reste (Ciphertext + Tag)
            const iv = encryptedBytes.slice(0, 12);
            const data = encryptedBytes.slice(12);

            // Déchiffrer
            const decrypted = await window.crypto.subtle.decrypt(
                {
                    name: "AES-GCM",
                    iv: iv
                },
                key,
                data
            );

            return new TextDecoder().decode(decrypted);
        } catch (e) {
            console.error("Erreur de déchiffrement JS :", e);
            return null; // Indique l'échec
        }
    }
};