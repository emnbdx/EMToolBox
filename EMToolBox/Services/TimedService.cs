using System;
using System.Threading;

namespace EMToolBox.Services
{
    public abstract class TimedService
    {
        #region Private & Internal

        private DebuggableService m_service = null;

        private bool m_stopping = false;

        internal void StopProcessing()
        {
            bool firstStopRequest;
            lock (this)
            {
                firstStopRequest = !m_stopping; // Was the service already stopping or is it the first stop request received?
                m_stopping = true;
            }

            // First stop request? This test is to garantee that the 'Finalize()' method won't be called more than once...
            if (firstStopRequest) Terminate();
        }

        internal DebuggableService Service { get { return m_service; } set { m_service = value; } }

        #endregion Private & Internal
        #region Public & Protected

        /// <summary>
        /// Constructeur
        /// </summary>
        public TimedService()
        {
            this.ThreadApartmentState = ApartmentState.MTA;
            Interval = 60 * 1000;
        }

        /// <summary>
        /// Le service est en cours d'arr�t
        /// </summary>
        protected bool Stopping { get { lock (this) { return m_stopping; } } }

        /// <summary>
        /// Virtuelle appel�e au moment de l'initialisation du service, avant le premier appel
        /// � la m�thode 'Process()'
        /// </summary>
        public virtual void Initialize() { }

        /// <summary>
        /// Virtuelle appel�e � chaque expiration de l'intervalle de d�clenchement du service
        /// </summary>
        public virtual void Process() { }

        /// <summary>
        /// Virtuelle appel�e � chaque expiration de l'intervalle de d�clenchement du service.
        /// </summary>
        /// <param name="firstProcess">True s'il s'agit du premier appel de la m�thode</param>
        public virtual void Process(bool firstProcess) { }

        /// <summary>
        /// Virtuelle appel�e lors de la demande d'arr�t du service
        /// </summary>
        public virtual void Terminate() { }

        /// <summary>
        /// Appel�e lors d'un crash du service
        /// </summary>
        /// <param name="e"></param>
        public virtual void Crash(Exception e) { }

        /// <summary>
        /// Demande d'arr�t au gestionnaire de services
        /// </summary>
        public void Stop()
        {
            m_service.Stop();
        }

        /// <summary>
        /// Intervalle de d�clenchement du service
        /// </summary>
        public int Interval { get; set; }

        /// <summary>
        /// Define the apartment state of the main thread of the service
        /// </summary>
        public virtual ApartmentState ThreadApartmentState { get; private set; }

        #endregion Public & Protected
    }
}
