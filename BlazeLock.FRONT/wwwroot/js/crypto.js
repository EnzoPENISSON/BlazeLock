window.blazeCrypto = {
    // 1. Générer un sel aléatoire (16 octets)
    generateSalt: function () {
        const salt = window.crypto.getRandomValues(new Uint8Array(16));
        return btoa(String.fromCharCode(...salt));
    },

    // Dériver la clé (PBKDF2)
    deriveKey: async function (password, saltBase64) {
        const enc = new TextEncoder();
        const salt = Uint8Array.from(atob(saltBase64), c => c.charCodeAt(0));

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

    // Chiffrer (AES-GCM)
    encryptData: async function (plainText, keyBase64) {
        const enc = new TextEncoder();
        const keyBytes = Uint8Array.from(atob(keyBase64), c => c.charCodeAt(0));

        const key = await window.crypto.subtle.importKey(
            "raw", keyBytes, { name: "AES-GCM" }, false, ["encrypt"]
        );

        // Générer l'IV (12 octets)
        const iv = window.crypto.getRandomValues(new Uint8Array(12));

        const encryptedBuffer = await window.crypto.subtle.encrypt(
            { name: "AES-GCM", iv: iv },
            key,
            enc.encode(plainText)
        );

        const bufferBytes = new Uint8Array(encryptedBuffer);
        const tagLength = 16;
        const cipherLength = bufferBytes.length - tagLength;

        const cipherTextBytes = bufferBytes.slice(0, cipherLength);
        const tagBytes = bufferBytes.slice(cipherLength);

        return {
            cipherText: btoa(String.fromCharCode(...cipherTextBytes)),
            iv: btoa(String.fromCharCode(...iv)),
            tag: btoa(String.fromCharCode(...tagBytes))
        };
    },

    // Déchiffrer (AES-GCM)
    decryptData: async function (encryptedBase64, keyBase64) {
        try {
            const keyBytes = Uint8Array.from(atob(keyBase64), c => c.charCodeAt(0));
            const encryptedBytes = Uint8Array.from(atob(encryptedBase64), c => c.charCodeAt(0));

            const key = await window.crypto.subtle.importKey(
                "raw",
                keyBytes,
                { name: "AES-GCM" },
                false,
                ["decrypt"]
            );

            const iv = encryptedBytes.slice(0, 12);
            const data = encryptedBytes.slice(12);

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
            return null;
        }
    }
};