![ForTea Logo](https://raw.github.com/MrJul/ForTea/master/Logo/ForTea%2032x32.png "ForTea Logo") ForTea
======

What's ForTea?
--------------
ForTea is a a plugin for [JetBrains ReSharper](http://www.jetbrains.com/resharper/) that adds support for editing T4 (.tt) files.  
This project corresponds to the issue [RSRP-191807](http://youtrack.jetbrains.com/issue/RSRP-191807) in JetBrains bug tracker.  
For the latest version, please see the [Releases](https://github.com/MrJul/ForTea/releases).  
Don't hesitate to [open an issue](https://github.com/MrJul/ForTea/issues) if you encounter any problem.

Installation
------------
Visual Studio 2010, 2012, 2013, 2015 and 2017 are supported.  
ReSharper 2017.2 must be installed (older releases are still available for ReSharper 8.2, 9.x, 10.0, 2016.x and 2017.1).  
To install ForTea, use Extension Manager from the ReSharper menu.


What's supported
----------------
 - Editing for .tt and .ttinclude and .t4 files
 - Syntax highlighting for T4 directives and blocks
 - Support for include files, both local and global to Visual Studio
 - T4 directives support and completion
 - Syntax highlighting for C# code
 - ReSharper automatic, basic and smart completion for C# code
 - File structure window support for T4 files
 - Everything you might expect from ReSharper in C# code: find usages, refactorings, etc.
 - ReSharper C# context actions and quick fixes
 - Support for adding assembly and import directives through quick fixes
 - Extending selection
 - T4 error highlightings and quick fixes
 - Auto update support
 - Both classic templates and runtime (aka preprocessed) templates.

Things to know
--------------
ForTea is only a ReSharper plugin at the moment, and doesn't provide any Visual Studio service,
meaning there are some limitations.
Amongst those, syntax highlighting is fully handled by ReSharper rather than Visual Studio:
to get coloring for identifiers, you must enable _Color identifiers_ in _ReSharper Options > Code Inspection > Settings_.
Plus, there is no code outlining support yet.

Concerning ReSharper support, a custom code formatter for T4 files hasn't been written yet.
You can use code cleanup on .tt files and the C# formatting rules configured in ReSharper options
will be used automatically. However, there is currently no way to overwrite them, and there are no
rules for T4 specific elements, such as placement of opening and closing blocks.

Visual Basic T4 files aren't supported yet.  
Custom T4 directives aren't supported yet.  

Licensed under [Apache License 2.0](http://www.apache.org/licenses/LICENSE-2.0)
