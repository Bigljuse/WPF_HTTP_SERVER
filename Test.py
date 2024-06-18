import http.client
import sys
import requests
import json
import base64

# get http server ip
#http_server = sys.argv[1]
# create a connection
#conn = http.client.HTTPConnection(http_server)
#with open("predictions.json", "r", encoding='utf8') as f:
#    json_data = json.load(f)

image_data = {}
with open('PredictionImagePNG.png', mode='rb', encoding=None) as file:
	image_file = file.read()
	image_data = base64.encodebytes(image_file)#.decode('utf-8')

headers = {"Type": "Prediction_Generation_Image"}
data = image_data

#response1 = requests.get('http://127.0.0.1:8888/', json='asdasdasd')
response_prediction = requests.post('http://127.0.0.1:8888/', headers=headers, data=data)


