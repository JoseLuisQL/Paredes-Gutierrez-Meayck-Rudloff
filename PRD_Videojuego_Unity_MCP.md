# PRD — Videojuego 3D en Unity 6 (URP) construido con IA vía MCP

> **Documento de Requisitos de Producto (PRD) + Plan de Ejecución por Fases**
> Diseñado para ejecutarse con un agente de IA (**Antigravity 2.0**) conectado en tiempo real a Unity mediante el MCP **[IvanMurzak/Unity-MCP — "AI Game Developer: Unity SKILLS, MCP"](https://github.com/IvanMurzak/Unity-MCP)**.
>
> **Regla de oro:** el agente NO avanza a la siguiente fase hasta que la **Puerta de Validación (✅ Gate)** de la fase actual pasa al 100%. Cada Gate se verifica con herramientas MCP reales (screenshots, logs, play-mode, tests).
>
> **📌 Versión 2 — Actualizado tras `git pull`.** El usuario importó los assets reales del juego (arte, animaciones, scripts base, escenas y prefabs). Este PRD ya NO asume assets genéricos: refleja el **inventario real del repositorio** (ver §0.B) y **reutiliza los scripts existentes** en lugar de reescribirlos. Las fases se reordenaron para *integrar y completar* lo que ya hay.

---

## 0. Contexto del proyecto (verificado)

| Dato | Valor |
|---|---|
| **Motor** | Unity `6000.4.9f1` (Unity 6) |
| **Render Pipeline** | Universal Render Pipeline (URP `17.4.0`) — perfiles PC y Móvil ya configurados |
| **Input** | New Input System `1.19.0` (`InputSystem_Actions` ya define Player: Move, Look, Attack, Interact, Crouch, Jump, Sprint + mapa UI) |
| **Producto** | `Paredes Gutierrez Meayck Rudloff` (v0.1.0) |
| **Estado actual** | Assets del juego **importados**: arte (Skeleton, Zombie, armas), escenarios (rampas, esferas), scripts base propios, 3 escenas y prefabs. Falta: encadenar escenas, completar lógica y cumplir cantidades de la rúbrica. |
| **Orquestador** | Agente IA Antigravity 2.0 |
| **Puente en tiempo real** | Unity-MCP (OpenUPM `com.ivanmurzak.unity.mcp`), puerto `8080` |

### ⚠️ Bloqueante crítico detectado (resolver en Fase 0)
El MCP de Unity **NO funciona si la ruta del proyecto contiene espacios**. El proyecto se llama `Paredes Gutierrez Meayck Rudloff` (con espacios) → **debe copiarse/moverse a una ruta sin espacios** (ej. `ParedesGutierrezMeayckRudloff/`) antes de conectar el MCP. Esto es un requisito duro del bridge stdio. **Además** varias carpetas de assets tienen espacios (`Standard Assets`, `Asset Packs`) — son rutas *internas* de assets y no rompen el MCP (solo la raíz del proyecto importa), pero conviene no crear nuevas carpetas con espacios.

---

## 0.B Inventario REAL de assets (verificado en el repo tras `git pull`)

> Tamaño total del repo: ~8.2 MB. Todo esto **ya existe** en `Assets/` — el agente debe **reutilizarlo**, no recrearlo. Localizar con `assets-find` antes de tocar nada.

### 🎭 Personajes y enemigos (arte + animaciones)
| Recurso | Ruta | Uso previsto |
|---|---|---|
| **Skeleton** (FantasyMonster) | `Assets/FantasyMonster/Skeleton/` | Enemigo principal. FBX con animaciones separadas: `Attack, Damage, Death, Idle, Knockback, Run, Skill, Stand, Walk` + `Skeleton@Skin.FBX` (malla) |
| **Zombie** | `Assets/Asset Packs/Zombie/` | Zombies de la escena Final. `Zombie.prefab`, `Zombie.controller` y clips `attack, idle, walk, fallingback, walk_in_place` |
| **Enemy** (prefab) | `Assets/Prefabs/Characters/Enemy.prefab` | Enemigo genérico ya montado |
| **Player** (prefab) | `Assets/Prefabs/Characters/Player.prefab` | Personaje jugable ya montado |
| **Enemy.controller** | `Assets/Animations/Enemy.controller` | Animator del enemigo con estados **Idle, Move, Attack, Die** (parámetros `idle/move/attack/die`) |

### 🔫 Armas y disparo (FPS)
| Recurso | Ruta |
|---|---|
| Armas (prefabs) | `Assets/Prefabs/Weapons/` → `Pistol`, `Shotgun`, `Carbine` |
| Malla Carbine | `Assets/Asset Packs/Survival Carbine/` (meshes, materiales, texturas) |
| Munición/pickups | `Assets/Prefabs/Pickups/` → `Ammo`, `Bullets`, `Rockets`, `Shells`, `Shotgun Shell`, `Bullet` |
| VFX disparo | `Assets/Prefabs/VFX/Muzzle Flash - Orange.prefab` |
| Linterna | `Assets/Prefabs/Lights/Flashlight.prefab` |

### 🏗️ Escenarios y props
| Recurso | Ruta | Uso |
|---|---|---|
| **Rampa** | `Assets/Mi_Escenario/Modelos/Rampa.fbx` + `Prefabs/Rampa Variant.prefab` | **Nivel 1 (Rampas)** — duplicar y animar sube/baja |
| Suelo / Pared | `Assets/Mi_Escenario/Modelos/Suelo_1.fbx`, `Pared_Principal_2.fbx`, `Prefabs/Piso2 Variant.prefab` | Construcción de niveles |
| Materiales escenario | `Assets/Mi_Escenario/Materiales/` (`Piso`, `caja_1`, `Esfera`, `Pared_computo`) | |
| **Puerta** | `Assets/Prefabs/Puerta.prefab` | Puerta final del Hangar (usa `ctrlPuerta`/`ctrlPuerta2`) |
| Standard Assets | `Assets/Standard Assets/` (Cameras, Characters, CrossPlatformInput, Environment, ParticleSystems, Utility) | FPSController, cámaras, **partículas**, waypoints |
| Prototyping / Water | `Assets/Asset Packs/Standard Assets/` (Prototyping, ParticleSystems, Water) | Materiales y **efectos de partículas** para zona de intercambio |
| ProGrids | `Assets/ProCore/ProGrids/` | Herramienta de grid (editor) |

### 🔊 Audio
| Recurso | Ruta |
|---|---|
| Efectos | `Assets/audios/` → `shot.mp3`, `door open.mp3`, `door close.mp3` |
| *(Falta)* | Música de fondo por escena → **conseguir/asignar** (ver Fase 7) |

### 🎬 Escenas existentes
| Escena | Ruta | Contenido actual | Rol destino |
|---|---|---|---|
| `SampleScene` | `Assets/Scenes/SampleScene.unity` | Plantilla (cámara, luz, volume) | Reconvertir a **Splash** o Nivel 1 |
| `Escenario2` | `Assets/Scenes/Escenario2.unity` | Esferas, `Fin` (trigger de fin), luz | Base para **Nivel 1 / Rampas** |
| `Sandbox` | `Assets/Scenes/Sandbox.unity` | Jerarquía `Environment / Enemies / Pickups` + `NavMesh.asset` | Base para **Nivel 2 / Hangar** (¡ya tiene NavMesh horneado!) |

### 🧩 Scripts propios existentes (reutilizar — NO reescribir)
| Script | Qué hace hoy | Estado vs. rúbrica |
|---|---|---|
| `controladorPersonaje.cs` | Movimiento CharacterController + salto + **vida (Slider `sldVida`)** + muerte (`death`) | ✅ Base del jugador; falta enganchar animaciones |
| `controlPersonaje1.cs` | Movimiento simple alterno | Alternativa |
| `SoldadoControl.cs` / `SoldadoControl2.cs` | Movimiento + **Animator** (`Caminando`, `Corriendo`, `Velocidad`) + rotación con mouse | ✅ Personaje animado (3ª persona) |
| `controlCamara.cs` | Cámara orbital que sigue al player | ✅ Cámara 3ª persona |
| `Weapon.cs` | **Disparo por raycast**, decals, daño a "Caja", muzzle | ✅ Ataque a distancia |
| `DestruirCajas.cs` | Vida/daño de objetos destruibles | ✅ Base para "destruir enemigos" |
| `DestruirDecal.cs` | Autodestruye decals de bala | ✅ |
| `ctrlPlayer.cs` | **Contadores coins + points** (TextMeshPro) + trigger `Fin` → menú continuar | ✅ Base del HUD (falta vida/kills/items) |
| `ctrlCoin.cs` | Ítem recogible → suma coin + sonido + destruye | ✅ Base de **Collectible** |
| `ctrlPuerta.cs` / `ctrlPuerta2.cs` | Puerta corrediza (Z / X) con sonidos open/close (particle comentado) | ✅ Puerta final; **descomentar partículas** |
| `death.cs` | Panel de muerte + reiniciar nivel | ✅ |
| `ContinueMenu.cs` | Panel "continuar" + reiniciar (falta cargar siguiente escena) | ⚠️ Ampliar para **cargar siguiente nivel** |
| `tecla.cs` / `tocarCancion.cs` | Piano interactivo (teclas con sonido) | ➖ Extra, no requerido por rúbrica |

### 🕳️ BRECHAS vs. la consigna (lo que el agente debe crear/completar)
| Falta | Dónde se resuelve |
|---|---|
| **Escena Splash** ≤ 20 s | Fase 1 |
| **Encadenar las 4 escenas** en Build Settings + transiciones | Fases 1,3,4,5 + `ContinueMenu` ampliado |
| **Rampas móviles** (sube/baja) — script `MovingRamp` | Fase 3 |
| **≥ 10 enemigos** colocados en el Hangar | Fase 4 |
| **2 ítems** que abren la **puerta final** + **partículas** en zona de intercambio | Fase 4 (usar `ctrlCoin` + `ctrlPuerta` + ParticleSystem de Standard Assets) |
| **Horda ≥ 5 zombies** con conteo de destruidos → créditos | Fase 5 (Zombie prefab + `DestruirCajas` adaptado) |
| **HUD completo**: vida + puntos + ítems (x/2) + kills | Fase 6 (ampliar `ctrlPlayer`) |
| **Animaciones del jugador**: Idle/Run/Attack/Jump enlazadas | Fase 2 (usar `SoldadoControl2` + Animator) |
| **Música de fondo** por escena | Fase 7 |
| **Créditos** `Apellido Nombre – NRC` | Fase 5 |

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

| # | Escena | Base existente → archivo | Propósito | Requisitos específicos de la consigna |
|---|--------|------------------|-----------|----------------------------------------|
| 0 | **Splash / Inicio** | *(nueva)* `Assets/Scenes/00_Splash.unity` (o reusar `SampleScene`) | Pantalla de inicio tipo splash | Duración **≤ 20 s**, luego auto-transición al Nivel 1 |
| 1 | **Nivel 1 — Rampas** | `Escenario2.unity` → `01_Rampas.unity` | Plataformas/rampas | Reusar `Rampa Variant.prefab`; **agregar más rampas** + **mecánica que suben y bajan** (`MovingRamp`) |
| 2 | **Nivel 2 — Hangar** | `Sandbox.unity` → `02_Hangar.unity` (ya tiene `Environment/Enemies/Pickups` + **NavMesh horneado**) | Sigilo/combate | **≥ 10 enemigos** (Skeleton/Enemy) · **2 ítems** (`ctrlCoin`) en Canvas → **abre `Puerta.prefab`** → **zona de intercambio con partículas** |
| 3 | **Final — Horda + Créditos** | *(nueva)* `03_Final.unity` | Horda | **Zombies** (`Zombie.prefab`) que atacan en horda · destruir **≥ 5** → **Canvas de créditos** con `Apellido Nombre – NRC` |

> **Build order** (`EditorBuildSettings`): `00_Splash` → `01_Rampas` → `02_Hangar` → `03_Final`. *(Hoy Build Settings solo tiene `SampleScene` — el agente debe registrar las 4 en orden.)*
> **Nota:** conservar nombres de escena originales o renombrar; si se renombra, actualizar `ContinueMenu`/`SceneManager.LoadScene`.

---

## 3. Sistemas: qué REUTILIZAR vs. qué CREAR

> Regla: **primero reutilizar los scripts propios existentes** (§0.B). Solo crear scripts nuevos donde hay una brecha real. Todo con `script-update-or-create` + `assets-refresh`.

### 3.A Reutilizar / completar (ya existen)
| Sistema | Script existente | Acción del agente |
|---|---|---|
| Movimiento + vida jugador | `controladorPersonaje.cs` (tiene `sldVida`, `perderVida`, `death`) | Enganchar parámetros del Animator (Idle/Run/Jump/Attack) |
| Movimiento 3ª persona animado | `SoldadoControl2.cs` (`Caminando/Corriendo/Velocidad`) | Usar como controlador principal del Player si se quiere 3ª persona |
| Cámara | `controlCamara.cs` | Asignar `player`/`referencia` |
| Ataque a distancia | `Weapon.cs` (raycast + decals + daño a tag "Caja") | Extender tag/daño para golpear **enemigos/zombies** |
| Objeto con vida/daño | `DestruirCajas.cs` | Adaptar a enemigos: al morir → sumar kill/puntos |
| Ítem recogible | `ctrlCoin.cs` | Usar para los **2 ítems** del Hangar |
| Puerta | `ctrlPuerta.cs` / `ctrlPuerta2.cs` | Abrir al recoger 2 ítems; **descomentar `ParticleSystem`** |
| Contadores UI | `ctrlPlayer.cs` (coins + points TMP) | Ampliar a HUD completo (vida + ítems x/2 + kills) |
| Muerte / reinicio | `death.cs` | OK |
| Menú continuar | `ContinueMenu.cs` | **Ampliar**: en vez de solo reiniciar, `SceneManager.LoadScene(siguiente)` |

### 3.B Crear nuevo (brechas)
| Sistema | Script nuevo | Responsabilidad |
|---|---|---|
| **GameManager** (singleton, `DontDestroyOnLoad`) | `GameManager.cs` | Estado global: vida/puntos/ítems/kills; carga de escenas; eventos al HUD |
| **MovingRamp** | `MovingRamp.cs` | Rampa que sube/baja (Lerp ping-pong entre 2 puntos) |
| **EnemyAI** | `EnemyAI.cs` | IA Skeleton/Enemy: patrulla/persigue con NavMesh; dispara `Enemy.controller` (Idle/Move/Attack/Die); daña al player; al morir suma kill |
| **ZombieHorde** | `ZombieHorde.cs` | Spawner de horda (Zombie.prefab); cuenta destruidos ≥ 5 → créditos |
| **SplashTimer** | `SplashTimer.cs` | Temporizador ≤ 20 s → carga Nivel 1 |
| **Credits** | `Credits.cs` | Muestra Canvas con `Apellido Nombre – NRC` |

### 3.C Animator Controllers
- **Player**: crear/ajustar controller con `Idle ⇄ Run`, `→ Jump`, `→ Attack` (parámetros que ya usa `SoldadoControl2`: `Caminando`, `Corriendo`, `Velocidad`; añadir `Jump`/`Attack` triggers). Fuente de clips: FantasyMonster Skeleton o el rig del Player prefab.
- **Enemy**: `Assets/Animations/Enemy.controller` (ya tiene **Idle/Move/Attack/Die**) — **reutilizar tal cual**.
- **Zombie**: `Assets/Asset Packs/Zombie/Animations/Zombie.controller` (attack/idle/walk/fallingback) — **reutilizar**.

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
5. Verificar assets importados con `assets-find` según el **inventario §0.B**:
   - Skeleton (`Assets/FantasyMonster/Skeleton/`), Zombie (`Assets/Asset Packs/Zombie/`), prefabs `Player`/`Enemy`.
   - `Enemy.controller` y `Zombie.controller` (estados de animación).
   - Rampa (`Assets/Mi_Escenario/`), `Puerta.prefab`, armas y pickups.
   - Audios (`Assets/audios/shot.mp3`, `door open/close.mp3`).
   - Escenas `Escenario2`, `Sandbox` (con NavMesh), `SampleScene`.
   - Scripts propios en `Assets/Scripts/` (confirmar que compilan).

**✅ Gate 0** — *no continuar hasta que TODO pase:*
- [ ] `editor-application-get-state` responde (Unity accesible por MCP).
- [ ] Ruta del proyecto **sin espacios** confirmada.
- [ ] `assets-find` confirma los assets clave del §0.B (Skeleton, Zombie, Rampa, Puerta, armas, audios, escenas).
- [ ] `console-get-logs` sin errores de compilación (los scripts propios compilan).

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

**Tareas** *(reutilizar lo existente — §0.B / §3.A)*
- Abrir `Escenario2.unity` → guardar como `01_Rampas.unity` (`scene-open` + `scene-save`).
- Usar el **`Player.prefab` existente** (`assets-prefab-instantiate`); si se opta por 3ª persona, montar `SoldadoControl2.cs` + `controlCamara.cs`; si 1ª persona, FPSController de Standard Assets + `Weapon.cs`.
- **Animator del Player**: crear/ajustar controller con `Idle ⇄ Run`, `→ Jump`, `→ Attack`. Reusar los parámetros que ya emite `SoldadoControl2` (`Caminando`, `Corriendo`, `Velocidad`) y añadir triggers `Jump`/`Attack`. Clips de FantasyMonster Skeleton o del rig del Player.
- Enlazar salto/ataque a los triggers del Animator (editar el script existente, no crear uno nuevo salvo necesidad).
- Guardar cambios en `Player.prefab` (`assets-prefab-save`).

**✅ Gate 2** *(rúbrica: animaciones del personaje)*
- [ ] En Play: quieto → **Idle**; moverse → **Run**; botón salto → **Jump**; botón ataque → **Attack** (evidencia: 4 `screenshot-game-view` o clip).
- [ ] Transiciones limpias, sin *T-pose* ni deslizamiento evidente.
- [ ] Sin errores en `console-get-logs`.

---

### 🧗 FASE 3 — Nivel 1: Rampas + rampas móviles
**Objetivo:** Completar `01_Rampas` con más rampas y mecánica sube/baja.

**Tareas**
- Instanciar/duplicar `Assets/Mi_Escenario/Prefabs/Rampa Variant.prefab` (`assets-prefab-instantiate` + `gameobject-duplicate`) → **más rampas que el set original**.
- `script-update-or-create` **`MovingRamp.cs`** (Lerp entre `pointA`/`pointB`, velocidad configurable, ping-pong).
- Añadir `MovingRamp` a ≥ 2 rampas; verificar `Collider`s (usar `Suelo_1`/`Piso2 Variant` como base).
- Reusar el objeto **`Fin`** (trigger) que ya existe en `Escenario2` → al entrar, `ContinueMenu`/`GameManager` carga `02_Hangar`.
- Música de nivel + SFX de salto (ver Fase 7).

**✅ Gate 3**
- [ ] Personaje sube por las rampas y salta entre plataformas (screenshots).
- [ ] Rampas móviles **suben y bajan** de forma visible y continua.
- [ ] Al llegar a la meta → transición a Nivel 2 sin errores.

---

### 🛩️ FASE 4 — Nivel 2: Hangar (enemigos + ítems + puerta + partículas)
**Objetivo:** `02_Hangar` con ≥10 enemigos, 2 ítems, puerta que se abre, zona de intercambio con partículas.

**Tareas** *(base: `Sandbox.unity`, que ya trae `Environment/Enemies/Pickups` + NavMesh horneado)*
- Abrir `Sandbox.unity` → guardar como `02_Hangar.unity`.
- Poblar el grupo **`Enemies`** con **≥ 10 enemigos**: instanciar `Enemy.prefab` o Skeleton (FantasyMonster) en ubicaciones distintas; asignar `Enemy.controller` (Idle/Move/Attack/Die) + `EnemyAI.cs` (nuevo) usando el **NavMesh ya horneado**.
- Colocar **2 ítems** con `ctrlCoin.cs` en el grupo `Pickups`; el HUD muestra progreso (0/2 → 2/2).
- **Puerta final** (`Puerta.prefab` + `ctrlPuerta`/`ctrlPuerta2`): cerrada por defecto; `GameManager` la habilita al recoger 2 ítems.
- **Zona de intercambio**: instanciar un `ParticleSystem` de `Assets/Asset Packs/Standard Assets/ParticleSystems/` **y** descomentar el `particle.Play()` en `ctrlPuerta`.
- Trigger tras la puerta → carga `03_Final`.

**✅ Gate 4** *(rúbrica: panel de puntos, ítems, adversarios)*
- [ ] `gameobject-find` confirma **≥ 10** enemigos en la escena.
- [ ] Enemigos animan Idle/Move/Attack/Die y persiguen/atacan (screenshots).
- [ ] Recoger 2 ítems → HUD 2/2 → **la puerta se abre** (evidencia visual).
- [ ] Zona de intercambio muestra **partículas** activas.
- [ ] Transición a Final sin errores.

---

### 🧟 FASE 5 — Escena Final: Horda + Créditos
**Objetivo:** `03_Final` con horda de zombies (destruir ≥ 5) → Canvas de créditos.

**Tareas**
- `scene-create` → `03_Final.unity`; suelo base (Prototyping) + hornear NavMesh si se usa.
- `ZombieHorde.cs` (nuevo) spawnea **`Assets/Asset Packs/Zombie/Prefabs/Zombie.prefab`** en horda; `Zombie.controller` da idle/walk/attack/fallingback.
- Combate: `Weapon.cs` (o ataque) destruye zombies vía `DestruirCajas` adaptado (tag `Zombie`); `GameManager` cuenta kills; al llegar a **≥ 5** → dispara créditos.
- **Canvas de créditos** (`Credits.cs`): por integrante `Apellido Nombre – NRC` (datos reales — §7).
- Música final / créditos (ver Fase 7).

**✅ Gate 5** *(rúbrica: niveles → créditos finales)*
- [ ] Zombies aparecen en horda y atacan (screenshots).
- [ ] Al destruir **≥ 5** → aparece Canvas de créditos con `Apellido Nombre – NRC` de cada integrante.
- [ ] Sin errores en `console-get-logs`.

---

### 🎛️ FASE 6 — HUD (panel de puntos) global
**Objetivo:** Panel único y coherente en todas las escenas jugables.

**Tareas** *(ampliar lo que ya hace `ctrlPlayer.cs`)*
- Partir de `ctrlPlayer.cs` (ya muestra coins + points con TextMeshPro) y **añadir**: barra de **Vida** (Slider `sldVida` que ya usa `controladorPersonaje`), **Ítems (x/2)** y **Enemigos destruidos**.
- Usar `Assets/Prefabs/UI/UI.prefab` como Canvas base si aplica.
- Suscribir el HUD a eventos del `GameManager` (actualización en tiempo real).

**✅ Gate 6** *(rúbrica: panel de puntos)*
- [ ] HUD visible en Niveles 1, 2 y Final.
- [ ] Vida baja al recibir daño; puntos suben al matar; ítems suben al recoger; kills suben en horda (evidencia en 1 playthrough).

---

### 🔊 FASE 7 — Audio (música + SFX)
**Objetivo:** Sonido completo y coherente.

**Tareas**
- **SFX ya disponibles** en `Assets/audios/`: `shot.mp3` (disparo → `Weapon`), `door open.mp3`/`door close.mp3` (→ `ctrlPuerta`, ya cableados). `ctrlCoin` ya reproduce sonido al recoger.
- **Falta música de fondo por escena** (loop) → conseguir/importar clips y asignar un `AudioSource` de fondo por escena (o vía `GameManager`).
- Añadir SFX faltantes: salto, daño al jugador, muerte de enemigo (Animator events o llamadas desde scripts).

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

- [ ] **Integrantes + NRC** para créditos: `Apellido Nombre – NRC` (×N). *(único dato que el repo NO tiene).*
- [ ] ¿Personaje jugable en **1ª persona** (FPSController + `Weapon.cs`) o **3ª persona** (`SoldadoControl2` + `controlCamara`)? El repo soporta ambos.
- [ ] ¿Enemigo principal = **Skeleton** (FantasyMonster) o el **Enemy.prefab** genérico? (Zombies quedan para la escena Final).
- [ ] Música de fondo: ¿tienes clips o el agente debe conseguir libres de derechos?
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
├─ Scenes/              SampleScene · Escenario2(→01_Rampas) · Sandbox(→02_Hangar) · (03_Final nuevo)
├─ Scripts/             [EXISTEN] controladorPersonaje, SoldadoControl(2), controlCamara,
│                       Weapon, DestruirCajas/Decal, ctrlPlayer, ctrlCoin, ctrlPuerta(2),
│                       death, ContinueMenu, tecla, tocarCancion
│                       [NUEVOS] GameManager, MovingRamp, EnemyAI, ZombieHorde,
│                       SplashTimer, Credits
├─ Animations/          Enemy.controller (Idle/Move/Attack/Die)
├─ FantasyMonster/      Skeleton (Idle/Run/Attack/Death/… FBX)
├─ Asset Packs/Zombie/  Zombie.prefab + Zombie.controller
├─ Asset Packs/Survival Carbine/, Prefabs/Weapons/, Prefabs/Pickups/   (armas + munición)
├─ Mi_Escenario/        Rampa, Suelo, Pared + prefabs Variant
├─ Prefabs/             Characters/{Player,Enemy}, Puerta, UI, VFX, Lights
├─ Standard Assets/     Cameras, Characters(FPS), CrossPlatformInput, ParticleSystems, Utility
├─ audios/              shot.mp3, door open/close.mp3   (falta música de fondo)
└─ Settings/, Materials/, ProCore/
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
- Unity 6000.4.9f1, URP 17.4.0, New Input System (aunque los scripts existentes
  usan el Input clásico Input.GetAxis — respétalo, no lo migres sin pedir permiso).
- Los assets del juego YA están importados. Consulta el INVENTARIO REAL en la
  sección §0.B del PRD y reutilízalo. Piezas clave:
    · Enemigo: Assets/FantasyMonster/Skeleton/ (+ Assets/Animations/Enemy.controller)
    · Zombies: Assets/Asset Packs/Zombie/ (Zombie.prefab + Zombie.controller)
    · Jugador: Assets/Prefabs/Characters/Player.prefab
    · Rampa:  Assets/Mi_Escenario/Prefabs/Rampa Variant.prefab
    · Puerta: Assets/Prefabs/Puerta.prefab (scripts ctrlPuerta/ctrlPuerta2)
    · Armas:  Assets/Prefabs/Weapons/ + Weapon.cs
    · Audios: Assets/audios/ (shot, door open/close)
    · Escenas: Escenario2 (→Nivel1), Sandbox (→Nivel2, con NavMesh horneado)
    · Scripts propios en Assets/Scripts/ (ver §0.B — REUTILÍZALOS)
- El proyecto DEBE estar en una ruta sin espacios (requisito del MCP).

REGLAS ESTRICTAS:
1. Trabaja UNA FASE A LA VEZ, en el orden del PRD (Fase 0 → 8).
2. NO avances a la siguiente fase hasta pasar su "✅ Gate" al 100%.
3. Para CADA Gate presenta evidencia: al menos un `screenshot-game-view`
   (o `screenshot-scene-view`) y un `console-get-logs` SIN errores rojos.
4. REUTILIZA lo existente antes de crear. Lee el script con `script-read`,
   extiéndelo con `script-update-or-create`. Solo crea scripts nuevos para las
   BRECHAS listadas en §0.B (GameManager, MovingRamp, EnemyAI, ZombieHorde,
   SplashTimer, Credits). No reescribas ni dupliques lógica que ya funciona.
5. Antes de instanciar, verifica el asset con `assets-find`. Si falta algo
   crítico, DETENTE y pídemelo — no lo inventes.
6. Código de producción con `script-update-or-create` (+`assets-refresh`),
   no con `script-execute`.
7. Al final de cada fase, dame un resumen: qué hiciste, evidencia del Gate,
   y qué sigue. Espera mi "OK" o corrige si el Gate falla.

DATOS QUE NECESITO DE TI ANTES DE EMPEZAR (pregúntame en la Fase 0):
- Integrantes y NRC para los créditos (formato: "Apellido Nombre – NRC").
- ¿Jugador en 1ª persona (FPS + Weapon.cs) o 3ª persona (SoldadoControl2 + controlCamara)?
- ¿Enemigo principal Skeleton o Enemy.prefab? (los zombies son para la escena Final).
- ¿Tienes música de fondo o la consigo libre de derechos?

Empieza por la FASE 0: verifica la conexión MCP con `editor-application-get-state`,
confirma que la ruta no tiene espacios, valida el inventario §0.B con `assets-find`,
comprueba que los scripts de Assets/Scripts/ compilan (`console-get-logs`),
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

*Fin del PRD (v2 — sincronizado con el repo tras `git pull`). Ejecutar con Antigravity 2.0 + Unity-MCP, fase por fase, validando cada Gate. Los assets del §0.B son reales y verificados; reutilízalos.*
