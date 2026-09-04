---
trigger: always_on
---

# Qué significa "verificado" en este proyecto

Este es un juego de Unity. **No hay navegador que abrir ni servidor que levantar**, y el editor de
Unity no corre en modo headless en el entorno del agente.

## Lo que sí puedes verificar

- Que el C# compila y no tiene errores de nombres o de tipos.
- Que la lógica coincide, paso a paso, con el flowchart de su documento.
- Que los invariantes de `AGENTS.md` se respetan.
- Matemática pura (ángulos, sectores, curvas): escribe un script pequeño y **ejecútalo** para
  comprobar los valores, no lo razones de cabeza.

## Lo que NO puedes verificar

Nada que dependa de ver el juego en marcha: si la cámara tiembla, si el sprite parpadea en los
giros, si la curva de sospecha da margen suficiente, si el personaje se engancha en una rampa.

## Cómo cerrar una tarea

Termina siempre con dos listas separadas:

1. **Verificado:** qué comprobaste y cómo.
2. **Por comprobar en el editor:** pasos concretos en Play mode y qué debería verse.

**No digas que algo funciona si no lo has visto funcionar.** Un "implementado y probado" falso
cuesta más que un "implementado, falta probar esto" honesto.
