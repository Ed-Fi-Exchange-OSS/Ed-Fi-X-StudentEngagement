var WhitelistService = {
    SaveWhitelistFromAPI: function (){
        var xhr = new XMLHttpRequest();
        var url = config.api.Whitelist;
        xhr.open("GET", url, true);
        xhr.onload = function (){
          if (xhr.readyState === 4) {
            if (xhr.status === 200) {      
                var strWhitelist = xhr.responseText;
                /* TODO: Check that the object is correct*/
                localStorage.setItem(WHITELIST_KEY, strWhitelist)
                setTimeout(this.SaveWhitelistFromAPI, 30 * 60000);
            } else {
                console.error(xhr.statusText);
                setTimeout(SaveWhitelistFromAPI, 5000)
            }
          }    
        };
        xhr.send(null);
      },
      
      GetWhitelist: function (){
        var strWhitelist = localStorage.getItem(WHITELIST_KEY);
        if(strWhitelist == null){ 
          error = "No whitelist in localstorage";
          console.error(error);
          setTimeout(this.SaveWhitelistFromAPI, 5000);
          throw(error);
        }
        try{
          return JSON.parse(strWhitelist);
        }catch(ex) {
          console.error(`Can't parse whitelist: [${strWhitelist}]`);
          setTimeout(this.SaveWhitelistFromAPI, 5000);
          throw(ex);
        }
      },
      
      IsInWhiteList: function (url){
        try{
          var whitelist = this.GetWhitelist();
        }catch{
          /* 
            if whitelist doen't exits, dont send data. 
            Better to fail to send data than to send
            inapropiate data.
          */
          return false;
        }
        for (var i=0; i<whitelist.length; i++){
          var regEx = RegExp(whitelist[i].regex);
          if(regEx.test(url)){
            return true;
          }
        }
        return false;
      }
}