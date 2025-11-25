# GitHub Copilot Instructions for RimWorld "Rats!" Mod

## Mod Overview and Purpose

The "Rats!" mod introduces a dynamic and immersive ecosystem event into RimWorld where rats will spawn to consume any unattended food or corpses that are not kept refrigerated, simulating a real-world scenario. This encourages players to manage their food storage and cleanliness within the colony more effectively. Additional features include compatibility with existing rat mods (e.g., FancyRats and SnowRats), customization options for spawning mechanics, and integration with other mods for enhanced gameplay experiences, such as corpse-based farming.

## Key Features and Systems

- **Dynamic Rat Spawning:** Rats appear to consume food and corpses that are not refrigerated, adding an additional layer of challenge to resource management.
- **Mod Compatibility and Expansion:** Integrates with FancyRats and SnowRats for more rat varieties, with the possibility to add more through patches.
- **Customization Options:** Includes settings to dictate animal spawning behaviors based on biome and location (inside or outside), and restrict spawning to only corpses or non-corpses.
- **Additional Mod Synergies:** Works in tandem with the "Some Like It Rotten" mod for rat farming through corpses, and with "Cats Hunt For Fun" to naturally manage the rat population.
- **Performance Optimization:** Enhanced efficiency and reduced performance impact, contributed by Taranchuk.

## Coding Patterns and Conventions

- **Assembly Info Management:** Utilization of the appropriate `AssemblyInfo.cs` files for the different .NET Framework versions ensures compatibility.
- **Class Structures:** Follows a clean and modular class structure with each class responsible for specific functionalities related to rat tracking, extension, and mod settings management.

## XML Integration

The mod configuration uses XML for defining additional rat types, mod settings, and integrating with other mods. XML is used effectively to manage and update the behavior and types of rats through DefModExtensions and other configuration entries.

## Harmony Patching

Though not explicitly detailed in the file summary, Harmony patching is assumed to be utilized for modifying RimWorld's base game behavior, allowing for seamless integration of the spawning system without direct modification of the game's source code.

## Suggestions for Copilot

To enhance development efficiency using GitHub Copilot:

- **Pattern Recognition:** Leverage Copilot's ability to suggest code snippets for common RimWorld modding tasks, such as DefModExtension design and MapComponent implementations.
- **Boilerplate Generation:** Use Copilot to generate boilerplate code for new features or settings as per the ModSettings and GameComponent patterns shown in the existing files.
- **Optimization Tips:** Utilize suggestions for optimizing code performance, especially for frequently called methods and components interacting with the map.
- **Debugging Assistance:** Get help from Copilot for debugging through proposed bug-fix patterns and logging mechanisms to streamline troubleshooting efforts.

This guidance document should provide the necessary overview and instructions to utilize GitHub Copilot effectively in the enhancement and maintenance of the "Rats!" mod for RimWorld.
