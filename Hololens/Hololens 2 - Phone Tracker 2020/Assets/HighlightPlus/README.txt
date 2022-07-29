**************************************
*          HIGHLIGHT PLUS            *
* Created by Ramiro Oliva (Kronnect) * 
*            README FILE             *
**************************************


Quick help: how to use this asset?
----------------------------------

1) Highlighting specific objects: add HighlightEffect.cs script to any GameObject. Customize the appearance options.
  In the Highlight Effect inspector, you can specify which objects, in addition to this one, are also affected by the effect:
    a) Only this object
    b) This object and its children
    c) All objects from the root to the children
    d) All objects belonging to a layer

2) Control highlight effect when mouse is over:
  Add HighlightTrigger.cs script to the GameObject. It will activate highlight on the gameobject when mouse pass over it.

3) Highlighting any object in the scene:
  Select top menu GameObject -> Effects -> Highlight Plus -> Create Manager.
  Customize appearance and behaviour of Highlight Manager. Those settings are default settings for all objects. If you want different settings for certain objects just add another HighlightEffect script to each different object. The manager will use those settings.

4) Make transparent shaders compatible with See-Through effect:
  If you want the See-Through effect be seen through other transparent objects, they need to be modified so they write to depth buffer (by default transparent objects do not write to z-buffer).
  To do so, select top menu GameObject -> Effects -> Highlight Plus -> Add Depth To Transparent Object.

5) Static batching:
  Objects marked as "static" need a MeshCollider in order to be highlighted. This is because Unity combines the meshes of static objects so it's not possible to highlight individual objects if their meshes are combined.
  To allow highlighting static objects make sure they have a MeshCollider attached (the MeshCollider can be disabled).



Help & Support Forum
--------------------

Check the Documentation (PDF) for detailed instructions:

Have any question or issue?
* Email: contact@kronnect.com
* Support Forum: https://kronnect.com/support
* Twitter: @Kronnect

If you like Highlight Plus, please rate it on the Asset Store. It encourages us to keep improving it! Thanks!



Universal Rendering Pipeline
----------------------------

Customers can download a specific version of Highlight Plus designed for Universal Rendering Pipeline for free from our support forum on https://kronnect.com
Please sign up on the forum and send a pm to "Kronnect" or email to contact@kronnect.com to get access to the private board.

The Unity Asset Store currently does not allow you to select which package according to the pipeline to download so we have to offer this way so you can download it.


Future updates
--------------

All our assets follow an incremental development process by which a few beta releases are published on our support forum (kronnect.com).
We encourage you to signup and engage our forum. The forum is the primary support and feature discussions medium.

Of course, all updates of Highlight Plus will be eventually available on the Asset Store.



More Cool Assets!
-----------------
Check out our other assets here:
https://assetstore.unity.com/publishers/15018



Version history
---------------

Current version
- Added "Border Only" option to See-Through effect
- Outline/glow shape now properly cuts when partially occluded (instead of following the shape of the occluder)
- Adding a Highlight Effect component to a parent no longer deactivates highlighted children

Version 8.3
- Upgraded to Unity 2020.3.16
- [Fix] Fixed outline/glow render issue when MSAA is enabled

Version 8.2
- Added "Ignore Mask" option to glow. Can be used to render the glow effect alone
- [Fix] Fixed issue with new input system and highlight manager/trigger if no Event System is present in the scene
- [Fix] Fixed glow passes UI overlap in Unity 2021.3.3 due to reorderable array bug

Version 8.1
- Selection state is now visible in inspector (used only by trigger and manager components)
- [Fix] Fixed mobile input using the new input system
- [Fix] Fixed outline settings mismatch when using a combination of Highlight Trigger and Manager

Version 8.0
- Added SelectObject / ToggleObject / UnselectObject methods to Highlight Manager
- Added ability to control rendering order of effects (check documentation: Custom sorting section)

Version 7.9.1
- Default values for all effects are now 0 (disabled) except outline so desired effects must be enabled. This option allows you to ensure no extra/undesired effects are activated by mistake
- Redesigned Highlight Plus Profile editor interface
- Removed dependency of HighlightManager

Version 7.8
- Added outer glow blend mode option
- API: added OnObjectHighlightStart/End events to HighlightTrigger (check documentation for differences with similar events on Highlight Effect main script)
- [Fix] API: Fixed specific issues with SetTarget method when used on shader graph based materials that don't use standard texture names

Version 7.7.2
- [Fix] Fixed fade in/out issue when disabling/enabling objects

Version 7.7
- Added support for the new Input System
- [Fix] Fixes to the align to ground option of target fx effect

Version 7.6.2
- [Fix] VR: fixed target effect "Align to Ground" issue with Single Pass Instanced

Version 7.6.1
- [Fix] Fixed overlay animation speed issue

Version 7.6
- Added "Target FX Align to Ground" option
- Added isSeeThroughOccluded(camera). Is true when any see-through occluder using raycast mode is blocking the see-through effect
- All shader keywords are now of local type reducing global keyword usage
- Fixes and improvements to see-through when combined with outline/outer glow

Version 7.5.2
- [Fix] See-through is now visible when using glow/outline/inner glow with Always Visible option

Version 7.5
- Added new HitFX style: "Local Hit"
- Added new demo scene showcasing the HitFx variations
- Added "Overlay Texture" option
- Added "Min Distance" option to Highlight Manager and Highlight Trigger
- Added support for "Domain Reload" disabled option
- API: added OnObjectHighlightStart, OnObjectHighlightEnd events to HighlightManager
- [Fix] Fixed inner glow and overlay issue when MaterialPropertyBlock is used on the character material

Version 7.1
- Added "Respect UI" to Highlight Manager and Trigger which blocks interaction if pointer is over an UI element

Version 7.0.2
- Memory optimizations

Version 7.0.1
- [Fix] Highest quality outline/glow fixes on mobile

Version 7.0
- Added support for Single Pass Instanced
- Internal improvements and fixes

Version 6.9
- Internal improvements to see-through

Version 6.8
- Changed see-through effect rendering order to improve support with other stencil effects
- [Fix] Fixed properties not being reflected in scene immediately when invoking Undo

Version 6.7
- Added "SeeThrough Max Depth" option. Limits the visibility of the see-through effect to certain distance from the occluders
- Added "SeeThrough Check Individual Objects" option. If enabled, occlusion test is performed for each individual child of the object, instead of using combined bounds

Version 6.6
- Added "SeeThrough Depth Offset" option. This option allows you to control the minimum distance from the occluder to the object before showing the see-through effect
- Added "SeeThrough Non Overlap" option. Enable it only if the see-through effect produces flickering due to overlapping geometry in the hidden object
- [Fix] Fixed properties not being reflected in scene immediately when invoking Undo

Version 6.5.1
- Calling ProfileLoad() method will now assign that profile to the highlight effect component in addition to loading its values
- Prevents _Time overflow which can cause glitching on some Android devices

Version 6.5
- Name filter now is ignored when effect group is set to Only This Object
- New shader "HighlightPlus/Geometry/UIMask" to cancel highlight effects when rendering through a UI Canvas (see documentation)

Version 6.4
- Added "Cameras Layer Mask" to specify which cameras can render the effects
- Hit FX color in Highlight Profile now exposes HDR color options

Version 6.3.1
- Added "Single Selection" option to Highlight Manager/Trigger
- Added "Toggle" option to Highlight Manager/Trigger
- Selection is cleared now when clicking anywhere in the scene (requires Highlight Manager)
- API: added SetGlowColor
- Improved Highlight Manager inspector

Version 6.2
- Added TargetFX Scale To Object Bounds (defaults to false)
- Added support for HDR color to Hit FX color field
- Option to list occluders in the inspector when See Through Occluder Mask "Accurate" option is enabled

Version 6.1
- Added more accurate occluder layer system ("Accurate" option)
- Added default hit fx settings to inspector & profile
- Added hit fx modes (overlay or inner glow)

Version 6.0
- Added Selection feature
- Inspector: sections can be now collapsed to reduce screen space
- API: added OnObjectSelected / OnObjectUnSelected events

Version 5.5
- Added "Planar" mode to Normals option. Best choice for highlighting 2D meshes (quad/planes)

Version 5.4 5/Feb/2021
- Added Visibility option to targete effect
- Stencil mask is no longer computed when only overlay or inner glow is used improving performance

Version 5.3.5 22/Jan/2021
- Added "CustomVertexTransform.cginc" file which can be used to include user-defined vertex transformations
- Optimizations to material setters

Version 5.3.4
- Improvements to combine meshes option

Version 5.3.3
- Effects now reflect object transform changes when combines meshes option is enabled

Version 5.3.2
- Memory optimizations

Version 5.3.1
- Optimizations and fixes

Version 5.3
- Added "Combine Meshes" option to profile

Version 5.2
- Added "Object Name Filter" option to profile

Version 5.0
- API: added "TargetFX" method to programmatically start the target effect  
- [Fix] Depth Clip option can now be used on mobile even with visibility set to Always On Top

Version 4.9
- Added "Medium" quality level

Version 4.8.1
- [Fix] Fixed outline/glow issue on iOS when using Highest Quality mode in Unity 2010.1

Version 4.8
- Added "Outer Glow Blend Passes" option
- Added support for HDR colors

Version 4.7
- Added "Normals Option" with Smooth, Preserve and Reorient variants to improve results
- Target effect now only renders once per gameobject if a specific target transform is specified
- API: added OnTargetAnimates. Allows you to override center, rotation and scale of target effect on a per-frame basis.

Version 4.6
- Added "SubMesh Mask" which allows to exclude certain submeshes
- [Fix] Fixed shader compilation issue with Single Pass Instanced mode enabled

Version 4.5
- Added "Preserve Original Mesh" option to inspector and profile

Version 4.4
- Added HitFX effect
- Improved quality of outer glow when using Highest Quality mode
- Improvements to SeeThrough Occluder when Detection Mode is set to RayCast
- API: added SetTargets(transform, renderers)
- API: added static method HighlightEffect.DrawEffectsNow() to force render all effects on demand

Version 4.2
- Added GPU Instancing support for outline / glow effects
- Highlight Trigger: added volume collision detection

Version 4.1.1
- [Fix] Fixed issue with grouped objects when independent option is enabled and Highest Quality outer glow or outline is used

Version 4.1
- Improved "Outline Independent" option for Highest Quality Mode
- Consistency: enabling "Outline Independent" in Highest Quality Mode now also affects Outer Glow is used

Version 4.0
- Start up peformance & memory allocation optimizations
- Added "Independent" support to outline in Highest Quality mode
- Added "Make Transparent Object Compatible With Depth Clip" option

Version 3.9
- Added "Depth Clip" option (only applies to HQ outline/glow effects)

Version 3.8
- Glow/Outline downsampling and glow blend mode option added to profiles
- [Fix] Fixed an issue which marked the scene as dirty
- [Fix] Removed VR API usage console warning

Version 3.7
- See Through: added "Occluder Mask" option. When set to a custom layer, it performs a BoxCast check to ensure only objects in the specific layers are occluding the target. Customize this behaviour using the Radius Threshold and Check Interval settings.
- Added "Max Distance" to Highlight Trigger
- Reduced allocations when averaging normals
- [Fix] Fixed flickering of outer glow when used in Highest quality with thin objects

Version 3.6
- Added "Outline Independent" option. Shows full outline regardless of any other highlighted object behind.

Version 3.5
- Improved quality of Outline effect when quality level is set to Highest
- Added "SeeThrough Border" feature 
- Added "Blend Mode" option to Outer Glow for highest quality level
- [Fix] Fixed issue during Prefab editor mode

Version 3.4.4
- Added option in occluder script to use raycast instead of stencil buffer to cancel see-through (useful for avoiding terrain triggering see-through effect)
- [Fix] Fixed see-through in deferred rendering path

Version 3.4.2
- [Fix] Fixed an issue when adding the effect at runtime with outline/glow in higuest quality

Version 3.4.1
- [Fix] Fixed occluder objects removing glow effect when placed in the background
- [Fix] Added missing SeeThrough noise option to Highlight Profile asset

Version 3.4
- Added LayerInChildren option to "Include" filter
- Improved occluder system
- [Fix] Prevent an error when the mesh normals count does not match the vertex count

Version 3.3
- Outline, Glow and Inner Glow "Always On Top" option expanded to "Normal", "Always On Top" or "Only When Occluded"
- Added Noise slider to See-Through effect

Version 3.2.4
- [Fix] Fixed flickering issue when combining mesh & skinned mesh renderers

Version 3.2.3
- General improvements and fixes

Version 3.2.2
- [Fix] Fixed issue when trying to read normals from a non-readable mesh

Version 3.2.1
- [Fix] Fixed glow disappearing when object crosses camera near clip

Version 3.2
- Added "Reflection Probes" option
- Internal improvements and fixes

Version 3.1
- Added "Constant Width" option to Glow/Outline in Fastest/High quality level
- Added "Optimal Blit" option to Glow/Outline with Debug View

Version 3.0.2
- API: added proxy methods ProfileLoad, ProfileReload, ProfileSaveChanges to load/store profile settings at runtime. You can also load/save changes between effect and profile using the Load/Save methods on the profile object itself.

Version 3.0.1
- [Fix] Fixed an exception when glow was enabled, outline disabled in highest quality level

Version 3.0
- Added HQ Highest quality glow and outline options
- Added "Include" option to choose which objects are affected by the effects (same gameobject, children, root to children, or many objects in layer)
- Added "Alpha CutOff"

Version 2.6.1
- Minor internal improvements

Version 2.6
- Added Target effect
- Improved performance on Skinned Mesh Renderers. Slightly improved performance on normal renderers.

Version 2.5.2
- [Fix] Fixed issue with HQ Outer Glow not showing when there's multiple selected objects parented to the same object

Version 2.5.1
- Added support for orthographic camera

Version 2.5
- Added support for VR Single Pass Instanced
- Minor improvements and fixes

Version 2.4
- New HighlightSeeThroughOccluder script. Add it to any object to cancel any see-through effect
- Added "Fade In Duration" / "Fade Out Duration" to create smooth transition states
- Added "Glow HQ" to produce better outer glow on certain shapes
- Added "OnRendererHighlightStart" event
- API: added "OverlayOnShot" method for impact effects

Version 2.3
- Added "Raycast Source" to Highlight Trigger and Manager components
- Added "Skinned Mesh Bake Mode" to optimize highlight on many models

Version 2.2
- Added "Always On Top" option to Outline, Outer and Inner Glow
- Added "Trigger Mode" to Highlight Trigger to support complex objects

Version 2.1
- Added "Outline HQ" to inspector. Creates a better outline on certain shapes
- Added "Ignore Object Visibility" to enable effects on disabled renderers or hidden objects

Version 2.0
- Profiles. Store/load/share settings across different objects.
- [Fix] Fixed issue when copying component values between two objects
- [Fix] Fixed effects ignoring culling mask on additional cameras

Version 1.5
- Added "Inner Glow" effect

Version 1.4
- Added "Overlay Min Intensity" and "Overlay Blending" options
- Added "Ignore" option
- Minor improvements & fixes

Version 1.3
- Added option to add depth compatibility for transparent shaders

Version 1.2.4
- [Fix] Fix for multiple skinned models
- [Fix] Fix for scaled skinned models

Version 1.2.3
- [Fix] Fixes for Steam VR

Version 1.2.1
- Internal improvements and fixes

Version 1.2.1
- [Fix] Fixed script execution order issue with scripts changing transform in LateUpdate()

Version 1.2
- Support for LOD groups

Version 1.1
- Redesigned editor inspector
- Minor improvements

Version 1.0.4
- Supports meshes with negative scales

Version 1.0.3
- Support for multiple submeshes

Version 1.0.2
- [Fix] Fixed scale issue with grouped objects

Version 1.0.1
- Supports combined meshes

Version 1.0 - Nov/2018
- Initial release
