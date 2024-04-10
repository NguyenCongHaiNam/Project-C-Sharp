import os
import json
import pyodbc

# Kết nối đến cơ sở dữ liệu MSSQL
conn = pyodbc.connect(
    'Driver={SQL Server};'
    'Server=H1N4M\\MSSQLSERVER01;'
    'Database=NewsClassifier;'
    'UID=h1n4m;'
    'PWD=h1n4m;'
)

# Tạo con trỏ
cursor = conn.cursor()

# Đường dẫn đến thư mục chứa các tệp tin JSON
directory_path = 'D:\\PythonClassifierAPI\\data_neg'

# Duyệt qua từng tệp tin trong thư mục
for filename in os.listdir(directory_path):
    if filename.endswith('.txt'):
        # Đọc nội dung từ tệp tin JSON
        with open(os.path.join(directory_path, filename), 'r', encoding='utf-8') as file:
            json_data = file.read()

        # Parse JSON
        data = json.loads(json_data)
        # Extract values
        url = data.get('url', '')
        title = data.get('title', '')
        keywords = data.get('keywords', '')
        published_date = data.get('published_date', '')
        top_img = data.get('top_img', '')
        content = data.get('content', '')

        # SQL statement
        sql = """
        INSERT INTO raw (url, title, keywords, published_date, top_img, content)
        VALUES (?, ?, ?, ?, ?, ?)
        """

        # Thực thi truy vấn SQL
        cursor.execute(sql, (url, title, keywords, published_date, top_img, content))

        # Lưu thay đổi
        conn.commit()

# Đóng con trỏ và kết nối
cursor.close()
conn.close()
