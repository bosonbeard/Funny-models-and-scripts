<?php



function count_votes($number, $date_gt, $date_lt,  $first, $second)
{  
    # $first - string for 1st candidate
    # $second - string for 2ns candidate

    #settings
    $url = "https://api.mtt.ru/ms-customer-gateway/v1/GetMessagesHistoryList";
    $customer_name = "te******";
    $token = '******';
    
    #curl request
    $curl = curl_init();
    curl_setopt_array($curl, array(
    CURLOPT_URL => 'https://api.mtt.ru/ms-customer-gateway/v1/GetMessagesHistoryList',
    CURLOPT_RETURNTRANSFER => true,
    CURLOPT_ENCODING => '',
    CURLOPT_MAXREDIRS => 10,
    CURLOPT_TIMEOUT => 0,
    CURLOPT_FOLLOWLOCATION => true,
    CURLOPT_HTTP_VERSION => CURL_HTTP_VERSION_1_1,
    CURLOPT_CUSTOMREQUEST => 'POST',
    CURLOPT_POSTFIELDS =>'{
        "customer_name": "'.$customer_name.'",
        "number":"'.$number.'",
        "event_date_gt":"'.$date_gt.'",
        "event_date_lt":"'.$date_lt.'",
        "direction":"incoming",
        "delivery_status":"delivered"
    }
    ',
    CURLOPT_HTTPHEADER => array(
        'Authorization: Bearer '.$token.'',
        'Content-Type: application/json'
    ),
    ));

    $response = curl_exec($curl);
    curl_close($curl);

    # count voices
    $msg = json_decode($response , true);

    $count_first =0 ;
    $count_second = 0;

    foreach ($msg as $key  ) {

        foreach ($key  as $key2=>$value ) {

            if ( strtolower( $value["text"]) == $first )
            {
                $count_first+= 1;
            }
            if ( strtolower( $value["text"]) == $second )
            {
                $count_second+= 1;
            }            
        }
     
    }

    return [$count_first,$count_second] ;

}
?>
