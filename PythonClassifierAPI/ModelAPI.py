from flask import Flask, request, jsonify, render_template
import requests
from newspaper import Article
import pickle
import cleanData
import crawlData

app = Flask(__name__)

# Load trained model
MODEL_PATH = "D:\\PythonClassifierAPI\\model\\naive_bayes.pkl"
model = pickle.load(open(MODEL_PATH, 'rb'))

def preprocess_text(text):
    text = cleanData.text_preprocess(text)
    return text


def extract_text_from_url(url):
    try:
        return crawlData.crawl(url)
    except Exception as e:
        return str(e)

@app.route('/', methods=['GET'])
def home():
    return render_template('index.html')
# API route for classification
@app.route('/classify', methods=['POST'])
def classify_text():
    # Get URL from request
    url = request.json.get('url')
    print(url)
    # Extract text content from URL
    text = extract_text_from_url(url)
    print(text)
    if not text:
        return jsonify({'error': 'Failed to extract text from URL'}), 400

    # Preprocess text
    preprocessed_text = preprocess_text(text['content'])
    # Predict label using the model
    predicted_label = model.predict([preprocessed_text])[0]
    if predicted_label == 1:
        result = 'Safe news'
    else : result  = 'Danger news'

    return jsonify({'url': url,'content':text, 'predicted_label': result}), 200

if __name__ == '__main__':
    app.run(debug=True)
