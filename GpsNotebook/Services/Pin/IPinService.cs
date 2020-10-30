using System.Collections.Generic;
using System.Threading.Tasks;
using GpsNotebook.Models;

namespace GpsNotebook.Services.Pin
{
    public interface IPinService
    {
        Task<int> AddPinAsync(PinModel pin);

        Task<int> UpdatePinAsync(PinModel pin);

        Task<int> DeletePinAsync(PinModel pin);

        Task<List<PinModel>> GetPinsAsync(string keyWord = default);

        Task<PinModel> GetByLabelAsync(string label);
    }
}
