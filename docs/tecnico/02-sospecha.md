# Módulo 2 — Sistema de sospecha

**Flowchart:** board de Figma, nodo `50:202`.
**Es la mecánica central del juego.**

## Enunciado

> Nero se infiltra en Aurea y hay zonas vigiladas. Si se queda en ellas la sospecha sube, si sale
> baja, y si llega al máximo lo detectan.

## Diagrama

```
Inicio del juego
       │
       ▼
┌──────────────┐◄────────────────────────────────────┐
│CADA FOTOGRAMA│                                     │
└──────┬───────┘                                     │
       ▼                                             │
   ◇ ¿Ya detectado? ◇──── sí ──────────────────────►─┤
       │ no                                          │
       ▼                                             │
   ◇ ¿Dentro de zona vigilada? ◇                     │
    sí │                    │ no                     │
       ▼                    ▼                        │
 Sospecha aumenta      Sospecha disminuye            │
 (tope: el máximo)     (piso: cero)                  │
       └────────┬───────────┘                        │
                ▼                                    │
        Actualizar barra de UI                       │
                ▼                                    │
      ◇ ¿Sospecha ≥ máximo? ◇──── no ──────────────►─┤
                │ sí                                 │
                ▼                                    │
      ╔═══════════════════════╗                      │
      ║ Detectado = verdadero ║──────────────────────┘
      ╚═══════════════════════╝
        avisa al resto del juego
```

Leyenda del diagrama original: blanco = proceso/decisión · dorado = estado de detección ·
azul punteado = retorno de ciclo.

## Nota del autor del diagrama

> El ciclo se repite cada fotograma: si el jugador ya fue detectado, el estado queda congelado;
> si no, la zona de vigilancia sube o baja la sospecha — nunca las dos cosas a la vez — la barra
> se actualiza, y al llegar al máximo se dispara la detección.

## Pseudocódigo

```
estado:
    sospecha  = 0          # 0 .. SOSPECHA_MAXIMA
    detectado = falso

cadaFotograma():
    si detectado:
        return                      # estado congelado, no se recalcula nada

    si dentroDeZonaVigilada():
        sospecha = min(sospecha + TASA_SUBIDA * dt, SOSPECHA_MAXIMA)
    si no:
        sospecha = max(sospecha - TASA_BAJADA * dt, 0)

    actualizarBarraUI(sospecha / SOSPECHA_MAXIMA)

    si sospecha >= SOSPECHA_MAXIMA:
        detectado = verdadero
        avisarAlRestoDelJuego()     # alarma, bloqueo de accesos, reinicio en checkpoint
```

## Detalles a respetar

- **Un solo camino por fotograma.** Sube *o* baja, nunca ambas. Parece obvio en el diagrama,
  pero se rompe fácil si se implementa como dos comprobaciones independientes (p. ej. un
  `OnTriggerStay` que suma y un decaimiento global que resta: en ese caso, dentro de la zona
  se aplican los dos y la tasa efectiva de subida queda mal).
- **Saturación en ambos extremos.** Tope en el máximo, piso en cero. Sin el piso, la sospecha
  se vuelve negativa fuera de las zonas y el jugador acumula un colchón invisible que retrasa
  la siguiente detección.
- **`detectado` es un latch.** Una vez verdadero, no vuelve solo a falso: el diagrama corta el
  ciclo en el primer rombo. Quien lo reinicia es el resto del juego (checkpoint, fin de la alerta),
  no este módulo.
- **La UI se actualiza siempre**, suba o baje, y **antes** de comprobar el máximo. Así la barra
  llega visualmente al tope en el mismo fotograma en que se dispara la detección, en lugar de
  quedarse un fotograma corta.

## Parámetros expuestos

| Parámetro | Descripción |
|---|---|
| `SOSPECHA_MAXIMA` | Valor que dispara la detección |
| `TASA_SUBIDA` | Sospecha por segundo dentro de zona vigilada |
| `TASA_BAJADA` | Sospecha por segundo fuera de zona vigilada |

La relación `TASA_SUBIDA / TASA_BAJADA` es la que define el margen de maniobra del jugador
y es el primer valor a ajustar en playtesting.

## Zonas vigiladas en el prototipo

Volúmenes de disparo (triggers) colocados a mano sobre el blockout. El jugador está "dentro"
mientras solapa con al menos uno.

**Simplificación consciente del prototipo:** el pitch describe conos de visión de guardias,
cámaras con alcance, rutas de patrulla y escáneres infrarrojos (PDF pág. 8). El flowchart
reduce todo eso a *«¿dentro de zona vigilada?»* — una sola pregunta booleana. Es la decisión
correcta para el prototipo: permite validar la curva de sospecha sin depender de IA de
patrullas. Cuando se implementen los conos de visión, sustituirán la implementación de
`dentroDeZonaVigilada()` **sin cambiar el resto del módulo**.

## Extensiones previstas (aún no en el flowchart)

- **Modificadores por acción**: esprintar sube más rápido, agacharse sube más lento
  (PDF pág. 7-8).
- **Modificadores por especialismo**: *Agente de Sombras* reduce la detección, *Persuasivo
  Social* permite bajar la sospecha vía diálogo (PDF pág. 5).
- **Alerta Local** como estado intermedio entre "sin sospecha" y "detectado" (PDF pág. 8): el
  guardia se acerca a investigar y la sospecha puede aún revertirse.
- **Pase Biométrico Falsificado**: cambia la consecuencia de ser detectado, no la acumulación.
