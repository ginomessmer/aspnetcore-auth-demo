# Authentication in ASP.NET Core
![.NET Core](https://github.com/ginomessmer/aspnetcore-auth-demo/workflows/.NET%20Core/badge.svg)

## Key Takeaways
- JSON Web Tokens (JWT) are a commonly used technique for HTTP authentication & authorization
- A JWT consists of a header, payload and a signature
- The JWT payload contains the required details of the authenticated subject (such as ID, etc.)

### Validate a JWT
- The authentication requirements are specified in the `Startup` class:

```cs
public void ConfigureServices(IServiceCollection services)
{
    // ...
    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) // Declare JWT authentication as the default authentication scheme
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidAudience = "test", // The designated consumer

                ValidIssuer = "test", // The source of the token provider

                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(
                        Configuration.GetValue<string>("Security:SigningKey"))) // The signing key used to encrypt the JWT
            };

            // Enable for production
            options.RequireHttpsMetadata = false;
        });
}
```

- Add the `[Authorize]` attribute to your controllers or route methods:
```cs
[ApiController]
[Route("[controller]")]
[Authorize] // <--
public class WeatherForecastController : ControllerBase { }
```

- Done
- If you want to retrieve the authenticated subject, access the `HttpContext.User` object


### Issue a JWT
```cs
var handler = new JwtSecurityTokenHandler();    // <-- declare a new handler
var descriptor = new SecurityTokenDescriptor    // <-- declare a descriptor that specifies the content of your JWT
{
    Audience = "test",                          // <-- remember the audience from the Startup class
    Issuer = "test",                            // <-- remember the issuer from the Startup class
    Expires = DateTime.UtcNow.AddDays(1),       // <-- specify when the token is supposed to expire

    Subject = new ClaimsIdentity(new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, dto.Username)     // <-- set the token claims
    }),

    SigningCredentials = new SigningCredentials(
        new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration.GetValue<string>("Security:SigningKey"))),
        SecurityAlgorithms.HmacSha256Signature)               // <-- sign the key
};

var token = handler.CreateToken(descriptor);    // <-- create the token
var jwt = handler.WriteToken(token);            // <-- write it to a string
```
