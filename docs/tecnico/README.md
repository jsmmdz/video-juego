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
| `decisiones-abiertas.md` | Contradicciones y decisiones pendientes de confirmar |
| `convenciones.md` | Nomenclatura, ramas, commits |
