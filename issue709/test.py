import httplib2

(response, content) = httplib2.Http().request('https://docs.googleapis.com/v1/documents/195j9eDD3ccgjQRttHhJPymLJUCOUjs-jmwTrekvdjFE?alt=json')
print(response.status)