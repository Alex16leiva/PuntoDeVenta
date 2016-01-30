using System.Data.Entity.Infrastructure;

namespace Infraestructura.Core
{
    class ModifiedEntityEntry
    {
        public DbEntityEntry EntityEntry
        {
            get { return _entityEntry; }
        }
        private DbEntityEntry _entityEntry;



        public string State
        {
            get { return _state; }
        }
        private string _state;

        public ModifiedEntityEntry(DbEntityEntry entityEntry, string state)
        {
            _entityEntry = entityEntry;
            _state = state;
        }
    }
}
