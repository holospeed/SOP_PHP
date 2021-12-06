<?php
include(__DIR__.'/../../config.php');

class DatabaseService{

    private $db_host = DB_HOST;
    private $db_name = DB_NAME;
    private $db_user = DB_USER;
    private $db_password = DB_PASS;
    private $connection;

    public function getConnection(){

        $this->connection = null;

        try{
            $this->connection = new PDO("mysql:host=" . $this->db_host . ";dbname=" . $this->db_name, $this->db_user, $this->db_password);
            $this->connection->exec("set names utf8");
        }catch(PDOException $exception){
            echo "Connection failed :( => " . $exception->getMessage();
        }

        return $this->connection;
    }
}
?>