import re
import json
import validators
from newspaper import Article as BaiBao
from bs4 import BeautifulSoup
from todate import *
from model_detectfakenews import detect_content,check_blacklist
from tomtatvanban import summarize_text


def is_url(url):
    return validators.url(url)

def clean_json(text):
    text = text.replace('"', '\\"').replace("\n"," ").replace("\t"," ").replace("\r"," ")
    text = re.sub(r'\s+', ' ', text)
    return text




# Function to extract the domain from a URL
def getDomain(url):
    split_url = url.split('/')
    return split_url[2]



import requests
from bs4 import BeautifulSoup
from fake_useragent import UserAgent

def crawler_viettan(url):
    ua = UserAgent()
    headers = {
        'User-Agent': ua.random,
        'Accept-Language': 'en-US,en;q=0.9',
        'Accept-Encoding': 'gzip, deflate, br',
        'Connection': 'keep-alive'
    }
    if("amp" not in url):
        url += "/amp"
        
    # url= "https://viettan.org/thu-di-tim-duong-cuu-nuoc/"
    while(True):
        try:
            response = requests.get(url, headers=headers)
            # print(response.status_code)
            code = response.status_code
            if(str(code) == "200"):
                break
            else: break
        except :
            pass

    soup = BeautifulSoup(response.content, "html.parser")
    
        
    # with open("output.html", "w", encoding="utf-8") as f:
    #     f.write(str(soup))

    title = soup.find("h1", class_="amp-wp-title").text
    author = soup.find("div",class_="author").text.strip()
    art_time = soup.find("meta",{'property': 'article:published_time'})['content'].strip()
    image = soup.find("amp-img",class_="attachment-large")['src']
    content = soup.find("div",class_="amp-wp-article-content").text.strip()

            
    result = {
                'url': url,
                'title': title,
                'keywords': "",
                'author': author,
                'published_date': todatetime(art_time),
                'top_img': image,
                'content': content,
                'summerize': summarize_text(content,3)
            }
    return result


def start_crawl(url):
    print("here")
    domain = getDomain(url)
    
    if not is_url(url):
        result = {
            'url': url,
            'error': 'Url không hợp lệ!',
            'success': False
        }

        return result

    result = {}

    print("detect bai viet thuong ", url)
    article = BaiBao(url)
    article.download()
    article.parse()        
    soup = BeautifulSoup(article.html, 'html.parser')
    content = soup.get_text(separator=' ')
    print("access 200")
    
    if(content == ""):
        content = re.sub('\\n+', '</p><p>', '<p>' + clean_json(content) + '</p>')

    # tom tat van ban 
    content_sum = summarize_text(content,1)

    # lay thoi gian dang bai
    pub_date = article.publish_date if article.publish_date else article.meta_data.get('pubdate', '')
    pub_date_str = str(pub_date) if isinstance(pub_date, datetime) else pub_date
    pub_date_parsed = todatetime(pub_date_str)
    if(pub_date_parsed is None):
        try:
            pub_date_parsed = extract_time(url)
        except:
            if(pub_date_parsed is None):
                pub_date_parsed = datetime.now()
                print("laytam now", pub_date_parsed)
            
    result = {
                'url': url,
                'error': '',
                'success': True,
                'title': article.title,
                'keywords': article.keywords if article.keywords else (
                    article.meta_keywords if article.meta_keywords else article.meta_data.get('keywords', [])),
                'published_date': pub_date,
                'top_img': article.top_image,
                'content': content,
                'summerize' : content_sum
            }
    
    result_isfake = detect_content(result["content"])
    
    if result_isfake:
        result['predicted_label'] = "Danger news"
    else:
        result['predicted_label'] = "Safe news"
    
    return result

    
    



def crawl_bbcnews(url):
    ua = UserAgent()

    while(True):
        try:
            headers = {
                'User-Agent': ua.random,
                'Accept-Language': 'en-US,en;q=0.9',
                'Accept-Encoding': 'gzip, deflate, br',
                'Connection': 'keep-alive'
            }
            response = requests.get(url, headers=headers)
            print(response.status_code)
            code = response.status_code
            if(str(code) == "200"):
                break
        except :
            pass


    soup = BeautifulSoup(response.content, "html.parser")
    with open("output.html", "w", encoding="utf-8") as f:
            f.write(str(soup))

    title = soup.find("h1", class_ = "bbc-e0ctyc e1p3vdyi0").text
    pub_time = soup.find("div", class_ = "bbc-19j92fr ebmt73l0").find("time")['datetime']
    tag_p = soup.find_all("p", class_ = "bbc-1y32vyc e17g058b0")
    top_img = soup.find("div", class_ = "bbc-j1srjl").find("img")['src']
    # author  = soup.find("span", class_ = "bbc-18ttg5u").text

    content = ""
    for pitem in tag_p:
        content += pitem.text

    data = {
        "title" : title,
        "url" : url,
        "image_url" : top_img,
        "author" : "phandong",
        "category_id": 1,
        "created_at": todatetime(pub_time),
        "content" : content,
        "summerize" : summarize_text(content,3),
        "is_fake" : True
    }

    return data

def crawl_vnexpress(url):
    article = BaiBao(url)
    article.download()
    article.parse()

    content = article.text
    headline = article.title
    url = article.url
    top_img = article.top_img
    author = article.authors

    data = {
        "title" : headline,
        "url" : url,
        "image_url" : top_img,
        "author" : "vnexpress",
        "content" : content,
        "category_id": 1,
        "created_at": extract_time(url),
        "summerize" : summarize_text(content,3),
        "is_fake" : True
    }
    print(data)
    
    return data


def crawl_unoffical():
    ua = UserAgent()
    headers = {
        'User-Agent': ua.random,
        'Accept-Language': 'en-US,en;q=0.9',
        'Accept-Encoding': 'gzip, deflate, br',
        'Connection': 'keep-alive'
    }

    url= "https://www.bbc.com/vietnamese/topics/ckdxnx1x5rnt?page="
    data = []
    count = 0
    for i in range(1,4):
        
        tempurl = url+ str(i)
        print("page = ", tempurl)
        while(True):
            try:
                response = requests.get(tempurl, headers=headers)
                print(response.status_code)
                code = response.status_code
                if(str(code) == "200"):
                    break
            except :
                pass
        content = response.content
        soup = BeautifulSoup(content, "html.parser")
        with open("output.html", "w", encoding="utf-8") as f:
            f.write(str(soup))

        all_art = soup.find_all("li", class_="bbc-t44f9r")
        
        for item in all_art:
            item_t = item.find("a")
            title = item_t.text
            url_item = item_t['href']
            if("articles" in url_item):
                print("Đang cào ", title, url_item)
                temp_data  = crawl_bbcnews(url_item)
                data.append(temp_data)
                print(count)
                count += 1
                
                # if(count > 10):
                #     break
    
    return data

    

def crawl_offical():
    ua = UserAgent()
    headers = {
        'User-Agent': ua.random,
        'Accept-Language': 'en-US,en;q=0.9',
        'Accept-Encoding': 'gzip, deflate, br',
        'Connection': 'keep-alive'
    }

    data = []
    url = "https://vnexpress.net/thoi-su/chinh-tri-p"

    count = 0
    for i in range(1,4):
        tempurl = url + str(i)
        print(f"---------------------------------Page {i}")
        
        while(True):
            try:
                response = requests.get(tempurl, headers=headers)
                code = response.status_code
                print(tempurl, code)
                if(str(code) == "200"):
                    break
            except :
                pass


        content = response.content

        soup = BeautifulSoup(content, "html.parser")

        all_art = soup.find_all("article", class_="item-news thumb-left item-news-common")
        
        for item in all_art:
            class_title = item.find("h2", class_="title-news")
            title = class_title.text
            url_item = class_title.find("a")['href']
            print("Đang cào ", title, url_item)
        
            temp_data  = crawl_vnexpress(url_item)
            data.append(temp_data)
            print(count)
            count += 1
        
        if(len(data) > 0):
            result = data
        else:
            result = None
    return result
