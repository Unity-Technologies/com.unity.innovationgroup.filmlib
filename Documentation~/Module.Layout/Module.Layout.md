* Layout Switcher *

Editor layouts in Unity are created per-user and not saved with the
 project by default. This means that it is difficult to share a 
 ‘standard layout’ with members of your team.  
 
 This module provides a simple way to switch between Editor Layouts from code. 

In the case of Sherman, there is a custom ‘Film Layout’ that exposes 
all of our custom editor tools. The Main toolbar (described below) 
has a button that allows you to switch into the Film Layout, or you 
can access this from the Tools menu (Tools => Switch Film Layout).

* Creating your own custom layouts *

To create your own custom layouts, you can use the built-in functionality
included in the editor:

https://docs.unity3d.com/Manual/CustomizingYourWorkspace.html

These files are saved in the AppData folder:

ie: c:/Users/[my user]/AppData\Roaming\Unity\Editor-5.x\Preferences\Layouts

Simply copy the .wlt file for your layout into the 'LayoutConfigs' folder of the module:

/packages/com.unity.innovationgroup.filmlib/Editor/Module.Layout/LayoutConfigs

TODO: Currently this module only supports loading a specific 'FilmLayout.wlt' from the
above folder. In the future, we'll properly scan the layout folder for additional configs,
and likely provide for project-level (ie in the Assets folder) layout configs as well to 
allow you to create custom layouts for your team and share them through source control etc.
