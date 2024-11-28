<?php
header("Access-Control-Allow-Origin: *");
$mysqli = new mysqli('localhost','drivers', 'a1b2c3d5e6f7','dbdrivers'); 
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
function SaveImage($mysqli,$invNum,$pg,$imgInd,$imgData){
  
  try{
     $qry2=$mysqli->query("INSERT INTO tbltraceimg (invNum,pg,img) VALUES ('$invNum','$pg','$imgData')");
  }
  catch (Exception $e) {
  }
  
  $img = $imgData;
  $img = str_replace('data:image/jpeg;base64,', '', $img);
  $img = str_replace(' ', '+', $img);
  $data_img = base64_decode($img);
  
  $result = $mysqli->query("SELECT numImages_". $pg ." AS numImages FROM tblInvs WHERE invNum='$invNum'") or die(mysqli_error());
  $data = $result->fetch_assoc();
  $numImages = $data['numImages']; 
  	
  $file = 'Images_Pinuyeem/'.$invNum.'_'.$pg.'_'.$imgInd.'.jpg';  
  
  $success = file_put_contents($file, $data_img);
  return $success;
}

/////////////////////////////////////////////////////////////
function CountNumInvImages($prefix) {

	$folderPath = "Images_Pinuyeem";
    // Get all files in the folder
    $files = scandir($folderPath);

    // Filter files that start with the specified prefix
    $filteredFiles = array_filter($files, function($file) use ($folderPath, $prefix) {
        return is_file($folderPath . DIRECTORY_SEPARATOR . $file) && strpos($file, $prefix) === 0;
    });

    // Return the count
    return count($filteredFiles);
}

/////////////////////////////////////////////////////////////
function GetImage($invNum,$pg,$imgInd){

  $file = 'Images_Pinuyeem/'.$invNum.'_'.$pg.'_'.$imgInd.'.jpg';
  $data = file_get_contents($file);
  $img = base64_encode($data); 
  //$img = str_replace('+', ' ', $img);
  return $img;
}	

/////////////////////////////////////////////////////////////
function LogPostVars($mysqli){

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


 //LogPostVars($mysqli);
 $op = $_POST['op'];

switch ($op) {
	 case "main":
	   $driver = $_POST["driver"];
   	   $qry = "SELECT id,dt,driverName,carNum from tblMain WHERE driver='$driver'";
       $result = $mysqli->query($qry) or die(mysqli_error());
	   echo RecordSetToJson($result);
	 break;
	case "invs":
	   $userCode = $_POST["userCode"];
   	   $dt = $_POST["dt"];
   	   $qry = "SELECT invNum,siteName,address,custName,contactName,contactPhone,contactMobile,numImages_erSnap,numImages_wgSnap,invStatus,invGuid FROM tblInvs WHERE userCode='$userCode' AND FROM_UNIXTIME(dt, '%d/%m/%Y')='$dt'";
       $result = $mysqli->query($qry) or die(mysqli_error());
	   echo RecordSetToJson($result);
	break;
	case "invByNum":
	   $invNum = $_POST["invNum"];
   	   $qry = "SELECT invNum,dt,siteName,address,custName,dischargeArea,contactName,contactPhone,contactMobile,quantity,remarks,signerName,signerRole,signEmail,signRemarks,CONVERT(signPic USING UTF8) AS signPic,weigeCertNum,weigeRemarks,weight,numImages,numImages_erPic,numImages_erSnap,numImages_wgPic,numImages_wgSnap,numExecRep,invStatus,invType,invDesc,invGuid FROM tblInvs where invNum='$invNum'";
       $result = $mysqli->query($qry) or die(mysqli_error());
	   echo RecordSetToJson($result);
	break;
	case "updInvRemark":
	  $invNum = $_POST["invNum"];
   	  $remarks = str_replace("'","''",$_POST["remarks"]);
	  $quantity = $_POST["quantity"];
	  $qry = "UPDATE tblInvs SET remarks='$remarks',quantity='$quantity' WHERE invNum='$invNum'"; 
	  $mysqli->query($qry) or die(mysqli_error());
	break;
	case "updInvSign":
	  $invNum = $_POST["invNum"];
   	  $signerName = str_replace("'","''",$_POST["signerName"]);
	  $signerRole = $_POST["signerRole"];
	  $signEmail = $_POST["signEmail"];
	  $signRemarks = str_replace("'","''",$_POST["signRemarks"]);
	  $signPic = $_POST["signPic"];
	  $signPic = str_replace(' ', '+', $signPic);
 	  $qry = "UPDATE tblInvs SET signerName = '$signerName' , signerRole = '$signerRole' , signEmail = '$signEmail',signRemarks='$signRemarks',signPic='$signPic' WHERE invNum='$invNum'"; 
   	 $mysqli->query($qry) or die(mysqli_error());
	break;
	case "updInvWeige":
	  $invNum = $_POST["invNum"];
   	  $dischargeArea = str_replace("'","''",$_POST["dischargeArea"]);
	  $weigeCertNum = $_POST["weigeCertNum"];
	  $weigeRemarks = str_replace("'","''",$_POST["weigeRemarks"]);
	  $weight = $_POST["weight"];
	  $qry = "UPDATE tblInvs SET dischargeArea='$dischargeArea' , weigeCertNum = '$weigeCertNum', weigeRemarks='$weigeRemarks',weight='$weight' WHERE invNum='$invNum'"; 
	 $mysqli->query($qry) or die(mysqli_error());
	break;
	case "updInvStatus":
	  $invNum = $_POST["invNum"];
   	  $invStatus = $_POST["invStatus"];
   	  $qry = "UPDATE tblInvs SET invStatus='$invStatus' WHERE invNum='$invNum'"; 
	  $mysqli->query($qry) or die(mysqli_error());
	break;
	case "insInvitation":
	  $invNum = $_POST["invNum"];
	  $userCode = $_POST["userCode"];
	  $dt = $_POST["dt"];
	  $custName = str_replace("'","''",$_POST["custName"]);
	  $siteName = str_replace("'","''",$_POST["siteName"]);
	  $address = str_replace("'","''",$_POST["address"]);
	  $contactName = str_replace("'","''",$_POST["contactName"]);
	  $contactPhone = $_POST["contactPhone"];
	  $contactMobile = $_POST["contactMobile"];
	  $message = str_replace("'","''",$_POST["message"]);
	  $quantity = $_POST["quantity"];
	  $dischargeArea = str_replace("'","''",$_POST["dischargeArea"]);
	  $carNum = $_POST["carNum"];
	  $driverName = str_replace("'","''",$_POST["driverName"]);
	  $numExecRep = $_POST["numExecRep"];
	  $invType = $_POST["invType"];
	  if(strlen(trim($invType)) == 0){
	    $invType = 0;  
	  }
	  $invDesc = str_replace("'","''",$_POST["invDesc"]);
	  
	  $qry = "INSERT INTO `tblinvs`(`invNum`, `userCode`, `dt`, `siteName`, `custName`, `address`, `dischargeArea`, `contactName`, `contactPhone`, `contactMobile`, `quantity`,numExecRep,invType,invDesc,invGuid) VALUES ('$invNum','$userCode','$dt','$siteName','$custName','$address','$dischargeArea','$contactName', '$contactPhone', '$contactMobile', '$quantity','$numExecRep','$invType','$invDesc',(select uuid())) ON DUPLICATE KEY UPDATE userCode='$userCode', dt='$dt', siteName='$siteName' , custName='$custName' , address='$address', dischargeArea='$dischargeArea', contactName='$contactName' , contactPhone='$contactPhone', contactMobile='$contactMobile', quantity='$quantity',invType='$invType',invDesc='$invDesc',invGuid=(select uuid())";
	  $mysqli->query($qry) or die(mysqli_error()); 
	  if(strlen(trim($carNum)) > 0){
	    $qry = "UPDATE tblMain SET carNum='$carNum',driverName='$driverName' WHERE driver='$userCode'";
	    $mysqli->query($qry) or die(mysqli_error()); 
	  }
	break;
	
	case "updInv":
	$jsn = json_decode($_POST['qry']);
      if(is_null($jsn))
	    die("could not parse json");
	  $invNum = $jsn[0]->invNum;  
      $siteName = str_replace("'","''",$jsn[0]->siteName);
	  $address = str_replace("'","''",$jsn[0]->address);
	  $custName = str_replace("'","''",$jsn[0]->custName);
	  $dischargeArea = str_replace("'","''",$jsn[0]->dischargeArea);
	  $contactName = str_replace("'","''",$jsn[0]->contactName);
	  $contactPhone = $jsn[0]->contactPhone;
	  $contactMobile = $jsn[0]->contactMobile;
	  $quantity = $jsn[0]->quantity;
	  $remarks = str_replace("'","''",$jsn[0]->remarks);
	  $signerName = str_replace("'","''",$jsn[0]->signerName);
	  $signerRole = $jsn[0]->signerRole;
	  $signEmail = $jsn[0]->signEmail;
	  $signRemarks = str_replace("'","''",$jsn[0]->signRemarks);
	  $signPic = $jsn[0]->signPic;
	  $signPic = str_replace(' ', '+', $signPic);
	  $weigeCertNum= str_replace("'","''",$jsn[0]->weigeCertNum); 
	  if(strlen(trim($weigeCertNum)) == 0)
	    $weigeCertNum = "0";
	  $weigeRemarks = str_replace("'","''",$jsn[0]->weigeRemarks);
	  $weight = $jsn[0]->weight;
	  if(strlen(trim($weight)) == 0)
	    $weight = "0";
	  $numExecRep = str_replace("'","''",$jsn[0]->numExecRep);
	  $invStatus = $jsn[0]->invStatus;
	  $qry = "UPDATE tblInvs SET siteName='$siteName',address='$address',custName='$custName',dischargeArea='$dischargeArea',contactName='$contactName',contactPhone='$contactPhone',contactMobile='$contactMobile',quantity='$quantity',remarks='$remarks',signerName='$signerName',signerRole='$signerRole',signEmail='$signEmail',signRemarks='$signRemarks',signPic='$signPic',weigeCertNum='$weigeCertNum',weigeRemarks='$weigeRemarks',weight='$weight',numExecRep='$numExecRep',invStatus='$invStatus' where invNum='$invNum' and ('$invStatus'<>6 or numImages_erSnap>0)";
      $result = $mysqli->query($qry) or die(mysqli_error());
	break;

	case "delInv":
	  $invNum = $_POST['invNum'];
	  $qry = "DELETE FROM tblinvs WHERE invNum = '$invNum'";
	  $mysqli->query($qry) or die(mysqli_error());
	break;
	    
	case "uploadImage":
	  $invNum = $_POST['invNum'];
	  $pg = $_POST['pg'];
	  $imgInd = $_POST["imgInd"];
	  if(strlen(trim($imgInd)) == 0){
	    $imgInd = "-1";  
	  }
	  $imgData = $_POST['imgData'];
	  if(!SaveImage($mysqli, $invNum, $pg, $imgInd, $imgData)===false){
	    $numImages = CountNumInvImages($invNum. "_". $pg);
		$query = "UPDATE `tblInvs` SET numImages_" . $pg . "=".  "'$numImages' WHERE invNum='$invNum'";
		$mysqli->query($query) or die(mysqli_error());
	  }
	  break;
	
	default:
        echo "Your favorite color is neither red, blue, nor green! op=".$op;
}
?>
