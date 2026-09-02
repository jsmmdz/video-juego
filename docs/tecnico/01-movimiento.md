# Módulo 1 — Movimiento del jugador

**Flowchart:** board de Figma, nodo `50:170`.

## Diagrama

```
Inicio del juego
       │
       ▼
┌──────────────┐◄──────────────────────────────┐
│CADA FOTOGRAMA│                               │
└──────┬───────┘                               │
       ▼                                       │
Leer entrada del teclado                       │
  (horizontal + profundidad)                   │
       ▼                                       │
Construir vector dirección                     │
       ▼                                       │
   ◇ ¿Hay entrada? (vector ≠ 0) ◇              │
    sí │                    │ no               │
       ▼                    │                  │
 Normalizar vector          │  sin entrada:    │
       ▼                    │  dirección = 0   │
 Rotar personaje            │  el personaje    │
       │                    │  conserva su     │
       └────────┬───────────┘  orientación     │
                ▼                              │
      ◇ ¿Tocando el piso? ◇                    │
       sí │            │ no                    │
          ▼            ▼                       │
 Reiniciar la caída   Acumular gravedad        │
 (valor mínimo        (la caída se acelera)    │
  hacia abajo)                                 │
          └──────┬─────┘                       │
                 ▼                             │
      Sumar avance + caída                     │
      (en una sola instrucción)                │
                 ▼                             │
      Mover al personaje                       │
      (una única vez por fotograma) ───────────┘
```

## Nota del autor del diagrama

> Las dos decisiones no son alternativas entre sí: la primera solo define si hay dirección
> horizontal, la segunda solo define cómo se calcula la caída. Sus resultados se suman en una
> sola instrucción de movimiento que se aplica una única vez — por eso el personaje sigue
> cayendo aunque esté quieto.

Esta nota es la parte más importante del módulo. El error clásico al implementarlo es tratar
los dos rombos como un `if/else` encadenado y terminar con dos llamadas de movimiento por
fotograma (o con el personaje flotando cuando no hay entrada).

## Pseudocódigo

```
cadaFotograma():
    # --- 1. dirección horizontal ---
    h = entradaHorizontal()      # A / D
    p = entradaProfundidad()     # W / S
    direccion = vector(h, 0, p)

    si direccion != 0:
        direccion = normalizar(direccion)
        rotarPersonajeHacia(direccion)
    # si no: direccion queda en 0 y NO se toca la orientación

    # --- 2. caída (independiente de lo anterior) ---
    si tocandoElPiso():
        velocidadVertical = CAIDA_MINIMA      # valor pequeño hacia abajo, no 0
    si no:
        velocidadVertical += GRAVEDAD * dt    # se acelera

    # --- 3. una sola aplicación ---
    desplazamiento = direccion * VELOCIDAD * dt
    desplazamiento.y = velocidadVertical * dt
    mover(desplazamiento)        # ← exactamente una vez por fotograma
```

## Detalles a respetar

- **Normalizar solo si hay entrada.** Normalizar el vector cero da un resultado indefinido o
  cero según la implementación; el diagrama evita el caso poniendo la normalización dentro de
  la rama `sí`.
- **`CAIDA_MINIMA` no es cero.** Reiniciar la caída a un valor pequeño *hacia abajo* (y no a 0)
  es lo que mantiene al personaje pegado al suelo y hace que la comprobación de "tocando el
  piso" siga dando verdadero en fotogramas sucesivos. Con 0, el personaje despega en rampas
  y bajadas.
- **La orientación se conserva sin entrada.** Al soltar las teclas el personaje no vuelve a una
  rotación por defecto: se queda mirando a donde iba. Esto importa para el módulo 4, que elige
  el dibujo a partir de la dirección de avance.
- **Una única llamada de movimiento.** Sumar avance y caída antes de mover, no mover dos
  veces.

## Parámetros expuestos

| Parámetro | Descripción | Valor inicial sugerido |
|---|---|---|
| `VELOCIDAD` | Velocidad de caminata | por definir en prototipo |
| `GRAVEDAD` | Aceleración de caída | por definir en prototipo |
| `CAIDA_MINIMA` | Empuje hacia abajo al estar en el piso | pequeño negativo |
| `VELOCIDAD_ROTACION` | Suavizado del giro del personaje | por definir |

## Pendiente

El pitch (PDF pág. 7) añade **esprintar** (`Shift`, genera ruido), **agacharse** (`Ctrl`/`C`,
reduce visibilidad y ruido) y **saltar** (`Espacio`). El flowchart actual **no los cubre**: solo
resuelve caminata + gravedad. Son extensiones naturales de este módulo, pero deben añadirse
al diagrama antes de implementarse, porque esprintar y agacharse tienen que alimentar al
módulo de sospecha (ruido / visibilidad) y eso cambia el contrato entre módulos.

Ver `decisiones-abiertas.md`.
