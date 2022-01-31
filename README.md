# Libraries and utils for Meadow F7 Micro


### Verify.* Apps

These are small apps used to verify various functions of the Meadow

#### [Wifi](src/Chibi.Verify.Https.MeadowF7v2)

This will do get requests to postman echo in a loop. Use `meadow listen` to read the output.

Add `appSettings.Local.yml` file to root of the `Verify.Wifi` project with your network credentials.

```yml
ssid: "YOUR_SSID"
password: "YOUR_PASSWORD"
```
