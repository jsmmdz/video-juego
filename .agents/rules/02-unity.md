---
trigger: always_on
---

# Unity — reglas del repositorio

## Archivos generados

**Nunca edites a mano** archivos `.meta`, `.unity`, `.prefab` ni `.asset`. Son YAML generado por
el editor; tocarlos a mano corrompe referencias de forma difícil de detectar.

La escena de prototipo **no se versiona**: la genera `Assets/Editor/PrototypeSceneBuilder.cs`
desde el menú `The Silent Divide ▸ Construir escena de prototipo`. Si necesitas cambiar la escena
de prueba, **modifica el constructor**, no la escena.

## Orden de ejecución

Cualquier componente nuevo que dependa de la posición del jugador o de la cámara debe declarar su
prioridad en `Assets/Scripts/ExecutionOrder.cs` y usar `[DefaultExecutionOrder]`. No confíes en el
orden por defecto de `LateUpdate`.

## Entrada

El prototipo usa el Input Manager clásico (`Input.GetAxisRaw`). Si algún día se migra al Input
System nuevo, el único archivo que debe cambiar es `PlayerMovement.cs`.

## Versión

Unity 6 LTS (mínimo 2022.3). `FindFirstObjectByType` requiere 2022.2+.
