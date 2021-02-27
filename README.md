# Racoon Secure


![GitHub](https://img.shields.io/github/license/TeleSoftas/RacoonSecure)
![Downloads](https://img.shields.io/nuget/dt/RacoonSecure.Core)
![Build](https://github.com/Telesoftas/RacoonSecure/actions/workflows/main.yml/badge.svg)

Latest version ![Version](https://img.shields.io/nuget/v/RacoonSecure.Core)

Latest Pre-release version ![Version](https://img.shields.io/nuget/vpre/RacoonSecure.Core)

![RacoonSecure Logo](RacoonSecure/RacoonSecure.Core/icon.jpg)

RacoonSecure is a free lightweight modular library for password validation in .NET projects. Library lets you set up predifined or custom password validation rules.

# Quick Start
To get started using library, you should instantiate PasswordValidator, this can be done using PasswordValidatorBuilder.

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
