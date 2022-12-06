# proxyvote
A sample app that demonstrates tech things through a scenario

> Note: This readme is under construction. Please ping me/create an issue/discussion if you need help :)

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