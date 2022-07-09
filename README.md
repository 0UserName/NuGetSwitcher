## Build Status

![Build Status](https://github.com/0UserName/NuGetSwitcher/actions/workflows/main.yml/badge.svg) [![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=0UserName_NuGetSwitcher&metric=alert_status)](https://sonarcloud.io/dashboard?id=0UserName_NuGetSwitcher)

## Description

<div style="text-align: justify">
For the extension to work, you need to specify a configuration file that essentially only contains paths (one per line) for searching for projects:
</div>

![ext_options](https://user-images.githubusercontent.com/46850587/148584567-ad3d5b06-1a70-486c-9b20-f23ad4b554d4.png)

<div style="text-align: justify">
When you click the <code>Use ProjectReference</code> button, the extension looks through the list of projects included in the solution and begins to process each of them sequentially.
</div>

![ext_sln_explorer](https://user-images.githubusercontent.com/46850587/148584494-a64ccc6d-864c-4267-bdc1-e1ab58995b33.png)

<div style="text-align: justify">
Project dependency search is based on <code>project.assets.json</code> file, which lists all the dependencies of the project. It is created in the /obj folder when using <code>dotnet restore</code> or <code>dotnet build</code> as it implicitly calls restore before build, or <code>msbuid.exe /t:restore</code> with MSBuild CLI. If <code>project.assets.json</code> is not found in the specified project directory, then the extension stops processing and displays the following message:
</div>

![ext_error_list](https://user-images.githubusercontent.com/46850587/148586028-5ed93be8-8ab5-4256-9f00-10af0507e4cd.png)

<div style="text-align: justify">
If the file is present, then the extension starts processing explicit and implicit (transitive) dependencies of the project - NuGet packages, framework assemblies. After processing, the Messages tab of the Error List displays information about each changed project and the references included in it.
</div>

## Conditional references

<div style="text-align: justify">
When working with references, only evaluated items are used. In this example, when using the Debug configuration, reference A will be replaced, while for the Release configuration, reference B.
</div>

```
<Choose>
  <When Condition=" '$(Configuration)' != 'Release' ">
    <ItemGroup>
      <PackageReference Include="LibraryA" Version="0.1.0*" />
    </ItemGroup>
  </When>
  <Otherwise>
    <ItemGroup>
      <ProjectReference Include="LibraryB" Version="0.1.0*" />
    </ItemGroup>
  </Otherwise>
</Choose>
```

## Multitargeting 

<div style="text-align: justify">
Multitargeting as such does not have special support, i.e. a project with <code>TFMs</code> defined as follows:
</div>

```
<TargetFrameworks>
  net461;net462;net472;netstandard2.0;netstandard2.1
</TargetFrameworks>
```

<div style="text-align: justify">
Will be processed as if it had <code>TFMs</code>:
</div>

```
<TargetFrameworkVersion>
  v4.6.1
</TargetFrameworkVersion>
```

## Comments

<div style="text-align: justify">
Separate lines entered in the configuration file can be commented out as follows:
</div>

```
#E:\Project\a\Library
```

<div style="text-align: justify">
With this configuration, at the moment of switching references, the search for in the commented out directories will be ignored.
</div>

## Requirements

<div style="text-align: justify">
It is assumed that in the stable version the extension should support Visual Studio 22. The author conducted an initial test in the next version of Visual Studio:
</div>

```
Microsoft Visual Studio Community 2022
Version 17.2.5
```

<div style="text-align: justify">
.NET Core SDK version >= 3.0 is <b>required</b> since version 0.5.0.
</div>

## Useful Links

 - [Github Issues](https://github.com/0UserName/NuGetSwitcher/issues)
 - [Visual Studio Marketplace](https://marketplace.visualstudio.com/items?itemName=github0UserName.DCB9FB28-5610-4A94-9471-4BF2D0556BC5)
