# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [0.2.0] - 2026-11-21

### Added
- Enum support. `Enum` is now a selectable type in the Builder. Choosing it shows an object selector in the new **Extra Type Info** column, where you assign the script that defines the enum. The generated field uses the enum's fully qualified name.
- Enum values are imported from the sheet by member name (case-sensitive) and shown as a dropdown in the Manager table.
- `EnumUtils` editor helper that resolves (and caches) the enum type defined by a script.

### Changed
- The Builder's **Type** column is half as wide to make room for **Extra Type Info**.

## [0.1.0] - 2026-10-30

### This is the first release of *Scriptable Objects From Sheets*.

Generate `ScriptableObject` classes and assets from a Google Sheet inside the Unity Editor.

### Dependencies
- Bundles the Google API client libraries (`Google.Apis`, `Google.Apis.Core`, `Google.Apis.Auth`, `Google.Apis.Sheets.v4`) and `System.Management`, Editor only. See `Third Party Notices.md`.
- Requires `com.unity.nuget.newtonsoft-json` and `com.unity.visualscripting`.
