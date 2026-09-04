using System.Collections.Generic;
using UnityEngine;

namespace SilentDivide.UI
{
    /// <summary>
    /// Recorrido del foco por una columna de <see cref="MenuItem"/>, y lectura del teclado.
    ///
    /// Es una clase normal, no un componente: el menú principal y la pantalla de Ajustes tienen uno
    /// cada uno, y solo el de la pantalla visible procesa entrada. Así el foco de cada pantalla se
    /// conserva al ir y volver, en vez de reiniciarse.
    /// </summary>
    public sealed class MenuNavigator
    {
        private readonly List<MenuItem> items;
        private int focusedIndex = -1;

        public MenuNavigator(IEnumerable<MenuItem> items)
        {
            this.items = new List<MenuItem>();
            foreach (MenuItem item in items)
                if (item != null) this.items.Add(item);

            foreach (MenuItem item in this.items)
                item.OnFocusRequested += Focus;
        }

        /// <summary>Hay que llamarlo al destruir la pantalla: los eventos son de C#, no de Unity.</summary>
        public void Dispose()
        {
            foreach (MenuItem item in items)
                if (item != null) item.OnFocusRequested -= Focus;
        }

        // ── Entrada ──────────────────────────────────────────────────────────────────────────

        /// <summary>Lo llama el controlador de la pantalla activa, una vez por fotograma.</summary>
        public void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
                Step(1);
            else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
                Step(-1);
            else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
                AdjustFocused(-1);
            else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
                AdjustFocused(1);
            else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)
                     || Input.GetKeyDown(KeyCode.Space))
                ActivateFocused();
        }

        // ── Foco ─────────────────────────────────────────────────────────────────────────────

        public void Focus(MenuItem item)
        {
            int index = items.IndexOf(item);
            if (index >= 0) SetFocus(index);
        }

        private void SetFocus(int index)
        {
            for (int i = 0; i < items.Count; i++)
                if (items[i] != null)
                    items[i].SetFocused(i == index);

            focusedIndex = index;
        }

        /// <summary>
        /// Avanza saltando las filas deshabilitadas —los encabezados de sección lo están— y
        /// recorre como mucho una vuelta completa, para no colgarse si ninguna es seleccionable.
        /// </summary>
        public void Step(int direction)
        {
            if (items.Count == 0) return;

            int index = focusedIndex;
            for (int i = 0; i < items.Count; i++)
            {
                index = (index + direction + items.Count) % items.Count;
                if (items[index] != null && items[index].Interactable)
                {
                    SetFocus(index);
                    return;
                }
            }
        }

        public void FocusFirstAvailable()
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] != null && items[i].Interactable)
                {
                    SetFocus(i);
                    return;
                }
            }
        }

        /// <summary>Apaga el resaltado sin olvidar cuál era la fila activa.</summary>
        public void ClearHighlight()
        {
            foreach (MenuItem item in items)
                if (item != null) item.SetFocused(false);
        }

        /// <summary>Vuelve a resaltar la fila que quedó enfocada al salir de la pantalla.</summary>
        public void RestoreHighlight()
        {
            if (focusedIndex >= 0 && focusedIndex < items.Count) SetFocus(focusedIndex);
            else FocusFirstAvailable();
        }

        private void ActivateFocused()
        {
            if (focusedIndex < 0 || focusedIndex >= items.Count) return;
            if (items[focusedIndex] != null) items[focusedIndex].Activate();
        }

        private void AdjustFocused(int direction)
        {
            if (focusedIndex < 0 || focusedIndex >= items.Count) return;
            if (items[focusedIndex] != null) items[focusedIndex].Adjust(direction);
        }
    }
}
