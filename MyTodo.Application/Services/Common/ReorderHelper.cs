using MyTodo.Application.Repositories.Interfaces;

namespace MyTodo.Application.Services.Common
{
    public static class ReorderHelper
    {
        public static async Task ReindexAsync<TEntity>(
            IBaseRepository<TEntity> repository,
            TEntity anchor,
            int anchorId,
            List<int> orderedIds,
            Action<TEntity, int> applyOrder) where TEntity : class
        {
            for (var index = 0; index < orderedIds.Count; index++)
            {
                var current = orderedIds[index] == anchorId ? anchor : await repository.GetByIdAsync(orderedIds[index]);
                if (current == null)
                {
                    continue;
                }

                applyOrder(current, index);
                await repository.UpdateAsync(current);
            }
        }
    }
}
