[![Build status](https://ci.appveyor.com/api/projects/status/github/bytedev/ByteDev.Subtitles.SubRip?branch=master&svg=true)](https://ci.appveyor.com/project/bytedev/ByteDev-Subtitles-SubRip/branch/master)
[![NuGet Package](https://img.shields.io/nuget/v/ByteDev.Subtitles.SubRip.svg)](https://www.nuget.org/packages/ByteDev.Subtitles.SubRip)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://github.com/ByteDev/ByteDev.Subtitles.SubRip/blob/master/LICENSE)

# ByteDev.Subtitles.SubRip

.NET Standard library to help when using and manipulating SubRip subtitle (.srt) files.

## Installation

ByteDev.Subtitles.SubRip is hosted as a package on nuget.org.  To install from the Package Manager Console in Visual Studio run:

`Install-Package ByteDev.Subtitles.SubRip`

Further details can be found on the [nuget page](https://www.nuget.org/packages/ByteDev.Subtitles.SubRip/).

## Release Notes

Releases follow semantic versioning.

Full details of the release notes can be viewed on [GitHub](https://github.com/ByteDev/ByteDev.Subtitles.SubRip/blob/master/docs/RELEASE-NOTES.md).

## Usage

Object hierarchy:

```
SubRipFile
- SubRipEntry
-- SubRipDuration
--- SubRipTimeSpan
```

```csharp
// Create SubRipFile from file (or create directly via public constructors)
SubRipFile file = SubRipFile.Load(@"C:\Videos\Carlito's Way [1993] (English Forced).srt");

Console.WriteLine(file.FileName);

SubRipEntry entry1 in file.Entries[0];

Console.WriteLine(entry1.OrderId);
Console.WriteLine(entry1.Duration.ToString());
Console.WriteLine(entry1.Text);

Console.WriteLine();

SubRipEntry entry2 in file.Entries[1];

Console.WriteLine(entry2.ToString());
```

Output:

```
Carlito's Way [1993] (English Forced).srt
1
01:40:55,758 --> 01:40:58,426
Listen to me carefully, Carlito.

2
01:40:58,677 --> 01:41:02,013
Rudy says Pachanga is complaining
about being broke;
```

The type `SubRipFile` also has a number of methods for acting on it's entries including:

- RemoveEntry
- RemoveTextFormatting
- RemoveTextReturnChars
- SetAbsoluteDuration
- SetMaxDuration
- SetOrderIds
- SetTextLineMaxLength
- Sort