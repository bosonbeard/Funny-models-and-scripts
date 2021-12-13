/**

             Adding custom status for hanr

*/

function register_voicebox_call_statuses() {

    register_post_status( 'wc-need_call', array(

        'label'                     => 'Нужен звонок',

        'public'                    => true,

        'show_in_admin_status_list' => true,

        'show_in_admin_all_list'    => true,

        'exclude_from_search'       => false,

        'label_count'               => _n_noop( 'Нужен звонок <span class="count">(%s)</span>', 'Нужен звонок <span class="count">(%s)</span>' )

    ) );
	
	    register_post_status( 'wc-confirmed_ordr', array(

        'label'                     => 'Заказ подтвержден',

        'public'                    => true,

        'show_in_admin_status_list' => true,

        'show_in_admin_all_list'    => true,

        'exclude_from_search'       => false,

        'label_count'               => _n_noop( 'Заказ подтвержден <span class="count">(%s)</span>', 'Заказ подтвержден <span class="count">(%s)</span>' )

    ) );

}

add_action( 'init', 'register_voicebox_call_statuses' );

function add_voicebox_call_to_order_statuses( $order_statuses ) {

    $new_order_statuses = array();

    foreach ( $order_statuses as $key => $status ) {

        $new_order_statuses[ $key ] = $status;

        if ( 'wc-processing' === $key ) {

            $new_order_statuses['wc-need_call'] = 'Нужен звонок';
			$new_order_statuses['wc-confirmed_ordr'] = 'Заказ подтвержден';

        }

    }

    return $new_order_statuses;

}

add_filter( 'wc_order_statuses', 'add_voicebox_call_to_order_statuses' );

/**

             Adding custom actions for hanr

*/
// Use the getter function to get order ID  



add_action( 'woocommerce_order_actions', 'call_voice1' );
function call_voice1( $actions ) {
    $actions['call_voice'] = __( 'Звонок бота', 'text_domain' );
    return $actions;
}

add_action( 'woocommerce_order_action_call_voice', 'call_voice2' );
function call_voice2(  ) {
	
    global $woocommerce, $post;

    $order = new WC_Order($post->ID);

//to escape # from order id 

    $order_id = trim(str_replace('#', '', $order->get_order_number()));	

    $billing_phone  = $order->get_billing_phone();
    $billing_phone =str_replace("+","",$billing_phone);
    $order_total = $order->get_total();

    $billing_first_name = $order->get_billing_first_name();
	
    // Iterating through each WC_Order_Item_Product objects

    $goods = "";	
	foreach ($order->get_items() as $item_key => $item_values):

    ## Using WC_Order_Item methods ##

    // Item ID is directly accessible from the $item_key in the foreach loop or

     
    ## Using WC_Order_Item_Product methods ##

        $item_name = $item_values->get_name(); // Name of the product
    	$goods=$goods.'"'.$item_name.'",';


    endforeach;
	
//echo ($goods);	
//echo ($billing_phone);
//echo ($billing_first_name);
//echo ($order_total);	
	
    $curl = curl_init();

    curl_setopt_array($curl, array(
        CURLOPT_URL => 'https://voicebox.mtt.ru/api/v1/sb',
        CURLOPT_RETURNTRANSFER => true,
        CURLOPT_ENCODING => '',
        CURLOPT_MAXREDIRS => 10,
        CURLOPT_TIMEOUT => 0,
        CURLOPT_FOLLOWLOCATION => true,
        CURLOPT_HTTP_VERSION => CURL_HTTP_VERSION_1_1,
        CURLOPT_CUSTOMREQUEST => 'POST',
        CURLOPT_POSTFIELDS =>'{
            "method": "10dada97-b6c0-450d-b681-6a46a6484be1",
            "data": {
            "number": "'.$billing_phone.'",
            "goods": ['.rtrim($goods, ",").'],
            "name": "'.$billing_first_name.'",
            "total":'.$order_total.',
            "orderid":'.$order_id.'}
            }',
        CURLOPT_HTTPHEADER => array(
        'Authorization: Basic NTA4NDhhNjgtMjAxZi00Mjg3LWEyZjItMjRhNmM4NGEyMDc5Ok93NyZAfjZyMUc=',
        'Content-Type: application/json'
    ),
    ));

    $response = curl_exec($curl);

    curl_close($curl);
    echo $response;

//	exit("Это заглушка");
}


add_action( 'woocommerce_order_actions', 'woman_voice1' );
function woman_voice1( $actions ) {
    $actions['woman_voice'] = __( 'Женский голос у бота', 'text_domain' );
    return $actions;
}



add_action( 'woocommerce_order_action_woman_voice', 'woman_voice2' );
function woman_voice2(  ) {


 $order= wc_get_order( $order_id ); // Get the WC_Order Object instance	
  echo ("aaaaa");
	


  echo $order_id;
  // echo $order->get_id();
  echo ($_GET['post']);
	  echo ("vvaaaaa");
  exit("Это заглушка");
}

add_action( 'woocommerce_order_actions', 'man_voice1' );
function man_voice1( $actions ) {
    $actions['man_voice'] = __( 'Мужской голос у бота', 'text_domain' );
    return $actions;
}

add_action( 'woocommerce_order_action_man_voice', 'man_voice2' );
function man_voice2(  ) {
   echo("<script>console.log('man_voice');</script>");
  exit("Это заглушка");
}
