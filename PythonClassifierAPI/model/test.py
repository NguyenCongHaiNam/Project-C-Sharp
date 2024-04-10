import pickle

# Đường dẫn đến mô hình đã lưu
MODEL_PATH = "D:\\PythonClassifierAPI\\model\\naive_bayes.pkl"

# Load mô hình từ tệp đã lưu
model = pickle.load(open(MODEL_PATH, 'rb'))

# Đọc dữ liệu từ tệp
file_path = "D:\\PythonClassifierAPI\\process.txt"
with open(file_path, 'r', encoding="utf-8") as file:
    new_texts = file.readlines()

# Loại bỏ khoảng trắng và dòng trống
new_texts = [text.strip() for text in new_texts if text.strip()]

# Dự đoán nhãn cho các văn bản mới
predicted_labels = model.predict(new_texts)

# In kết quả dự đoán
for text, label in zip(new_texts, predicted_labels):
    print(f"Văn bản: {text}")
    print(f"Nhãn dự đoán: {label}")
    print()
