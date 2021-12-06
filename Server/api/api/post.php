<?php
include_once './config/database.php';
include_once '../config.php';
require "../vendor/autoload.php";
use \Firebase\JWT\JWT;

header("Access-Control-Allow-Origin: *");
header("Content-Type: application/json; charset=UTF-8");
header("Access-Control-Allow-Methods: POST,GET,PUT,DELETE");
header("Access-Control-Max-Age: 3600");
header("Access-Control-Allow-Headers: Content-Type, Access-Control-Allow-Headers, Authorization, X-Requested-With");

$jwt = null;
$databaseService = new DatabaseService();
$conn = $databaseService->getConnection();

$authHeader = $_SERVER['HTTP_AUTHORIZATION'];
$requestMethod=$_SERVER["REQUEST_METHOD"];
$arr = explode(" ", $authHeader);
$jwt = $arr[1];


function isLoggedIn($jwt){
    try {
        $decoded = JWT::decode($jwt, SECRET_KEY, array('HS256'));
        return [true,$decoded];
    }catch (Exception $e){
        return [false];
    } 
}

function getMethod($jwt, $conn){
    $userStatus = isLoggedIn($jwt);

    if($jwt && $userStatus[0]){

        http_response_code(200);
        $query = 'SELECT * FROM content ORDER BY id desc';
        $stmt = $conn->prepare( $query );
        

        if($stmt->execute()){

            $responseData = array();
            $i=0;
            while($row = $stmt->fetch(PDO::FETCH_ASSOC)){
                array_push( $responseData, $row );
            }

            
            http_response_code(200);
            echo json_encode(array("message" => $responseData));
            exit();
        }
        else{
            http_response_code(400);
            echo json_encode(array("message" => "Something went wrong!"));
            exit();
        }


    }else{
        http_response_code(401);
    
         echo json_encode(
            array(
                "message" => "Auth error"
            )
        ); 
    }

}

function postMethod($jwt, $conn){

    $userStatus = isLoggedIn($jwt);
    
    if($jwt && $userStatus[0]){
        
        $id = $userStatus[1]->data->id;
       
        $data = json_decode(file_get_contents("php://input"));
        $comment = $data->comment;
 
        $query = 'INSERT INTO content (user_id, content) VALUES ('.$id.', "'.$comment.'")';
      
        $stmt = $conn->prepare( $query );

        if($stmt->execute()){

            http_response_code(200);
            echo json_encode(array("message" => "Success"));
            exit();
        }
        else{
            http_response_code(400);
            echo json_encode(array("message" => "Something went wrong!"));
            exit();
        } 


    }else{
        http_response_code(401);
        echo json_encode(
            array(
                "message" => "Auth error"
            )
        );
    }
}

function putMethod($jwt, $conn){
    $userStatus = isLoggedIn($jwt);

    if($jwt && $userStatus[0]){
        $id = $userStatus[1]->data->id;
        $comment_id = intval($_GET["comment_id"]);
        $comment = $_GET["comment"];

        $query = 'UPDATE content SET content="'.$comment.'" WHERE id='.$comment_id.' AND user_id="'.$id.'"';
        $stmt = $conn->prepare( $query );

        if($stmt->execute()){

            http_response_code(200);
            echo json_encode(array("message" => "Success"));
            exit();
        }
        else{
            http_response_code(400);
            echo json_encode(array("message" => "Something went wrong!"));
            exit();
        }

    }else{
        http_response_code(401);
        echo json_encode(
            array(
                "message" => "Auth error"
            )
        );
    }
}

function deleteMethod($jwt, $conn){
    $userStatus = isLoggedIn($jwt);

    if($jwt && $userStatus[0]){

        $comment_id = intval($_GET["comment_id"]);
        $id = $userStatus[1]->data->id;
      

        $query = "DELETE FROM content WHERE id=".$comment_id." AND user_id=".$id." ";
      
        $stmt = $conn->prepare( $query );

        if($stmt->execute()){

            http_response_code(200);
            echo json_encode(array("message" => "Success"));
            exit();
        }
        else{
            http_response_code(400);
            echo json_encode(array("message" => "Something went wrong!"));
            exit();
        } 

    }else{
        http_response_code(401);
        echo json_encode(
            array(
                "message" => "Auth error"
            )
        );
    }
}

switch($requestMethod){
    case 'GET':     return getMethod($jwt,      $conn);
    case 'POST':    return postMethod($jwt,     $conn);
    case 'PUT':     return putMethod($jwt,      $conn);
    case 'DELETE':  return deleteMethod($jwt,   $conn);
    default: 
        header("HTTP/1.0 405 Method Not Allowed");
        break;
}


















?>
