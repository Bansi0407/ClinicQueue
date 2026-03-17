using ClinicQueue.Models;
using ClinicQueue.ViewModels;

namespace ClinicQueue.Services.Interfaces
{
    public interface IPatientService
    {
        Task<bool> BookAppointmentAsync(BookAppointmentVM model, string patientId);
        Task<List<Appointment>> GetMyAppointmentsAsync();
        Task<Appointment?> GetAppointmentByIdAsync(string id);
        Task<List<Prescription>> GetMyPrescriptionsAsync();
        Task<List<Report>> GetMyReportsAsync();
        Task<List<User>> GetDoctorsAsync();
    }
}



/*using ClinicQueue.Models;
using ClinicQueue.ViewModels;

namespace ClinicQueue.Services.Interfaces
{
    public interface IPatientService
    {
        Task<bool> BookAppointmentAsync(BookAppointmentVM model);
        Task<List<Appointment>> GetMyAppointmentsAsync();
        Task<Appointment?> GetAppointmentByIdAsync(string id);
        Task<List<Prescription>> GetMyPrescriptionsAsync();
        Task<List<Report>> GetMyReportsAsync();
        Task<List<User>> GetDoctorsAsync();
    }
}*/
