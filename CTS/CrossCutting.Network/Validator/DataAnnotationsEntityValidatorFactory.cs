﻿using CrossCutting.Validator;

namespace CrossCutting.Network.Validator
{
    public class DataAnnotationsEntityValidatorFactory
         : IEntityValidatorFactory
    {
        /// <summary>
        /// Create a entity validator
        /// </summary>
        /// <returns></returns>
        public IEntityValidator Create()
        {
            return new DataAnnotationsEntityValidator();
        }
    }
}
