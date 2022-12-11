/*
 * Copyright Camunda Services GmbH and/or licensed to Camunda Services GmbH
 * under one or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information regarding copyright
 * ownership. Camunda licenses this file to you under the Apache License,
 * Version 2.0; you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

const { Client, logger } = require('camunda-external-task-client-js');
const { Variables } = require('camunda-external-task-client-js');
let axios = require('axios');
const mtt_login = "testcust_mtt";
const mtt_number = "79*******41";
const igor_number = "7******88";
const token = "eyJhbGciOiJIUzI1NiIs*****1Q30";


// configuration for the Client:
//  - 'baseUrl': url to the Process Engine
//  - 'logger': utility to automatically log important events
//  - 'asyncResponseTimeout': long polling timeout (then a new request will be issued)
const config = { baseUrl: 'http://localhost:8080/engine-rest', use: logger, asyncResponseTimeout: 10000 };

// create a Client instance with custom configuration
const client = new Client(config);

// susbscribe to the topic: 'charge-card'
client.subscribe('send-sms', async function ({ task, taskService }) {
	// Put your business logic here
	var axios = require('axios');

	var data = JSON.stringify({
		"number": mtt_number,
		"destination": igor_number,
		"text": "Полей цветы"
	});

	var config = {
		method: 'post',
		url: 'https://api.mtt.ru/ms-customer-gateway/v1/SendSms',
		headers: {
			'Authorization': `Bearer ${token}`,
			'Content-Type': 'application/json'
		},
		data: data
	};

	axios(config)
		.then(function (response) {
			 console.log(JSON.stringify(response.data));
		})
		.catch(function (error) {
			console.log(error);
		});


	console.log(`SMS sent`);

	// Complete the task
	await taskService.complete(task);
});


function get_numbers() {

	var endDate = new Date()

	var startdate = new Date(endDate);

	var durationInMinutes = 10;

	startdate.setMinutes(endDate.getMinutes() - durationInMinutes);


	var axios = require('axios');
	var data = JSON.stringify({
		"customer_name": mtt_login,
		"number": mtt_number,
		"direction": "incoming",
		"event_date_gt": startdate,
		"event_date_lt": endDate
	});

	var config = {
		method: 'post',
		url: 'https://api.mtt.ru/ms-customer-gateway/v1/GetMessagesHistoryCount',
		headers: {
			'Authorization': `Bearer ${token}`,
			'Content-Type': 'application/json'
		},
		data: data
	};
	return axios(config)
}

var count_numbers = async function () {
	let output = await get_numbers();
	return output
}


client.subscribe('check-sms', async function ({ task, taskService }) {

	const variables = new Variables();
	let sent = false;

	// get number of  received SMS in last 10 min interval
	res = await count_numbers();

	// if sms received go to end of process in Camunda
	if (res.data.count > 0) {
		sent = true;
	}
	else {
		// repeat send SMS in Camunda
		sent = false;
	}

	variables.set("sent", sent);

	console.log(`SMS recieved`);

	// Complete the task
	await taskService.complete(task, variables);
});
