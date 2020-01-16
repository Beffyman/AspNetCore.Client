# Beffyman.AspNetCore.Client
[![Build Status](https://dev.azure.com/beffyman/Beffyman.Github/_apis/build/status/Beffyman.AspNetCore.Client?branchName=master)](https://dev.azure.com/beffyman/Beffyman.Github/_build/latest?definitionId=7&branchName=master)
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
- AspNetCore 3.1 HTTP Controllers
- AspNetCore 3.1 SignalR Hubs
- Http Trigger Azure Functions v3

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

Requires version 1.7.3.7 at the moment due to https://github.com/dotnet/aspnetcore/issues/18074

```c#
services.AddTestWebClients(config=>
{
	config.UseMessagePackSerializer()
			.UseMessagePackDeserializer()
			.WithMessagePackBody();
});

```


## Beffyman.AspNetCore.Client.NewtonsoftJson
[![NuGet](https://img.shields.io/nuget/v/Beffyman.AspNetCore.Client.NewtonsoftJson.svg)](https://www.nuget.org/packages/Beffyman.AspNetCore.Client.NewtonsoftJson)

Contains a Newtonsoft Json serializer which can override the default json one via the UseNewtonsoftJsonHttpSerializer on the ClientConfiguration.

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
