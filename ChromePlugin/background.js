/*
This plugin was based on https://github.com/google/page-timer/ with Apache License
*/

// history should be an array of objects.
// History contaings History[tabId][0][0] = StartDate History[tabId][0][1] = URL
var History = {};
//var EncryptionService = new EncryptionService();
var UserInfo = {email: "", id: "" };
/* key used in localstorage for the whitelist */
const WHITELIST_KEY = "whitelist"
const NOT_IN_WHITELIST = "not_in_whitelist"
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

function GetUsageData(tabId, index){
  const DATE_INDEX = 0;
  const URL_INDEX = 1;

  if(!History[tabId][index]) {return null;}
  if(History[tabId][index][URL_INDEX] == NOT_IN_WHITELIST) {return null;}

  return { /*LearningActivityEventModel*/
    /* string */ DataPointType: index == 0 ? DataPointType.START : DataPointType.END,
    /* string */ IdentityElectronicMailAddress : UserInfo.email,
    /* string */ ReffererUrl : "",
    /* string */ LeaningAppUrl : History[tabId][index][URL_INDEX],
    /* DateTime */ UTCStartDateTime : History[tabId][index][DATE_INDEX],
    /* DateTime */ UTCEndDateTime : index == 0 ? null : History[tabId][index - 1][0]
   };
}

function SendDataToServer(data) {
  var xhr = new XMLHttpRequest();
  var url = config.api.Url;
  xhr.open("POST", url, true);
  xhr.setRequestHeader("Content-Type", "application/json");

  // MVP: For now we will ingore the response.

  let jsonPayload = JSON.stringify(data);
  
  let cypherText = EncryptionRCAService.encrypt(config.PublicPemKey, jsonPayload);
  xhr.send(JSON.stringify(cypherText));
}
/* ============================================*/


function FormatDuration(d) {
  if (d < 0) { return "?"; }
  var divisor = d < 3600000 ? [60000, 1000] : [3600000, 60000];
  function pad(x) { return x < 10 ? "0" + x : x; }
  return Math.floor(d / divisor[0]) + ":" + pad(Math.floor((d % divisor[0]) / divisor[1]));
}

function Update(dateTimeStart, tabId, url) {
  if (!url) { return; }
  //alert(url);
  if (tabId in History) {
    if (url == History[tabId][0][1]) { return; }
  } else { History[tabId] = []; }


  //Check if url is in whitelist
  if(!WhitelistService.IsInWhiteList(url)){
    url=NOT_IN_WHITELIST;
  }

  // Add to the beginning of the array.
  History[tabId].unshift([dateTimeStart, url]);

  // Clean History so it doesnt explode =)
  var history_limit = parseInt(localStorage["history_size"]);
  if (!history_limit) { history_limit = 23; }
  while (History[tabId].length > history_limit) { History[tabId].pop(); }

  chrome.browserAction.setBadgeText({ 'tabId': tabId, 'text': '0:00' });
  chrome.browserAction.setPopup({ 'tabId': tabId, 'popup': "popup.html#tabId=" + tabId });

  /* Sends the new site data and the previous site, including end datetime */
  let usageData = [];
  for(let i = 0; i <= 1; i++){
    let data = GetUsageData(tabId, i);
    if(null != data){ usageData.unshift(data) }
  }
  if(usageData.length > 0){SendDataToServer(usageData);}
}


function HandleUpdate(tabId, changeInfo, tab) {
  Update(new Date(), tabId, changeInfo.url);
}

function HandleRemove(tabId, removeInfo) {
  delete History[tabId];
}

function HandleReplace(addedTabId, removedTabId) {
  var now = new Date();
  delete History[removedTabId];
  chrome.tabs.get(addedTabId, function (tab) {
    Update(now, addedTabId, tab.url);
  });
}

function UpdateBadges() {
  var now = new Date();
  for (tabId in History) {
    var description = FormatDuration(now - History[tabId][0][0]);
    chrome.browserAction.setBadgeText({ 'tabId': parseInt(tabId), 'text': description });
  }
}

WhitelistService.SaveWhitelistFromAPI();

setInterval(UpdateBadges, 1000);

chrome.tabs.onUpdated.addListener(HandleUpdate);
chrome.tabs.onRemoved.addListener(HandleRemove);
chrome.tabs.onReplaced.addListener(HandleReplace);
