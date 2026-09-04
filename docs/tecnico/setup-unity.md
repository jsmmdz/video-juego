# Puesta en marcha — Unity

## Requisitos

- **Unity 6 LTS** (o 2022.3 LTS como mínimo).
  El código usa `FindFirstObjectByType`, disponible desde 2022.2. En versiones anteriores hay
  que sustituirlo por `FindObjectOfType`.
- Render pipeline: **Built-in** o **URP**, indistinto. Nada del prototipo depende del pipeline.
- Paquete **Unity UI** (`com.unity.ugui`), incluido por defecto.

## Crear el proyecto

Este repositorio contiene la carpeta `Assets/`, no un proyecto de Unity completo:
`ProjectSettings/`, `Packages/` y `Library/` los genera Unity, y `Library/` no se versiona.

1. Clona el repositorio.
2. En Unity Hub → **Add** → **Add project from disk** → selecciona la carpeta del repositorio.
3. Ábrelo. Unity generará `ProjectSettings/` y `Packages/` en el primer arranque e importará
   `Assets/`.
4. Commitea `ProjectSettings/` y `Packages/manifest.json` una vez generados: fijan la versión del
   editor y las dependencias para todo el equipo.

> Alternativa: crear un proyecto nuevo 3D vacío desde el Hub y copiar dentro la carpeta `Assets/`.

## Construir la escena de prototipo

Menú **The Silent Divide ▸ Construir escena de prototipo**.

Genera una escena con todo montado:

- Suelo de blockout a escala real (100 × 100 m, según `docs/diseno/escenarios.md`)
- **Nero**: `CharacterController` + `PlayerMovement` + `SuspicionSystem`, con un billboard hijo
- **Cámara**: ángulo fijo de 45°, `FollowCamera` apuntando al jugador
- Una **zona vigilada** (volumen dorado, visible como gizmo)
- Una **zona de cámara** con plano más cerrado, tipo callejón de Umbria
- **Barra de sospecha** en pantalla
- Ocho cubos de referencia, para tener contra qué juzgar movimiento y cámara

Se puede volver a ejecutar cuantas veces haga falta: crea una escena nueva cada vez.

## Qué verificar

Pulsa Play y comprueba, módulo por módulo:

| Módulo | Qué debe pasar |
|---|---|
| **Movimiento** | La cápsula camina con `WASD`, gira hacia donde va, y cae sin flotar ni despegarse. Al soltar las teclas conserva la orientación. |
| **Cámara** | Sigue con retraso suave, **no tiembla** en movimiento continuo y **no rota nunca**. Al entrar en la zona de cámara, el plano se cierra de golpe (corte seco). |
| **Vista del personaje** | El rectángulo cambia de color al cruzar cada uno de los seis sectores, y **no parpadea** caminando justo sobre una frontera. La muesca negra del borde se invierte al pasar de un lado al otro. |
| **Sospecha** | La barra sube dentro del volumen dorado y baja fuera. Satura arriba y abajo. Al llenarse cambia a rojo y **se queda ahí** (el estado queda congelado). |

## Construir el menú principal

Menú **The Silent Divide ▸ Construir menú principal**.

Genera la escena del menú a partir del kit de UX-UI del board: fondo, título, los cuatro botones
(Nueva Partida, Continuar, Ajustes, Salir) y la ayuda de navegación.

**Requiere TextMeshPro**, incluido por defecto en Unity 6. Si el proyecto no lo tiene, Unity lo
ofrece al abrir la escena (`Window ▸ TextMeshPro ▸ Import TMP Essential Resources`).

### Qué verificar

| Elemento | Qué debe pasar |
|---|---|
| **Estados del botón** | Reposo: contorno claro. Foco: contorno y texto en violeta. Pulsado: relleno violeta con borde claro. |
| **Foco compartido** | Mover el ratón sobre un botón mueve la selección del teclado al mismo botón. **Nunca debe haber dos resaltados a la vez.** |
| **Continuar** | Aparece **deshabilitado** (gris apagado) porque no hay partida guardada. Las flechas lo saltan al navegar. |
| **Forma del botón** | Las esquinas cortadas en diagonal tienen el contorno del **mismo grosor** que los lados rectos. |
| **Nueva Partida** | Carga la escena de juego. Requiere que esté añadida en `File ▸ Build Settings`. |

> El botón «Nueva Partida» avisará por consola si la escena no está en Build Settings, en lugar de
> fallar en silencio. Es lo primero que hay que mirar si no pasa nada al pulsarlo.

## Estructura del código

```
Assets/
├── Editor/
│   └── PrototypeSceneBuilder.cs   monta la escena de prueba
├── Editor/
│   └── MainMenuSceneBuilder.cs    monta la escena del menú
└── Scripts/
    ├── ExecutionOrder.cs          prioridades de LateUpdate (cámara → billboard)
    ├── Player/
    │   └── PlayerMovement.cs      módulo 1
    ├── Suspicion/
    │   ├── SuspicionSystem.cs     módulo 2 — mecánica central
    │   └── SurveillanceZone.cs    marca un volumen como zona vigilada
    ├── CameraSystem/
    │   ├── FollowCamera.cs        módulo 3
    │   └── CameraZone.cs          encuadre propio por zona
    ├── Rendering/
    │   ├── DirectionalBillboard.cs  módulo 4
    │   └── PlaceholderSprites.cs    4 rectángulos de color, sin arte final
    └── UI/
        ├── UITheme.cs             paleta compartida entre menú y HUD
        ├── ChamferedPanel.cs      forma de botón con esquinas cortadas
        ├── MenuButton.cs          estados: reposo, foco, pulsado, deshabilitado
        ├── MainMenuController.cs  navegación y acciones del menú
        └── SuspicionBar.cs        barra de sospecha
```

Cada script referencia en su cabecera el nodo del flowchart de Figma y el documento de
especificación que implementa, para poder leerlos en paralelo.

## Entrada

El prototipo usa el **Input Manager clásico** (`Input.GetAxisRaw`), que funciona sin
configuración adicional. Si el proyecto migra al **Input System** nuevo, el único archivo a
tocar es `PlayerMovement.cs`.
