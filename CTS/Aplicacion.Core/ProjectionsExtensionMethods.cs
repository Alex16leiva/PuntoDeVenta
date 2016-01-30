using CrossCutting.Adapters;
using Dominio.Core;
using System.Collections;
using System.Collections.Generic;

namespace Aplicacion.Core
{
    public static class ProjectionsExtensionMethods
    {
        public static List<TProjection> ProjectedAsCollection<TProjection>(this IEnumerable<Entity> items)
        where TProjection : class, new()
        {
            var adapter = TypeAdapterFactory.CreateAdapter();
            return adapter.Adapt<List<TProjection>>(items);
        }

        public static List<TProjection> ProjectedAsCollection<TProjection>(this IEnumerable items)
            where TProjection : class, new()
        {
            var adapter = TypeAdapterFactory.CreateAdapter();
            return adapter.Adapt<List<TProjection>>(items);
        }

        public static TProjection ProjectedAs<TProjection>(this Entity item)
        where TProjection : class, new()
        {
            var adapter = TypeAdapterFactory.CreateAdapter();
            return adapter.Adapt<TProjection>(item);
        }
    }
}
