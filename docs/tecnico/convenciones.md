# Convenciones

## Nombres canónicos

| Concepto | Escritura |
|---|---|
| Juego | The Silent Divide |
| Mundo superior | Aurea |
| Mundo inferior | Umbria (sin tilde) |
| Protagonista | Nero |
| Antagonista | Elías Varen |
| La división física | la Barrera (mayúscula) |

## Código

- Idioma del **código** (clases, variables, funciones): **inglés**.
- Idioma de **comentarios y documentación**: **español**.
- Nombres de dominio se mantienen sin traducir: `Suspicion`, `Nero`, `AureaZone`,
  `UmbriaZone`.

Ejemplo:

```
// La sospecha sube o baja, nunca las dos en el mismo fotograma.
private float suspicion;
```

## Módulos

Un archivo por módulo del flowchart, con el mismo nombre que el documento:

| Módulo | Archivo de código |
|---|---|
| Movimiento | `PlayerMovement` |
| Sospecha | `SuspicionSystem` |
| Cámara | `FollowCamera` |
| Vista del personaje | `DirectionalBillboard` |

Cada uno debe poder leerse al lado de su flowchart y reconocerse paso a paso.

## Git

### Ramas

- `main` — estado estable.
- `claude/<nombre>` — ramas de trabajo de esta sesión.
- `feat/<modulo>` — una rama por módulo del prototipo.

### Commits

Mensajes en español, en imperativo, describiendo el *qué* y el *por qué*:

```
Implementa el sistema de sospecha con saturación en ambos extremos

El piso en cero evita que la sospecha se vuelva negativa fuera de las
zonas vigiladas y le dé al jugador un colchón invisible.
```

### Qué NO se versiona

Ver `.gitignore`. En particular: carpetas de compilación del motor, cachés de importación de
assets y builds. Los assets de arte sí se versionan.
