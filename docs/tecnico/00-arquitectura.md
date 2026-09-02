# Arquitectura y orden de ejecución

## Principio rector

Los cuatro módulos son **independientes entre sí** pero tienen un **orden estricto dentro del
fotograma**. Ese orden no es un detalle de implementación: es lo que hace que la imagen sea
correcta.

```
FOTOGRAMA N
│
├─ 1. MOVIMIENTO ......... lee entrada, calcula avance + caída,
│                          mueve al jugador UNA sola vez
│
├─ 2. SOSPECHA ........... evalúa zona vigilada, sube/baja el valor,
│                          actualiza la barra de UI, dispara detección
│
├─ 3. CÁMARA ............. (fin del fotograma: el jugador YA se movió)
│                          calcula posición deseada y se acerca suavemente
│
└─ 4. VISTA DEL PERSONAJE  (fin del fotograma: la cámara YA está ubicada)
                           gira el plano hacia la cámara y elige el dibujo
```

### Por qué ese orden

- **Cámara después de movimiento.** Si la cámara se actualizara antes, seguiría la posición del
  fotograma anterior y el personaje se vería "adelantado" respecto al encuadre, produciendo
  temblor. El flowchart lo marca explícitamente: *«fin del fotograma · ya se movió el
  jugador»*.
- **Vista después de cámara.** El sector de 60° que decide qué dibujo mostrar se mide
  **respecto a la cámara**. Si se calculara antes de mover la cámara, usaría un ángulo obsoleto
  y el sprite parpadearía entre dibujos en los giros. El flowchart lo marca: *«fin del fotograma ·
  cámara ubicada»*.
- **Sospecha en cualquier punto tras el movimiento**, porque depende de la posición nueva del
  jugador, no de la cámara.

## Estado compartido

Sólo hay un dato que cruza módulos:

| Dato | Lo produce | Lo consume |
|---|---|---|
| Posición del jugador | Movimiento | Sospecha, Cámara |
| Dirección de avance | Movimiento | Vista del personaje |
| Posición/orientación de la cámara | Cámara | Vista del personaje |
| `detectado` (bool) | Sospecha | Resto del juego (alarma, reinicio) |
| Valor de sospecha `0..máximo` | Sospecha | UI (barra) |

Todo lo demás es interno a cada módulo. Mantener esta lista corta es lo que permite trabajar
los módulos por separado.

## Estado del prototipo

Se está armando con **primitivas** (cápsulas, cubos, planos) mientras llegan los assets de
ilustración. Consecuencia práctica: los cuatro módulos deben funcionar y ser verificables sin
ningún arte final. El módulo de vista del personaje se prueba con cuatro rectángulos de color
distinto en lugar de los cuatro dibujos.

## Motor

⚠️ **Pendiente de confirmar.** Ver `decisiones-abiertas.md`.

Los flowcharts están escritos de forma agnóstica, pero describen primitivas que existen tal
cual en Unity (movimiento acumulado aplicado en una única llamada por fotograma, plano
orientado a cámara, actualización de cámara en el paso posterior al de física). La
recomendación es **Unity + C#**; el resto de la documentación técnica usa términos neutros
para no cerrar la decisión.
