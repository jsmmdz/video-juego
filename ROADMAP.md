# Roadmap

El objetivo del prototipo **no es representar el pitch**, sino validar la mecánica central:
¿se siente bien el sistema de sospecha?

## Fase 0 — Desbloquear (ahora)

- [ ] **Decidir motor y lenguaje** — recomendación: Unity + C#.
      Ver [decisiones abiertas #1](docs/tecnico/decisiones-abiertas.md).
- [ ] **Confirmar el esquema de control** — recomendación: WASD.
      Ver [decisiones abiertas #2](docs/tecnico/decisiones-abiertas.md).
- [ ] Crear el proyecto y subir su estructura base.

## Fase 1 — Prototipo con primitivas

Cuatro módulos, en el orden en que se pueden probar de forma aislada.

- [ ] **Blockout plano** de un sector, a escala real (100 × 100 m).
- [ ] **Movimiento** (`01-movimiento.md`) — caminata + gravedad, una sola aplicación por
      fotograma. *Verificable:* la cápsula camina, gira hacia donde va, y cae sin flotar ni
      despegarse en rampas.
- [ ] **Cámara seguidora** (`03-camara.md`) — ángulo fijo por zona, seguimiento suave.
      *Verificable:* no tiembla en movimiento continuo y no rota nunca.
- [ ] **Vista del personaje** (`04-vista-personaje.md`) — billboard + 6 sectores, con cuatro
      rectángulos de color en lugar de arte. *Verificable:* el color cambia al cruzar cada
      sector y no parpadea en los bordes.
- [ ] **Sospecha** (`02-sospecha.md`) — volúmenes de zona vigilada + barra de UI.
      *Verificable:* sube dentro, baja fuera, satura en ambos extremos, y `detectado` queda
      congelado al dispararse.

**Hito:** el bucle completo funciona. Se puede entrar a una zona, ver subir la barra, salir a
tiempo, y ser detectado si no se sale.

## Fase 2 — Ajuste de la mecánica central

- [ ] Playtesting de `TASA_SUBIDA` / `TASA_BAJADA` — es el valor que define el juego.
- [ ] Estado intermedio de **Alerta Local** (el guardia investiga; aún reversible).
- [ ] Verbos de sigilo: agacharse y esprintar, con su efecto sobre la sospecha.
      Ver [decisiones abiertas #3](docs/tecnico/decisiones-abiertas.md).
- [ ] Salto y blockout vertical de Umbria.

## Fase 3 — Sustituir simplificaciones

- [ ] Conos de visión de guardias y alcance de cámaras, reemplazando los volúmenes de zona
      (sin tocar el módulo 2).
- [ ] Rutas de patrulla.
- [ ] Integración de los sprites finales de Nero (4 dibujos × 2 atuendos).
- [ ] Tipografías y botones definitivos de la sección UX-UI.

## Fuera del alcance del prototipo

Creación de personaje (perfiles, especialismos, atributos, ramas de sabotaje), inventario,
diálogos, mercado negro, generación dinámica de patrullas, progresión, guardado, cinemática de
introducción y menú principal. Todo está documentado en
[`docs/diseno/sistemas-de-juego.md`](docs/diseno/sistemas-de-juego.md) para cuando llegue su
momento.
