# proxyvote
A sample app that demonstrates tech things through a scenario


## Learn more about ProxyVote

### Quick intro to Proxy Vote (French - 15min)
[![Watch the video](https://img.youtube.com/vi/FkfrlYn-uHw/maxresdefault.jpg)](https://youtu.be/FkfrlYn-uHw)

### End-to-end demo: a voting app (English - 1hr)
[![Watch the video](https://img.youtube.com/vi/FqaJh3AfRBM/maxresdefault.jpg)](https://youtu.be/FqaJh3AfRBM)

### Building Serverless Application (French - 2 hours)
[![Watch the video](https://img.youtube.com/vi/itrgHlq06fE/maxresdefault.jpg)](https://youtu.be/itrgHlq06fE)

## Develop locally

Please create a `local.settings.json` file under `src\citizen-back` with the following content: 

```json
{
  "Host": {
    "CORS": "*"
  }
}
```


## Deploy to Azure

```bash
az deployment sub create --location francecentral --template-file main.bicep
```
