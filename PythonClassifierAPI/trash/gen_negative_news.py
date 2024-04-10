import json
import os
import re

# Đường dẫn thư mục để lưu các tệp
output_directory = 'D:\\PythonClassifierAPI\\data_neg'

# Đảm bảo thư mục tồn tại
os.makedirs(output_directory, exist_ok=True)

def clean_json(text):
    text = text.replace('"', '\\"').replace("\n"," ").replace("\t"," ").replace("\r"," ")
    text = re.sub(r'\s+', ' ', text)
    return text

with open('D:\\PythonClassifierAPI\\negative_news.txt', 'r', encoding="utf-8") as file:
    news = file.read().split("\n\n")

for index, new in enumerate(news):
    # Tạo một từ điển template cho mỗi tin tức
    template = {
        'url': 'url',
        'error': '',
        'success': True,
        'title': 'article.title',
        'keywords': '',
        'published_date': "article.publish_date if article.publish_date else article.meta_data.get('pubdate', '')",
        'top_img': "article.top_image",
        'content': f'{clean_json(new)}'
    }
    
    # Tạo tên tệp cho mỗi tin tức
    filename = os.path.join(output_directory, f'output{index}n.txt')
    
    # Ghi từ điển template vào tệp JSON tương ứng
    with open(filename, 'w', encoding='utf-8') as output_file:
        json.dump(template, output_file, indent=4, ensure_ascii=False)
