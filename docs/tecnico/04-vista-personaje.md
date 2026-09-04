# Módulo 4 — Vista del personaje (billboard direccional)

**Flowcharts:** board de Figma, nodos `50:222` (lógica) y `51:254` (rosa de sectores).

> **Depende de la perspectiva fija.** El pitch v2 define «vista cenital oblicua o vista aérea en
> ¾», y ese ángulo estable es lo que hace viable este módulo: con la tercera persona libre que
> planteaba la v1, un personaje plano orientado a cámara no se sostendría.

## Qué resuelve

El personaje **no es un modelo 3D**: es un **plano con un dibujo**. Este módulo hace dos cosas
para que ese plano se lea como un personaje dentro de un mundo tridimensional:

1. Lo gira hacia la cámara, para que nunca se vea de canto.
2. Elige cuál de los cuatro dibujos mostrar, según hacia dónde camina respecto a la cámara.

## Diagrama de lógica (`50:222`)

```
Inicio del juego
       │
       ▼
┌──────────────────────────────────────┐◄──────┐
│ FIN DEL FOTOGRAMA · CÁMARA UBICADA   │       │
└──────────────────┬───────────────────┘       │
                   ▼                           │
   Girar el plano a la cámara                  │
   (para no verlo de canto)                    │
                   ▼                           │
   Medir hacia dónde camina                    │
   (el ángulo respecto a la cámara)            │
                   ▼                           │
   Ubicarlo en un sector                       │
   (6 sectores de 60°)                         │
                   ▼                           │
   ◇ ¿Cambió de sector desde el anterior? ◇    │
    no ──────────────────────────────────────►─┤
       │ sí                                    │
       ▼                                       │
   Cambiar el dibujo mostrado                  │
   (espaldas · perfil · ¾ · frente) ───────────┘
```

### Nota del autor

> El personaje es un plano: girarlo hacia la cámara evita que se vea de canto, y el sector de
> 60° decide cuál de los cuatro dibujos se muestra. El chequeo de cambio de sector evita
> reemplazar la imagen en fotogramas donde nada cambió.

## Rosa de sectores (`51:254`)

Vista **desde arriba**, con la cámara abajo. Seis sectores de 60°, medidos como el ángulo entre
*hacia dónde camina el personaje* y *hacia dónde mira la cámara*:

```
                TRASERA
              (se aleja)
        LATERAL  ╱  ╲  LATERAL
       (espejada)     (de perfil)
              ●─────────► hacia dónde camina
         ¾    ╱   ╲    ¾
    (espejada)     (de sesgo)
                FRONTAL
          (viene hacia la cámara)

                   ▲
                   ┆
               [ CÁMARA ]
```

### Nota del autor

> Vista desde arriba, con la cámara abajo. El ángulo entre hacia dónde camina el personaje y
> hacia dónde mira la cámara cae en uno de seis sectores, y cada sector tiene su dibujo.
> Frontal y trasera se usan tal cual; lateral y ¾ se espejan para servir a ambos lados — por eso
> cuatro dibujos alcanzan para seis direcciones.

## Tabla de sectores

| Sector | Rango (aprox.) | Dibujo | Espejado |
|---|---|---|---|
| Frontal | −30° … +30° | `frontal` | no |
| ¾ de sesgo | +30° … +90° | `tres_cuartos` | no |
| Lateral de perfil | +90° … +150° | `lateral` | no |
| Trasera | +150° … −150° | `trasera` | no |
| Lateral espejada | −150° … −90° | `lateral` | **sí** |
| ¾ espejada | −90° … −30° | `tres_cuartos` | **sí** |

**4 dibujos → 6 direcciones.** Esta es la economía central del sistema: reduce a la mitad el
arte necesario por personaje y por atuendo.

## Pseudocódigo

```
estado:
    sectorAnterior = -1

finDelFotograma():              # después de que la cámara ya se ubicó
    # 1. billboard
    plano.orientarHacia(camara)

    # 2. ángulo entre avance del personaje y mirada de la cámara (en el plano horizontal)
    angulo = anguloConSigno(direccionDeAvance, camara.adelante, ejeVertical)

    # 3. sector de 60°
    sector = floor((angulo + 180 + 30) / 60) mod 6

    # 4. solo si cambió
    si sector != sectorAnterior:
        (dibujo, espejado) = TABLA_SECTORES[sector]
        plano.dibujo = dibujo
        plano.espejadoEnX = espejado
        sectorAnterior = sector
```

## Detalles a respetar

- **Se ejecuta después de la cámara.** El ángulo se mide *respecto a la cámara*; con una
  cámara del fotograma anterior, el sprite parpadea entre dos dibujos en los giros.
- **El chequeo de cambio de sector no es una optimización cosmética.** Reasignar la textura
  cada fotograma genera trabajo innecesario y, con animaciones por dibujo, reiniciaría la
  animación en cada fotograma.
- **El espejado es en X del plano**, no una rotación de 180°: rotar giraría también el billboard
  y lo pondría de espaldas a la cámara.
- **Sin entrada, la dirección de avance se conserva** (garantizado por el módulo 1): el
  personaje quieto mantiene el dibujo con el que llegó, en lugar de saltar a `frontal`.
- **Riesgo de oscilación en los bordes de sector.** Un personaje caminando justo sobre una
  frontera de 60° alternará entre dos dibujos. Si aparece en pruebas, la solución es una
  histéresis pequeña (exigir unos grados de más para salir del sector actual), no reducir el
  número de sectores.

## Sprites necesarios

Por personaje y **por atuendo** (Nero tiene dos: Umbria y Aurea):

| Dibujo | Uso |
|---|---|
| `frontal` | Viene hacia la cámara |
| `tres_cuartos` | ¾ de sesgo (+ su espejo) |
| `lateral` | De perfil (+ su espejo) |
| `trasera` | Se aleja |

Las hojas de modelo de Nero en el board ya incluyen exactamente esos cuatro puntos de vista
(frontal, ¾, lateral, trasera) para ambos atuendos — el arte y el sistema están alineados.

**En el prototipo** estos cuatro dibujos se sustituyen por cuatro rectángulos de color distinto,
lo que permite verificar el módulo completo sin arte final.
