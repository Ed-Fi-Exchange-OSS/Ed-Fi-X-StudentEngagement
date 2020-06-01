var EncryptionMixedService = function (publicKey) {

    let _publicKey = publicKey;
    const algoKeyGen = { name: 'AES-GCM', length: 256 };

    function strToArrayBuffer(str) {
        var buf = new ArrayBuffer(str.length * 2);
        var bufView = new Uint16Array(buf);
        for (var i = 0, strLen = str.length; i < strLen; i++) {
            bufView[i] = str.charCodeAt(i);
        }
        return buf;
    }

    function arrayBufferToString(buf) {
        return String.fromCharCode.apply(null, new Uint16Array(buf));
    }

    function arrayBufferToBase64(buffer) {
        var binary = '';
        var bytes = new Uint8Array(buffer);
        var len = bytes.byteLength;
        for (var i = 0; i < len; i++) {
            binary += String.fromCharCode(bytes[i]);
        }
        return window.btoa(binary);
    }

    /*  */
    async function generateSymetricKey() /* returns { key: CryptoKey, iv: arrayBuffer }*/ {
        var keyUsages = ['encrypt', 'decrypt'];
        var iv = window.crypto.getRandomValues(new Uint8Array(12));
        var key = await window.crypto.subtle.generateKey(algoKeyGen, true, keyUsages);
        var jsonKey = { key: key, iv: iv };
        return jsonKey;
    }

    async function encryptSymetricKey(/*{ key: CryptoKey, iv: arrayBuffer }*/ symetricKey) /* returns  string*/ {
        const exported = await window.crypto.subtle.exportKey("raw", symetricKey.key);
        var json = JSON.stringify({ k: arrayBufferToBase64(exported), iv: arrayBufferToBase64(symetricKey.iv) });
        var encryptedSymetricKey = RSAEncrypt(json, _publicKey);
        return encryptedSymetricKey;
    }


    function RSAEncrypt(/*string*/plainText, /*string*/publicKey) /* returns string*/ {
        let RSAEncrypt = new JSEncrypt();
        RSAEncrypt.setPublicKey(publicKey);
        let cipheredText = RSAEncrypt.encrypt(plainText);
        return cipheredText;
    }

    async function AESEncrypt(/*string*/plainText, /*{k:arrayBuff, iv:arrayBuff}*/ symetricKey, ) /* returns arrayBuffer*/ {
        var algoEncrypt = { name: 'AES-GCM', iv: symetricKey.iv, tagLength: 128 };
        var encryptedText = await window.crypto.subtle
            .encrypt(algoEncrypt, symetricKey.key, strToArrayBuffer(plainText));
        return encryptedText;
    }

    this.encrypt = async function (/*string*/plainText) /* returns { k:string, m:string} */ {
        var symetricKey = await generateSymetricKey();
        var encryptedText = await AESEncrypt(plainText, symetricKey);
        var encryptedSymetricKey = await encryptSymetricKey(symetricKey);
        var jsonEncrypted = { k: encryptedSymetricKey, m: arrayBufferToBase64(encryptedText) };
        return jsonEncrypted;
    }


};