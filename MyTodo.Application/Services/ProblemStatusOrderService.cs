using Microsoft.Extensions.Logging;
using MyTodo.Application.Repositories.Interfaces;
using MyTodo.Application.Services.Interfaces;
using MyTodo.Domain.Shared.Enums;

namespace MyTodo.Application.Services
{
    public class ProblemStatusOrderService : IProblemStatusOrderService
    {
        private readonly IProblemStatusOrderRepository _repository;
        private readonly ILogger<ProblemStatusOrderService> _logger;

        public ProblemStatusOrderService(IProblemStatusOrderRepository repository, ILogger<ProblemStatusOrderService> logger)
        {
            _repository = repository;
            _logger = logger;
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

            _logger.LogInformation("Problem status order updated to {OrderedStatuses}", string.Join(",", orderedStatuses));
        }
    }
}
