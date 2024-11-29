<div style="line-height: 1.8rem;">

# Configuration Documentation

## IdentityConfig

The `IdentityConfig` object is used to configure the behavior of identity and authentication features within the application. This configuration contains several nested configuration objects for Password, Lockout, SignIn, and User settings.

### Properties

- `Password`: Defines password requirements.
  - `RequireDigit`: (bool) Whether a digit is required in passwords. Default: `true`.
  - `RequireLowercase`: (bool) Whether a lowercase letter is required. Default: `true`.
  - `RequireUppercase`: (bool) Whether a uppercase letter is required. Default: `true`.
  - `RequireNonAlphanumeric`: (bool) Whether a non-alphanumeric character is required. Default: `false`.
  - `RequiredLength`: (int) The minimum required length of the password. Default: `6`.
<br> Cannot be set to less than `4`.
  - `RequiredUniqueChars`: (int) The minimum required unique characters within the password. Default: `2`.
<br> Cannot be set to less than `2`.
>NOTE: RequiredUniqueChars cannot be set greater than RequiredLength. It will instead match RequiredLength and each character in the password has to be unique.
- `Lockout`: Configures lockout settings after failed login attempts.
  - `AllowedForNewUsers`: (bool) Wether new users can be locked out. Default: `true`.
  - `MaxFailedAccessAttempts`: (int) The maximum allowed failed attempts before lockout. Default: `0`.
  - `DefaultLockoutTimeSpan`: (TimeSpan) The duration for which a user is locked out. Default: `00:00:00`.
<br> Can also be set using `DefaultLockoutInMinutes` which takes an integer and converts it into TimeSpan
>IMPORTANT: Lockout is enabled when `MaxFailedAccessAttempts` and `DefaultLockout` are both set to a value greater than 0.
- `SignIn`: Configures sign-in behavior.
  - `RequireConfirmedAccount`: (bool) Wheter a confirmed account is required for sign-in. Default: `false`.
  - `RequireConfirmedEmail`: (bool) Whether a confirmed email is required for sign-in. Default: `false`.
  - `RequireConfirmedPhoneNumber`: (bool) Whether a confirmed phone number is required for sign-in. Default: `false`.
- `User`: Configures user account settings.
  - `AllowedUserNameCharacters`: (string) A list of allowed characters for usernames. Default: `"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@"`.

### Default Configuration

```json
{
  "IdentitySettings": {
    "Password": {
      "RequireDigit": true,
      "RequireLowercase": true,
      "RequireUppercase": true,
      "RequireNonAlphanumeric": false,
      "RequiredLength": 6,
      "RequiredUniqueChars": 2
    },
    "Lockout": {
      "AllowedForNewUsers": true,
      "MaxFailedAccessAttempts": 0,
      "DefaultLockoutInMinutes": 0
    },
    "SignIn": {
      "RequireConfirmedEmail": false,
      "RequireConfirmedPhoneNumber": false,
      "RequireConfirmedAccount": false
    },
    "User": {
      "AllowedUserNameCharacters": "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+"
    }
  }
}
```

## JwtConfig

The `JwtConfig` class is used to configure the JWT (JSON Web Token) settings for authentication. It provides properties to set various aspects of token creation, validation, and expiration.

### Properties

 -  `Secret`* (string?):
A secret key used to sign the JWT token. This is crucial for validating the authenticity of the token during authentication. It is recommended to store this in a secure environment.
<br>An InvalidOperationException will be thrown if this is null or empty at startup.
>IMPORTANT: You can store your secret in appsettings.Secret.json during development but NOT for production!
<br>Be careful not to commit it by accident.
 -  `Audience` (string):
The intended recipient(s) of the JWT token. This value is typically the service or API that is expecting the token. Default: `""`.
 -  `Issuer` (string):
The issuer of the JWT token. This is usually the identity of the issuing service (e.g., your authentication service or API). Default: `""`.
 -  `ValidateAudience`, `ValidateIssuer` (bool): Cannot be set directly but is dependant on the values of `Audience` and `Issuer`. If one is the validation is set to `false`.
 -  `ExpiresInMinutes` (int):
The number of minutes after which the token will expire. This helps control the lifetime of the token for security purposes. Default: `30`.
 -  `ClockSkew` (TimeSpan):
The underlying TimeSpan value representing the tolerance for the token's expiration time validation. Cannot be set to less than 0. Default: `"00:05:00"`
<br>Can also be set using `ClockSkewInMinutes` (int).
Example of what appsettings.Development.json might look like:

```json
{
  "Jwt": {
    "Audience": "localhost",
    "Issuer": "localhost",
    "ExpiresInMinutes": 30,
    "ClockSkewInMinutes": 2
  }
}
```