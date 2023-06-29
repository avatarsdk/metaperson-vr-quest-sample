# MetaPerson - VR Oculus Quest Sample
This sample demonstrates how to embed the [MetaPerson Creator](https://metaperson.avatarsdk.com/) into Oculus Quest VR applications and export an avatar from it.
There are two ways of the integration. 
1. Showing the [MetaPerson Creator](https://metaperson.avatarsdk.com/) in a Web View component.<br/>
There is no default Unity Web View control for Oculus Quest applications. So you should use any of the 3rd party solutions.
We recommend using a [Vuplex Web View](https://store.vuplex.com/webview/android-gecko) plugin that is paid.

2. Installing a [MetaPerson Creator android application](TBD)<br/>
This application has similar features to the [MetaPerson Creator](https://metaperson.avatarsdk.com/) and can be run from your application instead of showing a web page.
The application is not available in the Oculus Quest Store for now and is destributed as an APK file. So this way best suits for evaluation and testing purposes while you didn't implement a solution with a Web View.

## Getting Started.
You need to complete the following steps before experimenting with this sample:

* Get an AvatarSDK developer account at https://accounts.avatarsdk.com/developer/signup/
* Create an application with Client credentials Authorization Grant at https://accounts.avatarsdk.com/developer/
* Copy `App Client ID` and `App Client Secret` from the Client Access application at https://accounts.avatarsdk.com/developer/
![App Client Credentials](./Documentation/Images/credentials.png "App Client Credentials")

Now you are ready to go:
* Clone this repository to your computer