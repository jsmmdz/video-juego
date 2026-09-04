# The Silent Divide — contexto para agentes

Juego de aventura, infiltración y sigilo social en **Unity + C#**. El jugador es Nero, que se
infiltra desde **Umbria** (mundo inferior) en **Aurea** (mundo superior) para destruir la Barrera
que los separa.

## Antes de tocar nada

Este proyecto **ya tiene su lógica diseñada**. No está en el código: está en cuatro flowcharts
del board de Figma, transcritos y anotados en `docs/tecnico/`. Son la fuente de verdad.

**Antes de modificar un módulo, lee su documento.** Si el código y el documento discrepan, el
documento gana y el código es el bug — salvo que el documento esté marcado como pendiente.

| Módulo | Documento | Código |
|---|---|---|
| Movimiento del jugador | `docs/tecnico/01-movimiento.md` | `Assets/Scripts/Player/PlayerMovement.cs` |
| Sistema de sospecha | `docs/tecnico/02-sospecha.md` | `Assets/Scripts/Suspicion/SuspicionSystem.cs` |
| Cámara seguidora | `docs/tecnico/03-camara.md` | `Assets/Scripts/CameraSystem/FollowCamera.cs` |
| Vista del personaje | `docs/tecnico/04-vista-personaje.md` | `Assets/Scripts/Rendering/DirectionalBillboard.cs` |

Contexto general: `docs/tecnico/00-arquitectura.md` (orden de ejecución),
`docs/tecnico/decisiones-abiertas.md` (lo que aún no está decidido).

## La mecánica central

Un sistema de sospecha: hay zonas vigiladas; si Nero permanece en ellas la sospecha sube, si sale
baja, y al llegar al máximo lo detectan. **Todo lo demás está al servicio de eso.** El objetivo
del prototipo no es representar el pitch completo, es validar si esa mecánica se siente bien.

## Invariantes que no se rompen

Estas reglas vienen de los diagramas y su violación produce bugs visuales sutiles y difíciles de
diagnosticar. No las "mejores" sin discutirlo:

1. **Orden dentro del fotograma:** movimiento → sospecha → cámara → vista del personaje.
   Los dos últimos corren en `LateUpdate` con prioridad explícita (`ExecutionOrder.cs`).
   Unity no garantiza el orden entre componentes sin ella.
2. **Una única llamada de movimiento por fotograma.** Avance y caída se suman *antes* de mover.
3. **El empuje al suelo no es cero.** Con cero, el personaje despega en rampas y bajadas.
4. **La sospecha sube o baja, nunca las dos en el mismo fotograma**, y satura en ambos extremos.
5. **`Detected` es un latch.** No vuelve solo a falso; lo reinicia el resto del juego.
6. **La cámara nunca rota.** El ángulo lo fija el desplazamiento de cada zona.
7. **El espejado del billboard es `flipX`**, no una rotación de 180°.

## Convenciones

- Código (clases, variables, funciones) en **inglés**; comentarios y documentación en **español**.
- Nombres de dominio sin traducir: `Nero`, `Aurea`, `Umbria`, `Suspicion`.
- «Umbria» se escribe **sin tilde**.
- Cada script cita en su cabecera el nodo de Figma y el documento que implementa. Manténlo.
- Detalle completo en `docs/tecnico/convenciones.md`.

## Límite de verificación — importante

**No puedes verificar el comportamiento de este proyecto.** Unity no corre en modo headless aquí,
y no hay navegador que abrir: es un juego, no una web. Puedes comprobar que el C# compila y que
la lógica es correcta leyéndola, pero **no puedes saber si la cámara tiembla o si la sospecha se
siente bien**.

Por tanto: **nunca declares un cambio "probado" o "funcionando".** Di qué verificaste y qué queda
por comprobar en el editor, y deja instrucciones concretas de qué mirar en Play mode.

## Alcance

Lo que está **fuera** del prototipo: creación de personaje, inventario, diálogos, mercado negro,
generación dinámica de patrullas, progresión, guardado, menú principal. Está documentado en
`docs/diseno/sistemas-de-juego.md` para cuando llegue su momento — no lo implementes por
iniciativa propia.
