#Timeline Toolbar

The Timeline Toolbar is a very simple custom Editor window that includes preset shortcuts to game objects in the loaded scenes. It also exposes the ‘Detail settings’ options provided by Module.RenderSettings.

* Note: the name is misleading (and should probably be changed) - this toolbar doesn't actually have much to do with Timeline, but is named as a result of it's position - we typically dock this EditorWindow above the Timeline window as it provides common shortcuts & helpers that are accessed while working on a scene or timeline sequence.*

> If you are using the MWU HDRP Branch (which provides the additional Fur render pass), then the Performance Overrides section becomes visible

[insert toolbar image]

From left to right:

**Scene Shortcuts**
- Master Timeline - finds & selects the ‘MasterTimeline’ gameobject in any open scene
- Scene Settings - finds & selects the ‘SceneSettings’ gameobject in any open scene
- Global Post - finds & selects the ‘PostVolume’ gameobject in any open scene

**Render Settings**
- Detail levels - see documentation for Module.RenderSettings [TODO: Insert link]
- Edit Settings - shortcut to edit the available detail levels for the current scene

**Performance overrides**
*only shown if you have #USING_MWU_HDRP enabled in your player settings

- Fur Shell count - allows you to adjust the number of ‘shells’ in the fur rendering (lower is faster, higher is better quality)
- Update Fur - if you adjust the shell count slider, this will update the HDRP settings for the fur