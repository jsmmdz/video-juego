# Técnico

Especificación de programación de *The Silent Divide*.

La **mecánica central** es un sistema de sospecha: Nero se infiltra en Aurea, hay zonas
vigiladas; si permanece en ellas la sospecha sube, si sale baja, y al llegar al máximo lo
detectan.

La lógica está planteada en cuatro módulos, cada uno con su flowchart en el board de Figma
(sección *PROGRAMATION*, nodo `111:177`). El prototipo se arma con **primitivas** mientras
llegan los assets.

| # | Módulo | Documento | Nodo Figma |
|---|---|---|---|
| 0 | Arquitectura y orden de ejecución | `00-arquitectura.md` | — |
| 1 | Movimiento del jugador | `01-movimiento.md` | `50:170` |
| 2 | Sistema de sospecha | `02-sospecha.md` | `50:202` |
| 3 | Cámara seguidora | `03-camara.md` | `50:215` |
| 4 | Vista del personaje (billboard) | `04-vista-personaje.md` | `50:222`, `51:254` |

| Otros | |
|---|---|
| `setup-unity.md` | Cómo abrir el proyecto y verificar los módulos |
| `decisiones-abiertas.md` | Contradicciones y decisiones pendientes de confirmar |
| `convenciones.md` | Nomenclatura, ramas, commits |

## Implementación

**Motor: Unity + C#.** El código está en `Assets/Scripts/`, un archivo por módulo:

| Módulo | Script |
|---|---|
| Movimiento | `Assets/Scripts/Player/PlayerMovement.cs` |
| Sospecha | `Assets/Scripts/Suspicion/SuspicionSystem.cs` |
| Cámara | `Assets/Scripts/CameraSystem/FollowCamera.cs` |
| Vista del personaje | `Assets/Scripts/Rendering/DirectionalBillboard.cs` |

El orden de ejecución dentro del fotograma se fija con `Assets/Scripts/ExecutionOrder.cs`, no se
deja al azar: Unity no garantiza el orden entre componentes en `LateUpdate` sin una prioridad
explícita.
