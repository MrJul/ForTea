using System;
using System.Collections.Generic;
using JetBrains.Application;
using JetBrains.Application.BuildScript;
using JetBrains.Application.BuildScript.Application;
using JetBrains.Application.BuildScript.Solution;
using JetBrains.Application.Environment;
using JetBrains.Application.Environment.HostParameters;
using JetBrains.Build.Serialization;
using JetBrains.Extension;
using JetBrains.Lifetimes;
using JetBrains.Metadata.Reader.API;
using JetBrains.Metadata.Utils;
using JetBrains.TestFramework;
using JetBrains.TestFramework.Application.Zones;
using JetBrains.TestFramework.Utils;
using JetBrains.Util;
using JetBrains.Util.Collections;
using JetBrains.Util.Storage;
using JetBrains.Util.Storage.StructuredStorage;

namespace JetBrains.ForTea.Tests
{
  /// <summary>
  /// A very dirty hack.
  /// Copied from platform and altered a bit.
  /// Nuget puts all the assemblies in the build folder.
  /// Default implementation loads all accessible assemblies
  /// and searches them for shell components,
  /// which causes serious troubles.
  /// JetBrains.Roslyn.References.Repack causes most headache.
  /// </summary>
  public abstract class T4ExtensionTestEnvironmentAssembly<TTestEnvironmentZone> : TestEnvironmentAssembly<TTestEnvironmentZone>
     where TTestEnvironmentZone : ITestsEnvZone
  {
    public override bool IsRunningTestsWithAsyncBehaviorProhibited { get { return true; } }

    public override void SetUp()
    {
      var mainAssembly = GetType().Assembly;
      var productBinariesDir = mainAssembly.GetPath().Parent;
        var assemblyNameInfo = AssemblyNameInfo.Parse(mainAssembly.FullName);

      var packageArtifactDummy = new ApplicationPackageArtifact(new SubplatformName(assemblyNameInfo.Name), new JetSemanticVersion(assemblyNameInfo.Version),  "FakeCompanyName", "FakeCompanyName", DateTime.UtcNow, null, null, EmptyList<ApplicationPackageFile>.InstanceList, EmptyList<ApplicationPackageReference>.InstanceList);
      var metafile = productBinariesDir / NugetApplicationPackageConvention.GetJetMetadataEffectivePath(packageArtifactDummy);
      metafile.DeleteWithMoveAside();
      
      var packages = AllAssembliesLocator.GetAllAssembliesOnLocallyInstalledBinariesFlat(new ProductBinariesDirArtifact(productBinariesDir));

        var packageFiles = new HashSet<ApplicationPackageFile>(
          EqualityComparer.Create<ApplicationPackageFile>(
            (file1, file2) => file1.LocalInstallPath == file2.LocalInstallPath,
            file => file.LocalInstallPath.GetHashCode())
          );

        var packageReferences = new HashSet<ApplicationPackageReference>(
          EqualityComparer.Create<ApplicationPackageReference>(
            (reference1, reference2) => string.Equals(reference1.PackageId, reference2.PackageId, StringComparison.OrdinalIgnoreCase),
            reference => reference.PackageId.GetHashCode())
          );

        using (var loader = new MetadataLoader(productBinariesDir))
        {
          ProcessAssembly(packages, productBinariesDir, loader, assemblyNameInfo, packageFiles, packageReferences);
        }
        var packageArtifact = new ApplicationPackageArtifact(new SubplatformName(assemblyNameInfo.Name), new JetSemanticVersion(assemblyNameInfo.Version),  "FakeCompanyName", "FakeCompanyName", DateTime.UtcNow, null, null, packageFiles, packageReferences);

      var fiMetadata = Lifetime.Using(lifeResolver => 
        {
          // All components together
          var resolver = new SerializedValuesResolver(lifeResolver, new []{packageArtifact});

          // Components in a file
          // NOTE: this file can't be a [Transformed]SubplatformFileForPackaging because the components are built out of the transformed files themselves, and this would require another level of post-transformed files, which we would not yet like to do
          return new SimpleFileItem(NugetApplicationPackageConvention.GetJetMetadataEffectivePath(packageArtifact), StructuredStorages.CreateMemoryStream(storage => resolver.GetObjectData(storage)));
        });

      metafile.WriteStream(sout => fiMetadata.FileContent.CopyStream(sout));

      base.SetUp();
    }

    protected override JetHostItems.Packages CreateJetHostPackages(JetHostItems.Engine engine)
    {
      var mainAssembly = GetType().Assembly;
      var productBinariesDir = mainAssembly.GetPath().Parent;

      TestUtil.SetHomeDir(mainAssembly);

      Lazy<ProductBinariesDirArtifact> productBinariesDirArtifact = Lazy.Of(() => new ProductBinariesDirArtifact(mainAssembly.GetPath().Directory));
      var jethostitempackages = new JetHostItems.Packages(engine.Items.Concat(new CollectProductPackagesInDirectoryFlatNoCachingHostMixin(productBinariesDirArtifact, allass => new[] {allass.FindSubplatformOfAssembly(mainAssembly.GetNameInfo(), OnError.Throw)}, packages =>
      {
        var packageFiles = new HashSet<ApplicationPackageFile>(
          EqualityComparer.Create<ApplicationPackageFile>(
            (file1, file2) => file1.LocalInstallPath == file2.LocalInstallPath,
            file => file.LocalInstallPath.GetHashCode())
          );

        var packageReferences = new HashSet<ApplicationPackageReference>(
          EqualityComparer.Create<ApplicationPackageReference>(
            (reference1, reference2) => string.Equals(reference1.PackageId, reference2.PackageId, StringComparison.OrdinalIgnoreCase),
            reference => reference.PackageId.GetHashCode())
          );

        var assemblyNameInfo = AssemblyNameInfo.Parse(mainAssembly.FullName);
        using (var loader = new MetadataLoader(productBinariesDir))
        {
          ProcessAssembly(packages, productBinariesDir, loader, assemblyNameInfo, packageFiles, packageReferences);
        }
        var packageArtifact = new ApplicationPackageArtifact(new SubplatformName(assemblyNameInfo.Name), new JetSemanticVersion(assemblyNameInfo.Version),  CompanyInfo.Name, CompanyInfo.NameWithInc, DateTime.UtcNow, null, null, packageFiles, packageReferences);
        return new AllAssembliesOnPackages(packages.Subplatforms.Concat(new SubplatformOnPackage(packageArtifact, null)).AsCollection());
      })));

      return base.CreateJetHostPackages(engine);
    }

    private static void ProcessAssembly(AllAssemblies allAssemblies, FileSystemPath productBinariesDir, MetadataLoader metadataLoader, AssemblyNameInfo assemblyNameInfo, HashSet<ApplicationPackageFile> packageFiles, HashSet<ApplicationPackageReference> packageReferences)
    {
      var assembly = metadataLoader.TryLoad(assemblyNameInfo, JetFunc<AssemblyNameInfo>.False, false);
      if (assembly == null) return;

      var subplatformOfAssembly = allAssemblies.FindSubplatformOfAssembly(assemblyNameInfo, OnError.Ignore);

      if (subplatformOfAssembly != null)
      {
        var subplatformReference = new ApplicationPackageReference(subplatformOfAssembly.Name, subplatformOfAssembly.GetCompanyNameHuman());
        packageReferences.Add(subplatformReference);
        return;
      }

      if (assembly.AssemblyName.Name.Contains("Microsoft.CodeAnalysis")) return;
      if (!packageFiles.Add(new ApplicationPackageFile(assembly.Location.MakeRelativeTo(productBinariesDir), assemblyNameInfo)))
        return;

      foreach (var referencedAssembly in assembly.ReferencedAssembliesNames)
      {
        ProcessAssembly(allAssemblies, productBinariesDir, metadataLoader, referencedAssembly, packageFiles, packageReferences);
      }
    }
  }
}
