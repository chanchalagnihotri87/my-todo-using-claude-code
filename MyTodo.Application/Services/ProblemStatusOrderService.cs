using MyTodo.Application.Repository.Interface;
using MyTodo.Application.Services.Interfaces;
using MyTodo.Domain.Enums;

namespace MyTodo.Application.Services
{
    public class ProblemStatusOrderService : IProblemStatusOrderService
    {
        private readonly IProblemStatusOrderRepository _repository;

        public ProblemStatusOrderService(IProblemStatusOrderRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ProblemStatus>> GetOrderAsync()
        {
            var saved = await _repository.GetAllOrderedAsync();
            var order = saved.Select(x => x.Status).ToList();

            foreach (ProblemStatus status in Enum.GetValues<ProblemStatus>())
            {
                if (!order.Contains(status))
                {
                    order.Add(status);
                }
            }

            return order;
        }

        public async Task ReorderAsync(List<ProblemStatus> orderedStatuses)
        {
            await _repository.ReorderAsync(orderedStatuses);
        }
    }
}
