## TD Core Systems Wiring — Implementation Checklist

### Scope & Constraints
- [ ] Use Unity Input System (new) for placement input.
- [ ] Use grid-snapped placement with blocked-cell validation.
- [ ] Use manual composition root for dependency injection.
- [ ] Defer economy spending/reward logic; keep extension seam only.

### Dependency Injection Map (What gets injected where)
- [ ] `IGameStateService` injected into:
  - [ ] Enemy event bridge / spawn adapter (handles enemy died/reached-goal)
  - [ ] `GameHudPresenter` (read-only UI updates)
  - [ ] `TowerPlacementController` (disable or gate on game-over)
- [ ] `IScoreService` and `IBaseHealthService` should generally be surfaced through `IGameStateService` to reduce constructor/method parameters.
- [ ] `ICurrencyService` (future) injected into:
  - [ ] `ITowerPlacementService` (spend checks)
  - [ ] `GameHudPresenter` (display)

### Minimal Contracts (Target shape)
- [ ] `IGameStateService`
  - [ ] `int BaseHealth { get; }`
  - [ ] `int Score { get; }`
  - [ ] `bool IsGameOver { get; }`
  - [ ] `event Action<int> OnBaseHealthChanged`
  - [ ] `event Action<int> OnScoreChanged`
  - [ ] `event Action OnGameOver`
  - [ ] `void AddScore(int amount)`
  - [ ] `void DamageBase(int amount)`
- [ ] `ICurrencyService` (placeholder for later)
  - [ ] `int Current { get; }`
  - [ ] `bool TrySpend(int amount)`
  - [ ] `void Add(int amount)`
  - [ ] `event Action<int> OnCurrencyChanged`

### 1) Composition Root
- [ ] Create `GameCompositionRoot` MonoBehaviour under `Assets/Scripts`.
- [ ] Centralize startup wiring of runtime services and adapters.
- [ ] Inject abstractions into systems that need cross-feature communication.
- [ ] Remove any new cross-system `Find`/direct lookups introduced during this feature.

### Composition Root Wiring Order (Awake/Start)
- [ ] In `Awake`, instantiate concrete services (`GameStateService`, optional `NullCurrencyService`).
- [ ] Resolve scene references via serialized fields (do not service-locate globally).
- [ ] Call explicit method injection on MonoBehaviours:
  - [ ] `enemyEventBridge.Initialize(gameStateService)`
  - [ ] `hudPresenter.Initialize(gameStateService /*, currencyService later */)`
  - [ ] `towerPlacementController.Initialize(placementService, inputReader, gameStateService)`
  - [ ] `placementService.Initialize(towerFactory, gridRules /*, currencyService later */)`
- [ ] In `OnDestroy`, call `Dispose`/`Shutdown` or explicit unbind methods for subscribers if needed.

### 2) Domain State Services
- [ ] Add interfaces: `IGameStateService`, `IBaseHealthService`, `IScoreService`.
- [ ] Implement a `GameStateService` with events:
  - [ ] `OnBaseHealthChanged`
  - [ ] `OnScoreChanged`
  - [ ] `OnGameOver`
- [ ] Keep domain service independent of scene object references.

### Concrete Injection Targets and Responsibilities
- [ ] Enemy outcome adapter/bridge:
  - [ ] Subscribes to spawned `Enemy.OnDied` and `Enemy.OnCompletedRoute`
  - [ ] Calls `gameState.AddScore(...)` and `gameState.DamageBase(...)`
  - [ ] Unsubscribes on enemy return/death to avoid pooled duplicate handlers
- [ ] `GameHudPresenter`:
  - [ ] Subscribes to `OnScoreChanged`, `OnBaseHealthChanged`, `OnGameOver`
  - [ ] Updates labels only (no game rules)
- [ ] `TowerPlacementController`:
  - [ ] Reads click/pointer input
  - [ ] Calls placement service
  - [ ] Stops processing placement when `gameState.IsGameOver` is true or `OnGameOver` fires

### 3) Enemy Outcome Integration
- [ ] In enemy spawn flow, subscribe to enemy lifecycle events on spawn.
- [ ] On enemy death, increment score through service.
- [ ] On enemy route completion, decrement base health through service.
- [ ] Ensure unsubscription/cleanup to avoid duplicate handlers on pooled reuse.

### 4) Placement Input Controller
- [ ] Create `TowerPlacementController` that reads click/point from Input System actions.
- [ ] Convert pointer world hit to grid cell coordinate.
- [ ] Call placement service only (no prefab logic in input controller).
- [ ] Disable placement input when game-over event is raised.

### Suggested Initialize(...) Signatures
- [ ] `EnemyEventBridge.Initialize(IGameStateService gameState)`
- [ ] `GameHudPresenter.Initialize(IGameStateService gameState)`
- [ ] `TowerPlacementController.Initialize(ITowerPlacementService placementService, IPlacementInputReader inputReader, IGameStateService gameState)`
- [ ] `GridPlacementService.Initialize(ITowerFactory towerFactory, IGridBuildRules gridRules)`
- [ ] Future-ready overload: `GridPlacementService.Initialize(ITowerFactory towerFactory, IGridBuildRules gridRules, ICurrencyService currencyService)`

### Dependency Rules (Keep coupling low)
- [ ] MonoBehaviours depend on interfaces, not concrete services.
- [ ] UI never mutates gameplay state directly.
- [ ] Input controller never instantiates towers directly.
- [ ] Tower factory knows prefab/config only; no input/UI/state knowledge.
- [ ] Composition root is the only place aware of concrete class graph.

### 5) Grid Placement Rules
- [ ] Add `ITowerPlacementService` and concrete `GridPlacementService`.
- [ ] Validate:
  - [ ] Cell is on allowed build layer/surface.
  - [ ] Cell is not already occupied.
  - [ ] Cell is within map/build bounds (if applicable).
- [ ] Track occupied cells and expose clear placement result codes.

### 6) Tower Factory
- [ ] Add `ITowerFactory` + `TowerFactory` as the only tower-instantiation point.
- [ ] Instantiate configured tower prefab at approved cell transform.
- [ ] Register occupancy after successful placement.
- [ ] Keep factory free from input/UI dependencies.

### 7) UI Presentation Layer
- [ ] Add Canvas + EventSystem to scene if missing.
- [ ] Create `GameHudPresenter` script(s) bound to HUD labels.
- [ ] Subscribe to state service events and display:
  - [ ] Base health
  - [ ] Score
  - [ ] Game over state/message
- [ ] Keep UI strictly read-only relative to game state.

### 8) Scene Wiring
- [ ] Add composition root object to scene and assign serialized dependencies.
- [ ] Configure build layer mask, grid settings, and tower prefab reference.
- [ ] Connect InputActionAsset references used by placement controller.
- [ ] Remove temporary direct references now replaced by injected interfaces.

### 9) Economy Seam (Deferred)
- [ ] Add `ICurrencyService` interface now with default/no-op implementation.
- [ ] Register it in composition root.
- [ ] Keep placement service API ready for future spend checks.

### Common Pitfalls to Avoid
- [ ] Do not subscribe to pooled enemy events without guaranteed unsubscribe path.
- [ ] Do not inject both `IGameStateService` and its child services everywhere unless required.
- [ ] Do not let HUD hold direct references to gameplay actors (`Enemy`, `Tower`, etc.).
- [ ] Do not put rule logic in presenters/controllers that belongs in services.
- [ ] Do not use static singletons for services if using composition root DI.

### Verification Checklist
- [ ] Existing enemy spawn, movement, and shooting still function.
- [ ] Killing an enemy increases score in HUD.
- [ ] Enemy reaching goal decreases base health in HUD.
- [ ] Base health reaching zero raises game-over and blocks placement input.
- [ ] Clicking valid grid cell places exactly one tower.
- [ ] Clicking invalid/occupied cells does not place towers.
- [ ] No legacy input calls used for new placement flow.
- [ ] No duplicate event handling across pooled enemy reuse.

### Done Criteria
- [ ] All checklist items above complete.
- [ ] Play mode sanity pass completed without regressions in current combat loop.
- [ ] New features communicate through interfaces with minimal direct dependencies.

---

## File-by-File Implementation Checklist (Exact Create/Edit Order)

### Phase 1 — Domain Services and DI Root

#### Create: `Assets/Scripts/Services/IGameStateService.cs`
- [ ] Define read-only state: `BaseHealth`, `Score`, `IsGameOver`.
- [ ] Define events: `OnBaseHealthChanged`, `OnScoreChanged`, `OnGameOver`.
- [ ] Define mutations: `AddScore(int amount)`, `DamageBase(int amount)`.

#### Create: `Assets/Scripts/Services/ICurrencyService.cs`
- [ ] Define placeholder contract for future economy: `Current`, `TrySpend`, `Add`, `OnCurrencyChanged`.

#### Create: `Assets/Scripts/Services/GameStateService.cs`
- [ ] Implement `IGameStateService` with serialized/default constructor values for start health.
- [ ] Guard game-over transition so `OnGameOver` fires once.
- [ ] Raise change events only when values actually change.

#### Create: `Assets/Scripts/Services/NullCurrencyService.cs`
- [ ] Implement `ICurrencyService` as temporary no-op/default.
- [ ] `TrySpend` behavior should match deferred-economy decision (usually always true for now).

#### Create: `Assets/Scripts/Composition/GameCompositionRoot.cs`
- [ ] Add serialized references for scene-side consumers:
  - [ ] `EnemySpawner` (or event bridge)
  - [ ] `GameHudPresenter`
  - [ ] `TowerPlacementController`
  - [ ] `GridPlacementService` / factory dependencies
- [ ] Instantiate concrete services in `Awake`.
- [ ] Inject dependencies via explicit `Initialize(...)` calls.
- [ ] Add cleanup (`OnDestroy`) to unbind long-lived subscriptions where needed.

### Phase 2 — Enemy Outcome to Domain State

#### Create: `Assets/Scripts/Gameplay/EnemyEventBridge.cs`
- [ ] Expose `Initialize(IGameStateService gameState)`.
- [ ] Expose `Bind(Enemy enemy)` method called on each spawn.
- [ ] On enemy death: `gameState.AddScore(killScore)`.
- [ ] On enemy route completion: `gameState.DamageBase(baseDamage)`.
- [ ] Ensure unbind logic for pooled reuse safety.

#### Edit: `Assets/Scripts/EnemySpawner.cs`
- [ ] Add injected dependency reference to `EnemyEventBridge` (through `Initialize` or serialized then injected).
- [ ] In `SpawnEnemy(...)`, after pool `Get()`, call bridge `Bind(enemy)`.
- [ ] Keep spawner focused on spawn mechanics only.

#### Edit: `Assets/Scripts/Enemy.cs`
- [ ] Verify pooled lifecycle does not wipe subscriptions unexpectedly in a way that conflicts with bridge unbinding strategy.
- [ ] Keep enemy responsible for movement/combat lifecycle only.

### Phase 3 — Placement Input and Build Rules

#### Create: `Assets/Scripts/Input/IPlacementInputReader.cs`
- [ ] Contract for pointer + click intent (Input System-backed).
- [ ] Expose enable/disable methods for game-over gating.

#### Create: `Assets/Scripts/Input/PlacementInputReader.cs`
- [ ] Implement Input System action reads from `InputSystem_Actions`.
- [ ] Normalize click events into one callback/event for controller.

#### Create: `Assets/Scripts/Placement/IGridBuildRules.cs`
- [ ] Contract: `TryGetCell(Vector3 world, out Vector3Int cell)`, `CanBuild(cell)`, `MarkOccupied(cell)`.

#### Create: `Assets/Scripts/Placement/GridBuildRules.cs`
- [ ] Implement grid snap + layer mask validation + occupied-cell tracking.

#### Create: `Assets/Scripts/Placement/ITowerFactory.cs`
- [ ] Contract for creating towers at resolved cell world position.

#### Create: `Assets/Scripts/Placement/TowerFactory.cs`
- [ ] Own concrete tower prefab instantiation.
- [ ] No input/UI/game-state logic inside.

#### Create: `Assets/Scripts/Placement/ITowerPlacementService.cs`
- [ ] Contract for placement request API + placement result enum.

#### Create: `Assets/Scripts/Placement/GridPlacementService.cs`
- [ ] `Initialize(ITowerFactory, IGridBuildRules)` now.
- [ ] Add future-ready overload/field for `ICurrencyService` without activating spend logic yet.
- [ ] Return explicit failure reason codes (invalid surface, occupied, game over, etc.).

#### Create: `Assets/Scripts/Placement/TowerPlacementController.cs`
- [ ] `Initialize(ITowerPlacementService, IPlacementInputReader, IGameStateService)`.
- [ ] On click: raycast to world, request placement, handle result.
- [ ] Disable input or ignore clicks when game-over is active.

### Phase 4 — UI Presentation

#### Create: `Assets/Scripts/UI/GameHudPresenter.cs`
- [ ] `Initialize(IGameStateService gameState)` now; future optional `ICurrencyService`.
- [ ] Bind text fields for score, base health, game-over label.
- [ ] Subscribe/unsubscribe in lifecycle-safe way.

#### Edit: `Assets/Scenes/SampleScene.unity`
- [ ] Add Canvas + EventSystem if missing.
- [ ] Add HUD text elements and assign to `GameHudPresenter`.
- [ ] Add `GameCompositionRoot` object and assign references.
- [ ] Add placement controller and required build surface/layer settings.

### Phase 5 — Existing Script Touchpoints (Quick Audit)

#### Edit: `Assets/Scripts/Tower.cs` (only if needed)
- [ ] Ensure no assumptions conflict with newly placed tower instances.

#### Edit: `Assets/Scripts/Shooter.cs` (only if needed)
- [ ] Verify spawned towers initialize shooter dependencies the same way as scene-placed towers.

#### Edit: `Assets/Scripts/Projectile.cs` (only if needed)
- [ ] No direct changes expected unless score hooks are moved here (not recommended).

### Wiring Validation (By Feature Slice)
- [ ] Slice A: Start game -> enemy reaches goal -> base health UI decrements.
- [ ] Slice B: Enemy dies -> score UI increments.
- [ ] Slice C: Click valid cell -> tower appears and cell blocks repeat placement.
- [ ] Slice D: Base health reaches zero -> game-over UI appears and placement stops.

### Optional Next Slice (After MVP)
- [ ] Activate currency spend in `GridPlacementService` via `ICurrencyService.TrySpend`.
- [ ] Add tower cost data object/config per prefab.
- [ ] Add reward-on-kill currency through `EnemyEventBridge`.
