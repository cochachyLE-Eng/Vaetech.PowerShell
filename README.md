﻿# Vaetech.PowerShell

[![Join the chat at (https://badges.gitter.im/Vaetech-PowerShell)](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/Vaetech-PowerShell/community?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge)

|    Package    |Latest Release|
|:--------------|:------------:|
|**Vaetech.PowerShell**    |[![NuGet Badge Vaetech.PowerShell](https://buildstats.info/nuget/Vaetech.PowerShell)](https://www.nuget.org/packages/Vaetech.PowerShell)

## What is Vaetech.PowerShell?

Vaetech.PowerShell is a C# library which may be used for running commands with lambda expressions and getting data from PowerShell, in
.Net Framework, .Net Standard and .Net Core

## License Information

```
MIT License

Copyright (C) 2021-2022 .NET Foundation and Contributors

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
```

## Installing via NuGet

The easiest way to install Vaetech.PowerShell is via [NuGet](https://www.nuget.org/packages/Vaetech.PowerShell/).

In Visual Studio's [Package Manager Console](http://docs.nuget.org/docs/start-here/using-the-package-manager-console),
enter the following command:

    Install-Package Vaetech.PowerShell

## Getting the Source Code

First, you'll need to clone Vaetech.PowerShell from my GitHub repository. To do this using the command-line version of Git,
you'll need to issue the following command in your terminal:

    git clone --recursive https://github.com/cochachyLE-Eng/Vaetech.PowerShell.git

## Updating the Source Code

Occasionally you might want to update your local copy of the source code if I have made changes to Vaetech.PowerShell since you
downloaded the source code in the step above. To do this using the command-line version fo Git, you'll need to issue
the following commands in your terminal within the Vaetech.PowerShell directory:

    git pull
    git submodule update

## Building

In the top-level Vaetech.PowerShell directory, there are a number of solution files; they are:

* **Vaetech.PowerShell.sln** - includes projects for .NET 4.5/4.6.1/4.8, .NETStandard 2.1 as well as the unit tests.

Once you've opened the appropriate Vaetech.PowerShell solution file in [Visual Studio](https://www.visualstudio.com/downloads/),
you can choose the **Debug** or **Release** build configuration and then build.

Both Visual Studio 2017 and Visual Studio 2019 should be able to build Vaetech.PowerShell without any issues, but older versions such as
Visual Studio 2015 will require modifications to the projects in order to build correctly. It has been reported that adding
NuGet package references to 

[Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json/) (>= 13.0.1)
[System.ComponentModel](https://www.nuget.org/packages/System.ComponentModel/) (>= 4.3.0)
[System.ComponentModel.Annotations](https://www.nuget.org/packages/System.ComponentModel.Annotations/) (>= 5.0.0)
[Vaetech.Data.ContentResult](https://www.nuget.org/packages/Vaetech.Data.ContentResult/) (>= 1.6.6)
[Vaetech.Runtime.Utils](https://www.nuget.org/packages/Vaetech.Runtime.Utils/) (>= 1.0.3)

will allow Vaetech.PowerShell to build successfully.

## Using Vaetech.PowerShell

### How can it be used?

At its most basic, it only takes a few lines of code to run PowerShell queries with lambda expressions.

```csharp
// Gets the processes that are running on the local computer.
var getProcessResponse = PShell.GetProcess("svchost").WhereObject(c => c.StartTime > DateTime.Now.AddDays(-5)).FormatList(x => new { x.Name, x.Id, x.StartTime });
// Get Command
string command = getProcessResponse.GetCommand();
```

Get results with ErrorAction in custom collection

```csharp
// Gets the processes running on the local computer, in custom collection.
var getProcessResponse = PShell.GetProcess(ErrorAction.SilentlyContinue,"svchost", "w3wp").SelectObject(x => new { x.Name, x.Id, x.StartTime }).WhereObject(c => c.StartTime.Date > DateTime.Now.AddDays(-24).Date).ConvertToJson();
// Get Command
string command = getProcessResponse.GetCommand();
```

Get results by date range

```csharp
// Gets the processes running on the local computer by date range, in custom collection.
var getProcessResponse = PShell.GetProcess("svchost", "w3wp").WhereObject(c => c.StartTime > DateTime.Now.AddDays(-7) && c.StartTime < DateTime.Now.AddDays(-1)).SelectObject(x => new { x.Name, x.Id, x.StartTime, x.PM, x.WS, x.VM, x.CPU, x.Handles }).ConvertToJson();            
// Get Command
string command = getProcessResponse.GetCommand();
```

Stop Process (-Force) by date range

```csharp
// Stops one or more running processes.
var getProcessResponse = PShell.GetProcess("w3wp").WhereObject(c => c.StartTime > DateTime.Now.AddDays(-7) && c.StartTime < DateTime.Now.AddDays(-1)).StopProcessForce();
            
// Get Command
string command = getProcessResponse.GetCommand();
```

Run command

```csharp
// Get-Process
ActionResult<GetProcessResponse> resultGetProcess = getProcessResponse.Execute();

if (resultGetProcess.IB_Exception)
    System.Console.WriteLine("Error: {0}",resultGetProcess.Message);            
else 
{
    foreach (var process in resultGetProcess.List)
    {
        System.Console.WriteLine("ID: {0}, Name: {1}, StartTime: {2}, CPU: {3}", process.Id, process.Name, process.StartTime.ToString(PShellSettings.DateFormat), process.CPU);
    }
}
```
