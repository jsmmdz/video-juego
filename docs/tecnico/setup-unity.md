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

**Unity Hub no puede abrir el repositorio tal cual.** «Add project from disk» comprueba que exista
`ProjectSettings/ProjectVersion.txt` y rechaza la carpeta si no está. Así que primero hay que
fabricar esos archivos, una sola vez y para todo el equipo:

1. Clona el repositorio y sitúate en la rama de trabajo.
2. En Unity Hub → **New project** → **3D (Built-in Render Pipeline)** → Unity 6 LTS.
   Créalo **fuera** del repositorio, con cualquier nombre: es desechable.
3. Cierra Unity cuando termine de crearlo.
4. Copia `ProjectSettings/` y `Packages/` del proyecto desechable **dentro** de la carpeta del
   repositorio. Ya puedes borrar el desechable.
5. Unity Hub → **Add** → **Add project from disk** → ahora sí, la carpeta del repositorio. Ábrelo.
6. Unity importará `Assets/` y compilará. La primera vez tarda.
7. Commitea `ProjectSettings/` y `Packages/manifest.json`: fijan la versión del editor y las
   dependencias, y a partir de ahí el resto del equipo se salta los pasos 2 a 4 y abre el
   repositorio directamente.

> `Library/`, `*.csproj` y `*.sln` los genera Unity y están en `.gitignore`. No los subas.

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

## Construir la pantalla de inicio

Menú **The Silent Divide ▸ Construir menú principal**.

Genera la escena de inicio a partir del mockup definitivo del kit de UX-UI: ilustración a pantalla
completa, velo oscuro en la columna izquierda, logotipo de tres líneas, las opciones «Jugar» y
«Ajustes», y la pantalla de Ajustes de Sistema oculta detrás.

**Requiere TextMeshPro**, incluido por defecto en Unity 6. Si el proyecto no lo tiene, Unity lo
ofrece al abrir la escena (`Window ▸ TextMeshPro ▸ Import TMP Essential Resources`).

### Arte que hay que colocar antes

Los dos faltan y la escena se construye igual, avisando por consola:

| Archivo | Dónde va | Si falta |
|---|---|---|
| Ilustración del callejón | `Assets/Art/UI/Menu/inicio-fondo.png` | Fondo de color plano |
| `Dune_Rise.ttf` | `Assets/Art/UI/Fonts/` | Título con la fuente por defecto de TMP |

La ilustración hay que importarla como **Sprite (2D and UI)** en el inspector, o
`AssetDatabase` no la encuentra y el constructor la da por ausente.

Para la tipografía: `Window ▸ TextMeshPro ▸ Font Asset Creator`, generar el *font asset* desde el
`.ttf`, y asignarlo en `MainMenuSceneBuilder.NewLabel`. Ver
[decisiones abiertas #8](decisiones-abiertas.md).

### Qué verificar

| Elemento | Qué debe pasar |
|---|---|
| **Composición** | La columna cae donde el mockup: velo en el 27 % izquierdo, título arriba con «SILENT» y «DIVIDE» **desbordando** el velo sobre la ilustración. Prueba a cambiar el aspecto del Game view (16:9, 16:10, 21:9): la ilustración se recorta, **nunca se deforma**, y la columna no se descoloca. |
| **Estados de la opción** | Reposo: texto hueso, filete gris tenue. Foco: texto blanco, filete **ámbar**. Pulsado: el filete se aclara y **engorda al doble**. |
| **Foco compartido** | Mover el ratón sobre una opción mueve la selección del teclado a la misma. **Nunca debe haber dos resaltadas a la vez.** |
| **Jugar** | Carga la escena de juego. Requiere que esté añadida en `File ▸ Build Settings`. |
| **Ajustes** | La columna del menú desaparece y sale la pantalla de Ajustes. La ilustración **no parpadea** al entrar ni al salir: es la misma escena. |
| **Escape** | Cierra Ajustes y devuelve el foco a la opción donde estaba, no a la primera. |

> «Jugar» avisará por consola si la escena de juego no está en Build Settings, en lugar de fallar
> en silencio. Es lo primero que hay que mirar si no pasa nada al pulsarlo.

### Qué verificar en Ajustes

| Elemento | Qué debe pasar |
|---|---|
| **Cabe entera** | Se ve «Volver» sin desplazar la vista. Es lo más justo de la pantalla: la última fila termina a 1044 px de los 1080 de referencia. |
| **Tamaño del texto** | Ponlo en «Muy grande» y vuelve a mirar lo anterior: es donde puede desbordar. |
| **Encabezados** | Las flechas **saltan** GRÁFICOS, AUDIO, CONTROLES y ACCESIBILIDAD, y las tres filas de controles, que son de solo lectura. |
| **Barras** | Flecha izquierda y derecha mueven de 5 en 5 y **saturan** en 0 % y 100 %, sin dar la vuelta. Clic sobre el canal salta al punto pulsado; clic fuera del canal no mueve nada. |
| **Selectores** | Enter y clic en la mitad derecha avanzan; clic en la mitad izquierda retrocede. **Dan la vuelta** en los extremos, al revés que las barras. |
| **Persistencia** | Cambia volumen y calidad, sal del Play mode, vuelve a entrar: los valores siguen puestos. Se guardan en `PlayerPrefs`. |
| **Pantalla completa** | En el editor no hace nada visible; es normal. Se comprueba en una build. |

> Música y efectos se guardan pero **todavía no suenan**: no hay `AudioMixer` en el proyecto.
> Solo el volumen general llega al motor.

## Estructura del código

```
Assets/
├── Art/UI/                        arte de interfaz (falta: ver tabla de arriba)
├── Editor/
│   ├── PrototypeSceneBuilder.cs   monta la escena de prueba
│   └── MainMenuSceneBuilder.cs    monta la pantalla de inicio y la de Ajustes
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
        ├── UITheme.cs             paleta, tomada del mockup de inicio
        ├── MenuItem.cs            base de todo lo seleccionable
        ├── MenuNavigator.cs       recorrido del foco y lectura del teclado
        ├── MenuButton.cs          opción: rótulo + filete
        ├── MenuOption.cs          fila de Ajustes que recorre valores
        ├── MenuSlider.cs          fila de Ajustes con valor continuo
        ├── MainMenuController.cs  pantalla de inicio
        ├── SettingsPanel.cs       Ajustes de Sistema
        ├── GameSettings.cs        persistencia en PlayerPrefs
        ├── TextScaler.cs          tamaño de texto de accesibilidad
        ├── ChamferedPanel.cs      forma con esquinas cortadas (para el HUD)
        └── SuspicionBar.cs        barra de sospecha
```

Cada script referencia en su cabecera el nodo del flowchart de Figma y el documento de
especificación que implementa, para poder leerlos en paralelo.

## Entrada

El prototipo usa el **Input Manager clásico** (`Input.GetAxisRaw`), que funciona sin
configuración adicional. Si el proyecto migra al **Input System** nuevo, el único archivo a
tocar es `PlayerMovement.cs`.
