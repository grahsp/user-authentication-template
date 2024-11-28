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