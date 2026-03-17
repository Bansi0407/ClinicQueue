using ClinicQueue.Models;

namespace ClinicQueue.Services.Interfaces
{
    public interface IReceptionService
    {
        Task<List<QueueEntry>> GetDailyQueueAsync(string date);
        Task<bool> UpdateStatusAsync(string queueId, string status);
    }
}