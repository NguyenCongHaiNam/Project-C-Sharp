document.getElementById('modelForm').addEventListener('submit', function(e) {
    e.preventDefault();
    
    let formData = new FormData();
    formData.append('modelName', document.getElementById('modelName').value);
    formData.append('alpha', document.getElementById('alpha').value);
    formData.append('fitPrior', document.getElementById('fitPrior').checked);
    formData.append('trainData', document.getElementById('trainData').files[0]);
    
    fetch('/train-model', {
        method: 'POST',
        body: formData
    })
    .then(response => response.json())
    .then(data => {
        document.getElementById('trainResult').innerText = `Model trained successfully. Accuracy: ${data.accuracy}`;
        loadModels(); // Refresh the model list
    })
    .catch(error => console.error('Error:', error));
});

document.getElementById('predictForm').addEventListener('submit', function(e) {
    e.preventDefault();

    let text = document.getElementById('text').value;
    let modelSelect = document.getElementById('modelSelect');
    let model_name = modelSelect.options[modelSelect.selectedIndex].value;
    
    fetch('/predict', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({ model_name: model_name, text: text })
    })
    .then(response => response.json())
    .then(data => {
        document.getElementById('predictResult').innerText = `Prediction: ${data.prediction}`;
    })
    .catch(error => console.error('Error:', error));
});

function loadModels() {
    fetch('/models')
    .then(response => response.json())
    .then(data => {
        let modelSelect = document.getElementById('modelSelect');
        modelSelect.innerHTML = ''; // Clear the current options
        data.models.forEach(function(model) {
            let option = document.createElement('option');
            option.value = model;
            option.text = model;
            modelSelect.add(option);
        });
    })
    .catch(error => console.error('Error:', error));
}

// Load models on page load
loadModels();
