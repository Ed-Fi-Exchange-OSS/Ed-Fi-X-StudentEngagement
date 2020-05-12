var EncriptionService = function () {
    salt = 'a-unique-salt'

    mode = "AES-GCM";
    length = 256;
    ivLength = 12;

    // Generate key from password
    async function genEncryptionKey(password, salt) {
        var algo = {
            name: 'PBKDF2',
            hash: 'SHA-256',
            salt: new TextEncoder().encode(salt),
            iterations: 1000
        };
        var derived = { name: mode, length: length };
        var encoded = new TextEncoder().encode(password);
        var key = await crypto.subtle.importKey('raw', encoded, { name: 'PBKDF2' }, false, ['deriveKey']);

        return crypto.subtle.deriveKey(algo, key, derived, false, ['encrypt', 'decrypt']);
    }

    async function encrypt(text, password) {
        var algo = {
            name: mode,
            length: length,
            iv: crypto.getRandomValues(new Uint8Array(ivLength))
        };
        var key = await genEncryptionKey(password, mode, length, salt);
        var encoded = new TextEncoder().encode(text);

        return {
            cT: new Uint8Array(await crypto.subtle.encrypt(algo, key, encoded)),
            iv: algo.iv
        };
    }

    async function decrypt(encrypted, password) {
        var algo = {
            name: mode,
            length: length,
            iv: encrypted.iv
        };
        var key = await genEncryptionKey(password, mode, length, salt);
        var decrypted = await crypto.subtle.decrypt(algo, key, encrypted.cT.buffer);

        return new TextDecoder().decode(decrypted);
    }

    return {
        encrypt: encrypt,
        decrypt: decrypt
    }

}