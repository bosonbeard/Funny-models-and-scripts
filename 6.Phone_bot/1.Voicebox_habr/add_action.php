/**

Adding custom status for hanr

*/

// Регистрируем наши новые статусы заказа в системе
function register_voicebox_call_statuses() {

register_post_status( 'wc-need_call', array(

'label' => 'Нужен звонок',

'public' => true,

'show_in_admin_status_list' => true,

'show_in_admin_all_list' => true,

'exclude_from_search' => false,

'label_count' => _n_noop( 'Нужен звонок <span class="count">(%s)</span>', 'Нужен звонок <span class="count">(%s)</span>' )

) );

register_post_status( 'wc-confirmed_ordr', array(

'label' => 'Заказ подтвержден',

'public' => true,

'show_in_admin_status_list' => true,

'show_in_admin_all_list' => true,

'exclude_from_search' => false,

'label_count' => _n_noop( 'Заказ подтвержден <span class="count">(%s)</span>', 'Заказ подтвержден <span class="count">(%s)</span>' )

) );

}

add_action( 'init', 'register_voicebox_call_statuses' );

// добавляем статусы куда-то
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



// Добавим действие со звонком в список


add_action( 'woocommerce_order_actions', 'call_voice1' );
function call_voice1( $actions ) {
$actions['call_voice'] = __( 'Звонок бота', 'text_domain' );
return $actions;
}

// Добавим непосредственно логику для звонка
add_action( 'woocommerce_order_action_call_voice', 'call_voice2' );
function call_voice2( ) {

// Получим данные о заказе
global $woocommerce, $post;
$order = new WC_Order($post->ID);

//to escape # from order id
$order_id = trim(str_replace('#', '', $order->get_order_number()));
//Получим основные данные о заказе и заказчике
$billing_phone = $order->get_billing_phone();
$billing_phone =str_replace("+","",$billing_phone);
$order_total = $order->get_total();
$billing_first_name = $order->get_billing_first_name();
$goods = "";
// Пройдемся циклически по всем продуктам заказа и заберем их названия

foreach ($order->get_items() as $item_key => $item_values):
$item_name = $item_values->get_name(); // Name of the product
$goods=$goods.'"'.$item_name.'",';
endforeach;


// cURL запрос автоматически сгенерирован в Postman, я просто добавил перменные в разрывы строк.
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
"method": "10dada97-XXXX-450d-b681-6a46a6484be1",
"data": {
"number": "'.$billing_phone.'",
"goods": ['.rtrim($goods, ",").'],
"name": "'.$billing_first_name.'",
"total":'.$order_total.',
"orderid":'.$order_id.'}
}',
CURLOPT_HTTPHEADER => array(
'Authorization: Basic XXXXXXhhNjgtMjAxZi00Mjg3LWEyZjItMjRhNmM4NGEyMDc5Ok93NyZAfjZyMUc=',
'Content-Type: application/json'
),
));
$response = curl_exec($curl);
curl_close($curl);
}

// Функция для обновления голоса, через полное обновление сценария

function change_voice($voice) {

$curl = curl_init();

curl_setopt_array($curl, array(
CURLOPT_URL => 'https://voicebox-api.mtt.ru/v1/customers/XX/scenarios/XX2536',
CURLOPT_RETURNTRANSFER => true,
CURLOPT_ENCODING => '',
CURLOPT_MAXREDIRS => 10,
CURLOPT_TIMEOUT => 0,
CURLOPT_FOLLOWLOCATION => true,
CURLOPT_HTTP_VERSION => CURL_HTTP_VERSION_1_1,
CURLOPT_CUSTOMREQUEST => 'PUT',
CURLOPT_POSTFIELDS =>'{
"id": 252536,
"name": "Для Хабра 1",
"type": "outgoing",
"comment": "",
"states": [
{
"type": "Initial",
"name": "State",
"x": -668,
"y": 102,
"error": null,
"success": {
"title": "",
"newState": "Вызов"
}
},
{
"type": "OutgoingCall",
"name": "Вызов",
"x": -512,
"y": 205,
"error": null,
"success": {
"title": "",
"newState": "Зачитать список товаров"
},
"phone": "{{numberB}}"
},
{
"type": "VoiceMessage",
"name": "Зачитать список товаров",
"x": -307,
"y": 331,
"error": null,
"success": {
"title": "",
"newState": "Подтверждение"
},
"detectAI": true,
"onDetectAI": {
"title": "",
"newState": "Неуспех"
},
"announcement": {
"type": "text",
"file": "",
"text": "Здравствуйте {{name}}, вы заказали {{goods}}, на сумму {{total}} рублей"
},
"onNoInput": null,
"onExpiry": null,
"onAParty": null,
"onInvalidInput": null
},
{
"type": "ReleaseCall",
"name": "Отбой",
"x": -72,
"y": 988,
"error": null,
"success": null
},
{
"type": "HTTPRequest",
"name": "Неуспех",
"x": -472,
"y": 669,
"error": null,
"success": {
"title": "",
"newState": "Отбой"
},
"method": "PUT",
"host": "XXXX.ru",
"protocol": "https",
"port": 443,
"url": "/wp-json/wc/v3/orders/{orderid}",
"headers": {
"Authorization": "Basic XXXXXjBiYTI0NjQ0MmUxYjU0ZThhM2RjY2Y1ZDFkY2JkZTE2N2Y5NTQ4YTpjc182NjU2ODVlYjUzMWEwN2JkY2Y3Y2FmYjcwNjY5ODQ4YmRlNmRjZjQ0"
},
"requestParameters": null,
"requestParametersRaw": "{\\"status\\": \\"need_call\\"}",
"requestParametersType": "raw",
"responseCodeVariable": "RESPONSE_CODE_1",
"responseBodyVariables": {}
},
{
"type": "IVR",
"name": "Подтверждение",
"x": 25,
"y": 364,
"error": {
"title": "",
"newState": "Дождитесь оператора"
},
"success": null,
"onDetectAI": null,
"awaitingTime": 10,
"announcement": {
"type": "text",
"file": "",
"text": "Вы подтверждаете заказ?"
},
"repeatCount": 3,
"detectAI": false,
"voiceInput": true,
"keyboardInput": false,
"immediateStt": false,
"inputAnalysis": [
{
"success": {
"title": "",
"newState": "Обновить статус"
},
"dtmf": "",
"words": null,
"keywords": [
{
"name": "Согласие",
"id": "5",
"lost": false
}
]
}
],
"onNoInput": {
"title": "",
"newState": "Дождитесь оператора"
},
"onExpiry": {
"title": "",
"newState": "Дождитесь оператора"
},
"onAParty": null,
"onInvalidInput": null
},
{
"type": "HTTPRequest",
"name": "Обновить статус",
"x": 91,
"y": 606,
"error": null,
"success": {
"title": "",
"newState": "Спасибо за подтверждение"
},
"method": "PUT",
"host": "XXXX.ru",
"protocol": "https",
"port": 443,
"url": "/wp-json/wc/v3/orders/{{orderid}}",
"headers": {
"Authorization": "Basic XXXXjBiYTI0NjQ0MmUxYjU0ZThhM2RjY2Y1ZDFkY2JkZTE2N2Y5NTQ4YTpjc182NjU2ODVlYjUzMWEwN2JkY2Y3Y2FmYjcwNjY5ODQ4YmRlNmRjZjQ0"
},
"requestParameters": null,
"requestParametersRaw": "{\\n \\"status\\": \\"confirmed_ordr\\"\\n}",
"requestParametersType": "raw",
"responseCodeVariable": "RESPONSE_CODE_2",
"responseBodyVariables": {}
},
{
"type": "Redirection",
"name": "Звонок оператору",
"x": -136,
"y": 722,
"error": null,
"success": {
"title": "",
"newState": "Отбой"
},
"numberB": "{{RESPONSE_CODE_1}}",
"extension": "",
"callingPartyDisplay": "forwarder_number",
"forwardType": "number",
"sipUri": "",
"credentialInfo": null
},
{
"type": "VoiceMessage",
"name": "Дождитесь оператора",
"x": -194,
"y": 499,
"error": null,
"success": {
"title": "",
"newState": "Звонок оператору"
},
"detectAI": false,
"onDetectAI": null,
"announcement": {
"type": "text",
"file": "",
"text": "Дождитесь оператора"
},
"onNoInput": null,
"onExpiry": null,
"onAParty": null,
"onInvalidInput": null
},
{
"type": "VoiceMessage",
"name": "Спасибо за подтверждение",
"x": 185,
"y": 842,
"error": null,
"success": {
"title": "",
"newState": "Отбой"
},
"detectAI": false,
"onDetectAI": null,
"announcement": {
"type": "text",
"file": "",
"text": "Спасибо за подтверждение"
},
"onNoInput": null,
"onExpiry": null,
"onAParty": null,
"onInvalidInput": null
}
],
"customVariables": [
"goods",
"total",
"name",
"orderid"
],
"campaign": {
"id": 252729,
"name": "Хабр1",
"campaignType": "outbound",
"scenario": {
"id": 252536,
"name": "",
"type": "",
"comment": "",
"states": null,
"customVariables": null,
"createdAt": "0001-01-01T00:00:00Z",
"updatedAt": "0001-01-01T00:00:00Z",
"settings": {
"voice": "",
"speed": 0,
"emotion": ""
},
"shareLink": ""
},
"callerNumber": {
"id": 157409,
"number": "74997059704",
"type": "ABC",
"owner": "MTT",
"regionName": "Moscow",
"regionCode": "MOW",
"category": "REGULAR",
"description": "number buying",
"status": "Active",
"campaigns": null
},
"entryPoint": {
"id": 252728,
"url": ""
},
"launchBy": "http",
"routeId": 26589,
"createdAt": "2021-12-12T17:21:50Z",
"updatedAt": "2021-12-12T17:21:50Z"
},
"createdAt": "0001-01-01T00:00:00Z",
"updatedAt": "0001-01-01T00:00:00Z",
"settings": {
"voice": "'.$voice.'",
"speed": 100,
"emotion": "neutral"
},
"shareLink": "XXXXX-c3d9-4d71-b23d-c0c47ca206f2"
}',
CURLOPT_HTTPHEADER => array(
'Authorization: Bearer XXXXXMMZKTNWEYYY0ZNJQXLWJLYTETOGRMYTY4ZGEZZMZK',
'Content-Type: application/json'
),
));

$response = curl_exec($curl);

curl_close($curl);
echo ($response);
// exit('Раскомментируй чтобы посмотреть вывод')

}

// добавляем дейсвтие в список действий
add_action( 'woocommerce_order_actions', 'woman_voice1' );
function woman_voice1( $actions ) {
$actions['woman_voice'] = __( 'Женский голос у бота', 'text_domain' );
return $actions;
}


// добавляем логику по замене голоса на женский
add_action( 'woocommerce_order_action_woman_voice', 'woman_voice2' );
function woman_voice2( ) {

change_voice("oksana");
}


// добавляем дейсвтие в список действий
add_action( 'woocommerce_order_actions', 'man_voice1' );
function man_voice1( $actions ) {
$actions['man_voice'] = __( 'Мужской голос у бота', 'text_domain' );
return $actions;
}

// добавляем логику по замене голоса на женский
add_action( 'woocommerce_order_action_man_voice', 'man_voice2' );
function man_voice2( ) {
change_voice("filipp");
}
