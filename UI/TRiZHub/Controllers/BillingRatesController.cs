#region Usings

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Web.Http;
using TRiZHub.BL.Context;
using TRiZHub.BL.Provider.BillingRatesData;
using TRiZHub.BL.Provider.Security;
using TRiZHub.BL.Provider.Settings;
using TRiZHub.Models;
using TRiZHub.Models.BillingRatesModels;

#endregion

namespace TRiZHub.Controllers
{
    public class BillingRatesController : TCRControllerBase
    {
        #region Ctor

        private BillingRatesProvider BillingRatesProvider { get; }
        private SecurityProvider SecurityProvider { get; }


        public BillingRatesController()
        {
            SecurityProvider = new SecurityProvider(Context, CurrentUser);
            BillingRatesProvider = new BillingRatesProvider(Context, CurrentUser);
        }

        public BillingRatesController(IAppSettings settings, DataContext context, ICurrentUser currentUser)
            : base(context, currentUser)
        {
            SecurityProvider = new SecurityProvider(Context, CurrentUser);
            BillingRatesProvider = new BillingRatesProvider(Context, CurrentUser);
        }

        #endregion

        #region Billing Rates 

        /// <summary>
        /// Create or update Billing Rate
        /// </summary>
        [HttpPost]
        public BillingRatesEditModel SaveBillingRates(BillingRatesEditModel model)
        {
            try
            {
                CheckModelState();

                if (model.Id == null || model.Id == Guid.Empty)
                {
                    var billingRates = BillingRatesProvider.GetBillingRates(model.Id ?? Guid.Empty);
                    if (billingRates != null)
                        model.Id = billingRates.Id;
                }

               

                var record = BillingRatesProvider.SaveBillingRates(model.Id,
                    model.UserAccountId, model.Rate, model.StartDate.ToLocalTime(),
                    model.EndDate.ToLocalTime());

                var billingRatesReturn = new BillingRatesEditModel
                {
                    Id = record.Id,
                    UserAccountId = record.UserAccountId,
                    Rate = record.Rate,
                    StartDate = record.StartDate,
                    EndDate = record.EndDate,
                };

                return billingRatesReturn;
            }
            catch (BillingRatesException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        /// <summary>
        /// Retrieve single Billing rate based on id
        /// </summary>
        [HttpGet]
        public BillingRatesEditModel BillingRatesGet(Guid id)
        {
            try
            {
                var billingRates = BillingRatesProvider.GetBillingRates(id);

                var model = new BillingRatesEditModel
                {
                    Id = billingRates.Id,
                    UserAccountId = billingRates.UserAccountId,
                    Rate = billingRates.Rate,
                    StartDate = billingRates.StartDate,
                    EndDate = billingRates.EndDate,
                   
                };
                return model;
            }
            catch (SecurityException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        /// <summary>
        /// Retrieve list of Billing rates sorted by Startdate
        /// </summary>
        [HttpPost]
        public GridResultModel<BillingRatesGridModel> BillingRatesGrid(IdGridModel model)
        {
            var begin = SetupGridParams(model);

            var filteredQuery = BillingRatesProvider.BillingRatesFilterList(model.Id ?? Guid.Empty)
                .Select(a => new BillingRatesGridModel
                {
                    Id = a.Id,
                    UserAccountId = a.UserAccountId,
                    Rate = a.Rate,
                    StartDate = a.StartDate,
                    EndDate = a.EndDate,
                   
                });


            var totalNumberOfRecords = filteredQuery.Count();

            filteredQuery = filteredQuery.OrderBy(a => a.StartDate);

            var returnList = filteredQuery.ToList();

            return new GridResultModel<BillingRatesGridModel>(returnList, totalNumberOfRecords);
        }

        /// <summary>
        /// Delete Billing Rate based on id
        /// </summary>
        [HttpPost]
        public void BillingRatesDelete(BillingRatesEditModel model)
        {
            try
            {
                BillingRatesProvider.DeleteBillingRatesEntry(model.Id ?? Guid.Empty);
            }
            catch (BillingRatesException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        #endregion
    }
}