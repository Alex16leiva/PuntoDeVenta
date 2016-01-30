using System;
using System.Collections.Generic;

namespace Dominio.Core
{
    public abstract class Entity
    {
        protected Entity()
        {

        }

        public Entity(string modificadoPor)
        {
            ModificadoPor = modificadoPor;
        }

        public string ModificadoPor { get; set; }
        public DateTime FechaTransaccion { get; set; }
        public string DescripcionTransaccion { get; set; }
        public byte[] RowVersion { get; set; }
        public Guid TransaccionUId { get; set; }
        public string TipoTransaccion { get; set; }
        public DateTime FechaTransaccionUtc { get; set; }

        public Dictionary<string, List<string>> ErroresValidacion
        {
            get
            {
                return _erroresValidacion ?? (_erroresValidacion = new Dictionary<string, List<string>>());
            }
        }
        private Dictionary<string, List<string>> _erroresValidacion;
    }
}
