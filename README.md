## Description

For the extension to work, you need to specify a configuration file that essentially only contains paths (one per line) for searching for projects:


![ext_cfg_0.png](https://github0username.gallerycdn.vsassets.io/extensions/github0username/dcb9fb28-5610-4a94-9471-4bf2d0556bc5/0.4.1/1595159901231/ext_cfg_0.png)

 - **Exclude File** - Path to a file containing enumerations of directories used to exclude projects and libraries
 - **Include Library File** - Path to a file containing enumerations of directories used to include libraries
 - **Include Project File** - Path to a file containing enumerations of directories used to include projects


When you click the **Use ProjectReference** button, the extension looks through the list of projects included in the solution and begins to process each of them sequentially.

![ext_menu_0.png](https://github0username.gallerycdn.vsassets.io/extensions/github0username/dcb9fb28-5610-4a94-9471-4bf2d0556bc5/0.4.1/1595159901231/ext_menu_0.png)

Three switching modes are available:

 - From NuGet packages to local projects
 - From NuGet packages to local libraries
 - From local projects/libraries to NuGet packages


Project dependency search is based on **project.assets.json** file, which lists all the dependencies of the project. It is created in the **/obj** folder when using `dotnet restore` or `dotnet build` as it implicitly calls restore before build, or `msbuid.exe /t:restore` with msbuild CLI.  If **project.assets.json** is not found in the specified project directory, then the extension stops processing and displays the following message:

![ext_switch_error_0.png](https://github0username.gallerycdn.vsassets.io/extensions/github0username/dcb9fb28-5610-4a94-9471-4bf2d0556bc5/0.4.1/1595159901231/ext_switch_error_0.png)


If the file is present, then the extension starts processing explicit and implicit (transitive) dependencies of the project - NuGet packages, framework assemblies. After processing, the **Messages** tab of the **Error List** displays information about each changed project and the references included in it.


**Note**:

When changing references of one type to another, there is no loss of user metadata.


---


## Conditional references

**ItemGroups** with conditions defined as follows are not handled correctly, which will be fixed in the following versions:

```
<Choose>
  <When Condition=" $(PackageVersion.Contains('unstable')) or '$(SolutionDir)' != '' ">
    <ItemGroup>
      <PackageReference Include="Library" Version="0.1.0-unstable*" />
    </ItemGroup>
  </When>
  <Otherwise>
    <ItemGroup>
      <ProjectReference Include="Library" Version="0.1.0-stable*"   />
    </ItemGroup>
  </Otherwise>
</Choose>
```


---


## Multitargeting 

Multitargeting as such does not have special support, i.e. a project with **TFMs** defined as follows:

```xml
<TargetFrameworks>
  net461;net462;net472;netstandard2.0;netstandard2.1
</TargetFrameworks>
```

Will be processed as if it had **TFM**:

```xml
<TargetFrameworkVersion>
  v4.6.1
</TargetFrameworkVersion>
```

In fact, this is governed by the **TargetFrameworkMoniker** property presented as [EnvDTE.Property](https://docs.microsoft.com/en-us/dotnet/api/envdte.property?view=visualstudiosdk-2017&viewFallbackFrom=visualstudiosdk-2019) of [EnvDTE.Project](https://docs.microsoft.com/en-us/dotnet/api/envdte.project?view=visualstudiosdk-2017&viewFallbackFrom=visualstudiosdk-2019).


---


## Requirements

It is assumed that in the stable version the extension should support **Visual Studio** **17**, **19**. However, at the moment, such compatibility is not guaranteed like anything else. The author conducted an initial test in the next version of **Visual Studio**:

```none
Microsoft Visual Studio Community 2019 Preview
Version 16.5.0 Preview 3.0
```

```
Microsoft Visual Studio Community 2019
Version 16.4.2
```

The test was carried out on classic projects using **.NET 4.6.2** with the following dependencies:

- **.NET 4.6.2**

A project using **.NET Core 3.1** and having the following dependencies was also tested:

- **.NET Standard 2.1**


---


## Useful Links

 - [Github Issues](https://github.com/0UserName/NuGetSwitcher/issues)
 - [Documentation](https://0username.github.io/NuGetSwitcher/)
