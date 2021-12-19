namespace NuGetSwitcher.Interface.Entity.Enum
{
    public enum ReferenceType : byte
    {
        /// <summary>
        /// <![CDATA[<ProjectReference Include="Project.csproj" />]]>
        /// </summary>
        ProjectReference = 0,

        /// <summary>
        /// <![CDATA[<PackageReference Include="Package" Version="1.0.0" />]]>
        /// </summary>
        PackageReference = 1,

        /// <summary>
        /// <![CDATA[<Reference Include="System.Data" />]]>
        /// </summary>
        Reference = 2
    }
}