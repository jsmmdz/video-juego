# Flujo de trabajo con Antigravity

Cómo usar los agentes de [Google Antigravity](https://antigravity.google/) en este proyecto sin
que el resultado sea mucho código plausible y sin probar.

---

## La limitación que define el flujo

Antigravity está diseñado para que sus agentes **validen su propio trabajo abriendo el navegador**:
implementan una función, levantan la app, la usan y adjuntan capturas como prueba. Ese bucle es lo
que hace confiable delegarles tareas.

**Ese bucle no existe aquí.** Unity no es una app de navegador y su editor no corre en modo
headless dentro del agente. Un agente puede escribir C#, comprobar que compila y razonar sobre la
lógica, pero **no puede entrar en Play mode**, ni ver si la cámara tiembla, ni sentir si la curva
de sospecha da margen suficiente.

De ahí sale la regla que ordena todo lo demás:

> **El agente produce código y evidencia de corrección lógica. La persona produce la evidencia de
> comportamiento.** Nada se da por terminado hasta que alguien lo vio correr en el editor.

Esto no reduce la utilidad de los agentes: la concentra. La mayor parte del trabajo pendiente de
este proyecto es lógica derivada de diagramas ya diseñados, que es exactamente donde un agente
rinde. Lo que no puede es cerrar el ciclo solo.

---

## Configuración del repositorio

Ya está en el repo:

| Archivo | Qué hace |
|---|---|
| `AGENTS.md` | Contexto del proyecto para cualquier herramienta de agentes. Estándar multi-herramienta: lo leen Antigravity, Claude Code y otros. |
| `.agents/rules/01-flowcharts-primero.md` | Los diagramas son la fuente de verdad, no el código |
| `.agents/rules/02-unity.md` | Archivos generados, orden de ejecución, entrada |
| `.agents/rules/03-verificacion.md` | Qué significa "verificado" aquí, y qué no |

Las reglas están marcadas `trigger: always_on`: se cargan en cada conversación del workspace.

Antigravity también lee `GEMINI.md`, que tiene prioridad sobre `AGENTS.md` si hay conflicto. No lo
usamos: mantener un solo archivo evita que las dos fuentes diverjan. Si algún día hace falta una
instrucción específica de Antigravity que no aplique a otras herramientas, ese es su sitio.

---

## Qué delegar y qué no

La pregunta útil no es "¿puede el agente hacerlo?", sino **"¿puede alguien saber si lo hizo bien
sin abrir Unity?"**.

### Buen candidato para delegar

- **Lógica derivada de un diagrama existente.** Hay una especificación escrita contra la cual
  contrastar el resultado línea por línea.
- **Matemática comprobable.** Ángulos, sectores, curvas: el agente puede escribir un script y
  ejecutarlo para verificar los valores.
- **Refactors mecánicos** con criterio claro.
- **Documentación derivada del código.**
- **Extensiones aisladas** que no cruzan el contrato entre módulos.

### Mal candidato

- **Cualquier cosa cuyo criterio de éxito sea "se siente bien".** El ajuste de
  `TASA_SUBIDA`/`TASA_BAJADA` es el corazón del juego y solo se decide jugando.
- **Montar o modificar escenas y prefabs.** Son YAML generado; a mano se corrompen.
- **Decidir cosas que están en `decisiones-abiertas.md`.** Esas las decide el equipo, no el agente.
- **Todo lo marcado fuera del alcance del prototipo.** Un agente sin límites implementará el
  inventario entero porque está en el pitch.

---

## Trabajo en paralelo

El Agent Manager permite varios agentes a la vez. La arquitectura del proyecto se presta bien:
los cuatro módulos son independientes y comparten un contrato mínimo, listado en
`00-arquitectura.md`.

**La restricción real no es la lógica, son los archivos compartidos.** Dos agentes tocando el
mismo `.unity` o el mismo `PrototypeSceneBuilder.cs` producen conflictos que en YAML de Unity son
muy caros de resolver.

### Reglas de paralelización

1. **Un agente por módulo**, cada uno en su rama.
2. **`PrototypeSceneBuilder.cs` tiene un solo dueño por tanda.** Si dos tareas necesitan cambiar
   la escena de prueba, van en serie.
3. **Nadie versiona escenas.** La escena de prototipo se genera; por eso los `.unity` no están en
   el repositorio. Esto elimina de raíz la peor fuente de conflictos en Unity.
4. **Cambios al contrato entre módulos van en serie**, nunca en paralelo. Si una tarea modifica lo
   que un módulo expone a otro, se hace sola y primero.

### Tandas que se pueden lanzar en paralelo hoy

| Carril | Tarea | Archivos | Riesgo de choque |
|---|---|---|---|
| A | Verbos de sigilo: salto, sprint, agacharse (`decisiones-abiertas #3`) | `PlayerMovement.cs` | Toca el contrato → **va primero, en solitario** |
| B | Estado intermedio de Alerta Local | `SuspicionSystem.cs` | Ninguno |
| C | Conos de visión sustituyendo `IsInsideSurveillanceZone()` | `SurveillanceZone.cs` + nuevos | Ninguno |
| D | Transición de cámara entre zonas (`decisiones-abiertas #5`) | `FollowCamera.cs`, `CameraZone.cs` | Ninguno |

El carril A modifica lo que el módulo 1 expone al 2 y al 4, así que **no va en paralelo con
nada**. B, C y D sí pueden ir a la vez.

---

## Cómo escribir una tarea para un agente

Antigravity produce un plan de implementación antes de escribir código. Ese plan es el punto de
control barato: **revísalo antes de dejarlo correr**, porque corregir un plan cuesta un minuto y
corregir una implementación entera cuesta una tarde.

Una tarea bien planteada aquí lleva cuatro cosas:

1. **El documento de especificación**, por ruta. No describas la lógica: apunta al documento.
2. **El límite explícito.** Qué archivos puede tocar y cuáles no.
3. **Qué NO hacer.** Los agentes tienden a "mejorar" de más — sobre todo a romper los invariantes
   del orden de ejecución porque parecen redundantes.
4. **Cómo debe cerrar.** Las dos listas de `.agents/rules/03-verificacion.md`.

### Ejemplo

> Implementa el estado de **Alerta Local** en `Assets/Scripts/Suspicion/SuspicionSystem.cs`.
>
> Lee primero `docs/tecnico/02-sospecha.md` (sección "Extensiones previstas") y
> `docs/contexto/pitch-pdf.md` (página 8), que describen el comportamiento: un estado intermedio
> entre "sin sospecha" y "detectado", en el que el guardia se acerca a investigar y la sospecha
> **aún puede revertirse**.
>
> Toca únicamente ese archivo. No modifiques el constructor de escena ni ningún otro módulo.
>
> Respeta los invariantes de `AGENTS.md`: la sospecha sigue subiendo o bajando pero nunca las dos
> en el mismo fotograma, y `Detected` sigue siendo un latch.
>
> Cierra con las dos listas: qué verificaste y qué debo comprobar yo en Play mode.

---

## El ciclo completo

```
1. ELEGIR      Una tarea del ROADMAP que se pueda juzgar sin abrir Unity.
                   │
2. PLANTEAR    Tarea con especificación, límites y criterio de cierre.
                   │
3. REVISAR     ← punto de control barato
   EL PLAN     ¿Leyó el documento correcto? ¿Se sale del alcance?
                   │  Si va mal, se corrige aquí. Cuesta un minuto.
                   ▼
4. DEJAR       El agente implementa y compila.
   CORRER      Feedback en los artefactos sin detener la ejecución.
                   │
5. LEER        El diff contra el flowchart, no contra la intuición.
   EL DIFF     Los invariantes son lo primero que hay que mirar.
                   │
6. PROBAR      ← el paso que el agente NO puede dar
   EN UNITY    Play mode, contra la tabla de setup-unity.md.
                   │
7. CERRAR      Merge + actualizar ROADMAP. Si el diagrama cambió,
                actualizar también el board de Figma.
```

El paso 6 es el cuello de botella real y **no se puede eliminar**. Conviene tenerlo en cuenta al
decidir cuántos agentes lanzar a la vez: cuatro agentes produciendo en paralelo generan cuatro
sesiones de verificación manual, y esa cola es de una sola persona.

Regla práctica: **no lances más trabajo en paralelo del que puedas verificar en la misma sesión.**

---

## Mantener los diagramas vivos

El riesgo mayor de acelerar con agentes es que el código avance y los diagramas se queden atrás.
Si eso pasa, `docs/tecnico/` deja de ser la fuente de verdad y se convierte en documentación
obsoleta — y el proyecto pierde justo lo que hoy tiene de bueno.

Por eso la regla `01-flowcharts-primero.md` obliga al agente a **señalar** cuando un cambio
requiere modificar el diagrama, en vez de cambiar el código en silencio.

Cuando eso aparezca en un resumen: actualiza el board de Figma **en la misma tanda**, no después.
