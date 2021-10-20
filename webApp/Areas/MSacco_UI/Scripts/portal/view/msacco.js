function MSACCODecryptor(secretKey, token, dataToDecrypt) {
  var key = CryptoJS.enc.Utf8.parse(secretKey);
  var iv = CryptoJS.enc.Utf8.parse(token);
  
  var decrypted = CryptoJS.AES.decrypt(dataToDecrypt, key, { iv: iv });

  return decrypted.toString(CryptoJS.enc.Utf8);
}