var EncryptionServiceAESGCM = function () {
    SALT = 'a-unique-salt'
    DERIVED_KEY_LENGHT = 256; /* bits*/

    MODE = "AES-GCM";
    TAG_LENGTH = 128;/* bits*/
    IV_LENGTH = 96;/* bits */

    // Generate key from password
    async function genEncryptionKey(password, SALT) {
        var algorithm = {
            name: 'PBKDF2', 
            hash: 'SHA-256',
            salt: new TextEncoder().encode(SALT),
            iterations: 1000
        };
        var derived = { name: MODE, length: DERIVED_KEY_LENGHT };
        var encoded = new TextEncoder().encode(password);
        var key = await crypto.subtle.importKey('raw', encoded, { name: 'PBKDF2' }, false, ['deriveKey']);

        return crypto.subtle.deriveKey(algorithm, key, derived, true, ['encrypt', 'decrypt']);
    }

    async function genEncryptionKeyFromExportedKey(exportedKey){
        return crypto.subtle.importKey("jwk",
            {
                alg: "A256GCM",
                ext: true,
                k: exportedKey, /* Exported key from genEncryptionKey */
                key_ops: (2)["encrypt", "decrypt"],
                kty: "oct"
            },
            {   //this is the algorithm options
                name: "AES-GCM",
            },
            false, //whether the key is extractable (i.e. can be used in exportKey)
            ["encrypt", "decrypt"] //can "encrypt", "decrypt", "wrapKey", or "unwrapKey"
        );
    }

    async function encrypt(plainText, exportedKey) {
        var algorithm = {
            name: MODE, 
            iv: crypto.getRandomValues(new Uint8Array(IV_LENGTH / 8)),
            taglength: TAG_LENGTH
        };
        
        var key = await genEncryptionKeyFromExportedKey(exportedKey);
        var encodedPlainText = new TextEncoder().encode(plainText);

        
        return /* encryptedModel */ {
            ct: new Uint8Array(await crypto.subtle.encrypt(algorithm, key, encodedPlainText)),
            iv: algorithm.iv

        };
    }

    async function decrypt(encryptedModel, password) {
        var algorithm = {
            name: MODE,
            iv: encryptedModel.iv,
            taglength: TAG_LENGTH   
        };
        var key = await genEncryptionKeyFromExportedKey(exportedKey);
        var decrypted = await crypto.subtle.decrypt(algorithm, key, encryptedModel.ct.buffer);

        return new TextDecoder().decode(decrypted);
    }

    return {
        encrypt: encrypt,
        decrypt: decrypt
    }

}