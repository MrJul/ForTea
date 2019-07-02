import org.gradle.api.tasks.testing.logging.TestExceptionFormat
import org.jetbrains.intellij.tasks.PrepareSandboxTask
import org.jetbrains.kotlin.daemon.common.toHexString
import org.jetbrains.kotlin.gradle.tasks.KotlinCompile

buildscript {
  repositories {
    maven { setUrl("https://cache-redirector.jetbrains.com/dl.bintray.com/kotlin/kotlin-eap") }
    mavenCentral()
  }
  dependencies {
    classpath("org.jetbrains.kotlin:kotlin-gradle-plugin:1.3.31")
  }
}

plugins {
  id("org.jetbrains.intellij") version "0.4.9"
  kotlin("jvm") version "1.3.31"
}

apply {
  plugin("kotlin")
}

repositories {
  mavenCentral()
  maven { setUrl("https://cache-redirector.jetbrains.com/dl.bintray.com/kotlin/kotlin-eap") }
}

java {
  sourceCompatibility = JavaVersion.VERSION_1_8
  targetCompatibility = JavaVersion.VERSION_1_8
}


version = "0.01"

intellij {
  type = "RD"
  version = "2019.1.2"
  instrumentCode = false
  downloadSources = false
  updateSinceUntilBuild = false
  // Workaround for https://youtrack.jetbrains.com/issue/IDEA-179607
  setPlugins("rider-plugins-appender")
}

val backendPluginName = "ReSharper.ForTea"
val riderBackedPluginName = "ForTea.RiderSupport"
val backendPluginSolutionName = "ReSharper.ForTea.sln"

val repoRoot = projectDir.parentFile!!
val backendPluginPath = File(repoRoot, backendPluginName)
val riderBackendPluginPath = File(repoRoot, riderBackedPluginName)
val backendPluginSolutionPath = File(backendPluginPath, backendPluginSolutionName)
val buildConfiguration = ext.properties["BuildConfiguration"] ?: "Debug"

val pluginFiles = listOf(
  "output/ForTea.Core/$buildConfiguration/ForTea.Core",
  "output/ForTea.RiderSupport/$buildConfiguration/ForTea.RiderSupport"
)

val nugetPackagesPath by lazy {
  val sdkPath = intellij.ideaDependency.classes

  println("SDK path: $sdkPath")
  val path = File(sdkPath, "lib/ReSharperHostSdk")

  println("NuGet packages: $path")
  if (!path.isDirectory) error("$path does not exist or not a directory")

  return@lazy path
}

val riderSdkPackageVersion by lazy {
  val sdkPackageName = "JetBrains.Rider.SDK"

  val regex = Regex("${Regex.escape(sdkPackageName)}\\.([\\d\\.]+.*)\\.nupkg")
  val version = nugetPackagesPath
    .listFiles()
    .mapNotNull { regex.matchEntire(it.name)?.groupValues?.drop(1)?.first() }
    .singleOrNull() ?: error("$sdkPackageName package is not found in $nugetPackagesPath (or multiple matches)")
  println("$sdkPackageName version is $version")

  return@lazy version
}

val nugetConfigPath = File(repoRoot, "NuGet.Config")
val riderSdkVersionPropsPath = File(backendPluginPath, "RiderSdkPackageVersion.props")

val riderForTeaTargetsGroup = "ForTea.Rider"

fun File.writeTextIfChanged(content: String) {
  val bytes = content.toByteArray()

  if (!exists() || readBytes().toHexString() != bytes.toHexString()) {
    println("Writing $path")
    writeBytes(bytes)
  }
}

tasks {
  withType<PrepareSandboxTask> {
    val files = pluginFiles.map { "$it.dll" } + pluginFiles.map { "$it.pdb" }
    val paths = files.map { File(backendPluginPath, it) }
    logger.lifecycle(paths.toString())

    paths.forEach {
      from(it) {
        into("${intellij.pluginName}/dotnet")
      }
    }

    into("${intellij.pluginName}/projectTemplates") {
      from("projectTemplates")
    }

    doLast {
      paths.forEach {
        val file = file(it)
        if (!file.exists()) throw RuntimeException("File $file does not exist")
        logger.warn("$name: ${file.name} -> $destinationDir/${intellij.pluginName}/dotnet")
      }
    }
  }

  withType<KotlinCompile> {
    kotlinOptions.jvmTarget = "1.8"
  }

  withType<Test> {
    useTestNG()
    testLogging {
      showStandardStreams = true
      exceptionFormat = TestExceptionFormat.FULL
    }
    val rerunSuccessfulTests = false
    outputs.upToDateWhen { !rerunSuccessfulTests }
    ignoreFailures = true
  }

  create("writeRiderSdkVersionProps") {
    group = riderForTeaTargetsGroup
    doLast {
      riderSdkVersionPropsPath.writeTextIfChanged(
        """<Project>
  <PropertyGroup>
    <RiderSDKVersion>[$riderSdkPackageVersion]</RiderSDKVersion>
  </PropertyGroup>
</Project>
"""
      )
    }
  }

  create("writeNuGetConfig") {
    group = riderForTeaTargetsGroup
    doLast {
      nugetConfigPath.writeTextIfChanged(
        """<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="resharper-sdk" value="$nugetPackagesPath" />
  </packageSources>
</configuration>
"""
      )
    }
  }

  getByName("assemble") {
    doLast {
      logger.lifecycle("Plugin version: $version")
      logger.lifecycle("##teamcity[buildNumber '$version']")
    }
  }

  create("prepare") {
    group = riderForTeaTargetsGroup
    dependsOn("writeNuGetConfig", "writeRiderSdkVersionProps")
    doLast {
      exec {
        executable = "dotnet"
        args = listOf("restore", backendPluginSolutionPath.canonicalPath)
      }
    }
  }

  create("buildReSharperPlugin") {
    group = riderForTeaTargetsGroup
    dependsOn("prepare")
    doLast {
      exec {
        executable = "msbuild"
        args = listOf(backendPluginSolutionPath.canonicalPath)
      }
    }
  }
}

defaultTasks("prepare")

// workaround for https://youtrack.jetbrains.com/issue/RIDER-18697
dependencies {
  testCompile("xalan", "xalan", "2.7.2")
  implementation(kotlin("stdlib-jdk8"))
}
val compileKotlin: KotlinCompile by tasks
compileKotlin.kotlinOptions {
  jvmTarget = "1.8"
}
val compileTestKotlin: KotlinCompile by tasks
compileTestKotlin.kotlinOptions {
  jvmTarget = "1.8"
}
