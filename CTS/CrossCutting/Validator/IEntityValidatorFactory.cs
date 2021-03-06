﻿namespace CrossCutting.Validator
{
    public interface IEntityValidatorFactory
    {
        /// <summary>
        /// Create a new IEntityValidator
        /// </summary>
        /// <returns>IEntityValidator</returns>
        IEntityValidator Create();
    }
}
