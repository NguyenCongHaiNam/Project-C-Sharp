from flask import Flask, request, jsonify, render_template
from crawlData import start_crawl, crawl_unoffical, crawl_offical
from tomtatvanban import summarize_text

app = Flask(__name__)

@app.route('/', methods=['GET'])
def home():
    return render_template('Index.html')

@app.route('/api/crawler', methods=['GET', 'POST'])
def crawler():
    if request.method == 'GET':
        # Xử lý yêu cầu GET nếu cần
        return render_template('trangcaodulieu.html')

    listNews = []
    listurl = ""
    if request.method == 'POST':
        data = request.json
        listurl = data.get("listUrl", "")
        if listurl:
            listurl = str(listurl).split("\n")
            for url in listurl:
                data = start_crawl(url.strip())
                listNews.append(data)
    return jsonify(listNews=listNews, predata=listurl)


@app.route('/api/summerize', methods=['GET', 'POST'])
def summerize():
    if request.method == 'GET':
        # Xử lý yêu cầu GET nếu cần
        return render_template('thongtintomtat.html')
    text = request.json.get('text')
    num_sentences = int(request.json.get('num_sentences', 3))
    summary = summarize_text(text, num_sentences)
    return jsonify({'summary': summary})


@app.route('/api/detectnews', methods=['GET', 'POST'])
def detectnews():
    if request.method == 'GET':
        # Xử lý yêu cầu GET nếu cần
        return render_template('thongtindetect.html')

    data = {}
    if request.method == 'POST':
        data = request.json
        url = data.get("url", "")
        if url:
            data = start_crawl(url)
    return jsonify(data=data)


@app.route('/api/cluster_news', methods=['GET', 'POST'])
def cluster_news():
    if request.method == 'GET':
        # Xử lý yêu cầu GET nếu cần
        return render_template('ketquaphancum.html')

    result = crawl_offical()
    return jsonify(result=result)


if __name__ == '__main__':
    app.run(debug=True, port=5001)
