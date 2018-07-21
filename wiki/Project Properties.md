# Project Properties

Example:

TestWebApp.Clients.csproj
``` xml
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


## GenerateClients
  - Whether or not the generator should run on build.
  - Keeps the Clients.cs file from changing due to changes to the ServiceProject


## RouteToServiceProjectFolder
  - Path to the Asp.Net.Core project folder from the clients project's folder

- MyService
  - MyService.csproj
- MyService.Clients
  - MyService.Clients.csproj

RouteToServiceProjectFolder would be ../MyService in this case

## ClientInterfaceName
  - Name used for the base interface of all clients generated
  - Name used for the wrapper class of the HttpClient
  - Not important unless you are using the interface for reflection

``` c#
	public class TestWebAppClient

	public interface ITestWebAppClient : IClient


	public interface IValuesClient : ITestWebAppClient

	public class ValuesClient : IValuesClient
	{
		public readonly TestWebAppClient Client;
		public readonly IHttpOverride HttpOverride;
		public readonly IHttpSerializer Serializer;
		public readonly IRequestModifier Modifier;

		public ValuesClient(TestWebAppClient client, IHttpOverride httpOverride, IHttpSerializer serializer, IRequestModifier modifier)
		{
			Client = client;
			HttpOverride = httpOverride;
			Serializer = serializer;
			Modifier = modifier;
		}
```


## [UseValueTask](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/async/async-return-types#generalized-async-return-types-and-valuetaskt)
  - Use ValueTask`<T>` instead of Task`<T>`, only applies to tasks that have a return type.

``` c#
ValueTask<string> GetAsync(int id,
	Action<HttpResponseMessage> ResponseCallback = null, 
	CancellationToken cancellationToken = default(CancellationToken));
```
vs 
``` c#
Task<string> GetAsync(int id,
	Action<HttpResponseMessage> ResponseCallback = null, 
	CancellationToken cancellationToken = default(CancellationToken));
```

## ClientNamespace
  - Namespace to be used for the Clients.cs


## AllowedNamespaces
  - Separated by Colons
  - Namespaces allowed to be pulled from Controllers that the generator is pulling the data from.
  - Allows wildcards


The following allows for `TestWebApp.Contracts*` to be included inside the Clients.cs
``` xml
<AllowedNamespaces>$(AllowedNamespaces);TestWebApp.Contracts*;</AllowedNamespaces>
```


## ExcludedNamespaces
  - Separated by Colons
  - Namespaces that will NOT be pulled from Controllers
  - Allows wildcards


The following removes `TestWebApp.Internal*` from the Clients.cs
``` xml
<ExcludedNamespaces>TestWebApp.Internal*;</ExcludedNamespaces>
```
