# PRD — Videojuego 3D en Unity 6 (URP) construido con IA vía MCP

> **Documento de Requisitos de Producto (PRD) + Plan de Ejecución por Fases**
> Diseñado para ejecutarse con un agente de IA (**Antigravity 2.0**) conectado en tiempo real a Unity mediante el MCP **[IvanMurzak/Unity-MCP — "AI Game Developer: Unity SKILLS, MCP"](https://github.com/IvanMurzak/Unity-MCP)**.
>
> **Regla de oro:** el agente NO avanza a la siguiente fase hasta que la **Puerta de Validación (✅ Gate)** de la fase actual pasa al 100%. Cada Gate se verifica con herramientas MCP reales (screenshots, logs, play-mode, tests).

---

## 0. Contexto del proyecto (verificado)

| Dato | Valor |
|---|---|
| **Motor** | Unity `6000.4.9f1` (Unity 6) |
| **Render Pipeline** | Universal Render Pipeline (URP `17.4.0`) — perfiles PC y Móvil ya configurados |
| **Input** | New Input System `1.19.0` (`InputSystem_Actions` ya define Player: Move, Look, Attack, Interact, Crouch, Jump, Sprint + mapa UI) |
| **Producto** | `Paredes Gutierrez Meayck Rudloff` (v0.1.0) |
| **Estado actual** | Plantilla *URP Empty*: 1 escena vacía (`SampleScene` → Main Camera, Directional Light, Global Volume). Sin gameplay propio. |
| **Assets del juego** | **Ya importados por el usuario** (personaje animado, enemigos, zombies, rampas, hangar, props, sonidos). El PRD asume su existencia y se centra en **integración + lógica + escenas**. |
| **Orquestador** | Agente IA Antigravity 2.0 |
| **Puente en tiempo real** | Unity-MCP (OpenUPM `com.ivanmurzak.unity.mcp`), puerto `8080` |

### ⚠️ Bloqueante crítico detectado (resolver en Fase 0)
El MCP de Unity **NO funciona si la ruta del proyecto contiene espacios**. El proyecto se llama `Paredes Gutierrez Meayck Rudloff` (con espacios) → **debe copiarse/moverse a una ruta sin espacios** (ej. `ParedesGutierrezMeayckRudloff/`) antes de conectar el MCP. Esto es un requisito duro del bridge stdio.

---

## 1. Objetivo del producto

Construir un videojuego 3D en tercera/primera persona con **4 escenas encadenadas**, personaje jugable animado, enemigos con IA básica, HUD de estado, sonido, y flujo completo hasta créditos — cumpliendo **al 100% la rúbrica de evaluación** (ver §12). Todo el desarrollo lo ejecuta el agente IA operando Unity en vivo por MCP.

### Criterios de éxito (Definition of Done global)
- [ ] Las **4 escenas** existen, están en *Build Settings* en orden y se transicionan sin errores.
- [ ] El **personaje principal** tiene animaciones diferenciadas: **Idle (espera), Run (correr), Attack (ataque), Jump (salto)**.
- [ ] Los **enemigos** tienen animaciones: **Idle, Run, Attack**.
- [ ] **HUD** muestra: vida, puntos, ítems recogidos, enemigos destruidos.
- [ ] **Música de fondo + efectos de sonido** integrados y audibles.
- [ ] Nivel 1 → Nivel 2 → Final → **Créditos** funciona de extremo a extremo.
- [ ] Créditos muestran: **Apellido y Nombre – NRC** por cada integrante.
- [ ] `console-get-logs` sin errores rojos tras un playthrough completo.

---

## 2. Arquitectura de escenas (obligatoria)

| # | Escena | Archivo sugerido | Propósito | Requisitos específicos de la consigna |
|---|--------|------------------|-----------|----------------------------------------|
| 0 | **Splash / Inicio** | `Assets/Scenes/00_Splash.unity` | Pantalla de inicio tipo splash | Duración **≤ 20 s**, luego auto-transición al Nivel 1 |
| 1 | **Nivel 1 — Rampas** | `Assets/Scenes/01_Rampas.unity` | Plataformas/rampas | **Agregar más rampas** al set original + **mecánica de rampas que suben y bajan** (móviles) |
| 2 | **Nivel 2 — Hangar** | `Assets/Scenes/02_Hangar.unity` | Sigilo/combate | **≥ 10 enemigos** en distintas ubicaciones · recoger **2 ítems** (mostrados en Canvas) → **abre la puerta final** → **zona de intercambio con efectos de partículas** |
| 3 | **Final — Horda + Créditos** | `Assets/Scenes/03_Final.unity` | Horda | Enemigos que **atacan en horda** · destruir **≥ 5 zombies** → **Canvas de créditos** con `Apellido Nombre – NRC` |

> **Build order** (`EditorBuildSettings` vía `scene-open`/`scene-save` + configuración): `00_Splash` → `01_Rampas` → `02_Hangar` → `03_Final`.

---

## 3. Sistemas transversales (se construyen una vez, se reutilizan)

Todos como scripts en `Assets/Scripts/` creados con `script-update-or-create` y compilados con `assets-refresh`.

| Sistema | Script | Responsabilidad |
|---|---|---|
| **GameManager** (singleton, `DontDestroyOnLoad`) | `GameManager.cs` | Estado global: puntos, vida, ítems, enemigos destruidos; carga de escenas por nombre; eventos. |
| **PlayerController** | `PlayerController.cs` | Movimiento (New Input System), salto, ataque; dispara triggers del Animator. |
| **PlayerHealth** | `PlayerHealth.cs` | Vida, daño, muerte; notifica al HUD. |
| **EnemyAI** | `EnemyAI.cs` | Estados Idle/Run/Attack (NavMesh o patrulla simple); daño al jugador; muerte → suma puntos/kills. |
| **ZombieHorde** | `ZombieHorde.cs` | Spawner de horda para escena Final; cuenta kills ≥ 5. |
| **Collectible** | `Collectible.cs` | Ítem recogible; incrementa contador; abre puerta al llegar a 2. |
| **MovingRamp** | `MovingRamp.cs` | Rampa que sube/baja (interpolación entre 2 puntos). |
| **HUDController** | `HUDController.cs` | Actualiza textos del Canvas: vida / puntos / ítems / kills. |
| **AudioManager** | `AudioManager.cs` | Música de fondo por escena + SFX (salto, ataque, recoger, muerte). |
| **SceneFlow** | `SceneFlow.cs` | Splash timer, triggers de fin de nivel, transiciones. |

**Animator Controllers** (uno por tipo de agente):
- `Player.controller`: estados `Idle ⇄ Run`, `Idle → Jump`, `Idle/Run → Attack`; parámetros `Speed (float)`, `Jump (trigger)`, `Attack (trigger)`, `Grounded (bool)`.
- `Enemy.controller`: `Idle ⇄ Run → Attack`; parámetros `Speed (float)`, `Attack (trigger)`.
- `Zombie.controller`: reutiliza `Enemy.controller` o variante.

---

## 4. Plan de ejecución por FASES (con Puertas de Validación)

> Convención por fase: **Objetivo → Tareas (herramientas MCP) → ✅ Gate de validación**. El agente debe reportar la evidencia del Gate (screenshot/logs) antes de continuar.

### 🔧 FASE 0 — Preparación del entorno y conexión MCP
**Objetivo:** Unity conectado al agente por MCP, ruta sin espacios, dependencias listas.

**Tareas**
1. **Copiar/renombrar** el proyecto a una ruta **sin espacios** (ej. `.../ParedesGutierrezMeayckRudloff/`). *(requisito duro del MCP)*.
2. Instalar el plugin MCP en Unity: OpenUPM `openupm add com.ivanmurzak.unity.mcp` **o** el `.unitypackage` installer.
3. En Unity: `Window ▸ AI Game Developer ▸ Auto-generate skills` (o *Configure MCP*). Puerto **8080**.
4. Configurar Antigravity 2.0 como cliente MCP apuntando a `ai-game-developer` (stdio o `http://localhost:8080`).
5. Verificar assets importados con `assets-find` (personaje, enemigos, zombies, rampas, hangar, audio).

**✅ Gate 0** — *no continuar hasta que TODO pase:*
- [ ] `editor-application-get-state` responde (Unity accesible por MCP).
- [ ] Ruta del proyecto **sin espacios** confirmada.
- [ ] `assets-find` lista los assets clave (modelos animados de personaje + enemigos + zombies + audio + rampas/hangar).
- [ ] `console-get-logs` sin errores de compilación.

---

### 🎬 FASE 1 — Splash / Escena de Inicio
**Objetivo:** Escena 0 tipo splash con auto-transición ≤ 20 s.

**Tareas**
- `scene-create` → `00_Splash.unity`.
- `gameobject-create` Canvas + logo/título; `gameobject-component-add` Image/Text.
- `script-update-or-create` `SceneFlow.cs` (temporizador; `Invoke("LoadNextScene", ≤20)` o barra de progreso).
- `AudioManager`: música/jingle de splash.
- Registrar escena en Build Settings (índice 0).

**✅ Gate 1**
- [ ] `editor-application-set-state` = Play → splash visible (`screenshot-game-view`).
- [ ] Transición automática al Nivel 1 en **≤ 20 s** (evidencia: log de carga).
- [ ] Sin errores en `console-get-logs`.

---

### 🛹 FASE 2 — Personaje jugable + Animaciones (núcleo reutilizable)
**Objetivo:** Personaje con Idle/Run/Attack/Jump controlado por New Input System. *(Se hace en `01_Rampas` y se convierte en Prefab reutilizable.)*

**Tareas**
- `scene-create` → `01_Rampas.unity`; añadir suelo base.
- Instanciar el modelo del personaje (`gameobject-create` / `assets-prefab-instantiate`).
- `gameobject-component-add`: `CharacterController` (o Rigidbody+Capsule), `PlayerInput` (usa `InputSystem_Actions`), `Animator` con `Player.controller`.
- Crear `Player.controller` con estados y transiciones (parámetros `Speed`, `Jump`, `Attack`, `Grounded`). *(Opcional: paquete AI Animation del MCP para acelerar.)*
- `script-update-or-create` `PlayerController.cs` + `PlayerHealth.cs`; enlazar acciones Move/Jump/Attack a parámetros del Animator.
- Convertir a **Prefab**: `assets-prefab-create` → `Assets/Prefabs/Player.prefab`.

**✅ Gate 2** *(rúbrica: animaciones del personaje)*
- [ ] En Play: quieto → **Idle**; moverse → **Run**; botón salto → **Jump**; botón ataque → **Attack** (evidencia: 4 `screenshot-game-view` o clip).
- [ ] Transiciones limpias, sin *T-pose* ni deslizamiento evidente.
- [ ] Sin errores en `console-get-logs`.

---

### 🧗 FASE 3 — Nivel 1: Rampas + rampas móviles
**Objetivo:** Completar `01_Rampas` con más rampas y mecánica sube/baja.

**Tareas**
- Duplicar/añadir rampas (`gameobject-duplicate`) → **más que el set original**.
- `script-update-or-create` `MovingRamp.cs` (Lerp entre `pointA`/`pointB`, velocidad configurable, ping-pong).
- Añadir `MovingRamp` a ≥ 2 rampas; colocar `Collider`s.
- Punta de meta / trigger de fin de nivel (`SceneFlow`) → carga `02_Hangar`.
- `AudioManager`: música de nivel + SFX de salto.

**✅ Gate 3**
- [ ] Personaje sube por las rampas y salta entre plataformas (screenshots).
- [ ] Rampas móviles **suben y bajan** de forma visible y continua.
- [ ] Al llegar a la meta → transición a Nivel 2 sin errores.

---

### 🛩️ FASE 4 — Nivel 2: Hangar (enemigos + ítems + puerta + partículas)
**Objetivo:** `02_Hangar` con ≥10 enemigos, 2 ítems, puerta que se abre, zona de intercambio con partículas.

**Tareas**
- `scene-create`/armar `02_Hangar` con el escenario hangar importado.
- Instanciar **≥ 10 enemigos** en ubicaciones distintas; Prefab `Enemy.prefab` con `Animator (Enemy.controller)` + `EnemyAI.cs` (Idle/Run/Attack, daño al jugador).
- `Collectible.cs` en **2 ítems**; HUD muestra progreso (0/2 → 2/2).
- **Puerta final**: cerrada por defecto; `GameManager` la abre al recoger 2 ítems (`gameobject-component-modify` / animación).
- **Zona de intercambio**: `gameobject-component-add` `ParticleSystem` (efectos visibles). *(Opcional: paquete AI ParticleSystem del MCP.)*
- Trigger tras la puerta → carga `03_Final`.

**✅ Gate 4** *(rúbrica: panel de puntos, ítems, adversarios)*
- [ ] `gameobject-find` confirma **≥ 10** enemigos en la escena.
- [ ] Enemigos animan Idle/Run/Attack y persiguen/atacan (screenshots).
- [ ] Recoger 2 ítems → HUD 2/2 → **puerta se abre** (evidencia visual).
- [ ] Zona de intercambio muestra **partículas** activas.
- [ ] Transición a Final sin errores.

---

### 🧟 FASE 5 — Escena Final: Horda + Créditos
**Objetivo:** `03_Final` con horda de zombies (destruir ≥ 5) → Canvas de créditos.

**Tareas**
- Armar `03_Final`; `ZombieHorde.cs` spawnea zombies que **atacan en horda** (`Zombie.controller`).
- Combate: ataque del jugador destruye zombies; `GameManager` cuenta kills; al llegar a **≥ 5** → dispara créditos.
- **Canvas de créditos**: por integrante `Apellido Nombre – NRC` (completar datos reales del equipo).
- `AudioManager`: música final / créditos.

**✅ Gate 5** *(rúbrica: niveles → créditos finales)*
- [ ] Zombies aparecen en horda y atacan (screenshots).
- [ ] Al destruir **≥ 5** → aparece Canvas de créditos con `Apellido Nombre – NRC` de cada integrante.
- [ ] Sin errores en `console-get-logs`.

---

### 🎛️ FASE 6 — HUD (panel de puntos) global
**Objetivo:** Panel único y coherente en todas las escenas jugables.

**Tareas**
- `HUDController.cs` + Canvas persistente (o instanciado por escena) mostrando **Vida · Puntos · Ítems (x/2) · Enemigos destruidos**.
- Suscribir el HUD a eventos del `GameManager` (actualización en tiempo real).
- Barra/valor de vida enlazada a `PlayerHealth`.

**✅ Gate 6** *(rúbrica: panel de puntos)*
- [ ] HUD visible en Niveles 1, 2 y Final.
- [ ] Vida baja al recibir daño; puntos suben al matar; ítems suben al recoger; kills suben en horda (evidencia en 1 playthrough).

---

### 🔊 FASE 7 — Audio (música + SFX)
**Objetivo:** Sonido completo y coherente.

**Tareas**
- `AudioManager.cs`: música de fondo por escena (loop) + SFX: salto, ataque, recoger ítem, daño, muerte enemigo, apertura de puerta.
- Enlazar SFX a eventos (Animator events o llamadas desde scripts).

**✅ Gate 7** *(rúbrica: sonido)*
- [ ] Música de fondo suena en cada escena.
- [ ] SFX se disparan en sus eventos (evidencia: playthrough con audio).

---

### 🧪 FASE 8 — Integración final, QA y build
**Objetivo:** Playthrough completo end-to-end + limpieza.

**Tareas**
- Verificar Build Settings: orden `00→01→02→03`.
- `tests-run` (si hay tests) + `console-clear-logs` → playthrough completo → `console-get-logs`.
- Ajustes de balance (velocidad enemigos, vida, spawns).
- `scene-save` en todas; opcional: build de PC.

**✅ Gate 8 (DoD global)**
- [ ] Playthrough completo: Splash → Rampas → Hangar → Final → Créditos, **sin errores rojos**.
- [ ] Todos los ítems de §1 (Definition of Done) marcados.
- [ ] Rúbrica §12 cubierta al 100%.

---

## 5. Mapa de herramientas MCP por tarea (referencia rápida)

| Necesidad | Herramientas MCP a usar |
|---|---|
| Crear/abrir/guardar escenas | `scene-create`, `scene-open`, `scene-save`, `scene-set-active`, `scene-list-opened` |
| Crear/mover objetos | `gameobject-create`, `gameobject-duplicate`, `gameobject-destroy`, `gameobject-set-parent`, `gameobject-find`, `gameobject-modify` |
| Componentes (Animator, Colliders, ParticleSystem, Audio…) | `gameobject-component-add`, `gameobject-component-modify`, `gameobject-component-get`, `gameobject-component-list-all` |
| Scripts C# | `script-update-or-create`, `script-read`, `script-execute`, `assets-refresh` |
| Prefabs | `assets-prefab-create`, `assets-prefab-instantiate`, `assets-prefab-open`, `assets-prefab-save` |
| Materiales/assets | `assets-material-create`, `assets-find`, `assets-find-built-in`, `assets-get-data`, `assets-modify` |
| Play / estado editor | `editor-application-get-state`, `editor-application-set-state`, `editor-selection-set` |
| **Validación (Gates)** | `screenshot-game-view`, `screenshot-scene-view`, `console-get-logs`, `console-clear-logs`, `tests-run` |
| Paquetes UPM | `package-add`, `package-list`, `package-search` |
| Extensiones opcionales | AI Animation, AI Navigation (NavMesh), AI ParticleSystem, AI Cinemachine |

---

## 6. Reglas para el agente (guardrails)

1. **Una fase a la vez.** No empieces la fase N+1 hasta que el **Gate N** pase con evidencia.
2. **Evidencia obligatoria** en cada Gate: al menos 1 `screenshot-game-view` **y** `console-get-logs` limpio.
3. **Commit lógico por fase** (si el repo está bajo control de versiones): mensaje = *por qué*, no el *qué*.
4. **Reutiliza Prefabs y sistemas** (no dupliques lógica): Player, Enemy, HUD, AudioManager son singletons/prefabs.
5. **Main thread:** las llamadas a la API de Unity van en el hilo principal.
6. **No inventes assets:** usa `assets-find` primero; si falta un asset crítico, **detente y repórtalo** (no lo simules).
7. **Datos del equipo:** los créditos requieren `Apellido Nombre – NRC` reales → pídelos si no están.

---

## 7. Datos que el agente debe pedir antes de empezar (placeholders)

- [ ] **Integrantes + NRC** para créditos: `Apellido Nombre – NRC` (×N).
- [ ] Rutas exactas de los assets importados (personaje, enemigos, zombies, rampas, hangar, clips de audio).
- [ ] ¿Tercera o primera persona? (por defecto: tercera persona).
- [ ] Plataforma objetivo del build (PC por defecto; hay perfil Móvil URP disponible).

---

## 8. Riesgos y mitigaciones

| Riesgo | Mitigación |
|---|---|
| Ruta con espacios rompe MCP | **Fase 0**: copiar a ruta sin espacios (bloqueante). |
| Assets no coinciden con lo asumido | `assets-find` en Fase 0; detener y reportar si falta algo. |
| Animator sin transiciones → T-pose | Definir parámetros y transiciones explícitas; validar en Gate 2. |
| Enemigos sin NavMesh | Usar `com.unity.ai.navigation` (ya instalado) o patrulla por waypoints. |
| `script-execute` (Roslyn) ejecuta C# arbitrario | Úsalo solo para prototipos; el código de producción va en `script-update-or-create`. |
| Ports desalineados (plugin vs server) | Ambos en **8080**. |

---

## 9. Estructura de carpetas objetivo

```
Assets/
├─ Scenes/           00_Splash · 01_Rampas · 02_Hangar · 03_Final
├─ Scripts/          GameManager, PlayerController, PlayerHealth, EnemyAI,
│                    ZombieHorde, Collectible, MovingRamp, HUDController,
│                    AudioManager, SceneFlow
├─ Prefabs/          Player, Enemy, Zombie, Collectible, MovingRamp, HUD
├─ Animators/        Player.controller, Enemy.controller, Zombie.controller
├─ Audio/            music/*, sfx/*
└─ (assets importados del usuario)
```

---

## 10. Secuencia recomendada (resumen visual)

```
FASE 0  Entorno + MCP        →  Gate 0 (Unity accesible, ruta OK, assets OK)
FASE 1  Splash               →  Gate 1 (≤20s, transición)
FASE 2  Personaje + Anim     →  Gate 2 (Idle/Run/Attack/Jump)   ← rúbrica anim
FASE 3  Nivel 1 Rampas       →  Gate 3 (rampas móviles suben/bajan)
FASE 4  Nivel 2 Hangar       →  Gate 4 (10 enemigos, 2 ítems, puerta, partículas)
FASE 5  Final + Créditos     →  Gate 5 (≥5 zombies, créditos NRC)   ← rúbrica flujo
FASE 6  HUD                  →  Gate 6 (vida/puntos/ítems/kills)  ← rúbrica panel
FASE 7  Audio                →  Gate 7 (música + SFX)             ← rúbrica sonido
FASE 8  QA + Build           →  Gate 8 (playthrough end-to-end sin errores)
```

---

## 11. SUPER-PROMPT para Antigravity 2.0 (pegar al iniciar)

> Copia este bloque como primer mensaje al agente. Está optimizado para el MCP de Unity y el flujo por fases.

```
Eres "AI Game Developer", un agente experto en Unity 6 (URP) conectado a mi
Editor de Unity EN VIVO mediante el MCP "ai-game-developer" (IvanMurzak/Unity-MCP,
puerto 8080). Vas a construir un videojuego 3D completo siguiendo el archivo
PRD_Videojuego_Unity_MCP.md que te adjunto, fase por fase.

CONTEXTO:
- Unity 6000.4.9f1, URP 17.4.0, New Input System (InputSystem_Actions ya define
  Player: Move/Look/Attack/Interact/Crouch/Jump/Sprint + mapa UI).
- Los assets del juego (personaje animado, enemigos, zombies, rampas, hangar,
  audio) YA están importados. Localízalos con `assets-find` antes de crear nada.
- El proyecto DEBE estar en una ruta sin espacios (requisito del MCP).

REGLAS ESTRICTAS:
1. Trabaja UNA FASE A LA VEZ, en el orden del PRD (Fase 0 → 8).
2. NO avances a la siguiente fase hasta pasar su "✅ Gate" al 100%.
3. Para CADA Gate presenta evidencia: al menos un `screenshot-game-view`
   (o `screenshot-scene-view`) y un `console-get-logs` SIN errores rojos.
4. Antes de crear assets, verifícalos con `assets-find`. Si falta un asset
   crítico, DETENTE y pídemelo — no lo simules ni lo inventes.
5. Reutiliza Prefabs y sistemas (GameManager, Player, Enemy, HUD, AudioManager
   son singletons/prefabs). No dupliques lógica.
6. Escribe el código de producción con `script-update-or-create` (+`assets-refresh`),
   no con `script-execute`.
7. Al final de cada fase, dame un resumen: qué hiciste, evidencia del Gate,
   y qué sigue. Espera mi "OK" o corrige si el Gate falla.

DATOS QUE NECESITO DE TI ANTES DE EMPEZAR (pregúntame en la Fase 0):
- Integrantes y NRC para los créditos (formato: "Apellido Nombre – NRC").
- Rutas exactas de los assets importados.
- Cámara: ¿tercera o primera persona? (por defecto tercera).

Empieza por la FASE 0: verifica la conexión MCP con `editor-application-get-state`,
confirma que la ruta no tiene espacios, lista los assets clave con `assets-find`,
y reporta el Gate 0. No sigas sin mi confirmación.
```

---

## 12. Matriz de trazabilidad con la RÚBRICA de evaluación

| Criterio de la rúbrica | Fase que lo cubre | Cómo se valida (evidencia MCP) |
|---|---|---|
| El personaje posee animaciones diferenciadas (espera, ataque, salto…) | **Fase 2** (Gate 2) | 4 `screenshot-game-view`: Idle/Run/Attack/Jump |
| Panel de puntos: vida, puntos, cantidad de adversarios… | **Fase 6** (Gate 6) + Fase 4 | HUD visible mostrando vida/puntos/ítems/kills en playthrough |
| Efectos de sonido y música de fondo acordes al juego | **Fase 7** (Gate 7) | Playthrough con audio: música por escena + SFX en eventos |
| Al concluir un nivel se pasa al siguiente y a créditos finales | **Fases 1,3,4,5** (Gates 1,3,4,5) | Transiciones Splash→R→H→Final→Créditos sin errores |
| La lógica del juego explicada por cada integrante | *(Presentación oral)* | El PRD documenta cada sistema (§3) para que el equipo lo explique |
| Video explicando el paso de estados de animación | *(Video del equipo)* | Animator Controllers documentados (§3) + Gate 2 muestra transiciones |
| Explicación de cómo opera el Panel de puntos | *(Presentación oral)* | `HUDController` + eventos `GameManager` documentados (§3, Fase 6) |

> **Nota:** los 3 últimos criterios de la rúbrica son de **presentación/video del equipo**, no de código. El PRD entrega la documentación (§3, §10) para que cada integrante los sustente con consistencia.

---

*Fin del PRD. Ejecutar con Antigravity 2.0 + Unity-MCP, fase por fase, validando cada Gate.*
