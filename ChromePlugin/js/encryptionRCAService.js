var EncryptionRCAService = {
    encrypt: function (publicKey, plainText){
        let RSAEncrypt = new JSEncrypt();
        RSAEncrypt.setPublicKey(publicKey);
        let cipherText = RSAEncrypt.encrypt(plainText);
        return cipherText;
    }
}