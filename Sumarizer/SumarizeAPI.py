from flask import Flask, render_template, request, jsonify
from summarizer import summarize_text

app = Flask(__name__)

@app.route('/')
def index():
    return render_template('index.html')

@app.route('/summarize', methods=['POST'])
def summarize():
    data = request.json
    text = data.get('text')
    num_sentences = int(data.get('num_sentences', 3))  # Chuyển đổi thành số nguyên
    summary = summarize_text(text, num_sentences)
    return jsonify({'summary': summary})


if __name__ == '__main__':
    app.run(debug=True)

