using MyTodo.Application.Repositories.Interfaces;

namespace MyTodo.Application.Services.Common
{
    public static class ReorderHelper
    {
        public static async Task<bool> ReindexAsync<TEntity>(
            IBaseRepository<TEntity> repository,
            Func<TEntity, int> idSelector,
            List<int> orderedIds,
            Action<TEntity, int> applyOrder,
            int? anchorId = null,
            Action<TEntity>? applyToAnchor = null) where TEntity : class
        {
            var entities = await repository.GetByIdsAsync(orderedIds);
            var byId = entities.ToDictionary(idSelector);
            var anchorFound = !anchorId.HasValue;

            for (var index = 0; index < orderedIds.Count; index++)
            {
                if (!byId.TryGetValue(orderedIds[index], out var entity))
                {
                    continue;
                }

                applyOrder(entity, index);

                if (anchorId.HasValue && orderedIds[index] == anchorId.Value)
                {
                    applyToAnchor?.Invoke(entity);
                    anchorFound = true;
                }
            }

            if (entities.Count > 0)
            {
                await repository.UpdateRangeAsync(entities);
            }

            return anchorFound;
        }
    }
}
