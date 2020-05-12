/*
This plugin was based on https://github.com/google/page-timer/ with Apache License
*/

// history should be an array of objects.
// History contaings History[tabId][0][0] = StartDate History[tabId][0][1] = URL
var History = {};
var EncriptionService = new EncriptionService();


// Initialize the badge on the chrome plugin icon that shows on the right.
chrome.browserAction.setBadgeText({ 'text': '?' });
chrome.browserAction.setBadgeBackgroundColor({ 'color': "#777" });

function SendDataToServer() {
  var xhr = new XMLHttpRequest();
  var url = config.api.Url;
  xhr.open("POST", url, true);
  xhr.setRequestHeader("Content-Type", "application/json");

  // MVP: For now we will ingore the response.
  // xhr.onreadystatechange = function () {
  //   if (xhr.readyState === 4 && xhr.status === 200) {
  //       // var json = JSON.parse(xhr.responseText);
  //       // console.log(json.email + ", " + json.password); 
  //   }
  //   console.log(xhr);
  // };
  let data = { "email": "hey@mail.com", "password": "101010" };
  let jsonPayload = JSON.stringify(data);
  EncriptionService.encrypt(jsonPayload, config.jsonEncryptionKey)
    .then(encrypted => {
      console.log("encrypted", encrypted); // { cipherText: Uint8Array, iv: Uint8Array }
      xhr.send(JSON.stringify(encrypted));

      // EncriptionService.decrypt(encrypted, config.jsonEncryptionKey)
      //   .then(decrypted => {
      //     console.log("decrypted:", decrypted); // { cipherText: Uint8Array, iv: Uint8Array }
      //   });

    });
}

function FormatDuration(d) {
  if (d < 0) { return "?"; }
  var divisor = d < 3600000 ? [60000, 1000] : [3600000, 60000];
  function pad(x) { return x < 10 ? "0" + x : x; }
  return Math.floor(d / divisor[0]) + ":" + pad(Math.floor((d % divisor[0]) / divisor[1]));
}

function Update(dateTime, tabId, url) {
  if (!url) { return; }
  //alert(url);
  if (tabId in History) {
    if (url == History[tabId][0][1]) { return; }
  } else { History[tabId] = []; }

  // Add to the beginning of the array.
  History[tabId].unshift([dateTime, url]);

  // Clean History so it doesnt explode =)
  var history_limit = parseInt(localStorage["history_size"]);
  if (!history_limit) { history_limit = 23; }
  while (History[tabId].length > history_limit) { History[tabId].pop(); }

  chrome.browserAction.setBadgeText({ 'tabId': tabId, 'text': '0:00' });
  chrome.browserAction.setPopup({ 'tabId': tabId, 'popup': "popup.html#tabId=" + tabId });

  SendDataToServer();
}

function HandleUpdate(tabId, changeInfo, tab) {
  Update(new Date(), tabId, changeInfo.url);
}

function HandleRemove(tabId, removeInfo) {
  delete History[tabId];
}

function HandleReplace(addedTabId, removedTabId) {
  var t = new Date();
  delete History[removedTabId];
  chrome.tabs.get(addedTabId, function (tab) {
    Update(t, addedTabId, tab.url);
  });
}

function UpdateBadges() {
  var now = new Date();
  for (tabId in History) {
    var description = FormatDuration(now - History[tabId][0][0]);
    chrome.browserAction.setBadgeText({ 'tabId': parseInt(tabId), 'text': description });
  }
}

setInterval(UpdateBadges, 1000);

chrome.tabs.onUpdated.addListener(HandleUpdate);
chrome.tabs.onRemoved.addListener(HandleRemove);
chrome.tabs.onReplaced.addListener(HandleReplace);
