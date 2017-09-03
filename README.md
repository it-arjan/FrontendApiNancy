# Data Api

This repository holds the data Api. Authenticated using OAUTH against identityserver3. 

To see how data Api fits in the overall system, see https://messagequeuefrontend.azurewebsites.net/systemlayout

# Most interesting

### /Bootstrapper.cs
- in the override of RequestStartup all incoming requests are sent to the WebSocket Api feed

### Modules folder
- Check out the easy way to define an api in NancyFx