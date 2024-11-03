<?php 
header("Access-Control-Allow-Origin: *");
$mysqli = new mysqli('localhost','root', 'a1b2c3d5','dbTech'); 
if ($mysqli->connect_error) {
    die('ERROR: Can\'t connect to MySQL - ' . $mysqli->connect_error);
}
$mysqli->set_charset("utf8") or die('Error setting charset');


//for example : sign_15493587_1
$picDtls = $_GET['pic'];  
$picDtlsArr = explode("_",$picDtls);
$callCode = $picDtlsArr[1];
if($picDtlsArr[0]=="sign"){
  $result = $mysqli->query("SELECT signPicFileName  FROM tblRep WHERE callCode='$callCode' and unitCode=0") or die(mysqli_error());  
  $dat = $result->fetch_assoc();
  $signPicFileName = $dat['signPicFileName'];
  $new_data=explode(";",$signPicFileName);
  $type=$new_data[0];
  $type = str_replace($type,"data:","");
  $data=explode(",",$new_data[1]);
  header("Content-type:".$type);
  echo base64_decode($data[1]);
}
else{ //picture
  //$picInd = $picDtlsArr[2];
  $picDtls = str_replace("pic_","",$picDtls);
  //echo 'Images/'.$callCode.'_'.$picInd.'.jpg';
  $data= file_get_contents('Images/'.$picDtls);
  echo $data;  
  //echo 'Images1/'.$picDtls; 
}

?>