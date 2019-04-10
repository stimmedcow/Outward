# ExtendedHotbar
A mod to extend the hotbar by 8 slots and improve the GUI experience when using these changes.

# Previews
https://imgur.com/a/21lI6O3 (New)

https://youtu.be/iJV-L4mDNF4 (Old)

# About
This is **not** a [Partiality](https://github.com/PartialityModding) mod!

This is a set of IL edits, via dnSpy, to modify the core game to having 16 hotbar slots rather than 8 when using a keyboard. Controller support is not planned for obvious reasons.

I've done basic testing and have found no issues so far, but I've also not tried it in multiplayer yet. My guess would be the host and client should both have the mod, but it's something I'll look into later most likely.

It is being provided as-is to allow other modders to make use of the method to achieve similar, or provide this functionality via another modding framework, such as Partiality.

The purpose of this project is twofold (excluding actually extending the hotbar):
1. To get familiar with the inner workings of the game for further modding purposes
2. To start identifying core edits required for a generalized modding framework (think Minecraft Forge)

#1 is pretty self explanatory.

#2 requires some elaboration.

While this mod adds 8 more hotbar slots, it also does a few more things to make the new hotbar slots usable:
* Adds new input actions to ReWired so the slots can be assigned hotkeys
* Adds localization entries for the slot names for pretty display in the options menu
* Adds input processing logic so the new input actions actually do something (in this case, trigger hotbar slots)

In the current setup, these changes are specific to this mod, and anyone who wants to do similar, will also do the same, potentially causing conflicts or breaking logic due to the nature of the changes done. This is not ideal but is necessary to pave the way forward.

Since this mod shows an example of adding new actions, localization, and the input processing to tie things together, we can began to generalize this functionality into a new modding framework that makes these core changes, but in a way to allow other mods to add logic to it.

For example, rather than manually adding hardcoded strings to the localization manager in the client assembly, we can instead make a set of changes to call a core modding framework's "LoadLocalization" function, which then exposes a way for mods to add localization data without them each needing to edit the game files.

By doing this, we now have the first cornerstone of the modding framework done, and have created the ability for mods to add localizations to the game in a non-conflicting way. This is the general direction I want my modding framework to head (if I stick with Outward modding). Ideally, a lot of game edits would be done with this in mind, to allow changing of core functionality as well, but that is a much bigger task that will take some time.

To summarize, my approach for modding is to do specific core changes, and then look into generalizing the changes that allows them to work to expose that functionality for other mods via a modding framework. The aforementioned modding framework doesn't exist yet, though.

# Process

This is a summary of what this mod does and how its currently accomplished. It is not the process I used to develop this mod, and it does not follow the development order of how things were done.

As the game updates, the specifics of this section will change, but there will be old/new code snippets to compare against to understand what is going on in the future. Making use of the code changes does require some C# knowledge and basic dnSpy experience. It's not too hard to figure out, but if you're brand new, you might run into some struggles.

NOTE: After you perform an edit, you need to save the module, as later changes require the assembly to have been changed with earlier edits so the code compiles correctly!

NOTE: In most cases, you will want to simply edit the relevant method (Edit Method (C#)...). Only a few edits required you to actually edit the entire class (Edit Class (C#)...) in dnSpy (#4, #7, #10, #11)

NOTE: I use CodeCompare, but use any diff program to check old vs new files to get a clear idea of what's changed more easily!

## 1. Add new InputAction objects to ReWired to support the new hotbar actions and log the current action list to get useful info.

The base action data exists somewhere, as objects get loaded that contain it (possibly through deserialization), but I'm not familiar enough with Unity to know how to access this before the ReWired library gets created. As a result, I'm modifying the ReWired_Core assembly to inject new actions into the library so the game has them as it would everything else.

In addition, I'm also logging all registered actions to know their ids to be able to refer to them later. This is useful to make sure everything makes sense afterwards and is consistent.

Assembly: `Rewired_Core.dll`

Old: `1. Rewired-InputManager_Base-Initialize-Old.cs`

New: `1. Rewired-InputManager_Base-Initialize-New.cs`

## 2. Add Localization entries for the new hotbar actions

With the new ReWired actions added, we now need to tell the game what to display when those action ids are processed. The default id will be something like InputAction_QS_Instant12 for the first action (read above for why it starts at 12), so we can just loop through and assign pretty names for it. Since we really want the actions to correspond to quickslot 9+, we can easily do that here.

Assembly: `Assembly-CSharp`

Old: `2. LocalizationManager-StartLoading-Old.cs`

New: `2. LocalizationManager-StartLoading-New.cs`

## 3. Add new QuickSlotIDs

Since we want to add 8 new slot ids, we need to update the QuickSlotIDs enum to have additional values to reference in our code. We might be able to get away with not doing this, but then you'd need to cast away enums in the future, and that just makes things more messy.

I'm not sure if there's an easier way to modify only an enum in dnSpy, but the entire class has to be edited at once from what I saw. Basically, just check the QuickSlotIDs enum at the very end of the file when applying this change and don't copy anything else.

Assembly: `Assembly-CSharp`

Old: `3. QuickSlot-QuickSlotIDs-Old.cs`

New: `3. QuickSlot-QuickSlotIDs-New.cs`

## 4. Register the new QuickSlotIDs with the KeyboardQuickSlotPanel

With the new quickslot ids added, we need to register them with the KeyboardQuickSlotPanel class so they can get displayed.

The idea here is to leverage how the game is already designed, and just add more data for it to process, as the code can support any number of slots, barring display issues.

Assembly: `Assembly-CSharp`

Old: `4. KeyboardQuickSlotPanel-InitializeQuickSlotDisplays-Old.cs`

New: `4. KeyboardQuickSlotPanel-InitializeQuickSlotDisplays-New.cs`

## 5. Register the new InputActions with the ControlMappingPanel

I think this section might be possible to add to ReWired as well, but I'm doing it here because ReWired is obfuscated, so making certain changes is more work than simply doing it in Assembly-CSharp.

The idea here is to modify the "QuickSlot" section in the ControlMappingPanel to add our new actions to the GUI so the user can rebind them. If we don't do this, we'd have no way of actually using the new actions in game with the quickslots (unless we modified the input settings xml, which is pretty tedious)!

Assembly: `Assembly-CSharp`

Old: `5. ControlMappingPanel-InitMappings-Old.cs`

New: `5. ControlMappingPanel-InitMappings-New.cs`

## 6. Add the new quickslots to the CharacterQuickSlotManager

We can now tell the CharacterQuickSlotManager that we have 8 new slots the player can use. The client code is setup to process any new slots as long as they were setup a specific way (child objects with a QuickSlot component). We name the QuickSlot as the code will process it as shown later on in the function. Once again, we start with 12 to avoid conflicts with 9 - 11 already being used.

Assembly: `Assembly-CSharp`

Old: `6. CharacterQuickSlotManager.Awake-Old.cs`

New: `6. CharacterQuickSlotManager.Awake-New.cs`

## 7. Add a new function to ControlsInput to see if our new InputActions were pressed

I've added a function called QuickSlotInstantX, which takes in a number and builds the QS_InstantX action string required to be passed to ReWired to see if our new action was triggered with whatever key is bound to it. This is preferable to adding 1 input function per action in this case.

You can find the QuickSlotInstantX function at the end of the code listing, but as with editing an enum, you must edit the entire class to add a function via code, so that's why the listing is so big. You only need to care about the new function added in the future, as any changes between everything else in the class won't matter when applying this change to a future version.

Assembly: `Assembly-CSharp`

Old: `7. ControlsInput-Old.cs`

New: `7. ControlsInput-New.cs`

## 8. Add new input logic to LocalCharacterControl for quickslots when the InputAction is triggered.

Our last main step is to add input checking logic to UpdateQuickSlots to check for our new InputActions, and trigger the correct quickslot if the bound input key is pressed.

Assembly: `Assembly-CSharp`

Old: `8. LocalCharacterControl-UpdateQuickSlots-Old.cs`

New: `8. LocalCharacterControl-UpdateQuickSlots-New.cs`

## 9. Reposition the QuickSlotPanel so it's not overlapping the StabilityDisplay_Simple object

With new quickslots taking up more screen space, there's an ugly overlap with the stability bar. This change will reposition the quickslot bar so it's nicely placed above the stability bar. There's still a possible overlap issue with buff icons on the far left side of the screen though.

Assembly: `Assembly-CSharp`

Old: `9. QuickSlotPanel-Update-Old.cs`

New: `9. QuickSlotPanel-Update-New.cs`

## 10. Update the StatusEffectIcon class to support displaying a timer under the icon

One of the features I most wanted was to display the remaining time of a buff/debuff under the icons. This and the next set of changes help accomplish that. Eventually, a config option will be added to enable/disable the text if users don't want this feature.

This set of changes requires a class edit, so be sure to note all the changes in the entire class.

Assembly: `Assembly-CSharp`

Old: `10. StatusEffectIcon-Old.cs`

New: `10. StatusEffectIcon-New.cs`

## 11. Update the StatusEffectPanel class to change the layout and position of icons

With the hotbar taking up most of the bottom of the screen, we want a more natural display for those icons, which the top left corner of the screen has typically been used in a lot of games. I prefer that location myself as well, so this set of changes moves the panel and changes the wrapping, so icons are displayed across the top of the screen. We'll eventually need to adjust the position of the compass, so that will be added in a future change.

NOTE: It might be possible for more icons that extend past the screen to display off-screen, but I'll look into fixing that if it becomes an issue. For now, on average, I don't think it's common to fill the entire width of the screen even on smaller resolutions, as the lowest I can test is 1152 on my system and it still looked good.

This set of changes requires a class edit, so be sure to note all the changes in the entire class.

Assembly: `Assembly-CSharp`

Old: `11. StatusEffectPanel-Old.cs`

New: `11. StatusEffectPanel-New.cs`

## 12. Reposition UICompass so it's not overlapping the StatusEffectPanel

Once StatusEffectPanel has an icon and we know its height, we'll move the UICompass down so it doesn't overlap anymore.

Assembly: `Assembly-CSharp`

Old: `12. UICompass-Update-Old.cs`

New: `12. UICompass-Update-New.cs`

# Final Remarks

This might seem very daunting at first, but once you understand the ideas behind the changes and get familiar with dnSpy, this mod only takes a few minutes to apply. Likewise, it only adds 8 slots, but if you wanted more or less, the code should be clear enough as to how to accomplish that. However, adding more slots with smaller resolutions begins to show some issues, so additional mods to modify the GUI are still needed.

I wrote this document and performed the edits at the same time, so there should not be any mistakes in the changes listed. Upon finishing the edits, you should be able to launch Outward and be able to bind the new actions in settings, and then assign items/skills in game!

Future work, if I stick with Outward modding, would be to move a lot of these changes into a modding framework to start building around, or at least automate the process of applying these mods to a clean Assembly-CSharp/Rewired_Core.

Since we're in the early days of Outward modding, I feel this information is more useful, so that's why I've taken the time to write out this modding process for others to understand and perhaps get more ideas of what can be done in the game.
