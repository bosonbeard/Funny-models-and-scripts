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


add_action( 'woocommerce_order_actions', 'call_voice1' );
function call_voice1( $actions ) {
    $actions['call_voice'] = __( 'Звонок бота', 'text_domain' );
    return $actions;
}

add_action( 'woocommerce_order_action_call_voice', 'call_voice2' );
function call_voice2(  ) {
   echo("<script>console.log('call_voice');</script>");
  exit("Это заглушка");
}


add_action( 'woocommerce_order_actions', 'woman_voice1' );
function woman_voice1( $actions ) {
    $actions['woman_voice'] = __( 'Женский голос у бота', 'text_domain' );
    return $actions;
}

add_action( 'woocommerce_order_action_woman_voice', 'woman_voice2' );
function woman_voice2(  ) {
   echo("<script>console.log('woman_voice');</script>");
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
