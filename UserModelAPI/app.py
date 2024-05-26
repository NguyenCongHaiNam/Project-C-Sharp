from flask import Flask, request, jsonify, render_template
import pandas as pd
from sklearn.model_selection import train_test_split
from sklearn.feature_extraction.text import CountVectorizer
from sklearn.naive_bayes import MultinomialNB
from sklearn.metrics import accuracy_score
from flask_sqlalchemy import SQLAlchemy
import io
import pickle
import os

app = Flask(__name__)
MODEL_DIR = 'models'
app.config['SQLALCHEMY_DATABASE_URI'] = 'mssql+pyodbc://h1n4m:h1n4m@H1N4M\\MSSQLSERVER01/NewsClassifier?driver=ODBC+Driver+17+for+SQL+Server'
db = SQLAlchemy(app)

class Model(db.Model):
    __tablename__ = 'UserModels'
    ID = db.Column(db.Integer, primary_key=True)
    idUser = db.Column(db.Integer)
    ModelName = db.Column(db.String(255))
    ModelPath = db.Column(db.String(255))
    accuracy = db.Column(db.Float)

    def __init__(self, idUser, ModelName, ModelPath, accuracy):
        self.idUser = idUser
        self.ModelName = ModelName
        self.ModelPath = ModelPath
        self.accuracy = accuracy

@app.route('/')
def index():
    return render_template('index.html')

@app.route('/train-model', methods=['POST'])
def train_model():
    alpha = float(request.form['alpha'])
    fit_prior = request.form['fitPrior'] == 'true'
    model_name = request.form['modelName']
    user_id = request.form.get('userId')
    if not model_name:
        return jsonify({'error': 'Model name is required'}), 400

    file = request.files['trainData']
    if not file:
        return jsonify({'error': 'No file uploaded'}), 400

    data = pd.read_csv(io.StringIO(file.stream.read().decode('utf-8')))

    if 'text' not in data.columns or 'label' not in data.columns:
        return jsonify({'error': 'CSV must contain text and label columns'}), 400

    X_train, X_test, y_train, y_test = train_test_split(data['text'], data['label'], test_size=0.2, random_state=42)

    vectorizer = CountVectorizer()
    X_train_vec = vectorizer.fit_transform(X_train)
    X_test_vec = vectorizer.transform(X_test)

    model = MultinomialNB(alpha=alpha, fit_prior=fit_prior)
    model.fit(X_train_vec, y_train)

    predictions = model.predict(X_test_vec)
    accuracy = accuracy_score(y_test, predictions)

    model_path = os.path.join(MODEL_DIR, model_name)
    if not os.path.exists(model_path):
        os.makedirs(model_path)

    with open(os.path.join(model_path, 'model.pkl'), 'wb') as model_file:
        pickle.dump(model, model_file)
    with open(os.path.join(model_path, 'vectorizer.pkl'), 'wb') as vectorizer_file:
        pickle.dump(vectorizer, vectorizer_file)

    new_model = Model(idUser=user_id, ModelName=model_name, ModelPath=model_path, accuracy=accuracy)
    db.session.add(new_model)
    db.session.commit()

    return jsonify({'accuracy': accuracy, 'user_id': new_model.idUser})

@app.route('/models', methods=['GET'])
def list_models():
    user_id = request.args.get('user_id')
    if not user_id:
        return jsonify({'error': 'User ID is required'}), 400

    models = Model.query.filter_by(idUser=user_id).all()
    model_data = [{'id': model.ID, 'idUser': model.idUser, 'model_name': model.ModelName, 'accuracy': model.accuracy} for model in models]
    return jsonify({'models': model_data})


@app.route('/predict', methods=['POST'])
def predict():
    model_name = request.json.get('model_name')
    text = request.json.get('text')
    user_id = request.json.get('user_id')

    model = Model.query.filter_by(idUser=user_id, ModelName=model_name).first()
    if not model:
        return jsonify({'error': 'Model not found for this user'}), 404

    model_path = model.ModelPath
    with open(os.path.join(model_path, 'model.pkl'), 'rb') as model_file:
        loaded_model = pickle.load(model_file)
    with open(os.path.join(model_path, 'vectorizer.pkl'), 'rb') as vectorizer_file:
        vectorizer = pickle.load(vectorizer_file)

    text_vec = vectorizer.transform([text])
    prediction = loaded_model.predict(text_vec)

    return jsonify({'prediction': prediction[0]})

if __name__ == '__main__':
    if not os.path.exists(MODEL_DIR):
        os.makedirs(MODEL_DIR)
    app.run(debug=True)
