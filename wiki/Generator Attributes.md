# Generator Attributes

Listing all of the attributes that the generator will pick up on and affect how the clients are generated.

## Attributes from AspNetCore.Client 

These attributes can be placed on either the entire controller or each method.
Controller attributes will apply to all endpoints under the controller

### HeaderParameterAttribute

- Adds a parameter into the client method that will be passed into the request as a header

``` c#
[HeaderParameter("ControllerHeader", typeof(int), "0")]
Would result in

IEnumerable<string> Get(int ControllerHeader = 0, 
			Action<string> BadRequestCallback = null, 
			Action InternalServerErrorCallback = null, 
			Action<HttpResponseMessage> ResponseCallback = null, 
			CancellationToken cancellationToken = default(CancellationToken))
```

### IncludeHeaderAttribute

- Includes a constant header into the requests

```c#
[IncludeHeader("Test", "EXTRA")]
```

### NoClientAttribute

- Ignores the endpoint during generation

---

## Attributes from AspNetCore

### ProducesResponseTypeAttribute

- Will generate a action of the type provided.  This action will be hit when the status code matches the response.
- Works on either controllers or methods

``` c#
[ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
[ProducesResponseType((int)HttpStatusCode.InternalServerError)]

Would result in

IEnumerable<string> Get(int ControllerHeader = 0, 
			Action<string> BadRequestCallback = null, 
			Action InternalServerErrorCallback = null, 
			Action<HttpResponseMessage> ResponseCallback = null, 
			CancellationToken cancellationToken = default(CancellationToken))

```

Supported styles
```c#
[ProducesResponseType(typeof(IEnumerable<int>), (int)HttpStatusCode.OK)]
[ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(int))]
[ProducesResponseType(typeof(int), StatusCodes.Status303SeeOther)]
[ProducesResponseType(typeof(string), 304)]
```

### RouteAttribute

- Determines what route to be used for the request

```c#

[HttpGet]
[Route("{id}")]
public IActionResult Get(int id)

```

### Http Attributes

- Used to determine what HTTP method to be used for the endpoint

- HttpGetAttribute
- HttpPostAttribute
- HttpPutAttribute
- HttpDeleteAttribute
- HttpPatchAttribute

- These can be used like RouteAttribute as well

```c#

[HttpGet("{id}")]
public IActionResult Get(int id)

```


### FromRouteAttribute

- Used to specify that a parameter binds from the route.
- Can be used to specify a different name than the parameter name
- The Name provided inside the attribute will be used when generator a client though.

```c#
[HttpPost("[action]/{testId:guid}")]
public IActionResult ComplexPost([FromRoute(Name = "testId")]Guid id, MyFancyDto dto)


void ComplexPost(Guid testId, 
			MyFancyDto dto,
			Action<HttpResponseMessage> ResponseCallback = null, 
			CancellationToken cancellationToken = default(CancellationToken))
```



### FromBodyAttribute

- Used during posts/put/patch to say whether or not a parameter comes from the body of the request.
- Only one parameter can have the FromBody attribute.

```c#

[HttpPut("{id}")]
public void Put(int id, [FromBody] string value)

```


### FromQueryAttribute

Until the following bug is fixed, the FromQuery with no params doesn't work.  https://github.com/aspnet/Mvc/issues/7712

Due to the current bug with ApiController, the truth in the below example doesn't work.

```c#
[HttpGet("[action]")]
public IActionResult EnumerableGet([FromQuery(Name = "customIds")]IEnumerable<int> ids, [FromQuery]IEnumerable<bool> truth)
```


### AuthorizeAttribute

- Adds a `SecurityHeader auth = null` to the parameters of the client, which allows for multiple types of auth headers.
- AspNetCore.Client provides the following types, but SecurityHeader is extendable
  - BasicAuthHeader
  - OAuthHeader

```c#
[HttpDelete("{id}")]
[Authorize]
public void Delete(int id)


void Delete(int id,
	Action<HttpResponseMessage> ResponseCallback = null, 
	SecurityHeader auth = null, 
	CancellationToken cancellationToken = default(CancellationToken))

```

