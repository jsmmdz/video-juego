# Sistemas de juego

Derivado del PDF de pitch (páginas 5–8). Ver transcripción completa en
`docs/contexto/pitch-pdf.md`.

## Género

Aventura, infiltración y **sigilo social** en tiempo real. El combate es **secundario**: al ser
descubierto, el jugador escapa, se esconde, distrae al enemigo o usa el entorno, en lugar de
enfrentarse.

**Perspectiva: vista cenital oblicua (aérea en ¾), de ángulo fijo** (pitch v2).

## Estructura de niveles

El juego se divide en niveles que muestran la evolución del jugador.

| Nivel | Escenario | Qué enseña |
|---|---|---|
| **1** | Abajo (Umbria) | Controles básicos, exploración, interacción con objetos, obtención de recursos |
| **2** | La Barrera | Exploración y pequeños puzles para encontrar cómo atravesarla |
| **3** | Arriba (Aurea) | Comienza la infiltración: recorrer la ciudad sin levantar sospechas |

Los siguientes aumentan la dificultad con más vigilancia, zonas restringidas y situaciones que
exigen aplicar lo aprendido.

Los dos mundos tienen enfoques distintos: **Umbria** gira en torno a exploración, supervivencia y
búsqueda de recursos, en espacios reducidos y desordenados; **Aurea** en torno a infiltración y
sigilo, en espacios amplios y luminosos pero muy vigilados.

## Menú principal

- **Nueva Partida** — inicia la historia desde el comienzo.
- **Continuar** — retoma en el último punto guardado, con inventario intacto y omitiendo la
  configuración de personaje.
- **Ajustes de Sistema** — gráficos, audio, controles y accesibilidad.

Precedido por una cinemática de introducción que muestra la brecha entre los rascacielos
iluminados de Aurea y la penumbra de Umbria.

## Creación de personaje

### 1. Origen del Perfil (dentro de Umbria)

Cada perfil otorga ventajas iniciales en áreas específicas.

- Técnico de Mantenimiento
- Informante Clandestino
- Chatarrero de Recursos

### 2. Especialismo de Infiltración

| Especialismo | Efecto |
|---|---|
| **Agente de Sombras** | Movimiento más rápido, sigilo mejorado, menor detección en zonas restringidas |
| **Persuasivo Social** | Opciones de diálogo exclusivas en Aurea para reducir rápido la sospecha |
| **Ingeniero de Redes** | Mayor efectividad hackeando terminales y alterando sensores biométricos |

### 3. Atributos

Cuatro categorías, asignadas por el sistema según las elecciones anteriores, más puntos de
libre asignación:

**Tecnología · Mimetismo · Agilidad · Cultura Aurea**

### 4. Ramas de Sabotaje

Especialización adicional, además de habilidades generales de sigilo comunes a todo perfil:

- Interferencia Electrónica
- Falsificación de Datos
- Modulación de Voz

### Estado inicial

Equipamiento inicial + una batería de luz cargada. Aparece en su refugio subterráneo en
Umbria.

## Inventario y herramientas

| Objeto | Función |
|---|---|
| **Inhibidor de Frecuencia de Umbría** | Dispositivo artesanal; ciega temporalmente las cámaras de seguridad |
| **Pase Biométrico Falsificado** | Acceso a sectores restringidos simulando ser ciudadano de Aurea |
| **Modulador de Voz** | Adapta acento y léxico de Nero para no generar sospechas en diálogos |
| **Escáner de Frecuencias** (casero) | Visualiza conos de visión de guardias, alcance de cámaras y puntos de hackeo |

Otros recursos: credenciales falsificadas, códigos de acceso, llaves, baterías, reparaciones,
suministros de salud.

## Visibilidad y detección

- Las zonas no iluminadas y los conductos de ventilación dan **cobertura**.
- Los **escáneres infrarrojos** de Aurea detectan la firma térmica a través de la penumbra
  (siluetas rojas en la interfaz).
- El mapa se **descubre progresivamente** al desplazarse.
- Entrar corriendo en un cono de visión → se interrumpe la acción y se activa **Alerta Local**.
- Ser detectado sin pase biométrico → alarma de Aurea, accesos bloqueados, reinicio desde el
  último punto de control.

## Bucle entre incursiones

Entre incursiones a Aurea, Nero vuelve a Umbria para:

- Visitar puestos clandestinos y conseguir suministros
- Acudir al médico del mercado negro para curarse
- Hablar con informantes de la resistencia (pistas sobre la Barrera)
- Descansar en el refugio y **guardar** datos robados o recursos que no quiera llevar consigo

## Personajes no jugables

Tendrán **rutinas y comportamientos propios**. El jugador podrá hablar con ellos, observar sus
movimientos y usar la información obtenida para avanzar. Algunos podrán ayudarlo y **otros podrán
aumentar el nivel de sospecha** — es decir, la sospecha no viene solo de cámaras y guardias, sino
también de la interacción social.

## Progresión

⚠️ Hay **dos modelos incompatibles** entre el pitch y la narrativa: por atributos y puntos, o por
conocimiento del entorno. Sin decidir — ver
[decisiones abiertas #11](../tecnico/decisiones-abiertas.md).

## Rejugabilidad y expansión

Generación dinámica de patrullas, personajes, objetos y eventos sociales impredecibles: ninguna
partida igual a la anterior.

Post-lanzamiento: nuevos dispositivos de infiltración, tipos de drones, identidades falsas y
sectores enteros de la ciudad (distritos industriales de Umbria, zonas residenciales exclusivas
de Aurea).
