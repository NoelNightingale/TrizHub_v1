#region Usings

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Web.Http;
using TRiZHub.BL.Context;
using TRiZHub.BL.Provider.Security;
using TRiZHub.BL.Provider.Settings;
using TRiZHub.BL.Provider.TravelInformationData;
using TRiZHub.Models;
using TRiZHub.Models.TravelInformationModels;

#endregion

namespace TRiZHub.Controllers
{
    public class TravelInformationController : TCRControllerBase
    {
        #region Ctor

        private TravelInformationProvider TravelInformationProvider { get; }
        private SecurityProvider SecurityProvider { get; }


        public TravelInformationController()
        {
            SecurityProvider = new SecurityProvider(Context, CurrentUser);
            TravelInformationProvider = new TravelInformationProvider(Context, CurrentUser);
        }

        public TravelInformationController(IAppSettings settings, DataContext context, ICurrentUser currentUser)
            : base(context, currentUser)
        {
            SecurityProvider = new SecurityProvider(Context, CurrentUser);
            TravelInformationProvider = new TravelInformationProvider(Context, CurrentUser);
        }

        #endregion

        #region Travel Information 

        /// <summary>
        /// Create or update Travel information based on a User
        /// </summary>
        [HttpPost]
        public TravelInformationEditModel SaveTravelInformation(TravelInformationEditModel model)
        {
            try
            {
                CheckModelState();

                if (model.Id == null || model.Id == Guid.Empty)
                {
                    var travelInformation = TravelInformationProvider.GetTravelInformation(model.Id ?? Guid.Empty);
                    if (travelInformation != null)
                        model.Id = travelInformation.Id;
                }

                CheckModelState();

                var record = TravelInformationProvider.SaveTravelInformation(model.Id,
                    model.UserAccountId, model.DocumentType, model.Number,
                    model.ExpiryDate.ToLocalTime());

                var travelInformationReturn = new TravelInformationEditModel
                {
                    Id = record.Id,
                    UserAccountId = record.UserAccountId,
                    DocumentType = record.DocumentType,
                    Number = record.Number,
                    ExpiryDate = record.ExpiryDate
                };

                return travelInformationReturn;
            }
            catch (TravelInfromationException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        /// <summary>
        /// Retrieve single Travel information record based on id
        /// </summary>
        [HttpGet]
        public TravelInformationEditModel TravelInformationGet(Guid id)
        {
            try
            {
                var travelInformation = TravelInformationProvider.GetTravelInformation(id);

                //   var user = SecurityProvider.GetUserList().Where(a => a.Id == id).Single();

                var model = new TravelInformationEditModel
                {
                    Id = travelInformation.Id,
                    UserAccountId = travelInformation.UserAccountId,
                    DocumentType = travelInformation.DocumentType,
                    Number = travelInformation.Number,
                    ExpiryDate = travelInformation.ExpiryDate
                };
                return model;
            }
            catch (TravelInfromationException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        /// <summary>
        /// Retrieve list of Travel Information records sorted by Expiry date 
        /// </summary>
        [HttpPost]
        public GridResultModel<TravelInformationGridModel> TravelInformationGrid(IdGridModel model)
        {
            var begin = SetupGridParams(model);

            var filteredQuery = TravelInformationProvider.TravelInformationFilterList(model.Id ?? Guid.Empty)
                .Select(a => new TravelInformationGridModel
                {
                    Id = a.Id,
                    UserAccountId = a.UserAccountId,
                    DocumentType = a.DocumentType,
                    Number = a.Number,
                    ExpiryDate = a.ExpiryDate
                });


            var totalNumberOfRecords = filteredQuery.Count();

            filteredQuery = filteredQuery.OrderBy(a => a.ExpiryDate);

            var returnList = filteredQuery.ToList();

            return new GridResultModel<TravelInformationGridModel>(returnList, totalNumberOfRecords);
        }

        /// <summary>
        /// Delete Travel Information record based on ID
        /// </summary>
        [HttpPost]
        public void TravelInformationDelete(TravelInformationEditModel model)
        {
            try
            {
                TravelInformationProvider.DeleteTravelInformation(model.Id ?? Guid.Empty);
            }
            catch (TravelInfromationException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        #endregion
    }
}