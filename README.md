# The Silent Divide

Juego de aventura, infiltración y **sigilo social**. El jugador es **Nero**, un joven de **Umbria**
—el mundo inferior, oscuro y superpoblado— que se infiltra con identidad falsa en **Aurea**
—la ciudad superior, luminosa y controlada— para descubrir las mentiras del sistema y
destruir la **Barrera** que separa a ambas sociedades.

> **Mecánica central:** un sistema de sospecha. Hay zonas vigiladas; si Nero permanece en
> ellas la sospecha sube, si sale baja, y si llega al máximo lo detectan.

**Perspectiva:** vista cenital oblicua (aérea en ¾), de ángulo fijo.

## Estado

**Prototipo jugable con primitivas.** Los cuatro módulos están implementados en **Unity + C#**
y se pueden probar sin ningún arte final.

| Módulo | Diagramado | Implementado | Probado en Unity |
|---|---|---|---|
| Movimiento del jugador | ✅ | ✅ | ⬜ |
| Sistema de sospecha | ✅ | ✅ | ⬜ |
| Cámara seguidora | ✅ | ✅ | ⬜ |
| Vista del personaje (billboard) | ✅ | ✅ | ⬜ |
| Pantalla de inicio | ✅ | ✅ | ⬜ |
| Ajustes de Sistema | ✅ | ✅ | ⬜ |

Para probarlo: abre el repositorio como proyecto de Unity y usa el menú
**The Silent Divide ▸ Construir escena de prototipo**, o
**▸ Construir menú principal** para la pantalla de inicio. Guía completa en
[setup-unity.md](docs/tecnico/setup-unity.md).

## Documentación

```
docs/
├── contexto/     material original, sin editar
│   ├── 00-fuentes.md          índice de fuentes y autorías
│   ├── pitch-pdf.md           transcripción del pitch v2 (vigente)
│   ├── pitch-cambios-v1-v2.md qué cambió entre versiones
│   ├── narrativa.md           narrativa larga + jugabilidad
│   ├── figma-board.md         volcado del board de Figma
│   └── fuentes/               archivos originales
├── diseno/       diseño interpretado
│   ├── mundo-y-narrativa.md   premisa, personajes, arco, referencias
│   ├── escenarios.md          escalas y elementos de Aurea y Umbria
│   ├── paletas.md             paletas de color
│   └── sistemas-de-juego.md   perfiles, atributos, inventario, detección
└── tecnico/      especificación de programación
    ├── setup-unity.md         cómo abrir y probar el proyecto
    ├── flujo-antigravity.md   trabajo con agentes
    ├── 00-arquitectura.md     orden de ejecución dentro del fotograma
    ├── 01-movimiento.md       ┐
    ├── 02-sospecha.md         │ un documento por flowchart,
    ├── 03-camara.md           │ transcrito y anotado
    ├── 04-vista-personaje.md  ┘
    ├── decisiones-abiertas.md contradicciones y decisiones pendientes
    └── convenciones.md        nomenclatura, código, git
```

El repositorio está preparado para trabajar con agentes: `AGENTS.md` y `.agents/rules/` cargan el
contexto y los invariantes del proyecto. El flujo está en
[flujo-antigravity.md](docs/tecnico/flujo-antigravity.md).

El código vive en `Assets/Scripts/`, un archivo por módulo del flowchart. Cada script cita en su
cabecera el nodo de Figma y el documento que implementa.

Plan de trabajo: [ROADMAP.md](ROADMAP.md)

## Fuentes

- Pitch / Game Design **v2** — `docs/contexto/fuentes/The_Silent_Divide_v2.docx` (editable) y `.pdf`
- Narrativa larga y de estudio — `docs/contexto/fuentes/Narrativa_*.pdf`
- Board de Figma — [The Silent Divide](https://www.figma.com/board/Lyg84G1ZWuFOkAsSqYbnkN/The-Silent-Divide)

Índice completo con autorías y relación entre documentos:
[`docs/contexto/00-fuentes.md`](docs/contexto/00-fuentes.md)

## Equipo

Sara Perilla · Juliana Sanabria · Valentina Sanabria · Mariana Fuentes · Junior Mejía · Samuel Silva

| Área | Responsable |
|---|---|
| Ilustración, mundo, narrativa | Juliana Sanabria |
| Diseño de botones (boceto) | Mariana Fuentes |

Parte del arte conceptual fue generado con IA (Gemini) a partir de sketches propios; el board
lo marca explícitamente en cada pieza.
