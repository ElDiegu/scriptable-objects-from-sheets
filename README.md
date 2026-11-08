# Scriptable Objects From Sheets

Generate Unity `ScriptableObject` classes and assets straight from a Google Sheet, without leaving the Editor.

Point the tool at a spreadsheet, let it infer the column types, generate a ready-to-use `ScriptableObject` class, and then create or update one asset per sheet row.

## Features

- **Builder:** reads a sheet range, infers a type for each column, and generates the C# class. You can review and edit the generated fields before the file is written.
- **Manager:** loads the existing assets of a given `ScriptableObject` type in a table and creates or updates them from the sheet.
- **Typed parsing:** values are converted automatically to the field type (see [Supported types](#supported-types)).
- **List fields:** list values are stored in a single cell and split with a configurable separator character.
- **Persistent settings:** spreadsheet URL, sheet name, query range and credentials path are remembered between sessions and shared by all tabs.
- **Editor only:** nothing from the Google API is included in your builds.

## Requirements

- Unity 6000.3 or newer.
- A Google Cloud **service account** with the Google Sheets API enabled (see [Setup](#setup)).
- Unity packages `com.unity.nuget.newtonsoft-json` and `com.unity.visualscripting`. Unity installs both automatically when the package is added.

The Google API client libraries are bundled with the package. See [Third Party Notices](Third%20Party%20Notices.md).

## Installation

Open **Window > Package Manager**, click **+ > Add package from git URL...** and enter:

```
https://github.com/ElDiegu/scriptable-objects-from-sheets.git?path=/Packages/com.eldieguuu.scriptableobjectsfromsheets
```

Alternatively, clone the repository and use **Add package from disk...** on the package's `package.json`.

## Setup

1. In the [Google Cloud Console](https://console.cloud.google.com/), create a project and enable the **Google Sheets API**.
2. Create a **service account** and download its credentials as a JSON key file.
3. In your spreadsheet, click **Share** and give the service account's e-mail address access (Viewer is enough to import data).
4. In Unity, open **ScriptableObjects From Sheets > User Interface**.
5. In the **Authentication** tab, browse to the JSON key file, click **Test Credentials**, then **Save file path**.

> The key file is a secret. Keep it outside your project's version control. The package only stores its path, not its contents.

## Usage

### 1. Prepare the sheet

The first row of the selected range holds the headers. Each following row becomes one `ScriptableObject` asset.

| Name   | Health | Speed | Tint    | Drops              |
|--------|--------|-------|---------|--------------------|
| Goblin | 30     | 2.5   | #55AA33 | Coin@Dagger        |
| Orc    | 80     | 1.8   | #228B22 | Coin@Axe@Shield    |

### 2. Build the class (Builder tab)

1. Enter the **Spreadsheet URL**, **Sheet Name** and **Range**, then click **Process Sheet**.
2. Set the class name, namespace, output folder and asset menu path.
3. Review the inferred fields (use **Suggest** to re-run inference, **Add Field** to add one manually).
4. Generate the class. The result inherits from `ImportableSO<T>` and tags each field with `[SheetImported("Header")]`.

You can also write the class yourself:

```csharp
using ScriptableObjectsFromSheets.Core;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemies/Enemy", fileName = "Enemy")]
public class Enemy : ImportableSO<Enemy>
{
    [SheetImported("Name")]   public string enemyName;
    [SheetImported("Health")] public int health;
    [SheetImported("Speed")]  public float speed;
    [SheetImported("Tint")]   public Color tint;
    [SheetImported("Drops", '@')] public System.Collections.Generic.List<string> drops;
}
```

`HeaderName` must match the first-row cell of the column. The optional second parameter is the list separator (default `@`).

### 3. Generate the assets (Manager tab)

1. Choose the output folder and the target class, then click **Load**.
2. Enter the sheet details and click **Generate Scriptable Objects**.

## Supported types

| Type                                   | Sheet format                                   |
|----------------------------------------|------------------------------------------------|
| `int`, `long`, `float`, `double`       | Plain numbers                                  |
| `bool`                                 | `true` / `false`                               |
| `string`                               | Any text                                       |
| `Vector2`, `Vector3`                   | `(x;y)` or `(x;y;z)`, components split by `;`  |
| `Color`                                | HTML color, such as `#RRGGBB` or `red`         |
| `List<T>` of the types above           | Values split by the attribute's separator      |

## Troubleshooting

- **"Failed to validate credentials":** check that the JSON file is a service account key and that the Sheets API is enabled for its project.
- **Permission or 404 errors when processing a sheet:** share the spreadsheet with the service account's e-mail address.
- **A column is ignored:** the `[SheetImported]` header name must match the sheet header exactly.

## Support

Report issues at <https://github.com/ElDiegu/scriptable-objects-from-sheets/issues>.
