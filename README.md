# Racoon Secure


[![Version](https://img.shields.io/nuget/v/RacoonSecure.Core)](https://www.nuget.org/packages/RacoonSecure.Core)
![Downloads](https://img.shields.io/nuget/dt/RacoonSecure.Core)
![Build](https://github.com/Telesoftas/RacoonSecure/actions/workflows/main.yml/badge.svg)
![GitHub](https://img.shields.io/github/license/TeleSoftas/RacoonSecure)


[![RacoonSecure Logo](RacoonSecure/RacoonSecure.Core/icon.jpg)](https://www.nuget.org/packages/RacoonSecure.Core)

RacoonSecure is a lightweight NuGet package for password validation in .NET. Library lets you set up predifined or custom password validation rules and have client passwords validated in no time.

# Contents

1. [Quick Start](#quick-start)
2. [Validation Rules](#validation-rules)
    1. [NIST](#nist-guidelines-rule)
    2. [Common Passwords](#common-passwords-rule)
    3. [Bloom Filter](#common-passwords-rule)
    4. [HIBP](#HIBP-Have-I-Been-Pwned-rule)
3. [Custom Validation Rules](#custom-validation-rules)
4. [Identity Framework Integration](#identity-framework-integration)
 

# Quick Start
To start validating away passwords reference [RacoonSecure.Core](https://www.nuget.org/packages/RacoonSecure.Core) package in your project (or use NuGet package manager). Next, instantiate PasswordValidator, this can be done using PasswordValidatorBuilder.

```csharp
//Initialization of PasswordValidator that uses NIST guidelines to validate password
var validator = new PasswordValidatorBuilder().UseNistGuidelines().Build();
```

Now you can pass passwords to validator and have them tested. `Validate()` method will return object of type `ValidationResult`, you can invoke `ValidationResult.IsValid()` to get boolean expression of validation result.

```csharp
var password = "password1" //Password to be validated
var validationResult = validator.Validate(password); //Validating password
var isValid = validationResult.IsValid() //Parsing result
```

Error messages can be accessed through `ValidationResult.Errors` property which yields `IEnumerable<string>` with error messages for failed password validation.


# Validation Rules

Validation rules are set up before `PasswordValidatorBuilder.Build()` is called. Multiple rules can be set up. Passed password will be tested against rules in same order they were added.

## Nist Guidelines rule
Invoking `UseNistGuidelines()` while building `PasswordValidator` will result in following password checks:

- Password is not `NULL` or empty `string`
- Password length is 8 or mor characters
- Password does not consist only of white space characters

## Common Passwords rule

`UseCommonPasswordCheck()` adds check against 100,000 most common passwords, if passed password is found amongst common passwords it is considered not valid.

## Bloom Filter rule

`UseBloomFilter()` adds check against 10,000,000 leaked passwords (with 1 in 10,000 false positive rate), if passed password is found amongst common passwords it is considered not valid.

## HIBP (Have I Been Pwned) rule

`UseHIBPApi()` adds a rule, that consults with [HIBP API](https://haveibeenpwned.com/) to determine whether password has been leaked in the past. Read more on topic of leaked passwords on this [FAQ](https://haveibeenpwned.com/FAQs)


# Custom Validation Rules

To create and use custom validation rule with RacoonSecure you will need to create class that inherits `IValidationRule` and register it using `PasswordValidationBuilder.UseCustom()`. Your class must implement `Validate()` method that returns error message string on failed validation. The Rest is up to you!

```csharp
public class CustomRegexRule : IPasswordValidationRule
{
    public string Validate(string password)
    {
        return !Regex.IsMatch(password, @"^[\!\@\#\$\%\^\&\*\(\)]+$") 
            ? "Password doesn't match custom regex"
            : string.Empty; 
    }
}
```
```csharp
var validator = new PasswordValidatorBuilder().UseCustom(new CustomRegexRule()).Build();
```  

# Identity Framework Integration
[![RacoonSecure Logo](RacoonSecure/RacoonSecure.Identity/icon.png)](https://www.nuget.org/packages/RacoonSecure.Identity)

IdentityFramework users should make use [RacoonSecure.Identity](https://www.nuget.org/packages/RacoonSecure.Identity) package for effortless integration with framework's password validation pipeline.

Please note that `.AddRacoonSecurePasswordValidator<TUser>()` placement has a meaning:
* if called **BEFORE** .`AddIdentity<Tuser, TRole>()` it will **OVERRIDE** default password validation rules of IdentityFramework. 
* If called **AFTER** .`AddIdentity<Tuser, TRole>()` **APPEND** new validation rules to existing ones.

Example below shows how to register RacoonSecure password validator in IdentityFramework (Overriding default validations and leaving everything to `RacoonSecurePasswordValidator`):
```csharp
//Build desired validator
var validator = new PasswordValidatorBuilder()
    .UseNistGuidelines()
    .UseBloomFilter()
    .Build();

//Call services.AddRacoonSecurePasswordValidator<TUser> and pass your validator as parameter
services.AddRacoonSecurePasswordValidator<IdentityUser>(validator);
services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
```

