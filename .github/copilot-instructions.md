# GitHub Copilot Instructions for Rats! Mod

## Mod Overview and Purpose
Rats! is a RimWorld mod that introduces a new feature where rats will spawn to eat unattended food or corpses that are not kept refrigerated. This mod enhances the realism of the game by simulating a common real-life scenario. The mod is compatible with other mods like FancyRats and SnowRats, and users can add more animals via patches. There are also mod options to select other animals, control their spawning based on biome, and limit their spawning to either corpses or non-corpses.

## Key Features and Systems
- **Rat Spawning**: Rats spawn to eat unattended food or corpses that are not refrigerated.
- **Compatibility**: Works with FancyRats and SnowRats mods.
- **Mod Options**: Users can select other animals, control their spawning based on biome, and limit their spawning to corpses or non-corpses.
- **Integration with Other Mods**: Works well with Some Like It Rotten for creating a corpse-based rat farm.
- **Performance Optimization**: Performance optimizations are provided by Taranchuk.

## Coding Patterns and Conventions
- **C# Code**: The mod primarily uses C# for its functionality. Key classes and methods are:
  - `TotalRatsTracker`: Manages the tracking of total rats.
  - `GameComponent_TotalRatTracker`: Manages game components related to rats.
  - `RatExtension`: Extends the functionality of rats.
  - `Rats`: Main class for the rats mod, handles logging and updating available rats.
  - `RatsModSettings`: Handles mod settings.
- **XML Integration**: XML is used for defining patches and other configurations.
- **Harmony Patching**: Harmony patching is used to modify existing game behavior.

## XML Integration
- **Core.xml**: Contains core patches for the mod.
- **FancyRats.xml**: Contains patches for compatibility with FancyRats.
- **SnowRats.xml**: Contains patches for compatibility with SnowRats.
- **List.xml**: Contains personal favorites list configurations.

## Harmony Patching
Harmony patching is used to modify existing game behavior. Key patches include:
- **Rat Spawning**: Patches to modify the spawning behavior of rats.
- **Compatibility Patches**: Patches to ensure compatibility with other mods like FancyRats and SnowRats.

## Suggestions for Copilot
- **C# Code**: Use Copilot to generate and complete C# code snippets, particularly for new features or bug fixes.
- **XML Integration**: Use Copilot to generate XML patches and configurations for new compatibility or features.
- **Harmony Patching**: Use Copilot to generate Harmony patches for modifying existing game behavior.
- **Mod Options**: Use Copilot to generate code for new mod options and their functionality.
- **Performance Optimization**: Use Copilot to suggest performance optimization techniques and code improvements.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.


## Hard rules (must follow)
- Do NOT run commands that modify the repo (no git commit, git apply, dotnet format) unless explicitly asked.
- Prefer minimal reads: read only the smallest code region needed (around the suspicious lines).
- When mentioning SonarQube issues, automatically use the SonarQube MCP service to fetch and address issues instead of making inferred fixes without querying SonarQube first.
- When mentioning the rimworld log, automatically use the Rimworld MCP service to fetch the log.

