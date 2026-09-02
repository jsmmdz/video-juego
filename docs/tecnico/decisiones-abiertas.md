# Decisiones abiertas

Puntos donde las fuentes se contradicen o falta información para implementar. Cada uno indica
**por qué bloquea** y una **recomendación**. Ninguno impide empezar el prototipo, pero todos
deben resolverse antes de dar por cerrado el módulo correspondiente.

---

## 1. ✅ Motor y lenguaje — RESUELTO

**Decisión: Unity + C#.** Confirmado por el equipo el 2026-09-02.

Encaja con lo diagramado sin traducción: los flowcharts describen primitivas que existen tal
cual en Unity (movimiento acumulado aplicado en una única llamada por fotograma, plano
orientado a cámara, actualización de cámara en el paso posterior al de física).

Puesta en marcha: `setup-unity.md`.

---

## 2. 🟡 Esquema de control: point-and-click vs. WASD

**Contradicción dentro del propio PDF de pitch:**

| Fuente | Esquema |
|---|---|
| PDF pág. 6 | **Point-and-click**: clic izquierdo en el suelo → el personaje camina hacia allí por la ruta más corta; clic izquierdo en objeto → se acerca e interactúa; clic derecho → activa dispositivo; teclas de dirección → mueven la vista del mapa |
| PDF pág. 7 | **WASD directo**: `WASD` movimiento, `Espacio` saltar, `Shift` esprintar, `Ctrl`/`C` agacharse, ratón para cámara y apuntado |
| Flowchart de movimiento (`50:170`) | **Teclado directo**: «leer entrada del teclado — horizontal + profundidad» |

**Los dos esquemas implican juegos distintos.** Point-and-click requiere navegación por malla
(pathfinding) y no necesita gravedad ni salto; control directo requiere lo que el flowchart ya
diagrama. También cambia la cámara: con point-and-click, «las teclas de dirección desplazan la
vista independientemente del personaje», lo que contradice la cámara seguidora del módulo 3.

**Recomendación: control directo (WASD).** Es lo que dicen dos de las tres fuentes, es lo ya
diagramado y programado, y es coherente con la cámara seguidora y con las mecánicas de
sigilo en tiempo real (esprintar hace ruido, agacharse reduce visibilidad — verbos que no
existen en point-and-click).

**Estado:** el prototipo está implementado con **WASD**, que es lo que dicen dos de las tres
fuentes y lo único que está diagramado. Queda pendiente **corregir la página 6 del pitch**
para que el documento no se contradiga a sí mismo.

---

## 3. 🟡 Verbos de movimiento faltantes en el flowchart

El módulo 1 solo resuelve **caminata + gravedad**. El pitch (pág. 7-8) define además:

| Verbo | Tecla | Efecto en sigilo |
|---|---|---|
| Saltar | `Espacio` | Superar obstáculos bajos |
| Esprintar | `Shift` izq. | Más rápido, **genera ruido que atrae guardias** |
| Agacharse | `Ctrl` izq. / `C` | **Reduce visibilidad y ruido** |

Los dos últimos no son solo movimiento: **alimentan al módulo de sospecha**. Añadirlos cambia
el contrato entre módulos (el módulo 2 pasaría a leer un estado de sigilo del jugador).

**Recomendación:** implementar primero los cuatro módulos tal como están diagramados,
verificar el bucle completo, y recién entonces extender el diagrama del módulo 1 con estos
verbos y el del módulo 2 con los modificadores correspondientes.

---

## 4. 🟡 Zonas vigiladas: volúmenes vs. conos de visión

El flowchart de sospecha pregunta `¿dentro de zona vigilada?` — un booleano. El pitch describe
conos de visión de guardias, alcance de cámaras, rutas de patrulla, escáneres infrarrojos y
generación dinámica de patrullas.

**No es una contradicción, es una simplificación deliberada del prototipo** y está bien
planteada: el módulo 2 queda desacoplado de *cómo* se determina la vigilancia. Cuando se
implementen los conos, sustituyen la implementación de `dentroDeZonaVigilada()` sin tocar el
resto.

**Acción:** ninguna ahora. Mantener esa función como único punto de contacto para que el
cambio sea local.

---

## 5. 🟡 Transición de cámara entre zonas

El módulo 3 fija el ángulo por zona y nunca rota. No está definido qué ocurre al cruzar de una
zona a otra con desplazamientos distintos.

**Opciones:** corte seco (coherente con la idea de "escena enmarcada" del referente) o
interpolación del desplazamiento durante N segundos.

**Recomendación:** corte seco por defecto, con la duración de transición como parámetro por
zona (0 = corte). Cubre ambos casos sin decidir ahora.

---

## 6. 🟡 Navegación multinivel en Umbria

`docs/diseno/escenarios.md` define para Umbria pasarelas entre edificios (2 m) y escaleras
exteriores (1–1,5 m). Eso implica varios niveles verticales de navegación.

**Recomendación:** el primer blockout de prototipo es **plano**, para validar los cuatro
módulos. La verticalidad de Umbria entra en la segunda iteración, cuando el módulo 1 ya tenga
salto.

---

## 7. 🟢 Nomenclatura: Aurea/Umbria vs. Arriba/Abajo

Los stickies de referencias narrativas en el board (`43:430`, `43:442`) usan «Arriba» y «Abajo»,
nombres provisionales anteriores a «Aurea» y «Umbria». El documento de monetización usa
«Aurea/Umbría» con tilde; el resto del board escribe «Umbria» sin tilde.

**Recomendación:** nombres canónicos **Aurea** y **Umbria** (sin tilde, para evitar problemas
con identificadores en código y nombres de archivo). Actualizar los stickies del board.

---

## 8. 🟢 Tipografías y tokens de UI

La sección *Design UX-UI* del board define dos familias tipográficas (una para títulos y
botones, otra para interfaz/diálogos/descripciones) y un set de botones, pero los nombres
concretos no están volcados en texto.

**Acción:** pedir a diseño los nombres de las tipografías y las licencias, y volcar los tokens de
UI a `docs/diseno/`.

---

## 9. 🟢 Alcance del prototipo

El pitch describe un juego muy grande (creación de personaje con perfiles y especialismos,
cuatro atributos, ramas de sabotaje, inventario, diálogos, mercado negro, generación
dinámica, progresión). Los flowcharts describen un prototipo mínimo de cuatro módulos.

**Recomendación:** mantener esa separación explícita. El objetivo del prototipo es **validar la
mecánica central de sospecha**, no representar el pitch. Ver el plan en `../../ROADMAP.md`.
