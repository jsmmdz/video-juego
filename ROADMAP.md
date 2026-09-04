# Roadmap

El objetivo del prototipo **no es representar el pitch**, sino validar la mecánica central:
¿se siente bien el sistema de sospecha?

## Fase 0 — Desbloquear (ahora)

- [x] **Decidir motor y lenguaje** → **Unity + C#**.
- [x] Subir la estructura de código (`Assets/Scripts/`).
- [ ] Generar `ProjectSettings/` y `Packages/` abriendo el repo en Unity, y commitearlos.
- [ ] **Corregir la página 6 del pitch**, que describe point-and-click en lugar de WASD.
      Ver [decisiones abiertas #2](docs/tecnico/decisiones-abiertas.md).

## Fase 1 — Prototipo con primitivas

Cuatro módulos, en el orden en que se pueden probar de forma aislada.

- [x] **Blockout plano** de un sector, a escala real (100 × 100 m) — generado por
      `PrototypeSceneBuilder`.
- [x] **Movimiento** (`01-movimiento.md`) — caminata + gravedad, una sola aplicación por
      fotograma. *Verificable:* la cápsula camina, gira hacia donde va, y cae sin flotar ni
      despegarse en rampas.
- [x] **Cámara seguidora** (`03-camara.md`) — ángulo fijo por zona, seguimiento suave.
      *Verificable:* no tiembla en movimiento continuo y no rota nunca.
- [x] **Vista del personaje** (`04-vista-personaje.md`) — billboard + 6 sectores, con cuatro
      rectángulos de color en lugar de arte. *Verificable:* el color cambia al cruzar cada
      sector y no parpadea en los bordes.
- [x] **Sospecha** (`02-sospecha.md`) — volúmenes de zona vigilada + barra de UI.
      *Verificable:* sube dentro, baja fuera, satura en ambos extremos, y `detectado` queda
      congelado al dispararse.

- [ ] **Abrir en Unity y verificar los cuatro módulos** contra la tabla de
      [setup-unity.md](docs/tecnico/setup-unity.md). ← siguiente paso

**Hito:** el bucle completo funciona. Se puede entrar a una zona, ver subir la barra, salir a
tiempo, y ser detectado si no se sale.

> A partir de aquí las tareas se pueden repartir entre agentes. Los carriles que pueden ir en
> paralelo y el que debe ir en solitario están en
> [flujo-antigravity.md](docs/tecnico/flujo-antigravity.md).

## Fase 2 — Ajuste de la mecánica central

- [ ] Playtesting de `TASA_SUBIDA` / `TASA_BAJADA` — es el valor que define el juego.
- [ ] Estado intermedio de **Alerta Local** (el guardia investiga; aún reversible).
- [ ] Verbos de sigilo: agacharse y esprintar, con su efecto sobre la sospecha.
      Ver [decisiones abiertas #3](docs/tecnico/decisiones-abiertas.md).
- [ ] Salto y blockout vertical de Umbria.

## Interfaz

Va en paralelo a las fases de jugabilidad: no depende de ellas.

- [x] **Menú principal** — botones con los cuatro estados del kit, navegación por teclado y ratón
      con foco compartido, «Continuar» deshabilitado sin partida guardada.
- [ ] Tipografías reales, en cuanto diseño entregue las familias y licencias
      ([decisiones abiertas #8](docs/tecnico/decisiones-abiertas.md)).
- [ ] Fondo del menú con la ilustración de la brecha entre mundos.
- [ ] **Barra de sospecha con los cuatro estados del kit**: normal, sospecha, alerta y alarma.
      Encaja con el estado intermedio de Alerta Local de la fase 2.
- [ ] Overlay «ALERTA — DETECCIÓN LOCAL» en pantalla.
- [ ] Botones de interacción del mundo (`[E] INTERACTUAR`, `ABRIR TERMINAL`, `RECOGER`, `HABLAR`).
- [ ] Pantalla de Ajustes de Sistema: gráficos, audio, controles, accesibilidad.

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
