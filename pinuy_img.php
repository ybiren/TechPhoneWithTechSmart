<?php 
header("Access-Control-Allow-Origin: *");
$mysqli = new mysqli('localhost','drivers', 'a1b2c3d5e6f7','dbdrivers'); 
if ($mysqli->connect_error) {
    die('ERROR: Can\'t connect to MySQL - ' . $mysqli->connect_error);
}
$mysqli->set_charset("utf8") or die('Error setting charset');


//for example : sign_15493587_1
$picDtls = $_GET['pic'];  
$picDtls = str_replace("P_sign","sign",$picDtls);  

$picDtlsArr = explode("_",$picDtls);
$callCode = $picDtlsArr[1];

if($picDtlsArr[0]=="sign"){
  $result=$mysqli->query("SELECT signPic  FROM tblInvs WHERE invNum='$callCode'") or die(mysqli_error());  
  $dat = $result->fetch_assoc();
  $signPicFileName = $dat['signPic'];
  $new_data=explode(";",$signPicFileName);
  $type=$new_data[0];
  $type = str_replace($type,"data:","");
  $data=explode(",",$new_data[1]);
  header("Content-type:".$type);
  echo base64_decode($data[1]);
}
else{ //picture
  //$picInd = $picDtlsArr[2];
  //echo 'Images/'.$callCode.'_'.$picInd.'.jpg';
  $picDtls = str_replace("P_","",$picDtls);  
  $data= file_get_contents('Images_Pinuyeem/'.$picDtls);
  echo $data;  
  //echo 'Images1/'.$picDtls; 
}
?>