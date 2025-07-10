# Codi

Ein CLI-Tool zur Generierung von C#-Initialisierungscode aus JSON-Datenmodellen.

## Übersicht

Codi ist ein Kommandozeilen-Tool, das JSON-Dateien analysiert und daraus C#-Initialisierungscode generiert. Das Tool ist besonders nützlich, um komplexe Datenstrukturen in C# zu initialisieren und dabei eine saubere, lesbare Syntax zu verwenden.

## Features

- 🔄 **JSON zu C# Konvertierung**: Wandelt JSON-Strukturen in C#-Initialisierungscode um
- 📁 **Flexible Ein- und Ausgabe**: Unterstützt benutzerdefinierte Eingabe- und Ausgabepfade
- 🏗️ **Komplexe Datenstrukturen**: Behandelt verschachtelte Objekte, Arrays und primitive Datentypen
- 🎯 **Clean Code**: Generiert sauberen, lesbaren C#-Code mit korrekter Einrückung
- ⚡ **Performance**: Effiziente Verarbeitung auch größerer JSON-Dateien

## Installation

### Voraussetzungen

- .NET 10.0 oder höher
- Windows, macOS oder Linux

### Build aus Quellcode

```bash
git clone https://github.com/dein-username/codi.git
cd codi
dotnet build
dotnet publish -c Release -o ./publish
```

## Verwendung

### Grundlegende Syntax

```bash
codi --from <pfad-zur-json-datei> [--to <ausgabe-verzeichnis>]
```

### Parameter

- `--from`, `-f` (erforderlich): Pfad zur JSON-Eingabedatei
- `--to`, `-t` (optional): Ausgabeverzeichnis für den generierten Code. Wenn nicht angegeben, wird das Verzeichnis der Eingabedatei verwendet.

### Beispiele

#### Einfache Verwendung
```bash
codi --from data.json
```

#### Mit benutzerdefiniertem Ausgabeverzeichnis
```bash
codi --from ./models/user.json --to ./generated
```

#### Kurze Parameter-Namen
```bash
codi -f data.json -t ./output
```

## Beispiel

### Eingabe (data.json)
```json
{
  "name": "John Doe",
  "age": 30,
  "isActive": true,
  "skills": ["C#", "JavaScript", "Python"],
  "address": {
    "street": "123 Main St",
    "city": "Anytown",
    "zipCode": "12345"
  },
  "projects": [
    {
      "name": "Project A",
      "completed": true
    },
    {
      "name": "Project B", 
      "completed": false
    }
  ]
}
```

### Ausgabe (GeneratedInitialization.cs)
```csharp
MyObject instance = new()
{
    name = "John Doe",
    age = 30,
    isActive = true,
    skills = 
    [
        "C#",
        "JavaScript", 
        "Python",
    ],
    address = new()
    {
        street = "123 Main St",
        city = "Anytown",
        zipCode = "12345",
    },
    projects = 
    [
        new()
        {
            name = "Project A",
            completed = true,
        },
        new()
        {
            name = "Project B",
            completed = false,
        },
    ],
};
```

## Unterstützte Datentypen

- **Primitive Typen**: string, number, boolean, null
- **Arrays**: Unterstützung für Arrays aller unterstützten Typen
- **Objekte**: Verschachtelte Objekte mit beliebiger Tiefe
- **Leere Arrays**: Werden als `[]` initialisiert
- **Null-Werte**: Werden als `null` behandelt

## Technische Details

- **Framework**: .NET 10.0
- **Abhängigkeiten**: System.CommandLine.Hosting
- **Sprache**: C# mit aktivierten Nullable Reference Types
- **Architektur**: Modular aufgebaut mit separaten Komponenten für Parsing und Code-Generierung

## Entwicklung

### Projektstruktur

```
codi/
├── Codi.Cli/                 # CLI-Anwendung
│   ├── Program.cs            # Haupteinstiegspunkt und Code-Generierung
│   └── Codi.Cli.csproj      # Projektdatei
├── Codi.slnx                 # Solution-Datei
├── LICENSE                   # MIT-Lizenz
└── README.md                # Diese Datei
```

### Build-Befehle

```bash
# Projekt bauen
dotnet build

# Tests ausführen (falls vorhanden)
dotnet test

# Release-Build erstellen
dotnet publish -c Release
```

## Mitwirkung

Beiträge sind willkommen! Bitte:

1. Fork das Repository
2. Erstelle einen Feature-Branch (`git checkout -b feature/amazing-feature`)
3. Committe deine Änderungen (`git commit -m 'Add amazing feature'`)
4. Push zum Branch (`git push origin feature/amazing-feature`)
5. Öffne einen Pull Request

## Lizenz

Dieses Projekt steht unter der MIT-Lizenz. Siehe [LICENSE](LICENSE) für Details.

## Autor

Johannes-Max Sorge

---

**Hinweis**: Dieses Tool ist Teil des WebOfMe-Projekts und wurde entwickelt, um die Arbeit mit komplexen Datenmodellen in C# zu vereinfachen.