import com.jetbrains.rd.generator.gradle.RdgenParams
import org.gradle.api.tasks.testing.logging.TestExceptionFormat
// import org.jetbrains.grammarkit.tasks.GenerateLexer
import org.jetbrains.intellij.tasks.PrepareSandboxTask
import org.jetbrains.kotlin.daemon.common.toHexString
import org.jetbrains.kotlin.gradle.tasks.KotlinCompile

buildscript {
  repositories {
    maven { setUrl("https://cache-redirector.jetbrains.com/www.myget.org/F/rd-snapshots/maven") }
    maven { setUrl("https://cache-redirector.jetbrains.com/dl.bintray.com/kotlin/kotlin-eap") }
    mavenCentral()
  }
  dependencies {
    classpath("com.jetbrains.rd:rd-gen:0.192.2")
    classpath("org.jetbrains.kotlin:kotlin-gradle-plugin:1.3.10")
  }
}

plugins {
  id("org.jetbrains.intellij") version "0.4.7"
  id("org.jetbrains.grammarkit") version "2018.1.7"
  kotlin("jvm") version "1.3.31"
}

apply {
  plugin("kotlin")
  plugin("com.jetbrains.rdgen")
  plugin("org.jetbrains.grammarkit")
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
  version = "2019.2-SNAPSHOT"
  instrumentCode = false
  downloadSources = false
  updateSinceUntilBuild = false
  // Workaround for https://youtrack.jetbrains.com/issue/IDEA-179607
  setPlugins("rider-plugins-appender")
}

val reSharperPluginName = "ReSharper.ForTea"
val riderPluginName = "Rider.ForTea"
val reSharperPluginSolutionName = "GammaJul.ReSharper.ForTea.sln"

val repoRoot = projectDir.parentFile!!
val reSharperPluginPath = File(repoRoot, reSharperPluginName)
val riderPluginPath = File(repoRoot, riderPluginName)
val reSharperPluginSolutionPath = File(reSharperPluginPath, reSharperPluginSolutionName)
 val buildConfiguration = ext.properties["BuildConfiguration"] ?: "Debug"

val pluginFiles = listOf(
  "output/$buildConfiguration/GammaJul.ReSharper.ForTea"
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
val riderSdkVersionPropsPath = File(reSharperPluginPath, "RiderSdkPackageVersion.props")

val riderForTeaTargetsGroup = "Rider.ForTea"

fun File.writeTextIfChanged(content: String) {
  val bytes = content.toByteArray()

  if (!exists() || readBytes().toHexString() != bytes.toHexString()) {
    println("Writing $path")
    writeBytes(bytes)
  }
}

configure<RdgenParams> {
  val csOutput = File(repoRoot, "ReSharper.ForTea/src/ForTea.ProjectModelBase/src/Protocol")
  val ktOutput = File(repoRoot, "Rider.ForTea/src/main/kotlin/com/jetbrains/fortea/protocol")

  verbose = true
  hashFolder = "build/rdgen"
  logger.info("Configuring rdgen params")
  classpath({
    logger.info("Calculating classpath for rdgen, intellij.ideaDependency is ${intellij.ideaDependency}")
    val sdkPath = intellij.ideaDependency.classes
    val rdLibDirectory = File(sdkPath, "lib/rd").canonicalFile

    "$rdLibDirectory/rider-model.jar"
  })
  sources(File(repoRoot, "Rider.ForTea/protocol/src/kotlin/model"))
  packages = "model"

  generator {
    language = "kotlin"
    transform = "asis"
    root = "com.jetbrains.rider.model.nova.ide.IdeRoot"
    namespace = "com.jetbrains.rider.model"
    directory = "$ktOutput"
  }

  generator {
    language = "csharp"
    transform = "reversed"
    root = "com.jetbrains.rider.model.nova.ide.IdeRoot"
    namespace = "JetBrains.Rider.Model"
    directory = "$csOutput"
  }
}

tasks {
  withType<PrepareSandboxTask> {
    val files = pluginFiles.map { "$it.dll" } + pluginFiles.map { "$it.pdb" }
    val paths = files.map { File(reSharperPluginPath, it) }
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

//  val generateT4Lexer = task<GenerateLexer>("generateT4Lexer") {
//    source = "src/main/java/com/jetbrains/rider/ideaInterop/fileTypes/fsharp/lexer/_FSharpLexer.flex"
//    targetDir = "src/main/java/com/jetbrains/rider/ideaInterop/fileTypes/fsharp/lexer"
//    targetClass = "_FSharpLexer"
//    purgeOldFiles = true
//  }

  withType<KotlinCompile> {
    kotlinOptions.jvmTarget = "1.8"
    dependsOn("rdgen")
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
    dependsOn("rdgen", "writeNuGetConfig", "writeRiderSdkVersionProps")
    doLast {
      exec {
        executable = "dotnet"
        args = listOf("restore", reSharperPluginSolutionPath.canonicalPath)
      }
    }
  }

  create("buildReSharperPlugin") {
    group = riderForTeaTargetsGroup
    dependsOn("prepare")
    doLast {
      exec {
        executable = "msbuild"
        args = listOf(reSharperPluginSolutionPath.canonicalPath)
      }
    }
  }

  task<Wrapper>("wrapper") {
    gradleVersion = "4.10"
    distributionType = Wrapper.DistributionType.ALL
    distributionUrl =
      "https://cache-redirector.jetbrains.com/services.gradle.org/distributions/gradle-$gradleVersion-all.zip"
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
