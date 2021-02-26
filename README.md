# webgrep

[![CC BY-NC-SA 4.0][cc-by-nc-sa-shield]][cc-by-nc-sa] ![project status][status-shield]

This work is licensed under a
[Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License][cc-by-nc-sa].

[cc-by-nc-sa]: LICENSE
[cc-by-nc-sa-shield]: https://img.shields.io/badge/License-CC%20BY--NC--SA%204.0-informational.svg
[status-shield]: https://img.shields.io/badge/status-active%20development-brightgreen

# Requirements

Add the following line(s) to your nuget includes:

```xml
<ItemGroup>
    <PackageReference Include="PlaywrightSharp" Version="0.180.0" />
</ItemGroup>
```

# Example program

```csharp
static void Main(string[] args)
{
    using (FirefoxInstance firefox = new FirefoxInstance())
    {
        firefox.NavigateTo("https://github.com/microsoft/playwright-sharp");
        Console.WriteLine(firefox.GetText("div.css-truncate:nth-child(1) > span:nth-child(2)"));
    }
}
```
