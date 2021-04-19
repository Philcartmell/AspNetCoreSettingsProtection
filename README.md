# Introduction
AspNetCoreSettingsProtection allows you to use the standard Microsoft.AspNetCore.DataProtection library to protect strings and decrypt them once when consumed by your application.

[![Build status](https://github.com/Philcartmell/AspNetCoreSettingsProtection/actions/workflows/build.yml/badge.svg)](https://github.com/Philcartmell/AspNetCoreSettingsProtection/actions/workflows/build.yml)
[![NuGet](https://img.shields.io/nuget/v/AspNetCoreSettingsProtection.svg)](https://www.nuget.org/packages/AspNetCoreSettingsProtection/)
[![GitHub stars](https://img.shields.io/github/stars/philcartmell/AspNetCoreSettingsProtection.svg)](https://github.com/philcartmell/AspNetCoreSettingsProtection/stargazers)
[![GitHub forks](https://img.shields.io/github/forks/philcartmell/AspNetCoreSettingsProtection.svg)](https://github.com/philcartmell/AspNetCoreSettingsProtection/network)
[![License: MIT](https://img.shields.io/github/license/philcartmell/AspNetCoreSettingsProtection.svg)](https://opensource.org/licenses/MIT)

## How to use it

* Mark your POCO property with ````[IsProtected("nameOfProtectorPurpose")]````

````
public class WeatherStationSettings
{
    public string IpAddress { get; set; }

    [IsProtected("default")]
    public string Token { get; set; }
}
````

*  Register the type and configSection

````
public void ConfigureServices(IServiceCollection services)
{
    services.AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo(@"c:\somewhere\secure\"))
            .SetDefaultKeyLifetime(TimeSpan.FromDays(14))
            .SetApplicationName("webapp");

    // here
    services.AddEncryptedOption<WeatherStationSettings>("weatherStation:credentials");

    services.AddControllers();
}
````

* Consume using the options pattern, specifically ````IOptions<T>````, for example:

````
[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly IOptions<WeatherStationSettings> options;

    public WeatherForecastController(IOptions<WeatherStationSettings> options)
    {
        this.options = options;
    }

    [HttpGet]
    public double Get()
    {
        // do something with the decrypted setting value. 
        var accessToken = this.options.Value.Token;
        
        return 12.34;
    }
}
````

## Disclaimer

Do not use this library if you can use something like Azure KeyVault/some other provider to store sensitive values outside of your configuration files.

**Wherever you persist your keys, ensure access is restricted to the minimum identities that need it.**

### Example appSettings.json
````
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "weatherStation": {
    "credentials": {
      "ipAddress": "1.2.3.4",
      "token": "CfDJ8Mf2VyECRqZMv1uUugXD-kGC_d_AnHaccxM1dO5RHvitHXMwuMAm2CbZjlDLDrjyWgmNCeGRKdPOEDP71hV0UME1B96cY3bYeGxAnItkxujqm8-JLTRCg-NTbWHCbbYExw"
    }
  }
}
````

### Encrypting values

Obviously you'll need a mechnaism of encrypting any strings you wish to use, using the same encryption keys as your environment.

This just involves creating a suitable ````IDataProtector```` that matches your purpose and encrypting your strings, i.e.

````
[ApiController]
[Route("[controller]")]
public class EncryptController : ControllerBase
{
    private readonly IDataProtectionProvider dataProtectionProvider;

    public WeatherForecastController(IDataProtectionProvider dataProtectionProvider)
    {
        this.dataProtectionProvider = dataProtectionProvider;
    }

    [HttpGet("encrypt/{purpose}/{value}")]
    public string Encrypt(string purpose, string value)
    {
        // purpose needs to match POCO [IsProtected("nameOfProtectorPurpose")] attribute.
        var protector = this.dataProtectionProvider.CreateProtector(purpose);

        // return value stored somewhere securely ready for deployment pipeline - i.e. not in source.
        return protector.Protect(value);
    }
}
````

### Key rotation

Luckily as long as you don't throw away old keys, then they can be used to decrypt data even after they have expired.

If you are encrypting as a build step that's part of your deployment process then you will always using a current, or nearly current, encryption key.