# AspNetCore.Client
[![AppVeyor](https://ci.appveyor.com/api/projects/status/984mqqfnwytd3oga?svg=true)](https://ci.appveyor.com/project/Beffyman/aspnetcore-client)
---

If you are anything like me, you look at

```c#
using(var client = new HttpClient())
{
	var request = await client.client.GetAsync($"api/mysuperspecialroute/{id}");
	//Now to make sure what came back is what is expected, in every case...
}
```
and think the following
- Why not just inject clients?
  - services.InstallClients();!
- How can I pool the HttpClient usage? 
  - HttpClient is injected!
- Yuck, hard coded routes, these can lead to issues if my endpoint is still under development. 
  - Generated On Build!
- How do I unit test this without spinning up a full web app? 
  - Works with Microsoft.AspNetCore.TestHost!
    - CancellationTokens are not respected inside the TestServer without some hacks though.
- How do I tell my teammates that an endpoint has headers it requires? 
  - HeaderParameterAttribute! 
  - IncludeHeaderAttribute!
- How do I tell my teammates that an endpoint has known response types for status codes?
  - ProducesResponseType!
- What if sometimes I want to intercept requests before they go out? 
  - IHttpOverride!
- If I own the endpoint's code, why can't I just generate clients from it?
  - Introducing AspNetCore.Client.Generator!


## AspNetCore.Client.Core
[![NuGet](https://img.shields.io/nuget/dt/AspNetCore.Client.Core.svg)](https://www.nuget.org/packages/AspNetCore.Client.Core?semVer=2.0.0)

Package that contains required classes/attributes used by the generator package.

Can be included inside the web app to configure behavior.

## AspNetCore.Client.Core.Protobuf
[![NuGet](https://img.shields.io/nuget/dt/AspNetCore.Client.Core.Protobuf.svg)](https://www.nuget.org/packages/AspNetCore.Client.Core.Protobuf?semVer=2.0.0)

Contains a protobuf serializer which can override the default json one via the UseProtobufSerlaizer on the ClientConfiguration.

## AspNetCore.Client.Generator
[![NuGet](https://img.shields.io/nuget/dt/AspNetCore.Client.Generator.svg)](https://www.nuget.org/packages/AspNetCore.Client.Generator?semVer=2.0.0)

On Build generator that will generate a Clients.cs file based on the ClientGeneratorSettings.json file the generator creates.


#### Generator Properties Reference

- GenerateClients
  - Whether or not the generator should run on build.
  - Keeps the Clients.cs file from changing
- RouteToServiceProjectFolder
  - Path to the Asp.Net.Core website from the current project's folder
- ClientInterfaceName
  - Name used for the base interface of all clients generated
  - Name used for the wrapper class of the HttpClient
- [UseValueTask](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/async/async-return-types#generalized-async-return-types-and-valuetaskt)
  - Use ValueTask`<T>` instead of Task`<T>`
- ClientNamespace
  - Namespace to be used for the Clients.cs
- AllowedNamespaces
  - Separated by Colons
  - Namespaces allowed to be pulled from Controllers that the generator is pulling the data from.
    - ex) You have `using MyApp.Contracts;` inside the Controller, if MyApp.Contracts is inside the allowed namespaces, it would be copied into the Clients.cs
- ExcludedNamespaces
  - Separated by Colons
  - Namespaces that will NOT be pulled from Controllers
    - ex) You have `using MyApp.Internals;` inside the controller, if MyApp.Internals is inside the excluded namespaces, it will not be copied into the Clients.cs