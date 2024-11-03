//http://stackoverflow.com/questions/17657184/using-jquerys-ajax-method-to-retrieve-images-as-a-blob
//http://stackoverflow.com/questions/19183180/how-to-save-an-image-to-localstorage-and-display-it-on-the-next-page


var CALL_ENDED = 6;
var CALL_FREEZED = 9;
var TECH_SOURCE = 2;
		
var g_qryStr ="";
		
		
/********************************************************************/
function GetVersion(){
  return "04.09";
}  

/********************************************************************/
function GetServerIp(){
  //return "ec2-52-37-115-198.us-west-2.compute.amazonaws.com";
  //"ec2-54-67-7-60.us-west-1.compute.amazonaws.com";
  //return "ec2-18-217-253-195.us-east-2.compute.amazonaws.com"; 
  //nanocomltd.co.il
  return "https://magos.co.il";
}  



/*start relax user functions*/
//-----------------------------------------------------------------------
var GetViewportScrollY = function() {
  if (document.documentElement && document.documentElement.scrollTop)
    return document.documentElement.scrollTop;
  else if (document.body && document.body.scrollTop)
    return document.body.scrollTop;
  else if (window.pageYOffset)
    return window.pageYOffset;
  else if (window.scrollY)
    return window.scrollY;
  else
    return 0;
}; // GetViewportScrollY

//-----------------------------------------------------------------------
var GetViewportScrollX = function() {
  if (document.documentElement && document.documentElement.scrollLeft)
    return document.documentElement.scrollLeft;
  else if (document.body && document.body.scrollLeft)
    return document.body.scrollLeft;
  else if (window.pageXOffset)
    return window.pageXOffset;
  else if (window.scrollX)
    return window.scrollX;
  else
    return 0;
}; // GetViewportScrollX

//-----------------------------------------------------------------------
function GetElementSize(oElement) {
  if (!oElement)
    return;

  var nElementWidth = typeof(oElement.clientWidth) == "number" ? oElement.clientWidth : oElement.offsetWidth;
  var nElementHeight = typeof(oElement.clientHeight) == "number" ? oElement.clientHeight : oElement.offsetHeight;

  return { width: nElementWidth, height: nElementHeight };
} // GetElementSize

//-----------------------------------------------------------------------
function GetWindowSize() {
  var nWinWidth = 0, nWinHeight = 0;
  if (typeof(window.innerWidth) == "number") {
  // Non-IE
   nWinWidth = window.innerWidth;
   nWinHeight = window.innerHeight;
 } else if (document.documentElement && (document.documentElement.clientWidth || document.documentElement.clientHeight)) {
   // IE 6+ in "standards compliant mode".
  	nWinWidth = document.documentElement.clientWidth;
	nWinHeight = document.documentElement.clientHeight;
 } else if (document.body && (document.body.clientWidth || document.body.clientHeight)) {
   // IE 4 compatible.
   nWinWidth = document.body.clientWidth;
   nWinHeight = document.body.clientHeight;
 }

 // alert( "Browser Window Width = " + nWinWidth + " Browser Window Height = " + nWinHeight);
return { width: nWinWidth, height: nWinHeight };
} // GetWindowSize

//-----------------------------------------------------------------------
function CenterElement(oElement) {

  if (!oElement)
    return;

  var oWinSize = GetWindowSize();
  var oElementSize = GetElementSize(oElement);

  g_scrollX = GetViewportScrollX();
  oElement.style.left = Math.max(0, /*GetViewportScrollX() +*/ ((oWinSize.width - oElementSize.width) >> 1)) + "px";

  g_scrollY = GetViewportScrollY();
  oElement.style.top = Math.max(0, /*GetViewportScrollY() +*/ ((oWinSize.height - oElementSize.height) >> 1)) + "px";

} // CenterElement

//-------------------------------------------------------
function GetDocumentSize() {

  var oDB = document.body;
  var oDE = document.documentElement;

  var nDocWidth = Math.max(oDB.scrollWidth,
                 oDE.scrollWidth,
				oDB.offsetWidth,
				oDE.offsetWidth,
				oDB.clientWidth,
				oDE.clientWidth);
			var nDocHeight = Math.max(oDB.scrollHeight,
				oDE.scrollHeight,
				oDB.offsetHeight,
				oDE.offsetHeight,
				oDB.clientHeight,
				oDE.clientHeight);

	// alert( "Browser Document Width = " + nWinWidth + " Browser Document Height = " + nDocHeight);
  return { width: nDocWidth, height: nDocHeight };
} // GetDocumentSize
 	
/*end relax user functions */ 
	
	
/********************************************************************/
String.prototype.replaceAll = function(str1, str2, ignore)
{
  return this.replace(new RegExp(str1.replace(/([\/\,\!\\\^\$\{\}\[\]\(\)\.\*\+\?\|\<\>\-\&])/g, "\\$&"), (ignore ? "gi" : "g")), (typeof (str2) == "string") ? str2.replace(/\$/g, "$$$$") : str2);
} // String.replaceAll


/********************************************************************/
Array.prototype.contains = function(k) {
  for(var i=0; i < this.length; i++){
    if(this[i] === k){
      return true;
    }
  }
  return false;
}

/********************************************************************/
// Read a page's GET URL variables and return them as an associative array.
function GetUrlVars(str)
{
    var vars = [], hash;
    var hashes; 
	
	if(typeof(str)=="undefined")
	  hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
    else
	  hashes = str.split('&');
	
	for(var i = 0; i < hashes.length; i++)
    {
        hash = hashes[i].split('=');
        vars.push(hash[0]);
        vars[hash[0]] = hash[1];
    }
    return vars;
}

/********************************************************************/
function ConvToUnixTimeStamp(dt){

  return Date.parse(dt)/1000;
}


/********************************************************************************************************/
function TimeConverter(UNIX_timestamp,getHour,doNotUseTimeOffset){
   
  var offset = 0;
  if(typeof(doNotUseTimeOffset)=="undefined")		  
    offset = new Date().getTimezoneOffset();
  var a = new Date((UNIX_timestamp) * 1000 +offset*60*1000);
  //var months = ['Jan','Feb','Mar','Apr','May','Jun','Jul','Aug','Sep','Oct','Nov','Dec'];
  var year = a.getFullYear();
  var month = a.getMonth() + 1;
  var date = a.getDate();
  var hour = a.getHours();
  var min = a.getMinutes();
  if(min<10)
    min = "0" + min;  
  
  var sec = a.getSeconds();
  var time = "";
  if (getHour)
    time = hour + ':' + min;
  else
    time = date + '/' + month + '/' + year;  
          
  return time;
}

 /********************************************************************************************************/
 function GetCallStatusStr(stat) {
 
   var statusStr = "";
   switch (stat) {
     case "1":
       statusStr = "חדשה";
       break;
     case "2":
       statusStr = "ממתינה לבצוע"
       break;
     case "3":
       statusStr = "בביצוע";
       break;
     case "31":
       statusStr = "נקראה";
       break;
     case "32":
       statusStr = "בנסיעה";
       break;
     case "33":
       statusStr = "בעבודה";
       break;
     case "34":
       statusStr = "בדיווח";
       break;
     case "6":
       statusStr = "הסתיימה";
       break;
     case "8":
       statusStr = "בהקפאה";
       break;
     case "9":
       statusStr = "מבוטלת";
       break;
     default:
       break;
    }
    return statusStr;
 }

 /********************************************************************************************************/
 function GetCallTypeStr(type) {
 
   var typeStr = "";
   switch (type) {
     case "1":
       typeStr = "שוטפת";
       break;
     case "2":
       statusStr = "מונעת"
       break;
     case "3":
       statusStr = "פרויקט";
       break;
     default:
       break;
    }
    return typeStr;
 } 
 
/********************************************************************/
function IsNormalInteger(str) {
    return /^\+?(0|[1-9]\d*)$/.test(str);
}
 
/********************************************************************/
function IsNormalFloat(str) {
  return parseFloat(str.match(/^-?\d*(\.\d+)?$/))>0;
}

/********************************************************************/
function ShowRelaxUser(dlgId,toShow){
		
  
    if (!document.getElementById(dlgId)) {
      var oRelUser = document.createElement("div");
	  document.body.appendChild(oRelUser);
	  oRelUser.outerHTML = "<div id='relaxUserDlg' style='position:absolute;z-index:10001;visibility:hidden;background-color:transparent;width:100%;height:100%;'><IMG style='position:absolute;top:40%;left:35%;' src='img/relax.gif' width=100 height=100></div>";
    }
  
    if (!document.getElementById("cover")) {
      var oCover = document.createElement("div");
	  oCover.id = "cover";
	  document.body.appendChild(oCover);
    }

    if(toShow!=null && toShow==true){
  
	var oWorkersDialog = document.getElementById(dlgId);
    CenterElement(oWorkersDialog);

    // Set cover height and width to documents ones.
    var oDocSize = GetDocumentSize();
    var oCover = document.getElementById("cover");
    oCover.style.width = oDocSize.width + "px";
    oCover.style.height = oDocSize.height + "px";
    // Set the visiblity of cover to visible
    document.getElementById("cover").style.visibility = "visible";
    // Set the visibility of box to visible.
    oWorkersDialog.style.visibility = "visible";
  
  }
}	
		

/********************************************************************/
function HideRelaxUser(dlgId){

  if(document.getElementById("cover")!=null){
    document.getElementById("cover").style.visibility = "hidden";
    document.getElementById(dlgId).style.visibility = "hidden";
  }
}
 
 
 /********************************************************************/
function GetCallsCount(){
  
  var qryStr = "op=getCallsCount"; 
  var userCode="tech";
  if(typeof(window.localStorage)!="undefined" && window.localStorage.getItem("UserName")!=null)
    userCode = window.localStorage.getItem("UserName");
  
  qryStr+="&userCode=" + userCode; 
  
  /*
  if(window.localStorage.getItem("techOnline")!=null && window.localStorage.getItem("techOnline")=="X"){
    HideRelaxUser('relaxUserDlg');
	if(window.localStorage.getItem(qryStr)!=null)
	  return window.localStorage.getItem(qryStr);
    else
      alert("נתונים לא קיימים באחסון המקומי");
  }
   */
  if(window.localStorage.getItem(qryStr)!=null){
	  HideRelaxUser('relaxUserDlg');
  	  DownloadcallListToPhone();
	  return window.localStorage.getItem(qryStr);
  }
   
  var callsCount = 0;
  $.ajax({
    type: "POST",
    url:  GetServerIp() + "/tech_post.php",
    data: qryStr ,
	headers: {"Content-type" : "application/x-www-form-urlencoded"},
    crossDomain: true,
	cache: false,
	async: false,
	success: function (msg) {
      callsCount = msg;
	  window.localStorage.setItem(qryStr,callsCount);
	  DownloadcallListToPhone();
	},
    error: function (e) {
 	  alert(JSON.stringify(e));
	  alert("קיימת בעיית תקשורת");
	  
	  //alert(e); 
	}
   });
 	  HideRelaxUser('relaxUserDlg');	
   	  return callsCount;
  }



/********************************************************************/
function GetData(qryStr){
   
  if(window.localStorage.getItem(qryStr)!=null && (/*qryStr.indexOf("op=getAllCalls")==-1 &&*/ qryStr.indexOf("op=getAllParts")==-1 || 
      (qryStr.indexOf("op=getAllParts")!=-1 && qryStr.indexOf("forPartChoice")!=-1)) 
	){
		  
	HideRelaxUser('relaxUserDlg');
    return JSON.parse(window.localStorage.getItem(qryStr));
  }
  
  var json = null;
  $.ajax({
    type: "POST",
    url:  GetServerIp() +  sessionStorage.getItem("qryPage"),   //"/tech_post.php",
    data: qryStr ,
	headers: {"Content-type" : "application/x-www-form-urlencoded"},
    crossDomain: true,
	cache: false,
	async: false,
	success: function (repCallRec) {
 	  //refresh all parts
	  if(qryStr.indexOf("op=getAllParts")!=-1 && qryStr.indexOf("forPartChoice")==-1){
	    localStorage.setItem("op=getAllParts&forPartChoice=1", repCallRec);
	    localStorage.setItem("op=getAllParts&forPartChoice=1" + "_Time", new Date());
	  }
	  if(qryStr.indexOf("op=getUnitTypes")!=-1){
	    localStorage.setItem("op=getUnitTypes", repCallRec);
	  }
    /*
    if(qryStr.indexOf("op=getCallRep&callCode=")!=-1){
	    localStorage.setItem(qryStr, repCallRec);
	  } */
        
    json = JSON.parse(repCallRec);
  },
    error: function (e) {
  	  alert("קיימת בעיית תקשורת");
	  //alert(e); 
	}
   });
   	  HideRelaxUser('relaxUserDlg');
	  return json;
  }


/********************************************************************/
  function GetCallFld(callCode,unitCode,section,fld){
    var json = GetData("op=getCallRep&callCode=" + callCode);
  	for(i=0;i<json.length;i++){
	  if(section=="main")
		  return json[0][fld];
    if(json[i] && typeof(json[i][section])!="undefined"){
	    if(section=="supPartsData")
		    return JSON.parse(json[i][section]);
		  repDataJson = JSON.parse(json[i][section]);
		  for(j=0;j<repDataJson.length;j++){ 
		    if(repDataJson[j] && repDataJson[j].unitCode!==undefined && repDataJson[j].unitCode == unitCode){
			  //alert(repDataJson[j][fld]);
			    return repDataJson[j][fld];
	      }
	    }
	  }
	  
	 }
  }
  
  function ShowCallOrUnitDtls(callCode,unitCode,targetWin,isCall){ 
    if(isCall)
	  window.location='callDtls.html?callCode=' + callCode + "&unitCode=" + unitCode + "&targetWin=" + targetWin;
    else
      window.location='unitDtls.html?callCode=' + callCode + "&unitCode=" + unitCode + "&targetWin=" + targetWin;
  }
/********************************************************************/
function UpdData(qryStr,toAlertRetStr,navigateTo){
  
  var hashArr = GetUrlVars(qryStr);
   
  if(qryStr.indexOf("&syncToServer=1")==-1){
    console.log(SaveQueryOffline);
    SaveQueryOffline(qryStr,navigateTo);   
    return;
  }
  
  var isSuccess = true;  
  
  callCode =  hashArr["callCode"];
  if(qryStr.indexOf("uploadImage")==-1){
    var qry = JSON.stringify(GetData("op=getCallRep&callCode=" + callCode));
	  postData = "op=updCallRep&qry=" + qry.replaceAll("&","%26");
  }
  else {
    postData = qryStr;  
  }
  $.ajax({
     type: "POST",
     url:  GetServerIp() + sessionStorage.getItem("qryPage"), //"/tech_post.php",
     data: postData ,
	 headers: {"Content-type" : "application/x-www-form-urlencoded"},
     crossDomain: true,
	 async: false,
	 success: function (retStr) {
    if(toAlertRetStr || retStr.trim()!=""){	
	     console.log(retStr);
		   alert(retStr);
	   }
     // Query execution in db failed 
     if(retStr.trim()!="") {
       isSuccess = false;
       alert(".העדכון לא הצליח – נא לנסות שוב או לפנות לאחראי המערכת");  
     }
     if(navigateTo!=null){ 
	 	   window.location = navigateTo;
	   }	
	 },
     error: function (e) {
       HideRelaxUser('relaxUserDlg');
	   
     //alert(JSON.stringify(e));
	   alert("קיימת בעיית תקשורת, הנתונים יישמרו במכשיר. יש לבצע סיום משימה מחדש כאשר יש תקשורת");
	   SaveQueryOffline(qryStr,navigateTo);
	   isSuccess = false;
	   //alert(e); 
	 }
   });
   return isSuccess;
 }
 
/********************************************************************/
function UpdDataAsync(qryStr,toAlertRetStr,navigateTo){
    
	if(qryStr.indexOf("&syncToServer=1")==-1){
      SaveQueryOffline(qryStr,navigateTo);   
      return;
    }
      
   $.ajax({
     type: "POST",
     url:  GetServerIp() + "/tech_post.php",
     data: qryStr ,
	 headers: {"Content-type" : "application/x-www-form-urlencoded"},
     crossDomain: true,
	 async: true,
	 success: function (retStr) {
       //if(toAlertRetStr)	
	     //alert(retStr);
	   //if(navigateTo!=null) 
         //window.location = navigateTo;
  	 },
     error: function (e) {
       alert("קיימת בעיית תקשורת");
	   SaveQueryOffline(qryStr,navigateTo);
	   //alert(e); 
	 }
  });
 }
 
/********************************************************************/
function DisableUpdateForCallThatIsEnded(callStatus){

  //disable page controls if call ended or current call does not belong to logged in user   
  if(callStatus==CALL_ENDED ||  (window.localStorage.getItem("callUserCode").toLowerCase().trim()!= window.localStorage.getItem("UserName").toLowerCase().trim()) ){
    var x = document.getElementsByTagName("INPUT");
    for (i = 0; i < x.length; i++) {
      if(typeof(x[i].id)!="undefined" && x[i].id!="btnSignature")
	    x[i].disabled = true;
    }
    x = document.getElementsByTagName("TEXTAREA");
    for (i = 0; i < x.length; i++) {
      x[i].disabled = true;
    } 
  
    x = document.getElementsByTagName("BUTTON");
    for (i = 0; i < x.length; i++) {
      if(x[i].id == "btnSync")
		x[i].disabled = true;
    }
  
    //supParts
	if(document.getElementById("addRowSupParts")!=null)
	  document.getElementById("addRowSupParts").disabled = true;
    //supParts
	if(document.getElementById("delRowSupParts")!=null)
	  document.getElementById("delRowSupParts").disabled = true;
    //camera
	if(document.getElementById("btnActions")!=null)
      document.getElementById("btnActions").disabled = true;
	
  }
} 

/********************************************************************/
function CreateHeader(isSync){
  
  if(isSync)
    document.write("<TABLE dir='ltr' style='width:100%;' border=1><TR><TD width='12%'><button id='btnSync' onclick='UpdateAndExit();SyncOfflineQueriesToServer(%callCode%,null,false);'>שמור</button></TD><TD width='12%'>&nbsp;</TD><TD width='12%'>&nbsp;</TD><TD width='12%'>&nbsp;</TD><TD width='12%'>&nbsp;</TD><TD id='tdCallsCount' width='12%' style='background-color:#87CEFA'>&nbsp;</TD><TD width='12%'>&nbsp;</TD><TD id='tdOnline' width='12%' style='background-color:#87CEFA'>&nbsp;</TD></TR></TABLE>");
  else
      document.write("<TABLE dir='ltr' style='width:100%;' border=1><TR><TD width='12%'>&nbsp;</TD><TD width='12%'>&nbsp;</TD><TD width='12%'>&nbsp;</TD><TD width='12%'>&nbsp;</TD><TD width='12%'>&nbsp;</TD><TD id='tdCallsCount' width='12%' style='background-color:#87CEFA'>&nbsp;</TD><TD width='12%'>&nbsp;</TD><TD id='tdOnline' width='12%' style='background-color:#87CEFA'>&nbsp;</TD></TR></TABLE>");
  
} 

/********************************************************************/
function RegisterToPing() {

  var p, success, err, ipList;
  ipList = [{query: 'www.google.com', timeout: 4,retry: 1,version:'v4'}];
            
  pingSuccess = function (results) {
    	   
    if(results[0].response.result.pctLoss=="100%"){  	
	  window.localStorage.setItem("techOnline","X");
	  document.getElementById("tdOnline").innerText = "X";
      document.getElementById("tdOnline").style.backgroundColor="red";
	  /*
	  var elems = document.getElementsByTagName("BUTTON");
	  for (i = 0; i < elems.length; i++) 
        elems[i].disabled = true;
	  elems = document.getElementsByTagName("INPUT");
	  for (i = 0; i < elems.length; i++) 
        elems[i].disabled = true;
	  elems = document.getElementsByTagName("TEXTAREA");
	  for (i = 0; i < elems.length; i++) 
        elems[i].disabled = true;
	  */
	}
	else{ 
	 window.localStorage.setItem("techOnline","V");
	 document.getElementById("tdOnline").innerText = "V";
	 document.getElementById("tdOnline").style.backgroundColor="#87CEFA";
	 //SyncOfflineQueriesToServer();
	 
	 /*
	 var elems = document.getElementsByTagName("BUTTON");
	  for (i = 0; i < elems.length; i++) 
        elems[i].disabled = false;
	  elems = document.getElementsByTagName("INPUT");
	  for (i = 0; i < elems.length; i++) 
        elems[i].disabled = false;
	  elems = document.getElementsByTagName("TEXTAREA");
	  for (i = 0; i < elems.length; i++) 
        elems[i].disabled = false;
	  */ 
	}	
	//console.log(results);
  	   //alert(results);
	   //alert(results.length); 
	   //alert(results[0].response.result.pctLoss);
	    //for(var name in results[0].response.result.pctLoss){
              //alert(name);
	    //}
	  
	};
  	pingErr = function (e) {
      //console.log('Error: ' + e);
  	  alert("ping err");
	};
  	if(typeof(Ping)!="undefined"){
	  p = new Ping();
  	  p.ping(ipList, pingSuccess, pingErr);
	  setInterval(function() { p.ping(ipList, pingSuccess, pingErr);},10000);
    }
} 
 
/********************************************************************/
function UpdateHeader(callCode){
  
  RegisterToPing();
  
  if(window.localStorage.getItem("techOnline")!=null && window.localStorage.getItem("techOnline")=="X"){
    document.getElementById("tdOnline").innerText = "X";
    document.getElementById("tdOnline").style.backgroundColor="red";
  }	  
  else{
    document.getElementById("tdOnline").innerText = "V";
    document.getElementById("tdOnline").style.backgroundColor="#87CEFA";
  }	  
   
  // Do this in order to avoid download to phone on each screen
  ////if(!window.sessionStorage.getItem("callsCount")) {
    document.getElementById("tdCallsCount").innerText = GetCallsCount();
    ////window.sessionStorage.setItem("callsCount",document.getElementById("tdCallsCount").innerText);
  ////} else {
    ////document.getElementById("tdCallsCount").innerText = window.sessionStorage.getItem("callsCount");
  ////}
  
  if(typeof(callCode)!="undefined") {
    document.getElementById("btnSync").outerHTML = document.getElementById("btnSync").outerHTML.replace("%callCode%",callCode);  
  }
}	
 
/********************************************************************/
function RegisterToTouchEvent(elem,fnForLeftSwipe,prmForLeftSwipe,fnForRightSwipe,prmForRightSwipe){
 
  var xDown = null;                                                       
  var yDown = null;                                                        

  handleTouchStart= function(evt) {                                         
    xDown = evt.touches[0].clientX;                                      
    yDown = evt.touches[0].clientY;                                      
  };                                                
  
  handleTouchMove = function(evt) {
    
	if ( ! xDown || ! yDown ) {
        return;
    }

    var xUp = evt.touches[0].clientX;                                    
    var yUp = evt.touches[0].clientY;

    var xDiff = xDown - xUp;
    var yDiff = yDown - yUp;

    if ( Math.abs( xDiff ) > Math.abs( yDiff ) ) {/*most significant*/
        if ( xDiff > 0 ) 
          fnForLeftSwipe(prmForLeftSwipe);    
		 //alert("left swipe"); 
        else 
          fnForRightSwipe(prmForRightSwipe);
		  //alert("right swipe"); 
    }                           
         
    /*
    if ( Math.abs( xDiff ) > Math.abs( yDiff ) ) {
     
        if ( yDiff > 0 ) 
            alert("up swipe"); 
         else  
            alert("down swipe"); 
         
                                                                         
    }
    */
	/* reset values */
    xDown = null;
    yDown = null;                                             
  };
  
  
  elem.addEventListener('touchstart', handleTouchStart, false);        
  elem.addEventListener('touchmove', handleTouchMove, false);

} 	 

/********************************************************************/
function IsTimeToDownloadDataToPhone(qryStr){
   var diff = -1;
   if(localStorage.getItem(qryStr + "_Time")!=null){
     var diff = ((new Date()).getTime() - (new Date(localStorage.getItem(qryStr + "_Time"))).getTime()) / (1000*60);
   }
   else {
    alert("null");
   }
   return (diff == -1) || (diff > 5);
}	


/********************************************************************/
function DownloadDataToPhone(qryStr){
  
  $.ajax({
    type: "POST",
    url:  GetServerIp() + "/tech_post.php",
    data: qryStr ,
	headers: {"Content-type" : "application/x-www-form-urlencoded"},
    crossDomain: true,
	cache: false,
	async: true,
	success: function (repCallRec) {
    //repCallRec = JSON.stringify(repCallRec);
	  localStorage.setItem(qryStr, repCallRec);
	  localStorage.setItem(qryStr + "_Time", new Date());
	  //alert("end download data " + qryStr);
	  if(qryStr.indexOf("op=getAllCalls") != -1)
	    DownloadCallsDataToPhone(repCallRec);
	},
    error: function (e) {
      //alert("error download to phone");
      //alert("error download to phone");
	  //alert(e); 
	}
   });
  }

/********************************************************************/
//this function is not in use
function  DownloadCallImageToPhone(callCode,imgInd){

  return;
  qryStr  = "op=getImage&callCode=" + callCode + "&ind=" + imgInd;
  $.ajax({
    type: "POST",
    url:  GetServerIp() + "/tech_post.php",
    data: qryStr ,
	headers: {"Content-type" : "application/x-www-form-urlencoded"},
    crossDomain: true,
	cache: false,
	async: true,
	success: function (json) {
 	  json=JSON.parse(json);
	  if(json.imgData.length>0)
	    localStorage.setItem(callCode + "_" + imgInd, "data:image/jpeg;base64," + json.imgData);
	  //alert("end download data " + qryStr);
	},
    error: function (e) {
      //alert("error download image to phone");
	  //alert(e); 
	}
   });
}	
 
  
  
/********************************************************************/
 function DownloadcallListToPhone(){
   
   //alert("DownloadcallListToPhone");
   var userCode = window.localStorage.getItem("UserName");
    
   var qryStr =  "op=getAllCalls" + "&userCode=" + userCode;
   //if(IsTimeToDownloadDataToPhone(qryStr))    
   DownloadDataToPhone(qryStr);
   DownloadAllPartsToPhone();
   //if(localStorage.getItem(qryStr)!=null){   
     //var callList = localStorage.getItem(qryStr);   
     //DownloadCallsDataToPhone(callList);
   //}
 } 	 


/********************************************************************/
function RemoveCallDataFromPhone(currCall){

  //alert("start RemoveCallDataFromPhone " + currCall);
  localStorage.removeItem("op=getCallRep&callCode=" + currCall);
  localStorage.removeItem("op=getCallRep&callCode=" + currCall + "_Time");
} 
 
/********************************************************************/
function DownloadCallsDataToPhone(callList){

  //alert("DownloadCallsDataToPhone");
  var  callsArr = [];
  var callsIdArr = [];
  var json = JSON.parse(callList); 
  console.log("json", json);
  for (i = 0; i < json.length; i++) {
    if (json[i] && typeof (json[i].code) != "undefined") {
	    callsArr.push(json[i].code);
      callsIdArr.push(json[i].id);
	  }
  }
  
  //delete from phone calls that were deleted/updated from db
  for(var key in localStorage) {
    if(key.indexOf("op=getCallRep&callCode=")!=-1){
      var res = key.split("callCode=");
	    var currCall = res[1];
	    currCall = currCall.replace("_Time","");
	    var callInd = callsArr.indexOf(currCall);
	    if(callInd==-1){ // call was deleted from db
	      RemoveCallDataFromPhone(currCall); 	
	    }
	    else{
	      var json = GetData("op=getCallRep&callCode=" + currCall);
  	    for(i=0;i<json.length;i++){
		      if(json[i] && typeof(json[i].repData)!="undefined"){
		        repDataJson = JSON.parse(json[i].repData);
            for(j=0;j<repDataJson.length;j++) 
			        // call was updated on db
              if(repDataJson[j] && repDataJson[j].unitCode==0 && (typeof(repDataJson[j].callID)=="undefined" || repDataJson[j].callID!=callsIdArr[callInd]))
		            RemoveCallDataFromPhone(currCall); 
		      } 	
		    }
      }
	  }
  }
   
  for (i = 0; i < callsArr.length; i++) {
      var callCode = callsArr[i];
	  //try{
	  if(localStorage.getItem("op=getCallRep&callCode=" + callCode)==null)
	    DownloadDataToPhone("op=getCallRep&callCode=" + callCode);
 }	
}	

/********************************************************************/
function DownloadAllPartsToPhone(){
  
  var qryStr = "op=getAllParts&forPartChoice=1";
  if(IsTimeToDownloadDataToPhone(qryStr)){	
	DownloadDataToPhone(qryStr);
    qryStr = "op=getUnitTypes";
    DownloadDataToPhone(qryStr);
  }
}	

  
/********************************************************************/
function SaveCameraImageToFile(fileName,imgData){

try{
    	
	window.resolveLocalFileSystemURL(cordova.file.dataDirectory, function (directoryEntry) {
	  directoryEntry.getFile(fileName, { create: true }, function (fileEntry) {
	    fileEntry.createWriter(function (fileWriter) {
		  fileWriter.onwriteend = function (e) {
		    //alert("write end11");
		  };

		  fileWriter.onerror = function (e) {
		    alert('Write failed: ' + e.toString());                      
		   // you could hook this up with our global error handler, or pass in an error callback
		   console.log('Write failed: ' + e.toString());
		  };

	  	  var blob = new Blob([imgData], { type: 'text/plain' });
		  fileWriter.write(blob);
		}, FileErrorHandler.bind(null, fileName));
	   }, FileErrorHandler.bind(null, fileName));
	  }, FileErrorHandler.bind(null, fileName));
	 }
	 catch(err){
	   alert(err);
	   alert(err.message);
	 }
}	

  
/********************************************************************/
function SaveQueryOffline(qryStr,navigateTo){

  console.log("SaveQueryOffline=" + qryStr);
  var hashArr = GetUrlVars(qryStr);
  callCode =  hashArr["callCode"];
  unitCode =  hashArr["unitCode"];
	
  if (typeof(window.localStorage)!="undefined"){
     itm = "op=getCallRep&callCode=" + callCode;
	   if(qryStr.indexOf("updUnitHandling")!=-1){ //update unit handling
	     handlingCode =  hashArr["handlingCode"];
       isJobDone =  hashArr["isJobDone"];
	     var relI = -1;
	
	     var json = JSON.parse(window.localStorage.getItem(itm));
	     for(i=0;i<json.length;i++){
	       if(json[i] && typeof(json[i].unitHandlingData)!="undefined" ){
		       relI = i;
		       unitHandlingDataJson = JSON.parse(json[i].unitHandlingData);
			     for(j=0;j<unitHandlingDataJson.length;j++){ 
			       if (unitHandlingDataJson[j] && unitHandlingDataJson[j].unitCode == g_UnitCode  && unitHandlingDataJson[j].handlingCode == handlingCode){
				       unitHandlingDataJson[j].isDone = isJobDone; 
			       }   
			     }    
		    }
	     }
	     json[relI].unitHandlingData = JSON.stringify(unitHandlingDataJson);
	     window.localStorage.setItem(itm,JSON.stringify(json));
	   }
	 
	 
	   if(qryStr.indexOf("updSupParts")!=-1){ //update sup parts
	     var descArr = hashArr["desc"].split('|||');
	     var quantityArr = hashArr["quantity"].split('|||');
	     var codeArr = hashArr["code"].split('|||');
	     var barcodeArr = hashArr["barcode"].split('|||');
	  
	     var relI = -1;
	     var json = JSON.parse(window.localStorage.getItem(itm));
	     var supPartsDataJson =  {};
	     for(i=0;i<json.length;i++){
	       if(json[i] && typeof(json[i].supPartsData)!="undefined"){
		       relI = i;
			     supPartsDataJson = JSON.parse(json[i].supPartsData);
			     for(j=supPartsDataJson.length-1;j>=0;j--){ 
			       if(supPartsDataJson[j]&& supPartsDataJson[j].unitCode == unitCode)
			         supPartsDataJson.splice(j,1); 
			      //delete supPartsDataJson[j];
			     }
		     } 	
	     }
	
	     for(i=0;i<descArr.length-1;i++){	
	       var supJson = {"desc_" :"","quantity":"","code":"","barcode":"","unitCode":unitCode };
		     supJson.desc_ =  decodeURIComponent(descArr[i]).replaceAll("''","'");
		     supJson.quantity =  quantityArr[i];
		     supJson.code =  codeArr[i];
		     supJson.barcode =  barcodeArr[i];
		     supPartsDataJson.push(supJson); //.splice(0, 0, supJson);
 	    }
	  
	    json[relI].supPartsData = JSON.stringify(supPartsDataJson);
	    window.localStorage.setItem(itm,JSON.stringify(json));
	  }

    if(qryStr.indexOf("updCallRep")!=-1){
	    if(window.localStorage.getItem(itm)!=null){
		    var relJson ={};
		    var repDataJson ={};
		    var relI = -1;
		    var relJ = -1;
		    var json = JSON.parse(window.localStorage.getItem(itm));
	      for(i=0;i<json.length;i++){
		      if(json[i] && typeof(json[i].repData)!="undefined"){
		        relI = i;
			      repDataJson = JSON.parse(json[i].repData);
			      for(j=0;j<repDataJson.length;j++){ 
			        if(repDataJson[j] && repDataJson[j].unitCode == unitCode){
			          relJ = j;
			  	      relJson = repDataJson[j];
			        }
			      }
		      } 	
		    }
	  
        var keysArr =  Object.keys(hashArr);
	      for(i=0;i< keysArr.length;i++){
	        var key = keysArr[i];
		      if(relJson.hasOwnProperty(key))
            relJson[key] = decodeURIComponent(hashArr[key]).replaceAll("''","'");	  
        }	
		  
		    repDataJson[relJ] =  relJson; 
		    json[relI].repData = JSON.stringify(repDataJson);
        window.localStorage.setItem(itm,JSON.stringify(json));
	      console.log("set item=" + itm);
      }
	
    }	
	
	  if(navigateTo!=null) {
      setTimeout(() =>{
        window.location = navigateTo;
      },0);
      
    }
  }
}

/********************************************************************/
function DoPostSyncToServerOps(callCode,isEndMission,navigateTo){

   DownloadcallListToPhone();
   document.getElementById("tdCallsCount").innerText = GetCallsCount();
   
	 //after mission is ended , delete call localstorage items
	 if(isEndMission!=null && isEndMission==true){
	    for(var key in localStorage) {
          //call images          
		  if(key.indexOf("&callCode=" + callCode)!=-1 || key.indexOf(callCode + "_")!=-1){
			localStorage.removeItem(key);
		  } 	
		} 	
    }	  
	  
	 if(isEndMission!=null && isEndMission==true){
	   QueryForExecRep(callCode,3,null,null,navigateTo);
	 }
	 else{
	   if(navigateTo!=null)  
         window.location = navigateTo;
	 }

}


/********************************************************************/
function SyncImageToServer(callCode,fileName,isEndMission,navigateTo){
      
    window.resolveLocalFileSystemURL(cordova.file.dataDirectory, 
      function(directoryEntry) {
        console.log('Directory Entry Log - ', directoryEntry);
        directoryEntry.getFile(fileName, { create: true, exclusive: false },

            function(fileEntry) {
                fileEntry.file(

                    function(file){
                        var reader = new FileReader();
                        reader.onloadend = function(evt) {
                  		  imgDat = reader.result;
						  unitCode = fileName.split("_")[2];
						  qry = "op=uploadImage&callCode=" + callCode + "&unitCode=" + unitCode + "&syncToServer=1&imgData=" + encodeURIComponent(imgDat);
              var ky = fileName.replace(".txt","");
              if(localStorage[ky] !== "DELETED") {
                UpdData(qry,false);
              }
              RemoveLocalFile(fileEntry);
						  localStorage.removeItem(fileName.replace(".txt",""));
						  
						  for(var key in localStorage){
							if(key.indexOf("img_" + callCode)!=-1){  
								SyncImageToServer(callCode,key + ".txt",isEndMission,navigateTo);
								return;
							}
						  }
						  HideRelaxUser('relaxUserDlg');
						  DoPostSyncToServerOps(callCode,isEndMission,navigateTo);
						  
						};
                        reader.readAsText(file);
                    },
                    function(error) {
                        alert('File Read cannot complete on File System - ', error);
                    }
                );
            }, function(error) {
                alert('Reader cannot read from the File System - ', error);
            }
        );
        }, function(error) {
          alert('Error - ', error);
        }
      );
}

/********************************************************************/
function SyncOfflineQueriesToServer(callCode,navigateTo,isEndMission){

  ShowRelaxUser('relaxUserDlg',true);
  setTimeout(function(){ DoSyncOfflineQueriesToServer(callCode,navigateTo,isEndMission); },200);
}

/********************************************************************/
function DoSyncOfflineQueriesToServer(callCode,navigateTo,isEndMission){
   //find all unitcode for call     
   itm = "op=getCallRep&callCode=" + callCode;
   json = GetData(itm);
   var callStatus = json[0]["callStatus"];
   
   if(isEndMission){ 	  
     json[0]["callStatus"] = CALL_ENDED;
     window.localStorage.setItem(itm,JSON.stringify(json));
   }
   qry = "callCode=" + callCode;
   qry += "&syncToServer=1";
   
      
   var isSuccess = UpdData(qry,false);
   // if update failed do not continue
   if(!isSuccess){
     // update failed , rollback
	 if(isEndMission){ 	  
       json[0]["callStatus"] = callStatus;
       window.localStorage.setItem(itm,JSON.stringify(json));
     }
     return;
   }
      
   for(var key in localStorage){
     if(key.indexOf("img_" + callCode)!=-1){  
       SyncImageToServer(callCode,key + ".txt",isEndMission,navigateTo);
	   return;
     }
   }
   HideRelaxUser('relaxUserDlg');
   DoPostSyncToServerOps(callCode,isEndMission,navigateTo);
}	


/********************************************************************/
function ShowExecRep(callCode){

  //callCode="17111969"; 
  //ShowExecRepIfExists("http://ec2-52-37-115-198.us-west-2.compute.amazonaws.com/tech_post.php");
  
  //if(g_ExecRepFileName!=null && g_ExecRepFileName.length>0)
  window.location= "https://docs.google.com/viewer?url=" + GetServerIp() +  "/Images/" + callCode + "_rep.pdf&embedded=true";  //'execRep.html?callCode=' + g_CallCode + "&callStatus=" + g_CallStatus + "&custName=" + document.getElementById("divCustName").innerText;
  //else 
  //alert("לא קיים דוח ביצוע לקריאה");
}

/********************************************************************/
function GetPosition(el){

  var x=0,y= 0;
  while(el) {
    x+=(el.offsetLeft-el.scrollLeft+el.clientLeft);
	y+=(el.offsetTop-el.scrollTop+el.clientTop);
	el=el.offsetParent;
  }
  return{x:x,y:y}
}


/********************************************************************/
function FileRemoveSuccess(entry) {
   //alert("Removal succeeded");
}

function FileRemoveFail(error) {
    alert('Error removing file: ' + error.code);
}


/********************************************************************/
function RemoveLocalFile(entry){

  // remove the file
  entry.remove(FileRemoveSuccess, FileRemoveFail);	
}


/********************************************************************/
function UploadToFtp(fileEntry,localPath,callCode,recType,reqInd,reqRemarks,navigateTo,isDoAlert) {
 
 //alert("start upload to ftp " + reqType + "," + reqInd);
 localPath=localPath.replace("file://","");
 //alert(localPath);
 
 window.cordova.plugin.ftp.connect('mail.hca.co.il', 'mobileftp', 'Mobileftp11', function(ok) {
        //alert("ftp: connect ok=" + ok);
        //alert('Tec-Phone-Put-' + callCode +  '.txt');
		
        // You can do any ftp actions from now on...
        window.cordova.plugin.ftp.upload(localPath, 'Tec-Phone-Put-' + callCode +  '.txt', function(percent) {
            if (percent == 1) {
              if(isDoAlert)
			    alert("בקשה להפקת דוח ביצוע נשלחה לשרת");     
			  RemoveLocalFile(fileEntry); 	
			  if(recType==2)
	    		UpdData("op=updCallRep&callCode=" + g_CallCode + "&reqInd=" + reqInd + "&reqRemarks=" + reqRemarks , false, "callActions.html?callCode=" + callCode + "&custName=" + g_CustName);
			  if(recType==3)
				window.location = navigateTo;
			} 
			else {
                console.debug("ftp: upload percent=" + percent * 100 + "%");
            }
        }, function(error) {
            alert("ftp: upload error=" + error);
        });

    }, function(error) {
        alert("ftp: connect error=" + error);
    });
}


/********************************************************************/
var FileErrorHandler = function (fileName, e) {  
    var msg = '';

    switch (e.code) {
        case FileError.QUOTA_EXCEEDED_ERR:
            msg = 'Storage quota exceeded';
            break;
        case FileError.NOT_FOUND_ERR:
            msg = 'File not found';
            break;
        case FileError.SECURITY_ERR:
            msg = 'Security error';
            break;
        case FileError.INVALID_MODIFICATION_ERR:
            msg = 'Invalid modification';
            break;
        case FileError.INVALID_STATE_ERR:
            msg = 'Invalid state';
            break;
        default:
            msg = 'Unknown error';
            break;
    };

    alert('Error (' + fileName + '): ' + msg);
}

/********************************************************************/
function QueryForExecRep(callCode,recType,reqInd,reqRemarks,navigateTo,isDoAlert) {
 
  try{
    fileName = callCode + ".txt";
	data = "Put\r\n";
	data+="Chr Set ~ Ansii\r\n" 
    data+="ID ~ " + callCode + "\r\n";
    data+="Record-Type~" + recType + "\r\n";
    if(recType==2){
	  data+="Request-Code~" +  reqInd + "\r\n";  
	  data+="Remarks~" +  reqRemarks;
	}
	
	//data = JSON.stringify(data, null, '\t');
	window.resolveLocalFileSystemURL(cordova.file.dataDirectory, function (directoryEntry) {
	  directoryEntry.getFile(fileName, { create: true }, function (fileEntry) {
	    fileEntry.createWriter(function (fileWriter) {
		  fileWriter.onwriteend = function (e) {
		    //alert("write end11");
			//alert(fileEntry.toNativeURL());
			// for real-world usage, you might consider passing a success callback
			//alert('Write of file "' + fileName + '"" completed.');
		    UploadToFtp(fileEntry,fileEntry.toNativeURL(),callCode,recType,reqInd,reqRemarks,navigateTo,isDoAlert);
		  };

		  fileWriter.onerror = function (e) {
		    alert('Write failed: ' + e.toString());                      
		   // you could hook this up with our global error handler, or pass in an error callback
		   console.log('Write failed: ' + e.toString());
		  };

	  	  var blob = new Blob([data], { type: 'text/plain' });
		  fileWriter.write(blob);
		}, FileErrorHandler.bind(null, fileName));
	   }, FileErrorHandler.bind(null, fileName));
	  }, FileErrorHandler.bind(null, fileName));
	 }
	 catch(err){
	   alert(err);
	   alert(err.message);
	 }
}


