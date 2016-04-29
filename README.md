# Watson Visual Recognition ASP.NET Core Starter Application

  The IBM Watson [Visual Recognition][service_url] and [Alchemy Vision][alchemy_vision_url] services analyze the visual content of images to understand them without any input text describing the images.

Give it a try! Click the button below to fork into IBM DevOps Services and deploy your own copy of this application on Bluemix.

[![Deploy to Bluemix](https://bluemix.net/deploy/button.png)](https://bluemix.net/deploy)

## Getting Started

1. Create a Bluemix Account

  [Sign up][sign_up] in Bluemix, or use an existing account. Watson Services in Beta are free to use.

2. Download and install the [Cloud-foundry CLI][cloud_foundry] tool

3. Edit the `manifest.yml` file and change the `<application-name>` to something unique.
  ```none
applications:
- name: <application-name>
  memory: 256M
  services:
  - visual-recognition-service
  - alchemy-api
  ```
  The name you use will determine your application url initially, e.g. `<application-name>.mybluemix.net`.

4. Connect to Bluemix in the command line tool
  ```sh
  $ cf api https://api.ng.bluemix.net
  $ cf login -u <your user ID>
  ```

5a. Create the Visual Recognition service in Bluemix
  ```sh
  $ cf create-service visual_recognition free visual-recognition-service
  ```

5b. Create the AlchemyAPI service in Bluemix
  ```sh
  $ cf create-service alchemy_api free alchemy-api
  ```

6. Push it live!
  ```sh
  $ cf push
  ```

## Running the app locally
This sample app targets the ASP.Net Core and the .Net CoreCLR version 1.0.0-rc1-update1.

1. Copy the credentials from your `visual-recognition-service` and `alchemy-api` services in Bluemix to `src/VisualRecognition/config.json`, you can see the credentials using:

    ```sh
    $ cf env <application-name>
    ```
    Example output:
    ```sh
    System-Provided:
    {
    "VCAP_SERVICES": {
      "alchemy_api": [{
          "credentials": {
            "apikey": "<apikey>",
            "note": "It may take up to 5 minutes for this key to become active. This is your previously active free apikey. If you want a different one, please wait 24 hours after unbinding the key and try again.",
            "url": "<url>"
          },
        "label": "alchemy_api",
        "name": "alchemy-api",
        "plan": "free"
     }],
      "visual_recognition": [{
          "credentials": {
            "url": "<url>",
            "password": "<password>",
            "username": "<username>"
          },
        "label": "visual_recognition",
        "name": "visual-recognition-service",
        "plan": "free"
     }]
    }
    }
    ```

    You need to copy the value of `VCAP_SERVICES` to `src/VisualRecognition/config.json`.  An example config.json file is available in `examples/config.json`.

2. Install [ASP.Net Core](https://get.asp.net)
  Be sure to install and use the 1.0.0-rc1-update1 version of the CoreCLR runtime:
  ```sh
  dnvm install 1.0.0-rc1-update1 -r coreclr
  dnvm use 1.0.0-rc1-update1 -r coreclr
  ```

3.1 Run the project (Linux/Mac). Go to the project folder in a terminal and run:
  ```sh
  $ dnu restore
  $ dnu build
  $ dnx web
  ```

3.2 Run the project (Windows).
  Open the solution in Visual Studio 2015 and wait for NuGet to restore packages, then press F5 to start debugging.  Alternatively, you can run the project from a command-line as described in section 3.1.

4. Go to `http://localhost:5000`

## Decomposition Instructions

* See src/VisualRecognition/Startup.cs for how to obtain the Watson Visual Recognition credentials
* See src/WatsonServices/Services/VisualRecognitionService.cs for how to use the Watson Visual Recognition credentials and call the Watson Visual Recognition REST API
* See src/WatsonServices/Services/AlchemyVisionService.cs for how to use the Alchemy Vision credentials and call the AlchemyVision REST API
* To use the VisualRecognitionService or AlchemyVisionService in your own project, simply import the WatsonServices project into your solution and add a dependency for it to your project.json.

## Troubleshooting

The primary source of debugging information for your Bluemix app is the logs. To see them, run the following command using the Cloud Foundry CLI:

  ```
  $ cf logs <application-name> --recent
  ```
For more detailed information on troubleshooting your application, see the [Troubleshooting section](https://www.ng.bluemix.net/docs/troubleshoot/tr.html) in the Bluemix documentation.

## License

  This sample code is licensed under Apache 2.0. Full license text is available in [LICENSE](LICENSE).

## Contributing

  See [CONTRIBUTING](CONTRIBUTING.md).

## Open Source @ IBM
  Find more open source projects on the [IBM Github Page](http://ibm.github.io/)

### Useful links
[IBM Bluemix](https://bluemix.net/)  
[IBM Bluemix Documentation](https://www.ng.bluemix.net/docs/)  
[IBM Bluemix Developers Community](http://developer.ibm.com/bluemix)  
[IBM Watson Visual Recognition Overview](https://www.ibm.com/smarterplanet/us/en/ibmwatson/developercloud/doc/visual-recognition/overview.shtml)  
[IBM Watson Visual Recognition API Reference](https://www.ibm.com/smarterplanet/us/en/ibmwatson/developercloud/visual-recognition/api/v2/)  

[cloud_foundry]: https://github.com/cloudfoundry/cli
[service_url]: http://www.ibm.com/smarterplanet/us/en/ibmwatson/developercloud/visual-recognition.html
[alchemy_vision_url]: http://www.ibm.com/smarterplanet/us/en/ibmwatson/developercloud/alchemy-vision.html
[sign_up]: http://bluemix.net/
