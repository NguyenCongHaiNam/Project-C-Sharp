import re
from collections import Counter
from nltk.tokenize import sent_tokenize
from pyvi import ViTokenizer

def preprocess_text(text):
    sentences = text.split(".")  
    unique_sentences = list(set(sentences)) 
    cleaned_text = ". ".join(unique_sentences)  
    output = cleaned_text.lower()  
    output = output.replace('\n', '. ')     
    output = output.strip()  
    output = re.sub(r'([.!?])\1+', r'\1', output)  
    output = re.sub(r'[\"\'‘’“”`‛‟«»„‹›「」『』()〝〞〟〰]', '', output)  
    return output

def tokenize_sentences(text):
    return sent_tokenize(text)

def tokenize_meaningful_words(text):
    return ViTokenizer.tokenize(text)

def calculate_sentence_scores(sentences):
    word_frequency = Counter(' '.join(sentences).split())
    sentence_scores = {}
    for i, sentence in enumerate(sentences):
        tokenized_words = ViTokenizer.tokenize(sentence).split() 
        sentence_scores[i] = sum(word_frequency[word] for word in tokenized_words)
    return sentence_scores

def generate_summary(sentences, num_sentences):
    sentence_scores = calculate_sentence_scores(sentences)
    top_sentences = sorted(sentence_scores.keys(), key=lambda i: sentence_scores[i], reverse=True)[:num_sentences]
    top_sentences.sort()
    summary = ' '.join(sentences[i] for i in top_sentences)
    return summary

def postprocess_summary(summary):
    formatted_summary = summary.replace("_", " ")
    formatted_summary = re.sub(r'\s*([,\.])', r'\1', formatted_summary)
    return formatted_summary

def summarize_text(text, num_sentences):
    preprocessed_text = preprocess_text(text)
    tokenized_text = tokenize_meaningful_words(preprocessed_text)
    sentences = tokenize_sentences(tokenized_text)
    summary = generate_summary(sentences, num_sentences)
    processed_summary = postprocess_summary(summary)
    return processed_summary
