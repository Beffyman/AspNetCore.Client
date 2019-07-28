# Beffyman.AspNetCore.Client
[![AppVeyor](https://ci.appveyor.com/api/projects/status/984mqqfnwytd3oga?svg=true)](https://ci.appveyor.com/project/Beffyman/aspnetcore-client)
---

If you are anything like me, you look at

```c#
using(var client = new HttpClient())
{
	var request = await client.GetAsync($"api/mysuperspecialroute/{id}");
	//Now to make sure what came back is what is expected, in every case...
}
```
and think the following
- Why not just inject clients?
  - services.AddTestWebClients();!
- How can I pool the HttpClient usage? 
  - HttpClient is injected! Which allows you to control it's lifecycle
- Yuck, hard coded routes, these can lead to issues if my endpoint is still under development. 
  - Generated On Build! Beffyman.AspNetCore.Client.Generator is a before compile build task that will generate clients inside the project that implements it.
- How do I unit test this without spinning up a full web app? 
  - Works with Microsoft.AspNetCore.TestHost!
    - CancellationTokens are not respected inside the TestServer without some hacks though *(registering a kill of the server)* due to the Token not being checked.
- How do I tell my teammates that an endpoint has headers it requires? 
  - HeaderParameterAttribute! Which makes a header a parameter inside the generated methods, which may or may not be required.
  - IncludeHeaderAttribute! Which defines a constant header value to be included
- How do I tell my teammates that an endpoint has known response types for status codes?
  - ProducesResponseType! Generates action parameters that allow custom logic depending on the status code returned, without needing to manually check it.
- What if sometimes I want to intercept requests before they go out? 
  - IHttpOverride! Which allows for potential cache interception of requests.
- If I own the endpoint's code, why can't I just generate clients from it to make interacting with it as simple as injecting it?
  - Introducing Beffyman.AspNetCore.Client.Generator!

[First Time Setup](https://github.com/Beffyman/AspNetCore.Client/wiki/First-Time-Setup)

## Supported Frameworks
- AspNetCore 2.2 HTTP Controllers
- AspNetCore 2.2 SignalR Hubs
- Http Trigger Azure Functions v2

## Beffyman.AspNetCore.Client
[![NuGet](https://img.shields.io/nuget/v/Beffyman.AspNetCore.Client.svg)](https://www.nuget.org/packages/Beffyman.AspNetCore.Client)

Includes ServiceCollection registration logic, used on the Client

## Beffyman.AspNetCore.Server
[![NuGet](https://img.shields.io/nuget/v/Beffyman.AspNetCore.Server.svg)](https://www.nuget.org/packages/Beffyman.AspNetCore.Server)

Includes attributes that can affect generation, used on your AspNetCore api app

## Beffyman.AspNetCore.Client.Protobuf
[![NuGet](https://img.shields.io/nuget/v/Beffyman.AspNetCore.Client.Protobuf.svg)](https://www.nuget.org/packages/Beffyman.AspNetCore.Client.Protobuf)

Contains a protobuf serializer which can override the default json one via the UseProtobufSerlaizer on the ClientConfiguration.

```c#
services.AddTestWebClients(config=>
{
	config.UseProtobufSerializer()
			.UseProtobufDeserializer()
			.WithProtobufBody();
});

```


## Beffyman.AspNetCore.Client.MessagePack
[![NuGet](https://img.shields.io/nuget/v/Beffyman.AspNetCore.Client.MessagePack.svg)](https://www.nuget.org/packages/Beffyman.AspNetCore.Client.MessagePack)

Contains a MessagePack serializer which can override the default json one via the UseMessagePackSerializer on the ClientConfiguration.

```c#
services.AddTestWebClients(config=>
{
	config.UseMessagePackSerializer()
			.UseMessagePackDeserializer()
			.WithMessagePackBody();
});

```


## Beffyman.AspNetCore.Client.BlazorJson
[![NuGet](https://img.shields.io/nuget/v/Beffyman.AspNetCore.Client.BlazorJson.svg)](https://www.nuget.org/packages/Beffyman.AspNetCore.Client.BlazorJson)

(Pre-3.0 Blazor, 3.0 Blazor can use the default Json Serializer)

Contains a blazor simpleJson serializer which can override the default json one via the UseBlazorSimpleJsonSerlaizer on the ClientConfiguration.

```c#
services.AddTestBlazorClients(config=>
{
	config.UseBlazorSimpleJsonSerlaizer()
			.UseBlazorSimpleJsonDeserlaizer()
			.WithJsonBody()
			.UseExistingHttpClient();
});

```


## Beffyman.AspNetCore.Client.JSInterop
[![NuGet](https://img.shields.io/nuget/v/Beffyman.AspNetCore.Client.JSInterop.svg)](https://www.nuget.org/packages/Beffyman.AspNetCore.Client.JSInterop)

Contains a preview js interop simpleJson serializer which can override the default json one via the UseJSInteropJsonSerializer on the ClientConfiguration.

```c#
services.AddTestRazorComponentsClients(config=>
{
	config.UseJSInteropJsonSerializer()
			.UseJSInteropJsonDeserializer()
			.WithJsonBody();
});

```


## Beffyman.AspNetCore.Client.Generator
[![NuGet](https://img.shields.io/nuget/v/Beffyman.AspNetCore.Client.Generator.svg)](https://www.nuget.org/packages/Beffyman.AspNetCore.Client.Generator)

On Build generator that will generate a Clients.cs file based on the Properties in the csproj.
