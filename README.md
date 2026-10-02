# Code Racer

Code Racer is een **top-down racegame** geschreven in **C#** met **WPF**. Deze README is bedoeld voor het interne ontwikkelteam: hoe we met Git werken en welke codeconventies we volgen.

---

## Inhoudsopgave

1. [Git-workflow](#git-workflow)
2. [Branchnamen](#branchnamen)
3. [Commitberichten](#commitberichten)
4. [Pull requests](#pull-requests)
5. [C#-codeconventies](#c-codeconventies)
6. [Testen](#testen)
7. [Problemen oplossen](#problemen-oplossen)

---

## Git-workflow

We werken met drie soorten branches:

```
feature/...  ──PR──▶  dev  ──PR──▶  main
```

| Branch                   | Doel                                                              |
| ------------------------ | ----------------------------------------------------------------- |
| `main`                   | Stabiele versie. Alleen wat volledig getest en goedgekeurd is.    |
| `dev`                    | Integratiebranch. Alle afgeronde features komen hier eerst samen. |
| `feature/...`, `fix/...` | Jouw eigen werk. Altijd afgetakt van `dev`.                       |

Niemand pusht rechtstreeks naar `dev` of `main` — alles gaat via een pull request.

### 1. Begin vanaf een actuele `dev`

```bash
git checkout dev
git pull origin dev
```

### 2. Maak een nieuwe branch

```bash
git checkout -b feature/drift-mechanic
```

`-b` maakt de branch aan en schakelt er in één stap naartoe.

### 3. Wijzigingen toevoegen en committen

```bash
git status                 # bekijk wat er gewijzigd is
git add <bestand>          # een specifiek bestand stagen
git add .                  # of alle wijzigingen stagen (controleer eerst git status!)
git commit -m "feat: add drift mechanic to car physics"
```

Commit kleine, logische stukken werk.

### 4. Pushen

De **eerste keer** dat je een nieuwe branch pusht, stel je de upstream in:

```bash
git push -u origin feature/drift-mechanic
```

`-u` koppelt je lokale branch aan de branch op de server. Daarna is gewoon dit genoeg:

```bash
git push
```

### 5. Pull request naar `dev`

Is je feature klaar? Open een PR van je branch naar **`dev`** (niet naar `main`). Zie [Pull requests](#pull-requests).

### 6. Van `dev` naar `main`

Als alles op `dev` getest is en goed werkt, wordt er een PR van `dev` naar `main` geopend. Dit gebeurt in overleg met het team, niet per losse feature.

### 7. Opruimen na de merge

```bash
git checkout dev
git pull origin dev

git branch -d feature/drift-mechanic
# Oude feature verwijderen
```

### Snelle referentie

| Taak                | Commando                                    |
| ------------------- | ------------------------------------------- |
| Actuele dev ophalen | `git checkout dev` en `git pull origin dev` |
| Nieuwe branch       | `git checkout -b <branch>`                  |
| Status bekijken     | `git status`                                |
| Bestanden stagen    | `git add <bestand>` of `git add .`          |
| Committen           | `git commit -m "<bericht>"`                 |
| Eerste push         | `git push -u origin <branch>`               |
| Latere pushes       | `git push`                                  |

---

## Branchnamen

Formaat: `<type>/<korte-omschrijving>` — kleine letters, woorden gescheiden door koppeltekens. Branchnamen schrijven we in het Engels.

| Type        | Gebruik voor                              | Voorbeeld                       |
| ----------- | ----------------------------------------- | ------------------------------- |
| `feature/`  | Nieuwe functionaliteit                    | `feature/nitro-boost`           |
| `fix/`      | Bugfixes                                  | `fix/lap-counter-off-by-one`    |
| `refactor/` | Codewijzigingen zonder gedragsverandering | `refactor/split-car-controller` |
| `chore/`    | Build, tooling, dependencies              | `chore/update-dotnet-sdk`       |
| `docs/`     | Alleen documentatie                       | `docs/track-format`             |

---

## Commitberichten

We volgen [Conventional Commits](https://www.conventionalcommits.org/). Commitberichten schrijven we in het Engels.

```
<type>: <korte samenvatting in de gebiedende wijs>
```

Voorbeelden:

```
feat: add nitro boost pickup
fix: prevent car from clipping through track walls at high speed
refactor: extract lap timing into LapTimer class
test: add tests for checkpoint ordering
docs: describe track file format
```

Regels:

- Samenvatting ≤ 72 tekens, in de gebiedende wijs ("add", niet "added" / "adds")
- Geen punt aan het eind
- Eén logische wijziging per commit
- Commit geen uitgecommentarieerde code of debuglogs

---

## Pull requests

**Voordat je een PR opent:**

- [ ] De build slaagt **zonder nieuwe warnings**
- [ ] De tests slagen
- [ ] Je hebt de game gespeeld en gecontroleerd dat je wijziging echt werkt

**De PR-beschrijving bevat:**

- Wat er veranderd is en waarom
- Hoe het te testen is (welk circuit / scenario / besturing)
- Screenshots of een korte GIF bij alles wat visueel is

**Reviewregels:**

- Feature-PR's gaan naar **`dev`**; alleen `dev` gaat naar `main`
- Minimaal **1 goedkeuring** nodig voor het mergen
- Houd PR's klein — streef naar minder dan ~400 gewijzigde regels
- Reviewers: wees concreet en vriendelijk. Begin niet-blokkerende opmerkingen met `nit:`

---

## C#-codeconventies

We volgen de standaard [.NET-codeconventies](https://learn.microsoft.com/dotnet/csharp/fundamentals/coding-style/coding-conventions). Code, identifiers en commentaar schrijven we in het Engels.

### Naamgeving

| Element                     | Stijl                 | Voorbeeld                               |
| --------------------------- | --------------------- | --------------------------------------- |
| Namespace                   | PascalCase            | `CodeRacer.Gameplay`                    |
| Class, struct, record, enum | PascalCase            | `CarController`, `TrackSegment`         |
| Interface                   | `I` + PascalCase      | `IDriveable`                            |
| Methode                     | PascalCase, werkwoord | `ApplyThrottle()`, `ResetLap()`         |
| Property                    | PascalCase            | `CurrentSpeed`                          |
| Publiek veld / constante    | PascalCase            | `MaxSpeed`                              |
| Privéveld                   | `_camelCase`          | `_currentLap`                           |
| Statisch privéveld          | `s_camelCase`         | `s_instanceCount`                       |
| Lokale variabele, parameter | camelCase             | `deltaTime`, `targetAngle`              |
| Enumwaarden                 | PascalCase            | `RaceState.Countdown`                   |
| Async-methode               | eindigt op `Async`    | `LoadTrackAsync()`                      |
| Boolean                     | leest als een vraag   | `IsGrounded`, `HasFinished`, `CanBoost` |

### Opmaak

- 4 spaties inspringing, geen tabs
- Allman-accolades (openingsaccolade op een eigen regel)
- Altijd accolades gebruiken, ook bij een `if` / `for` van één regel
- Eén class per bestand; bestandsnaam komt overeen met de classnaam
- Maximale regellengte ~120 tekens
- File-scoped namespaces: `namespace CodeRacer.Gameplay;`
- `using`-directives bovenaan, `System`-namespaces eerst

```csharp
namespace CodeRacer.Gameplay;

public sealed class LapTimer
{
    private readonly List<TimeSpan> _lapTimes = new();
    private TimeSpan _currentLapTime;

    public int CompletedLaps => _lapTimes.Count;

    public TimeSpan? BestLap => _lapTimes.Count > 0 ? _lapTimes.Min() : null;

    public void Update(TimeSpan deltaTime)
    {
        _currentLapTime += deltaTime;
    }

    public void CompleteLap()
    {
        _lapTimes.Add(_currentLapTime);
        _currentLapTime = TimeSpan.Zero;
    }
}
```

### Taalgebruik

- Gebruik `var` als het type duidelijk is uit de rechterkant; schrijf anders het type uit
- Geef de voorkeur aan `readonly`-velden en immutable types (`record`, `readonly struct`) waar mogelijk
- Markeer classes als `sealed`, tenzij ze bedoeld zijn voor overerving
- Zet nullable reference types aan (`<Nullable>enable</Nullable>`) en onderdruk warnings niet met `!` zonder commentaar dat uitlegt waarom
- Gebruik string-interpolatie: `$"Lap {lap}/{totalLaps}"`
- Gebruik `nameof(...)` in plaats van hardgecodeerde membernamen
- Gebruik `is null` / `is not null` in plaats van `== null`
- Schrijf access modifiers altijd expliciet (`private`, `public`, ...)
- Volgorde binnen een class: constanten → velden → constructors → properties → publieke methodes → privémethodes

### WPF & XAML

- **MVVM voor alle UI** (menu's, HUD, instellingen, uitslagen): views in XAML binden aan viewmodels. Houd code-behind (`.xaml.cs`) minimaal — alleen puur visuele zaken zoals focus of het starten van een animatie. Geen gamelogica in code-behind.
- Viewmodels implementeren `INotifyPropertyChanged`. Gebruik `ICommand` voor knoppen in plaats van `Click`-eventhandlers.
- **Naamgeving:** views eindigen op `View` of `Window` (`RaceView.xaml`, `MainWindow.xaml`); het bijbehorende viewmodel heet `RaceViewModel`. Geef elementen alleen een `x:Name` (PascalCase) als de code-behind ernaar moet verwijzen.
- Zet kleuren, brushes en styles in `ResourceDictionary`-bestanden — geen hardgecodeerde kleuren verspreid door de XAML. Gebruik `StaticResource`, tenzij de waarde tijdens runtime verandert (`DynamicResource`).
- **XAML-opmaak:** heeft een element meer dan ~3 attributen, zet dan één attribuut per regel, met `x:Name` als eerste.
- **Blokkeer nooit de UI-thread.** Laad circuits en assets met `async`/`await`. Raak UI-elementen alleen aan vanaf de UI-thread (gebruik `Dispatcher` als je vanaf een achtergrondthread komt).

### Game loop & rendering

- De game loop wordt aangedreven door **`CompositionTarget.Rendering`**, dat één keer per gerenderd frame afgaat. Gebruik **geen** `DispatcherTimer` voor de game loop — die is te onnauwkeurig.
- `Rendering` geeft geen delta time mee en kan meer dan één keer per frame afgaan. Meet de delta time met een `Stopwatch` en sla de tick over als `RenderingEventArgs.RenderingTime` niet veranderd is.
- **Framerate-onafhankelijkheid:** schaal beweging en timers altijd met delta time.
- **Maak of verwijder niet elk frame UI-elementen.** Maak de visuals van auto's en circuit één keer aan en verplaats ze daarna via hun `RenderTransform` (`TranslateTransform` / `RotateTransform`).
- **Data binding is voor menu's en HUD, niet voor beweging per frame.** De renderlaag zet posities direct.
- Roep `Freeze()` aan op brushes, geometries en bitmaps die nooit veranderen — bevroren objecten zijn veel goedkoper voor WPF om te renderen.
- Zijn er veel bewegende objecten (deeltjes, remsporen), teken ze dan met `DrawingVisual` / `OnRender` in plaats van duizenden `Shape`-elementen.
- **Alloceer nooit in de tick per frame** (geen `new`, LINQ, stringconcatenatie of closures). Hergebruik buffers en cache resultaten. GC-pauzes zie je terug als haperingen.
- **Geen magic numbers:** zet tuningwaarden (snelheid, grip, driftfactor) in benoemde constanten of config-bestanden, zodat ze makkelijk bij te stellen zijn.
- **Houd gamelogica los van WPF.** Spelregels (physics, ronden, AI) verwijzen niet naar WPF-types (`Point`, `Brush`, `Key`, ...), zodat ze testbaar zijn zonder venster.
- Geef bij auto's, pickups en circuitobjecten de voorkeur aan compositie boven diepe overerving.

### Commentaar & documentatie

- Code legt uit _wat_; commentaar legt uit _waarom_
- Gebruik `///` XML-doccommentaar bij publieke API's
- `// TODO:` moet een naam of issuenummer bevatten: `// TODO(#57): handle reverse driving`
- Verwijder dode code in plaats van die uit te commentariëren — Git onthoudt het toch

### Foutafhandeling

- Slik geen exceptions in met een lege `catch`
- Vang specifieke exceptions af, niet `Exception`, behalve op het hoogste niveau
- Valideer argumenten van publieke methodes (`ArgumentNullException.ThrowIfNull(car);`)
- Gebruik exceptions niet voor normale control flow (een auto die van de baan raakt is gamelogica, geen exception)

---

## Testen

- Geef tests de naam `MethodName_Scenario_ExpectedResult`:

```csharp
[Fact]
public void CompleteLap_WhenCalled_IncrementsCompletedLaps()
{
    var timer = new LapTimer();

    timer.CompleteLap();

    Assert.Equal(1, timer.CompletedLaps);
}
```

- Gebruik de structuur Arrange / Act / Assert
- Elke bugfix gaat vergezeld van een test die de bug reproduceert
- Richt tests op gameplaylogica (physics-berekeningen, ronde-/checkpointregels, scores, AI-beslissingen)

---

## Problemen oplossen

**"fatal: The current branch has no upstream branch"**
Je hebt deze branch nog niet eerder gepusht. Voer `git push -u origin <branchnaam>` uit.

**"Updates were rejected because the remote contains work that you do not have"**
Iemand anders heeft naar dezelfde branch gepusht. Haal eerst de wijzigingen op met `git pull` en push daarna opnieuw met `git push`.

**Ik heb per ongeluk op `dev` gecommit (nog niet gepusht)**

```bash
git branch feature/my-work     # bewaar je commits op een nieuwe branch
git reset --hard origin/dev    # zet lokale dev terug
git checkout feature/my-work   # ga verder op je eigen branch
```

**Build faalt na het pullen van `dev`**
Doe in Visual Studio _Build → Clean Solution_ en daarna _Rebuild Solution_. Faalt het nog steeds, vraag het dan in het teamkanaal — `dev` hoort altijd te builden.

**XAML-designer geeft fouten of laadt niet**
Build eerst de solution — de designer heeft gecompileerde code nodig. Werkt het nog steeds niet, sluit Visual Studio en verwijder de mappen `bin/` en `obj/`.

**De game hapert**
Meestal door allocaties of het aanmaken van UI-elementen in de tick per frame. Controleer dit met de profiler van Visual Studio (_Debug → Performance Profiler → .NET Object Allocation_).

---

Vragen? Stel ze in het teamkanaal.
