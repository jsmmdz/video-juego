namespace SilentDivide
{
    /// <summary>
    /// Orden de ejecución dentro del fotograma. No es un detalle de implementación: es lo que hace
    /// que la imagen sea correcta. Ver docs/tecnico/00-arquitectura.md
    ///
    ///   movimiento → sospecha → cámara → vista del personaje
    ///
    /// Los dos últimos corren en LateUpdate, y Unity no garantiza el orden entre componentes sin
    /// una prioridad explícita. Estas constantes la fijan.
    /// </summary>
    public static class ExecutionOrder
    {
        /// <summary>La cámara se coloca cuando el jugador ya se movió.</summary>
        public const int Camera = 100;

        /// <summary>
        /// La vista del personaje se resuelve cuando la cámara ya está ubicada: el sector de 60°
        /// se mide respecto a ella, y con una cámara del fotograma anterior el sprite parpadearía
        /// entre dibujos en los giros.
        /// </summary>
        public const int Billboard = 200;
    }
}
