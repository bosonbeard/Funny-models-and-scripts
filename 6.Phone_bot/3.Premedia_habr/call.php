<?php
    header("Access-Control-Allow-Origin: *");
    header("Access-Control-Allow-Headers: access");
    header("Access-Control-Allow-Methods: POST");
    header("Access-Control-Allow-Credentials: true");
    header('Content-Type: application/json; charset=utf-8');
    # logger
    $params = (array) json_decode(file_get_contents('php://input'), TRUE);
    $fw = fopen("log.txt", "a+");
    fwrite($fw, var_export($params, true));
    fclose($fw);
    # read Telecom API request data
    $data = json_decode(file_get_contents("php://input"));
    # get caller phone number
	$numberA = (int)$data->params->numberA;
    $id = (int)$data->id;

	# set initial vars

  	$shop_URL="http://your_domain.ru/";
	$API_key="Presta api key";

    $default_number = "7900000000Z";
    $kitchen_number="7900000000Z";
    $delivery_number="7900000000Z";

    $client_order_id =0;
    $client_order_state=0;
    $client_mobile=0;

    $promt_migele="c89020935c6157f8370730a08faXXXX";
    $promt_pablo="92b12508157acc3e69d5b3d09af7XXXX";
    $promt_25mc="ca15d7db7e1ecc6d95976bfa1efeXXXX";
    $promt_50mc="ade3e6b9ff021b8df3174f2e6b37XXXX";
    $promt_pablo="";

    $file_to_A = "";
    $file_to_B = "";

	# import prestaShop library
	require_once('./PSWebServiceLibrary.php');

	# get orders
	# state: 14 -kitchen , 15 - delivery
	try {
		// creating webservice access
		$webService = new PrestaShopWebservice($shop_URL, $API_key, false);

		// call to retrieve all customers
		$xml = $webService->get(
			array
			(
				'resource' => 'orders',
				'filter[current_state]' => '[14|15]',
				'display'  => '[id,current_state,id_address_delivery]'
			)

	);

	} catch (PrestaShopWebserviceException $ex) {
		// Shows a message related to the error
	}
	$orders = $xml->orders->children();

    # check if caller has on finished orders
	foreach ($orders as $order) {

			foreach ($order  as $key => $value) {
                if ($key == 'id') {
                    $client_order_id =   $value;
                }
                elseif ($key == 'current_state') {
                    $client_order_state =   $value;
                  }
				  else {

                    try {
                        // check address to get mobile phone number
                        $webService = new PrestaShopWebservice($shop_URL, $API_key, false);

                        // call to retrieve all customers
                        $xml2 = $webService->get(
                            array
                            (
                                'resource' => 'addresses',
                                'id' =>  $value
                            )

                    );

                    } catch (PrestaShopWebserviceException $ex) {
                        // Shows a message related to the error
                        echo 'Other error: <br />' . $ex->getMessage();
                    }

                    $mobile_phone = $xml2->address->phone_mobile;
                    if ($mobile_phone == $numberA){
                        $client_mobile=$mobile_phone;
                    }

                }
			}
			# if client has unfinished orders redirect call to kitchen or delivery
            if ($client_mobile != 0){

                $result = mail('roman-prog@mail.ru', "Call for order $client_order_id", "Client phone $client_mobile, order state $client_order_state");
                if(!$result) {
                 #    echo "Error";
                } else {
                 #   echo "Success";
                }
                if ($client_order_state == 14 )
                {
                    $redirect_number = $kitchen_number;
                    $file_to_A = $promt_50mc;
                    $file_to_B = $promt_migele;
                }
                else{
                    $redirect_number = $delivery_number;
                    $file_to_A = $promt_25mc;
                    $file_to_B = $promt_pablo;
                }
                break 1;
            }
            else
            {
   				# set default manager number to redirect
				$redirect_number = $default_number;
            }

		}

    # create API response to Telecom API
    $response_call = array(
        "jsonrpc" => "2.0",
        "id" => "1",
        "result" => array(
            "redirect_type" => 1,
            "event_URL" => 	"some URL",
            "event_extended" => "Y",
            "client_id" => "1235",
            "file_to_A" => "$file_to_A",
            "file_to_B" => "$file_to_B",
            "masking" => "Y",
            "followme_struct" => array(
                1,
                array(
                    array(
                        "I_FOLLOW_ORDER" => 1,
                        "PERIOD" => "always",
                        "PERIOD_DESCRIPTION" => "always",
                        "TIMEOUT" => "90",
                        "ACTIVE" => "Y",
                        "NAME" => "$numberA",
                        "REDIRECT_NUMBER" =>"$redirect_number"
                    )

                )
            )
        )
    );

  #  send response to Telecom API

  echo json_encode($response_call, JSON_UNESCAPED_UNICODE | JSON_UNESCAPED_SLASHES | JSON_PRETTY_PRINT);
