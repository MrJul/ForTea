ForTea
======

What's ForTea?
--------------

ForTea is a a plugin for ReSharper 7.1 that adds support for editing T4 (.tt) files.  
This project corresponds to the issue [RSRP-191807](http://youtrack.jetbrains.com/issue/RSRP-191807) in JetBrains bug tracker.

Installation
------------
Visual Studio 2010 or Visual Studio 2012 is required.  
[ReSharper 7.1.x](http://www.jetbrains.com/resharper/) must be installed.
If you have the T4 editor from Tangible, you must uninstall it. For technical reasons, ForTea will never be compatible with it.
Grab the latest binary version of ForTea (1.0.0) [here](http://download.flynware.com/ForTea/ForTea-1.0.0.msi).

What's supported
----------------
 - Editing for .tt and .ttinclude files
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

Visual Basic T4 files aren't supported.

Licensed under [Apache 2.0 License](http://www.apache.org/licenses/LICENSE-2.0)