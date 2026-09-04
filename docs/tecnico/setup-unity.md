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
`ProjectSettings/ProjectVersion.txt` y rechaza la carpeta si no está.

Ese archivo tiene **una sola línea** con la versión del editor. Basta con crearlo: Unity genera el
resto de `ProjectSettings/` y un `Packages/manifest.json` por defecto en el primer arranque. No
hace falta un proyecto desechable del que copiar nada.

### Windows (PowerShell)

```powershell
cd $HOME\Documents
git clone https://github.com/jsmmdz/video-juego.git
cd video-juego
git checkout claude/chat-continuation-oszk43

New-Item -ItemType Directory -Force -Path ProjectSettings | Out-Null
Set-Content ProjectSettings\ProjectVersion.txt "m_EditorVersion: 6000.6.0f1" -Encoding ASCII
Get-Content ProjectSettings\ProjectVersion.txt
```

Sustituye `6000.6.0f1` por tu versión, que es el nombre de la carpeta en
**Unity Hub ▸ Installs** (tres puntos de la versión → **Show in Explorer**).

**`-Encoding ASCII` es obligatorio.** El `-Encoding UTF8` de PowerShell 5 antepone un BOM
invisible al archivo, Unity no lo parsea y el Hub muestra el proyecto con la versión
«Desconocido» y un triángulo de aviso, sin decir por qué. La última línea imprime el archivo para
comprobarlo: tiene que salir exactamente `m_EditorVersion: <tu versión>`.

### macOS y Linux

```bash
git clone https://github.com/jsmmdz/video-juego.git
cd video-juego
git checkout claude/chat-continuation-oszk43

mkdir -p ProjectSettings
echo "m_EditorVersion: 6000.6.0f1" > ProjectSettings/ProjectVersion.txt   # pon tu versión
```

### Después, en las dos

1. Unity Hub → **Add** → **Add project from disk** → la carpeta `video-juego`.
   Si ya lo habías añadido antes de crear `ProjectVersion.txt`, el Hub lo tiene cacheado como
   «Desconocido»: quítalo de la lista (los tres puntos → **Remove project**) y vuelve a añadirlo.
2. Ábrelo. La primera importación tarda: Unity genera `Library/`, el resto de `ProjectSettings/`
   y `Packages/manifest.json`, y compila los scripts.
3. **Unity arrancará en Safe Mode con decenas de errores de compilación.** Es lo esperado la
   primera vez y no es culpa del código: al generar `Packages/manifest.json` desde cero, Unity
   escribe un juego de paquetes mínimo que **no incluye Unity UI**, y sin él no existen
   `VertexHelper`, `IPointerEnterHandler` ni el resto de la interfaz.

   Arréglalo en **Window ▸ Package Manager** → desplegable **Unity Registry** → busca
   **Unity UI** → **Install**. Ese paquete basta: en Unity 6 `com.unity.ugui` trae TextMeshPro
   dentro. Unity recompila y sale solo del Safe Mode.

4. Commitea `ProjectSettings/` y `Packages/manifest.json`. Fijan versión de editor y dependencias,
   y a partir de ahí el resto del equipo clona y abre sin ningún paso previo — incluido el paquete
   de Unity UI, así que no repetirán el paso 3.

> Si el Hub avisa de que el proyecto se hizo con otra versión, aquí se puede aceptar sin miedo:
> `Assets/` solo contiene archivos `.cs`, que no tienen formato serializado que migrar.

> `Library/`, `*.csproj` y `*.sln` los genera Unity y están en `.gitignore`. No los subas.

## Dejarlo jugable de una vez

Menú **The Silent Divide ▸ Construir todo y dejarlo jugable**.

Es el atajo: construye las dos escenas, las guarda en `Assets/Scenes/` y las registra en Build
Settings con el inicio de primera. Al terminar deja abierta la pantalla de inicio, lista para
pulsar Play y recorrerlo entero — menú, «Jugar», y moverse por el escenario.

Los dos comandos de abajo hacen lo mismo por separado, cuando solo se está tocando una de las dos.

> Las escenas **no se versionan** (ver `.gitignore`): las genera el constructor y así no hay
> conflictos de YAML al trabajar en paralelo. Cada quien las construye una vez en su clon.

## Construir la escena de prototipo

Menú **The Silent Divide ▸ Construir escena de prototipo**.

Genera una escena con todo montado:

- Suelo de blockout a escala real (100 × 100 m, según `docs/diseno/escenarios.md`)
- **Nero**: `CharacterController` + `PlayerMovement` + `SuspicionSystem`, con un billboard hijo
- **Cámara**: ángulo fijo de 45°, `FollowCamera` apuntando al jugador
- Una **zona vigilada**, con su huella en bronce pintada en el suelo
- Una **zona de cámara** con plano más cerrado, tipo callejón de Umbria, con huella violeta
- Una **escalera de bloques** y una plataforma, para probar el salto
- **Barra de sospecha** en pantalla
- Ocho cubos de referencia de colores, para tener contra qué juzgar movimiento y cámara

Se puede volver a ejecutar cuantas veces haga falta: crea una escena nueva cada vez, la guarda en
`Assets/Scenes/Prototipo.unity` y la registra en Build Settings.

## Qué verificar

Pulsa Play y comprueba, módulo por módulo:

| Módulo | Qué debe pasar |
|---|---|
| **Movimiento** | La cápsula camina con `WASD`, gira hacia donde va, y cae sin flotar ni despegarse. Al soltar las teclas conserva la orientación. |
| **Salto** | Con `Espacio` salta ~1,6 m. Sube la escalera de bloques escalón a escalón y alcanza la plataforma de madera. **No** se puede saltar dos veces en el aire. Ver [01-movimiento.md](01-movimiento.md). |
| **Referencias** | Los ocho cubos del círculo tienen colores distintos, de las paletas de Umbria y Aurea. Sirven para saber hacia dónde apunta la cámara al girar. |
| **Suelo** | Damero de 2 × 2 m en tonos de Umbria. Es la regla del escenario: sirve para medir de un vistazo cuánto avanza el personaje por segundo y cuánto cubre un salto. |
| **Zonas** | Las dos zonas tienen su huella pintada en el suelo: **bronce** la vigilada, **violeta** la de cámara. La barra debe empezar a subir justo al pisar el bronce. |
| **Cámara** | Sigue con retraso suave, **no tiembla** en movimiento continuo y **no rota nunca**. Al entrar en la zona de cámara, el plano se cierra de golpe (corte seco). |
| **Vista del personaje** | El rectángulo cambia de color al cruzar cada uno de los seis sectores, y **no parpadea** caminando justo sobre una frontera. La muesca negra del borde se invierte al pasar de un lado al otro. |
| **Sospecha** | La barra sube dentro del volumen dorado y baja fuera. Satura arriba y abajo. Al llenarse cambia a rojo y **se queda ahí** (el estado queda congelado). |

## Construir la pantalla de inicio

Menú **The Silent Divide ▸ Construir menú principal**.

Genera la escena de inicio a partir del mockup definitivo del kit de UX-UI: ilustración a pantalla
completa, velo oscuro en la columna izquierda, logotipo de tres líneas, las opciones «Jugar» y
«Ajustes», y la pantalla de Ajustes de Sistema oculta detrás.

**Requiere TextMeshPro**, que en Unity 6 viene dentro de `com.unity.ugui`. Si Unity ofrece
importar los recursos (`Window ▸ TextMeshPro ▸ Import TMP Essential Resources`), acepta: sin ellos
no hay fuente por defecto y las etiquetas salen vacías.

### Arte que hay que colocar antes

Los dos faltan y la escena se construye igual, avisando por consola:

| Archivo | Dónde va | Si falta |
|---|---|---|
| Ilustración del callejón | cualquier imagen en `Assets/Art/UI/Menu/` | Fondo de color plano |
| `Dune_Rise.ttf` | `Assets/Art/UI/Fonts/` | Título con la fuente por defecto de TMP |

La ilustración basta con soltarla en esa carpeta: da igual el nombre y el formato, y el
constructor la reimporta solo como **Sprite (2D and UI)** si hace falta. Si hay varias imágenes
coge la primera, así que deja solo la que quieras de fondo.

Para la tipografía: `Window ▸ TextMeshPro ▸ Font Asset Creator`, generar el *font asset* desde el
`.ttf`, y asignarlo en `MainMenuSceneBuilder.NewLabel`. Ver
[decisiones abiertas #8](decisiones-abiertas.md).

### Qué verificar

| Elemento | Qué debe pasar |
|---|---|
| **Composición** | La columna cae donde el mockup: velo en el 27 % izquierdo, título arriba con «SILENT» y «DIVIDE» **desbordando** el velo sobre la ilustración. Prueba a cambiar el aspecto del Game view (16:9, 16:10, 21:9): la ilustración se recorta, **nunca se deforma**, y la columna no se descoloca. |
| **Estados de la opción** | Reposo: texto hueso, filete gris tenue. Foco: texto blanco, filete **ámbar**. Pulsado: el filete se aclara y **engorda al doble**. |
| **Foco compartido** | Mover el ratón sobre una opción mueve la selección del teclado a la misma. **Nunca debe haber dos resaltadas a la vez.** |
| **Jugar** | Carga la escena de juego. Si construiste con «Construir todo», ya está registrada. |
| **Ajustes** | La columna del menú desaparece y sale la pantalla de Ajustes. La ilustración **no parpadea** al entrar ni al salir: es la misma escena. |
| **Escape** | Cierra Ajustes y devuelve el foco a la opción donde estaba, no a la primera. |

> «Jugar» avisará por consola si la escena de juego no está en Build Settings, en lugar de fallar
> en silencio. Es lo primero que hay que mirar si no pasa nada al pulsarlo: se arregla ejecutando
> **Construir todo y dejarlo jugable**.

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
        ├── MenuEntry.cs           base de todo lo seleccionable
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
