# First Time Setup
(This guide assumes you already have an AspNetCore WebApi project that you want to generate clients for)

Create a .Net Standard library project inside the same solution as your AspNetCore project named like so: [ServerProjectName].Client  (not enforced).


Add AspNetCore.Client.Generator to the clients project

(version may be different, please use the latest version)

``` xml
<PackageReference Include="AspNetCore.Client.Generator" Version="0.5.11" />
```

You may also control the generator's output by using the following properties

[TEMP](Add link to property details wiki)

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

Make sure that RouteToServiceProjectFolder is a relative route from the clients project into the same folder where the server's csproj is located.

Once you build your client project, you should have a Clients.cs file inside of your clients project.

The generator will look into the Controllers folder of the project and generate clients for the Controllers inside by using Roslyn to parse the files.
This means we pull all of the namespaces as well.

- MyService.csproj
  - Controllers
    - ValuesController.cs

The first thing you should do would be to add any namespaces from your server project that are required by the client project to the AllowedNamespaces property inside the Clients.csproj
This may require you to include projects depending on if you have contract projects that your service depends on.

``` c#
using TestWebApp.Contracts;

	[HttpGet("[action]/{id:int}")]
	[ProducesResponseType(typeof(TestWebApp.Contracts.MyFancyDto), (int)HttpStatusCode.OK)]
	public IActionResult FancyDtoReturn(int id)
	{
		return Ok(new TestWebApp.Contracts.MyFancyDto
		{
			Id = id,
			Collision = Guid.NewGuid(),
			Description = "Hello There",
			When = DateTime.UtcNow
		});
	}
```


There will also be namespaces that you don't want to include like internals for the Service project, you can exclude these namespaces from the generator with the ExcludedNamespaces property inside the Clients.csproj

``` xml
<ExcludedNamespaces>TestWebApp.Internal*;</ExcludedNamespaces>
```

Once you have made your modifications, you can run the build again to regenerate the Clients.cs



You should have a static method like so inside your Clients.cs

``` c#
public static IServiceCollection InstallClients(this IServiceCollection services, Action<ClientConfiguration> configure)
```

This is used to register the clients and associated required services into your consuming app's ServiceCollection.

The registration will look something like this:

``` c#
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




Although if you are using Blazor it would look more like this:

``` c#
var serviceProvider = new BrowserServiceProvider(services =>
{
	services.InstallClients(config=>
	{
		config.UseBlazorSimpleJsonSerlaizer();
	});
});
```

Which is mainly due to Blazor already injecting a HttpClient by default.


You should be able to inject your Clients now and use them!

``` c#
private readonly IValuesClient _valuesClient;

public MyFancyService(IValuesClient valuesClient)
{
	_valuesClient = valuesClient;

}


public MyFancyDto MyFancyAction(int id)
{
	return _valuesClient.Get(id);
}

public async Task<MyFancyDto> MyFancyActionAsync(int id)
{
	return _valuesClient.GetAsync(id);
}
```