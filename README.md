## Build Status

![Build Status](https://github.com/0UserName/NuGetSwitcher/actions/workflows/main.yml/badge.svg) [![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=0UserName_NuGetSwitcher&metric=alert_status)](https://sonarcloud.io/dashboard?id=0UserName_NuGetSwitcher)

## Description

<div style="text-align: justify">
For the extension to work, you need to specify a configuration file that essentially only contains paths (one per line) for searching for projects:
</div>

![ext_cfg_0__1.png](https://github0username.gallerycdn.vsassets.io/extensions/github0username/dcb9fb28-5610-4a94-9471-4bf2d0556bc5/0.5.1/1612794019107/ext_cfg_0__1.png)

<div style="text-align: justify">
When you click the <code>Use ProjectReference</code> button, the extension looks through the list of projects included in the solution and begins to process each of them sequentially.
</div>

![ext_menu_0__1.png](https://github0username.gallerycdn.vsassets.io/extensions/github0username/dcb9fb28-5610-4a94-9471-4bf2d0556bc5/0.5.1/1612794760354/ext_menu_0__1.png)

<div style="text-align: justify">
Project dependency search is based on <code>project.assets.json</code> file, which lists all the dependencies of the project. It is created in the /obj folder when using <code>dotnet restore</code> or <code>dotnet build</code> as it implicitly calls restore before build, or <code>msbuid.exe /t:restore</code> with MSBuild CLI. If <code>project.assets.json</code> is not found in the specified project directory, then the extension stops processing and displays the following message:
</div>

![ext_switch_error_0.png](https://github0username.gallerycdn.vsassets.io/extensions/github0username/dcb9fb28-5610-4a94-9471-4bf2d0556bc5/0.4.1/1595159901231/ext_switch_error_0.png)

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

<div style="text-align: justify">
In fact, this is governed by the <code>TargetFrameworkMoniker</code> property presented as <code>EnvDTE.Property</code> of <code>EnvDTE.Project</code>.
</div>

## Requirements

<div style="text-align: justify">
It is assumed that in the stable version the extension should support Visual Studio 17, 19. However, at the moment, such compatibility is not guaranteed like anything else. The author conducted an initial test in the next version of Visual Studio:
</div>

```
Microsoft Visual Studio Community 2019 Preview
Version 16.9.0 Preview 2.0
```

```
Microsoft Visual Studio Community 2019 Preview
Version 16.5.0 Preview 3.0
```

```
Microsoft Visual Studio Community 2019
Version 16.4.2
```

<div style="text-align: justify">
.NET SDK version >= 3.0 is <b>required</b> since version 0.5.0.
</div>

## Useful Links

 - [Github Issues](https://github.com/0UserName/NuGetSwitcher/issues)
 - [Documentation](https://0username.github.io/NuGetSwitcher/)
