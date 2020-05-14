/*
This plugin was based on https://github.com/google/page-timer/ with Apache License
*/

// history should be an array of objects.
// History contaings History[tabId][0][0] = StartDate History[tabId][0][1] = URL
var History = {};
var EncryptionService = new EncryptionService();
var UserInfo = {email: "", id: "" };
/* get user info fromm chrome. Only works if the user has synchronization activated */
chrome.identity.getProfileUserInfo(userInfo => UserInfo = userInfo);


// Initialize the badge on the chrome plugin icon that shows on the right.
chrome.browserAction.setBadgeText({ 'text': '?' });
chrome.browserAction.setBadgeBackgroundColor({ 'color': "#777" });

/* ================= Send data ================*/
const DataPointType = {
  START: "start",
  END: "end"
}

function getUsageData(tabId, index){
  if(!History[tabId][index]) return null;

  return { 
    /* string */ DataPointType: index == 0 ? DataPointType.START : DataPointType.END,
    /* string */ IdentityEmailAddress : UserInfo.email,
    /* string */ ReffererUrl : "",
    /* string */ LeaningAppUrl : History[tabId][index][1],
    /* DateTime */ UTCDateTimeStart : History[tabId][index][0],
    /* DateTime */ UTCDateTimeEnd : index == 0 ? null : History[tabId][index - 1][0]
   };
}

function SendDataToServer(data) {
  var xhr = new XMLHttpRequest();
  var url = config.api.Url;
  xhr.open("POST", url, true);
  xhr.setRequestHeader("Content-Type", "application/json");

  // MVP: For now we will ingore the response.

  let jsonPayload = JSON.stringify(data);
  EncryptionService.encrypt(jsonPayload, config.encryptionExportedKey)
    .then(encrypted => {
      xhr.send(JSON.stringify(encrypted));
    });
}
/* ============================================*/


function FormatDuration(d) {
  if (d < 0) { return "?"; }
  var divisor = d < 3600000 ? [60000, 1000] : [3600000, 60000];
  function pad(x) { return x < 10 ? "0" + x : x; }
  return Math.floor(d / divisor[0]) + ":" + pad(Math.floor((d % divisor[0]) / divisor[1]));
}

function Update(dateTimeStart, dateTimeEnd, tabId, url, reffererUrl) {
  if (!url) { return; }
  //alert(url);
  if (tabId in History) {
    if (url == History[tabId][0][1]) { return; }
  } else { History[tabId] = []; }

  // Add to the beginning of the array.
  History[tabId].unshift([dateTimeStart, url]);

  // Clean History so it doesnt explode =)
  var history_limit = parseInt(localStorage["history_size"]);
  if (!history_limit) { history_limit = 23; }
  while (History[tabId].length > history_limit) { History[tabId].pop(); }

  chrome.browserAction.setBadgeText({ 'tabId': tabId, 'text': '0:00' });
  chrome.browserAction.setPopup({ 'tabId': tabId, 'popup': "popup.html#tabId=" + tabId });

  /* Sends the new site data and the previous site, including end datetime */
  let newSite = getUsageData(tabId, 0);
  let PreviousSite = getUsageData(tabId, 1);
  let usageData = PreviousSite != null ? [newSite, PreviousSite] : [newSite];
  SendDataToServer(usageData);
}


function HandleUpdate(tabId, changeInfo, tab) {
  Update(new Date(), null, tabId, changeInfo.url, null);
}

function HandleRemove(tabId, removeInfo) {
  delete History[tabId];
}

function HandleReplace(addedTabId, removedTabId) {
  var now = new Date();
  delete History[removedTabId];
  chrome.tabs.get(addedTabId, function (tab) {
    Update(now, null, addedTabId, tab.url, null);
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
