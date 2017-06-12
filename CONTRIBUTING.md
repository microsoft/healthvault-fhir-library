# How to contribute
One of the easiest ways to contribute is to participate in discussions and discuss issues. You can also contribute by submitting pull requests with code changes.

## General feedback and discussions?
Please start a discussion on the [issue tracker](https://github.com/Microsoft/healthvault-fhir-library/issues).

## Bugs and feature requests?
Please log a new issue in the appropriate GitHub repo. Here are some of the most common repos:

## Filing issues
When filing bugs, please use our [bug template](https://github.com/Microsoft/healthvault-fhir-library/wiki/Bug-Template).
The best way to get your bug fixed is to be as detailed as you can be about the problem, with a simple repro project if that will help illustrate the issue.

GitHub supports [markdown](http://github.github.com/github-flavored-markdown/), so when filing bugs make sure you check the formatting before clicking submit.

## Contributing code and content
Make sure you can build the code. Familiarize yourself with the project workflow and our coding conventions. If you don't know what a pull request is read this article: https://help.github.com/articles/using-pull-requests.

Before submitting a feature or substantial code contribution please discuss it with the team and ensure it follows the product roadmap. You might also read these two blogs posts on contributing code: [Open Source Contribution Etiquette](http://tirania.org/blog/archive/2010/Dec-31.html) by Miguel de Icaza and [Don't "Push" Your Pull Requests](http://www.igvita.com/2011/12/19/dont-push-your-pull-requests/) by Ilya Grigorik. Note that all code submissions will be reviewed by the HealthVault team, and only those that meet the bar for both quality and design/roadmap appropriateness will be merged into the source.

Here are some guidelines you should follow when making changes to the code base:

### Engineering guidelines
Pull requests should normally be submitted to the repo's `master` branch because that's where daily development takes place.  Submitted changes should follow the commit/PR format and coding style guidelines below.

### Commit/Pull Request Format
```
Brief summary of the changes

 - Detail 1
 - Detail 2
 ...

#bugnumber
```

Lines starting with `#` are usually stripped out by default, but this behavior can be adjusted by setting the [`cleanup` option](https://git-scm.com/docs/git-commit#git-commit---cleanupltmodegt) to something other than `strip` while committing.

### C# Coding Style
Our code style is largely borrowed from the [CoreFX C# Coding Style](https://github.com/dotnet/corefx/blob/eb883d78defbc7d4cae3b8ebd0fa68852eb583e3/Documentation/coding-guidelines/coding-style.md), which should hopefully privide a well-known base familiar to the community at large.  Like CoreFX, we have also included a Visual Studio settings file in our repo (/OneSDK.vssettings) that you can use to import many of the relevant settings into Visual Studio to help you adhere to the following coding style.  It is based on the one provided by the CoreFX team, though ours has been modified slightly to match the newline and indentation conventions in our codebase.

The general rule we follow is "use Visual Studio defaults".
1.	We use [Allman style](http://en.wikipedia.org/wiki/Indent_style#Allman_style) braces, where each brace begins on a new line. A single line statement block can go without braces but the block must be properly indented on its own line and it must not be nested in other statement blocks that use braces.
2.	We use four spaces of indentation (no tabs).
3.	We use `_camelCase` for internal and private fields and use `readonly` where possible. Prefix instance fields with `_`, static fields with `s_` and thread static fields with `t_`.  When used on static fields, `readonly` should come after `static` (i.e. `static readonly` not `readonly static`).
4.	We avoid `this.` unless absolutely necessary.
5.	We always specify the visibility, even if it's the default (i.e. `private string _foo` not `string _foo`). Visibility should be the first modifier (i.e. `public abstract` not `abstract public`).
6.	Namespace imports should be specified at the top of the file, outside of `namespace` declarations and should be sorted alphabetically, with System namespaces sorted before others.
7.	Avoid more than one empty line at any time. For example, do not have two blank lines between members of a type.  Comments should be preceded by an empty line.  Closing braces should be followed by an empty line unless followed by another closing brace or a paired statement (e.g., `if`/`else` or `try`/`catch`/`finally`).
8.	Avoid spurious free spaces. For example avoid `if (someVar == 0)...`, where the dots mark the spurious free spaces. Consider enabling "View White Space (Ctrl+E, S)" if using Visual Studio, to aid detection.
9.	If a file happens to differ in style from these guidelines (e.g. private members are named `m_member` rather than `_member`), the existing style in that file takes precedence.
10.	We only use var when it's obvious what the variable type is (i.e. `var stream = new FileStream(...)` not `var stream = OpenStandardInput()`).
11.	We use language keywords instead of BCL types (i.e. `int`, `string`, `float` instead of `Int32`, `String`, `Single`, etc) for both type references as well as method calls (i.e. `int.Parse` instead of `Int32.Parse`).
12.	We use PascalCasing to name all our constant local variables and fields. The only exception is for interop code where the constant value should exactly match the name and value of the code you are calling via interop.
13.	We use `nameof(...)` instead of `"..."` whenever possible and relevant.
