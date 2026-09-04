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

## 1b. ✅ Perspectiva de cámara — RESUELTO por el pitch v2

**Decisión: vista cenital oblicua (aérea en ¾), de ángulo fijo.**

El pitch v1 planteaba dos perspectivas —tercera persona y «vista central»— sin decir cuándo se
usaba cada una. **El pitch v2 lo sustituye por una sola:** «una perspectiva de vista cenital
oblicua o vista aérea en ¾».

Es exactamente la que ya está implementada en `03-camara.md`: ángulo fijo por zona, la cámara
sigue la posición del jugador y **nunca rota**. Y valida retroactivamente el módulo 4: un
personaje plano orientado a cámara solo se sostiene con un ángulo estable — con la tercera
persona libre de la v1, el billboard no habría funcionado.

**Efecto sobre el código: ninguno.** Confirma la arquitectura existente.

---

## 2. 🟡 Esquema de control: point-and-click vs. WASD

**Sigue sin resolverse en el pitch v2**, que mantiene ambas páginas sin tocar.

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
fuentes y lo único que está diagramado.

**Acción pendiente:** corregir la página 6 en `The_Silent_Divide_v2.docx` — que es el documento
editable, así que el arreglo es directo — y volver a exportar el PDF.

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

**Bloquea ahora:** el menú principal ya está implementado y usa la **fuente por defecto de
TextMeshPro** como marcador de posición. El kit del board usa un espaciado entre letras muy
marcado, que sí está reproducido (`characterSpacing`), pero la familia tipográfica no.

**Acción:** pedir a diseño los nombres de las dos tipografías y sus licencias, generar los
*font assets* de TMP y asignarlos en `MainMenuSceneBuilder.NewLabel`. Es un cambio de una línea
por familia.

Los colores del kit sí están volcados, en `Assets/Scripts/UI/UITheme.cs`.

---

## 9. 🟢 Alcance del prototipo

El pitch describe un juego muy grande (creación de personaje con perfiles y especialismos,
cuatro atributos, ramas de sabotaje, inventario, diálogos, mercado negro, generación
dinámica, progresión). Los flowcharts describen un prototipo mínimo de cuatro módulos.

**Recomendación:** mantener esa separación explícita. El objetivo del prototipo es **validar la
mecánica central de sospecha**, no representar el pitch. Ver el plan en `../../ROADMAP.md`.

---

## 10. 🟡 «Tercera persona» en los documentos de narrativa

Los dos documentos de narrativa (`Narrativa_larga.pdf` y `Narrativa_estudio.pdf`, sección de
jugabilidad, puntos 1 y 2) dicen:

> «Videojuego de sigilo y aventura en **tercera persona**. […] El juego será en tercera persona,
> para que el jugador pueda ver al personaje y observar mejor el entorno.»

**El pitch v2 dice vista cenital oblicua en ¾.** Son cosas distintas.

**Recomendación: manda el pitch v2**, por tres razones: es el documento más reciente, es el que
usa el equipo como Game Design, y es lo que ya está construido y validado en los flowcharts.

Además, la intención declarada en la narrativa —«ver al personaje y observar mejor el entorno»—
la cumple igual de bien una vista cenital oblicua; de hecho, mejor para planificar rutas de
sigilo.

**Acción:** actualizar los puntos 1 y 2 de la sección de jugabilidad en ambos documentos de
narrativa.

---

## 11. 🟡 Dos modelos de progresión incompatibles

| Fuente | Modelo |
|---|---|
| Pitch v2 (pág. 5–6) | **Atributos y puntos**: Tecnología, Mimetismo, Agilidad y Cultura Aurea, con puntos de libre asignación, más perfiles, especialismos y Ramas de Sabotaje |
| Narrativa (punto 8) | **Conocimiento del entorno**: «el jugador progresará principalmente mediante el conocimiento del entorno […] observar rutinas, descubrir rutas y aprender cómo funcionan los sistemas de seguridad» |

No son variantes del mismo sistema: uno progresa en la **ficha de personaje**, el otro en la
**cabeza del jugador**.

**Recomendación:** no son excluyentes si se ordenan. La progresión por conocimiento es la
principal —es lo que hace bueno a un juego de sigilo— y los atributos son el modificador que
la acompaña. Pero conviene decidirlo explícitamente, porque **cambia qué se implementa primero**:
el modelo de conocimiento no necesita ficha de personaje ni pantalla de creación, y por tanto
permite un vertical slice mucho antes.

**Fuera del alcance del prototipo** en cualquier caso. Decidir antes de la fase 3.

---

## 12. 🟢 Error de escritura en la narrativa larga

En la descripción de Elías Varen:

> «ha dedicado años a impedir que cualquiera de ellos consiga llegar al mundo de **abajo**»

Por contexto debe ser **arriba**: Elías impide que la resistencia de Umbría llegue a Aurea.

**Acción:** corregir en `Narrativa_larga.pdf`.

---

## 13. 🟢 El ratón «orienta la cámara»

El pitch v2 mantiene en la página 7:

> «Ratón (Mouse): **Orientación de la cámara**, apuntado de dispositivos y selección de elementos
> interactivos.»

Pero la misma v2 define una cámara **cenital oblicua de ángulo fijo**, y el flowchart del módulo 3
establece que la cámara **no rota nunca**. Con esa cámara, el ratón no puede orientarla.

**Recomendación:** redefinir el papel del ratón como **apuntado de dispositivos y selección de
elementos interactivos**, quitando la orientación de cámara. Es lo coherente con la perspectiva
elegida y con lo implementado, y encaja con los botones de interacción del kit de UX-UI
(`[E] INTERACTUAR`, `ABRIR TERMINAL`, `RECOGER`, `HABLAR`).

**Acción:** corregir la línea del ratón en `The_Silent_Divide_v2.docx`, páginas 7 y 8.

---

## 14. 🟡 Estructura por niveles

La narrativa define tres niveles iniciales —**1: Abajo**, **2: La Barrera**, **3: Arriba**— y el
pitch v2 pasa de hablar de «partida» a hablar de «nivel». El prototipo actual es **un solo
escenario plano**.

**Recomendación:** mantener el prototipo como está para validar la mecánica de sospecha, y montar
la estructura por niveles al empezar la fase 3. El nivel 1 (Umbria, sin vigilancia) es el más
barato de construir y el que enseña los controles, así que es el candidato natural al primer
nivel jugable de verdad.
