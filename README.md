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
  - HttpClient is injected! Which allows you to control it's lifecycle
- Yuck, hard coded routes, these can lead to issues if my endpoint is still under development. 
  - Generated On Build! AspNetCore.Client.Generator is a before compile build task that will generate clients inside the project that implements it.
- How do I unit test this without spinning up a full web app? 
  - Works with Microsoft.AspNetCore.TestHost!
    - CancellationTokens are not respected inside the TestServer without some hacks though *(registering a kill of the server)* due to the Token not being checked.
- How do I tell my teammates that an endpoint has headers it requires? 
  - HeaderParameterAttribute! Which makes a header a parameter inside the generated methods, which may or may not be required.
  - IncludeHeaderAttribute! Which defines a constant header value to be included
- How do I tell my teammates that an endpoint has known response types for status codes?
  - ProducesResponseType! Generates action paramters that allow custom logic depending on the status code returned, without needing to manually check it.
- What if sometimes I want to intercept requests before they go out? 
  - IHttpOverride! Which allows for potential cache interception of requests.
- If I own the endpoint's code, why can't I just generate clients from it to make interacting with it as simple as injecting it?
  - Introducing AspNetCore.Client.Generator!

### Example Registration, AspNetCore

```c#

//Microsoft.Extensions.Http, allows for httpclient pooling, which doesn't fall into the trap of staticly creating clients which can be vulnerable to DNS changes.
services.AddHttpClient("MyApp");

//Injecting an HttpClient from the pooling handler.
//You don't need to use the Microsoft.Extensions.Http package, the Clients just expect an HttpClient to be able to be resolved via DI.
services.AddTransient<HttpClient>((provider) =>
{
	var factory = provider.GetService<IHttpClientFactory>();
	return factory.CreateClient();
});

//Tell the IServiceCollection to register the required services 
services.InstallClients(config =>
{
	//Tells the clients what to use as the base url
	config.HttpBaseAddress = Configuration.GetConnectionString("MyBackendApp");

	//What if you had a protobuf consuming server?
	//config.WithProtobufBody();
	//config.UseProtobufSerlaizer();
});

```

### Example Blazor Registration
HttpClient is injected by default inside blazor, so you only need to register the clients + a blazor json serializer unless you overrided the default one for something like protobuf

```c#

var serviceProvider = new BrowserServiceProvider(services =>
{
	services.InstallClients(config=>
	{
		config.UseBlazorSimpleJsonSerlaizer();
	});
});

```


### Example XYZ.Clients csproj
These values control the generator output

```xml

<PropertyGroup>
	<GenerateClients>true</GenerateClients>
	<RouteToServiceProjectFolder>../TestWebApp</RouteToServiceProjectFolder>
	<ClientInterfaceName>TestWebAppClient</ClientInterfaceName>
	<UseValueTask>true</UseValueTask>
	<ClientNamespace>TestWebApp.Clients</ClientNamespace>
	<AllowedNamespaces>$(AllowedNamespaces);TestWebApp.Contracts*;</AllowedNamespaces>
	<ExcludedNamespaces></ExcludedNamespaces>
</PropertyGroup>


```


## AspNetCore.Client
[![NuGet](https://img.shields.io/nuget/v/AspNetCore.Client.svg)](https://www.nuget.org/packages/AspNetCore.Client)

Package that contains required classes/attributes used by the generator package.

Can be included inside the web app to configure behavior.

## AspNetCore.Client.Protobuf
[![NuGet](https://img.shields.io/nuget/v/AspNetCore.Client.Protobuf.svg)](https://www.nuget.org/packages/AspNetCore.Client.Protobuf)

Contains a protobuf serializer which can override the default json one via the UseProtobufSerlaizer on the ClientConfiguration.

```c#
services.InstallClients(config=>
{
	config.UseProtobufSerlaizer();
	config.WithProtobufBody();
});

```

## AspNetCore.Client.BlazorJson
[![NuGet](https://img.shields.io/nuget/v/AspNetCore.Client.BlazorJson.svg)](https://www.nuget.org/packages/AspNetCore.Client.BlazorJson)

Contains a blazor simpleJson serializer which can override the default json one via the UseBlazorSimpleJsonSerlaizer on the ClientConfiguration.

```c#
services.InstallClients(config=>
{
	config.UseBlazorSimpleJsonSerlaizer();
});

```

## AspNetCore.Client.Generator
[![NuGet](https://img.shields.io/nuget/v/AspNetCore.Client.Generator.svg)](https://www.nuget.org/packages/AspNetCore.Client.Generator)

On Build generator that will generate a Clients.cs file based on the Properties in the csproj.


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
  - Use ValueTask`<T>` instead of Task`<T>`, only applies to tasks that have a return type.
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