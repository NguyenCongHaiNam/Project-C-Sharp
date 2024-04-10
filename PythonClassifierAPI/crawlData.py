import re
import json
import validators
from newspaper import Article
from bs4 import BeautifulSoup


def is_url(url):
    return validators.url(url)

def clean_json(text):
    text = text.replace('"', '\\"').replace("\n"," ").replace("\t"," ").replace("\r"," ")
    text = re.sub(r'\s+', ' ', text)
    return text

def crawl(url):
    if not is_url(url):
        result = {
            'url': url,
            'error': 'Url không hợp lệ!',
            'success': False
        }

        return result
    
    article = Article(url)
    article.download()
    article.parse()
    
    soup = BeautifulSoup(article.html, 'html.parser')
    content = soup.get_text(separator=' ')

    result = {
        'url': url,
        'error': '',
        'success': True,
        'title': clean_json(article.title),
        'keywords': ', '.join(article.keywords if article.keywords else (
            article.meta_keywords if article.meta_keywords else article.meta_data.get('keywords', []))),
        'published_date': article.publish_date if article.publish_date else article.meta_data.get('pubdate', ''),
        'top_img': article.top_image,
        'content': re.sub('\\n+', '</p><p>', '<p>' + clean_json(content) + '</p>')
    }
    return result


if __name__ == '__main__':
    URL_file = "D:\\PythonClassifierAPI\\positive_URL.txt"
    # URL_file = "D:\\PythonClassifierAPI\\negative_URL.txt"
    with open(URL_file, "r") as file:
        url = [line.strip() for line in file]
    for i in range(1,len(url)+1):
        try:
            print(f"{i}: {url[i]}")
            res = crawl(
                f'{url[i]}')
            # output_file_path = f"data_neg\\new\\output{i}n.txt"
            output_file_path = f"data\\output{i}.txt"
            with open(output_file_path, "w") as output_file:
                output_file.write(json.dumps(res))
        except:
            continue
    
    # #test
    # res = crawl(
    #             f'https://www.tapchicongsan.org.vn/web/guest/nghien-cu/-/2018/823613/ban-luan-ve-con-duong-di-len-chu-nghia-xa-hoi-o-viet-nam-hien-nay---khoa-hoc-va-niem-tin.aspx')
    # output_file_path = f"output_test.txt"
    # with open(output_file_path, "w") as output_file:
    #     output_file.write(json.dumps(res))