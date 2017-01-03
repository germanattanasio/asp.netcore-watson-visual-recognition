# Watson Visual Recognition ASP.NET Core Starter Application

  The IBM Watson [Visual Recognition][service_url] service analyzes the visual content of images to understand them without any input text describing the images.

Give it a try! Click the button below to fork this project into IBM DevOps Services and deploy it on Bluemix.

[![Deploy to Bluemix](https://bluemix.net/deploy/button.png)](https://bluemix.net/deploy)

## Getting Started

1. Create a Bluemix Account

  [Sign up][sign_up] in Bluemix, or use an existing account.

2. Download and install the [Cloud-foundry CLI][cloud_foundry] tool

3. Edit the `manifest.yml` file and change the `<application-name>` to something unique.

  ```yml
applications:
- name: <application-name>
  memory: 512M
  services:
  - visual-recognition-service
  ```
  The name you use will determine your application url initially, e.g. `<application-name>.mybluemix.net`.

4. Connect to Bluemix using the command line interface

  ```sh
  cf login -a https://api.ng.bluemix.net -u <your user ID>
  ```

5. Create the Visual Recognition service in Bluemix

  ```sh
  cf create-service watson_vision_combined free visual-recognition-service
  ```

6. Push it live!

  ```sh
  cf push
  ```

## Running the app locally

This sample app targets the ASP.NET Core and the .NET CoreCLR version 1.0.0 using .NET CLI version 1.0.0-preview2-003121.

1. Copy the credentials from your `watson_vision_combined` service in Bluemix to `src/VisualRecognition/vcap_services.json`, you can see the credentials using:

    ```sh
    cf env <application-name>
    ```

    Example output:

    ```json
    System-Provided:
    {
    "VCAP_SERVICES": {
      "watson_vision_combined": [{
          "credentials": {
            "apikey": "<apikey>",
            "note": "It may take up to 5 minutes for this key to become active. This is your previously active free apikey. If you want a different one, please wait 24 hours after unbinding the key and try again.",
            "url": "<url>"
          },
        "label": "watson_vision_combined",
        "name": "visual-recognition-service",
        "plan": "free"
     }]
    }
    }
    ```

    You need to copy the value of `VCAP_SERVICES` to `src/VisualRecognition/vcap_services.json`.  An example vcap_services.json file is available in `examples/vcap_services.json`.

2. Install [.NET Core](https://www.microsoft.com/net/core)
  Be sure to install the 1.0.0-preview1-002702 version of the .NET CLI.

3. Run the project

  3.1. **(Linux/Mac)**. Go to the solution folder in a terminal and run:

  ```sh
  dotnet restore
  dotnet run -p src/VisualRecognition
  ```

  3.2. **(Windows)**.
  Open the solution in Visual Studio 2015 and wait for NuGet to restore packages, then press F5 to start debugging.  Alternatively, you can run the project from a command-line as described in section 3.1.

4. Go to `http://localhost:5000`

## Decomposition Instructions

* See src/VisualRecognition/Startup.cs for how to obtain the Watson Visual Recognition credentials
* See src/WatsonServices/Services/VisualRecognitionService.cs for how to use the Watson Visual Recognition credentials and call the Watson Visual Recognition REST API
* To use the VisualRecognitionService in your own project, simply import the WatsonServices project into your solution and add a dependency for it to your project.json.

## Troubleshooting

The primary source of debugging information for your Bluemix app is the logs. To see them, run the following command using the Cloud Foundry CLI:

  ```sh
  cf logs <application-name> --recent
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
[IBM Watson Visual Recognition API Reference](https://www.ibm.com/smarterplanet/us/en/ibmwatson/developercloud/visual-recognition/api/v3/)  

[cloud_foundry]: https://github.com/cloudfoundry/cli
[service_url]: https://www.ibm.com/smarterplanet/us/en/ibmwatson/developercloud/doc/visual-recognition/
[sign_up]: http://bluemix.net/
