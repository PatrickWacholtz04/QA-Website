async function loadApiData(endpoint) {
    try {
        const response = await fetch('../secret/api.json');
        const data = await response.json();

        api_key = data.API_KEY;

        if (endpoint == "base") {
            api_url = data.base_url;
        }
        else if (endpoint == "hardware") {
            api_url = data.hardware_url;
        }
        else if (endpoint == "user") {
            api_url = data.user_url;
        }
        else {
            api_url = undefined;
        }
        return { key: api_key, url: api_url };
    } catch (error) {
        console.error(error);
    }

}

async function search() {
    // Check inputs in order of precedence given as id, email, name
    const id_input = document.getElementById("id-input");
    const email_input = document.getElementById("email-input");
    const name_input = document.getElementById("name-input");
    // Find which value we should search by in order
    if (id_input.value.trim() != "") { // If input has a value
        get_user_info('i', id_input.value);
        id_input.value = "Got the id";
    }
    else if (email_input.value.trim() != "") {
        email_input.value = "Get the email";
    }
    else if (name_input.value.trim() != "") {
        name_input.value = "get the name"
    }

    result_populate([1, 2, 3, 4, 5, 6, 7, 8, 9, 10]);
}

function result_populate(search_results) {
    const container = document.getElementById("result-table");
    container.innerHTML = "";   // Clear the previous results

    const num_results = search_results.length;
    for (let i = 0; i < num_results; i++) {
        let button = document.createElement("button");
        button.className = "result-button";

        // Create a table to show student information
        let table = document.createElement("table");
        let row = table.insertRow();

        // Create and populate table cells
        let cell_id = row.insertCell(0);
        let cell_name = row.insertCell(1);
        let cell_email = row.insertCell(2);

        cell_id.textContent = "901012345";
        cell_name.textContent = "First Last";
        cell_email.textContent = "first_last@mymail.eku.edu";

        button.appendChild(table);
        container.appendChild(button);   // Add the button to the result list
    }
}

// Function to fetch relevant user info from the provided information
// Flag: char, {i, e, n}
// Content: string, {user input}
async function get_user_info(flag, content) {
    console.log(`Begin user search with specification ${flag}:${content}`);

    // Load the api key and url
    api = await loadApiData("user");
    console.log("Getting API settings");

    // Generate the endpoint based on the user's inputs
    let endpoint = undefined;
    if (flag == 'i') {
        endpoint = `?employee_num=${content}`;
    }
    else if (flag == "e") {
        endpoint = `?email=${content}`;
    }
    else if (flag == 'n') {
        // Split content from 'first last' to 'first' and 'last'
        const names = content.split(" ");
        const first_name = names[0];
        const last_name = names[1];
        endpoingt = `?first_name=${first_name}&last_name=${last_name}`;
    }
    else {
        // Quit out of invalid flag was passed since no endpoint can be made
        console.log("Unknown flag passed to get_user_info()");
        return;
    }

    // Make the API fetch request
    const options = {
        method: 'GET',
        headers: {
            accept: 'application/json',
            Authorization: api.key
        }
    };

    fetch(api.url + endpoint, options)
        .then(res => res.json())
        .then(res => console.log(res))
        .catch(err => console.error(err));

}