﻿using System.Collections.Generic;
using System.Threading.Tasks;
using GpsNotebook.Helpers;
using GpsNotebook.Models;
using GpsNotebook.Services.Repository;

namespace GpsNotebook.Services.Pin
{
    public class PinService : IPinService
    {
        private readonly IRepositoryService RepositoryService;

        public PinService(IRepositoryService repositoryService)
        {
            RepositoryService = repositoryService;
            RepositoryService.InitTable<PinModel>();
        }

        #region -- IPinService implementation --

        public async Task<int> AddPinAsync(PinModel pin)
        {
            pin.UserId = Settings.RememberedUserId;
            return await RepositoryService.InsertAsync(pin);
        }

        public async Task<int> DeletePinAsync(PinModel pin)
        {
            return await RepositoryService.DeleteAsync(pin);
        }

        public async Task<PinModel> GetByLabelAsync(string label)
        {
            return await RepositoryService.GetAsync<PinModel>(p => p.UserId == Settings.RememberedUserId
            && p.Label == label);
        }

        public async Task<List<PinModel>> GetPinsAsync(string keyWord = null)
        {
            List<PinModel> result = null;

            if (keyWord == null)
            {
                result = await RepositoryService.GetAllAsync<PinModel>(p => p.UserId == Settings.RememberedUserId);
            }
            else
            {
                result = await RepositoryService.GetAllAsync<PinModel>(p => (p.UserId == Settings.RememberedUserId)
                  && (p.Label.Contains(keyWord) || p.Description.Contains(keyWord)
                  || p.Latitude.Contains(keyWord) || p.Longitude.Contains(keyWord)));
            }

            return result;
        }

        public async Task<int> UpdatePinAsync(PinModel pin)
        {
            return await RepositoryService.UpdateAsync<PinModel>(pin);
        }

        #endregion
    }
}
