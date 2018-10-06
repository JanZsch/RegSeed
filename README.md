# RegSeed
RegSeed is a C# class library for creating evenly distributed random strings from regular expressions.  
It was inspired by Xeger, overcomes, however, some of its drawbacks (see below).


## Quick Start

For generating random strings from a regular expression you can simply create a new RegSeed instance, pass the constructor the regex pattern and then call its *Generate*-method as often as you wish.

```c#
public void Main(string[] argv)
{
    const string pattern = "[Hh]allo (W|w)orld!";
    var regseed = new RegSeed(pattern);
    
    Console.WriteLine(regseed.Generate());
    Console.WriteLine(regseed.Generate());
}
```

The constructor call first parses the regex pattern and then builds an associated expression that ultimately generates the random strings. 
In case the regex pattern is malformed (for the supported regex grammar see [here]()), the constructor throws an exception containing detailed information on the parsing error.  
If you wish to handle such parsing errors programmatically, you can create a new RegSeed instance by a parameterless constructor call and then use the *TryLoadRegexPattern* method.

```c#
public void Main(string[] argv)
{
    const string malformedPattern = "([Hh]allo (W|w)orld!){X,2}";
    var regseed = new RegSeed();
    
    IParseResult parseResult = regseed.TryLoadRegexPattern(malformedPattern);

    if (!parseResult.IsSuccess) 
    {
        Console.WriteLine($"The provided regex pattern {malformedPattern} is malformed at position {parseresult.Position}. ErrorCode: {parseResult.ErrorCode}");
        return;
    }
    
    Console.WriteLine(regseed.Generate());
}
```
This method returns an object of type *IParseResult* containing the position of the parsing error as well as an string valued error code encoding the nature of the parsing error (for a list possible error codes an their meaning see [here]()).

## Advantages

As already mentioned above, RegSeed was inspired by *Xeger* which is a regex based, string generation engine employing finite automatons. It has, however, a major drawback concerning the distribution of characters within a generated string.   
Consider, for instance, take the following regex string which might be used for password generation:
```c#
var passwordPattern = @"[a-zA-Z0-9_]{10,15}";
```
Feeding this pattern into *Xeger* will result in random strings containing significantly more underscores than letters or digits which might lead to security risks.


## Where can I get it?
To be released on nuget.org soon.

## More Information
+ supported [Regex Backus-Naur-Form]()
+ define [custom alphabets]()
+ error codes an their meaning

## Acknowledgment

+ [Coverlet](https://github.com/tonerdo/coverlet/)
+ [NSubstitute](https://github.com/nsubstitute/NSubstitute)
+ [NUnit](https://github.com/nunit/nunit)

## License
This project is licensed under the Apache 2.0 license. For more information see [LICENSE](https://github.com/janzsch/regseed/blob/master/license).