<?php

include_once __DIR__.'/config/database.php';
include_once __DIR__.'/../config.php';
require __DIR__."/../vendor/autoload.php";



use \Firebase\JWT\JWT;

header("Access-Control-Allow-Origin: *");
header("Content-Type: application/json; charset=UTF-8");
header("Access-Control-Allow-Methods: POST");
header("Access-Control-Max-Age: 2200");
header("Access-Control-Allow-Headers: Content-Type, Access-Control-Allow-Headers, Authorization, X-Requested-With");


function returnJWT(&$username,&$id){
    $secret_key = SECRET_KEY;
    $issuer_claim = "THE_SERVER_NAME"; // this can be the servername
    $audience_claim = "THE_AUDIENCE";
    $issuedat_claim = time(); // issued at
    $notbefore_claim = $issuedat_claim + 0; //not before in seconds
    $expire_claim = $issuedat_claim + JWT_EXPIRE; // expire time in seconds
    $token = array(
        "iss" => $issuer_claim,     // iss – A string containing the name or identifier of the issuer application. Can be a domain name and can be used to discard tokens from other applications.
        "aud" => $audience_claim,
        "iat" => $issuedat_claim,   // iat – timestamp of token issuing.
        "nbf" => $notbefore_claim,  // nbf – Timestamp of when the token should start being considered valid. Should be equal to or greater than iat. In this case, the token will begin to be valid after 10 seconds after being issued
        "exp" => $expire_claim,     // exp – Timestamp of when the token should stop to be valid. Needs to be greater than iat and nbf. In our example, the token will expire after 60 seconds of being issued.
        "data" => array(
            "id" => $id,
            "username" => $username
    ));

    $jwt = JWT::encode($token, $secret_key);
    echo json_encode(
        array(
            "message" => "Successful login.",
            "jwt" => $jwt,
            "id" => $id,
            "username" => $username,
            "expireAt" => $expire_claim
        ));
};

$username = '';
$password = '';

$databaseService = new DatabaseService();
$conn = $databaseService->getConnection();


$username  = trim($_GET["username"]);
$password  = trim($_GET["password"]);

$query = 'SELECT id, username, password FROM user WHERE username="'.$username.'" LIMIT 0,1';

$stmt = $conn->prepare( $query );
$stmt->execute();
$num = $stmt->rowCount();

if($num > 0){
    $row = $stmt->fetch(PDO::FETCH_ASSOC);
    $id = $row['id'];
    $username_db = $row['username'];
    $password_db = $row['password'];



    if(trim($password) == trim($password_db))
    {

        http_response_code(200);
        returnJWT($username, $id);
 
    }else{

        http_response_code(401);
        echo json_encode(array("message" => "Login failed.", "password" => $password));
    }
}else{
  

    $query = 'INSERT INTO user SET username="'.$username.'",password="'.$password.'"';

    $stmt = $conn->prepare($query);
    $stmt->bindValue(':username', $username);
    $stmt->bindValue(':password', $password);


    if($stmt->execute()){

        $query = 'SELECT id FROM user WHERE username="'.$username.'" LIMIT 0,1';
        $stmt = $conn->prepare( $query );
        $stmt->execute();
        $num = $stmt->rowCount();
        if($num > 0){
            $row = $stmt->fetch(PDO::FETCH_ASSOC);
            $id = $row['id'];
            http_response_code(200);
            returnJWT($username,$id);
        }else{
            http_response_code(400);
            echo json_encode(array("message" => "An error occurred"));
        }

    

    }
    else{
        http_response_code(400);
        echo json_encode(array("message" => "An error occurred"));
    }



}

?>