# Racoon Secure Password Validator

[![RacoonSecure Logo](RacoonSecure/RacoonSecure.PasswordValidator/icon.jpg)](https://www.nuget.org/packages/RacoonSecure.PasswordValidator)

[![Version](https://img.shields.io/nuget/v/RacoonSecure.PasswordValidator?style=for-the-badge)](https://www.nuget.org/packages/RacoonSecure.PasswordValidator/)
[![Downloads](https://img.shields.io/nuget/dt/RacoonSecure.PasswordValidator?style=for-the-badge)](https://www.nuget.org/packages/RacoonSecure.PasswordValidator/)
![Build](https://img.shields.io/github/workflow/status/Telesoftas/RacoonSecure/Publish%20NuGet?style=for-the-badge)
[![License](https://img.shields.io/github/license/Telesoftas/RacoonSecure?style=for-the-badge)](https://github.com/TeleSoftas/RacoonSecure/blob/main/LICENSE)



RacoonSecure is a lightweight NuGet package for password validation in .NET. Library lets you set up password validation rules pipeline with pre-written checks against 100,000 most common passwords, 10MIL leaked passwords (provided by [HIBP](https://haveibeenpwned.com/)) and more.
This package is compatible with IdentityFramework, read more in [Identity Framework Integration](#identity-framework-integration)

You are welcome to define validation rules of your own as well. For further information on existing validation rules and creation of custom ones continue reading further.

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
To start validating away passwords reference [RacoonSecure.PasswordValidator](https://www.nuget.org/packages/RacoonSecure.PasswordValidator) package in your project (or use NuGet package manager). Next, instantiate PasswordValidator, this can be done using PasswordValidatorBuilder.

```csharp
//Initialization of PasswordValidator that uses NIST guidelines to validate the password
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

## NIST Guidelines rule

NIST Guidelines rule was designed in accordance with [NIST Digital Identity Guidelines p5.1.1.2](https://pages.nist.gov/800-63-3/sp800-63b.html#memsecret).
Invoking `UseNistGuidelines()` while building `PasswordValidator` will result in the following password checks:

- Password is not `NULL` or empty `string`
- Password length is 8 or more characters
- Password does not consist only of white space characters

## Common Passwords rule

`UseCommonPasswordCheck()` adds check against 100,000 most common passwords, if passed password is found amongst common passwords it is considered not valid.

Common passwords are stored as binary information inside of this package. Number of passwords to check against is chosen by considering practical value of the check as well as memory required to store information, so that package would provide maximum value while being as lightweight as possible.

## Bloom Filter rule

`UseBloomFilter()` adds check against 10,000,000 leaked passwords (with 0.0001% or 1 in 10,000 false positive rate), if passed password is found amongst leaked passwords it is considered not valid. Passwords used in BloomFilter rule are a superset of 100,000 most common passwords collection.

Bloom Filter is a probabilistic data structure which means, it is more memory-efficient in regards to standart data structures at cost of possible False positives (not false negatives). [Using some calculations](https://hur.st/bloomfilter/?n=10000000&p=1.0E-4&m=&k=) it's not hard to determine how much elements you can put into said-size bloom filter at pre-determined filter size to come out with desired false-positives rate.

In our case, we want to keep package as lightweight as possible, thus we end up giving ~23MB of memory for bloom filter, which allows us to place 10,000,000 elements inside and have false-positive rate of 0.0001%. 

Bloom Filter is great for our solution since it provides us with two possible outcomes "Password is definitely not in the filter" and "Password might be in the filter" (false positive). In case we encounter first validation outcome, it is safe to say, that password is never seen in list of 10,000,000 leaked passwords, on the other hand, if validation provides latter outcome, it has 0.0001% chance of being false-positive (better safe than sorry).   

## HIBP (Have I Been Pwned) rule

`UseHIBPApi()` adds a rule, that consults with [HIBP API](https://haveibeenpwned.com/) to determine whether password has been leaked in the past. Read more on topic of leaked passwords on this [FAQ](https://haveibeenpwned.com/FAQs)


# Custom Validation Rules

To create and use a custom validation rule with RacoonSecure you will need to create a class that inherits `IValidationRule` and register it using `PasswordValidationBuilder.UseCustom()`. Your class must implement `Validate()` method that returns error message string on failed validation. The Rest is up to you!

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

[![RacoonSecure Logo](RacoonSecure/RacoonSecure.PasswordValidator.Identity/icon.png)](https://www.nuget.org/packages/RacoonSecure.PasswordValidator.Identity)

[![Version](https://img.shields.io/nuget/v/RacoonSecure.PasswordValidator.Identity?style=for-the-badge)](https://www.nuget.org/packages/RacoonSecure.PasswordValidator.Identity/)
[![Downloads](https://img.shields.io/nuget/dt/RacoonSecure.PasswordValidator.Identity?style=for-the-badge)](https://www.nuget.org/packages/RacoonSecure.PasswordValidator.Identity/)
![Build](https://img.shields.io/github/workflow/status/Telesoftas/RacoonSecure/Publish%20NuGet?style=for-the-badge)
[![License](https://img.shields.io/github/license/Telesoftas/RacoonSecure?style=for-the-badge)](https://github.com/TeleSoftas/RacoonSecure/blob/main/LICENSE)


IdentityFramework users should make use [RacoonSecure.PasswordValidator.Identity](https://www.nuget.org/packages/RacoonSecure.PasswordValidator.Identity) package for effortless integration with framework's password validation pipeline.

Please note that `.AddRacoonSecurePasswordValidator<TUser>()` has an optional parameter to decide whether to override default IdentityFramework password validations.

Example below shows how to register RacoonSecure password validator in IdentityFramework (Overriding default validations and leaving everything to `RacoonSecurePasswordValidator`):
```csharp
//Build desired validator
var validator = new PasswordValidatorBuilder()
    .UseNistGuidelines()
    .UseBloomFilter()
    .Build();

//Call services.AddRacoonSecurePasswordValidator<TUser> and pass your validator as parameter
//pass false as seccond parameter to persist current validation rules and append
//RacoonSecure rules on top of existing ones.
services.AddIdentity<IdentityUser, IdentityRole>()
    .AddRacoonSecurePasswordValidator<User>(validator)
    
```

After this step is done, registering a user via `UserManage` will result in checking password against previously registered `validator` and failing if password is seen as not compliant by validator.

