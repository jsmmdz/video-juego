# Módulo 3 — Cámara seguidora

**Flowchart:** board de Figma, nodo `50:215`.

> **Confirmado por el pitch v2:** «El juego se desarrolla desde una perspectiva de vista cenital
> oblicua o vista aérea en ¾». La v1 pedía dos perspectivas —tercera persona y vista central—;
> la v2 las sustituye por esta única, que es la que este módulo implementa.

## Diagrama

```
Inicio del juego
       │
       ▼
┌────────────────────────────────────────────┐◄──────┐
│ FIN DEL FOTOGRAMA · YA SE MOVIÓ EL JUGADOR │       │
└──────────────────┬─────────────────────────┘       │
                   ▼                                 │
        ◇ ¿Tiene a quién seguir? ◇──── no ─────────►─┤
                   │ sí                              │
                   ▼                                 │
     Calcular posición deseada                       │
     (jugador + desplazamiento de zona)              │
                   ▼                                 │
     Acercar la cámara                               │
     (suavemente, sin rotar) ────────────────────────┘
```

## Nota del autor del diagrama

> La cámara se actualiza al final del fotograma, cuando el jugador ya cambió de posición. Solo
> sigue su posición: el ángulo lo fija el desplazamiento de cada zona y no se modifica nunca,
> que es lo que sostiene la lectura de escena enmarcada del referente.

## Pseudocódigo

```
finDelFotograma():              # después de que el jugador ya se movió
    si objetivo == nulo:
        return

    posicionDeseada = objetivo.posicion + desplazamientoDeZonaActual
    camara.posicion = suavizar(camara.posicion, posicionDeseada, SUAVIZADO * dt)
    # la rotación de la cámara NO se toca aquí, nunca
```

## Detalles a respetar

- **La cámara nunca rota.** Es la regla más importante del módulo y la que define la identidad
  visual del juego: el ángulo es fijo y viene dado por el `desplazamientoDeZona`. Rotar la
  cámara para seguir al personaje destruiría la composición "de escena enmarcada" que
  persigue el referente, y además rompería el módulo 4, que asume que la cámara define un
  eje estable contra el cual medir la dirección del personaje.
- **Se ejecuta al final del fotograma.** Si corriera antes del movimiento, encuadraría la
  posición del fotograma anterior → temblor perceptible en movimiento continuo.
- **Comprobación de objetivo nulo primero.** Evita errores en carga de escena, cambio de nivel
  y cinemáticas, donde puede no haber jugador todavía.
- **Seguimiento suave, no rígido.** El `suavizar` hace que la cámara vaya "por detrás" del
  jugador con retraso constante. Un seguimiento rígido (copiar la posición) transmite cada
  micro-corrección del movimiento a la imagen.

## El desplazamiento por zona

`desplazamientoDeZona` es el vector desde el jugador hasta la cámara. Al ser por zona y no
global, permite que cada área tenga su propio encuadre — un plano más cerrado en un callejón
de Umbria, uno más abierto en la plaza de Aurea — sin que la cámara jamás gire durante el
juego.

Con el pitch v2 la perspectiva es una sola, así que el desplazamiento por zona ya no sirve para
alternar entre dos sistemas de cámara: sirve para **variar la distancia y el encuadre dentro de la
misma perspectiva** — más cerrado en un callejón de Umbria, más abierto en la plaza de Aurea.

## Parámetros expuestos

| Parámetro | Descripción |
|---|---|
| `objetivo` | Transform a seguir (el jugador) |
| `desplazamientoDeZona` | Vector jugador → cámara, propio de cada zona |
| `SUAVIZADO` | Velocidad de acercamiento a la posición deseada |

## Pendiente

Cómo se produce la **transición entre zonas** con desplazamientos distintos: si el
`desplazamientoDeZona` cambia de golpe, la cámara hace un corte. El diagrama no lo cubre.
Opciones: corte seco (lenguaje de cine, coherente con "escena enmarcada"), o interpolación
del propio desplazamiento durante N segundos. Ver `decisiones-abiertas.md`.
