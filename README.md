# AspNetCore.Client
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
  - Generated On Build! AspNetCore.Client.Generator is a before compile build task that will generate clients inside the project that implements it.
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
  - Introducing AspNetCore.Client.Generator!

[First Time Setup](https://github.com/Beffyman/AspNetCore.Client/wiki/First-Time-Setup)

## AspNetCore.Client
[![NuGet](https://img.shields.io/nuget/v/AspNetCore.Client.svg)](https://www.nuget.org/packages/AspNetCore.Client)

Package that contains required classes/attributes used by the generator package.

Can be included inside the web app to configure behavior.

## AspNetCore.Client.Protobuf
[![NuGet](https://img.shields.io/nuget/v/AspNetCore.Client.Protobuf.svg)](https://www.nuget.org/packages/AspNetCore.Client.Protobuf)

Contains a protobuf serializer which can override the default json one via the UseProtobufSerlaizer on the ClientConfiguration.

```c#
services.AddTestWebClients(config=>
{
	config.UseProtobufSerializer()
			.WithProtobufBody();
});

```

## AspNetCore.Client.BlazorJson
[![NuGet](https://img.shields.io/nuget/v/AspNetCore.Client.BlazorJson.svg)](https://www.nuget.org/packages/AspNetCore.Client.BlazorJson)

Contains a blazor simpleJson serializer which can override the default json one via the UseBlazorSimpleJsonSerlaizer on the ClientConfiguration.

```c#
services.AddTestBlazorClients(config=>
{
	config.UseBlazorSimpleJsonSerlaizer()
            .UseExistingHttpClient();
});

```

## AspNetCore.Client.Generator
[![NuGet](https://img.shields.io/nuget/v/AspNetCore.Client.Generator.svg)](https://www.nuget.org/packages/AspNetCore.Client.Generator)

On Build generator that will generate a Clients.cs file based on the Properties in the csproj.
