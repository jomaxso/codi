# GitHub Actions Setup für Codi CLI

Diese GitHub Actions Pipelines automatisieren das Bauen, Testen und Veröffentlichen des Codi CLI Tools.

## Pipeline Übersicht

### 1. Continuous Integration (`ci.yml`)
**Ausgelöst bei:** Push auf `main`, `develop`, `feature/*` Branches und Pull Requests

**Features:**
- ✅ Baut das Projekt auf Linux, Windows und macOS
- ✅ Testet das CLI Tool mit verschiedenen JSON-Dateien
- ✅ Überprüft Code-Formatierung mit `dotnet format`
- ✅ Prüft auf Build-Warnungen
- ✅ Testet Error-Handling

### 2. Build and Release (`build-and-release.yml`)
**Ausgelöst bei:** Release-Erstellung

**Features:**
- 📦 Erstellt .NET Global Tool (NuGet Package)
- 🚀 Baut selbständige Executables für:
  - Windows (x64)
  - Linux (x64)
  - macOS (Intel x64)
  - macOS (Apple Silicon arm64)
- 📋 Lädt automatisch Release Assets hoch
- 📦 Veröffentlicht auf NuGet.org (optional)

## Setup Anweisungen

### 1. Repository Secrets konfigurieren

Für die automatische NuGet-Veröffentlichung:

1. Gehen Sie zu GitHub Repository → Settings → Secrets and variables → Actions
2. Fügen Sie folgendes Secret hinzu:
   - `NUGET_API_KEY`: Ihr NuGet.org API Key

### 2. NuGet API Key erstellen

1. Gehen Sie zu [nuget.org](https://www.nuget.org)
2. Melden Sie sich an und gehen zu Account Settings → API Keys
3. Erstellen Sie einen neuen API Key mit "Push" Berechtigungen
4. Kopieren Sie den Key in das GitHub Secret

### 3. Release erstellen

1. Gehen Sie zu GitHub Repository → Releases
2. Klicken Sie "Create a new release"
3. Erstellen Sie einen neuen Tag (z.B. `v1.0.0`)
4. Füllen Sie Titel und Beschreibung aus
5. Klicken Sie "Publish release"

Die Pipeline wird automatisch gestartet und erstellt:
- NuGet Package
- Executables für alle Plattformen
- Release Assets

## Verwendung des veröffentlichten Tools

### Als .NET Global Tool installieren:
```bash
dotnet tool install -g Codi.CLI
codi --from input.json --to output
```

### Als Standalone Executable:
1. Laden Sie die entsprechende Datei vom Release herunter
2. Führen Sie sie direkt aus:
```bash
# Windows
codi-win-x64.exe --from input.json --to output

# Linux/macOS
chmod +x codi-linux-x64
./codi-linux-x64 --from input.json --to output
```

## Pipeline Konfiguration

### .NET Version
- **Target:** .NET 10.0 (Preview)
- **Quality:** Preview (für bleeding-edge Features)

### Build Konfiguration
- **Debug:** Für CI/Tests
- **Release:** Für Veröffentlichung
- **Self-contained:** Ja (keine .NET Runtime erforderlich)
- **Single File:** Ja (alles in einer Datei)
- **Trimmed:** Ja (reduzierte Dateigröße)

### Plattform-Support
- ✅ Windows x64
- ✅ Linux x64
- ✅ macOS x64 (Intel)
- ✅ macOS arm64 (Apple Silicon)

## Troubleshooting

### Pipeline Fehler beheben

1. **Build Fehler:** Überprüfen Sie .NET 10.0 Kompatibilität
2. **Test Fehler:** Stellen Sie sicher, dass alle Test-JSON Dateien gültig sind
3. **NuGet Fehler:** Überprüfen Sie API Key und Package-Metadaten
4. **Release Fehler:** Stellen Sie sicher, dass der Tag ein korrektes SemVer Format hat

### Lokales Testen

```bash
# Build testen
dotnet build Codi.Cli/Codi.Cli.csproj

# CLI Tool testen
echo '{"test": "data"}' > test.json
dotnet run --project Codi.Cli/Codi.Cli.csproj -- --from test.json --to output

# Executable erstellen
dotnet publish Codi.Cli/Codi.Cli.csproj -c Release -r win-x64 --self-contained
```
