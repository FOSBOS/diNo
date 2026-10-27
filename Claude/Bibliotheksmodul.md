# Analyse: Architektur für neues Buchentleih-Modul in diNO

## Kontext

diNO soll um ein Modul zur Verwaltung von Buchentleihungen pro Schüler erweitert werden. Vorgabe des Nutzers: gleiche Datenbank wie diNO, Berechtigungen sollen von diNO übernommen werden (Klassenleiter dürfen nur ihre eigene Klasse verwalten, Sonderpersonen dürfen alles verwalten – analog zu bestehenden Admin-/Sekretariat-Rollen). Vor der Umsetzung soll geklärt werden, ob das Modul als **eigenes Unterprojekt** oder **direkt integriert** angelegt wird. Klärung mit dem Nutzer: das Modul soll dauerhaft Teil von diNO bleiben, keine eigenständige Nutzung an Schulen ohne diNO ist vorgesehen.

Dieses Dokument ist das Ergebnis der Analyse (keine Implementierung) und dient als Entscheidungsgrundlage.

## Befund zur bestehenden Architektur

- **Solution:** `F:\diNo\diNo.sln` enthält genau ein Projekt (`diNo.csproj`, .NET Framework 4.8, WPF/MVVM via CommunityToolkit.Mvvm). Kein Mehrprojekt-Aufbau, kein Plugin-/Modulsystem.
- **Bestehendes Modul-Muster:** Jedes Fachthema = Modellklasse + ViewModel + View im selben Projekt (Beispiel: `Vorkommnis.cs` + `VorkommnisseViewModel.cs` + `VorkommnisseView.xaml`).
- **Datenbankzugriff:** Kein EF/Dapper, klassisches ADO.NET mit Typed DataSet (`diNoDataSet.xsd`/.Designer.cs) + generierten TableAdaptern. Zentrale Singleton-Fassade `Zugriff.cs` (`Zugriff.Instance`) verwaltet Connection und ein generisches Repository-Pattern (`Repository<T>`).
- **Berechtigungen:** Enum `Rolle` in `Zugriff.cs` (Lehrer, Admin, Schulleitung, Sekretariat, Seminarfach, FpA...). Zusatzrollen in DB-Tabelle `LehrerRolle` (n:m). Zentrale Flags: `Zugriff.Instance.SiehtAlles` (Admin/Sekretariat/Schulleitung), `HatVerwaltungsrechte` (Admin/Sekretariat), `eigeneKlasse`/`Lehrer.KlassenleiterVon` (Klassenleiter-Bindung). **Es gibt keine zentrale `CanEdit()`-API** – Filterung erfolgt durch Auswahl unterschiedlicher TableAdapter-Methoden je Rolle, gesteuert in `Zugriff.LoadSchueler()`. ViewModels fragen die Flags direkt ab (z.B. `KlassenansichtViewModel.cs`, `SekretariatViewModel.cs`).
- **Deployment:** `diNo.csproj` enthält ClickOnce-artige Publish-Einstellungen (`PublishUrl`, `Install=true`, `InstallFrom=Disk`, Bootstrapper-Pakete). Schulen erhalten eine einzige `.exe`.
- **Datenmodell-Verknüpfung:** `Schueler.KlasseId → Klasse.Id`; `Klasse.KlassenleiterId → Lehrer.Id`; `LehrerRolle` n:m `Lehrer ↔ Rolle`.

## Option 1: Eigenes Unterprojekt

Realistische Varianten:
- **A – Zweites `.csproj` in derselben Solution** (Klassenbibliothek mit Models/ViewModels/Views als WPF-UserControls), referenziert von `diNo.csproj`. Ergebnis bleibt eine einzige `.exe`.
- **B – Komplett eigenständige Anwendung** (eigene Solution/`.exe`, eigenes Login), die nur die Datenbank teilt.
- **C – Geteilte Core-Bibliothek** (`diNo.Core.dll` mit Lehrer/Klasse/Schueler/`Zugriff`-Logik), referenziert von zwei Shell-Anwendungen.

**Vorteile:**
- Build-Isolation, klarere fachliche Trennung im Code.
- (Nur B/C) Spätere eigenständige Nutzung ohne diNO wäre möglich.
- (Nur A) Kein Mehraufwand bei Deployment, da weiterhin eine `.exe`.

**Nachteile:**
- **B:** Berechtigungslogik muss dupliziert oder unabhängig neu nachgebaut werden (eigener Login, eigene Rollen-/Klassenleiter-Abfrage) → hohes Drift-Risiko bei jeder künftigen Änderung an diNOs Rollenmodell. Zusätzlich eine zweite komplette Deployment-Pipeline (eigenes Publish-Profil, eigene IT-Freigabe an Schulen) nötig – erheblicher Mehraufwand für Schul-IT, die aktuell nur eine `.exe` kennt.
- **C:** Technisch sauberste Lösung, verlangt aber die Extraktion der Rollen-/Zugriffslogik aus der bestehenden `Zugriff.cs`-Singleton-Fassade (dort sind Connection-Handling, Repository-Pattern und Rollenlogik vermischt) in eine wiederverwendbare Bibliothek – das ist ein eigenständiges, nicht triviales Refactoring-Projekt, bevor überhaupt am Buchmodul gearbeitet werden kann.
- **A:** Kaum echter Vorteil gegenüber Integration, da ohnehin eine `.exe` entsteht und `Zugriff.Instance` weiterhin direkt genutzt wird – im Wesentlichen "Integration mit zusätzlicher Projektdatei".
- Initialer Mehraufwand: Projekt-Setup, NuGet-Referenzen duplizieren (CommunityToolkit.Mvvm etc.), Verweisführung zwischen Projekten pflegen.

## Option 2: Direkte Integration in `diNo.csproj`

**Vorteile:**
- Konsistent mit dem bestehenden, einzigen etablierten Muster im Projekt (Modell + ViewModel + View pro Thema im selben Projekt).
- Berechtigungsübernahme ohne jede Zusatzarbeit: Neues Modul ruft direkt `Zugriff.Instance.SiehtAlles`, `.HatVerwaltungsrechte`, `.eigeneKlasse`/`Lehrer.KlassenleiterVon` ab – exakt wie bestehende ViewModels. Kein Duplizieren, kein Drift-Risiko.
- Keine Deployment-Änderung: weiterhin eine `.exe`, ein Publish-Vorgang über das bestehende ClickOnce-Setup.
- Geringster Zeitaufwand bis zur ersten lauffähigen Version.
- Für eine neue Rolle "Bibliothek" (Sonderpersonen, die alle Bücher verwalten dürfen) reicht eine Erweiterung des bestehenden `Rolle`-Enums + `LehrerRolle`-Zuordnung – kein Architektureingriff.

**Nachteile:**
- Projekt wächst weiter (aktuell ~196 Dateien) – langfristig steigender Umfang des Monolithen.
- Keine Möglichkeit, das Modul später ohne größeren Schnitt eigenständig/an anderen Schulen ohne diNO einzusetzen (ist laut Nutzer-Entscheidung aber nicht gefordert).
- Kein zusätzlicher Build-Isolationsvorteil.

## Empfehlung

**Direkte Integration in `diNo.csproj` nach dem bestehenden Muster** (neue Modellklasse(n), z.B. `Buchentleihung.cs`/`Buch.cs`, ein `BuchverwaltungViewModel.cs`, eine `BuchverwaltungView.xaml`, DB-Zugriff über neue TableAdapter im bestehenden Typed DataSet).

Begründung:
1. Die Anforderung "Berechtigungen von diNO übernehmen" lässt sich nur verzögerungs- und risikofrei umsetzen, wenn das Modul im selben Prozess direkten Zugriff auf `Zugriff.Instance` hat. Jede Trennung (Varianten B/C) würde Berechtigungslogik duplizieren oder ein eigenes Vorab-Refactoring von `Zugriff.cs` erfordern.
2. Die Entscheidung des Nutzers, dass das Modul dauerhaft an diNO gebunden bleibt, entzieht dem Hauptargument für Trennung (Eigenständigkeit) die Grundlage.
3. Keine zusätzliche Deployment-Pipeline nötig – relevant, da Schulen bereits nur eine `.exe`-Verteilung gewohnt sind.
4. "Eigenes Unterprojekt in derselben Solution" (Variante A) bringt gegenüber direkter Integration keinen relevanten Zusatznutzen, da es ohnehin dieselbe `Zugriff`-Instanz und dieselbe `.exe` nutzt – der einzige Unterschied wäre eine zusätzliche Projektdatei ohne echten Mehrwert.

## Hinweis für eine spätere Umsetzung (nicht Teil dieser Analyse)

Falls später implementiert wird, sind diese Dateien als Orientierung relevant:
- `F:\diNo\Zugriff.cs` – Berechtigungs-Flags (`SiehtAlles`, `HatVerwaltungsrechte`, `eigeneKlasse`), Rollen-Enum, Repository-Registrierung.
- `F:\diNo\Repository.cs` – generisches Caching-Pattern für neue Entität (z.B. `BuchRep`).
- `F:\diNo\ViewModels\KlassenansichtViewModel.cs`, `F:\diNo\ViewModels\SekretariatViewModel.cs` – Referenzmuster, wie Rollen-Flags in ViewModels zur UI-Steuerung genutzt werden.
- `F:\diNo\diNo.csproj` / `diNoDataSet.xsd` – Einbindung neuer Tabellen/TableAdapter.
- Erweiterung des `Rolle`-Enums (z.B. neue Rolle `Bibliothek`) für Sonderpersonen, die alle Entleihungen verwalten dürfen.

## Verifikation

Diese Analyse erfordert keine Code-Änderung oder Testlauf – sie ist eine Entscheidungsgrundlage. Vor einer tatsächlichen Umsetzung sollte der Nutzer die Empfehlung (Integration) bestätigen; danach folgt ein separater Implementierungsplan (Datenmodell, DB-Schema für Bücher/Entleihungen, neue Rolle, View/ViewModel).
