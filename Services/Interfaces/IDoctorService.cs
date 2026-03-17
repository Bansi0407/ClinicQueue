using ClinicQueue.Models;
using ClinicQueue.ViewModels;

namespace ClinicQueue.Services.Interfaces
{
    public interface IDoctorService
    {
        Task<List<QueueEntry>> GetQueueAsync();
        Task<bool> AddPrescriptionAsync(string appointmentId, PrescriptionVM model);
        Task<bool> AddReportAsync(string appointmentId, ReportVM model);
    }
}