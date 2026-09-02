# The Silent Divide

Juego de aventura, infiltración y **sigilo social**. El jugador es **Nero**, un joven de **Umbria**
—el mundo inferior, oscuro y superpoblado— que se infiltra con identidad falsa en **Aurea**
—la ciudad superior, luminosa y controlada— para descubrir las mentiras del sistema y
destruir la **Barrera** que separa a ambas sociedades.

> **Mecánica central:** un sistema de sospecha. Hay zonas vigiladas; si Nero permanece en
> ellas la sospecha sube, si sale baja, y si llega al máximo lo detectan.

## Estado

**Prototipo jugable con primitivas.** Los cuatro módulos están implementados en **Unity + C#**
y se pueden probar sin ningún arte final.

| Módulo | Diagramado | Implementado | Probado en Unity |
|---|---|---|---|
| Movimiento del jugador | ✅ | ✅ | ⬜ |
| Sistema de sospecha | ✅ | ✅ | ⬜ |
| Cámara seguidora | ✅ | ✅ | ⬜ |
| Vista del personaje (billboard) | ✅ | ✅ | ⬜ |

Para probarlo: abre el repositorio como proyecto de Unity y usa el menú
**The Silent Divide ▸ Construir escena de prototipo**. Guía completa en
[setup-unity.md](docs/tecnico/setup-unity.md).

## Documentación

```
docs/
├── contexto/     material original, sin editar
│   ├── 00-fuentes.md          índice de fuentes y autorías
│   ├── pitch-pdf.md           transcripción del PDF de pitch
│   ├── figma-board.md         volcado del board de Figma
│   └── fuentes/               archivos originales
├── diseno/       diseño interpretado
│   ├── mundo-y-narrativa.md   premisa, personajes, arco, referencias
│   ├── escenarios.md          escalas y elementos de Aurea y Umbria
│   ├── paletas.md             paletas de color
│   └── sistemas-de-juego.md   perfiles, atributos, inventario, detección
└── tecnico/      especificación de programación
    ├── setup-unity.md         cómo abrir y probar el proyecto
    ├── 00-arquitectura.md     orden de ejecución dentro del fotograma
    ├── 01-movimiento.md       ┐
    ├── 02-sospecha.md         │ un documento por flowchart,
    ├── 03-camara.md           │ transcrito y anotado
    ├── 04-vista-personaje.md  ┘
    ├── decisiones-abiertas.md contradicciones y decisiones pendientes
    └── convenciones.md        nomenclatura, código, git
```

El código vive en `Assets/Scripts/`, un archivo por módulo del flowchart. Cada script cita en su
cabecera el nodo de Figma y el documento que implementa.

Plan de trabajo: [ROADMAP.md](ROADMAP.md)

## Fuentes

- Documento de pitch (PDF) — `docs/contexto/fuentes/The_Silent_Divide.pdf`
- Board de Figma — [The Silent Divide](https://www.figma.com/board/Lyg84G1ZWuFOkAsSqYbnkN/The-Silent-Divide)

## Equipo

| Área | Responsable |
|---|---|
| Ilustración, mundo, narrativa | Juliana |
| Diseño de botones (boceto) | Mariana Fuentes |
| Desarrollo | — |

Parte del arte conceptual fue generado con IA (Gemini) a partir de sketches propios; el board
lo marca explícitamente en cada pieza.
