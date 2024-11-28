<?php
header("Access-Control-Allow-Origin: *");
$mysqli = new mysqli('localhost','root', 'a1b2c3d5','dbTech'); 
if ($mysqli->connect_error) {
    die('ERROR: Can\'t connect to MySQL - ' . $mysqli->connect_error);
}
$mysqli->set_charset("utf8") or die('Error setting charset');
		
	    

/////////////////////////////////////////////////////////////
function RecordSetToJson($mysql_result) {
  $rs = array();
  while($rs[] = $mysql_result->fetch_assoc()) {
    // you don´t really need to do anything here.
  }
  return json_encode($rs);
}

/////////////////////////////////////////////////////////////
function SaveImage($mysqli,$callCode,$unitCode,$imgData){
  
  $img = $imgData;
  $img = str_replace('data:image/jpeg;base64,', '', $img);
  $img = str_replace(' ', '+', $img);
  
  $imgData = base64_decode($img);
  
  $result=$mysqli->query("SELECT numImages FROM tblRep WHERE callCode='$callCode' AND unitCode='$unitCode'") or die(mysqli_error());
  $data = $result->fetch_assoc();
  $numImages = $data['numImages']; 
  if($unitCode==0)
    $file = 'Images/'.$callCode.'_'.$numImages.'.jpg';
  else
    $file = 'Images/'.$callCode.'_'.$unitCode.'_'.$numImages.'.jpg';  
  $success = file_put_contents($file, $imgData);
  return $success;
}

/////////////////////////////////////////////////////////////
function GetImage($callCode,$unitCode,$imgInd){

  if($unitCode==0)
    $file = 'Images/'.$callCode.'_'.$imgInd.'.jpg';
  else
    $file = 'Images/'.$callCode.'_'.$unitCode.'_'.$imgInd.'.jpg';
  $data = file_get_contents($file);
  $img = base64_encode($data); 
  //$img = str_replace('+', ' ', $img);
  return $img;
}	

/////////////////////////////////////////////////////////////
function UpdSupParts($mysqli,$callCode,$unitCode,$descArr,$quantityArr,$codeArr,$barcodeArr,$source){
		 
  //delete old sup parts
  $query = "DELETE FROM tblSuppParts WHERE callCode='$callCode' AND unitCode='$unitCode'";
  $mysqli->query($query) or die(mysqli_error());
  //insert new sup parts		 
  for($ind=0;$ind<count($descArr);$ind++){
    if($descArr[$ind]!=""){
	  $result=$mysqli->query("SELECT count(*) as total from tblParts WHERE code='$codeArr[$ind]'") or die(mysqli_error());
	  $data=$result->fetch_assoc();
	  if($data['total']>0){
	    $query = "INSERT INTO tblSuppParts (callCode,unitCode,code,partDesc,quantity,ind) SELECT '$callCode','$unitCode','$codeArr[$ind]','$descArr[$ind]','$quantityArr[$ind]','$ind'";
		$mysqli->query($query) or die(mysqli_error());
	  }
	} 
  }
}	


/////////////////////////////////////////////////////////////
function LogPostVars(){

  $qryLog="";
  foreach($_POST as $key => $value){
    $qryLog.=$key.'='.$value.'&';
  }	
  if(strpos($qryLog,'updCallRep')!==false){
  
    $qryLog = str_replace("'","''", $qryLog);
    $query = "INSERT INTO tblLog (qryLog) VALUES (LEFT('$qryLog',300))";
    $mysqli->query($query) or die(mysqli_error());
  }
}	


 if(isset($_REQUEST["execRepFileName"])){
   $execRepFileName = $_REQUEST["execRepFileName"];
   $callCode = $_REQUEST["callCode"];
   $uploadFile ='Images/' . $execRepFileName;
   if (move_uploaded_file($_FILES['file']['tmp_name'], $uploadFile)){
     $query = "UPDATE `tblRep` SET execRepFileName='$execRepFileName' WHERE callCode='$callCode'";
	 $mysqli->query($query) or die(mysqli_error());	
     die('EOK');
   }   
   else 
     die('ERROR');
 } 


 //LogPostVars();
 $op = $_POST['op'];

switch ($op) {
    case "test":
	    $val = $_POST['val'];
		$query = "INSERT INTO `tblTest`(val) VALUES ('$val')";
		$mysqli->query($query) or die(mysqli_error());
		echo "EOK";
		break;
	case "login":
		$userName = $_POST['userName'];
		$pass = $_POST['pass'];
		$query = $mysqli->query("SELECT perm FROM tblUsers WHERE userName='$userName' AND pass='$pass'") or die(mysqli_error());
		echo RecordSetToJson($query);
		break;
    case "getVersion":
	    $query = $mysqli->query("SELECT version FROM tblVersion") or die(mysqli_error());
		echo RecordSetToJson($query);
	    break;
	case "getAllCalls":
        $qry="";
		if(isset($_POST['includeHistory']))
		  $qry="SELECT `code`, `charsType`, `callType`, `callDateTime`, `isContinue`,`inService`, `custName`, `siteName`, `projName`, `address`, `id`, `location`, `recieverServiceName`, `callDesc`, `callStatus`,userCode FROM `tblCalls` WHERE isTechSmartInsCallCompleted=1"; 
		else //do not get finished or cancelled calls	
		  $qry="SELECT `code`, `charsType`, `callType`, `callDateTime`, `isContinue`,`inService`, `custName`, `siteName`, `projName`, `address`, `id`, `location`, `recieverServiceName`, `callDesc`, `callStatus`,userCode FROM `tblCalls` WHERE callStatus NOT IN (6,9) AND isTechSmartInsCallCompleted=1"; 
		
		$histByUser = $_POST['histByUser'];
		
		if(isset($_POST['userCode'])){
		  $userCode = $_POST['userCode'];
		  if(isset($_POST['includeHistory']))
		    $qry.= " AND userCode='$userCode'";
		  else
			$qry.=" AND userCode='$userCode'";
		}
	     
		 if(isset($_POST['siteName'])){
		   $siteName = $_POST['siteName'];
		   $qry.= " AND siteName='$siteName'  AND (userCode='$histByUser' OR '$histByUser' = '-1')";
		 }
		 
		 if(isset($_POST['callDateTime'])){
		   $callDateTime = $_POST['callDateTime'];
		   $qry.= " AND callDateTime BETWEEN '$callDateTime' AND UNIX_TIMESTAMP(DATE_ADD(FROM_UNIXTIME('$callDateTime'), INTERVAL 24 HOUR))  AND (userCode='$histByUser' OR '$histByUser' = '-1')";
		 }
				 
		 if(isset($_POST['callCode'])){
		   $callCode = $_POST['callCode'];
		   $qry.= " AND code='$callCode'";
		 }
		 		 
		 if(isset($_POST['all'])){
		   $qry.= "  AND (userCode='$histByUser' OR '$histByUser' = '-1')";
		 }
    	
		 $qry.=" ORDER BY callDateTime DESC";

	
        $result = $mysqli->query($qry) or die(mysqli_error());
	
		echo RecordSetToJson($result);
    	break;
    case "getCallByCode":
        $code = $_POST['code'];
		$query=$mysqli->query("SELECT `code`, `charsType`, `callType`, `callDateTime`, `isContinue`,`inService`, `custName`, `siteName`, `projName`, `address`, `id`, `location`, `recieverServiceName`, `callDesc`, `callStatus` FROM `tblCalls`WHERE code='$code'") or die(mysqli_error()); 
		echo RecordSetToJson($query);
    	break;
    
	case "getCallsCount":
		$qry="";
		if(isset($_POST['includeHistory']))
		  $qry = "SELECT count(*) as total from tblCalls";
		else
		  $qry = "SELECT count(*) as total from tblCalls  WHERE callStatus NOT IN (6,9)";
				
		if(isset($_POST['userCode'])){
		  $userCode = $_POST['userCode'];
		  if(isset($_POST['includeHistory']))
		    $qry.= " WHERE userCode='$userCode'";
		  else
		    $qry.= " AND userCode='$userCode'";
		}
			
         //echo $qry;			
	   	$result=$mysqli->query($qry) or die(mysqli_error());
		$data=$result->fetch_assoc();
		echo $data['total'];
	    break;

   case "getAllSites":
     $qry = "SELECT DISTINCT custName,siteName from tblCalls";
     $result = $mysqli->query($qry) or die(mysqli_error());
	 echo RecordSetToJson($result);
	 break;
	
	case "getUnitsByCallCode":
      $callCode = $_POST['callCode'];
	  $query=$mysqli->query("SELECT unitCode,unitDesc,building,floor,room,manufacturer,model,outputTon,remarks FROM tblUnits WHERE callCode =  '$callCode'") or die(mysqli_error());
	  echo RecordSetToJson($query);
      break;
   	case "getUnitByCallCodeUnitCode":
      $callCode = $_POST['callCode'];
	  $unitCode = $_POST['unitCode'];
	  $query=$mysqli->query("SELECT unitCode,unitDesc,building,floor,room,manufacturer,model,outputTon,remarks FROM tblUnits WHERE callCode = '$callCode' AND unitCode = '$unitCode'") or die(mysqli_error());
	  echo RecordSetToJson($query);
      break;
	
	case "insCall":
		 $code = $_POST['code'];
		 $charsType=$_POST['charsType'];
		 $callType = $_POST['callType'];
		 $callDateTime=$_POST['callDateTime'];
	     $isContinue = $_POST['isContinue'];
		 $inService = $_POST['inService'];
		 $custName = str_replace("'","''", $_POST['custName']); 
		 $siteName = str_replace("'","''", $_POST['siteName']); 
		 $projName = str_replace("'","''", $_POST['projName']); 
		 $address =  str_replace("'","''", $_POST['address']);
		 $id=  $_POST['id']; 
		 $location = $_POST['location'];
		 $recieverServiceName = $_POST['recieverServiceName'];
		 $callDesc = str_replace("'","''", $_POST['callDesc']);
		 $callStatus = $_POST['callStatus'];
		 $userCode = $_POST['userCode'];
		
		$result=$mysqli->query("SELECT count(*) as total from tblCalls WHERE code='$code'") or die(mysqli_error());
		$data=$result->fetch_assoc();
		if($data['total']==0){
		  //ins call to tblCall
		  $query = "INSERT INTO `tblCalls`(code, charsType, callType, callDateTime, isContinue,inService, custName, siteName, projName, address, id, location, recieverServiceName, callDesc, callStatus,userCode) VALUES ('$code', '$charsType', '$callType', '$callDateTime', '$isContinue','$inService' ,'$custName', '$siteName', '$projName', '$address', uuid(), '$location', '$recieverServiceName', '$callDesc', '$callStatus','$userCode')";
		  $mysqli->query($query) or die(mysqli_error());
		  //ins call to tblRep
		  $query = "INSERT INTO tblRep (callCode,numExecRep,callID) SELECT '$code','1' ,(select id from tblCalls where code='$code')";
		  $mysqli->query($query) or die(mysqli_error());
		}  
		else{ //update call		
		  $query = "UPDATE tblCalls set  charsType='$charsType', callType='$callType', callDateTime='$callDateTime', isContinue='$isContinue',inService='$inService', custName='$custName', siteName='$siteName', projName='$projName', address='$address', id=uuid(), location='$location', recieverServiceName='$recieverServiceName', callDesc='$callDesc', callStatus = (CASE WHEN callStatus <> 6 THEN '$callStatus' ELSE 6 END), userCode='$userCode'  WHERE code='$code'";
		  $mysqli->query($query) or die(mysqli_error());	
		  if(((int)$callStatus)<6){ //work not finihed 
		    $query = "UPDATE tblRep SET dtEndWork=0,isEnded=0 WHERE callCode='$code'"; 
		    $mysqli->query($query) or die(mysqli_error());
		  }   
		  $query = "UPDATE tblRep SET callID=(select id from tblCalls where code='$code') WHERE callCode='$code'";
		  $mysqli->query($query) or die(mysqli_error());
		}	
		echo "EOK";
		break;
	case "delCall":
	  	 $code = $_POST['code'];
		 $query = "DELETE FROM `tblCalls` WHERE code='$code'";
		 $mysqli->query($query) or die(mysqli_error());
		 $query = "DELETE FROM `tblRep` WHERE callCode NOT IN (SELECT code FROM tblCalls)";
		 $mysqli->query($query) or die(mysqli_error());
		 $query = "DELETE FROM tblSuppParts WHERE callCode NOT IN (SELECT code FROM tblCalls)";
		 $mysqli->query($query) or die(mysqli_error());
		 $query = "DELETE FROM tblInvParts WHERE callCode NOT IN (SELECT code FROM tblCalls)";
		 $mysqli->query($query) or die(mysqli_error());
		 $query = "DELETE FROM tblunits WHERE callCode not in (select code from tblCalls)";
		 $mysqli->query($query) or die(mysqli_error());
		 $query = "DELETE FROM tblunithandling WHERE callCode not in (select code from tblCalls)";
		 $mysqli->query($query) or die(mysqli_error());
		 echo "EOK";
		 break;
	case "updCallStatus":
	  $callCode = $_POST['code'];  
      $callStatus = $_POST['callStatus'];  
      $mysqli->query("UPDATE tblCalls SET callStatus='$callStatus' WHERE code='$callCode';")  or die(mysqli_error());
	break;
	case "updCallRep":
	  $jsn = json_decode($_POST['qry']);
      if(is_null($jsn))
	    die("could not parse json");
	  $callCode = $jsn[0]->code;  
      $callStatus = $jsn[0]->callStatus;  
      $mysqli->query("START TRANSACTION;");
      $result = $mysqli->query("UPDATE tblCalls SET callStatus='$callStatus' WHERE code='$callCode';");
	  if(!$result){
        echo mysqli_error();
	    $mysqli->query("ROLLBACK;");
	    die();  
      }
	  for($i=0;$i<count($jsn);$i++){
        if(is_array(json_decode($jsn[$i]->repData))){
	     $repDataJson = json_decode($jsn[$i]->repData);  
	     for($j=0;$j<count($repDataJson);$j++){  
	       $unitCode = $repDataJson[$j]->unitCode;
		   $isEnded = $repDataJson[$j]->isEnded;
		   $dtCallConfirm = $repDataJson[$j]->dtCallConfirm;
		   $dtStartRide = $repDataJson[$j]->dtStartRide;
		   $dtStartWork = $repDataJson[$j]->dtStartWork;
		   $dtEndWork = $repDataJson[$j]->dtEndWork;
		   $summary = str_replace("'","''",$repDataJson[$j]->summary);
		   $recommendation =  str_replace("'","''",$repDataJson[$j]->recommendation); 
		   $signerName = $repDataJson[$j]->signerName;
		   $signerRole = $repDataJson[$j]->signerRole;
		   $signerMail = $repDataJson[$j]->signerMail;
		   $signPicFileName = $repDataJson[$j]->signPicFileName;  
		   $signPicFileName = str_replace(' ', '+', $signPicFileName);
  		   $signComments = str_replace("'","''",$repDataJson[$j]->signComments);
		   $numImages = $repDataJson[$j]->numImages;  
		   $execRepFileName = $repDataJson[$j]->execRepFileName;
		   $reqInd = $repDataJson[$j]->reqInd;
		   $reqRemarks = str_replace("'","''",$repDataJson[$j]->reqRemarks); 
		   if($unitCode!=""){
		     $qry = "INSERT INTO tblRep (callCode,unitCode,summary,recommendation) VALUES('$callCode','$unitCode','$summary','$recommendation') ON DUPLICATE KEY UPDATE isEnded = '$isEnded',dtCallConfirm = '$dtCallConfirm' ,dtStartRide = '$dtStartRide',dtStartWork='$dtStartWork',dtEndWork='$dtEndWork',summary='$summary',recommendation='$recommendation',signerName='$signerName',signerRole='$signerRole',signerMail='$signerMail',signPicFileName='$signPicFileName',signComments='$signComments',execRepFileName='$execRepFileName',  reqInd= '$reqInd',  reqRemarks='$reqRemarks';";
	         $result = $mysqli->query($qry); 
		     if(!$result){
		       echo $qry;  //mysqli_error();
			   $mysqli->query("ROLLBACK;");
		       die();  
   	 	     }	 
		  }
		 //print_r($jsn[$i]->repData[$j]);
	     }
	    }
      if(is_array(json_decode($jsn[$i]->supPartsData))){ 
         $result = $mysqli->query("DELETE FROM tblSuppParts where callCode=".$callCode); 
	     if(!$result){
	       echo mysqli_error();
		   $mysqli->query("ROLLBACK;");
		   die();  
   	     }
	     $supPartsDataJson = json_decode($jsn[$i]->supPartsData);  
	     for($j=0;$j<count($supPartsDataJson);$j++){
	 	   $unitCode = $supPartsDataJson[$j]->unitCode;
	 	   if($unitCode!=""){
		     $desc = str_replace("'","''",$supPartsDataJson[$j]->desc_);
	         $quantity = $supPartsDataJson[$j]->quantity;
	         $barcode = $supPartsDataJson[$j]->barcode; 
	         $code = $supPartsDataJson[$j]->code; 
	     
		     $qry = "INSERT INTO tblSuppParts (callCode,unitCode,code,partDesc,quantity,ind) SELECT '$callCode','$unitCode','$code','$desc','$quantity','$j'";
             $result = $mysqli->query($qry); 
	         if(!$result){
       		   echo mysqli_error();
			   $mysqli->query("ROLLBACK;");
		       die();  
   	         }    
	       }
	     } 
	   }
	 
	   if(is_array(json_decode($jsn[$i]->unitHandlingData))){
	     $unitHandlingDataJson = json_decode($jsn[$i]->unitHandlingData);  
	     for($j=0;$j<count($unitHandlingDataJson);$j++){
	 	   $unitCode = $unitHandlingDataJson[$j]->unitCode;
	 	   $handlingCode = $unitHandlingDataJson[$j]->handlingCode;
		   $isDone = $unitHandlingDataJson[$j]->isDone;
		   if($unitCode!=""){
		     $qry = "UPDATE tblUnitHandling SET isDone=".$isDone." WHERE callCode='$callCode' AND unitCode='$unitCode' AND handlingCode='$handlingCode'";
             $result = $mysqli->query($qry); 
	         if(!$result){
       		   echo mysqli_error();
			   $mysqli->query("ROLLBACK;");
		       die();
			  }
			}
		  }
	   }
	 
	   //$unitsDataJson[$j]->isInsertedByUser == "1"
	   if(is_array(json_decode($jsn[$i]->unitsData))){
	     $unitsDataJson = json_decode($jsn[$i]->unitsData);  
	     for($j=0;$j<count($unitsDataJson);$j++){
	 	      $unitCode = $unitsDataJson[$j]->unitCode;
			  $unitDesc = str_replace("'","''",$unitsDataJson[$j]->unitDesc);
			  $unitType = str_replace("'","''",$unitsDataJson[$j]->unitType);
			  $building = str_replace("'","''",$unitsDataJson[$j]->building);
			  $floor = str_replace("'","''",$unitsDataJson[$j]->floor);
			  $room = str_replace("'","''",$unitsDataJson[$j]->room);
			  $manufacturer = str_replace("'","''",$unitsDataJson[$j]->manufacturer);
			  $model = str_replace("'","''",$unitsDataJson[$j]->model);
			  $outputTon = $unitsDataJson[$j]->outputTon;
			  if(!is_numeric($outputTon))
			    $outputTon = 0;
			  $remarks = str_replace("'","''",$unitsDataJson[$j]->remarks);
			  $creationYear = $unitsDataJson[$j]->creationYear;
			  $serialNum = str_replace("'","''",$unitsDataJson[$j]->serialNum);
			  if($unitCode!=""){
			    $qry = "INSERT INTO `tblUnits` (callCode,unitCode,unitDesc,unitType,building,floor,room,manufacturer,model,outputTon,remarks,creationYear,serialNum) VALUES('$callCode','$unitCode','$unitDesc','$unitType','$building','$floor','$room','$manufacturer','$model','$outputTon','$remarks','$creationYear','$serialNum') ON DUPLICATE KEY UPDATE unitDesc='$unitDesc',unitType='$unitType',building='$building',floor='$floor',room='$room',manufacturer='$manufacturer',model='$model',outputTon='$outputTon',remarks='$remarks',creationYear='$creationYear',serialNum='$serialNum'";	
   	            $result = $mysqli->query($qry); 
	            if(!$result){
       		      echo mysqli_error();
			      $mysqli->query("ROLLBACK;");
		          die();
			   }
			 }
		 }
	   }
	 
	}    
    $mysqli->query("COMMIT;");
	break;
	
	case "uploadImage":
	  $callCode = $_POST['callCode'];
	  $unitCode = $_POST['unitCode'];
	  $imgData = $_POST['imgData'];
	  if(!SaveImage($mysqli,$callCode,$unitCode,$imgData)===false){
		  $query = "UPDATE `tblRep` SET numImages = numImages+1 WHERE callCode='$callCode' AND unitCode='$unitCode'";
		  $mysqli->query($query) or die(mysqli_error());
	  }
	  break;
	
	case "getCallRep":
		 
		 $callCode = $_POST['callCode'];
		 $query=$mysqli->query("SELECT `code`, `charsType`, `callType`, `callDateTime`, `isContinue`,`inService`, `custName`, `siteName`, `projName`, `address`, `id`, `location`, `recieverServiceName`, `callDesc`, `callStatus` FROM `tblCalls`WHERE code='$callCode'") or die(mysqli_error()); 
		 $json = json_decode(RecordSetToJson($query));
		 $query=$mysqli->query("SELECT `callCode`, `isEnded`, numExecRep, (SELECT callStatus FROM tblCalls WHERE code='$callCode') AS status ,`dtCallConfirm`, `dtStartRide`, `dtStartWork`, `dtEndWork`, `summary`, `recommendation`, `signerName`,`signerRole`, signerMail, CONVERT(signPicFileName USING UTF8) AS signPicFileName ,signComments, `audioFilesName`,numImages,execRepFileName,reqInd,reqRemarks, callID,unitCode FROM `tblRep` WHERE callCode='$callCode' ORDER BY unitCode") or die(mysqli_error());  
		 $json[] = ['repData' => RecordSetToJson($query)];
	     $query=$mysqli->query("SELECT unitCode,unitDesc,building,floor,room,manufacturer,model,outputTon,remarks,creationYear,serialNum,unitType FROM tblUnits WHERE callCode =  '$callCode'") or die(mysqli_error());
	     $json[] = ['unitsData' => RecordSetToJson($query)];
	     $query=$mysqli->query("SELECT  tblSuppParts.partDesc AS desc_, tblSuppParts.quantity ,tblSuppParts.unitCode ,tblParts.unit,tblParts.code,tblParts.barcode  FROM tblParts,tblSuppParts WHERE tblParts.code=tblSuppParts.code AND tblSuppParts.callCode='$callCode' ORDER BY tblSuppParts.ind") or die(mysqli_error());
		 $json[] = ['supPartsData' => RecordSetToJson($query)];
	     $query=$mysqli->query("SELECT unitCode,handlingCode,handlingDesc,remarks,isDone FROM tblUnitHandling WHERE callCode =  '$callCode'") or die(mysqli_error());
	     $json[] = ['unitHandlingData' => RecordSetToJson($query)];
	     echo json_encode($json);
		 break;
	case "getCallRepVideo":
		 $callCode = $_POST['callCode'];
	 	 $unitCode = 0;
		 if(isset($_POST['unitCode']))
		   $unitCode = $_POST['unitCode'];
		 $query=$mysqli->query("SELECT `callCode`, numImages FROM `tblRep` WHERE callCode='$callCode' AND unitCode='$unitCode'") or die(mysqli_error());  
		 echo RecordSetToJson($query);
		 break;
	case "updSupParts":
  	 	 $unitCode = 0;
		 if(isset($_POST['unitCode']))
		   $unitCode = $_POST['unitCode'];
		  UpdSupParts($mysqli,$_POST['callCode'],$unitCode,explode("|||",$_POST['desc']),explode("|||",$_POST['quantity']),explode("|||",$_POST['code']),explode("|||",$_POST['barcode']),$_POST['source']);
		  echo "EOK";
		 break;
	case "getSupParts":
      $callCode = $_POST['callCode'];
  	  $unitCode = 0;
	  if(isset($_POST['unitCode']))
	    $unitCode = $_POST['unitCode'];
	  $query=$mysqli->query("SELECT  tblSuppParts.partDesc AS desc_, tblSuppParts.quantity  ,tblParts.unit,tblParts.code,tblParts.barcode  FROM tblParts,tblSuppParts WHERE tblParts.code=tblSuppParts.code AND tblSuppParts.callCode='$callCode'  AND tblSuppParts.unitCode='$unitCode' ORDER BY tblSuppParts.ind") or die(mysqli_error());
	  echo RecordSetToJson($query);
      break;
    case "insInvPart":
		 $callCode = $_POST['callCode'];
		 $unitCode = 0;
		 if(isset($_POST['unitCode']))
	       $unitCode = $_POST['unitCode'];
		 $code=$_POST['code'];
		 $partDesc = str_replace("'","''",$_POST['partDesc']); 
		 $partUnit = $_POST['partUnit'];
		 $partQuantity = $_POST['partQuantity'];
		 
		 //insert new inv part		 
		 $query = "INSERT IGNORE INTO tblParts (code,desc_,unit) VALUES('$code','$partDesc','$partUnit')";
		 $mysqli->query($query) or die(mysqli_error());
		 $query = "INSERT INTO tblInvParts (callCode,unitCode,code,quantity) SELECT '$callCode','$unitCode','$code','$partQuantity'";
		 $mysqli->query($query) or die(mysqli_error());
		 $query = "INSERT INTO tblSuppParts (callCode,unitCode,code,quantity,partDesc,ind) SELECT '$callCode','$unitCode','$code','0' ,'$partDesc' , (select count(callCode) from tblSuppParts where callCode='$callCode' AND unitCode='$unitCode')";
		 $mysqli->query($query) or die(mysqli_error());
		 
		 echo "EOK";
		 break;
	case "delInvParts":
		$callCode = $_POST['callCode'];
		$unitCode = 0;
		 if(isset($_POST['unitCode']))
	       $unitCode = $_POST['unitCode'];
		$query = "DELETE FROM tblInvParts WHERE callCode='$callCode' AND unitCode='$unitCode'";
		$mysqli->query($query) or die(mysqli_error());
		$query = "DELETE FROM tblSuppParts WHERE callCode='$callCode' AND unitCode='$unitCode'";
		 $mysqli->query($query) or die(mysqli_error());
		echo "EOK";
		break;
	case "getInvParts":
        $callCode = $_POST['callCode'];
		$unitCode = 0;
		 if(isset($_POST['unitCode']))
	       $unitCode = $_POST['unitCode'];
		$query=$mysqli->query("SELECT  tblParts.desc_, tblInvParts.quantity,tblParts.unit  FROM tblParts,tblInvParts WHERE tblParts.code=tblInvParts.code AND tblInvParts.callCode='$callCode' AND tblInvParts.unitCode='$unitCode'") or die(mysqli_error());
		//$query=$mysqli->query("SELECT code,partDesc,quantity,unit FROM tblInvParts WHERE callCode='$callCode'") or die(mysqli_error()); 
		echo RecordSetToJson($query);
    	break;
    case "getAllParts":
		$query=$mysqli->query("SELECT tblParts.code,tblParts.desc_,tblParts.unit,tblPartBarcode.barcode,tblParts.source,tblParts.status FROM tblParts , tblPartBarcode WHERE tblParts.code = tblPartBarcode.code AND tblParts.status=1 ORDER BY tblParts.desc_") or die(mysqli_error()); 
		echo RecordSetToJson($query);
      break;
    case "getAllNewParts":
		$query=$mysqli->query("SELECT tblParts.code,tblParts.desc_,tblParts.unit,tblPartBarcode.barcode,tblParts.source,tblParts.status FROM tblParts LEFT JOIN tblPartBarcode ON tblParts.code = tblPartBarcode.code WHERE tblParts.isNew=1 ORDER BY tblParts.desc_") or die(mysqli_error()); 
		echo RecordSetToJson($query);
    	break;
    case "insPart":
	    $code =  $_POST['code']; 
		$desc =  str_replace("'","''",$_POST['desc']); 
		$unit =  $_POST['unit']; 
		$barcode = $_POST['barcode']; 
		$source =  $_POST['source'];
		$status =  $_POST['status'];
		
		$query = "DELETE FROM tblParts WHERE code='$code'";
	    $mysqli->query($query) or die(mysqli_error());
		$query = "INSERT IGNORE INTO tblParts (code,desc_,unit,barcode,source,status) VALUES('$code','$desc','$unit','$barcode','$source','$status')";
	    $mysqli->query($query) or die(mysqli_error());
		if($barcode!="0"){
		  $query = "INSERT IGNORE INTO tblPartBarcode (code,barcode) VALUES('$code','$barcode')";
	      $mysqli->query($query) or die(mysqli_error());
		}
		break;
	case "updPart":
	    $code =  $_POST['code']; 
		$desc =  str_replace("'","''",$_POST['desc']); 
		$unit =  $_POST['unit']; 
		$barcode = $_POST['barcode']; 
		$source =  $_POST['source'];
		$status =  $_POST['status'];
		
		$query = "UPDATE tblParts SET desc_='$desc',unit='$unit',barcode='$barcode',status='$status',isNew=1 WHERE code='$code'";
	    $mysqli->query($query) or die(mysqli_error());
		if($barcode!="0"){
		  $query = "INSERT IGNORE INTO tblPartBarcode (code,barcode) VALUES('$code','$barcode')";
	      $mysqli->query($query) or die(mysqli_error());
		}
		break;
	case "delPart":
	  $code =  $_POST['code']; 
	  $query = "UPDATE tblParts SET status=2,isNew=1 WHERE code='$code'";
	  $mysqli->query($query) or die(mysqli_error());	
	  break;
	case "makePartsDirty":
	  $query = "UPDATE tblParts SET isNew=0";
	  $mysqli->query($query) or die(mysqli_error());
	break;
	case "getImage":
	   $callCode = $_POST['callCode'];
	   $unitCode = 0;
	   if(isset($_POST['unitCode']))
	     $unitCode = $_POST['unitCode'];
	   $ind = $_POST['ind'];
	   echo  '{"imgData" :'.'"'.GetImage($callCode,$unitCode,$ind).'"}';  
	break;
	case "insCallUnit":
	  $unitCode = $_POST['unitCode'];
      $callCode = $_POST['callCode'];
      $unitDesc = str_replace("'","''",$_POST['unitDesc']);
      $building = str_replace("'","''",$_POST['building']);
      $floor = str_replace("'","''",$_POST['floor']);
      $room = str_replace("'","''",$_POST['room']);
      $manufacturer = str_replace("'","''",$_POST['manufacturer']);
      $model = str_replace("'","''",$_POST['model']);
      $outputTon = str_replace("'","''",$_POST['outputTon']);
      $remarks = str_replace("'","''",$_POST['remarks']);
	  $creationYear = $_POST['creationYear'];
	  $serialNum = $_POST['serialNum'];
	  $unitType = $_POST['unitType'];
	  $query = "INSERT INTO `tblUnits` (`unitCode`,callCode,unitDesc,building,floor,room,manufacturer,model,outputTon,remarks,creationYear,serialNum,unitType) VALUES( '$unitCode','$callCode','$unitDesc','$building','$floor','$room','$manufacturer','$model','$outputTon','$remarks','$creationYear','$serialNum','$unitType') ON DUPLICATE KEY UPDATE unitDesc='$unitDesc',building='$building',floor='$floor',room='$room',manufacturer='$manufacturer',model='$model',outputTon='$outputTon',remarks='$remarks',creationYear='$creationYear',serialNum='$serialNum',unitType='$unitType'";
  	  $mysqli->query($query) or die(mysqli_error());	
	  //ins unit to tblRep
	  $query = "INSERT INTO tblRep (callCode,unitCode,numExecRep,callID) VALUES ('$callCode','$unitCode','-1' ,'-1') ON DUPLICATE KEY UPDATE numExecRep='-1'";
	  $mysqli->query($query) or die(mysqli_error());   
	break;
	case "insCallUnitHandling":
      $unitCode = $_POST['unitCode'];
      $callCode = $_POST['callCode'];
      $handlingCode = $_POST['handlingCode'];
      $handlingDesc = $_POST['handlingDesc'];
      $isDone = $_POST['isDone'];
      $remarks = $_POST['remarks'];
	  $query = "INSERT INTO `tblUnitHandling` (callCode,`unitCode`,handlingCode,handlingDesc,isDone,remarks) VALUES('$callCode','$unitCode','$handlingCode','$handlingDesc','$isDone','$remarks') ON DUPLICATE KEY UPDATE handlingDesc='$handlingDesc',isDone='$isDone',remarks='$remarks'";	
   	  $mysqli->query($query) or die(mysqli_error());	
	break;
	case "delUnitTypeTable":
	  $query = "DELETE FROM tblunittype";
	  $mysqli->query($query) or die(mysqli_error());
	break;
	case "insUnitType":
	  $code = $_POST['code'];
      $txt = $_POST['txt'];
      $query = "INSERT INTO tblunittype (code,txt) VALUES ('$code','$txt')";
	  $mysqli->query($query) or die(mysqli_error());
	break;
	case "getUnitTypes":
	  $query=$mysqli->query("SELECT code,txt FROM tblunittype") or die(mysqli_error());
	  echo RecordSetToJson($query);
	break;
	case "updCallCompleted":
	  $code = $_POST['code'];
      $query = "UPDATE tblCalls SET isTechSmartInsCallCompleted=1 WHERE code='$code'";
	  $mysqli->query($query) or die(mysqli_error());
	  echo "EOK"; 
	break;
	default:
        //echo RecordSetToJson($query);
		echo "Your favorite color is neither red, blue, nor green! op=".$op;
}

$mysqli->close();

?>