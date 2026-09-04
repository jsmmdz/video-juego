# Cambios del pitch entre v1 y v2

Comparación literal entre `fuentes/The_Silent_Divide_v1.pdf` y `fuentes/The_Silent_Divide_v2.pdf`.
La v2 tiene 9 páginas frente a las 11 de la v1, y añade créditos del equipo.

---

## 1. 🔴 Cambio de perspectiva — el más importante

| v1 | v2 |
|---|---|
| «El juego se desarrolla desde **dos perspectivas**: tercera persona, donde la cámara acompaña a Nero y permite observar su cuerpo y el entorno, y **vista central**, que ofrece una visión general de los escenarios.» | «El juego se desarrolla desde una perspectiva de **vista cenital oblicua o vista aérea en ¾**.» |

**Por qué importa:** la v1 planteaba dos sistemas de cámara distintos y no decía cuándo se usaba
cada uno. La v2 define **una sola perspectiva fija**, y es exactamente la que ya está
implementada: cámara de ángulo fijo que sigue la posición del jugador y **nunca rota**
(`docs/tecnico/03-camara.md`).

También justifica retroactivamente el módulo de **vista del personaje** (billboard direccional):
un personaje plano orientado a cámara solo funciona con una cámara de ángulo estable. Con la
tercera persona libre de la v1, ese enfoque no se sostendría.

**Efecto sobre el código: ninguno.** La arquitectura ya estaba alineada con la v2.

---

## 2. 🟡 De «partida» a «nivel»

| v1 | v2 |
|---|---|
| «ninguna **partida** será igual a la anterior» | «ningún **nivel** será igual al anterior» |
| «El jugador podrá desplazarse por los escenarios…» | «**En cada nivel** el jugador podrá desplazarse…» |
| «los **niveles** más protegidos de Aurea» | «los **lugares** más protegidos de Aurea» |

El juego pasa de leerse como una experiencia continua a una **estructura por niveles**, coherente
con los tres niveles que define la narrativa (ver `narrativa.md`). Afecta al alcance: hay
progresión entre escenas, no un mundo único.

---

## 3. 🟢 Otros

- Se añade el rótulo **GAME DESIGN** como título de sección.
- Se añaden los **créditos del equipo** al final.
- Se elimina una página en blanco.

---

## Lo que NO cambió

**La contradicción del esquema de control sigue ahí.** La v2 mantiene, sin tocar:

- Página 6: control **point-and-click** (clic en el suelo, el personaje va por la ruta más corta;
  las teclas de dirección desplazan la vista del mapa).
- Página 7: control **WASD** directo, con salto, esprint y agacharse.

Ver `docs/tecnico/decisiones-abiertas.md` #2.
