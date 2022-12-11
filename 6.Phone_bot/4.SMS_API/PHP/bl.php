<?php

   header("Access-Control-Allow-Origin: *");

   header("Access-Control-Allow-Headers: access");

   header("Access-Control-Allow-Methods: POST");

   header("Access-Control-Allow-Credentials: true");

   header('Content-Type: application/json; charset=utf-8');



   $data = json_decode(file_get_contents("php://input"),true);
      # logger - лог проверки запроса
    $fw = fopen("log.txt", "a+");

    fwrite($fw, var_export($data, true));
 
    fclose($fw);

#settings
    $url = "https://api.mtt.ru/ms-customer-gateway/v1/AddBlackList";
    $customer_name = "te*****";
    $token = 'eX****XfGv7wMQ30';
   
    # read Telecom API request data
   
  # get caller phone number
  $body = Json_decode($data["request"]["body"]);
  $text = mb_strtolower($body->text, 'UTF-8');
  $sender = $body->sender;

   if ( strpos($text, 'мышь' ) !== false)
   {
        echo "correct ". $sender." in black list";
     
# send request to blacklist method
    $curl = curl_init();

    curl_setopt_array($curl, array(
      CURLOPT_URL => $url,
      CURLOPT_RETURNTRANSFER => true,
      CURLOPT_ENCODING => '',
      CURLOPT_MAXREDIRS => 10,
      CURLOPT_TIMEOUT => 0,
      CURLOPT_FOLLOWLOCATION => true,
      CURLOPT_HTTP_VERSION => CURL_HTTP_VERSION_1_1,
      CURLOPT_CUSTOMREQUEST => 'POST',
      CURLOPT_POSTFIELDS =>'{
        "customer_name": "'.$customer_name.'",
      "numbers":[
            "'. $sender.'"
        ]
    }
    ',
      CURLOPT_HTTPHEADER => array(
        'Authorization: Bearer '.$token.'',
        'Content-Type: application/json'
      ),
    ));

    $response = curl_exec($curl);

    curl_close($curl);
   }

echo json_encode($response, JSON_UNESCAPED_UNICODE | JSON_UNESCAPED_SLASHES | JSON_PRETTY_PRINT);
?>
