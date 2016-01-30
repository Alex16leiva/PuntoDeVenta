namespace CrossCutting.Validator
{
    public static class EntityValidatorFactory
    {
        #region Members

        static IEntityValidatorFactory _factory;

        #endregion

        #region Public Methods

        /// <summary>
        /// Set the  log factory to use
        /// </summary>
        /// <param name="factory">Log factory to use</param>
        public static void SetCurrent(IEntityValidatorFactory factory)
        {
            _factory = factory;
        }

        /// <summary> Createt a new 
        ///     <paramref>
        ///         <name>Microsoft.Samples.NLayerApp.Infrastructure.Crosscutting.Logging.ILog</name>
        ///     </paramref>
        /// </summary>
        /// <returns>Created ILog</returns>
        public static IEntityValidator CreateValidator()
        {
            return (_factory != null) ? _factory.Create() : null;
        }

        #endregion
    }
}
